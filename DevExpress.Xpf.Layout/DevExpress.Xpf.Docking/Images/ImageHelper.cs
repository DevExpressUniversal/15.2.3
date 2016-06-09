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
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.Images {
	public class DefaultImages {
		public const string AutoHidePanelCaption = "DefaultAutoHidePanelCaption";
		public const string TabPageCaption = "DefaultTabPageCaption";
		public const string LayoutControlItem = "LayoutControlItem";
		public const string LayoutGroup = "LayoutGroup";
		public const string LayoutPanel = "LayoutPanel";
		public const string TabbedGroup = "TabbedGroup";
		public const string FloatGroup = "FloatGroup";
		public const string DocumentPanel = "DocumentPanel";
		public const string DocumentGroup = "DocumentGroup";
		public const string EmptySpaceItem = "EmptySpaceItem";
		public const string Label = "Label";
		public const string Separator = "Separator";
		public const string Splitter = "Splitter";
		public const string BeginCustomizationIcon = "Customization";
		public const string EndCustomizationIcon = "Customization";
		public const string NewHorizontalTabGroup = "NewHorizontalTabGroup";
		public const string NewVerticalTabGroup = "NewVerticalTabGroup";
	}
	public class ImageHelper {
		static Dictionary<string, BitmapImage> images;
		static Dictionary<string, BitmapImage> Images {
			get {
				if(images == null) 
					images = new Dictionary<string, BitmapImage>();
				return images;
			}
		}
		static BitmapImage LoadImage(string imageName) {
			string resourcePath = string.Format("DevExpress.Xpf.Docking.Images.{0}.png", imageName);
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			using(Stream stream = currentAssembly.GetManifestResourceStream(resourcePath)) {
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = stream;
				bitmapImage.EndInit();
				bitmapImage.Freeze();
				return bitmapImage;
			}
		}
		public static BitmapImage GetImage(string imageName) {
			BitmapImage result;
			if(!Images.TryGetValue(imageName, out result)) {
				result = LoadImage(imageName);
				Images.Add(imageName, result);
			}
			return result;
		}
		public static ImageSource GetImageForItem(BaseLayoutItem item) {
			ImageSource imageSource = null;
			if(item == null) return null;
			switch(item.ItemType) {
				case LayoutItemType.ControlItem:
					imageSource = ImageHelper.GetImage(DefaultImages.LayoutControlItem);
					break;
				case LayoutItemType.Group:
					imageSource = ImageHelper.GetImage(DefaultImages.LayoutGroup);
					break;
				case LayoutItemType.Panel:
					imageSource = ImageHelper.GetImage(DefaultImages.LayoutPanel);
					break;
				case LayoutItemType.TabPanelGroup:
					imageSource = ImageHelper.GetImage(DefaultImages.TabbedGroup);
					break;
				case LayoutItemType.Document:
					imageSource = ImageHelper.GetImage(DefaultImages.DocumentPanel);
					break;
				case LayoutItemType.DocumentPanelGroup:
					imageSource = ImageHelper.GetImage(DefaultImages.DocumentGroup);
					break;
				case LayoutItemType.FloatGroup:
					imageSource = ImageHelper.GetImage(DefaultImages.FloatGroup);
					break;
				case LayoutItemType.LayoutSplitter:
					imageSource = ImageHelper.GetImage(DefaultImages.Splitter);
					break;
				case LayoutItemType.Separator:
					imageSource = ImageHelper.GetImage(DefaultImages.Separator);
					break;
				case LayoutItemType.EmptySpaceItem:
					imageSource = ImageHelper.GetImage(DefaultImages.EmptySpaceItem);
					break;
				case LayoutItemType.Label:
					imageSource = ImageHelper.GetImage(DefaultImages.Label);
					break;
			}
			return imageSource;
		}
	}
}
