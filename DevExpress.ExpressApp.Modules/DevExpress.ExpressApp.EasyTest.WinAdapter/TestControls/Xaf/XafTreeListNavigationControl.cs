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
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Xaf {
	class XafTreeListNavigationControl : TestControlInterfaceImplementerBase<TreeListNavigationControl>,
		IControlAct, IControlReadOnlyText, IControlActionItems {
		private void FindItems(string parameterValue, List<string> foundValues) {
			ITestControl actionTreeList = TestControl; 
			IGridBase grid = actionTreeList.GetInterface<IGridBase>();
			List<IGridColumn> columns = new List<IGridColumn>(grid.Columns);
			if(columns.Count > 0) {
				int rowCount = grid.GetRowCount();
				string[] cellValues = new string[rowCount];
				for(int i = 0; i < rowCount; i++) {
					cellValues[i] = grid.GetCellValue(i, columns[0]);
				}
				foreach(string cellValue in cellValues) {
					if(cellValue == parameterValue) {
						foundValues.Add(cellValue);
					}
				}
				if(foundValues.Count == 0) {
					foreach(string cellValue in cellValues) {
						if(cellValue.EndsWith("." + parameterValue)) {
							foundValues.Add(cellValue);
						}
					}
				}
			}
		}
		private void Execute(string parameterValue) {
			Guard.ArgumentNotNull(parameterValue, "parameterValue");
			EasyTestTracer.Tracer.LogText("XafTreeListNavigationControl.Execute: " + parameterValue);
			ITestControl actionTreeList = TestControl; 
			IGridBase grid = actionTreeList.GetInterface<IGridBase>();
			List<IGridColumn> columns = new List<IGridColumn>(grid.Columns);
			if(columns.Count > 0) {
				string rows = "";
				int rowCount = grid.GetRowCount();
				bool isFound = false;
				for(int i = 0; i < rowCount; i++) {
					string cellValue = grid.GetCellValue(i, columns[0]);
					rows += "'" + (string.IsNullOrEmpty(cellValue) ? "string.IsNullOrEmpty" : cellValue) + "'; ";
					if(cellValue == parameterValue) {
						isFound = true;
						actionTreeList.GetInterface<IGridDoubleClick>().DoubleClickToCell(i, columns[0]);
						break;
					}
				}
				if(!isFound) {
					throw new AdapterOperationException(String.Format("Cannot find '{0}' item. Actual items: {1}", parameterValue, rows));
				}
			}
			else {
				throw new AdapterOperationException(String.Format("Cannot execute operation: grid.Columns.Count is '{0}'", columns.Count));
			}
		}
		public XafTreeListNavigationControl(TreeListNavigationControl control) : base(control) { }
		#region IControlAct Members
		public void Act(string value) {
			List<string> items = new List<string>();
			FindItems(value, items);
			if(items.Count == 0) {
				throw new AdapterOperationException(String.Format("Cannot find the '{0}' action's '{1}' parameter", TestControl.Name, value));
			}
			if(items.Count > 1) {
				throw new WarningException(String.Format("The action's '{0}' parameter '{1}' must be unique. Use one of following variants: {2}", TestControl.Name, value, string.Join(", ", items.ToArray())));
			}
			Execute(items[0]);
		}
		#endregion
		#region IControlReadOnlyText Members
		public string Text {
			get {
				ITestControl actionTreeList = TestControl; 
				IGridBase grid = actionTreeList.GetInterface<IGridBase>();
				List<IGridColumn> columns = new List<IGridColumn>(grid.Columns);
				if(columns.Count > 0) {
					TreeListNode node = control.FocusedNode;
					string result = "";
					while(node != null) {
						result = node.GetDisplayText(((TreeListGridColumn)columns[0]).Column) + "." + result;
						node = node.ParentNode;
					}
					return result.TrimEnd('.');
				}
				throw new NotImplementedException();
			}
		}
		#endregion
		#region IControlActionItems Members
		public bool IsVisible(string value) {
			List<string> items = new List<string>();
			FindItems(value, items);
			if (items.Count > 1) {
				throw new WarningException(String.Format("The action's '{0}' parameter '{1}' must be unique. Use one of following variants: {2}", TestControl.Name, value, string.Join(", ", items.ToArray())));
			}
			return items.Count > 0;
		}
		public bool IsEnabled(string value) {
			return IsVisible(value);
		}
		#endregion
	}
}
