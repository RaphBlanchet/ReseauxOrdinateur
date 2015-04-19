using System;
using System.Collections.Generic;

namespace ReseauxOrdinateur
{
	public class ConnexionReseau{
        public int numeroConnexion;
        public int adresseSource;
        public int adresseDestinataire;
		public int niec;
		public int pr;
		public int ps;
		public ConnexionReseau(int _num, int _adresseSource, int _adresseDestinataire, int _niec){
            numeroConnexion = _num;
            adresseSource = _adresseSource;
            adresseDestinataire = _adresseDestinataire;
			niec = _niec;
			pr = 0;
			ps = 0;
		}

	}
	public class TableConnexionReseau
	{
		
		List<ConnexionReseau> listeConnexions;
		static int nbConnexions = 0;

		public TableConnexionReseau ()
		{
			listeConnexions = new List<ConnexionReseau> ();
		}

		public ConnexionReseau EtablirConnexion(int addrSrouce, int addreDest, int niec){
			ConnexionReseau conn = new ConnexionReseau (nbConnexions, addrSrouce, addreDest, niec);
			listeConnexions.Add (conn);
			nbConnexions ++;
			return conn;
		}

		public void RetirerConnexion(ConnexionReseau conn){
			listeConnexions.Remove (conn);
		}

		public void ModifierPS(int num, int val){
			ConnexionReseau conn = this.findConnexionWithNum (num);
			conn.ps = (conn.ps + val)%8;
		}

		public void ModifierPR(int num, int val){
			ConnexionReseau conn = this.findConnexionWithNum (num);
			conn.pr = (conn.pr + val)%8;
		}

		public ConnexionReseau findConnexionWithNIEC(int niec){
			ConnexionReseau conn = null ;
			foreach(ConnexionReseau c in listeConnexions){
				if (c.niec == niec)
				{
					conn = c;
					break;
				}
			}

			return conn;
		}

		public ConnexionReseau findConnexionWithNum(int num){
			ConnexionReseau conn = null ;
			foreach(ConnexionReseau c in listeConnexions){
				if (c.numeroConnexion == num)
				{
					conn = c;
					break;
				}
			}

			return conn;
		}
	}
}

