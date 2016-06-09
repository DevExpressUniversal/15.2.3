#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing.Design;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Localization;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum ImageSizeMode {
		Clip,	   
		Stretch,
		Zoom,
		Squeeze
	}
	public enum ImageHorizontalAlignment {
		Left,
		Center,
		Right		
	}
	public enum ImageVerticalAlignment {
		Top,
		Center,
		Bottom
	}
	[
	DashboardItemType(DashboardItemType.Image)
	]
	public class ImageDashboardItem : DashboardItem {
		const string xmlSizeMode = "SizeMode";
		const string xmlHorizontalAlignment = "HorizontalAlignment";
		const string xmlVerticalAlignment = "VerticalAlignment";
		const ImageSizeMode DefaultSizeMode = ImageSizeMode.Clip;  
		const ImageHorizontalAlignment DefaultHorizontalAlignment = ImageHorizontalAlignment.Center;
		const ImageVerticalAlignment DefaultVerticalAlignment = ImageVerticalAlignment.Center;		
		readonly DashboardImage image = new DashboardImage();
		ImageSizeMode sizeMode = DefaultSizeMode;
		ImageHorizontalAlignment horizontalAlignment = DefaultHorizontalAlignment;
		ImageVerticalAlignment verticalAlignment = DefaultVerticalAlignment;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ImageDashboardItemUrl"),
#endif
		Category(CategoryNames.Data), 
		Editor(TypeNames.ImageFileNameEditor, typeof(UITypeEditor)),
		DefaultValue(null),
		Localizable(false)
		]
		public string Url { get { return image.Url; } set { image.Url = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ImageDashboardItemData"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(TypeNames.DisplayNameNoneObjectConverter),
		Editor(TypeNames.ImageDataEditor, typeof(UITypeEditor))
		]
		public byte[] Data { get { return image.Data; } set { image.Data = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)
		]
		public string DataSerializable { get { return image.Base64Data; } set { image.Base64Data = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ImageDashboardItemSizeMode"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultSizeMode)
		]
		public ImageSizeMode SizeMode {
			get { return sizeMode; }
			set {
				if (value != sizeMode) {
					sizeMode = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ImageDashboardItemHorizontalAlignment"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultHorizontalAlignment)
		]
		public ImageHorizontalAlignment HorizontalAlignment {
			get { return horizontalAlignment; }
			set {
				if (horizontalAlignment != value) {
					horizontalAlignment = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ImageDashboardItemVerticalAlignment"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultVerticalAlignment)
		]
		public ImageVerticalAlignment VerticalAlignment {
			get { return verticalAlignment; }
			set {
				if (verticalAlignment != value) {
					verticalAlignment = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		internal DashboardImage Image { get { return image; } }
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameImageItem); } } 
		public ImageDashboardItem() {
			image.Changed += (sender, e) => OnChanged(ChangeReason.View);
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			return new ImageDashboardItemViewModel(this);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			image.SaveToXml(element);
			if(sizeMode != DefaultSizeMode)
				element.Add(new XAttribute(xmlSizeMode, sizeMode));
			if(horizontalAlignment != DefaultHorizontalAlignment)
				element.Add(new XAttribute(xmlHorizontalAlignment, horizontalAlignment));
			if(verticalAlignment != DefaultVerticalAlignment)
				element.Add(new XAttribute(xmlVerticalAlignment, verticalAlignment));
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			image.LoadFromXml(element);
			string sizeModeAttr = XmlHelper.GetAttributeValue(element, xmlSizeMode);
			if (!String.IsNullOrEmpty(sizeModeAttr))
				sizeMode = XmlHelper.EnumFromString<ImageSizeMode>(sizeModeAttr);
			string horizontalAlignmentAttr = XmlHelper.GetAttributeValue(element, xmlHorizontalAlignment);
			if (!String.IsNullOrEmpty(horizontalAlignmentAttr))
				horizontalAlignment = XmlHelper.EnumFromString<ImageHorizontalAlignment>(horizontalAlignmentAttr);
			string verticalAlignmentAttr = XmlHelper.GetAttributeValue(element, xmlVerticalAlignment);
			if (!String.IsNullOrEmpty(verticalAlignmentAttr))
				verticalAlignment = XmlHelper.EnumFromString<ImageVerticalAlignment>(verticalAlignmentAttr);
		}
	}
}
