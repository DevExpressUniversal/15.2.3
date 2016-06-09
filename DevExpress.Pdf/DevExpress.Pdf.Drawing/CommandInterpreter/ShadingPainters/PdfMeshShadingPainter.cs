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
using System.Drawing.Imaging;
using System.Collections.Generic;
namespace DevExpress.Pdf.Drawing {		
	public abstract class PdfMeshShadingPainter : PdfShadingPainter {
		const int colorPaletteSize = 256;
		readonly ColorMap[] colorMaps;
		readonly int bitmapWidth;
		readonly int bitmapHeight;
		readonly PdfMeshShading shading;
		protected PdfMeshShadingPainter(PdfMeshShading shading, bool shouldDrawBackground, bool shouldUseTransparentBackgroundColor, PdfViewerCommandInterpreter interpreter, PdfTransformationMatrix matrix, int bitmapWidth, int bitmapHeight) 
				: base(shading, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight) { 
			this.shading = shading;
			this.bitmapWidth = bitmapWidth;
			this.bitmapHeight = bitmapHeight;
			IList<PdfCustomFunction> functions = shading.Function;
			if (functions != null) { 
				PdfCustomFunction function = functions[0];
				if (function != null) { 
					PdfRange functionDomain = function.Domain[0];
					if (functionDomain != null) {
						double domainMin = functionDomain.Min;
						double domainSize = functionDomain.Max - domainMin; 
						if (domainSize != 0) {
							ColorConverter = new PdfFunctionDomainConverter(functionDomain);
							colorMaps = new ColorMap[colorPaletteSize];
							for (int i = 0; i < colorPaletteSize; i++) { 
								ColorMap colorMap = new ColorMap();
								colorMap.OldColor = Color.FromArgb(i, i, i);
								colorMap.NewColor = ConvertColor(new double[] { domainMin + i * domainSize / 255 });
								colorMaps[i] = colorMap;
							}
						}
					}
				}
			}
		} 
		protected override void Paint(Graphics graphics) { 
			base.Paint(graphics);
			GraphicsState graphicsState = graphics.Save();
			try { 
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.SmoothingMode = SmoothingMode.Default;
				if (shading.Function == null)
					DrawMesh(graphics);
				else 
					using (Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight)) { 
						using (Graphics newGraphics = Graphics.FromImage(bitmap)) 
							DrawMesh(newGraphics);
						if (colorMaps != null) { 
							ImageAttributes imageAttributes = new ImageAttributes();
							imageAttributes.SetRemapTable(colorMaps);
							graphics.DrawImage(bitmap, new PointF[] { new PointF(0, 0), new PointF(bitmapWidth, 0), new PointF(0, bitmapHeight) }, new RectangleF(0, 0, bitmapWidth, bitmapHeight), 
								GraphicsUnit.Pixel, imageAttributes);
						}
					}
			}
			finally { 
				graphics.Restore(graphicsState);
			}
		}
		protected abstract void DrawMesh(Graphics graphics);
	}
}
