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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
#if !SL
using System.Drawing.Drawing2D;
#else
using System.Windows.Media;
using DevExpress.XtraPrinting.Stubs;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Drawing.Drawing2D;
using Brush = System.Windows.Media.Brush;
#endif
namespace DevExpress.Office.Drawing {
	#region GdiPlusPainterBase (abstract class)
	public abstract class GdiPlusPainterBase : Painter {
		StringFormat sf;
		protected static StringFormat CreateStringFormat() {
			StringFormat result = (StringFormat)StringFormat.GenericTypographic.Clone();
			result.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces;
			result.FormatFlags &= ~StringFormatFlags.LineLimit;
			return result;
		}
		protected GdiPlusPainterBase() {
			this.sf = CreateStringFormat();
		}
		public StringFormat StringFormat { get { return sf; } }
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (sf != null) {
						sf.Dispose();
						sf = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public override void DrawSpacesString(string text, FontInfo fontInfo, Rectangle bounds) {
			DrawString(text, fontInfo, bounds);
		}
		protected Rectangle CorrectTextDrawingBounds(FontInfo fontInfo, Rectangle textBounds) {
			Rectangle correctedTextBounds = textBounds;
			correctedTextBounds.Y += fontInfo.Free;
			return correctedTextBounds;
		}
	}
	#endregion
}
