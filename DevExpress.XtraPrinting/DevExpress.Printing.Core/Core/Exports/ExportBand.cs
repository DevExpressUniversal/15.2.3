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
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using System.Drawing;
namespace DevExpress.XtraPrinting.Export {
	public class ExportBand {
		List<Brick> bricks;
		LinkedListNode<ExportBand> selfNode;
		DocumentBand documentBand;
		PointF pageOffset;
		int pageRowIndex;
		Page page;
		PointF PageOffsetWithoutMargins {
			get {
				return new PointF(pageOffset.X, Math.Max(pageOffset.Y - page.MarginsF.Top, 0));
			}
		}
		public List<Brick> Bricks {
			get {
				if(bricks == null)
					bricks = new List<Brick>();
				return bricks;
			}
		}
		public DocumentBand DocumentBand { get { return documentBand; } }
		internal LinkedListNode<ExportBand> SelfNode { get { return selfNode; } set { selfNode = value; } }
		internal PointF PageOffset { get { return pageOffset; } }
		internal int PageRowIndex { get { return pageRowIndex; } }
		internal bool IsFirst { get { return selfNode == null || selfNode.Previous == null; } }
		internal bool IsLast { get { return selfNode == null || selfNode.Next == null; } }
		internal ExportBand PreviousBand {
			get {
				if(IsFirst)
					return null;
				return selfNode.Previous.Value;
			}
		}
		internal ExportBand NextBand {
			get {
				if(IsLast)
					return null;
				return selfNode.Next.Value;
			}
		}
		internal bool LastInRowOfPages { get { return pageRowIndex != NextBand.pageRowIndex; } }
		internal bool NextParentDifferent { get { return documentBand.Parent != NextBand.documentBand.Parent; } }
		public ExportBand(DocumentBand documentBand, PointF location, Page page, int pageRowIndex) {
			this.documentBand = documentBand;
			pageOffset = location;
			this.page = page;
			this.pageRowIndex = pageRowIndex;
		}
		internal float FirstOffset() {
			if(DocumentBand.Kind.IsFooter())
				return 0;
			return PageOffsetWithoutMargins.Y;
		}
		internal float AddExportBandElements(IBrickExportVisitor visitor, float verticalOffset) {
			foreach(Brick brick in Bricks) {
				AddExportBandBricks(visitor, verticalOffset, brick);
			}
			return verticalOffset + UpriseVerticalOffset();
		}
		float UpriseVerticalOffset() {
			if(!IsLast)
				return OffsetToNextBand();
			return 0;
		}
		float OffsetToNextBand() {
			float upOffset = 0;
			if(EndOfRow(this)) {
				upOffset = GetMaxRowHeight();
				if(IsHeaderOrNextFooter())
					return upOffset + OffsetPreviousLastBand();
				return ExactOffsetToNextBand(upOffset);
			}
			return 0;
		}
		float ExactOffsetToNextBand(float upOffset) {
			if(!LastInRowOfPages)
				return Math.Max(upOffset, NextBand.PageOffsetWithoutMargins.Y - PageOffsetWithoutMargins.Y);
			if(NextParentDifferent && NextAncestorDifferent())
				return upOffset + GetParentBottomSpans(DocumentBand);
			return upOffset;
		}
		bool NextAncestorDifferent() {
			return GetAncestor(documentBand) != GetAncestor(NextBand.documentBand);			
		}
		DocumentBand GetAncestor(DocumentBand docBand) {
			DocumentBand ancestor = docBand;
			if(ancestor.Parent != null)
				while(ancestor.Parent.Parent != null) {
					ancestor = ancestor.Parent;
				}
			return ancestor;
		}
		bool IsHeaderOrNextFooter() {
			return NextBand.DocumentBand.Kind.IsFooter() || DocumentBand.Kind.IsHeader();
		}
		float OffsetPreviousLastBand() {
			if(NextBand.IsLast)
				return GetParentBottomSpans(DocumentBand);
			return 0;
		}
		float GetParentBottomSpans(DocumentBand docBand) {
			float bottom = 0;
			DocumentBand parent = docBand;
			while(parent.Parent != null) {
				parent = parent.Parent;
				bottom += parent.BottomSpan;
			}
			return bottom;
		}
		float GetMaxRowHeight() {
			float maxRowHeight = DocumentBand.SelfHeight;
			ExportBand previousValue = selfNode.Value;
			while(!IsFirstColumn(previousValue)) {
				previousValue = previousValue.PreviousBand;
				maxRowHeight = Math.Max(maxRowHeight, previousValue.DocumentBand.SelfHeight);
			}
			return maxRowHeight;
		}
		bool IsFirstColumn(ExportBand exportBand) {
			return (exportBand.IsFirst || EndOfRow(exportBand.PreviousBand));
		}
		bool EndOfRow(ExportBand exportBand) {
			if(exportBand.IsLast)
				return true;
			PointF nextLocation = exportBand.NextBand.PageOffsetWithoutMargins;
			return exportBand.PageOffsetWithoutMargins.X >= nextLocation.X || exportBand.PageOffsetWithoutMargins.Y != nextLocation.Y;
		}
		void AddExportBandBricks(IBrickExportVisitor visitor, float verticalOffset, Brick brick) {
			visitor.ExportBrick(PageOffsetWithoutMargins.X, verticalOffset, brick);
		}
	}
}
