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
using System.Windows.Media;
using System.ComponentModel;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Internal.Implementations;
using DevExpress.Xpf.Scheduler.Internal.Implementations;
namespace DevExpress.Xpf.Scheduler {
	public static class IResourceEx {
		public static Color GetColor(this Resource resource) {
			ResourceInstance resourceObject = resource as ResourceInstance;
			if (resourceObject == null)
				return ColorExtension.Empty;
			return resourceObject.Color;
		}
		public static void SetColor(this Resource resource, Color color) {
			ResourceInstance resourceObject = resource as ResourceInstance;
			if (resourceObject == null)
				return;
			resourceObject.Color = color;
		}
		public static ImageSource GetImage(this Resource resource) {
			ResourceInstance resourceObject = resource as ResourceInstance;
			if (resourceObject == null)
				return null;
			return resourceObject.Image;
		}
		public static void SetImage(this Resource resource, ImageSource image) {
			ResourceInstance resourceObject = resource as ResourceInstance;
			if (resourceObject == null)
				return;
			resourceObject.Image = image;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Internal.Implementations {
	public class ResourceInstance : ResourceBase {
		ImageSource image = null;
		internal ResourceInstance() {
		}
		public ResourceInstance(object id, string caption)
			: base(id, caption) {
		}
		public ResourceInstance(object id, string caption, object parentId)
			: base(id, caption, parentId) {
		}
		public new Color Color {
			get { return WpfColorToStringSerializer.StringToColor(ColorValue); }
			set {
				string newColorValue = WpfColorToStringSerializer.ColorToString(value);
				if (ColorValue == newColorValue)
					return;
				Color oldColor = Color;
				if (OnContentChanging("Color", oldColor, value)) {
					ColorValue = newColorValue;
					OnContentChanged();
					NotifyPropertyChanged("Color");
				}
			}
		}
		public new ImageSource Image {
			get {
				if (image == null)
					image = WpfSchedulerImageHelper.CreateImageFromBytes(ImageBytes);
				return image;
			}
			set {
				if (Object.Equals(image, value))
					return;
				if (OnContentChanging("Image", this.image, value)) {
					AssignImage(value);
					OnContentChanged();
				}
			}
		}
		void AssignImage(ImageSource val) {
			if (val != null) {
				image = val.Clone();
				ImageBytes = WpfSchedulerImageHelper.GetImageBytes(image);
			} else {
				image = null;
				ImageBytes = null;
			}
		}
		internal void ReplaceImage(ImageSource newImage) {
			if (newImage != null) {
				image = newImage;
				ImageBytes = WpfSchedulerImageHelper.GetImageBytes(image);
			} else {
				image = null;
				ImageBytes = null;
			}
		}
	}
}
