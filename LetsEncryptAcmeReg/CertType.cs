using System;

namespace LetsEncryptAcmeReg
{
    [Flags]
    public enum CertType
    {
        KeyPEM = 1,
        CsrPEM = 2,
        CertificatePEM = 4,
        CertificateDER = 8,
        IssuerPEM = 16,
        IssuerDER = 32,
        Pkcs12 = 64,
    }
}