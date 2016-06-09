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
using DevExpress.Utils;
using System.Collections;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Localization;
using System.Text.RegularExpressions;
using DevExpress.Office.History;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IHyperlinkViewInfo {
		string DisplayText { get; set; }
		string TooltipText { get; set; }
		string TargetUri { get; }
		bool IsExternal { get; set; }
		CellRange Range { get; }
		Worksheet Worksheet { get; }
		DocumentModel Workbook { get; }
		ParsedExpression Expression { get; }
		bool IsDrawingObject { get; }
		void SetTargetUriWithoutHistory(string uri);
		CellRangeBase GetTargetRange();
	}
	public sealed class HyperlinkExpressionParser {
		private HyperlinkExpressionParser() {
		}
		public static string GetAbsoluteFilePath(string targetUri, DocumentModel documentModel) {
			Uri uri = new Uri(targetUri, UriKind.RelativeOrAbsolute);
			if (!uri.IsAbsoluteUri) {
				string fileNameWithPath = documentModel.DocumentSaveOptions.CurrentFileName;
				char directorySeparator = System.IO.Path.DirectorySeparatorChar;
				return fileNameWithPath.Substring(0, fileNameWithPath.LastIndexOf(directorySeparator) + 1) + targetUri;
			}
			return targetUri;
		}
		public static ParsedExpression Parse(WorkbookDataContext context, string uri, bool isExternal) {
			if (isExternal)
				return null;
			ParsedExpression result = context.ParseExpression(uri, OperandDataType.Reference, false);
			if (result == null) {
				bool useR1C1 = context.UseR1C1ReferenceStyle;
				try {
					context.PushUseR1C1(!useR1C1);
					result = context.ParseExpression(uri, OperandDataType.Reference, false);
				}
				finally {
					context.PopUseR1C1();
				}
			}
			return result;
		}
		public static CellRangeBase GetTargetRange(WorkbookDataContext context, ParsedExpression expression, bool isExternal) {
			if (isExternal || expression == null)
				return null;
			VariantValue value = expression.Evaluate(context);
			if (!value.IsCellRange)
				return null;
			return value.CellRangeValue;
		}
		public static bool IsFilePath(string targetUri) {
			if (targetUri.Contains("mailto:"))
				return false;
			string isWebPagePattern = @"^http(s)?:";
			Regex isWebPageRegex = new Regex(isWebPagePattern);
			return !isWebPageRegex.IsMatch(targetUri);
		}
		public static bool IsWebPage(string uri) {
			if (uri.Contains("mailto:"))
				return false;
			string isWebPagePattern = @"^http(s)?:";
			Regex isWebPageRegex = new Regex(isWebPagePattern);
			return isWebPageRegex.IsMatch(uri);
		}
		public static bool IsValidateWebPageUri(string uri) {
			string urlPattern = @"^http(s)?:[/\\]{0,2}[^\s/\\]*[\s]+";
			Regex urlRegex = new Regex(urlPattern);
			return !urlRegex.IsMatch(uri);
		}
		public static bool IsValidateFileUri(string uri) {
			if (uri.StartsWith("file:", StringComparison.OrdinalIgnoreCase)) {
				string urlPattern = @"^file:[/\\]*[^\s/\\]+[/\\]+";
				Regex urlRegex = new Regex(urlPattern);
				if (urlRegex.IsMatch(uri))
					return true;
				urlPattern = @"^file:[/\\]*[^\s/\\]+[\s]*$";
				urlRegex = new Regex(urlPattern);
				return urlRegex.IsMatch(uri);
			}
			return !String.IsNullOrEmpty(uri);
		}
		public static ParsedExpression ValidateCellRefExpression(WorkbookDataContext context, string sheetName, string cellReference, bool correctAbsoluteReferences) {
			ParsedExpression expression = context.ParseExpression("=" + cellReference, OperandDataType.Default, false);
			if (expression == null || expression.Count != 1)
				return null;
			IParsedThing thing = expression[0];
			ParsedThingArea3d area3dThing = thing as ParsedThingArea3d;
			if (area3dThing != null) {
				if (correctAbsoluteReferences)
					area3dThing.CellRange = CorrectCellRangePositionType(area3dThing.CellRange, context.UseR1C1ReferenceStyle);
				return expression;
			}
			ParsedThingRef3d ref3dThing = thing as ParsedThingRef3d;
			if (ref3dThing != null) {
				if (correctAbsoluteReferences)
					ref3dThing.Position = CorrectCellPositionType(ref3dThing.Position, context.UseR1C1ReferenceStyle);
				return expression;
			}
			expression.Clear();
			ParsedThingRef refThing = thing as ParsedThingRef;
			if (refThing != null) {
				CellPosition position = refThing.Position;
				if (correctAbsoluteReferences)
					position = CorrectCellPositionType(position, context.UseR1C1ReferenceStyle);
				expression.Add(new ParsedThingRef3d(position, GetSheetDefinitionIndex(context, sheetName)));
				return expression;
			}
			ParsedThingArea areaThing = thing as ParsedThingArea;
			if (areaThing != null) {
				CellRange range = areaThing.CellRange;
				if (correctAbsoluteReferences)
					range = CorrectCellRangePositionType(range, context.UseR1C1ReferenceStyle);
				expression.Add(new ParsedThingArea3d(range, GetSheetDefinitionIndex(context, sheetName)));
				return expression;
			}
			return null;
		}
		static CellRange CorrectCellRangePositionType(CellRange range, bool isAbsolute) {
			CellPosition topLeft = CorrectCellPositionType(range.TopLeft, isAbsolute);
			CellPosition bottomRight = CorrectCellPositionType(range.BottomRight, isAbsolute);
			return new CellRange(range.Worksheet, topLeft, bottomRight);
		}
		static CellPosition CorrectCellPositionType(CellPosition position, bool isAbsolute) {
			PositionType type = isAbsolute ? PositionType.Absolute : PositionType.Relative;
			return new CellPosition(position.Column, position.Row, type, type);
		}
		static int GetSheetDefinitionIndex(WorkbookDataContext context, string sheetName) {
			SheetDefinition sheetDefinition = new SheetDefinition();
			sheetDefinition.SheetNameStart = sheetName;
			return context.RPNContext.IndexOfSheetDefinition(sheetDefinition);
		}
	}
	public static class HyperlinkToolTipCalculatior {
		public static string GetActualToolTip(IHyperlinkViewInfo hyperlink) {
			if (!String.IsNullOrEmpty(hyperlink.TooltipText))
				return hyperlink.TooltipText;
			string result = GetPrefix(hyperlink) + hyperlink.TargetUri;
			if (hyperlink.IsDrawingObject) 
				return result;
			return result + " - " + XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Tooltip_Hyperlink);
		}
		static string GetPrefix(IHyperlinkViewInfo hyperlink) {
			if (!hyperlink.IsExternal)
				return GetFileName(hyperlink) + " - ";
			if (hyperlink.TargetUri.Contains(@"\"))
				return "file:///";
			return String.Empty;
		}
		static string GetFileName(IHyperlinkViewInfo hyperlink) {
			WorkbookSaveOptions saveOptions = hyperlink.Workbook.DocumentSaveOptions;
			bool isNewDocument = String.IsNullOrEmpty(saveOptions.CurrentFileName);
			if (isNewDocument)
				return !String.IsNullOrEmpty(saveOptions.DefaultFileName) ? saveOptions.DefaultFileName : "Book";
			return "file:///" + saveOptions.CurrentFileName;
		}
	}
	#region ModelHyperlink
	public class ModelHyperlink : IHyperlinkViewInfo {
		CellRange innerRange;
		string displayText = String.Empty;
		string tooltipText = String.Empty;
		string targetUri;
		bool isExternal;
		readonly Worksheet sheet;
		ParsedExpression expression;
		public ModelHyperlink(Worksheet sheet, CellRange range, string uri, bool isExternal) {
			Guard.ArgumentNotNull(sheet, "sheet");
			Guard.ArgumentNotNull(range, "range");
			Guard.ArgumentNotNull(uri, "uri");
			this.sheet = sheet;
			this.innerRange = range;
			this.targetUri = uri;
			this.expression = null;
			this.isExternal = isExternal;
		}
		public ModelHyperlink(Worksheet sheet, CellRange range, string uri, bool isExternal, string displayText, string toolTiptext)
			: this(sheet, range, uri, isExternal) {
			this.displayText = displayText;
			this.tooltipText = toolTiptext;
		}
		public Worksheet Worksheet { get { return sheet; } }
		public DocumentModel Workbook { get { return sheet.Workbook; } }
		public CellRange Range { get { return innerRange; } }
		protected WorkbookDataContext DataContext { get { return this.Worksheet.Workbook.DataContext; } }
		public string DisplayText {
			get {
				return displayText;
			}
			set {
				if (displayText == value)
					return;
				DocumentHistory history = Workbook.History;
				HyperlinkChangeDisplayTextHistoryItem historyItem = new HyperlinkChangeDisplayTextHistoryItem(this, displayText, value);
				history.Add(historyItem);
				historyItem.Execute();
			}
		}
		public string TooltipText {
			get {
				return tooltipText;
			}
			set {
				if (tooltipText == value)
					return;
				DocumentHistory history = Workbook.History;
				HyperlinkChangeTooltipTextHistoryItem historyItem = new HyperlinkChangeTooltipTextHistoryItem(this, tooltipText, value);
				history.Add(historyItem);
				historyItem.Execute();
			}
		}
		public string TargetUri { get { return GetTargetUri(); } }
		public bool IsDrawingObject { get { return false; } }
		protected internal virtual string GetTargetUri() {
			ParsedExpression expression = Expression;
			if (expression != null)
				return expression.BuildExpressionString(Workbook.DataContext);
			return targetUri;
		}
		public bool IsExternal {
			get {
				return isExternal;
			}
			set {
				if (isExternal == value)
					return;
				DocumentHistory history = Workbook.History;
				HyperlinkChangeIsExternalHistoryItem historyItem = new HyperlinkChangeIsExternalHistoryItem(this, isExternal, value);
				history.Add(historyItem);
				historyItem.Execute();
			}
		}
		public ParsedExpression Expression {
			get {
				if (expression == null)
					expression = HyperlinkExpressionParser.Parse(Workbook.DataContext, targetUri, IsExternal);
				return expression;
			}
		}
		public void SetRange(CellRange newHyperlinkRange) {
			if (newHyperlinkRange == null)
				throw new ArgumentNullException("newHyperlinkRange");
			if (!Object.ReferenceEquals(newHyperlinkRange.Worksheet, innerRange.Worksheet))
				throw new ArgumentException("newHyperlinkRange is from another Worksheet");
			if (innerRange.EqualsPosition(newHyperlinkRange))
				return;
			DocumentHistory history = Workbook.History;
			HyperlinkChangeReferenceHistoryItem historyItem = new HyperlinkChangeReferenceHistoryItem(this, innerRange, newHyperlinkRange);
			history.Add(historyItem);
			historyItem.Execute();
		}
		public void SetTargetUriWithoutHistory(string uri) {
			this.targetUri = uri;
			this.expression = null;
		}
		protected internal void SetReferenceCore(CellRange range) {
			this.innerRange = range;
		}
		protected internal void SetIsExternalCore(bool isExternal) {
			this.isExternal = isExternal;
		}
		protected internal void SetDisplayTextCore(string displayText) {
			this.displayText = displayText;
		}
		protected internal void SetTooltipTextCore(string tooltipText) {
			this.tooltipText = tooltipText;
		}
		public CellRangeBase GetTargetRange() {
			return HyperlinkExpressionParser.GetTargetRange(sheet.DataContext, Expression, isExternal);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			CellRange newRange = notificationContext.Visitor.ProcessCellRange(Range.Clone()) as CellRange;
			SetRange(newRange);
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			CellRange newRange = notificationContext.Visitor.ProcessCellRange(Range.Clone()) as CellRange;
			SetRange(newRange);
		}
		public void CopyFrom(ModelHyperlink sourceHyperlink) {
			this.isExternal = sourceHyperlink.IsExternal;
			this.SetTargetUriWithoutHistory(sourceHyperlink.TargetUri);
			this.DisplayText = sourceHyperlink.DisplayText;
			this.TooltipText = sourceHyperlink.TooltipText;
			System.Diagnostics.Debug.Assert(this.Range != null);
		}
	}
	#endregion
	#region ModelHyperlinkCollection
	public partial class ModelHyperlinkCollection : UndoableCollection<ModelHyperlink> {
		readonly Worksheet sheet;
		public ModelHyperlinkCollection(Worksheet worksheet) 
			: base(worksheet) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.sheet = worksheet;
		}
		public Worksheet Worksheet { get { return sheet; } }
		#region GetHyperlink
		public int GetHyperlink(ICell modelCell) {
			for (int i = Count - 1; i >= 0; i--) {
				ModelHyperlink hyperlink = this[i];
				if (hyperlink.Range.ContainsCell(modelCell))
					return i;
			}
			return Int32.MinValue;
		}
		#endregion
		#region GetHyperlink
		public int GetHyperlink(CellKey cellKey) {
			for (int i = Count - 1; i >= 0; i--) {
				ModelHyperlink hyperlink = this[i];
				if (hyperlink.Range.ContainsCell(cellKey))
					return i;
			}
			return Int32.MinValue;
		}
		#endregion
		#region GetHyperlink
		public ModelHyperlink GetHyperlink(CellRange range) {
			for (int i = Count - 1; i >= 0; i--) {
				ModelHyperlink hyperlink = this[i];
				if (range.Intersects(hyperlink.Range))
					return hyperlink;
			}
			return null;
		}
		#endregion
		#region GetIndexesInsideRangeSortedDescending
		public List<int> GetIndexesInsideRangeSortedDescending(Predicate<CellRange> hyperlinkRange) {
			List<int> result = new List<int>();
			for (int i = Count - 1; i >= 0; i--) {
				ModelHyperlink hyperlink = this[i];
				bool condition = hyperlinkRange(hyperlink.Range);
				if (condition)
					result.Add(i);
			}
			return result;
		}
		#endregion
		#region OnRangeRemoving
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			for (int i = Count - 1; i >= 0; i--) {
				if (this.sheet.SheetId == context.Range.SheetId && context.Range.Includes(this[i].Range))
					sheet.RemoveHyperlinkAt(i);
				else
					this[i].OnRangeRemoving(context);
			}
		}
		#endregion
		#region OnRangeInserting
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			for (int i = Count - 1; i >= 0; i--)
				this[i].OnRangeInserting(context);
		}
		#endregion
		#region ContainsHyperlink
		public bool ContainsHyperlink(CellRange range) {
			for (int i = Count - 1; i >= 0; i--) {
				ModelHyperlink hyperlink = this[i];
				if (range.Intersects(hyperlink.Range))
					return true;
			}
			return false;
		}
		#endregion
		#region ContainsHyperlink
		public bool ContainsHyperlink(IList<CellRange> ranges) {
			foreach (CellRange range in ranges)
				if (ContainsHyperlink(range))
					return true;
			return false;
		}
		#endregion
	}
	#endregion
}
