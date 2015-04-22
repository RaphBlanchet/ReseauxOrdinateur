/*	EntitéTransport.cs
 * 	Par Raphaël Blanchet, Catherine Béliveau, Joel Gbalou, Sébastien Piché Aubin et Marfous Lawani
 * 	Créé le 1er Avril 2015
 * 	Classe permettant d'implémenter la couche Transport du protocole OSI
 */

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
	//Classe implémentant la couche Transport
    class EntiteTransport
    {
		AnonymousPipeClientStream transportIn;		//Pipe de lecture venant de la couche Réseau
		AnonymousPipeServerStream transportOut;		//Pipe d'écriture permettant la communication avec la couche Réseau
        ListeConnexionsTransport connexions;		//Liste des connexions gérées par la couche Transport
        public bool isRunning = true;				//Booléen déterminant si le processus est en cours de traitement

		//Constructeur de la couche Transport
		public EntiteTransport(AnonymousPipeClientStream _transportIn, AnonymousPipeServerStream _transportOut)
        {
			transportIn = _transportIn;
			transportOut = _transportOut;
            connexions = new ListeConnexionsTransport();
        }

		//Fonction contenant la boucle principale du processus
        public void ThreadRun()
        {
            while (isRunning)
            {
                lire_de_reseau();
            }
        }

		//Fonction permettant de lire les primitives envoyées par la couche Réseau
        public void lire_de_reseau()
        {
			string commande = "";
			try{
				char c;
				do{
					c = (char)transportIn.ReadByte();

					//Si on a pas atteint le caractère de fin de primitive
					if(c != Constantes.FIN_PRIMITIVE){
						commande += c;	//Alors on ajoute le caractère lu dans la chaîne de caractère
					}else{
						TraiterCommandeDeReseau(commande);	//Sinon on envoie la chaîne dans la fonction qui traitera celle-ci
					}

				}while(c != Constantes.FIN_PRIMITIVE);

			}catch(IOException e){
				//Affichage du message d'erreur s'il y en a un
				Utility.AfficherDansConsole(e.Message, Constantes.ERREUR_COLOR);
			}
        }

		//Fonction permettant l'écriture vers la couche Réseau
		public void ecrire_vers_reseau(string str)
        {
			//Affichage de la primitive à envoyer
			Utility.AfficherDansConsole("Transport vers Reseau : " + str, Constantes.TRANSPORT_RESEAU_COLOR);
			str += Constantes.FIN_PRIMITIVE;

			try{
				//Transformation de la chaîne de caractère en tableau de Bytes
                byte[] bytes = Encoding.UTF8.GetBytes(str);
				//Envoie du tableau dans le Pipe vers la couche Réseau
				transportOut.Write(bytes, 0, str.Length);

			}catch (IOException e){
				//Affichage du message d'erreur, s'il y en a un
				Utility.AfficherDansConsole(e.Message, Constantes.ERREUR_COLOR);
			}

        }

		//Fonction parcourant le fichier de commandes "S_lec.txt" et traitant les commandes
        public void lireCommandes()
        {
            StreamReader file = new StreamReader("S_lec.txt");

            string line;
            while((line = file.ReadLine()) != null){		//Parcours des lignes du fichier
                string[] lineSplit = line.Split(';');
                string identifiant = lineSplit[0];

                //On vérifie si la connexion n'est pas déjà établie
				Utility.AfficherDansConsole("\nLecture de S_Lec : " + line, Constantes.INPUT_COLOR);
                if (!connexions.ContientConnexion(identifiant))
                {
					//Établissement de la connexion
                    EtablirConnexion(identifiant);
                }

                //Envoi des données - On attend d'abord une confirmation de connexion de la part de Réseau
				//Si le délai de connexion est dépassé (3 secondes), alors la connexion échoue et l'envoi de données est impossible
				DateTime tempo = DateTime.Now;
                while (true)
                {
					//Si la connexion n'existe plus, on envoit rien (il y a eu un problème lors de la connecion)
					if (connexions[identifiant] == null) {	
						Utility.AfficherDansConsole("Impossible d'enovyer les données de " + identifiant + " - La connexion est fermée!", Constantes.ERREUR_COLOR);
						break;
                    }

					//Sinon, on attend une confirmation de connexion et on envoi les données
                    else
                    {
                        TimeSpan elapsed = new TimeSpan (DateTime.Now.Ticks - tempo.Ticks);
                        //On attend la connexion
                        try
                        {
							//Vérification de l'état de connexion
                            if (connexions[identifiant].etat == EtatConnexion.CONNECTE)
                            {
								//Envoie de données
                                EnvoyerDonnees(identifiant, lineSplit[1]);
                                break;

							//Délai de connexion dépassé - Déconnexion
                            }else if (elapsed.Seconds > 3) {
						        connexions.FermerConnexion (identifiant, "Délai de connexion expiré");
						        break;
					        }
                        }
                        catch (NullReferenceException e)
                        {
							//Affichage d'un message d'erreur, s'il y a lieu
							Utility.AfficherDansConsole("Impossible d'enovyer les données de " + identifiant + " - La connexion est fermée!", Constantes.ERREUR_COLOR);
                            break;
                        }
                    }
                }
            }
        }

		//Fonction permettant d'établir une connexion avec un distant
        public void EtablirConnexion(string _identifiant)
        {
            ConnexionTransport conn = connexions.EtablirConnexion(_identifiant);

			//Tentative d'ouverture de connexion
            if (conn != null)
            {
                int numConn = conn.numeroConnexion;
                int addrSource = conn.adresseSource;
                int addrDestinataire = conn.adresseDestination;

				//Envoie de la primitive de demande de connexion vers la couche Réseau
                Utility.EcrireDansFichier("S_ecr.txt", "Ouverture de connexion pour " + _identifiant + "...", true);
				ecrire_vers_reseau (numConn + ";" + N_CONNECT.req + ";" + addrSource + ";" + addrDestinataire);
            }
        }

		//Fonction permettant d'envoyer des données vers la couche réseau
		private void EnvoyerDonnees(string identifiant, string donnees){
			//Si la connexion a été confirmée, on peut envoyer!
			if (connexions [identifiant].etat == EtatConnexion.CONNECTE) {
				ecrire_vers_reseau (connexions[identifiant].numeroConnexion + ";" + N_DATA.req + ";" + donnees);
			}
		}

		//Focntion permettant de faire la demande de fermeture de toutes les connexions, utilisée à la fin d'exécution des commandes
		//du fichier S_lec.txt
        public void DemanderFermetureConnexions()
        {
			while (connexions.nbConnexions > 0) {
				//Fermeture d'une connexion
				ConnexionTransport conn = connexions.findConnexionAtIndex (0);
				ecrire_vers_reseau (conn.numeroConnexion + ";" + N_DISCONNECT.req + ";" + conn.adresseDestination);
				connexions.FermerConnexion (conn.numeroConnexion, "Fin d'exécution");
			}
        }

		//Fonction permettant de savoir si on a toujours des connexions d'ouvertes
		public bool ContientConnexions(){
			return (connexions.nbConnexions != 0);
		}

		//Fonction permettant de traiter une primitive envoyée par la couche Réseau
		private void TraiterCommandeDeReseau(string commande){
            string[] split = commande.Split(';');
            int numeroConnexion = Int32.Parse (split [0]);
            ConnexionTransport conn = connexions[numeroConnexion];

			if (split [1] == N_CONNECT.conf.ToString ()) {				//Confirmation de connexion
				connexions.ConfirmerConnexion (numeroConnexion);
			} else if (split [1] == N_DISCONNECT.ind.ToString ()) {		//Indication de déconnexion
				connexions.FermerConnexion (numeroConnexion, split[3]);
			}
		}
    }
}
