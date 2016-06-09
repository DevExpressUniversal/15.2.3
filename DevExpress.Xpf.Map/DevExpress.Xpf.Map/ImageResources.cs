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
namespace DevExpress.Xpf.Map {
	public class DXMapImages {
		const string imagesPath = "DevExpress.Xpf.Map.Images.{0}";
		BitmapImage pushpinPointerShadow = null;
		BitmapImage kmlDefaultPoint = null;
		BitmapImage kmlWrongIcon = null;
		static BitmapImage GetImageFromResources(string name) {
			Assembly assembly = Assembly.GetExecutingAssembly();
			string path = String.Format(imagesPath, name);
			using (Stream stream = assembly.GetManifestResourceStream(path)) {
				BitmapImage image = new BitmapImage();
				image.BeginInit();
				try {
					image.StreamSource = stream;
				}
				finally {
					image.EndInit();
					if (image.CanFreeze)
						image.Freeze();
				}
				return image;
			}
		}
		public BitmapImage PushpinPointerShadow {
			get {
				if (pushpinPointerShadow == null)
					pushpinPointerShadow = GetImageFromResources("Shadow.png");
				return pushpinPointerShadow;
			}
		}
		public BitmapImage KmlDefaultPoint {
			get {
				if (kmlDefaultPoint == null)
					kmlDefaultPoint = GetImageFromResources("KmlPushpin.png");
				return kmlDefaultPoint;
			}
		}
		public BitmapImage KmlWrongIcon {
			get {
				if (kmlWrongIcon == null)
					kmlWrongIcon = GetImageFromResources("KmlPushpin.png");
				return kmlWrongIcon;
			}
		}
	}
}
