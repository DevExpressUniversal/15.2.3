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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Utils;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.Persistent;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using DevExpress.XtraBars.InternalItems;
using System.IO;
namespace DevExpress.XtraBars.Utils {
	public class BarLinkDragManager : IDisposable {
		BarManager manager;
		bool isDragging, fireDoDragging, useDefaultCursors;
		object dragObject;
		Point dragStartPoint;
		BarItemLink sizingLink;
		Rectangle sizingRectangle;
		public BarLinkDragManager(BarManager manager) {
			this.manager = manager;
			this.sizingRectangle = Rectangle.Empty;
			this.useDefaultCursors = this.fireDoDragging = this.isDragging = false;
			this.dragObject = null;
			this.dragStartPoint = Point.Empty;
			this.sizingLink = null;
		}
		public virtual void Dispose() {
		}
		protected virtual Rectangle SizingRectangle { get { return sizingRectangle; } }
		public virtual bool FireDoDragging { get { return fireDoDragging; } set { fireDoDragging = value; } }
		public virtual bool UseDefaultCursors { get { return useDefaultCursors; } set { useDefaultCursors = value; } }
		public virtual BarItemLink SizingLink { get { return sizingLink; } }
		public virtual Point DragStartPoint { get { return dragStartPoint; } }
		internal virtual void SetDragObject(object obj) { this.dragObject = obj; }
		public virtual object DragObject { get { return dragObject; } }
		public virtual bool IsLinkSizing { get { return SizingLink != null; } }
		public virtual BarManager Manager { get { return manager; } }
		public virtual bool IsDragging { get { return isDragging; } set { isDragging = value; } }
		protected virtual ArrayList DragCursors { get { return Manager.GetController().DragCursors; } }
		DragHitInfo dragHitInfo;
		internal DragHitInfo HitInfo { get { return dragHitInfo; } set { dragHitInfo = value; } }
		public virtual void DoDragging(Control control, MouseEventArgs e) {
			CustomLinksControl barControl = control as CustomLinksControl;
			if(!IsDragging) return;
			DragHitInfo hitInfo = CalcDragHitInfo(barControl, Control.MousePosition, dragObject);
			HitInfo = hitInfo;
			if(!UseDefaultCursors)
				Cursor.Current = (Cursor)Manager.GetController().DragCursors[hitInfo.Cursor];
			if(hitInfo.Link == Manager.SelectionInfo.CustomizeSelectedLink && hitInfo.Cursor != BarManager.CopyCursor) {
				Manager.SelectionInfo.DropSelectedLink = null;
			} else
				Manager.SelectionInfo.DropSelectedLink = hitInfo.Link;
			if(hitInfo.Link != null) {
				LinkDropTargetEnum markStyle = LinkDropTargetEnum.Before;
				if(barControl != null) {
					CustomControlViewInfo cvi = barControl.ViewInfo as CustomControlViewInfo;
					if(cvi != null) {
						BarLinkViewInfo vi = cvi.GetLinkViewInfo(hitInfo.Link, LinkViewInfoRange.Next);
						if(vi == null || (vi.Link.Alignment == BarItemLinkAlignment.Right && hitInfo.Link.Alignment != BarItemLinkAlignment.Right) || (vi.Link.GetBeginGroup() || vi.Link.Item.IsPrivateItem)) {
							if(hitInfo.AfterCenter) markStyle = LinkDropTargetEnum.After;
						}
					}
				}
				Manager.SelectionInfo.DropSelectStyle = markStyle;
			}
		}
		public virtual void StartLinkSizing(Control barControl, BarItemLink link, Point startPosition) {
			this.dragStartPoint = startPosition;
			this.sizingLink = link;
			barControl.Capture = true;
			link.Invalidate();
			Application.DoEvents();
			DoLinkSizing(link, startPosition, true);
		}
		public virtual void StopLinkSizing(bool acceptNewSize) {
			if(!IsLinkSizing) {
				this.sizingLink = null;
				return;
			}
			BarItemLink link = SizingLink;
			this.sizingLink = null;
			DrawSizingFrame(link);
			if(link != null) {
				if(acceptNewSize) {
					int startWidth = CalcSizingRectangle(link, this.DragStartPoint, this.DragStartPoint).Width;
					int endWidth = CalcSizingRectangle(link, this.DragStartPoint, Control.MousePosition).Width;
					int finalWidth = link.ScreenBounds.Width + (endWidth - startWidth);
					if(finalWidth < link.MinWidth) finalWidth = link.MinWidth;
					link.Width = finalWidth; 
				}
				link.Invalidate();
			}
			this.dragStartPoint = BarManager.zeroPoint;
		}
		protected Rectangle CalcSizingRectangle(BarItemLink link, Point startPoint, Point p) {
			bool dragLeft = CalcLinkHitInfo(link, startPoint).HitTest == LinkHitTest.LeftEdge;
			Rectangle r = link.LinkViewInfo.SelectRect;
			p = link.ScreenToLinkPoint(p);
			if(dragLeft) {
				r.X = p.X; 
				r.Width = link.LinkViewInfo.SelectRect.Right - r.X;
				if(r.Width < link.MinWidth) {
					int d = link.MinWidth - r.Width;
					r.Width += d;
					r.X -= d;
				}
			} else {
				r.Width = p.X - link.LinkViewInfo.SelectRect.X;
				if(r.Width < link.MinWidth) r.Width = link.MinWidth;
			}
			r.Location = link.LinkPointToScreen(r.Location);
			return r;
		}
		public virtual void DoLinkSizing(BarItemLink link, Point p, bool alwaysRedraw) {
			if(p == this.DragStartPoint && !alwaysRedraw) return;
			if(link.LinkViewInfo == null) return;
			DrawSizingFrame(link); 
			Rectangle r = CalcSizingRectangle(link, DragStartPoint, p);
			this.sizingRectangle = r;
			DrawSizingFrame(link);
			this.sizingRectangle = r;
		}
		public virtual void DrawSizingFrame(BarItemLink link) {
			if(SizingRectangle.IsEmpty) return;
			this.sizingRectangle.Height --;
			this.sizingRectangle.Offset(1, 1);
			System.Windows.Forms.ControlPaint.DrawReversibleFrame(this.SizingRectangle, Manager.DrawParameters.StateAppearance(BarAppearance.Bar).Normal.BackColor, FrameStyle.Thick);
			this.sizingRectangle = Rectangle.Empty;
		}
		protected internal virtual BarDragControl CreateDragControl(Control ctrl) { return new BarDragControl(this); }
		protected virtual bool ShouldStartDragging() {
			return !Manager.SelectionInfo.ModalTextBoxActive;
		}
		protected virtual void OnStartDragging(Control ABarControl, MouseEventArgs e, object dragObject, Control ctrl) {
			if(Manager.Helper.CustomizationManager.MenuEditor != null && Manager.Helper.CustomizationManager.MenuEditor.EditObject == dragObject) Manager.Helper.CustomizationManager.MenuEditor.HideMenu();
		}
		protected virtual void OnEndDragging(Control ABarControl, MouseEventArgs e, object dragObject, Control ctrl) {
			Manager.FireManagerChanged();
		}
		public virtual void StartDragging(Control ABarControl, MouseEventArgs e, object dragObject, Control ctrl) {
			if(!ShouldStartDragging()) return;
			OnStartDragging(ABarControl,e, dragObject, ctrl);
			this.isDragging = true;
			this.dragObject = dragObject;
			this.dragStartPoint = Control.MousePosition;
			ctrl.Capture = false;
			if(dragObject is BarItemLink)
				(dragObject as BarItemLink).OnLinkActionCore(BarLinkAction.StartDrag, null);
			BarDragControl dragControl = CreateDragControl(ctrl);
			if(dragControl.Handle != IntPtr.Zero) {
			}
			FireDoDragging = true;
			DragDropEffects effects = dragControl.DoDragDrop(dragObject, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Scroll);
			StopDragging(null, effects, false);
			dragControl.Dispose();
			OnEndDragging(ABarControl, e, dragObject, ctrl);
		}
		public virtual void StopDragging(Control barControl, DragDropEffects effects, bool cancelDrag) {
			if(!IsDragging) return;
			this.isDragging = false;
			Cursor.Current = Cursors.Default;
			BarItemLink selectedLink = (dragObject is BarItemLink ? dragObject as BarItemLink : null); 
			LinkDropTargetEnum mark = Manager.SelectionInfo.DropSelectStyle;
			Manager.SelectionInfo.CustomizeSelectedLink = null;
			Manager.SelectionInfo.DropSelectStyle = LinkDropTargetEnum.None;
			if(cancelDrag) return;
			DragHitInfo hitInfo = CalcDragHitInfo(barControl, Control.MousePosition, dragObject);
			if(hitInfo == null) return;
			BarButtonItem item = dragObject as BarButtonItem;
			if(item != null && hitInfo.Link != null && !item.CanAddItemToMenu(hitInfo.Link)) {
				MessageBox.Show("Recursion detected. Cannot add the item to the menu owned by this item.");
				return;
			}
			if(selectedLink != null && hitInfo.Link == selectedLink && hitInfo.Cursor != BarManager.CopyCursor) {
				Point p = Control.MousePosition;
				p.Offset(-DragStartPoint.X, -DragStartPoint.Y);
				if(selectedLink.IsVertical || selectedLink.IsLinkInMenu) {
					if(Math.Abs(p.Y) > 3) {
						hitInfo.Link.BeginGroup = p.Y > 0;
					}
				}
				else {
					if(Math.Abs(p.X) > 3) {
						hitInfo.Link.BeginGroup = p.X > 0;
					}
				}
			} else {
				if(selectedLink != null && hitInfo.Cursor == BarManager.NoDropCursor) {
					if((Control.ModifierKeys & Keys.Control) == 0)
						RemoveLink(selectedLink, selectedLink.GetBeginGroup());
				} else {
					if(hitInfo.Link != null) {
						CopyLink(dragObject, hitInfo.Link, hitInfo.Cursor != BarManager.CopyCursor, mark == LinkDropTargetEnum.After);
					}
				}
			}
		}
		#region dragging code
		protected virtual void RemoveLink(BarLinksHolder holder, BarItemLink link) {
			holder.RemoveLink(link);
		}
		protected void RemoveLink(BarItemLink link, bool prevBeginGroup) {
			BarLinksHolder itemHolder = link.Holder;
			if(itemHolder != null) {
				try {
					if(prevBeginGroup) {
						int index = itemHolder.ItemLinks.IndexOf(link);
						if(index != -1 && index + 1 < itemHolder.ItemLinks.Count) {
							BarItemLink nextLink = itemHolder.ItemLinks[index + 1];
							nextLink.BeginGroup = true;
						}
					}
					RemoveLink(itemHolder, link);
				} finally {
				}
			}
			Manager.FireManagerChanged();
		}
		protected virtual BarLinksHolder GetLinkHolder(BarItemLink link) { return link.Holder; }
		protected virtual int GetLinkIndex(BarLinksHolder holder, BarItemLink link) { return holder.ItemLinks.IndexOf(link); }
		protected virtual BarItemLink InsertItem(BarLinksHolder holder, BarItemLink beforeLink, BarItem item) {
			return holder.InsertItem(beforeLink, item);
		}
		protected virtual void CopyLink(object dragObject, BarItemLink dest, bool move, bool afterDest) {
			BarItem item = null;
			BarItemLink dragLink = dragObject as BarItemLink;
			if(dragObject is BarItem) item = dragObject as BarItem;
			if(dragObject is BarItemLink) item = (dragObject as BarItemLink).Item;
			BarLinksHolder itemHolder = GetLinkHolder(dest);
			bool dragBeginGroup = dragLink != null ? dragLink.GetBeginGroup() : false;
			BarItemLink finalLink = null;
			if(itemHolder != null) {
				if(afterDest) {
					int i = GetLinkIndex(itemHolder, dest);
					if(i < 0 || i + 1 >= itemHolder.ItemLinks.Count)
						finalLink = itemHolder.AddItem(item);
					else
						finalLink = itemHolder.InsertItem(itemHolder.ItemLinks[i + 1], item);
				} else {
					finalLink = InsertItem(itemHolder, dest, item);
					if(dest.GetBeginGroup()) {
						finalLink.BeginGroup = true;
						dest.BeginGroup = false;
					}
				}
			} 
			if(dragLink != null && finalLink != null) {
				finalLink.Assign(dragLink);
				finalLink.LayoutChanged();
			}
			if(move && (dragObject is BarItemLink))
				RemoveLink(dragObject as BarItemLink, dragBeginGroup);
			Manager.FireManagerChanged();
		}
		internal virtual void ItemOnQueryContinueDrag(QueryContinueDragEventArgs e, Control barControl) {
			if(e.EscapePressed) {
				StopDragging(null, DragDropEffects.None, true);
				e.Action = DragAction.Cancel;
			} else {
				if(FireDoDragging)
					DoDragging(barControl, new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
			}
		}
		internal virtual void ItemOnGiveFeedback(GiveFeedbackEventArgs e, Control barControl) {
			e.UseDefaultCursors = UseDefaultCursors;
			if(FireDoDragging) {
				DoDragging(barControl, new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
			}
		}
		#endregion
		protected internal class DragHitInfo {
			public BarItemLink Link;
			public int Cursor;
			public bool AfterCenter;
			internal DragHitInfo() {
				Link = null;
				Cursor = 0;
				AfterCenter = false; 
			}
			public bool IsEqual(DragHitInfo obj) {
				if(Link == obj.Link && Cursor == obj.Cursor) return true;
				return false;
			}
		}
		public virtual bool CheckCanDropIntoItem(BarItem contItem, object dragObject) {
			BarItem item = dragObject as BarItem;
			if(item == null) return true;
			if(item.ContainsItem(contItem)) return false;
			return true;
		}
		protected virtual bool CheckCanDropInto(Control control, object dragObject) {
			CustomLinksControl barControl = control as CustomLinksControl;
			if(barControl == null) return false;
			BarItem item = dragObject as BarItem;
			if(dragObject is BarItemLink) item = ((BarItemLink)dragObject).Item;
			if(item == null) return false;
			CustomPopupBarControl pc = barControl as CustomPopupBarControl;
			if(pc == null) return true; 
			if(pc.OwnerLink == null) return true;
			return CheckCanDropIntoItem(pc.OwnerLink.Item, item);
		}
		protected virtual DragHitInfo CalcDragHitInfo(Control control, Point p, object dragObject) {
			CustomLinksControl barControl = control as CustomLinksControl;
			DragHitInfo hitInfo = new DragHitInfo();
			hitInfo.Cursor = (Control.ModifierKeys & Keys.Control) != 0 || dragObject is BarItem? 
				BarManager.CopyCursor : BarManager.DragCursor;
			if(barControl == null) {
				hitInfo.Cursor = BarManager.NoDropCursor;
				return hitInfo;
			}
			CustomBarControl bc = barControl as CustomBarControl;
			if(!Manager.IsDesignMode && bc != null && bc.Bar != null && bc.Bar.OptionsBar.DisableCustomization) {
				hitInfo.Cursor = BarManager.NoDropCursor;
				return hitInfo;
			}
			BarItemLink link = barControl.GetLinkByPoint(p, true);
			if(link != null) {
				if(link.Item.IsPrivateItem) {
					if(!(link.Item is BarEmptyItem || link.Item is BarDesignTimeItem)) link = null;
				}
			}
			if(link != null) {
				if(!CheckCanDropInto(barControl, dragObject)) link = null;
			}
			if(link != null) {
				BarItemLink dragLink = dragObject as BarItemLink;
				if(dragLink != null && dragLink.ContainsSubItemLink(link)) link = null;
			}
			if(link != null) {
				BarLinkViewInfo vi = link.LinkViewInfo;
				if(vi != null) {
					Point pp = barControl.PointToClient(p);
					if(link.IsVertical || (link.IsLinkInMenu && vi.DrawMode != BarLinkDrawMode.InMenuGallery))
						hitInfo.AfterCenter = pp.Y > vi.Bounds.Y + vi.Bounds.Height / 2;
					else
						hitInfo.AfterCenter = pp.X > vi.Bounds.X + vi.Bounds.Width / 2;
				}
			}
			hitInfo.Link = link;
			if(link == null) {
				hitInfo.Cursor = BarManager.NoDropCursor;
			}
			return hitInfo;
		}
		public object GetDraggingObject(IDataObject data) {
			string[] s = data.GetFormats();
			object obj;
			if(s != null) {
				for(int i = 0; i < s.Length; i++) { 
					obj = data.GetData(s[i]);
					if(!(obj is MemoryStream)) return obj;
				}
			}
			return null;
		}
		public virtual bool CanAcceptDragObject(IDataObject data) {
			object obj = GetDraggingObject(data);
			if(obj is BarItem) {
				if((obj as BarItem).Manager == Manager) return true;
			}
			if(obj is BarItemLink) {
				BarItemLink link = obj as BarItemLink;
				if(link.Manager == Manager) return true;
			}
			return false;
		}
		public LinkHitInfo CalcLinkHitInfo(BarItemLink link, Point screenPoint) {
			LinkHitInfo hitInfo = new LinkHitInfo();
			hitInfo.Position = screenPoint;
			if(link.LinkViewInfo == null || !link.Visible) return hitInfo;
			Point p = link.ScreenToLinkPoint(screenPoint);
			if(link.LinkViewInfo.Bounds.Contains(p)) {
				hitInfo.HitTest = LinkHitTest.Body;
				if(link.CanResize) {
					if(p.X - link.LinkViewInfo.Bounds.X < 6)
						hitInfo.HitTest = LinkHitTest.LeftEdge;
					if(link.LinkViewInfo.Bounds.Right - p.X < 6) 
						hitInfo.HitTest = LinkHitTest.RightEdge;
				}
			}
			return hitInfo;
		}
		protected internal class BarDragControl : Control {
			private BarLinkDragManager dragManager;
			public BarLinkDragManager DragManager { get { return dragManager; } }
			internal BarDragControl(BarLinkDragManager dragManager) {
				this.dragManager = dragManager;
			}
			public virtual Control Control { get { return null; } }
			protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
				DragManager.ItemOnQueryContinueDrag(e, Control);
			}
			protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
				DragManager.ItemOnGiveFeedback(e, Control);
				base.OnGiveFeedback(e);
			}
		}
	}
}
