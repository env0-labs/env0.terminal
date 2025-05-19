using System;
using Env0.Terminal.Config;



class Program
{
    static void Main(string[] args)
    {

Console.WriteLine("Current working directory: " + Environment.CurrentDirectory);


        // Load all configs (right now, just BootConfig)
        JsonLoader.LoadAll();

        // Print validation errors (if any)
        if (JsonLoader.ValidationErrors.Count > 0)
        {
            Console.WriteLine("Validation Errors:");
            foreach (var error in JsonLoader.ValidationErrors)
                Console.WriteLine("- " + error);
            Console.WriteLine();
        }

        // Print loaded BootText
        if (JsonLoader.BootConfig != null && JsonLoader.BootConfig.BootText != null)
        {
            Console.WriteLine("Boot Sequence:");
            foreach (var line in JsonLoader.BootConfig.BootText)
                Console.WriteLine(line);
        }
        else
        {
            Console.WriteLine("BootConfig not loaded or BootText missing.");
        }

        // Pause so you can see the output
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
