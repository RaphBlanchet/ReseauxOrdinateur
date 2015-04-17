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

		public ConnexionLiaison AjouterConnexion(int _no, int _adrSource, int _adrDestination){
			ConnexionLiaison conn = new ConnexionLiaison (_no, _adrSource, _adrDestination);
			listeConnexions.Add (conn);
			return conn;
		}

		public void RetirerConnexion(int _no){
			
		}

	}

	public class ConnexionLiaison
	{
		int numeroConnexion;
		int adresseSource;
		int adresseDestination;

		public ConnexionLiaison(int _num, int _adrSource, int _adrDestination){
			numeroConnexion = _num;
			adresseSource = _adrSource;
			adresseDestination = _adrDestination;
		}
	}
}

