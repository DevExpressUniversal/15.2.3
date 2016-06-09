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
using DevExpress.Office.Drawing;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region Cell : IBorderInfo
	partial class Cell {
		public IBorderInfo Border { get { return this; } }
		public IActualBorderInfo ActualBorder { get { return this; } } 
		protected internal virtual IActualBorderInfo InnerActualBorder { get { return FormatInfo.ActualBorder; } }
		#region IBorderInfo Members
		#region IBorderInfo.LeftLineStyle
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get { return FormatInfo.Border.LeftLineStyle; }
			set {
				SetBorderPropertyValue(SetBorderLeftLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.LeftLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.LeftColor
		Color IBorderInfo.LeftColor {
			get { return FormatInfo.Border.LeftColor; }
			set {
				SetBorderPropertyValue(SetBorderLeftColor, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColor(FormatBase info, Color value) {
			info.Border.LeftColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.LeftColorIndex
		int IBorderInfo.LeftColorIndex {
			get { return FormatInfo.Border.LeftColorIndex; }
			set {
				SetBorderPropertyValue(SetBorderLeftColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColorIndex(FormatBase info, int value) {
			info.Border.LeftColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightLineStyle
		XlBorderLineStyle IBorderInfo.RightLineStyle {
			get { return FormatInfo.Border.RightLineStyle; }
			set {
				SetBorderPropertyValue(SetBorderRightLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderRightLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.RightLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightColor
		Color IBorderInfo.RightColor {
			get { return FormatInfo.Border.RightColor; }
			set {
				SetBorderPropertyValue(SetBorderRightColor, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColor(FormatBase info, Color value) {
			info.Border.RightColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightColorIndex
		int IBorderInfo.RightColorIndex {
			get { return FormatInfo.Border.RightColorIndex; }
			set {
				SetBorderPropertyValue(SetBorderRightColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColorIndex(FormatBase info, int value) {
			info.Border.RightColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopLineStyle
		XlBorderLineStyle IBorderInfo.TopLineStyle {
			get { return FormatInfo.Border.TopLineStyle; }
			set {
				SetBorderPropertyValue(SetBorderTopLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderTopLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.TopLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopColor
		Color IBorderInfo.TopColor {
			get { return FormatInfo.Border.TopColor; }
			set {
				SetBorderPropertyValue(SetBorderTopColor, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColor(FormatBase info, Color value) {
			info.Border.TopColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopColorIndex
		int IBorderInfo.TopColorIndex {
			get { return FormatInfo.Border.TopColorIndex; }
			set {
				SetBorderPropertyValue(SetBorderTopColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColorIndex(FormatBase info, int value) {
			info.Border.TopColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomLineStyle
		XlBorderLineStyle IBorderInfo.BottomLineStyle {
			get { return FormatInfo.Border.BottomLineStyle; }
			set {
				SetBorderPropertyValue(SetBorderBottomLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.BottomLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomColor
		Color IBorderInfo.BottomColor {
			get { return FormatInfo.Border.BottomColor; }
			set {
				SetBorderPropertyValue(SetBorderBottomColor, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColor(FormatBase info, Color value) {
			info.Border.BottomColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomColorIndex
		int IBorderInfo.BottomColorIndex {
			get { return FormatInfo.Border.BottomColorIndex; }
			set {
				SetBorderPropertyValue(SetBorderBottomColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColorIndex(FormatBase info, int value) {
			info.Border.BottomColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalLineStyle
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle {
			get { return FormatInfo.Border.HorizontalLineStyle; }
			set {
				SetBorderPropertyValue(SetBorderHorizontalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.HorizontalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalColor
		Color IBorderInfo.HorizontalColor {
			get { return FormatInfo.Border.HorizontalColor; }
			set {
				SetBorderPropertyValue(SetBorderHorizontalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColor(FormatBase info, Color value) {
			info.Border.HorizontalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalColorIndex
		int IBorderInfo.HorizontalColorIndex {
			get { return FormatInfo.Border.HorizontalColorIndex; }
			set {
				SetBorderPropertyValue(SetBorderHorizontalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColorIndex(FormatBase info, int value) {
			info.Border.HorizontalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalLineStyle
		XlBorderLineStyle IBorderInfo.VerticalLineStyle {
			get { return FormatInfo.Border.VerticalLineStyle; }
			set {
				SetBorderPropertyValue(SetBorderVerticalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.VerticalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalColor
		Color IBorderInfo.VerticalColor {
			get { return FormatInfo.Border.VerticalColor; }
			set {
				SetBorderPropertyValue(SetBorderVerticalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColor(FormatBase info, Color value) {
			info.Border.VerticalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalColorIndex
		int IBorderInfo.VerticalColorIndex {
			get { return FormatInfo.Border.VerticalColorIndex; }
			set {
				SetBorderPropertyValue(SetBorderVerticalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColorIndex(FormatBase info, int value) {
			info.Border.VerticalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalUpLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle {
			get { return FormatInfo.Border.DiagonalUpLineStyle; }
			set {
				SetBorderPropertyValue(SetBorderDiagonalUpLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalUpLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalUpLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalDownLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle {
			get { return FormatInfo.Border.DiagonalDownLineStyle; }
			set {
				SetBorderPropertyValue(SetBorderDiagonalDownLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalDownLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalDownLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalColor
		Color IBorderInfo.DiagonalColor {
			get { return FormatInfo.Border.DiagonalColor; }
			set {
				SetBorderPropertyValue(SetBorderDiagonalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColor(FormatBase info, Color value) {
			info.Border.DiagonalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalColorIndex
		int IBorderInfo.DiagonalColorIndex {
			get { return FormatInfo.Border.DiagonalColorIndex; }
			set {
				SetBorderPropertyValue(SetBorderDiagonalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColorIndex(FormatBase info, int value) {
			info.Border.DiagonalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.Outline
		bool IBorderInfo.Outline {
			get { return FormatInfo.Border.Outline; }
			set {
				SetBorderPropertyValue(SetBorderOutline, value);
			}
		}
		DocumentModelChangeActions SetBorderOutline(FormatBase info, bool value) {
			info.Border.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		protected internal  void SetBorderPropertyValue<U>(SetPropertyValueDelegate<U> setter, U newValue) {
			DocumentModel.BeginUpdate();
			try {
					FormatBase info = GetInfoForModification();
					info.SetActualBorder(ActualBorder);
					DocumentModelChangeActions changeActions = setter(info, newValue);
					ReplaceInfo(info, changeActions);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region IActualBorderInfo Members
		XlBorderLineStyle IActualBorderInfo.LeftLineStyle {
			get {
				return GetActualBorderLeftLineStyle();
			}
		}
		XlBorderLineStyle IActualBorderInfo.RightLineStyle {
			get {
				return GetActualBorderRightLineStyle();
			}
		}
		XlBorderLineStyle IActualBorderInfo.TopLineStyle {
			get {
				return GetActualBorderTopLineStyle();
			}
		}
		XlBorderLineStyle IActualBorderInfo.BottomLineStyle {
			get {
				return GetActualBorderBottomLineStyle();
			}
		}
		XlBorderLineStyle IActualBorderInfo.HorizontalLineStyle { get { return GetActualBorderHorizontalLineStyle(); ; } }
		XlBorderLineStyle IActualBorderInfo.VerticalLineStyle { get { return GetActualBorderVerticalLineStyle(); ; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalUpLineStyle { get { return GetActualBorderDiagonalUpLineStyle(); ; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalDownLineStyle { get { return GetActualBorderDiagonalDownLineStyle(); ; } }
		Color IActualBorderInfo.LeftColor { get { return GetRgbColor(ActualBorder.LeftColorIndex ); } }
		Color IActualBorderInfo.RightColor { get { return GetRgbColor(ActualBorder.RightColorIndex ); } }
		Color IActualBorderInfo.TopColor { get { return GetRgbColor(ActualBorder.TopColorIndex ); } }
		Color IActualBorderInfo.BottomColor { get { return GetRgbColor(ActualBorder.BottomColorIndex ); } }
		Color IActualBorderInfo.HorizontalColor { get { return GetRgbColor(GetActualBorderHorizontalColorIndex()); } }
		Color IActualBorderInfo.VerticalColor { get { return GetRgbColor(GetActualBorderVerticalColorIndex()); } }
		Color IActualBorderInfo.DiagonalColor { get { return GetRgbColor(GetActualBorderDiagonalColorIndex()); } }
		int IActualBorderInfo.LeftColorIndex {
			get {
				return GetActualBorderLeftColorIndex();
			}
		}
		int IActualBorderInfo.RightColorIndex {
			get {
				return GetActualBorderRightColorIndex();
			}
		}
		int IActualBorderInfo.TopColorIndex {
			get {
				return GetActualBorderTopColorIndex();
			}
		}
		int IActualBorderInfo.BottomColorIndex {
			get {
				return GetActualBorderBottomColorIndex();
			}
		}
		int IActualBorderInfo.HorizontalColorIndex { get { return GetActualBorderHorizontalColorIndex(); } }
		int IActualBorderInfo.VerticalColorIndex { get { return GetActualBorderVerticalColorIndex(); } }
		int IActualBorderInfo.DiagonalColorIndex { get { return GetActualBorderDiagonalColorIndex(); } }
		bool IActualBorderInfo.Outline { get { return GetActualBorderOutline(); } }
		#endregion
		#region GetActualBorderValue
		protected T GetActualDisplayBorderValueCore<T>(T cellActualValue, bool actualApply, DifferentialFormatDisplayBorderDescriptor propertyDescriptor) {
			return TableStyleFormatBuilderFactory.DisplayBorderBuilder.Build(cellActualValue, actualApply, propertyDescriptor, Position, Worksheet);
		}
		protected virtual T GetActualDisplayBorderValue<T>(T cellFormatActualValue, DifferentialFormatDisplayBorderDescriptor propertyDescriptor) {
			return GetActualDisplayBorderValueCore(cellFormatActualValue, ActualApplyInfo.ApplyBorder, propertyDescriptor);
		}
		protected virtual XlBorderLineStyle GetActualBorderLeftLineStyle() {
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.BorderAssigned)
					return DocumentModel.Cache.BorderInfoCache[conditionalFormatAccumulator.BorderIndex].LeftLineStyle;
			}
			return GetActualDisplayBorderValue(InnerActualBorder.LeftLineStyle, DifferentialFormatDisplayBorderDescriptor.LeftLineStyle);
		}
		protected virtual XlBorderLineStyle GetActualBorderRightLineStyle() {
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.BorderAssigned)
					return DocumentModel.Cache.BorderInfoCache[conditionalFormatAccumulator.BorderIndex].RightLineStyle;
			}
			return GetActualDisplayBorderValue(InnerActualBorder.RightLineStyle, DifferentialFormatDisplayBorderDescriptor.RightLineStyle);
		}
		protected virtual XlBorderLineStyle GetActualBorderTopLineStyle() {
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.BorderAssigned)
					return DocumentModel.Cache.BorderInfoCache[conditionalFormatAccumulator.BorderIndex].TopLineStyle;
			}
			return GetActualDisplayBorderValue(InnerActualBorder.TopLineStyle, DifferentialFormatDisplayBorderDescriptor.TopLineStyle);
		}
		protected virtual XlBorderLineStyle GetActualBorderBottomLineStyle() {
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.BorderAssigned)
					return DocumentModel.Cache.BorderInfoCache[conditionalFormatAccumulator.BorderIndex].BottomLineStyle;
			}
			return GetActualDisplayBorderValue(InnerActualBorder.BottomLineStyle, DifferentialFormatDisplayBorderDescriptor.BottomLineStyle);
		}
		protected virtual int GetActualBorderLeftColorIndex() {
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.BorderAssigned)
					return DocumentModel.Cache.BorderInfoCache[conditionalFormatAccumulator.BorderIndex].LeftColorIndex;
			}
			return GetActualDisplayBorderValue(InnerActualBorder.LeftColorIndex, DifferentialFormatDisplayBorderDescriptor.LeftColorIndex);
		}
		protected virtual int GetActualBorderRightColorIndex() {
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.BorderAssigned)
					return DocumentModel.Cache.BorderInfoCache[conditionalFormatAccumulator.BorderIndex].RightColorIndex;
			}
			return GetActualDisplayBorderValue(InnerActualBorder.RightColorIndex, DifferentialFormatDisplayBorderDescriptor.RightColorIndex);
		}
		protected virtual int GetActualBorderTopColorIndex() {
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.BorderAssigned)
					return DocumentModel.Cache.BorderInfoCache[conditionalFormatAccumulator.BorderIndex].TopColorIndex;
			}
			return GetActualDisplayBorderValue(InnerActualBorder.TopColorIndex, DifferentialFormatDisplayBorderDescriptor.TopColorIndex);
		}
		protected virtual int GetActualBorderBottomColorIndex() {
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.BorderAssigned)
					return DocumentModel.Cache.BorderInfoCache[conditionalFormatAccumulator.BorderIndex].BottomColorIndex;
			}
			return GetActualDisplayBorderValue(InnerActualBorder.BottomColorIndex, DifferentialFormatDisplayBorderDescriptor.BottomColorIndex);
		}
		protected T GetActualBorderValueCore<T>(T cellActualValue, bool actualApply, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return TableStyleFormatBuilderFactory.PropertyBuilder.Build(cellActualValue, actualApply, propertyDescriptor, Position, Worksheet);
		}
		protected virtual T GetActualBorderValue<T>(T cellFormatActualValue, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualBorderValueCore(cellFormatActualValue, ActualApplyInfo.ApplyBorder, propertyDescriptor);
		}
		protected virtual XlBorderLineStyle GetActualBorderDiagonalUpLineStyle() {
			return GetActualBorderValue(InnerActualBorder.DiagonalUpLineStyle, DifferentialFormatPropertyDescriptor.BorderDiagonalUpLineStyle);
		}
		protected virtual XlBorderLineStyle GetActualBorderDiagonalDownLineStyle() {
			return GetActualBorderValue(InnerActualBorder.DiagonalDownLineStyle, DifferentialFormatPropertyDescriptor.BorderDiagonalDownLineStyle);
		}
		protected virtual XlBorderLineStyle GetActualBorderHorizontalLineStyle() {
			return GetActualBorderValue(InnerActualBorder.HorizontalLineStyle, DifferentialFormatPropertyDescriptor.BorderHorizontalLineStyle);
		}
		protected virtual XlBorderLineStyle GetActualBorderVerticalLineStyle() {
			return GetActualBorderValue(InnerActualBorder.VerticalLineStyle, DifferentialFormatPropertyDescriptor.BorderVerticalLineStyle);
		}
		protected virtual int GetActualBorderDiagonalColorIndex() {
			return GetActualBorderValue(InnerActualBorder.DiagonalColorIndex, DifferentialFormatPropertyDescriptor.BorderDiagonalColorIndex);
		}
		protected virtual int GetActualBorderHorizontalColorIndex() {
			return GetActualBorderValue(InnerActualBorder.HorizontalColorIndex, DifferentialFormatPropertyDescriptor.BorderHorizontalColorIndex);
		}
		protected virtual int GetActualBorderVerticalColorIndex() {
			return GetActualBorderValue(InnerActualBorder.VerticalColorIndex, DifferentialFormatPropertyDescriptor.BorderVerticalColorIndex);
		}
		protected virtual bool GetActualBorderOutline() {
			return GetActualBorderValue(InnerActualBorder.Outline, DifferentialFormatPropertyDescriptor.BorderOutline);
		}
		#endregion
	}
	#endregion
	#region DefaultGridBorder
	public class DefaultGridBorder : IActualBorderInfo {
		readonly DocumentModel workbook;
		readonly IGridlinesColorAccessor gridColorAccessor;
		int colorIndex;
		public DefaultGridBorder(DocumentModel workbook, IGridlinesColorAccessor gridColorAccessor) {
			Guard.ArgumentNotNull(workbook, "workbook");
			Guard.ArgumentNotNull(gridColorAccessor, "gridColorAccessor");
			this.workbook = workbook;
			this.gridColorAccessor = gridColorAccessor;
			ColorModelInfo color = new ColorModelInfo();
			color.Rgb = CalculateActualGridlineColor();
			this.colorIndex = workbook.Cache.ColorModelInfoCache.GetItemIndex(color);
		}
		protected virtual XlBorderLineStyle DefaultBorderLineStyle { get { return SpecialBorderLineStyle.DefaultGrid; } }
		IActualBorderInfo NormalStyleActualBorder { get { return workbook.StyleSheet.CellStyles.Normal.ActualBorder; } }
		protected virtual Color CalculateActualGridlineColor() {
			if (!DXColor.IsTransparentOrEmpty(gridColorAccessor.GridlinesColor))
				return gridColorAccessor.GridlinesColor;
			if (!DXColor.IsTransparentOrEmpty(workbook.SkinGridlineColor))
				return workbook.SkinGridlineColor;
			return DXColor.FromArgb(192, 192, 192);
		}
		protected Color GetColor(int colorIndex) {
			Color color = workbook.Cache.ColorModelInfoCache[colorIndex].ToRgb(workbook.StyleSheet.Palette, workbook.OfficeTheme.Colors);
			return color;
		}
		#region IActualBorderInfo Members
		XlBorderLineStyle IActualBorderInfo.LeftLineStyle { get { return GetLeftLineStyle(); } }
		XlBorderLineStyle IActualBorderInfo.RightLineStyle { get { return GetRightLineStyle(); } }
		XlBorderLineStyle IActualBorderInfo.TopLineStyle { get { return GetTopLineStyle(); } }
		XlBorderLineStyle IActualBorderInfo.BottomLineStyle { get { return GetBottomLineStyle(); } }
		XlBorderLineStyle IActualBorderInfo.HorizontalLineStyle { get { return GetHorizontalLineStyle(); } }
		XlBorderLineStyle IActualBorderInfo.VerticalLineStyle { get { return GetVerticalLineStyle(); } }
		XlBorderLineStyle IActualBorderInfo.DiagonalUpLineStyle { get { return XlBorderLineStyle.None; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalDownLineStyle { get { return XlBorderLineStyle.None; } }
		Color IActualBorderInfo.LeftColor { get { return GetColor(GetLeftColorIndex()); } }
		Color IActualBorderInfo.RightColor { get { return GetColor(GetRightColorIndex()); } }
		Color IActualBorderInfo.TopColor { get { return GetColor(GetTopColorIndex()); } }
		Color IActualBorderInfo.BottomColor { get { return GetColor(GetBottomColorIndex()); } }
		Color IActualBorderInfo.HorizontalColor { get { return GetColor(GetHorizontalColorIndex()); } }
		Color IActualBorderInfo.VerticalColor { get { return GetColor(GetVerticalColorIndex()); } }
		Color IActualBorderInfo.DiagonalColor { get { return DXColor.Empty; } }
		int IActualBorderInfo.LeftColorIndex { get { return GetLeftColorIndex(); } }
		int IActualBorderInfo.RightColorIndex { get { return GetRightColorIndex(); } }
		int IActualBorderInfo.TopColorIndex { get { return GetTopColorIndex(); } }
		int IActualBorderInfo.BottomColorIndex { get { return GetBottomColorIndex(); } }
		int IActualBorderInfo.HorizontalColorIndex { get { return GetHorizontalColorIndex(); } }
		int IActualBorderInfo.VerticalColorIndex { get { return GetVerticalColorIndex(); } }
		int IActualBorderInfo.DiagonalColorIndex { get { return colorIndex; } }
		bool IActualBorderInfo.Outline { get { return false; } }
		#endregion
		bool IsEmptyLineStyle(XlBorderLineStyle lineStyle) {
			return lineStyle == XlBorderLineStyle.None;
		}
		T GetActualValue<T>(XlBorderLineStyle lineStyle, T defaultValue, T value) {
			return IsEmptyLineStyle(lineStyle) ? defaultValue : value;
		}
		XlBorderLineStyle GetActualLineStyle(XlBorderLineStyle normalLineStyle) {
			return GetActualValue(normalLineStyle, DefaultBorderLineStyle, normalLineStyle);
		}
		int GetActualColorIndex(XlBorderLineStyle normalLineStyle, int normalColorIndex) {
			return GetActualValue(normalLineStyle, colorIndex, normalColorIndex);
		}
		#region GetLineStyle methods
		XlBorderLineStyle GetLeftLineStyle() {
			return GetActualLineStyle(NormalStyleActualBorder.LeftLineStyle);
		}
		XlBorderLineStyle GetRightLineStyle() {
			return GetActualLineStyle(NormalStyleActualBorder.RightLineStyle);
		}
		XlBorderLineStyle GetTopLineStyle() {
			return GetActualLineStyle(NormalStyleActualBorder.TopLineStyle);
		}
		XlBorderLineStyle GetBottomLineStyle() {
			return GetActualLineStyle(NormalStyleActualBorder.BottomLineStyle);
		}
		XlBorderLineStyle GetHorizontalLineStyle() {
			return GetActualLineStyle(NormalStyleActualBorder.HorizontalLineStyle);
		}
		XlBorderLineStyle GetVerticalLineStyle() {
			return GetActualLineStyle(NormalStyleActualBorder.VerticalLineStyle);
		}
		#endregion
		#region GetColorIndex methods
		int GetLeftColorIndex() {
			return GetActualColorIndex(NormalStyleActualBorder.LeftLineStyle, NormalStyleActualBorder.LeftColorIndex);
		}
		int GetRightColorIndex() {
			return GetActualColorIndex(NormalStyleActualBorder.RightLineStyle, NormalStyleActualBorder.RightColorIndex);
		}
		int GetTopColorIndex() {
			return GetActualColorIndex(NormalStyleActualBorder.TopLineStyle, NormalStyleActualBorder.TopColorIndex);
		}
		int GetBottomColorIndex() {
			return GetActualColorIndex(NormalStyleActualBorder.BottomLineStyle, NormalStyleActualBorder.BottomColorIndex);
		}
		int GetHorizontalColorIndex() {
			return GetActualColorIndex(NormalStyleActualBorder.HorizontalLineStyle, NormalStyleActualBorder.HorizontalColorIndex);
		}
		int GetVerticalColorIndex() {
			return GetActualColorIndex(NormalStyleActualBorder.VerticalLineStyle, NormalStyleActualBorder.VerticalColorIndex);
		}
		#endregion
	}
	#endregion
	#region PrintGridBorder
	public class PrintGridBorder : DefaultGridBorder {
		public PrintGridBorder(DocumentModel workbook, IGridlinesColorAccessor gridColorAccessor)
			: base(workbook, gridColorAccessor) {
		}
		protected override XlBorderLineStyle DefaultBorderLineStyle { get { return SpecialBorderLineStyle.PrintGrid; } }
		protected override Color CalculateActualGridlineColor() {
			return DXColor.Black;
		}
	}
	#endregion
	#region EmptyBorder
	public class EmptyBorder : IActualBorderInfo {
		readonly DocumentModel documentModel;
		const int defaultColorIndex = 0;
		public EmptyBorder(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		protected virtual XlBorderLineStyle DefaultBorderLineStyle { get { return XlBorderLineStyle.None; } }
		protected Color GetColor() {
			Color color = documentModel.Cache.ColorModelInfoCache[defaultColorIndex].ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors);
			return color;
		}
		#region IActualBorderInfo Members
		XlBorderLineStyle IActualBorderInfo.LeftLineStyle { get { return DefaultBorderLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.RightLineStyle { get { return DefaultBorderLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.TopLineStyle { get { return DefaultBorderLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.BottomLineStyle { get { return DefaultBorderLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.HorizontalLineStyle { get { return DefaultBorderLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.VerticalLineStyle { get { return DefaultBorderLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalUpLineStyle { get { return XlBorderLineStyle.None; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalDownLineStyle { get { return XlBorderLineStyle.None; } }
		Color IActualBorderInfo.LeftColor { get { return GetColor(); } }
		Color IActualBorderInfo.RightColor { get { return GetColor(); } }
		Color IActualBorderInfo.TopColor { get { return GetColor(); } }
		Color IActualBorderInfo.BottomColor { get { return GetColor(); } }
		Color IActualBorderInfo.HorizontalColor { get { return GetColor(); } }
		Color IActualBorderInfo.VerticalColor { get { return GetColor(); } }
		Color IActualBorderInfo.DiagonalColor { get { return DXColor.Empty; } }
		int IActualBorderInfo.LeftColorIndex { get { return defaultColorIndex; } }
		int IActualBorderInfo.RightColorIndex { get { return defaultColorIndex; } }
		int IActualBorderInfo.TopColorIndex { get { return defaultColorIndex; } }
		int IActualBorderInfo.BottomColorIndex { get { return defaultColorIndex; } }
		int IActualBorderInfo.HorizontalColorIndex { get { return defaultColorIndex; } }
		int IActualBorderInfo.VerticalColorIndex { get { return defaultColorIndex; } }
		int IActualBorderInfo.DiagonalColorIndex { get { return defaultColorIndex; } }
		bool IActualBorderInfo.Outline { get { return false; } }
		#endregion
	}
	#endregion
}
