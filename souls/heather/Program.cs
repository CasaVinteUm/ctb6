using System.Text;

namespace CTB6.Souls.Heather;

using NBitcoin;

internal class Program
{
    private static readonly HashSet<string> _hashes =
    [
        "2cfe48324ef1f79c1f223144fd205d6121098174",
        "c3b7b000c884aebc2001f6c1c2fd5635fdb1ab39",
        "2f697b5ae96d29247b0c86ae6ed14469a2048af9",
        "3b41d0421a2031aa09659936e661e1846c0d52db",
        "7330c2973e5d103252a3b9726c3c41cb39ad8d24",
        "6fb82d56ec1eb7522c3ceb046c00b4a2bdac9f34",
        "5cd535886195b9398d1f6114f57899d34561b073",
        "628e5baab7dc469d50cdd462c4d093e582778616",
        "57a2bf66a9473b089819ed7083170ef2ce89b43e",
        "6e81b5225e89fbcfb9a456ed595e022b976feb79",
        "2d07ad3af94dc29d561995ea83c1e8c03a6a02ea",
        "0d97b878c1d9846ce80364786ed2af5d200610d0"
    ];
    private static readonly KeyPath _derivationPath = new("m/86'/0'/0'/0/0");
    private static readonly string _targetAddress = "bc1p9yhthlqzla7nsehw9fkx5fj5kdp9ahmkgl6snhzgvll5xyd2vw4slwnk58";
    private static readonly Network _network = Network.Main;
    private static readonly ScriptPubKeyType _type = ScriptPubKeyType.TaprootBIP86;

    public static void Main(string[] args)
    {
        var bip39Words = Wordlist.English.GetWords();
        var hashedWords = new List<(string word, string hash)>();

        foreach (var word in bip39Words)
        {
            var bytes = Encoding.UTF8.GetBytes(word);
            var sha256 = NBitcoin.Crypto.Hashes.SHA256(bytes);
            var ripemd160 = NBitcoin.Crypto.Hashes.RIPEMD160(sha256);
            var hashedWord = BitConverter.ToString(ripemd160).Replace("-", "").ToLower();
            hashedWords.Add((word, hashedWord));
        }

        var foundWords = hashedWords
            .Where(wh => _hashes.Contains(wh.hash))
            .OrderBy(wh => _hashes.ToList().IndexOf(wh.hash))
            .ToList();

        if (foundWords.Count != _hashes.Count)
        {
            throw new ApplicationException("Words not found");
        }

        var computedMnemonic = string.Join(" ", foundWords.Select(wh => wh.word));

        var mnemonic = new Mnemonic(computedMnemonic);
        var node = mnemonic.DeriveExtKey();
        var childNode = node.Derive(_derivationPath);
        var computedAddress = childNode.PrivateKey.GetAddress(_type, _network).ToString();

        if (!computedAddress.Equals(_targetAddress))
        {
            throw new ApplicationException("Address mismatch");
        }

        var publicKey = childNode.PrivateKey.PubKey;
        var privateKeyWIF = childNode.PrivateKey.GetWif(_network);

        Console.WriteLine("### Heather");
        Console.WriteLine($"Derivation Path:\t{_derivationPath}");
        Console.WriteLine($"Address:\t\t{computedAddress}");
        Console.WriteLine($"Mnemonic:\t\t{computedMnemonic}");
        Console.WriteLine($"Public Key:\t\t{publicKey}");
        Console.WriteLine($"Private Key:\t\t{privateKeyWIF}");
    }
}
