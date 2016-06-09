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
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Layout;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Win.Wizard;
namespace DevExpress.XtraGauges.Win.Gauges.Linear {
	public class LinearGaugeProvider : BaseGaugeProviderWin {
		LinearScaleComponentCollection scalesCore;
		LinearScaleBackgroundLayerComponentCollection backgroundLayersCore;
		LinearScaleRangeBarComponentCollection rangeBarsCore;
		LinearScaleMarkerComponentCollection markersCore;
		LinearScaleLevelComponentCollection levelsCore;
		LinearScaleEffectLayerComponentCollection effectsCore;
		LinearScaleStateIndicatorComponentCollection indicatorsCore;
		public LinearGaugeProvider(LinearGauge owner, BaseGaugeChangedHandler handler)
			: base(owner, handler) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.scalesCore = new LinearScaleComponentCollection();
			this.backgroundLayersCore = new LinearScaleBackgroundLayerComponentCollection();
			this.rangeBarsCore = new LinearScaleRangeBarComponentCollection();
			this.markersCore = new LinearScaleMarkerComponentCollection();
			this.levelsCore = new LinearScaleLevelComponentCollection();
			this.effectsCore = new LinearScaleEffectLayerComponentCollection();
			this.indicatorsCore = new LinearScaleStateIndicatorComponentCollection();
			Scales.CollectionChanged += OnScalesCollectionChanged;
			Levels.CollectionChanged += OnLevelsCollectionChanged;
			BackgroundLayers.CollectionChanged += OnBackgroundLayersCollectionChanged;
			RangeBars.CollectionChanged += OnRangeBarsCollectionChanged;
			Markers.CollectionChanged += OnMarkersCollectionChanged;
			EffectLayers.CollectionChanged += OnEffectsCollectionChanged;
			Indicators.CollectionChanged += OnIndicatorsCollectionChanged;
		}
		protected override void OnDispose() {
			if(Scales != null) {
				Scales.CollectionChanged -= OnScalesCollectionChanged;
				Scales.Dispose();
				scalesCore = null;
			}
			if(Levels != null) {
				Levels.CollectionChanged -= OnLevelsCollectionChanged;
				Levels.Dispose();
				levelsCore = null;
			}
			if(BackgroundLayers != null) {
				BackgroundLayers.CollectionChanged -= OnBackgroundLayersCollectionChanged;
				BackgroundLayers.Dispose();
				backgroundLayersCore = null;
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
			base.OnDispose();
		}
		void OnScalesCollectionChanged(CollectionChangedEventArgs<LinearScaleComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnLevelsCollectionChanged(CollectionChangedEventArgs<LinearScaleLevelComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnBackgroundLayersCollectionChanged(CollectionChangedEventArgs<LinearScaleBackgroundLayerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnRangeBarsCollectionChanged(CollectionChangedEventArgs<LinearScaleRangeBarComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnMarkersCollectionChanged(CollectionChangedEventArgs<LinearScaleMarkerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnEffectsCollectionChanged(CollectionChangedEventArgs<LinearScaleEffectLayerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnIndicatorsCollectionChanged(CollectionChangedEventArgs<LinearScaleStateIndicatorComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		public LinearScaleComponentCollection Scales {
			get { return scalesCore; }
		}
		public LinearScaleLevelComponentCollection Levels {
			get { return levelsCore; }
		}
		public LinearScaleBackgroundLayerComponentCollection BackgroundLayers {
			get { return backgroundLayersCore; }
		}
		public LinearScaleEffectLayerComponentCollection EffectLayers {
			get { return effectsCore; }
		}
		public LinearScaleRangeBarComponentCollection RangeBars {
			get { return rangeBarsCore; }
		}
		public LinearScaleMarkerComponentCollection Markers {
			get { return markersCore; }
		}
		public LinearScaleStateIndicatorComponentCollection Indicators {
			get { return indicatorsCore; }
		}
		public override void BuildModel(BaseGaugeModel model) {
			base.BuildModel(model);
			ModelRoot rotationNode = new ModelRoot(PredefinedCoreNames.LinearGaugeRotationNode);
			BuildRotationNode(rotationNode);
			bool vertical = ((Owner as LinearGauge).Orientation == ScaleOrientation.Vertical);
			rotationNode.Angle = vertical ? 0f : 90f;
			rotationNode.Location = vertical ? PointF2D.Empty : new PointF2D(250f, 0f);
			model.Composite.Add(rotationNode);
		}
		public void BuildRotationNode(BaseCompositePrimitive rotationNode) {
			rotationNode.Composite.AddRange(Scales.ToArray());
			rotationNode.Composite.AddRange(BackgroundLayers.ToArray());
			rotationNode.Composite.AddRange(Levels.ToArray());
			rotationNode.Composite.AddRange(RangeBars.ToArray());
			rotationNode.Composite.AddRange(Markers.ToArray());
			rotationNode.Composite.AddRange(EffectLayers.ToArray());
			rotationNode.Composite.AddRange(Indicators.ToArray());
		}
	}
	[ToolboxItem(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.LinearGaugeDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearGauge : BaseGaugeWin, ILinearGauge {
		ScaleOrientation orientationCore = ScaleOrientation.Vertical;
		public LinearGauge() : base() { }
		protected override BaseGaugeProviderWin CreateGaugeProvider() {
			return new LinearGaugeProvider(this, OnComponentsChanged);
		}
		protected override void ClearCore() {
			base.ClearCore();
			Scales.Clear();
			BackgroundLayers.Clear();
			Levels.Clear();
			RangeBars.Clear();
			Markers.Clear();
			EffectLayers.Clear();
			Indicators.Clear();
		}
		protected override BaseGaugeModel CreateModel() {
			BaseGaugeModel model = new LinearGaugeModel(this);
			LinearGaugeProvider.BuildModel(model);
			return model;
		}
		protected override void RemoveElementFromModel(BaseElement<IRenderableElement> element) {
			ModelRoot rotationNode = Model.Composite[PredefinedCoreNames.LinearGaugeRotationNode] as ModelRoot;
			rotationNode.Composite.Remove(element);
		}
		protected LinearGaugeProvider LinearGaugeProvider {
			get { return GaugeProvider as LinearGaugeProvider; }
		}
		protected override DevExpress.XtraGauges.Core.Layout.PreferredLayoutType CalcPreferredLayoutType() {
			return (Orientation == ScaleOrientation.Vertical) ? PreferredLayoutType.Right : PreferredLayoutType.Bottom;
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearGaugeAutoSize"),
#endif
		DefaultValue(DevExpress.Utils.DefaultBoolean.Default), Category("Layout"),
		DevExpress.Utils.Serializing.XtraSerializableProperty]
		public DevExpress.Utils.DefaultBoolean AutoSize {
			get { return base.AutoSizeByActualBounds; }
			set { base.AutoSizeByActualBounds = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearGaugeOrientation"),
#endif
DefaultValue(ScaleOrientation.Vertical)]
		[XtraSerializableProperty]
		public ScaleOrientation Orientation {
			get { return orientationCore; }
			set {
				if(Orientation == value) return;
				orientationCore = value;
				OnModelChanged(true);
			}
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearGaugeScales"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearScaleComponentCollection Scales {
			get { return LinearGaugeProvider.Scales; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearGaugeLevels"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearScaleLevelComponentCollection Levels {
			get { return LinearGaugeProvider.Levels; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearGaugeBackgroundLayers"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearScaleBackgroundLayerComponentCollection BackgroundLayers {
			get { return LinearGaugeProvider.BackgroundLayers; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearGaugeEffectLayers"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearScaleEffectLayerComponentCollection EffectLayers {
			get { return LinearGaugeProvider.EffectLayers; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearGaugeMarkers"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearScaleMarkerComponentCollection Markers {
			get { return LinearGaugeProvider.Markers; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearGaugeRangeBars"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearScaleRangeBarComponentCollection RangeBars {
			get { return LinearGaugeProvider.RangeBars; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearGaugeIndicators"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearScaleStateIndicatorComponentCollection Indicators {
			get { return LinearGaugeProvider.Indicators; }
		}
		protected override List<ISerizalizeableElement> GetChildernCore() {
			List<ISerizalizeableElement> list = new List<ISerizalizeableElement>(base.GetChildernCore());
			CollectChildren(list, Scales, "Scales");
			CollectChildren(list, BackgroundLayers, "BackgroundLayers");
			CollectChildren(list, Levels, "Levels");
			CollectChildren(list, RangeBars, "RangeBars");
			CollectChildren(list, Markers, "Markers");
			CollectChildren(list, EffectLayers, "EffectLayers");
			CollectChildren(list, Indicators, "Indicators");
			return list;
		}
		protected sealed override string GetCategoryCore() {
			return Orientation.ToString();
		}
		protected override string GetPrefixType() { return "linear"; }
		public LinearScaleComponent AddScale() {
			BeginUpdate();
			string[] names = new string[Scales.Count];
			int i = 0;
			Scales.Accept(
					delegate(LinearScaleComponent s) { names[i++] = s.Name; }
				);
			LinearScaleComponent scale = new LinearScaleComponent(UniqueNameHelper.GetUniqueName(Prefix("Scale"), names, Scales.Count + 1));
			InitializeScaleDefault(scale);
			Scales.Add(scale);
			AddComponentToDesignTimeSurface(scale);
			EndUpdate();
			return scale;
		}
		public LinearScaleLevelComponent AddLevel() {
			BeginUpdate();
			string[] names = new string[Levels.Count];
			int i = 0;
			Levels.Accept(
					delegate(LinearScaleLevelComponent s) { names[i++] = s.Name; }
				);
			LinearScaleLevelComponent level = new LinearScaleLevelComponent(UniqueNameHelper.GetUniqueName(Prefix("Level"), names, Levels.Count + 1));
			InitializeLevelDefault(level);
			Levels.Add(level);
			AddComponentToDesignTimeSurface(level);
			EndUpdate();
			return level;
		}
		public LinearScaleBackgroundLayerComponent AddBackgroundLayer() {
			BeginUpdate();
			string[] names = new string[BackgroundLayers.Count];
			int i = 0;
			BackgroundLayers.Accept(
					delegate(LinearScaleBackgroundLayerComponent l) { names[i++] = l.Name; }
				);
			LinearScaleBackgroundLayerComponent layer = new LinearScaleBackgroundLayerComponent(UniqueNameHelper.GetUniqueName(Prefix("BackgroundLayer"), names, BackgroundLayers.Count + 1));
			InitializeBackgroundLayerDefault(layer);
			BackgroundLayers.Add(layer);
			AddComponentToDesignTimeSurface(layer);
			EndUpdate();
			return layer;
		}
		public LinearScaleEffectLayerComponent AddEffectLayer() {
			BeginUpdate();
			string[] names = new string[EffectLayers.Count];
			int i = 0;
			EffectLayers.Accept(
					delegate(LinearScaleEffectLayerComponent l) { names[i++] = l.Name; }
				);
			LinearScaleEffectLayerComponent layer = new LinearScaleEffectLayerComponent(UniqueNameHelper.GetUniqueName(Prefix("EffectLayer"), names, EffectLayers.Count + 1));
			InitializeEffectLayerDefault(layer);
			EffectLayers.Add(layer);
			AddComponentToDesignTimeSurface(layer);
			EndUpdate();
			return layer;
		}
		public LinearScaleMarkerComponent AddMarker() {
			BeginUpdate();
			string[] names = new string[Markers.Count];
			int i = 0;
			Markers.Accept(
					delegate(LinearScaleMarkerComponent m) { names[i++] = m.Name; }
				);
			LinearScaleMarkerComponent marker = new LinearScaleMarkerComponent(UniqueNameHelper.GetUniqueName(Prefix("Marker"), names, Markers.Count + 1));
			InitializeMarkerDefault(marker);
			Markers.Add(marker);
			AddComponentToDesignTimeSurface(marker);
			EndUpdate();
			return marker;
		}
		public LinearScaleRangeBarComponent AddRangeBar() {
			BeginUpdate();
			string[] names = new string[RangeBars.Count];
			int i = 0;
			RangeBars.Accept(
					delegate(LinearScaleRangeBarComponent r) { names[i++] = r.Name; }
				);
			LinearScaleRangeBarComponent rangeBar = new LinearScaleRangeBarComponent(UniqueNameHelper.GetUniqueName(Prefix("RangeBar"), names, RangeBars.Count + 1));
			InitializeRangeBarDefault(rangeBar);
			RangeBars.Add(rangeBar);
			AddComponentToDesignTimeSurface(rangeBar);
			EndUpdate();
			return rangeBar;
		}
		public LinearScaleStateIndicatorComponent AddStateIndicator() {
			BeginUpdate();
			string[] names = new string[Indicators.Count];
			int i = 0;
			Indicators.Accept(
					delegate(LinearScaleStateIndicatorComponent m) { names[i++] = m.Name; }
				);
			LinearScaleStateIndicatorComponent indicator = new LinearScaleStateIndicatorComponent(UniqueNameHelper.GetUniqueName(Prefix("Indicator"), names, Indicators.Count + 1));
			InitializeStateIndicatorDefault(indicator);
			Indicators.Add(indicator);
			AddComponentToDesignTimeSurface(indicator);
			EndUpdate();
			return indicator;
		}
		protected void InitializeScaleDefault(LinearScale scale) {
			LinearGaugeExtention.InitializeScaleDefault(scale,
					new PointF2D(Model.ContentSize.Width * 0.5f, Model.ContentSize.Height * 0.86f),
					new PointF2D(Model.ContentSize.Width * 0.5f, Model.ContentSize.Height * 0.14f)
				);
		}
		protected void InitializeLevelDefault(LinearScaleLevel level) {
			if(Scales.Count > 0) LinearGaugeExtention.InitializeLevelDefault(level, Scales[0]);
		}
		protected void InitializeBackgroundLayerDefault(LinearScaleBackgroundLayer backgroundLayer) {
			if(Scales.Count > 0) LinearGaugeExtention.InitializeBackgroundLayerDefault(backgroundLayer, Scales[0]);
		}
		protected void InitializeEffectLayerDefault(LinearScaleEffectLayer effectLayer) {
			if(Scales.Count > 0) LinearGaugeExtention.InitializeEffectLayerDefault(effectLayer, Scales[0]);
		}
		protected void InitializeMarkerDefault(LinearScaleMarker marker) {
			if(Scales.Count > 0) LinearGaugeExtention.InitializeMarkerDefault(marker, Scales[0]);
		}
		protected void InitializeRangeBarDefault(LinearScaleRangeBar rangeBar) {
			if(Scales.Count > 0) LinearGaugeExtention.InitializeRangeBarDefault(rangeBar, Scales[0]);
		}
		protected void InitializeStateIndicatorDefault(ScaleStateIndicator indicator) {
			if(Scales.Count > 0) LinearGaugeExtention.InitializeStateIndicatorDefault(indicator, Scales[0]);
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
				AddLevel();
				EndUpdate();
			}
		}
		protected void ChangeStyle() {
			StyleChooser.Show(this);
		}
		protected void RunDesigner(bool editIsWin) {
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(this)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new LinearGaugeOverviewDesignerPage("Linear Gauge"),
					new PrimitiveCustomizationDesignerPage<LinearScaleComponent>(1,"Scales",UIHelper.LinearGaugeElementImages[0],Scales.ToArray(),this) ,
					new PrimitiveCustomizationDesignerPage<LinearScaleBackgroundLayerComponent>(2,"Background Layers",UIHelper.LinearGaugeElementImages[1],BackgroundLayers.ToArray(),this) , 
					new PrimitiveCustomizationDesignerPage<LinearScaleLevelComponent>(3,"Levels",UIHelper.LinearGaugeElementImages[2],Levels.ToArray(),this),  
					new PrimitiveCustomizationDesignerPage<LinearScaleRangeBarComponent>(4,"RangeBars",UIHelper.LinearGaugeElementImages[3],RangeBars.ToArray(),this),  
					new PrimitiveCustomizationDesignerPage<LinearScaleMarkerComponent>(5,"Markers",UIHelper.LinearGaugeElementImages[4],Markers.ToArray(),this) , 
					new PrimitiveCustomizationDesignerPage<LinearScaleEffectLayerComponent>(6,"Effect Layers",UIHelper.LinearGaugeElementImages[5],EffectLayers.ToArray(),this) , 
					new PrimitiveCustomizationDesignerPage<LinearScaleStateIndicatorComponent>(7,"State Indicators",UIHelper.LinearGaugeElementImages[6],Indicators.ToArray(),this) , 
					new PrimitiveCustomizationDesignerPage<LabelComponent>(9,"Labels",UIHelper.UIOtherImages[1],Labels.ToArray(),this) , 
					new PrimitiveCustomizationDesignerPage<ImageIndicatorComponent>(9,"Images",UIHelper.UIOtherImages[2],Images.ToArray(),this) , 
					new LinearGaugeDataBindingPage(10,"Data Bindings",this)
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
			ComponentCollectionExtention.SetEnabled(Labels, enabled);
			ComponentCollectionExtention.SetEnabled(Images, enabled);
			ComponentCollectionExtention.SetEnabled(BackgroundLayers, enabled);
			ComponentCollectionExtention.SetEnabled(EffectLayers, enabled);
			ComponentCollectionExtention.SetEnabled(Scales, enabled);
			ComponentCollectionExtention.SetEnabled(Levels, enabled);
			ComponentCollectionExtention.SetEnabled(RangeBars, enabled);
			ComponentCollectionExtention.SetEnabled(Markers, enabled);
			ComponentCollectionExtention.SetEnabled(Indicators, enabled);
		}
		protected override List<BaseElement<IRenderableElement>> FindDependentGaugeElements(BaseElement<IRenderableElement> element) {
			List<BaseElement<IRenderableElement>> resultElements = new List<BaseElement<IRenderableElement>>();
			if(element is IScale) {
				IScale scale = element as IScale;
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(BackgroundLayers, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(EffectLayers, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(Levels, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(RangeBars, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(Markers, scale));
				resultElements.AddRange(ComponentCollectionExtention.FindScaleDependentElements(Indicators, scale));
			}
			return resultElements;
		}
		protected sealed override List<string> GetNamesCore() {
			List<string> names = new List<string>(base.GetNamesCore());
			ComponentCollectionExtention.CollectNames(Scales, names);
			ComponentCollectionExtention.CollectNames(BackgroundLayers, names);
			ComponentCollectionExtention.CollectNames(EffectLayers, names);
			ComponentCollectionExtention.CollectNames(Levels, names);
			ComponentCollectionExtention.CollectNames(RangeBars, names);
			ComponentCollectionExtention.CollectNames(Markers, names);
			ComponentCollectionExtention.CollectNames(Indicators, names);
			return names;
		}
		protected sealed override void AddGaugeElementToComponentCollection(IComponent component) {
			if(ComponentCollectionExtention.TryAddComponent(Labels, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Images, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Scales, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(BackgroundLayers, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(EffectLayers, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Levels, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(RangeBars, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Markers, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Indicators, component)) return;
		}
		protected sealed override BaseElement<IRenderableElement> DuplicateGaugeElementCore(BaseElement<IRenderableElement> component) {
			IComponent duplicate = null;
			if(ComponentCollectionExtention.TryDuplicateComponent(Labels, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Images, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Scales, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(BackgroundLayers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(EffectLayers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Levels, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(RangeBars, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Markers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Indicators, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			return duplicate as BaseElement<IRenderableElement>;
		}
	}
	public class LinearGaugeModel : BaseGaugeModel {
		protected LinearGauge LinearGauge {
			get { return Owner as LinearGauge; }
		}
		public LinearGaugeModel(BaseGaugeWin gauge) : base(gauge) { }
		public override void Calc(IGauge owner, RectangleF bounds) {
			OnShapesChanged();
			base.Calc(owner, bounds);
		}
		public override SizeF ContentSize {
			get { return (LinearGauge.Orientation == ScaleOrientation.Horizontal) ? new SizeF(250, 125) : new SizeF(125, 250); }
		}
		public override bool SmartLayout {
			get { return true; }
		}
		protected override CustomizeActionInfo[] GetActionsCore() {
			ArrayList list = new ArrayList(
				new CustomizeActionInfo[]{
					new CustomizeActionInfo("RunDesigner", "Run Circular Gauge Designer", "Run Designer", UIHelper.GaugeTypeImages[1]),
					new CustomizeActionInfo("ChangeStyle", "Show Style Chooser", "Change Style", UIHelper.ChangeStyleImage),
					new CustomizeActionInfo("AddScale", "Add default Scale", "Add Scale", UIHelper.LinearGaugeElementImages[0],"Scales"),
					new CustomizeActionInfo("AddBackgroundLayer", "Add default Background Layer", "Add Background Layer", UIHelper.LinearGaugeElementImages[1],"Layers"),
					new CustomizeActionInfo("AddLevel", "Add default Level pointer", "Add Level", UIHelper.LinearGaugeElementImages[2],"Pointers"),
					new CustomizeActionInfo("AddRangeBar", "Add default RangeBar pointer", "Add RangeBar", UIHelper.LinearGaugeElementImages[3],"Pointers"),
					new CustomizeActionInfo("AddMarker", "Add default Marker pointer", "Add Marker", UIHelper.LinearGaugeElementImages[4],"Pointers"),
					new CustomizeActionInfo("AddEffectLayer", "Add default Effect Layer", "Add Effect Layer", UIHelper.LinearGaugeElementImages[5],"Layers"),
					new CustomizeActionInfo("AddStateIndicator", "Add default State Indicator", "Add State Indicator", UIHelper.LinearGaugeElementImages[6],"Other Elements"),
				});
			if(LinearGauge.Scales.Count == 0) {
				list.RemoveRange(2, 6);
				list.Add(defaultInitialization);
			}
			list.AddRange(base.GetActionsCore());
			return (CustomizeActionInfo[])list.ToArray(typeof(CustomizeActionInfo));
		}
	}
}
