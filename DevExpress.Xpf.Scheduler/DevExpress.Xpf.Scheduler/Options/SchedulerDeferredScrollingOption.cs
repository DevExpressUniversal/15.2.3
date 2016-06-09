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
using DevExpress.XtraScheduler.Native;
using System.Windows;
namespace DevExpress.Xpf.Scheduler {
	public class SchedulerDeferredScrollingOption : 
#if !SL
		Freezable,
#else
		DependencyObject,
#endif
		ISchedulerDeferredScrollingOption {
		#region Allow
		public bool Allow {
			get { return (bool)GetValue(AllowProperty); }
			set { SetValue(AllowProperty, value); }
		}
		public static readonly DependencyProperty AllowProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerDeferredScrollingOption, bool>("Allow", false, (d, e) => d.OnAllowChanged(e.OldValue, e.NewValue), null);
		void OnAllowChanged(bool oldValue, bool newValue) {
			RaisePropertyChanged("Allow", oldValue, newValue);
		}	
		#endregion
#if !SL
		protected override bool FreezeCore(bool isChecking) {
			if (isChecking)
				return false;
			return base.FreezeCore(isChecking);
		}
		protected override void CloneCore(Freezable sourceFreezable) {
			if (sourceFreezable.GetType() != this.GetType())
				return;
			base.CloneCore(sourceFreezable);
		}
		protected override Freezable CreateInstanceCore() {
			SchedulerDeferredScrollingOption clone = new SchedulerDeferredScrollingOption();
			clone.Allow = Allow;
			return clone;
		}
#endif        
		#region ISchedulerDeferredScrollingOption.Changed
		DevExpress.Utils.Controls.BaseOptionChangedEventHandler propertyChanged;
		event DevExpress.Utils.Controls.BaseOptionChangedEventHandler ISchedulerDeferredScrollingOption.Changed {
			add { propertyChanged += value; }
			remove { propertyChanged -= value; }
		}
		protected virtual void RaisePropertyChanged(string propertyName, object oldValue, object newValue) {
			if (propertyChanged == null)
				return;
			propertyChanged(this, new DevExpress.Utils.Controls.BaseOptionChangedEventArgs(propertyName, oldValue, newValue));
		}
		#endregion
	}
}
