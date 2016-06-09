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
using System.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardWin.Forms.Export.Groups;
using DevExpress.DashboardWin.Native.Printing;
namespace DevExpress.DashboardWin.Forms.Export {
	public class ExportOptionsForm : OptionsForm {
		public ExportOptionsForm(string name, string type, DashboardExportFormat format, IExportOptionsOwner options):
			base(name, type, format, options) {
		}
		protected override string GetTitle(string name, DashboardExportFormat format) {
			string caption = null;
			switch(format) {
				case DashboardExportFormat.PDF:
					caption = DashboardLocalizer.GetString(DashboardStringId.ActionExportToPdf);
					break;
				case DashboardExportFormat.Excel:
					caption = DashboardLocalizer.GetString(DashboardStringId.ActionExportToExcel);
					break;
				default:
					caption = DashboardLocalizer.GetString(DashboardStringId.ActionExportToImage);
					break;
			}
			if(name.Length > 0)
				caption = String.Format(DashboardLocalizer.GetString(DashboardStringId.ActionExportTemplate), caption, name);
			return caption;
		}
		protected override string GetSubmitButtonText() {
			return String.Format("&{0}", DashboardLocalizer.GetString(DashboardStringId.ButtonExport));
		}
		protected override OptionsGroup CreateGroup(string type, DashboardExportFormat format) {
			switch(format) {
				case DashboardExportFormat.Excel:
					return new DataOptionsGroup();
				case DashboardExportFormat.Image:
					switch(type) {
						case DashboardItemType.Image:
						case DashboardItemType.Text:
							return new SimplyImageOptionsGroup();
						default:
							return new ImageOptionsGroup();
					}
				default:
					switch(type) {
						case DashboardType:
							return new DashboardOptionsGroup();
						case DashboardItemType.Grid:
							return new GridOptionsGroup();
						case DashboardItemType.Pivot:
							return new PivotOptionsGroup();
						case DashboardItemType.Chart:
						case DashboardItemType.ScatterChart:
						case DashboardItemType.RangeFilter:
							return new ChartOptionsGroup();
						case DashboardItemType.ChoroplethMap:
						case DashboardItemType.GeoPointMap:
						case DashboardItemType.BubbleMap:
						case DashboardItemType.PieMap:
							return new MapOptionsGroup();
						case DashboardItemType.Pie:
						case DashboardItemType.Gauge:
						case DashboardItemType.Card:
							return new KPIOptionsGroup();
						case DashboardItemType.Image:
						case DashboardItemType.Text:
							return new SimplyDocumentOptionsWithScaleModeGroup();
						default:
							throw new Exception("There is not ExportOptionsGroup for this type");
					};
			}
		}
	}
}
