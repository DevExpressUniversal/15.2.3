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

extern alias Platform;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::System.Windows;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Printing.Design.LayoutCreators;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::System.Windows.Data;
using Microsoft.Windows.Design.Metadata;
using Platform::DevExpress.Xpf.Printing.Native;
using Platform::System.Windows.Controls;
using Platform::DevExpress.Xpf.Core;
using System.Windows.Shapes;
using System.Windows.Media;
using Platform::DevExpress.Xpf.Printing;
using Platform::DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Printing.Design.Bars {
	public static class BarItemInfos {
		static readonly BarItemInfo buttonItem = new BarButtonItemInfo();
		static readonly BarItemInfo checkItem = new BarCheckItemInfo();
		static readonly BarItemInfo barStaticItem = new ZoomFactorItemInfo();
		public static BarItemInfo BarButtonItem { get { return buttonItem; } }
		public static BarItemInfo BarCheckItem { get { return checkItem; } }
	}
	public class ZoomFactorItemInfo : BarItemInfo {
		public override ModelItem CreateItem(Microsoft.Windows.Design.EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarStaticItem));
		}
		public override void SetupItem(CommandBarXamlCreator creator, System.Text.StringBuilder writer, string command, string masterControlName) { }
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			item.Properties["AutoSizeMode"].SetValue(BarItemAutoSizeMode.Fill);
			item.Properties["ContentAlignment"].SetValue(HorizontalAlignment.Right);
			item.Properties["ShowBorder"].SetValue(false);
			var contentBinding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			contentBinding.Properties["Path"].SetValue("Model.ZoomDisplayText");
			contentBinding.Properties["ElementName"].SetValue(masterControl.Name);
			item.Properties["Content"].SetValue(contentBinding);
		}
		public override string XamlItemLinkTag {
			get { return typeof(BarStaticItemLink).Name; }
		}
		public override string XamlItemTag {
			get { return typeof(BarStaticItem).Name; }
		}
		public override string XamlPrefix {
			get { return XmlNamespaceConstants.BarsPrefix; }
		}
	}
	public class PageNumberItemInfo : BarItemInfo {
		ModelItem masterControl;
		public override ModelItem CreateItem(Microsoft.Windows.Design.EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarEditItem));
		}
		public override void SetupItem(CommandBarXamlCreator creator, System.Text.StringBuilder writer, string command, string masterControlName) { }
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			this.masterControl = masterControl;
			SetupEditor(creator, item);
			var converter = ResourceManager.CreateConverter<FormatStringConverter>(item, ResourceKeys.FormatStringConverter, "/ {0}");
			var staticResourceBinding = ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.FormatStringConverter);
			var content2ItemBinding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			content2ItemBinding.Properties["Path"].SetValue("Model.PageCount");
			content2ItemBinding.Properties["FallbackValue"].SetValue(" / 0");
			content2ItemBinding.Properties["ElementName"].SetValue(masterControl.Name);
			content2ItemBinding.Properties["Converter"].SetValue(staticResourceBinding);
			var editValueBinding = ModelFactory.CreateItem(masterControl.Context, typeof(Binding));
			editValueBinding.Properties["ElementName"].SetValue(masterControl.Name);
			editValueBinding.Properties["Path"].SetValue("Model.CurrentPageNumber");
			editValueBinding.Properties["UpdateSourceTrigger"].SetValue(UpdateSourceTrigger.Explicit);
			editValueBinding.Properties["Mode"].SetValue("TwoWay");
			item.Properties["EditValue"].SetValue(editValueBinding);
			item.Properties["Content2"].SetValue(content2ItemBinding);
		}
		void SetupEditor(CommandBarCreator creator, ModelItem item) {
			CreateEditSettings(creator, item);
			item.Properties["EditStyle"].SetValue(ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.PageNumberEditStyle));
		}
		void CreateEditSettings(CommandBarCreator creator, ModelItem item) {
			var settings = ModelFactory.CreateItem(item.Context, typeof(TextEditSettings));
			settings.Properties["HorizontalContentAlignment"].SetValue(HorizontalAlignment.Center);
			settings.Properties["MaskType"].SetValue(MaskType.RegEx);
			settings.Properties["Mask"].SetValue(@"\d{0,6}");
			settings.Properties["AllowNullInput"].SetValue(true);
			item.Properties["EditSettings"].SetValue(settings);
			var style = ModelFactory.CreateItem(item.Context, typeof(Style));
			style.Properties["TargetType"].SetValue(typeof(TextEdit));
			var minWidthSetter = ModelFactory.CreateItem(item.Context, typeof(Setter));
#if !SILVERLIGHT
			minWidthSetter.Properties["Property"].SetValue(TextEdit.MinWidthProperty);
#else
			minWidthSetter.Properties["Property"].SetValue("MinWidth");
#endif
			minWidthSetter.Properties["Value"].SetValue(30d);
			style.Properties["Setters"].Collection.Add(minWidthSetter);
			var paddingSetter = ModelFactory.CreateItem(item.Context, typeof(Setter));
#if !SILVERLIGHT
			paddingSetter.Properties["Property"].SetValue(TextEdit.PaddingProperty);
#else
			paddingSetter.Properties["Property"].SetValue("Padding");
#endif
			paddingSetter.Properties["Value"].SetValue(new Thickness(0,-2,0,-2));
			style.Properties["Setters"].Collection.Add(paddingSetter);
			ResourceManager.AddResource(style, ResourceKeys.PageNumberEditStyle);
			CreateBehavior(item);
		}
		void CreateBehavior(ModelItem item) {
			var behaviorsProperty = new PropertyIdentifier(typeof(Interaction), "Behaviors");
			var behaviorsCollection = item.Properties[behaviorsProperty].Collection;
			ModelItem behavior = ModelFactory.CreateItem(masterControl.Context, typeof(PageNumberBehavior));
			var focusItem = ModelFactory.CreateItem(masterControl.Context, typeof(Binding));
			focusItem.Properties["ElementName"].SetValue(masterControl.Name);
			behavior.Properties["FocusTarget"].SetValue(focusItem);
#if SILVERLIGHT
			var revertValueItem = ModelFactory.CreateItem(masterControl.Context, typeof(Binding));
			revertValueItem.Properties["ElementName"].SetValue(masterControl.Name);
			revertValueItem.Properties["Path"].SetValue("Model.CurrentPageNumber");
			behavior.Properties["RevertValue"].SetValue(revertValueItem);
#endif
			behaviorsCollection.Add(behavior);
		}
		public override string XamlItemLinkTag {
			get { return typeof(BarEditItemLink).Name; }
		}
		public override string XamlItemTag {
			get { return typeof(BarEditItem).Name; }
		}
		public override string XamlPrefix {
			get { return XmlNamespaceConstants.BarsPrefix; }
		}
	}
	public class ProgressItemInfo : BarItemInfo {
		#region inner classes
		public enum ProgressBarType {
			Normal = 0,
			Indeterminate = 1
		}
		#endregion
		readonly ProgressBarType progressBarType;
		string masterControlName;
		public override ModelItem CreateItem(Microsoft.Windows.Design.EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarEditItem));
		}
		public ProgressItemInfo(ProgressBarType type) {
			this.progressBarType = type;
		}
		public override void SetupItem(CommandBarXamlCreator creator, System.Text.StringBuilder writer, string command, string masterControlName) { }
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			this.masterControlName = masterControl.Name;
			SetupEditor(item);
		}
		void SetupEditor(ModelItem item) {
			if(progressBarType == ProgressBarType.Normal)
				CreateProgressBarSettings(item);
			else
				CreateMarqueeProgressBarSettings(item);
		}
		void CreateMarqueeProgressBarSettings(ModelItem item) {
				var dataTemplate = ModelFactory.CreateItem(item.Context, typeof(DataTemplate));
				var progressBarItem = ModelFactory.CreateItem(item.Context, typeof(ProgressBarEdit));
				progressBarItem.Properties["MinWidth"].SetValue(150d);
				progressBarItem.Properties["MinHeight"].SetValue(12d);
				progressBarItem.Properties["Height"].SetValue(12d);
				progressBarItem.Properties["StyleSettings"].SetValue(ModelFactory.CreateItem(item.Context, typeof(ProgressBarMarqueeStyleSettings)));
				dataTemplate.Properties["VisualTree"].SetValue(progressBarItem);
				ResourceManager.AddResource(dataTemplate, ResourceKeys.ProgressBarMarqueeTemplate);
				item.Properties["EditTemplate"].SetValue(ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.ProgressBarMarqueeTemplate));
		}
		void CreateProgressBarSettings(ModelItem item) {
			CreateEditStyle(item);
			item.Properties["EditStyle"].SetValue(ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.ProgressBarEditStyle));
			ModelItem editSettingsItem = editSettingsItem = ModelFactory.CreateItem(item.Context, typeof(ProgressBarEditSettings));
			editSettingsItem.Properties["Minimum"].SetValue(0);
			var maximumBinding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			maximumBinding.Properties["Path"].SetValue("Model.ProgressMaximum");
			maximumBinding.Properties["ElementName"].SetValue(masterControlName);
			editSettingsItem.Properties["Maximum"].SetValue(maximumBinding);
			item.Properties["EditSettings"].SetValue(editSettingsItem);
		}
		void CreateEditStyle(ModelItem item) {
			var styleItem = ModelFactory.CreateItem(item.Context, typeof(Style));
			styleItem.Properties["TargetType"].SetValue(typeof(ProgressBarEdit));
			var minWidthSetter = ModelFactory.CreateItem(item.Context, typeof(Setter));
#if !SILVERLIGHT
			minWidthSetter.Properties["Property"].SetValue(TextEdit.MinWidthProperty);
#else
			minWidthSetter.Properties["Property"].SetValue("MinWidth");
#endif
			minWidthSetter.Properties["Value"].SetValue(150d);
			styleItem.Properties["Setters"].Collection.Add(minWidthSetter);
			var minHeightSetter = ModelFactory.CreateItem(item.Context, typeof(Setter));
#if !SILVERLIGHT
			minHeightSetter.Properties["Property"].SetValue(TextEdit.MinHeightProperty);
#else
			minHeightSetter.Properties["Property"].SetValue("MinHeight");
#endif
			minHeightSetter.Properties["Value"].SetValue(12d);
			styleItem.Properties["Setters"].Collection.Add(minHeightSetter);
			ResourceManager.AddResource(styleItem, ResourceKeys.ProgressBarEditStyle);
		}
		public override string XamlItemLinkTag {
			get { return typeof(BarEditItemLink).Name; }
		}
		public override string XamlItemTag {
			get { return typeof(BarEditItem).Name; }
		}
		public override string XamlPrefix {
			get { return XmlNamespaceConstants.BarsPrefix; }
		}
	}
	public class StopButtonItemInfo : BarButtonItemInfo {
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			var visibleBinding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			visibleBinding.Properties["Path"].SetValue("IsEnabled");
			visibleBinding.Properties["RelativeSource"].SetValue("{RelativeSource Self}");
			item.Properties["IsVisible"].SetValue(visibleBinding);
			if(creator is BarsCommandCreator)
				item.Properties["BarItemDisplayMode"].SetValue(BarItemDisplayMode.ContentAndGlyph);
		}
	}
	public class BarsZoomModeItemInfo : BarItemInfo {
		string masterControlName;
		public override ModelItem CreateItem(Microsoft.Windows.Design.EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarEditItem));
		}
		public override void SetupItem(CommandBarXamlCreator creator, System.Text.StringBuilder writer, string command, string masterControlName) { }
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			this.masterControlName = masterControl.Name;
			ConfigureBarsItem(item);
		}
		void ConfigureBarsItem(ModelItem item) {
			var valueBinding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			valueBinding.Properties["ElementName"].SetValue(masterControlName);
			valueBinding.Properties["Path"].SetValue("Model.ZoomMode");
			valueBinding.Properties["Mode"].SetValue(BindingMode.TwoWay);
			item.Properties["EditValue"].SetValue(valueBinding);
			item.Properties["EditWidth"].SetValue(100d);
			item.Properties["BarItemDisplayMode"].SetValue(BarItemDisplayMode.Content);
			var isEnabledBinding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			isEnabledBinding.Properties["ElementName"].SetValue(masterControlName);
			isEnabledBinding.Properties["Path"].SetValue("Model");
			isEnabledBinding.Properties["Converter"].SetValue(ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.ObjectToBooleanConverter));
			item.Properties["IsEnabled"].SetValue(isEnabledBinding);
			new BarEditItem().BarItemDisplayMode = BarItemDisplayMode.Content;
			var customizationText = ModelFactory.CreateItem(item.Context, typeof(PrintingStringIdExtension));
			customizationText.Properties["StringId"].SetValue(PrintingStringId.Zoom);
			item.Properties["CustomizationContent"].SetValue(customizationText);
			var customizationGlyph = ModelFactory.CreateItem(item.Context, typeof(PrintingResourceImageExtension));
			customizationGlyph.Properties["ResourceName"].SetValue("Images/BarItems/Zoom_16x16.png");
			item.Properties["CustomizationGlyph"].SetValue(customizationGlyph);
			CreateZoomModeTemplates(item);
		}
		void CreateZoomModeTemplates(ModelItem item) {
			var settings = ModelFactory.CreateItem(item.Context, typeof(ComboBoxEditSettings));
			var itemSource = ModelFactory.CreateItem(item.Context, typeof(Binding));
			itemSource.Properties["ElementName"].SetValue(masterControlName);
			itemSource.Properties["Path"].SetValue("Model.ZoomModes");
			settings.Properties["ItemsSource"].SetValue(itemSource);
			settings.Properties["IsTextEditable"].SetValue(false);
#if !SILVERLIGHT
			CreateItemTemplates(item);
			var templateSelector = ModelFactory.CreateItem(item.Context, typeof(ZoomComboBoxTemplateSelector));
			templateSelector.Properties["ZoomModeItemTemplate"].SetValue(ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.ZoomModeItemTemplate));
			templateSelector.Properties["SeparatorItemTemplate"].SetValue(ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.SeparatorItemTemplate));
			settings.Properties["ItemTemplateSelector"].SetValue(templateSelector);
#endif
			item.Properties["EditSettings"].SetValue(settings);
		}
		void CreateItemTemplates(ModelItem item) {
			var textBlock = ModelFactory.CreateItem(item.Context, typeof(TextBlock));
			textBlock.Properties["Text"].SetValue(ModelFactory.CreateItem(item.Context, typeof(Binding), "DisplayedText"));
			var itemsTemplate = ModelFactory.CreateItem(item.Context, typeof(DataTemplate));
			itemsTemplate.Properties["VisualTree"].SetValue(textBlock);
			ResourceManager.AddResource(itemsTemplate, ResourceKeys.ZoomModeItemTemplate);
			var separator = ModelFactory.CreateItem(item.Context, typeof(Rectangle));
			separator.Properties["Margin"].SetValue(new Thickness(0, 1, 0, 1));
			separator.Properties["Width"].SetValue(100d);
			separator.Properties["Height"].SetValue(1d);
			separator.Properties["Fill"].SetValue(new SolidColorBrush(Colors.Black));
			var separatorTemplate = ModelFactory.CreateItem(item.Context, typeof(DataTemplate));
			separatorTemplate.Properties["VisualTree"].SetValue(separator);
			ResourceManager.AddResource(separatorTemplate, ResourceKeys.SeparatorItemTemplate);
		}
		public override string XamlItemLinkTag {
			get { return XmlNamespaceConstants.BarsPrefix; }
		}
		public override string XamlItemTag {
			get { return typeof(BarEditItem).Name; }
		}
		public override string XamlPrefix {
			get { return typeof(BarEditItemLink).Name; }
		}
	}
	public class RibbonZoomModeItemInfo : BarItemInfo {
		string masterControlName;
		public override Microsoft.Windows.Design.Model.ModelItem CreateItem(Microsoft.Windows.Design.EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarSubItem));
		}
		public override void SetupItem(CommandBarXamlCreator creator, System.Text.StringBuilder writer, string command, string masterControlName) { }
		public override void SetupItem(CommandBarCreator creator, Microsoft.Windows.Design.Model.ModelItem barManager, Microsoft.Windows.Design.Model.ModelItem item, Microsoft.Windows.Design.Model.ModelItem masterControl) {
			this.masterControlName = masterControl.Name;
			ConfigureRibbonItem(item);
		}
		void ConfigureRibbonItem(ModelItem item) {
			var itemsBinding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			itemsBinding.Properties["ElementName"].SetValue(masterControlName);
			itemsBinding.Properties["Path"].SetValue("Model.ZoomModes");
			item.Properties["ItemLinksSource"].SetValue(itemsBinding);
			var isEnabledBinding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			isEnabledBinding.Properties["ElementName"].SetValue(masterControlName);
			isEnabledBinding.Properties["Path"].SetValue("Model");
			isEnabledBinding.Properties["Converter"].SetValue(ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.ObjectToBooleanConverter));
			item.Properties["IsEnabled"].SetValue(isEnabledBinding);
			CreateRibbonZoomModeTemplates(item);
#if !SILVERLIGHT
			var templateSelector = ModelFactory.CreateItem(item.Context, typeof(ZoomSubItemTemplateSelector));
			templateSelector.Properties["ZoomModeItemTemplate"].SetValue(ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.ZoomModeItemTemplate));
			templateSelector.Properties["SeparatorItemTemplate"].SetValue(ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.SeparatorItemTemplate));
			item.Properties["ItemTemplateSelector"].SetValue(templateSelector);
#else
			item.Properties["ItemTemplate"].SetValue(ResourceManager.CreateStaticResourceBinding(item, ResourceKeys.ZoomModeItemTemplate));
#endif
		}
		void CreateRibbonZoomModeTemplates(ModelItem item) {
			#region itemTemplate
			var barButtonItem = ModelFactory.CreateItem(item.Context, typeof(BarButtonItem));
			barButtonItem.Properties["Content"].SetValue(ModelFactory.CreateItem(item.Context, typeof(Binding)));
			barButtonItem.Properties["CommandParameter"].SetValue(ModelFactory.CreateItem(item.Context, typeof(Binding)));
			var behavior = new PropertyIdentifier(typeof(BarItemBehavior), "SendZoomToModelOnClick");
			var behaviorBinding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			behaviorBinding.Properties["ElementName"].SetValue(masterControlName);
			behaviorBinding.Properties["Path"].SetValue("Model");
			barButtonItem.Properties[behavior].SetValue(behaviorBinding);
			var content = ModelFactory.CreateItem(item.Context, typeof(ContentControl));
			content.Properties["Content"].SetValue(barButtonItem);
			var itemsTemplate = ModelFactory.CreateItem(item.Context, typeof(DataTemplate));
			itemsTemplate.Properties["VisualTree"].SetValue(content);
			ResourceManager.AddResource(itemsTemplate, ResourceKeys.ZoomModeItemTemplate);
			#endregion
#if !SILVERLIGHT
			#region separatorTemplate
			var separatorItem = ModelFactory.CreateItem(item.Context, typeof(BarItemSeparator));
			var separatorContent = ModelFactory.CreateItem(item.Context, typeof(ContentControl));
			separatorContent.Properties["Content"].SetValue(separatorItem);
			var separatorTemplate = ModelFactory.CreateItem(item.Context, typeof(DataTemplate));
			separatorTemplate.Properties["VisualTree"].SetValue(separatorContent);
			ResourceManager.AddResource(separatorTemplate, ResourceKeys.SeparatorItemTemplate);
			#endregion
#endif
		}
		public override string XamlItemLinkTag {
			get { return typeof(BarSubItemLink).Name; }
		}
		public override string XamlItemTag {
			get { return typeof(BarSubItem).Name; }
		}
		public override string XamlPrefix {
			get { return XmlNamespaceConstants.BarsPrefix; }
		}
	}
	public class StatusBarInfo {
		public BarInfoItems LeftItems { get; set; }
		public BarInfoItems RightItems { get; set; }
		public StatusBarInfo(BarInfoItems leftItems, BarInfoItems rightItems) {
			this.LeftItems = leftItems;
			this.RightItems = rightItems;
		}
	}
}
