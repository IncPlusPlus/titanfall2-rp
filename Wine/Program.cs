using System;

namespace titanfall2_rp.Wine
{
    internal static class Program
    {
        private static void Main()
        {
            // Instantiate the RP manager
            RichPresenceManager manager = new();
            manager.Begin();
            // Keep the manager running until the user decides to end it
            Console.ReadLine();
            // Stop the manager and close (by reaching the end of the program)
            manager.Stop();
        }
    }
}