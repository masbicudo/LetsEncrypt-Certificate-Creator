using JetBrains.Annotations;

namespace LetsEncryptAcmeReg
{
    public class AcmeTextAssets
    {
        public string KeyPem { get; set; }
        public string CsrPem { get; set; }
        public string CrtPem { get; set; }
        public string CrtDer { get; set; }
        public string IssuerPem { get; set; }
        public string IssuerDer { get; set; }

        [CanBeNull]
        public string GetAsset(CertType certType)
        {
            switch (certType)
            {
                case CertType.KeyPEM: return this.KeyPem;
                case CertType.CsrPEM: return this.CsrPem;
                case CertType.CertificatePEM: return this.CrtPem;
                case CertType.IssuerPEM: return this.IssuerPem;
                default: return null;
            }
        }
    }
}