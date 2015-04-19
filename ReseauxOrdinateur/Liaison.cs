using System;

namespace ReseauxOrdinateur
{
	public class Liaison
	{
        TableConnexionLiaison connexions = null;
		public Liaison ()
		{
            connexions = new TableConnexionLiaison();
		}

        public Paquet TraiterPaquetDeReseau(Paquet paquet)
        {
            Console.WriteLine("Liaison recoit de réseau : " + paquet.ToPaquetString());

            Paquet reponse = null;
            if (paquet is PaquetAppel)                      //Paquet d'appel
            {
                PaquetAppel p = (PaquetAppel)paquet;
                int addrSource = p.adresseSource;

                if (addrSource % 13 == 0) //REFUS DE LA CONNEXION DU DISTANT
                {
                    reponse = new PaquetIndicationLiberation(p.numero_connexion, p.adresseSource, p.adresseDestination, Constantes.RAISON_REFUS_DISTANT);
                }
                else                      //ACCEPTATION DE LA CONNEXION
                {
                    connexions.AjouterConnexion(p.numero_connexion, p.adresseSource, p.adresseDestination);
                    reponse = new PaquetConnexionEtablie(p.numero_connexion, p.adresseSource, p.adresseDestination);
                }
            }
            else if (paquet is PaquetDonnees)               //Paquet de données
            {
				ConnexionLiaison conn = connexions.findConnexion (paquet.numero_connexion);
				if (conn.adresseSource % 15 == 0) {
					reponse = null; //Pas de reponse
				} else {
					PaquetDonnees p = (PaquetDonnees)paquet;
					Console.WriteLine("Donnees recues : " + p.donnees);
					reponse = new PaquetAcquittement(p.numero_connexion, p.pR, true);
				}
            }
            else if (paquet is PaquetDemandeLiberation)     //Paquet Demande Liberation
            {
                PaquetDemandeLiberation p = (PaquetDemandeLiberation)paquet;
                reponse = new PaquetIndicationLiberation(p.numero_connexion, p.adresseSource, p.adresseDestination, "Deconnexion");
            }

            return reponse;
        }


	}
}

