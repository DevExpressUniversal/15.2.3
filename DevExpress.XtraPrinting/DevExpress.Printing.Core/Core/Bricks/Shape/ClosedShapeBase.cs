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

using DevExpress.XtraPrinting.Native;
using System;
using System.Drawing;
using DevExpress.XtraPrinting.Shape.Native;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.Shape {
	public abstract class ClosedShapeBase : ShapeBase {
		protected virtual int GetFilletValueInPercents() {
			return 100;
		}
		protected internal abstract PointF[] CreatePoints(RectangleF bounds, int angle);
		protected abstract ILinesAdjuster GetLinesAdjuster();
		protected internal override ShapeCommandCollection CreateCommands(RectangleF bounds, int angle) {
			ShapePathCommandCollection pathCommands = new ShapePathCommandCollection();
			ShapeFillPathCommand fillPathCommand = new ShapeFillPathCommand(CreatePointsCommands(bounds, angle));
			System.Diagnostics.Debug.Assert(fillPathCommand.IsClosed);
			pathCommands.Add(fillPathCommand);
			ShapeDrawPathCommand drawPathCommand = new ShapeDrawPathCommand(CreatePointsCommands(bounds, angle));
			System.Diagnostics.Debug.Assert(drawPathCommand.IsClosed);
			pathCommands.Add(drawPathCommand);
			return pathCommands;
		}
		protected override RectangleF AdjustClientRectangle(IGraphics gr, RectangleF clientBounds, float lineWidth) {
			clientBounds = DeflateRectangleHalfLineWidth(clientBounds, lineWidth);
			float delta = GraphicsUnitConverter.Convert(1f, gr.Dpi, GraphicsDpi.Document);
			float width = Math.Max(0f, clientBounds.Width - delta);
			float height = Math.Max(0f, clientBounds.Height - delta);
			return new RectangleF(clientBounds.X, clientBounds.Y, width, height);
		}
		static RectangleF DeflateRectangleHalfLineWidth(RectangleF clientBounds, float lineWidth) {
			int widht = (int)Math.Round(lineWidth);
			RectangleF bounds = clientBounds;
			bounds.X += widht / 2;
			bounds.Y += widht / 2;
			bounds.Width -= widht;
			bounds.Height -= widht;
			return bounds;
		}
		ShapePointsCommandCollection CreatePointsCommands(RectangleF bounds, int angle) {
			return ConsecutiveLineCommandsRounder.CreateRoundedConsecutiveLinesCommands(ShapeHelper.CreateConsecutiveLinesFromPoints(CreatePoints(bounds, angle)), GetFilletValueInPercents(), GetLinesAdjuster());
		}
	}
}
