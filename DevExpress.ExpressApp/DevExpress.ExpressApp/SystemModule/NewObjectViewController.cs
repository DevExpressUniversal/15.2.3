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
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	public class ModelCreatableItemsGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) {
			IModelCreatableItems creatabelItems = (IModelCreatableItems)node;
			foreach(IModelClass classInfo in node.Application.BOModel) {
				if(classInfo.IsCreatableItem) {
					IModelCreatableItem item = creatabelItems.AddNode<IModelCreatableItem>(classInfo.Name);
					item.SetValue<IModelClass>("ModelClass", classInfo);
				}
			}
		}
	}
	public interface IModelApplicationCreatableItems {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelApplicationCreatableItemsCreatableItems")]
#endif
		IModelCreatableItems CreatableItems { get; }
	}
	[ModelNodesGenerator(typeof(ModelCreatableItemsGenerator))]
	[ImageName("ModelEditor_CreatableItems_Object")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelCreatableItems")]
#endif
	public interface IModelCreatableItems : IModelNode, IModelList<IModelCreatableItem> { }
	[DisplayProperty("Caption")]
	[KeyProperty("ClassName")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelCreatableItem")]
#endif
	public interface IModelCreatableItem : IModelNode, IModelBaseChoiceActionItem {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCreatableItemModelClass"),
#endif
 Required]
		[ModelPersistentName("ClassName")]
		[DataSourceProperty("Application.BOModel")]
		[Category("Data")]
		IModelClass ModelClass { get; set; }
		[Browsable(false)]
		string ClassName { get; set; }
	}
	[DomainLogic(typeof(IModelCreatableItem))]
	public static class ModelCreatableItemLogic {
		public static IModelClass Get_ModelClass(IModelCreatableItem creatableItem) {
			return creatableItem.Application.BOModel[creatableItem.ClassName];
		}
		public static void Set_ModelClass(IModelCreatableItem creatableItem, IModelClass modelClass) {
			creatableItem.ClassName = modelClass.Name;
		}
		public static string Get_Caption(IModelCreatableItem creatableItem) {
			if(creatableItem.ModelClass != null) {
				return creatableItem.ModelClass.Caption;
			}
			return creatableItem.Id;
		}
		public static string Get_ImageName(IModelCreatableItem creatableItem) {
			if(creatableItem.ModelClass != null) {
				return creatableItem.ModelClass.ImageName;
			}
			return null;
		}
	}
	public class NewObjectViewController : ViewController, IComparer<ChoiceActionItem>, IModelExtender {
		public const string SecurityAllowNewByPermissions = "SecurityAllowNewByPermissions";
		public const string NewActionId = "New";
		private SingleChoiceAction newObjectAction;
		private System.ComponentModel.IContainer components;
		private PropertyCollectionSourceLink propertyCollectionSourceLink;
		private List<Type> orderedIndexedCreatableItemTypes;
		private Dictionary<ChoiceActionItem, String> choiceActionItemCaptions;
		protected Boolean linkNewObjectToParentImmediately = true;
		int IComparer<ChoiceActionItem>.Compare(ChoiceActionItem x, ChoiceActionItem y) {
			if(x.Data == y.Data)
				return 0;
			if(object.ReferenceEquals(x.Data, ((ObjectView)View).ObjectTypeInfo.Type))
				return -1;
			if(object.ReferenceEquals(y.Data, ((ObjectView)View).ObjectTypeInfo.Type))
				return 1;
			int xIndex = GetChoiceActionItemOrderIndex(x);
			int yIndex = GetChoiceActionItemOrderIndex(y);
			if(xIndex != yIndex) {
				if((yIndex == -1) || ((xIndex < yIndex) && (xIndex != -1))) {
					return -1;
				}
				else {
					return 1;
				}
			}
			String xCaption = GetChoiceActionItemCaption(x);
			if(!String.IsNullOrEmpty(xCaption)) {
				return xCaption.CompareTo(GetChoiceActionItemCaption(y));
			}
			else {
				return ((Type)x.Data).Name.CompareTo(((Type)y.Data).Name);
			}
		}
		private int GetChoiceActionItemOrderIndex(ChoiceActionItem item) {
			return orderedIndexedCreatableItemTypes.IndexOf(item.Data as Type);
		}
		private void UpdateOrderedIndexedCreatableItemTypes() {
			orderedIndexedCreatableItemTypes.Clear();
			if((Application != null) && (Application.Model != null)) {
				foreach(IModelCreatableItem creatableItem in ((IModelApplicationCreatableItems)Application.Model).CreatableItems) {
					bool isIndexHasValue = ((ModelNode)creatableItem).HasValue("Index");
					if(isIndexHasValue
						&& creatableItem.ModelClass != null
						&& creatableItem.ModelClass.TypeInfo != null) {
						orderedIndexedCreatableItemTypes.Add(creatableItem.ModelClass.TypeInfo.Type);
					}
				}
			}
		}
		private void SortChoiceActionItems(List<ChoiceActionItem> choiceActionItems) {
			choiceActionItemCaptions.Clear();
			foreach(ChoiceActionItem item in choiceActionItems) {
				if(!choiceActionItemCaptions.ContainsKey(item)) {
					choiceActionItemCaptions.Add(item, item.Caption);
				}
			}
			choiceActionItems.Sort(this);
			choiceActionItemCaptions.Clear();
		}
		private String GetChoiceActionItemCaption(ChoiceActionItem item) {
			String caption;
			if(choiceActionItemCaptions.TryGetValue(item, out caption)) {
				return caption;
			}
			return item.Caption;
		}
		private void View_AllowNewChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void CollectionSource_CollectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.newObjectAction = new SingleChoiceAction(this.components);
			this.newObjectAction.Caption = "New";
			this.newObjectAction.Category = "ObjectsCreation";
			this.newObjectAction.Id = NewActionId;
			this.newObjectAction.ImageName = "MenuBar_New";
			this.newObjectAction.Shortcut = "CtrlN";
			this.newObjectAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			this.newObjectAction.Execute += new SingleChoiceActionExecuteEventHandler(this.newObjectAction_OnExecute);
		}
		private PropertyCollectionSource GetPropertyCollectionSource() {
			if(View == null || !(View is ListView)) {
				return null;
			}
			return (View != null) ? (((ListView)View).CollectionSource as PropertyCollectionSource) : null;
		}
		private Object GetCollectionOwner(Object masterObject, IMemberInfo memberInfo) {
			return memberInfo.GetOwnerInstance(masterObject);
		}
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			NewObjectAction.Enabled.SetItemValue("MasterObjectIsNew", true);
		}
		private void AddNewObjectToCollectionSource(CollectionSourceBase currentCollectionSource, Object newObject, IObjectSpace objectSpace) {
			ITypeInfo newObjectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(newObject.GetType());
			if((currentCollectionSource != null) && currentCollectionSource.ObjectTypeInfo.IsAssignableFrom(newObjectTypeInfo)) {
				if(objectSpace == currentCollectionSource.ObjectSpace) {
					currentCollectionSource.Add(newObject);
				}
				else {
					PropertyCollectionSource propertyCollectionSource = (currentCollectionSource as PropertyCollectionSource);
					ListView listview = GetListView();
					if(propertyCollectionSource != null
						&& propertyCollectionSource.MasterObject != null
						&& (propertyCollectionSource.MemberInfo.IsAggregated || propertyCollectionSource  is LookupEditPropertyCollectionSource || listview == null || (listview != null && linkNewObjectToParentImmediately))) {
						Object collectionOwner = null;
						IMemberInfo collectionMemberInfo = null;
						if(propertyCollectionSource.MemberInfo.GetPath().Count > 1) {
							collectionOwner = GetCollectionOwner(propertyCollectionSource.MasterObject, propertyCollectionSource.MemberInfo);
							collectionMemberInfo = propertyCollectionSource.MemberInfo.LastMember;
						}
						else {
							collectionOwner = propertyCollectionSource.MasterObject;
							collectionMemberInfo = propertyCollectionSource.MemberInfo;
						}
						if((collectionOwner != null) && XafTypesInfo.Instance.FindTypeInfo(collectionOwner.GetType()).IsPersistent) {
							PropertyCollectionSource collectionSource = Application.CreatePropertyCollectionSource(
								objectSpace, propertyCollectionSource.MasterObjectType, objectSpace.GetObject(collectionOwner), collectionMemberInfo, "", propertyCollectionSource.DataAccessMode, CollectionSourceMode.Normal);
							collectionSource.Add(newObject);
						}
					}
				}
			}
		}
		private ListView GetListView() {
			if(ListView != null) {
				return ListView;
			}
			LinkToListViewController linkToListViewController = Frame.GetController<LinkToListViewController>();
			if((linkToListViewController != null) && (linkToListViewController.Link != null)) {
				return linkToListViewController.Link.ListView;
			}
			return null;
		}
		private void SetLinkNewObjectToParentImmediatelyToPropertyCollectionSourceLink() {
			if(Frame != null) {
				LinkToListViewController linkToListViewController = Frame.GetController<LinkToListViewController>();
				if((linkToListViewController != null) && (linkToListViewController.Link != null) && (linkToListViewController.Link.PropertyCollectionSourceLink != null)) {
					linkToListViewController.Link.PropertyCollectionSourceLink.LinkNewObjectToParentImmediately = linkNewObjectToParentImmediately;
				}
			}
		}
		private void newObjectAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs args) {
			New(args);
		}
		private void CommitMasterObject(CollectionSourceBase currentCollectionSource, IObjectSpace objectSpaceForNewObject) {
			PropertyCollectionSource propertyCollectionSource = currentCollectionSource as PropertyCollectionSource;
			LookupEditPropertyCollectionSource lookupCollectionSource = (propertyCollectionSource as LookupEditPropertyCollectionSource);
			if(ObjectSpace.IsModified
					&& (objectSpaceForNewObject != ObjectSpace)
					&& (propertyCollectionSource != null)
					&& ObjectSpace.IsNewObject(GetCollectionOwner(propertyCollectionSource.MasterObject, propertyCollectionSource.MemberInfo))
					&& !(objectSpaceForNewObject is INestedObjectSpace)
					&& ((lookupCollectionSource == null) || ((lookupCollectionSource.LookupMode == LookupEditCollectionSourceMode.Link)))
					&& (ListView != null)
					&& (propertyCollectionSource.MemberInfo.IsAggregated || linkNewObjectToParentImmediately)
					&& (ListView.EditView == null)) {
				ObjectSpace.CommitChanges();
			}
		}
		protected List<ITypeInfo> currentViewTypes;
		protected ChoiceActionItem CreateItem(Type type, IModelBaseChoiceActionItem info) {
			if(info == null) {
				ChoiceActionItem choiceActionItem = new ChoiceActionItem(type.Name, type);
				IModelClass modelClass = Application.Model.BOModel.GetClass(type);
				if(modelClass != null) {
					choiceActionItem.Caption = modelClass.Caption;
					choiceActionItem.ImageName = modelClass.ImageName;
				}
				return choiceActionItem;
			}
			else {
				return new ChoiceActionItem(info, type);
			}
		}
		protected void FillActionItems(List<ChoiceActionItem> items, Dictionary<Type, ChoiceActionItem> dictionaryTypes, List<Type> descendantTypes) {
			foreach(Type type in descendantTypes) {
				ChoiceActionItem itemInfo = null;
				dictionaryTypes.TryGetValue(type, out itemInfo);
				if(itemInfo != null) {
					items.Add(itemInfo);
				}
				else {
					items.Add(CreateItem(type, null));
				}
			}
		}
		protected bool CanCreateAndEdit(Type type, out string diagnosticInfo) {
			CollectionSourceBase collectionSource = (View is ListView) ? ((ListView)View).CollectionSource : LinkToListViewController.FindCollectionSource(Frame);
			Boolean canCreate = DataManipulationRight.CanCreate(View, type, collectionSource, out diagnosticInfo);
			Boolean detailViewExists = !string.IsNullOrEmpty(Application.FindDetailViewId(type));
			diagnosticInfo += ", DetailViewExists: " + detailViewExists.ToString();
			return canCreate && detailViewExists;
		}
		protected IList<ChoiceActionItem> CollectItems(List<ChoiceActionItem> choiceActionItems, Dictionary<Type, ChoiceActionItem> typeAndActionItems, EventHandler<CollectTypesEventArgs> eventHandler) {
			SortChoiceActionItems(choiceActionItems);
			List<Type> itemTypes = new List<Type>();
			foreach(ChoiceActionItem choiceActionItem in choiceActionItems) {
				itemTypes.Add(choiceActionItem.Data as Type);
			}
			if(eventHandler != null) {
				eventHandler(this, new CollectTypesEventArgs(itemTypes));
			}
			IList<ChoiceActionItem> result = new List<ChoiceActionItem>();
			foreach(Type type in itemTypes) {
				ChoiceActionItem itemInfo = null;
				typeAndActionItems.TryGetValue(type, out itemInfo);
				if(itemInfo != null) {
					result.Add(itemInfo);
				}
				else {
					result.Add(CreateItem(type, null));
				}
			}
			return result;
		}
		protected IList<ChoiceActionItem> CollectRootObjectItems() {
			List<ChoiceActionItem> result = new List<ChoiceActionItem>();
			Dictionary<Type, ChoiceActionItem> dictionaryTypes = new Dictionary<Type, ChoiceActionItem>();
			IModelApplicationCreatableItems modelCreatableItems = Application.Model as IModelApplicationCreatableItems;
			if(modelCreatableItems != null && modelCreatableItems.CreatableItems != null) {
				CollectionSourceBase collectionSource = LinkToListViewController.FindCollectionSource(Frame);
				foreach(IModelCreatableItem modelCreatableItem in modelCreatableItems.CreatableItems) {
					ITypeInfo typeInfo = modelCreatableItem.ModelClass == null ? null : modelCreatableItem.ModelClass.TypeInfo;
					if(typeInfo != null && !((ObjectView)View).ObjectTypeInfo.IsAssignableFrom(typeInfo) && !String.IsNullOrEmpty(Application.GetDetailViewId(typeInfo.Type))) {
						bool isCreationGranted = false;
						if(SecuritySystem.Instance is IRequestSecurity) {
							isCreationGranted = DataManipulationRight.HasPermissionTo(typeInfo.Type, null, null, View.ObjectSpace, SecurityOperations.Create);
						}
						else {
							isCreationGranted = DataManipulationRight.HasPermissionTo(typeInfo.Type, ObjectAccess.Create, null, null, collectionSource);
						}
						if(DataManipulationRight.CanInstantiate(typeInfo.Type, ObjectSpace)
							&& isCreationGranted) {
							ChoiceActionItem actionItem = CreateItem(typeInfo.Type, modelCreatableItem);
							dictionaryTypes.Add(typeInfo.Type, actionItem);
							result.Add(actionItem);
						}
					}
				}
			}
			return CollectItems(result, dictionaryTypes, CollectCreatableItemTypes);
		}
		protected IList<ChoiceActionItem> CollectDescendantItems() {
			List<ChoiceActionItem> items = new List<ChoiceActionItem>();
			Dictionary<Type, ChoiceActionItem> dictionaryTypes = new Dictionary<Type, ChoiceActionItem>();
			System.Text.StringBuilder diagnosticInfo = new System.Text.StringBuilder(newObjectAction.DiagnosticInfo);
			foreach(ITypeInfo typeInfo in currentViewTypes) {
				string info = string.Empty;
				if(CanCreateAndEdit(typeInfo.Type, out info)) {
					ChoiceActionItem actionItem = CreateItem(typeInfo.Type, null);
					dictionaryTypes.Add(typeInfo.Type, actionItem);
					items.Add(actionItem);
				}
				diagnosticInfo.AppendLine().AppendFormat("{0}: {1}", typeInfo.Name, info);
			}
			newObjectAction.DiagnosticInfo = diagnosticInfo.ToString();
			return CollectItems(items, dictionaryTypes, CollectDescendantTypes);
		}
		protected virtual void OnObjectCreating(CollectionSourceBase currentCollectionSource, ObjectCreatingEventArgs queryNewObjectArgs) {
			queryNewObjectArgs.ShowDetailView = Application.ShowDetailViewFrom(Frame);
			if(ObjectCreating != null) {
				ObjectCreating(this, queryNewObjectArgs);
			}
		}
		protected virtual void OnObjectCreated(Object newObject, IObjectSpace objectSpace) {
			if(ObjectCreated != null) {
				ObjectCreated(this, new ObjectCreatedEventArgs(newObject, objectSpace));
			}
		}
		protected virtual void OnCustomAddObjectToCollection(ProcessNewObjectEventArgs args) {
			if(CustomAddObjectToCollection != null) {
				CustomAddObjectToCollection(this, args);
			}
		}
		protected CollectionSourceBase GetCurrentCollectionSource() {
			CollectionSourceBase result = null;
			if(View is ListView) {
				result = ((ListView)View).CollectionSource;
				propertyCollectionSourceLink = null;
			}
			else {
				LinkToListViewController linkToListViewController = Frame.GetController<LinkToListViewController>();
				bool hasLink = (linkToListViewController != null) && (linkToListViewController.Link != null);
				if(hasLink) {
					if(linkToListViewController.Link.ListView != null) {
						result = linkToListViewController.Link.ListView.CollectionSource;
					}
					propertyCollectionSourceLink = linkToListViewController.Link.PropertyCollectionSourceLink;
				}
			}
			if(result == null) {
				if(propertyCollectionSourceLink != null) {
					result = propertyCollectionSourceLink.GetPropertyCollectionSource(Application, ObjectSpace); 
				}
			}
			return result;
		}
		protected ListView ListView {
			get { return View as ListView; }
		}
		protected virtual bool IsViewShownFromNestedObjectSpace(Frame sourceFrame) {
			return false;
		}
		protected virtual IObjectSpace CreateObjectSpace(Type objectType) {
			IObjectSpace objectSpaceForNewObject = null;
			if(Application.ShowDetailViewFrom(Frame) || (View.ObjectSpace.Owner == View)) {
				objectSpaceForNewObject = Application.GetObjectSpaceToShowDetailViewFrom(Frame, objectType);
			}
			else {
				objectSpaceForNewObject = View.ObjectSpace;
			}
			return objectSpaceForNewObject;
		}
		protected virtual void New(SingleChoiceActionExecuteEventArgs args) {
			Type newObjectType;
			if(args.SelectedChoiceActionItem == null) {
				newObjectType = ((ObjectView)View).ObjectTypeInfo.Type;
			}
			else {
				newObjectType = (Type)args.SelectedChoiceActionItem.Data;
			}
			IObjectSpace objectSpaceForNewObject = CreateObjectSpace(newObjectType);
			SetLinkNewObjectToParentImmediatelyToPropertyCollectionSourceLink();
			ObjectCreatingEventArgs objectCreatingArgs = new ObjectCreatingEventArgs(objectSpaceForNewObject, newObjectType);
			CollectionSourceBase currentCollectionSource = GetCurrentCollectionSource();
			OnObjectCreating(currentCollectionSource, objectCreatingArgs);
			if(!objectCreatingArgs.Cancel) {
				CommitMasterObject(currentCollectionSource, objectCreatingArgs.ObjectSpace);
				if(objectCreatingArgs.NewObject == null) {
					objectCreatingArgs.NewObject = objectCreatingArgs.ObjectSpace.CreateObject(newObjectType);
				}
				ProcessNewObjectEventArgs processNewObjectArgs = new ProcessNewObjectEventArgs(objectCreatingArgs.NewObject, objectCreatingArgs.ObjectSpace, currentCollectionSource);
				OnCustomAddObjectToCollection(processNewObjectArgs);
				if(!processNewObjectArgs.Handled) {
					AddNewObjectToCollectionSource(currentCollectionSource, objectCreatingArgs.NewObject, objectCreatingArgs.ObjectSpace);
				}
				OnObjectCreated(objectCreatingArgs.NewObject, objectCreatingArgs.ObjectSpace);
				if(objectCreatingArgs.ShowDetailView) {
					args.ShowViewParameters.CreatedView =
						Application.CreateDetailView(objectCreatingArgs.ObjectSpace, objectCreatingArgs.NewObject, View);
				}
				else {
					if(View is DetailView) {
						args.ShowViewParameters.CreatedView =
							Application.CreateDetailView(objectCreatingArgs.ObjectSpace, objectCreatingArgs.NewObject, View);
						args.ShowViewParameters.TargetWindow = TargetWindow.Current;
					}
				}
			}
		}
		protected virtual Boolean NeedToUpdateNewObjectAction() {
			return linkNewObjectToParentImmediately;
		}
		protected virtual void UpdateActionState() {
			UpdateOrderedIndexedCreatableItemTypes();
			if(ListView != null && (ListView.CollectionSource is PropertyCollectionSource)) {
				UpdateIsManyToManyKey(NeedToUpdateNewObjectAction() ? View : null, newObjectAction);
			}
			bool isNestedDetailView = (View is DetailView) && !View.IsRoot;
			newObjectAction.Active.SetItemValue("IsRootDetailView", !isNestedDetailView);
			newObjectAction.Active.SetItemValue("AllowNew", View.AllowNew);
			string allowAddDiagnosticInfo;
			newObjectAction.Active.SetItemValue("ListAllowAdd", DataManipulationRight.IsAddToCollectionAllowed(View, ((ObjectView)View).ObjectTypeInfo.Type, out allowAddDiagnosticInfo));
			bool isGranted = true;
			PropertyCollectionSource propertyCollectionSource = GetPropertyCollectionSource();
			if(propertyCollectionSource != null) {
				if(!DataManipulationRight.CanRead(View.ObjectTypeInfo.Type, "", null, propertyCollectionSource, View.ObjectSpace)) {
					isGranted = false;
					allowAddDiagnosticInfo += Environment.NewLine + String.Format("No access to read {0}", View.ObjectTypeInfo);
				}
				if(isGranted && !DataManipulationRight.CanEdit(propertyCollectionSource.MemberInfo.Owner.Type, propertyCollectionSource.MemberInfo.Name, propertyCollectionSource.MasterObject, null, propertyCollectionSource.ObjectSpace)) {
					isGranted = false;
					allowAddDiagnosticInfo += Environment.NewLine + String.Format("No access to modify the {0} member of the {1}", propertyCollectionSource.MemberInfo.Name, propertyCollectionSource.MemberInfo.Owner.Type);
				}
				if(isGranted) {
					IMemberInfo associatedMember = propertyCollectionSource.MemberInfo.AssociatedMemberInfo;
					if(associatedMember != null) {
						ITypeInfo currentObjectTypeInfo = null;
						if(View.CurrentObject != null) {
							currentObjectTypeInfo = ObjectSpace.TypesInfo.FindTypeInfo(View.CurrentObject.GetType());
							IMemberInfo associatedMemberInfo = currentObjectTypeInfo.FindMember(associatedMember.Name);
							if((associatedMemberInfo != null)
									&& !DataManipulationRight.CanEdit(associatedMember.Owner.Type, associatedMember.Name, View.CurrentObject, propertyCollectionSource, View.ObjectSpace)) {
								isGranted = false;
								allowAddDiagnosticInfo += Environment.NewLine + String.Format("No access to modify the {0} member of the {1}", associatedMember.Name, associatedMember.Owner.Type);
							}
						}
					}
				}
				newObjectAction.Active.SetItemValue(SecurityAllowNewByPermissions, isGranted);
			}
			newObjectAction.DiagnosticInfo = allowAddDiagnosticInfo.ToString();
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if(Application != null) {
				linkNewObjectToParentImmediately = Application.LinkNewObjectToParentImmediately;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			currentViewTypes = new List<ITypeInfo>(ReflectionHelper.FindTypeDescendants(((ObjectView)View).ObjectTypeInfo));
			currentViewTypes.Insert(0, ((ObjectView)View).ObjectTypeInfo);
			View.AllowNewChanged += new EventHandler(View_AllowNewChanged);
			if(ListView != null) {
				ListView.CollectionSource.CollectionChanged += new EventHandler(CollectionSource_CollectionChanged);
				if(ListView.CollectionSource.IsLoaded) {
					UpdateActionState();
				}
				NewObjectAction.Enabled.RemoveItem("MasterObjectIsNew");
				LookupEditPropertyCollectionSource lookupCollectionSource = ListView.CollectionSource as LookupEditPropertyCollectionSource;
				if((lookupCollectionSource != null)
					&& (lookupCollectionSource.LookupMode == LookupEditCollectionSourceMode.Lookup)
					&& lookupCollectionSource.MemberInfo.IsAssociation
					&& ObjectSpace.IsNewObject(GetCollectionOwner(lookupCollectionSource.MasterObject, lookupCollectionSource.MemberInfo))
					&& !IsViewShownFromNestedObjectSpace(Frame)) {
					NewObjectAction.Enabled.SetItemValue("MasterObjectIsNew", false);
					ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
				}
			}
			else {
				UpdateActionState();
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.AllowNewChanged -= new EventHandler(View_AllowNewChanged);
			if(ListView != null) {
				ListView.CollectionSource.CollectionChanged -= new EventHandler(CollectionSource_CollectionChanged);
				ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
			}
		}
		public NewObjectViewController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
			TypeOfView = typeof(ObjectView);
			orderedIndexedCreatableItemTypes = new List<Type>();
			choiceActionItemCaptions = new Dictionary<ChoiceActionItem, String>();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Object CreateObject() {
			ObjectCreatingEventArgs objectCreatingArgs = new ObjectCreatingEventArgs(View.ObjectSpace, ((ObjectView)View).ObjectTypeInfo.Type);
			CollectionSourceBase currentCollectionSource = GetCurrentCollectionSource();
			OnObjectCreating(currentCollectionSource, objectCreatingArgs);
			if(!objectCreatingArgs.Cancel) {
				CommitMasterObject(currentCollectionSource, objectCreatingArgs.ObjectSpace);
				if(objectCreatingArgs.NewObject == null) {
					objectCreatingArgs.NewObject = objectCreatingArgs.ObjectSpace.CreateObject(((ObjectView)View).ObjectTypeInfo.Type);
				}
				ProcessNewObjectEventArgs processNewObjectArgs = new ProcessNewObjectEventArgs(objectCreatingArgs.NewObject, objectCreatingArgs.ObjectSpace, currentCollectionSource);
				OnCustomAddObjectToCollection(processNewObjectArgs);
				if(!processNewObjectArgs.Handled) {
					AddNewObjectToCollectionSource(currentCollectionSource, objectCreatingArgs.NewObject, objectCreatingArgs.ObjectSpace);
				}
			}
			return objectCreatingArgs.NewObject;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseObjectCreated(Object obj) {
			OnObjectCreated(((ObjectView)View).CurrentObject, ObjectSpace);
		}
		public Boolean LinkNewObjectToParentImmediately {
			get { return linkNewObjectToParentImmediately; }
			set {
				linkNewObjectToParentImmediately = value;
				UpdateActionState();
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("NewObjectViewControllerNewObjectAction")]
#endif
		public SingleChoiceAction NewObjectAction { get { return newObjectAction; } }
		public event EventHandler<ProcessNewObjectEventArgs> CustomAddObjectToCollection;
		public event EventHandler<CollectTypesEventArgs> CollectDescendantTypes;
		public event EventHandler<CollectTypesEventArgs> CollectCreatableItemTypes;
		public event EventHandler<ObjectCreatedEventArgs> ObjectCreated;
		public event EventHandler<ObjectCreatingEventArgs> ObjectCreating;
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelApplication, IModelApplicationCreatableItems>();
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void UpdateIsManyToManyKey(View view, ActionBase action) {
			String activeKey = "IsAggregatedOrOneToMany";
			if((view is ListView) && !((ListView)view).IsRoot && (((ListView)view).CollectionSource is PropertyCollectionSource)) {
				IMemberInfo member = (((ListView)view).CollectionSource as PropertyCollectionSource).MemberInfo;
				action.Active.SetItemValue(activeKey, member.IsAggregated || !member.IsManyToMany);
			}
			else {
				action.Active.RemoveItem(activeKey);
			}
		}
	}
	public class CollectTypesEventArgs : EventArgs {
		private ICollection<Type> types;
		public CollectTypesEventArgs(ICollection<Type> types) {
			this.types = types;
		}
		public ICollection<Type> Types {
			get { return types; }
		}
	}
	public class ObjectCreatingEventArgs : EventArgs {
		private Type objectType;
		private object newObject;
		private bool showDetailView;
		private bool cancel;
		private IObjectSpace objectSpace;
		public ObjectCreatingEventArgs(IObjectSpace objectSpace, Type objectType) {
			this.objectSpace = objectSpace;
			this.objectType = objectType;
			this.showDetailView = true;
		}
		public Type ObjectType {
			get { return objectType; }
		}
		public object NewObject {
			get { return newObject; }
			set { newObject = value; }
		}
		public bool ShowDetailView {
			get { return showDetailView; }
			set { showDetailView = value; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public class ObjectCreatedEventArgs : EventArgs {
		private Object createdObject;
		private IObjectSpace objectSpace;
		public ObjectCreatedEventArgs(Object createdObject, IObjectSpace objectSpace) {
			this.createdObject = createdObject;
			this.objectSpace = objectSpace;
		}
		public Object CreatedObject {
			get { return createdObject; }
			set { createdObject = value; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
	}
	public class ProcessNewObjectEventArgs : HandledEventArgs {
		private Object newObject;
		private IObjectSpace objectSpace;
		private CollectionSourceBase currentCollectionSource;
		public ProcessNewObjectEventArgs(Object newObject, IObjectSpace objectSpace, CollectionSourceBase currentCollectionSource)
			: base(false) {
			this.newObject = newObject;
			this.objectSpace = objectSpace;
			this.currentCollectionSource = currentCollectionSource;
		}
		public Object NewObject {
			get { return newObject; }
			set { newObject = value; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
		public CollectionSourceBase CurrentCollectionSource {
			get { return currentCollectionSource; }
		}
	}
}
