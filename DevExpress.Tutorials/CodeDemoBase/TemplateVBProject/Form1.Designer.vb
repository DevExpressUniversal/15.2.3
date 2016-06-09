Namespace DxSample
	Partial Public Class Form1
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing AndAlso (components IsNot Nothing) Then
				components.Dispose()
			End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
			Me.//LowerName = New //NameSpace.//UpperName()
            Me.SuspendLayout()
            ' 
            ' 'LowerName
            ' 
            Me.//LowerName.Dock = System.Windows.Forms.DockStyle.Fill
            Me.//LowerName.Location = New System.Drawing.Point(0, 0)
            Me.//LowerName.Name = "//LowerName"
            Me.//LowerName.Size = New System.Drawing.Size(623, 411)
            Me.//LowerName.TabIndex = 0
            ' 
            ' Form1
            ' 
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(623, 411)
            Me.Controls.Add(Me.//LowerName)
            Me.Name = "Form1"
            Me.Text = "Form1"
            Me.ResumeLayout(False)

        End Sub

		#End Region

		Private //LowerName As //NameSpace.//UpperName

	End Class
End Namespace

