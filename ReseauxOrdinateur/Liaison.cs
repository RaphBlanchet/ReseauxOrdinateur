/*	Liaison.cs
 * 	Par Raphaël Blanchet, Catherine Béliveau, Joel Gbalou, Sébastien Piché Aubin et Marfous Lawani
 * 	Créé le 1er Avril 2015
 * 	Classe permettant d'implémenter la couche Liaison du protocole OSI
 */

using System;

namespace ReseauxOrdinateur
{
	public class Liaison
	{
        ListeConnexionsLiaison connexions = null;		//Liste des connexions gérées par la couche Liaison
        String donneesEnCours = "";						//String des données en cours d'envoi

		//Constructeur de la couche Liaison
		public Liaison ()
		{
            connexions = new ListeConnexionsLiaison();
		}

		//Fonction permettant de traiter un paquet reçu de la couche Réseau
        public Paquet TraiterPaquetDeReseau(Paquet paquet)
        {
			//Affichage du paquet reçu
			Utility.AfficherDansConsole("Liaison recoit de réseau : " + paquet.ToString(), Constantes.RESEAU_LIAISON_COLOR);
			Utility.EcrireDansFichier ("L_ecr.txt", paquet.ToString (), true);

			//Traitement du paquet
            Paquet reponse = null;
            if (paquet is PaquetAppel)                      //Paquet d'appel----------------------------------
            {
                PaquetAppel p = (PaquetAppel)paquet;
                int addrSource = p.adresseSource;

				if (addrSource % 13 == 0) { 			    //REFUS DE LA CONNEXION DU DISTANT
					reponse = new PaquetIndicationLiberation (p.numero_connexion, p.adresseSource, p.adresseDestination, Constantes.RAISON_REFUS_DISTANT);
				} else if (addrSource % 19 == 0) {		    //AUCUNE RÉPONSE DE LA COUCHE LIAISON
					reponse = null;
				}
				else{                      				    //ACCEPTATION DE LA CONNEXION
                    connexions.AjouterConnexion(p.numero_connexion, p.adresseSource, p.adresseDestination);
                    reponse = new PaquetConnexionEtablie(p.numero_connexion, p.adresseSource, p.adresseDestination);
                }
            }
            else if (paquet is PaquetDonnees)               //Paquet de données----------------------------------
            {
				PaquetDonnees p = (PaquetDonnees)paquet;
				ConnexionLiaison conn = connexions.findConnexion (paquet.numero_connexion);
				int rdm = new Random ().Next (8);
				if (conn.adresseSource % 15 == 0) {	        //Pas de reponse-------------------------------------
					reponse = null; 
				} else if(p.pS == rdm){				        //Acquittement négatif
					reponse = new PaquetAcquittement(p.numero_connexion, p.pR, false);
				}else{								        //Acquittement positif
					Utility.AfficherDansConsole("Donnees bien recues : " + p.donnees, Constantes.OUTPUT_COLOR);
					reponse = new PaquetAcquittement(p.numero_connexion, p.pR+1, true);

                    //Écriture dans le fichier
                    donneesEnCours += p.donnees;

					//Si M est égale à 0, cela veut dire que la couche réseau à bien transmis tout ses paquets de données
					//On peut donc écrire la chaîne dans le fichier de sortie et la réinitialiser
                    if (p.M == 0)
                    {
                        Utility.EcrireDansFichier("S_ecr.txt", donneesEnCours, true);
                        donneesEnCours = "";
                    }
				}
            }
            else if (paquet is PaquetDemandeLiberation)     //Paquet Demande Liberation--------------------------
            {
                PaquetDemandeLiberation p = (PaquetDemandeLiberation)paquet;
				connexions.RetirerConnexion (p.numero_connexion);
            }

			//Écriture du paquet à envoyer dans le fichier de sortie L_lec.txt
			if (reponse != null) {
				Utility.EcrireDansFichier ("L_lec.txt", reponse.ToString(), true);
			}

            return reponse;
        }


	}
}

