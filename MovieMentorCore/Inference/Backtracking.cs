namespace MovieMentorCore.Inference;

public class Backtracking<T>
{
    public delegate bool Predicate(int step, T[] current);

    public delegate IEnumerable<T> PossibilitiesFunction(int step);

    private readonly T[] _current;
    private readonly IList<T[]> _solutions = new List<T[]>();
    private readonly Predicate _isValidPredicate;
    private readonly Predicate _isSolutionPredicate;
    private readonly PossibilitiesFunction _possibilitiesFunction;
    private readonly IDictionary<int, IEnumerable<T>> _possibilities = new Dictionary<int, IEnumerable<T>>();

    public Backtracking(int solutionSize, Predicate isValidPredicate, Predicate isSolutionPredicate,
        PossibilitiesFunction possibilitiesFunction)
    {
        _isValidPredicate = isValidPredicate;
        _isSolutionPredicate = isSolutionPredicate;
        _possibilitiesFunction = possibilitiesFunction;
        _current = new T[solutionSize];
    }

    public IEnumerable<T[]> GetSolutions()
    {
        for (var i = 0; i < _current.Length; i++)
        {
            _possibilities.Add(i, _possibilitiesFunction(i));
        }

        Back(0);
        return _solutions;
    }

    private void Back(int step)
    {
        if (step >= _current.Length)
        {
            return;
        }

        foreach (var possibility in _possibilities[step])
        {
            _current[step] = possibility;
            if (!_isValidPredicate(step, _current))
            {
                continue;
            }

            if (_isSolutionPredicate(step, _current))
            {
                var solution = new T[_current.Length];
                _current.CopyTo(solution, 0);

                _solutions.Add(solution);
            }
            else
            {
                Back(step + 1);
            }
        }
    }
}
