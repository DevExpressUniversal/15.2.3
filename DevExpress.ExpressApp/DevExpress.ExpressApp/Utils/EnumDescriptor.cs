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
using System.Reflection;
using System.Collections;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Utils {
	public interface IEnumDescriptor {
		string GetCaption(object value);
	}
	public class EnumDescriptor : IEnumDescriptor {
		private const string GroupPathPrefix = "Enums/";
		private Dictionary<string, string> captionsByNames;
		private Dictionary<string, object> valuesByCaptions;
		private Type enumType;
		private ArrayList values = new ArrayList();
		private bool isNullable;
		private static string NullValueText {
			get { return CaptionHelper.GetLocalizedText("Enums", "NullValueText", CaptionHelper.NullValueText); }
		}
		private void FillLostCaptions() {
			if(captionsByNames == null)
				captionsByNames = new Dictionary<string, string>(values.Count);
			for(int i = 0; i < values.Count; i++) {
				string valueName = values[i] != null ? values[i].ToString() : NullValueText;
				if(!captionsByNames.ContainsKey(valueName)) {
					captionsByNames.Add(valueName, GetCaptionEnumValue(enumType, valueName, CompoundNameConvertStyle.None));
				}
			}
		}
		private static string GetCaptionEnumValue(Type enumType, string valueName, CompoundNameConvertStyle compoundNameConvertStyle) {
			string captionEnumValue = CaptionHelper.ConvertCompoundName(valueName, compoundNameConvertStyle);
			FieldInfo info = enumType.GetField(valueName);
			if(info != null) {
				XafDisplayNameAttribute[] attr = AttributeHelper.GetAttributes<XafDisplayNameAttribute>(info, false);
				if(attr.Length == 1) {
					captionEnumValue = attr[0].DisplayName;
				}
			}
			return captionEnumValue;
		}
		private void Setup(IModelLocalizationGroup enumLocalizationGroup) {
			Dictionary<string, string> captionsTemp = new Dictionary<string, string>();
			if(enumLocalizationGroup != null) {
				this.values.Clear();
				this.values.AddRange(Enum.GetValues(enumType));
				int itemIndex = 0;
				foreach(IModelLocalizationItem item in enumLocalizationGroup) {
					if(item.Index != null) {
						values.Remove(Enum.Parse(enumType, item.Name));
						values.Insert(itemIndex++, Enum.Parse(enumType, item.Name));
					}
					captionsTemp.Add(item.Name, item.Value); 
				}
			}
			Setup(captionsTemp);
		}
		private void Setup(Dictionary<string, string> captionsByNames) {
			this.captionsByNames = captionsByNames;
			if(this.values.Count == 0) {
				this.values.AddRange(Enum.GetValues(enumType));
			}
			if(isNullable) {
				this.values.Insert(0, null);
			}
			FillLostCaptions();
		}
		public EnumDescriptor(Type enumType, Dictionary<string, string> captionsByNames)
			: this(enumType) {
			Setup(captionsByNames);
		}
		public EnumDescriptor(Type type) {
			Guard.ArgumentNotNull(type, "type");
			isNullable = TypeHelper.IsNullable(type);
			if(isNullable) {
				type = TypeHelper.GetUnderlyingType(type);
			}
			if(type.IsEnum) {
				enumType = type;
			}
			else {
				throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.TypeIsNotAEnum, type.ToString()), "type");
			}
			Setup(CaptionHelper.GetModelLocalizationGroup(GroupPathPrefix + enumType.FullName));
		}
		public string GetCaption(string valueName) {
			string result;
			string keyName = valueName != null ? valueName : NullValueText;
			if(!captionsByNames.TryGetValue(keyName, out result)) {
				result = keyName.ToString();
			}
			return result;
		}
		public string GetCaption(object value) {
			return value != null ? GetCaption(value.ToString()) : NullValueText;
		}
		public string GetCaption(int intValue) {
			Array enumValues = Enum.GetValues(EnumType);
			foreach(object enumValue in enumValues) {
				if(intValue.Equals((int)enumValue)) {
					return GetCaption(enumValue);
				}
			}
			return "";
		}
		public ImageInfo GetImageInfo(object value) {
			if(value != null) {
				return ImageLoader.Instance.GetEnumValueImageInfo(this.EnumType, value);
			}
			return new ImageInfo();
		}
		public object ParseCaption(string caption) {
			if(valuesByCaptions == null) {
				valuesByCaptions = new Dictionary<string, object>(values.Count);
				for(int i = 0; i < values.Count; i++) {
					object value = values[i];
					valuesByCaptions.Add(GetCaption(value), value);
				}
			}
			return valuesByCaptions[caption];
		}
		public static Type GetEnumType(Type type) {
			if(type != null) {
				if(TypeHelper.IsNullable(type)) {
					type = TypeHelper.GetUnderlyingType(type);
				}
				if(type.IsEnum) {
					return type;
				}
			}
			return null;
		}
		public static void GenerateDefaultCaptions(IModelLocalizationGroup nodeGroup, Type enumType, CompoundNameConvertStyle compoundNameConvertStyle) {
			nodeGroup.Value = CaptionHelper.ConvertCompoundName(enumType.Name);
			Array enumValues = Enum.GetValues(enumType);
			List<string> enumValueNames = new List<string>(enumValues.Length);
			List<string> enumValueCaptions = new List<string>(enumValues.Length);
			for(int i = 0; i < enumValues.Length; i++) {
				object enumValue = enumValues.GetValue(i);
				string valueName = enumValue.ToString();
				enumValueNames.Add(valueName);
				string captionEnumValue = GetCaptionEnumValue(enumType, valueName, compoundNameConvertStyle);
				enumValueCaptions.Add(captionEnumValue);
			}
			CaptionHelper.SetLocalizedText(nodeGroup, enumValueNames, enumValueCaptions);
		}
		public static void GenerateDefaultCaptions(IModelLocalizationGroup nodeGroup, Type enumType) {
			GenerateDefaultCaptions(nodeGroup, enumType, CompoundNameConvertStyle.SplitAndCapitalization);
		}
		public Type EnumType {
			get { return enumType; }
		}
		public Array Values {
			get { return values.ToArray(); }
		}
		public ICollection<string> Captions {
			get { return captionsByNames.Values; }
		}
		public bool IsNullable {
			get { return isNullable; }
		}
	}
}
