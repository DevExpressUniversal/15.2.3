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
using DevExpress.Utils;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraRichEdit.Mouse {
	#region RichEditAutoScroller
	public class RichEditAutoScroller : AutoScroller {
		public RichEditAutoScroller(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public new RichEditMouseHandler MouseHandler { get { return (RichEditMouseHandler)base.MouseHandler; } }
		protected override void PopulateHotZones() {
			HotZones.Add(new RichEditVerticalScrollBackwardHotZone(MouseHandler));
			HotZones.Add(new RichEditVerticalScrollForwardHotZone(MouseHandler));
			HotZones.Add(new RichEditHorizontalScrollBackwardHotZone(MouseHandler));
			HotZones.Add(new RichEditHorizontalScrollForwardHotZone(MouseHandler));
		}
	}
	#endregion
	#region RichEditAutoScrollerHotZone (abstract class)
	public abstract class RichEditAutoScrollerHotZone : AutoScrollerHotZone {
		readonly RichEditMouseHandler mouseHandler;
		protected RichEditAutoScrollerHotZone(RichEditMouseHandler mouseHandler) {
			Guard.ArgumentNotNull(mouseHandler, "mouseHandler");
			this.mouseHandler = mouseHandler;
		}
		protected internal RichEditMouseHandler MouseHandler { get { return mouseHandler; } }
		protected internal IRichEditControl Control { get { return MouseHandler.Control; } }
		protected internal virtual Rectangle CalculateCompatibleViewBounds() {
			IRichEditControl control = Control;
			Rectangle result = control.ViewBounds;
			result.X = 0;
			result.Y = 0;
			return result;
		}
	}
	#endregion
	#region RichEditVerticalScrollHotZone (abstract class)
	public abstract class RichEditVerticalScrollHotZone : RichEditAutoScrollerHotZone {
		protected RichEditVerticalScrollHotZone(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public override void PerformAutoScroll() {
			MouseEventArgs args = MouseHandler.CreateFakeMouseMoveEventArgs();
			ScrollVerticallyByPhysicalOffsetCommand command = new ScrollVerticallyByPhysicalOffsetCommand(Control);
			command.PhysicalOffset = CalculatePhysicalOffset(args);
			command.Execute();
			ContinueSelectionByRangesMouseHandlerState state = MouseHandler.State as ContinueSelectionByRangesMouseHandlerState;
			if (state != null)
				state.ContinueSelection(args);
		}
		protected internal abstract int CalculatePhysicalOffset(MouseEventArgs args);
	}
	#endregion
	#region RichEditVerticalScrollForwardHotZone
	public class RichEditVerticalScrollForwardHotZone : RichEditVerticalScrollHotZone {
		public RichEditVerticalScrollForwardHotZone(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			Rectangle result = CalculateCompatibleViewBounds();
			DocumentLayoutUnitConverter unitConverter = Control.InnerControl.DocumentModel.LayoutUnitConverter;
			int height = unitConverter.PixelsToLayoutUnits(16);
			result.Y = result.Bottom - height;
			result.Height = Int32.MaxValue / 4;
			return result;
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition.Y >= bounds.Top)
				return Rectangle.FromLTRB(bounds.Left, mousePosition.Y + 1, bounds.Right, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			PageViewInfoGenerator generator = Control.InnerControl.ActiveView.PageViewInfoGenerator;
			if (generator.TopInvisibleHeight >= generator.TotalHeight - generator.VisibleHeight)
				return false;
			return mousePosition.Y >= Bounds.Top;
		}
		protected internal override int CalculatePhysicalOffset(MouseEventArgs args) {
			DocumentLayoutUnitConverter unitConverter = Control.InnerControl.DocumentModel.LayoutUnitConverter;
			if ((args.Y - Bounds.Top) > unitConverter.PixelsToLayoutUnits(10))
				return unitConverter.DocumentsToLayoutUnits(150);
			else
				return unitConverter.DocumentsToLayoutUnits(50);
		}
	}
	#endregion
	#region RichEditVerticalScrollBackwardHotZone
	public class RichEditVerticalScrollBackwardHotZone : RichEditVerticalScrollHotZone {
		public RichEditVerticalScrollBackwardHotZone(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			Rectangle result = CalculateCompatibleViewBounds();
			result.Y = Int32.MinValue / 4;
			result.Height = -result.Y;
			return result;
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition.Y <= bounds.Bottom)
				return Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, mousePosition.Y - 1);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			PageViewInfoGenerator generator = Control.InnerControl.ActiveView.PageViewInfoGenerator;
			if (generator.TopInvisibleHeight <= 0)
				return false;
			return mousePosition.Y <= Bounds.Bottom;
		}
		protected internal override int CalculatePhysicalOffset(MouseEventArgs args) {
			DocumentLayoutUnitConverter unitConverter = Control.InnerControl.DocumentModel.LayoutUnitConverter;
			if ((Bounds.Bottom - args.Y) > unitConverter.PixelsToLayoutUnits(10))
				return -unitConverter.DocumentsToLayoutUnits(150);
			else
				return -unitConverter.DocumentsToLayoutUnits(50);
		}
	}
	#endregion
	#region RichEditHorizontalScrollHotZone (abstract class)
	public abstract class RichEditHorizontalScrollHotZone : RichEditAutoScrollerHotZone {
		protected RichEditHorizontalScrollHotZone(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public override void PerformAutoScroll() {
			MouseEventArgs args = MouseHandler.CreateFakeMouseMoveEventArgs();
			ScrollHorizontallyByPhysicalOffsetCommand command = new ScrollHorizontallyByPhysicalOffsetCommand(Control);
			command.PhysicalOffset = CalculatePhysicalOffset(args);
			command.Execute();
			ContinueSelectionByRangesMouseHandlerState state = MouseHandler.State as ContinueSelectionByRangesMouseHandlerState;
			if (state != null)
				state.ContinueSelection(args);
		}
		protected internal abstract int CalculatePhysicalOffset(MouseEventArgs args);
	}
	#endregion
	#region RichEditHorizontalScrollForwardHotZone
	public class RichEditHorizontalScrollForwardHotZone : RichEditHorizontalScrollHotZone {
		public RichEditHorizontalScrollForwardHotZone(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			Rectangle result = CalculateCompatibleViewBounds();
			DocumentLayoutUnitConverter unitConverter = Control.InnerControl.DocumentModel.LayoutUnitConverter;
			int width = unitConverter.PixelsToLayoutUnits(16);
			result.X = result.Right - width;
			result.Width = Int32.MaxValue / 4;
			return result;
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition.X >= bounds.Left)
				return Rectangle.FromLTRB(mousePosition.X + 1, bounds.Top, bounds.Right, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			PageViewInfoGenerator generator = Control.InnerControl.ActiveView.PageViewInfoGenerator;
			if (generator.VisibleWidth >= generator.TotalWidth)
				return false;
			if (generator.LeftInvisibleWidth >= generator.TotalWidth - generator.VisibleWidth)
				return false;
			return mousePosition.X >= Bounds.Left;
		}
		protected internal override int CalculatePhysicalOffset(MouseEventArgs args) {
			DocumentLayoutUnitConverter unitConverter = Control.InnerControl.DocumentModel.LayoutUnitConverter;
			if ((args.X - Bounds.Left) > unitConverter.PixelsToLayoutUnits(10))
				return unitConverter.DocumentsToLayoutUnits(150);
			else
				return unitConverter.DocumentsToLayoutUnits(50);
		}
	}
	#endregion
	#region RichEditHorizontalScrollBackwardHotZone
	public class RichEditHorizontalScrollBackwardHotZone : RichEditHorizontalScrollHotZone {
		public RichEditHorizontalScrollBackwardHotZone(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected override Rectangle CalculateHotZoneBounds() {
			Rectangle result = CalculateCompatibleViewBounds();
			result.X = Int32.MinValue / 4;
			result.Width = -result.X;
			return result;
		}
		protected override Rectangle AdjustHotZoneBounds(Rectangle bounds, Point mousePosition) {
			if (mousePosition.X <= bounds.Right)
				return Rectangle.FromLTRB(bounds.Left, bounds.Top, mousePosition.X - 1, bounds.Bottom);
			else
				return bounds;
		}
		public override bool CanActivate(Point mousePosition) {
			PageViewInfoGenerator generator = Control.InnerControl.ActiveView.PageViewInfoGenerator;
			if (generator.VisibleWidth >= generator.TotalWidth)
				return false;
			if (generator.LeftInvisibleWidth <= 0)
				return false;
			return mousePosition.X <= Bounds.Right;
		}
		protected internal override int CalculatePhysicalOffset(MouseEventArgs args) {
			DocumentLayoutUnitConverter unitConverter = Control.InnerControl.DocumentModel.LayoutUnitConverter;
			if ((Bounds.Right - args.X) > unitConverter.PixelsToLayoutUnits(10))
				return -unitConverter.DocumentsToLayoutUnits(150);
			else
				return -unitConverter.DocumentsToLayoutUnits(50);
		}
	}
	#endregion
}
