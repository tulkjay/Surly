using System;
using System.Collections.Generic;

namespace Surly.Helpers
{
    public static class ConsoleInterface
    {
        public enum Color
        {
            Red,
            Blue,
            Yellow,
            Cyan,
            Magenta,
            Green,
            DarkGreen
        }

        public static void Set(Color selection)
        {
            switch (selection)
            {
                case Color.Red:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case Color.Blue:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;

                case Color.Yellow:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case Color.Cyan:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

                case Color.Magenta:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;

                case Color.Green:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case Color.DarkGreen:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(selection), selection, null);
            }
        }

        public static void WelcomeOurGuests()
        {
            Set(Color.Magenta);
            Console.WriteLine("\t\tWelcome, welcome! \n\n" +
                              "\t\tYou are thusly a guest to the surliest database under the sun! Begin..." +
                              "\n\n");
            Set(Color.Cyan);
        }

        public static void WriteRow(List<string> dataList, string startString, Color textColor = Color.Magenta, Color postTextColor = Color.Cyan)
        {
            Set(textColor);

            Console.Write(startString);

            dataList.ForEach(x => Console.Write($"{x} "));

            Console.WriteLine();

            Set(postTextColor);
        }
    }
}