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
using System.IO;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Internal;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Drawing.Printing;
using DevExpress.Xpf.ComponentModel;
using DevExpress.Xpf.Drawing;
using System.Windows.Input;
#else
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Runtime;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Localization;
#endif
namespace DevExpress.XtraPrinting.Native {
	public interface ILink2 : ILink {
		PaperKind PaperKind { get; set; }
		Margins Margins { get; set; }
		Margins MinMargins { get; set; }
		Size CustomPaperSize { get; set; }
		void AddSubreport(PrintingSystemBase ps, DocumentBand band, PointF offset);
	}
}
namespace DevExpress.XtraPrinting {
#if !SL
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DocumentSourceAttribute : Attribute {
		public static readonly DocumentSourceAttribute DocumentSource = new DocumentSourceAttribute(true);
		public static readonly DocumentSourceAttribute Default = NonDocumentSource;
		public static readonly DocumentSourceAttribute NonDocumentSource = new DocumentSourceAttribute(false);
		bool isDocumentSource;
		public bool IsDocumentSource {
			get {
				return isDocumentSource;
			}
		}
		public DocumentSourceAttribute()
			: this(true) {
		}
		public DocumentSourceAttribute(bool isDocumentSource) {
			this.isDocumentSource = isDocumentSource;
		}
		public override bool Equals(object obj) {
			if(obj == this) {
				return true;
			}
			DocumentSourceAttribute attribute = obj as DocumentSourceAttribute;
			return attribute != null && attribute.IsDocumentSource == IsDocumentSource;
		}
		public override int GetHashCode() {
			return isDocumentSource.GetHashCode();
		}
		public override bool IsDefaultAttribute() {
			return Equals(Default);
		}
	}
#endif
	public interface IDocumentSource : ILink {
		PrintingSystemBase PrintingSystemBase { get; }
	}
	[
	ToolboxItem(false),
	DesignTimeVisible(false)
	]
	public class LinkBase : Component, ILink2, ICommandHandler {
		protected delegate void CreateAreaDelegate(BrickGraphics gr);
		#region Fields & Properties
		protected PrintingSystemBase ps;
		protected bool fLandscape;
		protected Margins fMargins = XtraPageSettingsBase.DefaultMargins;
		protected Margins fMinMargins = XtraPageSettingsBase.DefaultMinMargins;
		protected BrickModifier skipModifier = BrickModifier.None;
		protected PaperKind fPaperKind = XtraPageSettingsBase.DefaultPaperKind;
		protected Size fCustomPaperSize = Size.Empty;
		GraphicsUnit initialPageUnit;
		bool initialDip;
		VerticalContentSplitting fVerticalContentSplitting = VerticalContentSplitting.Smart;
		private LinkBase owner;
		protected PageHeaderFooter pageHF = new PageHeaderFooter();
		protected bool fEnablePageDialog = true;
		protected string fPaperName = XtraPageSettingsBase.DefaultPaperName;
		PrintingSystemActivity activity = PrintingSystemActivity.Idle;
		int subreportLevel = 0;
		protected virtual string InnerPageHeader { get { return string.Empty; } }
		protected virtual string InnerPageFooter { get { return string.Empty; } }
		protected virtual string ReportHeader { get { return RtfReportHeader; } }
		protected virtual string ReportFooter { get { return RtfReportFooter; } }
		[Browsable(false)]
		public PrintingSystemActivity Activity { get { return activity; } }
		bool IsDocumentEmpty { get { return PrintingSystemBase.Document.IsEmpty; } }
		ExportOptions ExportOptions {
			get {
				return PrintingSystemBase.ExportOptions;
			}
		}
		[
		Category(NativeSR.CatPageLayout),
		DefaultValue("")
		]
		public string PaperName {
			get { return fPaperName; }
			set { fPaperName = value; }
		}
		[
		DefaultValue(true),
		Category(NativeSR.CatHeadersFooters),
		]
		public bool EnablePageDialog {
			get { return fEnablePageDialog; }
			set { fEnablePageDialog = value; }
		}
		[
		Browsable(true),
		Category(NativeSR.CatHeadersFooters),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
#if !SL
		Editor("DevExpress.XtraPrinting.Design.PageHeaderFooterEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor)),
#endif
		]
		public virtual object PageHeaderFooter {
			get {
				return pageHF;
			}
			set {
				if(value is PageHeaderFooter)
					pageHF = (PageHeaderFooter)value;
			}
		}
		[
		Category(NativeSR.CatHeadersFooters),
		DefaultValue(""),
#if !SL
 Editor("DevExpress.XtraPrinting.Design.RichTextEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor)),
#endif
		]
		public string RtfReportHeader { 
			get;
			set;
		}
		[
		Category(NativeSR.CatHeadersFooters),
		DefaultValue(""),
#if !SL
 Editor("DevExpress.XtraPrinting.Design.RichTextEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor)),
#endif
		]
		public string RtfReportFooter { 
			get; 
			set; 
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LinkBase Owner {
			get { return owner; }
			set { owner = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBasePrintableObjectType"),
#endif
		Browsable(false),
		]
		public virtual Type PrintableObjectType {
			get { return typeof(object); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseSkipArea"),
#endif
		Browsable(true),
		Category(NativeSR.CatPrinting),
		DefaultValue(BrickModifier.None),
#if !SL
		Editor("DevExpress.XtraPrinting.Design.SelectAreaEditor," + AssemblyInfo.SRAssemblyPrintingDesign, typeof(System.Drawing.Design.UITypeEditor)),
#endif
		]
		public BrickModifier SkipArea {
			get { return skipModifier; }
			set { skipModifier = value; }
		}
		[
		Category(NativeSR.CatPrinting),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseVerticalContentSplitting"),
#endif
		DefaultValue(VerticalContentSplitting.Smart)
		]
		public VerticalContentSplitting VerticalContentSplitting {
			get { return fVerticalContentSplitting; }
			set { fVerticalContentSplitting = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBasePaperKind"),
#endif
		Category(NativeSR.CatPageLayout),
		]
		public PaperKind PaperKind {
			get { return fPaperKind; }
			set { fPaperKind = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseLandscape"),
#endif
		Category(NativeSR.CatPageLayout),
		DefaultValue(false)
		]
		public bool Landscape {
			get { return fLandscape; }
			set { fLandscape = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseMinMargins"),
#endif
		Category(NativeSR.CatPageLayout),
		]
		public Margins MinMargins {
			get { return fMinMargins; }
			set { fMinMargins = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseMargins"),
#endif
		Category(NativeSR.CatPageLayout),
		]
		public Margins Margins {
			get { return fMargins; }
			set { fMargins = value; }
		}
		[Browsable(false)]
		public Size CustomPaperSize {
			get { return fCustomPaperSize; }
			set { fCustomPaperSize = value; }
		}
		bool ShouldSerializeCustomPaperSize() {
			return fCustomPaperSize != Size.Empty;
		}
		[
		Browsable(false)
		]
		public virtual PrintingSystemBase PrintingSystemBase {
			get { return ps; }
			set { ps = value; }
		}
		protected virtual BrickModifier InternalSkipArea {
			get { return BrickModifier.ReportHeader | BrickModifier.ReportFooter; }
		}
		bool AllowRtfHederFooter {
			get { return subreportLevel == 0 || subreportLevel == 1 && Owner != null; }
		}
		IPrintingSystem ILink.PrintingSystem { get { return PrintingSystemBase; } }
		#endregion
		#region Events
		private static readonly object BeforeCreateEvent = new object();
		private static readonly object AfterCreateEvent = new object();
		private static readonly object CreateMarginalHeaderEvent = new object();
		private static readonly object CreateMarginalFooterEvent = new object();
		private static readonly object CreateInnerPageHeaderEvent = new object();
		private static readonly object CreateInnerPageFooterEvent = new object();
		private static readonly object CreateReportHeaderEvent = new object();
		private static readonly object CreateReportFooterEvent = new object();
		private static readonly object CreateDetailHeaderEvent = new object();
		private static readonly object CreateDetailFooterEvent = new object();
		private static readonly object CreateDetailEvent = new object();
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseCreateMarginalHeaderArea"),
#endif
 Category(NativeSR.CatReportArea)]
		public event CreateAreaEventHandler CreateMarginalHeaderArea {
			add { Events.AddHandler(CreateMarginalHeaderEvent, value); }
			remove { Events.RemoveHandler(CreateMarginalHeaderEvent, value); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseCreateMarginalFooterArea"),
#endif
 Category(NativeSR.CatReportArea)]
		public event CreateAreaEventHandler CreateMarginalFooterArea {
			add { Events.AddHandler(CreateMarginalFooterEvent, value); }
			remove { Events.RemoveHandler(CreateMarginalFooterEvent, value); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseCreateInnerPageHeaderArea"),
#endif
 Category(NativeSR.CatReportArea)]
		public virtual event CreateAreaEventHandler CreateInnerPageHeaderArea {
			add { Events.AddHandler(CreateInnerPageHeaderEvent, value); }
			remove { Events.RemoveHandler(CreateInnerPageHeaderEvent, value); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseCreateInnerPageFooterArea"),
#endif
 Category(NativeSR.CatReportArea)]
		public virtual event CreateAreaEventHandler CreateInnerPageFooterArea {
			add { Events.AddHandler(CreateInnerPageFooterEvent, value); }
			remove { Events.RemoveHandler(CreateInnerPageFooterEvent, value); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseCreateReportHeaderArea"),
#endif
 Category(NativeSR.CatReportArea)]
		public event CreateAreaEventHandler CreateReportHeaderArea {
			add { Events.AddHandler(CreateReportHeaderEvent, value); }
			remove { Events.RemoveHandler(CreateReportHeaderEvent, value); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseCreateReportFooterArea"),
#endif
 Category(NativeSR.CatReportArea)]
		public event CreateAreaEventHandler CreateReportFooterArea {
			add { Events.AddHandler(CreateReportFooterEvent, value); }
			remove { Events.RemoveHandler(CreateReportFooterEvent, value); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseCreateDetailHeaderArea"),
#endif
 Category(NativeSR.CatReportArea)]
		public virtual event CreateAreaEventHandler CreateDetailHeaderArea {
			add { Events.AddHandler(CreateDetailHeaderEvent, value); }
			remove { Events.RemoveHandler(CreateDetailHeaderEvent, value); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseCreateDetailFooterArea"),
#endif
 Category(NativeSR.CatReportArea)]
		public virtual event CreateAreaEventHandler CreateDetailFooterArea {
			add { Events.AddHandler(CreateDetailFooterEvent, value); }
			remove { Events.RemoveHandler(CreateDetailFooterEvent, value); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseCreateDetailArea"),
#endif
 Category(NativeSR.CatReportArea)]
		public virtual event CreateAreaEventHandler CreateDetailArea {
			add { Events.AddHandler(CreateDetailEvent, value); }
			remove { Events.RemoveHandler(CreateDetailEvent, value); }
		}
		void OnCreateMarginalHeader(CreateAreaEventArgs e) {
			CreateAreaEventHandler eventDelegate = (CreateAreaEventHandler)Events[CreateMarginalHeaderEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		void OnCreateMarginalFooter(CreateAreaEventArgs e) {
			CreateAreaEventHandler eventDelegate = (CreateAreaEventHandler)Events[CreateMarginalFooterEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		void OnCreateInnerPageHeader(CreateAreaEventArgs e) {
			CreateAreaEventHandler eventDelegate = (CreateAreaEventHandler)Events[CreateInnerPageHeaderEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		void OnCreateInnerPageFooter(CreateAreaEventArgs e) {
			CreateAreaEventHandler eventDelegate = (CreateAreaEventHandler)Events[CreateInnerPageFooterEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		void OnCreateReportHeader(CreateAreaEventArgs e) {
			CreateAreaEventHandler eventDelegate = (CreateAreaEventHandler)Events[CreateReportHeaderEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		void OnCreateReportFooter(CreateAreaEventArgs e) {
			CreateAreaEventHandler eventDelegate = (CreateAreaEventHandler)Events[CreateReportFooterEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		void OnCreateDetailHeader(CreateAreaEventArgs e) {
			CreateAreaEventHandler eventDelegate = (CreateAreaEventHandler)Events[CreateDetailHeaderEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		void OnCreateDetailFooter(CreateAreaEventArgs e) {
			CreateAreaEventHandler eventDelegate = (CreateAreaEventHandler)Events[CreateDetailFooterEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		void OnCreateDetail(CreateAreaEventArgs e) {
			CreateAreaEventHandler eventDelegate = (CreateAreaEventHandler)Events[CreateDetailEvent];
			if(eventDelegate != null) eventDelegate(this, e);
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseBeforeCreateAreas"),
#endif
 Category(NativeSR.CatReportArea)]
		public event EventHandler BeforeCreateAreas {
			add { Events.AddHandler(BeforeCreateEvent, value); }
			remove { Events.RemoveHandler(BeforeCreateEvent, value); }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("LinkBaseAfterCreateAreas"),
#endif
 Category(NativeSR.CatReportArea)]
		public event EventHandler AfterCreateAreas {
			add { Events.AddHandler(AfterCreateEvent, value); }
			remove { Events.RemoveHandler(AfterCreateEvent, value); }
		}
		protected virtual void OnBeforeCreate(EventArgs e) {
			PrintingSystemBase.AddCommandHandler(this);
			EventHandler eventDelegate = (EventHandler)Events[BeforeCreateEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		protected virtual void OnAfterCreate(EventArgs e) {
			EventHandler eventDelegate = (EventHandler)Events[AfterCreateEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		#endregion
		public LinkBase() {
			RtfReportHeader = string.Empty;
			RtfReportFooter = string.Empty;
		}
		public LinkBase(PrintingSystemBase ps) : this() {
			this.ps = ps;
		}
		public LinkBase(System.ComponentModel.IContainer container) : this() {
			container.Add(this);
		}
		public virtual void SetDataObject(object data) {
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(ps != null) {
					ps.FinalizeLink(this);
					ps = null;
				}
			}
			base.Dispose(disposing);
		}
		protected bool ShouldSerializePageHeaderFooter() {
			return (pageHF != null && pageHF.ShouldSerialize());
		}
		protected bool ShouldSerializePaperKind() {
			return (fPaperKind.Equals(XtraPageSettingsBase.DefaultPaperKind) == false);
		}
		protected bool ShouldSerializeMinMargins() {
			return (fMinMargins.Equals(XtraPageSettingsBase.DefaultMinMargins) == false);
		}
		protected bool ShouldSerializeMargins() {
			return (fMargins.Equals(XtraPageSettingsBase.DefaultMargins) == false);
		}
		public virtual void CreateDocument(PrintingSystemBase ps) {
			if(ps != null) {
				this.ps = ps;
				CreateDocument();
			}
		}
		public void ClearDocument() {
			PrintingSystemBase.ClearContent();
		}
		public virtual void CreateDocument() {
			ExecuteActivity(PrintingSystemActivity.Preparing, delegate() {
				CreateDocument(false);
			});
		}
		public virtual void CreateDocument(bool buildPagesInBackground) {
			if(owner != null) {
				owner.CreateDocument();
				return;
			}
			if(ps == null || ps.Locked)
				return;
			if(ps.RaisingAfterChange) {
				ClearDocument();
				ps.DelayedAction |= PrintingSystemAction.CreateDocument;
				return;
			}
			((ISupportInitialize)ps).BeginInit();
			PrintingDocument printingDocument = ps.Document as PrintingDocument;
			if(printingDocument != null)
				printingDocument.VerticalContentSplitting = VerticalContentSplitting;
			if(!ps.PageSettings.IsPresetted)
				ApplyPageSettings();
			ps.PageSettings.IsPresetted = false;
			ps.Begin();
			OnBeforeCreate(EventArgs.Empty);
			BeforeCreate();
			try {
				CreateReportArea(new CreateAreaDelegate(CreateMarginalHeader), ps.Graph, BrickModifier.MarginalHeader);
				CreateReportArea(new CreateAreaDelegate(CreateMarginalFooter), ps.Graph, BrickModifier.MarginalFooter);
				CreateReportArea(new CreateAreaDelegate(CreateInnerPageHeader), ps.Graph, BrickModifier.InnerPageHeader);
				CreateReportArea(new CreateAreaDelegate(CreateInnerPageFooter), ps.Graph, BrickModifier.InnerPageFooter);
				CreateReportArea(new CreateAreaDelegate(CreateReportHeader), ps.Graph, BrickModifier.ReportHeader);
				CreateDocumentCore();
				CreateReportArea(new CreateAreaDelegate(CreateReportFooter), ps.Graph, BrickModifier.ReportFooter);
			} finally {
				OnAfterCreate(EventArgs.Empty);
				AfterCreate();
				ps.End(this, buildPagesInBackground);
				((ISupportInitialize)ps).EndInit();
			}
		}
		protected virtual void CreateDocumentCore() {
			BrickModifier saveSkipArea = this.SkipArea;
			this.SkipArea |= InternalSkipArea;
			CreateReportArea(new CreateAreaDelegate(CreateDetailHeader), ps.Graph, BrickModifier.DetailHeader);
			CreateReportArea(new CreateAreaDelegate(CreateDetail), ps.Graph, BrickModifier.Detail);
			CreateReportArea(new CreateAreaDelegate(CreateDetailFooter), ps.Graph, BrickModifier.DetailFooter);
			this.SkipArea = saveSkipArea;
		}
		protected virtual void ApplyPageSettings() {
			if(!ApplyPageSettingsCore())
				PrintingSystemBase.PageSettings.Assign(GetValidMargins(fMargins, new Margins(0, 0, 0, 0)), fPaperKind, fPaperName, fLandscape);
		}
		Margins GetValidMargins(Margins margins, Margins minMargins) {
			if(pageHF != null && pageHF.IncreaseMarginsByContent) {
				Image[] images = GetImageArray();
				SizeF headerSize = GraphicsUnitConverter.Convert(pageHF.MeasureMarginalHeader(ps.Graph, images), ps.Graph.Dpi, GraphicsDpi.HundredthsOfAnInch);
				SizeF footerSize = GraphicsUnitConverter.Convert(pageHF.MeasureMarginalFooter(ps.Graph, images), ps.Graph.Dpi, GraphicsDpi.HundredthsOfAnInch);
				Margins validMargins = new Margins(margins.Left, margins.Right,
					(int)Math.Max(margins.Top, headerSize.Height + minMargins.Top),
					(int)Math.Max(margins.Bottom, footerSize.Height + minMargins.Bottom));
				return validMargins;
			}
			return margins;
		}
		protected bool ApplyPageSettingsCore() {
			return XtraPageSettingsBase.ApplyPageSettings(ps.PageSettings, fPaperKind, fCustomPaperSize, GetValidMargins(fMargins, fMinMargins), fMinMargins, fLandscape, fPaperName);
		}
		public virtual void AddSubreport(PrintingSystemBase ps, DocumentBand band, PointF offset) {
			this.ps = ps;
			AddSubreportInternal(band, offset);
		}
		public virtual void AddSubreport(PointF offset) {
			DocumentBand reportContainer = ps.Document.AddReportContainer();
			reportContainer.AddBand(new DocumentBand(DocumentBandKind.ReportHeader));
			DocumentBand docBand = new DocumentBandContainer();
			reportContainer.AddBand(docBand);
			docBand.TopSpan = offset.Y;
			reportContainer.AddBand(new DocumentBand(DocumentBandKind.ReportFooter));
			AddSubreportInternal(docBand, PointF.Empty);
		}
		void AddSubreportInternal(DocumentBand docBand, PointF offset) {
			if(ps == null)
				return;
			BrickGraphics gr = ps.Graph;
			BrickModifier modifier = gr.Modifier;
			subreportLevel++;
			try {
				ps.BeginSubreportInternal(docBand, offset);
				BeforeCreate();
				CreateReportArea(new CreateAreaDelegate(CreateReportHeader), gr, BrickModifier.ReportHeader);
				BrickModifier saveSkipArea = this.SkipArea;
				this.SkipArea |= InternalSkipArea;
				CreateReportArea(new CreateAreaDelegate(CreateDetailHeader), gr, BrickModifier.DetailHeader);
				CreateReportArea(new CreateAreaDelegate(CreateDetail), gr, BrickModifier.Detail);
				CreateReportArea(new CreateAreaDelegate(CreateDetailFooter), gr, BrickModifier.DetailFooter);
				this.SkipArea = saveSkipArea;
				CreateReportArea(new CreateAreaDelegate(CreateReportFooter), gr, BrickModifier.ReportFooter);
				AfterCreate();
				ps.EndSubreport();
			} finally {
				gr.Modifier = modifier;
				subreportLevel--;
			}
		}
		private void CreateReportArea(CreateAreaDelegate proc, BrickGraphics gr, BrickModifier modifier) {
			gr.Modifier = modifier;
			if(EnableCreate(gr.Modifier)) {
				proc(gr);
			}
		}
		private bool EnableCreate(BrickModifier modifier) {
			return ((modifier & skipModifier) == 0 || modifier == BrickModifier.Detail);
		}
		internal void UpdatePageSettingsInternal() {
			UpdatePageSettings();
		}
		protected virtual void UpdatePageSettings() {
			try {
				fMargins = ps.PageSettings.Margins;
				fLandscape = ps.PageSettings.Landscape;
				fPaperKind = ps.PageSettings.PaperKind;
				fPaperName = PrintingSystemBase.PageSettings.PaperName;
			} catch { }
		}
		protected virtual void BeforeCreate() {
			initialPageUnit = ps.Graph.PageUnit;
			initialDip = ps.Graph.DeviceIndependentPixel;
			if((DesignMode || EnablePageDialog) && (PrintingSystemBase.Document is PSLinkDocument))
				EnableCommand(PrintingSystemCommand.EditPageHF, true);
		}
		protected void EnableCommand(PrintingSystemCommand command, bool enabled) {
			PrintingSystemBase.SetCommandVisibility(command, enabled ? CommandVisibility.All : CommandVisibility.None, Priority.Low);
			PrintingSystemBase.EnableCommandInternal(command, enabled);
		}
		protected virtual void AfterCreate() {
			ps.Graph.PageUnit = initialPageUnit;
			ps.Graph.DeviceIndependentPixel = initialDip;
		}
		internal virtual void BeforeDestroy() {
			DisableCommands();
		}
		protected virtual void DisableCommands() {
			EnableCommand(PrintingSystemCommand.EditPageHF, false);
		}
		protected virtual void CreateMarginalHeader(BrickGraphics graph) {
			if(pageHF != null)
				pageHF.CreateMarginalHeader(graph, GetImageArray());
			OnCreateMarginalHeader(new CreateAreaEventArgs(graph));
		}
		protected virtual void CreateMarginalFooter(BrickGraphics graph) {
			if(pageHF != null)
				pageHF.CreateMarginalFooter(graph, GetImageArray());
			OnCreateMarginalFooter(new CreateAreaEventArgs(graph));
		}
		protected virtual Image[] GetImageArray() {
			return new Image[0];
		}
		protected virtual void CreateInnerPageHeader(BrickGraphics graph) {
			DrawBrick(graph, InnerPageHeader);
			OnCreateInnerPageHeader(new CreateAreaEventArgs(graph));
		}
		protected virtual void CreateInnerPageFooter(BrickGraphics graph) {
			DrawBrick(graph, InnerPageFooter);
			OnCreateInnerPageFooter(new CreateAreaEventArgs(graph));
		}
		protected virtual void CreateDetailHeader(BrickGraphics graph) {
			OnCreateDetailHeader(new CreateAreaEventArgs(graph));
		}
		protected virtual void CreateDetailFooter(BrickGraphics graph) {
			OnCreateDetailFooter(new CreateAreaEventArgs(graph));
		}
		protected virtual void CreateReportHeader(BrickGraphics graph) {
			if(AllowRtfHederFooter)
				DrawBrick(graph, ReportHeader);
			OnCreateReportHeader(new CreateAreaEventArgs(graph));
		}
		protected virtual void CreateDetail(BrickGraphics graph) {
			OnCreateDetail(new CreateAreaEventArgs(graph));
		}
		protected virtual void CreateReportFooter(BrickGraphics graph) {
			if(AllowRtfHederFooter)
				DrawBrick(graph, ReportFooter);
			OnCreateReportFooter(new CreateAreaEventArgs(graph));
		}
		void DrawBrick(BrickGraphics graph, string rtf) {
			if(!string.IsNullOrEmpty(rtf) && this.ps != null) {
				IRichTextBrick brick = ((IPrintingSystem)ps).CreateRichTextBrick();
				VisualBrickHelper.SetBrickBounds((VisualBrick)brick, new RectangleF(0, 0, graph.ClientPageSize.Width, brick.InfiniteHeight), graph.Dpi);
				((VisualBrick)brick).Style = new BrickStyle(BorderSide.None, 0, DXColor.Empty, DXColor.Empty, DXColor.Black, BrickStyle.DefaultFont, new BrickStringFormat());
				brick.RtfText = rtf;
				float height = GraphicsUnitConverter.Convert((float)brick.EffectiveHeight, GraphicsDpi.Document, graph.Dpi);
				graph.DrawBrick((VisualBrick)brick, new RectangleF(0, 0, graph.ClientPageSize.Width, height));
			}
		}
		#region Save Restore PageHeaderFooter
		public void SavePageHeaderFooterToXml(string xmlFile) {
			SavePageHeaderFooterCore(new XmlXtraSerializer(), xmlFile);
		}
		public void RestorePageHeaderFooterFromXml(string xmlFile) {
			RestorePageHeaderFooterCore(new XmlXtraSerializer(), xmlFile);
		}
		public void SavePageHeaderFooterToRegistry(string path) {
			SavePageHeaderFooterCore(new RegistryXtraSerializer(), path);
		}
		public void RestorePageHeaderFooterFromRegistry(string path) {
			RestorePageHeaderFooterCore(new RegistryXtraSerializer(), path);
		}
		public void SavePageHeaderFooterToStream(Stream stream) {
			SavePageHeaderFooterCore(new XmlXtraSerializer(), stream);
		}
		public void RestorePageHeaderFooterFromStream(Stream stream) {
			RestorePageHeaderFooterCore(new XmlXtraSerializer(), stream);
		}
		void SavePageHeaderFooterCore(XtraSerializer serializer, object path) {
			serializer.SerializeObject(PageHeaderFooter, path, "XtraPrintingPageHeaderFooter");
		}
		void RestorePageHeaderFooterCore(XtraSerializer serializer, object path) {
			serializer.DeserializeObject(PageHeaderFooter, path, "XtraPrintingPageHeaderFooter");
		}
		#endregion
		#region ICommandHandler members
		public virtual void HandleCommand(PrintingSystemCommand command, object[] args, IPrintControl printControl, ref bool handled) {
		}
		public virtual bool CanHandleCommand(PrintingSystemCommand command, IPrintControl printControl) {
			return false;
		}
		#endregion // ICommandHandler members
		protected virtual bool CanHandleCommandInternal(PrintingSystemCommand command, IPrintControl printControl) {
			return command == PrintingSystemCommand.EditPageHF;
		}
#if !SL
		#region export to xls
		public void ExportToXls(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToXls(filePath, ExportOptions.Xls);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToXls(filePath, options);
			});
		}
		public void ExportToXls(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToXls(stream, ExportOptions.Xls);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToXls(stream, options);
			});
		}
		#endregion
		#region export to xlsx
		public void ExportToXlsx(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToXlsx(filePath, ExportOptions.Xlsx);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToXlsx(filePath, options);
			});
		}
		public void ExportToXlsx(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToXlsx(stream, ExportOptions.Xlsx);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToXlsx(stream, options);
			});
		}
		#endregion
		#region export to image
		public void ExportToImage(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToImage(filePath, ExportOptions.Image);
		}
		public void ExportToImage(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToImage(stream, ExportOptions.Image);
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToImage(filePath, options);
			});
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToImage(stream, options);
			});
		}
		#endregion
		#region export to rtf
		public void ExportToRtf(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToRtf(filePath, ExportOptions.Rtf);
		}
		public void ExportToRtf(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToRtf(stream, ExportOptions.Rtf);
		}
		public void ExportToRtf(string filePath, RtfExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToRtf(filePath, options);
			});
		}
		public void ExportToRtf(Stream stream, RtfExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToRtf(stream, options);
			});
		}
		#endregion
		#region export to html
		public void ExportToHtml(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToHtml(filePath, ExportOptions.Html);
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToHtml(filePath, options);
			});
		}
		public void ExportToHtml(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToHtml(stream, ExportOptions.Html);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToHtml(stream, options);
			});
		}
		#endregion
		#region export to mht
		public void ExportToMht(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToMht(filePath, ExportOptions.Mht);
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToMht(filePath, options);
			});
		}
		public void ExportToMht(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToMht(stream, ExportOptions.Mht);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToMht(stream, options);
			});
		}
		#endregion
		#region export to pdf
		public void ExportToPdf(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToPdf(filePath, ExportOptions.Pdf);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToPdf(filePath, options);
			});
		}
		public void ExportToPdf(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToPdf(stream, ExportOptions.Pdf);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToPdf(stream, options);
			});
		}
		#endregion
		#region export to text
		public void ExportToText(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToText(filePath, ExportOptions.Text);
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToText(filePath, options);
			});
		}
		public void ExportToText(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToText(stream, ExportOptions.Text);
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToText(stream, options);
			});
		}
		#endregion
		#region export to csv
		public void ExportToCsv(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToCsv(filePath, ExportOptions.Csv);
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToCsv(filePath, options);
			});
		}
		public void ExportToCsv(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToCsv(stream, ExportOptions.Csv);
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			ExecuteExport(PrintingSystemActivity.Exporting, options, delegate() {
				PrintingSystemBase.ExportToCsv(stream, options);
			});
		}
		#endregion
		protected void ExecuteExport(PrintingSystemActivity activity, ExportOptionsBase options, Action0 callback) {
			if(IsDocumentEmpty) {
				activity |= PrintingSystemActivity.Preparing;
				ExecuteActivity(activity, delegate() {
					CreateDocument(false);
					if(!IsDocumentEmpty)
						callback();
				});
			} else 
				ExecuteActivity(activity, delegate() {
					callback();
				});
		}
#endif
		internal void ExecuteActivity(PrintingSystemActivity activity, Action0 callback) {
			PrintingSystemBase.ResetCancelPending();
			this.activity = activity;
			OnStartActivity();
			try {
				callback();
			} finally {
				OnEndActivity();
			}
		}
		protected virtual void OnStartActivity() { 
		}
		protected virtual void OnEndActivity() {
		}
	}
}
