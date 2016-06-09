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
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.Office.Layout;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[DXToolboxItem(false)]
	#region FormatBorderControl
	public partial class FormatBorderControl : XtraUserControl, INotifyPropertyChanged, IPatternLinePaintingSupport {
		#region Fields
		readonly BorderLayoutManager borderLayoutManager;
		SpreadsheetHorizontalPatternLinePainter horizontalLinePainter;
		SpreadsheetVerticalPatternLinePainter verticalLinePainter;
		Graphics graphics;
		Color leftColor;
		Color topColor;
		Color bottomColor;
		Color rightColor;
		Color diagonalColor;
		Color horizontalColor;
		Color verticalColor;
		XlBorderLineStyle leftLineStyle;
		XlBorderLineStyle topLineStyle;
		XlBorderLineStyle bottomLineStyle;
		XlBorderLineStyle rightLineStyle;
		XlBorderLineStyle diagonalUpLineStyle;
		XlBorderLineStyle diagonalDownLineStyle;
		XlBorderLineStyle horizontalLineStyle;
		XlBorderLineStyle verticalLineStyle;
		SelectedRangeTypeForBorderPreview rangeType;
		#endregion
		public FormatBorderControl() {
			InitializeComponent();
			SubscribeEvents();
			this.borderLayoutManager = CreateBorderLayoutManager();
			this.DoubleBuffered = true;
		}
		#region Properties
		public Color LeftColor {
			get { return leftColor; }
			set {
				if (LeftColor == value)
					return;
				leftColor = value;
				OnPropertyChanged("LeftColor");
			}
		}
		public Color TopColor {
			get { return topColor; }
			set {
				if (TopColor == value)
					return;
				topColor = value;
				OnPropertyChanged("TopColor");
			}
		}
		public Color BottomColor {
			get { return bottomColor; }
			set {
				if (BottomColor == value)
					return;
				bottomColor = value;
				OnPropertyChanged("BottomColor");
			}
		}
		public Color RightColor {
			get { return rightColor; }
			set {
				if (RightColor == value)
					return;
				rightColor = value;
				OnPropertyChanged("RightColor");
			}
		}
		public Color DiagonalColor {
			get { return diagonalColor; }
			set {
				if (DiagonalColor == value)
					return;
				diagonalColor = value;
				OnPropertyChanged("DiagonalColor");
			}
		}
		public Color HorizontalColor {
			get { return horizontalColor; }
			set {
				if (HorizontalColor == value)
					return;
				horizontalColor = value;
				OnPropertyChanged("HorizontalColor");
			}
		}
		public Color VerticalColor {
			get { return verticalColor; }
			set {
				if (VerticalColor == value)
					return;
				verticalColor = value;
				OnPropertyChanged("VerticalColor");
			}
		}
		public XlBorderLineStyle? LeftLineStyle {
			get { return leftLineStyle; }
			set {
				if (LeftLineStyle == value)
					return;
				leftLineStyle = GetNotNullableBorderLineStyle(value);
				OnPropertyChanged("LeftLineStyle");
			}
		}
		public XlBorderLineStyle? TopLineStyle {
			get { return topLineStyle; }
			set {
				if (TopLineStyle == value)
					return;
				topLineStyle = GetNotNullableBorderLineStyle(value);
				OnPropertyChanged("TopLineStyle");
			}
		}
		public XlBorderLineStyle? BottomLineStyle {
			get { return bottomLineStyle; }
			set {
				if (BottomLineStyle == value)
					return;
				bottomLineStyle = GetNotNullableBorderLineStyle(value);
				OnPropertyChanged("BottomLineStyle");
			}
		}
		public XlBorderLineStyle? RightLineStyle {
			get { return rightLineStyle; }
			set {
				if (RightLineStyle == value)
					return;
				rightLineStyle = GetNotNullableBorderLineStyle(value);
				OnPropertyChanged("RightLineStyle");
			}
		}
		public XlBorderLineStyle? DiagonalUpLineStyle {
			get { return diagonalUpLineStyle; }
			set {
				if (DiagonalUpLineStyle == value)
					return;
				diagonalUpLineStyle = GetNotNullableBorderLineStyle(value);
				OnPropertyChanged("DiagonalUpLineStyle");
			}
		}
		public XlBorderLineStyle? DiagonalDownLineStyle {
			get { return diagonalDownLineStyle; }
			set {
				if (DiagonalDownLineStyle == value)
					return;
				diagonalDownLineStyle = GetNotNullableBorderLineStyle(value);
				OnPropertyChanged("DiagonalDownLineStyle");
			}
		}
		public XlBorderLineStyle? HorizontalLineStyle {
			get { return horizontalLineStyle; }
			set {
				if (HorizontalLineStyle == value)
					return;
				horizontalLineStyle = GetNotNullableBorderLineStyle(value);
				OnPropertyChanged("HorizontalLineStyle");
			}
		}
		public XlBorderLineStyle? VerticalLineStyle {
			get { return verticalLineStyle; }
			set {
				if (VerticalLineStyle == value)
					return;
				verticalLineStyle = GetNotNullableBorderLineStyle(value);
				OnPropertyChanged("VerticalLineStyle");
			}
		}
		XlBorderLineStyle GetNotNullableBorderLineStyle(XlBorderLineStyle? lineStyle) {
			return lineStyle.HasValue ? lineStyle.Value : XlBorderLineStyle.None;
		}
		public bool IsLeftBorderChecked {
			get { return this.chkLeftBorder.Checked; }
			set {
				if (IsLeftBorderChecked == value)
					return;
				this.chkLeftBorder.Checked = value;
			}
		}
		public bool IsRightBorderChecked {
			get { return this.chkRightBorder.Checked; }
			set {
				if (IsRightBorderChecked == value)
					return;
				this.chkRightBorder.Checked = value;
			}
		}
		public bool IsTopBorderChecked {
			get { return this.chkTopBorder.Checked; }
			set {
				if (IsTopBorderChecked == value)
					return;
				this.chkTopBorder.Checked = value;
			}
		}
		public bool IsBottomBorderChecked {
			get { return this.chkBottomBorder.Checked; }
			set {
				if (IsBottomBorderChecked == value)
					return;
				this.chkBottomBorder.Checked = value;
			}
		}
		public bool IsDiagonalDownBorderChecked {
			get { return this.chkDiagonalDownBorder.Checked; }
			set {
				if (IsDiagonalDownBorderChecked == value)
					return;
				this.chkDiagonalDownBorder.Checked = value;
			}
		}
		public bool IsDiagonalUpBorderChecked {
			get { return this.chkDiagonalUpBorder.Checked; }
			set {
				if (IsDiagonalUpBorderChecked == value)
					return;
				this.chkDiagonalUpBorder.Checked = value;
			}
		}
		public bool IsInsideVerticalBorderChecked {
			get { return this.chkInsideVerticalBorder.Checked; }
			set {
				if (IsInsideVerticalBorderChecked == value)
					return;
				this.chkInsideVerticalBorder.Checked = value;
			}
		}
		public bool IsInsideHorizontalBorderChecked {
			get { return this.chkInsideHorizontalBorder.Checked; }
			set {
				if (IsInsideHorizontalBorderChecked == value)
					return;
				this.chkInsideHorizontalBorder.Checked = value;
			}
		}
		public SelectedRangeTypeForBorderPreview RangeType {
			get { return rangeType; }
			set {
				if (RangeType == value)
					return;
				rangeType = value;
				OnPropertyChanged("RangeType");
				InitializeInsideBorderButtons();
			}
		}
		#endregion
		#region Events
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected void OnPropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		protected internal void InitializeInsideBorderButtons() {
			if (RangeType == SelectedRangeTypeForBorderPreview.Row)
				this.chkInsideVerticalBorder.Enabled = true;
			if (RangeType == SelectedRangeTypeForBorderPreview.Column)
				this.chkInsideHorizontalBorder.Enabled = true;
			if (RangeType == SelectedRangeTypeForBorderPreview.Table) {
				this.chkInsideHorizontalBorder.Enabled = true;
				this.chkInsideVerticalBorder.Enabled = true;
			}
		}
		protected internal void InitializeOutlineBorder(bool checkedValue) {
			this.chkLeftBorder.Checked = checkedValue;
			this.chkRightBorder.Checked = checkedValue;
			this.chkTopBorder.Checked = checkedValue;
			this.chkBottomBorder.Checked = checkedValue;
		}
		protected internal void InitializeNoBorderBorder(bool checkedValue) {
			this.chkLeftBorder.Checked = checkedValue;
			this.chkRightBorder.Checked = checkedValue;
			this.chkTopBorder.Checked = checkedValue;
			this.chkBottomBorder.Checked = checkedValue;
			this.chkInsideHorizontalBorder.Checked = checkedValue;
			this.chkInsideVerticalBorder.Checked = checkedValue;
			this.chkDiagonalDownBorder.Checked = checkedValue;
			this.chkDiagonalUpBorder.Checked = checkedValue;
		}
		protected internal void SubscribeEvents() {
			this.chkLeftBorder.CheckedChanged += OnLeftBorderCheckedChanged;
			this.chkRightBorder.CheckedChanged += OnRightBorderCheckedChanged;
			this.chkTopBorder.CheckedChanged += OnTopBorderCheckedChanged;
			this.chkBottomBorder.CheckedChanged += OnBottomBorderCheckedChanged;
			this.chkDiagonalUpBorder.CheckedChanged += OnDiagonalUpBorderCheckedChanged;
			this.chkDiagonalDownBorder.CheckedChanged += OnDiagonalDownBorderCheckedChanged;
			this.chkInsideVerticalBorder.CheckedChanged += OnInsideVerticalBorderCheckedChanged;
			this.chkInsideHorizontalBorder.CheckedChanged += OnInsideHorizontalBorderCheckedChanged;
			this.drawPanel.MouseUp += OnMouseUp;
			this.drawPanel.Paint += OnPaint;
		}
		BorderLayoutManager CreateBorderLayoutManager() {
			BorderLayoutManager manager = new BorderLayoutManager();
			manager.Initialize();
			return manager;
		}
		protected internal void InitializeBorderPainters(ISpreadsheetControl control) {
			DocumentLayoutUnitConverter converter = control.InnerControl.DocumentModel.LayoutUnitConverter;
			if (this.horizontalLinePainter == null)
				horizontalLinePainter = new SpreadsheetHorizontalPatternLinePainter(this, converter);
			if (this.verticalLinePainter == null)
				verticalLinePainter = new SpreadsheetVerticalPatternLinePainter(this, converter);
		}
		void OnLeftBorderCheckedChanged(object sender, EventArgs e) {
			OnPropertyChanged("IsLeftBorderChecked");
			this.drawPanel.Refresh();
		}
		void OnTopBorderCheckedChanged(object sender, EventArgs e) {
			OnPropertyChanged("IsTopBorderChecked");
			this.drawPanel.Refresh();
		}
		void OnBottomBorderCheckedChanged(object sender, EventArgs e) {
			OnPropertyChanged("IsBottomBorderChecked");
			this.drawPanel.Refresh();
		}
		void OnRightBorderCheckedChanged(object sender, EventArgs e) {
			OnPropertyChanged("IsRightBorderChecked");
			this.drawPanel.Refresh();
		}
		void OnDiagonalDownBorderCheckedChanged(object sender, EventArgs e) {
			OnPropertyChanged("IsDiagonalDownBorderChecked");
			this.drawPanel.Refresh();
		}
		void OnDiagonalUpBorderCheckedChanged(object sender, EventArgs e) {
			OnPropertyChanged("IsDiagonalUpBorderChecked");
			this.drawPanel.Refresh();
		}
		void OnInsideVerticalBorderCheckedChanged(object sender, EventArgs e) {
			OnPropertyChanged("IsInsideVerticalBorderChecked");
			this.drawPanel.Refresh();
		}
		void OnInsideHorizontalBorderCheckedChanged(object sender, EventArgs e) {
			OnPropertyChanged("IsInsideHorizontalBorderChecked");
			this.drawPanel.Refresh();
		}
		void OnPaint(object sender, PaintEventArgs e) {
			this.graphics = e.Graphics;
			borderLayoutManager.DrawCorners(graphics, RangeType);
			DrawLeftBorder();
			DrawRightBorder();
			DrawTopBorder();
			DrawBottomBorder();
			DrawDiagonalDownBorder();
			DrawDiagonalUpBorder();
			if (RangeType == SelectedRangeTypeForBorderPreview.Row || RangeType == SelectedRangeTypeForBorderPreview.Table)
				DrawInsideVerticalBorder();
			if (RangeType == SelectedRangeTypeForBorderPreview.Column || RangeType == SelectedRangeTypeForBorderPreview.Table)
				DrawInsideHorizontalBorder();
		}
		void OnMouseUp(object sender, MouseEventArgs e) {
			Point location = e.Location;
			if (borderLayoutManager.IsHitLeftBorderHotZone(location))
				InvertChecked(chkLeftBorder);
			else if (borderLayoutManager.IsHitRightBorderHotZone(location))
				InvertChecked(chkRightBorder);
			else if (borderLayoutManager.IsHitTopBorderHotZone(location))
				InvertChecked(chkTopBorder);
			else if (borderLayoutManager.IsHitBottomBorderHotZone(location))
				InvertChecked(chkBottomBorder);
			else if (RangeType == SelectedRangeTypeForBorderPreview.Cell) {
				if (borderLayoutManager.IsHitDiagonalDownBorderTopHotZone(location) || borderLayoutManager.IsHitDiagonalDownBorderBottomHotZone(location))
					InvertChecked(chkDiagonalDownBorder);
				if (borderLayoutManager.IsHitDiagonalUpBorderTopHotZone(location) || borderLayoutManager.IsHitDiagonalUpBorderBottomHotZone(location))
					InvertChecked(chkDiagonalUpBorder);
			}
			else if (RangeType == SelectedRangeTypeForBorderPreview.Row) {
				if (borderLayoutManager.IsHitDiagonalUpBorderHotZoneOneOnSetRowRange(location) || borderLayoutManager.IsHitDiagonalUpBorderHotZoneTwoOnSetRowRange(location) ||
					borderLayoutManager.IsHitDiagonalUpBorderHotZoneThreeOnSetRowRange(location) || borderLayoutManager.IsHitDiagonalUpBorderHotZoneFourOnSetRowRange(location))
					InvertChecked(chkDiagonalUpBorder);
				if (borderLayoutManager.IsHitDiagonalDownBorderHotZoneOneOnSetRowRange(location) || borderLayoutManager.IsHitDiagonalDownBorderHotZoneTwoOnSetRowRange(location) ||
					borderLayoutManager.IsHitDiagonalDownBorderHotZoneThreeOnSetRowRange(location) || borderLayoutManager.IsHitDiagonalDownBorderHotZoneFourOnSetRowRange(location))
					InvertChecked(chkDiagonalDownBorder);
				if (borderLayoutManager.IsHitInsideVerticalBorderHotZone(location))
					InvertChecked(chkInsideVerticalBorder);
			}
			else if (RangeType == SelectedRangeTypeForBorderPreview.Column) {
				if (borderLayoutManager.IsHitDiagonalUpBorderHotZoneOneOnSetColumnRange(location) || borderLayoutManager.IsHitDiagonalUpBorderHotZoneTwoOnSetColumnRange(location) ||
					borderLayoutManager.IsHitDiagonalUpBorderHotZoneThreeOnSetColumnRange(location) || borderLayoutManager.IsHitDiagonalUpBorderHotZoneFourOnSetColumnRange(location))
					InvertChecked(chkDiagonalUpBorder);
				if (borderLayoutManager.IsHitDiagonalDownBorderHotZoneOnSetColumnRange(location) || borderLayoutManager.IsHitDiagonalDownBorderHotZoneTwoOnSetColumnRange(location) ||
					borderLayoutManager.IsHitDiagonalDownBorderHotZoneThreeOnSetColumnRange(location) || borderLayoutManager.IsHitDiagonalDownBorderHotZoneFourOnSetColumnRange(location))
					InvertChecked(chkDiagonalDownBorder);
				if (borderLayoutManager.IsHitInsideHorizontalBorderHotZone(location))
					InvertChecked(chkInsideHorizontalBorder);
			}
			else if (RangeType == SelectedRangeTypeForBorderPreview.Table) {
				if (borderLayoutManager.IsHitDiagonalUpBorderHotZoneOneOnSetTableRange(location) || borderLayoutManager.IsHitDiagonalUpBorderHotZoneTwoOnSetTableRange(location) ||
					borderLayoutManager.IsHitDiagonalUpBorderHotZoneThreeOnSetTableRange(location) || borderLayoutManager.IsHitDiagonalUpBorderHotZoneFourOnSetTableRange(location) ||
					borderLayoutManager.IsHitDiagonalUpBorderHotZoneFiveOnSetTableRange(location) || borderLayoutManager.IsHitDiagonalUpBorderHotZoneSixOnSetTableRange(location) ||
					borderLayoutManager.IsHitDiagonalUpBorderHotZoneSevenOnSetTableRange(location) || borderLayoutManager.IsHitDiagonalUpBorderHotZoneEightOnSetTableRange(location))
					InvertChecked(chkDiagonalUpBorder);
				if (borderLayoutManager.IsHitDiagonalDownBorderHotZoneOneOnSetTableRange(location) || borderLayoutManager.IsHitDiagonalDownBorderHotZoneTwoOnSetTableRange(location) ||
					borderLayoutManager.IsHitDiagonalDownBorderHotZoneThreeOnSetTableRange(location) || borderLayoutManager.IsHitDiagonalDownBorderHotZoneFourOnSetTableRange(location) ||
					borderLayoutManager.IsHitDiagonalDownBorderHotZoneFiveOnSetTableRange(location) || borderLayoutManager.IsHitDiagonalDownBorderHotZoneSixOnSetTableRange(location) ||
					borderLayoutManager.IsHitDiagonalDownBorderHotZoneSevenOnSetTableRange(location) || borderLayoutManager.IsHitDiagonalDownBorderHotZoneEightOnSetTableRange(location))
					InvertChecked(chkDiagonalDownBorder);
				if (borderLayoutManager.IsHitInsideVerticalBorderHotZone(location))
					InvertChecked(chkInsideVerticalBorder);
				if (borderLayoutManager.IsHitInsideHorizontalBorderLeftHotZoneOnSetTableRange(location) || borderLayoutManager.IsHitInsideHorizontalBorderRightHotZoneOnSetTableRange(location))
					InvertChecked(chkInsideHorizontalBorder);
			}
		}
		void InvertChecked(CheckButton button) {
			button.Checked = !button.Checked;
		}
		void DrawLeftBorder() {
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(leftLineStyle);
			BorderLayout leftBorder = borderLayoutManager.CalculateLeftBorderInfo(leftLineStyle);
			DrawBorderCore(verticalLinePainter, borderLine, leftBorder, GetColor(leftLineStyle, leftColor));
		}
		void DrawRightBorder() {
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(rightLineStyle);
			BorderLayout rightBorder = borderLayoutManager.CalculateRightBorderInfo(rightLineStyle);
			DrawBorderCore(verticalLinePainter, borderLine, rightBorder, GetColor(rightLineStyle, rightColor));
		}
		void DrawTopBorder() {
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(topLineStyle);
			BorderLayout topBorder = borderLayoutManager.CalculateTopBorderInfo(topLineStyle);
			DrawBorderCore(horizontalLinePainter, borderLine, topBorder, GetColor(topLineStyle, topColor));
		}
		void DrawBottomBorder() {
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(bottomLineStyle);
			BorderLayout bottomBorder = borderLayoutManager.CalculateBottomBorderInfo(bottomLineStyle);
			DrawBorderCore(horizontalLinePainter, borderLine, bottomBorder, GetColor(bottomLineStyle, bottomColor));
		}
		void DrawDiagonalDownBorder() {
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(diagonalDownLineStyle);
			BorderLayout diagonalsDownBorder;
			if (RangeType == SelectedRangeTypeForBorderPreview.Cell) {
				diagonalsDownBorder = borderLayoutManager.CalculateDiagonalDownBorderInfo(diagonalDownLineStyle);
				DrawBorderCore(horizontalLinePainter, borderLine, diagonalsDownBorder, GetColor(diagonalDownLineStyle, diagonalColor));
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Row) {
				diagonalsDownBorder = borderLayoutManager.CalculateLeftDiagonalDownOnSetRowRange(diagonalDownLineStyle);
				DrawBorderCore(verticalLinePainter, borderLine, diagonalsDownBorder, GetColor(diagonalDownLineStyle, diagonalColor));
				diagonalsDownBorder = borderLayoutManager.CalculateRightDiagonalDownOnSetRowRange(diagonalDownLineStyle);
				DrawBorderCore(verticalLinePainter, borderLine, diagonalsDownBorder, GetColor(diagonalDownLineStyle, diagonalColor));
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Column) {
				diagonalsDownBorder = borderLayoutManager.CalculateTopDiagonalDownOnSetColumnRange(diagonalDownLineStyle);
				DrawBorderCore(verticalLinePainter, borderLine, diagonalsDownBorder, GetColor(diagonalDownLineStyle, diagonalColor));
				diagonalsDownBorder = borderLayoutManager.CalculateBottomDiagonalDownOnSetColumnRange(diagonalDownLineStyle);
				DrawBorderCore(verticalLinePainter, borderLine, diagonalsDownBorder, GetColor(diagonalDownLineStyle, diagonalColor));
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Table) {
				diagonalsDownBorder = borderLayoutManager.CalculateDiagonalDownBorderInfo(diagonalDownLineStyle);
				DrawBorderCore(horizontalLinePainter, borderLine, diagonalsDownBorder, GetColor(diagonalDownLineStyle, diagonalColor));
				diagonalsDownBorder = borderLayoutManager.CalculateTopDiagonalDownOnSetTableRange(diagonalDownLineStyle);
				DrawBorderCore(horizontalLinePainter, borderLine, diagonalsDownBorder, GetColor(diagonalDownLineStyle, diagonalColor));
				diagonalsDownBorder = borderLayoutManager.CalculateBottomDiagonalDownOnSetTableRange(diagonalDownLineStyle);
				DrawBorderCore(horizontalLinePainter, borderLine, diagonalsDownBorder, GetColor(diagonalDownLineStyle, diagonalColor));
			}
		}
		void DrawDiagonalUpBorder() {
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(diagonalUpLineStyle);
			BorderLayout diagonalsUpBorder;
			if (RangeType == SelectedRangeTypeForBorderPreview.Cell) {
				diagonalsUpBorder = borderLayoutManager.CalculateDiagonalUpBorderInfo(diagonalUpLineStyle);
				DrawBorderCore(horizontalLinePainter, borderLine, diagonalsUpBorder, GetColor(diagonalUpLineStyle, diagonalColor));
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Row) {
				diagonalsUpBorder = borderLayoutManager.CalculateLeftDiagonalUpOnSetRowRange(diagonalUpLineStyle);
				DrawBorderCore(verticalLinePainter, borderLine, diagonalsUpBorder, GetColor(diagonalUpLineStyle, diagonalColor));
				diagonalsUpBorder = borderLayoutManager.CalculateRightDiagonalUpOnSetRowRange(diagonalUpLineStyle);
				DrawBorderCore(verticalLinePainter, borderLine, diagonalsUpBorder, GetColor(diagonalUpLineStyle, diagonalColor));
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Column) {
				diagonalsUpBorder = borderLayoutManager.CalculateTopDiagonalUpOnSetColumnRange(diagonalUpLineStyle);
				DrawBorderCore(horizontalLinePainter, borderLine, diagonalsUpBorder, GetColor(diagonalUpLineStyle, diagonalColor));
				diagonalsUpBorder = borderLayoutManager.CalculateBottomDiagonalUpOnSetColumnRange(diagonalUpLineStyle);
				DrawBorderCore(horizontalLinePainter, borderLine, diagonalsUpBorder, GetColor(diagonalUpLineStyle, diagonalColor));
			}
			if (RangeType == SelectedRangeTypeForBorderPreview.Table) {
				diagonalsUpBorder = borderLayoutManager.CalculateDiagonalUpBorderInfo(diagonalUpLineStyle);
				DrawBorderCore(horizontalLinePainter, borderLine, diagonalsUpBorder, GetColor(diagonalUpLineStyle, diagonalColor));
				diagonalsUpBorder = borderLayoutManager.CalculateTopDiagonalUpOnSetTableRange(diagonalUpLineStyle);
				DrawBorderCore(horizontalLinePainter, borderLine, diagonalsUpBorder, GetColor(diagonalUpLineStyle, diagonalColor));
				diagonalsUpBorder = borderLayoutManager.CalculateBottomDiagonalUpOnSetTableRange(diagonalUpLineStyle);
				DrawBorderCore(horizontalLinePainter, borderLine, diagonalsUpBorder, GetColor(diagonalUpLineStyle, diagonalColor));
			}
		}
		void DrawInsideVerticalBorder() {
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(verticalLineStyle);
			BorderLayout insideVerticalBorder = borderLayoutManager.CalculateInsideVerticalBorderInfo(verticalLineStyle);
			DrawBorderCore(verticalLinePainter, borderLine, insideVerticalBorder, GetColor(verticalLineStyle, verticalColor));
		}
		void DrawInsideHorizontalBorder() {
			BorderLine borderLine = DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(horizontalLineStyle);
			BorderLayout insideHorizontalBorder = borderLayoutManager.CalculateInsideHorizontalBorderInfo(horizontalLineStyle);
			DrawBorderCore(horizontalLinePainter, borderLine, insideHorizontalBorder, GetColor(horizontalLineStyle, horizontalColor));
		}
		void DrawBorderCore(SpreadsheetPatternLinePainter linePainter, BorderLine borderLine, BorderLayout infoBorderLine, Color color) {
			borderLine.Draw(linePainter, infoBorderLine.Start, infoBorderLine.End, color, infoBorderLine.Width);
		}
		Color GetColor(XlBorderLineStyle lineStyle, Color color) {
			if (lineStyle == SpecialBorderLineStyle.OutsideComplexBorder || lineStyle == SpecialBorderLineStyle.InsideComplexBorder)
				return Color.LightGray;
			return color;
		}
		#region IPatternLinePaintingSupport Members
		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2) {
			graphics.DrawLine(pen, x1, y1, x2, y2);
		}
		public void DrawLines(Pen pen, PointF[] points) {
			graphics.DrawLines(pen, points);
		}
		public Brush GetBrush(Color color) {
			return new SolidBrush(color);
		}
		public Pen GetPen(Color color, float thickness) {
			return new Pen(color, thickness);
		}
		public Pen GetPen(Color color) {
			return new Pen(color);
		}
		public void ReleaseBrush(Brush brush) {
			brush.Dispose();
		}
		public void ReleasePen(Pen pen) {
			pen.Dispose();
		}
		#endregion
	}
	#endregion
	#region BorderLayout
	public struct BorderLayout {
		#region Fields
		Point start;
		Point end;
		int width;
		#endregion
		public BorderLayout(Point start, Point end, int width) {
			this.start = start;
			this.end = end;
			this.width = width;
		}
		#region Properties
		public Point Start {
			get { return start; }
		}
		public Point End {
			get { return end; }
		}
		public int Width {
			get { return width; }
		}
		#endregion
	}
	#endregion
	#region BorderLayoutManager
	public class BorderLayoutManager {
		#region Fields
		static int centerCornerOffset = 12;
		static int hotZoneOffset = 10;
		static int horizontalCenterDrawPanel = 85;
		static int leftHalfHorizontalCenterDrawPanel = 50;
		static int rightHalfHorizontalCenterDrawPanel = 122;
		static int verticalCenterDrawPanel = 51;
		static int topHalfVerticalCenterDrawPanel = 32;
		static int bottomHalfVerticalCenterDrawPanel = 70;
		static Color cornerLineColor = Color.Gray;
		static int cornerLineWidth = 1;
		static int cornerLineLength = 5;
		Point topLeftCenterCorner = new Point(centerCornerOffset, centerCornerOffset);
		Point topRightCenterCorner = new Point(horizontalCenterDrawPanel * 2 - centerCornerOffset, centerCornerOffset);
		Point bottomLeftCenterCorner = new Point(centerCornerOffset, verticalCenterDrawPanel * 2 - centerCornerOffset);
		Point bottomRightCenterCorner = new Point(horizontalCenterDrawPanel * 2 - centerCornerOffset, verticalCenterDrawPanel * 2 - centerCornerOffset);
		Point topMiddleCenterCorner = new Point(horizontalCenterDrawPanel, centerCornerOffset);
		Point bottomMiddleCenterCorner = new Point(horizontalCenterDrawPanel, verticalCenterDrawPanel * 2 - centerCornerOffset);
		Point rightMiddleCenterCorner = new Point(horizontalCenterDrawPanel * 2 - centerCornerOffset, verticalCenterDrawPanel);
		Point leftMiddleCenterCorner = new Point(centerCornerOffset, verticalCenterDrawPanel);
		Point[] topLeftCornerPoints;
		Point[] topRightCornerPoints;
		Point[] bottomLeftCornerPoints;
		Point[] bottomRightCornerPoints;
		Point[] topMiddleCenterCornerPoints;
		Point[] bottomMiddleCenterCornerPoints;
		Point[] rightMiddleCenterCornerPoints;
		Point[] leftMiddleCenterCornerPoints;
		Rectangle leftBorderHotZone;
		Rectangle rightBorderHotZone;
		Rectangle topBorderHotZone;
		Rectangle bottomBorderHotZone;
		Rectangle diagonalDownBorderTopHotZone;
		Rectangle diagonalDownBorderBottomHotZone;
		Rectangle diagonalUpBorderTopHotZone;
		Rectangle diagonalUpBorderBottomHotZone;
		Rectangle insideVerticalBorderHotZone;
		Rectangle insideHorizontalBorderHotZone;
		Rectangle diagonalUpBorderOneHotZoneOnSetRowRange;
		Rectangle diagonalUpBorderTwoHotZoneOnSetRowRange;
		Rectangle diagonalUpBorderThreeHotZoneOnSetRowRange;
		Rectangle diagonalUpBorderFourHotZoneOnSetRowRange;
		Rectangle diagonalDownBorderOneHotZoneOnSetRowRange;
		Rectangle diagonalDownBorderTwoHotZoneOnSetRowRange;
		Rectangle diagonalDownBorderThreeHotZoneOnSetRowRange;
		Rectangle diagonalDownBorderFourHotZoneOnSetRowRange;
		Rectangle diagonalUpBorderOneHotZoneOnSetColumnRange;
		Rectangle diagonalUpBorderTwoHotZoneOnSetColumnRange;
		Rectangle diagonalUpBorderThreeHotZoneOnSetColumnRange;
		Rectangle diagonalUpBorderFourHotZoneOnSetColumnRange;
		Rectangle diagonalDownBorderOneHotZoneOnSetColumnRange;
		Rectangle diagonalDownBorderTwoHotZoneOnSetColumnRange;
		Rectangle diagonalDownBorderThreeHotZoneOnSetColumnRange;
		Rectangle diagonalDownBorderFourHotZoneOnSetColumnRange;
		Rectangle diagonalUpBorderOneHotZoneOnSetTableRange;
		Rectangle diagonalUpBorderTwoHotZoneOnSetTableRange;
		Rectangle diagonalUpBorderThreeHotZoneOnSetTableRange;
		Rectangle diagonalUpBorderFourHotZoneOnSetTableRange;
		Rectangle diagonalUpBorderFiveHotZoneOnSetTableRange;
		Rectangle diagonalUpBorderSixHotZoneOnSetTableRange;
		Rectangle diagonalUpBorderSevenHotZoneOnSetTableRange;
		Rectangle diagonalUpBorderEightHotZoneOnSetTableRange;
		Rectangle diagonalDownBorderOneHotZoneOnSetTableRange;
		Rectangle diagonalDownBorderTwoHotZoneOnSetTableRange;
		Rectangle diagonalDownBorderThreeHotZoneOnSetTableRange;
		Rectangle diagonalDownBorderFourHotZoneOnSetTableRange;
		Rectangle diagonalDownBorderFiveHotZoneOnSetTableRange;
		Rectangle diagonalDownBorderSixHotZoneOnSetTableRange;
		Rectangle diagonalDownBorderSevenHotZoneOnSetTableRange;
		Rectangle diagonalDownBorderEightHotZoneOnSetTableRange;
		Rectangle insideHorizontalBorderLeftHotZoneOnSetTableRange;
		Rectangle insideHorizontalBorderRightHotZoneOnSetTableRange;
		#endregion
		#region Properties
		protected internal Rectangle LeftBorderHotZone { get { return leftBorderHotZone; } }
		protected internal Rectangle RightBorderHotZone { get { return rightBorderHotZone; } }
		protected internal Rectangle TopBorderHotZone { get { return topBorderHotZone; } }
		protected internal Rectangle BottomBorderHotZone { get { return bottomBorderHotZone; } }
		protected internal Rectangle DiagonalDownBorderTopHotZone { get { return diagonalDownBorderTopHotZone; } }
		protected internal Rectangle DiagonalDownBorderBottomHotZone { get { return diagonalDownBorderBottomHotZone; } }
		protected internal Rectangle DiagonalUpBorderTopHotZone { get { return diagonalUpBorderTopHotZone; } }
		protected internal Rectangle DiagonalUpBorderBottomHotZone { get { return diagonalUpBorderBottomHotZone; } }
		protected internal Rectangle InsideVerticalBorderHotZone { get { return insideVerticalBorderHotZone; } }
		protected internal Rectangle InsideHorizontalBorderHotZone { get { return insideHorizontalBorderHotZone; } }
		protected internal Rectangle DiagonalUpBorderOneHotZoneOnSetRowRange { get { return diagonalUpBorderOneHotZoneOnSetRowRange; } }
		protected internal Rectangle DiagonalUpBorderTwoHotZoneOnSetRowRange { get { return diagonalUpBorderTwoHotZoneOnSetRowRange; } }
		protected internal Rectangle DiagonalUpBorderThreeHotZoneOnSetRowRange { get { return diagonalUpBorderThreeHotZoneOnSetRowRange; } }
		protected internal Rectangle DiagonalUpBorderFourHotZoneOnSetRowRange { get { return diagonalUpBorderFourHotZoneOnSetRowRange; } }
		protected internal Rectangle DiagonalDownBorderOneHotZoneOnSetRowRange { get { return diagonalDownBorderOneHotZoneOnSetRowRange; } }
		protected internal Rectangle DiagonalDownBorderTwoHotZoneOnSetRowRange { get { return diagonalDownBorderTwoHotZoneOnSetRowRange; } }
		protected internal Rectangle DiagonalDownBorderThreeHotZoneOnSetRowRange { get { return diagonalDownBorderThreeHotZoneOnSetRowRange; } }
		protected internal Rectangle DiagonalDownBorderFourHotZoneOnSetRowRange { get { return diagonalDownBorderFourHotZoneOnSetRowRange; } }
		protected internal Rectangle DiagonalUpBorderOneHotZoneOnSetColumnRange { get { return diagonalUpBorderOneHotZoneOnSetColumnRange; } }
		protected internal Rectangle DiagonalUpBorderTwoHotZoneOnSetColumnRange { get { return diagonalUpBorderTwoHotZoneOnSetColumnRange; } }
		protected internal Rectangle DiagonalUpBorderThreeHotZoneOnSetColumnRange { get { return diagonalUpBorderThreeHotZoneOnSetColumnRange; } }
		protected internal Rectangle DiagonalUpBorderFourHotZoneOnSetColumnRange { get { return diagonalUpBorderFourHotZoneOnSetColumnRange; } }
		protected internal Rectangle DiagonalDownBorderOneHotZoneOnSetColumnRange { get { return diagonalDownBorderOneHotZoneOnSetColumnRange; } }
		protected internal Rectangle DiagonalDownBorderTwoHotZoneOnSetColumnRange { get { return diagonalDownBorderTwoHotZoneOnSetColumnRange; } }
		protected internal Rectangle DiagonalDownBorderThreeHotZoneOnSetColumnRange { get { return diagonalDownBorderThreeHotZoneOnSetColumnRange; } }
		protected internal Rectangle DiagonalDownBorderFourHotZoneOnSetColumnRange { get { return diagonalDownBorderFourHotZoneOnSetColumnRange; } }
		protected internal Rectangle DiagonalUpBorderOneHotZoneOnSetTableRange { get { return diagonalUpBorderOneHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalUpBorderTwoHotZoneOnSetTableRange { get { return diagonalUpBorderTwoHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalUpBorderThreeHotZoneOnSetTableRange { get { return diagonalUpBorderThreeHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalUpBorderFourHotZoneOnSetTableRange { get { return diagonalUpBorderFourHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalUpBorderFiveHotZoneOnSetTableRange { get { return diagonalUpBorderFiveHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalUpBorderSixHotZoneOnSetTableRange { get { return diagonalUpBorderSixHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalUpBorderSevenHotZoneOnSetTableRange { get { return diagonalUpBorderSevenHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalUpBorderEightHotZoneOnSetTableRange { get { return diagonalUpBorderEightHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalDownBorderOneHotZoneOnSetTableRange { get { return diagonalDownBorderOneHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalDownBorderTwoHotZoneOnSetTableRange { get { return diagonalDownBorderTwoHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalDownBorderThreeHotZoneOnSetTableRange { get { return diagonalDownBorderThreeHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalDownBorderFourHotZoneOnSetTableRange { get { return diagonalDownBorderFourHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalDownBorderFiveHotZoneOnSetTableRange { get { return diagonalDownBorderFiveHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalDownBorderSixHotZoneOnSetTableRange { get { return diagonalDownBorderSixHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalDownBorderSevenHotZoneOnSetTableRange { get { return diagonalDownBorderSevenHotZoneOnSetTableRange; } }
		protected internal Rectangle DiagonalDownBorderEightHotZoneOnSetTableRange { get { return diagonalDownBorderEightHotZoneOnSetTableRange; } }
		protected internal Rectangle InsideHorizontalBorderLeftHotZoneOnSetTableRange { get { return insideHorizontalBorderLeftHotZoneOnSetTableRange; } }
		protected internal Rectangle InsideHorizontalBorderRightHotZoneOnSetTableRange { get { return insideHorizontalBorderRightHotZoneOnSetTableRange; } }
		#endregion
		protected internal void Initialize() {
			this.topLeftCornerPoints = new Point[] { new Point(leftMiddleCenterCorner.X - cornerLineLength, topMiddleCenterCorner.Y), topLeftCenterCorner, new Point(leftMiddleCenterCorner.X, topMiddleCenterCorner.Y - cornerLineLength) };
			this.topRightCornerPoints = new Point[] { new Point(rightMiddleCenterCorner.X + cornerLineLength, topMiddleCenterCorner.Y), topRightCenterCorner, new Point(rightMiddleCenterCorner.X, topMiddleCenterCorner.Y - cornerLineLength) };
			this.bottomLeftCornerPoints = new Point[] { new Point(leftMiddleCenterCorner.X - cornerLineLength, bottomMiddleCenterCorner.Y), bottomLeftCenterCorner, new Point(leftMiddleCenterCorner.X, bottomMiddleCenterCorner.Y + cornerLineLength) };
			this.bottomRightCornerPoints = new Point[] { new Point(rightMiddleCenterCorner.X + cornerLineLength, bottomMiddleCenterCorner.Y), bottomRightCenterCorner, new Point(rightMiddleCenterCorner.X, bottomMiddleCenterCorner.Y + cornerLineLength) };
			this.topMiddleCenterCornerPoints = new Point[] { new Point(topMiddleCenterCorner.X - cornerLineLength, topMiddleCenterCorner.Y), new Point(topMiddleCenterCorner.X, topMiddleCenterCorner.Y), new Point(topMiddleCenterCorner.X, topMiddleCenterCorner.Y - cornerLineLength), new Point(topMiddleCenterCorner.X, topMiddleCenterCorner.Y), new Point(topMiddleCenterCorner.X + cornerLineLength, topMiddleCenterCorner.Y) };
			this.bottomMiddleCenterCornerPoints = new Point[] { new Point(topMiddleCenterCorner.X - cornerLineLength, bottomMiddleCenterCorner.Y), new Point(topMiddleCenterCorner.X, bottomMiddleCenterCorner.Y), new Point(topMiddleCenterCorner.X, bottomMiddleCenterCorner.Y + cornerLineLength), new Point(topMiddleCenterCorner.X, bottomMiddleCenterCorner.Y), new Point(topMiddleCenterCorner.X + cornerLineLength, bottomMiddleCenterCorner.Y) };
			this.leftMiddleCenterCornerPoints = new Point[] { new Point(leftMiddleCenterCorner.X, leftMiddleCenterCorner.Y - cornerLineLength), new Point(leftMiddleCenterCorner.X, leftMiddleCenterCorner.Y), new Point(leftMiddleCenterCorner.X - cornerLineLength, leftMiddleCenterCorner.Y), new Point(leftMiddleCenterCorner.X, leftMiddleCenterCorner.Y), new Point(leftMiddleCenterCorner.X, leftMiddleCenterCorner.Y + cornerLineLength) };
			this.rightMiddleCenterCornerPoints = new Point[] { new Point(rightMiddleCenterCorner.X, leftMiddleCenterCorner.Y - cornerLineLength), new Point(rightMiddleCenterCorner.X, leftMiddleCenterCorner.Y), new Point(rightMiddleCenterCorner.X + cornerLineLength, leftMiddleCenterCorner.Y), new Point(rightMiddleCenterCorner.X, leftMiddleCenterCorner.Y), new Point(rightMiddleCenterCorner.X, leftMiddleCenterCorner.Y + cornerLineLength) };
			this.leftBorderHotZone = Rectangle.FromLTRB(leftMiddleCenterCorner.X - hotZoneOffset, topMiddleCenterCorner.Y, leftMiddleCenterCorner.X + hotZoneOffset, bottomMiddleCenterCorner.Y);
			this.rightBorderHotZone = Rectangle.FromLTRB(rightMiddleCenterCorner.X - hotZoneOffset, topMiddleCenterCorner.Y, rightMiddleCenterCorner.X + hotZoneOffset, bottomMiddleCenterCorner.Y);
			this.topBorderHotZone = Rectangle.FromLTRB(leftMiddleCenterCorner.X, topMiddleCenterCorner.Y - hotZoneOffset, rightMiddleCenterCorner.X, topMiddleCenterCorner.Y + hotZoneOffset);
			this.bottomBorderHotZone = Rectangle.FromLTRB(leftMiddleCenterCorner.X, bottomMiddleCenterCorner.Y - hotZoneOffset, rightMiddleCenterCorner.X, bottomMiddleCenterCorner.Y + hotZoneOffset);
			this.diagonalDownBorderTopHotZone = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, topMiddleCenterCorner.Y + hotZoneOffset, topMiddleCenterCorner.X, leftMiddleCenterCorner.Y);
			this.diagonalDownBorderBottomHotZone = Rectangle.FromLTRB(topMiddleCenterCorner.X, leftMiddleCenterCorner.Y, rightMiddleCenterCorner.X - hotZoneOffset, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderTopHotZone = Rectangle.FromLTRB(topMiddleCenterCorner.X, topMiddleCenterCorner.Y + hotZoneOffset, rightMiddleCenterCorner.X - hotZoneOffset, leftMiddleCenterCorner.Y);
			this.diagonalUpBorderBottomHotZone = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, leftMiddleCenterCorner.Y, topMiddleCenterCorner.X, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.insideVerticalBorderHotZone = Rectangle.FromLTRB(topMiddleCenterCorner.X - hotZoneOffset, topMiddleCenterCorner.Y + hotZoneOffset, topMiddleCenterCorner.X + hotZoneOffset, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.insideHorizontalBorderHotZone = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, leftMiddleCenterCorner.Y - hotZoneOffset, rightMiddleCenterCorner.X - hotZoneOffset, leftMiddleCenterCorner.Y + hotZoneOffset);
			this.diagonalUpBorderOneHotZoneOnSetRowRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, leftMiddleCenterCorner.Y, leftHalfHorizontalCenterDrawPanel, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderTwoHotZoneOnSetRowRange = Rectangle.FromLTRB(leftHalfHorizontalCenterDrawPanel, topMiddleCenterCorner.Y + hotZoneOffset, topMiddleCenterCorner.X - hotZoneOffset, leftMiddleCenterCorner.Y);
			this.diagonalUpBorderThreeHotZoneOnSetRowRange = Rectangle.FromLTRB(topMiddleCenterCorner.X + hotZoneOffset, leftMiddleCenterCorner.Y, rightHalfHorizontalCenterDrawPanel, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderFourHotZoneOnSetRowRange = Rectangle.FromLTRB(rightHalfHorizontalCenterDrawPanel, topMiddleCenterCorner.Y + hotZoneOffset, rightMiddleCenterCorner.X - hotZoneOffset, leftMiddleCenterCorner.Y);
			this.diagonalDownBorderOneHotZoneOnSetRowRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, topMiddleCenterCorner.Y + hotZoneOffset, leftHalfHorizontalCenterDrawPanel, leftMiddleCenterCorner.Y);
			this.diagonalDownBorderTwoHotZoneOnSetRowRange = Rectangle.FromLTRB(leftHalfHorizontalCenterDrawPanel, leftMiddleCenterCorner.Y, topMiddleCenterCorner.X - hotZoneOffset, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalDownBorderThreeHotZoneOnSetRowRange = Rectangle.FromLTRB(topMiddleCenterCorner.X + hotZoneOffset, topMiddleCenterCorner.Y + hotZoneOffset, rightHalfHorizontalCenterDrawPanel, rightMiddleCenterCorner.Y);
			this.diagonalDownBorderFourHotZoneOnSetRowRange = Rectangle.FromLTRB(rightHalfHorizontalCenterDrawPanel, rightMiddleCenterCorner.Y, rightMiddleCenterCorner.X - hotZoneOffset, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderOneHotZoneOnSetColumnRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, topHalfVerticalCenterDrawPanel, topMiddleCenterCorner.X, leftMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderTwoHotZoneOnSetColumnRange = Rectangle.FromLTRB(topMiddleCenterCorner.X, topMiddleCenterCorner.Y + hotZoneOffset, rightMiddleCenterCorner.X - hotZoneOffset, topHalfVerticalCenterDrawPanel);
			this.diagonalUpBorderThreeHotZoneOnSetColumnRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, bottomHalfVerticalCenterDrawPanel, topMiddleCenterCorner.X, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderFourHotZoneOnSetColumnRange = Rectangle.FromLTRB(topMiddleCenterCorner.X, leftMiddleCenterCorner.Y + hotZoneOffset, rightMiddleCenterCorner.X - hotZoneOffset, bottomHalfVerticalCenterDrawPanel);
			this.diagonalDownBorderOneHotZoneOnSetColumnRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, topMiddleCenterCorner.Y + hotZoneOffset, topMiddleCenterCorner.X, topHalfVerticalCenterDrawPanel);
			this.diagonalDownBorderTwoHotZoneOnSetColumnRange = Rectangle.FromLTRB(topMiddleCenterCorner.X, topHalfVerticalCenterDrawPanel, rightMiddleCenterCorner.X - hotZoneOffset, leftMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalDownBorderThreeHotZoneOnSetColumnRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, leftMiddleCenterCorner.Y + hotZoneOffset, topMiddleCenterCorner.X, bottomHalfVerticalCenterDrawPanel);
			this.diagonalDownBorderFourHotZoneOnSetColumnRange = Rectangle.FromLTRB(topMiddleCenterCorner.X, bottomHalfVerticalCenterDrawPanel, rightMiddleCenterCorner.X - hotZoneOffset, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderOneHotZoneOnSetTableRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, topHalfVerticalCenterDrawPanel, leftHalfHorizontalCenterDrawPanel, leftMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderTwoHotZoneOnSetTableRange = Rectangle.FromLTRB(leftHalfHorizontalCenterDrawPanel, topMiddleCenterCorner.Y + hotZoneOffset, topMiddleCenterCorner.X - hotZoneOffset, topHalfVerticalCenterDrawPanel);
			this.diagonalUpBorderThreeHotZoneOnSetTableRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, bottomHalfVerticalCenterDrawPanel, leftHalfHorizontalCenterDrawPanel, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderFourHotZoneOnSetTableRange = Rectangle.FromLTRB(leftHalfHorizontalCenterDrawPanel, leftMiddleCenterCorner.Y + hotZoneOffset, topMiddleCenterCorner.X - hotZoneOffset, bottomHalfVerticalCenterDrawPanel);
			this.diagonalUpBorderFiveHotZoneOnSetTableRange = Rectangle.FromLTRB(topMiddleCenterCorner.X + hotZoneOffset, topHalfVerticalCenterDrawPanel, rightHalfHorizontalCenterDrawPanel, leftMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderSixHotZoneOnSetTableRange = Rectangle.FromLTRB(rightHalfHorizontalCenterDrawPanel, topMiddleCenterCorner.Y + hotZoneOffset, rightMiddleCenterCorner.X - hotZoneOffset, topHalfVerticalCenterDrawPanel);
			this.diagonalUpBorderSevenHotZoneOnSetTableRange = Rectangle.FromLTRB(topMiddleCenterCorner.X + hotZoneOffset, bottomHalfVerticalCenterDrawPanel, rightHalfHorizontalCenterDrawPanel, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalUpBorderEightHotZoneOnSetTableRange = Rectangle.FromLTRB(rightHalfHorizontalCenterDrawPanel, leftMiddleCenterCorner.Y + hotZoneOffset, rightMiddleCenterCorner.X - hotZoneOffset, bottomHalfVerticalCenterDrawPanel);
			this.diagonalDownBorderOneHotZoneOnSetTableRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, topMiddleCenterCorner.Y + hotZoneOffset, leftHalfHorizontalCenterDrawPanel, topHalfVerticalCenterDrawPanel);
			this.diagonalDownBorderTwoHotZoneOnSetTableRange = Rectangle.FromLTRB(leftHalfHorizontalCenterDrawPanel, topHalfVerticalCenterDrawPanel, topMiddleCenterCorner.X - hotZoneOffset, leftMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalDownBorderThreeHotZoneOnSetTableRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, leftMiddleCenterCorner.Y + hotZoneOffset, leftHalfHorizontalCenterDrawPanel, bottomHalfVerticalCenterDrawPanel);
			this.diagonalDownBorderFourHotZoneOnSetTableRange = Rectangle.FromLTRB(leftHalfHorizontalCenterDrawPanel, bottomHalfVerticalCenterDrawPanel, topMiddleCenterCorner.X - hotZoneOffset, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalDownBorderFiveHotZoneOnSetTableRange = Rectangle.FromLTRB(topMiddleCenterCorner.X + hotZoneOffset, topMiddleCenterCorner.Y + hotZoneOffset, rightHalfHorizontalCenterDrawPanel, topHalfVerticalCenterDrawPanel);
			this.diagonalDownBorderSixHotZoneOnSetTableRange = Rectangle.FromLTRB(rightHalfHorizontalCenterDrawPanel, topHalfVerticalCenterDrawPanel, rightMiddleCenterCorner.X - hotZoneOffset, leftMiddleCenterCorner.Y - hotZoneOffset);
			this.diagonalDownBorderSevenHotZoneOnSetTableRange = Rectangle.FromLTRB(topMiddleCenterCorner.X + hotZoneOffset, leftMiddleCenterCorner.Y + hotZoneOffset, rightHalfHorizontalCenterDrawPanel, bottomHalfVerticalCenterDrawPanel);
			this.diagonalDownBorderEightHotZoneOnSetTableRange = Rectangle.FromLTRB(rightHalfHorizontalCenterDrawPanel, bottomHalfVerticalCenterDrawPanel, rightMiddleCenterCorner.X - hotZoneOffset, bottomMiddleCenterCorner.Y - hotZoneOffset);
			this.insideHorizontalBorderLeftHotZoneOnSetTableRange = Rectangle.FromLTRB(leftMiddleCenterCorner.X + hotZoneOffset, leftMiddleCenterCorner.Y - hotZoneOffset, topMiddleCenterCorner.X - hotZoneOffset, leftMiddleCenterCorner.Y + hotZoneOffset);
			this.insideHorizontalBorderRightHotZoneOnSetTableRange = Rectangle.FromLTRB(topMiddleCenterCorner.X + hotZoneOffset, leftMiddleCenterCorner.Y - hotZoneOffset, rightMiddleCenterCorner.X - hotZoneOffset, leftMiddleCenterCorner.Y + hotZoneOffset);
		}
		protected internal void DrawCorners(Graphics graphics, SelectedRangeTypeForBorderPreview rangeType) {
			using (Pen pen = new Pen(cornerLineColor, cornerLineWidth)) {
				graphics.DrawLines(pen, topLeftCornerPoints);
				graphics.DrawLines(pen, topRightCornerPoints);
				graphics.DrawLines(pen, bottomLeftCornerPoints);
				graphics.DrawLines(pen, bottomRightCornerPoints);
				if (rangeType == SelectedRangeTypeForBorderPreview.Row || rangeType == SelectedRangeTypeForBorderPreview.Table) {
					graphics.DrawLines(pen, topMiddleCenterCornerPoints);
					graphics.DrawLines(pen, bottomMiddleCenterCornerPoints);
				}
				if (rangeType == SelectedRangeTypeForBorderPreview.Column || rangeType == SelectedRangeTypeForBorderPreview.Table) {
					graphics.DrawLines(pen, leftMiddleCenterCornerPoints);
					graphics.DrawLines(pen, rightMiddleCenterCornerPoints);
				}
			}
		}
		protected internal BorderLayout CalculateLeftBorderInfo(XlBorderLineStyle lineStyle) {
			return new BorderLayout(topLeftCenterCorner, bottomLeftCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateRightBorderInfo(XlBorderLineStyle lineStyle) {
			return new BorderLayout(topRightCenterCorner, bottomRightCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateTopBorderInfo(XlBorderLineStyle lineStyle) {
			return new BorderLayout(topLeftCenterCorner, topRightCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateBottomBorderInfo(XlBorderLineStyle lineStyle) {
			return new BorderLayout(bottomLeftCenterCorner, bottomRightCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateDiagonalDownBorderInfo(XlBorderLineStyle lineStyle) {
			return new BorderLayout(topLeftCenterCorner, bottomRightCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateDiagonalUpBorderInfo(XlBorderLineStyle lineStyle) {
			return new BorderLayout(bottomLeftCenterCorner, topRightCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateInsideVerticalBorderInfo(XlBorderLineStyle lineStyle) {
			return new BorderLayout(new Point(topMiddleCenterCorner.X, topLeftCenterCorner.Y), new Point(topMiddleCenterCorner.X, bottomLeftCenterCorner.Y), GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateInsideHorizontalBorderInfo(XlBorderLineStyle lineStyle) {
			return new BorderLayout(new Point(topLeftCenterCorner.X, leftMiddleCenterCorner.Y), new Point(topRightCenterCorner.X, leftMiddleCenterCorner.Y), GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateLeftDiagonalUpOnSetRowRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(bottomLeftCenterCorner, topMiddleCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateRightDiagonalUpOnSetRowRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(bottomMiddleCenterCorner, topRightCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateLeftDiagonalDownOnSetRowRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(topLeftCenterCorner, bottomMiddleCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateRightDiagonalDownOnSetRowRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(topMiddleCenterCorner, bottomRightCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateTopDiagonalUpOnSetColumnRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(leftMiddleCenterCorner, topRightCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateBottomDiagonalUpOnSetColumnRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(bottomLeftCenterCorner, rightMiddleCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateTopDiagonalDownOnSetColumnRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(topLeftCenterCorner, rightMiddleCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateBottomDiagonalDownOnSetColumnRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(leftMiddleCenterCorner, bottomRightCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateTopDiagonalUpOnSetTableRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(leftMiddleCenterCorner, topMiddleCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateBottomDiagonalUpOnSetTableRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(bottomMiddleCenterCorner, rightMiddleCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateTopDiagonalDownOnSetTableRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(topMiddleCenterCorner, rightMiddleCenterCorner, GetBorderWidth(lineStyle));
		}
		protected internal BorderLayout CalculateBottomDiagonalDownOnSetTableRange(XlBorderLineStyle lineStyle) {
			return new BorderLayout(leftMiddleCenterCorner, bottomMiddleCenterCorner, GetBorderWidth(lineStyle));
		}
		int GetBorderWidth(XlBorderLineStyle lineStyle) {
			return BorderInfo.LinePixelThicknessTable[lineStyle];
		}
		protected internal bool IsHitLeftBorderHotZone(Point point) {
			return LeftBorderHotZone.Contains(point);
		}
		protected internal bool IsHitRightBorderHotZone(Point point) {
			return RightBorderHotZone.Contains(point);
		}
		protected internal bool IsHitTopBorderHotZone(Point point) {
			return TopBorderHotZone.Contains(point);
		}
		protected internal bool IsHitBottomBorderHotZone(Point point) {
			return BottomBorderHotZone.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderTopHotZone(Point point) {
			return DiagonalDownBorderTopHotZone.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderBottomHotZone(Point point) {
			return DiagonalDownBorderBottomHotZone.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderTopHotZone(Point point) {
			return DiagonalUpBorderTopHotZone.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderBottomHotZone(Point point) {
			return DiagonalUpBorderBottomHotZone.Contains(point);
		}
		protected internal bool IsHitInsideVerticalBorderHotZone(Point point) {
			return InsideVerticalBorderHotZone.Contains(point);
		}
		protected internal bool IsHitInsideHorizontalBorderHotZone(Point point) {
			return InsideHorizontalBorderHotZone.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneOneOnSetRowRange(Point point) {
			return DiagonalUpBorderOneHotZoneOnSetRowRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneTwoOnSetRowRange(Point point) {
			return DiagonalUpBorderTwoHotZoneOnSetRowRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneThreeOnSetRowRange(Point point) {
			return DiagonalUpBorderThreeHotZoneOnSetRowRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneFourOnSetRowRange(Point point) {
			return DiagonalUpBorderFourHotZoneOnSetRowRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneOneOnSetRowRange(Point point) {
			return DiagonalDownBorderOneHotZoneOnSetRowRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneTwoOnSetRowRange(Point point) {
			return DiagonalDownBorderTwoHotZoneOnSetRowRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneThreeOnSetRowRange(Point point) {
			return DiagonalDownBorderThreeHotZoneOnSetRowRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneFourOnSetRowRange(Point point) {
			return DiagonalDownBorderFourHotZoneOnSetRowRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneOneOnSetColumnRange(Point point) {
			return DiagonalUpBorderOneHotZoneOnSetColumnRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneTwoOnSetColumnRange(Point point) {
			return DiagonalUpBorderTwoHotZoneOnSetColumnRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneThreeOnSetColumnRange(Point point) {
			return DiagonalUpBorderThreeHotZoneOnSetColumnRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneFourOnSetColumnRange(Point point) {
			return DiagonalUpBorderFourHotZoneOnSetColumnRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneOnSetColumnRange(Point point) {
			return DiagonalDownBorderOneHotZoneOnSetColumnRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneTwoOnSetColumnRange(Point point) {
			return DiagonalDownBorderTwoHotZoneOnSetColumnRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneThreeOnSetColumnRange(Point point) {
			return DiagonalDownBorderThreeHotZoneOnSetColumnRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneFourOnSetColumnRange(Point point) {
			return DiagonalDownBorderFourHotZoneOnSetColumnRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneOneOnSetTableRange(Point point) {
			return DiagonalUpBorderOneHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneTwoOnSetTableRange(Point point) {
			return DiagonalUpBorderTwoHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneThreeOnSetTableRange(Point point) {
			return DiagonalUpBorderThreeHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneFourOnSetTableRange(Point point) {
			return DiagonalUpBorderFourHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneFiveOnSetTableRange(Point point) {
			return DiagonalUpBorderFiveHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneSixOnSetTableRange(Point point) {
			return DiagonalUpBorderSixHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneSevenOnSetTableRange(Point point) {
			return DiagonalUpBorderSevenHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalUpBorderHotZoneEightOnSetTableRange(Point point) {
			return DiagonalUpBorderEightHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneOneOnSetTableRange(Point point) {
			return DiagonalDownBorderOneHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneTwoOnSetTableRange(Point point) {
			return DiagonalDownBorderTwoHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneThreeOnSetTableRange(Point point) {
			return DiagonalDownBorderThreeHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneFourOnSetTableRange(Point point) {
			return DiagonalDownBorderFourHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneFiveOnSetTableRange(Point point) {
			return DiagonalDownBorderFiveHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneSixOnSetTableRange(Point point) {
			return DiagonalDownBorderSixHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneSevenOnSetTableRange(Point point) {
			return DiagonalDownBorderSevenHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitDiagonalDownBorderHotZoneEightOnSetTableRange(Point point) {
			return DiagonalDownBorderEightHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitInsideHorizontalBorderLeftHotZoneOnSetTableRange(Point point) {
			return InsideHorizontalBorderLeftHotZoneOnSetTableRange.Contains(point);
		}
		protected internal bool IsHitInsideHorizontalBorderRightHotZoneOnSetTableRange(Point point) {
			return InsideHorizontalBorderRightHotZoneOnSetTableRange.Contains(point);
		}
	}
	#endregion
}
