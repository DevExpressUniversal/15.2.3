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
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Web.ASPxGauges.Data;
namespace DevExpress.Web.ASPxGauges.Gauges.Linear {
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[ControlBuilder(typeof(LinearScaleComponentBuilder))]
	[Designer("DevExpress.Web.ASPxGauges.Design.LinearScaleComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleComponent : LinearScale, IComponent, ISupportAssign<LinearScaleComponent>, IStateManagedHierarchyObject {
		public LinearScaleComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public LinearScaleComponent(string name)
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
			return new LinearScaleRangeCollectionWeb(this);
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
	[ControlBuilder(typeof(LinearScaleBackgroundLayerComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.LinearScaleBackgroundLayerComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleBackgroundLayerComponent : LinearScaleBackgroundLayer, IComponent, ISupportAssign<LinearScaleBackgroundLayerComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public LinearScaleBackgroundLayerComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public LinearScaleBackgroundLayerComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		void ISupportAssign<LinearScaleBackgroundLayerComponent>.Assign(LinearScaleBackgroundLayerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleBackgroundLayerComponent>.IsDifferFrom(LinearScaleBackgroundLayerComponent source) {
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
	[ControlBuilder(typeof(LinearScaleLevelComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.LinearScaleLevelComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleLevelComponent : LinearScaleLevel, IComponent, ISupportAssign<LinearScaleLevelComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public LinearScaleLevelComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public LinearScaleLevelComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		void ISupportAssign<LinearScaleLevelComponent>.Assign(LinearScaleLevelComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleLevelComponent>.IsDifferFrom(LinearScaleLevelComponent source) {
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
	[ControlBuilder(typeof(LinearScaleMarkerComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.LinearScaleMarkerComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleMarkerComponent : LinearScaleMarker, IComponent, ISupportAssign<LinearScaleMarkerComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public LinearScaleMarkerComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public LinearScaleMarkerComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		void ISupportAssign<LinearScaleMarkerComponent>.Assign(LinearScaleMarkerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleMarkerComponent>.IsDifferFrom(LinearScaleMarkerComponent source) {
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
	[ControlBuilder(typeof(LinearScaleRangeBarComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.LinearScaleRangeBarComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleRangeBarComponent : LinearScaleRangeBar, IComponent, ISupportAssign<LinearScaleRangeBarComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public LinearScaleRangeBarComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public LinearScaleRangeBarComponent(string name)
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
		public BaseShapeAppearance AppearanceRangeBar {
			get { return Appearance; }
		}
		void ISupportAssign<LinearScaleRangeBarComponent>.Assign(LinearScaleRangeBarComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleRangeBarComponent>.IsDifferFrom(LinearScaleRangeBarComponent source) {
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
	[ControlBuilder(typeof(LinearScaleStateIndicatorComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.LinearScaleStateIndicatorComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleStateIndicatorComponent : ScaleStateIndicator, IComponent, ISupportAssign<LinearScaleStateIndicatorComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public LinearScaleStateIndicatorComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public LinearScaleStateIndicatorComponent(string name)
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
		void ISupportAssign<LinearScaleStateIndicatorComponent>.Assign(LinearScaleStateIndicatorComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleStateIndicatorComponent>.IsDifferFrom(LinearScaleStateIndicatorComponent source) {
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
	[ControlBuilder(typeof(LinearScaleEffectLayerComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.LinearScaleEffectLayerComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class LinearScaleEffectLayerComponent : LinearScaleEffectLayer, IComponent, ISupportAssign<LinearScaleEffectLayerComponent>, IStateManagedHierarchyObject, IScaleDependentElement {
		public LinearScaleEffectLayerComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public LinearScaleEffectLayerComponent(string name)
			: base(name) {
		}
		string arcScaleIDCore;
		[XtraSerializableProperty]
		[Editor("DevExpress.Web.ASPxGauges.Design.ScaleIDTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ScaleID {
			get { return arcScaleIDCore; }
			set { arcScaleIDCore = value; }
		}
		void ISupportAssign<LinearScaleEffectLayerComponent>.Assign(LinearScaleEffectLayerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<LinearScaleEffectLayerComponent>.IsDifferFrom(LinearScaleEffectLayerComponent source) {
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
	public sealed class LinearTagPrefix : Control { LinearTagPrefix() { } }
}
