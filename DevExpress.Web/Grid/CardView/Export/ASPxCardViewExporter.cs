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
using System.Web.UI;
using DevExpress.Web.Data;
using DevExpress.Web.Export;
using DevExpress.XtraPrinting;
namespace DevExpress.Web {
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.Design.ASPxGridExportDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	PersistChildren(false), ParseChildren(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxGridViewExporter.bmp"), 
	Themeable(true)
]
	public class ASPxCardViewExporter : ASPxGridExporterBase {
		public const int DefaultCardWidth = 150;
		static readonly object renderBrick = new object();
		public const int DefaultMargin = 10;
		ASPxCardView cardView;
		CardViewExportStyles styles;
		public ASPxCardViewExporter() 
			: base() {
		}
		public ASPxCardViewExporter(ASPxCardView cardView)
			: base() {
			GridBase = cardView;
			CardViewID = cardView.ID;
		}
		public ASPxCardView CardView {
			get {
				if(cardView == null)
					cardView = GridBase as ASPxCardView;
				return cardView;
			}
		}
		public override string DefaultFileName { get { return "CardView"; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterRenderBrick"),
#endif
 Category("Events")]
		public event ASPxCardViewExportRenderingEventHandler RenderBrick { add { Events.AddHandler(renderBrick, value); } remove { Events.RemoveHandler(renderBrick, value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterStyles"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CardViewExportStyles Styles {
			get {
				if(styles == null)
					styles = new CardViewExportStyles(null);
				return styles;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterCardViewID"),
#endif
 Category("Data"), DefaultValue(""), Themeable(false), IDReferenceProperty(typeof(ASPxCardView)), TypeConverter("DevExpress.Web.Design.GridViewIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public virtual string CardViewID { get { return GridBaseID; } set { GridBaseID = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterFileName"),
#endif
 DefaultValue(""), Themeable(false)]
		public override string FileName { get { return base.FileName; } set { base.FileName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterMaxColumnWidth"),
#endif
 Category("Behavior"), DefaultValue(DefaultMaxColumnWidth), Themeable(false)]
		public override int MaxColumnWidth { get { return base.MaxColumnWidth; } set { base.MaxColumnWidth = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterPrintSelectCheckBox"),
#endif
 Category("Behavior"), DefaultValue(false)]
		public override bool PrintSelectCheckBox { get { return base.PrintSelectCheckBox; } set { base.PrintSelectCheckBox = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterLandscape"),
#endif
 Category("PageSettings"), DefaultValue(false)]
		public override bool Landscape { get { return base.Landscape; } set { base.Landscape = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterExportSelectedCardsOnly"),
#endif
 Category("Behavior"), DefaultValue(GridViewExportedRowType.All)]
		public bool ExportSelectedCardsOnly { get { return base.ExportSelectedRowsOnly; } set { base.ExportSelectedRowsOnly = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterLeftMargin"),
#endif
 Category("PageSettings"), DefaultValue(DefaultMargin)]
		public override int LeftMargin { 
			get { return (int)GetObjectFromViewState("LeftMargin", DefaultMargin); }
			set {
				if(value < 0)
					value = DefaultMargin;
				SetObjectToViewState("LeftMargin", DefaultMargin, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterTopMargin"),
#endif
 Category("PageSettings"), DefaultValue(DefaultMargin)]
		public override int TopMargin { 
			get { return (int)GetObjectFromViewState("TopMargin", DefaultMargin); }
			set {
				if(value < 0)
					value = DefaultMargin;
				SetObjectToViewState("TopMargin", DefaultMargin, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterRightMargin"),
#endif
 Category("PageSettings"), DefaultValue(DefaultMargin)]
		public override int RightMargin {
			get { return (int)GetObjectFromViewState("RightMargin", DefaultMargin); }
			set {
				if(value < 0)
					value = DefaultMargin;
				SetObjectToViewState("RightMargin", DefaultMargin, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterBottomMargin"),
#endif
 Category("PageSettings"), DefaultValue(DefaultMargin)]
		public override int BottomMargin {
			get { return (int)GetObjectFromViewState("BottomMargin", DefaultMargin); }
			set {
				if(value < 0)
					value = DefaultMargin;
				SetObjectToViewState("BottomMargin", DefaultMargin, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterPageHeader"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		Editor("DevExpress.Web.Design.GridViewExporterPageHeaderFooterEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public override GridViewExporterHeaderFooter PageHeader { get { return base.PageHeader; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterPageFooter"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		Editor("DevExpress.Web.Design.GridViewExporterPageHeaderFooterEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public override GridViewExporterHeaderFooter PageFooter { get { return base.PageFooter; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterPaperKind"),
#endif
 Category("PageSettings"), DefaultValue(PaperKind.Letter)]
		public override PaperKind PaperKind { get { return base.PaperKind; } set { base.PaperKind = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterPaperName"),
#endif
 Category("PageSettings"), DefaultValue("")]
		public override string PaperName { get { return base.PaperName; } set { base.PaperName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterCardWidth"),
#endif
 Category("PageSettings"), DefaultValue("")]
		public int CardWidth { get { return (int)GetObjectFromViewState("CardWidth", DefaultCardWidth); } set { SetObjectToViewState("CardWidth", DefaultCardWidth, value); } }
		[Obsolete("Use the ExportSelectedCardsOnly property instead."), EditorBrowsable(EditorBrowsableState.Never), Browsable(false),
#if !SL
	DevExpressWebLocalizedDescription("ASPxCardViewExporterExportedRowType"),
#endif
 Category("Behavior"), DefaultValue(GridViewExportedRowType.All)]
		public override GridViewExportedRowType ExportedRowType { get { return base.ExportedRowType; } set { base.ExportedRowType = value; } }
		protected override GridLinkBase GetPrintableLink() {
			return new CardViewLink(this);
		}
		protected override bool AllowExcelDataExport(ExportOptionsBase options, ExportTarget target) {
			return target == ExportTarget.Csv || target == ExportTarget.Xls || target == ExportTarget.Xlsx;
		}
		protected internal virtual ASPxCardViewExportRenderingEventArgs RaiseRenderBrick(int cardIndex, CardViewColumn column, WebDataProxy data, BrickStyle brickStyle, string text, object textValue, string textValueFormat, string url, byte[] imageValue) {
			var handler = (ASPxCardViewExportRenderingEventHandler)Events[renderBrick];
			if(handler == null)
				return null;
			var e = new ASPxCardViewExportRenderingEventArgs(cardIndex, column, data, brickStyle, text, textValue, textValueFormat, url, imageValue);
			handler(this, e);
			return e;
		}
	}
	public class ASPxCardViewExportRenderingEventArgs : EventArgs {
		protected internal ASPxCardViewExportRenderingEventArgs(int visibleIndex, CardViewColumn column, WebDataProxy data, BrickStyle brickStyle,
			string text, object textValue, string textValueFormatString, string url, byte[] imageValue) {
			Data = data;
			VisibleIndex = visibleIndex;
			Column = column;
			Text = text;
			TextValue = textValue;
			BrickStyle = new BrickStyle(brickStyle);
			TextValueFormatString = textValueFormatString;
			Url = url;
			ImageValue = imageValue;
		}
		protected WebDataProxy Data { get; private set; }
		public int VisibleIndex { get; private set; }
		public CardViewColumn Column { get; private set; }
		public string Text { get; set; }
		public object TextValue { get; set; }
		public string TextValueFormatString { get; set; }
		public string Url { get; set; }
		public BrickStyle BrickStyle { get; private set; }
		public object KeyValue { get { return VisibleIndex > -1 ? Data.GetRowKeyValue(VisibleIndex) : null; } }
		public object GetValue(string fieldName) { return Data.GetRowValue(VisibleIndex, fieldName); }
		public object Value { get { return Data.GetRowValue(VisibleIndex, Column.FieldName); } }
		public byte[] ImageValue { get; set; }
	}
	public delegate void ASPxCardViewExportRenderingEventHandler(object sender, ASPxCardViewExportRenderingEventArgs e);
}
namespace DevExpress.Web.Export {
	public class CardViewLink : GridLinkBase {
		CardViewPrintInfoCalculator printingInfo;
		CardViewPrinter activePrinter;
		public CardViewLink(ASPxCardViewExporter exporter)
			: base(exporter) {
		}
		protected ASPxCardViewExporter Exporter { get { return (ASPxCardViewExporter)ExporterBase; } }
		CardViewPrintInfoCalculator PrintingInfo {
			get {
				if(printingInfo == null)
					printingInfo = new CardViewPrintInfoCalculator(ActivePrinter);
				return printingInfo;
			}
		}
		protected internal new CardViewPrinter ActivePrinter {
			get {
				if(activePrinter == null)
					activePrinter = (CardViewPrinter)base.ActivePrinter;
				return activePrinter;
			}
		}
		protected override void CreateFirstPrinter(ASPxGridBase grid) {
			var graph = PrintingSystemBase != null ? PrintingSystemBase.Graph : null;
			Printers.Push(new CardViewPrinter(Exporter, graph));
		}
		public override void CreateArea(string areaName, IBrickGraphics graph) {
		}
		protected override void CreateDetailHeader(BrickGraphics graph) {
			ActivePrinter.CreateDetailHeader();
		}
		protected override void CreateDetail(BrickGraphics graph) {
			ActivePrinter.CreateDetail();
		}
	}
}
