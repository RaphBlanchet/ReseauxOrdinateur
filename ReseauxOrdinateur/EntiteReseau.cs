using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;

namespace ReseauxOrdinateur
{
    class EntiteReseau
    {
		AnonymousPipeClientStream reseauIn;
		AnonymousPipeServerStream reseauOut;
        bool isRunning = true;
		TableConnexionReseau connexions;

		public EntiteReseau(AnonymousPipeClientStream _reseauIn, AnonymousPipeServerStream _reseauOut)
        {
			reseauIn = _reseauIn;
			reseauOut = _reseauOut;
			connexions = new TableConnexionReseau ();
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

		public void ecrire_vers_transport(Paquet paquet)
        {
			string strPaquet = paquet.ToPaquetString () + Constantes.FIN_PAQUET;

			try{
				byte[] bytes = new byte[strPaquet.Length * sizeof(char)];
				System.Buffer.BlockCopy(strPaquet.ToCharArray(), 0, bytes, 0, bytes.Length);

				reseauOut.Write(bytes, 0, strPaquet.Length);
			}catch (IOException e){

			}
        }

		private void traiterPaquetDeTransport(string paquet){
			Console.WriteLine ("Reseau reçoit de transport : " + paquet);
			string[] paq = paquet.Split (';');

			if (paq [0].Equals(N_CONNECT.req)) {
				
			}
		}
    }
}
