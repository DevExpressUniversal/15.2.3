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
using System;
using System.Text;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Platform::System.Windows;
using Platform::System.Windows.Controls;
using Platform::System.Windows.Data;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.XtraSpreadsheet;
using Platform::DevExpress.Xpf.Office.UI;
using Platform::DevExpress.Xpf.Spreadsheet.UI;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Xpf.Ribbon;
using Platform::DevExpress.Xpf.Spreadsheet;
using Platform::DevExpress.Utils;
namespace DevExpress.Xpf.Spreadsheet.Design {
	#region BarColorSplitButtonItemInfo (abstract class)
	public abstract class BarColorSplitButtonItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.OfficePrefix; } }
		public override string XamlItemTag { get { return "BarSplitButtonColorEditItem"; } }
		public override string XamlItemLinkTag { get { return "BarSplitButtonColorEditItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarSplitButtonColorEditItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem SpreadsheetControl) {
			ModelItem popupContainerInfo = ModelFactory.CreateItem(item.Context, typeof(PopupControlContainerInfo));
			item.Properties["PopupControl"].SetValue(popupContainerInfo);
			ModelItem colorEdit = ModelFactory.CreateItem(item.Context, typeof(ColorEdit));
			SetupColorEdit(colorEdit);
			popupContainerInfo.Properties["Content"].SetValue(colorEdit);
			ModelItem bindingModelItem = ModelFactory.CreateItem(item.Context, typeof(Binding));
			bindingModelItem.Properties["ElementName"].SetValue(item.Name);
			bindingModelItem.Properties["Mode"].SetValue(BindingMode.TwoWay);
			ModelItem propertyPathModelItem = ModelFactory.CreateItem(item.Context, new PropertyPath("EditValue"));
			bindingModelItem.Properties["Path"].SetValue(propertyPathModelItem);
			colorEdit.Properties["EditValue"].SetValue(bindingModelItem);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteStartElement(writer, XamlItemTag + ".PopupControl", XmlNamespaceConstants.OfficePrefix);
			try {
				creator.WriteStartElement(writer, "PopupControlContainerInfo", XmlNamespaceConstants.BarsPrefix);
				try {
					creator.WriteStartElement(writer, "ColorEdit", XmlNamespaceConstants.EditorsPrefix);
					try {
						creator.WriteAttributeString(writer, "EditValue", "{Binding ElementName=bi" + command + ", Mode=TwoWay, Path=EditValue}");
						SetupColorEdit(creator, writer);
					}
					finally {
						creator.WriteEndElement(writer);
					}
				}
				finally {
					creator.WriteEndElement(writer);
				}
			}
			finally {
				creator.WriteEndElement(writer);
			}
		}
		protected internal abstract void SetupColorEdit(ModelItem colorEdit);
		protected internal abstract void SetupColorEdit(CommandBarXamlCreator creator, StringBuilder writer);
	}
	#endregion
	#region BarForeColorSplitButtonItemInfo
	public class BarForeColorSplitButtonItemInfo : BarColorSplitButtonItemInfo {
		protected internal override void SetupColorEdit(ModelItem colorEdit) {
			colorEdit.Properties["ShowBorder"].SetValue(false);
		}
		protected internal override void SetupColorEdit(CommandBarXamlCreator creator, StringBuilder writer) {
			creator.WriteAttributeString(writer, "ShowBorder", "False");
		}
	}
	#endregion
	#region BarBackColorSplitButtonItemInfo
	public class BarBackColorSplitButtonItemInfo : BarColorSplitButtonItemInfo {
		protected internal override void SetupColorEdit(ModelItem colorEdit) {
			colorEdit.Properties["ShowBorder"].SetValue(false);
			colorEdit.Properties["ShowDefaultColorButton"].SetValue(false);
			colorEdit.Properties["ShowNoColorButton"].SetValue(true);
		}
		protected internal override void SetupColorEdit(CommandBarXamlCreator creator, StringBuilder writer) {
			creator.WriteAttributeString(writer, "ShowBorder", "False");
			creator.WriteAttributeString(writer, "ShowDefaultColorButton", "False");
			creator.WriteAttributeString(writer, "ShowNoColorButton", "True");
		}
	}
	#endregion
	#region BarColorButtonItemInfo (abstract class)
	public abstract class BarColorButtonItemInfo : BarColorSplitButtonItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.OfficePrefix; } }
		public override string XamlItemTag { get { return "BarButtonColorEditItem"; } }
		public override string XamlItemLinkTag { get { return "BarButtonColorEditItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarButtonColorEditItem));
		}
	}
	#endregion
	#region BarColorEditItemInfo
	public class BarColorEditItemInfo : BarColorButtonItemInfo {
		protected internal override void SetupColorEdit(ModelItem colorEdit) {
			colorEdit.Properties["ShowBorder"].SetValue(false);
			colorEdit.Properties["ShowDefaultColorButton"].SetValue(false);
			colorEdit.Properties["ShowNoColorButton"].SetValue(true);
		}
		protected internal override void SetupColorEdit(CommandBarXamlCreator creator, StringBuilder writer) {
			creator.WriteAttributeString(writer, "ShowBorder", "False");
			creator.WriteAttributeString(writer, "ShowDefaultColorButton", "False");
			creator.WriteAttributeString(writer, "ShowNoColorButton", "True");
		}
	}
	#endregion
	#region BarFontNameItemInfo
	public class BarFontNameItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return "BarEditItem"; } }
		public override string XamlItemLinkTag { get { return "BarEditItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarEditItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem SpreadsheetControl) {
			item.Properties["Content"].SetValue(string.Empty);
			item.Properties["EditWidth"].SetValue(150.0);
			ModelItem settings = ModelFactory.CreateItem(item.Context, typeof(FontComboBoxEditSettings));
			item.Properties["EditSettings"].SetValue(settings);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "Content", string.Empty);
			creator.WriteAttributeString(writer, "EditWidth", "150");
			creator.WriteStartElement(writer, "BarEditItem.EditSettings", XmlNamespaceConstants.BarsPrefix);
			try {
				creator.WriteStartElement(writer, "FontComboBoxEditSettings", XmlNamespaceConstants.OfficePrefix);
				creator.WriteEndElement(writer);
			}
			finally {
				creator.WriteEndElement(writer);
			}
		}
	}
	#endregion
	#region BarFontSizeItemInfo
	public class BarFontSizeItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return "BarEditItem"; } }
		public override string XamlItemLinkTag { get { return "BarEditItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarEditItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem SpreadsheetControl) {
			item.Properties["Content"].SetValue(string.Empty);
			item.Properties["EditWidth"].SetValue(50.0);
			ModelItem settings = ModelFactory.CreateItem(item.Context, typeof(FontSizeComboBoxEditSettings));
			item.Properties["EditSettings"].SetValue(settings);
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(SpreadsheetControl.Name);
			settings.Properties["OfficeFontSizeProvider"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "Content", string.Empty);
			creator.WriteAttributeString(writer, "EditWidth", "50");
			creator.WriteStartElement(writer, "BarEditItem.EditSettings", XmlNamespaceConstants.BarsPrefix);
			try {
				creator.WriteStartElement(writer, "FontSizeComboBoxEditSettings", XmlNamespaceConstants.OfficePrefix);
				try {
					creator.WriteAttributeString(writer, "OfficeFontSizeProvider", "{Binding ElementName=" + masterControlName + "}");
				}
				finally {
					creator.WriteEndElement(writer);
				}
			}
			finally {
				creator.WriteEndElement(writer);
			}
		}
	}
	#endregion
	#region PaperKindBarListItemInfo
	public class PaperKindBarListItemInfo : BarItemInfo {
		const string barItemName = "biPageLayoutSizeList";
		public override string XamlPrefix { get { return XmlNamespaceConstants.SpreadsheetPrefix; } }
		public override string XamlItemTag { get { return typeof(PaperKindBarListItem).Name; } }
		public override string XamlItemLinkTag { get { return typeof(PaperKindBarListItemLink).Name; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(PaperKindBarListItem));
		}
		public override string GenerateBarItemName(string commandName) {
			return barItemName;
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem SpreadsheetControl) {
			item.Name = barItemName;
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(SpreadsheetControl.Name);
			item.Properties["SpreadsheetControl"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "Name", barItemName);
			creator.WriteAttributeString(writer, "SpreadsheetControl", "{Binding ElementName=" + masterControlName + "}");
		}
	}
	#endregion
	#region PageMarginsBarCheckItemInfo
	public class PageMarginsBarCheckItemInfo : BarCheckItemInfo {
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem spreadsheetControl) {
			base.SetupItem(creator, barManager, item, spreadsheetControl);
			item.Properties["GlyphSize"].SetValue(GlyphSize.Large);
			object dictionaryEntry = creator.EnsureResourceDictionaryEntryExists(barManager, typeof(SpreadsheetDefaultBarItemDataTemplates), "defaultBarItemTemplates");
			if (dictionaryEntry == null)
				return;
			ModelItem binding = creator.CreateBindingToPropertyOfResourceEntryInstance(barManager.Context, dictionaryEntry, "MarginBarItemContentTemplate");
			item.Properties["ContentTemplate"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			base.SetupItem(creator, writer, command, masterControlName);
			creator.WriteAttributeString(writer, "GlyphSize", "Large");
			creator.WriteAttributeString(writer, "ContentTemplate", "{Binding Path=MarginBarItemContentTemplate, Mode=OneTime, Source={StaticResource defaultBarItemTemplates}}");
		}
	}
	#endregion
	#region InsertFunctionsBarSubItemInfo
	public class InsertFunctionsBarSubItemInfo : BarSubItemInfo {
		readonly Type barSubItemType;
		public InsertFunctionsBarSubItemInfo(Type barSubItemType) :
			base(new BarInfoItems(new string[] { }, new BarItemInfo[] { })) {
			this.barSubItemType = barSubItemType;
		}
		public override string XamlPrefix { get { return XmlNamespaceConstants.SpreadsheetPrefix; } }
		public override string XamlItemTag { get { return barSubItemType.Name; } }
		public override string XamlLinkPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemLinkTag { get { return "BarSubItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, barSubItemType);
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem spreadsheetControl) {
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(spreadsheetControl.Name);
			item.Properties["SpreadsheetControl"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "SpreadsheetControl", "{Binding ElementName=" + masterControlName + "}");
		}
	}
	#endregion
	#region InsertDefinedNameBarSubItemInfo
	public class InsertDefinedNameBarSubItemInfo : InsertFunctionsBarSubItemInfo {
		public InsertDefinedNameBarSubItemInfo(Type barSubItemType)
			: base(barSubItemType) {
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem spreadsheetControl) {
			base.SetupItem(creator, barManager, item, spreadsheetControl);
			if (creator.IsRibbonBarCreator)
				item.Properties["GlyphSize"].SetValue(GlyphSize.Large);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			base.SetupItem(creator, writer, command, masterControlName);
			if (creator.IsRibbonBarCreator)
				creator.WriteAttributeString(writer, "GlyphSize", "Large");
		}
	}
	#endregion
	#region GalleryChartLayoutsItemInfo
	public class GalleryChartLayoutsItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.RibbonPrefix; } }
		public override string XamlItemTag { get { return typeof(RibbonGalleryBarItem).Name; } }
		public override string XamlItemLinkTag { get { return typeof(RibbonGalleryBarItemLink).Name; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(RibbonGalleryBarItem));
		}
		public override string GenerateBarItemName(string commandName) {
			return "biGalleryChartLayout";
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			item.Name = "biGalleryChartLayout";
			ModelItem gallery = ModelFactory.CreateItem(item.Context, typeof(Gallery));
			item.Properties["Gallery"].SetValue(gallery);
			gallery.Properties["IsItemGlyphVisible"].SetValue(true);
			gallery.Properties["IsItemDescriptionVisible"].SetValue(false);
			gallery.Properties["IsGroupCaptionVisible"].SetValue(DefaultBoolean.False);
			gallery.Properties["MinColCount"].SetValue(3);
			gallery.Properties["ColCount"].SetValue(6);
			ModelItem galleryGroupSourceBinding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			galleryGroupSourceBinding.Properties["ElementName"].SetValue(masterControl.Name);
			galleryGroupSourceBinding.Properties["Path"].SetValue("ChartLayoutGalleryGroups");
			gallery.Properties["GroupsSource"].SetValue(galleryGroupSourceBinding);
			object dictionaryEntry = creator.EnsureResourceDictionaryEntryExists(barManager, typeof(SpreadsheetDefaultBarItemDataTemplates), "defaultBarItemTemplates");
			if (dictionaryEntry == null)
				return;
			ModelItem galleryGroupTemplateBinding = creator.CreateBindingToPropertyOfResourceEntryInstance(barManager.Context, dictionaryEntry, "ChartLayoutGalleryGroupTemplate");
			gallery.Properties["GroupTemplate"].SetValue(galleryGroupTemplateBinding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
		}
	}
	#endregion
	#region BarButtonItemInfoOverrideName
	public class BarButtonItemInfoOverrideName : BarButtonItemInfo {
		readonly string name;
		public BarButtonItemInfoOverrideName(string name) {
			this.name = name;
		}
		public override string GenerateBarItemName(string commandName) {
			if (!String.IsNullOrEmpty(name))
				return name;
			else
				return base.GenerateBarItemName(commandName);
		}
	}
	#endregion
	#region BarItemInfos
	public static class BarItemInfos {
		static readonly BarItemInfo button = new BarButtonItemInfo();
		static readonly BarItemInfo check = new BarCheckItemInfo();
		static readonly BarItemInfo galleryItem = new BarGalleryItemInfo();
		static readonly BarItemInfo buttonLargeRibbonGlyph = new BarButtonItemInfoLargeRibbonGlyph();
		static readonly BarItemInfo checkLargeRibbonGlyph = new BarCheckItemInfoLargeRibbonGlyph();
		static readonly BarItemInfo buttonSmallWithTextRibbonStyle = new BarButtonItemInfoSmallWithTextRibbonStyle();
		static readonly BarItemInfo checkSmallWithTextRibbonStyle = new BarCheckItemInfoSmallWithTextRibbonStyle();
		static readonly BarItemInfo foreColorSplitButton = new BarForeColorSplitButtonItemInfo();
		static readonly BarItemInfo backColorSplitButton = new BarBackColorSplitButtonItemInfo();
		static readonly BarItemInfo pageMarginsCheck = new PageMarginsBarCheckItemInfo();
		static readonly BarItemInfo checkEditItem = new BarStaticItemCheckEditInfo();
		static readonly BarItemInfo colorEditItem = new BarColorEditItemInfo();
		public static BarItemInfo Button { get { return button; } }
		public static BarItemInfo Check { get { return check; } }
		public static BarItemInfo GalleryItem { get { return galleryItem; } }
		public static BarItemInfo ButtonLargeRibbonGlyph { get { return buttonLargeRibbonGlyph; } }
		public static BarItemInfo CheckLargeRibbonGlyph { get { return checkLargeRibbonGlyph; } }
		public static BarItemInfo ButtonSmallWithTextRibbonStyle { get { return buttonSmallWithTextRibbonStyle; } }
		public static BarItemInfo CheckSmallWithTextRibbonStyle { get { return checkSmallWithTextRibbonStyle; } }
		public static BarItemInfo ForeColorSplitButton { get { return foreColorSplitButton; } }
		public static BarItemInfo BackColorSplitButton { get { return backColorSplitButton; } }
		public static BarItemInfo PageMarginsCheck { get { return pageMarginsCheck; } }
		public static BarItemInfo CheckEditItem { get { return checkEditItem; } }
		public static BarItemInfo ColorEditItem { get { return colorEditItem; } }
	}
	#endregion
}
