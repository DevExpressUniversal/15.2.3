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

using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardExport;
using DevExpress.DashboardWin.Forms.Export.Groups;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Native.Printing;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Forms.Export {
	public class PrintPreviewOptionsForm : OptionsForm {
		public PrintPreviewOptionsForm(string name, string type, IExportOptionsOwner options)
			: base(name, type, DashboardExportFormat.PDF, options) {
		}
		protected override string GetTitle(string name, DashboardExportFormat format) {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.PrintPreviewOptionsFormCaption);
		}
		protected override string GetSubmitButtonText() {
			return base.GetSubmitButtonText();
		}
		protected override OptionsGroup CreateGroup(string type, DashboardExportFormat format) {
			switch(type) {
				case DashboardType:
					return new DashboardPrintPreviewOptionsGroup();
				case DashboardItemType.Grid:
					return new GridPintPreviewOptionsGroup();
				case DashboardItemType.Pivot:
					return new PivotPintPreviewOptionsGroup();
				case DashboardItemType.Chart:
				case DashboardItemType.ScatterChart:
				case DashboardItemType.RangeFilter:
					return new ChartPintPreviewOptionsGroup();
				case DashboardItemType.ChoroplethMap:
				case DashboardItemType.GeoPointMap:
				case DashboardItemType.BubbleMap:
				case DashboardItemType.PieMap:
					return new MapPintPreviewOptionsGroup();
				case DashboardItemType.Pie:
				case DashboardItemType.Gauge:
				case DashboardItemType.Card:
					return new KPIPintPreviewOptionsGroup();
				case DashboardItemType.Image:
				case DashboardItemType.Text:
					return new TitleOptionsGroup();
				default:
					throw new Exception("There is not ExportOptionsGroup for this type");
			};
		}
	}
}
