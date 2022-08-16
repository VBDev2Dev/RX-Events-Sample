<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmRX
    Inherits RXForm



    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblCoordinates = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblCoordinates
        '
        Me.lblCoordinates.AutoSize = True
        Me.lblCoordinates.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblCoordinates.Location = New System.Drawing.Point(0, 0)
        Me.lblCoordinates.Name = "lblCoordinates"
        Me.lblCoordinates.Size = New System.Drawing.Size(0, 15)
        Me.lblCoordinates.TabIndex = 0
        '
        'frmRX
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 61)
        Me.Controls.Add(Me.lblCoordinates)
        Me.Name = "frmRX"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblCoordinates As Label
End Class
