#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Editors {
	public class NewObjectAddingEventArgs : EventArgs {
		private object addedObject;
		public NewObjectAddingEventArgs()
			: base() {
			addedObject = null;
		}
		public object AddedObject {
			get { return addedObject; }
			set { addedObject = value; }
		}
	}
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ListEditorAttribute : Attribute {
		private Type listElementType;
		private bool isDefault;
		public ListEditorAttribute(Type listElementType, bool isDefault) {
			this.listElementType = listElementType;
			this.isDefault = isDefault;
		}
		public ListEditorAttribute(Type listElementType)
			: this(listElementType, true) {
		}
		public Type ListElementType {
			get { return listElementType; }
		}
		public bool IsDefault {
			get {
				return isDefault;
			}
		}
	}
	public abstract class ListEditor : IDisposable, IProtectedContentEditor {
		private String protectedContentText;
		private Boolean isDisposed;
		private IModelListView model;
		private String name;
		private ErrorMessages errorMessages;
		private Object control;
		private Boolean allowEdit = true;
		private Object dataSource;
		private ITypeInfo objectTypeInfo = null;
		private Int32 selectionEventsLockCount;
		private Boolean isSelectionEventsPending;
		[Obsolete("Use the ModelApplying/ModelApplied/ModelSaving/ModelSaved events or override the ApplyModel/SaveModel methods instead.")]
		internal List<IModelSynchronizable> modelSynchronizerList;
		private void errorMessages_MessagesChanged(object sender, EventArgs e) {
			OnErrorMessagesChanged();
		}
		protected Type ObjectType {
			get { return objectTypeInfo != null ? objectTypeInfo.Type : null; }
		}
		protected ITypeInfo ObjectTypeInfo {
			get { return objectTypeInfo; }
			[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
			set { objectTypeInfo = value; } 
		}
		protected abstract object CreateControlsCore();
		protected abstract void AssignDataSourceToControl(Object dataSource);
		protected virtual void OnProcessSelectedItem() {
			if(ProcessSelectedItem != null) {
				ProcessSelectedItem(this, EventArgs.Empty);
			}
		}
		protected virtual bool OnFocusedObjectChanging() {
			CancelEventArgs cancelEventArgs = new CancelEventArgs();
			if(FocusedObjectChanging != null) {
				FocusedObjectChanging(this, cancelEventArgs);
			}
			return !cancelEventArgs.Cancel;
		}
		protected virtual void OnFocusedObjectChanged() {
			if(selectionEventsLockCount == 0) {
				if(FocusedObjectChanged != null) {
					FocusedObjectChanged(this, EventArgs.Empty);
				}
			}
			else {
				isSelectionEventsPending = true;
			}
		}
		protected virtual void OnSelectionChanged() {
			if(selectionEventsLockCount == 0) {
				if(SelectionChanged != null) {
					SelectionChanged(this, EventArgs.Empty);
				}
			}
			else {
				isSelectionEventsPending = true;
			}
		}
		protected virtual void OnSelectionTypeChanged() {
			if(SelectionTypeChanged != null) {
				SelectionTypeChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnObjectChanged() {
			if(ObjectChanged != null) {
				ObjectChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnDataSourceChanged() {
			if(DataSourceChanged != null) {
				DataSourceChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnProtectedContentTextChanged() {
		}
		protected virtual void OnValidateObject(ValidateObjectEventArgs ea) {
			if(ValidateObject != null) {
				ValidateObject(this, ea);
			}
		}
		protected internal void SetModel(IModelListView newModel) {
#pragma warning disable 0618 //[Obsolete("Use the ModelApplying/ModelApplied/ModelSaving/ModelSaved events or override the ApplyModel method instead.")]
			DisposeModelSynchronizer();
#pragma warning restore 0618
			model = newModel;
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(model.ModelClass, "Model.ModelClass");
			objectTypeInfo = model.ModelClass.TypeInfo;
		}
		protected String GetDisplayablePropertyName(String memberName) {
			IMemberInfo displayableMemberDescriptor = ReflectionHelper.FindDisplayableMemberDescriptor(ObjectTypeInfo, memberName);
			if(displayableMemberDescriptor != null) {
				return displayableMemberDescriptor.BindingName;
			}
			return memberName;
		}
		protected virtual object OnNewObjectAdding() {
			NewObjectAddingEventArgs args = new NewObjectAddingEventArgs();
			if(NewObjectAdding != null) {
				NewObjectAdding(this, args);
			}
			return args.AddedObject;
		}
		protected virtual void OnNewObjectCreated() {
			if(NewObjectCreated != null) {
				NewObjectCreated(this, EventArgs.Empty);
			}
		}
		protected virtual void OnNewObjectCanceled() {
			if(NewObjectCanceled != null) {
				NewObjectCanceled(this, EventArgs.Empty);
			}
		}
		protected virtual void OnControlsCreated() {
			if(ControlsCreated != null) {
				ControlsCreated(this, EventArgs.Empty);
			}
		}
		protected virtual void OnAllowEditChanged() {
			if(AllowEditChanged != null) {
				AllowEditChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnErrorMessagesChanged() {
		}
		protected virtual void OnModelApplying(CancelEventArgs args) {
			if(ModelApplying != null) {
				ModelApplying(this, args);
			}
		}
		protected virtual void OnModelApplied() {
			if(ModelApplied != null) {
				ModelApplied(this, EventArgs.Empty);
			}
		}
		protected virtual void OnModelSaving(CancelEventArgs args) {
			if(ModelSaving != null) {
				ModelSaving(this, args);
			}
		}
		protected virtual void OnModelSaved() {
			if(ModelSaved != null) {
				ModelSaved(this, EventArgs.Empty);
			}
		}
		protected internal void LockSelectionEvents() {
			selectionEventsLockCount++;
		}
		protected internal void UnlockSelectionEvents() {
			if(selectionEventsLockCount > 0) {
				selectionEventsLockCount--;
				if((selectionEventsLockCount == 0) && (isSelectionEventsPending)) {
					isSelectionEventsPending = false;
					OnSelectionChanged();
					OnFocusedObjectChanged();
				}
			}
		}
		protected ListEditor(IModelListView model) {
			if(model != null) {
				SetModel(model);
			}
			this.errorMessages = new ErrorMessages();
			this.errorMessages.MessagesChanged += new EventHandler(errorMessages_MessagesChanged);
		}
		protected ListEditor() : this((IModelListView)null) { }
		public virtual void Dispose() {
			if(isDisposed) {
				return;
			}
			isDisposed = true;
			if(errorMessages != null) {
				errorMessages.Clear();
				errorMessages.MessagesChanged -= new EventHandler(errorMessages_MessagesChanged);
				errorMessages = null;
			}
			dataSource = null;
			FocusedObjectChanging = null;
			FocusedObjectChanged = null;
			SelectionChanged = null;
			ControlsCreated = null;
			ObjectChanged = null;
			DataSourceChanged = null;
			ProcessSelectedItem = null;
			NewObjectAdding = null;
			NewObjectCreated = null;
			NewObjectCanceled = null;
			SelectionTypeChanged = null;
			AllowEditChanged = null;
			ValidateObject = null;
			CreateCustomModelSynchronizer = null;
			ModelApplying = null;
			ModelApplied = null;
			ModelSaving = null;
			ModelSaved = null;
		}
		public object CreateControls() {
#pragma warning disable 0618 //[Obsolete("Use the ModelApplying/ModelApplied/ModelSaving/ModelSaved events or override the ApplyModel method instead.")]
			modelSynchronizerList = null;
#pragma warning restore 0618
			control = CreateControlsCore();
			AssignDataSourceToControl(dataSource);
			OnControlsCreated();
			return control;
		}
		public virtual void ApplyModel() {
#pragma warning disable 0618 //[Obsolete("Use the ModelApplying/ModelApplied/ModelSaving/ModelSaved events or override the ApplyModel method instead.")]
			if(modelSynchronizerList == null && Model != null) {
				modelSynchronizerList = new List<IModelSynchronizable>();
				List<IModelSynchronizable> modelSynchronizers = CreateModelSynchronizers();
				modelSynchronizerList.AddRange(modelSynchronizers);
				IModelSynchronizable customModelSynchronizer = OnCreateCustomModelSynchronizer();
				if(customModelSynchronizer != null) {
					modelSynchronizerList.Add(customModelSynchronizer);
				}
			}
			if(modelSynchronizerList != null) {
				foreach(IModelSynchronizable synchronizer in modelSynchronizerList) {
					synchronizer.ApplyModel();
				}
			}
#pragma warning restore 0618
		}
		[Obsolete("Use the ModelApplying/ModelApplied/ModelSaving/ModelSaved events or override the ApplyModel/SaveModel methods instead.")]
		protected IModelSynchronizable OnCreateCustomModelSynchronizer() {
			if(CreateCustomModelSynchronizer != null) {
				CreateCustomModelSynchronizerEventArgs args = new CreateCustomModelSynchronizerEventArgs(Model);
				CreateCustomModelSynchronizer(this, args);
				return args.ModelSynchronizer;
			}
			return null;
		}
		protected virtual List<IModelSynchronizable> CreateModelSynchronizers() {
			return new List<IModelSynchronizable>();
		}
		[Obsolete("Use the ModelApplying/ModelApplied/ModelSaving/ModelSaved events or override the ApplyModel/SaveModel methods instead.")]
		private void DisposeModelSynchronizer() {
			if(modelSynchronizerList != null) {
				foreach(IModelSynchronizable synchronizer in modelSynchronizerList) {
					if(synchronizer is IDisposable) {
						((IDisposable)synchronizer).Dispose();
					}
				}
				modelSynchronizerList.Clear();
			}
			modelSynchronizerList = null;
		}
		public virtual void BreakLinksToControls() {
#pragma warning disable 0618 //[Obsolete("Use the ModelApplying/ModelApplied/ModelSaving/ModelSaved events or override the ApplyModel method instead.")]
			DisposeModelSynchronizer();
#pragma warning restore 0618
			control = null;
		}
		public virtual void StartIncrementalSearch(string searchString) { }
		public abstract void Refresh();
		public virtual void SaveModel() {
#pragma warning disable 0618 //[Obsolete("Use the ModelApplying/ModelApplied/ModelSaving/ModelSaved events or override the ApplyModel method instead.")]
			if(modelSynchronizerList != null) {
				foreach(IModelSynchronizable synchronizer in modelSynchronizerList) {
					synchronizer.SynchronizeModel();
				}
			}
#pragma warning restore 0618
		}
		public abstract IList GetSelectedObjects();
		public virtual Boolean SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode) {
			return (dataAccessMode == CollectionSourceDataAccessMode.Client) || (dataAccessMode == CollectionSourceDataAccessMode.Server);
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorRequiredProperties")]
#endif
		public virtual String[] RequiredProperties {
			get {
				List<String> result = new List<String>();
				foreach(IModelColumn column in model.Columns) {
					result.Add(GetDisplayablePropertyName(column.PropertyName));
				}
				return result.ToArray();
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorSelectionType")]
#endif
		public abstract SelectionType SelectionType {
			get;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorContextMenuTemplate")]
#endif
		public abstract IContextMenuTemplate ContextMenuTemplate {
			get;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorDataSource")]
#endif
		public Object DataSource {
			get {
				return dataSource;
			}
			set {
				dataSource = value;
				AssignDataSourceToControl(value);
				OnDataSourceChanged();
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorFocusedObject")]
#endif
		public virtual object FocusedObject {
			get {
				IList selectedObjects = GetSelectedObjects();
				if(selectedObjects.Count == 1) {
					return selectedObjects[0];
				}
				return null;
			}
			set { }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorModel")]
#endif
		public IModelListView Model {
			get { return model; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorControl")]
#endif
		public object Control {
			get { return control; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorErrorMessages")]
#endif
		public ErrorMessages ErrorMessages {
			get { return errorMessages; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorIsDisposed")]
#endif
		public bool IsDisposed {
			get { return isDisposed; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorName")]
#endif
		public virtual string Name {
			get { return name; }
			set { name = value; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ListEditorProtectedContentText")]
#endif
		public string ProtectedContentText {
			get { return protectedContentText; }
			set {
				if(protectedContentText != value) {
					protectedContentText = value;
					OnProtectedContentTextChanged();
				}
			}
		}
		public virtual bool AllowEdit {
			get { return allowEdit; }
			set {
				if(allowEdit != value) {
					allowEdit = value;
					OnAllowEditChanged();
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IList List {
			get { return ListHelper.GetList(DataSource); }
		}
		public event EventHandler<CancelEventArgs> FocusedObjectChanging;
		public event EventHandler FocusedObjectChanged;
		public event EventHandler SelectionChanged;
		public event EventHandler SelectionTypeChanged;
		public event EventHandler ControlsCreated;
		public event EventHandler ProcessSelectedItem;
		public event EventHandler ObjectChanged;
		public event EventHandler DataSourceChanged;
		public event EventHandler AllowEditChanged;
		public event EventHandler<ValidateObjectEventArgs> ValidateObject;
		public event EventHandler<NewObjectAddingEventArgs> NewObjectAdding;
		public event EventHandler NewObjectCreated;
		public event EventHandler NewObjectCanceled;
		[Obsolete("Use the ModelApplying/ModelApplied/ModelSaving/ModelSaved events or override the ApplyModel/SaveModel methods instead.")]
		public event EventHandler<CreateCustomModelSynchronizerEventArgs> CreateCustomModelSynchronizer;
		public event EventHandler<CancelEventArgs> ModelApplying; 
		public event EventHandler<EventArgs> ModelApplied;
		public event EventHandler<CancelEventArgs> ModelSaving;
		public event EventHandler<EventArgs> ModelSaved;
	}
	[Obsolete(ObsoleteMessages.ClassForInternalUseOnly), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class NonPersistentEditableAttribute : Attribute {
	}
	[Obsolete("Use the ModelApplying/ModelApplied/ModelSaving/ModelSaved events or override the ApplyModel/SaveModel methods instead.")]
	public class CreateCustomModelSynchronizerEventArgs : EventArgs {
		private IModelNode model;
		private IModelSynchronizable modelSynchronizer;
		public CreateCustomModelSynchronizerEventArgs(IModelNode model) {
			this.model = model;
		}
		public IModelNode Model {
			get { return model; }
		}
		public IModelSynchronizable ModelSynchronizer {
			get { return modelSynchronizer; }
			set { modelSynchronizer = value; }
		}
	}
}
