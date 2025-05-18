using System;

namespace env0.terminal.Login
{
    public class LoginHandler
    {
        public string Username { get; private set; }

        public void RunLogin()
        {
            Console.Clear();
            Console.Write("Username: ");
            Username = Console.ReadLine();

            Console.Write("Password: ");
            ReadPassword();

            // In the future, add credential check here (right now, accept anything).
        }

        private void ReadPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
            }
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
        }
    }
}
