namespace Expression;

internal static class ReduceTrigonometry
{
    /// <summary>
    /// Reducir la expresion Seno
    /// </summary>
    /// <param name="exp">Expresion para reducir</param>
    /// <returns>Expresion reducida</returns>
    internal static ExpressionType ReduceSin(UnaryExpression exp) => ReduceSinCos(exp, true);

    /// <summary>
    /// Reducir la expresion Coseno
    /// </summary>
    /// <param name="exp">Expresion para reducir</param>
    /// <returns>Expresion reducida</returns>
    internal static ExpressionType ReduceCos(UnaryExpression exp) => ReduceSinCos(exp, false);

    /// <summary>
    /// Reducir la expresion Seno o Coseno
    /// </summary>
    /// <param name="exp">Expresion para reducir</param>
    /// <param name="sin">Seno</param>
    /// <returns>Expresion reducida</returns>
    private static ExpressionType ReduceSinCos(UnaryExpression exp, bool sin)
    {
        NumberExpression? number = exp.Value as NumberExpression;

        if (number is not null)
        {
            if (number.Value == 0)
                return sin ? new NumberExpression(0) : new NumberExpression(1);
        }

        if (exp.Value is ConstantPI)
            return sin ? new NumberExpression(0) : new NumberExpression(-1);

        Multiply? multiply = exp.Value as Multiply;

        ExpressionType? index = null;
        ExpressionType? reduce = null;
        if (multiply is not null)
        {
            if (multiply.Left.Equals(new ConstantPI())) index = multiply.Right;
            if (multiply.Right.Equals(new ConstantPI())) index = multiply.Left;

            number = index as NumberExpression;
            if (number is not null) reduce = Determinate(number.Value, sin);

            BinaryExpression? binary = index as BinaryExpression;
            if (binary is not null)
            {
                number = Aux.Numbers(binary);
                if (number is not null) reduce = Determinate(number.Value, sin);
            }
        }

        return reduce is null ? exp : reduce;
    }

    /// <summary>
    /// Dado el coeficiente de pi determinar si se puede reducir
    /// </summary>
    /// <param name="number">Coeficiente de pi</param>
    /// <param name="sin">Seno</param>
    /// <returns>Expresion reducida(si es null no se puede reducir)</returns>
    private static ExpressionType? Determinate(double number, bool sin)
    {
        if (number-(int)number==0)
        {
            if (sin) return new NumberExpression(0);

            return (int)number % 2 == 0
                ? new NumberExpression(1)
                : new NumberExpression(-1);
        }

        double integer = number >= 0
            ? number - 0.5
            : number + 0.5;

        if (number-(int)number==0)
        {
            ExpressionType? result = Determinate(integer, !sin);

            if (result is null) return null;
            return number >= 0 ? result : new NumberExpression(-1) * result;
        }

        return null;
    }
}