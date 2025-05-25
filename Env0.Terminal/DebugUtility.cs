namespace Env0.Terminal
{
    public static class DebugUtility
    {
        public static bool Enabled = false;

        public static void Print(string tag, string message)
        {
            if (Enabled)
                Console.WriteLine($"[DEBUG:{tag}] {message}");
        }

        // Optionally include method context if you want:
        public static void PrintContext(string tag, string message, [System.Runtime.CompilerServices.CallerMemberName] string member = "")
        {
            if (Enabled)
                Console.WriteLine($"[DEBUG:{tag}:{member}] {message}");
        }
    }
}