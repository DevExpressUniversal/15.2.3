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
using System;
using Platform::DevExpress.Xpf.Ribbon.Design;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Design.SmartTags;
using Platform::DevExpress.Xpf.Core.Design;
using System.Collections.Generic;
namespace DevExpress.Xpf.Spreadsheet.Design {
	public sealed class SpreadsheetPropertyLinesProvider : PropertyLinesProviderBase {
		public SpreadsheetPropertyLinesProvider(Type itemType) : base(itemType) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new SeparatorLineViewModel(viewModel) { Text = "Create Ribbon Pages" });
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonAllPagesActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonFilePageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonHomePageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonInsertPageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonPageLayoutPageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonReviewPageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonViewPageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonFormulaPageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonDataPageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonChartToolsPageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonPictureToolsPageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonDrawingToolsPageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonTableToolsPageActionLineProvider(viewModel)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new SpreadsheetCreateRibbonPivotTableToolsPageActionLineProvider(viewModel)));
			return lines;
		}
	}
	public abstract class SpreadsheetCreateRibbonCommandActionLineProviderBase : CommandActionLineProvider {
		protected SpreadsheetCreateRibbonCommandActionLineProviderBase(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected abstract BarInfo[] BarInfos { get; }
		protected abstract string Caption { get; }
		protected override string GetCommandText() {
			return Caption;
		}
		protected override void OnCommandExecute(object param) {
			ModelItem spreadsheet = XpfModelItem.ToModelItem(Context.ModelItem);
			if (spreadsheet != null) {
				CommandBarCreator creator = CreateRibbonCreator();
				creator.CreateBars(spreadsheet, BarInfos);
			}
		}
		private CommandBarCreator CreateRibbonCreator() {
			return new SpreadsheetCommandRibbonCreator();
		}
	}
	public class SpreadsheetCreateRibbonAllPagesActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonAllPagesActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetAllBarInfos(); } }
		protected override string Caption { get { return "All"; } }
	}
	public class SpreadsheetCreateRibbonFilePageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonFilePageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetCommonBarInfos(); } }
		protected override string Caption { get { return "File"; } }
	}
	public class SpreadsheetCreateRibbonHomePageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonHomePageActionLineProvider
			(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetHomeBarInfos(); } }
		protected override string Caption { get { return "Home"; } }
	}
	public class SpreadsheetCreateRibbonInsertPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonInsertPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetInsertBarInfos(); } }
		protected override string Caption { get { return "Insert"; } }
	}
	public class SpreadsheetCreateRibbonPageLayoutPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonPageLayoutPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetPageLayoutBarInfos(); } }
		protected override string Caption { get { return "Page Layout"; } }
	}
	public class SpreadsheetCreateRibbonReviewPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonReviewPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerReviewModel)
			: base(ownerReviewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetReviewBarInfos(); } }
		protected override string Caption { get { return "Review"; } }
	}
	public class SpreadsheetCreateRibbonViewPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonViewPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetViewBarInfos(); } }
		protected override string Caption { get { return "View"; } }
	}
	public class SpreadsheetCreateRibbonFormulaPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonFormulaPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetFormulaBarInfos(); } }
		protected override string Caption { get { return "Formula"; } }
	}
	public class SpreadsheetCreateRibbonDataPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonDataPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetDataBarInfos(); } }
		protected override string Caption { get { return "Data"; } }
	}
	public class SpreadsheetCreateRibbonChartToolsPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonChartToolsPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetChartToolsBarInfos(); } }
		protected override string Caption { get { return "Chart Tools"; } }
	}
	public class SpreadsheetCreateRibbonPictureToolsPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonPictureToolsPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetDrawingToolsBarInfos(); } }
		protected override string Caption { get { return "Picture Tools"; } }
	}
	public class SpreadsheetCreateRibbonTableToolsPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonTableToolsPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetTableToolsBarInfos(); } }
		protected override string Caption { get { return "Table Tools"; } }
	}
	public class SpreadsheetCreateRibbonDrawingToolsPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonDrawingToolsPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetDataBarInfos(); } }
		protected override string Caption { get { return "Drawing Tools"; } }
	}
	public class SpreadsheetCreateRibbonPivotTableToolsPageActionLineProvider : SpreadsheetCreateRibbonCommandActionLineProviderBase {
		public SpreadsheetCreateRibbonPivotTableToolsPageActionLineProvider(FrameworkElementSmartTagPropertiesViewModel ownerViewModel)
			: base(ownerViewModel) { }
		protected override BarInfo[] BarInfos { get { return BarInfosProvider.GetPivotTableToolsBarInfos(); } }
		protected override string Caption { get { return "PivotTable Tools"; } }
	}
	public static class BarInfosProvider {
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
		public static BarInfo[] GetCommonBarInfos() {
			return new BarInfo[] { BarInfos.File, BarInfos.Info };
		}
		public static BarInfo[] GetHomeBarInfos() {
			return new BarInfo[] { BarInfos.Clipboard, BarInfos.Font, BarInfos.Alignment, BarInfos.Number, BarInfos.Styles, BarInfos.Cells, BarInfos.Editing };
		}
		public static BarInfo[] GetInsertBarInfos() {
			return new BarInfo[] { BarInfos.Tables, BarInfos.Illustrations, BarInfos.Charts, BarInfos.Links, BarInfos.Symbols };
		}
		public static BarInfo[] GetPageLayoutBarInfos() {
			return new BarInfo[] { BarInfos.PageSetup, BarInfos.PageLayoutShow, BarInfos.PageLayoutPrint, BarInfos.PageLayoutArrange };
		}
		public static BarInfo[] GetReviewBarInfos() {
			return new BarInfo[] { BarInfos.Comments, BarInfos.Changes };
		}
		public static BarInfo[] GetViewBarInfos() {
			return new BarInfo[] { BarInfos.Show, BarInfos.Zoom, BarInfos.Window };
		}
		public static BarInfo[] GetFormulaBarInfos() {
			return new BarInfo[] { BarInfos.FunctionLibrary, BarInfos.FormulaDefinedNames, BarInfos.FormulaAuditing, BarInfos.FormulaCalculation };
		}
		public static BarInfo[] GetDataBarInfos() {
			return new BarInfo[] { BarInfos.SortAndFilter, BarInfos.DataTools, BarInfos.Outline };
		}
		public static BarInfo[] GetChartToolsBarInfos() {
			return new BarInfo[] {
				BarInfos.ChartDesignChartData, BarInfos.ChartDesignChartLayouts,
				BarInfos.ChartToolsLayoutLabels, BarInfos.ChartToolsLayoutAxes, BarInfos.ChartToolsLayoutAnalysis,
				BarInfos.ChartToolsFormatArrange,
			};
		}
		public static BarInfo[] GetPictureToolsBarInfos() {
			return new BarInfo[] { BarInfos.PictureToolsFormatArrange };
		}
		public static BarInfo[] GetDrawingToolsBarInfos() {
			return new BarInfo[] { BarInfos.DrawingToolsFormatArrange };
		}
		public static BarInfo[] GetTableToolsBarInfos() {
			return new BarInfo[] { BarInfos.TableToolsDesignTools, BarInfos.TableToolsDesignStyleOptions };
		}
		public static BarInfo[] GetPivotTableToolsBarInfos() {
			return new BarInfo[] { BarInfos.PivotTableToolsAnalyzePivotTable, BarInfos.PivotTableToolsAnalyzeActiveField, BarInfos.PivotTableToolsAnalyzeData, BarInfos.PivotTableToolsAnalyzeActions,
				BarInfos.PivotTableToolsAnalyzeShow, BarInfos.PivotTableToolsDesignLayout, BarInfos.PivotTableToolsDesignPivotTableStyleOptions };
		}
	}
}
