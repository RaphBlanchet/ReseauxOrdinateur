using System;
using System.Collections.Generic;

namespace ReseauxOrdinateur
{
	public class TableConnexionLiaison
	{
		List<ConnexionLiaison> listeConnexions;
		public TableConnexionLiaison ()
		{
			listeConnexions = new List<ConnexionLiaison>();
		}

		public int nbConnexions{
			get{ return listeConnexions.Count; }
		}

		public ConnexionLiaison AjouterConnexion(int _no, int _adrSource, int _adrDestination){
			ConnexionLiaison conn = new ConnexionLiaison (_no, _adrSource, _adrDestination);
			listeConnexions.Add (conn);
			return conn;
		}

		public void RetirerConnexion(int _no){
			ConnexionLiaison conn = findConnexion(_no);
			listeConnexions.Remove (conn);
		}

		public ConnexionLiaison findConnexion(int _no){
			ConnexionLiaison conn = null ;
			foreach(ConnexionLiaison c in listeConnexions){
				if (c.numeroConnexion == _no)
				{
					conn = c;
					break;
				}
			}

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

