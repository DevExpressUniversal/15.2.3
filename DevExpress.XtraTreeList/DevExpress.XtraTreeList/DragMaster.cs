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

namespace DevExpress.XtraTreeList.Dragging {
	using System;
	using System.Drawing;
	using System.Windows.Forms;
	using DevExpress.Utils;
	public class DragMaster {
		[ThreadStatic]
		static DragWindow dragWindow;
		bool dragInProgress;
		DragDropEffects effects;
		DragDropEffects lastEffect;
		static Cursor customizationCursor = null;
		public DragMaster() {
			dragInProgress = false;
			lastEffect = effects = DragDropEffects.None;
		}
		static DragWindow DragWindow {
			get {
				if(dragWindow == null) dragWindow = new DragWindow();
				return dragWindow;
			}
		}
		public DragDropEffects LastEffect {
			get { return lastEffect; }
		}
		public bool DragInProgress {
			get { return dragInProgress; }
		}
		public Size DragSize {
			get {
				if(DragWindow == null) return Size.Empty;
				return DragWindow.Size;
			}
		}
		public byte Alpha {
			get { return DragWindow.Alpha; }
			set { DragWindow.Alpha = value; }
		}
		public void StartDrag(TreeList owner, Bitmap bmp, Point startPoint, DragDropEffects effects) {
			StopDrag();
			dragInProgress = true;
			this.effects = effects;
			lastEffect = effects;
			DragWindow.Create(owner.Handle);
			DragWindow.Show(bmp, startPoint);
			SetDragCursor(effects);
		}
		public void SetDragCursor(DragDropEffects e) {
			if(e == DragDropEffects.None)
				Cursor.Current = CustomizationCursor;
			else
				Cursor.Current = Cursors.Default;
		}
		protected void StopDrag() {
			dragInProgress = false;
			lastEffect = effects = DragDropEffects.None;
			DragWindow.Hide();
		}
		public void DoDrag(Point p, DragDropEffects e, bool setCursor) {
			if(!dragInProgress) return;
			lastEffect = e;
			if(setCursor) SetDragCursor(e);
			DragWindow.Show(p);
		}
		public void CancelDrag() {
			if(!dragInProgress) return;
			StopDrag();
		}
		public void EndDrag() {
			if(!dragInProgress) return;
			StopDrag();
		}
		static Cursor CustomizationCursor {
			get {
				if(customizationCursor == null) customizationCursor = ResourceImageHelper.CreateCursorFromResources("DevExpress.XtraTreeList.Images.customization.cur", typeof(DragMaster).Assembly);
				return customizationCursor;
			}
		}
	}
	public class DragWindow : DevExpress.Utils.Internal.DXLayeredWindowEx {
		private Bitmap dragBitmapCore;
		public DragWindow() { }
		public void Show(Bitmap dragImage, Point location) {
			if(IsVisible || dragImage == null) return;
			this.dragBitmapCore = dragImage;
			this.Size = dragImage.Size;
			Show(location);
			Update();
		}
		protected override bool AllowSafeSize { get { return true; } }
		public new void Hide() {
			if(!IsCreated || !IsVisible) return;
			base.Hide();
			DestroyImage();
			DestroyHandle();
			Alpha = (byte)255;
		}
		protected void DestroyImage() {
			if(dragBitmapCore == null) return;
			dragBitmapCore.Dispose();
			dragBitmapCore = null;
		}
		protected override void DrawCore(Utils.Drawing.GraphicsCache cache) {
			cache.Graphics.DrawImageUnscaled(dragBitmapCore, Point.Empty);
		}
		protected override void OnDisposing() {
			base.OnDisposing();
			DestroyImage();
		}
		public static readonly Point InvisiblePoint = new Point(-100000, -100000);
	}
}
