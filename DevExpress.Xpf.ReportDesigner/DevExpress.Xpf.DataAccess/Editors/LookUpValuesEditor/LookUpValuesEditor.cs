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

using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.DataAccess.Native;
using DevExpress.XtraReports.Parameters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public class LookUpValuesEditor : Control {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty ParameterTypeProperty;
		public static readonly DependencyProperty SelectedLookUpValueProperty;
		static LookUpValuesEditor() {
			DependencyPropertyRegistrator<LookUpValuesEditor>.New()
				.Register(d => d.EditValue, out EditValueProperty, null, (d, e) => d.OnEditValueChanged(e), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.Register(d => d.ParameterType, out ParameterTypeProperty, null)
				.Register(d => d.SelectedLookUpValue, out SelectedLookUpValueProperty, null)
				.OverrideDefaultStyleKey()
				;
		}
		public LookUpValuesEditor() {
			this.addParametersLookUpValueCommand = new DelegateCommand(AddParametersLookUpValue, false);
			this.removeParametersLookUpValueCommand = new DelegateCommand(() => LookUpValues.Remove(SelectedLookUpValue), () => SelectedLookUpValue != null, true);
			this.saveCommand = new DelegateCommand(Save, false);
			lookUpValues = new ObservableCollection<LookUpValue>();
		}
		readonly DelegateCommand removeParametersLookUpValueCommand;
		public ICommand RemoveParametersLookUpValueCommand { get { return removeParametersLookUpValueCommand; } }
		readonly DelegateCommand addParametersLookUpValueCommand;
		public ICommand AddParametersLookUpValueCommand { get { return addParametersLookUpValueCommand; } }
		readonly DelegateCommand saveCommand;
		public ICommand SaveCommand { get { return saveCommand; } }
		public LookUpValueCollection EditValue {
			get { return (LookUpValueCollection)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public Type ParameterType {
			get { return (Type)GetValue(ParameterTypeProperty); }
			set { SetValue(ParameterTypeProperty, value); }
		}
		public LookUpValue SelectedLookUpValue {
			get { return (LookUpValue)GetValue(SelectedLookUpValueProperty); }
			set { SetValue(SelectedLookUpValueProperty, value); }
		}
		readonly ObservableCollection<LookUpValue> lookUpValues;
		public ObservableCollection<LookUpValue> LookUpValues { get { return lookUpValues; } }
		void AddParametersLookUpValue() {
			LookUpValues.Add(new LookUpValue() { Value = ParametersHelper.GetDefaultValue(ParameterType) });
			SetCurrentValue(SelectedLookUpValueProperty, LookUpValues.Last());
		}
		void Save() {
			EditValue.Clear();
			foreach(var item in LookUpValues)
				EditValue.Add(item);
		}
		void OnEditValueChanged(DependencyPropertyChangedEventArgs e) {
			if(e.NewValue == null) return;
			foreach(var lookUpValue in (LookUpValueCollection)e.NewValue)
				LookUpValues.Add(lookUpValue);
		}
	}
}
