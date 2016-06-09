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
using System.Windows;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Xpf.Scheduler.Native {
	#region SchedulerControlPropertySyncManager
	public class SchedulerControlPropertySyncManager : DependencyPropertySyncManager {
		SchedulerControl control;
		public SchedulerControlPropertySyncManager(SchedulerControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		public SchedulerControl Control { get { return control; } }
		public override void Register() {
			PropertyMapperTable.RegisterPropertyMapper(SchedulerControl.StartProperty, new ControlStartPropertyMapper(SchedulerControl.StartProperty, Control));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerControl.StorageProperty, new ControlSetStoragePropertyMapper(SchedulerControl.StorageProperty, Control));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerControl.ActiveViewTypeProperty, new ControlActiveViewTypePropertyMapper(SchedulerControl.ActiveViewTypeProperty, Control));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerControl.GroupTypeProperty, new ControlGroupTypePropertyMapper(SchedulerControl.GroupTypeProperty, Control));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerControl.OptionsBehaviorProperty, new ContainerOptionsBehaviorPropertyMapper(SchedulerControl.OptionsBehaviorProperty, Control));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerControl.OptionsCustomizationProperty, new ContainerOptionsCustomizationPropertyMapper(SchedulerControl.OptionsCustomizationProperty, Control));
			PropertyMapperTable.RegisterPropertyMapper(SchedulerControl.OptionsViewProperty, new ContainerOptionsViewPropertyMapper(SchedulerControl.OptionsViewProperty, Control));
		}
	}
	#endregion
	#region SchedulerControl mapers
	public abstract class SchedulerControlPropertyMapperBase : DependencyPropertyMapperBase {
		protected SchedulerControlPropertyMapperBase(DependencyProperty property, SchedulerControl control)
			: base(property, control) {
		}
		public SchedulerControl SchedulerControl { get { return (SchedulerControl)Owner; } }
		public InnerSchedulerControl InnerControl { get { return SchedulerControl.InnerControl; } }
	}
	public class ControlStartPropertyMapper : SchedulerControlPropertyMapperBase {
		public ControlStartPropertyMapper(DependencyProperty property, SchedulerControl control)
			: base(property, control) {
		}
		protected override void SubscribeEvents() {
			InnerControl.VisibleIntervalChanged += OnVisibleIntervalChanged;
		}
		protected void OnVisibleIntervalChanged(object sender, EventArgs e) {
			UpdateOwnerPropertyValue();
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerControl.Start = Convert.ToDateTime(newValue);
		}
		public override object GetInnerPropertyValue() {
			return InnerControl.ActiveView.VisibleStart;
		}
		public override void Dispose() {
			base.Dispose();
			InnerControl.VisibleIntervalChanged -= OnVisibleIntervalChanged;
		}
	}
	public class ControlActiveViewTypePropertyMapper : SchedulerControlPropertyMapperBase {
		public ControlActiveViewTypePropertyMapper(DependencyProperty property, SchedulerControl control)
			: base(property, control) {
		}
		protected override void SubscribeEvents() {
			InnerControl.ActiveViewChanged += OnInnerControlActiveViewChanged;
		}
		void OnInnerControlActiveViewChanged(object sender, EventArgs e) {
			UpdateOwnerPropertyValue();
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			InnerControl.ActiveViewType = (SchedulerViewType)newValue;
		}
		public override object GetInnerPropertyValue() {
			return InnerControl.ActiveViewType;
		}
		public override void Dispose() {
			base.Dispose();
			InnerControl.ActiveViewChanged -= OnInnerControlActiveViewChanged;
		}
	}
	public class ControlGroupTypePropertyMapper : SchedulerControlPropertyMapperBase {
		public ControlGroupTypePropertyMapper(DependencyProperty property, SchedulerControl control)
			: base(property, control) {
		}
		protected override void SubscribeEvents() {
			InnerControl.GroupTypeChanged += OnInnerControlGroupTypeChanged;
			InnerControl.ActiveViewChanged += OnInnerControlActiveViewChanged;
		}
		void OnInnerControlActiveViewChanged(object sender, EventArgs e) {
			UpdateOwnerPropertyValue();
		}
		void OnInnerControlGroupTypeChanged(object sender, EventArgs e) {
			UpdateOwnerPropertyValue();
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			if (InnerControl.GroupType != (SchedulerGroupType)newValue)
				InnerControl.GroupType = (SchedulerGroupType)newValue;
		}
		public override object GetInnerPropertyValue() {
			return InnerControl.GroupType;
		}
		public override void Dispose() {
			base.Dispose();
			InnerControl.GroupTypeChanged -= OnInnerControlGroupTypeChanged;
			InnerControl.ActiveViewChanged -= OnInnerControlActiveViewChanged;
		}
	}
	public class ControlSetStoragePropertyMapper : SchedulerControlPropertyMapperBase {
		public ControlSetStoragePropertyMapper(DependencyProperty property, SchedulerControl control)
			: base(property, control) {
		}
		protected override void SubscribeEvents() {
			this.InnerControl.StorageChanged += InnerControl_StorageChanged;
		}
		void InnerControl_StorageChanged(object sender, EventArgs e) {
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			SchedulerStorage newStorage = newValue as SchedulerStorage;
			InnerControl.Storage = newStorage != null ? newStorage.InnerStorage : null;
		}
		public override object GetInnerPropertyValue() {
			return InnerControl.Storage;
		}
		public override void Dispose() {
			base.Dispose();
			this.InnerControl.StorageChanged -= InnerControl_StorageChanged;
		}
	}
	public class ContainerOptionsBehaviorPropertyMapper : InnerObjectContainerPropertyMapperBase<SchedulerControl, SchedulerOptionsBehavior> {
		public ContainerOptionsBehaviorPropertyMapper(DependencyProperty property, SchedulerControl owner)
			: base(property, owner) {
		}
		public override object GetInnerPropertyValue() {
			return PropertyOwner.InnerControl.OptionsBehavior;
		}
	}
	public class ContainerOptionsCustomizationPropertyMapper : InnerObjectContainerPropertyMapperBase<SchedulerControl, SchedulerOptionsCustomization> {
		public ContainerOptionsCustomizationPropertyMapper(DependencyProperty property, SchedulerControl owner)
			: base(property, owner) {
		}
		public override object GetInnerPropertyValue() {
			return PropertyOwner.InnerControl.OptionsCustomization;
		}
	}
	public class ContainerOptionsViewPropertyMapper : InnerObjectContainerPropertyMapperBase<SchedulerControl, SchedulerOptionsViewBase> {
		public ContainerOptionsViewPropertyMapper(DependencyProperty property, SchedulerControl owner)
			: base(property, owner) {
		}
		public override object GetInnerPropertyValue() {
			return PropertyOwner.InnerControl.OptionsView;
		}
	}
	#endregion
	#region NOT IN USE NOW
	#endregion
}
