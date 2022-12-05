using Expression;

namespace SystemEquationsLogic;

internal static class ConvertEquation
{
    private static ExpressionType? Parsing(string s)
    {
        string[] aux = s.Split('=');

        if (aux.Length != 2) return null;

        ExpressionType? exp1 = ConvertExpression.Parsing(aux[0]);
        ExpressionType? exp2 = ConvertExpression.Parsing(aux[1]);

        if (exp1 is null || exp2 is null) return null;

        return ReduceExpression.Reduce(exp1 - exp2);
    }

    internal static (ExpressionType[], List<char>) ParsingSystem(string[] s)
    {
        ExpressionType[] exps = new ExpressionType[s.Length];
        List<char> variables = new List<char>(s.Length);

        for (int i = 0; i < s.Length; i++)
        {
            ExpressionType? exp = Parsing(s[i]);

            if (exp is null) return (Array.Empty<ExpressionType>(), variables);

            exps[i] = exp;
        }

        if (!Check(exps, variables)) return (Array.Empty<ExpressionType>(), variables);

        return (exps, variables);
    }

    private static bool Check(ExpressionType[] exps, List<char> variables)
    {
        HashSet<char> variablesSystem = new HashSet<char>();

        foreach (var item1 in exps)
        {
            List<char> aux = Aux.VariablesToExpression(item1);

            foreach (var item2 in aux) variablesSystem.Add(item2);
        }

        foreach (var item in variablesSystem) variables.Add(item);

        return variablesSystem.Count == exps.Length;
    }
}