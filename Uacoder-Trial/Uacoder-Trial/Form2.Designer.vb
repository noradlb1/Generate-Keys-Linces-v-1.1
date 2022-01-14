<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
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
        Me.lblApplicationStatus = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblApplicationStatus
        '
        Me.lblApplicationStatus.BackColor = System.Drawing.SystemColors.Control
        Me.lblApplicationStatus.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.lblApplicationStatus.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblApplicationStatus.ForeColor = System.Drawing.Color.Black
        Me.lblApplicationStatus.Location = New System.Drawing.Point(12, 9)
        Me.lblApplicationStatus.Name = "lblApplicationStatus"
        Me.lblApplicationStatus.Size = New System.Drawing.Size(341, 30)
        Me.lblApplicationStatus.TabIndex = 15
        Me.lblApplicationStatus.Text = "Unregistered"
        '
        'Form2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(365, 262)
        Me.Controls.Add(Me.lblApplicationStatus)
        Me.Name = "Form2"
        Me.Text = "Form2"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lblApplicationStatus As System.Windows.Forms.Label
End Class
