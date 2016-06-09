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

using DevExpress.Spreadsheet;
using DevExpress.Utils;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Xpf.Spreadsheet {
	#region SpreadsheetControlOptions
	public class SpreadsheetControlOptions : DependencyObject {
		#region Fields
		public static readonly DependencyProperty BehaviorProperty;
		public static readonly DependencyProperty DocumentCapabilitiesProperty;
		public static readonly DependencyProperty SaveProperty;
		public static readonly DependencyProperty ExportProperty;
		public static readonly DependencyProperty ImportProperty;
		public static readonly DependencyProperty ViewProperty;
		public static readonly DependencyProperty PivotTableFieldListProperty;
		public static readonly DependencyProperty CultureProperty;
		public static readonly DependencyProperty RaiseEventsOnModificationsViaAPIProperty;
		DocumentOptions source;
		#endregion
		static SpreadsheetControlOptions() {
			Type ownerType = typeof(SpreadsheetControlOptions);
			BehaviorProperty = DependencyProperty.Register("Behavior", typeof(SpreadsheetBehaviorOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetControlOptions)d).OnBehaviorChanged((SpreadsheetBehaviorOptions)e.OldValue, (SpreadsheetBehaviorOptions)e.NewValue)));
			DocumentCapabilitiesProperty = DependencyProperty.Register("DocumentCapabilities", typeof(SpreadsheetCapabilitiesOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetControlOptions)d).OnDocumentCapabilitiesChanged((SpreadsheetCapabilitiesOptions)e.OldValue, (SpreadsheetCapabilitiesOptions)e.NewValue)));
			SaveProperty = DependencyProperty.Register("Save", typeof(SpreadsheetSaveOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetControlOptions)d).OnSaveChanged((SpreadsheetSaveOptions)e.OldValue, (SpreadsheetSaveOptions)e.NewValue)));
			ExportProperty = DependencyProperty.Register("Export", typeof(SpreadsheetExportOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetControlOptions)d).OnExportChanged((SpreadsheetExportOptions)e.OldValue, (SpreadsheetExportOptions)e.NewValue)));
			ImportProperty = DependencyProperty.Register("Import", typeof(SpreadsheetImportOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetControlOptions)d).OnImportChanged((SpreadsheetImportOptions)e.OldValue, (SpreadsheetImportOptions)e.NewValue)));
			ViewProperty = DependencyProperty.Register("View", typeof(SpreadsheetViewOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetControlOptions)d).OnViewChanged((SpreadsheetViewOptions)e.OldValue, (SpreadsheetViewOptions)e.NewValue)));
			PivotTableFieldListProperty = DependencyProperty.Register("PivotTableFieldList", typeof(SpreadsheetPivotTableFieldListOptions), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetControlOptions)d).OnPivotTableFieldListOptionsChanged((SpreadsheetPivotTableFieldListOptions)e.OldValue, (SpreadsheetPivotTableFieldListOptions)e.NewValue)));
			CultureProperty = DependencyProperty.Register("Culture", typeof(CultureInfo), ownerType,
				new FrameworkPropertyMetadata(CultureInfo.CurrentCulture));
			RaiseEventsOnModificationsViaAPIProperty = DependencyProperty.Register("RaiseEventsOnModificationsViaAPI", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
		}
		public SpreadsheetControlOptions() {
			Behavior = new SpreadsheetBehaviorOptions();
			DocumentCapabilities = new SpreadsheetCapabilitiesOptions();
			Save = new SpreadsheetSaveOptions();
			Export = new SpreadsheetExportOptions();
			Import = new SpreadsheetImportOptions();
			View = new SpreadsheetViewOptions();
			PivotTableFieldList = new SpreadsheetPivotTableFieldListOptions();
		}
		#region Properties
		public SpreadsheetBehaviorOptions Behavior {
			get { return (SpreadsheetBehaviorOptions)GetValue(BehaviorProperty); }
			set { SetValue(BehaviorProperty, value); }
		}
		public SpreadsheetCapabilitiesOptions DocumentCapabilities {
			get { return (SpreadsheetCapabilitiesOptions)GetValue(DocumentCapabilitiesProperty); }
			set { SetValue(DocumentCapabilitiesProperty, value); }
		}
		public SpreadsheetSaveOptions Save {
			get { return (SpreadsheetSaveOptions)GetValue(SaveProperty); }
			set { SetValue(SaveProperty, value); }
		}
		public SpreadsheetExportOptions Export {
			get { return (SpreadsheetExportOptions)GetValue(ExportProperty); }
			set { SetValue(ExportProperty, value); }
		}
		public SpreadsheetImportOptions Import {
			get { return (SpreadsheetImportOptions)GetValue(ImportProperty); }
			set { SetValue(ImportProperty, value); }
		}
		public SpreadsheetViewOptions View {
			get { return (SpreadsheetViewOptions)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public SpreadsheetPivotTableFieldListOptions PivotTableFieldList {
			get { return (SpreadsheetPivotTableFieldListOptions)GetValue(PivotTableFieldListProperty); }
			set { SetValue(PivotTableFieldListProperty, value); }
		}
		public CultureInfo Culture {
			get { return (CultureInfo)GetValue(CultureProperty); }
			set { SetValue(CultureProperty, value); }
		}
		public bool RaiseEventsOnModificationsViaAPI {
			get { return (bool)GetValue(RaiseEventsOnModificationsViaAPIProperty); }
			set { SetValue(RaiseEventsOnModificationsViaAPIProperty, value); }
		}
		#endregion
		void OnBehaviorChanged(SpreadsheetBehaviorOptions oldValue, SpreadsheetBehaviorOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.InnerBehavior);
		}
		void OnDocumentCapabilitiesChanged(SpreadsheetCapabilitiesOptions oldValue, SpreadsheetCapabilitiesOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.DocumentCapabilities);
		}
		void OnSaveChanged(SpreadsheetSaveOptions oldValue, SpreadsheetSaveOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Save);
		}
		void OnExportChanged(SpreadsheetExportOptions oldValue, SpreadsheetExportOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Export);
		}
		void OnImportChanged(SpreadsheetImportOptions oldValue, SpreadsheetImportOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.Import);
		}
		void OnViewChanged(SpreadsheetViewOptions oldValue, SpreadsheetViewOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.InnerView);
		}
		void OnPivotTableFieldListOptionsChanged(SpreadsheetPivotTableFieldListOptions oldValue, SpreadsheetPivotTableFieldListOptions newValue) {
			if (oldValue != null)
				oldValue.Reset();
			if (newValue != null && source != null)
				newValue.SetSource(source.InnerPivotTableFieldList);
		}
		internal void SetSource(DocumentOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.source = source;
			UpdateSourceProperties();
			BindProperties();
			Behavior.SetSource(source.InnerBehavior);
			DocumentCapabilities.SetSource(source.DocumentCapabilities);
			Save.SetSource(source.Save);
			Export.SetSource(source.Export);
			Import.SetSource(source.Import);
			View.SetSource(source.InnerView);
			PivotTableFieldList.SetSource(source.InnerPivotTableFieldList);
		}
		void UpdateSourceProperties() {
			if (Culture != (CultureInfo)GetDefaultValue(CultureProperty))
				source.Culture = Culture;
			if (RaiseEventsOnModificationsViaAPI != (bool)GetDefaultValue(RaiseEventsOnModificationsViaAPIProperty))
				source.Events.RaiseOnModificationsViaAPI = RaiseEventsOnModificationsViaAPI;
		}
		object GetDefaultValue(DependencyProperty property) {
			return property.DefaultMetadata.DefaultValue;
		}
		void BindProperties() {
			BindCultureProperty();
			BindRaiseEventsOnModificationsViaAPIProperty();
		}
		void BindCultureProperty() {
			Binding bind = new Binding("Culture") { Source = source, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, CultureProperty, bind);
		}
		void BindRaiseEventsOnModificationsViaAPIProperty() {
			Binding bind = new Binding("RaiseOnModificationsViaAPI") { Source = source.Events, Mode = BindingMode.TwoWay };
			BindingOperations.SetBinding(this, RaiseEventsOnModificationsViaAPIProperty, bind);
		}
		public void Reset() {
			if (source != null)
				source.Reset();
		}
	}
	#endregion
}
