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
using System.Threading;
using DevExpress.EasyTest.Framework.Utils;
using DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public class JSPivotGridTestControl : JSStandartTestControl, IGridBase, IGridControlAct {
		public JSPivotGridTestControl(IControlDescription controlDescription) : base(controlDescription) { }
		protected override string ScriptText {
			get {
				return ReadScriptFormResource("DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls.JSPivotGridTestControl.js");
			}
		}
		protected override string JSControlName {
			get { return "PivotGridTestControl_JS"; }
		}
		protected override string RegisterControlType {
			get { return "DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid"; }
		}
		#region IGridControlAct Members
		public void GridActEx(string actionName, int rowIndex, IGridColumn column, string[] paramValues) {
			string fieldAreaName = actionName;
			foreach(string fieldCaption in paramValues) {
				ExecuteFunction("MoveFieldToArea", new object[] { fieldCaption, fieldAreaName });
			}
		}
		#endregion
		#region IGridBase Members
		public IEnumerable<IGridColumn> Columns {
			get { return new IGridColumn[0]; }
		}
		public string GetCellValue(int row, IGridColumn column) {
			return null;
		}
		public int GetRowCount() {
			return 0;
		}
		#endregion
	}
}
