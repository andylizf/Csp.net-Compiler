using System;

public class Error
{
    public static ConsoleColor FontRed = ConsoleColor.Red,
        FontWhite = ConsoleColor.White,
        FontBlue = ConsoleColor.Blue;
    public static void WriteLine(string str, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(str);
    }
    public static void WriteLine(string str = "")
    {
        Console.WriteLine(str);
    }
    public static void Write(string str, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(str);
    }
    public static void Write(string str)
    {
        Console.Write(str);
    }
}
