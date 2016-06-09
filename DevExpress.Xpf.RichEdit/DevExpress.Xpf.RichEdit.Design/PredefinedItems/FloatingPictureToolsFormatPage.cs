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

extern alias Platform;
using System;
using Platform::DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.RichEdit.Design {
#if !SL
	[System.Diagnostics.CodeAnalysis.SuppressMessage("DevExpress.Design", "DCA0002")]
#endif
	public static partial class BarInfos {
		#region PictureToolsShapeStyles
		public static BarInfo PictureToolsShapeStyles { get { return pictureToolsShapeStyles; } }
		static readonly BarInfo pictureToolsShapeStyles = new BarInfo(
			"Picture Tools",
			"Format",
			"Shape Styles",
			new BarInfoItems(
				new string[] { "PictureShapeFillColor", "PictureShapeOutlineColor" },
				new BarItemInfo[] { BarItemInfos.BackColorSplitButton, BarItemInfos.BackColorSplitButton }
			),
			String.Empty,
			"Caption_PageCategoryFloatingObjectPictureTools",
			"Caption_PageFloatingObjectPictureToolsFormat",
			"Caption_GroupFloatingPictureToolsShapeStyles",
			"ToolsFloatingPictureCommandGroup"
		);
		#endregion
		#region PictureToolsArrange
		public static BarInfo PictureToolsArrange { get { return pictureToolsArrange; } }
		static readonly BarInfo pictureToolsArrange = new BarInfo(
			"Picture Tools",
			"Format",
			"Arrange",
			new BarInfoItems(
				new string[] { "PictureWrapText", "PicturePosition", "PictureBringForwardPlaceholder", "PictureSendBackwardPlaceholder" },
				new BarItemInfo[] {
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PictureWrapTextSquare", "PictureWrapTextTight", "PictureWrapTextThrough", "PictureWrapTextTopAndBottom", "PictureWrapTextBehind", "PictureWrapTextInFrontOf" },
						new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check, BarItemInfos.Check })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PictureTopLeftAlignment", "PictureTopCenterAlignment", "PictureTopRightAlignment", "PictureMiddleLeftAlignment", "PictureMiddleCenterAlignment", "PictureMiddleRightAlignment", "PictureBottomLeftAlignment", "PictureBottomCenterAlignment", "PictureBottomRightAlignment" },
						new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PictureBringForward", "PictureBringToFront", "PictureBringInFrontOfText" },
						new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button })
					),
					new BarSubItemInfo(new BarInfoItems(
						new string[] { "PictureSendBackward", "PictureSendToBack", "PictureSendBehindText" },
						new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button})
					)
				}
			),
			String.Empty,
			"Caption_PageCategoryFloatingObjectPictureTools",
			"Caption_PageFloatingObjectPictureToolsFormat",
			"Caption_GroupFloatingPictureToolsArrange",
			"ToolsFloatingPictureCommandGroup"
		);
		#endregion
	}
}
