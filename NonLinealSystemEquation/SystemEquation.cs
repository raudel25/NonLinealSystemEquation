using Expression;
using MathNet.Numerics.LinearAlgebra;

namespace NonLinealSystemEquation;

public static class SystemEquation
{
    public static List<(char, double)> ResolveSystem(string[] s)
    {
        (ExpressionType[] exps, List<char> variables) = ConvertEquation.ParsingSystem(s);

        if (exps.Length == 0) return new List<(char, double)>();

        MatrixFunction matrixF = new MatrixFunction(exps, variables);
        VectorFunction vectorF = new VectorFunction(exps);

        return ResolveSystem(matrixF, vectorF, variables);
    }

    private static List<(char, double)> ResolveSystem(MatrixFunction matrixF, VectorFunction vectorF,
        List<char> variables)
    {
        double[] item = new double[variables.Count];
        bool stop = false;

        for (int i = 0; i < variables.Count; i++) item[i] = 1;

            while (!stop)
        {
            List<(char, double)> evaluate = BuildTuple(variables, item);

            (item, stop) = SolveIteration(matrixF.Evaluate(evaluate), vectorF.Evaluate(evaluate), item);
        }

        return BuildTuple(variables, item);
    }

    private static (double[], bool) SolveIteration(double[,] matrix, double[] vector, double[] item)
    {
        Matrix<double> m = Matrix<double>.Build.DenseOfArray(matrix);
        Vector<double> v = Vector<double>.Build.DenseOfArray(vector);

        Vector<double> sol = m.Solve(-v);

        foreach (var i in sol)
        {
            if (i is Double.NaN || i is Double.NegativeInfinity || i is Double.PositiveInfinity)
                return (item, true);
        }

        return ((sol + Vector<double>.Build.DenseOfArray(item)).ToArray(), sol.Norm(1) < 0.000001);
    }

    private static List<(char, double)> BuildTuple(List<char> variables, double[] values)
    {
        List<(char, double)> aux = new List<(char, double)>(values.Length);

        for (int i = 0; i < values.Length; i++) aux.Add((variables[i], values[i]));

        return aux;
    }
}