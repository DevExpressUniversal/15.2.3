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
	class PrintAtBottomHelper {
		PrintingSystemBase ps;
		public PrintAtBottomHelper(PrintingSystemBase ps) {
			this.ps = ps;
		}
		public static void CorrectPrintAtBottomBricks(List<BandBricksPair> docBands, float pageBottom, PSPage psPage, bool ignoreBottomSpan) {
			if(docBands == null || docBands.Count == 0)
				return;
			List<BandBricksPair> printAtBottomBands = GetPrintAtBottomBands(docBands);
			List<BandBricksPair> detailBands = GetDetailBands(docBands);
			List<BandBricksPair> bandsToMove = GetBandsToMove(printAtBottomBands, detailBands);
			if(bandsToMove.Count > 0) {
				bandsToMove.Sort(CompareDocumentBands);
				BandBricksPair moveDocumentBand = bandsToMove[bandsToMove.Count - 1];
				float emptySpace = pageBottom - GetBandBottom(moveDocumentBand);
				if(ignoreBottomSpan)
					emptySpace -= moveDocumentBand.Band.PrimaryParent.BottomSpan;
				else
					emptySpace -= GetBottomSpanRecursive(moveDocumentBand.Band.PrimaryParent, 0f);
				if(emptySpace > 0f)
					MoveBandsToBottom(bandsToMove, emptySpace);
			}
		}
		public void FireFillPage(List<BandBricksPair> docBands, float pageBottom, PSPage psPage, bool fillEmptySpace) {
			if(docBands == null || docBands.Count == 0)
				return;
			List<BandBricksPair> printAtBottomBands = GetPrintAtBottomBands(docBands);
			List<BandBricksPair> detailBands = GetDetailBands(docBands);
			List<BandBricksPair> bandsToMove = GetBandsToMove(printAtBottomBands, detailBands);
			if(bandsToMove.Count == 0) {
				List<BandBricksPair> bottomBands = GetBottomBands(detailBands);
				if (bottomBands.Count == 0)
					return;
				float bandBottom = GetBricksBottom(bottomBands[bottomBands.Count - 1].Bricks);
				if (bandBottom >= pageBottom)
					return;
				if(fillEmptySpace)
					FillEmptySpace(psPage, pageBottom - bandBottom, bandBottom);
				return;
			}
			bandsToMove.Sort(CompareDocumentBands);
			List<BandBricksPair> bottomBands2 = GetBottomBands(detailBands);
			if(bottomBands2.Count == 0)
				return;
			float bricksBottom = GetBricksBottom(bottomBands2[bottomBands2.Count - 1].Bricks);
			float emptySpace = GetBandTop(bandsToMove[0]) - bricksBottom;
			if(emptySpace <= 0f)
				return;
			List<BandBricksPair> remainBands = new List<BandBricksPair>(docBands);
			foreach(BandBricksPair item in bandsToMove)
				remainBands.Remove(item);
			DocumentBand moveDocumentBand2 = bandsToMove[0].Band;
			FillEmptySpace(psPage, emptySpace, bricksBottom);
		}
		static List<BandBricksPair> GetPrintAtBottomBands(List<BandBricksPair> docBands) {
			List<BandBricksPair> printAtBottomBands = new List<BandBricksPair>();
			foreach(BandBricksPair keyValuePair in docBands) {
				if(IsPrintAtBottomBand(keyValuePair.Band))
					printAtBottomBands.Add(keyValuePair);
			}
			return printAtBottomBands;
		}
		static List<BandBricksPair> GetDetailBands(List<BandBricksPair> docBands) {
			List<BandBricksPair> detailBands = new List<BandBricksPair>();
			foreach(BandBricksPair keyValuePair in docBands) {
				if(!IsPrintAtBottomBand(keyValuePair.Band))
					detailBands.Add(keyValuePair);
			}
			return detailBands;
		}
		static List<BandBricksPair> GetBandsToMove(List<BandBricksPair> printAtBottomBands, List<BandBricksPair> detailBands) {
			List<BandBricksPair> bandsToMove = new List<BandBricksPair>();
			foreach(BandBricksPair printAtBottomBand in printAtBottomBands) {
				if (ShouldMoveBottom(printAtBottomBand, detailBands)) {
					bandsToMove.Add(printAtBottomBand);
				}
			}
			return bandsToMove;
		}
		public void FillEmptySpace(PSPage psPage, float emptySpace, float vertOffset) {
			DocumentBand docBand = new DocumentBand(DocumentBandKind.Footer);
			ps.OnFillEmptySpace(new EmptySpaceEventArgs(psPage, docBand, emptySpace, vertOffset));
			AddBricks(psPage, docBand, vertOffset);
			foreach (DocumentBand child in docBand.Bands)
				AddBricks(psPage,child, vertOffset);
		}
		void AddBricks(PSPage psPage, DocumentBand docBand, float vertOffset) {
			float height = docBand.BrickBounds.Height;
			foreach (Brick brick in docBand.Bricks) {
				brick.Scale(ps.Document.ScaleFactor);
				brick.Height = height;
				brick.Initialize(ps, brick.Rect);
				brick.PageBuilderOffset = new PointF(docBand.OffsetX, vertOffset);
			}
			psPage.AddContent(docBand.Bricks);
		}
		static int CompareDocumentBands(BandBricksPair keyValuePair1, BandBricksPair keyValuePair2) {
			PointF location1 = GetBandLocation(keyValuePair1);
			PointF location2 = GetBandLocation(keyValuePair2);
			int result = FloatsComparer.Default.CompareDoubles(location1.Y, location2.Y);
			if(result != 0)
				return result;
			float bottom1 = GetBricksBottom(keyValuePair1.Bricks);
			float bottom2 = GetBricksBottom(keyValuePair2.Bricks);
			return FloatsComparer.Default.CompareDoubles(bottom1, bottom2);
		}
		static int CompareDocumentBands2(Pair<BandBricksPair, float> keyValuePair1, Pair<BandBricksPair, float> keyValuePair2) {
			return FloatsComparer.Default.CompareDoubles(keyValuePair1.Second, keyValuePair2.Second);
		}
		static void MoveBandsToBottom(List<BandBricksPair> bandsToMove, float emptySpace) {
			if(bandsToMove == null || bandsToMove.Count == 0 || emptySpace < 0)
				return;
			for(int i = 0; i < bandsToMove.Count; i++) {
				foreach(Brick brick in bandsToMove[i].Bricks) 
					brick.PageBuilderOffset = new PointF(brick.PageBuilderOffset.X, brick.PageBuilderOffset.Y + emptySpace);
			}
		}
		static bool ShouldMoveBottom(BandBricksPair bandForMove, List<BandBricksPair> detailBands) {
			float bandForMoveTop = GetBandTop(bandForMove);
			foreach(BandBricksPair detailBand in detailBands) {
				float detailBandTop = GetBandTop(detailBand);
				if(FloatsComparer.Default.FirstGreaterSecond(detailBandTop, bandForMoveTop))
					return false;
			}
			return true;
		}
		static PointF GetBandLocation(BandBricksPair item) {
			Brick brick = (Brick)item.Bricks[0];
			return new PointF(brick.Rect.Left - brick.DocumentBandRect.Left, brick.Rect.Top - brick.DocumentBandRect.Top);
		}
		static bool IsPrintAtBottomBand(DocumentBand docBand) {
			if(docBand == null)
				return false;
			if(docBand.PrintAtBottom || docBand.IsKindOf(DocumentBandKind.PageFooter))
				return true;
			return IsPrintAtBottomBand(docBand.PrimaryParent);
		}
		static float GetBottomSpanRecursive(DocumentBand docBand, float bottomSpan) {
			if(docBand == null)
				return bottomSpan;
			float currentBottomSpan = bottomSpan + docBand.BottomSpan;
			return GetBottomSpanRecursive(docBand.PrimaryParent, currentBottomSpan);
		}
		static List<BandBricksPair> GetBottomBands(List<BandBricksPair> docBands) {
			var sortableBands = new List<Pair<BandBricksPair, float>>();
			foreach(BandBricksPair item in docBands)
				sortableBands.Add(new Pair<BandBricksPair, float>(item, GetBandBottom(item)));
			sortableBands.Sort(CompareDocumentBands2);
			var result = new List<BandBricksPair>();
			for(int i = sortableBands.Count - 1; i >= 0; i--) {
				if(sortableBands[i].Second == sortableBands[docBands.Count - 1].Second)
					result.Add(sortableBands[i].First);
				else
					break;
			}
			return result;
		}
		static bool ContainsClippingBrickContainers(IList list) {
			foreach(Brick brick in list) {
				BrickContainer brickContiner = brick as BrickContainer;
				if(brickContiner != null) {
					return brickContiner.BrickOffsetX != 0
						|| brickContiner.BrickOffsetY != 0
						|| !FloatsComparer.Default.SizeFEquals(brickContiner.Rect.Size, brickContiner.Brick.Rect.Size);
				}
				return brick is BrickContainerBase;
			}
			return false;
		}
		static float GetBandBottom(BandBricksPair item) {
			if(ContainsClippingBrickContainers(item.Bricks))
				return GetBricksBottom(item.Bricks);
			return GetBandTop(item) + item.Band.SelfHeight;
		}
		static float GetBricksBottom(IList bricks) {
			float bottom = 0f;
			foreach(Brick brick in bricks)
				bottom = Math.Max(bottom, brick.Rect.Bottom);
			return bottom;
		}
		static float GetBandTop(BandBricksPair item) {
			return GetBandLocation(item).Y;
		}		
	}
}
