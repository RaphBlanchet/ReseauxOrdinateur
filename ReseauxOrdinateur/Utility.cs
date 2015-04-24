/*	Program.cs
 * 	Par Raphaël Blanchet, Catherine Béliveau, Joel Gbalou, Sébastien Piché Aubin et Manfouss Lawani
 * 	Créé le 1er Avril 2015
 * 	Fichier contenant des fonctions utiles communes au programme
 */

using System;
using System.IO;
using System.Threading;

namespace ReseauxOrdinateur
{
	//Classe statique Utility contenant les différentes fonctions
	static public class Utility
	{
		static private Semaphore file_sem = new Semaphore(1, 1);		//Sémaphore bloquant l'écriture dans un fichier
		static private Semaphore console_sem = new Semaphore(1, 1);		//Sémaphore bloquant l'écriture en console

		//Fonction permettant de convertir une chaîne de caractères en tableau de Bytes
		static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		//Fonction permettant de convertir un tableau de bytes en chaîne de caractère
		static string GetString(byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}

		//Fonction permettant d'écrire une chaîne en console avec une couleur passée en paramètre
		public static void AfficherDansConsole(string str, ConsoleColor color){
			console_sem.WaitOne ();		//Blocage
			Console.ForegroundColor = color;
			Console.WriteLine (str);
			Console.ResetColor ();
			console_sem.Release ();		//Déblocage
		}

		//Fonction permettant d'écrire une chaîne de caractère dans une fichier
		//Paramètres : Chemin d'écriture du fichier, chaîne à écrire et booléen de concaténation du fichier
		public static void EcrireDansFichier(string path, string str, bool append){
            file_sem.WaitOne();
			StreamWriter sw = new StreamWriter (path, append);
			sw.WriteLine (str);
			sw.Close ();
            file_sem.Release();
		}

		//Fonction permettant de supprimer un fichier passé en paramètre
		public static void SupprimerFichier(string path){
			if (File.Exists (path)) {
				File.Delete (path);
			}
		}
	}
}

