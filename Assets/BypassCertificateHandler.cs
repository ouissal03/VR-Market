using UnityEngine;

public class BypassCertificateHandler : UnityEngine.Networking.CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Skip certificate validation (for local development purposes)
        return true;
    }
}
