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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DataLabelPosition
	public enum DataLabelPosition {
		Left = 0,
		Top = 1,
		Right = 2,
		Bottom = 3,
		Center = 4,
		BestFit = 5,
		InsideBase = 6,
		InsideEnd = 7,
		OutsideEnd = 8,
		Default = 0xf
	}
	#endregion
	#region DataLabelInfo
	public class DataLabelInfo : ICloneable<DataLabelInfo>, ISupportsCopyFrom<DataLabelInfo>, ISupportsSizeOf {
		#region Fields
		const int offsetLabelPosition = 8;
		const uint maskDelete = 0x0001;
		const uint maskShowBubbleSize = 0x0002;
		const uint maskShowCategoryName = 0x0004;
		const uint maskShowSeriesName = 0x0008;
		const uint maskShowLegendKey = 0x0010;
		const uint maskShowPercent = 0x0020;
		const uint maskShowValue = 0x0040;
		const uint maskShowLeaderLines = 0x0080;
		const uint maskLabelPosition = 0x0f00;
		const uint maskApply = 0x1000;
		uint packedValues = 0x0f80;
		#endregion
		#region Properties
		public bool Apply {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskApply); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskApply, value); }
		}
		public bool Delete {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDelete); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDelete, value); }
		}
		public bool ShowBubbleSize {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowBubbleSize); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowBubbleSize, value); }
		}
		public bool ShowCategoryName {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowCategoryName); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowCategoryName, value); }
		}
		public bool ShowSeriesName {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowSeriesName); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowSeriesName, value); }
		}
		public bool ShowLegendKey {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowLegendKey); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowLegendKey, value); }
		}
		public bool ShowPercent {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowPercent); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowPercent, value); }
		}
		public bool ShowValue {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowValue); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowValue, value); }
		}
		public DataLabelPosition LabelPosition {
			get { return (DataLabelPosition)PackedValues.GetIntBitValue(this.packedValues, maskLabelPosition, offsetLabelPosition); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskLabelPosition, offsetLabelPosition, (int)value); }
		}
		public bool ShowLeaderLines {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowLeaderLines); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowLeaderLines, value); }
		}
		#endregion
		#region ICloneable<DataLabelInfo> Members
		public DataLabelInfo Clone() {
			DataLabelInfo result = new DataLabelInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DataLabelInfo> Members
		public void CopyFrom(DataLabelInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			DataLabelInfo other = obj as DataLabelInfo;
			if (other == null)
				return false;
			return this.packedValues == other.packedValues;
		}
		public override int GetHashCode() {
			return (int)this.packedValues;
		}
	}
	#endregion
	#region DataLabelInfoCache
	public class DataLabelInfoCache : UniqueItemsCache<DataLabelInfo> {
		public DataLabelInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DataLabelInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DataLabelInfo();
		}
	}
	#endregion
	#region DataLabelBase
	public abstract class DataLabelBase : SpreadsheetUndoableIndexBasedObject<DataLabelInfo>, ISupportsCopyFrom<DataLabelBase> {
		#region Fields
		public const string DefaultSeparator = ", ";
		readonly IChart parent;
		string separator = DefaultSeparator;
		ShapeProperties shapeProperties;
		TextProperties textProperties;
		NumberFormatOptions numberFormat;
		#endregion
		protected DataLabelBase(IChart parent)
			: base(parent.DocumentModel) {
			this.parent = parent;
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.textProperties = new TextProperties(DocumentModel) { Parent = parent };
			this.numberFormat = new NumberFormatOptions(parent);
		}
		#region Properties
		public IChart Parent { get { return parent; } }
		#region Delete
		public bool Delete {
			get { return Info.Delete; }
			set {
				if (Delete == value)
					return;
				SetPropertyValue(SetDeleteCore, value);
			}
		}
		DocumentModelChangeActions SetDeleteCore(DataLabelInfo info, bool value) {
			info.Delete = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region LabelPosition
		public DataLabelPosition LabelPosition {
			get { return Info.LabelPosition; }
			set {
				if (LabelPosition == value)
					return;
				SetPropertyValue(SetLabelPositionCore, value);
			}
		}
		DocumentModelChangeActions SetLabelPositionCore(DataLabelInfo info, DataLabelPosition value) {
			info.LabelPosition = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowBubbleSize
		public bool ShowBubbleSize {
			get { return Info.ShowBubbleSize; }
			set {
				if (ShowBubbleSize == value)
					return;
				SetPropertyValue(SetShowBubbleSizeCore, value);
			}
		}
		DocumentModelChangeActions SetShowBubbleSizeCore(DataLabelInfo info, bool value) {
			info.ShowBubbleSize = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowCategoryName
		public bool ShowCategoryName {
			get { return Info.ShowCategoryName; }
			set {
				if (ShowCategoryName == value)
					return;
				SetPropertyValue(SetShowCategoryNameCore, value);
			}
		}
		DocumentModelChangeActions SetShowCategoryNameCore(DataLabelInfo info, bool value) {
			info.ShowCategoryName = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowSeriesName
		public bool ShowSeriesName {
			get { return Info.ShowSeriesName; }
			set {
				if (ShowSeriesName == value)
					return;
				SetPropertyValue(SetShowSeriesNameCore, value);
			}
		}
		DocumentModelChangeActions SetShowSeriesNameCore(DataLabelInfo info, bool value) {
			info.ShowSeriesName = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowLegendKey
		public bool ShowLegendKey {
			get { return Info.ShowLegendKey; }
			set {
				if (ShowLegendKey == value)
					return;
				SetPropertyValue(SetShowLegendKeyCore, value);
			}
		}
		DocumentModelChangeActions SetShowLegendKeyCore(DataLabelInfo info, bool value) {
			info.ShowLegendKey = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowPercent
		public bool ShowPercent {
			get { return Info.ShowPercent; }
			set {
				if (ShowPercent == value)
					return;
				SetPropertyValue(SetShowPercentCore, value);
			}
		}
		DocumentModelChangeActions SetShowPercentCore(DataLabelInfo info, bool value) {
			info.ShowPercent = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowValue
		public bool ShowValue {
			get { return Info.ShowValue; }
			set {
				if (ShowValue == value)
					return;
				SetPropertyValue(SetShowValueCore, value);
			}
		}
		DocumentModelChangeActions SetShowValueCore(DataLabelInfo info, bool value) {
			info.ShowValue = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Separator
		public string Separator {
			get { return separator; }
			set {
				if (string.IsNullOrEmpty(value))
					value = string.Empty;
				if (separator == value)
					return;
				SetSeparator(value);
			}
		}
		void SetSeparator(string value) {
			DataLabelSeparatorPropertyChangedHistoryItem historyItem = new DataLabelSeparatorPropertyChangedHistoryItem(DocumentModel, this, separator, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetSeparatorCore(string value) {
			this.separator = value;
			Parent.Invalidate();
		}
		#endregion
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public TextProperties TextProperties { get { return textProperties; } }
		public NumberFormatOptions NumberFormat { get { return numberFormat; } }
		public virtual bool IsVisible { get { return !Delete && (ShowCategoryName || ShowSeriesName || ShowValue || ShowPercent || ShowBubbleSize); } }
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<DataLabelInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.DataLabelInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			Parent.Invalidate();
		}
		#endregion
		#region ISupportsCopyFrom<DataLabelBase> Members
		public void CopyFrom(DataLabelBase value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			numberFormat.CopyFrom(value.numberFormat);
			Separator = value.Separator;
			this.shapeProperties.CopyFrom(value.shapeProperties);
			this.textProperties.CopyFrom(value.textProperties);
		}
		#endregion
		public virtual void ResetToStyle() {
			DocumentModel.BeginUpdate();
			try {
				ShapeProperties.ResetToStyle();
				TextProperties.ResetToStyle();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region DataLabel
	public class DataLabel : DataLabelBase, ISupportsCopyFrom<DataLabel>, IChartTextOwnerEx {
		#region Fields
		int itemIndex;
		LayoutOptions layout;
		IChartText text;
		#endregion
		public DataLabel(IChart parent, int itemIndex)
			: base(parent) {
			this.itemIndex = itemIndex;
			this.layout = new LayoutOptions(parent);
			this.text = ChartText.Empty;
		}
		#region Properties
		public int ItemIndex { get { return itemIndex; } set { itemIndex = value; } }
		public LayoutOptions Layout { get { return layout; } }
		#region Text
		public IChartText Text {
			get { return text; }
			set {
				if (value == null)
					value = ChartText.Empty;
				if (text.Equals(value))
					return;
				SetText(value);
			}
		}
		void SetText(IChartText value) {
			ChartTextPropertyChangedHistoryItem historyItem = new ChartTextPropertyChangedHistoryItem(DocumentModel, this, text, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetTextCore(IChartText value) {
			text = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			text.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			text.OnRangeRemoving(context);
		}
		#region ISupportsCopyFrom<DataLabel> Members
		public void CopyFrom(DataLabel value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.layout.CopyFrom(value.layout);
			Text = value.Text.CloneTo(Parent);
		}
		#endregion
	}
	#endregion
	#region DataLabelCollection
	public class DataLabelCollection : ChartUndoableCollectionSupportsCopyFrom<DataLabel> {
		public DataLabelCollection(IChart parent)
			: base(parent) {
		}
		public DataLabel FindByItemIndex(int itemIndex) {
			foreach (DataLabel item in this)
				if (item.ItemIndex == itemIndex)
					return item;
			return null;
		}
		protected override DataLabel CreateNewItem(DataLabel source) {
			DataLabel result = new DataLabel(Parent, source.ItemIndex);
			result.CopyFrom(source);
			return result;
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			foreach (DataLabel dataLabel in this)
				dataLabel.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			foreach (DataLabel dataLabel in this)
				dataLabel.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region DataLabels
	public class DataLabels : DataLabelBase, ISupportsCopyFrom<DataLabels> {
		#region Fields
		DataLabelCollection labels;
		ShapeProperties leaderLinesProperties;
		#endregion
		public DataLabels(IChart parent)
			: base(parent) {
			this.labels = new DataLabelCollection(parent);
			this.leaderLinesProperties = new ShapeProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		#region Apply
		public bool Apply {
			get { return Info.Apply; }
			set {
				if (Apply == value)
					return;
				SetPropertyValue(SetApplyCore, value);
			}
		}
		DocumentModelChangeActions SetApplyCore(DataLabelInfo info, bool value) {
			info.Apply = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowLeaderLines
		public bool ShowLeaderLines {
			get { return Info.ShowLeaderLines; }
			set {
				if (ShowLeaderLines == value)
					return;
				SetPropertyValue(SetShowLeaderLinesCore, value);
			}
		}
		DocumentModelChangeActions SetShowLeaderLinesCore(DataLabelInfo info, bool value) {
			info.ShowLeaderLines = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public DataLabelCollection Labels { get { return labels; } }
		public ShapeProperties LeaderLinesProperties { get { return leaderLinesProperties; } }
		public override bool IsVisible {
			get {
				foreach (DataLabel label in Labels)
					if (label.IsVisible)
						return true;
				return base.IsVisible;
			}
		}
		#endregion
		#region ISupportsCopyFrom<DataLabels> Members
		public void CopyFrom(DataLabels value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			leaderLinesProperties.CopyFrom(value.leaderLinesProperties);
			labels.CopyFrom(value.labels);
		}
		#endregion
		public override void ResetToStyle() {
			base.ResetToStyle();
			foreach (DataLabel label in Labels)
				label.ResetToStyle();
			LeaderLinesProperties.ResetToStyle();
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			labels.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			labels.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region DataLabelsHelper
	public static class DataLabelsHelper {
		public static bool IsDataLabelsVisible(DataLabels viewDataLabels, DataLabels seriesDataLabels) {
			if (seriesDataLabels.Apply)
				return IsDataLabelsVisible(seriesDataLabels);
			return IsDataLabelsVisible(viewDataLabels);
		}
		public static bool IsDataLabelsVisible(DataLabels dataLabels) {
			foreach (DataLabel label in dataLabels.Labels)
				if (IsDataLabelsVisibleCore(label))
					return true;
			return IsDataLabelsVisibleCore(dataLabels);
		}
		static bool IsDataLabelsVisibleCore(DataLabelBase dataLabel) {
			return !dataLabel.Delete && (dataLabel.ShowCategoryName || dataLabel.ShowSeriesName || dataLabel.ShowValue || dataLabel.ShowPercent || dataLabel.ShowBubbleSize);
		}
		public static DataLabelPosition GetDataLabelsPosition(DataLabels viewDataLabels, DataLabels seriesDataLabels, DataLabelPosition defaultPosition) {
			if (seriesDataLabels.Apply)
				return GetDataLabelsPosition(seriesDataLabels, defaultPosition);
			return GetDataLabelsPosition(viewDataLabels, defaultPosition);
		}
		public static DataLabelPosition GetDataLabelsPosition(DataLabels dataLabels, DataLabelPosition defaultPosition) {
			DataLabelPosition position = dataLabels.LabelPosition;
			if (position == DataLabelPosition.Default)
				position = defaultPosition;
			return position;
		}
	}
	#endregion
}
