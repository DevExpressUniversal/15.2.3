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

using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public enum GridColumnWidthMode { Manual, AutoFitToContents, AutoFitToGrid }
	public class GridOptions {
		const string xmlEnableBandedRows = "EnableBandedRows";
		const string xmlShowVerticalLines = "ShowVerticalLines";
		const string xmlShowHorizontalLines = "ShowHorizontalLines";
		const string xmlAllowCellMerge = "AllowGridCellMerge";
		const string xmlShowColumnHeaders = "ShowColumnHeaders";
		const string xmlColumnWidthMode = "ColumnWidthMode";
		const string xmlWordWrap = "WordWrap";
		const bool DefaultEnableBandedRows = false;
		const bool DefaultShowVerticalLines = true;
		const bool DefaultShowHorizontalLines = true;
		const bool DefaultAllowCellMerge = false;
		const bool DefaultShowColumnHeaders = true;
		const GridColumnWidthMode DefaultColumnWidthMode = GridColumnWidthMode.AutoFitToGrid;
		const bool DefaultWordWrap = false;
		readonly GridDashboardItem dashboardItem;
		bool enableBandedRows = DefaultEnableBandedRows;
		bool showVerticalLines = DefaultShowVerticalLines;
		bool showHorizontalLines = DefaultShowHorizontalLines;
		bool allowCellMerge = DefaultAllowCellMerge;
		bool showColumnHeaders = DefaultShowColumnHeaders;
		GridColumnWidthMode columnWidthMode = DefaultColumnWidthMode;
		bool wordWrap = DefaultWordWrap;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridOptionsEnableBandedRows"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultEnableBandedRows)
		]
		public bool EnableBandedRows {
			get { return enableBandedRows; }
			set {
				if(enableBandedRows != value) {
					enableBandedRows = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridOptionsShowVerticalLines"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowVerticalLines)
		]
		public bool ShowVerticalLines {
			get { return showVerticalLines; }
			set {
				if(showVerticalLines != value) {
					showVerticalLines = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridOptionsShowHorizontalLines"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowHorizontalLines)
		]
		public bool ShowHorizontalLines {
			get { return showHorizontalLines; }
			set {
				if(showHorizontalLines != value) {
					showHorizontalLines = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridOptionsAllowCellMerge"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultAllowCellMerge)
		]
		public bool AllowCellMerge {
			get { return allowCellMerge; }
			set {
				if(allowCellMerge != value) {
					allowCellMerge = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridOptionsShowColumnHeaders"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowColumnHeaders)
		]
		public bool ShowColumnHeaders {
			get { return showColumnHeaders; }
			set {
				if(showColumnHeaders != value) {
					showColumnHeaders = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridOptionsColumnWidthMode"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultColumnWidthMode)
		]
		public GridColumnWidthMode ColumnWidthMode {
			get { return columnWidthMode; }
			set {
				if(columnWidthMode != value) {
					columnWidthMode = value;
					OnChanged(ChangeReason.View, this, columnWidthMode);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridOptionsWordWrap"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultWordWrap)
		]
		public bool WordWrap {
			get { return wordWrap; }
			set {
				if(wordWrap != value) {
					wordWrap = value;
					OnChanged();
				}
			}
		}
		internal GridOptions(GridDashboardItem dashboardItem) {
			this.dashboardItem = dashboardItem;
		}
		protected internal void SaveToXml(XElement element) {
			if(enableBandedRows != DefaultEnableBandedRows)
				element.Add(new XAttribute(xmlEnableBandedRows, enableBandedRows));
			if(allowCellMerge != DefaultAllowCellMerge)
				element.Add(new XAttribute(xmlAllowCellMerge, allowCellMerge));
			if(showHorizontalLines != DefaultShowHorizontalLines)
				element.Add(new XAttribute(xmlShowHorizontalLines, showHorizontalLines));
			if(showVerticalLines != DefaultShowVerticalLines)
				element.Add(new XAttribute(xmlShowVerticalLines, showVerticalLines));
			if(showColumnHeaders != DefaultShowColumnHeaders)
				element.Add(new XAttribute(xmlShowColumnHeaders, showColumnHeaders));
			if(columnWidthMode != DefaultColumnWidthMode)
				element.Add(new XAttribute(xmlColumnWidthMode, columnWidthMode));
			if(wordWrap != DefaultWordWrap)
				element.Add(new XAttribute(xmlWordWrap, wordWrap));
		}
		protected internal void LoadFromXml(XElement element) {
			string enableBandedRowsString = XmlHelper.GetAttributeValue(element, xmlEnableBandedRows);
			if(!string.IsNullOrEmpty(enableBandedRowsString))
				enableBandedRows = XmlHelper.FromString<bool>(enableBandedRowsString);
			string showHorizontalLinesString = XmlHelper.GetAttributeValue(element, xmlShowHorizontalLines);
			if(!string.IsNullOrEmpty(showHorizontalLinesString))
				showHorizontalLines = XmlHelper.FromString<bool>(showHorizontalLinesString);
			string showVerticalLinesString = XmlHelper.GetAttributeValue(element, xmlShowVerticalLines);
			if(!string.IsNullOrEmpty(showVerticalLinesString))
				showVerticalLines = XmlHelper.FromString<bool>(showVerticalLinesString);
			string allowCellMergeString = XmlHelper.GetAttributeValue(element, xmlAllowCellMerge);
			if(!string.IsNullOrEmpty(allowCellMergeString))
				allowCellMerge = XmlHelper.FromString<bool>(allowCellMergeString);
			string showColumnHeadersString = XmlHelper.GetAttributeValue(element, xmlShowColumnHeaders);
			if(!string.IsNullOrEmpty(showColumnHeadersString))
				showColumnHeaders = XmlHelper.FromString<bool>(showColumnHeadersString);
			string columnWidthModeString = XmlHelper.GetAttributeValue(element, xmlColumnWidthMode);
			if(!string.IsNullOrEmpty(columnWidthModeString))
				columnWidthMode = XmlHelper.FromString<GridColumnWidthMode>(columnWidthModeString);
			string wordWrapString = XmlHelper.GetAttributeValue(element, xmlWordWrap);
			if(!string.IsNullOrEmpty(wordWrapString))
				wordWrap = XmlHelper.FromString<bool>(wordWrapString);
		}
		void OnChanged() {
			if(dashboardItem != null)
				dashboardItem.OnGridOptionsChanged();
		}
		protected void OnChanged(ChangeReason reason, object context, object param) {
			dashboardItem.OnChanged(new ChangedEventArgs(reason, context, param));
		}
	}
}
