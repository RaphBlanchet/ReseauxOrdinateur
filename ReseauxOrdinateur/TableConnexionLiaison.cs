using System;
using System.Collections.Generic;
using System.Threading;

namespace ReseauxOrdinateur
{
	public class TableConnexionLiaison
	{
		List<ConnexionLiaison> listeConnexions;
        static Semaphore sem = new Semaphore(1, 3);

		public TableConnexionLiaison ()
		{
			listeConnexions = new List<ConnexionLiaison>();
		}

		public int nbConnexions{
			get{ return listeConnexions.Count; }
		}

		public ConnexionLiaison AjouterConnexion(int _no, int _adrSource, int _adrDestination){
            sem.WaitOne();
			ConnexionLiaison conn = new ConnexionLiaison (_no, _adrSource, _adrDestination);
			listeConnexions.Add (conn);
            sem.Release();
			return conn;
		}

		public void RetirerConnexion(int _no){
            sem.WaitOne();
			ConnexionLiaison conn = findConnexion(_no);
			listeConnexions.Remove (conn);
            sem.Release();
		}

		public ConnexionLiaison findConnexion(int _no){
			ConnexionLiaison conn = null ;
            sem.WaitOne();
			foreach(ConnexionLiaison c in listeConnexions){
				if (c.numeroConnexion == _no)
				{
					conn = c;
					break;
				}
			}
            sem.Release();

			return conn;
		}

	}

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

