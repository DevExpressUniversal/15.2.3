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
using DevExpress.XtraPrinting.Export;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using System.Drawing.Printing;
using System.Linq;
using System.Collections.ObjectModel;
using DevExpress.Utils.Serializing;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class ContinuousPageBuildEngine : PageBuildEngine {
		#region inner classes
		protected class ContinuousPageHeaderFooterRowBuilder : PageHeaderFooterRowBuilder {
			public ContinuousPageHeaderFooterRowBuilder(YPageContentEngine psPage)
				: base(psPage) {
			}
			public override bool PrintOnPages(DocumentBand docBand,  DevExpress.XtraReports.UI.PrintOnPages printOnPages) {
				return (DevExpress.XtraReports.UI.PrintOnPages.AllPages & printOnPages) > 0;
			}
		}
		protected class ContinuousContentEngine : YPageContentEngine {
			public ContinuousContentEngine(PSPage psPage, PrintingSystemBase ps)
				: base(psPage, ps) {
			}
			public override void CorrectPrintAtBottomBricks(List<BandBricksPair> docBands, float pageBottom, bool ignoreBottomSpan) {
			}
		}
		#endregion
		protected PageRowBuilder builder;
		protected override RectangleF ActualUsefulPageRectF { 
			get {
				MarginsF margins = PrintingSystem.PageSettings.Data.MarginsF;
				return new RectangleF(margins.Left, margins.Top, float.MaxValue, float.MaxValue); 
			}
		}
		public ContinuousPageBuildEngine(PrintingDocument document) : base(document.Root, document) {
		}
		public ContinuousPageBuildEngine(PrintingDocument document, XPageContentEngine xContentEngine, bool fillEmptySpace)
			: base(document.Root, document, xContentEngine, fillEmptySpace) {
		}
		protected override PageRowBuilder CreatePageRowBuilder(YPageContentEngine psPage) {
			builder = new ContinuousPageHeaderFooterRowBuilder(psPage);
			builder.CanApplyPageBreaks = false;
			return builder;
		}
		protected override bool PageBreaksActiveStatus {
			get { return false; }
		}
		protected override bool ShouldResetBricksOffset {
			get { return false; }
		}
		protected override YPageContentEngine CreateContentEngine(PSPage psPage, YPageContentEngine previous) {
			return new ContinuousContentEngine(psPage, PrintingSystem);
		}
		protected override PSPage CreatePage(SizeF pageSize) {
			PSPage page = new PSPage(new PageData(ActualPageData));
			page.Rect = new RectangleF(PointF.Empty, pageSize);
			return page;
		}
	}
	public class ExportPageBuildEngine : ContinuousPageBuildEngine {
		#region inner classes
		protected class ExportContentEngine : ContinuousContentEngine {
			bool wasMultiColumn;
			float intermediateContentHeight = 0;
			public Collection<float> pageBreakPositions;
			public Collection<MultiColumnInfo> multiColumnInfo;
			public Dictionary<Brick, RectangleDF> brickList;
			public override int PageBricksCount {
				get { return brickList.Count; }
			}
			public ExportContentEngine(PSPage psPage, PrintingSystemBase ps)
				: base(psPage, ps) {
				brickList = new Dictionary<Brick, RectangleDF>();
				pageBreakPositions = new Collection<float>();
				multiColumnInfo = new Collection<MultiColumnInfo>();
			}
			public override PageUpdateData UpdateContent(DocumentBand docBand, PointD offset, RectangleF bounds, bool forceSplit) {
				foreach(Brick brick in docBand.Bricks) {
					RectangleDF rect = RectangleDF.Offset(brick.InitialRect, bounds.X + offset.X, bounds.Y + offset.Y);
					brick.PerformLayout(ps);
					brickList.Add(brick, rect);
				}
				if(docBand.Bricks.Count > 0)
					AddedBands.Add(new BandBricksPair(docBand, ArrayListExtentions.CreateInstance(docBand.Bricks)));
				UpdateAdditionalInfo(docBand);
				return new PageUpdateData(bounds, true);
			}
			void UpdateAdditionalInfo(DocumentBand docBand) {
				if(docBand.PageBreaks.Count > 0) {
					foreach(PageBreakInfo pbInfo in docBand.PageBreaks)
						pageBreakPositions.Add(intermediateContentHeight + pbInfo.Value);
				}
				Tuple<bool, int> mcResult = TryGetMultiColumn(docBand);
				bool isMultiColumn = mcResult.Item1;
				if(isMultiColumn != wasMultiColumn) {
					if(isMultiColumn) {
						MultiColumnInfo mci = new MultiColumnInfo();
						mci.Start = intermediateContentHeight;
						mci.ColumnCount = mcResult.Item2;
						multiColumnInfo.Add(mci);
					} else {
						multiColumnInfo.Last().End = intermediateContentHeight;
					}
				}
				wasMultiColumn = isMultiColumn;
				intermediateContentHeight += docBand.TotalHeight;
			}
			Tuple<bool, int> TryGetMultiColumn(DocumentBand docBand) {
				MultiColumn mc = docBand.MultiColumn;
				if(mc != null && mc.Order == ColumnLayout.DownThenAcross)
					return new Tuple<bool,int>(true, mc.ColumnCount);
				return docBand.Parent != null && (!(docBand.Parent is RootDocumentBand) || docBand.IsKindOf(DocumentBandKind.Detail)) ? TryGetMultiColumn(docBand.Parent)
					: new Tuple<bool,int>(false, 0); 
			}
		}
		protected class ContinuousSimplePageRowBuilder : SimplePageRowBuilder {
			public ContinuousSimplePageRowBuilder(PrintingSystemBase ps) : base(ps, null) {
			}
			protected override PageUpdateData UpdatePageContent(DocumentBand docBand, RectangleF bounds) {
				foreach(Brick brick in docBand.Bricks) {
					brick.PerformLayout(PrintingSystem);
					if(brick.IsPageBrick) {
						PageBricks.Add(brick, brick.Rect);
					} else {
						RectangleF rect = RectF.Offset(brick.InitialRect, (float)Offset.X, (float)(bounds.Y + Offset.Y));
						PageBricks.Add(brick, rect);
					}
				}
				return new PageUpdateData(bounds, true);
			}
		}
		#endregion
		ExportContentEngine contentEngine;
		public ExportPageBuildEngine(PrintingDocument document)
			: base(document) {
		}
		protected override bool AddPage(Dictionary<PSPage, Pair<int, int>> pages, PSPage psPage, Pair<int, int> rowIndexes, YPageContentEngine pageContentEngine) {
			pages.Add(psPage, rowIndexes);
			return true;
		}
		protected override YPageContentEngine CreateContentEngine(PSPage psPage, YPageContentEngine previous) {
			this.contentEngine = new ExportContentEngine(psPage, PrintingSystem);
			return contentEngine;
		}
		public ContinuousExportInfo CreateContinuousExportInfo() {
			if(Root == null)
				return ContinuousExportInfo.Empty;
			Pair<Dictionary<Brick, RectangleDF>, double> infoBricks = ExecuteInfo(document.InfoString);
			Pair<Dictionary<Brick, RectangleF>, double> topBricks = ExecuteMargin(DocumentBandKind.TopMargin, BrickModifier.MarginalHeader);
			Pair<Dictionary<Brick, RectangleDF>, double> bricks = Execute();
			PrintingSystem.OnAfterPagePrint(new PageEventArgs(null, null));
			Pair<Dictionary<Brick, RectangleF>, double> bottomBricks = ExecuteMargin(DocumentBandKind.BottomMargin, BrickModifier.MarginalFooter);
			double topBricksOffset = topBricks.First.Count > 0 ? GraphicsUnitConverter.Convert(PrintingSystem.PageMargins.Top, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.Document) : 0;
			JoinBricks(infoBricks.First, ToRectangleDFEnumerable(topBricks.First), infoBricks.Second);
			JoinBricks(infoBricks.First, bricks.First, infoBricks.Second + topBricksOffset);
			JoinBricks(infoBricks.First, ToRectangleDFEnumerable(bottomBricks.First), infoBricks.Second + topBricksOffset + bricks.Second);
			RectangleDF usefulPageRect = new RectangleDF(double.NaN, topBricksOffset, float.NaN, (float)bricks.Second);
			MergeBrickHelper mergeHelper = ((IServiceProvider)PrintingSystem).GetService(typeof(MergeBrickHelper)) as MergeBrickHelper;
			if(mergeHelper != null) mergeHelper.ProcessBricks(PrintingSystem, infoBricks.First);
			return new ContinuousExportInfo(infoBricks.First, PrintingSystem.PageMargins, PrintingSystem.PageBounds, (float)bottomBricks.Second,
				contentEngine.pageBreakPositions, contentEngine.multiColumnInfo);
		}
		Pair<Dictionary<Brick, RectangleDF>, double> ExecuteInfo(string infoString) {
			Dictionary<Brick, RectangleDF> brickDictionary = new Dictionary<Brick, RectangleDF>();
#if !SL
			if(!String.IsNullOrEmpty(infoString)) {
				Brick infoBrick = new TextBrick(BorderSide.None, 1.0f, DXColor.White, DXColor.Transparent, InformationHelper.Color) {
					Text = infoString,
					HorzAlignment = Utils.HorzAlignment.Center,
					VertAlignment = Utils.VertAlignment.Center,
					Font = InformationHelper.Font,
					PrintingSystem = PrintingSystem,
					Location = PointF.Empty,
					Size = InformationHelper.CalcSize(infoString, PrintingSystem.Graph.Dpi, ((IPrintingSystemContext)PrintingSystem).Measurer)
				};
				brickDictionary.Add(infoBrick, RectangleDF.FromRectangleF(infoBrick.Rect));
				return new Pair<Dictionary<Brick, RectangleDF>, double>(brickDictionary, infoBrick.Rect.Bottom);
			}
#endif
			return new Pair<Dictionary<Brick, RectangleDF>, double>(brickDictionary, 0f);
		}
		Pair<Dictionary<Brick, RectangleDF>, double> Execute() {
			BuildPages(Root);
			System.Diagnostics.Debug.Assert(contentEngine != null);
			return new Pair<Dictionary<Brick, RectangleDF>, double>(this.contentEngine.brickList, builder.Offset.Y);
		}
		Pair<Dictionary<Brick, RectangleF>, double> ExecuteMargin(DocumentBandKind marginKind, BrickModifier modifier) {
			DocumentBand marginBand = Root.GetBand(marginKind);
			ContinuousSimplePageRowBuilder builder = new ContinuousSimplePageRowBuilder(PrintingSystem);
			Dictionary<Brick, RectangleF> bricks = GetPageBricks(marginBand, new RectangleF(PointF.Empty, ActualUsefulPageRectF.Size), builder, marginBand.RowIndex);
			foreach(Brick brick in bricks.Keys) {
				brick.SetModifierRecursive(modifier);
			}
			return new Pair<Dictionary<Brick, RectangleF>, double>(bricks, builder.StartBand.TopSpan);
		}
		void JoinBricks(Dictionary<Brick, RectangleDF> destBricks, IEnumerable<KeyValuePair<Brick, RectangleDF>> bricks, double dy) {
			if(destBricks == null || bricks == null)
				return;
			foreach(KeyValuePair<Brick, RectangleDF> item in bricks)
				destBricks.Add(item.Key, RectangleDF.Offset(item.Value, 0f, dy));
		}
		IEnumerable<KeyValuePair<Brick, RectangleDF>> ToRectangleDFEnumerable(IEnumerable<KeyValuePair<Brick, RectangleF>> enumerable) {
			foreach(KeyValuePair<Brick, RectangleF> pair in enumerable)
				yield return new KeyValuePair<Brick, RectangleDF>(pair.Key, RectangleDF.FromRectangleF(pair.Value));
		}
	}
}
