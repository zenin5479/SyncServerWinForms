namespace SyncServerWinForms
{
   partial class FormOne
   {
      /// <summary>
      ///  Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      ///  Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      ///  Required method for Designer support - do not modify
      ///  the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         ButtonStart = new System.Windows.Forms.Button();
         SuspendLayout();
         // 
         // ButtonStart
         // 
         ButtonStart.Location = new System.Drawing.Point(12, 12);
         ButtonStart.Name = "ButtonStart";
         ButtonStart.Size = new System.Drawing.Size(110, 23);
         ButtonStart.TabIndex = 0;
         ButtonStart.Text = "Запуск сервера";
         ButtonStart.UseVisualStyleBackColor = true;
         ButtonStart.Click += ButtonStart_Click;
         // 
         // FormOne
         // 
         AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
         AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         ClientSize = new System.Drawing.Size(684, 461);
         Controls.Add(ButtonStart);
         Name = "FormOne";
         StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         Text = "Синхронный Json сервер Windows Forms";
         ResumeLayout(false);
      }

      #endregion

      private System.Windows.Forms.Button ButtonStart;
   }
}
