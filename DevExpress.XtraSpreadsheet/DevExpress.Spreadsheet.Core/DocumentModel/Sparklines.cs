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
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SparklineGroupType (enum)
	public enum SparklineGroupType {
		Line = 0,
		Column = 1,
		Stacked = 2
	}
	#endregion
	#region SparklineAxisScaling (enum)
	public enum SparklineAxisScaling {
		Individual = 0,
		Group = 1,
		Custom = 2
	}
	#endregion
	#region SparklineColorType (enum)
	public enum SparklineColorType {
		Series = 0,
		Negative = 1,
		Axis = 2,
		Markers = 3,
		First = 4,
		Last = 5,
		Highest = 6,
		Lowest = 7
	}
	#endregion
	#region SparklineGroupInfo
	public class SparklineGroupInfo : ICloneable<SparklineGroupInfo>, ISupportsCopyFrom<SparklineGroupInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskType = 0x00000003;			 
		const uint MaskMinAxisScaleType = 0x0000000C; 
		const uint MaskMaxAxisScaleType = 0x00000030; 
		const uint MaskDisplayBlanksAs = 0x000000C0;  
		const uint MaskUseDateAxis = 0x00000100;	  
		const uint MaskShowMarkers = 0x00000200;	  
		const uint MaskShowHighest = 0x00000400;	  
		const uint MaskShowLowest = 0x00000800;	   
		const uint MaskShowFirst = 0x00001000;		
		const uint MaskShowLast = 0x00002000;		 
		const uint MaskShowNegative = 0x00004000;	 
		const uint MaskShowXAxis = 0x00008000;		
		const uint MaskShowHidden = 0x00010000;	   
		const uint MaskRightToLeft = 0x00020000;	  
		uint packedValues;
		double maxAxisValue;
		double minAxisValue;
		double lineWeight = 0.75;
		#endregion
		#region Properties
		public SparklineGroupType Type {
			get { return (SparklineGroupType)PackedValues.GetIntBitValue(packedValues, MaskType, 0); }
			set { PackedValues.SetIntBitValue(ref packedValues, MaskType, 0, (int)value); }
		}
		public SparklineAxisScaling MinAxisScaleType {
			get { return (SparklineAxisScaling)PackedValues.GetIntBitValue(packedValues, MaskMinAxisScaleType, 2); }
			set { PackedValues.SetIntBitValue(ref packedValues, MaskMinAxisScaleType, 2, (int)value); }
		}
		public SparklineAxisScaling MaxAxisScaleType {
			get { return (SparklineAxisScaling)PackedValues.GetIntBitValue(packedValues, MaskMaxAxisScaleType, 4); }
			set { PackedValues.SetIntBitValue(ref packedValues, MaskMaxAxisScaleType, 4, (int)value); }
		}
		public DisplayBlanksAs DisplayBlanksAs {
			get { return (DisplayBlanksAs)PackedValues.GetIntBitValue(packedValues, MaskDisplayBlanksAs, 6); }
			set { PackedValues.SetIntBitValue(ref packedValues, MaskDisplayBlanksAs, 6, (int)value); }
		}
		public bool UseDateAxis {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskUseDateAxis); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskUseDateAxis, value); }
		}
		public bool ShowMarkers {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskShowMarkers); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskShowMarkers, value); }
		}
		public bool ShowHighest {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskShowHighest); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskShowHighest, value); }
		}
		public bool ShowLowest {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskShowLowest); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskShowLowest, value); }
		}
		public bool ShowFirst {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskShowFirst); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskShowFirst, value); }
		}
		public bool ShowLast {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskShowLast); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskShowLast, value); }
		}
		public bool ShowNegative {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskShowNegative); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskShowNegative, value); }
		}
		public bool ShowXAxis {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskShowXAxis); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskShowXAxis, value); }
		}
		public bool ShowHidden {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskShowHidden); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskShowHidden, value); }
		}
		public bool RightToLeft {
			get { return PackedValues.GetBoolBitValue(packedValues, MaskRightToLeft); }
			set { PackedValues.SetBoolBitValue(ref packedValues, MaskRightToLeft, value); }
		}
		public double MaxAxisValue { get { return maxAxisValue; } set { maxAxisValue = value; } }
		public double MinAxisValue { get { return minAxisValue; } set { minAxisValue = value; } }
		public double LineWeightInPoints {
			get { return lineWeight; }
			set {
				ValueChecker.CheckValue(value, 0, DrawingValueConstants.MaxWidthInPoints, "LineWeight");
				lineWeight = value;
			}
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		#region ICloneable<SparklineGroupInfo> Members
		public SparklineGroupInfo Clone() {
			SparklineGroupInfo result = new SparklineGroupInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<SparklineGroupInfo> Members
		public void CopyFrom(SparklineGroupInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.maxAxisValue = value.maxAxisValue;
			this.minAxisValue = value.minAxisValue;
			this.lineWeight = value.lineWeight;
		}
		#endregion
		public override bool Equals(object obj) {
			SparklineGroupInfo info = obj as SparklineGroupInfo;
			if (info == null)
				return false;
			return packedValues == info.packedValues &&
				   maxAxisValue == info.maxAxisValue &&
				   minAxisValue == info.minAxisValue &&
				   lineWeight == info.lineWeight;
		}
		public override int GetHashCode() {
			return packedValues.GetHashCode() ^ maxAxisValue.GetHashCode() ^
				   minAxisValue.GetHashCode() ^ lineWeight.GetHashCode();
		}
	}
	#endregion
	#region SparklineGroupInfoCache
	public class SparklineGroupInfoCache : UniqueItemsCache<SparklineGroupInfo> {
		public const int DefaultItemIndex = 0;
		public SparklineGroupInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override SparklineGroupInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new SparklineGroupInfo();
		}
	}
	#endregion
	#region ISparklineColors
	public interface ISparklineColors {
		Color Series { get; set; }
		Color Negative { get; set; }
		Color Axis { get; set; }
		Color Markers { get; set; }
		Color First { get; set; }
		Color Last { get; set; }
		Color Highest { get; set; }
		Color Lowest { get; set; }
	}
	#endregion
	#region SparklineGroupCollection
	public class SparklineGroupCollection : UndoableCollection<SparklineGroup> {
		public SparklineGroupCollection(Worksheet sheet) : base(sheet) { }
		#region Notifications
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			for (int i = Count - 1; i >= 0; i--) {
				SparklineGroup group = InnerList[i];
				if (group.OnRangeRemoving(context.Mode, context.Range))
					Remove(group);
			}
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			for (int i = Count - 1; i >= 0; i--)
				InnerList[i].OnRangeInserting(context.Mode, context.Range);
		}
		#endregion
	}
	#endregion
	#region SparklineGroup
	public class SparklineGroup : SpreadsheetUndoableIndexBasedObject<SparklineGroupInfo>, ICloneable<SparklineGroup>, ISupportsCopyFrom<SparklineGroup>, ISparklineColors {
		#region Fields
		readonly SparklineCollection sparklines;
		readonly int[] colorIndexes = new int[8];
		ParsedExpression expression;
		#endregion
		public SparklineGroup(Worksheet sheet)
			: base(sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sparklines = new SparklineCollection(sheet);
			this.expression = new ParsedExpression();
		}
		#region Properties
		public Worksheet Sheet { get { return (Worksheet)DocumentModelPart; } }
		public SparklineCollection Sparklines { get { return sparklines; } }
		public CellRange DateRange { get { return ParseRangeExpression(expression); } }
		public WorkbookDataContext Context { get { return DocumentModel.DataContext; } }
		public bool ColorsAreDefault {
			get {
				for (int i = 0; i < colorIndexes.Length; i++)
					if (!GetColor((SparklineColorType)i).Equals(Color.Empty))
						return false;
				return true;
			}
		}
		#region Expression
		protected internal ParsedExpression Expression {
			get { return expression.Clone(); }
			set {
				if (value == null)
					value = new ParsedExpression();
				if (expression.Equals(value))
					return;
				SparklineGroupExpressionHistoryItem historyItem = new SparklineGroupExpressionHistoryItem(this, expression, value);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		internal void SetExpressionCore(ParsedExpression expression) {
			this.expression = expression;
		}
		#endregion
		#region Type
		public SparklineGroupType Type {
			get { return Info.Type; }
			set {
				if (Type == value)
					return;
				SetPropertyValue(SetTypeCore, value);
			}
		}
		DocumentModelChangeActions SetTypeCore(SparklineGroupInfo info, SparklineGroupType value) {
			info.Type = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MinAxisScaleType
		public SparklineAxisScaling MinAxisScaleType {
			get { return Info.MinAxisScaleType; }
			set {
				if (MinAxisScaleType == value)
					return;
				SetPropertyValue(SetMinAxisScaleTypeCore, value);
			}
		}
		DocumentModelChangeActions SetMinAxisScaleTypeCore(SparklineGroupInfo info, SparklineAxisScaling value) {
			info.MinAxisScaleType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MaxAxisScaleType
		public SparklineAxisScaling MaxAxisScaleType {
			get { return Info.MaxAxisScaleType; }
			set {
				if (MaxAxisScaleType == value)
					return;
				SetPropertyValue(SetMaxAxisScaleTypeCore, value);
			}
		}
		DocumentModelChangeActions SetMaxAxisScaleTypeCore(SparklineGroupInfo info, SparklineAxisScaling value) {
			info.MaxAxisScaleType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DisplayBlanksAs
		public DisplayBlanksAs DisplayBlanksAs {
			get { return Info.DisplayBlanksAs; }
			set {
				if (DisplayBlanksAs == value)
					return;
				SetPropertyValue(SetDisplayBlanksAsCore, value);
			}
		}
		DocumentModelChangeActions SetDisplayBlanksAsCore(SparklineGroupInfo info, DisplayBlanksAs value) {
			info.DisplayBlanksAs = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region UseDateAxis
		public bool UseDateAxis {
			get { return Info.UseDateAxis; }
			set {
				if (UseDateAxis == value)
					return;
				SetPropertyValue(SetUseDateAxisCore, value);
			}
		}
		DocumentModelChangeActions SetUseDateAxisCore(SparklineGroupInfo info, bool value) {
			info.UseDateAxis = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowMarkers
		public bool ShowMarkers {
			get { return Info.ShowMarkers; }
			set {
				if (ShowMarkers == value)
					return;
				SetPropertyValue(SetShowMarkersCore, value);
			}
		}
		DocumentModelChangeActions SetShowMarkersCore(SparklineGroupInfo info, bool value) {
			info.ShowMarkers = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowHighest
		public bool ShowHighest {
			get { return Info.ShowHighest; }
			set {
				if (ShowHighest == value)
					return;
				SetPropertyValue(SetShowHighestCore, value);
			}
		}
		DocumentModelChangeActions SetShowHighestCore(SparklineGroupInfo info, bool value) {
			info.ShowHighest = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowLowest
		public bool ShowLowest {
			get { return Info.ShowLowest; }
			set {
				if (ShowLowest == value)
					return;
				SetPropertyValue(SetShowLowestCore, value);
			}
		}
		DocumentModelChangeActions SetShowLowestCore(SparklineGroupInfo info, bool value) {
			info.ShowLowest = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowFirst
		public bool ShowFirst {
			get { return Info.ShowFirst; }
			set {
				if (ShowFirst == value)
					return;
				SetPropertyValue(SetShowFirstCore, value);
			}
		}
		DocumentModelChangeActions SetShowFirstCore(SparklineGroupInfo info, bool value) {
			info.ShowFirst = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowLast
		public bool ShowLast {
			get { return Info.ShowLast; }
			set {
				if (ShowLast == value)
					return;
				SetPropertyValue(SetShowLastCore, value);
			}
		}
		DocumentModelChangeActions SetShowLastCore(SparklineGroupInfo info, bool value) {
			info.ShowLast = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowNegative
		public bool ShowNegative {
			get { return Info.ShowNegative; }
			set {
				if (ShowNegative == value)
					return;
				SetPropertyValue(SetShowNegativeCore, value);
			}
		}
		DocumentModelChangeActions SetShowNegativeCore(SparklineGroupInfo info, bool value) {
			info.ShowNegative = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowXAxis
		public bool ShowXAxis {
			get { return Info.ShowXAxis; }
			set {
				if (ShowXAxis == value)
					return;
				SetPropertyValue(SetShowXAxisCore, value);
			}
		}
		DocumentModelChangeActions SetShowXAxisCore(SparklineGroupInfo info, bool value) {
			info.ShowXAxis = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowHidden
		public bool ShowHidden {
			get { return Info.ShowHidden; }
			set {
				if (ShowHidden == value)
					return;
				SetPropertyValue(SetShowHiddenCore, value);
			}
		}
		DocumentModelChangeActions SetShowHiddenCore(SparklineGroupInfo info, bool value) {
			info.ShowHidden = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region RightToLeft
		public bool RightToLeft {
			get { return Info.RightToLeft; }
			set {
				if (RightToLeft == value)
					return;
				SetPropertyValue(SetRightToLeftCore, value);
			}
		}
		DocumentModelChangeActions SetRightToLeftCore(SparklineGroupInfo info, bool value) {
			info.RightToLeft = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MinAxisValue
		public double MinAxisValue {
			get { return Info.MinAxisValue; }
			set {
				if (MinAxisValue == value)
					return;
				SetPropertyValue(SetMinAxisValueCore, value);
			}
		}
		DocumentModelChangeActions SetMinAxisValueCore(SparklineGroupInfo info, double value) {
			info.MinAxisValue = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MaxAxisValue
		public double MaxAxisValue {
			get { return Info.MaxAxisValue; }
			set {
				if (MaxAxisValue == value)
					return;
				SetPropertyValue(SetMaxAxisValueCore, value);
			}
		}
		DocumentModelChangeActions SetMaxAxisValueCore(SparklineGroupInfo info, double value) {
			info.MaxAxisValue = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region LineWeightInPoints
		public double LineWeightInPoints {
			get { return Info.LineWeightInPoints; }
			set {
				if (LineWeightInPoints == value)
					return;
				SetPropertyValue(SetLineWeightInPointsCore, value);
			}
		}
		DocumentModelChangeActions SetLineWeightInPointsCore(SparklineGroupInfo info, double value) {
			info.LineWeightInPoints = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ISparklineColors Members
		public ISparklineColors ColorOf { get { return this; } }
		Color ISparklineColors.Series { get { return GetColor(SparklineColorType.Series); } set { SetColor(value, SparklineColorType.Series); } }
		Color ISparklineColors.Negative { get { return GetColor(SparklineColorType.Negative); } set { SetColor(value, SparklineColorType.Negative); } }
		Color ISparklineColors.Axis { get { return GetColor(SparklineColorType.Axis); } set { SetColor(value, SparklineColorType.Axis); } }
		Color ISparklineColors.Markers { get { return GetColor(SparklineColorType.Markers); } set { SetColor(value, SparklineColorType.Markers); } }
		Color ISparklineColors.First { get { return GetColor(SparklineColorType.First); } set { SetColor(value, SparklineColorType.First); } }
		Color ISparklineColors.Last { get { return GetColor(SparklineColorType.Last); } set { SetColor(value, SparklineColorType.Last); } }
		Color ISparklineColors.Highest { get { return GetColor(SparklineColorType.Highest); } set { SetColor(value, SparklineColorType.Highest); } }
		Color ISparklineColors.Lowest { get { return GetColor(SparklineColorType.Lowest); } set { SetColor(value, SparklineColorType.Lowest); } }
		#endregion
		#endregion
		#region Get/Set Color
		public int GetColorIndex(SparklineColorType colorType) {
			return colorIndexes[(int)colorType];
		}
		internal ColorModelInfo GetColorInfo(SparklineColorType colorType) {
			int index = GetColorIndex(colorType);
			return DocumentModel.Cache.ColorModelInfoCache[index];
		}
		internal Color GetColor(SparklineColorType colorType) {
			return GetColorInfo(colorType).ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
		}
		internal void SetColor(Color value, SparklineColorType colorType) {
			Color oldValue = GetColor(colorType);
			if (oldValue == value)
				return;
			SparklineGroupColorsHistoryItem historyItem = new SparklineGroupColorsHistoryItem(this, colorType, oldValue, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		internal void SetColorIndexCore(Color value, SparklineColorType colorType) {
			colorIndexes[(int)colorType] = ColorModelInfo.GetColorIndex(DocumentModel.Cache.ColorModelInfoCache, value);
		}
		internal void SetColorIndexCore(ColorModelInfo info, SparklineColorType colorType) {
			colorIndexes[(int)colorType] = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(info);
		}
		#endregion
		#region Get/Set Expression
		CellRange ParseRangeExpression(ParsedExpression expression) {
			if (expression == null || expression.Count != 1)
				return null;
			CellRangeBase resultRange = expression.Evaluate(Context).CellRangeValue;
			if (resultRange == null || resultRange.RangeType == CellRangeType.UnionRange)
				return null;
			return (CellRange)resultRange;
		}
		internal bool TrySetDataRange(string dataRangeFormula) {
			if (string.IsNullOrEmpty(dataRangeFormula))
				return false;
			ParsedExpression expression = Context.ParseExpression(dataRangeFormula, OperandDataType.None, true);
			if (ParseRangeExpression(expression) == null && !Sheet.Workbook.DefinedNames.Contains(dataRangeFormula))
				return false;
			this.Expression = expression;
			return true;
		}
		internal void TrySetDataRange(CellRange range) {
			if (range == null)
				Expression = null;
			else if (SparklineAddCommand.GetDataRangeError(range) != ModelErrorType.None)
				throw new ArgumentException("Invalid data range");
			else
				Expression = Context.ParseExpression(range.ToString(true), OperandDataType.None, true);
		}
		#endregion
		#region GetIsVisible
		internal bool GetIsVisible(SparklineColorType colorType) {
			switch (colorType) {
				case SparklineColorType.First:
					return ShowFirst;
				case SparklineColorType.Last:
					return ShowLast;
				case SparklineColorType.Axis:
					return ShowXAxis;
				case SparklineColorType.Highest:
					return ShowHighest;
				case SparklineColorType.Lowest:
					return ShowLowest;
				case SparklineColorType.Negative:
					return ShowNegative;
				case SparklineColorType.Markers:
					return ShowMarkers;
			}
			return true;
		}
		#endregion
		#region ICloneable<SparklineGroup> Members
		public SparklineGroup Clone() {
			return CloneTo(Sheet, true);
		}
		public SparklineGroup Clone(bool copySparklines) {
			return CloneTo(Sheet, copySparklines);
		}
		public SparklineGroup CloneTo(Worksheet sheet, bool copySparklines) {
			SparklineGroup result = new SparklineGroup(sheet);
			result.CopyFrom(this, copySparklines);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<SparklineGroup> Members
		public void CopyFrom(SparklineGroup other) {
			CopyFrom(other, true);
		}
		public void CopyFrom(SparklineGroup other, bool copySparklines) {
			Guard.ArgumentNotNull(other, "other");
			DocumentModel.BeginUpdate();
			try {
				base.CopyFrom(other);
				CopyDataRange(other.Sheet, other.DateRange);
				CopyColors(other.ColorOf);
				if (copySparklines)
					sparklines.CopyFrom(other.Sparklines);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void CopyDataRange(Worksheet otherWorksheet, CellRange otherDataRange) {
			if (otherDataRange == null)
				TrySetDataRange(otherDataRange);
			else {
				if (Object.ReferenceEquals(otherWorksheet, otherDataRange.Worksheet))
					TrySetDataRange(CellRange.Create(Sheet, otherDataRange.ToString()));
				else
					TrySetDataRange(otherDataRange);
			}
		}
		void CopyColors(ISparklineColors otherColorOf) {
			ColorOf.Series = otherColorOf.Series;
			ColorOf.Negative = otherColorOf.Negative;
			ColorOf.Axis = otherColorOf.Axis;
			ColorOf.Markers = otherColorOf.Markers;
			ColorOf.First = otherColorOf.First;
			ColorOf.Last = otherColorOf.Last;
			ColorOf.Highest = otherColorOf.Highest;
			ColorOf.Lowest = otherColorOf.Lowest;
		}
		#endregion
		#region SpreadsheetUndoableIndexBasedObject Members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<SparklineGroupInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.SparklineGroupInfoCache;
		}
		#endregion
		#region Notifications
		public bool OnRangeRemoving(RemoveCellMode mode, CellRange removingRange) {
			Sparklines.OnRangeRemoving(mode, removingRange);
			if (Sparklines.Count == 0)
				return true;
			SparklineNotificationsHelper helper = new SparklineNotificationsHelper(DateRange);
			helper.Calculate(mode, removingRange);
			if (helper.DataRange != null || helper.ShouldClearDataRange)
				TrySetDataRange(helper.DataRange);
			return false;
		}
		public void OnRangeInserting(InsertCellMode mode, CellRange insertingRange) {
			Sparklines.OnRangeInserting(mode, insertingRange);
			SparklineNotificationsHelper helper = new SparklineNotificationsHelper(DateRange);
			helper.Calculate(mode, insertingRange);
			if (helper.DataRange != null)
				TrySetDataRange(helper.DataRange);
		}
		#endregion
	}
	#endregion
	#region SparklineCollection
	public class SparklineCollection : UndoableCollection<Sparkline>, ISupportsCopyFrom<SparklineCollection> {
		public SparklineCollection(Worksheet sheet) : base(sheet) { }
		public Worksheet Sheet { get { return (Worksheet)DocumentModelPart; } }
		public void CopyFrom(SparklineCollection value) {
			DocumentModel.BeginUpdate();
			try {
				Clear();
				foreach (Sparkline item in value)
					Add(item.CloneTo(Sheet));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#region Notifications
		public void OnRangeRemoving(RemoveCellMode mode, CellRange removingRange) {
			for (int i = Count - 1; i >= 0; i--) {
				Sparkline sparkline = InnerList[i];
				if (removingRange.Includes(sparkline.GetPositionRange()))
					Remove(sparkline);
				else
					sparkline.OnRangeRemoving(mode, removingRange);
			}
		}
		public void OnRangeInserting(InsertCellMode mode, CellRange insertingRange) {
			int count = Count - 1;
			for (int i = count; i >= 0; i--) {
				Sparkline sparkline = InnerList[i];
				if (sparkline.OnRangeInserting(mode, insertingRange)) {
					bool shiftCellsDown = mode == InsertCellMode.ShiftCellsDown;
					int insertCount = shiftCellsDown ? insertingRange.Height : insertingRange.Width;
					MultiplySparklines(sparkline, shiftCellsDown, insertCount, i);
				}
			}
		}
		void MultiplySparklines(Sparkline original, bool shiftCellsDown, int count, int currentIndex) {
			for (int i = 1; i <= count; i++) {
				Sparkline sparkline = new Sparkline(Sheet);
				if (shiftCellsDown) {
					sparkline.TrySetDataRange(original.SourceDataRange.GetResized(0, i, 0, i));
					sparkline.Position = new CellPosition(original.Position.Column, original.Position.Row + i);
				}
				else {
					sparkline.TrySetDataRange(original.SourceDataRange.GetResized(i, 0, i, 0));
					sparkline.Position = new CellPosition(original.Position.Column + i, original.Position.Row);
				}
				Insert(currentIndex + i, sparkline);
			}
		}
		#endregion
	}
	#endregion
	#region Sparkline
	public class Sparkline : ISupportsCopyFrom<Sparkline> {
		#region Fields
		readonly Worksheet sheet;
		ParsedExpression expression;
		CellPosition position;
		#endregion
		public Sparkline(Worksheet sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.expression = new ParsedExpression();
		}
		#region Properties
		public Worksheet Sheet { get { return sheet; } }
		public CellRange SourceDataRange { get { return ParseRangeExpression(expression); } }
		public WorkbookDataContext Context { get { return sheet.Workbook.DataContext; } }
		#region Expression
		protected internal ParsedExpression Expression {
			get { return expression.Clone(); }
			set {
				if (value == null)
					value = new ParsedExpression();
				if (expression.Equals(value))
					return;
				SparklineExpressionHistoryItem historyItem = new SparklineExpressionHistoryItem(this, expression, value);
				sheet.Workbook.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		internal void SetExpressionCore(ParsedExpression expression) {
			this.expression = expression;
		}
		#endregion
		#region Position
		public CellPosition Position {
			get { return position; }
			set {
				if (!value.IsValid || position.EqualsPosition(value))
					return;
				SparklinePositionHistoryItem historyItem = new SparklinePositionHistoryItem(this, position, value);
				sheet.Workbook.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		internal void SetPositionCore(CellPosition value) {
			position = value;
		}
		#endregion
		#endregion
		#region Get/Set Expression
		CellRange ParseRangeExpression(ParsedExpression expression) {
			if (expression == null || expression.Count != 1)
				return null;
			CellRangeBase resultRange = expression.Evaluate(Context).CellRangeValue;
			if (resultRange == null || resultRange.RangeType == CellRangeType.UnionRange)
				return null;
			return (CellRange)resultRange;
		}
		internal bool TrySetDataRange(string dataRangeFormula) {
			if (string.IsNullOrEmpty(dataRangeFormula))
				return false;
			ParsedExpression expression = Context.ParseExpression(dataRangeFormula, OperandDataType.None, true);
			if (ParseRangeExpression(expression) == null && !Sheet.Workbook.DefinedNames.Contains(dataRangeFormula))
				return false;
			this.Expression = expression;
			return true;
		}
		internal void TrySetDataRange(CellRange range) {
			if (range == null)
				Expression = null;
			else if (SparklineAddCommand.GetDataRangeError(range) != ModelErrorType.None)
				throw new ArgumentException("Invalid data range");
			else
				Expression = Context.ParseExpression(range.ToString(true), OperandDataType.None, true);
		}
		#endregion
		#region CloneTo & CopyFrom
		public Sparkline CloneTo(Worksheet sheet) {
			Sparkline result = new Sparkline(sheet);
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(Sparkline other) {
			sheet.Workbook.BeginUpdate();
			try {
				CopyDataRange(other.Sheet, other.SourceDataRange);
				Position = other.Position;
			}
			finally {
				sheet.Workbook.EndUpdate();
			}
		}
		void CopyDataRange(Worksheet otherWorksheet, CellRange otherDataRange) {
			if (otherDataRange != null && Object.ReferenceEquals(otherWorksheet, otherDataRange.Worksheet))
				TrySetDataRange(CellRange.Create(Sheet, otherDataRange.ToString()));
			else
				TrySetDataRange(otherDataRange);
		}
		#endregion
		public CellRange GetPositionRange() {
			return new CellRange(Sheet, Position, Position);
		}
		#region Notifications
		public void OnRangeRemoving(RemoveCellMode mode, CellRange removingRange) {
			SparklineNotificationsHelper helper = new SparklineNotificationsHelper(SourceDataRange, GetPositionRange());
			helper.Calculate(mode, removingRange);
			Position = helper.Position.TopLeft;
			if (helper.DataRange != null || helper.ShouldClearDataRange)
				TrySetDataRange(helper.DataRange);
		}
		public bool OnRangeInserting(InsertCellMode mode, CellRange insertingRange) {
			SparklineNotificationsHelper helper = new SparklineNotificationsHelper(SourceDataRange, GetPositionRange());
			helper.Calculate(mode, insertingRange);
			Position = helper.Position.TopLeft;
			if (helper.DataRange != null)
				TrySetDataRange(helper.DataRange);
			return helper.ShouldMultiplySparklines;
		}
		#endregion
	}
	#endregion
	#region SparklineNotificationsHelper
	public class SparklineNotificationsHelper {
		CellRange dataRange;
		CellRange position = null;
		public SparklineNotificationsHelper(CellRange dataRange) {
			this.dataRange = dataRange;
		}
		public SparklineNotificationsHelper(CellRange dataRange, CellRange position)
			: this(dataRange) {
			this.position = position;
		}
		#region Properties
		public CellRange DataRange { get { return dataRange; } }
		public CellRange Position { get { return position; } }
		public bool ShouldClearDataRange { get; private set; }
		public bool ShouldMultiplySparklines { get; private set; }
		#endregion
		#region Calcualte
		public void Calculate(RemoveCellMode mode, CellRange removingRange) {
			if (mode == RemoveCellMode.ShiftCellsLeft) {
				if (position != null && NotificationsHelper.LeftRightShiftAffectsRange(removingRange, position))
					position = position.GetResized(-removingRange.Width, 0, -removingRange.Width, 0);
				dataRange = CalculateRangeShiftCellsLeft(removingRange);
			}
			else {
				if (position != null && NotificationsHelper.UpDownShiftAffectsRange(removingRange, position))
					position = position.GetResized(0, -removingRange.Height, 0, -removingRange.Height);
				dataRange = CalculateRangeShiftCellsUp(removingRange);
			}
		}
		public void Calculate(InsertCellMode mode, CellRange insertingRange) {
			if (mode == InsertCellMode.ShiftCellsDown) {
				dataRange = CalculateRangeShiftCellsDown(insertingRange);
				CalculatePositionShiftCellsDown(insertingRange);
			}
			else {
				dataRange = CalculateRangeShiftCellsRight(insertingRange);
				CalculatePositionShiftCellsRight(insertingRange);
			}
		}
		#endregion
		#region Internal
		#region OnRangeRemoving
		#region CalculateRangeShiftCellsLeft
		CellRange CalculateRangeShiftCellsLeft(CellRange removingRange) {
			if (dataRange == null || !NotificationsHelper.LeftRightShiftAffectsRange(removingRange, dataRange))
				return null;
			CellRange intersection = removingRange.Intersection(dataRange);
			if (intersection != null) {
				if (intersection.Equals(dataRange)) {
					ShouldClearDataRange = true;
					return null;
				}
				if (dataRange.Height > dataRange.Width)
					return CutIntersection(intersection, dataRange);
				else {
					int offset = dataRange.TopLeft.Column - removingRange.TopLeft.Column;
					return dataRange.GetResized(offset <= 0 ? 0 : -offset, 0, -removingRange.Width, 0);
				}
			}
			else if (NotificationsHelper.RangeHeightIsCovered(removingRange, dataRange))
				return dataRange.GetResized(-removingRange.Width, 0, -removingRange.Width, 0);
			return null;
		}
		#endregion
		#region CalculateRangeShiftCellsUp
		CellRange CalculateRangeShiftCellsUp(CellRange removingRange) {
			if (dataRange == null || !NotificationsHelper.UpDownShiftAffectsRange(removingRange, dataRange))
				return null;
			CellRange intersection = removingRange.Intersection(dataRange);
			if (intersection != null) {
				if (intersection.Equals(dataRange)) {
					ShouldClearDataRange = true;
					return null;
				}
				if (dataRange.Width > dataRange.Height)
					return CutIntersection(intersection, dataRange);
				else {
					int offset = dataRange.TopLeft.Row - removingRange.TopLeft.Row;
					return dataRange.GetResized(0, offset <= 0 ? 0 : -offset, 0, -removingRange.Height);
				}
			}
			else if (NotificationsHelper.RangeWidthIsCovered(removingRange, dataRange))
				return dataRange.GetResized(0, -removingRange.Height, 0, -removingRange.Height);
			return null;
		}
		#endregion
		CellRange CutIntersection(CellRange intersection, CellRange dataRange) {
			CellRangeBase complementedRange = dataRange.ExcludeRange(intersection);
			if (complementedRange.RangeType != CellRangeType.UnionRange)
				return (CellRange)complementedRange;
			return null;
		}
		#endregion
		#region OnRangeInserting
		void CalculatePositionShiftCellsDown(CellRange insertingRange) {
			if (position == null)
				return;
			if (insertingRange.TopLeft.Row == position.TopLeft.Row + 1 && NotificationsHelper.RangeWidthIsCovered(insertingRange, position))
				ShouldMultiplySparklines = true;
			if (NotificationsHelper.UpDownShiftAffectsRange(insertingRange, position))
				position = position.GetResized(0, insertingRange.Height, 0, insertingRange.Height);
		}
		CellRange CalculateRangeShiftCellsDown(CellRange insertingRange) {
			if (dataRange == null || 
				!NotificationsHelper.UpDownShiftAffectsRange(insertingRange, dataRange) || 
				!NotificationsHelper.RangeWidthIsCovered(insertingRange, dataRange))
				return null;
			int offset = insertingRange.TopLeft.Row > dataRange.TopLeft.Row ? 0 : insertingRange.Height;
			return dataRange.GetResized(0, offset, 0, insertingRange.Height);
		}
		void CalculatePositionShiftCellsRight(CellRange insertingRange) {
			if (position == null)
				return;
			if (insertingRange.TopLeft.Column == position.TopLeft.Column + 1 && NotificationsHelper.RangeHeightIsCovered(insertingRange, position))
				ShouldMultiplySparklines = true;
			if (NotificationsHelper.LeftRightShiftAffectsRange(insertingRange, position))
				position = position.GetResized(insertingRange.Width, 0, insertingRange.Width, 0);
		}
		CellRange CalculateRangeShiftCellsRight(CellRange insertingRange) {
			if (dataRange == null || 
				!NotificationsHelper.LeftRightShiftAffectsRange(insertingRange, dataRange) || 
				!NotificationsHelper.RangeHeightIsCovered(insertingRange, dataRange))
				return null;
			int offset = insertingRange.TopLeft.Column > dataRange.TopLeft.Column ? 0 : insertingRange.Width;
			return dataRange.GetResized(offset, 0, insertingRange.Width, 0);
		}
		#endregion
		#endregion
	}
	#endregion
}
