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
using System.Linq;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Native {
	class MergeBrickHelper {
		public MergeBrickHelper() {
		}
		public void ProcessBricks(PrintingSystemBase ps, Dictionary<Brick, RectangleDF> bricks) {
			OnStartProcess(null);
			Dictionary<object, List<BrickLayoutInfo>> brickListContainer = new Dictionary<object, List<BrickLayoutInfo>>();
			foreach(var exportBrick in bricks) {
				double offsetX = exportBrick.Value.Left - exportBrick.Key.Location.X;
				double offsetY = exportBrick.Value.Top - exportBrick.Key.Location.Y;
				NestedBrickIterator iterator = new NestedBrickIterator(new[] { exportBrick.Key });
				while(iterator.MoveNext()) {
					VisualBrick brick = iterator.CurrentBrick as VisualBrick;
					if(brick != null) {
						RectangleF rect = iterator.CurrentBrickRectangle;
						if(!iterator.CurrentClipRectangle.IsEmpty)
							rect.Intersect(iterator.CurrentClipRectangle);
						UpdateMergeBricks(brickListContainer, brick, RectangleDF.Offset( RectangleDF.FromRectangleF(rect), offsetX, offsetY));
					}
				}
			}
			Action<Brick, RectangleF> addBrick = (brick, rect) => {
				brick.Initialize(ps, RectangleF.Empty);
				brick.SetBounds(rect, GraphicsDpi.Document);
				bricks.Add(brick, RectangleDF.FromRectangleF(rect));
			};
			foreach(var pair in brickListContainer)
				MergeBricks(pair.Value, addBrick, null);
			OnEndProcess(null);
		}
		public void ProcessPage(PrintingSystemBase ps, PSPage page) {
			if(page.InnerBrickList.Count == 0)
				return;
			OnStartProcess(page);
			Dictionary<object, BrickLayoutInfo> marginalHeaderInfo = null;
			Dictionary<object, BrickLayoutInfo> marginalFooterInfo = null;
			for(int i = 0; i < page.InnerBrickList.Count; i++) {
				CompositeBrick innerPageBrick = page.InnerBrickList[i] as CompositeBrick;
				if(innerPageBrick.Modifier == BrickModifier.MarginalHeader)
					marginalHeaderInfo = CreateBrickInfos(innerPageBrick);
				else if(innerPageBrick.Modifier == BrickModifier.MarginalFooter)
					marginalFooterInfo = CreateBrickInfos(innerPageBrick);
			}
			for(int i = 0; i < page.InnerBrickList.Count; i++) {
				CompositeBrick innerPageBrick = page.InnerBrickList[i] as CompositeBrick;
				if(innerPageBrick.Modifier == BrickModifier.MarginalHeader || innerPageBrick.Modifier == BrickModifier.MarginalFooter)
					continue;
				Dictionary<object, List<BrickLayoutInfo>> brickListContainer = new Dictionary<object, List<BrickLayoutInfo>>();
				NestedBrickIterator iterator = new NestedBrickIterator(innerPageBrick.InnerBrickList);
				while(iterator.MoveNext()) {
					VisualBrick brick = iterator.CurrentBrick as VisualBrick;
					if(brick != null) {
						RectangleF rect = iterator.CurrentBrickRectangle;
						if(!iterator.CurrentClipRectangle.IsEmpty)
							rect.Intersect(iterator.CurrentClipRectangle);
						UpdateMergeBricks(brickListContainer, brick, RectangleDF.FromRectangleF(rect));
					}
				}
				Action<Brick, RectangleF> addBrick = (brick, rect) => {
					brick.Initialize(ps, RectangleF.Empty);
					brick.SetBounds(rect, GraphicsDpi.Document);
					innerPageBrick.InnerBrickList.Add(brick);
				};
				foreach(var pair in brickListContainer)
					MergeBricks(pair.Value, addBrick, page);
				if(marginalHeaderInfo != null && marginalFooterInfo != null) {
					foreach(var pair in marginalHeaderInfo) {
						if(brickListContainer.ContainsKey(pair.Key) || !marginalFooterInfo.ContainsKey(pair.Key))
							continue;
						VisualBrick newBrick = (VisualBrick)pair.Value.Brick.Clone();
						RectangleF rect = pair.Value.Rect.ToRectangleF();
						addBrick(newBrick, new RectangleF(rect.X, 0, rect.Width, innerPageBrick.Height));
					}
				}
			}
			OnEndProcess(page);
		}
		static Dictionary<object, BrickLayoutInfo> CreateBrickInfos(CompositeBrick innerPageBrick) {
			Dictionary<object, BrickLayoutInfo> brickInfo = new Dictionary<object, BrickLayoutInfo>();
			NestedBrickIterator iterator = new NestedBrickIterator(innerPageBrick.InnerBrickList);
			while(iterator.MoveNext()) {
				VisualBrick brick = iterator.CurrentBrick as VisualBrick;
				if(brick != null) {
					RectangleF rect = iterator.CurrentBrickRectangle;
					if(!iterator.CurrentClipRectangle.IsEmpty)
						rect.Intersect(iterator.CurrentClipRectangle);
					UpdateBrickInfos(brickInfo, brick, RectangleDF.FromRectangleF(rect));
				}
			}
			return brickInfo;
		}
		static void UpdateBrickInfos(Dictionary<object, BrickLayoutInfo> brickInfo, VisualBrick visualBrick, RectangleDF brickRect) {
			object mergeValue = null;
			if(TryGetMergeValue(visualBrick, out mergeValue)) {
				mergeValue = CombineMergeValue(mergeValue, brickRect);
				brickInfo[mergeValue] = new BrickLayoutInfo(visualBrick, brickRect);
			}
		}
		void UpdateMergeBricks(Dictionary<object, List<BrickLayoutInfo>> brickListContainer, VisualBrick visualBrick, RectangleDF brickRect) {
			object mergeValue = null;
			if(TryGetMergeValue(visualBrick, out mergeValue)) {
				mergeValue = CombineMergeValue(mergeValue, brickRect);
				List<BrickLayoutInfo> bricks = null;
				if(!brickListContainer.TryGetValue(mergeValue, out bricks)) {
					bricks = new List<BrickLayoutInfo>();
					brickListContainer[mergeValue] = bricks;
				}
				bricks.Add(new BrickLayoutInfo(visualBrick, brickRect));
			}
		}
		protected virtual void OnStartProcess(PSPage page) {
		}
		protected virtual void OnEndProcess(PSPage page) {
		}
		static object CombineMergeValue(object mergeValue, RectangleDF brickRect) {
			const int roundDigits = 2;
			return new MultiKey(mergeValue, Math.Round(brickRect.X, roundDigits), Math.Round(brickRect.Width, roundDigits));
		}
		static bool TryGetMergeValue(VisualBrick brick, out object value) {
			return brick.TryGetAttachedValue(BrickAttachedProperties.MergeValue, out value);
		}
		protected RectangleF GetUnionRect(List<BrickLayoutInfo> brickInfos) {
			PointF leftTop = new PointF(float.MaxValue, float.MaxValue);
			PointF rightBottom = new PointF(float.MinValue, float.MinValue);
			brickInfos.ForEach(item => {
				RectangleF brickRect = item.Rect.ToRectangleF();
				CalculateLTRB(brickRect, ref leftTop, ref rightBottom);
			});
			return RectangleF.FromLTRB(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);
		}
		protected void CalculateLTRB(RectangleF rect, ref PointF leftTop, ref PointF rightBottom) {
			if(rect.Left < leftTop.X)
				leftTop.X = rect.Left;
			if(rect.Top < leftTop.Y)
				leftTop.Y = rect.Top;
			if(rect.Right > rightBottom.X)
				rightBottom.X = rect.Right;
			if(rect.Bottom > rightBottom.Y)
				rightBottom.Y = rect.Bottom;
		}
		protected virtual void MergeBricks(List<BrickLayoutInfo> bricksToMerge, Action<Brick, RectangleF> addBrick, PSPage page) {
			if(bricksToMerge.Count < 1)
				return;
			bricksToMerge.Sort((x, y) => {
				return Comparer<double>.Default.Compare(x.Rect.Top, y.Rect.Top);
			});
			VisualBrick prototypeBrick = (VisualBrick)bricksToMerge.First().Brick;
			System.Diagnostics.Debug.Assert(prototypeBrick != null);
			if(prototypeBrick == null)
				return;
			RectangleF unionRect = GetUnionRect(bricksToMerge);
			if(FloatsComparer.Default.FirstLessSecond(unionRect.Height, prototypeBrick.Height)) {
				prototypeBrick.IsVisible = true;
				return;
			}
			VisualBrick newBrick = (VisualBrick)prototypeBrick.Clone();
			addBrick(newBrick, unionRect);
			newBrick.OnAfterMerge();
			newBrick.BrickOwner.AddToSummaryUpdater(newBrick, prototypeBrick);
			bricksToMerge.ForEach(item => { if(item.Brick != null) item.Brick.IsVisible = false; });
		}
	}
}
