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
#if SL
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
#else
#endif
namespace DevExpress.XtraPrinting.Native {	
	public class BrickStyleHelper {
		public static BrickStyle ChangeSides(BrickStyle source, BorderSide sides) {
			BrickStyle val = (BrickStyle)source.Clone();
			val.Sides = sides;
			return val;
		}
		public static BrickStyle ChangeBorderWidth(BrickStyle source, float width) {
			BrickStyle val = (BrickStyle)source.Clone();
			val.BorderWidth = width;
			return val;
		}
		public static BrickStyle ChangeBorderDashStyle(BrickStyle source, BorderDashStyle borderDashStyle) {
			BrickStyle val = (BrickStyle)source.Clone();
			val.BorderDashStyle = borderDashStyle;
			return val;
		}
		public static BrickStyle ChangeBorderColor(BrickStyle source, Color color) {
			BrickStyle val = (BrickStyle)source.Clone();
			val.BorderColor = color;
			return val;
		}
		public static BrickStyle ChangeBackColor(BrickStyle source, Color color) {
			BrickStyle val = (BrickStyle)source.Clone();
			val.BackColor = color;
			return val;
		}
		public static BrickStyle ChangeForeColor(BrickStyle source, Color color) {
			BrickStyle val = (BrickStyle)source.Clone();
			val.ForeColor = color;
			return val;
		}
		public static BrickStyle ChangeFont(BrickStyle source, Font font) {
			BrickStyle val = (BrickStyle)source.Clone();
			val.Font = font;
			return val;
		}
		public static BrickStyle ChangeStringFormat(BrickStyle source, BrickStringFormat sf) {
			BrickStyle val = (BrickStyle)source.Clone();
			val.StringFormat = sf;
			val.TextAlignment = TextAlignmentConverter.ToTextAlignment(sf.Alignment, sf.LineAlignment); 
			return val;
		}
		public static BrickStyle ChangePadding(BrickStyle source, PaddingInfo padding) {
			BrickStyle val = (BrickStyle)source.Clone();
			val.Padding = padding;
			return val;
		}
		public static BrickStyle ChangeBorderStyle(BrickStyle source, BrickBorderStyle borderStyle) {
			BrickStyle val = (BrickStyle)source.Clone();
			val.BorderStyle = borderStyle;
			return val;
		}
	}
}
