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
using System.Security.Cryptography;
using System.Numerics;

namespace DigitalSignatureByRSAIvanovAG
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBoxInputP.Text = GenerateBigInt(BigInteger.Parse(TextBoxIntervalB.Text), Convert.ToInt32(TextBoxIntervalA.Text)).ToString();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TextBoxInputQ.Text = GenerateBigInt(BigInteger.Parse(TextBoxIntervalB.Text), Convert.ToInt32(TextBoxIntervalA.Text)).ToString();
        }
        private BigInteger GenerateBigInt(BigInteger Upper, int acc)
        {
            BigInteger result = RandomIntegerBelow(BigInteger.Pow(10, (int)Upper));
            while(MillerRabinTest(result, acc ) == false)
            {
                result++;
            }
            return result;
        }
        private BigInteger RandomIntegerBelow(BigInteger N)
        {
            byte[] bytes = N.ToByteArray();
            BigInteger R;
            Random random = new Random();
            do
            {
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F; //force sign bit to positive
                R = new BigInteger(bytes);
            } while (R >= N);
            return R;
        }

        private bool MillerRabinTest(BigInteger n, int k)
        {
            if (n == 2 || n == 3)
                return true;
            if (n < 2 || n % 2 == 0)
                return false;
            BigInteger t = n - 1;
            int s = 0;
            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }

            for (int i = 0; i < k; i++)
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] _a = new byte[n.ToByteArray().LongLength];
                BigInteger a;
                do
                {
                    rng.GetBytes(_a);
                    a = new BigInteger(_a);
                }
                while (a < 2 || a >= n - 2);
                BigInteger x = BigInteger.ModPow(a, t, n);
                if (x == 1 || x == n - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                        break;
                }
                if (x != n - 1)
                    return false;
            }
            return true;
        }

        public static BigInteger Sqrt(BigInteger n)
        {
            BigInteger result = 0;
            if (n == 0) return 0;
            if (n > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!isSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }
                result = root;
            }
            return result;
        }

        private static Boolean isSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BigInteger p = BigInteger.Parse(TextBoxInputP.Text);
            BigInteger q = BigInteger.Parse(TextBoxInputQ.Text);
            if(p==q) { MessageBox.Show("Values of the parameters 'p' and 'q' cannot be equal!", "Error", MessageBoxButton.OK, MessageBoxImage.Error); goto exit0; }
            BigInteger n = p * q;
            BigInteger fi_n = (p - 1) * (q - 1);
            BigInteger public_e = GetPrimeWithFi_n(fi_n);

            BigInteger d = Get_d(public_e, fi_n);
            TextBoxInputE.Text = public_e.ToString();
            TextBoxInputD.Text = d.ToString();
            TextBoxInputN1.Text = n.ToString();
            TextBoxInputN2.Text = n.ToString();
        exit0:;
        }

        private BigInteger GetPrimeWithFi_n(BigInteger fi_n)
        {
            BigInteger limit = (BigInteger)BigInteger.Log(fi_n, Convert.ToDouble(2));
            Random rnd = new Random();
            BigInteger randomValue;
            BigInteger temp;
            do
            {
                randomValue = rnd.Next(0, (int)limit);
                temp = BigInteger.Pow(BigInteger.Parse("2"), (int)randomValue) + 1;
                //MessageBox.Show(temp.ToString() + "||" + fi_n.ToString());
            }
            while (IsCoprimeTest(temp, fi_n) == false);
            return temp;
        }

        private bool IsCoprimeTest(BigInteger num1, BigInteger num2)
        {
            BigInteger temp = 0;
            int q = 0;
            bool res = false;
            while(q==0)
            {
                if (num1 < num2)
                {
                    temp = num1;
                    num1 = num2;
                    num2 = temp;
                }
                num1 = num1 % num2;
                if (num1 == 1) { q = 1; res = true; };
                if (num1 == 0) { q = 1; res = false; };
            }
            return res;
        }
        private bool IsCoprimeNew(BigInteger num1, BigInteger num2)
        {
            while (num2 != 0)
            {
                if (num1 > num2)
                {
                    num1 -= num2;
                }
                else
                {
                    num2 -= num1;
                }
            }
            if (num1 == 1) return true;
            else
            {
                return false;
            }
        }
        private BigInteger Get_d(BigInteger e, BigInteger phi_n)
        {
            BigInteger a = e;
            BigInteger b = phi_n, x = 0, d_ = 1;
            while (a > 0)
            {
                BigInteger q_ = b / a;
                BigInteger y = a;
                a = b % a;
                b = y;
                y = d_;
                d_ = x - q_ * d_;
                x = y;
            }
            x = (x + phi_n) % phi_n;
            return x;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string h = GetHash(TextBoxTextToSend.Text);
            string[] input_a = new string[12];
            int input = 0;
            for(int i=0; i<12;i++)
            {
                input_a[i] = h.Substring(i * 2, 2);
                byte[] input_b = Encoding.ASCII.GetBytes(input_a[i]);
                for(int j=0; j <2; j++ )
                {
                    input += input_b[j];
                }
            }
            MessageBox.Show(input.ToString());
            BigInteger d = BigInteger.Parse(TextBoxInputD.Text);
            BigInteger n = BigInteger.Parse(TextBoxInputN2.Text);
            BigInteger s = BigInteger.ModPow(input, d, n);
            TextBoxInputS.Text = s.ToString();
        }

        private string GetHash(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            TextBoxTextToRecieve.Text = TextBoxTextToSend.Text;
            TextBoxInputSFromSend.Text = TextBoxInputS.Text;
            TextBoxOutputE.Text = TextBoxInputE.Text;
            TextBoxOutPutN2.Text = TextBoxInputN2.Text;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            string h = GetHash(TextBoxTextToRecieve.Text);
            string[] output_a = new string[12];
            BigInteger output = 0;
            for (int i = 0; i < 12; i++)
            {
                output_a[i] = h.Substring(i * 2, 2);
                byte[] output_b = Encoding.ASCII.GetBytes(output_a[i]);
                for (int j = 0; j < 2; j++)
                {
                    output += output_b[j];
                }
            }
            BigInteger s = BigInteger.Parse(TextBoxInputSFromSend.Text);
            BigInteger public_e = BigInteger.Parse(TextBoxOutputE.Text);
            BigInteger n = BigInteger.Parse(TextBoxOutPutN2.Text);
            BigInteger h_orig = BigInteger.ModPow(s, public_e, n);
            output = output % n;
            MessageBox.Show(output.ToString() + "||" + h_orig.ToString());
            if(output == h_orig)
            {
                MessageBox.Show("Успех", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Realization of Digital Signature on RSA algorithm by IvanovA (2021).\n\n- How to use it? \nThis algorithm is able to work in a short time with numbers up to 2^1024 (10^309). It is recommended to set the number of checks (Acc) equal to the square root of the specified number (For 10^309 - 1024). This is necessary for the correct operation of the simplicity test, which is made according to the Miller-Rabin algorithm. "+ "\n"+
            "For it's work just follow the necessary steps indicated on the buttons (Number). After pressing the button '4) Get signature' you may write some text (which you need to send) into the textbox."
            , "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
