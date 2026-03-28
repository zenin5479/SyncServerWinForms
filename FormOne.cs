using System;
using System.Net;
using System.Windows.Forms;

namespace SyncServerWinForms
{
   public partial class FormOne : Form
   {
      private Timer _timer;

      public FormOne()
      {
         InitializeComponent();
      }

      private void ButtonStart_Click(object sender, EventArgs e)
      {
         // Создаём и настраиваем таймер
         _timer = new Timer();
         // Интервал в миллисекундах (1 секунда)
         _timer.Interval = 1000;

         string lineone = "Проверка связи";
         TextBoxReader.AppendText(lineone);
         TextBoxReader.AppendText(Environment.NewLine);

         ListBoxReader.Items.Add(lineone);

         RichTextBoxReader.AppendText(lineone);
         RichTextBoxReader.AppendText(Environment.NewLine);

         string url = "http://127.0.0.1:8080/";
         // Создаём экземпляр класса и передаём ему ссылки на элементы управления
         SyncServer server = new SyncServer(url, TextBoxReader, ListBoxReader, RichTextBoxReader, _timer);
         try
         {
            server.Start();
            string line = "Синхронный Json сервер запущен по адресу: ";
            TextBoxReader.AppendText(line + url);
            TextBoxReader.AppendText(Environment.NewLine);
            // Прокрутка TextBox вниз
            TextBoxReader.ScrollToCaret();

            ListBoxReader.Items.Add(line + url);
            // Прокрутка ListBox вниз
            ListBoxReader.TopIndex = ListBoxReader.Items.Count - 1;

            RichTextBoxReader.AppendText(line + url);
            RichTextBoxReader.AppendText(Environment.NewLine);
            // Прокрутка RichTextBox вниз
            RichTextBoxReader.ScrollToCaret();
         }
         catch (HttpListenerException ex)
         {
            string line = "Не удалось запустить сервер: ";
            TextBoxReader.AppendText(line + ex.Message);
            TextBoxReader.AppendText(Environment.NewLine);
            // Прокрутка TextBox вниз
            TextBoxReader.ScrollToCaret();

            ListBoxReader.Items.Add(line + ex.Message);
            // Прокрутка ListBox вниз
            ListBoxReader.TopIndex = ListBoxReader.Items.Count - 1;

            RichTextBoxReader.AppendText(line + ex.Message);
            RichTextBoxReader.AppendText(Environment.NewLine);
            // Прокрутка RichTextBox вниз
            RichTextBoxReader.ScrollToCaret();

         }
         catch (Exception ex)
         {
            string line = "Ошибка: ";
            TextBoxReader.AppendText(line + ex.Message);

            TextBoxReader.AppendText(line + url);
            TextBoxReader.AppendText(Environment.NewLine);
            // Прокрутка TextBox вниз
            TextBoxReader.ScrollToCaret();

            ListBoxReader.Items.Add(line + ex.Message);
            // Прокрутка ListBox вниз
            ListBoxReader.TopIndex = ListBoxReader.Items.Count - 1;

            RichTextBoxReader.AppendText(line + ex.Message);
            RichTextBoxReader.AppendText(Environment.NewLine);
            // Прокрутка RichTextBox вниз
            RichTextBoxReader.ScrollToCaret();
         }
      }

      private void ButtonClear_Click(object sender, EventArgs e)
      {
         TextBoxReader.Clear();
         RichTextBoxReader.Clear();
         ListBoxReader.Items.Clear();
      }

      private void ButtonStop_Click(object sender, EventArgs e)
      {
         SyncServer.Stop();
      }
   }
}