using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RecycleSystem.Ulitity
{
    public class TypeEnum
    {
        public enum UserType
        {
            isGeneralUser=1,
            isEnterpriseUser
        }
        public enum DemendOrderStatus
        {
            unAccept=1,
            Accepted,
            Finished,
            ApplyingCancel,
            ForbinCancel,
            Canceled
        }
        public enum PressStatus
        {
            unPress,
            Pressed
        }
        public enum OrderStatus
        {
            Running,
            Finished,
            Canceled
        }
        public enum WorkFlowType
        {
            SpecialWithdrew=1
        }
        public enum WorkFlowStatus
        {
            Applying=1,
            Withdrew,
            UnAccept,
            Allow
        }
        public enum WorkFlowStepStatus
        {
            Deny,
            Allowed
        }
        public enum ReviewStatus
        {
            UnView,
            Viewed
        }
        public enum UploadFileType
        {
            [Description("头像")]
            Portrait = 1,

            [Description("新闻图片")]
            News = 2,

            [Description("导入的文件")]
            Import = 10
        }
    }
}
