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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Menu;
namespace DevExpress.XtraGrid.Views.Grid.Customization {
	[ToolboxItem(false)]
	public class CustomCustomizationListBox : DevExpress.XtraEditors.Customization.CustomCustomizationListBoxBase {
		public CustomCustomizationListBox(CustomizationForm form) : base(form) { }
		public new CustomizationForm CustomizationForm { get { return base.CustomizationForm as CustomizationForm; } }
		public GridView View { get { return CustomizationForm.View; } }
		protected override AppearanceObject GetHintForEmptyListAppearance() {
			return View.ViewInfo.PaintAppearance.CustomizationFormHint;
		}
		protected GridHandler GridHandler { get { return (GridHandler)View.Handler; } }
		protected override void ShowItemMenu(object item) {
			GridColumn column = item as GridColumn;
			if (column == null) return;
			if (!View.OptionsMenu.EnableColumnMenu) return;
			GridViewColumnMenu menu = new GridViewColumnMenu(View);
			menu.Init(column);
			GridHitInfo hitInfo = View.CalcHitInfo(View.GridControl.PointToClient(Control.MousePosition));
			if(hitInfo.HitTest != GridHitTest.CustomizationForm)
				hitInfo.HitTest = GridHitTest.Column;
			hitInfo.Column = column;
			View.DoShowGridMenu(menu, hitInfo);
		}
		protected override void EndDrag(MouseEventArgs e) {
			View.GridControl.DragController.EndDrag(e);
		}
		protected override void CancelDrag() {
			View.GridControl.DragController.CancelDrag();
		}
		protected override void DoDragging(MouseEventArgs e) {
			View.GridControl.DragController.DoDragging(e);
		}
		protected override void DoDragDrop(object dragItem, Point p) {
			GridHandler.DownPointHitInfo = View.CalcHitInfo(p);
			GridHandler.DoStartDragObject(dragItem, new Size(ClientSize.Width, ItemHeight));
		}
		protected override void DoShowItem(object item) {
			base.DoShowItem(item);
			GridColumn col = item as GridColumn;
			if(col == null) return;
			col.Visible = true;
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
	}
	[ToolboxItem(false)]
	public class ColumnCustomizationListBox : CustomCustomizationListBox {
		public ColumnCustomizationListBox(CustomizationForm form)
			: base(form) {
		}
		public override int GetItemHeight() { return (View.ViewInfo as GridViewInfo).ColumnRowHeight; }
		protected override void DrawItemObject(GraphicsCache cache, int index, Rectangle bounds, DrawItemState itemState) {
			GridColumn column = GetItemValue(index) as GridColumn;
			if(IsRightToLeft) bounds.X--;
			bounds.Width++;
			((GridPainter)View.Painter).DrawColumnDrag(cache, View.ViewInfo as GridViewInfo,
				column, bounds,
				column == CustomizationForm.PressedItem, true);
		}
		protected override IComparer CreateComparer() {
			return new ColumnComparer();
		}
		protected override void AddItems(ArrayList list) {
			foreach (GridColumn column in View.Columns) {
				if (column.CanShowInCustomizationForm) list.Add(column);
			}
		}
		protected override bool IsDragging { get { return View != null && View.State == GridState.ColumnDragging; } }
		class ColumnComparer : IComparer {
			int IComparer.Compare(object a, object b) {
				GridColumn c1 = (GridColumn)a, c2 = (GridColumn)b;
				ColumnView view = c1.View;
				int res = 0;
				if(view != null)
					res = Comparer.Default.Compare(view.GetNonFormattedCaption(c1.GetCustomizationCaption()), view.GetNonFormattedCaption(c2.GetCustomizationCaption()));
				if (res == 0) res = Comparer.Default.Compare(c1.AbsoluteIndex, c2.AbsoluteIndex);
				return res;
			}
		}
		protected override string GetHintCaptionForEmptyList() {
			return GridLocalizer.Active.GetLocalizedString(GridStringId.CustomizationFormColumnHint);
		}
	}
	[ToolboxItem(false)]
	public class CustomizationForm : DevExpress.XtraEditors.Customization.CustomizationFormBase {
		GridView view;
		public CustomizationForm(GridView view) : base() {
			this.view = view;
			if(view.WorkAsLookup || ParentFormIsTopMost) 
				this.TopMost = true;
		}
		bool ParentFormIsTopMost {
			get { 
				Form parent = null;
				if(view != null && view.GridControl != null)
					parent = view.GridControl.FindForm();
				if(parent == null) return false;
				return parent.TopMost;
			}
		}
		protected override bool AllowSearchBox {
			get {
				return View != null ? View.OptionsCustomization.CustomizationFormSearchBoxVisible : base.AllowSearchBox;
			}
		}
		protected override string FormCaption {
			get { return GridLocalizer.Active.GetLocalizedString(GridStringId.CustomizationCaption); }
		}
		protected override void StopCustomization() {
			if (View != null) {
				GridView oldView = View;
				this.view = null;
				oldView.DestroyCustomization();
			}
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		public override Control ControlOwner { get { return GridControl; } }
		protected override DevExpress.XtraEditors.Customization.CustomizationListBoxBase CreateCustomizationListBox() {
			return new ColumnCustomizationListBox(this);
		}
		protected override Rectangle CustomizationFormBounds { get { return View.CustomizationFormBounds; } }
		protected override DevExpress.LookAndFeel.UserLookAndFeel ControlOwnerLookAndFeel { get { return View.ElementsLookAndFeel; } }
		public virtual GridControl GridControl { get { return View.GridControl; } }
		public virtual GridView View { get { return view; } }
		protected virtual GridViewInfo ViewInfo { get { return View.ViewInfo as GridViewInfo; } }
	}
}
