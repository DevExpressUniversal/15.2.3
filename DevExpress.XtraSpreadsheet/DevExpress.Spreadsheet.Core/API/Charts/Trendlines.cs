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
using DevExpress.Spreadsheet.Drawings;
namespace DevExpress.Spreadsheet.Charts {
	public enum ChartTrendlineType {
		Linear = DevExpress.XtraSpreadsheet.Model.ChartTrendlineType.Linear,
		Logarithmic = DevExpress.XtraSpreadsheet.Model.ChartTrendlineType.Logarithmic,
		Exponential = DevExpress.XtraSpreadsheet.Model.ChartTrendlineType.Exponential,
		MovingAverage = DevExpress.XtraSpreadsheet.Model.ChartTrendlineType.MovingAverage,
		Polynomial = DevExpress.XtraSpreadsheet.Model.ChartTrendlineType.Polynomial,
		Power = DevExpress.XtraSpreadsheet.Model.ChartTrendlineType.Power
	}
	public interface TrendlineLabel : ShapeFormat, ShapeTextFormat {
		ChartText Text { get; }
		LayoutOptions Layout { get; }
		NumberFormatOptions NumberFormat { get; }
	}
	public interface Trendline : ShapeFormat {
		ChartTrendlineType TrendlineType { get; set; }
		bool AutoName { get; set; }
		string CustomName { get; set; }
		bool DisplayEquation { get; set; }
		bool DisplayRSquare { get; set; }
		int Order { get; set; }
		int Period { get; set; }
		double Backward { get; set; }
		double Forward { get; set; }
		double Intercept { get; set; }
		bool HasIntercept { get; set; }
		TrendlineLabel Label { get; }
	}
	public interface TrendlineCollection : ISimpleCollection<Trendline> {
		Trendline Add(ChartTrendlineType trendlineType);
		Trendline Add(ChartTrendlineType trendlineType, string customName);
		bool Remove(Trendline trendline);
		void RemoveAt(int index);
		void Clear();
		bool Contains(Trendline trendline);
		int IndexOf(Trendline trendline);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	#region NativeTrendlineLabel
	partial class NativeTrendlineLabel : NativeShapeFormat, TrendlineLabel {
		#region Fields
		readonly Model.TrendlineLabel modelLabel;
		NativeChartText text;
		NativeLayoutOptions layout;
		NativeNumberFormatOptions numberFormat;
		NativeShapeTextFormat textFormat;
		#endregion
		public NativeTrendlineLabel(Model.TrendlineLabel modelLabel, NativeWorkbook nativeWorkbook)
			: base(modelLabel.ShapeProperties, nativeWorkbook) {
			this.modelLabel = modelLabel;
		}
		#region TrendlineLabel Members
		public ChartText Text {
			get {
				CheckValid();
				if (text == null)
					text = new NativeChartText(modelLabel);
				return text;
			}
		}
		public LayoutOptions Layout {
			get {
				CheckValid();
				if (layout == null)
					layout = new NativeLayoutOptions(modelLabel.Layout);
				return layout;
			}
		}
		public NumberFormatOptions NumberFormat {
			get {
				CheckValid();
				if (numberFormat == null)
					numberFormat = new NativeNumberFormatOptions(modelLabel.NumberFormat);
				return numberFormat; 
			}
		}
		#endregion
		#region ShapeTextFormat Members
		public ShapeTextFont Font {
			get {
				CheckValid();
				CheckTextFormat();
				return textFormat.Font; 
			}
		}
		public ShapeTextDirection TextDirection {
			get {
				CheckValid();
				CheckTextFormat();
				return textFormat.TextDirection;
			}
			set {
				CheckValid();
				CheckTextFormat();
				textFormat.TextDirection = value;
			}
		}
		public int TextRotation {
			get {
				CheckValid();
				CheckTextFormat();
				return textFormat.TextRotation;
			}
			set {
				CheckValid();
				CheckTextFormat();
				textFormat.TextRotation = value;
			}
		}
		void CheckTextFormat() {
			if (textFormat == null)
				textFormat = new NativeShapeTextFormat(modelLabel);
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (text != null)
				text.IsValid = value;
			if (layout != null)
				layout.IsValid = value;
			if (numberFormat != null)
				numberFormat.IsValid = value;
			if (textFormat != null)
				textFormat.IsValid = value;
		}
		public override void ResetToMatchStyle() {
			modelLabel.DocumentModel.BeginUpdate();
			try {
				modelLabel.ResetToStyle();
			}
			finally {
				modelLabel.DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region NativeTrendline
	partial class NativeTrendline : NativeShapeFormat, Trendline {
		#region Fields
		readonly Model.Trendline modelTrendline;
		NativeTrendlineLabel label;
		#endregion
		public NativeTrendline(Model.Trendline modelTrendline, NativeWorkbook nativeWorkbook)
			: base(modelTrendline.ShapeProperties, nativeWorkbook) {
			this.modelTrendline = modelTrendline;
		}
		#region Trendline Members
		public ChartTrendlineType TrendlineType {
			get {
				CheckValid();
				return (ChartTrendlineType)modelTrendline.TrendlineType;
			}
			set {
				CheckValid();
				modelTrendline.TrendlineType = (Model.ChartTrendlineType)value;
			}
		}
		public bool AutoName {
			get {
				CheckValid();
				return !modelTrendline.HasCustomName;
			}
			set {
				CheckValid();
				modelTrendline.HasCustomName = !value;
			}
		}
		public string CustomName {
			get {
				CheckValid();
				return modelTrendline.HasCustomName ? modelTrendline.Name : string.Empty;
			}
			set {
				CheckValid();
				modelTrendline.DocumentModel.BeginUpdate();
				try {
					modelTrendline.Name = value;
					modelTrendline.HasCustomName = true;
				}
				finally {
					modelTrendline.DocumentModel.EndUpdate();
				}
			}
		}
		public bool DisplayEquation {
			get {
				CheckValid();
				return modelTrendline.DisplayEquation;
			}
			set {
				CheckValid();
				modelTrendline.DisplayEquation = value;
			}
		}
		public bool DisplayRSquare {
			get {
				CheckValid();
				return modelTrendline.DisplayRSquare;
			}
			set {
				CheckValid();
				modelTrendline.DisplayRSquare = value;
			}
		}
		public int Order {
			get {
				CheckValid();
				return modelTrendline.Order;
			}
			set {
				CheckValid();
				modelTrendline.Order = value;
			}
		}
		public int Period {
			get {
				CheckValid();
				return modelTrendline.Period;
			}
			set {
				CheckValid();
				modelTrendline.Period = value;
			}
		}
		public double Backward {
			get {
				CheckValid();
				return modelTrendline.Backward;
			}
			set {
				CheckValid();
				modelTrendline.Backward = value;
			}
		}
		public double Forward {
			get {
				CheckValid();
				return modelTrendline.Forward;
			}
			set {
				CheckValid();
				modelTrendline.Forward = value;
			}
		}
		public double Intercept {
			get {
				CheckValid();
				return modelTrendline.Intercept;
			}
			set {
				CheckValid();
				modelTrendline.Intercept = value;
			}
		}
		public bool HasIntercept {
			get {
				CheckValid();
				return modelTrendline.HasIntercept;
			}
			set {
				CheckValid();
				modelTrendline.HasIntercept = value;
			}
		}
		public TrendlineLabel Label {
			get {
				CheckValid();
				if (label == null)
					label = new NativeTrendlineLabel(modelTrendline.Label, NativeWorkbook);
				return label; 
			}
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (label != null)
				label.IsValid = value;
		}
		public override void ResetToMatchStyle() {
			modelTrendline.DocumentModel.BeginUpdate();
			try {
				modelTrendline.ResetToStyle();
			}
			finally {
				modelTrendline.DocumentModel.EndUpdate();
			}
		}
		public override bool Equals(object obj) {
			if (!IsValid)
				return false;
			NativeTrendline other = obj as NativeTrendline;
			if (other == null || !other.IsValid)
				return false;
			return object.ReferenceEquals(modelTrendline, other.modelTrendline);
		}
		public override int GetHashCode() {
			if (!IsValid)
				return -1;
			return modelTrendline.GetHashCode();
		}
	}
	#endregion
	#region NativeTrendlineCollection
	partial class NativeTrendlineCollection : NativeChartCollectionBase<Trendline, NativeTrendline, Model.Trendline>, TrendlineCollection {
		readonly NativeWorkbook nativeWorkbook;
		public NativeTrendlineCollection(Model.TrendlineCollection modelCollection, NativeWorkbook nativeWorkbook)
			: base(modelCollection) {
			this.nativeWorkbook = nativeWorkbook;
		}
		Model.TrendlineCollection ModelTrendlines { get { return ModelCollection as Model.TrendlineCollection; } }
		protected override NativeTrendline CreateNativeObject(Model.Trendline modelItem) {
			return new NativeTrendline(modelItem, nativeWorkbook);
		}
		#region TrendlineCollection Members
		public Trendline Add(ChartTrendlineType trendlineType) {
			CheckValid();
			ModelTrendlines.DocumentModel.BeginUpdate();
			try {
				Model.Trendline modelTrendline = new Model.Trendline(ModelTrendlines.Parent);
				modelTrendline.TrendlineType = (Model.ChartTrendlineType)trendlineType;
				ModelTrendlines.Add(modelTrendline);
			}
			finally {
				ModelTrendlines.DocumentModel.EndUpdate();
			}
			return InnerList[Count - 1];
		}
		public Trendline Add(ChartTrendlineType trendlineType, string customName) {
			CheckValid();
			ModelTrendlines.DocumentModel.BeginUpdate();
			try {
				Model.Trendline modelTrendline = new Model.Trendline(ModelTrendlines.Parent);
				modelTrendline.TrendlineType = (Model.ChartTrendlineType)trendlineType;
				modelTrendline.Name = customName;
				modelTrendline.HasCustomName = true;
				ModelTrendlines.Add(modelTrendline);
			}
			finally {
				ModelTrendlines.DocumentModel.EndUpdate();
			}
			return InnerList[Count - 1];
		}
		public bool Remove(Trendline trendline) {
			int index = IndexOf(trendline);
			if (index != -1)
				RemoveAt(index);
			return index != -1;
		}
		public bool Contains(Trendline trendline) {
			CheckValid();
			return IndexOf(trendline) != -1;
		}
		public int IndexOf(Trendline trendline) {
			CheckValid();
			NativeTrendline nativeTrendline = trendline as NativeTrendline;
			if (nativeTrendline != null)
				return InnerList.IndexOf(nativeTrendline);
			return -1;
		}
		#endregion
	}
	#endregion
}
