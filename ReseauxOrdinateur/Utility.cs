using System;
using System.IO;
using System.Threading;

namespace ReseauxOrdinateur
{
	static public class Utility
	{
		static private Semaphore file_sem = new Semaphore(1, 1);
		static private Semaphore console_sem = new Semaphore(1, 1);

		static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}

		static string GetString(byte[] bytes)
		{
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
		}

		public static void AfficherDansConsole(string str, ConsoleColor color){
			console_sem.WaitOne ();
			Console.ForegroundColor = color;
			Console.WriteLine (str);
			Console.ResetColor ();
			console_sem.Release ();
		}

		public static void EcrireDansFichier(string path, string str, bool append){
            file_sem.WaitOne();
			StreamWriter sw = new StreamWriter (path, append);
			sw.WriteLine (str);
			sw.Close ();
            file_sem.Release();
		}
		public static void SupprimerFichier(string path){
			if (File.Exists (path)) {
				File.Delete (path);
			}
		}
	}
}

