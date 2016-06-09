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
using DevExpress.Utils.Controls;
using DevExpress.Utils.Win;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.Customization;
using System.ComponentModel;
namespace DevExpress.XtraLayout.Handlers {
	public class LayoutControlCustomizeHandler : LayoutControlHandler {
		Point screenPoint;
		public LayoutControlCustomizeHandler(ILayoutControl control):base(control) { }
		protected virtual LayoutItemDragController CreateLayoutItemDragController(int X, int Y) {
			return new LayoutItemDragController(Item, Group, new Point(X, Y));
		}
		protected Rectangle layoutControlScreenRectangle {
			get {
				return new Rectangle(Owner.Control.PointToScreen(Point.Empty), Owner.Size);
			}
		}
		protected Point controlPoint {
			get {
				return Owner.Control.PointToClient(screenPoint);
			}
		}
		protected void CalculateEventArgs(object sender, MouseEventArgs mea) {
			screenPoint = ((Control)sender).PointToScreen(mea.Location);
		}
		protected bool IsOnLayoutControl {
			get {
				return layoutControlScreenRectangle.Contains(screenPoint) ;}
		}
		protected bool IsOnCustomizationWindow {
			get {return !IsOnLayoutControl;}
		}
		protected void ProcessDraggingItemFromCustomizationForm() {
			if(Item == null) return;
			Size size = Item.Size;
			Group.BeginChangeUpdate();
			Item.Owner = Owner;
			MouseEventArgs newMouseEvent = new MouseEventArgs(MouseButtons.Left, 1, controlPoint.X, controlPoint.Y, 0);
			base.OnMouseUp(newMouseEvent);
			if(Item != null && Item.Parent != null) Item.OnItemRestoredCore();
			Group.EndChangeUpdate();
			EmptySpaceItem emptySpaceItem = Item as EmptySpaceItem;
			if(emptySpaceItem != null && !Owner.DesignMode && !emptySpaceItem.IsHidden) {
				emptySpaceItem.Name = Owner.GetUniqueName(emptySpaceItem);
			}
		}
		protected internal BaseLayoutItem Item{
			get { return RootHandlerHelper.StartHitTest != null ? RootHandlerHelper.StartHitTest.Item : null;} 
		}
		protected bool IsDragItemHidden {
			get {
				if(DragDropDispatcherFactory.Default.DragItem == null) return false;
				return DragDropDispatcherFactory.Default.DragItem.IsHidden;
			}
		}
		public override bool OnMouseUp(object sender, MouseEventArgs e) {
			if(sender != null) {
				CalculateEventArgs(sender, e);
				if(sender is LayoutControl && !IsDragItemHidden) {
					if(IsOnLayoutControl || Owner.Control.Capture) base.PerformControlActions(EventType.MouseUp, e);
				}
			}
			return false;
		}
		public override bool OnMouseDown(object sender, MouseEventArgs e) {
			if(sender != null) {
				CalculateEventArgs(sender, e);
				if(sender is LayoutControl) {
					if(IsOnLayoutControl) { base.PerformControlActions(EventType.MouseDown, e); }
				}
			}
			return false;
		}
		public override void Dispose() {
			base.Dispose();
		}
		public override void OnMouseMove(object sender, MouseEventArgs e) {
			if(sender != null && e != null) {
				CalculateEventArgs(sender, e);
				if(IsOnLayoutControl || (Owner != null && Owner.Control != null &&Owner.Control.Capture)) {
					MouseEventArgs newMouseEvent  = new MouseEventArgs(e.Button, 1, controlPoint.X, controlPoint.Y,0);
					base.OnMouseMove(newMouseEvent);
				}
			}
		}
	}
}
