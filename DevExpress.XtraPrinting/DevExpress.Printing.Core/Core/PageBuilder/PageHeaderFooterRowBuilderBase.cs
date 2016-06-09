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
namespace DevExpress.XtraPrinting.Native {
	public class PageHeaderFooterRowBuilderBase : PageRowBuilder {
		static bool IsNextPageBreak(DocumentBand footerBand) {
			if(footerBand.Bands.Count > 0) {
				DocumentBand lastBand = footerBand.Bands.GetLast<DocumentBand>();
				return lastBand.Kind == DocumentBandKind.PageBreak && lastBand.HasActivePageBreaks;
			}
			return false;
		}
		public PageHeaderFooterRowBuilderBase(YPageContentEngine pageContentEngine)
			: base(pageContentEngine) {
		}
		protected internal override FillPageResult FillPageRecursive(DocumentBand rootBand, DocumentBand docBand, RectangleF bounds) {
			bounds = BuildHelper.CorrectBoundsFooter(rootBand, docBand.RowIndex, bounds, this);
			return base.FillPageRecursive(rootBand, docBand, bounds);
		}
		protected override bool CanFillPageWithBricks(DocumentBand docBand) {
			if(ProcessState == ProcessState.ReportDetails || ProcessState == ProcessState.ReportFooter)
				return !docBand.IsKindOf(DocumentBandKind.PageBand) && base.CanFillPageWithBricks(docBand);
			return base.CanFillPageWithBricks(docBand);
		}
		protected override FillPageResult FillReportDetailsAndFooter(DocumentBand rootBand, RectangleF bounds) {
			RectangleF bounds1 = bounds;
			DocumentBand lastAddedBand = PageContentEngine.LastAddedBand;
			Pair<int, int> indices = new Pair<int, int>(PageContentEngine.PageBricksCount, -1);
			if(ShouldFillHeader(rootBand)) {
				bounds1 = FillPageHeader(rootBand, bounds);
				indices.Second = PageContentEngine.PageBricksCount;
			}
			DocumentBand lastAddedBand2 = PageContentEngine.LastAddedBand;
			FillPageResult fillPageResult = FillReportDetails(rootBand, bounds1);
			if(!fillPageResult.IsComplete()) {
				DocumentBand pageHeader = rootBand.GetBand(DocumentBandKind.PageHeader);
				if(ReferenceEquals(lastAddedBand2, PageContentEngine.LastAddedBand) && PrintOnPages(pageHeader, DevExpress.XtraReports.UI.PrintOnPages.NotWithReportFooter) && RenderHistory.PageHeaderRendered) {
					for(int i = indices.Second - 1; i >= indices.First; i--)
						PageContentEngine.Page.Bricks.RemoveAt(i);
					RenderHistory.PageHeaderRendered = false;
					indices.Second = -1;
					bounds1 = bounds;
				}
				lastAddedBand2 = PageContentEngine.LastAddedBand;
				fillPageResult = FillReportFooter(rootBand, bounds1);
				if(!ReferenceEquals(lastAddedBand2, PageContentEngine.LastAddedBand) && PrintOnPages(pageHeader, DevExpress.XtraReports.UI.PrintOnPages.NotWithReportFooter) && RenderHistory.PageHeaderRendered) {
					for(int i = indices.First; i < indices.Second; i++)
						PageContentEngine.Page.Bricks[i].IsVisible = false;
				}
			}
			DocumentBand footerBand = rootBand.GetPageFooterBand();
			if(CanFillFooterBand(footerBand) && (footerBand.IsKindOf(DocumentBandKind.PageBand) || lastAddedBand != PageContentEngine.LastAddedBand)) {
				float pos = Math.Max(bounds1.Y, (float)(bounds1.Y + Offset.Y));
				FillPageFooter(rootBand, bounds1, pos);
				int bi = BuildInfoContainer.GetBuildInfo(rootBand);
				if(bi == rootBand.Bands.IndexOf(footerBand) && IsNextPageBreak(footerBand)) {
					OffsetY += bounds1.Y - bounds.Y;
					return FillPageResult.Fulfil;
				}
			}
			OffsetY += bounds1.Y - bounds.Y;
			return fillPageResult;
		}
		protected override bool CanProcessDetail(DocumentBand rootBand, PageBuildInfo pageBuildInfo) {
			DocumentBand docBand = rootBand.GetBand(pageBuildInfo);
			if(docBand != null) {
				if(rootBand.GetPageFooterBand() == docBand && ProcessState != ProcessState.ReportHeader) {
					return false;
				}
				return base.CanProcessDetail(rootBand, pageBuildInfo);
			}
			return false;
		}
		bool ShouldFillHeader(DocumentBand rootBand) {
			DocumentBand docBand = rootBand.GetPageBand(DocumentBandKind.Header);
			if(docBand != null) {
				if(PrintOnPages(docBand, DevExpress.XtraReports.UI.PrintOnPages.NotWithReportFooter) && ProcessState == ProcessState.ReportFooter)
					return false;
				if(PrintOnPages(docBand, DevExpress.XtraReports.UI.PrintOnPages.NotWithReportHeader) && RenderHistory.ReportHeaderRendered)
					return false;
				if(docBand.IsKindOf(DocumentBandKind.PageBand))
					return true;
				int index = BuildInfoContainer.GetBuildInfo(rootBand);
				if(rootBand.Bands.IsValidIndex<DocumentBand>(index) && rootBand.Bands[index].HasDetailBandBehaviour)
					return true;
			}
			return false;
		}
		protected virtual int GetHeaderRowIndex(DocumentBand rootBand) {
			return -1;
		}
		RectangleF FillPageHeader(DocumentBand rootBand, RectangleF bounds) {
			DocumentBand docBand = rootBand.GetPageBand(DocumentBandKind.Header);
			if(docBand == null)
				return bounds;
			docBand = docBand.CopyBand(Math.Max(docBand.RowIndex, GetHeaderRowIndex(rootBand)));
			if(docBand.Bricks.Count == 0 && docBand.Bands.Count == 0)
				return bounds;
			float headerHeight = GetDocBandHeight(docBand, bounds);
			headerHeight = Math.Min((float)(bounds.Height - Offset.Y - PrintingDocument.MinPageSize.Height), headerHeight);
			RectangleF bounds1 = new RectangleF(bounds.Left, (float)(bounds.Top + Offset.Y), bounds.Width, headerHeight);
			if(PageBreakRect != RectangleF.Empty) {
				bounds1.Intersect(PageBreakRect);
				if(bounds1.IsEmpty)
					return bounds;
			}
			System.Diagnostics.Debug.Assert(Offset.X >= 0);
			bounds1.X += (float)Offset.X;
			PageRowBuilder builder = new PageRowBuilder(this.PageContentEngine);
			builder.CanApplyPageBreaks = false;
			builder.ForceSplit = true;
			if(docBand.Bricks.Count > 0) {
				builder.FillPageWithBricks(rootBand, docBand, bounds1);
			} else {
				builder.OffsetY += docBand.TopSpan;
				builder.FillPage(docBand, bounds1);
			}
			RenderHistory.PageHeaderRendered = true;
			if(NegativeOffsetY == 0) {
				OffsetY = 0;
			}
			return RectangleF.FromLTRB(bounds.Left, bounds1.Bottom, bounds.Right, bounds.Bottom);
		}
		protected virtual float GetDocBandHeight(DocumentBand docBand, RectangleF bounds) {
			return docBand.SelfHeight;
		}
		protected virtual int GetFooterRowIndex(DocumentBand rootBand) {
			return -1;
		}
		void FillPageFooter(DocumentBand rootBand, RectangleF bounds, float pos) {
			DocumentBand docBand = rootBand.GetPageFooterBand();
			docBand = docBand.CopyBand(GetFooterRowIndex(rootBand));
			if(docBand.Bricks.Count == 0 && docBand.Bands.Count == 0)
				return;
			float bandHeight = GetDocBandHeight(docBand, bounds);
			pos = Math.Max(bounds.Top + PrintingDocument.MinPageSize.Height, Math.Min(bounds.Bottom - bandHeight, pos));
			RectangleF bounds1 = RectangleF.FromLTRB(bounds.Left, pos, bounds.Right, bounds.Bottom);
			System.Diagnostics.Debug.Assert(Offset.X >= 0);
			bounds1.X += (float)Offset.X;
			PageRowBuilder builder = new PageRowBuilder(this.PageContentEngine);
			builder.CanApplyPageBreaks = false;
			if(docBand.Bricks.Count > 0)
				builder.FillPageWithBricks(rootBand, docBand, bounds1);
			else
				builder.FillPage(docBand, bounds1);
			OffsetY += builder.Offset.Y;
			OffsetY = Math.Min(Offset.Y, bounds.Height);
		}
		protected internal override PageRowBuilderBase CreateInternalPageRowBuilder() {
			return new PageHeaderFooterRowBuilderBase(PageContentEngine);
		}
		protected override RectangleF GetCorrectedBounds(DocumentBand rootBand, RectangleF bounds) {
			int rowIndex = RenderHistory.GetLastDetailRowIndex(rootBand) + 1;
			return BuildHelper.CorrectBoundsFooter(rootBand, rowIndex, bounds, this);
		}
		protected override RectangleF ValidateBounds(DocumentBand rootBand, RectangleF bounds, RectangleF newBounds) {
			int bi = BuildInfoContainer.GetBuildInfo(rootBand);
			return rootBand.Bands[bi] != rootBand.GetPageFooterBand() ? newBounds : bounds;
		}
	}
}
