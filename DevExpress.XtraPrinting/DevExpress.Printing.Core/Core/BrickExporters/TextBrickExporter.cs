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
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using System;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class TextBrickExporter : VisualBrickExporter {
		internal static TextExportMode ToTextExportMode(DevExpress.Utils.DefaultBoolean value, TextExportMode defaultTextExportMode) {
			return value == DevExpress.Utils.DefaultBoolean.True ? TextExportMode.Value :
				value == DevExpress.Utils.DefaultBoolean.False ? TextExportMode.Text :
				defaultTextExportMode;
		}
		protected TextBrick TextBrick { get { return Brick as TextBrick; } }
		object TextValue { get { return TextBrick.TextValue; } }
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			if (clientRect.IsEmpty)
				return;
			Brush brush = gr.PrintingSystem.Gdi.GetBrush(TextBrick.ForeColor);
			SetStringFormatTabStops(Style, gr.Measurer);
			DrawText(gr, clientRect, Style.StringFormat.Value, brush);
		}
		protected virtual void SetStringFormatTabStops(BrickStyle style, Measurer measurer) {
			style.StringFormat.SetTabStops(style.CalculateTabStops(measurer));
		}
		protected virtual void DrawText(IGraphics gr, RectangleF clientRectangle, StringFormat sf, Brush brush) {
			if(Style.IsJustified)
				JustifiedStringPainter.DrawString(TextBrick.Text, gr, TextBrick.Font, brush, clientRectangle, sf, true);
			else
				gr.DrawString(Text, TextBrick.Font, brush, clientRectangle, sf);
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			TextLayoutBuilder layoutBuilder = exportProvider as TextLayoutBuilder;
			string text = layoutBuilder == null ? Text : layoutBuilder.GetText(Text, TextValue);
			if (shouldSplitText) {
				string[] lines = text.Split(new char[] { '\x000D' });
				for (int i = 0; i < lines.Length; i++) {
					lines[i] = lines[i].TrimStart(new char[] { '\x000A' });
				}
				if (lines.Length == 1)
					exportProvider.SetCellText(lines[0], null);
				else
					exportProvider.SetCellText(lines, null);
			} else
				exportProvider.SetCellText(text, null);
		}
		protected override void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
			exportProvider.SetCellText(Text, Url);
		}
		protected override void FillRtfTableCellCore(IRtfExportProvider exportProvider) {
			exportProvider.SetCellText(Text, Url);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			TextExportMode mode = GetRealTextExportMode(exportProvider);
			exportProvider.SetCellText(GetTextValue(mode), null);
		}
		TextExportMode GetRealTextExportMode(IXlsExportProvider exportProvider) {
			return ToTextExportMode(TextBrick.XlsExportNativeFormat, exportProvider.XlsExportContext.XlsExportOptions.TextExportMode);
		}
#if DEBUGTEST
		internal
#endif
		object GetTextValue(TextExportMode textExportMode) {
			if(textExportMode == TextExportMode.Text)
				return Text;
			if(TextValue == null)
				return TextBrick.ConvertHelper.GetNativeValue(Text);
			return GetValidValue(TextValue, Text);
		}
		static object GetValidValue(object value, string text) {
			if(value is DateTime || value is TimeSpan)
				return value;
			if(value is string && string.IsNullOrEmpty((string)value))
				return text;
			if(value is IConvertible)
				return ConvertHelper.ToCodeType((IConvertible)value, text);
			if(value != DBNull.Value && !value.GetType().IsPrimitive)
				return text;
			return value;
		}
	}
}
