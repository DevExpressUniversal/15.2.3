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
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.Utils.Drawing;
using DevExpress.XtraReports.Native;
using DevExpress.XtraScheduler.Drawing;
using System.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraReports.Native.Printing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraReports;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Services;
using System.Drawing.Printing;
using DevExpress.XtraScheduler.Printing;
using DevExpress.XtraScheduler.Printing.Native;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.Utils.Serializing;
using System.Linq;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraScheduler.Reporting {
	public class ResFinder {
	}
	public enum ControlContentLayoutType { Fit, Tile };
	#region ReportViewControlBase
	[XRDesigner("DevExpress.XtraScheduler.Reporting.Design.ReportViewControlDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions),
  Designer("DevExpress.XtraScheduler.Reporting.Design.ReportViewControlDesigner_," + AssemblyInfo.SRAssemblySchedulerDesign),
	DXToolboxItem(false),
	ToolboxItemFilter(DevExpress.XtraReports.Design.AttributeSR.SchedulerToolboxItem, ToolboxItemFilterType.Require),
	]
	public abstract class ReportViewControlBase : XRControl, IPrintable, ISupportCustomDraw {
		#region Constants
		const bool DefaultCanShrink = false;
		const bool DefaultCanGrow = false;
		#endregion
		#region Fields
		PrintableComponentLinkBase link;
		BaseHeaderAppearance appearance;
		ReportViewBase view;
		PrintColorSchemaOptions printColorSchemas;
		ControlLayoutOptions layoutOptionsHorizontal;
		ControlLayoutOptions layoutOptionsVertical;
		SchedulerPrinterParameters printerParameters;
		ControlPrintControllerBase printController;
		ViewInfoPainterBase painter;
		AnchorCollection actualHorizontalAnchors;
		AnchorCollection actualVerticalAnchors;
		Rectangle actualPrintBounds;
		ControlCornersOptions cornersOptions;
		ControlPrintInfo printInfo;
		#endregion
		#region Ctors
		protected ReportViewControlBase()
			: base() {
			Initialize();
		}
		protected ReportViewControlBase(ReportViewBase view)
			: base() {
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			View = view;
			Initialize();
		}
		#endregion
		#region Base Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Font Font { get { return base.Font; } set { base.Font = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override XRControl.XRControlStyles Styles { get { return base.Styles; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override StylePriority StylePriority { get { return base.StylePriority; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override TextAlignment TextAlignment { get { return base.TextAlignment; } set { base.TextAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PaddingInfo Padding { get { return base.Padding; } set { base.Padding = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new XRControlScripts Scripts { get { return base.Scripts; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FormattingRuleCollection FormattingRules { get { return base.FormattingRules; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAnchorStyles AnchorVertical { get { return base.AnchorVertical; } set { base.AnchorVertical = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string StyleName { get { return string.Empty; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string EvenStyleName { get { return string.Empty; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string OddStyleName { get { return string.Empty; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool WordWrap { get { return base.WordWrap; } set { base.WordWrap = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(DefaultCanShrink)]
		public override bool CanShrink { get { return base.CanShrink; } set { base.CanShrink = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(DefaultCanGrow)]
		public override bool CanGrow { get { return base.CanGrow; } set { base.CanGrow = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool KeepTogether { get { return base.KeepTogether; } set { base.KeepTogether = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override XRBindingCollection DataBindings { get { return base.DataBindings; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string XlsxFormatString { get { return base.XlsxFormatString; } set { base.XlsxFormatString = value; } }
		#endregion
		#region Properties
		protected internal virtual Type[] SupportedViewTypes { get { return new Type[] { typeof(ReportViewBase) }; } }
		protected override bool HasUndefinedBounds { get { return true; } }
		internal bool IsDesignMode { get { return DesignMode; } }
		protected internal virtual XtraSchedulerReport SchedulerReport { get { return RootReport as XtraSchedulerReport; } }
		protected internal int VisibleIntervalCount { get { return View != null ? View.VisibleIntervalCount : 1; } }
		protected internal ViewInfoPainterBase Painter { get { return painter; } }
		protected internal BaseHeaderAppearance Appearance { get { return appearance; } }
		[Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PrintColorSchemaOptions PrintColorSchemas { get { return printColorSchemas; } }
		protected internal ControlLayoutOptions LayoutOptionsHorizontal { get { return layoutOptionsHorizontal; } }
		protected internal ControlLayoutOptions LayoutOptionsVertical { get { return layoutOptionsVertical; } }
		protected internal bool HasHorizontalMaster { get { return LayoutOptionsHorizontal.MasterControl != null; } }
		protected internal bool HasVerticalMaster { get { return LayoutOptionsVertical.MasterControl != null; } }
		protected internal AnchorCollection ActualHorizontalAnchors { get { return actualHorizontalAnchors; } }
		protected internal AnchorCollection ActualVerticalAnchors { get { return actualVerticalAnchors; } }
		protected internal Rectangle ActualPrintBounds { get { return actualPrintBounds; } set { actualPrintBounds = value; } }
		protected ControlPrintInfo PrintInfo { get { return printInfo; } set { printInfo = value; } }
		internal ControlPrintInfo PublicPrintInfo { get { return PrintInfo; } }
		[Category(SRCategoryNames.Scheduler), DefaultValue(null)]
		public virtual ReportViewBase View {
			get { return view; }
			set {
				if (view == value)
					return;
				UnsubscribeViewEvents();
				SetView(value);
				SubscribeViewEvents();
			}
		}
		internal ControlCornersOptions CornersOptionsInternal { get { return cornersOptions; } }
		protected internal ControlPrintControllerBase PrintController { get { return printController; } }
		protected internal PrintableComponentLinkBase Link {
			get {
				if (link == null) {
					link = new PrintableComponentLinkBase();
					link.SetDataObject(this);
				}
				return link;
			}
		}
		protected SchedulerPrinterParameters PrinterParameters { get { return printerParameters; } }
		GraphicsInfo tempInfo;
		public virtual void SetGInfo(GraphicsInfo info) {
			this.tempInfo = info;
		}
		protected GraphicsInfo GInfo { get { return (tempInfo == null) ? printerParameters.GInfo : tempInfo; } }
		protected internal GraphicsCache Cache { get { return GInfo.Cache; } }
		protected virtual BorderSide DefaultBorders { get { return BorderSide.All; } }
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					UnsubscribeViewEvents();
					if (layoutOptionsHorizontal != null) {
						layoutOptionsHorizontal = null;
					}
					if (layoutOptionsVertical != null) {
						layoutOptionsVertical = null;
					}
					if (cornersOptions != null)
						cornersOptions = null;
					if (appearance != null) {
						appearance.Dispose();
						appearance = null;
					}
					DisposePrintInfo();
					painter = null;
					view = null;
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected void DisposePrintInfo() {
			if (printInfo != null) {
				printInfo.Dispose();
				printInfo = null;
			}
		}
		#endregion
		#region IPrintable implementation
		System.Windows.Forms.UserControl IPrintable.PropertyEditorControl { get { return null; } }
		bool IPrintable.CreatesIntersectedBricks {
			get { return true; }
		}
		void IPrintable.AcceptChanges() {
		}
		void IPrintable.RejectChanges() {
		}
		void IPrintable.ShowHelp() {
		}
		bool IPrintable.SupportsHelp() {
			return false;
		}
		bool IPrintable.HasPropertyEditor() {
			return false;
		}
		#endregion
		#region IBasePrintable implementation
		public void Initialize(IPrintingSystem ps, ILink link) {
			printerParameters = new SchedulerPrinterParameters(ps, link);
		}
		public void Finalize(IPrintingSystem ps, ILink link) {
			if (printerParameters != null) {
				printerParameters.Dispose();
				printerParameters = null;
			}
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graphics) {
			switch (areaName) {
				case DevExpress.XtraPrinting.SR.Detail:
					CreateDetailArea(graphics);
					break;
			}
		}
		protected internal virtual void CreateDetailArea(IBrickGraphics graphics) {
			GInfo.Cache.Paint = new XBrickPaint(PrinterParameters, graphics);
			try {
				CreateDetailAreaCore();
			} catch (Exception exception) {
				if (!DevExpress.XtraPrinting.Native.PSNativeMethods.AspIsRunning)
					XtraMessageBox.Show(exception.Message, SchedulerLocalizer.GetString(SchedulerStringId.Msg_Warning), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				else
					throw exception;
			} finally {
			}
		}
		protected internal virtual Rectangle CalculateClipBounds() {
			int innerBorderWidth = (int)Math.Ceiling((double)BorderWidth / 2.0);
			int left = (Borders & BorderSide.Left) != 0 ? innerBorderWidth : 0;
			int top = (Borders & BorderSide.Top) != 0 ? innerBorderWidth : 0;
			int rightOffset = (Borders & BorderSide.Right) != 0 ? innerBorderWidth : 0;
			int bottomOffset = (Borders & BorderSide.Bottom) != 0 ? innerBorderWidth : 0;
			Size size = ActualPrintBounds.Size;
			return new Rectangle(new Point(left, top), new Size(size.Width - rightOffset - left, size.Height - top - bottomOffset));
		}
		#endregion
		#region ISupportCustomDraw implementation
		void ISupportCustomDraw.RaiseCustomDrawTimeCell(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawTimeCellCore(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawDayViewAllDayArea(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawDayViewAllDayAreaCore(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawResourceHeader(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawResourceHeaderCore(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawGroupSeparator(CustomDrawObjectEventArgs e) {
		}
		void ISupportCustomDraw.RaiseCustomDrawDayHeader(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawDayHeaderCore(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawWeekViewTopLeftCorner(CustomDrawObjectEventArgs e) {
		}
		void ISupportCustomDraw.RaiseCustomDrawDayOfWeekHeader(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawDayOfWeekHeaderCore(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawDayViewTimeRuler(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawDayViewTimeRulerCore(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawAppointment(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawAppointmentCore(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawAppointmentBackground(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawAppointmentBackgroundCore(e);
		}
		void ISupportCustomDraw.RaiseCustomDrawDependency(CustomDrawObjectEventArgs e) {
		}
		void ISupportCustomDraw.RaiseCustomDrawTimeIndicator(CustomDrawObjectEventArgs e) {
			RaiseCustomDrawTimeIndicator(e);
		}
		protected internal virtual void RaiseCustomDrawTimeIndicator(CustomDrawObjectEventArgs e) {
		}
		protected internal virtual void RaiseCustomDrawTimeCellCore(CustomDrawObjectEventArgs e) {
		}
		protected internal virtual void RaiseCustomDrawDayViewAllDayAreaCore(CustomDrawObjectEventArgs e) {
		}
		protected internal virtual void RaiseCustomDrawResourceHeaderCore(CustomDrawObjectEventArgs e) {
		}
		protected internal virtual void RaiseCustomDrawDayHeaderCore(CustomDrawObjectEventArgs e) {
		}
		protected internal virtual void RaiseCustomDrawDayOfWeekHeaderCore(CustomDrawObjectEventArgs e) {
		}
		protected internal virtual void RaiseCustomDrawDayViewTimeRulerCore(CustomDrawObjectEventArgs e) {
		}
		protected internal virtual void RaiseCustomDrawAppointmentCore(CustomDrawObjectEventArgs e) {
		}
		protected internal virtual void RaiseCustomDrawAppointmentBackgroundCore(CustomDrawObjectEventArgs e) {
		}
		protected internal virtual void RaiseCustomDrawEvent(object cevent, CustomDrawObjectEventArgs e) {
			CustomDrawObjectEventHandler handler = (CustomDrawObjectEventHandler)this.Events[cevent];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		protected internal virtual void Initialize() {
			this.painter = CreatePainter();
			this.appearance = CreateAppearance();
			this.layoutOptionsHorizontal = CreateOptionsHorizontal();
			this.layoutOptionsVertical = CreateOptionsVertical();
			this.printColorSchemas = CreatePrintColorSchemas();
			this.actualHorizontalAnchors = new AnchorCollection();
			this.actualVerticalAnchors = new AnchorCollection();
			this.cornersOptions = new ControlCornersOptions();
			this.Borders = DefaultBorders;
			this.printInfo = CreatePrintInfo();
			this.CanShrink = DefaultCanShrink;
			this.CanGrow = DefaultCanGrow;
		}
		protected override bool ShouldSerializeBorders() {
			return Borders != DefaultBorders;
		}
		public override void ResetBorders() {
			Borders = DefaultBorders;
		}
		protected internal virtual int GetActualVisibleIntervalColumnCount() {
			return View != null ? View.VisibleIntervalColumnCount : ReportViewBase.DefaultVisibleIntervalColumnCount;
		}
		protected internal virtual PrintColorSchemaOptions CreatePrintColorSchemas() {
			return new PrintColorSchemaOptions();
		}
		protected abstract BaseHeaderAppearance CreateAppearance();
		protected virtual ControlLayoutOptions CreateOptionsVertical() {
			return new ControlVerticalLayoutOptions(this);
		}
		protected virtual ControlLayoutOptions CreateOptionsHorizontal() {
			return new ControlHorizontalLayoutOptions(this);
		}
		protected internal virtual ControlContentAnchorType CalculateHorizontalAnchorType() {
			return ControlLayoutOptions.DefaultAnchorType;
		}
		protected internal virtual ControlContentAnchorType CalculateVerticalAnchorType() {
			return ControlLayoutOptions.DefaultAnchorType;
		}
		protected virtual bool CanHaveMasterDateIterator { get { return true; } }
		protected internal virtual ISchedulerDateIterator GetDateIterator() {
			ISchedulerDateIterator result = null;
			if (CanHaveMasterDateIterator)
				result = GetMasterDateIterator();
			if (result != null)
				return result;
			return this as ISchedulerDateIterator;
		}
		protected internal virtual ISchedulerResourceIterator GetResourceIterator() {
			ISchedulerResourceIterator result = GetMasterResourceIterator();
			if (result != null)
				return result;
			return this as ISchedulerResourceIterator;
		}
		protected internal virtual ISchedulerResourceIterator GetMasterResourceIterator() {
			return null;
		}
		protected internal virtual ISchedulerDateIterator GetMasterDateIterator() {
			return null;
		}
		protected internal virtual bool IsPrintingComplete() {
			if (PrintController == null)
				return false;
			return PrintController.IsPrintingComplete();
		}
		public void CreateDetailAreaCore() {
			EnsurePrintController();
			PrintController.Print(this);
		}
		protected virtual void PrintColorSchemaChanged() {
		}
		protected override void OnBeforePrint(PrintEventArgs e) {
			base.OnBeforePrint(e);
			if (e.Cancel) {
				UpdateDataCacheOnCancelPrint();
			}
		}
		protected virtual void UpdateDataCacheOnCancelPrint() {
			EnsurePrintController();
			PrintController.MoveNext();
		}
		protected virtual void SetView(ReportViewBase value) {
			this.view = value;
			CalculatePrintAppearance();
			DropPrintController();
		}
		protected internal virtual void UpdatePrintController() {
			if (PrintController != null)
				SetupPrintController();
		}
		protected virtual void SubscribeViewEvents() {
			if (View == null)
				return;
			View.Disposed += new EventHandler(OnViewDisposed);
			View.AfterApplyChanges += new AfterApplyReportControlChangesEventHandler(OnViewAfterApplyChanges);
		}
		protected virtual void OnViewDisposed(object sender, EventArgs e) {
			View = null;
		}
		protected virtual void UnsubscribeViewEvents() {
			if (View == null)
				return;
			View.Disposed -= new EventHandler(OnViewDisposed);
			View.AfterApplyChanges -= new AfterApplyReportControlChangesEventHandler(OnViewAfterApplyChanges);
		}
		protected virtual void OnViewAfterApplyChanges(object sender, AfterApplyReportControlChangesEventArgs e) {
			if ((e.Actions & ReportControlChangeActions.InitializePrintController) != 0)
				if (PrintController != null)
					SetupPrintController();
			if ((e.Actions & ReportControlChangeActions.AppearanceChanges) != 0) 
				CalculatePrintAppearance();
		}
		protected virtual void CalculatePrintAppearance() {
			if (View == null)
				return;
			BaseViewAppearance appearance = View.PrintAppearance;
			Appearance.AlternateHeaderCaption.Assign(appearance.AlternateHeaderCaption);
			Appearance.AlternateHeaderCaptionLine.Assign(appearance.AlternateHeaderCaptionLine);
			Appearance.HeaderCaption.Assign(appearance.HeaderCaption);
			Appearance.HeaderCaptionLine.Assign(appearance.HeaderCaptionLine);
		}
		protected internal virtual void EnsurePrintController() {
			if (printController != null)
				return;
			ControlPrintControllerBase controller = CreatePrintController();
			controller.SetControlInternal(this);
			SetPrintControllerInternal(controller);
			SetupPrintController();
		}
		protected internal abstract ControlPrintControllerBase CreatePrintController();
		protected internal void DropPrintController() {
			if (printController != null) {
				printController = null;
			}
		}
		ControlLayoutInfo lastInfo;
		protected internal virtual void CalculateLayout(ControlLayoutInfo info) {
			this.lastInfo = info;
			PrepareControlLayout();
			UpdateActualAnchors(info);
			CalculateLayoutCore(info);
			CalculateActualPrintBounds(info.ControlPrintBounds);
			ApplyPrintColorSchema();
		}
		protected internal virtual void RecalculateLayout() {
			PrepareControlLayout();
			UpdateActualAnchors(this.lastInfo);
			CalculateLayoutCore(this.lastInfo);
			CalculateActualPrintBounds(this.lastInfo.ControlPrintBounds);
			ApplyPrintColorSchema();
		}
		protected abstract void ApplyPrintColorSchema();
		protected virtual PrintColorConverter GetColorConverter(PrintColorSchema optionColorSchema) {
			return GetColorConverterInstance(GetActualPrintColorSchema(optionColorSchema));
		}
		protected PrintColorConverter GetColorConverterInstance(PrintColorSchema printColorSchema) {
			switch (printColorSchema) {
				case PrintColorSchema.FullColor:
					return PrintColorConverter.FullColor;
				case PrintColorSchema.GrayScale:
					return PrintColorConverter.GrayScaleColor;
				case PrintColorSchema.BlackAndWhite:
					return PrintColorConverter.BlackAndWhiteColor;
			}
			return PrintColorConverter.DefaultConverter;
		}
		protected PrintColorSchema GetActualPrintColorSchema(PrintColorSchema elementSchema) {
			if (elementSchema != PrintColorSchema.Default)
				return elementSchema;
			return SchedulerReport != null ? SchedulerReport.PrintColorSchema : PrintColorSchema.Default;
		}
		protected internal virtual void PrepareControlLayout() {
			ActualHorizontalAnchors.Clear();
			ActualVerticalAnchors.Clear();
		}
		protected internal virtual void UpdateActualAnchors(ControlLayoutInfo info) {
			ActualHorizontalAnchors.AddRange(info.HorizontalAnchors);
			ActualVerticalAnchors.AddRange(info.VerticalAnchors);
		}
		protected internal virtual void CalculateActualPrintBounds(Rectangle controlPrintBounds) {
			int hAnchorsCount = ActualHorizontalAnchors.Count;
			int vAnchorsCount = ActualVerticalAnchors.Count;
			Rectangle hBounds = hAnchorsCount == 0 ? controlPrintBounds : Rectangle.FromLTRB(ActualHorizontalAnchors[0].Bounds.Left, 0, ActualHorizontalAnchors[hAnchorsCount - 1].Bounds.Right, 0);
			Rectangle vBounds = vAnchorsCount == 0 ? controlPrintBounds : Rectangle.FromLTRB(0, ActualVerticalAnchors[0].Bounds.Top, 0, ActualVerticalAnchors[vAnchorsCount - 1].Bounds.Bottom);
			ActualPrintBounds = Rectangle.FromLTRB(hBounds.Left, vBounds.Top, hBounds.Right, vBounds.Bottom);
		}
		protected internal virtual void ExecutePrinter() {
			Rectangle clipBounds = CalculateClipBounds();
			GraphicsInfoArgs info = new GraphicsInfoArgs(GInfo.Cache, clipBounds);
			InitializeGraphicsInfoArgs(info);
			try {
				GraphicsClip clipInfo = info.Cache.ClipInfo;
				Rectangle oldMaxBounds = clipInfo.MaximumBounds;
				clipInfo.MaximumBounds = clipBounds;
				GraphicsClipState oldClipping = clipInfo.SaveAndSetClip(clipBounds);
				ExecutePrinterCore();
				clipInfo.RestoreClipRelease(oldClipping);
				clipInfo.MaximumBounds = oldMaxBounds;
			} catch (Exception exception) {
				XtraMessageBox.Show( exception.Message, SchedulerLocalizer.GetString(SchedulerStringId.Msg_Warning), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			} finally {
				FinalizeGraphicsInfoArgs(info);
			}
		}
		protected internal virtual void ExecutePrinterCore() {
			VerticalSplitControllerBrick splitController = CreateVerticalSplitController();
			try {
				using (PanelBrickGraphics graphics = new PanelBrickGraphics(Link.PrintingSystemBase, splitController)) {
					PrintingHelper helper = ((XBrickPaint)(GInfo.Cache.Paint)).Helper;
					IBrickGraphics prevGraphics = helper.BrickGraphics;
					helper.BrickGraphics = graphics;
					PrintInfo.Print(Cache);  
					helper.BrickGraphics = prevGraphics;
					RectangleF rect = CalculateVerticalSplitControllerBounds();
					foreach(AppointmentPanelBrick appointmentBrick in splitController.Bricks.OfType<AppointmentPanelBrick>()) {
						appointmentBrick.PrintingSystem = Link.PrintingSystemBase;
						RectangleF bounds = appointmentBrick.GetBounds();
						appointmentBrick.Size = bounds.Size;
						appointmentBrick.Location = bounds.Location;
					}
					prevGraphics.DrawBrick(splitController, rect);
				}
			} finally {
			}
		}
		protected internal abstract ControlPrintInfo CreatePrintInfo();
		protected internal virtual VerticalSplitControllerBrick CreateVerticalSplitController() {
			SplitPoints splitPoints = GetVerticalSplitPoints();
			VerticalSplitControllerBrick panel = new VerticalSplitControllerBrick(splitPoints, PrintInfo.Clone(), this);
			panel.Style = new XRControlStyle();
			panel.BackColor = Color.Transparent;
			panel.BorderWidth = BorderWidth;
			panel.Sides = Borders;  
			panel.BorderColor = BorderColor; 
			panel.NoClip = true;
			panel.BorderStyle = BrickBorderStyle.Center;
			return panel;
		}
		protected internal virtual SplitPoints GetVerticalSplitPoints() {
			SplitPoints points = GetVerticalSplitPointsCore();
			return CalculateActualSplitPointsValue(points);
		}
		protected internal virtual SplitPoints CalculateActualSplitPointsValue(SplitPoints points) {
			SplitPoints result = new SplitPoints();
			int count = points.Count;
			for (int i = 0; i < count; i++) {
				float actualPoint = ((PrintingSystemBase)PrinterParameters.PS).Graph.DocumValueOf(points[i]);
				result.Add(actualPoint);
			}
			return result;
		}
		protected internal virtual SplitPoints GetVerticalSplitPointsCore() {
			return new SplitPoints();
		}
		protected internal virtual Rectangle CalculateVerticalSplitControllerBounds() {
			Rectangle r = new Rectangle(ActualPrintBounds.Left, ActualPrintBounds.Top, ActualPrintBounds.Right, ActualPrintBounds.Bottom);
			return r;
		}
		protected virtual void InitializeGraphicsInfoArgs(GraphicsInfoArgs info) {
		}
		protected virtual void FinalizeGraphicsInfoArgs(GraphicsInfoArgs info) {
			info.Cache.Dispose();
		}
		protected internal abstract ViewInfoPainterBase CreatePainter();
		protected internal virtual void SetPrintControllerInternal(ControlPrintControllerBase controller) {
			this.printController = controller;
		}
		protected internal virtual void SetupPrintController() {
			PrintController.EnsureDataCaches();
		}
		protected internal virtual AnchorCollection GetHorizontalAnchors() {
			return CloneAnchors(ActualHorizontalAnchors);
		}
		protected internal virtual AnchorCollection GetVerticalAnchors() {
			return CloneAnchors(ActualVerticalAnchors);
		}
		protected internal virtual AnchorCollection CloneAnchors(AnchorCollection anchors) {
			AnchorCollection result = new AnchorCollection();
			foreach (AnchorBase anchor in anchors)
				result.Add(anchor.Clone());
			return result;
		}
		protected internal virtual AnchorCollection GetHorizontalMasterPrintAnchors() {
			return new AnchorCollection();
		}
		protected internal virtual AnchorCollection GetVerticalMasterPrintAnchors() {
			return new AnchorCollection();
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			if (DesignMode)
				return CreateFakedBrick();
			return new SubreportBrick(this);
		}
		protected virtual bool CanCreateDesignBrick() {
			return View == null;
		}
		protected virtual VisualBrick CreateFakedBrick() {
			if (CanCreateDesignBrick())
				return new LabelBrick(this);
			SchedulerPanelBrick panelBrick = new SchedulerPanelBrick(this);
			PrintingSystem ps = SchedulerReport.FakedPrintingSystem;
			ps.ClearContent();
			try {
				Link.CreateDocument(ps);
			} finally {
			}
			if (ps.Pages.Count > 0) {
				BrickList brickList = ((PSPage)ps.Pages[0]).Bricks;
				int brickCount = brickList.Count;
				for (int i = 0; i < brickCount; i++) {
					panelBrick.Bricks.Add(brickList[i]);
				}
			}
			return panelBrick;
		}
		protected override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			brick.BorderStyle = BrickBorderStyle.Center;
			if (IsDesignFakeBrick())
				SetDesignBrickText((LabelBrick)brick, GetDesignBrickText());
		}
		protected internal bool IsDesignFakeBrick() {
			return IsDesignMode && CanCreateDesignBrick();
		}
		protected virtual string GetDesignBrickText() {
			return SchedulerLocalizer.GetString(SchedulerStringId.Reporting_NotAssigned_View);
		}
		protected void SetDesignBrickText(LabelBrick label, string text) {
			label.Text = text;
			label.StringFormat = new BrickStringFormat(StringAlignment.Center, StringAlignment.Center);
			label.Sides = BorderSide.All;
		}
		protected internal virtual void PrintViewInfo(SchedulerViewInfoBase viewInfo, GraphicsCache cache, Rectangle bounds) {
		}
		protected override void WriteContentToCore(XRWriteInfo writeInfo, VisualBrick brick) {
			if (!CanWriteContent())
				return;
			SubreportBrick subrepBrick = (SubreportBrick)brick;
			if (PrintController.DataCache.NeedPageBreakBeforeNexColumn())
				writeInfo.InsertPageBreak(0.0f);
			SubreportDocumentBand subrepBand = new SubreportDocumentBand(brick.Rect);
			subrepBrick.DocumentBand = subrepBand;
			PrintingSystemBase ps = writeInfo.PrintingSystem;
			Link.AddSubreport(ps, subrepBand, PointF.Empty);
			ps.Graph.PageUnit = GraphicsUnit.Document;
			base.WriteContentToCore(writeInfo, brick);
		}
		protected internal virtual bool CanWriteContent() {
			return View != null && PrintController != null;
		}
		protected internal abstract void CalculateLayoutCore(ControlLayoutInfo info);
		protected internal virtual void SynchronizeMasterControlsProperties() {
		}
	}
	#endregion
	#region ReportRelatedControlBase
	[XRDesigner("DevExpress.XtraScheduler.Reporting.Design.ReportRelatedControlDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions),
Designer("DevExpress.XtraScheduler.Reporting.Design.ReportRelatedControlDesigner_," + AssemblyInfo.SRAssemblySchedulerDesign)
	]
	public abstract class ReportRelatedControlBase : ReportViewControlBase {
		protected ReportRelatedControlBase()
			: base() {
		}
		protected ReportRelatedControlBase(ReportViewBase view)
			: base(view) {
		}
		#region Properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public virtual HorizontalHeadersControlBase HorizontalHeaders {
			get {
				return LayoutOptionsHorizontal != null ? LayoutOptionsHorizontal.MasterControl as HorizontalHeadersControlBase : null;
			}
			set {
				if (LayoutOptionsHorizontal != null)
					LayoutOptionsHorizontal.MasterControl = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public virtual VerticalHeadersControlBase VerticalHeaders {
			get {
				return LayoutOptionsVertical != null ? LayoutOptionsVertical.MasterControl as VerticalHeadersControlBase : null;
			}
			set {
				if (LayoutOptionsVertical != null)
					LayoutOptionsVertical.MasterControl = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(null), Category(SRCategoryNames.Layout)]
		public TimeCellsControlBase TimeCells {
			get {
				return GetTimeCells();
			}
			set {
				SetTimeCells(value);
			}
		}
		protected internal virtual TimeCellsControlBase GetTimeCells() {
			return LayoutOptionsHorizontal != null ? LayoutOptionsHorizontal.MasterControl as TimeCellsControlBase : null;
		}
		protected internal virtual void SetTimeCells(TimeCellsControlBase value) {
			if (LayoutOptionsHorizontal != null)
				LayoutOptionsHorizontal.MasterControl = value;
		}
		protected internal bool IsHorizontalTimeCells {
			get { return Object.Equals(TimeCells, LayoutOptionsHorizontal.MasterControl); }
		}
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					UnsubscribeLayoutOptionsChanged(LayoutOptionsHorizontal);
					UnsubscribeLayoutOptionsChanged(LayoutOptionsVertical);
					DisposePrintInfo();
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected internal override void Initialize() {
			base.Initialize();
			SubscribeLayoutOptionsChanged(LayoutOptionsHorizontal);
			SubscribeLayoutOptionsChanged(LayoutOptionsVertical);
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			if (PrintController != null)
				PrintController.SetPrintState(ControlPrintState.None);
		}
		protected virtual void SubscribeLayoutOptionsChanged(ControlLayoutOptions options) {
			if (options != null)
				options.MasterControlChanging += new ReportViewControlCancelEventHandler(OnLayoutOptionsMasterControlChanging);
		}
		protected virtual void UnsubscribeLayoutOptionsChanged(ControlLayoutOptions options) {
			if (options != null)
				options.MasterControlChanging -= new ReportViewControlCancelEventHandler(OnLayoutOptionsMasterControlChanging);
		}
		void OnLayoutOptionsMasterControlChanging(object sender, ReportViewControlCancelEventArgs e) {
			e.Cancel = CanAssignMasterControl(e.Control);
		}
		protected bool CanAssignMasterControl(ReportViewControlBase newMaster) {
			if (newMaster == null)
				return true;
			if (this == newMaster)
				return false;
			if (LayoutOptionsHorizontal.MasterControl == newMaster || LayoutOptionsVertical.MasterControl == newMaster)
				return false;
			if (newMaster.LayoutOptionsHorizontal.MasterControl == this)
				return false;
			if (newMaster.LayoutOptionsVertical.MasterControl == this)
				return false;
			return true;
		}
		protected internal int CalculateVisibleResourceCount() {
			return View != null ? View.ActualVisibleResourceCount : 0;
		}
		protected internal override AnchorCollection GetHorizontalMasterPrintAnchors() {
			return LayoutOptionsHorizontal.MasterControl != null ? LayoutOptionsHorizontal.MasterControl.GetHorizontalAnchors() : base.GetHorizontalMasterPrintAnchors();
		}
		protected internal override AnchorCollection GetVerticalMasterPrintAnchors() {
			return LayoutOptionsVertical.MasterControl != null ? LayoutOptionsVertical.MasterControl.GetVerticalAnchors() : base.GetVerticalMasterPrintAnchors();
		}
		protected internal override ISchedulerDateIterator GetMasterDateIterator() {
			ISchedulerDateIterator result = GetHorizontalMasterDateIterator();
			if (result == null)
				result = GetVerticalMasterDateIterator();
			return result;
		}
		protected internal virtual ISchedulerDateIterator GetHorizontalMasterDateIterator() {
			return GetMasterDateIteratorCore(LayoutOptionsHorizontal.MasterControl);
		}
		protected internal virtual ISchedulerDateIterator GetVerticalMasterDateIterator() {
			return GetMasterDateIteratorCore(LayoutOptionsVertical.MasterControl);
		}
		protected internal virtual ISchedulerDateIterator GetMasterDateIteratorCore(ReportViewControlBase masterControl) {
			if (masterControl == null)
				return null;
			ISchedulerDateIterator result = masterControl as ISchedulerDateIterator;
			if (result != null)
				return result;
			return masterControl.GetDateIterator();
		}
		protected internal override ISchedulerResourceIterator GetMasterResourceIterator() {
			ISchedulerResourceIterator result = GetMasterResourceIteratorCore(LayoutOptionsHorizontal.MasterControl);
			if (result == null)
				result = GetMasterResourceIteratorCore(LayoutOptionsVertical.MasterControl);
			return result;
		}
		protected internal virtual ISchedulerResourceIterator GetMasterResourceIteratorCore(ReportViewControlBase masterControl) {
			if (masterControl == null)
				return null;
			ISchedulerResourceIterator result = masterControl as ISchedulerResourceIterator;
			if (result != null)
				return result;
			return masterControl.GetResourceIterator();
		}
	}
	#endregion
	#region TimeCellsControlBase
	public abstract class TimeCellsControlBase : ReportRelatedControlBase, ISupportAppointments,
		ISchedulerDateIterator, ISchedulerResourceIterator, ISupportDataIterationPriority,
		ISupportPrintableTimeInterval, ISupportPrintableResources {
		new internal static class EventNames {
			public const string CustomDrawTimeCell = "CustomDrawTimeCell";
			public const string CustomDrawAppointment = "CustomDrawAppointment";
			public const string CustomDrawAppointmentBackground = "CustomDrawAppointmentBackground";
			public const string InitAppointmentDisplayText = "InitAppointmentDisplayText";
			public const string InitAppointmentImages = "InitAppointmentImages";
			public const string AppointmentViewInfoCustomizing = "AppointmentViewInfoCustomizing";
		}
		#region Fields
		AppointmentDisplayOptions appointmentDisplayOptions;
		IAppointmentComparerProvider appointmentComparerProvider;
		SchedulerCancellationTokenSource tokenSource = new SchedulerCancellationTokenSource();
		#endregion
		protected TimeCellsControlBase(ReportViewBase view)
			: base(view) {
		}
		protected TimeCellsControlBase()
			: base() {
		}
		#region Properties
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeCellsControlBaseAppointmentDisplayOptions"),
#endif
Category(SRCategoryNames.Layout), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppointmentDisplayOptions AppointmentDisplayOptions { get { return appointmentDisplayOptions; } }
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeCellsControlBasePrintColorSchemas"),
#endif
Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new TimeCellsPrintColorSchemaOptions PrintColorSchemas { get { return (TimeCellsPrintColorSchemaOptions)base.PrintColorSchemas; } }
		protected new TimeCellsControlPrintInfo PrintInfo { get { return (TimeCellsControlPrintInfo)base.PrintInfo; } }
		protected internal SchedulerViewCellContainerCollection CellContainers { get { return PrintInfo.CellContainers; } }
		protected internal AppointmentsLayoutResult AppointmentsLayoutResult { get { return PrintInfo.AppointmentsLayoutResult; } }
		protected internal new BaseViewAppearance Appearance { get { return (BaseViewAppearance)base.Appearance; } }
		protected internal new TimeCellsControlPainter Painter { get { return (TimeCellsControlPainter)base.Painter; } }
		protected internal abstract bool ActualShowMoreItems { get; }
		internal new CellsControlPrintControllerBase PrintController { get { return (CellsControlPrintControllerBase)base.PrintController; } }
		internal bool IsTiledAtDesignMode { get { return IsDesignMode && (LayoutOptionsVertical.LayoutType == ControlContentLayoutType.Tile); } }
		internal bool ActualCanShrink { get { return CanShrink && !IsDesignMode; } }
		internal bool ActualCanGrow { get { return CanGrow && !IsDesignMode; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraScheduler.Reporting.TimeCellsControlBase.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new TimeCellsControlBaseScripts Scripts { get { return (TimeCellsControlBaseScripts)fEventScripts; } }
		#endregion
		#region Events
		#region InitAppointmentImages
		static readonly object InitAppointmentImagesEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeCellsControlBaseInitAppointmentImages"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentImagesEventHandler InitAppointmentImages {
			add { Events.AddHandler(InitAppointmentImagesEvent, value); }
			remove { Events.RemoveHandler(InitAppointmentImagesEvent, value); }
		}
		protected internal virtual void RaiseInitAppointmentImages(AppointmentImagesEventArgs e) {
			RunEventScript(InitAppointmentImagesEvent, EventNames.InitAppointmentImages, e);
			AppointmentImagesEventHandler handler = (AppointmentImagesEventHandler)Events[InitAppointmentImagesEvent];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region InitAppointmentDisplayText
		static readonly object InitAppointmentDisplayTextEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeCellsControlBaseInitAppointmentDisplayText"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentDisplayTextEventHandler InitAppointmentDisplayText {
			add { Events.AddHandler(InitAppointmentDisplayTextEvent, value); }
			remove { Events.RemoveHandler(InitAppointmentDisplayTextEvent, value); }
		}
		protected internal virtual void RaiseInitAppointmentDisplayText(AppointmentDisplayTextEventArgs e) {
			RunEventScript(InitAppointmentDisplayTextEvent, EventNames.InitAppointmentDisplayText, e);
			AppointmentDisplayTextEventHandler handler = (AppointmentDisplayTextEventHandler)Events[InitAppointmentDisplayTextEvent];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region AppointmentViewInfoCustomizing
		static readonly object AppointmentViewInfoCustomizingEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeCellsControlBaseAppointmentViewInfoCustomizing"),
#endif
		Category(SRCategoryNames.Scheduler)]
		public event AppointmentViewInfoCustomizingEventHandler AppointmentViewInfoCustomizing {
			add { Events.AddHandler(AppointmentViewInfoCustomizingEvent, value); }
			remove { Events.RemoveHandler(AppointmentViewInfoCustomizingEvent, value); }
		}
		protected internal virtual void RaiseAppointmentViewInfoCustomizing(AppointmentViewInfoCustomizingEventArgs e) {
			RunEventScript(AppointmentViewInfoCustomizingEvent, EventNames.AppointmentViewInfoCustomizing, e);
			AppointmentViewInfoCustomizingEventHandler handler = (AppointmentViewInfoCustomizingEventHandler)Events[AppointmentViewInfoCustomizingEvent];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		#region CustomDrawTimeCell
		static readonly object CustomDrawTimeCellEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeCellsControlBaseCustomDrawTimeCell"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawTimeCell {
			add { Events.AddHandler(CustomDrawTimeCellEvent, value); }
			remove { Events.RemoveHandler(CustomDrawTimeCellEvent, value); }
		}
		protected internal override void RaiseCustomDrawTimeCellCore(CustomDrawObjectEventArgs e) {
			RunEventScript(CustomDrawTimeCellEvent, EventNames.CustomDrawTimeCell, e);
			RaiseCustomDrawEvent(CustomDrawTimeCellEvent, e);
		}
		#endregion
		#region CustomDrawAppointment
		static readonly object CustomDrawAppointmentEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeCellsControlBaseCustomDrawAppointment"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawAppointment {
			add { Events.AddHandler(CustomDrawAppointmentEvent, value); }
			remove { Events.RemoveHandler(CustomDrawAppointmentEvent, value); }
		}
		protected internal override void RaiseCustomDrawAppointmentCore(CustomDrawObjectEventArgs e) {
			RunEventScript(CustomDrawAppointmentEvent, EventNames.CustomDrawAppointment, e);
			RaiseCustomDrawEvent(CustomDrawAppointmentEvent, e);
		}
		#endregion
		#region CustomDrawAppointmentBackground
		static readonly object CustomDrawAppointmentBackgroundEvent = new object();
		[
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("TimeCellsControlBaseCustomDrawAppointmentBackground"),
#endif
		Category(SRCategoryNames.CustomDraw)]
		public event CustomDrawObjectEventHandler CustomDrawAppointmentBackground {
			add { Events.AddHandler(CustomDrawAppointmentBackgroundEvent, value); }
			remove { Events.RemoveHandler(CustomDrawAppointmentBackgroundEvent, value); }
		}
		protected internal override void RaiseCustomDrawAppointmentBackgroundCore(CustomDrawObjectEventArgs e) {
			RunEventScript(CustomDrawAppointmentBackgroundEvent, EventNames.CustomDrawAppointmentBackground, e);
			RaiseCustomDrawEvent(CustomDrawAppointmentBackgroundEvent, e);
		}
		#endregion
		#endregion
		#region ISchedulerDateIterator Members
		TimeIntervalDataCache ISchedulerDateIterator.GetTimeIntervalDataCache() {
			EnsurePrintController();
			return PrintController.DataCache;
		}
		int ISchedulerDateIterator.VisibleIntervalColumnCount { get { return GetActualVisibleIntervalColumnCount(); } }
		ColumnArrangementMode ISchedulerDateIterator.ColumnArrangement { get { return View != null ? View.ColumnArrangement : ReportViewBase.DefaultColumnArrangement; } }
		#endregion
		#region ISchedulerResourceIterator Members
		ResourceDataCache ISchedulerResourceIterator.GetResourceDataCache() {
			EnsurePrintController();
			return PrintController.ResourceDataCache;
		}
		#endregion
		#region ISupportDataIterationPriority Members
		SchedulerDataIterationPriority ISupportDataIterationPriority.IterationPriority {
			get { return CalculateIterationPriority(); }
		}
		ISchedulerResourceIterator ISupportDataIterationPriority.GetResourceIterator() {
			return GetResourceIterator();
		}
		ISchedulerDateIterator ISupportDataIterationPriority.GetDateIterator() {
			return GetDateIterator();
		}
		#endregion
		#region ISupportTimeIntervalInfo Members
		TimeInterval ISupportPrintableTimeInterval.GetPrintTimeInterval(PrintContentMode displayMode) {
			TimeInterval result = GetPrintTimeIntervalCore(displayMode);
			return ValidatePrintTimeInterval(result);
		}
		protected internal virtual TimeInterval ValidatePrintTimeInterval(TimeInterval interval) {
			return interval;
		}
		protected internal virtual TimeInterval GetPrintTimeIntervalCore(PrintContentMode displayMode) {
			ISchedulerDateIterator iterator = GetMasterDateIterator();
			if (iterator != null) {
				TimeIntervalDataCache cache = iterator.GetTimeIntervalDataCache();
				return cache.GetPrintTimeInterval(displayMode);
			}
			return PrintController != null ? PrintController.GetPrintTimeInterval(displayMode) : TimeInterval.Empty;
		}
		#endregion
		#region ISupportPrintableResources Members
		ResourceBaseCollection ISupportPrintableResources.GetPrintResources() {
			ResourceBaseCollection result = new ResourceBaseCollection();
			ISchedulerResourceIterator iterator = GetMasterResourceIterator();
			if (iterator != null) {
				ResourceDataCache cache = iterator.GetResourceDataCache();
				result.AddRange(cache.PrintResources);
				return result;
			}
			if (PrintController != null) {
				result.AddRange(PrintController.PrintResources);
			}
			return result;
		}
		#endregion
		#region ISupportAppointments Members
		AppearanceObject ISupportAppointments.AppointmentAppearance {
			get { return Appearance.Appointment; }
		}
		object ISupportAppointments.AppointmentImages {
			get { return null; }
		}
		Size ISupportAppointments.CalculateMoreButtonMinSize() {
			return CalculateMoreButtonSize();
		}
		internal Size CalculateMoreButtonSize() {
			return MoreItems.CalculateMinSize(GInfo);
		}
		ColoredSkinElementCache ISupportAppointments.ColoredSkinElementCache {
			get {
				XtraSchedulerDebug.Assert(false);
				return null;
			}
		}
		MoreButton ISupportAppointments.CreateMoreButton() {
			return new MoreItems();
		}
		IAppointmentStatus ISupportAppointments.GetStatus(object statusId) {
			return View.GetStatus(statusId);
		}
		bool ISupportAppointments.ShowMoreButtons {
			get { return ActualShowMoreItems; }
		}
		TimeZoneHelper ISupportAppointments.TimeZoneHelper {
			get { return TimeZoneHelper; }
		}
		IAppointmentFormatStringService ISupportAppointments.GetFormatStringProvider() {
			return (IAppointmentFormatStringService)View.GetService(typeof(IAppointmentFormatStringService));
		}
		Color ISupportAppointments.GetLabelColor(object labelId) {
			return View.GetLabelColor(labelId);
		}
		bool ISupportAppointments.DrawMoreButtonsOverAppointments { get { return DrawMoreButtonsOverAppointmentsInternal; } }
		bool ISupportAppointments.ShouldShowContainerScrollBar() {
			return false;
		}
		bool ISupportAppointments.OverriddenAppointmentForeColor { get { return false; } }
		protected internal TimeZoneHelper TimeZoneHelper {
			get { return SchedulerReport.ActualSchedulerAdapter.TimeZoneHelper; }
		}
		protected internal virtual bool DrawMoreButtonsOverAppointmentsInternal { get { return false; } }
		#endregion
		#region ISupportAppointmentsBase Members
		TimeInterval ISupportAppointmentsBase.GetVisibleInterval() {
			return GetCellsInterval();
		}
		AppointmentDisplayOptions ISupportAppointmentsBase.AppointmentDisplayOptions {
			get { return AppointmentDisplayOptions; }
		}
		IAppointmentComparerProvider ISupportAppointmentsBase.AppointmentComparerProvider {
			get {
				if (appointmentComparerProvider == null) {
					appointmentComparerProvider = View.GetService(typeof(IAppointmentComparerProvider)) as IAppointmentComparerProvider;
				}
				if (appointmentComparerProvider == null)
					appointmentComparerProvider = new AppointmentComparerProvider(View);
				return appointmentComparerProvider;
			}
		}
		bool ISupportAppointmentsBase.UseAsyncMode { get { return false; } }
		SchedulerCancellationTokenSource ISupportAppointmentsBase.CancellationToken {
			get { return this.tokenSource;}
		}
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (appointmentComparerProvider != null) {
						appointmentComparerProvider = null;
					}
					if (appointmentDisplayOptions != null) {
						appointmentDisplayOptions = null;
					}
					if (this.tokenSource != null) {
						this.tokenSource.Dispose();
						this.tokenSource = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnSizeChanged(DevExpress.XtraReports.UI.ChangeEventArgs e) {
			base.OnSizeChanged(e);
		}
		protected internal override void Initialize() {
			base.Initialize();
			this.fEventScripts = CreateScripts();
			this.appointmentDisplayOptions = CreateAppointmentDisplayOptions();
		}
		protected override XRControlScripts CreateScripts() {
			return new TimeCellsControlBaseScripts(this);
		}
		protected internal SchedulerGroupType CalculateViewGroupType() {
			return View != null ?  ((ISchedulerResourceProvider)View).GetGroupType() : ReportViewBase.DefaultGroupType;
		}
		protected internal SchedulerDataIterationPriority CalculateIterationPriority() {
			return CalculateViewGroupType() == SchedulerGroupType.Date ? SchedulerDataIterationPriority.Resource
				: SchedulerDataIterationPriority.Date;
		}
		protected internal override PrintColorSchemaOptions CreatePrintColorSchemas() {
			return new TimeCellsPrintColorSchemaOptions();
		}
		protected override BaseHeaderAppearance CreateAppearance() {
			return new BaseViewAppearance();
		}
		protected override void CalculatePrintAppearance() {
			base.CalculatePrintAppearance();
			if (View == null)
				return;
			BaseViewAppearance appearance = View.PrintAppearance;
			Appearance.Appointment.Assign(appearance.Appointment);
			Appearance.ResourceHeaderCaption.Assign(appearance.ResourceHeaderCaption);
			Appearance.ResourceHeaderCaptionLine.Assign(appearance.ResourceHeaderCaptionLine);
		}
		protected override void ApplyPrintColorSchema() {
			CellContainers.ForEach(ApplyColorConverterToContainer);
			ApplyColorConverterToAppointments(AppointmentsLayoutResult.AppointmentViewInfos);
		}
		protected void ApplyColorConverterToAppointments(AppointmentViewInfoCollection appointmentViewInfos) {
			PrintColorConverter converter = GetColorConverter(PrintColorSchemas.Appointment);
			ColorConverterHelper helper = new ColorConverterHelper(converter);
			helper.ApplyColorConverterToAppointments(appointmentViewInfos);
		}
		protected virtual void ApplyColorConverterToContainer(SchedulerViewCellContainer cellContainer) {
			PrintColorConverter converter = GetColorConverter(PrintColorSchemas.Content);
			converter.ConvertAppearance(cellContainer.Appearance);
			cellContainer.Cells.ForEach(ApplyColorConverterToCell);
		}
		protected virtual void ApplyColorConverterToCell(SchedulerViewCellBase cell) {
			PrintColorConverter converter = GetColorConverter(PrintColorSchemas.Content);
			converter.ConvertCellAppearance(cell.Appearance);
		}
		protected internal override void PrepareControlLayout() {
			base.PrepareControlLayout();
			AppointmentsLayoutResult.Clear();
			CellContainers.Clear();
		}
		protected internal override void CalculateLayoutCore(ControlLayoutInfo info) {
			CellsLayoutStrategyBase strategy = CreateLayoutStrategy();
			strategy.CalculateLayout(info);
		}
		protected internal virtual AppointmentBaseCollection GetAppointments(TimeInterval interval, ResourceBaseCollection resources) {
			if (DesignMode)
				return new AppointmentBaseCollection();
			return View.GetAppointments(interval, resources);
		}
		protected internal override void SetupPrintController() {
			base.SetupPrintController();
			PrintController.ResourcesEnabled = GetMasterResourceIterator() == null;
			if (PrintController.ResourcesEnabled) {
				PrintController.ResourceDataCache.GroupLength = CalculateVisibleResourceCount();
			}
		}
		protected internal virtual TimeInterval GetCellsInterval() {
			int count = CellContainers.Count;
			if (count == 0)
				return TimeInterval.Empty;
			DateTime start = CellContainers[0].Interval.Start;
			DateTime end = CellContainers[count - 1].Interval.End;
			return new TimeInterval(start, end);
		}
		protected internal virtual void SubscribeAppointmentContentCalculatorEvents(AppointmentContentLayoutCalculator contentCalculator) {
			contentCalculator.InitAppointmentDisplayText += new AppointmentDisplayTextEventHandler(OnInitAppointmentDisplayText);
			contentCalculator.InitAppointmentImages += new AppointmentImagesEventHandler(OnInitAppointmentImages);
			contentCalculator.AppointmentViewInfoCustomizing += new AppointmentViewInfoCustomizingEventHandler(OnAppointmentViewInfoCustomizing);
		}
		protected internal virtual void UnsubscribeAppointmentContentCalculatorEvents(AppointmentContentLayoutCalculator contentCalculator) {
			contentCalculator.AppointmentViewInfoCustomizing -= new AppointmentViewInfoCustomizingEventHandler(OnAppointmentViewInfoCustomizing);
			contentCalculator.InitAppointmentImages -= new AppointmentImagesEventHandler(OnInitAppointmentImages);
			contentCalculator.InitAppointmentDisplayText -= new AppointmentDisplayTextEventHandler(OnInitAppointmentDisplayText);
		}
		protected void OnInitAppointmentImages(object sender, AppointmentImagesEventArgs e) {
			RaiseInitAppointmentImages(e);
		}
		protected void OnInitAppointmentDisplayText(object sender, AppointmentDisplayTextEventArgs e) {
			RaiseInitAppointmentDisplayText(e);
		}
		protected void OnAppointmentViewInfoCustomizing(object sender, AppointmentViewInfoCustomizingEventArgs e) {
			RaiseAppointmentViewInfoCustomizing(e);
		}
		protected void OnCustomDrawAppointment(object sender, CustomDrawObjectEventArgs e) {
			RaiseCustomDrawAppointmentCore(e);
		}
		protected void OnCustomDrawAppointmentBackground(object sender, CustomDrawObjectEventArgs e) {
			RaiseCustomDrawAppointmentBackgroundCore(e);
		}
		protected void OnCustomDrawTimeCell(object sender, CustomDrawObjectEventArgs e) {
			RaiseCustomDrawTimeCellCore(e);
		}
		protected internal override SplitPoints GetVerticalSplitPointsCore() {
			SplitPoints result = new SplitPoints();
			int count = ActualVerticalAnchors.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(CalculateVerticalSplitPoints(ActualVerticalAnchors[i]));
			return result;
		}
		protected internal virtual SplitPoints CalculateVerticalSplitPoints(AnchorBase anchorBase) {
			SplitPoints result = new SplitPoints();
			result.Add(anchorBase.Bounds.Top);
			int count = anchorBase.InnerAnchors.Count;
			for (int i = 0; i < count; i++)
				result.Add(anchorBase.InnerAnchors[i].Bounds.Bottom);
			return result;
		}
		protected internal override ControlPrintInfo CreatePrintInfo() {
			TimeCellsControlPrintInfo printInfo = new TimeCellsControlPrintInfo(this);
			return printInfo;
		}
		protected internal virtual bool ShouldFitIntoBounds() {
			return LayoutOptionsVertical.LayoutType == ControlContentLayoutType.Fit;
		}
		protected internal abstract AppointmentDisplayOptions CreateAppointmentDisplayOptions();
		protected internal abstract CellsLayoutStrategyBase CreateLayoutStrategy();
		bool ShouldSerializeScripts() {
			return !fEventScripts.IsDefault();
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Reporting.Native {
	public enum SchedulerDataIterationPriority { Date, Resource };
	public class SplitPoints : List<float> {
	}
	#region TimeCellsControlPainter
	public abstract class TimeCellsControlPainter : ViewInfoPainterBase {
		readonly AppointmentPainter appointmentPainter;
		readonly ViewInfoPainterBase cellPainter;
		readonly MoreItemsPrintPainter moreItemsPainter;
		protected TimeCellsControlPainter() {
			this.cellPainter = CreateCellPainter();
			this.appointmentPainter = CreateAppointmentPainter();
			this.moreItemsPainter = CreateMoreItemsPainter();
		}
		protected internal AppointmentPainter AppointmentPainter { get { return appointmentPainter; } }
		protected internal ViewInfoPainterBase CellPainter { get { return cellPainter; } }
		protected internal MoreItemsPrintPainter MoreItemsPainter { get { return moreItemsPainter; } }
		protected internal abstract AppointmentPainter CreateAppointmentPainter();
		protected internal abstract ViewInfoPainterBase CreateCellPainter();
		protected internal virtual MoreItemsPrintPainter CreateMoreItemsPainter() {
			return new MoreItemsPrintPainter();
		}
		public void Print(GraphicsCache cache, SchedulerViewCellContainerCollection cells, AppointmentsLayoutResult aptLayoutResult, ISupportCustomDraw customDrawProvider, IStatusBrushAdapter brushAdapter) {
			PrintCells(cache, cells, customDrawProvider);
			PrintAppointments(cache, aptLayoutResult, customDrawProvider, brushAdapter);
			PrintMoreItems(cache, aptLayoutResult);
		}
		private void PrintMoreItems(GraphicsCache cache, AppointmentsLayoutResult aptLayoutResult) {
			int count = aptLayoutResult.MoreButtons.Count;
			for (int i = 0; i < count; i++)
				MoreItemsPainter.Draw(cache, aptLayoutResult.MoreButtons[i]);
		}
		protected internal virtual void PrintAppointments(GraphicsCache cache, AppointmentsLayoutResult aptLayoutResult, ISupportCustomDraw customDrawProvider, IStatusBrushAdapter brushAdapter) {
			AppointmentPainter.DrawAppointmentsWithoutScrolling(cache, aptLayoutResult.AppointmentViewInfos, customDrawProvider);
		}
		protected internal abstract void PrintCells(GraphicsCache cache, SchedulerViewCellContainerCollection cellContainers, ISupportCustomDraw customDrawProvider);
	}
	#endregion
	[BrickExporter(typeof(SchedulerPanelBrickExporter))]
	public class SchedulerPanelBrick : PanelBrick {
		public SchedulerPanelBrick()
			:base() {
		}
		public SchedulerPanelBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
		}
	}
	public class SchedulerPanelBrickExporter : PanelBrickExporter {
		protected override void DrawObject(IGraphics gr, RectangleF rect) {
			DrawForeground(gr, rect);
			DrawBorders(gr, rect);
		}
	}
	public class VerticalSplitControllerBrickPainter : SchedulerPanelBrickExporter {
		VerticalSplitControllerBrick VerticalSplitControllerBrick { get { return Brick as VerticalSplitControllerBrick; } }
		protected override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			return DrawContentToViewData(exportContext, GraphicsUnitConverter.Round(rect), TextAlignment.MiddleCenter);
		}
		protected override void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
			FillHtmlTableCellCore(exportProvider);
			base.FillHtmlTableCellInternal(exportProvider);
		}
		protected override void FillRtfTableCellInternal(IRtfExportProvider exportProvider) {
			FillTableCell(exportProvider);
		}
		protected override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			FillTableCell(exportProvider);
		}
		void FillTableCell(ITableExportProvider exportProvider) {
			FillTableCellWithImage(exportProvider, ImageSizeMode.StretchImage, DevExpress.XtraPrinting.ImageAlignment.Default, exportProvider.CurrentData.Bounds);
		}
		protected override void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
			Rectangle boundsWithoutBorders = exportProvider.CurrentData.OriginalBounds;
			boundsWithoutBorders.Size = System.Drawing.Size.Round(VerticalSplitControllerBrick.GetClientRectangle(boundsWithoutBorders, GraphicsDpi.Pixel).Size);
			Bitmap exportImage = CreateExportImage(exportProvider.CurrentData);
			System.Drawing.Size imageSize = GetResolutionImageSize(exportImage, exportProvider.ExportContext);
			exportProvider.SetCellImage(exportImage, String.Empty, ImageSizeMode.StretchImage, DevExpress.XtraPrinting.ImageAlignment.Default, boundsWithoutBorders, imageSize, VerticalSplitControllerBrick.Padding, VerticalSplitControllerBrick.Url);
		}
		Bitmap CreateExportImage(BrickViewData brickViewData) {
			Rectangle bounds = brickViewData.OriginalBounds;
			Bitmap imageWithBorders = new Bitmap(bounds.Width, bounds.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Bitmap imageWithoutBorders = null;
			try {
				RectangleF drawBounds = GraphicsUnitConverter.PixelToDoc(new RectangleF(0, 0, bounds.Width, bounds.Height));
				using (GdiGraphics g = new ImageGraphics(imageWithBorders, VerticalSplitControllerBrick.PrintingSystem)) {
					g.FillRectangle(new SolidBrush(GetExportImageBackColor()), drawBounds);
					Draw(g, drawBounds, drawBounds);
				}
				RectangleF borderFreeBounds = CalculateBorderFreeClientBounds(VerticalSplitControllerBrick, bounds);
				imageWithoutBorders = new Bitmap((int)borderFreeBounds.Width, (int)borderFreeBounds.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				using (Graphics g = Graphics.FromImage(imageWithoutBorders)) {
					g.DrawImage(imageWithBorders, borderFreeBounds.X, borderFreeBounds.Y);
				}
			} finally {
			}
			return imageWithoutBorders;
		}
		RectangleF CalculateBorderFreeClientBounds(VisualBrick brick, RectangleF bounds) {
			float offsetX = 0;
			float offsetY = 0;
			float width = bounds.Width;
			float height = bounds.Height;
			float borderWidth = brick.BorderWidth;
			if (borderWidth == 0)
				return new RectangleF(offsetX, offsetY, width, height);
			if ((brick.Sides & BorderSide.Left) != 0) {
				offsetX -= borderWidth;
				width -= borderWidth;
			}
			if ((brick.Sides & BorderSide.Right) != 0)
				width -= borderWidth;
			if ((brick.Sides & BorderSide.Top) != 0) {
				offsetY -= borderWidth;
				height -= borderWidth;
			}
			if ((brick.Sides & BorderSide.Bottom) != 0)
				height -= borderWidth;
			return new RectangleF(offsetX, offsetY, width, height);
		}
		protected internal virtual Color GetExportImageBackColor() {
			Color color = VerticalSplitControllerBrick.Control.BackColor;   
			if (color == Color.Transparent)
				color = ((XtraReport)VerticalSplitControllerBrick.Control.Report).PageColor;
			return color == Color.Transparent ? Color.White : color;
		}
		protected Size GetResolutionImageSize(Image image, IPrintingSystemContext context) {
			if (image == null)
				return System.Drawing.Size.Empty;
			return MathMethods.Scale(image.Size, VisualBrick.GetScaleFactor(context)).ToSize();
		}
	}
	[BrickExporter(typeof(VerticalSplitControllerBrickPainter))]
	public class VerticalSplitControllerBrick : SchedulerPanelBrick {
		SplitPoints splitPoints = new SplitPoints();
		ControlPrintInfo printInfo;
		public VerticalSplitControllerBrick(SplitPoints splitPoints, ControlPrintInfo printInfo, XRControl brickOwner)
			: base(brickOwner) {
			Guard.ArgumentNotNull(splitPoints, "splitPoints");
			Guard.ArgumentNotNull(printInfo, "printInfo");
			this.splitPoints.AddRange(splitPoints);
			this.printInfo = printInfo;
		}
		internal SplitPoints SplitPoints { get { return splitPoints; } }
		internal ControlPrintInfo PrintInfo { get { return printInfo; } }
		internal ReportViewControlBase Control { get { return PrintInfo.Control; } }
		public override bool SeparableVert { get { return true; } }
		protected internal virtual SplitPoints CalculateActualSplitPoints(RectangleF rect) {
			SplitPoints result = new SplitPoints();
			int count = splitPoints.Count;
			for (int i = 0; i < count; i++) {
				float point = rect.Top + splitPoints[i]; 
				result.Add(point);
			}
			return result;
		}
		public override void Dispose() {
			try {
				if (printInfo != null) {
					printInfo.Dispose();
					printInfo = null;
				}
				splitPoints = null;
			} finally {
				base.Dispose();
			}
		}
		protected internal virtual float FindAvailableSplitPoint(float pageBottom, SplitPoints actualSplitPoints) {
			float result = actualSplitPoints[0];
			int count = actualSplitPoints.Count;
			for (int i = 0; i < count; i++) {
				float point = actualSplitPoints[i];
				if (point > pageBottom)
					break;
				result = point;
			}
			return result;
		}
		protected override void OnSetPrintingSystem(bool cacheStyle) {
		}
	}
	public class PanelBrickGraphics : BrickGraphics {
		PanelBrick panel;
		public PanelBrickGraphics(PrintingSystemBase ps, PanelBrick panel)
			: base(ps) {
			if (panel == null)
				Exceptions.ThrowArgumentNullException("panel");
			this.panel = panel;
		}
		protected internal PanelBrick Panel { get { return panel; } }
		protected override Brick AddBrick(Brick brick) {
			if (brick != null) {
				brick.Modifier = Modifier;
				if (brick is ISchedulerBrick) {
					ISchedulerBrick schedulerBrick = brick as ISchedulerBrick;
					if (schedulerBrick != null && schedulerBrick.ViewInfo != null) {
						AppointmentPanelBrick appointmentBrick = panel.Bricks.OfType<AppointmentPanelBrick>().FirstOrDefault(apb => apb.ViewInfo == schedulerBrick.ViewInfo);
						if (appointmentBrick == null) {
							appointmentBrick = new AppointmentPanelBrick(schedulerBrick.ViewInfo);
							panel.Bricks.Add(appointmentBrick);
						}
						appointmentBrick.Bricks.Add(brick);
						return brick;
					}
				}
				panel.Bricks.Add(brick);
			}
			return brick;
		}
	}
	#region ControlPrintInfo
	public abstract class ControlPrintInfo : IDisposable, ICloneable {
		#region Fields
		ReportViewControlBase control;
		#endregion
		#region Ctors
		protected ControlPrintInfo() {
		}
		protected ControlPrintInfo(ReportViewControlBase control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#endregion
		#region Properties
		protected internal ReportViewControlBase Control { get { return control; } set { control = value; } }
		#endregion
		#region ICloneable Members
		object ICloneable.Clone() {
			return CloneCore();
		}
		public ControlPrintInfo Clone() {
			return CloneCore();
		}
		protected internal abstract ControlPrintInfo CloneCore();
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ControlPrintInfo() {
			Dispose(false);
		}
		#endregion
		public abstract void Print(GraphicsCache cache);
	}
	#endregion
	#region TimeCellsControlPrintInfo
	public class TimeCellsControlPrintInfo : ControlPrintInfo {
		SchedulerViewCellContainerCollection cellContainers;
		AppointmentsLayoutResult appointmentsLayoutResult;
		public TimeCellsControlPrintInfo(TimeCellsControlBase control)
			: base(control) {
			cellContainers = new SchedulerViewCellContainerCollection();
			appointmentsLayoutResult = CreateAppointmentsLayoutResult();
		}
		protected internal new TimeCellsControlBase Control { get { return (TimeCellsControlBase)base.Control; } }
		public AppointmentsLayoutResult AppointmentsLayoutResult { get { return appointmentsLayoutResult; } set { appointmentsLayoutResult = value; } }
		public SchedulerViewCellContainerCollection CellContainers { get { return cellContainers; } set { cellContainers = value; } }
		protected internal virtual AppointmentsLayoutResult CreateAppointmentsLayoutResult() {
			return new AppointmentsLayoutResult();
		}
		protected internal override ControlPrintInfo CloneCore() {
			TimeCellsControlPrintInfo printInfo = new TimeCellsControlPrintInfo(Control);
			printInfo.AppointmentsLayoutResult.Merge(AppointmentsLayoutResult);
			printInfo.CellContainers.AddRange(CellContainers);
			return printInfo;
		}
		public override void Print(GraphicsCache cache) {
			Control.Painter.Print(cache, CellContainers, AppointmentsLayoutResult, Control, Control.View.SchedulerAdapter as IStatusBrushAdapter);
		}
	}
	#endregion
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraScheduler.Reporting.TimeCellsControlBaseScripts")]
	public class TimeCellsControlBaseScripts : XRControlScripts {
		string customDrawTimeCell = String.Empty;
		string customDrawAppointment = String.Empty;
		string customDrawAppointmentBackground = String.Empty;
		string initAppointmentDisplayText = String.Empty;
		string initAppointmentImages = String.Empty;
		string appointmentViewInfoCustomizing = String.Empty;
		public TimeCellsControlBaseScripts(XRControl control)
			: base(control) {
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraScheduler.Reporting.TimeCellsControlBaseScripts.OnCustomDrawTimeCell"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(TimeCellsControlBase), TimeCellsControlBase.EventNames.CustomDrawTimeCell),
		XtraSerializableProperty,
	   ]
		public string OnCustomDrawTimeCell {
			get { return customDrawTimeCell; }
			set { customDrawTimeCell = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraScheduler.Reporting.TimeCellsControlBaseScripts.OnCustomDrawAppointment"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(TimeCellsControlBase), TimeCellsControlBase.EventNames.CustomDrawAppointment),
		XtraSerializableProperty,
		]
		public string OnCustomDrawAppointment {
			get { return customDrawAppointment; }
			set { customDrawAppointment = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraScheduler.Reporting.TimeCellsControlBaseScripts.OnCustomDrawAppointmentBackground"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(TimeCellsControlBase), TimeCellsControlBase.EventNames.CustomDrawAppointmentBackground),
		XtraSerializableProperty,
		]
		public string OnCustomDrawAppointmentBackground {
			get { return customDrawAppointmentBackground; }
			set { customDrawAppointmentBackground = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraScheduler.Reporting.TimeCellsControlBaseScripts.OnInitAppointmentDisplayText"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(TimeCellsControlBase), TimeCellsControlBase.EventNames.InitAppointmentDisplayText),
		XtraSerializableProperty,
		]
		public string OnInitAppointmentDisplayText {
			get { return initAppointmentDisplayText; }
			set { initAppointmentDisplayText = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraScheduler.Reporting.TimeCellsControlBaseScripts.OnInitAppointmentImages"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(TimeCellsControlBase), TimeCellsControlBase.EventNames.InitAppointmentImages),
		XtraSerializableProperty,
		]
		public string OnInitAppointmentImages {
			get { return initAppointmentImages; }
			set { initAppointmentImages = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraScheduler.Reporting.TimeCellsControlBaseScripts.OnAppointmentViewInfoCustomizing"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.ScriptEditor," + AssemblyInfo.SRAssemblyReportsExtensions, typeof(System.Drawing.Design.UITypeEditor)),
		NotifyParentProperty(true),
		EventScriptAttribute(typeof(TimeCellsControlBase), TimeCellsControlBase.EventNames.AppointmentViewInfoCustomizing),
		XtraSerializableProperty,
		]
		public string OnAppointmentViewInfoCustomizing {
			get { return appointmentViewInfoCustomizing; }
			set { appointmentViewInfoCustomizing = value; }
		}
	}
}
