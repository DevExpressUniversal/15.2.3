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
using System.Drawing;
using DevExpress.Utils.Text;
namespace DevExpress.XtraCharts.Native {
	public class TextMeasurer : IDisposable {
		public const float TabStop = 20;
		readonly Bitmap bitmap;
		readonly Graphics graphics;
		public TextMeasurer() {
			bitmap = new Bitmap(100, 100);
			graphics = Graphics.FromImage(bitmap);
		}
		~TextMeasurer() {
			Dispose();
		}
		public void Dispose() {
			graphics.Dispose();
			bitmap.Dispose();
		}
		public SizeF MeasureString(string str, Font font) {
			using (StringFormat sf = new StringFormat()) {
				float[] tabs = { TabStop };
				sf.SetTabStops(0, tabs);
				return graphics.MeasureString(str, font, 0, sf);
			}
		}
		public SizeF MeasureString(string str, Font font, int width) {
			return graphics.MeasureString(str, font, width);
		}
		public SizeF MeasureString(string str, Font font, int width, StringAlignment alignment, StringAlignment lineAlignment, out int linesFilled) {
			using (StringFormat sf = new StringFormat()) {
				sf.Alignment = alignment;
				sf.LineAlignment = lineAlignment;
				float[] tabs = { TabStop };
				sf.SetTabStops(0, tabs);
				int charsFilled;
				return graphics.MeasureString(str, font, new SizeF(width, float.MaxValue), sf, out charsFilled, out linesFilled);
			}
		}
		public SizeF MeasureString(string str, Font font, StringAlignment alignment, StringAlignment lineAlignment) {
			using (StringFormat sf = new StringFormat()) {
				sf.Alignment = alignment;
				sf.LineAlignment = lineAlignment;
				return graphics.MeasureString(str, font, Int32.MaxValue, sf);
			}
		}
		public Size MeasureStringRounded(string text, Font font) {
			return Size.Round(MeasureString(text, font));
		}
		public SizeF MeasureStringTypographic(string str, Font font) {
			using (StringFormat sf = (StringFormat)StringFormat.GenericTypographic.Clone()) {
				sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
				return graphics.MeasureString(str, font, Int32.MaxValue, sf);
			}
		}
		public int GetFontAscentHeight(Font font) {
			try {
				return TextUtils.GetFontAscentHeight(graphics, font);
			}
			catch {
				return 0;
			}
		}
	}
}
