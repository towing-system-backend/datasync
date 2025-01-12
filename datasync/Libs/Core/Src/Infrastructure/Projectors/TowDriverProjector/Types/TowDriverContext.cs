namespace Datasync.Core
{
    public record TowDriverContext
    (
        string TowDriverId,
        string TowDriverName,
        string TowDriverEmail,
        string LicenseOwnerName,
        DateOnly LicenseIssueDate,
        DateOnly LicenseExpirationDate,
        string MedicalCertificateOwnerName,
        int MedicalCertificateOwnerAge,
        DateOnly MedicalCertificateIssueDate,
        DateOnly MedicalCertificateExpirationDate,
        int TowDriverIdentificationNumber,
        string TowDriverLocation,
        string TowDriverStatus
    );
}
