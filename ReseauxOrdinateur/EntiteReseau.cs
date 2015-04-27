/*	EntiteReseau.cs
 * 	Par Raphaël Blanchet, Catherine Béliveau, Joel Gbalou, Sébastien Piché Aubin et Manfouss Lawani
 * 	Créé le 1er Avril 2015
 * 	Classe permettant d'implémenter la couche Réseau du protocole OSI
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Pipes;
using System.IO;

namespace ReseauxOrdinateur
{
    class EntiteReseau
    {
        Liaison liaison;							//Couche Liaison
		AnonymousPipeClientStream reseauIn;			//Pipe de lecture venant de la couche transport
		AnonymousPipeServerStream reseauOut;		//Pire d'écriture allant vers la couche transport
        public bool isRunning = true;				//Booléen déterminant si le Thread est en train de tourner
		ListeConnexionsReseau connexions;			//Liste contenant les différentes connexions de la couche Réseau
		Semaphore pipe_sem = new Semaphore(1,1);	//Sémaphore de blocage d'écriture dans le pipe vers transport

		//Constructeur de la couche Réseau
		public EntiteReseau(AnonymousPipeClientStream _reseauIn, AnonymousPipeServerStream _reseauOut)
        {
            liaison = new Liaison();
			reseauIn = _reseauIn;
			reseauOut = _reseauOut;
			connexions = new ListeConnexionsReseau ();
        }

		//Boucle principale du processus
        public void ThreadRun()
        {
            while (isRunning)
            {
				//Lecture du pipe venant de transport
				lire_de_transport ();
            }
        }

		//Fonction permettant de faire la lecture du pipe venant de transport
		public void lire_de_transport()
        {
			string paquet = "";
			try{
				char c;
				do{
					c = (char)reseauIn.ReadByte();

					//Si on a pas atteint le caractère de fin de primitive
					if(c != Constantes.FIN_PRIMITIVE){
						paquet += c;	//On ajoute le caractère lu dans la chaîne
					}else{
						traiterPrimitiveDeTransport(paquet);	//Sinon on envoi la primitive lue dans une fonction qui la traitera
					}
				}while(c != Constantes.FIN_PRIMITIVE);

			}catch(IOException e){
				//Affichage du message d'erreur, s'il y a lieu
				Utility.AfficherDansConsole (e.Message, Constantes.ERREUR_COLOR);
			}
        }

		//Focntion permettant l'écriture d'une chaîne de caractère vers la couche transport
		public void ecrire_vers_transport(string str)
        {
			//Affichage de la chaîne à écrire
			Utility.AfficherDansConsole("Reseau vers Transport : " + str, Constantes.TRANSPORT_RESEAU_COLOR);
            str += Constantes.FIN_PRIMITIVE;

            try
            {
				pipe_sem.WaitOne();
                byte[] bytes = Encoding.UTF8.GetBytes(str);		//Transformartion de la chaîne en tableau de Bytes
                reseauOut.Write(bytes, 0, str.Length);			//Écriture du tableau dans le pipe vers transport
            }
            catch (IOException e)
            {	
				//Affichage du message d'erreur, s'il y a lieu
				Utility.AfficherDansConsole (e.Message, Constantes.ERREUR_COLOR);
			}finally{
				pipe_sem.Release ();
			}
        }

		//Fonction permettant de savoir si on a toujours des connexions d'ouvertes
		public bool ContientConnexions(){
			return (connexions.nbConnexions != 0);
		}

		//Fonction privée permettant de traiter une primitive venant de la couche transport
		private void traiterPrimitiveDeTransport(string strPrimitive){
			//Affichage de la primitive reçue
			Utility.AfficherDansConsole("Reseau reçoit de transport : " + strPrimitive, Constantes.TRANSPORT_RESEAU_COLOR);

			//On sépare la primitive pour retrouver ses paramètres
			string[] split = strPrimitive.Split (';');
			string primitive = split [1];

			//Traitement de la primitive
			if (primitive == N_CONNECT.req.ToString ()) {					//APPEL DE CONNEXION-----------------
				int addrSource = Convert.ToInt32 (split [2]);

				if (addrSource % 27 != 0) {						//Le fournisseur internet accepte la connexion
					//Parametres : Adresse Source, Adresse Destination, NIEC
					ConnexionReseau conn = connexions.EtablirConnexion (Convert.ToInt32 (split [2]), Convert.ToInt32 (split [3]), Convert.ToInt32 (split [0]));

					//Parametres : NIEC, Adresse Source, Adresse Destination
					PaquetAppel paquet = new PaquetAppel (conn.getNumeroConnexion(), conn.getAdresseSource(), conn.getAdresseDestination());
					Paquet reponse = liaison.TraiterPaquetDeReseau (paquet);
					TraiterPaquetDeLiaison (reponse, paquet, true);
				} else {										//Le fournisseur internet refuse la connexion
					ConnexionReseau conn = connexions.findConnexionWithNIEC (Convert.ToInt32 (split [0]));
					//Parametres : NIEC, Primitive, Adresse Source, Adresse Destination
					ecrire_vers_transport (split [0] + ";" + N_DISCONNECT.ind + ";" + split [3] + ";" + Constantes.RAISON_REFUS_FOURNISSEUR);
				}

			} else if (primitive == N_DATA.req.ToString ()) {               //ENVOI DE DONNÉES------------------
				int niec = Convert.ToInt32 (split [0]);
                String donnees = split[2];
                ConnexionReseau conn;

				//Segmentation des données à envoyée
				//On vérifie d'abord s'il reste des données à envoyer, et si la connexion existe toujours (car la connexion peut être perdue
				//en cours de traitement
                while (donnees.Length > 0 && (conn = connexions.findConnexionWithNIEC(niec)) != null)
                {
                    int count = Math.Min(donnees.Length, 128);
                    String data = donnees.Substring(0, count);	//On récupère au maximum 128 caractères de données
                    donnees = donnees.Remove(0, count);			//Et on l'enlève de la chaîne d'origine
                    int m = (data.Length == 128 ? 1 : 0);		//Valeur de M : 0 - Il n'y a plus d'autre paquet à transmettre, 1 - Il y en a d'autre
					PaquetDonnees paquet = new PaquetDonnees(conn.getNumeroConnexion(), conn.getPR(), conn.getPS(), m, data);	//Paquet de données
                    Paquet reponse = liaison.TraiterPaquetDeReseau(paquet);
					connexions.ModifierPS(conn.getNumeroConnexion(), 1);	//Augmentation de la valeur de P(S)
					TraiterPaquetDeLiaison(reponse, paquet, true);		//Appel de la fonction pour traiter le paquet reçu de Liaison
                }
			} else if (primitive == N_DISCONNECT.req.ToString ()) {         //DÉCONNEXION
				int niec = Convert.ToInt32 (split [0]);
				//On retrouve la connexion et on la retire
				ConnexionReseau conn = connexions.findConnexionWithNIEC (niec);
				connexions.RetirerConnexion (conn);

				//Construction du paquet pour libérer la connexion du coté liaison
				PaquetDemandeLiberation paquet = new PaquetDemandeLiberation(conn.getNumeroConnexion(), conn.getAdresseSource(), conn.getAdresseDestination());
				liaison.TraiterPaquetDeReseau (paquet);
			}
		}

		//Fonction permettant de traiter un paquet reçu de la couche Liaison
		private void TraiterPaquetDeLiaison(Paquet reponse, Paquet origin, bool renvoi)
		{
			if (reponse == null) {		//Aucune réponse... Tentative de renvoi
				if (renvoi) {
					Utility.AfficherDansConsole ("*Réseau : Aucune réponse de la couche liaison, tentative de renvoi...*", Constantes.ERREUR_COLOR);
					reponse = liaison.TraiterPaquetDeReseau (origin);
					TraiterPaquetDeLiaison (reponse, origin, false);
				} else {
					deconnecterVoieLogique(origin.numero_connexion, "Aucune reponse du distant");
				}

			} else {					//Traitement de la réponse
				//Affichage du paquet reçu
				Utility.AfficherDansConsole("Réseau recoit de Liaison : " + reponse.ToString (), Constantes.RESEAU_LIAISON_COLOR);

				ConnexionReseau conn;
				if (reponse is PaquetConnexionEtablie) {                        //Paquet de connexion établie----------------
					PaquetConnexionEtablie p = (PaquetConnexionEtablie)reponse;
					conn = connexions.findConnexionWithNum (p.numero_connexion);
					ecrire_vers_transport (conn.getNIEC() + ";" + N_CONNECT.conf + ";" + conn.getAdresseDestination());

				} else if (reponse is PaquetIndicationLiberation) {             //Paquet d'indication de libération----------
					PaquetIndicationLiberation p = (PaquetIndicationLiberation)reponse;
					conn = connexions.findConnexionWithNum(p.numero_connexion);
					ecrire_vers_transport(conn.getNIEC() + ";" + N_DISCONNECT.ind + ";" + conn.getAdresseDestination() + ";" + p.raison);

				} else if (reponse is PaquetAcquittement) {                     //Paquet d'acquittement----------------------
					PaquetAcquittement p = (PaquetAcquittement)reponse;
					string type = p.typePaquet.Substring (1);

					if (type == Constantes.TYPE_PAQUET_ACQUITTEMENT_POSITIF)    //Acquittement positif-----------------------
					{
						conn = connexions.findConnexionWithNum(reponse.numero_connexion);
						ecrire_vers_transport(conn.getNIEC() + ";" + N_DATA.ind + ";" + conn.getAdresseSource() + ";" + conn.getAdresseDestination());
						connexions.ModifierPR(reponse.numero_connexion, 1);	//Modification du P(R) de la connexion
					}
					else                                                        //Acquittement négatif - On tente de renvoyer le paquet
					{
						if (renvoi) {
							//Tentative de renvoi du paquet
							Utility.AfficherDansConsole ("*Réseau : Acquittement négatif de Liaison - Tentative de renvoi*", Constantes.ERREUR_COLOR);
							Paquet p_rep = liaison.TraiterPaquetDeReseau (origin);
							TraiterPaquetDeLiaison (p_rep, origin, false);
						} else {
							//Déconnexion de la voie logique
							deconnecterVoieLogique (origin.numero_connexion, "Acquittement negatif");
						}
					}
				}
			}
        }

		//Fonction permettant de déconnecter la voie logique
		private void deconnecterVoieLogique(int no_conn, String raison){
			//Récupération de la connexion et retrait
			ConnexionReseau conn = connexions.findConnexionWithNum (no_conn);
			connexions.RetirerConnexion (conn);
			int addrDestination = conn.getAdresseDestination();

			//Affichage à la console du message de déconnexion
			Utility.AfficherDansConsole("*Réseau : " + raison + " - Déconnexion de " + conn.getNIEC() + "*", Constantes.ERREUR_COLOR);

			//Envoie de la primitive de déconnexion
			ecrire_vers_transport (conn.getNIEC() + ";" + N_DISCONNECT.ind + ";" + addrDestination + ";" + raison);
		}
    }
}
