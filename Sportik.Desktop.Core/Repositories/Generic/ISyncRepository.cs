using System.Collections.Generic;

namespace Sportik.Desktop.Core.Repositories.Generic
{
    public interface ISyncRepository<T>
    {
        T GetById(int id);

        IEnumerable<T> GetAll();

        T Add(T entity);

        T Update(T entity);

        T DeleteById(int id);

        T Delete(T entity);
    }
}
