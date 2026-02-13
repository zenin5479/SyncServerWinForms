using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SyncServerWinForms
{
   public partial class FormOne : Form
   {
      public class Item
      {
         public DateTimeOffset Date { get; set; }
         public long Timestamp { get; set; }
         public int Id { get; set; }
         public string Vendor { get; set; }
         public string Name { get; set; }
         public double Price { get; set; }
      }


      public FormOne()
      {
         InitializeComponent();
      }

      private void ButtonStart_Click(object sender, EventArgs e)
      {
         //TextBoxReader.AppendText(lineone);
         TextBoxReader.AppendText(Environment.NewLine);
         // Прокрутка TextBox вниз
         TextBoxReader.ScrollToCaret();

         //ListBoxReader.Items.Add(lineone);
         // Прокрутка ListBox вниз
         ListBoxReader.TopIndex = ListBoxReader.Items.Count - 1;

         //RichTextBoxReader.AppendText(lineone);
         RichTextBoxReader.AppendText(Environment.NewLine);
         // Прокрутка RichTextBox вниз
         RichTextBoxReader.ScrollToCaret();
      }
   }
}