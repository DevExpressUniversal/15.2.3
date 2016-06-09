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
	public class NOderMultiColumnBuilder : PageHeaderFooterRowBuilder {
		#region inner classes
		class NOderOffsetHelperXY : OffsetHelperXY {
			public NOderOffsetHelperXY(float offsetX)
				: base(offsetX) {
			}
			protected override bool ShouldUpdateNegativeOffsetY { get { return false; } }
		}
		#endregion
		int columnIndex;
		public NOderMultiColumnBuilder(YPageContentEngine pageContentEngine)
			: base(pageContentEngine) {
				columnIndex = -1;
		}
		protected override bool CanFillPageWithBricks(DocumentBand docBand) {
			return !docBand.IsKindOf(DocumentBandKind.PageBand) && base.CanFillPageWithBricks(docBand);
		}
		protected internal override PageRowBuilderBase CreateInternalPageRowBuilder() {
			return new NOderMultiColumnBuilder(PageContentEngine) { columnIndex = this.columnIndex };
		}
		public FillPageResult BuildNOrderMultiColumn(DocumentBand rootBand, MultiColumn mc, RectangleF bounds) {
			OffsetHelperY offsetHelper = new NOderOffsetHelperXY((float)Offset.X);
			FillPageResult fillPageResult = FillPageResult.None;
			List<List<BandBricksPair>> bandsList = new List<List<BandBricksPair>>();
			columnIndex = 0;
			float pageOffset = (float)FullOffsetY > 0 ? bounds.Y + (float)FullOffsetY : bounds.Y;
			RectangleF bounds1 = RectangleF.FromLTRB(bounds.Left, pageOffset, bounds.Right, bounds.Bottom);
			OffsetY = 0;
			for(int i = 0; i < mc.ColumnCount; i++, columnIndex++) {
				PageBreakRect = RectangleF.Empty;
				MarkLastAddedPage();
				fillPageResult = this.FillPageForBands(rootBand, bounds1, CanProcessDetail);
				bandsList.Add(this.GetAddedBands());
				OffsetY += pageOffset - bounds.Y;
				offsetHelper.Update(this);
				if(fillPageResult.IsComplete())
					OffsetY = 0;
				else
					break;
				OffsetX += mc.ColumnWidth;
			}
			offsetHelper.UpdateBuilder(this);
			foreach(List<BandBricksPair> docBands in bandsList) {
				CorrectPrintAtBottomBricks(docBands, (float)Offset.Y);
			}
			if (fillPageResult == FillPageResult.Overfulfil)
				return FillPageResult.Overfulfil;
			else
				return FillPageResult.None;
		}
		protected override void CorrectPrintAtBottomBricks(List<BandBricksPair> docBands, float pageBottom) {
			this.PageContentEngine.CorrectPrintAtBottomBricks(docBands, pageBottom, false);
		}
		DocumentBand GetPrintingBand(DocumentBand docBand) {
			int bi = BuildInfoContainer.GetBuildInfo(docBand);
			if(bi < docBand.Bands.Count)
				return GetPrintingBand(docBand.Bands[bi]);
			return docBand;
		}
		protected override bool AreFriendsTogether(DocumentBand docBand, RectangleF bounds) {
#pragma warning disable 612, 618
			if(PrintingSettings.MultiColumnDownThenAcrossNewBehavior)
				return base.AreFriendsTogether(docBand, bounds);
#pragma warning restore 612, 618
			#region remove in 15.2
			return !AreBoundsComplete(bounds, BuildHelper.GetDocBandHeight(docBand, bounds, true), (float)Offset.Y);
			#endregion
		}
		protected override bool AreFriendsTogetherRecursive(DocumentBand docBand, RectangleF bounds) {
			if (docBand.KeepTogether && !docBand.KeepTogetherOnTheWholePage) {
				if(AreBoundsComplete(bounds, BuildHelper.GetDocBandHeight(docBand, bounds, false), (float)Offset.Y)) {
					NegativeOffsetY = 0;
					return false;
				}
			}
			return true;
		}
		protected override PageUpdateData UpdatePageContent(DocumentBand docBand, RectangleF bounds) {
			if(columnIndex != -1)
				UpdateMultiColumnBricks(docBand.Bricks);
			return base.UpdatePageContent(docBand, bounds);
		}
		void UpdateMultiColumnBricks(IEnumerable<Brick> bricks) {
			foreach(Brick brick in bricks) {
				if(!brick.CanMultiColumn)
					brick.CanAddToPage = columnIndex == 0;
			}
		}
		protected override bool ShouldOverFulfil(DocumentBand docBand, RectangleF bounds) {
#pragma warning disable 612, 618
			if(PrintingSettings.MultiColumnDownThenAcrossNewBehavior)
				return ShouldOverFulfilCore(docBand, bounds, new NOrderBandContentAnalyzer(PageContentEngine.AddedBands, OffsetX));
#pragma warning restore 612, 618
			#region remove in 15.2
			return base.ShouldOverFulfil(docBand, bounds);
			#endregion
		}
	}
	class NOrderBandContentAnalyzer : BandContentAnalyzer {
		double offsetX;
		public NOrderBandContentAnalyzer(IEnumerable<BandBricksPair> docBands, double offsetX) : base(docBands) {
			this.offsetX = offsetX;
		}
		protected override bool AboveComparison(RectangleF rect, float bound) {
			return FloatsComparer.Default.FirstGreaterSecond(rect.Bottom, bound) && FloatsComparer.Default.FirstGreaterSecond(rect.Left, offsetX);
		}
	}
}
