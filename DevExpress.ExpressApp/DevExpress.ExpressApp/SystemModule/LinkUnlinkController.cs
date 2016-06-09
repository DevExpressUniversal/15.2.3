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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	public class LinkNewObjectController : DialogController {
		private LinkDialogController owner;
		protected override void Accept(SimpleActionExecuteEventArgs args) {
			Window.View.ObjectSpace.CommitChanges();
			owner.SetObjectFilter(Window.View.CurrentObject);
			Window.Close(true);
		}
		protected override void Cancel(SimpleActionExecuteEventArgs args) {
			Window.Close(false);
		}
		public LinkNewObjectController() { }
		public LinkNewObjectController(LinkDialogController owner) {
			this.owner = owner;
		}
		public LinkDialogController Owner {
			get { return owner; }
			set { owner = value; }
		}
	}
	public class LinkDialogController : DialogController {
		private LookupEditorHelper helper;
		private bool canFilterDataSource;
		private void NewObjectAction_Executed(Object sender, ActionBaseEventArgs e) {
			e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
			LinkNewObjectController linkNewObjectController = Application.CreateController<LinkNewObjectController>();
			linkNewObjectController.Owner = this;
			e.ShowViewParameters.Controllers.Add(linkNewObjectController);
		}
		protected override void OnDeactivated() {
			NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
			if(newObjectViewController != null) {
				newObjectViewController.NewObjectAction.Executed -= new EventHandler<ActionBaseEventArgs>(NewObjectAction_Executed);
			}
			base.OnDeactivated();
		}
		protected override void OnActivated() {
			base.OnActivated();
			NewObjectViewController newController = Window.GetController<NewObjectViewController>();
			if(newController != null) {
				newController.NewObjectAction.Executed += new EventHandler<ActionBaseEventArgs>(NewObjectAction_Executed);
			}
			AcceptAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
		}
		public LinkDialogController() {
			SaveOnAccept = false;
		}
		public void Initialize(LookupEditorHelper helper, Boolean canFilterDataSource) {
			this.helper = helper;
			this.canFilterDataSource = canFilterDataSource;
		}
		public void SetObjectFilter(Object obj) {
			if(canFilterDataSource) {
				FilterController filterController = Frame.GetController<FilterController>();
				if(filterController != null) {
					filterController.FullTextFilterAction.DoExecute(helper.GetDisplayText(obj, "", null));
				}
			}
			else {
				Frame.View.CurrentObject = Frame.View.ObjectSpace.GetObject(obj);
			}
		}
	}
	public class LinkUnlinkController : ViewController, IModelExtender {
		public const string CriteriaKeyForLinkView = "ExcludeAlreadyLinkedObjects";
		private const bool RequirePersistentTypeDefaultValue = true;
		private const bool RequireLookupListViewDefaultValue = true;
		private PopupWindowShowAction linkAction;
		private SimpleAction unlinkAction;
		private IModelMemberViewItem listPropertyEditorInfo;
		private LookupEditorHelper lookupEditorHelper;
		private IContainer components;
		private ListView linkListView;
		private Boolean? canFilterLinkView;
		private PropertyCollectionSource PropertyCollectionSource {
			get { return (View != null) ? (View.CollectionSource as PropertyCollectionSource) : null; }
		}
		private Object MasterObject {
			get { return (PropertyCollectionSource != null) ? PropertyCollectionSource.MasterObject : null; }
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.unlinkAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
			this.linkAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
			this.unlinkAction.Caption = "Unlink";
			this.unlinkAction.Category = "Link";
			this.unlinkAction.ConfirmationMessage = "You are about to unlink the selected record(s). Do you want to proceed?";
			this.unlinkAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
			this.unlinkAction.Id = "Unlink";
			this.unlinkAction.ImageName = "MenuBar_Unlink";
			this.unlinkAction.Application = null;
			this.unlinkAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.unlinkAction_OnExecute);
			this.linkAction.Caption = "Link";
			this.linkAction.Category = "Link";
			this.linkAction.ConfirmationMessage = "";
			this.linkAction.Id = "Link";
			this.linkAction.ImageName = "MenuBar_Link";
			this.linkAction.Application = null;
			this.linkAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.linkAction_OnExecute);
			this.linkAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.linkAction_OnCustomizePopupWindow);
			this.linkAction.CustomizeTemplate += new EventHandler<CustomizeTemplateEventArgs>(this.linkAction_OnCustomizeTemplate);
			this.TargetViewNesting = DevExpress.ExpressApp.Nesting.Nested;
			this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
		}
		private void DataSource_CollectionLoaded(Object sender, EventArgs e) {
			UpdateActionsState();
		}
		private void linkAction_OnExecute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
			Link(args);
		}
		private void linkAction_OnCustomizeTemplate(Object sender, CustomizeTemplateEventArgs args) {
			CustomizeLinkTemplate(args.Template);
		}
		protected virtual void CustomizeLinkTemplate(IFrameTemplate template) {
			if(template != null) {
				((ILookupPopupFrameTemplate)template).IsSearchEnabled = CanFilterLinkView();
			}
		}
		private void linkAction_OnCustomizePopupWindow(Object sender, CustomizePopupWindowParamsEventArgs args) {
			args.View = CreateLinkView();
			linkListView = args.View as ListView;
			canFilterLinkView = null;
			LinkDialogController dialogController = Application.CreateController<LinkDialogController>();
			args.DialogController = dialogController;
			if (lookupEditorHelper != null) {
				dialogController.Initialize(lookupEditorHelper, CanFilterLinkView());
			}
		}
		private void unlinkAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			Unlink(e);
		}
		private void View_CurrentObjectChanged(Object sender, EventArgs e) {
			UpdateActionsState();
		}
		private void View_AllowLinkOrUnlinkChanged(object sender, EventArgs e) {
			UpdateActionsState();
		}
		private CriteriaOperator GetExcludeLinkedObjectsCriteria() {
			CriteriaOperator result = null;
			IMemberInfo collectionMemberInfo = ((PropertyCollectionSource)View.CollectionSource).MemberInfo;
			CriteriaOperator linkedObjectsCriteria = View.ObjectSpace.GetAssociatedCollectionCriteria(MasterObject, collectionMemberInfo);
			if(!ReferenceEquals(linkedObjectsCriteria, null)) {
				if(!collectionMemberInfo.IsManyToMany) {
					result = new GroupOperator(GroupOperatorType.Or,
						new NullOperator(collectionMemberInfo.AssociatedMemberInfo.Name),
						new NotOperator(linkedObjectsCriteria)
					);
				}
				else {
					result = new NotOperator(linkedObjectsCriteria);
				}
			}
			return result;
		}
		private ListView CreateLinkViewCore() {
			ListView result = null;
			if((lookupEditorHelper != null) && (MasterObject != null)) {
				result = lookupEditorHelper.CreateListView(MasterObject);
				if(result.CollectionSource is LookupEditPropertyCollectionSource) {
					LookupEditPropertyCollectionSource lookupEditPropertyCollectionSource = (LookupEditPropertyCollectionSource)result.CollectionSource;
					lookupEditPropertyCollectionSource.LookupMode = LookupEditCollectionSourceMode.Link;
				}
			}
			else {
				String listViewID = Application.FindLookupListViewId(View.ObjectTypeInfo.Type);
				CollectionSourceBase collectionSource = Application.CreateCollectionSource(ObjectSpace, View.ObjectTypeInfo.Type, listViewID);
				result = Application.CreateListView(listViewID, collectionSource, false);
			}
			if(ExcludeLinkedObjects && (MasterObject != null)) {
				CriteriaOperator excludeLinkedObjectsCriteria = GetExcludeLinkedObjectsCriteria();
				if(!ReferenceEquals(excludeLinkedObjectsCriteria, null)) {
					result.CollectionSource.Criteria[CriteriaKeyForLinkView] = excludeLinkedObjectsCriteria;
				}
			}
			return result;
		}
		private void LinkObjectsCore(IList selectedObjects) {
			IObjectSpace objectSpace = ObjectSpace;
			CollectionSourceBase collectionSource = View.CollectionSource;
			foreach(Object selectedObject in selectedObjects) {
				Object obj = objectSpace.GetObject(selectedObject);
				collectionSource.Add(obj);
			}
			Object masterObject = MasterObject;
			if(masterObject != null) {
				objectSpace.SetModified(masterObject);
			}
		}
		private void UnlinkObjectsCore(IList selectedObjects) {
			IObjectSpace objectSpace = ObjectSpace;
			CollectionSourceBase collectionSource = View.CollectionSource;
			foreach(Object selectedObject in selectedObjects) {
				Object obj = objectSpace.GetObject(selectedObject);
				collectionSource.Remove(obj);
			}
			Object masterObject = MasterObject;
			if(masterObject != null) {
				objectSpace.SetModified(masterObject);
			}
		}
		protected bool CanFilterLinkView() {
			if((linkListView != null) && (lookupEditorHelper != null) && !canFilterLinkView.HasValue) {
				canFilterLinkView = lookupEditorHelper.CanFilterDataSource(linkListView.CollectionSource, MasterObject);
			}
			return canFilterLinkView.HasValue ? canFilterLinkView.Value : false;
		}
		protected internal void SetListPropertyEditorInfo(IModelMemberViewItem listPropertyEditorInfo) {
			this.listPropertyEditorInfo = listPropertyEditorInfo;
		}
		protected virtual void LinkObjects(IList selectedObjects) {
			if(selectedObjects == null) {
				return;
			}
			if(AutoCommit) {
				CollectionSourceBase collectionSource = View.CollectionSource;
				ArrayList linkedObjects = new ArrayList();
				if(collectionSource.DataAccessMode == CollectionSourceDataAccessMode.Client) {
					linkedObjects.AddRange(collectionSource.List);
				}
				IList selectedObjectsCashe = new ArrayList(selectedObjects);
				LinkObjectsCore(selectedObjectsCashe);
				try {
					ObjectSpace.CommitChanges();
				}
				catch {
					if(collectionSource.DataAccessMode == CollectionSourceDataAccessMode.Client) {
						foreach(Object selectedObject in selectedObjectsCashe) {
							Object obj = ObjectSpace.GetObject(selectedObject);
							if(linkedObjects.IndexOf(obj) < 0) {
								collectionSource.Remove(obj);
							}
						}
					}
					throw;
				}
				finally {
					if(collectionSource.DataAccessMode != CollectionSourceDataAccessMode.Client) {
						collectionSource.Reload();
					}
				}
			}
			else {
				LinkObjectsCore(selectedObjects);
			}
		}
		protected virtual void UnlinkObjects(IList selectedObjects) {
			if(AutoCommit) {
				CollectionSourceBase collectionSource = View.CollectionSource;
				ArrayList linkedObjects = new ArrayList();
				if(collectionSource.DataAccessMode == CollectionSourceDataAccessMode.Client) {
					linkedObjects.AddRange(collectionSource.List);
				}
				IList selectedObjectsCashe = new ArrayList(selectedObjects);
				UnlinkObjectsCore(selectedObjectsCashe);
				try {
					ObjectSpace.CommitChanges();
				}
				catch {
					if(collectionSource.DataAccessMode == CollectionSourceDataAccessMode.Client) {
						foreach(Object selectedObject in selectedObjectsCashe) {
							if(linkedObjects.IndexOf(selectedObject) >= 0) {
								collectionSource.Add(selectedObject);
							}
						}
					}
					throw;
				}
				finally {
					if(collectionSource.DataAccessMode != CollectionSourceDataAccessMode.Client) {
						collectionSource.Reload();
					}
				}
			}
			else {
				UnlinkObjectsCore(selectedObjects);
			}
		}
		protected virtual View CreateLinkView() {
			CustomCreateLinkViewEventArgs customArgs = new CustomCreateLinkViewEventArgs(View);
			OnCustomCreateLinkView(customArgs); 
			if(customArgs.Handled && (customArgs.LinkView != null)) {
				if(lookupEditorHelper != null) {
					lookupEditorHelper.LookupObjectTypeInfo = customArgs.LinkView.ObjectTypeInfo;
				}
			}
			else {
				return CreateLinkViewCore();
			}
			return customArgs.LinkView;
		}
		protected virtual void OnCustomCreateLinkView(CustomCreateLinkViewEventArgs customArgs) {
			if(CustomCreateLinkView != null) {
				CustomCreateLinkView(this, customArgs);
			}
		}
		protected virtual void Link(PopupWindowShowActionExecuteEventArgs args) {
			QueryLinkObjectsEventArgs customArgs = new QueryLinkObjectsEventArgs(args.PopupWindow);
			OnQueryLinkObjects(customArgs);
			IList linkObjects = customArgs.LinkObjects;
			if(!customArgs.Handled && (linkListView != null)) {
				linkObjects = linkListView.SelectedObjects;
			}
			LinkObjects(linkObjects);
		}
		protected virtual void OnQueryLinkObjects(QueryLinkObjectsEventArgs customArgs) {
			if(QueryLinkObjects != null) {
				QueryLinkObjects(this, customArgs);
			}
		}
		protected virtual void Unlink(SimpleActionExecuteEventArgs args) {
			UnlinkObjects(args.SelectedObjects);
		}
		protected virtual void UpdateActionsState() {
			if(View != null) {
				linkAction.BeginUpdate();
				unlinkAction.BeginUpdate();
				try {
					System.Text.StringBuilder diagnosticInfo = new System.Text.StringBuilder();
					if(RequirePersistentType) {
						linkAction.Active.SetItemValue("IsPersistent", View.ObjectTypeInfo.IsPersistent);
					}
					linkAction.Active.SetItemValue("IsPropertyCollectionSource", PropertyCollectionSource != null);
					string addToCollectionAllowedInfo;
					linkAction.Active.SetItemValue("ListAllowAdd", DataManipulationRight.IsAddToCollectionAllowed(View, View.ObjectTypeInfo.Type, out addToCollectionAllowedInfo));
					diagnosticInfo.AppendLine("ListAllowAdd: " + addToCollectionAllowedInfo);
					if(Application != null && RequireLookupListView) {
						linkAction.Active.SetItemValue("LookupListInfoPresent", Application.FindLookupListViewId(View.ObjectTypeInfo.Type) != null);
					}
					linkAction.Active.SetItemValue("ViewAllowLink", View.AllowLink); ;
					if(RequirePersistentType) {
						unlinkAction.Active.SetItemValue("IsPersistent", View.ObjectTypeInfo.IsPersistent);
					}
					unlinkAction.Active.SetItemValue("IsPropertyCollectionSource", PropertyCollectionSource != null);
					string removeFromCollectionAllowedInfo;
					unlinkAction.Active.SetItemValue("ListAllowDelete", DataManipulationRight.IsRemoveFromCollectionAllowed(View, out removeFromCollectionAllowedInfo));
					diagnosticInfo.AppendLine("ListAllowDelete: " + removeFromCollectionAllowedInfo);
					unlinkAction.Active.SetItemValue("ViewAllowUnlink", View.AllowUnlink);
					if(!(SecuritySystem.Instance is DevExpress.ExpressApp.Security.IRequestSecurity)) {
						if(View.AllowEdit.Contains(DevExpress.ExpressApp.View.SecurityReadOnlyItemName)) {
							linkAction.Active.SetItemValue("ViewAllowEditBySecurity", View.AllowEdit[DevExpress.ExpressApp.View.SecurityReadOnlyItemName]);
							unlinkAction.Active.SetItemValue("ViewAllowEditBySecurity", View.AllowEdit[DevExpress.ExpressApp.View.SecurityReadOnlyItemName]);
						}
					}
					if(PropertyCollectionSource != null && (View.ObjectTypeInfo != null)) {
						bool isGranted = true;
						if(!DataManipulationRight.CanRead(View.ObjectTypeInfo.Type, "", null, null, View.ObjectSpace)) {
							isGranted = false;
							diagnosticInfo.AppendLine("No access to read " + View.ObjectTypeInfo.ToString());
						}
						if(isGranted) {
							bool canEditCollectionMember = DataManipulationRight.CanEdit(
								PropertyCollectionSource.MemberInfo.Owner.Type, PropertyCollectionSource.MemberInfo.Name, 
								PropertyCollectionSource.MasterObject, null, PropertyCollectionSource.ObjectSpace);
							if(!canEditCollectionMember) {
								isGranted = false;
								diagnosticInfo.AppendLine(string.Format("No access to modify the {0} member of the {1}", PropertyCollectionSource.MemberInfo.Name, PropertyCollectionSource.MemberInfo.Owner.Type));
							}
						}
						if(isGranted && !(SecuritySystem.Instance is DevExpress.ExpressApp.Security.IRequestSecurity)) {
							IMemberInfo associatedMember = PropertyCollectionSource.MemberInfo.AssociatedMemberInfo;
							if(associatedMember != null) {
								ITypeInfo currentObjectTypeInfo = null;
								if(View.CurrentObject != null) {
									currentObjectTypeInfo = ObjectSpace.TypesInfo.FindTypeInfo(View.CurrentObject.GetType());
									IMemberInfo associatedMemberInfo = currentObjectTypeInfo.FindMember(associatedMember.Name);
									if((associatedMemberInfo != null)
											&& !DataManipulationRight.CanEdit(associatedMember.Owner.Type, associatedMember.Name, View.CurrentObject, PropertyCollectionSource, View.ObjectSpace)) {
										isGranted = false;
										diagnosticInfo.AppendLine(string.Format("No access to modify the {0} member of the {1}", associatedMember.Name, associatedMember.Owner.Type));
									}
								}
							}
						}
						linkAction.Active.SetItemValue("Security", isGranted);
						linkAction.DiagnosticInfo += "ListAllowAdd: " + addToCollectionAllowedInfo;
						linkAction.DiagnosticInfo += diagnosticInfo.ToString();
						unlinkAction.Active.SetItemValue("Security", isGranted);
						unlinkAction.DiagnosticInfo += diagnosticInfo.ToString();
					}
					unlinkAction.Active.RemoveItem("IsAggregatedOrManyToManyCollectionProperty");
					linkAction.Active.RemoveItem("IsAggregatedOrManyToManyCollectionProperty");
					if(PropertyCollectionSource != null) {
						if(!PropertyCollectionSource.MemberInfo.IsManyToMany && PropertyCollectionSource.MemberInfo.IsAggregated) {
							unlinkAction.Active.SetItemValue("IsAggregatedOrManyToManyCollectionProperty", false);
							linkAction.Active.SetItemValue("IsAggregatedOrManyToManyCollectionProperty", false);
						}
					}
				}
				finally {
					linkAction.EndUpdate();
					unlinkAction.EndUpdate();
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(listPropertyEditorInfo != null) {
				lookupEditorHelper = new LookupEditorHelper(Application, ObjectSpace, View.ObjectTypeInfo, listPropertyEditorInfo);
				lookupEditorHelper.LookupView = ((IModelPropertyEditorLinkView)listPropertyEditorInfo).LinkView;
			}
			View.AllowLinkChanged += new EventHandler(View_AllowLinkOrUnlinkChanged);
			View.AllowUnlinkChanged += new EventHandler(View_AllowLinkOrUnlinkChanged);
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			View.CollectionSource.CollectionChanged += new EventHandler(DataSource_CollectionLoaded);
			UpdateActionsState();
		}
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			Active.SetItemValue("PropertyCollectionSource",
				(view is ListView)
				&&
				(((ListView)view).CollectionSource is PropertyCollectionSource));
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.AllowLinkChanged -= new EventHandler(View_AllowLinkOrUnlinkChanged);
			View.AllowUnlinkChanged -= new EventHandler(View_AllowLinkOrUnlinkChanged);
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			View.CollectionSource.CollectionChanged -= new EventHandler(DataSource_CollectionLoaded);
			lookupEditorHelper = null;
		}
		protected LookupEditorHelper LookUpEditorHelper {
			get { return lookupEditorHelper; }
		}
		protected new ListView View {
			get { return base.View as ListView; }
		}
		public LinkUnlinkController() {
			InitializeComponent();
			RegisterActions(components);
			AutoCommit = false;
			ExcludeLinkedObjects = false;
			RequirePersistentType = RequirePersistentTypeDefaultValue;
			RequireLookupListView = RequireLookupListViewDefaultValue;
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("LinkUnlinkControllerAutoCommit")]
#endif
		public bool AutoCommit { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("LinkUnlinkControllerExcludeLinkedObjects")]
#endif
		public bool ExcludeLinkedObjects { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("LinkUnlinkControllerUnlinkAction")]
#endif
		public SimpleAction UnlinkAction {
			get { return unlinkAction; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("LinkUnlinkControllerLinkAction")]
#endif
		public PopupWindowShowAction LinkAction {
			get { return linkAction; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(RequirePersistentTypeDefaultValue)]
		public bool RequirePersistentType { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(RequireLookupListViewDefaultValue)]
		public bool RequireLookupListView { get; set; }		
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelPropertyEditor, IModelPropertyEditorLinkView>();
			extenders.Add<IModelColumn, IModelPropertyEditorLinkView>();
		}
		#endregion
		public event EventHandler<CustomCreateLinkViewEventArgs> CustomCreateLinkView;
		public event EventHandler<QueryLinkObjectsEventArgs> QueryLinkObjects;
	}
	[DomainLogic(typeof(IModelPropertyEditorLinkView))]
	public static class ModelPropertyEditorLinkViewDomainLogic {
		public static IModelListView Get_LinkView(IModelMemberViewItem modelPropertyEditor) {
			if((modelPropertyEditor.ModelMember != null) && (modelPropertyEditor.ModelMember.MemberInfo != null)) {
				if(!modelPropertyEditor.ModelMember.MemberInfo.IsList) {
					IModelClass modelClass = modelPropertyEditor.Application.BOModel.GetClass(modelPropertyEditor.ModelMember.Type);
					if(modelClass != null) {
						IModelListView defaultLookupListView = modelPropertyEditor.Application.BOModel.GetClass(modelPropertyEditor.ModelMember.Type).DefaultLookupListView;
						if(defaultLookupListView != null) {
							return defaultLookupListView;
						}
					}
				}
				else {
					IModelClass modelClass = modelPropertyEditor.Application.BOModel.GetClass(modelPropertyEditor.ModelMember.MemberInfo.ListElementType);
					if(modelClass != null && modelClass.DefaultLookupListView != null) {
						return modelClass.DefaultLookupListView;
					}
				}
			}
			return null;
		}
		public static IModelList<IModelListView> Get_LinkViews(IModelMemberViewItem modelMemberViewItem) {
			CalculatedModelNodeList<IModelListView> views = new CalculatedModelNodeList<IModelListView>();
			IEnumerable<IModelView> modelViews = ViewNamesCalculator.GetViews(modelMemberViewItem, true);
			foreach(IModelView modelView in modelViews) {
				views.Add((IModelListView)modelView);
			}
			return views;
		}
	}
	public interface IModelPropertyEditorLinkView {
		[Browsable(false)]
		IModelList<IModelListView> LinkViews { get; }
		[ModelBrowsable(typeof(CollectionPropertyOnlyCalculator))]
		[DataSourceProperty("LinkViews")]
		[Category("Appearance")]
		IModelListView LinkView { get; set; }
	}
	public class CustomCreateLinkViewEventArgs : HandledEventArgs {
		private ListView sourceViewCore;
		public CustomCreateLinkViewEventArgs(ListView sourceView) {
			sourceViewCore = sourceView;
		}
		public ListView SourceView {
			get { return sourceViewCore; }
		}
		public View LinkView { get; set; }
	}
	public class QueryLinkObjectsEventArgs : HandledEventArgs {
		private Window linkWindowCore;
		public QueryLinkObjectsEventArgs(Window linkWindow) {
			linkWindowCore = linkWindow;
		}
		public Window LinkWindow {
			get { return linkWindowCore; }
		}
		public IList LinkObjects { get; set; }
	}
}
