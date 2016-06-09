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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet.Charts {
	#region ErrorBarTypes
	public enum ErrorBarType {
		Both = DevExpress.XtraSpreadsheet.Model.ErrorBarType.Both,
		Minus = DevExpress.XtraSpreadsheet.Model.ErrorBarType.Minus,
		Plus = DevExpress.XtraSpreadsheet.Model.ErrorBarType.Plus
	}
	#endregion
	#region ErrorBarDirections
	public enum ErrorBarDirection {
		Auto = DevExpress.XtraSpreadsheet.Model.ErrorBarDirection.Auto,
		X = DevExpress.XtraSpreadsheet.Model.ErrorBarDirection.X,
		Y = DevExpress.XtraSpreadsheet.Model.ErrorBarDirection.Y
	}
	#endregion
	#region ErrorBarValueType
	public enum ErrorBarValueType {
		FixedValue = DevExpress.XtraSpreadsheet.Model.ErrorValueType.FixedValue,
		Percentage = DevExpress.XtraSpreadsheet.Model.ErrorValueType.Percentage,
		StandardDeviation = DevExpress.XtraSpreadsheet.Model.ErrorValueType.StandardDeviation,
		StandardError = DevExpress.XtraSpreadsheet.Model.ErrorValueType.StandardError,
		Custom = DevExpress.XtraSpreadsheet.Model.ErrorValueType.Custom
	}
	#endregion
	public interface ErrorBarsOptions : ShapeFormat {
		ErrorBarType BarType { get; set; }
		ErrorBarDirection BarDirection { get; set; }
		ErrorBarValueType ValueType { get; set; }
		bool NoEndCap { get; set; }
		double Value { get; set; }
		ChartData Minus { get; set; }
		ChartData Plus { get; set; }
	}
	public interface ErrorBarsCollection : ISimpleCollection<ErrorBarsOptions> {
		ErrorBarsOptions Add(ErrorBarType barType);
		ErrorBarsOptions Add(ErrorBarType barType, ErrorBarDirection barDirection);
		bool Remove(ErrorBarsOptions errorBars);
		void RemoveAt(int index);
		void Clear();
		bool Contains(ErrorBarsOptions errorBars);
		int IndexOf(ErrorBarsOptions errorBars);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	#region NativeErrorBarsOptions
	partial class NativeErrorBarsOptions : NativeShapeFormat, ErrorBarsOptions {
		#region Fields
		Model.ErrorBars modelOptions;
		#endregion
		public NativeErrorBarsOptions(Model.ErrorBars modelOptions, NativeWorkbook nativeWorkbook)
			: base(modelOptions.ShapeProperties, nativeWorkbook) {
			this.modelOptions = modelOptions;
		}
		#region ErrorBarsOptions Members
		public ErrorBarType BarType {
			get {
				CheckValid();
				return (ErrorBarType)modelOptions.BarType;
			}
			set {
				CheckValid();
				modelOptions.BarType = (Model.ErrorBarType)value;
			}
		}
		public ErrorBarDirection BarDirection {
			get {
				CheckValid();
				return (ErrorBarDirection)modelOptions.BarDirection;
			}
			set {
				CheckValid();
				modelOptions.BarDirection = (Model.ErrorBarDirection)value;
			}
		}
		public ErrorBarValueType ValueType {
			get {
				CheckValid();
				return (ErrorBarValueType)modelOptions.ValueType;
			}
			set {
				CheckValid();
				modelOptions.ValueType = (Model.ErrorValueType)value;
			}
		}
		public bool NoEndCap {
			get {
				CheckValid();
				return modelOptions.NoEndCap;
			}
			set {
				CheckValid();
				modelOptions.NoEndCap = value;
			}
		}
		public double Value {
			get {
				CheckValid();
				return modelOptions.Value;
			}
			set {
				CheckValid();
				modelOptions.Value = value;
			}
		}
		public ChartData Minus {
			get {
				CheckValid();
				return ChartDataConverter.ToChartData(modelOptions.Minus, NativeWorkbook);
			}
			set {
				CheckValid();
				modelOptions.Minus = ChartDataConverter.ToDataReference(value, modelOptions.DocumentModel, 
					Model.ChartViewSeriesDirection.Vertical, true);
			}
		}
		public ChartData Plus {
			get {
				CheckValid();
				return ChartDataConverter.ToChartData(modelOptions.Plus, NativeWorkbook);
			}
			set {
				CheckValid();
				modelOptions.Plus = ChartDataConverter.ToDataReference(value, modelOptions.DocumentModel,
					Model.ChartViewSeriesDirection.Vertical, true);
			}
		}
		#endregion
		public override bool Equals(object obj) {
			if (!IsValid)
				return false;
			NativeErrorBarsOptions other = obj as NativeErrorBarsOptions;
			if (other == null || !other.IsValid)
				return false;
			return object.ReferenceEquals(modelOptions, other.modelOptions);
		}
		public override int GetHashCode() {
			if (!IsValid)
				return -1;
			return modelOptions.GetHashCode();
		}
	}
	#endregion
	#region NativeErrorBarsCollection
	partial class NativeErrorBarsCollection : NativeChartCollectionBase<ErrorBarsOptions, NativeErrorBarsOptions, Model.ErrorBars>, ErrorBarsCollection {
		readonly NativeWorkbook nativeWorkbook;
		public NativeErrorBarsCollection(Model.ErrorBarsCollection modelCollection, NativeWorkbook nativeWorkbook)
			: base(modelCollection) {
			this.nativeWorkbook = nativeWorkbook;
		}
		Model.ErrorBarsCollection ModelErrorBars { get { return ModelCollection as Model.ErrorBarsCollection; } }
		protected override NativeErrorBarsOptions CreateNativeObject(Model.ErrorBars modelItem) {
			return new NativeErrorBarsOptions(modelItem, nativeWorkbook);
		}
		#region ErrorBarsCollection Members
		public ErrorBarsOptions Add(ErrorBarType barType) {
			CheckValid();
			CheckAddBarDirection(Model.ErrorBarDirection.Auto);
			ModelErrorBars.DocumentModel.BeginUpdate();
			try {
				Model.ErrorBars modelItem = new Model.ErrorBars(ModelErrorBars.Parent);
				InitModelItem(modelItem, (Model.ErrorBarType)barType, Model.ErrorBarDirection.Auto);
				ModelErrorBars.Add(modelItem);
			}
			finally {
				ModelErrorBars.DocumentModel.EndUpdate();
			}
			return InnerList[Count - 1];
		}
		public ErrorBarsOptions Add(ErrorBarType barType, ErrorBarDirection barDirection) {
			CheckValid();
			CheckAddBarDirection((Model.ErrorBarDirection)barDirection);
			ModelErrorBars.DocumentModel.BeginUpdate();
			try {
				Model.ErrorBars modelItem = new Model.ErrorBars(ModelErrorBars.Parent);
				InitModelItem(modelItem, (Model.ErrorBarType)barType, (Model.ErrorBarDirection)barDirection);
				ModelErrorBars.Add(modelItem);
			}
			finally {
				ModelErrorBars.DocumentModel.EndUpdate();
			}
			return InnerList[Count - 1];
		}
		public bool Remove(ErrorBarsOptions errorBars) {
			int index = IndexOf(errorBars);
			if (index != -1)
				RemoveAt(index);
			return index != -1;
		}
		public bool Contains(ErrorBarsOptions errorBars) {
			CheckValid();
			return IndexOf(errorBars) != -1;
		}
		public int IndexOf(ErrorBarsOptions errorBars) {
			CheckValid();
			NativeErrorBarsOptions nativeItem = errorBars as NativeErrorBarsOptions;
			if (nativeItem != null)
				return InnerList.IndexOf(nativeItem);
			return -1;
		}
		#endregion
		#region Internal
		void InitModelItem(Model.ErrorBars modelItem, Model.ErrorBarType barType, Model.ErrorBarDirection barDirection) {
			modelItem.BeginUpdate();
			try {
				modelItem.BarType = barType;
				modelItem.BarDirection = barDirection;
				modelItem.ValueType = Model.ErrorValueType.Percentage;
				modelItem.Value = 5.0;
				modelItem.NoEndCap = false;
			} finally {
				modelItem.EndUpdate();
			}
		}
		void CheckAddBarDirection(Model.ErrorBarDirection barDirection) {
			int count = ModelErrorBars.Count;
			if (count >= 2)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorIncorrectErrorBarsCollectionCount);
			for (int i = 0; i < count; i++)
				CheckBarDirection(barDirection, ModelErrorBars[i].BarDirection);
		}
		void CheckBarDirection(Model.ErrorBarDirection addedBarDirection, Model.ErrorBarDirection existingBarDirection) {
			if (addedBarDirection == existingBarDirection && (addedBarDirection != Model.ErrorBarDirection.Auto || existingBarDirection != Model.ErrorBarDirection.Auto))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorErrorBarsWithBarDirectionAlreadyExists);
		}
		#endregion
	}
	#endregion
}
