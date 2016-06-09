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
namespace DevExpress.Web.ASPxGauges.Gauges.Digital {
	[ControlBuilder(typeof(DigitalBackgroundLayerComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.DigitalBackgroundLayerComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class DigitalBackgroundLayerComponent : DigitalBackgroundLayer, IComponent, ISupportAssign<DigitalBackgroundLayerComponent>, IStateManagedHierarchyObject {
		public DigitalBackgroundLayerComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public DigitalBackgroundLayerComponent(string name)
			: base(name) {
		}
		void ISupportAssign<DigitalBackgroundLayerComponent>.Assign(DigitalBackgroundLayerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<DigitalBackgroundLayerComponent>.IsDifferFrom(DigitalBackgroundLayerComponent source) {
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
	[ControlBuilder(typeof(DigitalEffectLayerComponentBuilder))]
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.DigitalEffectLayerComponentDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class DigitalEffectLayerComponent : DigitalEffectLayer, IComponent, ISupportAssign<DigitalEffectLayerComponent>, IStateManagedHierarchyObject {
		public DigitalEffectLayerComponent()
			: base() {
			ComponentBuilderInterceptor.Instance.RaiseObjectCreated(this);
		}
		public DigitalEffectLayerComponent(string name)
			: base(name) {
		}
		void ISupportAssign<DigitalEffectLayerComponent>.Assign(DigitalEffectLayerComponent source) {
			Assign(source);
		}
		bool ISupportAssign<DigitalEffectLayerComponent>.IsDifferFrom(DigitalEffectLayerComponent source) {
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
	public sealed class DigitalTagPrefix : Control { DigitalTagPrefix() { } }
}
