using Expression;
using NonLinealSystemEquation;

string[] s=new string[2];

s[0]="x=sin(y)";
s[1]="y=2+z";

ExpressionType[] e=ConvertEquation.ParsingSystem(s);

Console.WriteLine(e.Length);
Console.WriteLine(e[1]);