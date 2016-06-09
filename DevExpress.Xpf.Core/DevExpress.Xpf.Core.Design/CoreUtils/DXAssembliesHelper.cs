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

#if SILVERLIGHT
extern alias Platform;
using AssemblyInfo = Platform::AssemblyInfo;
#endif
#if !SL
using DevExpress.Xpf.Core.Native;
#endif
using System;
using System.Reflection;
using System.Text;
using System.Linq;
using DevExpress.Utils.Design;
namespace DevExpress.Xpf.Core.Design {
	public static class DXAssembliesHelper {
		#region assembly names
		private const string defaultDataNamespace = "DevExpress.Data";
		private const string defaultLayoutCoreNamespace = "DevExpress.Xpf.Layout.Core";
		private const string defaultImagesNamespace = "DevExpress.Images";
		private const string defaultSchedulerCoreNamespace = "DevExpress.XtraScheduler";
		private const string defaultPivotGridCoreNamespace = "DevExpress.Data.PivotGrid";
		private const string defaultPrintingCoreNamespace = "DevExpress.Printing";
		private const string defaultChartsCoreNamespace = "DevExpress.Charts.Native";
		private const string defaultRibbonTypeName = "RibbonControl";
		private const string defaultDataTypeName = "UnboundErrorObject";
		private const string defaultGridTypeName = "GridControl";
		private const string defaultBarsTypeName = "BarManager";
		private const string defaultNavBarTypeName = "NavBarControl";
		private const string defaultDokingTypeName = "DockLayoutManager";
		private const string defaultLayoutCoreTypeName = "BaseLayoutElementHost";
		private const string defaultTabControlNamespace = "DevExpress.Xpf.Core";
		private const string defaultTabControlTypeName = "DXTabControl";
		private const string defaultGridCoreTypeName = "DataViewBase";
		private const string defaultImagesTypeName = "ImagesAssemblyType";
		private const string defaultSchedulerTypeName = "SchedulerControl";
		private const string defaultSchedulerCoreTypeName = "WorkDaysCollection";
		private const string defaultEditorsTypeName = "ButtonEdit";
		private const string defaultPivotGridTypeName = "PivotGridControl";
		private const string defaultPivotGridCoreTypeName = "PivotSummaryDisplayTypeConverter";
		private const string defaultPrintingTypeName = "PrintingSystem";
		private const string defaultPrintingCoreTypeName = "ResFinder";
		private const string defaultChartsTypeName = "ChartControl";
		private const string defaultChartsCoreTypeName = "InteractionSeriesContainer";
		private const string defaultMapTypeName = "MapControl";
		private const string defaultLayoutTypeName = "LayoutControl";
#if !SL
		private const string shcedulerCoreAssembly = "DevExpress.XtraScheduler" + AssemblyInfo.VSuffix + ".Core";
		private const string layoutAssembly = "DevExpress.Xpf.LayoutControl" + AssemblyInfo.VSuffix;
#endif
		#endregion
#if !SL
		#region get type
		public static Type GetTypeFromRibbonAssembly(string typeName, string nameSpace = XmlNamespaceConstants.RibbonNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfRibbon);
		}
		public static Type GetTypeFromBarsAssembly(string typeName, string nameSpace = XmlNamespaceConstants.BarsNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfCore);
		}
		public static Type GetTypeFromEditorsAssembly(string typeName, string nameSpace = XmlNamespaceConstants.EditorsNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfCore);
		}
		public static Type GetTypeFromDockingAssembly(string typeName, string nameSpace = XmlNamespaceConstants.DockingNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfDocking);
		}
		public static Type GetTypeFromGridAssembly(string typeName, string nameSpace = XmlNamespaceConstants.GridNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfGrid);
		}
		public static Type GetTypeFromGridCoreAssembly(string typeName, string nameSpace = XmlNamespaceConstants.GridNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfGridCore);
		}
		public static Type GetTypeFromImagesAssembly(string typeName, string nameSpace = defaultImagesNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyImages);
		}
		public static Type GetTypeFromSchedulerAssembly(string typeName, string nameSpace = XmlNamespaceConstants.SchedulerNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfScheduler);
		}
		public static Type GetTypeFromSchedulerCoreAssembly(string typeName, string nameSpace = defaultSchedulerCoreNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, shcedulerCoreAssembly);
		}
		public static Type GetTypeFromPivotGridAssembly(string typeName, string nameSpace = XmlNamespaceConstants.PivotGridNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyDXPivotGrid);
		}
		public static Type GetTypeFromPivotGridCoreAssembly(string typeName, string nameSpace = defaultPivotGridCoreNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyPivotGridCore);
		}
		public static Type GetTypeFromPrintingAssembly(string typeName, string nameSpace = XmlNamespaceConstants.PrintingNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfPrinting);
		}
		public static Type GetTypeFromPrintingCoreAssembly(string typeName, string nameSpace = defaultPrintingCoreNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyPrintingCore);
		}
		public static Type GetTypeFromChartsAssembly(string typeName, string nameSpace = XmlNamespaceConstants.ChartsNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyDXCharts);
		}
		public static Type GetTypeFromChartsCoreAssembly(string typeName, string nameSpace = defaultChartsCoreNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyChartsCore);
		}
		public static Type GetTypeFromMapAssembly(string typeName, string nameSpace = XmlNamespaceConstants.MapNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyDXMap);
		}
		public static Type GetTypeFromLayoutAssembly(string typeName, string nameSpace = XmlNamespaceConstants.LayoutControlNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, layoutAssembly);
		}
		public static Type GetTypeFromDataAssembly(string typeName, string nameSpace = defaultDataNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyData);
		}
		public static Type GetTypeFromNavBarAssembly(string typeName, string nameSpace = XmlNamespaceConstants.NavBarNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfNavBar);
		}
		public static Type GetTypeFromLayoutCoreAssembly(string typeName, string nameSpace = defaultLayoutCoreNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfLayoutCore);
		}
		public static Type GetTypeFromTabControlAssembly(string typeName, string nameSpace = XmlNamespaceConstants.UtilsNamespace) {
			return GetTypeFromAssembly(typeName, nameSpace, AssemblyInfo.SRAssemblyXpfCore);
		}
		#endregion
		#region get assembly
		public static Assembly GetDataAssembly() {
			return GetTypeFromDataAssembly(defaultDataTypeName).Assembly;
		}
		public static Assembly GetRibbonAssembly() {
			return GetTypeFromRibbonAssembly(defaultRibbonTypeName).Assembly;
		}
		public static Assembly GetGridAssembly() {
			return GetTypeFromGridAssembly(defaultGridTypeName).Assembly;
		}
		public static Assembly GetGridCoreAssembly() {
			return GetTypeFromGridCoreAssembly(defaultGridCoreTypeName).Assembly;
		}
		public static Assembly GetImagesAssembly() {
			return GetTypeFromImagesAssembly(defaultImagesTypeName).Assembly;
		}
		public static Assembly GetSchedulerAssembly() {
			return GetTypeFromSchedulerAssembly(defaultSchedulerTypeName).Assembly;
		}
		public static Assembly GetSchedulerCoreAssembly() {
			return GetTypeFromSchedulerCoreAssembly(defaultSchedulerCoreTypeName).Assembly;
		}
		public static Assembly GetPivotGridAssembly() {
			return GetTypeFromPivotGridAssembly(defaultPivotGridTypeName).Assembly;
		}
		public static Assembly GetPivotGridCoreAssembly() {
			return GetTypeFromPivotGridCoreAssembly(defaultPivotGridCoreTypeName).Assembly;
		}
		public static Assembly GetPrintingAssembly() {
			return GetTypeFromPrintingAssembly(defaultPrintingTypeName).Assembly;
		}
		public static Assembly GetPrintingCoreAssembly() {
			return GetTypeFromPrintingCoreAssembly(defaultPrintingCoreTypeName).Assembly;
		}
		public static Assembly GetChartsAssembly() {
			return GetTypeFromChartsAssembly(defaultChartsTypeName).Assembly;
		}
		public static Assembly GetChartsCoreAssembly() {
			return GetTypeFromChartsCoreAssembly(defaultChartsCoreTypeName).Assembly;
		}
		public static Assembly GetMapAssembly() {
			return GetTypeFromMapAssembly(defaultMapTypeName).Assembly;
		}
		public static Assembly GetLayoutAssembly() {
			return GetTypeFromLayoutAssembly(defaultLayoutTypeName).Assembly;
		}
		public static Assembly GetBarsAssembly() {
			return GetTypeFromBarsAssembly(defaultBarsTypeName).Assembly;
		}
		public static Assembly GetEditorsAssembly() {
			return GetTypeFromEditorsAssembly(defaultEditorsTypeName).Assembly;
		}
		public static Assembly GetNavBarAssembly() {
			return GetTypeFromNavBarAssembly(defaultNavBarTypeName).Assembly;
		}
		public static Assembly GetLayoutCoreAssembly() {
			return GetTypeFromLayoutCoreAssembly(defaultLayoutCoreTypeName).Assembly;
		}
		public static Assembly GetDockingAssembly() {
			return GetTypeFromDockingAssembly(defaultDokingTypeName).Assembly;
		}
		public static Assembly GetTabControlAssembly() {
			return GetTypeFromTabControlAssembly(defaultTabControlTypeName).Assembly;
		}
		#endregion
#endif
		public static Type GetTypeFromAssembly(string typeName, string nameSpace, string assemblyName) {
			return DXAssemblyHelper.GetTypeFromAssembly(typeName, nameSpace, assemblyName);
		}
	}
}
