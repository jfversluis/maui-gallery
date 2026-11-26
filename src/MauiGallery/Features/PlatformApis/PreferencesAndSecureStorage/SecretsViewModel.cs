using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the SecureStorage sample.</summary>
public sealed partial class SecretsViewModel(ISecureStorage secureStorage) : ObservableObject
{
    private const string TokenKey = "gallery_sample_api_token";

    [ObservableProperty]
    public partial string Token { get; set; } = "sk-12345-not-a-real-token";

    [ObservableProperty]
    public partial string Stored { get; set; } = "Nothing read yet";

    [ObservableProperty]
    public partial string? Result { get; set; }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            await secureStorage.SetAsync(TokenKey, Token);
            Result = "SetAsync stored the token";
            await ReadAsync();
        }
        catch (Exception ex)
        {
            // The keychain or keystore can be unavailable, for example without the right entitlement.
            Result = $"SetAsync failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ReadAsync()
    {
        try
        {
            Stored = await secureStorage.GetAsync(TokenKey) ?? "Nothing stored";
        }
        catch (Exception ex)
        {
            Result = $"GetAsync failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task RemoveAsync()
    {
        Result = secureStorage.Remove(TokenKey) ? "Remove deleted the token" : "There was no token to remove";
        await ReadAsync();
    }
}
