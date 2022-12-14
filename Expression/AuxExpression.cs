namespace Expression;

public static class Aux
{
    /// <summary>
    /// Determina si la expresion es un polinomio
    /// </summary>
    /// <param name="exp">Expresion</param>
    /// <returns>Si la expresion es un polinomio</returns>
    public static bool IsPolynomial(ExpressionType exp)
    {
        if (exp is NumberExpression || exp is VariableExpression) return true;

        BinaryExpression? binary = exp as BinaryExpression;

        if (binary is not null)
        {
            if (binary is Division || binary is Pow)
                return binary.Right is NumberExpression && IsPolynomial(binary.Left);
            if (binary is Sum || binary is Subtraction || binary is Multiply)
                return IsPolynomial(binary.Left) && IsPolynomial(binary.Right);
        }

        return false;
    }

    /// <summary>
    /// Colocar parentesis
    /// </summary>
    /// <param name="s">Cadena de texto</param>
    /// <returns>Cadena modificada</returns>
    public static string Colocated(string s) => s[0] == '(' && s[^1] == ')' ? s : $"({s})";

    /// <summary>
    /// Colocar el signo negativo
    /// </summary>
    /// <param name="exp">Cadena de texto</param>
    /// <returns>Cadena modificada</returns>
    public static string Opposite(ExpressionType exp)
    {
        string s = exp.ToString()!;
        if (s[0] == '-')
            return s.Substring(1, s.Length - 1);
        if (s == "0") return "0";
        return exp.Priority == 1 ? $"-({exp})" : $"-{exp}";
    }

    /// <summary>
    /// Determinar las varibles de una expresion
    /// </summary>
    /// <param name="exp">Expresion</param>
    /// <returns>Lista de variables de la expresion</returns>
    public static List<char> VariablesToExpression(ExpressionType exp)
    {
        HashSet<char> variables = new HashSet<char>();
        VariablesToExpression(exp, variables);

        return variables.ToList();
    }

    private static void VariablesToExpression(ExpressionType exp, HashSet<char> variables)
    {
        VariableExpression? variable = exp as VariableExpression;

        if (variable != null)
        {
            if (!variables.Contains(variable.Variable)) variables.Add(variable.Variable);
            return;
        }

        BinaryExpression? binary = exp as BinaryExpression;

        if (binary != null)
        {
            VariablesToExpression(binary.Left, variables);
            VariablesToExpression(binary.Right, variables);
            return;
        }

        UnaryExpression? unary = exp as UnaryExpression;

        if (unary != null) VariablesToExpression(unary.Value, variables);
    }

    /// <summary>
    /// Determinar si la expresion es completamente numerica
    /// </summary>
    /// <param name="binary">Expresion binaria</param>
    /// <returns>Expresion resultante(si es null es que no se pudo reducir)</returns>
    internal static NumberExpression? Numbers(BinaryExpression binary)
    {
        if (binary.Left is NumberExpression && binary.Right is NumberExpression)
            return new NumberExpression(binary.Evaluate(new List<(char, double)>()));

        return null;
    }

    internal static long Factorial(int n)
    {
        long factorial = 1;

        for (int i = 1; i <= n; i++) factorial *= i;

        return factorial;
    }
}