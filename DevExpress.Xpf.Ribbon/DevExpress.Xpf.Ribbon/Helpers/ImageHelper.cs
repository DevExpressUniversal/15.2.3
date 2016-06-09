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

using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Ribbon {
	public class ImageHelper {
		static Dictionary<int, Dictionary<string, BitmapImage>> images = new Dictionary<int, Dictionary<string, BitmapImage>>();
		static object olock = new object();
		static Dictionary<string, BitmapImage> Images {
			get {
				lock(olock) {
					if(!images.ContainsKey(System.Threading.Thread.CurrentThread.ManagedThreadId))
						images[System.Threading.Thread.CurrentThread.ManagedThreadId] = new Dictionary<string, BitmapImage>();
				}
				return images[System.Threading.Thread.CurrentThread.ManagedThreadId];
			}
		}
		const string themeAssemblyPath = @"/DevExpress.Xpf.Ribbon/Images/";
		static BitmapImage LoadImage(string imageName) {
			string resourcePath = string.Format("DevExpress.Xpf.Ribbon.Images.{0}", imageName);
			using(System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath)) {
				return DevExpress.Xpf.Core.Native.ImageHelper.CreateImageFromStream(stream);
			}
		}
		public static BitmapImage GetImage(string imageName) {
			BitmapImage image;
			if(!Images.TryGetValue(imageName, out image)) {
				image = LoadImage(imageName);
				Images.Add(imageName, image);
			}
			return image;
		}
		public static BitmapImage GetThemeImage(string themeName, string imageName) {
			if(themeName == null || themeName.Length == 0)
				return null;
			if(themeName == Theme.DeepBlueName)
				return null;
			string assemblyThemeName = GetThemeAssemblyName(themeName);
			if(assemblyThemeName == null) return null;
			return new BitmapImage(new Uri("/" + assemblyThemeName + ";component" + themeAssemblyPath + imageName + ".png", UriKind.Relative));
		}
		private static string GetThemeAssemblyName(string themeName) {
			var theme = Theme.FindTheme(themeName);
			return theme == null ? null : theme.AssemblyName;
		}
		public static ImageSource GetSmallIcon(ImageSource originalImage) {
			BitmapFrame bitmapFrame = originalImage as BitmapFrame;
			BitmapDecoder decoder = null;
			if(bitmapFrame == null) {
				BitmapImage bitmapImage = originalImage as BitmapImage;
				if(bitmapImage == null)
					return originalImage;
				decoder = BitmapDecoder.Create(new Uri(bitmapImage.ToString()), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);
			} else decoder = bitmapFrame.Decoder;
			if(decoder == null)
				return originalImage;
			var result = decoder.Frames.FirstOrDefault(f => f.Width == 16);
			if(result == default(BitmapFrame))
				result = decoder.Frames.OrderBy(f => f.Width).First();
			return result;
		}
	}
}
