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

using System.Windows;
using System.Drawing;
using System;
#if SL
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class ContentAlignmentHelper {
		public static VerticalAlignment VerticalAlignmentFromContentAlignment(ContentAlignment alignment) {
			int align = (int)alignment;
			VerticalAlignment result;
			if((align & 0x7) != 0) {
				result = VerticalAlignment.Top;
			} else if((align & 0x70) != 0) {
				result = VerticalAlignment.Center;
			} else if((align & 0x700) != 0) {
				result = VerticalAlignment.Bottom;
			} else {
				throw new InvalidOperationException();
			}
			return result;
		}
		public static HorizontalAlignment HorizontalAlignmentFromContentAlignment(ContentAlignment alignment) {
			int align = (int)alignment;
			HorizontalAlignment result;
			if((align & 0x111) != 0) {
				result = HorizontalAlignment.Left;
			} else if((align & 0x222) != 0) {
				result = HorizontalAlignment.Center;
			} else if((align & 0x444) != 0) {
				result = HorizontalAlignment.Right;
			} else {
				throw new InvalidOperationException();
			}
			return result;
		}
		public static ContentAlignment ContentAlignmentFromAlignments(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment) {
			ContentAlignment result;
			if(verticalAlignment == VerticalAlignment.Top) {
				switch(horizontalAlignment) {
					case HorizontalAlignment.Left:
						result = ContentAlignment.TopLeft;
						break;
					case HorizontalAlignment.Center:
						result = ContentAlignment.TopCenter;
						break;
					case HorizontalAlignment.Right:
						result = ContentAlignment.TopRight;
						break;
					default:
						throw new InvalidOperationException();
				}
			} else if(verticalAlignment == VerticalAlignment.Center) {
				switch(horizontalAlignment) {
					case HorizontalAlignment.Left:
						result = ContentAlignment.MiddleLeft;
						break;
					case HorizontalAlignment.Center:
						result = ContentAlignment.MiddleCenter;
						break;
					case HorizontalAlignment.Right:
						result = ContentAlignment.MiddleRight;
						break;
					default:
						throw new InvalidOperationException();
				}
			} else if(verticalAlignment == VerticalAlignment.Bottom) {
				switch(horizontalAlignment) {
					case HorizontalAlignment.Left:
						result = ContentAlignment.BottomLeft;
						break;
					case HorizontalAlignment.Center:
						result = ContentAlignment.BottomCenter;
						break;
					case HorizontalAlignment.Right:
						result = ContentAlignment.BottomRight;
						break;
					default:
						throw new InvalidOperationException();
				}
			} else {
				throw new InvalidOperationException();
			}
			return result;
		}
	}
}
