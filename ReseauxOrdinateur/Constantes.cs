/*	Constantes.cs
 * 	Par Raphaël Blanchet, Catherine Béliveau, Joel Gbalou, Sébastien Piché Aubin et Manfouss Lawani
 * 	Créé le 1er Avril 2015
 * 	Fichier contenant plusieurs constantes utiles dans tout le programme
 */

using System;

namespace ReseauxOrdinateur
{
	public enum EtatConnexion{ATTENTE_ETABLISSEMENT, CONNECTE};

	public abstract class Constantes
	{
		public const char FIN_PRIMITIVE = '|';

		//Constantes de type de paquet
		public const string TYPE_PAQUET_APPEL = "00001011";
		public const string TYPE_PAQUET_CONNEXION_ETABLIE = "00001111";
		public const string TYPE_PAQUET_LIBERATION = "00010011";
		public const string TYPE_PAQUET_ACQUITTEMENT_POSITIF = "00001";
		public const string TYPE_PAQUET_ACQUITTEMENT_NEGATIF = "01001";

		//Raisons de déconnexion
        public const string RAISON_REFUS_DISTANT = "Refus du distant";
        public const string RAISON_REFUS_FOURNISSEUR = "Refus du fournisseur";
        public const string RAISON_REFUS_DECONNEXION = "Déconnexion";

		//Couleurs de la console par catégorie de communication
		public const ConsoleColor TRANSPORT_RESEAU_COLOR = ConsoleColor.Magenta;
		public const ConsoleColor RESEAU_LIAISON_COLOR = ConsoleColor.DarkGreen;
		public const ConsoleColor INPUT_COLOR = ConsoleColor.Yellow;
		public const ConsoleColor OUTPUT_COLOR = ConsoleColor.Gray;
		public const ConsoleColor ERREUR_COLOR = ConsoleColor.Red;
	}
}

