using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ReseauxOrdinateur
{
    enum EtatConnexion{ATTENTE_ETABLISSEMENT, CONNECTE};

    class Connexion{
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

            Random rand = new Random(250);
            int adresse = 0;

            do
            {
                adresse = rand.Next();
            } while (adressesUtilises[adresse] == true);

            nbAdressesUtilises++;

            return adresse;
        }

        public Connexion this[int i]
        {
            get
            {
                return listeConnexions[i];
            }

            set
            {
                listeConnexions[i] = value;
            }
        }

        public Connexion this[String identifiant]
        {
            get
            {
                Connexion conn = null ;
                foreach(Connexion c in listeConnexions){
                    if (c.identifiant.Equals(identifiant))
                    {
                        conn = c;
                        break;
                    }
                }

                return conn;
            }

            set
            {

            }
        }
    }
}
