namespace MauiGallery.Features.Fundamentals;

/// <summary>The source of the "Handle missing values" sample: a teammate who may have no nickname.</summary>
public sealed record Teammate(string Name, string? Nickname);
