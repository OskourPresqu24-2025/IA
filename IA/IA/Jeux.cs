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
        #endregion

        #region Constructeur
        public Jeux(Server server)
        {
            pioche = new List<Carte>();
            listPersos = new List<Perso>();
            listMonstres = new List<Monstre>();
            this.server = server;
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
            for (int i = 0; i < this.pioche.Count; i++)
            {
                if (this.pioche[i].Valeur == 5)
                {
                    choix = i;
                    definitif = true;
                }
                else if(this.pioche[i].Valeur >2 && this.pioche[i].Type != TypeDeCarte.ATTAQUE && !definitif)
                {
                    choix=i;
                }
            }
            if (choix == -1) choix = 1;
            return choix;
        }

        public void NouveauTour()
        {
            // A appeler a chaque tour
        }

        public void NouveauJour()
        {
            // A appeler a chaque jour
        }

        public void NouvellePhase()
        {
            this.pioche = this.server.GetPioche().ToList();
        }

    }
}
