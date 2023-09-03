using WebBanBalo.Model;

namespace WebBanBalo.Interface
{
    public interface IColorRepository
    {
        public ICollection<Color> GetAll();
        public Color Get(int id);

        public bool Delete(Color color);
        public bool Update(Color color);

        public bool Add(Color color);
        public bool Save();

    }
}
