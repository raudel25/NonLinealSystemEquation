using Expression;
using MathNet.Numerics.LinearAlgebra;

namespace NonLinealSystemEquation;

public static class SystemEquation
{
    public enum SystemState
    {
        Error,
        IncorrectEquations,
        Correct
    }
    public static (List<(char, double)>,SystemState) ResolveSystem(string[] s)
    {
        (ExpressionType[] exps, List<char> variables) = ConvertEquation.ParsingSystem(s);

        if (exps.Length == 0) return (new List<(char, double)>(),SystemState.IncorrectEquations);

        MatrixFunction matrixF = new MatrixFunction(exps, variables);
        VectorFunction vectorF = new VectorFunction(exps);

        return ResolveSystem(matrixF, vectorF, variables);
    }

    private static (List<(char, double)>,SystemState) ResolveSystem(MatrixFunction matrixF, VectorFunction vectorF,
        List<char> variables)
    {
        double[] item = new double[variables.Count];
        bool stop = false;
        bool error = false;
        int ind = 0;

        for (int i = 0; i < variables.Count; i++) item[i] = 1;

        while (!stop)
        {
            List<(char, double)> evaluate = BuildTuple(variables, item);

            (item, stop,error) = SolveIteration(matrixF.Evaluate(evaluate), vectorF.Evaluate(evaluate), item);

            ind++;
            if(!error) error = ind == 1000;
        }

        if (error) return (new List<(char, double)>(), SystemState.Error);

        return (BuildTuple(variables, item),SystemState.Correct);
    }

    private static (double[], bool,bool) SolveIteration(double[,] matrix, double[] vector, double[] item)
    {
        Matrix<double> m = Matrix<double>.Build.DenseOfArray(matrix);
        Vector<double> v = Vector<double>.Build.DenseOfArray(vector);

        Vector<double> sol = m.Solve(-v);

        foreach (var i in sol)
        {
            if (i is Double.NaN || i is Double.NegativeInfinity || i is Double.PositiveInfinity)
                return (item, true,true);
        }

        return ((sol + Vector<double>.Build.DenseOfArray(item)).ToArray(), sol.Norm(1) < 0.000001,false);
    }

    private static List<(char, double)> BuildTuple(List<char> variables, double[] values)
    {
        List<(char, double)> aux = new List<(char, double)>(values.Length);

        for (int i = 0; i < values.Length; i++) aux.Add((variables[i], values[i]));

        return aux;
    }
}