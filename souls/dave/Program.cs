namespace CTB6.Souls.Dave;

using NBitcoin;

internal class Program
{
    private static readonly string _xpriv =
        "xprv9s21ZrQH143K4PBx8nb1j4BEpSt7cbBdjzqqZbYcbRREfCxzAqdnxrDRD9kQv8VudFo1BavMT8s7Lq8HH8VwSZAxyGrFoHTC5fF2LZMs8J4";
    private static readonly KeyPath _derivationPath = new("m/69'/420'/999999999'/8008135'/0");
    private static readonly string _targetAddress = "bc1pjfpqgtuztfcwne84xxe294xnamphu7thj5gv3v2mp8j3rgaqd2gsnydssw";
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

        Console.WriteLine("### Dave");
        Console.WriteLine($"Derivation Path:\t{_derivationPath}");
        Console.WriteLine($"Address:\t\t{computedAddress}");
        Console.WriteLine($"Public Key:\t\t{publicKey}");
        Console.WriteLine($"Private Key:\t\t{privateKeyWIF}");
    }
}
