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
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework;
	using DevExpress.Xpf.Core.Design.Wizards.Mvvm;
	using DevExpress.Design.Mvvm.EntityFramework;
	using DevExpress.Design.Mvvm;
	using DevExpress.Entity.Model;
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public partial class SingleObjectViewModelTemplate : SingleObjectViewModelTemplateBase
	{
		public virtual string TransformText()
		{
	T4TemplateInfo templateInfo = this.GetTemplateInfo();
	var context = templateInfo.Properties[TemplatesConstants.STR_TemplateGenerationContext] as TemplateGenerationContext;
	var generateInstantFeedbackMode = context == null || context.PlatformType != PlatformType.WinForms;
			this.Write(@"
using System;
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
			this.Write(" {\r\n    /// <summary>\r\n    /// The base class for POCO view models exposing a sin" +
					"gle entity of a given type and CRUD operations against this entity.\r\n    /// Thi" +
					"s is a partial class that provides the extension point to add custom properties," +
					" commands and override methods without modifying the auto-generated code.\r\n    /" +
					"// </summary>\r\n    /// <typeparam name=\"TEntity\">An entity type.</typeparam>\r\n  " +
					"  /// <typeparam name=\"TPrimaryKey\">A primary key value type.</typeparam>\r\n    /" +
					"// <typeparam name=\"TUnitOfWork\">A unit of work type.</typeparam>\r\n    public ab" +
					"stract partial class SingleObjectViewModel<TEntity, TPrimaryKey, TUnitOfWork> : " +
					"SingleObjectViewModelBase<TEntity, TPrimaryKey, TUnitOfWork>\r\n        where TEnt" +
					"ity : class\r\n        where TUnitOfWork : IUnitOfWork {\r\n\r\n        /// <summary>\r" +
					"\n        /// Initializes a new instance of the SingleObjectViewModel class.\r\n   " +
					"     /// </summary>\r\n        /// <param name=\"unitOfWorkFactory\">A factory used " +
					"to create the unit of work instance.</param>\r\n        /// <param name=\"getReposi" +
					"toryFunc\">A function that returns the repository representing entities of a give" +
					"n type.</param>\r\n        /// <param name=\"getEntityDisplayNameFunc\">An optional " +
					"parameter that provides a function to obtain the display text for a given entity" +
					". If ommited, the primary key value is used as a display text.</param>\r\n        " +
					"protected SingleObjectViewModel(IUnitOfWorkFactory<TUnitOfWork> unitOfWorkFactor" +
					"y, Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRepositoryFunc, Func<" +
					"TEntity, object> getEntityDisplayNameFunc = null)\r\n            : base(unitOfWork" +
					"Factory, getRepositoryFunc, getEntityDisplayNameFunc) {\r\n        }\r\n    }\r\n\r\n   " +
					" public class SingleObjectViewModelState {\r\n        public object[] Key { get; s" +
					"et; }\r\n        public string Title { get; set; }\r\n    }\r\n\r\n    /// <summary>\r\n  " +
					"  /// The base class for POCO view models exposing a single entity of a given ty" +
					"pe and CRUD operations against this entity.\r\n    /// It is not recommended to in" +
					"herit directly from this class. Use the SingleObjectViewModel class instead.\r\n  " +
					"  /// </summary>\r\n    /// <typeparam name=\"TEntity\">An entity type.</typeparam>\r" +
					"\n    /// <typeparam name=\"TPrimaryKey\">A primary key value type.</typeparam>\r\n  " +
					"  /// <typeparam name=\"TUnitOfWork\">A unit of work type.</typeparam>\r\n    [POCOV" +
					"iewModel]\r\n    public abstract class SingleObjectViewModelBase<TEntity, TPrimary" +
					"Key, TUnitOfWork> : ISingleObjectViewModel<TEntity, TPrimaryKey>, ISupportParame" +
					"ter, IDocumentContent, ISupportLogicalLayout<SingleObjectViewModelState>\r\n      " +
					"  where TEntity : class\r\n        where TUnitOfWork : IUnitOfWork {\r\n\r\n        ob" +
					"ject title;\r\n        protected readonly Func<TUnitOfWork, IRepository<TEntity, T" +
					"PrimaryKey>> getRepositoryFunc;\r\n        protected readonly Func<TEntity, object" +
					"> getEntityDisplayNameFunc;\r\n        Action<TEntity> entityInitializer;\r\n       " +
					" bool isEntityNewAndUnmodified;\r\n        readonly Dictionary<string, IDocumentCo" +
					"ntent> lookUpViewModels = new Dictionary<string, IDocumentContent>();\r\n\r\n       " +
					" /// <summary>\r\n        /// Initializes a new instance of the SingleObjectViewMo" +
					"delBase class.\r\n        /// </summary>\r\n        /// <param name=\"unitOfWorkFacto" +
					"ry\">A factory used to create the unit of work instance.</param>\r\n        /// <pa" +
					"ram name=\"getRepositoryFunc\">A function that returns repository representing ent" +
					"ities of a given type.</param>\r\n        /// <param name=\"getEntityDisplayNameFun" +
					"c\">An optional parameter that provides a function to obtain the display text for" +
					" a given entity. If ommited, the primary key value is used as a display text.</p" +
					"aram>\r\n        protected SingleObjectViewModelBase(IUnitOfWorkFactory<TUnitOfWor" +
					"k> unitOfWorkFactory, Func<TUnitOfWork, IRepository<TEntity, TPrimaryKey>> getRe" +
					"positoryFunc, Func<TEntity, object> getEntityDisplayNameFunc) {\r\n            Uni" +
					"tOfWorkFactory = unitOfWorkFactory;\r\n            this.getRepositoryFunc = getRep" +
					"ositoryFunc;\r\n            this.getEntityDisplayNameFunc = getEntityDisplayNameFu" +
					"nc;\r\n            UpdateUnitOfWork();\r\n            if(this.IsInDesignMode())\r\n   " +
					"             this.Entity = this.Repository.FirstOrDefault();\r\n            else\r\n" +
					"                OnInitializeInRuntime();\r\n        }\r\n\r\n        /// <summary>\r\n  " +
					"      /// The display text for a given entity used as a title in the correspondi" +
					"ng view.\r\n        /// </summary>\r\n        /// <returns></returns>\r\n        publi" +
					"c object Title { get { return title; } }\r\n\r\n        /// <summary>\r\n        /// A" +
					"n entity represented by this view model.\r\n        /// Since SingleObjectViewMode" +
					"lBase is a POCO view model, this property will raise INotifyPropertyChanged.Prop" +
					"ertyEvent when modified so it can be used as a binding source in views.\r\n       " +
					" /// </summary>\r\n        /// <returns></returns>\r\n        public virtual TEntity" +
					" Entity { get; protected set; }\r\n\r\n        /// <summary>\r\n        /// Updates th" +
					"e Title property value and raises CanExecute changed for relevant commands.\r\n   " +
					"     /// Since SingleObjectViewModelBase is a POCO view model, an instance of th" +
					"is class will also expose the UpdateCommand property that can be used as a bindi" +
					"ng source in views.\r\n        /// </summary>\r\n        [Display(AutoGenerateField " +
					"= false)]\r\n        public void Update() {\r\n            isEntityNewAndUnmodified " +
					"= false;\r\n            UpdateTitle();\r\n            UpdateCommands();\r\n        }\r\n" +
					"\r\n        /// <summary>\r\n        /// Saves changes in the underlying unit of wor" +
					"k.\r\n        /// Since SingleObjectViewModelBase is a POCO view model, an instanc" +
					"e of this class will also expose the SaveCommand property that can be used as a " +
					"binding source in views.\r\n        /// </summary>\r\n        public virtual void Sa" +
					"ve() {\r\n            SaveCore();\r\n        }\r\n\r\n        /// <summary>\r\n        ///" +
					" Determines whether entity has local changes that can be saved.\r\n        /// Sin" +
					"ce SingleObjectViewModelBase is a POCO view model, this method will be used as a" +
					" CanExecute callback for SaveCommand.\r\n        /// </summary>\r\n        public vi" +
					"rtual bool CanSave() {\r\n            return Entity != null && !HasValidationError" +
					"s() && NeedSave();\r\n        }\r\n\r\n        /// <summary>\r\n        /// Saves change" +
					"s in the underlying unit of work and closes the corresponding view.\r\n        ///" +
					" Since SingleObjectViewModelBase is a POCO view model, an instance of this class" +
					" will also expose the SaveAndCloseCommand property that can be used as a binding" +
					" source in views.\r\n        /// </summary>\r\n        [Command(CanExecuteMethodName" +
					" = \"CanSave\")]\r\n        public void SaveAndClose() {\r\n            if(SaveCore())" +
					"\r\n                Close();\r\n        }\r\n\r\n        /// <summary>\r\n        /// Save" +
					"s changes in the underlying unit of work and create new entity.\r\n        /// Sin" +
					"ce SingleObjectViewModelBase is a POCO view model, an instance of this class wil" +
					"l also expose the SaveAndNewCommand property that can be used as a binding sourc" +
					"e in views.\r\n        /// </summary>\r\n        [Command(CanExecuteMethodName = \"Ca" +
					"nSave\")]\r\n        public void SaveAndNew() {\r\n            if(SaveCore())\r\n      " +
					"          CreateAndInitializeEntity(this.entityInitializer);\r\n        }\r\n\r\n     " +
					"   /// <summary>\r\n        /// Reset entity local changes.\r\n        /// Since Sin" +
					"gleObjectViewModelBase is a POCO view model, an instance of this class will also" +
					" expose the ResetCommand property that can be used as a binding source in views." +
					"\r\n        /// </summary>\r\n\t\t[Display(Name = \"Reset Changes\")]\r\n        public vo" +
					"id Reset() {\r\n            MessageResult confirmationResult = MessageBoxService.S" +
					"howMessage(CommonResources.Confirmation_Reset, CommonResources.Confirmation_Capt" +
					"ion, MessageButton.OKCancel);\r\n            if(confirmationResult == MessageResul" +
					"t.OK)\r\n                Reload();\r\n        }\r\n\r\n        /// <summary>\r\n        //" +
					"/ Determines whether entity has local changes.\r\n        /// Since SingleObjectVi" +
					"ewModelBase is a POCO view model, this method will be used as a CanExecute callb" +
					"ack for ResetCommand.\r\n        /// </summary>\r\n        public bool CanReset() {\r" +
					"\n            return NeedReset();\r\n        }\r\n\r\n\t\tstring ViewName { get { return " +
					"typeof(TEntity).Name + \"View\"; } }\r\n\r\n\t\t[DXImage(\"Save\")]\r\n        [Display(Name" +
					" = \"Save Layout\")]\r\n        public void SaveLayout() {\r\n            PersistentLa" +
					"youtHelper.TrySerializeLayout(LayoutSerializationService, ViewName);\r\n\t\t\tPersist" +
					"entLayoutHelper.SaveLayout();\r\n        }\r\n\t\t\r\n\t\t[DXImage(\"Reset\")]\r\n        [Dis" +
					"play(Name = \"Reset Layout\")]\r\n        public void ResetLayout() {\r\n\t\t\tif (Layout" +
					"SerializationService != null)\r\n\t\t\t\tLayoutSerializationService.Deserialize(null);" +
					"\r\n        }\r\n\r\n\t\t[Display(AutoGenerateField = false)]\r\n        public void OnLoa" +
					"ded() {\r\n            PersistentLayoutHelper.TryDeserializeLayout(LayoutSerializa" +
					"tionService, ViewName);\r\n        }\r\n\r\n        /// <summary>\r\n        /// Deletes" +
					" the entity, save changes and closes the corresponding view if confirmed by a us" +
					"er.\r\n        /// Since SingleObjectViewModelBase is a POCO view model, an instan" +
					"ce of this class will also expose the DeleteCommand property that can be used as" +
					" a binding source in views.\r\n        /// </summary>\r\n        public virtual void" +
					" Delete() {\r\n            if(MessageBoxService.ShowMessage(string.Format(CommonRe" +
					"sources.Confirmation_Delete, typeof(TEntity).Name), GetConfirmationMessageTitle(" +
					"), MessageButton.YesNo) != MessageResult.Yes)\r\n                return;\r\n        " +
					"    try {\r\n                OnBeforeEntityDeleted(PrimaryKey, Entity);\r\n         " +
					"       Repository.Remove(Entity);\r\n                UnitOfWork.SaveChanges();\r\n  " +
					"              TPrimaryKey primaryKeyForMessage = PrimaryKey;\r\n                TE" +
					"ntity entityForMessage = Entity;\r\n                Entity = null;\r\n              " +
					"  OnEntityDeleted(primaryKeyForMessage, entityForMessage);\r\n                Clos" +
					"e();\r\n            } catch (DbException e) {\r\n                MessageBoxService.S" +
					"howMessage(e.ErrorMessage, e.ErrorCaption, MessageButton.OK, MessageIcon.Error);" +
					"\r\n            }\r\n        }\r\n\r\n        /// <summary>\r\n        /// Determines whet" +
					"her the entity can be deleted.\r\n        /// Since SingleObjectViewModelBase is a" +
					" POCO view model, this method will be used as a CanExecute callback for DeleteCo" +
					"mmand.\r\n        /// </summary>\r\n        public virtual bool CanDelete() {\r\n     " +
					"       return Entity != null && !IsNew();\r\n        }\r\n\r\n        /// <summary>\r\n " +
					"       /// Closes the corresponding view.\r\n        /// Since SingleObjectViewMod" +
					"elBase is a POCO view model, an instance of this class will also expose the Clos" +
					"eCommand property that can be used as a binding source in views.\r\n        /// </" +
					"summary>\r\n        public void Close() {\r\n            if(!TryClose())\r\n          " +
					"      return;\r\n            if(DocumentOwner != null)\r\n                DocumentOw" +
					"ner.Close(this);\r\n        }\r\n\r\n        protected IUnitOfWorkFactory<TUnitOfWork>" +
					" UnitOfWorkFactory { get; private set; }\r\n\r\n        protected TUnitOfWork UnitOf" +
					"Work { get; private set; }\r\n\r\n        protected virtual bool SaveCore() {\r\n     " +
					"       try {\r\n                bool isNewEntity = IsNew();\r\n                if(!i" +
					"sNewEntity) {\r\n                    Repository.SetPrimaryKey(Entity, PrimaryKey);" +
					"\r\n                    Repository.Update(Entity);\r\n                }\r\n           " +
					"     OnBeforeEntitySaved(PrimaryKey, Entity, isNewEntity);\r\n                Unit" +
					"OfWork.SaveChanges();\r\n                LoadEntityByKey(Repository.GetPrimaryKey(" +
					"Entity));\r\n                OnEntitySaved(PrimaryKey, Entity, isNewEntity);\r\n\t\t\t\t" +
					"this.RaisePropertyChanged(x => x.IsPrimaryKeyReadOnly);\r\n                return " +
					"true;\r\n            } catch (DbException e) {\r\n                MessageBoxService." +
					"ShowMessage(e.ErrorMessage, e.ErrorCaption, MessageButton.OK, MessageIcon.Error)" +
					";\r\n                return false;\r\n            }\r\n        }\r\n\r\n        protected " +
					"virtual void OnBeforeEntitySaved(TPrimaryKey primaryKey, TEntity entity, bool is" +
					"NewEntity) { }\r\n\r\n        protected virtual void OnEntitySaved(TPrimaryKey prima" +
					"ryKey, TEntity entity, bool isNewEntity) {\r\n            Messenger.Default.Send(n" +
					"ew EntityMessage<TEntity, TPrimaryKey>(primaryKey, isNewEntity ? EntityMessageTy" +
					"pe.Added : EntityMessageType.Changed, this));\r\n        }\r\n\r\n        protected vi" +
					"rtual void OnBeforeEntityDeleted(TPrimaryKey primaryKey, TEntity entity) { }\r\n\r\n" +
					"        protected virtual void OnEntityDeleted(TPrimaryKey primaryKey, TEntity e" +
					"ntity) {\r\n            Messenger.Default.Send(new EntityMessage<TEntity, TPrimary" +
					"Key>(primaryKey, EntityMessageType.Deleted));\r\n        }\r\n\r\n        protected vi" +
					"rtual void OnInitializeInRuntime() {\r\n            Messenger.Default.Register<Ent" +
					"ityMessage<TEntity, TPrimaryKey>>(this, x => OnEntityMessage(x));\r\n            M" +
					"essenger.Default.Register<SaveAllMessage>(this, x => Save());\r\n            Messe" +
					"nger.Default.Register<CloseAllMessage>(this, m => {\r\n                if(m.Should" +
					"Process(this)) {\r\n                    OnClosing(m);\r\n                }\r\n        " +
					"    });\r\n        }\r\n\r\n        protected virtual void OnEntityMessage(EntityMessa" +
					"ge<TEntity, TPrimaryKey> message) {\r\n            if(Entity == null) return;\r\n   " +
					"         if(message.MessageType == EntityMessageType.Deleted && object.Equals(me" +
					"ssage.PrimaryKey, PrimaryKey))\r\n                Close();\r\n        }\r\n\r\n        p" +
					"rotected virtual void OnEntityChanged() {\r\n            if(Entity != null && Repo" +
					"sitory.HasPrimaryKey(Entity)) {\r\n                PrimaryKey = Repository.GetPrim" +
					"aryKey(Entity);\r\n                RefreshLookUpCollections(true);\r\n            }\r" +
					"\n            Update();\r\n        }\r\n\r\n        protected IRepository<TEntity, TPri" +
					"maryKey> Repository { get { return getRepositoryFunc(UnitOfWork); } }\r\n\r\n       " +
					" protected TPrimaryKey PrimaryKey { get; private set; }\r\n\r\n        protected IMe" +
					"ssageBoxService MessageBoxService { get { return this.GetRequiredService<IMessag" +
					"eBoxService>(); } }\r\n\t\tprotected ILayoutSerializationService LayoutSerialization" +
					"Service { get { return this.GetService<ILayoutSerializationService>(); } }\r\n\r\n\r\n" +
					"        protected virtual void OnParameterChanged(object parameter) {\r\n         " +
					"   var initializer = parameter as Action<TEntity>;\r\n            if(initializer !" +
					"= null)\r\n                CreateAndInitializeEntity(initializer);\r\n            el" +
					"se if(parameter is TPrimaryKey)\r\n                LoadEntityByKey((TPrimaryKey)pa" +
					"rameter);\r\n            else\r\n                Entity = null;\r\n        }\r\n\r\n      " +
					"  protected virtual TEntity CreateEntity() {\r\n            return Repository.Crea" +
					"te();\r\n        }\r\n\r\n        protected void Reload() {\r\n            if(Entity == " +
					"null || IsNew())\r\n                CreateAndInitializeEntity(this.entityInitializ" +
					"er);\r\n            else\r\n                LoadEntityByKey(PrimaryKey);\r\n        }\r" +
					"\n\r\n        protected void CreateAndInitializeEntity(Action<TEntity> entityInitia" +
					"lizer) {\r\n            UpdateUnitOfWork();\r\n            this.entityInitializer = " +
					"entityInitializer;\r\n            var entity = CreateEntity();\r\n            if(thi" +
					"s.entityInitializer != null)\r\n                this.entityInitializer(entity);\r\n " +
					"           Entity = entity;\r\n            isEntityNewAndUnmodified = true;\r\n     " +
					"   }\r\n\r\n        protected void LoadEntityByKey(TPrimaryKey primaryKey) {\r\n      " +
					"      UpdateUnitOfWork();\r\n            Entity = Repository.Find(primaryKey);\r\n  " +
					"      }\r\n\r\n        void UpdateUnitOfWork() {\r\n            UnitOfWork = UnitOfWor" +
					"kFactory.CreateUnitOfWork();\r\n        }\r\n\r\n        void UpdateTitle(string nullE" +
					"ntityValue = null) {\r\n            if(Entity == null)\r\n                title = nu" +
					"llEntityValue;\r\n            else if(IsNew())\r\n                title = GetTitleFo" +
					"rNewEntity();\r\n            else\r\n                title = GetTitle(GetState() == " +
					"EntityState.Modified);\r\n            this.RaisePropertyChanged(x => x.Title);\r\n  " +
					"      }\r\n\r\n        protected virtual void UpdateCommands() {\r\n            this.R" +
					"aiseCanExecuteChanged(x => x.Save());\r\n            this.RaiseCanExecuteChanged(x" +
					" => x.SaveAndClose());\r\n            this.RaiseCanExecuteChanged(x => x.SaveAndNe" +
					"w());\r\n            this.RaiseCanExecuteChanged(x => x.Delete());\r\n            th" +
					"is.RaiseCanExecuteChanged(x => x.Reset());\r\n        }\r\n\r\n        protected IDocu" +
					"mentOwner DocumentOwner { get; private set; }\r\n\r\n        protected virtual void " +
					"OnDestroy() {\r\n            Messenger.Default.Unregister(this);\r\n            Refr" +
					"eshLookUpCollections(false);\r\n        }\r\n\r\n        protected virtual bool TryClo" +
					"se() {\r\n            if(HasValidationErrors()) {\r\n                MessageResult w" +
					"arningResult = MessageBoxService.ShowMessage(CommonResources.Warning_SomeFieldsC" +
					"ontainInvalidData, CommonResources.Warning_Caption, MessageButton.OKCancel);\r\n  " +
					"              return warningResult == MessageResult.OK;\r\n            }\r\n        " +
					"    if(!NeedReset()) return true;\r\n            MessageResult result = MessageBox" +
					"Service.ShowMessage(CommonResources.Confirmation_Save, GetConfirmationMessageTit" +
					"le(), MessageButton.YesNoCancel);\r\n            if(result == MessageResult.Yes)\r\n" +
					"                return SaveCore();\r\n\t\t\tif(result == MessageResult.No)\r\n         " +
					"       Reload();\r\n            return result != MessageResult.Cancel;\r\n        }\r" +
					"\n\r\n        protected virtual void OnClosing(CloseAllMessage message) {\r\n        " +
					"    if(!message.Cancel)\r\n                message.Cancel = !TryClose();\r\n        " +
					"}\r\n\r\n        protected virtual string GetConfirmationMessageTitle() {\r\n         " +
					"   return GetTitle();\r\n        }\r\n\r\n        public bool IsNew() {\r\n            r" +
					"eturn GetState() == EntityState.Added;\r\n        }\r\n\r\n\t\tpublic bool IsPrimaryKeyR" +
					"eadOnly {\r\n            get { return !IsNew(); }\r\n        }\r\n\r\n        protected " +
					"virtual bool NeedSave() {\r\n            if(Entity == null)\r\n                retur" +
					"n false;\r\n            EntityState state = GetState();\r\n            return state " +
					"== EntityState.Modified || state == EntityState.Added;\r\n        }\r\n\r\n        pro" +
					"tected virtual bool NeedReset() {\r\n            return NeedSave() && !isEntityNew" +
					"AndUnmodified;\r\n        }\r\n\r\n        protected virtual bool HasValidationErrors(" +
					") {\r\n            IDataErrorInfo dataErrorInfo = Entity as IDataErrorInfo;\r\n     " +
					"       return dataErrorInfo != null && IDataErrorInfoHelper.HasErrors(dataErrorI" +
					"nfo);\r\n        }\r\n\r\n        string GetTitle(bool entityModified) {\r\n            " +
					"return GetTitle() + (entityModified ? CommonResources.Entity_Changed : string.Em" +
					"pty);\r\n        }\r\n\r\n        protected virtual string GetTitleForNewEntity() {\r\n " +
					"           return typeof(TEntity).Name + CommonResources.Entity_New;\r\n        }\r" +
					"\n\r\n        protected virtual string GetTitle() {\r\n            return (typeof(TEn" +
					"tity).Name + \" - \" + Convert.ToString(getEntityDisplayNameFunc != null ? getEnti" +
					"tyDisplayNameFunc(Entity) : PrimaryKey))\r\n            .Split(new string[] { \"\\r\"" +
					", \"\\n\" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();\r\n        }\r\n\r" +
					"\n        protected EntityState GetState() {\r\n            try {\r\n                " +
					"return Repository.GetState(Entity);\r\n            } catch (InvalidOperationExcept" +
					"ion) {\r\n                Repository.SetPrimaryKey(Entity, PrimaryKey);\r\n         " +
					"       return Repository.GetState(Entity);\r\n            }\r\n\r\n        }\r\n\r\n      " +
					"  #region look up and detail view models\r\n        protected virtual void Refresh" +
					"LookUpCollections(bool raisePropertyChanged) {\r\n            var values = lookUpV" +
					"iewModels.ToArray();\r\n            lookUpViewModels.Clear();\r\n            foreach" +
					"(var item in values) {\r\n                item.Value.OnDestroy();\r\n               " +
					" if(raisePropertyChanged)\r\n                    ((IPOCOViewModel)this).RaisePrope" +
					"rtyChanged(item.Key);\r\n            }\r\n\t\t\tOnLookupCollectionsUpdated();\r\n        " +
					"}\r\n\r\n\t\tprotected virtual void OnLookupCollectionsUpdated() { }\r\n\t\t\r\n\t\tprotected " +
					"AddRemoveDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, TDetailPri" +
					"maryKey, TUnitOfWork>\r\n            CreateAddRemoveDetailEntitiesViewModel<TDetai" +
					"lEntity, TDetailPrimaryKey>(Func<TUnitOfWork, IRepository<TDetailEntity, TDetail" +
					"PrimaryKey>> getDetailsRepositoryFunc, Func<TEntity, ICollection<TDetailEntity>>" +
					" getDetailsFunc)\r\n            where TDetailEntity : class {\r\n            var vie" +
					"wModel = AddRemoveDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEntity, T" +
					"DetailPrimaryKey, TUnitOfWork>.Create(UnitOfWorkFactory, this.getRepositoryFunc," +
					" getDetailsRepositoryFunc, getDetailsFunc, PrimaryKey);\r\n            viewModel.S" +
					"etParentViewModel(this);\r\n            return viewModel;\r\n        }\r\n\r\n        pr" +
					"otected AddRemoveJunctionDetailEntitiesViewModel<TEntity, TPrimaryKey, TDetailEn" +
					"tity, TDetailPrimaryKey, TJunction, TJunctionPrimaryKey, TUnitOfWork>\r\n         " +
					"   CreateAddRemoveJunctionDetailEntitiesViewModel<TDetailEntity, TDetailPrimaryK" +
					"ey, TJunction, TJunctionPrimaryKey>(\r\n                Func<TUnitOfWork, IReposit" +
					"ory<TDetailEntity, TDetailPrimaryKey>> getDetailsRepositoryFunc,\r\n              " +
					"  Func<TUnitOfWork, IRepository<TJunction, TJunctionPrimaryKey>> getJunctionRepo" +
					"sitoryFunc,\r\n                Expression<Func<TJunction, TPrimaryKey>> getEntityF" +
					"oreignKey,\r\n                Expression<Func<TJunction, TDetailPrimaryKey>> getDe" +
					"tailForeignKey)\r\n            where TDetailEntity : class\r\n            where TJun" +
					"ction : class, new()\r\n            where TJunctionPrimaryKey : class {\r\n         " +
					"   var viewModel = AddRemoveJunctionDetailEntitiesViewModel<TEntity, TPrimaryKey" +
					", TDetailEntity, TDetailPrimaryKey, TJunction, TJunctionPrimaryKey, TUnitOfWork>" +
					"\r\n                .CreateViewModel(UnitOfWorkFactory, this.getRepositoryFunc, ge" +
					"tDetailsRepositoryFunc, getJunctionRepositoryFunc, getEntityForeignKey, getDetai" +
					"lForeignKey, PrimaryKey);\r\n            viewModel.SetParentViewModel(this);\r\n    " +
					"        return viewModel;\r\n        }\r\n\r\n        protected CollectionViewModel<TD" +
					"etailEntity, TDetailPrimaryKey, TUnitOfWork> GetDetailsCollectionViewModel<TView" +
					"Model, TDetailEntity, TDetailPrimaryKey, TForeignKey>(\r\n            Expression<F" +
					"unc<TViewModel, CollectionViewModel<TDetailEntity, TDetailPrimaryKey, TUnitOfWor" +
					"k>>> propertyExpression,\r\n            Func<TUnitOfWork, IRepository<TDetailEntit" +
					"y, TDetailPrimaryKey>> getRepositoryFunc,\r\n            Expression<Func<TDetailEn" +
					"tity, TForeignKey>> foreignKeyExpression,\r\n            Action<TDetailEntity, TPr" +
					"imaryKey> setMasterEntityKeyAction,\r\n            Func<IRepositoryQuery<TDetailEn" +
					"tity>, IQueryable<TDetailEntity>> projection = null) where TDetailEntity : class" +
					" {\r\n            return GetCollectionViewModelCore<CollectionViewModel<TDetailEnt" +
					"ity, TDetailPrimaryKey, TUnitOfWork>, TDetailEntity, TDetailEntity, TForeignKey>" +
					"(propertyExpression,\r\n\t\t\t\t() => CollectionViewModel<TDetailEntity, TDetailPrimar" +
					"yKey, TUnitOfWork>.CreateCollectionViewModel(UnitOfWorkFactory, getRepositoryFun" +
					"c, AppendForeignKeyPredicate<TDetailEntity, TDetailEntity, TForeignKey>(foreignK" +
					"eyExpression, projection), CreateForeignKeyPropertyInitializer(setMasterEntityKe" +
					"yAction, () => PrimaryKey), () => CanCreateNewEntity(), true));\r\n        }\r\n\r\n  " +
					"      protected CollectionViewModel<TDetailEntity, TDetailProjection, TDetailPri" +
					"maryKey, TUnitOfWork> GetDetailProjectionsCollectionViewModel<TViewModel, TDetai" +
					"lEntity, TDetailProjection, TDetailPrimaryKey, TForeignKey>(\r\n            Expres" +
					"sion<Func<TViewModel, CollectionViewModel<TDetailEntity, TDetailProjection, TDet" +
					"ailPrimaryKey, TUnitOfWork>>> propertyExpression,\r\n            Func<TUnitOfWork," +
					" IRepository<TDetailEntity, TDetailPrimaryKey>> getRepositoryFunc,\r\n            " +
					"Expression<Func<TDetailEntity, TForeignKey>> foreignKeyExpression,\r\n            " +
					"Action<TDetailEntity, TPrimaryKey> setMasterEntityKeyAction,\r\n            Func<I" +
					"RepositoryQuery<TDetailEntity>, IQueryable<TDetailProjection>> projection = null" +
					") where TDetailEntity : class where TDetailProjection : class {\r\n            ret" +
					"urn GetCollectionViewModelCore<CollectionViewModel<TDetailEntity, TDetailProject" +
					"ion, TDetailPrimaryKey, TUnitOfWork>, TDetailEntity, TDetailProjection, TForeign" +
					"Key>(propertyExpression,\r\n\t\t\t\t() => CollectionViewModel<TDetailEntity, TDetailPr" +
					"ojection, TDetailPrimaryKey, TUnitOfWork>.CreateProjectionCollectionViewModel(Un" +
					"itOfWorkFactory, getRepositoryFunc, AppendForeignKeyPredicate<TDetailEntity, TDe" +
					"tailProjection, TForeignKey>(foreignKeyExpression, projection), CreateForeignKey" +
					"PropertyInitializer(setMasterEntityKeyAction, () => PrimaryKey), () => CanCreate" +
					"NewEntity(), true));\r\n        }\r\n\r\n");
if(generateInstantFeedbackMode){
			this.Write("\t\tprotected InstantFeedbackCollectionViewModel<TDetailEntity, TDetailPrimaryKey, " +
					"TUnitOfWork> GetDetailsInstantFeedbackCollectionViewModel<TViewModel, TDetailEnt" +
					"ity, TDetailPrimaryKey, TForeignKey>(\r\n            Expression<Func<TViewModel, I" +
					"nstantFeedbackCollectionViewModel<TDetailEntity, TDetailPrimaryKey, TUnitOfWork>" +
					">> propertyExpression,\r\n            Func<TUnitOfWork, IRepository<TDetailEntity," +
					" TDetailPrimaryKey>> getRepositoryFunc,\r\n            Expression<Func<TDetailEnti" +
					"ty, TForeignKey>> foreignKeyExpression,\r\n            Action<TDetailEntity, TPrim" +
					"aryKey> setMasterEntityKeyAction,\r\n            Func<IRepositoryQuery<TDetailEnti" +
					"ty>, IQueryable<TDetailEntity>> projection = null)\r\n            where TDetailEnt" +
					"ity : class, new() {\r\n            return GetCollectionViewModelCore<InstantFeedb" +
					"ackCollectionViewModel<TDetailEntity, TDetailPrimaryKey, TUnitOfWork>, TDetailEn" +
					"tity, TDetailEntity, TForeignKey>(propertyExpression,\r\n\t\t\t\t() => InstantFeedback" +
					"CollectionViewModel<TDetailEntity, TDetailPrimaryKey, TUnitOfWork>.CreateInstant" +
					"FeedbackCollectionViewModel(UnitOfWorkFactory, getRepositoryFunc, AppendForeignK" +
					"eyPredicate<TDetailEntity, TDetailEntity, TForeignKey>(foreignKeyExpression, pro" +
					"jection), () => CanCreateNewEntity()));\r\n        }\r\n\r\n        protected InstantF" +
					"eedbackCollectionViewModel<TDetailEntity, TDetailEntityProjection, TDetailPrimar" +
					"yKey, TUnitOfWork> GetDetailsInstantFeedbackCollectionViewModel<TViewModel, TDet" +
					"ailEntity, TDetailEntityProjection, TDetailPrimaryKey, TForeignKey>(\r\n          " +
					"  Expression<Func<TViewModel, InstantFeedbackCollectionViewModel<TDetailEntity, " +
					"TDetailEntityProjection, TDetailPrimaryKey, TUnitOfWork>>> propertyExpression,\r\n" +
					"            Func<TUnitOfWork, IRepository<TDetailEntity, TDetailPrimaryKey>> get" +
					"RepositoryFunc,\r\n            Expression<Func<TDetailEntity, TForeignKey>> foreig" +
					"nKeyExpression,\r\n            Action<TDetailEntity, TPrimaryKey> setMasterEntityK" +
					"eyAction,\r\n            Func<IRepositoryQuery<TDetailEntity>, IQueryable<TDetailE" +
					"ntityProjection>> projection = null)\r\n            where TDetailEntity : class, n" +
					"ew()\r\n            where TDetailEntityProjection : class, new() {\r\n            re" +
					"turn GetCollectionViewModelCore<InstantFeedbackCollectionViewModel<TDetailEntity" +
					", TDetailEntityProjection, TDetailPrimaryKey, TUnitOfWork>, TDetailEntity, TDeta" +
					"ilEntity, TForeignKey>(propertyExpression, () => InstantFeedbackCollectionViewMo" +
					"del<TDetailEntity, TDetailEntityProjection, TDetailPrimaryKey, TUnitOfWork>.Crea" +
					"teInstantFeedbackCollectionViewModel(UnitOfWorkFactory, getRepositoryFunc, Appen" +
					"dForeignKeyPredicate<TDetailEntity, TDetailEntityProjection, TForeignKey>(foreig" +
					"nKeyExpression, projection), () => CanCreateNewEntity()));\r\n        }\r\n");
}
			this.Write("\r\n        protected ReadOnlyCollectionViewModel<TDetailEntity, TUnitOfWork> GetRe" +
					"adOnlyDetailsCollectionViewModel<TViewModel, TDetailEntity, TForeignKey>(\r\n     " +
					"       Expression<Func<TViewModel, ReadOnlyCollectionViewModel<TDetailEntity, TD" +
					"etailEntity, TUnitOfWork>>> propertyExpression,\r\n            Func<TUnitOfWork, I" +
					"ReadOnlyRepository<TDetailEntity>> getRepositoryFunc,\r\n            Expression<Fu" +
					"nc<TDetailEntity, TForeignKey>> foreignKeyExpression,\r\n            Func<IReposit" +
					"oryQuery<TDetailEntity>, IQueryable<TDetailEntity>> projection = null) where TDe" +
					"tailEntity : class {\r\n            return GetCollectionViewModelCore<ReadOnlyColl" +
					"ectionViewModel<TDetailEntity, TUnitOfWork>, TDetailEntity, TDetailEntity, TFore" +
					"ignKey>(propertyExpression, () => ReadOnlyCollectionViewModel<TDetailEntity, TUn" +
					"itOfWork>.CreateReadOnlyCollectionViewModel(UnitOfWorkFactory, getRepositoryFunc" +
					", AppendForeignKeyPredicate<TDetailEntity, TDetailEntity, TForeignKey>(foreignKe" +
					"yExpression, projection)));\r\n        }\r\n\r\n        protected ReadOnlyCollectionVi" +
					"ewModel<TDetailEntity, TDetailProjection, TUnitOfWork> GetReadOnlyDetailProjecti" +
					"onsCollectionViewModel<TViewModel, TDetailEntity, TDetailProjection, TForeignKey" +
					">(\r\n            Expression<Func<TViewModel, ReadOnlyCollectionViewModel<TDetailE" +
					"ntity, TDetailProjection, TUnitOfWork>>> propertyExpression,\r\n            Func<T" +
					"UnitOfWork, IReadOnlyRepository<TDetailEntity>> getRepositoryFunc,\r\n            " +
					"Expression<Func<TDetailEntity, TForeignKey>> foreignKeyExpression,\r\n            " +
					"Func<IRepositoryQuery<TDetailEntity>, IQueryable<TDetailProjection>> projection)" +
					" where TDetailEntity : class where TDetailProjection : class {\r\n            retu" +
					"rn GetCollectionViewModelCore<ReadOnlyCollectionViewModel<TDetailEntity, TDetail" +
					"Projection, TUnitOfWork>, TDetailEntity, TDetailProjection, TForeignKey>(propert" +
					"yExpression, () => ReadOnlyCollectionViewModel<TDetailEntity, TDetailProjection," +
					" TUnitOfWork>.CreateReadOnlyProjectionCollectionViewModel(UnitOfWorkFactory, get" +
					"RepositoryFunc, AppendForeignKeyPredicate<TDetailEntity, TDetailProjection, TFor" +
					"eignKey>(foreignKeyExpression, projection)));\r\n        }\r\n\r\n        Func<IReposi" +
					"toryQuery<TDetailEntity>, IQueryable<TDetailProjection>> AppendForeignKeyPredica" +
					"te<TDetailEntity, TDetailProjection, TForeignKey>(\r\n            Expression<Func<" +
					"TDetailEntity, TForeignKey>> foreignKeyExpression,\r\n            Func<IRepository" +
					"Query<TDetailEntity>, IQueryable<TDetailProjection>> projection)\r\n            wh" +
					"ere TDetailEntity : class\r\n            where TDetailProjection : class {\r\n      " +
					"      var predicate = ExpressionHelper.GetKeyEqualsExpression<TDetailEntity, TDe" +
					"tailEntity, TForeignKey>(foreignKeyExpression, (TForeignKey)(object)PrimaryKey);" +
					"\r\n            return ReadOnlyRepositoryExtensions.AppendToProjection<TDetailEnti" +
					"ty, TDetailProjection>(predicate, projection);\r\n        }\r\n\r\n        protected I" +
					"EntitiesViewModel<TLookUpEntity> GetLookUpEntitiesViewModel<TViewModel, TLookUpE" +
					"ntity, TLookUpEntityKey>(Expression<Func<TViewModel, IEntitiesViewModel<TLookUpE" +
					"ntity>>> propertyExpression, Func<TUnitOfWork, IRepository<TLookUpEntity, TLookU" +
					"pEntityKey>> getRepositoryFunc, Func<IRepositoryQuery<TLookUpEntity>, IQueryable" +
					"<TLookUpEntity>> projection = null) where TLookUpEntity : class {\r\n            r" +
					"eturn GetLookUpProjectionsViewModel(propertyExpression, getRepositoryFunc, proje" +
					"ction);\r\n        }\r\n\r\n        protected virtual IEntitiesViewModel<TLookUpProjec" +
					"tion> GetLookUpProjectionsViewModel<TViewModel, TLookUpEntity, TLookUpProjection" +
					", TLookUpEntityKey>(Expression<Func<TViewModel, IEntitiesViewModel<TLookUpProjec" +
					"tion>>> propertyExpression, Func<TUnitOfWork, IRepository<TLookUpEntity, TLookUp" +
					"EntityKey>> getRepositoryFunc, Func<IRepositoryQuery<TLookUpEntity>, IQueryable<" +
					"TLookUpProjection>> projection) where TLookUpEntity : class where TLookUpProject" +
					"ion : class {\r\n            return GetEntitiesViewModelCore<IEntitiesViewModel<TL" +
					"ookUpProjection>, TLookUpProjection>(propertyExpression, () => LookUpEntitiesVie" +
					"wModel<TLookUpEntity, TLookUpProjection, TLookUpEntityKey, TUnitOfWork>.Create(U" +
					"nitOfWorkFactory, getRepositoryFunc, projection));\r\n        }\r\n\r\n        Action<" +
					"TDetailEntity> CreateForeignKeyPropertyInitializer<TDetailEntity, TForeignKey>(A" +
					"ction<TDetailEntity, TPrimaryKey> setMasterEntityKeyAction, Func<TForeignKey> ge" +
					"tMasterEntityKey) where TDetailEntity : class {\r\n            return x => setMast" +
					"erEntityKeyAction(x, (TPrimaryKey)(object)getMasterEntityKey());\r\n        }\r\n\r\n " +
					"       protected virtual bool CanCreateNewEntity() {\r\n            if(!IsNew())\r\n" +
					"                return true;\r\n            string message = string.Format(CommonR" +
					"esources.Confirmation_SaveParent, typeof(TEntity).Name);\r\n            var result" +
					" = MessageBoxService.ShowMessage(message, CommonResources.Confirmation_Caption, " +
					"MessageButton.YesNo);\r\n            return result == MessageResult.Yes && SaveCor" +
					"e();\r\n        }\r\n\r\n        TViewModel GetCollectionViewModelCore<TViewModel, TDe" +
					"tailEntity, TDetailProjection, TForeignKey>(\r\n            LambdaExpression prope" +
					"rtyExpression,\r\n            Func<TViewModel> createViewModelCallback)\r\n         " +
					"   where TViewModel : IDocumentContent\r\n            where TDetailEntity : class\r" +
					"\n            where TDetailProjection : class {\r\n            return GetEntitiesVi" +
					"ewModelCore<TViewModel, TDetailProjection>(propertyExpression, () =>\r\n          " +
					"  {\r\n                var viewModel = createViewModelCallback();\r\n               " +
					" viewModel.SetParentViewModel(this);\r\n                return viewModel;\r\n       " +
					"     });\r\n        }\r\n\r\n        TViewModel GetEntitiesViewModelCore<TViewModel, T" +
					"DetailEntity>(LambdaExpression propertyExpression, Func<TViewModel> createViewMo" +
					"delCallback)\r\n            where TViewModel : IDocumentContent\r\n            where" +
					" TDetailEntity : class {\r\n\r\n            IDocumentContent result = null;\r\n       " +
					"     string propertyName = ExpressionHelper.GetPropertyName(propertyExpression);" +
					"\r\n            if(!lookUpViewModels.TryGetValue(propertyName, out result)) {\r\n   " +
					"             result = createViewModelCallback();\r\n                lookUpViewMode" +
					"ls[propertyName] = result;\r\n            }\r\n            return (TViewModel)result" +
					";\r\n        }\r\n        #endregion\r\n\r\n        #region ISupportParameter\r\n        o" +
					"bject ISupportParameter.Parameter {\r\n            get { return null; }\r\n         " +
					"   set { OnParameterChanged(value); }\r\n        }\r\n        #endregion\r\n\r\n        " +
					"#region IDocumentContent\r\n        object IDocumentContent.Title { get { return T" +
					"itle; } }\r\n\r\n        void IDocumentContent.OnClose(CancelEventArgs e) {\r\n       " +
					"     e.Cancel = !TryClose();\r\n\t\t\tMessenger.Default.Send(new DestroyOrphanedDocum" +
					"entsMessage());\r\n        }\r\n\r\n        void IDocumentContent.OnDestroy() {\r\n     " +
					"       OnDestroy();\r\n        }\r\n\r\n        IDocumentOwner IDocumentContent.Docume" +
					"ntOwner {\r\n            get { return DocumentOwner; }\r\n            set { Document" +
					"Owner = value; }\r\n        }\r\n        #endregion\r\n\r\n        #region ISingleObject" +
					"ViewModel\r\n        TEntity ISingleObjectViewModel<TEntity, TPrimaryKey>.Entity {" +
					" get { return Entity; } }\r\n\r\n        TPrimaryKey ISingleObjectViewModel<TEntity," +
					" TPrimaryKey>.PrimaryKey { get { return PrimaryKey; } }\r\n        #endregion\r\n\r\n\t" +
					"\t#region ISupportLogicalLayout\r\n        bool ISupportLogicalLayout.CanSerialize " +
					"{\r\n            get { return Entity != null && !IsNew(); }\r\n        }\r\n\r\n        " +
					"SingleObjectViewModelState ISupportLogicalLayout<SingleObjectViewModelState>.Sav" +
					"eState() {\r\n            return new SingleObjectViewModelState {\r\n               " +
					" Key = ExpressionHelper.GetKeyPropertyValues(PrimaryKey),\r\n                Title" +
					" = GetTitle(false)\r\n            };\r\n        }\r\n\r\n        void ISupportLogicalLay" +
					"out<SingleObjectViewModelState>.RestoreState(SingleObjectViewModelState state) {" +
					"\r\n            var key = ExpressionHelper.IsTuple<TPrimaryKey>()\r\n               " +
					"     ? ExpressionHelper.MakeTuple<TPrimaryKey>(state.Key)\r\n                    :" +
					" (TPrimaryKey)state.Key.First();\r\n            LoadEntityByKey(key);\r\n           " +
					" if(Entity == null)\r\n                UpdateTitle(state.Title + CommonResources.E" +
					"ntity_Deleted);\r\n        }\r\n\r\n\t\tIDocumentManagerService ISupportLogicalLayout.Do" +
					"cumentManagerService {\r\n            get { return this.GetService<IDocumentManage" +
					"rService>(); }\r\n        }\r\n\r\n\t\tIEnumerable<object> ISupportLogicalLayout.LookupV" +
					"iewModels {\r\n            get { return lookUpViewModels.Values; }\r\n        }\r\n   " +
					"     #endregion\r\n    }\r\n}");
			return this.GenerationEnvironment.ToString();
		}
	}
	#region Base class
	[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
	public class SingleObjectViewModelTemplateBase
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
