using ExifTweaker.Models;

namespace ExifTweaker.Services;

/// <summary>Nature du filtrage proposé pour une colonne de la grille.</summary>
public enum ColumnFilterKind { None, Values, Date, City }

/// <summary>Granularité retenue pour le filtre d'une colonne de type date.</summary>
public enum DateFilterGranularity { Year, Month, Day }

/// <summary>
/// Filtres actifs sur les colonnes de la grille. Les valeurs retenues sont
/// exprimées sous forme de clés textuelles afin de rester indépendantes du
/// formatage d'affichage.
/// </summary>
public sealed class ColumnFilterService
{
    private readonly Dictionary<string, HashSet<string>> _filters = new(StringComparer.Ordinal);
    private readonly Dictionary<string, DateFilterGranularity> _dateGranularities = new(StringComparer.Ordinal);

    public const string EmptyKey = "\u0000empty";
    public const string EmptyLabel = "(Vides)";

    public int ActiveColumnCount => _filters.Count;
    public bool HasFilters => _filters.Count > 0;

    public IReadOnlyCollection<string> FilteredColumns => _filters.Keys;

    public bool IsFiltered(string column) => _filters.ContainsKey(column);

    public DateFilterGranularity GetGranularity(string column) =>
        _dateGranularities.TryGetValue(column, out var granularity) ? granularity : DateFilterGranularity.Day;

    public void SetGranularity(string column, DateFilterGranularity granularity)
    {
        _dateGranularities[column] = granularity;
        _filters.Remove(column);
    }

    public bool IsSelected(string column, string key) =>
        !_filters.TryGetValue(column, out var selected) || selected.Contains(key);

    public void SetSelection(string column, IEnumerable<string> keys, int availableCount)
    {
        var selected = keys.ToHashSet(StringComparer.Ordinal);
        if (selected.Count == 0 || selected.Count >= availableCount) _filters.Remove(column);
        else _filters[column] = selected;
    }

    public void Toggle(string column, string key, IReadOnlyCollection<string> availableKeys)
    {
        var selected = _filters.TryGetValue(column, out var existing)
            ? new HashSet<string>(existing, StringComparer.Ordinal)
            : availableKeys.ToHashSet(StringComparer.Ordinal);
        if (!selected.Remove(key)) selected.Add(key);
        SetSelection(column, selected, availableKeys.Count);
    }

    public void Clear(string column) => _filters.Remove(column);

    public void ClearAll() => _filters.Clear();

    /// <summary>Indique si l'élément satisfait tous les filtres, en ignorant éventuellement une colonne (cascade).</summary>
    public bool Matches(PhotoItem item, string? ignoredColumn = null)
    {
        foreach (var (column, selected) in _filters)
        {
            if (ignoredColumn is not null && column.Equals(ignoredColumn, StringComparison.Ordinal)) continue;
            if (!selected.Contains(KeyFor(item, column, GetGranularity(column)))) return false;
        }
        return true;
    }

    /// <summary>Clé de regroupement d'un élément pour une colonne donnée.</summary>
    public static string KeyFor(PhotoItem item, string column, DateFilterGranularity granularity) => column switch
    {
        nameof(PhotoItem.Date) => item.EffectiveCaptureDate is DateTime date
            ? granularity switch
            {
                DateFilterGranularity.Year => date.ToString("yyyy"),
                DateFilterGranularity.Month => date.ToString("yyyy-MM"),
                _ => date.ToString("yyyy-MM-dd")
            }
            : EmptyKey,
        nameof(PhotoItem.Location) => CityOf(item),
        _ => Normalize(TextOf(item, column))
    };

    /// <summary>Libellé affiché dans le menu pour une clé donnée.</summary>
    public static string LabelFor(string key, string column, DateFilterGranularity granularity)
    {
        if (key == EmptyKey) return EmptyLabel;
        if (column != nameof(PhotoItem.Date)) return key;
        return granularity switch
        {
            DateFilterGranularity.Year => key,
            DateFilterGranularity.Month => DateTime.TryParseExact(key, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out var month)
                ? month.ToString("MMMM yyyy")
                : key,
            _ => key
        };
    }

    /// <summary>Nature du filtre proposé pour la colonne.</summary>
    public static ColumnFilterKind KindOf(string column) => column switch
    {
        nameof(PhotoItem.Latitude) or nameof(PhotoItem.Longitude) or nameof(PhotoItem.Altitude) => ColumnFilterKind.None,
        nameof(PhotoItem.Date) => ColumnFilterKind.Date,
        nameof(PhotoItem.Location) => ColumnFilterKind.City,
        nameof(PhotoItem.FileName) or nameof(PhotoItem.Name) or nameof(PhotoItem.Timezone) or
        nameof(PhotoItem.Device) or nameof(PhotoItem.Dimensions) or nameof(PhotoItem.Status) or
        nameof(PhotoItem.Details) => ColumnFilterKind.Values,
        _ => ColumnFilterKind.None
    };

    /// <summary>Extrait la ville d'une adresse résolue par le géocodage inverse.</summary>
    public static string CityOf(PhotoItem item)
    {
        var location = item.Location;
        if (string.IsNullOrWhiteSpace(location) || location == "Identification…" || location == "Adresse indisponible")
            return EmptyKey;
        var parts = location.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0) return EmptyKey;
        // Nominatim retourne « numéro, rue, quartier, ville, …, département, code postal, pays ».
        // La ville est le dernier segment non numérique avant le département/pays.
        var candidates = parts
            .Where(part => !part.All(character => char.IsDigit(character) || char.IsWhiteSpace(character)))
            .ToList();
        if (candidates.Count == 0) return EmptyKey;
        var index = Math.Max(0, candidates.Count - 4);
        return candidates[Math.Min(index, candidates.Count - 1)];
    }

    private static string TextOf(PhotoItem item, string column) => column switch
    {
        nameof(PhotoItem.FileName) => item.FileName,
        nameof(PhotoItem.Name) => item.Name,
        nameof(PhotoItem.Timezone) => item.Timezone,
        nameof(PhotoItem.Device) => item.Device,
        nameof(PhotoItem.Dimensions) => item.Dimensions,
        nameof(PhotoItem.Status) => item.Status,
        nameof(PhotoItem.Details) => item.Details,
        nameof(PhotoItem.Location) => item.Location,
        _ => string.Empty
    };

    private static string Normalize(string value) => string.IsNullOrWhiteSpace(value) ? EmptyKey : value.Trim();
}
