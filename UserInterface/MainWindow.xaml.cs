using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using NonLinealSystemEquation;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private List<TextBox> _equationsValue;
        private List<Grid> _equations;
        private List<Button> _eliminateEquations;
        private List<TextBox> _initialValues;

        public MainWindow()
        {
            this._equationsValue = new List<TextBox>();
            this._equations = new List<Grid>();
            this._eliminateEquations = new List<Button>();
            this._initialValues = new List<TextBox>();

            InitializeComponent();
        }

        private void AddEquation(object sender, RoutedEventArgs e)
        {
            Initial.Visibility = Visibility.Collapsed;

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

        private void EliminateEquation(object sender, RoutedEventArgs e)
        {
            Initial.Visibility = Visibility.Collapsed;
            int ind = this._eliminateEquations.IndexOf((Button)sender);

            
            this._equations.RemoveAt(ind);
            this._eliminateEquations.RemoveAt(ind);
            this._equationsValue.RemoveAt(ind);
            
            for (int i = 0; i < this._equations.Count; i++) Grid.SetRow(this._equations[i], i);
            Equations.Children.RemoveAt(ind);
            Equations.RowDefinitions.RemoveAt(Equations.RowDefinitions.Count - 1);

        }

        private void Resolve(object sender, RoutedEventArgs e)
        {
            Initial.Visibility = Visibility.Collapsed;
            (List<(char, double)> result, SystemEquation.SystemState state) =
                SystemEquation.ResolveSystem(GetEquationValue(), GetInitialValue());

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

            Show.Text = answer;
            
            this._initialValues.Clear();
        }

        private void InitialValue(object sender, RoutedEventArgs e)
        {
            Initial.Visibility = Visibility.Visible;
            Initial.ColumnDefinitions.Clear();
            Initial.Children.Clear();
            this._initialValues.Clear();

            List<char> variables = SystemEquation.Variables(GetEquationValue());
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

            for (int i = 0; i < s.Length; i++) s[i] = this._equationsValue[i].Text;

            return s;
        }

        private double[] GetInitialValue()
        {
            double[] aux = new double[this._initialValues.Count];

            for (int i = 0; i < aux.Length; i++)
            {
                if (!double.TryParse(this._initialValues[i].Text, out aux[i])) return Array.Empty<double>();
            }

            return aux;
        }
    }
}