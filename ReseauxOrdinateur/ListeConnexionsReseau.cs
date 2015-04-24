/*	ListeConnexionsReseau.cs
 * 	Par Raphaël Blanchet, Catherine Béliveau, Joel Gbalou, Sébastien Piché Aubin et Manfouss Lawani
 * 	Créé le 1er Avril 2015
 * 	Classe permettant de gérer les différentes connexions de la couche Reseau
 */

using System;
using System.Collections.Generic;
using System.Threading;

namespace ReseauxOrdinateur
{
	//Classe représentant une connexion gérée par la couche Reseau
	public class ConnexionReseau{
        public int numeroConnexion;			//Numéro de voie logique
        public int adresseSource;			//Adresse source
        public int adresseDestinataire;		//Adresse destination
		public int niec;					//Numéro d'Identification d'Extremité de Connexion
		public int pr;						//P(R)
		public int ps;						//P(S)

		//Constructeur de la classe ConnexionReseau
		public ConnexionReseau(int _num, int _adresseSource, int _adresseDestinataire, int _niec){
            numeroConnexion = _num;
            adresseSource = _adresseSource;
            adresseDestinataire = _adresseDestinataire;
			niec = _niec;
			pr = 0;
			ps = 0;
		}

	}

	//Classe contenant la liste des connexions de Reseau
	public class ListeConnexionsReseau
	{
		List<ConnexionReseau> listeConnexions;			//Liste de connexions
		static int connexionsTotales = 0;				//Nombre total de connexions générées depuis le début de l'exécution du programme
        static Semaphore sem = new Semaphore(1, 1);		//Sémaphore permettant le blocage de la modification de la liste de connexions

		//Constructeur
		public ListeConnexionsReseau ()
		{
			listeConnexions = new List<ConnexionReseau> ();
		}

		//Accesseur du nombre de connexions dans la liste
		public int nbConnexions{
			get{ return listeConnexions.Count; }
		}

		//Fonction permettant d'établir une nouvelle connexion
		public ConnexionReseau EtablirConnexion(int addrSrouce, int addreDest, int niec){
            sem.WaitOne();	//Blocage
			//Création de la connexion
			ConnexionReseau conn = new ConnexionReseau (connexionsTotales, addrSrouce, addreDest, niec);
			listeConnexions.Add (conn);
			connexionsTotales ++;
            sem.Release();	//Déblocage
			return conn;
		}

		//Fonction permettant de retirer une connexion dans la table
		public void RetirerConnexion(ConnexionReseau conn){
            sem.WaitOne();
			listeConnexions.Remove (conn);
            sem.Release();
		}

		//Fonction permettant de modifier le P(S) d'une connexion
		public void ModifierPS(int num, int val){
			ConnexionReseau conn = this.findConnexionWithNum (num);
			conn.ps = (conn.ps + val)%8;
		}

		//Fonction permettant de modifier le P(R) d'une connexion
		public void ModifierPR(int num, int val){
			ConnexionReseau conn = this.findConnexionWithNum (num);
			conn.pr = (conn.pr + val)%8;
		}

		//Fontion permettant de retrouver une connexion avec le NIEC correspondant
		public ConnexionReseau findConnexionWithNIEC(int niec){
			ConnexionReseau conn = null ;
            sem.WaitOne();	//Blocage
			foreach(ConnexionReseau c in listeConnexions){
				if (c.niec == niec)
				{
					conn = c;
					break;
				}
			}
            sem.Release();	//Déblocage
			return conn;
		}

		//Fonction permettant de retrouver une connexion selon son numéro de voie logique
		public ConnexionReseau findConnexionWithNum(int num){
			ConnexionReseau conn = null ;
            sem.WaitOne();	//Blocage
			foreach(ConnexionReseau c in listeConnexions){
				if (c.numeroConnexion == num)
				{
					conn = c;
					break;
				}
			}
            sem.Release();	//Déblocage
			return conn;
		}
	}
}

