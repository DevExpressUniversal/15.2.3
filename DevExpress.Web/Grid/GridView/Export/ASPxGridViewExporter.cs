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
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.Web.Data;
using DevExpress.Web.Export;
using DevExpress.Web.Internal;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Web {
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.Design.ASPxGridExportDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	PersistChildren(false), ParseChildren(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxGridViewExporter.bmp"),
	Themeable(true)
]
	public class ASPxGridViewExporter : ASPxGridExporterBase {
		static readonly object renderBrick = new object();
		GridViewExportStyles styles;
		static ASPxGridViewExporter() {
			BrickResolver.EnsureStaticConstructor();
		}
		public ASPxGridViewExporter() {
		}
		public override string DefaultFileName { get { return "GridView"; } } [
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterRenderBrick"),
#endif
 Category("Events")]
		public event ASPxGridViewExportRenderingEventHandler RenderBrick { add { Events.AddHandler(renderBrick, value); } remove { Events.RemoveHandler(renderBrick, value); } }
		protected internal virtual ASPxGridViewExportRenderingEventArgs RaiseRenderBrick(int visibleIndex, GridViewColumn column, GridViewRowType rowType, WebDataProxy data, BrickStyle brickStyle, string text, object textValue, string textValueFormat, string url, byte[] imageValue) {
			var handler = (ASPxGridViewExportRenderingEventHandler)Events[renderBrick];
			if(handler == null)
				return null;
			var e = new ASPxGridViewExportRenderingEventArgs(visibleIndex, column, rowType, data, brickStyle, text, textValue, textValueFormat, url, imageValue);
			handler(this, e);
			return e;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterStyles"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportStyles Styles {
			get {
				if(styles == null)
					styles = new GridViewExportStyles(null);
				return styles;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterGridViewID"),
#endif
 Category("Data"), DefaultValue(""), Themeable(false), IDReferenceProperty(typeof(ASPxGridView)), TypeConverter("DevExpress.Web.Design.GridViewIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public virtual string GridViewID { get { return GridBaseID; } set { GridBaseID = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterFileName"),
#endif
 DefaultValue(""), Themeable(false)]
		public override string FileName { get { return base.FileName; } set { base.FileName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterMaxColumnWidth"),
#endif
 Category("Behavior"), DefaultValue(DefaultMaxColumnWidth), Themeable(false)]
		public override int MaxColumnWidth { get { return base.MaxColumnWidth; } set { base.MaxColumnWidth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterPrintSelectCheckBox"),
#endif
 Category("Behavior"), DefaultValue(false)]
		public override bool PrintSelectCheckBox { get { return base.PrintSelectCheckBox; } set { base.PrintSelectCheckBox = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterLandscape"),
#endif
 Category("PageSettings"), DefaultValue(false)]
		public override bool Landscape { get { return base.Landscape; } set { base.Landscape = value; } }
		[ Category("PageSettings"), DefaultValue(-1)]
		public override bool ExportSelectedRowsOnly { get { return base.ExportSelectedRowsOnly; } set { base.ExportSelectedRowsOnly = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterBottomMargin"),
#endif
 Category("PageSettings"), DefaultValue(-1)]
		public override int BottomMargin { get { return base.BottomMargin; } set { base.BottomMargin = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterTopMargin"),
#endif
 Category("PageSettings"), DefaultValue(-1)]
		public override int TopMargin { get { return base.TopMargin; } set { base.TopMargin = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterLeftMargin"),
#endif
 Category("PageSettings"), DefaultValue(-1)]
		public override int LeftMargin { get { return base.LeftMargin; } set { base.LeftMargin = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterRightMargin"),
#endif
 Category("PageSettings"), DefaultValue(-1)]
		public override int RightMargin { get { return base.RightMargin; } set { base.RightMargin = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterPageHeader"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		Editor("DevExpress.Web.Design.GridViewExporterPageHeaderFooterEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public override GridViewExporterHeaderFooter PageHeader { get { return base.PageHeader; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterPageFooter"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		Editor("DevExpress.Web.Design.GridViewExporterPageHeaderFooterEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public override GridViewExporterHeaderFooter PageFooter { get { return base.PageFooter; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterReportHeader"),
#endif
 NotifyParentProperty(true), DefaultValue(""), Editor("DevExpress.Web.Design.GridViewExporterReportHeaderFooterEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public override string ReportHeader { get { return base.ReportHeader; } set { base.ReportHeader = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterReportFooter"),
#endif
 NotifyParentProperty(true), DefaultValue(""), Editor("DevExpress.Web.Design.GridViewExporterReportHeaderFooterEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public override string ReportFooter { get { return base.ReportFooter; } set { base.ReportFooter = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterPaperKind"),
#endif
 Category("PageSettings"), DefaultValue(PaperKind.Letter)]
		public override PaperKind PaperKind { get { return base.PaperKind; } set { base.PaperKind = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterPaperName"),
#endif
 Category("PageSettings"), DefaultValue("")]
		public override string PaperName { get { return base.PaperName; } set { base.PaperName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterClientID"),
#endif
 EditorBrowsable(EditorBrowsableState.Never)]
		public override string ClientID { get { return base.ClientID; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterControls"),
#endif
 EditorBrowsable(EditorBrowsableState.Never)]
		public override ControlCollection Controls { get { return base.Controls; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterPreserveGroupRowStates"),
#endif
 Category("Behavior"), DefaultValue(false)]
		public virtual bool PreserveGroupRowStates {
			get { return (bool)GetObjectFromViewState("PreserveGroupRowStates", false); }
			set { SetObjectToViewState("PreserveGroupRowStates", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterDetailVerticalOffset"),
#endif
 NotifyParentProperty(true), DefaultValue(5)]
		public int DetailVerticalOffset {
			get { return (int)GetObjectFromViewState("DetailVerticalOffset", 5); }
			set {
				if(value < 0)
					value = 0;
				SetObjectToViewState("DetailVerticalOffset", 5, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterDetailHorizontalOffset"),
#endif
		NotifyParentProperty(true), DefaultValue(5)]
		public int DetailHorizontalOffset {
			get { return (int)GetObjectFromViewState("DetailHorizontalOffset", 5); }
			set {
				if(value < 0)
					value = 0;
				SetObjectToViewState("DetailHorizontalOffset", 5, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterExportEmptyDetailGrid"),
#endif
		NotifyParentProperty(true), DefaultValue(false)]
		public bool ExportEmptyDetailGrid {
			get { return (bool)GetObjectFromViewState("ExportEmptyDetailGrid", false); }
			set {
				SetObjectToViewState("ExportEmptyDetailGrid", false, value);
			}
		}
		[Obsolete("Use the ExportSelectedRowsOnly property instead."), EditorBrowsable(EditorBrowsableState.Never), Browsable(false),
#if !SL
	DevExpressWebLocalizedDescription("ASPxGridViewExporterExportedRowType"),
#endif
 Category("Behavior"), DefaultValue(GridViewExportedRowType.All)]
		public override GridViewExportedRowType ExportedRowType { get { return base.ExportedRowType; } set { base.ExportedRowType = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ASPxGridView GridView { get { return GridBase as ASPxGridView; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void RenderControl(HtmlTextWriter writer) {
			base.RenderControl(writer);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void ApplyStyleSheetSkin(System.Web.UI.Page page) { base.ApplyStyleSheetSkin(page); }
		protected override ControlCollection CreateControlCollection() { return new EmptyControlCollection(this); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Control FindControl(string id) { return base.FindControl(id); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void Focus() { }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool HasControls() { return base.HasControls(); }
		protected override GridExcelDataPrinter CreateExcelDataPrinter() {
			return new GridViewExcelDataPrinter(this);
		}
		protected override void PrepareExporterOptions(IDataAwareExportOptions options) {
			base.PrepareExporterOptions(options);
			var vertLines = GridView.Settings.GridLines == GridLines.Both || GridView.Settings.GridLines == GridLines.Vertical;
			var horzLines = GridView.Settings.GridLines == GridLines.Both || GridView.Settings.GridLines == GridLines.Horizontal;
			options.AllowVertLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.AllowVertLines, vertLines);
			options.AllowHorzLines = DataAwareExportOptionsFactory.UpdateDefaultBoolean(options.AllowHorzLines, horzLines);
			options.ShowColumnHeaders = GridView.Settings.ShowColumnHeaders ? DefaultBoolean.True : DefaultBoolean.False;
		}
		protected override GridLinkBase GetPrintableLink() { return new GridViewLink(this); }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GridViewExporterHeaderFooter : StateManager {
		Style style;
		public GridViewExporterHeaderFooter() {
			this.style = new Style();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExporterHeaderFooterLeft"),
#endif
 DefaultValue(""), NotifyParentProperty(true)]
		public string Left {
			get { return GetStringProperty("Left", string.Empty); }
			set { SetStringProperty("Left", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExporterHeaderFooterCenter"),
#endif
 DefaultValue(""), NotifyParentProperty(true)]
		public string Center {
			get { return GetStringProperty("Center", string.Empty); }
			set { SetStringProperty("Center", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExporterHeaderFooterRight"),
#endif
 DefaultValue(""), NotifyParentProperty(true)]
		public string Right {
			get { return GetStringProperty("Right", string.Empty); }
			set { SetStringProperty("Right", string.Empty, value); }
		}
		protected internal string[] Texts { get { return new string[] { Left, Center, Right }; } }
		protected Style Style { get { return style; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExporterHeaderFooterFont"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public FontInfo Font { get { return style.Font; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewExporterHeaderFooterVerticalAlignment"),
#endif
		DefaultValue(BrickAlignment.Center), NotifyParentProperty(true)]
		public BrickAlignment VerticalAlignment {
			get { return (BrickAlignment)GetEnumProperty("VerticalAlignment", BrickAlignment.Center); }
			set { SetEnumProperty("VerticalAlignment", BrickAlignment.Center, value); }
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Style };
		}
	}
	public class ASPxGridViewExportRenderingEventArgs : EventArgs {
		int visibleIndex;
		GridViewColumn column;
		GridViewRowType rowType;
		WebDataProxy data;
		BrickStyle brickStyle;
		string text;
		object textValue;
		string textValueFormatString;
		string url;
		protected internal ASPxGridViewExportRenderingEventArgs(int visibleIndex, GridViewColumn column, GridViewRowType rowType, WebDataProxy data, BrickStyle brickStyle,
			string text, object textValue, string textValueFormatString, string url, byte[] imageValue) {
			this.visibleIndex = visibleIndex;
			this.column = column;
			this.rowType = rowType;
			this.data = data;
			this.brickStyle = new BrickStyle(brickStyle);
			this.text = text;
			this.textValue = textValue;
			this.textValueFormatString = textValueFormatString;
			this.url = url;
			ImageValue = imageValue;
		}
		protected WebDataProxy Data { get { return data; } }
		public int VisibleIndex { get { return visibleIndex; } }
		public GridViewColumn Column { get { return column; } }
		public GridViewRowType RowType { get { return rowType; } }
		public string Text { get { return text; } set { text = value; } }
		public object TextValue { get { return textValue; } set { textValue = value; } }
		public string TextValueFormatString { get { return textValueFormatString; } set { textValueFormatString = value; } }
		public string Url { get { return url; } set { url = value; } }
		public BrickStyle BrickStyle { get { return brickStyle; } }
		public object KeyValue { get { return VisibleIndex > -1 ? Data.GetRowKeyValue(VisibleIndex) : null; } }
		public object GetValue(string fieldName) { return Data.GetRowValue(visibleIndex, fieldName); }
		public object Value {
			get {
				if(Column == null)
					return Data.GetRowValue(VisibleIndex, string.Empty);
				GridViewDataColumn dataColumn = Column as GridViewDataColumn;
				if(dataColumn != null)
					return Data.GetRowValue(VisibleIndex, dataColumn.FieldName);
				return null;
			}
		}
		public byte[] ImageValue { get; set; }
	}
	public delegate void ASPxGridViewExportRenderingEventHandler(object sender, ASPxGridViewExportRenderingEventArgs e);
}
