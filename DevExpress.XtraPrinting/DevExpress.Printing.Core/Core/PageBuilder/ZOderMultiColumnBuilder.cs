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
	public class ZOderMultiColumnBuilder : PageHeaderFooterRowBuilder {
		int columnIndex;
		public ZOderMultiColumnBuilder(YPageContentEngine pageContentEngine)
			: base(pageContentEngine) {
		}
		public FillPageResult BuildZOrderMultiColumn(DocumentBand rootBand, MultiColumn mc, RectangleF bounds) {
			int bi = BuildInfoContainer.GetBuildInfo(rootBand);
			while(CanProcessDetail(rootBand, new PageBuildInfo(bi, bounds, Offset.ToPointF()))) {
				bool isPageUpdated;
				FillPageResult fillPageResult = PrintRow(rootBand, bounds, mc, out isPageUpdated);
				bi = BuildInfoContainer.GetBuildInfo(rootBand);
				if(!isPageUpdated)
					return FillPageResult.Overfulfil;
				if(fillPageResult == FillPageResult.Overfulfil)
					return fillPageResult;
			}
			return FillPageResult.None;
		}
		protected override bool CanProcessDetail(DocumentBand rootBand, PageBuildInfo pageBuildInfo) {
			DocumentBand docBand = rootBand.GetBand(pageBuildInfo.Index);
			if(docBand != null)
				return !docBand.IsKindOf(DocumentBandKind.Footer) && base.CanProcessDetail(rootBand, pageBuildInfo);
			return false;
		}
		protected override bool CanFillPageWithBricks(DocumentBand docBand) {
			return !docBand.IsKindOf(DocumentBandKind.PageBand, DocumentBandKind.Header) &&
				base.CanFillPageWithBricks(docBand);
		}
		List<DocumentBand> GetDocumentBands(DocumentBand rootBand, int count, RectangleF bounds) {
			List<DocumentBand> result = new List<DocumentBand>();
			SkipEmptyBands(rootBand);
			for(int i = 0; i < count; i++) {
				int bi = BuildInfoContainer.GetBuildInfo(rootBand);
				if(CanProcessDetail(rootBand, new PageBuildInfo(bi, bounds, Offset.ToPointF()))) {
					result.Add(rootBand.Bands[bi]);
					IncreaseBuildInfo(rootBand, bi, 1);
					bi++;
				}
				else
					return result;
			}
			return result;
		}
		void SkipEmptyBands(DocumentBand rootBand) {
			int bi = BuildInfoContainer.GetBuildInfo(rootBand);
			for(; bi < rootBand.Bands.Count; ) {
				DocumentBand docBand = rootBand.Bands[bi];
				if(!CanFillPageWithBricks(docBand) || !docBand.IsValid) {
					IncreaseBuildInfo(rootBand, bi, 1);
					bi++;
				} else
					return;
			}
		}
		FillPageResult PrintRow(DocumentBand rootBand, RectangleF bounds, MultiColumn mc, out bool pageIsUpdated) {
			pageIsUpdated = true;
			FillPageResult fillPageResult = FillPageResult.None;
			OffsetHelperY offsetHelper = new OffsetHelperXY((float)Offset.X);
			float startOffsetY = (float)this.Offset.Y;
			float startNegativeOffsetY = (float)NegativeOffsetY;
			try {
				int bi = BuildInfoContainer.GetBuildInfo(rootBand);
				SkipEmptyBands(rootBand);
				if(!CanProcessDetail(rootBand, new PageBuildInfo(bi, bounds, Offset.ToPointF())))
					return FillPageResult.None;
				int inilialBuildInfo = bi;
				columnIndex = 0;
				while(columnIndex < mc.ColumnCount) {
					if(CanProcessDetail(rootBand, new PageBuildInfo(bi, bounds, Offset.ToPointF()))) {
						DocumentBand docBand = rootBand.GetBand(bi);
						if(docBand.IsEmpty) {
							IncreaseBuildInfo(rootBand, bi, 1);
							bi++;
							continue;
						}
						PageRowBuilderBase builder = CreateInternalPageRowBuilder();
						builder.CopyFrom(this);
						builder.NegativeOffsetY = this.NegativeOffsets.GetValueOrDefault(docBand.ID, startNegativeOffsetY);
						double oldNegativeOffsetY = builder.NegativeOffsetY;
						FillPageResult fillResult = builder.FillPageForBand(rootBand, bounds, GetCorrectedBounds(rootBand, bounds));
						this.CopyFrom(builder);
						if(builder.NegativeOffsetY != 0 || fillResult == FillPageResult.Overfulfil) {
							this.NegativeOffsets[docBand.ID] = (float)builder.NegativeOffsetY;
						} else {
							this.NegativeOffsets.Remove(docBand.ID);
							builder.NegativeOffsetY = -builder.OffsetY + oldNegativeOffsetY;
						}
						if(fillResult == FillPageResult.Overfulfil) {
							fillPageResult = FillPageResult.Overfulfil;
							if(bi < rootBand.Bands.Count)
								rootBand.Bands[bi].GenerateBandChildren();
							if (!builder.RenderHistory.IsDocumentBandRendered(docBand)) {
								pageIsUpdated = false;
								break;
							}
						}
						offsetHelper.Update(builder);
						if(columnIndex < mc.ColumnCount - 1) {
							OffsetX += mc.ColumnWidth;
							OffsetY = startOffsetY;
							NegativeOffsetY = startNegativeOffsetY;
						}
						IncreaseBuildInfo(rootBand, bi, 1);
						bi++;
						columnIndex++;
					} else
						break;
				}
				if(fillPageResult == FillPageResult.Overfulfil)
					BuildInfoContainer.SetBuildInfo(rootBand, inilialBuildInfo);
			} finally {
				columnIndex = -1;
				offsetHelper.UpdateBuilder(this);
				if(fillPageResult == FillPageResult.None)
					NegativeOffsetY = 0;
			}
			return fillPageResult;
		}
		protected internal override PageRowBuilderBase CreateInternalPageRowBuilder() {
			return new ZOderMultiColumnBuilderInternal(PageContentEngine, columnIndex);
		}
	}
	public class ZOderMultiColumnBuilderInternal : PageHeaderFooterRowBuilder {
		int columnIndex { get; set; }
		public ZOderMultiColumnBuilderInternal(YPageContentEngine pageContentEngine, int columnIndex)
			: base(pageContentEngine) {
				this.columnIndex = columnIndex;
		}
		protected override bool ShouldOverFulfil(DocumentBand docBand, RectangleF bounds) {
			return ForceSplit ? false : base.ShouldOverFulfil(docBand, bounds);
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
	}
}
