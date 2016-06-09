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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChartTextType
	public enum ChartTextType {
		None,
		Auto,
		Value,
		Ref,
		Rich
	}
	#endregion
	#region IChartText
	public interface IChartText : IEquatable<IChartText> {
		ChartTextType TextType { get; }
		string PlainText { get; }
		string[] Lines { get; }
		IChartText CloneTo(IChart parent);
		void Visit(IChartTextVisitor visitor);
		bool CheckUpdates();
		void OnRangeInserting(InsertRangeNotificationContext context);
		void OnRangeRemoving(RemoveRangeNotificationContext context);
	}
	#endregion
	#region IChartTextOwner
	public interface IChartTextOwner {
		IChartText Text { get; set; }
		IChart Parent { get; }
		void SetTextCore(IChartText value);
	}
	#endregion
	#region IChartTextOwnerEx
	public interface IChartTextOwnerEx : IChartTextOwner {
		TextProperties TextProperties { get; }
	}
	#endregion
	#region IChartTextVisitor
	public interface IChartTextVisitor {
		void Visit(ChartText value);
		void Visit(ChartTextValue value);
		void Visit(ChartTextRef value);
		void Visit(ChartRichText value);
	}
	#endregion
	#region ChartText
	public class ChartText : IChartText {
		ChartTextType textType;
		static IChartText empty = new ChartText(ChartTextType.None);
		static IChartText auto = new ChartText(ChartTextType.Auto);
		public static IChartText Empty {
			get { return empty; }
		}
		public static IChartText Auto {
			get { return auto; }
		}
		ChartText(ChartTextType textType) {
			this.textType = textType;
		}
		#region IChartText Members
		public ChartTextType TextType {
			get { return textType; }
		}
		public string PlainText {
			get { return string.Empty; }
		}
		public string[] Lines {
			get { return new string[0]; }
		}
		public bool Equals(IChartText value) {
			if (value == null)
				return false;
			return TextType == value.TextType;
		}
		public IChartText CloneTo(IChart parent) {
			if (TextType == ChartTextType.Auto)
				return ChartText.Auto;
			return ChartText.Empty;
		}
		public void Visit(IChartTextVisitor visitor) {
			visitor.Visit(this);
		}
		public bool CheckUpdates() {
			return false;
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
		}
		#endregion
		#endregion
	}
	#endregion
	#region ChartTextBase
	public abstract class ChartTextBase {
		#region Fields
		readonly IChart parent;
		#endregion
		protected ChartTextBase(IChart parent) {
			this.parent = parent;
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public string PlainText {
			get { return GetPlainText(); }
			set {
				if (string.IsNullOrEmpty(value))
					value = string.Empty;
				if (GetPlainText() == value)
					return;
				SetPlainText(value);
			}
		}
		protected abstract string GetPlainText();
		protected abstract void SetPlainText(string value);
		#endregion
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
		}
		#endregion
	}
	#endregion
	#region ChartPlainText
	public abstract class ChartPlainText : ChartTextBase {
		#region Fields
		string plainText;
		#endregion
		protected ChartPlainText(IChart parent)
			: base(parent) {
			this.plainText = string.Empty;
		}
		protected override string GetPlainText() {
			return this.plainText;
		}
		protected override void SetPlainText(string value) {
			ChartTextPlainTextPropertyChangedHistoryItem historyItem = new ChartTextPlainTextPropertyChangedHistoryItem(DocumentModel, this, plainText, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetPlainTextCore(string value) {
			this.plainText = value;
			Parent.Invalidate();
		}
		public string[] Lines {
			get { return new string[] { PlainText }; }
		}
	}
	#endregion
	#region ChartTextValue
	public class ChartTextValue : ChartPlainText, IChartText, ISupportsCopyFrom<ChartTextValue> {
		#region Static Members
		internal static ChartTextValue Create(IChart chart, string seriesName) {
			ChartTextValue result = new ChartTextValue(chart);
			result.PlainText = seriesName;
			return result;
		}
		#endregion
		public ChartTextValue(IChart parent)
			: base(parent) {
		}
		#region IChartText Members
		public ChartTextType TextType { get { return ChartTextType.Value; } }
		public bool Equals(IChartText value) {
			ChartTextValue other = value as ChartTextValue;
			if (other == null)
				return false;
			return TextType == other.TextType && PlainText == other.PlainText;
		}
		public IChartText CloneTo(IChart parent) {
			ChartTextValue result = new ChartTextValue(parent);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IChartTextVisitor visitor) {
			visitor.Visit(this);
		}
		public bool CheckUpdates() {
			return false;
		}
		#endregion
		#region ISupportsCopyFrom<ChartTextValue> Members
		public void CopyFrom(ChartTextValue value) {
			Guard.ArgumentNotNull(value, "value");
			PlainText = value.PlainText;
		}
		#endregion
	}
	#endregion
	#region ChartTextRef
	public class ChartTextRef : IChartText, ISupportsCopyFrom<ChartTextRef>, ISupportsInvalidate {
		#region Static Members
		internal static ChartTextRef Create(IChart chart, CellRangeBase range) {
			ChartTextRef result = new ChartTextRef(chart);
			result.SetRange(range);
			return result;
		}
		internal static ChartTextRef Create(IChart chart, string seriesName) {
			ChartTextRef result = new ChartTextRef(chart);
			result.FormulaBody = seriesName;
			return result;
		}
		#endregion
		#region Fields
		readonly IChart parent;
		readonly FormulaData formulaData;
		FormulaReferencedRanges referencedRanges;
		string previousText = null;
		#endregion
		public ChartTextRef(IChart parent) {
			this.parent = parent;
			this.formulaData = new FormulaData(DocumentModel); 
			this.formulaData.Parent = this;
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public FormulaData FormulaData { get { return formulaData; } }
		public string FormulaBody {
			get { return formulaData.FormulaBody; }
			set {
				formulaData.FormulaBody = value;
				ResetReferencedRanges();
			}
		}
		public VariantValue CachedValue { get { return formulaData.CachedValue; } set { formulaData.CachedValue = value; } }
		public ParsedExpression Expression { get { return formulaData.Expression; } set { formulaData.Expression = value; } }
		public WorkbookDataContext Context { get { return DocumentModel.DataContext; } }
		#endregion
		public void SetRange(CellRangeBase cellRange) {
			formulaData.SetRange(cellRange);
			ResetReferencedRanges();
		}
		void ResetReferencedRanges() {
			this.referencedRanges = null;
		}
		public void ObtainReferencedRanges(FormulaReferencedRanges where) {
			if (referencedRanges != null) {
				where.AddRange(referencedRanges);
				return;
			}
			ParsedExpression expression = DocumentModel.DataContext.ParseExpression("=" + this.FormulaBody, OperandDataType.Default, false);
			VariantValue value = expression.Evaluate(DocumentModel.DataContext);
			if (value.IsCellRange) {
				referencedRanges = new FormulaReferencedRanges();
				foreach (CellRange range in value.CellRangeValue.GetAreasEnumerable()) {
					FormulaReferencedRange formulaReferencedRange = new FormulaReferencedRange(range, 0, 0, true);
					referencedRanges.Add(formulaReferencedRange);
				}
				where.AddRange(referencedRanges);
			}
		}
		#region IChartText Members
		public ChartTextType TextType { get { return ChartTextType.Ref; } }
		public bool Equals(IChartText value) {
			ChartTextRef other = value as ChartTextRef;
			if (other == null)
				return false;
			return TextType == other.TextType && FormulaBody == other.FormulaBody;
		}
		public IChartText CloneTo(IChart parent) {
			ChartTextRef result = new ChartTextRef(parent);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IChartTextVisitor visitor) {
			visitor.Visit(this);
		}
		public bool CheckUpdates() {
			VariantValue currentValue = FormulaData.CurrentValue;
			string currentText = GetPlainText(GetLines(currentValue));
			bool result = currentText != previousText;
			if (result) {
				previousText = currentText;
				CachedValue = currentValue;
			}
			return result;
		}
		public string PlainText { get { return GetPlainText(Lines); } }
		public string[] Lines { get { return GetLines(CachedValue); } }
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			formulaData.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			formulaData.OnRangeRemoving(context);
		}
		#endregion
		#endregion
		#region ISupportsCopyFrom<ChartTextRef> Members
		public void CopyFrom(ChartTextRef value) {
			Guard.ArgumentNotNull(value, "value");
			if (Object.ReferenceEquals(value.DocumentModel, DocumentModel))
				this.FormulaData.FormulaBody = value.FormulaData.FormulaBody;
			else {
				this.FormulaData.Expression = value.FormulaData.Expression.Clone();
				this.CachedValue = VariantValue.Empty;
			}
		}
		#endregion
		#region Get text lines
		string GetPlainText(string[] lines) {
			if(lines == null)
				return string.Empty;
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < lines.Length; i++)			{
				builder.Append(lines[i]);
				builder.Append(" ");
			}
			if(builder.Length > 0)
				builder.Remove(builder.Length - 1, 1);
			return builder.ToString();
		}
		string[] GetLines(VariantValue cachedValue) {
			switch (cachedValue.Type) {
				case VariantValueType.None:
				case VariantValueType.Missing:
					return new string[0];
				case VariantValueType.Array:
					return GetLinesFromArray(cachedValue.ArrayValue, DocumentModel.DataContext);
				case VariantValueType.CellRange:
					return GetLinesFromCellRange(cachedValue.CellRangeValue, DocumentModel.DataContext);
				default:
					return GetLinesFromValue(cachedValue, DocumentModel.DataContext);
			}
		}
		string[] GetLinesFromArray(IVariantArray variantArray, WorkbookDataContext context) {
			List<string> list = new List<string>();
			int pointCount = (int)variantArray.Count;
			for (int i = 0; i < pointCount; i++) {
				VariantValue value = variantArray[i];
				if (value.IsEmpty || value.IsMissing)
					continue;
				list.Add(ConvertValueToString(value, context));
			}
			return list.ToArray();
		}
		string[] GetLinesFromCellRange(CellRangeBase cellRangeBase, WorkbookDataContext context) {
			List<string> list = new List<string>();
			foreach (VariantValue value in new Enumerable<VariantValue>(cellRangeBase.GetExistingValuesEnumerator())) {
				if (value.IsEmpty || value.IsMissing)
					continue;
				list.Add(ConvertValueToString(value, context));
			}
			return list.ToArray();
		}
		string[] GetLinesFromValue(VariantValue value, WorkbookDataContext context) {
			string[] result = new string[1];
			result[0] = ConvertValueToString(value, context);
			return result;
		}
		string ConvertValueToString(VariantValue value, WorkbookDataContext context) {
			return value.ToText(context).GetTextValue(context.Workbook.SharedStringTable);
		}
		#endregion
		#region ISupportsInvalidate Members
		public void Invalidate() {
			ResetReferencedRanges();
			parent.Invalidate();
		}
		#endregion
	}
	#endregion
	#region ChartRichText
	#region ChartRichText
	public class ChartRichText : ChartTextBase, IChartText, ISupportsCopyFrom<ChartRichText> {
		#region Fields
		readonly TextProperties textProperties;
		#endregion
		public ChartRichText(IChart parent)
			: base(parent) {
			this.textProperties = new TextProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		public TextProperties TextProperties { get { return textProperties; } }
		public DrawingTextParagraphCollection Paragraphs { get { return textProperties.Paragraphs; } }
		public DrawingTextBodyProperties BodyProperties { get { return textProperties.BodyProperties; } }
		public DrawingTextListStyles ListStyles { get { return textProperties.ListStyles; } }
		#endregion
		protected override string GetPlainText() {
			DrawingTextParagraphCollection paragraphs = Paragraphs;
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < paragraphs.Count; i++) {
				string text = paragraphs[i].GetPlainText();
				sb.Append(text);
				sb.Append('\n');
			}
			if (sb.Length > 0)
				sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}
		protected override void SetPlainText(string value) {
			DrawingTextParagraphCollection paragraphs = Paragraphs;
			paragraphs.Clear();
			if (string.IsNullOrEmpty(value))
				return;
			DrawingTextParagraph item = new DrawingTextParagraph(this.DocumentModel);
			item.SetPlainText(value);
			item.ApplyParagraphProperties = true;
			item.ParagraphProperties.ApplyDefaultCharacterProperties = true;
			paragraphs.Add(item);
		}
		#region IChartText Members
		public ChartTextType TextType { get { return ChartTextType.Rich; } }
		public string[] Lines {
			get {
				DrawingTextParagraphCollection paragraphs = Paragraphs;
				List<string> lines = new List<string>(paragraphs.Count);
				for (int i = 0; i < paragraphs.Count; i++)
					lines.Add(paragraphs[i].GetPlainText());
				return lines.ToArray();
			}
		}
		public bool Equals(IChartText value) {
			ChartRichText other = value as ChartRichText;
			if (other == null)
				return false;
			if (TextType != other.TextType)
				return false;
			return textProperties.Equals(other.textProperties);
		}
		public IChartText CloneTo(IChart parent) {
			ChartRichText result = new ChartRichText(parent);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IChartTextVisitor visitor) {
			visitor.Visit(this);
		}
		public bool CheckUpdates() {
			return false;
		}
		#endregion
		#region ISupportsCopyFrom<ChartRichText> Members
		public void CopyFrom(ChartRichText value) {
			Guard.ArgumentNotNull(value, "value");
			textProperties.CopyFrom(value.TextProperties);
		}
		#endregion
	}
	#endregion
	#endregion
	#region ChartTitleAutoText
	public class ChartTitleAutoText : IChartText {
		#region Fields
		readonly IChart parent;
		#endregion
		public ChartTitleAutoText(IChart parent) {
			this.parent = parent;
		}
		#region IChartText Members
		public ChartTextType TextType { get { return ChartTextType.Auto; } }
		public string PlainText {
			get {
				string result = string.Empty;
				ChartViewCollection views = this.parent.Views;
				if (views.Count == 1 && (views[0].IsSingleSeriesView || views[0].Series.Count == 1))
					result = views[0].Series[0].Text.PlainText;
				if (string.IsNullOrEmpty(result))
					result = "Chart Title";
				return result;
			}
		}
		public string[] Lines { get { return new string[] { PlainText }; } }
		public IChartText CloneTo(IChart parent) { 
			return new ChartTitleAutoText(parent); 
		}
		public bool CheckUpdates() {
			return false;
		}
		public void Visit(IChartTextVisitor visitor) {
			visitor.Visit((ChartText)ChartText.Auto);
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
		}
		#endregion
		#endregion
		#region IEquatable<IChartText> Members
		public bool Equals(IChartText other) {
			if (other == null)
				return false;
			return TextType == other.TextType;
		}
		#endregion
	}
	#endregion
	#region AxisTitleAutoText
	public class AxisTitleAutoText : IChartText {
		static IChartText instance = new AxisTitleAutoText();
		public static IChartText Instance {
			get { return instance; }
		}
		AxisTitleAutoText() {
		}
		#region IChartText Members
		public ChartTextType TextType { get { return ChartTextType.Auto; } }
		public string PlainText { get { return "Axis Title"; } }
		public string[] Lines { get { return new string[] { PlainText }; } }
		public IChartText CloneTo(IChart parent) {
			return AxisTitleAutoText.Instance; 
		}
		public void Visit(IChartTextVisitor visitor) {
			visitor.Visit((ChartText)ChartText.Auto);
		}
		public bool CheckUpdates() {
			return false;
		}
		#endregion
		#region IEquatable<IChartText> Members
		public bool Equals(IChartText other) {
			if (other == null)
				return false;
			return TextType == other.TextType;
		}
		#endregion
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
		}
		#endregion
	}
	#endregion
}
