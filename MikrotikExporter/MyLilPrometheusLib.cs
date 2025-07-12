namespace MikrotikExporter;

public enum MetricType
{
    Gauge,
    Counter,
    Histogram,
    Summary
};

file static class LabelStringExtensions
{
    public static string EscapeLabel(this string value) =>
        value.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\"", "\\\"");
}

public abstract class MetricInfo
{
    protected MetricInfo(MetricType type, string name, string help)
    {
        Type = type;
        Name = name;
        Help = help;
        
        // This string will never change during lifetime of this instance, so let's cache it
        _stringified = $"# HELP {Name} {Help}\n# TYPE {Name} {Type.ToString().ToLowerInvariant()}\n";
    }
    
    public MetricType Type { get; }
    public string Name { get; }
    public string Help { get; }

    private readonly string _stringified;

    public override string ToString()
    {
        return _stringified;
    }
}

public abstract class MetricInfo<T> : MetricInfo
{
    protected MetricInfo(MetricType type, string name, string help) : base(type, name, help) { }
    
    public abstract MetricValueSet<T> CreateValueSet();
    public abstract MetricValueSet<T> CreateValueSet(Dictionary<string, string> staticLabels);
}

public abstract class MetricValueSet
{
    protected MetricValueSet() {}
    protected MetricValueSet(Dictionary<string, string> staticLabels)
    {
        _staticLabels = staticLabels;
    }
    
    public IReadOnlyDictionary<string, string> StaticLabels => _staticLabels;
    private readonly Dictionary<string, string> _staticLabels = new();
    public abstract MetricInfo Info { get; }
}

public abstract class MetricValueSet<T> : MetricValueSet
{
    public override MetricInfo<T> Info { get; }
    
    public abstract void AddValue(T value);
    public abstract void AddValue(T value, Dictionary<string, string> labels);
    
    protected MetricValueSet(MetricInfo<T> metricInfo)
    {
        Info = metricInfo;
    }

    protected MetricValueSet(MetricInfo<T> info, Dictionary<string, string> staticLabels)
    : base(staticLabels)
    {
        Info = info;
    }

    public override string ToString()
    {
        return Info.ToString();
    }
}

public class MetricValue
{
    public MetricValue(string value)
    {
        Value = value;
    }

    public MetricValue(string value, Dictionary<string, string> labels)
    {
        Value = value;
        _labels = labels;
    }
    
    public string Value { get; }
    
    public IReadOnlyDictionary<string, string> Labels => _labels;
    
    private readonly Dictionary<string, string> _labels = new();
}

public class Gauge<T> : MetricInfo<T>
{
    public Gauge(string name, string help, Func<T, string?> converter) : base(MetricType.Gauge, name, help)
    {
        Converter = converter;
    }

    public override GaugeValueSet<T> CreateValueSet()
    {
        return new GaugeValueSet<T>(this);
    }

    public override GaugeValueSet<T> CreateValueSet(Dictionary<string, string> staticLabels)
    {
        return new GaugeValueSet<T>(this, staticLabels);
    }

    public Func<T, string?> Converter { get; }
}

public class SimpleValueSet<T> : MetricValueSet<T>
{
    protected SimpleValueSet(MetricInfo<T> info, Func<T, string?> converter) : base(info)
    {
        _converter = converter;
    }

    protected SimpleValueSet(MetricInfo<T> info, Func<T, string?> converter, Dictionary<string, string> staticLabels) : base(info, staticLabels)
    {
        _converter = converter;
    }
    
    private readonly List<MetricValue> _values = [];
    private readonly Func<T, string?> _converter;

    public override void AddValue(T value)
    {
        var x = _converter(value);

        if (x is null)
        {
            return;
        }
        
        _values.Add(new MetricValue(x));
    }
    
    public override void AddValue(T value, Dictionary<string, string> labels)
    {
        var x = _converter(value);

        if (x is null)
        {
            return;
        }
        
        _values.Add(new MetricValue(x, labels));
    }

    public override string ToString()
    {
        var header = Info.ToString();
        var name = Info.Name;
        var staticLabels = string.Join(',', StaticLabels.Select(x => $"{x.Key}=\"{x.Value.EscapeLabel()}\""));

        var values = string.Join('\n', _values.Select(v =>
        {
            var labels = staticLabels;

            if (labels.Length > 0 && v.Labels.Count > 0)
            {
                labels += ',';
            }
            
            labels += string.Join(",", v.Labels.Select(x => $"{x.Key}=\"{x.Value.EscapeLabel()}\""));
            
            return $"{name}{{{labels}}} {v.Value}";
        }));

        return header + values;
    }
}

public class GaugeValueSet<T> : SimpleValueSet<T>
{
    public GaugeValueSet(Gauge<T> info) : base(info, info.Converter)
    {
    }

    public GaugeValueSet(Gauge<T> info, Dictionary<string, string> staticLabels) : base(info, info.Converter, staticLabels)
    {
    }
}

public class Counter<T> : MetricInfo<T>
{
    public Counter(string name, string help, Func<T, string> converter) : base(MetricType.Counter, name, help)
    {
        Converter = converter;
    }

    public override CounterValueSet<T> CreateValueSet()
    {
        return new CounterValueSet<T>(this);
    }

    public override CounterValueSet<T> CreateValueSet(Dictionary<string, string> staticLabels)
    {
        return new CounterValueSet<T>(this, staticLabels);
    }
    
    public Func<T, string> Converter { get; }
}

public class CounterValueSet<T> : SimpleValueSet<T>
{
    public CounterValueSet(Counter<T> info) : base(info, info.Converter)
    {
    }

    public CounterValueSet(Counter<T> info, Dictionary<string, string> staticLabels) : base(info, info.Converter, staticLabels)
    {
    }
}

public abstract class MetricsCollection
{
    public abstract override string ToString();
    
    public static MetricsCollection Empty => new MetricsCollection<bool>();
}

public class MetricsCollection<T> : MetricsCollection
{
    private readonly List<MetricValueSet<T>> _metrics = [];

    public void CreateValueSet(MetricInfo<T> info)
    {
        _metrics.Add(info.CreateValueSet());
    }

    public void CreateValueSet(MetricInfo<T> info, Dictionary<string, string> staticLabels)
    {
        _metrics.Add(info.CreateValueSet(staticLabels));
    }

    public void CreateValueSets(params IEnumerable<MetricInfo<T>> infos)
    {
        _metrics.AddRange(infos.Select(i => i.CreateValueSet()));
    }

    public void CreateValueSets(Dictionary<string, string> staticLabels, params IEnumerable<MetricInfo<T>> infos)
    {
        _metrics.AddRange(infos.Select(i => i.CreateValueSet(staticLabels)));
    }

    public void MergeWith(MetricsCollection<T> other)
    {
        _metrics.AddRange(other._metrics);
    }

    public static MetricsCollection<T> Merge(IEnumerable<MetricsCollection<T>> collections)
    {
        var result = new MetricsCollection<T>();
        foreach (var collection in collections)
        {
            result.MergeWith(collection);
        }
        
        return result;
    }

    public void AddValue(T value)
    {
        foreach (var metric in _metrics)
        {
            metric.AddValue(value);
        }
    }
    
    public void AddValue(T value, Dictionary<string, string> labels)
    {
        foreach (var metric in _metrics)
        {
            metric.AddValue(value, labels);
        }
    }

    public override string ToString()
    {
        IEnumerable<string> xpp = _metrics.Select(x => x.ToString());
        
        return string.Join('\n', xpp);
    }
}