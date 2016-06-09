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
using DevExpress.XtraReports.UserDesigner;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using DevExpress.XtraReports.UI;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using System.ComponentModel;
namespace DevExpress.XtraReports.Native {
	public class ResLoaderBase {
		public static Bitmap LoadBitmap(string name, Type type, Color transparentColor) {
			Bitmap bitmap = ResourceImageHelper.CreateBitmapFromResources(name, type);
			if(transparentColor != Color.Empty)
				bitmap.MakeTransparent(transparentColor);
			return bitmap;
		}
		public static Cursor LoadCursor(string name, Type type) {
			System.IO.Stream stream = null;
			try {
				stream = GetResourceStream(name, type);
				try {
					return new Cursor(stream);
				} catch {
					return Cursors.Default;
				}
			} finally {
				if(stream != null)
					stream.Close();
			}
		}
		[System.Security.SecuritySafeCritical]
		static IntPtr LoadCursorFromFile(string fileName) { return LoadCursorFromFile_(fileName); }
		[DllImport("user32.dll", EntryPoint = "LoadCursorFromFile")]
		static extern IntPtr LoadCursorFromFile_(string fileName);
		[System.Security.SecuritySafeCritical]
		static IntPtr DeleteObject(IntPtr hDc) { return DeleteObject_(hDc); }
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		static extern IntPtr DeleteObject_(IntPtr hDc);
		public static Cursor LoadColoredCursor(string name, Type type) {
			Cursor coloredCursor;
			IntPtr coloredCursorPtr = IntPtr.Zero;
			Stream resourceStream = null;
			try {
				resourceStream = GetResourceStream(name, type);
				string tempFilePath = Path.GetTempFileName();
				try {
					FileStream tempFileStream = new FileStream(tempFilePath, FileMode.Create);
					ReadWriteStream(resourceStream, tempFileStream);
					coloredCursorPtr = LoadCursorFromFile(tempFilePath);
					coloredCursor = new Cursor(coloredCursorPtr);
				} catch {
					coloredCursor = Cursors.Default;
				} finally {
					File.Delete(tempFilePath);
					DeleteObject(coloredCursorPtr);
				}
			} finally {
				if(resourceStream != null)
					resourceStream.Close();
			}
			return coloredCursor;
		}
		private static System.IO.Stream GetResourceStream(string name, Type type) {
			string s = GetFullName(name, type);
			return type.Assembly.GetManifestResourceStream(s);
		}
		private static string GetFullName(string name, Type type) {
			return string.Concat(type.Namespace, ".", name);
		}
		private static void ReadWriteStream(Stream readStream, Stream writeStream) {
			const int length = 1024;
			Byte[] buffer = new Byte[length];
			int bytesRead = readStream.Read(buffer, 0, length);
			while(bytesRead > 0) {
				writeStream.Write(buffer, 0, bytesRead);
				bytesRead = readStream.Read(buffer, 0, length);
			}
			readStream.Close();
			writeStream.Close();
		}
		public static System.Resources.ResourceSet GetResourceSet(System.Resources.ResourceManager resourceManager) {
			try {
				return resourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentUICulture, true, true);
			} catch {
				return null;
			}
		}
	}
}
