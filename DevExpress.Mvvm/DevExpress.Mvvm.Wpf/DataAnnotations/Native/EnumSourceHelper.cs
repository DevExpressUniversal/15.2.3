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

using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Mvvm.Native {
	public static class EnumSourceHelperCore {
		public static readonly string ValueMemberName = BindableBase.GetPropertyName(() => new EnumMemberInfo(null, null, null, null).Id);
		public static readonly string DisplayMemberName = BindableBase.GetPropertyName(() => new EnumMemberInfo(null, null, null, null).Name);
		const int DefaultDisplayOrder = 10000;
		public static int GetEnumCount(Type enumType) {
			return Enum.GetValues(enumType).Length;
		}
		public static IEnumerable<EnumMemberInfo> GetEnumSource(Type enumType, bool useUnderlyingEnumValue = true, IValueConverter nameConverter = null, bool splitNames = false, EnumMembersSortMode sortMode = EnumMembersSortMode.Default, Func<string, bool, string> getKnownImageUriCallback = null, bool showImage = true, bool showName = true) {
			if(enumType == null || !enumType.IsEnum)
				return Enumerable.Empty<EnumMemberInfo>();
			var result = enumType.GetFields(BindingFlags.Static | BindingFlags.Public)
				.Where(field => DataAnnotationsAttributeHelper.GetAutoGenerateField(field))
				.Select(field => {
					Enum value = (Enum)field.GetValue(null);
					string name = GetEnumName(field, value, nameConverter, splitNames);
					var imageInfo = GetImageInfo(MetadataHelper.GetAttribute<ImageAttribute>(field), MetadataHelper.GetAttribute<DXImageAttribute>(field), null, getKnownImageUriCallback);
					ImageSource image = ViewModelBase.IsInDesignMode ? null : (imageInfo.Item1 ?? imageInfo.Item2).With(x => (ImageSource)new ImageSourceConverter().ConvertFrom(x));
					return new EnumMemberInfo(name, DataAnnotationsAttributeHelper.GetFieldDescription(field), useUnderlyingEnumValue ? GetUnderlyingEnumValue(value) : value, image, showImage, showName,
					 DataAnnotationsAttributeHelper.GetFieldOrder(field));
				});
			switch(sortMode) {
				case EnumMembersSortMode.DisplayName:
					result = result.OrderBy(x => x.Name);
					break;
				case EnumMembersSortMode.DisplayNameDescending:
					result = result.OrderByDescending(x => x.Name);
					break;
				case EnumMembersSortMode.DisplayNameLength:
					result = result.OrderBy(x => x.Name.Length);
					break;
				case EnumMembersSortMode.DisplayNameLengthDescending:
					result = result.OrderByDescending(x => x.Name.Length);
					break;
			}
			return result.OrderBy(x => (x.Order != null) ? x.Order : DefaultDisplayOrder).ToArray();
		}
		static string GetEnumName(FieldInfo field, Enum value, IValueConverter nameConverter, bool splitNames) {
			if(nameConverter != null)
				return nameConverter.Convert(value, typeof(string), null, CultureInfo.CurrentCulture) as string;
			string name = DataAnnotationsAttributeHelper.GetFieldDisplayName(field) ??
#if !SILVERLIGHT
 TypeDescriptor.GetConverter(value.GetType()).ConvertTo(value, typeof(string)) as string
#else
				value.ToString()
#endif
;
			if(splitNames)
				return SplitStringHelper.SplitPascalCaseString(name);
			return name;
		}
		static object GetUnderlyingEnumValue(Enum value) {
			Type enumType = Enum.GetUnderlyingType(value.GetType());
			return Convert.ChangeType(value, enumType, System.Globalization.CultureInfo.CurrentCulture);
		}
#if !FREE
		public
#endif
 static Tuple<string, string> GetImageInfo(ImageAttribute image, DXImageAttribute dxImage, string defaultImageName, Func<string, bool, string> getKnownImageUriCallback) {
			if(image != null)
				return Tuple.Create(image.ImageUri, (string)null);
			string imageName = dxImage.With(x => x.ImageName) ?? defaultImageName;
			return Tuple.Create(dxImage.With(x => x.SmallImageUri) ?? GetKnownImageUri(getKnownImageUriCallback, imageName, false),
								dxImage.With(x => x.LargeImageUri) ?? GetKnownImageUri(getKnownImageUriCallback, imageName, true));
		}
		static string GetKnownImageUri(Func<string, bool, string> getKnownImageUriCallback, string imageName, bool large) {
			if(getKnownImageUriCallback == null || string.IsNullOrEmpty(imageName))
				return null;
			return getKnownImageUriCallback(imageName, large);
		}
	}
}
namespace DevExpress.Mvvm {
	public static class EnumSourceHelper {
		public static IEnumerable<EnumMemberInfo> GetEnumSource(Type enumType, bool useUnderlyingEnumValue = true, IValueConverter nameConverter = null, bool splitNames = false, EnumMembersSortMode sortMode = EnumMembersSortMode.Default) {
			return EnumSourceHelperCore.GetEnumSource(enumType, useUnderlyingEnumValue, nameConverter, splitNames, sortMode, null);
		}
	}
	public class EnumMemberInfo {
		public EnumMemberInfo(string value, string description, object id, ImageSource image)
			: this(value, description, id, image, true, true) {
			this.Name = value;
			this.Description = description;
			this.Id = id;
			this.Image = image;
		}
		public EnumMemberInfo(string value, string description, object id, ImageSource image, bool showImage, bool showName, int? order = null) {
			this.Name = value;
			this.Description = description;
			this.Id = id;
			this.Image = image;
			this.ShowImage = showImage;
			this.ShowName = showName;
			this.Order = order;
		}
		public string Name { get; private set; }
		public bool ShowName { get; private set; }
		public object Id { get; private set; }
		public string Description { get; private set; }
		public ImageSource Image { get; private set; }
		public bool ShowImage { get; private set; }
		public int? Order { get; private set; }
		public override string ToString() {
			return Name.ToString();
		}
		public override bool Equals(object obj) {
			return object.Equals(Id, (obj as EnumMemberInfo).Return(o => o.Id, () => null));
		}
		public override int GetHashCode() {
			return Id.GetHashCode();
		}
	}
	public enum EnumMembersSortMode {
		Default,
		DisplayName,
		DisplayNameDescending,
		DisplayNameLength,
		DisplayNameLengthDescending,
	}
}
