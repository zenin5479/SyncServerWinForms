using System;
using System.Net;
using System.Windows.Forms;

namespace SyncServerWinForms
{
   public partial class FormOne : Form
   {
      private TextBox textBoxReader;
      private ListBox listBoxReader;
      private RichTextBox richTextBoxReader;


      public FormOne()
      {
         InitializeComponent();
         // здесь создаются элементы управления
         // Создаём экземпляр вспомогательного класса и передаём ему ссылки
         var helper = new SyncServer(textBoxInput, listBoxLog, richTextBoxContent);

      }

      private void ButtonStart_Click(object sender, EventArgs e)
      {
         string lineone = "Проверка связи";
         TextBoxReader.AppendText(lineone);
         TextBoxReader.AppendText(Environment.NewLine);
         // Прокрутка TextBox вниз
         TextBoxReader.ScrollToCaret();
         string url = "http://127.0.0.1:8080/";

         SyncServer server = new SyncServer(url, TextBoxReader, ListBoxReader, RichTextBoxReader);
         try
         {
            server.Start();
            string line = "Синхронный Json сервер запущен по адресу: ";
            TextBoxReader.AppendText(line + url);
         }
         catch (HttpListenerException ex)
         {
            string line = "Не удалось запустить сервер: ";
            TextBoxReader.AppendText(line + ex.Message);
         }
         catch (Exception ex)
         {
            string line = "Ошибка: ";
            TextBoxReader.AppendText(line + ex.Message);
         }

         // Написать методы класса SyncServer без отправки данных на печать
      }

      private void ButtonClear_Click(object sender, EventArgs e)
      {
         TextBoxReader.Clear();
         RichTextBoxReader.Clear();
         ListBoxReader.Items.Clear();
      }
   }
}