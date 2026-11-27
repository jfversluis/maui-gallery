using System.Text.Json.Serialization;

namespace MauiGallery.Features.Media;

/// <summary>What <see cref="HybridBridge.GetDeviceInfo"/> returns to JavaScript.</summary>
public sealed record DeviceSummary(string Platform, string Version, string Model, string Idiom);

/// <summary>The JSON payload of the raw messages the HybridWebView sample exchanges.</summary>
public sealed record HybridChatMessage(string Author, string Text);

/// <summary>
/// Source-generated JSON metadata for every type that crosses the JavaScript boundary, so the HybridWebView samples
/// stay trim-safe and need no reflection.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(DeviceSummary))]
[JsonSerializable(typeof(HybridChatMessage))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
internal sealed partial class HybridJsonContext : JsonSerializerContext;
