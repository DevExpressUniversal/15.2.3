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

using DevExpress.Utils.Controls;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout.Handlers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraLayout.Customization.Behaviours {
	public class AdornerWindowHandlerBase {
		private List<LayoutBaseBehaviour> behaviours;
		private AdornerWindowHandlerStates currentState = AdornerWindowHandlerStates.Normal;
		private BaseLayoutItem hotTrackedItem = null;
		private LayoutControl owner;
		private BaseLayoutItem selectedItem = null;
		public AdornerWindowHandlerBase(LayoutControl lc) {
			owner = lc;
			behaviours = new List<LayoutBaseBehaviour>();
			PopulateBehaviours();
		}
		protected internal virtual List<LayoutBaseBehaviour> Behaviours {
			get {
				return behaviours;
			}
		}
		protected virtual LayoutControl Owner {
			get {
				return owner;
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
					Owner.Invalidate();
				}
			}
		}
		public BaseLayoutItem SelectedItem {
			get {
				return selectedItem;
			}
			set {
				if(selectedItem == value) {
					return;
				}
				selectedItem = value;
				if(value != null) {
					((ILayoutControl)Owner).FakeFocusContainer.Focus();
				}
				try {
					Owner.Root.StartChangeSelection();
					Owner.Root.ClearSelection();
					if(selectedItem != null) {
						selectedItem.Selected = true;
					}
					Invalidate();
					Owner.Invalidate();
				} finally {
					Owner.Root.EndChangeSelection();
				}
			}
		}
		public AdornerWindowHandlerStates State {
			get {
				return currentState;
			}
			set {
				if(currentState != value) {
					var previousState = currentState;
					currentState = value;
					Invalidate();
					OnStateChanged(previousState);
				}
			}
		}
		protected virtual void AddBehaviour(LayoutBaseBehaviour b) {
			Behaviours.Add(b);
		}
		protected virtual bool CheckCursorOnLayout() {
			var hWndHit = DragDropDispatcher.SafeWindowFromPoint(Cursor.Position);
			var controlHit = Control.FromHandle(hWndHit);
			if(controlHit == null) {
				if(hWndHit == owner.layoutAdorner.adornerWindow.Handle) return true;
				return false;
			}
			if(controlHit == Owner) {
				return true;
			}
			var watchDog = 0;
			while(controlHit.Parent != null && watchDog++ < 20) {
				if(controlHit.Parent == Owner) {
					return true;
				}
				controlHit = controlHit.Parent;
			}
			return false;
		}
		protected virtual void OnStateChanged(AdornerWindowHandlerStates previousState) {
		}
		protected virtual void PopulateBehaviours() {
		}
		internal void SetCurrentState(AdornerWindowHandlerStates currentState) {
			this.currentState = currentState;
		}
		public virtual void Invalidate() {
		}
		public static bool IsLayoutChangingState(AdornerWindowHandlerStates state) {
			return state == AdornerWindowHandlerStates.Sizing || state == AdornerWindowHandlerStates.Dragging;
		}
		public virtual void Paint(GraphicsCache cache) {
			var temp = new List<LayoutBaseBehaviour>(Behaviours);
			temp.Reverse();
			foreach(LayoutBaseBehaviour b in temp) {
				b.Paint(cache);
			}
		}
		public virtual bool ProcessEvent(EventType eventType, object sender, MouseEventArgs e, KeyEventArgs key) {
			foreach(LayoutBaseBehaviour b in Behaviours) {
				if(b.ProcessEvent(eventType, sender, e, key)) {
					return true;
				}
			}
			return false;
		}
		public virtual void RefreshBehaviours() {
			Behaviours.Clear();
			PopulateBehaviours();
		}
		public void SetCursor(Cursor cursor) {
			Owner.SetCursor(cursor);
		}
	}
	public class LayoutAdornerWindowHandler :AdornerWindowHandlerBase, IDisposable {
		public LayoutAdornerWindowHandler(LayoutControl lc) : base(lc) { }
		public LayoutControl CastedOwner { get { return base.Owner; } }
		protected override void PopulateBehaviours() {
			AddBehaviour(new FlowLayoutBehaviour(this));
			AddBehaviour(new ResizingLayoutBehaviour(this));
			if(!(CastedOwner as ILayoutControl).DesignMode) AddBehaviour(new DragDropLayoutBehaviour(this));
		}
		public override void Invalidate() {
			if(CastedOwner == null)
				return;
			if(CastedOwner.layoutAdorner == null)
				return;
			bool needHideShow = CastedOwner.layoutAdorner.NeedHideShowAdorner();
			if(needHideShow)
				CastedOwner.layoutAdorner.Hide();
			foreach(LayoutBaseBehaviour b in Behaviours) {
				b.Invalidate();
			}
			Point cursorPosition = Cursor.Position;
			cursorPosition = CastedOwner.PointToClient(cursorPosition);
			Rectangle rect = new Rectangle(Point.Empty, CastedOwner.Size);
			rect.Inflate(-2,-2);
			if(rect.Contains(cursorPosition) && CheckCursorOnLayout()) {
				if(needHideShow) CastedOwner.layoutAdorner.Show();
			} else {
				CastedOwner.layoutAdornerWindowHandler.HotTrackedItem = null;
				CastedOwner.layoutAdorner.Hide();
			}
			CastedOwner.layoutAdorner.Invalidate();
		}
		public void Dispose() {
			if(Behaviours != null) Behaviours.Clear();
		}
		internal LayoutItemDragController dragController;
	}
}
