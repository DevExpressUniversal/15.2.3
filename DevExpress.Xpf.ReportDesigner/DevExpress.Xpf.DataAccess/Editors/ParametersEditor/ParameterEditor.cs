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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Data;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using System.Collections.Specialized;
using System.Windows.Data;
using DevExpress.Mvvm;
using System;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public class ParameterEditor : Control {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty FixedParametersProperty;
		public static readonly DependencyProperty ReportParametersProperty;
		public static readonly DependencyProperty EditorProperty;
		public static readonly DependencyProperty ReportParametersNamesProperty;
		static ParameterEditor() {
			DependencyPropertyRegistrator<ParameterEditor>.New()
				.Register(d => d.EditValue, out EditValueProperty, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.Register(d => d.ReportParametersNames, out ReportParametersNamesProperty, null)
				.Register(d => d.ItemsSource, out ItemsSourceProperty, null, d => d.OnItemsSourceChanged())
				.Register(d => d.FixedParameters, out FixedParametersProperty, true)
				.Register(d => d.ReportParameters, out ReportParametersProperty, null, d => d.OnReportParametersChanged())
				.RegisterAttached((DependencyObject d) => GetEditor(d), out EditorProperty, null, FrameworkPropertyMetadataOptions.Inherits)
				.OverrideDefaultStyleKey()
			;
		}
		IDisposable parametersToParametersNamesCollectionBinding;
		void OnReportParametersChanged() {
			parametersToParametersNamesCollectionBinding.Do(x => x.Dispose());
			parametersToParametersNamesCollectionBinding = CollectionBindingHelper.BindOneWay((ReportParametersNames = new ObservableCollection<string>()), (IParameter x) => x.Name, ReportParameters);
		}
		public static ParameterEditor GetEditor(DependencyObject d) { return (ParameterEditor)d.GetValue(EditorProperty); }
		public static void SetEditor(DependencyObject d, ParameterEditor v) { d.SetValue(EditorProperty, v); }
		public ParameterEditor() {
			SetEditor(this, this);
			this.openExpressionEditorCommand = DelegateCommandFactory.Create(OpenExpressionEditor);
		}
		readonly ICommand openExpressionEditorCommand;
		public ICommand OpenExpressionEditorCommand { get { return openExpressionEditorCommand; } }
		public IParameter EditValue {
			get { return (IParameter)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public IEnumerable<IParameter> ItemsSource {
			get { return (IEnumerable<IParameter>)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
		public ObservableCollection<IParameter> ReportParameters {
			get { return (ObservableCollection<IParameter>)GetValue(ReportParametersProperty); }
			set { SetValue(ReportParametersProperty, value); }
		}
		public ObservableCollection<string> ReportParametersNames {
			get { return (ObservableCollection<string>)GetValue(ReportParametersNamesProperty); }
			set { SetValue(ReportParametersNamesProperty, value); }
		}
		void OnItemsSourceChanged() {
			EditValue = ItemsSource.FirstOrDefault();
		}
		public bool FixedParameters {
			get { return (bool)GetValue(FixedParametersProperty); }
			set { SetValue(FixedParametersProperty, value); }
		}
		void OpenExpressionEditor() {
		}
	}
}
