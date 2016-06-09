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
using DevExpress.Utils.Controls;
namespace DevExpress.Xpf.Scheduler.Native {
	public abstract class InnerObjectContainerPropertyMapperBase<U, T> : DependencyPropertyMapperBase
		where U : DependencyObject
		where T : BaseOptions {
		SchedulerOptionsBase<T> currentValue;
		protected InnerObjectContainerPropertyMapperBase(DependencyProperty property, DependencyObject owner)
			: base(property, owner) {
		}
		protected U PropertyOwner { get { return (U)Owner; } }
		protected override void UpdateOwnerPropertyValue() {
			T newInnerObject = (T)GetInnerPropertyValue();
			SchedulerOptionsBase<T> externObject = (SchedulerOptionsBase<T>)Owner.GetValue(TargetProperty);
			if(externObject == null)
				return;
			if(!Object.Equals(externObject.InnerObject, newInnerObject)) {
				externObject.DetachExistingInnerObject();
				externObject.AttachExistingInnerObject(newInnerObject);
			}
		}
		protected override void SetInnerPropertyValue(object oldValue, object newValue) {
			SchedulerOptionsBase<T> newPropertyValue = (SchedulerOptionsBase<T>)newValue;
			SchedulerOptionsBase<T> oldProperty = (SchedulerOptionsBase<T>)oldValue;
			T newInnerProperty = (T)GetInnerPropertyValue();
			OnOptionsAssigned(oldProperty, newPropertyValue, newInnerProperty);
		}
		protected internal virtual void OnOptionsAssigned(SchedulerOptionsBase<T> oldValue, SchedulerOptionsBase<T> newValue, T innerOptionsObject) {
			if(oldValue == newValue)
				return;
			if(oldValue != null)
				oldValue.DetachExistingInnerObject();
			if(newValue != null)
				newValue.AttachExistingInnerObject(innerOptionsObject);
			this.currentValue = newValue;
		}
		protected override void Deactivate() {
			base.Deactivate();
			if(this.currentValue != null)
				this.currentValue.DetachExistingInnerObject();
		}
	}
}
