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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Reports.UserDesigner.FieldList;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension;
using DevExpress.Diagram.Core;
using System.Collections;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Browsing;
using System.Collections.ObjectModel;
using DevExpress.XtraReports.Native.Summary;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using System.ComponentModel;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class SummaryUITypeEditor : Control {
		static object[] floatData = new object[] { 1.5f, null, 0.3f, -0.4f, 1.5f, 5f, 2.8f, 3.3f };
		static object[] intData = new Object[] { 5, null, 7, -2, 5, 4, 10, 3 };
		static object[] boolData = new Object[] { true, null, false, false, true, true, true, false };
		IDataContextService contextService;
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty BoundFieldProperty;
		public static readonly DependencyProperty SummaryFunctionProperty;
		public static readonly DependencyProperty FormatStringProperty;
		public static readonly DependencyProperty IgnoreNullValuesProperty;
		public static readonly DependencyProperty SummaryRunningProperty;
		public static readonly DependencyProperty PrimarySelectionProperty;
		public static readonly DependencyProperty FieldListNodesProperty;
		static readonly DependencyPropertyKey PreviewDataPropertyKey;
		public static readonly DependencyProperty PreviewDataProperty;
		static readonly DependencyPropertyKey PreviewResultPropertyKey;
		public static readonly DependencyProperty PreviewResultProperty;
		public ICommand SaveCommand { get; private set; }
		public SelectionModel<XRSummary> EditValue {
			get { return (SelectionModel<XRSummary>)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public BindingData BoundField {
			get { return (BindingData)GetValue(BoundFieldProperty); }
			set { SetValue(BoundFieldProperty, value); }
		}
		public SummaryFunc SummaryFunction {
			get { return (SummaryFunc)GetValue(SummaryFunctionProperty); }
			set { SetValue(SummaryFunctionProperty, value); }
		}
		public string FormatString {
			get { return (string)GetValue(FormatStringProperty); }
			set { SetValue(FormatStringProperty, value); }
		}
		public bool IgnoreNullValues {
			get { return (bool)GetValue(IgnoreNullValuesProperty); }
			set { SetValue(IgnoreNullValuesProperty, value); }
		}
		public SummaryRunning SummaryRunning {
			get { return (SummaryRunning)GetValue(SummaryRunningProperty); }
			set { SetValue(SummaryRunningProperty, value); }
		}
		public XRControlModelBase PrimarySelection {
			get { return (XRControlModelBase)GetValue(PrimarySelectionProperty); }
			set { SetValue(PrimarySelectionProperty, value); }
		}
		public IEnumerable<FieldListNodeBase<XRDiagramControl>> FieldListNodes {
			get { return (IEnumerable<FieldListNodeBase<XRDiagramControl>>)GetValue(FieldListNodesProperty); }
			set { SetValue(FieldListNodesProperty, value); }
		}
		public ObservableCollection<object> PreviewData {
			get { return (ObservableCollection<object>)GetValue(PreviewDataProperty); }
			private set { SetValue(PreviewDataPropertyKey, value); }
		}
		public object PreviewResult {
			get { return GetValue(PreviewResultProperty); }
			private set { SetValue(PreviewResultPropertyKey, value); }
		}
		public TypeSpecifics FieldTypeSpecifics { get; set; }
		static SummaryUITypeEditor() {
			DependencyPropertyRegistrator<SummaryUITypeEditor>.New()
				.Register(owner => owner.EditValue, out EditValueProperty, null, d => d.OnEditValueChanged())
				.Register(owner => owner.BoundField, out BoundFieldProperty, null, d => d.OnFieldChanged())
				.Register(owner => owner.SummaryFunction, out SummaryFunctionProperty, SummaryFunc.Avg)
				.Register(owner => owner.FormatString, out FormatStringProperty, null)
				.Register(owner => owner.IgnoreNullValues, out IgnoreNullValuesProperty, false, d => d.OnIgnoreNullValuesChanged())
				.Register(owner => owner.SummaryRunning, out SummaryRunningProperty, SummaryRunning.None)
				.Register(owner => owner.PrimarySelection, out PrimarySelectionProperty, null, d => d.OnPrimarySelectionChanged())
				.Register(d => d.FieldListNodes, out FieldListNodesProperty, null, d => d.OnFieldNodesChanged())
				.RegisterReadOnly(owner => owner.PreviewData, out PreviewDataPropertyKey, out PreviewDataProperty, new ObservableCollection<object>())
				.RegisterReadOnly(owner => owner.PreviewResult, out PreviewResultPropertyKey, out PreviewResultProperty, null)
				.OverrideDefaultStyleKey();
		}
		public SummaryUITypeEditor() {
			SaveCommand = DelegateCommandFactory.Create(Save, CanSave);
		}
		void InitializePreviewData() {
			if(FieldListNodes == null)
				return;
			PreviewData.Clear();
			var previewData = new ArrayList();
			if(FieldTypeSpecifics == TypeSpecifics.Float)
				previewData.AddRange(floatData);
			else if(FieldTypeSpecifics == TypeSpecifics.Bool)
				previewData.AddRange(boolData);
			else
				previewData.AddRange(intData);
			foreach(var item in previewData) {
				if(IgnoreNullValues && (item == null || (item is DBNull)))
					continue;
				PreviewData.Add(item);
			}
			PreviewResult = SummaryHelper.CalcResult(SummaryFunction, PreviewData);
		}
		void OnPrimarySelectionChanged() {
			if(PrimarySelection == null)
				return;
			var control = PrimarySelection.XRObject;
			BoundField = new BindingSettings(control.DataBindings, "Text").Data;
			contextService = PrimarySelection.Report.DesignerHost.GetService(typeof(IDataContextService)) as IDataContextService;
		}
		void OnEditValueChanged() {
			SummaryFunction = EditValue.GetPropertyValue(x => x.Func);
			SummaryRunning = EditValue.GetPropertyValue(x => x.Running);
			FormatString = EditValue.GetPropertyValue(x => x.FormatString);
			IgnoreNullValues = EditValue.GetPropertyValue(x => x.IgnoreNullValues);
			InitializePreviewData();
		}
		void OnFieldNodesChanged() {
			OnFieldChanged();
		}
		void OnIgnoreNullValuesChanged() {
			InitializePreviewData();
		}
		void OnFieldChanged() {
			if(FieldListNodes == null)
				return;
			var pathProvider = new BindingDataHierarchicalPathProvider();
			HierarchicalDataLocator locator = new HierarchicalDataLocator(FieldListNodes, "BindingData", "Children", null, pathProvider);
			var node = locator.FindItemByValue(BoundField) as FieldListNodeBase<XRDiagramControl>;
			FieldTypeSpecifics = node.Return(x => x.TypeSpecifics, () => TypeSpecifics.None);
			InitializePreviewData();
		}
		void Save() {
			DataBindingHelper.SetDataBindingAndInvalidate(PrimarySelection.XRObject, BoundField.GetXRBinding("Text", FormatString));
			EditValue.SetPropertyValue(x => x.Running, SummaryRunning);
			EditValue.SetPropertyValue(x => x.Func, SummaryFunction);
			EditValue.SetPropertyValue(x => x.IgnoreNullValues, IgnoreNullValues);
			EditValue.SetPropertyValue(x => x.FormatString, FormatString);
		}
		bool CanSave() {
			return true;
		}
	}
	public class SummaryPreviewDataToFormatDisplayTextConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
			var value = values[0];
			var formatString = values[1] as string;
			if(value == null)
				return "<null>";
			else if(!string.IsNullOrEmpty(formatString)) {
				try {
					return string.Format(formatString, value);
				} catch { }
			}
			return value.ToString();
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class SummaryToDisplayTextConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var model = value as SelectionModel<XRSummary>;
			if(model == null)
				return value;
			var running = model.GetPropertyValue(x => x.Running);
			var func = model.GetPropertyValue(x => x.Func);
			var ignoreNull = model.GetPropertyValue(x => x.IgnoreNullValues);
			var format = model.GetPropertyValue(x => x.FormatString);
			var summary = new XRSummary(running, func, format) { IgnoreNullValues = ignoreNull };
			var typeConverter = TypeDescriptor.GetConverter(summary);
			return typeConverter.ConvertToString(summary);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class SummaryToDisplayTextConverterExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new SummaryToDisplayTextConverter();
		}
	}
}
