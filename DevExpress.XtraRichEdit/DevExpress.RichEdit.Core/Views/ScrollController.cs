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
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Diagnostics;
namespace DevExpress.XtraRichEdit.Internal.PrintLayout {
#region VerticalScrollBarAdapter
	public class VerticalScrollBarAdapter : ScrollBarAdapter {
		bool synchronized;
		public VerticalScrollBarAdapter(IOfficeScrollbar scrollBar, IPlatformSpecificScrollBarAdapter adapter)
			: base(scrollBar, adapter) {
		}
		protected override bool DeferredScrollBarUpdate { get { return true; } }
		protected override bool Synchronized { get { return synchronized; } set { synchronized = value; } }
	}
#endregion
#region HorizontalScrollBarAdapter
	public class HorizontalScrollBarAdapter : ScrollBarAdapter {
		public HorizontalScrollBarAdapter(IOfficeScrollbar scrollBar, IPlatformSpecificScrollBarAdapter adapter)
			: base(scrollBar, adapter) {
		}
		protected override bool DeferredScrollBarUpdate { get { return false; } }
		protected override bool Synchronized { get { return true; } set { } }
	}
#endregion
#region RichEditViewScrollControllerBase (abstract class)
	public abstract partial class RichEditViewScrollControllerBase : OfficeScrollControllerBase {
#region Fields
		readonly RichEditView view;
#endregion
		protected RichEditViewScrollControllerBase(RichEditView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			Initialize();
		}
#region Properties
		public RichEditView View { get { return view; } }
#endregion
		public override void UpdateScrollBar() {
			if (ScrollBar == null)
				return;
			if (!Object.ReferenceEquals(View, View.Control.InnerControl.ActiveView))
				return;
			Debug.Assert(Object.ReferenceEquals(View, View.Control.InnerControl.ActiveView));
			base.UpdateScrollBar();
		}
	}
#endregion
#region RichEditViewVerticalScrollController (abstract class)
	public abstract class RichEditViewVerticalScrollController : RichEditViewScrollControllerBase {
		protected RichEditViewVerticalScrollController(RichEditView view)
			: base(view) {
		}
		public override IOfficeScrollbar ScrollBar {
			get {
				if (View.Control == null)
					return null;
				if (View.Control.InnerControl == null)
					return null;
				return View.Control.InnerControl.VerticalScrollBar;
			}
		}
		protected override ScrollBarAdapter CreateScrollBarAdapter() {
			return new VerticalScrollBarAdapter(ScrollBar, View.Control.CreatePlatformSpecificScrollBarAdapter());
		}
		public long GetTopInvisibleHeight() {
			return ScrollBarAdapter.Value;
		}
		protected internal abstract bool IsScrollTypeValid(ScrollEventArgs e);
		protected override void OnScroll(object sender, ScrollEventArgs e) {
			if (UpdatePageNumberOnScroll(e))
				return;
			if (!IsScrollTypeValid(e))
				return;
			int delta = CalculateScrollDelta(e);
			int value = (int)e.NewValue;
			if (delta != 0) {
				if (delta == 1)
					value = ScrollLineDown();
				else if (delta == -1)
					value = ScrollLineUp();
				else
					ApplyNewScrollValue(value);
				View.OnVerticalScroll();
				ApplyNewScrollValueToScrollEventArgs(e, value);
			}
		}
		protected internal abstract int CalculateScrollDelta(ScrollEventArgs e);
		protected internal abstract void ApplyNewScrollValue(int value);
		protected internal abstract void ApplyNewScrollValueToScrollEventArgs(ScrollEventArgs e, int value);
		protected internal abstract bool UpdatePageNumberOnScroll(ScrollEventArgs e);
		protected internal virtual bool ScrollPageUp() {
			return ScrollBarAdapter.SetRawScrollBarValue(ScrollBarAdapter.GetPageUpRawScrollBarValue());
		}
		protected internal virtual bool ScrollPageDown() {
			return ScrollBarAdapter.SetRawScrollBarValue(ScrollBarAdapter.GetPageDownRawScrollBarValue());
		}
		protected internal virtual int ScrollLineUp() {
			return ScrollLineUpDown(-View.DocumentLayout.UnitConverter.DocumentsToLayoutUnits(50));
		}
		protected internal virtual int ScrollLineDown() {
			return ScrollLineUpDown(View.DocumentLayout.UnitConverter.DocumentsToLayoutUnits(50));
		}
		protected internal virtual int ScrollLineUpDown(int physicalOffset) {
			ScrollVerticallyByPhysicalOffsetCommand command = new ScrollVerticallyByPhysicalOffsetCommand(View.Control);
			command.UpdateScrollBarBeforeExecution = false;
			command.PhysicalOffset = physicalOffset;
			command.Execute();
			return ScrollBarAdapter.GetRawScrollBarValue();
		}
		public virtual void ScrollByTopInvisibleHeightDelta(long topInvisibleHeightDelta) {
			ScrollToAbsolutePosition(ScrollBarAdapter.Value + topInvisibleHeightDelta);
		}
		public virtual void ScrollToAbsolutePosition(long value) {
			ScrollBarAdapter.BeginUpdate();
			try {
				ScrollBarAdapter.Maximum = View.PageViewInfoGenerator.TotalHeight - 1;
				ScrollBarAdapter.Value = value;
			}
			finally {
				ScrollBarAdapter.EndUpdate();
			}
			SynchronizeScrollbar();
			ScrollBarAdapter.EnsureSynchronized();
		}
		protected override void UpdateScrollBarAdapter() {
			PageViewInfoGenerator generator = View.PageViewInfoGenerator;
			if (generator.VisibleHeight < 0)
				return;
			ScrollBarAdapter.BeginUpdate();
			try {
				ScrollBarAdapter.Minimum = 0;
				ScrollBarAdapter.Maximum = generator.TotalHeight - 1;
				ScrollBarAdapter.LargeChange = generator.VisibleHeight;
				ScrollBarAdapter.Value = generator.TopInvisibleHeight;
				ScrollBarAdapter.Enabled = IsScrollPossible();
			}
			finally {
				ScrollBarAdapter.EndUpdate();
			}
		}
	}
#endregion
#region RichEditViewHorizontalScrollController (abstract class)
	public abstract class RichEditViewHorizontalScrollController : RichEditViewScrollControllerBase {
		protected RichEditViewHorizontalScrollController(RichEditView view)
			: base(view) {
		}
		public override IOfficeScrollbar ScrollBar {
			get {
				if (View.Control == null)
					return null;
				if (View.Control.InnerControl == null)
					return null;
				return View.Control.InnerControl.HorizontalScrollBar;
			}
		}
		protected override ScrollBarAdapter CreateScrollBarAdapter() {
			return new HorizontalScrollBarAdapter(ScrollBar, View.Control.CreatePlatformSpecificScrollBarAdapter());
		}
		public long GetLeftInvisibleWidth() {
			return ScrollBarAdapter.Value;
		}
		public int GetPhysicalLeftInvisibleWidth() {
			if (ScrollBar == null) 
				return 0;
			return (int)Math.Round(View.PageViewInfoGenerator.LeftInvisibleWidth * View.ZoomFactor);
		}
		protected override void OnScroll(object sender, ScrollEventArgs e) {
			OnScrollCore(e);
			View.OnHorizontalScroll();
		}
		protected internal abstract void OnScrollCore(ScrollEventArgs e);
		protected override void UpdateScrollBarAdapter() {
			PageViewInfoGenerator generator = View.PageViewInfoGenerator;
			if (generator.VisibleWidth < 0)
				return;
			ScrollBarAdapter.BeginUpdate();
			try {
				ScrollBarAdapter.Minimum = 0;
				ScrollBarAdapter.Maximum = generator.TotalWidth - 1;
				ScrollBarAdapter.LargeChange = generator.VisibleWidth;
				ScrollBarAdapter.Value = Math.Min(generator.LeftInvisibleWidth, ScrollBarAdapter.Maximum);
				ScrollBarAdapter.Enabled = IsScrollEnabled();
			}
			finally {
				ScrollBarAdapter.EndUpdate();
			}
		}
		bool IsScrollEnabled() {
			return ScrollBarAdapter.Maximum - ScrollBarAdapter.Minimum > ScrollBarAdapter.LargeChange &&
				ScrollBarAdapter.Value >= ScrollBarAdapter.Minimum;
		}
		public virtual void ScrollByLeftInvisibleWidthDelta(long leftInvisibleWidthDelta) {
			ScrollToAbsolutePosition(ScrollBarAdapter.Value + leftInvisibleWidthDelta);
		}
		public virtual void ScrollToAbsolutePosition(long value) {
			ScrollBarAdapter.BeginUpdate();
			try {
				ScrollBarAdapter.Maximum = View.PageViewInfoGenerator.TotalWidth - 1;
				ScrollBarAdapter.Value = value;
			}
			finally {
				ScrollBarAdapter.EndUpdate();
			}
			SynchronizeScrollbar();
			ScrollBarAdapter.EnsureSynchronized();
		}
	}
#endregion
#region ScrollByPhysicalHeightCalculator (abstract class)
	public abstract class ScrollByPhysicalHeightCalculator {
		RichEditView view;
		protected ScrollByPhysicalHeightCalculator(RichEditView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		public RichEditView View { get { return view; } }
		protected internal virtual long CalculateRowsTotalVisibleHeight(PageViewInfoGeneratorBase generator, int lastRowIndex, int bottomY) {
			PageGeneratorLayoutManager layoutManager = View.PageViewInfoGenerator;
			PageViewInfoRowCollection rows = generator.PageRows;
			long result = 0;
			if (lastRowIndex > 0) {
				result = layoutManager.CalculatePagesTotalLogicalHeightBelow(rows[0], 0);
				for (int i = 1; i < lastRowIndex; i++)
					result += layoutManager.CalculatePagesTotalLogicalHeight(rows[i]);
			}
			Rectangle viewPortBounds = new Rectangle(0, 0, 0, bottomY);
			result += layoutManager.CalculatePagesTotalVisibleLogicalHeight(rows[lastRowIndex], viewPortBounds);
			return result;
		}
		public virtual long CalculateScrollDelta(int physicalVerticalOffset) {
			Debug.Assert(physicalVerticalOffset >= 0);
			if (physicalVerticalOffset <= 0)
				return 0;
			long? result = CalculateScrollDeltaForVisibleRows(physicalVerticalOffset);
			if (!result.HasValue) {
				int invisiblePhysicalVerticalOffset = CalculateInvisiblePhysicalVerticalOffset(physicalVerticalOffset);
				result = CalculateScrollDeltaForInvisibleRows(invisiblePhysicalVerticalOffset);
			}
			return result.Value;
		}
		protected internal virtual long CalculateScrollDeltaForInvisibleRows(int physicalVerticalOffset) {
			int firstInvisiblePageIndex = CalculateFirstInvisiblePageIndex();
			if (firstInvisiblePageIndex < 0)
				return 0;
			InvisiblePageRowsGenerator generator = new InvisiblePageRowsGenerator(View.FormattingController.PageController.Pages, View.PageViewInfoGenerator);
			generator.FirstPageIndex = firstInvisiblePageIndex;
			generator.FirstInvalidPageIndex = CalculateFirstInvalidPageIndex();
			int index = LookupIntersectingRowIndexInInvisibleRows(generator, physicalVerticalOffset);
			if (index < 0)
				return GetDefaultScrollDeltaForInvisibleRows();
			long result = CalculateRowsTotalVisibleHeight(generator.Generator, index, physicalVerticalOffset);
			result += CalculateVisibleRowsScrollDelta();
			return result;
		}
		protected internal virtual int LookupIntersectingRowIndexInInvisibleRows(InvisiblePageRowsGenerator generator, int y) {
			int rowIndex = 0;
			for (; ; ) {
				PageViewInfoRow row = generator.GenerateNextRow();
				if (row == null)
					return -1;
				if (row.IntersectsWithHorizontalLine(y))
					return rowIndex;
				rowIndex++;
			}
		}
		protected internal abstract int CalculateFirstInvisiblePageIndex();
		protected internal abstract int CalculateFirstInvalidPageIndex();
		protected internal abstract int CalculateInvisiblePhysicalVerticalOffset(int physicalVerticalOffset);
		protected internal abstract long CalculateVisibleRowsScrollDelta();
		protected internal abstract long? CalculateScrollDeltaForVisibleRows(int physicalVerticalOffset);
		protected internal abstract long GetDefaultScrollDeltaForInvisibleRows();
	}
#endregion
#region ScrollUpByPhysicalHeightCalculator
	public class ScrollUpByPhysicalHeightCalculator : ScrollByPhysicalHeightCalculator {
		public ScrollUpByPhysicalHeightCalculator(RichEditView view)
			: base(view) {
		}
		protected internal override long? CalculateScrollDeltaForVisibleRows(int physicalVerticalOffset) {
			PageViewInfoRow firstRow = View.PageViewInfoGenerator.ActiveGenerator.PageRows.First;
			if (firstRow.IntersectsWithHorizontalLine(-physicalVerticalOffset) || firstRow.Bounds.Top == -physicalVerticalOffset) {
				Rectangle viewPortBounds = new Rectangle(0, -physicalVerticalOffset, 0, physicalVerticalOffset);
				return View.PageViewInfoGenerator.CalculatePagesTotalVisibleLogicalHeight(firstRow, viewPortBounds);
			}
			else {
				int pageIndex = View.FormattingController.PageController.Pages.IndexOf(firstRow.First.Page);
				if (pageIndex == 0)
					return View.PageViewInfoGenerator.CalculatePagesTotalLogicalHeightAbove(firstRow, 0);
				else
					return null;
			}
		}
		protected internal override long GetDefaultScrollDeltaForInvisibleRows() {
			return View.PageViewInfoGenerator.TopInvisibleHeight;
		}
		protected internal override long CalculateVisibleRowsScrollDelta() {
			PageGeneratorLayoutManager layoutManager = View.PageViewInfoGenerator;
			PageViewInfoRow firstRow = View.PageViewInfoGenerator.ActiveGenerator.PageRows.First;
			Rectangle viewPortBounds = layoutManager.ViewPortBounds;
			viewPortBounds.Y = firstRow.Bounds.Top;
			viewPortBounds.Height = -viewPortBounds.Y;
			return layoutManager.CalculatePagesTotalVisibleLogicalHeight(firstRow, viewPortBounds);
		}
		protected internal override int CalculateInvisiblePhysicalVerticalOffset(int physicalVerticalOffset) {
			PageViewInfoRow firstRow = View.PageViewInfoGenerator.ActiveGenerator.PageRows.First;
			return physicalVerticalOffset + firstRow.Bounds.Top;
		}
		protected internal override int CalculateFirstInvisiblePageIndex() {
			return View.CalculateFirstInvisiblePageIndexBackward();
		}
		protected internal override int CalculateFirstInvalidPageIndex() {
			return -1;
		}
	}
#endregion
#region ScrollDownByPhysicalHeightCalculator
	public class ScrollDownByPhysicalHeightCalculator : ScrollByPhysicalHeightCalculator {
		public ScrollDownByPhysicalHeightCalculator(RichEditView view)
			: base(view) {
		}
		protected internal override long? CalculateScrollDeltaForVisibleRows(int physicalVerticalOffset) {
			int intersectingRowIndex = LookupIntersectingRowIndex(physicalVerticalOffset);
			if (intersectingRowIndex >= 0)
				return CalculateRowsTotalVisibleHeight(View.PageViewInfoGenerator.ActiveGenerator, intersectingRowIndex, physicalVerticalOffset);
			else
				return null;
		}
		protected internal override long GetDefaultScrollDeltaForInvisibleRows() {
			try {
				PageViewInfoRowCollection rows = View.PageViewInfoGenerator.ActiveGenerator.PageRows;
				PageViewInfoRow lastRow = rows.Last;
				if (lastRow == null)
					return 0;
				return CalculateRowsTotalVisibleHeight(View.PageViewInfoGenerator.ActiveGenerator, rows.Count - 1, lastRow.Bounds.Bottom);
			}
			catch {
				return 0;
			}
		}
		protected internal override long CalculateVisibleRowsScrollDelta() {
			PageGeneratorLayoutManager layoutManager = View.PageViewInfoGenerator;
			PageViewInfoRowCollection rows = View.PageViewInfoGenerator.ActiveGenerator.PageRows;
			long result = layoutManager.CalculatePagesTotalLogicalHeightBelow(rows[0], layoutManager.ViewPortBounds.Top);
			int count = rows.Count;
			for (int i = 1; i < count; i++)
				result += layoutManager.CalculatePagesTotalLogicalHeight(rows[i]);
			return result;
		}
		protected internal override int CalculateInvisiblePhysicalVerticalOffset(int physicalVerticalOffset) {
			PageViewInfoRowCollection visibleRows = View.PageViewInfoGenerator.ActiveGenerator.PageRows;
			int totalRowsHeight = visibleRows[0].Bounds.Bottom;
			int count = visibleRows.Count;
			for (int i = 1; i < count; i++)
				totalRowsHeight += visibleRows[i].Bounds.Height;
			return physicalVerticalOffset - totalRowsHeight;
		}
		protected internal override int CalculateFirstInvisiblePageIndex() {
			return View.CalculateFirstInvisiblePageIndexForward();
		}
		protected internal override int CalculateFirstInvalidPageIndex() {
			return View.FormattingController.PageController.Pages.Count;
		}
		protected internal virtual int LookupIntersectingRowIndex(int y) {
			PageViewInfoRowCollection rows = View.PageViewInfoGenerator.ActiveGenerator.PageRows;
			int count = rows.Count;
			for (int i = 0; i < count; i++)
				if (rows[i].IntersectsWithHorizontalLine(y))
					return i;
			return -1;
		}
	}
#endregion
}
