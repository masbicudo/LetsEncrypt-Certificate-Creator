namespace LetsEncryptAcmeReg
{
    public class ManagerBindableModel
    {
        // regex for valid lines in this file:
        // public Bindable(<(?><(?<c>)|[^<>)]+|(?:>|$)(?<-c>))*(?:>|$))\s(\S*)\s\{ get; \} = new Bindable\1\(nameof\(\2\)(?:[^\)]*)\);

        public Bindable<string> Challenge { get; } = new Bindable<string>(nameof(Challenge));
        public Bindable<string[]> Challenges { get; } = new Bindable<string[]>(nameof(Challenges));
        public Bindable<string[]> Certificates { get; } = new Bindable<string[]>(nameof(Certificates));
    }
}