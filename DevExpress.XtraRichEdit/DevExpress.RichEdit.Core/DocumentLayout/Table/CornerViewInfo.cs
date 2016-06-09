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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using LayoutUnit = System.Int32;
using ModelUnit = System.Int32;
using LayoutUnitF = System.Single;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout.TableLayout {
	public enum CornerViewInfoType {
		Normal = 0,
		OuterHorizontalStart,
		OuterVerticalStart,
		OuterHorizontalEnd,
		OuterVerticalEnd,
		InnerTopLeft,
		InnerTopMiddle,
		InnerTopRight,
		InnerLeftMiddle,
		InnerRightMiddle,
		InnerBottomLeft,
		InnerBotomMiddle,
		InnerBottomRight,
		InnerNormal
	}
	public abstract class CornerViewInfoBase {
		public static CornerViewInfoBase CreateCorner(DocumentModelUnitToLayoutUnitConverter converter, BorderInfo borderAtLeft, BorderInfo borderAtTop, BorderInfo borderAtRight, BorderInfo borderAtBottom, ModelUnit cellSpacing) {
			return CreateCorner(CornerViewInfoType.Normal, converter, borderAtLeft, borderAtTop, borderAtRight, borderAtBottom, cellSpacing);
		}
		public static CornerViewInfoBase CreateCorner(CornerViewInfoType cornerType, DocumentModelUnitToLayoutUnitConverter converter, BorderInfo borderAtLeft, BorderInfo borderAtTop, BorderInfo borderAtRight, BorderInfo borderAtBottom, ModelUnit cellSpacing) {		  
			TableBorderCalculator tableBorderCalculator = new TableBorderCalculator();
			BorderInfo result = tableBorderCalculator.GetVerticalBorderSource(null, borderAtLeft, borderAtTop);
			result = tableBorderCalculator.GetVerticalBorderSource(null, result, borderAtRight);
			result = tableBorderCalculator.GetVerticalBorderSource(null, result, borderAtBottom);
			if (result == null)
				return new NoneLineCornerViewInfo();
			bool isResultHorizontal = (result == borderAtLeft || result == borderAtRight);
			BorderInfo actualBorderAtLeft = borderAtLeft;
			BorderInfo actualBorderAtRight = borderAtRight;
			BorderInfo actualBorderAtTop = borderAtTop;
			BorderInfo actualBorderAtBottom = borderAtBottom;
			if (!tableBorderCalculator.IsVisuallyAdjacentBorder(result, borderAtLeft, isResultHorizontal))
				actualBorderAtLeft = null;
			if (!tableBorderCalculator.IsVisuallyAdjacentBorder(result, borderAtRight, isResultHorizontal))
				actualBorderAtRight = null;
			if (!tableBorderCalculator.IsVisuallyAdjacentBorder(result, borderAtTop, !isResultHorizontal))
				actualBorderAtTop = null;
			if (!tableBorderCalculator.IsVisuallyAdjacentBorder(result, borderAtBottom, !isResultHorizontal))
				actualBorderAtBottom = null;
			ModelUnit width = 0;
			if (actualBorderAtTop == null && actualBorderAtBottom == null) {
				if (borderAtTop != null)
					width = Math.Max(width, tableBorderCalculator.GetActualWidth(borderAtTop));
				if (borderAtBottom != null)
					width = Math.Max(width, tableBorderCalculator.GetActualWidth(borderAtBottom));
				if (width == 0) {
					width = cellSpacing / 2;
					if (cellSpacing > 0)
						if (borderAtLeft == null)
							cornerType = CornerViewInfoType.OuterHorizontalStart;
						else
							cornerType = CornerViewInfoType.OuterHorizontalEnd;
				}
			}
			else {
				if (actualBorderAtTop != null)
					width = Math.Max(width, tableBorderCalculator.GetActualWidth(actualBorderAtTop));
				if (actualBorderAtBottom != null)
					width = Math.Max(width, tableBorderCalculator.GetActualWidth(actualBorderAtBottom));
			}
			ModelUnit height = 0;
			if (actualBorderAtLeft == null && actualBorderAtRight == null) {
				if (borderAtLeft != null)
					height = Math.Max(height, tableBorderCalculator.GetActualWidth(borderAtLeft));
				if (borderAtRight != null)
					height = Math.Max(height, tableBorderCalculator.GetActualWidth(borderAtRight));
				if (height == 0) {
					height = cellSpacing / 2;
					if (cellSpacing > 0)
						if (borderAtTop == null)
							cornerType = CornerViewInfoType.OuterVerticalStart;
						else
							cornerType = CornerViewInfoType.OuterVerticalEnd;
				}
			}
			else {
				if (actualBorderAtLeft != null)
					height = Math.Max(height, tableBorderCalculator.GetActualWidth(actualBorderAtLeft));
				if (actualBorderAtRight != null)
					height = Math.Max(height, tableBorderCalculator.GetActualWidth(actualBorderAtRight));
			}
			LayoutUnitF layoutBorderWidth = converter.ToLayoutUnits((float)width);
			LayoutUnitF layoutBorderHeight = converter.ToLayoutUnits((float)height);
			int lineCount = tableBorderCalculator.GetLineCount(result);
			switch (lineCount) {
				case 0:
					return new NoneLineCornerViewInfo();
				case 1:
					return new SingleLineCornerViewInfo(actualBorderAtLeft, actualBorderAtTop, actualBorderAtRight, actualBorderAtBottom, layoutBorderWidth, layoutBorderHeight, cornerType);
				case 2:
					return new DoubleLineCornerViewInfo(actualBorderAtLeft, actualBorderAtTop, actualBorderAtRight, actualBorderAtBottom, layoutBorderWidth, layoutBorderHeight, cornerType);
				case 3:
					return new TripleLineCornerViewInfo(actualBorderAtLeft, actualBorderAtTop, actualBorderAtRight, actualBorderAtBottom, layoutBorderWidth, layoutBorderHeight, cornerType);					
				default:
					Exceptions.ThrowInternalException();
					return null;
			}
		}
		float[] widths;
		float[] heights;
		bool[][] pattern;
		LayoutUnitF widthF;
		LayoutUnitF heightF;
		CornerViewInfoType cornerType;
		protected CornerViewInfoBase(CornerViewInfoType cornerType) {
			CornerType = cornerType;
		}
		public CornerViewInfoType CornerType {
			get { return cornerType; } 
			protected set { cornerType = value; }
		}
		public LayoutUnit Width {
			get { return (int)WidthF; }
		}
		public LayoutUnitF WidthF {
			get { return widthF; }
			protected set { widthF = value; }
		}
		public LayoutUnit Height {
			get { return (int)HeightF; }
		}
		public LayoutUnitF HeightF {
			get { return heightF; }
			protected set { heightF = value; }
		}
		public float[] Widths {
			get { return widths; }
			protected set { widths = value; }
		}
		public float[] Heights {
			get { return heights; }
			protected set { heights = value; }
		}
		public bool[][] Pattern {
			get { return pattern; }
			protected set { pattern = value; }
		}
		public abstract Color Color { get; }
		public abstract void Export(IDocumentLayoutExporter exporter, int x, int y);
	}
	public class NoneLineCornerViewInfo : CornerViewInfoBase {
		public NoneLineCornerViewInfo() : base(CornerViewInfoType.Normal) {
			this.Pattern = new bool[][] { };
		}
		public override void Export(IDocumentLayoutExporter exporter, int x, int y) {
		}
		public override Color Color { get { return DXColor.Empty; } }
	}
	public class SingleLineCornerViewInfo : CornerViewInfoBase {
		static readonly bool[][] singleLinePattern = new bool[][] { new bool[] { true } };
		Color color;
		public SingleLineCornerViewInfo(BorderInfo leftBorder, BorderInfo rightBorder, BorderInfo topBorder, BorderInfo bottomBorder, LayoutUnitF width, LayoutUnitF height, CornerViewInfoType cornerType)
			: base(cornerType) {
			BorderInfo border = (leftBorder ?? rightBorder ?? topBorder ?? bottomBorder);
			color = border.Color;
			this.Widths = new float[] { 0f, 1f };
			this.Heights = new float[] { 0f, 1f };
			this.WidthF = width;
			this.HeightF = height;
			this.Pattern = singleLinePattern;
		}
		public override Color Color { get { return color; } }
		public override void Export(IDocumentLayoutExporter exporter, int x, int y) {
			exporter.ExportTableBorderCorner(this, x, y);
		}
	}
	public abstract class MultiLineCornerViewInfoBase : CornerViewInfoBase {
		readonly Color color;
		protected MultiLineCornerViewInfoBase(BorderInfo borderAtLeft, BorderInfo borderAtTop, BorderInfo borderAtRight, BorderInfo borderAtBottom, LayoutUnitF width, LayoutUnitF height, CornerViewInfoType cornerType)
			: base(cornerType) {
			this.WidthF = width;
			this.HeightF = height;
			this.color = (borderAtLeft ?? borderAtTop ?? borderAtRight ?? borderAtBottom).Color;
			Pattern = CreatePattern(borderAtLeft, borderAtTop, borderAtRight, borderAtBottom);
		}
		protected abstract bool[][] VerticalBordersMask { get; }
		protected abstract bool[][] HorizontalBordersMask { get; }
		protected abstract bool[][] CommonBorderMask { get; }
		protected abstract bool[][] LeftBorderMask { get; }
		protected abstract bool[][] RightBorderMask { get; }
		protected abstract bool[][] TopBorderMask { get; }
		protected abstract bool[][] BottomBorderMask { get; }
		public override Color Color { get { return color; } }
		protected virtual bool[][] CreatePattern(BorderInfo borderAtLeft, BorderInfo borderAtTop, BorderInfo borderAtRight, BorderInfo borderAtBottom) {
			TableBorderCalculator tableBorderCalculator = new TableBorderCalculator();
			if (borderAtLeft == null && borderAtRight == null) {
				Widths = tableBorderCalculator.GetDrawingCompoundArray(borderAtTop ?? borderAtBottom);
				Heights = new float[] { 0, 1 };
				return VerticalBordersMask;
			}
			if (borderAtTop == null && borderAtBottom == null) {				
				Widths = new float[] { 0, 1 };
				Heights = tableBorderCalculator.GetDrawingCompoundArray(borderAtLeft ?? borderAtRight);
				return HorizontalBordersMask;
			}
			bool[][] pattern = CommonBorderMask;
			if (borderAtLeft != null)
				pattern = IntersectPattern(pattern, LeftBorderMask);
			if (borderAtRight != null)
				pattern = IntersectPattern(pattern, RightBorderMask);
			if (borderAtTop != null)
				pattern = IntersectPattern(pattern, TopBorderMask);
			if (borderAtBottom != null)
				pattern = IntersectPattern(pattern, BottomBorderMask);
			TableBorderCalculator calculator = new TableBorderCalculator();
			this.Widths = calculator.GetDrawingCompoundArray(borderAtTop ?? borderAtBottom);
			this.Heights = calculator.GetDrawingCompoundArray(borderAtLeft ?? borderAtRight);
			return pattern;
		}
		protected bool[][] IntersectPattern(bool[][] pattern1, bool[][] pattern2) {
			int size = pattern1.Length;
			bool[][] result = new bool[size][];
			for (int i = 0; i < size; i++) {
				result[i] = new bool[size];
				for (int j = 0; j < size; j++) {
					result[i][j] = pattern1[i][j] & pattern2[i][j];
				}
			}
			return result;
		}
		public override void Export(IDocumentLayoutExporter exporter, int x, int y) {
			exporter.ExportTableBorderCorner(this, x, y);
		}
	}
	public class DoubleLineCornerViewInfo : MultiLineCornerViewInfoBase {
		static readonly bool[][] commonBorderMask = new bool[][]{
			new bool[] {true,true,true},
			new bool[] {true,false,true},
			new bool[] {true,true,true}
		};
		static readonly bool[][] leftBorderMask = new bool[][] {
			new bool[] {true, true, true},
			new bool[] {false, true,true},
			new bool[] {true,true,true}
		};
		static readonly bool[][] rightBorderMask = new bool[][] {
			new bool[] {true, true, true},
			new bool[] {true, true,false},
			new bool[] {true,true,true}
		};
		static readonly bool[][] topBorderMask = new bool[][] {
			new bool[] {true, false, true},
			new bool[] {true, true,true},
			new bool[] {true,true,true}
		};
		static readonly bool[][] bottomBorderMask = new bool[][] {
			new bool[] {true, true, true},
			new bool[] {true, true,true},
			new bool[] {true,false,true}
		};
		static readonly bool[][] verticalBordersMask = new bool[][] {
			new bool[] { true, false, true},
		};
		static readonly bool[][] horizontalBordersMask = new bool[][] {
			new bool[] { true},
			new bool[] { false},
			new bool[] { true}
		};
		public DoubleLineCornerViewInfo(BorderInfo borderAtLeft, BorderInfo borderAtTop, BorderInfo borderAtRight, BorderInfo borderAtBottom, LayoutUnitF width, LayoutUnitF height, CornerViewInfoType cornerType)
			: base(borderAtLeft, borderAtTop, borderAtRight, borderAtBottom, width, height, cornerType) {
		}
		protected override bool[][] BottomBorderMask { get { return bottomBorderMask; } }
		protected override bool[][] TopBorderMask { get { return topBorderMask; } }
		protected override bool[][] LeftBorderMask { get { return leftBorderMask; } }
		protected override bool[][] RightBorderMask { get { return rightBorderMask; } }
		protected override bool[][] VerticalBordersMask { get { return verticalBordersMask; } }
		protected override bool[][] HorizontalBordersMask { get { return horizontalBordersMask; } }
		protected override bool[][] CommonBorderMask { get { return commonBorderMask; } }
	}
	public class TripleLineCornerViewInfo : MultiLineCornerViewInfoBase {
		static readonly bool[][] commonBorderMask = new bool[][]{
			new bool[] {true,true,true,true,true},
			new bool[] {true,false,true,false,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,false,true,false,true},
			new bool[] {true,true,true,true,true}
		};
		static readonly bool[][] leftBorderMask = new bool[][] {
			new bool[] {true,true,true,true,true},
			new bool[] {false,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {false,true,true,true,true},
			new bool[] {true,true,true,true,true}
		};
		static readonly bool[][] NoLeftBorderMask = new bool[][] {
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,false,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true}
		};
		static readonly bool[][] rightBorderMask = new bool[][] {
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,false},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,false},
			new bool[] {true,true,true,true,true}
		};
		static readonly bool[][] NoRightBorderMask = new bool[][] {
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,false,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true}
		};
		static readonly bool[][] topBorderMask = new bool[][] {
			new bool[] {true,false,true,false,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true}
		};
		static readonly bool[][] NoTopBorderMask = new bool[][] {
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,false,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true}
		};
		static readonly bool[][] bottomBorderMask = new bool[][] {
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,false,true,false,true}			
		};
		static readonly bool[][] NoBottomBorderMask = new bool[][] {
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,true,true,true},
			new bool[] {true,true,false,true,true},
			new bool[] {true,true,true,true,true}
		};
		static readonly bool[][] verticalBordersMask = new bool[][] {
			new bool[] { true, false, true, false, true},
		};
		static readonly bool[][] horizontalBordersMask = new bool[][] {
			new bool[] { true},
			new bool[] { false},
			new bool[] { true},
			new bool[] { false},
			new bool[] { true}
		};
		public TripleLineCornerViewInfo(BorderInfo borderAtLeft, BorderInfo borderAtTop, BorderInfo borderAtRight, BorderInfo borderAtBottom, LayoutUnitF width, LayoutUnitF height, CornerViewInfoType cornerType)
			: base(borderAtLeft, borderAtTop, borderAtRight, borderAtBottom, width, height, cornerType) {
		}
		protected override bool[][] CreatePattern(BorderInfo borderAtLeft, BorderInfo borderAtTop, BorderInfo borderAtRight, BorderInfo borderAtBottom) {
			bool[][] pattern = base.CreatePattern(borderAtLeft, borderAtTop, borderAtRight, borderAtBottom);
			if (pattern == HorizontalBordersMask || pattern == VerticalBordersMask)
				return pattern;
			if (borderAtLeft == null)
				pattern = IntersectPattern(pattern, NoLeftBorderMask);
			if (borderAtRight == null)
				pattern = IntersectPattern(pattern, NoRightBorderMask);
			if (borderAtTop == null)
				pattern = IntersectPattern(pattern, NoTopBorderMask);
			if (borderAtBottom == null)
				pattern = IntersectPattern(pattern, NoBottomBorderMask);
			return pattern;
		}
		protected override bool[][] BottomBorderMask { get { return bottomBorderMask; } }
		protected override bool[][] TopBorderMask { get { return topBorderMask; } }
		protected override bool[][] LeftBorderMask { get { return leftBorderMask; } }
		protected override bool[][] RightBorderMask { get { return rightBorderMask; } }
		protected override bool[][] VerticalBordersMask { get { return verticalBordersMask; } }
		protected override bool[][] HorizontalBordersMask { get { return horizontalBordersMask; } }
		protected override bool[][] CommonBorderMask { get { return commonBorderMask; } }
	}
}
