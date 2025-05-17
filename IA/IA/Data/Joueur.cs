namespace IA.Data
{
    public class Joueur : Perso
    {
        private Dictionary<TypeDeCarte, List<Carte>> valeurCarte;
        private int position;

        public Joueur()
        {
            valeurCarte = new Dictionary<TypeDeCarte, List<Carte>>();
        }

        public int Position
        {
            get => position;
            set => position = value;
        }

        public int ValeurCartes(TypeDeCarte type)
        {
            int res = 0;
            double multiplicateur = 0.0;
            switch (valeurCarte[type].Count)
            {
                case 0:
                case 1:
                case 2:
                    multiplicateur = 1.0;
                    break;
                case 3:
                case 4:
                    multiplicateur = 1.0;
                    break;
                case 5:
                case 6:
                    multiplicateur = 1.5;
                    break;
                case 7:
                case 8:
                    multiplicateur = 1.5;
                    break;
                default:
                    multiplicateur = 2.0;
                    break;
            }
            foreach (Carte carte in valeurCarte[type])
            {
                res += carte.Valeur;
            }
            return (int)(res * multiplicateur);
        }

        public int TotalAttaque()
        {
            return this.Attaque + ValeurCartes(TypeDeCarte.ATTAQUE);
        }

        public int TotalDefense()
        {
            return this.Def + ValeurCartes(TypeDeCarte.DEFENSE);
        }

        public int TotalSavoir()
        {
            return this.Savoir + ValeurCartes(TypeDeCarte.SAVOIR);
        }
    }
}
