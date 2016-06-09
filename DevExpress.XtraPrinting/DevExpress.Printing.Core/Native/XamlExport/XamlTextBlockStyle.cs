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
#if SL
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
#else
using System.Drawing;
#endif
namespace DevExpress.XtraPrinting.XamlExport {
	public class XamlTextBlockStyle : XamlResourceBase {
		Font font;
		TextAlignment textAlignment;
		bool wrapText;
		Color foreground;
		StringTrimming stringTrimming;
		public Font Font { get { return font; } }
		public TextAlignment TextAlignment { get { return textAlignment; } }
		public bool WrapText { get { return wrapText; } }
		public Color Foreground { get { return foreground; } }
		public StringTrimming StringTrimming { get { return stringTrimming; } }
		public XamlTextBlockStyle(Font font, TextAlignment textAlignment, bool wrapText, Color foreground, StringTrimming stringTrimming) {
			this.font = font;
			this.textAlignment = textAlignment;
			this.wrapText = wrapText;
			this.foreground = foreground;
			this.stringTrimming = stringTrimming;
		}
		public override bool Equals(object obj) {
			XamlTextBlockStyle style = obj as XamlTextBlockStyle;
			if(style == null)
				return false;
			return (style.TextAlignment == textAlignment && style.WrapText == wrapText &&
				style.Foreground == foreground && style.Font.Size == font.Size &&
				style.Font.Bold == font.Bold && FontHelper.GetFamilyName(style.Font) == FontHelper.GetFamilyName(font) && 
				style.Font.Italic == font.Italic && style.Font.Strikeout == font.Strikeout && 
				style.Font.Underline == font.Underline && style.Font.Unit == font.Unit &&
				style.stringTrimming == stringTrimming);
		}
		public override int GetHashCode() {
			return HashCodeHelper.CalcHashCode(font.GetHashCode(), (int)textAlignment, foreground.GetHashCode(), stringTrimming.GetHashCode());
		}
	}
}
