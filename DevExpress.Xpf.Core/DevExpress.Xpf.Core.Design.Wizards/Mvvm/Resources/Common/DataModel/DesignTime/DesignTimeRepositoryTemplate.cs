﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	public partial class DesignTimeRepositoryTemplate : DesignTimeRepositoryTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Linq;\r\nusing System.Linq.Expressions;\r\nusing System.C" +
					"ollections.Generic;\r\nusing DevExpress.Mvvm;\r\n");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// DesignTimeRepository is an IRepository interface i" +
					"mplementation representing the collection of entities of a given type for design" +
					"-time mode. \r\n    /// DesignTimeRepository objects are created from a DesignTime" +
					"UnitOfWork class instance using the GetRepository method. \r\n    /// Write operat" +
					"ions against entities of a given type are not supported in this implementation a" +
					"nd throw InvalidOperationException.\r\n    /// </summary>\r\n    /// <typeparam name" +
					"=\"TEntity\">A repository entity type.</typeparam>\r\n    /// <typeparam name=\"TPrim" +
					"aryKey\">An entity primary key type.</typeparam>\r\n    public class DesignTimeRepo" +
					"sitory<TEntity, TPrimaryKey> : DesignTimeReadOnlyRepository<TEntity>, IRepositor" +
					"y<TEntity, TPrimaryKey>\r\n        where TEntity : class {\r\n\r\n        readonly Exp" +
					"ression<Func<TEntity, TPrimaryKey>> getPrimaryKeyExpression;\r\n        readonly E" +
					"ntityTraits<TEntity, TPrimaryKey> entityTraits;\r\n\r\n        /// <summary>\r\n      " +
					"  /// Initializes a new instance of the DesignTimeRepository class.\r\n        ///" +
					" </summary>\r\n        /// <param name=\"getPrimaryKeyExpression\">A lambda-expressi" +
					"on that returns the entity primary key.</param>\r\n        public DesignTimeReposi" +
					"tory(DesignTimeUnitOfWork unitOfWork, Expression<Func<TEntity, TPrimaryKey>> get" +
					"PrimaryKeyExpression)\r\n\t\t\t: base(unitOfWork) {\r\n            this.getPrimaryKeyEx" +
					"pression = getPrimaryKeyExpression;\r\n            this.entityTraits = ExpressionH" +
					"elper.GetEntityTraits(this, getPrimaryKeyExpression);\r\n        }\r\n\r\n        prot" +
					"ected virtual TEntity CreateCore() {\r\n            return DesignTimeHelper.Create" +
					"DesignTimeObject<TEntity>();\r\n        }\r\n\r\n        protected virtual void Update" +
					"Core(TEntity entity) {\r\n        }\r\n\r\n        protected virtual EntityState GetSt" +
					"ateCore(TEntity entity) {\r\n            return EntityState.Detached;\r\n        }\r\n" +
					"\r\n        protected virtual TEntity FindCore(TPrimaryKey primaryKey) {\r\n        " +
					"    throw new InvalidOperationException();\r\n        }\r\n\r\n        protected virtu" +
					"al void RemoveCore(TEntity entity) {\r\n            throw new InvalidOperationExce" +
					"ption();\r\n        }\r\n\r\n        protected virtual TEntity ReloadCore(TEntity enti" +
					"ty) {\r\n            throw new InvalidOperationException();\r\n        }\r\n\r\n        " +
					"protected virtual TPrimaryKey GetPrimaryKeyCore(TEntity entity) {\r\n            r" +
					"eturn entityTraits.GetPrimaryKey(entity);\r\n        }\r\n\r\n        protected virtua" +
					"l void SetPrimaryKeyCore(TEntity entity, TPrimaryKey primaryKey) {\r\n            " +
					"var setPrimaryKeyAction = entityTraits.SetPrimaryKey;\r\n            setPrimaryKey" +
					"Action(entity, primaryKey);\r\n        }\r\n\r\n\t\tprotected virtual void AddCore(TEnti" +
					"ty entity) {\r\n\t\t\tthrow new InvalidOperationException();\r\n\t\t}\r\n\r\n        #region " +
					"IRepository\r\n        TEntity IRepository<TEntity, TPrimaryKey>.Find(TPrimaryKey " +
					"primaryKey) {\r\n            return FindCore(primaryKey);\r\n        }\r\n\r\n\t\tvoid IRe" +
					"pository<TEntity, TPrimaryKey>.Add(TEntity entity) {\r\n            AddCore(entity" +
					");\r\n        }\r\n\r\n        void IRepository<TEntity, TPrimaryKey>.Remove(TEntity e" +
					"ntity) {\r\n            RemoveCore(entity);\r\n        }\r\n\r\n        TEntity IReposit" +
					"ory<TEntity, TPrimaryKey>.Create(bool add) {\r\n            return CreateCore();\r\n" +
					"        }\r\n\r\n        void IRepository<TEntity, TPrimaryKey>.Update(TEntity entit" +
					"y) {\r\n            UpdateCore(entity);\r\n        }\r\n\r\n        EntityState IReposit" +
					"ory<TEntity, TPrimaryKey>.GetState(TEntity entity) {\r\n            return GetStat" +
					"eCore(entity);\r\n        }\r\n\r\n        TEntity IRepository<TEntity, TPrimaryKey>.R" +
					"eload(TEntity entity) {\r\n            return ReloadCore(entity);\r\n        }\r\n    " +
					"    Expression<Func<TEntity, TPrimaryKey>> IRepository<TEntity, TPrimaryKey>.Get" +
					"PrimaryKeyExpression {\r\n            get { return getPrimaryKeyExpression; }\r\n   " +
					"     }\r\n\r\n        TPrimaryKey IRepository<TEntity, TPrimaryKey>.GetPrimaryKey(TE" +
					"ntity entity) {\r\n            return GetPrimaryKeyCore(entity);\r\n        }\r\n\r\n   " +
					"     bool IRepository<TEntity, TPrimaryKey>.HasPrimaryKey(TEntity entity) {\r\n   " +
					"         return entityTraits.HasPrimaryKey(entity);\r\n        }\r\n\r\n        void I" +
					"Repository<TEntity, TPrimaryKey>.SetPrimaryKey(TEntity entity, TPrimaryKey prima" +
					"ryKey) {\r\n            SetPrimaryKeyCore(entity, primaryKey);\r\n        }\r\n       " +
					" #endregion\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class DesignTimeRepositoryTemplateBase
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
