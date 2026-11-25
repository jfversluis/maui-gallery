namespace MauiGallery.Services;

public interface IRecentsService
{
    IReadOnlyList<string> Ids { get; }

    void Add(string itemId);
}
