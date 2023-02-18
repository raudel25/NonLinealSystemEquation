using Avalonia.Controls;
using SystemEquationsLogic;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Expression.Arithmetics;

namespace UserInterfaceAUI;

public partial class MainWindow : Window
{
    private readonly List<TextBox> _equationsValue;
    private readonly List<Grid> _equations;
    private readonly List<Button> _eliminateEquations;
    private readonly List<TextBox> _initialValues;
    private readonly ArithmeticExp<double> _arithmetic;

    public MainWindow()
    {
        this._equationsValue = new List<TextBox>();
        this._equations = new List<Grid>();
        this._eliminateEquations = new List<Button>();
        this._initialValues = new List<TextBox>();
        this._arithmetic = new ArithmeticExp<double>(new NativeExp());

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

        this._eliminateEquations.Add(button);
        this._equationsValue.Add(text);
        this._equations.Add(grid);

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
        int ind = this._eliminateEquations.IndexOf((Button)sender);


        this._equations.RemoveAt(ind);
        this._eliminateEquations.RemoveAt(ind);
        this._equationsValue.RemoveAt(ind);

        for (int i = 0; i < this._equations.Count; i++) Grid.SetRow(this._equations[i], i);
        Equations.Children.RemoveAt(ind);
        Equations.RowDefinitions.RemoveAt(Equations.RowDefinitions.Count - 1);
    }

    private void Resolve(object? sender, RoutedEventArgs e)
    {
        Initial.IsVisible = false;
        (List<(char, double)> result, SystemEquation.SystemState state) =
            SystemEquation.ResolveSystem(GetEquationValue(), GetInitialValue(), _arithmetic);

        string answer;

        if (state == SystemEquation.SystemState.Correct)
        {
            answer = "\n";
            foreach (var item in result) answer = $"{answer}{item.Item1} = {item.Item2}\n";
        }
        else
            answer = state == SystemEquation.SystemState.IncorrectEquations
                ? "Las ecuaciones no son válidas"
                : "No se ha podido encontrar solución";

        Result.Text = answer;

        this._initialValues.Clear();
    }

    private void InitialValue(object? sender, RoutedEventArgs e)
    {
        Initial.IsVisible = true;
        Initial.ColumnDefinitions.Clear();
        Initial.Children.Clear();
        this._initialValues.Clear();

        List<char> variables = SystemEquation.Variables(GetEquationValue(), _arithmetic);
        List<Grid> grids = new List<Grid>(variables.Count);

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

            var aux = new Grid();
            aux.Height = 25;
            aux.Margin = new Thickness(2);
            aux.ColumnDefinitions.Add(new ColumnDefinition());
            aux.ColumnDefinitions.Add(new ColumnDefinition());

            Grid.SetColumn(label, 0);
            Grid.SetColumn(text, 1);
            aux.Children.Add(label);
            aux.Children.Add(text);
            this._initialValues.Add(text);

            grids.Add(aux);
        }

        for (int i = 0; i < grids.Count; i++)
        {
            Initial.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(grids[i], i);
            Initial.Children.Add(grids[i]);
        }
    }

    private string[] GetEquationValue()
    {
        string[] s = new string[this._equationsValue.Count];

        for (int i = 0; i < s.Length; i++)
            s[i] = this._equationsValue[i].Text is null ? "" : this._equationsValue[i].Text;

        return s;
    }

    private double[] GetInitialValue()
    {
        bool initial = this._initialValues.Count != 0;
        double[] aux = new double[this._initialValues.Count];

        for (int i = 0; i < aux.Length; i++)
        {
            if (!double.TryParse(this._initialValues[i].Text, out aux[i])) initial = false;
        }

        if (!initial)
        {
            aux = new double[this._equationsValue.Count];
            for (int i = 0; i < this._equationsValue.Count; i++) aux[i] = 1;
        }

        return aux;
    }
}