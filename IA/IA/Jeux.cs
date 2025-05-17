using IA.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace IA
{
    public class Jeux
    {
        #region Attributs
        private List<Carte> pioche;
        private List<Perso> listPersos;
        private List<Monstre> listMonstres;
        private int attaqueLune;
        private Tour tourActuel;
        private Server server;
        private int numJoueur;
        private Joueur joueur;
        private bool choisiDef;
        #endregion

        #region Constructeur
        public Jeux(Server server)
        {
            pioche = new List<Carte>();
            listPersos = new List<Perso>();
            listMonstres = new List<Monstre>();
            this.server = server;
            this.choisiDef = false;
            this.joueur = new Joueur();
        }
        #endregion

        public void initTour()
        {
            //MAJ info
        }

        public void Jouer()
        {
            this.numJoueur = this.server.ConnexionPartie();
            this.JouerPhase();

        }

        public void Init()
        {
            //A faire tour 1-1
        }

        public void JouerPhase()
        {
            this.tourActuel = this.server.AttenteDebutTour();
            if (this.tourActuel.NumeroTour != -1)
            {


                this.JouerPhase();
            }
        }


        public int ChoixPioche()
        {
            int choix = -1;
            bool definitif = false;
            Carte? rep = null;

            // Carte à 5
            if (this.pioche.Any(p => p.Valeur == 5))
            {
                var carteCinq = this.pioche.Where(p => p.Valeur == 5);

                // Plusieurs à 5
                if (carteCinq.Count() > 1)
                {
                    // Type Savoir
                    rep = carteCinq.FirstOrDefault(p => p.Type == TypeDeCarte.SAVOIR);
                    if (carteCinq == null)
                    {
                        // Type Défense
                        rep = carteCinq.FirstOrDefault(p => p.Type == TypeDeCarte.DEFENSE);

                        // Type Attaque
                        if (rep == null)
                        {
                            rep = carteCinq.First();
                        }
                        else
                        {
                            choisiDef = true;
                        }
                    }
                }
                // Une seule à 5
                else
                {
                    rep = carteCinq.First();
                }
                definitif = true;
            }
            else
            {
                var carteOk = this.pioche.Where(p => p.Valeur > 2);
                if (carteOk.Count() > 0)
                    /*
                    for (int i = 0; i < this.pioche.Count; i++)
                    {
                        //Sinon SAV 3/4 -- Sinon DEF 3 / 4
                        if (this.pioche[i].Valeur > 2 && this.pioche[i].Type != TypeDeCarte.ATTAQUE && !definitif)
                        {
                                choix = i;
                        }
                    }
                    */
                    if (choix == -1) choix = 1;

            }
            return choix;
        }


        public void NouveauTour()
        {
            this.attaqueLune = this.server.DegatsDR();
        }

        public void NouveauJour()
        {
            this.joueur = this.server.GetJoueur();
            this.listMonstres = this.server.GetMonstres().ToList();
            this.choisiDef = false;
        }

        public void NouvellePhase()
        {
            this.pioche = this.server.GetPioche().ToList();
        }

        public void NouvelleNuit()
        {
            string action = "";
            this.joueur = this.server.GetJoueur();
            this.listMonstres = this.server.GetMonstres().ToList();
            int monstreAttaque = -1;

            //Prend le malus si il est elevé et totalement utilisé par les stats d'un adversaire
            int carteMalus = -1;
            int idAdversaire = -1;
            for (int i = 0; i < this.pioche.Count; i++)
            {
                for (int j = 0; this.listPersos.Count > 0; j++)
                {
                    if (j != this.numJoueur)
                    {
                        switch (this.pioche[i].Type)
                        {
                            case TypeDeCarte.ATTAQUE:
                                {
                                    if ((this.pioche[i].Valeur < -23) && (this.listPersos[j].Attaque > 23))
                                    {
                                        action = "malus";
                                        carteMalus = i;
                                        idAdversaire = j;
                                    }
                                }
                                break;
                            case TypeDeCarte.DEFENSE:
                                {
                                    if ((this.pioche[i].Valeur < -23) && (this.listPersos[j].Def > 23))
                                    {
                                        action = "malus";
                                        carteMalus = i;
                                        idAdversaire = j;
                                    }
                                }
                                break;
                            case TypeDeCarte.SAVOIR:
                                {
                                    if ((this.pioche[i].Valeur < -23) && (this.listPersos[j].Savoir > 23))
                                    {
                                        action = "malus";
                                        carteMalus = i;
                                        idAdversaire = j;
                                    }
                                }
                                break;
                        }

                    }

                }
            }

            //Attaque si on peut oneshot le mob ou si il est low
            for (int i = 0; i < this.listMonstres.Count; i++)
            {
                if ((this.joueur.TotalAttaque() > listMonstres[i].Vie) || (listMonstres[i].Vie < (listMonstres[i].Vie * (30 / 100))))
                {
                    action = "attaquer";
                    monstreAttaque = i;
                }
            }

            //Utilisation de la défense pour tanker l'attaque de la dame en rouge
            if (this.tourActuel.Phase == 15)
            {
                action = "dernière nuit";
            }

            //Prend le savoir si on a assez pour gagner
            if (this.joueur.TotalSavoir() > 2000)
            {
                action = "prendre savoir";
            }

            //Si aucune action n'a été réalisée
            if (action == "") action = "pioche";

            //Envoi de l'action au serveur
            switch (action)
            {
                case "malus": this.server.Piocher(carteMalus, idAdversaire); break;
                case "attaquer": this.server.Attaquer(monstreAttaque); break;
                case "prendre savoir": this.server.Utiliser(TypeDeCarte.SAVOIR); break;
                case "dernière nuit": this.server.Utiliser(TypeDeCarte.DEFENSE); break;
                case "pioche": this.server.Piocher(this.ChoixPioche(), numJoueur); break;
            }
        }
        public (int, int)  GetMalus()
        {

            var meilleurOppDef = (Id : -1, stat : -1 ); 
            var meilleurOppAtk = ( Id : -1, stat :-1 ); 
            var meilleurOppSav = ( Id : -1, stat :-1 ); 

            for (int i = 0; i < this.listPersos.Count; i++)
            {
                if (i == this.numJoueur) continue;
                var perso = listPersos[i]; 
                if (perso.Attaque > meilleurOppAtk.stat)
                {
                    meilleurOppAtk = (i, perso.Attaque); 
                }
                if (perso.Def > meilleurOppDef.stat)
                {
                    meilleurOppDef = (i, perso.Def);
                }
                if (perso.Savoir > meilleurOppSav.stat)
                {
                    meilleurOppSav = (i, perso.Attaque);
                }
            }

            double meilleurScore = double.MinValue;
            int meilleurCardIdx = -1, meilleurOppId = -1;

            for (int i = 0; i < pioche.Count; i++)
            {
                var card = pioche[i];
                int oppIdx, oppStat;
                switch (card.Type)
                {
                    case TypeDeCarte.ATTAQUE:
                        (oppIdx, oppStat) = meilleurOppAtk; break;
                    case TypeDeCarte.DEFENSE:
                        (oppIdx, oppStat) = meilleurOppDef; break;
                    case TypeDeCarte.SAVOIR:
                        (oppIdx, oppStat) = meilleurOppSav; break;
                    default:
                        continue;
                }

                if (oppIdx < 0)
                    continue; 

                double score = -card.Valeur * oppStat;
                if (score > meilleurScore)
                {
                    meilleurScore = score;
                    meilleurCardIdx = i;
                    meilleurOppId = oppIdx;
                }
            }

            return (meilleurCardIdx, meilleurOppId);
        }


        }
    }
}
