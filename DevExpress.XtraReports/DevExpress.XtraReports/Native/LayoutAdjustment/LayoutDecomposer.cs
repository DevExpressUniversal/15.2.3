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
using System.Text;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.XtraPrinting.Native;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native.LayoutAdjustment;
namespace DevExpress.XtraReports.Native.LayoutAdjustment {
	public class LayoutDecomposer {
		#region inner classes
		class BottomSpanEvaluator {
			Func<int, float> getTop;
			public BottomSpanEvaluator(Func<int, float> getTop) {
				this.getTop = getTop;
			}
			public float Eval(float bottom, int index, int count) {
				for(int i = index; i < count; i++) {
					float nextTop = getTop(i);
					if(!FloatsComparer.Default.FirstGreaterSecond(bottom, nextTop)) {
						return nextTop - bottom;
					}
				}
				return 0f;
			}
		}
		class BandComparer : IComparer<DocumentBand> {
			IListWrapper<DocumentBand> bands;
			Dictionary<DocumentBand, RectangleF> rectDictionary;
			public BandComparer(IListWrapper<DocumentBand> bands, Dictionary<DocumentBand, RectangleF> rectDictionary) {
				this.bands = bands;
				this.rectDictionary = rectDictionary;
			}
			public int Compare(DocumentBand x, DocumentBand y) {
				RectangleF rectX = rectDictionary[x];
				RectangleF rectY = rectDictionary[y];
				int result = Comparer.Default.Compare(rectX.Y, rectY.Y);
				if(result == 0)
					result = Comparer.Default.Compare(rectX.X, rectY.X);
				if(result == 0)
					result = Comparer.Default.Compare(bands.IndexOf(x), bands.IndexOf(y));
				return result;
			}
		}
		#endregion
		static bool IsCrossBandLineBrick(Brick brick) {
			return brick is LineBrick && brick.CanOverflow;
		}
		DocumentBand band;
		public LayoutDecomposer(DocumentBand band) {
			this.band = band;
		}
		public void Decompose() {
			if(band.IsEmpty) return;
			Dictionary<object, ILayoutData> layoutDictionary = CreateLayoutDictionary(band.Bricks, band.PageBreaks);
			bool shouldDecompose = false;
			List<Pair<Brick, RectangleF>> brickPairs = new List<Pair<Brick, RectangleF>>(band.Bricks.Count);
			foreach(Brick brick in band.Bricks) {
				brickPairs.Add(new Pair<Brick, RectangleF>(brick, brick.Rect));
				if(brick is SubreportBrick && ((SubreportBrick)brick).DocumentBand != null)
					shouldDecompose = true;
			}
			List<ILayoutData> layoutData = new List<ILayoutData>();
			layoutData.Add(new LayoutDataContainer(new List<ILayoutData>(layoutDictionary.Values)));
			new LayoutAdjusterWithAnchoring(GraphicsDpi.Document).Process(layoutData);
			band.InvalidateBrickBounds();
			if(!shouldDecompose)
				return;
			brickPairs.Sort(CompareBricks);
			Dictionary<DocumentBand, RectangleF> rectDictionary = new Dictionary<DocumentBand, RectangleF>();
			Decompose(brickPairs, rectDictionary);
			AddPageBreaks(layoutDictionary, rectDictionary);
			band.SortBands(new BandComparer(band.Bands, rectDictionary));
		}
		Dictionary<object, ILayoutData> CreateLayoutDictionary(IEnumerable<Brick> bricks, IEnumerable<PageBreakInfo> pageBreaks) {
			Dictionary<object, ILayoutData> result = new Dictionary<object,ILayoutData>();
			foreach(Brick item in bricks)
				if(item is VisualBrick) {
					VisualBrick brick = (VisualBrick)item;
					ILayoutData data = brick.CreateLayoutData(GraphicsDpi.Document);
					result.Add(brick, data);
				}
			foreach(ValueInfo item in pageBreaks) {
				if(item.Value != DocumentBand.MaxBandHeightPix)
					result.Add(item, new PageBreakLayoutData(item, GraphicsDpi.Document));
			}
			return result;
		}
		int CompareBricks(Pair<Brick, RectangleF> x, Pair<Brick, RectangleF> y) {
			int result = x.Second.Top.CompareTo(y.Second.Top);
			return result != 0 ? result : x.Second.Left.CompareTo(y.Second.Left);
		}
		void Decompose(List<Pair<Brick, RectangleF>> brickPairs, Dictionary<DocumentBand, RectangleF> rectDictionary) {
			BottomSpanEvaluator ev = new BottomSpanEvaluator(i => brickPairs[i].Second.Top);
			for(int i = 0; i < brickPairs.Count; i++) {
				Brick brick = brickPairs[i].First;
				RectangleF brickRect = brickPairs[i].Second;
				SubreportBrick subreportBrick = brick as SubreportBrick;
				if(subreportBrick != null) {
					DocumentBand docBand = subreportBrick.DocumentBand;
					if(docBand == null)
						continue;
					docBand.BottomSpan = ev.Eval(brickRect.Bottom, i + 1, brickPairs.Count) + subreportBrick.AdditionalBottomSpan;
					docBand.OffsetX = brickRect.X;
					brickRect.Height = Math.Max(0, brickRect.Height - subreportBrick.AdditionalBottomSpan);
					((ISubreportDocumentBand)docBand).ReportRect = brickRect;
					band.Bands.Add(docBand);
					rectDictionary.Add(docBand, brickRect);
				} else {
					SubreportDocumentBand subrepBand = IsCrossBandLineBrick(brick) ?
						new CrossBandBrickDocumentBand(brickRect, brick as VisualBrick) :
						new BrickDocumentBand(brickRect, brick as VisualBrick);
					band.Bands.Add(subrepBand);
					rectDictionary.Add(subrepBand, brickRect);
					subrepBand.BottomSpan = ev.Eval(brickRect.Bottom, i + 1, brickPairs.Count);
					subrepBand.OffsetX = brickRect.X;
					DocumentBand brickBand = new DocumentBand(DocumentBandKind.Detail, band.RowIndex);
					subrepBand.Bands.Add(brickBand);
					brick.Location = PointF.Empty;
					brickBand.Bricks.Add(brick);
				}
			}
			band.Bricks.Clear();
		}
		void AddPageBreaks(Dictionary<object, ILayoutData> layoutDictionary, Dictionary<DocumentBand, RectangleF> rectDictionary) {
			for(int i = band.PageBreaks.Count - 1; i >= 0; i--) {
				ValueInfo pageBreak = band.PageBreaks[i];
				if(pageBreak.Value == 0 || pageBreak.Value == DocumentBand.MaxBandHeightPix)
					continue;
				DocumentBand pageBreakBand = DocumentBand.CreatePageBreakBand(0f);
				band.Bands.Add(pageBreakBand);
				band.PageBreaks.RemoveAt(i);
				rectDictionary.Add(pageBreakBand, layoutDictionary[pageBreak].InitialRect);
			}
		}
	}
}
