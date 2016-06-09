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
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Export;
#if SL
using DevExpress.Xpf.Drawing.Printing;
#else
using System.Drawing.Printing;
#endif
namespace DevExpress.XtraPrinting.Native {
	public interface IDocumentProxy {
		string InfoString { get; }
		bool SmartXDivision { get; }
		bool SmartYDivision { get; }
		int PageCount { get; }
		void AddPage(PSPage page);
	}
	public class PageBuildEngine {
		#region static
		protected void EnsureProgressReflectorRanges(DocumentBand rootBand) {
			if(PrintingSystem.ProgressReflector.RangeCount == 0)
				PrintingSystem.ProgressReflector.SetProgressRanges(new float[] { 50, 50 });
		}
		static RectangleF ValidateMinSize(RectangleF rect) {
			rect.Width = Math.Max(rect.Width, PrintingDocument.MinPageSize.Width);
			rect.Height = Math.Max(rect.Height, PrintingDocument.MinPageSize.Height);
			return rect;
		}
		static ReadonlyPageData GetActualPageData(ReadonlyPageData defaultPageData, CustomPageData nextPageData) {
			if (nextPageData == null)
				return new ReadonlyPageData(defaultPageData);
			bool landscape = nextPageData.Landscape.HasValue ? (bool)nextPageData.Landscape : defaultPageData.Landscape;
			Margins margins = nextPageData.Margins != null ? nextPageData.Margins : defaultPageData.Margins;
			PaperKind paperKind = nextPageData.PaperKind.HasValue ? (PaperKind)nextPageData.PaperKind : defaultPageData.PaperKind;
			Size pageSize = PageSizeInfo.GetPageSize(paperKind, defaultPageData.Size);
			if (!nextPageData.PageSize.IsEmpty && paperKind == PaperKind.Custom)
				pageSize = nextPageData.PageSize;
			Margins minMargins = defaultPageData.MinMargins;
			return new ReadonlyPageData(margins, minMargins, paperKind, pageSize, landscape);
		}
		#endregion
		protected DocumentBand rootBand;
		protected IDocumentProxy document;
		ReadonlyPageData defaultPageData;
		ReadonlyPageData actualPageData;
		CustomPageData nextPageData;
		RootDocumentBand root;
		protected XPageContentEngine xContentEngine;
		protected bool Stopped { get; private set; }
		protected bool Aborted { get; private set; }
		protected PrintingSystemBase PrintingSystem {
			get { return root.PrintingSystem; }
		}
		protected ReadonlyPageData ActualPageData {
			get {
				if (actualPageData == null)
					actualPageData = GetActualPageData(defaultPageData, NextPageData);
				return actualPageData;
			}
		}
		protected CustomPageData NextPageData {
			get { return nextPageData; }
			set { 
				nextPageData = value;
				InvalidateActualPageData();
			}
		}
		protected SizeF ActualPageSizeF {
			get { return new SizeF(float.MaxValue, ActualUsefulPageRectF.Height); }
		}
		protected virtual RectangleF ActualUsefulPageRectF {
			get { return ActualPageData.UsefulPageRectF; }
		}
		bool FillEmptySpace {
			get;
			set;
		}
		protected RootDocumentBand Root {
			get { return root; }
		}
		public PageBuildEngine(RootDocumentBand root, IDocumentProxy document) : this(root, document, XPageContentEngine.CreateInstance(document.SmartXDivision), true) {
		}
		protected PageBuildEngine(RootDocumentBand root, IDocumentProxy document, XPageContentEngine xContentEngine, bool fillEmptySpace) {
			this.document = document;
			this.root = root;
			this.xContentEngine = xContentEngine;
			FillEmptySpace = fillEmptySpace;
		}
		public virtual void Abort() {
			Aborted = true;
		}
		public virtual void Stop() {
			Stopped = true;
		}
		void InvalidateActualPageData() {
			actualPageData = null;
		}
		public void BuildPages(DocumentBand rootBand) {
			try {
				this.rootBand = rootBand;
				if(rootBand == null || PrintingSystem == null)
					return;
				rootBand.Reset(ShouldResetBricksOffset, PageBreaksActiveStatus);
				this.defaultPageData = PrintingSystem.PageSettings.Data;
				NextPageData = null;
				Build();
			} catch(Exception exception) {
				Tracer.TraceError(NativeSR.TraceSource, exception);
				RaiseCreateDocumentException(exception);
			}
		}
		protected void RaiseCreateDocumentException(Exception exception) {
			ExceptionEventArgs args = new ExceptionEventArgs(exception);
			PrintingSystem.OnCreateDocumentException(args);
		}
		protected internal void AfterBuildPages() {
			if(this.rootBand != null && document.PageCount == 0) {
				PSPage docPage = CreatePage(ActualUsefulPageRectF.Size);
				AddMargins(docPage, new Pair<int, int>(0, 0));
				if(BricksAreEmpty(docPage.InnerBricks))
					return;
				new PrintAtBottomHelper(PrintingSystem).FillEmptySpace(docPage, docPage.Rect.Height, docPage.Rect.Top);
				AfterBuildPage(docPage, ActualUsefulPageRectF);
				document.AddPage(docPage);
			}
		}
		void ProcessPage(PSPage page, Pair<int, int> indexes, XPageContentEngine contentEngine) {
			RectangleF usefulPageRect = ValidateMinSize(ActualUsefulPageRectF);
			List<PSPage> docPages = contentEngine.CreatePages(page, usefulPageRect, usefulPageRect.Width);
			PrintingSystem.PerformIfNotNull<GroupingManager>(groupingManager => {
				IList<DocumentBandInfo> bands;
				if(groupingManager.PageBands.TryGetValue(page.ID, out bands)) {
					for(int i = 0; i < docPages.Count; i++)
						groupingManager.PageBands[docPages[i].ID] = bands;
				}
			});
			for(int j = 0; j < docPages.Count; j++) {
				PSPage docPage = (PSPage)docPages[j];
				AddMargins(docPage, indexes);
				AfterBuildPage(docPage, usefulPageRect);
				document.AddPage(docPage);
			}
#if DEBUGTEST
			if(page.Owner != null) {
				PrintingSystem.PerformIfNotNull<GroupingManager>(groupingManager => {
					bool b = groupingManager.PageBands.IsEmpty();
					System.Diagnostics.Debug.Assert(b);
				});
			}
#endif
		}
		protected virtual void AfterBuildPage(PSPage page, RectangleF usefulPageArea) {
			page.AfterCreate(usefulPageArea, PrintingSystem);
		}
		void AddMargins(PSPage psPage, Pair<int, int> rowIndexes) {
			AddTopMargin(psPage, rowIndexes.First);
			AddBottomMargin(psPage, rowIndexes.Second);
		}
		void AddTopMargin(PSPage psPage, int rowIndex) {
			DocumentBand topMarginBand = Root.GetBand(DocumentBandKind.TopMargin);
			if(topMarginBand == null)
				return;
			ICollection bricks = GetPageBricks(topMarginBand, psPage, psPage.Rect, rowIndex);
			if(bricks != null && topMarginBand != null) {
				RectangleF rect = psPage.PageData.PageHeaderRect;
				rect = RectangleF.FromLTRB(rect.Left, Math.Max(0, rect.Bottom - topMarginBand.TotalHeight), rect.Right, rect.Bottom);
				BrickBase brick = psPage.AddBricks(bricks, rect);
				if(brick != null)
					brick.Modifier = BrickModifier.MarginalHeader;
			}
		}
		protected Dictionary<Brick, RectangleF> GetPageBricks(DocumentBand docBand, RectangleF bounds, SimplePageRowBuilder builder, int rowIndex) {
			if(docBand != null) {
				docBand = docBand.CopyBand(rowIndex);
				builder.FillPageBricks(docBand, bounds);
				return builder.PageBricks;
			}
			return null;
		}
		protected ICollection GetPageBricks(DocumentBand docBand, Page page, RectangleF bounds, int rowIndex) {
			if(docBand != null) {
				SimplePageRowBuilder pageBuilder = new SimplePageRowBuilder(PrintingSystem, page);
				return GetPageBricks(docBand, bounds, pageBuilder, rowIndex).Keys;
			}
			return null;
		}
		void AddBottomMargin(PSPage psPage, int rowIndex) {
			DocumentBand bottomMarginBand = Root.GetBand(DocumentBandKind.BottomMargin);
			if(bottomMarginBand == null)
				return;
			ICollection bricks = GetPageBricks(bottomMarginBand, psPage, psPage.Rect, rowIndex);
			if(bricks != null && bottomMarginBand != null) {
				RectangleF rect = psPage.PageData.PageFooterRect;
				rect.Height = Math.Min(rect.Height, bottomMarginBand.SelfHeight);
				BrickBase brick = psPage.AddBricks(bricks, rect);
				if(brick != null)
					brick.Modifier = BrickModifier.MarginalFooter;
			}
		}
		protected virtual void Build() {
			if(root.Completed) {
				EnsureProgressReflectorRanges(rootBand);
				PrintingSystem.ProgressReflector.InitializeRange(rootBand.GetBandsCountRecursive());
			}
			YPageContentEngine pageContentEngine = null;
			try {
				PSPage psPage = CreatePage(ActualPageSizeF);
				pageContentEngine = CreateContentEngine(psPage, null);
				InitializeContentEngine(pageContentEngine);
				PageRowBuilder rowBuilder = CreatePageRowBuilder(pageContentEngine);
				Dictionary<PSPage, Pair<int, int>> pages = new Dictionary<PSPage, Pair<int, int>>();
				while(true) {
					rowBuilder.BeforeFillPage(pageContentEngine);
					rowBuilder.FillPage(rootBand, psPage.Rect);
					if(Stopped || rowBuilder.IsBuildCompleted(rootBand))
						break;
					AddPage(pages, psPage, rowBuilder.GetDetailRowIndexes(rootBand).Clone(), pageContentEngine);
					NextPageData = rowBuilder.NextPageData;
					psPage = CreatePage(ActualPageSizeF);
					pageContentEngine.ResetObservableItems();
					pageContentEngine = CreateContentEngine(psPage, pageContentEngine);
					InitializeContentEngine(pageContentEngine);
				}
				AddPage(pages, psPage, rowBuilder.GetDetailRowIndexes(rootBand).Clone(), pageContentEngine);
				NextPageData = rowBuilder.NextPageData;
			} finally {
				pageContentEngine.ResetObservableItems();
				if(root.Completed)
					PrintingSystem.ProgressReflector.MaximizeRange();
			}
		}
		protected virtual YPageContentEngine CreateContentEngine(PSPage psPage, YPageContentEngine previous) {
			return document.SmartYDivision ? new YPageContentEngine2(psPage, PrintingSystem, (YPageContentEngine2)previous) :
				new YPageContentEngine(psPage, PrintingSystem);
		}
		protected virtual void InitializeContentEngine(YPageContentEngine contentEngine) {
		}
		protected virtual PageRowBuilder CreatePageRowBuilder(YPageContentEngine pageContentEngine) {
			return new PageHeaderFooterRowBuilder(pageContentEngine);
		}
		protected virtual bool PageBreaksActiveStatus {
			get { return true; }
		}
		protected virtual bool ShouldResetBricksOffset {
			get { return true;  }
		}
		protected virtual PSPage CreatePage(SizeF pageSize) {
			PSPage psPage = new PSPage(new ReadonlyPageData(ActualPageData));
			psPage.Rect = new RectangleF(PointF.Empty, pageSize);
			return psPage;
		}
		protected virtual bool AddPage(Dictionary<PSPage, Pair<int, int>> pages, PSPage psPage, Pair<int, int> rowIndexes, YPageContentEngine pageContentEngine) {
			if(pages.Count > 0 && !AddedBandsAreActual(pageContentEngine.AddedBands))
				return false;
			if(!pages.ContainsKey(psPage) && !BricksAreEmpty(psPage.Bricks)) {
				pages.Add(psPage, rowIndexes);
				pageContentEngine.CorrectPrintAtBottomBricks(pageContentEngine.AddedBands, psPage.Rect.Bottom, false);
				new PrintAtBottomHelper(PrintingSystem).FireFillPage(pageContentEngine.AddedBands, psPage.Rect.Bottom, psPage, FillEmptySpace);
				ProcessPage(psPage, rowIndexes, xContentEngine);
				return true;
			}
			return false;
		}
		static bool AddedBandsAreActual(IList<BandBricksPair> addedBands) {
			foreach(BandBricksPair item in addedBands) {
				DocumentBand topLevelBand = item.Band.FindTopLevelBand();
				if(!topLevelBand.IsKindOf(DocumentBandKind.PageBand))
					return true;
			}
			return false;
		}
		static bool BricksAreEmpty(IList bricks) {
			foreach(BrickBase brick in bricks)
				if(brick.Rect.Height > 0 && brick.Rect.Width > 0)
					return false;
			return true;
		}
	}
}
