using System;
using System.Net;
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
         
         // Написать методы класса SyncServer без отправки данных на печать

         SyncServer server = new SyncServer("http://127.0.0.1:8080/");
         try
         {
            server.Start();
         }
         catch (HttpListenerException ex)
         {
            Console.WriteLine("Не удалось запустить сервер: {0}", ex.Message);
         }
         catch (Exception ex)
         {
            Console.WriteLine("Ошибка: {0}", ex.Message);
         }


         //SyncServer.Start();
      }

      private void ButtonClear_Click(object sender, EventArgs e)
      {
         TextBoxReader.Clear();
         RichTextBoxReader.Clear();
         ListBoxReader.Items.Clear();
      }
   }
}