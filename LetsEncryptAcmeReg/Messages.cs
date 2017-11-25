using System.Collections.Generic;

namespace LetsEncryptAcmeReg
{
    public static class Messages
    {
        private static Dictionary<string, string> dicMsgExplain;

        public static string MultipleIdentities { get; }
            = "Multiple identitiers with the same domain were found.\n" +
              "In this case, the first one will be used.";

        public static string DomainNotFound { get; }
            = "Cannot find the specified domain.";

        public static string CannotCreateIdentifier_DomainAlreadyUsed { get; }
            = "Cannot create identifier. The alias or domain is already used by another identifier.";

        public static string CannotCreateIdentifier_NoRegistrationProvided { get; }
            = "Cannot create identifier. Registration argument is null.";

        public static string ToolTipForConfigYml { get; }
            = "Updates or creates a ` _config.yml ` file,\n" +
              "with instructions to _**not ignore**_ the\n" +
              "path ` .well-known\\ `.";

        public static string ToolTipForCname { get; }
            = "Updates or creates a ` CNAME ` file,\n" +
              "with the name of the ***selected domain***.";

        public static string ToolTipForConfPy { get; }
            = "Updates the ` conf.py ` file,\n" +
              "with instructions to _**copy**_\n" +
              "the path ` .well-known\\ ` to the\n" +
              "output directory, so that\n" +
              "it is served by Nikola.";

        public static string ToolTipForUpdateStatus { get; }
            = "Updates the status of the current domain after issuing the validate command for LetsEncrypt.\n" +
              "If LetsEncrypt finds that the domain failed the validation process, then a fail message is returned when updating.\n" +
              "Though this is intended to be used after the validation command is issued, there is no harm in using it before.\n" +
              "In this case, LetsEncrypt will simply state that validation command is pending.";

        public static string ToolTipForChallengeTarget { get; }
            = "URL that Let's Encrypt will look for when validating the challenge.\n" +
              "It must contain the challenge key data to prove that the site is yours.";

        public static string ToolTipForChallengeKey { get; }
            = "Key data that must be found at the challenge URL.\n" +
              "This is the proof that the site is yours.";

        public static string ToolTipForSsg { get; }
            = "Static site generator being used to build your site.\n" +
              "Each SSG has it's own settings on how to expose the\n" +
              "challenge data publicly (so that Let's Encrypt can see it).\n" +
              "** Manual **: this option lets you setup your SSG manually.\n" +
              "  This is the case if the SSG is not in the list, or if you\n" +
              "  feel that you should do it yourself. Maybe your settings\n" +
              "  file is too much modified to be updated automatically.";

        public static string ToolTipForSaveChallenge { get; }
            = "Saves all the files needed to fulfill the challenge, including needed directories.\n" +
              "The files are the main key file and directory, and also settings files for the given\n" +
              "static site generator (if one is selected).";

        public static string ToolTipForCommitChallenge { get; }
            = "If a GIT repository is detected at your site's location,\n" +
              "you can use this button to automatically create a commit\n" +
              "with the text `\"Let's Encrypt HTTP challenge files.\"`,\n" +
              "and then push the changes back to the origin using the given\n" +
              "GIT credentials.";

        public static string CommitMessage { get; } = "Let's Encrypt HTTP challenge files.";

        public static string ToolTipForTestChallenge { get; }
            = "Before asking Let's Encrypt to validate your site,\n" +
              "you should test to see if the file is really visible.\n" +
              "Let's Encrypt won't allow you to reuse a failed challenge.";

        public static string ToolTipForValidate { get; }
            = "Asks Let's Encrypt to validate your site by looking at the exposed key file.\n" +
              "The file must be there, otherwise the challenge fails.";

        public static string ToolTipForRegister { get; }
            = "Creates a new registration at **LetsEncrypt.com** using the given e-mail.";

        public static string ToolTipForAcceptTos { get; }
            = "After reading the Terms of Service, you may accept it to continue.";

        public static string ToolTipForAddDomain { get; }
            = "Adds a new domain (an identity) to the current registration.";

        public static string ToolTipForDomain { get; }
            = "The domain name (the identity) as given for DNS to resolve.\n" +
              "This does not include slashes ` \"/\" ` nor protocols like ` http `,\n" +
              "only the domain (e.g. *www.masbicudo.com*).\n" +
              "You can add multiple domains under a single registration.";

        public static string ToolTipForGitLabCertHelp { get; }
            = "GitLab has the option to use your own certificate when using a custom domain.\n" +
              "\n" +
              "They ask for the **Certificate (PEM)** and the **Key (PEM)**.\n" +
              "\n" +
              "The certificate PEM they ask is in fact two PEMs concatenated:\n" +
              " - ` Certificate PEM `\n" +
              " - ` Issuer PEM `\n" +
              "You just have to copy all the text from the above text-box, for both of these\n" +
              "options, and concatenate them in the order given above.\n" +
              "\n" +
              "On the other hand, there is no mystery with the key PEM.\n" +
              "Just copy the whole base64 text given by the ` Key PEM ` option.";

        public static string SuccessGetIssuerCert { get; }
            = "The certificate information was generated by LetsEncrypt.com and stored.";

        public static string IssuerSerialNumberNotSet { get; }
            = "IssuerSerialNumber was not set.";

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
                        CannotCreateIdentifier_DomainAlreadyUsed,

                        "There is another registration that alrady contains the domain you are trying to use.\n" +
                        "This is not an error, but can be a source of confusion when creating certificates."
                    },
                    {
                        "Too many redirects or authentication replays",

                        "This could mean that either your git username or git password is wrong."
                    }
                };

            string result;
            dicMsgExplain.TryGetValue(message, out result);

            return result;
        }
    }
}