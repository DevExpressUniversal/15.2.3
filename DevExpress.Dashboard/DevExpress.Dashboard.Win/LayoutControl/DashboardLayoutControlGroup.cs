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
using DevExpress.XtraLayout.Utils;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DashboardWin.Commands;
using DevExpress.Utils.Commands;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardWin.Native {
	public class DashboardLayoutControlGroup : DashboardLayoutControlGroupBase, IDashboardLayoutControlItem {		
		readonly DashboardItemCaptionButtonsRepository captionButtonsInfo;
		readonly DashboardLayoutControlCaption layoutControlCaption;
		readonly IServiceProvider serviceProvider;
		readonly Locker customizationLocker = new Locker();
		DashboardViewer dashboardViewer;
		bool isHidden;
		bool selected;
		Padding groupPadding = Padding.Empty;
		public string Type { get; set; }
		public DashboardItemViewer ItemViewer { get; set; }
		public DashboardItemDesigner ItemDesigner { get; set; }
		protected override bool IsHiddenGroup { get { return isHidden && !IsRoot; } }
		protected override Padding DefaultPaddings { 
			get {
				if(Owner == null || IsHiddenGroup)
					return Padding.Empty;
				return !TextVisible ? Padding.Empty : GroupPadding; 
			} 
		}
		Padding GroupPadding {
			get {
				if(groupPadding == Padding.Empty) {
					Skin skin = DashboardSkins.GetSkin(Owner.LookAndFeel);
					SkinPaddingEdges skinPadding = skin.Properties["GroupPadding"] as SkinPaddingEdges;
					if(skinPadding != null)
						groupPadding = new Padding(skinPadding.Left, skinPadding.Right, skinPadding.Top, skinPadding.Bottom);
				}
				return groupPadding;
			}
		}
		bool IDashboardLayoutControlItem.IsHidden { get { return isHidden || IsRoot; } }
		BaseLayoutItem IDashboardLayoutControlItem.Inner { get { return this; } }
		IDashboardLayoutControlItem IDashboardLayoutControlItem.ParentGroup { get { return null; } }
		bool IDashboardItemViewerContainer.ShowBorders { get { return BordersVisible; } set { } }
		bool IDashboardItemViewerContainer.IsSelected { get { return selected || !dashboardViewer.IsDesignMode; } }
		public DashboardLayoutControlGroup(DashboardViewer dashboardViewer, IServiceProvider serviceProvider, bool isHidden) {
			Guard.ArgumentNotNull(dashboardViewer, "dashboardViewer");
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			this.dashboardViewer = dashboardViewer;
			this.serviceProvider = serviceProvider;
			this.isHidden = isHidden;
			SubscribeServiceEvents();
			AllowDropGroup = isHidden;
			AllowHtmlStringInCaption = true;
			captionButtonsInfo = new DashboardItemCaptionButtonsRepository(dashboardViewer);
			layoutControlCaption = new DashboardLayoutControlCaption(this);
			ButtonMouseClick += OnButtonMouseClick;
		}
		protected override LayoutGroup CreateLayoutGroup() {
			return new DashboardLayoutControlGroup(dashboardViewer, serviceProvider, true);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeServiceEvents();
				if(ItemViewer != null) {
					ItemViewer.Parent = null;
					ItemViewer.Dispose();
				}
				if(dashboardViewer != null)
					dashboardViewer = null;
				captionButtonsInfo.Dispose();
			}
			base.Dispose(disposing);
		}
		public override string ToString() {
			return Name;
		}
		public override void HideToCustomization() {
			base.HideToCustomization();
			if (!customizationLocker.IsLocked) {
				IDashboardCommandService commandService = serviceProvider.RequestService<IDashboardCommandService>();
				if (commandService != null)
					commandService.ExecuteCommand(DashboardCommandId.DeleteGroup);
			}
		}
		public void BeginCustomization() {
			customizationLocker.Lock();
		}
		public void EndCustomization() {
			customizationLocker.Unlock();
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
		void IDashboardItemViewerContainer.UpdateCaption(DashboardItemCaptionViewModel captionViewModel, IList<FormattableValue> drillDownValues) {
			GroupBordersVisible = TextVisible = captionViewModel.ShowCaption;
			layoutControlCaption.UpdateCaption(captionViewModel.Caption, drillDownValues);
		}
		void OnButtonMouseClick(object sender, DashboardLayoutControlGroupBase.DashboardLayoutControlGroupButtonClickEventArgs e) {
			captionButtonsInfo.ExecuteButtonInfo(ItemViewer, e.ButtonIndex);
		}
		void OnLayoutItemSelected(object sender, DashboardLayoutItemSelectedEventArgs e) {
			DashboardItemViewer selectedItemViewer = null;
			if (e.SelectedItem != null)
				selectedItemViewer = e.SelectedItem.ItemViewer;
			selected = selectedItemViewer == ItemViewer;
		}
		void IDashboardItemViewerContainer.UpdateLookAndFeel(UserLookAndFeel lookAndFeel) {
			layoutControlCaption.UpdateLookAndFeel(lookAndFeel);
		}
		void IDashboardItemViewerContainer.RefreshCaptionButtons() {
			BaseButtonCollection buttonInfo = new BaseButtonCollection(null);
			captionButtonsInfo.FillButtonInfo(buttonInfo, ItemViewer.GetButtonInfoCreators());
			CustomHeaderButtons.BeginUpdate();
			CustomHeaderButtons.Clear();
			CustomHeaderButtons.AddRange(buttonInfo.ToArray());
			CustomHeaderButtons.EndUpdate();
		}
		ClientArea IDashboardItemViewerContainer.CalcCaptionArea(ClientArea viewerArea) {
			int layoutControlLocationY = ViewInfo.BoundsRelativeToControl.Top;
			Skin skin = DashboardSkins.GetSkin(ItemViewer.LookAndFeel);
			if(skin != null) {
				SkinElement sel = skin[DashboardSkins.SkinDashboardItemBackground];
				if(sel != null && sel.HasImage)
					layoutControlLocationY += sel.Image.SizingMargins.Top;
			}
			return layoutControlCaption.CalcCaptionArea(viewerArea, layoutControlLocationY);
		}
		ClientArea IDashboardItemViewerContainer.CalcClientArea() {
			Point location = ViewInfo.ClientAreaRelativeToControl.Location;
			return new ClientArea { Left = location.X, Top = location.Y, Width = Size.Width, Height = Size.Height - location.Y };
		}
		void IItemCaption.Update(ItemCaptionContentInfo itemCaptionContentInfo) {
			StringBuilder sb = new StringBuilder();
			OptionsToolTip.ToolTip = layoutControlCaption.Update(itemCaptionContentInfo, sb);
			Text = sb.ToString();
		}
		TDesigner IDashboardItemDesignerProvider.GetDesigner<TDesigner>() {
			return ItemDesigner as TDesigner;
		}
	}
}
