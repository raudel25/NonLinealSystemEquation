namespace Expression;

public static class ReduceMultiplyDivision
{
    /// <summary>
    /// Reducir una multiplicacion
    /// </summary>
    /// <param name="binary">Expresion binaria</param>
    /// <returns>Expresion resultante</returns>
    internal static ExpressionType ReduceMultiply(BinaryExpression binary)
    {
        ExpressionType? aux = ReduceMultiplySimple(binary);
        if (aux is not null) return aux;

        aux = ReduceMultiply(binary.Left, binary.Right, 1, 1);
        if (aux is not null) return aux;

        return binary;
    }

    /// <summary>
    /// Determinar si una multiplicacion se puede reducir dadas caracteristicas simples
    /// </summary>
    /// <param name="binary">Expresion binaria</param>
    /// <returns>Expresion resultante(si es null es que no se pudo reducir)</returns>
    internal static ExpressionType? ReduceMultiplySimple(BinaryExpression binary)
    {
        if (binary.Left.Equals(new NumberExpression(0)) ||
            binary.Right.Equals(new NumberExpression(0)))
            return new NumberExpression(0);
        if (binary.Left.Equals(new NumberExpression(1))) return binary.Right;
        if (binary.Right.Equals(new NumberExpression(1))) return binary.Left;

        return null;
    }

    /// <summary>
    /// Comprobar si es posible reducir nuevamente la multiplicacion
    /// </summary>
    /// <param name="left">Expresion izquierda</param>
    /// <param name="right">Expresion derecha</param>
    /// <param name="indLeft">Indice de la expresion izquierda</param>
    /// <param name="indRight">Indice de la expresion derecha</param>
    /// <returns>Expresion resultante</returns>
    private static ExpressionType ReduceMultiplyCheck(ExpressionType left, ExpressionType right, double indLeft,
        double indRight)
    {
        ExpressionType? aux = ReduceMultiply(left, right, indLeft, indRight);
        if (aux is not null) return aux;

        BinaryExpression binary = Pow.DeterminatePow(left, new NumberExpression(indLeft)) *
                                  Pow.DeterminatePow(right, new NumberExpression(indRight));

        aux = ReduceMultiplySimple(binary);
        if (aux is not null) return aux;

        return binary;
    }

    /// <summary>
    /// Determinar si es posible reducir una multiplicacion dadas su expresion izquierda y derecha
    /// </summary>
    /// <param name="left">Expresion izquierda</param>
    /// <param name="right">Expresion derecha</param>
    /// <param name="indLeft">Indice de la expresion izquierda</param>
    /// <param name="indRight">Indice de la expresion derecha</param>
    /// <returns>Expresion resultante(si es null es que no se pudo reducir)</returns>
    private static ExpressionType? ReduceMultiply(ExpressionType left, ExpressionType right, double indLeft,
        double indRight)
    {
        var auxExp = PowInd(left);
        (left, indLeft) = (auxExp.Item1, auxExp.Item2 * indLeft);
        auxExp = PowInd(right);
        (right, indRight) = (auxExp.Item1, auxExp.Item2 * indRight);

        (BinaryExpression? leftBinary, BinaryExpression? rightBinary) =
            (left as BinaryExpression, right as BinaryExpression);

        ExpressionType? aux;

        if (leftBinary is Multiply)
        {
            aux = ReduceMultiply(leftBinary.Left, right, indLeft, indRight);
            if (aux is not null) return ReduceMultiplyCheck(leftBinary.Right, aux, indLeft, 1);
            aux = ReduceMultiply(leftBinary.Right, right, indLeft, indRight);
            if (aux is not null) return ReduceMultiplyCheck(leftBinary.Left, aux, indLeft, 1);
        }

        if (rightBinary is Multiply)
        {
            aux = ReduceMultiply(left, rightBinary.Left, indLeft, indRight);
            if (aux is not null) return ReduceMultiplyCheck(aux, rightBinary.Right, 1, indRight);
            aux = ReduceMultiply(left, rightBinary.Right, indLeft, indRight);
            if (aux is not null) return ReduceMultiplyCheck(aux, rightBinary.Left, 1, indRight);
        }

        if (leftBinary is Division)
        {
            aux = ReduceMultiply(leftBinary.Left, right, indLeft, indRight);
            if (aux is not null) return ReduceDivisionCheck(aux, leftBinary.Right, 1, indLeft);
            aux = ReduceDivision(right, leftBinary.Right, indRight, indLeft);
            if (aux is not null) return ReduceMultiplyCheck(leftBinary.Left, aux, indLeft, 1);
        }

        if (rightBinary is Division)
        {
            aux = ReduceMultiply(left, rightBinary.Left, indLeft, indRight);
            if (aux is not null) return ReduceDivisionCheck(aux, rightBinary.Right, 1, indRight);
            aux = ReduceDivision(left, rightBinary.Right, indLeft, indRight);
            if (aux is not null) return ReduceMultiplyCheck(aux, rightBinary.Left, 1, indRight);
        }

        if (left.Equals(right))
        {
            BinaryExpression pow = Pow.DeterminatePow(left, new NumberExpression(indLeft + indRight));
            aux = ReducePowLogarithm.ReducePowSimple(pow);
            if (aux is not null) return aux;

            return pow;
        }

        if (left is NumberExpression && right is NumberExpression)
            return new NumberExpression(Math.Pow(left.Evaluate(new List<(char, double)>()), indLeft) *
                                        Math.Pow(right.Evaluate(new List<(char, double)>()), indRight));

        return null;
    }

    /// <summary>
    /// Reducir una division
    /// </summary>
    /// <param name="binary">Expresion binaria</param>
    /// <returns>Expresion resultante</returns>
    internal static ExpressionType ReduceDivision(BinaryExpression binary)
    {
        ExpressionType? aux = ReduceDivisionSimple(binary);
        if (aux is not null) return aux;

        aux = ReduceDivision(binary.Left, binary.Right, 1, 1);
        if (aux is not null) return aux;

        return binary;
    }

    /// <summary>
    /// Determinar si una division se puede reducir dadas caracteristicas simples
    /// </summary>
    /// <param name="binary">Expresion binaria</param>
    /// <returns>Expresion resultante(si es null es que no se pudo reducir)</returns>
    internal static ExpressionType? ReduceDivisionSimple(BinaryExpression binary)

    {
        if (binary.Left.Equals(new NumberExpression(0)))
            return new NumberExpression(0);
        if (binary.Right.Equals(new NumberExpression(1))) return binary.Left;

        return null;
    }

    /// <summary>
    /// Comprobar si es posible reducir nuevamente la division
    /// </summary>
    /// <param name="left">Expresion izquierda</param>
    /// <param name="right">Expresion derecha</param>
    /// <param name="indLeft">Indice de la expresion izquierda</param>
    /// <param name="indRight">Indice de la expresion derecha</param>
    /// <returns>Expresion resultante</returns>
    private static ExpressionType ReduceDivisionCheck(ExpressionType left, ExpressionType right, double indLeft,
        double indRight)
    {
        ExpressionType? aux = ReduceDivision(left, right, indLeft, indRight);
        if (aux is not null) return aux;

        BinaryExpression binary = Pow.DeterminatePow(left, new NumberExpression(indLeft)) /
                                  Pow.DeterminatePow(right, new NumberExpression(indRight));

        aux = ReduceDivisionSimple(binary);
        if (aux is not null) return aux;

        return binary;
    }

    /// <summary>
    /// Determinar si es posible reducir una division dadas su expresion izquierda y derecha
    /// </summary>
    /// <param name="left">Expresion izquierda</param>
    /// <param name="right">Expresion derecha</param>
    /// <param name="indLeft">Indice de la expresion izquierda</param>
    /// <param name="indRight">Indice de la expresion derecha</param>
    /// <returns>Expresion resultante(si es null es que no se pudo reducir)</returns>
    private static ExpressionType? ReduceDivision(ExpressionType left, ExpressionType right, double indLeft,
        double indRight)
    {
        var auxExp = PowInd(left);
        (left, indLeft) = (auxExp.Item1, auxExp.Item2 * indLeft);
        auxExp = PowInd(right);
        (right, indRight) = (auxExp.Item1, auxExp.Item2 * indRight);

        (BinaryExpression? leftBinary, BinaryExpression? rightBinary) =
            (left as BinaryExpression, right as BinaryExpression);

        ExpressionType? aux;

        if (leftBinary is Multiply)
        {
            aux = ReduceDivision(leftBinary.Left, right, indLeft, indRight);
            if (aux is not null) return ReduceMultiplyCheck(leftBinary.Right, aux, indLeft, 1);
            aux = ReduceDivision(leftBinary.Right, right, indLeft, indRight);
            if (aux is not null) return ReduceMultiplyCheck(leftBinary.Left, aux, indLeft, 1);
        }

        if (rightBinary is Multiply)
        {
            aux = ReduceDivision(left, rightBinary.Left, indLeft, indRight);
            if (aux is not null) return ReduceDivisionCheck(aux, rightBinary.Right, 1, indRight);
            aux = ReduceDivision(left, rightBinary.Right, indLeft, indRight);
            if (aux is not null) return ReduceDivisionCheck(aux, rightBinary.Left, 1, indRight);
        }

        if (leftBinary is Division)
        {
            aux = ReduceMultiply(leftBinary.Right, right, indLeft, indRight);
            if (aux is not null) return ReduceDivisionCheck(leftBinary.Left, aux, indLeft, 1);
            aux = ReduceDivision(leftBinary.Left, right, indLeft, indRight);
            if (aux is not null) return ReduceDivisionCheck(aux, leftBinary.Right, 1, indLeft);
        }

        if (rightBinary is Division)
        {
            aux = ReduceMultiply(left, rightBinary.Right, indLeft, indRight);
            if (aux is not null) return ReduceDivisionCheck(aux, rightBinary.Left, 1, indRight);
            aux = ReduceDivision(left, rightBinary.Left, indLeft, indRight);
            if (aux is not null) return ReduceMultiplyCheck(aux, rightBinary.Right, 1, indRight);
        }

        if (left.Equals(right))
        {
            BinaryExpression pow = Pow.DeterminatePow(left, new NumberExpression(indLeft - indRight));
            aux = ReducePowLogarithm.ReducePowSimple(pow);
            if (aux is not null) return aux;

            return pow;
        }

        if (left is NumberExpression && right is NumberExpression)
            return new NumberExpression(Math.Pow(left.Evaluate(new List<(char, double)>()), indLeft) /
                                        Math.Pow(right.Evaluate(new List<(char, double)>()), indRight));

        return null;
    }

    private static (ExpressionType, double) PowInd(ExpressionType exp)
    {
        Pow? binary = exp as Pow;
        if (binary is null) return (exp, 1);

        if (binary.Right is NumberExpression) return (binary.Left, ((NumberExpression) binary.Right).Value);

        return (binary, 1);
    }
}