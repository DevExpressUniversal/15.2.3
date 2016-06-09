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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Win.Customization;
using DevExpress.XtraGauges.Win.Data;
using DevExpress.XtraGauges.Win.Wizard;
namespace DevExpress.XtraGauges.Win.Gauges.Circular {
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.ArcScaleComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleComponent : ArcScale, ISupportInitialize, IBindableComponent, ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<ArcScaleComponent> {
		BaseBindableProvider bindableProviderCore;
		public ArcScaleComponent()
			: base() {
		}
		public ArcScaleComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new SelectionFrame(this),
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get { return GetCustomizationBounds(); }
			set { }
		}
		public override Color GetLabelShemeColor() {
			IGauge owner = GetOwner();
			if(owner != null && (owner.GetColorScheme().TargetElements & TargetElement.Label) != 0) {
				ColorScheme scheme = owner.GetColorScheme();
				return scheme.Color;
			}
			return Color.Empty;
		}
		IGauge GetOwner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model != null)
				return model.Owner;
			return null;
		}
		protected override BaseTextAppearance GetDefaultAppearance() {
			FrozenTextAppearance result = new FrozenTextAppearance();
			IGauge owner = GetOwner();
			if(owner != null && owner.Container is GaugeControlBase) {
				GaugeControlBase gaugeControl = owner.Container as GaugeControlBase;
				AppearanceDefault defaultAppearanceText = gaugeControl.DefaultAppearanceText;
				if(defaultAppearanceText != null)
					ShapeAppearanceHelper.Init(result, defaultAppearanceText);
			}
			return result;
		}
		protected override void UpdateComponent() { Update(); }
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open Scale Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[0])
			};
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(ArcScaleComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, this);
		}
		void ISupportAssign<ArcScaleComponent>.Assign(ArcScaleComponent arcScale) {
			Assign(arcScale);
		}
		bool ISupportAssign<ArcScaleComponent>.IsDifferFrom(ArcScaleComponent arcScale) {
			return IsDifferFrom(arcScale) ||
				AppearanceScale.IsDifferFrom(arcScale.AppearanceScale) ||
				AppearanceMinorTickmark.IsDifferFrom(arcScale.AppearanceMinorTickmark) ||
				AppearanceMajorTickmark.IsDifferFrom(arcScale.AppearanceMajorTickmark) ||
				AppearanceTickmarkTextBackground.IsDifferFrom(arcScale.AppearanceTickmarkTextBackground) ||
				AppearanceTickmarkText.IsDifferFrom(arcScale.AppearanceTickmarkText);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<ArcScaleComponent>(1,"Scales", UIHelper.CircularGaugeElementImages[0],new ArcScaleComponent[]{this},model.Owner) 
				};
				designerform.ShowDialog();
			}
		}
		bool ShouldSerializeAppearanceScale() { return AppearanceScale.ShouldSerialize(); }
		void ResetAppearanceScale() { AppearanceScale.Reset(); }
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentAppearanceScale"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseScaleAppearance AppearanceScale {
			get { return Appearance; }
		}
		bool ShouldSerializeAppearanceMinorTickmark() { return AppearanceMinorTickmark.ShouldSerialize(); }
		void ResetAppearanceMinorTickmark() { AppearanceMinorTickmark.Reset(); }
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentAppearanceMinorTickmark"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceMinorTickmark {
			get { return MinorTickmark.Shape.Appearance; }
		}
		bool ShouldSerializeAppearanceMajorTickmark() { return AppearanceMajorTickmark.ShouldSerialize(); }
		void ResetAppearanceMajorTickmark() { AppearanceMajorTickmark.Reset(); }
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentAppearanceMajorTickmark"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceMajorTickmark {
			get { return MajorTickmark.Shape.Appearance; }
		}
		bool ShouldSerializeAppearanceTickmarkTextBackground() { return AppearanceTickmarkTextBackground.ShouldSerialize(); }
		void ResetAppearanceTickmarkTextBackground() { AppearanceTickmarkTextBackground.Reset(); }
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentAppearanceTickmarkTextBackground"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceTickmarkTextBackground {
			get { return MajorTickmark.TextShape.Appearance; }
		}
		bool ShouldSerializeAppearanceTickmarkText() { return AppearanceTickmarkText.ShouldSerialize(); }
		void ResetAppearanceTickmarkText() { AppearanceTickmarkText.Reset(); }
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentAppearanceTickmarkText"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseTextAppearance AppearanceTickmarkText {
			get { return MajorTickmark.TextShape.AppearanceText; }
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
#if !SL
	[DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentIsAnimating")]
#endif
		public override bool IsAnimating {
			get { return base.IsAnimating; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentEasingMode"),
#endif
 DefaultValue(EasingMode.EaseIn)]
		public EasingMode EasingMode {
			get { return Provider.EasingMode; }
			set { Provider.EasingMode = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentEasingFunction"),
#endif
 DefaultValue(null)]
		public IEasingFunction EasingFunction {
			get { return Provider.EasingFunction; }
			set { Provider.EasingFunction = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentEnableAnimation"),
#endif
 DefaultValue(false)]
		public bool EnableAnimation {
			get { return Provider.EnableAnimation; }
			set { Provider.EnableAnimation = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentFrameCount"),
#endif
 DefaultValue(1000)]
		public int FrameCount {
			get { return Provider.FrameCount; }
			set { Provider.FrameCount = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleComponentFrameDelay"),
#endif
 DefaultValue(10000)]
		public int FrameDelay {
			get { return Provider.FrameDelay; }
			set { Provider.FrameDelay = value; }
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.ArcScaleBackgroundLayerComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleBackgroundLayerComponent : ArcScaleBackgroundLayer, ISupportInitialize, IBindableComponent, ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<ArcScaleBackgroundLayerComponent> {
		BaseBindableProvider bindableProviderCore;
		public ArcScaleBackgroundLayerComponent()
			: base() {
		}
		public ArcScaleBackgroundLayerComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(ArcScaleBackgroundLayerComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, ArcScale);
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new SelectionFrame(this),
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get { return Rectangle.Round(Info.RelativeBoundBox); }
			set { }
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open Background Layer Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[1])
			};
		}
		void ISupportAssign<ArcScaleBackgroundLayerComponent>.Assign(ArcScaleBackgroundLayerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleBackgroundLayerComponent>.IsDifferFrom(ArcScaleBackgroundLayerComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<ArcScaleBackgroundLayerComponent>(2,"Background Layers",UIHelper.CircularGaugeElementImages[1],new ArcScaleBackgroundLayerComponent[]{this},model.Owner)  
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleBackgroundLayerComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.ArcScaleNeedleComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleNeedleComponent : ArcScaleNeedle, ISupportInitialize, IBindableComponent, ISupportVisualDesigning, ICustomizationFrameClient, ISupportAssign<ArcScaleNeedleComponent> {
		BaseBindableProvider bindableProviderCore;
		public ArcScaleNeedleComponent()
			: base() {
		}
		public ArcScaleNeedleComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(ArcScaleNeedleComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, ArcScale);
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new SelectionFrame(this),
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get { return Rectangle.Round(Info.RelativeBoundBox); }
			set { }
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open Needle Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[2])
			};
		}
		void ISupportAssign<ArcScaleNeedleComponent>.Assign(ArcScaleNeedleComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleNeedleComponent>.IsDifferFrom(ArcScaleNeedleComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<ArcScaleNeedleComponent>(3,"Needles",UIHelper.CircularGaugeElementImages[2],new ArcScaleNeedleComponent[]{this},model.Owner),  
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleNeedleComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.ArcScaleSpindleCapComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleSpindleCapComponent : ArcScaleSpindleCap, ISupportInitialize, IBindableComponent, ISupportVisualDesigning, ICustomizationFrameClient, ISupportAssign<ArcScaleSpindleCapComponent> {
		BaseBindableProvider bindableProviderCore;
		public ArcScaleSpindleCapComponent()
			: base() {
		}
		public ArcScaleSpindleCapComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(ArcScaleSpindleCapComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, ArcScale);
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new SelectionFrame(this),
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get { return Rectangle.Round(Info.RelativeBoundBox); }
			set { }
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open SpindleCap Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[5])
			};
		}
		void ISupportAssign<ArcScaleSpindleCapComponent>.Assign(ArcScaleSpindleCapComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleSpindleCapComponent>.IsDifferFrom(ArcScaleSpindleCapComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<ArcScaleSpindleCapComponent>(6,"SpindleCaps",UIHelper.CircularGaugeElementImages[5],new ArcScaleSpindleCapComponent[]{ this },model.Owner)
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleSpindleCapComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.ArcScaleMarkerComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleMarkerComponent : ArcScaleMarker, ISupportInitialize, IBindableComponent, ISupportVisualDesigning, ICustomizationFrameClient, ISupportAssign<ArcScaleMarkerComponent> {
		BaseBindableProvider bindableProviderCore;
		public ArcScaleMarkerComponent()
			: base() {
		}
		public ArcScaleMarkerComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(ArcScaleMarkerComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, ArcScale);
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new SelectionFrame(this),
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get { return Rectangle.Round(Info.RelativeBoundBox); }
			set { }
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open Marker Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[4])
			};
		}
		void ISupportAssign<ArcScaleMarkerComponent>.Assign(ArcScaleMarkerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleMarkerComponent>.IsDifferFrom(ArcScaleMarkerComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<ArcScaleMarkerComponent>(5,"Markers",UIHelper.CircularGaugeElementImages[4],new ArcScaleMarkerComponent[]{this},model.Owner) , 
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleMarkerComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.ArcScaleRangeBarComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleRangeBarComponent : ArcScaleRangeBar, ISupportInitialize, IBindableComponent, ISupportVisualDesigning, ICustomizationFrameClient, ISupportAssign<ArcScaleRangeBarComponent> {
		BaseBindableProvider bindableProviderCore;
		public ArcScaleRangeBarComponent()
			: base() {
		}
		public ArcScaleRangeBarComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
			AppearanceRangeBar.Changed += OnAppearanceRangeBarChanged;
		}
		void OnAppearanceRangeBarChanged(object sender, EventArgs e) {
			Update();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			AppearanceRangeBar.Changed -= OnAppearanceRangeBarChanged;
			base.OnDispose();
		}
		protected RangeBarAppearance GetActualAppearance() {
			FrozenRangeBarAppearance target = new FrozenRangeBarAppearance();
			ShapeAppearanceHelper.Combine(target, AppearanceRangeBar, GetDefaultAppearance());
			return target;
		}
		FrozenRangeBarAppearance GetDefaultAppearance() {
			FrozenRangeBarAppearance result = new FrozenRangeBarAppearance();
			IGauge owner = GetOwner();
			if(owner != null && owner.Container is GaugeControlBase) {
				AppearanceDefault defaultAppearanceRangeBar = (owner.Container as GaugeControlBase).DefaultAppearanceRangeBar;
				ShapeAppearanceHelper.Init(result, defaultAppearanceRangeBar);
				ColorScheme scheme = owner.GetColorScheme();
				if((scheme.TargetElements & TargetElement.RangeBar) != 0 && (scheme.Color != Color.Empty)) {
					result.ContentBrush = BrushesCache.GetBrushByColor(scheme.Color);
				}
			}
			return result;
		}
		protected override void ShapeProcessing(BaseShape shape) {
			(shape as SectorShape).RoundedCaps = RoundedCaps;
			(shape as SectorShape).ShowBackground = ShowBackground;
			IGauge owner = GetOwner();
			if(owner != null) {
				ColorScheme scheme = owner.GetColorScheme();
				(shape as SectorShape).Appearance.Assign(GetActualAppearance());
				(shape as SectorShape).AppearanceRangeBar.Assign(GetActualAppearance());
			}
		}
		IGauge GetOwner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model != null)
				return model.Owner;
			return null;
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float StartAngle {
			get { return ArcScale != null ? ArcScale.StartAngle : 0; }
			set {
				if(ArcScale != null)
					ArcScale.StartAngle = value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float EndAngle {
			get { return ArcScale != null ? ArcScale.EndAngle : 0; }
			set {
				if(ArcScale != null)
					ArcScale.EndAngle = value;
			}
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(ArcScaleRangeBarComponentWrapper); }
		}
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, ArcScale);
		}
		void ISupportVisualDesigning.OnInitDesigner() {
			if(Scale.Value == AnchorValue) LockValue(Scale.MinValue + Scale.ScaleLength / 2f);
		}
		void ISupportVisualDesigning.OnCloseDesigner() {
			UnlockValue();
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new SelectionFrame(this),
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get { return Rectangle.Round(Info.RelativeBoundBox); }
			set { }
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open RangeBar Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[3])
			};
		}
		void ISupportAssign<ArcScaleRangeBarComponent>.Assign(ArcScaleRangeBarComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleRangeBarComponent>.IsDifferFrom(ArcScaleRangeBarComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<ArcScaleRangeBarComponent>(4,"RangeBars",UIHelper.CircularGaugeElementImages[3],new ArcScaleRangeBarComponent[]{this},model.Owner),  
				};
				designerform.ShowDialog();
			}
		}
		bool ShouldSerializeAppearanceRangeBar() { return AppearanceRangeBar.ShouldSerialize(); }
		void ResetAppearanceRangeBar() { AppearanceRangeBar.Reset(); }
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleRangeBarComponentAppearanceRangeBar"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public RangeBarAppearance AppearanceRangeBar {
			get { return Appearance; }
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleRangeBarComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.ArcScaleEffectLayerComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleEffectLayerComponent : ArcScaleEffectLayer, ISupportInitialize, IBindableComponent, ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<ArcScaleEffectLayerComponent> {
		BaseBindableProvider bindableProviderCore;
		public ArcScaleEffectLayerComponent()
			: base() {
		}
		public ArcScaleEffectLayerComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(ArcScaleEffectLayerComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, ArcScale);
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new SelectionFrame(this),
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get { return Rectangle.Round(Info.RelativeBoundBox); }
			set { }
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open Effect Layer Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[6])
			};
		}
		void ISupportAssign<ArcScaleEffectLayerComponent>.Assign(ArcScaleEffectLayerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleEffectLayerComponent>.IsDifferFrom(ArcScaleEffectLayerComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<ArcScaleEffectLayerComponent>(7,"Effect Layers",UIHelper.CircularGaugeElementImages[6],new ArcScaleEffectLayerComponent[]{this},model.Owner)  
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleEffectLayerComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.ArcScaleStateIndicatorComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleStateIndicatorComponent : ScaleStateIndicator, ISupportInitialize, IBindableComponent,
		ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<ArcScaleStateIndicatorComponent> {
		BaseBindableProvider bindableProviderCore;
		public ArcScaleStateIndicatorComponent()
			: base() {
		}
		public ArcScaleStateIndicatorComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
		}
		protected override void OnDispose() {
			if(BindableProvider != null) {
				BindableProvider.Dispose();
				bindableProviderCore = null;
			}
			base.OnDispose();
		}
		public void BeginInit() {
			BeginUpdate();
		}
		public void EndInit() {
			EndUpdate();
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(ArcScaleStateIndicatorComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, Scale as IArcScale);
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new MoveFrame(this),
				new SelectionFrame(this),
				new ActionListFrame(this)
			};
			return customizeFrames;
		}
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get {
				PointF[] pts = MathHelper.ModelPointsToPoints(this.Parent, new PointF[] { 
						new PointF(Center.X - Size.Width * 0.5f, Center.Y - Size.Height * 0.5f),
						new PointF(Center.X + Size.Width * 0.5f, Center.Y + Size.Height * 0.5f)
						}
					);
				return new Rectangle(Point.Round(pts[0]), new Size((int)(pts[1].X - pts[0].X), (int)(pts[1].Y - pts[0].Y)));
			}
			set {
				PointF pt = MathHelper.PointToModelPoint(this.Parent, MathHelper.GetCenterOfRect(value));
				this.Center = new PointF2D((float)Math.Round((double)pt.X, 1), (float)Math.Round((double)pt.Y, 1));
			}
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open State Indicator Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[7])
			};
		}
		void ISupportAssign<ArcScaleStateIndicatorComponent>.Assign(ArcScaleStateIndicatorComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleStateIndicatorComponent>.IsDifferFrom(ArcScaleStateIndicatorComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<ArcScaleStateIndicatorComponent>(8,"Indicators",UIHelper.CircularGaugeElementImages[7],new ArcScaleStateIndicatorComponent[]{this} ,model.Owner)  
				};
				designerform.ShowDialog();
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		protected BaseBindableProvider BindableProvider {
			get { return bindableProviderCore; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("ArcScaleStateIndicatorComponentDataBindings"),
#endif
ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
	}
}
