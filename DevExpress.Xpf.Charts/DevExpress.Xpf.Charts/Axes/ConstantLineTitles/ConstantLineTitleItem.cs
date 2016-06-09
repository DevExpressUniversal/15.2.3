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
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class ConstantLineTitleItem : AxisElementTitleItem {
		readonly ConstantLineItem constantLineItem;
		ConstantLineTitle ConstantLineTitle { get { return (ConstantLineTitle)Title; } }
		Axis2D Axis { get { return (Axis2D)Title.Axis; } }
		ConstantLine ConstantLine { get { return constantLineItem.ConstantLine; } }
		internal bool Visible { get { return Title.ActualVisible && ConstantLine.GetActualVisible() && constantLineItem.IsVisibleForLayout; } }
		internal ConstantLineTitleItem(ConstantLineTitle constantLineTitle, ConstantLineItem constantLineItem) : base(constantLineTitle) {
			this.constantLineItem = constantLineItem;
		}
		internal Rect CalculateTitleRect(Size constraint) {
			AxisElementTitleBase title = Title;
			if (!Visible)
				return RectExtensions.Zero;
			Axis axis = Axis;
			Size size = Size;
			double startLocationOffset = constantLineItem.Offset;
			double xLocation, yLocation;
			if (axis.IsVertical) {
				xLocation = CalculateTitleAlignmentOffset(size.Width, constraint.Width);
				yLocation = constraint.Height - startLocationOffset - 1 + CalcOppositeLineOffset(size.Height);
			}
			else {
				xLocation = startLocationOffset + CalcOppositeLineOffset(size.Width);
				yLocation = CalculateTitleAlignmentOffset(size.Height, constraint.Height);
			}
			return new Rect(MathUtils.StrongRound(xLocation), MathUtils.StrongRound(yLocation), size.Width, size.Height);
		}
		double CalculateTitleAlignmentOffset(double titleSize, double availableSize) {
			ConstantLineTitleAlignment alignment = ConstantLineTitle.Alignment;
			bool isVertical = Axis.IsVertical;
			return (((isVertical && alignment == ConstantLineTitleAlignment.Far) || (!isVertical && alignment == ConstantLineTitleAlignment.Near))) ? (availableSize - titleSize - 2.0) : 2.0;
		}
		double CalcOppositeLineOffset(double titleSize) {
			double halfThickness = ConstantLine.ActualLineStyle.Thickness / 2.0;
			return (Axis.Reverse ^ ConstantLineTitle.ShowBelowLine) ? (halfThickness + 1) : (1 - titleSize - halfThickness);
		}
	}
}
