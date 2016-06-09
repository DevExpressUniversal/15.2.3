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

namespace DevExpress.DashboardCommon.Native {
	public static class TypeNames {
		const string Prefix = "DevExpress.DashboardWin.Design.";
		const string Postfix = "," + AssemblyInfo.SRAssemblyDashboardWinDesign;
		const string PrefixNative = "DevExpress.DashboardWin.Native.";
		const string PostfixNative = "," + AssemblyInfo.SRAssemblyDashboardWin;
		public const string CurrencyEditor = Prefix + "CurrencyEditor" + Postfix;
		public const string CalculatedFieldExpressionEditor = Prefix + "CalculatedFieldExpressionEditor" + Postfix;
		public const string DashboardSourceConvertor = Prefix + "DashboardSourceConvertor" + Postfix;
		public const string DashboardSourceEditor = Prefix + "DashboardSourceEditor" + Postfix;
		public const string DashboardItemCollectionEditor = Prefix + "DashboardItemCollectionEditor" + Postfix;
		public const string DashboardItemGroupCollectionEditor = Prefix + "DashboardItemGroupCollectionEditor" + Postfix;
		public const string DashboardItemFilterCriteriaEditor = Prefix + "DashboardItemFilterCriteriaEditor" + Postfix;
		public const string DashboardParameterCollectionEditor = PrefixNative + "DashboardParameterCollectionEditor" + PostfixNative;
		public const string DataSourceCollectionEditor = Prefix + "DataSourceCollectionEditor" + Postfix;
		public const string DisplayNameObjectConverter = Prefix + "DisplayNameObjectConverter" + Postfix;
		public const string DefaultCollectionEditor = "DevExpress.Utils.UI." + "CollectionEditor," + AssemblyInfo.SRAssemblyUtilsUI;
		public const string DisplayNameNoneObjectConverter = Prefix + "DisplayNameNoneObjectConverter" + Postfix;
		public const string GridColumnCollectionEditor = Prefix + "GridColumnCollectionEditor" + Postfix;
		public const string ChoroplethMapCollectionEditor = Prefix + "ChoroplethMapCollectionEditor" + Postfix;
		public const string ChartPanesCollectionEditor = Prefix + "ChartPanesCollectionEditor" + Postfix;
		public const string ImageFileNameEditor = Prefix + "ImageFileNameEditor" + Postfix;
		public const string ImageDataEditor = Prefix + "ImageDataEditor" + Postfix;
		public const string RtfEditor = Prefix + "RtfEditor" + Postfix;
		public const string TypeListSelectorEditor = Prefix + "TypeListSelectorEditor" + Postfix;
		public const string NotifyingCollectionEditor = Prefix + "NotifyingCollectionEditor" + Postfix;
		public const string ChartSeriesCollectionEditor =  Prefix + "ChartSeriesCollectionEditor" + Postfix;
		public const string RangeFilterSeriesCollectionEditor =  Prefix + "RangeFilterSeriesCollectionEditor" + Postfix;
		public const string DataProcessingModeEditor = Prefix + "DataProcessingModeEditor" + Postfix;
		public const string DashboardColorSchemeEditor = Prefix + "DashboardColorSchemeEditor" + Postfix;
		public const string DashboardItemColorSchemeEditor = Prefix + "DashboardItemColorSchemeEditor" + Postfix;
		public const string FormatRulesManageEditor = Prefix + "FormatRulesManageEditor" + Postfix;
		public const string DataProcessingModeConverter = Prefix + "DataProcessingModeConverter" + Postfix;
		public const string CreatableMapPaletteConverter = Prefix + "CreatableMapPaletteConverter" + Postfix;
		public const string CreatableMapScaleConverter = Prefix + "CreatableMapScaleConverter" + Postfix;
		public const string CreatableDimensionPropertyTypeConverter = Prefix + "CreatableDimensionPropertyTypeConverter" + Postfix;
		public const string CreatableMeasurePropertyTypeConverter = Prefix + "CreatableMeasurePropertyTypeConverter" + Postfix;
		public const string CreatableGeoPointMapTypeConverter = Prefix + "CreatableGeoPointMapTypeConverter" + Postfix;
		public const string CodeDomSerializer = "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design";
		public const string DataItemContainerCodeDomSerializer = Prefix + "DataItemContainerCodeDomSerializer" + Postfix;
		public const string DataItemRepositoryCodeDomSerializer = Prefix + "DataItemRepositoryCodeDomSerializer" + Postfix;
		public const string ColorDefinitionCodeDomSerializer = Prefix + "ColorDefinitionCodeDomSerializer" + Postfix;
		public const string DimensionDefinitionCodeDomSerializer = Prefix + "DimensionDefinitionCodeDomSerializer" + Postfix;
		public const string MeasureDefinitionCodeDomSerializer = Prefix + "MeasureDefinitionCodeDomSerializer" + Postfix;
		public const string ColorSchemeDimensionKeyCodeDomSerializer = Prefix + "ColorSchemeDimensionKeyCodeDomSerializer" + Postfix;
		public const string ColorSchemeMeasureKeyCodeDomSerializer = Prefix + "ColorSchemeMeasureKeyCodeDomSerializer" + Postfix;
		public const string ISupportInitializeCodeDomSerializer = Prefix + "ISupportInitializeCodeDomSerializer" + Postfix;
		public const string DashboardCodeSerializer = Prefix + "DashboardCodeDomSerializer" + Postfix;
		public const string TypeCodeDomSerializer = "System.ComponentModel.Design.Serialization.TypeCodeDomSerializer, System.Design";
		public const string DashboardComponentDesigner = Prefix + "DashboardComponentDesigner" + Postfix;
		public const string SimpleSeriesSeriesTypeConverter = Prefix + "SimpleSeriesSeriesTypeConverter" + Postfix;
		public const string SimpleSeriesConverter = Prefix + "SimpleSeriesConverter" + Postfix;
		public const string MeasureConverter = Prefix + "MeasureConverter" + Postfix;
		public const string DimensionConverter = Prefix + "DimensionConverter" + Postfix;
		public const string DashboardOlapDataSourceComponentDesigner = Prefix + "DashboardOlapDataSourceComponentDesigner" + Postfix;
		public const string DashboardItemGroupListConverter = Prefix + "DashboardItemGroupListConverter" + Postfix;
		public const string DashboardItemDataMemberListConverter = Prefix + "DashboardItemDataMemberListConverter" + Postfix;
		public const string CalculatedFieldDataMemberListConverter = Prefix + "CalculatedFieldDataMemberListConverter" + Postfix;
		public const string ListSelectorEditor = PrefixNative + "ListSelectorEditor" + PostfixNative;
		public const string DataSourceListConverter = PrefixNative + "DataSourceListConverter" + PostfixNative;
		public const string DataItemDataMemberEditor = PrefixNative + "DataItemDataMemberSelectorEditor" + PostfixNative;
		public const string ParameterLookUpSettingsEditor = PrefixNative + "ParameterLookUpSettingsEditor" + PostfixNative;
		public const string ParameterLookUpSettingsConverter = PrefixNative + "ParameterLookUpSettingsConverter" + PostfixNative;
		public const string LookUpSettingsDataSourceListConverter = PrefixNative + "LookUpSettingsDataSourceListConverter" + PostfixNative;
		public const string LookUpSettingsDataMemberListConverter = PrefixNative + "LookUpSettingsDataMemberListConverter" + PostfixNative;
		public const string LookUpSettingsDataMemberSelectorEditor = PrefixNative + "LookUpSettingsDataMemberSelectorEditor" + PostfixNative;
	}
	static class CategoryNames {
		public const string Behavior = "Behavior";
		public const string Data = "Data";
		public const string Connection = "Connection";
		public const string Format = "Format";
		public const string General = "General";
		public const string Interactivity = "Interactivity";
		public const string Layout = "Layout";
		public const string Style = "Style";
		public const string Elements = "Elements";
	}
}
