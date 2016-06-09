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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Core;
using System.Collections;
using DevExpress.Utils.Commands;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.Xpf.Scheduler.Native;
using System.ComponentModel;
using DevExpress.Mvvm.UI;
using System.Windows.Data;
using System.Windows.Input;
namespace DevExpress.Xpf.Scheduler.UI {
	public partial class RemindersControl : UserControl {
		public RemindersControl() {
			InitializeComponent();
			lbReminders.SelectedItems.CollectionChanged += new NotifyCollectionChangedEventHandler(OnRemindersSelectionChanged);
			DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
#if DEBUGTEST
			Loaded += new RoutedEventHandler(RemindersForm_Loaded);
#endif
		}
		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(DataContext != null) {
				Binding binding = new Binding("SelectedReminders");
				binding.Mode = BindingMode.TwoWay;
				this.SetBinding(RemindersControl.SelectedRemindersProperty, binding);
			}
		}
#if DEBUGTEST
		void RemindersForm_Loaded(object sender, RoutedEventArgs e) {
			if(DevExpress.Xpf.Scheduler.Tests.TestFormHelper.IsTest)
				DevExpress.Xpf.Scheduler.Tests.TestFormHelper.ActiveViewModels.Add(DataContext, this);
		}
#endif
		public static readonly DependencyProperty SelectedRemindersProperty = DependencyPropertyHelper.RegisterProperty<RemindersControl, ObservableCollection<ReminderInfo>>("SelectedReminders", null);
		public ObservableCollection<ReminderInfo> SelectedReminders {
			get { return (ObservableCollection<ReminderInfo>)GetValue(SelectedRemindersProperty); }
			set { SetValue(SelectedRemindersProperty, value); }
		}
		void OnRemindersSelectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ObservableCollection<ReminderInfo> list = new ObservableCollection<ReminderInfo>();
			for(int i = 0; i < lbReminders.SelectedItems.Count; i++) {
				list.Add(lbReminders.SelectedItems[i] as ReminderInfo);
			}
			SelectedReminders = list;
		}
	}
	public class RemindersDoubleClickEventArgsConverter : IEventArgsConverter {
		public object Convert(object sender, object args) {
#if !SL
			MouseButtonEventArgs mouseButtonEventArgs = args as MouseButtonEventArgs;
			if(mouseButtonEventArgs == null)
				return false;
			return mouseButtonEventArgs.ChangedButton == MouseButton.Left;
#else
			return false;
#endif
		}
	}
}
