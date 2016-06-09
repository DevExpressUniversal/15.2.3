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
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Viewer {
	public static class CellContentArranger {
		public static Rectangle GetImageBounds(Rectangle cellBounds, Image image, HorzAlignment textHAlignment, int leftPadding, int rightPadding, bool absoluteCoord) {
			if(cellBounds.Height < image.Height || cellBounds.Width < image.Width + leftPadding + rightPadding)
				return Rectangle.Empty;
			int imageXCoord = textHAlignment == HorzAlignment.Far ? leftPadding : cellBounds.Width - rightPadding - image.Width;
			if(absoluteCoord)
				imageXCoord += cellBounds.X;
			int imageYCoord = absoluteCoord ? cellBounds.Y : 0;
			if(cellBounds.Height > image.Height)
				imageYCoord += (cellBounds.Height - image.Height) / 2;
			return new Rectangle(imageXCoord, imageYCoord, image.Width, image.Height);
		}
		public static Rectangle GetTextBounds(Rectangle imageBounds, Rectangle cellBounds, HorzAlignment textHAlignment, int leftPadding, int rightPadding, bool absoluteCoord) {
			int imageWidhtPaddingWidth = imageBounds.IsEmpty ? 0 : imageBounds.Width + leftPadding;
			int textXCoord = textHAlignment == HorzAlignment.Far ? imageWidhtPaddingWidth + leftPadding : leftPadding;
			if(absoluteCoord)
				textXCoord += cellBounds.X;
			int textWidth = cellBounds.Width - leftPadding - rightPadding - imageWidhtPaddingWidth;
			textWidth = Math.Max(0, textWidth);
			return new Rectangle(textXCoord, absoluteCoord ? cellBounds.Y : 0, textWidth, cellBounds.Height);
		}
	}
}
