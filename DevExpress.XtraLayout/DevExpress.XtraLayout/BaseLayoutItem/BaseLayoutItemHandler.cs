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
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout.Handlers {
	public enum LayoutHandlerState {Normal, Sizing };
	public class BaseLayoutItemHandler {
		BaseLayoutItem ownerItem;
		LayoutHandlerState state;
		bool limitedFunctionalityMode = true; 
		protected BaseLayoutItemHitInfo mouseHitTestInternal;
		public BaseLayoutItemHitInfo MouseHitTest {
			get {return mouseHitTestInternal;}
			set { mouseHitTestInternal = value; } 
		}
		public bool LimitedFunctionality{
			get { return limitedFunctionalityMode;}
			set {limitedFunctionalityMode = value;}
		}
		private bool allowChangeSelectionCore = true;
		public bool AllowChangeSelection {
			get { return allowChangeSelectionCore; }
			set { allowChangeSelectionCore = value; }
		}
		protected internal void CalculateMouseHitTest(Point point) {
			mouseHitTestInternal = CreateHitTest(point.X, point.Y);
			if(mouseHitTestInternal.HitType == LayoutItemHitTest.None) 
				mouseHitTestInternal = CreateHitTest(point.X, point.Y);
		}
		public BaseLayoutItemHandler(BaseLayoutItem item) {
			ownerItem = item;	 
		}
		public BaseLayoutItem Owner{
			get { return ownerItem;}
			set { ownerItem = value;} 
		}
		protected bool AllowChangeState(LayoutHandlerState newState) {
			if(!LimitedFunctionality) return true;
			if(MouseHitTest == null) return false;
			if(State == LayoutHandlerState.Normal && newState == LayoutHandlerState.Sizing ||
			   State == LayoutHandlerState.Sizing && newState == LayoutHandlerState.Normal) return true;
			return false;
		}
		public virtual LayoutHandlerState State {
			get { return state; }
			set {
				if(AllowChangeState(value)) {
					if(value == LayoutHandlerState.Normal)
						mouseHitTestInternal = null;
					LayoutHandlerState oldState = state;
					state = value;
					if(oldState == LayoutHandlerState.Normal && state != oldState) {
						if(!Owner.StartChange()) {
							state = oldState;
							return;
						}
					}
					if(oldState != state && state == LayoutHandlerState.Normal)
						Owner.EndChange();
				} else
					state = LayoutHandlerState.Normal;
			}
		}
		public virtual void ResetHandler() {
			State = LayoutHandlerState.Normal;
			MouseHitTest = null;
		}
		protected virtual bool IsControlPressed {	get { return (Control.ModifierKeys & Keys.Control) != 0; } }
		protected virtual bool IsShiftPressed {	get { return (Control.ModifierKeys & Keys.Shift) != 0; } }
		protected internal virtual bool PerformControlActions(EventType  eventType, MouseEventArgs e) {
			mouseHitTestInternal = CreateHitTest(e.X, e.Y);
			if(!PerformOwnersActions(eventType, e))
				return PerformChildHandlerActions(eventType, e);
			return false;
		}
		protected internal virtual bool PerformOwnersActions(EventType  eventType, MouseEventArgs e) {
			if(mouseHitTestInternal == null) return false;
			if(e.Button == MouseButtons.Left && eventType == EventType.MouseDown && mouseHitTestInternal.HitType == LayoutItemHitTest.Item && MouseHitTest.Item != null) { 
				return false;
			}
			return false;
		}
		protected virtual bool PerformSelection() {
			return false;
		}
		protected internal virtual bool PerformChildHandlerActions(EventType  eventType, MouseEventArgs e) {
			return false;	 
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			if(!PerformControlActions(EventType.MouseMove, e)) {
			}
		}
		public virtual void OnMouseMove(object sender, MouseEventArgs e) {
			OnMouseMove(e);
		}
		public virtual bool OnMouseDown(object sender, MouseEventArgs e)
		{
			bool result = OnMouseDown(e);
			return result;
		}
		public virtual bool OnMouseDown(MouseEventArgs e) {
			if(!PerformControlActions(EventType.MouseDown, e)) {
			}
			return true;
		}
		public virtual bool OnMouseUp(object sender, MouseEventArgs e)
		{
			bool result = OnMouseUp(e);
			return result;
		}
		public virtual bool OnMouseUp(MouseEventArgs e) {
			if(!PerformControlActions(EventType.MouseUp, e)) {
			}
			return true;
		}
		protected virtual BaseLayoutItemHitInfo CreateHitTest(int X, int Y) {
			return Owner.CalcHitInfo(new Point(X, Y), true);
		}
	}
}
