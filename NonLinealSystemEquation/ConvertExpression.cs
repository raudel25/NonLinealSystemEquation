using Expression;

namespace NonLinealSystemEquation;

public static class ConvertEquation
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

    public static ExpressionType[] ParsingSystem(string[] s)
    {
        ExpressionType[] exps = new ExpressionType[s.Length];

        for (int i = 0; i < s.Length; i++)
        {
            ExpressionType? exp = Parsing(s[i]);

            if (exp is null) return Array.Empty<ExpressionType>();

            exps[i] = exp;
        }

        if (!Check(exps)) return Array.Empty<ExpressionType>();

        return exps;
    }

    private static bool Check(ExpressionType[] exps)
    {
        HashSet<char> variables = new HashSet<char>();
        
        foreach (var item1 in exps)
        {
            List<char> aux = Aux.VariablesToExpression(item1);

            foreach (var item2 in aux) variables.Add(item2);
        }

        return variables.Count == exps.Length;
    }
}