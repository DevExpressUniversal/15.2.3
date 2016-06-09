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
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
#if SL
using DevExpress.Xpf.Collections;
#else
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export.Web;
#endif
namespace DevExpress.XtraPrinting.Native {
	public abstract class XPageContentEngine {
		public static XPageContentEngine CreateInstance(bool smartXDivision) {
			return smartXDivision && PrintingSettings.VerticalContentSplittingNewBehavior ? new SmartXPageContentEngine2() :
				smartXDivision ? new SmartXPageContentEngine() : (XPageContentEngine)new SimpleXPageContentEngine();
		}
		public abstract List<PSPage> CreatePages(PSPage source, RectangleF usefulArea, float usefulPageWidth);
	}
	public class SimpleXPageContentEngine : XPageContentEngine {
		public override List<PSPage> CreatePages(PSPage source, RectangleF usefulArea, float usefulPageWidth) {
			List<PSPage> docPages = new List<PSPage>();
			RectangleF rect = new RectangleF(0.0f, source.Rect.Top, usefulArea.Width, source.Rect.Height);
			float remainWidth = source.BricksSize.Width;
			float lastEffectiveWidth = 0;
			while(PageSizeAccuracyComaprer.Instance.FirstGreaterSecond(remainWidth, usefulPageWidth)) {
				PSPage addingPage = new PSPage(source.PageData, true);
				addingPage.Rect = rect;
				float effectiveWidth = AddBricksFrom(addingPage, source.Bricks);
				if(addingPage.Bricks.Count > 0) {
					if(docPages.Count > 0 && lastEffectiveWidth > 0.0) {
						float newEffectiveWidth = lastEffectiveWidth + effectiveWidth;
						if(newEffectiveWidth <= usefulPageWidth) {
							PSPage lastPage = docPages[docPages.Count - 1];
							if(lastPage != null) {
								AddAdditionalBricksFrom(lastPage, addingPage.Bricks);
								ValidateRect(lastPage, addingPage.Rect.Right);
								addingPage = null;
								lastEffectiveWidth = newEffectiveWidth;
							}
						}
					}
					if(addingPage != null) {
						docPages.Add(addingPage);
						lastEffectiveWidth = effectiveWidth;
					}
				} else
					lastEffectiveWidth = 0.0f;
				rect.X += effectiveWidth;
				remainWidth -= effectiveWidth;
			}
			source.Rect = rect;
			source.RemoveOuterBricks();
			docPages.Add(source);
			return docPages;
		}
	   protected virtual float AddBricksFrom(PSPage addingPage, BrickList bricks) {
			foreach(Brick brick in bricks)
				if(addingPage.Rect.IntersectsWith(brick.Rect))
					addingPage.Bricks.Add(brick);
			return addingPage.Rect.Width;
		}
		void AddAdditionalBricksFrom(PSPage addingPage, BrickList bricks) {
			foreach(Brick brick in bricks)
				if(!addingPage.Bricks.Contains(brick))
					addingPage.Bricks.Add(brick);
		}
		void ValidateRect(PSPage addingPage, float right) {
			RectangleF rect = addingPage.Rect;
			float diff = right - rect.Right;
			rect.Width += diff;
			addingPage.Rect = rect;
		}
	}
	public class SmartXPageContentEngine : SimpleXPageContentEngine {
		#region inner classes
		class ContentAlgorithmByX : ContentAlgorithmBase {
			protected internal ContentAlgorithmByX(PSPage page, BrickList bricks)
				: base(page, bricks, page.Rect) {
			}
			protected override void FillPage(out List<Brick> newPageBricks, out List<Pair<Brick, Brick>> intersectBricks, out float maxBrickBound) {
				newPageBricks = new List<Brick>();
				intersectBricks = new List<Pair<Brick, Brick>>();
				maxBrickBound = MinBound;
				Brick previous = page.Bricks.GetLast<Brick>();
				foreach(Brick brick in bricks) {
					if(brick.CanAddToPage && IntersectFunction(bounds, brick.Rect)) {
						RectangleF brickRect = CorrectBrickRect(brick.Rect);
						if(ContainsFunction(bounds, brickRect)) {
							previous = brick;
							newPageBricks.Add(brick);
							maxBrickBound = Math.Max(maxBrickBound, GetMaxBrickBound(brick));
						} else
							intersectBricks.Add(new Pair<Brick, Brick>(brick, previous));
					}
				}
			}
			RectangleF CorrectBrickRect(RectangleF brickRect) {
				if(brickRect.Left < bounds.Left) {
					brickRect.Width -= bounds.Left - brickRect.Left;
					brickRect.X = bounds.Left;
				}
				return brickRect;
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
				float right = brick.ValidatePageRight(bounds.Right, brick.Rect);
				if(right <= bounds.Left)
					return bounds.Right;
				if(!forceSplit && !brick.SeparableHorz && right > bounds.Left)
					return right;
				float evalRight = right;
				return FloatsComparer.Default.FirstGreaterSecond(evalRight, maxBrickBound) ||
					FloatsComparer.Default.FirstEqualsSecond(evalRight, maxBrickBound) ? right : bounds.Right;
			}
			public float Process() {
				ProcessCore();
				page.Rect = bounds;
				return bounds.Width;
			}
		}
		#endregion
	   protected override float AddBricksFrom(PSPage addingPage, BrickList bricks) {
			return new ContentAlgorithmByX(addingPage, bricks).Process();
	   }
	}
	public class BandBricksPair {
		public DocumentBand Band { get; private set; }
		public IList Bricks { get; private set; }
		public BandBricksPair(DocumentBand band, IList bricks) {
			Band = band;
			Bricks = bricks;
		}
	}
	public class YPageContentEngine {
		#region inner classes
		protected class ContentAlgorithmByY : ContentAlgorithmBase, IPageContentAlgorithm {
			protected bool forceSplitY = false;
			protected IPrintingSystemContext context;
			protected YPageContentEngine pageContentEngine;
			DocumentBand documentBand;
			#region IPageContentAlgorithm Members
			bool IPageContentAlgorithm.Process(PSPage page, DocumentBand documentBand, RectangleF bounds, PointF offset) {
				return Process(offset);
			}
			RectangleF IPageContentAlgorithm.Bounds {
				get { return bounds; }
			}
			IList IPageContentAlgorithm.AddedPageBricks {
				get { return addedPageBricks; }
			}
			#endregion
			public ContentAlgorithmByY(YPageContentEngine pageContentEngine, DocumentBand documentBand, RectangleF bounds, bool forceSplit, IPrintingSystemContext ps)
				: base(pageContentEngine.psPage, documentBand.Bricks, bounds) {
				this.documentBand = documentBand;
				this.pageContentEngine = pageContentEngine;
				this.forceSplitY = forceSplit;
				context = ps;
			}
			protected override void FillPage(out List<Brick> newPageBricks, out List<Pair<Brick, Brick>> intersectBricks, out float maxBrickBound) {
				newPageBricks = new List<Brick>();
				intersectBricks = new List<Pair<Brick, Brick>>();
				maxBrickBound = MinBound;
				Brick previous = page.Bricks.GetLast<Brick>();
				for(int i = 0; i < bricks.Count; i++) {
					Brick brick = bricks[i];
					if(!brick.CanAddToPage) continue;
					RectangleF rect = RectF.Offset(brick.InitialRect, bounds.X + offset.X, bounds.Y + offset.Y);
					if(RectContains(bounds, rect)) {
						if(brick.CanOverflow)
							brick = CreateBrickContainer(brick, brick.InitialRect);
						previous = brick;
						brick.PageBuilderOffset = new PointF(bounds.X + offset.X, bounds.Y + offset.Y);
						newPageBricks.Add(brick);
						maxBrickBound = Math.Max(maxBrickBound, GetMaxBrickBound(brick));
					} else if(bounds.IntersectsWith(rect) && brick is PanelBrick && ((PanelBrick)brick).Merged) {
						PanelBrick panel = (PanelBrick)brick.Clone();
						panel.InitialRect = RectangleF.Intersect(bounds, rect);
						panel.CenterChildControls();
						newPageBricks.Add(panel);
					} else if(bounds.IntersectsWith(rect)) {
						RectangleF rect1 = RectangleF.Intersect(bounds, rect);
						BrickContainer brick1 = CreateBrickContainer(brick, rect1);
						brick1.PageBuilderOffset = PointF.Empty;
						if(BetweenTopAndBottom(bounds.Bottom, rect)) {
							if(BetweenTopAndBottom(bounds.Top, rect))
								brick1.BrickOffsetY = rect.Y - bounds.Y;
							intersectBricks.Add(new Pair<Brick, Brick>(brick1, previous));
						} else {
							brick1.BrickOffsetY = rect.Y - bounds.Y;
							previous = brick1;
							newPageBricks.Add(brick1);
							maxBrickBound = Math.Max(maxBrickBound, GetMaxBrickBound(brick1));
						}
					}
				}
			}
			protected static bool BetweenTopAndBottom(float value, RectangleF rect) {
				return value > rect.Top && value < (float)rect.Bottom;
			}
			protected static bool RectContains(RectangleF baseRect, RectangleF rect) {
				if((baseRect.X <= rect.X) && ((float)(rect.X + rect.Width) <= (float)(baseRect.X + baseRect.Width)) && (baseRect.Y <= rect.Y)) {
					float bottom = (float)Math.Round((float)(rect.Y + rect.Height), 2);
					float baseBottom = (float)Math.Round((float)(baseRect.Y + baseRect.Height), 2);
					return bottom <= baseBottom;
				}
				return false;
			}
			protected override void OnInterectedBrickAdded(Brick brick, float brickBound) {
				if(!forceSplitY)
					brick.Rect = RectangleF.Intersect(bounds, brick.Rect);
			}
			protected override float MinBound {
				get {
					return bounds.Top;
				}
			}
			protected override float MaxBound {
				get {
					return bounds.Bottom;
				}
				set {
					bounds.Height = value - bounds.Y;
				}
			}
			protected override float GetMaxBrickBound(Brick brick) {
				return brick.Rect.Bottom;
			}
			protected override bool IsSeparable(Brick brick) {
				return brick.SeparableVert;
			}
			protected override bool IntersectFunction(RectangleF rect1, RectangleF rect2) {
				return RectFBase.IntersectByY(rect1, rect2);
			}
			protected override bool ContainsFunction(RectangleF rect1, RectangleF rect2) {
				return RectFBase.ContainsByY(rect1, rect2);
			}
			protected override float GetMinBound(IDictionary<Brick, float> boundList) {
				float value = MaxBound;
				foreach(var bound in boundList) {
					if(BetweenTopAndBottom(bound.Key.Rect.Bottom, bounds))
						continue;
					value = Math.Min(value, bound.Value);
				}
				return value;
			}
			protected override float GetBrickBound(Brick brick, bool forceSplit, float maxBrickBound) {
				if(BetweenTopAndBottom(brick.Rect.Bottom,  bounds))
					return brick.Rect.Bottom;
				bool hasMeat = new BandContentAnalyzer(pageContentEngine.AddedBands).ExistsDetailBand(documentBand, brick.Rect);
				float bottom = brick.ValidatePageBottom(bounds, !hasMeat, brick.Rect, context);
				if(hasMeat)
					return bottom;
				if(forceSplitY)
					return bounds.Bottom;
				if(bottom <= bounds.Top)
					return bounds.Bottom;
				if(!forceSplit && !brick.SeparableVert && bottom > bounds.Top)
					return bottom;
				return bottom >= maxBrickBound ? bottom : bounds.Bottom;
			}
			public bool Process() {
				int bricksCount = page.Bricks.Count;
				ProcessCore();
				return page.Bricks.Count > bricksCount;
			}
			public bool Process(PointF offset) {
				this.offset = offset;
				return Process();
			}
		}
		#endregion
		PSPage psPage;
		protected IPrintingSystemContext ps;
		List<BandBricksPair> addedDocBands = new List<BandBricksPair>();
		public PrintingSystemBase PrintingSystem {
			get { return ps.PrintingSystem; }
		}
		public Observable<bool> Stopped {
			get;  
			private set;
		}
		public Observable<bool> BuildInfoIncreased {
			get;
			private set;
		}
		public virtual int PageBricksCount {
			get { return psPage.Bricks.Count; }
		}
		public PSPage Page { get { return psPage; } }
		public YPageContentEngine(PSPage psPage, IPrintingSystemContext ps) {
			this.psPage = psPage;
			this.ps = ps;
			Stopped = new Observable<bool>(false);
			BuildInfoIncreased = new Observable<bool>(false);
		}
		public List<BandBricksPair> AddedBands {
			get { return addedDocBands; }
		}
		public DocumentBand LastAddedBand {
			get {
				return addedDocBands.Count > 0 ? addedDocBands[addedDocBands.Count - 1].Band : null;
			}
		}
		public List<BandBricksPair> GetAddedBands(int markedBandID) {
			List<BandBricksPair> docBands = new List<BandBricksPair>();
			if(markedBandID >= 0) {
				for(int i = addedDocBands.Count - 1; i >= 0; i--) {
					if(addedDocBands[i].Band.ID == markedBandID)
						break;
					docBands.Add(addedDocBands[i]);
				}
			}
			return docBands;
		}
		public virtual PageUpdateData UpdateContent(DocumentBand docBand, PointD offset, RectangleF bounds, bool forceSplit) {
			PageUpdateData pageUpdateData = new PageUpdateData(bounds);
			if(offset.Y + bounds.Y < bounds.Bottom) {
				IPageContentAlgorithm al = GetPageContentAlgorithm(docBand, () => CreateAlgorithm(docBand, bounds, forceSplit));
				pageUpdateData.IsUpdated = al.Process(this.Page, docBand, bounds, offset.ToPointF());
				pageUpdateData.Bounds = al.Bounds;
				if(al.AddedPageBricks.Count > 0)
					addedDocBands.Add(new BandBricksPair(docBand, al.AddedPageBricks));
			}
			return pageUpdateData;
		}
		public virtual void OnBuildDocumentBand(DocumentBand docBand) { 
		}
		protected virtual IPageContentAlgorithm CreateAlgorithm(DocumentBand docBand, RectangleF bounds, bool forceSplit) {
			return new ContentAlgorithmByY(this, docBand, bounds, forceSplit, ps);
		}
		IPageContentAlgorithm GetPageContentAlgorithm(DocumentBand docBand, Func<IPageContentAlgorithm> callback) {
			IPageContentService serv = ((IServiceProvider)ps).GetService(typeof(IPageContentService)) as IPageContentService;
			return serv != null ? serv.GetAlgorithm(docBand) ?? callback() : callback(); 
		}
		public virtual void CorrectPrintAtBottomBricks(List<BandBricksPair> docBands, float pageBottom, bool ignoreBottomSpan) {
			PrintAtBottomHelper.CorrectPrintAtBottomBricks(docBands, pageBottom, this.psPage, ignoreBottomSpan);
		}
		public void ResetObservableItems() {
			Stopped.Reset();
			BuildInfoIncreased.Reset();
			BuildInfoIncreased.Value = false;
		}
	}
	public class BandContentAnalyzer {
		IEnumerable<BandBricksPair> docBands;
		public BandContentAnalyzer(IEnumerable<BandBricksPair> docBands) {
			this.docBands = docBands;
		}
		public bool ExistsDetailBand(DocumentBand documentBand, RectangleF rectangle) {
			DocumentBand subRoot = documentBand.GetSubRoot();
			foreach(BandBricksPair item in docBands) {
				if(!item.Band.IsKindOf(DocumentBandKind.Detail))
					continue;
				DocumentBand topLevelBand = item.Band.FindTopLevelBand();
				if(topLevelBand != null && topLevelBand.IsKindOf(DocumentBandKind.Detail) && subRoot != topLevelBand.GetSubRoot()) {
					foreach(Brick brick in item.Bricks) {
						RectangleF brickRect = brick.Rect;
						if((brickRect.Top < rectangle.Top &&
							(brickRect.Left <= rectangle.Left && rectangle.Left < brickRect.Right ||
							brickRect.Left < rectangle.Right && rectangle.Right <= brickRect.Right)))
							return true;
					}
				}
			}
			return false;
		}
		public bool ExistsPrimaryContentAbove(float bound) {
			bool result = false;
			foreach(BandBricksPair item in docBands) {
				if(IsPrimaryBand(item.Band) && !HasPageBandInTree(item.Band)) {
					result = true;
					foreach(Brick brick in item.Bricks)
						if(AboveComparison(brick.Rect, bound))
							return false;
				}
			}
			return result;
		}
		protected virtual bool AboveComparison(RectangleF rect, float bound) {
			return FloatsComparer.Default.FirstGreaterSecond(rect.Bottom, bound);
		}
		public bool ExistsPrimaryContentBellow(float bound) {
			foreach(BandBricksPair item in docBands) {
				if(IsPrimaryBand(item.Band) && !HasPageBandInTree(item.Band)) {
					foreach(Brick brick in item.Bricks)
						if(FloatsComparer.Default.FirstGreaterSecond(brick.Rect.Bottom, bound))
							return true;
				}
			}
			return false;
		}
		static bool IsPrimaryBand(DocumentBand item) {
			bool result = item.IsKindOf(DocumentBandKind.Detail, DocumentBandKind.ReportHeader, DocumentBandKind.ReportFooter);
			result |= item.IsKindOf(DocumentBandKind.Header) && !item.KeepTogetherOnTheWholePage && !item.IsFriendLevelSet;
			return result;
		}
		static bool HasPageBandInTree(DocumentBand documentBand) {
			if(documentBand == null)
				return false;
			if(documentBand.IsPageBand(DocumentBandKind.Header))
				return true;
			return HasPageBandInTree(documentBand.Parent);
		}
	}
	public abstract class ContentAlgorithmBase {
		protected PSPage page;
		protected IListWrapper<Brick> bricks;
		protected RectangleF initialBounds;
		protected RectangleF bounds;
		protected ArrayList addedPageBricks = new ArrayList();
		protected PointF offset;
		protected ContentAlgorithmBase(PSPage page, IListWrapper<Brick> bricks, RectangleF bounds) {
			this.page = page;
			this.bricks = bricks;
			this.initialBounds = this.bounds = bounds;
		}
		protected abstract float MinBound { get; }
		protected abstract float MaxBound { get; set; }
		protected abstract float GetMaxBrickBound(Brick brick);
		protected abstract bool IsSeparable(Brick brick);
		protected abstract bool IntersectFunction(RectangleF rect1, RectangleF rect2);
		protected abstract bool ContainsFunction(RectangleF rect1, RectangleF rect2);
		protected abstract float GetBrickBound(Brick brick, bool forceSplit, float maxBrickBound);
		protected abstract void FillPage(out List<Brick> newPageBricks, out List<Pair<Brick, Brick>> intersectBricks, out float maxBrickBound);
		protected BrickContainer CreateBrickContainer(Brick brick, RectangleF rect) {
			BrickContainer brickContainer = new BrickContainer(brick);
			System.Diagnostics.Debug.Assert(brick.PrintingSystem != null);
			brickContainer.Initialize(brick.PrintingSystem, rect);
			return brickContainer;
		}
		IDictionary<Brick, float> GetBrickBounds(IEnumerable<Brick> bricks, bool forceSplit, float maxBrickBound) {
			Dictionary<Brick, float> list = new Dictionary<Brick, float>();
			foreach(Brick brick in bricks)
				list[brick] = GetBrickBound(brick, forceSplit, maxBrickBound);
			return list;
		}
		protected virtual float GetMinBound(IDictionary<Brick, float> boundList) {
			float value = MaxBound;
			foreach(var bound in boundList)
				value = Math.Min(value, bound.Value);
			return value;
		}
		protected void FillPageAdditional(IEnumerable<Pair<Brick, Brick>> intersectBricks, Func<Brick, float> callback) {
			Pair<Brick, Brick> lastInserted = null;
			foreach(Pair<Brick, Brick> pair in intersectBricks) {
				Brick brick = pair.First;
				if(IntersectFunction(bounds, brick.Rect)) {
					Brick previous = pair.Second;
					if(lastInserted != null && (previous == null || previous == lastInserted.Second))
						previous = lastInserted.First;
					page.InsertAfter(brick, previous);
					addedPageBricks.Add(brick);
					OnInterectedBrickAdded(brick, callback(brick));
					lastInserted = pair;
				}
			}
		}
		protected virtual void OnInterectedBrickAdded(Brick brick, float brickBound) {
		}
		protected virtual void AddBricks(List<Brick> bricks) {
			page.AddContent(bricks);
			this.addedPageBricks.AddRange(bricks);
		}
		protected void ProcessCore() {
			float maxBrickBound;
			List<Brick> newPageBricks;
			List<Pair<Brick, Brick>> intersectBricks;
			if(TryFillPage(out newPageBricks, out intersectBricks, out maxBrickBound))
				return;
			IEnumerable<Brick> en = intersectBricks.ConvertAll<Pair<Brick, Brick>, Brick>(x => x.First);
			IDictionary<Brick, float> boundList = GetBrickBounds(en, false, maxBrickBound);
			if(TryFillPageAdditional(newPageBricks, intersectBricks, boundList, maxBrickBound, false))
				return;
			float newBound = GetMinBound(boundList);
			MaxBound = newBound;
			if(newBound != MinBound) {
				float maxBrickBound2;
				List<Brick> newPageBricks2;
				List<Pair<Brick, Brick>> intersectBricks2;
				if(TryFillPage(out newPageBricks2, out intersectBricks2, out maxBrickBound2))
					return;
				IEnumerable<Brick> en2 = intersectBricks2.ConvertAll<Pair<Brick, Brick>, Brick>(x => x.First);
				IDictionary<Brick, float> boundList2 = GetBrickBounds(en2, false, maxBrickBound2);
				if(TryFillPageAdditional(newPageBricks2, intersectBricks2, boundList2, maxBrickBound2, false))
					return;
			}
			bounds = initialBounds;
			boundList = GetBrickBounds(en, true, maxBrickBound);
			TryFillPageAdditional(newPageBricks, intersectBricks, boundList, maxBrickBound, true);
		}
		bool TryFillPage(out List<Brick> newPageBricks, out List<Pair<Brick, Brick>> intersectBricks, out float maxBrickBound) {
			FillPage(out newPageBricks, out intersectBricks, out maxBrickBound);
			if(intersectBricks.Count == 0 && newPageBricks.Count > 0) {
				AddBricks(newPageBricks);
				return true;
			}
			return false;
		}
		bool TryFillPageAdditional(List<Brick> newPageBricks, List<Pair<Brick, Brick>> intersectBricks, IDictionary<Brick, float> boundList, float maxBrickBound, bool forceSplit) {
			float newBound = GetMinBound(boundList);
			if(newBound != MinBound && newBound >= maxBrickBound) {
				MaxBound = newBound;
				AddBricks(newPageBricks);
				FillPageAdditional(intersectBricks, brick => boundList[brick]);
				return true;
			} else if(forceSplit)
				MaxBound = newBound;
			return false;
		}
	}
	public class PageUpdateData {
		public bool IsUpdated;
		public RectangleF Bounds = RectangleF.Empty;
		public PageUpdateData(RectangleF bounds)
			: this(bounds, false) {
		}
		public PageUpdateData(RectangleF bounds, bool isUpdated) {
			this.Bounds = bounds;
			this.IsUpdated = isUpdated;
		}
		public PageUpdateData() {
		}
	}
}
