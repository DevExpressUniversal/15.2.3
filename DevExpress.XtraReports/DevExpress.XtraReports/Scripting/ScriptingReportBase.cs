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
using System.Drawing;
using System.Drawing.Printing;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Net.Mail;
namespace DevExpress.XtraReports {
	public abstract class ScriptingReportBase {
		public abstract XtraReport OwnerReport { get; }
		#region Properties
		public string DisplayName {
			get {
				return OwnerReport.DisplayName;
			}
			set {
				OwnerReport.DisplayName = value;
			}
		}
		public System.Collections.Generic.IDictionary<string, string> Extensions {
			get {
				return OwnerReport.Extensions;
			}
		}
		public System.Collections.ObjectModel.Collection<DevExpress.XtraPrinting.Native.IObject> ObjectStorage {
			get {
				return OwnerReport.ObjectStorage;
			}
		}
		public System.Collections.ObjectModel.Collection<System.ComponentModel.IComponent> ComponentStorage {
			get {
				return OwnerReport.ComponentStorage;
			}
		}
		public string ScriptsSource {
			get {
				return OwnerReport.ScriptsSource;
			}
			set {
				OwnerReport.ScriptsSource = value;
			}
		}
		public System.Drawing.Color PageColor {
			get {
				return OwnerReport.PageColor;
			}
			set {
				OwnerReport.PageColor = value;
			}
		}
		public float SnapGridSize {
			get {
				return OwnerReport.SnapGridSize;
			}
			set {
				OwnerReport.SnapGridSize = value;
			}
		}
		public System.Drawing.SizeF GridSizeF {
			get {
				return OwnerReport.GridSizeF;
			}
		}
		public DevExpress.XtraReports.UI.FormattingRuleSheet FormattingRuleSheet {
			get {
				return OwnerReport.FormattingRuleSheet;
			}
		}
		public string Bookmark {
			get {
				return OwnerReport.Bookmark;
			}
			set {
				OwnerReport.Bookmark = value;
			}
		}
		public DevExpress.XtraReports.Parameters.ParameterCollection Parameters {
			get {
				return OwnerReport.Parameters;
			}
		}
		public bool DrawGrid {
			get {
				return OwnerReport.DrawGrid;
			}
			set {
				OwnerReport.DrawGrid = value;
			}
		}
		public bool DrawWatermark {
			get {
				return OwnerReport.DrawWatermark;
			}
			set {
				OwnerReport.DrawWatermark = value;
			}
		}
		public bool SnapToGrid {
			get {
				return OwnerReport.SnapToGrid;
			}
			set {
				OwnerReport.SnapToGrid = value;
			}
		}
		public DevExpress.XtraReports.UI.DesignerOptions DesignerOptions {
			get {
				return OwnerReport.DesignerOptions;
			}
		}
		public string DataSourceSchema {
			get {
				return OwnerReport.DataSourceSchema;
			}
			set {
				OwnerReport.DataSourceSchema = value;
			}
		}
		public DevExpress.XtraReports.UI.SnappingMode SnappingMode {
			get {
				return OwnerReport.SnappingMode;
			}
			set {
				OwnerReport.SnappingMode = value;
			}
		}
		public bool Expanded {
			get {
				return OwnerReport.Expanded;
			}
			set {
				OwnerReport.Expanded = value;
			}
		}
		public DevExpress.XtraReports.UI.CalculatedFieldCollection CalculatedFields {
			get {
				return OwnerReport.CalculatedFields;
			}
		}
		public DevExpress.XtraReports.UI.XRCrossBandControlCollection CrossBandControls {
			get {
				return OwnerReport.CrossBandControls;
			}
		}
		public bool LockedInUserDesigner {
			get {
				return OwnerReport.LockedInUserDesigner;
			}
			set {
				OwnerReport.LockedInUserDesigner = value;
			}
		}
		public DevExpress.XtraReports.UI.XtraReportScripts Scripts {
			get {
				return OwnerReport.Scripts;
			}
		}
		public DevExpress.XtraPrinting.PrinterSettingsUsing DefaultPrinterSettingsUsing {
			get {
				return OwnerReport.DefaultPrinterSettingsUsing;
			}
		}
		public bool ShowPreviewMarginLines {
			get {
				return OwnerReport.ShowPreviewMarginLines;
			}
			set {
				OwnerReport.ShowPreviewMarginLines = value;
			}
		}
		public DevExpress.XtraPrinting.VerticalContentSplitting VerticalContentSplitting {
			get {
				return OwnerReport.VerticalContentSplitting;
			}
			set {
				OwnerReport.VerticalContentSplitting = value;
			}
		}
		public DevExpress.XtraPrinting.HorizontalContentSplitting HorizontalContentSplitting {
			get {
				return OwnerReport.HorizontalContentSplitting;
			}
			set {
				OwnerReport.HorizontalContentSplitting = value;
			}
		}
		public bool BookmarkDuplicateSuppress {
			get {
				return OwnerReport.BookmarkDuplicateSuppress;
			}
			set {
				OwnerReport.BookmarkDuplicateSuppress = value;
			}
		}
		public DevExpress.XtraReports.UI.PageBreak PageBreak {
			get {
				return OwnerReport.PageBreak;
			}
			set {
				OwnerReport.PageBreak = value;
			}
		}
		public DevExpress.XtraReports.UI.XtraReport MasterReport {
			get {
				return OwnerReport.MasterReport;
			}
			set {
				OwnerReport.MasterReport = value;
			}
		}
		public string SourceUrl { 
			get { 
				return MasterReport.SourceUrl;
			} 
			set { 
				MasterReport.SourceUrl = value; 
			}
		}
		public DevExpress.XtraPrinting.PrintingSystemBase PrintingSystem {
			get {
				return OwnerReport.PrintingSystem;
			}
		}
		public DevExpress.XtraPrinting.ExportOptions ExportOptions {
			get {
				return OwnerReport.ExportOptions;
			}
		}
		public DevExpress.XtraReports.UI.ReportUnit ReportUnit {
			get {
				return OwnerReport.ReportUnit;
			}
			set {
				OwnerReport.ReportUnit = value;
			}
		}
		public bool Landscape {
			get {
				return OwnerReport.Landscape;
			}
			set {
				OwnerReport.Landscape = value;
			}
		}
		public bool RollPaper {
			get {
				return OwnerReport.RollPaper;
			}
			set {
				OwnerReport.RollPaper = value;
			}
		}
		public System.Drawing.Printing.Margins Margins {
			get {
				return OwnerReport.Margins;
			}
			set {
				OwnerReport.Margins = value;
			}
		}
		public string PrinterName {
			get {
				return OwnerReport.PrinterName;
			}
			set {
				OwnerReport.PrinterName = value;
			}
		}
		public System.Drawing.Printing.PaperKind PaperKind {
			get {
				return OwnerReport.PaperKind;
			}
			set {
				OwnerReport.PaperKind = value;
			}
		}
		public string PaperName {
			get {
				return OwnerReport.PaperName;
			}
			set {
				OwnerReport.PaperName = value;
			}
		}
		public System.Drawing.Size PageSize {
			get {
				return OwnerReport.PageSize;
			}
			set {
				OwnerReport.PageSize = value;
			}
		}
		public int PageWidth {
			get {
				return OwnerReport.PageWidth;
			}
			set {
				OwnerReport.PageWidth = value;
			}
		}
		public int PageHeight {
			get {
				return OwnerReport.PageHeight;
			}
			set {
				OwnerReport.PageHeight = value;
			}
		}
		public DevExpress.XtraReports.UI.XRControlStyleSheet StyleSheet {
			get {
				return OwnerReport.StyleSheet;
			}
		}
		public string StyleSheetPath {
			get {
				return OwnerReport.StyleSheetPath;
			}
			set {
				OwnerReport.StyleSheetPath = value;
			}
		}
		public DevExpress.XtraReports.UI.XRWatermark Watermark {
			get {
				return OwnerReport.Watermark;
			}
		}
		public DevExpress.XtraReports.ScriptLanguage ScriptLanguage {
			get {
				return OwnerReport.ScriptLanguage;
			}
			set {
				OwnerReport.ScriptLanguage = value;
			}
		}
		public string[] ScriptReferences {
			get {
				return OwnerReport.ScriptReferences;
			}
			set {
				OwnerReport.ScriptReferences = value;
			}
		}
		public DevExpress.XtraReports.ScriptSecurityPermissionCollection ScriptSecurityPermissions {
			get {
				return OwnerReport.ScriptSecurityPermissions;
			}
		}
		public string ScriptReferencesString {
			get {
				return OwnerReport.ScriptReferencesString;
			}
			set {
				OwnerReport.ScriptReferencesString = value;
			}
		}
		public DevExpress.XtraPrinting.PageList Pages {
			get {
				return OwnerReport.Pages;
			}
		}
		public string Version {
			get {
				return OwnerReport.Version;
			}
			set {
				OwnerReport.Version = value;
			}
		}
		public bool RequestParameters {
			get {
				return OwnerReport.RequestParameters;
			}
			set {
				OwnerReport.RequestParameters = value;
			}
		}
		public bool ShowPrintStatusDialog {
			get {
				return OwnerReport.ShowPrintStatusDialog;
			}
			set {
				OwnerReport.ShowPrintStatusDialog = value;
			}
		}
		public bool ShowPrintMarginsWarning {
			get {
				return OwnerReport.ShowPrintMarginsWarning;
			}
			set {
				OwnerReport.ShowPrintMarginsWarning = value;
			}
		}
		public string ControlType {
			get {
				return OwnerReport.ControlType;
			}
		}
		public string EventsInfo {
			get {
				return OwnerReport.EventsInfo;
			}
			set {
				OwnerReport.EventsInfo = value;
			}
		}
		public DevExpress.XtraReports.UI.ReportPrintOptions ReportPrintOptions {
			get {
				return OwnerReport.ReportPrintOptions;
			}
		}
		public DevExpress.XtraPrinting.PaddingInfo SnapLinePadding {
			get {
				return OwnerReport.SnapLinePadding;
			}
			set {
				OwnerReport.SnapLinePadding = value;
			}
		}
		public float HeightF {
			get {
				return OwnerReport.HeightF;
			}
			set {
				OwnerReport.HeightF = value;
			}
		}
		public DevExpress.XtraReports.UI.XRControl.XRControlStyles Styles {
			get {
				return OwnerReport.Styles;
			}
		}
		public DevExpress.XtraReports.UI.StylePriority StylePriority {
			get {
				return OwnerReport.StylePriority;
			}
		}
		public string StyleName {
			get {
				return OwnerReport.StyleName;
			}
			set {
				OwnerReport.StyleName = value;
			}
		}
		public string EvenStyleName {
			get {
				return OwnerReport.EvenStyleName;
			}
			set {
				OwnerReport.EvenStyleName = value;
			}
		}
		public string OddStyleName {
			get {
				return OwnerReport.OddStyleName;
			}
			set {
				OwnerReport.OddStyleName = value;
			}
		}
		public string FilterString {
			get {
				return OwnerReport.FilterString;
			}
			set {
				OwnerReport.FilterString = value;
			}
		}
		public string DataMember {
			get {
				return OwnerReport.DataMember;
			}
			set {
				OwnerReport.DataMember = value;
			}
		}
		public object DataSource {
			get {
				return OwnerReport.DataSource;
			}
			set {
				OwnerReport.DataSource = value;
			}
		}
		public object DataAdapter {
			get {
				return OwnerReport.DataAdapter;
			}
			set {
				OwnerReport.DataAdapter = value;
			}
		}
		public string XmlDataPath {
			get {
				return OwnerReport.XmlDataPath;
			}
			set {
				OwnerReport.XmlDataPath = value;
			}
		}
		public DevExpress.XtraReports.UI.XRControlCollection Controls {
			get {
				return OwnerReport.Controls;
			}
		}
		public int CurrentRowIndex {
			get {
				return OwnerReport.CurrentRowIndex;
			}
		}
		public int RowCount {
			get {
				return OwnerReport.RowCount;
			}
		}
		public DevExpress.XtraReports.UI.BandCollection Bands {
			get {
				return OwnerReport.Bands;
			}
		}
		public bool KeepTogether {
			get {
				return OwnerReport.KeepTogether;
			}
			set {
				OwnerReport.KeepTogether = value;
			}
		}
		public DevExpress.XtraPrinting.PaddingInfo SnapLineMargin {
			get {
				return OwnerReport.SnapLineMargin;
			}
			set {
				OwnerReport.SnapLineMargin = value;
			}
		}
		public bool CanHaveChildren {
			get {
				return OwnerReport.CanHaveChildren;
			}
		}
		public string NavigateUrl {
			get {
				return OwnerReport.NavigateUrl;
			}
			set {
				OwnerReport.NavigateUrl = value;
			}
		}
		public string Target {
			get {
				return OwnerReport.Target;
			}
			set {
				OwnerReport.Target = value;
			}
		}
		public DevExpress.XtraReports.UI.XRControl BookmarkParent {
			get {
				return OwnerReport.BookmarkParent;
			}
			set {
				OwnerReport.BookmarkParent = value;
			}
		}
		public DevExpress.XtraReports.UI.VerticalAnchorStyles AnchorVertical {
			get {
				return OwnerReport.AnchorVertical;
			}
			set {
				OwnerReport.AnchorVertical = value;
			}
		}
		public DevExpress.XtraReports.UI.HorizontalAnchorStyles AnchorHorizontal {
			get {
				return OwnerReport.AnchorHorizontal;
			}
			set {
				OwnerReport.AnchorHorizontal = value;
			}
		}
		public float WidthF {
			get {
				return OwnerReport.WidthF;
			}
			set {
				OwnerReport.WidthF = value;
			}
		}
		public DevExpress.XtraReports.UI.XRBindingCollection DataBindings {
			get {
				return OwnerReport.DataBindings;
			}
		}
		public System.Drawing.RectangleF BoundsF {
			get {
				return OwnerReport.BoundsF;
			}
			set {
				OwnerReport.BoundsF = value;
			}
		}
		public string Text {
			get {
				return OwnerReport.Text;
			}
			set {
				OwnerReport.Text = value;
			}
		}
		public string XlsxFormatString {
			get {
				return OwnerReport.XlsxFormatString;
			}
			set {
				OwnerReport.XlsxFormatString = value;
			}
		}
		public System.Drawing.SizeF SizeF {
			get {
				return OwnerReport.SizeF;
			}
			set {
				OwnerReport.SizeF = value;
			}
		}
		public System.Drawing.PointF LocationF {
			get {
				return OwnerReport.LocationF;
			}
			set {
				OwnerReport.LocationF = value;
			}
		}
		public DevExpress.Utils.PointFloat LocationFloat {
			get {
				return OwnerReport.LocationFloat;
			}
			set {
				OwnerReport.LocationFloat = value;
			}
		}
		public float LeftF {
			get {
				return OwnerReport.LeftF;
			}
			set {
				OwnerReport.LeftF = value;
			}
		}
		public float TopF {
			get {
				return OwnerReport.TopF;
			}
			set {
				OwnerReport.TopF = value;
			}
		}
		public float RightF {
			get {
				return OwnerReport.RightF;
			}
		}
		public float BottomF {
			get {
				return OwnerReport.BottomF;
			}
		}
		public bool CanGrow {
			get {
				return OwnerReport.CanGrow;
			}
			set {
				OwnerReport.CanGrow = value;
			}
		}
		public bool CanShrink {
			get {
				return OwnerReport.CanShrink;
			}
			set {
				OwnerReport.CanShrink = value;
			}
		}
		public bool WordWrap {
			get {
				return OwnerReport.WordWrap;
			}
			set {
				OwnerReport.WordWrap = value;
			}
		}
		public System.Drawing.PointF RightBottomF {
			get {
				return OwnerReport.RightBottomF;
			}
		}
		public bool IsSingleChild {
			get {
				return OwnerReport.IsSingleChild;
			}
		}
		public DevExpress.XtraReports.UI.FormattingRuleCollection FormattingRules {
			get {
				return OwnerReport.FormattingRules;
			}
		}
		public DevExpress.XtraReports.Serialization.ItemLinkCollection FormattingRuleLinks {
			get {
				return OwnerReport.FormattingRuleLinks;
			}
		}
		public bool HasChildren {
			get {
				return OwnerReport.HasChildren;
			}
		}
		public DevExpress.XtraReports.UI.XRControl Parent {
			get {
				return OwnerReport.Parent;
			}
			set {
				OwnerReport.Parent = value;
			}
		}
		public int Index {
			get {
				return OwnerReport.Index;
			}
			set {
				OwnerReport.Index = value;
			}
		}
		public string NullValueText {
			get {
				return OwnerReport.NullValueText;
			}
			set {
				OwnerReport.NullValueText = value;
			}
		}
		public DevExpress.XtraPrinting.TextAlignment TextAlignment {
			get {
				return OwnerReport.TextAlignment;
			}
			set {
				OwnerReport.TextAlignment = value;
			}
		}
		public System.Drawing.Rectangle Bounds {
			get {
				return OwnerReport.Bounds;
			}
			set {
				OwnerReport.Bounds = value;
			}
		}
		public int Width {
			get {
				return OwnerReport.Width;
			}
			set {
				OwnerReport.Width = value;
			}
		}
		public int Height {
			get {
				return OwnerReport.Height;
			}
			set {
				OwnerReport.Height = value;
			}
		}
		public System.Drawing.Size Size {
			get {
				return OwnerReport.Size;
			}
			set {
				OwnerReport.Size = value;
			}
		}
		public System.Drawing.Point Location {
			get {
				return OwnerReport.Location;
			}
			set {
				OwnerReport.Location = value;
			}
		}
		public int Left {
			get {
				return OwnerReport.Left;
			}
			set {
				OwnerReport.Left = value;
			}
		}
		public int Top {
			get {
				return OwnerReport.Top;
			}
			set {
				OwnerReport.Top = value;
			}
		}
		public int Right {
			get {
				return OwnerReport.Right;
			}
		}
		public int Bottom {
			get {
				return OwnerReport.Bottom;
			}
		}
		public bool IsDisposed {
			get {
				return OwnerReport.IsDisposed;
			}
		}
		public float Dpi {
			get {
				return OwnerReport.Dpi;
			}
			set {
				OwnerReport.Dpi = value;
			}
		}
		public string Name {
			get {
				return OwnerReport.Name;
			}
			set {
				OwnerReport.Name = value;
			}
		}
		public DevExpress.XtraReports.UI.StyleUsing ParentStyleUsing {
			get {
				return OwnerReport.ParentStyleUsing;
			}
		}
		public System.Drawing.Font Font {
			get {
				return OwnerReport.Font;
			}
			set {
				OwnerReport.Font = value;
			}
		}
		public System.Drawing.Color ForeColor {
			get {
				return OwnerReport.ForeColor;
			}
			set {
				OwnerReport.ForeColor = value;
			}
		}
		public System.Drawing.Color BackColor {
			get {
				return OwnerReport.BackColor;
			}
			set {
				OwnerReport.BackColor = value;
			}
		}
		public DevExpress.XtraPrinting.PaddingInfo Padding {
			get {
				return OwnerReport.Padding;
			}
			set {
				OwnerReport.Padding = value;
			}
		}
		public System.Drawing.Color BorderColor {
			get {
				return OwnerReport.BorderColor;
			}
			set {
				OwnerReport.BorderColor = value;
			}
		}
		public DevExpress.XtraPrinting.BorderSide Borders {
			get {
				return OwnerReport.Borders;
			}
			set {
				OwnerReport.Borders = value;
			}
		}
		public float BorderWidth {
			get {
				return OwnerReport.BorderWidth;
			}
			set {
				OwnerReport.BorderWidth = value;
			}
		}
		public DevExpress.XtraPrinting.BorderDashStyle BorderDashStyle {
			get {
				return OwnerReport.BorderDashStyle;
			}
			set {
				OwnerReport.BorderDashStyle = value;
			}
		}
		public object Tag {
			get {
				return OwnerReport.Tag;
			}
			set {
				OwnerReport.Tag = value;
			}
		}
		public bool Visible {
			get {
				return OwnerReport.Visible;
			}
			set {
				OwnerReport.Visible = value;
			}
		}
		public DevExpress.XtraReports.UI.XtraReport RootReport {
			get {
				return OwnerReport.RootReport;
			}
		}
		public DevExpress.XtraReports.UI.XtraReportBase Report {
			get {
				return OwnerReport.Report;
			}
		}
		public DevExpress.XtraReports.UI.Band Band {
			get {
				return OwnerReport.Band;
			}
		}
		public System.ComponentModel.ISite Site {
			get {
				return OwnerReport.Site;
			}
			set {
				OwnerReport.Site = value;
			}
		}
		public System.ComponentModel.IContainer Container {
			get {
				return OwnerReport.Container;
			}
		}
		#endregion
		#region Methods
		public IEnumerable<XRControl> HasExportWarningControls() {
			return OwnerReport.HasExportWarningControls();
		}
		public IEnumerable<T> AllControls<T>() where T : XRControl {
			return OwnerReport.AllControls<T>();
		}
		public void ExportToText(string path) {
			OwnerReport.ExportToText(path);
		}
		public void ExportToText(string path, DevExpress.XtraPrinting.TextExportOptions options) {
			OwnerReport.ExportToText(path, options);
		}
		public void ExportToText(System.IO.Stream stream, DevExpress.XtraPrinting.TextExportOptions options) {
			OwnerReport.ExportToText(stream, options);
		}
		public void ExportToCsv(string path) {
			OwnerReport.ExportToCsv(path);
		}
		public void ExportToCsv(string path, DevExpress.XtraPrinting.CsvExportOptions options) {
			OwnerReport.ExportToCsv(path, options);
		}
		public void ExportToCsv(System.IO.Stream stream, DevExpress.XtraPrinting.CsvExportOptions options) {
			OwnerReport.ExportToCsv(stream, options);
		}
		public void ExportToXls(string path) {
			OwnerReport.ExportToXls(path);
		}
		public void ExportToXls(string path, DevExpress.XtraPrinting.XlsExportOptions options) {
			OwnerReport.ExportToXls(path, options);
		}
		public void ExportToXls(System.IO.Stream stream, DevExpress.XtraPrinting.XlsExportOptions options) {
			OwnerReport.ExportToXls(stream, options);
		}
		public void ExportToXlsx(string path) {
			OwnerReport.ExportToXlsx(path);
		}
		public void ExportToXlsx(string path, DevExpress.XtraPrinting.XlsxExportOptions options) {
			OwnerReport.ExportToXlsx(path, options);
		}
		public void ExportToXlsx(System.IO.Stream stream, DevExpress.XtraPrinting.XlsxExportOptions options) {
			OwnerReport.ExportToXlsx(stream, options);
		}
		public void ExportToRtf(string path) {
			OwnerReport.ExportToRtf(path);
		}
		public void ExportToRtf(string path, DevExpress.XtraPrinting.RtfExportOptions options) {
			OwnerReport.ExportToRtf(path, options);
		}
		public void ExportToRtf(System.IO.Stream stream, DevExpress.XtraPrinting.RtfExportOptions options) {
			OwnerReport.ExportToRtf(stream, options);
		}
		public void ExportToHtml(string path) {
			OwnerReport.ExportToHtml(path);
		}
		public void ExportToHtml(string path, DevExpress.XtraPrinting.HtmlExportOptions options) {
			OwnerReport.ExportToHtml(path, options);
		}
		public void ExportToHtml(System.IO.Stream stream, DevExpress.XtraPrinting.HtmlExportOptions options) {
			OwnerReport.ExportToHtml(stream, options);
		}
		public void ExportToMht(string path) {
			OwnerReport.ExportToMht(path);
		}
		public void ExportToMht(string path, DevExpress.XtraPrinting.MhtExportOptions options) {
			OwnerReport.ExportToMht(path, options);
		}
		public void ExportToMht(System.IO.Stream stream, DevExpress.XtraPrinting.MhtExportOptions options) {
			OwnerReport.ExportToMht(stream, options);
		}
		public AlternateView ExportToMail() {
			return OwnerReport.ExportToMail();
		}
		public AlternateView ExportToMail(DevExpress.XtraPrinting.MailMessageExportOptions options) {
			return OwnerReport.ExportToMail(options);
		}
		public MailMessage ExportToMail(string from, string to, string subject) {
			return OwnerReport.ExportToMail(from, to, subject);
		}
		public MailMessage ExportToMail(DevExpress.XtraPrinting.MailMessageExportOptions options, string from, string to, string subject) {
			return OwnerReport.ExportToMail(options, from, to, subject);
		}
		public void ExportToPdf(string path) {
			OwnerReport.ExportToPdf(path);
		}
		public void ExportToPdf(string path, DevExpress.XtraPrinting.PdfExportOptions options) {
			OwnerReport.ExportToPdf(path, options);
		}
		public void ExportToPdf(System.IO.Stream stream, DevExpress.XtraPrinting.PdfExportOptions options) {
			OwnerReport.ExportToPdf(stream, options);
		}
		public void ExportToImage(string path) {
			OwnerReport.ExportToImage(path);
		}
		public void ExportToImage(string path, DevExpress.XtraPrinting.ImageExportOptions options) {
			OwnerReport.ExportToImage(path, options);
		}
		public void ExportToImage(System.IO.Stream stream, DevExpress.XtraPrinting.ImageExportOptions options) {
			OwnerReport.ExportToImage(stream, options);
		}
		public void CreateLayoutViewDocument() {
			OwnerReport.CreateLayoutViewDocument();
		}
		public void CreateDocument() {
			OwnerReport.CreateDocument();
		}
		public void CreateDocument(bool buildPagesInBackground) {
			OwnerReport.CreateDocument(buildPagesInBackground);
		}
		public void StopPageBuilding() {
			OwnerReport.StopPageBuilding();
		}
		public System.CodeDom.Compiler.CompilerErrorCollection ValidateScripts() {
			return OwnerReport.ValidateScripts();
		}
		public object GetEffectiveDataSource() {
			return OwnerReport.GetEffectiveDataSource();
		}
		public void CollectParameters(System.Collections.Generic.IList<DevExpress.XtraReports.Parameters.Parameter> list, System.Predicate<DevExpress.XtraReports.Parameters.Parameter> condition) {
			OwnerReport.CollectParameters(list, condition);
		}
		public void SaveLayoutToXml(string fileName) {
			OwnerReport.SaveLayoutToXml(fileName);
		}
		public void LoadLayoutFromXml(string fileName) {
			OwnerReport.LoadLayoutFromXml(fileName);
		}
		public void SaveLayout(string fileName) {
			OwnerReport.SaveLayout(fileName);
		}
		public void LoadLayout(string fileName) {
			OwnerReport.LoadLayout(fileName);
		}
		public void SaveLayout(string fileName, bool throwOnError) {
			OwnerReport.SaveLayout(fileName, throwOnError);
		}
		public void SaveLayout(System.IO.Stream stream, bool throwOnError) {
			OwnerReport.SaveLayout(stream, throwOnError);
		}
		public void BeginUpdate() {
			OwnerReport.BeginUpdate();
		}
		public void EndUpdate() {
			OwnerReport.EndUpdate();
		}
		public void BeginInit() {
			OwnerReport.BeginInit();
		}
		public void EndInit() {
			OwnerReport.EndInit();
		}
		public void ApplyFiltering() {
			OwnerReport.ApplyFiltering();
		}
		public object GetPreviousRow() {
			return OwnerReport.GetPreviousRow();
		}
		public object GetNextRow() {
			return OwnerReport.GetNextRow();
		}
		public object GetCurrentRow() {
			return OwnerReport.GetCurrentRow();
		}
		public object GetPreviousColumnValue(string columnName) {
			return OwnerReport.GetPreviousColumnValue(columnName);
		}
		public object GetNextColumnValue(string columnName) {
			return OwnerReport.GetNextColumnValue(columnName);
		}
		public object GetCurrentColumnValue(string columnName) {
			return OwnerReport.GetCurrentColumnValue(columnName);
		}
		public string FromDisplayName(string displayName) {
			return OwnerReport.FromDisplayName(displayName);
		}
		public void FillDataSource() {
			OwnerReport.FillDataSource();
		}
		public void IterateReportsRecursive(System.Action<DevExpress.XtraReports.UI.XtraReportBase> action) {
			OwnerReport.IterateReportsRecursive(action);
		}
		public System.Drawing.Size GetMinSize() {
			return OwnerReport.GetMinSize();
		}
		public System.Drawing.Color GetEffectiveBackColor() {
			return OwnerReport.GetEffectiveBackColor();
		}
		public System.Drawing.Color GetEffectiveBorderColor() {
			return OwnerReport.GetEffectiveBorderColor();
		}
		public DevExpress.XtraPrinting.BorderDashStyle GetEffectiveBorderDashStyle() {
			return OwnerReport.GetEffectiveBorderDashStyle();
		}
		public DevExpress.XtraPrinting.BorderSide GetEffectiveBorders() {
			return OwnerReport.GetEffectiveBorders();
		}
		public float GetEffectiveBorderWidth() {
			return OwnerReport.GetEffectiveBorderWidth();
		}
		public System.Drawing.Font GetEffectiveFont() {
			return OwnerReport.GetEffectiveFont();
		}
		public System.Drawing.Color GetEffectiveForeColor() {
			return OwnerReport.GetEffectiveForeColor();
		}
		public DevExpress.XtraPrinting.PaddingInfo GetEffectivePadding() {
			return OwnerReport.GetEffectivePadding();
		}
		public DevExpress.XtraPrinting.TextAlignment GetEffectiveTextAlignment() {
			return OwnerReport.GetEffectiveTextAlignment();
		}
		public void RemoveInvalidBindings(System.Predicate<DevExpress.XtraReports.UI.XRBinding> predicate) {
			OwnerReport.RemoveInvalidBindings(predicate);
		}
		public void SuspendLayout() {
			OwnerReport.SuspendLayout();
		}
		public void ResumeLayout() {
			OwnerReport.ResumeLayout();
		}
		public void PerformLayout() {
			OwnerReport.PerformLayout();
		}
		public DevExpress.XtraReports.UI.XRControl FindControl(string name, bool ignoreCase) {
			return OwnerReport.FindControl(name, ignoreCase);
		}
		public void ResetBackColor() {
			OwnerReport.ResetBackColor();
		}
		public void ResetBorderColor() {
			OwnerReport.ResetBorderColor();
		}
		public void ResetBorders() {
			OwnerReport.ResetBorders();
		}
		public void ResetBorderDashStyle() {
			OwnerReport.ResetBorderDashStyle();
		}
		public void ResetBorderWidth() {
			OwnerReport.ResetBorderWidth();
		}
		public void ResetFont() {
			OwnerReport.ResetFont();
		}
		public void ResetForeColor() {
			OwnerReport.ResetForeColor();
		}
		public void ResetPadding() {
			OwnerReport.ResetPadding();
		}
		public void ResetTextAlignment() {
			OwnerReport.ResetTextAlignment();
		}
		public void SendToBack() {
			OwnerReport.SendToBack();
		}
		public void BringToFront() {
			OwnerReport.BringToFront();
		}
		public void Dispose() {
			OwnerReport.Dispose();
		}
		public new string ToString() {
			return OwnerReport.ToString();
		}
		public object GetLifetimeService() {
			return OwnerReport.GetLifetimeService();
		}
		public object InitializeLifetimeService() {
			return OwnerReport.InitializeLifetimeService();
		}
		public System.Runtime.Remoting.ObjRef CreateObjRef(System.Type requestedType) {
			return OwnerReport.CreateObjRef(requestedType);
		}
		public new bool Equals(object obj) {
			return OwnerReport.Equals(obj);
		}
		public new int GetHashCode() {
			return OwnerReport.GetHashCode();
		}
		public new System.Type GetType() {
			return OwnerReport.GetType();
		}
		public Delegate GetHandler(Type type, string name) {
			System.Reflection.MethodInfo mi = ((object)this).GetType().GetMethod(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			return mi != null ? Delegate.CreateDelegate(type, this, mi) : null;
		}
		#endregion
	}
}
