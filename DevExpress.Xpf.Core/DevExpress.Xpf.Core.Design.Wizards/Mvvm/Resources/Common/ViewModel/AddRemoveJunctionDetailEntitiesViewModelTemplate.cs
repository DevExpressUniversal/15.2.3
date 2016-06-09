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
	public partial class AddRemoveJunctionDetailEntitiesViewModelTemplate : AddRemoveJunctionDetailEntitiesViewModelTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Linq.Expressions;\r\nusing DevExpress.Mvvm;\r\nusing Syst" +
					"em.Collections.Generic;\r\nusing System.Collections.ObjectModel;\r\nusing DevExpress" +
					".Mvvm.POCO;\r\nusing System.Linq;\r\n");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write("{\r\n    public class AddRemoveJunctionDetailEntitiesViewModel<TEntity, TPrimaryKey" +
					", TDetailEntity, TDetailPrimaryKey, TJunction, TJunctionPrimaryKey, TUnitOfWork>" +
					" : AddRemoveDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetail" +
					"PrimaryKey, TUnitOfWork>\r\n        where TEntity : class\r\n        where TDetailEn" +
					"tity : class\r\n        where TJunction : class, new()\r\n        where TJunctionPri" +
					"maryKey : class\r\n        where TUnitOfWork : IUnitOfWork {\r\n\r\n        public sta" +
					"tic AddRemoveJunctionDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity" +
					", TDetailPrimaryKey, TJunction, TJunctionPrimaryKey, TUnitOfWork> CreateViewMode" +
					"l(\r\n            IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory, \r\n           " +
					" Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,\r\n      " +
					"      Func<TUnitOfWork, IRepository<TDetailEntity, TDetailPrimaryKey>> getDetail" +
					"sRepositoryFunc,\r\n            Func<TUnitOfWork, IRepository<TJunction, TJunction" +
					"PrimaryKey>> getJunctionRepositoryFunc,\r\n            Expression<Func<TJunction, " +
					"TPrimaryKey>> getEntityForeignKey,\r\n            Expression<Func<TJunction, TDeta" +
					"ilPrimaryKey>> getDetailForeignKey,\r\n            TPrimaryKey id)\r\n        {\r\n   " +
					"         return ViewModelSource.Create(() => new AddRemoveJunctionDetailEntities" +
					"ViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TJunction, TJu" +
					"nctionPrimaryKey, TUnitOfWork>(unitOfWorkFactory, getRepositoryFunc, getDetailsR" +
					"epositoryFunc, getJunctionRepositoryFunc, getEntityForeignKey, getDetailForeignK" +
					"ey, id));\r\n        }\r\n        \r\n        readonly Func<TUnitOfWork, IRepository<T" +
					"Junction, TJunctionPrimaryKey>> getJunctionRepositoryFunc;\r\n        readonly Exp" +
					"ression<Func<TJunction, TPrimaryKey>> getEntityForeignKey;\r\n        readonly Exp" +
					"ression<Func<TJunction, TDetailPrimaryKey>> getDetailForeignKey;\r\n        \r\n    " +
					"    IRepository<TDetailEntity, TDetailPrimaryKey> DetailsRepository { get { retu" +
					"rn getDetailsRepositoryFunc(UnitOfWork); } }\r\n        IRepository<TJunction, TJu" +
					"nctionPrimaryKey> JunctionRepository { get { return getJunctionRepositoryFunc(Un" +
					"itOfWork); } }\r\n\t\tpublic override bool IsCreateDetailButtonVisible { get { retur" +
					"n false; } }\r\n\r\n        public override ICollection<TDetailEntity> DetailEntitie" +
					"s {\r\n            get {\r\n\t\t\t    if(Entity == null)\r\n                    return En" +
					"umerable.Empty<TDetailEntity>().ToArray();\r\n                var entityPrimaryKey" +
					" = Repository.GetPrimaryKey(Entity);\r\n                var junctions = JunctionRe" +
					"pository.Where(GetJunctionPredicate(entityPrimaryKey));\r\n                return " +
					"junctions.Join(DetailsRepository, getDetailForeignKey, DetailsRepository.GetPrim" +
					"aryKeyExpression, (j, d) => d).ToArray();\r\n            }\r\n        }\r\n\r\n        p" +
					"rotected AddRemoveJunctionDetailEntitiesViewModel(\r\n                IUnitOfWorkF" +
					"actory<TUnitOfWork> unitOfWorkFactory,\r\n                Func<TUnitOfWork, IRepos" +
					"itory<TEntity, TPrimaryKey>> getRepositoryFunc,\r\n                Func<TUnitOfWor" +
					"k, IRepository<TDetailEntity, TDetailPrimaryKey>> getDetailsRepositoryFunc,\r\n   " +
					"             Func<TUnitOfWork, IRepository<TJunction, TJunctionPrimaryKey>> getJ" +
					"unctionRepositoryFunc,\r\n                Expression<Func<TJunction, TPrimaryKey>>" +
					" getEntityForeignKey,\r\n                Expression<Func<TJunction, TDetailPrimary" +
					"Key>> getDetailForeignKey,\r\n                TPrimaryKey id)\r\n            : base(" +
					"unitOfWorkFactory, getRepositoryFunc, getDetailsRepositoryFunc, null, id)\r\n     " +
					"   {\r\n            this.getJunctionRepositoryFunc = getJunctionRepositoryFunc;\r\n " +
					"           this.getEntityForeignKey = getEntityForeignKey;\r\n            this.get" +
					"DetailForeignKey = getDetailForeignKey;\r\n        }\r\n\r\n        Expression<Func<TJ" +
					"unction, bool>> GetJunctionPredicate(TPrimaryKey primaryKey) {\r\n            var " +
					"param = Expression.Parameter(typeof(TJunction));\r\n            var entityForeignK" +
					"eyExpr = Expression.Property(param, ExpressionHelper.GetPropertyName(getEntityFo" +
					"reignKey));\r\n            var expr = Expression.Equal(entityForeignKeyExpr, Expre" +
					"ssion.Constant(primaryKey));\r\n            return Expression.Lambda<Func<TJunctio" +
					"n, bool>>(expr, param);\r\n        }\r\n\r\n        Expression<Func<TJunction, bool>> " +
					"GetJunctionPredicate(TPrimaryKey primaryKey, TDetailPrimaryKey detailPrimaryKey)" +
					" {\r\n            var param = Expression.Parameter(typeof(TJunction));\r\n          " +
					"  var entityForeignKeyExpr = Expression.Property(param, ExpressionHelper.GetProp" +
					"ertyName(getEntityForeignKey));\r\n            var expr = Expression.Equal(entityF" +
					"oreignKeyExpr, Expression.Constant(primaryKey));\r\n            var detailForeignK" +
					"eyExpr = Expression.Property(param, ExpressionHelper.GetPropertyName(getDetailFo" +
					"reignKey));\r\n            var detailEqual = Expression.Equal(detailForeignKeyExpr" +
					", Expression.Constant(detailPrimaryKey));\r\n            expr = Expression.And(exp" +
					"r, detailEqual);\r\n            return Expression.Lambda<Func<TJunction, bool>>(ex" +
					"pr, param);\r\n        }\r\n\r\n        public override void AddDetailEntities() {\r\n  " +
					"          var availableEntities = DetailsRepository.ToList().Except(DetailEntiti" +
					"es).ToArray();\r\n            var selectEntitiesViewModel = new SelectDetailEntiti" +
					"esViewModel<TDetailEntity>(availableEntities);\r\n            if(DialogService.Sho" +
					"wDialog(MessageButton.OKCancel, CommonResources.AddRemoveDetailEntities_SelectOb" +
					"jects, selectEntitiesViewModel) == MessageResult.OK && selectEntitiesViewModel.S" +
					"electedEntities.Any()) {\r\n                foreach(var selectedEntity in selectEn" +
					"titiesViewModel.SelectedEntities) {\r\n                    var junction = new TJun" +
					"ction();\r\n                    var entityKey = Repository.GetPrimaryKey(Entity);\r" +
					"\n                    var detailKey = DetailsRepository.GetPrimaryKey(selectedEnt" +
					"ity);\r\n                    var junctionType = typeof(TJunction);\r\n              " +
					"      junctionType.GetProperty(ExpressionHelper.GetPropertyName(getEntityForeign" +
					"Key)).SetValue(junction, entityKey, null);\r\n                    junctionType.Get" +
					"Property(ExpressionHelper.GetPropertyName(getDetailForeignKey)).SetValue(junctio" +
					"n, detailKey, null);\r\n                    JunctionRepository.Add(junction);\r\n   " +
					"             }\r\n                SaveChangesAndNotify(selectEntitiesViewModel.Sel" +
					"ectedEntities);\r\n            }\r\n        }\r\n\r\n        public override void Remove" +
					"DetailEntities() {\r\n            if(!SelectedEntities.Any())\r\n                ret" +
					"urn;\r\n            var entityKey = Repository.GetPrimaryKey(Entity);\r\n           " +
					" foreach(var selectedEntity in SelectedEntities) {\r\n                var detailKe" +
					"y = DetailsRepository.GetPrimaryKey(selectedEntity);\r\n                var juncti" +
					"on = JunctionRepository.First(GetJunctionPredicate(entityKey, detailKey));\r\n    " +
					"            JunctionRepository.Remove(junction);\r\n            }\r\n            Sav" +
					"eChangesAndNotify(SelectedEntities);\r\n            SelectedEntities.Clear();\r\n   " +
					"     }\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class AddRemoveJunctionDetailEntitiesViewModelTemplateBase
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
