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

using System;
using System.IO;
using System.Drawing;
using DevExpress.Data.Utils;
#if SL
using DevExpress.XtraPrinting.Stubs;
using DevExpress.Xpf.Drawing.Imaging;
using DevExpress.Xpf.Drawing;
#else
using System.Drawing.Imaging;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class PSConvert {
		static ImageTool imageTool;
		public static ImageTool ImageTool {
			get {
				if(imageTool == null)
					imageTool = new ImageTool();
				return imageTool;
			}
			set {
				imageTool = value;
			}
		}
		static public string ToRomanString(int val) {
			const int length = 13;
			string[] roman = new string[length] {"M","CM","D","CD","C","XC","L","XL","X","IX","V","IV","I"};
			int[] grade = new int[length] {1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1};
			string s = String.Empty;
			for(int i = 0; i < length; i++) {
				int quot = val / grade[i];
				for(int j = 0; j < quot; j++)  s += roman[i];
				val = val % grade[i];
			}
			return s;
		}
		public static byte[] ImageToArray(Image img) {
			return ImageTool.ToArray(img);
		}
		public static byte[] ImageToArray(Image img, ImageFormat format) {
			return ImageTool.ToArray(img, format);
		}
		public static void SaveImage(Image img, Stream stream, ImageFormat format) {
			ImageTool.SaveImage(img, stream, format);
		}
		public static Image ImageFromArray(byte[] buffer) {
			return ImageTool.FromArray(buffer);
		}
		public static StringAlignment ToStringAlignment(DevExpress.Utils.HorzAlignment hAlignment) {
			return hAlignment == DevExpress.Utils.HorzAlignment.Center ? StringAlignment.Center :
				hAlignment == DevExpress.Utils.HorzAlignment.Far ? StringAlignment.Far :
				StringAlignment.Near;
		}
		public static StringAlignment ToStringAlignment(DevExpress.Utils.VertAlignment vAlignment) {
			return vAlignment == DevExpress.Utils.VertAlignment.Center ? StringAlignment.Center :
				vAlignment == DevExpress.Utils.VertAlignment.Bottom ? StringAlignment.Far :
				StringAlignment.Near;
		}
		public static DevExpress.Utils.HorzAlignment ToHorzAlignment(StringAlignment alignment) {
			return alignment == StringAlignment.Center ? DevExpress.Utils.HorzAlignment.Center :
				alignment == StringAlignment.Far ? DevExpress.Utils.HorzAlignment.Far :
				DevExpress.Utils.HorzAlignment.Near;
		}
		public static DevExpress.Utils.VertAlignment ToVertAlignment(StringAlignment alignment) {
			return alignment == StringAlignment.Center ? DevExpress.Utils.VertAlignment.Center :
				alignment == StringAlignment.Far ? DevExpress.Utils.VertAlignment.Bottom :
				DevExpress.Utils.VertAlignment.Top;
		}
	}
}
