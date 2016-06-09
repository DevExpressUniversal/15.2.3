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
using DevExpress.XtraSpreadsheet.Model.History;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Office;
using System.Runtime.InteropServices;
using DevExpress.Office.History;
using System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class Table : ITableBorderFormatPropertyChanger {
		#region ITableBorderFormatPropertyChanger Members
		#region TableBorderLeftLineStyle
		XlBorderLineStyle ITableBorderFormatPropertyChanger.GetTableBorderLeftLineStyle(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.LeftLineStyle;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderLeftLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.LeftLineStyle == value && info.BorderOptionsInfo.ApplyLeftLineStyle)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderLeftLineStyleCore, value);
		}
		#endregion
		#region TableBorderRightLineStyle
		XlBorderLineStyle ITableBorderFormatPropertyChanger.GetTableBorderRightLineStyle(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.RightLineStyle;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderRightLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.RightLineStyle == value && info.BorderOptionsInfo.ApplyRightLineStyle)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderRightLineStyleCore, value);
		}
		#endregion
		#region TableBorderTopLineStyle
		XlBorderLineStyle ITableBorderFormatPropertyChanger.GetTableBorderTopLineStyle(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.TopLineStyle;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderTopLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.TopLineStyle == value && info.BorderOptionsInfo.ApplyTopLineStyle)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderTopLineStyleCore, value);
		}
		#endregion
		#region TableBorderBottomLineStyle
		XlBorderLineStyle ITableBorderFormatPropertyChanger.GetTableBorderBottomLineStyle(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.BottomLineStyle;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderBottomLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.BottomLineStyle == value && info.BorderOptionsInfo.ApplyBottomLineStyle)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderBottomLineStyleCore, value);
		}
		#endregion
		#region TableBorderDiagonalUpLineStyle
		XlBorderLineStyle ITableBorderFormatPropertyChanger.GetTableBorderDiagonalUpLineStyle(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.DiagonalUpLineStyle;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderDiagonalUpLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.DiagonalUpLineStyle == value && info.BorderOptionsInfo.ApplyDiagonalLineStyle)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderDiagonalUpLineStyleCore, value);
		}
		#endregion
		#region TableBorderDiagonalDownLineStyle
		XlBorderLineStyle ITableBorderFormatPropertyChanger.GetTableBorderDiagonalDownLineStyle(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.DiagonalDownLineStyle;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderDiagonalDownLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.DiagonalDownLineStyle == value && info.BorderOptionsInfo.ApplyDiagonalLineStyle)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderDiagonalDownLineStyleCore, value);
		}
		#endregion
		#region TableBorderHorizontalLineStyle
		XlBorderLineStyle ITableBorderFormatPropertyChanger.GetTableBorderHorizontalLineStyle(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.HorizontalLineStyle;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderHorizontalLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.HorizontalLineStyle == value && info.BorderOptionsInfo.ApplyHorizontalLineStyle)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderHorizontalLineStyleCore, value);
		}
		#endregion
		#region TableBorderVerticalLineStyle
		XlBorderLineStyle ITableBorderFormatPropertyChanger.GetTableBorderVerticalLineStyle(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.VerticalLineStyle;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderVerticalLineStyle(int elementIndex, XlBorderLineStyle value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.VerticalLineStyle == value && info.BorderOptionsInfo.ApplyVerticalLineStyle)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderVerticalLineStyleCore, value);
		}
		#endregion
		#region TableBorderOutline
		bool ITableBorderFormatPropertyChanger.GetTableBorderOutline(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.Outline;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderOutline(int elementIndex, bool value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.Outline == value && info.BorderOptionsInfo.ApplyOutline)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderOutlineCore, value);
		}
		#endregion
		#region TableBorderLeftColor
		Color ITableBorderFormatPropertyChanger.GetTableBorderLeftColor(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.LeftColor;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderLeftColor(int elementIndex, Color value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.LeftColor == value && info.BorderOptionsInfo.ApplyLeftColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderLeftColorCore, value);
		}
		#endregion
		#region TableBorderRightColor
		Color ITableBorderFormatPropertyChanger.GetTableBorderRightColor(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.RightColor;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderRightColor(int elementIndex, Color value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.RightColor == value && info.BorderOptionsInfo.ApplyRightColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderRightColorCore, value);
		}
		#endregion
		#region TableBorderTopColor
		Color ITableBorderFormatPropertyChanger.GetTableBorderTopColor(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.TopColor;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderTopColor(int elementIndex, Color value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.TopColor == value && info.BorderOptionsInfo.ApplyTopColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderTopColorCore, value);
		}
		#endregion
		#region TableBorderBottomColor
		Color ITableBorderFormatPropertyChanger.GetTableBorderBottomColor(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.BottomColor;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderBottomColor(int elementIndex, Color value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.BottomColor == value && info.BorderOptionsInfo.ApplyBottomColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderBottomColorCore, value);
		}
		#endregion
		#region TableBorderDiagonalColor
		Color ITableBorderFormatPropertyChanger.GetTableBorderDiagonalColor(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.DiagonalColor;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderDiagonalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.DiagonalColor == value && info.BorderOptionsInfo.ApplyDiagonalColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderDiagonalColorCore, value);
		}
		#endregion
		#region TableBorderHorizontalColor
		Color ITableBorderFormatPropertyChanger.GetTableBorderHorizontalColor(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.HorizontalColor;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderHorizontalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.HorizontalColor == value && info.BorderOptionsInfo.ApplyHorizontalColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderHorizontalColorCore, value);
		}
		#endregion
		#region TableBorderVerticalColor
		Color ITableBorderFormatPropertyChanger.GetTableBorderVerticalColor(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.VerticalColor;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderVerticalColor(int elementIndex, Color value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.VerticalColor == value && info.BorderOptionsInfo.ApplyVerticalColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderVerticalColorCore, value);
		}
		#endregion
		#region TableBorderLeftColorIndex
		int ITableBorderFormatPropertyChanger.GetTableBorderLeftColorIndex(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.LeftColorIndex;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderLeftColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.LeftColorIndex == value && info.BorderOptionsInfo.ApplyLeftColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderLeftColorIndexCore, value);
		}
		#endregion
		#region TableBorderRightColorIndex
		int ITableBorderFormatPropertyChanger.GetTableBorderRightColorIndex(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.RightColorIndex;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderRightColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.RightColorIndex == value && info.BorderOptionsInfo.ApplyRightColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderRightColorIndexCore, value);
		}
		#endregion
		#region TableBorderTopColorIndex
		int ITableBorderFormatPropertyChanger.GetTableBorderTopColorIndex(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.TopColorIndex;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderTopColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.TopColorIndex == value && info.BorderOptionsInfo.ApplyTopColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderTopColorIndexCore, value);
		}
		#endregion
		#region TableBorderBottomColorIndex
		int ITableBorderFormatPropertyChanger.GetTableBorderBottomColorIndex(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.BottomColorIndex;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderBottomColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.BottomColorIndex == value && info.BorderOptionsInfo.ApplyBottomColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderBottomColorIndexCore, value);
		}
		#endregion
		#region TableBorderDiagonalColorIndex
		int ITableBorderFormatPropertyChanger.GetTableBorderDiagonalColorIndex(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.DiagonalColorIndex;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderDiagonalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.DiagonalColorIndex == value && info.BorderOptionsInfo.ApplyDiagonalColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderDiagonalColorIndexCore, value);
		}
		#endregion
		#region TableBorderHorizontalColorIndex
		int ITableBorderFormatPropertyChanger.GetTableBorderHorizontalColorIndex(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.HorizontalColorIndex;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderHorizontalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.HorizontalColorIndex == value && info.BorderOptionsInfo.ApplyHorizontalColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderHorizontalColorIndexCore, value);
		}
		#endregion
		#region TableBorderVerticalColorIndex
		int ITableBorderFormatPropertyChanger.GetTableBorderVerticalColorIndex(int elementIndex) {
			return GetBorderFormat(elementIndex).Border.VerticalColorIndex;
		}
		void ITableBorderFormatPropertyChanger.SetTableBorderVerticalColorIndex(int elementIndex, int value) {
			DifferentialFormat info = GetBorderFormat(elementIndex);
			if (info.Border.VerticalColorIndex == value && info.BorderOptionsInfo.ApplyVerticalColor)
				return;
			SetPropertyValue(GetTableBorderFormatIndexAccessor(elementIndex), SetBorderVerticalColorIndexCore, value);
		}
		#endregion
		#endregion
	}
}
