namespace documentsParser
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.open_button = new System.Windows.Forms.Button();
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.replace_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // open_button
            // 
            this.open_button.Location = new System.Drawing.Point(382, 13);
            this.open_button.Name = "open_button";
            this.open_button.Size = new System.Drawing.Size(133, 23);
            this.open_button.TabIndex = 0;
            this.open_button.Text = "Открыть документ";
            this.open_button.UseVisualStyleBackColor = true;
            this.open_button.Click += new System.EventHandler(this.open_button_Click);
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(13, 42);
            this.logBox.Name = "logBox";
            this.logBox.Size = new System.Drawing.Size(317, 399);
            this.logBox.TabIndex = 1;
            this.logBox.Text = "";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 13);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(317, 23);
            this.progressBar.TabIndex = 2;
            // 
            // replace_button
            // 
            this.replace_button.Enabled = false;
            this.replace_button.Location = new System.Drawing.Point(382, 43);
            this.replace_button.Name = "replace_button";
            this.replace_button.Size = new System.Drawing.Size(133, 23);
            this.replace_button.TabIndex = 3;
            this.replace_button.Text = "Заменить";
            this.replace_button.UseVisualStyleBackColor = true;
            this.replace_button.Click += new System.EventHandler(this.replace_button_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 453);
            this.Controls.Add(this.replace_button);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.open_button);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Парсер";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button open_button;
        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button replace_button;
    }
}

