using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA.Data
{
    public class Tour
    {
        public int NumeroTour { get; set; }
        public int Phase { get; set; }

        public TypeJour Etat
        {
            get
            {
                if ((Phase + 1) % 4 == 0)
                {
                    return TypeJour.NUIT;
                }
                else
                {
                    return TypeJour.JOUR;
                }
            }
        }

        public int Jour
        {
            get
            {
                return (Phase / 4)+1;
            }
        }
    }
}
