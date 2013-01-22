using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DeleteAzureDeployment
{
    class AzureManager
    {
        public AzureSubscription Subscription { get; private set; }

        public AzureManager(AzureSubscription subscription)
        {
            Subscription = subscription;
        }

        public string DeleteDeployment()
        {
            string result;

            var request = BuildDeleteDeploymentRequest();
            request.ClientCertificates.Add(FindAzureManagementCertificate());

            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                result = response.Headers["x-ms-request-id"];
            }
            catch (Exception e)
            {
                result = e.Message + " (Exception!)";
            }

            return result;
        }

        public string GetOperationStatus(string requestId)
        {
            string result;

            var request = BuildGetOperationStatusRequest(requestId);
            request.ClientCertificates.Add(FindAzureManagementCertificate());

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream);
                result = reader.ReadToEnd();
                response.Close();
                responseStream.Close();
                reader.Close();
            }
            catch (Exception e)
            {
                result = e.ToString();
            }

            return result;
        }

        private X509Certificate2 FindAzureManagementCertificate()
        {
            var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            try
            {
                certStore.Open(OpenFlags.ReadOnly);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, Subscription.CertificateThumbprint, false);
            certStore.Close();

            if (certCollection.Count == 0)
            {
                throw new Exception("Error: No certificate found containing thumbprint " + Subscription.CertificateThumbprint);
            }

            var certificate = certCollection[0];
            return certificate;
        }

        private HttpWebRequest BuildDeleteDeploymentRequest()
        {
            var uri = string.Format("https://management.core.windows.net/{0}/services/hostedservices/{1}/deploymentslots/{2}",
                                    Subscription.SubscriptionId, Subscription.DnsPrefix, Subscription.DeploymentSlot);

            var requestUri = new Uri(uri);
            return BuildRequest(requestUri, "DELETE");
        }

        private HttpWebRequest BuildGetOperationStatusRequest(string requestId)
        {
            var uri = string.Format("https://management.core.windows.net/{0}/operations/{1}",
                                    Subscription.SubscriptionId, requestId);

            var requestUri = new Uri(uri);
            return BuildRequest(requestUri, "GET");
        }

        private static HttpWebRequest BuildRequest(Uri requestUri, string method)
        {
            var request = (HttpWebRequest) HttpWebRequest.Create(requestUri);
            request.Headers.Add("x-ms-version", "2012-08-01");
            request.Method = method;
            request.ContentType = "application/xml";

            return request;
        }
    }
}
