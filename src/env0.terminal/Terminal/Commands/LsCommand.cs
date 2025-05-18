namespace env0.terminal.Terminal.Commands
{
    public class LsCommand : CommandHandler
    {
        public override string Execute(string[] args)
        {
            // For now, return a simple placeholder output
            return "file1.txt\nfile2.txt\nfolder1";
        }
    }
}
