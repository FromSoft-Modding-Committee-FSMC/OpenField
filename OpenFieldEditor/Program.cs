using OFC.Utility;

namespace OFE
{
    class Program
    {
        static void Main()
        {
            //Program Initialization
            Log.EnableColour(false);

            using Game game = new Game();
            game.Run();
        }
    }
}
