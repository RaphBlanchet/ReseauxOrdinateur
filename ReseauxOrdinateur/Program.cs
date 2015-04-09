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
			AnonymousPipeServerStream transportOut = new AnonymousPipeServerStream (PipeDirection.Out);
			AnonymousPipeClientStream reseauIn = new AnonymousPipeClientStream (PipeDirection.In, transportOut);

			AnonymousPipeServerStream reseauOut = new AnonymousPipeServerStream("reseauout", PipeDirection.Out);
			AnonymousPipeClientStream transportIn = new AnonymousPipeClientStream(".", "reseauout", PipeDirection.In);

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
