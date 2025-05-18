namespace BugFixer.Application.Statics
{
    public static class PathTools
    {
        #region User Avatar Path
        public static readonly string DefaultUserAvatar = "DefaultUserAvatar.png";
        // Path for retrieving user avatar in the application
        public static readonly string UserAvatarPath = "/content/userAvatar/";
        // Path for save user avatar in wwwroot folder
        public static readonly string UserAvatarFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/userAvatar");
        #endregion

        #region WEB address
        public static readonly string SiteAddress = "https://localhost:44316";
        #endregion

        #region Static Path for upload imag in CkEditor
        // Path for save ckeditor image in wwwroot folder
        public static readonly string CkEditorImageFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/ckeditor");
        public static readonly string CkEditorReadImage = "/content/ckeditor";

        #endregion
    }
}
