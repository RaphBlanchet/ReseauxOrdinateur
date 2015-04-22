/*	ListeConnexionsLiaison.cs
 * 	Par Raphaël Blanchet, Catherine Béliveau, Joel Gbalou, Sébastien Piché Aubin et Marfous Lawani
 * 	Créé le 1er Avril 2015
 * 	Classe permettant de gérer les différentes connexions de la couche Liaison
 */

using System;
using System.Collections.Generic;
using System.Threading;

namespace ReseauxOrdinateur
{
	public class ListeConnexionsLiaison
	{
		List<ConnexionLiaison> listeConnexions;			//Table contenant les connexions
        static Semaphore sem = new Semaphore(1, 1);		//Sémaphore bloquant la modification de la liste de connexions pour gérer la concurrence

		//Constructeur de la liste
		public ListeConnexionsLiaison ()
		{
			listeConnexions = new List<ConnexionLiaison>();
		}

		//Accesseur de la vairable nbConnexions
		public int nbConnexions{
			get{ return listeConnexions.Count; }
		}

		//Fonction permettant d'ajouter une nouvelle connexion à la liste
		public ConnexionLiaison AjouterConnexion(int _no, int _adrSource, int _adrDestination){
            sem.WaitOne();	//Blocage
			ConnexionLiaison conn = new ConnexionLiaison (_no, _adrSource, _adrDestination);
			listeConnexions.Add (conn);
            sem.Release();	//Déblocage
			return conn;
		}

		//Fonction permettant de retirer une connexion dans la liste
		public void RetirerConnexion(int _no){
            sem.WaitOne();	//Blocage
			ConnexionLiaison conn = findConnexion(_no);
			listeConnexions.Remove (conn);
            sem.Release();	//Déblocage
		}

		//Fonction permettant de trouver une connexion selon le numéro passé en paramètre
		public ConnexionLiaison findConnexion(int _no){
			ConnexionLiaison conn = null ;
            sem.WaitOne();	//Blocage
			foreach(ConnexionLiaison c in listeConnexions){
				if (c.numeroConnexion == _no)
				{
					conn = c;
					break;
				}
			}
            sem.Release();	//Déblocage

			return conn;
		}

	}

	//Classe représentant une connexion gérée par la couche Liaison
	public class ConnexionLiaison
	{
		public int numeroConnexion;
		public int adresseSource;
		public int adresseDestination;

		public ConnexionLiaison(int _num, int _adrSource, int _adrDestination){
			numeroConnexion = _num;
			adresseSource = _adrSource;
			adresseDestination = _adrDestination;
		}
	}
}

