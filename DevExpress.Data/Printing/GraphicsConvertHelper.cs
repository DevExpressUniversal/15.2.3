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
using System.Text;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
#if DXPORTABLE
using DevExpress.Xpf.Drawing;
#endif
#if SL
using DevExpress.Xpf.Windows.Forms;
#else
using System.Windows.Forms;
#endif
namespace DevExpress.XtraPrinting {
	public class GraphicsConvertHelper {
		public static StringAlignment ToHorzStringAlignment(TextAlignment align) {
			switch (align) {
				case TextAlignment.TopLeft:
				case TextAlignment.MiddleLeft:
				case TextAlignment.BottomLeft:
				case TextAlignment.TopJustify:
				case TextAlignment.MiddleJustify:
				case TextAlignment.BottomJustify:
					return StringAlignment.Near;
				case TextAlignment.TopCenter:
				case TextAlignment.MiddleCenter:
				case TextAlignment.BottomCenter:
					return StringAlignment.Center;
				case TextAlignment.TopRight:
				case TextAlignment.MiddleRight:
				case TextAlignment.BottomRight:
					return StringAlignment.Far;
			}
			throw new ArgumentException("align");
		}
		public static StringAlignment ToVertStringAlignment(TextAlignment align) {
			switch (align) {
				case TextAlignment.TopCenter:
				case TextAlignment.TopLeft:
				case TextAlignment.TopRight:
				case TextAlignment.TopJustify:
					return StringAlignment.Near;
				case TextAlignment.MiddleCenter:
				case TextAlignment.MiddleLeft:
				case TextAlignment.MiddleRight:
				case TextAlignment.MiddleJustify:
					return StringAlignment.Center;
				case TextAlignment.BottomCenter:
				case TextAlignment.BottomRight:
				case TextAlignment.BottomLeft:
				case TextAlignment.BottomJustify:
					return StringAlignment.Far;
			}
			throw new ArgumentException("align");
		}
		public static BrickAlignment ToHorzBrickAlignment(ImageAlignment align, ImageSizeMode sizeMode) {
			switch (align) {
				case ImageAlignment.BottomLeft:
				case ImageAlignment.MiddleLeft:
				case ImageAlignment.TopLeft:
					return BrickAlignment.Near;
				case ImageAlignment.BottomCenter:
				case ImageAlignment.MiddleCenter:
				case ImageAlignment.TopCenter:
					return BrickAlignment.Center;
				case ImageAlignment.BottomRight:
				case ImageAlignment.MiddleRight:
				case ImageAlignment.TopRight:
					return BrickAlignment.Far;
				case ImageAlignment.Default:
					return sizeMode == ImageSizeMode.CenterImage || sizeMode == ImageSizeMode.Squeeze || sizeMode == ImageSizeMode.ZoomImage ?
						BrickAlignment.Center : BrickAlignment.Near;
			}
			throw new ArgumentException("align");
		}
		public static BrickAlignment ToVertBrickAlignment(ImageAlignment align, ImageSizeMode sizeMode) {
			switch(align) {
				case ImageAlignment.BottomCenter:
				case ImageAlignment.BottomLeft:
				case ImageAlignment.BottomRight:
					return BrickAlignment.Far;
				case ImageAlignment.MiddleCenter:
				case ImageAlignment.MiddleLeft:
				case ImageAlignment.MiddleRight:
					return BrickAlignment.Center;
				case ImageAlignment.TopCenter:
				case ImageAlignment.TopLeft:
				case ImageAlignment.TopRight:
					return BrickAlignment.Near;
				case ImageAlignment.Default:
					return sizeMode == ImageSizeMode.CenterImage || sizeMode == ImageSizeMode.Squeeze || sizeMode == ImageSizeMode.ZoomImage ?
						BrickAlignment.Center : BrickAlignment.Near;
			}
			throw new ArgumentException("align");
		}
		public static TextAlignment RTLTextAlignment(TextAlignment align) {
			switch(align) {
				case TextAlignment.BottomLeft:
					return TextAlignment.BottomRight;
				case TextAlignment.BottomRight:
					return TextAlignment.BottomLeft;
				case TextAlignment.MiddleLeft:
					return TextAlignment.MiddleRight;
				case TextAlignment.MiddleRight:
					return TextAlignment.MiddleLeft;
				case TextAlignment.TopLeft:
					return TextAlignment.TopRight;
				case TextAlignment.TopRight:
					return TextAlignment.TopLeft;
				default:
					return align;
			}
		}
		public static ContentAlignment RTLContentAlignment(ContentAlignment contentAlignment) {
			switch(contentAlignment) {
				case ContentAlignment.BottomCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.TopCenter:
					return contentAlignment;
				case ContentAlignment.BottomLeft:
					return ContentAlignment.BottomRight;
				case ContentAlignment.BottomRight:
					return ContentAlignment.BottomLeft;
				case ContentAlignment.MiddleLeft:
					return ContentAlignment.MiddleRight;
				case ContentAlignment.MiddleRight:
					return ContentAlignment.MiddleLeft;
				case ContentAlignment.TopLeft:
					return ContentAlignment.TopRight;
				case ContentAlignment.TopRight:
					return ContentAlignment.TopLeft;
			}
			throw new ArgumentException("contentAlignment");
		} 
	}
}
