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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.Native {
	public static class FontMeasurer {
		static char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
		const float DefaultCharacterWidth = 5;
		public static float MeasureMaxDigitWidthF(Font fontInfo) {
			try {
				using(Graphics graphics = Graphics.FromHwnd(IntPtr.Zero)) {
					StringFormat measureStringFormat = CreateMeasureStringFormat();
					int count = digits.Length;
					float result = MeasureCharacterWidthF(graphics, digits[0], fontInfo, measureStringFormat);
					for(int i = 1; i < count; i++)
						result = Math.Max(result, MeasureCharacterWidthF(graphics, digits[i], fontInfo, measureStringFormat));
					return result;
				}
			}
			catch {
				return DefaultCharacterWidth;
			}
		}
		static float MeasureCharacterWidthF(Graphics g, char character, Font font, StringFormat measureStringFormat) {
			SizeF characterSize = g.MeasureString(new String(character, 1), font, int.MaxValue, measureStringFormat);
			return characterSize.Width;
		}
		static StringFormat CreateMeasureStringFormat() {
			StringFormat sf = (StringFormat)StringFormat.GenericTypographic.Clone();
			sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
			return sf;
		}
	}
}
