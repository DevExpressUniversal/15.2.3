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
	using System.Linq;
	using System.Text;
	using System.Collections.Generic;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using System;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class InstantFeedbackCollectionViewModelTemplate : InstantFeedbackCollectionViewModelTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write(@"using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataAnnotations;
using System.Collections.ObjectModel;
using DevExpress.Data.Linq;
using System.Collections;
");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    public partial class InstantFeedbackCollectionViewModel<TEntity, TPrimary" +
					"Key, TUnitOfWork> : InstantFeedbackCollectionViewModelBase<TEntity, TEntity, TPr" +
					"imaryKey, TUnitOfWork>\r\n        where TEntity : class, new()\r\n        where TUni" +
					"tOfWork : IUnitOfWork {\r\n\r\n        public static InstantFeedbackCollectionViewMo" +
					"del<TEntity, TPrimaryKey, TUnitOfWork> CreateInstantFeedbackCollectionViewModel(" +
					"\r\n            IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,\r\n            Fu" +
					"nc<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,\t\t\t\r\n\t\t\tFun" +
					"c<IRepositoryQuery<TEntity>, IQueryable<TEntity>> projection = null,\r\n\t\t\tFunc<bo" +
					"ol> canCreateNewEntity = null) {\r\n            return ViewModelSource.Create(() =" +
					"> new InstantFeedbackCollectionViewModel<TEntity, TPrimaryKey, TUnitOfWork>(unit" +
					"OfWorkFactory, getRepositoryFunc, projection, canCreateNewEntity));\r\n        }\r\n" +
					"\r\n        protected InstantFeedbackCollectionViewModel(\r\n            IUnitOfWork" +
					"Factory<TUnitOfWork> unitOfWorkFactory,\r\n            Func<TUnitOfWork, IReposito" +
					"ry<TEntity, TPrimaryKey>> getRepositoryFunc,\r\n\t\t\tFunc<IRepositoryQuery<TEntity>," +
					" IQueryable<TEntity>> projection = null,\r\n\t\t\tFunc<bool> canCreateNewEntity = nul" +
					"l)\r\n            : base(unitOfWorkFactory, getRepositoryFunc, projection, canCrea" +
					"teNewEntity) {\r\n        }\r\n    }\r\n\r\n\tpublic partial class InstantFeedbackCollect" +
					"ionViewModel<TEntity, TProjection, TPrimaryKey, TUnitOfWork> : InstantFeedbackCo" +
					"llectionViewModelBase<TEntity, TProjection, TPrimaryKey, TUnitOfWork>\r\n        w" +
					"here TEntity : class, new()\r\n        where TProjection : class\r\n        where TU" +
					"nitOfWork : IUnitOfWork {\r\n\r\n        public static InstantFeedbackCollectionView" +
					"Model<TEntity, TProjection, TPrimaryKey, TUnitOfWork> CreateInstantFeedbackColle" +
					"ctionViewModel(\r\n            IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,\r" +
					"\n            Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryF" +
					"unc,\r\n            Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> proje" +
					"ction,\r\n\t\t\tFunc<bool> canCreateNewEntity = null) {\r\n            return ViewModel" +
					"Source.Create(() => new InstantFeedbackCollectionViewModel<TEntity, TProjection," +
					" TPrimaryKey, TUnitOfWork>(unitOfWorkFactory, getRepositoryFunc, projection, can" +
					"CreateNewEntity));\r\n        }\r\n\r\n        protected InstantFeedbackCollectionView" +
					"Model(\r\n            IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,\r\n        " +
					"    Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,\r\n   " +
					"         Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> projection,\r\n\t" +
					"\t\tFunc<bool> canCreateNewEntity = null)\r\n\t\t\t: base(unitOfWorkFactory, getReposit" +
					"oryFunc, projection, canCreateNewEntity) {\r\n        }\r\n    }\r\n\r\n    public abstr" +
					"act class InstantFeedbackCollectionViewModelBase<TEntity, TProjection, TPrimaryK" +
					"ey, TUnitOfWork> : IDocumentContent, ISupportLogicalLayout\r\n        where TEntit" +
					"y : class, new()\r\n        where TProjection : class\r\n        where TUnitOfWork :" +
					" IUnitOfWork {\r\n\r\n        #region inner classes\r\n        public class InstantFee" +
					"dbackSourceViewModel : IListSource {\r\n            public static InstantFeedbackS" +
					"ourceViewModel Create(Func<int> getCount, IInstantFeedbackSource<TProjection> so" +
					"urce) {\r\n                return ViewModelSource.Create(() => new InstantFeedback" +
					"SourceViewModel(getCount, source));\r\n            }\r\n\r\n            readonly Func<" +
					"int> getCount;\r\n            readonly IInstantFeedbackSource<TProjection> source;" +
					"\r\n\r\n            protected InstantFeedbackSourceViewModel(Func<int> getCount, IIn" +
					"stantFeedbackSource<TProjection> source) {\r\n                this.getCount = getC" +
					"ount;\r\n                this.source = source;\r\n            }\r\n\r\n            publi" +
					"c int Count { get { return getCount(); } }\r\n\r\n            public void Refresh() " +
					"{\r\n                source.Refresh();\r\n                this.RaisePropertyChanged(" +
					"x => x.Count);\r\n            }\r\n\r\n            bool IListSource.ContainsListCollec" +
					"tion { get { return source.ContainsListCollection; } }\r\n\r\n            IList ILis" +
					"tSource.GetList() {\r\n                return source.GetList();\r\n            }\r\n  " +
					"      }\r\n        #endregion\r\n\r\n        protected readonly IUnitOfWorkFactory<TUn" +
					"itOfWork> unitOfWorkFactory;\r\n        protected readonly Func<TUnitOfWork, IRepo" +
					"sitory<TEntity, TPrimaryKey>> getRepositoryFunc;\r\n        protected Func<IReposi" +
					"toryQuery<TEntity>, IQueryable<TProjection>> Projection { get; private set; }\r\n\t" +
					"\tFunc<bool> canCreateNewEntity;\r\n        readonly IRepository<TEntity, TPrimaryK" +
					"ey> helperRepository;\r\n        readonly IInstantFeedbackSource<TProjection> sour" +
					"ce;\r\n\r\n        protected InstantFeedbackCollectionViewModelBase(\r\n\t\t\tIUnitOfWork" +
					"Factory<TUnitOfWork> unitOfWorkFactory, \r\n\t\t\tFunc<TUnitOfWork, IRepository<TEnti" +
					"ty, TPrimaryKey>> getRepositoryFunc,\r\n\t\t\tFunc<IRepositoryQuery<TEntity>, IQuerya" +
					"ble<TProjection>> projection,\r\n\t\t\tFunc<bool> canCreateNewEntity = null)\r\n\t\t{\r\n  " +
					"          this.unitOfWorkFactory = unitOfWorkFactory;\r\n\t\t\tthis.canCreateNewEntit" +
					"y = canCreateNewEntity;\r\n            this.getRepositoryFunc = getRepositoryFunc;" +
					"\r\n            this.Projection = projection;\r\n\t\t\tthis.helperRepository = CreateRe" +
					"pository();\r\n\r\n            RepositoryExtensions.VerifyProjection(helperRepositor" +
					"y, projection);\r\n\r\n            this.source = unitOfWorkFactory.CreateInstantFeed" +
					"backSource(getRepositoryFunc, Projection);\r\n            this.Entities = InstantF" +
					"eedbackSourceViewModel.Create(() => helperRepository.Count(), source);\r\n\r\n      " +
					"      if(!this.IsInDesignMode())\r\n                OnInitializeInRuntime();\r\n    " +
					"    }\r\n\r\n        public InstantFeedbackSourceViewModel Entities { get; private s" +
					"et; }\r\n        public virtual object SelectedEntity { get; set; }\r\n\r\n        pub" +
					"lic virtual void New() {\r\n\t\t\tif (canCreateNewEntity != null && !canCreateNewEnti" +
					"ty())\r\n\t\t\t\treturn;\r\n            DocumentManagerService.ShowNewEntityDocument<TEn" +
					"tity>(this);\r\n        }\r\n\r\n        public virtual void Edit(object threadSafePro" +
					"xy) {\r\n\t\t\tif(!source.IsLoadedProxy(threadSafeProxy))\r\n\t\t\t\treturn;\r\n            T" +
					"PrimaryKey primaryKey = GetProxyPrimaryKey(threadSafeProxy);\r\n\t\t\tTEntity entity " +
					"= helperRepository.Find(primaryKey);\r\n            if(entity == null) {\r\n        " +
					"        DestroyDocument(DocumentManagerService.FindEntityDocument<TEntity, TPrim" +
					"aryKey>(primaryKey));\r\n                return;\r\n            }\r\n            Docum" +
					"entManagerService.ShowExistingEntityDocument<TEntity, TPrimaryKey>(this, primary" +
					"Key);\r\n        }\r\n\r\n        public virtual bool CanEdit(object threadSafeProxy) " +
					"{\r\n            return threadSafeProxy != null;\r\n        }\r\n\r\n        public virt" +
					"ual void Delete(object threadSafeProxy) {\r\n\t\t\tif(!source.IsLoadedProxy(threadSaf" +
					"eProxy))\r\n\t\t\t\treturn;\r\n            if(MessageBoxService.ShowMessage(string.Forma" +
					"t(CommonResources.Confirmation_Delete, typeof(TEntity).Name), CommonResources.Co" +
					"nfirmation_Caption, MessageButton.YesNo) != MessageResult.Yes)\r\n                " +
					"return;\r\n            try {\r\n                TPrimaryKey primaryKey = GetProxyPri" +
					"maryKey(threadSafeProxy);\r\n                TEntity entity = helperRepository.Fin" +
					"d(primaryKey);\r\n                if(entity != null) {\r\n\t\t\t\t\tOnBeforeEntityDeleted" +
					"(primaryKey, entity);\r\n                    helperRepository.Remove(entity);\r\n   " +
					"                 helperRepository.UnitOfWork.SaveChanges();\r\n                   " +
					" OnEntityDeleted(primaryKey, entity);\r\n                }\r\n            } catch (D" +
					"bException e) {\r\n                Refresh();\r\n                MessageBoxService.S" +
					"howMessage(e.ErrorMessage, e.ErrorCaption, MessageButton.OK, MessageIcon.Error);" +
					"\r\n            }\r\n            Refresh();\r\n        }\r\n\r\n        public virtual boo" +
					"l CanDelete(object threadSafeProxy) {\r\n            return threadSafeProxy != nul" +
					"l;\r\n        }\r\n\r\n\t\tprotected ILayoutSerializationService LayoutSerializationServ" +
					"ice { get { return this.GetService<ILayoutSerializationService>(); } }\r\n\r\n      " +
					"  string ViewName { get { return typeof(TEntity).Name + \"InstantFeedbackCollecti" +
					"onView\"; } }\r\n\r\n        [Display(AutoGenerateField = false)]\r\n        public vir" +
					"tual void OnLoaded() {\r\n            PersistentLayoutHelper.TryDeserializeLayout(" +
					"LayoutSerializationService, ViewName);\r\n        }\r\n\r\n        [Display(AutoGenera" +
					"teField = false)]\r\n        public virtual void OnUnloaded() {\r\n            SaveL" +
					"ayout();\r\n        }\r\n\r\n        void SaveLayout() {\r\n            PersistentLayout" +
					"Helper.TrySerializeLayout(LayoutSerializationService, ViewName);\r\n        }\r\n\r\n " +
					"       public virtual void Refresh() {\r\n            Entities.Refresh();\r\n       " +
					" }\r\n\r\n        protected TPrimaryKey GetProxyPrimaryKey(object threadSafeProxy) {" +
					"\r\n            var expression = RepositoryExtensions.GetSinglePropertyPrimaryKeyP" +
					"rojectionProperty<TEntity, TProjection, TPrimaryKey>(helperRepository);\r\n       " +
					"     return GetProxyPropertyValue(threadSafeProxy, expression);\r\n        }\r\n\r\n\t\t" +
					"protected TProperty GetProxyPropertyValue<TProperty>(object threadSafeProxy, Exp" +
					"ression<Func<TProjection, TProperty>> propertyExpression) {\r\n            return " +
					"source.GetPropertyValue(threadSafeProxy, propertyExpression);\r\n        }\r\n\r\n    " +
					"    protected virtual void OnEntityDeleted(TPrimaryKey primaryKey, TEntity entit" +
					"y) {\r\n            Messenger.Default.Send(new EntityMessage<TEntity, TPrimaryKey>" +
					"(primaryKey, EntityMessageType.Deleted));\r\n        }\r\n\r\n        protected IMessa" +
					"geBoxService MessageBoxService { get { return this.GetRequiredService<IMessageBo" +
					"xService>(); } }\r\n        protected IDocumentManagerService DocumentManagerServi" +
					"ce { get { return this.GetService<IDocumentManagerService>(); } }\r\n\r\n\t\tprotected" +
					" virtual void OnBeforeEntityDeleted(TPrimaryKey primaryKey, TEntity entity) { }\r" +
					"\n\r\n        protected void DestroyDocument(IDocument document) {\r\n            if(" +
					"document != null)\r\n                document.Close();\r\n        }\r\n\r\n        prote" +
					"cted IRepository<TEntity, TPrimaryKey> CreateRepository() {\r\n            return " +
					"getRepositoryFunc(CreateUnitOfWork());\r\n        }\r\n\r\n        protected TUnitOfWo" +
					"rk CreateUnitOfWork() {\r\n            return unitOfWorkFactory.CreateUnitOfWork()" +
					";\r\n        }\r\n\r\n        protected virtual void OnInitializeInRuntime() {\r\n      " +
					"      Messenger.Default.Register<EntityMessage<TEntity, TPrimaryKey>>(this, x =>" +
					" OnMessage(x));\r\n        }\r\n\r\n        protected virtual void OnDestroy() {\r\n    " +
					"        Messenger.Default.Unregister(this);\r\n        }\r\n\r\n        void OnMessage" +
					"(EntityMessage<TEntity, TPrimaryKey> message) {\r\n            Refresh();\r\n       " +
					" }\r\n\r\n        protected IDocumentOwner DocumentOwner { get; private set; }\r\n\r\n  " +
					"      #region IDocumentContent\r\n        object IDocumentContent.Title { get { re" +
					"turn null; } }\r\n\r\n        void IDocumentContent.OnClose(CancelEventArgs e) {\r\n\t\t" +
					"\tSaveLayout();\r\n\t\t}\r\n\r\n        void IDocumentContent.OnDestroy() {\r\n            " +
					"OnDestroy();\r\n        }\r\n\r\n        IDocumentOwner IDocumentContent.DocumentOwner" +
					" {\r\n            get { return DocumentOwner; }\r\n            set { DocumentOwner =" +
					" value; }\r\n        }\r\n        #endregion\r\n\r\n\t\t#region ISupportLogicalLayout\r\n   " +
					"     bool ISupportLogicalLayout.CanSerialize {\r\n            get { return true; }" +
					"\r\n        }\r\n\r\n\t\tIDocumentManagerService ISupportLogicalLayout.DocumentManagerSe" +
					"rvice {\r\n            get { return DocumentManagerService; }\r\n        }\r\n\r\n\t\tIEnu" +
					"merable<object> ISupportLogicalLayout.LookupViewModels {\r\n            get { retu" +
					"rn null; }\r\n        }\r\n        #endregion\r\n\t}\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class InstantFeedbackCollectionViewModelTemplateBase
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
