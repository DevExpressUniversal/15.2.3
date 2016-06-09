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
using System.Text;
using DevExpress.XtraGauges.Core.Model;
using System.ComponentModel;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core.Drawing;
using System.Drawing;
using DevExpress.XtraGauges.Core.Resources;
using System.Web.UI;
using DevExpress.Web.ASPxGauges.Base;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing.Design;
using DevExpress.Web;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Web.ASPxGauges.Data;
namespace DevExpress.Web.ASPxGauges.Gauges.Circular {
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[ControlBuilder(typeof(ArcScaleComponentBuilder))]
	[Designer("DevExpress.Web.ASPxGauges.Design.ArcScaleComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleComponent : ArcScale, IComponent, ISupportAssign<ArcScaleComponent>, IStateManagedHierarchyObject {
		public ArcScaleComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public ArcScaleComponent(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.bindableProviderCore = CreateBindableProvider();
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
			return new BaseBindableProvider(this);
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
			get { return BindableProvider.BindingContainer; }
		}
		#endregion Databinding
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
		bool ShouldSerializeAppearanceScale() { return AppearanceScale.ShouldSerialize(); }
		void ResetAppearanceScale() { AppearanceScale.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseScaleAppearance AppearanceScale {
			get { return Appearance; }
		}
		bool ShouldSerializeAppearanceMinorTickmark() { return AppearanceMinorTickmark.ShouldSerialize(); }
		void ResetAppearanceMinorTickmark() { AppearanceMinorTickmark.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceMinorTickmark {
			get { return MinorTickmark.Shape.Appearance; }
		}
		bool ShouldSerializeAppearanceMajorTickmark() { return AppearanceMajorTickmark.ShouldSerialize(); }
		void ResetAppearanceMajorTickmark() { AppearanceMajorTickmark.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceMajorTickmark {
			get { return MajorTickmark.Shape.Appearance; }
		}
		bool ShouldSerializeAppearanceTickmarkTextBackground() { return AppearanceTickmarkTextBackground.ShouldSerialize(); }
		void ResetAppearanceTickmarkTextBackground() { AppearanceTickmarkTextBackground.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseShapeAppearance AppearanceTickmarkTextBackground {
			get { return MajorTickmark.TextShape.Appearance; }
		}
		bool ShouldSerializeAppearanceTickmarkText() { return AppearanceTickmarkText.ShouldSerialize(); }
		void ResetAppearanceTickmarkText() { AppearanceTickmarkText.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseTextAppearance AppearanceTickmarkText {
			get { return MajorTickmark.TextShape.AppearanceText; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Editor("DevExpress.Web.ASPxGauges.Design.RangeWebCollectionEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public override RangeCollection Ranges {
			get { return base.Ranges; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Editor("DevExpress.Web.ASPxGauges.Design.LabelWebCollectionEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public override LabelCollection Labels {
			get { return base.Labels; }
		}
		protected override ScaleRangeCollection CreateRanges() {
			return new ArcScaleRangeCollectionWeb(this);
		}
		protected override ScaleLabelCollection CreateLabels() {
			return new ScaleLabelCollectionWeb(this);
		}
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { Ranges as IStateManagedHierarchyObject }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	[ControlBuilder(typeof(ArcScaleBackgroundLayerComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.ArcScaleBackgroundLayerComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleBackgroundLayerComponent : ArcScaleBackgroundLayer, IComponent, ISupportAssign<ArcScaleBackgroundLayerComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public ArcScaleBackgroundLayerComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public ArcScaleBackgroundLayerComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		void ISupportAssign<ArcScaleBackgroundLayerComponent>.Assign(ArcScaleBackgroundLayerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleBackgroundLayerComponent>.IsDifferFrom(ArcScaleBackgroundLayerComponent source) {
			return IsDifferFrom(source);
		}
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	[ControlBuilder(typeof(ArcScaleNeedleComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.ArcScaleNeedleComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleNeedleComponent : ArcScaleNeedle, IComponent, ISupportAssign<ArcScaleNeedleComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public ArcScaleNeedleComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public ArcScaleNeedleComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		void ISupportAssign<ArcScaleNeedleComponent>.Assign(ArcScaleNeedleComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleNeedleComponent>.IsDifferFrom(ArcScaleNeedleComponent source) {
			return IsDifferFrom(source);
		}
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	[ControlBuilder(typeof(ArcScaleMarkerComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.ArcScaleMarkerComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleMarkerComponent : ArcScaleMarker, IComponent, ISupportAssign<ArcScaleMarkerComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public ArcScaleMarkerComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public ArcScaleMarkerComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		void ISupportAssign<ArcScaleMarkerComponent>.Assign(ArcScaleMarkerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleMarkerComponent>.IsDifferFrom(ArcScaleMarkerComponent source) {
			return IsDifferFrom(source);
		}
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	[ControlBuilder(typeof(ArcScaleRangeBarComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.ArcScaleRangeBarComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleRangeBarComponent : ArcScaleRangeBar, IComponent, ISupportAssign<ArcScaleRangeBarComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public ArcScaleRangeBarComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public ArcScaleRangeBarComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		bool ShouldSerializeAppearanceRangeBar() { return AppearanceRangeBar.ShouldSerialize(); }
		void ResetAppearanceRangeBar() { AppearanceRangeBar.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public RangeBarAppearance AppearanceRangeBar {
			get { return Appearance; }
		}
		void ISupportAssign<ArcScaleRangeBarComponent>.Assign(ArcScaleRangeBarComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleRangeBarComponent>.IsDifferFrom(ArcScaleRangeBarComponent source) {
			return IsDifferFrom(source);
		}
		protected override void ShapeProcessing(BaseShape shape) {
			(shape as SectorShape).RoundedCaps = RoundedCaps;
			(shape as SectorShape).ShowBackground = ShowBackground;
			(shape as SectorShape).Appearance.Assign(AppearanceRangeBar);
			(shape as SectorShape).AppearanceRangeBar.Assign(AppearanceRangeBar);
		}
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	[ControlBuilder(typeof(ArcScaleSpindleCapComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.ArcScaleSpindleCapComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleSpindleCapComponent : ArcScaleSpindleCap, IComponent, ICustomizationFrameClient, ISupportAssign<ArcScaleSpindleCapComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public ArcScaleSpindleCapComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public ArcScaleSpindleCapComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() { return null; }
		void ICustomizationFrameClient.ResetAutoLayout() { }
		Rectangle ICustomizationFrameClient.Bounds {
			get { return Rectangle.Round(Info.RelativeBoundBox); }
			set { }
		}
		CustomizeActionInfo[] ISupportCustomizeAction.GetActions() { return null; }
		void ISupportAssign<ArcScaleSpindleCapComponent>.Assign(ArcScaleSpindleCapComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleSpindleCapComponent>.IsDifferFrom(ArcScaleSpindleCapComponent source) {
			return IsDifferFrom(source);
		}
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	[ControlBuilder(typeof(ArcScaleEffectLayerComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.ArcScaleEffectLayerComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleEffectLayerComponent : ArcScaleEffectLayer, IComponent, ISupportAssign<ArcScaleEffectLayerComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public ArcScaleEffectLayerComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public ArcScaleEffectLayerComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		void ISupportAssign<ArcScaleEffectLayerComponent>.Assign(ArcScaleEffectLayerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleEffectLayerComponent>.IsDifferFrom(ArcScaleEffectLayerComponent source) {
			return IsDifferFrom(source);
		}
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	[ControlBuilder(typeof(ArcScaleStateIndicatorComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.ArcScaleStateIndicatorComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class ArcScaleStateIndicatorComponent : ScaleStateIndicator, IComponent, ISupportAssign<ArcScaleStateIndicatorComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public ArcScaleStateIndicatorComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public ArcScaleStateIndicatorComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		protected override IndicatorStateCollection CreateStates() {
			return new IndicatorStateCollectionWeb();
		}
		protected override IIndicatorState CreateState(string name) {
			return new ScaleIndicatorStateWeb(name);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Editor("DevExpress.Web.ASPxGauges.Design.IndicatorStateWebCollectionEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public override IndicatorStateCollection States {
			get { return base.States; }
		}
		void ISupportAssign<ArcScaleStateIndicatorComponent>.Assign(ArcScaleStateIndicatorComponent source) {
			Assign(source);
		}
		bool ISupportAssign<ArcScaleStateIndicatorComponent>.IsDifferFrom(ArcScaleStateIndicatorComponent source) {
			return IsDifferFrom(source);
		}
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { States as IStateManagedHierarchyObject }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	public sealed class CircularTagPrefix : Control { CircularTagPrefix() { } }
}
