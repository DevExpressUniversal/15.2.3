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
using System.ComponentModel;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class SyncDataBindingsPropertyBehavior : Behavior<BaseEdit> {
		public class BehaviorParameters {
			public SelectionModel<IDiagramItem> SelectionModel { get; private set; }
			public string XRBindingName { get; private set; }
			public string PropertyName { get; private set; }
			public BehaviorParameters(SelectionModel<IDiagramItem> selectionModel, string xrBindingName, string propertyName) {
				this.SelectionModel = selectionModel;
				this.XRBindingName = xrBindingName;
				this.PropertyName = propertyName;
			}
		}
		public static readonly DependencyProperty ParametersProperty =
			DependencyProperty.Register(
				"Parameters",
				typeof(BehaviorParameters),
				typeof(SyncDataBindingsPropertyBehavior),
				new PropertyMetadata(null, ParametersChangedCallback));
		public BehaviorParameters Parameters {
			get {
				return (BehaviorParameters)GetValue(ParametersProperty);
			}
			set {
				SetValue(ParametersProperty, value);
			}
		}
		static void ParametersChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SyncDataBindingsPropertyBehavior behavior = (SyncDataBindingsPropertyBehavior)d;
			BehaviorParameters oldParameters = e.OldValue as BehaviorParameters;
			if(oldParameters != null && oldParameters.SelectionModel != null) {
				oldParameters.SelectionModel.PropertyChanged -= behavior.SelectionModel_PropertyChanged;
			}
			BehaviorParameters newParameters = e.NewValue as BehaviorParameters;
			if(behavior.AssociatedObject != null) {
				behavior.SyncEditorWithModel(newParameters);
			}
			if(newParameters != null && newParameters.SelectionModel != null) {
				newParameters.SelectionModel.PropertyChanged += behavior.SelectionModel_PropertyChanged;
			}
		}
		void SelectionModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			SyncEditorWithModel(Parameters);
		}
		protected override void OnAttached() {
			base.OnAttached();
			SyncEditorWithModel(Parameters);
			AssociatedObject.EditValueChanged += AssociatedObject_EditValueChanged;
		}
		protected override void OnDetaching() {
			AssociatedObject.EditValueChanged -= AssociatedObject_EditValueChanged;
			base.OnDetaching();
		}
		void AssociatedObject_EditValueChanged(object sender, RoutedEventArgs e) {
			SyncModelWithEditor(Parameters);
		}
		internal void SyncEditorWithModel(BehaviorParameters parameters) {
			object propertyValue = null;
			bool enableEditor = false;
			if(parameters != null) {
				SelectionModel<BindingSettings> bindingSettingsModel = GetBindingSettingsModel(parameters.SelectionModel, parameters.XRBindingName);
				PropertyDescriptor pd = bindingSettingsModel.GetProperties(null)[parameters.PropertyName];
				if(pd != null) {
					propertyValue = pd.GetValue(bindingSettingsModel);
					enableEditor = true;
				}
			}
			if(!object.Equals(AssociatedObject.EditValue, propertyValue)) {
				AssociatedObject.EditValue = propertyValue;
			}
			AssociatedObject.IsEnabled = enableEditor;
		}
		internal void SyncModelWithEditor(BehaviorParameters parameters) {
			if(parameters != null) {
				SelectionModel<BindingSettings> bindingSettingsModel = GetBindingSettingsModel(parameters.SelectionModel, parameters.XRBindingName);
				PropertyDescriptor pd = bindingSettingsModel.GetProperties(null)[parameters.PropertyName];
				pd.SetValue(bindingSettingsModel, AssociatedObject.EditValue);
			}
		}
		SelectionModel<BindingSettings> GetBindingSettingsModel(SelectionModel<IDiagramItem> model, string xrBindingName) {
			PropertyDescriptor pdDataBindings = PropertyDescriptorHelper.GetPropertyDescriptors(model)["DataBindings"];
			var bindingCollectionModel = pdDataBindings.GetValue(model);
			var pdBindingCollection = PropertyDescriptorHelper.GetPropertyValueDescriptors(model, pdDataBindings)[xrBindingName];
			SelectionModel<BindingSettings> bindingSettingsModel = (SelectionModel<BindingSettings>)pdBindingCollection.GetValue(bindingCollectionModel);
			return bindingSettingsModel;
		}
	}
}
