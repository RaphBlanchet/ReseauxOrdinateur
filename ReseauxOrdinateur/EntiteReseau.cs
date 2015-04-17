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

		private void traiterPrimitiveDeTransport(string strPrimitive){
			Console.WriteLine ("Reseau reçoit de transport : " + strPrimitive);
			string[] split = strPrimitive.Split (';');
			string primitive = split [1];

			if (primitive == N_CONNECT.req.ToString ()) {
				int addrSource = Convert.ToInt32(split[2]);

				if (addrSource % 27 != 0) {				//Le fournisseur internet accepte la connexion
					//Parametres : Adresse Source, Adresse Destination, NIEC
					ConnexionReseau conn = connexions.EtablirConnexion (Convert.ToInt32 (split [2]), Convert.ToInt32 (split [3]), Convert.ToInt32 (split [0]));

					//Parametres : NIEC, Adresse Source, Adresse Destination
					PaquetAppel paquet = new PaquetAppel (conn.numeroConnexion, conn.adresseSource, conn.adresseDestinataire);
					Paquet reponse = liaison.TraiterPaquetDeReseau (paquet);
					TraiterPaquetDeLiaison (reponse);
				} else {								//Le fournisseur internet refuse la connexion
					ConnexionReseau conn = connexions.findConnexionWithNIEC(Convert.ToInt32(split[0]));
					//Parametres : NIEC, Primitive, Adresse Source, Adresse Destination
					ecrire_vers_transport(split[0] + ";" + N_DISCONNECT.ind + ";" + split[2] + ";" + split[3] + ";" + Constantes.RAISON_REFUS_FOURNISSEUR);
				}

			} else if (primitive == N_DATA.req.ToString ()) {
				int niec = Convert.ToInt32 (split [0]);
				ConnexionReseau conn = connexions.findConnexionWithNIEC(niec);
				PaquetDonnees paquet = new PaquetDonnees (niec, conn.pr, conn.ps, 0, split [1]);
				Paquet reponse = liaison.TraiterPaquetDeReseau (paquet);
				TraiterPaquetDeLiaison (reponse);
			}
		}

        private void TraiterPaquetDeLiaison(Paquet paquet)
        {
			Console.WriteLine ("Réseau recoit de Liaison : " + paquet.ToPaquetString ());
			ConnexionReseau conn;
			if (paquet is PaquetConnexionEtablie) {
				PaquetConnexionEtablie p = (PaquetConnexionEtablie)paquet;
				conn = connexions.findConnexionWithNum (p.numero_connexion);
				ecrire_vers_transport (conn.niec + ";" + N_CONNECT.conf + ";" + conn.adresseDestinataire);
			} else if (paquet is PaquetIndicationLiberation) {
				PaquetIndicationLiberation p = (PaquetIndicationLiberation)paquet;
				conn = connexions.findConnexionWithNum(p.numero_connexion);
				ecrire_vers_transport(conn.niec + ";" + N_DISCONNECT.ind + ";" + conn.adresseDestinataire);
				break;
			} else if (paquet is PaquetAcquittement) {
				PaquetAcquittement p = (PaquetAcquittement)paquet;
				string pr = p.typePaquet.Substring (0, 3);
				string type = p.typePaquet.Substring (3);
			}
        }
    }
}
