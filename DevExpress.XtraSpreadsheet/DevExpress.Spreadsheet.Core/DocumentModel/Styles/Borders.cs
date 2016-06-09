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
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public static class SpecialBorderLineStyle {
		public const XlBorderLineStyle ForcedNone = (XlBorderLineStyle)14; 
		public const XlBorderLineStyle NoneOverrideDefaultGrid = (XlBorderLineStyle)15; 
		public const XlBorderLineStyle DefaultGrid = (XlBorderLineStyle)16; 
		public const XlBorderLineStyle PrintGrid = (XlBorderLineStyle)17; 
		public const XlBorderLineStyle InsideComplexBorder = (XlBorderLineStyle)18; 
		public const XlBorderLineStyle OutsideComplexBorder = (XlBorderLineStyle)19; 
	}
	#region BorderInfo
	public class BorderInfo : XlBordersBase, ICloneable<BorderInfo>, ISupportsCopyFrom<BorderInfo>, ISupportsSizeOf {
		readonly static Dictionary<XlBorderLineStyle, int> linePixelThicknessTable = CreateLinePixelThicknessTable();
		public static Dictionary<XlBorderLineStyle, int> LinePixelThicknessTable { get { return linePixelThicknessTable; } }
		static Dictionary<XlBorderLineStyle, int> CreateLinePixelThicknessTable() {
			Dictionary<XlBorderLineStyle, int> result = new Dictionary<XlBorderLineStyle, int>();
			result.Add(SpecialBorderLineStyle.ForcedNone, 0);
			result.Add(SpecialBorderLineStyle.NoneOverrideDefaultGrid, 0);
			result.Add(XlBorderLineStyle.None, 0);
			result.Add(XlBorderLineStyle.DashDot, 1);
			result.Add(XlBorderLineStyle.DashDotDot, 1);
			result.Add(XlBorderLineStyle.Dashed, 1);
			result.Add(XlBorderLineStyle.Dotted, 1);
			result.Add(SpecialBorderLineStyle.DefaultGrid, 1);
			result.Add(SpecialBorderLineStyle.PrintGrid, 1);
			result.Add(SpecialBorderLineStyle.InsideComplexBorder, 1);
			result.Add(XlBorderLineStyle.Hair, 1);
			result.Add(XlBorderLineStyle.Thin, 1);
			result.Add(XlBorderLineStyle.Medium, 2);
			result.Add(SpecialBorderLineStyle.OutsideComplexBorder, 3);
			result.Add(XlBorderLineStyle.MediumDashDot, 2);
			result.Add(XlBorderLineStyle.MediumDashDotDot, 2);
			result.Add(XlBorderLineStyle.MediumDashed, 2);
			result.Add(XlBorderLineStyle.SlantDashDot, 2);
			result.Add(XlBorderLineStyle.Thick, 3);
			result.Add(XlBorderLineStyle.Double, 3);
			return result;
		}
		readonly static Dictionary<XlBorderLineStyle, int> lineWeightTable = CreateLineWeightTable();
		public static Dictionary<XlBorderLineStyle, int> LineWeightTable { get { return lineWeightTable; } }
		static Dictionary<XlBorderLineStyle, int> CreateLineWeightTable() {
			Dictionary<XlBorderLineStyle, int> result = new Dictionary<XlBorderLineStyle, int>();
			result.Add(XlBorderLineStyle.None, 0);
			result.Add(XlBorderLineStyle.Hair, 1);
			result.Add(XlBorderLineStyle.Dotted, 2);
			result.Add(XlBorderLineStyle.DashDotDot, 3);
			result.Add(XlBorderLineStyle.DashDot, 4);
			result.Add(XlBorderLineStyle.Dashed, 5);
			result.Add(XlBorderLineStyle.Thin, 6);
			result.Add(XlBorderLineStyle.MediumDashDotDot, 7);
			result.Add(XlBorderLineStyle.SlantDashDot, 8);
			result.Add(XlBorderLineStyle.MediumDashDot, 9);
			result.Add(XlBorderLineStyle.MediumDashed, 10);
			result.Add(XlBorderLineStyle.Medium, 11);
			result.Add(XlBorderLineStyle.Thick, 12);
			result.Add(XlBorderLineStyle.Double, 13);
			return result;
		}
		#region Fields
		static readonly BorderInfoBorderAccessor leftBorderAccessor = new BorderInfoLeftBorderAccessor();
		static readonly BorderInfoBorderAccessor rightBorderAccessor = new BorderInfoRightBorderAccessor();
		static readonly BorderInfoBorderAccessor topBorderAccessor = new BorderInfoTopBorderAccessor();
		static readonly BorderInfoBorderAccessor bottomBorderAccessor = new BorderInfoBottomBorderAccessor();
		static readonly BorderInfoBorderAccessor verticalBorderAccessor = new BorderInfoVerticalBorderAccessor();
		static readonly BorderInfoBorderAccessor horizontalBorderAccessor = new BorderInfoHorizontalBorderAccessor();
		static readonly BorderInfoBorderAccessor diagonalBorderAccessor = new BorderInfoDiagonalBorderAccessor();
		public static BorderInfoBorderAccessor LeftBorderAccessor { get { return leftBorderAccessor; } }
		public static BorderInfoBorderAccessor RightBorderAccessor { get { return rightBorderAccessor; } }
		public static BorderInfoBorderAccessor TopBorderAccessor { get { return topBorderAccessor; } }
		public static BorderInfoBorderAccessor BottomBorderAccessor { get { return bottomBorderAccessor; } }
		public static BorderInfoBorderAccessor VerticalBorderAccessor { get { return verticalBorderAccessor; } }
		public static BorderInfoBorderAccessor HorizontalBorderAccessor { get { return horizontalBorderAccessor; } }
		public static BorderInfoBorderAccessor DiagonalBorderAccessor { get { return diagonalBorderAccessor; } }
		int leftColorIndex;
		int rightColorIndex;
		int topColorIndex;
		int bottomColorIndex;
		int diagonalColorIndex;
		int horizontalColorIndex;
		int verticalColorIndex;
		#endregion
		#region Properties
		public int LeftColorIndex { get { return leftColorIndex; } set { leftColorIndex = value; } }
		public int RightColorIndex { get { return rightColorIndex; } set { rightColorIndex = value; } }
		public int TopColorIndex { get { return topColorIndex; } set { topColorIndex = value; } }
		public int BottomColorIndex { get { return bottomColorIndex; } set { bottomColorIndex = value; } }
		public int DiagonalColorIndex { get { return diagonalColorIndex; } set { diagonalColorIndex = value; } }
		public int HorizontalColorIndex { get { return horizontalColorIndex; } set { horizontalColorIndex = value; } }
		public int VerticalColorIndex { get { return verticalColorIndex; } set { verticalColorIndex = value; } }
		#endregion
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			BorderInfo info = obj as BorderInfo;
			if (info == null)
				return false;
			return
				this.LeftColorIndex == info.LeftColorIndex &&
				this.RightColorIndex == info.RightColorIndex &&
				this.TopColorIndex == info.TopColorIndex &&
				this.BottomColorIndex == info.BottomColorIndex &&
				this.DiagonalColorIndex == info.DiagonalColorIndex &&
				this.HorizontalColorIndex == info.HorizontalColorIndex &&
				this.VerticalColorIndex == info.VerticalColorIndex;
		}
		internal bool EqualsForDifferentWorkbooks(BorderInfo otherInfo, DocumentModel targetDocumentModel, DocumentModel otherDocumentModel) {
			return base.Equals(otherInfo) && EqualsByColorsOnly(otherInfo, targetDocumentModel, otherDocumentModel);
		}
		bool EqualsByColorsOnly(BorderInfo otherInfo, DocumentModel targetDocumentModel, DocumentModel otherDocumentModel) {
			return
				EqualsByColorOnly(otherInfo, targetDocumentModel, otherDocumentModel, LeftBorderAccessor) &&
				EqualsByColorOnly(otherInfo, targetDocumentModel, otherDocumentModel, RightBorderAccessor) &&
				EqualsByColorOnly(otherInfo, targetDocumentModel, otherDocumentModel, TopBorderAccessor) &&
				EqualsByColorOnly(otherInfo, targetDocumentModel, otherDocumentModel, BottomBorderAccessor) &&
				EqualsByColorOnly(otherInfo, targetDocumentModel, otherDocumentModel, DiagonalBorderAccessor) &&
				EqualsByColorOnly(otherInfo, targetDocumentModel, otherDocumentModel, HorizontalBorderAccessor) &&
				EqualsByColorOnly(otherInfo, targetDocumentModel, otherDocumentModel, VerticalBorderAccessor);
		}
		bool EqualsByColorOnly(BorderInfo otherInfo, DocumentModel targetDocumentModel, DocumentModel otherDocumentModel, BorderInfoBorderAccessor accessor) {
			return GetColorModelInfo(targetDocumentModel, accessor).Rgb == otherInfo.GetColorModelInfo(otherDocumentModel, accessor).Rgb;
		}
		internal bool EqualsNoColorIndexes(BorderInfo borderInfo) {
			return base.Equals(borderInfo);
		}
		ColorModelInfo GetColorModelInfo(DocumentModel documentModel, BorderInfoBorderAccessor accessor) {
			return documentModel.Cache.ColorModelInfoCache[accessor.GetColorIndex(this)];
		}
		public override int GetHashCode() {
			CombinedHashCode hash = new CombinedHashCode();
			hash.AddInt(base.GetHashCode());
			hash.AddInt(leftColorIndex);
			hash.AddInt(rightColorIndex);
			hash.AddInt(topColorIndex);
			hash.AddInt(bottomColorIndex);
			hash.AddInt(diagonalColorIndex );
			hash.AddInt(horizontalColorIndex);
			hash.AddInt(verticalColorIndex);
			return hash.CombinedHash32;
		}
		#region ICloneable<BorderInfo> Members
		public BorderInfo Clone() {
			BorderInfo result = new BorderInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<BorderInfo> Members
		public void CopyFrom(BorderInfo value) {
			base.CopyFrom(value);
			this.LeftColorIndex = value.LeftColorIndex;
			this.RightColorIndex = value.RightColorIndex;
			this.TopColorIndex = value.TopColorIndex;
			this.BottomColorIndex = value.BottomColorIndex;
			this.DiagonalColorIndex = value.DiagonalColorIndex;
			this.HorizontalColorIndex = value.HorizontalColorIndex;
			this.VerticalColorIndex = value.VerticalColorIndex;
		}
		public void CopyFrom(IActualBorderInfo value) {
			this.BottomColorIndex = value.BottomColorIndex;
			this.BottomLineStyle = value.BottomLineStyle;
			this.DiagonalColorIndex = value.DiagonalColorIndex;
			this.DiagonalDown = GetBorderDiagonal(value.DiagonalDownLineStyle);
			this.DiagonalUp = GetBorderDiagonal(value.DiagonalUpLineStyle);
			this.DiagonalLineStyle = GetBorderActualDiagonalLineStyle(value);
			this.HorizontalColorIndex = value.HorizontalColorIndex;
			this.HorizontalLineStyle = value.HorizontalLineStyle;
			this.LeftColorIndex = value.LeftColorIndex;
			this.LeftLineStyle = value.LeftLineStyle;
			this.Outline = value.Outline;
			this.RightColorIndex = value.RightColorIndex;
			this.RightLineStyle = value.RightLineStyle;
			this.TopColorIndex = value.TopColorIndex;
			this.TopLineStyle = value.TopLineStyle;
			this.VerticalColorIndex = value.VerticalColorIndex;
			this.VerticalLineStyle = value.VerticalLineStyle;
		}
		protected internal XlBorderLineStyle GetBorderActualDiagonalLineStyle(IActualBorderInfo value) {
			XlBorderLineStyle result = value.DiagonalUpLineStyle;
			if (GetBorderDiagonal(result))
				return result;
			result = value.DiagonalDownLineStyle;
			if (GetBorderDiagonal(result))
				return result;
			return XlBorderLineStyle.None;
		}
		protected internal bool GetBorderDiagonal(XlBorderLineStyle diagonalLineStyle) {
			return diagonalLineStyle != XlBorderLineStyle.None;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
#if DEBUGTEST
		public const int TestNone = 0;
		public const int TestLeftLine = 1;
		public const int TestRightLine = 2;
		public const int TestTopLine = 3;
		public const int TestBottomLine = 4;
		public const int TestDiagonalLine = 5;
		public const int TestHorizontalLine = 6;
		public const int TestVerticalLine = 7;
		public const int TestDiagonalDown = 8;
		public const int TestDiagonalUp = 9;
		public const int TestOutline = 10;
		public bool CheckDefaults2() {
			return Check(TestNone) && CheckDefaultColors();
		}
		public bool CheckDefaultColors() {
			bool result = true;
			result &= 0 == LeftColorIndex;
			result &= 0 == RightColorIndex;
			result &= 0 == TopColorIndex;
			result &= 0 == BottomColorIndex;
			result &= 0 == DiagonalColorIndex;
			result &= 0 == HorizontalColorIndex;
			result &= 0 == VerticalColorIndex;
			return result;
		}
		public bool Check(int without) {
			if (without != TestLeftLine)
				return XlBorderLineStyle.None == this.LeftLineStyle;
			if (without != TestRightLine)
				return XlBorderLineStyle.None == this.RightLineStyle;
			if (without != TestTopLine)
				return XlBorderLineStyle.None == this.TopLineStyle;
			if (without != TestBottomLine)
				return XlBorderLineStyle.None == this.BottomLineStyle;
			if (without != TestDiagonalLine)									  
				return XlBorderLineStyle.None == this.DiagonalLineStyle;
			if (without != TestHorizontalLine)									
				return XlBorderLineStyle.None == this.HorizontalLineStyle;
			if (without != TestVerticalLine)
				return XlBorderLineStyle.None == this.VerticalLineStyle;
			if (without != TestDiagonalDown)
				return !this.DiagonalDown;
			if (without != TestDiagonalUp)
				return !this.DiagonalUp;
			if (without != TestOutline)
				return this.Outline;
			return true;
		}
#endif
	}
	#endregion
	#region BorderInfoBorderAccessor (abstract class)
	public abstract class BorderInfoBorderAccessor {
		public abstract int BorderIndex { get; }
		public abstract XlBorderLineStyle GetLineStyle(BorderInfo info);
		public abstract void SetLineStyle(BorderInfo info, XlBorderLineStyle value);
		public abstract int GetColorIndex(BorderInfo info);
		public abstract void SetColorIndex(BorderInfo info, int value);
		public ColorModelInfo GetColorModelInfo(BorderInfo info, DocumentModel documentModel) {
			int colorIndex = GetColorIndex(info);
			return documentModel.Cache.ColorModelInfoCache[colorIndex];
		}
	}
	#endregion
	#region BorderInfoLeftBorderAccessor
	public class BorderInfoLeftBorderAccessor : BorderInfoBorderAccessor {
		const int index = 0;
		public override int BorderIndex { get { return index; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.LeftLineStyle;
		}
		public override void SetLineStyle(BorderInfo info, XlBorderLineStyle value) {
			info.LeftLineStyle = value;
		}
		public override int GetColorIndex(BorderInfo info) {
			return info.LeftColorIndex;
		}
		public override void SetColorIndex(BorderInfo info, int value) {
			info.LeftColorIndex = value;
		}
	}
	#endregion
	#region BorderInfoRightBorderAccessor
	public class BorderInfoRightBorderAccessor : BorderInfoBorderAccessor {
		const int index = 1;
		public override int BorderIndex { get { return index; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.RightLineStyle;
		}
		public override void SetLineStyle(BorderInfo info, XlBorderLineStyle value) {
			info.RightLineStyle = value;
		}
		public override int GetColorIndex(BorderInfo info) {
			return info.RightColorIndex;
		}
		public override void SetColorIndex(BorderInfo info, int value) {
			info.RightColorIndex = value;
		}
	}
	#endregion
	#region BorderInfoTopBorderAccessor
	public class BorderInfoTopBorderAccessor : BorderInfoBorderAccessor {
		const int index = 2;
		public override int BorderIndex { get { return index; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.TopLineStyle;
		}
		public override void SetLineStyle(BorderInfo info, XlBorderLineStyle value) {
			info.TopLineStyle = value;
		}
		public override int GetColorIndex(BorderInfo info) {
			return info.TopColorIndex;
		}
		public override void SetColorIndex(BorderInfo info, int value) {
			info.TopColorIndex = value;
		}
	}
	#endregion
	#region BorderInfoBottomBorderAccessor
	public class BorderInfoBottomBorderAccessor : BorderInfoBorderAccessor {
		const int index = 3;
		public override int BorderIndex { get { return index; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.BottomLineStyle;
		}
		public override void SetLineStyle(BorderInfo info, XlBorderLineStyle value) {
			info.BottomLineStyle = value;
		}
		public override int GetColorIndex(BorderInfo info) {
			return info.BottomColorIndex;
		}
		public override void SetColorIndex(BorderInfo info, int value) {
			info.BottomColorIndex = value;
		}
	}
	#endregion
	#region BorderInfoVerticalBorderAccessor
	public class BorderInfoVerticalBorderAccessor : BorderInfoBorderAccessor {
		const int index = 4;
		public override int BorderIndex { get { return index; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.VerticalLineStyle;
		}
		public override void SetLineStyle(BorderInfo info, XlBorderLineStyle value) {
			info.VerticalLineStyle = value;
		}
		public override int GetColorIndex(BorderInfo info) {
			return info.VerticalColorIndex;
		}
		public override void SetColorIndex(BorderInfo info, int value) {
			info.VerticalColorIndex = value;
		}
	}
	#endregion
	#region BorderInfoHorizontalBorderAccessor
	public class BorderInfoHorizontalBorderAccessor : BorderInfoBorderAccessor {
		const int index = 5;
		public override int BorderIndex { get { return index; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.HorizontalLineStyle;
		}
		public override void SetLineStyle(BorderInfo info, XlBorderLineStyle value) {
			 info.HorizontalLineStyle = value;
		}
		public override int GetColorIndex(BorderInfo info) {
			return info.HorizontalColorIndex;
		}
		public override void SetColorIndex(BorderInfo info, int value) {
			 info.HorizontalColorIndex = value;
		}
	}
	#endregion
	#region BorderInfoDiagonalBorderAccessor
	public class BorderInfoDiagonalBorderAccessor : BorderInfoBorderAccessor {
		const int index = 6;
		public override int BorderIndex { get { return index; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.DiagonalLineStyle;
		}
		public override void SetLineStyle(BorderInfo info, XlBorderLineStyle value) {
			info.DiagonalLineStyle = value;
		}
		public override int GetColorIndex(BorderInfo info) {
			return info.DiagonalColorIndex;
		}
		public override void SetColorIndex(BorderInfo info, int value) {
			info.DiagonalColorIndex = value;
		}
	}
	#endregion
	#region IBorderInfo
	public interface IBorderInfo {
		XlBorderLineStyle LeftLineStyle { get; set; }
		XlBorderLineStyle RightLineStyle { get; set; }
		XlBorderLineStyle TopLineStyle { get; set; }
		XlBorderLineStyle BottomLineStyle { get; set; }
		XlBorderLineStyle DiagonalUpLineStyle { get; set; }
		XlBorderLineStyle DiagonalDownLineStyle { get; set; }
		XlBorderLineStyle HorizontalLineStyle { get; set; }
		XlBorderLineStyle VerticalLineStyle { get; set; }
		bool Outline { get; set; }
		Color LeftColor { get; set; }
		Color RightColor { get; set; }
		Color TopColor { get; set; }
		Color BottomColor { get; set; }
		Color DiagonalColor { get; set; }
		Color HorizontalColor { get; set; }
		Color VerticalColor { get; set; }
		int LeftColorIndex { get; set; }
		int RightColorIndex { get; set; }
		int TopColorIndex { get; set; }
		int BottomColorIndex { get; set; }
		int DiagonalColorIndex { get; set; }
		int HorizontalColorIndex { get; set; }
		int VerticalColorIndex { get; set; }
	}
	#endregion
	#region IActualBorderInfo
	public interface IActualBorderInfo {
		XlBorderLineStyle LeftLineStyle { get; }
		XlBorderLineStyle RightLineStyle { get; }
		XlBorderLineStyle TopLineStyle { get; }
		XlBorderLineStyle BottomLineStyle { get; }
		XlBorderLineStyle DiagonalUpLineStyle { get; }
		XlBorderLineStyle DiagonalDownLineStyle { get; }
		XlBorderLineStyle HorizontalLineStyle { get; }
		XlBorderLineStyle VerticalLineStyle { get; }
		bool Outline { get; }
		Color LeftColor { get; }
		Color RightColor { get; }
		Color TopColor { get; }
		Color BottomColor { get; }
		Color DiagonalColor { get; }
		Color HorizontalColor { get; }
		Color VerticalColor { get; }
		int LeftColorIndex { get; }
		int RightColorIndex { get; }
		int TopColorIndex { get; }
		int BottomColorIndex { get; }
		int DiagonalColorIndex { get; }
		int HorizontalColorIndex { get; }
		int VerticalColorIndex { get; }
	}
	#endregion
	#region BorderInfoCache
	public class BorderInfoCache : UniqueItemsCache<BorderInfo> {
		internal const int DefaultItemIndex = 0;
		public BorderInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override BorderInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			BorderInfo item = new BorderInfo();
			return item;
		}
#if DEBUGTEST
		public static bool CheckDefaults2(BorderInfoCache collection) {
			bool result = true;
			result &= collection != null;
			result &= collection.Count > 0;
			BorderInfo info = (BorderInfo)collection.DefaultItem;
			result &= 0 == info.BottomColorIndex;
			result &= (XlBorderLineStyle.None == info.BottomLineStyle);
			result &= 0 == info.DiagonalColorIndex;
			result &= XlBorderLineStyle.None == info.DiagonalLineStyle;
			result &= 0 == info.HorizontalColorIndex;
			result &= XlBorderLineStyle.None == info.HorizontalLineStyle;
			result &= 0 == info.LeftColorIndex;
			result &= XlBorderLineStyle.None == info.LeftLineStyle;
			result &= 0 == info.RightColorIndex;
			result &= XlBorderLineStyle.None == info.RightLineStyle;
			result &= 0 == info.TopColorIndex;
			result &= XlBorderLineStyle.None == info.TopLineStyle;
			result &= 0 == info.VerticalColorIndex;
			result &= XlBorderLineStyle.None == info.VerticalLineStyle;
			result &= ! info.DiagonalDown;
			result &= ! info.DiagonalUp;
			result &= info.Outline;
			return result;
		}
#endif
	}
	#endregion
	#region BorderSideAccessor (abstract class)
	public abstract class BorderSideAccessor {
		readonly static BorderSideAccessor top = new TopBorderSideAccessor();
		readonly static BorderSideAccessor bottom = new BottomBorderSideAccessor();
		readonly static BorderSideAccessor left = new LeftBorderSideAccessor();
		readonly static BorderSideAccessor right = new RightBorderSideAccessor();
		readonly static BorderSideAccessor diagonalUp = new DiagonalUpBorderSideAccessor();
		readonly static BorderSideAccessor diagonalDown = new DiagonalDownBorderSideAccessor();
		public static BorderSideAccessor Top { get { return top; } }
		public static BorderSideAccessor Bottom { get { return bottom; } }
		public static BorderSideAccessor Left { get { return left; } }
		public static BorderSideAccessor Right { get { return right; } }
		public static BorderSideAccessor DiagonalUp { get { return diagonalUp; } }
		public static BorderSideAccessor DiagonalDown { get { return diagonalDown; } }
		public abstract Color GetLineColor(IActualBorderInfo borders);
		public abstract Color GetLineColor(IBorderInfo borders);
		public abstract int GetLineColorIndex(IActualBorderInfo borders);
		public abstract XlBorderLineStyle GetLineStyle(IActualBorderInfo borders);
		public abstract XlBorderLineStyle GetLineStyle(IBorderInfo borders);
		public abstract void SetLineStyle(IBorderInfo borders, XlBorderLineStyle style);
		public abstract void SetLineColorIndex(IBorderInfo borders, int colorIndex);
	}
	#endregion
	#region TopBorderSideAccessor
	public class TopBorderSideAccessor : BorderSideAccessor {
		public override Color GetLineColor(IActualBorderInfo borders) {
			return borders.TopColor;
		}
		public override Color GetLineColor(IBorderInfo borders) {
			return borders.TopColor;
		}
		public override int GetLineColorIndex(IActualBorderInfo borders) {
			return borders.TopColorIndex;
		}
		public override XlBorderLineStyle GetLineStyle(IActualBorderInfo borders) {
			return borders.TopLineStyle;
		}
		public override XlBorderLineStyle GetLineStyle(IBorderInfo borders) {
			return borders.TopLineStyle;
		}
		public override void SetLineStyle(IBorderInfo borders, XlBorderLineStyle style) {
			borders.TopLineStyle = style;
		}
		public override void SetLineColorIndex(IBorderInfo borders, int colorIndex) {
			borders.TopColorIndex = colorIndex;
		}
	}
	#endregion
	#region BottomBorderSideAccessor
	public class BottomBorderSideAccessor : BorderSideAccessor {
		public override Color GetLineColor(IActualBorderInfo borders) {
			return borders.BottomColor;
		}
		public override Color GetLineColor(IBorderInfo borders) {
			return borders.BottomColor;
		}
		public override int GetLineColorIndex(IActualBorderInfo borders) {
			return borders.BottomColorIndex;
		}
		public override XlBorderLineStyle GetLineStyle(IActualBorderInfo borders) {
			return borders.BottomLineStyle;
		}
		public override XlBorderLineStyle GetLineStyle(IBorderInfo borders) {
			return borders.BottomLineStyle;
		}
		public override void SetLineStyle(IBorderInfo borders, XlBorderLineStyle style) {
			borders.BottomLineStyle = style;
		}
		public override void SetLineColorIndex(IBorderInfo borders, int colorIndex) {
			borders.BottomColorIndex = colorIndex;
		}
	}
	#endregion
	#region LeftBorderSideAccessor
	public class LeftBorderSideAccessor : BorderSideAccessor {
		public override Color GetLineColor(IActualBorderInfo borders) {
			return borders.LeftColor;
		}
		public override Color GetLineColor(IBorderInfo borders) {
			return borders.LeftColor;
		}
		public override int GetLineColorIndex(IActualBorderInfo borders) {
			return borders.LeftColorIndex;
		}
		public override XlBorderLineStyle GetLineStyle(IActualBorderInfo borders) {
			return borders.LeftLineStyle;
		}
		public override XlBorderLineStyle GetLineStyle(IBorderInfo borders) {
			return borders.LeftLineStyle;
		}
		public override void SetLineStyle(IBorderInfo borders, XlBorderLineStyle style) {
			borders.LeftLineStyle = style;
		}
		public override void SetLineColorIndex(IBorderInfo borders, int colorIndex) {
			borders.LeftColorIndex = colorIndex;
		}
	}
	#endregion
	#region RightBorderSideAccessor
	public class RightBorderSideAccessor : BorderSideAccessor {
		public override Color GetLineColor(IActualBorderInfo borders) {
			return borders.RightColor;
		}
		public override Color GetLineColor(IBorderInfo borders) {
			return borders.RightColor;
		}
		public override int GetLineColorIndex(IActualBorderInfo borders) {
			return borders.RightColorIndex;
		}
		public override XlBorderLineStyle GetLineStyle(IActualBorderInfo borders) {
			return borders.RightLineStyle;
		}
		public override XlBorderLineStyle GetLineStyle(IBorderInfo borders) {
			return borders.RightLineStyle;
		}
		public override void SetLineStyle(IBorderInfo borders, XlBorderLineStyle style) {
			borders.RightLineStyle = style;
		}
		public override void SetLineColorIndex(IBorderInfo borders, int colorIndex) {
			borders.RightColorIndex = colorIndex;
		}
	}
	#endregion
	#region DiagonalUpBorderSideAccessor
	public class DiagonalUpBorderSideAccessor : BorderSideAccessor {
		public override Color GetLineColor(IActualBorderInfo borders) {
			return borders.DiagonalColor;
		}
		public override Color GetLineColor(IBorderInfo borders) {
			return borders.DiagonalColor;
		}
		public override int GetLineColorIndex(IActualBorderInfo borders) {
			return borders.DiagonalColorIndex;
		}
		public override XlBorderLineStyle GetLineStyle(IActualBorderInfo borders) {
			return borders.DiagonalUpLineStyle;
		}
		public override XlBorderLineStyle GetLineStyle(IBorderInfo borders) {
			return borders.DiagonalUpLineStyle;
		}
		public override void SetLineStyle(IBorderInfo borders, XlBorderLineStyle style) {
			borders.DiagonalUpLineStyle = style;
		}
		public override void SetLineColorIndex(IBorderInfo borders, int colorIndex) {
			borders.DiagonalColorIndex = colorIndex;
		}
	}
	#endregion
	#region DiagonalDownBorderSideAccessor
	public class DiagonalDownBorderSideAccessor : BorderSideAccessor {
		public override Color GetLineColor(IActualBorderInfo borders) {
			return borders.DiagonalColor;
		}
		public override Color GetLineColor(IBorderInfo borders) {
			return borders.DiagonalColor;
		}
		public override int GetLineColorIndex(IActualBorderInfo borders) {
			return borders.DiagonalColorIndex;
		}
		public override XlBorderLineStyle GetLineStyle(IActualBorderInfo borders) {
			return borders.DiagonalDownLineStyle;
		}
		public override XlBorderLineStyle GetLineStyle(IBorderInfo borders) {
			return borders.DiagonalDownLineStyle;
		}
		public override void SetLineStyle(IBorderInfo borders, XlBorderLineStyle style) {
			borders.DiagonalDownLineStyle = style;
		}
		public override void SetLineColorIndex(IBorderInfo borders, int colorIndex) {
			borders.DiagonalColorIndex = colorIndex;
		}
	}
	#endregion
	#region BorderIndex
	public enum BorderIndex {
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3,
		DiagonalUp = 4,
		DiagonalDown = 5,
		Horizontal = 6,
		Vertical = 7
	}
	#endregion
	#region BorderElementInfo (struct)
	public struct BorderElementInfo {
		#region Static Members
		public static BorderElementInfo Create(XlBorderLineStyle lineStyle, ColorModelInfo colorInfo) {
			BorderElementInfo info = new BorderElementInfo();
			info.lineStyle = lineStyle;
			info.colorInfo = colorInfo;
			info.isDefaultLineStyle = false;
			return info;
		}
		#endregion
		#region Fields
		XlBorderLineStyle lineStyle;
		ColorModelInfo colorInfo;
		bool isDefaultLineStyle;
		#endregion
		#region Properties
		public XlBorderLineStyle LineStyle { get { return lineStyle; } }
		public ColorModelInfo ColorInfo { get { return colorInfo; } }
		public bool IsDefaultLineStyle { get { return isDefaultLineStyle; } }
		#endregion
	}
	#endregion
	#region ActualBorderElementInfo (struct)
	public struct ActualBorderElementInfo {
		#region Static Members
		public static ActualBorderElementInfo CreateDefault(XlBorderLineStyle lineStyle) {
			return new ActualBorderElementInfo(lineStyle, 0);
		}
		#endregion
		#region Fields
		XlBorderLineStyle lineStyle;
		int colorIndex;
		#endregion
		public ActualBorderElementInfo(XlBorderLineStyle lineStyle, int colorIndex) {
			this.lineStyle = lineStyle;
			this.colorIndex = colorIndex;
		}
		#region Properties
		public XlBorderLineStyle LineStyle { get { return lineStyle; } }
		public int ColorIndex { get { return colorIndex; } }
		public bool IsEmptyBorder {
			get {
				return
					lineStyle == XlBorderLineStyle.None ||
					lineStyle == SpecialBorderLineStyle.DefaultGrid ||
					lineStyle == SpecialBorderLineStyle.PrintGrid;
			}
		}
		#endregion
	}
	#endregion
	#region ActualCellBorderAccessorBase (abstract class)
	public abstract class ActualCellBorderAccessor {
		#region Static Members
		static ActualCellBorderAccessor left = new ActualCellBorderLeftAccessor();
		static ActualCellBorderAccessor right = new ActualCellBorderRightAccessor();
		static ActualCellBorderAccessor top = new ActualCellBorderTopAccessor();
		static ActualCellBorderAccessor bottom = new ActualCellBorderBottomAccessor();
		public static ActualCellBorderAccessor Left { get { return left; } }
		public static ActualCellBorderAccessor Right { get { return right; } }
		public static ActualCellBorderAccessor Top { get { return top; } }
		public static ActualCellBorderAccessor Bottom { get { return bottom; } }
		#endregion
		public abstract DifferentialFormatDisplayBorderDescriptor Descriptor { get; }
		public abstract XlBorderLineStyle GetLineStyle(BorderInfo info);
		public abstract int GetLineColorIndex(BorderInfo info);
		public abstract XlBorderLineStyle GetLineStyle(IActualBorderInfo info);
		public abstract int GetLineColorIndex(IActualBorderInfo info);
		public abstract bool CheckEmptyMergedBorder(CellPosition position, CellPosition topLeft, CellPosition bottomRight);
		public abstract bool GetHasBorderElement(ActualBorderInfo info);
		public abstract ActualBorderElementInfo GetBorderElement(ActualBorderInfo info);
		public abstract void SetBorderElement(ActualBorderInfo info, ActualBorderElementInfo value);
		public abstract void SetHasBorderElement(ActualBorderInfo info, bool value);
		public ActualBorderElementInfo CreateActualBorder(BorderInfo info) {
			XlBorderLineStyle lineStyle = GetLineStyle(info);
			int colorIndex = GetLineColorIndex(info);
			return new ActualBorderElementInfo(lineStyle, colorIndex);
		}
		public ActualBorderElementInfo CreateActualBorder(IActualBorderInfo info) {
			XlBorderLineStyle lineStyle = GetLineStyle(info);
			int colorIndex = GetLineColorIndex(info);
			return new ActualBorderElementInfo(lineStyle, colorIndex);
		}
	}
	#endregion
	#region ActualCellBorderLeftAccessor
	public class ActualCellBorderLeftAccessor : ActualCellBorderAccessor {
		public override DifferentialFormatDisplayBorderDescriptor Descriptor { get { return DifferentialFormatDisplayBorderDescriptor.LeftInfo; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.LeftLineStyle;
		}
		public override int GetLineColorIndex(BorderInfo info) {
			return info.LeftColorIndex;
		}
		public override XlBorderLineStyle GetLineStyle(IActualBorderInfo info) {
			return info.LeftLineStyle;
		}
		public override int GetLineColorIndex(IActualBorderInfo info) {
			return info.LeftColorIndex;
		}
		public override bool CheckEmptyMergedBorder(CellPosition position, CellPosition topLeft, CellPosition bottomRight) {
			return position.Column > topLeft.Column;
		}
		public override bool GetHasBorderElement(ActualBorderInfo info) {
			return info.HasLeftBorder;
		}
		public override ActualBorderElementInfo GetBorderElement(ActualBorderInfo info) {
			return info.LeftBorder;
		}
		public override void SetBorderElement(ActualBorderInfo info, ActualBorderElementInfo value) {
			info.LeftBorder = value; 
		}
		public override void SetHasBorderElement(ActualBorderInfo info, bool value) {
			info.HasLeftBorder = value;
		}
	}
	#endregion
	#region ActualCellBorderRightAccessor
	public class ActualCellBorderRightAccessor : ActualCellBorderAccessor {
		public override DifferentialFormatDisplayBorderDescriptor Descriptor { get { return DifferentialFormatDisplayBorderDescriptor.RightInfo; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.RightLineStyle;
		}
		public override int GetLineColorIndex(BorderInfo info) {
			return info.RightColorIndex;
		}
		public override XlBorderLineStyle GetLineStyle(IActualBorderInfo info) {
			return info.RightLineStyle;
		}
		public override int GetLineColorIndex(IActualBorderInfo info) {
			return info.RightColorIndex;
		}
		public override bool CheckEmptyMergedBorder(CellPosition position, CellPosition topLeft, CellPosition bottomRight) {
			return position.Column < bottomRight.Column;
		}
		public override bool GetHasBorderElement(ActualBorderInfo info) {
			return info.HasRightBorder;
		}
		public override ActualBorderElementInfo GetBorderElement(ActualBorderInfo info) {
			return info.RightBorder;
		}
		public override void SetBorderElement(ActualBorderInfo info, ActualBorderElementInfo value) {
			info.RightBorder = value;
		}
		public override void SetHasBorderElement(ActualBorderInfo info, bool value) {
			info.HasRightBorder = value;
		}
	}
	#endregion
	#region ActualCellBorderTopAccessor
	public class ActualCellBorderTopAccessor : ActualCellBorderAccessor {
		public override DifferentialFormatDisplayBorderDescriptor Descriptor { get { return DifferentialFormatDisplayBorderDescriptor.TopInfo; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.TopLineStyle;
		}
		public override int GetLineColorIndex(BorderInfo info) {
			return info.TopColorIndex;
		}
		public override XlBorderLineStyle GetLineStyle(IActualBorderInfo info) {
			return info.TopLineStyle;
		}
		public override int GetLineColorIndex(IActualBorderInfo info) {
			return info.TopColorIndex;
		}
		public override bool CheckEmptyMergedBorder(CellPosition position, CellPosition topLeft, CellPosition bottomRight) {
			return position.Row > topLeft.Row;
		}
		public override bool GetHasBorderElement(ActualBorderInfo info) {
			return info.HasTopBorder;
		}
		public override ActualBorderElementInfo GetBorderElement(ActualBorderInfo info) {
			return info.TopBorder;
		}
		public override void SetBorderElement(ActualBorderInfo info, ActualBorderElementInfo value) {
			info.TopBorder = value;
		}
		public override void SetHasBorderElement(ActualBorderInfo info, bool value) {
			info.HasTopBorder = value;
		}
	}
	#endregion
	#region ActualCellBorderBottomAccessor
	public class ActualCellBorderBottomAccessor : ActualCellBorderAccessor {
		public override DifferentialFormatDisplayBorderDescriptor Descriptor { get { return DifferentialFormatDisplayBorderDescriptor.BottomInfo; } }
		public override XlBorderLineStyle GetLineStyle(BorderInfo info) {
			return info.BottomLineStyle;
		}
		public override int GetLineColorIndex(BorderInfo info) {
			return info.BottomColorIndex;
		}
		public override XlBorderLineStyle GetLineStyle(IActualBorderInfo info) {
			return info.BottomLineStyle;
		}
		public override int GetLineColorIndex(IActualBorderInfo info) {
			return info.BottomColorIndex;
		}
		public override bool CheckEmptyMergedBorder(CellPosition position, CellPosition topLeft, CellPosition bottomRight) {
			return position.Row < bottomRight.Row;
		}
		public override bool GetHasBorderElement(ActualBorderInfo info) {
			return info.HasBottomBorder;
		}
		public override ActualBorderElementInfo GetBorderElement(ActualBorderInfo info) {
			return info.BottomBorder;
		}
		public override void SetBorderElement(ActualBorderInfo info, ActualBorderElementInfo value) {
			info.BottomBorder = value;
		}
		public override void SetHasBorderElement(ActualBorderInfo info, bool value) {
			info.HasBottomBorder = value;
		}
	}
	#endregion
	#region ActualBorderInfo
	public class ActualBorderInfo {
		#region Fields
		ActualBorderElementInfo leftBorder;
		ActualBorderElementInfo rightBorder;
		ActualBorderElementInfo topBorder;
		ActualBorderElementInfo bottomBorder;
		bool hasLeftBorder;
		bool hasRightBorder;
		bool hasTopBorder;
		bool hasBottomBorder;
		#endregion
		#region Properties
		protected internal ActualBorderElementInfo LeftBorder { get { return leftBorder; } set { leftBorder = value; } }
		protected internal ActualBorderElementInfo RightBorder { get { return rightBorder; } set { rightBorder = value; } }
		protected internal ActualBorderElementInfo TopBorder { get { return topBorder; } set { topBorder = value; } }
		protected internal ActualBorderElementInfo BottomBorder { get { return bottomBorder; } set { bottomBorder = value; } }
		protected internal bool HasLeftBorder { get { return hasLeftBorder; } set { hasLeftBorder = value; } }
		protected internal bool HasRightBorder { get { return hasRightBorder; } set { hasRightBorder = value; } }
		protected internal bool HasTopBorder { get { return hasTopBorder; } set { hasTopBorder = value; } }
		protected internal bool HasBottomBorder { get { return hasBottomBorder; } set { hasBottomBorder = value; } }
		#endregion
		protected internal ActualBorderElementInfo CreateBorderElement(Func<ActualCellBorderAccessor, ActualBorderElementInfo> builder, ActualCellBorderAccessor accessor) {
			if (accessor.GetHasBorderElement(this))
				return accessor.GetBorderElement(this);
			ActualBorderElementInfo result = builder(accessor);
			accessor.SetBorderElement(this, result);
			accessor.SetHasBorderElement(this, true);
			return result;
		}
	}
	#endregion
	#region ImportOptionsBorderInfo
	public class ImportOptionsBorderInfo {
		internal const int CountBorderElements = 7;
		readonly bool[] applyLineStyles = new bool[CountBorderElements];
		readonly bool[] applyColorIndexes = new bool[CountBorderElements];
		bool applyDiagonalUp;
		bool applyDiagonalDown;
		bool applyOutline;
		public bool[] ApplyLineStyles { get { return applyLineStyles; } }
		public bool[] ApplyColorIndexes { get { return applyColorIndexes; } }
		public bool ApplyDiagonalUp { get { return applyDiagonalUp; } set { applyDiagonalUp = value; } }
		public bool ApplyDiagonalDown { get { return applyDiagonalDown; } set { applyDiagonalDown = value; } }
		public bool ApplyOutline { get { return applyOutline; } set { applyOutline = value; } }
	}
	#endregion
	#region UIBorderInfoItem
	public class UIBorderInfoItem {
		XlBorderLineStyle lineStyle;
		Color color;
		public XlBorderLineStyle LineStyle { get { return lineStyle; } set { lineStyle = value; } }
		public Color Color { get { return color; } set { color = value; } }
	}
	#endregion
	#region UIBorderInfoRepository
	public class UIBorderInfoRepository {
		#region Fields
		readonly UIBorderInfoItem currentItem;
		readonly List<UIBorderInfoItem> items;
		int currentItemIndex;
		#endregion
		public UIBorderInfoRepository() {
			this.items = new List<UIBorderInfoItem>();
			this.currentItemIndex = 1;
			PopulateItems();
			currentItem = new UIBorderInfoItem();
			currentItem.Color = DXColor.Black;
			currentItem.LineStyle = Items[CurrentItemIndex].LineStyle;
		}
		#region Properties
		public List<UIBorderInfoItem> Items { get { return items; } }
		public int CurrentItemIndex {
			get { return currentItemIndex; }
			set {
				if (currentItemIndex == value)
					return;
				currentItemIndex = value;
				currentItem.LineStyle = Items[currentItemIndex].LineStyle;
				RaiseUpdateUI();
			}
		}
		public UIBorderInfoItem CurrentItem { get { return currentItem; } }
		#endregion
		#region Events
		#region UpdateUI
		EventHandler onUpdateUI;
		protected internal event EventHandler UpdateUI { add { onUpdateUI += value; } remove { onUpdateUI -= value; } }
		protected internal virtual void RaiseUpdateUI() {
			if (onUpdateUI != null)
				onUpdateUI(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal virtual void PopulateItems() {
			AddItem(XlBorderLineStyle.None);
			AddItem(XlBorderLineStyle.Thin);
			AddItem(XlBorderLineStyle.Hair);
			AddItem(XlBorderLineStyle.Dotted);
			AddItem(XlBorderLineStyle.Dashed);
			AddItem(XlBorderLineStyle.DashDot);
			AddItem(XlBorderLineStyle.DashDotDot);
			AddItem(XlBorderLineStyle.Double);
			AddItem(XlBorderLineStyle.Medium);
			AddItem(XlBorderLineStyle.MediumDashed);
			AddItem(XlBorderLineStyle.MediumDashDot);
			AddItem(XlBorderLineStyle.MediumDashDotDot);
			AddItem(XlBorderLineStyle.SlantDashDot);
			AddItem(XlBorderLineStyle.Thick);
		}
		protected internal virtual void AddItem(XlBorderLineStyle lineStyle) {
			UIBorderInfoItem borderInfo = new UIBorderInfoItem();
			borderInfo.Color = DXColor.Black;
			borderInfo.LineStyle = lineStyle;
			Items.Add(borderInfo);
		}
		public int GetItemIndexByLineStyle(XlBorderLineStyle style) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				if (items[i].LineStyle == style) {
					return i;
				}
			}
			return -1;
		}
		public UIBorderInfoItem GetItemByLineStyle(XlBorderLineStyle style) {
			int index = GetItemIndexByLineStyle(style);
			if (index < 0)
				return null;
			else
				return items[index];
		}
	}
	#endregion
}
