using Expression.Expressions;

namespace SystemEquationsLogic;

public class VectorFunction
{
    /// <summary>
    /// Vector de funciones
    /// </summary>
    private readonly ExpressionType<double>[] _vector;

    public VectorFunction(ExpressionType<double>[] exps)
    {
        _vector = exps;
    }

    /// <summary>
    /// Evaluar el vector de funciones
    /// </summary>
    /// <param name="variables">Variables con sus resoectivos valores</param>
    /// <returns>Vector</returns>
    public double[] Evaluate(List<(char, double)> variables)
    {
        double[] result = new double[_vector.Length];

        for (int i = 0; i < _vector.Length; i++) result[i] = _vector[i].Evaluate(variables);

        return result;
    }
}