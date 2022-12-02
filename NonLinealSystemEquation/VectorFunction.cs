using Expression;

namespace NonLinealSystemEquation;

public class VectorFunction
{
    private ExpressionType[] _vector;

    public VectorFunction(ExpressionType[] exps)
    {
        _vector = exps;
    }

    public double[] Evaluate(List<(char, double)> variables)
    {
        double[] result = new double[_vector.Length];

        for (int i = 0; i < _vector.Length; i++) result[i] = _vector[i].Evaluate(variables);

        return result;
    }
}