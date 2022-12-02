namespace Expression;

public class NumberExpression : ExpressionType
{
    public readonly double Value;

    public NumberExpression(double value)
    {
        this.Value = value;
    }

    public override ExpressionType Derivative(char variable) => new NumberExpression(0);

    public override double Evaluate(List<(char, double)> variables) => this.Value;

    public override ExpressionType EvaluateExpression(List<(char, ExpressionType)> variables) => this;

    public override string ToString() => this.Value.ToString();

    public override int Priority => 6;

    public override bool Equals(object? obj)
    {
        NumberExpression? exp = obj as NumberExpression;
        if (exp is null) return false;

        return exp.Value == this.Value;
    }

    public override int GetHashCode() => this.Value.GetHashCode();
}

public class VariableExpression : ExpressionType
{
    public readonly char Variable;

    public VariableExpression(char variable)
    {
        this.Variable = variable;
    }

    public override ExpressionType Derivative(char variable) => variable == this.Variable
        ? new NumberExpression(1)
        : new NumberExpression(0);

    public override double Evaluate(List<(char, double)> variables)
    {
        foreach (var item in variables)
        {
            if (item.Item1 == this.Variable) return item.Item2;
        }

        throw new Exception("No se ha introducido un valor para cada variable");
    }

    public override ExpressionType EvaluateExpression(List<(char, ExpressionType)> variables)
    {
        foreach (var item in variables)
        {
            if (item.Item1 == this.Variable) return item.Item2;
        }

        return this;
    }

    public override int Priority => 6;

    public override bool Equals(object? obj)
    {
        VariableExpression? exp = obj as VariableExpression;
        if (exp is null) return false;

        return exp.Variable == this.Variable;
    }

    public override int GetHashCode() => this.Variable.GetHashCode();

    public override string ToString() => this.Variable.ToString();
}

public class ConstantE : ExpressionType
{
    public override ExpressionType Derivative(char variable) => new NumberExpression(0);

    public override double Evaluate(List<(char, double)> variables) => Math.E;

    public override ExpressionType EvaluateExpression(List<(char, ExpressionType)> variables) => this;

    public override int Priority => 6;

    public override string ToString() => "e";

    public override bool Equals(object? obj) => obj is ConstantE;

    public override int GetHashCode() => (int)Math.E;
}

public class ConstantPI : ExpressionType
{
    public override ExpressionType Derivative(char variable) => new NumberExpression(0);

    public override double Evaluate(List<(char, double)> variables) => Math.PI;

    public override ExpressionType EvaluateExpression(List<(char, ExpressionType)> variables) => this;

    public override int Priority => 6;

    public override string ToString() => "pi";

    public override bool Equals(object? obj) => obj is ConstantPI;

    public override int GetHashCode() => (int)Math.PI;
}

public class Factorial : ExpressionType
{
    private readonly int _integer;

    private long _value;

    public long Value
    {
        get
        {
            if (_value == -1) _value = Aux.Factorial(_integer);

            return _value;
        }
        set
        {
            if (_value == -1) _value = value;
        }
    }

    public Factorial(int value)
    {
        this._integer = value;
        this._value = -1;
    }

    public override ExpressionType Derivative(char variable) => new NumberExpression(0);

    public override double Evaluate(List<(char, double)> variables) => this.Value;

    public override ExpressionType EvaluateExpression(List<(char, ExpressionType)> variables) => this;

    public override string ToString() => $"{this._integer}!";

    public override int Priority
    {
        get => 5;
    }

    public override bool Equals(object? obj)
    {
        NumberExpression? exp = obj as NumberExpression;
        if (exp is null) return false;

        return exp.Value == this.Value;
    }

    public override int GetHashCode() => this.Value.GetHashCode();
}