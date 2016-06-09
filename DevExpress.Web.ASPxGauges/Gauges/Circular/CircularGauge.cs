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
using System.Web.UI;
using DevExpress.Web.ASPxGauges.Base;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Resources;
namespace DevExpress.Web.ASPxGauges.Gauges.Circular {
	public class CircularGaugeProvider : BaseGaugeProviderWeb {
		ArcScaleComponentCollection scalesCore;
		ArcScaleBackgroundLayerComponentCollection backgroundLayersCore;
		ArcScaleNeedleComponentCollection needlesCore;
		ArcScaleRangeBarComponentCollection rangeBarsCore;
		ArcScaleMarkerComponentCollection markersCore;
		ArcScaleSpindleCapComponentCollection spindleCapsCore;
		ArcScaleEffectLayerComponentCollection effectsCore;
		ArcScaleStateIndicatorComponentCollection indicatorsCore;
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
			Scales.CollectionChanged += OnScalesCollectionChanged;
			BackgroundLayers.CollectionChanged += OnBackgroundLayersCollectionChanged;
			Needles.CollectionChanged += OnNeedlesCollectionChanged;
			SpindleCaps.CollectionChanged += OnSpindleCapsCollectionChanged;
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
			base.OnDispose();
		}
		void OnScalesCollectionChanged(CollectionChangedEventArgs<ArcScaleComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnBackgroundLayersCollectionChanged(CollectionChangedEventArgs<ArcScaleBackgroundLayerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnNeedlesCollectionChanged(CollectionChangedEventArgs<ArcScaleNeedleComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnSpindleCapsCollectionChanged(CollectionChangedEventArgs<ArcScaleSpindleCapComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnMarkersCollectionChanged(CollectionChangedEventArgs<ArcScaleMarkerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnRangeBarsCollectionChanged(CollectionChangedEventArgs<ArcScaleRangeBarComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnEffectsCollectionChanged(CollectionChangedEventArgs<ArcScaleEffectLayerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnIndicatorsCollectionChanged(CollectionChangedEventArgs<ArcScaleStateIndicatorComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		public ArcScaleComponentCollection Scales {
			get { return scalesCore; }
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
			model.Composite.AddRange(Scales.ToArray());
			ScaleAffinityHelper.ResolveArcScaleAffinity(BackgroundLayers, Scales);
			ScaleAffinityHelper.ResolveArcScaleAffinity(EffectLayers, Scales);
			ScaleAffinityHelper.ResolveArcScaleAffinity(Needles, Scales);
			ScaleAffinityHelper.ResolveArcScaleAffinity(Markers, Scales);
			ScaleAffinityHelper.ResolveArcScaleAffinity(RangeBars, Scales);
			ScaleAffinityHelper.ResolveArcScaleAffinity(SpindleCaps, Scales);
			ScaleAffinityHelper.ResolveIndicatorArcScaleAffinity(Indicators, Scales);
			model.Composite.AddRange(BackgroundLayers.ToArray());
			model.Composite.AddRange(EffectLayers.ToArray());
			model.Composite.AddRange(Needles.ToArray());
			model.Composite.AddRange(Markers.ToArray());
			model.Composite.AddRange(RangeBars.ToArray());
			model.Composite.AddRange(SpindleCaps.ToArray());
			model.Composite.AddRange(Indicators.ToArray());
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.CircularGaugeDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class CircularGauge : BaseGaugeWeb, ICircularGauge {
		protected override List<string> GetNamesCore() {
			List<string> names = new List<string>(base.GetNamesCore());
			ComponentCollectionExtention.CollectNames(Scales, names);
			ComponentCollectionExtention.CollectNames(BackgroundLayers, names);
			ComponentCollectionExtention.CollectNames(EffectLayers, names);
			ComponentCollectionExtention.CollectNames(Needles, names);
			ComponentCollectionExtention.CollectNames(RangeBars, names);
			ComponentCollectionExtention.CollectNames(Markers, names);
			ComponentCollectionExtention.CollectNames(SpindleCaps, names);
			ComponentCollectionExtention.CollectNames(Indicators, names);
			return names;
		}
		protected sealed internal override void CheckElementsAffinity() {
			ScaleAffinityHelper.CheckScaleID(BackgroundLayers);
			ScaleAffinityHelper.CheckScaleID(EffectLayers);
			ScaleAffinityHelper.CheckScaleID(Needles);
			ScaleAffinityHelper.CheckScaleID(Markers);
			ScaleAffinityHelper.CheckScaleID(RangeBars);
			ScaleAffinityHelper.CheckScaleID(SpindleCaps);
			ScaleAffinityHelper.CheckScaleID(EffectLayers);
			ScaleAffinityHelper.CheckScaleID(Indicators);
		}
		protected sealed override void AddGaugeElementToComponentCollection(IComponent component) {
			if(ComponentCollectionExtention.TryAddComponent(Labels, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Scales, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(BackgroundLayers, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(EffectLayers, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Needles, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Markers, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(RangeBars, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(SpindleCaps, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Indicators, component)) return;
		}
		protected sealed override BaseElement<IRenderableElement> DuplicateGaugeElementCore(BaseElement<IRenderableElement> component) {
			IComponent duplicate = null;
			if(ComponentCollectionExtention.TryDuplicateComponent(Labels, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Scales, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(BackgroundLayers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(EffectLayers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Needles, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(RangeBars, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Markers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(SpindleCaps, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Indicators, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			return duplicate as BaseElement<IRenderableElement>;
		}
		protected override void SetEnabledCore(bool enabled) {
			ComponentCollectionExtention.SetEnabled(Labels, enabled);
			ComponentCollectionExtention.SetEnabled(Scales, enabled);
			ComponentCollectionExtention.SetEnabled(BackgroundLayers, enabled);
			ComponentCollectionExtention.SetEnabled(EffectLayers, enabled);
			ComponentCollectionExtention.SetEnabled(Needles, enabled);
			ComponentCollectionExtention.SetEnabled(Markers, enabled);
			ComponentCollectionExtention.SetEnabled(RangeBars, enabled);
			ComponentCollectionExtention.SetEnabled(SpindleCaps, enabled);
			ComponentCollectionExtention.SetEnabled(Indicators, enabled);
		}
		protected override BaseGaugeProviderWeb CreateGaugeProvider() {
			return new CircularGaugeProvider(this, OnComponentsChanged);
		}
		protected CircularGaugeProvider CircularGaugeProvider {
			get { return GaugeProvider as CircularGaugeProvider; }
		}
		protected override BaseGaugeModel CreateModel() {
			CircularGaugeModel model = new CircularGaugeModel(this);
			CircularGaugeProvider.BuildModel(model);
			return model;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ArcScaleComponentCollection Scales {
			get { return CircularGaugeProvider.Scales; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ArcScaleBackgroundLayerComponentCollection BackgroundLayers {
			get { return CircularGaugeProvider.BackgroundLayers; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ArcScaleEffectLayerComponentCollection EffectLayers {
			get { return CircularGaugeProvider.EffectLayers; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ArcScaleNeedleComponentCollection Needles {
			get { return CircularGaugeProvider.Needles; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ArcScaleMarkerComponentCollection Markers {
			get { return CircularGaugeProvider.Markers; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ArcScaleRangeBarComponentCollection RangeBars {
			get { return CircularGaugeProvider.RangeBars; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ArcScaleSpindleCapComponentCollection SpindleCaps {
			get { return CircularGaugeProvider.SpindleCaps; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ArcScaleStateIndicatorComponentCollection Indicators {
			get { return CircularGaugeProvider.Indicators; }
		}
		[DefaultValue(DevExpress.Utils.DefaultBoolean.Default), Category("Layout")]
		public DevExpress.Utils.DefaultBoolean AutoSize {
			get { return base.AutoSizeByActualBounds; }
			set { base.AutoSizeByActualBounds = value; }
		}
		protected sealed override string GetPrefixType() { return "circular"; }
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
		public ArcScaleEffectLayerComponent AddEffectLayer() {
			BeginUpdate();
			string[] names = new string[EffectLayers.Count];
			int i = 0;
			EffectLayers.Accept(
					delegate(ArcScaleEffectLayerComponent e) { names[i++] = e.Name; }
				);
			ArcScaleEffectLayerComponent layer = new ArcScaleEffectLayerComponent(UniqueNameHelper.GetUniqueName(Prefix("EffectLayer"), names, EffectLayers.Count + 1));
			InitializeEffectLayerDefault(layer);
			EffectLayers.Add(layer);
			AddComponentToDesignTimeSurface(layer);
			EndUpdate();
			return layer;
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
		protected void InitializeScaleDefault(ArcScale scale) {
			CircularGaugeExtention.InitializeScaleDefault(
					scale, new PointF2D(Model.ContentSize.Width / 2, Model.ContentSize.Height / 2)
				);
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
		protected override void ClearCore() {
			base.ClearCore();
			Scales.Clear();
			BackgroundLayers.Clear();
			EffectLayers.Clear();
			Needles.Clear();
			Markers.Clear();
			RangeBars.Clear();
			SpindleCaps.Clear();
			Indicators.Clear();
		}
		protected override IStateManagedHierarchyObject[] GetStateManagedHierarchyObjects() {
			List<IStateManagedHierarchyObject> objects = new List<IStateManagedHierarchyObject>(base.GetStateManagedHierarchyObjects());
			objects.Add(Scales);
			objects.Add(BackgroundLayers);
			objects.Add(EffectLayers);
			objects.Add(Needles);
			objects.Add(Markers);
			objects.Add(RangeBars);
			objects.Add(SpindleCaps);
			objects.Add(Indicators);
			return objects.ToArray();
		}
		protected override List<ISerizalizeableElement> GetChildernCore() {
			List<ISerizalizeableElement> list = new List<ISerizalizeableElement>(base.GetChildernCore());
			CollectChildren(list, Scales, "Scales");
			CollectChildren(list, BackgroundLayers, "BackgroundLayers");
			CollectChildren(list, EffectLayers, "EffectLayers");
			CollectChildren(list, Needles, "Needles");
			CollectChildren(list, Markers, "Markers");
			CollectChildren(list, RangeBars, "RangeBars");
			CollectChildren(list, SpindleCaps, "SpindleCaps");
			CollectChildren(list, Indicators, "Indicators");
			return list;
		}
	}
	public class CircularGaugeModel : BaseGaugeModel {
		protected CircularGauge CircilarGauge {
			get { return Owner as CircularGauge; }
		}
		public CircularGaugeModel(BaseGaugeWeb gauge) : base(gauge) { }
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
			ArrayList list = new ArrayList(GetActions());
			if(CircilarGauge.Scales.Count == 0) {
				list.RemoveRange(2, 7);
				list.Add(defaultInitialization);
			}
			list.AddRange(base.GetActionsCore());
			return (CustomizeActionInfo[])list.ToArray(typeof(CustomizeActionInfo));
		}
		CustomizeActionInfo[] actions;
		CustomizeActionInfo[] GetActions() {
			if(actions == null)
				actions = new CustomizeActionInfo[]{
					new CustomizeActionInfo("RunDesigner", "Run Circular Gauge Designer", "Run Designer", UIHelper.GaugeTypeImages[0]),
					new CustomizeActionInfo("AddScale", "Add default Scale", "Add Scale", UIHelper.CircularGaugeElementImages[0],"Scales"),
					new CustomizeActionInfo("AddBackgroundLayer", "Add default Background Layer", "Add Background Layer", UIHelper.CircularGaugeElementImages[1],"Layers"),
					new CustomizeActionInfo("AddNeedle", "Add default Needle pointer", "Add Needle", UIHelper.CircularGaugeElementImages[2],"Pointers"),
					new CustomizeActionInfo("AddRangeBar", "Add default RangeBar pointer", "Add RangeBar", UIHelper.CircularGaugeElementImages[3],"Pointers"),
					new CustomizeActionInfo("AddMarker", "Add default Marker pointer", "Add Marker", UIHelper.CircularGaugeElementImages[4],"Pointers"),
					new CustomizeActionInfo("AddSpindleCap", "Add default Spindle Cap", "Add Spindle Cap", UIHelper.CircularGaugeElementImages[5],"Layers"),
					new CustomizeActionInfo("AddEffectLayer", "Add default Effect Layer", "Add Effect Layer", UIHelper.CircularGaugeElementImages[6],"Layers"),
					new CustomizeActionInfo("AddStateIndicator", "Add default State Indicator", "Add State Indicator", UIHelper.CircularGaugeElementImages[7],"Other Elements")
				};
			return actions;
		}
	}
}
