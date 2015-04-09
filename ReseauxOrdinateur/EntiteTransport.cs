using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.Threading;
using System.IO;

namespace ReseauxOrdinateur
{
    class EntiteTransport
    {
		AnonymousPipeClientStream transportIn;
		AnonymousPipeServerStream transportOut;
        TableConnexions connexions;
        bool isRunning = true;

        public EntiteTransport(NamedPipeClientStream _transportIn, NamedPipeServerStream _transportOut)
        {
			transportIn = _transportIn;
			transportOut = _transportOut;
            connexions = new TableConnexions();
        }

        public void ThreadRun()
        {
            while (isRunning)
            {
                
            }
        }

        public void lire_de_reseau()
        {

        }

        public void ecrire_vers_reseau(Paquet paquet)
        {
			string strPaquet = paquet.ToPaquetString () + Constantes.FIN_PAQUET;

			try{
				byte[] bytes = new byte[strPaquet.Length * sizeof(char)];
				System.Buffer.BlockCopy(strPaquet.ToCharArray(), 0, bytes, 0, bytes.Length);

				transportOut.Write(bytes, 0, strPaquet.Length);

			}catch (Exception e){
				
			}

        }

        public void lireCommandes()
        {
            StreamReader file = new StreamReader("S_lec.txt");

            string line;
            while((line = file.ReadLine()) != null){
                string[] lineSplit = line.Split(';');
                string identifiant = lineSplit[0];

                //On vérifie si la connexion n'est pas déjà établie
                if (!connexions.ContientConnexion(identifiant))
                {
                    EtablirConnexion(identifiant);
                    Thread.Sleep(250);
                }

                //Envoi des données

            }
        }

        public void EtablirConnexion(string _identifiant)
        {
            Connexion conn = connexions.EtablirConnexion(_identifiant);

            if (conn != null)
            {
				int numeroConnexion = conn.numeroConnexion;
                int addrSource = conn.adresseSource;
                int addrDestinataire = conn.adresseDestinataire;

				PaquetAppel paquet = new PaquetAppel (numeroConnexion, addrSource, addrDestinataire);
				ecrire_vers_reseau (paquet);
            }
        }

		private void EnvoyerDonnees(string identifiant, string donnees){
			
		}
    }
}
