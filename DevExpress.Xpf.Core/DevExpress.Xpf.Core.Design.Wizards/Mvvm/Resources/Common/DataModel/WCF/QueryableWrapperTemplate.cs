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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.DataModel.WCF
{
	using System;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class QueryableWrapperTemplate : QueryableWrapperTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Linq;\r\nusing System.Collections;\r\nusing System.Linq.E" +
					"xpressions;\r\nusing System.Collections.Generic;\r\n\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write("{\r\n    public class QueryableWrapper<T, TEntity> : IQueryable<T> where TEntity : " +
					"class {\r\n        class EnumeratorWrapper : IEnumerator<T> {\r\n            readonl" +
					"y IEnumerator<T> enumerator;\r\n            readonly Action<TEntity> loadItemCallb" +
					"ack;\r\n            public EnumeratorWrapper(IEnumerator<T> enumerable, Action<TEn" +
					"tity> loadItemCallback) {\r\n                this.enumerator = enumerable;\r\n      " +
					"          this.loadItemCallback = loadItemCallback;\r\n            }\r\n            " +
					"T IEnumerator<T>.Current {\r\n                get { return enumerator.Current; }\r\n" +
					"\t\t\t}\r\n\r\n            void IDisposable.Dispose() {\r\n                enumerator.Dis" +
					"pose();\r\n            }\r\n\r\n            object IEnumerator.Current {\r\n            " +
					"    get { return enumerator.Current; }\r\n            }\r\n            bool IEnumera" +
					"tor.MoveNext() {\r\n                bool result = enumerator.MoveNext();\r\n        " +
					"        if(result) {\r\n                    object item = enumerator.Current;\r\n   " +
					"                 if(item is TEntity)\r\n                        loadItemCallback(i" +
					"tem as TEntity);\r\n                }\r\n                return result;\r\n           " +
					" }\r\n            void IEnumerator.Reset() {\r\n                enumerator.Reset();\r" +
					"\n            }\r\n        }\r\n\r\n        class QueryProviderWrapper: IQueryProvider " +
					"{\r\n            readonly IQueryProvider queryProvider;\r\n            readonly Acti" +
					"on<TEntity> loadItemCallback;\r\n            public QueryProviderWrapper(IQueryPro" +
					"vider queryProvider, Action<TEntity> loadItemCallback) {\r\n                this.l" +
					"oadItemCallback = loadItemCallback;\r\n                this.queryProvider = queryP" +
					"rovider;\r\n            }\r\n            IQueryable<TElement> IQueryProvider.CreateQ" +
					"uery<TElement>(Expression expression) {\r\n                return new QueryableWra" +
					"pper<TElement, TEntity>(() => queryProvider.CreateQuery<TElement>(expression), l" +
					"oadItemCallback);\r\n            }\r\n\r\n            TResult IQueryProvider.Execute<T" +
					"Result>(Expression expression) {\r\n                var result = queryProvider.Exe" +
					"cute<TResult>(expression);\r\n                if(result is TEntity){\r\n            " +
					"        object o = result;\r\n                    loadItemCallback(o as TEntity);\r" +
					"\n                }\r\n                return result;\r\n            }\r\n            I" +
					"Queryable IQueryProvider.CreateQuery(Expression expression) {\r\n                t" +
					"hrow new NotImplementedException();\r\n            }\r\n            object IQueryPro" +
					"vider.Execute(Expression expression) {\r\n                throw new NotImplemented" +
					"Exception();\r\n            }\r\n        }\r\n\r\n        readonly Lazy<IQueryable<T>> q" +
					"ueryable;\r\n        IQueryable<T> Queryable { get { return queryable.Value; } }\r\n" +
					"        readonly Lazy<IQueryProvider> queryProvider;\r\n        IQueryProvider Que" +
					"ryProvider { get { return queryProvider.Value; } }\r\n        protected Action<TEn" +
					"tity> LoadItemCallback { get; private set; }\r\n        public QueryableWrapper(Fu" +
					"nc<IQueryable<T>> getQueryable, Action<TEntity> loadItemCallback) {\r\n           " +
					" this.queryable = new Lazy<IQueryable<T>>(getQueryable);\r\n            this.query" +
					"Provider = new Lazy<IQueryProvider>(() => new QueryProviderWrapper(Queryable.Pro" +
					"vider, this.LoadItemCallback));\r\n            this.LoadItemCallback = loadItemCal" +
					"lback ?? (x => LoadItem(x));\r\n        }\r\n        protected virtual void LoadItem" +
					"(TEntity entity) {\r\n            throw new NotSupportedException();\r\n        }\r\n " +
					"       IEnumerator<T> IEnumerable<T>.GetEnumerator() {\r\n            return new E" +
					"numeratorWrapper(Queryable.GetEnumerator(), LoadItemCallback);\r\n        }\r\n\r\n   " +
					"     System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator" +
					"() {\r\n            return new EnumeratorWrapper(Queryable.GetEnumerator(), LoadIt" +
					"emCallback);\r\n        }\r\n\r\n        Type IQueryable.ElementType {\r\n            ge" +
					"t { return Queryable.ElementType; }\r\n        }\r\n\r\n        Expression IQueryable." +
					"Expression {\r\n            get { return Queryable.Expression; }\r\n        }\r\n\r\n   " +
					"     IQueryProvider IQueryable.Provider {\r\n            get { return QueryProvide" +
					"r; }\r\n        }\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class QueryableWrapperTemplateBase
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
