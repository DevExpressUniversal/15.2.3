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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.RichEdit.Controls.Internal;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Internal;
#if SL
using PlatformIndependentIDataObject = System.Windows.IDataObject;
using PlatformIndependentDataObject = DevExpress.Utils.DataObject;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentMouseButtons = DevExpress.Data.MouseButtons;
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformImage = System.Windows.Controls.Image;
using TransformMatrix = DevExpress.Xpf.Core.Native.Matrix;
#else
using PlatformIndependentIDataObject = System.Windows.Forms.IDataObject;
using PlatformIndependentDataObject = System.Windows.Forms.DataObject;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentMouseButtons = System.Windows.Forms.MouseButtons;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformImage = System.Windows.Controls.Image;
using TransformMatrix = System.Drawing.Drawing2D.Matrix;
#endif
namespace DevExpress.XtraRichEdit.Mouse {
	public class XpfRichEditMouseHandlerStrategyFactory : RichEditMouseHandlerStrategyFactory {
		public override DragContentMouseHandlerStateBaseStrategy CreateDragContentMouseHandlerStateBaseStrategy(DragContentMouseHandlerStateBase state) {
			return new XpfDragContentMouseHandlerStateBaseStrategy(state);
		}
		public override DragFloatingObjectManuallyMouseHandlerStateStrategy CreateDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state) {
			return new XpfDragFloatingObjectManuallyMouseHandlerStateStrategy(state);
		}
		public override RichEditMouseHandlerStrategy CreateMouseHandlerStrategy(RichEditMouseHandler mouseHandler) {
			return new XpfRichEditMouseHandlerStrategy(mouseHandler);
		}
		public override ResizeTableRowMouseHandlerStateStrategy CreateResizeTableRowMouseHandlerStateStrategy(ResizeTableRowMouseHandlerState state) {
			return new XpfResizeTableRowMouseHandlerStateStrategy(state);
		}
		public override ResizeTableVirtualColumnMouseHandlerStateStrategy CreateResizeTableVirtualColumnMouseHandlerStateStrategy(ResizeTableVirtualColumnMouseHandlerState state) {
			return new XpfResizeTableVirtualColumnMouseHandlerStateStrategy(state);
		}
		public override RichEditRectangularObjectResizeMouseHandlerStateStrategy CreateRichEditRectangularObjectResizeMouseHandlerStateStrategy(RichEditRectangularObjectResizeMouseHandlerState state) {
			return new XpfRichEditRectangularObjectResizeMouseHandlerStateStrategy(state);
		}
	}
	public class XpfRichEditMouseHandlerStrategy : RichEditMouseHandlerStrategy {
		public XpfRichEditMouseHandlerStrategy(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public RichEditControl XpfControl { get { return (RichEditControl)Control; } }
		protected internal override void StartOfficeScroller(System.Drawing.Point clientPoint) {
		}
		protected internal override IOfficeScroller CreateOfficeScroller() {
			return null;
		}
		protected internal override void OnMouseUp(PlatformIndependentMouseEventArgs e) {
			if (e.Button == PlatformIndependentMouseButtons.Right) {
				MouseHandler.OnPopupMenu(new PlatformIndependentMouseEventArgs(PlatformIndependentMouseButtons.Right, 1, e.X, e.Y, 0));
				XpfControl.SetFocus();
				XpfControl.OnPopupMenu(XpfTypeConverter.ToPlatformPoint(e.Location));
			}
		}
		protected internal override PlatformIndependentMouseEventArgs CreateFakeMouseMoveEventArgs() {
			return new PlatformIndependentMouseEventArgs(PlatformIndependentMouseButtons.None, 0, 0, 0, 0);
		}
		protected internal override PlatformIndependentMouseEventArgs ConvertMouseEventArgs(PlatformIndependentMouseEventArgs screenMouseEventArgs) {
			DocumentLayoutUnitConverter unitConverter = Control.InnerControl.DocumentModel.LayoutUnitConverter;
			int x = unitConverter.PixelsToLayoutUnits(screenMouseEventArgs.X);
			int y = unitConverter.PixelsToLayoutUnits(screenMouseEventArgs.Y);
#if SL
			return new PlatformIndependentMouseEventArgs(screenMouseEventArgs.Button, screenMouseEventArgs.Clicks, x, y, screenMouseEventArgs.Delta, screenMouseEventArgs.Shift, screenMouseEventArgs.Ctrl);
#else
			OfficeMouseEventArgs richEditEventArgs = OfficeMouseEventArgs.Convert(screenMouseEventArgs);
			return new OfficeMouseEventArgs(richEditEventArgs.Button, richEditEventArgs.Clicks, x, y, richEditEventArgs.Delta, richEditEventArgs.Horizontal);
#endif
		}
		protected internal override PlatformIndependentDragEventArgs ConvertDragEventArgs(PlatformIndependentDragEventArgs screenDragEventArgs) {
			DocumentLayoutUnitConverter unitConverter = Control.InnerControl.DocumentModel.LayoutUnitConverter;
			int x = unitConverter.PixelsToLayoutUnits(screenDragEventArgs.X);
			int y = unitConverter.PixelsToLayoutUnits(screenDragEventArgs.Y);
			return new PlatformIndependentDragEventArgs(screenDragEventArgs.Data, screenDragEventArgs.KeyState, x, y, screenDragEventArgs.AllowedEffect, screenDragEventArgs.Effect);
		}
		protected internal override void AutoScrollerOnDragOver(System.Drawing.Point pt) {
		}
		protected internal override DragContentMouseHandlerStateBase CreateInternalDragState() {
			return new DragContentManuallyMouseHandlerState(MouseHandler);
		}
	}
	public class XpfDragFloatingObjectManuallyMouseHandlerStateStrategy : DragFloatingObjectManuallyMouseHandlerStateStrategy {
		Border feedbackShape;
		public XpfDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state) : base(state) {
		}
		public RichEditControl XpfControl { get { return (RichEditControl)Control; } }
		protected internal override OfficeImage CreateFeedbackImage(OfficeImage originalImage) {
			return originalImage;
		}
		protected internal override void HideVisualFeedbackCore(Rectangle bounds, PageViewInfo pageViewInfo) {
		}
		protected internal override void ShowVisualFeedbackCore(Rectangle bounds, PageViewInfo pageViewInfo, OfficeImage image) {
			Rect rect = XpfRichEditRectangularObjectResizeMouseHandlerStateStrategy.GetSelectionBounds(this.Control, bounds, pageViewInfo.ClientBounds);
			feedbackShape.Width = rect.Width;
			feedbackShape.Height = rect.Height;
			feedbackShape.SetValue(Canvas.LeftProperty, rect.Left);
			feedbackShape.SetValue(Canvas.TopProperty, rect.Top);
			Rect normalizedBounds = new Rect();
			normalizedBounds.X = 0;
			normalizedBounds.Y = 0;
			normalizedBounds.Width = rect.Width;
			normalizedBounds.Height = rect.Height;
			TransformMatrix matrix = XpfSelectionPainter.CreateTransform(State.RotationAngle, normalizedBounds, State.DocumentModel);
			if (matrix != null) {
				MatrixTransform transform = new MatrixTransform();
				transform.Matrix = XpfPainter.ToPlatformMatrix(matrix);
				feedbackShape.RenderTransform = transform;
			}
		}
		protected internal override void BeginVisualFeedback() {
			this.feedbackShape = new Border();
			if (!DXColor.IsTransparentOrEmpty(Run.Shape.OutlineColor)) {
				int outlineWidth = DocumentModel.UnitConverter.ModelUnitsToPixels(Run.Shape.OutlineWidth);
				feedbackShape.BorderThickness = new Thickness(outlineWidth);
				System.Windows.Media.Color outlineColor = XpfTypeConverter.ToPlatformColor(Run.Shape.OutlineColor);
				outlineColor.A = 153; 
				feedbackShape.BorderBrush = new SolidColorBrush(outlineColor);
			}
			System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();			
			if (State.FeedbackImage != null) {
				ImageBrush brush = new ImageBrush();
				brush.Opacity = 0.6;
				PlatformImage platformImage = XpfPainter.CreatePlatformImage(State.FeedbackImage);
				if (platformImage != null)
					brush.ImageSource = platformImage.Source;
				brush.Stretch = Stretch.Fill;
				rectangle.Fill = brush;
			}
			else {
				if (!DXColor.IsTransparentOrEmpty(Run.Shape.FillColor)) {
					System.Windows.Media.Color fillColor = XpfTypeConverter.ToPlatformColor(Run.Shape.FillColor);
					fillColor.A = 153; 
					rectangle.Fill = new SolidColorBrush(fillColor);
				}
			}
			feedbackShape.Child = rectangle;
			State.ShowVisualFeedback();
			XpfControl.Surface.Children.Add(this.feedbackShape);
		}
		protected internal override void EndVisualFeedback() {
			XpfControl.Surface.Children.Remove(this.feedbackShape);
		}
	}
	public class XpfResizeTableRowMouseHandlerStateStrategy : ResizeTableRowMouseHandlerStateStrategy {
		readonly XpfRichEditSizerSelection agSelection = new XpfRichEditSizerSelection();
		public XpfResizeTableRowMouseHandlerStateStrategy(ResizeTableRowMouseHandlerState state)
			: base(state) {
		}
		public RichEditControl XpfControl { get { return (RichEditControl)Control; } }
		protected internal System.Windows.Rect GetSelectionBounds(int y, Rectangle pageBounds) {
			Rectangle bounds = new Rectangle();
			bounds.Width = pageBounds.Width;
			bounds.Y = y;
			bounds.Height = 1;
			return XpfRichEditRectangularObjectResizeMouseHandlerStateStrategy.GetSelectionBounds(Control, bounds, pageBounds);
		}
		protected internal override void DrawReversibleLineCore(int y) {
			agSelection.AddRect(GetSelectionBounds(y, PageViewInfo.ClientBounds), null);
			agSelection.Recalculate();
		}
		protected internal override void BeginVisualFeedback() {
			ShowVisualFeedback();
			XpfControl.Surface.Children.Add(agSelection);
		}
		protected internal override void ShowVisualFeedback() {
			State.DrawReversibleLineCore();
		}
		protected internal override void EndVisualFeedback() {
			XpfControl.Surface.Children.Remove(agSelection);
		}
	}
	public class XpfResizeTableVirtualColumnMouseHandlerStateStrategy : ResizeTableVirtualColumnMouseHandlerStateStrategy {
		readonly XpfRichEditSizerSelection agSelection = new XpfRichEditSizerSelection();
		public XpfResizeTableVirtualColumnMouseHandlerStateStrategy(ResizeTableVirtualColumnMouseHandlerState state) : base(state) {
		}
		public RichEditControl XpfControl { get { return (RichEditControl)Control; } }
		protected internal System.Windows.Rect GetSelectionBounds(int x, Rectangle pageBounds) {
			Rectangle bounds = new Rectangle();
			bounds.Width = 1;
			bounds.X = x;
			bounds.Height = pageBounds.Height;
			return XpfRichEditRectangularObjectResizeMouseHandlerStateStrategy.GetSelectionBounds(Control, bounds, pageBounds);
		}
		protected internal override void DrawReversibleLineCore(int x) {
			agSelection.AddRect(GetSelectionBounds(x, PageViewInfo.ClientBounds), null);
			agSelection.Recalculate();
		}
		protected internal override void BeginVisualFeedback() {
			ShowVisualFeedback();
			XpfControl.Surface.Children.Add(agSelection);
		}
		protected internal override void ShowVisualFeedback() {
			State.DrawReversibleLineCore();
		}
		protected internal override void EndVisualFeedback() {
			XpfControl.Surface.Children.Remove(agSelection);
		}
		protected internal override void HideVisualFeedback() {
			State.DrawReversibleLineCore();
		}
	}
	public class XpfDragContentMouseHandlerStateBaseStrategy : DragContentMouseHandlerStateBaseStrategy {		
		public XpfDragContentMouseHandlerStateBaseStrategy(DragContentMouseHandlerStateBase state) : base(state) {
		}
		protected internal override DragCaretVisualizer CreateCaretVisualizer() {
			return new XpfDragCaretVisualizer((RichEditControl)Control);
		}
		public override void Finish() {
		}
	}
	public class XpfRichEditRectangularObjectResizeMouseHandlerStateStrategy : RichEditRectangularObjectResizeMouseHandlerStateStrategy {
		XpfRichEditSizerSelection agSelection = new XpfRichEditSizerSelection();
		public XpfRichEditRectangularObjectResizeMouseHandlerStateStrategy(RichEditRectangularObjectResizeMouseHandlerState state)
			: base(state) {
		}
		public RichEditControl SilverlightControl { get { return (RichEditControl)Control; } }
		#region GetSelectionBounds
		protected internal System.Windows.Rect GetSelectionBounds(Rectangle viewInfoBounds, Rectangle pageBounds) {
			return GetSelectionBounds(Control, viewInfoBounds, pageBounds);
		}
		protected internal static System.Windows.Rect GetSelectionBounds(IRichEditControl control, Rectangle viewInfoBounds, Rectangle pageBounds) {
			double HOffset = control.InnerControl.ActiveView.HorizontalScrollController.GetPhysicalLeftInvisibleWidth();
			double Zoom = control.InnerControl.ActiveView.ZoomFactor;
			DocumentLayoutUnitConverter unitConverter = control.InnerControl.DocumentModel.LayoutUnitConverter;
			return new System.Windows.Rect(Math.Floor(unitConverter.LayoutUnitsToPixelsF((float)(pageBounds.Left + viewInfoBounds.Left * Zoom - HOffset))),
							Math.Floor(unitConverter.LayoutUnitsToPixelsF((float)(pageBounds.Top + viewInfoBounds.Top * Zoom))),
							Math.Ceiling(unitConverter.LayoutUnitsToPixelsF((float)(viewInfoBounds.Width * Zoom))),
							Math.Ceiling(unitConverter.LayoutUnitsToPixelsF((float)(viewInfoBounds.Height * Zoom))));
		}
		#endregion
		protected internal override void BeginVisualFeedback() {
			State.ShowVisualFeedback();
		}
		protected internal override void ShowVisualFeedback() {
			if (!SilverlightControl.Surface.Children.Contains(agSelection))
				SilverlightControl.Surface.Children.Add(agSelection);
			Box box = State.HotZone.Box;
			TransformMatrix transform = XpfSelectionPainter.CreateTransform(State.RotationAngle, GetSelectionBounds(box.ActualSizeBounds, PageViewInfo.ClientBounds), State.DocumentModel);
			agSelection.AddRect(GetSelectionBounds(ObjectActualBounds, PageViewInfo.ClientBounds), transform);
			agSelection.Recalculate();
		}
		protected internal override void EndVisualFeedback() {
			SilverlightControl.Surface.Children.Remove(agSelection);
		}
		protected internal override void HideVisualFeedback() {
		}
	}
}
