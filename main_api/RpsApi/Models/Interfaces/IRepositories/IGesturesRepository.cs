using RpsApi.Models.Database;

namespace RpsApi.Models.Interfaces.IRepositories;

public interface IGesturesRepository
{
    bool AddGesture(Gesture gesture);
    Gesture? GetGesture(int id);
    bool DeleteGesture(Gesture gesture);
    IQueryable<Gesture> GetAllUserGestures(int userId);
}