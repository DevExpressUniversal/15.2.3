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
	public partial class EntitiesViewModelTemplate : EntitiesViewModelTemplateBase
	{
		public virtual string TransformText()
		{
			this.Write(@"using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataAnnotations;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// The base class for POCO view models exposing a col" +
					"lection of entities of the given type.\r\n    /// This is a partial class that pro" +
					"vides an extension point to add custom properties, commands and override methods" +
					" without modifying the auto-generated code.\r\n    /// </summary>\r\n    /// <typepa" +
					"ram name=\"TEntity\">A repository entity type.</typeparam>\r\n    /// <typeparam nam" +
					"e=\"TProjection\">A projection entity type.</typeparam>\r\n    /// <typeparam name=\"" +
					"TUnitOfWork\">A unit of work type.</typeparam>\r\n    public abstract partial class" +
					" EntitiesViewModel<TEntity, TProjection, TUnitOfWork> :\r\n        EntitiesViewMod" +
					"elBase<TEntity, TProjection, TUnitOfWork>\r\n        where TEntity : class\r\n      " +
					"  where TProjection : class\r\n        where TUnitOfWork : IUnitOfWork {\r\n\r\n      " +
					"  /// <summary>\r\n        /// Initializes a new instance of the EntitiesViewModel" +
					" class.\r\n        /// </summary>\r\n        /// <param name=\"unitOfWorkFactory\">A f" +
					"actory used to create a unit of work instance.</param>\r\n        /// <param name=" +
					"\"getRepositoryFunc\">A function that returns a repository representing entities o" +
					"f the given type.</param>\r\n        /// <param name=\"projection\">A LINQ function " +
					"used to customize a query for entities. The parameter, for example, can be used " +
					"for sorting data and/or for projecting data to a custom type that does not match" +
					" the repository entity type.</param>\r\n        protected EntitiesViewModel(\r\n    " +
					"        IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory, \r\n            Func<TU" +
					"nitOfWork, IReadOnlyRepository<TEntity>> getRepositoryFunc, \r\n            Func<I" +
					"RepositoryQuery<TEntity>, IQueryable<TProjection>> projection) \r\n            : b" +
					"ase(unitOfWorkFactory, getRepositoryFunc, projection) {\r\n        }\r\n    }\r\n\r\n   " +
					" /// <summary>\r\n    /// The base class for a POCO view models exposing a collect" +
					"ion of entities of the given type.\r\n    /// It is not recommended to inherit dir" +
					"ectly from this class. Use the EntitiesViewModel class instead.\r\n    /// </summa" +
					"ry>\r\n    /// <typeparam name=\"TEntity\">A repository entity type.</typeparam>\r\n  " +
					"  /// <typeparam name=\"TProjection\">A projection entity type.</typeparam>\r\n    /" +
					"// <typeparam name=\"TUnitOfWork\">A unit of work type.</typeparam>\r\n    [POCOView" +
					"Model]\r\n    public abstract class EntitiesViewModelBase<TEntity, TProjection, TU" +
					"nitOfWork> : IEntitiesViewModel<TProjection>\r\n        where TEntity : class\r\n   " +
					"     where TProjection : class\r\n        where TUnitOfWork : IUnitOfWork {\r\n\r\n   " +
					"     #region inner classes\r\n        protected interface IEntitiesChangeTracker {" +
					"\r\n            void RegisterMessageHandler();\r\n            void UnregisterMessage" +
					"Handler();\r\n        }\r\n\r\n        protected class EntitiesChangeTracker<TPrimaryK" +
					"ey> : IEntitiesChangeTracker {\r\n\r\n            readonly EntitiesViewModelBase<TEn" +
					"tity, TProjection, TUnitOfWork> owner;\r\n            ObservableCollection<TProjec" +
					"tion> Entities { get { return owner.Entities; } }\r\n            IRepository<TEnti" +
					"ty, TPrimaryKey> Repository { get { return (IRepository<TEntity, TPrimaryKey>)ow" +
					"ner.ReadOnlyRepository; } }\r\n\r\n            public EntitiesChangeTracker(Entities" +
					"ViewModelBase<TEntity, TProjection, TUnitOfWork> owner) {\r\n                this." +
					"owner = owner;\r\n            }\r\n\r\n            void IEntitiesChangeTracker.Registe" +
					"rMessageHandler() {\r\n                Messenger.Default.Register<EntityMessage<TE" +
					"ntity, TPrimaryKey>>(this, x => OnMessage(x));\r\n            }\r\n\r\n            voi" +
					"d IEntitiesChangeTracker.UnregisterMessageHandler() {\r\n                Messenger" +
					".Default.Unregister(this);\r\n            }\r\n\r\n            public TProjection Find" +
					"LocalProjectionByKey(TPrimaryKey primaryKey) {\r\n                var primaryKeyEq" +
					"ualsExpression = RepositoryExtensions.GetProjectionPrimaryKeyEqualsExpression<TE" +
					"ntity, TProjection, TPrimaryKey>(Repository, primaryKey);\r\n                retur" +
					"n Entities.AsQueryable().FirstOrDefault(primaryKeyEqualsExpression);\r\n          " +
					"  }\r\n\r\n            public TProjection FindActualProjectionByKey(TPrimaryKey prim" +
					"aryKey) {\r\n                var projectionEntity = Repository.FindActualProjectio" +
					"nByKey(owner.Projection, primaryKey);\r\n                if(projectionEntity != nu" +
					"ll && ExpressionHelper.IsFitEntity(Repository.Find(primaryKey), owner.GetFilterE" +
					"xpression())) {\r\n                    owner.OnEntitiesLoaded(GetUnitOfWork(Reposi" +
					"tory), new TProjection[] { projectionEntity });\r\n                    return proj" +
					"ectionEntity;\r\n                }\r\n                return null;\r\n            }\r\n\r" +
					"\n            void OnMessage(EntityMessage<TEntity, TPrimaryKey> message) {\r\n    " +
					"            if(!owner.IsLoaded)\r\n                    return;\r\n                sw" +
					"itch(message.MessageType) {\r\n                    case EntityMessageType.Added:\r\n" +
					"                        OnEntityAdded(message.PrimaryKey);\r\n                    " +
					"    break;\r\n                    case EntityMessageType.Changed:\r\n               " +
					"         OnEntityChanged(message.PrimaryKey);\r\n                        break;\r\n " +
					"                   case EntityMessageType.Deleted:\r\n                        OnEn" +
					"tityDeleted(message.PrimaryKey);\r\n                        break;\r\n              " +
					"  }\r\n            }\r\n\r\n            void OnEntityAdded(TPrimaryKey primaryKey) {\r\n" +
					"                var projectionEntity = FindActualProjectionByKey(primaryKey);\r\n " +
					"               if(projectionEntity != null)\r\n                    Entities.Add(pr" +
					"ojectionEntity);\r\n            }\r\n\r\n            void OnEntityChanged(TPrimaryKey " +
					"primaryKey) {\r\n                var existingProjectionEntity = FindLocalProjectio" +
					"nByKey(primaryKey);\r\n                var projectionEntity = FindActualProjection" +
					"ByKey(primaryKey);\r\n                if(projectionEntity == null) {\r\n            " +
					"        Entities.Remove(existingProjectionEntity);\r\n                    return;\r" +
					"\n                }\r\n                if(existingProjectionEntity != null) {\r\n    " +
					"                Entities[Entities.IndexOf(existingProjectionEntity)] = projectio" +
					"nEntity;\r\n                    owner.RestoreSelectedEntity(existingProjectionEnti" +
					"ty, projectionEntity);\r\n                    return;\r\n                }\r\n        " +
					"        OnEntityAdded(primaryKey);\r\n            }\r\n\r\n            void OnEntityDe" +
					"leted(TPrimaryKey primaryKey) {\r\n                Entities.Remove(FindLocalProjec" +
					"tionByKey(primaryKey));\r\n            }\r\n        }\r\n        #endregion\r\n\r\n       " +
					" ObservableCollection<TProjection> entities = new ObservableCollection<TProjecti" +
					"on>();\r\n        CancellationTokenSource loadCancellationTokenSource;\r\n        pr" +
					"otected readonly IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory;\r\n        pro" +
					"tected readonly Func<TUnitOfWork, IReadOnlyRepository<TEntity>> getRepositoryFun" +
					"c;\r\n        protected Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> P" +
					"rojection { get; private set; }\r\n\r\n        /// <summary>\r\n        /// Initialize" +
					"s a new instance of the EntitiesViewModelBase class.\r\n        /// </summary>\r\n  " +
					"      /// <param name=\"unitOfWorkFactory\">A factory used to create a unit of wor" +
					"k instance.</param>\r\n        /// <param name=\"getRepositoryFunc\">A function that" +
					" returns a repository representing entities of the given type.</param>\r\n        " +
					"/// <param name=\"projection\">A LINQ function used to customize a query for entit" +
					"ies. The parameter, for example, can be used for sorting data and/or for project" +
					"ing data to a custom type that does not match the repository entity type.</param" +
					">\r\n        protected EntitiesViewModelBase(\r\n            IUnitOfWorkFactory<TUni" +
					"tOfWork> unitOfWorkFactory,\r\n            Func<TUnitOfWork, IReadOnlyRepository<T" +
					"Entity>> getRepositoryFunc,\r\n            Func<IRepositoryQuery<TEntity>, IQuerya" +
					"ble<TProjection>> projection\r\n            ) {\r\n            this.unitOfWorkFactor" +
					"y = unitOfWorkFactory;\r\n            this.getRepositoryFunc = getRepositoryFunc;\r" +
					"\n            this.Projection = projection;\r\n            this.ChangeTracker = Cre" +
					"ateEntitiesChangeTracker();\r\n            if(!this.IsInDesignMode())\r\n           " +
					"     OnInitializeInRuntime();\r\n        }\r\n\r\n        /// <summary>\r\n        /// U" +
					"sed to check whether entities are currently being loaded in the background. The " +
					"property can be used to show the progress indicator.\r\n        /// </summary>\r\n  " +
					"      public virtual bool IsLoading { get; protected set; }\r\n\r\n        /// <summ" +
					"ary>\r\n        /// The collection of entities loaded from the unit of work.\r\n    " +
					"    /// </summary>\r\n        public ObservableCollection<TProjection> Entities {\r" +
					"\n            get {\r\n                if(!IsLoaded)\r\n                    LoadEntit" +
					"ies(false);\r\n                return entities;\r\n            }\r\n        }\r\n\r\n     " +
					"   protected IEntitiesChangeTracker ChangeTracker { get; private set; }\r\n\r\n     " +
					"   protected IReadOnlyRepository<TEntity> ReadOnlyRepository { get; private set;" +
					" }\r\n\r\n        protected bool IsLoaded { get { return ReadOnlyRepository != null;" +
					" } }\r\n\r\n        protected void LoadEntities(bool forceLoad) {\r\n            if(fo" +
					"rceLoad) {\r\n                if(loadCancellationTokenSource != null)\r\n           " +
					"         loadCancellationTokenSource.Cancel();\r\n            } else if(IsLoading)" +
					" {\r\n                return;\r\n            }\r\n            loadCancellationTokenSou" +
					"rce = LoadCore();\r\n        }\r\n\r\n        void CancelLoading() {\r\n            if(l" +
					"oadCancellationTokenSource != null)\r\n                loadCancellationTokenSource" +
					".Cancel();\r\n            IsLoading = false;\r\n        }\r\n\r\n        CancellationTok" +
					"enSource LoadCore() {\r\n            IsLoading = true;\r\n            var cancellati" +
					"onTokenSource = new CancellationTokenSource();\r\n            var selectedEntityCa" +
					"llback = GetSelectedEntityCallback();\r\n            System.Threading.Tasks.Task.F" +
					"actory.StartNew(() => {\r\n\t\t\t\t\tvar repository = CreateReadOnlyRepository();\r\n\t\t\t\t" +
					"\tvar entities = new ObservableCollection<TProjection>(repository.GetFilteredEnti" +
					"ties(GetFilterExpression(), Projection));\r\n\t\t\t\t\tOnEntitiesLoaded(GetUnitOfWork(r" +
					"epository), entities);\r\n\t\t\t\t\treturn new Tuple<IReadOnlyRepository<TEntity>, Obse" +
					"rvableCollection<TProjection>>(repository, entities);\r\n\t\t\t\t}).ContinueWith(x => " +
					"{\r\n\t\t\t\t\tif(!x.IsFaulted) {\r\n\t                    ReadOnlyRepository = x.Result.I" +
					"tem1;\r\n\t\t                entities = x.Result.Item2;\r\n\t\t\t            this.RaisePr" +
					"opertyChanged(y => y.Entities);\r\n\t\t\t\t        OnEntitiesAssigned(selectedEntityCa" +
					"llback);\r\n\t\t\t\t\t}\r\n\t\t\t\t\tIsLoading = false;\r\n\t\t\t\t}, cancellationTokenSource.Token," +
					" TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()" +
					");\r\n            return cancellationTokenSource;\r\n        }\r\n\r\n        static TUn" +
					"itOfWork GetUnitOfWork(IReadOnlyRepository<TEntity> repository) {\r\n            r" +
					"eturn (TUnitOfWork)repository.UnitOfWork;\r\n        }\r\n\r\n        protected virtua" +
					"l void OnEntitiesLoaded(TUnitOfWork unitOfWork, IEnumerable<TProjection> entitie" +
					"s) {\r\n        }\r\n\r\n        protected virtual void OnEntitiesAssigned(Func<TProje" +
					"ction> getSelectedEntityCallback) {\r\n        }\r\n\r\n        protected virtual Func" +
					"<TProjection> GetSelectedEntityCallback() {\r\n            return null;\r\n        }" +
					"\r\n\r\n        protected virtual void RestoreSelectedEntity(TProjection existingPro" +
					"jectionEntity, TProjection projectionEntity) {\r\n        }\r\n\r\n        protected v" +
					"irtual Expression<Func<TEntity, bool>> GetFilterExpression() {\r\n            retu" +
					"rn null;\r\n        }\r\n\r\n        protected virtual void OnInitializeInRuntime() {\r" +
					"\n            if(ChangeTracker != null)\r\n                ChangeTracker.RegisterMe" +
					"ssageHandler();\r\n        }\r\n\r\n        protected virtual void OnDestroy() {\r\n    " +
					"        CancelLoading();\r\n            if(ChangeTracker != null)\r\n               " +
					" ChangeTracker.UnregisterMessageHandler();\r\n        }\r\n\r\n        protected virtu" +
					"al void OnIsLoadingChanged() {\r\n        }\r\n\r\n        protected IReadOnlyReposito" +
					"ry<TEntity> CreateReadOnlyRepository() {\r\n            return getRepositoryFunc(C" +
					"reateUnitOfWork());\r\n        }\r\n\r\n        protected TUnitOfWork CreateUnitOfWork" +
					"() {\r\n            return unitOfWorkFactory.CreateUnitOfWork();\r\n        }\r\n\r\n   " +
					"     protected virtual IEntitiesChangeTracker CreateEntitiesChangeTracker() {\r\n " +
					"           return null;\r\n        }\r\n\r\n        protected IDocumentOwner DocumentO" +
					"wner { get; private set; }\r\n\r\n        #region IDocumentContent\r\n        object I" +
					"DocumentContent.Title { get { return null; } }\r\n\r\n        protected virtual void" +
					" OnClose(CancelEventArgs e) { }\r\n        void IDocumentContent.OnClose(CancelEve" +
					"ntArgs e) {\r\n            OnClose(e);\r\n        }\r\n\r\n        void IDocumentContent" +
					".OnDestroy() {\r\n            OnDestroy();\r\n        }\r\n\r\n        IDocumentOwner ID" +
					"ocumentContent.DocumentOwner {\r\n            get { return DocumentOwner; }\r\n     " +
					"       set { DocumentOwner = value; }\r\n        }\r\n        #endregion\r\n\r\n        " +
					"#region IEntitiesViewModel\r\n        ObservableCollection<TProjection> IEntitiesV" +
					"iewModel<TProjection>.Entities { get { return Entities; } }\r\n\r\n        bool IEnt" +
					"itiesViewModel<TProjection>.IsLoading { get { return IsLoading; } }\r\n        #en" +
					"dregion\r\n    }\r\n\r\n    /// <summary>\r\n    /// The base interface for view models " +
					"exposing a collection of entities of the given type.\r\n    /// </summary>\r\n    //" +
					"/ <typeparam name=\"TEntity\">An entity type.</typeparam>\r\n    public interface IE" +
					"ntitiesViewModel<TEntity> : IDocumentContent where TEntity : class {\r\n\r\n        " +
					"/// <summary>\r\n        /// The loaded collection of entities.\r\n        /// </sum" +
					"mary>\r\n        ObservableCollection<TEntity> Entities { get; }\r\n\r\n        /// <s" +
					"ummary>\r\n        /// Used to check whether entities are currently being loaded i" +
					"n the background. The property can be used to show the progress indicator.\r\n    " +
					"    /// </summary>\r\n        bool IsLoading { get; }\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class EntitiesViewModelTemplateBase
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
