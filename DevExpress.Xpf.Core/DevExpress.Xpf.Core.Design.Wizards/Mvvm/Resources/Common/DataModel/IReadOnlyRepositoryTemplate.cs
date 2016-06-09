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
	public partial class IReadOnlyRepositoryTemplate : IReadOnlyRepositoryTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Linq;\r\nusing System.Collections.ObjectModel;\r\nusing S" +
					"ystem.Linq.Expressions;\r\nusing System.Collections;\r\nusing System.Collections.Gen" +
					"eric;\r\n\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// The IReadOnlyRepository interface represents the r" +
					"ead-only implementation of the Repository pattern \r\n    /// such that it can be " +
					"used to query entities of a given type. \r\n    /// </summary>\r\n    /// <typeparam" +
					" name=\"TEntity\">Repository entity type.</typeparam>\r\n    public interface IReadO" +
					"nlyRepository<TEntity> : IRepositoryQuery<TEntity> where TEntity : class {\r\n\r\n  " +
					"      /// <summary>\r\n        /// The owner unit of work.\r\n        /// </summary>" +
					"\r\n        IUnitOfWork UnitOfWork { get; }\r\n    }\r\n\r\n    /// <summary>\r\n    /// T" +
					"he IRepositoryQuery interface represents an extension of IQueryable designed to " +
					"provide an ability to specify the related objects to include in the query result" +
					"s.\r\n    /// </summary>\r\n    /// <typeparam name=\"T\">An entity type.</typeparam>\r" +
					"\n    public interface IRepositoryQuery<T> : IQueryable<T> {\r\n\r\n\t\t/// <summary>\r\n" +
					"\t\t/// Specifies the related objects to include in the query results.\r\n\t\t/// </su" +
					"mmary>\r\n\t\t/// <typeparam name=\"TProperty\">The type of the navigation property to" +
					" be included.</typeparam>\r\n        /// <param name=\"path\">A lambda expression th" +
					"at represents the path to include.</param>\r\n        IRepositoryQuery<T> Include<" +
					"TProperty>(Expression<Func<T, TProperty>> path);\r\n\r\n\t\t/// <summary>\r\n\t\t/// Filte" +
					"rs a sequence of entities based on the given predicate.\r\n\t\t/// </summary>\r\n     " +
					"   /// <param name=\"predicate\">A function to test each entity for a condition.</" +
					"param>\r\n\t\tIRepositoryQuery<T> Where(Expression<Func<T, bool>> predicate);\r\n    }" +
					"\r\n\r\n    /// <summary>\r\n    /// The base class that helps to implement the IRepos" +
					"itoryQuery interface as a wrapper over an existing IQuerable instance.\r\n    /// " +
					"</summary>\r\n    /// <typeparam name=\"T\">An entity type.</typeparam>\r\n    public " +
					"abstract class RepositoryQueryBase<T> : IQueryable<T> {\r\n        readonly Lazy<I" +
					"Queryable<T>> queryable;\r\n        protected IQueryable<T> Queryable { get { retu" +
					"rn queryable.Value; } }\r\n        protected RepositoryQueryBase(Func<IQueryable<T" +
					">> getQueryable) {\r\n            this.queryable = new Lazy<IQueryable<T>>(getQuer" +
					"yable);\r\n        }\r\n        Type IQueryable.ElementType { get { return this.Quer" +
					"yable.ElementType; } }\r\n        Expression IQueryable.Expression { get { return " +
					"this.Queryable.Expression; } }\r\n        IQueryProvider IQueryable.Provider { get" +
					" { return this.Queryable.Provider; } }\r\n        IEnumerator IEnumerable.GetEnume" +
					"rator() { return this.Queryable.GetEnumerator(); }\r\n        IEnumerator<T> IEnum" +
					"erable<T>.GetEnumerator() { return this.Queryable.GetEnumerator(); }\r\n    }\r\n\r\n " +
					"   /// <summary>\r\n    /// Provides a set of extension methods to perform commonl" +
					"y used operations with IReadOnlyRepository.\r\n    /// </summary>\r\n    public stat" +
					"ic class ReadOnlyRepositoryExtensions {\r\n\t\t/// <summary>\r\n\t\t/// Returns IQuerabl" +
					"e representing sequence of entities from repository filtered by the given predic" +
					"ate and projected to the specified projection entity type by the given LINQ func" +
					"tion.\r\n\t\t/// </summary>\r\n        /// <typeparam name=\"TEntity\">A repository enti" +
					"ty type.</typeparam>\r\n        /// <typeparam name=\"TProjection\">A projection ent" +
					"ity type.</typeparam>\r\n        /// <param name=\"repository\">A repository.</param" +
					">\r\n        /// <param name=\"predicate\">A function to test each element for a con" +
					"dition.</param>\r\n        /// <param name=\"projection\">A LINQ function used to tr" +
					"ansform entities from repository entity type to projection entity type.</param>\r" +
					"\n                public static IQueryable<TProjection> GetFilteredEntities<TEnti" +
					"ty, TProjection>(this IReadOnlyRepository<TEntity> repository, Expression<Func<T" +
					"Entity, bool>> predicate, Func<IRepositoryQuery<TEntity>, IQueryable<TProjection" +
					">> projection) where TEntity : class {\r\n            return AppendToProjection(pr" +
					"edicate, projection)(repository);\r\n        }\r\n\r\n        /// <summary>\r\n        /" +
					"// Combines an initial projection and a predicate into a new projection with the" +
					" effect of both.\r\n        /// </summary>\r\n        /// <typeparam name=\"TEntity\">" +
					"A repository entity type.</typeparam>\r\n        /// <typeparam name=\"TProjection\"" +
					">A projection entity type.</typeparam>\r\n        /// <param name=\"predicate\">A fu" +
					"nction to test each element for a condition.</param>\r\n        /// <param name=\"p" +
					"rojection\">A LINQ function used to transform entities from repository entity typ" +
					"e to projection entity type.</param>\r\n        /// <returns></returns>\r\n        p" +
					"ublic static Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> AppendToPr" +
					"ojection<TEntity, TProjection>(Expression<Func<TEntity, bool>> predicate, Func<I" +
					"RepositoryQuery<TEntity>, IQueryable<TProjection>> projection) where TEntity : c" +
					"lass {\r\n            if(predicate == null && projection == null)\r\n               " +
					" return q => (IQueryable<TProjection>)q;\r\n            if(predicate == null)\r\n   " +
					"             return projection;\r\n            if(projection == null)\r\n           " +
					"     return q => (IQueryable<TProjection>)q.Where(predicate);\r\n            retur" +
					"n q => projection(q.Where(predicate));\r\n        }\r\n\r\n\t\t/// <summary>\r\n\t\t/// Retu" +
					"rns IQuerable representing sequence of entities from repository filtered by the " +
					"given predicate.\r\n\t\t/// </summary>\r\n        /// <typeparam name=\"TEntity\">A repo" +
					"sitory entity type.</typeparam>\r\n        /// <param name=\"repository\">A reposito" +
					"ry.</param>\r\n        /// <param name=\"predicate\">A function to test each element" +
					" for a condition.</param>\r\n        public static IQueryable<TEntity> GetFiltered" +
					"Entities<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<" +
					"TEntity, bool>> predicate) where TEntity : class {\r\n            return repositor" +
					"y.GetFilteredEntities(predicate, x => x);\r\n        }\r\n    }\r\n}\r\n");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class IReadOnlyRepositoryTemplateBase
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
