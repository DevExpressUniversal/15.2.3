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
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class DocumentManagerView_Outlook : DocumentManagerView_OutlookBase
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
					"ices;\r\nusing DevExpress.Utils.MVVM;\r\n\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("{\r\n    public partial class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write(" : XtraUserControl {\r\n        public ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("() {\r\n\t\t\tInitializeComponent();\r\n\t\t\tif(!mvvmContext.IsDesignMode)\r\n              " +
					"  InitializeNavigation();\r\n            ribbonControl.Merge += ribbonControl_Merg" +
					"e;\r\n            ribbonControl.UnMerge += ribbonControl_UnMerge;\r\n        }\r\n\r\n  " +
					"      private void ribbonControl_UnMerge(object sender, DevExpress.XtraBars.Ribb" +
					"on.RibbonMergeEventArgs e) {\r\n            ribbonControl.SelectedPage = e.MergeOw" +
					"ner.SelectedPage;\r\n            ribbonControl.StatusBar.UnMergeStatusBar();\r\n    " +
					"    }\r\n\r\n        void ribbonControl_Merge(object sender, DevExpress.XtraBars.Rib" +
					"bon.RibbonMergeEventArgs e) {\r\n            ribbonControl.SelectedPage = e.Merged" +
					"Child.SelectedPage;\r\n            ribbonControl.StatusBar.MergeStatusBar(e.Merged" +
					"Child.StatusBar);\r\n        }\r\n        void InitializeNavigation() {\r\n\t\t\tDevExpre" +
					"ss.XtraEditors.WindowsFormsSettings.SetDPIAware();\r\n            DevExpress.XtraE" +
					"ditors.WindowsFormsSettings.EnableFormSkins();\r\n            DevExpress.XtraEdito" +
					"rs.WindowsFormsSettings.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.Tr" +
					"ue;\r\n            DevExpress.XtraEditors.WindowsFormsSettings.ScrollUIMode = DevE" +
					"xpress.XtraEditors.ScrollUIMode.Touch;\r\n            DevExpress.LookAndFeel.UserL" +
					"ookAndFeel.Default.SetSkinStyle(\"Office 2013 Light Gray\");\t\r\n\r\n            mvvmC" +
					"ontext.RegisterService(DocumentManagerService.Create(navigationFrame));\r\n       " +
					"     DevExpress.Utils.MVVM.MVVMContext.RegisterFlyoutDialogService();\r\n         " +
					"   // We want to use buttons in Ribbon to show the specific modules\r\n           " +
					" var fluentAPI = mvvmContext.OfType<");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(">();\r\n\t\t\t");
			int indexer =0;
			foreach(var item in viewModelData.Tables){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("            fluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(", (x, m) => x.Show(m), x => x.Modules[");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("]);\r\n\t\t\tfluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(", (x, m) => x.Show(m), x => x.Modules[");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("]);\r\n\t\t\t");
indexer++;}
			this.Write("\t\t\t");
			foreach(var item in viewModelData.Views){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\t\tfluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(", (x, m) => x.Show(m), x => x.Modules[");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("]);\r\n\t\t\tfluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(", (x, m) => x.Show(m), x => x.Modules[");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("]);\r\n\t\t\t");
indexer++;}
			this.Write("            // We want show the default module when our UserControl is loaded\r\n  " +
					"          fluentAPI.WithEvent<EventArgs>(this, \"Load\")\r\n                .EventTo" +
					"Command(x => x.OnLoaded(null), x => x.DefaultModule);\r\n        }\r\n    }\r\n}\r\n");
}
if(IsVisualBasic){
			this.Write("Imports System\r\nImports System.Linq\r\nImports System.Collections.Generic\r\nImports " +
					"DevExpress.XtraEditors\r\nImports DevExpress.XtraBars\r\nImports DevExpress.Utils.MV" +
					"VM.Services\r\nImports DevExpress.Utils.MVVM\r\n\r\nNamespace Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewFullName));
			this.Write("\r\n\tPartial Public Class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewName));
			this.Write("\r\n\t\tInherits XtraUserControl\r\n\r\n\t\tPublic Sub New()\r\n\t\t\tInitializeComponent()\r\n\t\t\t" +
					"If Not mvvmContext.IsDesignMode Then\r\n\t\t\t\tInitializeNavigation()\r\n\t\t\tEnd If\r\n\t\t\t" +
					"AddHandler ribbonControl.Merge, AddressOf ribbonControl_Merge\r\n\t\t\tAddHandler rib" +
					"bonControl.UnMerge, AddressOf ribbonControl_UnMerge\r\n\t\tEnd Sub\r\n\r\n\t\tPrivate Sub " +
					"ribbonControl_UnMerge(ByVal sender As Object, ByVal e As DevExpress.XtraBars.Rib" +
					"bon.RibbonMergeEventArgs)\r\n\t\t\tribbonControl.SelectedPage = e.MergeOwner.Selected" +
					"Page\r\n\t\t\tribbonControl.StatusBar.UnMergeStatusBar()\r\n\t\tEnd Sub\r\n\r\n\t\tPrivate Sub " +
					"ribbonControl_Merge(ByVal sender As Object, ByVal e As DevExpress.XtraBars.Ribbo" +
					"n.RibbonMergeEventArgs)\r\n\t\t\tribbonControl.SelectedPage = e.MergedChild.SelectedP" +
					"age\r\n\t\t\tribbonControl.StatusBar.MergeStatusBar(e.MergedChild.StatusBar)\r\n\t\tEnd S" +
					"ub\r\n\t\tPrivate Sub InitializeNavigation()\r\n\t\t\tDevExpress.XtraEditors.WindowsForms" +
					"Settings.SetDPIAware()\r\n\t\t\tDevExpress.XtraEditors.WindowsFormsSettings.EnableFor" +
					"mSkins()\r\n\t\t\tDevExpress.XtraEditors.WindowsFormsSettings.AllowPixelScrolling = D" +
					"evExpress.Utils.DefaultBoolean.True\r\n\t\t\tDevExpress.XtraEditors.WindowsFormsSetti" +
					"ngs.ScrollUIMode = DevExpress.XtraEditors.ScrollUIMode.Touch\r\n\t\t\tDevExpress.Look" +
					"AndFeel.UserLookAndFeel.Default.SetSkinStyle(\"Office 2013 Light Gray\")\r\n\r\n\t\t\tmvv" +
					"mContext.RegisterService(DocumentManagerService.Create(navigationFrame))\r\n\t\t\tDev" +
					"Express.Utils.MVVM.MVVMContext.RegisterFlyoutDialogService()\r\n\t\t\t\' We want to us" +
					"e buttons in Ribbon to show the specific modules\r\n\t\t\tDim fluentAPI = mvvmContext" +
					".OfType(Of Global.");
			this.Write(this.ToStringHelper.ToStringWithCulture(mvvmContextFullName));
			this.Write(")()\r\n\t\t\t");
			int indexer =0;
			foreach(var item in viewModelData.Tables){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("            fluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(", Sub(x, m) x.Show(m), Function(x) x.Modules(");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("))\r\n\t\t\tfluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(", Sub(x, m) x.Show(m), Function(x) x.Modules(");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("))\r\n\t\t\t");
indexer++;}
			this.Write("\t\t\t");
			foreach(var item in viewModelData.Views){
				string nameForItem = "navigationBarItem" + item.ViewName;
				string nameForRibbonItem = "barButtonItem" + item.ViewName;
			this.Write("\t\t\tfluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForItem));
			this.Write(", Sub(x, m) x.Show(m), Function(x) x.Modules(");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("))\r\n\t\t\tfluentAPI.BindCommand(");
			this.Write(this.ToStringHelper.ToStringWithCulture(nameForRibbonItem));
			this.Write(", Sub(x, m) x.Show(m), Function(x) x.Modules(");
			this.Write(this.ToStringHelper.ToStringWithCulture(indexer));
			this.Write("))\r\n\t\t\t");
indexer++;}
			this.Write("\t\t\t\' We want show the default module when our UserControl is loaded\r\n\t\t\tfluentAPI" +
					".WithEvent (Of EventArgs)(Me, \"Load\").EventToCommand(Sub(x) x.OnLoaded(Nothing)," +
					" Function(x) x.DefaultModule)\r\n\t\tEnd Sub\r\n\tEnd Class\r\nEnd Namespace\r\n");
}
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerView_OutlookBase
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
