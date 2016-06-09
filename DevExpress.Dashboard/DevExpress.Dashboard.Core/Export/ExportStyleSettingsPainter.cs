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
using DevExpress.DashboardCommon.Viewer;
using DevExpress.PivotGrid.Printing;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DashboardExport {
	public static class ExportImageStyleSettingsPainter {
		public static IVisualBrick CreatePanelBrick(IPivotPrintAppearance appearance, Rectangle bounds, string formatString, string text, object value, StyleSettingsInfo styleSettings) {
			PanelBrick panel = new PanelBrick() { BackColor = styleSettings.BackColor, Rect = bounds };
			Rectangle imageBounds = CellContentArranger.GetImageBounds(bounds, styleSettings.Image, appearance.TextHorizontalAlignment, CellSizeProviderBase.DefaultLeftCellPadding, CellSizeProviderBase.DefaultRightCellPadding, false);
			ImageBrick imageBrick = new ImageBrick() {
				Image = styleSettings.Image,
				Rect = imageBounds,
				Sides = BorderSide.None,
				Padding = new PaddingInfo(0, 0, 0, 0),
				BackColor = styleSettings.BackColor,
				SizeMode = XtraPrinting.ImageSizeMode.StretchImage
			};
			panel.Bricks.Add(imageBrick);
			Rectangle textBounds = CellContentArranger.GetTextBounds(imageBounds, bounds, appearance.TextHorizontalAlignment, CellSizeProviderBase.DefaultLeftCellPadding, CellSizeProviderBase.DefaultRightCellPadding, false);
			TextBrick textBrick = new TextBrick() {
				Text = text,
				TextValue = value,
				Font = styleSettings.Font,
				ForeColor = styleSettings.ForeColor,
				Padding = new PaddingInfo(0, 0, 0, 0),
				Rect = textBounds,
				Sides = BorderSide.None,
				BackColor = styleSettings.BackColor,
				StringFormat = BrickStringFormat.Create(TextAlignmentConverter.ToTextAlignment(appearance.TextHorizontalAlignment, appearance.TextVerticalAlignment), false, StringTrimming.EllipsisWord),
				TextValueFormatString = formatString
			};
			panel.Bricks.Add(textBrick);
			return panel;
		}
	}
}
