using System;
using System.Windows.Forms;

namespace SyncServerWinForms
{
   public partial class FormOne : Form
   {
      public FormOne()
      {
         InitializeComponent();
      }

      private void ButtonStart_Click(object sender, EventArgs e)
      {
         string lineone = "Проверка связи";
         TextBoxReader.AppendText(lineone);
         TextBoxReader.AppendText(Environment.NewLine);
         // Прокрутка TextBox вниз
         TextBoxReader.ScrollToCaret();

         ListBoxReader.Items.Add(lineone);
         // Прокрутка ListBox вниз
         ListBoxReader.TopIndex = ListBoxReader.Items.Count - 1;

         RichTextBoxReader.AppendText(lineone);
         RichTextBoxReader.AppendText(Environment.NewLine);
         // Прокрутка RichTextBox вниз
         RichTextBoxReader.ScrollToCaret();


      }

      private void ButtonClear_Click(object sender, EventArgs e)
      {
         TextBoxReader.Clear();
         RichTextBoxReader.Clear();
         ListBoxReader.Items.Clear();
      }
   }
}