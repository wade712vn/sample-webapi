using System;

namespace AccountManager.Domain.Entities
{
    public class UserOperation
    {
        public long Id { get; set; }
        public long MachineId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Params { get; set; }
        public string TypeName { get; set; }
        public string User { get; set; }

        public Machine Machine { get; set; }
        public UserOperationType Type { get; set; }
    }

    public static class UserOperationTypes
    {
        public static string CreateAccount = "CRTEACC";

        public static string Backup = "BACKUP";
        public static string Restore = "RESTORE";

        public static string PushLicenseSettings = "PUSHLIC";
        public static string PushInstanceSettings = "PUSHSOFT";
        public static string PushBackupSettings = "PUSHBKUP";

        public static string StartMachine = "STRTMAC";
        public static string StopMachine = "STOPMAC";
        public static string TerminateMachine = "TERMMAC";
    }
}