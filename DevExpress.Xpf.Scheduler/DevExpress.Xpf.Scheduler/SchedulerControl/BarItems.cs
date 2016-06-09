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

using DevExpress.Utils.Commands;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler.Commands;
using DevExpress.Xpf.Scheduler.Commands;
using System;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Ribbon;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Office.Internal;
using System.ComponentModel;
namespace DevExpress.Xpf.Scheduler {
	public partial class SchedulerControl {
		readonly SchedulerControlBarCommandManager commandManager;
		protected SchedulerCommandId EmptyCommandId { get { return SchedulerCommandId.None; } }
		protected internal SchedulerControlAccessor Accessor { get { return this.accessor; } }
		protected internal SchedulerControlBarCommandManager CommandManager { get { return commandManager; } }
		#region BarManager
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("SchedulerControlBarManager")]
#endif
		public BarManager BarManager {
			get { return (BarManager)GetValue(BarManagerProperty); }
			set { SetValue(BarManagerProperty, value); }
		}
		public static readonly DependencyProperty BarManagerProperty = CreateBarManagerProperty();
		static DependencyProperty CreateBarManagerProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, BarManager>("BarManager", null, (d, e) => d.OnBarManagerChanged(e.OldValue, e.NewValue), null);
		}
		#endregion
		#region Ribbon
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			set { SetValue(RibbonProperty, value); }
		}
		public static readonly DependencyProperty RibbonProperty = CreateRibbonProperty();
		static DependencyProperty CreateRibbonProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerControl, RibbonControl>("Ribbon", null, (d, e) => d.OnRibbonChanged(e.OldValue, e.NewValue), null);
		}
		#endregion
		protected internal virtual void OnBarManagerChanged(BarManager oldValue, BarManager newValue) {
			if (oldValue != null) {
				CommandManager.UnsubscribeBarItemsEvents(oldValue);
				UnsubscribeBarManagerEvents(oldValue);
			}
			if (newValue != null)
				SubscribeBarManagerEvents(newValue);
			CommandManager.UpdateBarItemsDefaultValues();
		}
		protected internal void OnRibbonChanged(RibbonControl oldValue, RibbonControl newValue) {
			if (oldValue != null)
				UnsubscribeRibbonEvents(oldValue);
			if (newValue != null)
				SubscribeRibbonEvents(newValue);
			CommandManager.UpdateRibbonItemsDefaultValues();
		}
		protected internal virtual void SubscribeBarManagerEvents(BarManager barManager) {
			barManager.Loaded += OnBarManagerLoaded;
		}
		protected internal virtual void UnsubscribeBarManagerEvents(BarManager barManager) {
			barManager.Loaded -= OnBarManagerLoaded;
		}
		protected internal virtual void SubscribeRibbonEvents(RibbonControl ribbon) {
			ribbon.Loaded += OnRibbonLoaded;
		}
		protected internal virtual void UnsubscribeRibbonEvents(RibbonControl ribbon) {
			ribbon.Loaded -= OnRibbonLoaded;
		}
		void OnBarManagerLoaded(object sender, RoutedEventArgs e) {
			CommandManager.UpdateBarItemsDefaultValues();
			UnsubscribeBarManagerEvents(BarManager);
			OnUpdateUI(this, EventArgs.Empty);
		}
		void OnRibbonLoaded(object sender, RoutedEventArgs e) {
			commandManager.UpdateRibbonItemsDefaultValues();
			UnsubscribeRibbonEvents(Ribbon);
			OnUpdateUI(this, EventArgs.Empty);
		}
		void OnUpdateUI(object sender, EventArgs e) {
			RaiseUpdateUI();
			try {
				CommandManager.UpdateBarItemsState();
				CommandManager.UpdateRibbonItemsState();
			} catch (Exception ex) {
				if (!HandleException(ex))
					throw;
			}			
		}	   
	}
}
