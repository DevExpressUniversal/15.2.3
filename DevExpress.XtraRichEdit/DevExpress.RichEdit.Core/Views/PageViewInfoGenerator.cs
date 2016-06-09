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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Internal.PrintLayout {
	#region PageViewInfoGeneratorState
	public enum PageViewInfoGeneratorState {
		InvisibleEmptyRow,
		InvisibleRow,
		PartialVisibleEmptyRow,
		PartialVisibleRow,
		VisibleEmptyRow,
		VisibleRow,
		VisiblePagesGenerationComplete
	}
	#endregion
	#region StateProcessPageResult
	public enum StateProcessPageResult {
		Success,
		TryAgain
	}
	#endregion
	#region ProcessPageResult
	public enum ProcessPageResult {
		VisiblePagesGenerationComplete,
		VisiblePagesGenerationIncomplete,
	}
	#endregion
	#region PageVisibility
	public enum PageVisibility {
		Entire,
		Partial,
		Invisible
	}
	#endregion
	#region PageViewInfoRow
	public class PageViewInfoRow : PageViewInfoCollection {
		Rectangle bounds;
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public bool IntersectsWithHorizontalLine(int lineY) {
			return bounds.Top < lineY && bounds.Bottom >= lineY;
		}
		public PageViewInfo GetPageAtPoint(Point point, bool strictSearch) {
			if (Count <= 0)
				return null;
			PageViewInfoAndPointComparable predicate = new PageViewInfoAndPointComparable(point);
			int index = Algorithms.BinarySearch(this, predicate);
			if (index >= 0)
				return this[index];
			if (strictSearch)
				return null;
			if (index == ~Count) {
				if (predicate.CompareTo(First) > 0)
					return First;
				else
					return Last;
			}
			else
				return this[~index];
		}
#if DEBUG
		public override string ToString() {
			return String.Format("PageViewInfoRow. Bounds: {0}, PageCount: {1}", Bounds, Count);
		}
#endif
	}
	#endregion
	#region PageViewInfoAndPointComparable
	public class PageViewInfoAndPointComparable : IComparable<PageViewInfo> {
		int x;
		public PageViewInfoAndPointComparable(Point point) {
			this.x = point.X;
		}
		public int X { get { return x; } }
		#region IComparable<PageViewInfo> Members
		public int CompareTo(PageViewInfo pageViewInfo) {
			Rectangle bounds = pageViewInfo.Bounds;
			if (x < bounds.X)
				return 1;
			else if (x >= bounds.Right)
				return -1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region PageViewInfoRowCollection
	public class PageViewInfoRowCollection : List<PageViewInfoRow> {
		#region Properties
		#region First
		public PageViewInfoRow First {
			get {
				if (Count <= 0)
					return null;
				else
					return this[0];
			}
		}
		#endregion
		#region Last
		public PageViewInfoRow Last {
			get {
				if (Count <= 0)
					return null;
				else
					return this[Count - 1];
			}
		}
		#endregion
		#endregion
		public PageViewInfoRow GetRowAtPoint(Point point, bool strictSearch) {
			if (Count <= 0)
				return null;
			PageViewInfoRowAndPointComparable predicate = new PageViewInfoRowAndPointComparable(point);
			int index = Algorithms.BinarySearch(this, predicate);
			if (index >= 0)
				return this[index];
			if (strictSearch)
				return null;
			if (index == ~Count) {
				if (predicate.CompareTo(First) > 0)
					return First;
				else
					return Last;
			}
			else
				return this[~index];
		}
	}
	#endregion
	#region PageGenerationStrategyType
	public enum PageGenerationStrategyType {
		RunningHeight,
		FirstPageOffset
	}
	#endregion
	#region PageViewInfoGenerator (abstract class)
	public abstract class PageViewInfoGenerator : PageGeneratorLayoutManager {
		#region Fields
		long totalWidth;
		long visibleWidth;
		long leftInvisibleWidth;
		long maxPageWidth;
		readonly RunningHeightFirstPageAnchor runningHeightAnchor;
		readonly FirstPageOffsetFirstPageAnchor firstPageOffsetAnchor;
		PageViewInfoGeneratorRunningHeight runningHeightGenerator;
		PageViewInfoGeneratorFirstPageOffset firstPageOffsetGenerator;
		PageViewInfoGeneratorBase activeGenerator;
		#endregion
		protected PageViewInfoGenerator(RichEditView view): base (view){
			this.runningHeightAnchor = new RunningHeightFirstPageAnchor();
			this.firstPageOffsetAnchor = new FirstPageOffsetFirstPageAnchor();
			Reset(PageGenerationStrategyType.RunningHeight);
		}
		#region Properties
		public override bool ShowComments { get { return GetShowComments(); } }
		public long TotalWidth { get { return totalWidth; } protected internal set { totalWidth = value; } }
		public long VisibleWidth {
			get { return visibleWidth; }
			protected internal set { visibleWidth = value; } 
		}
		public long MaxPageWidth { get { return maxPageWidth; } protected set { maxPageWidth = value; } }
		public long LeftInvisibleWidth { get { return leftInvisibleWidth; } set { leftInvisibleWidth = value; } }
		public long VisibleHeight { get { return activeGenerator.VisibleHeight; } }
		public long TotalHeight { get { return activeGenerator.TotalHeight; } }
		public long TopInvisibleHeight { get { return runningHeightAnchor.TopInvisibleHeight; } set { runningHeightAnchor.TopInvisibleHeight = value; } }
		public PageViewInfoGeneratorBase ActiveGenerator { get { return activeGenerator; } }
		internal RunningHeightFirstPageAnchor RunningHeightAnchor { get { return runningHeightAnchor; } }
		internal FirstPageOffsetFirstPageAnchor FirstPageOffsetAnchor { get { return firstPageOffsetAnchor; } }
		#endregion
		public virtual ProcessPageResult PreProcessPage(Page page, int pageIndex) {
			return activeGenerator.PreProcessPage(page, pageIndex);
		}
		public virtual ProcessPageResult ProcessPage(Page page, int pageIndex) {
			ProcessPageResult result = activeGenerator.ProcessPage(page, pageIndex);
			if (result == ProcessPageResult.VisiblePagesGenerationIncomplete) {
				if (activeGenerator == firstPageOffsetGenerator)
					UpdateRunningHeightAnchor();
			}
			return result;
		}
		public void ResetAnchors() {
			runningHeightAnchor.TopInvisibleHeight = 0;
			firstPageOffsetAnchor.PageIndex = 0;
			firstPageOffsetAnchor.VerticalOffset = 0;
		}
		public virtual void Reset(PageGenerationStrategyType strategy) {
			this.ViewPortBounds = View.Bounds;
			this.ZoomFactor = View.ZoomFactor;
			if (strategy == PageGenerationStrategyType.RunningHeight) {
				PageViewInfoGeneratorRunningHeight newGenerator = new PageViewInfoGeneratorRunningHeight(this, runningHeightAnchor, View.PageViewInfos);
				if (activeGenerator != runningHeightGenerator)
					UpdateRunningHeightAnchor();
				this.runningHeightGenerator = newGenerator;
				this.activeGenerator = runningHeightGenerator;
			}
			else {
				PageViewInfoGeneratorFirstPageOffset newGenerator = new PageViewInfoGeneratorFirstPageOffset(this, firstPageOffsetAnchor, View.PageViewInfos);
				if (activeGenerator != firstPageOffsetGenerator)
					UpdateFirstPageOffsetAnchor();
				this.firstPageOffsetGenerator = newGenerator;
				this.activeGenerator = firstPageOffsetGenerator;
			}
		}
		protected internal virtual void CalculateWidthParameters() {
			long totalWidthValue = long.MinValue;
			long visibleWidthValue = long.MinValue;
			int count = ActiveGenerator.PageRows.Count;
			for (int i = 0; i < count; i++) {
				PageViewInfoRow row = ActiveGenerator.PageRows[i];
				if (row.Count == 1) {
					PageViewInfo page = row[0];
					totalWidthValue = Math.Max(totalWidthValue, CalculatePageLogicalWidth(page.Page));
					visibleWidthValue = Math.Max(visibleWidthValue, CalculatePageVisibleLogicalWidth(page));
				}
			}
			if (totalWidthValue != long.MinValue && visibleWidthValue != long.MinValue) {
				this.TotalWidth = totalWidthValue;
				this.VisibleWidth = visibleWidthValue;
			}
			else {
				this.TotalWidth = 100;
				this.VisibleWidth = 100;
			}
		}
		protected internal virtual void CalculateMaxPageWidth() {
			SectionCollection sections = View.DocumentModel.Sections;
			SectionIndex count = new SectionIndex(sections.Count);
			maxPageWidth = long.MinValue;
			for (SectionIndex i = new SectionIndex(0); i < count; i++) {				
				Page page = new Page();
				page.Bounds = View.FormattingController.PageController.CalculatePageBounds(sections[i]);
				maxPageWidth = Math.Max(maxPageWidth, CalculatePageLogicalWidth(page));
			}
		}
		protected internal virtual void UpdateRunningHeightAnchor() {
			PageViewInfoGeneratorBase generator = firstPageOffsetGenerator;
			PageViewInfoRow firstRow = generator.PageRows.First;
			if (firstRow == null)
				return;
			PageViewInfo firstPage = firstRow.First;
			if (firstPage == null)
				return;
			long result = CalculatePagesTotalLogicalHeightAbove(firstRow, 0);
			PageCollection pages = View.FormattingController.PageController.Pages;
			int lastIndex = pages.IndexOf(firstPage.Page);
			for (int i = 0; i < lastIndex; i++)
				result += CalculatePageLogicalHeight(pages[i]);
			this.runningHeightAnchor.TopInvisibleHeight = result;
		}
		protected internal virtual void UpdateFirstPageOffsetAnchor() {
			PageViewInfoGeneratorBase generator = runningHeightGenerator;
			PageViewInfoRow firstRow = generator.PageRows.First;
			if (firstRow == null)
				return;
			PageViewInfo pageViewInfo = LookupFirstVisiblePage(firstRow);
			if (pageViewInfo == null)
				return;
			Rectangle viewPort = this.ViewPortBounds;
			viewPort.Height = int.MaxValue / 2;
			this.firstPageOffsetAnchor.PageIndex = View.FormattingController.PageController.Pages.IndexOf(pageViewInfo.Page);
			this.firstPageOffsetAnchor.VerticalOffset = CalculatePageLogicalHeight(pageViewInfo.Page) - CalculatePageVisibleLogicalHeight(pageViewInfo, viewPort);
		}
		protected internal virtual PageViewInfo LookupFirstVisiblePage(PageViewInfoRow row) {
			int count = row.Count;
			for (int i = 0; i < count; i++)
				if (CalculatePageVisibleLogicalHeight(row[i]) > 0)
					return row[i];
			return null;
		}
		public virtual PageViewInfoRow GetPageRowAtPoint(Point point) {
			return GetPageRowAtPoint(point, true);
		}
		public virtual PageViewInfoRow GetPageRowAtPoint(Point point, bool strictSearch) {
			return ActiveGenerator.GetPageRowAtPoint(point, strictSearch);
		}
		protected internal override void OnLayoutUnitChanging(DocumentLayoutUnitConverter unitConverter) {
			base.OnLayoutUnitChanging(unitConverter);
			this.HorizontalPageGap = unitConverter.LayoutUnitsToTwips(HorizontalPageGap);
			this.VerticalPageGap = unitConverter.LayoutUnitsToTwips(VerticalPageGap);
			this.leftInvisibleWidth = unitConverter.LayoutUnitsToTwips(leftInvisibleWidth);
			this.runningHeightAnchor.TopInvisibleHeight = unitConverter.LayoutUnitsToTwips(runningHeightAnchor.TopInvisibleHeight);
			this.firstPageOffsetAnchor.VerticalOffset = unitConverter.LayoutUnitsToTwips(firstPageOffsetAnchor.VerticalOffset);
		}
		protected internal override void OnLayoutUnitChanged(DocumentLayoutUnitConverter unitConverter) {
			base.OnLayoutUnitChanged(unitConverter);
			this.HorizontalPageGap = unitConverter.TwipsToLayoutUnits(HorizontalPageGap);
			this.VerticalPageGap = unitConverter.TwipsToLayoutUnits(VerticalPageGap);
			this.leftInvisibleWidth = unitConverter.TwipsToLayoutUnits(leftInvisibleWidth);
			this.runningHeightAnchor.TopInvisibleHeight = unitConverter.TwipsToLayoutUnits(runningHeightAnchor.TopInvisibleHeight);
			this.firstPageOffsetAnchor.VerticalOffset = unitConverter.TwipsToLayoutUnits(firstPageOffsetAnchor.VerticalOffset);
		}
	}
	#endregion
	#region PageGeneratorLayoutManager (abstract class)
	public abstract class PageGeneratorLayoutManager : ICloneable {
		#region Fields
		float zoomFactor;
		int horizontalPageGap;
		int verticalPageGap;
		readonly RichEditView view;
		Rectangle viewPortBounds;
		#endregion
		protected PageGeneratorLayoutManager(RichEditView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			DocumentLayoutUnitConverter unitConverter = view.DocumentModel.LayoutUnitConverter;
			this.HorizontalPageGap = unitConverter.DocumentsToLayoutUnits(60); 
			this.VerticalPageGap = unitConverter.DocumentsToLayoutUnits(60); 
		}
		#region Properties
		public float ZoomFactor {
			get { return zoomFactor; }
			set {
				zoomFactor = value;
			}
		}
		public Rectangle ViewPortBounds {
			get { return viewPortBounds; }
			set {
				viewPortBounds = value;
			}
		}
		internal RichEditView View { get { return view; } }
		public virtual int VerticalPageGap { get { return verticalPageGap; } set { verticalPageGap = value; } }
		public virtual int HorizontalPageGap { get { return horizontalPageGap; } set { horizontalPageGap = value; } }
		public virtual bool ShowComments { get { return GetShowComments(); } }
		#endregion
		#region ICloneable Members
		object ICloneable.Clone() {
			return CloneCore();
		}
		public PageGeneratorLayoutManager Clone() {
			return CloneCore();
		}
		protected internal virtual PageGeneratorLayoutManager CloneCore() {
			PageGeneratorLayoutManager clone = CreateEmptyClone();
			clone.CopyFrom(this);
			return clone;
		}
		protected internal abstract PageGeneratorLayoutManager CreateEmptyClone();
		protected internal virtual void CopyFrom(PageGeneratorLayoutManager manager) {
			this.ZoomFactor = manager.ZoomFactor;
			this.ViewPortBounds = manager.ViewPortBounds;
			this.HorizontalPageGap = manager.HorizontalPageGap;
			this.VerticalPageGap = manager.VerticalPageGap;
		}
		#endregion
		protected bool GetShowComments() {
			if (View.DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible)
				return true;
			return false;
		}
		public virtual int CalculatePageLogicalWidth(Page page) {
			return page.Bounds.Width + HorizontalPageGap + page.CommentBounds.Width;
		}
		public virtual int CalculatePageLogicalTotalWidth(Page page) {
			return CalculatePageLogicalWidth(page);
		}
		public virtual int CalculatePagePhysicalWidth(Page page) {
			return (int)Math.Ceiling((CalculatePageLogicalWidth(page) * ZoomFactor));
		}
		public virtual int CalculatePageLogicalHeight(Page page) {
			return page.Bounds.Height + VerticalPageGap;
		}
		public virtual int CalculatePagePhysicalHeight(Page page) {
			return (int)Math.Ceiling((CalculatePageLogicalHeight(page) * ZoomFactor));
		}
		public virtual Size CalculatePageLogicalSize(Page page) {
			Rectangle pageBounds = page.Bounds;
			return new Size(pageBounds.Width + HorizontalPageGap, pageBounds.Height + VerticalPageGap);
		}
		public virtual Size CalculatePagePhysicalSize(Page page) {
			Rectangle pageBounds = page.Bounds;
			return new Size((int)Math.Ceiling((pageBounds.Width + HorizontalPageGap) * ZoomFactor), (int)Math.Ceiling((pageBounds.Height + VerticalPageGap) * ZoomFactor));
		}
		public virtual Size CalculatePageCommentPhysicalSize(Page page, int commentWidth) {
			Rectangle pageBounds = page.Bounds;
			return new Size((int)Math.Ceiling((pageBounds.Width + HorizontalPageGap + commentWidth) * ZoomFactor), (int)Math.Ceiling((pageBounds.Height + VerticalPageGap) * ZoomFactor));
		}
		public virtual bool CanFitPageToPageRow(Page page, PageViewInfoRow row) {
			Size pageSize = CalculatePagePhysicalSize(page);
			return row.Bounds.Width + pageSize.Width <= ViewPortBounds.Width;
		}
		protected internal virtual float CalculateVisibleHeightFactor(PageViewInfo pageViewInfo, Rectangle viewPort) {
			Rectangle extendedViewPortBounds = new Rectangle(0, viewPort.Top, int.MaxValue, viewPort.Height);
			Rectangle pageViewInfoVisibleBounds = Rectangle.Intersect(extendedViewPortBounds, pageViewInfo.Bounds);
			int pageViewInfoTotalHeight = pageViewInfo.Bounds.Height;
			if (pageViewInfoTotalHeight <= 0)
				return 0;
			return pageViewInfoVisibleBounds.Height / (float)pageViewInfoTotalHeight;
		}
		protected internal virtual float CalculateVisibleWidthFactor(PageViewInfo pageViewInfo, Rectangle viewPort) {
			Rectangle extendedViewPortBounds = new Rectangle(0, viewPort.Top, viewPort.Width, int.MaxValue);
			Rectangle pageViewInfoVisibleBounds = Rectangle.Intersect(extendedViewPortBounds, pageViewInfo.Bounds);
			int pageViewInfoTotalWidth = pageViewInfo.Bounds.Width;
			if (pageViewInfoTotalWidth <= 0)
				return 0;
			return pageViewInfoVisibleBounds.Width / (float)pageViewInfoTotalWidth;
		}
		public virtual int CalculatePageVisibleLogicalWidth(PageViewInfo pageViewInfo) {
			return CalculatePageVisibleLogicalWidth(pageViewInfo, ViewPortBounds);
		}
		public virtual int CalculatePageVisibleLogicalWidth(PageViewInfo pageViewInfo, Rectangle viewPort) {
			float visibleWidthFactor = CalculateVisibleWidthFactor(pageViewInfo, viewPort);
			if (visibleWidthFactor <= 0)
				return 0;
			float resultF = (float)((float)visibleWidthFactor * (float)CalculatePageLogicalWidth(pageViewInfo.Page));
			return (int)(float)Math.Round(resultF);
		}
		public virtual int CalculatePageVisibleLogicalHeight(PageViewInfo pageViewInfo) {
			return CalculatePageVisibleLogicalHeight(pageViewInfo, ViewPortBounds);
		}
		public virtual int CalculatePageVisibleLogicalHeight(PageViewInfo pageViewInfo, Rectangle viewPort) {
			float visibleHeightFactor = CalculateVisibleHeightFactor(pageViewInfo, viewPort);
			if (visibleHeightFactor <= 0)
				return 0;
			float resultF = (float)((float)visibleHeightFactor * (float)CalculatePageLogicalHeight(pageViewInfo.Page));
			return (int)(float)Math.Round(resultF);
		}
		#region Precision lose note!
		#endregion
		public virtual long CalculatePagesTotalLogicalHeight(PageViewInfoRow row) {
			long result = 0;
			int count = row.Count;
			for (int i = 0; i < count; i++)
				result += CalculatePageLogicalHeight(row[i].Page);
			return result;
		}
		public virtual long CalculatePagesTotalVisibleLogicalHeight(PageViewInfoRow row) {
			return CalculatePagesTotalVisibleLogicalHeight(row, ViewPortBounds);
		}
		public virtual long CalculatePagesTotalVisibleLogicalHeight(PageViewInfoRow row, Rectangle viewPort) {
			long result = 0;
			int count = row.Count;
			for (int i = 0; i < count; i++)
				result += CalculatePageVisibleLogicalHeight(row[i], viewPort);
			return result;
		}
		public virtual long CalculatePagesTotalLogicalHeightAbove(PageViewInfoRow row, int logicalY) {
			Rectangle viewPort = new Rectangle(0, int.MinValue / 2, 0, logicalY - int.MinValue / 2);
			return CalculatePagesTotalVisibleLogicalHeight(row, viewPort);
		}
		public virtual long CalculatePagesTotalLogicalHeightBelow(PageViewInfoRow row, int logicalY) {
			Rectangle viewPort = new Rectangle(0, logicalY, 0, int.MaxValue / 2);
			return CalculatePagesTotalVisibleLogicalHeight(row, viewPort);
		}
		protected internal virtual void UpdatePageClientBounds(PageViewInfo pageViewInfo) {
			Rectangle clientBounds = pageViewInfo.Bounds;
			int horizontalGapPhysicalSize = (int)Math.Floor(HorizontalPageGap * ZoomFactor);
			int verticalGapPhysicalSize = (int)Math.Floor(VerticalPageGap * ZoomFactor);
			clientBounds.Width -= horizontalGapPhysicalSize;
			clientBounds.Height -= verticalGapPhysicalSize;
			clientBounds.X += horizontalGapPhysicalSize / 2;
			clientBounds.Y += verticalGapPhysicalSize / 2;
			pageViewInfo.ClientBounds = clientBounds;
			Rectangle commentsClientBounds = pageViewInfo.CommentsBounds;
			if (commentsClientBounds.Width <= 0)
				pageViewInfo.ClientCommentsBounds = Rectangle.Empty;
			else {
				commentsClientBounds.Width -= horizontalGapPhysicalSize / 2;
				commentsClientBounds.Height = clientBounds.Height;
				commentsClientBounds.Y = clientBounds.Y;
				commentsClientBounds.X = clientBounds.X + clientBounds.Width - commentsClientBounds.Width;
				pageViewInfo.ClientCommentsBounds = commentsClientBounds;
			}
		}
		protected internal  Rectangle OffsetRectangle(Rectangle rect, int dx, int dy) {
			rect.Offset(dx, dy);
			return rect;
		}
		protected internal  void OffsetRowVertically(PageViewInfoRow row, int offset) {
			int count = row.Count;
			for (int i = 0; i < count; i++) {
				PageViewInfo pageViewInfo = row[i];
				pageViewInfo.Bounds = OffsetRectangle(pageViewInfo.Bounds, 0, offset);
				UpdatePageClientBounds(pageViewInfo);
			}
			row.Bounds = OffsetRectangle(row.Bounds, 0, offset);
		}
		protected internal virtual Rectangle CreateInitialPageViewInfoRowBounds(int y) {
			return new Rectangle(ViewPortBounds.Width / 2, y, 0, 0);
		}
		protected internal virtual int CalculateFirstPageLeftOffset(int totalPagesWidth) {
			return (ViewPortBounds.Width - totalPagesWidth) / 2;
		}
		protected internal virtual void AlignPages(PageViewInfoRow row) {
			int totalPagesWidth = row.Last.Bounds.Right - row.First.Bounds.Left;
			int offset = Math.Max(0, CalculateFirstPageLeftOffset(totalPagesWidth));
			AlignPagesHorizontally(row, offset);
			AlignPagesVertically(row);
			UpdatePagesClientBounds(row);
			row.Bounds = new Rectangle(offset, row.Bounds.Y, totalPagesWidth, row.Bounds.Height);
		}
		protected internal virtual void AlignPagesVertically(PageViewInfoRow row) {
			int rowHeight = row.Bounds.Height;
			int count = row.Count;
			for (int i = 0; i < count; i++) {
				PageViewInfo pageViewInfo = row[i];
				Rectangle r = pageViewInfo.Bounds;
				pageViewInfo.Bounds = new Rectangle(r.Left, row.Bounds.Top + (rowHeight - r.Height) / 2, r.Width, r.Height);
			}
		}
		protected internal virtual void AlignPagesHorizontally(PageViewInfoRow row, int rowLeftOffset) {
			int x = rowLeftOffset;
			int count = row.Count;
			for (int i = 0; i < count; i++) {
				PageViewInfo pageViewInfo = row[i];
				Rectangle r = pageViewInfo.Bounds;
				pageViewInfo.Bounds = new Rectangle(x, r.Top, r.Width, r.Height);
				x += r.Width;
			}
		}
		protected internal virtual void UpdatePagesClientBounds(PageViewInfoRow row) {
			row.ForEach(UpdatePageClientBounds);
		}
		protected internal virtual void OnLayoutUnitChanging(DocumentLayoutUnitConverter unitConverter) {
		}
		protected internal virtual void OnLayoutUnitChanged(DocumentLayoutUnitConverter unitConverter) {
		}
	}
	#endregion
	#region PageViewInfoGeneratorBase (abstract class)
	public abstract class PageViewInfoGeneratorBase {
		#region Fields
		readonly PageViewInfoCollection pages;
		readonly PageViewInfoRowCollection pageRows;
		PageViewInfoRow currentPageRow;
		long totalHeight;
		long visibleHeight;
		PageViewInfoGeneratorStateBase state;
		readonly FirstPageAnchor firstPageAnchor;
		readonly PageGeneratorLayoutManager layoutManager;
		#endregion
		protected PageViewInfoGeneratorBase(PageGeneratorLayoutManager layoutManager, FirstPageAnchor firstPageAnchor, PageViewInfoCollection pages) {
			Guard.ArgumentNotNull(layoutManager, "layoutManager");
			Guard.ArgumentNotNull(firstPageAnchor, "firstPageAnchor");
			Guard.ArgumentNotNull(pages, "pages");
			this.layoutManager = layoutManager;
			this.firstPageAnchor = firstPageAnchor;
			this.pages = pages;
			this.pageRows = new PageViewInfoRowCollection();
			this.currentPageRow = new PageViewInfoRow();
			this.currentPageRow.Bounds = layoutManager.CreateInitialPageViewInfoRowBounds(0);
			this.pageRows.Add(currentPageRow);
			ChangeState(PageViewInfoGeneratorState.InvisibleEmptyRow);
		}
		#region Properties
		public PageViewInfoCollection Pages { get { return pages; } }
		public PageViewInfoRowCollection PageRows { get { return pageRows; } }
		public long TotalHeight { get { return totalHeight; } set { totalHeight = value; } }
		public long VisibleHeight { get { return visibleHeight; } set { visibleHeight = value; } }
		public PageViewInfoRow CurrentPageRow { get { return currentPageRow; } set { currentPageRow = value; } }
		public PageViewInfoGeneratorStateBase State { get { return state; } }
		public FirstPageAnchor FirstPageAnchor { get { return firstPageAnchor; } }
		public PageGeneratorLayoutManager LayoutManager { get { return layoutManager; } }
		#endregion
		#region Events
		#region RowFinished
		EventHandler onRowFinished;
		public event EventHandler RowFinished { add { onRowFinished += value; } remove { onRowFinished -= value; } }
		protected internal virtual void RaiseRowFinished() {
			if (onRowFinished != null)
				onRowFinished(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal virtual void OnLayoutUnitChanging() {
		}
		protected internal virtual void OnLayoutUnitChanged() {
		}
		public virtual ProcessPageResult PreProcessPage(Page page, int pageIndex) {
			return state.PreProcessPage(page, pageIndex);
		}
		public virtual ProcessPageResult ProcessPage(Page page, int pageIndex) {
			for (; ; ) {
				if (state.ProcessPage(page, pageIndex) == StateProcessPageResult.Success)
					break;
			}
			if (state.Type != PageViewInfoGeneratorState.VisiblePagesGenerationComplete)
				return ProcessPageResult.VisiblePagesGenerationIncomplete;
			else
				return ProcessPageResult.VisiblePagesGenerationComplete;
		}
		protected internal virtual void AddPageRow(PageViewInfoRow row) {
			pageRows.Add(row);
		}
		public virtual PageViewInfoGeneratorStateBase ChangeState(PageViewInfoGeneratorState stateType) {
			state = CreateState(stateType);
			return state;
		}
		protected internal abstract PageViewInfoGeneratorStateBase CreateInvisibleEmptyRowState();
		protected internal abstract PageViewInfoGeneratorStateBase CreateInvisibleRowState();
		protected internal abstract PageViewInfoGeneratorStateBase CreatePartialVisibleEmptyRowState();
		protected internal abstract PageViewInfoGeneratorStateBase CreatePartialVisibleRowState();
		protected internal virtual PageViewInfoGeneratorStateBase CreateState(PageViewInfoGeneratorState stateType) {
			switch (stateType) {
				case PageViewInfoGeneratorState.InvisibleEmptyRow:
					return CreateInvisibleEmptyRowState();
				case PageViewInfoGeneratorState.InvisibleRow:
					return CreateInvisibleRowState();
				case PageViewInfoGeneratorState.PartialVisibleEmptyRow:
					return CreatePartialVisibleEmptyRowState();
				case PageViewInfoGeneratorState.PartialVisibleRow:
					return CreatePartialVisibleRowState();
				case PageViewInfoGeneratorState.VisibleEmptyRow:
					return new StateVisibleEmptyRow(this);
				case PageViewInfoGeneratorState.VisibleRow:
					return new StateVisibleRow(this);
				case PageViewInfoGeneratorState.VisiblePagesGenerationComplete:
					return new StateVisiblePagesGenerationComplete(this);
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
		public virtual PageViewInfoRow GetPageRowAtPoint(Point point, bool strictSearch) {
			return pageRows.GetRowAtPoint(point, strictSearch);
		}
	}
	#endregion
	#region PageViewInfoRowAndPointComparable
	public class PageViewInfoRowAndPointComparable : IComparable<PageViewInfoRow> {
		int y;
		public PageViewInfoRowAndPointComparable(Point point) {
			this.y = point.Y;
		}
		public int Y { get { return y; } }
		#region IComparable<PageViewInfo> Members
		public int CompareTo(PageViewInfoRow row) {
			Rectangle bounds = row.Bounds;
			if (y < bounds.Y)
				return 1;
			else if (y >= bounds.Bottom)
				return -1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region PageViewInfoGeneratorRunningHeight
	public class PageViewInfoGeneratorRunningHeight : PageViewInfoGeneratorBase {
		long runningHeight;
		public PageViewInfoGeneratorRunningHeight(PageGeneratorLayoutManager layoutManager, RunningHeightFirstPageAnchor firstPageAnchor, PageViewInfoCollection pages)
			: base(layoutManager, firstPageAnchor, pages) {
		}
		public long RunningHeight { get { return runningHeight; } set { runningHeight = value; } }
		public new RunningHeightFirstPageAnchor FirstPageAnchor { get { return (RunningHeightFirstPageAnchor)base.FirstPageAnchor; } }
		protected internal override PageViewInfoGeneratorStateBase CreateInvisibleEmptyRowState() {
			return new StateInvisibleEmptyRowRunningHeight(this);
		}
		protected internal override PageViewInfoGeneratorStateBase CreateInvisibleRowState() {
			return new StateInvisibleRowRunningHeight(this);
		}
		protected internal override PageViewInfoGeneratorStateBase CreatePartialVisibleEmptyRowState() {
			return new StatePartialVisibleEmptyRowRunningHeight(this);
		}
		protected internal override PageViewInfoGeneratorStateBase CreatePartialVisibleRowState() {
			return new StatePartialVisibleRowRunningHeight(this);
		}
		protected internal override void OnLayoutUnitChanging() {
			base.OnLayoutUnitChanging();
		}
		protected internal override void OnLayoutUnitChanged() {
			base.OnLayoutUnitChanged();
		}
	}
	#endregion
	#region PageViewInfoGeneratorFirstPageOffset
	public class PageViewInfoGeneratorFirstPageOffset : PageViewInfoGeneratorBase {
		public PageViewInfoGeneratorFirstPageOffset(PageGeneratorLayoutManager layoutManager, FirstPageOffsetFirstPageAnchor firstPageAnchor, PageViewInfoCollection pages)
			: base(layoutManager, firstPageAnchor, pages) {
		}
		public new FirstPageOffsetFirstPageAnchor FirstPageAnchor { get { return (FirstPageOffsetFirstPageAnchor)base.FirstPageAnchor; } }
		protected internal override PageViewInfoGeneratorStateBase CreateInvisibleEmptyRowState() {
			return new StateInvisibleEmptyRowFirstPageOffset(this);
		}
		protected internal override PageViewInfoGeneratorStateBase CreateInvisibleRowState() {
			return new StateInvisibleRowFirstPageOffset(this);
		}
		protected internal override PageViewInfoGeneratorStateBase CreatePartialVisibleEmptyRowState() {
			return new StatePartialVisibleEmptyRowFirstPageOffset(this);
		}
		protected internal override PageViewInfoGeneratorStateBase CreatePartialVisibleRowState() {
			return new StatePartialVisibleRowFirstPageOffset(this);
		}
	}
	#endregion
	#region PageViewInfoGeneratorStateBase (abstract class)
	public abstract class PageViewInfoGeneratorStateBase {
		#region Fields
		readonly PageViewInfoGeneratorBase generator;
		readonly PageGeneratorLayoutManager layoutManager;
		#endregion
		protected PageViewInfoGeneratorStateBase(PageViewInfoGeneratorBase generator) {
			Guard.ArgumentNotNull(generator, "generator");
			this.generator = generator;
			this.layoutManager = generator.LayoutManager;
		}
		#region Properties
		public abstract PageViewInfoGeneratorState Type { get; }
		public PageViewInfoGeneratorBase Generator { get { return generator; } }
		public Rectangle ViewPortBounds { get { return layoutManager.ViewPortBounds; } }
		public float ZoomFactor { get { return layoutManager.ZoomFactor; } }
		public long TotalHeight { get { return Generator.TotalHeight; } set { Generator.TotalHeight = value; } }
		public FirstPageAnchor FirstPageAnchor { get { return Generator.FirstPageAnchor; } }
		public long VisibleHeight { get { return Generator.VisibleHeight; } set { Generator.VisibleHeight = value; } }
		public PageGeneratorLayoutManager LayoutManager { get { return layoutManager; } }
		public abstract PageViewInfoRow CurrentRow { get; }
		#endregion
		public virtual ProcessPageResult PreProcessPage(Page page, int pageIndex) {
			return ProcessPageResult.VisiblePagesGenerationIncomplete;
		}
		public abstract StateProcessPageResult ProcessPage(Page page, int pageIndex);
		protected internal virtual PageViewInfoGeneratorStateBase ChangeState(PageViewInfoGeneratorState stateType) {
			return Generator.ChangeState(stateType);
		}
		protected internal virtual void AddPageToOutput(PageViewInfo page) {
			Generator.Pages.Add(page);
		}
	}
	#endregion
	#region AddPageGeneratorStateBase (abstract class)
	public abstract class AddPageGeneratorStateBase : PageViewInfoGeneratorStateBase {
		protected AddPageGeneratorStateBase(PageViewInfoGeneratorBase generator)
			: base(generator) {
		}
		public override PageViewInfoRow CurrentRow { get { return Generator.CurrentPageRow; } }
		protected internal virtual void AddPage(Page page, int index) {
			PageViewInfoRow row = CurrentRow;
			PageViewInfo pageViewInfo = new PageViewInfo(page);
			pageViewInfo.Index = index;
			int count = Generator.LayoutManager.View.DocumentModel.MainPieceTable.Comments.Count;
			if (Generator.LayoutManager.ShowComments && (count > 0)) {
				page.EnsureCommentBounds(page, Generator.LayoutManager.View.DocumentModel);
				int commentLeft = page.CommentBounds.Left;
				int commentWidth = page.CommentBounds.Width;
				int logicalCommentWidth = commentWidth - (page.Bounds.Right - commentLeft);
				pageViewInfo.Bounds = new Rectangle(new Point(row.Bounds.Right, row.Bounds.Top), LayoutManager.CalculatePageCommentPhysicalSize(page, logicalCommentWidth));
				pageViewInfo.CommentsBounds = Rectangle.FromLTRB((int)(pageViewInfo.Bounds.Right - (LayoutManager.HorizontalPageGap / 2 + commentWidth) * ZoomFactor), pageViewInfo.Bounds.Top, pageViewInfo.Bounds.Right, pageViewInfo.Bounds.Bottom);
			}
			else {
				pageViewInfo.Bounds = new Rectangle(new Point(row.Bounds.Right, row.Bounds.Top), LayoutManager.CalculatePagePhysicalSize(page));
				pageViewInfo.CommentsBounds = Rectangle.Empty;
			}
			row.Add(pageViewInfo);
			RecalculateCurrentRowPagesLayout(page);
			LayoutManager.UpdatePageClientBounds(pageViewInfo);
			row.Bounds = new Rectangle(row.Bounds.Location, new Size(row.Bounds.Width + pageViewInfo.Bounds.Width, Math.Max(row.Bounds.Height, pageViewInfo.Bounds.Height)));
			UpdateHeights(page, pageViewInfo);
			AddPageToOutput(pageViewInfo);
		}
		protected internal abstract void RecalculateCurrentRowPagesLayout(Page page);
		protected internal virtual bool CanAddPage(Page page) {
			return LayoutManager.CanFitPageToPageRow(page, CurrentRow);
		}
		protected internal virtual void UpdateVisibleHeight(PageViewInfo pageViewInfo) {
			int pageVisibleLogicalHeight = LayoutManager.CalculatePageVisibleLogicalHeight(pageViewInfo);
			if (pageVisibleLogicalHeight <= 0)
				pageVisibleLogicalHeight = 0;
			UpdateVisibleHeightCore(pageVisibleLogicalHeight);
		}
		protected internal virtual void UpdateVisibleHeightCore(int visiblePageHeight) {
			VisibleHeight += visiblePageHeight;
		}
		protected internal virtual void UpdateHeights(Page page, PageViewInfo pageViewInfo) {
			UpdateVisibleHeight(pageViewInfo);
			TotalHeight += LayoutManager.CalculatePageLogicalHeight(page);
		}
		protected internal virtual Rectangle CalculatePageBounds(Page page) {
			Rectangle rowBounds = CurrentRow.Bounds;
			Size pageSize = LayoutManager.CalculatePagePhysicalSize(page);
			return new Rectangle(new Point(rowBounds.Right, rowBounds.Top), pageSize);
		}
		protected internal virtual PageVisibility CalculatePageVisibility(Page page, int pageIndex) {
			Rectangle pageBounds = CalculatePageBounds(page);
			Rectangle extendedViewPortBounds = new Rectangle(0, ViewPortBounds.Top, int.MaxValue, ViewPortBounds.Height);
			Rectangle pageViewInfoVisibleBounds = Rectangle.Intersect(extendedViewPortBounds, pageBounds);
			if (pageBounds == pageViewInfoVisibleBounds)
				return PageVisibility.Entire;
			else if (pageViewInfoVisibleBounds.Width > 0 && pageViewInfoVisibleBounds.Height > 0)
				return PageVisibility.Partial;
			else
				return PageVisibility.Invisible;
		}
		protected internal virtual void FinishCurrentRow() {
			Generator.RaiseRowFinished();
			PageViewInfoRow newRow = new PageViewInfoRow();
			newRow.Bounds = LayoutManager.CreateInitialPageViewInfoRowBounds(CurrentRow.Bounds.Bottom);
			Generator.AddPageRow(newRow);
			Generator.CurrentPageRow = newRow;
		}
		protected internal virtual void AlignPagesVertically() {
			LayoutManager.AlignPages(CurrentRow);
		}
	}
	#endregion
	#region StateInvisibleRowBase (abstract class)
	public abstract class StateInvisibleRowBase : AddPageGeneratorStateBase {
		PageViewInfoRow currentRow;
		protected StateInvisibleRowBase(PageViewInfoGeneratorBase generator)
			: base(generator) {
			currentRow = new PageViewInfoRow();
			currentRow.Bounds = LayoutManager.CreateInitialPageViewInfoRowBounds(0);
		}
		public override PageViewInfoRow CurrentRow { get { return currentRow; } }
		protected internal override PageVisibility CalculatePageVisibility(Page page, int pageIndex) {
			if (IsPageFullyInvisible(page, pageIndex))
				return PageVisibility.Invisible;
			else
				return PageVisibility.Partial;
		}
		protected internal override void AddPageToOutput(PageViewInfo page) {
		}
		protected internal override void UpdateVisibleHeight(PageViewInfo pageViewInfo) {
		}
		protected internal virtual void CopyPagesTo(PageViewInfoGeneratorStateBase newState) {
			PageViewInfoRow row = newState.CurrentRow;
			PageViewInfoRow currentRow = CurrentRow;
			row.AddRange(currentRow);
			row.Bounds = currentRow.Bounds;
			int count = currentRow.Count;
			for (int i = 0; i < count; i++)
				newState.AddPageToOutput(currentRow[i]);
		}
		protected internal virtual void HideStateCurrentRow(PageViewInfoGeneratorStateBase newState) {
			PageViewInfoRow row = newState.CurrentRow;
			LayoutManager.OffsetRowVertically(row, -row.Bounds.Height);
		}
		protected internal override void FinishCurrentRow() {
			this.currentRow = new PageViewInfoRow();
			this.currentRow.Bounds = LayoutManager.CreateInitialPageViewInfoRowBounds(0);
		}
		protected internal override void RecalculateCurrentRowPagesLayout(Page page) {
		}
		protected internal abstract bool IsPageFullyVisible(Page page, int pageIndex);
		protected internal abstract bool IsPageFullyInvisible(Page page, int pageIndex);
	}
	#endregion
	#region StateInvisibleEmptyRow (abstract class)
	public abstract class StateInvisibleEmptyRow : StateInvisibleRowBase {
		protected StateInvisibleEmptyRow(PageViewInfoGeneratorBase generator)
			: base(generator) {
		}
		public override PageViewInfoGeneratorState Type { get { return PageViewInfoGeneratorState.InvisibleEmptyRow; } }
		public override StateProcessPageResult ProcessPage(Page page, int pageIndex) {
			switch (CalculatePageVisibility(page, pageIndex)) {
				case PageVisibility.Invisible:
					AddPage(page, pageIndex);
					PageViewInfoGeneratorStateBase newState = ChangeState(PageViewInfoGeneratorState.InvisibleRow);
					CopyPagesTo(newState);
					return StateProcessPageResult.Success;
				case PageVisibility.Partial:
					ChangeState(PageViewInfoGeneratorState.PartialVisibleEmptyRow);
					return StateProcessPageResult.TryAgain;
				default:
				case PageVisibility.Entire:
					ChangeState(PageViewInfoGeneratorState.VisibleEmptyRow);
					return StateProcessPageResult.TryAgain;
			}
		}
		protected internal override PageVisibility CalculatePageVisibility(Page page, int pageIndex) {
			if (IsPageFullyVisible(page, pageIndex))
				return PageVisibility.Entire;
			else
				return base.CalculatePageVisibility(page, pageIndex);
		}
	}
	#endregion
	#region StateInvisibleRow (abstract class)
	public abstract class StateInvisibleRow : StateInvisibleRowBase {
		protected StateInvisibleRow(PageViewInfoGeneratorBase generator)
			: base(generator) {
		}
		public override PageViewInfoGeneratorState Type { get { return PageViewInfoGeneratorState.InvisibleRow; } }
		public override StateProcessPageResult ProcessPage(Page page, int pageIndex) {
			if (IsPageFullyVisible(page, pageIndex)) {
				if (CanAddPage(page))
					SwitchToPartialVisibleRowState(page, pageIndex);
				else {
					FinishCurrentRow();
					ChangeState(PageViewInfoGeneratorState.VisibleEmptyRow);
				}
				return StateProcessPageResult.TryAgain;
			}
			if (!CanAddPage(page)) {
				FinishCurrentRow();
				ChangeState(PageViewInfoGeneratorState.InvisibleEmptyRow);
				return StateProcessPageResult.TryAgain;
			}
			switch (CalculatePageVisibility(page, pageIndex)) {
				case PageVisibility.Invisible:
					AddPage(page, pageIndex);
					return StateProcessPageResult.Success;
				case PageVisibility.Partial:
					return SwitchToPartialVisibleRowState(page, pageIndex);
				default:
				case PageVisibility.Entire:
					Exceptions.ThrowInternalException();
					return StateProcessPageResult.Success;
			}
		}
		protected internal virtual StateProcessPageResult SwitchToPartialVisibleRowState(Page page, int pageIndex) {
			AlignPagesVertically();
			StatePartialVisibleRow state = (StatePartialVisibleRow)ChangeState(PageViewInfoGeneratorState.PartialVisibleRow);
			RollbackStateToTheStartOfRow();
			CopyPagesTo(state);
			HideStateCurrentRow(state);
			return StateProcessPageResult.TryAgain;
		}
		protected internal abstract void RollbackStateToTheStartOfRow();
	}
	#endregion
	#region StateInvisibleEmptyRowRunningHeight
	public class StateInvisibleEmptyRowRunningHeight : StateInvisibleEmptyRow {
		public StateInvisibleEmptyRowRunningHeight(PageViewInfoGeneratorRunningHeight generator)
			: base(generator) {
		}
		public new PageViewInfoGeneratorRunningHeight Generator { get { return (PageViewInfoGeneratorRunningHeight)base.Generator; } }
		protected internal override void UpdateHeights(Page page, PageViewInfo pageViewInfo) {
			base.UpdateHeights(page, pageViewInfo);
			Generator.RunningHeight += LayoutManager.CalculatePageLogicalHeight(page);
		}
		protected internal override bool IsPageFullyVisible(Page page, int pageIndex) {
			return Generator.RunningHeight == Generator.FirstPageAnchor.TopInvisibleHeight;
		}
		protected internal override bool IsPageFullyInvisible(Page page, int pageIndex) {
			long runningHeight = Generator.RunningHeight + LayoutManager.CalculatePageLogicalHeight(page);
			return runningHeight <= Generator.FirstPageAnchor.TopInvisibleHeight;
		}
	}
	#endregion
	#region StateInvisibleRowRunningHeight
	public class StateInvisibleRowRunningHeight : StateInvisibleRow {
		public StateInvisibleRowRunningHeight(PageViewInfoGeneratorRunningHeight generator)
			: base(generator) {
		}
		public new PageViewInfoGeneratorRunningHeight Generator { get { return (PageViewInfoGeneratorRunningHeight)base.Generator; } }
		protected internal override void UpdateHeights(Page page, PageViewInfo pageViewInfo) {
			base.UpdateHeights(page, pageViewInfo);
			Generator.RunningHeight += LayoutManager.CalculatePageLogicalHeight(page);
		}
		protected internal override bool IsPageFullyVisible(Page page, int pageIndex) {
			return Generator.RunningHeight == Generator.FirstPageAnchor.TopInvisibleHeight;
		}
		protected internal override bool IsPageFullyInvisible(Page page, int pageIndex) {
			long runningHeight = Generator.RunningHeight + LayoutManager.CalculatePageLogicalHeight(page);
			return runningHeight <= Generator.FirstPageAnchor.TopInvisibleHeight;
		}
		protected internal override void RollbackStateToTheStartOfRow() {
			Generator.RunningHeight -= LayoutManager.CalculatePagesTotalLogicalHeight(CurrentRow);
		}
	}
	#endregion
	#region StateInvisibleEmptyRowFirstPageOffset
	public class StateInvisibleEmptyRowFirstPageOffset : StateInvisibleEmptyRow {
		public StateInvisibleEmptyRowFirstPageOffset(PageViewInfoGeneratorFirstPageOffset generator)
			: base(generator) {
		}
		public new PageViewInfoGeneratorFirstPageOffset Generator { get { return (PageViewInfoGeneratorFirstPageOffset)base.Generator; } }
		protected internal override bool IsPageFullyVisible(Page page, int pageIndex) {
			return false;
		}
		protected internal override bool IsPageFullyInvisible(Page page, int pageIndex) {
			FirstPageOffsetFirstPageAnchor anchor = Generator.FirstPageAnchor;
			return pageIndex < anchor.PageIndex;
		}
	}
	#endregion
	#region StateInvisibleRowFirstPageOffset
	public class StateInvisibleRowFirstPageOffset : StateInvisibleRow {
		public StateInvisibleRowFirstPageOffset(PageViewInfoGeneratorFirstPageOffset generator)
			: base(generator) {
		}
		public new PageViewInfoGeneratorFirstPageOffset Generator { get { return (PageViewInfoGeneratorFirstPageOffset)base.Generator; } }
		protected internal override bool IsPageFullyVisible(Page page, int pageIndex) {
			return false;
		}
		protected internal override bool IsPageFullyInvisible(Page page, int pageIndex) {
			FirstPageOffsetFirstPageAnchor anchor = Generator.FirstPageAnchor;
			return pageIndex < anchor.PageIndex;
		}
		protected internal override void RollbackStateToTheStartOfRow() {
		}
		protected internal override StateProcessPageResult SwitchToPartialVisibleRowState(Page page, int pageIndex) {
			AddPage(page, pageIndex);
			base.SwitchToPartialVisibleRowState(page, pageIndex);
			return StateProcessPageResult.Success;
		}
		protected internal override void HideStateCurrentRow(PageViewInfoGeneratorStateBase newState) {
			int lastPageOffset = (int)Math.Ceiling(Generator.FirstPageAnchor.VerticalOffset * ZoomFactor);
			PageViewInfoRow row = newState.CurrentRow;
			int sectionLine = row.Last.Bounds.Top + lastPageOffset;
			LayoutManager.OffsetRowVertically(row, -row.Bounds.Top - sectionLine);
			VisibleHeight = LayoutManager.CalculatePagesTotalVisibleLogicalHeight(row);
		}
	}
	#endregion
	#region StatePartialVisibleEmptyRow (abstract class)
	public abstract class StatePartialVisibleEmptyRow : AddPageGeneratorStateBase {
		protected StatePartialVisibleEmptyRow(PageViewInfoGeneratorBase generator)
			: base(generator) {
		}
		public override PageViewInfoGeneratorState Type { get { return PageViewInfoGeneratorState.PartialVisibleEmptyRow; } }
		public override StateProcessPageResult ProcessPage(Page page, int pageIndex) {
			Debug.Assert(VisibleHeight == 0);
			AddPage(page, pageIndex);
			ChangeState(PageViewInfoGeneratorState.PartialVisibleRow);
			return StateProcessPageResult.Success;
		}
		protected internal override void RecalculateCurrentRowPagesLayout(Page page) {
			PageViewInfoRow row = CurrentRow;
			Debug.Assert(row.Count == 1);
			int invisiblePageAreaHeight = CalculateVerticalRowOffset();
			row.Bounds = LayoutManager.OffsetRectangle(row.Bounds, 0, -invisiblePageAreaHeight);
			PageViewInfo pageViewInfo = row[0];
			pageViewInfo.Bounds = LayoutManager.OffsetRectangle(pageViewInfo.Bounds, 0, -invisiblePageAreaHeight);
			LayoutManager.UpdatePageClientBounds(pageViewInfo);
		}
		protected internal override void UpdateHeights(Page page, PageViewInfo pageViewInfo) {
			AlignPagesVertically();
			base.UpdateHeights(page, pageViewInfo);
		}
		protected internal abstract int CalculateVerticalRowOffset();
	}
	#endregion
	#region StatePartialVisibleRow (abstract class)
	public abstract class StatePartialVisibleRow : AddPageGeneratorStateBase {
		protected StatePartialVisibleRow(PageViewInfoGeneratorBase generator)
			: base(generator) {
		}
		public override PageViewInfoGeneratorState Type { get { return PageViewInfoGeneratorState.PartialVisibleRow; } }
		public override ProcessPageResult PreProcessPage(Page page, int pageIndex) {
			if (CurrentRow.Bounds.Bottom >= ViewPortBounds.Bottom && !CanAddPage(page))
				return ProcessPageResult.VisiblePagesGenerationComplete;
			else
				return ProcessPageResult.VisiblePagesGenerationIncomplete;
		}
		public override StateProcessPageResult ProcessPage(Page page, int pageIndex) {
			if (!CanAddPage(page)) {
				FinishCurrentRow();
				ChangeState(PageViewInfoGeneratorState.VisibleEmptyRow);
				return StateProcessPageResult.TryAgain;
			}
			else {
				AddPage(page, pageIndex);
				return StateProcessPageResult.Success;
			}
		}
		protected internal override void UpdateHeights(Page page, PageViewInfo pageViewInfo) {
			AlignPagesVertically();
			base.UpdateHeights(page, pageViewInfo);
		}
		protected internal override void UpdateVisibleHeight(PageViewInfo pageViewInfo) {
			VisibleHeight = LayoutManager.CalculatePagesTotalVisibleLogicalHeight(CurrentRow);
		}
	}
	#endregion
	#region StatePartialVisibleEmptyRowRunningHeight
	public class StatePartialVisibleEmptyRowRunningHeight : StatePartialVisibleEmptyRow {
		public StatePartialVisibleEmptyRowRunningHeight(PageViewInfoGeneratorRunningHeight generator)
			: base(generator) {
		}
		public new PageViewInfoGeneratorRunningHeight Generator { get { return (PageViewInfoGeneratorRunningHeight)base.Generator; } }
		protected internal override int CalculateVerticalRowOffset() {
			return (int)Math.Round(((Generator.FirstPageAnchor.TopInvisibleHeight - Generator.RunningHeight)) * ZoomFactor);
		}
	}
	#endregion
	#region StatePartialVisibleRowRunningHeight
	public class StatePartialVisibleRowRunningHeight : StatePartialVisibleRow {
		public StatePartialVisibleRowRunningHeight(PageViewInfoGeneratorRunningHeight generator)
			: base(generator) {
		}
		public new PageViewInfoGeneratorRunningHeight Generator { get { return (PageViewInfoGeneratorRunningHeight)base.Generator; } }
		protected internal override void RecalculateCurrentRowPagesLayout(Page page) {
			PageViewInfoRow row = CurrentRow;
			int physicalPageHeight = LayoutManager.CalculatePagePhysicalHeight(page);
			PageViewInfo pageViewInfo = row.Last;
			pageViewInfo.Bounds = LayoutManager.OffsetRectangle(pageViewInfo.Bounds, 0, (row.Bounds.Height - physicalPageHeight) / 2);
			if (pageViewInfo.Bounds.Top < row.Bounds.Top) {
				row.Bounds = new Rectangle(row.Bounds.Left, pageViewInfo.Bounds.Top, row.Bounds.Width, pageViewInfo.Bounds.Height);
			}
			int offset = CalculatePartialRowOffset();
			Debug.Assert(offset != ~(row.Bounds.Height + 1));
			if (offset < 0)
				offset = ~offset;
			LayoutManager.OffsetRowVertically(row, -row.Bounds.Top - offset);
		}
		protected internal virtual int CalculatePartialRowOffset() {
			long totalInvisibleRowHeight = (long)Math.Ceiling((Generator.FirstPageAnchor.TopInvisibleHeight - Generator.RunningHeight) * ZoomFactor);
			int low = 1;
			int hi = CurrentRow.Bounds.Height;
			while (low <= hi) {
				int median = (low + ((hi - low) >> 1));
				int compareResult = CheckRowOffset(median, totalInvisibleRowHeight);
				if (compareResult == 0)
					return median;
				if (compareResult < 0)
					low = median + 1;
				else
					hi = median - 1;
			}
			return ~low;
		}
		protected internal virtual int CheckRowOffset(int offset, long totalInvisibleRowHeight) {
			PageViewInfoRow row = CurrentRow;
			Rectangle invisibleBounds = row.Bounds;
			invisibleBounds.Height = offset;
			int total = 0;
			int count = row.Count;
			for (int i = 0; i < count; i++) {
				Rectangle pageBounds = row[i].Bounds;
				int delta = Math.Min(invisibleBounds.Bottom, pageBounds.Bottom) - pageBounds.Y;
				if (delta > 0) {
					total += delta;
				}
			}
			if (total < totalInvisibleRowHeight)
				return -1;
			if (total > totalInvisibleRowHeight)
				return 1;
			return 0;
		}
	}
	#endregion
	#region StatePartialVisibleEmptyRowFirstPageOffset
	public class StatePartialVisibleEmptyRowFirstPageOffset : StatePartialVisibleEmptyRow {
		public StatePartialVisibleEmptyRowFirstPageOffset(PageViewInfoGeneratorFirstPageOffset generator)
			: base(generator) {
		}
		public new PageViewInfoGeneratorFirstPageOffset Generator { get { return (PageViewInfoGeneratorFirstPageOffset)base.Generator; } }
		protected internal override int CalculateVerticalRowOffset() {
			return (int)Math.Ceiling(Generator.FirstPageAnchor.VerticalOffset * ZoomFactor);
		}
	}
	#endregion
	#region StatePartialVisibleRowFirstPageOffset
	public class StatePartialVisibleRowFirstPageOffset : StatePartialVisibleRow {
		public StatePartialVisibleRowFirstPageOffset(PageViewInfoGeneratorFirstPageOffset generator)
			: base(generator) {
		}
		public new PageViewInfoGeneratorFirstPageOffset Generator { get { return (PageViewInfoGeneratorFirstPageOffset)base.Generator; } }
		protected internal override void RecalculateCurrentRowPagesLayout(Page page) {
		}
		protected internal override void UpdateHeights(Page page, PageViewInfo pageViewInfo) {
			AlignPagesVertically();
			base.UpdateHeights(page, pageViewInfo);
		}
	}
	#endregion
	#region StateVisibleEmptyRow
	public class StateVisibleEmptyRow : AddPageGeneratorStateBase {
		public StateVisibleEmptyRow(PageViewInfoGeneratorBase generator)
			: base(generator) {
		}
		public override PageViewInfoGeneratorState Type { get { return PageViewInfoGeneratorState.VisibleEmptyRow; } }
		public override StateProcessPageResult ProcessPage(Page page, int pageIndex) {
			if (CalculatePageVisibility(page, pageIndex) == PageVisibility.Invisible) {
				RemoveLastRow();
				ChangeState(PageViewInfoGeneratorState.VisiblePagesGenerationComplete);
				return StateProcessPageResult.TryAgain;
			}
			else {
				long initialVisibleHeight = VisibleHeight;
				AddPage(page, pageIndex);
				StateVisibleRow state = (StateVisibleRow)ChangeState(PageViewInfoGeneratorState.VisibleRow);
				state.InitialVisibleHeight = initialVisibleHeight;
				return StateProcessPageResult.Success;
			}
		}
		protected internal virtual void RemoveLastRow() {
			PageViewInfoRowCollection rows = Generator.PageRows;
			rows.RemoveAt(rows.Count - 1);
		}
		protected internal override void UpdateHeights(Page page, PageViewInfo pageViewInfo) {
			AlignPagesVertically();
			base.UpdateHeights(page, pageViewInfo);
		}
		protected internal override void RecalculateCurrentRowPagesLayout(Page page) {
		}
	}
	#endregion
	#region StateVisibleRow
	public class StateVisibleRow : AddPageGeneratorStateBase {
		long initialVisibleHeight;
		public StateVisibleRow(PageViewInfoGeneratorBase generator)
			: base(generator) {
		}
		public override PageViewInfoGeneratorState Type { get { return PageViewInfoGeneratorState.VisibleRow; } }
		public long InitialVisibleHeight { get { return initialVisibleHeight; } set { initialVisibleHeight = value; } }
		public override ProcessPageResult PreProcessPage(Page page, int pageIndex) {
			if (CurrentRow.Bounds.Bottom >= ViewPortBounds.Bottom && !CanAddPage(page))
				return ProcessPageResult.VisiblePagesGenerationComplete;
			else
				return ProcessPageResult.VisiblePagesGenerationIncomplete;
		}
		public override StateProcessPageResult ProcessPage(Page page, int pageIndex) {
			if (!CanAddPage(page)) {
				FinishCurrentRow();
				ChangeState(PageViewInfoGeneratorState.VisibleEmptyRow);
				return StateProcessPageResult.TryAgain;
			}
			switch (CalculatePageVisibility(page, pageIndex)) {
				default:
				case PageVisibility.Invisible:
					Exceptions.ThrowInternalException();
					return StateProcessPageResult.Success;
				case PageVisibility.Partial:
				case PageVisibility.Entire:
					AddPage(page, pageIndex);
					return StateProcessPageResult.Success;
			}
		}
		protected internal override void UpdateVisibleHeight(PageViewInfo pageViewInfo) {
			AlignPagesVertically();
			VisibleHeight = InitialVisibleHeight + LayoutManager.CalculatePagesTotalVisibleLogicalHeight(CurrentRow);
		}
		protected internal override void RecalculateCurrentRowPagesLayout(Page page) {
		}
	}
	#endregion
	#region StateVisiblePagesGenerationComplete
	public class StateVisiblePagesGenerationComplete : PageViewInfoGeneratorStateBase {
		public StateVisiblePagesGenerationComplete(PageViewInfoGeneratorBase generator)
			: base(generator) {
		}
		public override PageViewInfoGeneratorState Type { get { return PageViewInfoGeneratorState.VisiblePagesGenerationComplete; } }
		public override PageViewInfoRow CurrentRow { get { return null; } }
		public override StateProcessPageResult ProcessPage(Page page, int pageIndex) {
			TotalHeight += LayoutManager.CalculatePageLogicalHeight(page);
			return StateProcessPageResult.Success;
		}
	}
	#endregion
	#region InvisiblePageRowsGenerator
	public class InvisiblePageRowsGenerator {
		#region Fields
		readonly PageCollection pages;
		readonly PageViewInfoGeneratorBase generator;
		int firstPageIndex;
		int firstInvalidPageIndex;
		int step;
		int currentPageIndex;
		PageViewInfoRow result;
		readonly PageGeneratorLayoutManager layoutManager;
		bool isFinished;
		int pagesProcessed;
		#endregion
		public InvisiblePageRowsGenerator(PageCollection pages, PageGeneratorLayoutManager sourceLayoutManager) {
			Guard.ArgumentNotNull(pages, "pages");
			Guard.ArgumentNotNull(sourceLayoutManager, "sourceLayoutManager");
			this.layoutManager = sourceLayoutManager.Clone();
			Rectangle viewPortBounds = layoutManager.ViewPortBounds;
			viewPortBounds.Height = int.MaxValue / 2;
			layoutManager.ViewPortBounds = viewPortBounds;
			this.pages = pages;
			this.generator = new PageViewInfoGeneratorRunningHeight(layoutManager, new RunningHeightFirstPageAnchor(), new PageViewInfoCollection());
			Reset();
		}
		#region Properties
		#region FirstPageIndex
		public int FirstPageIndex {
			get { return firstPageIndex; }
			set {
				if (firstPageIndex == value)
					return;
				firstPageIndex = value;
				Reset();
			}
		}
		#endregion
		#region FirstInvalidPageIndex
		public int FirstInvalidPageIndex {
			get { return firstInvalidPageIndex; }
			set {
				if (firstInvalidPageIndex == value)
					return;
				firstInvalidPageIndex = value;
				Reset();
			}
		}
		#endregion
		protected internal PageViewInfoGeneratorBase Generator { get { return generator; } }
		protected internal PageCollection Pages { get { return pages; } }
		protected internal int Step { get { return step; } }
		protected internal int CurrentPageIndex { get { return currentPageIndex; } }
		protected internal PageViewInfoRow Result { get { return result; } }
		protected internal PageGeneratorLayoutManager LayoutManager { get { return layoutManager; } }
		protected internal bool IsFinished { get { return isFinished; } }
		protected internal int PagesProcessed { get { return pagesProcessed; } }
		#endregion
		protected internal virtual void Reset() {
			if (firstPageIndex <= firstInvalidPageIndex)
				step = 1;
			else
				step = -1;
			currentPageIndex = firstPageIndex;
			pagesProcessed = 0;
			isFinished = false;
			result = null;
		}
		public virtual PageViewInfoRow GenerateNextRow() {
			if (isFinished)
				return null;
			result = null;
			generator.RowFinished += OnRowFinished;
			try {
				for (; currentPageIndex != firstInvalidPageIndex; currentPageIndex += step) {
					Generator.ProcessPage(pages[currentPageIndex], currentPageIndex);
					pagesProcessed++;
					if (result != null) {
						currentPageIndex += step;
						return result;
					}
				}
			}
			finally {
				generator.RowFinished -= OnRowFinished;
			}
			isFinished = true;
			if (pagesProcessed <= 0)
				return null;
			result = generator.PageRows.Last;
			return result;
		}
		protected internal virtual void OnRowFinished(object sender, EventArgs e) {
			result = generator.PageRows.Last;
		}
	}
	#endregion
}
