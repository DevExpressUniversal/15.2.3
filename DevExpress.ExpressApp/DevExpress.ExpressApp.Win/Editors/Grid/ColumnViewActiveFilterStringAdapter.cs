#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;
using System.IO;
namespace DevExpress.ExpressApp.Win.Editors {
	public interface IColumnViewActiveFilterStringAdapter {
		string ActiveFilterString { get; set; }
		event EventHandler<EventArgs> ActiveFilterStringChanged;
	}
	public interface ICriteriaOperatorParser {
		CriteriaOperator Parse(string criteriaString);
	}
	public class ObjectSpaceCriteriaOperatorParser : ICriteriaOperatorParser {
		private IObjectSpace objectSpace;
		private bool isAsyncServerMode;
		private ITypeInfo typeInfo;
		public ObjectSpaceCriteriaOperatorParser(IObjectSpace objectSpace, bool isAsyncServerMode, ITypeInfo typeInfo) {
			DevExpress.Utils.Guard.ArgumentNotNull(objectSpace, "objectSpace");
			DevExpress.Utils.Guard.ArgumentNotNull(typeInfo, "typeInfo");
			this.objectSpace = objectSpace;
			this.isAsyncServerMode = isAsyncServerMode;
			this.typeInfo = typeInfo;
		}
		public CriteriaOperator Parse(string criteriaString) {
			CriteriaOperator result = objectSpace.ParseCriteria(criteriaString);
			if(isAsyncServerMode) {
				new AsyncServerModeCriteriaProccessor(typeInfo).Process(result);
			}
			EnumPropertyValueCriteriaProcessor enumParametersProcessor = new EnumPropertyValueCriteriaProcessor(typeInfo);
			enumParametersProcessor.Process(result);
			return result;
		}
	}
	public class SerializedColumnMRUFilter {
		public SerializedColumnMRUFilter(object value) {
			this.ConstantValueString = CriteriaOperator.ToString(new ConstantValue(value));
		}
		public object GetValue(ICriteriaOperatorParser parser) {
			ConstantValue result;
			if(parser != null) {
				result = parser.Parse(ConstantValueString) as ConstantValue;
			}
			else {
				result = CriteriaOperator.Parse(ConstantValueString) as ConstantValue;
			}
			if(!CriteriaOperator.Equals(result, null)) {
				return result.Value;
			}
			return null;
		}
		public string ConstantValueString { get; private set; }
	}
	public class SerializedColumnFilter {
		public SerializedColumnFilter(object value, string serializedCriteria) {
			this.ConstantValueString = CriteriaOperator.ToString(new ConstantValue(value));
			this.SerializedCriteria = serializedCriteria;
		}
		public object GetValue(ICriteriaOperatorParser parser) {
			ConstantValue result;
			if(parser != null) {
				result = parser.Parse(ConstantValueString) as ConstantValue;
			}
			else {
				result = CriteriaOperator.Parse(ConstantValueString) as ConstantValue;
			}
			if(!CriteriaOperator.Equals(result, null)) {
				return result.Value;
			}
			return null;
		}
		public string ConstantValueString { get; private set; }
		public string SerializedCriteria { get; private set; }
	}
	public class XafColumnViewActiveFilterStringAdapter : IColumnViewActiveFilterStringAdapter {
		private ColumnView view;
		private GridControl grid;
		private string activeFilterString;
		private ICriteriaOperatorParser criteriaOperatorParser;
		private bool gridViewActiveFilterCriteriaIsApplied = false;
		private bool lockRowFilterChangedEventHandler = false;
		private Dictionary<GridColumn, List<SerializedColumnMRUFilter>> serializedColumnMRUFilters = new Dictionary<GridColumn, List<SerializedColumnMRUFilter>>();
		private Dictionary<GridColumn, SerializedColumnFilter> serializedColumnFilters = new Dictionary<GridColumn,SerializedColumnFilter>();
		private void FilteredComponentBase_RowFilterChanged(object sender, EventArgs e) {
			if(lockRowFilterChangedEventHandler) {
				return;
			}
			activeFilterString = CriteriaOperator.ToString(view.ActiveFilterCriteria);
			if(ActiveFilterStringChanged != null) {
				ActiveFilterStringChanged(this, EventArgs.Empty);
			}
		}
		private void grid_VisibleChanged(object sender, EventArgs e) {
			TryApplyActiveFilterStringToGridView();
		}
		private void grid_HandleCreated(object sender, EventArgs e) {
			TryApplyActiveFilterStringToGridView();
		}
		private void gridControlDataSourceManager_ControlDataSourceChanging(object sender, EventArgs e) {
			DisposeDeserializedCriteria(); 
			TryApplyActiveFilterStringToGridView();
		}
		private void DisposeDeserializedCriteria() {
			grid.DataSource = null; 
			lockRowFilterChangedEventHandler = true;
			try {
				GetSerializedColumnMRUFilters(view, out serializedColumnMRUFilters, out serializedColumnFilters);
				SetActiveFilterCriteria(view, null, null, null, null);
			}
			finally {
				lockRowFilterChangedEventHandler = false;
			}
			gridViewActiveFilterCriteriaIsApplied = false; 
		}
		private void TryApplyActiveFilterStringToGridView() {
			if(gridViewActiveFilterCriteriaIsApplied || !grid.IsHandleCreated || !grid.Visible) {
				return;
			}
			SetActiveFilterCriteria(view, activeFilterString, serializedColumnMRUFilters, serializedColumnFilters, criteriaOperatorParser);
			serializedColumnMRUFilters = null; 
			serializedColumnFilters = null;
			gridViewActiveFilterCriteriaIsApplied = true;
		}
		private static void SetActiveFilterCriteria(ColumnView columnView, string activeFilterString,
				Dictionary<GridColumn, List<SerializedColumnMRUFilter>> serializedColumnMRUFilters, Dictionary<GridColumn, SerializedColumnFilter> serializedColumnFilters, ICriteriaOperatorParser criteriaOperatorParser) {
			foreach(GridColumn column in columnView.Columns) {
				column.FilterInfo = null;
				column.MRUFilters.Clear();
			}
			if(serializedColumnMRUFilters != null) {
				foreach(GridColumn column in serializedColumnMRUFilters.Keys) {
					foreach(SerializedColumnMRUFilter filterInfo in serializedColumnMRUFilters[column]) {
						column.MRUFilters.Add(new ColumnFilterInfo(column, filterInfo.GetValue(criteriaOperatorParser)));
					}
				}
			}
			CriteriaOperator activeFilterCriteria = (criteriaOperatorParser == null) ? CriteriaOperator.Parse(activeFilterString) : criteriaOperatorParser.Parse(activeFilterString);
			if(!CriteriaOperator.ReferenceEquals(columnView.ActiveFilterCriteria, activeFilterCriteria)) {
				DevExpress.Utils.Guard.ArgumentNotNull(columnView, "columnView");
				bool currentEnabledValue = columnView.ActiveFilterEnabled;
				columnView.ActiveFilterCriteria = activeFilterCriteria; 
				if(serializedColumnFilters != null) {
					foreach(GridColumn column in serializedColumnFilters.Keys) {
						column.FilterInfo = new ColumnFilterInfo(ColumnFilterType.Value, serializedColumnFilters[column].GetValue(criteriaOperatorParser), criteriaOperatorParser.Parse(serializedColumnFilters[column].SerializedCriteria), null);
					}
				}
				columnView.ActiveFilterEnabled = currentEnabledValue;
			}
		}
		private static void GetSerializedColumnMRUFilters(ColumnView view,
				out Dictionary<GridColumn, List<SerializedColumnMRUFilter>> serializedColumnMRUFilters, out Dictionary<GridColumn, SerializedColumnFilter> serializedColumnFilters) {
			DevExpress.Utils.Guard.ArgumentNotNull(view, "view");
			serializedColumnMRUFilters = new Dictionary<GridColumn, List<SerializedColumnMRUFilter>>();
			serializedColumnFilters = new Dictionary<GridColumn, SerializedColumnFilter>();
			foreach(GridColumn column in view.Columns) {
				if(column.FilterInfo != null && column.FilterInfo.Type != ColumnFilterType.None) {
					serializedColumnFilters[column] = new SerializedColumnFilter(column.FilterInfo.Value, column.FilterInfo.FilterString);
				}
				if(column.MRUFilters.Count > 0) {
					List<SerializedColumnMRUFilter> newMRUFilters = new List<SerializedColumnMRUFilter>();
					foreach(ColumnFilterInfo filterInfo in column.MRUFilters) {
						newMRUFilters.Add(new SerializedColumnMRUFilter(filterInfo.Value));
					}
					serializedColumnMRUFilters[column] = newMRUFilters;
				}
			}
		}
		public XafColumnViewActiveFilterStringAdapter(GridControl grid, ColumnView view, ICriteriaOperatorParser criteriaOperatorParser, IGridControlDataSourceAdapter gridControlDataSourceManager) {
			DevExpress.Utils.Guard.ArgumentNotNull(view, "view");
			DevExpress.Utils.Guard.ArgumentNotNull(grid, "grid");
			DevExpress.Utils.Guard.ArgumentNotNull(criteriaOperatorParser, "criteriaOperatorParser");
			DevExpress.Utils.Guard.ArgumentNotNull(gridControlDataSourceManager, "gridControlDataSourceManager");
			this.view = view;
			this.grid = grid;
			this.criteriaOperatorParser = criteriaOperatorParser;
			this.grid.HandleCreated += grid_HandleCreated;
			this.grid.VisibleChanged += grid_VisibleChanged;
			((IFilteredComponentBase)this.grid).RowFilterChanged += FilteredComponentBase_RowFilterChanged;
			gridControlDataSourceManager.ControlDataSourceChanging += gridControlDataSourceManager_ControlDataSourceChanging;
		}
		public string ActiveFilterString {
			get { return activeFilterString; }
			set {
				if(activeFilterString != value) {
					activeFilterString = value; 
					TryApplyActiveFilterStringToGridView();
				}
			}
		}
		public event EventHandler<EventArgs> ActiveFilterStringChanged;
	}
}
