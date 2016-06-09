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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.DataModel.DesignTime
{
	using System;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class DesignTimeReadOnlyRepositoryTemplate : DesignTimeReadOnlyRepositoryTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Linq;\r\nusing System.Linq.Expressions;\r\nusing System.C" +
					"ollections.Generic;\r\nusing System.Collections.ObjectModel;\r\nusing DevExpress.Mvv" +
					"m;\r\n\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// DesignTimeReadOnlyRepository is an IReadOnlyReposi" +
					"tory interface implementation that represents the collection of entities of a gi" +
					"ven type for design-time mode. \r\n    /// DesignTimeReadOnlyRepository objects ar" +
					"e created from a DesignTimeInitOfWork class instance using the GetReadOnlyReposi" +
					"tory method. \r\n    /// DesignTimeReadOnlyRepository provides only read-only oper" +
					"ations against entities of a given type.\r\n    /// </summary>\r\n    /// <typeparam" +
					" name=\"TEntity\">A repository entity type.</typeparam>\r\n    public class DesignTi" +
					"meReadOnlyRepository<TEntity> : DesignTimeRepositoryQuery<TEntity>, IReadOnlyRep" +
					"ository<TEntity>\r\n        where TEntity : class {\r\n\r\n        static IQueryable<T" +
					"Entity> CreateSampleQueryable() {\r\n            return DesignTimeHelper.CreateDes" +
					"ignTimeObjects<TEntity>(2).AsQueryable();\r\n        }\r\n\r\n        readonly DesignT" +
					"imeUnitOfWork unitOfWork;\r\n\r\n        /// <summary>\r\n        /// Initializes a ne" +
					"w instance of DesignTimeReadOnlyRepository class.\r\n        /// </summary>\r\n     " +
					"   /// <param name=\"unitOfWork\">Owner unit of work that provides context for rep" +
					"ository entities.</param>\r\n        public DesignTimeReadOnlyRepository(DesignTim" +
					"eUnitOfWork unitOfWork) \r\n            : base(CreateSampleQueryable()) {\r\n       " +
					"     this.unitOfWork = unitOfWork;\r\n        }\r\n\r\n        #region IReadOnlyReposi" +
					"tory\r\n        IUnitOfWork IReadOnlyRepository<TEntity>.UnitOfWork {\r\n           " +
					" get { return unitOfWork; }\r\n        }\r\n        #endregion\r\n    }\r\n\r\n    /// <su" +
					"mmary>\r\n    /// DesignTimeRepositoryQuery is an IRepositoryQuery interface imple" +
					"mentation that is an extension of IQueryable designed to specify the related obj" +
					"ects to include in query results for design-time mode.\r\n    /// </summary>\r\n    " +
					"/// <typeparam name=\"TEntity\">An entity type.</typeparam>\r\n    public class Desi" +
					"gnTimeRepositoryQuery<TEntity> : RepositoryQueryBase<TEntity>, IRepositoryQuery<" +
					"TEntity> {\r\n\r\n        /// <summary>\r\n        /// Initializes a new instance of t" +
					"he DesignTimeRepositoryQuery class.\r\n        /// </summary>\r\n        /// <param " +
					"name=\"queryable\">The IQueryable instance which will be used by DesignTimeReposit" +
					"oryQuery to perform queries.</param>\r\n        public DesignTimeRepositoryQuery(I" +
					"Queryable<TEntity> queryable)\r\n            : base(() => queryable) { }\r\n\r\n      " +
					"  IRepositoryQuery<TEntity> IRepositoryQuery<TEntity>.Include<TProperty>(Express" +
					"ion<Func<TEntity, TProperty>> path) {\r\n            return this;\r\n        }\r\n\r\n  " +
					"      IRepositoryQuery<TEntity> IRepositoryQuery<TEntity>.Where(Expression<Func<" +
					"TEntity, bool>> predicate) {\r\n            return this;\r\n        }\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DesignTimeReadOnlyRepositoryTemplateBase
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
