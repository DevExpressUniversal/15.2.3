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
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	internal static class ASPxGridViewHelper {
		public static EditPropertiesBase GetAutoFilterCellEditor(EditPropertiesBase sourceEditorProperties, Type memberType,
			string captionForTrue, string imageForTrue, string captionForFalse, string imageForFalse, EnumDescriptor enumDescriptor) {
			EditPropertiesBase editorProperties = sourceEditorProperties;
			if(memberType == typeof(bool)) {
				ComboBoxProperties comboBoxProperties = editorProperties as ComboBoxProperties;
				if(comboBoxProperties != null) {
					foreach(ListEditItem item in comboBoxProperties.Items) {
						bool? val = item.Value as bool?;
						if(val != null && val.HasValue) {
							if(val.Value) {
								if(!string.IsNullOrEmpty(captionForTrue)) {
									item.Text = captionForTrue;
								}
								ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageForTrue);
								if(!imageInfo.IsUrlEmpty) {
									item.ImageUrl = imageInfo.ImageUrl;
									comboBoxProperties.ShowImageInEditBox = true;
								}
							}
							else {
								if(!string.IsNullOrEmpty(captionForFalse)) {
									item.Text = captionForFalse;
								}
								ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageForFalse);
								if(!imageInfo.IsUrlEmpty) {
									item.ImageUrl = imageInfo.ImageUrl;
									comboBoxProperties.ShowImageInEditBox = true;
								}
							}
						}
					}
				}
			}
			else if(memberType.IsEnum && (enumDescriptor != null)) {
				ComboBoxProperties props = new ComboBoxProperties();
				editorProperties = props;
				props.ValueType = enumDescriptor.EnumType;
				foreach(object enumValue in enumDescriptor.Values) {
					ImageInfo imageInfo = enumDescriptor.GetImageInfo(enumValue);
					if(imageInfo.IsUrlEmpty) {
						props.Items.Add(enumDescriptor.GetCaption(enumValue), enumValue);
					}
					else {
						props.Items.Add(enumDescriptor.GetCaption(enumValue), enumValue, imageInfo.ImageUrl);
						props.ShowImageInEditBox = true;
					}
				}
			}
			return editorProperties;
		}
	}
}
