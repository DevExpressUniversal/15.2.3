#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Text;
using DevExpress.Data.Utils;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Utils;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Utils.Commands;
using DevExpress.DashboardWin.Commands;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardWin.Native {
	public class DashboardLayoutControlItem : DashboardLayoutControlItemBase, IDashboardLayoutControlItem {
		readonly DashboardViewer dashboardViewer;
		readonly IServiceProvider serviceProvider;
		readonly DashboardItemCaptionButtonsRepository captionButtons;
		readonly DashboardLayoutControlCaption layoutControlCaption;
		readonly Locker customizationLocker = new Locker();
		bool selected;
		Padding itemInGroupPadding = Padding.Empty;
		public string Type { get; set; }
		public DashboardItemViewer ItemViewer { get { return (DashboardItemViewer)Control; } set { Control = value; } }
		public DashboardItemDesigner ItemDesigner { get; set; }
		public DashboardItemCaptionButtonsRepository CaptionButtons { get { return captionButtons; } }
		public bool ShowBorders { get { return BordersVisible; } set { BordersVisible = value; } }
		public override LayoutGroup Parent {
			get { return base.Parent; }
			set {
				base.Parent = value;
				UpdateSizeConstraints();
			}
		}
		protected override Padding DefaultPaddings {
			get {
				LayoutGroup parent = Parent;
				if(parent != null && parent.GroupBordersVisible)
					return ItemInGroupPadding;
				return Padding.Empty;
			}
		}
		Padding ItemInGroupPadding {
			get {
				if(itemInGroupPadding == Padding.Empty) {
					Skin skin = DashboardSkins.GetSkin(Owner.LookAndFeel);
					SkinPaddingEdges skinPadding = skin.Properties["ItemInGroupSpacing"] as SkinPaddingEdges;
					if(skinPadding != null)
						itemInGroupPadding = new Padding(skinPadding.Left, skinPadding.Right, skinPadding.Top, skinPadding.Bottom);
				}
				return itemInGroupPadding;
			}
		}
		IDashboardLayoutControlItem ParentGroup {
			get {
				IDashboardLayoutControlItem parentGroup = Parent as IDashboardLayoutControlItem;
				return parentGroup != null && !parentGroup.IsHidden ? parentGroup : null;
			} 
		}
		bool IDashboardLayoutControlItem.IsHidden { get { return false; } }
		BaseLayoutItem IDashboardLayoutControlItem.Inner { get { return this; } }
		IDashboardLayoutControlItem IDashboardLayoutControlItem.ParentGroup { get { return ParentGroup; } }
		bool IDashboardItemViewerContainer.IsSelected { get { return selected || !dashboardViewer.IsDesignMode; } }
		public DashboardLayoutControlItem(DashboardViewer dashboardViewer, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(dashboardViewer, "dashboardViewer");
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.dashboardViewer = dashboardViewer;
			this.serviceProvider = serviceProvider;
			SubscribeServiceEvents();
			captionButtons = new DashboardItemCaptionButtonsRepository(dashboardViewer);
			base.BeginInit();
			try {
				layoutControlCaption = new DashboardLayoutControlCaption(this);
				TextLocation = Locations.Top;
				SetVisible(false);
				AllowHtmlStringInCaption = true;
				ButtonMouseClick += OnButtonMouseClick;
			}
			finally {
				base.EndInit();
			}
		}
		public override void EndInit() {
			base.EndInit();
			if (Control != null) {
				originalEnabled = true;
				originalEnabledInitialized = true;
			}
		}
		public override void HideToCustomization() {
			if (!customizationLocker.IsLocked) {
				IDashboardCommandService commandService = serviceProvider.RequestService<IDashboardCommandService>();
				if (commandService != null)
					commandService.ExecuteCommand(DashboardCommandId.DeleteItem);
			}
		}
		public void BeginCustomization() {
			customizationLocker.Lock();
		}
		public void EndCustomization() {
			customizationLocker.Unlock();
		}
		public override string ToString() {
			return Name;
		}
		public void ForceExecuteButtonInfo(DashboardItemCaptionButtonInfo info) {
			info.Execute(dashboardViewer, ItemViewer, DashboardArea.DashboardItemCaption);
		}
		void ClearViewInfo() {
			ViewInfo.Dispose();
			viewInfoCore = null;
		}
		public void UpdateSizeConstraints() {
			DashboardItemViewer itemViewer = ItemViewer;
			if(itemViewer == null)
				return;
			IFilterElementControl viewControl = itemViewer.ViewControl as IFilterElementControl;
			if(viewControl != null && viewControl.EditorHeight > 0) {
				ClearViewInfo();
				Rectangle bounds = ViewInfo.BoundsRelativeToControl;
				Rectangle client = ViewInfo.ClientAreaRelativeToControl;
				int height = viewControl.EditorHeight + client.Top - bounds.Top;
				Skin skin = DashboardSkins.GetSkin(itemViewer.LookAndFeel);
				if(skin != null) {
					SkinElement sel = skin[DashboardSkins.SkinDashboardItemBackground];
					if(sel != null && sel.HasImage)
						height += sel.Image.SizingMargins.Bottom;
				}
				IDashboardLayoutControlItem parentGroup = ParentGroup;
				if(parentGroup != null)
					height += ItemInGroupPadding.Bottom;
				SizeConstraintsType = SizeConstraintsType.Custom;
				MaxSize = MinSize = new Size(0, height);
				IsLocked = true;
			}
		}
		void SubscribeServiceEvents() {
			IDashboardLayoutSelectionService selectionService = serviceProvider.RequestServiceStrictly<IDashboardLayoutSelectionService>();
			selectionService.ItemSelected += OnLayoutItemSelected;
		}
		void UnsubscribeServiceEvents() {
			IDashboardLayoutSelectionService selectionService = serviceProvider.RequestService<IDashboardLayoutSelectionService>();
			if (selectionService != null)
				selectionService.ItemSelected -= OnLayoutItemSelected;
		}
		TDesigner IDashboardItemDesignerProvider.GetDesigner<TDesigner>() {
			return ItemDesigner as TDesigner;
		}
		void IDashboardItemViewerContainer.UpdateLookAndFeel(UserLookAndFeel lookAndFeel) {
			layoutControlCaption.UpdateLookAndFeel(lookAndFeel);
			UpdateSizeConstraints();
		}
		void IDashboardItemViewerContainer.RefreshCaptionButtons() {
			BaseButtonCollection buttonInfo = new BaseButtonCollection(null);
			captionButtons.FillButtonInfo(buttonInfo, ItemViewer.GetButtonInfoCreators());
			CustomHeaderButtons.BeginUpdate();
			CustomHeaderButtons.Clear();
			CustomHeaderButtons.AddRange(buttonInfo.ToArray());
			CustomHeaderButtons.EndUpdate();
		}
		ClientArea CalcCaptionArea(ClientArea viewerArea) {
			Rectangle bounds = ViewInfo.BoundsRelativeToControl;
			Padding padding = ViewInfo.Padding;
			int captionMarginTop = padding.Height - padding.Top;
			return layoutControlCaption.CalcCaptionArea(viewerArea, bounds.Top + captionMarginTop);
		}
		ClientArea IDashboardItemViewerContainer.CalcCaptionArea(ClientArea viewerArea) {
			return CalcCaptionArea(viewerArea);
		}
		ClientArea IDashboardItemViewerContainer.CalcClientArea() {
			return null;
		}
		void IDashboardItemViewerContainer.UpdateCaption(DashboardItemCaptionViewModel captionViewModel, IList<FormattableValue> drillDownValues) {
			TextVisible = captionViewModel.ShowCaption;
			layoutControlCaption.UpdateCaption(captionViewModel.Caption, drillDownValues);
			ILayoutControl layoutControl = Owner;
			if (layoutControl != null) {
				UserCustomizationForm customizationForm = layoutControl.CustomizationForm;
				if (!Visible && customizationForm != null)
					customizationForm.Invalidate(true);
			}
			UpdateSizeConstraints();
		}
		void IItemCaption.Update(ItemCaptionContentInfo itemCaptionContentInfo) {
			StringBuilder sb = new StringBuilder();
			OptionsToolTip.ToolTip = layoutControlCaption.Update(itemCaptionContentInfo, sb);
			Text = sb.ToString().Replace("&", "&&");
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (ItemViewer != null) {
					ItemViewer.Parent = null;
					ItemViewer.Dispose();
				}
				if (ItemDesigner != null)
					ItemDesigner.Dispose();
				captionButtons.Dispose();
				UnsubscribeServiceEvents();
			}
			base.Dispose(disposing);
		}
		void OnButtonMouseClick(object sender, DashboardLayoutControlItemButtonClickEventArgs e) {
			captionButtons.ExecuteButtonInfo(ItemViewer, e.ButtonIndex);
		}
		void OnLayoutItemSelected(object sender, DashboardLayoutItemSelectedEventArgs e) {
			DashboardItemViewer selectedItemViewer = null;
			if (e.SelectedItem != null)
				selectedItemViewer = e.SelectedItem.ItemViewer;
			selected = selectedItemViewer == ItemViewer;
		}
	}
}
