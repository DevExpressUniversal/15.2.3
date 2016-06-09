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
using System.Linq;
using System.Text;
using System.Windows.Data;
namespace DevExpress.Xpf.Editors {
	public delegate void RaisePropertyChangedDelegate<T>(T oldValue, T newValue);
	public class DateTimePickerPart : INotifyPropertyChanged {
		DateTimePickerData selectedItem;
		IEnumerable<DateTimePickerData> items;
		bool isLooped;
		bool isAnimated;
		bool isExpanded;
		int visibleItemsCount;
		bool useTransitions;
		bool isEnabled = true;
		public DateTimePickerData SelectedItem {
			get { return selectedItem; }
			set { SetValue("SelectedItem", ref selectedItem, value); }
		}
		public int VisibleItemsCount {
			get { return visibleItemsCount; }
			set { SetValue("VisibleItemsCount", ref visibleItemsCount, value); }
		}
		public IEnumerable<DateTimePickerData> Items {
			get { return items; }
			set { SetValue("Items", ref items, value); }
		}
		public bool IsAnimated {
			get { return isAnimated; }
			set { SetValue("IsAnimated", ref isAnimated, value, IsAnimatedChanged); }
		}
		public bool IsExpanded {
			get { return isExpanded; }
			set { SetValue("IsExpanded", ref isExpanded, value, IsExpandedChanged); }
		}
		public bool IsLooped {
			get { return isLooped; }
			set { SetValue("IsLooped", ref isLooped, value); }
		}
		public bool UseTransitions {
			get { return useTransitions; }
			set { SetValue("UseTransitions", ref useTransitions, value); }
		}
		public bool IsEnabled {
			get { return isEnabled; }
			set { SetValue("IsEnabled", ref isEnabled, value); }
		}
		public event EventHandler<AnimatedChangedEventArgs> AnimatedChanged;
		public event EventHandler<ExpandedChangedEventArgs> ExpandedChanged;
		void RaiseAnimatedChanged(bool value) {
			if (AnimatedChanged != null)
				AnimatedChanged(this, new AnimatedChangedEventArgs(value));
		}
		void RaiseExpandedChanged(bool value) {
			if (ExpandedChanged != null)
				ExpandedChanged(this, new ExpandedChangedEventArgs(value));
		}
		void IsAnimatedChanged(bool oldValue, bool newValue) {
			RaiseAnimatedChanged(newValue);
		}
		void IsExpandedChanged(bool oldValue, bool newValue) {
			RaiseExpandedChanged(newValue);
		}
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		void SetValue<T>(string propertyName, ref T field, T newValue, RaisePropertyChangedDelegate<T> raiseChangedDelegate) {
			if (object.Equals(field, newValue))
				return;
			T oldValue = field;
			field = newValue;
			RaisePropertyChanged(propertyName);
			if (raiseChangedDelegate != null)
				raiseChangedDelegate(oldValue, newValue);
		}
		void SetValue<T>(string propertyName, ref T field, T newValue) {
			SetValue(propertyName, ref field, newValue, null);
		}
	}
}
