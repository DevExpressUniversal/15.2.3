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
using DevExpress.XtraPivotGrid;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls {
	[Serializable]
	public class PivotGridTestControl : IGridBase, IGridControlAct {
		private PivotGridControl pivotGrid;
		public PivotGridTestControl(PivotGridControl pivotGrid) {
			this.pivotGrid = pivotGrid;
		}
		#region IGridControlAct Members
		public void GridActEx(string actionName, int rowIndex, IGridColumn column, string[] paramValues) {
			PivotArea aria = (PivotArea)Enum.Parse(typeof(PivotArea), actionName);
			foreach(string parameterValue in paramValues) {
				foreach(PivotGridField field in pivotGrid.Fields) {
					if(field.Caption == parameterValue) {
						field.Area = aria;
					}
				}
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
