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
using DevExpress.Mvvm.POCO;
using System;
using System.Globalization;
namespace DevExpress.Mvvm.Native {
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class DataFormGroupAttribute : Attribute {
		public DataFormGroupAttribute(string groupName, int order) {
			this.GroupName = groupName;
			this.Order = order;
		}
		public string GroupName { get; private set; }
		public int Order { get; private set; }
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class TableGroupAttribute : Attribute {
		public TableGroupAttribute(string groupName, int order) {
			this.GroupName = groupName;
			this.Order = order;
		}
		public string GroupName { get; private set; }
		public int Order { get; private set; }
	}
	public enum GroupView { Group, GroupBox, Tabs }
	public static class LayoutGroupInfoConstants {
		public const int LastPropertiesStartIndex = 10000;
		public static char GroupPathSeparator = '/';
		public static char BorderlessGroupMarkStart = '<';
		public static char BorderlessGroupMarkEnd = '>';
		public static char GroupBoxMarkStart = '[';
		public static char GroupBoxMarkEnd = ']';
		public static char TabbedGroupMarkStart = '{';
		public static char TabbedGroupMarkEnd = '}';
		public static char HorizontalGroupMark = '-';
		public static char VerticalGroupMark = '|';
	}
	public enum LayoutType {
		Default,
		DataForm,
		ToolBar,
		ContextMenu,
		Table
	}
	public enum RegExMaskType {
		Simple, Regular, RegEx
	}
	public class MaskInfo {
		public const char DefaultPlaceHolderValue = '_';
		public const bool DefaultIgnoreBlankValue = true;
		public const bool DefaultSaveLiteralValue = true;
		public const bool DefaultUseAsDisplayFormatValue = false;
		public const bool DefaultAutomaticallyAdvanceCaretValue = false;
		public const bool DefaultShowPlaceHoldersValue = true;
		public const string DefaultDateTimeMaskValue = "d";
		public static MaskInfo GetMaskIfo(MaskAttributeBase mask, string defaulMask, bool defaultNotUseAsDisplayFormat, RegExMaskType? defaultMaskType, bool allowUseMaskAsDisplayFormat) {
			var regExMaskType = GetRegExMaskType(mask, defaultMaskType);
			MaskInfo result = mask != null
				? new MaskInfo(regExMaskType, mask.Mask, mask.IsDefaultMask(mask.Mask), allowUseMaskAsDisplayFormat && mask.UseAsDisplayFormat, mask.CultureInternal, mask.AutomaticallyAdvanceCaretInternal, mask.IgnoreBlankInternal, mask.PlaceHolderInternal, mask.SaveLiteralInternal, mask.ShowPlaceHoldersInternal)
				: new MaskInfo(regExMaskType, null, true, allowUseMaskAsDisplayFormat && !defaultNotUseAsDisplayFormat, null, DefaultAutomaticallyAdvanceCaretValue, DefaultIgnoreBlankValue, DefaultPlaceHolderValue, DefaultSaveLiteralValue, DefaultShowPlaceHoldersValue);
			if(result.IsDefaultMask) {
				result.Mask = defaulMask;
				result.IsDefaultMask = defaultNotUseAsDisplayFormat;
			}
			return result;
		}
		public static RegExMaskType? GetRegExMaskType(MaskAttributeBase mask, RegExMaskType? defaultMaskType) {
			return mask.With(x => x as RegExMaskAttributeBase).Return(x => (RegExMaskType?)x.RegExMaskType, () => defaultMaskType);
		}
		internal MaskInfo(RegExMaskType? regExMaskType, string mask, bool isDefaultMask, bool useAsDisplayFormat, CultureInfo culture, bool automaticallyAdvanceCaret, bool ignoreBlank, char placeHolder, bool saveLiteral, bool showPlaceHolders) {
			this.RegExMaskType = regExMaskType;
			this.Mask = mask;
			this.IsDefaultMask = isDefaultMask;
			this.UseAsDisplayFormat = useAsDisplayFormat;
			this.Culture = culture;
			this.AutomaticallyAdvanceCaret = automaticallyAdvanceCaret;
			this.IgnoreBlank = ignoreBlank;
			this.PlaceHolder = placeHolder;
			this.SaveLiteral = saveLiteral;
			this.ShowPlaceHolders = showPlaceHolders;
		}
		public RegExMaskType? RegExMaskType { get; private set; }
		public string Mask { get; private set; }
		public bool IsDefaultMask { get; private set; }
		public bool UseAsDisplayFormat { get; private set; }
		public CultureInfo Culture { get; private set; }
		public bool AutomaticallyAdvanceCaret { get; private set; }
		public bool IgnoreBlank { get; private set; }
		public char PlaceHolder { get; private set; }
		public bool SaveLiteral { get; private set; }
		public bool ShowPlaceHolders { get; private set; }
		protected bool Equals(MaskInfo other) {
			return RegExMaskType == other.RegExMaskType
				&& string.Equals(Mask, other.Mask)
				&& IsDefaultMask == other.IsDefaultMask
				&& UseAsDisplayFormat == other.UseAsDisplayFormat
				&& Equals(Culture, other.Culture)
				&& AutomaticallyAdvanceCaret == other.AutomaticallyAdvanceCaret
				&& IgnoreBlank == other.IgnoreBlank
				&& PlaceHolder == other.PlaceHolder
				&& SaveLiteral == other.SaveLiteral
				&& ShowPlaceHolders == other.ShowPlaceHolders;
		}
		public override bool Equals(object obj) {
			if(ReferenceEquals(null, obj))
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != this.GetType())
				return false;
			return Equals((MaskInfo)obj);
		}
		public override int GetHashCode() {
			unchecked {
				int hashCode = RegExMaskType.GetHashCode();
				hashCode = (hashCode * 397) ^ (Mask != null ? Mask.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IsDefaultMask.GetHashCode();
				hashCode = (hashCode * 397) ^ UseAsDisplayFormat.GetHashCode();
				hashCode = (hashCode * 397) ^ (Culture != null ? Culture.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ AutomaticallyAdvanceCaret.GetHashCode();
				hashCode = (hashCode * 397) ^ IgnoreBlank.GetHashCode();
				hashCode = (hashCode * 397) ^ PlaceHolder.GetHashCode();
				hashCode = (hashCode * 397) ^ SaveLiteral.GetHashCode();
				hashCode = (hashCode * 397) ^ ShowPlaceHolders.GetHashCode();
				return hashCode;
			}
		}
	}
	public static class NativeMetadataExtensions {
		public static ContextMenuLayoutBuilder<T> ContextMenuLayout<T>(this MetadataBuilder<T> builder) {
			return new ContextMenuLayoutBuilder<T>(builder);
		}
	}
}
