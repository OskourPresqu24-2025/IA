﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            Console.WriteLine(">>" + message);
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
                throw new InvalidOperationException("Erreur nomEquipe "+messageServer);
            }
            this.EnvoyerMessage("Oskour");
            messageServer = this.RecevoirMessage();
            if ( !messageServer.StartsWith("Bonjour"))
            {
                throw new InvalidOperationException("Erreur bienvenue"+ messageServer);
            }

            return int.Parse(messageServer.Split("|")[1]); 

        }

        public void AttenteDebutTour()
        {
                
        }

        public int DegatsDR()
        {
            this.EnvoyerMessage("DEGATS");
            string messageServer = this.RecevoirMessage();
            return Convert.ToInt32(messageServer);
        }
    }
}