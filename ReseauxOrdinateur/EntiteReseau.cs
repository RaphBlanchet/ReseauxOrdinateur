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
		AnonymousPipeClientStream reseauIn;
		AnonymousPipeServerStream reseauOut;
        bool isRunning = true;

		public EntiteReseau(AnonymousPipeClientStream _reseauIn, AnonymousPipeServerStream _reseauOut)
        {
			reseauIn = _reseauIn;
			reseauOut = _reseauOut;
        }

        public void ThreadRun()
        {
            while (isRunning)
            {
				lire_de_transport ();
            }
        }

		public void lire_de_transport()
        {
			string paquet = "";
			try{
				char c;
				do{
					c = (char)reseauIn.ReadByte();

					if(c != Constantes.FIN_PAQUET){
						paquet += c;
					}else{
						traiterPaquetDeTransport(paquet);
					}

				}while(c != Constantes.FIN_PAQUET);

			}catch(Exception e){
				
			}
        }

        public void ecrire_vers_transport()
        {

        }

		private void traiterPaquetDeTransport(string paquet){
			Console.WriteLine ("Reseau reçoit de transport : " + paquet);
		}
    }
}
