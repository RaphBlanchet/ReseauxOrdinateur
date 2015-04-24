/*	ListeConnexionsTransport.cs
 * 	Par Raphaël Blanchet, Catherine Béliveau, Joel Gbalou, Sébastien Piché Aubin et Manfouss Lawani
 * 	Créé le 1er Avril 2015
 * 	Classe permettant de gérer les différentes connexions de la couche Transport
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace ReseauxOrdinateur
{
	//Classe représentant une connexion gérée par la couche Transport
    public class ConnexionTransport{
		private int numeroConnexion;			//Numéro de connexion
		private string identifiant;			//Identificateur de l'application (ex. MSG)
		private int adresseSource;			//Adresse source de la connexion
		private int adresseDestination;		//Adresse destination de la connexion
		private EtatConnexion etat;			//État en cours de la connexion

		//Constructeur de la classe ConnexionTransport
        public ConnexionTransport(int _num, string _identifiant, int _adresseSource, int _adresseDestinataire){
			numeroConnexion = _num;
            identifiant = _identifiant;
            adresseSource = _adresseSource;
            adresseDestination = _adresseDestinataire;
			etat = EtatConnexion.ATTENTE_ETABLISSEMENT;
        }

		public int getNumeroConnexion(){
			return numeroConnexion;
		}

		public string getIdentifiant(){
			return identifiant;
		}

		public int getAdresseSource(){
			return adresseSource;
		}

		public int getAdresseDestination(){
			return adresseDestination;
		}

		public EtatConnexion getEtatConnexion(){
			return etat;
		}

		public void setEtatConnexion(EtatConnexion e){
			this.etat = e;
		}
    }

	//Classe contenant la liste des connexions de Transport
    class ListeConnexionsTransport
    {
        List<ConnexionTransport> listeConnexions;		//Table contenant les connexions de Transport
        bool[] adressesUtilises;						//Table contenant les adresses déjà utilisées, empêchant de générer les mêmes
        int nbAdressesUtilises = 0;						//Nombre d'adresses utilisées
		static int nbConnexionsTotales = 0;				//Nombre de connexions totales générées depuis le début du programme
        static Semaphore sem = new Semaphore(1, 1);		//Sémaphore permettant le blocage de la modification de la liste de connexions

		//Constructeur de la classe ListeConnexionsTransport
        public ListeConnexionsTransport()
        {
            listeConnexions = new List<ConnexionTransport>();
            adressesUtilises = new bool[250];
        }

		//Accesseur du nombre de connexions dans la table listeConnexions
		public int nbConnexions{
			get{ return listeConnexions.Count; }
		}

		//Fonction permettant d'établir une nouvelle connexion
        public ConnexionTransport EtablirConnexion(string _identifiant)
        {
			//On génère deux adresses
            int adresseSource = GenererAdresse();
            int adresseDestinataire = GenererAdresse();

			//Plus d'adresses disponibles
			if (adresseSource == -1 || adresseDestinataire == -1) {
				Utility.AfficherDansConsole ("Plus d'adresses disponibles! Impossible d'établir la connexion.", Constantes.ERREUR_COLOR);
				return null;
			}

            sem.WaitOne();	//Blocage
            ConnexionTransport conn = new ConnexionTransport(nbConnexionsTotales, _identifiant, adresseSource, adresseDestinataire);
            listeConnexions.Add(conn);
			nbConnexionsTotales++;
            sem.Release();	//Déblocage

            return conn;
        }

		//Fonction permettant de déterminer si la table contient une connexion selon l'identifiant passée en paramètre
        public bool ContientConnexion(string _identifiant)
        {
            return (this[_identifiant] != null);
        }
        
		//Fonction permettant de générer une adresse aléatoire
        public int GenererAdresse()
        {
			//Plus d'adresses disponibles
            if (nbAdressesUtilises >= 250)
                return -1;

            Random rand = new Random();
            int adresse = 0;

			//Génération d'une adresse aléatoire unique
            do
            {
                adresse = rand.Next(250);
            } while (adressesUtilises[adresse] == true);

			//On notifie que l'adresse est maintenant utilisée
            adressesUtilises[adresse] = true;
            nbAdressesUtilises++;

            return adresse;
        }

		//Fonction permettant de confirmer une connexion
        public void ConfirmerConnexion(int _numConn)
        {
            ConnexionTransport conn = this[_numConn];
			conn.setEtatConnexion(EtatConnexion.CONNECTE);

			//Affichage en console et écriture dans le fichier de sortie
			Utility.AfficherDansConsole("Connexion établie pour " + conn.getIdentifiant(), Constantes.OUTPUT_COLOR);
			Utility.EcrireDansFichier("S_ecr.txt", "Connexion établie pour " + conn.getIdentifiant(), true);
        }

		//Fermeture d'une connexion selon son numéro de connexion
		public void FermerConnexion(int _numConn, String raison){
			ConnexionTransport conn = this [_numConn];
            sem.WaitOne();	//Blocage
			listeConnexions.Remove (conn);

			//Libération des adresses utilisées
			adressesUtilises [conn.getAdresseSource()] = false;
			adressesUtilises [conn.getAdresseDestination()] = false;
			nbAdressesUtilises -= 2;

            sem.Release();	//Déblocage

			//Affichage en console et écriture dans le fichier de sortie
			Utility.AfficherDansConsole("Fermeture de connexion pour " + conn.getIdentifiant() + " - " + raison, Constantes.OUTPUT_COLOR);
			Utility.EcrireDansFichier ("S_ecr.txt", "Fermeture de connexion pour " + conn.getIdentifiant() + " - " + raison, true);
		}

		//Fonction permettant de fermer une connexion selon on identifiant d'application
		public void FermerConnexion(string identifiant, String raison){
			this.FermerConnexion (this [identifiant].getNumeroConnexion(), raison);
		}

		//Fonction permettant de trouver une connexion selon son index dans la liste
		public ConnexionTransport findConnexionAtIndex(int i){
			return listeConnexions [i];
		}

		//Fonction permettant de trouver une connexion selon son numéro de connexion
        public ConnexionTransport this[int numConn]
        {
            get
            {
				ConnexionTransport conn = null ;
                sem.WaitOne();	//Blocage
				for(int i = 0; i < listeConnexions.Count; i++){
					try{
						ConnexionTransport c = listeConnexions[i];
						if (c.getNumeroConnexion() == numConn)
						{
							conn = c;
							break;
						}
					}catch(IndexOutOfRangeException e){
                        break;
					}
				}
                sem.Release();	//Déblocage

                return conn;
            }
        }

		//Fonction permettant de trouver une connexion selon son identifiant d'application
        public ConnexionTransport this[String identifiant]
        {
            get
            {
                ConnexionTransport conn = null ;
                sem.WaitOne();	//Blocage
				for(int i = 0; i < listeConnexions.Count; i++){
					try{
						ConnexionTransport c = listeConnexions[i];
						if (c.getIdentifiant().Equals(identifiant))
	                    {
	                        conn = c;
	                        break;
	                    }
					}catch(IndexOutOfRangeException e){
                        break;
					}
                }
                sem.Release();	//Déblocage

                return conn;
            }
        }
    }
}
