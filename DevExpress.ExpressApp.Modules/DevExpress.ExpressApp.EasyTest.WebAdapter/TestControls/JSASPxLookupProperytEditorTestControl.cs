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
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public class JSASPxLookupProperytEditorTestControl : JSTestControl {
		public JSASPxLookupProperytEditorTestControl(WebCommandAdapter adapter, object testControl)
			: base(adapter, testControl) {
		}
		public override string Text {
			get {
				return base.Text;
			}
			set {
				string searchActionName = this.InvokeMethod("GetSearchActionName",
									delegate(object[] args) {
										throw new EasyTestException("The 'GetSearchActionName' method is not implemented");
									},
									new object[] { }
				).ToString();
				Act(null);
				if(adapter.FindControl(TestControlType.Action, searchActionName) != null) {
					ITestControl findAction = adapter.CreateTestControl(TestControlType.Action, searchActionName);
					findAction.GetInterface<IControlAct>().Act(value);
				}
				ITestControl grig = adapter.CreateTestControl(TestControlType.Table, "");
				IEnumerable<IGridColumn> columns = ((IGridBase)grig.GetInterface<IGridBase>()).Columns;
				IGridColumn gridColumnResult = null;
				foreach(IGridColumn gridColumn in columns) {
					if(gridColumn.Visible) {
						gridColumnResult = gridColumn;
						break;
					}
				}
				grig.GetInterface<IGridAct>().GridAct("", 0, gridColumnResult);
			}
		}
	}
}
