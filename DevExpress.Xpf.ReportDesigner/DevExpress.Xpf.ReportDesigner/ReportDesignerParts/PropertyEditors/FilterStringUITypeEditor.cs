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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.DataAccess.Sql;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	#region Inner classes
	public class FilterColumnsProvider : IFilteredComponent {
		public FilterColumnsProvider(List<IBoundProperty> columns) {
			this.columns = new List<FilterColumn>();
			foreach(var column in columns)
				this.columns.Add(new FilterColumn() { FieldName = column.Name, ColumnType = column.Type });
		}
		List<FilterColumn> columns;
		IEnumerable<FilterColumn> IFilteredComponent.CreateFilterColumnCollection() {
			return columns;
		}
		EventHandler propertiesChanged;
		event EventHandler IFilteredComponentBase.PropertiesChanged {
			add { propertiesChanged += value; }
			remove { propertiesChanged -= value; }
		}
		CriteriaOperator IFilteredComponentBase.RowCriteria { get; set; }
		EventHandler rowFilterChanged;
		event EventHandler IFilteredComponentBase.RowFilterChanged {
			add { rowFilterChanged += value; }
			remove { rowFilterChanged -= value; }
		}
	}
	public class ColumnData : IBoundProperty {
		public ColumnData(PropertyDescriptor property) {
			this.property = property;
		}
		readonly PropertyDescriptor property;
		List<IBoundProperty> IBoundProperty.Children { get { throw new NotImplementedException(); } }
		string IBoundProperty.DisplayName { get { return property.DisplayName; } }
		bool IBoundProperty.HasChildren { get { return false; } }
		bool IBoundProperty.IsAggregate { get { return false; } }
		bool IBoundProperty.IsList { get { return false; } }
		string IBoundProperty.Name { get { return property.Name; } }
		IBoundProperty IBoundProperty.Parent { get { return null; } }
		Type IBoundProperty.Type { get { return property.PropertyType; } }
	}
	#endregion
	public class FilterStringUITypeEditor : Control {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty ReportProperty;
		public static readonly DependencyProperty FilterCriteriaProperty;
		static readonly DependencyPropertyKey SourceControlPropertyKey;
		public static readonly DependencyProperty SourceControlProperty;
		static FilterStringUITypeEditor() {
			DependencyPropertyRegistrator<FilterStringUITypeEditor>.New()
				.Register(d => d.EditValue, out EditValueProperty, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.Register(d => d.Report, out ReportProperty, null, d => d.OnReportChanged())
				.Register(d => d.FilterCriteria, out FilterCriteriaProperty, null, d => d.OnFilterCriteriaChanged())
				.RegisterReadOnly(d => d.SourceControl, out SourceControlPropertyKey, out SourceControlProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		readonly Locker locker;
		readonly List<IBoundProperty> columnsData;
		ErrorsEvaluatorCriteriaValidator criteriaValidator;
		public FilterStringUITypeEditor() {
			locker = new Locker();
			columnsData = new List<IBoundProperty>();
		}
		public string EditValue {
			get { return (string)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public XtraReport Report {
			get { return (XtraReport)GetValue(ReportProperty); }
			set { SetValue(ReportProperty, value); }
		}
		void OnReportChanged() {
			if(Report == null || !(Report is XtraReport))
				return;
			var dataSource = ReportHelper.GetEffectiveDataSource(Report);
			if(dataSource == null)
				return;
			var dataBrowserHelper = new DataBrowserHelper();
			var propertyDescriptor = dataBrowserHelper.GetListItemProperties(dataSource).Find(Report.DataMember, true);
			if(propertyDescriptor == null)
				return;
			var properties = dataBrowserHelper.GetListItemProperties(dataSource, new PropertyDescriptor[] { propertyDescriptor });
			foreach(PropertyDescriptor property in properties)
				columnsData.Add(new ColumnData(property));
			criteriaValidator = new ErrorsEvaluatorCriteriaValidator(columnsData);
			locker.DoLockedActionIfNotLocked(() => {
				FilterCriteria = CriteriaOperator.Parse(EditValue);
				if(!IsFilterCriteriaValid(FilterCriteria))
					FilterCriteria = null;
			});
			SourceControl = new FilterColumnsProvider(columnsData);
		}
		public CriteriaOperator FilterCriteria {
			get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); }
			set { SetValue(FilterCriteriaProperty, value); }
		}
		void OnFilterCriteriaChanged() {
			locker.DoLockedAction(() => EditValue = CriteriaOperator.ToString(FilterCriteria));
		}
		public object SourceControl {
			get { return GetValue(SourceControlProperty); }
			private set { SetValue(SourceControlPropertyKey, value); }
		}
		bool IsFilterCriteriaValid(CriteriaOperator filterCriteria) {
			criteriaValidator.Validate(filterCriteria);
			return criteriaValidator.Count == 0;
		}
	}
}
