using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FactFlux.Logic
{
    public class HelperLogic
    {
        public static string GenerateRandomColor()
        {
            string chosenCOlor;

            Random rnd = new Random();

            int randomNumber = rnd.Next(1, 33);

            #region random colors
            if (randomNumber == 1)
            {
                chosenCOlor = "#92B558";
            }
            else if (randomNumber == 2)
            {
                chosenCOlor = "#DC4C46";
            }
            else if (randomNumber == 3)
            {
                chosenCOlor = "#672E3B";
            }
            else if (randomNumber == 4)
            {
                chosenCOlor = "#F3D6E4";
            }
            else if (randomNumber == 5)
            {
                chosenCOlor = "#C48F65";
            }
            else if (randomNumber == 6)
            {
                chosenCOlor = "#223A5E";
            }
            else if (randomNumber == 7)
            {
                chosenCOlor = "#898E8C";
            }
            else if (randomNumber == 8)
            {
                chosenCOlor = "#005960";
            }
            else if (randomNumber == 9)
            {
                chosenCOlor = "#9C9A40";
            }
            else if (randomNumber == 10)
            {
                chosenCOlor = "#4F84C4";
            }
            else if (randomNumber == 11)
            {
                chosenCOlor = "#D2691E";
            }
            else if (randomNumber == 12)
            {
                chosenCOlor = "#578CA9";
            }
            else if (randomNumber == 13)
            {
                chosenCOlor = "#F6D155";
            }
            else if (randomNumber == 14)
            {
                chosenCOlor = "#004B8D";
            }
            else if (randomNumber == 15)
            {
                chosenCOlor = "#F2552C";
            }
            else if (randomNumber == 16)
            {
                chosenCOlor = "#95DEE3";
            }
            else if (randomNumber == 17)
            {
                chosenCOlor = "#92B558";
            }
            else if (randomNumber == 18)
            {
                chosenCOlor = "#EDCDC2";
            }
            else if (randomNumber == 19)
            {
                chosenCOlor = "#CE3175";
            }
            else if (randomNumber == 20)
            {
                chosenCOlor = "#5A7247";
            }
            else if (randomNumber == 21)
            {
                chosenCOlor = "#CFB095";
            }
            else if (randomNumber == 22)
            {
                chosenCOlor = "#4C6A92";
            }
            else if (randomNumber == 23)
            {
                chosenCOlor = "#92B6D5";
            }
            else if (randomNumber == 24)
            {
                chosenCOlor = "#838487";
            }
            else if (randomNumber == 25)
            {
                chosenCOlor = "#838487";
            }
            else if (randomNumber == 26)
            {
                chosenCOlor = "#B93A32";
            }
            else if (randomNumber == 27)
            {
                chosenCOlor = "#AF9483";
            }
            else if (randomNumber == 28)
            {
                chosenCOlor = "#AD5D5D";
            }
            else if (randomNumber == 29)
            {
                chosenCOlor = "#006E51";
            }
            else if (randomNumber == 30)
            {
                chosenCOlor = "#006E51";
            }
            else if (randomNumber == 31)
            {
                chosenCOlor = "#D8AE47";
            }
            else if (randomNumber == 32)
            {
                chosenCOlor = "#D8AE47";
            }
            else
            {
                chosenCOlor = "#B76BA3";
            }
            #endregion
            return chosenCOlor;
        }
    }
}