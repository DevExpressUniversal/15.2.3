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
using System.Linq;
using System.Text;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design {
	public class ControlDisposer : ControlChanger {
		public static void ProcessControl(XRControl control) {
			if(control is XRTableCell)
				new CellDisposer(null).DisposeControl(control);
			else
				new ControlDisposer(null).DisposeControl(control);
		}
		public ControlDisposer(IComponentChangeService changeServ)
			: base(changeServ) {
		}
		public virtual void DisposeControl(XRControl control) {
			if(control != null && !control.IsDisposed)
				control.Dispose();
		}
	}
	public class CellDisposer : ControlDisposer {
		public CellDisposer(IComponentChangeService changeServ)
			: base(changeServ) { 
		}
		public override void DisposeControl(XRControl control) {
			XRTableCell cell = (XRTableCell)control;
			if(cell.IsSingleChild && cell.Row != null && cell.Row.Table != null)
				cell.Row.Table.DeleteRow(cell.Row);
			else
				DisposeJustCell(cell);
		}
		public void DisposeJustCell(XRTableCell cell) {
			XRTableCell neighbor = cell.NextCell;
			if(neighbor == null)
				neighbor = cell.PreviousCell;
			if(neighbor != null) {
				RaiseComponentChanging(neighbor, XRComponentPropertyNames.Weight);
				neighbor.Weight += cell.Weight;
				RaiseComponentChanged(neighbor, XRComponentPropertyNames.Weight, null, null);
			}
			XRTableRow row = cell.Row;
			cell.Dispose();
			if(row != null)
				row.ArrangeCells();
		}
	}
}
