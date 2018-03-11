using System;

public class Error
{
    public static ConsoleColor FontRed = ConsoleColor.Red,
        FontWhite = ConsoleColor.White,
        FontBlue = ConsoleColor.Blue;
    public static void WriteLine(String str, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(str);
    }
    public static void WriteLine(String str = "")
    {
        Console.WriteLine(str);
    }
    public static void Write(String str, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(str);
    }
    public static void Write(String str)
    {
        Console.Write(str);
    }
}
