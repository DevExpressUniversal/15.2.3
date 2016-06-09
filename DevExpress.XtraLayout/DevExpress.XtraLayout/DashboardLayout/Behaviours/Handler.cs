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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;
using DevExpress.XtraLayout;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraEditors;
using System.Reflection;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Helpers;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraLayout.Handlers;
using DevExpress.Utils.Controls;
using System.Diagnostics;
namespace DevExpress.XtraDashboardLayout {
	public class DashboardCustomizeHandler :LayoutControlCustomizeHandler {
		public DashboardCustomizeHandler(ILayoutControl control) : base(control) { }
	}
	public class BaseBehaviour :IDisposable {
		protected List<Glyph> glyphs;
		protected AdornerWindowHandler owner;
		public BaseBehaviour(AdornerWindowHandler handler) {
			owner = handler;
			glyphs = new List<Glyph>();
		}
		public virtual Glyph GetGlyphAtPoint(Point p) {
			foreach(Glyph g in glyphs) {
				if(g.Bounds.Contains(p)) {
					return g;
				}
			}
			return null;
		}
		public virtual bool ProcessEvent(EventType eventType, MouseEventArgs e) {
			return false;
		}
		public virtual bool ProcessEvent(EventType eventType, object sender, MouseEventArgs e, KeyEventArgs key) {
			return ProcessEvent(eventType, e);
		}
		public virtual void Invalidate() {
			glyphs.Clear();
		}
		public virtual void Paint(Graphics g) {
			foreach(Glyph glyph in glyphs) {
				glyph.Paint(g);
			}
		}
		public virtual void Dispose() {
		}
	}
	public enum AdornerWindowHandlerStates { Dragging, Sizing, Normal, LostFocus }
	public class AdornerWindowHandler {
		static bool IsLayoutChangingState(AdornerWindowHandlerStates state) {
			return state == AdornerWindowHandlerStates.Sizing || state == AdornerWindowHandlerStates.Dragging;
		}
		public AdornerWindowHandler(DashboardLayoutControl lc) {
			owner = lc;
			behaviours = new List<BaseBehaviour>();
			PopulateBehaviours();
			InvalidateOnTimer = false;
		}
		BaseLayoutItem selectedItem = null;
		BaseLayoutItem hotTrackedItem = null;
		AdornerWindowHandlerStates currentState = AdornerWindowHandlerStates.Normal;
		DashboardLayoutControl owner;
		List<BaseBehaviour> behaviours;
		internal bool InvalidateOnTimer { get; set; }
		public DashboardLayoutControl Owner {
			get {
				return owner;
			}
		}
		public void SetCursor(Cursor cursor) {
			Owner.SetCursor(cursor);
		}
		public BaseLayoutItem SelectedItem {
			get {
				return selectedItem;
			}
			set {
				if(selectedItem == value) return;
				selectedItem = value;
				if(value != null) ((ILayoutControl)owner).FakeFocusContainer.Focus();
				try {
					Owner.Root.StartChangeSelection();
					Owner.Root.ClearSelection();
					if(selectedItem != null)
						selectedItem.Selected = true;
					Invalidate();
					owner.Invalidate();
				} finally {
					Owner.Root.EndChangeSelection();
				}
			}
		}
		public BaseLayoutItem HotTrackedItem {
			get {
				return hotTrackedItem;
			}
			set {
				if(hotTrackedItem != value) {
					hotTrackedItem = value;
					Invalidate();
					owner.Invalidate();
				}
			}
		}
		public AdornerWindowHandlerStates State {
			get {
				return currentState;
			}
			set {
				if(currentState != value) {
					AdornerWindowHandlerStates previousState = currentState;
					currentState = value;
					Invalidate();
					OnStateChanged(previousState);
				}
			}
		}
		private void OnStateChanged(AdornerWindowHandlerStates previousState) {
			if(!IsLayoutChangingState(previousState) && IsLayoutChangingState(currentState))
				owner.RaiseDashboardLayoutChanging();
			else if(IsLayoutChangingState(previousState) && !IsLayoutChangingState(currentState))
				owner.RaiseDashboardLayoutChanged();
		}
		DraggingBehaviour draggingBehaviourCore = null;
		public DraggingBehaviour DraggingBehaviour { get { return draggingBehaviourCore; } }
		protected virtual void PopulateBehaviours() {
			if(owner.AllowDragDrop) {
				draggingBehaviourCore = new DraggingBehaviour(this);
				AddBehaviour(draggingBehaviourCore);
			}
			ResizeBehaviour rb = new ResizeBehaviour(this);
			if(owner.AllowCrosshair && owner.AllowResize)
				AddBehaviour(new CrosshairBehaviour(this, rb));
			if(owner.AllowResize)
				AddBehaviour(rb);
			if(owner.AllowSelection) {
				AddBehaviour(new SelectionBehaviour(this));
				if(owner.OptionsView.AllowHotTrack)
					AddBehaviour(new HotTrackBehaviour(this));
			}
			AddBehaviour(new ContextMenuBehaviour(this));
			AddBehaviour(new RemoveItemBehaviour(this));
		}
		protected void AddBehaviour(BaseBehaviour b) {
			behaviours.Add(b);
		}
		public void Invalidate() {
			if(owner == null || owner.IsDisposed)
				return;
			if(owner.adorner == null)
				return;
			if(State == AdornerWindowHandlerStates.Sizing) {
				owner.adorner.Hide();																															   
				return;
			}
			bool needHideShow = owner.adorner.NeedHideShowAdorner() && owner.DashboardAdornerWindowVisible;
			if(owner.adorner.adornerWindow.IsVisible && !owner.DashboardAdornerWindowVisible) owner.adorner.Hide();
			if(needHideShow)
				owner.adorner.Hide();
			foreach(BaseBehaviour b in behaviours) {
				b.Invalidate();
			}
			Point cursorPosition = Cursor.Position;
			cursorPosition = owner.PointToClient(cursorPosition);
			Rectangle rect = new Rectangle(Point.Empty, owner.Size);
			if(rect.Contains(cursorPosition) && owner.ApplicationActive && CheckCursorOnLayout()) {
				if(needHideShow) owner.adorner.Show();
			} else {
				owner.handler.HotTrackedItem = null;
				owner.adorner.Hide();
			}
		   owner.adorner.Invalidate();
		}
		internal bool CheckCursorOnLayout() {
			IntPtr hWndHit = DragDropDispatcher.SafeWindowFromPoint(Cursor.Position);
			Control controlHit = Control.FromHandle(hWndHit);
			if(controlHit == null) return false;
			if(controlHit == Owner) return true;
			int watchDog = 0;
			while(controlHit.Parent != null && watchDog++ < 20) {
				if(controlHit.Parent == Owner) return true;
				controlHit = controlHit.Parent;
			}
			return false;
		}
		public void Paint(Graphics g) {
			List<BaseBehaviour> temp = new List<BaseBehaviour>(behaviours);
			temp.Reverse();
			foreach(BaseBehaviour b in temp) {
				b.Paint(g);
			}
		}
		public bool ProcessEvent(EventType eventType, object sender, MouseEventArgs e, KeyEventArgs key) {
			foreach(BaseBehaviour b in behaviours) {
				if(b.ProcessEvent(eventType, sender, e, key)) {
					Invalidate();
					return true;
				}
			}
			if(eventType != EventType.MouseMove) Invalidate();
			return false;
		}
		public void RefreshBehaviours() {
			behaviours.Clear();
			PopulateBehaviours();
		}
	}
}
