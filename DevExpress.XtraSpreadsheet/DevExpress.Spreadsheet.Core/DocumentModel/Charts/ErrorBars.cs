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
	#region ErrorBarTypes
	public enum ErrorBarType {
		Both = 0,
		Minus = 1,
		Plus = 2
	}
	#endregion
	#region ErrorBarDirections
	public enum ErrorBarDirection {
		Auto = 0,
		X = 1,
		Y = 2
	}
	#endregion
	#region ErrorValueType
	public enum ErrorValueType {
		FixedValue = 0,
		Percentage = 1,
		StandardDeviation = 2,
		StandardError = 3,
		Custom = 4
	}
	#endregion
	#region ErrorBarsInfo
	public class ErrorBarsInfo : ICloneable<ErrorBarsInfo>, ISupportsCopyFrom<ErrorBarsInfo>, ISupportsSizeOf {
		#region Fields
		const int offsetBarDirection = 2;
		const int offsetValueType = 4;
		const uint maskBarType = 0x0003;
		const uint maskBarDirection = 0x000c;
		const uint maskValueType = 0x0070;
		const uint maskNoEndCap = 0x0080;
		uint packedValues = 0x0080;
		#endregion
		#region Properties
		public ErrorBarType BarType {
			get { return (ErrorBarType)PackedValues.GetIntBitValue(this.packedValues, maskBarType); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskBarType, (int)value);  }
		}
		public ErrorBarDirection BarDirection {
			get { return (ErrorBarDirection)PackedValues.GetIntBitValue(this.packedValues, maskBarDirection, offsetBarDirection); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskBarDirection, offsetBarDirection, (int)value); }
		}
		public ErrorValueType ValueType {
			get { return (ErrorValueType)PackedValues.GetIntBitValue(this.packedValues, maskValueType, offsetValueType); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskValueType, offsetValueType, (int)value); }
		}
		public bool NoEndCap {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskNoEndCap); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskNoEndCap, value); }
		}
		#endregion
		#region ICloneable<ErrorBarsInfo> Members
		public ErrorBarsInfo Clone() {
			ErrorBarsInfo result = new ErrorBarsInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ErrorBarsInfo> Members
		public void CopyFrom(ErrorBarsInfo value) {
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
			ErrorBarsInfo other = obj as ErrorBarsInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues;
		}
		public override int GetHashCode() {
			return (int)this.packedValues;
		}
	}
	#endregion
	#region ErrorBarsInfoCache
	public class ErrorBarsInfoCache : UniqueItemsCache<ErrorBarsInfo> {
		public ErrorBarsInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override ErrorBarsInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new ErrorBarsInfo();
		}
	}
	#endregion
	#region ErrorBars
	public class ErrorBars : SpreadsheetUndoableIndexBasedObject<ErrorBarsInfo>, ISupportsCopyFrom<ErrorBars> {
		#region Fields
		readonly IChart parent;
		IDataReference minus;
		IDataReference plus;
		ShapeProperties shapeProperties;
		double value;
		#endregion
		public ErrorBars(IChart parent) 
			: base(parent.DocumentModel) {
			this.parent = parent;
			this.minus = DataReference.Empty;
			this.plus = DataReference.Empty;
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.value = 5.0;
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		#region BarType
		public ErrorBarType BarType {
			get { return Info.BarType; }
			set {
				if(BarType == value)
					return;
				SetPropertyValue(SetBarTypeCore, value);
			}
		}
		DocumentModelChangeActions SetBarTypeCore(ErrorBarsInfo info, ErrorBarType value) {
			info.BarType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region BarDirection
		public ErrorBarDirection BarDirection {
			get { return Info.BarDirection; }
			set {
				if(BarDirection == value)
					return;
				SetPropertyValue(SetBarDirectionCore, value);
			}
		}
		DocumentModelChangeActions SetBarDirectionCore(ErrorBarsInfo info, ErrorBarDirection value) {
			info.BarDirection = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ValueType
		public ErrorValueType ValueType {
			get { return Info.ValueType; }
			set {
				if(ValueType == value)
					return;
				SetPropertyValue(SetValueTypeCore, value);
			}
		}
		DocumentModelChangeActions SetValueTypeCore(ErrorBarsInfo info, ErrorValueType value) {
			info.ValueType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region NoEndCap
		public bool NoEndCap {
			get { return Info.NoEndCap; }
			set {
				if(NoEndCap == value)
					return;
				SetPropertyValue(SetNoEndCapCore, value);
			}
		}
		DocumentModelChangeActions SetNoEndCapCore(ErrorBarsInfo info, bool value) {
			info.NoEndCap = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Minus
		public IDataReference Minus {
			get { return minus; }
			set {
				if(value == null)
					value = DataReference.Empty;
				if(minus.Equals(value))
					return;
				SetMinus(value);
			}
		}
		void SetMinus(IDataReference value) {
			ErrorBarsMinusPropertyChangedHistoryItem historyItem = new ErrorBarsMinusPropertyChangedHistoryItem(DocumentModel, this, minus, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetMinusCore(IDataReference value) {
			this.minus = value;
			Parent.Invalidate();
		}
		#endregion
		#region Plus
		public IDataReference Plus {
			get { return plus; }
			set {
				if(value == null)
					value = DataReference.Empty;
				if(plus.Equals(value))
					return;
				SetPlus(value);
			}
		}
		void SetPlus(IDataReference value) {
			ErrorBarsPlusPropertyChangedHistoryItem historyItem = new ErrorBarsPlusPropertyChangedHistoryItem(DocumentModel, this, plus, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetPlusCore(IDataReference value) {
			this.plus = value;
			Parent.Invalidate();
		}
		#endregion
		#region Value
		public double Value {
			get { return value; }
			set {
				if(this.value == value)
					return;
				SetValue(value);
			}
		}
		void SetValue(double value) {
			ErrorBarsValuePropertyChangedHistoryItem historyItem = new ErrorBarsValuePropertyChangedHistoryItem(DocumentModel, this, this.value, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetValueCore(double value) {
			this.value = value;
			Parent.Invalidate();
		}
		#endregion
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<ErrorBarsInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.ErrorBarsInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			Parent.Invalidate();
		}
		#endregion
		#region ISupportsCopyFrom<ErrorBars> Members
		public void CopyFrom(ErrorBars value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			Minus = value.Minus.CloneTo(this.DocumentModel);
			Plus = value.Plus.CloneTo(this.DocumentModel);
			Value = value.Value;
			this.shapeProperties.CopyFrom(value.shapeProperties);
		}
		#endregion
		public void ResetToStyle() {
			ShapeProperties.ResetToStyle();
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			minus.OnRangeInserting(context);
			plus.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			minus.OnRangeRemoving(context);
			plus.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region ErrorBarsCollection
	public class ErrorBarsCollection : ChartUndoableCollectionSupportsCopyFrom<ErrorBars> {
		public ErrorBarsCollection(IChart parent)
			: base(parent) {
		}
		protected override ErrorBars CreateNewItem(ErrorBars source) {
			ErrorBars result = new ErrorBars(Parent);
			result.CopyFrom(source);
			return result;
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			foreach (ErrorBars bars in this)
				bars.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			foreach (ErrorBars bars in this)
				bars.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
}
