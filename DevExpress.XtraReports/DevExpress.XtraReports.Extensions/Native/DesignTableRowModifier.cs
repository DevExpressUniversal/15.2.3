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

using System.ComponentModel.Design;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native {
	public class DesignTableRowModifier : TableRowModifier {
		IDesignerHost designerHost;
		IComponentChangeService componentChangeService;
		public DesignTableRowModifier(XRTableRow row, IDesignerHost designerHost)
			: base(row) {
			this.designerHost = designerHost;
			componentChangeService = (IComponentChangeService)designerHost.GetService(typeof(IComponentChangeService));
		}
		public override void DeleteCell(XRTableCell cell) {
			DesignTableModifier.DeleteControl(cell);
			base.DeleteCell(cell);
		}
		protected override void AssignCellWeight(XRTableCell cell, double weight) {
			XRControlDesignerBase.RaiseComponentChanging(componentChangeService, cell, XRComponentPropertyNames.Weight);
			base.AssignCellWeight(cell, weight);
			XRControlDesignerBase.RaiseComponentChanged(componentChangeService, cell, XRComponentPropertyNames.Weight, null, null);
		}
		protected override void PreProcessInsertCell(XRTableCell newCell) {
			DesignToolHelper.AddToContainer(designerHost, newCell);
		}
		protected override void InsertCell(XRTableCell newCell, int index) {
			XRControlDesignerBase.RaiseCollectionChanging(row, this.componentChangeService);
			base.InsertCell(newCell, index);
			XRControlDesignerBase.RaiseCollectionChanged(row, this.componentChangeService);
		}
		public override void SetCellRange(XRTableCell[] tableCells) {
			foreach(XRTableCell tableCell in tableCells) {
				DesignToolHelper.AddToContainer(designerHost, tableCell);
			}
			XRControlDesignerBase.RaiseCollectionChanging(row, this.componentChangeService);
			base.SetCellRange(tableCells);
			XRControlDesignerBase.RaiseCollectionChanged(row, this.componentChangeService);
		}
	}
}
