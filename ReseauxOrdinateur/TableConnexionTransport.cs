using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

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

            ConnexionTransport conn = new ConnexionTransport(nbConnexionsTotales, _identifiant, adresseSource, adresseDestinataire);
            listeConnexions.Add(conn);
			nbConnexionsTotales++;

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
            this[_numConn].etat = EtatConnexion.CONNECTE;
            Console.WriteLine("Connexion établie pour " + this[_numConn].identifiant);
        }

		public void FermerConnexion(int _numConn){
			ConnexionTransport conn = this [_numConn];
			listeConnexions.Remove (conn);
			Console.WriteLine ("Fermeture de connexion pour " + conn.identifiant);
			Utility.EcrireDansFichier ("S_ecr.txt", "Fermeture de connexion pour " + conn.identifiant, true);
		}

		public void FermerConnexion(string identifiant){
			this.FermerConnexion (this [identifiant].numeroConnexion);
		}

		public ConnexionTransport findConnexionAtIndex(int i){
			return listeConnexions [i];
		}

        public ConnexionTransport this[int numConn]
        {
            get
            {
				ConnexionTransport conn = null ;
				for(int i = 0; i < listeConnexions.Count; i++){
					try{
						ConnexionTransport c = listeConnexions[i];
						if (c.numeroConnexion == numConn)
						{
							conn = c;
							break;
						}
					}catch(IndexOutOfRangeException e){
					}
				}

                return conn;
            }
        }

        public ConnexionTransport this[String identifiant]
        {
            get
            {
                ConnexionTransport conn = null ;
				for(int i = 0; i < listeConnexions.Count; i++){
					try{
						ConnexionTransport c = listeConnexions[i];
	                    if (c.identifiant.Equals(identifiant))
	                    {
	                        conn = c;
	                        break;
	                    }
					}catch(IndexOutOfRangeException e){
					}
                }

                return conn;
            }
        }
    }
}
