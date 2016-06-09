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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.DataModel
{
	using System;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using DevExpress.Design.Mvvm.EntityFramework;
	using DevExpress.Design.Mvvm;
	using DevExpress.Entity.Model;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class UnitOfWorkSourceTemplate : UnitOfWorkSourceTemplateBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	var usingList = templateInfo.UsingList;
	this.PasteUsingList(usingList);
	EntityModelData entityModel = templateInfo.Properties["EntityModelData"] as EntityModelData;
			this.Write("using DevExpress.Mvvm;\r\nusing System.Collections;\r\nusing System.ComponentModel;\r\n" +
					"using DevExpress.Data.Linq;\r\nusing DevExpress.Data.Linq.Helpers;\r\nusing DevExpre" +
					"ss.Data.Async.Helpers;\r\n\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n");
	this.PasteAliasList(usingList, entityModel.FullName);
			this.Write("    /// <summary>\r\n    /// Provides methods to obtain the relevant IUnitOfWorkFac" +
					"tory.\r\n    /// </summary>\r\n    public static class UnitOfWorkSource {\r\n");
if(!entityModel.WithoutDesignTime){
			this.Write("\t\t/// <summary>\r\n        /// Returns the IUnitOfWorkFactory implementation based " +
					"on the current mode (run-time or design-time).\r\n        /// </summary>\r\n        " +
					"public static IUnitOfWorkFactory<I");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write(@"UnitOfWork> GetUnitOfWorkFactory() {
            return GetUnitOfWorkFactory(ViewModelBase.IsInDesignMode);
        }

		/// <summary>
        /// Returns the IUnitOfWorkFactory implementation based on the given mode (run-time or design-time).
        /// </summary>
        /// <param name=""isInDesignTime"">Used to determine which implementation of IUnitOfWorkFactory should be returned.</param>
        public static IUnitOfWorkFactory<I");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("UnitOfWork> GetUnitOfWorkFactory(bool isInDesignTime) {\r\n\t\t\tif(isInDesignTime)\r\n " +
					"               return new DesignTimeUnitOfWorkFactory<I");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("UnitOfWork>(() => new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("DesignTimeUnitOfWork());\r\n");
if(entityModel.ContainerType == DbContainerType.WCF){
			this.Write("\t\t\tUri svcUri = new Uri(\"");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.SourceUrl));
			this.Write("\");\r\n\t\t\treturn new DbUnitOfWorkFactory<I");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("UnitOfWork, ");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write(">(\r\n\t\t\t\t() => new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("UnitOfWork(() => new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("(svcUri)),\r\n\t\t\t\t() => new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("(svcUri));\r\n");
} else {
			this.Write("            return new DbUnitOfWorkFactory<I");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("UnitOfWork>(() => new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("UnitOfWork(() => new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("()));\r\n");
}
			this.Write("        }\r\n");
} else {
			this.Write("\r\n\t\t/// <summary>\r\n        /// Returns the IUnitOfWorkFactory implementation.\r\n  " +
					"      /// </summary>\r\n        public static IUnitOfWorkFactory<I");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("UnitOfWork> GetUnitOfWorkFactory() {\r\n            return new DbUnitOfWorkFactory<" +
					"I");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("UnitOfWork>(() => new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("UnitOfWork(() => new ");
			this.Write(this.ToStringHelper.ToStringWithCulture(entityModel.Name));
			this.Write("()));\r\n        }\r\n");
}
			this.Write("    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class UnitOfWorkSourceTemplateBase
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
