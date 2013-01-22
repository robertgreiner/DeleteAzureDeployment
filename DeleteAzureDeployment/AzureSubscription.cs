using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeleteAzureDeployment
{
    class AzureSubscription
    {
        public string SubscriptionId { get; private set; } 
        public string DnsPrefix { get; private set; } 
        public string DeploymentSlot { get; private set; }
        public string CertificateThumbprint { get; private set; }

        public AzureSubscription(string subscriptionId, string dnsPrefix, string deploymentSlot, string certificateThumbprint)
        {
            SubscriptionId = subscriptionId;
            DnsPrefix = dnsPrefix;
            DeploymentSlot = deploymentSlot;
            CertificateThumbprint = certificateThumbprint;
        }
    }
}
