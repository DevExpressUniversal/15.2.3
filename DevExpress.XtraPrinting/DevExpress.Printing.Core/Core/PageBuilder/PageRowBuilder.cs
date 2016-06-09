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
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Preview;
namespace DevExpress.XtraPrinting.Native {
	public class PageRowBuilder : PageRowBuilderBase {
		#region static
		static float GetPageRelativeY(float pageOffset, float value, float offsetY) {
			return offsetY + value + pageOffset;
		}
		#endregion
		int markedBandID;
		YPageContentEngine pageContentEngine = null;
		protected override bool Stopped {
			get { return PageContentEngine != null && PageContentEngine.Stopped.Value; }
		}
		protected override bool CanApplyPageBreak {
			get {
				foreach(BandBricksPair item in PageContentEngine.AddedBands) {
					if(!item.Band.IsKindOf(DocumentBandKind.PageHeader, DocumentBandKind.PageHeader, DocumentBandKind.TopMargin, DocumentBandKind.BottomMargin))
						return true;
				}
				return false;
			}
		}
		protected YPageContentEngine PageContentEngine { 
			get { return pageContentEngine; } 
		}
		public override int PageBricksCount {
			get { return PageContentEngine.PageBricksCount; }
		}
		protected PrintingSystemBase PrintingSystem {
			get { return PageContentEngine.PrintingSystem; }
		}
		public PageRowBuilder(YPageContentEngine pageContentEngine)
			: base() {
			this.pageContentEngine = pageContentEngine;
			MarkLastAddedPage();
		}
		protected override void AfterDocumentBandFill(DocumentBand docBand) {
			if(RenderHistory.IsDocumentBandRendered(docBand)) {
				PrintingSystem.PerformIfNotNull<GroupingManager>(groupingManager => groupingManager.AfterBandPrinted(PageContentEngine.Page.ID, docBand));
				if(docBand.Bricks.Count > 0)
					PrintingSystem.OnAfterBandPrint(new PageEventArgs(PageContentEngine.Page, new DocumentBand[] { docBand }));
			}
		}
		protected override void IncreaseBuildInfo(DocumentBand rootBand,  int bi, int value) {
			if(PageContentEngine != null) {
				for(int i = bi; i < bi + value; i++)
					PageContentEngine.OnBuildDocumentBand(rootBand.Bands[i]);
				PageContentEngine.BuildInfoIncreased.Value = true;
				if(PrintingSystem.PrintingDocument.Root.Completed)
					PrintingSystem.ProgressReflector.IncrementRangeValue(value);
			}
			base.IncreaseBuildInfo(rootBand, bi, value);
		}
		protected override bool ShouldOverFulfil(DocumentBand docBand, RectangleF bounds) {
			return ShouldOverFulfilCore(docBand, bounds, new BandContentAnalyzer(PageContentEngine.AddedBands));
		}
		protected bool ShouldOverFulfilCore(DocumentBand docBand, RectangleF bounds, BandContentAnalyzer analyzer) {
			return base.ShouldOverFulfil(docBand, bounds) && analyzer.ExistsPrimaryContentAbove((float)(Offset.Y + bounds.Y));
		}
		protected override void CorrectPrintAtBottomBricks(List<BandBricksPair> docBands, float pageBottom) {
			PageContentEngine.CorrectPrintAtBottomBricks(docBands, pageBottom, false);
		}
		protected void MarkLastAddedPage() {
			if(PageContentEngine != null)
				markedBandID = PageContentEngine.LastAddedBand.GetID();
		}
		public override List<BandBricksPair> GetAddedBands() {
			return PageContentEngine.GetAddedBands(markedBandID);
		}
		protected override bool ShouldSplitContent(RectangleF bounds, float bandHeight, float offsetY) {
			return !PageSizeAccuracyComaprer.Instance.FirstLessSecond(bandHeight, PageContentEngine.BuildInfoIncreased.Value ? bounds.Height : bounds.Height - offsetY);
		}
		public void BeforeFillPage(YPageContentEngine pageContentEngine) {
			this.pageContentEngine = pageContentEngine;
			OffsetY = 0f;
			PageBreakRect = RectangleF.Empty;
			RenderHistory.Reset();
		}
		protected override bool AreFriendsTogetherRecursive(DocumentBand docBand, RectangleF bounds) {
			if(docBand.KeepTogether || docBand.KeepTogetherOnTheWholePage) {
				float bandHeight = BuildHelper.GetDocBandHeight(docBand, bounds, false);
				if(AreBoundsComplete(bounds, bandHeight, (float)Offset.Y)) {
					NegativeOffsetY = 0;
					return false;
				}
			}
			if(docBand.IsFriendLevelSet) {
				DocumentBand friendsContainer = FriendsHelper.CollectFriends(docBand);
				float bandHeight = BuildHelper.GetDocBandHeight(friendsContainer, bounds, true);
				if(AreBoundsComplete(bounds, bandHeight, (float)Offset.Y)) {
					NegativeOffsetY = 0;
					return false;
				}
			}
			return true;
		}
		protected internal override PageRowBuilderBase CreateInternalPageRowBuilder() {
			return new PageRowBuilder(this.PageContentEngine);
		}
		protected override bool AreFriendsTogether(DocumentBand docBand, RectangleF bounds) {
			if(docBand.IsFriendLevelSet) {
				DocumentBand band = FriendsHelper.CollectFriends(docBand);
				float height = BuildHelper.GetDocBandHeight(band, bounds, true);
				return !AreBoundsComplete(bounds, height, (float)Offset.Y);
			}
			return true;
		}
		protected override PageUpdateData UpdatePageContent(DocumentBand docBand, RectangleF bounds) {
			PointD offset = Offset;
			offset.Y = FullOffsetY;
			return PageContentEngine.UpdateContent(docBand, offset, bounds, ForceSplit);
		}
		protected override FillPageResult BuildZOrderMultiColumnInternal(DocumentBand rootBand, RectangleF bounds, MultiColumn mc) {
			ZOderMultiColumnBuilder builder = new ZOderMultiColumnBuilder(PageContentEngine);
			builder.CopyFrom(this);
			try {
				return builder.BuildZOrderMultiColumn(rootBand, mc, bounds);
			} finally {
				this.CopyFrom(builder);
			}
		}
		protected override FillPageResult BuildNOrderMultiColumn(DocumentBand rootBand, RectangleF bounds, MultiColumn mc) {
			NOderMultiColumnBuilder builder = new NOderMultiColumnBuilder(PageContentEngine);
			builder.CopyFrom(this);
			FillPageResult fillPageResult = builder.BuildNOrderMultiColumn(rootBand, mc, bounds);
			this.CopyFrom(builder);
			return fillPageResult;
		}
		public void FillPageWithBricks(DocumentBand rootBand, DocumentBand docBand, RectangleF bounds) {
			FillPageWithBricks(docBand, bounds);
			AfterDocumentBandFill(docBand);
		}
	}
	public class BuildHelper {
		static RectangleF DecreaseRectBottom(RectangleF rect, float delta, float minHeight) {
			float height = minHeight < rect.Height ? Math.Max(minHeight, rect.Height - delta) : rect.Height - delta;
			return new RectangleF(rect.X, rect.Y, rect.Width, height);
		}
		static bool ShouldCorrectBoundsByPageFooter(DocumentBand pageFooterBand, PageRowBuilderBase pageRowBuilderBase) {
			if(pageFooterBand == null)
				return false;
			if(pageRowBuilderBase.PrintOnPages(pageFooterBand, DevExpress.XtraReports.UI.PrintOnPages.NotWithReportHeader) && pageRowBuilderBase.ProcessState == ProcessState.ReportHeader)
				return false;
			if(pageRowBuilderBase.PrintOnPages(pageFooterBand, DevExpress.XtraReports.UI.PrintOnPages.NotWithReportFooter) && pageRowBuilderBase.ProcessState == ProcessState.ReportFooter)
				return false;
			return true;
		}
		public static RectangleF CorrectBoundsFooter(DocumentBand rootBand, int rowIndex, RectangleF bounds, PageRowBuilder builder) {
			DocumentBand footerBand = GetFooterBand(rootBand);
			if(builder.CanFillFooterBand(footerBand) && ShouldCorrectBoundsByPageFooter(footerBand, builder)) {
				DocumentBand pageFooter = footerBand.CopyBand(rowIndex);
				float clonedPageFooterHeight = GetDocBandHeight(pageFooter, bounds, true);
				bounds = DecreaseRectBottom(bounds, clonedPageFooterHeight, (float)builder.Offset.Y);
				bounds.Height = Math.Max(bounds.Height, PrintingDocument.MinPageSize.Height);
			}
			return bounds;
		}
		public static DocumentBand GetFooterBand(DocumentBand rootBand) {
			return rootBand != null ? rootBand.GetPageFooterBand() : null;
		}
		public static float GetDocBandHeight(DocumentBand docBand, RectangleF bounds, bool includeBottomSpans) {
			DocumentBand rootBand = null;
			if(docBand.Bricks.Count > 0) {
				rootBand = new ServiceDocumentBand(DocumentBandKind.Storage);
				rootBand.Bands.Add(docBand);
			} else
				rootBand = docBand;
			PageRowCalculator calc = new PageRowCalculator();
			FillPageResult fillPageResult = calc.FillPage(rootBand, new RectangleF(PointF.Empty, bounds.Size));
			if((fillPageResult & FillPageResult.Complete) > 0)
				return bounds.Height;
			float result = includeBottomSpans ?
				calc.y + rootBand.TopSpan + rootBand.BottomSpan :
				calc.y + rootBand.TopSpan - calc.bottomSpan;
			return Math.Min(bounds.Height, result);
		}
	}
}
