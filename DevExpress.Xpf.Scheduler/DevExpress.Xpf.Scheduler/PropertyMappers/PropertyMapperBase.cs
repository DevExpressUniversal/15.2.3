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
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.Xpf.Scheduler.Native {
	public enum PropertyMapperSyncDirection { InnerToOwner,  OwnerToInner };
	public interface IDependencyPropertyMapper : IDisposable {
		DependencyProperty TargetProperty { get; }
		DependencyObject Owner { get; }
		void Activate(PropertyMapperSyncDirection direction);
		void Deactivate();
		void UpdateInnerPropertyValue(object oldValue, object newValue);
	}
	public abstract class DependencyPropertyMapperBase : IDependencyPropertyMapper {
		bool isActivated;
		readonly DependencyObject owner;
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		readonly DependencyProperty targetProperty;
		protected DependencyPropertyMapperBase(DependencyProperty property, DependencyObject owner) {
			Guard.ArgumentNotNull(property, "property");
			Guard.ArgumentNotNull(owner, "owner");
			this.targetProperty = property;
			this.owner = owner;
		}
		public DependencyProperty TargetProperty { get { return targetProperty; } }
		public DependencyObject Owner { get { return owner; } }
		public abstract object GetInnerPropertyValue();
		protected abstract void SetInnerPropertyValue(object oldValue, object newValue);
		protected virtual void SubscribeEvents() {
		}
		protected virtual void UnsubscribeEvents() {
		}
		#region IDependencyPropertyMapper members
		void IDependencyPropertyMapper.Activate(PropertyMapperSyncDirection direction) {
			Activate(direction);
		}
		void IDependencyPropertyMapper.Deactivate() {
			Deactivate();
		}
		void IDependencyPropertyMapper.UpdateInnerPropertyValue(object oldValue, object newValue) {
			SetInnerPropertyValue(oldValue, newValue);
		}
		#endregion
		protected virtual void UpdateOwnerPropertyValue() {
			object newValue = GetInnerPropertyValue();
			object oldValue = Owner.GetValue(TargetProperty);
			if(!Object.Equals(newValue, oldValue))
				Owner.SetValue(TargetProperty, newValue);
		}
		protected virtual void Activate(PropertyMapperSyncDirection direction) {
			if (direction == PropertyMapperSyncDirection.InnerToOwner) {
				if (TargetProperty.DefaultMetadata.DefaultValue == Owner.GetValue(TargetProperty))
					UpdateOwnerPropertyValue();
			} else
				ApplyOwnerProperty();
			XtraSchedulerDebug.Assert(!isActivated);
			if (!this.isActivated)
				SubscribeEvents();
			this.isActivated = true;
		}
		protected virtual void ApplyOwnerProperty() {
			object newValue = Owner.GetValue(TargetProperty);
			SetInnerPropertyValue(null, newValue);
		}
		protected virtual void Deactivate() {
			UnsubscribeEvents();
			this.isActivated = false;
		}
		public virtual void Dispose() {
			Deactivate();
		}
	}
}
