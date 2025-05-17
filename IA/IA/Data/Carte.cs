namespace IA.Data
{
    public class Carte
    {
        private int valeur;
        private TypeDeCarte type;

        public int Valeur
        {
            get => valeur;
            set => valeur = value;
        }
        public TypeDeCarte Type
        {
            get => type;
            set => type = value;
        }
    }
}
