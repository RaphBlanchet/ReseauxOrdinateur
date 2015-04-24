/*	Program.cs
 * 	Par Raphaël Blanchet, Catherine Béliveau, Joel Gbalou, Sébastien Piché Aubin et Manfouss Lawani
 * 	Créé le 1er Avril 2015
 * 	Fichier contenant la fonction principale d'exécution du programme
 */
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
		//Fonction d'exécution du programme
		static void Main(string[] args)
		{
			//On supprime les fichiers de sorties qui sont déjà présents
			Utility.SupprimerFichier ("S_ecr.txt");
			Utility.SupprimerFichier ("L_lec.txt");
			Utility.SupprimerFichier ("L_ecr.txt");

			//Création des pipes pour permettre la communication inter-processus
			AnonymousPipeServerStream transportOut = new AnonymousPipeServerStream (PipeDirection.Out, System.IO.HandleInheritability.Inheritable);
			String transportID = transportOut.GetClientHandleAsString ();
			AnonymousPipeClientStream reseauIn = new AnonymousPipeClientStream (PipeDirection.In, transportID);

			AnonymousPipeServerStream reseauOut = new AnonymousPipeServerStream (PipeDirection.Out, System.IO.HandleInheritability.Inheritable);
			String reseauID = reseauOut.GetClientHandleAsString ();
			AnonymousPipeClientStream transportIn = new AnonymousPipeClientStream (PipeDirection.In, reseauID);

			//Création des différentes couches du protocole
			EntiteReseau ER = new EntiteReseau (reseauIn, reseauOut);
			EntiteTransport ET = new EntiteTransport (transportIn, transportOut);

			//Création des processus
			Thread t_ER = new Thread (new ThreadStart (ER.ThreadRun));
			Thread t_ET = new Thread (new ThreadStart (ET.ThreadRun));

			//Démarrage des processus
			t_ER.Start ();
			t_ET.Start ();

			//Lecture des commandes en entrée dans le fichier S_lec.txt
			ET.lireCommandes ();

			//On s'assure que tout à été traité avant de fermer les connexions
			Thread.Sleep (1000);

			//Fermeture des connexions
			Utility.AfficherDansConsole ("Plus de commandes, fermeture des connexions...", Constantes.OUTPUT_COLOR);
			ET.DemanderFermetureConnexions ();

			//On s'assure que toutes les connexions ont été fermées avant de poursuivre
			while (true)
				if (!ET.ContientConnexions ())
					break;
			
			//Thread.Sleep (1000);
			ET.isRunning = false;
			ER.isRunning = false;

            t_ET.Abort();
            t_ER.Abort();

			//Exécution terminée, fermeture du programme
			Utility.AfficherDansConsole("\nExécution terminée! Appuyez sur Enter pour terminer...", Constantes.OUTPUT_COLOR);
			Console.Read ();
		}
	}
}
