using System;

namespace SyncServerWinForms
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
}