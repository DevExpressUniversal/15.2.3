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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Localization;
using System.Collections;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Design;
using DevExpress.Utils.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Skins;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
using DevExpress.DocumentView.Controls;
namespace System.Drawing {
	public static class GraphicsExtentions {
		public static void ExecuteAndKeepState(this Graphics graph, Action action) {
			GraphicsState state = graph.Save();
			try {
				action();
			} finally {
				graph.Restore(state);
			}
		}
	}
}
namespace DevExpress.DocumentView {
	public class PageEnumerator : IEnumerator {
		private ViewManager viewManager;
		private int index;
		object IEnumerator.Current {
			get { 
				return Current;
			}
		}
		int PageIndex {
			get { return viewManager.StartIndex + index; }
		}
		public virtual IPage Current {
			get {
				IPage page;
				return viewManager.Pages.TryGetValue(PageIndex, out page) ? page : null;
			}
		}
		public RectangleF CurrentPlace { get { return viewManager.GetPageRect(Current); } }
		public PageEnumerator(ViewManager viewManager) {
			this.viewManager = viewManager;
			Reset();
		}
		public virtual bool MoveNext() {
			index += 1;
			return viewManager.Places.IsValidIndex(index) && viewManager.Pages.IsValidIndex(PageIndex);
		}
		public virtual void Reset() {
			index = -1;
		}
	}
	public class ViewManager {
		static SizeF fakedPageSize = new SizeF(1, 1);
		#region fields & properties
		protected int fStartIndex;
		protected SizeF placeSize;
		protected PointF fScrollPos = PointF.Empty;
		protected IList<PointF> pagePlaces = new List<PointF>();
		protected List<SizeF> heights = new List<SizeF>();
		protected DocumentViewerBase pc;
		ViewControl viewControl;
		protected int fSelectedPagePlaceIndex = -1;
		IDocument Document {
			get { return pc.Document; }
		}
		public IList<IPage> Pages {
			get { return Document != null ? Document.Pages : new IPage[] { }; }
		}
		public IList<PointF> Places {
			get { return pagePlaces; } 
		}
		public IPage SelectedPage { 
			get {
				IPage page;
				return Pages.TryGetValue(SelectedPagePlaceIndex, out page) ? page : null; 
			}
			set {
				SelectedPagePlaceIndex = Pages.IndexOf(value);
			}
		}
		public int SelectedPagePlaceIndex { 
			get { return fSelectedPagePlaceIndex; }
			set {
				if(Pages.Count > 0) {
					SetSelectedIndex(value, false);
				} else {
					fSelectedPagePlaceIndex = -1;
				}
			}
		}
		protected SkinPaddingEdges Edges {
			get { return pc.ControlInfo.PagePaddingEdges; }
		}
		protected float GetInnerMarginHeight(float zoom) {
			return pc.ControlInfo.PageVerticalIndent + PSUnitConverter.PixelToDoc(Edges.Height, zoom);
		}
		protected float GetInnerMarginWidth(float zoom) {
			return pc.ControlInfo.PageHorizontalIndent + PSUnitConverter.PixelToDoc(Edges.Width, zoom);
		}
		public virtual int StartIndex { 
			get { return fStartIndex; } 
			set { 
				fStartIndex = Math.Max(0, Math.Min(value, Pages.Count / pagePlaces.Count * pagePlaces.Count)); 
			}
		}
		public PointF ScrollPos { get { return fScrollPos; }
		}
		protected float MaxScrollY {
			get {
				SizeF previewSize = PSUnitConverter.PixelToDoc(ClientSize, pc.Zoom);
				return Math.Max(0, placeSize.Height - previewSize.Height);
			}
		}
#if DEBUGTEST
		public float Test_MaxScrollY {
			get { return MaxScrollY; }
		}
#endif
		protected float MaxScrollX {
			get {
				SizeF previewSize = PSUnitConverter.PixelToDoc(ClientSize, pc.Zoom);
				return Math.Max(0, placeSize.Width - previewSize.Width);
			}
		}
		private int DocPlaceCount { 
			get { 
				if(pagePlaces.Count > 0) {
					float count = (float)Pages.Count / pagePlaces.Count;
					return (int)Math.Ceiling(count);
				}
				return 0;
			}
		}
		[
		Browsable(false), 
		]
		public SizeF ViewSize { 
			get { 
				SizeF size = placeSize;
				if(pagePlaces.Count != 0) {
					size.Height *= DocPlaceCount;
				}
				return size;
			}
		}
		public SizeF DocOffset { 
			get {
				SizeF offset = new SizeF(fScrollPos);
				if (pagePlaces.Count == 0)
					return offset;
				offset.Height += StartIndex / pagePlaces.Count * placeSize.Height;
				return offset;
			}
		}
		protected bool IsMarginResizing { 
			get {
				IPage page = SelectedPage;
				return (page == null) ? false : pc.Margins.IsMarginResizing;
			}
		}
		public bool IsWholePageVisible { 
			get {
				if (pagePlaces.Count < 1) return false;
				RectangleF previewRect = new RectangleF(new PointF(0,0), (SizeF)PSUnitConverter.PixelToDoc(ClientSize, pc.Zoom));
				for(int i = 0; i < pagePlaces.Count; i++) {
					if(!previewRect.Contains(GetPageRect(i)))
						return false;
				}
				return true;
			}
		}
		protected Size ClientSize { get { return viewControl.ClientSize; } 
		}
		protected Rectangle ClientRectangle { get { return viewControl.ClientRectangle; } 
		}
		protected int Width { get { return viewControl.Width; } 
		}
		protected int Height { get { return viewControl.Height; } 
		}
		SizeF PageSize { get { return Document != null ? Document.PageSettings.PageSize : fakedPageSize; } }
		#endregion
		public ViewManager(ViewManager viewManager) : this(viewManager.pc, viewManager.viewControl) {
			this.fSelectedPagePlaceIndex = viewManager.fSelectedPagePlaceIndex;
		}
		public ViewManager(DocumentViewerBase pc, ViewControl viewControl) {
			this.pc = pc;
			this.viewControl = viewControl;
		}
		public virtual int GetPageIndex(float verticalScroll) {
			return GetStartIndex(verticalScroll);
		}
		public bool IsSelected(IPage page) { 
			if(page == null || SelectedPage == null) return false;
			return page.Equals(SelectedPage);
		}
		protected internal void SetSelectedIndex(int value, bool force) {
			int index = Pages.GetValidIndex(value);
			if(fSelectedPagePlaceIndex == index && !force)
				return;
			InvalidatePage(SelectedPage);
			fSelectedPagePlaceIndex = index;
			ShowPage(SelectedPage);
			InvalidatePage(SelectedPage);
			pc.RaiseSelectedPageChanged();
		}
		public void ResetSelectedPageIndex() {
			SelectedPagePlaceIndex = Pages.Count == 0 ? -1 : 
				Math.Max(0, fSelectedPagePlaceIndex);
		}
		public void Reset() {
			pagePlaces.Clear();
			fStartIndex = 0;
			fScrollPos = PointF.Empty;
		}
		private int GetPlaceCount(float vertScrool) {
			int count = (int)Math.Round(vertScrool / placeSize.Height);
			return Math.Min(count, DocPlaceCount - 1);
		}
		public int GetStartIndex(float vertScrool) {
			int count = GetPlaceCount(vertScrool);
			return count * pagePlaces.Count;
		}
		public virtual void SetVertScroll(float val) {
			int count = GetPlaceCount(val);
			StartIndex = count * pagePlaces.Count;
			if(!IsVisiblePage(SelectedPage))
				SelectedPagePlaceIndex = StartIndex;
			SetScrollPosY(val - count * placeSize.Height);
		}
		public virtual void ValidateVertScroll() {
			if(ContainsWholePage())
				fScrollPos.Y = 0;
		}
		bool ContainsWholePage() {
			SizeF pageSize = PSUnitConverter.DocToPixel(PageSize, pc.Zoom);
			return (pageSize.Height <= ClientSize.Height);
		}
		protected void SetScrollPosY(float val) {
			fScrollPos.Y = Math.Max(0, Math.Min(val, MaxScrollY));
		}
		public void SetHorizScroll(float val) {
			fScrollPos.X = Math.Max(0, Math.Min(val, MaxScrollX)); 
		}
		public virtual void OffsetVertScroll(float dy) {
			OffsetVertScroll(dy, true);
		}
		public virtual void OffsetVertScroll(float dy, bool canChangePage) {
			if(dy > 0 && fScrollPos.Y < MaxScrollY) {
				SetScrollPosY(fScrollPos.Y + dy);
			} else if(dy > 0 && canChangePage && ShowNextPages()) {
				SetScrollPosY(0);
				SelectedPagePlaceIndex = StartIndex;
			} else if(dy <= 0 && fScrollPos.Y > 0) {
				SetScrollPosY(fScrollPos.Y + dy);
			} else if(dy <= 0 && canChangePage &&  ShowPrevPages()) {
				SetScrollPosY(MaxScrollY);
				SelectedPagePlaceIndex = StartIndex;
			}
		}
		public virtual void OffsetHorzScroll(float dx) {
			SetHorizScroll(fScrollPos.X + dx);
		}
		public bool IsVisiblePage(IPage page) {
			return IsVisiblePlace(GetPlaceIndex(page));
		}
		bool IsVisiblePlace(int placeIndex) {
			if(!pagePlaces.IsValidIndex(placeIndex)) 
				return false;
			RectangleF previewRect = new RectangleF(new PointF(ScrollPos.X, ScrollPos.Y), PSUnitConverter.PixelToDoc(ClientSize, pc.Zoom));
			RectangleF pageRect = GetPageRect(placeIndex);
			float square = pageRect.Width * pageRect.Height;
			if(square == 0)
				return false;
			previewRect.Intersect(pageRect);
			return (previewRect.Width * previewRect.Height / square > 0.75f);
		}
		protected RectangleF GetPageRect(int placeIndex) {
			int pageIndex = StartIndex + placeIndex;
			SizeF pageSize = Pages.IsValidIndex(pageIndex) ? Pages[pageIndex].PageSize : SizeF.Empty;
			return GetAlignedPageRect(pagePlaces, placeIndex, pageSize, heights);
		}
		RectangleF GetPageRectCore(int placeIndex, SizeF pageSize, List<SizeF> heights) {
			if(pagePlaces.IsValidIndex(placeIndex))
				return GetAlignedPageRect(pagePlaces, placeIndex, pageSize, heights);
			return RectangleF.Empty;
		}
		RectangleF GetAlignedPageRect(IList<PointF> places, int placeIndex, SizeF pageSize, List<SizeF> heights) {
			RectangleF pageRect = new RectangleF(PointF.Empty, pageSize);
			RectangleF placeRect = new RectangleF(places[placeIndex], heights[placeIndex]);
			return RectF.Align(pageRect, placeRect, BrickAlignment.Center, BrickAlignment.Center);
		}
		public virtual void ShowPage(IPage page) {
			if(page == null || IsVisiblePage(page) || pagePlaces.Count == 0) return;
			StartIndex = page.Index / pagePlaces.Count * pagePlaces.Count; 
			System.Diagnostics.Debug.Assert(StartIndex >= 0);
		}
		public void ShowSelectedPage() {
			ShowPage(SelectedPage);
		}
		public void ShowPagePoint(IPage page, PointF pagePoint) {
			if(page == null) return;
			ShowPage(page);
			RectangleF r = GetPageRect(page);
			if(r.IsEmpty) return;
			PointF pt = PSNativeMethods.TranslatePointF(pagePoint, r.Location);
			SizeF sz = ClientSize;
			sz = PSUnitConverter.PixelToDoc(sz, pc.Zoom);
			PointF centerPoint = new PointF(sz.Width / 2, sz.Height / 2);
			SetScrollPosY(pt.Y- centerPoint.Y);
			SetHorizScroll(pt.X - centerPoint.X);
		}
		public void ShowPagePointTop(IPage page, PointF pagePoint) {
			if(page == null) return;
			ShowPage(page);
			RectangleF r = GetPageRect(page);
			if(r.IsEmpty) return;
			PointF pt = PSNativeMethods.TranslatePointF(pagePoint, r.Location);
			SetScrollPosY(pt.Y);
			SetHorizScroll(pt.X);
		}
		protected bool ShowNextPages() {
			if(StartIndex + pagePlaces.Count < Pages.Count) {
				StartIndex += pagePlaces.Count; 
				return true;
			}
			return false;
		}
		protected bool ShowPrevPages() {
			if(StartIndex - pagePlaces.Count >= 0) {
				StartIndex -= pagePlaces.Count; 
				return true;
			}
			return false;
		}
		[Obsolete("Use PageContainsScreenPoint"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool PageContainsSreenPoint(IPage page, Point pt) {
			return PageContainsScreenPoint(page, viewControl.PointToClient(pt));
		}
		public bool PageContainsScreenPoint(IPage page, Point pt) {
			return PageContainsPoint(page, viewControl.PointToClient(pt));
		}
		public bool PageContainsPoint(IPage page, PointF pt) {
			if(page == null) return false;
			pt = PSUnitConverter.PixelToDoc(pt, pc.Zoom, fScrollPos);
			RectangleF r = GetPageRect(page);
			return r.IsEmpty ? false : r.Contains(pt);
		}
		public PageEnumerator CreatePageEnumerator() {
			return new PageEnumerator(this);
		}
		public IPage FindPage(Point screenPoint) {
			PointF pt = PSUnitConverter.PixelToDoc(PointToView(screenPoint), pc.Zoom, fScrollPos);
			PageEnumerator pe = CreatePageEnumerator();
			while(pe.MoveNext()) {
				RectangleF r = pe.CurrentPlace;
				if(r.Contains(pt)) return pe.Current;
			}
			return null;
		}
		public RectangleF GetPageRect(IPage page) {
			return page != null ? GetPageRectCore(GetPlaceIndex(page), page.PageSize, heights) : RectangleF.Empty;
		}
		int GetPlaceIndex(IPage page) {
			return page != null ? page.Index - StartIndex : -1;
		}
		protected SizeF CalcMaxPageSize() {
			if(pc.DocumentIsCreating || Pages.Count == 0)
				return this.PageSize;
				float maxWidth = 0;
				float maxHeight = 0;
				foreach(IPage page in Pages) {
					SizeF pageSize = page.PageSize;
					if(pageSize.Width > maxWidth)
						maxWidth = pageSize.Width;
					if(pageSize.Height > maxHeight)
						maxHeight = pageSize.Height;
				}
				return new SizeF(maxWidth, maxHeight);
			}
		public RectangleF CalcWidestUsefulPageRect() {
			RectangleF widestUsefulPageRect = new RectangleF(0, 0, 1, 1);
			foreach(IPage page in Pages) {
				RectangleF usefulPageRect = page.UsefulPageRectF;
				if(usefulPageRect.Width > widestUsefulPageRect.Width)
					widestUsefulPageRect = usefulPageRect;
			}
			return widestUsefulPageRect;
		}
		public float GetMaxPageWidthZoomFactor() {
			return GetWidthZoomFactor(CalcMaxPageSize().Width);
		}
		public float GetWidthZoomFactor(float width) {
			return GetZoomFactor(new SizeF(width, 0f), 1, 0);
		}
		public float GetZoomFactor(int columns, int rows) {
			return GetZoomFactor(CalcMaxPageSize(), columns, rows);
		}
		float GetZoomFactor(SizeF pageSize, int columns, int rows) {
			System.Diagnostics.Debug.Assert(!pageSize.IsEmpty);
			float zoom1 = GetZoomFactorCore(columns, rows, pageSize, pc.Zoom);
			for(; ; ) {
				float zoom2 = GetZoomFactorCore(columns, rows, pageSize, zoom1);
				if(Math.Abs(zoom1 - zoom2) < 0.0005)
					return zoom2;
				zoom1 = zoom2;
			}
		}
		float GetZoomFactorCore(int columns, int rows, SizeF pageSize, float zoom) {
			SizeF placeSize = GetPagePlaceSize(pageSize, zoom);
			float horiz = placeSize.Width * columns - GetInnerMarginWidth(zoom) + GetMinLeftMargin(zoom) + GetMinRightMargin(zoom);
			float horZoom = columns != 0 ? Math.Max(1, Width) / GraphicsUnitConverter.DocToPixel(horiz) : float.MaxValue;
			float vert = placeSize.Height * rows - GetInnerMarginHeight(zoom) + GetMinTopMargin(zoom) + GetMinBottomMargin(zoom);
			float verZoom = rows != 0 ? Math.Max(1, Height) / GraphicsUnitConverter.DocToPixel(vert) : float.MaxValue;
			return Math.Min(horZoom, verZoom);
		}
		private SizeF GetPagePlaceSize(float zoom) {
			return GetPagePlaceSize(CalcMaxPageSize(), zoom);
		}
		private SizeF GetPagePlaceSize(SizeF size, float zoom) {
			size.Width += GetInnerMarginWidth(zoom);
			size.Height += GetInnerMarginHeight(zoom);
			return size;
		}
		public int GetViewColumns(float zoom) {
			float columns = ClientRectangle.Width / PSUnitConverter.DocToPixel(GetPagePlaceSize(zoom), zoom).Width;
			columns = Math.Max(1f, (float)Math.Floor(columns));
			return (int)columns;
		}
		public int GetViewRows(float zoom) {
			float rows = ClientRectangle.Height / PSUnitConverter.DocToPixel(GetPagePlaceSize(zoom), zoom).Height;
			rows = Math.Max(1f, (float)Math.Floor(rows));
			return (int)rows;
		}
		public virtual void SetPagePlace(int columns, int rows) {
			if(pc.DocumentIsEmpty || columns == 0) return;
			rows = Math.Max(1 , Math.Min(GetMaxRows(columns), rows));
			columns = Math.Max(1 , Math.Min(Pages.Count, columns));
			FillPagePlace(columns, rows, pc.Zoom);
			SetScrollPosY(fScrollPos.Y);
			SetHorizScroll(fScrollPos.X);
			StartIndex = StartIndex / pagePlaces.Count * pagePlaces.Count;
		}
		protected int GetMaxRows(int columns) {
			return (int)Math.Ceiling((float)Pages.Count / columns);
		}
		protected virtual float GetPageHeight(float maxHeight, int pageIndex) {
			return maxHeight;
		}
		protected PointF GetOuterMarginLocation(int columns, int rows, float zoom) {
			SizeF pagePlaceSize = GetPagePlaceSize(zoom);
			float pageHorizontal = columns * pagePlaceSize.Width - GetInnerMarginWidth(zoom);
			float pageVertical = rows * pagePlaceSize.Height - GetInnerMarginHeight(zoom);
			float outerMarginX = (PSUnitConverter.PixelToDoc(Width, zoom) - pageHorizontal) / 2f;
			outerMarginX = Math.Max(outerMarginX, GetMinLeftMargin(zoom));
			float outerMarginY = (PSUnitConverter.PixelToDoc(Height, zoom) - pageVertical) / 2f;
			outerMarginY = Math.Max(outerMarginY, GetMinTopMargin(zoom));
			return new PointF(outerMarginX, outerMarginY);
		}
		protected virtual float GetMinLeftMargin(float zoom) {
			return pc.ControlInfo.PageHorizontalIndent / 2f + PSUnitConverter.PixelToDoc(Edges.Left, zoom);
		}
		protected virtual float GetMinRightMargin(float zoom) {
			return pc.ControlInfo.PageHorizontalIndent / 2f + PSUnitConverter.PixelToDoc(Edges.Right, zoom);
		}
		protected virtual float GetMinTopMargin(float zoom) {
			return pc.ControlInfo.PageVerticalIndent / 2f + PSUnitConverter.PixelToDoc(Edges.Top, zoom);
		}
		protected virtual float GetMinBottomMargin(float zoom) {
			return pc.ControlInfo.PageVerticalIndent / 2f + PSUnitConverter.PixelToDoc(Edges.Bottom, zoom);
		}
		protected float GetHalfInnerMarginHeight(float edge, float zoom) {
			return pc.ControlInfo.PageVerticalIndent / 2f + PSUnitConverter.PixelToDoc(edge, zoom);
		}
		protected virtual void FillPagePlace(int columns, int rows, float zoom) {
			FillPagePlaceCore(columns, rows, zoom, 0);
		}
		protected void FillPagePlaceCore(int columns, int rows, float zoom, float dy) {
			PointF outerMargin = GetOuterMarginLocation(columns, rows, zoom);
			PointF location = outerMargin;
			placeSize = new SizeF(location.X, location.Y);
			pagePlaces.Clear();
			heights.Clear();
			SizeF maxPageSize = CalcMaxPageSize();
			for(int i = 0; i < rows; i++) {
				float rowHeight = 0;
				for(int j = 0; j < columns; j++) {
					rowHeight = Math.Max(rowHeight, GetPageHeight(maxPageSize.Height, i * columns + j));
					pagePlaces.Add(location);
					placeSize.Width = Math.Max(placeSize.Width, location.X + maxPageSize.Width);
					placeSize.Height = Math.Max(placeSize.Height, location.Y + rowHeight);
					location.X += (maxPageSize.Width + GetInnerMarginWidth(zoom));
				}
				for(int j = 0; j < columns; j++) {
					heights.Add(new SizeF(maxPageSize.Width, rowHeight));
				}
				location.X = outerMargin.X;
				location.Y += rowHeight + GetInnerMarginHeight(zoom) + dy;
			}
			placeSize.Width += GetMinRightMargin(zoom);
			placeSize.Height += GetMinBottomMargin(zoom) + dy;
		}
		protected void InvalidatePage(IPage page) {
			if(page == null) return;
			RectangleF rect = GetPageRect(page);
			SizeF size = PSUnitConverter.PixelToDoc( new SizeF(1,1), pc.Zoom );
			rect.Inflate(size);
			rect = PSUnitConverter.DocToPixel(rect, pc.Zoom, fScrollPos);
			rect = Edges.Inflate(Rectangle.Round(rect));
			viewControl.InvalidateRect(rect, false);
		}
		private Point PointToView(Point screenPoint) {
			return viewControl.PointToClient(screenPoint);
		}
	}
	public class ContinuousViewManager : ViewManager {
		public override int StartIndex { get { return 0; } set { fStartIndex = 0; }
		}
		public ContinuousViewManager(ViewManager viewManager) : base(viewManager) {
		}
		public ContinuousViewManager(DocumentViewerBase pc, ViewControl viewControl) : base (pc, viewControl) {
		}
		public override int GetPageIndex(float verticalScroll) {
			return SelectedPagePlaceIndex;
		}
		public override void SetPagePlace(int columns, int rows) {
			if(pc.DocumentIsEmpty || columns == 0) return;
			rows = Math.Max(1 , Math.Min(GetMaxRows(columns), rows));
			columns = Math.Max(1 , Math.Min(Pages.Count, columns));
			FillPagePlace(columns, GetMaxRows(columns), pc.Zoom);
			if(!pc.DocumentIsCreating || (pc.DocumentIsCreating && !IsVisiblePage(this.SelectedPage)))
			ShowContinuousPage(SelectedPagePlaceIndex, columns, rows, pc.Zoom);
			SetHorizScroll(fScrollPos.X);
		}
		void ShowContinuousPage(int index, int fColumns, int fRows, float zoom) {
			int upperPagePlaceIndex = Math.Max(index - (fRows - 1) / 2 * fColumns, 0);
			if(Places.IsValidIndex(upperPagePlaceIndex)) 
				SetScrollPosY(GetPageRect(upperPagePlaceIndex).Y - GetHalfInnerMarginHeight(Edges.Top, zoom));
		}		
		public override void SetVertScroll(float val) {
			SetContinuousScroll(val);
		}
		public override void ShowPage(IPage page) {
			if(page == null || IsVisiblePage(page) || Places.Count == 0) return;
			ShowContinuousPage(page.Index, pc.ViewColumns, pc.ViewRows, pc.Zoom);
		}
		public override void OffsetVertScroll(float dy, bool canChangePage) {
			if(fScrollPos.Y < MaxScrollY || fScrollPos.Y > 0) {
				SetScrollPosY(fScrollPos.Y + dy);
				SetContinuousScroll(fScrollPos.Y);
			} else 
				base.OffsetVertScroll(dy, canChangePage);
		}
		public override void ValidateVertScroll() {
		}
		protected override float GetPageHeight(float maxHeight, int pageIndex) {
			if(Pages.IsValidIndex(pageIndex))
				return Pages[pageIndex].PageSize.Height;
			return 0;
		}
		protected virtual void SetContinuousScroll(float val) {
			if(Pages.Count < 1) return;
			SetScrollPosY(val);
			if(Places.IsValidIndex(SelectedPagePlaceIndex) && !IsPageInMiddle(SelectedPagePlaceIndex)) {
				int index = GetMiddlePlaceIndex();
				if(index != -1) {
					index += GetColumnIndex(SelectedPagePlaceIndex, pc.ViewColumns);
					SelectedPagePlaceIndex = Math.Min(index, Pages.Count - 1);
				}
			}
			SetScrollPosY(val);
			InvalidatePage(SelectedPage);
			pc.UpdateScrollBars();
		}
		int GetColumnIndex(int index, int columnCount) {
			return index - (index / columnCount) * columnCount;
		}
		bool IsPageInMiddle(int pageIndex) {
			return IsRectInMiddle(GetPageRect(pageIndex));
		}
		bool IsRectInMiddle(RectangleF rect) {
			float height = ((SizeF)PSUnitConverter.PixelToDoc(ClientSize, pc.Zoom)).Height;
			float middle = fScrollPos.Y + height / 2;
			return (rect.Top <= middle && rect.Bottom >= middle);
		}
		int GetMiddlePlaceIndex() {
			for(int i = 0; i < Places.Count; i++)
				if(IsPageInMiddle(i)) return i;
			return -1;
		}
	} 
}
