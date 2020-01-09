using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace AccountManager.Infrastructure.Ldap
{
    public class CustomLdapMembershipProvider : MembershipProvider
    {
        private const int LdapErrorInvalidCredentials = 0x31;

        private string _ldapServerUrl;
        private string _ldapBaseDn;
        private string _ldapAdmin;
        private string _ldapAdminPassword;
        private int _ldapPort;
        private bool _enableSsl;

        public CustomLdapMembershipProvider()
        {
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            _ldapServerUrl = GetConfigValue(config["ldapServerUrl"], null);
            _ldapBaseDn = GetConfigValue(config["ldapBaseDn"], null);
            _ldapPort = Convert.ToInt32(GetConfigValue(config["ldapPort"], "389"));
            _enableSsl = Convert.ToBoolean(GetConfigValue(config["ldapUseSsl"], "false"));

            _ldapAdmin = GetConfigValue(config["ldapAdmin"], null);
            _ldapAdminPassword = GetConfigValue(config["ldapAdminPassword"], null);
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
            bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new System.NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion,
            string newPasswordAnswer)
        {
            throw new System.NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new System.NotImplementedException();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new System.NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new System.NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            try
            {
                LdapDirectoryIdentifier ldapDirectoryIdentifier = new LdapDirectoryIdentifier(_ldapServerUrl, _ldapPort);
                NetworkCredential credentials = new NetworkCredential(string.Format("uid={0},{1}", username, _ldapBaseDn), password);
                using (LdapConnection ldapConnection = new LdapConnection(ldapDirectoryIdentifier, credentials, AuthType.Basic))
                {
                    ldapConnection.SessionOptions.SecureSocketLayer = false;
                    ldapConnection.SessionOptions.ProtocolVersion = 3;
                    ldapConnection.Bind();
                }

                return true;
            }
            catch (LdapException ldapException)
            {
                if (ldapException.ErrorCode.Equals(LdapErrorInvalidCredentials)) return false;
                throw;
            }
        }

        public override bool UnlockUser(string userName)
        {
            throw new System.NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new System.NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {

            var connection = GetLdapConnection();

            var distinguishedName = $"uid={username},{_ldapBaseDn}";
            SearchRequest searchRequest = new SearchRequest
            {
                DistinguishedName = distinguishedName
            };

            SearchResponse response = connection.SendRequest(searchRequest) as SearchResponse;

            if (response == null || response.Entries.Count == 0) return null;

            var entry = response.Entries[0];

            var user = new LdapMembershipUser(
                this.Name,
                username,
                GetEntryAttributeValue(entry, "uidnumber"),
                GetEntryAttributeValue(entry, "mail"),
                null,
                true,
                false,
                DateTime.MinValue,
                DateTime.Now,
                DateTime.Now,
                DateTime.MinValue,
                DateTime.MinValue)
            {
                Name = GetEntryAttributeValue(entry, "cn")
            };

            return user;
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new System.NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new System.NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new System.NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new System.NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new System.NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new System.NotImplementedException();
        }

        public override bool EnablePasswordRetrieval { get; }
        public override bool EnablePasswordReset { get; }
        public override bool RequiresQuestionAndAnswer { get; }
        public override string ApplicationName { get; set; }
        public override int MaxInvalidPasswordAttempts { get; }
        public override int PasswordAttemptWindow { get; }
        public override bool RequiresUniqueEmail { get; }
        public override MembershipPasswordFormat PasswordFormat { get; }
        public override int MinRequiredPasswordLength { get; }
        public override int MinRequiredNonAlphanumericCharacters { get; }
        public override string PasswordStrengthRegularExpression { get; }

        private LdapConnection GetLdapConnection()
        {
            var ldapDirectoryIdentifier = new LdapDirectoryIdentifier(_ldapServerUrl, _ldapPort);

            var connection = new LdapConnection(ldapDirectoryIdentifier);

            connection.SessionOptions.SecureSocketLayer = _enableSsl;
            connection.SessionOptions.ProtocolVersion = 3;
            connection.AuthType = AuthType.Basic;
            connection.Credential = new NetworkCredential(_ldapAdmin, _ldapAdminPassword);

            return connection;
        }

        private string GetEntryAttributeValue(SearchResultEntry entry, string attributeName, string separator = null)
        {
            if (entry.Attributes[attributeName] == null)
                return null;

            var values = new List<string>();
            for (var index = 0; index < entry.Attributes[attributeName].Count; index++)
            {
                values.Add(entry.Attributes[attributeName][index].ToString());
            }

            return string.Join(separator ?? " ", values);
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }

            return configValue;
        }
    }

    public class LdapMembershipUser : MembershipUser
    {
        public LdapMembershipUser(
                string providerName,
                string userName,
                string providerUserKey,
                string email,
                string comment,
                bool isApproved,
                bool isLockedOut,
                DateTime creationDate,
                DateTime lastLoginDate,
                DateTime lastActivityDate,
                DateTime lastPasswordChangedDate,
                DateTime lastLockoutDate) :
            base(
                providerName,
                userName,
                providerUserKey,
                email,
                null,
                comment,
                isApproved,
                isLockedOut,
                creationDate,
                lastLoginDate,
                lastActivityDate,
                lastPasswordChangedDate,
                lastLockoutDate)
        {

        }

        public string Name { get; set; }
    }
}
