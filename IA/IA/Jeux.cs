using IA.Data;

namespace IA
{
    public class Jeux
    {
        private List<Carte> pioche;
        private List<Perso> listPersos;
        private List<Monstre> listMonstres;
        private int attaqueLune;
        private int tour;
        private int phase;
        private Server server;

        public Jeux(Server server)
        {
            pioche = new List<Carte>();
            listPersos = new List<Perso>();
            listMonstres = new List<Monstre>();
            this.server = server;
        }
    }
}
