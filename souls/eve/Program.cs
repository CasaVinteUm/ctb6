namespace CTB6.Souls.Eve;

using NBitcoin;

internal class Program
{
    private static readonly string _xpriv =
        "xprv9s21ZrQH143K2iyZFCLtQ157aER7Rt8THLvBjZCDp6sY5FYv6EwcBDGRAYNaMvAyQJpwXWzQg7g9ZtVpFj8D2u7mDD7coEUqmEWYJWdqVqd";
    private static readonly KeyPath _derivationPath = new("m/86'/0'/0'/0/0");
    private static readonly string _targetAddress = "bc1pkycnt06vz5y490fq6vxlkz7a060xpulljrh3u329eh77cejhrpmqvda0ff";
    private static readonly Network _network = Network.Main;
    private static readonly ScriptPubKeyType _type = ScriptPubKeyType.TaprootBIP86;

    public static void Main(string[] args)
    {
        var node = ExtKey.Parse(_xpriv, _network);
        var childNode = node.Derive(_derivationPath);
        var computedAddress = childNode.PrivateKey.GetAddress(_type, _network).ToString();

        if (!computedAddress.Equals(_targetAddress))
        {
            throw new ApplicationException("Address mismatch");
        }

        var publicKey = childNode.PrivateKey.PubKey;
        var privateKeyWIF = childNode.PrivateKey.GetWif(_network);

        Console.WriteLine("### Eve");
        Console.WriteLine($"Derivation Path:\t{_derivationPath}");
        Console.WriteLine($"Address:\t\t{computedAddress}");
        Console.WriteLine($"Public Key:\t\t{publicKey}");
        Console.WriteLine($"Private Key:\t\t{privateKeyWIF}");
    }
}