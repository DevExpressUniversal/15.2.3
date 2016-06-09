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
using Platform::DevExpress.Xpf.RichEdit.UI;
using Platform::DevExpress.XtraRichEdit;
using Platform::DevExpress.Xpf.Office.UI;
using Platform::DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.RichEdit.Design {
	public abstract class BarColorSplitButtonItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.RichEditPrefix; } }
		public override string XamlItemTag { get { return "BarSplitButtonColorEditItem"; } }
		public override string XamlItemLinkTag { get { return "BarSplitButtonColorEditItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarSplitButtonColorEditItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem richEditControl) {
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
			creator.WriteStartElement(writer, XamlItemTag + ".PopupControl", XmlNamespaceConstants.RichEditPrefix);
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
	public class BarForeColorSplitButtonItemInfo : BarColorSplitButtonItemInfo {
		protected internal override void SetupColorEdit(ModelItem colorEdit) {
			colorEdit.Properties["ShowBorder"].SetValue(false);
		}
		protected internal override void SetupColorEdit(CommandBarXamlCreator creator, StringBuilder writer) {
			creator.WriteAttributeString(writer, "ShowBorder", "False");
		}
	}
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
	public class BarCharactersBackgroundColorSplitButtonItemInfo : BarBackColorSplitButtonItemInfo {
		protected internal override void SetupColorEdit(ModelItem colorEdit) {
			base.SetupColorEdit(colorEdit);
			colorEdit.Properties["ColumnCount"].SetValue(5);
			colorEdit.Properties["ChipSize"].SetValue(ChipSize.Large);
			colorEdit.Properties["ChipMargin"].SetValue(new Thickness(5));
			colorEdit.Properties["ShowMoreColorsButton"].SetValue(false);
			ModelItem palettesItem = ModelFactory.CreateItem(colorEdit.Context, typeof(CharactersBackgroundColorPaletteCollection));
			colorEdit.Properties["Palettes"].SetValue(palettesItem);
		}
		protected internal override void SetupColorEdit(CommandBarXamlCreator creator, StringBuilder writer) {
			base.SetupColorEdit(creator, writer);
			creator.WriteAttributeString(writer, "ColumnCount", "5");
			creator.WriteAttributeString(writer, "ChipSize", "Large");
			creator.WriteAttributeString(writer, "ChipMargin", "5");
			creator.WriteAttributeString(writer, "ShowMoreColorsButton", "False");
			creator.WriteStartElement(writer, "ColorEdit.Palettes", XmlNamespaceConstants.EditorsPrefix);
			try {
				creator.WriteStartElement(writer, "CharactersBackgroundColorPaletteCollection", XmlNamespaceConstants.RichEditPrefix);
				creator.WriteEndElement(writer);
			}
			finally {
				creator.WriteEndElement(writer);
			}
		}
	}
	public abstract class BarColorButtonItemInfo : BarColorSplitButtonItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.RichEditPrefix; } }
		public override string XamlItemTag { get { return "BarButtonColorEditItem"; } }
		public override string XamlItemLinkTag { get { return "BarButtonColorEditItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarButtonColorEditItem));
		}
	}
	public class BarPageColorButtonItemInfo : BarColorButtonItemInfo {
		protected internal override void SetupColorEdit(ModelItem colorEdit) {
			colorEdit.Properties["ShowBorder"].SetValue(false);
		}
		protected internal override void SetupColorEdit(CommandBarXamlCreator creator, StringBuilder writer) {
			creator.WriteAttributeString(writer, "ShowBorder", "False");
		}
	}
	public class BarFontNameItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return "BarEditItem"; } }
		public override string XamlItemLinkTag { get { return "BarEditItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarEditItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem richEditControl) {
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
#if SL
				creator.WriteStartElement(writer, "FontComboBoxEditSettings", XmlNamespaceConstants.RichEditPrefix);
#else
				creator.WriteStartElement(writer, "FontComboBoxEditSettings", XmlNamespaceConstants.OfficePrefix);
#endif
				creator.WriteEndElement(writer);
			}
			finally {
				creator.WriteEndElement(writer);
			}
		}
	}
	public class BarFontSizeItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return "BarEditItem"; } }
		public override string XamlItemLinkTag { get { return "BarEditItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarEditItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem richEditControl) {
			item.Properties["Content"].SetValue(string.Empty);
			item.Properties["EditWidth"].SetValue(50.0);
			ModelItem settings = ModelFactory.CreateItem(item.Context, typeof(FontSizeComboBoxEditSettings));
			item.Properties["EditSettings"].SetValue(settings);
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(richEditControl.Name);
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
	public class BarStyleItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return "BarEditItem"; } }
		public override string XamlItemLinkTag { get { return "BarEditItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarEditItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem richEditControl) {
			item.Properties["Content"].SetValue(string.Empty);
			item.Properties["EditWidth"].SetValue(150.0);
			ModelItem settings = ModelFactory.CreateItem(item.Context, typeof(RichEditStyleComboBoxEditSettings));
			item.Properties["EditSettings"].SetValue(settings);
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(richEditControl.Name);
			settings.Properties["RichEditControl"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "Content", string.Empty);
			creator.WriteAttributeString(writer, "EditWidth", "150");
			creator.WriteStartElement(writer, "BarEditItem.EditSettings", XmlNamespaceConstants.BarsPrefix);
			try {
				creator.WriteStartElement(writer, "RichEditStyleComboBoxEditSettings", XmlNamespaceConstants.RichEditPrefix);
				try {
					creator.WriteAttributeString(writer, "RichEditControl", "{Binding ElementName=" + masterControlName + "}");
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
	public class InsertMergeFieldsBarListItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.RichEditPrefix; } }
		public override string XamlItemTag { get { return "InsertMergeFieldsBarListItem"; } }
		public override string XamlItemLinkTag { get { return "InsertMergeFieldsBarListItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(InsertMergeFieldsBarListItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem richEditControl) {
			item.Name = "biMailMergeInsertFieldList";
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(richEditControl.Name);
			item.Properties["RichEditControl"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "Name", "biMailMergeInsertFieldList");
			creator.WriteAttributeString(writer, "RichEditControl", "{Binding ElementName=" + masterControlName + "}");
		}
	}
	public class GalleryStyleItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.RichEditPrefix; } }
		public override string XamlItemTag { get { return "GalleryStyleItem"; } }
		public override string XamlItemLinkTag { get { return "RibbonStyleGalleryItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(GalleryStyleItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem richEditControl) {
			item.Name = "biGalleryStyle";
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(richEditControl.Name);
			item.Properties["RichEditControl"].SetValue(binding);
			ModelItem gallery = ModelFactory.CreateItem(item.Context, typeof(RichEditStyleGallery));
			item.Properties["Gallery"].SetValue(gallery);
			gallery.Properties["IsItemGlyphVisible"].SetValue(false);
			gallery.Properties["IsItemDescriptionVisible"].SetValue(false);
			gallery.Properties["MinColCount"].SetValue(3);
			gallery.Properties["ColCount"].SetValue(10);
			gallery.Properties["ItemCheckMode"].SetValue(GalleryItemCheckMode.Single);
			object dictionaryEntry = creator.EnsureResourceDictionaryEntryExists(barManager, typeof(DefaultBarItemDataTemplates), "defaultBarItemTemplates");
			if (dictionaryEntry == null)
				return;
			ModelItem galleryStyleGalleryItemCaptionTemplateBinding = creator.CreateBindingToPropertyOfResourceEntryInstance(barManager.Context, dictionaryEntry, "StyleGalleryItemCaptionTemplate");
			gallery.Properties["ItemCaptionTemplate"].SetValue(galleryStyleGalleryItemCaptionTemplateBinding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "RichEditControl", "{Binding ElementName=" + masterControlName + "}");
			creator.WriteStartElement(writer, "GalleryStyleItem.Gallery", XmlNamespaceConstants.RichEditPrefix);
			try {
				creator.WriteStartElement(writer, "RichEditStyleGallery", XmlNamespaceConstants.RichEditPrefix);
				try {
					creator.WriteAttributeString(writer, "ItemCaptionTemplate", "{Binding Path=StyleGalleryItemCaptionTemplate, Mode=OneTime, Source={StaticResource defaultBarItemTemplates}}");
					creator.WriteAttributeString(writer, "IsItemDescriptionVisible", "False");
					creator.WriteAttributeString(writer, "IsItemGlyphVisible", "False");
					creator.WriteAttributeString(writer, "ItemCheckMode", "Single");
					creator.WriteAttributeString(writer, "MinColCount", "3");
					creator.WriteAttributeString(writer, "ColCount", "10");
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
	public class PaperKindBarListItemInfo : BarItemInfo {
		const string barItemName = "biPageLayoutSizeList";
		public override string XamlPrefix { get { return XmlNamespaceConstants.RichEditPrefix; } }
		public override string XamlItemTag { get { return "PaperKindBarListItem"; } }
		public override string XamlItemLinkTag { get { return "PaperKindBarListItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(PaperKindBarListItem));
		}
		public override string GenerateBarItemName(string commandName) {
			return barItemName;
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem richEditControl) {
			item.Name = barItemName;
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(richEditControl.Name);
			item.Properties["RichEditControl"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "Name", barItemName);
			creator.WriteAttributeString(writer, "RichEditControl", "{Binding ElementName=" + masterControlName + "}");
		}
	}
	public class PageMarginsBarCheckItemInfo : BarCheckItemInfo {
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem richEditControl) {
			base.SetupItem(creator, barManager, item, richEditControl);
			item.Properties["GlyphSize"].SetValue(GlyphSize.Large);
			object dictionaryEntry = creator.EnsureResourceDictionaryEntryExists(barManager, typeof(DefaultBarItemDataTemplates), "defaultBarItemTemplates");
			if (dictionaryEntry == null)
				return;
			ModelItem binding = creator.CreateBindingToPropertyOfResourceEntryInstance(barManager.Context, dictionaryEntry, "SectionMarginBarItemContentTemplate");
			item.Properties["ContentTemplate"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			base.SetupItem(creator, writer, command, masterControlName);
			creator.WriteAttributeString(writer, "GlyphSize", "Large");
			creator.WriteAttributeString(writer, "ContentTemplate", "{Binding Path=SectionMarginBarItemContentTemplate, Mode=OneTime, Source={StaticResource defaultBarItemTemplates}}");
		}
	}
	public class InsertMergeFieldsBarSubItemInfo : BarSubItemInfo {
		public InsertMergeFieldsBarSubItemInfo() :
			base(new BarInfoItems(new string[] { }, new BarItemInfo[] { })) {
		}
		public override string XamlPrefix { get { return XmlNamespaceConstants.RichEditPrefix; } }
		public override string XamlItemTag { get { return "InsertMergeFieldsBarSubItem"; } }
		public override string XamlLinkPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemLinkTag { get { return "BarSubItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(InsertMergeFieldsBarSubItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem richEditControl) {
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(richEditControl.Name);
			item.Properties["RichEditControl"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "RichEditControl", "{Binding ElementName=" + masterControlName + "}");
		}
	}
	public class ReviewersSubItemInfo : BarSubItemInfo {
		public ReviewersSubItemInfo() :
			base(new BarInfoItems(new string[] { }, new BarItemInfo[] { })) { 
		}
		public override string XamlPrefix { get { return XmlNamespaceConstants.RichEditPrefix; } }
		public override string XamlItemTag { get { return "ReviewersBarSubItem"; } }
		public override string XamlLinkPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemLinkTag { get { return "BarSubItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(ReviewersBarSubItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem richEditControl) {
			ModelItem binding = ModelFactory.CreateItem(item.Context, typeof(Binding));
			binding.Properties["ElementName"].SetValue(richEditControl.Name);
			item.Properties["RichEditControl"].SetValue(binding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			creator.WriteAttributeString(writer, "RichEditControl", "{Binding ElementName=" + masterControlName + "}");
		}
	}
	public static class BarItemInfos {
		static readonly BarItemInfo button = new BarButtonItemInfo();
		static readonly BarItemInfo check = new BarCheckItemInfo();
		static readonly BarItemInfo buttonLargeRibbonGlyph = new BarButtonItemInfoLargeRibbonGlyph();
		static readonly BarItemInfo checkLargeRibbonGlyph = new BarCheckItemInfoLargeRibbonGlyph();
		static readonly BarItemInfo foreColorSplitButton = new BarForeColorSplitButtonItemInfo();
		static readonly BarItemInfo backColorSplitButton = new BarBackColorSplitButtonItemInfo();
		static readonly BarItemInfo pageMarginsCheck = new PageMarginsBarCheckItemInfo();
		public static BarItemInfo Button { get { return button; } }
		public static BarItemInfo Check { get { return check; } }
		public static BarItemInfo ButtonLargeRibbonGlyph { get { return buttonLargeRibbonGlyph; } }
		public static BarItemInfo CheckLargeRibbonGlyph { get { return checkLargeRibbonGlyph; } }
		public static BarItemInfo ForeColorSplitButton { get { return foreColorSplitButton; } }
		public static BarItemInfo BackColorSplitButton { get { return backColorSplitButton; } }
		public static BarItemInfo PageMarginsCheck { get { return pageMarginsCheck; } }
	}
}
