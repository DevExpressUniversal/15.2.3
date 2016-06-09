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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Charts.Native {
	public class CustomShape {
		readonly int pixelWidth;
		readonly int pixelHeight;
		readonly int stride;
		readonly byte[] pixels;
		public bool IsEmpty { get { return pixels == null || pixels.Length == 0; } }
		public CustomShape(UIElement presenterElement) {
			RenderTargetBitmap bitmap = new RenderTargetBitmap((int)presenterElement.DesiredSize.Width, (int)presenterElement.DesiredSize.Height, 96, 96, PixelFormats.Pbgra32);
			presenterElement.Arrange(new Rect(0, 0, presenterElement.DesiredSize.Width, presenterElement.DesiredSize.Height)); 
			bitmap.Render(presenterElement);
			pixelWidth = (int)bitmap.Width;
			pixelHeight = (int)bitmap.Height;
			stride = pixelWidth * 4;
			pixels = new byte[pixelHeight * stride];
			bitmap.CopyPixels(pixels, stride, 0);
		}
		public byte? GetAlpha(int x, int y) {
			int index = (pixelHeight - y) * stride + x * 4 + 3;
			if (index < pixels.Length)
				return pixels[index];
			return null;
		}
		public void Clear() {
			for (int index = 0; index < stride * pixelHeight; index += 4) {
				pixels[index] = 0;
				pixels[index + 1] = 0;
				pixels[index + 2] = 0;
			}
		}
	}
}
