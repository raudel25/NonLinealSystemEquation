using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Expression.Arithmetics;
using SystemEquationsLogic;

namespace UserInterfaceAUI;

public partial class MainWindow : Window
{
    private readonly ArithmeticExp<double> _arithmetic;
    private readonly List<Button> _eliminateEquations;
    private readonly List<Grid> _equations;
    private readonly List<TextBox> _equationsValue;
    private readonly List<TextBox> _initialValues;

    public MainWindow()
    {
        _equationsValue = new List<TextBox>();
        _equations = new List<Grid>();
        _eliminateEquations = new List<Button>();
        _initialValues = new List<TextBox>();
        _arithmetic = new ArithmeticExp<double>(new NativeExp());

        InitializeComponent();
    }

    private void AddEquation(object? sender, RoutedEventArgs e)
    {
        Initial.IsVisible = false;

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        grid.ColumnDefinitions[1].Width = new GridLength(30);

        var text = new TextBox();
        var button = new Button();
        text.TextAlignment = TextAlignment.Center;
        text.VerticalContentAlignment = VerticalAlignment.Center;
        text.FontSize = 16;

        Grid.SetColumn(text, 0);
        Grid.SetColumn(button, 1);
        button.Click += EliminateEquation;
        button.Height = 30;
        button.Width = 30;
        button.HorizontalContentAlignment = HorizontalAlignment.Center;
        button.Background = Brushes.Azure;
        button.Content = "X";

        _eliminateEquations.Add(button);
        _equationsValue.Add(text);
        _equations.Add(grid);

        grid.Children.Add(text);
        grid.Children.Add(button);
        grid.Margin = new Thickness(5);

        Grid.SetRow(grid, Equations.RowDefinitions.Count);
        Equations.RowDefinitions.Add(new RowDefinition());
        Equations.RowDefinitions[^1].Height = new GridLength(40);

        Equations.Children.Add(grid);
    }

    private void EliminateEquation(object? sender, RoutedEventArgs e)
    {
        if (sender is null) return;

        Initial.IsVisible = false;
        var ind = _eliminateEquations.IndexOf((Button)sender);


        _equations.RemoveAt(ind);
        _eliminateEquations.RemoveAt(ind);
        _equationsValue.RemoveAt(ind);

        for (var i = 0; i < _equations.Count; i++) Grid.SetRow(_equations[i], i);
        Equations.Children.RemoveAt(ind);
        Equations.RowDefinitions.RemoveAt(Equations.RowDefinitions.Count - 1);
    }

    private void Resolve(object? sender, RoutedEventArgs e)
    {
        Initial.IsVisible = false;
        var (result, state) =
            SystemEquation.ResolveSystem(GetEquationValue(), GetInitialValue(), _arithmetic);

        string answer;

        if (state == SystemEquation.SystemState.Correct)
        {
            answer = "\n";
            foreach (var item in result) answer = $"{answer}{item.Item1} = {item.Item2}\n";
        }
        else
        {
            answer = state == SystemEquation.SystemState.IncorrectEquations
                ? "Las ecuaciones no son válidas"
                : "No se ha podido encontrar solución";
        }

        Result.Text = answer;

        _initialValues.Clear();
    }

    private void InitialValue(object? sender, RoutedEventArgs e)
    {
        Initial.IsVisible = true;
        Initial.ColumnDefinitions.Clear();
        Initial.Children.Clear();
        _initialValues.Clear();

        var variables = SystemEquation.Variables(GetEquationValue(), _arithmetic);
        var grids = new List<Grid>(variables.Count);

        foreach (var item in variables)
        {
            var label = new TextBlock();
            var text = new TextBox();
            label.Text = $"{item} = ";
            label.Width = 25;
            label.VerticalAlignment = VerticalAlignment.Center;
            text.VerticalContentAlignment = VerticalAlignment.Center;
            text.TextAlignment = TextAlignment.Center;
            text.Width = 25;

            var aux = new Grid
            {
                Height = 25,
                Margin = new Thickness(2)
            };
            aux.ColumnDefinitions.Add(new ColumnDefinition());
            aux.ColumnDefinitions.Add(new ColumnDefinition());

            Grid.SetColumn(label, 0);
            Grid.SetColumn(text, 1);
            aux.Children.Add(label);
            aux.Children.Add(text);
            _initialValues.Add(text);

            grids.Add(aux);
        }

        for (var i = 0; i < grids.Count; i++)
        {
            Initial.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(grids[i], i);
            Initial.Children.Add(grids[i]);
        }
    }

    private string[] GetEquationValue()
    {
        var s = new string[_equationsValue.Count];

        for (var i = 0; i < s.Length; i++)
            s[i] = _equationsValue[i].Text is null ? "" : _equationsValue[i].Text;

        return s;
    }

    private double[] GetInitialValue()
    {
        var initial = _initialValues.Count != 0;
        var aux = new double[_initialValues.Count];

        for (var i = 0; i < aux.Length; i++)
            if (!double.TryParse(_initialValues[i].Text, out aux[i]))
                initial = false;

        if (!initial)
        {
            aux = new double[_equationsValue.Count];
            for (var i = 0; i < _equationsValue.Count; i++) aux[i] = 1;
        }

        return aux;
    }
}