#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardWin.Native.Printing;
using System.Linq;
using DevExpress.DashboardWin.Forms.Export;
using System.Drawing;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardItemGroupViewer : DashboardItemViewer {
		protected override string ExportItemType { get { return ExportOptionsForm.DashboardType; } }
		protected override ExtendedReportOptions GetDefaultExportOptions() {
			return DashboardViewer.GetDefaultReportOptions(DashboardItemCaption, CaptionViewModel.ShowCaption, null);
		}
		protected override Control GetViewControl() {
			return null;
		}
		protected override void PrepareViewControl() {
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = ItemContainer.CalcClientArea();
		}
		protected override ExportInfo CreateExportInfo(DashboardReportOptions exportOptions) {
			DashboardLayoutControlGroup layoutGroup = DashboardViewer.FindLayoutControlGroup(DashboardItemName);
			ItemStateCollection itemStateCollection = new ItemStateCollection();
			foreach(DashboardLayoutControlItem item in layoutGroup.Items.Cast<DashboardLayoutControlItem>())
				itemStateCollection.AddRange(((IDashboardExportItem)item.ItemViewer).GetItemStateCollection());
			ExportInfo exportInfo = new ExportInfo();
			exportInfo.GroupName = DashboardItemName;
			exportInfo.Mode = DashboardExportMode.EntireDashboard;
			exportInfo.ViewerState = new ViewerState();
			exportInfo.ViewerState.ItemsState = itemStateCollection;
			exportInfo.ViewerState.Size = new Size(GetControlClientArea(this).Width, GetControlClientArea(this).Height);
			exportInfo.ExportOptions = exportOptions;
			return exportInfo;
		}
	}
}
