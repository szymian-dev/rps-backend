using RpsApi.Database;
using RpsApi.Models.Database;
using RpsApi.Models.Interfaces.IRepositories;

namespace RpsApi.Repositories;

public class GesturesRepository(ApplicationDbContext dbContext) : IGesturesRepository
{
    public bool AddGesture(Gesture gesture)
    {
        gesture.CreatedAt = DateTime.UtcNow;
        gesture.UpdatedAt = DateTime.UtcNow;
        dbContext.Gestures.Add(gesture);
        return dbContext.SaveChanges() > 0;
    }

    public Gesture? GetGesture(int id)
    {
        return dbContext.Gestures.SingleOrDefault(g => g.Id == id);
    }
}