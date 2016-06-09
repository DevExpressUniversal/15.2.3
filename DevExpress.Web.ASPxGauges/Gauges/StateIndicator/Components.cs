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
using DevExpress.Web.ASPxGauges.Data;
namespace DevExpress.Web.ASPxGauges.Gauges.State {
	[ControlBuilder(typeof(StateIndicatorComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.StateIndicatorComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class StateIndicatorComponent : StateIndicator, IComponent, ISupportAssign<StateIndicatorComponent>, IStateManagedHierarchyObject {
		public StateIndicatorComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public StateIndicatorComponent(string name)
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
		void ISupportAssign<StateIndicatorComponent>.Assign(StateIndicatorComponent source) {
			Assign(source);
		}
		bool ISupportAssign<StateIndicatorComponent>.IsDifferFrom(StateIndicatorComponent source) {
			return IsDifferFrom(source);
		}
		protected override IndicatorStateCollection CreateStates() {
			return new IndicatorStateCollectionWeb();
		}
		protected override IIndicatorState CreateState(string name) {
			return new IndicatorStateWeb(name);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Editor("DevExpress.Web.ASPxGauges.Design.IndicatorStateWebCollectionEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public override IndicatorStateCollection States {
			get { return base.States; }
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
	public sealed class StateIndicatorTagPrefix : Control { StateIndicatorTagPrefix() { } }
}
