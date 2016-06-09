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
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.XtraGrid.Views.Base.ViewInfo {
	public abstract class BaseSelectionInfo { 
		int lockClear = 0;
		protected int[] fValidHotTracks, fValidPressedHitTests, fViewPressedStates;
		protected int fFocusedRowHandle, fHotRowHandle;
		protected BaseHitInfo fPressedInfo, fHotTrackedInfo;
		BaseView view;
		public BaseSelectionInfo(BaseView view) {
			CreateStates();
			this.view = view;
			this.fPressedInfo = null;
			this.fHotTrackedInfo = null;
			this.fHotRowHandle = this.fFocusedRowHandle = GridControl.InvalidRowHandle;
		}
		protected bool IsClearLocked { get { return this.lockClear != 0; } }
		public void LockClear() { 
			this.lockClear ++;
		}
		public void UnLockClear() {
			this.lockClear --;
		}
		public int FocusedRowHandle { get { return fFocusedRowHandle; } set { fFocusedRowHandle = value; } }
		public int HotRowHandle { get { return fHotRowHandle; } set { fHotRowHandle = value; } }
		protected virtual int NoneHitTest { get { return 0; } }
		protected virtual int DefaultState { get { return -1; } }
		public BaseView View { get { return view; } }
		public void ClearPressedInfo() {
			BaseHitInfo prevPressedInfo = this.fPressedInfo;
			this.fPressedInfo = null;
			DoSetPressedState(NoneHitTest);
			if(prevPressedInfo != null)
				View.InvalidateHitObject(prevPressedInfo);
		}
		public bool IsPressed() { return PressedInfo != null; } 
		public BaseHitInfo PressedInfo { get { return fPressedInfo; } }
		public BaseHitInfo HotTrackedInfo { get { return fHotTrackedInfo; } }
		public virtual bool IsPressed(int hitTest) {
			return IsPressed() && PressedInfo.HitTestInt == hitTest;
		}
		protected virtual void CreateStates() {
			fValidHotTracks = new int[0];
			fValidPressedHitTests = new int[0];
			fViewPressedStates = new int[0];
		}
		protected virtual int GetRowHandle(BaseHitInfo hitInfo) {
			return GridControl.InvalidRowHandle;
		}
		protected int[] ValidHotTracks { get { return fValidHotTracks; } }
		protected int[] ValidPressedHitTests { get { return fValidPressedHitTests; } }
		protected int[] ViewPressedStates { get { return fViewPressedStates; } }
		public virtual bool DoSetPressedState(int hitTest) {
			int n = Array.IndexOf(ValidPressedHitTests, hitTest);
			int newState = n == -1 ? DefaultState : ViewPressedStates[n];
			DoSetState(newState);
			return newState == GetState();
		}
		protected virtual void DoSetState(int newState) {
		}
		protected abstract int GetState();
		protected bool IsValidHotTrackInfo(BaseHitInfo hitInfo) {
			return Array.IndexOf(ValidHotTracks, hitInfo.HitTestInt) != -1;
		}
		public bool IsHotTrack(int hitTest) {
			if(HotTrackedInfo == null) 
				return false;
			return (hitTest == HotTrackedInfo.HitTestInt);
		}
		public void OnHotTrackChanged(BaseHitInfo newInfo) {
			BaseHitInfo hOld = HotTrackedInfo;
			if(hOld == newInfo) return;
			fHotTrackedInfo = newInfo;
			int newRowHandle = GetRowHandle(newInfo);
			if(newRowHandle != HotRowHandle) {
				int oldRow = HotRowHandle;
				HotRowHandle = newRowHandle;
				View.OnRowHotTrackChanged(oldRow, HotRowHandle);
			}
			View.OnHotTrackChanged(hOld, HotTrackedInfo);
		}
		public virtual void OnHotTrackEnter(BaseHitInfo hitInfo) {
			bool valid = IsValidHotTrackInfo(hitInfo);
			if(!valid) hitInfo = null;
			OnHotTrackChanged(hitInfo);
		}
		public virtual void OnHotTrackLeave() {
			OnHotTrackChanged(null);
		}
		protected virtual bool IsHotEquals(BaseHitInfo h1, BaseHitInfo h2) {
			return true;
		}
		public virtual bool CheckHotTrackMouseMove(BaseHitInfo hitInfo) {
			bool valid = IsValidHotTrackInfo(hitInfo);
			if(HotTrackedInfo != null) {
				if(IsHotEquals(hitInfo, HotTrackedInfo)) {
					this.fHotTrackedInfo = hitInfo;
					return View.OnCheckHotTrackMouseMove(HotTrackedInfo);
				}
			}
			if(!valid) hitInfo = null;
			OnHotTrackChanged(hitInfo);
			return true;
		}
		protected virtual bool CanMousePressObject(BaseHitInfo hitInfo) { return true; }
		protected virtual bool AllowActualCheckMouseDown { get { return View != null && !View.IsDesignMode; } }
		public virtual bool CheckMouseDown(BaseHitInfo hitInfo) {
			ClearPressedInfo();
			if(Array.IndexOf(ValidPressedHitTests, hitInfo.HitTestInt) == -1) 
				return false;
			if(!CanMousePressObject(hitInfo)) return false;
			this.fPressedInfo = hitInfo;
			bool res = DoSetPressedState(hitInfo.HitTestInt);
			if(!res) ClearPressedInfo();
			View.InvalidateHitObject(hitInfo);
			if(AllowActualCheckMouseDown && res && Control.MouseButtons == MouseButtons.None) ClearPressedInfo();
			return res;
		}
		protected virtual bool IsPressedEquals(BaseHitInfo h1, BaseHitInfo h2) {
			return true;
		}
		public virtual void CheckMouseUp(BaseHitInfo hitInfo) {
			if(!IsPressed() || Array.IndexOf(ValidPressedHitTests, PressedInfo.HitTestInt) == -1) 
				return;
			if(IsPressedEquals(hitInfo, PressedInfo)) {
				ClearPressedInfo();
				View.Handler.DoClickAction(hitInfo);
				return;
			}
			ClearPressedInfo();
		}
		public void Clear() {
			if(IsClearLocked) return;
			OnClearCore();
		}
		protected virtual void OnClearCore() {
			OnHotTrackChanged(null);
			ClearPressedInfo();
		}
	}
}
