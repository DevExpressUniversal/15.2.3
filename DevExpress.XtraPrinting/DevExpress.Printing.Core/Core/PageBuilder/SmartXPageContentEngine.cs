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
namespace DevExpress.XtraPrinting.Native {
	class SmartXPageContentEngine2 : XPageContentEngine {
		#region inner classes
		class ContentAlgorithmByX : ContentAlgorithmBase {
			IDictionary<Brick, float> printedLengths;
			protected internal ContentAlgorithmByX(PSPage page, BrickList bricks, IDictionary<Brick, float> printedLengths)
				: base(page, bricks, page.Rect) {
				this.printedLengths = printedLengths;
			}
			protected override void FillPage(out List<Brick> newPageBricks, out List<Pair<Brick, Brick>> intersectBricks, out float maxBrickBound) {
				newPageBricks = new List<Brick>();
				intersectBricks = new List<Pair<Brick, Brick>>();				
				maxBrickBound = MinBound;
				Brick previous = null;
				foreach(Brick brick in bricks) {
					float printedLength;
					RectangleF brickRect = brick.Rect;
					if(printedLengths.TryGetValue(brick.GetRealBrick(), out printedLength) && brickRect.Width == printedLength)
							continue;
					if(brick.CanAddToPage && IntersectFunction(bounds, brickRect)) {
						if(ContainsFunction(bounds, brickRect)) {
							previous = brick;
							newPageBricks.Add(brick);
						} else {
							RectangleF restBrickRect = brickRect;
							restBrickRect.X += printedLength;
							restBrickRect.Width -= printedLength;
							RectangleF rect1 = RectangleF.Intersect(bounds, restBrickRect);
							BrickContainer brick1 = CreateBrickContainer(brick.GetRealBrick(), rect1);
							brick1.PageBuilderOffset = PointF.Empty;
							brick1.BrickOffsetX = -printedLength;
							intersectBricks.Add(new Pair<Brick, Brick>(brick1, previous));
						}
					}
				}
			}
			static bool BetweenLeftAndRight(float value, RectangleF rect) {
				return value > rect.Left && value < (float)rect.Right;
			}
			protected override void AddBricks(List<Brick> bricks) {
				base.AddBricks(bricks);
				foreach(Brick brick in bricks)
					printedLengths[brick.GetRealBrick()] = brick.Rect.Width;
			}
			protected override void OnInterectedBrickAdded(Brick brick, float brickBound) {
				float printedLength = CalcPrintedLength((BrickContainer)brick, brickBound);
				RectangleF rect = brick.Rect;
				rect.Width = ((BrickContainer)brick).BrickOffsetX + printedLength;
				float prevPrintedLength;
				if(printedLengths.TryGetValue(brick.GetRealBrick(), out prevPrintedLength)) {
					float brickWidth = printedLength - prevPrintedLength;
					float delta = rect.Width - brickWidth;
					rect.X = rect.Right - brickWidth;
					((BrickContainer)brick).BrickOffsetX -= delta;
				}
				printedLengths[brick.GetRealBrick()] = printedLength;
				brick.Rect = rect;
			}
			static float CalcPrintedLength(BrickContainer brick, float right) {
				return Math.Abs(brick.BrickOffsetX) + (right - brick.Rect.Left);
			}
			protected override float MinBound {
				get {
					return bounds.Left;
				}
			}
			protected override float MaxBound {
				get {
					return bounds.Right;
				}
				set {
					bounds.Width = value - bounds.X;
				}
			}
			protected override float GetMaxBrickBound(Brick brick) {
				return brick.Rect.Right;
			}
			protected override bool IsSeparable(Brick brick) {
				return brick.SeparableHorz;
			}
			protected override bool IntersectFunction(RectangleF rect1, RectangleF rect2) {
				return RectFBase.IntersectByX(rect1, rect2);
			}
			protected override bool ContainsFunction(RectangleF rect1, RectangleF rect2) {
				return RectFBase.ContainsByX(rect1, rect2);
			}
			protected override float GetBrickBound(Brick brick, bool forceSplit, float maxBrickBound) {
				if(forceSplit)
					return bounds.Right;				
				float right = brick.ValidatePageRight(bounds.Right, brick.Rect);
				if(FloatsComparer.Default.FirstEqualsSecond(brick.Rect.Left, right)) {
					RectangleF rect = brick.GetRealBrick().Rect;
					float width = brick.GetRealBrick().Rect.Right - brick.Rect.Left;
					if(FloatsComparer.Default.FirstGreaterOrEqualSecond(width, bounds.Width) || printedLengths.ContainsKey(brick.GetRealBrick()))
						return bounds.Right;
				}
				if(!forceSplit && right > bounds.Left)
					return right;
				float evalRight = right;
				return FloatsComparer.Default.FirstGreaterOrEqualSecond(evalRight, maxBrickBound) ? right : bounds.Right;
			}
			public float Process() {
				ProcessCore();
				return bounds.Width;
			}
		}
		#endregion
		IDictionary<Brick, float> printedLengths;
		public SmartXPageContentEngine2() {
			printedLengths = new Dictionary<Brick, float>();
		}
		public override List<PSPage> CreatePages(PSPage source, RectangleF usefulArea, float usefulPageWidth) {
			printedLengths.Clear();
			List<PSPage> docPages = new List<PSPage>();
			RectangleF rect = new RectangleF(0.0f, source.Rect.Top, usefulArea.Width, source.Rect.Height);
			float remainWidth = source.BricksSize.Width;
			float lastEffectiveWidth = 0;
			while(PageSizeAccuracyComaprer.Instance.FirstGreaterSecond(remainWidth, 0)) {
				PSPage addingPage = new PSPage(source.PageData, true);
				addingPage.Rect = rect;
				addingPage.NoClip = true;
				float effectiveWidth = AddBricksFrom(addingPage, source.Bricks);
				if(addingPage.Bricks.Count > 0) {
					docPages.Add(addingPage);
					lastEffectiveWidth = effectiveWidth;
				} else
					lastEffectiveWidth = 0.0f;
				rect.X += effectiveWidth;
				remainWidth -= effectiveWidth;
			}
			source.Rect = rect;
			source.RemoveOuterBricks();
			return docPages;
		}
		float AddBricksFrom(PSPage addingPage, BrickList bricks) {
			return new ContentAlgorithmByX(addingPage, bricks, printedLengths).Process();
		}
	}
}
