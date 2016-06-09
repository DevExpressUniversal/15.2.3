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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
namespace DevExpress.ExpressApp.Web.Layout {
	public class LayoutItemTemplateContainerBase : Control {
		public const string ControlVisibilityKey = "ControlVisibility";
		public const string CollectionsEditModeVisibilityKey = "CollectionsEditModeVisibility";
		private WebLayoutManager layoutManager;
		private IModelViewLayoutElement model;
		private ViewItemsCollection viewItems;
		private BoolList visibility = new BoolList();
		private void visibility_ResultValueChanged(object sender, BoolValueChangedEventArgs e) {
			Visible = (BoolList)sender;
		}
		public LayoutItemTemplateContainerBase(WebLayoutManager layoutManager, ViewItemsCollection viewItems, IModelViewLayoutElement model) {
			layoutManager.Items.Add(WebIdHelper.GetLayoutItemId(model), this);
			this.layoutManager = layoutManager;
			this.viewItems = viewItems;
			this.model = model;
			visibility.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(visibility_ResultValueChanged);
		}
		public override void Dispose() {
			visibility.ResultValueChanged -= new EventHandler<BoolValueChangedEventArgs>(visibility_ResultValueChanged);
			base.Dispose();
		}
		public BoolList Visibility {
			get { return visibility; }
		}
		public WebLayoutManager LayoutManager {
			get { return layoutManager; }
		}
		public IModelViewLayoutElement Model {
			get { return model; }
		}
		public ViewItemsCollection ViewItems {
			get { return viewItems; }
		}
		public ViewItem ViewItem {
			get {
				IModelLayoutViewItem modelLayoutItem = model as IModelLayoutViewItem;
				if(modelLayoutItem != null) {
					string detailViewItemId = modelLayoutItem.ViewItem != null ? modelLayoutItem.ViewItem.Id : modelLayoutItem.Id;
					return viewItems[detailViewItemId];
				}
				else {
					return null;
				}
			}
		}
		public LayoutGroupTemplateContainer ParentGroupTemplateContainer {
			get {
				Control currentControl = Parent;
				while(currentControl != null && !(currentControl is LayoutItemTemplateContainerBase)) {
					currentControl = currentControl.Parent;
				}
				return currentControl as LayoutGroupTemplateContainer;
			}
		}
		public TabbedGroupTemplateContainer ParentTabbedGroupTemplateContainer {
			get {
				Control currentControl = Parent;
				while(currentControl != null && !(currentControl is LayoutItemTemplateContainerBase)) {
					currentControl = currentControl.Parent;
				}
				return currentControl as TabbedGroupTemplateContainer;
			}
		}
		public WebControl CaptionControl { get; set; }
	}
	public class LayoutGroupTemplateContainerBase : LayoutItemTemplateContainerBase {
		private Dictionary<string, LayoutItemTemplateContainerBase> innerItems = new Dictionary<string, LayoutItemTemplateContainerBase>();
		public LayoutGroupTemplateContainerBase(WebLayoutManager layoutManager, ViewItemsCollection detailViewItems, IModelViewLayoutElement groupInfo) : base(layoutManager, detailViewItems, groupInfo) { }
		public override void Dispose() {
			if(innerItems != null) {
				innerItems.Clear();
				innerItems = null;
			}
			base.Dispose();
		}
		public Dictionary<string, LayoutItemTemplateContainerBase> Items {
			get { return innerItems; }
		}
	}
	public class TabbedGroupTemplateContainer : LayoutGroupTemplateContainerBase {
		public TabbedGroupTemplateContainer(WebLayoutManager layoutManager, ViewItemsCollection detailViewItems, IModelTabbedGroup groupInfo) : base(layoutManager, detailViewItems, groupInfo) { }
		public new IModelTabbedGroup Model {
			get { return (IModelTabbedGroup)base.Model; }
		}
		public string ActiveTabName { get; set; }
		#region Obsolete 14.2
		private int activeTabIndex = -1;
		[Obsolete("Use the ActiveTabName property instead.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int ActiveTabIndex {
			get { return activeTabIndex; }
			set { activeTabIndex = value; }
		}
		#endregion
	}
	public class LayoutGroupTemplateContainer : LayoutGroupTemplateContainerBase {
		private ImageInfo headerImageInfo;
		public LayoutGroupTemplateContainer(WebLayoutManager layoutManager, ViewItemsCollection detailViewItems, IModelLayoutGroup groupInfo, ImageInfo headerImageInfo)
			: base(layoutManager, detailViewItems, groupInfo) {
			this.headerImageInfo = headerImageInfo;
		}
		public new IModelLayoutGroup Model {
			get { return (IModelLayoutGroup)base.Model; }
		}
		public bool ShowCaption {
			get { return (Model.ShowCaption.HasValue && Model.ShowCaption.Value); } 
		}
		public string Caption {
			get { return Model.Caption; }
		}
		public bool HasHeaderImage {
			get { return !HeaderImageInfo.IsEmpty; }
		}
		public ImageInfo HeaderImageInfo {
			get { return headerImageInfo; }
		}
		public bool IsOnTabPage {
			get { return Model.Parent != null && Model.Parent is IModelTabbedGroup; }
		}
		public FlowDirection ParentGroupDirection {
			get { return Model.Parent is IModelLayoutGroup ? ((IModelLayoutGroup)Model.Parent).Direction : FlowDirection.Horizontal; }
		}
	}
	public class LayoutItemTemplateContainer : LayoutItemTemplateContainerBase, INamingContainer, IInstantiateMode {
		private bool showCaption = true;
		private string caption;
		private Unit captionWidth;
		private DevExpress.Utils.Locations captionLocation;
		private DevExpress.Utils.HorzAlignment captionHorizontalAlignment;
		private DevExpress.Utils.VertAlignment captionVerticalAlignment;
		public LayoutItemTemplateContainer(WebLayoutManager layoutManager, ViewItemsCollection detailViewItems, IModelLayoutViewItem info) : base(layoutManager, detailViewItems, info) { }
		public new IModelLayoutViewItem Model {
			get { return (IModelLayoutViewItem)base.Model; }
		}
		public bool ShowCaption {
			get { return showCaption; }
			set { showCaption = value; }
		}
		public string Caption {
			get { return caption; }
			set { caption = value; }
		}
		public Unit CaptionWidth {
			get { return captionWidth; }
			set { captionWidth = value; }
		}
		public DevExpress.Utils.Locations CaptionLocation {
			get { return captionLocation; }
			set { captionLocation = value; }
		}
		public DevExpress.Utils.HorzAlignment CaptionHorizontalAlignment {
			get { return captionHorizontalAlignment; }
			set { captionHorizontalAlignment = value; }
		}
		public DevExpress.Utils.VertAlignment CaptionVerticalAlignment {
			get { return captionVerticalAlignment; }
			set { captionVerticalAlignment = value; }
		}
		public WebControl LayoutItemControl { get; set; }
		bool IInstantiateMode.ForceInstantiation {
			get { return LayoutItemControl != null; }
		}
	}
}
