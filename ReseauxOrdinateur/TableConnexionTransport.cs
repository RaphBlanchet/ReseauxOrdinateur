using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace ReseauxOrdinateur
{
    public class ConnexionTransport{
		public int numeroConnexion;
        public string identifiant;
        public int adresseSource;
        public int adresseDestinataire;
        public EtatConnexion etat;

        public ConnexionTransport(int _num, string _identifiant, int _adresseSource, int _adresseDestinataire){
			numeroConnexion = _num;
            identifiant = _identifiant;
            adresseSource = _adresseSource;
            adresseDestinataire = _adresseDestinataire;
			etat = EtatConnexion.ATTENTE_ETABLISSEMENT;
        }
    }

    class TableConnexionTransport
    {
        List<ConnexionTransport> listeConnexions;
        bool[] adressesUtilises;
        int nbAdressesUtilises = 0;
		static int nbConnexionsTotales = 0;
        static Semaphore sem = new Semaphore(1, 3);

        public TableConnexionTransport()
        {
            listeConnexions = new List<ConnexionTransport>();
            adressesUtilises = new bool[250];
        }

		public int nbConnexions{
			get{ return listeConnexions.Count; }
		}

        public ConnexionTransport EtablirConnexion(string _identifiant)
        {
            int adresseSource = GenererAdresse();
            int adresseDestinataire = GenererAdresse();

            sem.WaitOne();
            ConnexionTransport conn = new ConnexionTransport(nbConnexionsTotales, _identifiant, adresseSource, adresseDestinataire);
            listeConnexions.Add(conn);
			nbConnexionsTotales++;
            sem.Release();

            return conn;
        }

        public bool ContientConnexion(string _identifiant)
        {
            return (this[_identifiant] != null);
        }
        
        public int GenererAdresse()
        {
            if (nbAdressesUtilises >= 250)
                return -1;

            Random rand = new Random();
            int adresse = 0;

            do
            {
                adresse = rand.Next(250);
            } while (adressesUtilises[adresse] == true);

            adressesUtilises[adresse] = true;
            nbAdressesUtilises++;

            return adresse;
        }

        public void ConfirmerConnexion(int _numConn)
        {
            ConnexionTransport conn = this[_numConn];
            conn.etat = EtatConnexion.CONNECTE;
            Console.WriteLine("Connexion établie pour " + conn.identifiant);
            Utility.EcrireDansFichier("S_ecr.txt", "Connexion établie pour " + conn.identifiant, true);
        }

		public void FermerConnexion(int _numConn, String raison){
			ConnexionTransport conn = this [_numConn];
            sem.WaitOne();
			listeConnexions.Remove (conn);
            sem.Release();
			Console.WriteLine ("Fermeture de connexion pour " + conn.identifiant + " - " + raison);
			Utility.EcrireDansFichier ("S_ecr.txt", "Fermeture de connexion pour " + conn.identifiant + " - " + raison, true);
		}

		public void FermerConnexion(string identifiant, String raison){
			this.FermerConnexion (this [identifiant].numeroConnexion, raison);
		}

		public ConnexionTransport findConnexionAtIndex(int i){
			return listeConnexions [i];
		}

        public ConnexionTransport this[int numConn]
        {
            get
            {
				ConnexionTransport conn = null ;
                sem.WaitOne();
				for(int i = 0; i < listeConnexions.Count; i++){
					try{
						ConnexionTransport c = listeConnexions[i];
						if (c.numeroConnexion == numConn)
						{
							conn = c;
							break;
						}
					}catch(IndexOutOfRangeException e){
                        break;
					}
				}
                sem.Release();

                return conn;
            }
        }

        public ConnexionTransport this[String identifiant]
        {
            get
            {
                ConnexionTransport conn = null ;
                sem.WaitOne();
				for(int i = 0; i < listeConnexions.Count; i++){
					try{
						ConnexionTransport c = listeConnexions[i];
	                    if (c.identifiant.Equals(identifiant))
	                    {
	                        conn = c;
	                        break;
	                    }
					}catch(IndexOutOfRangeException e){
                        break;
					}
                }
                sem.Release();

                return conn;
            }
        }
    }
}
