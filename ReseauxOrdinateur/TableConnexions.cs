using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ReseauxOrdinateur
{
    public class Connexion{
		public int numeroConnexion;
        public string identifiant;
        public int adresseSource;
        public int adresseDestinataire;
        public EtatConnexion etat;

        public Connexion(int _num, string _identifiant, int _adresseSource, int _adresseDestinataire){
			numeroConnexion = _num;
            identifiant = _identifiant;
            adresseSource = _adresseSource;
            adresseDestinataire = _adresseDestinataire;
			etat = EtatConnexion.ATTENTE_ETABLISSEMENT;
        }
    }

    class TableConnexions
    {
        List<Connexion> listeConnexions;
        bool[] adressesUtilises;
        int nbAdressesUtilises = 0;
		static int nbConnexions = 0;

        public TableConnexions()
        {
            listeConnexions = new List<Connexion>();
            adressesUtilises = new bool[250];
        }

        public Connexion EtablirConnexion(string _identifiant)
        {
            int adresseSource = GenererAdresse();
            int adresseDestinataire = GenererAdresse();

            Connexion conn = new Connexion(nbConnexions, _identifiant, adresseSource, adresseDestinataire);
            listeConnexions.Add(conn);
			nbConnexions++;

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
			Connexion conn = this [_numConn];
			listeConnexions.Remove (conn);
			Console.WriteLine ("Connexion fermée pour " + conn.identifiant);
		}

		public void FermerConnexion(string identifiant){
			this.FermerConnexion (this [identifiant].numeroConnexion);
		}

        public Connexion this[int numConn]
        {
            get
            {
				Connexion conn = null ;
				for(int i = 0; i < listeConnexions.Count; i++){
					try{
						Connexion c = listeConnexions[i];
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

        public Connexion this[String identifiant]
        {
            get
            {
                Connexion conn = null ;
				for(int i = 0; i < listeConnexions.Count; i++){
					try{
						Connexion c = listeConnexions[i];
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
