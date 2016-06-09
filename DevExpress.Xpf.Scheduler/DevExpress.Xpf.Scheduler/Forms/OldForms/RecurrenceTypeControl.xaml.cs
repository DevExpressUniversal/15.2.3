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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.UI {
	public partial class RecurrenceTypeControl : UserControl, INotifyPropertyChanged {
		public RecurrenceTypeControl() {
			InitializeComponent();
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected virtual void RaiseOnPropertyChange(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#region PatternRecurrenceInfo
		public RecurrenceInfo RecurrenceInfo {
			get { return (RecurrenceInfo)GetValue(RecurrenceInfoProperty); }
			set { SetValue(RecurrenceInfoProperty, value); }
		}
		public static readonly DependencyProperty RecurrenceInfoProperty = CreatePatternRecurrenceInfoProperty();
		static DependencyProperty CreatePatternRecurrenceInfoProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceTypeControl, RecurrenceInfo>("RecurrenceInfo", null, (d, e) => d.OnPatternRecurrenceInfoChanged(e.OldValue, e.NewValue), null);
		}
		void OnPatternRecurrenceInfoChanged(RecurrenceInfo oldValue, RecurrenceInfo newValue) {
		}
		#endregion
		#region Controller
		public RecurrenceVisualController Controller {
			get { return (RecurrenceVisualController)GetValue(ControllerProperty); }
			set { SetValue(ControllerProperty, value); }
		}
		public static readonly DependencyProperty ControllerProperty = CreateControllerProperty();
		static DependencyProperty CreateControllerProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceTypeControl, RecurrenceVisualController>("Controller", null, (d, e) => d.OnRecurrenceElementChanged(e.OldValue, e.NewValue), null);
		}
		void OnRecurrenceElementChanged(RecurrenceVisualController oldValue, RecurrenceVisualController newValue) {
			SetRecurrenceType(Controller.RecurrenceType);
		}
		#endregion
		#region IsReadOnly
		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		public static readonly DependencyProperty IsReadOnlyProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<RecurrenceTypeControl, bool>("IsReadOnly", false, (d,e) => d.OnIsReadOnlyChanged(e.OldValue, e.NewValue));
		protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue) {
			brdRecurrenceType.IsHitTestVisible = !newValue;
		}
		#endregion
		protected internal virtual void SetRecurrenceType(Nullable<RecurrenceType> type) {
			switch (type) {
				default:
				case RecurrenceType.Daily:
					DailyRadioButton.IsChecked = true;
					break;
				case RecurrenceType.Weekly:
					WeeklyRadioButton.IsChecked = true;
					break;
				case RecurrenceType.Monthly:
					MonthlyRadioButton.IsChecked = true;
					break;
				case RecurrenceType.Yearly:
					YearlyRadioButton.IsChecked = true;
					break;
			}
		}
		private void DailyRadioButton_Checked(object sender, RoutedEventArgs e) {
			Controller.RecurrenceType = RecurrenceType.Daily;
		}
		private void WeeklyRadioButton_Checked(object sender, RoutedEventArgs e) {
			Controller.RecurrenceType = RecurrenceType.Weekly;
		}
		private void MonthlyRadioButton_Checked(object sender, RoutedEventArgs e) {
			Controller.RecurrenceType = RecurrenceType.Monthly;
		}
		private void YearlyRadioButton_Checked(object sender, RoutedEventArgs e) {
			Controller.RecurrenceType = RecurrenceType.Yearly;
		}
	}
}
