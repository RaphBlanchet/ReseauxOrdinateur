using System;

namespace ReseauxOrdinateur
{
	public class Liaison
	{
		public Liaison ()
		{
		}

        public Paquet TraiterPaquetDeReseau(Paquet paquet)
        {
            Console.WriteLine("Liaison recoit de réseau : " + paquet.ToPaquetString());

            Paquet reponse = null;
            switch (paquet.typePaquet)
            {
                case Constantes.TYPE_PAQUET_APPEL:
                    PaquetAppel p = (PaquetAppel)paquet;
                    int addrSource = p.adresseSource;

                    if (addrSource % 13 == 0) //REFUS DE LA CONNEXION DU DISTANT
                    {
                        reponse = new PaquetIndicationLiberation(p.numero_connexion, p.adresseSource, p.adresseDestination, Constantes.RAISON_REFUS_DISTANT);
                    }
                    else                      //ACCEPTATION DE LA CONNEXION
                    {
                        reponse = new PaquetConnexionEtablie(p.numero_connexion, p.adresseSource, p.adresseDestination);
                    }

                    break;
            }

            return reponse;
        }


	}
}

