using System;

namespace protectTreesV2.SystemManagement
{
    public enum AccountVerifyStatus
    {
        尚未審核 = 0,
        已審核 = 1
    }

    [Serializable]
    public class AccountManageFilter
    {
        public AccountVerifyStatus? verfiyStatus { get; set; }
    }
}
