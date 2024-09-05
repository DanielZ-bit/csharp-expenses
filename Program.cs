using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WindowsFormsApp1.Program;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        static List<Expenses> details = new List<Expenses>();
        static List<Expenses> descriptions = new List<Expenses>();

        public class Expenses
        {
            public int amount;
            public DateTime Date;
            public string description;

            public void Print()
            {
                Console.WriteLine($"{amount}, {description}");
            }
        }

        public class MainForm : Form
        {
            private TextBox textbox;
            private TextBox textbox1;
            private TextBox textbox2;
            private TextBox Deletebox;
            private TextBox Searchbox;
            private TextBox YearBox;
            private Label label1;
            private Label label2;

            public MainForm()
            {
                InitializeComponents();
            }

            private void InitializeComponents()
            {
                textbox = new TextBox(); // Amount
                textbox.Location = new System.Drawing.Point(10, 10);
                textbox.Width = 200;

                textbox1 = new TextBox(); // Date
                textbox1.Location = new System.Drawing.Point(10, 30);
                textbox1.Width = 200;

                textbox2 = new TextBox(); // Description
                textbox2.Location = new System.Drawing.Point(10, 50);
                textbox2.Width = 200;

                Deletebox = new TextBox();
                Deletebox.Location = new System.Drawing.Point(160, 70);
                Deletebox.Width = 60;
                Deletebox.Text = "Position of Item";

                Searchbox = new TextBox();
                Searchbox.Location = new System.Drawing.Point(200, 100);
                Searchbox.Width = 60;

                YearBox = new TextBox();
                YearBox.Location = new System.Drawing.Point(10, 250);
                YearBox.Width = 60;

                // Add Button
                Button button1 = new Button();
                button1.Text = "Add Expense";
                button1.Location = new System.Drawing.Point(10, 70);
                button1.Click += Button_Click;

                Button DeleteButton = new Button();
                DeleteButton.Text = "Delete";
                DeleteButton.Location = new System.Drawing.Point(80, 70);
                DeleteButton.Click += Delete_click;

                Button YOrderButton = new Button();
                YOrderButton.Text = "Order Year";
                YOrderButton.Location = new System.Drawing.Point(10, 100);
                YOrderButton.Click += (sender, e) =>
                {
                    details = details.OrderBy(x => x.Date.Year).ToList();
                    ShowText();
                };

                Button MOrderButton = new Button();
                MOrderButton.Text = "Order Month";
                MOrderButton.Location = new System.Drawing.Point(80, 100);
                MOrderButton.Click += (sender, e) =>
                {
                    details = details.OrderBy(x => x.Date.Month).ToList();
                    ShowText();
                };

                Button SearchButton = new Button();
                SearchButton.Text = "Search";
                SearchButton.Location = new System.Drawing.Point(300, 100);
                SearchButton.Click += (sender, e) =>
                {
                    label2.Text = string.Empty;
                    descriptions = details.Where(x => x.description == Searchbox.Text).ToList();
                    foreach (var desc in descriptions)
                    {
                        label2.Text += $"£{desc.amount} Date: {desc.Date.Month}, {desc.Date.Year}, Description: {desc.description} \n";
                    }
                };

                Button SumButton = new Button();
                SumButton.Text = "Add Year's Expenses";
                SumButton.Location = new System.Drawing.Point(70, 250);
                SumButton.Click += (sender, e) =>
                {
                    int year;
                    int.TryParse(YearBox.Text, out year);
                    int YearTotal = details.Where(x => x.Date.Year == year).Sum(x => x.amount);
                    MessageBox.Show($"Total expenses for year {year}: £{YearTotal}");
                };

                label1 = new Label();
                label1.Location = new System.Drawing.Point(10, 130);
                label1.Width = 300;
                label1.Height = 100;
                label1.AutoSize = true;

                label2 = new Label();
                label2.Location = new System.Drawing.Point(10, 200);
                label2.Width = 300;
                label2.Height = 100;
                label2.AutoSize = true;

                this.Load += FormLoad;

                this.FormClosing += (sender, e) =>
                {
                    StreamWriter sw = new StreamWriter("expenses.txt");
                    foreach (var item in details)
                    {
                        sw.WriteLine($"amount: {item.amount}, Date: {item.Date}, Description: {item.description}");
                    }
                    sw.Close();
                };

                // Add controls to the form
                Controls.Add(textbox);
                Controls.Add(textbox1);
                Controls.Add(textbox2);
                Controls.Add(Deletebox);
                Controls.Add(Searchbox);
                Controls.Add(YearBox);
                Controls.Add(SearchButton);
                Controls.Add(button1);
                Controls.Add(YOrderButton);
                Controls.Add(MOrderButton);
                Controls.Add(DeleteButton);
                Controls.Add(SumButton);
                Controls.Add(label1);
                Controls.Add(label2);
            }

            [STAThread]
            public static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }

            private void Button_Click(object sender, EventArgs e)
            {
                try
                {
                    details.Add(new Expenses
                    {
                        amount = Int32.Parse(textbox.Text),
                        Date = DateTime.Parse(textbox1.Text),
                        description = textbox2.Text
                    });
                    ShowText();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid input. Please enter valid values.");
                }
            }

            private void Delete_click(object sender, EventArgs e)
            {
                int Amount;
                bool success = Int32.TryParse(Deletebox.Text, out Amount);
                if (success && Amount > 0 && Amount <= details.Count)
                {
                    details.RemoveAt(Amount - 1);
                    ShowText();
                }
                else
                {
                    Deletebox.Text = "Provide a valid number";
                }
            }

            public void ShowText()
            {
                label1.Text = "";
                foreach (Expenses expenses in details)
                {
                    label1.Text += $"£{expenses.amount} Date: {expenses.Date.Day}/{expenses.Date.Month}/{expenses.Date.Year}, Description: {expenses.description} \n";
                }
                textbox.Clear();
                textbox1.Clear();
                textbox2.Clear();
            }

            private void FormLoad(object sender, EventArgs e)
            {
                string filePath = "expenses.txt";
                if (File.Exists(filePath))
                {
                    StreamReader sr = new StreamReader(filePath);
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 3)
                        {
                            var amountStr = parts[0].Split(':')[1].Trim();
                            var parts2 = parts[1].Split(':');
                            var dateStr = parts2[1].Split(' ')[1].Trim(); ;
                            var description = parts[2].Split(':')[1].Trim();

                            int Amount = int.Parse(amountStr);
                            DateTime date = DateTime.Parse(dateStr);

                            details.Add(new Expenses
                            {
                                amount = Amount,
                                Date = date,
                                description = description
                            });
                        }
                    }
                    sr.Close();
                    ShowText();
                }
            }
        }
    }
}
