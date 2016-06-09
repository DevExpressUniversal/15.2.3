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
using System.Data;
using System.Drawing;
using System.IO;
using DevExpress.Data.Browsing;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Wizards {
	public class ReportBuilder : ReportBuilderBase {
		public const string StyleNameTitle = "Title";
		public const string StyleNameFieldCaption = "FieldCaption";
		public const string StyleNamePageInfo = "PageInfo";
		public const string StyleNameDataField = "DataField";
		#region fields & properties
		ReportInfo reportInfo;
		int pageWidth;
		public ReportInfo ReportInfo {
			get { return reportInfo; }
			set { reportInfo = value; }
		}
		ObjectNameCollection Fields {
			get { return reportInfo.Fields; }
		}
		ObjectNameCollection SelectedFields {
			get { return reportInfo.SelectedFields; }
		}
		ObjectNameCollection UngroupingFields {
			get { return reportInfo.UngroupingFields; }
		}
		ObjectNameCollection OrderedSelectedFields {
			get { return reportInfo.OrderedSelectedFields; }
		}
		ObjectNameCollectionsSet GroupingFieldsSet {
			get { return reportInfo.GroupingFieldsSet; }
		}
		ArrayList SummaryFields {
			get { return reportInfo.SummaryFields; }
		}
		bool IgnoreNullValuesForSummary {
			get { return reportInfo.IgnoreNullValuesForSummary; }
		}
		PageOrientation Orientation {
			get { return reportInfo.Orientation; }
		}
		bool FitFieldsToPage {
			get { return reportInfo.FitFieldsToPage; }
		}
		ReportLayout Layout {
			get { return reportInfo.Layout; }
			set { reportInfo.Layout = value; }
		}
		ReportStyle Style {
			get { return reportInfo.Style; }
		}
		string ReportTitle {
			get { return reportInfo.ReportTitle; }
		}
		string ReportName {
			get { return Report.Name; }
		}
		int Spacing {
			get { return reportInfo.Spacing; }
		}
		int PageWidth {
			get { return pageWidth; }
			set { pageWidth = value; }
		}
		#endregion
		public ReportBuilder(XtraReport report, IComponentFactory componentFactory)
			: base(report, componentFactory) {
		}
		public ReportBuilder(XtraReport report, IComponentFactory componentFactory, ReportInfo reportInfo)
			: base(report, componentFactory) {
			this.reportInfo = reportInfo;
		}
		protected virtual String GetFormatString(string fieldName) {
			if(IsCurrencyField(fieldName))
				return "{0:C2}";
			return string.Empty;
		}
		public static bool FoundInGroupingFields(ObjectName field, ObjectNameCollectionsSet fieldsSet) {
			for(int i = 0; i < fieldsSet.Count; i++) {
				if(FoundInGroup(fieldsSet[i], field.Name))
					return true;
			}
			return false;
		}
		static bool FoundInGroup(ObjectNameCollection group, string fieldName) {
			for(int i = 0; i < group.Count; i++)
				if(group[i].Name == fieldName)
					return true;
			return false;
		}
		public ObjectNameCollection GetFieldsForSummary() {
			ObjectNameCollection numericalFields = new ObjectNameCollection();
			foreach(ObjectName f in SelectedFields) {
				if(!FoundInGroupingFields(f, GroupingFieldsSet) && (IsNumericalField(f.Name) || IsDateTimeField(f.Name)))
					numericalFields.Add(f);
			}
			return numericalFields;
		}
		SummaryField GetSummaryField(string fieldName) {
			foreach(SummaryField field in SummaryFields)
				if(field.Name == fieldName)
					return field;
			return null;
		}
		XRPictureBox CreatePictureBox(string bindingFieldName) {
			XRPictureBox control = (XRPictureBox)CreateComponent(typeof(XRPictureBox));
			control.Sizing = ImageSizeMode.ZoomImage;
			if(String.IsNullOrEmpty(bindingFieldName))
				return control;
			control.DataBindings.Add(new XRBinding("Image", Report.DataSource, bindingFieldName));
			return control;
		}
		void PrepareFields() {
			UngroupingFields.Clear();
			for(int i = 0; i < SelectedFields.Count; i++) {
				ObjectName f = SelectedFields[i];
				if(!FoundInGroupingFields(f, GroupingFieldsSet))
					UngroupingFields.Add(f);
			}
			OrderedSelectedFields.Clear();
			for(int i = 0; i < GroupingFieldsSet.Count; i++) {
				ObjectNameCollection group = GroupingFieldsSet[i];
				OrderedSelectedFields.AddRange(group);
			}
			OrderedSelectedFields.AddRange(UngroupingFields);
			ArrayList newSummaryFields = new ArrayList(UngroupingFields.Count);
			for(int i = 0; i < UngroupingFields.Count; i++) {
				string fieldName = UngroupingFields[i].Name;
				string displayName = UngroupingFields[i].DisplayName;
				SummaryField summary = GetSummaryField(fieldName);
				if(summary != null && summary.IsActive) {
					newSummaryFields.Add(summary);
					reportInfo.HasSummaries = true;
					if(summary.Sum)
						reportInfo.MakeGrandTotal = true;
				} else
					newSummaryFields.Add(new SummaryField(fieldName, displayName));
			}
			SummaryFields.Clear();
			SummaryFields.AddRange(newSummaryFields);
		}
		SizeF MeasureControl(XRControl control) {
			XRLabel label = control as XRLabel;
			if(label != null) {
				string text = label.Text;
				if(String.IsNullOrEmpty(text))
					text = "Wy";
				using(BrickStyle bs = new BrickStyle()) {					
					bs.StringFormat = BrickStringFormat.Create(label.GetEffectiveTextAlignment(), label.WordWrap, label.TextTrimming);
					bs.Font = label.GetEffectiveFont();
					bs.Padding = label.Padding;
					return label.MeasureText(text, PageWidth, bs);
				}
			} else
				return control.SizeF;
		}
		void DetermineReportLayout() {
			if(Layout == ReportLayout.Default)
				Layout = GroupingFieldsSet.Count > 0 ? ReportLayout.Stepped : ReportLayout.Columnar;
		}
		#region Creators
		XRLabel CreateLabel(string bindingFieldName) {
			XRLabel control = (XRLabel)CreateComponent(typeof(XRLabel));
			if(String.IsNullOrEmpty(bindingFieldName))
				return control;
			XRBinding binding = new XRBinding("Text", Report.DataSource, bindingFieldName);
			binding.FormatString = GetFormatString(bindingFieldName);
			control.DataBindings.Add(binding);
			return control;
		}
		XRLabel CreateLabel() {
			return CreateLabel(null);
		}
		XRTableCell CreateTableCell(string bindingFieldName) {
			XRTableCell control = (XRTableCell)CreateComponent(typeof(XRTableCell));
			if(String.IsNullOrEmpty(bindingFieldName))
				return control;
			control.DataBindings.Add(new XRBinding("Text", Report.DataSource, bindingFieldName));
			return control;
		}
		XRTableCell CreateTableCell() {
			return CreateTableCell(null);
		}
		XRCheckBox CreateCheckBox(string bindingFieldName) {
			XRCheckBox control = (XRCheckBox)CreateComponent(typeof(XRCheckBox));
			if(String.IsNullOrEmpty(bindingFieldName))
				return control;
			control.DataBindings.Add(new XRBinding("CheckState", Report.DataSource, bindingFieldName));
			control.Text = string.Empty;
			return control;
		}
		bool IsCurrencyField(string fieldName) {
			return typeof(System.Decimal).Equals(GetFieldType(fieldName));
		}
		bool IsNumericalField(string fieldName) {
			return PSNativeMethods.IsNumericalType(GetFieldType(fieldName));
		}
		bool IsDateTimeField(string fieldName) {
			return (typeof(System.DateTime).Equals(GetFieldType(fieldName)));
		}
		XRControl CreateBindedControl(string bindingFieldName) {
			try {
				Type type = GetFieldType(bindingFieldName);
				if(typeof(bool).Equals(type))
					return CreateCheckBox(bindingFieldName);
				if(typeof(Byte[]).Equals(type))
					return CreatePictureBox(bindingFieldName);
				return CreateLabel(bindingFieldName);
			} catch {
				return CreateLabel(bindingFieldName);
			}
		}
		XRPageInfo CreatePageInfo() {
			return (XRPageInfo)CreateComponent(typeof(XRPageInfo));
		}
		XRPageInfo CreateDateTimePageInfo() {
			XRPageInfo info = CreatePageInfo();
			info.PageInfo = PageInfo.DateTime;
			return info;
		}
		XRPageInfo CreateNumberOfTotalPageInfo() {
			XRPageInfo info = CreatePageInfo();
			info.PageInfo = PageInfo.NumberOfTotal;
			info.Format = DevExpress.XtraPrinting.Localization.PreviewLocalizer.GetString(DevExpress.XtraPrinting.Localization.PreviewStringId.SB_PageOfPages);
			return info;
		}
		XRLine CreateXRLine() {
			return (XRLine)CreateComponent(typeof(XRLine));
		}
		ArrayList CreateCaptionLabels(ObjectNameCollection fields, string styleName) {
			ArrayList result = new ArrayList();
			int count = fields.Count;
			for(int i = 0; i < count; i++) {
				XRLabel control = CreateLabel();
				control.Text = GetDisplayName(fields[i]);
				if(!string.IsNullOrEmpty(styleName))
					control.StyleName = styleName;
				result.Add(control);
			}
			return result;
		}
		static string GetDisplayName(ObjectName objectName) {
			return objectName.Name != objectName.DisplayName ? objectName.DisplayName :
				Native.NativeMethods.MakeFieldDisplayName(objectName.DisplayName);
		}
		ArrayList CreateBindedControls(ObjectNameCollection fields, string styleName) {
			ArrayList result = new ArrayList();
			int count = fields.Count;
			for(int i = 0; i < count; i++) {
				XRControl control = CreateBindedControl(GetBindingFieldName(fields[i].Name));
				if(!string.IsNullOrEmpty(styleName))
					control.StyleName = styleName;
				result.Add(control);
			}
			return result;
		}
		string GetBindingFieldName(string fieldName) {
			return String.IsNullOrEmpty(Report.DataMember) ? fieldName : string.Format("{0}.{1}", Report.DataMember, fieldName);
		}
		#endregion
		#region Calculators
		float CalcMaxControlWidth(ArrayList controls) {
			float maxWidth = 0;
			int count = controls.Count;
			for(int i = 0; i < count; i++) {
				XRControl control = (XRControl)controls[i];
				maxWidth = Math.Max(maxWidth, MeasureControl(control).Width);
			}
			return maxWidth;
		}
		float CalcMaxControlHeight(ArrayList controls) {
			float maxHeight = 0;
			int count = controls.Count;
			for(int i = 0; i < count; i++) {
				XRControl control = (XRControl)controls[i];
				maxHeight = Math.Max(maxHeight, MeasureControl(control).Height);
			}
			return maxHeight;
		}
		float CalcTotalControlsWidth(ArrayList controls) {
			float totalWidth = 0;
			int count = controls.Count;
			for(int i = 0; i < count; i++) {
				XRControl control = (XRControl)controls[i];
				totalWidth += MeasureControl(control).Width;
			}
			return totalWidth;
		}
		float CalcRatio(float pageWidth, float totalControlsWidth) {
			float ratio = 1.0f;
			if(FitFieldsToPage) {
				if(totalControlsWidth > 0)
					ratio = (pageWidth - 2 * Spacing) / totalControlsWidth;
			}
			return ratio;
		}
		#endregion
		protected int GetPrintablePageWidth() {
			return Report.PageWidth - Report.Margins.Left - Report.Margins.Right;
		}
		XRLine MakeHorzLine(float x, float y, float width, Band band) {
			XRLine line = CreateXRLine();
			band.Controls.Add(line);
			line.HeightF = 0;
			line.WidthF = width;
			line.LocationF = new PointF(x, y);
			return line;
		}
		void MakeReportHeader(bool topLine, bool bottomLine) {
			ReportHeaderBand header = GetBandByType(typeof(ReportHeaderBand)) as ReportHeaderBand;
			XRLabel headerLabel = CreateLabel();
			header.Controls.Add(headerLabel);
			headerLabel.Text = ReportTitle;
			headerLabel.StyleName = StyleNameTitle;
			headerLabel.LocationF = new Point(Spacing, Spacing);
			SizeF size = MeasureControl(headerLabel);
			headerLabel.WidthF = PageWidth - 2 * Spacing;
			headerLabel.HeightF = size.Height;
			header.HeightF = header.HeightF + 2 * Spacing;
			if(topLine) {
				XRLine line = MakeHorzLine(Spacing, 0, PageWidth - 2 * Spacing, header);
				headerLabel.TopF = line.BottomF;
			}
			if(bottomLine) {
				XRLine line = MakeHorzLine(Spacing, header.HeightF - Spacing, PageWidth - 2 * Spacing, header);
				line.TopF = headerLabel.BottomF;
			}
		}
		void MakePageInfo(bool appendLine) {
			PageFooterBand footer = GetBandByType(typeof(PageFooterBand)) as PageFooterBand;
			float yOffset = 0;
			if(appendLine) {
				XRLine line = MakeHorzLine(Spacing, 0, PageWidth - 2 * Spacing, footer);
				yOffset = line.BottomF;
			}
			XRPageInfo info = CreateDateTimePageInfo();
			footer.Controls.Add(info);
			info.LocationF = new PointF(Spacing, Spacing + yOffset);
			info.WidthF = PageWidth / 2 - 2 * Spacing;
			info.StyleName = StyleNamePageInfo;
			info = CreateNumberOfTotalPageInfo();
			footer.Controls.Add(info);
			info.LocationF = new PointF(PageWidth / 2 + Spacing, Spacing + yOffset);
			info.WidthF = PageWidth / 2 - 2 * Spacing;
			info.TextAlignment = TextAlignment.TopRight;
			info.StyleName = StyleNamePageInfo;
		}
		void MakeColumnarReport() {
			DetailBand detail = GetBandByType(typeof(DetailBand)) as DetailBand;
			ArrayList captionLabels = CreateCaptionLabels(SelectedFields, ReportBuilder.StyleNameFieldCaption);
			ArrayList bindedControls = CreateBindedControls(SelectedFields, ReportBuilder.StyleNameDataField);
			detail.Controls.AddRange((XRControl[])captionLabels.ToArray(typeof(XRControl)));
			detail.Controls.AddRange((XRControl[])bindedControls.ToArray(typeof(XRControl)));
			float captionLabelWidth = CalcMaxControlWidth(captionLabels);
			if(captionLabelWidth < PageWidth / 4)
				captionLabelWidth = PageWidth / 4;
			float bindedControlWidth = PageWidth - captionLabelWidth - 3 * Spacing;
			float y = 3 * Spacing / 2;
			for(int i = 0; i < SelectedFields.Count; i++) {
				XRLabel caption = (XRLabel)captionLabels[i];
				XRControl dataControl = (XRControl)bindedControls[i];
				float labelHeight = Math.Max(MeasureControl(caption).Height, MeasureControl(dataControl).Height);
				caption.LocationF = new PointF(Spacing, y);
				caption.WidthF = captionLabelWidth;
				caption.HeightF = labelHeight;
				dataControl.LocationF = new PointF(captionLabelWidth + 2 * Spacing, y);
				dataControl.WidthF = bindedControlWidth;
				dataControl.HeightF = labelHeight;
				y += Spacing + labelHeight;
			}
			foreach(XRControl item in bindedControls)
				if(item is XRPictureBox)
					((XRPictureBox)item).Sizing = ImageSizeMode.AutoSize;
			MakeHorzLine(Spacing, Spacing / 2, PageWidth - 2 * Spacing, detail);
			MakePageInfo(false);
			MakeReportHeader(false, false);
		}
		void LayoutControlsHorizontally(float xOffset, float y, float height, float ratio, ArrayList controls, ArrayList captions, int captionsStartIndex) {
			float x = xOffset;
			int count = controls.Count;
			for(int i = 0; i < count; i++) {
				XRControl control = (XRControl)controls[i];
				XRControl caption = (captions != null) ? (XRControl)captions[i + captionsStartIndex] : null;
				float width;
				if(caption != null)
					width = caption.WidthF;
				else
					width = MeasureControl(control).Width * ratio;
				control.WidthF = width;
				control.HeightF = height;
				if(captions != null)
					control.LocationF = new PointF(caption.LocationF.X, y);
				else
					control.LocationF = new PointF(x, y);
				if(i + 1 == count)
					control.WidthF += (PageWidth - Spacing - control.BoundsF.Right);
				x = control.RightF;
			}
		}
		void LayoutControlsHorizontally(float xOffset, float y, float height, float ratio, ArrayList controls) {
			LayoutControlsHorizontally(xOffset, y, height, ratio, controls, null, 0);
		}
		void LayoutControlsHorizontally(float y, float height, ArrayList controls, ArrayList captions, int captionsStartIndex) {
			LayoutControlsHorizontally(0, y, height, 1.0f, controls, captions, captionsStartIndex);
		}
		void MakeTabularReportByTable() {
			Report.StyleSheet[StyleNameFieldCaption].Borders = BorderSide.Bottom;
			DetailBand detail = GetBandByType(typeof(DetailBand)) as DetailBand;
			PageHeaderBand pageHeader = GetBandByType(typeof(PageHeaderBand)) as PageHeaderBand;
			XRTable headerTable = (XRTable)CreateComponent(typeof(XRTable));
			XRTable detailTable = (XRTable)CreateComponent(typeof(XRTable));
			headerTable.SuspendLayout();
			detailTable.SuspendLayout();
			XRTableRow headerRow = (XRTableRow)CreateComponent(typeof(XRTableRow));
			XRTableRow detailRow = (XRTableRow)CreateComponent(typeof(XRTableRow));
			headerTable.Rows.Clear();
			headerTable.Rows.Add(headerRow);
			detailTable.Rows.Clear();
			detailTable.Rows.Add(detailRow);
			headerTable.LocationF = new Point(Spacing, Spacing);
			headerTable.AnchorVertical = VerticalAnchorStyles.Bottom;
			detailTable.LocationF = new Point(Spacing, 0);
			detailTable.AnchorVertical = VerticalAnchorStyles.Both;
			pageHeader.Controls.Add(headerTable);
			detail.Controls.Add(detailTable);
			headerTable.UpdatedBounds();
			detailTable.UpdatedBounds();
			XRTableCell[] captionCells = new XRTableCell[SelectedFields.Count];
			ArrayList bindedControls = new ArrayList();
			SizeF[] sizes = new SizeF[SelectedFields.Count];
			float totalCaptionsWidth = 0;
			float maxCaptionHeight = -1;
			float maxDetailHeight = -1;
			for(int i = 0; i < SelectedFields.Count; i++) {
				captionCells[i] = CreateTableCell();
				captionCells[i].Text = GetDisplayName(SelectedFields[i]);
				captionCells[i].StyleName = StyleNameFieldCaption;
				captionCells[i].TextAlignment = TextAlignment.MiddleLeft;
				XRControl dataControl = CreateTableCell(GetBindingFieldName(SelectedFields[i].Name));
				bindedControls.Add(dataControl);
				dataControl.StyleName = StyleNameDataField;
				headerRow.Cells.Add(captionCells[i]);
				detailRow.Cells.Add(dataControl);
				sizes[i] = MeasureControl(captionCells[i]);
				totalCaptionsWidth += sizes[i].Width;
				maxCaptionHeight = Math.Max(maxCaptionHeight, sizes[i].Height);
				maxDetailHeight = Math.Max(maxDetailHeight, MeasureControl(dataControl).Height);
			}
			float ratio = CalcRatio(PageWidth, totalCaptionsWidth);
			detail.HeightF = maxDetailHeight;
			headerTable.WidthF = (int)(totalCaptionsWidth * ratio);
			detailTable.WidthF = headerTable.WidthF;
			headerRow.WidthF = headerTable.WidthF;
			detailRow.WidthF = headerTable.WidthF;
			headerTable.HeightF = 2 * maxCaptionHeight;
			detailTable.HeightF = detail.HeightF;
			headerRow.HeightF = headerTable.HeightF;
			detailRow.HeightF = detailTable.HeightF;
			for(int i = 0; i < SelectedFields.Count; i++) {
				int width = (int)(sizes[i].Width * ratio);
				captionCells[i].WidthF = width;
				captionCells[i].HeightF = headerTable.HeightF;
				XRControl dataControl = (XRControl)bindedControls[i];
				dataControl.WidthF = width;
				dataControl.HeightF = detailTable.HeightF;
			}
			pageHeader.HeightF = 2 * maxCaptionHeight + Spacing;
			headerTable.ResumeLayout();
			detailTable.ResumeLayout();
			headerTable.PerformLayout();
			detailTable.PerformLayout();
			MakePageInfo(false);
			MakeReportHeader(false, false);
		}
		void MakeTabularReportByLabels() {
			DetailBand detail = GetBandByType(typeof(DetailBand)) as DetailBand;
			detail.StyleName = ReportBuilder.StyleNameDataField;
			PageHeaderBand pageHeader = GetBandByType(typeof(PageHeaderBand)) as PageHeaderBand;
			ArrayList captionLabels = CreateCaptionLabels(SelectedFields, ReportBuilder.StyleNameFieldCaption);
			ArrayList bindedControls = CreateBindedControls(SelectedFields, string.Empty);
			pageHeader.Controls.AddRange((XRControl[])captionLabels.ToArray(typeof(XRControl)));
			detail.Controls.AddRange((XRControl[])bindedControls.ToArray(typeof(XRControl)));
			float totalCaptionsWidth = CalcTotalControlsWidth(captionLabels);
			float maxCaptionHeight = CalcMaxControlHeight(captionLabels);
			float maxDetailHeight = CalcMaxControlHeight(bindedControls);
			float ratio = CalcRatio(PageWidth, totalCaptionsWidth);
			LayoutControlsHorizontally(Spacing, Spacing, maxCaptionHeight * 2, ratio, captionLabels);
			LayoutControlsHorizontally(0, maxDetailHeight, bindedControls, captionLabels, 0);
			detail.HeightF = maxDetailHeight;
			pageHeader.HeightF = maxCaptionHeight + Spacing;
			MakePageInfo(false);
			MakeReportHeader(false, false);
			MakeHorzLine(Spacing, pageHeader.HeightF, PageWidth - 2 * Spacing, pageHeader);
		}
		bool AreThereBinaryFields(ObjectNameCollection fields) {
			int count = fields.Count;
			for(int i = 0; i < count; i++) {
				try {
					if(typeof(Byte[]).Equals(GetFieldType(fields[i].Name)))
						return true;
				} catch {
				}
			}
			return false;
		}
		void MakeTabularReport() {
			if(AreThereBinaryFields(SelectedFields))
				MakeTabularReportByLabels();
			else
				MakeTabularReportByTable();
		}
		void MakeJustifiedReport() {
			DetailBand detail = GetBandByType(typeof(DetailBand)) as DetailBand;
			detail.StyleName = StyleNameDataField;
			ArrayList captionLabels = CreateCaptionLabels(SelectedFields, StyleNameFieldCaption);
			ArrayList bindedControls = CreateBindedControls(SelectedFields, string.Empty);
			detail.Controls.AddRange((XRControl[])captionLabels.ToArray(typeof(XRControl)));
			detail.Controls.AddRange((XRControl[])bindedControls.ToArray(typeof(XRControl)));
			ArrayList rows = new ArrayList();
			JustifiedReportRow currentRow = new JustifiedReportRow();
			int count = captionLabels.Count;
			for(int i = 0; i < count; i++) {
				float width = 0;
				for(int index = i; index < count; i++, index++) {
					XRLabel caption = (XRLabel)captionLabels[index];
					XRControl dataControl = (XRControl)bindedControls[index];
					XRPictureBox picBox = dataControl as XRPictureBox;
					if(picBox != null) {
						caption.WidthF = PageWidth - 2 * Spacing;
						dataControl.WidthF = caption.WidthF;
					} else
						caption.WidthF = MeasureControl(caption).Width;
					dataControl.WidthF = caption.WidthF;
					if(caption.WidthF + width > PageWidth - 2 * Spacing) {
						rows.Add(currentRow);
						currentRow = new JustifiedReportRow();
						currentRow.Captions.Add(caption);
						currentRow.DataControls.Add(dataControl);
						break;
					} else
						width += caption.WidthF;
					currentRow.Captions.Add(caption);
					currentRow.DataControls.Add(dataControl);
				}
			}
			rows.Add(currentRow);
			count = rows.Count;
			XRLine line = MakeHorzLine(Spacing, Spacing / 2, PageWidth - 2 * Spacing, detail);
			float y = Spacing + line.BottomF;
			for(int i = 0; i < count; i++) {
				JustifiedReportRow row = (JustifiedReportRow)rows[i];
				float totalControlsWidth = CalcTotalControlsWidth(row.Captions);
				float ratio = CalcRatio(PageWidth, totalControlsWidth);
				float maxCaptionsHeight = CalcMaxControlHeight(row.Captions);
				float maxControlsHeight = CalcMaxControlHeight(row.DataControls);
				LayoutControlsHorizontally(Spacing, y, maxCaptionsHeight, ratio, row.Captions);
				y += maxCaptionsHeight;
				LayoutControlsHorizontally(y, maxControlsHeight, row.DataControls, row.Captions, 0);
				y += maxControlsHeight;
			}
			detail.HeightF = y + Spacing;
			MakeHorzLine(Spacing, detail.HeightF - Spacing / 2, PageWidth - 2 * Spacing, detail);
			MakePageInfo(false);
			MakeReportHeader(false, false);
		}
		XRLabel CreateSummaryLabel(SummaryFunc func, SummaryRunning running, string summaryFieldName) {
			XRLabel label = CreateLabel(GetBindingFieldName(summaryFieldName));
			label.StyleName = StyleNameFieldCaption;
			label.Summary.Running = running;
			label.Summary.Func = func;
			label.Summary.IgnoreNullValues = IgnoreNullValuesForSummary;
			label.Summary.FormatString = GetFormatString(summaryFieldName);
			return label;
		}
		float MakeSummaries(Band band, ArrayList captions, int captionsStartIndex, float xOffset, float y, SummaryFunc func, SummaryRunning running, string rowCaption) {
			if(rowCaption == null)
				rowCaption = func.ToString();
			ArrayList controls = new ArrayList();
			int captionIndex = captionsStartIndex;
			for(int i = 0; i < SummaryFields.Count; i++, captionIndex++) {
				SummaryField summary = (SummaryField)SummaryFields[i];
				if(ShouldCreateSummary(func, summary)) {
					XRLabel label = CreateSummaryLabel(func, running, summary.DisplayName);
					controls.Add(label);
					band.Controls.Add(label);
					XRLabel caption = (XRLabel)captions[captionIndex];
					label.LocationF = new PointF(caption.LocationF.X, y);
					label.WidthF = caption.WidthF;
				}
			}
			if(controls.Count <= 0)
				return 0;
			float maxHeight = CalcMaxControlHeight(controls);
			for(int i = 0; i < controls.Count; i++)
				((XRControl)controls[i]).HeightF = maxHeight;
			XRLabel rowCaptionLabel = CreateLabel();
			rowCaptionLabel.Dpi = Report.Dpi;
			rowCaptionLabel.LocationF = new PointF(xOffset, y);
			rowCaptionLabel.HeightF = maxHeight;
			rowCaptionLabel.WidthF = ((XRLabel)captions[captionsStartIndex]).LocationF.X - xOffset;
			rowCaptionLabel.Text = rowCaption;
			rowCaptionLabel.StyleName = StyleNameFieldCaption;
			band.Controls.Add(rowCaptionLabel);
			if(rowCaptionLabel.WidthF <= XRControl.GetMinimumWidth(rowCaptionLabel.Dpi))
				rowCaptionLabel.WidthF = MeasureControl(rowCaptionLabel).Width;
			return maxHeight + Spacing;
		}
		static bool ShouldCreateSummary(SummaryFunc func, SummaryField field) {
			switch(func) {
				case SummaryFunc.Sum:
					return field.Sum;
				case SummaryFunc.Avg:
					return field.Avg;
				case SummaryFunc.Min:
					return field.Min;
				case SummaryFunc.Max:
					return field.Max;
				case SummaryFunc.Count:
					return field.Count;
				default:
					return false;
			}
		}
		float MakeGroupSummaries(Band band, ArrayList captions, int captionsStartIndex, float xOffset, float y, SummaryFunc func) {
			return MakeSummaries(band, captions, captionsStartIndex, xOffset, y, func, SummaryRunning.Group, null);
		}
		void MakeGroupSummaries(Band band, ArrayList captions, int captionsStartIndex, float xOffset) {
			SummaryFunc[] sumFuncs = new SummaryFunc[] { SummaryFunc.Sum, SummaryFunc.Avg, SummaryFunc.Min, SummaryFunc.Max, SummaryFunc.Count };
			float y = Spacing;
			foreach(SummaryFunc func in sumFuncs)
				y += MakeGroupSummaries(band, captions, captionsStartIndex, xOffset, y, func);
			band.HeightF = y;
		}
		void MakeReportSummary(ArrayList captions, int captionsStartIndex) {
			if(!reportInfo.MakeGrandTotal)
				return;
			ReportFooterBand footer = GetBandByType(typeof(ReportFooterBand)) as ReportFooterBand;
			float height = MakeSummaries(footer, captions, captionsStartIndex, Spacing, Spacing, SummaryFunc.Sum, SummaryRunning.Report, "Grand Total");
			footer.HeightF = height + Spacing;
		}
		void MakeSteppedReport() {
			DetailBand detail = GetBandByType(typeof(DetailBand)) as DetailBand;
			detail.StyleName = StyleNameDataField;
			PageHeaderBand pageHeader = GetBandByType(typeof(PageHeaderBand)) as PageHeaderBand;
			ArrayList captionLabels = CreateCaptionLabels(OrderedSelectedFields, StyleNameFieldCaption);
			pageHeader.Controls.AddRange((XRControl[])captionLabels.ToArray(typeof(XRControl)));
			float totalCaptionsWidth = CalcTotalControlsWidth(captionLabels);
			float ratio = CalcRatio(PageWidth, totalCaptionsWidth);
			float maxCaptionHeight = CalcMaxControlHeight(captionLabels);
			XRLine line = MakeHorzLine(Spacing, Spacing - 1, PageWidth - 2 * Spacing, pageHeader);
			LayoutControlsHorizontally(Spacing, line.BottomF, maxCaptionHeight * 2, ratio, captionLabels);
			pageHeader.HeightF = maxCaptionHeight + Spacing;
			line = MakeHorzLine(Spacing, pageHeader.HeightF, PageWidth - 2 * Spacing, pageHeader);
			int firstUngroupedIndex = 0;
			for(int groupIndex = 0; groupIndex < GroupingFieldsSet.Count; groupIndex++) {
				ObjectNameCollection group = GroupingFieldsSet[groupIndex];
				firstUngroupedIndex += group.Count;
			}
			int fieldIndex = 0;
			for(int groupIndex = 0; groupIndex < GroupingFieldsSet.Count; groupIndex++) {
				GroupHeaderBand groupHeader = (GroupHeaderBand)GetBandByType(typeof(GroupHeaderBand));
				groupHeader.StyleName = StyleNameDataField;
				ObjectNameCollection group = GroupingFieldsSet[groupIndex];
				ArrayList groupControls = CreateBindedControls(group, string.Empty);
				groupHeader.Controls.AddRange((XRControl[])groupControls.ToArray(typeof(XRControl)));
				float maxHeight = CalcMaxControlHeight(groupControls);
				if(reportInfo.HasSummaries) {
					GroupFooterBand groupFooter = (GroupFooterBand)GetBandByType(typeof(GroupFooterBand));
					float xOffset = ((XRLabel)captionLabels[fieldIndex]).LocationF.X;
					MakeGroupSummaries(groupFooter, captionLabels, firstUngroupedIndex, xOffset);
				}
				int groupControlsCount = groupControls.Count;
				for(int i = 0; i < groupControlsCount; i++, fieldIndex++) {
					XRLabel captionLabel = (XRLabel)captionLabels[fieldIndex];
					XRControl control = (XRControl)groupControls[i];
					control.WidthF = captionLabel.WidthF;
					control.LocationF = new PointF(captionLabel.LeftF, 0);
					groupHeader.GroupFields.Add(new GroupField(group[i].Name));
				}
				groupHeader.HeightF = maxHeight;
				groupHeader.Level = 0;
			}
			ArrayList bindedControls = CreateBindedControls(UngroupingFields, string.Empty);
			detail.Controls.AddRange((XRControl[])bindedControls.ToArray(typeof(XRControl)));
			float maxDetailHeight = CalcMaxControlHeight(bindedControls);
			LayoutControlsHorizontally(0, maxDetailHeight, bindedControls, captionLabels, fieldIndex);
			detail.HeightF = maxDetailHeight;
			MakePageInfo(false);
			MakeReportHeader(true, false);
			MakeReportSummary(captionLabels, firstUngroupedIndex);
		}
		void MakeOutlineReportCommon(int multiplier, bool alternativeLayout) {
			DetailBand detail = GetBandByType(typeof(DetailBand)) as DetailBand;
			detail.StyleName = StyleNameDataField;
			float xOffset = 5 * Spacing * multiplier;
			for(int groupIndex = 0; groupIndex < GroupingFieldsSet.Count; groupIndex++) {
				float offset = Spacing + groupIndex * xOffset;
				GroupHeaderBand groupHeader = (GroupHeaderBand)GetBandByType(typeof(GroupHeaderBand));
				ObjectNameCollection group = GroupingFieldsSet[groupIndex];
				ArrayList groupCaptions = CreateCaptionLabels(group, StyleNameFieldCaption);
				ArrayList groupControls = CreateBindedControls(group, StyleNameDataField);
				groupHeader.Controls.AddRange((XRControl[])groupControls.ToArray(typeof(XRControl)));
				groupHeader.Controls.AddRange((XRControl[])groupCaptions.ToArray(typeof(XRControl)));
				float maxHeight = 2 * Math.Max(CalcMaxControlHeight(groupControls), CalcMaxControlHeight(groupCaptions));
				XRLine line = null;
				float lineBottom = 0;
				if(alternativeLayout) {
					line = MakeHorzLine(offset, 0, 0, groupHeader);
					lineBottom = line.BottomF;
				}
				float x = offset;
				for(int i = 0; i < groupControls.Count; i++) {
					XRLabel captionLabel = (XRLabel)groupCaptions[i];
					XRControl control = (XRControl)groupControls[i];
					captionLabel.HeightF = maxHeight;
					captionLabel.WidthF = MeasureControl(captionLabel).Width;
					captionLabel.LocationF = new PointF(x, lineBottom);
					x += captionLabel.WidthF;
					control.HeightF = maxHeight;
					control.WidthF = captionLabel.WidthF;
					control.LocationF = new PointF(x, lineBottom);
					x += control.WidthF + Spacing;
					groupHeader.GroupFields.Add(new GroupField(group[i].Name));
				}
				if(alternativeLayout && line != null) {
					line.LeftF = offset;
					line.WidthF = x - offset;
				}
				groupHeader.HeightF = maxHeight;
				groupHeader.Level = 0;
				FitControlsToPageWidth(groupHeader);
			}
			GroupHeaderBand detailHeader = GetBandByType(typeof(GroupHeaderBand)) as GroupHeaderBand;
			detailHeader.StyleName = StyleNameFieldCaption;
			detailHeader.Level = 0;
			ArrayList captionLabels = CreateCaptionLabels(UngroupingFields, string.Empty);
			detailHeader.Controls.AddRange((XRControl[])captionLabels.ToArray(typeof(XRControl)));
			float totalCaptionWidth = CalcTotalControlsWidth(captionLabels);
			float ratio = CalcRatio(PageWidth - GroupingFieldsSet.Count * xOffset, totalCaptionWidth);
			float maxCaptionHeight = CalcMaxControlHeight(captionLabels);
			float lineWidth = PageWidth - 2 * Spacing - GroupingFieldsSet.Count * xOffset;
			float yOffset = Spacing;
			if(!alternativeLayout) {
				XRLine line = MakeHorzLine(Spacing + GroupingFieldsSet.Count * xOffset, Spacing - 1, lineWidth, detailHeader);
				yOffset = line.BottomF;
			}
			LayoutControlsHorizontally(Spacing + GroupingFieldsSet.Count * xOffset, yOffset, maxCaptionHeight, ratio, captionLabels);
			detailHeader.HeightF = Spacing + maxCaptionHeight;
			if(!alternativeLayout)
				MakeHorzLine(Spacing + GroupingFieldsSet.Count * xOffset, detailHeader.HeightF, lineWidth, detailHeader);
			ArrayList bindedControls = CreateBindedControls(UngroupingFields, string.Empty);
			detail.Controls.AddRange((XRControl[])bindedControls.ToArray(typeof(XRControl)));
			float maxDetailHeight = CalcMaxControlHeight(bindedControls);
			LayoutControlsHorizontally(0, maxDetailHeight, bindedControls, captionLabels, 0);
			detail.HeightF = maxDetailHeight;
			MakePageInfo(alternativeLayout);
			MakeReportHeader(alternativeLayout, alternativeLayout);
			if(reportInfo.HasSummaries) {
				GroupFooterBand fakeDetailFooter = GetBandByType(typeof(GroupFooterBand)) as GroupFooterBand;
				for(int groupIndex = 0; groupIndex < GroupingFieldsSet.Count; groupIndex++) {
					float offset = Spacing + (GroupingFieldsSet.Count - groupIndex - 1) * xOffset;
					GroupFooterBand groupFooter = (GroupFooterBand)GetBandByType(typeof(GroupFooterBand));
					MakeGroupSummaries(groupFooter, captionLabels, 0, offset);
				}
				Report.Bands.Remove(fakeDetailFooter);
				MakeReportSummary(captionLabels, 0);
			}
		}
		void FitControlsToPageWidth(Band band) {
			float rightBound = 0;
			foreach(XRControl control in band.Controls) {
				rightBound = Math.Max(rightBound, control.RightF);
			}
			float ratio = CalcRatio(PageWidth, rightBound);
			if(ratio >= 1)
				return;
			foreach(XRControl control in band.Controls) {
				control.LeftF = control.LeftF * ratio;
				control.WidthF = control.WidthF * ratio;
			}
		}
		void MakeOutlineReport1() {
			MakeOutlineReportCommon(1, false);
		}
		void MakeOutlineReport2() {
			MakeOutlineReportCommon(1, true);
		}
		void MakeLeftAlignedReport1() {
			MakeOutlineReportCommon(0, false);
		}
		void MakeLeftAlignedReport2() {
			MakeOutlineReportCommon(0, true);
		}
		protected override void ExecuteCore() {
			if(SelectedFields.Count == 0)
				return;
			DetermineReportLayout();
			PrepareFields();
			ReportStyle.LoadStyleSheetFromResources(Report.StyleSheet, Style);
			Report.Landscape = object.Equals(Orientation, PageOrientation.Landscape);
			this.PageWidth = GetPrintablePageWidth();
			switch(Layout) {
				case ReportLayout.Columnar:
					MakeColumnarReport();
					break;
				case ReportLayout.Tabular:
					MakeTabularReport();
					break;
				case ReportLayout.Justified:
					MakeJustifiedReport();
					break;
				case ReportLayout.Stepped:
					MakeSteppedReport();
					break;
				case ReportLayout.Block:
					break;
				case ReportLayout.Outline1:
					MakeOutlineReport1();
					break;
				case ReportLayout.Outline2:
					MakeOutlineReport2();
					break;
				case ReportLayout.AlignLeft1:
					MakeLeftAlignedReport1();
					break;
				case ReportLayout.AlignLeft2:
					MakeLeftAlignedReport2();
					break;
			}
			DetailBand detailBand = Report.Bands[BandKind.Detail] as DetailBand;
			if(detailBand != null) detailBand.MultiColumn.Mode = MultiColumnMode.None;
		}
		protected string ExtractFieldName(string fieldName) {
			return fieldName.StartsWith(Report.DataMember + ".") ? fieldName.Substring(Report.DataMember.Length + 1) : fieldName;
		}
		protected Type GetFieldType(string fieldName) {
			object dataSource = Report.GetEffectiveDataSource();
			if(dataSource is DataSet) {
				DataSet dataSet = (DataSet)dataSource;
				return dataSet.Tables.Count > 0 ? GetFieldType(dataSet.Tables[0], ExtractFieldName(fieldName)) : null;
			}
			using(DataContext dataContext = new DataContext(true)) {
				string name = ExtractFieldName(fieldName);
				return dataContext.GetPropertyType(dataSource, String.Concat(Report.DataMember, ".", name));
			}
		}
		static Type GetFieldType(DataTable table, string name) {
			foreach(DataColumn item in table.Columns)
				if(item.ColumnName == name)
					return item.DataType;
			return null;
		}
	}
	public class ReportInfo {
		ObjectNameCollection selectedFields = new ObjectNameCollection();
		ObjectNameCollection fields = new ObjectNameCollection();
		ObjectNameCollectionsSet groupingFieldsSet = new ObjectNameCollectionsSet();
		ObjectNameCollection ungroupingFields = new ObjectNameCollection();
		ObjectNameCollection orderedSelectedFields = new ObjectNameCollection();
		ArrayList summaryFields = new ArrayList();
		bool ignoreNullValuesForSummary;
		PageOrientation orientation = PageOrientation.Portrait;
		bool fitFieldsToPage = true;
		ReportLayout layout = ReportLayout.Default;
		ReportStyle style;
		string reportTitle = String.Empty;
		bool hasSummaries;
		bool makeGrandTotal;
		int spacing;
		public int Spacing {
			get { return spacing; }
			set { spacing = value; }
		}
		public bool MakeGrandTotal {
			get { return makeGrandTotal; }
			set { makeGrandTotal = value; }
		}
		public bool HasSummaries {
			get { return hasSummaries; }
			set { hasSummaries = value; }
		}
		public string ReportTitle {
			get { return reportTitle; }
			set { reportTitle = value; }
		}
		public ReportStyle Style {
			get { return style; }
			set { style = value; }
		}
		public ReportLayout Layout {
			get { return layout; }
			set { layout = value; }
		}
		public bool FitFieldsToPage {
			get { return fitFieldsToPage; }
			set { fitFieldsToPage = value; }
		}
		public PageOrientation Orientation {
			get { return orientation; }
			set { orientation = value; }
		}
		public bool IgnoreNullValuesForSummary {
			get { return ignoreNullValuesForSummary; }
			set { ignoreNullValuesForSummary = value; }
		}
		public ArrayList SummaryFields {
			get { return summaryFields; }
		}
		public ObjectNameCollection OrderedSelectedFields {
			get { return orderedSelectedFields; }
		}
		public ObjectNameCollection UngroupingFields {
			get { return ungroupingFields; }
		}
		public ObjectNameCollectionsSet GroupingFieldsSet {
			get { return groupingFieldsSet; }
		}
		public ObjectNameCollection Fields {
			get { return fields; }
		}
		public ObjectNameCollection SelectedFields {
			get { return selectedFields; }
		}
	}
	public class SummaryField : ObjectName {
		public bool Sum;
		public bool Avg;
		public bool Max;
		public bool Min;
		public bool Count;
		public bool IsActive {
			get { return Sum || Avg || Max || Min || Count; }
		}
		public SummaryField(string name, string displayName) : base(name, displayName) { }
	}
	public class JustifiedReportRow {
		public ArrayList Captions = new ArrayList();
		public ArrayList DataControls = new ArrayList();
	}
	public class ReportStyle {
		public static void LoadStyleSheetFromResources(XRControlStyleSheet styleSheet, ReportStyle style) {
			Stream stream = ResourceStreamHelper.GetStream(style.resourceName, style.type);
			if(stream != null) {
				try {
					styleSheet.LoadFromStream(stream);
				} finally {
					stream.Close();
				}
			}
		}
		#region fields & properties
		Type type;
		string resourceName;
		string name;
		XRControlStyleSheet styleSheet;
		public XRControlStyleSheet StyleSheet {
			get {
				if(styleSheet == null) {
					styleSheet = new XRControlStyleSheet();
					LoadStyleSheetFromResources(styleSheet, this);
				}
				return styleSheet;
			}
		}
		public XRControlStyle this[string name] {
			get { return StyleSheet[name]; }
		}
		public string Name {
			get { return name; }
		}
		#endregion
		public ReportStyle(string name, string resourceName, Type type) {
			this.resourceName = resourceName;
			this.name = name;
			this.type = type;
		}
	}
}
