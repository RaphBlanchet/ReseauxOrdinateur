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
        TableConnexionTransport connexions;
        public bool isRunning = true;

		public EntiteTransport(AnonymousPipeClientStream _transportIn, AnonymousPipeServerStream _transportOut)
        {
			transportIn = _transportIn;
			transportOut = _transportOut;
            connexions = new TableConnexionTransport();
        }

        public void ThreadRun()
        {
            while (isRunning)
            {
                lire_de_reseau();
            }
        }

        public void lire_de_reseau()
        {
			string commande = "";
			try{
				char c;
				do{
					c = (char)transportIn.ReadByte();

					if(c != Constantes.FIN_PAQUET){
						commande += c;
					}else{
						TraiterCommandeDeReseau(commande);
					}

				}while(c != Constantes.FIN_PAQUET);

			}catch(Exception e){

			}
        }

		public void ecrire_vers_reseau(string str)
        {
            Console.Out.WriteLine("Transport vers Reseau : " + str);
			str += Constantes.FIN_PAQUET;

			try{
                byte[] bytes = Encoding.UTF8.GetBytes(str);

				transportOut.Write(bytes, 0, str.Length);

			}catch (IOException e){
				
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
                Console.WriteLine("\nLecture de S_Lec : " + line);
                if (!connexions.ContientConnexion(identifiant))
                {
                    EtablirConnexion(identifiant);
                }

                //Envoi des données
				DateTime tempo = DateTime.Now;
                while (true)
                {
					if (connexions[identifiant] == null) {	//Si la connexion n'existe plus, on envoit rien
						Console.Out.WriteLine("Impossible d'enovyer les données de " + identifiant + " - La connexion est fermée!");
						break;
                    }
                    else
                    {
                        TimeSpan elapsed = new TimeSpan (DateTime.Now.Ticks - tempo.Ticks);
                        //On attend la connexion
                        try
                        {
                            if (connexions[identifiant].etat == EtatConnexion.CONNECTE)
                            {
                                EnvoyerDonnees(identifiant, lineSplit[1]);
                                break;
                            }else if (elapsed.Seconds > 2) {
						        connexions.FermerConnexion (identifiant, "Délai de connexion expiré");
						        break;
					        }
                        }
                        catch (NullReferenceException e)
                        {
                            Console.Out.WriteLine("Impossible d'enovyer les données de " + identifiant + " - La connexion est fermée!");
                            break;
                        }
                    }
                }
            }
        }

        public void EtablirConnexion(string _identifiant)
        {
            ConnexionTransport conn = connexions.EtablirConnexion(_identifiant);

            if (conn != null)
            {
                int numConn = conn.numeroConnexion;
                int addrSource = conn.adresseSource;
                int addrDestinataire = conn.adresseDestinataire;

				//PaquetAppel paquet = new PaquetAppel (numeroConnexion, addrSource, addrDestinataire);
                Utility.EcrireDansFichier("S_ecr.txt", "Ouverture de connexion pour " + _identifiant + "...", true);
				ecrire_vers_reseau (numConn + ";" + N_CONNECT.req + ";" + addrSource + ";" + addrDestinataire);
            }
        }

		private void EnvoyerDonnees(string identifiant, string donnees){
			if (connexions [identifiant].etat == EtatConnexion.CONNECTE) {
				ecrire_vers_reseau (connexions[identifiant].numeroConnexion + ";" + N_DATA.req + ";" + donnees);
			}
		}

        public void DemanderFermetureConnexions()
        {
			while (connexions.nbConnexions > 0) {
				ConnexionTransport conn = connexions.findConnexionAtIndex (0);
				ecrire_vers_reseau (conn.numeroConnexion + ";" + N_DISCONNECT.req + ";" + conn.adresseDestinataire);
				connexions.FermerConnexion (conn.numeroConnexion, "Fin d'exécution");
			}
        }

		private void TraiterCommandeDeReseau(string commande){
            string[] split = commande.Split(';');
            int numeroConnexion = Int32.Parse (split [0]);
            ConnexionTransport conn = connexions[numeroConnexion];

			if (split [1] == N_CONNECT.conf.ToString ()) {
				connexions.ConfirmerConnexion (numeroConnexion);
			} else if (split [1] == N_DISCONNECT.ind.ToString ()) {
				connexions.FermerConnexion (numeroConnexion, split[3]);
			}
		}
    }
}
