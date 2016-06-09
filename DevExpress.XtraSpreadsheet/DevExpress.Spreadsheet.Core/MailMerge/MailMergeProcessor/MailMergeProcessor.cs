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

using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using System.Data;
namespace DevExpress.XtraSpreadsheet {
	public abstract class MailMergeProcessor {
		#region static
		public const string DividerString = ".";
		public static MailMergeProcessor CreateInstance(DocumentModel template) {
			MailMergeOptions options = new MailMergeOptions(template);
			MailMergeProcessor instance = null;
			if (options.MailMergeMode == MailMergeMode.OneDocumentMode)
				instance = new OneDocumentMailmergeProcessor(template.ActiveSheet, options);
			else if (options.MailMergeMode == MailMergeMode.DocumentsMode)
				instance = new DocumentsMailmergeProcessor(template.ActiveSheet, options);
			else
				instance = new OneSheetMailmergeProcessor(template.ActiveSheet, options);
			return instance;
		}
		#endregion
		#region fields
		MailMergeOptions options;
		SpreadsheetDataControllerAdapter adapter;
		GrowthStrategy growthStrategy;
		Dictionary<CellRange, CellRange> postProcessedHeaderRanges;
		GroupInfo rootInfo;
		#endregion
		protected MailMergeProcessor(Worksheet templateSheet, MailMergeOptions options) {
			this.options = options;
			adapter = new SpreadsheetDataControllerAdapter(new SpreadsheetDataController());
			growthStrategy = GrowthStrategy.CreateInstance(options.HorizontalMode, templateSheet);
			postProcessedHeaderRanges = new Dictionary<CellRange, CellRange>();
		}
		#region Properties
		public object DataSource { get { return adapter.DataSource; } set { adapter.DataSource = value; } }
		public string DataMember { get { return adapter.DataMember; } 
			set { 
				adapter.DataMember = value;
			} 
		}
		public List<SpreadsheetParameter> Parameters { get { return adapter.Parameters; } }
		protected MailMergeOptions Options { get { return options; } }
		protected SpreadsheetDataControllerAdapter DataAdapter { get { return adapter; } }
		protected GroupInfo RootInfo { get { return rootInfo; } }
		protected abstract bool NeedFinishList { get; }
		#endregion
		public abstract List<DocumentModel> Process();
		internal object GetValueByName(string name) {
			return GetValueBySimpleName(name);
		}
		object GetValueBySimpleName(string name) {
			object result = null;
			if (string.IsNullOrEmpty(name))
				result = DataAdapter.GetCurrentRowValue(0);
			else {
				result = DataAdapter.GetCurrentRowValue(name);
			}
			return result;
		}
		internal CellRangeBase GetRangeByCellRange(CellRangeBase positionRange) {
			CellRangeBase topLeftRange = growthStrategy.GetRangeByPosition(positionRange.TopLeft);
			if (positionRange.Height == 1 && positionRange.Width == 1)
				return topLeftRange;
			CellRangeBase bottomRightRange = growthStrategy.GetRangeByPosition(positionRange.BottomRight);
			return new CellRange(topLeftRange.Worksheet, topLeftRange.TopLeft, bottomRightRange.BottomRight);
		}
		protected void GenerateContent(Worksheet targetSheet) {
			growthStrategy.BeginGrow(targetSheet);
			CellRange targetHeaderRange = null;
			CellRange targetFooterRange = null;
			CellRange headerRange = Options.HeaderRange as CellRange;
			CellRange footerRange = Options.FooterRange as CellRange;
			if (Options.DetailRange != null)
				targetHeaderRange = growthStrategy.AppendRange(targetSheet, headerRange);
			CellRange sourceRange = Options.DetailRange as CellRange;
			bool useReportScenario = sourceRange != null;
			if (!useReportScenario)
				sourceRange = growthStrategy.TemplateSheet.GetPrintRange();
			DataAdapter.SetFilter(options.GetFilterInfoByDataMember(DataMember));
			if (options.IsGroupedRange(sourceRange)) {
				List<GroupInfo> groupInfo = options.GetGroupInfo(sourceRange);
				if (!DataAdapter.IsSorted)
					DataAdapter.SetGroup(groupInfo);
				rootInfo = groupInfo[0];
			}
			ProcessDetailRange(targetSheet, sourceRange);
			if (Options.DetailRange != null)
				targetFooterRange = growthStrategy.AppendRange(targetSheet, footerRange);
			ProcessRange(targetHeaderRange, headerRange);
			ProcessRange(targetFooterRange, footerRange);
			targetSheet.Workbook.CalculationChain.CalculateWorkbookIfHasMarkedCells();
			SetPageSettings(targetSheet);
		}
		void SetPageSettings(Worksheet targetSheet) {
			Worksheet templateSheet = growthStrategy.TemplateSheet;
			targetSheet.Margins.Bottom = templateSheet.Margins.Bottom;
			targetSheet.Margins.Top = templateSheet.Margins.Top;
			targetSheet.Margins.Right = templateSheet.Margins.Right;
			targetSheet.Margins.Left = templateSheet.Margins.Left;
			targetSheet.PrintSetup.PaperKind = templateSheet.PrintSetup.PaperKind;
			targetSheet.PrintSetup.PagePrintOrder = templateSheet.PrintSetup.PagePrintOrder;
			targetSheet.PrintSetup.Scale = templateSheet.PrintSetup.Scale;
			targetSheet.PrintSetup.CenterHorizontally = templateSheet.PrintSetup.CenterHorizontally;
			targetSheet.PrintSetup.CenterVertically = templateSheet.PrintSetup.CenterVertically;
			targetSheet.PrintSetup.Orientation = templateSheet.PrintSetup.Orientation;
			targetSheet.PrintSetup.PrintHeadings = templateSheet.PrintSetup.PrintHeadings;
			targetSheet.PrintSetup.PrintGridLines = templateSheet.PrintSetup.PrintGridLines;
		}
		protected virtual void ProcessDetailRange(Worksheet targetSheet, CellRange sourceRange) {
			ProcessDetailedRange(targetSheet, sourceRange);
		}
		protected void DetectedTrackedPositions() {
			growthStrategy.DetectedTrackedPositions();
		}
		void ProcessDetailedRange(Worksheet targetSheet, CellRange sourceRange) {
			if (sourceRange == null)
				return;
			Dictionary<CellRange, CellRange> postProcessedRanges = new Dictionary<CellRange, CellRange>();
			DetailedMap detailedMap = new DetailedMap(sourceRange, options, growthStrategy);
			if (options.IsGroupedRange(sourceRange)) {
				ProcessGroupRanges(targetSheet, sourceRange);
				if (DataAdapter.LastRowProcessed || NeedFinishList)
					return;
			}
			foreach (RangeInfo subRange in detailedMap.Ranges)
				if (subRange.IsDetailRange)
					ProcessDetailLevel(targetSheet, subRange.Range);
				else
					postProcessedRanges.Add(growthStrategy.AppendRange(targetSheet, subRange.Range), subRange.Range);
			foreach (KeyValuePair<CellRange, CellRange> processedRange in postProcessedRanges)
				ProcessRange(processedRange.Key, processedRange.Value);
			DataAdapter.GoToNextRow();
		}
		void ProcessGroupRanges(Worksheet targetSheet, CellRange sourceRange) {
			while (DataAdapter.NeedProcessGroupRanges && !DataAdapter.LastRowProcessed && !NeedFinishList) {
				GroupInfo info = options.GetGroupInfo(sourceRange, DataAdapter.GetCurrentHeaderFooterGroupedName());
				CellRange header = info.Header as CellRange;
				CellRange footer = info.Footer as CellRange;
				if (DataAdapter.IsGroupHeader)
					PreProcessGroupRanges(targetSheet, header);
				else {
					PostProcessGroupRanges(targetSheet, header, footer);
					if (info == rootInfo)
						rootInfo = null;
				}
				DataAdapter.GoToNextRow();
			}
		}
		void PreProcessGroupRanges(Worksheet targetSheet, CellRange header) {
			if (header != null)
				postProcessedHeaderRanges.Add(header, growthStrategy.AppendRange(targetSheet, header));
		}
		void PostProcessGroupRanges(Worksheet targetSheet, CellRange header, CellRange footer) {
			if (header != null) {
				ProcessRange(postProcessedHeaderRanges[header], header);
				postProcessedHeaderRanges.Remove(header);
			}
			if (footer != null)
				ProcessRange(growthStrategy.AppendRange(targetSheet, footer), footer);
		}
		void ProcessDetailLevel(Worksheet targetSheet, CellRange detailLevel) {
			string dataMember = options.DataMembers[options.DetailLevels.IndexOf(detailLevel)];
			object dataSource = this.DataAdapter.GetCurrentRowValue(dataMember);
			if (dataSource == null)
				return;
			DataAdapter.SaveCurrentState();
			DataAdapter.ResetState();
			DataAdapter.DataSource = dataSource;
			DataAdapter.DataMember = dataMember;
			DataAdapter.SetFilter(options.GetFilterInfoByDataMember(dataMember));
			if (options.IsGroupedRange(detailLevel))
				DataAdapter.SetGroup(options.GetGroupInfo(detailLevel));
			while (!DataAdapter.LastRowProcessed)
				ProcessDetailedRange(targetSheet, detailLevel);
			DataAdapter.ReturnToLastState();
		}
		void ProcessRange(CellRange range, CellRange sourceRange) {
			if (range == null)
				return;
			growthStrategy.SetOffset(range, sourceRange);
			DocumentModel workbook = (DocumentModel)range.Worksheet.Workbook;
			CalculationChain calculationChain = workbook.CalculationChain;
			WorkbookDataContext context = workbook.DataContext;
			foreach (ICellBase info in range.GetExistingCellsEnumerable()) {
				ICell cell = info as ICell;
				if (cell == null || !cell.HasFormula)
					continue;
				growthStrategy.SetWatchingPosition(cell.Position);
				FormulaBase formula = cell.GetFormula();
				if (formula.Expression != null && formula.Expression.ContainsInternalFunction) {
					CustomFunctionReplaceVisitor2 visitor = new CustomFunctionReplaceVisitor2(context);
					context.PushCurrentCell(cell);
					bool suppressAutoStartCalculation = workbook.SuppressAutoStartCalculation;
					workbook.SuppressAutoStartCalculation = true;
					try { formula.UpdateFormula(visitor, cell); }
					finally {
						context.PopCurrentCell();
						workbook.SuppressAutoStartCalculation = suppressAutoStartCalculation;
					}
					if (visitor.FormulaChanged && SimpleExpressionChecker.CheckExpression(formula.Expression)) {
						calculationChain.CalculateCell(cell);
						cell.ApplyFormula(null);
					}
				}
			}
		}
	}
}
