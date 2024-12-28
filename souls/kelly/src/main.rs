use bip39::Mnemonic;
use bitcoin::absolute::LockTime;
use bitcoin::bip32::{DerivationPath, Xpriv, Xpub};
use bitcoin::consensus::{deserialize, serialize};
use bitcoin::hex::{Case, DisplayHex, FromHex};
use bitcoin::{secp256k1, transaction, Network, OutPoint, PrivateKey, Psbt, Script, Sequence, Transaction, TxIn, TxOut};
use miniscript::psbt::PsbtExt;
use miniscript::{DefiniteDescriptorKey, Descriptor, DescriptorPublicKey};
use std::str::FromStr;
use bitcoin::hashes::{sha256, Hash};

fn main() {
    let secp: &secp256k1::Secp256k1<secp256k1::All> = &secp256k1::Secp256k1::new();
    let network = Network::Bitcoin;
    let path = "86'/0'/0'";

    let internal_public_key = "xpub6DP7ebuRrfK2QYDecC1MrCzkkyi9jh26uNBVhsoTasNitJ94FC3ZuvkQyeLr9qaFzepVp4e5uzLXexMBiKjVrd6nwVEpeSXLbRFkPM4SZyG";
    let seed = Mnemonic::from_str("fetch print airport upon jazz run loyal live creek oblige inflict tent").unwrap().to_seed("");
    let master_key = Xpriv::new_master(network, &seed).unwrap();
    let extended_private_key = master_key.derive_priv(&secp, &path.parse::<DerivationPath>().unwrap()).unwrap();
    let extended_public_key = Xpub::from_priv(&secp, &extended_private_key);
    let (preimage, digest) = produce_preimage("the ultimate pre-preimage");

    let descriptor_string = format!("
        tr(
            {internal_public_key}/0/0,
            and_v(
                v:pk([00000000/{path}]{extended_public_key}/0/*),
                sha256({digest})
            )
        )
    ").replace(&[' ', '\n', '\t'][..], "");

    let descriptor = Descriptor::<DescriptorPublicKey>::from_str(&descriptor_string).unwrap();
    let derived_descriptor = descriptor.derived_descriptor(&secp, 0).unwrap();

    assert!(derived_descriptor.sanity_check().is_ok());
    println!("DESCRIPTOR = {}", derived_descriptor);

    let address = derived_descriptor
        .address(network)
        .unwrap();

    assert_eq!(&address.to_string(), "bc1pkxpc2h8y9k72r868a5v93mlgxt74jcgge4s84kwm7d6uqfaj64hsu6m5np");
    println!("ADDRESS = {}", address);

    // create the psbt

    let previous_tx_hex = "02000000000101dd53800b097d9ede45f293418b7a98cd4365c8d5e502d47ecb1120c7285983320000000000fdffffff0de09304000000000022512071db494599c4170c16c1edc2d492a64f30bd1a7ec033d342da7289f16a303d1ee0930400000000002251202bf9b66e7d77c6a8a9f228539a54397e88d92738e50eeeeb96738c4eca006c51e093040000000000225120327e026cc17fd5974934b12133f4968d64ee29e1011f5a1169de1420d52d0500e0930400000000002251209242042f825a70e9e4f531b2a2d4d3eec37e79779510c8b15b09e511a3a06a91e093040000000000225120b13135bf4c150952bd20d30dfb0bdd7e9e60f3ff90ef1e4545cdfdec66571876e093040000000000225120408e8b73b811e149b9832637a843e6a727cf041b1110aaa39274ee12962e618be093040000000000225120292ebbfc02ff7d3866ee2a6c6a2654b3425edf7647f509dc4867ff4311aa63abe09304000000000022512096741b004d8b4c256b8b9bf9cc8f6ded0d4c2fbe39ec0d4a795d5c6862f4988e880c0b0000000000225120f0ff1ba1866f9655071731ea1e3e850119d97ac1e898719c79249896213b57f9e09304000000000022512098f2a6bd0ac76d0afab437740560172ca2513e0f4a2de717036397651ff0b6e8e093040000000000225120b183855ce42dbca19f47ed1858efe832fd596108cd607ad9dbf375c027b2d56fe09304000000000022512094ba57cf68c6bc9d9193b1a657be700f607abc2020e8c479b3e4b36edec11f7500093d0000000000225120e9ec3a06ef68efecd72527c88663fa26133e6d43b4cbaff744d861857546241e0140bd2c0b672f7b6b5379eb71438438987f59fc93994995c83018aa1579c5bc1c8c9ef64b0faccf308ad32520352a37c1c0df51f7ddba9cc40f00506c502766373900000000";
    let previous_tx: Transaction = deserialize(&Vec::from_hex(previous_tx_hex).unwrap()).unwrap();
    let (outpoint, witness_utxo) = get_vout(&previous_tx, &derived_descriptor.script_pubkey());

    assert_eq!(&outpoint.to_string(), "f003ac8f3edda413fad73bc27b9f1bb8271a9a6528fd17c3365e3d22b37324a0:10");

    let destination_address = bitcoin::Address::from_str("tb1ph2ac76y93lfqthj7d7q5l4ck2qg4swt3yt0mqa8w78a8h0aygfcqdta5za").unwrap().assume_checked();

    let txin = TxIn {
        previous_output: outpoint,
        sequence: Sequence::ENABLE_RBF_NO_LOCKTIME,
        ..Default::default()
    };

    let txout = TxOut {
        script_pubkey: destination_address.script_pubkey(),
        value: bitcoin::Amount::from_sat(300_000 - 1337),
    };

    let unsigned_tx = Transaction {
        version: transaction::Version::TWO,
        lock_time: LockTime::ZERO,
        input: vec![txin],
        output: vec![txout],
    };

    println!("UNSIGNED TX = {}", serialize(&unsigned_tx).to_hex_string(Case::Lower));

    let mut psbt = Psbt::from_unsigned_tx(unsigned_tx).unwrap();

    // sign

    psbt.inputs[0].witness_utxo = Some(witness_utxo);
    let desc = Descriptor::<DefiniteDescriptorKey>::from_str(&derived_descriptor.to_string()).unwrap();
    psbt.update_input_with_descriptor(0, &desc).unwrap();
    let key_to_sign = &master_key.derive_priv(&secp, &"86'/0'/0'/0/0".parse::<DerivationPath>().unwrap()).unwrap();
    psbt.sign(key_to_sign, secp).unwrap();

    psbt.inputs[0]
        .sha256_preimages
        .insert(digest, preimage.to_byte_array().to_vec());

    psbt.finalize_mut(&secp).unwrap();

    let signed_tx = psbt.extract_tx().unwrap();
    let raw_tx = serialize(&signed_tx).to_hex_string(Case::Lower);

    println!("SIGNED TX = {}", raw_tx);
    println!("PREIMAGE = {}", preimage);
    let wif = PrivateKey::new(key_to_sign.private_key, network).to_wif();
    println!("PRIVATE KEY WIF = {}", wif);
}

fn get_vout(tx: &Transaction, spk: &Script) -> (OutPoint, TxOut) {
    for (i, txout) in tx.clone().output.into_iter().enumerate() {
        if spk == &txout.script_pubkey {
            return (OutPoint::new(tx.compute_txid(), i as u32), txout);
        }
    }
    panic!("Only call get vout on functions which have the expected outpoint");
}

fn produce_preimage(secret: &str) -> (sha256::Hash, sha256::Hash) {
    let preimage = sha256::Hash::hash(secret.as_bytes());
    let digest = sha256::Hash::hash(&preimage.to_byte_array());
    (preimage, digest)
}