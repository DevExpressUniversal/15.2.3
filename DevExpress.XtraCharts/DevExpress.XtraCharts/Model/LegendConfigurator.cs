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

using System.Collections.Generic;
using DevExpress.Charts.Model;
using DevExpress.XtraCharts.Native;
using Model = DevExpress.Charts.Model;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public class LegendConfigurator {
		static readonly Dictionary<LegendPosition, LegendAlignmentHorizontal> hotzAlignmentDict = new Dictionary<LegendPosition, LegendAlignmentHorizontal>();
		static readonly Dictionary<LegendPosition, LegendAlignmentVertical> vertAlignmentDict = new Dictionary<LegendPosition, LegendAlignmentVertical>();
		static LegendConfigurator() {
			hotzAlignmentDict[LegendPosition.Left] = LegendAlignmentHorizontal.Left;
			hotzAlignmentDict[LegendPosition.Top] = LegendAlignmentHorizontal.Center;
			hotzAlignmentDict[LegendPosition.Right] = LegendAlignmentHorizontal.Right;
			hotzAlignmentDict[LegendPosition.Bottom] = LegendAlignmentHorizontal.Center;
			hotzAlignmentDict[LegendPosition.TopRight] = LegendAlignmentHorizontal.Right;
			vertAlignmentDict[LegendPosition.Left] = LegendAlignmentVertical.Center;
			vertAlignmentDict[LegendPosition.Top] = LegendAlignmentVertical.Top;
			vertAlignmentDict[LegendPosition.Right] = LegendAlignmentVertical.Center;
			vertAlignmentDict[LegendPosition.Bottom] = LegendAlignmentVertical.Bottom;
			vertAlignmentDict[LegendPosition.TopRight] = LegendAlignmentVertical.Top;
		}
		public void Configure(Legend legend, Model.Legend modelLegend) {
			legend.Visibility = DefaultBooleanUtils.ToDefaultBoolean(modelLegend != null);
			if(modelLegend == null) return;
			legend.EnableAntialiasing = modelLegend.EnableAntialiasing;
			legend.AlignmentHorizontal = CalculateHorizontalAlignment(modelLegend.LegendPosition, modelLegend.Overlay);
			legend.AlignmentVertical = CalculateVerticalAlignment(modelLegend.LegendPosition, modelLegend.Overlay);
			legend.Direction = modelLegend.Orientation == Model.LegendOrientation.Vertical ? LegendDirection.TopToBottom : LegendDirection.LeftToRight;
			Model.Border border = modelLegend.Border;
			legend.Border.Visibility = DevExpress.XtraCharts.Native.DefaultBooleanUtils.ToDefaultBoolean(border != null);
			if (border != null) {
				if (border.Color != Model.ColorARGB.Empty)
					legend.Border.Color = ModelConfigaratorHelper.ToColor(border.Color);
				legend.Border.Thickness = border.Thickness;
			}
		}
		internal LegendAlignmentHorizontal CalculateHorizontalAlignment(LegendPosition pos, bool overlay) {
			LegendAlignmentHorizontal align = LegendAlignmentHorizontal.Right;
			hotzAlignmentDict.TryGetValue(pos, out align);
			if(!overlay) {
				if(align == LegendAlignmentHorizontal.Left)
					align = LegendAlignmentHorizontal.LeftOutside;
				if(align == LegendAlignmentHorizontal.Right)
					align = LegendAlignmentHorizontal.RightOutside;
			}
			return align;
		}
		internal LegendAlignmentVertical CalculateVerticalAlignment(LegendPosition pos, bool overlay) {
			LegendAlignmentVertical align = LegendAlignmentVertical.Top;
			vertAlignmentDict.TryGetValue(pos, out align);
			if(!overlay) {
				if(align == LegendAlignmentVertical.Top)
					align = LegendAlignmentVertical.TopOutside;
				if(align == LegendAlignmentVertical.Bottom)
					align = LegendAlignmentVertical.BottomOutside;
			}
			return align;
		}
	}
}
