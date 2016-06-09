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

using DevExpress.Data;
using DevExpress.Web.Data;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Web.Mvc.Internal {
	public class MVCxGridLookupWrapperDataProxy: MVCxGridViewDataProxy {
		public MVCxGridLookupWrapperDataProxy(IWebDataOwner owner, IWebControlPageSettings pageSettings, IWebDataEvents events)
			: base(owner, pageSettings, events) {
		}
		protected internal new Dictionary<object, bool> GetFilteredSelectedKeys() {
			return base.GetFilteredSelectedKeys();
		}
		public IEnumerable<object> GetRowValuesByKeys(IEnumerable<object> keys, string[] fieldNames) {
			var customOperationsProvider = DataProvider as GridLookupWrapperCustomOperationsProvider;
			if(customOperationsProvider != null)
				return customOperationsProvider.GetRowValuesByKeys(keys, fieldNames);
			return keys.Select(k => {
				int visibleIndex = DataProvider.FindRowByKey(KeyFieldName, k, false);
				return GetListSourceRowValues(BoundDataProvider.DataController.GetControllerRowHandle(visibleIndex), fieldNames);
			});
		}
		protected override GridViewCustomOperationsProvider CreateCustomOperationsProvider() {
			return new GridLookupWrapperCustomOperationsProvider(this);
		}
	}
	public class GridLookupWrapperCustomOperationsProvider: GridViewCustomOperationsProvider {
		public GridLookupWrapperCustomOperationsProvider(MVCxGridLookupWrapperDataProxy proxy)
			: base(proxy) {
		}
		protected new GridLookupCustomOperationHelper CustomOperationHelper { get { return (GridLookupCustomOperationHelper)base.CustomOperationHelper; } }
		public IEnumerable<object> GetRowValuesByKeys(IEnumerable<object> keys, string[] fieldNames) {
			IEnumerable<object> rowValues = CustomOperationHelper.GetRowValuesByKeys(keys);
			if(rowValues.Count() == 0)
				return new List<object>();
			return ConvertRowValuesToArray(rowValues, fieldNames);
		}
		IEnumerable<object[]> ConvertRowValuesToArray(IEnumerable<object> rowValues, string[] fieldNames) {
			using(var dataController = new ListSourceDataController()) {
				dataController.SetDataSource(rowValues);
				dataController.PopulateColumns();
				for(int i = 0; i < rowValues.Count(); i++)
					yield return fieldNames
						.Select(f => dataController.GetListSourceRowValue(i, f))
						.ToArray();
			}
		}
	}
	public class GridLookupWrapperSelection: GridViewSelection {
		public GridLookupWrapperSelection(MVCxGridLookupWrapperDataProxy webData)
			: base(webData) {
		}
		protected new MVCxGridLookupWrapperDataProxy WebData {
			get { return (MVCxGridLookupWrapperDataProxy)base.WebData; }
		}
		protected internal override List<object> GetSelectedValues(string[] fieldNames, bool searchFilteredValues) {
			if(!WebData.EnableCustomOperations)
				return base.GetSelectedValues(fieldNames, searchFilteredValues);
			Dictionary<object, bool> selectedHash = searchFilteredValues ? WebData.GetFilteredSelectedKeys() : Selected;
			return WebData.GetRowValuesByKeys(selectedHash.Keys, fieldNames).ToList();
		}
	}
	public class GridLookupCustomOperationHelper: GridViewCustomOperationHelper {
		public GridLookupCustomOperationHelper(GridViewModel viewModel)
			: base(viewModel) {
		}
		public GridViewCustomBindingGetRowValuesHandler GetRowValueMethod { get; set; }
		public void ProcessCustomBinding(
				GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
				GridViewCustomBindingGetDataHandler getDataMethod,
				GridViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod,
				GridViewCustomBindingGetGroupingInfoHandler getGroupingInfoMethod,
				GridViewCustomBindingGetUniqueHeaderFilterValuesHandler getUniqueHeaderFilterValuesMethod,
				GridViewCustomBindingGetRowValuesHandler getRowValueMethod) {
			GetRowValueMethod = getRowValueMethod;
			base.ProcessCustomBinding(getDataRowCountMethod, getDataMethod, getSummaryValuesMethod, getGroupingInfoMethod, getUniqueHeaderFilterValuesMethod);
		}
		public IEnumerable<object> GetRowValuesByKeys(IEnumerable<object> keys) {
			if(GetRowValueMethod == null)
				throw new Exception("A required handler is not specified via the ProcessCustomBinding method's getRowValueMethod parameter");
			var args = new GridViewCustomBindingGetRowValuesArgs(GridViewModel, keys);
			GetRowValueMethod(args);
			return args.RowValues;
		}
	}
}
