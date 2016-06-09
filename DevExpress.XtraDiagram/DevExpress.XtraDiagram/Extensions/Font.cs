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
using System.Linq;
using System.Text;
namespace DevExpress.XtraDiagram.Extensions {
	public static class FontExtensions {
		public static Font Update(this Font font, string fontFamily = null, double? fontSize = null, FontStyle? fontStyle = null) {
			return new Font(fontFamily ?? font.FontFamily.Name, (float)(fontSize ?? font.Size), fontStyle ?? font.Style);
		}
		public static FontStyle Update(this FontStyle fontStyle, bool? bold = null, bool? italic = null, bool? underlined = null, bool? strikeout = null) {
			if(bold != null)
				fontStyle = bold.Value ? (fontStyle | FontStyle.Bold) : (fontStyle & ~FontStyle.Bold);
			if(italic != null)
				fontStyle = italic.Value ? (fontStyle | FontStyle.Italic) : (fontStyle & ~FontStyle.Italic);
			if(underlined != null)
				fontStyle = underlined.Value ? (fontStyle | FontStyle.Underline) : (fontStyle & ~FontStyle.Underline);
			if(strikeout != null)
				fontStyle = strikeout.Value ? (fontStyle | FontStyle.Strikeout) : (fontStyle & ~FontStyle.Strikeout);
			return fontStyle;
		}
	}
}
