namespace R.Systems.Auth.SharedKernel.Interfaces
{
    public interface IRsaKeys
    {
        public string? PublicKey { get; init; }

        public string? PrivateKey { get; init; }
    }
}
