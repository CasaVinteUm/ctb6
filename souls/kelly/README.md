# Keyless Kelly

> "I was working on some interesting Taproot encumbrances when I heard voices. Slowly, I was led down a path and coaxed into thinking the Taproot tree wanted to share its knowledge with me, but instead, it took my hash and used it as part of its own wicked scheme. I'll do you a favor,.. `here is my seed phrase, used in conjunction with a hash`. Free me first, and then reuse the hash to help destroy this tree once and for all."
```
"fetch print airport upon jazz run loyal live creek oblige inflict tent"

sha256 hashes of this phrase should get you the preimage you need: 'the ultimate pre-preimage'

kelly taptee descriptor: tr([d932f4a5/86'/0'/0']xpub6DP7ebuRrfK2QYDecC1MrCzkkyi9jh26uNBVhsoTasNitJ94FC3ZuvkQyeLr9qaFzepVp4e5uzLXexMBiKjVrd6nwVEpeSXLbRFkPM4SZyG/0/*,and_v(v:pk([0ae8c905/86'/0'/0']xpub6CGL4xcGvqEpkzWPBfUraPFX9kqv2QdKMz8XTbMr6tzqBLqWpPGLfT577BNEYYiTo2MkruYSqW3vz49Vb9FyJiov7R9CUw22xLvc4c969XM/0/*),sha256(2db9cdb5e102541f19b455fa798e0cb009f5faa6358b9d3507858caf797bca41)))#ar4a5hy9, 86'/0'/0'/0/*
```

## Solution

```
Derivation Path:        86'/0'/0'/0/0
Address:                bc1pkxpc2h8y9k72r868a5v93mlgxt74jcgge4s84kwm7d6uqfaj64hsu6m5np
Public Key:             03beaa5391df0d7156aeb68bd22372bf789f292a4c05ffa7711b5ae07bc0051f3c
Private Key:            L2SACJTsnT5gqJ4cKUJPSk3fvJFPadGjRxsoHJoTszZwXmpdhxCa
Preimage:               9562ef4e826d891eaa72f2cee753b80a3f7f6b5aed07b850227e83546fa61857
```

## Notes

- We noticed that the internal key for this soul is actually Frank's xpub. Therefore, we signed with it on the mainnet.
- The following Rust code demonstrates the correct approach for performing a script path spend if Tidwell were to use a NUMS key as the internal key. [Here](https://mempool.space/signet/tx/fc57c30ac10e9b1635eb8b92ea7332008eb7b2dc6d0678df4b776502667a8b37) is an example of a key-path spend on signet.
