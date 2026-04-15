using System.Collections.Generic;
using MedCenterApp.Models;

namespace MedCenterApp.Services;

public interface IRepository<T> where T : BaseEntity
{
    List<T> GetAll();
    T? GetById(int id);
    void Add(T item);
    void Update(T item);
    void Delete(int id);
}
