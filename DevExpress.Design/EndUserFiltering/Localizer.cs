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

namespace DevExpress.Design.Filtering {
	using DevExpress.Utils.Localization;
	using DevExpress.Utils.Localization.Internal;
	#region FilteringModelConfiguratorLocalizerStringId
	public enum FilteringModelConfiguratorLocalizerStringId {
		ConfiguratorTitle,
		ConfiguratorTitleXAML,
		ModelTypeCaption,
		ModelTypePrompt,
		BackCaption,
		EditCaption,
		FinishCaption,
		CaptionMetricPropertyName,
		CaptionMetricPropertyDescription,
		CaptionMetricPropertyPrompt,
		EditorTypeMetricPropertyName,
		EditorTypeMetricPropertyDescription,
		DescriptionMetricPropertyName,
		DescriptionMetricPropertyDescription,
		DescriptionMetricPropertyPrompt,
		RangeMetricEditorTypeName,
		RangeMetricEditorTypeDescription,
		LookupMetricEditorTypeName,
		LookupMetricEditorTypeDescription,
		BooleanChoiceMetricEditorTypeName,
		BooleanChoiceMetricEditorTypeDescription,
		EnumChoiceMetricEditorTypeName,
		EnumChoiceMetricEditorTypeDescription,
		NewPropertyCaption,
		NoPropertiesAvailableText,
	}
	#endregion FilteringLocalizerStringId
	public class FilteringModelConfiguratorLocalizer : XtraLocalizer<FilteringModelConfiguratorLocalizerStringId> {
		#region static
		static FilteringModelConfiguratorLocalizer() {
			SetActiveLocalizerProvider(
					new DefaultActiveLocalizerProvider<FilteringModelConfiguratorLocalizerStringId>(CreateDefaultLocalizer())
				);
		}
		public static XtraLocalizer<FilteringModelConfiguratorLocalizerStringId> CreateDefaultLocalizer() {
			return new FilteringModelConfiguratorResXLocalizer();
		}
		public new static XtraLocalizer<FilteringModelConfiguratorLocalizerStringId> Active {
			get { return XtraLocalizer<FilteringModelConfiguratorLocalizerStringId>.Active; }
			set { XtraLocalizer<FilteringModelConfiguratorLocalizerStringId>.Active = value; }
		}
		public static string GetString(FilteringModelConfiguratorLocalizerStringId id) {
			return Active.GetLocalizedString(id);
		}
		#endregion static
		public override XtraLocalizer<FilteringModelConfiguratorLocalizerStringId> CreateResXLocalizer() {
			return new FilteringModelConfiguratorResXLocalizer();
		}
		protected override void PopulateStringTable() {
			#region AddString
			AddString(FilteringModelConfiguratorLocalizerStringId.ConfiguratorTitle, "Filtering Model Configuration Wizard");
			AddString(FilteringModelConfiguratorLocalizerStringId.ConfiguratorTitleXAML, "Filtering Model Configuration Wizard");
			AddString(FilteringModelConfiguratorLocalizerStringId.ModelTypeCaption, "Model Type");
			AddString(FilteringModelConfiguratorLocalizerStringId.ModelTypePrompt, "Start typing Model Type");
			AddString(FilteringModelConfiguratorLocalizerStringId.BackCaption, "Back");
			AddString(FilteringModelConfiguratorLocalizerStringId.EditCaption, "Edit...");
			AddString(FilteringModelConfiguratorLocalizerStringId.FinishCaption, "Finish");
			AddString(FilteringModelConfiguratorLocalizerStringId.CaptionMetricPropertyName, "Caption");
			AddString(FilteringModelConfiguratorLocalizerStringId.CaptionMetricPropertyDescription, "Caption Description");
			AddString(FilteringModelConfiguratorLocalizerStringId.CaptionMetricPropertyPrompt, "Enter Caption");
			AddString(FilteringModelConfiguratorLocalizerStringId.EditorTypeMetricPropertyName, "EditorType");
			AddString(FilteringModelConfiguratorLocalizerStringId.EditorTypeMetricPropertyDescription, "EditorType Description");
			AddString(FilteringModelConfiguratorLocalizerStringId.DescriptionMetricPropertyName, "Description");
			AddString(FilteringModelConfiguratorLocalizerStringId.DescriptionMetricPropertyDescription, "Description Description");
			AddString(FilteringModelConfiguratorLocalizerStringId.DescriptionMetricPropertyPrompt, "Enter Description");
			AddString(FilteringModelConfiguratorLocalizerStringId.RangeMetricEditorTypeName, "Range");
			AddString(FilteringModelConfiguratorLocalizerStringId.RangeMetricEditorTypeDescription, "Range Description");
			AddString(FilteringModelConfiguratorLocalizerStringId.LookupMetricEditorTypeName, "Lookup");
			AddString(FilteringModelConfiguratorLocalizerStringId.LookupMetricEditorTypeDescription, "Lookup Description");
			AddString(FilteringModelConfiguratorLocalizerStringId.BooleanChoiceMetricEditorTypeName, "Boolean Choice");
			AddString(FilteringModelConfiguratorLocalizerStringId.BooleanChoiceMetricEditorTypeDescription, "Boolean Choice Description");
			AddString(FilteringModelConfiguratorLocalizerStringId.EnumChoiceMetricEditorTypeName, "Enum Choice");
			AddString(FilteringModelConfiguratorLocalizerStringId.EnumChoiceMetricEditorTypeDescription, "Enum Choice Description");
			AddString(FilteringModelConfiguratorLocalizerStringId.NewPropertyCaption, "New Column...");
			AddString(FilteringModelConfiguratorLocalizerStringId.NoPropertiesAvailableText, "Choose the available ModelType or click the 'New' button to create a new column");
			#endregion AddString
		}
	}
	public class FilteringModelConfiguratorResXLocalizer : XtraResXLocalizer<FilteringModelConfiguratorLocalizerStringId> {
		const string baseName = "DevExpress.Design.EndUserFiltering.LocalizationRes";
		public FilteringModelConfiguratorResXLocalizer()
			: base(new FilteringModelConfiguratorLocalizer()) {
		}
		protected override System.Resources.ResourceManager CreateResourceManagerCore() {
			return new System.Resources.ResourceManager(baseName, typeof(FilteringModelConfiguratorResXLocalizer).Assembly);
		}
	}
}
namespace DevExpress.Design.Filtering.UI {
	public class FilteringModelConfiguratorLocalizerStringIdExtension : System.Windows.Markup.MarkupExtension {
		public FilteringModelConfiguratorLocalizerStringId ID { get; set; }
		public override object ProvideValue(System.IServiceProvider serviceProvider) {
			return FilteringModelConfiguratorLocalizer.GetString(ID);
		}
	}
}
