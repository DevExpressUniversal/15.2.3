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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Win.Wizard;
namespace DevExpress.XtraGauges.Win.Gauges.Circular {
	public class CircularGaugeProvider : BaseGaugeProviderWin, ICircularGauge {
		ArcScaleComponentCollection scalesCore;
		ArcScaleBackgroundLayerComponentCollection backgroundLayersCore;
		ArcScaleNeedleComponentCollection needlesCore;
		ArcScaleRangeBarComponentCollection rangeBarsCore;
		ArcScaleMarkerComponentCollection markersCore;
		ArcScaleSpindleCapComponentCollection spindleCapsCore;
		ArcScaleEffectLayerComponentCollection effectsCore;
		ArcScaleStateIndicatorComponentCollection indicatorsCore;
		StateImageIndicatorComponentCollection stateImageIndicatorsCore;
		public CircularGaugeProvider(CircularGauge gauge, BaseGaugeChangedHandler handler)
			: base(gauge, handler) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.scalesCore = new ArcScaleComponentCollection();
			this.backgroundLayersCore = new ArcScaleBackgroundLayerComponentCollection();
			this.needlesCore = new ArcScaleNeedleComponentCollection();
			this.rangeBarsCore = new ArcScaleRangeBarComponentCollection();
			this.markersCore = new ArcScaleMarkerComponentCollection();
			this.spindleCapsCore = new ArcScaleSpindleCapComponentCollection();
			this.effectsCore = new ArcScaleEffectLayerComponentCollection();
			this.indicatorsCore = new ArcScaleStateIndicatorComponentCollection();
			this.stateImageIndicatorsCore = new StateImageIndicatorComponentCollection();
			Scales.CollectionChanged += OnScalesCollectionChanged;
			BackgroundLayers.CollectionChanged += OnBackgroundLayersCollectionChanged;
			Needles.CollectionChanged += OnNeedlesCollectionChanged;
			SpindleCaps.CollectionChanged += OnSpindleCapsCollectionChanged;
			RangeBars.CollectionChanged += OnRangeBarsCollectionChanged;
			Markers.CollectionChanged += OnMarkersCollectionChanged;
			EffectLayers.CollectionChanged += OnEffectsCollectionChanged;
			Indicators.CollectionChanged += OnIndicatorsCollectionChanged;
			ImageIndicators.CollectionChanged += OnStateImageIndicatorsCollectionChanged;
		}
		protected override void OnDispose() {
			if(Scales != null) {
				Scales.CollectionChanged -= OnScalesCollectionChanged;
				Scales.Dispose();
				scalesCore = null;
			}
			if(BackgroundLayers != null) {
				BackgroundLayers.CollectionChanged -= OnBackgroundLayersCollectionChanged;
				BackgroundLayers.Dispose();
				backgroundLayersCore = null;
			}
			if(Needles != null) {
				Needles.CollectionChanged -= OnNeedlesCollectionChanged;
				Needles.Dispose();
				needlesCore = null;
			}
			if(SpindleCaps != null) {
				SpindleCaps.CollectionChanged -= OnSpindleCapsCollectionChanged;
				SpindleCaps.Dispose();
				spindleCapsCore = null;
			}
			if(RangeBars != null) {
				RangeBars.CollectionChanged -= OnRangeBarsCollectionChanged;
				RangeBars.Dispose();
				rangeBarsCore = null;
			}
			if(Markers != null) {
				Markers.CollectionChanged -= OnMarkersCollectionChanged;
				Markers.Dispose();
				markersCore = null;
			}
			if(EffectLayers != null) {
				EffectLayers.CollectionChanged -= OnEffectsCollectionChanged;
				EffectLayers.Dispose();
				effectsCore = null;
			}
			if(Indicators != null) {
				Indicators.CollectionChanged -= OnIndicatorsCollectionChanged;
				Indicators.Dispose();
				indicatorsCore = null;
			}
			if(ImageIndicators != null) {
				ImageIndicators.CollectionChanged -= OnStateImageIndicatorsCollectionChanged;
				ImageIndicators.Dispose();
				stateImageIndicatorsCore = null;
			}
			base.OnDispose();
		}
		void OnScalesCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<ArcScaleComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnBackgroundLayersCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<ArcScaleBackgroundLayerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnNeedlesCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<ArcScaleNeedleComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnSpindleCapsCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<ArcScaleSpindleCapComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnMarkersCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<ArcScaleMarkerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnRangeBarsCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<ArcScaleRangeBarComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnEffectsCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<ArcScaleEffectLayerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnIndicatorsCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<ArcScaleStateIndicatorComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnStateImageIndicatorsCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<StateImageIndicatorComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		public ArcScaleComponentCollection Scales {
			get { return scalesCore; }
		}
		public StateImageIndicatorComponentCollection ImageIndicators {
			get { return stateImageIndicatorsCore; }
		}
		public ArcScaleBackgroundLayerComponentCollection BackgroundLayers {
			get { return backgroundLayersCore; }
		}
		public ArcScaleNeedleComponentCollection Needles {
			get { return needlesCore; }
		}
		public ArcScaleRangeBarComponentCollection RangeBars {
			get { return rangeBarsCore; }
		}
		public ArcScaleMarkerComponentCollection Markers {
			get { return markersCore; }
		}
		public ArcScaleSpindleCapComponentCollection SpindleCaps {
			get { return spindleCapsCore; }
		}
		public ArcScaleEffectLayerComponentCollection EffectLayers {
			get { return effectsCore; }
		}
		public ArcScaleStateIndicatorComponentCollection Indicators {
			get { return indicatorsCore; }
		}
		public override void BuildModel(BaseGaugeModel model) {
			base.BuildModel(model);
			model.Composite.AddRange(BackgroundLayers.ToArray());
			model.Composite.AddRange(Scales.ToArray());
			model.Composite.AddRange(RangeBars.ToArray());
			model.Composite.AddRange(Markers.ToArray());
			model.Composite.AddRange(Needles.ToArray());
			model.Composite.AddRange(SpindleCaps.ToArray());
			model.Composite.AddRange(EffectLayers.ToArray());
			model.Composite.AddRange(Indicators.ToArray());
			model.Composite.AddRange(ImageIndicators.ToArray());
		}
	}
	[ToolboxItem(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.CircularGaugeDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class CircularGauge : BaseGaugeWin, ICircularGauge {
		public CircularGauge() : base() { }
		protected override BaseGaugeProviderWin CreateGaugeProvider() {
			return new CircularGaugeProvider(this, OnComponentsChanged);
		}
		protected override void ClearCore() {
			base.ClearCore();
			Scales.Clear();
			BackgroundLayers.Clear();
			Markers.Clear();
			Needles.Clear();
			RangeBars.Clear();
			SpindleCaps.Clear();
			EffectLayers.Clear();
			Indicators.Clear();
			ImageIndicators.Clear();
		}
		protected override BaseGaugeModel CreateModel() {
			BaseGaugeModel model = new CircularGaugeModel(this);
			CircularGaugeProvider.BuildModel(model);
			return model;
		}
		protected CircularGaugeProvider CircularGaugeProvider {
			get { return GaugeProvider as CircularGaugeProvider; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("CircularGaugeAutoSize"),
#endif
		DefaultValue(DevExpress.Utils.DefaultBoolean.Default), Category("Layout"),
		DevExpress.Utils.Serializing.XtraSerializableProperty]
		public DevExpress.Utils.DefaultBoolean AutoSize {
			get { return base.AutoSizeByActualBounds; }
			set { base.AutoSizeByActualBounds = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("CircularGaugeImageIndicators"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateImageIndicatorComponentCollection ImageIndicators {
			get { return CircularGaugeProvider.ImageIndicators; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("CircularGaugeScales"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ArcScaleComponentCollection Scales {
			get { return CircularGaugeProvider.Scales; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("CircularGaugeBackgroundLayers"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ArcScaleBackgroundLayerComponentCollection BackgroundLayers {
			get { return CircularGaugeProvider.BackgroundLayers; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("CircularGaugeNeedles"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ArcScaleNeedleComponentCollection Needles {
			get { return CircularGaugeProvider.Needles; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("CircularGaugeRangeBars"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ArcScaleRangeBarComponentCollection RangeBars {
			get { return CircularGaugeProvider.RangeBars; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("CircularGaugeMarkers"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ArcScaleMarkerComponentCollection Markers {
			get { return CircularGaugeProvider.Markers; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("CircularGaugeSpindleCaps"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ArcScaleSpindleCapComponentCollection SpindleCaps {
			get { return CircularGaugeProvider.SpindleCaps; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("CircularGaugeEffectLayers"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ArcScaleEffectLayerComponentCollection EffectLayers {
			get { return CircularGaugeProvider.EffectLayers; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("CircularGaugeIndicators"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ArcScaleStateIndicatorComponentCollection Indicators {
			get { return CircularGaugeProvider.Indicators; }
		}
		protected override List<ISerizalizeableElement> GetChildernCore() {
			List<ISerizalizeableElement> list = new List<ISerizalizeableElement>(base.GetChildernCore());
			CollectChildren(list, Scales, "Scales");
			CollectChildren(list, BackgroundLayers, "BackgroundLayers");
			CollectChildren(list, Markers, "Markers");
			CollectChildren(list, Needles, "Needles");
			CollectChildren(list, RangeBars, "RangeBars");
			CollectChildren(list, SpindleCaps, "SpindleCaps");
			CollectChildren(list, EffectLayers, "EffectLayers");
			CollectChildren(list, Indicators, "Indicators");
			CollectChildren(list, ImageIndicators, "ImageIndicators");
			return list;
		}
		protected sealed override string GetCategoryCore() {
			string[] bgTypes = new string[] { "CircularFull", "CircularHalf", "CircularThreeFourth", "CircularThreeFourth", "CircularQuarter", "CircularWide" };
			if(BackgroundLayers.Count > 0) {
				string bgtype = BackgroundLayers[0].ShapeType.ToString();
				for(int i = 0; i < bgTypes.Length; i++) {
					if(bgtype.StartsWith(bgTypes[i])) return bgTypes[i];
				}
			}
			return base.GetCategoryCore();
		}
		protected sealed override string GetPrefixType() { return "circular"; }
		public StateImageIndicatorComponent AddStateImageIndicator() {
			BeginUpdate();
			string[] names = new string[ImageIndicators.Count];
			int i = 0;
			ImageIndicators.Accept(
					delegate(StateImageIndicatorComponent s) { names[i++] = s.Name; }
				);
			StateImageIndicatorComponent stateImageIndicator = new StateImageIndicatorComponent(UniqueNameHelper.GetUniqueName(Prefix("StateImageIndicator"), names, ImageIndicators.Count + 1));
			InitializeStateImageIndicatorDefault(stateImageIndicator);
			ImageIndicators.Add(stateImageIndicator);
			AddComponentToDesignTimeSurface(stateImageIndicator);
			EndUpdate();
			return stateImageIndicator;
		}
		public ArcScaleComponent AddScale() {
			BeginUpdate();
			string[] names = new string[Scales.Count];
			int i = 0;
			Scales.Accept(
					delegate(ArcScaleComponent s) { names[i++] = s.Name; }
				);
			ArcScaleComponent scale = new ArcScaleComponent(UniqueNameHelper.GetUniqueName(Prefix("Scale"), names, Scales.Count + 1));
			InitializeScaleDefault(scale);
			Scales.Add(scale);
			AddComponentToDesignTimeSurface(scale);
			EndUpdate();
			return scale;
		}
		public ArcScaleNeedleComponent AddNeedle() {
			BeginUpdate();
			string[] names = new string[Needles.Count];
			int i = 0;
			Needles.Accept(
					delegate(ArcScaleNeedleComponent n) { names[i++] = n.Name; }
				);
			ArcScaleNeedleComponent needle = new ArcScaleNeedleComponent(UniqueNameHelper.GetUniqueName(Prefix("Needle"), names, Needles.Count + 1));
			InitializeNeedleDefault(needle);
			Needles.Add(needle);
			AddComponentToDesignTimeSurface(needle);
			EndUpdate();
			return needle;
		}
		public ArcScaleRangeBarComponent AddRangeBar() {
			BeginUpdate();
			string[] names = new string[RangeBars.Count];
			int i = 0;
			RangeBars.Accept(
					delegate(ArcScaleRangeBarComponent r) { names[i++] = r.Name; }
				);
			ArcScaleRangeBarComponent rangeBar = new ArcScaleRangeBarComponent(UniqueNameHelper.GetUniqueName(Prefix("RangeBar"), names, RangeBars.Count + 1));
			InitializeRangeBarDefault(rangeBar);
			RangeBars.Add(rangeBar);
			AddComponentToDesignTimeSurface(rangeBar);
			EndUpdate();
			return rangeBar;
		}
		public ArcScaleBackgroundLayerComponent AddBackgroundLayer() {
			BeginUpdate();
			string[] names = new string[BackgroundLayers.Count];
			int i = 0;
			BackgroundLayers.Accept(
					delegate(ArcScaleBackgroundLayerComponent l) { names[i++] = l.Name; }
				);
			ArcScaleBackgroundLayerComponent layer = new ArcScaleBackgroundLayerComponent(UniqueNameHelper.GetUniqueName(Prefix("BackgroundLayer"), names, BackgroundLayers.Count + 1));
			InitializeBackgroundLayerDefault(layer);
			BackgroundLayers.Add(layer);
			AddComponentToDesignTimeSurface(layer);
			EndUpdate();
			return layer;
		}
		public ArcScaleSpindleCapComponent AddSpindleCap() {
			BeginUpdate();
			string[] names = new string[SpindleCaps.Count];
			int i = 0;
			SpindleCaps.Accept(
					delegate(ArcScaleSpindleCapComponent sc) { names[i++] = sc.Name; }
				);
			ArcScaleSpindleCapComponent spindleCap = new ArcScaleSpindleCapComponent(UniqueNameHelper.GetUniqueName(Prefix("SpindleCap"), names, SpindleCaps.Count + 1));
			InitializeSpindleCapDefault(spindleCap);
			SpindleCaps.Add(spindleCap);
			AddComponentToDesignTimeSurface(spindleCap);
			EndUpdate();
			return spindleCap;
		}
		public ArcScaleEffectLayerComponent AddEffectLayer() {
			BeginUpdate();
			string[] names = new string[EffectLayers.Count];
			int i = 0;
			EffectLayers.Accept(
					delegate(ArcScaleEffectLayerComponent l) { names[i++] = l.Name; }
				);
			ArcScaleEffectLayerComponent layer = new ArcScaleEffectLayerComponent(UniqueNameHelper.GetUniqueName(Prefix("EffectLayer"), names, EffectLayers.Count + 1));
			InitializeEffectLayerDefault(layer);
			EffectLayers.Add(layer);
			AddComponentToDesignTimeSurface(layer);
			EndUpdate();
			return layer;
		}
		public ArcScaleMarkerComponent AddMarker() {
			BeginUpdate();
			string[] names = new string[Markers.Count];
			int i = 0;
			Markers.Accept(
					delegate(ArcScaleMarkerComponent m) { names[i++] = m.Name; }
				);
			ArcScaleMarkerComponent marker = new ArcScaleMarkerComponent(UniqueNameHelper.GetUniqueName(Prefix("Marker"), names, Markers.Count + 1));
			InitializeMarkerDefault(marker);
			Markers.Add(marker);
			AddComponentToDesignTimeSurface(marker);
			EndUpdate();
			return marker;
		}
		public ArcScaleStateIndicatorComponent AddStateIndicator() {
			BeginUpdate();
			string[] names = new string[Indicators.Count];
			int i = 0;
			Indicators.Accept(
					delegate(ArcScaleStateIndicatorComponent m) { names[i++] = m.Name; }
				);
			ArcScaleStateIndicatorComponent indicator = new ArcScaleStateIndicatorComponent(UniqueNameHelper.GetUniqueName(Prefix("Indicator"), names, Indicators.Count + 1));
			InitializeStateIndicatorDefault(indicator);
			Indicators.Add(indicator);
			AddComponentToDesignTimeSurface(indicator);
			EndUpdate();
			return indicator;
		}
		protected void InitializeStateImageIndicatorDefault(StateImageIndicatorComponent stateImageIndicator) {
			if(Scales.Count > 0){
				stateImageIndicator.BeginUpdate();
				ImageIndicatorState state = new ImageIndicatorState("Default");
				state.StartValue = 0;
				state.IntervalLength = 1;
				stateImageIndicator.ImageStateCollection.Add(state);
				stateImageIndicator.IndicatorScale = Scales[0];
				stateImageIndicator.Size = new Size(32, 32);
				stateImageIndicator.StateIndex = null;
				stateImageIndicator.EndUpdate();
			}
		}
		protected void InitializeScaleDefault(ArcScale scale) {
			CircularGaugeExtention.InitializeScaleDefault(scale, new PointF2D(Model.ContentSize.Width / 2, Model.ContentSize.Height / 2));
		}
		protected void InitializeBackgroundLayerDefault(ArcScaleBackgroundLayer backgroundLayer) {
			if(Scales.Count > 0) CircularGaugeExtention.InitializeBackgroundLayerDefault(backgroundLayer, Scales[0]);
		}
		protected void InitializeEffectLayerDefault(ArcScaleEffectLayer effectLayer) {
			if(Scales.Count > 0) CircularGaugeExtention.InitializeEffectLayerDefault(effectLayer, Scales[0]);
		}
		protected void InitializeSpindleCapDefault(ArcScaleSpindleCap spindleCap) {
			if(Scales.Count > 0) CircularGaugeExtention.InitializeSpindleCapDefault(spindleCap, Scales[0]);
		}
		protected void InitializeMarkerDefault(ArcScaleMarker marker) {
			if(Scales.Count > 0) CircularGaugeExtention.InitializeMarkerDefault(marker, Scales[0]);
		}
		protected void InitializeStateIndicatorDefault(ScaleStateIndicator indicator) {
			if(Scales.Count > 0) CircularGaugeExtention.InitializeStateIndicatorDefault(indicator, Scales[0]);
		}
		protected void InitializeNeedleDefault(ArcScaleNeedle needle) {
			if(Scales.Count > 0) CircularGaugeExtention.InitializeNeedleDefault(needle, Scales[0]);
		}
		protected void InitializeRangeBarDefault(ArcScaleRangeBar rangeBar) {
			if(Scales.Count > 0) CircularGaugeExtention.InitializeRangeBarDefault(rangeBar, Scales[0]);
		}
		protected override void InitializeDefaultCore() {
			AddDefaultElements();
		}
		public void AddDefaultElements() {
			using(new ComponentTransaction(((IGauge)this).Container as IComponent)) {
				ClearCore();
				BeginUpdate();
				AddScale();
				AddBackgroundLayer();
				AddNeedle();
				EndUpdate();
			}
		}
		protected void ChangeStyle() {
			StyleChooser.Show(this);
		}
		protected void RunDesigner(bool editIsWin) {
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(this)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new CircularGaugeOverviewDesignerPage("Circular Gauge"),
					new PrimitiveCustomizationDesignerPage<ArcScaleComponent>(1,"Scales", UIHelper.CircularGaugeElementImages[0],Scales.ToArray(),this) ,
					new PrimitiveCustomizationDesignerPage<ArcScaleBackgroundLayerComponent>(2,"Background Layers",UIHelper.CircularGaugeElementImages[1],BackgroundLayers.ToArray(),this) , 
					new PrimitiveCustomizationDesignerPage<ArcScaleNeedleComponent>(3,"Needles",UIHelper.CircularGaugeElementImages[2],Needles.ToArray(),this),  
					new PrimitiveCustomizationDesignerPage<ArcScaleRangeBarComponent>(4,"RangeBars",UIHelper.CircularGaugeElementImages[3],RangeBars.ToArray(),this),  
					new PrimitiveCustomizationDesignerPage<ArcScaleMarkerComponent>(5,"Markers",UIHelper.CircularGaugeElementImages[4],Markers.ToArray(),this) , 
					new PrimitiveCustomizationDesignerPage<ArcScaleSpindleCapComponent>(6,"Spindle Caps",UIHelper.CircularGaugeElementImages[5],SpindleCaps.ToArray(),this),  
					new PrimitiveCustomizationDesignerPage<ArcScaleEffectLayerComponent>(7,"Effect Layers",UIHelper.CircularGaugeElementImages[6],EffectLayers.ToArray(),this),  
					new PrimitiveCustomizationDesignerPage<ArcScaleStateIndicatorComponent>(8,"State Indicators",UIHelper.CircularGaugeElementImages[7],Indicators.ToArray(),this),  
					new PrimitiveCustomizationDesignerPage<LabelComponent>(8,"Labels",UIHelper.UIOtherImages[1],Labels.ToArray(),this),  
					new PrimitiveCustomizationDesignerPage<ImageIndicatorComponent>(8,"Images",UIHelper.UIOtherImages[2],Images.ToArray(),this), 
					new PrimitiveCustomizationDesignerPage<StateImageIndicatorComponent>(8,"Image Indicators",UIHelper.CircularGaugeElementImages[8],ImageIndicators.ToArray(),this),
					new CircularGaugeDataBindingPage(10,"Data Bindings",this)
				};
				if(!editIsWin) {
					List<BaseGaugeDesignerPage> pages = designerform.Pages.ToList();
					pages.RemoveAll(p => p.Caption.Equals("Images") || p.Caption.Equals("Image Indicators"));
					designerform.Pages = pages.ToArray();
				}
				designerform.ShowDialog();
			}
		}
		protected override void SetEnabledCore(bool enabled) {
			ComponentCollectionExtention.SetEnabled(Images, enabled);
			ComponentCollectionExtention.SetEnabled(Labels, enabled);
			ComponentCollectionExtention.SetEnabled(BackgroundLayers, enabled);
			ComponentCollectionExtention.SetEnabled(EffectLayers, enabled);
			ComponentCollectionExtention.SetEnabled(Scales, enabled);
			ComponentCollectionExtention.SetEnabled(Needles, enabled);
			ComponentCollectionExtention.SetEnabled(RangeBars, enabled);
			ComponentCollectionExtention.SetEnabled(Markers, enabled);
			ComponentCollectionExtention.SetEnabled(SpindleCaps, enabled);
			ComponentCollectionExtention.SetEnabled(Indicators, enabled);
			ComponentCollectionExtention.SetEnabled(ImageIndicators, enabled);
		}
		protected override List<BaseElement<IRenderableElement>> FindDependentGaugeElements(BaseElement<IRenderableElement> element) {
			List<BaseElement<IRenderableElement>> resultElements = new List<BaseElement<IRenderableElement>>();
			if(element is IScale) {
				IScale scale = element as IScale;
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(BackgroundLayers, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(EffectLayers, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(Needles, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(RangeBars, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(Markers, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(SpindleCaps, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(Indicators, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(ImageIndicators, scale));
			}
			return resultElements;
		}
		protected sealed override List<string> GetNamesCore() {
			List<string> names = new List<string>(base.GetNamesCore());
			ComponentCollectionExtention.CollectNames(Scales, names);
			ComponentCollectionExtention.CollectNames(BackgroundLayers, names);
			ComponentCollectionExtention.CollectNames(EffectLayers, names);
			ComponentCollectionExtention.CollectNames(Needles, names);
			ComponentCollectionExtention.CollectNames(RangeBars, names);
			ComponentCollectionExtention.CollectNames(Markers, names);
			ComponentCollectionExtention.CollectNames(SpindleCaps, names);
			ComponentCollectionExtention.CollectNames(Indicators, names);
			ComponentCollectionExtention.CollectNames(ImageIndicators, names);
			return names;
		}
		protected sealed override void AddGaugeElementToComponentCollection(IComponent component) {
			if(ComponentCollectionExtention.TryAddComponent(Labels, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Images, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Scales, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(BackgroundLayers, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(EffectLayers, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Needles, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(RangeBars, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Markers, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(SpindleCaps, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Indicators, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(ImageIndicators, component)) return;
		}
		protected sealed override BaseElement<IRenderableElement> DuplicateGaugeElementCore(BaseElement<IRenderableElement> component) {
			IComponent duplicate = null;
			if(ComponentCollectionExtention.TryDuplicateComponent(Labels, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Images, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Scales, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(BackgroundLayers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(EffectLayers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Needles, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(RangeBars, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Markers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(SpindleCaps, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Indicators, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(ImageIndicators, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			return duplicate as BaseElement<IRenderableElement>;
		}
	}
	public class CircularGaugeModel : BaseGaugeModel {
		protected CircularGauge CircilarGauge {
			get { return Owner as CircularGauge; }
		}
		public CircularGaugeModel(BaseGaugeWin gauge) : base(gauge) { }
		public override void Calc(IGauge owner, RectangleF bounds) {
			OnShapesChanged();
			base.Calc(owner, bounds);
		}
		public override SizeF ContentSize {
			get { return new SizeF(250, 250); }
		}
		public override bool SmartLayout {
			get { return true; }
		}
		protected override CustomizeActionInfo[] GetActionsCore() {
			ArrayList list = new ArrayList(
				new CustomizeActionInfo[]{
					new CustomizeActionInfo("RunDesigner", "Run Circular Gauge Designer", "Run Designer", UIHelper.GaugeTypeImages[0]),
					new CustomizeActionInfo("ChangeStyle", "Show Style Chooser", "Change Style", UIHelper.ChangeStyleImage),
					new CustomizeActionInfo("AddScale", "Add default Scale", "Add Scale", UIHelper.CircularGaugeElementImages[0],"Scales"),
					new CustomizeActionInfo("AddBackgroundLayer", "Add default Background Layer", "Add Background Layer", UIHelper.CircularGaugeElementImages[1],"Layers"),
					new CustomizeActionInfo("AddNeedle", "Add default Needle pointer", "Add Needle", UIHelper.CircularGaugeElementImages[2],"Pointers"),
					new CustomizeActionInfo("AddRangeBar", "Add default RangeBar pointer", "Add RangeBar", UIHelper.CircularGaugeElementImages[3],"Pointers"),
					new CustomizeActionInfo("AddMarker", "Add default Marker pointer", "Add Marker", UIHelper.CircularGaugeElementImages[4],"Pointers"),
					new CustomizeActionInfo("AddSpindleCap", "Add default Spindle Cap", "Add Spindle Cap", UIHelper.CircularGaugeElementImages[5],"Layers"),
					new CustomizeActionInfo("AddEffectLayer", "Add default Effect Layer", "Add Effect Layer", UIHelper.CircularGaugeElementImages[6],"Layers"),
					new CustomizeActionInfo("AddStateIndicator", "Add default State Indicator", "Add State Indicator", UIHelper.CircularGaugeElementImages[7],"Other Elements"),
					new CustomizeActionInfo("AddStateImageIndicator", "Add default State Image Indicator", "Add State Image Indicator", UIHelper.CircularGaugeElementImages[8],"Other Elements")
				});
			if(CircilarGauge.Scales.Count == 0) {
				list.RemoveRange(2, 7);
				list.Add(defaultInitialization);
			}
			list.AddRange(base.GetActionsCore());
			return (CustomizeActionInfo[])list.ToArray(typeof(CustomizeActionInfo));
		}
	}
}
