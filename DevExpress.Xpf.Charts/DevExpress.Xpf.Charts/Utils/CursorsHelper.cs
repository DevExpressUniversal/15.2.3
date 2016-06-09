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
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Windows;
namespace DevExpress.Xpf.Charts.Native {
	public class BitmapCursor {
		public BitmapImage Image { get; set; }
		public Point Offset { get; set; }
		public BitmapCursor(BitmapImage image, Point offset) {
			Image = image;
			Offset = offset;
		}
	}
	public sealed class DragCursors {
		const string cursorsPath = "DevExpress.Xpf.Charts.Images.{0}";
		[ThreadStatic]
		static BitmapCursor handCursorField;
		[ThreadStatic]
		static BitmapCursor handDragCursorField;
		[ThreadStatic]
		static BitmapCursor zoomInCursorField;
		[ThreadStatic]
		static BitmapCursor zoomOutCursorField;
		[ThreadStatic]
		static BitmapCursor zoomLimitCursorField;
		public static BitmapCursor HandCursor {
			get {
				if (handCursorField == null)
					handCursorField = CreateCursor("CursorHand.png", new Point(-11, -8));
				return handCursorField;
			}
		}
		public static BitmapCursor HandDragCursor {
			get {
				if (handDragCursorField == null)
					handDragCursorField = CreateCursor("CursorHandDrag.png", new Point(-11, -8));
				return handDragCursorField;
			}
		}
		public static BitmapCursor ZoomInCursor {
			get {
				if (zoomInCursorField == null)
					zoomInCursorField = CreateCursor("CursorZoomIn.png", new Point(0, 0));
				return zoomInCursorField;
			}
		}
		public static BitmapCursor ZoomOutCursor {
			get {
				if (zoomOutCursorField == null)
					zoomOutCursorField = CreateCursor("CursorZoomOut.png", new Point(0, 0));
				return zoomOutCursorField;
			}
		}
		public static BitmapCursor ZoomLimitCursor {
			get {
				if (zoomLimitCursorField == null)
					zoomLimitCursorField = CreateCursor("CursorZoomLimit.png", new Point(0, 0));
				return zoomLimitCursorField;
			}
		}
		static BitmapCursor CreateCursor(string fileName, Point offset) {
			return new BitmapCursor(LoadCursorImageFromResource(String.Format(cursorsPath, fileName), Assembly.GetExecutingAssembly()), offset);
		}
		static BitmapImage LoadCursorImageFromResource(string resourceName, Assembly assembly) {
			using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
				BitmapImage image = new BitmapImage();
				image.BeginInit();
				try {
					image.StreamSource = stream;
				}
				finally {
					image.EndInit();
				}
				return image;
			}
		}
	}
}
