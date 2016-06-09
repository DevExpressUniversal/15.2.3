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
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design;
using System.ComponentModel;
namespace DevExpress.XtraReports.Native {
	public class DesignTableModifier : TableModifier {
		IDesignerHost designerHost;
		IComponentChangeService componentChangeService;
		public DesignTableModifier(XRTable table, IDesignerHost designerHost)
			: base(table) {
			this.designerHost = designerHost;
			componentChangeService = (IComponentChangeService)designerHost.GetService(typeof(IComponentChangeService));
		}
		protected override void AssignRowWeight(XRTableRow row, double weight) {
			XRControlDesignerBase.RaiseComponentChanging(this.componentChangeService, row, XRComponentPropertyNames.Weight);
			base.AssignRowWeight(row, weight);
			XRControlDesignerBase.RaiseComponentChanged(this.componentChangeService, row, XRComponentPropertyNames.Weight, null, null);
		}
		protected override void AssignTableHeight(float height) {
			XRControlDesignerBase.RaiseComponentChanging(this.componentChangeService, table, XRComponentPropertyNames.Height);
			base.AssignTableHeight(height);
			XRControlDesignerBase.RaiseComponentChanged(this.componentChangeService, table, XRComponentPropertyNames.Height, null, null);
		}
		protected override void AssignTableWidth(float width) {
			XRControlDesignerBase.RaiseComponentChanging(this.componentChangeService, table, XRComponentPropertyNames.Width);
			base.AssignTableWidth(width);
			XRControlDesignerBase.RaiseComponentChanged(this.componentChangeService, table, XRComponentPropertyNames.Width, null, null);
		}
		protected override void AssignTableTop(float top) {
			XRControlDesignerBase.RaiseComponentChanging(this.componentChangeService, table, XRComponentPropertyNames.Location);
			base.AssignTableTop(top);
			XRControlDesignerBase.RaiseComponentChanged(this.componentChangeService, table, XRComponentPropertyNames.Location, null, null);
		}
		public static void DeleteControl(XRControl control) {
			if(control != null && !control.IsDisposed)
				control.Dispose();
		}
		protected override void DeleteOrdinaryRow(XRTableRow row) {
			DeleteControl(row);
			base.DeleteOrdinaryRow(row);
		}
		protected override void DeleteSingleRow() {
			DeleteControl(table);
			base.DeleteSingleRow();
		}
		protected override void InsertRowInTable(int index, XRTableRow row) {
			XRControlDesignerBase.RaiseCollectionChanging(table, this.componentChangeService);
			base.InsertRowInTable(index, row);
			XRControlDesignerBase.RaiseCollectionChanged(table, this.componentChangeService);
		}
		protected override XRTableRow CreateRow() {
			XRTableRow row = base.CreateRow();
			DesignToolHelper.AddToContainer(designerHost, row);
			return row;
		}
		protected override XRTableCell[] CloneRowCells(XRTableRow baseRow) {
			XRTableCell[] tableCells = baseRow.CloneCells();
			foreach(IComponent cell in tableCells) {
				DesignToolHelper.AddToContainer(designerHost, cell);
			}
			return tableCells;
		}
		protected override void AddControl(XRControl control, XRControl newControl) {
			DesignToolHelper.AddToContainer(designerHost, newControl);
			CollectionChanging(control);
			base.AddControl(control, newControl);
			CollectionChanged(control);
		}
		protected override void RemoveControl(XRControl control) {
			System.Collections.IList list = DesignToolHelper.GetAssociatedComponents(control, designerHost);
			foreach(IComponent associatedComponent in list) {
				this.componentChangeService.OnComponentChanging(associatedComponent, null);
			}
			designerHost.DestroyComponent(control);
		}
		protected override void CollectionChanging(XRControl control) {
			XRControlDesignerBase.RaiseCollectionChanging(control, this.componentChangeService);
		}
		protected override void CollectionChanged(XRControl control) {
			XRControlDesignerBase.RaiseCollectionChanged(control, this.componentChangeService);
		}
	}
}
