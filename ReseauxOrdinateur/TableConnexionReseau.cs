using System;
using System.Collections.Generic;

namespace ReseauxOrdinateur
{
	public class ConnexionReseau : Connexion{
		public int niec;
		public int pr;
		public int ps;
		public ConnexionReseau(int _num, string _identifiant, int _adresseSource, int _adresseDestinataire, int _niec) : base(_num, _identifiant, _adresseSource, _adresseDestinataire){
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

		public void EtablirConnexion(string identifiant, int addrSrouce, int addreDest, int niec){
			ConnexionReseau conn = new ConnexionReseau (nbConnexions, identifiant, addrSrouce, addreDest, niec);
			listeConnexions.Add (conn);
			nbConnexions ++;
		}

		public void RetirerConnexion(string identifiant){
			ConnexionReseau conn = this [identifiant];
			listeConnexions.Remove (conn);
		}

		public void ModifierPS(string identifiant, int val){
			this [identifiant].ps += val;
		}

		public void ModifierPR(string identifiant, int val){
			this [identifiant].pr += val;
		}

		public ConnexionReseau this[int i]
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

		public ConnexionReseau this[String identifiant]
		{
			get
			{
				ConnexionReseau conn = null ;
				foreach(ConnexionReseau c in listeConnexions){
					if (c.identifiant.Equals(identifiant))
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

