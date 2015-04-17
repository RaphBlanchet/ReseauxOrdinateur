using System;

namespace ReseauxOrdinateur
{
	public enum EtatConnexion{ATTENTE_ETABLISSEMENT, CONNECTE};

	public abstract class Constantes
	{
		public const char FIN_PAQUET = '|';
		public const string TYPE_PAQUET_APPEL = "00001011";
		public const string TYPE_PAQUET_CONNEXION_ETABLIE = "00001111";
		public const string TYPE_PAQUET_LIBERATION = "00010011";
		public const string TYPE_PAQUET_ACQUITTEMENT_POSITIF = "00001";
		public const string TYPE_PAQUET_ACQUITTEMENT_NEGATIF = "01001";

        public const string RAISON_REFUS_DISTANT = "00000001";
        public const string RAISON_REFUS_FOURNISSEUR = "00000010";
        public const string RAISON_REFUS_DECONNEXION = "00000100";
	}
}

