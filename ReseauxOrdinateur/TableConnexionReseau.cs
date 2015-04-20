using System;
using System.Collections.Generic;
using System.Threading;

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
		static int connexionsTotales = 0;
        static Semaphore sem = new Semaphore(1, 3);

		public TableConnexionReseau ()
		{
			listeConnexions = new List<ConnexionReseau> ();
		}

		public int nbConnexions{
			get{ return listeConnexions.Count; }
		}

		public ConnexionReseau EtablirConnexion(int addrSrouce, int addreDest, int niec){
            sem.WaitOne();
			ConnexionReseau conn = new ConnexionReseau (connexionsTotales, addrSrouce, addreDest, niec);
			listeConnexions.Add (conn);
			connexionsTotales ++;
            sem.Release();
			return conn;
		}

		public void RetirerConnexion(ConnexionReseau conn){
            sem.WaitOne();
			listeConnexions.Remove (conn);
            sem.Release();
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
            sem.WaitOne();
			foreach(ConnexionReseau c in listeConnexions){
				if (c.niec == niec)
				{
					conn = c;
					break;
				}
			}
            sem.Release();
			return conn;
		}

		public ConnexionReseau findConnexionWithNum(int num){
			ConnexionReseau conn = null ;
            sem.WaitOne();
			foreach(ConnexionReseau c in listeConnexions){
				if (c.numeroConnexion == num)
				{
					conn = c;
					break;
				}
			}
            sem.Release();
			return conn;
		}
	}
}

