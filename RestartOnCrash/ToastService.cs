using System;

namespace RestartOnCrash;

public static class ToastService
{
    public static void Notify(string message)
    {
        // TODO: provide a multi platform solution
        Console.WriteLine("RestartOnCrash: ", message);
    }
}