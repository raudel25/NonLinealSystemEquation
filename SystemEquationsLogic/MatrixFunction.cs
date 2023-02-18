using Expression.Expressions;

namespace SystemEquationsLogic;

public class MatrixFunction
{
    /// <summary>
    /// Matriz Jacobiana
    /// </summary>
    private readonly ExpressionType<double>[,] _matrix;

    public MatrixFunction(ExpressionType<double>[] exps, List<char> variables)
    {
        _matrix = BuildMatrix(exps, variables);
    }

    /// <summary>
    /// Evaluar la matriz de funciones
    /// </summary>
    /// <param name="variables">Lista de variables con sus respectivos valores</param>
    /// <returns></returns>
    public double[,] Evaluate(List<(char, double)> variables)
    {
        double[,] result = new double[_matrix.GetLength(0), _matrix.GetLength(1)];

        for (int i = 0; i < _matrix.GetLength(0); i++)
        {
            for (int j = 0; j < _matrix.GetLength(1); j++) result[i, j] = _matrix[i, j].Evaluate(variables);
        }

        return result;
    }

    /// <summary>
    /// Crear la matriz Jacobiana
    /// </summary>
    /// <param name="exps">Expresions</param>
    /// <param name="variables">Variables</param>
    /// <returns>Matriz Jacoviana</returns>
    private ExpressionType<double>[,] BuildMatrix(ExpressionType<double>[] exps, List<char> variables)
    {
        ExpressionType<double>[,] matrix = new ExpressionType<double>[exps.Length, exps.Length];

        for (int i = 0; i < exps.Length; i++)
        {
            for (int j = 0; j < exps.Length; j++) matrix[i, j] = exps[i].Derivative(variables[j]);
        }

        return matrix;
    }
}