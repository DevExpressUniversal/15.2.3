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
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Markup;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class ImageHelper {
		static Dictionary<string, BitmapImage> images = new Dictionary<string, BitmapImage>();
		static BitmapImage LoadImage(string imageName, bool fromCore = false) {
			string resourcePath = string.Format("DevExpress.{1}PivotGrid.Images.{0}.png", imageName, fromCore ? "Xtra" : "Xpf.");
			Assembly asm = fromCore ? typeof(DevExpress.XtraPivotGrid.Data.PivotGridData).Assembly : Assembly.GetExecutingAssembly();
			return DevExpress.Xpf.Core.Native.ImageHelper.CreateImageFromEmbeddedResource(asm, resourcePath);
		}
		public static BitmapImage GetImage(string imageName, bool fromCore = false) {
			BitmapImage image;
			if(!images.TryGetValue(imageName, out image)) {
				image = LoadImage(imageName, fromCore);
				images.Add(imageName, image);
			}
			return image;
		}
		public static BitmapImage GetImage(PivotKpiGraphic graphic, int state) {
			if(state > 1 || state < -1 || graphic == PivotKpiGraphic.ServerDefined || graphic == PivotKpiGraphic.None)
				return null;
			return GetImage(graphic.ToString() + "." + state.ToString(), true);
		}
		internal static BitmapImage GetImage(FieldListActualArea area) {
			switch(area) {
				case FieldListActualArea.AllFields:
					return GetImage("Customization2007HiddenFields");
				case FieldListActualArea.HiddenFields:
					return GetImage("Customization2007HiddenFields");
				case FieldListActualArea.FilterArea:
					return GetImage("Customization2007Filter");
				case FieldListActualArea.ColumnArea:
					return GetImage("Customization2007Column");
				case FieldListActualArea.RowArea:
					return GetImage("Customization2007Row");
				case FieldListActualArea.DataArea:
					return GetImage("Customization2007Data");
				default:
					throw new ArgumentException("FieldListActualArea");
			}
		}
	}
	public class PivotImageExtension : MarkupExtension {
		public string ImageName { get; set; }
		public PivotImageExtension() { }
		public PivotImageExtension(string imageName) {
			ImageName = imageName;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return ImageHelper.GetImage(ImageName);
		}
	}
}
