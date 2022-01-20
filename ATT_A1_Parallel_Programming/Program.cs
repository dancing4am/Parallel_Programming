

// See https://aka.ms/new-console-template for more information

namespace ParallelProcessing
{
    internal enum InputCode
    {
        Quit = 0,
        Pass = 1,
        NULLOrEmpty = 2,
        Length = 3,
        Format = 4
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            Validator validator = new Validator();
            if (validator.GetInput())
            {
                // do guess
            }
            // else quit
        }
    }


    internal class Validator
    {
        const int minLength = 6;
        const int maxLength = 18;
        const string quitCode = "Q";
        readonly string[] announceMsgs = {
            "Bye!",
            "Processing . . .",
            "Password cannot be empty.",
            "Password digits cannot be less than 6 or larger than 18.",
            "Passord should be made up of the digits 0 through 9 only."
        };

        string currentMsg = "";

        internal bool GetInput()
        {
            string? input = "";
            int currentInputCode = (int)InputCode.Quit;

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Please Enter a Password! (Press Q to Exit)");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("The password");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(" must ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("be:");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(" - made up of the digits 0 through 9 only");
                Console.WriteLine(" - between 6 and 18 digits in length");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("" + currentMsg);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Your password is: ");

                input = Console.ReadLine();
            } while (ValidateInput(ref input, currentInputCode) > (int)InputCode.Pass);

            if (currentInputCode <= (int)InputCode.Quit)
            {
                Console.WriteLine(currentMsg);
                return false;
            }
            else
            {
                return true;
            }
        }

        private int ValidateInput(ref string? input, int currentInputCode)
        {
            if (string.IsNullOrEmpty(input))
            {
                currentInputCode = (int)InputCode.NULLOrEmpty;

            }
            else if (input.Equals(quitCode, StringComparison.OrdinalIgnoreCase))
            {
                currentInputCode = (int)InputCode.Quit;
            }
            else if ((input.Length < minLength) || (input.Length > maxLength))
            {
                currentInputCode = (int)InputCode.Length;
            }
            else if (!Int32.TryParse(input, out _))
            {
                currentInputCode = (int)InputCode.Format;
            }

            currentMsg = announceMsgs[currentInputCode];
            return currentInputCode;
        }
    }
}