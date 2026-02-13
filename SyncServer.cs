using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SyncServerWinForms
{
   public class SyncServer
   {
      public static HttpListener Listener;
      public string Url;
      public static readonly List<Item> Items = new List<Item>();

      public SyncServer(string url)
      {
         Url = url;
         Listener = new HttpListener();
         Listener.Prefixes.Add(url);
      }

      public static void Start()
      {
         Listener.Start();

         string lineone = "Проверка связи";

         System.Windows.Forms.TextBoxReader;

         //TextBoxReader.AppendText(lineone);
         //TextBoxReader.AppendText(Environment.NewLine);
         // Прокрутка TextBox вниз
         //TextBoxReader.ScrollToCaret();

         //textBoxReader = textBoxReader.AppendText(lineone);
         //TextBoxReader.AppendText(Environment.NewLine);
         // Прокрутка TextBox вниз
         //TextBoxReader.ScrollToCaret();

         //Console.WriteLine("Синхронный Json сервер");
         //Console.WriteLine("Сервер запущен по адресу {0}", _url);

         while (true)
         {
            try
            {
               HttpListenerContext context = Listener.GetContext();
               ProcessRequest(context);
            }
            catch (Exception ex)
            {
               //Console.WriteLine("Ошибка: {0}", ex.Message);
            }
         }
      }

      public static void ProcessRequest(HttpListenerContext context)
      {
         HttpListenerRequest request = context.Request;
         HttpListenerResponse response = context.Response;
         try
         {
            //Console.WriteLine("{0} {1}", request.HttpMethod, request.Url.AbsolutePath);
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

      public static void HandleGet(HttpListenerRequest request, HttpListenerResponse response)
      {
         string path = request.Url.AbsolutePath.Trim('/');
         if (string.IsNullOrEmpty(path) || path == "api/items")
         {
            SendResponse(response, 200, Items);
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

               Item item = Items.Find(Match);
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

      public static void HandlePost(HttpListenerRequest request, HttpListenerResponse response)
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

            Items.Add(newItem);
            SendResponse(response, 201, newItem);
         }
      }

      public static void HandlePut(HttpListenerRequest request, HttpListenerResponse response)
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

         Item existingItem = Items.Find(Match);
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

      public static void HandleDelete(HttpListenerRequest request, HttpListenerResponse response)
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

         Item item = Items.Find(Match);
         if (item == null)
         {
            SendResponse(response, 404, new { error = "Товар не найден" });
            return;
         }

         Items.Remove(item);
         SendResponse(response, 200, new { message = "Элемент удален" });
      }

      public static void SendResponse(HttpListenerResponse response, int statusCode, object data)
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
         Listener.Stop();
         Listener.Close();
      }
   }
}