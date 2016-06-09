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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Localization;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraReports.UserDesigner.Native {
	abstract class ToolboxLogicBase {
		#region inner classes
		struct DragDropData {
			public static DragDropData Create() {
				DragDropData result = new DragDropData();
				result.DownPoint = new Point(int.MinValue, int.MinValue);
				;
				return result;
			}
			Point downPoint;
			ToolboxItem data;
			public Point DownPoint {
				get { return downPoint; }
				set { downPoint = value; }
			}
			public ToolboxItem Data {
				get { return data; }
				set { data = value; }
			}
		};
		#endregion
		protected BarManager barManager;
		DragDropData dragDropData = DragDropData.Create();
		protected abstract Control Control {
			get;
		}
		public ToolboxLogicBase(BarManager barManager) {
			this.barManager = barManager;
		}
		void ClearDragData() {
			if(Control != null) {
				barManager.SelectLink(null);
				Control.MouseMove -= CustomBarControl_MouseMove;
				Control.MouseUp -= CustomBarControl_MouseUp;
			}
			dragDropData.Data = null; 
		}
		public virtual void PrepareDragData(BarItemLink link) {
			if(link == null || link.Item == null)
				return;
			dragDropData.Data = link.Item.Tag as ToolboxItem;
			if(Control != null && dragDropData.Data != null) {
				Control.MouseMove += CustomBarControl_MouseMove;
				Control.MouseUp += CustomBarControl_MouseUp;
				dragDropData.DownPoint = Control.PointToClient(Cursor.Position);
			}
		}
		void CustomBarControl_MouseUp(object sender, MouseEventArgs e) {
			ClearDragData();
		}
		void CustomBarControl_MouseMove(object sender, MouseEventArgs e) {
			if(!e.Button.IsLeft() || dragDropData.Data == null
				|| (Math.Abs(e.X - dragDropData.DownPoint.X) < SystemInformation.DragSize.Width
				&& Math.Abs(e.Y - dragDropData.DownPoint.Y) < SystemInformation.DragSize.Height))
				return;
			DoDragDrop(new DataObject(dragDropData.Data));
			ClearDragData();
		}
		protected virtual void DoDragDrop(DataObject data) {
			Control.DoDragDrop(data, DragDropEffects.Copy);
		}
	}
	class ToolboxRibbonrLogic : ToolboxLogicBase {
		RibbonControl ribbon;
		protected override Control Control {
			get {
				return ribbon;
			}
		}
		public override void PrepareDragData(BarItemLink link) {
			if(link == null || link.Item == null)
				return;
			BarCheckItem currentItem = (BarCheckItem)link.Item;
			currentItem.Checked = true;
			base.PrepareDragData(link);
		}
		public ToolboxRibbonrLogic(RibbonControl ribbon)
			: base(ribbon.Manager) {
				this.ribbon = ribbon;
		}
		protected override void DoDragDrop(DataObject data) {
			base.DoDragDrop(data);
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(Control.Handle, DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEMOVE, 0, new IntPtr(0));
		}
	}
	class ToolboxBarLogic : ToolboxLogicBase {
		Bar bar;
		Control control;
		List<BarItem> addedItems = new List<BarItem>();
		public BarCheckItem CursorBarItem {
			get {
				return addedItems.Count > 0 && addedItems[0].Tag == null ?
					addedItems[0] as BarCheckItem : null;
			}
		}
		XRDesignBarManager BarManager { get { return (XRDesignBarManager)barManager; } }
		protected override Control Control {
			get {
				if(control == null || control.IsDisposed) {
					PropertyInfo barControlProperty = bar.GetType().GetProperty("BarControl", BindingFlags.Instance | BindingFlags.NonPublic);
					if(barControlProperty != null)
						control = barControlProperty.GetValue(bar, null) as CustomBarControl;
				}
				return control;
			}
		}
		public ToolboxBarLogic(XRDesignBarManager barManager, Bar bar) : base(barManager) {
			this.bar = bar;
		}
		public void ApplyBarItems() {
			barManager.PressedLinkChanged += Manager_PressedLinkChanged;
			bar.DockChanged += XRToolboxBar_DockChanged;
			foreach(BarItemLink link in bar.ItemLinks)
				addedItems.Add(link.Item);
		}
		public void SetBarItemsEnabled(bool value) {
			foreach(BarItemLink link in bar.ItemLinks)
				link.Item.Enabled = value;
		}
		public void ClearBarItems() {
			barManager.PressedLinkChanged -= Manager_PressedLinkChanged;
			bar.DockChanged -= XRToolboxBar_DockChanged;
			foreach(BarItem item in addedItems)
				item.Dispose();
			addedItems.Clear();
			bar.ItemLinks.Clear();
		}
		void XRToolboxBar_DockChanged(object sender, EventArgs e) {
			control = null;
		}
		void Manager_PressedLinkChanged(object sender, HighlightedLinkChangedEventArgs e) {
			if(e.Link == null || e.Link.Bar != this.bar)
				return;
			PrepareDragData(e.Link);
		}
		public void UnpressAllItems() {
			UnpressItemsExcept(null);
		}
		public void UnpressItemsExcept(BarCheckItem barItem) {
			foreach(BarItem item in addedItems) {
				if(item is BarCheckItem)
					((BarCheckItem)item).Checked = item == barItem;
			}
		}
	}
}
