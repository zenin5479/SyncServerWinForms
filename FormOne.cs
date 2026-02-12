using System.Net;
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

      private void button1_Click(object sender, System.EventArgs e)
      {
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
      }
   }
}