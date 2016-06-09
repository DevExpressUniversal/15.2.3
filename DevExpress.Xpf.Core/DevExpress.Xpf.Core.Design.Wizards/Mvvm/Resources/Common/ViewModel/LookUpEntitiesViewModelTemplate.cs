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
	public partial class LookUpEntitiesViewModelTemplate : LookUpEntitiesViewModelTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Linq;\r\nusing System.ComponentModel;\r\nusing DevExpress" +
					".Mvvm;\r\nusing DevExpress.Mvvm.POCO;\r\nusing System.Collections.ObjectModel;\r\n");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// Represents a POCO view models used by SingleObject" +
					"ViewModel to exposing collections of related entities.\r\n    /// This is a partia" +
					"l class that provides an extension point to add custom properties, commands and " +
					"override methods without modifying the auto-generated code.\r\n    /// </summary>\r" +
					"\n    /// <typeparam name=\"TEntity\">A repository entity type.</typeparam>\r\n    //" +
					"/ <typeparam name=\"TProjection\">A projection entity type.</typeparam>\r\n    /// <" +
					"typeparam name=\"TPrimaryKey\">A primary key value type.</typeparam>\r\n    /// <typ" +
					"eparam name=\"TUnitOfWork\">A unit of work type.</typeparam>\r\n    public class Loo" +
					"kUpEntitiesViewModel<TEntity, TProjection, TPrimaryKey, TUnitOfWork> : EntitiesV" +
					"iewModel<TEntity, TProjection, TUnitOfWork>, IDocumentContent\r\n        where TEn" +
					"tity : class\r\n        where TProjection : class\r\n        where TUnitOfWork : IUn" +
					"itOfWork {\r\n\r\n        /// <summary>\r\n        /// Creates a new instance of LookU" +
					"pEntitiesViewModel as a POCO view model.\r\n        /// </summary>\r\n        /// <p" +
					"aram name=\"unitOfWorkFactory\">A factory used to create a unit of work instance.<" +
					"/param>\r\n        /// <param name=\"getRepositoryFunc\">A function that returns a r" +
					"epository representing entities of the given type.</param>\r\n        /// <param n" +
					"ame=\"projection\">An optional parameter that provides a LINQ function used to cus" +
					"tomize a query for entities. The parameter, for example, can be used for sorting" +
					" data and/or for projecting data to a custom type that does not match the reposi" +
					"tory entity type.</param>\r\n        public static LookUpEntitiesViewModel<TEntity" +
					", TProjection, TPrimaryKey, TUnitOfWork> Create(\r\n            IUnitOfWorkFactory" +
					"<TUnitOfWork> unitOfWorkFactory,\r\n            Func<TUnitOfWork, IReadOnlyReposit" +
					"ory<TEntity>> getRepositoryFunc,\r\n            Func<IRepositoryQuery<TEntity>, IQ" +
					"ueryable<TProjection>> projection = null) {\r\n            return ViewModelSource." +
					"Create(() => new LookUpEntitiesViewModel<TEntity, TProjection, TPrimaryKey, TUni" +
					"tOfWork>(unitOfWorkFactory, getRepositoryFunc, projection));\r\n        }\r\n\r\n     " +
					"   /// <summary>\r\n        /// Initializes a new instance of the LookUpEntitiesVi" +
					"ewModel class.\r\n        /// This constructor is declared protected to avoid an u" +
					"ndesired instantiation of the LookUpEntitiesViewModel type without the POCO prox" +
					"y factory.\r\n        /// </summary>\r\n        /// <param name=\"unitOfWorkFactory\">" +
					"A factory used to create a unit of work instance.</param>\r\n        /// <param na" +
					"me=\"getRepositoryFunc\">A function that returns a repository representing entitie" +
					"s of the given type.</param>\r\n        /// <param name=\"projection\">A LINQ functi" +
					"on used to customize a query for entities. The parameter, for example, can be us" +
					"ed for sorting data and/or for projecting data to a custom type that does not ma" +
					"tch the repository entity type.</param>\r\n        protected LookUpEntitiesViewMod" +
					"el(\r\n            IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,\r\n           " +
					" Func<TUnitOfWork, IReadOnlyRepository<TEntity>> getRepositoryFunc,\r\n           " +
					" Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> projection\r\n          " +
					"  ) : base(unitOfWorkFactory, getRepositoryFunc, projection) {\r\n        }\r\n\r\n   " +
					"     protected override IEntitiesChangeTracker CreateEntitiesChangeTracker() {\r\n" +
					"            return new EntitiesChangeTracker<TPrimaryKey>(this);\r\n        }\r\n   " +
					" }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class LookUpEntitiesViewModelTemplateBase
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
