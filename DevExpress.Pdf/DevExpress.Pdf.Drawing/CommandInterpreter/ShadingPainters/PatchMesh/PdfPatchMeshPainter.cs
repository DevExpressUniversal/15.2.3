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

using System.Security;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfPatchMeshPainter : PdfMeshShadingPainter {
		static int colorComponents = 4;
		readonly int bitmapWidth;
		readonly int bitmapHeight;
		readonly IList<PdfRenderingPatch> patches;
		protected PdfPatchMeshPainter(PdfMeshShading shading, bool shouldDrawBackground, bool shouldUseTransparentBackgroundColor, PdfViewerCommandInterpreter interpreter, PdfTransformationMatrix matrix, int bitmapWidth, int bitmapHeight) 
				: base(shading, shouldDrawBackground, shouldUseTransparentBackgroundColor, interpreter, matrix, bitmapWidth, bitmapHeight) {
			this.bitmapWidth = bitmapWidth + 1;
			this.bitmapHeight = bitmapHeight + 1;
			patches = CreatePatches(shading);
		} 
		[SecuritySafeCritical]
		protected override void DrawMesh(Graphics graphics) { 
			byte[] bitmapData = new byte[colorComponents * bitmapWidth * bitmapHeight];
			foreach (PdfRenderingPatch patch in patches) { 
				double du = 1 /  (patch.Width * 2);
				double dv = 1 /  (patch.Height * 2);
				for (double v = 0; v <= 1; v += dv)
					for (double u = 0; u <= 1; u += du) {
						Point point = patch.CalculatePoint(u, v); 
						int x = point.X;
						int y = point.Y;
						if (x >= 0 && x < bitmapWidth && y >= 0 && y < bitmapHeight) {
							Color color = patch.CalculateColor(u, v);
							int position = (x + y * bitmapWidth) * colorComponents; 
							bitmapData[position++] = color.B;
							bitmapData[position++] = color.G;
							bitmapData[position++] = color.R;
							bitmapData[position] = 255;
						}
					}
			}
			GCHandle dataHandle = GCHandle.Alloc(bitmapData, GCHandleType.Pinned);
			try { 
				using (Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight, colorComponents * bitmapWidth, PixelFormat.Format32bppArgb, dataHandle.AddrOfPinnedObject())) 
					graphics.DrawImage(bitmap, new Point(0, 0));
			}
			finally {
				if (dataHandle.IsAllocated)
					dataHandle.Free();
			};	
		}
		protected abstract IList<PdfRenderingPatch> CreatePatches(PdfMeshShading shading);
	}
}
