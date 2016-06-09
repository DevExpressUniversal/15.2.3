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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.CustomEditor;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Layout;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Presets;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Printing;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Win.Customization;
using DevExpress.XtraGauges.Win.Gauges.Circular;
using DevExpress.XtraGauges.Win.Gauges.Digital;
using DevExpress.XtraGauges.Win.Gauges.Linear;
using DevExpress.XtraGauges.Win.Gauges.State;
using DevExpress.XtraGauges.Win.Printing;
using DevExpress.XtraPrinting;
using DXEvents = DevExpress.XtraGauges.Core.Customization.Events;
namespace DevExpress.XtraGauges.Win {
	[DXToolboxItem(true),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.XtraGauges.Win.Design.GaugeContainerDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner)),
	ToolboxBitmap(typeof(PointF2DConverter), "Images.GaugeControl.Win.bmp"),
	Description("Visualizes your data using various types of gauges."),
	ToolboxTabName(AssemblyInfo.DXTabData)
]
	public class GaugeControl : GaugeControlBase, IToolTipControlClient, IAnyControlEdit, ICloneable {
		IPrintable printableProviderCore;
		ComponentPrinterBase componentPrinterCore;
		GaugePrinter printerCore;
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinGauge));
		}
		public GaugeControl() {
			this.printerCore = new GaugePrinter(this);
		}
		protected override void OnCreate() {
			base.OnCreate();
			toolTipControllerCore = ToolTipController.DefaultController;
			AttachTooltipController();
		}
		protected override void OnDispose() {
			DetachTooltipController();
			if(componentPrinterCore != null) {
				componentPrinterCore.Dispose();
				componentPrinterCore = null;
			}
			if(printerCore != null) {
				printerCore.Dispose();
				printerCore = null;
			}
			base.OnDispose();
		}
		protected override IPrintable GetPrintableCore() {
			return PrintableProvider;
		}
		protected internal GaugePrinter Printer {
			get {
				if(printerCore == null) printerCore = CreateGaugePrinter();
				return printerCore;
			}
		}
		protected IPrintable PrintableProvider {
			get {
				if(printableProviderCore == null) printableProviderCore = CreatePrintableProvide();
				return printableProviderCore;
			}
		}
		protected ComponentPrinterBase ComponentPrinter {
			get {
				if(componentPrinterCore == null) componentPrinterCore = CreateComponentPrinter();
				return componentPrinterCore;
			}
		}
		protected virtual GaugePrinter CreateGaugePrinter() {
			return new GaugePrinter(this);
		}
		protected virtual IPrintable CreatePrintableProvide() {
			return new WinPrintableProvider(Printer);
		}
		protected virtual ComponentPrinterBase CreateComponentPrinter() {
			return new ComponentPrinter(PrintableProvider);
		}
		#region IToolTipControlClient
		ToolTipController toolTipControllerCore;
		[DefaultValue(null), 
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("GaugeControlToolTipController"),
#endif
 Category("Tooltip")]
		public ToolTipController ToolTipController {
			get { return toolTipControllerCore; }
			set {
				if(toolTipControllerCore == value) return;
				DetachTooltipController();
				this.toolTipControllerCore = value;
				AttachTooltipController();
			}
		}
		void AttachTooltipController() {
			if(ToolTipController != null) ToolTipController.AddClientControl(this);
		}
		void DetachTooltipController() {
			if(ToolTipController != null) {
				ToolTipController.RemoveClientControl(this);
				toolTipControllerCore = null;
			}
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return DesignMode ? null : GetToolTipObjectInfo(point);
		}
		bool showToolTipsCore = false;
		[DefaultValue(false), 
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("GaugeControlShowToolTips"),
#endif
 Category("Tooltip")]
		public bool ShowToolTips {
			get { return showToolTipsCore; }
			set { this.showToolTipsCore = value; }
		}
		protected virtual ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			ToolTipControlInfo result = null;
			BasePrimitiveHitInfo hi = (Root != null) ? Root.CalcHitInfo(pt) : null;
			if(hi != null && hi.Element != null) {
				IConvertibleScale scale = hi.Element as IConvertibleScale;
				IScaleComponent scaleComponent = hi.Element as IScaleComponent;
				if(scaleComponent != null) scale = scaleComponent.Scale as IConvertibleScale;
				result = (scale != null) ? GetValueTooltip(hi.Element, scale.Value, scale.Percent) : GetElementTooltip(hi.Element);
			}
			return result;
		}
		protected virtual ToolTipControlInfo GetValueTooltip(IRenderableElement element, float value, float percent) {
			return GetToolTipCore(element, new object[] { element.Name, value, percent });
		}
		protected virtual ToolTipControlInfo GetElementTooltip(IRenderableElement element) {
			return GetToolTipCore(element, new object[] { element.Name });
		}
		ToolTipControlInfo GetToolTipCore(IRenderableElement element, object[] values) {
			BaseGaugeWin gauge = GetGauge(element);
			string textFormat = "{1:F2}";
			string titleFormat = "";
			string text = null;
			string title = null;
			ToolTipIconType iconType = ToolTipIconType.None;
			if(gauge != null) {
				if(gauge is IDigitalGauge) {
					values = new object[] { values[0], ((IDigitalGauge)gauge).Text };
				}
				textFormat = gauge.OptionsToolTip.TooltipFormat;
				titleFormat = gauge.OptionsToolTip.TooltipTitleFormat;
				text = gauge.OptionsToolTip.Tooltip;
				title = gauge.OptionsToolTip.TooltipTitle;
				iconType = gauge.OptionsToolTip.TooltipIconType;
			}
			if(string.IsNullOrEmpty(text)) text = GetText(textFormat, values);
			if(string.IsNullOrEmpty(title)) title = GetText(titleFormat, values);
			return new ToolTipControlInfo(element, text, title, iconType);
		}
		BaseGaugeWin GetGauge(IRenderableElement element) {
			BaseGaugeModel model = BaseGaugeModel.Find(element);
			return model != null ? model.Owner as BaseGaugeWin : null;
		}
		string GetText(string format, object[] values) {
			string text = string.Empty;
			if(!string.IsNullOrEmpty(format) && values.Length > 0) {
				text = values.Length > 1 ? string.Format(format, values) : values[0].ToString();
			}
			return text;
		}
		#endregion
		#region Export
		static void AssertSizeIsNotEmpty(Size size) {
			if(size.Width <= 0 || size.Height <= 0) throw new ArgumentException("size");
		}
		public void ExportToHtml(string filePath) {
			ComponentPrinter.Export(ExportTarget.Html, filePath);
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			ComponentPrinter.Export(ExportTarget.Html, filePath, options);
		}
		public void ExportToHtml(Stream stream) {
			ComponentPrinter.Export(ExportTarget.Html, stream);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			ComponentPrinter.Export(ExportTarget.Html, stream, options);
		}
		public void ExportToMht(string filePath) {
			ComponentPrinter.Export(ExportTarget.Mht, filePath);
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			ComponentPrinter.Export(ExportTarget.Mht, filePath, options);
		}
		public void ExportToMht(Stream stream) {
			ComponentPrinter.Export(ExportTarget.Mht, stream);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			ComponentPrinter.Export(ExportTarget.Mht, stream, options);
		}
		public void ExportToPdf(string filePath) {
			ComponentPrinter.Export(ExportTarget.Pdf, filePath);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			ComponentPrinter.Export(ExportTarget.Pdf, filePath, options);
		}
		public void ExportToPdf(Stream stream) {
			ComponentPrinter.Export(ExportTarget.Pdf, stream);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			ComponentPrinter.Export(ExportTarget.Pdf, stream, options);
		}
		public void ExportToXls(string filePath) {
			ComponentPrinter.Export(ExportTarget.Xls, filePath);
		}
		public void ExportToXls(Stream stream) {
			ComponentPrinter.Export(ExportTarget.Xls, stream);
		}
		public void ExportToImage(Stream stream, ImageFormat format) {
			AssertSizeIsNotEmpty(Bounds.Size);
			IGaugeContainerEx gContainer = this as IGaugeContainerEx;
			if(format == ImageFormat.Wmf || format == ImageFormat.Emf) {
				using(gContainer.GetMetafile(stream, Bounds.Width, Bounds.Height)) { }
			}
			else {
				using(System.Drawing.Image img = gContainer.GetImage(Bounds.Width, Bounds.Height, BackColor)) {
					img.Save(stream, format);
				}
			}
		}
		public void ExportToImage(string filePath, ImageFormat format) {
			using(FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite)) {
				ExportToImage(fs, format);
				fs.Close();
			}
		}
		#endregion
		#region Printing
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrintingAvailable {
			get { return ComponentPrinterBase.IsPrintingAvailable(false); }
		}
		void SetPrinterSizeMode(PrintSizeMode sizeMode) {
			if(Printer != null) Printer.SizeMode = sizeMode;
		}
		public void ShowPrintPreview(PrintSizeMode sizeMode) {
			SetPrinterSizeMode(sizeMode);
			ComponentPrinter.ShowPreview(LookAndFeel);
		}
		public void ShowRibbonPrintPreview(PrintSizeMode sizeMode) {
			SetPrinterSizeMode(sizeMode);
			ComponentPrinter.ShowRibbonPreview(LookAndFeel);
		}
		public void Print() {
			SetPrinterSizeMode(PrintSizeMode.None);
			ComponentPrinter.Print();
		}
		public void Print(PrintSizeMode sizeMode) {
			SetPrinterSizeMode(sizeMode);
			ComponentPrinter.Print();
		}
		#endregion
		public event EventHandler EditValueChanged;
		void RaiseValueChanged() {
			if(EditValueChanged != null)
				EditValueChanged(this, EventArgs.Empty);
		}
		object ICloneable.Clone() {
			return CloneCore(this);
		}
		GaugeControl CloneCore(GaugeControl source) {
			GaugeControl result = new GaugeControl();
			using(MemoryStream ms = new MemoryStream()) {
				source.SaveLayoutToStream(ms);
				ms.Seek(0, SeekOrigin.Begin);
				result.RestoreLayoutFromStream(ms);
				ms.Close();
			}
			result.BackColor = source.BackColor;
			result.ColorScheme.Color = source.ColorScheme.Color;
			result.ColorScheme.TargetElements = source.ColorScheme.TargetElements;
			result.BorderStyle = source.BorderStyle;
			result.Size = source.Size;
			return result;
		}
		#region IAnyControlEdit Members
		bool IAnyControlEdit.AllowBitmapCache {
			get { return true; }
		}
		bool IAnyControlEdit.AllowBorder {
			get { return true; }
		}
		bool IAnyControlEdit.AllowClick(Point point) {
			return false;
		}
		Size IAnyControlEdit.CalcSize(Graphics g) {
			return Size;
		}
		void IAnyControlEdit.Draw(GraphicsCache cache, AnyControlEditViewInfo viewInfo) {
			Bounds = viewInfo.ContentRect;
			using(new GraphicsCustomizer(cache.Graphics, GraphicsProperties)) {
				Matrix oldM = Root.Self.Transform;
				using(Matrix m = new Matrix(1, 0, 0, 1, Bounds.X, Bounds.Y)) {
					OnEditValueChanging(viewInfo.EditValue);
					SetRootTransform(m);
					using(var rContext = RenderingContext.FromGraphics(cache.Graphics)) {
						Root.Self.Render(rContext);
					}
				}
				SetRootTransform(oldM);
			}
		}
		object editValue = null;
		object IAnyControlEdit.EditValue {
			get { return editValue; }
			set {
				if(value == editValue) return;
				editValue = value;
				OnEditValueChanging(value);
			}
		}
		void OnEditValueChanging(object value) {
			float fValue = 0;
			if(value is IValueProvider)
				fValue = GetValueAsSingle((value as IValueProvider).Value);
			else
				fValue = GetValueAsSingle(value);
			if(this is GaugeControlBase && value is IColorSchemeProvider) {
				ColorScheme.Color = (value as IColorSchemeProvider).ColorScheme.Color;
				ColorScheme.TargetElements = (value as IColorSchemeProvider).ColorScheme.TargetElements;
			}
			foreach(IGauge gauge in Gauges) {
				ISerizalizeableElement element = gauge as ISerizalizeableElement;
				if(element != null) {
					var children = element.GetChildren();
					foreach(ISerizalizeableElement child in children) {
						IScale scale = child as IScale;
						if(scale != null)
							scale.Value = fValue;
						ImageIndicatorComponent imageIndicator = child as ImageIndicatorComponent;
						if(imageIndicator != null && value is IImageColorProvider)
							imageIndicator.Color = (value as IImageColorProvider).ImageColor;
					}
				}
			}
		}
		float GetValueAsSingle(object value) {
			float fValue = 0f;
			if(value != null) {
				try { fValue = Convert.ToSingle(value); }
				catch { };
			}
			return fValue;
		}
		string IAnyControlEdit.GetDisplayText(object EditValue) { return null; }
		bool IAnyControlEdit.IsNeededKey(KeyEventArgs e) { return false; }
		void IAnyControlEdit.SetupAsDrawControl() { }
		void IAnyControlEdit.SetupAsEditControl() { }
		bool IAnyControlEdit.SupportsDraw { get { return true; } }
		#endregion
	}
	[ToolboxItem(false)]
	public class GaugeControlBase : Control, IGaugeContainerEx, ILayoutManagerContainer, IXtraSerializable {
		GaugeCollection gaugesCore = null;
		ModelRoot rootCore;
		bool isCustomizationModeCore = false;
		bool autoLayoutCore = true;
		BorderPainter BorderPainter;
		BorderStyles borderStyleCore;
		CustomizeManager customizeManagerCore;
		PresetHelper presetHelperCore;
		readonly UserLookAndFeel lookAndFeel;
		public GaugeControlBase() {
			SetStyle(
					ControlStyles.SupportsTransparentBackColor |
					ControlConstants.DoubleBuffer |
					ControlStyles.ResizeRedraw |
					ControlStyles.AllPaintingInWmPaint |
					ControlStyles.ResizeRedraw |
					ControlStyles.UserMouse |
					ControlStyles.UserPaint,
					true
				);
			this.borderStyleCore = BorderStyles.Default;
			lookAndFeel = new ControlUserLookAndFeel(this);
			LookAndFeel.StyleChanged += OnStyleChanged;
			UpdateStyle();
			OnCreate();
		}
		protected virtual void OnCreate() {
			this.gaugesCore = new GaugeCollection();
			graphicsPropertiesCore = new DevExpress.XtraGauges.Core.Drawing.GraphicsProperties();
			colorSchemeCore = new ColorScheme();
			colorSchemeCore.PropertyChanged += OnColorSchemeChanged;
			Gauges.CollectionChanged += OnGaugeCollectionChanged;
			this.rootCore = new ModelRoot();
			this.customizeManagerCore = CreateCustomizeManager();
			this.presetHelperCore = new PresetHelper(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && !DesignMode) {
				LookAndFeel.StyleChanged -= OnStyleChanged;
				colorSchemeCore.PropertyChanged -= OnColorSchemeChanged;
				OnDispose();
			}
			base.Dispose(disposing);
		}
		protected virtual void OnDispose() {
			if(CustomizeManager != null) {
				((IDisposable)CustomizeManager).Dispose();
				customizeManagerCore = null;
			}
			if(Gauges != null) {
				List<IGauge> list = new List<IGauge>(Gauges);
				foreach(IGauge gauge in list)
					gauge.Dispose();
				Gauges.CollectionChanged -= OnGaugeCollectionChanged;
				gaugesCore = null;
			}
			BorderPainter = null;
			BorderSkinElement = null;
		}
		void OnStyleChanged(object sender, EventArgs e) {
			UpdateStyle();
		}
		SkinElement BorderSkinElement;
		void UpdateStyle() {
			defaultAppearance = null;
			UpdateBorder();
			UpdateRangeBarSkin();
			UpdateLabelSkin();
		}
		void UpdateRangeBarSkin() {
			defaultAppearanceRangeBar = null;
			UpdateChildren();
		}
		void UpdateLabelSkin() {
			defaultAppearanceText = null;
			UpdateChildren();
		}
		protected virtual void UpdateChildren() {
			if(Gauges != null) {
				for(int i = 0; i < Gauges.Count; i++)
					Gauges[i].ForceUpdateChildren();
			}
		}
		void UpdateBorder() {
			BorderSkinElement = null;
			BorderPainter = null;
			if(BorderStyle == BorderStyles.Default) {
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					BorderSkinElement = GetBackgroundSkin(LookAndFeel);
					return;
				}
			}
			BorderPainter = BorderHelper.GetPainter(BorderStyle, LookAndFeel);
		}
		public void InitializeDefault(object parameter) {
			IGauge[] gaugesToDispose = Gauges.ToArray();
			Gauges.Clear();
			foreach(IGauge gauge in gaugesToDispose) {
				gauge.Dispose();
			}
			if(parameter is GaugeType) {
				IGauge defaultGauge = AddGauge((GaugeType)parameter);
				defaultGauge.InitializeDefault();
			}
			else {
				using(MemoryStream memoryStream = new MemoryStream()) {
					if(parameter is IGaugeContainer && parameter != this) {
						((IGaugeContainer)parameter).SaveLayoutToStream(memoryStream);
						parameter = memoryStream;
					}
					PresetHelper.RestoreLayoutCore(new XmlXtraSerializer(), parameter);
				}
			}
		}
		protected internal CustomizeManager CustomizeManager {
			get { return customizeManagerCore; }
		}
		CustomizeManager IGaugeContainer.CustomizeManager {
			get { return CustomizeManager; }
		}
		ColorScheme colorSchemeCore;
		[XtraSerializableProperty(XtraSerializationVisibility.Content), Category("Appearance"), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorScheme ColorScheme {
			get { return colorSchemeCore; }
		}
		public bool ShouldSerializeColorScheme() {
			return ColorScheme.ShouldSerialize();
		}
		public void ResetColorScheme() {
			ColorScheme.Reset();
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("GaugeControlBaseBorderStyle"),
#endif
Category("Appearance")]
		[DefaultValue(BorderStyles.Default)]
		public BorderStyles BorderStyle {
			get { return borderStyleCore; }
			set {
				if(BorderStyle == value) return;
				this.borderStyleCore = value;
				UpdateBorder();
				Invalidate();
			}
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("GaugeControlBaseLookAndFeel"),
#endif
		Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UserLookAndFeel LookAndFeel {
			get { return lookAndFeel; }
		}
		CustomizationFrameBase[] IGaugeContainer.OnCreateCustomizeFrames(IGauge gauge, CustomizationFrameBase[] frames) {
			CustomizationFrameBase[] newFrames = new CustomizationFrameBase[frames.Length + 1];
			newFrames[0] = new ActionListFrame(gauge.Model);
			for(int i = 1; i < newFrames.Length; i++) {
				newFrames[i] = frames[i - 1];
			}
			return newFrames;
		}
		protected override Size DefaultSize {
			get { return new Size(100, 100); }
		}
		protected ModelRoot Root {
			get { return rootCore; }
		}
		void IGaugeContainer.DesignerLoaded() {
			foreach(IGauge gauge in Gauges) AddGaugeToDTComponents(gauge);
			UpdateCustomizationModeState();
		}
		BasePrimitiveHitInfo IGaugeContainer.CalcHitInfo(Point p) {
			return Root.CalcHitInfo(p);
		}
		event EventHandler ModelChangedCore;
		event EventHandler IGaugeContainer.ModelChanged {
			add { ModelChangedCore += value; }
			remove { ModelChangedCore -= value; }
		}
		protected void RaiseModelChanged() {
			if(ModelChangedCore != null) ModelChangedCore(this, EventArgs.Empty);
		}
		void IGaugeContainer.OnModelChanged(BaseGaugeModel oldModel, BaseGaugeModel model) {
			if(model != null) (this as IGaugeContainer).UpdateGaugesZOrder();
			(this as IGaugeContainer).AddPrimitive(model);
			RaiseModelChanged();
			if(oldModel == CustomizeManager.SelectedClient) {
				CustomizeManager.SelectedClient = model;
			}
		}
		void IGaugeContainer.AddPrimitive(IElement<IRenderableElement> primitve) {
			Root.Composite.Add(primitve);
		}
		bool forceClearOnRestoreCore = false;
		bool IGaugeContainer.ForceClearOnRestore {
			get { return forceClearOnRestoreCore; }
			set { forceClearOnRestoreCore = value; }
		}
		bool IGaugeContainer.DesignMode {
			get { return DesignMode; }
		}
		void IGaugeContainer.RemovePrimitive(IElement<IRenderableElement> primitve) {
			Root.Composite.Remove(primitve);
		}
		void OnColorSchemeChanged(object sender, PropertyChangedEventArgs e) {
			UpdateChildren();
		}
		void OnGaugeCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<IGauge> ea) {
			switch(ea.ChangedType) {
				case ElementChangedType.ElementAdded: OnGaugeAdded(ea); break;
				case ElementChangedType.ElementRemoved: OnGaugeRemoved(ea); break;
			}
		}
		protected virtual void OnGaugeRemoved(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<IGauge> ea) {
			((IGaugeContainer)this).RemovePrimitive(ea.Element.Model);
			RemoveGaugeFromDTComponents(ea.Element);
			PresetHelper.OnRemoveItem(ea);
		}
		protected virtual void OnGaugeAdded(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<IGauge> ea) {
			AddGaugeToDTComponents(ea.Element);
			PresetHelper.OnAddItem(ea);
			((IGaugeContainer)this).UpdateGaugesZOrder();
		}
		void IGaugeContainer.UpdateGaugesZOrder() {
			int i = Gauges.Count;
			foreach(IGauge gauge in Gauges) {
				i--;
				gauge.Model.ZOrder = i + 60;
			}
		}
		void RemoveGaugeFromDTComponents(IGauge gauge) {
			if(Site != null) Site.Container.Remove(gauge as IComponent);
			if(gauge != null) gauge.SetContainer(null);
			if(CustomizeManager != null) CustomizeManager.SelectedClient = null;
			((ILayoutManagerContainer)this).DoLayout();
		}
		void AddGaugeToDTComponents(IComponent component) {
			if(Site != null) Site.Container.Add(component);
			IGauge gauge = component as IGauge;
			if(gauge != null) gauge.SetContainer(this);
			((ILayoutManagerContainer)this).DoLayout();
		}
		void IGaugeContainer.ComponentChanging(IComponent component, string property) {
			Changing(component, property);
		}
		void IGaugeContainer.ComponentChanged(IComponent component, string property, object oldValue, object newValue) {
			Changed(component, property, oldValue, newValue);
			if(property != "Bounds") {
				ICustomizationFrameClient client = CustomizeManager.SelectedClient;
			};
		}
		protected virtual void Changing(IComponent component, string property) {
			if(ComponentChangeService != null) {
				try {
					ComponentChangeService.OnComponentChanging(
						component, (property != null) ? TypeDescriptor.GetProperties(component)[property] : null);
				}
				catch { }
			}
		}
		protected virtual void Changed(IComponent component, string property, object oldValue, object newValue) {
			if(ComponentChangeService != null) {
				try {
					ComponentChangeService.OnComponentChanged(component,
						(property != null) ? TypeDescriptor.GetProperties(component)[property] : null
						, oldValue, newValue);
				}
				catch { }
			}
		}
		IComponentChangeService componentChangeService;
		protected IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null && Site != null)
					componentChangeService = Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return componentChangeService;
			}
		}
		protected virtual LayoutManager CreateLayoutManager() {
			return new LayoutManager(this, true);
		}
		protected virtual CustomizeManager CreateCustomizeManager() {
			return new CustomizeManager(this);
		}
		Cursor cursorCore = Cursors.Arrow;
		void IGaugeContainer.SetCursor(CursorInfo ci) {
			switch(ci) {
				case CursorInfo.Hand: cursorCore = Cursors.Hand; break;
				case CursorInfo.HSizing: cursorCore = Cursors.SizeWE; break;
				case CursorInfo.VSizing: cursorCore = Cursors.SizeNS; break;
				case CursorInfo.Move: cursorCore = Cursors.SizeAll; break;
				case CursorInfo.NWSESizing: cursorCore = Cursors.SizeNWSE; break;
				case CursorInfo.NESWSizing: cursorCore = Cursors.SizeNESW; break;
				case CursorInfo.Normal: cursorCore = Cursors.Default; break;
			}
		}
#if !SL
	[DevExpressXtraGaugesWinLocalizedDescription("GaugeControlBaseCursor")]
#endif
		public override Cursor Cursor {
			get {
				if(cursorCore == Cursors.Default) return base.Cursor;
				return cursorCore;
			}
			set { base.Cursor = value; }
		}
		public void InvalidateRect(RectangleF bounds) {
			if(DesignMode) Invalidate();
			else Invalidate(Rectangle.Round(bounds));
		}
		public void UpdateRect(RectangleF rect) {
			if(CustomizeManager != null) rect = CustomizeManager.CorrectRenderRect(rect);
			if(rect == RectangleF.Empty) rect = ClientRectangle;
			Invalidate(
				new Rectangle(
				(int)rect.X,
				(int)rect.Y,
				(int)rect.Width + 1,
				(int)rect.Height + 1));
			if(Site != null) Update();
		}
		Image IGaugeContainerEx.GetImage(int width, int height, Color background) {
			return GetImageCore(width, height, background);
		}
		Image IGaugeContainer.GetImage(int width, int height) {
			return GetImageCore(width, height);
		}
		Metafile IGaugeContainerEx.GetMetafile(Stream stream, int width, int height) {
			return GetMetafileCore(stream, width, height);
		}
		IPrintable IGaugeContainerEx.Printable {
			get { return GetPrintableCore(); }
		}
		protected virtual IPrintable GetPrintableCore() { return null; }
		protected Image GetImageCore(int width, int height, Color background) {
			Image result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			DrawToImage(result, width, height, background);
			return result;
		}
		protected Image GetImageCore(int width, int height) {
			Image result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			DrawToImage(result, width, height, null);
			return result;
		}
		protected Metafile GetMetafileCore(Stream stream, int width, int height) {
			Metafile metaFile = CreateMetafile(width, height, stream);
			DrawToImage(metaFile, width, height, BackColor);
			return metaFile;
		}
		static Metafile CreateMetafile(int width, int height, Stream stream) {
			Metafile metaFile;
			IntPtr hDC = DevExpress.Utils.Drawing.Helpers.NativeMethods.GetWindowDC(IntPtr.Zero);
			try {
				metaFile = new Metafile(stream, hDC, new RectangleF(0, 0, width, height), MetafileFrameUnit.Pixel, EmfType.EmfPlusDual);
			}
			finally { DevExpress.Utils.Drawing.Helpers.NativeMethods.ReleaseDC(IntPtr.Zero, hDC); }
			return metaFile;
		}
		protected void SetRootTransform(Matrix m) {
			foreach(BaseGauge bg in Gauges) bg.BeginUpdate();
			Root.Self.Transform = m;
			foreach(BaseGauge bg in Gauges) bg.EndUpdate();
		}
		protected void DrawToImage(Image image, int width, int height, Color? background) {
			MakeAnyElementsInvisibleCore(true);
			float sx = (float)width / ClientSize.Width;
			float sy = (float)height / ClientSize.Height;
			using(Graphics g = Graphics.FromImage(image)) {
				if(background.HasValue)
					g.Clear(background.Value);
				Matrix m = g.Transform.Clone();
				Matrix oldM = Root.Self.Transform;
				m.Scale(sx, sy);
				bool matrixEquals = object.Equals(m, oldM);
				if(!matrixEquals)
					SetRootTransform(m);
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
				if(!matrixEquals)
					Root.Self.Transform = m;
				Root.Self.Render(g);
				if(!matrixEquals)
					SetRootTransform(oldM);
			}
			MakeAnyElementsInvisibleCore(false);
		}
		void MakeAnyElementsInvisibleCore(bool value) {
			for(int i = 0; i < Gauges.Count; i++) Gauges[i].SuppressDrawBorder = value;
			foreach(IRenderableElement e in Root.Composite.Elements) {
				if(e is CustomizationFrameBase || e is HotTrackFrame) {
					e.Accept(
							delegate(IElement<IRenderableElement> frameElement) {
								frameElement.Self.Renderable = !value;
							}
						);
				}
			}
		}
		protected internal PresetHelper PresetHelper {
			get { return presetHelperCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("GaugeControlBaseGauges"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GaugeCollection Gauges {
			get { return gaugesCore; }
		}
		protected bool GetCustomizationModeState() {
			return (this as IGaugeContainer).EnableCustomizationMode || (Site != null && Site.DesignMode);
		}
		bool IGaugeContainer.EnableCustomizationMode {
			get { return isCustomizationModeCore; }
			set {
				if(isCustomizationModeCore == value) return;
				isCustomizationModeCore = value;
				UpdateCustomizationModeState();
			}
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("GaugeControlBaseAutoLayout"),
#endif
DefaultValue(true), Category("Layout")]
		[XtraSerializableProperty]
		public bool AutoLayout {
			get { return autoLayoutCore; }
			set {
				if(autoLayoutCore != value) {
					autoLayoutCore = value;
					if(AutoLayout)
						((ILayoutManagerContainer)this).DoLayout();
					UpdateRect(Rectangle.Empty);
				}
			}
		}
		void UpdateCustomizationModeState() {
			CustomizeManager.Enabled = GetCustomizationModeState();
			for(int i = 0; i < Gauges.Count; i++) {
				Gauges[i].Model.UpdateBackgroundShape();
			}
			UpdateRect(ClientRectangle);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			ProcessMessage(DXEvents.MouseDown, e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			ProcessMessage(DXEvents.MouseUp, e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			ProcessMessage(DXEvents.MouseMove, e);
		}
		protected virtual void ProcessMessage(Events eventType, MouseEventArgs mea) {
			if(CustomizeManager != null) CustomizeManager.ProcessMessage(eventType, mea);
		}
		protected override void OnResize(EventArgs e) {
			((ILayoutManagerContainer)this).DoLayout();
			base.OnResize(e);
		}
		protected override void OnBindingContextChanged(EventArgs e) {
			base.OnBindingContextChanged(e);
			UpdateComponentBindings();
		}
		bool lockUpdateComponentBindings = false;
		protected void UpdateComponentBindings() {
			if(lockUpdateComponentBindings) return;
			try {
				lockUpdateComponentBindings = true;
				foreach(IGauge gauge in Gauges) {
					ISerizalizeableElement element = gauge as ISerizalizeableElement;
					if(element != null) {
						List<ISerizalizeableElement> children = element.GetChildren();
						foreach(ISerizalizeableElement child in children) {
							if(child is IBindableComponent) {
								((IBindableComponent)child).BindingContext = BindingContext;
							}
						}
						if(element is IBindableComponent) ((IBindableComponent)element).BindingContext = BindingContext;
					}
				}
			}
			finally {
				lockUpdateComponentBindings = false;
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				GraphicsInfoArgs ea = new GraphicsInfoArgs(cache, e.ClipRectangle);
				UpdateBeforePaint(ea);
				DrawBorder(ea);
				DrawContent(ea);
			}
		}
		protected void UpdateBeforePaint(GraphicsInfoArgs ea) {
			for(int i = 0; i < Gauges.Count; i++)
				Gauges[i].CheckEnabledState(Enabled);
		}
		GraphicsProperties graphicsPropertiesCore;
		[XtraSerializableProperty, Category("Appearance"), 
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("GaugeControlBaseGraphicsProperties"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GraphicsProperties GraphicsProperties {
			get { return graphicsPropertiesCore; }
		}
		public bool ShouldSerializeGraphicsProperties() {
			return GraphicsProperties.ShouldSerialize();
		}
		public void ResetGraphicsProperties() {
			GraphicsProperties.Reset();
		}
		protected void DrawContent(GraphicsInfoArgs ea) {
			using(new GraphicsCustomizer(ea.Graphics, GraphicsProperties)) {
				using(IRenderingContext rContext = RenderingContext.FromGraphics(ea.Graphics)) {
					Root.Self.Render(rContext);
				}
			}
		}
		internal class GraphicsCustomizer : IDisposable {
			GraphicsState savedState;
			Graphics graphicsCore;
			public GraphicsCustomizer(Graphics g, GraphicsProperties graphicsProperties) {
				savedState = g.Save();
				graphicsCore = g;
				g.SmoothingMode = graphicsProperties.SmoothingMode;
				g.InterpolationMode = graphicsProperties.InterpolationMode;
				g.CompositingQuality = graphicsProperties.CompositingQuality;
				g.TextRenderingHint = graphicsProperties.TextRenderingHint;
			}
			public void Dispose() {
				graphicsCore.Restore(savedState);
			}
		}
		protected void DrawBorder(GraphicsInfoArgs e) {
			ObjectInfoArgs info;
			ObjectPainter painter = BorderPainter;
			if(painter == null) {
				painter = SkinElementPainter.Default;
				info = new SkinElementInfo(BorderSkinElement);
			}
			else info = new BorderObjectInfoArgs();
			info.Cache = e.Cache;
			info.Bounds = new Rectangle(Point.Empty, Size);
			info.State = Enabled ? ObjectState.Normal : ObjectState.Disabled;
			ObjectPainter.DrawObject(e.Cache, painter, info);
			Rectangle clentRect = ObjectPainter.GetObjectClientRectangle(e.Cache.Graphics, painter, info);
			if(!backColor.IsEmpty)
				e.Cache.FillRectangle(backColor, clentRect);
			if(BackgroundImage != null) {
				BackgroundImagePainter.DrawBackgroundImage(
					e.Cache.Graphics, BackgroundImage, BackColor, BackgroundImageLayout,
					clentRect, clentRect, Point.Empty, RightToLeft);
			}
		}
		#region ILayoutManagerContainer
		void ILayoutManagerContainer.DoLayout() {
			if(AutoLayout && !Disposing && !IsDisposed)
				CreateLayoutManager().Layout();
		}
		List<ILayoutManagerClient> ILayoutManagerContainer.Clients {
			get {
				List<ILayoutManagerClient> list = new List<ILayoutManagerClient>();
				foreach(BaseGaugeWin gb in Gauges) {
					ILayoutManagerClient client = gb as ILayoutManagerClient;
					if(client != null) list.Add(client);
				}
				return list;
			}
		}
		Rectangle ILayoutManagerContainer.Bounds {
			get { return ClientRectangle; }
		}
		IThickness ILayoutManagerContainer.LayoutPadding {
			get { return paddingCore; }
			set {
				if(paddingCore == value) return;
				paddingCore = (Thickness)value;
				((ILayoutManagerContainer)this).DoLayout();
				Invalidate();
			}
		}
		Thickness paddingCore = new Thickness(6);
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("GaugeControlBaseLayoutPadding"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public Thickness LayoutPadding {
			get { return paddingCore; }
			set { ((ILayoutManagerContainer)this).LayoutPadding = value; }
		}
		int intervalCore = 6;
		[DefaultValue(6)]
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("GaugeControlBaseLayoutInterval"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public int LayoutInterval {
			get { return intervalCore; }
			set {
				if(intervalCore == value) return;
				intervalCore = value;
				((ILayoutManagerContainer)this).DoLayout();
				Invalidate();
			}
		}
		internal void ResetLayoutPadding() {
			LayoutPadding = new Thickness(6);
		}
		internal bool ShouldSerializeLayoutPadding() {
			return LayoutPadding.All != 6;
		}
		#endregion
		#region HideProperties
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RightToLeft RightToLeft {
			get { return base.RightToLeft; }
			set { base.RightToLeft = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty,
		]
		public Size SizeEx {
			get { return Size; }
			set { Size = value; }
		}
		#endregion
		#region UserPropertiesAndMethods
		public DigitalGauge AddDigitalGauge() {
			return AddGauge(GaugeType.Digital) as DigitalGauge;
		}
		public LinearGauge AddLinearGauge() {
			return AddGauge(GaugeType.Linear) as LinearGauge;
		}
		public CircularGauge AddCircularGauge() {
			return AddGauge(GaugeType.Circular) as CircularGauge;
		}
		public StateIndicatorGauge AddStateIndicatorGauge() {
			return AddGauge(GaugeType.StateIndicator) as StateIndicatorGauge;
		}
		public IGauge AddGauge(GaugeType type) {
			IGauge gauge = null;
			switch(type) {
				case GaugeType.Circular: gauge = new CircularGauge(); break;
				case GaugeType.Linear: gauge = new LinearGauge(); break;
				case GaugeType.Digital:
					gauge = new DigitalGauge();
					using(Core.GaugeSettings settings = Core.GaugeSettings.FromGauge(gauge)) {
						((DigitalGauge)gauge).AppearanceOn.ContentBrush = new Core.Drawing.SolidBrushObject(Color.Black);
						settings.TextSettings.Text = "00.000";
					}
					break;
				case GaugeType.StateIndicator: gauge = new StateIndicatorGauge(); break;
			}
			if(gauge != null) {
				gauge.Name = UniqueNameHelper.GetUniqueName("Gauge", CollectionHelper.GetNames(Gauges), 0);
				Gauges.Add(gauge);
			}
			return gauge;
		}
		#endregion
		#region IXtraSerializeable
		#endregion
		Color backColor = Color.Empty;
#if !SL
	[DevExpressXtraGaugesWinLocalizedDescription("GaugeControlBaseBackColor")]
#endif
		public override Color BackColor {
			get { return GetColor(backColor, DefaultAppearance.BackColor); }
			set {
				if(backColor == value) return;
				backColor = value;
				Invalidate();
			}
		}
		AppearanceDefault defaultAppearanceText;
		protected internal AppearanceDefault DefaultAppearanceText {
			get {
				if(defaultAppearanceText == null)
					defaultAppearanceText = CreateDefaultAppearanceText();
				return defaultAppearanceText;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceText() {
			AppearanceDefault app = new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, SystemFonts.DefaultFont);
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = GetLabelSkin(LookAndFeel);
				if(element != null)
					element.ApplyForeColorAndFont(app);
				LookAndFeelHelper.CheckColors(LookAndFeel, app, this);
			}
			return app;
		}
		AppearanceDefault defaultAppearanceRangeBar;
		protected internal AppearanceDefault DefaultAppearanceRangeBar {
			get {
				if(defaultAppearanceRangeBar == null)
					defaultAppearanceRangeBar = CreateDefaultAppearanceRangeBar();
				return defaultAppearanceRangeBar;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceRangeBar() {
			AppearanceDefault app = new AppearanceDefault(SystemColors.WindowText, SystemColors.Window);
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = GetRangeBarSkin(LookAndFeel);
				if(element != null)
					element.Apply(app);
				LookAndFeelHelper.CheckColors(LookAndFeel, app, this);
			}
			app.BackColor = Color.FromArgb(90, app.BackColor);
			return app;
		}
		AppearanceDefault defaultAppearance;
		protected virtual AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault app = new AppearanceDefault(SystemColors.WindowText, SystemColors.Window);
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = GetBackgroundSkin(LookAndFeel);
				if(element != null)
					element.Apply(app);
				LookAndFeelHelper.CheckColors(LookAndFeel, app, this);
			}
			return app;
		}
		static Color GetColor(Color color, Color defColor) {
			return (color == Color.Empty) ? defColor : color;
		}
		static SkinElement GetBackgroundSkin(ISkinProvider provider) {
			Skin skin = GaugesSkins.GetSkin(provider);
			return (skin != null) ? skin[GaugesSkins.SkinBackground] : null;
		}
		static SkinElement GetRangeBarSkin(ISkinProvider provider) {
			Skin skin = GaugesSkins.GetSkin(provider);
			return (skin != null) ? skin[GaugesSkins.SkinRangeBar] : null;
		}
		static SkinElement GetLabelSkin(ISkinProvider provider) {
			Skin skin = GaugesSkins.GetSkin(provider);
			return (skin != null) ? skin[GaugesSkins.SkinLabel] : null;
		}
		public bool ShouldSerializeBackColor() {
			return !backColor.IsEmpty;
		}
		public override void ResetBackColor() {
			backColor = Color.Empty;
		}
		#region IXtraSerializable Members
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((IXtraSerializable)PresetHelper).OnEndDeserializing(restoredVersion);
		}
		void IXtraSerializable.OnEndSerializing() {
			((IXtraSerializable)PresetHelper).OnEndSerializing();
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			if(CustomizeManager != null)
				CustomizeManager.ResetSelection();
			((IXtraSerializable)PresetHelper).OnStartDeserializing(e);
		}
		void IXtraSerializable.OnStartSerializing() {
			((IXtraSerializable)PresetHelper).OnStartSerializing();
		}
		public void SaveStyleToXml(string path) {
			PresetHelper.SaveStyle(path);
		}
		public void RestoreStyleFromXml(string path) {
			PresetHelper.RestoreStyle(path);
		}
		public void SaveStyleToStream(Stream stream) {
			PresetHelper.SaveStyle(stream);
		}
		public void RestoreStyleFromStream(Stream stream) {
			PresetHelper.RestoreStyle(stream);
		}
		public virtual void SaveLayoutToStream(Stream stream) {
			PresetHelper.SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void RestoreLayoutFromStream(Stream stream) {
			PresetHelper.RestoreLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void SaveLayoutToXml(string xmlFile) {
			PresetHelper.SaveLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			PresetHelper.RestoreLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void SaveLayoutToRegistry(string path) {
			PresetHelper.SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			PresetHelper.RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		object XtraFindItemsItem(XtraItemEventArgs e) {
			return PresetHelper.XtraFindItemsItem(e);
		}
		void IGaugeContainer.AddToParentCollection(ISerizalizeableElement element, ISerizalizeableElement parent) {
			string originalName = element.Name;
			BaseGaugeWin gb = parent as BaseGaugeWin;
			if(gb != null) {
				switch(element.ParentCollectionName) {
					case "Labels": gb.Labels.Add((LabelComponent)element); break;
					case "Images": gb.Images.Add((ImageIndicatorComponent)element); break;
				}
			}
			ICircularGauge cg = parent as ICircularGauge;
			if(cg != null) {
				switch(element.ParentCollectionName) {
					case "Scales": cg.Scales.Add((ArcScaleComponent)element); break;
					case "BackgroundLayers": cg.BackgroundLayers.Add((ArcScaleBackgroundLayerComponent)element); break;
					case "Markers": cg.Markers.Add((ArcScaleMarkerComponent)element); break;
					case "Needles": cg.Needles.Add((ArcScaleNeedleComponent)element); break;
					case "RangeBars": cg.RangeBars.Add((ArcScaleRangeBarComponent)element); break;
					case "SpindleCaps": cg.SpindleCaps.Add((ArcScaleSpindleCapComponent)element); break;
					case "EffectLayers": cg.EffectLayers.Add((ArcScaleEffectLayerComponent)element); break;
					case "Indicators": cg.Indicators.Add((ArcScaleStateIndicatorComponent)element); break;
					case "ImageIndicators": cg.ImageIndicators.Add((StateImageIndicatorComponent)element); break;
				}
			}
			ILinearGauge lg = parent as ILinearGauge;
			if(lg != null) {
				switch(element.ParentCollectionName) {
					case "Scales": lg.Scales.Add((LinearScaleComponent)element); break;
					case "BackgroundLayers": lg.BackgroundLayers.Add((LinearScaleBackgroundLayerComponent)element); break;
					case "Levels": lg.Levels.Add((LinearScaleLevelComponent)element); break;
					case "Markers": lg.Markers.Add((LinearScaleMarkerComponent)element); break;
					case "RangeBars": lg.RangeBars.Add((LinearScaleRangeBarComponent)element); break;
					case "EffectLayers": lg.EffectLayers.Add((LinearScaleEffectLayerComponent)element); break;
					case "Indicators": lg.Indicators.Add((LinearScaleStateIndicatorComponent)element); break;
				}
			}
			IDigitalGauge dg = parent as IDigitalGauge;
			if(dg != null) {
				switch(element.ParentCollectionName) {
					case "BackgroundLayers": dg.BackgroundLayers.Add((DigitalBackgroundLayerComponent)element); break;
					case "EffectLayers": dg.EffectLayers.Add((DigitalEffectLayerComponent)element); break;
				}
			}
			IStateIndicatorGauge sg = parent as IStateIndicatorGauge;
			if(sg != null) {
				switch(element.ParentCollectionName) {
					case "Indicators": sg.Indicators.Add((StateIndicatorComponent)element); break;
				}
			}
			element.Name = originalName;
		}
		ISerizalizeableElement IGaugeContainer.CreateSerializableInstance(XtraPropertyInfo info, XtraPropertyInfo infoType) {
			ISerizalizeableElement result = null;
			switch(infoType.Value.ToString()) {
				case "ImageIndicatorComponent":
					result = new ImageIndicatorComponent();
					break;
				case "LabelComponent":
					result = new LabelComponent();
					break;
				case "DigitalGauge":
					result = new DigitalGauge();
					break;
				case "DigitalBackgroundLayerComponent":
					result = new DigitalBackgroundLayerComponent();
					break;
				case "DigitalEffectLayerComponent":
					result = new DigitalEffectLayerComponent();
					break;
				case "CircularGauge":
					result = new CircularGauge();
					break;
				case "ArcScaleBackgroundLayerComponent":
					result = new ArcScaleBackgroundLayerComponent();
					break;
				case "ArcScaleComponent":
					result = new ArcScaleComponent();
					break;
				case "ArcScaleMarkerComponent":
					result = new ArcScaleMarkerComponent();
					break;
				case "ArcScaleNeedleComponent":
					result = new ArcScaleNeedleComponent();
					break;
				case "ArcScaleRangeBarComponent":
					result = new ArcScaleRangeBarComponent();
					break;
				case "ArcScaleSpindleCapComponent":
					result = new ArcScaleSpindleCapComponent();
					break;
				case "ArcScaleEffectLayerComponent":
					result = new ArcScaleEffectLayerComponent();
					break;
				case "ArcScaleStateIndicatorComponent":
					result = new ArcScaleStateIndicatorComponent();
					break;
				case "StateImageIndicatorComponent":
					result = new StateImageIndicatorComponent();
					break;
				case "LinearGauge":
					result = new LinearGauge();
					break;
				case "LinearScaleBackgroundLayerComponent":
					result = new LinearScaleBackgroundLayerComponent();
					break;
				case "LinearScaleComponent":
					result = new LinearScaleComponent();
					break;
				case "LinearScaleMarkerComponent":
					result = new LinearScaleMarkerComponent();
					break;
				case "LinearScaleLevelComponent":
					result = new LinearScaleLevelComponent();
					break;
				case "LinearScaleRangeBarComponent":
					result = new LinearScaleRangeBarComponent();
					break;
				case "LinearScaleEffectLayerComponent":
					result = new LinearScaleEffectLayerComponent();
					break;
				case "LinearScaleStateIndicatorComponent":
					result = new LinearScaleStateIndicatorComponent();
					break;
				case "StateIndicatorGauge":
					result = new StateIndicatorGauge();
					break;
				case "StateIndicatorComponent":
					result = new StateIndicatorComponent();
					break;
			}
			result.Name = (string)info.Value;
			return result;
		}
		[XtraSerializableProperty(false, true, false), EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public List<ISerizalizeableElement> Items {
			get { return PresetHelper.Items; }
			set { PresetHelper.Items = value; }
		}
		#endregion
	}
}
