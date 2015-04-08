using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;

namespace ReseauxOrdinateur
{
    class EntiteReseau
    {
        NamedPipeClientStream reseauIn;
        NamedPipeServerStream reseauOut;
        bool isRunning = true;

        public EntiteReseau(NamedPipeClientStream _reseauIn, NamedPipeServerStream _reseauOut)
        {
            reseauIn = _reseauIn;
            reseauOut = _reseauOut;
        }

        public void ThreadRun()
        {
            while (isRunning)
            {
                Console.WriteLine("Entité de réseau");
            }
        }

		public void lire_de_transport()
        {

        }

        public void ecrire_vers_transport()
        {

        }
    }
}
