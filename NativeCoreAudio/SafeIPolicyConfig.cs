using System;
using static NativeCoreAudio.ComInterfaces;

namespace NativeCoreAudio
{
    /// <summary>
    /// Auto disposable IPolicyConfig
    /// </summary>
    public class SafeIPolicyConfig : SafeComObject
    {
        private bool disposedValue;

        private static object GetPolicyConfig()
        {
            Guid CLSID_CPolicyConfigClient
            = new("{870AF99C-171D-4F9E-AF0D-E63DF40C2BC9}");

            Type CPolicyConfigClientType
                = Type.GetTypeFromCLSID(CLSID_CPolicyConfigClient, true);

            object PolicyConfigClient
                = Activator.CreateInstance(CPolicyConfigClientType);

            return PolicyConfigClient;
        }

        public SafeIPolicyConfig()
        {
            SetReleaseObject(GetPolicyConfig());
        }

        private IPolicyConfig GetIPolicyConfig()
        {
            return GetReleaseObject() as IPolicyConfig;
        }

        public void SetDefaultEndpoint(string Id, ERole role)
        {
            ComResult.Check(
            GetIPolicyConfig().SetDefaultEndpoint(Id, role)
            );
        }

        public void SetDefaultEndpointForPolicy(string Id, EDataFlow role)
        {
            ComResult.Check(
            GetIPolicyConfig().SetDefaultEndpointForPolicy(Id, role)
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    base.Dispose(false);
                }
                disposedValue = true;
            }
        }
    }
}
