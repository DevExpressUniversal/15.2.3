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
using Model = DevExpress.Charts.Model;
using System.Windows.Controls;
namespace DevExpress.Xpf.Charts.ModelSupport {
	public class LegendConfigurator : ConfiguratorBase {
		static readonly Dictionary<Model.LegendPosition, HorizontalPosition> hotzAlignmentDict = new Dictionary<Model.LegendPosition, HorizontalPosition>();
		static readonly Dictionary<Model.LegendPosition, VerticalPosition> vertAlignmentDict = new Dictionary<Model.LegendPosition, VerticalPosition>();
		static LegendConfigurator() {
			hotzAlignmentDict[Model.LegendPosition.Left] = HorizontalPosition.Left;
			hotzAlignmentDict[Model.LegendPosition.Top] = HorizontalPosition.Center;
			hotzAlignmentDict[Model.LegendPosition.Right] = HorizontalPosition.Right;
			hotzAlignmentDict[Model.LegendPosition.Bottom] = HorizontalPosition.Center;
			hotzAlignmentDict[Model.LegendPosition.TopRight] = HorizontalPosition.Right;
			vertAlignmentDict[Model.LegendPosition.Left] = VerticalPosition.Center;
			vertAlignmentDict[Model.LegendPosition.Top] = VerticalPosition.Top;
			vertAlignmentDict[Model.LegendPosition.Right] = VerticalPosition.Center;
			vertAlignmentDict[Model.LegendPosition.Bottom] = VerticalPosition.Bottom;
			vertAlignmentDict[Model.LegendPosition.TopRight] = VerticalPosition.Top;
		}
		Model.Legend LegendModel { get { return ModelElement as Model.Legend; } }
		public LegendConfigurator(Model.Legend legendModel, Model.IModelElementContainer container) : base(legendModel, container) {
		}
		HorizontalPosition CalculateHorizontalAlignment(Model.LegendPosition position, bool overlay) {
			HorizontalPosition align = HorizontalPosition.Right;
			hotzAlignmentDict.TryGetValue(position, out align);
			if (!overlay) {
				if (align == HorizontalPosition.Left)
					align = HorizontalPosition.LeftOutside;
				if (align == HorizontalPosition.Right)
					align = HorizontalPosition.RightOutside;
			}
			return align;
		}
		VerticalPosition CalculateVerticalAlignment(Model.LegendPosition position, bool overlay) {
			VerticalPosition align = VerticalPosition.Top;
			vertAlignmentDict.TryGetValue(position, out align);
			if (!overlay) {
				if (align == VerticalPosition.Top)
					align = VerticalPosition.TopOutside;
				if (align == VerticalPosition.Bottom)
					align = VerticalPosition.BottomOutside;
			}
			return align;
		}
		public void Configure(Legend legend) {
			legend.HorizontalPosition = CalculateHorizontalAlignment(LegendModel.LegendPosition, LegendModel.Overlay);
			legend.VerticalPosition = CalculateVerticalAlignment(LegendModel.LegendPosition, LegendModel.Overlay);
			legend.Orientation = LegendModel.Orientation == Model.LegendOrientation.Vertical ? Orientation.Vertical : Orientation.Horizontal;
		}
	}
}
