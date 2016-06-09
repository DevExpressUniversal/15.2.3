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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.DataModel
{
	using System;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class IUnitOfWorkTemplate : IUnitOfWorkTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.ComponentModel;\r\nusing System.Linq;\r\nusing System.Lin" +
					"q.Expressions;\r\n\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// The IUnitOfWork interface represents the Unit Of W" +
					"ork pattern \r\n    /// such that it can be used to query from a database and grou" +
					"p together changes that will then be written back to the store as a unit. \r\n    " +
					"/// </summary>\r\n    public interface IUnitOfWork {\r\n        /// <summary>\r\n     " +
					"   /// Saves all changes made in this unit of work to the underlying store.\r\n   " +
					"     /// </summary>\r\n        void SaveChanges();\r\n\r\n        /// <summary>\r\n     " +
					"   /// Checks if the unit of work is tracking any new, deleted, or changed entit" +
					"ies or relationships that will be sent to the store if SaveChanges() is called.\r" +
					"\n        /// </summary>\r\n        bool HasChanges();\r\n    }\r\n\r\n    /// <summary>\r" +
					"\n    /// Provides the method to create a unit of work of a given type.\r\n    /// " +
					"</summary>\r\n    /// <typeparam name=\"TUnitOfWork\">A unit of work type.</typepara" +
					"m>\r\n    public interface IUnitOfWorkFactory<TUnitOfWork> where TUnitOfWork : IUn" +
					"itOfWork {\r\n\r\n        /// <summary>\r\n        /// Creates a new unit of work.\r\n  " +
					"      /// </summary>\r\n        TUnitOfWork CreateUnitOfWork();\r\n\r\n\t\t/// <summary>" +
					"\r\n        /// Creates a new IInstantFeedbackSource instance.\r\n        /// </summ" +
					"ary>\r\n\t\tIInstantFeedbackSource<TProjection> CreateInstantFeedbackSource<TEntity," +
					" TProjection, TPrimaryKey>(Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> " +
					"getRepositoryFunc, Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> proj" +
					"ection) \r\n            where TEntity : class, new()\r\n            where TProjectio" +
					"n : class;\r\n    }\r\n\r\n    /// <summary>\r\n    /// A data source suitable as an Ins" +
					"tant Feedback source.\r\n    /// The GetList() method of the base IListSource inte" +
					"rface is expected to return an instance of an internal type that happens to\r\n   " +
					" /// implement the IList interface. As such the IInstantFeedbackSource interface" +
					" can only be implemented as a wrapper\r\n    /// for an existing Instant Feedback " +
					"source, e.g. EntityInstantFeedbackSource or WcfInstantFeedbackDataSource.\r\n    /" +
					"// </summary>\r\n    /// <typeparam name=\"TEntity\"></typeparam>\r\n    public interf" +
					"ace IInstantFeedbackSource<TEntity> : IListSource\r\n        where TEntity : class" +
					" {\r\n        /// <summary>\r\n        /// Invalidate all loaded entities. This meth" +
					"od is used to make changes made to the data source visible to\r\n        /// consu" +
					"mers if this Instant Feedback source.\r\n        /// Currently, in scaffolded impl" +
					"ementations this method only works for WCF when the MergeOption set to NoTrackin" +
					"g \r\n        /// and for EntityFramework when a projection is used.\r\n        /// " +
					"</summary>\r\n        void Refresh();\r\n\r\n        /// <summary>\r\n        /// Get th" +
					"e value of a property.\r\n        /// </summary>\r\n        /// <typeparam name=\"TPr" +
					"operty\">The type of the property.</typeparam>\r\n        /// <param name=\"threadSa" +
					"feProxy\">A proxy object.</param>\r\n        /// <param name=\"propertyExpression\">A" +
					"n expression specifying the property which value is to be fetched.</param>\r\n    " +
					"    /// <returns></returns>\r\n        TProperty GetPropertyValue<TProperty>(objec" +
					"t threadSafeProxy, Expression<Func<TEntity, TProperty>> propertyExpression);\r\n\r\n" +
					"        /// <summary>\r\n        /// Check if a proxy object is in the Loaded stat" +
					"e.\r\n        /// If a proxy object is not in the Loaded state, its properties hav" +
					"e not yet been initialized.\r\n        /// </summary>\r\n        /// <param name=\"th" +
					"readSafeProxy\"></param>\r\n        /// <returns></returns>\r\n        bool IsLoadedP" +
					"roxy(object threadSafeProxy);\r\n    }\r\n}\r\n");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class IUnitOfWorkTemplateBase
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
