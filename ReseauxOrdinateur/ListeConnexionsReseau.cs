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
        private int numeroConnexion;			//Numéro de voie logique
		private int adresseSource;			//Adresse source
		private int adresseDestination;		//Adresse destination
		private int niec;					//Numéro d'Identification d'Extremité de Connexion
		private int pr;						//P(R)
		private int ps;						//P(S)

		//Constructeur de la classe ConnexionReseau
		public ConnexionReseau(int _num, int _adresseSource, int _adresseDestinataire, int _niec){
            numeroConnexion = _num;
            adresseSource = _adresseSource;
            adresseDestination = _adresseDestinataire;
			niec = _niec;
			pr = 0;
			ps = 0;
		}

		public int getNumeroConnexion(){
			return numeroConnexion;
		}

		public int getAdresseSource(){
			return adresseSource;
		}

		public int getAdresseDestination(){
			return adresseDestination;
		}

		public int getNIEC(){
			return niec;
		}

		public int getPR(){
			return pr;
		}

		public void setPR(int val){
			this.pr = val;
		}

		public int getPS(){
			return ps;
		}

		public void setPS(int val){
			this.ps = val;
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
			conn.setPS((conn.getPS() + val)%8);
		}

		//Fonction permettant de modifier le P(R) d'une connexion
		public void ModifierPR(int num, int val){
			ConnexionReseau conn = this.findConnexionWithNum (num);
			conn.setPR((conn.getPR() + val)%8);
		}

		//Fontion permettant de retrouver une connexion avec le NIEC correspondant
		public ConnexionReseau findConnexionWithNIEC(int niec){
			ConnexionReseau conn = null ;
            sem.WaitOne();	//Blocage
			foreach(ConnexionReseau c in listeConnexions){
				if (c.getNIEC() == niec)
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
				if (c.getNumeroConnexion() == num)
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

