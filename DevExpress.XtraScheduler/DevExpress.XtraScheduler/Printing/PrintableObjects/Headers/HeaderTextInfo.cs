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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	public class HeaderTextInfo : IPrintableObjectViewInfo, IDisposable, ICloneable {
		string text;
		Rectangle bounds;
		Font font;
		AppearanceObject appearance;
		public HeaderTextInfo(string text, Rectangle bounds, double scale, Font font, Color foreColor, float fontSizeMultiplier) {
			this.font = new Font(font.FontFamily, (float)(font.Size * scale * fontSizeMultiplier), font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
			Initialize(font, text, bounds, foreColor);
		}
		public HeaderTextInfo(Font font, string text, Rectangle bounds, Color foreColor) {
			Initialize(font, text, bounds, foreColor);
		}
		public string Text { get { return text; } }
		public Rectangle Bounds { get { return bounds; } }
		public AppearanceObject TextAppearance { get { return appearance; } }
		protected internal void Initialize(Font font, string text, Rectangle bounds, Color foreColor) {
			this.text = text;
			this.bounds = bounds;
			this.appearance = new AppearanceObject();
			this.appearance.Font = this.font;
			this.appearance.ForeColor = foreColor;
		}
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~HeaderTextInfo() {
			Dispose(false);
		}
		public virtual void Dispose(bool disposing) {
			if (disposing) {
				if (appearance != null) {
					appearance.Dispose();
					appearance = null;
				}
			}
		}
		#endregion
		#region IPrintableObjectViewInfo Members
		public virtual void Print(GraphicsInfoArgs gInfoArgs) {
			appearance.DrawString(gInfoArgs.Cache, text, bounds);
		}
		public virtual void Print(GraphicsCache cache) {
			appearance.DrawString(cache, text, bounds);
		}
		#endregion
		#region ICloneable Members
		object ICloneable.Clone() {
			return CloneCore();
		}
		public HeaderTextInfo Clone() {
			return CloneCore();
		}
		internal HeaderTextInfo CloneCore() {
			HeaderTextInfo info = new HeaderTextInfo(this.font, Text, Bounds, TextAppearance.ForeColor);
			info.TextAppearance.Assign(TextAppearance);
			return info;
		}
		#endregion
	}
}
