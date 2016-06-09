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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.CloneObject {
	public class CustomGetCloneActionTargetTypesEventArgs : HandledEventArgs {
		private IModelApplication applicationModel;
		private ITypeInfo sourceType;
		private Dictionary<IModelNode, Type> targetTypes = new Dictionary<IModelNode, Type>();
		public CustomGetCloneActionTargetTypesEventArgs(ITypeInfo sourceType, IModelApplication applicationModel) {
			Guard.ArgumentNotNull(sourceType, "sourceType");
			this.sourceType = sourceType;
			this.applicationModel = applicationModel;
		}
		public Dictionary<IModelNode, Type> GetDefaultTargetTypes() {
			Dictionary<IModelNode, Type> result = new Dictionary<IModelNode, Type>();
			if(applicationModel != null) {
				CloneObjectActionHelper helper = new CloneObjectActionHelper(applicationModel);
				IList<Type> targetTypes = helper.GetTargetTypes(sourceType.Type);
				foreach(Type destinationType in targetTypes) {
					IModelClass modelClass = (IModelClass)applicationModel.BOModel.GetClass(destinationType);
					if(modelClass != null) {
						if(destinationType.IsInterface) {
							IDCEntityStore dcEntityStore = (IDCEntityStore)((TypesInfo)XafTypesInfo.Instance).FindEntityStore(typeof(IDCEntityStore));
							Type generatedEntityType = dcEntityStore != null ? dcEntityStore.GetGeneratedEntityType(destinationType) : null;
							if(generatedEntityType != null) {
								result.Add(modelClass, generatedEntityType);
							}
						}
						else {
							result.Add(modelClass, destinationType);
						}
					}
				}
			}
			return result;
		}
		public ITypeInfo SourceType {
			get { return sourceType; }
		}
		public Dictionary<IModelNode, Type> TargetTypes {
			get { return targetTypes; }
		}
	}
	public class CustomCloneObjectEventArgs : EventArgs {
		private XafApplication application;
		private Frame frame;
		private View view;
		public CustomCloneObjectEventArgs(object sourceObject, Type targetType, XafApplication application, Frame frame, View view) {
			this.SourceObject = sourceObject;
			this.TargetType = targetType;
			this.application = application;
			this.frame = frame;
			this.view = view;
		}
		public IObjectSpace TargetObjectSpace { get; set; }
		public IObjectSpace CreateDefaultTargetObjectSpace() {
			if(application.ShowDetailViewFrom(frame) || (view.ObjectSpace.Owner == view)) {
				return application.GetObjectSpaceToShowDetailViewFrom(frame, TargetType);
			}
			else {
				return view.ObjectSpace;
			}
		}
		public object SourceObject { get; private set; }
		public object ClonedObject { get; set; }
		public Type TargetType { get; private set; }
	}
	public class CustomShowClonedObjectEventArgs : HandledEventArgs {
		private object sourceObject;
		private object clonedObject;
		private IObjectSpace targetObjectSpace;
		public CustomShowClonedObjectEventArgs(IObjectSpace targetObjectSpace, object sourceObject, object clonedObject) {
			this.targetObjectSpace = targetObjectSpace;
			this.sourceObject = sourceObject;
			this.clonedObject = clonedObject;
		}
		public IObjectSpace TargetObjectSpace {
			get { return targetObjectSpace; }
		}
		public object SourceObject {
			get { return sourceObject; }
		}
		public object ClonedObject {
			get { return clonedObject; }
		}
	}
	public class CloneObjectViewController : ObjectViewController {
		public const string CloneObjectActionId = "CloneObject";
		public const string CloneObjectActionImageName = "MenuBar_CloneObject";
		public const string IsNotModifiedEnabledKey = "NotModified";
		private const string AllowNewActiveKey = "AllowNew";
		private SingleChoiceAction cloneObjectAction;
		private bool allowCloneWhenModified = false;
		private void SynchActionWithViewReadOnly() {
			foreach(ChoiceActionItem item in CloneObjectAction.Items) {
				if(View.IsRoot) {
					if(((ObjectView)View).ObjectTypeInfo.Type.IsAssignableFrom((Type)item.Data)) {
						item.Enabled["ViewIsNotReadOnly"] = View.AllowNew;
					}
				}
				else {
					item.Enabled["ViewIsNotReadOnly"] = View.AllowNew;
				}
			}
		}
		private void CloneObjectAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs args) {
			CloneObject(args);
		}
		private void View_AllowNewChanged(object sender, EventArgs e) {
			UpdateActionsState();
		}
		private void ObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			UpdateActionsState();
		}
		protected virtual void UpdateActionsState() {
			SynchActionWithViewReadOnly();
			cloneObjectAction.Active.SetItemValue(AllowNewActiveKey, View.AllowNew);
			cloneObjectAction.Enabled[IsNotModifiedEnabledKey] = allowCloneWhenModified || !View.ObjectSpace.IsModified;
			NewObjectViewController.UpdateIsManyToManyKey(View, cloneObjectAction);
		}
		protected virtual void CloneObject(SingleChoiceActionExecuteEventArgs args) {
			if(args.SelectedChoiceActionItem != null) {
				object currentObject = View.ObjectSpace.GetObject(View.CurrentObject);
				Type targetType = (Type)args.SelectedChoiceActionItem.Data;
				IObjectSpace objectSpace;
				object clonedObj;
				CloneObject(currentObject, targetType, out objectSpace, out clonedObj);
				ShowClonedObject(args, currentObject, objectSpace, clonedObj);
			}
		}
		protected void CloneObject(object currentObject, Type targetType, out IObjectSpace targetObjectSpace, out object clonedObj) {
			Guard.ArgumentNotNull(currentObject, "currentObject");
			CustomCloneObjectEventArgs cloneObjectArgs = new CustomCloneObjectEventArgs(currentObject, targetType, Application, Frame, View);
			OnCustomCloneObject(cloneObjectArgs);
			if(cloneObjectArgs.ClonedObject != null) {
				Guard.ArgumentNotNull(cloneObjectArgs.TargetObjectSpace, "cloneObjectArgs.TargetObjectSpace"); 
				clonedObj = cloneObjectArgs.ClonedObject;
				targetObjectSpace = cloneObjectArgs.TargetObjectSpace;
			}
			else {
				if(cloneObjectArgs.TargetObjectSpace == null) {
					cloneObjectArgs.TargetObjectSpace = cloneObjectArgs.CreateDefaultTargetObjectSpace();
				}
				targetObjectSpace = cloneObjectArgs.TargetObjectSpace;
				Guard.ArgumentNotNull(targetObjectSpace, "targetObjectSpace");
				object objectFromTargetObjectSpace = targetObjectSpace.GetObject(currentObject);
				if(objectFromTargetObjectSpace == null) {
					Guard.ArgumentNotNull(objectFromTargetObjectSpace, "objectFromTargetObjectSpace");
				}
				clonedObj = new Cloner().CloneTo(objectFromTargetObjectSpace, targetType);
			}
		}
		protected void ShowClonedObject(SingleChoiceActionExecuteEventArgs args, object currentObject, IObjectSpace objectSpace, object clonedObj) {
			CustomShowClonedObjectEventArgs showClonedObjectArgs = new CustomShowClonedObjectEventArgs(objectSpace, currentObject, clonedObj);
			OnCustomShowClonedObject(showClonedObjectArgs);
			if(!showClonedObjectArgs.Handled) {
				string viewId = Application.FindDetailViewId(currentObject.GetType());
				if(!string.IsNullOrEmpty(viewId) && Application.ShowDetailViewFrom(Frame)) { 
					objectSpace.SetModified(clonedObj);
					args.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, clonedObj, View);
				}
				else {
					if(View is ListView) {
						((ListView)View).CollectionSource.Add(clonedObj);
					}
					else {
						throw new InvalidOperationException("viewId is null");
					}
				}
			}
		}
		protected virtual void OnCustomCloneObject(CustomCloneObjectEventArgs args) {
			if(CustomCloneObject != null) {
				CustomCloneObject(this, args);
			}
		}
		protected virtual void OnCustomShowClonedObject(CustomShowClonedObjectEventArgs showClonedObjectArgs) {
			if(CustomShowClonedObject != null) {
				CustomShowClonedObject(this, showClonedObjectArgs);
			}
		}
		public CloneObjectViewController() {
			TypeOfView = typeof(ObjectView);
			this.cloneObjectAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this, CloneObjectActionId, PredefinedCategory.ObjectsCreation);
			this.cloneObjectAction.Caption = "Clone...";
			this.cloneObjectAction.ToolTip = "Clone object";
			this.cloneObjectAction.ImageName = CloneObjectActionImageName;
			this.cloneObjectAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
			this.cloneObjectAction.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
			this.cloneObjectAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.CloneObjectAction_OnExecute);
		}
		protected override void OnActivated() {
			base.OnActivated();
			CloneObjectAction.BeginUpdate();
			try {
				CloneObjectAction.Items.Clear();
				Dictionary<IModelNode, Type> targetTypes = GetCloneActionTargetTypes(((ObjectView)View).ObjectTypeInfo);
				foreach(IModelNode node in targetTypes.Keys) {
					IModelClass modelClass = (IModelClass)node;
					ChoiceActionItem item = new ChoiceActionItem(modelClass.Name, modelClass.Caption, targetTypes[node]);
					item.ImageName = modelClass.ImageName;
					CloneObjectAction.Items.Add(item);
					string diagnosticInfo = "";
					if(!DataManipulationRight.CanCreate(null, targetTypes[node], null, out diagnosticInfo)) {
						item.Active.SetItemValue("Security", false);
					}
				}
			}
			finally {
				CloneObjectAction.EndUpdate();
			}
			UpdateActionsState();
			View.AllowNewChanged += new EventHandler(View_AllowNewChanged);
			View.ObjectSpace.ModifiedChanged += new EventHandler(ObjectSpace_ModifiedChanged);
			if(View is ListView && ((ListView)View).CollectionSource is PropertyCollectionSource) {
				PropertyCollectionSource collectionSource = (PropertyCollectionSource)((ListView)View).CollectionSource;
				UpdateActionState(collectionSource);
				collectionSource.MasterObjectChanged += collectionSource_MasterObjectChanged;
			}
		}
		void collectionSource_MasterObjectChanged(object sender, EventArgs e) {
			UpdateActionState((PropertyCollectionSource)sender);
		}
		private void UpdateActionState(PropertyCollectionSource collectionSource) {
			if(SecuritySystem.Instance != null && SecuritySystem.Instance is IRequestSecurity && collectionSource.MasterObject != null) {
				CloneObjectAction.Enabled["IsGranted"] = SecuritySystem.IsGranted(new PermissionRequest(collectionSource.ObjectSpace, collectionSource.MasterObjectType, SecurityOperations.Write, collectionSource.MasterObject, collectionSource.MemberInfo.Name));
			}
			else {
				CloneObjectAction.Enabled.RemoveItem("IsGranted");
			}
		}
		private Dictionary<IModelNode, Type> GetCloneActionTargetTypes(ITypeInfo sourceType) {
			CustomGetCloneActionTargetTypesEventArgs args = new CustomGetCloneActionTargetTypesEventArgs(sourceType, GetApplicationModel());
			if(CustomGetCloneActionTargetTypes != null) {
				CustomGetCloneActionTargetTypes(this, args);
				if(args.Handled) {
					return args.TargetTypes;
				}
			}
			return args.GetDefaultTargetTypes();
		}
		private IModelApplication GetApplicationModel() {
			return (Application != null) ? Application.Model : null;
		}
		protected override void OnDeactivated() {
			View.AllowNewChanged -= new EventHandler(View_AllowNewChanged);
			View.ObjectSpace.ModifiedChanged -= new EventHandler(ObjectSpace_ModifiedChanged);
			if(View is ListView && ((ListView)View).CollectionSource is PropertyCollectionSource) {
				((PropertyCollectionSource)((ListView)View).CollectionSource).MasterObjectChanged -= collectionSource_MasterObjectChanged;
			}
			base.OnDeactivated();
		}
		[DefaultValue(false)]
		public bool AllowCloneWhenModified { get { return allowCloneWhenModified; } set { allowCloneWhenModified = value; } }
		public SingleChoiceAction CloneObjectAction {
			get { return cloneObjectAction; }
		}
		public event EventHandler<CustomCloneObjectEventArgs> CustomCloneObject;
		public event EventHandler<CustomShowClonedObjectEventArgs> CustomShowClonedObject;
		public event EventHandler<CustomGetCloneActionTargetTypesEventArgs> CustomGetCloneActionTargetTypes;
	}
	public class CloneObjectActionHelper {
		private IModelApplication application;
		private bool IsCloneable(Type type) {
			IModelClassCloneable modelClass = (IModelClassCloneable)application.BOModel.GetClass(type);
			return modelClass != null ? modelClass.IsCloneable : false;
		}
		private bool IsCreatable(Type type) {
			if(type.IsClass && !type.IsAbstract) {
				return true;
			}
			IDCEntityStore dcEntityStore = (IDCEntityStore)((TypesInfo)XafTypesInfo.Instance).FindEntityStore(typeof(IDCEntityStore));
			return type.IsInterface && dcEntityStore != null && dcEntityStore.GetGeneratedEntityType(type) != null;
		}
		public IList<Type> GetTargetTypes(Type sourceType) {
			if(IsCloneable(sourceType)) {
				Type rootClonedType = sourceType;
				Type currentType = sourceType.BaseType;
				while(currentType != null) {
					if(IsCloneable(currentType)) {
						rootClonedType = currentType;
					}
					currentType = currentType.BaseType;
				}
				List<Type> result = GetDescendantTargetTypes(XafTypesInfo.Instance.FindTypeInfo(rootClonedType), sourceType);
				return result;
			}
			else {
				return Type.EmptyTypes;
			}
		}
		private List<Type> GetDescendantTargetTypes(ITypeInfo currentTypeInfo, Type sourceType) {
			List<Type> result = new List<Type>();
			foreach(ITypeInfo typeInfo in currentTypeInfo.Descendants) {
				if(IsCloneable(typeInfo.Type) && IsCreatable(typeInfo.Type)) {
					result.Add(typeInfo.Type);
				}
			}
			result.Remove(sourceType);
			if(IsCreatable(sourceType)) {
				result.Insert(0, sourceType);
			}
			return result;
		}
		public CloneObjectActionHelper(IModelApplication application) {
			this.application = application;
		}
	}
}
