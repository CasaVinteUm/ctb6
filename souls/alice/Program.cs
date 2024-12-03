namespace CTB6.Souls.Alice;

using NBitcoin;

internal class Program
{
    private static readonly string _xpriv =
        "xprv9s21ZrQH143K3kVxaer4FSKkM5W398t9VmMGLxmedqb8Ndw9N6rfcTHAfUyAYJ9o12TMQX7jd2Z6xGpuWBeBz4SfbCJP7tdvVsPkizKBtG2";
    private static readonly KeyPath _derivationPath = new("m/86'/0'/0'/0/0");
    private static readonly string _targetAddress = "bc1pw8d5j3vecstsc9kpahpdfy4xfuct6xn7cqeaxsk6w2ylz63s850q7sragu";
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

        Console.WriteLine("### Alice");
        Console.WriteLine($"Derivation Path:\t{_derivationPath}");
        Console.WriteLine($"Address:\t\t{computedAddress}");
        Console.WriteLine($"Public Key:\t\t{publicKey}");
        Console.WriteLine($"Private Key:\t\t{privateKeyWIF}");
    }
}
