using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ITP104_FINAL_PROJECT.Services
{
    /// <summary>
    /// Service to check network connectivity status
    /// </summary>
    public static class NetworkService
    {
        /// <summary>
        /// Check if internet connection is available
        /// </summary>
        public static bool IsInternetAvailable()
        {
            try
            {
                // Check if any network interface is up and operational
                if (!NetworkInterface.GetIsNetworkAvailable())
                    return false;

                // Try to ping a reliable host
                using (var ping = new Ping())
                {
                    try
                    {
                        var reply = ping.Send("8.8.8.8", 3000); // Google DNS
                        return reply.Status == IPStatus.Success;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check internet connectivity asynchronously
        /// </summary>
        public static async Task<bool> IsInternetAvailableAsync()
        {
            return await Task.Run(() => IsInternetAvailable());
        }

        /// <summary>
        /// Get network status message
        /// </summary>
        public static string GetNetworkStatusMessage()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return "No network adapters detected";

            if (!IsInternetAvailable())
                return "Connected to network but no internet access";

            return "Internet connection available";
        }
    }
}
