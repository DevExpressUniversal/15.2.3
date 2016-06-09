#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Drawing;
using System;
namespace DevExpress.DashboardWin.Native {
	public static class WarningRenderer {
		const int DefaultMaxWidth = 450;
		const int DefaultHeightOffset = 0;
		readonly static Font font = new Font("Tahoma", 11);
		public static void MessageRenderer(PaintEventArgs e, Rectangle bounds, string text, UserLookAndFeel lookAndFeel, int maxWidth, int offsetHeight) {
			Graphics g = e.Graphics;
			Color color = lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin ? 
				CommonSkins.GetSkin(lookAndFeel).Colors[CommonColors.DisabledText] : CommonColors.GetSystemColor(CommonColors.DisabledText);
			using (Brush brush = new SolidBrush(color))
				using (StringFormat sf = new StringFormat(StringFormat.GenericDefault)) {
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;
					sf.Trimming = StringTrimming.EllipsisCharacter;
					using (XPaintMixed painter = new XPaintMixed())
						using (GraphicsCache cache = new GraphicsCache(g)) {
							bounds.Width -= 30;
							bounds.Height -= 30;
							bounds.X += 15;
							bounds.Y += 15;
							if(offsetHeight > 0) {
								Size messageSize = painter.CalcTextSizeInt(g, text, font, sf, maxWidth);
								int delta = Math.Max((bounds.Height - messageSize.Height) / 2, 0);
								bounds.Y -= Math.Min(offsetHeight / 2, delta);
							} 
							if (bounds.Width > maxWidth) {
								bounds.X += (bounds.Width - maxWidth) / 2;
								bounds.Width = maxWidth;
							}
							painter.DrawString(cache, text, font, brush, bounds, sf);
						}
				}
		}
		public static void MessageRenderer(PaintEventArgs e, Rectangle bounds, string text, UserLookAndFeel lookAndFeel, int maxWidth) {
			MessageRenderer(e, bounds, text, lookAndFeel, maxWidth, DefaultHeightOffset);
		}
		public static void MessageRenderer(PaintEventArgs e, Rectangle bounds, string text, UserLookAndFeel lookAndFeel) {
			MessageRenderer(e, bounds, text, lookAndFeel, DefaultMaxWidth, DefaultHeightOffset);
		}
	}
}
