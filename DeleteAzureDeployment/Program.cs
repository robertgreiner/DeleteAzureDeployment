using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DeleteAzureDeployment
{
    class Program
    {
        static void Main(string[] args)
        {

            var subscriptionInfo = new AzureSubscription(subscriptionId: "", 
                                                         dnsPrefix: "", 
                                                         deploymentSlot: "", 
                                                         certificateThumbprint:"");

            var azureManager = new AzureManager(subscriptionInfo);
            var requestId = azureManager.DeleteDeployment();
            var result = azureManager.GetOperationStatus(requestId);
            Console.WriteLine(result);
        }
    }
}
