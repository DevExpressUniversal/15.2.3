#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.ExpressApp.Design {
	public enum EmbeddedResourceImage { DomainObject, Module, ModuleLink, Property, PersistentAssembly, BaseTypesNode, DerivedTypesNode };
	class DesignImagesLoader {
		public static Image GetImage(EmbeddedResourceImage imageKey) {
			switch(imageKey) { 
				case EmbeddedResourceImage.DomainObject:
					return GetImage("Designer_Object_DomainObject.ico");
				case EmbeddedResourceImage.Module:
					return GetImage("Module.ico");
				case EmbeddedResourceImage.ModuleLink:
					return GetImage("Designer_Module_Link.ico");
				case EmbeddedResourceImage.Property:
					return GetImage("Designer_Object_DomainObjectProperty.ico");
				case EmbeddedResourceImage.PersistentAssembly:
					return GetImage("Designer_Object_PersistentObjectAssembly.ico");
				case EmbeddedResourceImage.BaseTypesNode:
					return GetImage("Designer_BaseTypesNode.ico");
				case EmbeddedResourceImage.DerivedTypesNode:
					return GetImage("Designer_DerivedTypesNode.ico");
			}
			return new Bitmap(16, 16);
		}
		public static void AddResourceImage(ImageList imageList, EmbeddedResourceImage imageKey) {
			imageList.Images.Add(imageKey.ToString(), GetImage(imageKey));
		}
		public static Image GetImage(string imageName) {
			try {
				return Image.FromStream(typeof(DesignImagesLoader).Assembly.GetManifestResourceStream("DevExpress.ExpressApp.Design.Resources." + imageName));
			}
			catch(Exception e) {
#if DEBUG
				MessageBox.Show("Cannot load image " + imageName + "\n" + e.Message);
#endif
				return new Bitmap(16, 16);
			}
		}
		public static string GetImageKey(ImageList imageList, Type type) {
			return GetImageKey(imageList, type, false);
		}
		public static string GetImageKey(ImageList imageList, Type type, bool needLargeImage) {
			Type toolboxType = null;
			Type currentType = type;
			while(currentType != typeof(object)) {
				object[] attributes = currentType.GetCustomAttributes(typeof(ToolboxBitmapAttribute), false);
				if(attributes.Length == 1) {
					toolboxType = currentType;
					break;
				}
				currentType = currentType.BaseType;
			}
			if(toolboxType != null) {
				if(!imageList.Images.ContainsKey(toolboxType.FullName)) {
					ToolboxBitmapAttribute bitmapAttribute = (ToolboxBitmapAttribute)System.Attribute.GetCustomAttribute(toolboxType, typeof(ToolboxBitmapAttribute));
					if(bitmapAttribute != null) {
						Image toolboxImage = bitmapAttribute.GetImage(type, needLargeImage);
						GC.Collect();
						if(toolboxImage != null) {
							try {
								imageList.Images.Add(toolboxType.FullName, toolboxImage);
							}
							catch(InvalidOperationException) {
								try {
									imageList.Images.Add(toolboxType.FullName, toolboxImage);
								}
								catch(InvalidOperationException ex) {
									string message = "InvalidOperationException occured while trying to add an image into the image list.";
									message += string.Format("\r\nWidth: {0}, Height: {1}, RawFormat: {2}", toolboxImage.Width, toolboxImage.Height, toolboxImage.RawFormat);
									InvalidOperationException newException = new InvalidOperationException(message, ex);
									throw newException;
								}
							}
						}
					}
				}
				return toolboxType.FullName;
			}
			return "";
		}
		public static string GetImageKey(ImageList imageList, Component component) {
			return GetImageKey(imageList, component.GetType(), false);
		}
		public static string GetImageKey(ImageList imageList, Component component, bool needLargeImage) {
			return GetImageKey(imageList, component.GetType(), needLargeImage);
		}
		public static string GetImageKey(ImageList imageList, EmbeddedResourceImage imageKey) {
			string key = imageKey.ToString();
			if(!imageList.Images.ContainsKey(key)) { 
				imageList.Images.Add(key, GetImage(imageKey));
			}
			return key;
		}
	}
}
