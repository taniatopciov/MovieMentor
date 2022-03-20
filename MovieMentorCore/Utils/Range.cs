using System.Collections;

namespace MovieMentorCore.Utils;

public class Range : IEnumerable<int>
{
    private readonly int _start;
    private readonly int _end;

    public Range(int start, int end)
    {
        _start = start;
        _end = end;
    }

    public Range(int end) : this(0, end)
    {
    }

    public IEnumerator<int> GetEnumerator()
    {
        for (var i = _start; i < _end; i++)
        {
            yield return i;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
