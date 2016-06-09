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
using System.Web.UI;
using DevExpress.Utils.Serializing;
using DevExpress.Web.ASPxGauges.Base;
using DevExpress.Web.ASPxGauges.Data;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Layout;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
namespace DevExpress.Web.ASPxGauges.Gauges.Digital {
	public class DigitalGaugeProvider : BaseGaugeProviderWeb, IDigitalGauge {
		string textCore;
		float letterSpacingCore = 0;
		int digitCountCore = -1;
		TextSpacing paddingCore;
		DigitalGaugeDisplayMode displayModeCore;
		BaseShapeAppearance appearanceOnCore;
		BaseShapeAppearance appearanceOffCore;
		DigitalBackgroundLayerComponentCollection backgroundLayersCore;
		DigitalEffectLayerComponentCollection effectLayersCore;
		public DigitalGaugeProvider(DigitalGauge gauge, BaseGaugeChangedHandler handler)
			: base(gauge, handler) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.textCore = "00.000";
			this.paddingCore = new TextSpacing(20, 20, 20, 20);
			this.displayModeCore = DigitalGaugeDisplayMode.FourteenSegment;
			this.appearanceOnCore = new BaseShapeAppearance(BrushObject.Empty, new SolidBrushObject(Color.Black));
			this.appearanceOffCore = new BaseShapeAppearance(BrushObject.Empty, new SolidBrushObject(Color.Transparent));
			this.backgroundLayersCore = new DigitalBackgroundLayerComponentCollection();
			this.effectLayersCore = new DigitalEffectLayerComponentCollection();
			BackgroundLayers.CollectionChanged += OnBackgroundLayersCollectionChanged;
			EffectLayers.CollectionChanged += OnEffectLayersCollectionChanged;
		}
		protected override void OnDispose() {
			if(AppearanceOn != null) {
				AppearanceOn.Dispose();
				appearanceOnCore = null;
			}
			if(AppearanceOff != null) {
				AppearanceOff.Dispose();
				appearanceOffCore = null;
			}
			if(BackgroundLayers != null) {
				BackgroundLayers.CollectionChanged -= OnBackgroundLayersCollectionChanged;
				BackgroundLayers.Dispose();
				backgroundLayersCore = null;
			}
			if(EffectLayers != null) {
				EffectLayers.CollectionChanged -= OnEffectLayersCollectionChanged;
				EffectLayers.Dispose();
				effectLayersCore = null;
			}
			base.OnDispose();
		}
		void OnBackgroundLayersCollectionChanged(CollectionChangedEventArgs<DigitalBackgroundLayerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		void OnEffectLayersCollectionChanged(CollectionChangedEventArgs<DigitalEffectLayerComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		public DigitalBackgroundLayerComponentCollection BackgroundLayers {
			get { return backgroundLayersCore; }
		}
		public DigitalEffectLayerComponentCollection EffectLayers {
			get { return effectLayersCore; }
		}
		public override void BuildModel(BaseGaugeModel model) {
			base.BuildModel(model);
			model.Composite.AddRange(BackgroundLayers.ToArray());
			model.Composite.AddRange(EffectLayers.ToArray());
		}
		public string Text {
			get { return textCore; }
			set { textCore = value; }
		}
		public int DigitCount {
			get { return digitCountCore; }
			set { digitCountCore = value; }
		}
		public TextSpacing Padding {
			get { return paddingCore; }
			set { paddingCore = value; }
		}
		public float LetterSpacing {
			get { return letterSpacingCore; }
			set { letterSpacingCore = value; }
		}
		public DigitalGaugeDisplayMode DisplayMode {
			get { return displayModeCore; }
			set { displayModeCore = value; }
		}
		public BaseShapeAppearance AppearanceOn {
			get { return appearanceOnCore; }
		}
		public BaseShapeAppearance AppearanceOff {
			get { return appearanceOffCore; }
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.DigitalGaugeDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class DigitalGauge : BaseGaugeWeb, IDigitalGauge {
		protected override void OnCreate() {
			base.OnCreate();
			bindableProviderCore = CreateBindableProvider();
			AppearanceOn.Changed += OnAppearanceOnChanged;
			AppearanceOff.Changed += OnAppearanceOffChanged;
		}
		protected override void OnDispose() {
			if(bindableProviderCore != null) {
				bindableProviderCore.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		#region Databinding
		BaseBindableProvider bindableProviderCore;
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(null);
		}
		public void DataBind() {
			BindableProvider.DataBind();
		}
		public event EventHandler DataBinding {
			add { BindableProvider.DataBinding += value; }
			remove { BindableProvider.DataBinding += value; }
		}
		[Browsable(false), Bindable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control BindingContainer {
			get { return this.GaugeContainer as Control; }
		}
		#endregion Databinding
		protected override BaseGaugeProviderWeb CreateGaugeProvider() {
			return new DigitalGaugeProvider(this, OnComponentsChanged);
		}
		protected DigitalGaugeProvider DigitalGaugeProvider {
			get { return GaugeProvider as DigitalGaugeProvider; }
		}
		public override void ApplyTextSettings(XtraGauges.Core.TextSettings settings) {
			base.ApplyTextSettings(settings);
			Text = settings.Text;
		}
		protected override void ReadTextSettingsCore(XtraGauges.Core.TextSettings settings) {
			settings.Text = Text;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public DigitalBackgroundLayerComponentCollection BackgroundLayers {
			get { return DigitalGaugeProvider.BackgroundLayers; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public DigitalEffectLayerComponentCollection EffectLayers {
			get { return DigitalGaugeProvider.EffectLayers; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceOn {
			get { return DigitalGaugeProvider.AppearanceOn; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceOff {
			get { return DigitalGaugeProvider.AppearanceOff; }
		}
		void OnAppearanceOnChanged(object sender, EventArgs ea) {
			OnModelChanged(false);
		}
		void OnAppearanceOffChanged(object sender, EventArgs ea) {
			OnModelChanged(false);
		}
		[XtraSerializableProperty]
		public TextSpacing Padding {
			get { return DigitalGaugeProvider.Padding; }
			set {
				if(DigitalGaugeProvider.Padding == value) return;
				DigitalGaugeProvider.Padding = value;
				OnModelChanged(false);
			}
		}
		[Bindable(true)]
		[DefaultValue("0")]
		[XtraSerializableProperty]
		public string Text {
			get { return DigitalGaugeProvider.Text; }
			set {
				if(DigitalGaugeProvider.Text == value) return;
				DigitalGaugeProvider.Text = value;
				OnModelChanged(false);
			}
		}
		[DefaultValue(0f)]
		[XtraSerializableProperty]
		public float LetterSpacing {
			get { return DigitalGaugeProvider.LetterSpacing; }
			set {
				if(DigitalGaugeProvider.LetterSpacing == value) return;
				DigitalGaugeProvider.LetterSpacing = value;
				OnModelChanged(false);
			}
		}
		[DefaultValue(-1)]
		[XtraSerializableProperty]
		public int DigitCount {
			get { return DigitalGaugeProvider.DigitCount; }
			set {
				if(DigitalGaugeProvider.DigitCount == value) return;
				DigitalGaugeProvider.DigitCount = value;
				OnModelChanged(false);
			}
		}
		[DefaultValue(DigitalGaugeDisplayMode.FourteenSegment)]
		[XtraSerializableProperty]
		public DigitalGaugeDisplayMode DisplayMode {
			get { return DigitalGaugeProvider.DisplayMode; }
			set {
				if(DigitalGaugeProvider.DisplayMode == value) return;
				DigitalGaugeProvider.DisplayMode = value;
				OnModelChanged(true);
			}
		}
		protected override string GetPrefixType() { return "digital"; }
		protected override void InitializeDefaultCore() {
			AddDefaultElements();
		}
		public void AddDefaultElements() {
			BeginUpdate();
			AddBackgroundLayer();
			this.AppearanceOn.ContentBrush = new SolidBrushObject(Color.White);
			EndUpdate();
		}
		protected override void InitializeLabelDefault(DevExpress.XtraGauges.Core.Model.Label label) {
			label.BeginUpdate();
			base.InitializeLabelDefault(label);
			label.Position = new PointF2D(125, 50);
			label.EndUpdate();
		}
		public DigitalBackgroundLayerComponent AddBackgroundLayer() {
			BeginUpdate();
			string[] names = new string[BackgroundLayers.Count];
			int i = 0;
			BackgroundLayers.Accept(
					delegate(DigitalBackgroundLayerComponent l) { names[i++] = l.Name; }
				);
			DigitalBackgroundLayerComponent layer = new DigitalBackgroundLayerComponent(UniqueNameHelper.GetUniqueName(Prefix("BackgroundLayer"), names, BackgroundLayers.Count + 1));
			InitializeBackgroundLayerDefault(layer);
			BackgroundLayers.Add(layer);
			AddComponentToDesignTimeSurface(layer);
			EndUpdate();
			return layer;
		}
		public DigitalEffectLayerComponent AddEffectLayer() {
			BeginUpdate();
			string[] names = new string[EffectLayers.Count];
			int i = 0;
			EffectLayers.Accept(
					delegate(DigitalEffectLayerComponent l) { names[i++] = l.Name; }
				);
			DigitalEffectLayerComponent layer = new DigitalEffectLayerComponent(UniqueNameHelper.GetUniqueName(Prefix("EffectLayer"), names, EffectLayers.Count + 1));
			InitializeEffectLayerDefault(layer);
			EffectLayers.Add(layer);
			AddComponentToDesignTimeSurface(layer);
			EndUpdate();
			return layer;
		}
		protected void InitializeBackgroundLayerDefault(DigitalBackgroundLayer backgroundLayer) {
			DigitalGaugeExtention.InitializeBackgroundLayerDefault(backgroundLayer, new PointF2D(Bounds.Width, Bounds.Height));
		}
		protected void InitializeEffectLayerDefault(DigitalEffectLayer effectLayer) {
			DigitalGaugeExtention.InitializeEffectLayerDefault(effectLayer, new PointF2D(Bounds.Width, Bounds.Height));
		}
		protected override void ClearCore() {
			base.ClearCore();
			BackgroundLayers.Clear();
			EffectLayers.Clear();
		}
		protected override PreferredLayoutType CalcPreferredLayoutType() {
			return PreferredLayoutType.Bottom;
		}
		protected override BaseGaugeModel CreateModel() {
			BaseGaugeModel model = null;
			switch(DisplayMode) {
				case DigitalGaugeDisplayMode.FourteenSegment: model = new DigitalGaugeModel_S14(this); break;
				case DigitalGaugeDisplayMode.SevenSegment: model = new DigitalGaugeModel_S7(this); break;
				case DigitalGaugeDisplayMode.Matrix5x8: model = new DigitalGaugeModel_M5x8(this); break;
				case DigitalGaugeDisplayMode.Matrix8x14: model = new DigitalGaugeModel_M8x14(this); break;
			}
			DigitalGaugeProvider.BuildModel(model);
			return model;
		}
		protected override void SetEnabledCore(bool enabled) {
			BeginUpdate();
			ComponentCollectionExtention.SetEnabled(Labels, enabled);
			ComponentCollectionExtention.SetEnabled(BackgroundLayers, enabled);
			ComponentCollectionExtention.SetEnabled(EffectLayers, enabled);
			EndUpdate();
		}
		protected sealed override List<string> GetNamesCore() {
			List<string> names = new List<string>(base.GetNamesCore());
			ComponentCollectionExtention.CollectNames(BackgroundLayers, names);
			ComponentCollectionExtention.CollectNames(EffectLayers, names);
			return names;
		}
		protected sealed override void AddGaugeElementToComponentCollection(IComponent component) {
			if(ComponentCollectionExtention.TryAddComponent(Labels, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(BackgroundLayers, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(EffectLayers, component)) return;
		}
		protected sealed override BaseElement<IRenderableElement> DuplicateGaugeElementCore(BaseElement<IRenderableElement> component) {
			IComponent duplicate;
			if(ComponentCollectionExtention.TryDuplicateComponent(Labels, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(BackgroundLayers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(EffectLayers, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			return duplicate as BaseElement<IRenderableElement>;
		}
		protected internal override void CheckElementsAffinity() { }
		protected override IStateManagedHierarchyObject[] GetStateManagedHierarchyObjects() {
			List<IStateManagedHierarchyObject> objects = new List<IStateManagedHierarchyObject>(base.GetStateManagedHierarchyObjects());
			objects.Add(BackgroundLayers);
			objects.Add(EffectLayers);
			return objects.ToArray();
		}
		protected override List<ISerizalizeableElement> GetChildernCore() {
			List<ISerizalizeableElement> list = new List<ISerizalizeableElement>(base.GetChildernCore());
			CollectChildren(list, BackgroundLayers, "BackgroundLayers");
			CollectChildren(list, EffectLayers, "EffectLayers");
			return list;
		}
	}
}
