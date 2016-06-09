#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardExport;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Layout;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Presets;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Printing;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Win.Gauges.Circular;
using DevExpress.XtraGauges.Win.Gauges.Digital;
using DevExpress.XtraGauges.Win.Gauges.Linear;
using DevExpress.XtraGauges.Win.Gauges.State;
using DevExpress.XtraGauges.Win.Printing;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
namespace DevExpress.DashboardCommon.Viewer {
	public class GaugeContainer : IDisposable, IGaugeContainerEx, ILayoutManagerContainer, IXtraSerializable {
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2141")]
		static Metafile CreateMetafileInstance(Rectangle bounds, MetafileFrameUnit units, EmfType emfType) {
			Metafile metaFile;
			using(Graphics gr = Graphics.FromHwnd(IntPtr.Zero)) {
				IntPtr hdc = gr.GetHdc();
				try {
					metaFile = new Metafile(hdc, bounds, units, emfType);
				} finally {
					gr.ReleaseHdc(hdc);
				}
			}
			return metaFile;
		}
		readonly DevExpress.XtraGauges.Base.GaugeCollection gauges = new DevExpress.XtraGauges.Base.GaugeCollection();
		readonly ModelRoot root = new ModelRoot();
		readonly PresetHelper presetHelper;
		Size size;
		GaugePrinter printer;
		IPrintable printable;
		bool autoLayout = true;
		bool isDisposed;
		GaugePrinter Printer { get { return printer ?? (printer = new GaugePrinter(this)); } }
		bool IGaugeContainer.DesignMode { get { return false; } }
		bool IGaugeContainer.Enabled { get { return true; } set { } }
		string IGaugeContainer.Name { get { return null; } }
		CustomizeManager IGaugeContainer.CustomizeManager { get { return null; } }
		bool IGaugeContainer.ForceClearOnRestore { get { return false; } set { } }
		bool IGaugeContainer.EnableCustomizationMode { get { return false; } set { } }
		IThickness ILayoutManagerContainer.LayoutPadding { get { return new Thickness(0); } set { } }
		int ILayoutManagerContainer.LayoutInterval { get { return 0; } set { } }
		List<ILayoutManagerClient> ILayoutManagerContainer.Clients {
			get {
				List<ILayoutManagerClient> clients = new List<ILayoutManagerClient>();
				foreach(BaseGaugeWin gauge in Gauges) {
					ILayoutManagerClient client = gauge as ILayoutManagerClient;
					if(client != null) 
						clients.Add(client);
				}
				return clients;
			}
		}
		Rectangle ILayoutManagerContainer.Bounds { get { return ((IGaugeContainer)this).Bounds; } }
		[XtraSerializableProperty(false, true, false)]
		public List<ISerizalizeableElement> Items { get { return presetHelper.Items; } set { presetHelper.Items = value; } }
		public Rectangle Bounds { get { return new Rectangle(new Point(0, 0), size); } }
		public IPrintable Printable { get { return printable ?? (printable = new WinPrintableProvider(Printer)); } }
		public DevExpress.XtraGauges.Base.GaugeCollection Gauges { get { return gauges; } }
		public bool AutoLayout {
			get { return autoLayout; }
			set {
				if(autoLayout != value) {
					autoLayout = value;
					if(AutoLayout)
						((ILayoutManagerContainer)this).DoLayout();
				}
			}
		}
		public Size Size { get { return size; } set { size = value; } }
		event EventHandler IGaugeContainer.ModelChanged { add { } remove { } }
		public GaugeContainer() {
			presetHelper = new PresetHelper(this);
			gauges.CollectionChanged += e => {
				switch(e.ChangedType) {
					case ElementChangedType.ElementAdded: 
						if(e.Element != null)
							e.Element.SetContainer(this);
						((ILayoutManagerContainer)this).DoLayout();
						((IGaugeContainer)this).UpdateGaugesZOrder();
						break;
					case ElementChangedType.ElementRemoved:
						if(e.Element != null) {
							((IGaugeContainer)this).RemovePrimitive(e.Element.Model);
							e.Element.SetContainer(null);
						}
						((ILayoutManagerContainer)this).DoLayout();
						break;
				}
			};
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				for(int i = 0; i < gauges.Count; i++)
					gauges[i].Dispose();
				if(printer != null) {
					printer.Dispose();
					printer = null;
				}
			}
			isDisposed = true;
		}
		LayoutManager CreateLayoutManager() {
			return new LayoutManager(this, true);
		}
		void IGaugeContainer.InitializeDefault(object parameter) {
		}
		CustomizationFrameBase[] IGaugeContainer.OnCreateCustomizeFrames(IGauge gauge, CustomizationFrameBase[] frames) {
			return frames;
		}
		void IGaugeContainer.DesignerLoaded() {
		}
		BasePrimitiveHitInfo IGaugeContainer.CalcHitInfo(Point p) {
			return BasePrimitiveHitInfo.Empty;
		}
		void IGaugeContainer.OnModelChanged(BaseGaugeModel oldModel, BaseGaugeModel model) {
			((IGaugeContainer)this).AddPrimitive(model);
		}
		void IGaugeContainer.AddPrimitive(IElement<IRenderableElement> primitve) {
			root.Composite.Add(primitve);
		}
		void IGaugeContainer.RemovePrimitive(IElement<IRenderableElement> primitve) {
			root.Composite.Remove(primitve);
		}
		void IGaugeContainer.UpdateGaugesZOrder() {
		}
		void IGaugeContainer.ComponentChanging(IComponent component, string property) {
		}
		void IGaugeContainer.ComponentChanged(IComponent component, string property, object oldValue, object newValue) {
		}
		void IGaugeContainer.SetCursor(CursorInfo ci) {
		}
		void IGaugeContainer.InvalidateRect(RectangleF bounds) {
		}
		void IGaugeContainer.UpdateRect(RectangleF rect) {
		}
		void IGaugeContainer.RestoreLayoutFromStream(Stream stream) {
		}
		void IGaugeContainer.RestoreLayoutFromXml(string path) {
		}
		void IGaugeContainer.SaveLayoutToStream(Stream stream) {
		}
		void IGaugeContainer.SaveLayoutToXml(string path) {
		}
		void IGaugeContainer.AddToParentCollection(ISerizalizeableElement element, ISerizalizeableElement parent) {
			string originalName = element.Name;
			BaseGaugeWin gb = parent as BaseGaugeWin;
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
		Metafile IGaugeContainerEx.GetMetafile(Stream stream, int width, int height) {
			return null;
		}
		Image IGaugeContainerEx.GetImage(int width, int height, Color background) {
			return DrawToImage(width, height, background);
		}
		XtraGauges.Core.Base.ColorScheme IGaugeContainer.ColorScheme {
			get { return null; }
		}
		void DrawContent(Image image, int width, int height, Color? background) {
			MakeAnyElementsInvisibleCore(true);
			try {
				float sx = (float)width / size.Width;
				float sy = (float)height / size.Height;
				using(Graphics gr = Graphics.FromImage(image)) {
					if(background.HasValue)
						gr.Clear(background.Value);
					Matrix m = gr.Transform.Clone();
					Matrix oldM = root.Self.Transform;
					m.Scale(sx, sy);
					bool matrixEquals = object.Equals(m, oldM);
					if(!matrixEquals)
						SetRootTransform(m);
					gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
					gr.SmoothingMode = SmoothingMode.HighQuality;
					gr.CompositingQuality = CompositingQuality.HighQuality;
					if(!matrixEquals)
						root.Self.Transform = m;
					root.Self.Render(gr);
					if(!matrixEquals)
						SetRootTransform(oldM);
				}
			} finally {
				MakeAnyElementsInvisibleCore(false);
			}
		}
		Image DrawToImage(int width, int height, Color? background) {
			Image image;
			if(ExportHelper.SupportMetafileImageFormat)
				image = CreateMetafileInstance(new Rectangle(Point.Empty, new Size(width, height)), MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly);
			else
				image = new Bitmap(width, height);
			DrawContent(image, width, height, background);
			return image;
		}
		void SetRootTransform(Matrix m) {
			foreach(IGauge gauge in gauges)
				gauge.BeginUpdate();
			try {
				root.Self.Transform = m;
			}
			finally {
				foreach(IGauge gauge in gauges)
					gauge.EndUpdate();
			}
		}
		void MakeAnyElementsInvisibleCore(bool value) {
			foreach(IGauge gauge in gauges)
				gauge.SuppressDrawBorder = value;
			foreach(IRenderableElement e in root.Composite.Elements)
				if(e is CustomizationFrameBase || e is HotTrackFrame)
					e.Accept(frameElement => frameElement.Self.Renderable = !value);
		}
		void ILayoutManagerContainer.DoLayout() {
			if(autoLayout && !isDisposed)
				CreateLayoutManager().Layout();
		}
#if DEBUG
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811")]
#endif
		object XtraFindItemsItem(XtraItemEventArgs e) {
			return presetHelper.XtraFindItemsItem(e);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((IXtraSerializable)presetHelper).OnEndDeserializing(restoredVersion);
		}
		void IXtraSerializable.OnEndSerializing() {
			((IXtraSerializable)presetHelper).OnEndSerializing();
		}
		void IXtraSerializable.OnStartDeserializing(Utils.LayoutAllowEventArgs e) {
			((IXtraSerializable)presetHelper).OnStartDeserializing(e);
		}
		void IXtraSerializable.OnStartSerializing() {
			((IXtraSerializable)presetHelper).OnStartSerializing();
		}
		public IGauge AddGauge(GaugeType type) {
			IGauge gauge = null;
			switch(type) {
				case GaugeType.Circular: 
					gauge = new CircularGauge(); 
					break;
				case GaugeType.Linear: 
					gauge = new LinearGauge(); 
					break;
				case GaugeType.Digital:
					gauge = new DigitalGauge();
					using(GaugeSettings settings = GaugeSettings.FromGauge(gauge)) {
						((DigitalGauge)gauge).AppearanceOn.ContentBrush = new SolidBrushObject(Color.Black);
						settings.TextSettings.Text = "00.000";
					}
					break;
				case GaugeType.StateIndicator: 
					gauge = new StateIndicatorGauge(); 
					break;
			}
			if(gauge != null) {
				gauge.Name = UniqueNameHelper.GetUniqueName("Gauge", CollectionHelper.GetNames(gauges), 0);
				gauges.Add(gauge);
			}
			return gauge;
		}
		public DigitalGauge AddDigitalGauge() {
			return (DigitalGauge)AddGauge(GaugeType.Digital);
		}
		public LinearGauge AddLinearGauge() {
			return (LinearGauge)AddGauge(GaugeType.Linear);
		}
		public CircularGauge AddCircularGauge() {
			return (CircularGauge)AddGauge(GaugeType.Circular);
		}
		public StateIndicatorGauge AddStateIndicatorGauge() {
			return (StateIndicatorGauge)AddGauge(GaugeType.StateIndicator);
		}
		public Image GetImage(int width, int height) {
			return DrawToImage(width, height, null);
		}
		public Image GetImage() {
			return DrawToImage(size.Width, size.Height, null);
		}
	}
}
