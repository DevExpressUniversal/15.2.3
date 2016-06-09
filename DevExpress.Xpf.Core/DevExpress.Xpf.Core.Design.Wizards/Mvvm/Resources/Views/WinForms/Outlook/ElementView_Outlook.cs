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
	using System.Data.Entity.Design.PluralizationServices;
	using System.Globalization;
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class ElementView_Outlook : ElementView_OutlookBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	EntityViewModelData viewModelData = templateInfo.Properties["IViewModelInfo"] as EntityViewModelData;
	string viewName = templateInfo.Properties["ViewName"].ToString();
	string localNamespace = templateInfo.Properties["Namespace"].ToString();
	string viewFullName = localNamespace +"." + viewName;
	string mvvmContextFullName = viewModelData.Namespace+"."+viewModelData.Name;
	string bindingSourceName = Char.ToLowerInvariant(viewName[0]) + viewName.Substring(1) + "BindingSource";
List<PropertyEditorInfo> listLookUpInfo = templateInfo.Properties["GeneratedLookups"] as List<PropertyEditorInfo>;
	bool IsVisualBasic = (bool)templateInfo.Properties["IsVisualBasic"];
if(!IsVisualBasic){
			this.Write(@"using System;
using System.Linq;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.Utils.MVVM;
using DevExpress.Utils.MVVM.Services;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;

namespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("{\r\n    public partial class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(" : XtraUserControl {\r\n        public ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("() {\r\n            InitializeComponent();\r\n\t\t\tif(!mvvmContext.IsDesignMode)\r\n\t\t\t\tI" +
					"nitBindings();\r\n\t\t}\r\n\t\tvoid InitBindings() {\r\n\t\t    var fluentAPI = mvvmContext." +
					"OfType<");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(">();\r\n\t\t\tfluentAPI.WithEvent(this, \"Load\").EventToCommand(x => x.OnLoaded());\r\n  " +
					"          fluentAPI.SetObjectDataSourceBinding(\r\n                ");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(", x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityPropertyName));
			this.Write(", x => x.Update());\r\n\t\t\t");
foreach(var lookUpTable in viewModelData.LookUpTables){
				string nameForGridControl = lookUpTable.LookUpCollectionPropertyAssociationName +"GridControl";
				string nameForGridView = lookUpTable.LookUpCollectionPropertyAssociationName +"GridView";
				string nameForMVVMContext = lookUpTable.LookUpCollectionPropertyAssociationName +"MVVMContext";
				string nameForPopUpMenu = lookUpTable.LookUpCollectionPropertyAssociationName +"PopUpMenu";
				string lookUpPropertyName = GetLookUpPropertyName(viewModelData,lookUpTable);
			this.Write("\t\t\t#region ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write(" Detail Collection\r\n\t\t\t// We want to synchronize the ViewModel.SelectedEntity and" +
					" the GridView.FocusedRowRandle in two-way manner\r\n            fluentAPI.WithEven" +
					"t<GridView, FocusedRowObjectChangedEventArgs>(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(", \"FocusedRowObjectChanged\")\r\n                .SetBinding(x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".SelectedEntity,\r\n                    args => args.Row as ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.EntityTypeFullName));
			this.Write(",\r\n                    (gView, entity) => gView.FocusedRowHandle = gView.FindRow(" +
					"entity));\r\n\t\t\t");
			if(lookUpTable.HasEntityEditProperty()){
			this.Write("\t\t\t// We want to proceed the Edit command when row double-clicked\r\n\t\t\tfluentAPI.W" +
					"ithEvent<RowClickEventArgs>(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(", \"RowClick\")\r\n\t\t\t\t\t\t.EventToCommand(\r\n\t\t\t\t\t\t    x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".Edit(null), x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".SelectedEntity,\r\n\t\t\t\t\t\t    args => (args.Clicks == 2) && (args.Button == System." +
					"Windows.Forms.MouseButtons.Left));\r\n\t\t\t");
}
			this.Write("\t\t\t//We want to show PopupMenu when row clicked by right button\r\n\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".RowClick += (s, e) => {\r\n                if(e.Clicks == 1 && e.Button == System." +
					"Windows.Forms.MouseButtons.Right) {\r\n                    ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(".ShowPopup(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".PointToScreen(e.Location), s);\r\n                }\r\n            };\r\n\t\t\t// We want" +
					" to show the ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(" collection in grid and react on this collection external changes (Reload, server" +
					"-side Filtering)\r\n\t\t\tfluentAPI.SetBinding(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(", g => g.DataSource, x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".Entities);\r\n\t\t\t\t\r\n\t\t\t\t");
				foreach(var commandLookUpTable in lookUpTable.Commands){
				string commandName = commandLookUpTable.CommandPropertyName.Remove(commandLookUpTable.CommandPropertyName.Length -7,7);
				string nameForBarItemInLookUpTable = lookUpTable.LookUpCollectionPropertyAssociationName + commandName;
			this.Write("\t\t\t\t\t");
					if(String.IsNullOrEmpty(commandLookUpTable.ParameterPropertyName))
					{
			this.Write("\t\t\t\t\tfluentAPI.BindCommand(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(", x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".");
			this.Write(this.ToStringHelper.ToStringWithCulture(commandName));
			this.Write("());\r\n\t\t\t\t\t");
}
			this.Write("\t\t\t\t\t");
					if(!String.IsNullOrEmpty(commandLookUpTable.ParameterPropertyName))
					{
			this.Write("\t\t\t\t\tfluentAPI.BindCommand(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(",x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".");
			this.Write(this.ToStringHelper.ToStringWithCulture(commandName));
			this.Write("(null), x=>x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".SelectedEntity);\r\n\t\t\t\t\t");
}
			this.Write("\t\t\t\t");
}
			this.Write("\t\t\t#endregion\r\n\t\t\t");
}
			this.Write("\t\t\t");
			foreach(var realLookUpInfo in listLookUpInfo){
			string nameForLookUp = realLookUpInfo.Property.Name+"LookUpEdit";
			if(realLookUpInfo.Lookup.ItemsSource != null){
			this.Write("\t\t\t// Binding for ");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Property.Name));
			this.Write(" LookUp editor\r\n\t\t\tfluentAPI.SetBinding(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".Properties, p => p.DataSource, x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.ItemsSource));
			this.Write(");\r\n\t\t\t");
}}
			this.Write(" \r\n\t\t\tbbiCustomize.ItemClick += (s, e) => { dataLayoutControl1.ShowCustomizationF" +
					"orm(); };\r\n       }\r\n    }\r\n}\r\n");
}
if(IsVisualBasic){
			this.Write(@"Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports DevExpress.XtraEditors
Imports DevExpress.Utils.MVVM
Imports DevExpress.Utils.MVVM.Services
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraGrid.Views.Base

Namespace Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\r\n\tPartial Public Class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("\r\n\t\tInherits XtraUserControl\r\n\r\n\t\tPublic Sub New()\r\n\t\t\tInitializeComponent()\r\n\t\t\t" +
					"If Not mvvmContext.IsDesignMode Then\r\n\t\t\t\tInitBindings()\r\n\t\t\tEnd If\r\n\t\tEnd Sub\r\n" +
					"\t\tPrivate Sub InitBindings()\r\n\t\t\tDim fluentAPI = mvvmContext.OfType(Of Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(")()\r\n\t\t\tfluentAPI.WithEvent(Me, \"Load\").EventToCommand(Sub(x) x.OnLoaded())\r\n\t\t\tf" +
					"luentAPI.SetObjectDataSourceBinding(");
			this.Write(this.ToStringHelper.ToStringWithCulture(bindingSourceName));
			this.Write(",Function(x) x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.EntityPropertyName));
			this.Write(", Sub(x) x.Update())\r\n\t\t\t");
foreach(var lookUpTable in viewModelData.LookUpTables){
				string nameForGridControl = lookUpTable.LookUpCollectionPropertyAssociationName +"GridControl";
				string nameForGridView = lookUpTable.LookUpCollectionPropertyAssociationName +"GridView";
				string nameForMVVMContext = lookUpTable.LookUpCollectionPropertyAssociationName +"MVVMContext";
				string nameForPopUpMenu = lookUpTable.LookUpCollectionPropertyAssociationName +"PopUpMenu";
				string lookUpPropertyName = GetLookUpPropertyName(viewModelData,lookUpTable);
			this.Write("#Region \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.LookUpCollectionPropertyAssociationName));
			this.Write(" Detail Collection\"\r\n\t\t\t\' We want to synchronize the ViewModel.SelectedEntity and" +
					" the GridView.FocusedRowRandle in two-way manner\r\n\t\t\tfluentAPI.WithEvent(Of Grid" +
					"View, FocusedRowObjectChangedEventArgs)(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(", \"FocusedRowObjectChanged\").SetBinding(\r\n                Function(x) x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".SelectedEntity,\r\n                Function(args) TryCast(args.Row, ");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpTable.EntityTypeFullName));
			this.Write("),\r\n                Sub(gView, entity) gView.FocusedRowHandle = gView.FindRow(ent" +
					"ity))\r\n\t\t\t");
			if(lookUpTable.HasEntityEditProperty()){
			this.Write("\t\t\t\' We want to proceed the Edit command when row double-clicked\r\n            flu" +
					"entAPI.WithEvent(Of RowClickEventArgs)(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(", \"RowClick\").EventToCommand(\r\n                Sub(x) x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".Edit(Nothing),\r\n                Function(x) x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".SelectedEntity,\r\n                Function(args) (args.Clicks = 2) AndAlso (args." +
					"Button = System.Windows.Forms.MouseButtons.Left))\r\n\t\t\t");
}
			this.Write("\t\t\t\' We want to show PopupMenu when row clicked by right button\r\n            AddH" +
					"andler ");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridView));
			this.Write(".RowClick, Sub(s, e)\r\n\t\t\t\tIf e.Clicks = 1 AndAlso e.Button = System.Windows.Forms" +
					".MouseButtons.Right Then\r\n\t\t\t\t\t");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForPopUpMenu));
			this.Write(".ShowPopup(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(".PointToScreen(e.Location), s)\r\n\t\t\t\tEnd If\r\n\t\t\tEnd Sub\r\n\t\t\t\' We want to show the " +
					"");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(" collection in grid and react on this collection external changes (Reload, server" +
					"-side Filtering)\r\n\t\t\tfluentAPI.SetBinding(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForGridControl));
			this.Write(", Function(g) g.DataSource, Function(x) x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".Entities)\r\n\t\t\t\t");
				foreach(var commandLookUpTable in lookUpTable.Commands){
				string commandName = commandLookUpTable.CommandPropertyName.Remove(commandLookUpTable.CommandPropertyName.Length -7,7);
				string nameForBarItemInLookUpTable = lookUpTable.LookUpCollectionPropertyAssociationName + commandName;
			this.Write("\t\t\t\t\t");
					if(String.IsNullOrEmpty(commandLookUpTable.ParameterPropertyName))
					{
			this.Write("\t\t\t\t\tfluentAPI.BindCommand(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(", Sub(x) x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".");
			this.Write(this.ToStringHelper.ToStringWithCulture(commandName));
			this.Write("())\r\n\t\t\t\t\t");
}
			this.Write("\t\t\t\t\t");
					if(!String.IsNullOrEmpty(commandLookUpTable.ParameterPropertyName))
					{
			this.Write("\t\t\t\t\tfluentAPI.BindCommand(bbi");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForBarItemInLookUpTable));
			this.Write(", Sub(x) x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".");
			this.Write(this.ToStringHelper.ToStringWithCulture(commandName));
			this.Write("(Nothing), Function(x) x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(lookUpPropertyName));
			this.Write(".SelectedEntity)\r\n\t\t\t\t\t");
}
			this.Write("\t\t\t\t");
}
			this.Write("#End Region\r\n\t\t\t");
}
			this.Write("\t\t\t");
			foreach(var realLookUpInfo in listLookUpInfo){
			string nameForLookUp = realLookUpInfo.Property.Name+"LookUpEdit";
			if(realLookUpInfo.Lookup.ItemsSource != null){
			this.Write("\t\t\t\' Binding for ");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Property.Name));
			this.Write(" LookUp editor\r\n\t\t\tfluentAPI.SetBinding(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForLookUp));
			this.Write(".Properties, Function(p) p.DataSource, Function(x) x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(realLookUpInfo.Lookup.ItemsSource));
			this.Write(")\r\n\t\t\t");
}}
			this.Write(" \r\n\t\t\tAddHandler bbiCustomize.ItemClick, Sub(s, e) dataLayoutControl1.ShowCustomi" +
					"zationForm()\r\n\t\tEnd Sub\r\n\tEnd Class\r\nEnd Namespace\r\n");
}
			return this.GenerationEnvironment.ToString();
		}
static string GetLookUpPropertyName(EntityViewModelData parentViewModelData, LookUpCollectionViewModelData viewModelData){
	return parentViewModelData.EntityTypeName+viewModelData.LookUpCollectionPropertyAssociationName + "Details";
}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class ElementView_OutlookBase
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
