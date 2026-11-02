using System.Globalization;

namespace MauiCalculator;

public partial class StandardPage : ContentPage
{
    private double primaryOperand = 0;
    private double secondaryOperand = 0;
    private string activeOperator = string.Empty;
    private bool resetScreenOnInput = true;
    private double storedMemoryValue = 0;

    public StandardPage()
    {
        InitializeComponent();
    }

    private void OnNumberClicked(object sender, EventArgs e)
    {
        if (sender is Button actionButton)
        {
            string numberValue = actionButton.Text;

            if (resetScreenOnInput || DisplayLabel.Text == "0" || DisplayLabel.Text == "Sıfıra bölünemez")
            {
                DisplayLabel.Text = numberValue;
                resetScreenOnInput = false;
            }
            else
            {
                DisplayLabel.Text += numberValue;
            }
        }
    }

    private void OnDecimalClicked(object sender, EventArgs e)
    {
        if (resetScreenOnInput)
        {
            DisplayLabel.Text = "0,";
            resetScreenOnInput = false;
        }
        else if (!DisplayLabel.Text.Contains(","))
        {
            DisplayLabel.Text += ",";
        }
    }

    private void OnNegateClicked(object sender, EventArgs e)
    {
        if (double.TryParse(DisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedVal))
        {
            parsedVal = -parsedVal;
            DisplayLabel.Text = parsedVal.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
        }
    }

    private void OnOperatorClicked(object sender, EventArgs e)
    {
        if (sender is Button actionButton)
        {
            if (double.TryParse(DisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedVal))
            {
                primaryOperand = parsedVal;
                activeOperator = actionButton.Text;
                HistoryLabel.Text = $"{DisplayLabel.Text} {activeOperator}";
                resetScreenOnInput = true;
            }
        }
    }

    private void OnEqualClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(activeOperator))
            return;

        if (double.TryParse(DisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedVal))
        {
            secondaryOperand = parsedVal;
            HistoryLabel.Text = $"{primaryOperand.ToString().Replace(".", ",")} {activeOperator} {secondaryOperand.ToString().Replace(".", ",")} =";

            double computationResult = 0;
            switch (activeOperator)
            {
                case "+":
                    computationResult = primaryOperand + secondaryOperand;
                    break;
                case "-":
                    computationResult = primaryOperand - secondaryOperand;
                    break;
                case "×":
                    computationResult = primaryOperand * secondaryOperand;
                    break;
                case "÷":
                    if (secondaryOperand == 0)
                    {
                        DisplayLabel.Text = "Sıfıra bölünemez";
                        activeOperator = string.Empty;
                        resetScreenOnInput = true;
                        return;
                    }
                    computationResult = primaryOperand / secondaryOperand;
                    break;
            }

            DisplayLabel.Text = computationResult.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            primaryOperand = computationResult;
            activeOperator = string.Empty;
            resetScreenOnInput = true;
        }
    }

    private void OnSpecialOpClicked(object sender, EventArgs e)
    {
        if (sender is Button actionButton && double.TryParse(DisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedVal))
        {
            double computationResult = 0;
            string operationType = actionButton.Text;

            switch (operationType)
            {
                case "1/x":
                    if (parsedVal == 0)
                    {
                        DisplayLabel.Text = "Sıfıra bölünemez";
                        resetScreenOnInput = true;
                        return;
                    }
                    computationResult = 1 / parsedVal;
                    HistoryLabel.Text = $"1/({parsedVal})";
                    break;
                case "x²":
                    computationResult = Math.Pow(parsedVal, 2);
                    HistoryLabel.Text = $"sqr({parsedVal})";
                    break;
                case "√x":
                    if (parsedVal < 0)
                    {
                        DisplayLabel.Text = "Geçersiz giriş";
                        resetScreenOnInput = true;
                        return;
                    }
                    computationResult = Math.Sqrt(parsedVal);
                    HistoryLabel.Text = $"√({parsedVal})";
                    break;
                case "%":
                    computationResult = (primaryOperand != 0) ? (primaryOperand * parsedVal / 100.0) : (parsedVal / 100.0);
                    HistoryLabel.Text = computationResult.ToString();
                    break;
            }

            DisplayLabel.Text = computationResult.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
            resetScreenOnInput = true;
        }
    }

    private void OnClearEntryClicked(object sender, EventArgs e)
    {
        DisplayLabel.Text = "0";
        resetScreenOnInput = true;
    }

    private void OnClearClicked(object sender, EventArgs e)
    {
        primaryOperand = 0;
        secondaryOperand = 0;
        activeOperator = string.Empty;
        HistoryLabel.Text = "";
        DisplayLabel.Text = "0";
        resetScreenOnInput = true;
    }

    private void OnBackspaceClicked(object sender, EventArgs e)
    {
        if (resetScreenOnInput || DisplayLabel.Text.Length <= 1 || DisplayLabel.Text == "Sıfıra bölünemez" || DisplayLabel.Text == "Geçersiz giriş")
        {
            DisplayLabel.Text = "0";
            resetScreenOnInput = true;
        }
        else
        {
            DisplayLabel.Text = DisplayLabel.Text.Substring(0, DisplayLabel.Text.Length - 1);
        }
    }

    private void OnMemoryClicked(object sender, EventArgs e)
    {
        if (sender is Button memoryButton && double.TryParse(DisplayLabel.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedVal))
        {
            switch (memoryButton.Text)
            {
                case "MC":
                    storedMemoryValue = 0;
                    break;
                case "MR":
                    DisplayLabel.Text = storedMemoryValue.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
                    resetScreenOnInput = true;
                    break;
                case "M+":
                    storedMemoryValue += parsedVal;
                    resetScreenOnInput = true;
                    break;
                case "M-":
                    storedMemoryValue -= parsedVal;
                    resetScreenOnInput = true;
                    break;
                case "MS":
                    storedMemoryValue = parsedVal;
                    resetScreenOnInput = true;
                    break;
            }
        }
    }
}
