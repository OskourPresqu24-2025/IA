// See https://aka.ms/new-console-template for more information
using IA;
using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Hello, World!");


Server server = new Server();
Jeux ia = new Jeux(server);
Console.WriteLine (server.ConnexionPartie()) ;