using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;

namespace WinFormsApp_0430
{
    
    public enum Operators { None, Add, Sub, Multi, Div, Percen, Fact };


    public partial class Form1 : Form
    {

        //public Operators Opt = Operators.None;
       // public double Value = 0.0;
        string expression = " ";
        //public double Num1 = 0;

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "0";
        }

        private void textBox1_TextChanged(object sender, EventArgs e){}
        /// <summary>
        /// 클릭 했을 때 화면에 숫자를 적는 함수 . 
        /// </summary>
        /// <param name="num">  </param>
        public void ClickFunc(double num)
        {
            
            if (textBox1.Text == "0")
            {
                textBox1.Text = num.ToString();
                expression = num.ToString(); 
            }
            else
            {
                textBox1.Text += num.ToString();
                expression += num.ToString();
            }

        }

        //클릭 했을 때의 동작은 위에 Click_func에 구현됨. 
        private void button_0_Click(object sender, EventArgs e) { ClickFunc(0); }
        private void button_1_Click(object sender, EventArgs e) { ClickFunc(1); }
        private void button_2_Click(object sender, EventArgs e) { ClickFunc(2); }
        private void button_3_Click(object sender, EventArgs e) { ClickFunc(3); }
        private void button_4_Click(object sender, EventArgs e) { ClickFunc(4); }
        private void button_5_Click(object sender, EventArgs e) { ClickFunc(5); }
        private void button_6_Click(object sender, EventArgs e) { ClickFunc(6); }
        private void button_7_Click(object sender, EventArgs e) { ClickFunc(7); }
        private void button_8_Click(object sender, EventArgs e) { ClickFunc(8); }
        private void button_9_Click(object sender, EventArgs e) { ClickFunc(9); }

        /// <summary>
        /// 소수점을 처리 하는 함수
        /// </summary>
        bool point = false;
        private void PointButton_Click(object sender, EventArgs e)
        {

            if (!point)
            {
                if (string.IsNullOrEmpty(textBox1.Text))// 비었을 경우 : 0.으로 세팅 
                {
                    textBox1.Text = "0.";
                }

                else
                {
                    textBox1.Text += ".";
                }
                point = true; // 한번 .이 찍히고 나면 더 이상 처리 못하는 역할. 

            }

        }

        // 연산자 버튼() 전부 호출해서 연산되게 구현
        private void PlusButton_Click(object sender, EventArgs e)
        {
            Operator_Click(Operators.Add);
        }

        private void SubButton_Click(object sender, EventArgs e)
        {
            Operator_Click(Operators.Sub);
        }

        private void MulButton_Click(object sender, EventArgs e)
        {
            Operator_Click(Operators.Multi);
        }

        private void DivButton_Click(object sender, EventArgs e)
        {
            Operator_Click(Operators.Div);
        }

        private void PrecentButton_Click(object sender, EventArgs e)
        {
            Operator_Click(Operators.Percen);
        }

        // 단항 연산이라 여러개의 연산자가 안들어가 Operator_Click 호출 안함. 
        private void FactorialButton_Click(object sender, EventArgs e)
        {
            expression += "!";
            textBox1.Text += "!";


        }

        /// <summary>
        /// = 을 눌렀을때 동작되는 함수 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EqualButton_Click(object sender, EventArgs e)
        {
            //double currentValue = double.Parse(textBox1.Text);
            
                double result = Evaluate_Expression(textBox1.Text);

                textBox1.Text = result.ToString();
                expression = result.ToString();

                
                point = false;
            
            
        }

        private double Evaluate_Expression(string expr)
        {

            if (expr.StartsWith("-"))// 0-3+5 방식 _이렇게 해야 음수 연산할때 에러 안남. 
            {
                expr = "0" + expr;
            }

            Stack<double> num_stack = new Stack<double>();
            Stack<char> op_stack = new Stack<char>();
            int i = 0;

            while (i < expr.Length)
            {
                if (char.IsWhiteSpace(expr[i]))
                {
                    i++;
                    continue;
                }

                if (char.IsDigit(expr[i]) || expr[i] == '.')
                {
                    string val = "";
                    while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
                    {
                        char current_value = expr[i];
                        val += current_value;
                        i++;
                    }

                    num_stack.Push(double.Parse(val));
                }
                else if (expr[i] == '(')
                {
                    op_stack.Push(expr[i]);
                    i++;
                }
                else if (expr[i] == ')')
                {
                    while (op_stack.Peek() != '(')
                        num_stack.Push(ApplyOp(op_stack.Pop(), num_stack.Pop(), num_stack.Pop()));
                    op_stack.Pop(); // '(' 제거
                    i++;
                }
                else if (IsOperator(expr[i]))
                {
                    while (op_stack.Count > 0 && Precedence(op_stack.Peek()) >= Precedence(expr[i]))
                        num_stack.Push(ApplyOp(op_stack.Pop(), num_stack.Pop(), num_stack.Pop()));
                    op_stack.Push(expr[i]);
                    i++;
                }
                else if (expr[i] == '!')
                {
                    
                    if (num_stack.Count == 0 && expr =="0")
                        return 0;

                    double val = num_stack.Pop();
                    double result = 1;
                    for (int j = 2; j <= (int)val; j++)
                        result *= j;

                    num_stack.Push(result);
                    i++; 
                }
                else if (expr[i] == '%')
                {
                    if (num_stack.Count == 0) 
                        return 0;

                    double val = num_stack.Pop();
                    num_stack.Push(val / 100.0); // 백분율 처리
                    i++; 
                }

            }

            while (op_stack.Count > 0)
                num_stack.Push(ApplyOp(op_stack.Pop(), num_stack.Pop(), num_stack.Pop()));

            return num_stack.Pop();
        }

        private bool IsOperator(char c)
        {
            if (c == '+'  || c == '-' )
            {
                return true;
            }
            else if( c == '/'|| c == '*' )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int Precedence(char op)
        {
            switch (op)
            {
                case '+':
                case '-':
                    return 1;

                case '*':
                case '/':
                case '%':
                    return 2;

                default:
                    return 0;
            }
        }

        private double ApplyOp(char op, double b, double a)
        {
            switch (op)
            {
                case '+':
                    return a + b;

                case '-':
                    return a - b;

                case '*':
                    return a * b;

                case '/':
                    if (b != 0)
                        return a / b;
                    else
                        return 0;
                case '%':
                    return a * b / 100;

                default:
                    return 0;

            }
        }
        

        private void Operator_Click(Operators newOp)
        {
            string opChar;

            switch (newOp)
            {
                case Operators.Add:
                    opChar = "+";
                    textBox1.Text += opChar;
                    expression += opChar;
                    break;

                case Operators.Sub:
                    opChar = "-";
                    textBox1.Text += opChar;
                    expression += opChar;
                    break;

                case Operators.Multi:
                    opChar = "*";
                    textBox1.Text += opChar;
                    expression += opChar;
                    break;

                case Operators.Div:
                    opChar = "/";
                    textBox1.Text += opChar;
                    expression += opChar;
                    break;

                case Operators.Percen:
                    opChar = "%";
                    textBox1.Text += opChar;
                    expression += opChar;
                    break;
                default:
                    break;
            }

           
        }






        /// <summary>
        /// 한개씩 삭제하는 함수 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelButton_Click(object sender, EventArgs e)
        {
            string tmp = textBox1.Text;

            if (string.IsNullOrEmpty(tmp))
            {
                textBox1.Text = tmp;
            }
            else
            {
                // 한개씩 잘라서 마지막 글자 제외하고 다시 문자열로 만들어서 화면에 쓰기 
                string[] str_arr = tmp.Select(c => c.ToString()).ToArray();

                str_arr = str_arr.Take(str_arr.Length - 1).ToArray();

                string result = string.Join("", str_arr);

                textBox1.Text = result;
            }
        }

        /// <summary>
        /// 전체를 삭제하는 함수 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            textBox1.Text = "0";
            point = false;

        }

        /// <summary>
        /// 현재 입력된 문자만 삭제 하는 함수 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearEntryButton_Click(object sender, EventArgs e)
        {
            string txt = textBox1.Text;

            char[] char_arr = { '+', '-', 'x', '/', '%', '!', '(', ')' };

            int idx = txt.Length - 1;

            //지울게 숫자인 경우.문자(연산자) 앞까지 지워야 함.  
            if (char.IsDigit(txt[idx]) || txt[idx] == '.')
            {
                while (idx >= 0 && (char.IsDigit(txt[idx]) || txt[idx] == '.'))
                    idx--;

                textBox1.Text = txt.Substring(0, idx + 1);
            }

            // 지울게 문자(연산자)인 경우._ 마지막 딱 하나만 지우면 됨. 
            else if (char_arr.Contains(txt[idx]))
            {
                idx--;
                textBox1.Text = txt.Substring(0, idx + 1);
            }

            if (string.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Text = "0";
            }
        }

        /// <summary>
        /// 역수로 바꿔주는 함수 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InverseButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                return;
            }

            double value = double.Parse(textBox1.Text);

            if (value == 0)
            {
                return;
            }

            double result = 1 / value;
            textBox1.Text = result.ToString();
        }

        /// <summary>
        /// 제곱함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SquareButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                return;
            }

            double value = double.Parse(textBox1.Text);
            double result = value * value;

            textBox1.Text = result.ToString();
        }

        /// <summary>
        /// root  함수. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                return;
            }

            double value = double.Parse(textBox1.Text);

            if (value < 0)
            {
                return;
            }
            double result = Math.Sqrt(value);
            textBox1.Text = result.ToString();
        }


        /// <summary>
        /// 부호 변환하는 함수 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pos_Neg_button_Click(object sender, EventArgs e)
        {
            double value = double.Parse(textBox1.Text);

            value = value * -1;

            textBox1.Text = value.ToString();
        }

      
        private void OpenParenButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "0")
            {
                textBox1.Text = "(";
                expression = "(";
            }
            else
            {
                textBox1.Text += "(";
                expression += "(";
            }
        }

     
        private void CloseParenButton_Click(object sender, EventArgs e)
        {
            expression += ")";
            textBox1.Text += ")";
        }

        
    }
}
