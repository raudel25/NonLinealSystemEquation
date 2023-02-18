using Expression.Arithmetics;
using Expression.Expressions;
using Expression.Reduce;

namespace SystemEquationsLogic;

internal static class ConvertEquation
{
    /// <summary>
    ///     Parsear la ecuacion
    /// </summary>
    /// <param name="s">Ecuacion</param>
    /// <param name="arithmeticExp">Aritmetica</param>
    /// <returns>Expresion resultante</returns>
    private static ExpressionType<double>? Parsing(string s, ArithmeticExp<double> arithmeticExp)
    {
        var aux = s.Split('=');

        if (aux.Length != 2) return null;

        var exp1 = arithmeticExp.Parsing(aux[0]);
        var exp2 = arithmeticExp.Parsing(aux[1]);

        if (exp1 is null || exp2 is null) return null;

        return ReduceExpression<double>.Reduce(exp1 - exp2);
    }

    /// <summary>
    ///     Parsear el sistema de ecuaciones
    /// </summary>
    /// <param name="s">Sistema de ecuaciones</param>
    /// <param name="arithmeticExp">Aritmetica</param>
    /// <returns>Lista de expresiones, Lista de variables</returns>
    internal static (ExpressionType<double>[], List<char>) ParsingSystem(string[] s,
        ArithmeticExp<double> arithmeticExp)
    {
        var exps = new ExpressionType<double>[s.Length];
        var variables = new List<char>(s.Length);

        for (var i = 0; i < s.Length; i++)
        {
            var exp = Parsing(s[i], arithmeticExp);

            if (exp is null) return (Array.Empty<ExpressionType<double>>(), variables);

            exps[i] = exp;
        }

        if (!Check(exps, variables)) return (Array.Empty<ExpressionType<double>>(), variables);

        return (exps, variables);
    }

    /// <summary>
    ///     Determinar si el sistema es correcto
    /// </summary>
    /// <param name="exps">Expresiones</param>
    /// <param name="variables">Variables</param>
    /// <returns>Si es sistema es correcto</returns>
    private static bool Check(ExpressionType<double>[] exps, List<char> variables)
    {
        var variablesSystem = new HashSet<char>();

        foreach (var item1 in exps)
        {
            var aux = ExpressionType<double>.VariablesToExpression(item1);

            foreach (var item2 in aux) variablesSystem.Add(item2);
        }

        foreach (var item in variablesSystem) variables.Add(item);

        return variablesSystem.Count == exps.Length;
    }
}