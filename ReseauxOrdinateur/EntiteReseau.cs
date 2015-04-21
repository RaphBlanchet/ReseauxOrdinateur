using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipes;
using System.IO;

namespace ReseauxOrdinateur
{
    class EntiteReseau
    {
        Liaison liaison;
		AnonymousPipeClientStream reseauIn;
		AnonymousPipeServerStream reseauOut;
        public bool isRunning = true;
		TableConnexionReseau connexions;

		public EntiteReseau(AnonymousPipeClientStream _reseauIn, AnonymousPipeServerStream _reseauOut)
        {
            liaison = new Liaison();
			reseauIn = _reseauIn;
			reseauOut = _reseauOut;
			connexions = new TableConnexionReseau ();
        }

        public void ThreadRun()
        {
            while (isRunning)
            {
				lire_de_transport ();
            }
        }

		public void lire_de_transport()
        {
			string paquet = "";
			try{
				char c;
				do{
					c = (char)reseauIn.ReadByte();

					if(c != Constantes.FIN_PAQUET){
						paquet += c;
					}else{
						traiterPrimitiveDeTransport(paquet);
					}
				}while(c != Constantes.FIN_PAQUET);

			}catch(Exception e){
				
			}
        }

		public void ecrire_vers_transport(string str)
        {

			Utility.AfficherDansConsole("Reseau vers Transport : " + str, Constantes.TRANSPORT_RESEAU_COLOR);
            str += Constantes.FIN_PAQUET;

            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);

                reseauOut.Write(bytes, 0, str.Length);
            }
            catch (IOException e)
            {

            }
        }

		private void traiterPrimitiveDeTransport(string strPrimitive){
			Utility.AfficherDansConsole("Reseau reçoit de transport : " + strPrimitive, Constantes.TRANSPORT_RESEAU_COLOR);
			string[] split = strPrimitive.Split (';');
			string primitive = split [1];

			if (primitive == N_CONNECT.req.ToString ()) {
				int addrSource = Convert.ToInt32 (split [2]);

				if (addrSource % 27 != 0) {				//Le fournisseur internet accepte la connexion
					//Parametres : Adresse Source, Adresse Destination, NIEC
					ConnexionReseau conn = connexions.EtablirConnexion (Convert.ToInt32 (split [2]), Convert.ToInt32 (split [3]), Convert.ToInt32 (split [0]));

					//Parametres : NIEC, Adresse Source, Adresse Destination
					PaquetAppel paquet = new PaquetAppel (conn.numeroConnexion, conn.adresseSource, conn.adresseDestinataire);
					Paquet reponse = liaison.TraiterPaquetDeReseau (paquet);
					TraiterPaquetDeLiaison (reponse, paquet);
				} else {								//Le fournisseur internet refuse la connexion
					ConnexionReseau conn = connexions.findConnexionWithNIEC (Convert.ToInt32 (split [0]));
					//Parametres : NIEC, Primitive, Adresse Source, Adresse Destination
					ecrire_vers_transport (split [0] + ";" + N_DISCONNECT.ind + ";" + split [3] + ";" + Constantes.RAISON_REFUS_FOURNISSEUR);
				}

			} else if (primitive == N_DATA.req.ToString ()) {               //ENVOI DE DONNÉES
				int niec = Convert.ToInt32 (split [0]);
                String donnees = split[2];
                ConnexionReseau conn;
                while (donnees.Length > 0 && (conn = connexions.findConnexionWithNIEC(niec)) != null)
                {
                    int count = Math.Min(donnees.Length, 128);
                    String data = donnees.Substring(0, count);
                    donnees = donnees.Remove(0, count);
                    int m = (data.Length == 128 ? 1 : 0);
                    PaquetDonnees paquet = new PaquetDonnees(niec, conn.pr, conn.ps, m, data);
                    Paquet reponse = liaison.TraiterPaquetDeReseau(paquet);
                    connexions.ModifierPS(conn.numeroConnexion, 1);
                    TraiterPaquetDeLiaison(reponse, paquet);
                }
			} else if (primitive == N_DISCONNECT.req.ToString ()) {         //DÉCONNEXION
				int niec = Convert.ToInt32 (split [0]);
				ConnexionReseau conn = connexions.findConnexionWithNIEC (niec);
				connexions.RetirerConnexion (conn);

				//Construction du paquet pour libérer la connexion du coté liaison
				PaquetDemandeLiberation paquet = new PaquetDemandeLiberation(conn.numeroConnexion, conn.adresseSource, conn.adresseDestinataire);
				liaison.TraiterPaquetDeReseau (paquet);
			}
		}

		private void TraiterPaquetDeLiaison(Paquet reponse, Paquet origin)
		{
			if (reponse == null) {		//Aucune réponse... Tentative de renvoi
				Utility.AfficherDansConsole("*Réseau : Aucune réponse de la couche liaison, tentative de renvoi...*", Constantes.ERREUR_COLOR);
				reponse = liaison.TraiterPaquetDeReseau (origin);
				if (reponse == null) {
					deconnecterVoieLogique (origin.numero_connexion, "Aucune reponse du distant");
				}
			} else {
				Utility.AfficherDansConsole("Réseau recoit de Liaison : " + reponse.ToPaquetString (), Constantes.RESEAU_LIAISON_COLOR);
				ConnexionReseau conn;
				if (reponse is PaquetConnexionEtablie) {                        //Paquet de connexion établie
					PaquetConnexionEtablie p = (PaquetConnexionEtablie)reponse;
					conn = connexions.findConnexionWithNum (p.numero_connexion);
					ecrire_vers_transport (conn.niec + ";" + N_CONNECT.conf + ";" + conn.adresseDestinataire);
				} else if (reponse is PaquetIndicationLiberation) {             //Paquet d'indication de libération
					PaquetIndicationLiberation p = (PaquetIndicationLiberation)reponse;
					conn = connexions.findConnexionWithNum(p.numero_connexion);
					ecrire_vers_transport(conn.niec + ";" + N_DISCONNECT.ind + ";" + conn.adresseDestinataire + ";" + p.raison);
				} else if (reponse is PaquetAcquittement) {                     //Paquet d'acquittement
					PaquetAcquittement p = (PaquetAcquittement)reponse;
					string type = p.typePaquet.Substring (1);
					if (type == Constantes.TYPE_PAQUET_ACQUITTEMENT_POSITIF)    //Acquittement positif
					{
						conn = connexions.findConnexionWithNum(reponse.numero_connexion);
						ecrire_vers_transport(conn.niec + ";" + N_DATA.ind + ";" + conn.adresseSource + ";" + conn.adresseDestinataire);
                        connexions.ModifierPR(reponse.numero_connexion, 1);
					}
					else                                                        //Acquittement négatif - On tente de renvoyer le paquet
					{
						//Tentative de renvoi du paquet
						Utility.AfficherDansConsole("*Réseau : Acquittement négatif de Liaison - Tentative de renvoi*", Constantes.ERREUR_COLOR);
						PaquetAcquittement p_rep = (PaquetAcquittement)liaison.TraiterPaquetDeReseau (origin);
						type = p_rep.typePaquet.Substring (1);
						if (type == Constantes.TYPE_PAQUET_ACQUITTEMENT_NEGATIF) {
							deconnecterVoieLogique (p_rep.numero_connexion, "Acquittement negatif");
						}
					}
				}
			}

        }

		private void deconnecterVoieLogique(int no_conn, String raison){
			ConnexionReseau conn = connexions.findConnexionWithNum (no_conn);
			connexions.RetirerConnexion (conn);
			int addrDestination = conn.adresseDestinataire;
			Utility.AfficherDansConsole("*Réseau : Aucune réponse du distant - Déconnexion de " + conn.niec + "*", Constantes.ERREUR_COLOR);
			ecrire_vers_transport (conn.niec + ";" + N_DISCONNECT.ind + ";" + addrDestination + ";" + raison);
		}
    }
}
