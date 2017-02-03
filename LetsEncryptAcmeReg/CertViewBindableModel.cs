using ACMESharp.Vault.Model;

namespace LetsEncryptAcmeReg
{
    public class CertViewBindableModel
    {
        // regex for valid lines in this file:
        // public Bindable(<(?><(?<c>)|[^<>)]+|(?:>|$)(?<-c>))*(?:>|$))\s(\S*)\s\{ get; \} = new Bindable\1\(nameof\(\2\)(?:[^\)]*)\);

        public Bindable<string[]> Certificates { get; } = new Bindable<string[]>(nameof(Certificates));
        public Bindable<string> Certificate { get; } = new Bindable<string>(nameof(Certificate), flags: BindableOptions.EqualMeansUnchanged);
        public Bindable<CertificateInfo> CurrentCertificate { get; } = new Bindable<CertificateInfo>(nameof(CurrentCertificate));
        public Bindable<AcmeTextAssets> TextAssets { get; } = new Bindable<AcmeTextAssets>(nameof(TextAssets));
        public Bindable<CertType> CertificateType { get; } = new Bindable<CertType>(nameof(CertificateType));
        public Bindable<string> Base64Data { get; } = new Bindable<string>(nameof(Base64Data));
    }
}