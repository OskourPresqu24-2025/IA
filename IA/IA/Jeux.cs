using IA.Data;

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

        #region methodes
        public void Jouer()
        {
            this.numJoueur = this.server.ConnexionPartie();
            this.JouerPhase();

        }

        public void JouerPhase()
        {
            this.tourActuel = this.server.AttenteDebutTour();
            if (this.tourActuel.NumeroTour != -1)
            {
                if (this.tourActuel.Phase == 0)
                {
                    this.NouveauTour();
                }

                if ((this.tourActuel.Phase % 4) == 0)
                {
                    this.NouveauJour();
                }

                if (this.tourActuel.Etat == TypeJour.NUIT)
                {
                    this.NouvelleNuit();
                }

                this.NouvellePhase();


                //Utilisation de la défense pour tanker l'attaque de la dame en rouge
                if (this.tourActuel.Phase == 15)
                {
                    this.Utiliser(TypeDeCarte.DEFENSE);
                }
                
                if(this.tourActuel.Etat == TypeJour.NUIT)
                {
                    this.JouerNuit();
                }
                else
                {
                    this.Piocher();
                }
                

                this.JouerPhase();
            }
        }


        public (int,int?) ChoixPioche()
        {
            int choix = -1;
            Carte? rep = null;
            int? cible = null;

            if(this.tourActuel.Phase%4==2 && this.choisiDef == false)
            {
                rep  = this.pioche.Where(p => p.Type==TypeDeCarte.DEFENSE)
                    .MaxBy(p => p.Valeur);
                this.choisiDef = true;
            }
            // Carte à 5
            else if (this.pioche.Any(p => p.Valeur == 5))
            {
                var carteCinq = this.pioche.Where(p => p.Valeur == 5);

                // Plusieurs à 5
                if (carteCinq.Count() > 1)
                {
                    if (this.tourActuel.NumeroTour < 7)
                    {
                        rep = carteCinq.FirstOrDefault(p => p.Type == TypeDeCarte.ATTAQUE);
                    }
                    else
                    {
                        // Type Defense
                        rep = carteCinq.FirstOrDefault(p => p.Type == TypeDeCarte.DEFENSE);
                    }
                    
                    if (carteCinq == null)
                    {
                        if (this.tourActuel.NumeroTour < 7)
                        {
                            rep = carteCinq.FirstOrDefault(p => p.Type == TypeDeCarte.DEFENSE);
                        }
                        else
                        {
                            // Type Defense
                            rep = carteCinq.FirstOrDefault(p => p.Type == TypeDeCarte.ATTAQUE);
                        }

                        // Type SAVOIR
                        if (rep == null)
                        {
                            rep = carteCinq.First();
                        }
                    }
                    else
                    {
                        choisiDef = true;
                    }
                }
                // Une seule à 5
                else
                {
                    rep = carteCinq.First();
                }
            }
            else
            {
                var carteOk = this.pioche.Where(p => p.Valeur > 3);
                if (carteOk.Count() > 0)
                {
                    if (carteOk.Count() > 1)
                    {
                        var repList = carteOk;
                        if (this.tourActuel.NumeroTour < 5)
                        {
                            // Type Defense
                            repList = carteOk.Where(p => p.Type == TypeDeCarte.ATTAQUE);
                        }
                        else
                        {
                            repList = carteOk.Where(p=> p.Type == TypeDeCarte.DEFENSE);
                        }
                        
                        if (repList.Count() == 0)
                        {
                            if (this.tourActuel.NumeroTour < 5)
                            {
                                // Type Defense
                                repList = carteOk.Where(p => p.Type == TypeDeCarte.DEFENSE);
                            }
                            else
                            {
                                repList = carteOk.Where(p => p.Type == TypeDeCarte.ATTAQUE);
                            }

                            // Type Savoir
                            if (repList.Count() == 0)
                            {
                                rep = carteOk.MaxBy(p => p.Valeur);
                            }
                            else
                            {
                                rep = repList.MaxBy(p => p.Valeur);
                                
                            }
                        }
                        else
                        {
                            rep = repList.MaxBy(p => p.Valeur);
                            choisiDef = true;
                        }
                    }
                    else
                    {
                        rep = carteOk.First();
                    }
                }
                // Si attaque ou pas de valeur haute
                var carteMeh = this.pioche.Where(p => p.Valeur >2);
                if (carteMeh.Count() > 0 && rep == null)
                {
                    rep = carteMeh.Where(p=> p.Type != TypeDeCarte.SAVOIR).MaxBy(p => p.Valeur);
                }
                else
                {
                    var malus = this.GetMalus();
                    choix = malus.Item1;
                    cible = malus.Item2;
                }


            }
            // Assignation num
            if (rep != null)
            {
                choix = this.pioche.FindIndex(p => p == rep);
            }
            if (choix == -1) choix = 1;
            rep = this.pioche[choix];

            return (choix,cible);
        }


        public void NouveauTour()
        {
            this.attaqueLune = this.server.DegatsDR();
        }

        public void NouveauJour()
        {
            
            this.joueur = this.joueur.Copy(this.server.GetJoueur());
            this.listMonstres = this.server.GetMonstres().ToList();
            this.choisiDef = false;
        }

        public void NouvellePhase()
        {
            this.pioche = this.server.GetPioche().ToList();
        }

        public void NouvelleNuit()
        {
            this.joueur = this.joueur.Copy(this.server.GetJoueur());
            this.listMonstres = this.server.GetMonstres().ToList();
        }

        public void JouerNuit()
        {
            string action = "";
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
                if (((this.joueur.TotalAttaque() > listMonstres[i].Vie) && (listMonstres[i].Vie != 0)) || (listMonstres[i].Vie < (listMonstres[i].Vie * (30 / 100))))
                {
                    action = "attaquer";
                    monstreAttaque = i;
                }
            }

            //Prend le savoir si on a assez pour gagner
            if ((this.joueur.TotalSavoir() > 2000 || (this.joueur.Pv < this.attaqueLune) && this.tourActuel.Phase == 15))
            {
                action = "prendre savoir";
            }

            //Si aucune action n'a été réalisée
            if (action == "") action = "pioche";

            //Envoi de l'action au serveur
            switch (action)
            {
                case "malus": this.server.Piocher(carteMalus, idAdversaire); break;
                case "attaquer": {
                        this.Utiliser(TypeDeCarte.ATTAQUE);
                        this.server.Attaquer(monstreAttaque); 
                    }
                    break;
                case "prendre savoir":
                    {
                        this.Utiliser(TypeDeCarte.SAVOIR);
                        this.Piocher();
                    }
                    break;
                case "pioche":
                    {
                        this.Piocher();
                    }
                    break;
            }
        }
        public (int, int) GetMalus()
        {

            var meilleurOppDef = (Id: -1, stat: -1);
            var meilleurOppAtk = (Id: -1, stat: -1);
            var meilleurOppSav = (Id: -1, stat: -1);

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

        public void Utiliser(TypeDeCarte type)
        {
            this.server.Utiliser(type);
            this.joueur.DicoCarte[type].Clear();
        }

        public void Piocher()
        {
            var choix = this.ChoixPioche();
            this.server.Piocher(choix.Item1, choix.Item2);
            Carte carte = this.pioche[choix.Item1];
            if (choix.Item2 == null)
            {
                this.joueur.DicoCarte[carte.Type].Add(carte);
            }
        }

        #endregion
    }
}
    

