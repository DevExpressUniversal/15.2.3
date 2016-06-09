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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Views.WinForms.Outlook
{
	using System.Linq;
	using System.Text;
	using System.Collections.Generic;
	using DevExpress.Design.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
	using DevExpress.Xpf.Core.Native;
	using DevExpress.Mvvm.UI.Native.ViewGenerator;
	using DevExpress.Entity.Model;
	using DevExpress.Mvvm.Native;
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class CollectionView_Outlook : CollectionView_OutlookBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	CollectionViewModelData viewModelData = templateInfo.Properties["IViewModelInfo"] as CollectionViewModelData;
	UIType uiType = (UIType)templateInfo.Properties["UIType"];
	string viewName = templateInfo.Properties["ViewName"].ToString();	
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace +"." + viewName;
	string mvvmContextFullName = viewModelData.Namespace+"."+viewModelData.Name;
	string bindingSourceName = Char.ToLowerInvariant(viewName[0]) + viewName.Substring(1) + "BindingSource";
	bool IsVisualBasic = (bool)templateInfo.Properties["IsVisualBasic"];
if(!IsVisualBasic){
			this.Write(@"using System;
using System.Linq;
using System.Collections.Generic;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.Utils.MVVM.Services;
using DevExpress.XtraBars;

namespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("{\r\n    public partial class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(" : XtraUserControl {\r\n        public ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"() {
            InitializeComponent();
			if(!mvvmContext.IsDesignMode)
                InitBindings();
        }
        void InitBindings() {
			mvvmContext.RegisterService(WindowedDocumentManagerService.Create(this));
            var fluentAPI = mvvmContext.OfType<");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(">();\r\n\t\t\tfluentAPI.WithEvent(this, \"Load\").EventToCommand(x => x.OnLoaded());\r\n  " +
					"          // We want to show the ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.CollectionPropertyName));
			this.Write(" collection in grid and react on this collection external changes (Reload, server" +
					"-side Filtering)\r\n            fluentAPI.SetBinding(gridControl, gControl => gCon" +
					"trol.DataSource, x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.CollectionPropertyName));
			this.Write(");\r\n\t\t\t// We want to show loading-indicator when data is loading asynchronously\r\n" +
					"            fluentAPI.SetBinding(gridView, gView => gView.LoadingPanelVisible, x" +
					" => x.IsLoading);\r\n\t\t\t");
			if(viewModelData.HasEntityEditProperty()){
			this.Write(@"			// We want to proceed the Edit command when row double-clicked
            fluentAPI.WithEvent<RowClickEventArgs>(gridView, ""RowClick"").EventToCommand(
                    x => x.Edit(null),
					x => x.SelectedEntity,
                    args => (args.Clicks == 2) && (args.Button == System.Windows.Forms.MouseButtons.Left));
			");
}
			this.Write(@"			// We want to synchronize the ViewModel.SelectedEntity and the GridView.FocusedRowRandle in two-way manner
            fluentAPI.WithEvent<GridView, FocusedRowObjectChangedEventArgs>(gridView, ""FocusedRowObjectChanged"")
                .SetBinding(x => x.SelectedEntity,
                    args => args.Row as ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeFullName));
			this.Write(@",
                    (gView, entity) => gView.FocusedRowHandle = gView.FindRow(entity));
			//We want to show ribbon print preview when bbiPrintPreview clicked
			bbiPrintPreview.ItemClick += (s, e) => { gridControl.ShowRibbonPrintPreview(); };
			//We want to show RECORDS count on BarStaticItem
			fluentAPI.SetBinding(bsiRecordsCount, item => item.Caption,	x => x.Entities.Count, 
					count => string.Format(""RECORDS : {0}"", count));
			//We want to show PopupMenu when row clicked by right button
			gridView.RowClick += (s, e) => {
                if(e.Clicks == 1 && e.Button == System.Windows.Forms.MouseButtons.Right) {
                    popupMenu.ShowPopup(gridControl.PointToScreen(e.Location), s);
                }
            };
        }
    }
}
");
}
if(IsVisualBasic){
			this.Write(@"Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraEditors
Imports DevExpress.Utils.MVVM.Services
Imports DevExpress.XtraBars

Namespace Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\r\n\tPartial Public Class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(@"
		Inherits XtraUserControl

		Public Sub New()
			InitializeComponent()
			If Not mvvmContext.IsDesignMode Then
				InitBindings()
			End If
		End Sub
		Private Sub InitBindings()
			mvvmContext.RegisterService(WindowedDocumentManagerService.Create(Me))
			fluentAPI.WithEvent(Me, ""Load"").EventToCommand(Sub(x) x.OnLoaded())
			Dim fluentAPI = mvvmContext.OfType(Of Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(")()\r\n\t\t\t\' We want to show the ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.CollectionPropertyName));
			this.Write(" collection in grid and react on this collection external changes (Reload, server" +
					"-side Filtering)\r\n            fluentAPI.SetBinding(gridControl, Function(gContro" +
					"l) gControl.DataSource, Function(x) x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.CollectionPropertyName));
			this.Write(")\r\n\t\t\t\' We want to show loading-indicator when data is loading asynchronously\r\n  " +
					"          fluentAPI.SetBinding(gridView, Function(gView) gView.LoadingPanelVisib" +
					"le, Function(x) x.IsLoading)\r\n\t\t\t");
			if(viewModelData.HasEntityEditProperty()){
			this.Write(@"			' We want to proceed the Edit command when row double-clicked
            fluentAPI.WithEvent(Of RowClickEventArgs)(gridView, ""RowClick"").EventToCommand(
                Sub(x) x.Edit(Nothing),
                Function(x) x.SelectedEntity,
                Function(args) (args.Clicks = 2) AndAlso (args.Button = System.Windows.Forms.MouseButtons.Left))
			");
}
			this.Write(@"			' We want to synchronize the ViewModel.SelectedEntity and the GridView.FocusedRowRandle in two-way manner
            fluentAPI.WithEvent(Of GridView, FocusedRowObjectChangedEventArgs)(gridView, ""FocusedRowObjectChanged"").SetBinding(
                Function(x) x.SelectedEntity,
                Function(args) TryCast(args.Row, ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityTypeFullName));
			this.Write(@"),
                Sub(gView, entity) gView.FocusedRowHandle = gView.FindRow(entity))
            'We want to show ribbon print preview when bbiPrintPreview clicked
            AddHandler bbiPrintPreview.ItemClick, Sub(s, e) gridControl.ShowRibbonPrintPreview()
			'We want to show RECORDS count on BarStaticItem
			fluentAPI.SetBinding(bsiRecordsCount, Function(item) item.Caption, Function(x) x.Entities.Count, 
                                 Function(count) String.Format(""RECORDS : {0}"", count))
			'We want to show PopupMenu when row clicked by right button
			AddHandler gridView.RowClick, Sub(s, e)
											   If e.Clicks = 1 AndAlso e.Button = System.Windows.Forms.MouseButtons.Right Then
											   	popupMenu.ShowPopup(gridControl.PointToScreen(e.Location), s)
											   End If
										  End Sub
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
	public class CollectionView_OutlookBase
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
