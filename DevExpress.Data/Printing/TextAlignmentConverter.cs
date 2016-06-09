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
using DevExpress.Utils;
using DevExpress.Data.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraPrinting.Native {
	public class TextAlignmentConverter {
		public static TextAlignment ToTextAlignment(StringAlignment alignment, StringAlignment lineAlignment) {
			if(lineAlignment == StringAlignment.Near && alignment == StringAlignment.Near) {
				return TextAlignment.TopLeft;
			} else if(lineAlignment == StringAlignment.Near && alignment == StringAlignment.Center) {
				return TextAlignment.TopCenter;
			} else if(lineAlignment == StringAlignment.Near && alignment == StringAlignment.Far) {
				return TextAlignment.TopRight;
			} else if(lineAlignment == StringAlignment.Center && alignment == StringAlignment.Near) {
				return TextAlignment.MiddleLeft;
			} else if(lineAlignment == StringAlignment.Center && alignment == StringAlignment.Center) {
				return TextAlignment.MiddleCenter;
			} else if(lineAlignment == StringAlignment.Center && alignment == StringAlignment.Far) {
				return TextAlignment.MiddleRight;
			} else if(lineAlignment == StringAlignment.Far && alignment == StringAlignment.Near) {
				return TextAlignment.BottomLeft;
			} else if(lineAlignment == StringAlignment.Far && alignment == StringAlignment.Center) {
				return TextAlignment.BottomCenter;
			} else if(lineAlignment == StringAlignment.Far && alignment == StringAlignment.Far) {
				return TextAlignment.BottomRight;
			}
			return TextAlignment.TopLeft;
		}
		public static TextAlignment ToTextAlignment(HorzAlignment alignment, VertAlignment lineAlignment) {
			return ToTextAlignment(
				AlignmentConverter.HorzAlignmentToStringAlignment(alignment),
				AlignmentConverter.VertAlignmentToStringAlignment(lineAlignment));
		}
	}
}
