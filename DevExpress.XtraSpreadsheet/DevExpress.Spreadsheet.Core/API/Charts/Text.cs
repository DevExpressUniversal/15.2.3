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
	public enum ChartTextType {
		Auto = DevExpress.XtraSpreadsheet.Model.ChartTextType.Auto,
		Reference = DevExpress.XtraSpreadsheet.Model.ChartTextType.Ref,
		Value = DevExpress.XtraSpreadsheet.Model.ChartTextType.Value,
	}
	public interface ChartText {
		ChartTextType TextType { get; }
		string PlainText { get; }
		string[] Lines { get; }
		void SetAuto();
		void SetReference(string expression);
		void SetReference(Range range);
		void SetValue(string value);
	}
	public interface ChartTitleOptions : ChartText, ShapeFormat, ShapeTextFormat {
		bool Visible { get; set; }
		LayoutOptions Layout { get; }
	}
	public interface ChartTitle : ChartTitleOptions {
		bool Overlay { get; set; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	using DevExpress.XtraSpreadsheet.Localization;
	#region NativeChartText
	partial class NativeChartText : NativeObjectBase, ChartText {
		readonly Model.IChartTextOwner modelOwner;
		public NativeChartText(Model.IChartTextOwner modelOwner) {
			this.modelOwner = modelOwner;
		}
		Model.IChart Parent { get { return modelOwner.Parent; } }
		Model.DocumentModel DocumentModel { get { return Parent.DocumentModel; } }
		#region ChartText Members
		#region TextType
		public ChartTextType TextType {
			get {
				CheckValid();
				switch(modelOwner.Text.TextType) {
					case Model.ChartTextType.Ref:
						return ChartTextType.Reference;
					case Model.ChartTextType.Rich:
					case Model.ChartTextType.Value:
						return ChartTextType.Value;
				}
				return ChartTextType.Auto;
			}
		}
		#endregion
		#region PlainText
		public string PlainText {
			get {
				CheckValid();
				return modelOwner.Text.PlainText;
			}
		}
		#endregion
		#region Lines
		public string[] Lines {
			get {
				CheckValid();
				return modelOwner.Text.Lines;
			}
		}
		#endregion
		#region SetChartText
		public void SetAuto() {
			CheckValid();
			modelOwner.Text = Model.ChartText.Auto;
		}
		public void SetReference(string expression) {
			CheckValid();
			DocumentModel.BeginUpdate();
			try {
				Model.ChartTextRef textRef = new Model.ChartTextRef(Parent);
				textRef.FormulaBody = expression;
				modelOwner.Text = textRef;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void SetReference(Range range) {
			CheckValid();
			if (range == null)
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidRange));
			NativeRange nativeRange = range.GetRangeWithAbsoluteReference() as NativeRange;
			if (nativeRange == null)
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidRange));
			DocumentModel.BeginUpdate();
			try {
				Model.ChartTextRef textRef = new Model.ChartTextRef(Parent);
				textRef.SetRange(nativeRange.ModelRange);
				modelOwner.Text = textRef;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void SetValue(string value) {
			CheckValid();
			DocumentModel.BeginUpdate();
			try {
				Model.IChartTextOwnerEx modelOwnerEx = modelOwner as Model.IChartTextOwnerEx;
				if (modelOwnerEx != null) {
					Model.ChartRichText richText = new Model.ChartRichText(Parent);
					richText.PlainText = value;
					modelOwner.Text = richText;
				}
				else {
					Model.ChartTextValue textLiteral = new Model.ChartTextValue(Parent);
					textLiteral.PlainText = value;
					modelOwner.Text = textLiteral;
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#endregion
	}
	#endregion
	#region NativeChartTitleOptions
	partial class NativeChartTitleOptions : NativeShapeTextFormat, ChartTitleOptions {
		#region Fields
		readonly Model.TitleOptions modelTitle;
		readonly NativeWorkbook nativeWorkbook;
		NativeChartText nativeChartText;
		NativeShapeFormat nativeShapeFormat;
		NativeLayoutOptions nativeLayout;
		#endregion
		public NativeChartTitleOptions(Model.TitleOptions modelTitle, NativeWorkbook nativeWorkbook) :
			base(modelTitle) {
			this.modelTitle = modelTitle;
			this.nativeWorkbook = nativeWorkbook;
		}
		protected Model.TitleOptions ModelTitle { get { return modelTitle; } }
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeChartText != null)
				nativeChartText.IsValid = value;
			if (nativeShapeFormat != null)
				nativeShapeFormat.IsValid = value;
			if (nativeLayout != null)
				nativeLayout.IsValid = value;
		}
		#region ChartTitleOptions Members
		#region Layout
		public LayoutOptions Layout {
			get {
				CheckValid();
				if (nativeLayout == null)
					nativeLayout = new NativeLayoutOptions(modelTitle.Layout);
				return nativeLayout;
			}
		}
		#endregion
		#region Visible
		public bool Visible {
			get {
				CheckValid();
				return modelTitle.Text.TextType != Model.ChartTextType.None;
			}
			set {
				if (Visible != value)
					SetVisibleCore(value);
			}
		}
		#endregion
		#region ChartText Members
		#region TextType
		public ChartTextType TextType {
			get {
				CheckValid();
				CheckChartText();
				return nativeChartText.TextType;
			}
		}
		#endregion
		#region PlainText
		public string PlainText {
			get {
				CheckValid();
				CheckChartText();
				return nativeChartText.PlainText;
			}
		}
		#endregion
		#region Lines
		public string[] Lines {
			get {
				CheckValid();
				CheckChartText();
				return nativeChartText.Lines;
			}
		}
		#endregion
		#region SetChartText
		public void SetAuto() {
			CheckValid();
			CheckChartText();
			SetAutoCore();
		}
		public void SetReference(string expression) {
			CheckValid();
			CheckChartText();
			SetReferenceCore(expression);
		}
		public void SetReference(Range range) {
			CheckValid();
			CheckChartText();
			SetReferenceCore(range);
		}
		public void SetValue(string value) {
			CheckValid();
			CheckChartText();
			SetValueCore(value);
		}
		#endregion
		#endregion
		#region ShapeFormat Members
		public ShapeFill Fill {
			get {
				CheckValid();
				CheckShapeFormat();
				return nativeShapeFormat.Fill;
			}
		}
		public ShapeOutline Outline {
			get {
				CheckValid();
				CheckShapeFormat();
				return nativeShapeFormat.Outline;
			}
		}
		public void ResetToMatchStyle() {
			CheckValid();
			ModelTitle.DocumentModel.BeginUpdate();
			try {
				ModelTitle.ResetToStyle();
			}
			finally {
				ModelTitle.DocumentModel.EndUpdate();
			}
		}
		#endregion
		#endregion
		#region Internal
		void CheckShapeFormat() {
			if (nativeShapeFormat == null)
				nativeShapeFormat = new NativeShapeFormat(modelTitle.ShapeProperties, nativeWorkbook);
		}
		void CheckChartText() {
			if (nativeChartText == null)
				nativeChartText = new NativeChartText(modelTitle);
		}
		protected virtual void SetVisibleCore(bool value) {
			modelTitle.Text = value ? Model.ChartText.Auto : Model.ChartText.Empty;
		}
		protected virtual void SetAutoCore() {
			nativeChartText.SetAuto();
		}
		protected virtual void SetReferenceCore(string expression) {
			nativeChartText.SetReference(expression);
		}
		protected virtual void SetReferenceCore(Range range) {
			nativeChartText.SetReference(range);
		}
		protected virtual void SetValueCore(string value) {
			nativeChartText.SetValue(value);
		}
		#endregion
	}
	#endregion
	#region NativeChartTitle
	partial class NativeChartTitle : NativeChartTitleOptions, ChartTitle {
		public NativeChartTitle(Model.TitleOptions modelTitle, NativeWorkbook nativeWorkbook)
			: base(modelTitle, nativeWorkbook) { }
		Model.Chart ModelChart { get { return ModelTitle.Parent as Model.Chart; } }
		Model.DocumentModel DocumentModel { get { return ModelTitle.Parent.DocumentModel; } }
		#region Overlay
		public bool Overlay {
			get {
				CheckValid();
				return ModelTitle.Overlay;
			}
			set {
				CheckValid();
				ModelTitle.Overlay = value;
			}
		}
		#endregion
		protected override void SetAutoCore() {
			DocumentModel.BeginUpdate();
			try {
				base.SetAutoCore();
				ModelChart.AutoTitleDeleted = false;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override void SetReferenceCore(string expression) {
			DocumentModel.BeginUpdate();
			try {
				base.SetReferenceCore(expression);
				ModelChart.AutoTitleDeleted = false;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override void SetReferenceCore(Range range) {
			DocumentModel.BeginUpdate();
			try {
				base.SetReferenceCore(range);
				ModelChart.AutoTitleDeleted = false;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override void SetValueCore(string value) {
			DocumentModel.BeginUpdate();
			try {
				if (string.IsNullOrEmpty(value)) {
					base.SetAutoCore();
					base.SetVisibleCore(false);
					ModelChart.AutoTitleDeleted = true;
				}
				else {
					base.SetValueCore(value);
					ModelChart.AutoTitleDeleted = false;
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected override void SetVisibleCore(bool value) {
			DocumentModel.BeginUpdate();
			try {
				base.SetVisibleCore(value);
				ModelChart.AutoTitleDeleted = !value;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
}
