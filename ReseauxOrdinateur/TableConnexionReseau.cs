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

		public void EtablirConnexion(int addrSrouce, int addreDest, int niec){
			ConnexionReseau conn = new ConnexionReseau (nbConnexions, addrSrouce, addreDest, niec);
			listeConnexions.Add (conn);
			nbConnexions ++;
		}

		public void RetirerConnexion(int niec){
			ConnexionReseau conn = this [niec];
			listeConnexions.Remove (conn);
		}

		public void ModifierPS(int niec, int val){
			this [niec].ps += val;
		}

		public void ModifierPR(int niec, int val){
			this [niec].pr += val;
		}

		/*public ConnexionReseau this[int i]
		{
			get
			{
				return listeConnexions[i];
			}

			set
			{
				listeConnexions[i] = value;
			}
		}*/

		public ConnexionReseau this[int niec]
		{
			get
			{
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
		}
	}
}

