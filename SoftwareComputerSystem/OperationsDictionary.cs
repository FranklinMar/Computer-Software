using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareComputerSystem
{
    public class OperationsDictionary
    {
        private int Add { get; set; }
        private int Subtract { get; set; }
        private int Multiply { get; set; }
        private int Divide { get; set; }

        public OperationsDictionary(int add, int subtract, int multiply, int divide)
        {
            if (add <= 0 || subtract <= 0 || multiply <= 0 || divide <= 0)
            {
                throw new ArgumentException("All operation values must be greater than 0.");
            }

            Add = add;
            Subtract = subtract;
            Multiply = multiply;
            Divide = divide;
        }

        public int this[string operatorSymbol]
        {
            get
            {
                return operatorSymbol switch
                {
                    "+" => Add,
                    "-" => Subtract,
                    "*" => Multiply,
                    "/" => Divide,
                    _ => throw new ArgumentException("Invalid operator."),
                };
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("All operation values must be greater than 0.");
                }
            }
        }
    }
}
