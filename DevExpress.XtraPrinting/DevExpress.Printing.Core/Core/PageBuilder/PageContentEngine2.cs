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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Native {
	public class YPageContentEngine2 : YPageContentEngine {
		#region inner classes
		protected class ContentAlgorithmByY2 : ContentAlgorithmByY {
			IDictionary<Brick, float> PrintedLengths { get { return ((YPageContentEngine2)pageContentEngine).PrintedLengths; } }
			public ContentAlgorithmByY2(YPageContentEngine2 pageContentEngine, DocumentBand documentBand, RectangleF bounds, bool forceSplit, IPrintingSystemContext ps)
				: base(pageContentEngine, documentBand, bounds, forceSplit, ps) {
			}
			protected override void FillPage(out List<Brick> newPageBricks, out List<Pair<Brick, Brick>> intersectBricks, out float maxBrickBound) {
				newPageBricks = new List<Brick>();
				intersectBricks = new List<Pair<Brick, Brick>>();
				maxBrickBound = MinBound;
				Brick previous = null;
				for(int i = 0; i < bricks.Count; i++) {
					Brick brick = bricks[i];
					if(!brick.CanAddToPage) continue;
					float printedHeight;
					if(TryGetValue(PrintedLengths, brick, out printedHeight) && brick.Rect.Height == printedHeight)
						continue;
					RectangleF rect = RectF.Offset(brick.InitialRect, bounds.X + offset.X, bounds.Y + offset.Y);
					if(RectContains(bounds, rect)) {
						if(brick.CanOverflow)
							brick = CreateBrickContainer(brick, brick.InitialRect);
						previous = brick;
						brick.PageBuilderOffset = new PointF(bounds.X + offset.X, bounds.Y + offset.Y);
						newPageBricks.Add(brick);
						PrintedLengths[brick] = rect.Height;
					} else if(bounds.IntersectsWith(rect) && brick is PanelBrick && ((PanelBrick)brick).Merged) {
						PanelBrick panel = (PanelBrick)brick.Clone();
						panel.InitialRect = RectangleF.Intersect(bounds, rect);
						panel.CenterChildControls();
						newPageBricks.Add(panel);
						PrintedLengths[brick] = rect.Height;
					} else if(bounds.IntersectsWith(rect)) {
						RectangleF rect1 = RectangleF.Intersect(bounds, rect);
						BrickContainer brick1 = CreateBrickContainer(brick, rect1);
						brick1.PageBuilderOffset = PointF.Empty;
						if(BetweenTopAndBottom(bounds.Top, rect))
							brick1.BrickOffsetY = rect.Y - bounds.Y;
						intersectBricks.Add(new Pair<Brick, Brick>(brick1, previous));
					}
				}
			}
			protected override void OnInterectedBrickAdded(Brick brick, float brickBound) {
				if(forceSplitY) return;
				float printedLength = CalcPrintedLength((BrickContainer)brick, brickBound);
				RectangleF rect = brick.Rect;
				rect.Height = ((BrickContainer)brick).BrickOffsetY + printedLength;
				float prevPrintedLength;
				if(TryGetValue(PrintedLengths, ((BrickContainer)brick).Brick, out prevPrintedLength)) {
					float brickHeight = printedLength - prevPrintedLength;
					float delta = rect.Height - brickHeight;
					rect.Y = rect.Bottom - brickHeight;
					((BrickContainer)brick).BrickOffsetY -= delta;
				}
				PrintedLengths[((BrickContainer)brick).Brick] = printedLength;
				brick.Rect = rect;
			}
			static bool TryGetValue(IDictionary<Brick, float> dictionary, Brick brick, out float value) {
				if(dictionary != null && dictionary.TryGetValue(brick, out value))
					return true;
				value = default(float);
				return false;
			}
			static float CalcPrintedLength(BrickContainer brick, float bottom) {
				return Math.Abs(brick.BrickOffsetY) + (bottom - brick.Rect.Top);
			}
		}
		#endregion
		IDictionary<Brick, float> PrintedLengths { get; set; }
		public YPageContentEngine2(PSPage psPage, PrintingSystemBase ps, YPageContentEngine2 previous)
			: base(psPage, ps) {
				if(previous != null) {
					PrintedLengths = previous.PrintedLengths;
				} else 
					PrintedLengths = new Dictionary<Brick, float>();
		}
		public override void OnBuildDocumentBand(DocumentBand docBand) {
			base.OnBuildDocumentBand(docBand);
			foreach(Brick brick in docBand.Bricks) {
				PrintedLengths.Remove(brick);
			}
		}
		protected override IPageContentAlgorithm CreateAlgorithm(DocumentBand docBand, RectangleF bounds, bool forceSplit) {
			return new ContentAlgorithmByY2(this, docBand, bounds, forceSplit, ps);
		}
	}
}
