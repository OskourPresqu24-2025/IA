namespace IA.Data
{
    public abstract class Perso
    {
        private int pv;
        private int def;
        private int attaque;
        private int savoir;


        public Perso()
        {
            Pv = 500;
            Def = 0;
            Attaque = 0;
            Savoir = 0;
        }

        public int Pv
        {
            get => pv;
            set => pv = value;
        }
        public int Def
        {
            get => def;
            set => def = value;
        }
        public int Attaque
        {
            get => attaque;
            set => attaque = value;
        }
        public int Savoir
        {
            get => savoir;
            set => savoir = value;
        }
    }
}
