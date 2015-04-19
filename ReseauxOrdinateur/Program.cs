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
			//On supprime les fichiers de sorties qui sont déjà présents
			Utility.SupprimerFichier("S_ecr.txt");
			Utility.SupprimerFichier ("L_lec.txt");
			Utility.SupprimerFichier ("L_ecr.txt");

			//Création des pipes pour permettre la communication inter-processus
			AnonymousPipeServerStream transportOut = new AnonymousPipeServerStream (PipeDirection.Out, System.IO.HandleInheritability.Inheritable);
			String transportID = transportOut.GetClientHandleAsString();
			AnonymousPipeClientStream reseauIn = new AnonymousPipeClientStream (PipeDirection.In, transportID);

			AnonymousPipeServerStream reseauOut = new AnonymousPipeServerStream(PipeDirection.Out, System.IO.HandleInheritability.Inheritable);
			String reseauID = reseauOut.GetClientHandleAsString();
			AnonymousPipeClientStream transportIn = new AnonymousPipeClientStream(PipeDirection.In, reseauID);

			//Création des différentes couches du protocol
			EntiteReseau ER = new EntiteReseau(reseauIn, reseauOut);
			EntiteTransport ET = new EntiteTransport(transportIn, transportOut);

			//Starting the threads
			Thread t_ER = new Thread(new ThreadStart(ER.ThreadRun));
			Thread t_ET = new Thread(new ThreadStart(ET.ThreadRun));

			t_ER.Start();
			t_ET.Start();

			ET.lireCommandes ();

			Console.WriteLine ("Plus de commandes, fermeture des connexions...");
			ET.DemanderFermetureConnexions ();

			Thread.Sleep (500);
			ET.isRunning = false;
			ER.isRunning = false;

			Console.WriteLine ("Exécution terminée!");
			Console.Read ();

			//Thread.Sleep(10000);

			//t_ER.Abort();
			//t_ET.Abort();

			//Console.Read();
		}
	}
}
