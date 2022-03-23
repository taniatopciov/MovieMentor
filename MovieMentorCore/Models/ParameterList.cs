using System.Collections;

namespace MovieMentorCore.Models;

public class ParameterList : IEnumerable<KeyValuePair<string, Parameter>>
{
    private Dictionary<string, Parameter> Parameters { get; } = new();

    public int Count => Parameters.Count;

    public bool HasReferences => Parameters.Values.Any(param => param is Parameter.Reference);

    private ParameterList()
    {
    }

    public Parameter? this[string parameterName]
    {
        get => Parameters.TryGetValue(parameterName, out var parameter) ? parameter : null;
        set
        {
            if (value != null)
            {
                Parameters[parameterName] = value;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<KeyValuePair<string, Parameter>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, Parameter>>)Parameters).GetEnumerator();
    }

    public class Builder
    {
        private readonly ParameterList _parameterList;

        public Builder()
        {
            _parameterList = new ParameterList();
        }

        public Builder AddParameter(string name, Parameter parameter)
        {
            _parameterList.Parameters[name] = parameter;
            return this;
        }

        public ParameterList Build()
        {
            return _parameterList;
        }
    }
}
