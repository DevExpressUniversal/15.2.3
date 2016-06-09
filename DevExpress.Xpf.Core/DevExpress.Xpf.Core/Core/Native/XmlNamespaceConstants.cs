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
namespace DevExpress.Xpf.Core.Native {
	public static class XmlNamespaceConstants {
		#region DevExpress.Mvvm.UI
		public const string MvvmPrefix = "dxmvvm";
		public const string MvvmNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/mvvm";
		public const string MvvmNamespace = "DevExpress.Mvvm";
		public const string MvvmIntenalPrefix = "dxmvvminternal";
		public const string MvvmInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/mvvm/internal";
		public const string MvvmUINamespace = "DevExpress.Mvvm.UI";
		public const string MvvmInteractivityNamespace = "DevExpress.Mvvm.UI.Interactivity";
		public const string MvvmInteractivityInternalNamespace = "DevExpress.Mvvm.UI.Interactivity.Internal";
		#endregion
		public const string DocumentViewerPrefix = "dxdv";
		public const string PdfViewerPrefix = "dxpdf";
		public const string PdfViewerInternalPrefix = "dxpdfi";
		public const string OfficePrefix = "dxo";
		public const string SpreadsheetPrefix = "dxsps";
		public const string UtilsPrefix = "dx";
		public const string GridPrefix = "dxg";
		public const string GridThemesPrefix = "dxgt";
		public const string PivotGridPrefix = "dxpg";
		public const string PivotGridInternalPrefix = "dxpgi";
		public const string BarsPrefix = "dxb";
		public const string ChartsPrefix = "dxc";
		public const string GaugesPrefix = "dxga";
		public const string MapPrefix = "dxm";
		public const string TreeMapPrefix = "dxtm";
		public const string NavBarPrefix = "dxn";
		public const string EditorsPrefix = "dxe";
		public const string CarouselPrefix = "dxca";
		public const string DockingPrefix = "dxdo";
		public const string DockingVisualElementsPrefix = "dxdove";
		public const string DockingInternalPrefix = "dxdoi";
		public const string SchedulerPrefix = "dxsch";
		public const string RichEditPrefix = "dxre";
		public const string RichEditExtensionsPrefix = "dxreex";
		public const string PrintingPrefix = "dxp";
		public const string PrintingBarsPrefix = "dxpbars";
		public const string PrintingParametersPrefix = "dxpp";
		public const string LayoutControlPrefix = "dxlc";
		public const string RibbonPrefix = "dxr";
		public const string RibbonInternalPrefix = "dxri";
		public const string ControlsPrefix = "dxco";
		public const string NavigationPrefix = "dxnav";
		public const string NavigationInternalPrefix = "dxnavi";
		public const string PropertyGridPrefix = "dxprg";
		public const string DiagramPrefix = "dxdiag";
		public const string DiagramCorePrefix = "dxdiagcore";
		public const string DiagramThemesPrefix = "dxdiagt";
		public const string ReportDesignerPrefix = "dxrud";
		public const string ReportDesignerInternalPrefix = "dxrudi";
		public const string ReportDesignerExtensionsPrefix = "dxrudex";
		public const string ReportDesignerThemesPrefix = "dxrudt";
		public const string WindowsUIPrefix = "dxwui";
		public const string WindowsUINavigationPrefix = "dxwuin";
		public const string WizardFrameworkPrefix = "dxwf";
		public const string DocumentViewerNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/documentviewer";
		public const string DocumentViewerThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/documentviewer/themekeys";
		public const string PdfViewerNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/pdf";
		public const string PdfViewerThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/pdf/themekeys";
		public const string PdfViewerInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/pdf/internal";
		public const string SpreadsheetNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/spreadsheet";
		public const string SpreadsheetInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/spreadsheet/internal";
		public const string SpreadsheetThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/spreadsheet/themekeys";
		public const string UtilsNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/core";
		public const string WizardFrameworkNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/core/wizardframework";
		public const string UtilsInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/core/internal";
		public const string UtilsThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/core/themekeys";
		public const string GridNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/grid";
		public const string GridThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/grid/themekeys";
		public const string GridInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/grid/internal";
		public const string ControlsNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/controls";
		public const string ControlsThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/controls/themekeys";
		public const string NavigationNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/navigation";
		public const string NavigationInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/navigation/internal";
		public const string PivotGridNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/pivotgrid";
		public const string PivotGridThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/pivotgrid/themekeys";
		public const string PivotGridInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/pivotgrid/internal";
		public const string BarsNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/bars";
		public const string BarsThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/bars/themekeys";
		public const string BarsInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/bars/internal";
		public const string RibbonNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/ribbon";
		public const string RibbonInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/ribbon/internal";
		public const string RibbonThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/ribbon/themekeys";
		public const string ChartsNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/charts";
		public const string ChartsRangeControlClientDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/charts/rangecontrolclient";
		public const string ChartsThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/charts/themekeys";
		public const string GaugesNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/gauges";
		public const string GaugesThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/gauges/themekeys";
		public const string MapNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/map";
		public const string MapThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/map/themekeys";
		public const string TreeMapNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/treemap";
		public const string TreeMapThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/treemap/themekeys";
		public const string NavBarNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/navbar";
		public const string NavBarThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/navbar/themekeys";
		public const string EditorsNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/editors";
		public const string EditorsThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/editors/themekeys";
		public const string EditorsInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/editors/internal";
		public const string PropertyGridNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/propertygrid";
		public const string PropertyGridThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/propertygrid/themekeys";
		public const string DiagramNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/diagram";
		public const string DiagramCoreNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/diagram/core";
		public const string DiagramThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/diagram/themekeys";
		public const string CarouselNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/carousel";
		public const string CarouselThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/carousel/themekeys";
		public const string DockingNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/docking";
		public const string DockingThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/docking/themekeys";
		public const string DockingPlatformNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/docking/platform";
		public const string DockingVisualElementsNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/docking/visualelements";
		public const string SchedulerNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/scheduler";
		public const string SchedulerThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/scheduler/themekeys";
		public const string SchedulerInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/scheduler/internal";
		public const string OfficeNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/office";
		public const string RichEditNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/richedit";
		public const string RichEditInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/richedit/internal";
		public const string RichEditThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/richedit/themekeys";
		public const string RichEditExtensionsNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/richeditextensions";
		public const string PrintingNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/printing";
		public const string PrintingBarsNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/printing/bars";
		public const string PrintingThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/printing/themekeys";
		public const string PrintingNativeNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/printing/native";
		public const string PrintingParametersNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/printing/parameters";
		public const string ReportDesignerNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner";
		public const string ReportDesignerInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner/internal";
		public const string ReportDesignerThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner/themekeys";
		public const string ReportDesignerExtensionsNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesignerextensions";
		public const string LayoutControlNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/layoutcontrol";
		public const string WindowsUINamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/windowsui";
		public const string WindowsUIThemeKeysNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/windowsui/themekeys";
		public const string WindowsUIInternalNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/windowsui/internal";
		public const string WindowsUINavigationNamespaceDefinition = "http://schemas.devexpress.com/winfx/2008/xaml/windowsui/navigation";
		public const string DocumentViewerNamespace = "DevExpress.Xpf.DocumentViewer";
		public const string DocumentViewerThemesNamespace = "DevExpress.Xpf.DocumentViewer.Themes";
		public const string PdfViewerNamespace = "DevExpress.Xpf.PdfViewer";
		public const string PdfViewerThemesNamespace = "DevExpress.Xpf.PdfViewer.Themes";
		public const string PdfViewerInternalNamespace = "DevExpress.Xpf.PdfViewer.Internal";
		public const string SpreadsheetNamespace = "DevExpress.Xpf.Spreadsheet";
		public const string SpreadsheetInternalNamespace = "DevExpress.Xpf.Spreadsheet.Internal";
		public const string SpreadsheetUINamespace = "DevExpress.Xpf.Spreadsheet.UI";
		public const string SpreadsheetMenuNamespace = "DevExpress.Xpf.Spreadsheet.Menu";
		public const string SpreadsheetThemesNamespace = "DevExpress.Xpf.Spreadsheet.Themes";
		public const string CoreNamespace = "DevExpress.Core";
		public const string UtilsNamespace = "DevExpress.Xpf.Core";
		public const string UtilsNativeNamespace = "DevExpress.Xpf.Core.Native";
		public const string ServerModeNamespace = "DevExpress.Xpf.Core.ServerMode";
		public const string DataSourcesNamespace = "DevExpress.Xpf.Core.DataSources";
		public const string WizardFrameworkNamespace = "DevExpress.Xpf.Core.WizardFramework";
		public const string SerializationNamespace = "DevExpress.Xpf.Core.Serialization";
		public const string ThemesNamespace = "DevExpress.Xpf.Core";
		public const string UtilsThemesNamespace = "DevExpress.Xpf.Utils.Themes";
		public const string DataNamespace = "DevExpress.Xpf.Data";
		public const string GridNamespace = "DevExpress.Xpf.Grid";
		public const string GridHitTestNamespace = "DevExpress.Xpf.Grid.HitTest";
		public const string GridThemesNamespace = "DevExpress.Xpf.Grid.Themes";
		public const string GridLookUpNamespace = "DevExpress.Xpf.Grid.LookUp";
		public const string TreeListNamespace = "DevExpress.Xpf.Grid.TreeList";
		public const string ControlsNamespace = "DevExpress.Xpf.Controls";
		public const string ControlsThemeKeysNamespace = "DevExpress.Xpf.Controls.ThemeKeys";
		public const string NavigationNamespace = "DevExpress.Xpf.Navigation";
		public const string NavigationInternalNamespace = "DevExpress.Xpf.Navigation.Internal";
		public const string ConditionalFormattingNamespace = "DevExpress.Xpf.Core.ConditionalFormatting";
		public const string ConditionalFormattingNativeNamespace = "DevExpress.Xpf.Core.ConditionalFormatting.Native";
		public const string ConditionalFormattingThemeKeysNamespace = "DevExpress.Xpf.Core.ConditionalFormatting.Themes";
		public const string PivotGridNamespace = "DevExpress.Xpf.PivotGrid";
		public const string PivotGridInternalNamespace = "DevExpress.Xpf.PivotGrid.Internal";
		public const string BarsNamespace = "DevExpress.Xpf.Bars";
		public const string BarsThemeKeysNamespace = "DevExpress.Xpf.Bars.Themes";
		public const string BarsHelpersNamespace = "DevExpress.Xpf.Bars.Helpers";
		public const string BarsCustomizationNamespace = "DevExpress.Xpf.Bars.Customization";
		public const string RibbonNamespace = "DevExpress.Xpf.Ribbon";
		public const string RibbonInternalNamespace = "DevExpress.Xpf.Ribbon.Internal";
		public const string RibbonThemeKeysNamespace = "DevExpress.Xpf.Ribbon.Themes";
		public const string ChartsNamespace = "DevExpress.Xpf.Charts";
		public const string ChartsRangeControlClientNamespace = "DevExpress.Xpf.Charts.RangeControlClient";
		public const string ChartDesignerNamespace = "DevExpress.Charts.Designer";
		public const string ChartsThemeKeysNamespace = "DevExpress.Xpf.Charts.Themes";
		public const string GaugesNamespace = "DevExpress.Xpf.Gauges";
		public const string GaugesThemeKeysNamespace = "DevExpress.Xpf.Gauges.Themes";
		public const string MapNamespace = "DevExpress.Xpf.Map";
		public const string MapThemeKeysNamespace = "DevExpress.Xpf.Map.Themes";
		public const string TreeMapNamespace = "DevExpress.Xpf.TreeMap";
		public const string TreeMapThemeKeysNamespace = "DevExpress.Xpf.TreeMap.Themes";
		public const string NavBarNamespace = "DevExpress.Xpf.NavBar";
		public const string NavBarThemeKeysNamespace = "DevExpress.Xpf.NavBar.Themes";
		public const string EditorsNamespace = "DevExpress.Xpf.Editors";
		public const string EditorsHelpersNamespace = "DevExpress.Xpf.Editors.Helpers";
		public const string EditorsInternalNamespace = "DevExpress.Xpf.Editors.Internal";
		public const string EditorsFilteringNamespace = "DevExpress.Xpf.Editors.Filtering";
		public const string EditorsFlyoutNamespace = "DevExpress.Xpf.Editors.Flyout";
		public const string EditorsFlyoutInternalNamespace = "DevExpress.Xpf.Editors.Flyout.Native";
		public const string EditorsDateNavigatorNamespace = "DevExpress.Xpf.Editors.DateNavigator";
		public const string EditorsDateNavigatorControlsNamespace = "DevExpress.Xpf.Editors.DateNavigator.Controls";
		public const string EditorsDataPagerNamespace = "DevExpress.Xpf.Editors.DataPager";
		public const string EditorsExpressionEditorNamespace = "DevExpress.Xpf.Editors.ExpressionEditor";
		public const string EditorsValidationNamespace = "DevExpress.Xpf.Editors.Validation";
		public const string EditorsPopupsNamespace = "DevExpress.Xpf.Editors.Popups";
		public const string EditorsCalendarNamespace = "DevExpress.Xpf.Editors.Popups.Calendar";
		public const string EditorsSettingsNamespace = "DevExpress.Xpf.Editors.Settings";
		public const string EditorsSettingsExtensionNamespace = "DevExpress.Xpf.Editors.Settings.Extension";
		public const string EditorsThemesNamespace = "DevExpress.Xpf.Editors.Themes";
		public const string EditorsRangeControlNamespace = "DevExpress.Xpf.Editors.RangeControl";
		public const string EditorsRangeControlInternalNamespace = "DevExpress.Xpf.Editors.RangeControl.Internal";
		public const string PropertyGridNamespace = "DevExpress.Xpf.PropertyGrid";
		public const string PropertyGridThemesNamespace = "DevExpress.Xpf.PropertyGrid.Themes";
		public const string DiagramNamespace = "DevExpress.Xpf.Diagram";
		public const string DiagramCoreNamespace = "DevExpress.Xpf.Diagram.Native";
		public const string DiagramThemesNamespace = "DevExpress.Xpf.Diagram.Themes";
		public const string CarouselNamespace = "DevExpress.Xpf.Carousel";
		public const string CarouselThemesNamespace = "DevExpress.Xpf.Carousel.Themes";
		public const string DockingNamespace = "DevExpress.Xpf.Docking";
		public const string DockingThemeKeysNamespace = "DevExpress.Xpf.Docking.ThemeKeys";
		public const string DockingPlatformNamespace = "DevExpress.Xpf.Docking.Platform";
		public const string DockingVisualElementsNamespace = "DevExpress.Xpf.Docking.VisualElements";
		public const string SchedulerNamespace = "DevExpress.Xpf.Scheduler";
		public const string SchedulerUINamespace = "DevExpress.Xpf.Scheduler.UI";
		public const string SchedulerReportingNamespace = "DevExpress.Xpf.Scheduler.Reporting";
		public const string SchedulerReportingUINamespace = "DevExpress.Xpf.Scheduler.Reporting.UI";
		public const string SchedulerDrawingNamespace = "DevExpress.Xpf.Scheduler.Drawing";
		public const string SchedulerThemeKeysNamespace = "DevExpress.Xpf.Scheduler.ThemeKeys";
		public const string SchedulerCommandsNamespace = "DevExpress.Xpf.Scheduler.Commands";
		public const string OfficeUINamespace = "DevExpress.Xpf.Office.UI";
		public const string RichEditNamespace = "DevExpress.XtraRichEdit";
		public const string RichEditXpfNamespace = "DevExpress.Xpf.RichEdit";
		public const string RichEditUINamespace = "DevExpress.Xpf.RichEdit.UI";
		public const string RichEditMenuNamespace = "DevExpress.XtraRichEdit.Menu";
		public const string RichEditInternalControlsNamespace = "DevExpress.Xpf.RichEdit.Controls.Internal";
		public const string RichEditCoreNamespace = "DevExpress.Xpf.Core";
		public const string RichEditThemeKeysNamespace = "DevExpress.Xpf.RichEdit.Themes";
		public const string RichEditExtensionsNamespace = "DevExpress.Xpf.RichEdit.Extensions";
		public const string PrintingNamespace = "DevExpress.Xpf.Printing";
		public const string PrintingBarsNamespace = "DevExpress.Xpf.Printing.PreviewControl.Bars";
		public const string PrintingUserInterfaceNamespace = "DevExpress.Xpf.Printing.UI";
		public const string PrintingNativeNamespace = "DevExpress.Xpf.Printing.Native";
		public const string PrintingParametersNamespace = "DevExpress.Xpf.Printing.Parameters";
		public const string PrintingThemeKeysNamespace = "DevExpress.Xpf.Printing.Themes";
		public const string LayoutControlNamespace = "DevExpress.Xpf.LayoutControl";
		public const string ReportDesignerNamespace = "DevExpress.Xpf.Reports.UserDesigner";
		public const string ReportDesignerEditorsNamespace = "DevExpress.Xpf.Reports.UserDesigner.Editors";
		public const string ReportDesignerNativeNamespace = "DevExpress.Xpf.Reports.UserDesigner.Native";
		public const string ReportDesignerXRDiagramNamespace = "DevExpress.Xpf.Reports.UserDesigner.XRDiagram";
		public const string ReportDesignerFieldListNamespace = "DevExpress.Xpf.Reports.UserDesigner.FieldList";
		public const string ReportDesignerReportExplorerNamespace = "DevExpress.Xpf.Reports.UserDesigner.ReportExplorer";
		public const string ReportDesignerToolboxNamespace = "DevExpress.Xpf.Reports.UserDesigner.Toolbox";
		public const string ReportDesignerThemesNamespace = "DevExpress.Xpf.Reports.UserDesigner.Themes";
		public const string ReportDesignerExtensionsNamespace = "DevExpress.Xpf.Reports.UserDesigner.Extensions";
		public const string WindowsUINamespace = "DevExpress.Xpf.WindowsUI";
		public const string WindowsUIThemeKeysNamespace = "DevExpress.Xpf.WindowsUI.ThemeKeys";
		public const string WindowsUIInternalNamespace = "DevExpress.Xpf.WindowsUI.Internal";
		public const string WindowsUINavigationNamespace = "DevExpress.Xpf.WindowsUI.Navigation";
		public const string XpfCoreNamespace = "DevExpress.Xpf.Core";
	}
}
