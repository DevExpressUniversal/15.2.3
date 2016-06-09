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
using System.Drawing;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	[DomainComponent]
	public abstract class DashboardOrganizationItem : INotifyPropertyChanged {
		protected const string LocalizationGroupName = "DashboardOrganizationItems";
		public const string VisibilityPropertyName = "Visibility";
		private IModelApplication modelApplication;
		private ViewItemVisibility visibility = ViewItemVisibility.Show;
		protected void OnChanged(string propertyName) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		protected IModelApplication ModelApplication {
			get { return modelApplication; }
		}
		public DashboardOrganizationItem(IModelApplication modelApplication) {
			this.modelApplication = modelApplication;
		}
		public abstract void InitializeFromViewItem(IModelViewItem sourceModelViewItem);
		public IModelViewItem CreateDashboardViewItem(IModelViewItems targetItemsCollection) {
			return CreateDashboardViewItem(targetItemsCollection, Guid.NewGuid().ToString());
		}
		public abstract IModelViewItem CreateDashboardViewItem(IModelViewItems targetItemsCollection, string itemId);
		public abstract void SetupItem(IModelViewItem modelViewItem);
		[Browsable(false)]
		public abstract string Id { get; }
		[VisibleInDetailView(false)]
		public string Item {
			get { return this.ToString(); }
		}
		[VisibleInDetailView(false)]
		public ViewItemVisibility Visibility {
			get { return visibility; }
			set {
				if (visibility != value) {
					visibility = value;
					OnChanged(VisibilityPropertyName);
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
	public abstract class DashboardOrganizationItem<ViewItemType> : DashboardOrganizationItem where ViewItemType : IModelViewItem {
		private ViewItemType viewItem;
		protected abstract void SetupItemCore(ViewItemType modelViewItem);
		protected abstract void InitializeFromViewItemCore(ViewItemType sourceModelViewItem);
		public DashboardOrganizationItem(IModelApplication modelApplication) : base(modelApplication) { }
		public sealed override void InitializeFromViewItem(IModelViewItem sourceModelViewItem) {
			if (sourceModelViewItem is ViewItemType) {
				this.viewItem = (ViewItemType)sourceModelViewItem;
				InitializeFromViewItemCore((ViewItemType)sourceModelViewItem);
			}
		}
		protected virtual bool CanCreateDashboardViewItem(IModelViewItems targetItemsCollection, string itemId, out string reason) {
			reason = string.Empty;
			return true;
		}
		public sealed override IModelViewItem CreateDashboardViewItem(IModelViewItems items, string itemId) {
			string reason;
			if (!CanCreateDashboardViewItem(items, itemId, out reason)) {
				string message = "Unable to create view item.";
				if (!string.IsNullOrEmpty(reason)) {
					message += " " + reason;
				}
				throw new InvalidOperationException(message);
			}
			ViewItemType createdViewItem = items.AddNode<ViewItemType>(itemId);
			SetupItem(createdViewItem);
			return createdViewItem;
		}
		public sealed override void SetupItem(IModelViewItem modelViewItem) {
			if (modelViewItem is ViewItemType) {
				SetupItemCore((ViewItemType)modelViewItem);
			}
		}
		public override string Id {
			get { return viewItem != null ? viewItem.Id : string.Empty; }
		}
	}
	[DomainComponent]
	[DisplayName("Static Text")]
	public class StaticTextDashboardOrganizationItem : DashboardOrganizationItem<IModelStaticText> {
		protected override void InitializeFromViewItemCore(IModelStaticText sourceModelViewItem) {
			this.Text = sourceModelViewItem.Text;
		}
		protected override void SetupItemCore(IModelStaticText modelViewItem) {
			modelViewItem.Text = Text;
		}
		public StaticTextDashboardOrganizationItem(IModelApplication modelApplication) : base(modelApplication) { }
		public override string ToString() {
			return string.Format("{0}: {1}", CaptionHelper.GetLocalizedText(LocalizationGroupName, "StaticText"), Text);
		}
		[FieldSize(FieldSizeAttribute.Unlimited)]
		public string Text { get; set; }
	}
	[DomainComponent]
	[DisplayName("Static Image")]
	public class StaticImageDashboardOrganizationItem : DashboardOrganizationItem<IModelStaticImage> {
		protected override void InitializeFromViewItemCore(IModelStaticImage sourceModelViewItem) {
			this.ImageName = sourceModelViewItem.ImageName;
		}
		protected override void SetupItemCore(IModelStaticImage modelViewItem) {
			modelViewItem.ImageName = ImageName;
		}
		public StaticImageDashboardOrganizationItem(IModelApplication modelApplication) : base(modelApplication) { }
		public override string ToString() {
			return string.Format("{0}: {1}", CaptionHelper.GetLocalizedText(LocalizationGroupName, "StaticImage"), ImageName);
		}
		[ImmediatePostData]
		public string ImageName { get; set; }
		[ImageEditor(ImageSizeMode = ImageSizeMode.CenterImage)]
		public Image Preview {
			get { return ImageLoader.Instance.GetImageInfo(ImageName).Image; }
		}
	}
	[DomainComponent]
	[DefaultProperty("ActionContainerId")]
	public class ActionContainerDescriptor : IComparable<ActionContainerDescriptor> {
		private IModelActionContainer actionContainer;
		public ActionContainerDescriptor(IModelActionContainer actionContainer) {
			this.actionContainer = actionContainer;
		}
		public string ActionContainerId {
			get { return actionContainer.Id; }
		}
		#region IComparable<ActionContainerDescriptor> Members
		int IComparable<ActionContainerDescriptor>.CompareTo(ActionContainerDescriptor other) {
			return string.Compare(this.ActionContainerId, other.ActionContainerId);
		}
		#endregion
	}
	[DomainComponent]
	[DisplayName("Action Container")]
	public class ActionContainerDashboardOrganizationItem : DashboardOrganizationItem<IModelActionContainerViewItem> {
		private Dictionary<string, ActionContainerDescriptor> availableActionContainers;
		private void CollectAvailableActionContainers() {
			availableActionContainers = new Dictionary<string, ActionContainerDescriptor>();
			IModelActionDesignContainerMapping actionDesignContainerMapping = ModelApplication.ActionDesign as IModelActionDesignContainerMapping;
			if (actionDesignContainerMapping != null) {
				foreach (IModelActionContainer actionContainer in actionDesignContainerMapping.ActionToContainerMapping) {
					availableActionContainers[actionContainer.Id] = new ActionContainerDescriptor(actionContainer);
				}
			}
		}
		protected override void InitializeFromViewItemCore(IModelActionContainerViewItem sourceModelViewItem) {
			if (availableActionContainers == null) {
				CollectAvailableActionContainers();
			}
			string containerId = sourceModelViewItem.ActionContainer.Id;
			if (!availableActionContainers.ContainsKey(containerId)) {
				availableActionContainers[containerId] = new ActionContainerDescriptor(sourceModelViewItem.ActionContainer);
			}
			ActionContainer = availableActionContainers[containerId];
		}
		protected override void SetupItemCore(IModelActionContainerViewItem modelViewItem) {
			if (ActionContainer == null || string.IsNullOrEmpty(ActionContainer.ActionContainerId)) {
				return;
			}
			IModelActionDesignContainerMapping actionDesignContainerMapping = ModelApplication.ActionDesign as IModelActionDesignContainerMapping;
			if (actionDesignContainerMapping != null) {
				modelViewItem.ActionContainer = actionDesignContainerMapping.ActionToContainerMapping[ActionContainer.ActionContainerId];
			}
		}
		public ActionContainerDashboardOrganizationItem(IModelApplication modelApplication) : base(modelApplication) { }
		public override string ToString() {
			string containerId = ActionContainer == null ? "" : ActionContainer.ActionContainerId;
			return string.Format("{0}: {1}", CaptionHelper.GetLocalizedText(LocalizationGroupName, "ActionContainer"), containerId);
		}
		[DataSourceProperty("ActionContainers")]
		public ActionContainerDescriptor ActionContainer { get; set; }
		[Browsable(false)]
		public IList<ActionContainerDescriptor> ActionContainers {
			get {
				if (availableActionContainers == null) {
					CollectAvailableActionContainers();
				}
				return new List<ActionContainerDescriptor>(availableActionContainers.Values);
			}
		}
	}
	public class ViewDashboardOrganizationItemController : ViewController<DetailView> {
		protected override void OnActivated() {
			base.OnActivated();
			if (View.CurrentObject != null) {
				((ViewDashboardOrganizationItem)View.CurrentObject).PropertyChanged += new PropertyChangedEventHandler(ViewDashboardOrganizationItemController_PropertyChanged);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if (View.CurrentObject != null) {
				((ViewDashboardOrganizationItem)View.CurrentObject).PropertyChanged -= new PropertyChangedEventHandler(ViewDashboardOrganizationItemController_PropertyChanged);
			}
		}
		private void ViewDashboardOrganizationItemController_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "ObjectType") {
				ViewItem item = View.FindItem("ViewDescriptor");
				if (item != null) {
					item.Refresh();
				}
			}
		}
		public ViewDashboardOrganizationItemController() {
			TargetObjectType = typeof(ViewDashboardOrganizationItem);
		}
	}
	[DomainComponent]
	[DefaultProperty("ViewDescription")]
	public class DashboardViewItemDescriptor {
		private IModelView modelView;
		private string viewDescription = string.Empty;
		public DashboardViewItemDescriptor(IModelView modelView) {
			Guard.ArgumentNotNull(modelView, "modelView");
			this.modelView = modelView;
		}
		private string GetViewTypeString() {
			if (modelView is IModelListView)
				return CaptionHelper.GetLocalizedText(CaptionHelper.CaptionsLocalizationGroup, typeof(ListView).Name); ;
			if (modelView is IModelDetailView)
				return CaptionHelper.GetLocalizedText(CaptionHelper.CaptionsLocalizationGroup, typeof(DetailView).Name); ;
			if (modelView is IModelDashboardView)
				return CaptionHelper.GetLocalizedText(CaptionHelper.CaptionsLocalizationGroup, typeof(DashboardView).Name); ;
			return CaptionHelper.GetLocalizedText(CaptionHelper.CaptionsLocalizationGroup, typeof(View).Name);
		}
		public override string ToString() {
			return ViewDescription;
		}
		public string ViewCaption {
			get { return modelView.Caption; }
		}
		public string ViewId {
			get { return modelView.Id; }
		}
		public string ViewDescription {
			get {
				if (string.IsNullOrEmpty(viewDescription)) {
					string viewTypeString = GetViewTypeString();
					string prefix = string.IsNullOrEmpty(viewTypeString) ? string.Empty : string.Format(@"{0}: ", viewTypeString);
					viewDescription = prefix + string.Format("{0} ({1})", ViewCaption, modelView.Id);
				}
				return viewDescription;
			}
		}
		[Browsable(false)]
		public IModelView ModelView {
			get { return modelView; }
		}
	}
	[DomainComponent]
	[DisplayName("View")]
	public class ViewDashboardOrganizationItem : DashboardOrganizationItem<IModelDashboardViewItem> {
		private Type objectType;
		private DashboardViewItemDescriptor viewDescriptor;
		protected override void InitializeFromViewItemCore(IModelDashboardViewItem sourceModelViewItem) {
			if (sourceModelViewItem.View != null) {
				IModelObjectView modelObjectView = sourceModelViewItem.View.AsObjectView;
				if (modelObjectView != null) {
					this.objectType = modelObjectView.ModelClass.TypeInfo.Type;
				}
				this.viewDescriptor = new DashboardViewItemDescriptor(sourceModelViewItem.View);
			}
			Criteria = sourceModelViewItem.Criteria;
			ActionsToolbarVisibility = sourceModelViewItem.ActionsToolbarVisibility;
		}
		protected override void SetupItemCore(IModelDashboardViewItem modelViewItem) {
			if (viewDescriptor != null) {
				modelViewItem.View = viewDescriptor.ModelView;
				modelViewItem.Criteria = Criteria;
				modelViewItem.ActionsToolbarVisibility = ActionsToolbarVisibility;
			}
		}
		protected override bool CanCreateDashboardViewItem(IModelViewItems targetItemsCollection, string itemId, out string reason) {
			if (ViewDescriptor == null || string.IsNullOrEmpty(viewDescriptor.ViewId)) {
				reason = "View is unassigned.";
				return false;
			}
			reason = string.Empty;
			return true;
		}
		public ViewDashboardOrganizationItem(IModelApplication modelApplication) : base(modelApplication) { }
		public override string ToString() {
			return viewDescriptor != null ? viewDescriptor.ToString() : string.Empty;
		}
		[Browsable(false)]
		public IList<DashboardViewItemDescriptor> AvailableViews {
			get {
				List<DashboardViewItemDescriptor> result = new List<DashboardViewItemDescriptor>();
				if (objectType != null) {
					foreach (IModelView availableView in ModelApplication.Views) {
						if (availableView.AsObjectView != null && availableView.AsObjectView.ModelClass != null && availableView.AsObjectView.ModelClass.TypeInfo.Type == objectType && !availableView.Id.Contains("Lookup")) {
							if (viewDescriptor != null && viewDescriptor.ViewId == availableView.Id) {
								result.Add(viewDescriptor);
							}
							else {
								result.Add(new DashboardViewItemDescriptor(availableView));
							}
						}
					}
				}
				return result;
			}
		}
		[ImmediatePostData]
		public Type ObjectType {
			get { return objectType; }
			set {
				if (objectType != value) {
					objectType = value;
					Criteria = null;
					ViewDescriptor = null;
					OnChanged("ObjectType");
					OnChanged("AvailableViews");
				}
			}
		}
		[DataSourceProperty("AvailableViews")]
		[DisplayName("View")]
		public DashboardViewItemDescriptor ViewDescriptor {
			get { return viewDescriptor; }
			set {
				if (!object.ReferenceEquals(viewDescriptor, value)) {
					viewDescriptor = value;
					OnChanged("ViewDescriptor");
				}
			}
		}
		[CriteriaOptions("ObjectType")]
		public string Criteria { get; set; }
		public ActionsToolbarVisibility ActionsToolbarVisibility { get; set; }
	}
}
