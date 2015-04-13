using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReseauxOrdinateur
{
	

    public abstract class Paquet			//Classe de définition des champs
    {
		public int numero_connexion;
		public string typePaquet;

		public Paquet(int _num){
			numero_connexion = _num;
		}

		public abstract string ToPaquetString ();
    }

    class PaquetConnexion : Paquet
    {
		public int adresseSource;
		public int adresseDestination;

		public PaquetConnexion(int _num, int _addrSource, int _addrDestination) : base(_num){
			adresseSource = _addrSource;
			adresseDestination = _addrDestination;
		}

		public override string ToPaquetString ()
		{
			return numero_connexion + ";" + typePaquet + ";" + adresseSource + ";" + adresseDestination;
		}
    }

    class PaquetAppel : PaquetConnexion		//Classe d'initialisation des champs
    {
		public PaquetAppel(int _num, int _addrSource, int _addrDestination) : base(_num, _addrSource, _addrDestination){
			typePaquet = Constantes.TYPE_PAQUET_APPEL;
		}
    }

    class PaquetConnexionEtablie : PaquetConnexion
    {
		public PaquetConnexionEtablie(int _num, int _addrSource, int _addrDestination) : base(_num, _addrSource, _addrDestination){
			typePaquet = Constantes.TYPE_PAQUET_CONNEXION_ETABLIE;
		}
    }

	class PaquetIndicationLiberation : PaquetConnexion{
		string raison;
		public PaquetIndicationLiberation(int _num, int _addrSource, int _addrDestination, string _raison) : base(_num, _addrSource, _addrDestination){
			typePaquet = Constantes.TYPE_PAQUET_LIBERATION;
			raison = _raison;
		}

		public override string ToPaquetString ()
		{
			return base.ToPaquetString () + ";" + raison;
		}
	}

	class PaquetDonnees : Paquet{
		int pR, pS, M;
		string donnees;

		public PaquetDonnees(int _num, int _pr, int _ps, int _m, string _donnees) : base(_num){
			pR = _pr;
			pS = _ps;
			M = _m;
			//typePaquet = string.Format ("{0:D3}{1:D}{2:D3}0", _pr%8, _ps%8, _m);
			typePaquet = string.Format("{0:D3}{1:D}{0:D3}0", Convert.ToString(pR%8, 2), M, Convert.ToString(pS%8, 2));
			donnees = _donnees;
		}

		public override string ToPaquetString ()
		{
			return numero_connexion + ";" + typePaquet + ";" + donnees;
		}
	}

	class PaquetAcquittement : Paquet{
		bool isPositif;
		int pR;

		public PaquetAcquittement(int _num, int _pr, bool _pos) : base(_num){
			isPositif = _pos;
			pR = _pr;
			string acquittement = (isPositif ? Constantes.TYPE_PAQUET_ACQUITTEMENT_POSITIF : Constantes.TYPE_PAQUER_ACQUITTEMENT_NEGATIF);
			typePaquet = string.Format ("{0:D3}" + acquittement, Convert.ToString (pR, 2));
		}

		public override string ToPaquetString ()
		{
			return numero_connexion + ";" + typePaquet;
		}
	}

	class PaquetDemandeLiberation : PaquetConnexion{
		public PaquetDemandeLiberation(int _num, int _addrSource, int _addrDestination) : base(_num, _addrSource, _addrDestination){
			typePaquet = Constantes.TYPE_PAQUET_LIBERATION;
		}
	}
}
