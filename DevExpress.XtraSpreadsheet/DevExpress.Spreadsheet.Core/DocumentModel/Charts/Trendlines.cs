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
	#region ChartTrendlineType
	public enum ChartTrendlineType {
		Linear = 0,
		Logarithmic = 1,
		Exponential = 2,
		MovingAverage = 3,
		Polynomial = 4,
		Power = 5
	}
	#endregion
	#region TrendlineInfo
	public class TrendlineInfo : ICloneable<TrendlineInfo>, ISupportsCopyFrom<TrendlineInfo>, ISupportsSizeOf {
		#region Fields
		const int offsetOrder = 8;
		const int offsetPeriod = 12;
		const uint maskTrendlineType = 0x0007;
		const uint maskDispEquation = 0x0008;
		const uint maskDispRSquare = 0x0010;
		const uint maskHasCustomName = 0x0020;
		const uint maskHasIntercept = 0x0040;
		const uint maskOrder = 0x0700;
		const uint maskPeriod = 0x000ff000;
		uint packedValues = 0x00002200;
		double backward = 0.0;
		double forward = 0.0;
		double intercept = 0.0;
		#endregion
		#region Properties
		public ChartTrendlineType TrendlineType {
			get { return (ChartTrendlineType)PackedValues.GetIntBitValue(this.packedValues, maskTrendlineType); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskTrendlineType, (int)value);  }
		}
		public bool DisplayEquation {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDispEquation); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDispEquation, value); }
		}
		public bool DisplayRSquare {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDispRSquare); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDispRSquare, value); }
		}
		public bool HasCustomName {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskHasCustomName); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskHasCustomName, value); }
		}
		public bool HasIntercept {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskHasIntercept); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskHasIntercept, value); }
		}
		public int Order {
			get { return PackedValues.GetIntBitValue(this.packedValues, maskOrder, offsetOrder); }
			set {
				ValueChecker.CheckValue(value, 2, 6, "Order");
				PackedValues.SetIntBitValue(ref this.packedValues, maskOrder, offsetOrder, value);
			}
		}
		public int Period {
			get { return PackedValues.GetIntBitValue(this.packedValues, maskPeriod, offsetPeriod); }
			set {
				ValueChecker.CheckValue(value, 2, 255, "Period");
				PackedValues.SetIntBitValue(ref this.packedValues, maskPeriod, offsetPeriod, value);
			}
		}
		public double Backward {
			get { return backward; }
			set { backward = value; }
		}
		public double Forward {
			get { return forward; }
			set { forward = value; }
		}
		public double Intercept {
			get { return intercept; }
			set { intercept = value; }
		}
		#endregion
		#region ICloneable<TrendlineInfo> Members
		public TrendlineInfo Clone() {
			TrendlineInfo result = new TrendlineInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<TrendlineInfo> Members
		public void CopyFrom(TrendlineInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.backward = value.backward;
			this.forward = value.forward;
			this.intercept = value.intercept;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			TrendlineInfo other = obj as TrendlineInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues &&
				this.backward == other.backward &&
				this.forward == other.forward &&
				this.intercept == other.intercept;
		}
		public override int GetHashCode() {
			return (int)this.packedValues ^ this.backward.GetHashCode() ^ this.forward.GetHashCode() ^ this.intercept.GetHashCode();
		}
	}
	#endregion
	#region TrendlineInfoCache
	public class TrendlineInfoCache : UniqueItemsCache<TrendlineInfo> {
		public TrendlineInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override TrendlineInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new TrendlineInfo();
		}
	}
	#endregion
	#region Trendline
	public class Trendline : SpreadsheetUndoableIndexBasedObject<TrendlineInfo>, ISupportsCopyFrom<Trendline> {
		#region Fields
		readonly IChart parent;
		string name;
		TrendlineLabel label;
		ShapeProperties shapeProperties;
		#endregion
		public Trendline(IChart parent)
			: base(parent.DocumentModel) {
				this.parent = parent;
			this.name = string.Empty;
			this.label = new TrendlineLabel(parent);
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		#region Name
		public string Name {
			get { return name; }
			set {
				if(string.IsNullOrEmpty(value))
					value = string.Empty;
				if(name == value)
					return;
				SetName(value);
			}
		}
		void SetName(string value) {
			TrendlineNamePropertyChangedHistoryItem historyItem = new TrendlineNamePropertyChangedHistoryItem(DocumentModel, this, name, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetNameCore(string value) {
			this.name = value;
			Parent.Invalidate();
		}
		#endregion
		#region TrendlineType
		public ChartTrendlineType TrendlineType {
			get { return Info.TrendlineType; }
			set {
				if(TrendlineType == value)
					return;
				SetPropertyValue(SetTrendlineTypeCore, value);
			}
		}
		DocumentModelChangeActions SetTrendlineTypeCore(TrendlineInfo info, ChartTrendlineType value) {
			info.TrendlineType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DisplayEquation
		public bool DisplayEquation {
			get { return Info.DisplayEquation; }
			set {
				if(DisplayEquation == value)
					return;
				SetPropertyValue(SetDisplayEquationCore, value);
			}
		}
		DocumentModelChangeActions SetDisplayEquationCore(TrendlineInfo info, bool value) {
			info.DisplayEquation = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DisplayRSquare
		public bool DisplayRSquare {
			get { return Info.DisplayRSquare; }
			set {
				if(DisplayRSquare == value)
					return;
				SetPropertyValue(SetDisplayRSquareCore, value);
			}
		}
		DocumentModelChangeActions SetDisplayRSquareCore(TrendlineInfo info, bool value) {
			info.DisplayRSquare = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HasCustomName
		public bool HasCustomName {
			get { return Info.HasCustomName; }
			set {
				if(HasCustomName == value)
					return;
				SetPropertyValue(SetHasCustomNameCore, value);
			}
		}
		DocumentModelChangeActions SetHasCustomNameCore(TrendlineInfo info, bool value) {
			info.HasCustomName = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HasIntercept
		public bool HasIntercept {
			get { return Info.HasIntercept; }
			set {
				if(HasIntercept == value)
					return;
				SetPropertyValue(SetHasInterceptCore, value);
			}
		}
		DocumentModelChangeActions SetHasInterceptCore(TrendlineInfo info, bool value) {
			info.HasIntercept = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Order
		public int Order {
			get { return Info.Order; }
			set {
				if(Order == value)
					return;
				SetPropertyValue(SetOrderCore, value);
			}
		}
		DocumentModelChangeActions SetOrderCore(TrendlineInfo info, int value) {
			info.Order = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Period
		public int Period {
			get { return Info.Period; }
			set {
				if(Period == value)
					return;
				SetPropertyValue(SetPeriodCore, value);
			}
		}
		DocumentModelChangeActions SetPeriodCore(TrendlineInfo info, int value) {
			info.Period = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Backward
		public double Backward {
			get { return Info.Backward; }
			set {
				if(Backward == value)
					return;
				SetPropertyValue(SetBackwardCore, value);
			}
		}
		DocumentModelChangeActions SetBackwardCore(TrendlineInfo info, double value) {
			info.Backward = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Forward
		public double Forward {
			get { return Info.Forward; }
			set {
				if(Forward == value)
					return;
				SetPropertyValue(SetForwardCore, value);
			}
		}
		DocumentModelChangeActions SetForwardCore(TrendlineInfo info, double value) {
			info.Forward = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Intercept
		public double Intercept {
			get { return Info.Intercept; }
			set {
				if(Intercept == value)
					return;
				SetPropertyValue(SetInterceptCore, value);
			}
		}
		DocumentModelChangeActions SetInterceptCore(TrendlineInfo info, double value) {
			info.Intercept = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public bool HasLabel { get { return DisplayEquation || DisplayRSquare; } }
		public TrendlineLabel Label { get { return label; } }
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<TrendlineInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.TrendlineInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			Parent.Invalidate();
		}
		#endregion
		#region ISupportsCopyFrom<Trendline> Members
		public void CopyFrom(Trendline value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			Name = value.Name;
			this.label.CopyFrom(value.label);
			this.shapeProperties.CopyFrom(value.shapeProperties);
		}
		#endregion
		public void ResetToStyle() {
			ShapeProperties.ResetToStyle();
			Label.ResetToStyle();
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			Label.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			Label.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region TrendlineCollection
	public class TrendlineCollection : ChartUndoableCollectionSupportsCopyFrom<Trendline> {
		public TrendlineCollection(IChart parent)
			: base(parent) {
		}
		protected override Trendline CreateNewItem(Trendline source) {
			Trendline result = new Trendline(Parent);
			result.CopyFrom(source);
			return result;
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			foreach(Trendline trendline in this)
				trendline.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			foreach (Trendline trendline in this)
				trendline.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region TrendlineLabel
	public class TrendlineLabel : ISupportsCopyFrom<TrendlineLabel>, IChartTextOwnerEx {
		#region Fields
		readonly IChart parent;
		LayoutOptions layout;
		IChartText text;
		ShapeProperties shapeProperties;
		TextProperties textProperties;
		NumberFormatOptions numberFormat;
		#endregion
		public TrendlineLabel(IChart parent) {
			this.parent = parent;
			this.layout = new LayoutOptions(parent);
			this.text = ChartText.Empty;
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.textProperties = new TextProperties(DocumentModel) { Parent = parent };
			this.numberFormat = new NumberFormatOptions(parent);
		}
		#region Properties
		public IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		#region Text
		public IChartText Text {
			get { return text; }
			set {
				if(value == null)
					value = ChartText.Empty;
				if(text.Equals(value))
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
		public LayoutOptions Layout { get { return layout; } }
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public TextProperties TextProperties { get { return textProperties; } }
		public NumberFormatOptions NumberFormat { get { return numberFormat; } }
		#endregion
		#region ISupportsCopyFrom<TrendlineLabel> Members
		public void CopyFrom(TrendlineLabel value) {
			Guard.ArgumentNotNull(value, "value");
			this.layout.CopyFrom(value.layout);
			this.NumberFormat.CopyFrom(value.NumberFormat);
			Text = value.Text.CloneTo(Parent);
			this.shapeProperties.CopyFrom(value.shapeProperties);
			this.textProperties.CopyFrom(value.textProperties);
		}
		#endregion
		public void ResetToStyle() {
			ShapeProperties.ResetToStyle();
			TextProperties.ResetToStyle();
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			Text.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			Text.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
}
