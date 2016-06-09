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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.ViewModel
{
	using System;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class DocumentManagerServiceExtensionsTemplate : DocumentManagerServiceExtensionsTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.ComponentModel;\r\nusing System.Collections.Generic;\r\nu" +
					"sing System.Linq;\r\nusing System.Linq.Expressions;\r\nusing DevExpress.Mvvm;\r\n\r\nnam" +
					"espace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n\t/// <summary>\r\n    /// Provides the extension methods that are used to imple" +
					"ment the IDocumentManagerService interface.\r\n    /// </summary>\r\n    public stat" +
					"ic class DocumentManagerServiceExtensions {\r\n        /// <summary>\r\n        /// " +
					"Creates and shows a document containing a single object view model for the exist" +
					"ing entity.\r\n        /// </summary>\r\n        /// <param name=\"documentManagerSer" +
					"vice\">An instance of the IDocumentManager interface used to create and show the " +
					"document.</param>\r\n        /// <param name=\"parentViewModel\">An object that is p" +
					"assed to the view model of the created view.</param>\r\n        /// <param name=\"p" +
					"rimaryKey\">An entity primary key.</param>\r\n        public static IDocument ShowE" +
					"xistingEntityDocument<TEntity, TPrimaryKey>(this IDocumentManagerService documen" +
					"tManagerService, object parentViewModel, TPrimaryKey primaryKey) {\r\n            " +
					"IDocument document = FindEntityDocument<TEntity, TPrimaryKey>(documentManagerSer" +
					"vice, primaryKey) ?? CreateDocument<TEntity>(documentManagerService, primaryKey," +
					" parentViewModel);\r\n            if(document != null)\r\n                document.S" +
					"how();\r\n\t\t\treturn document;\r\n        }\r\n\t\t\r\n        /// <summary>\r\n        /// C" +
					"reates and shows a document containing a single object view model for new entity" +
					".\r\n        /// </summary>\r\n        /// <param name=\"documentManagerService\">An i" +
					"nstance of the IDocumentManager interface used to create and show the document.<" +
					"/param>\r\n        /// <param name=\"parentViewModel\">An object that is passed to t" +
					"he view model of the created view.</param>\r\n        /// <param name=\"newEntityIn" +
					"itializer\">An optional parameter that provides a function that initializes a new" +
					" entity.</param>\r\n        public static void ShowNewEntityDocument<TEntity>(this" +
					" IDocumentManagerService documentManagerService, object parentViewModel, Action<" +
					"TEntity> newEntityInitializer = null) {\r\n            IDocument document = Create" +
					"Document<TEntity>(documentManagerService, newEntityInitializer ?? (x => DefaultE" +
					"ntityInitializer(x)), parentViewModel);\r\n            if(document != null)\r\n     " +
					"           document.Show();\r\n        }\r\n\r\n        /// <summary>\r\n        /// Sea" +
					"rches for a document that contains a single object view model editing entity wit" +
					"h a specified primary key.\r\n        /// </summary>\r\n        /// <param name=\"doc" +
					"umentManagerService\">An instance of the IDocumentManager interface used to find " +
					"a document.</param>\r\n        /// <param name=\"primaryKey\">An entity primary key." +
					"</param>\r\n        public static IDocument FindEntityDocument<TEntity, TPrimaryKe" +
					"y>(this IDocumentManagerService documentManagerService, TPrimaryKey primaryKey) " +
					"{\r\n            if(documentManagerService == null)\r\n                return null;\r" +
					"\n            foreach(IDocument document in documentManagerService.Documents) {\r\n" +
					"                ISingleObjectViewModel<TEntity, TPrimaryKey> entityViewModel = d" +
					"ocument.Content as ISingleObjectViewModel<TEntity, TPrimaryKey>;\r\n              " +
					"  if(entityViewModel != null && object.Equals(entityViewModel.PrimaryKey, primar" +
					"yKey))\r\n                    return document;\r\n            }\r\n            return " +
					"null;\r\n        }\r\n\r\n        static void DefaultEntityInitializer<TEntity>(TEntit" +
					"y entity) { }\r\n\r\n        static IDocument CreateDocument<TEntity>(IDocumentManag" +
					"erService documentManagerService, object parameter, object parentViewModel) {\r\n " +
					"           if(documentManagerService == null)\r\n                return null;\r\n   " +
					"         var document = documentManagerService.CreateDocument(GetDocumentTypeNam" +
					"e<TEntity>(), parameter, parentViewModel);\r\n            document.Id = \"_\" + Guid" +
					".NewGuid().ToString().Replace(\'-\', \'_\');\r\n\t\t\tdocument.DestroyOnClose = false;\r\n " +
					"           return document;\r\n        }\r\n\r\n        public static string GetDocume" +
					"ntTypeName<TEntity>() {\r\n            return typeof(TEntity).Name + \"View\";\r\n    " +
					"    }\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DocumentManagerServiceExtensionsTemplateBase
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
