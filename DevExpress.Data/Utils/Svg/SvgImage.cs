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
using System.Drawing;
using DevExpress.Utils.Design;
namespace DevExpress.Utils.Svg {
	public class SvgImage {
		const int DefaultScale = 16;
		static Image image = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		List<SvgElement> elementsCore;
		public SvgImage() {
			elementsCore = new List<SvgElement>();
		}
		public List<SvgElement> Elements {
			get { return elementsCore; }
		}
		public Image Render(Size imageSize, ISvgPaletteProvider paletteProvider) {
			using(Graphics g = Graphics.FromImage(image)) {
				g.Clear(Color.Transparent);
				foreach(var item in Elements) {
					item.Render(g, paletteProvider, DefaultScale, 0);
				}
			}
			return ScaleImg(imageSize);
		}
		static Bitmap ScaleImg(Size imageSize) {
			var img2 = new Bitmap(imageSize.Width, imageSize.Height,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using(Graphics g = Graphics.FromImage(img2)) {
				g.DrawImage(image, 0, 0, img2.Width, img2.Height);
			}
			return img2;
		}
	}
}
