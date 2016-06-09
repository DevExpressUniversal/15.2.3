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
#else
using System.Drawing;
#endif
namespace DevExpress.XtraPrinting.XamlExport {
	public class XamlBorderStyle : XamlResourceBase {
		float borderWidth;
		Color borderBrush;
		BorderSide sides;
		Color backColor;
		PaddingInfo padding;
		public float BorderWidth { get { return borderWidth; } }
		public Color BorderBrush { get { return borderBrush; } }
		public BorderSide Sides { get { return sides; } }
		public Color BackColor { get { return backColor; } }
		public PaddingInfo Padding { get { return padding; } }
		public XamlBorderStyle(float borderWidth, Color borderBrush, BorderSide sides, Color backColor, PaddingInfo padding) {
			this.borderWidth = borderWidth;
			this.borderBrush = borderBrush;
			this.sides = sides;
			this.backColor = backColor;
			this.padding = padding;
		}
		public override bool Equals(object obj) {
			XamlBorderStyle style = obj as XamlBorderStyle;
			if(style == null)
				return false;
			return (style.BorderWidth == borderWidth && style.BorderBrush == borderBrush && style.Sides == sides && style.BackColor == backColor && style.Padding == padding);
		}
		public override int GetHashCode() {
			return HashCodeHelper.CalcHashCode((int)borderWidth, borderBrush.GetHashCode(), (int)sides, backColor.GetHashCode(), padding.GetHashCode());
		}
	}
}
