using Expression;
using NonLinealSystemEquation;

string[] s=new string[2];

s[0]="0=sin(x)+cos(y)";
s[1] ="x=y+1";


var a=SystemEquation.ResolveSystem(s);

foreach(var item in a) Console.WriteLine(item);

Console.WriteLine(Math.Sin(9.13));