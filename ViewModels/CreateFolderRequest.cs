namespace PersonalCloudDrive.ViewModels
{
    public class CreateFolderRequest
    {
        public string FolderName { get; set; } = string.Empty;
        // antiforgery token may be present but is handled by ValidateAntiForgeryToken
        public string? __RequestVerificationToken { get; set; }
    }
}