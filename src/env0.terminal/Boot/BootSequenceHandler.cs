using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace env0.terminal.Boot
{
    public class BootLine
    {
        public string Text { get; set; }
        public int DelayMs { get; set; }
    }

    public class BootConfig
    {
        public List<BootLine> Lines { get; set; }
    }

    public class BootSequenceHandler
    {
        private BootConfig config;

        public BootSequenceHandler(string configPath)
        {
            string json = File.ReadAllText(configPath);
            config = JsonSerializer.Deserialize<BootConfig>(json);
        }

        public void RunBootSequence()
        {
            foreach (var line in config.Lines)
            {
                Console.WriteLine(line.Text);
                Thread.Sleep(line.DelayMs);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }
}
