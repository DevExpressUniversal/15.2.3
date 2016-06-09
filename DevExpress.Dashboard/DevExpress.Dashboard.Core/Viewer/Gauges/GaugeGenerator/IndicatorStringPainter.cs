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

using DevExpress.XtraGauges.Core.Drawing;
using System;
using DevExpress.DashboardCommon.Data;
using DevExpress.XtraGauges.Core.Model;
using System.Drawing;
namespace DevExpress.DashboardCommon.Viewer {
	class IndicatorStringPainter : Utils_StringPainter, IStringPainter, IDisposable {
		public static string IndicatorTypeToString(IndicatorType deltaIndicatorType, bool deltaIsGood) {
			int value = (int)deltaIndicatorType * 10;
			if(deltaIndicatorType != IndicatorType.None && deltaIsGood)
				value++;
			return value.ToString();
		}
		public static void IndicatorTypeFromString(string imageName, out IndicatorType deltaIndicatorType, out bool deltaIsGood) {
			int value;
			if(!Int32.TryParse(imageName, out value)) {
				deltaIndicatorType = IndicatorType.None;
				deltaIsGood = default(bool);
				return;
			}
			else {
				deltaIndicatorType = (IndicatorType)((value % 100 - value % 10) / 10);
				deltaIsGood = value % 10 == 1;
			}
		}
		IndicatorPresenter.CustomShader shader;
		BaseShape up, down, warning;
		public IndicatorStringPainter(DeltaColorsGetter deltaColorsGetter) {
			shader = new IndicatorPresenter.CustomShader(deltaColorsGetter.Good, deltaColorsGetter.Bad, deltaColorsGetter.Warning);
			up = StateIndicatorShapesFactory.GetIndicatorStateShape(StateIndicatorShapeType.DashboardTrendArrowUp);
			down = StateIndicatorShapesFactory.GetIndicatorStateShape(StateIndicatorShapeType.DashboardTrendArrowDown);
			warning = StateIndicatorShapesFactory.GetIndicatorStateShape(StateIndicatorShapeType.DashboardWarning);
		}
		public void SetGraphics(Graphics g, Brush brush) {
			this.StringBrush = brush;
		}
		public void Dispose() {
			this.StringBrush = null;
			if(shader != null) {
				shader.Dispose();
				shader = null;
			}
			if(up != null) {
				up.Dispose();
				up = null;
			}
			if(down != null) {
				down.Dispose();
				down = null;
			}
			if(warning != null) {
				warning.Dispose();
				warning = null;
			}
		}
		Utils_StringInfo IStringPainter.Calculate(Graphics graphics, BaseTextAppearance appearance, string text, Rectangle bounds) {
			return Calculate(new Utils_StringCalculateArgs(graphics, text, bounds, this) { Appearance = appearance });
		}
		protected override void DrawStringBlock(Graphics g, object context, StringFormat format, Utils.Text.Internal.StringBlock sb, Rectangle rect, Brush brush) {
			if(sb.Type == Utils.Text.Internal.StringBlockType.Image) {
				BaseShape shape = GetShape(sb.ImageName);
				if(shape == null)
					return;
				float proportion = Math.Max(sb.Size.Width, sb.Size.Height) / Math.Max(shape.Bounds.Width, shape.Bounds.Height);
				shape.Transform = new System.Drawing.Drawing2D.Matrix(proportion, 0, 0, proportion, rect.X, rect.Y);
				using(new ShapeTextPainterHelper(g, shape)) {
					shape.Render(g, this);
				}
			}
			else {
				base.DrawStringBlock(g, context, format, sb, rect, brush);
			}
		}
		BaseShape GetShape(string imageName) {
			IndicatorType deltaIndicatorType;
			bool isGood;
			IndicatorTypeFromString(imageName, out deltaIndicatorType, out isGood);
			shader.ResultType = deltaIndicatorType == IndicatorType.Warning ? ResultType.Warning : (isGood ? ResultType.Good : ResultType.Bad);
			BaseShape shape;
			switch(deltaIndicatorType) {
				case IndicatorType.DownArrow:
					shape = down;
					break;
				case IndicatorType.UpArrow:
					shape = up;
					break;
				case IndicatorType.Warning:
					shape = warning;
					break;
				default:
					return null;
			}
			shape.Accept(shader);
			return shape;
		}
	}
}
