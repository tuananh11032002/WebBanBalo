using WebBanBalo.Data;
using WebBanBalo.Interface;
using WebBanBalo.Model;

namespace WebBanBalo.Repository
{
    public class ColorRepository : IColorRepository
    {
        private readonly DataContext _dataContext;
        public ColorRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public bool Add(Color color)
        {
            _dataContext.Add(color);
            return Save();
        }

        public bool Delete(Color color)
        {
            _dataContext.Remove(color);
            return Save();
        }

        public Color Get(int id)
        {
            return _dataContext.Color.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<Color> GetAll()
        {
            return _dataContext.Color.ToList();

        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool Update(Color color)
        {
            _dataContext.Update(color);
            return Save();
        }
    }
}
