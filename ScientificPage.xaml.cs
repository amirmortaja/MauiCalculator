using System.Globalization;

namespace MauiCalculator;

public partial class ScientificPage : ContentPage
{
    private double firstValOperand = 0;
    private double secondValOperand = 0;
    private string activeSciOperator = string.Empty;
    private bool clearOnNextInput = true;
    private double memoryCacheValue = 0;
    private bool isDegreeUnitActive = true;

    public ScientificPage()
    {
        InitializeComponent();
    }

    private void OnNumberClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            string digitStr = btn.Text;

            if (clearOnNextInput || SciDisplayLabel.Text == "0" || SciDisplayLabel.Text == "Geçersiz giriş" || SciDisplayLabel.Text == "Sıfıra bölünemez")
            {
                SciDisplayLabel.Text = digitStr;
                clearOnNextInput = false;
            }
            else
            {
                SciDisplayLabel.Text += digitStr;
            }
        }
    }

    private void OnDecimalClicked(object sender, EventArgs e)
    {
        if (clearOnNextInput)
        {
            SciDisplayLabel.Text = "0,";
            clearOnNextInput = false;
        }
        else if (!SciDisplayLabel.Text.Contains(","))
        {
            SciDisplayLabel.Text += ",";
        }
    }

    private void OnNegateClicked(object sender, EventArgs e)
    {
        if (double.TryParse(SciDisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
        {
            val = -val;
            SciDisplayLabel.Text = val.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
        }
    }

    private void OnAngleModeClicked(object sender, EventArgs e)
    {
        isDegreeUnitActive = !isDegreeUnitActive;
        AngleModeButton.Text = isDegreeUnitActive ? "DEG" : "RAD";
    }

    private async void OnTrigMenuClicked(object sender, EventArgs e)
    {
        string selectedAction = await DisplayActionSheet("Trigonometrik Fonksiyon", "İptal", null, "sin", "cos", "tan");
        if (selectedAction == "İptal" || string.IsNullOrEmpty(selectedAction)) return;

        if (double.TryParse(SciDisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
        {
            double angleVal = isDegreeUnitActive ? (val * Math.PI / 180.0) : val;
            double operationResult = 0;

            switch (selectedAction)
            {
                case "sin":
                    operationResult = Math.Sin(angleVal);
                    SciHistoryLabel.Text = $"sin({val})";
                    break;
                case "cos":
                    operationResult = Math.Cos(angleVal);
                    SciHistoryLabel.Text = $"cos({val})";
                    break;
                case "tan":
                    operationResult = Math.Tan(angleVal);
                    SciHistoryLabel.Text = $"tan({val})";
                    break;
            }

            SciDisplayLabel.Text = operationResult.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            clearOnNextInput = true;
        }
    }

    private void OnConstantClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            double constantVal = btn.Text == "π" ? Math.PI : Math.E;
            SciDisplayLabel.Text = constantVal.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            clearOnNextInput = true;
        }
    }

    private void OnOperatorClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            if (double.TryParse(SciDisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            {
                firstValOperand = val;
                activeSciOperator = btn.Text;
                SciHistoryLabel.Text = $"{SciDisplayLabel.Text} {activeSciOperator}";
                clearOnNextInput = true;
            }
        }
    }

    private void OnEqualClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(activeSciOperator))
            return;

        if (double.TryParse(SciDisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
        {
            secondValOperand = val;
            SciHistoryLabel.Text = $"{firstValOperand.ToString().Replace(".", ",")} {activeSciOperator} {secondValOperand.ToString().Replace(".", ",")} =";

            double finalResult = 0;
            switch (activeSciOperator)
            {
                case "+":
                    finalResult = firstValOperand + secondValOperand;
                    break;
                case "-":
                    finalResult = firstValOperand - secondValOperand;
                    break;
                case "×":
                    finalResult = firstValOperand * secondValOperand;
                    break;
                case "÷":
                    if (secondValOperand == 0)
                    {
                        SciDisplayLabel.Text = "Sıfıra bölünemez";
                        activeSciOperator = string.Empty;
                        clearOnNextInput = true;
                        return;
                    }
                    finalResult = firstValOperand / secondValOperand;
                    break;
                case "mod":
                    if (secondValOperand == 0)
                    {
                        SciDisplayLabel.Text = "Sıfıra bölünemez";
                        activeSciOperator = string.Empty;
                        clearOnNextInput = true;
                        return;
                    }
                    finalResult = firstValOperand % secondValOperand;
                    break;
                case "xʸ":
                    finalResult = Math.Pow(firstValOperand, secondValOperand);
                    break;
            }

            SciDisplayLabel.Text = finalResult.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            firstValOperand = finalResult;
            activeSciOperator = string.Empty;
            clearOnNextInput = true;
        }
    }

    private void OnSpecialOpClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && double.TryParse(SciDisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
        {
            double resValue = 0;
            string opName = btn.Text;

            switch (opName)
            {
                case "log":
                    if (val <= 0) { SciDisplayLabel.Text = "Geçersiz giriş"; clearOnNextInput = true; return; }
                    resValue = Math.Log10(val);
                    SciHistoryLabel.Text = $"log({val})";
                    break;
                case "ln":
                    if (val <= 0) { SciDisplayLabel.Text = "Geçersiz giriş"; clearOnNextInput = true; return; }
                    resValue = Math.Log(val);
                    SciHistoryLabel.Text = $"ln({val})";
                    break;
                case "√x":
                    if (val < 0) { SciDisplayLabel.Text = "Geçersiz giriş"; clearOnNextInput = true; return; }
                    resValue = Math.Sqrt(val);
                    SciHistoryLabel.Text = $"√({val})";
                    break;
                case "x²":
                    resValue = Math.Pow(val, 2);
                    SciHistoryLabel.Text = $"sqr({val})";
                    break;
                case "1/x":
                    if (val == 0) { SciDisplayLabel.Text = "Sıfıra bölünemez"; clearOnNextInput = true; return; }
                    resValue = 1 / val;
                    SciHistoryLabel.Text = $"1/({val})";
                    break;
                case "|x|":
                    resValue = Math.Abs(val);
                    SciHistoryLabel.Text = $"abs({val})";
                    break;
                case "10ˣ":
                    resValue = Math.Pow(10, val);
                    SciHistoryLabel.Text = $"10^({val})";
                    break;
                case "n!":
                    if (val < 0 || val != Math.Floor(val) || val > 170) { SciDisplayLabel.Text = "Geçersiz giriş"; clearOnNextInput = true; return; }
                    resValue = ComputeFactorial((int)val);
                    SciHistoryLabel.Text = $"fact({val})";
                    break;
            }

            SciDisplayLabel.Text = resValue.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            clearOnNextInput = true;
        }
    }

    private double ComputeFactorial(int n)
    {
        if (n == 0 || n == 1) return 1;
        double resultVal = 1;
        for (int i = 2; i <= n; i++)
        {
            resultVal *= i;
        }
        return resultVal;
    }

    private void OnParenthesisClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            SciHistoryLabel.Text += btn.Text;
        }
    }

    private void OnFunctionClicked(object sender, EventArgs e) { }

    private void OnClearClicked(object sender, EventArgs e)
    {
        firstValOperand = 0;
        secondValOperand = 0;
        activeSciOperator = string.Empty;
        SciHistoryLabel.Text = "";
        SciDisplayLabel.Text = "0";
        clearOnNextInput = true;
    }

    private void OnBackspaceClicked(object sender, EventArgs e)
    {
        if (clearOnNextInput || SciDisplayLabel.Text.Length <= 1 || SciDisplayLabel.Text == "Sıfıra bölünemez" || SciDisplayLabel.Text == "Geçersiz giriş")
        {
            SciDisplayLabel.Text = "0";
            clearOnNextInput = true;
        }
        else
        {
            SciDisplayLabel.Text = SciDisplayLabel.Text.Substring(0, SciDisplayLabel.Text.Length - 1);
        }
    }

    private void OnMemoryClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && double.TryParse(SciDisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
        {
            switch (btn.Text)
            {
                case "MC": memoryCacheValue = 0; break;
                case "MR": SciDisplayLabel.Text = memoryCacheValue.ToString(CultureInfo.InvariantCulture).Replace(".", ","); clearOnNextInput = true; break;
                case "M+": memoryCacheValue += val; clearOnNextInput = true; break;
                case "M-": memoryCacheValue -= val; clearOnNextInput = true; break;
                case "MS": memoryCacheValue = val; clearOnNextInput = true; break;
            }
        }
    }
}
