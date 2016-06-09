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

using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region SpreadsheetCommentPainter (abstract class)
	public abstract class SpreadsheetCommentPainter {
		#region Fields
		const int borderWidthInPixels = 1;
		static StringFormat stringFormat = CreateStringFormat();
		readonly SpreadsheetControl control;
		#endregion
		protected SpreadsheetCommentPainter(SpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		static StringFormat CreateStringFormat() {
			StringFormat result = (StringFormat)StringFormat.GenericDefault.Clone();
			result.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip;
			result.Trimming = StringTrimming.None;
			return result;
		}
		public void Draw(GraphicsCache cache, CommentBox box) {
			if (!box.CanDraw())
				return;
			using (Pen pen = new Pen(Color.Black, 1)) {
				DrawCommentLine(cache, box, pen);
				DrawComment(cache, box, pen);
			}
		}
		void DrawComment(GraphicsCache cache, CommentBox box, Pen borderPen) {
			Rectangle bounds = box.Bounds;
			if (bounds.IsEmpty)
				return;
			cache.FillRectangle(box.FillColor, bounds);
			cache.DrawRectangle(borderPen, bounds);
			DrawText(cache, box.GetNormalizedPlainText(), box.TextRuns, box, bounds);
		}
		void DrawText(GraphicsCache cache, string text, CommentRunCollection textRuns, CommentBox box, Rectangle bounds) {
			Rectangle textBounds = Rectangle.Inflate(bounds, -3, -1);
			textBounds.Width += 1;
			DevExpress.Utils.Text.TextUtils.DrawString(cache.Graphics, text, GetActualFont(Comment.DefaultFont), Comment.DefaultForeColor, textBounds, Rectangle.Empty, stringFormat);
		}
		Font GetActualFont(Font font) {
			float fontSize = control.DocumentModel.LayoutUnitConverter.PixelsToLayoutUnitsF(font.Size, DocumentModel.Dpi);
			return new Font(font.FontFamily, fontSize, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
		}
		void DrawCommentLine(GraphicsCache cache, CommentBox box, Pen linePen) {
			Graphics graphics = cache.Graphics;
			SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			try {
				cache.Graphics.DrawLine(linePen, box.GetIndicatorLineStartPoint(), box.GetIndicatorLineEndPoint());
			}
			finally {
				graphics.SmoothingMode = oldSmoothingMode;
			}
		}
	}
	#endregion
	#region SpreadsheetCommentFlatPainter
	public class SpreadsheetCommentFlatPainter : SpreadsheetCommentPainter {
		public SpreadsheetCommentFlatPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetCommentUltraFlatPainter
	public class SpreadsheetCommentUltraFlatPainter : SpreadsheetCommentPainter {
		public SpreadsheetCommentUltraFlatPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetCommentStyle3DPainter
	public class SpreadsheetCommentStyle3DPainter : SpreadsheetCommentPainter {
		public SpreadsheetCommentStyle3DPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetCommentOffice2003Painter
	public class SpreadsheetCommentOffice2003Painter : SpreadsheetCommentPainter {
		public SpreadsheetCommentOffice2003Painter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetCommentWindowsXPPainter
	public class SpreadsheetCommentWindowsXPPainter : SpreadsheetCommentPainter {
		public SpreadsheetCommentWindowsXPPainter(SpreadsheetControl control)
			: base(control) {
		}
	}
	#endregion
	#region SpreadsheetCommentSkinPainter
	public class SpreadsheetCommentSkinPainter : SpreadsheetCommentPainter {
		#region Fields
		UserLookAndFeel lookAndFeel;
		#endregion
		public SpreadsheetCommentSkinPainter(SpreadsheetControl control, UserLookAndFeel lookAndFeel)
			: base(control) {
			this.lookAndFeel = lookAndFeel;
		}
		#region Properties
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		#endregion
	}
	#endregion
}
