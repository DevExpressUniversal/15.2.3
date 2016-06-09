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
using System.Collections.Generic;
using System.Text;
using DevExpress.EasyTest.Framework;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using mshtml;
using System.Threading;
using DevExpress.EasyTest.Framework.Utils;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public class JSTestTable : JSTestControl,
		IGridBase, IGridRowsSelection, IGridAct, IGridCellControlCreation, IGridControlAct {
		private bool isExpanded = false;
		private bool HasTableAction(string methodName) {
			MethodInfo mi = controlReflectWrapper.GetMethodInfo(methodName);
			return mi != null;
		}
		protected virtual void ExpandGroups() {
			if(!isExpanded && HasTableAction("ExpandGroups")) {
				isExpanded = true;
				if(HasTableAction("IsGrouped")) {
					bool? isGrouped = InvokeMethod("IsGrouped", delegate(object[] args) {
						throw new EasyTestException("The 'IsGrouped' method is not implemented");
					}, new object[] { null }) as bool?;
					if(isGrouped.HasValue && !isGrouped.Value) {
						return;
					}
				}
				ExecuteTableAction("ExpandGroups");
			}
		}
		public JSTestTable(WebCommandAdapter adapter, Object testControl)
			: base(adapter, testControl) { }
		public override T FindInterface<T>() {
			T result = base.FindInterface<T>();
			if(typeof(T) == typeof(IGridCellControlCreation)) {
				return result;
			}
			if(typeof(T) == typeof(IGridControlAct)) {
				return result;
			}
			if(typeof(T) == typeof(IGridRowsSelection)) {
				object isSupportSelection = InvokeMethod("IsSupportSelection", delegate(object[] args) { return null; }, new object[] { });
				if(isSupportSelection != null && !((bool)isSupportSelection)) {
					return default(T);
				}
			}
			Type interfaceType = typeof(T);
			foreach(MethodInfo mi in interfaceType.GetMethods()) {
				string methodName = mi.Name;
				if(methodName == "GridAct") {
					methodName = "ExecuteAction";
				}
				if(methodName == "CheckGridAct") {
					methodName = "CheckAction";
				}
				if(methodName == "GetRowCount") {
					methodName = "GetTableRowCount";
				}
				if(methodName == "get_Columns") {
					methodName = "GetColumnsCaptions";
				}
				if(methodName == "ActEx") {
					continue;
				}
				if(controlReflectWrapper.GetMethodInfo(methodName) == null) {
					return default(T);
				}
			}
			return result;
		}
		public virtual object ExecuteTableAction(string action, params object[] parameters) {
			return InvokeMethod(action,
				delegate(object[] args) {
					throw new AdapterException("The '" + action + "' method is not found.");
				},
				parameters);
		}
		#region IGridBase Members
		public virtual IEnumerable<IGridColumn> Columns {
			get {
				string captions = (string)InvokeMethod("GetColumnsCaptions", delegate(object[] args) { return null; }, new object[] { });
				EasyTestTracer.Tracer.LogText("GetColumnsCaptions result: " + (string.IsNullOrEmpty(captions) ? "<null>" : captions));
				int index = 0;
				foreach(string columnCaption in captions.Split(';')) {
					yield return new TestGridColumn(columnCaption, index);
					index++;
				}
			}
		}
		public virtual string GetCellValue(int row, IGridColumn column) {
			if(column == null) {
				throw new ArgumentNullException("column");
			}
			ExpandGroups();
			EasyTestTracer.Tracer.LogText(string.Format(">GetCellValue: column = '{0}'", column.Caption));
			string result = null;
			if(this.ClassName == "SchedulerEditor") {
				EasyTestTracer.Tracer.LogText("ClassName is SchedulerEditor");
				result = (string)InvokeMethod("GetCellValue", delegate(object[] args) { return null; }, new object[] { row, ((TestGridColumn)column).ColumnIndex });
			}
			else {
				result = (string)InvokeMethod("GetCellValue", delegate(object[] args) { return null; }, new object[] { row, column.Caption });
			}
			EasyTestTracer.Tracer.LogText(string.Format("<GetCellValue: column = '{0}', result '{1}'", column.Caption, (string.IsNullOrEmpty(result) ? "<null>" : result)));
			return result;
		}
		public virtual int GetRowCount() {
			ExpandGroups();
			int result = (int)InvokeMethod("GetTableRowCount", delegate(object[] args) { return -1; }, new object[] { });
			EasyTestTracer.Tracer.LogText("GetRowCount result: " + result);
			return result;
		}
		#endregion
		#region IGridRowsSelection Members
		public virtual void ClearSelection() {
			InvokeMethod("ClearSelection", delegate(object[] args) { return null; }, new object[] { });
		}
		public virtual void SelectRow(int rowIndex) {
			InvokeMethod("SelectRow", delegate(object[] args) { return null; }, new object[] { rowIndex });
		}
		public virtual void UnselectRow(int rowIndex) {
			InvokeMethod("UnselectRow", delegate(object[] args) { return null; }, new object[] { rowIndex });
		}
		public virtual bool IsRowSelected(int rowIndex) {
			object result = InvokeMethod("IsRowSelected", delegate(object[] args) { return null; }, new object[] { rowIndex });
			if(result == null) {
				return false;
			}
			else {
				return (bool)result;
			}
		}
		#endregion
		#region IGridAct Members
		public virtual void GridAct(string actionName, int rowIndex, IGridColumn column) {
			string composedActionName = string.IsNullOrEmpty(Name) ? actionName : Name + "." + actionName;
			if(!string.IsNullOrEmpty(actionName) && adapter.IsControlExist(TestControlType.Action, composedActionName)) {
				adapter.CreateTestControl(TestControlType.Action, composedActionName).GetInterface<IControlAct>().Act(null);
			}
			else {
				int columnIndex = -1;
				if(column != null) {
					columnIndex = ((TestGridColumn)column).ColumnIndex;
				}
				InvokeMethod("ExecuteAction", delegate(object[] args) { return null; }, new object[] { actionName, rowIndex, columnIndex });
			}
		}
		public void CheckGridAct(string actionName, int rowIndex, IGridColumn column, bool isInlineOnly) {
			string composedActionName = string.IsNullOrEmpty(Name) ? actionName : Name + "." + actionName;
			if(!isInlineOnly && !string.IsNullOrEmpty(actionName) && adapter.IsControlExist(TestControlType.Action, composedActionName)) {
				if(!adapter.CreateTestControl(TestControlType.Action, composedActionName).GetInterface<IControlEnabled>().Enabled) {
					throw new AdapterOperationException(string.Format("The '{0}' action of '{1}' table is not enabled", actionName, Name));
				}
			}
			else {
				int columnIndex = -1;
				if(column != null) {
					columnIndex = ((TestGridColumn)column).ColumnIndex;
				}
				InvokeMethod("CheckAction", delegate(object[] args) { return null; }, new object[] { actionName, rowIndex, columnIndex });
			}
		}
		#endregion
		#region IGridCellControlCreation Members
		public virtual void BeginEdit(int row) {
			InvokeMethod("BeginEdit", delegate(object[] args) { return null; }, new object[] { row });
		}
		public virtual ITestControl CreateCellControl(int row, IGridColumn column) {
			object control = null;
			if(this.ClassName == "SchedulerEditor") {
				control = InvokeMethod("GetCellControl", delegate(object[] args) { return null; }, new object[] { row, ((TestGridColumn)column).ColumnIndex });
			}
			else {
				control = InvokeMethod("GetCellControl", delegate(object[] args) { return null; }, new object[] { row, ((TestGridColumn)column).Caption });
			}
			if(control == null) {
				return null;
			}
			return new JSTestControl(adapter, control);
		}
		public virtual void EndEdit() {
			InvokeMethod("EndEdit", delegate(object[] args) { return null; }, new object[] { });
		}
		#endregion
		#region IGridControlAct Members
		public void GridActEx(string actionName, int rowIndex, IGridColumn column, string[] paramValues) {
			if(actionName == "Action" && paramValues != null && paramValues.Length == 1) {
				actionName = paramValues[0];
				paramValues = new string[] { "" };
			}
			int columnIndex = -1;
			if(column != null) {
				columnIndex = ((TestGridColumn)column).ColumnIndex;
			}
			object objParamValues = null;
			if(paramValues != null) {
				if(paramValues.Length == 1) {
					objParamValues = paramValues[0];
				}
				else {
					objParamValues = (new List<string>(paramValues)).ConvertAll<object>(delegate(string a) { return a; }).ToArray();
				}
			}
			InvokeDelegate errorDelegate = delegate(object[] args) {
				throw new AdapterOperationException(String.Format("The table control does not supported the '{0}' action", actionName));
			};
			if(rowIndex == -1 && columnIndex == -1) {
				InvokeMethod(actionName, errorDelegate, new object[] { objParamValues });
			}
			else {
				InvokeMethod(actionName, errorDelegate, new object[] { objParamValues, rowIndex, columnIndex });
			}
		}
		#endregion
	}
	public class TestGridColumn : IGridColumn {
		string caption;
		private int columnIndex;
		public TestGridColumn(string caption, int columnIndex) {
			this.caption = caption;
			this.columnIndex = columnIndex;
		}
		public int ColumnIndex {
			get { return columnIndex; }
		}
		#region IGridColumn Members
		public string Caption {
			get { return caption; }
		}
		public bool Visible {
			get { return true; }
		}
		#endregion
	}
}
