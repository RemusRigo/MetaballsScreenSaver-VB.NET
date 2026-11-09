<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCfg
   Inherits System.Windows.Forms.Form

   'Form overrides dispose to clean up the component list.
   <System.Diagnostics.DebuggerNonUserCode()> _
   Protected Overrides Sub Dispose(ByVal disposing As Boolean)
      Try
         If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
         End If
      Finally
         MyBase.Dispose(disposing)
      End Try
   End Sub

   'Required by the Windows Form Designer
   Private components As System.ComponentModel.IContainer

   'NOTE: The following procedure is required by the Windows Form Designer
   'It can be modified using the Windows Form Designer.  
   'Do not modify it using the code editor.
   <System.Diagnostics.DebuggerStepThrough()> _
   Private Sub InitializeComponent()
      lblColorScheme = New Label()
      rBtnBlue = New RadioButton()
      rBtnRed = New RadioButton()
      lblBlobs = New Label()
      cbBlobs = New ComboBox()
      btnOk = New Button()
      btnCancel = New Button()
      tbSpeed = New TrackBar()
      lblSpeed = New Label()
      lblRadius = New Label()
      tbRadius = New TrackBar()
      lblGlowIntensity = New Label()
      tbGlowIntensity = New TrackBar()
      pnlPreview = New Panel()
      CType(tbSpeed, ComponentModel.ISupportInitialize).BeginInit()
      CType(tbRadius, ComponentModel.ISupportInitialize).BeginInit()
      CType(tbGlowIntensity, ComponentModel.ISupportInitialize).BeginInit()
      SuspendLayout()
      ' 
      ' lblColorScheme
      ' 
      lblColorScheme.AutoSize = True
      lblColorScheme.Location = New Point(12, 9)
      lblColorScheme.Name = "lblColorScheme"
      lblColorScheme.Size = New Size(83, 15)
      lblColorScheme.TabIndex = 0
      lblColorScheme.Text = "Color scheme:"
      ' 
      ' rBtnBlue
      ' 
      rBtnBlue.AutoSize = True
      rBtnBlue.Location = New Point(12, 27)
      rBtnBlue.Name = "rBtnBlue"
      rBtnBlue.Size = New Size(48, 19)
      rBtnBlue.TabIndex = 1
      rBtnBlue.TabStop = True
      rBtnBlue.Text = "Blue"
      rBtnBlue.UseVisualStyleBackColor = True
      ' 
      ' rBtnRed
      ' 
      rBtnRed.AutoSize = True
      rBtnRed.Location = New Point(12, 52)
      rBtnRed.Name = "rBtnRed"
      rBtnRed.Size = New Size(45, 19)
      rBtnRed.TabIndex = 2
      rBtnRed.TabStop = True
      rBtnRed.Text = "Red"
      rBtnRed.UseVisualStyleBackColor = True
      ' 
      ' lblBlobs
      ' 
      lblBlobs.AutoSize = True
      lblBlobs.Location = New Point(142, 9)
      lblBlobs.Name = "lblBlobs"
      lblBlobs.Size = New Size(100, 15)
      lblBlobs.TabIndex = 3
      lblBlobs.Text = "Number of blobs:"
      ' 
      ' cbBlobs
      ' 
      cbBlobs.DropDownStyle = ComboBoxStyle.DropDownList
      cbBlobs.FormattingEnabled = True
      cbBlobs.Location = New Point(142, 26)
      cbBlobs.Name = "cbBlobs"
      cbBlobs.Size = New Size(121, 23)
      cbBlobs.TabIndex = 4
      ' 
      ' btnOk
      ' 
      btnOk.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
      btnOk.DialogResult = DialogResult.OK
      btnOk.Location = New Point(389, 442)
      btnOk.Name = "btnOk"
      btnOk.Size = New Size(75, 23)
      btnOk.TabIndex = 5
      btnOk.Text = "Ok"
      btnOk.UseVisualStyleBackColor = True
      ' 
      ' btnCancel
      ' 
      btnCancel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
      btnCancel.DialogResult = DialogResult.Cancel
      btnCancel.Location = New Point(470, 442)
      btnCancel.Name = "btnCancel"
      btnCancel.Size = New Size(75, 23)
      btnCancel.TabIndex = 6
      btnCancel.Text = "Cancel"
      btnCancel.UseVisualStyleBackColor = True
      ' 
      ' tbSpeed
      ' 
      tbSpeed.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
      tbSpeed.LargeChange = 25
      tbSpeed.Location = New Point(12, 110)
      tbSpeed.Maximum = 300
      tbSpeed.Minimum = 25
      tbSpeed.Name = "tbSpeed"
      tbSpeed.Size = New Size(533, 45)
      tbSpeed.SmallChange = 5
      tbSpeed.TabIndex = 7
      tbSpeed.TickFrequency = 25
      tbSpeed.Value = 25
      ' 
      ' lblSpeed
      ' 
      lblSpeed.AutoSize = True
      lblSpeed.Location = New Point(12, 92)
      lblSpeed.Name = "lblSpeed"
      lblSpeed.Size = New Size(42, 15)
      lblSpeed.TabIndex = 8
      lblSpeed.Text = "Speed:"
      ' 
      ' lblRadius
      ' 
      lblRadius.AutoSize = True
      lblRadius.Location = New Point(12, 142)
      lblRadius.Name = "lblRadius"
      lblRadius.Size = New Size(45, 15)
      lblRadius.TabIndex = 10
      lblRadius.Text = "Radius:"
      ' 
      ' tbRadius
      ' 
      tbRadius.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
      tbRadius.LargeChange = 25
      tbRadius.Location = New Point(12, 160)
      tbRadius.Maximum = 200
      tbRadius.Minimum = 50
      tbRadius.Name = "tbRadius"
      tbRadius.Size = New Size(533, 45)
      tbRadius.SmallChange = 5
      tbRadius.TabIndex = 9
      tbRadius.TickFrequency = 25
      tbRadius.Value = 50
      ' 
      ' lblGlowIntensity
      ' 
      lblGlowIntensity.AutoSize = True
      lblGlowIntensity.Location = New Point(12, 192)
      lblGlowIntensity.Name = "lblGlowIntensity"
      lblGlowIntensity.Size = New Size(85, 15)
      lblGlowIntensity.TabIndex = 12
      lblGlowIntensity.Text = "Glow Intensity:"
      ' 
      ' tbGlowIntensity
      ' 
      tbGlowIntensity.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
      tbGlowIntensity.LargeChange = 25
      tbGlowIntensity.Location = New Point(12, 210)
      tbGlowIntensity.Maximum = 300
      tbGlowIntensity.Minimum = 25
      tbGlowIntensity.Name = "tbGlowIntensity"
      tbGlowIntensity.Size = New Size(533, 45)
      tbGlowIntensity.SmallChange = 5
      tbGlowIntensity.TabIndex = 11
      tbGlowIntensity.TickFrequency = 25
      tbGlowIntensity.Value = 25
      ' 
      ' pnlPreview
      ' 
      pnlPreview.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
      pnlPreview.BackColor = Color.Black
      pnlPreview.BorderStyle = BorderStyle.FixedSingle
      pnlPreview.Location = New Point(12, 261)
      pnlPreview.Name = "pnlPreview"
      pnlPreview.Size = New Size(533, 175)
      pnlPreview.TabIndex = 13
      ' 
      ' frmCfg
      ' 
      AcceptButton = btnOk
      CancelButton = btnCancel
      ClientSize = New Size(547, 468)
      Controls.Add(pnlPreview)
      Controls.Add(lblGlowIntensity)
      Controls.Add(tbGlowIntensity)
      Controls.Add(lblRadius)
      Controls.Add(tbRadius)
      Controls.Add(lblSpeed)
      Controls.Add(tbSpeed)
      Controls.Add(btnCancel)
      Controls.Add(btnOk)
      Controls.Add(cbBlobs)
      Controls.Add(lblBlobs)
      Controls.Add(rBtnRed)
      Controls.Add(rBtnBlue)
      Controls.Add(lblColorScheme)
      FormBorderStyle = FormBorderStyle.FixedToolWindow
      MaximizeBox = False
      MinimizeBox = False
      Name = "frmCfg"
      StartPosition = FormStartPosition.CenterScreen
      Text = "Settings"
      CType(tbSpeed, ComponentModel.ISupportInitialize).EndInit()
      CType(tbRadius, ComponentModel.ISupportInitialize).EndInit()
      CType(tbGlowIntensity, ComponentModel.ISupportInitialize).EndInit()
      ResumeLayout(False)
      PerformLayout()

   End Sub

   Friend WithEvents lblColorScheme As Label
   Friend WithEvents rBtnBlue As RadioButton
   Friend WithEvents rBtnRed As RadioButton
   Friend WithEvents lblBlobs As Label
   Friend WithEvents cbBlobs As ComboBox
   Friend WithEvents btnOk As Button
   Friend WithEvents btnCancel As Button
   Friend WithEvents tbSpeed As TrackBar
   Friend WithEvents lblSpeed As Label
   Friend WithEvents lblRadius As Label
   Friend WithEvents tbRadius As TrackBar
   Friend WithEvents lblGlowIntensity As Label
   Friend WithEvents tbGlowIntensity As TrackBar
   Friend WithEvents pnlPreview As Panel

End Class
