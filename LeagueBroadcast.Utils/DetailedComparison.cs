using System;
using System.Collections.Generic;
using System.Reflection;

namespace LeagueBroadcast.Utils
{
    public static class DetailedComparison
    {
        public static List<Variance> DetailedCompare<T>(this T val1, T val2)
        {
            if(val1 is null)
                throw new ArgumentNullException(nameof(val1));
            List<Variance> variances = new();
            IEnumerable<FieldInfo> fi = val1.GetType()
                .GetFields( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
            foreach (FieldInfo f in fi)
            {
                Variance v = new();
                v.Prop = f.Name;
                v.ValA = f.GetValue(val1);
                v.ValB = f.GetValue(val2);
                if (!Equals(v.ValA, v.ValB))
                    variances.Add(v);

            }
            return variances;
        }
    }

    public class Variance
    {
        public string Prop { get; set; } = "";
        public object? ValA { get; set; }
        public object? ValB { get; set; }
    }
}
