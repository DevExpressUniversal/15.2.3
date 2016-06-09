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
using DevExpress.Utils;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.Forms;
namespace DevExpress.XtraNavBar.ViewInfo {
	public class NavBarDragDropEventArgs : DragEventArgs {
		NavBarGroup group;
		int insertPosition;
		public NavBarDragDropEventArgs(NavBarGroup group, int insertPosition, IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect) : 
						base(data, keyState, x, y, allowedEffect, effect) {
			this.group = group;
			this.insertPosition = insertPosition;
		}
		public NavBarGroup Group { get { return group; } }
		public int InsertPosition {
			get { return insertPosition; }
		}
	}
	public delegate void NavBarDragDropEventHandler(object sender, NavBarDragDropEventArgs e);
	public class NavBarDragDropInfo : IDisposable {
		protected enum DragOverTarget { None, Group, UpButton, DownButton, ScrollUpArea, ScrollDownArea};
		NavBarViewInfo viewInfo;
		NavBarHitInfo startInfo;
		Timer groupExpandTimer;
		NavBarGroup groupToExpand;
		DragOverTarget timerTarget;
		NavPaneForm form;
		public NavBarDragDropInfo(NavPaneForm form, NavBarHitInfo startInfo)
			: this(form.NavBar.ViewInfo, startInfo) {
			this.form = form;
		}
		public NavBarDragDropInfo(NavBarViewInfo viewInfo, NavBarHitInfo startInfo) {
			this.viewInfo = viewInfo;
			this.startInfo = startInfo;
			this.groupToExpand = null;
			this.timerTarget = DragOverTarget.None;
			this.groupExpandTimer = new Timer();
			this.groupExpandTimer.Tick += new EventHandler(OnGroupExpandTimerTick);
		}
		public virtual void Dispose() {
			this.groupExpandTimer.Tick -= new EventHandler(OnGroupExpandTimerTick);
			this.groupExpandTimer.Dispose();
			ViewInfo.DropTarget = new NavBarDropTargetArgs();
		}
		void OnGroupExpandTimerTick(object sender, EventArgs e) {
			int delta = 0;
			NavBarHitInfo hInfo = ViewInfo.CalcHitInfo(ViewInfo.MousePosition);
			NavBarGroup group = groupToExpand;
			DragOverTarget drag = GetTarget(ViewInfo.MousePosition), timer = timerTarget;
			ClearTimer();
			if(timer == DragOverTarget.None || timer != drag) return;
			switch(timer) {
				case DragOverTarget.Group :
					if(group != null && group == hInfo.Group) {
						if(ViewInfo.AllowExpandCollapse && hInfo.InGroupCaption && group.VisibleItemLinks.Count > 0)
							group.Expanded = !group.Expanded;
						else 
							group.Expanded = true;
					}
					break;
				case DragOverTarget.UpButton:
				case DragOverTarget.DownButton:
					delta = timer == DragOverTarget.UpButton ? -1 : 1;
					if(NavBar.ActiveGroup != null)
						NavBar.ActiveGroup.TopVisibleLinkIndex += delta;
					break;
				case DragOverTarget.ScrollUpArea:
				case DragOverTarget.ScrollDownArea:
					if(ViewInfo.AllowScrollBar) {
						delta = timer == DragOverTarget.ScrollUpArea ? -4 : 4;
						ViewInfo.TopY += delta;
					}
					break;
			}
		}
		protected void ClearTimer() {
			groupToExpand = null;
			timerTarget = DragOverTarget.None;
			groupExpandTimer.Stop();
		}
		public virtual bool IsLink {
			get { return StartInfo.InLink; }
		}
		public virtual object GetStartDragObject() {
			if(IsLink) return StartInfo.Link;
			return StartInfo.Group;
		}
		protected NavPaneForm NavPaneForm { get { return form; } }
		public virtual void Start() {
			Control ctrl = NavBar;
			if(NavPaneForm != null) ctrl = NavPaneForm;
			ctrl.DoDragDrop(GetStartDragObject(), DragDropEffects.Copy | DragDropEffects.Move);
			ViewInfo.DropTarget = new NavBarDropTargetArgs();
		}
		public NavBarViewInfo ViewInfo { get { return viewInfo; } }
		public NavBarControl NavBar { get { return ViewInfo.NavBar; } }
		public NavBarHitInfo StartInfo { get { return startInfo; } }
		public virtual void OnDragEnter(DragEventArgs e) {
			e.Effect = CalcEffect(e);
		}
		public virtual void OnDragLeave(EventArgs e) {
			ClearDropTarget(null);
		}
		public NavBarHitInfo CreateHitInfo() {
			if(NavPaneForm != null) return NavPaneForm.ViewInfo.CreateHitInfo();
			return ViewInfo.CreateHitInfo();
		}
		protected NavLinkInfoArgs GetLinkInfo(NavBarItemLink link) {
			if(NavPaneForm == null) return ViewInfo.GetLinkInfo(link);
			return NavPaneForm.ViewInfo.GetLinkInfo(link);
		}
		Point GetClientPoint(Point pt) {
			if(NavPaneForm != null) return NavPaneForm.PointToClient(pt);
			return NavBar.PointToClient(pt);
		}
		public virtual void OnDragOver(DragEventArgs e) {
			e.Effect = CalcEffect(e);
			NavBarGroup group = GetDestinationGroup(e);
			Point p = GetClientPoint(new Point(e.X, e.Y));
			DragOverTarget dragOver = GetTarget(p);
			if(dragOver != DragOverTarget.None) {
				if(dragOver != timerTarget) {
					timerTarget = dragOver;
					groupToExpand = group;
					groupExpandTimer.Stop();
					groupExpandTimer.Interval = GetTimerInterval(dragOver);
					groupExpandTimer.Start();
				} else {
					if(groupToExpand != group) {
						groupToExpand = group;
						if(timerTarget == DragOverTarget.Group) {
							groupExpandTimer.Stop();
							groupExpandTimer.Start();
						}
					}
				}
			}
			else {
				ClearTimer();
			}
			e.Effect = CalcEffect(e);
			if(e.Effect == DragDropEffects.None || group == null) {
				ClearDropTarget(e);
				return;
			}
			NavBarHitInfo hitInfo = CreateHitInfo();
			hitInfo.CalcHitInfo(p, null);
			bool afterLink = true;
			if(hitInfo.InLink) {
				NavLinkInfoArgs li = GetLinkInfo(hitInfo.Link);
				NavBarItemLink link = hitInfo.Link;
				if(li != null && CalcLinkPosition(li, p)) {
					int vIndex = link.Group.VisibleItemLinks.IndexOf(link);
					if(vIndex > hitInfo.Group.TopVisibleLinkIndex) {
						link = link.Group.VisibleItemLinks[vIndex - 1];
					} else {
						afterLink = false;
					}
				}
				SetDropTarget(e, new NavBarDropTargetArgs(hitInfo.Group, link, afterLink));
				return;
			}
			if(!hitInfo.InGroup) {
				ClearDropTarget(e);
				return;
			}
			NavBarDropTargetArgs targetArgs = new NavBarDropTargetArgs(hitInfo.Group, null, true);
			int vc = hitInfo.Group.VisibleItemLinks.Count;
			if(hitInfo.InGroupCaption) {
				if(vc > 0) {
					targetArgs.Link = hitInfo.Group.VisibleItemLinks[vc - 1];
					targetArgs.InsertAfter = true;
				}
			} else {
				if(hitInfo.Link != null) {
					for(int n = hitInfo.Group.TopVisibleLinkIndex; n < vc; n++) {
						NavBarItemLink link = hitInfo.Group.VisibleItemLinks[n];
						NavLinkInfoArgs li = ViewInfo.GetLinkInfo(hitInfo.Link);
						if(li.Bounds.Top < p.Y) {
							targetArgs.Link = link;
							targetArgs.InsertAfter = false;
						} 
						if(li.Bounds.Bottom > p.Y) {
							targetArgs.Link = link;
							targetArgs.InsertAfter = true;
						}
						if(li.Bounds.Top >= p.Y && li.Bounds.Bottom <= p.Y) {
							targetArgs.Link = link;
							targetArgs.InsertAfter = !CalcLinkPosition(li, p);
						}
					}
				}
			}
			SetDropTarget(e, targetArgs);
		}
		bool CalcLinkPosition(NavLinkInfoArgs li, Point p) {
			int height = li.Bounds.Height;
			int sy = li.Bounds.Y, dy = p.Y;
			if(li.Link.Group.GetShowAsIconsView()) {
				height = li.Bounds.Width;
				sy = li.Bounds.X;
				dy = p.X;
			}
			return (height / 2) > dy - sy;
		}
		protected virtual void ClearDropTarget(DragEventArgs e) {
			SetDropTarget(e, new NavBarDropTargetArgs());
		}
		protected virtual void SetDropTarget(DragEventArgs e, NavBarDropTargetArgs args) {
			NavBarItemLink link = GetDragLink(e);
			if(e != null) {
				if(link != null && args.Link != null && (Control.ModifierKeys & Keys.Control) == 0) { 
					if(link.Group == args.Link.Group && (link == args.Link || 
						(link.Group.VisibleItemLinks.IndexOf(link) == args.LinkInsertIndex))) {
						args = new NavBarDropTargetArgs();
						e.Effect = DragDropEffects.None;
					}
				}
				if(e.Effect != DragDropEffects.None) {
					RaiseOnDragOver(e, ref args);
				}
			}
			NavBarDropTargetArgs old = ViewInfo.DropTarget;
			ViewInfo.DropTarget = args;
			if(NavPaneForm != null && old == ViewInfo.DropTarget) {
				NavPaneForm.Invalidate();
			}
		}
		protected virtual NavBarDragDropEventArgs CreateDragDropArgs(DragEventArgs e, NavBarDropTargetArgs args) {
			return new NavBarDragDropEventArgs(args.Group, args.LinkInsertIndex, e.Data, e.KeyState, e.X, e.Y, e.AllowedEffect, e.Effect);
		}
		protected virtual void RaiseOnDragOver(DragEventArgs e, ref NavBarDropTargetArgs args) {
			NavBarDragDropEventArgs drag = CreateDragDropArgs(e, args);
			NavBar.RaiseNavDragOver(drag);
			e.Effect = drag.Effect;
			if(drag.Effect == DragDropEffects.None) {
				args = new NavBarDropTargetArgs();
			}
		}
		protected virtual void RaiseOnDragDrop(DragEventArgs e, NavBarDropTargetArgs args) {
			NavBarDragDropEventArgs drag = CreateDragDropArgs(e, args);
			NavBar.RaiseNavDragDrop(drag);
			e.Effect = drag.Effect;
		}
		protected virtual void OnDragDropCore(DragEventArgs e) {
			if(e.Effect == DragDropEffects.None) return;
			RaiseOnDragDrop(e, ViewInfo.DropTarget);
			if(e.Effect == DragDropEffects.None) return;
			NavBarGroup group = ViewInfo.DropTarget.Group;
			int index = ViewInfo.DropTarget.LinkInsertIndex;
			if(group == null) return;
			NavBarItemLink link = GetDragLink(e);
			if(link != null && link.NavBar == NavBar) {
				NavBarItemLink addedLink;
				if(index != -1) {
					addedLink = group.ItemLinks.Insert(index, link.Item) as NavBarItemLink;
				} else
					addedLink = group.ItemLinks.Add(link.Item);
				if(e.Effect == DragDropEffects.Move) 
					link.Dispose();
				UpdateNavPaneFormAfterDrop();
				if(addedLink != null) addedLink.Group.SelectedLink = addedLink;
			}
			ViewInfo.ResetAccessible();
		}
		protected void UpdateNavPaneFormAfterDrop() {
			NavPaneForm form = NavPaneForm == null? NavBar.NavPaneForm: NavPaneForm;
			if(form == null || !form.Visible) return;
			form.ViewInfo.IsReady = false;
			Size sz = form.CalcBestSize();
			if(form.Size != sz)
				form.Size = sz;
			else
				form.Invalidate();
		}
		public virtual void OnDragDrop(DragEventArgs e) {
			OnDragDropCore(e);
			ClearDropTarget(null);
		}
		public virtual void OnGiveFeedback(GiveFeedbackEventArgs e) {
			e.UseDefaultCursors = true;
		}
		protected virtual DragDropEffects CalcEffect(DragEventArgs e) {
			NavBarGroup group = GetDestinationGroup(e);
			NavBarItemLink link = GetDragLink(e);
			if(group == null) return DragDropEffects.None;
			if(link == null || link.NavBar != NavBar) {
				if(group == null) return DragDropEffects.None;
				if((group.GetDragDropFlags() & NavBarDragDrop.AllowOuterDrop) == 0) return DragDropEffects.None;
				return DragDropEffects.Copy;
			}
			if((group.GetDragDropFlags() & NavBarDragDrop.AllowDrop) == 0) return DragDropEffects.None;
			return (Control.ModifierKeys & Keys.Control) != 0  ? DragDropEffects.Copy : DragDropEffects.Move;
		}
		protected NavBarHitInfo CalcHitInfo(DragEventArgs e) {
			if(NavPaneForm != null) 
				return NavPaneForm.ViewInfo.CalcHitInfo(NavPaneForm.PointToClient(new Point(e.X, e.Y)));
			return ViewInfo.CalcHitInfo(NavBar.PointToClient(new Point(e.X, e.Y)));
		}
		protected NavBarItemLink GetDragLink(DragEventArgs e) {
			if(e == null) return null;
			NavBarItemLink link = GetDragDropObjectCore<NavBarItemLink>(e, typeof(NavBarItemLink));
			if(link == null && StartInfo.InLink) {
				Type linkType = StartInfo.Link.GetType();
				link = GetDragDropObjectCore<NavBarItemLink>(e, linkType);
			}
			return link;
		}
		protected TResult GetDragDropObjectCore<TResult>(DragEventArgs e, Type type) where TResult : class {
			if(e.Data.GetDataPresent(type)) {
				return e.Data.GetData(type) as TResult;
			}
			return default(TResult);
		}
		protected NavBarGroup GetDestinationGroup(DragEventArgs e) {
			NavBarHitInfo hitInfo = CalcHitInfo(e);
			return hitInfo.Group;
		}
		protected virtual int GetTimerInterval(DragOverTarget target) {
			switch(target) {
				case DragOverTarget.DownButton :
				case DragOverTarget.UpButton :
					return 500;
				case DragOverTarget.Group :
					return 1000;
				case DragOverTarget.ScrollDownArea :
				case DragOverTarget.ScrollUpArea :
					return 40;
			}
			return 1000;
		}
		protected virtual DragOverTarget GetTarget(Point p) {
			const int scrollArea = 10;
			NavBarHitInfo hitInfo;
			if(NavPaneForm != null) {
				hitInfo = NavPaneForm.ViewInfo.CalcHitInfo(p);
			}
			else {
				hitInfo = ViewInfo.CalcHitInfo(p);
			if(ViewInfo.AllowScrollBar) {
				if(p.Y < scrollArea) return DragOverTarget.ScrollUpArea;
				if(NavBar.ClientRectangle.Bottom - p.Y < scrollArea) return DragOverTarget.ScrollDownArea;
			}
			}
			if(hitInfo.HitTest == NavBarHitTest.UpButton) return DragOverTarget.UpButton;
			if(hitInfo.HitTest == NavBarHitTest.DownButton) return DragOverTarget.DownButton;
			if(hitInfo.Group != null) return DragOverTarget.Group;
			return DragOverTarget.None;
		}
	}
	public class NavBarDropTargetArgs {
		NavBarGroup group;
		NavBarItemLink link;
		bool insertAfter;
		public NavBarDropTargetArgs() : this(null, null, true) {
		}
		public NavBarDropTargetArgs(NavBarGroup group, NavBarItemLink link) : this(group, link, true) {
		}
		public NavBarDropTargetArgs(NavBarGroup group, NavBarItemLink link, bool insertAfter) {
			this.group = group;
			this.link = link;
			this.insertAfter = insertAfter;
		}
		public NavBarDropTargetArgs(NavBarGroup group, bool insertAfter) : this(group, null, insertAfter) {
		}
		public virtual void Clear() {
			this.group = null;
			this.link = null;
			this.insertAfter = true;
		}
		public NavBarGroup Group { 
			get { return group; }
			set {
				group = value;
			}
		}
		public NavBarItemLink Link {
			get { return link; }
			set {
				link = value;
			}
		}
		public bool InsertAfter {
			get { return insertAfter; }
			set {
				insertAfter = value;
			}
		}
		public int LinkInsertIndex {
			get {
				if(Group == null) return -1;
				if(Link != null) {
					int linkIndex = Group.ItemLinks.IndexOf(Link);
					if(linkIndex != -1) {
						if(InsertAfter) linkIndex ++;
					}
					return linkIndex;
				}
				return -1;
			}
		}
		public virtual bool IsEquals(NavBarItemLink link) {
			if(link == null) return false;
			if(Group == null) return false;
			return (this.Group == link.Group && this.Link == link);
		}
		public virtual bool IsEquals(NavBarDropTargetArgs e) {
			if(e == null) return false;
			if(e == this) return true;
			return (e.Group == Group && e.Link == Link && e.InsertAfter == InsertAfter);
		}
	}
}
