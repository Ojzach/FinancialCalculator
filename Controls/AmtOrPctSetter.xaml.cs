using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinancialCalculator.Controls
{
    /// <summary>
    /// Interaction logic for AmtOrPctSetter.xaml
    /// </summary>
    public partial class AmtOrPctSetter : UserControl
    {


        public bool IsSetByAmt { get => (bool)GetValue(IsSetByAmtProperty); set => SetValue(IsSetByAmtProperty, value); }

        public float SetAmount { get => (float)GetValue(SetAmountProperty); set => SetValue(SetAmountProperty, value); }

        public float SetPercent { get => (float)GetValue(SetPercentProperty); set => SetValue(SetPercentProperty, value); }

        public string TitleText { get => (string)GetValue(TitleTextProperty); set => SetValue(TitleTextProperty, value); }


        public static readonly DependencyProperty IsSetByAmtProperty =
            DependencyProperty.Register(nameof(IsSetByAmt), typeof(bool), typeof(AmtOrPctSetter), new PropertyMetadata(false));

        public static readonly DependencyProperty SetAmountProperty =
            DependencyProperty.Register(nameof(SetAmount), typeof(float), typeof(AmtOrPctSetter), new PropertyMetadata(0.0f));

        public static readonly DependencyProperty SetPercentProperty =
            DependencyProperty.Register(nameof(SetPercent), typeof(float), typeof(AmtOrPctSetter), new PropertyMetadata(0.0f));

        public static readonly DependencyProperty TitleTextProperty =
            DependencyProperty.Register(nameof(TitleText), typeof(string), typeof(AmtOrPctSetter), new PropertyMetadata(""));



        public AmtOrPctSetter()
        {
            InitializeComponent();
        }
    }
}
