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
using System.Diagnostics;
using DevExpress.Office;
using DevExpress.Utils;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Spreadsheet;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region WorkbookProperties
	public class WorkbookProperties : SpreadsheetNotificationOptions, ICloneable<WorkbookProperties>, ISupportsCopyFrom<WorkbookProperties> {
		#region Fields
		readonly DocumentModel workbook;
		bool useR1C1ReferenceStyle;
		bool refreshAllOnLoading;
		bool saveBackup;
		bool showBordersOfUnselectedTables;
		bool showIncAnnotations;
		bool hidePivotFieldList;
		bool showPivotChartFilter;
		bool supportNaturalLanguagesFormulaInput;
		int builtInFunctionGroupCount;
		List<int> sheetIdTable;
		DisplayObjectsOptions displayObjects;
		CalculationOptions calculationOptions;
		WorkbookProtectionOptions protection;
		List<WorkbookWindowProperties> workbookWindowPropertiesList;
		string codeName;
		int defaultThemeVersion;
		#endregion
		public WorkbookProperties(DocumentModel workbook) {
			this.workbook = workbook;
			Initialize();
		}
		#region Properties
		public bool UseR1C1ReferenceStyle {
			get { return useR1C1ReferenceStyle; }
			set {
				if (useR1C1ReferenceStyle == value)
					return;
				workbook.BeginUpdate();
				try {
					useR1C1ReferenceStyle = value;
					workbook.ApplyChanges(DocumentModelChangeActions.ResetHeaderContent);
				}
				finally {
					workbook.EndUpdate();
				}
			}
		}
		public bool RefreshAllOnLoading { get { return refreshAllOnLoading; } set { refreshAllOnLoading = value; } }
		public bool SaveBackup { get { return saveBackup; } set { saveBackup = value; } }
		public bool ShowBordersOfUnselectedTables { get { return showBordersOfUnselectedTables; } set { showBordersOfUnselectedTables = value; } }
		public bool ShowIncAnnotations { get { return showIncAnnotations; } set { showIncAnnotations = value; } }
		public bool ShowPivotChartFilter { get { return showPivotChartFilter; } set { showPivotChartFilter = value; } }
		public bool SupportNaturalLanguagesFormulaInput { get { return supportNaturalLanguagesFormulaInput; } set { supportNaturalLanguagesFormulaInput = value; } }
		public int BuiltInFunctionGroupCount { get { return builtInFunctionGroupCount; } set { builtInFunctionGroupCount = value; } }
		protected internal List<int> SheetIdTable { get { return this.sheetIdTable; } }
		public DisplayObjectsOptions DisplayObjects { get { return displayObjects; } set { displayObjects = value; } }
		public CalculationOptions CalculationOptions { get { return calculationOptions; } }
		public WorkbookProtectionOptions Protection { get { return this.protection; } }
		public List<WorkbookWindowProperties> WorkbookWindowPropertiesList { get { return workbookWindowPropertiesList; } set { workbookWindowPropertiesList = value; } }
		public bool HidePivotFieldList {
			get { return hidePivotFieldList; }
			set {
				if (hidePivotFieldList == value)
					return;
				hidePivotFieldList = value;
				workbook.InnerApplyChanges(DocumentModelChangeActions.ResetPivotTableFieldsPanelVisibility | DocumentModelChangeActions.RaiseUpdateUI);
			}
		}
		public string CodeName {
			get { return codeName; }
			set {
				if(string.IsNullOrEmpty(value))
					codeName = string.Empty;
				else
					codeName = value;
			}
		}
		public int DefaultThemeVersion { 
			get { return defaultThemeVersion; }
			set { defaultThemeVersion = OfficeThemeBuilder<DocumentFormat>.IsDefaultOfficeThemeVersion(value) ? value : 0; } 
		}
		public bool IsDefaultThemeVersion { get { return OfficeThemeBuilder<DocumentFormat>.IsDefaultOfficeThemeVersion(DefaultThemeVersion); } }
		#endregion
		public WorkbookProperties Clone() {
			WorkbookProperties result = new WorkbookProperties(workbook);
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(WorkbookProperties value) {
			this.useR1C1ReferenceStyle = value.useR1C1ReferenceStyle;
			this.refreshAllOnLoading = value.refreshAllOnLoading;
			this.saveBackup = value.saveBackup;
			this.supportNaturalLanguagesFormulaInput = value.supportNaturalLanguagesFormulaInput;
			this.showBordersOfUnselectedTables = value.showBordersOfUnselectedTables;
			this.showIncAnnotations = value.showIncAnnotations;
			this.showPivotChartFilter = value.showPivotChartFilter;
			this.builtInFunctionGroupCount = value.builtInFunctionGroupCount;
			int count = value.sheetIdTable.Count;
			for (int i = 0; i < count; i++) {
				this.sheetIdTable.Add(value.sheetIdTable[i]);
			}
			this.displayObjects = value.displayObjects;
			this.calculationOptions.CopyFrom(value.calculationOptions);
			this.protection.CopyFrom(value.protection);
			this.codeName = value.codeName;
			this.defaultThemeVersion = value.defaultThemeVersion;
			this.workbookWindowPropertiesList.Clear();
			foreach (WorkbookWindowProperties sourceWorkbookWindowProperties in value.workbookWindowPropertiesList) {
				WorkbookWindowProperties targetWorkbookWindowProperties = new WorkbookWindowProperties(this.workbook);
				targetWorkbookWindowProperties.CopyFrom(sourceWorkbookWindowProperties);
				this.workbookWindowPropertiesList.Add(targetWorkbookWindowProperties);
			}
		}
		protected internal override void ResetCore() {
			this.useR1C1ReferenceStyle = false;
			this.refreshAllOnLoading = false;
			this.saveBackup = false;
			this.showBordersOfUnselectedTables = false;
			this.showIncAnnotations = false;
			this.showPivotChartFilter = false;
			this.supportNaturalLanguagesFormulaInput = false;
			this.builtInFunctionGroupCount = 0;
			this.displayObjects = DisplayObjectsOptions.ShowAll;
			this.sheetIdTable = new List<int>();
			this.protection = new WorkbookProtectionOptions();
			this.codeName = string.Empty;
			this.defaultThemeVersion = 0;
			if (calculationOptions != null)
				calculationOptions.Reset();
			if (this.workbookWindowPropertiesList != null) {
				this.workbookWindowPropertiesList.Clear();
				this.workbookWindowPropertiesList.Add(new WorkbookWindowProperties(workbook));
			}
		}
		void Initialize() {
			System.Diagnostics.Debug.Assert(workbook != null);
			this.protection = new WorkbookProtectionOptions();
			this.calculationOptions = new CalculationOptions(workbook);
			this.workbookWindowPropertiesList = new List<WorkbookWindowProperties>();
			this.workbookWindowPropertiesList.Add(new WorkbookWindowProperties(workbook));
		}
	}
	#endregion
	public class WorkbookWindowInfo : SpreadsheetPackedValuesInfoObject , ICloneable<WorkbookWindowInfo>, ISupportsCopyFrom<WorkbookWindowInfo>, ISupportsSizeOf {
		const uint MaskMinimized = 0x00000001;				  
		const uint MaskDisplayHorizontalScroll = 0x00000002;	
		const uint MaskDisplayVerticalScroll = 0x0000004;	   
		const uint MaskDisplaySheetTabs = 0x0000008;			
		const uint MaskNoAutoFilterDateGroup = 0x00000010;	  
		const uint MaskVisibility = 0x00000060;				 
		const uint MaskTabRatio = 0x0001FF80;				   
		#region Fields
		int horizontalPosition; 
		int verticalPosition; 
		int widthinTwips; 
		int heightInTwips; 
		int firstDisplayedTabIndex; 
		int selectedTabsCount;
		#endregion
		#region Properties
		public bool Minimized { get { return GetBooleanVal(MaskMinimized); } set { SetBooleanVal(MaskMinimized, value); } }
		public bool ShowHorizontalScroll { get { return GetBooleanVal(MaskDisplayHorizontalScroll); } set { SetBooleanVal(MaskDisplayHorizontalScroll, value); } }
		public bool ShowVerticalScroll { get { return GetBooleanVal(MaskDisplayVerticalScroll); } set { SetBooleanVal(MaskDisplayVerticalScroll, value); } }
		public bool ShowSheetTabs { get { return GetBooleanVal(MaskDisplaySheetTabs); } set { SetBooleanVal(MaskDisplaySheetTabs, value); } }
		public bool AutoFilterDateGrouping { get { return GetBooleanVal(MaskNoAutoFilterDateGroup); } set { SetBooleanVal(MaskNoAutoFilterDateGroup, value); } }
		public SheetVisibleState Visibility { get { return (SheetVisibleState)GetUInt(MaskVisibility, 5); } set { SetUInt((uint)value, MaskVisibility, 5); } }
		public int TabRatio { get { return (int)GetUInt(MaskTabRatio, 7); } set { SetUInt((uint)value, MaskTabRatio, 7); } }
		public int HorizontalPosition { get { return horizontalPosition; } set { horizontalPosition = value; } }
		public int VerticalPosition { get { return verticalPosition; } set { verticalPosition = value; } }
		public int WidhtInTwips { get { return widthinTwips; } set { widthinTwips = value; } }
		public int HeightInTwips { get { return heightInTwips; } set { heightInTwips = value; } }
		public int FirstDisplayedTabIndex { get { return firstDisplayedTabIndex; } set { firstDisplayedTabIndex = value; } }
		public int SelectedTabsCount { get { return selectedTabsCount; } set { selectedTabsCount = value; } }
		#endregion
		#region ICloneable<WorkbookWindowInfo> Members
		public WorkbookWindowInfo Clone() {
			WorkbookWindowInfo result = new WorkbookWindowInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<WorkbookWindowInfo> Members
		public void CopyFrom(WorkbookWindowInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.PackedValues = value.PackedValues;
			this.horizontalPosition = value.horizontalPosition;
			this.verticalPosition = value.verticalPosition;
			this.widthinTwips = value.widthinTwips;
			this.heightInTwips = value.heightInTwips;
			this.firstDisplayedTabIndex = value.firstDisplayedTabIndex;
			this.selectedTabsCount = value.selectedTabsCount;
		}
		#endregion
		public override bool EqualsCore(SpreadsheetPackedValuesInfoObject obj) {
			WorkbookWindowInfo info = obj as WorkbookWindowInfo;
			if (info == null)
				return false;
			return this.PackedValues == info.PackedValues
			   && horizontalPosition == info.horizontalPosition
			   && horizontalPosition == info.horizontalPosition
			   && verticalPosition == info.verticalPosition
			   && widthinTwips == info.widthinTwips
			   && heightInTwips == info.heightInTwips
			   && firstDisplayedTabIndex == info.firstDisplayedTabIndex
			   && selectedTabsCount == info.selectedTabsCount;
		}
		public override int GetHashCodeCore() {
			CombinedHashCode calculator = new CombinedHashCode((long)PackedValues);
			calculator.AddInt(horizontalPosition);
			calculator.AddInt(verticalPosition);
			calculator.AddInt(widthinTwips);
			calculator.AddInt(heightInTwips);
			calculator.AddInt(firstDisplayedTabIndex);
			calculator.AddInt(selectedTabsCount);
			return calculator.CombinedHash32;
		}
	}
	#region WorkbookWindowProperties
	public class WorkbookWindowProperties : SpreadsheetUndoableIndexBasedObject<WorkbookWindowInfo>, ICloneable<WorkbookWindowProperties> {
		#region Fields
		public const int Size = 0x12;
		public static int DefaultSelectedTabIndex = 0;
		int selectedTabIndex;
		#endregion
		public WorkbookWindowProperties(DocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		#region SelectedTabIndex
		public int SelectedTabIndex {
			get { return selectedTabIndex; }
			set {
				if (selectedTabIndex == value)
					return;
				selectedTabIndex = value;
				DocumentModel.InnerApplyChanges(DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.RaiseUpdateUI);
			}
		}
		#endregion
		#region HorizontalPosition
		public int HorizontalPosition {
			get { return Info.HorizontalPosition; }
			set {
				if (HorizontalPosition == value)
					return;
				SetPropertyValue(SetHorizontalPositionCore, value);
			}
		}
		DocumentModelChangeActions SetHorizontalPositionCore(WorkbookWindowInfo info, int value) {
			info.HorizontalPosition = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region VerticalPosition
		public int VerticalPosition {
			get { return Info.VerticalPosition; }
			set {
				if (VerticalPosition == value)
					return;
				SetPropertyValue(SetVerticalPositionCore, value);
			}
		}
		DocumentModelChangeActions SetVerticalPositionCore(WorkbookWindowInfo info, int value) {
			info.VerticalPosition = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region WidhtInTwips
		public int WidhtInTwips {
			get { return Info.WidhtInTwips; }
			set {
				if (WidhtInTwips == value)
					return;
				SetPropertyValue(SetWidhtInTwipsCore, value);
			}
		}
		DocumentModelChangeActions SetWidhtInTwipsCore(WorkbookWindowInfo info, int value) {
			info.WidhtInTwips = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HeightInTwips
		public int HeightInTwips {
			get { return Info.HeightInTwips; }
			set {
				if (HeightInTwips == value)
					return;
				SetPropertyValue(SetHeightInTwipsCore, value);
			}
		}
		DocumentModelChangeActions SetHeightInTwipsCore(WorkbookWindowInfo info, int value) {
			info.HeightInTwips = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FirstDisplayedTabIndex
		public int FirstDisplayedTabIndex {
			get { return Info.FirstDisplayedTabIndex; }
			set {
				if (FirstDisplayedTabIndex == value)
					return;
				SetPropertyValue(SetFirstDisplayedTabIndexCore, value);
			}
		}
		DocumentModelChangeActions SetFirstDisplayedTabIndexCore(WorkbookWindowInfo info, int value) {
			info.FirstDisplayedTabIndex = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SelectedTabsCount
		public int SelectedTabsCount {
			get { return Info.SelectedTabsCount; }
			set {
				if (SelectedTabsCount == value)
					return;
				SetPropertyValue(SetSelectedTabsCountCore, value);
			}
		}
		DocumentModelChangeActions SetSelectedTabsCountCore(WorkbookWindowInfo info, int value) {
			info.SelectedTabsCount = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TabRatio
		public int TabRatio {
			get { return Info.TabRatio; }
			set {
				if (TabRatio == value)
					return;
				SetPropertyValue(SetTabRatioCore, value);
			}
		}
		DocumentModelChangeActions SetTabRatioCore(WorkbookWindowInfo info, int value) {
			info.TabRatio = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Minimized
		public bool Minimized {
			get { return Info.Minimized; }
			set {
				if (Minimized == value)
					return;
				SetPropertyValue(SetMinimizedCore, value);
			}
		}
		DocumentModelChangeActions SetMinimizedCore(WorkbookWindowInfo info, bool value) {
			info.Minimized = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowHorizontalScroll
		public bool ShowHorizontalScroll {
			get { return Info.ShowHorizontalScroll; }
			set {
				if (ShowHorizontalScroll == value)
					return;
				SetPropertyValue(SetShowHorizontalScrollCore, value);
			}
		}
		DocumentModelChangeActions SetShowHorizontalScrollCore(WorkbookWindowInfo info, bool value) {
			info.ShowHorizontalScroll = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowVerticalScroll
		public bool ShowVerticalScroll {
			get { return Info.ShowVerticalScroll; }
			set {
				if (ShowVerticalScroll == value)
					return;
				SetPropertyValue(SetShowVerticalScrollCore, value);
			}
		}
		DocumentModelChangeActions SetShowVerticalScrollCore(WorkbookWindowInfo info, bool value) {
			info.ShowVerticalScroll = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowSheetTabs
		public bool ShowSheetTabs {
			get { return Info.ShowSheetTabs; }
			set {
				if (ShowSheetTabs == value)
					return;
				SetPropertyValue(SetShowSheetTabsCore, value);
			}
		}
		DocumentModelChangeActions SetShowSheetTabsCore(WorkbookWindowInfo info, bool value) {
			info.ShowSheetTabs = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AutoFilterDateGrouping
		public bool AutoFilterDateGrouping {
			get { return Info.AutoFilterDateGrouping; }
			set {
				if (AutoFilterDateGrouping == value)
					return;
				SetPropertyValue(SetAutoFilterDateGroupingCore, value);
			}
		}
		DocumentModelChangeActions SetAutoFilterDateGroupingCore(WorkbookWindowInfo info, bool value) {
			info.AutoFilterDateGrouping = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Visibility
		public SheetVisibleState Visibility {
			get { return Info.Visibility; }
			set {
				if (Visibility == value)
					return;
				SetPropertyValue(SetVisibilityCore, value);
			}
		}
		DocumentModelChangeActions SetVisibilityCore(WorkbookWindowInfo info, SheetVisibleState value) {
			info.Visibility = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<WorkbookWindowInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.WindowInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
		}
		#region ICloneable<WorkbookWindowProperties> Members
		public WorkbookWindowProperties Clone() {
			WorkbookWindowProperties result = new WorkbookWindowProperties(DocumentModel);
			result.Info.CopyFrom(this.Info);
			return result;
		}
		#endregion
	}
	#endregion
	#region WorkbookWindowInfoCache
	public class WorkbookWindowInfoCache : UniqueItemsCache<WorkbookWindowInfo> {
		public WorkbookWindowInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override WorkbookWindowInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			WorkbookWindowInfo item = new WorkbookWindowInfo();
			item.Visibility = SheetVisibleState.Visible;
			item.Minimized = false;
			item.ShowHorizontalScroll = true;
			item.ShowVerticalScroll = true;
			item.ShowSheetTabs = true;
			item.TabRatio = 600;
			item.FirstDisplayedTabIndex = 0;
			item.AutoFilterDateGrouping = true;
			return item;
		}
	}
	#endregion
}
