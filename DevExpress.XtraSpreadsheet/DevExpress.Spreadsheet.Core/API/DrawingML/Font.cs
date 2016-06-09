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

using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet.Drawings {
	public enum ShapeTextUnderlineType {
		None = DevExpress.Office.Drawing.DrawingTextUnderlineType.None,
		Words = DevExpress.Office.Drawing.DrawingTextUnderlineType.Words,
		Single = DevExpress.Office.Drawing.DrawingTextUnderlineType.Single,
		Double = DevExpress.Office.Drawing.DrawingTextUnderlineType.Double,
		Heavy = DevExpress.Office.Drawing.DrawingTextUnderlineType.Heavy,
		Dotted = DevExpress.Office.Drawing.DrawingTextUnderlineType.Dotted,
		HeavyDotted = DevExpress.Office.Drawing.DrawingTextUnderlineType.HeavyDotted,
		Dashed = DevExpress.Office.Drawing.DrawingTextUnderlineType.Dashed,
		HeavyDashed = DevExpress.Office.Drawing.DrawingTextUnderlineType.HeavyDashed,
		LongDashed = DevExpress.Office.Drawing.DrawingTextUnderlineType.LongDashed,
		HeavyLongDashed = DevExpress.Office.Drawing.DrawingTextUnderlineType.HeavyLongDashed,
		DotDash = DevExpress.Office.Drawing.DrawingTextUnderlineType.DotDash,
		HeavyDotDash = DevExpress.Office.Drawing.DrawingTextUnderlineType.HeavyDotDash,
		DotDotDash = DevExpress.Office.Drawing.DrawingTextUnderlineType.DotDotDash,
		HeavyDotDotDash = DevExpress.Office.Drawing.DrawingTextUnderlineType.HeavyDotDotDash,
		Wavy = DevExpress.Office.Drawing.DrawingTextUnderlineType.Wavy,
		HeavyWavy = DevExpress.Office.Drawing.DrawingTextUnderlineType.HeavyWavy,
		DoubleWavy = DevExpress.Office.Drawing.DrawingTextUnderlineType.DoubleWavy
	}
	public enum ShapeTextStrikeType {
		None = DevExpress.Office.Drawing.DrawingTextStrikeType.None,
		Single = DevExpress.Office.Drawing.DrawingTextStrikeType.Single,
		Double = DevExpress.Office.Drawing.DrawingTextStrikeType.Double
	}
	public enum ShapeTextCapsType {
		None = DevExpress.Office.Drawing.DrawingTextCapsType.None,
		Small = DevExpress.Office.Drawing.DrawingTextCapsType.Small,
		All = DevExpress.Office.Drawing.DrawingTextCapsType.All
	}
	public interface ShapeTextFont {
		bool Auto { get; }
		string Name { get; set; }
		double Size { get; set; }
		bool Bold { get; set; }
		bool Italic { get; set; }
		ScriptType Script { get; set; }
		ShapeTextUnderlineType UnderlineType { get; set; }
		ShapeTextStrikeType StrikeType { get; set; }
		ShapeTextCapsType CapsType { get; set; }
		Color Color { get; set; } 
		void SetAuto();
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Drawings;
	using DevExpress.Utils;
	using DevExpress.Export.Xl;
	using DevExpress.XtraSpreadsheet.Import.Xls;
	using DevExpress.Office.Drawing;
	partial class NativeShapeTextFont : NativeObjectBase, ShapeTextFont {
		readonly Model.TextProperties modelTextProperties;
		readonly Model.IChartTextOwnerEx modelTextOwner;
		public NativeShapeTextFont(Model.TextProperties modelTextProperties) {
			this.modelTextProperties = modelTextProperties;
		}
		public NativeShapeTextFont(Model.IChartTextOwnerEx modelTextOwner) {
			this.modelTextOwner = modelTextOwner;
		}
		Model.DocumentModel DocumentModel { 
			get {
				if (modelTextOwner != null)
					return modelTextOwner.Parent.DocumentModel;
				return modelTextProperties.DocumentModel; 
			} 
		}
		DrawingTextParagraphCollection Paragraphs {
			get {
				if (modelTextOwner != null) {
					if (modelTextOwner.Text.TextType == Model.ChartTextType.Rich)
						return ((Model.ChartRichText)modelTextOwner.Text).Paragraphs;
					return modelTextOwner.TextProperties.Paragraphs;
				}
				return modelTextProperties.Paragraphs;
			}
		}
		DrawingTextCharacterProperties ModelTextFont {
			get {
				if (Paragraphs.Count == 0)
					return null;
				if (IsRichText && Paragraphs[0].Runs.Count > 0)
					return Paragraphs[0].Runs[0].RunProperties;
				return Paragraphs[0].ParagraphProperties.DefaultCharacterProperties; 
			} 
		}
		IDrawingFill ModelFill { 
			get { return (ModelTextFont == null) ? null : ModelTextFont.Fill; } 
		}
		bool IsRichText {
			get {
				if (modelTextOwner != null)
					return modelTextOwner.Text.TextType == Model.ChartTextType.Rich;
				return false;
			}
		}
		#region ShapeTextFont Members
		#region Auto
		public bool Auto { 
			get {
				if (modelTextOwner != null) {
					if (modelTextOwner.Text.TextType == Model.ChartTextType.Rich)
						return false;
					return modelTextOwner.TextProperties.IsDefault;
				}
				return modelTextProperties.IsDefault; 
			} 
		}
		#endregion
		#region Name
		public string Name {
			get { 
				CheckValid();
				if (Auto)
					return string.Empty; 
				return ModelTextFont.Latin.Typeface;
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					CheckCustomFont();
					SetFontNameCore(value);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void SetFontNameCore(string value) {
			if (IsRichText) {
				foreach (DrawingTextParagraph paragraph in Paragraphs) {
					paragraph.ParagraphProperties.DefaultCharacterProperties.Latin.Typeface = value;
					foreach (IDrawingTextRun run in paragraph.Runs)
						run.RunProperties.Latin.Typeface = value;
				}
			}
			else
				ModelTextFont.Latin.Typeface = value;
		}
		#endregion
		#region Size
		public double Size {
			get {
				CheckValid();
				if (Auto)
					return 10.0; 
				return ConvertFontSizeToPoints(ModelTextFont.FontSize); 
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					CheckCustomFont();
					SetSizeCore(ConvertFontSizeToModelUnits(value));
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void SetSizeCore(int value) {
			if (IsRichText) {
				foreach (DrawingTextParagraph paragraph in Paragraphs) {
					paragraph.ParagraphProperties.DefaultCharacterProperties.FontSize = value;
					foreach (IDrawingTextRun run in paragraph.Runs)
						run.RunProperties.FontSize = value;
				}
			}
			else
				ModelTextFont.FontSize = value;
		}
		#endregion
		#region Bold
		public bool Bold {
			get {
				CheckValid();
				if (Auto)
					return false; 
				return ModelTextFont.Bold;
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					CheckCustomFont();
					SetBoldCore(value);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void SetBoldCore(bool value) {
			if (IsRichText) {
				foreach (DrawingTextParagraph paragraph in Paragraphs) {
					paragraph.ParagraphProperties.DefaultCharacterProperties.Bold = value;
					foreach (IDrawingTextRun run in paragraph.Runs)
						run.RunProperties.Bold = value;
				}
			}
			else
				ModelTextFont.Bold = value;
		}
		#endregion
		#region Italic
		public bool Italic {
			get {
				CheckValid();
				if (Auto)
					return false; 
				return ModelTextFont.Italic;
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					CheckCustomFont();
					SetItalicCore(value);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void SetItalicCore(bool value) {
			if (IsRichText) {
				foreach (DrawingTextParagraph paragraph in Paragraphs) {
					paragraph.ParagraphProperties.DefaultCharacterProperties.Italic = value;
					foreach (IDrawingTextRun run in paragraph.Runs)
						run.RunProperties.Italic = value;
				}
			}
			else
				ModelTextFont.Italic = value;
		}
		#endregion
		#region Script
		public ScriptType Script {
			get {
				CheckValid();
				if (Auto)
					return ScriptType.None; 
				return (ScriptType)XlsCharacterPropertiesHelper.ConvertBaselineToModelScriptType(ModelTextFont.Baseline);
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					CheckCustomFont();
					SetBaselineCore(XlsCharacterPropertiesHelper.ConvertModelScriptTypeToBaseline((XlScriptType)value));
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void SetBaselineCore(int value) {
			if (IsRichText) {
				foreach (DrawingTextParagraph paragraph in Paragraphs) {
					paragraph.ParagraphProperties.DefaultCharacterProperties.Baseline = value;
					foreach (IDrawingTextRun run in paragraph.Runs)
						run.RunProperties.Baseline = value;
				}
			}
			else
				ModelTextFont.Baseline = value;
		}
		#endregion
		#region UnderlineType
		public ShapeTextUnderlineType UnderlineType {
			get {
				CheckValid();
				if (Auto)
					return ShapeTextUnderlineType.None; 
				return (ShapeTextUnderlineType)ModelTextFont.Underline;
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					CheckCustomFont();
					SetUnderlineCore((DrawingTextUnderlineType)value);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void SetUnderlineCore(DrawingTextUnderlineType value) {
			if (IsRichText) {
				foreach (DrawingTextParagraph paragraph in Paragraphs) {
					paragraph.ParagraphProperties.DefaultCharacterProperties.Underline = value;
					foreach (IDrawingTextRun run in paragraph.Runs)
						run.RunProperties.Underline = value;
				}
			}
			else
				ModelTextFont.Underline = value;
		}
		#endregion
		#region StrikeType
		public ShapeTextStrikeType StrikeType {
			get {
				CheckValid();
				if (Auto)
					return ShapeTextStrikeType.None;
				return (ShapeTextStrikeType)ModelTextFont.Strikethrough;
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					CheckCustomFont();
					SetStrikethroughCore((DrawingTextStrikeType)value);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void SetStrikethroughCore(DrawingTextStrikeType value) {
			if (IsRichText) {
				foreach (DrawingTextParagraph paragraph in Paragraphs) {
					paragraph.ParagraphProperties.DefaultCharacterProperties.Strikethrough = value;
					foreach (IDrawingTextRun run in paragraph.Runs)
						run.RunProperties.Strikethrough = value;
				}
			}
			else
				ModelTextFont.Strikethrough = value;
		}
		#endregion
		#region CapsType
		public ShapeTextCapsType CapsType {
			get {
				CheckValid();
				if (Auto)
					return ShapeTextCapsType.None;
				return (ShapeTextCapsType)ModelTextFont.Caps;
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					CheckCustomFont();
					SetCapsCore((DrawingTextCapsType)value);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void SetCapsCore(DrawingTextCapsType value) {
			if (IsRichText) {
				foreach (DrawingTextParagraph paragraph in Paragraphs) {
					paragraph.ParagraphProperties.DefaultCharacterProperties.Caps = value;
					foreach (IDrawingTextRun run in paragraph.Runs)
						run.RunProperties.Caps = value;
				}
			}
			else
				ModelTextFont.Caps = value;
		}
		#endregion
		#region Color
		public Color Color {
			get {
				CheckValid();
				if (Auto)
					return DXColor.Black; 
				DrawingSolidFill fill = ModelFill as DrawingSolidFill;
				if (fill == null)
					return DXColor.Empty; 
				return fill.Color.FinalColor;
			}
			set {
				CheckValid();
				DocumentModel.BeginUpdate();
				try {
					CheckCustomFont();
					if (IsRichText)
						SetRichTextColor(value);
					else
						SetTextColor(ModelTextFont, value);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		void SetRichTextColor(Color value) {
			foreach (DrawingTextParagraph paragraph in Paragraphs) {
				SetTextColor(paragraph.ParagraphProperties.DefaultCharacterProperties, value);
				foreach (IDrawingTextRun run in paragraph.Runs)
					SetTextColor(run.RunProperties, value);
			}
		}
		void SetTextColor(DrawingTextCharacterProperties properties, Color value) {
			if (properties == null)
				return;
			DrawingSolidFill fill = properties.Fill as DrawingSolidFill;
			if (fill != null)
				fill.Color.OriginalColor.SetColorFromRGB(value);
			else {
				DrawingSolidFill solidFill = new DrawingSolidFill(DocumentModel);
				solidFill.Color.OriginalColor.SetColorFromRGB(value);
				properties.Fill = solidFill;
			}
		}
		#endregion
		#region SetAuto
		public void SetAuto() {
			DocumentModel.BeginUpdate();
			try {
				if (modelTextOwner != null) {
					if(modelTextOwner.Text.TextType != Model.ChartTextType.Rich)
						modelTextOwner.TextProperties.ResetToStyle();
				}
				else 
					modelTextProperties.ResetToStyle();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#endregion
		void CheckCustomFont() {
			if (Paragraphs.Count > 0)
				return;
			DrawingTextParagraph paragraph = new DrawingTextParagraph(DocumentModel);
			paragraph.ApplyParagraphProperties = true;
			paragraph.ParagraphProperties.ApplyDefaultCharacterProperties = true;
			Paragraphs.Add(paragraph);
		}
		double ConvertFontSizeToPoints(int value) {
			return value / 100.0;
		}
		int ConvertFontSizeToModelUnits(double value) {
			double result = value * 100.0;
			return result - (int)result >= 0.5 ? (int)result + 1 : (int)result;
		}
	}
}
