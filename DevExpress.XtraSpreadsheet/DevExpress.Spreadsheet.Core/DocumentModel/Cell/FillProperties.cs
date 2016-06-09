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
using DevExpress.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region Cell : IFillInfo
	partial class Cell : IFormatBaseBatchUpdateable {
		public IFillInfo Fill { get { return this; } }
		public IActualFillInfo ActualFill { get { return this; } }  
		public IActualGradientFillInfo ActualGradientFillInfo { get { return this; } }
		public IActualConvergenceInfo ActualConvergenceInfo { get { return this; } }
		public virtual IActualFillInfo InnerActualFill { get { return FormatInfo.ActualFill; } }
		public virtual IActualGradientFillInfo InnerActualGradientFillInfo { get { return FormatInfo.ActualFill.GradientFill; } }
		public virtual IActualConvergenceInfo InnerActualConvergenceInfo { get { return FormatInfo.ActualFill.GradientFill.Convergence; } }
		#region IFillInfo Members
		#region IFillInfo.Clear
		void IFillInfo.Clear() {
			ClearFillCore();
		}
		void ClearFillCore() {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification();
				info.Fill.Clear();
				ReplaceInfo(info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region IFillInfo.PatternType
		XlPatternType IFillInfo.PatternType {
			get { return FormatInfo.Fill.PatternType; }
			set {
				SetFillPropertyValue(SetFillPatternType, value);
			}
		}
		DocumentModelChangeActions SetFillPatternType(FormatBase info, XlPatternType value) {
			info.Fill.PatternType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.ForeColor
		Color IFillInfo.ForeColor {
			get { return FormatInfo.Fill.ForeColor; }
			set {
				SetFillPropertyValue(SetFillForeColor, value);
			}
		}
		DocumentModelChangeActions SetFillForeColor(FormatBase info, Color value) {
			info.Fill.ForeColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.BackColor
		Color IFillInfo.BackColor {
			get { return FormatInfo.Fill.BackColor; }
			set {
				SetFillPropertyValue(SetFillBackColor, value);
			}
		}
		DocumentModelChangeActions SetFillBackColor(FormatBase info, Color value) {
			info.Fill.BackColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.GradientFill
		IGradientFillInfo IFillInfo.GradientFill { get { return this; } }
		#endregion
		#region FillType
		ModelFillType IFillInfo.FillType {
			get { return FormatInfo.Fill.FillType; }
			set {
				if (FormatInfo.Fill.FillType == value)
					return;
				SetPropertyValue(SetModelFillType, value);
			}
		}
		DocumentModelChangeActions SetModelFillType(FormatBase info, ModelFillType value) {
			info.Fill.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo Members
		#region IGradientFillInfo.Convergence 
		IConvergenceInfo IGradientFillInfo.Convergence { get { return this; } }
		#endregion
		#region IGradientFillInfo.GradientStops
		IGradientStopCollection IGradientFillInfo.GradientStops { get { return FormatInfo.Fill.GradientFill.GradientStops; } }
		#endregion
		#region IGradientFillInfo.Type
		ModelGradientFillType IGradientFillInfo.Type {
			get { return FormatInfo.Fill.GradientFill.Type; }
			set {
				if (FormatInfo.Fill.GradientFill.Type == value)
					return;
				SetPropertyValue(SetGradientFillInfoType, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoType(FormatBase info, ModelGradientFillType value) {
			info.Fill.GradientFill.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo.Degree
		double IGradientFillInfo.Degree {
			get { return FormatInfo.Fill.GradientFill.Degree; }
			set {
				if (FormatInfo.Fill.GradientFill.Degree == value)
					return;
				SetPropertyValue(SetGradientFillInfoDegree, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoDegree(FormatBase info, double value) {
			info.Fill.GradientFill.Degree = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo Members
		#region IConvergenceInfo.Left
		float IConvergenceInfo.Left {
			get { return FormatInfo.Fill.GradientFill.Convergence.Left; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Left == value)
					return;
				SetPropertyValue(SetGradientFillInfoLeft, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoLeft(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Left = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Right
		float IConvergenceInfo.Right {
			get { return FormatInfo.Fill.GradientFill.Convergence.Right; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Right == value)
					return;
				SetPropertyValue(SetGradientFillInfoRight, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoRight(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Right = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Top
		float IConvergenceInfo.Top {
			get { return FormatInfo.Fill.GradientFill.Convergence.Top; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Top == value)
					return;
				SetPropertyValue(SetGradientFillInfoTop, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoTop(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Top = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Bottom
		float IConvergenceInfo.Bottom {
			get { return FormatInfo.Fill.GradientFill.Convergence.Bottom; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Bottom == value)
					return;
				SetPropertyValue(SetGradientFillInfoBottom, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoBottom(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		protected internal virtual void SetFillPropertyValue<U>(SetPropertyValueDelegate<U> setter, U newValue) {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification();
				info.SetActualFill(ActualFill);
				DocumentModelChangeActions changeActions = setter(info, newValue);
				ReplaceInfo(info, changeActions);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region IActualFillInfo Members
		XlPatternType IActualFillInfo.PatternType {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.ColorScaleFillModified)
						return XlPatternType.Solid;
					if (conditionalFormatAccumulator.PatternFillAssigned) {
						FillInfo fillInfo = DocumentModel.Cache.FillInfoCache[conditionalFormatAccumulator.PatternFillIndex];
						return fillInfo.PatternType;
					}
				}
				return GetActualFillPatternType();
			}
		}
		Color IActualFillInfo.ForeColor {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.ColorScaleFillModified)
						return conditionalFormatAccumulator.ColorScaleFill;
					if (conditionalFormatAccumulator.PatternFillAssigned) {
						FillInfo fillInfo = DocumentModel.Cache.FillInfoCache[conditionalFormatAccumulator.PatternFillIndex];
						return GetRgbColor(fillInfo.ForeColorIndex);
					}
				}
				return GetRgbColor(GetActualFillForeColorIndex());
			}
		}
		Color IActualFillInfo.BackColor {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.ColorScaleFillModified)
						return conditionalFormatAccumulator.ColorScaleFill;
					if (conditionalFormatAccumulator.PatternFillAssigned) {
						FillInfo fillInfo = DocumentModel.Cache.FillInfoCache[conditionalFormatAccumulator.PatternFillIndex];
						return GetRgbColor(fillInfo.BackColorIndex);
					}
				}
				return GetRgbColor(GetActualFillBackColorIndex());
			}
		}
		int IActualFillInfo.ForeColorIndex { get { return GetActualFillForeColorIndex(); } }
		int IActualFillInfo.BackColorIndex { get { return GetActualFillBackColorIndex(); } }
		bool IActualFillInfo.ApplyPatternType { get { return GetActualFillApplyPatternType(); } }
		bool IActualFillInfo.ApplyForeColor { get { return GetActualFillApplyForeColor(); } }
		bool IActualFillInfo.ApplyBackColor { get { return GetActualFillApplyBackColor(); } }
		bool IActualFillInfo.IsDifferential { get { return IsDifferential(); } }
		IActualGradientFillInfo IActualFillInfo.GradientFill { get { return ActualGradientFillInfo; } }
		IActualConvergenceInfo IActualGradientFillInfo.Convergence { get { return ActualConvergenceInfo; } }
		ModelFillType IActualFillInfo.FillType {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.ColorScaleFillModified)
						return ModelFillType.Pattern;
					if (conditionalFormatAccumulator.FillAssigned)
						return conditionalFormatAccumulator.FillType;
				}
				return GetActualFillType();
			}
		}
		ModelGradientFillType IActualGradientFillInfo.Type {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.GradientFillAssigned) {
						GradientFillInfo gradientFillInfo = DocumentModel.Cache.GradientFillInfoCache[conditionalFormatAccumulator.GradientFillIndex];
						return gradientFillInfo.Type;
					}
				}
				return GetActualFillGradientFillType();
			}
		}
		double IActualGradientFillInfo.Degree {
			get {
				if (conditionalFormatAccumulator != null) {
					conditionalFormatAccumulator.Update(this);
					if (conditionalFormatAccumulator.GradientFillAssigned) {
						GradientFillInfo gradientFillInfo = DocumentModel.Cache.GradientFillInfoCache[conditionalFormatAccumulator.GradientFillIndex];
						return gradientFillInfo.Degree;
					}
				}
				return GetActualFillGradientFillDegree();
			}
		}
		IActualGradientStopCollection IActualGradientFillInfo.GradientStops { get { return GetActualFillGradientFillGradientStops(); } }
		float IActualConvergenceInfo.Left { get { return GetActualFillGradientFillConvergenceLeft(); } }
		float IActualConvergenceInfo.Right { get { return GetActualFillGradientFillConvergenceRight(); } }
		float IActualConvergenceInfo.Top { get { return GetActualFillGradientFillConvergenceTop(); } }
		float IActualConvergenceInfo.Bottom { get { return GetActualFillGradientFillConvergenceBottom(); } }
		#endregion
		#region GetActualFillValue
		protected virtual T GetActualFillValue<T>(T cellFormatActualValue, DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualFormatValue(cellFormatActualValue, ActualApplyInfo.ApplyFill, propertyDescriptor);
		}
		protected virtual bool GetActualApplyFillValue(DifferentialFormatPropertyDescriptor propertyDescriptor) {
			return GetActualApplyFormatValue(ActualApplyInfo.ApplyFill, propertyDescriptor);
		}
		protected virtual XlPatternType GetActualFillPatternType() {
			return GetActualFillValue(InnerActualFill.PatternType, DifferentialFormatPropertyDescriptor.FillPatternType);
		}
		protected virtual int GetActualFillForeColorIndex() {
			if (conditionalFormatAccumulator != null) {
				conditionalFormatAccumulator.Update(this);
				if (conditionalFormatAccumulator.PatternFillAssigned) {
					FillInfo FillInfo = DocumentModel.Cache.FillInfoCache[conditionalFormatAccumulator.PatternFillIndex];
					return FillInfo.ForeColorIndex;
				}
			}
			return GetActualFillValue(InnerActualFill.ForeColorIndex, DifferentialFormatPropertyDescriptor.FillForeColorIndex);
		}
		protected virtual int GetActualFillBackColorIndex() {
			return GetActualFillValue(InnerActualFill.BackColorIndex, DifferentialFormatPropertyDescriptor.FillBackColorIndex);
		}
		protected virtual bool GetActualFillApplyPatternType() {
			return GetActualApplyFillValue(DifferentialFormatPropertyDescriptor.FillPatternType);
		}
		protected virtual bool GetActualFillApplyForeColor() {
			return GetActualApplyFillValue(DifferentialFormatPropertyDescriptor.FillForeColorIndex);
		}
		protected virtual bool GetActualFillApplyBackColor() {
			return GetActualApplyFillValue(DifferentialFormatPropertyDescriptor.FillBackColorIndex);
		}
		protected virtual bool IsDifferential() {
			return !ActualApplyInfo.ApplyFill && (sheet.Tables.TryGetItem(Position) != null || sheet.PivotTables.TryGetItem(Position) != null);
		}
		protected virtual ModelFillType GetActualFillType() {
			return GetActualFillValue(InnerActualFill.FillType, DifferentialFormatPropertyDescriptor.FillType);
		}
		protected virtual ModelGradientFillType GetActualFillGradientFillType() {
			return GetActualFillValue(InnerActualGradientFillInfo.Type, DifferentialFormatPropertyDescriptor.FillGradientFillType);
		}
		protected virtual double GetActualFillGradientFillDegree() {
			return GetActualFillValue(InnerActualGradientFillInfo.Degree, DifferentialFormatPropertyDescriptor.FillGradientFillDegree);
		}
		protected virtual IActualGradientStopCollection GetActualFillGradientFillGradientStops() {
			return GetActualFillValue(InnerActualGradientFillInfo.GradientStops, DifferentialFormatPropertyDescriptor.FillGradientFillGradientStops);
		}
		protected virtual float GetActualFillGradientFillConvergenceLeft() {
			return GetActualFillValue(InnerActualConvergenceInfo.Left, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceLeft);
		}
		protected virtual float GetActualFillGradientFillConvergenceRight() {
			return GetActualFillValue(InnerActualConvergenceInfo.Right, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceRight);
		}
		protected virtual float GetActualFillGradientFillConvergenceTop() {
			return GetActualFillValue(InnerActualConvergenceInfo.Top, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceTop);
		}
		protected virtual float GetActualFillGradientFillConvergenceBottom() {
			return GetActualFillValue(InnerActualConvergenceInfo.Bottom, DifferentialFormatPropertyDescriptor.FillGradientFillCovergenceBottom);
		}
		#endregion
		public static Color GetBackgroundColor(IActualFillInfo fill) {
			if (fill.IsDifferential)
				return GetBackgroundColorFromDifferrentialFormat(fill);
			return GetBackgroundColorFromCellFormat(fill);
		}
		static Color GetBackgroundColorFromDifferrentialFormat(IActualFillInfo fill) {
			if (fill.ApplyBackColor)
				return fill.BackColor;
			else
				return DXColor.Empty;
		}
		static Color GetBackgroundColorFromCellFormat(IActualFillInfo fill) {
			XlPatternType patternType = fill.PatternType;
			if (patternType == XlPatternType.None)
				return DXColor.Empty;
			if (patternType == XlPatternType.Solid)
				return fill.ForeColor; 
			else
				return fill.BackColor;
		}
		public static Color GetForegroundColor(IActualFillInfo fill) {
			if (fill.IsDifferential)
				return GetForegroundColorFromDifferrentialFormat(fill);
			return GetForegroundColorFromCellFormat(fill);
		}
		static Color GetForegroundColorFromDifferrentialFormat(IActualFillInfo fill) {
			if (fill.ApplyForeColor)
				return fill.ForeColor;
			else
				return DXColor.Empty;
		}
		static Color GetForegroundColorFromCellFormat(IActualFillInfo fill) {
			XlPatternType patternType = fill.PatternType;
			if (patternType == XlPatternType.None)
				return DXColor.Empty;
			if (patternType == XlPatternType.Solid)
				return fill.BackColor; 
			else
				return fill.ForeColor;
		}
	}
	#endregion
}
