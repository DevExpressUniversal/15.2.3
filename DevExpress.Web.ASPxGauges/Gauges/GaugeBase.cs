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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.ASPxGauges.Gauges;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Layout;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
namespace DevExpress.Web.ASPxGauges.Base {
	[ControlBuilder(typeof(BaseGaugeBuilder))]
	public abstract class BaseGaugeWeb : BaseGauge, ILayoutManagerClient, IStateManagedHierarchyObject {
		BaseGaugeProviderWeb gaugeProviderCore;
		public BaseGaugeWeb()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public BaseGaugeWeb(IGaugeContainer container)
			: base(container) {
		}
		protected override void OnCreate() {
			this.gaugeProviderCore = CreateGaugeProvider();
		}
		protected override void OnDispose() {
			if(GaugeProvider != null) {
				GaugeProvider.Dispose();
				this.gaugeProviderCore = null;
			}
		}
		protected BaseGaugeProviderWeb GaugeProvider {
			get { return gaugeProviderCore; }
		}
		protected override void ClearCore() {
			Labels.Clear();
		}
		protected abstract BaseGaugeProviderWeb CreateGaugeProvider();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
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
		protected bool IsLoaded {
			get {
				ASPxWebControl webControl = GaugeContainer as ASPxWebControl;
				return (webControl != null) && webControl.Loaded;
			}
		}
		protected override void AddComponentToDesignTimeSurface(IComponent component) {
			if(IsInEditMode(this)) return;
			base.AddComponentToDesignTimeSurface(component);
		}
		protected override void RemoveComponentFromDesignTimeSurface(IComponent component) {
			if(!IsInEditMode(this)) return;
			base.RemoveComponentFromDesignTimeSurface(component);
		}
		internal void AddComponentToWebDesignTimeSurface(IComponent component) {
			AddComponentToDesignTimeSurface(component);
			if(IsLoaded) ForceUpdateModel();
		}
		internal void RemoveComponentFromWebDesignTimeSurface(IComponent component) {
			RemoveComponentFromDesignTimeSurface(component);
			if(IsLoaded) ForceUpdateModel();
		}
		protected override List<ISerizalizeableElement> GetChildernCore() {
			List<ISerizalizeableElement> list = new List<ISerizalizeableElement>();
			CollectChildren(list, Labels, "Labels");
			return list;
		}
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		string fakeID = null; 
		public string ID {
			get { return fakeID; }
			set { fakeID = value; }
		}
		protected override List<string> GetNamesCore() {
			List<string> names = new List<string>();
			ComponentCollectionExtention.CollectNames(Labels, names);
			return names;
		}
		protected abstract string GetPrefixType();
		protected string Prefix(string element) {
			string name = (Site != null) ? Site.Name : Name;
			if(string.IsNullOrEmpty(name)) return GetPrefixType() + element + "Component";
			return name + "_" + element;
		}
		protected internal abstract void CheckElementsAffinity();
		#region IStateManagedHierarchyObject
		ViewStateInternal ViewState = new ViewStateInternal();
		IStateManagedHierarchyObject[] IStateManagedHierarchyObject.StateManagedObjects {
			get { return GetStateManagedHierarchyObjects(); }
		}
		protected virtual IStateManagedHierarchyObject[] GetStateManagedHierarchyObjects() {
			return new IStateManagedHierarchyObject[] { Labels };
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
		void IStateManager.LoadViewState(object state) {
			StateManagedHierarchyObjectExtension.LoadViewState(this, state);
		}
		object IStateManager.SaveViewState() {
			return StateManagedHierarchyObjectExtension.SaveViewState(this);
		}
		void IStateManager.TrackViewState() {
			StateManagedHierarchyObjectExtension.TrackViewState(this);
		}
		#endregion IStateManagedHierarchyObject
		protected internal class ComponentsHost : IDisposable {
			System.ComponentModel.Design.IDesignerHost host;
			IGauge gauge;
			static IDictionary<IGauge, ComponentsHost> hosts = new Dictionary<IGauge, ComponentsHost>();
			public ComponentsHost(ITypeDescriptorContext context, IServiceProvider provider, object value) {
				host = GetDesignerHost(provider);
				gauge = context.Instance as IGauge;
				if(gauge != null)
					hosts.Add(gauge, this);
				if(host != null && host.Container != null)
					AddComponentsToHost(gauge, host);
			}
			public void Dispose() {
				if(host != null && host.Container != null)
					RemoveComponentsFromHost(gauge, host);
				if(gauge != null)
					hosts.Remove(gauge);
			}
			public static bool GetIsInComponentsHost(IGauge gauge) {
				return (gauge != null) && hosts.ContainsKey(gauge);
			}
			void AddComponentsToHost(IGauge gauge, System.ComponentModel.Design.IDesignerHost host) {
				List<ISerizalizeableElement> childrenList = ((ISerizalizeableElement)gauge).GetChildren();
				foreach(object element in childrenList) {
					IComponent component = element as IComponent;
					if(component != null)
						host.Container.Add(component);
				}
			}
			void RemoveComponentsFromHost(IGauge gauge, System.ComponentModel.Design.IDesignerHost host) {
				List<ISerizalizeableElement> childrenList = ((ISerizalizeableElement)gauge).GetChildren();
				foreach(object element in childrenList) {
					IComponent component = element as IComponent;
					if(component != null)
						host.Container.Remove(component);
				}
			}
			System.ComponentModel.Design.IDesignerHost GetDesignerHost(IServiceProvider provider) {
				return (System.ComponentModel.Design.IDesignerHost)provider.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			}
		}
		protected internal static bool IsInEditMode(IGauge gauge) {
			return ComponentsHost.GetIsInComponentsHost(gauge);
		}
	}
	[Editor("DevExpress.Web.ASPxGauges.Design.GaugeCollectionTypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class GaugeCollectionWeb : GaugeCollection, IStateManagedHierarchyObjectCollection {
		protected override void OnDispose() {
			ViewState.SaveStateSnapshot(null);
			ViewState = null;
			base.OnDispose();
		}
		protected override void OnElementAdded(IGauge element) {
			base.OnElementAdded(element);
			if(((IStateManagedHierarchyObject)this).IsTrackingViewState)
				((IStateManagedHierarchyObject)element).TrackViewState();
		}
		#region IStateManagedHierarchyObjectCollection
		IStateManagedHierarchyObject IStateManagedHierarchyObjectCollection.this[int i] {
			get { return List[i] as IStateManagedHierarchyObject; }
		}
		void IStateManagedHierarchyObjectCollection.Add(IStateManagedHierarchyObject obj) {
			base.Add(obj as IGauge);
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
	public abstract class BaseGaugeProviderWeb : BaseObject {
		BaseGaugeWeb ownerCore;
		BaseGaugeChangedHandler gaugeChangedHandlerCore;
		LabelComponentCollection labelsCore;
		public BaseGaugeProviderWeb(BaseGaugeWeb gauge, BaseGaugeChangedHandler handler)
			: base() {
			this.ownerCore = gauge;
			this.gaugeChangedHandlerCore = handler;
			labelsCore = new LabelComponentCollection();
			Labels.CollectionChanged += OnLabelsCollectionChanged;
		}
		protected override void OnCreate() { }
		protected override void OnDispose() {
			if(Labels != null) {
				Labels.CollectionChanged -= OnLabelsCollectionChanged;
				Labels.Dispose();
				this.labelsCore = null;
			}
			this.gaugeChangedHandlerCore = null;
			this.ownerCore = null;
		}
		public virtual void BuildModel(BaseGaugeModel model) {
			model.Composite.AddRange(Labels.ToArray());
		}
		void OnLabelsCollectionChanged(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<LabelComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		protected BaseGaugeWeb Owner {
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
			if(Owner != null) Owner.AddComponentToWebDesignTimeSurface((IComponent)element);
		}
		void OnElementRemoved(BaseElement<IRenderableElement> element) {
			element.Changed -= OnComponentChanged;
			if(Owner != null) Owner.RemoveComponentFromWebDesignTimeSurface((IComponent)element);
		}
		void OnComponentChanged(object sender, EventArgs e) {
			if(gaugeChangedHandlerCore != null) gaugeChangedHandlerCore(sender, e);
		}
		public LabelComponentCollection Labels {
			get { return labelsCore; }
		}
	}
}
