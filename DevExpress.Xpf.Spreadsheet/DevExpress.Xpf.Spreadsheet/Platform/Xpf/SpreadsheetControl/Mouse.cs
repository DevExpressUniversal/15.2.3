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
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformIndependentControl = System.Windows.Forms.Control;
using PlatformIndependentCursor = System.Windows.Forms.Cursor;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;
using System.Windows.Media.Imaging;
using System.IO;
using DevExpress.Xpf.Spreadsheet.Internal;
using System.Windows.Media;
using DevExpress.Mvvm.UI.Interactivity;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using System.Linq;
using DevExpress.Xpf.Office.Internal;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.Xpf.Spreadsheet {
	public partial class SpreadsheetControl : IMouseWheelScrollClient {
		bool isCapture = false;
		bool isLeftBtn;
		void InitializeMouse() {
			BehaviorCollection behaviors = Interaction.GetBehaviors(this);
			behaviors.Add(new OfficeMouseWheelScrollBehavior<SpreadsheetControl>(false));
		}
		bool ISpreadsheetControl.CaptureMouse() {
			this.CaptureMouse();
			isCapture = true;
			return isCapture;
		}
		bool ISpreadsheetControl.ReleaseMouse() {
			this.ReleaseMouseCapture();
			this.isCapture = false;
			return !isCapture;
		}
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if (!InnerControl.IsAnyInplaceEditorActive && !InnerControl.IsDataValidationInplaceEditorActive) {
				this.Focus();
				e.Handled = true;
			}
			if (GetHitTest(GetPointRelativelySpreadsheetControl(e)) == SpreadsheetHitTestType.Worksheet) {
				isLeftBtn = true;
			}
		}
		protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			isLeftBtn = false;
		}
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseMove(e);
			System.Windows.Point point = GetPointWithoutScale(e.GetPosition(ViewControl.WorksheetControl));
			InnerControl.ActiveView.AllowCalculateHeaderHitTest = GetHitTest(point) == SpreadsheetHitTestType.Worksheet ? true : false;
			PlatformIndependentMouseEventArgs args =
				new PlatformIndependentMouseEventArgs(isLeftBtn ? MouseButtons.Left : MouseButtons.None, 0, (int)point.X, (int)point.Y, 0);
			if (InnerControl != null) {
				InnerControl.OnMouseMove(args);
			}
		}
		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(e);
			BeginUpdate();
			try {
				InnerControl.ResetAndRedrawHeader();
				InnerControl.HideHoveredComment();
			}
			finally {
				EndUpdate();
			}
		}
		protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			System.Windows.Point controlPoint = GetPointRelativelySpreadsheetControl(e);
			if (GetHitTest(controlPoint) == SpreadsheetHitTestType.Worksheet) {
				this.CaptureMouse();
				System.Windows.Point worksheetPoint = GetPointWithoutScale(e.GetPosition(ViewControl.WorksheetControl));
				PlatformIndependentMouseEventArgs args =
					new PlatformIndependentMouseEventArgs(GetMouseButton(e.ChangedButton), e.ClickCount, (int)worksheetPoint.X, (int)worksheetPoint.Y, 0);
				if (InnerControl != null)
					InnerControl.OnMouseDown(args);
			}
		}
		protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseUp(e);
			System.Windows.Point point = GetPointRelativelySpreadsheetControl(e);
			PlatformIndependentMouseEventArgs args;
			if (GetHitTest(point) == SpreadsheetHitTestType.Worksheet) {
				this.CaptureMouse();
				System.Windows.Point worksheetPoint = GetPointWithoutScale(e.GetPosition(ViewControl.WorksheetControl));
				args = new PlatformIndependentMouseEventArgs(GetMouseButton(e.ChangedButton), e.ClickCount, (int)worksheetPoint.X, (int)worksheetPoint.Y, 0);
			}
			else
				args = new PlatformIndependentMouseEventArgs(GetMouseButton(e.ChangedButton), e.ClickCount, (int)point.X, (int)point.Y, 0);
			if (InnerControl != null)
				InnerControl.OnMouseUp(args);
			this.ReleaseMouseCapture();
		}
		System.Windows.Point GetPointRelativelySpreadsheetControl(System.Windows.Input.MouseEventArgs e) {
			return e.GetPosition(this);
		}
		System.Windows.Point GetPointWithoutScale(System.Windows.Point p) {
			return ViewControl.WorksheetControl.GetPointСonsideringScale(p);
		}
		private MouseButtons GetMouseButton(System.Windows.Input.MouseButton mouseButton) {
			switch (mouseButton) {
				case System.Windows.Input.MouseButton.Left: return MouseButtons.Left;
				case System.Windows.Input.MouseButton.Right: return MouseButtons.Right;
				case System.Windows.Input.MouseButton.Middle: return MouseButtons.Middle;
				case System.Windows.Input.MouseButton.XButton1: return MouseButtons.XButton1;
				case System.Windows.Input.MouseButton.XButton2: return MouseButtons.XButton2;
				default: return MouseButtons.None;
			}
		}
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(System.Windows.Input.MouseWheelEventArgs e) {
			System.Windows.Point point = GetPointRelativelySpreadsheetControl(e);
			MouseWheelEventArgsEx mweax = e as MouseWheelEventArgsEx;
			int delta = (mweax == null) ? e.Delta : mweax.DeltaX;
			OfficeMouseWheelEventArgs ea = new OfficeMouseWheelEventArgs(MouseButtons.None, 0, (int)point.X, (int)point.Y, delta / SystemInformation.MouseWheelScrollDelta);
			ea.IsHorizontal = mweax != null;
			if (InnerControl != null)
				InnerControl.OnMouseWheel(ea);
			base.OnMouseWheel(e);
		}
	}
	public class XpfOfficeScroller : IOfficeScroller {
		readonly ISpreadsheetControl control;
		public XpfOfficeScroller(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public ISpreadsheetControl Control { get { return control; } }
		#region IOfficeScroller Members
		public void Start(PlatformIndependentControl control, Point screenPoint) {
		}
		public void Start(PlatformIndependentControl control) {
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
	}
	#region XpfSpreadsheetMouseHandlerStrategyFactory
	public class XpfSpreadsheetMouseHandlerStrategyFactory : SpreadsheetMouseHandlerStrategyFactory {
		public override DragFloatingObjectManuallyMouseHandlerStateStrategy CreateDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state) {
			return new XpfDragFloatingObjectManuallyMouseHandlerStateStrategy(state);
		}
		public override DragRangeManuallyMouseHandlerStateStrategy CreateDragRangeManuallyMouseHandlerStateStrategy(DragRangeManuallyMouseHandlerStateBase state) {
			return new XpfDragRangeManuallyMouseHandlerStateStrategy(state);
		}
		public override SpreadsheetMouseHandlerStrategy CreateMouseHandlerStrategy(SpreadsheetMouseHandler mouseHandler) {
			return new XpfSpreadsheetMouseHandlerStrategy(mouseHandler);
		}
		public override ResizeColumnMouseHandlerStateStrategy CreateResizeColumnMouseHandlerStateStrategy(ContinueResizeColumnsMouseHandlerState state) {
			return new XpfResizeColumnMouseHandlerStateStrategy(state);
		}
		public override ResizeRowMouseHandlerStateStrategy CreateResizeRowMouseHandlerStateStrategy(ContinueResizeRowsMouseHandlerState state) {
			return new XpfResizeRowMouseHandlerStateStrategy(state);
		}
		public override SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy CreateSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy(SpreadsheetRectangularObjectResizeMouseHandlerState state) {
			return new XpfSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy(state);
		}
		public override CommentMouseHandlerStateStrategy CreateCommentMouseHandlerStateStrategy(CommentMouseHandlerStateBase state) {
			return new XpfCommentMouseHandlerStateStrategy(state);
		}
	}
	#endregion
	#region XpfSpreadsheetMouseHandlerStrategy
	public class XpfSpreadsheetMouseHandlerStrategy : SpreadsheetMouseHandlerStrategy {
		public XpfSpreadsheetMouseHandlerStrategy(SpreadsheetMouseHandler state)
			: base(state) {
		}
		public SpreadsheetControl OwnerControl { get { return (SpreadsheetControl)Control; } }
		protected internal override void AutoScrollerOnDragOver(System.Drawing.Point pt) {
		}
		protected internal override PlatformIndependentDragEventArgs ConvertDragEventArgs(System.Windows.Forms.DragEventArgs screenDragEventArgs) {
			throw new NotImplementedException();
		}
		protected internal override PlatformIndependentMouseEventArgs ConvertMouseEventArgs(System.Windows.Forms.MouseEventArgs screenMouseEventArgs) {
			return screenMouseEventArgs;
		}
		protected internal override PlatformIndependentMouseEventArgs CreateFakeMouseMoveEventArgs() {
			System.Drawing.Point screenMousePos = PlatformIndependentCursor.Position;
			System.Windows.Point clientMousePoint = OwnerControl.PointFromScreen(new System.Windows.Point(screenMousePos.X, screenMousePos.Y));
			PlatformIndependentMouseEventArgs args = new PlatformIndependentMouseEventArgs(System.Windows.Forms.MouseButtons.None, 0, (int)clientMousePoint.X, (int)clientMousePoint.Y, 0);
			return ConvertMouseEventArgs(args);
		}
		protected internal override DevExpress.Utils.IOfficeScroller CreateOfficeScroller() {
			return new XpfOfficeScroller(this.Control);
		}
		protected internal override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
		}
		protected internal override void StartOfficeScroller(System.Drawing.Point clientPoint) {
		}
	}
	#endregion
	#region XpfResizeColumnMouseHandlerStateStrategy
	public class XpfResizeColumnMouseHandlerStateStrategy : ResizeColumnMouseHandlerStateStrategy {
		Locker drawLineLocker = new Locker();
		public XpfResizeColumnMouseHandlerStateStrategy(ContinueResizeColumnsMouseHandlerState state)
			: base(state) {
		}
		SpreadsheetControl XpfControl { get { return (SpreadsheetControl)Control; } }
		protected internal override void BeginVisualFeedback() { }
		protected internal override void DrawReversibleLine(int coordinate) {
			if (drawLineLocker.IsLocked) return;
			XpfControl.ViewControl.ShowResizeFeedback(true);
			XpfControl.ViewControl.DrawReversibleLine(coordinate, true);
		}
		protected internal override void EndVisualFeedback() {
			drawLineLocker.LockOnce();
			XpfControl.ViewControl.ShowResizeFeedback(false);
		}
		protected internal override void HideVisualFeedback() { }
		protected internal override void ShowVisualFeedback() { }
	}
	#endregion
	#region XpfResizeRowMouseHandlerStateStrategy
	public class XpfResizeRowMouseHandlerStateStrategy : ResizeRowMouseHandlerStateStrategy {
		Locker drawLineLocker = new Locker();
		public XpfResizeRowMouseHandlerStateStrategy(ContinueResizeRowsMouseHandlerState state)
			: base(state) {
		}
		SpreadsheetControl XpfControl { get { return (SpreadsheetControl)Control; } }
		protected internal override void BeginVisualFeedback() { }
		protected internal override void DrawReversibleLine(int coordinate) {
			if (drawLineLocker.IsLocked) return;
			XpfControl.ViewControl.ShowResizeFeedback(true);
			XpfControl.ViewControl.DrawReversibleLine(coordinate, false);
		}
		protected internal override void EndVisualFeedback() {
			drawLineLocker.LockOnce();
			XpfControl.ViewControl.ShowResizeFeedback(false);
		}
		protected internal override void HideVisualFeedback() { }
		protected internal override void ShowVisualFeedback() { }
	}
	#endregion
	#region XpfSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy
	public class XpfSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy : SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy {
		public XpfSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy(SpreadsheetRectangularObjectResizeMouseHandlerState state)
			: base(state) {
		}
		SpreadsheetControl XpfControl { get { return (SpreadsheetControl)Control; } }
		protected internal override void BeginVisualFeedback() {
		}
		protected internal override void EndVisualFeedback() {
			XpfControl.ViewControl.ShowDragFloatingObjectVisualFeedback(false, null, new System.Windows.Rect(), 0);
		}
		protected internal override void HideVisualFeedback() {
		}
		protected internal override void ShowVisualFeedback(System.Drawing.Rectangle bounds, XtraSpreadsheet.Layout.Page page, DevExpress.Office.Utils.OfficeImage image) {
			XpfControl.ViewControl.ShowDragFloatingObjectVisualFeedback(true, Box, bounds.ToRect(), State.RotationAngle);
		}
	}
	#endregion
	#region XpfDragFloatingObjectManuallyMouseHandlerStateStrategy
	public class XpfDragFloatingObjectManuallyMouseHandlerStateStrategy : DragFloatingObjectManuallyMouseHandlerStateStrategy {
		public XpfDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state)
			: base(state) {
		}
		SpreadsheetControl XpfControl { get { return (SpreadsheetControl)Control; } }
		protected internal override void BeginVisualFeedback() {
		}
		protected internal override void EndVisualFeedback() {
			XpfControl.ViewControl.ShowDragFloatingObjectVisualFeedback(false, null, new System.Windows.Rect(), 0);
		}
		protected internal override void HideVisualFeedback(System.Drawing.Rectangle bounds, XtraSpreadsheet.Layout.Page page) {
		}
		protected internal override void ShowVisualFeedback(System.Drawing.Rectangle bounds, XtraSpreadsheet.Layout.Page page, DevExpress.Office.Utils.OfficeImage image) {
			XpfControl.ViewControl.ShowDragFloatingObjectVisualFeedback(true, Box, bounds.ToRect(), State.RotationAngle);
		}
	}
	#endregion
	#region XpfDragRangeManuallyMouseHandlerStateStrategy
	public class XpfDragRangeManuallyMouseHandlerStateStrategy : DragRangeManuallyMouseHandlerStateStrategy {
		public XpfDragRangeManuallyMouseHandlerStateStrategy(DragRangeManuallyMouseHandlerStateBase state)
			: base(state) {
		}
		#region Properties
		SpreadsheetControl XpfControl { get { return (SpreadsheetControl)Control; } }
		#endregion
		protected internal override void ShowVisualFeedback(System.Drawing.Rectangle bounds, XtraSpreadsheet.Layout.Page page) {
			XpfControl.ViewControl.ShowDragRangeFeedback(bounds.ToRect());
		}
		protected internal override void HideVisualFeedback() {
			XpfControl.ViewControl.HideVisualFeedback();
		}
	}
	#endregion
	#region XpfCommentMouseHandlerStateStrategy
	public class XpfCommentMouseHandlerStateStrategy : CommentMouseHandlerStateStrategy {
		public XpfCommentMouseHandlerStateStrategy(CommentMouseHandlerStateBase state)
			: base(state) {
		}
		#region Properties
		SpreadsheetControl XpfControl { get { return (SpreadsheetControl)Control; } }
		#endregion
		protected internal override void ShowVisualFeedback(Rectangle bounds, Page page) {
			XpfControl.ViewControl.ShowCommentVisualFeedback(bounds.ToRect());
		}
		protected internal override void HideVisualFeedback() {
			XpfControl.ViewControl.HideVisualFeedback();
		}
	}
	#endregion
}
