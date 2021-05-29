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
            //TextBoxInputP.Text = AtkinSieve(BigInteger.Parse(TextBoxIntervalA.Text), BigInteger.Parse(TextBoxIntervalB.Text));
            TextBoxInputP.Text = GenerateBigInt(BigInteger.Parse(TextBoxIntervalB.Text), Convert.ToInt32(TextBoxIntervalA.Text)).ToString();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //TextBoxInputQ.Text = AtkinSieve(BigInteger.Parse(TextBoxIntervalA.Text), BigInteger.Parse(TextBoxIntervalB.Text));
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
            // если n == 2 или n == 3 - эти числа простые, возвращаем true
            if (n == 2 || n == 3)
                return true;

            // если n < 2 или n четное - возвращаем false
            if (n < 2 || n % 2 == 0)
                return false;

            // представим n − 1 в виде (2^s)·t, где t нечётно, это можно сделать последовательным делением n - 1 на 2
            BigInteger t = n - 1;

            int s = 0;

            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }

            // повторить k раз
            for (int i = 0; i < k; i++)
            {
                // выберем случайное целое число a в отрезке [2, n − 2]
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                byte[] _a = new byte[n.ToByteArray().LongLength];

                BigInteger a;

                do
                {
                    rng.GetBytes(_a);
                    a = new BigInteger(_a);
                }
                while (a < 2 || a >= n - 2);

                // x ← a^t mod n, вычислим с помощью возведения в степень по модулю
                BigInteger x = BigInteger.ModPow(a, t, n);

                // если x == 1 или x == n − 1, то перейти на следующую итерацию цикла
                if (x == 1 || x == n - 1)
                    continue;

                // повторить s − 1 раз
                for (int r = 1; r < s; r++)
                {
                    // x ← x^2 mod n
                    x = BigInteger.ModPow(x, 2, n);

                    // если x == 1, то вернуть "составное"
                    if (x == 1)
                        return false;

                    // если x == n − 1, то перейти на следующую итерацию внешнего цикла
                    if (x == n - 1)
                        break;
                }

                if (x != n - 1)
                    return false;
            }

            // вернуть "вероятно простое"
            return true;
        }
        private string AtkinSieve(BigInteger limitA, BigInteger limitB)
        {
            string result = "";
            BigInteger sqr_lim = Sqrt(limitB);
            BigInteger x2 = 0;
            BigInteger y2, n;
            int i, j;
            BigInteger count = 0;
            BigInteger size = limitB + (BigInteger)(1);
            bool[] is_prime = new bool[(int)size];
            for (i=0; i< limitB; i++)
            {
                is_prime[i] = false;
            }
            is_prime[2] = true;
            is_prime[3] = true;
            for (i=1; i < sqr_lim; ++i)
            {
                x2 += 2 * i - 1;
                y2 = 0;
                for (j = 1; j < sqr_lim; ++j)
                {
                    y2 += 2 * j - 1;
                    n = 4 * x2 + y2;
                    if ((n <= limitB) && (n % 12 == 1 || n % 12 == 5))
                        is_prime[(int)n] = !is_prime[(int)n];

                    n -= x2;
                    if ((n <= limitB) && (n % 12 == 7))
                        is_prime[(int)n] = !is_prime[(int)n];

                    n -= 2 * y2;
                    if ((i > j) && (n <= limitB && (n % 12 == 11)))
                        is_prime[(int)n] = !is_prime[(int)n];
                }
            }
            for (i = 5; i <= sqr_lim; ++i)
            {
                if (is_prime[i])
                {
                    n = i * i;
                    for (j = (int)n; j <= limitB; j += (int)n)
                        is_prime[j] = false;
                }
            }
            for(i = (int)limitA; i <= limitB; ++i)
            {
                if ((is_prime[i] && (i % 3 != 0) && (i % 5 != 0))) count++;
            }
            int temp = 0;
            Random rnd = new Random();
            int randomValue = rnd.Next(0, (int)count);
            for (i = (int)limitA; i <= limitB; ++i)
            {
                if ((is_prime[i] && (i % 3 != 0) && (i % 5 != 0)))
                {
                    if(temp == randomValue)
                    {
                        result = i.ToString();
                        goto exit;
                    }
                        temp++;
                }
            }
            exit:
            return result;
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

        /*private bool IsCoprime(BigInteger num1, BigInteger num2)
        {
            if (num1 == num2)
            {
                return num1 == 1;
            }
            else
            {
                if (num1 > num2)
                {
                    return IsCoprime(num1 - num2, num2);
                }
                else
                {
                    return IsCoprime(num2 - num1, num1);
                }
            }
        }
        */
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
                //MessageBox.Show(num1.ToString() + "||" + num2.ToString());
                num1 = num1 % num2;
                //MessageBox.Show(num1.ToString());
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
            /*
            string input1a = h.Substring(0, 2);
            byte[] input1b = Encoding.ASCII.GetBytes(input1a);
            int input1 = 0;
            for (int i=0; i<2; i++)
            {
                input1 += input1b[i];
            }
            */
            //TextBoxInputQ.Text = input1.ToString();
            //TextBoxForOtladka.Text = input1.ToString();
            MessageBox.Show(input.ToString());
            BigInteger d = BigInteger.Parse(TextBoxInputD.Text);
            BigInteger n = BigInteger.Parse(TextBoxInputN2.Text);
            //MessageBox.Show((Math.Pow(Convert.ToInt32(TextBoxForOtladka.Text), d)).ToString());
            //MessageBox.Show((((int)(Math.Pow(Convert.ToInt32(TextBoxForOtladka.Text),d))).ToString()));
            //BigInteger s = (BigInteger.Pow(input, d) % n);
            BigInteger s = BigInteger.ModPow(input, d, n);
            //BigInteger s = (BigInteger.Pow(input1, d) % n);
            //BigInteger s = (int)(Math.Pow(Convert.ToInt32(TextBoxForOtladka.Text), d) % n);
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
            /*
            string h = GetHash(TextBoxTextToRecieve.Text);
            string output1a = h.Substring(0, 2);
            byte[] output1b = Encoding.ASCII.GetBytes(output1a);
            int output1 = 0;
            for (int i = 0; i < 2; i++)
            {
                output1 += output1b[i];
            }
            */
            //MessageBox.Show(output1.ToString());
            BigInteger s = BigInteger.Parse(TextBoxInputSFromSend.Text);
            BigInteger public_e = BigInteger.Parse(TextBoxOutputE.Text);
            BigInteger n = BigInteger.Parse(TextBoxOutPutN2.Text);
            //BigInteger h_orig = (BigInteger.Pow(s, public_e) % n);
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
