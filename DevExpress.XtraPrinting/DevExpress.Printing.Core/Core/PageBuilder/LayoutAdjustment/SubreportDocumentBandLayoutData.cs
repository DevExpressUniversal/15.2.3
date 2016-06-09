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
using DevExpress.XtraReports.UI;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using System.Collections;
namespace DevExpress.XtraPrinting.Native.LayoutAdjustment
{
	public class SubreportDocumentBandLayoutData : ILayoutData {
		ISubreportDocumentBand docBand;
		float bottom;
		float top;
		protected LayoutDataContext context;
		RectangleF initialRect;
		VerticalAnchorStyles ILayoutData.AnchorVertical { get { return VerticalAnchorStyles.None; } }
		bool ILayoutData.NeedAdjust { get { return true; } }
		public float Top { get { return top; } }
		public float Bottom { get { return bottom; } set { bottom = value; } }
		public RectangleF InitialRect { get { return initialRect; } }
		List<ILayoutData> ILayoutData.ChildrenData { get { return null; } }
		public DocumentBand DocumentBand { get { return docBand as DocumentBand; } }
		void ILayoutData.SetBoundsY(float y) {
			top = y;
		}
		public virtual void UpdateViewBounds() {
			bottom = context.FillPage(this) ? context.Bottom : top;
		}
		public virtual void FillPage() {
		}
		public virtual void Anchor(float delta, float dpi) {
		}
		public SubreportDocumentBandLayoutData(ISubreportDocumentBand docBand, LayoutDataContext context, RectangleF initialRect) {
			this.docBand = docBand;
			this.context = context;
			this.initialRect = initialRect;
			top = InitialRect.Top;
			bottom = InitialRect.Bottom;
		}
	}
	public class LayoutDataContext {
		PageRowBuilderBase pageRowBuilderBase;
		DocumentBand rootBand;
		RectangleF bounds;
		float startOffsetY;
		float bottomSpan;
		bool result;
		OffsetHelperY offsetHelper = new OffsetHelperY();
		List<List<BandBricksPair>> bandList = new List<List<BandBricksPair>>();
		List<Pair<Range, List<BandBricksPair>>> bandPairList = new List<Pair<Range, List<BandBricksPair>>>();
		float startNegativeOffsetY;
		public List<List<BandBricksPair>> BandList {
			get {
				return bandList;
			}
		}
		public FillPageResult Result { get { return result ? FillPageResult.Overfulfil : FillPageResult.None; } }
		public float Bottom { get { return (float)(pageRowBuilderBase.Offset.Y - bottomSpan - startOffsetY); } }
		public PageRowBuilderBase PageRowBuilder { get { return pageRowBuilderBase; } }
		public float StartNegativeOffsetY { get { return startNegativeOffsetY; } } 
		public LayoutDataContext(PageRowBuilderBase pageRowBuilderBase, DocumentBand rootBand, RectangleF bounds) {
			this.pageRowBuilderBase = pageRowBuilderBase;
			this.rootBand = rootBand;
			this.bounds = bounds;
			startOffsetY = (float)pageRowBuilderBase.Offset.Y;
			startNegativeOffsetY = (float)pageRowBuilderBase.NegativeOffsetY;
		}
		public bool FillPage(SubreportDocumentBandLayoutData layoutData) {
			if(layoutData == null)
				return false;
			DocumentBand docBand = layoutData.DocumentBand;
			if(pageRowBuilderBase.IsBuildCompleted(docBand))
				return false;
			PageRowBuilderBase rowBuilderBase = pageRowBuilderBase.CreateInternalPageRowBuilder();
			rowBuilderBase.CopyFrom(pageRowBuilderBase);
			rowBuilderBase.NegativeOffsetY = pageRowBuilderBase.NegativeOffsets.GetValueOrDefault(docBand.ID, 0);
			rowBuilderBase.OffsetY = startOffsetY + (docBand.TopSpanActive ? layoutData.Top : 0f);
			rowBuilderBase.RenderHistory.PageIsUpdated = false;
			FillPageResult fillPageResult = rowBuilderBase.FillPageRecursive(rootBand, docBand, bounds);
			bottomSpan = (!(fillPageResult.IsComplete()) && rowBuilderBase.OffsetY < bounds.Height) ? docBand.BottomSpan : 0;
			result |= fillPageResult.IsComplete();
			if(!rowBuilderBase.RenderHistory.PageIsUpdated && !rowBuilderBase.RenderHistory.PageHeaderRendered)
				return false;
			List<BandBricksPair> addedDocumentBands = rowBuilderBase.GetAddedBands();
			if(addedDocumentBands != null && addedDocumentBands.Count > 0) {
				RectangleF rect = ((ISubreportDocumentBand)docBand).ReportRect;
				bandPairList.Add(new Pair<Range, List<BandBricksPair>>(new Range(rect.Left, rect.Right), addedDocumentBands));
			}
			pageRowBuilderBase.CopyFrom(rowBuilderBase);
			if(rowBuilderBase.NegativeOffsetY != 0) {
				pageRowBuilderBase.NegativeOffsets[docBand.ID] = (float)rowBuilderBase.NegativeOffsetY;
			} else
				pageRowBuilderBase.NegativeOffsets.Remove(docBand.ID);
			offsetHelper.Update(rowBuilderBase.Offset.Y, rowBuilderBase.NegativeOffsetY);
			return true;
		}
		public void Commit() {
			while(bandPairList.Count > 0) {
				Pair<Range, List<BandBricksPair>> pair = bandPairList[0];
				bandPairList.Remove(pair);
				while(MergeBands(ref pair, bandPairList));
				bandList.Add(pair.Second);
			}
			offsetHelper.UpdateBuilder(pageRowBuilderBase);
		}
		bool MergeBands(ref Pair<Range, List<BandBricksPair>> samplePair, List<Pair<Range, List<BandBricksPair>>> bandsDictionary) {
			foreach(Pair<Range, List<BandBricksPair>> pair in bandsDictionary) {
				if(pair.First.Intersects(samplePair.First)) {
					List<BandBricksPair> valueUnion = new List<BandBricksPair>();
					valueUnion.AddRange(pair.Second);
					valueUnion.AddRange(samplePair.Second);
					samplePair = new Pair<Range, List<BandBricksPair>>(samplePair.First.Union(pair.First), valueUnion);
					bandsDictionary.Remove(pair);
					return true;
				}
			}
			return false;
		}
	}
	public class Range : DevExpress.XtraPrinting.Native.Pair<float, float> {
		public float Left {
			get { return First; }
		}
		public float Right {
			get { return Second; }
		}
		public Range(float left, float right) : base(left, right) { 
		}
		public bool Intersects(Range range) {
			return !((this.Left < range.Left && this.Right < range.First) || (range.Left < this.Left && range.Right < this.Left));
		}
		public Range Union(Range range) {
			return new Range(Math.Min(this.Left, range.Left), Math.Max(this.Right, range.Right));
		}
	}
}
