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
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Layout;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Win.Gauges.Circular;
using DevExpress.XtraGauges.Win.Gauges.Digital;
using DevExpress.XtraGauges.Win.Gauges.Linear;
using DevExpress.XtraGauges.Win.Gauges.State;
namespace DevExpress.XtraGauges.Win.Base {
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.GaugeDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public abstract class BaseGaugeWin : BaseGauge, ISupportInitialize, ILayoutManagerClient {
		BaseGaugeProviderWin gaugeProviderCore;
		OptionsToolTip optionsToolTipCore = null;
		public BaseGaugeWin()
			: base() {
		}
		public BaseGaugeWin(IGaugeContainer container)
			: base(container) {
		}
		protected override void OnCreate() {
			this.gaugeProviderCore = CreateGaugeProvider();
			this.optionsToolTipCore = CreateOptionsToolTip();
		}
		protected virtual OptionsToolTip CreateOptionsToolTip() {
			return new OptionsToolTip();
		}
		protected override void OnDispose() {
			if(GaugeProvider != null) {
				GaugeProvider.Dispose();
				this.gaugeProviderCore = null;
			}
			optionsToolTipCore = null;
		}
		void ResetOptionsToolTip() { OptionsToolTip.Reset(); }
		bool ShouldSerializeOptionsToolTip() { return OptionsToolTip.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("BaseGaugeWinOptionsToolTip"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), Category("ToolTip")]
		public virtual OptionsToolTip OptionsToolTip {
			get { return optionsToolTipCore; }
			set { optionsToolTipCore = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("BaseGaugeWinName"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[XtraSerializableProperty]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected override void SetBoundsCore(Rectangle value) {
			using(new NotificationHelper(GaugeContainer, this, "Bounds", Bounds, value)) {
				base.SetBoundsCore(value);
			}
		}
		protected override void SetProportionalStretchCore(bool value) {
			using(new NotificationHelper(GaugeContainer, this, "ProportionalStretch", ProportionalStretch, value)) {
				base.SetProportionalStretchCore(value);
			}
		}
		protected BaseGaugeProviderWin GaugeProvider {
			get { return gaugeProviderCore; }
		}
		protected abstract BaseGaugeProviderWin CreateGaugeProvider();
		protected override void ClearCore() {
			Labels.Clear();
			Images.Clear();
		}
		[
	DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageIndicatorComponentCollection Images {
			get { return GaugeProvider.Images; }
		}
		public ImageIndicatorComponent AddImageIndicator() {
			BeginUpdate();
			string[] names = new string[Images.Count];
			int i = 0;
			Images.Accept(
					delegate(ImageIndicatorComponent l) { names[i++] = l.Name; }
				);
			ImageIndicatorComponent imageIndicator = new ImageIndicatorComponent(UniqueNameHelper.GetUniqueName(Prefix("ImageIndicator"), names, Images.Count + 1));
			InitializeImageIndicatorDefault(imageIndicator);
			Images.Add(imageIndicator);
			AddComponentToDesignTimeSurface(imageIndicator);
			EndUpdate();
			return imageIndicator;
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("BaseGaugeWinLabels"),
#endif
	DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LabelComponentCollection Labels {
			get { return GaugeProvider.Labels; }
		}
		public LabelComponent AddLabel() {
			BeginUpdate();
			string[] names = new string[Labels.Count];
			int i = 0;
			Labels.Accept(
					delegate(LabelComponent l) { names[i++] = l.Name; }
				);
			LabelComponent label = new LabelComponent(UniqueNameHelper.GetUniqueName(Prefix("Label"), names, Labels.Count + 1));
			InitializeLabelDefault(label);
			Labels.Add(label);
			AddComponentToDesignTimeSurface(label);
			EndUpdate();
			return label;
		}
		protected virtual void InitializeLabelDefault(DevExpress.XtraGauges.Core.Model.Label label) {
			BaseGaugeExtension.InitializeLabelDefault(label);
		}
		protected virtual void InitializeImageIndicatorDefault(DevExpress.XtraGauges.Core.Model.ImageIndicator imageIndicator) {
			BaseGaugeExtension.InitializeImageIndicatorDefault(imageIndicator);
		}
		protected override List<string> GetNamesCore() {
			List<string> names = new List<string>();
			ComponentCollectionExtention.CollectNames(Labels, names);
			ComponentCollectionExtention.CollectNames(Images, names);
			return names;
		}
		protected abstract string GetPrefixType();
		protected string Prefix(string element) {
			string name = (Site != null) ? Site.Name : Name;
			if(string.IsNullOrEmpty(name)) return GetPrefixType() + element + "Component";
			return name + "_" + element;
		}
		void ISupportInitialize.BeginInit() {
			BeginUpdate();
		}
		void ISupportInitialize.EndInit() {
			EndUpdate();
		}
		protected override List<ISerizalizeableElement> GetChildernCore() {
			List<ISerizalizeableElement> list = new List<ISerizalizeableElement>();
			CollectChildren(list, Labels, "Labels");
			CollectChildren(list, Images, "Images");
			return list;
		}
		internal void AddComponentToWinDesignTimeSurface(IComponent component) {
			AddComponentToDesignTimeSurface(component);
			ForceUpdateModel();
		}
		internal void RemoveComponentFromWinDesignTimeSurface(IComponent component) {
			RemoveComponentFromDesignTimeSurface(component);
			ForceUpdateModel();
		}
	}
	public class NotificationHelper : IDisposable {
		IGaugeContainer containerCore = null;
		IComponent componentCore = null;
		string propertyNameCore;
		object oldValueCore = null;
		object newValueCore = null;
		public NotificationHelper(IGaugeContainer container, IComponent component, string propertyName, object oldValue, object newValue) {
			containerCore = container;
			componentCore = component;
			propertyNameCore = propertyName;
			oldValueCore = oldValue;
			newValueCore = newValue;
			if(containerCore != null) containerCore.ComponentChanging(componentCore, propertyNameCore);
		}
		void IDisposable.Dispose() {
			if(containerCore != null) containerCore.ComponentChanged(componentCore, propertyNameCore, oldValueCore, newValueCore);
			containerCore = null;
			componentCore = null;
			propertyNameCore = null;
			oldValueCore = null;
			newValueCore = null;
		}
	}
	public abstract class BaseGaugeProviderWin : BaseObject {
		BaseGaugeWin ownerCore;
		BaseGaugeChangedHandler gaugeChangedHandlerCore;
		LabelComponentCollection labelsCore;
		ImageIndicatorComponentCollection imageIndicatorsCore;
		public BaseGaugeProviderWin(BaseGaugeWin gauge, BaseGaugeChangedHandler handler)
			: base() {
			this.ownerCore = gauge;
			this.gaugeChangedHandlerCore = handler;
		}
		protected override void OnCreate() {
			labelsCore = new LabelComponentCollection();
			imageIndicatorsCore = new ImageIndicatorComponentCollection();
			Labels.CollectionChanged += OnLabelsCollectionChanged;
			Images.CollectionChanged += OnImageIndicatorsCollectrionChanged;
		}
		protected override void OnDispose() {
			if(Labels != null) {
				Labels.CollectionChanged -= OnLabelsCollectionChanged;
				Labels.Dispose();
				this.labelsCore = null;
			}
			if(Images != null) {
				Images.CollectionChanged -= OnImageIndicatorsCollectrionChanged;
				Images.Dispose();
				this.imageIndicatorsCore = null;
			}
			this.gaugeChangedHandlerCore = null;
			this.ownerCore = null;
		}
		public virtual void BuildModel(BaseGaugeModel model) {
			model.Composite.AddRange(Labels.ToArray());
			model.Composite.AddRange(Images.ToArray());
		}
		void OnLabelsCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<LabelComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnImageIndicatorsCollectrionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<ImageIndicatorComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		protected BaseGaugeWin Owner {
			get { return ownerCore; }
		}
		protected void OnCollectionElementChanged(ElementChangedType type, BaseElement<IRenderableElement> element) {
			switch(type) {
				case ElementChangedType.ElementAdded: OnElementAdded(element);
					break;
				case ElementChangedType.ElementRemoved: OnElementRemoved(element);
					break;
				case ElementChangedType.ElementDisposed: OnElementDisposed(element);
					break;
			}
		}
		void OnElementDisposed(BaseElement<IRenderableElement> element) {
			if(Owner != null) ((IGauge)Owner).RemoveGaugeElement(element);
		}
		void OnElementAdded(BaseElement<IRenderableElement> element) {
			element.Changed += OnComponentChanged;
			if(Owner != null) Owner.AddComponentToWinDesignTimeSurface((IComponent)element);
		}
		void OnElementRemoved(BaseElement<IRenderableElement> element) {
			element.Changed -= OnComponentChanged;
			if(Owner != null) Owner.RemoveComponentFromWinDesignTimeSurface((IComponent)element);
		}
		void OnComponentChanged(object sender, EventArgs e) {
			if(gaugeChangedHandlerCore != null) gaugeChangedHandlerCore(sender, e);
		}
		public LabelComponentCollection Labels {
			get { return labelsCore; }
		}
		public ImageIndicatorComponentCollection Images {
			get { return imageIndicatorsCore; }
		}
	}
	public static class WinGaugeExtention {
		delegate BaseElement<IRenderableElement> WinComponentCreator(BaseGaugeWin gaugeInterface);
		static Dictionary<Type, WinComponentCreator> Creators;
		static WinGaugeExtention() {
			Creators = new Dictionary<Type, WinComponentCreator>();
			Creators.Add(
					typeof(LabelComponent),
					delegate(BaseGaugeWin gauge) { return gauge.AddLabel(); }
				);
			Creators.Add(
					typeof(ImageIndicatorComponent),
					delegate(BaseGaugeWin gauge) { return gauge.AddImageIndicator(); }
				);
			Creators.Add(
					typeof(StateImageIndicatorComponent),
					delegate(BaseGaugeWin gauge) {
						CircularGauge cGauge = gauge as CircularGauge;
						return (cGauge != null) ? cGauge.AddStateImageIndicator() : null;
					}
				);
			Creators.Add(
					typeof(ArcScaleComponent),
					delegate(BaseGaugeWin gauge) {
						CircularGauge cGauge = gauge as CircularGauge;
						return (cGauge != null) ? cGauge.AddScale() : null;
					}
				);
			Creators.Add(
					typeof(ArcScaleBackgroundLayerComponent),
					delegate(BaseGaugeWin gauge) {
						CircularGauge cGauge = gauge as CircularGauge;
						return (cGauge != null) ? cGauge.AddBackgroundLayer() : null;
					}
				);
			Creators.Add(
					typeof(ArcScaleEffectLayerComponent),
					delegate(BaseGaugeWin gauge) {
						CircularGauge cGauge = gauge as CircularGauge;
						return (cGauge != null) ? cGauge.AddEffectLayer() : null;
					}
				);
			Creators.Add(
					typeof(ArcScaleNeedleComponent),
					delegate(BaseGaugeWin gauge) {
						CircularGauge cGauge = gauge as CircularGauge;
						return (cGauge != null) ? cGauge.AddNeedle() : null;
					}
				);
			Creators.Add(
					typeof(ArcScaleRangeBarComponent),
					delegate(BaseGaugeWin gauge) {
						CircularGauge cGauge = gauge as CircularGauge;
						return (cGauge != null) ? cGauge.AddRangeBar() : null;
					}
				);
			Creators.Add(
					typeof(ArcScaleMarkerComponent),
					delegate(BaseGaugeWin gauge) {
						CircularGauge cGauge = gauge as CircularGauge;
						return (cGauge != null) ? cGauge.AddMarker() : null;
					}
				);
			Creators.Add(
					typeof(ArcScaleStateIndicatorComponent),
					delegate(BaseGaugeWin gauge) {
						CircularGauge cGauge = gauge as CircularGauge;
						return (cGauge != null) ? cGauge.AddStateIndicator() : null;
					}
				);
			Creators.Add(
					typeof(ArcScaleSpindleCapComponent),
					delegate(BaseGaugeWin gauge) {
						CircularGauge cGauge = gauge as CircularGauge;
						return (cGauge != null) ? cGauge.AddSpindleCap() : null;
					}
				);
			Creators.Add(
					typeof(LinearScaleComponent),
					delegate(BaseGaugeWin gauge) {
						LinearGauge lGauge = gauge as LinearGauge;
						return (lGauge != null) ? lGauge.AddScale() : null;
					}
				);
			Creators.Add(
					typeof(LinearScaleBackgroundLayerComponent),
					delegate(BaseGaugeWin gauge) {
						LinearGauge lGauge = gauge as LinearGauge;
						return (lGauge != null) ? lGauge.AddBackgroundLayer() : null;
					}
				);
			Creators.Add(
					typeof(LinearScaleEffectLayerComponent),
					delegate(BaseGaugeWin gauge) {
						LinearGauge lGauge = gauge as LinearGauge;
						return (lGauge != null) ? lGauge.AddEffectLayer() : null;
					}
				);
			Creators.Add(
					typeof(LinearScaleLevelComponent),
					delegate(BaseGaugeWin gauge) {
						LinearGauge lGauge = gauge as LinearGauge;
						return (lGauge != null) ? lGauge.AddLevel() : null;
					}
				);
			Creators.Add(
					typeof(LinearScaleRangeBarComponent),
					delegate(BaseGaugeWin gauge) {
						LinearGauge lGauge = gauge as LinearGauge;
						return (lGauge != null) ? lGauge.AddRangeBar() : null;
					}
				);
			Creators.Add(
					typeof(LinearScaleMarkerComponent),
					delegate(BaseGaugeWin gauge) {
						LinearGauge lGauge = gauge as LinearGauge;
						return (lGauge != null) ? lGauge.AddMarker() : null;
					}
				);
			Creators.Add(
					typeof(LinearScaleStateIndicatorComponent),
					delegate(BaseGaugeWin gauge) {
						LinearGauge lGauge = gauge as LinearGauge;
						return (lGauge != null) ? lGauge.AddStateIndicator() : null;
					}
				);
			Creators.Add(
					typeof(DigitalBackgroundLayerComponent),
					delegate(BaseGaugeWin gauge) {
						DigitalGauge dGauge = gauge as DigitalGauge;
						return (dGauge != null) ? dGauge.AddBackgroundLayer() : null;
					}
				);
			Creators.Add(
					typeof(DigitalEffectLayerComponent),
					delegate(BaseGaugeWin gauge) {
						DigitalGauge dGauge = gauge as DigitalGauge;
						return (dGauge != null) ? dGauge.AddEffectLayer() : null;
					}
				);
			Creators.Add(
					typeof(StateIndicatorComponent),
					delegate(BaseGaugeWin gauge) {
						StateIndicatorGauge sGauge = gauge as StateIndicatorGauge;
						return (sGauge != null) ? sGauge.AddIndicator() : null;
					}
				);
		}
		public static BaseElement<IRenderableElement> AddGaugeElement(IGauge gauge, Type elementType) {
			BaseElement<IRenderableElement> result = null;
			if(Creators.ContainsKey(elementType)) result = Creators[elementType](gauge as BaseGaugeWin);
			return result;
		}
	}
	static class BaseDesignerElementVisualizerHelpers {
		static Pen linesPen;
		static Pen arrowPen;
		static BaseDesignerElementVisualizerHelpers() {
			arrowPen = new Pen(Color.FromArgb(230, 255, 0, 0), 0.1f);
			linesPen = new Pen(Color.FromArgb(230, 0, 0, 255), 0.1f);
			linesPen.DashPattern = new float[] { 15, 5, 3, 5 };
			linesPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
		}
		public static void DrawBoundsDesignerElements(Graphics g) {
			g.DrawLine(linesPen, 5, 0, 250, 0);
			g.DrawLine(linesPen, 0, 5, 0, 250);
			g.DrawLine(arrowPen, -5, 0, 5, 0);
			g.DrawLine(arrowPen, 0, -5, 0, 5);
			g.DrawLine(arrowPen, 240, -2, 250, 0);
			g.DrawLine(arrowPen, 240, 2, 250, 0);
			g.DrawLine(arrowPen, 2, 240, 0, 250);
			g.DrawLine(arrowPen, -2, 240, 0, 250);
		}
	}
	public class OptionsToolTip : BaseOptions {
		string tooltipCore;
		string tooltipFormatCore;
		string tooltipTitleCore;
		string tooltipTitleFormatCore;
		ToolTipIconType tooltipIconTypeCore;
		public OptionsToolTip() {
			this.tooltipCore = string.Empty;
			this.tooltipFormatCore = "{1:F2}";
			this.tooltipTitleCore = string.Empty;
			this.tooltipTitleFormatCore = "";
			this.tooltipIconTypeCore = ToolTipIconType.None;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			OptionsToolTip source = options as OptionsToolTip;
			if(source != null) {
				this.tooltipCore = source.tooltipCore;
				this.tooltipTitleCore = source.tooltipTitleCore;
				this.tooltipFormatCore = source.tooltipFormatCore;
				this.tooltipTitleFormatCore = source.tooltipTitleFormatCore;
				this.tooltipIconTypeCore = source.tooltipIconTypeCore;
			}
		}
		internal bool ShouldSerializeCore(IComponent owner) {
			return ShouldSerialize(owner);
		}
		[XtraSerializableProperty(), DefaultValue("{1:F2}")]
		public string TooltipFormat {
			get { return tooltipFormatCore; }
			set {
				if(value == null) value = string.Empty;
				if(TooltipFormat == value) return;
				string prev = TooltipTitleFormat;
				tooltipFormatCore = value;
				OnChanged("TooltipFormat", prev, value);
			}
		}
		[XtraSerializableProperty(), DefaultValue("")]
		public string TooltipTitleFormat {
			get { return tooltipTitleFormatCore; }
			set {
				if(value == null) value = string.Empty;
				if(TooltipTitleFormat == value) return;
				string prev = TooltipTitleFormat;
				tooltipTitleFormatCore = value;
				OnChanged("TooltipTitleFormat", prev, value);
			}
		}
		[XtraSerializableProperty(), DefaultValue("")]
		[Editor(ControlConstants.MultilineStringEditor, typeof(System.Drawing.Design.UITypeEditor))]
		public string Tooltip {
			get { return tooltipCore; }
			set {
				if(Tooltip == value) return;
				string prev = Tooltip;
				tooltipCore = value;
				OnChanged("ToolTip", prev, value);
			}
		}
		[XtraSerializableProperty(), DefaultValue("")]
		public string TooltipTitle {
			get { return tooltipTitleCore; }
			set {
				if(TooltipTitle == value) return;
				string prev = TooltipTitle;
				tooltipTitleCore = value;
				OnChanged("ToolTipTitle", prev, value);
			}
		}
		[XtraSerializableProperty(), DefaultValue(ToolTipIconType.None)]
		public ToolTipIconType TooltipIconType {
			get { return tooltipIconTypeCore; }
			set {
				if(TooltipIconType == value) return;
				ToolTipIconType prev = TooltipIconType;
				tooltipIconTypeCore = value;
				OnChanged("ToolTipIconType", prev, value);
			}
		}
	}
}
