using Expression.Arithmetics;
using Expression.Expressions;
using MathNet.Numerics.LinearAlgebra;

namespace SystemEquationsLogic;

public static class SystemEquation
{
    public enum SystemState
    {
        Error,
        IncorrectEquations,
        Correct
    }

    /// <summary>
    ///     Error en la aproximacion
    /// </summary>
    private const double E = 0.0000000001;

    /// <summary>
    ///     Determina si la expresion es un polinomio
    /// </summary>
    /// <param name="exp">Expresion</param>
    /// <returns>Si la expresion es un polinomio</returns>
    private static bool IsPolynomial(Function<double> exp)
    {
        if (exp is NumberExpression<double> || exp is VariableExpression<double>) return true;

        if (exp is BinaryExpression<double> binary)
        {
            if (binary is Division<double>)
                return binary.Right is NumberExpression<double> && IsPolynomial(binary.Left);
            if (binary is Pow<double>)
            {
                if (binary.Right is NumberExpression<double> number)
                    return (int)number.Value - number.Value == 0 && IsPolynomial(binary.Left);
            }

            if (binary is Sum<double> || binary is Subtraction<double> || binary is Multiply<double>)
                return IsPolynomial(binary.Left) && IsPolynomial(binary.Right);
        }

        return false;
    }

    /// <summary>
    ///     Resolver el sistema
    /// </summary>
    /// <param name="s">Sistema de ecuaciones</param>
    /// <param name="initial">Valores iniciales</param>
    /// <param name="arithmeticExp">Aritmetica</param>
    /// <returns>Resultado</returns>
    public static (List<(char, double)>, SystemState) ResolveSystem(string[] s, double[] initial,
        ArithmeticExp<double> arithmeticExp)
    {
        var (exps, variables) = ConvertEquation.ParsingSystem(s, arithmeticExp);

        if (exps.Length == 0) return (new List<(char, double)>(), SystemState.IncorrectEquations);

        if (exps.Length == 1 && IsPolynomial(exps[0]))
            return FindAllSolutions(exps, variables, initial, arithmeticExp);

        return ResolveSystem(exps, variables, initial);
    }

    /// <summary>
    ///     Determina la lista de variables del sistema
    /// </summary>
    /// <param name="s">Ecuaciones</param>
    /// <param name="arithmeticExp">Aritmetica</param>
    /// <returns>Variables del sistema</returns>
    public static List<char> Variables(string[] s, ArithmeticExp<double> arithmeticExp)
    {
        return ConvertEquation.ParsingSystem(s, arithmeticExp).Item2;
    }

    /// <summary>
    ///     Encontrar todas las soluciones de un sistema de una variable
    /// </summary>
    /// <param name="exps">Expresion</param>
    /// <param name="variables">Variable</param>
    /// <param name="initial">Valor inicial</param>
    /// <param name="arithmeticExp">Aritmetica</param>
    /// <returns>Lista con las soluciones</returns>
    private static (List<(char, double)>, SystemState) FindAllSolutions(Function<double>[] exps,
        List<char> variables,
        double[] initial, ArithmeticExp<double> arithmeticExp)
    {
        var solutions = new List<(char, double)>();
        var ind = 0;

        var (newSolutions, state) = ResolveSystem(exps, variables, initial);

        while (ind < 100 && state == SystemState.Correct)
        {
            exps[0] /= arithmeticExp.VariableExpression(newSolutions[0].Item1) -
                       arithmeticExp.NumberExpression(newSolutions[0].Item2);

            if (!FindSolutionList(solutions, newSolutions[0])) solutions.Add(newSolutions[0]);

            ind++;
            (newSolutions, state) = ResolveSystem(exps, variables, initial);
        }

        return (solutions, solutions.Count == 0 ? SystemState.Error : SystemState.Correct);
    }

    /// <summary>
    ///     Determina si la solucion ya ha sido encontrada
    /// </summary>
    /// <param name="list">Lista de soluciones</param>
    /// <param name="sol">Nueva solucion</param>
    /// <returns></returns>
    private static bool FindSolutionList(List<(char, double)> list, (char, double) sol)
    {
        foreach (var item in list)
            if (item.Item1 == sol.Item1 && Math.Abs(item.Item2 - sol.Item2) < 0.0001)
                return true;

        return false;
    }

    /// <summary>
    ///     Resolver el sistema
    /// </summary>
    /// <param name="exps">Ecuasiones</param>
    /// <param name="variables">Variables</param>
    /// <param name="initial">Valores iniciales</param>
    /// <returns>Soluciones</returns>
    private static (List<(char, double)>, SystemState) ResolveSystem(Function<double>[] exps,
        List<char> variables, double[] initial)
    {
        var matrixF = new MatrixFunction(exps, variables);
        var vectorF = new VectorFunction(exps);

        var item = initial;
        var stop = false;
        var error = false;
        var ind = 0;

        while (!stop && !error)
        {
            var evaluate = BuildTuple(variables, item);

            var m = Matrix<double>.Build.DenseOfArray(matrixF.Evaluate(evaluate));
            var v = Vector<double>.Build.DenseOfArray(vectorF.Evaluate(evaluate));

            if (v.Norm(1) < E) break;

            error = VectorError(v) || MatrixError(m) || error;

            var sol = m.Solve(-v);
            error = VectorError(sol) || error;

            if (!error) (item, stop) = NewApprox(sol, item);

            ind++;
            if (!error) error = ind == 100000;
        }

        if (error) return (new List<(char, double)>(), SystemState.Error);

        return (BuildTuple(variables, item), SystemState.Correct);
    }

    /// <summary>
    ///     Calcular la siguiente aproximacion
    /// </summary>
    /// <param name="sol">Vector solucion</param>
    /// <param name="item">Anterior aproximacion</param>
    /// <returns>Nueva aproximacion, valor de parada</returns>
    private static (double[], bool) NewApprox(Vector<double> sol, double[] item)
    {
        var item1 = Vector<double>.Build.DenseOfArray(item);
        var item2 = sol + item1;

        var stop = sol.Norm(1) < E;

        return (item2.ToArray(), stop);
    }

    /// <summary>
    ///     Determinar si hay errores de calculo
    /// </summary>
    /// <param name="matrix">Matrix</param>
    /// <returns></returns>
    private static bool MatrixError(Matrix<double> matrix)
    {
        for (var i = 0; i < matrix.RowCount; i++)
        for (var j = 0; j < matrix.ColumnCount; j++)
            if (matrix[i, j] is double.NaN || matrix[i, j] is double.NegativeInfinity ||
                matrix[i, j] is double.PositiveInfinity)
                return true;

        return false;
    }

    /// <summary>
    ///     Determinar si hay errores de calculo
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <returns></returns>
    private static bool VectorError(Vector<double> vector)
    {
        foreach (var i in vector)
            if (i is double.NaN || i is double.NegativeInfinity || i is double.PositiveInfinity)
                return true;

        return false;
    }

    private static List<(char, double)> BuildTuple(List<char> variables, double[] values)
    {
        var aux = new List<(char, double)>(values.Length);

        for (var i = 0; i < values.Length; i++) aux.Add((variables[i], values[i]));

        return aux;
    }
}