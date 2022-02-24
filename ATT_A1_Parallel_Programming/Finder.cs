/*
 * FILE             :   Finder.cs
 * PROJECT          :   ATT_A1_Parallel_Programming
 * PROGRAMMER       :   Chowon Jung
 * FIRST VERSION    :   2022-01-21
 * DESCRIPTION      :   This file contains Finder class for password guessing process.
 */

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ATT_A1_Parallel_Programming
{
    /// <summary>
    /// This class contains different methods for password guessing process,
    /// including reporting the result.
    /// </summary>
    internal class Finder
    {
        /// <summary> minimum char property to guess as in ASCII </summary>
        private int Min { get; }
        /// <summary> maximum char property to guess as in ASCII </summary>
        private int Max { get; }
        /// <summary> counts time amount taken to guess the password </summary>
        private Stopwatch stopwatch;
        /// <summary> contains original password used to transport between methods </summary>
        private char[] passwordChars = { };
        /// <summary> contains found password used to transport between methods </summary>
        private char[] foundChars = { };
        /// <summary> number of tries to guess for current password </summary>
        private int guessCount = 0;
        /// <summary> indicates whether current guess exceeded the given time limit (<see cref="Validator.Limit"/>) </summary>
        internal bool IsExceeded { get; set; } = false;

        /// <summary>
        /// Finder Constructor that allows setting up possible password ASCII code range
        /// </summary>
        /// <param name="min">minimum ASCII code possible for a password</param>
        /// <param name="max">maximum ASCII code possible for a password</param>
        internal Finder(int min, int max)
        {
            Min = min;
            Max = max;
            stopwatch = new Stopwatch();
        }

        /// <summary>
        /// This method sets up affinity used for process and invokes a search method.
        /// </summary>
        /// <param name="password">user entered original password</param>
        /// <param name="affinity">processor affinity</param>
        /// <param name="limit">time limit for a whole guess in milliseconds given by user input</param>
        internal void FindByAffinity(string password, int affinity, long limit)
        {
            // reset variables
            guessCount = 0;
            IsExceeded = false;
            if (stopwatch == null)
            {
                stopwatch = new Stopwatch();
            }
            else
            {
                stopwatch.Reset();
            }

            // check for OS platform that allows processor affinity adjustment
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process p = Process.GetCurrentProcess();
                p.ProcessorAffinity = (IntPtr)affinity;
                Console.WriteLine("using affinity " + p.ProcessorAffinity.ToString() + "...");
            }

            Console.WriteLine("Finding Password " + password);

            // =========== these finding options are ordered by speed from largest to smallest.
            // =========== you MUST COMMENT OUT other unused options when enabling one.

            // Option #1 ******* Enabling this snippet will find the password
            // sequentially from the largest digit in a single task.
            passwordChars = password.ToCharArray();
            foundChars = new char[passwordChars.Length];
            Task task = Task.Run(() => FindOneByOneSequentially(password, limit));
            task.Wait();

            //// Option #2 ******* Enabling this snippet will find the password
            //// with the same number of parallel tasks with the length of the password.
            //passwordChars = password.ToCharArray();
            //foundChars = new char[passwordChars.Length];
            //Task[] taskList = new Task[passwordChars.Length];
            //for (int i = 0; i < passwordChars.Length; i++)
            //{
            //    taskList[i] = new Task(() => FindIndividually(i, limit));
            //    taskList[i].Start();
            //    Console.WriteLine("Starting thread # " + i + " while password length is " + passwordChars.Length);
            //}
            //Task.WhenAll(taskList.ToArray());
            //stopwatch.Stop();

            //// Option #3 ******* Enabling this snippet will find the password
            //// straight forward ascending in a single task counting from 0 to maximum number.
            //Task task = Task.Run(() => FindAscending(password, limit));
            //task.Wait();

            // report user the result
            if (IsExceeded)
            {
                Console.WriteLine("Time exceeded. Abort.");
            }
            else
            {
                Console.Write("Found Password is ");
                Console.WriteLine(foundChars, 0, foundChars.Length);
                Console.WriteLine("Solved in " + guessCount + " guesses, " + stopwatch.ElapsedTicks + " ticks, and "
                    + TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).TotalSeconds + " seconds.");
            }
            Console.ReadKey();
        }

        /// <summary>
        /// This method guesses the password from the largest digit to the smallest,
        /// sequentially until the smallest digit is found.
        /// </summary>
        /// <param name="password">original full password to match</param>
        /// <param name="limit">time limit for finding a whole password</param>
        private void FindOneByOneSequentially(string password, long limit)
        {
            // set up and measure time amount taken
            char[] passwordChar = password.ToCharArray();
            if (!stopwatch.IsRunning)
            {
                stopwatch.Start();
            }

            // loop for each digit  
            for (int i = 0; i < passwordChar.Length; i++)
            {
                // guess from minimum possible ASCII to maximum for each
                for (int j = Min; j <= Max; j++)
                {
                    guessCount++;
                    // if match found, record
                    if (passwordChar[i] == j)
                    {
                        foundChars[i] = (char)j;
                        break;
                    }
                    // if time limit is exceeded, abort
                    else if (stopwatch.ElapsedMilliseconds >= limit)
                    {
                        IsExceeded = true;
                        break;
                    }
                }
                if (IsExceeded)
                {
                    break;
                }
            }
            stopwatch.Stop();
        }

        /// <summary>
        /// This method guesses a given single digit,
        /// from the minimum possible ASCII to the maximum for each.
        /// </summary>
        /// <param name="index">an index of the password a task instance to guess</param>
        /// <param name="limit">time limit for finding a whole password</param>
        private void FindIndividually(int index, long limit)
        {
            // measure time amount taken
            if (!stopwatch.IsRunning)
            {
                stopwatch.Start();
            }

            // guess from minimum possible ASCII to maximum for each
            for (int i = Min; i <= Max; i++)
            {
                guessCount++;
                // if match found, record
                if ((passwordChars.Length > index) && (passwordChars[index] == i))
                {
                    foundChars[index] = (char)i;
                    break;
                }
                // if time limit is exceeded, abort
                else if (stopwatch.ElapsedMilliseconds >= limit)
                {
                    IsExceeded = true;
                    break;
                }
            }
        }

        /// <summary>
        /// This method guesses password counting straight forward
        /// from 0 to maximum possible long integer.
        /// </summary>
        /// <param name="password">original full password to match</param>
        /// <param name="limit">time limit for finding a whole password</param>
        private void FindAscending(string password, long limit)
        {
            // validate datatype of the password
            if (long.TryParse(password, out long converted))
            {
                // measure time amount taken
                if (!stopwatch.IsRunning)
                {
                    stopwatch.Start();
                }
                // count until max value
                for (long i = 0; i < long.MaxValue; i++)
                {
                    guessCount++;
                    // if match found, record
                    if (converted == i)
                    {
                        stopwatch.Stop();
                        foundChars = i.ToString().ToCharArray();
                        break;
                    }
                    // if time limit is exceeded, abort
                    else if (stopwatch.ElapsedMilliseconds >= limit)
                    {
                        IsExceeded = true;
                        break;
                    }
                }
            }
        }
    }
}
