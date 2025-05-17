using IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace IA
{
    public class Server
    {
        /// <summary>Le client TCP</summary>
        private TcpClient client;
        /// <summary>Le flux entrant depuis le serveur</summary>
        private StreamReader fluxEntrant;
        /// <summary>Le flux sortant vers le serveur</summary>
        private StreamWriter fluxSortant;

        /// <summary>Création du client TCP </summary>
        private void Connexion()
        {
            this.client = new TcpClient("127.0.0.1", 1234);
        }

        /// <summary>Création du flux entrant et du flux sortant</summary>
        private void CreationFlux()
        {
            this.fluxEntrant = new StreamReader(this.client.GetStream());
            this.fluxSortant = new StreamWriter(this.client.GetStream())
            {
                AutoFlush = true
            };
        }

        /// <summary>Etablir la connexion avec le serveur</summary>
        public void EtablirConnexion()
        {
            this.Connexion();
            this.CreationFlux();
        }

        /// <summary>Envoyer un message au serveur</summary>
        /// <param name="message">Le message à envoyer</param>
        public void EnvoyerMessage(string message)
        {
            Console.WriteLine(">> " + message);
            this.fluxSortant.WriteLine(message);
        }

        /// <summary>Recevoir un message depuis le serveur (bloque jusqu'à réception d'un message)</summary>
        public string RecevoirMessage()
        {
            string message = this.fluxEntrant.ReadLine();
            Console.WriteLine("<< " + message);
            return message;
        }

        /// <summary>Termine la connexion au serveur</summary>
        public void FermerConnexion()
        {
            this.client.Close();
        }

        public int ConnexionPartie()
        {
            this.EtablirConnexion();
            var messageServer = this.RecevoirMessage();
            if (messageServer != "NOM_EQUIPE")
            {
                throw new InvalidOperationException("Erreur nomEquipe " + messageServer);
            }
            this.EnvoyerMessage("Oskour");
            messageServer = this.RecevoirMessage();
            if (!messageServer.StartsWith("Bonjour"))
            {
                throw new InvalidOperationException("Erreur bienvenue" + messageServer);
            }

            return int.Parse(messageServer.Split("|")[1]);

        }


        public Tour AttenteDebutTour()
        {
            var messageServer = this.RecevoirMessage();
            if (!messageServer.StartsWith("DEBUT_TOUR"))
            {
                if (messageServer == "FIN")
                {
                    return new Tour
                    {
                        NumeroTour = -1,
                    };
                }
                throw new InvalidOperationException(messageServer);
            }
            var info = messageServer.Split("|");
            return new Tour
            {
                NumeroTour = int.Parse(info[1]),
                Phase = int.Parse(info[2]),
            };

        }

        public int DegatsDR()
        {
            this.EnvoyerMessage("DEGATS");
            string messageServer = this.RecevoirMessage();
            return Convert.ToInt32(messageServer);
        }

        public bool Piocher(int numeroCarte, int? idJoueur)
        {
            var message = $"PIOCHER|{numeroCarte}";
            if (idJoueur.HasValue)
            {
                message += $"|{idJoueur}";
            }
            this.EnvoyerMessage(message);
            var reponse = this.RecevoirMessage();
            if (reponse == "OK")
            {
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(reponse);
                Console.ResetColor();

                return false;
            }
        }

        public bool Utiliser(TypeDeCarte type)
        {
            var message = $"UTILISER|{(int)type}";
            this.EnvoyerMessage(message);
            var reponse = this.RecevoirMessage();

            if (reponse == "OK")
            {
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(reponse);
                Console.ResetColor();

                return false;
            }
        }

        public bool Attaquer(int idMonstre)
        {
            var message = $"ATTAQUER|{idMonstre}";
            var reponse = this.RecevoirMessage();
            if (reponse == "OK")
            {
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(reponse);
                Console.ResetColor();
                return false;
            }
        }

        public IEnumerable<Perso> GetJoueurs()
        {

            this.EnvoyerMessage("JOUEURS");
            var reponse = this.RecevoirMessage();

            var joueurs = reponse.Split("|");
            List<Perso> listeJoueurs = new List<Perso>();

            for (int i = 0; i < joueurs.Length; i += 4)
            {
                listeJoueurs.Add(new Perso
                {
                    Pv = int.Parse(joueurs[i]),
                    Def = int.Parse(joueurs[i + 1]),
                    Attaque = int.Parse(joueurs[i + 2]),
                    Savoir = int.Parse(joueurs[i + 3])
                });
            }

            return listeJoueurs;

        }

        public Joueur GetJoueur()
        {
            this.EnvoyerMessage("MOI");
            var reponse = this.RecevoirMessage();
            var joueur = reponse.Split("|");

            Joueur perso = new Joueur()
            {
                Pv = int.Parse(joueur[0]),
                Def = int.Parse(joueur[1]),
                Attaque = int.Parse(joueur[2]),
                Savoir = int.Parse(joueur[3])
            };
            return perso;

        }

        public IEnumerable<Monstre> GetMonstres()
        {
            this.EnvoyerMessage("MONSTRES");
            var reponse = this.RecevoirMessage();
            var monstre = reponse.Split("|");
            List<Monstre> listeMonstres = new List<Monstre>();
            for (int i = 0; i < monstre.Length; i += 2)
            {
                listeMonstres.Add(new Monstre
                {
                    Vie = int.Parse(monstre[i]),
                    PointSavoir = int.Parse(monstre[i + 1])
                });
            }
            return listeMonstres;
        }

        public IEnumerable<Carte> GetPioche()
        {
            this.EnvoyerMessage("PIOCHES");
            var reponse = this.RecevoirMessage();
            var cartes = reponse.Split("|");
            List<Carte> listePioche = new List<Carte>();
            for (int i = 0; i < cartes.Length; i += 2)
            {
                listePioche.Add(new Carte
                {
                    Type = GetTypeDeCarteFromString(cartes[i]),
                    Valeur = int.Parse(cartes[i + 1]),
                });
            }
            return listePioche;
        }

        private TypeDeCarte GetTypeDeCarteFromString(string type)
        {
            switch (type)
            {
                case "ATTAQUE":
                    return TypeDeCarte.ATTAQUE;
                case "DEFENSE":
                    return TypeDeCarte.DEFENSE;
                case "SAVOIR":
                    return TypeDeCarte.SAVOIR;
                default:
                    throw new ArgumentException("Type de carte inconnu : " + type);
            }
        }
    }
}