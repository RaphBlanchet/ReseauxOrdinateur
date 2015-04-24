/*	Paquet.cs
 * 	Par Raphaël Blanchet, Catherine Béliveau, Joel Gbalou, Sébastien Piché Aubin et Manfouss Lawani
 * 	Créé le 1er Avril 2015
 * 	Fichier contenant la définition de tous les paquets utilisables par les couches Réseau et liaison
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReseauxOrdinateur
{
    public abstract class Paquet			//Classe Mère de tous les types paquets
    {
		//Définition des champs communs à tout les types de paquet
		public int numero_connexion;		//Numéro de connexion
		public string typePaquet;			//Type de paquet

		//Constructeur de base
		public Paquet(int _num){
			numero_connexion = _num;
		}

		//Fonction abstraite devant être implémentée par tout les types de paquet
		public abstract string ToPaquetString ();
    }

	//Paquet définissant les types de connexion
    class PaquetConnexion : Paquet
    {
		public int adresseSource;		//Adresse source de la connexion
		public int adresseDestination;	//Adresse destination de la connexion

		//Constructeur commun à tous les paquets de type Connexion
		public PaquetConnexion(int _num, int _addrSource, int _addrDestination) : base(_num){
			adresseSource = _addrSource;
			adresseDestination = _addrDestination;
		}

		//Fonction permettant l'affichage formaté du paquet
		public override string ToPaquetString ()
		{
			return numero_connexion + ";" + typePaquet + ";" + adresseSource + ";" + adresseDestination;
		}

		//Fonction permettant un affichage poli du paquet
		public override string ToString ()
		{
			return "Numéro connexion : " + numero_connexion + " | Type Paquet : " + typePaquet + " | Adresse Source : " + adresseSource + " | Adresse Destination : " + adresseDestination;
		}
    }

	//Classe d'implémentation du paquet d'Appel
    class PaquetAppel : PaquetConnexion
    {
		//Constructeur du paquet d'appel
		public PaquetAppel(int _num, int _addrSource, int _addrDestination) : base(_num, _addrSource, _addrDestination){
			typePaquet = Constantes.TYPE_PAQUET_APPEL;
		}

		public override string ToString ()
		{
			return "Paquet : Appel | " + base.ToString ();
		}
    }

	//Classe d'implémentation du paquet de Connexion Établie
    class PaquetConnexionEtablie : PaquetConnexion
    {
		//Constructeur
		public PaquetConnexionEtablie(int _num, int _addrSource, int _addrDestination) : base(_num, _addrSource, _addrDestination){
			typePaquet = Constantes.TYPE_PAQUET_CONNEXION_ETABLIE;
		}

		public override string ToString ()
		{
			return "Paquet : ConnexionEtablie | " + base.ToString ();
		}
    }

	//Classe d'implémentation du paquet d'Indication de Libération de Connexion
	class PaquetIndicationLiberation : PaquetConnexion{
		public string raison;	//Raison de la déconnexion

		//Constructeur
		public PaquetIndicationLiberation(int _num, int _addrSource, int _addrDestination, string _raison) : base(_num, _addrSource, _addrDestination){
			typePaquet = Constantes.TYPE_PAQUET_LIBERATION;
			raison = _raison;
		}

		public override string ToPaquetString ()
		{
			return base.ToPaquetString () + ";" + raison;
		}

		public override string ToString ()
		{
			return "Paquet : IndicationLiberation | " + base.ToString ();
		}
	}

	//Classe d'implémentation du paquet de Demande de Libération de Connexion
	class PaquetDemandeLiberation : PaquetConnexion{
		//Constructeur
		public PaquetDemandeLiberation(int _num, int _addrSource, int _addrDestination) : base(_num, _addrSource, _addrDestination){
			typePaquet = Constantes.TYPE_PAQUET_LIBERATION;
		}

		public override string ToString ()
		{
			return "Paquet : DemandeLiberation | " + base.ToString ();
		}
	}

	//Classe d'implémentation du paquet de Données
	class PaquetDonnees : Paquet{
		public int pR, pS, M;		//P(R), P(S) et M du paquet
		public string donnees;		//Données du paquet

		//Constructeur du paquet de données
		public PaquetDonnees(int _num, int _pr, int _ps, int _m, string _donnees) : base(_num){
			pR = _pr%8;	//P(R) codé sur 3 bits
			pS = _ps%8;	//P(S) codé sur 3 bits
			M = _m;
            typePaquet = pR.ToString() + M.ToString() + pS.ToString() + "0";
			donnees = _donnees;
		}

		public override string ToPaquetString ()
		{
			return numero_connexion + ";" + typePaquet + ";" + donnees;
		}

		public override string ToString ()
		{
			return "Paquet : Donnees | Numero Connexion : " + numero_connexion + " | P(R) : " + pR + " | P(S) : " + pS + " | M : " + M + " | Données : " + donnees;
		}
	}

	//Classe d'implémentation du paquet d'Acquittement, négatif ou positif
	class PaquetAcquittement : Paquet{
		bool isPositif;		//Acquittement positif ou négatif
		int pR;				//P(R) du paquet
			
		//Constructeur du paquet d'acquittement
		public PaquetAcquittement(int _num, int _pr, bool _pos) : base(_num){
			isPositif = _pos;
			pR = _pr;
			//Acquittement positif ou négatif
			string acquittement = (isPositif ? Constantes.TYPE_PAQUET_ACQUITTEMENT_POSITIF : Constantes.TYPE_PAQUET_ACQUITTEMENT_NEGATIF);
            typePaquet = pR.ToString() + acquittement;
		}

		public override string ToPaquetString ()
		{
			return numero_connexion + ";" + typePaquet;
		}

		public override string ToString ()
		{
			return "Paquet : Acquittement | Numéro Connexion : " + numero_connexion + " | P(R) : " + pR + " | Type Paquet : " + typePaquet;
		}
	}
}
