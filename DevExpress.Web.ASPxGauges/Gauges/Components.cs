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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Web.UI;
using DevExpress.Web.ASPxGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Resources;
namespace DevExpress.Web.ASPxGauges.Gauges {
	public class LabelComponentCollection :
		ComponentCollectionWeb<LabelComponent> {
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[ControlBuilder(typeof(LabelComponentBuilder))]
	[Designer("DevExpress.Web.ASPxGauges.Design.LabelComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class LabelComponent : DevExpress.XtraGauges.Core.Model.Label, IComponent, ICustomizationFrameClient, ISupportAssign<LabelComponent>, IStateManagedHierarchyObject {
		public LabelComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public LabelComponent(string name)
			: base(name) {
		}
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			CustomizationFrameBase[] customizeFrames = new CustomizationFrameBase[] { 
				new SelectionFrame(this)
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
				new CustomizeActionInfo("RunDesigner", "Open Labels Designer Page", "Run Designer", UIHelper.CircularGaugeElementImages[0])
			};
		}
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		void ISupportAssign<LabelComponent>.Assign(LabelComponent label) {
			Assign(label);
		}
		bool ISupportAssign<LabelComponent>.IsDifferFrom(LabelComponent label) {
			return IsDifferFrom(label);
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
	public class IndicatorStateCollectionWeb : IndicatorStateCollection, IStateManagedHierarchyObjectCollection {
		protected override void OnElementAdded(IIndicatorState element) {
			base.OnElementAdded(element);
			if(((IStateManagedHierarchyObject)this).IsTrackingViewState)
				((IStateManagedHierarchyObject)element).TrackViewState();
		}
		#region IStateManagedHierarchyObjectCollection
		IStateManagedHierarchyObject IStateManagedHierarchyObjectCollection.this[int i] {
			get { return List[i] as IStateManagedHierarchyObject; }
		}
		void IStateManagedHierarchyObjectCollection.Add(IStateManagedHierarchyObject obj) {
			base.Add(obj as IIndicatorState);
		}
		#endregion IStateManagedHierarchyObjectCollection
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectCollectionHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectCollectionHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectCollectionHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectCollectionExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectCollectionExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectCollectionExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	public class IndicatorStateWeb : BaseIndicatorState, IIndicatorState, ISupportAssign<IIndicatorState>, IStateManagedHierarchyObject {
		public IndicatorStateWeb() : base() { }
		public IndicatorStateWeb(string name) : base(name) { }
		public IndicatorStateWeb(StateIndicatorShapeType shapeType) : base(shapeType) { }
		protected override BaseObject CloneCore() {
			return new IndicatorStateWeb();
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
	public class ScaleIndicatorStateWeb : BaseIndicatorState, IScaleIndicatorState, ISupportAssign<IIndicatorState>, IStateManagedHierarchyObject {
		public ScaleIndicatorStateWeb() : base() { }
		public ScaleIndicatorStateWeb(string name) : base(name) { }
		public ScaleIndicatorStateWeb(StateIndicatorShapeType shapeType) : base(shapeType) { }
		protected override BaseObject CloneCore() {
			return new ScaleIndicatorStateWeb();
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
	public class ArcScaleRangeCollectionWeb : ScaleRangeCollection, IStateManagedHierarchyObjectCollection {
		public ArcScaleRangeCollectionWeb(IArcScale scale) : base(scale) { }
		protected override IRange CreateRange() {
			return new ArcScaleRangeWeb();
		}
		protected override void OnElementAdded(IRange element) {
			base.OnElementAdded(element);
			if(((IStateManagedHierarchyObject)this).IsTrackingViewState)
				((IStateManagedHierarchyObject)element).TrackViewState();
		}
		#region IStateManagedHierarchyObjectCollection
		IStateManagedHierarchyObject IStateManagedHierarchyObjectCollection.this[int i] {
			get { return List[i] as IStateManagedHierarchyObject; }
		}
		void IStateManagedHierarchyObjectCollection.Add(IStateManagedHierarchyObject obj) {
			base.Add(obj as IRange);
		}
		#endregion IStateManagedHierarchyObjectCollection
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectCollectionHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectCollectionHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectCollectionHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectCollectionExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectCollectionExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectCollectionExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	public class LinearScaleRangeCollectionWeb : ScaleRangeCollection, IStateManagedHierarchyObjectCollection {
		public LinearScaleRangeCollectionWeb(ILinearScale scale) : base(scale) { }
		protected override IRange CreateRange() {
			return new LinearScaleRangeWeb();
		}
		protected override void OnElementAdded(IRange element) {
			base.OnElementAdded(element);
			if(((IStateManagedHierarchyObject)this).IsTrackingViewState)
				((IStateManagedHierarchyObject)element).TrackViewState();
		}
		#region IStateManagedHierarchyObjectCollection
		IStateManagedHierarchyObject IStateManagedHierarchyObjectCollection.this[int i] {
			get { return List[i] as IStateManagedHierarchyObject; }
		}
		void IStateManagedHierarchyObjectCollection.Add(IStateManagedHierarchyObject obj) {
			base.Add(obj as IRange);
		}
		#endregion IStateManagedHierarchyObjectCollection
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectCollectionHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectCollectionHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectCollectionHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectCollectionExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectCollectionExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectCollectionExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	public class ArcScaleRangeWeb : ArcScaleRange, IStateManagedHierarchyObject {
		protected override BaseObject CloneCore() {
			return new ArcScaleRangeWeb();
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
	public class LinearScaleRangeWeb : LinearScaleRange, IStateManagedHierarchyObject {
		protected override BaseObject CloneCore() {
			return new LinearScaleRangeWeb();
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
	public class ScaleLabelCollectionWeb : ScaleLabelCollection, IStateManagedHierarchyObjectCollection {
		public ScaleLabelCollectionWeb(IScale scale) : base(scale) { }
		protected override ILabel CreateLabel() {
			return new ScaleLabelWeb();
		}
		protected override void OnElementAdded(ILabel element) {
			base.OnElementAdded(element);
			if(((IStateManagedHierarchyObject)this).IsTrackingViewState)
				((IStateManagedHierarchyObject)element).TrackViewState();
		}
		#region IStateManagedHierarchyObjectCollection
		IStateManagedHierarchyObject IStateManagedHierarchyObjectCollection.this[int i] {
			get { return List[i] as IStateManagedHierarchyObject; }
		}
		void IStateManagedHierarchyObjectCollection.Add(IStateManagedHierarchyObject obj) {
			base.Add(obj as ILabel);
		}
		#endregion IStateManagedHierarchyObjectCollection
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return new IStateManagedHierarchyObject[] { null }; }
		}
		object IStateManagedHierarchyObject.SaveViewStateCore() {
			return StateManagedObjectCollectionHelper.SaveViewStateCore(ViewState.State, this);
		}
		void IStateManagedHierarchyObject.LoadViewStateCore(object state) {
			StateManagedObjectCollectionHelper.LoadViewStateCore(this, state);
		}
		void IStateManagedHierarchyObject.TrackViewStateCore() {
			ViewState.TrackViewState();
			ViewState.SaveStateSnapshot(StateManagedObjectCollectionHelper.TrackViewStateCore(this));
		}
		bool IStateManager.IsTrackingViewState {
			get { return ViewState.IsTrackingViewState; }
		}
		void IStateManager.LoadViewState(object state) { StateManagedHierarchyObjectCollectionExtension.LoadViewState(this, state); }
		object IStateManager.SaveViewState() { return StateManagedHierarchyObjectCollectionExtension.SaveViewState(this); }
		void IStateManager.TrackViewState() { StateManagedHierarchyObjectCollectionExtension.TrackViewState(this); }
		#endregion IStateManagedHierarchyObject
	}
	public class ScaleLabelWeb : ScaleLabel, IStateManagedHierarchyObject {
		protected override BaseObject CloneCore() {
			return new ScaleLabelWeb();
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
	public sealed class GaugesTagPrefix : Control { GaugesTagPrefix() { } }
}
