using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReseauxOrdinateur
{
    //enum N_CONNECT { req="N_CONNECT.req", ind="N_CONNECT.ind", resp="N_CONNECT.resp", conf="N_CONNECT.conf"};
    //enum N_DATA { req="N_DATA.req", ind="N_DATA.ind"};
    //enum N_DISCONNECT { req="N_DISCONNECT.req", ind="N_DISCONNECT.ind"};

	public abstract class Primitive{
		private readonly String name;
		private readonly int value;

		protected Primitive(int value, String name){
			this.name = name;
			this.value = value;
		}

		public override String ToString(){
			return name;
		}
	}

	public sealed class N_CONNECT : Primitive{
		private N_CONNECT(int v, String n) : base(v, n){}
		public static readonly N_CONNECT req = new N_CONNECT (1, "N_CONNECT.req");
		public static readonly N_CONNECT ind = new N_CONNECT (2, "N_CONNECT.ind");
		public static readonly N_CONNECT resp = new N_CONNECT (3, "N_CONNECT.resp");
		public static readonly N_CONNECT conf = new N_CONNECT (4, "N_CONNECT.conf");
	}

	public sealed class N_DATA : Primitive{
		private N_DATA(int v, String n) : base(v, n){}
		public static readonly N_DATA req = new N_DATA (1, "N_DATA.req");
		public static readonly N_DATA ind = new N_DATA (2, "N_DATA.ind");
	}

	public sealed class N_DISCONNECT : Primitive{
		private N_DISCONNECT(int v, String n) : base(v, n){}
		public static readonly N_DISCONNECT req = new N_DISCONNECT (1, "N_DISCONNECT.req");
		public static readonly N_DISCONNECT ind = new N_DISCONNECT (2, "N_DISCONNECT.ind");
	}
}
