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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.ViewModels
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Xml;
	using System.Linq;
	using DevExpress.Design.Mvvm;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using DevExpress.Design.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class DocumentManagerViewModelTemplate : DocumentManagerViewModelTemplateBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	DocumentManagerViewModelInfo viewModelData = templateInfo.Properties["IViewModelInfo"] as DocumentManagerViewModelInfo;
	string viewModelName = templateInfo.Properties["ViewModelName"].ToString();
	string dbContainerName = templateInfo.Properties["DbContainerName"].ToString();
	var uiType = (DevExpress.Design.Mvvm.UIType)templateInfo.Properties["UIType"];
	var context = (TemplateGenerationContext)templateInfo.Properties["TemplateGenerationContext"];
			this.Write("using System;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\nusing Syste" +
					"m.ComponentModel;\r\nusing DevExpress.Mvvm;\r\nusing DevExpress.Mvvm.POCO;\r\n");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// Represents the root POCO view model for the ");
			this.Write(this.ToStringHelper.ToStringWithCulture(dbContainerName));
			this.Write(" data model.\r\n    /// </summary>\r\n    public partial class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelName));
			this.Write(" : DocumentsViewModel<");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.ModuleName));
			this.Write(", I");
			this.Write(this.ToStringHelper.ToStringWithCulture(dbContainerName));
			this.Write("UnitOfWork> {\r\n\r\n\t\tconst string TablesGroup = \"Tables\";\r\n\r\n\t\tconst string ViewsGr" +
					"oup = \"Views\";\r\n");
if(uiType == DevExpress.Design.Mvvm.UIType.WindowsUI || uiType == DevExpress.Design.Mvvm.UIType.OutlookInspired) {
			this.Write("\t\tINavigationService NavigationService { get { return this.GetService<INavigation" +
					"Service>(); } }\r\n");
}
			this.Write("\t\r\n        /// <summary>\r\n        /// Creates a new instance of ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelName));
			this.Write(" as a POCO view model.\r\n        /// </summary>\r\n        public static ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelName));
			this.Write(" Create() {\r\n            return ViewModelSource.Create(() => new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelName));
			this.Write("());\r\n        }\r\n\r\n        /// <summary>\r\n        /// Initializes a new instance " +
					"of the ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelName));
			this.Write(" class.\r\n        /// This constructor is declared protected to avoid undesired in" +
					"stantiation of the ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelName));
			this.Write(" type without the POCO proxy factory.\r\n        /// </summary>\r\n        protected " +
					"");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelName));
			this.Write("()\r\n\t\t    : base(UnitOfWorkSource.GetUnitOfWorkFactory()) {\r\n        }\r\n\r\n       " +
					" protected override ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.ModuleName));
			this.Write("[] CreateModules() {\r\n            return new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.ModuleName));
			this.Write("[] {\r\n");
foreach(DocumentInfo info in viewModelData.Tables) {
			this.Write("                new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.ModuleName));
			this.Write("(\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(info.Caption));
			this.Write("\", \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(info.ViewName));
			this.Write("\", TablesGroup");
if(!EntitySetInfoExtensions.ShouldGenerateReadOnlyView(info.EntityInfo)) {
			this.Write(", GetPeekCollectionViewModelFactory(x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(info.RepositoryInfo.Name));
			this.Write(")");
}
			this.Write("),\r\n");
}
foreach(DocumentInfo info in viewModelData.Views) {
			this.Write("                new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.ModuleName));
			this.Write("(\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(info.Caption));
			this.Write("\", \"");
			this.Write(this.ToStringHelper.ToStringWithCulture(info.ViewName));
			this.Write("\", ViewsGroup");
if(!EntitySetInfoExtensions.ShouldGenerateReadOnlyView(info.EntityInfo)) {
			this.Write(", GetPeekCollectionViewModelFactory(x => x.");
			this.Write(this.ToStringHelper.ToStringWithCulture(info.RepositoryInfo.Name));
			this.Write(")");
}
			this.Write("),\r\n");
}
this.ExecuteDocumentManagerViewModelHook(TemplatesCodeGen.STR_DocumentManagerViewModelHook_GenerateAdditionalModules);
			this.Write("\t\t\t};\r\n        }\r\n\r\n        ");
this.ExecuteDocumentManagerViewModelHook(TemplatesCodeGen.STR_DocumentManagerViewModelHooks_GenerateAdditionalServices);
			this.Write("\r\n        ");
this.ExecuteDocumentManagerViewModelHook(TemplatesCodeGen.STR_DocumentManagerViewModelHooks_GenerateAdditionalCommands);
			this.Write("\r\n");
if(uiType == DevExpress.Design.Mvvm.UIType.WindowsUI || uiType == DevExpress.Design.Mvvm.UIType.OutlookInspired) {
			this.Write("\t\tprotected override void OnActiveModuleChanged(");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.ModuleName));
			this.Write(" oldModule) {\r\n            if(ActiveModule != null && NavigationService != null) " +
					"{\r\n                NavigationService.ClearNavigationHistory();\r\n            }\r\n " +
					"           base.OnActiveModuleChanged(oldModule);\r\n        }\r\n");
}
			this.Write("\t}\r\n\r\n    public partial class ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.ModuleName));
			this.Write(" : ModuleDescription<");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.ModuleName));
			this.Write("> {\r\n        public ");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.ModuleName));
			this.Write("(string title, string documentType, string group, Func<");
			this.Write(this.ToStringHelper.ToStringWithCulture(viewModelData.ModuleName));
			this.Write(", object> peekCollectionViewModelFactory = null)\r\n            : base(title, docum" +
					"entType, group, peekCollectionViewModelFactory) {\r\n        }\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerViewModelTemplateBase
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
