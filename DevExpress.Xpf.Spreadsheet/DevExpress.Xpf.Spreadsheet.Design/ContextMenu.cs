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

extern alias Platform;
using System.Collections.Generic;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Platform::System.Collections.ObjectModel;
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.Spreadsheet.Design {
	[UsesItemPolicy(typeof(PrimarySelectionPolicy))]
	class SpreadsheetContextMenuProvider : ContextMenuProviderBase {
		public SpreadsheetContextMenuProvider() {
			SpreadsheetBarsMenuCreatorBase barsMenuCreator = new SpreadsheetBarsMenuCreator();
			barsMenuCreator.CreateMenu(Items);
			SpreadsheetBarsMenuCreatorBase ribbonMenuCreator = new SpreadsheetRibbonMenuCreator();
			ribbonMenuCreator.CreateMenu(Items);
		}
		protected override bool IsActive(Selection selection) {
			return selection.PrimarySelection != null;
		}
	}
	public abstract class SpreadsheetBarsMenuCreatorBase {
		MenuGroup createToolbarsMenuGroup;
		MenuAction createAllBars;
		MenuAction createFileBar;
		MenuAction createHomeBars;
		MenuAction createInsertBars;
		MenuAction createPageLayoutBars;
		MenuAction createReviewBars;
		MenuAction createViewBars;
		MenuAction createFormulaBars;
		MenuAction createDataBars;
		MenuAction createChartToolsBars;
		MenuAction createPictureToolsBars;
		MenuAction createDrawingToolsBars;
		MenuAction createTableToolsBars;
		MenuAction createPivotTableToolsBars;
		public void CreateMenu(System.Collections.Generic.IList<MenuBase> items) {
			createToolbarsMenuGroup = new MenuGroup(MenuGroupId, SubMenuCaption);
			createAllBars = new MenuAction(FormatMenuItemCaption("All"));
			createAllBars.Execute += OnCreateAllToolbars;
			createFileBar = new MenuAction(FormatMenuItemCaption("File"));
			createFileBar.Execute += OnCreateCommonToolbar;
			createHomeBars = new MenuAction(FormatMenuItemCaption("Home"));
			createHomeBars.Execute += OnCreateHomeToolbars;
			createInsertBars = new MenuAction(FormatMenuItemCaption("Insert"));
			createInsertBars.Execute += OnCreateInsertToolbars;
			createPageLayoutBars = new MenuAction(FormatMenuItemCaption("Page Layout"));
			createPageLayoutBars.Execute += OnCreatePageLayoutToolbars;
			createReviewBars = new MenuAction(FormatMenuItemCaption("Review"));
			createReviewBars.Execute += OnCreateReviewToolbars;
			createViewBars = new MenuAction(FormatMenuItemCaption("View"));
			createViewBars.Execute += OnCreateViewToolbars;
			createFormulaBars = new MenuAction(FormatMenuItemCaption("Formula"));
			createFormulaBars.Execute += OnCreateFormulaToolbars;
			createDataBars = new MenuAction(FormatMenuItemCaption("Data"));
			createDataBars.Execute += OnCreateDataToolbars;
			createChartToolsBars = new MenuAction(FormatMenuItemCaption("Chart Tools"));
			createChartToolsBars.Execute += OnCreateChartToolsToolbars;
			createPictureToolsBars = new MenuAction(FormatMenuItemCaption("Picture Tools"));
			createPictureToolsBars.Execute += OnCreatePictureToolsToolbars;
			createDrawingToolsBars = new MenuAction(FormatMenuItemCaption("Drawing Tools"));
			createDrawingToolsBars.Execute += OnCreateDrawingToolsToolbars;
			createTableToolsBars = new MenuAction(FormatMenuItemCaption("Table Tools"));
			createTableToolsBars.Execute += OnCreateTableToolsToolbars;
			createPivotTableToolsBars = new MenuAction(FormatMenuItemCaption("PivotTable Tools"));
			createPivotTableToolsBars.Execute += OnCreatePivotTableToolsToolbars;
			createToolbarsMenuGroup.HasDropDown = true;
			createToolbarsMenuGroup.Items.Add(createAllBars);
			createToolbarsMenuGroup.Items.Add(createFileBar);
			createToolbarsMenuGroup.Items.Add(createHomeBars);
			createToolbarsMenuGroup.Items.Add(createInsertBars);
			createToolbarsMenuGroup.Items.Add(createPageLayoutBars);
			createToolbarsMenuGroup.Items.Add(createReviewBars);
			createToolbarsMenuGroup.Items.Add(createViewBars);
			createToolbarsMenuGroup.Items.Add(createFormulaBars);
			createToolbarsMenuGroup.Items.Add(createDataBars);
			createToolbarsMenuGroup.Items.Add(createChartToolsBars);
			createToolbarsMenuGroup.Items.Add(createPictureToolsBars);
			createToolbarsMenuGroup.Items.Add(createDrawingToolsBars);
			createToolbarsMenuGroup.Items.Add(createTableToolsBars);
			createToolbarsMenuGroup.Items.Add(createPivotTableToolsBars);
			items.Add(createToolbarsMenuGroup);
		}
		protected internal abstract string SubMenuCaption { get; }
		protected internal abstract string MenuGroupId { get; }
		void OnCreateAllToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetAllBarInfos());
		}
		public static BarInfo[] GetAllBarInfos() {
			List<BarInfo> result = new List<BarInfo>();
			result.AddRange(GetCommonBarInfos());
			result.AddRange(GetHomeBarInfos());
			result.AddRange(GetInsertBarInfos());
			result.AddRange(GetPageLayoutBarInfos());
			result.AddRange(GetFormulaBarInfos());
			result.AddRange(GetDataBarInfos());
			result.AddRange(GetReviewBarInfos());
			result.AddRange(GetViewBarInfos());
			result.AddRange(GetChartToolsBarInfos());
			result.AddRange(GetPictureToolsBarInfos());
			result.AddRange(GetDrawingToolsBarInfos());
			result.AddRange(GetTableToolsBarInfos());
			result.AddRange(GetPivotTableToolsBarInfos());
			return result.ToArray();
		}
		void OnCreateCommonToolbar(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetCommonBarInfos());
		}
		static BarInfo[] GetCommonBarInfos() {
			return new BarInfo[] { BarInfos.File, BarInfos.Info };
		}
		void OnCreateHomeToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetHomeBarInfos());
		}
		static BarInfo[] GetHomeBarInfos() {
			return new BarInfo[] { BarInfos.Clipboard, BarInfos.Font, BarInfos.Alignment, BarInfos.Number, BarInfos.Styles, BarInfos.Cells, BarInfos.Editing };
		}
		void OnCreateInsertToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetInsertBarInfos());
		}
		static BarInfo[] GetInsertBarInfos() {
			return new BarInfo[] { BarInfos.Tables, BarInfos.Illustrations, BarInfos.Charts, BarInfos.Links, BarInfos.Symbols };
		}
		void OnCreatePageLayoutToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetPageLayoutBarInfos());
		}
		static BarInfo[] GetPageLayoutBarInfos() {
			return new BarInfo[] { BarInfos.PageSetup, BarInfos.PageLayoutShow, BarInfos.PageLayoutPrint, BarInfos.PageLayoutArrange };
		}
		void OnCreateReviewToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetReviewBarInfos());
		}
		static BarInfo[] GetReviewBarInfos() {
			return new BarInfo[] { BarInfos.Comments, BarInfos.Changes };
		}
		void OnCreateViewToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetViewBarInfos());
		}
		static BarInfo[] GetViewBarInfos() {
			return new BarInfo[] { BarInfos.Show, BarInfos.Zoom, BarInfos.Window };
		}
		void OnCreateFormulaToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetFormulaBarInfos());
		}
		static BarInfo[] GetFormulaBarInfos() {
			return new BarInfo[] { BarInfos.FunctionLibrary, BarInfos.FormulaDefinedNames, BarInfos.FormulaAuditing, BarInfos.FormulaCalculation };
		}
		void OnCreateDataToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetDataBarInfos());
		}
		static BarInfo[] GetDataBarInfos() {
			return new BarInfo[] { BarInfos.SortAndFilter, BarInfos.DataTools, BarInfos.Outline };
		}
		void OnCreateChartToolsToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetChartToolsBarInfos());
		}
		static BarInfo[] GetChartToolsBarInfos() {
			return new BarInfo[] {
				BarInfos.ChartDesignChartData, BarInfos.ChartDesignChartLayouts,
				BarInfos.ChartToolsLayoutLabels, BarInfos.ChartToolsLayoutAxes, BarInfos.ChartToolsLayoutAnalysis,
				BarInfos.ChartToolsFormatArrange,
			};
		}
		void OnCreatePictureToolsToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetPictureToolsBarInfos());
		}
		static BarInfo[] GetPictureToolsBarInfos() {
			return new BarInfo[] { BarInfos.PictureToolsFormatArrange };
		}
		void OnCreateTableToolsToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetTableToolsBarInfos());
		}
		static BarInfo[] GetTableToolsBarInfos() {
			return new BarInfo[] { BarInfos.TableToolsDesignTools, BarInfos.TableToolsDesignStyleOptions };
		}
		void OnCreateDrawingToolsToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetDrawingToolsBarInfos());
		}
		static BarInfo[] GetDrawingToolsBarInfos() {
			return new BarInfo[] { BarInfos.DrawingToolsFormatArrange };
		}
		void OnCreatePivotTableToolsToolbars(object sender, MenuActionEventArgs e) {
			CreateBars(e.Selection.PrimarySelection, GetPivotTableToolsBarInfos());
		}
		static BarInfo[] GetPivotTableToolsBarInfos() {
			return new BarInfo[] { BarInfos.PivotTableToolsAnalyzePivotTable, BarInfos.PivotTableToolsAnalyzeActiveField, BarInfos.PivotTableToolsAnalyzeData, BarInfos.PivotTableToolsAnalyzeActions,
				BarInfos.PivotTableToolsAnalyzeShow, BarInfos.PivotTableToolsDesignLayout, BarInfos.PivotTableToolsDesignPivotTableStyleOptions };
		}
		void CreateBars(ModelItem primarySelection, BarInfo[] barInfos) {
			if (barInfos.Length <= 0)
				return;
			CommandBarCreator creator = CreateBarCreator();
			creator.CreateBars(primarySelection, barInfos);
		}
		protected internal abstract CommandBarCreator CreateBarCreator();
		protected internal abstract string FormatMenuItemCaption(string name);
	}
	public class SpreadsheetBarsMenuCreator : SpreadsheetBarsMenuCreatorBase {
		protected internal override string MenuGroupId { get { return "CreateSpreadsheetBarItems"; } }
		protected internal override string SubMenuCaption { get { return "Create Bars"; } }
		protected internal override CommandBarCreator CreateBarCreator() {
			return new SpreadsheetCommandBarCreator();
		}
		protected internal override string FormatMenuItemCaption(string name) {
			return name;
		}
	}
	public class SpreadsheetRibbonMenuCreator : SpreadsheetBarsMenuCreatorBase {
		protected internal override string MenuGroupId { get { return "CreateSpreadsheetRibbonItems"; } }
		protected internal override string SubMenuCaption { get { return "Create Ribbon Items"; } }
		protected internal override CommandBarCreator CreateBarCreator() {
			return new SpreadsheetCommandRibbonCreator();
		}
		protected internal override string FormatMenuItemCaption(string name) {
			return name;
		}
	}
}
