using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SyncServerWinForms
{
   public class SyncServer
   {
      private readonly HttpListener _listener;
      private readonly string _url;
      private readonly List<Item> _items = new List<Item>();

      public SyncServer(string url)
      {
         _url = url;
         _listener = new HttpListener();
         _listener.Prefixes.Add(url);
      }

      // Запись логов
      private void Savelog(string tolog, Color color, TextBox textBoxReader, ListBox listBoxReader, RichTextBox richTextBoxReader)
      {
         // TextBox
         // Добавляет строку оставляя предыдущие
         //TextBoxReader.AppendText(line);
         //TextBoxReader.AppendText(Environment.NewLine);
         // Прокрутка TextBox вниз
         //TextBoxReader.ScrollToCaret();

         //textBoxReader.SelectionColor = color;
         textBoxReader.AppendText(tolog);
         textBoxReader.ScrollToCaret();

         // ListBox
         //ListBoxReader.Items.Add(line);
         // Прокрутка ListBox вниз
         //ListBoxReader.TopIndex = ListBoxReader.Items.Count - 1;

         // RichTextBox
         //RichTextBoxReader.AppendText(line);
         //RichTextBoxReader.AppendText(Environment.NewLine);
         // Прокрутка RichTextBox вниз
         //RichTextBoxReader.ScrollToCaret();

         richTextBoxReader.SelectionColor = color;
         richTextBoxReader.AppendText(tolog);
         richTextBoxReader.ScrollToCaret();



         richTextBoxReader.SelectionColor = color;
         listBoxReader.AppendText(tolog);
         textBoxReader.ScrollToCaret();



      }

      public void Start()
      {
         _listener.Start();
         while (true)
         {
            try
            {
               HttpListenerContext context = _listener.GetContext();
               ProcessRequest(context);
            }
            catch (Exception ex)
            {
               //Console.WriteLine("Ошибка: {0}", ex.Message);
            }
         }
      }

      private void ProcessRequest(HttpListenerContext context)
      {
         HttpListenerRequest request = context.Request;
         HttpListenerResponse response = context.Response;
         try
         {
            Console.WriteLine("{0} {1}", request.HttpMethod, request.Url.AbsolutePath);
            if (request.HttpMethod == "GET")
            {
               HandleGet(request, response);
            }
            else if (request.HttpMethod == "POST")
            {
               HandlePost(request, response);
            }
            else if (request.HttpMethod == "PUT")
            {
               HandlePut(request, response);
            }
            else if (request.HttpMethod == "DELETE")
            {
               HandleDelete(request, response);
            }
            else
            {
               SendResponse(response, 405, new { error = "Метод не разрешен" });
            }
         }
         catch (Exception ex)
         {
            SendResponse(response, 500, new { error = ex.Message });
         }
         finally
         {
            response.Close();
         }
      }

      private void HandleGet(HttpListenerRequest request, HttpListenerResponse response)
      {
         string path = request.Url.AbsolutePath.Trim('/');
         if (string.IsNullOrEmpty(path) || path == "api/items")
         {
            SendResponse(response, 200, _items);
            return;
         }

         if (path.StartsWith("api/items/"))
         {
            string idStr = path.Substring("api/items/".Length);
            if (int.TryParse(idStr, out int id))
            {
               bool Match(Item i)
               {
                  return i.Id == id;
               }

               Item item = _items.Find(Match);
               if (item != null)
               {
                  SendResponse(response, 200, item);
               }
               else
               {
                  SendResponse(response, 404, new { error = "Товар не найден" });
               }
            }
            else
            {
               SendResponse(response, 400, new { error = "Неверный идентификатор ID" });
            }

            return;
         }

         SendResponse(response, 404, new { error = "Не найдено" });
      }

      private void HandlePost(HttpListenerRequest request, HttpListenerResponse response)
      {
         if (request.Url.AbsolutePath.Trim('/') != "api/items")
         {
            SendResponse(response, 404, new { error = "Не найдено" });
            return;
         }

         using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
         {

            string body = reader.ReadToEnd();
            Item newItem = JsonConvert.DeserializeObject<Item>(body);
            if (newItem == null || string.IsNullOrEmpty(newItem.Name))
            {
               SendResponse(response, 400, new { error = "Недопустимые данные товара" });
               return;
            }

            _items.Add(newItem);
            SendResponse(response, 201, newItem);
         }
      }

      private void HandlePut(HttpListenerRequest request, HttpListenerResponse response)
      {
         string path = request.Url.AbsolutePath.Trim('/');
         if (!path.StartsWith("api/items/"))
         {
            SendResponse(response, 404, new { error = "Не найдено" });
            return;
         }

         string idStr = path.Substring("api/items/".Length);
         if (!int.TryParse(idStr, out int id))
         {
            SendResponse(response, 400, new { error = "Неверный идентификатор ID" });
            return;
         }

         bool Match(Item i)
         {
            return i.Id == id;
         }

         Item existingItem = _items.Find(Match);
         if (existingItem == null)
         {
            SendResponse(response, 404, new { error = "Товар не найден" });
            return;
         }

         using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
         {
            string body = reader.ReadToEnd();
            Item updatedItem = JsonConvert.DeserializeObject<Item>(body);
            if (updatedItem == null || string.IsNullOrEmpty(updatedItem.Name))
            {
               SendResponse(response, 400, new { error = "Недопустимые данные товара" });
               return;
            }
            existingItem.Date = updatedItem.Date;
            existingItem.Timestamp = updatedItem.Timestamp;
            existingItem.Id = updatedItem.Id;
            existingItem.Vendor = updatedItem.Vendor;
            existingItem.Name = updatedItem.Name;
            existingItem.Price = updatedItem.Price;
            SendResponse(response, 200, existingItem);
         }
      }

      private void HandleDelete(HttpListenerRequest request, HttpListenerResponse response)
      {
         string path = request.Url.AbsolutePath.Trim('/');
         if (!path.StartsWith("api/items/"))
         {
            SendResponse(response, 404, new { error = "Не найдено" });
            return;
         }

         string idStr = path.Substring("api/items/".Length);
         if (!int.TryParse(idStr, out int id))
         {
            SendResponse(response, 400, new { error = "Неверный идентификатор ID" });
            return;
         }

         bool Match(Item i)
         {
            return i.Id == id;
         }

         Item item = _items.Find(Match);
         if (item == null)
         {
            SendResponse(response, 404, new { error = "Товар не найден" });
            return;
         }

         _items.Remove(item);
         SendResponse(response, 200, new { message = "Элемент удален" });
      }

      private void SendResponse(HttpListenerResponse response, int statusCode, object data)
      {
         string json = JsonConvert.SerializeObject(data, Formatting.Indented);
         byte[] buffer = Encoding.UTF8.GetBytes(json);
         response.StatusCode = statusCode;
         response.ContentType = "application/json";
         response.ContentLength64 = buffer.Length;
         response.ContentEncoding = Encoding.UTF8;
         response.OutputStream.Write(buffer, 0, buffer.Length);
      }

      public void Stop()
      {
         _listener.Stop();
         _listener.Close();
      }
   }
}