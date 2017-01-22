namespace LetsEncryptAcmeReg
{
    public static class Messages
    {
        public static readonly string MultipleIdentities =
            "Multiple identitiers with the same domain were found.\nIn this case, the first one will be used.";

        public static readonly string DomainNotFound =
            "Cannot find the specified domain.";
    }
}