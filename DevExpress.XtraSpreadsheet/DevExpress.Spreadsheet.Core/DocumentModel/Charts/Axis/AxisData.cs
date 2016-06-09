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

using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Runtime.InteropServices;
using ChartsModel = DevExpress.Charts.Model;
namespace DevExpress.XtraSpreadsheet.Model {
	#region AxisType
	public enum AxisDataType {
		Agrument = 0,
		Value = 1,
		Series = 2
	}
	#endregion
	#region AxisCrosses
	public enum AxisCrosses {
		AutoZero = 0,
		Min = 1,
		Max = 2,
		AtValue = 3
	}
	#endregion
	#region AxisCrossBetween
	public enum AxisCrossBetween {
		Between = 0,
		Midpoint
	}
	#endregion
	#region AxisOrientation
	public enum AxisOrientation {
		MinMax,
		MaxMin
	}
	#endregion
	#region AxisPosition
	public enum AxisPosition {
		Left,
		Top,
		Right,
		Bottom
	}
	#endregion
	#region LabelAlignment
	public enum LabelAlignment {
		Center = 0,
		Left,
		Right
	}
	#endregion
	#region TickMark
	public enum TickMark {
		Cross = 0,
		Inside,
		None,
		Outside
	}
	#endregion
	#region TickLabelPosition
	public enum TickLabelPosition {
		None = 0,
		High,
		Low,
		NextTo
	}
	#endregion
	#region TimeUnits
	public enum TimeUnits {
		Days = 0,
		Months = 1,
		Years = 2,
		Auto = 3
	}
	#endregion
	#region IAxisVisitor
	public interface IAxisVisitor {
		void Visit(CategoryAxis axis);
		void Visit(ValueAxis axis);
		void Visit(DateAxis axis);
		void Visit(SeriesAxis axis);
	}
	#endregion
	#region AxisInfo
	public class AxisInfo : ICloneable<AxisInfo>, ISupportsCopyFrom<AxisInfo>, ISupportsSizeOf {
		#region Static Members
		readonly static AxisInfo defaultInfo = new AxisInfo();
		public static AxisInfo DefaultInfo { get { return defaultInfo; } }
		#endregion
		#region Fields
		const int offsetCrosses = 2;
		const int offsetMajorTickMark = 4;
		const int offsetMinorTickMark = 6;
		const int offsetTickLabelPos = 8;
		const int offsetLabelAlign = 14;
		const int offsetLabelOffset = 16;
		const int offsetBaseTimeUnit = 26;
		const int offsetMajorTimeUnit = 28;
		const int offsetMinorTimeUnit = 30;
		const uint maskAuto		   = 0x00000001;
		const uint maskDelete		 = 0x00000002;
		const uint maskCrosses		= 0x0000000c;
		const uint maskMajorTickMark  = 0x00000030;
		const uint maskMinorTickMark  = 0x000000c0;
		const uint maskTickLabelPos   = 0x00000300;
		const uint maskCrossBetween   = 0x00000400;
		const uint maskNoMultilevel   = 0x00000800;
		const uint maskMajorGridlines = 0x00001000;
		const uint maskMinorGridlines = 0x00002000;
		const uint maskLabelAlign	 = 0x0000c000;
		const uint maskLabelOffset	= 0x03ff0000;
		const uint maskBaseTimeUnit   = 0x0c000000;
		const uint maskMajorTimeUnit  = 0x30000000;
		const uint maskMinorTimeUnit  = 0xc0000000;
		uint packedValues			 = 0xfc640301;
		#endregion
		#region Properties
		public bool Auto {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskAuto); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskAuto, value); }
		}
		public bool Delete {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDelete); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDelete, value); }
		}
		public AxisCrosses Crosses {
			get { return (AxisCrosses)PackedValues.GetIntBitValue(this.packedValues, maskCrosses, offsetCrosses); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskCrosses, offsetCrosses, (int)value); }
		}
		public TickMark MajorTickMark {
			get { return (TickMark)PackedValues.GetIntBitValue(this.packedValues, maskMajorTickMark, offsetMajorTickMark); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskMajorTickMark, offsetMajorTickMark, (int)value); }
		}
		public TickMark MinorTickMark {
			get { return (TickMark)PackedValues.GetIntBitValue(this.packedValues, maskMinorTickMark, offsetMinorTickMark); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskMinorTickMark, offsetMinorTickMark, (int)value); }
		}
		public TickLabelPosition TickLabelPos {
			get { return (TickLabelPosition)PackedValues.GetIntBitValue(this.packedValues, maskTickLabelPos, offsetTickLabelPos); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskTickLabelPos, offsetTickLabelPos, (int)value); }
		}
		public AxisCrossBetween CrossBetween {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskCrossBetween) ? AxisCrossBetween.Midpoint : AxisCrossBetween.Between; }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskCrossBetween, value == AxisCrossBetween.Midpoint); }
		}
		public bool NoMultilevelLabels {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskNoMultilevel); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskNoMultilevel, value); }
		}
		public bool ShowMajorGridlines {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskMajorGridlines); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskMajorGridlines, value); }
		}
		public bool ShowMinorGridlines {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskMinorGridlines); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskMinorGridlines, value); }
		}
		public LabelAlignment LabelAlign {
			get { return (LabelAlignment)PackedValues.GetIntBitValue(this.packedValues, maskLabelAlign, offsetLabelAlign); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskLabelAlign, offsetLabelAlign, (int)value); }
		}
		public int LabelOffset {
			get { return PackedValues.GetIntBitValue(this.packedValues, maskLabelOffset, offsetLabelOffset); }
			set {
				ValueChecker.CheckValue(value, 0, 1000, "LabelOffset");
				PackedValues.SetIntBitValue(ref this.packedValues, maskLabelOffset, offsetLabelOffset, value);
			}
		}
		public TimeUnits BaseTimeUnit {
			get { return (TimeUnits)PackedValues.GetIntBitValue(this.packedValues, maskBaseTimeUnit, offsetBaseTimeUnit); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskBaseTimeUnit, offsetBaseTimeUnit, (int)value); }
		}
		public TimeUnits MajorTimeUnit {
			get { return (TimeUnits)PackedValues.GetIntBitValue(this.packedValues, maskMajorTimeUnit, offsetMajorTimeUnit); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskMajorTimeUnit, offsetMajorTimeUnit, (int)value); }
		}
		public TimeUnits MinorTimeUnit {
			get { return (TimeUnits)PackedValues.GetIntBitValue(this.packedValues, maskMinorTimeUnit, offsetMinorTimeUnit); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskMinorTimeUnit, offsetMinorTimeUnit, (int)value); }
		}
		#endregion
		#region ICloneable<AxisInfo> Members
		public AxisInfo Clone() {
			AxisInfo result = new AxisInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<AxisInfo> Members
		public void CopyFrom(AxisInfo value) {
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
			AxisInfo other = obj as AxisInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues;
		}
		public override int GetHashCode() {
			return (int)this.packedValues;
		}
	}
	#endregion
	#region AxisInfoCache
	public class AxisInfoCache : UniqueItemsCache<AxisInfo> {
		public AxisInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override AxisInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new AxisInfo();
		}
	}
	#endregion
	#region AxisBase
	public abstract class AxisBase : SpreadsheetUndoableIndexBasedObject<AxisInfo>, ISupportsCopyFrom<AxisBase>, ChartsModel.IAxisLabelFormatter {
		#region Fields
		readonly IChart parent;
		AxisBase crossesAxis;
		double crossesValue;
		AxisPosition position;
		ShapeProperties majorGridlines;
		ShapeProperties minorGridlines;
		ShapeProperties shapeProperties;
		ScalingOptions scaling;
		TitleOptions title;
		NumberFormatOptions numberFormat;
		TextProperties textProperties;
		#endregion
		protected AxisBase(IChart parent)
			: base(parent.DocumentModel) {
			this.parent = parent;
			crossesAxis = null;
			crossesValue = 0.0;
			position = AxisPosition.Bottom;
			majorGridlines = new ShapeProperties(DocumentModel) { Parent = parent };
			minorGridlines = new ShapeProperties(DocumentModel) { Parent = parent };
			shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			scaling = new ScalingOptions(this.parent);
			title = new AxisTitleOptions(this.parent);
			numberFormat = new NumberFormatOptions(this.parent);
			textProperties = new TextProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		public abstract AxisDataType AxisType { get; }
		public AxisBase CrossesAxis { get { return crossesAxis; } set { crossesAxis = value; } }
		#region CrossesValue
		public double CrossesValue {
			get { return crossesValue; }
			set {
				if(crossesValue == value)
					return;
				SetCrossesValue(value);
			}
		}
		void SetCrossesValue(double value) {
			AxisCrossesValuePropertyChangedHistoryItem historyItem = new AxisCrossesValuePropertyChangedHistoryItem(DocumentModel, this, this.crossesValue, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetCrossesValueCore(double value) {
			crossesValue = value;
			Parent.Invalidate();
		}
		#endregion
		#region Position
		public AxisPosition Position { 
			get { return position; } 
			set {
				if(position == value)
					return;
				SetPosition(value);
			} 
		}
		void SetPosition(AxisPosition value) {
			AxisPositionPropertyChangedHistoryItem historyItem = new AxisPositionPropertyChangedHistoryItem(DocumentModel, this, this.position, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetPositionCore(AxisPosition value) {
			position = value;
			Parent.Invalidate();
		}
		#endregion
		public ShapeProperties MajorGridlines { get { return majorGridlines; } }
		public ShapeProperties MinorGridlines { get { return minorGridlines; } }
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public ScalingOptions Scaling { get { return scaling; } }
		public TitleOptions Title { get { return title; } }
		public NumberFormatOptions NumberFormat { get { return numberFormat; } }
		#region Crosses
		public AxisCrosses Crosses {
			get { return Info.Crosses; }
			set {
				if(Crosses == value)
					return;
				SetPropertyValue(SetCrossesCore, value);
			}
		}
		DocumentModelChangeActions SetCrossesCore(AxisInfo info, AxisCrosses value) {
			info.Crosses = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Delete
		public bool Delete {
			get { return Info.Delete; }
			set {
				if(Delete == value)
					return;
				SetPropertyValue(SetDeleteCore, value);
			}
		}
		DocumentModelChangeActions SetDeleteCore(AxisInfo info, bool value) {
			info.Delete = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MajorTickMark
		public TickMark MajorTickMark {
			get { return Info.MajorTickMark; }
			set {
				if(MajorTickMark == value)
					return;
				SetPropertyValue(SetMajorTickMarkCore, value);
			}
		}
		DocumentModelChangeActions SetMajorTickMarkCore(AxisInfo info, TickMark value) {
			info.MajorTickMark = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region MinorTickMark
		public TickMark MinorTickMark {
			get { return Info.MinorTickMark; }
			set {
				if(MinorTickMark == value)
					return;
				SetPropertyValue(SetMinorTickMarkCore, value);
			}
		}
		DocumentModelChangeActions SetMinorTickMarkCore(AxisInfo info, TickMark value) {
			info.MinorTickMark = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TickLabelPos
		public TickLabelPosition TickLabelPos {
			get { return Info.TickLabelPos; }
			set {
				if(TickLabelPos == value)
					return;
				SetPropertyValue(SetTickLabelPosCore, value);
			}
		}
		DocumentModelChangeActions SetTickLabelPosCore(AxisInfo info, TickLabelPosition value) {
			info.TickLabelPos = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowMajorGridlines
		public bool ShowMajorGridlines {
			get { return Info.ShowMajorGridlines; }
			set {
				if (ShowMajorGridlines == value)
					return;
				SetPropertyValue(SetShowMajorGridlinesCore, value);
			}
		}
		DocumentModelChangeActions SetShowMajorGridlinesCore(AxisInfo info, bool value) {
			info.ShowMajorGridlines = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowMinorGridlines
		public bool ShowMinorGridlines {
			get { return Info.ShowMinorGridlines; }
			set {
				if (ShowMinorGridlines == value)
					return;
				SetPropertyValue(SetShowMinorGridlinesCore, value);
			}
		}
		DocumentModelChangeActions SetShowMinorGridlinesCore(AxisInfo info, bool value) {
			info.ShowMinorGridlines = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public TextProperties TextProperties { get { return textProperties; } }
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<AxisInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.AxisInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			Parent.Invalidate();
		}
		#endregion
		#region ISupportsCopyFrom<AxisBase> Members
		public void CopyFrom(AxisBase value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			crossesAxis = value.crossesAxis;
			CrossesValue = value.CrossesValue;
			numberFormat.CopyFrom(value.numberFormat);
			Position = value.Position;
			majorGridlines.CopyFrom(value.majorGridlines);
			minorGridlines.CopyFrom(value.minorGridlines);
			shapeProperties.CopyFrom(value.shapeProperties);
			scaling.CopyFrom(value.scaling);
			title.CopyFrom(value.title);
			textProperties.CopyFrom(value.textProperties);
		}
		#endregion
		protected internal AxisBase CloneTo(IChart parent) {
			Guard.ArgumentNotNull(parent, "parent");
			return CloneToCore(parent);
		}
		protected abstract AxisBase CloneToCore(IChart parent);
		public abstract void Visit(IAxisVisitor visitor);
		#region IAxisLabelFormatterCore Members
		public virtual string GetAxisLabelText(object axisValue) {
			if (axisValue == null)
				return string.Empty;
			return axisValue.ToString();
		}
		#endregion
		public virtual void ResetToStyle() {
			ShapeProperties.ResetToStyle();
			MajorGridlines.ResetToStyle();
			MinorGridlines.ResetToStyle();
			Title.ResetToStyle();
			TextProperties.ResetToStyle();
		}
		protected internal virtual void CopyLayout(AxisBase value, bool percentStacked) {
			CopyLayoutCore(value);
		}
		protected virtual void CopyLayoutCore(AxisBase value) {
			MajorTickMark = value.MajorTickMark;
			MinorTickMark = value.MinorTickMark;
			TickLabelPos = value.TickLabelPos;
			ShowMajorGridlines = value.ShowMajorGridlines;
			ShowMinorGridlines = value.ShowMinorGridlines;
			numberFormat.CopyFrom(value.numberFormat);
			majorGridlines.CopyFrom(value.majorGridlines);
			minorGridlines.CopyFrom(value.minorGridlines);
			shapeProperties.CopyFrom(value.shapeProperties);
			scaling.CopyFrom(value.scaling);
			title.CopyFrom(value.title);
			textProperties.CopyFrom(value.textProperties);
		}
		#region Notifications
		public virtual void OnRangeInserting(InsertRangeNotificationContext context) {
			Title.OnRangeInserting(context);
		}
		public virtual void OnRangeRemoving(RemoveRangeNotificationContext context) {
			Title.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region AxisMMUnitsBase
	public abstract class AxisMMUnitsBase : AxisBase, ISupportsCopyFrom<AxisMMUnitsBase> {
		#region Fields
		double majorUnit;
		double minorUnit;
		#endregion
		protected AxisMMUnitsBase(IChart parent) 
			: base(parent) {
			this.majorUnit = 0.0;
			this.minorUnit = 0.0;
		}
		#region Properties
		#region MajorUnit
		public double MajorUnit {
			get { return majorUnit; }
			set {
				Guard.ArgumentNonNegative((float)value, "MajorUnit");
				if(majorUnit == value)
					return;
				SetMajorUnit(value);
			}
		}
		void SetMajorUnit(double value) {
			AxisMajorUnitPropertyChangedHistoryItem historyItem = new AxisMajorUnitPropertyChangedHistoryItem(DocumentModel, this, this.majorUnit, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetMajorUnitCore(double value) {
			this.majorUnit = value;
			Parent.Invalidate();
		}
		#endregion
		#region MinorUnit
		public double MinorUnit {
			get { return minorUnit; }
			set {
				Guard.ArgumentNonNegative((float)value, "MinorUnit");
				if(minorUnit == value)
					return;
				SetMinorUnit(value);
			}
		}
		void SetMinorUnit(double value) {
			AxisMinorUnitPropertyChangedHistoryItem historyItem = new AxisMinorUnitPropertyChangedHistoryItem(DocumentModel, this, this.minorUnit, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetMinorUnitCore(double value) {
			this.minorUnit = value;
			Parent.Invalidate();
		}
		#endregion
		public bool FixedMajorUnit { get { return majorUnit != 0.0; } }
		public bool FixedMinorUnit { get { return minorUnit != 0.0; } }
		#endregion
		#region ISupportsCopyFrom<AxisMMUnitsBase> Members
		public void CopyFrom(AxisMMUnitsBase value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.MajorUnit = value.MajorUnit;
			this.MinorUnit = value.MinorUnit;
		}
		#endregion
		protected override void CopyLayoutCore(AxisBase value) {
			base.CopyLayoutCore(value);
			AxisMMUnitsBase other = value as AxisMMUnitsBase;
			if (other != null) {
				MajorUnit = other.MajorUnit;
				MinorUnit = other.MinorUnit;
			}
		}
	}
	#endregion
	#region AxisTickBase
	public abstract class AxisTickBase : AxisBase, ISupportsCopyFrom<AxisTickBase> {
		public static int DefaultTickSkip { get { return defaultTickSkip; } }
		#region Fields
		const int defaultTickSkip = 1;
		int tickLabelSkip;
		int tickMarkSkip;
		#endregion
		protected AxisTickBase(IChart parent)
			: base(parent) {
			this.tickLabelSkip = defaultTickSkip;
			this.tickMarkSkip = defaultTickSkip;
		}
		#region Properties
		#region TickLabelSkip
		public int TickLabelSkip {
			get { return tickLabelSkip; }
			set {
				if (value <= 0)
					throw new ArgumentException();
				if(tickLabelSkip == value)
					return;
				SetTickLabelSkip(value);
			}
		}
		void SetTickLabelSkip(int value) {
			AxisTickLabelSkipPropertyChangedHistoryItem historyItem = new AxisTickLabelSkipPropertyChangedHistoryItem(DocumentModel, this, this.tickLabelSkip, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetTickLabelSkipCore(int value) {
			this.tickLabelSkip = value;
			Parent.Invalidate();
		}
		#endregion
		#region TickMarkSkip
		public int TickMarkSkip {
			get { return tickMarkSkip; }
			set {
				if (value <= 0)
					throw new ArgumentException();
				if (tickMarkSkip == value)
					return;
				SetTickMarkSkip(value);
			}
		}
		void SetTickMarkSkip(int value) {
			AxisTickMarkSkipPropertyChangedHistoryItem historyItem = new AxisTickMarkSkipPropertyChangedHistoryItem(DocumentModel, this, this.tickLabelSkip, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetTickMarkSkipCore(int value) {
			this.tickMarkSkip = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region ISupportsCopyFrom<AxisTickBase> Members
		public void CopyFrom(AxisTickBase value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.TickLabelSkip = value.TickLabelSkip;
			this.TickMarkSkip = value.TickMarkSkip;
		}
		#endregion
		protected override void CopyLayoutCore(AxisBase value) {
			base.CopyLayoutCore(value);
			AxisTickBase other = value as AxisTickBase;
			if (other != null) {
				TickLabelSkip = other.TickLabelSkip;
				TickMarkSkip = other.TickMarkSkip;
			}
		}
	}
	#endregion
}
