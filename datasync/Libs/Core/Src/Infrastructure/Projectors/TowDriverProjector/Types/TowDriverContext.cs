using Datasync.Core;

namespace datasync.Libs.Core.Src.Infrastructure.Projectors.TowDriverProjector.Types
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
