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
			/*NamedPipeServerStream transportOut = new NamedPipeServerStream("transportout", PipeDirection.Out);
			NamedPipeClientStream reseauIn = new NamedPipeClientStream (".", "transportout", PipeDirection.In);

			NamedPipeServerStream reseauOut = new NamedPipeServerStream("reseauout", PipeDirection.Out);
			NamedPipeClientStream transportIn = new NamedPipeClientStream(".", "reseauout", PipeDirection.In);*/

			EntiteReseau ER = new EntiteReseau();
			EntiteTransport ET = new EntiteTransport();

			//Starting the threads
			Thread t_ER = new Thread(new ThreadStart(ER.ThreadRun));
			Thread t_ET = new Thread(new ThreadStart(ET.ThreadRun));

			t_ER.Start();
			t_ET.Start();

			Thread.Sleep(10000);

			t_ER.Abort();
			t_ET.Abort();

			Console.Read();
		}
	}
}
