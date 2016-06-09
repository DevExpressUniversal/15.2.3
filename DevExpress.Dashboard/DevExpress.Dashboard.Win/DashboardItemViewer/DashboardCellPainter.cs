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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.DashboardWin.Native {
	public static class DashboardCellPainter {
		public static void DrawContentWithBar(TextEditViewInfo viewInfo, StringFormat format, GraphicsCache cache, BarStyleSettingsInfo barInfo, Rectangle cellBounds) {
			decimal normalizedValue = barInfo.NormalizedValue;
			decimal zeroPosition = barInfo.ZeroPosition;
			bool isNegativeNumber = Convert.ToDecimal(viewInfo.EditValue) < 0;
			if(!barInfo.Color.IsEmpty) {
				Rectangle barBounds = BarBoundsCalculator.CalculateBounds(barInfo, cellBounds, viewInfo.EditValue);
				using(AppearanceObject appearance = new AppearanceObject()) {
					appearance.BackColor = appearance.BackColor2 = barInfo.Color;
					appearance.FillRectangle(cache, barBounds);
				}
			}
			if(barInfo.DrawAxis) {
				Point topPoint;
				Point bottomPoint;
				BarBoundsCalculator.CalculateAxisPoints(barInfo, cellBounds, out topPoint, out bottomPoint);
				cache.Graphics.DrawLine(new Pen(new SolidBrush(DashboardWinHelper.GetBarAxisColor(viewInfo.LookAndFeel))), topPoint, bottomPoint);
			}
			if(!barInfo.ShowBarOnly)
				viewInfo.PaintAppearance.DrawString(cache, viewInfo.DisplayText, viewInfo.MaskBoxRect, format);
		}
		public static void DrawContentWithIcon(TextEditViewInfo viewInfo, GraphicsCache cache, StringFormat format, StyleSettingsInfo styleSettings, Rectangle cellBounds) {
			AppearanceObject cellAppearance = viewInfo.PaintAppearance;
			int leftPadding = viewInfo.MaskBoxRect.X - cellBounds.X;
			int rightPadding = cellBounds.Right - viewInfo.MaskBoxRect.Right;
			format.Trimming = StringTrimming.EllipsisCharacter;
			Rectangle imageBounds = CellContentArranger.GetImageBounds(cellBounds, styleSettings.Image, cellAppearance.HAlignment, leftPadding, rightPadding, true);
			Rectangle textBounds = CellContentArranger.GetTextBounds(imageBounds, cellBounds, cellAppearance.HAlignment, leftPadding, rightPadding, true);
			cache.Paint.DrawImage(cache.Graphics, styleSettings.Image, imageBounds);
			cellAppearance.DrawString(cache, viewInfo.DisplayText, textBounds, format);
		}
	}
}
