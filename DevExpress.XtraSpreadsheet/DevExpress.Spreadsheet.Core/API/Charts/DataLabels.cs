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
	public enum DataLabelPosition {
		Default = DevExpress.XtraSpreadsheet.Model.DataLabelPosition.Default,
		Left = DevExpress.XtraSpreadsheet.Model.DataLabelPosition.Left,
		Top = DevExpress.XtraSpreadsheet.Model.DataLabelPosition.Top,
		Right = DevExpress.XtraSpreadsheet.Model.DataLabelPosition.Right,
		Bottom = DevExpress.XtraSpreadsheet.Model.DataLabelPosition.Bottom,
		Center = DevExpress.XtraSpreadsheet.Model.DataLabelPosition.Center,
		BestFit = DevExpress.XtraSpreadsheet.Model.DataLabelPosition.BestFit,
		InsideBase = DevExpress.XtraSpreadsheet.Model.DataLabelPosition.InsideBase,
		InsideEnd = DevExpress.XtraSpreadsheet.Model.DataLabelPosition.InsideEnd,
		OutsideEnd = DevExpress.XtraSpreadsheet.Model.DataLabelPosition.OutsideEnd
	}
	public interface DataLabelBase : ShapeFormat, ShapeTextFormat {
		bool Hidden { get; set; }
		bool ShowBubbleSize { get; set; }
		bool ShowCategoryName { get; set; }
		bool ShowSeriesName { get; set; }
		bool ShowLegendKey { get; set; }
		bool ShowPercent { get; set; }
		bool ShowValue { get; set; }
		string Separator { get; set; }
		DataLabelPosition LabelPosition { get; set; }
		NumberFormatOptions NumberFormat { get; }
	}
	public interface DataLabel : DataLabelBase {
		int Index { get; }
		LayoutOptions Layout { get; }
		ChartText Text { get; }
	}
	public interface DataLabelOptions : DataLabelBase {
		ChartLineOptions LeaderLines { get; }
	}
	public interface DataLabelCollection : ISimpleCollection<DataLabel>, DataLabelOptions {
		DataLabel Add(int itemIndex);
		bool Remove(DataLabel dataLabel);
		void RemoveAt(int index);
		void Clear();
		bool Contains(DataLabel dataLabel);
		int IndexOf(DataLabel dataLabel);
		DataLabel FindByIndex(int itemIndex);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	#region NativeDataLabelBase
	partial class NativeDataLabelBase : NativeShapeTextFormat, DataLabelBase { 
		#region Fields
		readonly Model.DataLabelBase modelDataLabelBase;
		readonly NativeWorkbook nativeWorkbook;
		NativeNumberFormatOptions nativeNumberFormatOptions;
		NativeShapeFill nativeShapeFill;
		NativeShapeOutline nativeShapeOutline;
		#endregion
		public NativeDataLabelBase(Model.DataLabel modelDataLabel, NativeWorkbook nativeWorkbook)
			: base(modelDataLabel) {
			this.modelDataLabelBase = modelDataLabel;
			this.nativeWorkbook = nativeWorkbook;
		}
		public NativeDataLabelBase(Model.DataLabels modelDataLabels, NativeWorkbook nativeWorkbook)
			: base(modelDataLabels.TextProperties) {
			this.modelDataLabelBase = modelDataLabels;
			this.nativeWorkbook = nativeWorkbook;
		}
		protected Model.DataLabelBase ModelDataLabelBase { get { return modelDataLabelBase; } }
		protected NativeWorkbook NativeWorkbook { get { return nativeWorkbook; } }
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeNumberFormatOptions != null)
				nativeNumberFormatOptions.IsValid = value;
			if (nativeShapeFill != null)
				nativeShapeFill.IsValid = value;
			if (nativeShapeOutline != null)
				nativeShapeOutline.IsValid = value;
		}
		#region DataLabelBase Members
		public bool Hidden {
			get {
				CheckValid();
				return modelDataLabelBase.Delete;
			}
			set {
				CheckValid();
				modelDataLabelBase.Delete = value;
			}
		}
		public bool ShowBubbleSize {
			get {
				CheckValid();
				return modelDataLabelBase.ShowBubbleSize;
			}
			set {
				CheckValid();
				modelDataLabelBase.ShowBubbleSize = value;
			}
		}
		public bool ShowCategoryName {
			get {
				CheckValid();
				return modelDataLabelBase.ShowCategoryName;
			}
			set {
				CheckValid();
				modelDataLabelBase.ShowCategoryName = value;
			}
		}
		public bool ShowSeriesName {
			get {
			   CheckValid();
			   return modelDataLabelBase.ShowSeriesName;
			}
			set {
				CheckValid();
				modelDataLabelBase.ShowSeriesName = value;
			}
		}
		public bool ShowLegendKey {
			get {
				CheckValid();
				return modelDataLabelBase.ShowLegendKey;
			}
			set {
				CheckValid();
				modelDataLabelBase.ShowLegendKey = value;
			}
		}
		public bool ShowPercent {
			get {
				CheckValid();
				return modelDataLabelBase.ShowPercent;
			}
			set {
				CheckValid();
				modelDataLabelBase.ShowPercent = value;
			}
		}
		public bool ShowValue {
			get {
				CheckValid();
				return modelDataLabelBase.ShowValue;
			}
			set {
				CheckValid();
				modelDataLabelBase.ShowValue = value;
			}
		}
		public string Separator {
			get {
				CheckValid();
				return modelDataLabelBase.Separator;
			}
			set {
				CheckValid();
				modelDataLabelBase.Separator = value;
			}
		}
		public DataLabelPosition LabelPosition {
			get {
				CheckValid();
				return (DataLabelPosition)modelDataLabelBase.LabelPosition;
			}
			set {
				CheckValid();
				modelDataLabelBase.LabelPosition = (Model.DataLabelPosition)value;
			}
		}
		public NumberFormatOptions NumberFormat {
			get {
				CheckValid();
				if (nativeNumberFormatOptions == null)
					nativeNumberFormatOptions = new NativeNumberFormatOptions(modelDataLabelBase.NumberFormat);
				return nativeNumberFormatOptions;
			}
		}
		public ShapeFill Fill {
			get {
				CheckValid();
				if (nativeShapeFill == null)
					nativeShapeFill = new NativeShapeFill(modelDataLabelBase.ShapeProperties, nativeWorkbook);
				return nativeShapeFill;
			}
		}
		public ShapeOutline Outline {
			get {
				CheckValid();
				if (nativeShapeOutline == null)
					nativeShapeOutline = new NativeShapeOutline(modelDataLabelBase.ShapeProperties.Outline, nativeWorkbook);
				return nativeShapeOutline;
			}
		}
		public void ResetToMatchStyle() {
			CheckValid();
			modelDataLabelBase.ResetToStyle();
		}
		#endregion
	}
	#endregion
	#region NativeDataLabel
	partial class NativeDataLabel : NativeDataLabelBase, DataLabel, ISupportIndex {
		#region Fields
		NativeLayoutOptions nativeLayoutOptions;
		NativeChartText nativeChartText;
		#endregion
		public NativeDataLabel(Model.DataLabel modelDataLabel, NativeWorkbook nativeWorkbook)
			: base(modelDataLabel, nativeWorkbook) { 
		}
		Model.DataLabel ModelDataLabel { get { return ModelDataLabelBase as Model.DataLabel; } }
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeLayoutOptions != null)
				nativeLayoutOptions.IsValid = value;
			if (nativeChartText != null)
				nativeChartText.IsValid = value;
		}
		#region DataLabel Members
		public ChartText Text {
			get {
				CheckValid();
				if (nativeChartText == null)
					nativeChartText = new NativeChartText(ModelDataLabel);
				return nativeChartText;
			}
		}
		public int Index {
			get {
				CheckValid();
				return ModelDataLabel.ItemIndex;
			}
		}
		public LayoutOptions Layout {
			get {
				CheckValid();
				if (nativeLayoutOptions == null)
					nativeLayoutOptions = new NativeLayoutOptions(ModelDataLabel.Layout);
				return nativeLayoutOptions;
			}
		}
		#endregion
	}
	#endregion
	#region NativeDataLabelOptions
	partial class NativeDataLabelOptions : NativeDataLabelBase, DataLabelOptions {
		#region Fields
		readonly Model.DataLabels modelOptions;
		NativeChartLineOptions leaderLines;
		#endregion
		public NativeDataLabelOptions(Model.DataLabels modelOptions, NativeWorkbook nativeWorkbook)
			: base(modelOptions, nativeWorkbook) {
			this.modelOptions = modelOptions;
		}
		#region DataLabelOptions Members
		public ChartLineOptions LeaderLines {
			get {
				CheckValid();
				if (leaderLines == null)
					leaderLines = new NativeChartLineOptions(new LeaderLinesAdapter(modelOptions), NativeWorkbook);
				return leaderLines;
			}
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (leaderLines != null)
				leaderLines.IsValid = value;
		}
	}
	#endregion
	#region NativeDataLabelCollection
	partial class NativeDataLabelCollection : NativeChartIndexedCollectionBase<DataLabel, NativeDataLabel, Model.DataLabel, Model.DataLabelCollection>, DataLabelCollection {
		#region Fields
		readonly Model.DataLabels modelDataLabels;
		NativeChartLineOptions nativeChartLineOptions;
		NativeDataLabelBase nativeDataLabelBase;
		#endregion
		public NativeDataLabelCollection(Model.DataLabels modelDataLabels, NativeWorkbook nativeWorkbook)
			: base(modelDataLabels.Labels, nativeWorkbook) {
			this.modelDataLabels = modelDataLabels;
		}
		#region NativeChartUndoableCollectionBase<DataLabel, NativeDataLabel, Model.DataLabel, Model.DataLabelCollection> Members
		protected override NativeDataLabel CreateNativeObject(Model.DataLabel modelItem) {
			return new NativeDataLabel(modelItem, NativeWorkbook);
		}
		protected override Model.DataLabel CreateModelObject(int index) {
			return new Model.DataLabel(ModelChartCollection.Parent, index);
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeChartLineOptions != null)
				nativeChartLineOptions.IsValid = value;
			if (nativeDataLabelBase != null)
				nativeDataLabelBase.IsValid = value;
		}
		#region DataLabelCollection Members
		public ChartLineOptions LeaderLines {
			get {
				CheckValid();
				if (nativeChartLineOptions == null)
					nativeChartLineOptions = new NativeChartLineOptions(new LeaderLinesAdapter(modelDataLabels), NativeWorkbook);
				return nativeChartLineOptions;
			}
		}
		public bool Hidden {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.Hidden; 
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.Hidden = value; 
			}
		}
		public bool ShowBubbleSize {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.ShowBubbleSize; 
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.ShowBubbleSize = value; 
			}
		}
		public bool ShowCategoryName {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.ShowCategoryName;
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.ShowCategoryName = value;
			}
		}
		public bool ShowSeriesName {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.ShowSeriesName;
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.ShowSeriesName = value;
			}
		}
		public bool ShowLegendKey {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.ShowLegendKey;
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.ShowLegendKey = value;
			}
		}
		public bool ShowPercent {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.ShowPercent;
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.ShowPercent = value;
			}
		}
		public bool ShowValue {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.ShowValue;
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.ShowValue = value;
			}
		}
		public string Separator {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.Separator;
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.Separator = value;
			}
		}
		public DataLabelPosition LabelPosition {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.LabelPosition;
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.LabelPosition = value;
			}
		}
		public NumberFormatOptions NumberFormat {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.NumberFormat;
			}
		}
		public ShapeFill Fill {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.Fill;
			}
		}
		public ShapeOutline Outline {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.Outline;
			}
		}
		public void ResetToMatchStyle() {
			CreateNativeDataLabelBase();
			nativeDataLabelBase.ResetToMatchStyle();
		}
		public ShapeTextFont Font {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.Font;
			}
		}
		public int TextRotation {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.TextRotation;
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.TextRotation = value;
			}
		}
		public ShapeTextDirection TextDirection {
			get {
				CreateNativeDataLabelBase();
				return nativeDataLabelBase.TextDirection;
			}
			set {
				CreateNativeDataLabelBase();
				nativeDataLabelBase.TextDirection = value;
			}
		}
		#endregion
		#region Internal
		void CreateNativeDataLabelBase() {
			CheckValid();
			if (nativeDataLabelBase == null)
				nativeDataLabelBase = new NativeDataLabelBase(modelDataLabels, NativeWorkbook);
		}
		#endregion
	}
	#endregion
}
