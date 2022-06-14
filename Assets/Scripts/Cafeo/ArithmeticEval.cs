using System.Collections.Generic;
using System.Data;

namespace Cafeo
{
    public static class ArithmeticEval
    {
        public static float EvaluateArithmeticExpression(string expression, Dictionary<string, float> state)
        {
            var table = new DataTable();
            foreach (var (k, v) in state)
            {
                var c = table.Columns.Add(k, typeof(float));
                c.DefaultValue = v;
            }

            var res = table.Columns.Add("result", typeof(float));
            res.Expression = expression;
            var row = table.NewRow();
            table.Rows.Add(row);
            using var view = new DataView(table);
            return (float)view[0]["result"];
        }
    }
}