using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed class EditHistory
{
    private readonly Stack<IReadOnlyDictionary<PhotoItem, MetadataPatch>> _undo = new();
    private readonly Stack<IReadOnlyDictionary<PhotoItem, MetadataPatch>> _redo = new();

    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public void Capture(IEnumerable<PhotoItem> items)
    {
        var snapshot = items.Distinct().ToDictionary(item => item, item => item.PendingChanges.Clone());
        if (snapshot.Count == 0) return;
        _undo.Push(snapshot);
        _redo.Clear();
    }

    public bool Undo(IEnumerable<PhotoItem> items) => Restore(_undo, _redo, items);
    public bool Redo(IEnumerable<PhotoItem> items) => Restore(_redo, _undo, items);
    public void Clear() { _undo.Clear(); _redo.Clear(); }

    public void Forget(IEnumerable<PhotoItem> items)
    {
        var forgotten = items.ToHashSet();
        Filter(_undo, forgotten);
        Filter(_redo, forgotten);
    }

    private static void Filter(Stack<IReadOnlyDictionary<PhotoItem, MetadataPatch>> stack, ISet<PhotoItem> forgotten)
    {
        var retained = stack
            .Select(snapshot => (IReadOnlyDictionary<PhotoItem, MetadataPatch>)snapshot.Where(pair => !forgotten.Contains(pair.Key)).ToDictionary())
            .Where(snapshot => snapshot.Count > 0)
            .Reverse()
            .ToList();
        stack.Clear();
        foreach (var snapshot in retained) stack.Push(snapshot);
    }

    private static bool Restore(Stack<IReadOnlyDictionary<PhotoItem, MetadataPatch>> source, Stack<IReadOnlyDictionary<PhotoItem, MetadataPatch>> destination, IEnumerable<PhotoItem> items)
    {
        if (!source.TryPop(out var snapshot)) return false;
        var current = items.Where(snapshot.ContainsKey).ToDictionary(item => item, item => item.PendingChanges.Clone());
        foreach (var (item, patch) in snapshot) { item.PendingChanges.CopyFrom(patch); item.NotifyChanged(); }
        destination.Push(current);
        return true;
    }
}
