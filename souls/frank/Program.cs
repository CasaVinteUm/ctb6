namespace CTB6.Souls.Frank;

using NBitcoin;

internal class Program
{
    private static readonly Mnemonic _mnemonic = new("please remember forget nothing need find all seed word they work okay");
    private static readonly KeyPath _derivationPath = new("m/86'/0'/0'/0/0");
    private static readonly string _targetAddress = "bc1pgz8gkuacz8s5nwvrycm6sslx5unu7pqmzyg24gujwnhp993wvx9sjv6xtu";
    private static readonly Network _network = Network.Main;
    private static readonly ScriptPubKeyType _type = ScriptPubKeyType.TaprootBIP86;

    public static void Main(string[] args)
    {
        var node = _mnemonic.DeriveExtKey();
        var childNode = node.Derive(_derivationPath);
        var computedAddress = childNode.PrivateKey.GetAddress(_type, _network).ToString();

        if (!computedAddress.Equals(_targetAddress))
        {
            throw new ApplicationException("Address mismatch");
        }

        var publicKey = childNode.PrivateKey.PubKey;
        var privateKeyWIF = childNode.PrivateKey.GetWif(_network);

        Console.WriteLine("### Frank");
        Console.WriteLine($"Derivation Path:\t{_derivationPath}");
        Console.WriteLine($"Address:\t\t{computedAddress}");
        Console.WriteLine($"Mnemonic:\t\t{_mnemonic}");
        Console.WriteLine($"Public Key:\t\t{publicKey}");
        Console.WriteLine($"Private Key:\t\t{privateKeyWIF}");
    }
}
