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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using DevExpress.Compatibility.System.Drawing;
#if !DXWINDOW
using DevExpress.Data.Utils;
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow.Data {
#else
namespace DevExpress.Utils {
#endif
	public static class ResourceStreamHelper {
		public static Stream GetStream(string name, Type type) {
			return GetStream(GetResourceName(type, name), type.GetAssembly());
		}
		public static Stream GetStream(string name, System.Reflection.Assembly asm) {
			return asm.GetManifestResourceStream(name);
		}
		public static byte[] GetBytes(string name, System.Reflection.Assembly asm) {
			using(Stream stream = GetStream(name, asm)) {
				byte[] bytes = new byte[stream.Length];
				stream.Read(bytes, 0, bytes.Length);
				return bytes;
			}
		}
		public static string GetResourceName(Type baseType, string name) {
			return string.Format("{0}.{1}", baseType.Namespace, name);
		}
	}
	public static class ResourceImageHelper {
#if !DXPORTABLE
		public static Cursor CreateCursorFromResources(string name, System.Reflection.Assembly asm) {
			System.IO.Stream stream = asm.GetManifestResourceStream(name);
			return new Cursor(stream);
		}
		public static ImageList CreateImageListFromResources(string name, System.Reflection.Assembly asm, Size size) {
			return CreateImageListFromResources(name, asm, size, Color.Magenta);
		}
		public static ImageList CreateImageListFromResources(string name, System.Reflection.Assembly asm, Size size, Color transparent) {
			return CreateImageListFromResources(name, asm, size, transparent, ColorDepth.Depth8Bit);
		}
		public static void FillImageListFromResources(ImageList images, string name, System.Reflection.Assembly asm, Color transparent) {
			Bitmap image = CreateBitmapFromResources(name, asm);
			image.MakeTransparent(transparent);
			images.Images.AddStrip(image);
		}
		public static void FillImageListFromResources(ImageList images, string name, Type type) {
			FillImageListFromResources(images, ResourceStreamHelper.GetResourceName(type, name), type.Assembly);
		}
		public static void FillImageListFromResources(ImageList images, string name, System.Reflection.Assembly asm) {
			FillImageListFromResources(images, name, asm, images.TransparentColor);
		}
		public static ImageList CreateImageListFromResources(string name, System.Reflection.Assembly asm, Size size, Color transparent, ColorDepth depth) {
			if(transparent == Color.Empty) transparent = Color.Magenta;
			ImageList images = new ImageList();
			images.ColorDepth = depth;
			images.ImageSize = size.IsEmpty ? new Size(16, 16) : size;
			FillImageListFromResources(images, name, asm, transparent);
			return images;
		}
#endif
		public static Bitmap CreateBitmapFromResources(string name, Type type) {
			return CreateBitmapFromResources(ResourceStreamHelper.GetResourceName(type, name), type.GetAssembly());
		}
		public static Bitmap CreateBitmapFromResources(string name, System.Reflection.Assembly asm) {
			return (Bitmap)CreateImageFromResources(name, asm);
		}
#if !DXPORTABLE
		public static Icon CreateIconFromResources(string name, Type type) {
			return CreateIconFromResources(ResourceStreamHelper.GetResourceName(type, name), type.Assembly);
		}
		public static Icon CreateIconFromResources(string name, System.Reflection.Assembly asm) {
			System.IO.Stream stream = asm.GetManifestResourceStream(name);
			Icon icon = new Icon(stream);
			return icon;
		}
#endif
		public static Image CreateImageFromResources(string name, Type type) {
			return CreateImageFromResources(ResourceStreamHelper.GetResourceName(type, name), type.GetAssembly());
		}
		public static Image CreateImageFromResources(string name, System.Reflection.Assembly asm) {
			System.IO.Stream stream = asm.GetManifestResourceStream(name);
#if DXPORTABLE
			return Image.FromStream(stream);
#else
			return ImageTool.ImageFromStream(stream);
#endif
		}
		public static Image CreateImageFromResourcesEx(string name, System.Reflection.Assembly asm) {
			System.IO.Stream stream = FindStream(name, asm);
			if(stream == null)
				return null;
#if DXPORTABLE
			return Image.FromStream(stream);
#else
			return ImageTool.ImageFromStream(stream);
#endif
		}
#if !DXPORTABLE
		public static Icon CreateIconFromResourcesEx(string name, System.Reflection.Assembly asm) {
			System.IO.Stream stream = FindStream(name, asm);
			if(stream == null) return null;
			Icon icon = new Icon(stream);
			return icon;
		}
#endif
		static System.IO.Stream FindStream(string name, System.Reflection.Assembly asm) {
			System.IO.Stream stream = asm.GetManifestResourceStream(name);
			if(stream == null) {
				string[] names = name.Split('.');
				if(names.Length >= 2)
					stream = asm.GetManifestResourceStream(string.Format("{0}.{1}", names.GetValue(names.Length - 2), names.GetValue(names.Length - 1)));
			}
			return stream;
		}
	}
}
