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
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native.LayoutAdjustment;
namespace DevExpress.XtraPrinting.Native {
	[Flags]
	public enum FillPageResult { 
		None, 
		Fulfil = 1, 
		Overfulfil = 2, 
		Complete = Fulfil | Overfulfil
	}
	public static class FillPageResultExtensions { 
		public static bool IsComplete(this FillPageResult fillPageResult) {
			return (fillPageResult & FillPageResult.Complete) != 0;
		}
		public static bool IsFulfil(this FillPageResult fillPageResult) {
			return (fillPageResult & FillPageResult.Fulfil) != 0;
		}
		public static bool IsOverFulfil(this FillPageResult fillPageResult) {
			return (fillPageResult & FillPageResult.Overfulfil) != 0;
		}
	}
	public class OffsetHelperY {
		double maxOffsetY = double.MinValue;
		double minNegativeOffsetY = double.MaxValue;
		protected virtual bool ShouldUpdateNegativeOffsetY { get { return true; } }
		public void Update(double offsetY, double negativeOffsetY) {
			maxOffsetY = Math.Max(maxOffsetY, offsetY);
			minNegativeOffsetY = Math.Min(minNegativeOffsetY, negativeOffsetY);
		}
		public void Update(PageRowBuilderBase builder) {
			Update(builder.OffsetY, builder.NegativeOffsetY);
		}
		public virtual void UpdateBuilder(PageRowBuilderBase builder) {
			if(maxOffsetY != double.MinValue)
				builder.OffsetY = Math.Max(0, maxOffsetY);
			if(ShouldUpdateNegativeOffsetY && minNegativeOffsetY != double.MaxValue)
				builder.NegativeOffsetY = Math.Min(0, minNegativeOffsetY);
		}
	}
	public class RenderHistory {
		Dictionary<int, Pair<int, int>> rowIndices = new Dictionary<int, Pair<int, int>>();
		HashSet<int> renderedDocumentBands = new HashSet<int>();
		bool reportHeaderRendered;
		bool reportFooterRendered;
		bool pageHeaderRendered;
		bool pageIsUpdated;
		public RenderHistory() {
		}
		public RenderHistory(RenderHistory source) {
			this.pageHeaderRendered = source.PageHeaderRendered;
			this.rowIndices = source.rowIndices;
			this.pageIsUpdated = source.pageIsUpdated;
			this.renderedDocumentBands = source.renderedDocumentBands;
		}
		public bool PageIsUpdated {
			get { return pageIsUpdated; }
			set { pageIsUpdated = value; }
		}
		public bool ReportHeaderRendered {
			get { return reportHeaderRendered; }
			set { reportHeaderRendered = value; }
		}
		public bool ReportFooterRendered {
			get { return reportFooterRendered; }
			set { reportFooterRendered = value; }
		}
		public bool PageHeaderRendered {
			get { return pageHeaderRendered; }
			set { pageHeaderRendered = value; }
		}
		public int GetLastDetailRowIndex(DocumentBand docBand) {
			return GetDetailRowIndexes(docBand).Second;
		}
		public Pair<int, int> GetDetailRowIndexes(DocumentBand docBand) {
			Pair<int, int> value;
			DocumentBand key = docBand.GetDataSourceRoot();
			return key != null && rowIndices.TryGetValue(key.ID, out value) ? value : new Pair<int, int>(-1, -1);
		}
		void CollectRenderedDocumentBands(DocumentBand docBand) {
			if(docBand != null) {
				renderedDocumentBands.Add(docBand.ID);
				CollectRenderedDocumentBands(docBand.Parent);
			}
		}
		public bool IsDocumentBandRendered(DocumentBand docBand) {
			return renderedDocumentBands.Contains(docBand.ID);
		}
		public void UpdateRenderInfo(DocumentBand docBand, ProcessState processState) {
			if(docBand.IsValid && processState == ProcessState.ReportHeader)
				reportHeaderRendered = true;
			if(docBand.IsValid && processState == ProcessState.ReportFooter)
				reportFooterRendered = true;
		}
		public void UpdateDetailBandInfo(DocumentBand docBand) {
			DocumentBand key = docBand.GetDataSourceRoot();
			if(key == null) return;
			if(docBand.IsKindOf(DocumentBandKind.Detail) || docBand.IsDetailBand) {
				Pair<int, int> value;
				if(!rowIndices.TryGetValue(key.ID, out value)) {
					rowIndices[key.ID] = new Pair<int, int>(docBand.RowIndex, docBand.RowIndex);
				} else {
					if(value.First < 0)
						value.First = docBand.RowIndex;
					value.Second = docBand.RowIndex;
				}
			}
			if(key.PrimaryParent != null)
				UpdateDetailBandInfo(key.PrimaryParent);
		}
		public void UpdateDocumentBandsInfo(DocumentBand docBand) {
			CollectRenderedDocumentBands(docBand);
		}
		public void Reset() {
			reportHeaderRendered = false;
			reportFooterRendered = false;
			pageHeaderRendered = false;
			pageIsUpdated = false;
			foreach(Pair<int, int> pair in rowIndices.Values)
				pair.First = -1;
			renderedDocumentBands.Clear();
		}
	}
	public abstract class PageRowBuilderBase {
		#region inner classes
		protected class FriendsHelper {
			public static DocumentBand CollectFriends(DocumentBand docBand) {
				DocumentBand resultBand = new ServiceDocumentBand(DocumentBandKind.Storage);
				if(docBand.IsKindOf(DocumentBandKind.Detail)) {
					resultBand.Bands.Add(docBand);
					docBand.GenerateBandChildren();
					new FriendsHelper().CollectFriends(docBand.FriendLevel, docBand, resultBand);
				} else
					new HeaderFriendsHelper().CollectFriends(docBand.FriendLevel, docBand.Parent, resultBand);
				return resultBand;
			}
			protected virtual void CollectFriends(int iterationCount, DocumentBand docBand, DocumentBand resultBand) {
				CollectBands(iterationCount, docBand, resultBand, lastBand => !lastBand.IsPageBand(DocumentBandKind.Footer));
			}
			protected void CollectBands(int iterationCount, DocumentBand docBand, DocumentBand resultBand, Predicate<DocumentBand> callback) {
				if(iterationCount < 0)
					return;
				docBand.Parent.EnsureGroupFooter();
				DocumentBand lastBand = docBand.Parent.Bands.GetLast<DocumentBand>();
				if(callback(lastBand)) {
					lastBand.GenerateBandChildren();
					DocumentBand footer = lastBand.CopyBand(docBand.RowIndex);
					resultBand.Bands.Add(footer);
				}
				CollectBands(--iterationCount, docBand.Parent, resultBand, callback);
			}
		}
		class HeaderFriendsHelper : FriendsHelper {
			protected override void CollectFriends(int iterationCount, DocumentBand docBand, DocumentBand resultBand) {
				if(iterationCount < 0)
					return;
				if(docBand.KeepTogether) {
					resultBand.Bands.Add(docBand);
					return;
				}
				resultBand.Bands.Add(docBand.GetBand(0));
				DocumentBand nextBand = docBand.GetBand(1);
				if(iterationCount == 0) {
					resultBand.Bands.Add(nextBand.KeepTogether ? nextBand : nextBand.GetDetailBand());
					if(nextBand.FriendLevel != DocumentBand.UndefinedFriendLevel)
						CollectBands(nextBand.FriendLevel, nextBand, resultBand, lastBand => true);
				} else
					CollectFriends(--iterationCount, nextBand, resultBand);
			}
		} 
		protected delegate bool ProcessBandsDelegate(DocumentBand rootBand, PageBuildInfo pageBuildInfo);
		protected class OffsetHelperXY : OffsetHelperY {
			float offsetX;
			public OffsetHelperXY(float offsetX) {
				this.offsetX = offsetX;
			}
			public override void UpdateBuilder(PageRowBuilderBase builder) {
				base.UpdateBuilder(builder);
				builder.offset.X = this.offsetX;
			}
		}
		#endregion
		#region static
		protected static FillPageResult GetFillResult(RectangleF bounds, float bandHeight, double offsetY) {
			float bottom = (float)(bounds.Y + offsetY + bandHeight);
			if(PageSizeAccuracyComaprer.Instance.FirstEqualsSecond(bottom, bounds.Bottom))
				return FillPageResult.Fulfil;
			if(bottom > bounds.Bottom)
				return FillPageResult.Overfulfil;
			return FillPageResult.None;
		}
		static float GetPageRelativeY(float pageOffset, float value, float offsetY) {
			return offsetY + value + pageOffset;
		}
		static bool CanProcessReportFooter(DocumentBand rootBand, PageBuildInfo bi) {
			return CanProcessReportFooterCore(rootBand, bi.Index);
		}
		static bool CanProcessReportFooterCore(DocumentBand rootBand, int index) {
			rootBand.EnsureReportFooterBand();
			if(index < rootBand.Bands.Count) {
				DocumentBand docBand = rootBand.Bands[index];
				return docBand.IsKindOf(DocumentBandKind.ReportFooter, DocumentBandKind.PageFooter, DocumentBandKind.BottomMargin, DocumentBandKind.PageBreak);
			}
			return false;
		}
		static bool CanProceedInternal(DocumentBand rootBand, ProcessBandsDelegate process, PageBuildInfo pageBuildInfo) {
			int startIndex = pageBuildInfo.Index;
			while(rootBand.GetBand(startIndex) != null) {
				if(rootBand.Bands[startIndex].IsValid)
					return process(rootBand, pageBuildInfo);
				startIndex++;
			}
			return false;
		}
#if DEBUGTEST
		public static bool Test_CanProceedInternal(DocumentBand rootBand, Func<DocumentBand, PageBuildInfo, bool> process, PageBuildInfo pageBuildInfo) {
			return CanProceedInternal(rootBand, new ProcessBandsDelegate(process), pageBuildInfo);
		}		
#endif
		#endregion
		bool forceSplit;
		RectangleF pageBreakRect = RectangleF.Empty;
		CustomPageData nextPageData;
		BuildInfoContainer buildInfoContainer;
		PointD offset;
		double negativeOffsetY;
		bool canApplyPageBreaks = true;
		RenderHistory fRenderHistory;
		ProcessState fProcessState = ProcessState.None;
		protected virtual bool Stopped { 
			get { return false; } 
		}
		protected PageRowBuilderBase() {
			this.fRenderHistory = new RenderHistory();
		}
		protected virtual bool CanApplyPageBreak {
			get { return true; }
		}
		protected RectangleF PageBreakRect {
			get { return pageBreakRect; }
			set { pageBreakRect = value; }
		}
		public CustomPageData NextPageData {
			get { return nextPageData; }
			set { nextPageData = value; }
		}
		protected BuildInfoContainer BuildInfoContainer {
			get {
				if(buildInfoContainer == null)
					buildInfoContainer = new BuildInfoContainer();
				return buildInfoContainer;  
			}
		}
		public Dictionary<int, float> NegativeOffsets {
			get {
				return BuildInfoContainer.NegativeOffsets;
			}
		}
		public virtual bool CanApplyPageBreaks {
			get {
				return canApplyPageBreaks;
			}
			set {
				canApplyPageBreaks = value;
			}
		}
		public ProcessState ProcessState {
			get { return fProcessState; }
			protected set { fProcessState = value; }
		}
		public RenderHistory RenderHistory {
			get { return fRenderHistory; }
		}
		public double FullOffsetY {
			get {
				if(negativeOffsetY < 0)
					return negativeOffsetY;
				return offset.Y;
			}
			set {
				if(value < 0) {
					negativeOffsetY = value;
				}
				else {
					offset.Y = value;
					negativeOffsetY = 0;
				}
			}
		}
		public virtual int PageBricksCount {
			get { return 0; }
		}
		public double NegativeOffsetY {
			get { return negativeOffsetY; }
			set { negativeOffsetY = value; }
		}
		public bool ForceSplit { 
			get { return forceSplit; }
			set { forceSplit = value; }
		}
		public PointD Offset { get { return offset; } }
		public double OffsetX {
			get { return offset.X; }
			set { offset = new PointD(value, offset.Y); }
		}
		public double OffsetY {
			get { return offset.Y; }
			set { offset = new PointD(offset.X, value); }
		}
		public virtual bool PrintOnPages(DocumentBand docBand, DevExpress.XtraReports.UI.PrintOnPages printOnPages) {
			return docBand != null && (docBand.PrintOn & printOnPages) > 0;
		}
		public virtual void CopyFrom(PageRowBuilderBase source) {
			this.offset = source.offset;
			this.negativeOffsetY = source.negativeOffsetY;
			this.pageBreakRect = source.pageBreakRect;
			this.nextPageData = source.nextPageData;
			this.buildInfoContainer = source.buildInfoContainer;
			this.fRenderHistory = new RenderHistory(source.fRenderHistory) { ReportHeaderRendered = RenderHistory.ReportHeaderRendered, ReportFooterRendered = RenderHistory.ReportFooterRendered };
			this.canApplyPageBreaks = source.canApplyPageBreaks;
			this.forceSplit = source.forceSplit;
			this.fProcessState = source.fProcessState;
		}
		public FillPageResult FillPage(DocumentBand rootBand, RectangleF bounds) {
			this.fProcessState = BuildInfoContainer.GetProcessState(rootBand);
			if(rootBand.HasReportHeader()) {
				FillPageResult fillPageResult = FillReportHeader(rootBand, bounds);
				if(fillPageResult.IsComplete()) {
					FillReportDetailsAndFooter(rootBand, bounds);
					return FillPageResult.Overfulfil;
				}
			}
			return FillReportDetailsAndFooter(rootBand, bounds);
		}
		protected virtual FillPageResult FillPageForBands(DocumentBand rootBand, RectangleF bounds, ProcessBandsDelegate process) {
			int bi = BuildInfoContainer.GetBuildInfo(rootBand);
			while(true) {
				RectangleF newBounds = GetCorrectedBounds(rootBand, bounds);
				if(!process(rootBand, new PageBuildInfo(bi, newBounds, offset.ToPointF())))
					break;
				FillPageResult fillPageResult = FillPageForBand(rootBand, bounds, newBounds);
				if(fillPageResult == FillPageResult.Overfulfil)
					return FillPageResult.Overfulfil;
				IncreaseBuildInfo(rootBand, bi, 1);
				bi++;
				if(fillPageResult == FillPageResult.None)
					continue;
				bool canProceed = CanProceedInternal(rootBand, process, new PageBuildInfo(bi, bounds, offset.ToPointF()));
				if(this.ProcessState != ProcessState.ReportDetails)
					return GetFillResult(rootBand, bi, canProceed);
				if(canProceed && fillPageResult == FillPageResult.Fulfil) {
					if(rootBand.Bands[bi].IsKindOf(DocumentBandKind.PageBreak))
						continue;
					return FillPageResult.Overfulfil;
				}
				canProceed |= CanProcessReportFooterCore(rootBand, bi);
				if(!canProceed)
					return GetFillResult(rootBand, bi, canProceed);
				DocumentBand docBand = rootBand.Bands[bi];
				if(docBand.IsEmpty) {
					IncreaseBuildInfo(rootBand, bi, 1);
					bi++;
					return GetFillResult(rootBand, bi, false);
				}
				if(newBounds != bounds && fillPageResult == FillPageResult.Fulfil)
					continue;
				return FillPageResult.Overfulfil;
			}
			return FillPageResult.None;
		}
		protected bool AreBoundsComplete(RectangleF bounds, float bandHeight, float offsetY) {
			FillPageResult result = GetFillResult(bounds, bandHeight, offsetY);
			return result.IsComplete() && !ShouldSplitContent(bounds, bandHeight, offsetY);
		}
		protected virtual bool ShouldSplitContent(RectangleF bounds, float bandHeight, float offsetY) {
			return bandHeight >= bounds.Height;
		}
		FillPageResult GetFillResult(DocumentBand rootBand, int index, bool canProceed) {
			if(!canProceed && rootBand.ProcessIsFinished(this.ProcessState, index))
				return FillPageResult.Fulfil;
			return FillPageResult.Overfulfil;
		}
		protected virtual RectangleF GetCorrectedBounds(DocumentBand rootBand, RectangleF bounds) {
			return bounds;
		}
		protected virtual void CorrectPrintAtBottomBricks(List<BandBricksPair> docBands, float pageBottom) {
		}
		protected virtual FillPageResult FillReportDetailsAndFooter(DocumentBand rootBand, RectangleF bounds) {
			FillPageResult fillPageResult = FillReportDetails(rootBand, bounds);
			if(fillPageResult.IsComplete())
				return fillPageResult;
			return FillReportFooter(rootBand, bounds);
		}
		protected FillPageResult FillReportFooter(DocumentBand rootBand, RectangleF bounds) {
			ProcessState savedProcessState = fProcessState;
			fProcessState = ProcessState.ReportFooter;
			BuildInfoContainer.SetProcessState(rootBand, fProcessState);
			try {
				return FillPageForBands(rootBand, bounds, CanProcessReportFooter);
			} finally {
				fProcessState = savedProcessState;
				BuildInfoContainer.SetProcessState(rootBand, fProcessState);
			}
		}
		FillPageResult FillReportHeader(DocumentBand rootBand, RectangleF bounds) {
			ProcessState savedProcessState = fProcessState;
			fProcessState = ProcessState.ReportHeader;
			BuildInfoContainer.SetProcessState(rootBand, fProcessState);
			try {
				return FillPageForBands(rootBand, bounds, CanProcessReportHeader);
			}
			finally {
				fProcessState = savedProcessState;
				BuildInfoContainer.SetProcessState(rootBand, fProcessState);
			}
		}
		public bool CanFillFooterBand(DocumentBand footerBand) {
			if(footerBand == null)
				return false;
			if(PrintOnPages(footerBand, DevExpress.XtraReports.UI.PrintOnPages.NotWithReportFooter) && RenderHistory.ReportFooterRendered)
				return false;
			if(PrintOnPages(footerBand, DevExpress.XtraReports.UI.PrintOnPages.NotWithReportHeader) && RenderHistory.ReportHeaderRendered)
				return false;
			return true;
		}
		protected virtual bool CanProcessDetail(DocumentBand rootBand, PageBuildInfo pageBuildInfo) {
			DocumentBand docBand = rootBand.GetBand(pageBuildInfo);
			return docBand != null && !docBand.IsKindOf(DocumentBandKind.TopMargin, DocumentBandKind.BottomMargin, DocumentBandKind.ReportHeader, DocumentBandKind.ReportFooter);
		}
		bool CanProcessReportHeader(DocumentBand rootBand, PageBuildInfo pageBuildInfo) {
			if(pageBuildInfo.Index < rootBand.Bands.Count) {
				DocumentBand docBand = rootBand.Bands[pageBuildInfo.Index];
				return docBand.IsKindOf(DocumentBandKind.TopMargin, DocumentBandKind.ReportHeader, DocumentBandKind.PageBreak);
			}
			return false;
		}
		public virtual FillPageResult FillPageForBand(DocumentBand rootBand, RectangleF bounds, RectangleF newBounds) {
			int bi = BuildInfoContainer.GetBuildInfo(rootBand);
			DocumentBand docBand = rootBand.Bands[bi];
			FillPageResult result = FillPageForBandCore(rootBand, bounds, newBounds);
			AfterDocumentBandFill(docBand);
			return result;
		}
		FillPageResult FillPageForBandCore(DocumentBand rootBand, RectangleF bounds, RectangleF newBounds) {
			int bi = BuildInfoContainer.GetBuildInfo(rootBand);
			DocumentBand docBand = rootBand.Bands[bi];
			if(docBand.HasBands(newBounds, offset.ToPointF())) {
				if(!CanFillPageRecursive(docBand))
					return FillPageResult.None;
				if(docBand.TopSpanActive)
					FullOffsetY += docBand.TopSpan;
				List<DocumentBand> sideBySideBands = GetSideBySideDocumentBands(rootBand.Bands, bi);
				if(sideBySideBands.Count > 0) {
					FillPageResult fillPageResult = FillPageSideBySide(rootBand, sideBySideBands, bounds);
					if(fillPageResult.IsComplete())
						return FillPageResult.Overfulfil;
					IncreaseBuildInfo(rootBand, bi, sideBySideBands.Count - 1);
					bi += sideBySideBands.Count - 1;
				} else if(!IsBuildCompleted(docBand))
					return FillPageRecursive(rootBand, docBand, bounds);
			} else if(CanFillPageWithBricksInternal(docBand)) {
				bool storeForceSplit = this.forceSplit;
				if((fProcessState == ProcessState.ReportDetails || fProcessState == ProcessState.ReportFooter) && newBounds.Height == PrintingDocument.MinPageSize.Height)
					this.forceSplit = true;
				FillPageResult fillPageResult = FillPageWithBricks(docBand, ValidateBounds(rootBand, bounds, newBounds));
				this.forceSplit = storeForceSplit;
				return fillPageResult;
			}
			return FillPageResult.None;
		}
		protected virtual RectangleF ValidateBounds(DocumentBand rootBand, RectangleF bounds, RectangleF newBounds) {
			return bounds;
		}
		protected bool CanFillPageWithBricksInternal(DocumentBand docBand) {
			if(docBand.IsValid) {
				if(docBand.IsKindOf(DocumentBandKind.PageBreak) && !CanApplyPageBreak)
					return false;
				return CanFillPageWithBricks(docBand);
			}
			return false;
		}
		protected virtual bool AreFriendsTogetherRecursive(DocumentBand docBand, RectangleF bounds) {
			return true;
		}
		protected virtual void ApplyBottomSpan(float bottomSpan, RectangleF bounds) {
			offset.Y = Math.Min(offset.Y + bottomSpan, bounds.Height);
		}
		protected virtual void AfterDocumentBandFill(DocumentBand docBand) {
		}
		protected internal virtual FillPageResult FillPageRecursive(DocumentBand rootBand, DocumentBand docBand, RectangleF bounds) {
			if(!AreFriendsTogetherRecursive(docBand, bounds))
				return FillPageResult.Overfulfil;
			PageRowBuilderBase builder = CreateInternalPageRowBuilder();
			this.offset.X += docBand.OffsetX;
			builder.CopyFrom(this);
			int pageBricksCount = PageBricksCount;
			FillPageResult fillPageResult = builder.FillPage(docBand, bounds);
			if(PageBricksCount > pageBricksCount) {
				if(ProcessState == ProcessState.ReportHeader)
					RenderHistory.ReportHeaderRendered = true;
				if(ProcessState == ProcessState.ReportFooter)
					RenderHistory.ReportFooterRendered = true;
			}
			float dy = (float)((builder.FullOffsetY - this.FullOffsetY));
			this.CopyFrom(builder);
			if(this.FullOffsetY > 0 && dy < docBand.MinSelfHeight)
				if(dy > 0)
					this.FullOffsetY += docBand.MinSelfHeight - dy;
				else if(this.FullOffsetY < docBand.MinSelfHeight)
					this.FullOffsetY = docBand.MinSelfHeight;
			this.offset.X -= docBand.OffsetX;
			if(!(fillPageResult.IsComplete()))
				ApplyBottomSpan(docBand.BottomSpan, bounds);
			BuildInfoContainer.SetProcessState(docBand, builder.ProcessState);
			return fillPageResult;
		}
		protected internal abstract PageRowBuilderBase CreateInternalPageRowBuilder();
		protected virtual bool CanFillPageWithBricks(DocumentBand docBand) {
			return !docBand.IsKindOf(DocumentBandKind.TopMargin, DocumentBandKind.BottomMargin);
		}
		protected virtual bool CanFillPageRecursive(DocumentBand docBand) {
			return CanFillPageWithBricks(docBand);
		}
		List<DocumentBand> GetSideBySideDocumentBands(IListWrapper<DocumentBand> bands, int index) {
			List<DocumentBand> result = new List<DocumentBand>();
			if(!IsSubreport(bands[index]))
				return result;
			if(IsBuildCompleted(bands[index]))
				return result;
			result.Add(bands[index]);
			RectangleF baseRect = GetRectangle((ISubreportDocumentBand)bands[index]);
			for(int i = index; i < bands.Count - 1; i++) {
				if(!IsSubreport(bands[i]) || !IsSubreport(bands[i + 1]))
					break;
				RectangleF rect = GetRectangle((ISubreportDocumentBand)bands[i + 1]);
				if(!PSNativeMethods.ValueInsideBounds(rect.Top, baseRect.Top, baseRect.Bottom))
					break;
				baseRect = RectangleF.Union(baseRect, rect);
				if(!IsBuildCompleted(bands[i + 1]))
					result.Add(bands[i + 1]);
			}
			if(result.Count == 1)
				result.Clear();
			return result;
		}
		static RectangleF GetRectangle(ISubreportDocumentBand docBand) {
			return docBand.ReportRect;
		}
		static bool IsSubreport(DocumentBand docBand) {
			return docBand is ISubreportDocumentBand && docBand.Bands.Count > 0;
		}
		protected virtual void ResetTopSpan(FillPageResult fillPageResult, DocumentBand docBand) {
			if(docBand == null)
				return;
			docBand.TopSpanActive = false;
			ResetTopSpan(fillPageResult, docBand.Parent);
		}
		public virtual FillPageResult FillPageWithBricks(DocumentBand docBand, RectangleF bounds) {
			if(!docBand.IsValid)
				return FillPageResult.None;
			if(!docBand.IsKindOf(DocumentBandKind.PageHeader, DocumentBandKind.PageFooter)) {
				if(pageBreakRect == RectangleF.Empty) {
					float pageBottom = ApplyPageBreaksInternal(docBand, bounds, (float)FullOffsetY);
					if(SingleComparer.AreNotEqual(pageBottom, bounds.Bottom)) {
						bounds.Height = pageBottom - bounds.Top;
						pageBreakRect = bounds;
					}
				} else if(pageBreakRect.Bottom < bounds.Bottom)
					bounds.Height = Math.Max(0, pageBreakRect.Bottom - bounds.Top);
			}
			if(!AreFriendsTogether(docBand, bounds)) {
				return FillPageResult.Overfulfil;
			}
			if(Stopped || ShouldOverFulfil(docBand, bounds)) {
				return FillPageResult.Overfulfil;
			}
			PageUpdateData pageUpdateData = UpdatePageContent(docBand, bounds);
			if(pageUpdateData.IsUpdated) {
				fRenderHistory.PageIsUpdated = true;
				fRenderHistory.UpdateRenderInfo(docBand, ProcessState);
				fRenderHistory.UpdateDetailBandInfo(docBand);
				fRenderHistory.UpdateDocumentBandsInfo(docBand);
			}
			RectangleF updateBounds = pageUpdateData.Bounds;
			float selfHeightWithoutBottomSpan = docBand.SelfHeight - docBand.BottomSpan;
			FillPageResult fillPageResult = GetFillResult(updateBounds, selfHeightWithoutBottomSpan, FullOffsetY);
			if(pageUpdateData.IsUpdated)
				ResetTopSpan(fillPageResult, docBand);
			if(fillPageResult == FillPageResult.Fulfil) {
				offset.Y = updateBounds.Height;
				negativeOffsetY = 0;
			} else if(fillPageResult == FillPageResult.Overfulfil) {
				double y = FullOffsetY + updateBounds.Y;
				negativeOffsetY = FloatsComparer.Default.FirstGreaterSecond(FullOffsetY, updateBounds.Bottom)
					? updateBounds.Bottom - FullOffsetY
					: (y > updateBounds.Bottom ? 0f : y - updateBounds.Bottom);
				offset.Y = updateBounds.Height;
			} else if(FullOffsetY + docBand.SelfHeight > updateBounds.Height) {
				offset.Y = updateBounds.Height;
				negativeOffsetY = 0;
				fillPageResult = FillPageResult.Fulfil;
			} else {
				double y = FullOffsetY + docBand.SelfHeight;
				FullOffsetY = y >= 0 ? y : 0;
			}
			return fillPageResult;
		}
		protected virtual bool ShouldOverFulfil(DocumentBand docBand, RectangleF bounds) {
			float selfHeightWithoutBottomSpan = docBand.SelfHeight - docBand.BottomSpan;
			return docBand.KeepTogether && (GetFillResult(bounds, selfHeightWithoutBottomSpan, FullOffsetY) == FillPageResult.Overfulfil);
		}
		float ApplyPageBreaksInternal(DocumentBand docBand, RectangleF bounds, float offsetY) {
			if(CanApplyPageBreaks) {
				for (int i = 0; i < docBand.PageBreaks.Count; i++) {
					PageBreakInfo pageBreak = docBand.PageBreaks[i];
					if (!pageBreak.Active)
						continue;
					float y = GetPageRelativeY(bounds.Y, pageBreak.Value, offsetY);
					if (y >= bounds.Top && y < bounds.Bottom) {
						pageBreak.Active = false;
						nextPageData = pageBreak.NextPageData;
						return y;
					}
				}
			}
			return bounds.Bottom;
		}
		protected virtual bool AreFriendsTogether(DocumentBand docBand, RectangleF bounds) {
			return true;
		}
		protected abstract PageUpdateData UpdatePageContent(DocumentBand docBand, RectangleF bounds);
		FillPageResult FillPageSideBySide(DocumentBand rootBand, List<DocumentBand> bands, RectangleF bounds) {
			List<ILayoutData> layoutData = new List<ILayoutData>();
			float minTop = float.MaxValue;
			foreach (ISubreportDocumentBand documentBand in bands)
				minTop = Math.Min(minTop, documentBand.ReportRect.Top);
			LayoutDataContext context = new LayoutDataContext(this, rootBand, bounds);
			foreach (ISubreportDocumentBand documentBand in bands) {
				RectangleF rect = documentBand.ReportRect;
				rect.Y -= minTop;
				layoutData.Add(documentBand.CreateLayoutData(context, rect));
			}
			LayoutAdjuster layoutAdjuster = CreateLayoutAdjuster();
			List<ILayoutData> layoutData2 = new List<ILayoutData>(new ILayoutData[] { new LayoutDataContainer(layoutData, rootBand.BottomSpan) });
			layoutAdjuster.Process(layoutData2);
			foreach(ILayoutData item in layoutData) {
				if(item is SubreportDocumentBandLayoutData)
					((SubreportDocumentBandLayoutData)item).FillPage();
			}
			context.Commit();
			foreach(List<BandBricksPair> docBands in context.BandList)
				CorrectPrintAtBottomBricks(docBands, (float)offset.Y);
			return context.Result;
		}
		protected virtual LayoutAdjuster CreateLayoutAdjuster() {
			return new LayoutAdjusterWithAnchoring(300);
		}
		public virtual List<BandBricksPair> GetAddedBands() {
			return null;
		}
		public Pair<int, int> GetDetailRowIndexes(DocumentBand docBand) {
			return fRenderHistory.GetDetailRowIndexes(docBand);
		}
		public bool IsBuildCompleted(DocumentBand rootBand) {
			int bi = BuildInfoContainer.GetBuildInfo(rootBand);
			rootBand.GetBand(bi);
			return bi >= rootBand.Bands.Count;
		}
		protected virtual void IncreaseBuildInfo(DocumentBand rootBand, int bi, int value) {
			BuildInfoContainer.SetBuildInfo(rootBand, bi + value);
		}
		protected FillPageResult FillReportDetails(DocumentBand rootBand, RectangleF bounds) {
			fProcessState = ProcessState.ReportDetails;
			BuildInfoContainer.SetProcessState(rootBand, fProcessState);
			MultiColumn mc = rootBand.MultiColumn;
			if(mc != null && mc.Order == ColumnLayout.DownThenAcross)
				return BuildNOrderMultiColumn(rootBand, bounds, mc);
			if(rootBand.ContainsDetailBands()) {
				if(mc != null && mc.Order == ColumnLayout.AcrossThenDown)
					return BuildZOrderMultiColumn(rootBand, bounds, mc);
			}
			return FillPageForBands(rootBand, bounds, CanProcessDetail);
		}
		protected FillPageResult BuildZOrderMultiColumn(DocumentBand rootBand, RectangleF bounds, MultiColumn mc) {
			FillPageResult fillResult = FillPageForBands(rootBand, bounds, CanProcessHeader);
			if(fillResult.IsComplete())
				return FillPageResult.Overfulfil;
			fillResult = BuildZOrderMultiColumnInternal(rootBand, bounds, mc);
			fillResult |= FillPageForBands(rootBand, bounds, CanProcessFooter);
			return fillResult;
		}
		protected virtual FillPageResult BuildNOrderMultiColumn(DocumentBand rootBand, RectangleF bounds, MultiColumn mc) {
			return FillPageResult.None;
		}
		protected virtual FillPageResult BuildZOrderMultiColumnInternal(DocumentBand rootBand, RectangleF bounds, MultiColumn mc) {
			return FillPageResult.None;
		}
		bool CanProcessHeader(DocumentBand rootBand, PageBuildInfo pageBuildInfo) {
			if(pageBuildInfo.Index < rootBand.Bands.Count) {
				DocumentBand docBand = rootBand.Bands[pageBuildInfo.Index];
				return docBand.IsKindOf(DocumentBandKind.Header);
			}
			return false;
		}
		bool CanProcessFooter(DocumentBand rootBand, PageBuildInfo pageBuildInfo) {
			if(pageBuildInfo.Index < rootBand.Bands.Count) {
				DocumentBand docBand = rootBand.Bands[pageBuildInfo.Index];
				return docBand.IsKindOf(DocumentBandKind.Footer) && !docBand.IsPageBand(DocumentBandKind.Footer);
			}
			return false;
		}
	}
}
