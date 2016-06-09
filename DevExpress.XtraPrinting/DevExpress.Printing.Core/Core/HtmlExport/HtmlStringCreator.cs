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
using System.Text;
using System.Drawing;
using DevExpress.XtraPrinting.Export.Web;
using System.Collections.Generic;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.HtmlExport.Native;
using DevExpress.XtraPrinting.HtmlExport;
namespace DevExpress.XtraPrinting.Native {
	public class HtmlStringCreator {
		readonly StringFormat stringFormat;
		float width;
		float height;
		bool RightToLeft { get { return stringFormat != null && (stringFormat.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0; } }
		public HtmlStringCreator(StringFormat stringFormat, float width, float height) {
			this.stringFormat = stringFormat;
			this.width = width;
			this.height = height;
		}
		DXWebControlBase[] ToHtmlControls(string[] strings) {
			List<DXWebControlBase> result = new List<DXWebControlBase>(strings.Length * 2 - 1);
			for(int i = 0; i < strings.Length; i++) {
				string str = strings[i];
				if(!string.IsNullOrEmpty(str)) {
					result.Add(new DXHtmlLiteralControl(NoLineBreakStart));
					result.Add(EncodeStringToHtmlControl(str));
					result.Add(new DXHtmlLiteralControl(NoLineBreakEnd));
				}
				if(i != strings.Length - 1) {
					result.Add(new DXHtmlLiteralControl(LineBreak));
					PostProcessLine(str, result);
				}
			}
			return result.ToArray();
		}
		protected virtual void PostProcessLine(string str, List<DXWebControlBase> result) {
		}
		DXHtmlLiteralControl EncodeStringToHtmlControl(string s) {
			if(s == null)
				return null;
			StringBuilder builder = new StringBuilder();
			foreach(char ch in s) {
				switch(ch) {
					case '&':
						builder.Append("&amp;");
						break;
					case '"':
						builder.Append("&quot;");
						break;
					case ' ':
						builder.Append(NBSP);
						break;
					case '<':
						builder.Append("&lt;");
						break;
					case '>':
						builder.Append("&gt;");
						break;
					default:
						if(ch >= 160 && ch < 256)
							builder.AppendFormat("&#{0};", ((int)ch).ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
						else
							builder.Append(ch);
						break;
				}
			}
			return new DXHtmlLiteralControl(builder.ToString());
		}
		public void AddHtmlControls(DXHtmlControl parent, string text, Measurer measurer, Font font) {
			string[] strings = SplitText(text, measurer, font, stringFormat);
			if(strings.Length == 0)
				return;
			int lineHeight = GetLineHeight(strings, measurer, font, stringFormat);
			SetParent(parent, ToHtmlControls(strings));
			ApplyAlignment(parent);
			ApplyLineAlignment(parent);
			parent.Style["line-height"] = HtmlConvert.ToHtml(lineHeight);
			if(RightToLeft)
				parent.Style["direction"] = "rtl";
			if(lineHeight - height > 1 && strings.Length == 1)
				HtmlHelper.SetClip(parent, Point.Empty, new Size((int)Math.Round(width), (int)Math.Round(height)), new Size((int)Math.Round(width), lineHeight));
		}
		string[] SplitText(string text, Measurer measurer, Font font, StringFormat stringFormat) {
			string[] strings;
			TextFormatter textFormatter = new TextFormatter(GraphicsUnit.Pixel, measurer);
			strings = textFormatter.FormatHtmlMultilineText(text, font, width, height, stringFormat, DesignateNewLines);
			return strings;
		}
		static void SetParent(DXHtmlControl parent, DXWebControlBase[] controls) {
			if(controls != null) {
				foreach(DXWebControlBase control in controls)
					parent.Controls.Add(control);
			}
		}
		static int GetLineHeight(string[] strings, Measurer measurer, Font font, StringFormat stringFormat) {
			float heightLogical = measurer.MeasureString(strings[0], font, GraphicsUnit.Pixel).Height;
			heightLogical = GraphicsUnitConverter.Convert(heightLogical, GraphicsDpi.Pixel, GraphicsDpi.DeviceIndependentPixel);
			int heightReal = (int)Math.Truncate(heightLogical) - 1;
			return heightReal;
		}
		protected virtual void ApplyAlignment(DXHtmlControl control) {
			if(stringFormat != null) {
				control.Style[DXHtmlTextWriterStyle.TextAlign] = HtmlConvert.ToHtmlAlign(RightToLeft ? RTLAlignment(stringFormat.Alignment) : stringFormat.Alignment);
			}
		}
		void ApplyLineAlignment(DXHtmlControl control) {
			if(stringFormat != null)
				control.Style[DXHtmlTextWriterStyle.VerticalAlign] = HtmlConvert.ToHtmlVAlign(stringFormat.LineAlignment);
		}
		static StringAlignment RTLAlignment(StringAlignment alignment) {
			switch(alignment) {
				case StringAlignment.Far:
					return StringAlignment.Near;
				case StringAlignment.Near:
					return StringAlignment.Far;
				default:
					return alignment;
			}
		}
		protected virtual string NBSP {
			get { return "&nbsp;"; }
		}
		protected virtual string LineBreak {
			get { return "<br/>"; }
		}
		protected virtual string NoLineBreakStart {
			get { return "<nobr>"; }
		}
		protected virtual string NoLineBreakEnd {
			get { return "</nobr>"; }
		}
		protected virtual bool DesignateNewLines {
			get { return false; }
		}
	}
	public class JustifyHtmlStringCreator : HtmlStringCreator {
		public JustifyHtmlStringCreator(StringFormat stringFormat, float width, float height)
			: base(stringFormat, width, height) {
		}
		protected override string NBSP {
			get { return " "; }
		}
		protected override void ApplyAlignment(DXHtmlControl control) {
			control.Style[DXHtmlTextWriterStyle.TextAlign] = "justify";
			control.Style["text-justify"] = "auto";
		}
		protected override string LineBreak {
			get { return " "; }
		}
		protected override bool DesignateNewLines {
			get { return true; }
		}
		protected override void PostProcessLine(string str, List<DXWebControlBase> result) {
			if(str == null)
				result.Add(new DXHtmlLiteralControl(base.LineBreak));
		}
	}
}
