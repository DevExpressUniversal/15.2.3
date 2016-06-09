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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxGauges.Base;
using DevExpress.Web.ASPxGauges.Design;
using DevExpress.Web.ASPxGauges.Gauges;
using DevExpress.Web.ASPxGauges.Gauges.Circular;
using DevExpress.Web.ASPxGauges.Gauges.Digital;
using DevExpress.Web.ASPxGauges.Gauges.Linear;
using DevExpress.Web.ASPxGauges.Gauges.State;
using DevExpress.Web.ASPxGauges.Printing;
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
using DevExpress.XtraPrinting;
using System.Drawing.Drawing2D;
namespace DevExpress.Web.ASPxGauges {
	[ToolboxTabName(AssemblyInfo.DXTabData)]
	[DevExpress.Utils.Design.DXClientDocumentationProvider("#AspNet/DevExpressWebASPxGaugesScripts"),
	System.Runtime.InteropServices.ComVisible(false),
	Designer("DevExpress.Web.ASPxGauges.Design.ASPxGaugeControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner)),
	ToolboxBitmap(typeof(PointF2DConverter), "Images.GaugeControl.Web.bmp"),
	DXWebToolboxItem(true),
	Description("Visualizes your data using various types of gauges.")
]
	[ToolboxData("<{0}:ASPxGaugeControl Height=\"250px\" Width=\"250px\" runat=\"server\"></{0}:ASPxGaugeControl>")]
	[ControlBuilder(typeof(ASPxGaugeControlBuilder))]
	public class ASPxGaugeControl : ASPxWebControl, IGaugeContainerEx, ILayoutManagerContainer, INamingContainer, IXtraSerializable, IRequiresLoadPostDataControl {
		IPrintable printableProviderCore;
		ComponentPrinterBase componentPrinterCore;
		GaugePrinter printerCore;
		System.Web.UI.WebControls.Image imageCore;
		bool isDisposingInProgressCore = false;
		GaugeCollectionWeb gaugesCore;
		ModelRoot rootCore;
		PresetHelper presetHelperCore;
		static readonly object customCallback = new object();
		public const string ASPxGaugeControlScriptResourceName = "DevExpress.Web.ASPxGauges.Scripts.ASPxGaugeControl.js";
		protected const string UrlKey = "url";
		internal object EditorForm;
		internal object Designer;
		readonly Font ContainerFont;
		readonly StringFormat ContainerStringFormat;
		const string EmptyContainerText =
			"Run the Preset Manager \r\nto load gauges from a preset.\r\n" +
			"Run the Visual Gauge Control Designer\r\n to create gauges from the ground up.";
		public ASPxGaugeControl()
			: base() {
			ContainerFont = new Font("Tahoma", 8.25f);
			ContainerStringFormat = new StringFormat();
			ContainerStringFormat.LineAlignment = StringAlignment.Center;
			ContainerStringFormat.Alignment = StringAlignment.Center;
			OnCreate();
		}
		public override void Dispose() {
			if(!isDisposingInProgressCore) {
				isDisposingInProgressCore = true;
				OnDispose();
			}
			base.Dispose();
		}
		protected void OnCreate() {
			this.rootCore = new ModelRoot();
			this.gaugesCore = new GaugeCollectionWeb();
			graphicsPropertiesCore = new GraphicsProperties();
			colorSchemeCore = new ColorScheme();
			colorSchemeCore.PropertyChanged += OnColorSchemeChanged;
			Gauges.CollectionChanged += OnGaugeCollectionChanged;
			this.presetHelperCore = new PresetHelper(this);
			Width = 250;
			Height = 250;
			BackColor = Color.White;
		}
		protected void OnDispose() {
			if(componentPrinterCore != null) {
				componentPrinterCore.Dispose();
				componentPrinterCore = null;
			}
			if(printerCore != null) {
				printerCore.Dispose();
				printerCore = null;
			}
			if(Gauges != null) {
				Gauges.CollectionChanged -= OnGaugeCollectionChanged;
				Gauges.Dispose();
				gaugesCore = null;
			}
			if(colorSchemeCore != null)
				colorSchemeCore.PropertyChanged -= OnColorSchemeChanged;
			if(Root != null) {
				Root.Dispose();
				rootCore = null;
			}
		}
		void OnColorSchemeChanged(object sender, PropertyChangedEventArgs e) {
			UpdateChildren();
		}
		protected virtual void UpdateChildren() {
			if(Gauges != null) {
				for(int i = 0; i < Gauges.Count; i++)
					Gauges[i].ForceUpdateChildren();
			}
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
					using(new ComponentTransaction(this)) {
						BeginUpdate();
						PresetHelper.RestoreLayoutCore(new XmlXtraSerializer(), parameter);
						CheckElementsAffinity();
						CheckGaugesID();
						CheckViewStateTracking();
						CancelUpdate();
					}
				}
			}
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new CustomCallbackClientSideEvents();
		}
		int lockBuildCounter = 0;
		protected bool IsBuildingInProgress {
			get { return lockBuildCounter > 0; }
		}
		public virtual void BeginInit() {
			lockBuildCounter++;
		}
		public virtual void EndInit() {
			if(!string.IsNullOrEmpty(valueCore)) {
				SetValueCore(valueCore);
			}
			lockBuildCounter--;
		}
		string valueCore = null;
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlValue"),
#endif
 Category("Data"), Bindable(true), DefaultValue("")]
		public string Value {
			get {
				if(IsBuildingInProgress) {
					object value = ViewState["Value"];
					return (value != null) ? (string)value : string.Empty;
				}
				else return GetValueCore();
			}
			set {
				if(IsBuildingInProgress) {
					ViewState["Value"] = value;
				}
				else SetValueCore(value);
			}
		}
		[Obsolete("Use the Value property instead.")]
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DefaultValue {
			get { return Value; }
			set { Value = value; }
		}
		protected string GetValueCore() {
			object result = null;
			if(Gauges.Count > 0) {
				ICircularGauge cGauge = Gauges[0] as ICircularGauge;
				ILinearGauge lGauge = Gauges[0] as ILinearGauge;
				IDigitalGauge dGauge = Gauges[0] as IDigitalGauge;
				IStateIndicatorGauge sGauge = Gauges[0] as IStateIndicatorGauge;
				if(cGauge != null && cGauge.Scales.Count > 0) result = cGauge.Scales[0].Value;
				if(lGauge != null && lGauge.Scales.Count > 0) result = lGauge.Scales[0].Value;
				if(sGauge != null && sGauge.Indicators.Count > 0) result = sGauge.Indicators[0].StateIndex;
				if(dGauge != null) result = dGauge.Text;
			}
			return (result != null) ? result.ToString() : string.Empty;
		}
		protected void SetValueCore(string value) {
			if(Gauges.Count > 0) {
				ICircularGauge cGauge = Gauges[0] as ICircularGauge;
				ILinearGauge lGauge = Gauges[0] as ILinearGauge;
				IDigitalGauge dGauge = Gauges[0] as IDigitalGauge;
				IStateIndicatorGauge sGauge = Gauges[0] as IStateIndicatorGauge;
				if((cGauge != null) && (cGauge.Scales.Count > 0)) {
					float result;
					if(float.TryParse(value, out result)) cGauge.Scales[0].Value = result;
				}
				if((lGauge != null) && (lGauge.Scales.Count > 0)) {
					float result;
					if(float.TryParse(value, out result)) lGauge.Scales[0].Value = result;
				}
				if((sGauge != null) && (sGauge.Indicators.Count > 0)) {
					int result;
					if(int.TryParse(value, out result)) sGauge.Indicators[0].StateIndex = result;
				}
				if(dGauge != null) dGauge.Text = value;
			}
		}
		ColorScheme colorSchemeCore;
		[XtraSerializableProperty(XtraSerializationVisibility.Content), Category("Appearance"), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorScheme ColorScheme {
			get { return colorSchemeCore; }
		}
		[Category("Action")]
		public event CallbackEventHandlerBase CustomCallback {
			add { Events.AddHandler(customCallback, value); }
			remove { Events.RemoveHandler(customCallback, value); }
		}
		[Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(ASPxWebControl.EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(ASPxWebControl.EventCustomJsProperties, value); }
		}
		[Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlJSProperties"),
#endif
 Browsable(false), AutoFormatDisable]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlEnableClientSideAPI"),
#endif
 AutoFormatDisable, Category("Client-Side"), DefaultValue(false)]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlSaveStateOnCallbacks"),
#endif
 AutoFormatDisable, Category("Behavior"), DefaultValue(true)]
		public bool SaveStateOnCallbacks {
			get { return GetBoolProperty("SaveStateOnCallbacks", true); }
			set { SetBoolProperty("SaveStateOnCallbacks", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(SettingsLoadingPanel.DefaultDelay), AutoFormatDisable]
		public int LoadingPanelDelay {
			get { return SettingsLoadingPanel.Delay; }
			set { SettingsLoadingPanel.Delay = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(StringResources.LoadingPanelText), AutoFormatEnable, Localizable(true)]
		public string LoadingPanelText {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatEnable]
		public bool ShowLoadingPanelImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatDisable]
		public bool ShowLoadingPanel {
			get { return SettingsLoadingPanel.Enabled; }
			set { SettingsLoadingPanel.Enabled = value; }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlLoadingPanelImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty), Category("LoadingPanel")]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable,
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlLoadingPanelStyle"),
#endif
		Category("LoadingPanel"), PersistenceMode(PersistenceMode.InnerProperty)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		protected LoadingPanelStyle GetDefaultLoadingPanelStyle() {
			LoadingPanelStyle style = new LoadingPanelStyle();
			style.Font.Name = "Tahoma";
			style.Font.Size = 10;
			style.ForeColor = Color.Black;
			style.BackColor = Color.White;
			style.Border.BorderStyle = System.Web.UI.WebControls.BorderStyle.Solid;
			style.Border.BorderColor = Color.FromArgb(0x7B, 0x7B, 0x7B);
			style.Border.BorderWidth = 1;
			style.HorizontalAlign = HorizontalAlign.Center;
			style.Paddings.Padding = new Unit(10, UnitType.Pixel);
			style.ImageSpacing = new Unit(5, UnitType.Pixel);
			return style;
		}
		protected override LoadingPanelStyle GetLoadingPanelStyle() {
			LoadingPanelStyle result = GetDefaultLoadingPanelStyle();
			result.CopyFrom(base.LoadingPanelStyle);
			return result;
		}
		[Localizable(false), 
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false)]
		public CustomCallbackClientSideEvents ClientSideEvents {
			get { return base.ClientSideEventsInternal as CustomCallbackClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlBinaryStorageMode"),
#endif
		DefaultValue(BinaryStorageMode.Default), AutoFormatDisable]
		public BinaryStorageMode BinaryStorageMode {
			get { return (BinaryStorageMode)GetEnumProperty("BinaryStorageMode", BinaryStorageMode.Default); }
			set { SetEnumProperty("BinaryStorageMode", BinaryStorageMode.Default, value); }
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			OnCustomCallback(new CallbackEventArgsBase(eventArgument));
		}
		protected virtual void OnCustomCallback(CallbackEventArgsBase e) {
			CallbackEventHandlerBase handler = base.Events[customCallback] as CallbackEventHandlerBase;
			if(handler != null) handler(this, e);
		}
		protected override object GetCallbackResult() {
			EnsureChildControls();
			Hashtable renderResult = new Hashtable();
			if(Image == null) return string.Empty;
			BeginRendering();
			try {
				PrepareImage();
				renderResult[UrlKey] = Image.ImageUrl;
				renderResult[CallbackResultProperties.StateObject] = GetClientObjectState();
			}
			finally { EndRendering(); }
			return renderResult;
		}
		protected override bool IsScriptEnabled() {
			return true;
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(HasEvents() && HasCustomCallbackEventHandler) {
				eventNames.Add("CustomCallback");
			}
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			if(SaveStateOnCallbacks)
				result.Add(ClientStateProperties.CallbackState, SaveStateCore());
			return result;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientGaugeControl";
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return base.HasFunctionalityScripts() || HasCustomCallbackEventHandler;
		}
		protected override bool IsServerSideEventsAssigned() {
			return IsServerCustomCallbackAssigned();
		}
		protected bool IsServerCustomCallbackAssigned() {
			return HasEvents() && HasCustomCallbackEventHandler;
		}
		bool HasCustomCallbackEventHandler {
			get { return Events[customCallback] != null; }
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(DevExpress.Web.ASPxGauges.ASPxGaugeControl), ASPxGaugeControlScriptResourceName);
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
			return new WebPrintableProvider(Printer);
		}
		protected virtual ComponentPrinterBase CreateComponentPrinter() {
			return new ComponentPrinterDynamic(PrintableProvider);
		}
		protected ModelRoot Root {
			get { return rootCore; }
		}
		protected virtual LayoutManager CreateLayoutManager() {
			return new LayoutManager(this, true);
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlGauges"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public GaugeCollectionWeb Gauges {
			get { return gaugesCore; }
		}
		public IGauge AddGauge(GaugeType type) {
			IGauge gauge = null;
			switch(type) {
				case GaugeType.Circular: gauge = new CircularGauge(); break;
				case GaugeType.Linear: gauge = new LinearGauge(); break;
				case GaugeType.Digital: gauge = new DigitalGauge(); break;
				case GaugeType.StateIndicator: gauge = new StateIndicatorGauge(); break;
			}
			if(gauge != null) Gauges.Add(gauge);
			return gauge;
		}
		protected System.Web.UI.WebControls.Image Image {
			get { return imageCore; }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlAlternateText"),
#endif
		Bindable(true), Localizable(true), Category("Appearance"), DefaultValue("")]
		public string AlternateText {
			get {
				object value = ViewState["AlternateText"];
				return (value != null) ? (string)value : string.Empty;
			}
			set { ViewState["AlternateText"] = value; }
		}
		string ActualAlternateText {
			get {
				string alternateText = AlternateText;
				return string.IsNullOrEmpty(alternateText) ?
					DefaultAlternateTextBuilder.BuildTextForGaugeControl(this) : alternateText;
			}
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlDescriptionUrl"),
#endif
UrlProperty, Category("Accessibility"), Editor("System.Web.UI.Design.UrlEditor", typeof(UITypeEditor)), DefaultValue("")]
		public string DescriptionUrl {
			get {
				object value = this.ViewState["DescriptionUrl"];
				return (value != null) ? (string)value : string.Empty;
			}
			set { ViewState["DescriptionUrl"] = value; }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlImageType"),
#endif
DefaultValue(ImageType.Default)]
		public ImageType ImageType {
			get {
				object value = ViewState["ImageType"];
				return (value == null) ? ImageType.Default : (ImageType)value;
			}
			set { ViewState["ImageType"] = value; }
		}
		protected ImageFormat GetImageFormat() {
			if(DesignMode) return ImageFormat.Bmp;
			switch(ImageType) {
				case ImageType.Bmp: return ImageFormat.Bmp;
				case ImageType.Gif: return ImageFormat.Gif;
				case ImageType.Jpeg: return ImageFormat.Jpeg;
				default: return ImageFormat.Png;
			}
		}
		int lockUpdateCounter = 0;
		protected bool IsUpdateLocked {
			get { return lockUpdateCounter > 0; }
		}
		public void BeginUpdate() {
			lockUpdateCounter++;
		}
		public void EndUpdate() {
			if(--lockUpdateCounter == 0) {
				using(new ComponentTransaction(this)) { }
			}
		}
		public void CancelUpdate() {
			lockUpdateCounter--;
		}
		protected override bool HasLoadingPanel() {
			return !DesignMode;
		}
		protected override void CreateControlHierarchy() {
			this.imageCore = CreateGaugeImage();
			Controls.Add(Image);
			base.CreateControlHierarchy();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[0];
		}
		protected override void TrackViewState() {
			base.TrackViewState();
			(Gauges as IStateManagedHierarchyObject).TrackViewState();
		}
		protected override bool CanLoadPostDataOnLoad() {
			return false;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(SaveStateOnCallbacks)
				LoadStateCore(GetClientObjectStateValueString(ClientStateProperties.CallbackState));
			return false;
		}
		void ClearDTSurface() {
			IGauge[] gauges = Gauges.ToArray();
			for(int i = 0; i < gauges.Length; i++) {
				IGauge gauge = gauges[i];
				List<ISerizalizeableElement> elements = ((ISerizalizeableElement)gauge).GetChildren();
				foreach(ISerizalizeableElement e in elements) {
					((BaseGaugeWeb)gauge).RemoveComponentFromWebDesignTimeSurface(e as IComponent);
				}
				((BaseGaugeWeb)gauge).RemoveComponentFromWebDesignTimeSurface(gauge as IComponent);
			}
		}
		protected override void OnInit(EventArgs e) {
			BinaryStorage.ProcessRequest();
			base.OnInit(e);
		}
		protected override void PrepareControlHierarchy() {
			PrepareImage();
			base.PrepareControlHierarchy();
		}
		protected void PrepareImage() {
			RenderUtils.AssignAttributes(this, Image); 
			RenderUtils.SetVisibility(Image, IsClientVisible(), true);
			GetControlStyle().AssignToControl(Image);
			byte[] content = GetGaugeContainerContent(Width, Height);
			ImageProperties imgProps = CreateImageProperties(content, Width, Height);
			imgProps.Align = Image.ImageAlign;
			imgProps.AssignToControl(Image, DesignMode);
		}
		protected internal ImageProperties CreateImageProperties(byte[] content, Unit width, Unit height) {
			ImageProperties imgProps = new ImageProperties();
			imgProps.AlternateText = ActualAlternateText;
			imgProps.ToolTip = ToolTip;
			imgProps.DescriptionUrl = DescriptionUrl;
			imgProps.Width = width;
			imgProps.Height = height;
			imgProps.Url = BinaryStorage.GetImageUrl(this, content, BinaryStorageMode);
			return imgProps;
		}
		byte[] GetGaugeContainerContent(Unit width, Unit height) {
			byte[] content = new byte[0];
			int w = (int)width.Value; int h = (int)height.Value;
			using(System.Drawing.Image img = GetImageCore(w, h, new RectangleF(0, 0, w, h))) {
				using(MemoryStream ms = new MemoryStream()) {
					img.Save(ms, GetImageFormat());
					content = ms.ToArray();
				}
			}
			return content;
		}
		protected internal virtual string SaveStateCore() {
			object objState = (Gauges as IStateManagedHierarchyObject).SaveViewState();
			return StateConverterHelper.FromObjectState(objState);
		}
		protected internal virtual void LoadStateCore(string state) {
			if(string.IsNullOrEmpty(state)) return;
			object objState = StateConverterHelper.FromStateString(state);
			(Gauges as IStateManagedHierarchyObject).LoadViewState(objState);
			((IGaugeContainer)this).UpdateGaugesZOrder();
		}
		protected virtual System.Web.UI.WebControls.Image CreateGaugeImage() {
			return RenderUtils.CreateImage();
		}
		void OnGaugeCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<IGauge> ea) {
			switch(ea.ChangedType) {
				case ElementChangedType.ElementAdded: OnGaugeAdded(ea); break;
				case ElementChangedType.ElementRemoved: OnGaugeRemoved(ea); break;
			}
		}
		protected virtual void OnGaugeRemoved(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<IGauge> ea) {
			if(ea.Element == null) return;
			RemoveGaugeFromDTComponents(ea.Element as IComponent);
			((IGaugeContainer)this).RemovePrimitive(ea.Element.Model);
			ea.Element.SetContainer(null);
			((ILayoutManagerContainer)this).DoLayout();
			PresetHelper.OnRemoveItem(ea);
		}
		protected virtual void OnGaugeAdded(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<IGauge> ea) {
			if(ea.Element == null) return;
			AddGaugeToDTComponents(ea.Element as IComponent);
			ea.Element.SetContainer(this);
			((ILayoutManagerContainer)this).DoLayout();
			PresetHelper.OnAddItem(ea);
			((IGaugeContainer)this).UpdateGaugesZOrder();
		}
		void RemoveGaugeFromDTComponents(IComponent gauge) {
			if(!IsInEditMode(this)) return; 
			ISite site = Site ?? (gauge != null ? gauge.Site : null);
			if(CanRemove(site, gauge)) site.Container.Remove(gauge);
		}
		void AddGaugeToDTComponents(IComponent gauge) {
			if(!IsInEditMode(this)) return; 
			ISite site = Site ?? (gauge != null ? gauge.Site : null);
			if(CanAdd(site, gauge)) site.Container.Add(gauge);
		}
		bool CanRemove(ISite site, IComponent component) {
			if(site == null || site.Container == null || component == null) return false;
			ArrayList components = new ArrayList(site.Container.Components);
			foreach(IComponent c in components)
				if(c == component) return true;
			return false;
		}
		bool CanAdd(ISite site, IComponent component) {
			if(site == null || site.Container == null || component == null) return false;
			ArrayList components = new ArrayList(site.Container.Components);
			foreach(IComponent c in components)
				if(c == component) return false;
			return true;
		}
		protected void UpdateBeforePaint() {
			for(int i = 0; i < Gauges.Count; i++) Gauges[i].CheckEnabledState(Enabled);
		}
		protected System.Drawing.Image GetImageCore(int width, int height, RectangleF fragment, Color background) {
			Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			DrawToImage(result, height, width, fragment, background);
			return result;
		}
		protected System.Drawing.Image GetImageCore(int width, int height, RectangleF fragment) {
			Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			DrawToImage(result, height, width, fragment, BackColor);
			return result;
		}
		protected Metafile GetMetafileCore(Stream stream, int width, int height, RectangleF fragment) {
			Metafile metaFile = CreateMetafile(width, height, stream);
			DrawToImage(metaFile, width, height, fragment, BackColor);
			return metaFile;
		}
		static Metafile CreateMetafile(int width, int height, Stream stream) {
			Metafile metaFile;
			IntPtr hDC = NativeMethods.GetWindowDC(IntPtr.Zero);
			try {
				metaFile = new Metafile(stream, hDC, new RectangleF(0, 0, width, height), MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly);
			}
			finally { NativeMethods.ReleaseDC(IntPtr.Zero, hDC); }
			return metaFile;
		}
		[System.Security.SecuritySafeCritical]
		static class NativeMethods {
			public static IntPtr GetWindowDC(IntPtr hWnd) {
				return UnsafeNativeMethods.GetWindowDC(hWnd);
			}
			public static int ReleaseDC(IntPtr hWnd, IntPtr hDC) {
				return UnsafeNativeMethods.ReleaseDC(hWnd, hDC);
			}
			#region SecurityCritical
			static class UnsafeNativeMethods {
				[System.Runtime.InteropServices.DllImport("USER32.dll")]
				internal static extern IntPtr GetWindowDC(IntPtr hWnd);
				[System.Runtime.InteropServices.DllImport("USER32.dll")]
				internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
			}
			#endregion SecurityCritical
		}
		void DrawToImage(System.Drawing.Image image, int height, int width, RectangleF fragment, Color? background) {
			using(Bitmap tmp = new Bitmap((int)Width.Value, (int)Height.Value, PixelFormat.Format32bppArgb)) {
				MakeAnyElementsInvisibleCore(true);
				using(Graphics g = Graphics.FromImage(tmp)) {
					if(background.HasValue)
						g.Clear(background.Value);
					using(GraphicsCustomizer custimizer = new GraphicsCustomizer(g, GraphicsProperties)) {
						Root.Self.Render(g);
					}
				}
				using(Graphics g = Graphics.FromImage(image)) {
					g.Clear(BackColor);
					g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
					float s = Math.Max((float)width / fragment.Width, (float)height / fragment.Height);
					g.Transform = new System.Drawing.Drawing2D.Matrix(s, 0, 0, s, -fragment.Left * s, -fragment.Top * s);
					g.DrawImage(tmp, PointF.Empty);
					if(DesignMode && Gauges.Count == 0)
						DrawEmpty(g, width, height);
				}
				MakeAnyElementsInvisibleCore(false);
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
		void DrawEmpty(Graphics graphics, int width, int height) {
			RectangleF r = new RectangleF(1, 1, width - 2, height - 2);
			graphics.DrawRectangle(Pens.Gray, r.Left, r.Top, r.Width, r.Height);
			graphics.DrawString(EmptyContainerText, ContainerFont, Brushes.Black, r, ContainerStringFormat);
		}
		void MakeAnyElementsInvisibleCore(bool value) {
			for(int i = 0; i < Gauges.Count; i++) Gauges[i].SuppressDrawBorder = value;
		}
		protected PresetHelper PresetHelper {
			get { return presetHelperCore; }
		}
		#region ILayoutManagerContainer
		void ILayoutManagerContainer.DoLayout() {
			if(AutoLayout)
				CreateLayoutManager().Layout();
		}
		List<ILayoutManagerClient> ILayoutManagerContainer.Clients {
			get {
				List<ILayoutManagerClient> list = new List<ILayoutManagerClient>();
				foreach(IGauge gauge in Gauges) {
					ILayoutManagerClient client = gauge as ILayoutManagerClient;
					if(client != null) list.Add(client);
				}
				return list;
			}
		}
		Rectangle ILayoutManagerContainer.Bounds {
			get { return new Rectangle(0, 0, (int)Width.Value, (int)Height.Value); }
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlLayoutPadding"),
#endif
 XtraSerializableProperty, Category("Layout")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		public ThicknessWeb LayoutPadding {
			get { return ((ILayoutManagerContainer)this).LayoutPadding as ThicknessWeb; }
			set { ((ILayoutManagerContainer)this).LayoutPadding = value; }
		}
		IThickness ILayoutManagerContainer.LayoutPadding {
			get {
				int[] value = ViewState["LayoutPadding"] as int[];
				if(value != null) {
					if(value.Length == 1)
						return new ThicknessWeb(value[0]);
					return new ThicknessWeb(value[0], value[1], value[2], value[3]);
				}
				return new ThicknessWeb(0);
			}
			set {
				int[] valueArray;
				if(value.All != 0)
					valueArray = new int[] { value.All };
				valueArray = new int[] { value.Left, value.Top, value.Right, value.Bottom }; ;
				ViewState["LayoutPadding"] = valueArray;
				((ILayoutManagerContainer)this).DoLayout();
			}
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlLayoutInterval"),
#endif
 XtraSerializableProperty, Category("Layout"), DefaultValue(0)]
		public int LayoutInterval {
			get {
				object value = ViewState["LayoutInterval"];
				return (value == null) ? 0 : (int)value;
			}
			set {
				ViewState["LayoutInterval"] = value;
				((ILayoutManagerContainer)this).DoLayout();
			}
		}
		internal bool ShouldSerializeLayoutPadding() {
			return ((ILayoutManagerContainer)this).LayoutPadding.All != 0;
		}
		internal void ResetLayoutPadding() {
			ViewState["LayoutPadding"] = null;
			((ILayoutManagerContainer)this).DoLayout();
		}
		#endregion
		#region IGaugeContainer Members
		string IGaugeContainer.Name { get { return ID; } }
		GaugeCollection IGaugeContainer.Gauges {
			get { return Gauges as GaugeCollection; }
		}
		void IGaugeContainer.AddPrimitive(IElement<IRenderableElement> primitive) {
			Root.Composite.Add(primitive);
		}
		void IGaugeContainer.RemovePrimitive(IElement<IRenderableElement> primitive) {
			Root.Composite.Remove(primitive);
		}
		[
#if !SL
	DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlAutoLayout"),
#endif
DefaultValue(true), Category("Layout")]
		[XtraSerializableProperty]
		public bool AutoLayout {
			get {
				object value = ViewState["AutoLayout"];
				return (value == null) ? true : (bool)value;
			}
			set {
				ViewState["AutoLayout"] = value;
				if(AutoLayout) ((ILayoutManagerContainer)this).DoLayout();
				(this as IGaugeContainer).UpdateRect(Rectangle.Empty);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty,
		]
		public Size SizeEx {
			get { return new Size((int)Width.Value, (int)Height.Value); }
			set {
				if(SizeEx == value) return;
				Width = new Unit(value.Width);
				Height = new Unit(value.Height);
			}
		}
		Rectangle IGaugeContainer.Bounds {
			get { return new Rectangle(0, 0, (int)Width.Value, (int)Height.Value); }
		}
		BasePrimitiveHitInfo IGaugeContainer.CalcHitInfo(Point p) {
			return Root.CalcHitInfo(p);
		}
		void IGaugeContainer.ComponentChanged(IComponent component, string property, object oldValue, object newValue) {
		}
		void IGaugeContainer.ComponentChanging(IComponent component, string property) {
		}
		bool forceClearOnRestoreCore = false;
		bool IGaugeContainer.ForceClearOnRestore {
			get { return forceClearOnRestoreCore; }
			set { forceClearOnRestoreCore = value; }
		}
		void IGaugeContainer.DesignerLoaded() {
			foreach(IGauge gauge in Gauges) AddGaugeToDTComponents(gauge);
		}
		void IGaugeContainer.UpdateGaugesZOrder() {
			int i = Gauges.Count;
			foreach(IGauge gauge in Gauges) {
				gauge.Model.ZOrder = --i + 60;
			}
		}
		CustomizeManager IGaugeContainer.CustomizeManager {
			get { return null; }
		}
		bool IGaugeContainer.EnableCustomizationMode {
			get { return false; }
			set { }
		}
		bool IGaugeContainer.Enabled {
			get { return Enabled; }
			set { Enabled = value; }
		}
		System.Drawing.Image IGaugeContainer.GetImage(int width, int height) {
			return GetImageCore(width, height, new RectangleF(0, 0, width, height));
		}
		System.Drawing.Image IGaugeContainerEx.GetImage(int width, int height, Color background) {
			return GetImageCore(width, height, new RectangleF(0, 0, width, height), background);
		}
		Metafile IGaugeContainerEx.GetMetafile(Stream stream, int width, int height) {
			return GetMetafileCore(stream, width, height, new RectangleF(0, 0, width, height));
		}
		IPrintable IGaugeContainerEx.Printable {
			get { return PrintableProvider; }
		}
		void IGaugeContainer.InvalidateRect(RectangleF bounds) {
			if(IsUpdateLocked) return;
			using(new ComponentTransaction(this)) { }
		}
		void IGaugeContainer.UpdateRect(RectangleF bounds) {
			if(IsUpdateLocked) return;
			using(new ComponentTransaction(this)) { }
		}
		void IGaugeContainer.OnModelChanged(BaseGaugeModel oldModel, BaseGaugeModel newModel) {
			(this as IGaugeContainer).AddPrimitive(newModel);
		}
		event EventHandler IGaugeContainer.ModelChanged {
			add { }
			remove { }
		}
		CustomizationFrameBase[] IGaugeContainer.OnCreateCustomizeFrames(IGauge gauge, CustomizationFrameBase[] frames) {
			return frames;
		}
		void IGaugeContainer.SetCursor(CursorInfo cursorInfo) {
		}
		#endregion
		#region IXtraSerializable Members
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((IXtraSerializable)PresetHelper).OnEndDeserializing(restoredVersion);
		}
		void IXtraSerializable.OnEndSerializing() {
			((IXtraSerializable)PresetHelper).OnEndSerializing();
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			((IXtraSerializable)PresetHelper).OnStartDeserializing(e);
		}
		void IXtraSerializable.OnStartSerializing() {
			((IXtraSerializable)PresetHelper).OnStartSerializing();
		}
		public void SaveStyleToXml(string path) {
			CheckGaugesID();
			PresetHelper.SaveStyle(path);
		}
		public void RestoreStyleFromXml(string path) {
			using(new ComponentTransaction(this)) {
				BeginUpdate();
				PresetHelper.RestoreStyle(path);
				CancelUpdate();
			}
		}
		public void SaveStyleToStream(Stream stream) {
			CheckGaugesID();
			PresetHelper.SaveStyle(stream);
		}
		public void RestoreStyleFromStream(Stream stream) {
			using(new ComponentTransaction(this)) {
				BeginUpdate();
				PresetHelper.RestoreStyle(stream);
				CancelUpdate();
			}
		}
		public virtual void SaveLayoutToStream(Stream stream) {
			CheckGaugesID();
			PresetHelper.SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void RestoreLayoutFromStream(Stream stream) {
			using(new ComponentTransaction(this)) {
				BeginUpdate();
				PresetHelper.RestoreLayoutCore(new XmlXtraSerializer(), stream);
				CheckElementsAffinity();
				CheckGaugesID();
				CheckViewStateTracking();
				CancelUpdate();
			}
		}
		public virtual void SaveLayoutToXml(string xmlFile) {
			CheckGaugesID();
			PresetHelper.SaveLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			using(new ComponentTransaction(this)) {
				BeginUpdate();
				PresetHelper.RestoreLayoutCore(new XmlXtraSerializer(), xmlFile);
				CheckElementsAffinity();
				CheckGaugesID();
				CheckViewStateTracking();
				CancelUpdate();
			}
		}
		public virtual void SaveLayoutToRegistry(string path) {
			CheckGaugesID();
			PresetHelper.SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			using(new ComponentTransaction(this)) {
				BeginUpdate();
				PresetHelper.RestoreLayoutCore(new RegistryXtraSerializer(), path);
				CheckElementsAffinity();
				CheckGaugesID();
				CheckViewStateTracking();
				CancelUpdate();
			}
		}
		object XtraFindItemsItem(XtraItemEventArgs e) {
			return PresetHelper.XtraFindItemsItem(e);
		}
		void IGaugeContainer.AddToParentCollection(ISerizalizeableElement element, ISerizalizeableElement parent) {
			string originalName = element.Name;
			BaseGaugeWeb gb = parent as BaseGaugeWeb;
			if(gb != null) {
				switch(element.ParentCollectionName) {
					case "Labels": gb.Labels.Add((LabelComponent)element); break;
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
				case "LinearGauge":
					result = new LinearGauge();
					break;
				case "LinearScaleBackgroundLayerComponent":
					result = new LinearScaleBackgroundLayerComponent();
					break;
				case "LinearScaleComponent":
					result = new LinearScaleComponent();
					break;
				case "LinearScaleLevelComponent":
					result = new LinearScaleLevelComponent();
					break;
				case "LinearScaleMarkerComponent":
					result = new LinearScaleMarkerComponent();
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
		[XtraSerializableProperty(false, true, false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public List<ISerizalizeableElement> Items {
			get { return PresetHelper.Items; }
			set { PresetHelper.Items = value; }
		}
		protected internal void CheckElementsAffinity() {
			foreach(BaseGaugeWeb gauge in Gauges) {
				gauge.CheckElementsAffinity();
			}
		}
		protected internal void CheckGaugesID() {
			foreach(BaseGaugeWeb gauge in Gauges) {
				List<string> names = UniqueNameHelper.GetSiteNames(Site);
				if(string.IsNullOrEmpty(gauge.Name)) gauge.Name = UniqueNameHelper.GetUniqueName("Gauge", names, 0);
			}
		}
		protected internal void CheckViewStateTracking() {
			if(IsTrackingViewState)
				((IStateManagedHierarchyObject)Gauges).TrackViewState();
		}
		#endregion
		#region HiddenProperties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssClass { get { return base.CssClass; } set { base.CssClass = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssFilePath { get { return base.CssFilePath; } set { base.CssFilePath = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle { get { return base.DisabledStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BackgroundImage BackgroundImage { get { return base.BackgroundImage; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Cursor { get { return base.Cursor; } set { base.Cursor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string SkinID { get { return base.SkinID; } set { base.SkinID = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableTheming { get { return base.EnableTheming; } set { base.EnableTheming = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override short TabIndex { get { return base.TabIndex; } set { base.TabIndex = value; } }
		#endregion HiddenProperties
#if !SL
	[DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlWidth")]
#endif
		public override Unit Width {
			get { return base.Width; }
			set {
				if(value.Type != UnitType.Pixel) return;
				base.Width = new Unit(Math.Max(1, value.Value));
			}
		}
		GraphicsProperties graphicsPropertiesCore;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		public GraphicsProperties GraphicsProperties {
			get { return graphicsPropertiesCore; }
		}
		public bool ShouldSerializeGraphicsProperties() {
			return GraphicsProperties.ShouldSerialize();
		}
		public void ResetGraphicsProperties() {
			GraphicsProperties.Reset();
		}
#if !SL
	[DevExpressWebASPxGaugesLocalizedDescription("ASPxGaugeControlHeight")]
#endif
		public override Unit Height {
			get { return base.Height; }
			set {
				if(value.Type != UnitType.Pixel) return;
				base.Height = new Unit(Math.Max(1, value.Value));
			}
		}
		#region Export
		static void CheckSize(Size size) {
			if(size.Width <= 0 || size.Height <= 0)
				throw new ArgumentException("size");
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
			Size size = new Size((int)Width.Value, (int)Height.Value);
			IGaugeContainerEx gContainer = this as IGaugeContainerEx;
			if(format == ImageFormat.Wmf || format == ImageFormat.Emf) {
				using(gContainer.GetMetafile(stream, size.Width, size.Height)) { }
			}
			else {
				using(System.Drawing.Image img = gContainer.GetImage(size.Width, size.Height, BackColor)) {
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
			get { return ComponentPrinterDynamic.IsPrintingAvailable(false); }
		}
		void SetPrinterSizeMode(PrintSizeMode sizeMode) {
			if(Printer != null) Printer.SizeMode = sizeMode;
		}
		public void Print(PrintSizeMode sizeMode) {
			SetPrinterSizeMode(sizeMode);
			ComponentPrinter.Print();
		}
		public void Print() {
			SetPrinterSizeMode(PrintSizeMode.None);
			ComponentPrinter.Print();
		}
		#endregion
		protected override void DataBindInternal() {
			base.DataBindInternal();
			foreach(IGauge gauge in Gauges) {
				ICircularGauge cGauge = gauge as ICircularGauge;
				if(cGauge != null) {
					for(int i = 0; i < cGauge.Scales.Count; i++) {
						cGauge.Scales[i].DataBind();
					}
					continue;
				}
				ILinearGauge lGauge = gauge as ILinearGauge;
				if(lGauge != null) {
					for(int i = 0; i < lGauge.Scales.Count; i++) {
						lGauge.Scales[i].DataBind();
					}
					continue;
				}
				IStateIndicatorGauge sGauge = gauge as IStateIndicatorGauge;
				if(sGauge != null) {
					for(int i = 0; i < sGauge.Indicators.Count; i++) {
						sGauge.Indicators[i].DataBind();
					}
					continue;
				}
				DigitalGauge dGauge = gauge as DigitalGauge;
				if(dGauge != null) dGauge.DataBind();
			}
		}
		protected internal class ComponentsHost : IDisposable {
			System.ComponentModel.Design.IDesignerHost host;
			IGaugeContainer gaugeContainer;
			static IDictionary<IGaugeContainer, ComponentsHost> hosts = new Dictionary<IGaugeContainer, ComponentsHost>();
			public ComponentsHost(ITypeDescriptorContext context, IServiceProvider provider, object value) {
				host = GetDesignerHost(provider);
				gaugeContainer = context.Instance as IGaugeContainer;
				if(host != null && host.Container != null)
					AddComponentsToHost(gaugeContainer, host);
			}
			public void Dispose() {
				if(host != null && host.Container != null)
					RemoveComponentsFromHost(gaugeContainer, host);
			}
			void AddComponentsToHost(IGaugeContainer gaugeContainer, System.ComponentModel.Design.IDesignerHost host) {
				foreach(object element in gaugeContainer.Gauges) {
					IComponent component = element as IComponent;
					if(component != null)
						host.Container.Add(component);
				}
			}
			void RemoveComponentsFromHost(IGaugeContainer gaugeContainer, System.ComponentModel.Design.IDesignerHost host) {
				foreach(object element in gaugeContainer.Gauges) {
					IComponent component = element as IComponent;
					if(component != null)
						host.Container.Remove(component);
				}
			}
			System.ComponentModel.Design.IDesignerHost GetDesignerHost(IServiceProvider provider) {
				return (System.ComponentModel.Design.IDesignerHost)provider.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			}
			public static bool GetIsInComponentsHost(IGaugeContainer gaugeContainer) {
				return (gaugeContainer != null) && hosts.ContainsKey(gaugeContainer);
			}
		}
		protected internal static bool IsInEditMode(IGaugeContainer gaugeContainer) {
			return ComponentsHost.GetIsInComponentsHost(gaugeContainer);
		}
	}
	public static class DefaultAlternateTextBuilder {
		static string BuildTextForGauges(GaugeCollection gauges) {
			return string.Format(" contains {0} gauge{1} with values: ",
					gauges.Count, gauges.Count > 1 ? "s" : ""
				);
		}
		static string BuildTextForGauge(IGauge gauge) {
			string values = string.Empty;
			ICircularGauge cGauge = gauge as ICircularGauge;
			if(cGauge != null)
				values = BuildCircularGaugeText(cGauge);
			ILinearGauge lGauge = gauge as ILinearGauge;
			if(lGauge != null)
				values = BuildLinearGaugeText(lGauge);
			IStateIndicatorGauge iGauge = gauge as IStateIndicatorGauge;
			if(iGauge != null)
				values = BuildStateIndicatorGaugeText(iGauge);
			IDigitalGauge dGauge = gauge as IDigitalGauge;
			if(dGauge != null)
				values = BuildDigitalGaugeText(dGauge);
			return string.Format("({0})", values);
		}
		static string BuildTextForScale<Scale>(Scale scale) where Scale : IScale, INamed {
			return string.Format("{0} - value {1}", scale.Name, scale.Value);
		}
		static string BuildTextForState<Indicator>(Indicator indicator) where Indicator : IStateIndicator, INamed {
			return string.Format("{0} - index {1}", indicator.Name, indicator.StateIndex);
		}
		static string BuildCircularGaugeText(ICircularGauge cGauge) {
			string[] values = new string[cGauge.Scales.Count];
			for(int i = 0; i < cGauge.Scales.Count; i++) {
				values[i] = BuildTextForScale(cGauge.Scales[i]);
			}
			return string.Join(", ", values);
		}
		static string BuildLinearGaugeText(ILinearGauge lGauge) {
			string[] values = new string[lGauge.Scales.Count];
			for(int i = 0; i < lGauge.Scales.Count; i++) {
				values[i] = BuildTextForScale(lGauge.Scales[i]);
			}
			return string.Join(", ", values);
		}
		static string BuildStateIndicatorGaugeText(IStateIndicatorGauge iGauge) {
			string[] values = new string[iGauge.Indicators.Count];
			for(int i = 0; i < iGauge.Indicators.Count; i++) {
				values[i] = BuildTextForState(iGauge.Indicators[i]);
			}
			return string.Join(", ", values);
		}
		static string BuildDigitalGaugeText(IDigitalGauge dGauge) {
			return dGauge.Text;
		}
		public static string BuildTextForGaugeControl(ASPxGaugeControl gaugeControl) {
			if(gaugeControl == null) return string.Empty;
			StringBuilder builder = new StringBuilder();
			builder.Append("The GaugeControl");
			if(gaugeControl.Gauges.Count > 0) {
				builder.Append(BuildTextForGauges(gaugeControl.Gauges));
				for(int i = 0; i < gaugeControl.Gauges.Count; i++) {
					if(i > 0) builder.Append(", ");
					builder.Append(BuildTextForGauge(gaugeControl.Gauges[i]));
				}
			}
			builder.Append(".");
			return builder.ToString();
		}
	}
}
