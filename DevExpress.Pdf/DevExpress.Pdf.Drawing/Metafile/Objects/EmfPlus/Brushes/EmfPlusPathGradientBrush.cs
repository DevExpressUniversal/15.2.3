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

using System.Drawing;
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusPathGradientBrush : EmfPlusBrush {
		public EmfPlusPathGradientBrush(EmfPlusReader reader) {
			EmfPlusBrushData pathGradientFlags = (EmfPlusBrushData)reader.ReadInt32();
			WrapMode pathGradientWrapMode = (WrapMode)reader.ReadInt32();
			Color centerColor = reader.ReadColor();
			PointF centerPoint = reader.ReadPointF(false);
			int colorsCount = reader.ReadInt32();
			Color[] surroundigColors = new Color[colorsCount];
			for (int i = 0; i < colorsCount; i++)
				surroundigColors[i] = reader.ReadColor();
			PdfPathGradientBrush pathGradientBrush = null;
			if (pathGradientFlags.HasFlag(EmfPlusBrushData.BrushDataPath)) {
				reader.ReadInt32();
				EmfPlusPath path = new EmfPlusPath(reader);
				using (GraphicsPath graphicsPath = new GraphicsPath(path.Points, path.Types))
					pathGradientBrush = new PdfPathGradientBrush(graphicsPath);
			}
			else {
				int pointsCount = reader.ReadInt32();
				PointF[] surroundingPoints = new PointF[pointsCount];
				for (int i = 0; i < pointsCount; i++)
					surroundingPoints[i] = reader.ReadPointF(false);
				pathGradientBrush = new PdfPathGradientBrush(surroundingPoints);
			}
			if (pathGradientFlags.HasFlag(EmfPlusBrushData.BrushDataTransform))
				pathGradientBrush.Transform = reader.ReadTransformMatrix();
			if (pathGradientFlags.HasFlag(EmfPlusBrushData.BrushDataBlendFactors)) 
				pathGradientBrush.Blend = reader.ReadBlend();
			if (pathGradientFlags.HasFlag(EmfPlusBrushData.BrushDataPresetColors))
				pathGradientBrush.InterpolationColors = reader.ReadColorBlend();
			if (pathGradientFlags.HasFlag(EmfPlusBrushData.BrushDataFocusScales)) {
				reader.ReadInt32();
				double xFocusScale = reader.ReadSingle();
				double yFocusScale = reader.ReadSingle();
				pathGradientBrush.FocusScales = new PdfPoint(xFocusScale, yFocusScale); 
			}
			pathGradientBrush.WrapMode = pathGradientWrapMode;
			pathGradientBrush.SurroundColors = surroundigColors;
			pathGradientBrush.CenterColor = centerColor;
			pathGradientBrush.CenterPoint = new PdfPoint(centerPoint.X, centerPoint.Y);
			BrushContainer = new PdfPathGradientBrushContainer(pathGradientBrush);
		}
	}
}
