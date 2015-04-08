using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReseauxOrdinateur
{
	

    class Paquet
    {
		public const char FIN_PAQUET = '|';
    }

    class PaquetConnexion : Paquet
    {

    }

    class PaquetAppel : PaquetConnexion
    {

    }

    class PaquetConnexionEtablie : PaquetConnexion
    {

    }
}
