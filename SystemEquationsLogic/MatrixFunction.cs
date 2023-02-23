using Expression.Expressions;

namespace SystemEquationsLogic;

public class MatrixFunction
{
    /// <summary>
    ///     Matriz Jacobiana
    /// </summary>
    private readonly Function<double>[,] _matrix;

    public MatrixFunction(Function<double>[] exps, List<char> variables)
    {
        _matrix = BuildMatrix(exps, variables);
    }

    /// <summary>
    ///     Evaluar la matriz de funciones
    /// </summary>
    /// <param name="variables">Lista de variables con sus respectivos valores</param>
    /// <returns></returns>
    public double[,] Evaluate(List<(char, double)> variables)
    {
        var result = new double[_matrix.GetLength(0), _matrix.GetLength(1)];

        for (var i = 0; i < _matrix.GetLength(0); i++)
        for (var j = 0; j < _matrix.GetLength(1); j++)
            result[i, j] = _matrix[i, j].Evaluate(variables);

        return result;
    }

    /// <summary>
    ///     Crear la matriz Jacobiana
    /// </summary>
    /// <param name="exps">Expresions</param>
    /// <param name="variables">Variables</param>
    /// <returns>Matriz Jacoviana</returns>
    private Function<double>[,] BuildMatrix(Function<double>[] exps, List<char> variables)
    {
        var matrix = new Function<double>[exps.Length, exps.Length];

        for (var i = 0; i < exps.Length; i++)
        for (var j = 0; j < exps.Length; j++)
            matrix[i, j] = exps[i].Derivative(variables[j]);

        return matrix;
    }
}