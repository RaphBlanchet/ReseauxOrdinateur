using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Pipes;

namespace ReseauxOrdinateur
{
	class Program
	{
		static void Main(string[] args)
		{
			NamedPipeServerStream transportOut = new NamedPipeServerStream("transportOut");
			NamedPipeClientStream reseauIn = new NamedPipeClientStream("transportOut");

			NamedPipeServerStream reseauOut = new NamedPipeServerStream("reseauOut");
			NamedPipeClientStream transportIn = new NamedPipeClientStream("reseauOut");

			EntiteReseau ER = new EntiteReseau(reseauIn, reseauOut);
			EntiteTransport ET = new EntiteTransport(transportIn, transportOut);

			//Starting the threads
			Thread t_ER = new Thread(new ThreadStart(ER.ThreadRun));
			Thread t_ET = new Thread(new ThreadStart(ET.ThreadRun));

			t_ER.Start();
			t_ET.Start();

			Thread.Sleep(1000);

			t_ER.Abort();
			t_ET.Abort();

			Console.Read();
		}
	}
}
