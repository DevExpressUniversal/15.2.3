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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Utils.Themes;
using System.Windows;
using System.Windows.Media;
using DevExpress.Export.Xl;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Themes {
	public enum ConditionalFormattingThemeKeys {
		PredefinedFormats,
		PredefinedColorScaleFormats,
		PredefinedDataBarFormats,
		PredefinedIconSetFormats,
		ApplyFormatConditionDialogTemplate,
		ConditionalFormattingManagerTemplate,
		ColorScaleMenuItemContent,
		DataBarMenuItemContent,
		IconSetMenuItemContent,
	}
	public class ConditionalFormattingThemeKeyExtension : ThemeKeyExtensionBase<ConditionalFormattingThemeKeys> { }
	public class FormatThemeKeyExtension : ThemeKeyExtensionBase<string> {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			string themeName = DynamicConditionalFormattingResourceExtension.CorrectThemeName(serviceProvider);
			if(!string.IsNullOrEmpty(themeName))
				ThemeName = themeName;
			return this;
		}
	}
	public class StandardFormatInfoExtension : MarkupExtension {
		public string FormatName { get; set; }
		public StandardFormatInfoExtension() { }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			var key = (new FormatThemeKeyExtension() { ResourceKey = FormatName }).ProvideValue(serviceProvider);
			var formatInfo = new FormatInfo() {
				FormatName = this.FormatName,
				Format = new StaticResourceExtension(key).ProvideValue(serviceProvider),
				DisplayName = ConditionalFormattingLocalizer.GetString(GetStringIdPrefix() + FormatName),
				Icon = GetIcon(),
				Description = GetDescription(),
				GroupName = GetGroupName(),
			};
			formatInfo.Freeze();
			return formatInfo;
		}
		protected virtual string GetGroupName() { return null; }
		protected virtual string GetStringIdPrefix() { return "ConditionalFormatting_PredefinedFormat_"; }
		protected virtual string GetDescription() { return null; }
		protected virtual ImageSource GetIcon() { return null; }
	}
	public class StandardIndicatorFormatInfoExtension : StandardFormatInfoExtension {
		public ImageSource Icon { get; set; }
		protected override ImageSource GetIcon() {
			return Icon;
		}
	}
	public class StandardColorScaleFormatInfoExtension : StandardIndicatorFormatInfoExtension {
		protected override string GetDescription() {
			return ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_Description);
		}
		protected override string GetStringIdPrefix() {
			return "ConditionalFormatting_PredefinedColorScaleFormat_";
		}
	}
	public enum StandardDataBarFormatGroup {
		GradientFillGroup,
		SolidFillGroup,
	}
	public class StandardDataBarFormatInfoExtension : StandardIndicatorFormatInfoExtension {
		public StandardDataBarFormatGroup Group { get; set; }
		protected override string GetGroupName() {
			return ConditionalFormattingLocalizer.GetString(GetStringIdPrefix() + Group.ToString());
		}
		protected override string GetDescription() {
			return ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_Description);
		}
		protected override string GetStringIdPrefix() {
			return "ConditionalFormatting_PredefinedDataBarFormat_";
		}
	}
	public enum StandardIconSetFormatGroup {
		DirectionalGroup,
		ShapesGroup,
		IndicatorsGroup,
		RatingsGroup,
		PositiveNegativeGroup,
	}
	public class StandardIconSetFormatInfoExtension : StandardIndicatorFormatInfoExtension {
		public StandardIconSetFormatGroup Group { get; set; }
		protected override string GetGroupName() {
			return ConditionalFormattingLocalizer.GetString(GetStringIdPrefix() + Group.ToString());
		}
		protected override string GetDescription() {
			return ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Description);
		}
		protected override string GetStringIdPrefix() {
			return "ConditionalFormatting_PredefinedIconSetFormat_";
		}
	}
	public class QuickIconSetFormatExtension : MarkupExtension {
		public static string DefaultPath { get { return ConditionalFormatResourceHelper.DefaultPathCore; } }
		public int ElementCount { get; set; }
		public string Path { get; set; }
		public string Name { get; set; }
		public XlCondFmtIconSetType? XlIconSetType { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			var format = new IconSetFormat();
			TryPopulate(format);
			format.Freeze();
			return format;
		}
		void TryPopulate(IconSetFormat format) {
			if(ElementCount == 0 || string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(Name))
				return;
			foreach(var index in Enumerable.Range(0, ElementCount)) {
				var element = new IconSetElement() {
					Threshold = 100d * (ElementCount - 1 - index) / ElementCount,
					Icon = new BitmapImage(new Uri(DefaultPath + Name + (index + 1).ToString() + ".png", UriKind.Absolute)),
				};
				format.Elements.Add(element);
			}
			format.IconSetType = XlIconSetType;
		}
	}
	public abstract class ConditionalFormattingIconExtensionBase : MarkupExtension {
		protected virtual string IconPrefix { get { return string.Empty; } }
		public string IconName { get; set; }
		public ConditionalFormattingIconExtensionBase(string iconName) {
			IconName = iconName;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new BitmapImage(new Uri(ConditionalFormatResourceHelper.BasePathCore + IconPrefix + IconName, UriKind.Absolute));
		}
	}
	public class ConditionalFormattingMenuIconExtension : ConditionalFormattingIconExtensionBase {
		protected override string IconPrefix { get { return "Menu/ConditionalFormatting"; } }
		public ConditionalFormattingMenuIconExtension(string iconName) : base(iconName) { }
	}
	public class ConditionalFormattingIconSetIconExtension : ConditionalFormattingIconExtensionBase {
		protected override string IconPrefix { get { return "IconSets/"; } }
		public ConditionalFormattingIconSetIconExtension(string iconName) : base(iconName) { }
		public ConditionalFormattingIconSetIconExtension() : base(string.Empty) { }
	}
}
