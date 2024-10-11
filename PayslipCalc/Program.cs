using static PayslipCalc.SalaryCalculate;

namespace PayslipCalc
{
    public class Payslip
    {
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal AdvanceSalary { get; set; }
        public decimal IRRF { get; set; }
        public decimal INSS { get; set; }
        public bool ReceiveAdvance { get; set; }
    }

    public class SalaryCalculate
    {
        private const decimal FIRST_INSS_LIMIT = 1412m;
        private const decimal SECOND_INSS_LIMIT = 2666.68m;
        private const decimal THIRD_INSS_LIMIT = 4000.03m;
        private const decimal MAX_INSS_LIMIT = 7786.02m;

        private const decimal FIRST_INSS_RATE = 0.075m;
        private const decimal SECOND_INSS_RATE = 0.09m;
        private const decimal THIRD_INSS_RATE = 0.12m;
        private const decimal MAX_INSS_RATE = 0.14m;

        private const decimal FIRST_IR_LIMIT = 2112.00m;
        private const decimal SECOND_IR_LIMIT = 2826.65m;
        private const decimal THIRD_IR_LIMIT = 3751.05m;
        private const decimal FOURTH_IR_LIMIT = 4664.68m;

        private const decimal FIRST_IR_RATE = 0.075m;
        private const decimal SECOND_IR_RATE = 0.15m;
        private const decimal THIRD_IR_RATE = 0.225m;
        private const decimal MAX_IR_RATE = 0.275m;

        private const decimal FIRST_IR_DEDUCTION = 169.44m;
        private const decimal SECOND_IR_DEDUCTION = 381.44m;
        private const decimal THIRD_IR_DEDUCTION = 662.77m;
        private const decimal MAX_IR_DEDUCTION = 896m;

        public void CalculateINSS(Payslip pay)
        {
            decimal[] limits = { FIRST_INSS_LIMIT, SECOND_INSS_LIMIT, THIRD_INSS_LIMIT, MAX_INSS_LIMIT };
            decimal[] rates = { FIRST_INSS_RATE, SECOND_INSS_RATE, THIRD_INSS_RATE, MAX_INSS_RATE };
            decimal inss = 0;
            decimal baseSalary = pay.GrossSalary;

            for (int i = 0; i < limits.Length; i++)
            {
                decimal lowerLimit = i > 0 ? limits[i - 1] : 0;

                if (baseSalary > limits[i])
                {
                    decimal taxableAmount = limits[i] - lowerLimit;
                    inss += taxableAmount * rates[i];
                }
                else
                {
                    inss += (baseSalary - lowerLimit) * rates[i];
                    break;
                }
            }
            pay.INSS = inss;
        }

        public void CalculateIR(Payslip pay)
        {
            decimal taxableSalary = pay.GrossSalary - pay.INSS;

            if (taxableSalary <= FIRST_IR_LIMIT)
            {
                pay.IRRF = 0;
            }
            else if (taxableSalary <= SECOND_IR_LIMIT)
            {
                pay.IRRF = taxableSalary * FIRST_IR_RATE - FIRST_IR_DEDUCTION;
            }
            else if (taxableSalary <= THIRD_IR_LIMIT)
            {
                pay.IRRF = taxableSalary * SECOND_IR_RATE - SECOND_IR_DEDUCTION;
            }
            else if (taxableSalary <= FOURTH_IR_LIMIT)
            {
                pay.IRRF = taxableSalary * THIRD_IR_RATE - THIRD_IR_DEDUCTION;
            }
            else
            {
                pay.IRRF = taxableSalary * MAX_IR_RATE - MAX_IR_DEDUCTION;
            }
        }

        public void CalculateNetSalary(Payslip pay)
        {
            CalculateINSS(pay);
            CalculateIR(pay);
            if (pay.ReceiveAdvance)
            {
                pay.AdvanceSalary = pay.GrossSalary * 0.4m;
                pay.NetSalary = pay.GrossSalary - pay.INSS - pay.IRRF - pay.AdvanceSalary;
            }
            else
            {
                pay.NetSalary = pay.GrossSalary - pay.INSS - pay.IRRF;
            }
            
            
        }

        public class SalaryPresenter
        {
            public void DisplayPayslip(Payslip pay)
            {
                Console.WriteLine("\nNet Salary: " + pay.NetSalary.ToString("F2"));
                if (pay.ReceiveAdvance) 
                { 
                    Console.WriteLine("Advance Salary: " + pay.AdvanceSalary.ToString("F2")); 
                }
                Console.WriteLine("INSS: " + pay.INSS.ToString("F2"));
                Console.WriteLine("IRRF: " + pay.IRRF.ToString("F2"));
            }
        }

    }

    public class Program
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();

                Payslip pay = new Payslip();
                SalaryCalculate calculator = new SalaryCalculate();
                SalaryPresenter presenter = new SalaryPresenter();

                pay.GrossSalary = GetValidSalary();
                pay.ReceiveAdvance = GetUserReceiveAdvance();
                calculator.CalculateNetSalary(pay);
                presenter.DisplayPayslip(pay);

                Console.WriteLine("\nDo you want to reload the application or exit? (Enter R to Reload or any other key to exit))");
                string key = Console.ReadLine().ToUpper();

                if (key != "R")
                {
                    Console.WriteLine("Closing app...");
                    Thread.Sleep(1300);
                    Environment.Exit(0);
                }
            }
        }

        private static decimal GetValidSalary()
        {
            decimal grossSal;
            
            while (true)
            {
                Console.WriteLine("Enter your salary: (0000.00)");
                if (decimal.TryParse(Console.ReadLine(), out grossSal) && grossSal > 0)
                    return grossSal;
                else
                    Console.WriteLine("Invalid salary, please try again.");
            }
        }

        private static bool GetUserReceiveAdvance()
        {
            string input;

            while (true)
            {
                Console.WriteLine("Do you receive salary advance? (y/N)");
                input = Console.ReadLine().ToUpper().Trim();

                if (input == "Y")
                {
                    return true;
                }
                else if (input == "N")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 'y' or 'N'.");
                }
            }
        }
    }
}