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


        public MainWindow()
        {
            this._equationsValue = new List<TextBox>();
            this._equations = new List<Grid>();
            this._eliminateEquations = new List<Button>();
            InitializeComponent();
        }

        private void AddEquation(object sender, RoutedEventArgs e)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions[0].Width = new GridLength(200);
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

            Grid.SetRow(grid, Equations.Children.Count);
            Equations.RowDefinitions.Add(new RowDefinition());
            Equations.RowDefinitions[^1].Height = new GridLength(40);

            Equations.Children.Add(grid);
        }

        private void EliminateEquation(object sender, RoutedEventArgs e)
        {
            int ind = this._eliminateEquations.IndexOf((Button)sender);
            this._equations[ind].Visibility = Visibility.Collapsed;

            this._equations.RemoveAt(ind);
            this._eliminateEquations.RemoveAt(ind);
            this._equationsValue.RemoveAt(ind);

            for (int i = 0; i < this._equations.Count; i++) Grid.SetRow(this._equations[i], i);
            Equations.RowDefinitions.RemoveAt(Equations.RowDefinitions.Count - 1);
        }

        private void Resolve(object sender, RoutedEventArgs e)
        {
            string[] s = new string[this._equationsValue.Count];

            for (int i = 0; i < s.Length; i++) s[i] = this._equationsValue[i].Text;

            (List<(char, double)> result,SystemEquation.SystemState state) = SystemEquation.ResolveSystem(s);

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
        }
    }
}