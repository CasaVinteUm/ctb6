namespace CTB6.Souls.Judy;

using NBitcoin;

internal class Program
{
    private static readonly Mnemonic _mnemonic = new("find all seed word they work when combine then become lucky tonight");
    private static readonly string _passphrase = "tabconfisgoat";
    private static readonly KeyPath _derivationPath = new("m/86'/0'/0'/0/0");
    private static readonly string _targetAddress = "bc1pnre2d0g2caks4745xa6q2cqh9j39z0s0fgk7w9crvwtk28lskm5qxwy4d9";
    private static readonly Network _network = Network.Main;
    private static readonly ScriptPubKeyType _type = ScriptPubKeyType.TaprootBIP86;

    public static void Main(string[] args)
    {
        var node = _mnemonic.DeriveExtKey(_passphrase);
        var childNode = node.Derive(_derivationPath);
        var computedAddress = childNode.PrivateKey.GetAddress(_type, _network).ToString();

        if (!computedAddress.Equals(_targetAddress))
        {
            throw new ApplicationException("Address mismatch");
        }

        var publicKey = childNode.PrivateKey.PubKey;
        var privateKeyWIF = childNode.PrivateKey.GetWif(_network);

        Console.WriteLine("### Judy");
        Console.WriteLine($"Derivation Path:\t{_derivationPath}");
        Console.WriteLine($"Address:\t\t{computedAddress}");
        Console.WriteLine($"Mnemonic:\t\t{_mnemonic}");
        Console.WriteLine($"Passphrase:\t\t{_passphrase}");
        Console.WriteLine($"Public Key:\t\t{publicKey}");
        Console.WriteLine($"Private Key:\t\t{privateKeyWIF}");
    }
}