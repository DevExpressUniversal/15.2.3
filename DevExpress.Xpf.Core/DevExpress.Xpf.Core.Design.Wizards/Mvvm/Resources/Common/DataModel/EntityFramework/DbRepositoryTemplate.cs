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

namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources.Common.DataModel.EntityFramework
{
	using System;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class DbRepositoryTemplate : DbRepositoryTemplateBase
	{
		public virtual string TransformText()
		{
this.PasteUsingList();
			this.Write("using System.Data.Entity;\r\nusing System.Data.Entity.Validation;\r\nusing System.Dat" +
					"a.Entity.Infrastructure;\r\n\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// A DbRepository is a IRepository interface implemen" +
					"tation representing the collection of all entities in the unit of work, or that " +
					"can be queried from the database, of a given type. \r\n    /// DbRepository object" +
					"s are created from a DbUnitOfWork using the GetRepository method. \r\n    /// DbRe" +
					"pository provides only write operations against entities of a given type in addi" +
					"tion to the read-only operation provided DbReadOnlyRepository base class.\r\n    /" +
					"// </summary>\r\n    /// <typeparam name=\"TEntity\">Repository entity type.</typepa" +
					"ram>\r\n    /// <typeparam name=\"TPrimaryKey\">Entity primary key type.</typeparam>" +
					"\r\n    /// <typeparam name=\"TDbContext\">DbContext type.</typeparam>\r\n    public c" +
					"lass DbRepository<TEntity, TPrimaryKey, TDbContext> : DbReadOnlyRepository<TEnti" +
					"ty, TDbContext>, IRepository<TEntity, TPrimaryKey>\r\n        where TEntity : clas" +
					"s\r\n        where TDbContext : DbContext {\r\n\r\n        readonly Expression<Func<TE" +
					"ntity, TPrimaryKey>> getPrimaryKeyExpression;\r\n        readonly EntityTraits<TEn" +
					"tity, TPrimaryKey> entityTraits;\r\n\r\n        /// <summary>\r\n        /// Initializ" +
					"es a new instance of DbRepository class.\r\n        /// </summary>\r\n        /// <p" +
					"aram name=\"unitOfWork\">Owner unit of work that provides context for repository e" +
					"ntities.</param>\r\n        /// <param name=\"dbSetAccessor\">Function that returns " +
					"DbSet entities from Entity Framework DbContext.</param>\r\n        /// <param name" +
					"=\"getPrimaryKeyExpression\">Lambda-expression that returns entity primary key.</p" +
					"aram>\r\n        public DbRepository(DbUnitOfWork<TDbContext> unitOfWork, Func<TDb" +
					"Context, DbSet<TEntity>> dbSetAccessor, Expression<Func<TEntity, TPrimaryKey>> g" +
					"etPrimaryKeyExpression)\r\n            : base(unitOfWork, dbSetAccessor) {\r\n      " +
					"      this.getPrimaryKeyExpression = getPrimaryKeyExpression;\r\n            this." +
					"entityTraits = ExpressionHelper.GetEntityTraits(this, getPrimaryKeyExpression);\r" +
					"\n        }\r\n\r\n        protected virtual TEntity CreateCore(bool add = true) {\r\n " +
					"           TEntity newEntity = DbSet.Create();\r\n            if(add) {\r\n         " +
					"       DbSet.Add(newEntity);\r\n            }\r\n            return newEntity;\r\n    " +
					"    }\r\n\r\n        protected virtual void UpdateCore(TEntity entity) {\r\n        }\r" +
					"\n\r\n        protected virtual EntityState GetStateCore(TEntity entity) {\r\n       " +
					"     return GetEntityState(Context.Entry(entity).State);\r\n        }\r\n\r\n        s" +
					"tatic EntityState GetEntityState(System.Data.Entity.EntityState entityStates) {\r" +
					"\n            switch(entityStates) {\r\n                case System.Data.Entity.Ent" +
					"ityState.Added:\r\n                    return EntityState.Added;\r\n                " +
					"case System.Data.Entity.EntityState.Deleted:\r\n                    return EntityS" +
					"tate.Deleted;\r\n                case System.Data.Entity.EntityState.Detached:\r\n  " +
					"                  return EntityState.Detached;\r\n                case System.Data" +
					".Entity.EntityState.Modified:\r\n                    return EntityState.Modified;\r" +
					"\n                case System.Data.Entity.EntityState.Unchanged:\r\n               " +
					"     return EntityState.Unchanged;\r\n                default:\r\n                  " +
					"  throw new NotImplementedException();\r\n            }\r\n        }\r\n\r\n\r\n        pr" +
					"otected virtual TEntity FindCore(TPrimaryKey primaryKey) {\r\n            object[]" +
					" values;\r\n            if(ExpressionHelper.IsTuple<TPrimaryKey>()) {\r\n           " +
					"     values = ExpressionHelper.GetKeyPropertyValues(primaryKey);\r\n            } " +
					"else {\r\n                values = new object[] { primaryKey };\r\n            }\r\n  " +
					"          return DbSet.Find(values);\r\n        }\r\n\r\n        protected virtual voi" +
					"d RemoveCore(TEntity entity) {\r\n            try {\r\n                DbSet.Remove(" +
					"entity);\r\n            } catch (DbEntityValidationException ex) {\r\n              " +
					"  throw DbExceptionsConverter.Convert(ex);\r\n            } catch (DbUpdateExcepti" +
					"on ex) {\r\n                throw DbExceptionsConverter.Convert(ex);\r\n            " +
					"}\r\n        }\r\n\r\n        protected virtual TEntity ReloadCore(TEntity entity) {\r\n" +
					"            Context.Entry(entity).Reload();\r\n            return FindCore(GetPrim" +
					"aryKeyCore(entity));\r\n        }\r\n        protected virtual TPrimaryKey GetPrimar" +
					"yKeyCore(TEntity entity) {\r\n            return entityTraits.GetPrimaryKey(entity" +
					");\r\n        }\r\n\r\n        protected virtual void SetPrimaryKeyCore(TEntity entity" +
					", TPrimaryKey primaryKey) {\r\n            var setPrimaryKeyAction = entityTraits." +
					"SetPrimaryKey;\r\n            setPrimaryKeyAction(entity, primaryKey);\r\n        }\r" +
					"\n\r\n        #region IRepository\r\n        TEntity IRepository<TEntity, TPrimaryKey" +
					">.Find(TPrimaryKey primaryKey) {\r\n            return FindCore(primaryKey);\r\n    " +
					"    }\r\n\r\n\t\tvoid IRepository<TEntity, TPrimaryKey>.Add(TEntity entity) {\r\n       " +
					"     DbSet.Add(entity);\r\n        }\r\n\t\t\r\n        void IRepository<TEntity, TPrima" +
					"ryKey>.Remove(TEntity entity) {\r\n            RemoveCore(entity);\r\n        }\r\n\r\n\t" +
					"\tTEntity IRepository<TEntity, TPrimaryKey>.Create(bool add) {\r\n            retur" +
					"n CreateCore(add);\r\n        }\r\n\t\t\r\n        void IRepository<TEntity, TPrimaryKey" +
					">.Update(TEntity entity) {\r\n            UpdateCore(entity);\r\n        }\r\n\r\n      " +
					"  EntityState IRepository<TEntity, TPrimaryKey>.GetState(TEntity entity) {\r\n    " +
					"        return GetStateCore(entity);\r\n        }\r\n\r\n        TEntity IRepository<T" +
					"Entity, TPrimaryKey>.Reload(TEntity entity) {\r\n            return ReloadCore(ent" +
					"ity);\r\n        }\r\n\r\n        Expression<Func<TEntity, TPrimaryKey>> IRepository<T" +
					"Entity, TPrimaryKey>.GetPrimaryKeyExpression {\r\n            get { return this.ge" +
					"tPrimaryKeyExpression; }\r\n        }\r\n\r\n        void IRepository<TEntity, TPrimar" +
					"yKey>.SetPrimaryKey(TEntity entity, TPrimaryKey primaryKey) {\r\n            SetPr" +
					"imaryKeyCore(entity, primaryKey);\r\n        }\r\n\r\n        TPrimaryKey IRepository<" +
					"TEntity, TPrimaryKey>.GetPrimaryKey(TEntity entity) {\r\n            return GetPri" +
					"maryKeyCore(entity);\r\n        }\r\n\r\n        bool IRepository<TEntity, TPrimaryKey" +
					">.HasPrimaryKey(TEntity entity) {\r\n            return entityTraits.HasPrimaryKey" +
					"(entity);\r\n        }\r\n        #endregion\r\n    }\r\n}\r\n");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DbRepositoryTemplateBase
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
