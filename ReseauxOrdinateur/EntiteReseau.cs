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
        Liaison liaison;
		AnonymousPipeClientStream reseauIn;
		AnonymousPipeServerStream reseauOut;
        bool isRunning = true;
		TableConnexionReseau connexions;

		public EntiteReseau(AnonymousPipeClientStream _reseauIn, AnonymousPipeServerStream _reseauOut)
        {
            liaison = new Liaison();
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
						traiterPrimitiveDeTransport(paquet);
					}
				}while(c != Constantes.FIN_PAQUET);

			}catch(Exception e){
				
			}
        }

		public void ecrire_vers_transport(string str)
        {

            Console.Out.WriteLine("Reseau vers Transport : " + str);
            str += Constantes.FIN_PAQUET;

            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);

                reseauOut.Write(bytes, 0, str.Length);

            }
            catch (IOException e)
            {

            }
        }

		private void traiterPrimitiveDeTransport(string strpaquet){
			Console.WriteLine ("Reseau reçoit de transport : " + strpaquet);
			string[] paq = strpaquet.Split (';');

			if (paq [1] == N_CONNECT.req.ToString()) {
                //Parametres : Adresse Source, Adresse Destination, NIEC
                connexions.EtablirConnexion(Convert.ToInt32(paq[2]), Convert.ToInt32(paq[3]), Convert.ToInt32(paq[0]));

                //Parametres : NIEC, Adresse Source, Adresse Destination
                PaquetAppel paquet = new PaquetAppel(Convert.ToInt32(paq[0]), Convert.ToInt32(paq[2]), Convert.ToInt32(paq[3]));
                Paquet reponse = liaison.TraiterPaquetDeReseau(paquet);
                TraiterPaquetDeLiaison(reponse);
			}
		}

        private void TraiterPaquetDeLiaison(Paquet paquet)
        {
            switch (paquet.typePaquet)
            {
                case Constantes.TYPE_PAQUET_CONNEXION_ETABLIE:
                    PaquetConnexionEtablie p = (PaquetConnexionEtablie)paquet;
                    ConnexionReseau conn = connexions[p.numero_connexion];
                    ecrire_vers_transport(conn.niec + ";" + N_CONNECT.conf + ";" + conn.adresseDestinataire);
                    break;
            }
        }
    }
}
