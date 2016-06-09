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

using DevExpress.Office.Internal;
using DevExpress.Office.Layout;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region CommentLayout
	public class CommentLayout : DocumentItemLayout {
		#region Fields
		CommentLayoutItemCollection items;
		Page lastPage = null;
		#endregion
		public CommentLayout(SpreadsheetView view)
			: base(view) {
			this.items = new CommentLayoutItemCollection();
		}
		#region Properties
		public CommentLayoutItemCollection Items { get { return items; } }
		#endregion
		public override void Update(Page page) {
			this.items = new CommentLayoutItemCollection();
			foreach (CommentBox box in page.CommentBoxes) {
				if (box.IsHidden)
					continue;
				CommentLayoutItem item = new CommentLayoutItem();
				item.Update(box, View);
				items.Add(item);
			}
			lastPage = page;
		}
		public override void Invalidate() {
			if (items != null) {
				int count = items.Count;
				for (int i = 0; i < count; i++)
					items[i] = null;
				items = null;
			}
		}
		protected internal override HotZone CalculateHotZone(Point point, Page page) {
			if (page != null && !object.ReferenceEquals(lastPage, page))
				Update(page);
			float zoomFactor = View.ZoomFactor;
			foreach (CommentLayoutItem item in items) {
				HotZone hotZone = item.CalculateHotZone(point, zoomFactor, LayoutUnitConverter);
				if (hotZone != null)
					return hotZone;
			}
			return null;
		}
	}
	#endregion
	#region CommentLayoutItem
	public class CommentLayoutItem {
		#region Fields
		const int dragOffsetInPixels = 4;
		const int resizeHotZoneInPixels = 7;
		HotZoneCollection resizeHotZones;
		HotZoneCollection dragHotZones;
		#endregion
		public CommentLayoutItem() {
			this.resizeHotZones = new HotZoneCollection();
			this.dragHotZones = new HotZoneCollection();
		}
		#region Properties
		public HotZoneCollection ResizeHotZones { get { return resizeHotZones; } }
		#endregion
		protected internal void Update(CommentBox box, SpreadsheetView view) {
			UpdateDragHotZones(box, view);
			UpdateResizeHotZones(box, view);
		}
		void UpdateDragHotZones(CommentBox box, SpreadsheetView view) {
			this.dragHotZones = new HotZoneCollection();
			AddDragHotZones(box, view);
		}
		void AddDragHotZones(CommentBox box, SpreadsheetView view) {
			SpreadsheetBehaviorOptions options = view.DocumentModel.BehaviorOptions;
			if (!options.Comment.MoveAllowed || !options.DragAllowed)
				return;
			IGestureStateIndicator indicator = view.Control.InnerControl;
			DocumentLayoutUnitConverter converter = view.DocumentModel.LayoutUnitConverter;
			int offsetInLayouts = converter.PixelsToLayoutUnits(dragOffsetInPixels, DocumentModel.Dpi);
			Rectangle bounds = box.Bounds;
			int left = bounds.Left;
			int right = bounds.Right;
			int top = bounds.Top;
			int bottom = bounds.Bottom;
			AddDragHotZone(box, indicator, Rectangle.FromLTRB(left - offsetInLayouts, top - offsetInLayouts, left + offsetInLayouts, bottom + offsetInLayouts));
			AddDragHotZone(box, indicator, Rectangle.FromLTRB(right - offsetInLayouts, top - offsetInLayouts, right + offsetInLayouts, bottom + offsetInLayouts));
			AddDragHotZone(box, indicator, Rectangle.FromLTRB(left, top - offsetInLayouts, right, top + offsetInLayouts));
			AddDragHotZone(box, indicator, Rectangle.FromLTRB(left, bottom - offsetInLayouts, right, bottom + offsetInLayouts));
		}
		void AddDragHotZone(CommentBox box, IGestureStateIndicator indicator, Rectangle bounds) {
			CommentDragHotZone dragHotZone = new CommentDragHotZone(box, indicator);
			dragHotZone.Bounds = bounds;
			dragHotZones.Add(dragHotZone);
		}
		void UpdateResizeHotZones(CommentBox box, SpreadsheetView view) {
			this.resizeHotZones = new HotZoneCollection();
			AddResizeHotZones(box, view);
		}
		void AddResizeHotZones(CommentBox box, SpreadsheetView view) {
			DocumentModel model = view.DocumentModel;
			SpreadsheetBehaviorOptions options = model.BehaviorOptions;
			int selectedIndex = model.ActiveSheet.Selection.SelectedCommentIndex;
			if (!options.Comment.ResizeAllowed || !options.DragAllowed || selectedIndex != box.GetCommentIndex())
				return;
			IGestureStateIndicator indicator = view.Control.InnerControl;
			DocumentLayoutUnitConverter converter = view.DocumentModel.LayoutUnitConverter;
			int resizeHotZoneInLayouts = converter.PixelsToLayoutUnits(resizeHotZoneInPixels, DocumentModel.Dpi);
			Rectangle bounds = PictureSelectionLayoutItem.ValidateBounds(box.Bounds, resizeHotZoneInLayouts);
			int left = bounds.Left - 1;
			int right = bounds.Right;
			int top = bounds.Top - 1;
			int bottom = bounds.Bottom;
			int middleX = bounds.Left + bounds.Width / 2 - resizeHotZoneInLayouts / 2;
			int middleY = bounds.Top + bounds.Height / 2 - resizeHotZoneInLayouts / 2;
			int minCommentBounds = converter.PixelsToLayoutUnits(Comment.MinSizeInPixels, DocumentModel.Dpi);
			AddResizeHotZone(new CommentResizeTopLeftHotZone(box, indicator, minCommentBounds), new Rectangle(left - resizeHotZoneInLayouts, top - resizeHotZoneInLayouts, resizeHotZoneInLayouts, resizeHotZoneInLayouts));
			AddResizeHotZone(new CommentResizeTopRightHotZone(box, indicator, minCommentBounds), new Rectangle(right, top - resizeHotZoneInLayouts, resizeHotZoneInLayouts, resizeHotZoneInLayouts));
			AddResizeHotZone(new CommentResizeBottomLeftHotZone(box, indicator, minCommentBounds), new Rectangle(left - resizeHotZoneInLayouts, bottom, resizeHotZoneInLayouts, resizeHotZoneInLayouts));
			AddResizeHotZone(new CommentResizeBottomRightHotZone(box, indicator, minCommentBounds), new Rectangle(right, bottom, resizeHotZoneInLayouts, resizeHotZoneInLayouts));
			AddResizeHotZone(new CommentResizeTopMiddleHotZone(box, indicator, minCommentBounds), new Rectangle(middleX, top - resizeHotZoneInLayouts, resizeHotZoneInLayouts, resizeHotZoneInLayouts));
			AddResizeHotZone(new CommentResizeBottomMiddleHotZone(box, indicator, minCommentBounds), new Rectangle(middleX, bottom, resizeHotZoneInLayouts, resizeHotZoneInLayouts));
			AddResizeHotZone(new CommentResizeLeftMiddleHotZone(box, indicator, minCommentBounds), new Rectangle(left - resizeHotZoneInLayouts, middleY, resizeHotZoneInLayouts, resizeHotZoneInLayouts));
			AddResizeHotZone(new CommentResizeRightMiddleHotZone(box, indicator, minCommentBounds), new Rectangle(right, middleY, resizeHotZoneInLayouts, resizeHotZoneInLayouts));
		}
		void AddResizeHotZone(CommentResizeHotZoneBase hotZone, Rectangle bounds) {
			hotZone.Bounds = bounds;
			resizeHotZones.Add(hotZone);
		}
		protected internal HotZone CalculateHotZone(Point point, float zoomFactor, DocumentLayoutUnitConverter converter) {
			HotZone result = HotZoneCalculator.CalculateHotZone(resizeHotZones, point, zoomFactor, converter);
			if (result != null)
				return result;
			return HotZoneCalculator.CalculateHotZone(dragHotZones, point, zoomFactor, converter);
		}
	}
	#endregion
	#region CommentLayoutItemCollection
	public class CommentLayoutItemCollection : List<CommentLayoutItem> {
	}
	#endregion
	#region CommentHotZoneBase (abstract class)
	public abstract class CommentHotZoneBase : HotZone {
		#region Fields
		readonly CommentBox box;
		#endregion
		protected CommentHotZoneBase(CommentBox box, IGestureStateIndicator gestureStateIndicator)
			: base(gestureStateIndicator) {
			Guard.ArgumentNotNull(box, "box");
			this.box = box;
		}
		#region Properties
		public CommentBox Box { get { return box; } }
		#endregion
	}
	#endregion
	#region CommentDragHotZone
	public class CommentDragHotZone : CommentHotZoneBase {
		public CommentDragHotZone(CommentBox box, IGestureStateIndicator gestureStateIndicator)
			: base(box, gestureStateIndicator) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.DragRange; } }
		#endregion
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			SelectComment();
			result.CommentBox = Box;
			MouseHandlerState dragState = handler.CreateCommentDragState(result);
			handler.SwitchStateCore(dragState, result.PhysicalPoint);
		}
		void SelectComment() {
			int index = Box.GetCommentIndex();
			Box.Worksheet.Selection.SelectComment(index);
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region CommentResizeHotZoneBase (abstract class)
	public abstract class CommentResizeHotZoneBase : CommentHotZoneBase, IResizeHotZone {
		#region Fields
		readonly int minBoundsInLayouts;
		#endregion
		protected CommentResizeHotZoneBase(CommentBox box, IGestureStateIndicator gestureStateIndicator, int minBoundsSizeInLayouts)
			: base(box, gestureStateIndicator) {
			this.minBoundsInLayouts = minBoundsSizeInLayouts;
		}
		#region Properties
		public ResizeHotZoneType Type { get { return ResizeHotZoneType.Rectangle; } }
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.Hand; } }
		protected internal int MinBoundsInLayouts { get { return minBoundsInLayouts; } }
		#endregion
		public override void Activate(Mouse.SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			result.CommentBox = Box;
			MouseHandlerState resizeState = handler.CreateCommentResizeState(result, this);
			handler.SwitchStateCore(resizeState, result.PhysicalPoint);
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
		public Rectangle CreateValidBoxBounds(Point point) {
			return CreateValidBoxBoundsCore(point);
		}
		protected internal abstract Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY);
		protected internal abstract Rectangle CreateValidBoxBoundsCore(Point point);
		protected internal Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, Rectangle originalBounds) {
			if (!Box.LockAspectRatio)
				return bounds;
			float aspectX = Math.Max(1, 100 * bounds.Width / (float)originalBounds.Width);
			float aspectY = Math.Max(1, 100 * bounds.Height / (float)originalBounds.Height);
			int width, height;
			if (aspectX > aspectY) {
				width = bounds.Width;
				height = (int)Math.Round(originalBounds.Height * aspectX / 100.0f);
			}
			else {
				width = (int)Math.Round(originalBounds.Width * aspectY / 100.0f);
				height = bounds.Height;
			}
			return ForceKeepOriginalAspectRatio(bounds, width, height);
		}
		protected internal virtual Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int width, int height) {
			return bounds;
		}
	}
	#endregion
	#region CommentResizeTopLeftHotZone
	public class CommentResizeTopLeftHotZone : CommentResizeHotZoneBase {
		public CommentResizeTopLeftHotZone(CommentBox box, IGestureStateIndicator gestureStateIndicator, int minBoundsSizeInLayouts)
			: base(box, gestureStateIndicator, minBoundsSizeInLayouts) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.SizeNWSE; } }
		#endregion
		protected internal override Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY) {
			int right = originalBounds.Right;
			int bottom = originalBounds.Bottom;
			int top = Math.Min(bottom - MinBoundsInLayouts, originalBounds.Top + offsetY);
			int left = Math.Min(right - MinBoundsInLayouts, originalBounds.Left + offsetX);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle bounds = Box.Bounds;
			int right = bounds.Right;
			int bottom = bounds.Bottom;
			int top = Math.Min(bottom - MinBoundsInLayouts, point.Y);
			int left = Math.Min(right - MinBoundsInLayouts, point.X);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.X = bounds.Right - desiredWidth;
			bounds.Y = bounds.Bottom - desiredHeight;
			bounds.Width = desiredWidth;
			bounds.Height = desiredHeight;
			return bounds;
		}
	}
	#endregion
	#region CommentResizeTopMiddleHotZone
	public class CommentResizeTopMiddleHotZone : CommentResizeHotZoneBase {
		public CommentResizeTopMiddleHotZone(CommentBox box, IGestureStateIndicator gestureStateIndicator, int minBoundsSizeInLayouts)
			: base(box, gestureStateIndicator, minBoundsSizeInLayouts) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.SizeNS; } }
		#endregion
		protected internal override Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY) {
			int left = originalBounds.Left;
			int right = originalBounds.Right;
			int bottom = originalBounds.Bottom;
			int top = Math.Min(bottom - MinBoundsInLayouts, originalBounds.Top + offsetY);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle bounds = Box.Bounds;
			int right = bounds.Right;
			int bottom = bounds.Bottom;
			int top = Math.Min(bottom - MinBoundsInLayouts, point.Y);
			int left = bounds.Left;
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
	}
	#endregion
	#region CommentResizeTopRightHotZone
	public class CommentResizeTopRightHotZone : CommentResizeHotZoneBase {
		public CommentResizeTopRightHotZone(CommentBox box, IGestureStateIndicator gestureStateIndicator, int minBoundsSizeInLayouts)
			: base(box, gestureStateIndicator, minBoundsSizeInLayouts) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.SizeNESW; } }
		#endregion
		protected internal override Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY) {
			int left = originalBounds.Left;
			int bottom = originalBounds.Bottom;
			int top = Math.Min(bottom - MinBoundsInLayouts, originalBounds.Top + offsetY);
			int right = Math.Max(left + MinBoundsInLayouts, originalBounds.Right + offsetX);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle bounds = Box.Bounds;
			int left = bounds.Left;
			int right = Math.Max(left + MinBoundsInLayouts, point.X);
			int bottom = bounds.Bottom;
			int top = Math.Min(bottom - MinBoundsInLayouts, point.Y);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.Width = desiredWidth;
			bounds.Y = bounds.Bottom - desiredHeight;
			bounds.Height = desiredHeight;
			return bounds;
		}
	}
	#endregion
	#region CommentResizeBottomLeftHotZone
	public class CommentResizeBottomLeftHotZone : CommentResizeHotZoneBase {
		public CommentResizeBottomLeftHotZone(CommentBox box, IGestureStateIndicator gestureStateIndicator, int minBoundsSizeInLayouts)
			: base(box, gestureStateIndicator, minBoundsSizeInLayouts) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.SizeNESW; } }
		#endregion
		protected internal override Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY) {
			int top = originalBounds.Top;
			int right = originalBounds.Right;
			int left = Math.Min(right - MinBoundsInLayouts, originalBounds.Left + offsetX);
			int bottom = Math.Max(top + MinBoundsInLayouts, originalBounds.Bottom + offsetY);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle bounds = Box.Bounds;
			int top = bounds.Top;
			int right = bounds.Right;
			int left = Math.Min(right - MinBoundsInLayouts, point.X);
			int bottom = Math.Max(top + MinBoundsInLayouts, point.Y);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.X = bounds.Right - desiredWidth;
			bounds.Width = desiredWidth;
			bounds.Height = desiredHeight;
			return bounds;
		}
	}
	#endregion
	#region CommentResizeBottomMiddleHotZone
	public class CommentResizeBottomMiddleHotZone : CommentResizeHotZoneBase {
		public CommentResizeBottomMiddleHotZone(CommentBox box, IGestureStateIndicator gestureStateIndicator, int minBoundsSizeInLayouts)
			: base(box, gestureStateIndicator, minBoundsSizeInLayouts) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.SizeNS; } }
		#endregion
		protected internal override Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY) {
			int top = originalBounds.Top;
			int left = originalBounds.Left;
			int right = originalBounds.Right;
			int bottom = Math.Max(top + MinBoundsInLayouts, originalBounds.Bottom + offsetY);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle bounds = Box.Bounds;
			int top = bounds.Top;
			int right = bounds.Right;
			int left = bounds.Left;
			int bottom = Math.Max(top + MinBoundsInLayouts, point.Y);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
	}
	#endregion
	#region CommentResizeBottomRightHotZone
	public class CommentResizeBottomRightHotZone : CommentResizeHotZoneBase {
		public CommentResizeBottomRightHotZone(CommentBox box, IGestureStateIndicator gestureStateIndicator, int minBoundsSizeInLayouts)
			: base(box, gestureStateIndicator, minBoundsSizeInLayouts) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.SizeNWSE; } }
		#endregion
		protected internal override Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY) {
			int top = originalBounds.Top;
			int left = originalBounds.Left;
			int right = Math.Max(left + MinBoundsInLayouts, originalBounds.Right + offsetX);
			int bottom = Math.Max(top + MinBoundsInLayouts, originalBounds.Bottom + offsetY);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle bounds = Box.Bounds;
			int left = bounds.Left;
			int top = bounds.Top;
			int right = Math.Max(left + MinBoundsInLayouts, point.X);
			int bottom = Math.Max(top + MinBoundsInLayouts, point.Y);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle ForceKeepOriginalAspectRatio(Rectangle bounds, int desiredWidth, int desiredHeight) {
			bounds.Width = desiredWidth;
			bounds.Height = desiredHeight;
			return bounds;
		}
	}
	#endregion
	#region CommentResizeLeftMiddleHotZone
	public class CommentResizeLeftMiddleHotZone : CommentResizeHotZoneBase {
		public CommentResizeLeftMiddleHotZone(CommentBox box, IGestureStateIndicator gestureStateIndicator, int minBoundsSizeInLayouts)
			: base(box, gestureStateIndicator, minBoundsSizeInLayouts) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.SizeWE; } }
		#endregion
		protected internal override Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY) {
			int top = originalBounds.Top;
			int right = originalBounds.Right;
			int bottom = originalBounds.Bottom;
			int left = Math.Min(right - MinBoundsInLayouts, originalBounds.Left + offsetX);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle bounds = Box.Bounds;
			int bottom = bounds.Bottom;
			int top = bounds.Top;
			int right = bounds.Right;
			int left = Math.Min(right - MinBoundsInLayouts, point.X);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
	}
	#endregion
	#region CommentResizeRightMiddleHotZone
	public class CommentResizeRightMiddleHotZone : CommentResizeHotZoneBase {
		public CommentResizeRightMiddleHotZone(CommentBox box, IGestureStateIndicator gestureStateIndicator, int minBoundsSizeInLayouts)
			: base(box, gestureStateIndicator, minBoundsSizeInLayouts) {
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.SizeWE; } }
		#endregion
		protected internal override Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY) {
			int top = originalBounds.Top;
			int left = originalBounds.Left;
			int bottom = originalBounds.Bottom;
			int right = Math.Max(left + MinBoundsInLayouts, originalBounds.Right + offsetX);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		protected internal override Rectangle CreateValidBoxBoundsCore(Point point) {
			Rectangle bounds = Box.Bounds;
			int top = bounds.Top;
			int left = bounds.Left;
			int bottom = bounds.Bottom;
			int right = Math.Max(left + MinBoundsInLayouts, point.X);
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
	}
	#endregion
}
