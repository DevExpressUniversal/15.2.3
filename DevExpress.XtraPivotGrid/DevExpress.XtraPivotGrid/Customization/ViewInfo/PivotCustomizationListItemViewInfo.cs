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

using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.Utils.Menu;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.FilterDropDown;
namespace DevExpress.XtraPivotGrid.Customization.ViewInfo {
	public class PivotCustomizationListItemViewInfo : CustomizationItemViewInfo {
		ListHeaderViewInfo headerViewInfo;
		readonly PivotCustomizationTreeBoxBase listBox;
		readonly PivotGridViewInfo viewInfo;
		public PivotCustomizationListItemViewInfo(PivotCustomizationTreeBoxBase listBox, IVisualCustomizationTreeItem node,
				PivotGridViewInfo viewInfo)
			: base(node) {
			this.listBox = listBox;
			this.viewInfo = viewInfo;
			if(ListBox != null && ListBox.CustomizationFields != null)
				ListBox.CustomizationFields.DeferUpdatesChanged += OnDeferUpdatesChanged;
		}
		protected internal ListHeaderViewInfo HeaderViewInfo {
			get {
				if(headerViewInfo == null)
					headerViewInfo = CreateHeaderViewInfo();
				return headerViewInfo;
			}
		}
		protected PivotCustomizationTreeBoxBase ListBox { get { return listBox; } }
		protected PivotGridViewInfo RootViewInfo { get { return viewInfo; } }
		protected virtual ListHeaderViewInfo CreateHeaderViewInfo() {
			return new ListHeaderViewInfo(ListBox, RootViewInfo, Item);
		}
		public override void Paint(GraphicsCache cache, Rectangle bounds, bool hotTrack, bool selected, bool focused) {
			HeaderViewInfo.Bounds = bounds;
			HeaderViewInfo.Paint(cache, selected && focused, hotTrack);
		}
		public override bool IsOpenCloseButton(Rectangle bounds, Point point) {
			return false;
		}
		public override ToolTipControlInfo GetToolTipObjectInfo() {
			return HeaderViewInfo.GetToolTipObjectInfo(Point.Empty);
		}
		public override void MouseUp(MouseEventArgs e) {
			HeaderViewInfo.MouseUp(e);
		}
		public override void MouseDown(MouseEventArgs e) {
			HeaderViewInfo.MouseDown(e);
		}
		public override void MouseMove(MouseEventArgs e) {
			HeaderViewInfo.MouseMove(e);
		}
		protected virtual void OnDeferUpdatesChanged(object sender, EventArgs e) {
			ResetViewInfo();
			ListBox.Invalidate();
		}
		protected internal void ResetViewInfo() {
			headerViewInfo = null;
		}
		public class ListHeaderViewInfo : PivotHeaderViewInfo {
			readonly PivotCustomizationTreeBoxBase listBox;
			readonly IVisualCustomizationTreeItem node;
			public ListHeaderViewInfo(PivotCustomizationTreeBoxBase listBox, PivotGridViewInfo viewInfo, IVisualCustomizationTreeItem node)
				: base(viewInfo, (PivotFieldItem)node.Field, false) {
				this.node = node;
				this.listBox = listBox;
				Initialize();
				UpdateCaption();
			}
			protected IVisualCustomizationTreeItem Node { get { return node; } }
			protected CustomizationFormFields CustomizationFields {
				get { return listBox != null ? listBox.CustomizationFields : null; }
			}
			protected override bool DeferUpdates {
				get { return CustomizationFields != null && CustomizationFields.DeferUpdates; }
			}
			protected override CustomizationFormFields GetCustomizationFormFields() {
				return CustomizationFields;
			}
			protected override bool CanShowFilterPopup { 
				get {
					if(DeferUpdates && Field.Area == PivotArea.DataArea || !Data.OptionsCustomization.AllowFilterInCustomizationForm || !Field.Visible)
						return false;
					return true; 
				}
			}
			protected override void MouseUpCore(MouseEventArgs e) {
				if(DeferUpdates && (!IsFilterDown || Field.Area == PivotArea.DataArea)) {
					ResetMouseDownLocation();
					Invalidate();
				} else {
					base.MouseUpCore(e);
				}
			}
			protected override bool CanSort {
				get { return !CanSortCore ? false : Field.CanSortCore || !Field.SortBySummaryInfo.IsEmpty; }
			}
			bool CanSortCore {
				get { return !DeferUpdates && Data.OptionsCustomization.AllowSortInCustomizationForm; }
			}
			public override bool ShowFilterButton {
				get {
					if(!Field.ShowFilterButton || !Field.ShowActiveFilterButton && !CanShowFilterPopup)
						return false;
					return true;
				}
			}
			public override bool ShowActiveFilterButton { get { return base.ShowActiveFilterButton || DeferUpdates && CustomizationFields.IsFieldFiltered(Data.GetField(Field)); } }
			public override bool ShowSortButton {
				get { return Field.ShowSortButton && CanSortCore; }
			}
			public override string Caption {
				get { return Node != null ? Node.Caption : string.Empty; }
			}
			public override bool ShowCollapsedButton {
				get { return false; }
			}
			protected override Control GetControlOwner() {
				if(this.listBox != null)
					return this.listBox;
				return base.GetControlOwner();
			}
			protected override void Invalidate(Rectangle bounds) {
				base.Invalidate(bounds);
				if(this.listBox != null)
					this.listBox.Invalidate(bounds);
			}
			protected override void CreatePopupMenuItems(DXPopupMenu menu) {
				AddPopupMenuSortAscending();
				AddPopupMenuSortDescending();
				AddPopupMenuClearSorting();
			}
			protected override bool IsClickAllowed(MouseEventArgs e) {
				return true;
			}
		}
	}
}
