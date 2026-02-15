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
      private readonly TextBox _textBoxReader;
      private readonly ListBox _listBoxReader;
      private readonly RichTextBox _richTextBoxReader;
      private readonly List<Item> _items = new List<Item>();

      public SyncServer(string url, TextBox textBoxReader, ListBox listBoxReader, RichTextBox richTextBoxReader)
      {
         _textBoxReader = textBoxReader;
         _listBoxReader = listBoxReader;
         _richTextBoxReader = richTextBoxReader;
         _listener = new HttpListener();
         _listener.Prefixes.Add(url);
      }

      // Запись логов
      private void Savelog(string tolog, Color color)
      {
         // TextBox
         // Добавляет строку оставляя предыдущие
         //_textBoxReader.AppendText(tolog);
         //_textBoxReader.AppendText(Environment.NewLine);
         // Прокрутка TextBox вниз
         //_textBoxReader.ScrollToCaret();

         //_textBoxReader.SelectionColor = color;
         _textBoxReader.AppendText(tolog);
         _textBoxReader.ScrollToCaret();

         // ListBox
         _listBoxReader.Items.Add(tolog);
         // Прокрутка ListBox вниз
         _listBoxReader.TopIndex = _listBoxReader.Items.Count - 1;

         // RichTextBox
         _richTextBoxReader.SelectionColor = color;
         _richTextBoxReader.AppendText(tolog);
         _richTextBoxReader.AppendText(Environment.NewLine);
         // Прокрутка RichTextBox вниз
         _richTextBoxReader.ScrollToCaret();
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
               _textBoxReader.AppendText("Старт"); //
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