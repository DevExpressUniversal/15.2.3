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
using DevExpress.Utils.Mdi;
using DevExpress.XtraBars.Docking.Helpers;
namespace DevExpress.XtraBars.Navigation {
	public interface INavigationPaneResizeListener : IDisposable {
		bool SizeMode { get; set; }
		void ProcessEnterSizeMove();
		void ProcessWindowsPosChanging(IntPtr intPtr);
		void ProcessExitSizeMove();
	}
	class BaseNavigationPaneResizeListener : INavigationPaneResizeListener {
		NavigationPane ownerCore;
		public BaseNavigationPaneResizeListener(NavigationPane owner) {
			ownerCore = owner;
		}
		public bool SizeMode { get; set; }
		public NavigationPane Owner {
			get { return ownerCore; }
		}
		public virtual void ProcessEnterSizeMove() { }
		public void ProcessWindowsPosChanging(IntPtr intPtr) {
			if(!SizeMode) return;
			ProcessWindowsPosChangingCore(intPtr);
		}
		public void ProcessExitSizeMove() {
			if(SizeMode) {
				SizeMode = false;
			}
			ProcessExitSizeMoveCore();
		}
		protected virtual void ProcessExitSizeMoveCore() { }
		protected virtual void ProcessWindowsPosChangingCore(IntPtr intPtr) { }
		public void Dispose() {
			ownerCore = null;
			DisposeCore();
		}
		protected virtual void DisposeCore() { }
	}
	class LiveResizeListener : BaseNavigationPaneResizeListener {
		public LiveResizeListener(NavigationPane owner)
			: base(owner) {
		}
		Size SizeBeforEnterSizing;
		public override void ProcessEnterSizeMove() {
			SizeBeforEnterSizing = Owner.Size;
		}
		protected override void ProcessWindowsPosChangingCore(IntPtr intPtr) {
			WinAPI.WINDOWPOS pos = (WinAPI.WINDOWPOS)BarNativeMethods.PtrToStructure(intPtr, typeof(WinAPI.WINDOWPOS));
			Rectangle position = pos.ToRectangle();
			if(position.Width - NavigationPane.StickyWidth <= Owner.ViewInfo.ButtonsBounds.Width) {
				pos.cx = Owner.ViewInfo.ButtonsBounds.Width;
				if(Owner.IsRightToLeftLayout())
					pos.x = Owner.Parent.ClientRectangle.Width - Owner.ViewInfo.ButtonsBounds.Width;
				Owner.State = NavigationPaneState.Collapsed;
			}
			else if(position.Width >= Owner.Parent.ClientRectangle.Width - NavigationPane.StickyWidth) {
				pos.cx = Owner.Parent.ClientRectangle.Width;
				if(Owner.IsRightToLeftLayout())
					pos.x = Owner.Parent.ClientRectangle.X;
				Owner.State = NavigationPaneState.Expanded;
			}
			else
				Owner.State = NavigationPaneState.Default;
			BarNativeMethods.StructureToPtr(pos, intPtr, false);
		}
		protected override void ProcessExitSizeMoveCore() {
			if(Owner.State != NavigationPaneState.Default && Owner.State != NavigationPaneState.Regular)
				Owner.RegularSize = SizeBeforEnterSizing;
			SizeBeforEnterSizing = Size.Empty;
		}
	}
	class AdornerResizeListener : BaseNavigationPaneResizeListener {
		public AdornerResizeListener(NavigationPane owner)
			: base(owner) {
		}
		protected override void DisposeCore() {
			base.DisposeCore();
			Docking2010.Ref.Dispose(ref sizingAdornerCore);
		}
		protected internal virtual Rectangle GetSizerBounds(bool checkDPISettings) {
			Rectangle frameBounds = new Rectangle(Owner.Bounds.Right, Owner.Bounds.Top, DockConsts.ResizeSelectionFrameWidth, Owner.Bounds.Height);
			if(Owner.IsRightToLeftLayout())
				frameBounds.X = Owner.Bounds.Left;
			Rectangle screenRect = Owner.Parent.RectangleToScreen(frameBounds);
			if(Owner.State == NavigationPaneState.Expanded && Owner.IsRightToLeftLayout())
				screenRect.X += screenRect.Width;
			else if(Owner.State == NavigationPaneState.Expanded)
				screenRect.X -= screenRect.Width;
			return screenRect;
		}
		void ResetSplit(SizingAdorer adorner) {
			adorner.EndSizing();
		}
		public override void ProcessEnterSizeMove() {
			SizingAdorner.StartSizing(GetSizerBounds(false));
		}
		SizingAdorer sizingAdornerCore;
		protected internal SizingAdorer SizingAdorner {
			get {
				if(!Owner.IsHandleCreated) return null;
				if(sizingAdornerCore == null)
					sizingAdornerCore = new SizingAdorer(Owner.Parent);
				return sizingAdornerCore;
			}
		}
		int prevX = 0;
		int prevX1 = 0;
		protected override void ProcessWindowsPosChangingCore(IntPtr intPtr) {
			WinAPI.WINDOWPOS pos = (WinAPI.WINDOWPOS)BarNativeMethods.PtrToStructure(intPtr, typeof(WinAPI.WINDOWPOS));
			Rectangle position = pos.ToRectangle();
			if(prevX == 0)
				prevX = pos.cx;
			if(position.Width - NavigationPane.StickyWidth <= Owner.ViewInfo.ButtonsBounds.Width) {
				pos.cx = Owner.ViewInfo.ButtonsBounds.Width;
			}
			else if(position.Width >= Owner.Parent.ClientRectangle.Width - NavigationPane.StickyWidth) {
				pos.cx = Owner.Parent.ClientRectangle.Width;
			}
			int positionX = pos.cx - prevX;
			if(Owner.IsRightToLeftLayout())
				positionX *= -1;
			SizingAdorner.Sizing(true, positionX);
			prevX = pos.cx;
			prevX1 = pos.x;
			pos.cx = Owner.Bounds.Width;
			pos.x = Owner.Bounds.X;
			BarNativeMethods.StructureToPtr(pos, intPtr, false);
			return;
		}
		protected override void ProcessExitSizeMoveCore() {
			UpdateState();
			prevX = 0;
			SizingAdorner.EndSizing();
		}
		protected void UpdateState() {
			if(prevX - NavigationPane.StickyWidth <= Owner.ViewInfo.ButtonsBounds.Width) {
				Owner.State = NavigationPaneState.Collapsed;
			}
			else if(prevX >= Owner.Parent.ClientRectangle.Width - NavigationPane.StickyWidth) {
				Owner.State = NavigationPaneState.Expanded;
			}
			else {
				Owner.State = NavigationPaneState.Default;
				Owner.LockPainting();
				Owner.Width = prevX;
				Owner.UnlockPainting();
				Owner.LayoutChanged();
			}
		}
	}
}
