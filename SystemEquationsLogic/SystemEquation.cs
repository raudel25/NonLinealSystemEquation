using Expression;
using MathNet.Numerics.LinearAlgebra;

namespace SystemEquationsLogic;

public static class SystemEquation
{
    /// <summary>
    /// Error en la aproximacion
    /// </summary>
    private static double _e = 0.0000000001;

    public enum SystemState
    {
        Error,
        IncorrectEquations,
        Correct
    }

    /// <summary>
    /// Resolver el sistema
    /// </summary>
    /// <param name="s">Sistema de ecuaciones</param>
    /// <param name="initial">Valores iniciales</param>
    /// <returns>Resultado</returns>
    public static (List<(char, double)>, SystemState) ResolveSystem(string[] s, double[] initial)
    {
        (ExpressionType[] exps, List<char> variables) = ConvertEquation.ParsingSystem(s);

        if (exps.Length == 0) return (new List<(char, double)>(), SystemState.IncorrectEquations);

        if (exps.Length == 1 && Aux.IsPolynomial(exps[0]))
            return FindAllSolutions(exps, variables, initial);

        return ResolveSystem(exps, variables, initial);
    }

    /// <summary>
    /// Determina la lista de variables del sistema
    /// </summary>
    /// <param name="s">Ecuaciones</param>
    /// <returns>Variables del sistema</returns>
    public static List<char> Variables(string[] s) => ConvertEquation.ParsingSystem(s).Item2;

    /// <summary>
    /// Encontrar todas las soluciones de un sistema de una variable
    /// </summary>
    /// <param name="exps">Expresion</param>
    /// <param name="variables">Variable</param>
    /// <param name="initial">Valor inicial</param>
    /// <returns>Lista con las soluciones</returns>
    private static (List<(char, double)>, SystemState) FindAllSolutions(ExpressionType[] exps, List<char> variables,
        double[] initial)
    {
        List<(char, double)> solutions = new List<(char, double)>();
        int ind = 0;

        (List<(char, double)> newSolutions, SystemState state) = ResolveSystem(exps, variables, initial);

        while (ind < 100 && state == SystemState.Correct)
        {
            exps[0] /= new VariableExpression(newSolutions[0].Item1) - new NumberExpression(newSolutions[0].Item2);

            if (!FindSolutionList(solutions, newSolutions[0])) solutions.Add(newSolutions[0]);

            ind++;
            (newSolutions, state) = ResolveSystem(exps, variables, initial);
        }

        return (solutions, solutions.Count == 0 ? SystemState.Error : SystemState.Correct);
    }

    /// <summary>
    /// Determina si la solucion ya ha sido encontrada
    /// </summary>
    /// <param name="list">Lista de soluciones</param>
    /// <param name="sol">Nueva solucion</param>
    /// <returns></returns>
    private static bool FindSolutionList(List<(char, double)> list, (char, double) sol)
    {
        foreach (var item in list)
        {
            if (item.Item1 == sol.Item1 && Math.Abs(item.Item2 - sol.Item2) < 0.0001) return true;
        }

        return false;
    }

    /// <summary>
    /// Resolver el sistema
    /// </summary>
    /// <param name="exps">Ecuasiones</param>
    /// <param name="variables">Variables</param>
    /// <param name="initial">Valores iniciales</param>
    /// <returns>Soluciones</returns>
    private static (List<(char, double)>, SystemState) ResolveSystem(ExpressionType[] exps,
        List<char> variables, double[] initial)
    {
        MatrixFunction matrixF = new MatrixFunction(exps, variables);
        VectorFunction vectorF = new VectorFunction(exps);

        double[] item = initial;
        bool stop = false;
        bool error = false;
        int ind = 0;

        while (!stop && !error)
        {
            List<(char, double)> evaluate = BuildTuple(variables, item);

            Matrix<double> m = Matrix<double>.Build.DenseOfArray(matrixF.Evaluate(evaluate));
            Vector<double> v = Vector<double>.Build.DenseOfArray(vectorF.Evaluate(evaluate));

            if (v.Norm(1) < _e) break;

            (Vector<double> sol, error) = SolveIteration(m, v);
            if (!error) (item, stop) = NewApprox(sol, item);

            ind++;
            if (!error) error = ind == 100000;
        }

        if (error) return (new List<(char, double)>(), SystemState.Error);

        return (BuildTuple(variables, item), SystemState.Correct);
    }

    /// <summary>
    /// Calcular la siguiente aproximacion
    /// </summary>
    /// <param name="sol">Vector solucion</param>
    /// <param name="item">Anterior aproximacion</param>
    /// <returns>Nueva aproximacion, valor de parada</returns>
    private static (double[], bool) NewApprox(Vector<double> sol, double[] item)
    {
        Vector<double> item1 = Vector<double>.Build.DenseOfArray(item);
        Vector<double> item2 = sol + item1;

        bool stop = sol.Norm(1) < _e;

        return (item2.ToArray(), stop);
    }

    /// <summary>
    /// Resolver el sistam de ecuaciones generado en una iteracion
    /// </summary>
    /// <param name="matrix">Matriz</param>
    /// <param name="vector">Vector</param>
    /// <returns>Solucion, error</returns>
    private static (Vector<double>, bool) SolveIteration(Matrix<double> matrix, Vector<double> vector)
    {
        Vector<double> sol = matrix.Solve(-vector);

        foreach (var i in sol)
        {
            if (i is Double.NaN || i is Double.NegativeInfinity || i is Double.PositiveInfinity)
                return (sol, true);
        }

        return (sol, false);
    }

    private static List<(char, double)> BuildTuple(List<char> variables, double[] values)
    {
        List<(char, double)> aux = new List<(char, double)>(values.Length);

        for (int i = 0; i < values.Length; i++) aux.Add((variables[i], values[i]));

        return aux;
    }
}