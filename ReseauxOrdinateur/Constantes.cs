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

        public const string RAISON_REFUS_DISTANT = "Refus du distant";
        public const string RAISON_REFUS_FOURNISSEUR = "Refus du fournisseur";
        public const string RAISON_REFUS_DECONNEXION = "00000100";

		public const ConsoleColor TRANSPORT_RESEAU_COLOR = ConsoleColor.Magenta;
		public const ConsoleColor RESEAU_LIAISON_COLOR = ConsoleColor.DarkGreen;
		public const ConsoleColor INPUT_COLOR = ConsoleColor.Yellow;
		public const ConsoleColor OUTPUT_COLOR = ConsoleColor.Gray;
		public const ConsoleColor ERREUR_COLOR = ConsoleColor.Red;
	}
}

