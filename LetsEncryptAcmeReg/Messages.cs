using System.Collections.Generic;

namespace LetsEncryptAcmeReg
{
    public static class Messages
    {
        private static Dictionary<string, string> dicMsgExplain;

        public static string MultipleIdentities { get; }
            = "Multiple identitiers with the same domain were found.\nIn this case, the first one will be used.";

        public static string DomainNotFound { get; }
            = "Cannot find the specified domain.";

        public static string CannotCreateIdentifierDomainAlreadyUsed { get; }
            = "Cannot create identifier. The alias or domain is already used by another identifier.";

        public static string ToolTipForConfigYml { get; }
            = "Updates or creates a ` _config.yml ` file,\r\nwith instructions to _**not ignore**_ the\r\npath ` .well-known\\ `.";

        public static string ToolTipForCname { get; }
            = $"Updates or creates a ` CNAME ` file,\r\nwith the name of the ***selected domain***.";

        public static string ToolTipForUpdateStatus { get; }
            = "Updates the status of the current domain after issuing the validate command for LetsEncrypt.\n" +
            "If LetsEncrypt finds that the domain failed the validation process, then a fail message is returned when updating.\n" +
            "Though this is intended to be used after the validation command is issued, there is no harm in using it before.\n" +
            "In this case, LetsEncrypt will simply state that validation command is pending.";

        public static string MapMessageToExplanation(string message)
        {
            if (dicMsgExplain == null)
                dicMsgExplain = new Dictionary<string, string>
                {
                    {
                        "Unable to find an Identifier for the given reference",
                        "This means that LetsEncrypt service didn't find the specified domain in it's database.\n" +
                        "This can happen if you created a challenge but didn't validate it before expiration.\n" +
                        "If this is the case, you need to create a new challenge."
                    },
                    {
                        CannotCreateIdentifierDomainAlreadyUsed,
                        "There is another registration that alrady contains the domain you are trying to use.\n" +
                        "This is not an error, but can be a source of confusion when creating certificates."
                    },
                };

            string result;
            dicMsgExplain.TryGetValue(message, out result);

            return result;
        }
    }
}