using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarPandaBackend
{
 
    class GameManger
    {
        private int currentGame;
        private DateTime gameStart;
        private DateTime gameEnd;




        public string GetCurrentGame()
        {
            if (currentGame == 0)
            {
                return "No Game Active";
            }
            else if (currentGame == 1)
            {
                return "Plinko";
            }
            else if (currentGame == 2)
            {
                return "Second Game";
            }
            else if (currentGame == 3)
            {
                return "Third Game";
            }
            else
            {
                return "Unknown Game";
            }
        }
        public void SetCurrentGame(int game)
        {
            // Add Safer logic later on;
            currentGame = game;
        }




    }
}
