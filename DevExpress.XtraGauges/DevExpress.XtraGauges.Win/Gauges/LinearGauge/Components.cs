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
namespace DevExpress.XtraGauges.Win.Gauges.Linear {
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.LinearScaleComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleComponent : LinearScale, ISupportInitialize, IBindableComponent, ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<LinearScaleComponent> {
		BaseBindableProvider bindableProviderCore;
		public LinearScaleComponent()
			: base() {
		}
		public LinearScaleComponent(string name)
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
			get { return Rectangle.Round(Info.RelativeBoundBox); }
			set { }
		}
		BaseGaugeModel GetParent() {
			if (Parent != null && Parent.Parent != null) 
				return (this.Parent.Parent as BaseGaugeModel);
			return null;
		}
		public override Color GetLabelShemeColor() {
			IGauge owner = GetOwner();
			if(owner != null) {
				ColorScheme scheme = owner.GetColorScheme();
				if((scheme.TargetElements & TargetElement.Label) != 0)
				return scheme.Color;
			}
			return Color.Empty;
		}
		IGauge GetOwner() {
			if(GetParent() != null)
				return GetParent().Owner;
			return null;
		}
		protected override BaseTextAppearance GetDefaultAppearance() {
			FrozenTextAppearance result = new FrozenTextAppearance();
			IGauge owner = GetOwner();
			if(owner != null && owner.Container is GaugeControlBase) {
				AppearanceDefault defaultAppearanceText = (owner.Container as GaugeControlBase).DefaultAppearanceText;
				if(defaultAppearanceText != null)
					ShapeAppearanceHelper.Init(result, defaultAppearanceText);
			}
			return result;
		}
		protected override void UpdateComponent() {
			Update();
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() {
			return new CustomizeActionInfo[]{
				new CustomizeActionInfo("RunDesigner", "Open Scale Designer Page", "Run Designer", UIHelper.LinearGaugeElementImages[0])
			};
		}
		Type ISupportPropertyGridWrapper.PropertyGridWrapperType {
			get { return typeof(LinearScaleComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, this);
		}
		void ISupportAssign<LinearScaleComponent>.Assign(LinearScaleComponent linearScale) {
			Assign(linearScale);
		}
		bool ISupportAssign<LinearScaleComponent>.IsDifferFrom(LinearScaleComponent linearScale) {
			return IsDifferFrom(linearScale) ||
				AppearanceScale.IsDifferFrom(linearScale.AppearanceScale) ||
				AppearanceMinorTickmark.IsDifferFrom(linearScale.AppearanceMinorTickmark) ||
				AppearanceMajorTickmark.IsDifferFrom(linearScale.AppearanceMajorTickmark) ||
				AppearanceTickmarkTextBackground.IsDifferFrom(linearScale.AppearanceTickmarkTextBackground) ||
				AppearanceTickmarkText.IsDifferFrom(linearScale.AppearanceTickmarkText);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<LinearScaleComponent>(1,"Scales", UIHelper.LinearGaugeElementImages[0],new LinearScaleComponent[]{this},model.Owner) 
				};
				designerform.ShowDialog();
			}
		}
		bool ShouldSerializeAppearanceScale() { return AppearanceScale.ShouldSerialize(); }
		void ResetAppearanceScale() { AppearanceScale.Reset(); }
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentAppearanceScale"),
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentAppearanceMinorTickmark"),
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentAppearanceMajorTickmark"),
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentAppearanceTickmarkTextBackground"),
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentAppearanceTickmarkText"),
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentDataBindings"),
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
	[DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentIsAnimating")]
#endif
		public override bool IsAnimating {
			get { return base.IsAnimating; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentEasingMode"),
#endif
 DefaultValue(EasingMode.EaseIn)]
		public EasingMode EasingMode {
			get { return Provider.EasingMode; }
			set { Provider.EasingMode = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentEasingFunction"),
#endif
 DefaultValue(null)]
		public IEasingFunction EasingFunction {
			get { return Provider.EasingFunction; }
			set { Provider.EasingFunction = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentEnableAnimation"),
#endif
 DefaultValue(false)]
		public bool EnableAnimation {
			get { return Provider.EnableAnimation; }
			set { Provider.EnableAnimation = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentFrameCount"),
#endif
 DefaultValue(1000)]
		public int FrameCount {
			get { return Provider.FrameCount; }
			set { Provider.FrameCount = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleComponentFrameDelay"),
#endif
 DefaultValue(10000)]
		public int FrameDelay {
			get { return Provider.FrameDelay; }
			set { Provider.FrameDelay = value; }
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.LinearScaleBackgroundLayerComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleBackgroundLayerComponent : LinearScaleBackgroundLayer, ISupportInitialize, IBindableComponent,
			ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<LinearScaleBackgroundLayerComponent> {
		BaseBindableProvider bindableProviderCore;
		public LinearScaleBackgroundLayerComponent()
			: base() {
		}
		public LinearScaleBackgroundLayerComponent(string name)
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
			get { return typeof(LinearScaleBackgroundLayerComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, LinearScale);
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
				new CustomizeActionInfo("RunDesigner", "Open Background Layer Designer Page", "Run Designer", UIHelper.LinearGaugeElementImages[1])
			};
		}
		void ISupportAssign<LinearScaleBackgroundLayerComponent>.Assign(LinearScaleBackgroundLayerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleBackgroundLayerComponent>.IsDifferFrom(LinearScaleBackgroundLayerComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<LinearScaleBackgroundLayerComponent>(2,"Background Layers",UIHelper.LinearGaugeElementImages[1],new LinearScaleBackgroundLayerComponent[]{this},model.Owner) 
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleBackgroundLayerComponentDataBindings"),
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
	[Designer("DevExpress.XtraGauges.Win.Design.LinearScaleEffectLayerComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleEffectLayerComponent : LinearScaleEffectLayer, ISupportInitialize, IBindableComponent,
			ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<LinearScaleEffectLayerComponent> {
		BaseBindableProvider bindableProviderCore;
		public LinearScaleEffectLayerComponent()
			: base() {
		}
		public LinearScaleEffectLayerComponent(string name)
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
			get { return typeof(LinearScaleEffectLayerComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, LinearScale);
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
				new CustomizeActionInfo("RunDesigner", "Open Effect Layer Designer Page", "Run Designer", UIHelper.LinearGaugeElementImages[5])
			};
		}
		void ISupportAssign<LinearScaleEffectLayerComponent>.Assign(LinearScaleEffectLayerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleEffectLayerComponent>.IsDifferFrom(LinearScaleEffectLayerComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<LinearScaleEffectLayerComponent>(6,"Effect Layers",UIHelper.LinearGaugeElementImages[5],new LinearScaleEffectLayerComponent[]{this},model.Owner) 
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleEffectLayerComponentDataBindings"),
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
	[Designer("DevExpress.XtraGauges.Win.Design.LinearScaleRangeBarComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleRangeBarComponent : LinearScaleRangeBar, ISupportInitialize, IBindableComponent,
			ISupportVisualDesigning, ICustomizationFrameClient, ISupportAssign<LinearScaleRangeBarComponent> {
		BaseBindableProvider bindableProviderCore;
		public LinearScaleRangeBarComponent()
			: base() {
		}
		public LinearScaleRangeBarComponent(string name)
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
			get { return typeof(LinearScaleRangeBarComponentWrapper); }
		}
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, LinearScale);
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
				new CustomizeActionInfo("RunDesigner", "Open RangeBar Designer Page", "Run Designer", UIHelper.LinearGaugeElementImages[3])
			};
		}
		void ISupportAssign<LinearScaleRangeBarComponent>.Assign(LinearScaleRangeBarComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleRangeBarComponent>.IsDifferFrom(LinearScaleRangeBarComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<LinearScaleRangeBarComponent>(4,"RangeBars",UIHelper.LinearGaugeElementImages[3],new LinearScaleRangeBarComponent[]{this},model.Owner)  
				};
				designerform.ShowDialog();
			}
		}
		bool ShouldSerializeAppearanceRangeBar() { return AppearanceRangeBar.ShouldSerialize(); }
		void ResetAppearanceRangeBar() { AppearanceRangeBar.Reset(); }
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleRangeBarComponentAppearanceRangeBar"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public BaseShapeAppearance AppearanceRangeBar {
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleRangeBarComponentDataBindings"),
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
	[Designer("DevExpress.XtraGauges.Win.Design.LinearScaleMarkerComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleMarkerComponent : LinearScaleMarker, ISupportInitialize, IBindableComponent,
		ISupportVisualDesigning, ICustomizationFrameClient, ISupportAssign<LinearScaleMarkerComponent> {
		BaseBindableProvider bindableProviderCore;
		public LinearScaleMarkerComponent()
			: base() {
		}
		public LinearScaleMarkerComponent(string name)
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
			get { return typeof(LinearScaleMarkerComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, LinearScale);
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
				new CustomizeActionInfo("RunDesigner", "Open Marker Designer Page", "Run Designer", UIHelper.LinearGaugeElementImages[4])
			};
		}
		void ISupportAssign<LinearScaleMarkerComponent>.Assign(LinearScaleMarkerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleMarkerComponent>.IsDifferFrom(LinearScaleMarkerComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<LinearScaleMarkerComponent>(5,"Markers",UIHelper.LinearGaugeElementImages[4],new LinearScaleMarkerComponent[]{this},model.Owner)  
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleMarkerComponentDataBindings"),
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
	[Designer("DevExpress.XtraGauges.Win.Design.LinearScaleLevelComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleLevelComponent : LinearScaleLevel, ISupportInitialize, IBindableComponent,
		ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<LinearScaleLevelComponent> {
		BaseBindableProvider bindableProviderCore;
		public LinearScaleLevelComponent()
			: base() {
		}
		public LinearScaleLevelComponent(string name)
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
			get { return typeof(LinearScaleLevelComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, LinearScale);
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
				new CustomizeActionInfo("RunDesigner", "Open Level Designer Page", "Run Designer", UIHelper.LinearGaugeElementImages[2])
			};
		}
		void ISupportAssign<LinearScaleLevelComponent>.Assign(LinearScaleLevelComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleLevelComponent>.IsDifferFrom(LinearScaleLevelComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<LinearScaleLevelComponent>(3,"Levels",UIHelper.LinearGaugeElementImages[2],new LinearScaleLevelComponent[]{this},model.Owner)  
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleLevelComponentDataBindings"),
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
	[Designer("DevExpress.XtraGauges.Win.Design.LinearScaleStateIndicatorComponentDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleStateIndicatorComponent : ScaleStateIndicator, ISupportInitialize, IBindableComponent,
		ICustomizationFrameClient, ISupportVisualDesigning, ISupportAssign<LinearScaleStateIndicatorComponent> {
		BaseBindableProvider bindableProviderCore;
		public LinearScaleStateIndicatorComponent()
			: base() {
		}
		public LinearScaleStateIndicatorComponent(string name)
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
			get { return typeof(LinearScaleStateIndicatorComponentWrapper); }
		}
		void ISupportVisualDesigning.OnInitDesigner() { }
		void ISupportVisualDesigning.OnCloseDesigner() { }
		void ISupportVisualDesigning.RenderDesignerElements(Graphics g) {
			DesignerElementVisualizerHelpers.DrawScaleDesignerElements(g, Scale as ILinearScale);
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
				new CustomizeActionInfo("RunDesigner", "Open State Indicator Designer Page", "Run Designer", UIHelper.LinearGaugeElementImages[6])
			};
		}
		void ISupportAssign<LinearScaleStateIndicatorComponent>.Assign(LinearScaleStateIndicatorComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleStateIndicatorComponent>.IsDifferFrom(LinearScaleStateIndicatorComponent source) {
			return IsDifferFrom(source);
		}
		protected void RunDesigner() {
			BaseGaugeModel model = BaseGaugeModel.Find(this);
			if(model == null) return;
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(model.Owner)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new PrimitiveCustomizationDesignerPage<LinearScaleStateIndicatorComponent>(7,"Indicators",UIHelper.LinearGaugeElementImages[6],new LinearScaleStateIndicatorComponent[]{this},model.Owner) 
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
	DevExpressXtraGaugesWinLocalizedDescription("LinearScaleStateIndicatorComponentDataBindings"),
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
