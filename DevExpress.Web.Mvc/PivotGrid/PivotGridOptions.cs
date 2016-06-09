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
using DevExpress.Web.ASPxPivotGrid;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxPivotGrid;
	using DevExpress.Web.ASPxPivotGrid.Export;
	using DevExpress.Web;
	using DevExpress.PivotGrid.Printing;
	using DevExpress.Utils.Serializing.Helpers;
	using DevExpress.Utils;
	using System.Collections;
	public class MVCxPivotGridWebOptionsLoadingPanel: PivotGridWebOptionsLoadingPanel {
		SettingsLoadingPanel settings;
		ImageProperties image;
		LoadingPanelStyle style;
		public MVCxPivotGridWebOptionsLoadingPanel()
			: base(null) {
			this.settings = new SettingsLoadingPanel(null);
			this.image = new ImageProperties();
			this.style = new LoadingPanelStyle();
		}
		protected override SettingsLoadingPanel Settings { get { return settings; } }
		protected override ImageProperties ImageInternal { get { return image; } }
		protected override LoadingPanelStyle StyleInternal { get { return style; } }
	}
	public class MVCxPivotGridWebOptionsData: PivotGridWebOptionsData {
		public MVCxPivotGridWebOptionsData()
			: base(null, null) {
			AllowCrossGroupVariation = true;
			AutoExpandGroups = DefaultBoolean.Default;
			CaseSensitive = true;
			DrillDownMaxRowCount = XtraPivotGrid.PivotDrillDownDataSource.AllRows;
		}
		public new bool CaseSensitive { get; set; }
		public new DefaultBoolean AutoExpandGroups { get; set; }
		public new ICustomObjectConverter CustomObjectConverter { get; set; }
		public new bool AllowCrossGroupVariation { get; set; }
		public new XtraPivotGrid.DataFieldUnboundExpressionMode DataFieldUnboundExpressionMode { get; set; }
		public new bool FilterByVisibleFieldsOnly { get; set; }
	}
	public class MVCxPivotGridWebOptionsDataField: PivotGridWebOptionsDataField {
		public MVCxPivotGridWebOptionsDataField()
			: base(null) {
			AreaIndex = -1;
			RowHeaderWidth = XtraPivotGrid.PivotGridFieldBase.DefaultWidth;
			FieldNaming = XtraPivotGrid.DataFieldNaming.FieldName;
			ColumnValueLineCount = 1;
			RowValueLineCount = 1;
		}
		public new XtraPivotGrid.PivotDataArea Area { get; set; }
		public new int AreaIndex { get; set; }
		public new int DataFieldAreaIndex { get; set; }
		public new bool DataFieldVisible { get; set; }
		public new int RowHeaderWidth { get; set; }
		public new string Caption { get; set; }
		public new XtraPivotGrid.DataFieldNaming FieldNaming { get; set; }
		public new int ColumnValueLineCount { get; set; }
		public new int RowValueLineCount { get; set; }
	}
	public class MVCxPivotGridExportSettings {
		WebPivotGridOptionsPrint optionsPrint;
		WebPrintAppearance printStyles;
		public MVCxPivotGridExportSettings() {
			this.optionsPrint = new WebPivotGridOptionsPrint(null);
			this.printStyles = new WebPrintAppearance();
		}
		public WebPivotGridOptionsPrint OptionsPrint { get { return optionsPrint; } }
		public WebPrintAppearance StylesPrint { get { return printStyles; } }
		public EventHandler<WebCustomExportHeaderEventArgs> CustomExportHeader { get; set;}
		public EventHandler<WebCustomExportFieldValueEventArgs> CustomExportFieldValue { get; set;}
		public EventHandler<WebCustomExportCellEventArgs> CustomExportCell { get; set;}
		public EventHandler BeforeExport { get; set; }
	}
	public class MVCxWebPrintAppearance: WebPrintAppearance {
		public MVCxWebPrintAppearance()
			: base() {
		}
		public virtual void CopyFrom(WebPrintAppearance source) {
			if (source == null)
				return;
			Cell = source.Cell;
			Cell = source.Cell;
			CustomTotalCell = source.CustomTotalCell;
			FieldHeader = source.FieldHeader;
			FieldValue = source.FieldValue;
			FieldValueGrandTotal = source.FieldValueGrandTotal;
			FieldValueTotal = source.FieldValueTotal;
			GrandTotalCell = source.GrandTotalCell;
			Lines = source.Lines;
			TotalCell = source.TotalCell;
		}
	}
	public class MVCxPivotGridWebOptionsView: PivotGridWebOptionsView {
		public MVCxPivotGridWebOptionsView()
			: base(null, null, "OptionsView") {
			ViewBag = new Hashtable();
		}
		protected Hashtable ViewBag { get; private set; }
		protected override T GetViewBagProperty<T>(string name, T value) {
			return ViewBag.Contains(name) ? (T)ViewBag[name] : value;
		}
		protected override void SetViewBagProperty<T>(string name, T defaultValue, T value) {
			ViewBag[name] = value;
		}
	}
}
