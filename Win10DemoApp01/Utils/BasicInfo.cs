using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.System.Profile;
using Windows.System.UserProfile;

namespace Win10DemoApp01.Utils
{
    public static class BasicInfo
    {
        public static string DeviceID = "";
        public static string UserName = "";
        static bool IsInitialized = false;   

        public static void Initialize()
        {
            if (!IsInitialized)
            {
                DeviceID=GetDeviceID();
                SetUserName();
                IsInitialized = true;
            }
        }

        //取得裝置ID
        private static string GetDeviceID()
        {
            string deviceID = "";
            bool isHardwareIdentificationAPIPresent=Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.Profile.HardwareIdentification");

            if (isHardwareIdentificationAPIPresent)
            {
                var token = HardwareIdentification.GetPackageSpecificToken(null);
                var hardwareId = token.Id;
                var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

                byte[] bytes = new byte[hardwareId.Length];
                dataReader.ReadBytes(bytes);
                deviceID= BitConverter.ToString(bytes);
               
            }
            return deviceID;
        }

        //取得使用者名稱
        private static async void SetUserName()
        {
            UserName = "";
            bool isUserInformationAPIPresent = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.System.UserProfile.UserInformation");
            if (isUserInformationAPIPresent)
            {
                UserName = await UserInformation.GetDisplayNameAsync();
            }
        }
    }
}
