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

using System.Windows;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data;
using System;
using DevExpress.Xpf.GridData;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using System.Linq;
using DevExpress.Xpf.Editors;
using System.Collections.Specialized;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Editors.Filtering;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using System.Windows.Input;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Native {
	public interface IConditionalFormattingClient<T> where T : FrameworkElement, IConditionalFormattingClient<T> {
		ConditionalFormattingHelper<T> FormattingHelper { get; }
		IList<FormatConditionBaseInfo> GetRelatedConditions();
		bool IsSelected { get; }
		FormatValueProvider? GetValueProvider(string fieldName);
		bool IsReady { get; }
		void UpdateBackground();
		void UpdateDataBarFormatInfo(DataBarFormatInfo info);
		Locker Locker { get; }
	}
	public interface IFormatInfoProvider {
		object GetCellValue(string fieldName);
		object GetCellValueByListIndex(int listIndex, string fieldName);
		object GetTotalSummaryValue(string fieldName, ConditionalFormatSummaryType summaryType);
		ValueComparer ValueComparer { get; }
	}
	public static class FormatInfoProviderExtensions {
		public static FormatValueProvider GetValueProvider(this IFormatInfoProvider provider, string fieldName) {
			return new FormatValueProvider(provider, provider.GetCellValue(fieldName), fieldName);
		}
	}
	public struct FormatValueProvider {
		readonly IFormatInfoProvider provider;
		readonly object value;
		readonly string fieldName;
		public FormatValueProvider(IFormatInfoProvider provider, object value, string fieldName) {
			this.provider = provider;
			this.value = value;
			this.fieldName = fieldName;
		}
		public object Value { get { return value; } }
		public ValueComparer ValueComparer { get { return provider.ValueComparer; } }
		public object GetTotalSummaryValue(ConditionalFormatSummaryType summaryType) {
			return provider.GetTotalSummaryValue(fieldName, summaryType);
		}
		public object GetValueByListIndex(int listIndex) {
			return provider.GetCellValueByListIndex(listIndex, fieldName);
		}
		public bool GetSelectiveValue(string name) {
			return FormatConditionBaseInfo.IsFit(provider.GetCellValue(name));
		}
		public object GetCellValue(string name) {
			return provider.GetCellValue(name);
		}
	}
	public static class ConditionalFormatResourceHelper {
		public static readonly string BasePathCore = "pack://application:,,,/" + AssemblyInfo.SRAssemblyXpfCore + ";component/Core/ConditionalFormatting/Images/";
		public static readonly string BasePath = "pack://application:,,,/" + AssemblyInfo.SRAssemblyXpfGridCore + ";component/Grid/ConditionalFormatting/Images/";
		public static readonly string DefaultPathCore = BasePathCore + "IconSets/";
		public static readonly string DefaultPath = BasePath + "IconSets/";
	}
	public enum ConditionalFormatSummaryType {
		Min,
		Max,
		Average,
		SortedList,
	}
	public class ConditionalFormatSummaryInfo {
		static SummaryItemType ToSummaryItemType(ConditionalFormatSummaryType summaryType, Type type) {
			switch(summaryType) {
				case ConditionalFormatSummaryType.Min:
					return SummaryItemType.Min;
				case ConditionalFormatSummaryType.Max:
					return SummaryItemType.Max;
				case ConditionalFormatSummaryType.Average:
					return type != typeof(DateTime) ? SummaryItemType.Average : SummaryItemType.Custom;
				case ConditionalFormatSummaryType.SortedList:
					return SummaryItemType.Custom;
				default:
					throw new InvalidOperationException();
			}
		}
		public ConditionalFormatSummaryInfo(ConditionalFormatSummaryType summaryType, string fieldName) {
			this.SummaryType = summaryType;
			this.FieldName = fieldName ?? string.Empty;
		}
		public ConditionalFormatSummaryType SummaryType { get; private set; }
		public string FieldName { get; private set; }
		public override int GetHashCode() {
			return SummaryType.GetHashCode() ^ FieldName.GetHashCode();
		}
		public override bool Equals(object obj) {
			var other = obj as ConditionalFormatSummaryInfo;
			return other != null && other.FieldName == FieldName && other.SummaryType == SummaryType;
		}
	}
	internal static class DecimalHelper {
		public static decimal AsDecimal(this double value) {
			if(double.IsNegativeInfinity(value))
				return decimal.MinValue;
			if(double.IsPositiveInfinity(value))
				return decimal.MaxValue;
			return (decimal)value;
		}
	}
	public static class InplaceBaseEditHelper {
		public static TextDecorationCollection GetTextDecorations(DependencyObject obj) {
			return (TextDecorationCollection)obj.GetValue(TextDecorationsProperty);
		}
		public static void SetTextDecorations(DependencyObject obj, TextDecorationCollection value) {
			obj.SetValue(TextDecorationsProperty, value);
		}
		public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.RegisterAttached("TextDecorations", typeof(TextDecorationCollection), typeof(InplaceBaseEditHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnTextDecorationsChanged));
		static void OnTextDecorationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DevExpress.Xpf.Editors.Helpers.BaseEditHelper.SetTextDecorations(d, (TextDecorationCollection)e.NewValue);
		}
	}
	public static class ConditionalFormattingMaskHelper {
		public static ConditionalFormatMask GetConditionsMask(IList<FormatConditionBaseInfo> conditions) {
			return conditions != null ? conditions.Select(x => x.FormatMask).Aggregate(ConditionalFormatMask.None, (sum, x) => sum | x) : ConditionalFormatMask.None;
		}
	}
	public class ConditionalFormattingColumnInfo : IColumnInfo {
		readonly Func<string> getExpressionFunc;
		readonly string fieldName = Guid.NewGuid().ToString();
		public ConditionalFormattingColumnInfo(Func<string> getExpressionFunc) {
			this.getExpressionFunc = getExpressionFunc;
		}
		#region IColumnInfo
		string IColumnInfo.FieldName { get { return fieldName; } }
		ColumnSortOrder IColumnInfo.SortOrder { get { return ColumnSortOrder.None; } }
		UnboundColumnType IColumnInfo.UnboundType { get { return UnboundColumnType.Object; } }
		string IColumnInfo.UnboundExpression { get { return getExpressionFunc(); } }
		bool IColumnInfo.ReadOnly { get { return true; } }
		#endregion  
	}
	public class SortedIndices {
		readonly int[] indices;
		public int Count { get { return indices.Length; } }
		public SortedIndices(int[] indices) {
			this.indices = indices;
		}
		public bool IsTopBottomItem(FormatValueProvider provider, int count, bool topItems) {
			if(count == 0)
				return false;
			count = Math.Min(indices.Length, count);
			int index = topItems ? indices.Length - count : count - 1;
			int comparisonResult = provider.ValueComparer.Compare(provider.Value, provider.GetValueByListIndex(indices[index]));
			if(topItems)
				return comparisonResult >= 0;
			else
				return comparisonResult <= 0;
		}
	}
	[Flags]
	public enum ConditionalFormatMask {
		None = 0,
		DataBarOrIcon = 1 << 0,
		Background = 1 << 1,
		Foreground = 1 << 2,
		FontSize = 1 << 3,
		FontStyle = 1 << 4,
		FontFamily = 1 << 5,
		FontStretch = 1 << 6,
		FontWeight = 1 << 7,
		TextDecorations = 1 << 8,
	}
	[Flags]
	public enum FormatConditionChangeType {
		AppearanceOnly = 0,
		UnboundColumn = 1 << 0,
		Summary = 1 << 1,
		All = UnboundColumn | Summary,
	}
	public interface IConditionalFormattingCommands {
		ICommand ShowLessThanFormatConditionDialog { get; }
		ICommand ShowGreaterThanFormatConditionDialog { get; }
		ICommand ShowEqualToFormatConditionDialog { get; }
		ICommand ShowBetweenFormatConditionDialog { get; }
		ICommand ShowTextThatContainsFormatConditionDialog { get; }
		ICommand ShowADateOccurringFormatConditionDialog { get; }
		ICommand ShowCustomConditionFormatConditionDialog { get; }
		ICommand ShowTop10ItemsFormatConditionDialog { get; }
		ICommand ShowBottom10ItemsFormatConditionDialog { get; }
		ICommand ShowTop10PercentFormatConditionDialog { get; }
		ICommand ShowBottom10PercentFormatConditionDialog { get; }
		ICommand ShowAboveAverageFormatConditionDialog { get; }
		ICommand ShowBelowAverageFormatConditionDialog { get; }
		ICommand ClearFormatConditionsFromAllColumns { get; }
		ICommand ClearFormatConditionsFromColumn { get; }
		ICommand ShowConditionalFormattingManager { get; }
		ICommand AddFormatCondition { get; }
	}
	public interface IDialogContext {
		IDataColumnInfo ColumnInfo { get; }
		FilterColumn FilterColumn { get; }
		IFilteredComponent FilteredComponent { get; }
		IFormatsOwner PredefinedFormatsOwner { get; }
		IModelProperty Conditions { get; }
		IConditionModelItemsBuilder Builder { get; }
		IDialogContext Find(string name);
		IEditingContext EditingContext { get; }
		IModelItem CreateModelItem(object obj);
		IModelItem GetRootModelItem();
		bool IsDesignTime { get; }
		bool IsPivot { get; }
		string ApplyToFieldNameCaption { get; }
		string ApplyToPivotRowCaption { get; }
		string ApplyToPivotColumnCaption { get; }
		IEnumerable<FieldNameWrapper> PivotSpecialFieldNames { get; }
	}
	public interface IFormatsOwner {
		FormatInfoCollection PredefinedFormats { get; set; }
		FormatInfoCollection PredefinedColorScaleFormats { get; set; }
		FormatInfoCollection PredefinedDataBarFormats { get; set; }
		FormatInfoCollection PredefinedIconSetFormats { get; set; }
	}
	public static class FormatConditionDesignDialogHelper {
		public static IEnumerable<DesignFormatInfo> GetDataBarDesignInfo() {
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_BlueGradientDataBar, "BlueGradientDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_GreenGradientDataBar, "GreenGradientDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_RedGradientDataBar, "RedGradientDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_OrangeGradientDataBar, "OrangeGradientDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_LightBlueGradientDataBar, "LightBlueGradientDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_PurpleGradientDataBar, "PurpleGradientDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_BlueSolidDataBar, "BlueSolidDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_GreenSolidDataBar, "GreenSolidDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_RedSolidDataBar, "RedSolidDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_OrangeSolidDataBar, "OrangeSolidDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_LightBlueSolidDataBar, "LightBlueSolidDataBar");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedDataBarFormat_PurpleSolidDataBar, "PurpleSolidDataBar");
		}
		public static IEnumerable<DesignFormatInfo> GetColorScaleDesignInfo() {
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_GreenYellowRedColorScale, "GreenYellowRedColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_RedYellowGreenColorScale, "RedYellowGreenColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_GreenWhiteRedColorScale, "GreenWhiteRedColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteGreenColorScale, "RedWhiteGreenColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_BlueWhiteRedColorScale, "BlueWhiteRedColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteBlueColorScale, "RedWhiteBlueColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_WhiteRedColorScale, "WhiteRedColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_RedWhiteColorScale, "RedWhiteColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_GreenWhiteColorScale, "GreenWhiteColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_WhiteGreenColorScale, "WhiteGreenColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_GreenYellowColorScale, "GreenYellowColorScale");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedColorScaleFormat_YellowGreenColorScale, "YellowGreenColorScale");
		}
		public static IEnumerable<DesignFormatInfo> GetIconSetDesignInfo() {
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows3ColoredIconSet, "Arrows3ColoredIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows3GrayIconSet, "Arrows3GrayIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Triangles3IconSet, "Triangles3IconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows4GrayIconSet, "Arrows4GrayIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows4ColoredIconSet, "Arrows4ColoredIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows5GrayIconSet, "Arrows5GrayIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Arrows5ColoredIconSet, "Arrows5ColoredIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_TrafficLights3UnrimmedIconSet, "TrafficLights3UnrimmedIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_TrafficLights3RimmedIconSet, "TrafficLights3RimmedIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Signs3IconSet, "Signs3IconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_TrafficLights4IconSet, "TrafficLights4IconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_RedToBlackIconSet, "RedToBlackIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Symbols3CircledIconSet, "Symbols3CircledIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Symbols3UncircledIconSet, "Symbols3UncircledIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Flags3IconSet, "Flags3IconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Stars3IconSet, "Stars3IconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Ratings4IconSet, "Ratings4IconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Quarters5IconSet, "Quarters5IconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Ratings5IconSet, "Ratings5IconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_Boxes5IconSet, "Boxes5IconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeArrowsColoredIconSet, "PositiveNegativeArrowsColoredIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeArrowsGrayIconSet, "PositiveNegativeArrowsGrayIconSet");
			yield return new DesignFormatInfo(ConditionalFormattingStringId.ConditionalFormatting_PredefinedIconSetFormat_PositiveNegativeTrianglesIconSet, "PositiveNegativeTrianglesIconSet");
		}
	}
	public struct DesignFormatInfo {
		ConditionalFormattingStringId nameID;
		string formatName;
		public ConditionalFormattingStringId NameID { get { return nameID; } }
		public string FormatName { get { return formatName; } }
		public DesignFormatInfo(ConditionalFormattingStringId nameID, string formatName) {
			this.nameID = nameID;
			this.formatName = formatName;
		}
	}
	public class DynamicConditionalFormattingResourceExtension : ThemeResourceExtensionBase {
		const string coreAssemblyName = XmlNamespaceConstants.XpfCoreNamespace;
		const string inThemePath = coreAssemblyName + "/" + coreAssemblyName + "/";
		const string defaultThemeName = Theme.DeepBlueName;
		const string themesPrefix = "DevExpress.Xpf.Themes";
		protected string ThemeName { get; set; }
		protected sealed override string Namespace {
			get { return ThemeName != null ? String.Format("{0}.{1}", themesPrefix, ThemeName) : coreAssemblyName; }
		}
		public DynamicConditionalFormattingResourceExtension(string resourcePath) : base(resourcePath) { }
		protected override string GetResourcePath(IServiceProvider serviceProvider) {
			ThemeName = CorrectThemeName(serviceProvider);
			return string.Format("{0}Themes/{1}/{2}", ThemeName == null ? "" : inThemePath, ThemeName ?? defaultThemeName, ResourcePath);
		}
		public static string CorrectThemeName(IServiceProvider serviceProvider) {
			string themeName = DevExpress.Xpf.Utils.Themes.ThemeNameHelper.GetThemeName(serviceProvider);
			if(string.IsNullOrEmpty(themeName) || themeName == Theme.MetropolisLightName)
				return null;
			return themeName;
		}
	}
#if DEBUGTEST
	public static class TestAdapterExtensions {
		public static ConditionalFormatMask FormatMask(this Format format) {
			return format.FormatMask;
		}
		public static IconSetElement[] GetSortedElements(this IconSetFormat format) {
			return format.GetSortedElements();
		}
		public static IconSetElement GetElement(this IconSetFormat format, FormatValueProvider provider, decimal? minValue, decimal? maxValue) {
			return format.GetElement(provider, minValue, maxValue);
		}
	}
#endif
}
