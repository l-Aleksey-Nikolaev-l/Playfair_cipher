using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Playfair_cipher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            panel4.Height = 40;
            panel5.Height = 0;
        }

        Button currentButton;

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        #region Fields for encrypt

        char[,] alph = new char[5, 5] { { 'A', 'B', 'C', 'D', 'E' },
                                        { 'F', 'G', 'H', 'I', 'K' },
                                        { 'L', 'M', 'N', 'O', 'P' },
                                        { 'Q', 'R', 'S', 'T', 'U' },
                                        { 'V', 'W', 'X', 'Y', 'Z' } };
        char[,] newalph = new char[5, 5];

        char[] newtext;
        int[] encrypt = new int[4];
        int[] first_second = new int[2];
        int countForString = 0;
        int countForChar = 0;
        int score = 0;
        int temp = 0;

        #endregion

        #region Code for encrypt

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "Encrypt")
            {
                //textBox2.ReadOnly = false;
                StartEncrypt();
                //textBox2.ReadOnly = true;
            }
            else if (button1.Text == "Decrypt")
            {
                //textBox2.ReadOnly = false;
                StartDecrypt();
                //textBox2.ReadOnly = true;
            }
        }

        private void StartEncrypt()
        {
            textBox2.Text = "";
            textBox1.Text = textBox1.Text.ToUpper();
            textBox3.Text = textBox3.Text.ToUpper();
            textBox1.Text = textBox1.Text.Trim();
            textBox3.Text = textBox3.Text.Trim();
            //if (button1.Enabled == false)
            //{
            //    return;
            //}
            KeyAlph();
            StringToChars(textBox1.Text);

            do
            {
                foreach (char sumbol in newalph)
                {
                    if (sumbol == newtext[countForChar])
                    {
                        FindIndex(sumbol);
                        EncryptRead();
                        countForChar++;
                        break;
                    }
                }
            }
            while (countForChar != newtext.Length);

            if (score == 2)
            {
                EncryptOneSym();
            }

            ClearVariables();
        }

        private void StartDecrypt()
        {
            string saveTB1 = textBox1.Text;

            for (int i = 0; i <= 8; i++)
            {
                StartEncrypt();
                textBox1.Text = textBox2.Text;
            }
            textBox1.Text = saveTB1;
        }

        private void ClearVariables() // Cleare all variables after encrypt
        {
            temp = 0;
            score = 0;
            countForChar = 0;
            countForString = 0;
            Array.Clear(newalph, 0, newalph.Length);
            Array.Clear(newtext, 0, newtext.Length);
            Array.Clear(encrypt, 0, encrypt.Length);
            Array.Clear(first_second, 0, first_second.Length);
        }

        private void StringToChars (string line) // Create (char array) with text for encrypt
        {
            countForString = 0;

            line = Regex.Replace(line, @"\s+", ""); // Delete all spaces
            line = Regex.Replace(line, "J", "I");  // All words (J) repleace to (I) word

            newtext = new char[line.Length];

            foreach (char letter in line)
            {
                newtext[countForString++] = letter;
            }
        }

        private void FindIndex(char sym) // Find letter index in new alphabet (newalph)
        {
            for (int one = 0; one <= 4; one++)
            {
                for (int two = 0; two <= 4; two++)
                    if (newalph[one,two] == sym)
                    {
                        first_second[0] = one;
                        first_second[1] = two;
                        score += 2;
                        return;
                    }
            }
        }

        private void EncryptRead() // Determine the number of symbols (one or two) and written they index in array (encrypt)
        {
            if (score == 2) // If we have only one symbol then go out
            {
                encrypt[0] = first_second[0];
                encrypt[1] = first_second[1];
            }
            else if (score == 4) // If we have two symbols then go to Encrypt() method
            {
                encrypt[2] = first_second[0];
                encrypt[3] = first_second[1];
                score = 0;
                Encrypt();
            }
        }

        private void Encrypt() // This method have 4 rules for the Playfair cipher
        {
            if (encrypt[0] == encrypt[2] && encrypt[1] == encrypt[3]) // If two letters equil then go to special 3 rules
            {
                EncryptOneSym();
                countForChar -= 2; ;
            }
            else if (encrypt[0] != encrypt[2])
            {
                if (encrypt[1] != encrypt[3]) // If letters have cross
                {
                    temp = encrypt[1];
                    encrypt[1] = encrypt[3];
                    encrypt[3] = temp;
                    temp = 0;
                }
                else // If letters have vertical line
                {
                    if (encrypt[0] != 4)
                    {
                        encrypt[0] += 1;
                    }
                    else
                    {
                        encrypt[0] = 0;
                    }

                    if (encrypt[2] != 4)
                    {
                        encrypt[2] += 1;
                    }
                    else
                    {
                        encrypt[2] = 0;
                    }
                }
                EncryptWrite();
            }
            else // If letters have horizontal line
            {
                if (encrypt[1] != 4)
                {
                    encrypt[1] += 1;
                }
                else
                {
                    encrypt[1] = 0;
                }

                if (encrypt[3] != 4)
                {
                    encrypt[3] += 1;
                }
                else
                {
                    encrypt[3] = 0;
                }
                EncryptWrite();
            }
        }

        private void EncryptOneSym() // This method have 3 special rules for the Playfair cipher
        {
            if (encrypt[0] == 4) // if two equil letters in the the last row
            {
                if (encrypt[1] == 4) // This is for the last letter in newalph array
                {
                    textBox2.Text += newalph[encrypt[0], encrypt[1] = 0];
                }
                else
                {
                    textBox2.Text += newalph[encrypt[0], encrypt[1] += 1];
                    encrypt[1] -= 1;
                }
                temp = encrypt[3];
                encrypt[3] = 3;
                textBox2.Text += newalph[encrypt[0], encrypt[3]];
                encrypt[3] = temp;
                temp = 0;
            }
            else if (encrypt[1] == 2) // if two equil letters in the centre column and not the last row
            {
                textBox2.Text += newalph[encrypt[0] += 1, encrypt[1]];
                encrypt[0] -= 1;
                temp = encrypt[2];
                encrypt[2] = 0;
                textBox2.Text += newalph[encrypt[2], encrypt[3]];
                encrypt[2] = temp;
                temp = 0;
            }
            else  // if two equil letters not in the centre and not the last row
            {
                temp = encrypt[1];
                encrypt[1] = 2;
                textBox2.Text += newalph[encrypt[0], encrypt[1]];
                encrypt[1] = temp;
                temp = encrypt[2];
                encrypt[2] = 4;
                textBox2.Text += newalph[encrypt[2], encrypt[1]];
                encrypt[2] = temp;
                temp = 0;
            }
            newtext[countForChar - 1] = ' '; // Delete one letter
            string AA = String.Join("", newtext);
            StringToChars(AA); // Update newtext array
        }

        private void EncryptWrite() // Writting encrypt result
        {
            newtext[countForChar - 1] = newalph[encrypt[0], encrypt[1]];
            newtext[countForChar] = newalph[encrypt[2], encrypt[3]];
            textBox2.Text += newtext[countForChar - 1].ToString() + newtext[countForChar].ToString();
        }

        private void KeyAlph()  // Create new an alphabet with or without keyword
        {
            if (textBox3.Text != "")  // If keyword was written then create newalph array with keyword
            {
                string key = textBox3.Text;
                key = Regex.Replace(key, @"\s+", "");
                textBox3.Text = key;
                int flag = 0;

                char[] newKey = key.ToCharArray().Distinct().ToArray();
                int keyLen = 0;

                for (int keyOne = 0; keyOne <= 4; keyOne++)
                {
                    for(int keyTwo = 0; keyTwo <= 4; keyTwo++)
                    {
                        if (keyLen != newKey.Length)
                        {
                            newalph[keyOne, keyTwo] = newKey[keyLen];
                            keyLen++;
                        }
                        else 
                        {
                            foreach(char A in alph)
                            {
                                foreach(char B in newalph)
                                {
                                    if (B == A)
                                    {
                                        flag = 1;
                                        break;
                                    }
                                    else if (B == '\0')
                                    {
                                        newalph[keyOne, keyTwo] = A;
                                        break;
                                    }
                                }

                                if (flag != 1)
                                {
                                    break;
                                }
                                else
                                {
                                    flag = 0;
                                }
                            }
                        }
                    }
                }
            }
            else// If keyword wasn't written then create newalph array without keyword
            {
                for (int one = 0; one <= 4; one++)
                {
                    for (int two = 0; two <= 4; two++)
                    {
                        newalph[one, two] = alph[one, two];
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }
       
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (label3.Text == "Rules")
            {
                return;
            }
            textBox1.Text = IsAlph(textBox1.Text);
            textBox1.SelectionStart = textBox1.Text.Length;

            if (textBox1.Text == "")
            {
                button1.Enabled = false;
                button3.Enabled = false;

                button1.BackColor = Color.Transparent;
                button3.BackColor = Color.Transparent;
            }
            else
            {
                button1.Enabled = true;
                button3.Enabled = true;

                button3.BackColor = Color.White;
                button1.BackColor = Color.White;
            }
            textBox1.ScrollToCaret();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox1.ScrollToCaret();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox3.Text = IsAlph(textBox3.Text);
            textBox3.SelectionStart = textBox3.Text.Length;
            label1.Text = "Keyword (" + (20 - textBox3.Text.Length) + " symbols max.)";
        }

        public string IsAlph(string input) // This method for check symbols
        {
            bool res = Regex.IsMatch(input, @"^[a-zA-Z\s+]+$"); // If char is not letter then return FALSE

            if (res != true & input != "")
            {
                input = Regex.Replace(input, @"[\p{N}|~`!@#$%^&*()_+=\[{\]};:<>|./?,\\'""-]", "");
            }
            return input;
        }

        #endregion

        async private void btnClose_Click(object sender, EventArgs e)
        {
            for (double opacity = 1; opacity >= 0.02; )
            {
                opacity -= 0.02;
                this.Opacity = opacity;
                await Task.Delay(1);
            }
            this.Dispose();
        }

        async private void Form1_Load(object sender, EventArgs e)
        {
            button6.BackColor = Color.FromArgb(103, 132, 150);
            button6.Font = new Font("Arial", 12.5F, FontStyle.Regular, GraphicsUnit.Point, 204);

            for (double opacity = 0; opacity <= 1;)
            {
                opacity += 0.02;
                this.Opacity = opacity;
                await Task.Delay(1);
            }

            
            for (int textBoxSize = 0; textBoxSize <= 600; textBoxSize+=15)
            {
                panel5.Height = textBoxSize;
                await Task.Delay(1);
            }
            await Task.Delay(100);
            textBox1.DeselectAll();
            textBox1.Text = Properties.Settings.Default.Rules;
            textBox2.ReadOnly = true;

        }

        private void ActButton(object buttonSender)
        {
            if (buttonSender != null)
            {
                if (currentButton != (Button)buttonSender)
                {
                    ButtonsDisable();
                    currentButton = (Button)buttonSender;
                    currentButton.BackColor = Color.FromArgb(103, 132, 150);
                    currentButton.ForeColor = Color.White;
                    currentButton.Font = new Font("Arial", 12.5F, FontStyle.Regular, GraphicsUnit.Point, 204);
                }
            }
        }

        private void ButtonsDisable()
        {
            foreach (Control otherButtons in panel1.Controls)
            {
                if(otherButtons.GetType() == typeof(Button))
                {
                    otherButtons.BackColor = Color.FromArgb(115, 147, 167);
                    otherButtons.ForeColor = Color.White;
                    otherButtons.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ActButton(sender);

            textBox1.DeselectAll();
            button1.Text = "Encrypt";
            textBox2.ReadOnly = false;
            SmallGrow();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ActButton(sender);

            textBox1.DeselectAll();
            button1.Text = "Decrypt";
            textBox2.ReadOnly = false;
            SmallGrow();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ActButton(sender);
            BigGrow();
        }

        async private void BigGrow()
        {
            if (panel5.Height < 600)
            {
                
                for (int textBoxSize = 210; textBoxSize > 30; textBoxSize -= 10)
                {
                    panel4.Height = textBoxSize;
                    panel5.Height = textBoxSize;
                    await Task.Delay(1);
                }
                textBox1.Text = "";
                label2.Text = "";
                label3.Text = "Rules";
                panel3.Visible = false;
                panel4.Visible = false;
                button1.Visible = false;
                button3.Visible = false;
                await Task.Delay(100);
                for (int textBoxSize = 30; textBoxSize <= 600; textBoxSize += 10)
                {
                    panel5.Height = textBoxSize;
                    await Task.Delay(1);
                }
            }
            await Task.Delay(100);
            textBox1.Text = Properties.Settings.Default.Rules;
            textBox2.ReadOnly = true;
        }

        async private void SmallGrow()
        {
            if (panel5.Height == 600)
            {
                textBox1.Text = "";
                await Task.Delay(100);
                for (int textBoxSize = 600; textBoxSize > 30; textBoxSize -= 10)
                {
                    panel5.Height = textBoxSize;
                    await Task.Delay(1);
                }
                await Task.Delay(100);
                panel3.Visible = true;
                panel4.Visible = true;
                button1.Visible = true;
                button3.Visible = true;
                LabelText();
                for (int textBoxSize = 30; textBoxSize <= 210; textBoxSize += 10)
                {
                    panel4.Height = textBoxSize;
                    panel5.Height = textBoxSize;
                    await Task.Delay(1);
                }
            }
            else if (panel5.Height == 210)
            {

                for (int textBoxSize = 210; textBoxSize > 30; textBoxSize -= 10)
                {
                    panel4.Height = textBoxSize;
                    panel5.Height = textBoxSize;
                    await Task.Delay(1);
                }
                
                LabelText();
                for (int textBoxSize = 30; textBoxSize <= 210; textBoxSize += 10)
                {
                    panel4.Height = textBoxSize;
                    panel5.Height = textBoxSize;
                    await Task.Delay(1);
                }
            }
            
            textBox2.ReadOnly = true;
        }

        async private void LabelText()
        {
            if(button1.Text == "Encrypt")
            {
                label2.Text = "Encrypted text";
                label3.Text = "Source decrypted text";
                textBox1.Text = "TEXT FOR EXAMPLE";
                textBox2.Text = "";
            }
            else if(button1.Text == "Decrypt")
            {
                label2.Text = "Decrypted text";
                label3.Text = "Source encrypted text";
                textBox1.Text = "UD YS IL UB VC NL PA";
                textBox2.Text = "";
            }
            await Task.Delay(100);
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }


        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.ForeColor = Color.Black;
            button1.FlatAppearance.BorderSize = 3;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.White;
            button1.FlatAppearance.BorderSize = 0;
        }

        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            button3.FlatAppearance.BorderSize = 3;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.ForeColor = Color.Black;
            button3.FlatAppearance.BorderSize = 0;
        }

    }
}