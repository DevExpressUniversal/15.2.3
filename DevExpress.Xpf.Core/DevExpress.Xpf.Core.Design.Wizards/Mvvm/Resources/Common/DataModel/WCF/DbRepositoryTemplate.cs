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
	public partial class DbRepositoryTemplate : DbRepositoryTemplateBase
	{
		public virtual string TransformText()
		{
this.PasteUsingList("System.Data.Services.Client");
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    public class DbRepository<TEntity, TPrimaryKey, TDbContext> : DbReadOnlyR" +
					"epository<TEntity, TDbContext>, IRepository<TEntity, TPrimaryKey>\r\n        where" +
					" TEntity : class, new()\r\n        where TDbContext : DataServiceContext {\r\n\r\n    " +
					"    readonly string dbSetName;\r\n        readonly Expression<Func<TEntity, TPrima" +
					"ryKey>> getPrimaryKeyExpression;\r\n        readonly EntityTraits<TEntity, TPrimar" +
					"yKey> entityTraits;\r\n\r\n        public DbRepository(DbUnitOfWork<TDbContext> unit" +
					"OfWork, Expression<Func<TDbContext, DataServiceQuery<TEntity>>> dbSetAccessorExp" +
					"ression, Expression<Func<TEntity, TPrimaryKey>> getPrimaryKeyExpression, bool us" +
					"eExtendedDataQuery)\r\n            : base(unitOfWork, dbSetAccessorExpression.Comp" +
					"ile(), useExtendedDataQuery) {\r\n                Expression body = dbSetAccessorE" +
					"xpression.Body;\r\n                while(body is MethodCallExpression) {\r\n        " +
					"            body = ((MethodCallExpression)body).Object;\r\n                }\r\n    " +
					"            this.dbSetName = ((MemberExpression)body).Member.Name;\r\n            " +
					"    this.getPrimaryKeyExpression = getPrimaryKeyExpression;\r\n                thi" +
					"s.entityTraits = ExpressionHelper.GetEntityTraits(this, getPrimaryKeyExpression)" +
					";\r\n        }\t\t\r\n        TEntity IRepository<TEntity, TPrimaryKey>.Find(TPrimaryK" +
					"ey primaryKey) {\r\n            try {\r\n                var entity = LocalCollectio" +
					"n.SingleOrDefault(x => object.Equals(GetPrimaryKeyCore(x), primaryKey));\r\n      " +
					"          if(entity != null)\r\n                    return entity;\r\n              " +
					"  entity = FindCore(primaryKey);\r\n                if(entity != null)\r\n          " +
					"          LocalCollection.Load(entity);\r\n                return entity;\r\n       " +
					"     } catch(DataServiceQueryException) {\r\n                return null;\r\n       " +
					"     }\r\n        }\r\n\t\tvoid IRepository<TEntity, TPrimaryKey>.Add(TEntity entity) " +
					"{\r\n            AddCore(entity);\r\n        }\r\n        void IRepository<TEntity, TP" +
					"rimaryKey>.Remove(TEntity entity) {\r\n            RemoveCore(entity);\r\n        }\r" +
					"\n\t\tTEntity IRepository<TEntity, TPrimaryKey>.Create(bool add) {\r\n            ret" +
					"urn CreateCore(add);\r\n        }        \r\n        void IRepository<TEntity, TPrim" +
					"aryKey>.Update(TEntity entity) {\r\n            UpdateCore(entity);\r\n        }\r\n  " +
					"      EntityState IRepository<TEntity, TPrimaryKey>.GetState(TEntity entity) {\r\n" +
					"            return GetStateCore(entity);\r\n        }\r\n        Expression<Func<TEn" +
					"tity, TPrimaryKey>> IRepository<TEntity, TPrimaryKey>.GetPrimaryKeyExpression {\r" +
					"\n            get { return this.getPrimaryKeyExpression; }\r\n        }\r\n        vo" +
					"id IRepository<TEntity, TPrimaryKey>.SetPrimaryKey(TEntity entity, TPrimaryKey p" +
					"rimaryKey) {\r\n            SetPrimaryKeyCore(entity, primaryKey);\r\n        }\r\n   " +
					"     TPrimaryKey IRepository<TEntity, TPrimaryKey>.GetPrimaryKey(TEntity entity)" +
					" {\r\n            return GetPrimaryKeyCore(entity);\r\n        }\r\n        bool IRepo" +
					"sitory<TEntity, TPrimaryKey>.HasPrimaryKey(TEntity entity) {\r\n            return" +
					" entityTraits.HasPrimaryKey(entity);\r\n        }\r\n\t\tprotected virtual void AddCor" +
					"e(TEntity entity) {\r\n\t\t\tLocalCollection.Add(entity);\r\n\t\t}\r\n        protected vir" +
					"tual TEntity CreateCore(bool add) {\r\n            TEntity newEntity = new TEntity" +
					"();\r\n\t\t\tif(add) {\r\n                AddCore(newEntity);\r\n            }\r\n         " +
					"   return newEntity;\r\n        }\r\n        protected virtual void UpdateCore(TEnti" +
					"ty entity) {\r\n            UnitOfWork.Context.UpdateObject(entity);\r\n        }\r\n " +
					"       protected virtual EntityState GetStateCore(TEntity entity) {\r\n           " +
					" var descriptor = UnitOfWork.Context.GetEntityDescriptor(entity);\r\n            r" +
					"eturn descriptor != null ? GetEntityState(descriptor.State) : EntityState.Detach" +
					"ed;\r\n        }\r\n        static EntityState GetEntityState(EntityStates entitySta" +
					"tes) {\r\n            switch(entityStates) {\r\n                case EntityStates.Ad" +
					"ded:\r\n                    return EntityState.Added;\r\n                case Entity" +
					"States.Deleted:\r\n                    return EntityState.Deleted;\r\n              " +
					"  case EntityStates.Detached:\r\n                    return EntityState.Detached;\r" +
					"\n                case EntityStates.Modified:\r\n                    return EntityS" +
					"tate.Modified;\r\n                case EntityStates.Unchanged:\r\n                  " +
					"  return EntityState.Unchanged;\r\n                default:\r\n                    t" +
					"hrow new NotImplementedException();\r\n            }\r\n        }\r\n        protected" +
					" virtual TEntity FindCore(TPrimaryKey primaryKey) {\r\n            return DbSet.Wh" +
					"ere(ExpressionHelper.GetKeyEqualsExpression<TEntity, TEntity, TPrimaryKey>(getPr" +
					"imaryKeyExpression, primaryKey)).Take(1).ToArray().FirstOrDefault();\r\n        }\r" +
					"\n        protected virtual void RemoveCore(TEntity entity) {\r\n            try {\r" +
					"\n                LocalCollection.Remove(entity);\r\n            }\r\n            cat" +
					"ch(Exception ex) {\r\n                throw DbExceptionsConverter.Convert(ex);\r\n  " +
					"          }\r\n        }\r\n        protected virtual TPrimaryKey GetPrimaryKeyCore(" +
					"TEntity entity) {\r\n            return entityTraits.GetPrimaryKey(entity);\r\n     " +
					"   }\r\n        protected virtual void SetPrimaryKeyCore(TEntity entity, TPrimaryK" +
					"ey primaryKey) {\r\n\t\t\tvar setPrimaryKeyAction = entityTraits.SetPrimaryKey;\r\n    " +
					"        setPrimaryKeyAction(entity, primaryKey);\r\n        }\r\n        TEntity IRe" +
					"pository<TEntity, TPrimaryKey>.Reload(TEntity entity) {\r\n            int index =" +
					" this.LocalCollection.IndexOf(entity);\r\n            UnitOfWork.Context.Detach(en" +
					"tity);\r\n            TEntity newEntity = FindCore(GetPrimaryKeyCore(entity));\r\n  " +
					"          if(newEntity == null)\r\n                LocalCollection.RemoveAt(index)" +
					";\r\n            else if(index >= 0)\r\n                LocalCollection[index] = new" +
					"Entity;\r\n            return newEntity;\r\n        }\r\n    }\r\n}");
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
