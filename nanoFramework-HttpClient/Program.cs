using nanoFramework.Hardware.Esp32;
using nanoFramework.Networking;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;

namespace nanoFramework_HttpClient
{
    public class Program
    {
        const string TheSsid = "wifissid";
        const string Password = "password";
       

        static HttpClient _httpClient; 

        public static void Main()
        {
            PrintMemory("Start");
            Thread.Sleep(1000);
            try
            {

                CancellationTokenSource cs = new(60000);

                Debug.WriteLine("Network Status: " + WifiNetworkHelper.Status.ToString());

                var success = WifiNetworkHelper.ConnectDhcp(TheSsid, Password, requiresDateTime: true, token: cs.Token);
                Console.WriteLine(success.ToString());
                if (!success)
                {
                    Console.WriteLine($"Can't connect to the network, status: {WifiNetworkHelper.Status}");
                    if (WifiNetworkHelper.HelperException != null)
                    {
                        Console.WriteLine($"ex: {WifiNetworkHelper.HelperException.Message}");
                    }
                }
                else
                {
                    Thread.Sleep(1_000);
                }
                Console.WriteLine("Wifi status:" + WifiNetworkHelper.Status);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Message:" + ex.Message);
                Debug.WriteLine("Halted");
                Thread.Sleep(Timeout.Infinite);
            }
            Console.WriteLine("Hello from nanoFramework!");

            Console.WriteLine("Date: " + DateTime.UtcNow.ToString());

            var storeCA = Resource.GetBytes(Resource.BinaryResources.DigiCert_Global_Root_G2);

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://www.example.org"); 
       
            _httpClient.HttpsAuthentCert = new System.Security.Cryptography.X509Certificates.X509Certificate(storeCA);


            for (int i = 1; i < 20; i++)
            {
                PrintMemory("Mem " + i);
               
                var result = _httpClient.Get("");

                result.EnsureSuccessStatusCode();
                result.Dispose();
                Thread.Sleep(1_000);
            }

         

            Console.WriteLine("Done");
            Thread.Sleep(Timeout.Infinite);

           
        }

        private static string CACert = @"-----BEGIN CERTIFICATE-----
MIIDrzCCApegAwIBAgIQCDvgVpBCRrGhdWrJWZHHSjANBgkqhkiG9w0BAQUFADBh
MQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3
d3cuZGlnaWNlcnQuY29tMSAwHgYDVQQDExdEaWdpQ2VydCBHbG9iYWwgUm9vdCBD
QTAeFw0wNjExMTAwMDAwMDBaFw0zMTExMTAwMDAwMDBaMGExCzAJBgNVBAYTAlVT
MRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5j
b20xIDAeBgNVBAMTF0RpZ2lDZXJ0IEdsb2JhbCBSb290IENBMIIBIjANBgkqhkiG
9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4jvhEXLeqKTTo1eqUKKPC3eQyaKl7hLOllsB
CSDMAZOnTjC3U/dDxGkAV53ijSLdhwZAAIEJzs4bg7/fzTtxRuLWZscFs3YnFo97
nh6Vfe63SKMI2tavegw5BmV/Sl0fvBf4q77uKNd0f3p4mVmFaG5cIzJLv07A6Fpt
43C/dxC//AH2hdmoRBBYMql1GNXRor5H4idq9Joz+EkIYIvUX7Q6hL+hqkpMfT7P
T19sdl6gSzeRntwi5m3OFBqOasv+zbMUZBfHWymeMr/y7vrTC0LUq7dBMtoM1O/4
gdW7jVg/tRvoSSiicNoxBN33shbyTApOB6jtSj1etX+jkMOvJwIDAQABo2MwYTAO
BgNVHQ8BAf8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAdBgNVHQ4EFgQUA95QNVbR
TLtm8KPiGxvDl7I90VUwHwYDVR0jBBgwFoAUA95QNVbRTLtm8KPiGxvDl7I90VUw
DQYJKoZIhvcNAQEFBQADggEBAMucN6pIExIK+t1EnE9SsPTfrgT1eXkIoyQY/Esr
hMAtudXH/vTBH1jLuG2cenTnmCmrEbXjcKChzUyImZOMkXDiqw8cvpOp/2PV5Adg
06O/nVsJ8dWO41P0jmP6P6fbtGbfYmbW0W5BjfIttep3Sp+dWOIrWcBAI+0tKIJF
PnlUkiaY4IBIqDfv8NZ5YBberOgOzW6sRBc4L0na4UU+Krk2U886UAb3LujEV0ls
YSEY1QSteDwsOoBrp+uvFRTp2InBuThs4pFsiv9kuXclVzDAGySj4dzp30d8tbQk
CAUw7C29C79Fv1C5qfPrmAESrciIxpg0X40KPMbp1ZWVbd4=
-----END CERTIFICATE-----
";

        public static void PrintMemory(string msg)
        {
            NativeMemory.GetMemoryInfo(NativeMemory.MemoryType.Internal, out uint totalSize, out uint totalFree, out uint largestFree);
            Console.WriteLine($"{msg} -> Internal Mem:  Total Internal: {totalSize} Free: {totalFree} Largest: {largestFree}");
            Console.WriteLine($"nF Mem:  {nanoFramework.Runtime.Native.GC.Run(false)}");
        }

    }
}
