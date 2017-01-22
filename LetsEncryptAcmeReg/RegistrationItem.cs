using System;
using System.Linq;
using ACMESharp.Vault.Model;

namespace LetsEncryptAcmeReg
{
    public class RegistrationItem :
        IEquatable<RegistrationInfo>,
        IEquatable<RegistrationItem>
    {
        public RegistrationInfo RegistrationInfo { get; }

        public RegistrationItem(RegistrationInfo registrationInfo)
        {
            this.RegistrationInfo = registrationInfo;
        }

        public bool Equals(RegistrationInfo other)
        {
            if (other == null) return this.RegistrationInfo == null;
            if (this.RegistrationInfo == null) return false;
            return other.Id == this.RegistrationInfo.Id;
        }

        public bool Equals(RegistrationItem other)
        {
            if (other?.RegistrationInfo == null) return this.RegistrationInfo == null;
            if (this.RegistrationInfo == null) return false;
            return other.RegistrationInfo.Id == this.RegistrationInfo.Id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as RegistrationInfo)
                   || this.Equals(obj as RegistrationItem)
                   || base.Equals(obj);
        }

        public override string ToString()
        {
            return string.Join("; ", this.RegistrationInfo.Registration.Contacts.Select(x => x?.Substring(7)));
        }

        public static explicit operator RegistrationInfo(RegistrationItem reg)
        {
            return reg.RegistrationInfo;
        }

        public static explicit operator RegistrationItem(RegistrationInfo reg)
        {
            return new RegistrationItem(reg);
        }
    }
}