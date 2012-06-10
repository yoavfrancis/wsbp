using System;

namespace CgWii1
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                using (Game2 game = new Game2())
                {
                    game.Run();
                }
            }
            catch (Exception e)
            {
                using (WiiCGErrorHandler errorHandler = new WiiCGErrorHandler())
                {
                    errorHandler.ErrorText = e.Message;
                    errorHandler.Run();
                }
            }
        }

    }
#endif
}

