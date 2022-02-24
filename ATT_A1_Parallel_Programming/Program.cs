/*
 * FILE             :   Program.cs
 * PROJECT          :   ATT_A1_Parallel_Programming
 * PROGRAMMER       :   Chowon Jung
 * FIRST VERSION    :   2022-01-21
 * DESCRIPTION      :   
 */


using ATT_A1_Parallel_Programming;

/// <summary>
/// This namespace includes classes and data used for parallel processing assignment.
/// </summary>
namespace ParallelProcessing
{
    /// <summary>
    /// Indicates current program status for flow decisions and report
    /// </summary>
    internal enum StatusCode
    {
        Quit = 0,           // to quit application
        NULLOrEmpty = 1,    // input is null or empty
        Length = 2,         // invalid password length
        Format = 3,         // invalid password format
        Time = 4,           // invalid time limit format
        Next = 5,           // time limit is entered but no password yet
        Pass = 6            // validation passed, move to guessing phase
    }

    /// <summary>
    /// This class contains a nested class and methods that invokes and validates user input.
    /// </summary>
    public static class Program
    {
        // password search area as ASCII for alphanumeric extensibility
        /// <summary> minimum char property to guess as in ASCII </summary>
        internal const int MIN = 48; // 48 as numeric 0
        /// <summary> maximum char property to guess as in ASCII </summary>
        internal const int MAX = 57; // 57 as numeric 9

        /// <summary> minimum length for a password </summary>
        const int MIN_LENGTH = 6;
        /// <summary> maximum length for a password </summary>
        const int MAX_LENGTH = 18;

        /// <summary> key code for quit request </summary>
        const string QUIT_CODE = "Q";
        /// <summary> key code for re-entering time limit (<see cref="Validator.Limit"/>) request </summary>
        const string TIME_CODE = "T";
        /// <summary> affinity number for current process </summary>
        const int AFF = 15;

        public static void Main(string[] args)
        {
            // set up class instances
            Validator validator = new Validator();
            Finder finder = new Finder(MIN, MAX);

            // repeat until quit request
            while (validator.InvokeInput() > (int)StatusCode.Quit)
            {
                // do guess when validated only
                if (validator.currentStatus == StatusCode.Pass)
                {
                    finder.FindByAffinity(validator.Password, AFF, validator.Limit);
                    validator.currentStatus = StatusCode.Time;      // reset status for next guess
                }
            }
            // else quit application
        }

        /// <summary>
        /// This class is a nested class of <see cref="Program"/> that validates user input.
        /// </summary>
        private class Validator
        {
            /// <summary>messages displayed for user to inform current validation status</summary>
            readonly string[] announceMsgs = {
            "Bye!",
            "Your input cannot be empty.",
            "Password digits cannot be less than " + MIN_LENGTH + " or larger than " + MAX_LENGTH + ".",
            "Password must be made up of " + (char)MIN + " through " + (char)MAX + " only.",
            "The time limit must be an integer larger than 0.",
            "The search will not exceed the time limit you entered.",
            ""
            };

            /// <summary>current message to be displayed</summary>
            string currentMsg = String.Empty;
            /// <summary>current application status</summary>
            internal StatusCode currentStatus { get; set; } = StatusCode.Quit;
            /// <summary>original user input password</summary>
            internal string Password { get; set; } = null!;
            /// <summary>time limit in milliseconds for a whole guesses for a password</summary>
            internal long Limit { get; set; } = -1;
            
            /// <summary>
            /// This method invokes user for time limit and password to guess.
            /// </summary>
            /// <returns>returns <see cref="currentStatus"/> as integer</returns>
            internal int InvokeInput()
            {
                // reset password for each play
                Password = string.Empty;

                // if time limit has to be entered
                if (Limit <= 0)
                {
                    // invoke for time limit input
                    Console.Clear();
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Please Enter a Time Limit in Milliseconds. (Enter " + QUIT_CODE + " to Exit)");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("" + currentMsg);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("The time limit is: ");

                    // get and validate time limit input
                    ValidateTimeLimit();
                }
                // if valid time limit is entered
                else if (currentStatus <= StatusCode.Next)
                {
                    // invoke for password input
                    Console.Clear();
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Please Enter a Password. (Enter " + TIME_CODE + " to go back to Time Limit screen)");
                    Console.WriteLine("");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("The password");
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(" must ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("be:");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(" - made up of " + (char)MIN + " through " + (char)MAX + " only");
                    Console.WriteLine(" - between " + MIN_LENGTH + " and " + MAX_LENGTH + " digits in length");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("" + currentMsg);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("Entered password is: ");

                    // get and validate password
                    if (ValidatePassword() <= (int)StatusCode.Quit)
                    {
                        // if quit requested, say bye
                        Console.WriteLine(currentMsg);
                    }
                }

                return (int)currentStatus;
            }

            /// <summary>
            /// This method gets user input for time limit and validates it.
            /// </summary>
            private void ValidateTimeLimit()
            {
                // get user input
                string? input = string.Empty;
                input = Console.ReadLine();

                // validate user input
                if (string.IsNullOrEmpty(input))
                {
                    currentStatus = StatusCode.NULLOrEmpty;
                }
                else if (input.Equals(QUIT_CODE, StringComparison.OrdinalIgnoreCase))
                {
                    // if requested to quit, set flag
                    currentStatus = StatusCode.Quit;
                }
                else if ((!long.TryParse(input, out long o)) || (o <= 0))
                {
                    currentStatus = StatusCode.Time;
                }
                else
                {
                    // if valid, record and set progress flag
                    Limit = long.Parse(input);
                    currentMsg = string.Empty;
                    currentStatus = StatusCode.Next;
                }

                // set announce message for report
                currentMsg = announceMsgs[(int)currentStatus];
            }

            /// <summary>
            /// This method gets and validates user input password.
            /// </summary>
            /// <returns>returns <see cref="currentStatus"/> as integer.</returns>
            private int ValidatePassword()
            {
                // get user input
                string? input = string.Empty;
                input = Console.ReadLine();

                // validate user input
                if (string.IsNullOrEmpty(input))
                {
                    currentStatus = StatusCode.NULLOrEmpty;

                }
                else if (input.Equals(TIME_CODE, StringComparison.OrdinalIgnoreCase))
                {
                    // if requested for time limit input, set flags
                    Limit = -1;
                    currentStatus = StatusCode.Time;
                }
                else if ((input.Length < MIN_LENGTH) || (input.Length > MAX_LENGTH))
                {
                    currentStatus = StatusCode.Length;
                }
                else
                {
                    Password = input;
                    currentStatus = StatusCode.Pass;

                    for (int i = 0; i < input.Length; i++)
                    {
                        if ((input[i] < MIN) || (input[i] > MAX))
                        {
                            currentStatus = StatusCode.Format;
                        }
                    }
                }

                // set announce message for report
                currentMsg = announceMsgs[(int)currentStatus];
                return (int)currentStatus;
            }
        }
    }
}