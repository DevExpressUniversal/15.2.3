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
	public partial class AddRemoveDetailEntitiesViewModelTemplate : AddRemoveDetailEntitiesViewModelTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write("using System;\r\nusing System.Linq.Expressions;\r\nusing DevExpress.Mvvm;\r\nusing Syst" +
					"em.Collections.Generic;\r\nusing System.Collections.ObjectModel;\r\nusing DevExpress" +
					".Mvvm.POCO;\r\nusing System.Linq;\r\n");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write("{\r\n    public class AddRemoveDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetai" +
					"lEntity, TDetailPrimaryKey, TUnitOfWork> : SingleObjectViewModelBase<TEntity, TP" +
					"rimaryKey, TUnitOfWork>\r\n        where TEntity : class\r\n        where TDetailEnt" +
					"ity : class\r\n        where TUnitOfWork : IUnitOfWork {\r\n\r\n        public static " +
					"AddRemoveDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetailPri" +
					"maryKey, TUnitOfWork> Create(IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory, " +
					"Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc, Func<TUn" +
					"itOfWork, IRepository<TDetailEntity, TDetailPrimaryKey>> getDetailsRepositoryFun" +
					"c, Func<TEntity, ICollection<TDetailEntity>> getDetailsFunc, TPrimaryKey id) {\r\n" +
					"            return ViewModelSource.Create(() => new AddRemoveDetailEntitiesViewM" +
					"odel<TEntity, TPrimaryKey, TDetailEntity, TDetailPrimaryKey, TUnitOfWork>(unitOf" +
					"WorkFactory, getRepositoryFunc, getDetailsRepositoryFunc, getDetailsFunc, id));\r" +
					"\n        }\r\n\r\n        protected readonly Func<TUnitOfWork, IRepository<TDetailEn" +
					"tity, TDetailPrimaryKey>> getDetailsRepositoryFunc;\r\n        readonly Func<TEnti" +
					"ty, ICollection<TDetailEntity>> getDetailsFunc;\r\n\r\n        protected IDialogServ" +
					"ice DialogService { get { return this.GetRequiredService<IDialogService>(); } }\r" +
					"\n\t\tprotected IDocumentManagerService DocumentManagerService { get { return this." +
					"GetRequiredService<IDocumentManagerService>(); } }\r\n\r\n        IRepository<TDetai" +
					"lEntity, TDetailPrimaryKey> DetailsRepository { get { return getDetailsRepositor" +
					"yFunc(UnitOfWork); } }\r\n\r\n        public virtual ICollection<TDetailEntity> Deta" +
					"ilEntities { get { return Entity != null ? getDetailsFunc(Entity) : Enumerable.E" +
					"mpty<TDetailEntity>().ToArray(); } }\r\n        public ObservableCollection<TDetai" +
					"lEntity> SelectedEntities { get; private set; }\r\n\t\tpublic virtual bool IsCreateD" +
					"etailButtonVisible { get { return true; } }\r\n\r\n        protected AddRemoveDetail" +
					"EntitiesViewModel(IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory, Func<TUnitO" +
					"fWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc, Func<TUnitOfWork, I" +
					"Repository<TDetailEntity, TDetailPrimaryKey>> getDetailsRepositoryFunc, Func<TEn" +
					"tity, ICollection<TDetailEntity>> getDetailsFunc, TPrimaryKey id)\r\n            :" +
					" base(unitOfWorkFactory, getRepositoryFunc, null) {\r\n            this.getDetails" +
					"RepositoryFunc = getDetailsRepositoryFunc;\r\n            this.getDetailsFunc = ge" +
					"tDetailsFunc;\r\n            SelectedEntities = new ObservableCollection<TDetailEn" +
					"tity>();\r\n            if(this.IsInDesignMode())\r\n                return;\r\n      " +
					"      LoadEntityByKey(id);\r\n            Messenger.Default.Register(this, (Entity" +
					"Message<TDetailEntity, TDetailPrimaryKey> m) => {\r\n                if(m.MessageT" +
					"ype != EntityMessageType.Added)\r\n                    return;\r\n                va" +
					"r withParent = m.Sender as ISupportParentViewModel;\r\n                if(withPare" +
					"nt == null || withParent.ParentViewModel != this)\r\n                    return;\r\n" +
					"                var withEntity = m.Sender as SingleObjectViewModel<TDetailEntity" +
					", TDetailPrimaryKey, TUnitOfWork>;\r\n                var detailEntity = DetailsRe" +
					"pository.Find(DetailsRepository.GetPrimaryKey(withEntity.Entity));\r\n            " +
					"    if(detailEntity == null)\r\n                    return;\r\n                Detai" +
					"lEntities.Add(detailEntity);\r\n                SaveChangesAndNotify(new TDetailEn" +
					"tity[] { detailEntity });\r\n            });\r\n        }\r\n\r\n        public virtual " +
					"void CreateDetailEntity() {\r\n            DocumentManagerService.ShowNewEntityDoc" +
					"ument<TDetailEntity>(this);\r\n        }\r\n\r\n        public virtual void EditDetail" +
					"Entity() {\r\n            if(SelectedEntities.Any()) {\r\n                var detail" +
					"Key = DetailsRepository.GetPrimaryKey(SelectedEntities.First());\r\n              " +
					"  DocumentManagerService.ShowExistingEntityDocument<TDetailEntity, TDetailPrimar" +
					"yKey>(this, detailKey);\r\n            }\r\n        }\r\n\r\n        protected override " +
					"void OnInitializeInRuntime() {\r\n            base.OnInitializeInRuntime();\r\n     " +
					"       Messenger.Default.Register<EntityMessage<TEntity, TPrimaryKey>>(this, m =" +
					"> OnMessage(m));\r\n        }\r\n\r\n        public virtual void AddDetailEntities() {" +
					"\r\n            var availalbleEntities = DetailsRepository.ToList().Except(DetailE" +
					"ntities).ToArray();\r\n            var selectEntitiesViewModel = new SelectDetailE" +
					"ntitiesViewModel<TDetailEntity>(availalbleEntities);\r\n            if(DialogServi" +
					"ce.ShowDialog(MessageButton.OKCancel, CommonResources.AddRemoveDetailEntities_Se" +
					"lectObjects, selectEntitiesViewModel) == MessageResult.OK && selectEntitiesViewM" +
					"odel.SelectedEntities.Any()) {\r\n                foreach(var selectedEntity in se" +
					"lectEntitiesViewModel.SelectedEntities) {\r\n                    DetailEntities.Ad" +
					"d(selectedEntity);\r\n                }\r\n                SaveChangesAndNotify(sele" +
					"ctEntitiesViewModel.SelectedEntities);\r\n            }\r\n        }\r\n\r\n        publ" +
					"ic bool CanAddDetailEntities() {\r\n            return Entity != null;\r\n        }\r" +
					"\n\r\n        public virtual void RemoveDetailEntities() {\r\n            if(!Selecte" +
					"dEntities.Any())\r\n                return;\r\n            foreach(var selectedEntit" +
					"y in SelectedEntities) {\r\n                DetailEntities.Remove(selectedEntity);" +
					"\r\n            }\r\n            SaveChangesAndNotify(SelectedEntities);\r\n          " +
					"  SelectedEntities.Clear();\r\n        }\r\n\r\n        public bool CanRemoveDetailEnt" +
					"ities() {\r\n            return Entity != null;\r\n        }\r\n\r\n        protected vo" +
					"id SaveChangesAndNotify(IEnumerable<TDetailEntity> detailEntities) {\r\n          " +
					"  try {\r\n                UnitOfWork.SaveChanges();\r\n                foreach(var " +
					"detailEntity in detailEntities) {\r\n                    Messenger.Default.Send(ne" +
					"w EntityMessage<TDetailEntity, TDetailPrimaryKey>(DetailsRepository.GetPrimaryKe" +
					"y(detailEntity), EntityMessageType.Changed));\r\n                }\r\n              " +
					"  Reload();\r\n            } catch (DbException e) {\r\n                MessageBoxSe" +
					"rvice.ShowMessage(e.ErrorMessage, e.ErrorCaption, MessageButton.OK, MessageIcon." +
					"Error);\r\n            }\r\n        }\r\n\r\n        void OnMessage(EntityMessage<TEntit" +
					"y, TPrimaryKey> message) {\r\n            if(message.MessageType == EntityMessageT" +
					"ype.Changed && Entity != null && object.Equals(PrimaryKey, message.PrimaryKey))\r" +
					"\n                Reload();\r\n        }\r\n\r\n        protected override void OnEntit" +
					"yChanged() {\r\n            base.OnEntityChanged();\r\n            this.RaisePropert" +
					"yChanged(x => x.DetailEntities);\r\n        }\r\n    }\r\n    public class SelectDetai" +
					"lEntitiesViewModel<TEntity> where TEntity : class {\r\n        public SelectDetail" +
					"EntitiesViewModel(TEntity[] availableCourses) {\r\n            AvailableEntities =" +
					" availableCourses;\r\n            SelectedEntities = new List<TEntity>();\r\n       " +
					" }\r\n        public TEntity[] AvailableEntities { get; private set; }\r\n        pu" +
					"blic List<TEntity> SelectedEntities { get; private set; }\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class AddRemoveDetailEntitiesViewModelTemplateBase
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
