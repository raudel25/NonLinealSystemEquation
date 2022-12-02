using Expression;

namespace NonLinealSystemEquation;

public class MatrixFunction
{
    private readonly ExpressionType[,] _matrix;

    public MatrixFunction(ExpressionType[] exps, List<char> variables)
    {
        _matrix = BuildMatrix(exps, variables);
    }

    public double[,] Evaluate(List<(char, double)> variables)
    {
        double[,] result = new double[_matrix.GetLength(0), _matrix.GetLength(1)];

        for (int i = 0; i < _matrix.GetLength(0); i++)
        {
            for (int j = 0; j < _matrix.GetLength(1); j++) result[i, j] = _matrix[i, j].Evaluate(variables);
        }

        return result;
    }

    private ExpressionType[,] BuildMatrix(ExpressionType[] exps, List<char> variables)
    {
        ExpressionType[,] matrix = new ExpressionType[exps.Length, exps.Length];

        for (int i = 0; i < exps.Length; i++)
        {
            for (int j = 0; j < exps.Length; j++) matrix[i, j] = exps[i].Derivative(variables[j]);
        }

        return matrix;
    }
}