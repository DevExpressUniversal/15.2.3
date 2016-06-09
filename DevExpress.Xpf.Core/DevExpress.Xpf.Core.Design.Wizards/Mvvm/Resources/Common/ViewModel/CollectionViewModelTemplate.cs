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
	public partial class CollectionViewModelTemplate : CollectionViewModelTemplateBase
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
");
this.PasteUsingList(this.GetTemplateInfo().UsingList, false);
			this.Write("\r\nnamespace ");
			this.Write(this.ToStringHelper.ToStringWithCulture(this.GetNamespace()));
			this.Write(" {\r\n    /// <summary>\r\n    /// The base class for a POCO view models exposing a c" +
					"olection of entities of a given type and CRUD operations against these entities." +
					"\r\n    /// This is a partial class that provides extension point to add custom pr" +
					"operties, commands and override methods without modifying the auto-generated cod" +
					"e.\r\n    /// </summary>\r\n    /// <typeparam name=\"TEntity\">An entity type.</typep" +
					"aram>\r\n    /// <typeparam name=\"TPrimaryKey\">A primary key value type.</typepara" +
					"m>\r\n    /// <typeparam name=\"TUnitOfWork\">A unit of work type.</typeparam>\r\n    " +
					"public partial class CollectionViewModel<TEntity, TPrimaryKey, TUnitOfWork> : Co" +
					"llectionViewModel<TEntity, TEntity, TPrimaryKey, TUnitOfWork>\r\n        where TEn" +
					"tity : class\r\n        where TUnitOfWork : IUnitOfWork {\r\n\r\n        /// <summary>" +
					"\r\n        /// Creates a new instance of CollectionViewModel as a POCO view model" +
					".\r\n        /// </summary>\r\n        /// <param name=\"unitOfWorkFactory\">A factory" +
					" used to create a unit of work instance.</param>\r\n        /// <param name=\"getRe" +
					"positoryFunc\">A function that returns a repository representing entities of the " +
					"given type.</param>\r\n        /// <param name=\"projection\">An optional parameter " +
					"that provides a LINQ function used to customize a query for entities. The parame" +
					"ter, for example, can be used for sorting data.</param>\r\n        /// <param name" +
					"=\"newEntityInitializer\">An optional parameter that provides a function to initia" +
					"lize a new entity. This parameter is used in the detail collection view models w" +
					"hen creating a single object view model for a new entity.</param>\r\n\t\t/// <param " +
					"name=\"canCreateNewEntity\">A function that is called before an attempt to create " +
					"a new entity is made. This parameter is used together with the newEntityInitiali" +
					"zer parameter.</param>\r\n        /// <param name=\"ignoreSelectEntityMessage\">An o" +
					"ptional parameter that used to specify that the selected entity should not be ma" +
					"naged by PeekCollectionViewModel.</param>\r\n        public static CollectionViewM" +
					"odel<TEntity, TPrimaryKey, TUnitOfWork> CreateCollectionViewModel(\r\n            " +
					"IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,\r\n            Func<TUnitOfWork" +
					", IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,\r\n            Func<IRepos" +
					"itoryQuery<TEntity>, IQueryable<TEntity>> projection = null,\r\n            Action" +
					"<TEntity> newEntityInitializer = null,\r\n\t\t\tFunc<bool> canCreateNewEntity = null," +
					"\r\n\t\t\tbool ignoreSelectEntityMessage = false) {\r\n            return ViewModelSour" +
					"ce.Create(() => new CollectionViewModel<TEntity, TPrimaryKey, TUnitOfWork>(unitO" +
					"fWorkFactory, getRepositoryFunc, projection, newEntityInitializer, canCreateNewE" +
					"ntity, ignoreSelectEntityMessage));\r\n        }\r\n\r\n        /// <summary>\r\n       " +
					" /// Initializes a new instance of the CollectionViewModel class.\r\n        /// T" +
					"his constructor is declared protected to avoid an undesired instantiation of the" +
					" CollectionViewModel type without the POCO proxy factory.\r\n        /// </summary" +
					">\r\n        /// <param name=\"unitOfWorkFactory\">A factory used to create a unit o" +
					"f work instance.</param>\r\n        /// <param name=\"getRepositoryFunc\">A function" +
					" that returns a repository representing entities of the given type.</param>\r\n   " +
					"     /// <param name=\"projection\">An optional parameter that provides a LINQ fun" +
					"ction used to customize a query for entities. The parameter, for example, can be" +
					" used for sorting data.</param>\r\n        /// <param name=\"newEntityInitializer\">" +
					"An optional parameter that provides a function to initialize a new entity. This " +
					"parameter is used in the detail collection view models when creating a single ob" +
					"ject view model for a new entity.</param>\r\n\t\t/// <param name=\"canCreateNewEntity" +
					"\">A function that is called before an attempt to create a new entity is made. Th" +
					"is parameter is used together with the newEntityInitializer parameter.</param>\r\n" +
					"        /// <param name=\"ignoreSelectEntityMessage\">An optional parameter that u" +
					"sed to specify that the selected entity should not be managed by PeekCollectionV" +
					"iewModel.</param>\r\n        protected CollectionViewModel(\r\n            IUnitOfWo" +
					"rkFactory<TUnitOfWork> unitOfWorkFactory,\r\n            Func<TUnitOfWork, IReposi" +
					"tory<TEntity, TPrimaryKey>> getRepositoryFunc,\r\n            Func<IRepositoryQuer" +
					"y<TEntity>, IQueryable<TEntity>> projection = null,\r\n            Action<TEntity>" +
					" newEntityInitializer = null,\r\n\t\t\tFunc<bool> canCreateNewEntity = null,\r\n\t\t\tbool" +
					" ignoreSelectEntityMessage = false\r\n            ) : base(unitOfWorkFactory, getR" +
					"epositoryFunc, projection, newEntityInitializer, canCreateNewEntity, ignoreSelec" +
					"tEntityMessage) {\r\n        }\r\n    }\r\n\r\n    /// <summary>\r\n    /// The base class" +
					" for a POCO view models exposing a collection of entities of a given type and CR" +
					"UD operations against these entities. \r\n    /// This is a partial class that pro" +
					"vides extension point to add custom properties, commands and override methods wi" +
					"thout modifying the auto-generated code.\r\n    /// </summary>\r\n    /// <typeparam" +
					" name=\"TEntity\">A repository entity type.</typeparam>\r\n    /// <typeparam name=\"" +
					"TProjection\">A projection entity type.</typeparam>\r\n    /// <typeparam name=\"TPr" +
					"imaryKey\">A primary key value type.</typeparam>\r\n    /// <typeparam name=\"TUnitO" +
					"fWork\">A unit of work type.</typeparam>\r\n    public partial class CollectionView" +
					"Model<TEntity, TProjection, TPrimaryKey, TUnitOfWork> : CollectionViewModelBase<" +
					"TEntity, TProjection, TPrimaryKey, TUnitOfWork>\r\n        where TEntity : class\r\n" +
					"        where TProjection : class\r\n        where TUnitOfWork : IUnitOfWork {\r\n\r\n" +
					"        /// <summary>\r\n        /// Creates a new instance of CollectionViewModel" +
					" as a POCO view model.\r\n        /// </summary>\r\n        /// <param name=\"unitOfW" +
					"orkFactory\">A factory used to create a unit of work instance.</param>\r\n        /" +
					"// <param name=\"getRepositoryFunc\">A function that returns a repository represen" +
					"ting entities of the given type.</param>\r\n        /// <param name=\"projection\">A" +
					" LINQ function used to customize a query for entities. The parameter, for exampl" +
					"e, can be used for sorting data and/or for projecting data to a custom type that" +
					" does not match the repository entity type.</param>\r\n        /// <param name=\"ne" +
					"wEntityInitializer\">An optional parameter that provides a function to initialize" +
					" a new entity. This parameter is used in the detail collection view models when " +
					"creating a single object view model for a new entity.</param>\r\n\t\t/// <param name" +
					"=\"canCreateNewEntity\">A function that is called before an attempt to create a ne" +
					"w entity is made. This parameter is used together with the newEntityInitializer " +
					"parameter.</param>\r\n        /// <param name=\"ignoreSelectEntityMessage\">An optio" +
					"nal parameter that used to specify that the selected entity should not be manage" +
					"d by PeekCollectionViewModel.</param>\r\n        public static CollectionViewModel" +
					"<TEntity, TProjection, TPrimaryKey, TUnitOfWork> CreateProjectionCollectionViewM" +
					"odel(\r\n            IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,\r\n         " +
					"   Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,\r\n    " +
					"        Func<IRepositoryQuery<TEntity>, IQueryable<TProjection>> projection,\r\n  " +
					"          Action<TEntity> newEntityInitializer = null,\r\n\t\t\tFunc<bool> canCreateN" +
					"ewEntity = null,\r\n            bool ignoreSelectEntityMessage = false) {\r\n       " +
					"     return ViewModelSource.Create(() => new CollectionViewModel<TEntity, TProje" +
					"ction, TPrimaryKey, TUnitOfWork>(unitOfWorkFactory, getRepositoryFunc, projectio" +
					"n, newEntityInitializer, canCreateNewEntity, ignoreSelectEntityMessage));\r\n     " +
					"   }\r\n\r\n        /// <summary>\r\n        /// Initializes a new instance of the Col" +
					"lectionViewModel class.\r\n        /// This constructor is declared protected to a" +
					"void an undesired instantiation of the CollectionViewModel type without the POCO" +
					" proxy factory.\r\n        /// </summary>\r\n        /// <param name=\"unitOfWorkFact" +
					"ory\">A factory used to create a unit of work instance.</param>\r\n        /// <par" +
					"am name=\"getRepositoryFunc\">A function that returns a repository representing en" +
					"tities of the given type.</param>\r\n        /// <param name=\"projection\">A LINQ f" +
					"unction used to customize a query for entities. The parameter, for example, can " +
					"be used for sorting data and/or for projecting data to a custom type that does n" +
					"ot match the repository entity type.</param>\r\n        /// <param name=\"newEntity" +
					"Initializer\">An optional parameter that provides a function to initialize a new " +
					"entity. This parameter is used in the detail collection view models when creatin" +
					"g a single object view model for a new entity.</param>\r\n\t\t/// <param name=\"canCr" +
					"eateNewEntity\">A function that is called before an attempt to create a new entit" +
					"y is made. This parameter is used together with the newEntityInitializer paramet" +
					"er.</param>\r\n        /// <param name=\"ignoreSelectEntityMessage\">An optional par" +
					"ameter that used to specify that the selected entity should not be managed by Pe" +
					"ekCollectionViewModel.</param>\r\n        protected CollectionViewModel(\r\n        " +
					"    IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactory,\r\n            Func<TUnitOf" +
					"Work, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc,\r\n            Func<IR" +
					"epositoryQuery<TEntity>, IQueryable<TProjection>> projection,\r\n            Actio" +
					"n<TEntity> newEntityInitializer = null,\r\n\t\t\tFunc<bool> canCreateNewEntity = null" +
					",\r\n\t\t\tbool ignoreSelectEntityMessage = false\r\n            ) : base(unitOfWorkFac" +
					"tory, getRepositoryFunc, projection, newEntityInitializer, canCreateNewEntity, i" +
					"gnoreSelectEntityMessage) {\r\n        }\r\n    }\r\n\r\n    /// <summary>\r\n    /// The " +
					"base class for POCO view models exposing a collection of entities of a given typ" +
					"e and CRUD operations against these entities.\r\n    /// It is not recommended to " +
					"inherit directly from this class. Use the CollectionViewModel class instead.\r\n  " +
					"  /// </summary>\r\n    /// <typeparam name=\"TEntity\">A repository entity type.</t" +
					"ypeparam>\r\n    /// <typeparam name=\"TProjection\">A projection entity type.</type" +
					"param>\r\n    /// <typeparam name=\"TPrimaryKey\">A primary key value type.</typepar" +
					"am>\r\n    /// <typeparam name=\"TUnitOfWork\">A unit of work type.</typeparam>\r\n   " +
					" public abstract class CollectionViewModelBase<TEntity, TProjection, TPrimaryKey" +
					", TUnitOfWork> : ReadOnlyCollectionViewModel<TEntity, TProjection, TUnitOfWork>," +
					" ISupportLogicalLayout\r\n        where TEntity : class\r\n        where TProjection" +
					" : class\r\n        where TUnitOfWork : IUnitOfWork {\r\n\r\n        EntitiesChangeTra" +
					"cker<TPrimaryKey> ChangeTrackerWithKey { get { return (EntitiesChangeTracker<TPr" +
					"imaryKey>)ChangeTracker; } }\r\n        readonly Action<TEntity> newEntityInitiali" +
					"zer;\r\n\t\treadonly Func<bool> canCreateNewEntity;\r\n        IRepository<TEntity, TP" +
					"rimaryKey> Repository { get { return (IRepository<TEntity, TPrimaryKey>)ReadOnly" +
					"Repository; } }\r\n        \r\n        /// <summary>\r\n        /// Initializes a new " +
					"instance of the CollectionViewModelBase class.\r\n        /// </summary>\r\n        " +
					"/// <param name=\"unitOfWorkFactory\">A factory used to create a unit of work inst" +
					"ance.</param>\r\n        /// <param name=\"getRepositoryFunc\">A function that retur" +
					"ns a repository representing entities of the given type.</param>\r\n        /// <p" +
					"aram name=\"projection\">A LINQ function used to customize a query for entities. T" +
					"he parameter, for example, can be used for sorting data and/or for projecting da" +
					"ta to a custom type that does not match the repository entity type.</param>\r\n   " +
					"     /// <param name=\"newEntityInitializer\">A function to initialize a new entit" +
					"y. This parameter is used in the detail collection view models when creating a s" +
					"ingle object view model for a new entity.</param>\r\n\t\t/// <param name=\"canCreateN" +
					"ewEntity\">A function that is called before an attempt to create a new entity is " +
					"made. This parameter is used together with the newEntityInitializer parameter.</" +
					"param>\r\n        /// <param name=\"ignoreSelectEntityMessage\">A parameter used to " +
					"specify whether the selected entity should be managed by PeekCollectionViewModel" +
					".</param>\r\n        protected CollectionViewModelBase(\r\n            IUnitOfWorkFa" +
					"ctory<TUnitOfWork> unitOfWorkFactory,\r\n            Func<TUnitOfWork, IRepository" +
					"<TEntity, TPrimaryKey>> getRepositoryFunc,\r\n            Func<IRepositoryQuery<TE" +
					"ntity>, IQueryable<TProjection>> projection,\r\n            Action<TEntity> newEnt" +
					"ityInitializer,\r\n\t\t\tFunc<bool> canCreateNewEntity,\r\n\t\t\tbool ignoreSelectEntityMe" +
					"ssage\r\n            ) : base(unitOfWorkFactory, getRepositoryFunc, projection) {\r" +
					"\n            RepositoryExtensions.VerifyProjection(CreateRepository(), projectio" +
					"n);\r\n            this.newEntityInitializer = newEntityInitializer;\r\n\t\t\tthis.canC" +
					"reateNewEntity = canCreateNewEntity;\r\n\t\t\tthis.ignoreSelectEntityMessage = ignore" +
					"SelectEntityMessage;\r\n\t\t\tif(!this.IsInDesignMode())\r\n\t\t\t\tRegisterSelectEntityMes" +
					"sage();\r\n        }\r\n\r\n        /// <summary>\r\n        /// Creates and shows a doc" +
					"ument that contains a single object view model for new entity.\r\n        /// Sinc" +
					"e CollectionViewModelBase is a POCO view model, an the instance of this class wi" +
					"ll also expose the NewCommand property that can be used as a binding source in v" +
					"iews.\r\n        /// </summary>\r\n        public virtual void New() {\r\n            " +
					"if(canCreateNewEntity != null && !canCreateNewEntity())\r\n                return;" +
					"\r\n            DocumentManagerService.ShowNewEntityDocument(this, newEntityInitia" +
					"lizer);\r\n        }\r\n\r\n        /// <summary>\r\n        /// Creates and shows a doc" +
					"ument that contains a single object view model for the existing entity.\r\n       " +
					" /// Since CollectionViewModelBase is a POCO view model, an the instance of this" +
					" class will also expose the EditCommand property that can be used as a binding s" +
					"ource in views.\r\n        /// </summary>\r\n        /// <param name=\"projectionEnti" +
					"ty\">Entity to edit.</param>\r\n        public virtual void Edit(TProjection projec" +
					"tionEntity) {\r\n            if(Repository.IsDetached(projectionEntity))\r\n        " +
					"        return;\r\n            TPrimaryKey primaryKey = Repository.GetProjectionPr" +
					"imaryKey(projectionEntity);\r\n            int index = Entities.IndexOf(projection" +
					"Entity);\r\n            projectionEntity = ChangeTrackerWithKey.FindActualProjecti" +
					"onByKey(primaryKey);\r\n            if(index >= 0) {\r\n                if(projectio" +
					"nEntity == null)\r\n                    Entities.RemoveAt(index);\r\n               " +
					" else\r\n                    Entities[index] = projectionEntity;\r\n            }\r\n " +
					"           if(projectionEntity == null) {\r\n                DestroyDocument(Docum" +
					"entManagerService.FindEntityDocument<TEntity, TPrimaryKey>(primaryKey));\r\n      " +
					"          return;\r\n            }\r\n            DocumentManagerService.ShowExistin" +
					"gEntityDocument<TEntity, TPrimaryKey>(this, primaryKey);\r\n        }\r\n\r\n        /" +
					"// <summary>\r\n        /// Determines whether an entity can be edited.\r\n        /" +
					"// Since CollectionViewModelBase is a POCO view model, this method will be used " +
					"as a CanExecute callback for EditCommand.\r\n        /// </summary>\r\n        /// <" +
					"param name=\"projectionEntity\">An entity to edit.</param>\r\n        public virtual" +
					" bool CanEdit(TProjection projectionEntity) {\r\n            return projectionEnti" +
					"ty != null && !IsLoading;\r\n        }\r\n\r\n        /// <summary>\r\n        /// Delet" +
					"es a given entity from the repository and saves changes if confirmed by the user" +
					".\r\n        /// Since CollectionViewModelBase is a POCO view model, an the instan" +
					"ce of this class will also expose the DeleteCommand property that can be used as" +
					" a binding source in views.\r\n        /// </summary>\r\n        /// <param name=\"pr" +
					"ojectionEntity\">An entity to edit.</param>\r\n        public virtual void Delete(T" +
					"Projection projectionEntity) {\r\n            if(MessageBoxService.ShowMessage(str" +
					"ing.Format(CommonResources.Confirmation_Delete, typeof(TEntity).Name), CommonRes" +
					"ources.Confirmation_Caption, MessageButton.YesNo) != MessageResult.Yes)\r\n       " +
					"         return;\r\n            try {\r\n                Entities.Remove(projectionE" +
					"ntity);\r\n                TPrimaryKey primaryKey = Repository.GetProjectionPrimar" +
					"yKey(projectionEntity);\r\n                TEntity entity = Repository.Find(primar" +
					"yKey);\r\n                if(entity != null) {\r\n\t\t\t\t\tOnBeforeEntityDeleted(primary" +
					"Key, entity);\r\n                    Repository.Remove(entity);\r\n                 " +
					"   Repository.UnitOfWork.SaveChanges();\r\n\t\t\t\t\tOnEntityDeleted(primaryKey, entity" +
					");\r\n                }\r\n            } catch (DbException e) {\r\n                Re" +
					"fresh();\r\n                MessageBoxService.ShowMessage(e.ErrorMessage, e.ErrorC" +
					"aption, MessageButton.OK, MessageIcon.Error);\r\n            }\r\n        }\r\n\r\n     " +
					"   /// <summary>\r\n        /// Determines whether an entity can be deleted.\r\n    " +
					"    /// Since CollectionViewModelBase is a POCO view model, this method will be " +
					"used as a CanExecute callback for DeleteCommand.\r\n        /// </summary>\r\n      " +
					"  /// <param name=\"projectionEntity\">An entity to edit.</param>\r\n        public " +
					"virtual bool CanDelete(TProjection projectionEntity) {\r\n            return proje" +
					"ctionEntity != null && !IsLoading;\r\n        }\r\n\r\n        /// <summary>\r\n        " +
					"/// Saves the given entity.\r\n        /// Since CollectionViewModelBase is a POCO" +
					" view model, the instance of this class will also expose the SaveCommand propert" +
					"y that can be used as a binding source in views.\r\n        /// </summary>\r\n      " +
					"  /// <param name=\"projectionEntity\">An entity to save.</param>\r\n        [Displa" +
					"y(AutoGenerateField = false)]\r\n        public virtual void Save(TProjection proj" +
					"ectionEntity) {\r\n            var entity = Repository.FindExistingOrAddNewEntity(" +
					"projectionEntity, (p, e) => { ApplyProjectionPropertiesToEntity(p, e); });\r\n    " +
					"        try {\r\n                OnBeforeEntitySaved(entity);\r\n                Rep" +
					"ository.UnitOfWork.SaveChanges();\r\n                var primaryKey = Repository.G" +
					"etPrimaryKey(entity);\r\n                Repository.SetProjectionPrimaryKey(projec" +
					"tionEntity, primaryKey);\r\n                OnEntitySaved(primaryKey, entity);\r\n  " +
					"          } catch(DbException e) {\r\n                MessageBoxService.ShowMessag" +
					"e(e.ErrorMessage, e.ErrorCaption, MessageButton.OK, MessageIcon.Error);\r\n       " +
					"     }\r\n        }\r\n\r\n        /// <summary>\r\n        /// Determines whether entit" +
					"y local changes can be saved.\r\n        /// Since CollectionViewModelBase is a PO" +
					"CO view model, this method will be used as a CanExecute callback for SaveCommand" +
					".\r\n        /// </summary>\r\n        /// <param name=\"projectionEntity\">An entity " +
					"to save.</param>\r\n        public virtual bool CanSave(TProjection projectionEnti" +
					"ty) {\r\n            return projectionEntity != null && !IsLoading;\r\n        }\r\n\r\n" +
					"        /// <summary>\r\n        /// Notifies that SelectedEntity has been changed" +
					" by raising the PropertyChanged event.\r\n        /// Since CollectionViewModelBas" +
					"e is a POCO view model, an the instance of this class will also expose the Updat" +
					"eSelectedEntityCommand property that can be used as a binding source in views.\r\n" +
					"        /// </summary>\r\n        [Display(AutoGenerateField = false)]\r\n        pu" +
					"blic virtual void UpdateSelectedEntity() {\r\n            this.RaisePropertyChange" +
					"d(x => x.SelectedEntity);\r\n        }\r\n\r\n        /// <summary>\r\n        /// Close" +
					"s the corresponding view.\r\n        /// Since CollectionViewModelBase is a POCO v" +
					"iew model, an the instance of this class will also expose the CloseCommand prope" +
					"rty that can be used as a binding source in views.\r\n        /// </summary>\r\n    " +
					"    [Display(AutoGenerateField = false)]\r\n        public void Close() {\r\n       " +
					"     if(DocumentOwner != null)\r\n                DocumentOwner.Close(this);\r\n    " +
					"    }\r\n\r\n\t\tprotected override string ViewName { get { return typeof(TEntity).Nam" +
					"e + \"CollectionView\"; } }\r\n\r\n        protected IMessageBoxService MessageBoxServ" +
					"ice { get { return this.GetRequiredService<IMessageBoxService>(); } }\r\n        p" +
					"rotected IDocumentManagerService DocumentManagerService { get { return this.GetS" +
					"ervice<IDocumentManagerService>(); } }\r\n\r\n        protected virtual void OnBefor" +
					"eEntityDeleted(TPrimaryKey primaryKey, TEntity entity) { }\r\n\r\n        protected " +
					"virtual void OnEntityDeleted(TPrimaryKey primaryKey, TEntity entity) {\r\n        " +
					"    Messenger.Default.Send(new EntityMessage<TEntity, TPrimaryKey>(primaryKey, E" +
					"ntityMessageType.Deleted));\r\n        }\r\n\r\n        protected override Func<TProje" +
					"ction> GetSelectedEntityCallback() {\r\n            var entity = SelectedEntity;\r\n" +
					"            return () => FindLocalProjectionWithSameKey(entity);\r\n        }\r\n\r\n " +
					"       TProjection FindLocalProjectionWithSameKey(TProjection projectionEntity) " +
					"{\r\n            bool primaryKeyAvailable = projectionEntity != null && Repository" +
					".ProjectionHasPrimaryKey(projectionEntity);\r\n            return primaryKeyAvaila" +
					"ble ? ChangeTrackerWithKey.FindLocalProjectionByKey(Repository.GetProjectionPrim" +
					"aryKey(projectionEntity)) : null;\r\n        }\r\n\r\n        protected virtual void O" +
					"nBeforeEntitySaved(TEntity entity) { }\r\n\r\n        protected virtual void OnEntit" +
					"ySaved(TPrimaryKey primaryKey, TEntity entity) {\r\n            Messenger.Default." +
					"Send(new EntityMessage<TEntity, TPrimaryKey>(primaryKey, EntityMessageType.Chang" +
					"ed));\r\n        }\r\n\r\n        protected virtual void ApplyProjectionPropertiesToEn" +
					"tity(TProjection projectionEntity, TEntity entity) {\r\n            throw new NotI" +
					"mplementedException(\"Override this method in the collection view model class and" +
					" apply projection properties to the entity so that it can be correctly saved by " +
					"unit of work.\");\r\n        }\r\n\r\n        protected override void OnSelectedEntityC" +
					"hanged() {\r\n            base.OnSelectedEntityChanged();\r\n            UpdateComma" +
					"nds();\r\n        }\r\n\r\n        protected override void RestoreSelectedEntity(TProj" +
					"ection existingProjectionEntity, TProjection newProjectionEntity) {\r\n           " +
					" base.RestoreSelectedEntity(existingProjectionEntity, newProjectionEntity);\r\n   " +
					"         if(ReferenceEquals(SelectedEntity, existingProjectionEntity))\r\n        " +
					"        SelectedEntity = newProjectionEntity;\r\n        }\r\n\r\n        protected ov" +
					"erride void OnIsLoadingChanged() {\r\n            base.OnIsLoadingChanged();\r\n    " +
					"        UpdateCommands();\r\n\t\t\tif(!IsLoading)\r\n\t\t\t\tRequestSelectedEntity();\r\n    " +
					"    }\r\n\r\n        void UpdateCommands() {\r\n            TProjection projectionEnti" +
					"ty = null;\r\n            this.RaiseCanExecuteChanged(x => x.Edit(projectionEntity" +
					"));\r\n            this.RaiseCanExecuteChanged(x => x.Delete(projectionEntity));\r\n" +
					"            this.RaiseCanExecuteChanged(x => x.Save(projectionEntity));\r\n       " +
					" }\r\n\r\n        protected void DestroyDocument(IDocument document) {\r\n            " +
					"if(document != null)\r\n                document.Close();\r\n        }\r\n\r\n        pr" +
					"otected IRepository<TEntity, TPrimaryKey> CreateRepository() {\r\n            retu" +
					"rn (IRepository<TEntity, TPrimaryKey>)CreateReadOnlyRepository();\r\n        }\r\n\r\n" +
					"        protected override IEntitiesChangeTracker CreateEntitiesChangeTracker() " +
					"{\r\n            return new EntitiesChangeTracker<TPrimaryKey>(this);\r\n        }\r\n" +
					"\r\n\t\t#region SelectEntityMessage\r\n        protected class SelectEntityMessage {\r\n" +
					"            public SelectEntityMessage(TPrimaryKey primaryKey) {\r\n              " +
					"  PrimaryKey = primaryKey;\r\n            }\r\n            public TPrimaryKey Primar" +
					"yKey { get; private set; }\r\n        }\r\n\r\n        protected class SelectedEntityR" +
					"equest { }\r\n\r\n        readonly bool ignoreSelectEntityMessage;\r\n\r\n        void R" +
					"egisterSelectEntityMessage() {\r\n            if(!ignoreSelectEntityMessage)\r\n\t\t\t\t" +
					"Messenger.Default.Register<SelectEntityMessage>(this, x => OnSelectEntityMessage" +
					"(x));\r\n        }\r\n\r\n\t\tvoid RequestSelectedEntity() {\r\n\t\t\tif(!ignoreSelectEntityM" +
					"essage)\r\n\t\t\t\tMessenger.Default.Send(new SelectedEntityRequest());\r\n\t\t}\r\n\r\n      " +
					"  void OnSelectEntityMessage(SelectEntityMessage message) {\r\n            if(!IsL" +
					"oaded)\r\n                return;\r\n            var projectionEntity = ChangeTracke" +
					"rWithKey.FindActualProjectionByKey(message.PrimaryKey);\r\n\t\t\tif(projectionEntity " +
					"== null) {\r\n\t\t\t\tFilterExpression = null;\r\n\t\t\t\tprojectionEntity = ChangeTrackerWi" +
					"thKey.FindActualProjectionByKey(message.PrimaryKey);\r\n\t\t\t}\r\n\t\t\tSelectedEntity = " +
					"projectionEntity;\r\n        }\r\n        #endregion\r\n\r\n\t\t#region ISupportLogicalLay" +
					"out\r\n\t\tbool ISupportLogicalLayout.CanSerialize {\r\n            get { return true;" +
					" }\r\n        }\r\n\r\n\t\tIDocumentManagerService ISupportLogicalLayout.DocumentManager" +
					"Service {\r\n            get { return DocumentManagerService; }\r\n        }\r\n\r\n\t\tIE" +
					"numerable<object> ISupportLogicalLayout.LookupViewModels {\r\n            get { re" +
					"turn null; }\r\n\t\t}\r\n        #endregion\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class CollectionViewModelTemplateBase
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
