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

using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.XtraPrintingLinks;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.UI {
	[
	XRDesigner("DevExpress.XtraReports.Design.WinControlContainerDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._WinControlContainerDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.WinControlContainer"),
	ToolboxItem(false),
	]
	public class WinControlContainer : XRControl {
		#region static
		const WinControlPrintMode DefaultPrintMode = WinControlPrintMode.Default;
		static readonly object WinControlChangedEvent = new object();
		#endregion
		#region hidden properties
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override PaddingInfo Padding {
			get { return base.Padding; }
			set { base.Padding = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override Color BackColor {
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override Color BorderColor {
			get { return base.BorderColor; }
			set { base.BorderColor = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override float BorderWidth {
			get { return base.BorderWidth; }
			set { base.BorderWidth = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override BorderDashStyle BorderDashStyle {
			get { return base.BorderDashStyle; }
			set { base.BorderDashStyle = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override BorderSide Borders {
			get { return base.Borders; }
			set { base.Borders = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override XRControlStyles Styles { get { return base.Styles; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override string StyleName { get { return ""; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override string EvenStyleName { get { return ""; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override string OddStyleName { get { return ""; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override XRBindingCollection DataBindings {
			get { return base.DataBindings; }
		}
		#endregion
		#region fields & properties
		Control winControl;
		ILink2 link;
		protected WindowControlOptions options = new WindowControlOptions();
		bool PossibleBandKind {
			get {
				return
					Band.BandKind == BandKind.ReportHeader ||
					Band.BandKind == BandKind.Detail ||
					Band.BandKind == BandKind.ReportFooter;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("WinControlContainerSyncBounds"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.WinControlContainer.SyncBounds"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(true),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool SyncBounds {
			get { return options.SyncBounds; }
			set { options.SyncBounds = value; }
		}
		[Browsable(false)]
		public virtual Control WinControl {
			get { return winControl; }
			set {
				if(Comparer.Equals(winControl, value))
					return;
				UnsubscribeWinControlEvents(winControl);
				RemoveControlFromDummyForm(winControl);
				winControl = value;
				SubscribeWinControlEvents(winControl);
				UpdateControlSize();
				OnWinControlChanged(EventArgs.Empty);
			}
		}
		protected internal override bool HasUndefinedBounds {
			get { return HasLink; }
		}
		internal bool HasLink { get { return Link != null; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("WinControlContainerDrawMethod"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.WinControlContainer.DrawMethod"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(WinControlDrawMethod.UseWMPaint)]
		public virtual WinControlDrawMethod DrawMethod {
			get { return options.DrawMethod; }
			set { options.DrawMethod = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("WinControlContainerImageType"),
#endif
 DefaultValue(WinControlImageType.Metafile),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.WinControlContainer.ImageType"),
		SRCategory(ReportStringId.CatBehavior),
		]
		public virtual WinControlImageType ImageType {
			get { return options.ImageType; }
			set { options.ImageType = value; }
		}
		protected ILink2 Link {
			get {
				if(link == null)
					link = CreateLink();
				return link;
			}
		}
		protected void ClearLink() {
			link = null;
		}
		protected virtual ILink2 CreateLink() {
			LinkBase link = LinkFactory.GetLinkOf(WinControl);
			if(link != null)
				link.SetDataObject(WinControl);
			return link;
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("WinControlContainerPrintMode"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.WinControlContainer.PrintMode"),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(DevExpress.XtraReports.Design.WinControlPrintModeConverter)),
		DefaultValue(DefaultPrintMode),
		]
		public virtual WinControlPrintMode PrintMode { get { return options.PrintMode; } set { options.PrintMode = value; } }
		#endregion
		internal override IList VisibleComponents {
			get {
				return WinControl != null ? new object[] { WinControl } :
					new object[] { };
			}
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("WinControlContainerWinControlChanged")]
#endif
		public virtual event EventHandler WinControlChanged {
			add { Events.AddHandler(WinControlChangedEvent, value); }
			remove { Events.RemoveHandler(WinControlChangedEvent, value); }
		}
		void OnWinControlChanged(EventArgs e) {
			EventHandler handler = (EventHandler)Events[WinControlChangedEvent];
			if(handler != null) handler(this, e);
		}
		public WinControlContainer()
			: base() {
			ParentStyleUsing.Assign(StyleUsing.CreateEmptyStyleUsing());
		}
		protected override void Dispose(bool disposing) {
			if(disposing && !IsDisposed) {
				UnsubscribeWinControlEvents(winControl);
				RemoveControlFromDummyForm(winControl);
			}
			base.Dispose(disposing);
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			if(WinControl != null)
				WinControl.DataBindings.Clear();
			serializer.SerializeReference("WinControl", WinControl);
			serializer.SerializeEnum("DrawMethod", DrawMethod);
			serializer.SerializeBoolean("SyncBounds", SyncBounds);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			winControl = serializer.DeserializeReference("WinControl", null) as Control;
			DrawMethod = (WinControlDrawMethod)serializer.DeserializeEnum("DrawMethod", typeof(WinControlDrawMethod), WinControlDrawMethod.UseWMPaint);
			SyncBounds = serializer.DeserializeBoolean("SyncBounds", true);
		}
		#endregion
		private void OnWinControlSizeChanged(object sender, EventArgs e) {
			if(sender is Control && !ReportIsLoading && SyncBounds) {
				UpdateSize((Control)sender);
			}
		}
		void UpdateSize(Control control) {
			if(control != null) {
				RectangleF rect = InflateBorderWidth(control.Bounds, GraphicsDpi.Pixel);
				rect = XRConvert.Convert(rect, GraphicsDpi.Pixel, Dpi);
				Size size = System.Drawing.Size.Round(rect.Size);
				if(SizeF != size)
					SizeF = size;
			}
		}
		protected override void OnSizeChanged(ChangeEventArgs e) {
			base.OnSizeChanged(e);
			UpdateControlSize();
		}
		void SubscribeWinControlEvents(Control control) {
			if(control != null)
				control.SizeChanged += new EventHandler(OnWinControlSizeChanged);
		}
		void UnsubscribeWinControlEvents(Control control) {
			if(control != null)
				control.SizeChanged -= new EventHandler(OnWinControlSizeChanged);
		}
		protected void UpdateControlSize() {
			if(ReportIsLoading)
				return;
			if(WinControl != null && SyncBounds) {
				RectangleF rect = DeflateBorderWidth(BoundsF);
				rect = XRConvert.Convert(rect, Dpi, GraphicsDpi.Pixel);
				Size size = System.Drawing.Size.Round(rect.Size);
				SetWinControlSize(size);
				if(WinControl.Size != size) { 
					UpdateSize(WinControl);
				}
			}
		}
		private void SetWinControlSize(Size size) {
			if(WinControl != null) {
				UnsubscribeWinControlEvents(WinControl);
				WinControl.Size = size;
				SubscribeWinControlEvents(WinControl);
			}
		}
		Image GetWinControlImage() {
			ValidateControlForm(WinControl);
			return XRControlPaint.GetControlImage(WinControl, WinControlDrawMethodConverter.ToUtilsWinControlDrawMethod(DrawMethod), WinControlImageTypeConverter.ToUtilsWinControlImageType(ImageType));
		}
		void AddControlToDummyForm(Control control) {
			XtraReport rootReport = RootReport;
			if(rootReport == null)
				return;
			try {
				rootReport.DummyForm.Controls.Add(control);
			} catch(ArgumentException ex) {
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
			}
		}
		void RemoveControlFromDummyForm(Control control) {
			XtraReport rootReport = RootReport;
			if(rootReport == null || control == null)
				return;
			Form form = rootReport.DummyForm;
			if(form.Controls.Contains(control))
				form.Controls.Remove(control);
		}
		private void ValidateControlForm(Control control) {
			if(control == null)
				return;
			Form form = null;
			if(control.Parent == null) {
				try {
					form = control.FindForm();
				} catch {
				}
				if(form == null) {
					AddControlToDummyForm(control);
					form = control.FindForm();
				}
			}
			if(form != null && RootReport != null && object.ReferenceEquals(form, RootReport.DummyForm))
				form.BackColor = RootReport.PageColor != Color.Transparent ? RootReport.PageColor : Color.White;
		}
		protected override void WriteContentToCore(XRWriteInfo writeInfo, VisualBrick brick) {
			if(brick is SubreportBrick) {
				ValidateControlForm(WinControl);
				AddSubreport(writeInfo, (SubreportBrick)brick, Link);
			}
			base.WriteContentToCore(writeInfo, brick);
		}
		protected static void AddSubreport(XRWriteInfo writeInfo, SubreportBrick brick, ILink2 linkBase) {
			SubreportDocumentBand subrepBand = new SubreportDocumentBand(brick.Rect);
			brick.DocumentBand = subrepBand;
			PrintingSystemBase ps = writeInfo.PrintingSystem;
			linkBase.AddSubreport(writeInfo.PrintingSystem, subrepBand, PointF.Empty);
			ps.Graph.PageUnit = GraphicsUnit.Document;
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			if(PrintAsBricks)
				return new SubreportBrick(this);
			return new ImageBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			if(brick is ImageBrick) {
				Image image = GetWinControlImage();
				if(image != null) {
					ImageBrick imageBrick = (ImageBrick)brick;
					imageBrick.Image = ps.Images.GetImage(image);
					imageBrick.Sides = BorderSide.None;
					imageBrick.Padding = PaddingInfo.Empty;
					imageBrick.SizeMode = ImageSizeMode.Normal;
				}
			}
		}
		protected override void CollectAssociatedComponents(DesignItemList components) {
			base.CollectAssociatedComponents(components);
			if(WinControl != null)
				components.Add(WinControl);
		}
		protected internal override void ValidateDataSource(object newSource, object oldSource, string dataMember) {
			base.ValidateDataSource(newSource, oldSource, dataMember);
			if(WinControl == null)
				return;
			PropertyInfo dataSourcePi = WinControl.GetType().GetProperty("DataSource");
			if(dataSourcePi == null)
				return;
			object dataSource = dataSourcePi.GetValue(WinControl, null);
			if(newSource != null && (dataSource == null || Object.Equals(dataSource, oldSource))) {
				dataSourcePi.SetValue(WinControl, newSource, null);
			}
		}
		protected bool PrintAsBricks {
			get {
				return HasLink && !DesignMode &&
						(PrintMode == WinControlPrintMode.AsBricks || (PrintMode == WinControlPrintMode.Default && PossibleBandKind));
			}
		}
	}
	[
	TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.WindowControlOptions"),
	]
	public class WindowControlOptions {
		WinControlDrawMethod drawMethod = WinControlDrawMethod.UseWMPaint;
		WinControlImageType imageType = WinControlImageType.Metafile;
		WinControlPrintMode printMode = WinControlPrintMode.Default;
		bool syncBounds = true;
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.WinControlContainer.PrintMode"),
		DefaultValue(WinControlPrintMode.Default),
		]
		public WinControlPrintMode PrintMode { get { return printMode; } set { printMode = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.WinControlContainer.SyncBounds"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(true),
		]
		public bool SyncBounds { get { return syncBounds; } set { syncBounds = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.WinControlContainer.ImageType"),
		DefaultValue(WinControlImageType.Metafile),
		]
		public WinControlImageType ImageType { get { return imageType; } set { imageType = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.WinControlContainer.DrawMethod"),
		DefaultValue(WinControlDrawMethod.UseWMPaint)
		]
		public WinControlDrawMethod DrawMethod { get { return drawMethod; } set { drawMethod = value; } }
		internal bool ShouldSerialize() {
			return drawMethod != WinControlDrawMethod.UseWMPaint ||
				imageType != WinControlImageType.Metafile ||
				printMode != WinControlPrintMode.Default ||
				syncBounds != true;
		}
	}
}
