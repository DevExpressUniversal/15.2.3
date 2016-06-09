#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views.WinForms.WinUI
{
	using System.Linq;
	using System.Text;
	using System.Collections.Generic;
	using DevExpress.Design.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
	using DevExpress.Xpf.Core.Native;
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class DocumentManagerView_WinUI : DocumentManagerView_WinUIBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	string viewName = templateInfo.Properties["ViewName"].ToString();
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace +"." + viewName; 
	DocumentManagerViewModelInfo viewModelData = templateInfo.Properties["IViewModelInfo"] as DocumentManagerViewModelInfo;
	string mvvmContextFullName = viewModelData.Namespace+"."+viewModelData.Name;
	bool IsVisualBasic = (bool)templateInfo.Properties["IsVisualBasic"];
if(!IsVisualBasic){
			this.Write("using System;\r\nusing System.Linq;\r\nusing System.Collections.Generic;\r\nusing DevEx" +
					"press.XtraEditors;\r\nusing DevExpress.XtraBars;\r\nusing DevExpress.Utils.MVVM.Serv" +
					"ices;\r\nusing DevExpress.Utils.MVVM;\r\nusing DevExpress.XtraBars.Navigation;\r\n\r\nna" +
					"mespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("{\r\n    public partial class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(" : XtraUserControl {\r\n        public ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"() {
		     InitializeComponent();
			 if(!mvvmContext.IsDesignMode)
                InitializeNavigation();
        }
        void InitializeNavigation() {
			DevExpress.XtraEditors.WindowsFormsSettings.SetDPIAware();
            DevExpress.XtraEditors.WindowsFormsSettings.EnableFormSkins();
            DevExpress.XtraEditors.WindowsFormsSettings.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
            DevExpress.XtraEditors.WindowsFormsSettings.TouchUIMode = DevExpress.LookAndFeel.TouchUIMode.True;
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(""Office 2013"");
            float fontSize = 10f;
            DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = new System.Drawing.Font(""Segoe UI"", fontSize);
            DevExpress.XtraEditors.WindowsFormsSettings.DefaultMenuFont = new System.Drawing.Font(""Segoe UI"", fontSize);

			tileBar.SelectionColorMode = SelectionColorMode.UseItemBackColor;
            mvvmContext.RegisterService(DocumentManagerService.Create(navigationFrame));
            DevExpress.Utils.MVVM.MVVMContext.RegisterFlyoutDialogService();
            // We want to use buttons in Ribbon to show the specific modules
            var fluentAPI = mvvmContext.OfType<");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(">();\r\n\t\t\t");
			int indexer = 0;
			foreach(var item in viewModelData.Tables){
				string nameForItem = "tileBarItem" + item.ViewName;
			if(indexer == 0){
			this.Write("\t\t\ttileBar.SelectedItem = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(";\r\n\r\n\t\t\t");
}
			this.Write("            fluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(", (x, m) => x.Show(m), x => x.Modules[");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("]);\r\n\t\t\t");
indexer++;}
			this.Write("\t\t\t");
			foreach(var item in viewModelData.Views){
				string nameForItem = "tileBarItem" + item.ViewName;
			this.Write("\t\t\tfluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(", (x, m) => x.Show(m), x => x.Modules[");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("]);\r\n\t\t\t");
indexer++;}
			this.Write("            // We want show the default module when our UserControl is loaded\r\n  " +
					"          fluentAPI.WithEvent<EventArgs>(this, \"Load\")\r\n                .EventTo" +
					"Command(x => x.OnLoaded(null), x => x.DefaultModule);\r\n\t\t\r\n        }\r\n    }\r\n}\r\n" +
					"");
}
if(IsVisualBasic){
			this.Write("Imports System\r\nImports System.Linq\r\nImports System.Collections.Generic\r\nImports " +
					"DevExpress.XtraEditors\r\nImports DevExpress.XtraBars\r\nImports DevExpress.Utils.MV" +
					"VM.Services\r\nImports DevExpress.XtraBars.Navigation\r\nNamespace Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\r\n\tPartial Public Class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
		Inherits XtraUserControl

		Public Sub New()
			InitializeComponent()
			If Not mvvmContext.IsDesignMode Then
				InitializeNavigation()
			End If
		End Sub
		Private Sub InitializeNavigation()
			DevExpress.XtraEditors.WindowsFormsSettings.SetDPIAware()
			DevExpress.XtraEditors.WindowsFormsSettings.EnableFormSkins()
			DevExpress.XtraEditors.WindowsFormsSettings.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True
			DevExpress.XtraEditors.WindowsFormsSettings.TouchUIMode = DevExpress.LookAndFeel.TouchUIMode.True
			DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(""Office 2013"")
			Dim fontSize As Single = 10F
			DevExpress.XtraEditors.WindowsFormsSettings.DefaultFont = New System.Drawing.Font(""Segoe UI"", fontSize)
			DevExpress.XtraEditors.WindowsFormsSettings.DefaultMenuFont = New System.Drawing.Font(""Segoe UI"", fontSize)

			tileBar.SelectionColorMode = SelectionColorMode.UseItemBackColor
            mvvmContext.RegisterService(DocumentManagerService.Create(navigationFrame))
            DevExpress.Utils.MVVM.MVVMContext.RegisterFlyoutDialogService()
            ' We want to use buttons in Ribbon to show the specific modules
            Dim fluentAPI = mvvmContext.OfType(Of Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(")()\r\n\t\t\t");
			int indexer = 0;
			foreach(var item in viewModelData.Tables){
				string nameForItem = "tileBarItem" + item.ViewName;
			if(indexer == 0){
			this.Write("\t\t\ttileBar.SelectedItem = ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write("\r\n\t\t\t");
}
			this.Write("            fluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(", Sub(x, m) x.Show(m), Function(x) x.Modules(");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("))\r\n\t\t\t");
indexer++;}
			this.Write("\t\t\t");
			foreach(var item in viewModelData.Views){
				string nameForItem = "tileBarItem" + item.ViewName;
			this.Write("\t\t\tfluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(", Sub(x, m) x.Show(m), Function(x) x.Modules(");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("))\r\n\t\t\t");
indexer++;}
			this.Write(@"            ' We want show the default module when our UserControl is loaded
            fluentAPI.WithEvent(Of EventArgs)(Me, ""Load"").EventToCommand(
                Sub(x) x.OnLoaded(Nothing), Function(x) x.DefaultModule)
		End Sub
	End Class
End Namespace

");
}
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerView_WinUIBase
	{
		#region Fields
		private global::System.Text.StringBuilder generationEnvironmentField;
		private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
		private global::System.Collections.Generic.List<int> indentLengthsField;
		private string currentIndentField = "";
		private bool endsWithNewline;
		private global::System.Collections.Generic.IDictionary<string, object> sessionField;
		#endregion
		#region Properties
		protected System.Text.StringBuilder GenerationEnvironment
		{
			get
			{
				if ((this.generationEnvironmentField == null))
				{
					this.generationEnvironmentField = new global::System.Text.StringBuilder();
				}
				return this.generationEnvironmentField;
			}
			set
			{
				this.generationEnvironmentField = value;
			}
		}
		public System.CodeDom.Compiler.CompilerErrorCollection Errors
		{
			get
			{
				if ((this.errorsField == null))
				{
					this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
				}
				return this.errorsField;
			}
		}
		private System.Collections.Generic.List<int> indentLengths
		{
			get
			{
				if ((this.indentLengthsField == null))
				{
					this.indentLengthsField = new global::System.Collections.Generic.List<int>();
				}
				return this.indentLengthsField;
			}
		}
		public string CurrentIndent
		{
			get
			{
				return this.currentIndentField;
			}
		}
		public virtual global::System.Collections.Generic.IDictionary<string, object> Session
		{
			get
			{
				return this.sessionField;
			}
			set
			{
				this.sessionField = value;
			}
		}
		#endregion
		#region Transform-time helpers
		public void Write(string textToAppend)
		{
			if (string.IsNullOrEmpty(textToAppend))
			{
				return;
			}
			if (((this.GenerationEnvironment.Length == 0) 
						|| this.endsWithNewline))
			{
				this.GenerationEnvironment.Append(this.currentIndentField);
				this.endsWithNewline = false;
			}
			if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
			{
				this.endsWithNewline = true;
			}
			if ((this.currentIndentField.Length == 0))
			{
				this.GenerationEnvironment.Append(textToAppend);
				return;
			}
			textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
			if (this.endsWithNewline)
			{
				this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
			}
			else
			{
				this.GenerationEnvironment.Append(textToAppend);
			}
		}
		public void WriteLine(string textToAppend)
		{
			this.Write(textToAppend);
			this.GenerationEnvironment.AppendLine();
			this.endsWithNewline = true;
		}
		public void Write(string format, params object[] args)
		{
			this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
		}
		public void WriteLine(string format, params object[] args)
		{
			this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
		}
		public void Error(string message)
		{
			System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
			error.ErrorText = message;
			this.Errors.Add(error);
		}
		public void Warning(string message)
		{
			System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
			error.ErrorText = message;
			error.IsWarning = true;
			this.Errors.Add(error);
		}
		public void PushIndent(string indent)
		{
			if ((indent == null))
			{
				throw new global::System.ArgumentNullException("indent");
			}
			this.currentIndentField = (this.currentIndentField + indent);
			this.indentLengths.Add(indent.Length);
		}
		public string PopIndent()
		{
			string returnValue = "";
			if ((this.indentLengths.Count > 0))
			{
				int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
				this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
				if ((indentLength > 0))
				{
					returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
					this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
				}
			}
			return returnValue;
		}
		public void ClearIndent()
		{
			this.indentLengths.Clear();
			this.currentIndentField = "";
		}
		#endregion
		#region ToString Helpers
		public class ToStringInstanceHelper
		{
			private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
			public System.IFormatProvider FormatProvider
			{
				get
				{
					return this.formatProviderField ;
				}
				set
				{
					if ((value != null))
					{
						this.formatProviderField  = value;
					}
				}
			}
			public string ToStringWithCulture(object objectToConvert)
			{
				if ((objectToConvert == null))
				{
					throw new global::System.ArgumentNullException("objectToConvert");
				}
				System.Type t = objectToConvert.GetType();
				System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
							typeof(System.IFormatProvider)});
				if ((method == null))
				{
					return objectToConvert.ToString();
				}
				else
				{
					return ((string)(method.Invoke(objectToConvert, new object[] {
								this.formatProviderField })));
				}
			}
		}
		private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
		public ToStringInstanceHelper ToStringHelper
		{
			get
			{
				return this.toStringHelperField;
			}
		}
		#endregion
	}
	#endregion
}
