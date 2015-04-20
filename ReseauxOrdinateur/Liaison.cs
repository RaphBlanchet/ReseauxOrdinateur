using System;

namespace ReseauxOrdinateur
{
	public class Liaison
	{
        TableConnexionLiaison connexions = null;
        String donneesEnCours = "";

		public Liaison ()
		{
            connexions = new TableConnexionLiaison();
		}

        public Paquet TraiterPaquetDeReseau(Paquet paquet)
        {
            Console.WriteLine("Liaison recoit de réseau : " + paquet.ToPaquetString());
			Utility.EcrireDansFichier ("L_ecr.txt", paquet.ToString (), true);

            Paquet reponse = null;
            if (paquet is PaquetAppel)                      //Paquet d'appel
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
            else if (paquet is PaquetDonnees)               //Paquet de données
            {
				PaquetDonnees p = (PaquetDonnees)paquet;
				ConnexionLiaison conn = connexions.findConnexion (paquet.numero_connexion);
				int rdm = new Random ().Next (8);
				if (conn.adresseSource % 15 == 0) {	        //Pas de reponse
					reponse = null; 
				} else if(p.pS == rdm){				        //Acquittement négatif
					reponse = new PaquetAcquittement(p.numero_connexion, p.pR, false);
				}else{								        //Acquittement positif
					Console.WriteLine("Donnees recues : " + p.donnees);
					reponse = new PaquetAcquittement(p.numero_connexion, p.pR+1, true);

                    //Écriture dans le fichier
                    donneesEnCours += p.donnees;
                    if (p.M == 0)
                    {
                        Utility.EcrireDansFichier("S_ecr.txt", donneesEnCours, true);
                        donneesEnCours = "";
                    }
				}
            }
            else if (paquet is PaquetDemandeLiberation)     //Paquet Demande Liberation
            {
                PaquetDemandeLiberation p = (PaquetDemandeLiberation)paquet;
				connexions.RetirerConnexion (p.numero_connexion);
            }

			if (reponse != null) {
				Utility.EcrireDansFichier ("L_lec.txt", reponse.ToString(), true);
			}

            return reponse;
        }


	}
}

