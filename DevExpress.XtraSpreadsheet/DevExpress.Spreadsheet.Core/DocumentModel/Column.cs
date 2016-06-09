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
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IColumnRange : IActualFormat {
		int StartIndex { get; }
		int EndIndex { get; }
		int FormatIndex { get; }
		Worksheet Sheet { get; }
		bool IsCustomWidth { get; }
		bool BestFit { get; }
		bool IsHidden { get; }
		float Width { get; }
		int OutlineLevel { get; }
		int StyleIndex { get; }
		bool HasVisibleBorder { get; }
		bool HasVisibleFill { get; }
		bool IsVisible { get; }
		bool IsCollapsed { get; }
		CellIntervalRange GetCellIntervalRange();
	}
	#region Column
	public class Column : MultiIndexObject<Column, DocumentModelChangeActions>, IColumnRange, ICellFormat, IFormatBaseBatchUpdateable {
		readonly static ColumnCellFormatIndexAccessor cellFormatIndexAccessor = new ColumnCellFormatIndexAccessor();
		readonly static ColumnWidthIndexAccessor widthIndexAccessor = new ColumnWidthIndexAccessor();
		readonly static ColumnInfoIndexAccessor infoIndexAccessor = new ColumnInfoIndexAccessor();
		readonly static IIndexAccessorBase<Column, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<Column, DocumentModelChangeActions>[] {
			cellFormatIndexAccessor,
			widthIndexAccessor,
			infoIndexAccessor,
		};
		public static ColumnCellFormatIndexAccessor CellFormatIndexAccessor { get { return cellFormatIndexAccessor; } }
		public static ColumnWidthIndexAccessor WidthIndexAccessor { get { return widthIndexAccessor; } }
		public static ColumnInfoIndexAccessor InfoIndexAccessor { get { return infoIndexAccessor; } }
		#region Fields
		int startIndex;
		int endIndex;
		int formatIndex;
		int widthIndex;
		int infoIndex;
		Worksheet sheet;
		#endregion
		public Column(Worksheet sheet, int startIndex, int endIndex) {
			Guard.ArgumentNotNull(sheet, "sheet");
			System.Diagnostics.Debug.Assert(startIndex <= endIndex);
			this.startIndex = startIndex;
			this.endIndex = endIndex;
			this.sheet = sheet;
			this.formatIndex = sheet.Workbook.StyleSheet.DefaultCellFormatIndex;
		}
		public void SetCustomWidth(float valueInCharacters) {
			SetCustomWidthCore(valueInCharacters, true);
		}
		public void SetCustomWidthCore(float valueInCharacters, bool shouldRecalculateAnchors) {
			if (valueInCharacters == 0) {
				if (!IsHidden)
					Sheet.HideColumns(StartIndex, EndIndex);
				return;
			}
			DocumentModel.BeginUpdate();
			this.BeginUpdate();
			try {
				if (IsHidden)
					Sheet.UnhideColumns(StartIndex, EndIndex);
				this.BestFit = false;
				this.IsCustomWidth = true;
				this.Width = valueInCharacters;
				if(shouldRecalculateAnchors) {
					InvalidateAnchorDatasByColumnHistoryItem historyItem = new InvalidateAnchorDatasByColumnHistoryItem(Sheet, StartIndex, EndIndex);
					DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
			finally {
				this.EndUpdate();
				DocumentModel.EndUpdate();
			}
		}
		#region Properties
		public int Index { get { return startIndex; } }
		public int StartIndex { get { return startIndex; } }
		internal void AssignStartIndex(int value) {
			DocumentHistory history = DocumentModel.History;
			SpreadsheetColumnStartIndexChangedHistoryItem historyItem = new SpreadsheetColumnStartIndexChangedHistoryItem(sheet, this, startIndex, value);
			history.Add(historyItem);
			historyItem.Execute();
		}
		internal void AssignStartIndexCore(int value) {
			this.startIndex = value;
		}
		public int EndIndex { get { return endIndex; } }
		int IColumnRange.StyleIndex { get { return FormatInfo.StyleIndex; } }
		public Worksheet Sheet { get { return this.sheet; } }
		public int FormatIndex { get { return formatIndex; } }
		internal int WidthIndex { get { return widthIndex; } }
		internal int InfoIndex { get { return infoIndex; } }
		internal new ColumnBatchUpdateHelper BatchUpdateHelper { get { return (ColumnBatchUpdateHelper)base.BatchUpdateHelper; } }
		public new DocumentModel DocumentModel { get { return sheet.Workbook; } }
		internal CellFormat FormatInfo { get { return BatchUpdateHelper != null ? (CellFormat)BatchUpdateHelper.CellFormat : FormatInfoCore; } }
		CellFormat FormatInfoCore { get { return (CellFormat)DocumentModel.Cache.CellFormatCache[formatIndex]; } }
		internal ColumnWidthInfo WidthInfo { get { return BatchUpdateHelper != null ? BatchUpdateHelper.WidthInfo : WidthInfoCore; } }
		ColumnWidthInfo WidthInfoCore { get { return widthIndexAccessor.GetInfo(this); } }
		internal ColumnInfo Info { get { return BatchUpdateHelper != null ? BatchUpdateHelper.Info : InfoCore; } }
		ColumnInfo InfoCore { get { return infoIndexAccessor.GetInfo(this); } }
		#region Width
		public float Width {
			get { return WidthInfo.FloatValue; }
			set {
				if (WidthInfo.FloatValue == value)
					return;
				if (value < 0 || value > 255)
					throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorIncorrectColumnWidth));
				SetPropertyValueForStruct(widthIndexAccessor, SetWidthCore, value);
			}
		}
		DocumentModelChangeActions SetWidthCore(ref ColumnWidthInfo info, float value) {
			info.FloatValue = value;
			sheet.WebRanges.ChangeRange(GetCellIntervalRange());
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region OutlineLevel
		public int OutlineLevel {
			get { return Info.OutlineLevel; }
			set {
				if (Info.OutlineLevel == value)
					return;
				SetPropertyValueForStruct(infoIndexAccessor, SetOutlineLevelCore, value);
			}
		}
		DocumentModelChangeActions SetOutlineLevelCore(ref ColumnInfo info, int value) {
			info.OutlineLevel = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region BestFit
		public bool BestFit {
			get { return Info.BestFit; }
			set {
				if (Info.BestFit == value)
					return;
				SetPropertyValueForStruct(infoIndexAccessor, SetBestFitCore, value);
			}
		}
		DocumentModelChangeActions SetBestFitCore(ref ColumnInfo info, bool value) {
			info.BestFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IsHidden
		public bool IsHidden {
			get { return Info.IsHidden; }
			set {
				if (Info.IsHidden == value)
					return;
				if (sheet.NeedColumnUnhideNotificated && Info.OutlineLevel > 0)
					sheet.UpdateColumnOutlineGroup(this.Index, this.Index, Info.OutlineLevel, value);
				BeginUpdate();
				SetPropertyValueForStruct(infoIndexAccessor, SetIsHiddenCore, value);
				InvalidateAnchorDatasByColumnHistoryItem historyItem = new InvalidateAnchorDatasByColumnHistoryItem(Sheet, StartIndex, EndIndex);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
				EndUpdate();
			}
		}
		DocumentModelChangeActions SetIsHiddenCore(ref ColumnInfo info, bool value) {
			info.IsHidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IsCollapsed
		public bool IsCollapsed {
			get { return Info.IsCollapsed; }
			set {
				if (Info.IsCollapsed == value)
					return;
				SetPropertyValueForStruct(infoIndexAccessor, SetIsCollapsedCore, value);
			}
		}
		DocumentModelChangeActions SetIsCollapsedCore(ref ColumnInfo info, bool value) {
			info.IsCollapsed = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IsCustomWidth
		public bool IsCustomWidth {
			get { return Info.IsCustomWidth; }
			set {
				if (Info.IsCustomWidth == value)
					return;
				SetPropertyValueForStruct(infoIndexAccessor, SetIsCustomWidthCore, value);
			}
		}
		DocumentModelChangeActions SetIsCustomWidthCore(ref ColumnInfo info, bool value) {
			info.IsCustomWidth = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		public bool IsVisible { get { return !(IsHidden || Width == 0 && IsCustomWidth); } }
		public bool HasVisibleFill {
			get {
				if (!IsVisible)
					return false;
				if (ActualFill.FillType == ModelFillType.Gradient)
					return true;
				return (ActualFill.PatternType == XlPatternType.Solid && !DXColor.IsTransparentOrEmpty(Cell.GetBackgroundColor(ActualFill))) || (ActualFill.PatternType != XlPatternType.None && ActualFill.PatternType != XlPatternType.Solid);
			}
		}
		public bool HasVisibleBorder {
			get {
				if (!IsVisible)
					return false;
				return !Cell.IsBorderEmpty(ActualBorder);
			}
		}
		#endregion
		#region MultiIndexObject
		protected override IDocumentModel GetDocumentModel() {
			return sheet.Workbook;
		}
		public override Column GetOwner() {
			return this;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override void ApplyChanges(DocumentModelChangeActions actions) {
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new ColumnBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new ColumnBatchInitHelper(this);
		}
		protected override IIndexAccessorBase<Column, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		internal void AssignCellFormatIndex(int value) {
			this.formatIndex = value;
		}
		internal void AssignWidthIndex(int value) {
			this.widthIndex = value;
		}
		internal void AssignInfoIndex(int value) {
			this.infoIndex = value;
		}
		#endregion
		#region ICellFormat Members
		public IRunFontInfo Font { get { return this; } }
		public IActualRunFontInfo ActualFont { get { return this; } }
		public IFillInfo Fill { get { return this; } }
		public IActualFillInfo ActualFill { get { return this; } }
		public ICellAlignmentInfo Alignment { get { return this; } }
		public IActualCellAlignmentInfo ActualAlignment { get { return this; } }
		public IBorderInfo Border { get { return this; } }
		public IActualBorderInfo ActualBorder { get { return this; } }
		public ICellProtectionInfo Protection { get { return this; } }
		public IActualCellProtectionInfo ActualProtection { get { return this; } }
		#region IRunFontInfo Members
		#region IRunFontInfo.Name
		string IRunFontInfo.Name {
			get { return FormatInfo.Font.Name; }
			set {
				if (FormatInfo.Font.Name == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontName, value);
			}
		}
		DocumentModelChangeActions SetFontName(FormatBase info, string value) {
			info.Font.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Color
		Color IRunFontInfo.Color {
			get { return FormatInfo.Font.Color; }
			set {
				if (FormatInfo.Font.Color == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontColor, value);
			}
		}
		DocumentModelChangeActions SetFontColor(FormatBase info, Color value) {
			info.Font.Color = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Bold
		bool IRunFontInfo.Bold {
			get { return FormatInfo.Font.Bold; }
			set {
				if (FormatInfo.Font.Bold == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontBold, value);
			}
		}
		DocumentModelChangeActions SetFontBold(FormatBase info, bool value) {
			info.Font.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Condense
		bool IRunFontInfo.Condense {
			get { return FormatInfo.Font.Condense; }
			set {
				if (FormatInfo.Font.Condense == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontCondense, value);
			}
		}
		DocumentModelChangeActions SetFontCondense(FormatBase info, bool value) {
			info.Font.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Extend
		bool IRunFontInfo.Extend {
			get { return FormatInfo.Font.Extend; }
			set {
				if (FormatInfo.Font.Extend == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontExtend, value);
			}
		}
		DocumentModelChangeActions SetFontExtend(FormatBase info, bool value) {
			info.Font.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Italic
		bool IRunFontInfo.Italic {
			get { return FormatInfo.Font.Italic; }
			set {
				if (FormatInfo.Font.Italic == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontItalic, value);
			}
		}
		DocumentModelChangeActions SetFontItalic(FormatBase info, bool value) {
			info.Font.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Outline
		bool IRunFontInfo.Outline {
			get { return FormatInfo.Font.Outline; }
			set {
				if (FormatInfo.Font.Outline == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontOutline, value);
			}
		}
		DocumentModelChangeActions SetFontOutline(FormatBase info, bool value) {
			info.Font.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Shadow
		bool IRunFontInfo.Shadow {
			get { return FormatInfo.Font.Shadow; }
			set {
				if (FormatInfo.Font.Shadow == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontShadow, value);
			}
		}
		DocumentModelChangeActions SetFontShadow(FormatBase info, bool value) {
			info.Font.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.StrikeThrough
		bool IRunFontInfo.StrikeThrough {
			get { return FormatInfo.Font.StrikeThrough; }
			set {
				if (FormatInfo.Font.StrikeThrough == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontStrikeThrough, value);
			}
		}
		DocumentModelChangeActions SetFontStrikeThrough(FormatBase info, bool value) {
			info.Font.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Charset
		int IRunFontInfo.Charset {
			get { return FormatInfo.Font.Charset; }
			set {
				if (FormatInfo.Font.Charset == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontCharset, value);
			}
		}
		DocumentModelChangeActions SetFontCharset(FormatBase info, int value) {
			info.Font.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.FontFamily
		int IRunFontInfo.FontFamily {
			get { return FormatInfo.Font.FontFamily; }
			set {
				if (FormatInfo.Font.FontFamily == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontFamily, value);
			}
		}
		DocumentModelChangeActions SetFontFamily(FormatBase info, int value) {
			info.Font.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Size
		double IRunFontInfo.Size {
			get { return FormatInfo.Font.Size; }
			set {
				if (FormatInfo.Font.Size == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontSize, value);
			}
		}
		DocumentModelChangeActions SetFontSize(FormatBase info, double value) {
			info.Font.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.SchemeStyle
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return FormatInfo.Font.SchemeStyle; }
			set {
				if (FormatInfo.Font.SchemeStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontSchemeStyle, value);
			}
		}
		DocumentModelChangeActions SetFontSchemeStyle(FormatBase info, XlFontSchemeStyles value) {
			info.Font.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Script
		XlScriptType IRunFontInfo.Script {
			get { return FormatInfo.Font.Script; }
			set {
				if (FormatInfo.Font.Script == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontScript, value);
			}
		}
		DocumentModelChangeActions SetFontScript(FormatBase info, XlScriptType value) {
			info.Font.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Underline
		XlUnderlineType IRunFontInfo.Underline {
			get { return FormatInfo.Font.Underline; }
			set {
				if (FormatInfo.Font.Underline == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFontUnderline, value);
			}
		}
		DocumentModelChangeActions SetFontUnderline(FormatBase info, XlUnderlineType value) {
			info.Font.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IActualRunFontInfo Members
		string IActualRunFontInfo.Name { get { return FormatInfo.ActualFont.Name; } }
		Color IActualRunFontInfo.Color { get { return FormatInfo.ActualFont.Color; } }
		bool IActualRunFontInfo.Bold { get { return FormatInfo.ActualFont.Bold; } }
		bool IActualRunFontInfo.Condense { get { return FormatInfo.ActualFont.Condense; } }
		bool IActualRunFontInfo.Extend { get { return FormatInfo.ActualFont.Extend; } }
		bool IActualRunFontInfo.Italic { get { return FormatInfo.ActualFont.Italic; } }
		bool IActualRunFontInfo.Outline { get { return FormatInfo.ActualFont.Outline; } }
		bool IActualRunFontInfo.Shadow { get { return FormatInfo.ActualFont.Shadow; } }
		bool IActualRunFontInfo.StrikeThrough { get { return FormatInfo.ActualFont.StrikeThrough; } }
		int IActualRunFontInfo.Charset { get { return FormatInfo.ActualFont.Charset; } }
		int IActualRunFontInfo.FontFamily { get { return FormatInfo.ActualFont.FontFamily; } }
		double IActualRunFontInfo.Size { get { return FormatInfo.ActualFont.Size; } }
		XlFontSchemeStyles IActualRunFontInfo.SchemeStyle { get { return FormatInfo.ActualFont.SchemeStyle; } }
		XlScriptType IActualRunFontInfo.Script { get { return FormatInfo.ActualFont.Script; } }
		XlUnderlineType IActualRunFontInfo.Underline { get { return FormatInfo.ActualFont.Underline; } }
		int IActualRunFontInfo.ColorIndex { get { return FormatInfo.ActualFont.ColorIndex; } }
		FontInfo IActualRunFontInfo.GetFontInfo() {
			return FormatInfo.ActualFont.GetFontInfo();
		}
		#endregion
		#region IFillInfo Members
		#region IFillInfo.Clear
		void IFillInfo.Clear() {
			if (FormatInfo.ApplyFill)
				ClearFillCore();
		}
		void ClearFillCore() {
			DocumentModel.BeginUpdate();
			try {
				FormatBase info = GetInfoForModification(cellFormatIndexAccessor);
				info.Fill.Clear();
				ReplaceInfo(cellFormatIndexAccessor, info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region IFillInfo.PatternType
		XlPatternType IFillInfo.PatternType {
			get { return FormatInfo.Fill.PatternType; }
			set {
				if (FormatInfo.Fill.PatternType == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFillPatternType, value);
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
				if (FormatInfo.Fill.ForeColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFillForeColor, value);
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
				if (FormatInfo.Fill.BackColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetFillBackColor, value);
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
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoType, value);
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
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoDegree, value);
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
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoLeft, value);
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
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoRight, value);
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
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoTop, value);
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
				SetPropertyValue(cellFormatIndexAccessor, SetGradientFillInfoBottom, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoBottom(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		#region IFillInfo.FillType
		ModelFillType IFillInfo.FillType {
			get { return FormatInfo.Fill.FillType; }
			set {
				if (FormatInfo.Fill.FillType == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetModelFillType, value);
			}
		}
		DocumentModelChangeActions SetModelFillType(FormatBase info, ModelFillType value) {
			info.Fill.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IActualFillInfo Members
		XlPatternType IActualFillInfo.PatternType { get { return FormatInfo.ActualFill.PatternType; } }
		Color IActualFillInfo.ForeColor { get { return FormatInfo.ActualFill.ForeColor; } }
		Color IActualFillInfo.BackColor { get { return FormatInfo.ActualFill.BackColor; } }
		int IActualFillInfo.ForeColorIndex { get { return FormatInfo.ActualFill.ForeColorIndex; } }
		int IActualFillInfo.BackColorIndex { get { return FormatInfo.ActualFill.BackColorIndex; } }
		bool IActualFillInfo.ApplyPatternType { get { return true; } }
		bool IActualFillInfo.ApplyForeColor { get { return true; } }
		bool IActualFillInfo.ApplyBackColor { get { return true; } }
		bool IActualFillInfo.IsDifferential { get { return false; } }
		IActualGradientFillInfo IActualFillInfo.GradientFill { get { return this; } }
		IActualConvergenceInfo IActualGradientFillInfo.Convergence { get { return this; } }
		ModelFillType IActualFillInfo.FillType { get { return FormatInfo.ActualFill.FillType; } }
		ModelGradientFillType IActualGradientFillInfo.Type { get { return FormatInfo.ActualFill.GradientFill.Type; } }
		double IActualGradientFillInfo.Degree { get { return FormatInfo.ActualFill.GradientFill.Degree; } }
		IActualGradientStopCollection IActualGradientFillInfo.GradientStops { get { return FormatInfo.ActualFill.GradientFill.GradientStops; } }
		float IActualConvergenceInfo.Left { get { return FormatInfo.ActualFill.GradientFill.Convergence.Left; } }
		float IActualConvergenceInfo.Right { get { return FormatInfo.ActualFill.GradientFill.Convergence.Right; } }
		float IActualConvergenceInfo.Top { get { return FormatInfo.ActualFill.GradientFill.Convergence.Top; } }
		float IActualConvergenceInfo.Bottom { get { return FormatInfo.ActualFill.GradientFill.Convergence.Bottom; } }
		#endregion
		#region ICellAlignmentInfo Members
		#region ICellAlignmentInfo.WrapText
		bool ICellAlignmentInfo.WrapText {
			get { return FormatInfo.Alignment.WrapText; }
			set {
				if (FormatInfo.Alignment.WrapText == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentWrapText, value);
			}
		}
		DocumentModelChangeActions SetAlignmentWrapText(FormatBase info, bool value) {
			info.Alignment.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.JustifyLastLine
		bool ICellAlignmentInfo.JustifyLastLine {
			get { return FormatInfo.Alignment.JustifyLastLine; }
			set {
				if (FormatInfo.Alignment.JustifyLastLine == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentJustifyLastLine, value);
			}
		}
		DocumentModelChangeActions SetAlignmentJustifyLastLine(FormatBase info, bool value) {
			info.Alignment.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.ShrinkToFit
		bool ICellAlignmentInfo.ShrinkToFit {
			get { return FormatInfo.Alignment.ShrinkToFit; }
			set {
				if (FormatInfo.Alignment.ShrinkToFit == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentShrinkToFit, value);
			}
		}
		DocumentModelChangeActions SetAlignmentShrinkToFit(FormatBase info, bool value) {
			info.Alignment.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.TextRotation
		int ICellAlignmentInfo.TextRotation {
			get { return FormatInfo.Alignment.TextRotation; }
			set {
				if (FormatInfo.Alignment.TextRotation == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentTextRotation, value);
			}
		}
		DocumentModelChangeActions SetAlignmentTextRotation(FormatBase info, int value) {
			info.Alignment.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Indent
		byte ICellAlignmentInfo.Indent {
			get { return FormatInfo.Alignment.Indent; }
			set {
				if (FormatInfo.Alignment.Indent == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentIndent(FormatBase info, byte value) {
			info.Alignment.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.RelativeIndent
		int ICellAlignmentInfo.RelativeIndent {
			get { return FormatInfo.Alignment.RelativeIndent; }
			set {
				if (FormatInfo.Alignment.RelativeIndent == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentRelativeIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentRelativeIndent(FormatBase info, int value) {
			info.Alignment.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Horizontal
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal {
			get { return FormatInfo.Alignment.Horizontal; }
			set {
				if (FormatInfo.Alignment.Horizontal == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentHorizontal, value);
			}
		}
		DocumentModelChangeActions SetAlignmentHorizontal(FormatBase info, XlHorizontalAlignment value) {
			info.Alignment.Horizontal = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Vertical
		XlVerticalAlignment ICellAlignmentInfo.Vertical {
			get { return FormatInfo.Alignment.Vertical; }
			set {
				if (FormatInfo.Alignment.Vertical == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentVertical, value);
			}
		}
		DocumentModelChangeActions SetAlignmentVertical(FormatBase info, XlVerticalAlignment value) {
			info.Alignment.Vertical = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.XlReadingOrder
		XlReadingOrder ICellAlignmentInfo.ReadingOrder {
			get { return FormatInfo.Alignment.ReadingOrder; }
			set {
				if (FormatInfo.Alignment.ReadingOrder == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetAlignmentReadingOrder, value);
			}
		}
		DocumentModelChangeActions SetAlignmentReadingOrder(FormatBase info, XlReadingOrder value) {
			info.Alignment.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IActualCellAlignmentInfo Members
		bool IActualCellAlignmentInfo.WrapText { get { return FormatInfo.ActualAlignment.WrapText; } }
		bool IActualCellAlignmentInfo.JustifyLastLine { get { return FormatInfo.ActualAlignment.JustifyLastLine; } }
		bool IActualCellAlignmentInfo.ShrinkToFit { get { return FormatInfo.ActualAlignment.ShrinkToFit; } }
		int IActualCellAlignmentInfo.TextRotation { get { return FormatInfo.ActualAlignment.TextRotation; } }
		byte IActualCellAlignmentInfo.Indent { get { return FormatInfo.ActualAlignment.Indent; } }
		int IActualCellAlignmentInfo.RelativeIndent { get { return FormatInfo.ActualAlignment.RelativeIndent; } }
		XlHorizontalAlignment IActualCellAlignmentInfo.Horizontal { get { return FormatInfo.ActualAlignment.Horizontal; } }
		XlVerticalAlignment IActualCellAlignmentInfo.Vertical { get { return FormatInfo.ActualAlignment.Vertical; } }
		XlReadingOrder IActualCellAlignmentInfo.ReadingOrder { get { return FormatInfo.ActualAlignment.ReadingOrder; } }
		#endregion
		#region IBorderInfo Members
		#region IBorderInfo.LeftLineStyle
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get { return FormatInfo.Border.LeftLineStyle; }
			set {
				if (FormatInfo.Border.LeftLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderLeftLineStyle, value);
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
				if (FormatInfo.Border.LeftColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderLeftColor, value);
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
				if (FormatInfo.Border.LeftColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderLeftColorIndex, value);
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
				if (FormatInfo.Border.RightLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderRightLineStyle, value);
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
				if (FormatInfo.Border.RightColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderRightColor, value);
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
				if (FormatInfo.Border.RightColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderRightColorIndex, value);
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
				if (FormatInfo.Border.TopLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderTopLineStyle, value);
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
				if (FormatInfo.Border.TopColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderTopColor, value);
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
				if (FormatInfo.Border.TopColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderTopColorIndex, value);
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
				if (FormatInfo.Border.BottomLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderBottomLineStyle, value);
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
				if (FormatInfo.Border.BottomColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderBottomColor, value);
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
				if (FormatInfo.Border.BottomColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderBottomColorIndex, value);
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
				if (FormatInfo.Border.HorizontalLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderHorizontalLineStyle, value);
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
				if (FormatInfo.Border.HorizontalColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderHorizontalColor, value);
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
				if (FormatInfo.Border.HorizontalColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderHorizontalColorIndex, value);
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
				if (FormatInfo.Border.VerticalLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderVerticalLineStyle, value);
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
				if (FormatInfo.Border.VerticalColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderVerticalColor, value);
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
				if (FormatInfo.Border.VerticalColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderVerticalColorIndex, value);
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
				if (FormatInfo.Border.DiagonalUpLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderDiagonalUpLineStyle, value);
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
				if (FormatInfo.Border.DiagonalDownLineStyle == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderDiagonalDownLineStyle, value);
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
				if (FormatInfo.Border.DiagonalColor == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderDiagonalColor, value);
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
				if (FormatInfo.Border.DiagonalColorIndex == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderDiagonalColorIndex, value);
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
				if (FormatInfo.Border.Outline == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetBorderOutline, value);
			}
		}
		DocumentModelChangeActions SetBorderOutline(FormatBase info, bool value) {
			info.Border.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IActualBorderInfo Members
		XlBorderLineStyle IActualBorderInfo.LeftLineStyle { get { return FormatInfo.ActualBorder.LeftLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.RightLineStyle { get { return FormatInfo.ActualBorder.RightLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.TopLineStyle { get { return FormatInfo.ActualBorder.TopLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.BottomLineStyle { get { return FormatInfo.ActualBorder.BottomLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.HorizontalLineStyle { get { return FormatInfo.ActualBorder.HorizontalLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.VerticalLineStyle { get { return FormatInfo.ActualBorder.VerticalLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalUpLineStyle { get { return FormatInfo.ActualBorder.DiagonalUpLineStyle; } }
		XlBorderLineStyle IActualBorderInfo.DiagonalDownLineStyle { get { return FormatInfo.ActualBorder.DiagonalDownLineStyle; } }
		Color IActualBorderInfo.LeftColor { get { return FormatInfo.ActualBorder.LeftColor; } }
		Color IActualBorderInfo.RightColor { get { return FormatInfo.ActualBorder.RightColor; } }
		Color IActualBorderInfo.TopColor { get { return FormatInfo.ActualBorder.TopColor; } }
		Color IActualBorderInfo.BottomColor { get { return FormatInfo.ActualBorder.BottomColor; } }
		Color IActualBorderInfo.HorizontalColor { get { return FormatInfo.ActualBorder.HorizontalColor; } }
		Color IActualBorderInfo.VerticalColor { get { return FormatInfo.ActualBorder.VerticalColor; } }
		Color IActualBorderInfo.DiagonalColor { get { return FormatInfo.ActualBorder.DiagonalColor; } }
		int IActualBorderInfo.LeftColorIndex { get { return FormatInfo.ActualBorder.LeftColorIndex; } }
		int IActualBorderInfo.RightColorIndex { get { return FormatInfo.ActualBorder.RightColorIndex; } }
		int IActualBorderInfo.TopColorIndex { get { return FormatInfo.ActualBorder.TopColorIndex; } }
		int IActualBorderInfo.BottomColorIndex { get { return FormatInfo.ActualBorder.BottomColorIndex; } }
		int IActualBorderInfo.HorizontalColorIndex { get { return FormatInfo.ActualBorder.HorizontalColorIndex; } }
		int IActualBorderInfo.VerticalColorIndex { get { return FormatInfo.ActualBorder.VerticalColorIndex; } }
		int IActualBorderInfo.DiagonalColorIndex { get { return FormatInfo.ActualBorder.DiagonalColorIndex; } }
		bool IActualBorderInfo.Outline { get { return FormatInfo.ActualBorder.Outline; } }
		#endregion
		#region ICellProtectionInfo Members
		#region ICellProtectionInfo.Locked
		bool ICellProtectionInfo.Locked {
			get { return FormatInfo.Protection.Locked; }
			set {
				if (FormatInfo.Protection.Locked == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetProtectionLocked, value);
			}
		}
		DocumentModelChangeActions SetProtectionLocked(FormatBase info, bool value) {
			info.Protection.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellProtectionInfo.Hidden
		bool ICellProtectionInfo.Hidden {
			get { return FormatInfo.Protection.Hidden; }
			set {
				if (FormatInfo.Protection.Hidden == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetProtectionHidden, value);
			}
		}
		DocumentModelChangeActions SetProtectionHidden(FormatBase info, bool value) {
			info.Protection.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IActualCellProtectionInfo Members
		bool IActualCellProtectionInfo.Locked { get { return FormatInfo.ActualProtection.Locked; } }
		bool IActualCellProtectionInfo.Hidden { get { return FormatInfo.ActualProtection.Hidden; } }
		#endregion
		#region FormatString members
		public string FormatString {
			get { return FormatInfo.FormatString; }
			set {
				if (FormatInfo.FormatString == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetNumberFormat, value);
			}
		}
		DocumentModelChangeActions SetNumberFormat(FormatBase info, string value) {
			info.FormatString = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ActualFormatString members
		public string ActualFormatString { get { return FormatInfo.ActualFormatString; } }
		#endregion
		#region ActualFormatIndex members
		public int ActualFormatIndex { get { return FormatInfo.ActualFormatIndex; } }
		#endregion
		#region IActualApplyInfo Members
		public IActualApplyInfo ActualApplyInfo { get { return FormatInfo.ActualApplyInfo; } }  
		#endregion
		#endregion
		#region CellStyle Members
		public CellStyleBase Style { get { return FormatInfo.Style; } set { SetPropertyValue(cellFormatIndexAccessor, SetStyleIndex, value); } }
		DocumentModelChangeActions SetStyleIndex(FormatBase info, CellStyleBase value) {
			((CellFormat)info).ApplyStyle(value, Sheet.IsProtected);
			return DocumentModelChangeActions.None;
		}
		#endregion
		protected internal void RemoveStyle() {
			bool applyNumberFormat = ApplyNumberFormat;
			bool applyFont = ApplyFont;
			bool applyFill = ApplyFill;
			bool applyBorder = ApplyBorder;
			bool applyAlignment = ApplyAlignment;
			bool applyProtection = ApplyProtection;
			Style = DocumentModel.StyleSheet.CellStyles[0];
			ApplyNumberFormat = applyNumberFormat;
			ApplyFont = applyFont;
			ApplyFill = applyFill;
			ApplyBorder = applyBorder;
			ApplyAlignment = applyAlignment;
			ApplyProtection = applyProtection;
		}
		public override bool Equals(object obj) {
			Column other = obj as Column;
			if (other == null)
				return false;
			return IsEqual(other);
		}
		bool IsEqual(Column other) {
			return
				this.StartIndex == other.StartIndex &&
				this.EndIndex == other.EndIndex &&
				this.Sheet == other.Sheet;
		}
		public bool PropertiesEquals(Column column) {
			return column.FormatIndex == FormatIndex &&
					column.InfoIndex == InfoIndex &&
					column.WidthIndex == WidthIndex;
		}
		public override string ToString() {
			return String.Concat("Column si:", StartIndex.ToString(), ", ei:", EndIndex.ToString(), ", wi:", WidthIndex.ToString(), ",fi: ", FormatIndex.ToString());
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ StartIndex ^ (EndIndex << 16) ^ Sheet.GetHashCode();
		}
		internal void OffsetColumnIndex(int from, int offset) {
			if (startIndex > from)
				startIndex += offset;
			if (endIndex >= from)
				endIndex += offset;
		}
		internal int OffsetColumnIndexWithOverflow(int from, int offset) {
			if (startIndex > from) {
				if (startIndex + offset >= IndicesChecker.MaxColumnCount)
					return -1;
				startIndex += offset;
			}
			if (endIndex >= from) {
				if (endIndex + offset >= IndicesChecker.MaxColumnCount) {
					int oldEndIndex = endIndex;
					endIndex = IndicesChecker.MaxColumnCount - 1;
					return oldEndIndex;
				}
				endIndex += offset;
			}
			return 0;
		}
		internal void RestoreEndIndex(int savedEndIndex) {
			if (savedEndIndex > 0)
				endIndex = savedEndIndex;
		}
		CellIntervalRange IColumnRange.GetCellIntervalRange() {
			return this.GetCellIntervalRange();
		}
		protected internal CellIntervalRange GetCellIntervalRange() {
			return CellIntervalRange.CreateColumnInterval(sheet, StartIndex, PositionType.Absolute, EndIndex, PositionType.Absolute);
		}
		#region Applies.ApplyNumberFormat
		public bool ApplyNumberFormat {
			get { return FormatInfo.ApplyNumberFormat; }
			set {
				if (FormatInfo.ApplyNumberFormat == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyNumberFormatCore, value);
			}
		}
		DocumentModelChangeActions SetApplyNumberFormatCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyNumberFormat = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyFont
		public bool ApplyFont {
			get { return FormatInfo.ApplyFont; }
			set {
				if (FormatInfo.ApplyFont == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyFontCore, value);
			}
		}
		DocumentModelChangeActions SetApplyFontCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyFont = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyFill
		public bool ApplyFill {
			get { return FormatInfo.ApplyFill; }
			set {
				if (FormatInfo.ApplyFill == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyFillCore, value);
			}
		}
		DocumentModelChangeActions SetApplyFillCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyFill = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyBorder
		public bool ApplyBorder {
			get { return FormatInfo.ApplyBorder; }
			set {
				if (FormatInfo.ApplyBorder == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyBorderCore, value);
			}
		}
		DocumentModelChangeActions SetApplyBorderCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyBorder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyAlignment
		public bool ApplyAlignment {
			get { return FormatInfo.ApplyAlignment; }
			set {
				if (FormatInfo.ApplyAlignment == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyAlignmentCore, value);
			}
		}
		DocumentModelChangeActions SetApplyAlignmentCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyAlignment = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyProtection
		public bool ApplyProtection {
			get { return FormatInfo.ApplyProtection; }
			set {
				if (FormatInfo.ApplyProtection == value)
					return;
				SetPropertyValue(cellFormatIndexAccessor, SetApplyProtectionCore, value);
			}
		}
		DocumentModelChangeActions SetApplyProtectionCore(FormatBase info, bool value) {
			((CellFormatBase)info).ApplyProtection = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
	}
	#endregion
	#region ColumnCellFormatIndexAccessor
	public class ColumnCellFormatIndexAccessor : IIndexAccessor<Column, FormatBase, DocumentModelChangeActions> {
		#region IIndexAccessor Members
		public int GetIndex(Column owner) {
			return owner.FormatIndex;
		}
		public int GetDeferredInfoIndex(Column owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Column owner, int value) {
			owner.AssignCellFormatIndex(value);
		}
		public int GetInfoIndex(Column owner, FormatBase value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public FormatBase GetInfo(Column owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(Column owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<FormatBase> GetInfoCache(Column owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Column owner) {
			return new ColumnCellFormatIndexChangeHistoryItem(owner);
		}
		public FormatBase GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((ColumnBatchUpdateHelper)helper).CellFormat;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FormatBase info) {
			ColumnBatchUpdateHelper columnBatchUpdateHelper = helper as ColumnBatchUpdateHelper;
			if (columnBatchUpdateHelper.CellFormat == null)
				columnBatchUpdateHelper.CellFormat = info.Clone();
			else
				columnBatchUpdateHelper.CellFormat.CopyFromDeferred(info); 
		}
		public void InitializeDeferredInfo(Column owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Column owner, Column from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(Column owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region ColumnWidthIndexAccessor
	public class ColumnWidthIndexAccessor : IIndexAccessor<Column, ColumnWidthInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<Column, ColumnWidthInfo> Members
		public int GetIndex(Column owner) {
			return owner.WidthIndex;
		}
		public int GetDeferredInfoIndex(Column owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Column owner, int value) {
			owner.AssignWidthIndex(value);
		}
		public int GetInfoIndex(Column owner, ColumnWidthInfo value) {
			return value.IntValue;
		}
		public ColumnWidthInfo GetInfo(Column owner) {
			ColumnWidthInfo info = new ColumnWidthInfo();
			info.IntValue = owner.WidthIndex;
			return info;
		}
		public bool IsIndexValid(Column owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Column owner) {
			return new ColumnWidthIndexChangeHistoryItem(owner);
		}
		public ColumnWidthInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((ColumnBatchUpdateHelper)helper).WidthInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, ColumnWidthInfo info) {
			((ColumnBatchUpdateHelper)helper).WidthInfo = info.Clone();
		}
		public void InitializeDeferredInfo(Column owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Column owner, Column from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(Column owner) {
			return owner.ReplaceInfoForFlags(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region ColumnInfoIndexAccessor
	public class ColumnInfoIndexAccessor : IIndexAccessor<Column, ColumnInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<Column, ColumnInfo> Members
		public int GetIndex(Column owner) {
			return owner.InfoIndex;
		}
		public int GetDeferredInfoIndex(Column owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Column owner, int value) {
			owner.AssignInfoIndex(value);
		}
		public int GetInfoIndex(Column owner, ColumnInfo value) {
			return value.PackedValues;
		}
		public ColumnInfo GetInfo(Column owner) {
			ColumnInfo info = new ColumnInfo();
			info.PackedValues = owner.InfoIndex;
			return info;
		}
		public bool IsIndexValid(Column owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Column owner) {
			return new ColumnInfoIndexChangeHistoryItem(owner);
		}
		public ColumnInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((ColumnBatchUpdateHelper)helper).Info;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, ColumnInfo info) {
			((ColumnBatchUpdateHelper)helper).Info = info.Clone();
		}
		public void InitializeDeferredInfo(Column owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Column owner, Column from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from).Clone());
		}
		public bool ApplyDeferredChanges(Column owner) {
			return owner.ReplaceInfoForFlags(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region ColumnBatchUpdateHelper
	public class ColumnBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		FormatBase cellFormat;
		ColumnWidthInfo widthInfo;
		ColumnInfo info;
		public ColumnBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public FormatBase CellFormat { get { return cellFormat; } set { cellFormat = value; } }
		public ColumnWidthInfo WidthInfo { get { return widthInfo; } set { widthInfo = value; } }
		public ColumnInfo Info { get { return info; } set { info = value; } }
		public override void BeginUpdateDeferredChanges() {
			cellFormat.BeginUpdate();
		}
		public override void CancelUpdateDeferredChanges() {
			cellFormat.CancelUpdate();
		}
		public override void EndUpdateDeferredChanges() {
			cellFormat.EndUpdate();
		}
	}
	public class ColumnBatchInitHelper : FormatBaseBatchUpdateHelper {
		public ColumnBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region ColumnCollection
	public class ColumnCollection {
		#region Fields
		readonly List<Column> innerList;
		readonly Worksheet sheet;
		int cachedContentVersion = -1;
		int cachedModelIndex = -1;
		int cachedColumnIndex;
		#endregion
		public ColumnCollection(Worksheet sheet) {
			this.sheet = sheet;
			this.innerList = new List<Column>();
		}
		#region Properties
		public int Count { get { return this.innerList.Count; } }
		public Worksheet Sheet { get { return this.sheet; } }
		protected internal Column First { get { return innerList.Count == 0 ? null : innerList[0]; } }
		protected internal Column Last { get { return innerList.Count == 0 ? null : innerList[innerList.Count - 1]; } }
		protected internal List<Column> InnerList { get { return innerList; } }
		#endregion
		public IColumnRange GetReadonlyColumnRange(int columnIndex) {
			return GetColumnRangeForReading(columnIndex);
		}
		public Column GetIsolatedColumn(int columnIndex) {
			if (!IndicesChecker.CheckIsColumnIndexValid(columnIndex))
				Exceptions.ThrowArgumentException("columnIndex", columnIndex);
			SplitAt(columnIndex);
			if (IndicesChecker.CheckIsColumnIndexValid(columnIndex + 1))
				SplitAt(columnIndex + 1);
			return GetColumnCore(columnIndex);
		}
		public Column GetColumnRangeForReading(int columnIndex) {
			if (!IndicesChecker.CheckIsColumnIndexValid(columnIndex))
				Exceptions.ThrowArgumentException("columnIndex", columnIndex);
			Column result = TryGetColumn(columnIndex);
			if (result == null)
				return new Column(Sheet, columnIndex, columnIndex);
			else
				return result;
		}
		public List<Column> GetColumnsForReading(int firstIndex, int lastIndex) {
			List<Column> result = new List<Column>();
			for (int i = firstIndex; i <= lastIndex; i++) {
				result.Add(GetColumnRangeForReading(i));
			}
			return result;
		}
		public Column CreateNewColumnRange(int firstColumnIndex, int lastColumnIndex) {
			CheckFirstLastColumnIndex(firstColumnIndex, lastColumnIndex);
			int startPosition = SplitAt(firstColumnIndex);
			if (startPosition < 0) {
				startPosition = ~startPosition;
			}
			int endPosition = SplitAt(lastColumnIndex + 1);
			if (endPosition < 0) {
				endPosition = ~endPosition;
			}
			if (endPosition != startPosition)
				RemoveRangeCore(startPosition, endPosition - startPosition, firstColumnIndex, lastColumnIndex - firstColumnIndex + 1, true, false);
			Column column = new Column(Sheet, firstColumnIndex, lastColumnIndex);
			InsertCore(startPosition, column);
			ResetTryGetColumnIndexCache();
			return column;
		}
		public IList<Column> GetColumnRangesEnsureExist(int firstColumnIndex, int lastColumnIndex) {
			int startPosition;
			int endPosition;
			CreateColumnRangesEnsureExistWithoutColumnsBetweenExisting(firstColumnIndex, lastColumnIndex, out startPosition, out endPosition);
			List<Column> result = new List<Column>();
			if (startPosition >= Count || endPosition > Count)
				return result;
			Column nextColumn = innerList[endPosition - 1];
			result.Add(nextColumn);
			for (int i = endPosition - 2; i >= startPosition; i--) {
				Column currentColumn = innerList[i];
				if (currentColumn.EndIndex + 1 < nextColumn.StartIndex) {
					Column column = CreateAndInsertColumnBetweenExisting(currentColumn.EndIndex + 1, nextColumn.StartIndex - 1, i + 1);
					result.Insert(0, column);
				}
				result.Insert(0, currentColumn);
				nextColumn = currentColumn;
			}
			return result;
		}
		public void CreateColumnRangesEnsureExist(int firstColumnIndex, int lastColumnIndex) {
			int startPosition;
			int endPosition;
			CreateColumnRangesEnsureExistWithoutColumnsBetweenExisting(firstColumnIndex, lastColumnIndex, out startPosition, out endPosition);
			if (startPosition >= Count || endPosition > Count)
				return;
			Column nextColumn = innerList[endPosition - 1];
			for (int i = endPosition - 2; i >= startPosition; i--) {
				Column currentColumn = innerList[i];
				if (currentColumn.EndIndex + 1 < nextColumn.StartIndex)
					CreateAndInsertColumnBetweenExisting(currentColumn.EndIndex + 1, nextColumn.StartIndex - 1, i + 1);
				nextColumn = currentColumn;
			}
		}
		void CheckFirstLastColumnIndex(int firstColumnIndex, int lastColumnIndex) {
			if (!IndicesChecker.CheckIsColumnIndexValid(firstColumnIndex))
				Exceptions.ThrowArgumentException("firstColumnIndex", firstColumnIndex);
			if (!IndicesChecker.CheckIsColumnIndexValid(firstColumnIndex))
				Exceptions.ThrowArgumentException("lastColumnIndex", lastColumnIndex);
			if (firstColumnIndex > lastColumnIndex)
				Exceptions.ThrowArgumentException("lastColumnIndex", lastColumnIndex);
		}
		void CreateColumnRangesEnsureExistWithoutColumnsBetweenExisting(int firstColumnIndex, int lastColumnIndex, out int startPosition, out int endPosition) {
			CheckFirstLastColumnIndex(firstColumnIndex, lastColumnIndex);
			startPosition = SplitAt(firstColumnIndex);
			if (startPosition < 0) {
				startPosition = ~startPosition;
				if (startPosition < Count) { 
					Column column = new Column(Sheet, firstColumnIndex, innerList[startPosition].StartIndex - 1);
					InsertCore(startPosition, column);
					ResetTryGetColumnIndexCache();
				}
			}
			endPosition = SplitAt(lastColumnIndex + 1);
			if (endPosition < 0) {
				endPosition = ~endPosition;
				int prevPosition = endPosition - 1;
				if (prevPosition < 0) {
					Column column = new Column(Sheet, firstColumnIndex, lastColumnIndex);
					InsertCore(endPosition, column);
					endPosition++;
				}
				else if (prevPosition < Count) { 
					Column last = innerList[prevPosition];
					if (last.EndIndex < lastColumnIndex) {
						int si = Math.Max(firstColumnIndex, last.EndIndex + 1);
						Column newColumnAfterLastExistingColumn = new Column(Sheet, si, lastColumnIndex);
						InsertCore(endPosition, newColumnAfterLastExistingColumn);
						endPosition++;
					}
				}
				ResetTryGetColumnIndexCache();
			}
		}
		Column CreateAndInsertColumnBetweenExisting(int startIndex, int endIndex, int position) {
			Column result = new Column(Sheet, startIndex, endIndex);
			InsertCore(position, result);
			ResetTryGetColumnIndexCache();
			return result;
		}
		int SplitAt(int columnIndex) {
			int position = TryGetColumnIndex(columnIndex);
			if (position < 0) 
				return position;
			Column column = innerList[position];
			if (column.StartIndex == columnIndex) 
				return position;
			int startIndex = column.StartIndex;
			column.AssignStartIndex(columnIndex);
			Column newColumn = new Column(Sheet, startIndex, columnIndex - 1);
			newColumn.CopyFrom(column);
			InsertCore(position, newColumn);
			ResetTryGetColumnIndexCache();
			return position + 1;
		}
		public IColumnRange TryGetColumnRange(int columnIndex) {
			return TryGetColumn(columnIndex);
		}
		internal Column TryGetColumn(int columnIndex) {
			int position = TryGetColumnIndex(columnIndex);
			if (position < 0)
				return null;
			else
				return this.innerList[position];
		}
		internal int TryGetColumnIndex(int modelIndex) {
			int workBookContentVersion = sheet.Workbook.ContentVersion;
			if (cachedModelIndex == modelIndex && workBookContentVersion == cachedContentVersion)
				return cachedColumnIndex;
			cachedContentVersion = workBookContentVersion;
			cachedModelIndex = modelIndex;
			cachedColumnIndex = Algorithms.BinarySearch<Column>(this.innerList, new ColumnComparable(modelIndex));
			return cachedColumnIndex;
		}
		public void ResetTryGetColumnIndexCache() {
			cachedModelIndex = -1;
			cachedContentVersion = -1;
			cachedColumnIndex = -1;
		}
		Column GetColumnCore(int index) {
			int position = TryGetColumnIndex(index);
			if (position < 0) {
				position = ~position;
				cachedColumnIndex = position; 
				Column column = new Column(this.sheet, index, index);
				InsertCore(position, column);
				return column;
			}
			else {
				System.Diagnostics.Debug.Assert(this.innerList[position].StartIndex == this.innerList[position].EndIndex);
				return this.innerList[position];
			}
		}
		public void InsertRange(int from, int count, InsertCellsFormatMode formatMode) {
			if (formatMode == InsertCellsFormatMode.FormatAsPrevious) {
				int positionFrom = 0;
				from--;
				if (from >= 0) {
					positionFrom = TryGetColumnIndex(from);
					if (positionFrom < 0)
						positionFrom = ~positionFrom;
				}
				else
					positionFrom = 0;
				if (positionFrom < this.innerList.Count)
					OffsetColumnIndices(positionFrom, from, count);
			}
			else if (formatMode == InsertCellsFormatMode.FormatAsNext) {
				int positionFrom;
				positionFrom = TryGetColumnIndex(from);
				if (positionFrom < 0)
					positionFrom = ~positionFrom;
				if (positionFrom < this.innerList.Count)
					OffsetColumnIndices(positionFrom, from, count);
			}
			else if (formatMode == InsertCellsFormatMode.ClearFormat) {
				int positionFrom = SplitAt(from);
				if (positionFrom < 0)
					positionFrom = ~positionFrom;
				from--;
				if (positionFrom < this.innerList.Count)
					OffsetColumnIndices(positionFrom, from, count);
			}
		}
		public void RemoveRange(int from, int count) {
			if (InnerList.Count == 0)
				return;
			bool removeRange = false;
			int positionFrom = TryGetColumnIndex(from);
			int positionTo = TryGetColumnIndex(from + count);
			if (positionFrom < 0 || positionFrom != positionTo) {
				positionFrom = SplitAt(from);
				positionTo = SplitAt(from + count);
				if (positionFrom < 0 && positionTo < 0) {
					positionFrom = ~positionFrom;
					if (positionFrom == InnerList.Count)
						return;
					positionTo = ~positionTo;
					if (positionTo != positionFrom) {
						removeRange = true;
					}
				}
				else {
					if (positionFrom < 0)
						positionFrom = ~positionFrom;
					if (positionTo < 0)
						positionTo = ~positionTo;
					else
						if (positionFrom < 0)
							positionTo++;
					removeRange = true;
				}
			}
			RemoveRangeCore(positionFrom, positionTo - positionFrom, from, count, removeRange, true);
		}
		protected internal void OffsetColumnIndices(int positionFrom, int fromColumnIndex, int offset) {
			using (HistoryTransaction transaction = new HistoryTransaction(Sheet.Workbook.History)) {
				SpreadsheetColumnsOffsetHistoryItem historyItem = new SpreadsheetColumnsOffsetHistoryItem(Sheet, positionFrom, fromColumnIndex, offset);
				sheet.Workbook.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		protected internal void OffsetColumnIndicesCore(int positionFrom, int fromColumnIndex, int offset) {
			ResetTryGetColumnIndexCache();
			for (int i = positionFrom; i < this.innerList.Count; i++)
				this.innerList[i].OffsetColumnIndex(fromColumnIndex, offset);
		}
		protected internal int RedoOffsetColumnIndices(int positionFrom, int fromColumnIndex, int offset, List<Column> removedColumns) {
			ResetTryGetColumnIndexCache();
			int count = this.innerList.Count;
			int savedEndIndex = 0;
			for (int i = count - 1; i >= positionFrom; i--) {
				Column item = this.innerList[i];
				int index = item.OffsetColumnIndexWithOverflow(fromColumnIndex, offset);
				if (index < 0) {
					removedColumns.Insert(0, item);
					this.innerList.RemoveAt(i);
				}
				else if (index > 0)
					savedEndIndex = index;
			}
			return savedEndIndex;
		}
		protected internal void UndoOffsetColumnIndices(int positionFrom, int fromColumnIndex, int offset, List<Column> savedColumns, int savedEndIndex) {
			ResetTryGetColumnIndexCache();
			int count = this.innerList.Count;
			if (count > 0) {
				for (int i = positionFrom; i < count; i++)
					this.innerList[i].OffsetColumnIndex(fromColumnIndex, offset);
				this.innerList[count - 1].RestoreEndIndex(savedEndIndex);
			}
			this.innerList.AddRange(savedColumns);
		}
		public virtual void Clear() {
			ResetTryGetColumnIndexCache();
			this.innerList.Clear();
		}
		public void ForEach(Action<Column> action) {
			innerList.ForEach(action);
		}
		void InsertCore(int index, Column column) {
			ResetTryGetColumnIndexCache();
			DocumentModel workbook = sheet.Workbook;
			DocumentHistory history = workbook.History;
			workbook.BeginUpdate();
			try {
				SpreadsheetColumnInsertedHistoryItem historyItem = new SpreadsheetColumnInsertedHistoryItem(Sheet, column, index);
				history.Add(historyItem);
				historyItem.Execute();
			}
			finally {
				workbook.EndUpdate();
			}
		}
		protected internal void InsertRangeFromHistory(int index, IList<Column> items) {
			ResetTryGetColumnIndexCache();
			if (items != null)
				InnerList.InsertRange(index, items);
		}
		void RemoveRangeCore(int positionFrom, int innerCount, int startIndex, int count, bool removeRange, bool offsetPositions) {
			if (count <= 0)
				return;
			SpreadsheetHistoryItem historyItem;
			DocumentHistory history = sheet.Workbook.History;
			if (offsetPositions)
				historyItem = new SpreadsheetColumnRangeRemovedHistoryItem(Sheet, positionFrom, innerCount, startIndex, count, removeRange);
			else
				historyItem = new SpreadsheetColumnRangeClearedHistoryItem(Sheet, positionFrom, innerCount);
			history.Add(historyItem);
			historyItem.Execute();
		}
		protected internal virtual void RemoveRangeInner(int index, int innerCount) {
			ResetTryGetColumnIndexCache();
			InnerList.RemoveRange(index, innerCount);
		}
		#region Column enumerators
		protected internal IEnumerable<Column> GetExistingColumns() {
			return innerList as IEnumerable<Column>;
		}
		protected internal IEnumerable<IColumnRange> GetExistingColumnRanges() {
			return new Enumerable<IColumnRange>(new EnumeratorConverter<Column, IColumnRange>(GetExistingColumns().GetEnumerator(), ConvertColumnToColumnRange));
		}
		protected internal IEnumerator<Column> GetAllColumnsEnumerator(int leftColumnIndex, int rightColumnIndex, bool reverseOrder) {
			return new AllColumnsEnumerator(Sheet, leftColumnIndex, rightColumnIndex, reverseOrder);
		}
		protected internal IEnumerator<Column> GetExistingColumnsEnumerator(int leftColumnIndex, int rightColumnIndex, bool reverseOrder) {
			return new ExistingColumnsEnumerator(innerList, leftColumnIndex, rightColumnIndex, reverseOrder);
		}
		protected internal IEnumerator<IColumnRange> GetExistingColumnRangesEnumerator(int leftColumnIndex, int rightColumnIndex, bool reverseOrder) {
			return new EnumeratorConverter<Column, IColumnRange>(GetExistingColumnsEnumerator(leftColumnIndex, rightColumnIndex, reverseOrder), ConvertColumnToColumnRange);
		}
		protected internal IEnumerator<Column> GetExistingVisibleColumnsEnumerator(int leftColumnIndex, int rightColumnIndex, bool reverseOrder) {
			return new ExistingVisibleColumnsEnumerator(innerList, leftColumnIndex, rightColumnIndex, reverseOrder);
		}
		protected internal IEnumerator<Column> GetExistingNotVisibleColumnsEnumerator(int leftColumnIndex, int rightColumnIndex, bool reverseOrder) {
			return new ExistingNotVisibleColumnsEnumerator(innerList, leftColumnIndex, rightColumnIndex, reverseOrder);
		}
		protected internal IEnumerator<IColumnRange> GetExistingVisibleColumnRangesEnumerator(int leftColumnIndex, int rightColumnIndex, bool reverseOrder) {
			return new EnumeratorConverter<Column, IColumnRange>(GetExistingVisibleColumnsEnumerator(leftColumnIndex, rightColumnIndex, reverseOrder), ConvertColumnToColumnRange);
		}
		static IColumnRange ConvertColumnToColumnRange(Column column) {
			return column;
		}
		#endregion
		#region ICell enumerators
		public static IEnumerable<ICell> GetExistingCells(Worksheet sheet, int columnIndex, int topRow, int bottomRow, bool reverseOrder) {
			return new Enumerable<ICell>(GetExistingCellsEnumerator(sheet, columnIndex, topRow, bottomRow, reverseOrder));
		}
		public static IOrderedEnumerator<ICell> GetExistingCellsEnumerator(Worksheet sheet, int columnIndex, int topRow, int bottomRow, bool reverseOrder) {
			return GetExistingCellsEnumeratorInfo(sheet, columnIndex, topRow, bottomRow, reverseOrder).Enumerator;
		}
		public static CellsOrderedEnumeratorInfo GetExistingCellsEnumeratorInfo(Worksheet sheet, int columnIndex, int topRow, int bottomRow, bool reverseOrder) {
			IOrderedItemsRangeEnumerator<Row> existingRows = sheet.Rows.GetExistingRowsEnumerator(topRow, bottomRow, reverseOrder);
			CellsOrderedEnumeratorInfo result = new CellsOrderedEnumeratorInfo(new ColumnCellsEnumerator(existingRows, columnIndex));
			ApplyInfinityModelIndices(result, existingRows, sheet);
			return result;
		}
		public static IOrderedEnumerator<ICell> GetExistingVisibleNonEmptyCellsEnumerator(Worksheet sheet, int columnIndex, int topRow, int bottomRow, bool reverseOrder) {
			return new ColumnNonEmptyCellsEnumerator(sheet.Rows.GetExistingVisibleRowsEnumerator(topRow, bottomRow, reverseOrder), columnIndex);
		}
		public static IEnumerable<ICell> GetAllCellsProvideFakeActualBorder(Worksheet sheet, int columnIndex, int topRow, int bottomRow, bool reverseOrder, IActualBorderInfo baseActualBorder) {
			return new Enumerable<ICell>(GetAllCellsProvideFakeActualBorderEnumerator(sheet, columnIndex, topRow, bottomRow, reverseOrder, baseActualBorder));
		}
		public static IEnumerator<ICell> GetAllCellsProvideFakeActualBorderEnumerator(Worksheet sheet, int columnIndex, int topRow, int bottomRow, bool reverseOrder, IActualBorderInfo baseActualBorder) {
			return GetAllCellsProvideFakeActualBorderEnumeratorInfo(sheet, columnIndex, topRow, bottomRow, reverseOrder, baseActualBorder).Enumerator;
		}
		public static CellsOrderedEnumeratorInfo GetAllCellsProvideFakeActualBorderEnumeratorInfo(Worksheet sheet, int columnIndex, int topRow, int bottomRow, bool reverseOrder, IActualBorderInfo baseActualBorder) {
			IOrderedItemsRangeEnumerator<Row> existingRows = sheet.Rows.GetExistingRowsEnumerator(topRow, bottomRow, reverseOrder);
			ContinuousRowsEnumeratorFakeRowsAsNull fakeRows = new ContinuousRowsEnumeratorFakeRowsAsNull(sheet, topRow, bottomRow, reverseOrder, null);
			JoinedOrderedEnumerator<Row> rowsEnumerator = new JoinedOrderedEnumerator<Row>(existingRows, fakeRows);
			CellsOrderedEnumeratorInfo result = new CellsOrderedEnumeratorInfo(new ContinuousColumnCellsProvideFakeActualBorderEnumerator(rowsEnumerator, columnIndex, sheet, topRow, bottomRow, baseActualBorder));
			ApplyInfinityModelIndices(result, existingRows, sheet);
			return result;
		}
		public static IEnumerator<ICell> GetAllCellsProvideColumnBorderEnumerator(Worksheet sheet, int columnIndex, int topRow, int bottomRow, bool reverseOrder) {
			return GetAllCellsProvideColumnBorderEnumeratorInfo(sheet, columnIndex, topRow, bottomRow, reverseOrder).Enumerator;
		}
		public static CellsOrderedEnumeratorInfo GetAllCellsProvideColumnBorderEnumeratorInfo(Worksheet sheet, int columnIndex, int topRow, int bottomRow, bool reverseOrder) {
			IOrderedItemsRangeEnumerator<Row> existingRows = sheet.Rows.GetExistingRowsEnumerator(topRow, bottomRow, reverseOrder);
			ContinuousRowsEnumeratorFakeRowsAsNull fakeRows = new ContinuousRowsEnumeratorFakeRowsAsNull(sheet, topRow, bottomRow, reverseOrder, null);
			JoinedOrderedEnumerator<Row> rowsEnumerator = new JoinedOrderedEnumerator<Row>(existingRows, fakeRows);
			CellsOrderedEnumeratorInfo result = new CellsOrderedEnumeratorInfo(new ContinuousColumnCellsProvideColumnBorderEnumerator(rowsEnumerator, columnIndex, sheet, topRow));
			ApplyInfinityModelIndices(result, existingRows, sheet);
			return result;
		}
		static void ApplyInfinityModelIndices(CellsOrderedEnumeratorInfo info, IOrderedItemsRangeEnumerator<Row> existingRowsEnumerator, Worksheet sheet) {
			int maxIndex = sheet.MaxRowCount - 1;
			info.InfinityEndModelIndex = maxIndex;
			IList<Row> rows = existingRowsEnumerator.Items;
			if (rows.Count <= 0) {
				info.InfinityStartModelIndex = Math.Min(maxIndex, CalculateBottomTablesRow(sheet) + 1);
				return;
			}
			if (existingRowsEnumerator.ShouldCalculateActualIndices()) {
				if (!existingRowsEnumerator.CalculateActualIndices() || existingRowsEnumerator.ActualLastIndex < 0)
					info.InfinityStartModelIndex = rows[rows.Count - 1].Index + 1;
				else
					info.InfinityStartModelIndex = rows[existingRowsEnumerator.ActualLastIndex].Index + 1;
				info.InfinityStartModelIndex = Math.Max(info.InfinityStartModelIndex, CalculateBottomTablesRow(sheet) + 1);
				info.InfinityEndModelIndex = Math.Max(info.InfinityStartModelIndex, maxIndex);
			}
			info.InfinityEndModelIndex = Math.Max(info.InfinityStartModelIndex, info.InfinityEndModelIndex);
		}
		static int CalculateBottomTablesRow(Worksheet sheet) {
			int result = 0;
			foreach (Table table in sheet.Tables)
				result = Math.Max(result, table.Range.BottomRight.Row);
			return result;
		}
		#endregion
		protected internal void MergeSameColumns(int firstColumnIndex, int lastColumnIndex) {
			if (!IndicesChecker.CheckIsColumnIndexValid(firstColumnIndex))
				Exceptions.ThrowArgumentException("firstColumnIndex", firstColumnIndex);
			if (!IndicesChecker.CheckIsColumnIndexValid(firstColumnIndex))
				Exceptions.ThrowArgumentException("lastColumnIndex", lastColumnIndex);
			if (firstColumnIndex > lastColumnIndex)
				Exceptions.ThrowArgumentException("lastColumnIndex", lastColumnIndex);
			int defaultFormatIndex = this.Sheet.Workbook.StyleSheet.DefaultCellFormatIndex;
			ResetTryGetColumnIndexCache();
			if (Count == 0)
				return;
			int startPosition = TryGetColumnIndex(firstColumnIndex);
			if (startPosition < 0) { 
				startPosition = ~startPosition;
				if (startPosition == Count) { 
					return;
				}
				Column columnAfterFistColumnIndex = innerList[startPosition];
				if (columnAfterFistColumnIndex.StartIndex < lastColumnIndex) {
				}
				else {
					return;
				}
			}
			int endPosition = TryGetColumnIndex(lastColumnIndex);
			if (endPosition < 0) {
				endPosition = ~endPosition;
				int prevPosition = endPosition - 1;
				if (endPosition == Count) { 
					endPosition = prevPosition; 
				}
				Column last = innerList[prevPosition];
				if (last.StartIndex == firstColumnIndex) {
					endPosition = prevPosition;
				}
			}
			if (startPosition == endPosition) {
				MergeSameColumnsCore(startPosition, endPosition, innerList[startPosition], defaultFormatIndex);
				return;
			}
			if (startPosition + 1 >= endPosition) {
				return;
			}
			int position = endPosition;
			Column lastColumnInRange = innerList[position];
			int lastColumnInRangePosition = endPosition;
			for (position--; position >= startPosition; ) {
				Column column = innerList[position];
				bool columnsAreSame = column.FormatIndex == lastColumnInRange.FormatIndex
					&& column.WidthIndex == lastColumnInRange.WidthIndex
					&& column.InfoIndex == lastColumnInRange.InfoIndex;
				if (columnsAreSame) {
				}
				else {
					Column differentColumn = column;
					MergeSameColumnsCore(position + 1, lastColumnInRangePosition, lastColumnInRange, defaultFormatIndex);
					lastColumnInRange = differentColumn;
					lastColumnInRangePosition = position;
				}
				if (position == startPosition) { 
					MergeSameColumnsCore(position, lastColumnInRangePosition, lastColumnInRange, defaultFormatIndex);
					return;
				}
				position--;
			}
		}
		void MergeSameColumnsCore(int rangeStartPosition, int lastColumnInRangePosition, Column lastColumnInRange, int defaultFormatIndex) {
			bool deleteThisRange = lastColumnInRange.FormatIndex == defaultFormatIndex
				&& lastColumnInRange.WidthIndex == 0
				&& lastColumnInRange.InfoIndex == 0;
			Column firstColumnInRange = (rangeStartPosition != lastColumnInRangePosition) ? innerList[rangeStartPosition] : lastColumnInRange;
			if (Object.ReferenceEquals(firstColumnInRange, lastColumnInRange)) {
				if (!deleteThisRange)
					return;
				RemoveRangeCore(rangeStartPosition, 1, firstColumnInRange.StartIndex, 1, true, false);
				return;
			}
			int positionFrom = rangeStartPosition;
			int innerCount = (lastColumnInRangePosition - 1) - rangeStartPosition + 1;
			int startIndex = firstColumnInRange.StartIndex;
			int count = (lastColumnInRange.StartIndex - 1) - startIndex + 1;
			if (deleteThisRange) {
				innerCount++;
				count++;
			}
			RemoveRangeCore(positionFrom, innerCount, startIndex, count, true, false);
			if (!deleteThisRange)
				lastColumnInRange.AssignStartIndex(startIndex);
		}
	}
	#endregion
	#region CellsEnumeratorInfo
	public class CellsOrderedEnumeratorInfo : IDisposable {
		public CellsOrderedEnumeratorInfo(IOrderedEnumerator<ICell> enumerator) {
			this.Enumerator = enumerator;
		}
		public IOrderedEnumerator<ICell> Enumerator { get; set; }
		public int InfinityStartModelIndex { get; set; }
		public int InfinityEndModelIndex { get; set; }
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				Enumerator.Dispose();
				Enumerator = null;
			}
		}
		#endregion
	}
	#endregion
	#region ColumnComparable
	internal class ColumnComparable : IComparable<Column> {
		readonly int index;
		public ColumnComparable(int index) {
			this.index = index;
		}
		public int CompareTo(Column other) {
			if (other.EndIndex < index)
				return -1;
			if (other.StartIndex > index)
				return 1;
			return 0;
		}
	}
	#endregion
	#region ColumnWidthInfo
	[StructLayout(LayoutKind.Explicit)]
	public struct ColumnWidthInfo : ICloneable<ColumnWidthInfo>, ISupportsSizeOf {
		const float defaultValue = 0.0f;
		public static float DefaultValue { get { return defaultValue; } }
		[FieldOffset(0)]
		float floatValue;
		[FieldOffset(0)]
		int intValue;
		public float FloatValue { get { return floatValue; } set { floatValue = value; } }
		public int IntValue { get { return intValue; } set { intValue = value; } }
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(ColumnWidthInfo));
		}
		#endregion
		#region ICloneable<ColumnWidthInfo> Members
		public ColumnWidthInfo Clone() {
			return this;
		}
		#endregion
	}
	#endregion
	#region ColumnInfo
	public struct ColumnInfo : ICloneable<ColumnInfo>, ISupportsSizeOf {
		#region Fields
		const int MaskOutlineLevel = 0x0000007;
		const int MaskBestFit = 0x00000008;
		const int MaskIsHidden = 0x00000010;
		const int MaskIsCollapsed = 0x00000020;
		const int MaskIsCustomWidth = 0x00000040;
		public const int DefaultValue = 0x0;
		int packedValues;
		#endregion
		#region Properties
		public int PackedValues { get { return packedValues; } set { this.packedValues = value; } }
		public int OutlineLevel {
			get {
				return (packedValues & MaskOutlineLevel);
			}
			set {
				packedValues &= ~MaskOutlineLevel;
				packedValues |= (value & MaskOutlineLevel);
			}
		}
		public bool BestFit { get { return GetBooleanValue(MaskBestFit); } set { SetBooleanValue(MaskBestFit, value); } }
		public bool IsHidden { get { return GetBooleanValue(MaskIsHidden); } set { SetBooleanValue(MaskIsHidden, value); } }
		public bool IsCollapsed { get { return GetBooleanValue(MaskIsCollapsed); } set { SetBooleanValue(MaskIsCollapsed, value); } }
		public bool IsCustomWidth { get { return GetBooleanValue(MaskIsCustomWidth); } set { SetBooleanValue(MaskIsCustomWidth, value); } }
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(int mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(int mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(ColumnInfo));
		}
		#endregion
		#region ICloneable<ColumnInfo> Members
		public ColumnInfo Clone() {
			return this;
		}
		#endregion
	}
	#endregion
	#region ColumnsLayoutVisibleCellsEnumerator
	public class ColumnsLayoutVisibleCellsEnumerator : IOrderedEnumerator<ICell> {
		readonly IEnumerator<IColumnRange> columnEnumerator;
		readonly Row row;
		readonly int firstColumnIndex;
		readonly int lastColumnIndex;
		ICell currentCell;
		IColumnRange currentColumnRange;
		int currentColumnIndex;
		public ColumnsLayoutVisibleCellsEnumerator(IEnumerator<IColumnRange> columnEnumerator, Row row, int firstColumnIndex, int lastColumnIndex) {
			this.columnEnumerator = columnEnumerator;
			this.row = row;
			this.firstColumnIndex = firstColumnIndex;
			this.lastColumnIndex = lastColumnIndex;
			this.currentColumnIndex = 0;
		}
		#region IOrderedEnumerator<ICell> Members
		public int CurrentValueOrder { get { return currentCell.ColumnIndex; } }
		public bool IsReverseOrder { get { return false; } }
		#endregion
		#region IEnumerator<ICell> Members
		public ICell Current { get { return currentCell; } }
		#endregion
		#region IDisposable Members
		public void Dispose() {
			IDisposable disposable = columnEnumerator as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current { get { return currentCell; } }
		public bool MoveNext() {
			if (currentColumnRange == null || currentColumnIndex >= currentColumnRange.EndIndex) {
				if (!columnEnumerator.MoveNext())
					return false;
				this.currentColumnRange = columnEnumerator.Current;
				this.currentColumnIndex = Math.Max(currentColumnRange.StartIndex, firstColumnIndex);
			}
			else
				currentColumnIndex++;
			if (currentColumnIndex > lastColumnIndex)
				return false;
			currentCell = new FakeCellWithColumnFormatting(currentColumnRange, currentColumnIndex, row.Index);
			return true;
		}
		public void Reset() {
			columnEnumerator.Reset();
			currentCell = null;
			currentColumnRange = null;
			currentColumnIndex = 0;
		}
		#endregion
		#region IShiftableEnumerator
		void IShiftableEnumerator.OnObjectInserted(int insertedItemValueOrder) {
		}
		void IShiftableEnumerator.OnObjectDeleted(int deletedItemValueOrder) {
		}
		#endregion
	}
	#endregion
	#region ExistingColumnsEnumerator
	public class ExistingColumnsEnumerator : SparseOrderedItemsRangeEnumerator<Column> {
		public ExistingColumnsEnumerator(IList<Column> columns, int nearItemIndex, int farItemIndex, bool reverseOrder)
			: base(columns, nearItemIndex, farItemIndex, reverseOrder, null) {
		}
		protected internal override IComparable<Column> CreateComparable(int itemIndex) {
			return new ColumnComparable(itemIndex);
		}
		protected internal override int GetCurrentValueOrder(int itemIndex) {
			return GetItem(itemIndex).Index;
		}
	}
	#endregion
	#region ExistingVisibleColumnsEnumerator
	public class ExistingVisibleColumnsEnumerator : ExistingColumnsEnumerator {
		public ExistingVisibleColumnsEnumerator(IList<Column> Columns, int nearItemIndex, int farItemIndex, bool reverseOrder)
			: base(Columns, nearItemIndex, farItemIndex, reverseOrder) {
		}
		protected override bool IsValidItem(int index) {
			return Items[index].IsVisible;
		}
	}
	#endregion
	#region ExistingNotVisibleColumnsEnumerator
	public class ExistingNotVisibleColumnsEnumerator : ExistingColumnsEnumerator {
		public ExistingNotVisibleColumnsEnumerator(IList<Column> Columns, int nearItemIndex, int farItemIndex, bool reverseOrder)
			: base(Columns, nearItemIndex, farItemIndex, reverseOrder) {
		}
		protected override bool IsValidItem(int index) {
			return !Items[index].IsVisible;
		}
	}
	#endregion
	#region ContinuousColumnsEnumerator
	public class ContinuousColumnsEnumerator : OrderedItemsRangeEnumerator<Column> {
		readonly Worksheet sheet;
		public ContinuousColumnsEnumerator(Worksheet sheet, int nearItemIndex, int farItemIndex, bool reverseOrder)
			: base(null, nearItemIndex, farItemIndex, reverseOrder, null) {
			this.sheet = sheet;
		}
		public Worksheet Sheet { get { return sheet; } }
		protected override Column GetItemCore(int index) {
			return CreateFakeColumn(index);
		}
		protected internal override int GetCurrentValueOrder(int itemIndex) {
			return itemIndex;
		}
		protected virtual Column CreateFakeColumn(int index) {
			return new Column(sheet, index, index);
		}
		public override void OnObjectInserted(int insertedItemValueOrder) {
		}
	}
	#endregion
	#region AllColumnsEnumerator
	public class AllColumnsEnumerator : ContinuousColumnsEnumerator {
		public AllColumnsEnumerator(Worksheet sheet, int nearItemIndex, int farItemIndex, bool reverseOrder)
			: base(sheet, nearItemIndex, farItemIndex, reverseOrder) {
		}
		protected override Column GetItemCore(int index) {
			return Sheet.Columns.GetIsolatedColumn(index);
		}
		protected internal override int GetCurrentValueOrder(int itemIndex) {
			return itemIndex;
		}
	}
	#endregion
}
