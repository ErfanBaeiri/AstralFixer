namespace BugFixer.Application.Generators
{
    public static class CodeGenerator
    {
        public static string CreateActivationCode()
        {
            // Generate a new GUID and convert it to a string
            return Guid.NewGuid().ToString("N");

        }
    }
}
