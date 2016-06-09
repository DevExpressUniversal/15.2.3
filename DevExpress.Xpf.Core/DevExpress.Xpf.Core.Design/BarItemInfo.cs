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
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Platform::System.Windows;
using Platform::System.Windows.Data;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.Core.Native;
using Platform::DevExpress.Xpf.Editors;
using Platform::DevExpress.Xpf.Office.UI;
using Platform::DevExpress.Utils;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.Xpf.Core.Design {
	#region BarInfo
	public class BarInfo {
		#region Fields
		readonly string ribbonCategoryName;
		readonly string pageName;
		readonly string groupName;
		readonly BarInfoItems items;
		readonly string ribbonPageGroupButtonCommand;
		readonly string ribbonCategoryStringId;
		readonly string pageCaptionStringId;
		readonly string groupCaptionStringId;
		#endregion
		public BarInfo(string ribbonCategoryName, string pageName, string groupName, BarInfoItems items, string ribbonPageGroupButtonCommand, string ribbonCategoryStringId, string pageCaptionStringId, string groupCaptionStringId)
			: this(ribbonCategoryName, pageName, groupName, items, ribbonPageGroupButtonCommand, ribbonCategoryStringId, pageCaptionStringId, groupCaptionStringId, string.Empty) {
		}
		public BarInfo(string ribbonCategoryName, string pageName, string groupName, BarInfoItems items, string ribbonPageGroupButtonCommand, string ribbonCategoryStringId, string pageCaptionStringId, string groupCaptionStringId, string categoryCommand) {
			this.ribbonCategoryName = ribbonCategoryName;
			this.pageName = pageName;
			this.groupName = groupName;
			this.items = items;
			this.ribbonCategoryStringId = ribbonCategoryStringId;
			this.ribbonPageGroupButtonCommand = ribbonPageGroupButtonCommand;
			this.pageCaptionStringId = pageCaptionStringId;
			this.groupCaptionStringId = groupCaptionStringId;
			this.CategoryCommand = categoryCommand;
		}
		#region Properties
		public string RibbonCategoryName { get { return ribbonCategoryName; } }
		public string PageName { get { return pageName; } }
		public string GroupName { get { return groupName; } }
		public BarInfoItems Items { get { return items; } }
		public string RibbonPageGroupButtonCommand { get { return ribbonPageGroupButtonCommand; } }
		public string RibbonCategoryStringId { get { return ribbonCategoryStringId; } }
		public string PageCaptionStringId { get { return pageCaptionStringId; } }
		public string GroupCaptionStringId { get { return groupCaptionStringId; } }
		public string CategoryCommand { get; set; }
		#endregion
	}
	#endregion
	#region CompositeBarInfo
	public class CompositeBarInfo : BarInfo {
		BarInfoItems ribbonItems;
		public CompositeBarInfo(string ribbonCategoryName,
			string pageName,
			string groupName,
			BarInfoItems barItems,
			BarInfoItems ribbonItems,
			string ribbonPageGroupButtonCommand,
			string ribbonCategoryStringId,
			string pageCaptionStringId,
			string groupCaptionStringId)
			: base(ribbonCategoryName, pageName, groupName, barItems, ribbonPageGroupButtonCommand, ribbonCategoryStringId, pageCaptionStringId, groupCaptionStringId) {
			this.ribbonItems = ribbonItems;
		}
		public BarInfoItems BarItems { get { return this.Items; } }
		public BarInfoItems RibbonItems { get { return this.ribbonItems; } }
	}
	#endregion
	#region BarInfoItems
	public class BarInfoItems {
		readonly string[] commands;
		readonly BarItemInfo[] infos;
		readonly int[] ribbonCustomButtonOrder;
		readonly IList<int> ribbonItemIndicesWithHiddenSeparatorBefore;
		readonly IList<int> ribbonItemIndicesWithDefaultRibbonStyle;
		public BarInfoItems(string[] commands, BarItemInfo[] infos) {
			this.commands = commands;
			this.infos = infos;
			System.Diagnostics.Debug.Assert(infos.Length == commands.Length);
		}
		public BarInfoItems(string[] commands, BarItemInfo[] infos, int[] ribbonCustomButtonOrder, IList<int> ribbonItemIndicesWithHiddenSeparatorBefore, IList<int> ribbonItemIndicesWithDefaultRibbonStyle)
			: this(commands, infos) {
			this.ribbonCustomButtonOrder = ribbonCustomButtonOrder;
			this.ribbonItemIndicesWithHiddenSeparatorBefore = ribbonItemIndicesWithHiddenSeparatorBefore;
			this.ribbonItemIndicesWithDefaultRibbonStyle = ribbonItemIndicesWithDefaultRibbonStyle;
			System.Diagnostics.Debug.Assert(infos.Length == ribbonCustomButtonOrder.Length);
		}
		public string[] Commands { get { return commands; } }
		public BarItemInfo[] Infos { get { return infos; } }
		public int[] RibbonCustomButtonOrder { get { return ribbonCustomButtonOrder; } }
		public IList<int> RibbonItemIndicesWithHiddenSeparatorBefore { get { return ribbonItemIndicesWithHiddenSeparatorBefore; } }
		public IList<int> RibbonItemIndicesWithDefaultRibbonStyle { get { return ribbonItemIndicesWithDefaultRibbonStyle; } }
	}
	#endregion
	public abstract class BarItemInfo {
		public abstract string XamlPrefix { get; }
		public abstract string XamlItemTag { get; }
		public virtual string XamlLinkPrefix { get { return XamlPrefix; } }
		public virtual bool SupportsCommandBinding { get { return true; } }
		public abstract string XamlItemLinkTag { get; }
		public abstract ModelItem CreateItem(EditingContext context);
		public abstract void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl);
		public abstract void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName);
		public virtual string GenerateBarItemName(string commandName) {
			return "bi" + commandName;
		}
		public virtual void CreateChildItems(CommandBarCreator creator, ModelItem item, ModelItem masterControl) {
		}
	}
	public class BarStaticItemCheckEditInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return "BarStaticItem"; } }
		public override string XamlItemLinkTag { get { return "BarStaticItemLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarStaticItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem SpreadsheetControl) {
			object dictionaryEntry = creator.EnsureResourceDictionaryEntryExists(barManager, typeof(OfficeDefaultBarItemDataTemplates), "defaultBarItemTemplates");
			ModelItem contentTemplateBinding = creator.CreateBindingToPropertyOfResourceEntryInstance(barManager.Context, dictionaryEntry, "CheckEditTemplate");
			item.Properties["ContentTemplate"].SetValue(contentTemplateBinding);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			try {
				creator.WriteAttributeString(writer, "ContentTemplate", "{Binding Path=CheckEditTemplate, Source={StaticResource defaultBarItemTemplates}}");
			}
			finally {
				creator.WriteEndElement(writer);
			}
		}
	}
	public class BarButtonItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return typeof(BarButtonItem).Name; } }
		public override string XamlItemLinkTag { get { return typeof(BarButtonItemLink).Name; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarButtonItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
		}
	}
	public class BarCheckItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return typeof(BarCheckItem).Name; } }
		public override string XamlItemLinkTag { get { return typeof(BarCheckItemLink).Name; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarCheckItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
		}
	}
	public class BarButtonItemInfoLargeRibbonGlyph : BarButtonItemInfo {
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			base.SetupItem(creator, barManager, item, masterControl);
			if (creator.IsRibbonBarCreator)
				item.Properties["GlyphSize"].SetValue(GlyphSize.Large);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			base.SetupItem(creator, writer, command, masterControlName);
			if (creator.IsRibbonBarCreator)
				creator.WriteAttributeString(writer, "GlyphSize", "Large");
		}
	}
	public class BarButtonItemInfoSmallRibbonGlyph : BarButtonItemInfo {
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			base.SetupItem(creator, barManager, item, masterControl);
			if (creator.IsRibbonBarCreator)
				item.Properties["GlyphSize"].SetValue(GlyphSize.Small);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			base.SetupItem(creator, writer, command, masterControlName);
			if (creator.IsRibbonBarCreator)
				creator.WriteAttributeString(writer, "GlyphSize", "Small");
		}
	}
	public class BarButtonItemInfoSmallWithTextRibbonStyle : BarButtonItemInfo {
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			base.SetupItem(creator, barManager, item, masterControl);
			if (creator.IsRibbonBarCreator)
				item.Properties["RibbonStyle"].SetValue(RibbonItemStyles.SmallWithText);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			base.SetupItem(creator, writer, command, masterControlName);
			if (creator.IsRibbonBarCreator)
				creator.WriteAttributeString(writer, "RibbonStyle", "SmallWithText");
		}
	}
	public class BarCheckItemInfoLargeRibbonGlyph : BarCheckItemInfo {
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			base.SetupItem(creator, barManager, item, masterControl);
			if (creator.IsRibbonBarCreator)
				item.Properties["GlyphSize"].SetValue(GlyphSize.Large);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			base.SetupItem(creator, writer, command, masterControlName);
			if (creator.IsRibbonBarCreator)
				creator.WriteAttributeString(writer, "GlyphSize", "Large");
		}
	}
	public class BarCheckItemInfoSmallRibbonGlyph : BarCheckItemInfo {
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			base.SetupItem(creator, barManager, item, masterControl);
			if (creator.IsRibbonBarCreator)
				item.Properties["GlyphSize"].SetValue(GlyphSize.Small);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			base.SetupItem(creator, writer, command, masterControlName);
			if (creator.IsRibbonBarCreator)
				creator.WriteAttributeString(writer, "GlyphSize", "Small");
		}
	}
	public class BarCheckItemInfoSmallWithTextRibbonStyle : BarCheckItemInfo {
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			base.SetupItem(creator, barManager, item, masterControl);
			if (creator.IsRibbonBarCreator)
				item.Properties["RibbonStyle"].SetValue(RibbonItemStyles.SmallWithText);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			base.SetupItem(creator, writer, command, masterControlName);
			if (creator.IsRibbonBarCreator)
				creator.WriteAttributeString(writer, "RibbonStyle", "SmallWithText");
		}
	}
	public class BarSubItemInfo : BarItemInfo {
		readonly BarInfoItems items;
		readonly RibbonItemStyles ribbonStyle;
		public BarSubItemInfo(BarInfoItems items)
			: this(items, RibbonItemStyles.Default) {
		}
		public BarSubItemInfo(BarInfoItems items, string namePrefix)
			: this(items, RibbonItemStyles.Default, namePrefix) {
		}
		public BarSubItemInfo(BarInfoItems items, RibbonItemStyles ribbonStyle)
			: this(items, ribbonStyle, string.Empty) {
		}
		public BarSubItemInfo(BarInfoItems items, RibbonItemStyles ribbonStyle, string namePrefix) {
			this.items = items;
			this.ribbonStyle = ribbonStyle;
			this.NamePrefix = namePrefix;
		}
		public BarInfoItems Items { get { return items; } }
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return typeof(BarSubItem).Name; } }
		public override string XamlItemLinkTag { get { return typeof(BarSubItemLink).Name; } }
		public string NamePrefix { get; set; }
		public override string GenerateBarItemName(string commandName) {
			return base.GenerateBarItemName(NamePrefix + commandName);
		}
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarSubItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			if (creator.IsRibbonBarCreator && ribbonStyle != RibbonItemStyles.Default)
				item.Properties["RibbonStyle"].SetValue(ribbonStyle);
		}
		public override void CreateChildItems(CommandBarCreator creator, ModelItem item, ModelItem masterControl) {
			List<ModelItem> subItems = creator.CreateBarItems(creator.BarManager.Properties["Items"].Collection, masterControl, this.Items);
			List<ModelItem> subItemLinks = creator.CreateItemLinks(subItems, this.Items);
			creator.AppendItemLinks(item, subItemLinks);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			if (creator.IsRibbonBarCreator && ribbonStyle != RibbonItemStyles.Default)
				creator.WriteAttributeString(writer, "RibbonStyle", ribbonStyle.ToString());
		}
	}
	public class BarSplitButtonSubItemInfo : BarItemInfo {
		readonly BarInfoItems items;
		readonly RibbonItemStyles ribbonStyle;
		public BarSplitButtonSubItemInfo(BarInfoItems items)
			: this(items, RibbonItemStyles.Default) {
		}
		public BarSplitButtonSubItemInfo(BarInfoItems items, string namePrefix)
			: this(items, RibbonItemStyles.Default, namePrefix) {
		}
		public BarSplitButtonSubItemInfo(BarInfoItems items, RibbonItemStyles ribbonStyle)
			: this(items, ribbonStyle, string.Empty) {
		}
		public BarSplitButtonSubItemInfo(BarInfoItems items, RibbonItemStyles ribbonStyle, string namePrefix) {
			this.items = items;
			this.ribbonStyle = ribbonStyle;
			this.NamePrefix = namePrefix;
		}
		public BarInfoItems Items { get { return items; } }
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return typeof(BarSplitButtonItem).Name; } }
		public override string XamlItemLinkTag { get { return typeof(BarSplitButtonItemLink).Name; } }
		public string NamePrefix { get; set; }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarSplitButtonItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			if(creator.IsRibbonBarCreator && ribbonStyle != RibbonItemStyles.Default)
				item.Properties["RibbonStyle"].SetValue(ribbonStyle);
		}
		public override void CreateChildItems(CommandBarCreator creator, ModelItem item, ModelItem masterControl) {
			List<ModelItem> subItems = creator.CreateBarItems(creator.BarManager.Properties["Items"].Collection, masterControl, this.Items);
			List<ModelItem> subItemLinks = creator.CreateItemLinks(subItems, this.Items);
			var popupItem = ModelFactory.CreateItem(item.Context, typeof(PopupMenu));
			creator.AppendItemLinks(popupItem, subItemLinks);
			item.Properties["PopupControl"].SetValue(popupItem);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
			if(creator.IsRibbonBarCreator && ribbonStyle != RibbonItemStyles.Default)
				creator.WriteAttributeString(writer, "RibbonStyle", ribbonStyle.ToString());
		}
	}
	public class BarButtonGroupItemInfo : BarSubItemInfo {
		public BarButtonGroupItemInfo(BarInfoItems items)
			: base(items) {
		}
		public override string XamlPrefix { get { return XmlNamespaceConstants.RibbonPrefix; } }
		public override string XamlItemTag { get { return "BarButtonGroup"; } }
		public override string XamlItemLinkTag { get { return "BarButtonGroupLink"; } }
		public override ModelItem CreateItem(EditingContext context) {
			TypeIdentifier typeIdenitifier = new TypeIdentifier("http://schemas.devexpress.com/winfx/2008/xaml/ribbon", "BarButtonGroup");
			return ModelFactory.CreateItem(context, typeIdenitifier);
		}
	}
	public class GalleryItemGroupInfo : BarItemInfo {
		readonly BarInfoItems items;
		readonly bool isCaptionVisible;
		readonly string captionStringId;
		public GalleryItemGroupInfo(BarInfoItems items, bool isCaptionVisible, string captionStringId) {
			this.items = items;
			this.isCaptionVisible = isCaptionVisible;
			this.captionStringId = captionStringId;
		}
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return typeof(GalleryItemGroup).Name; } }
		public override string XamlItemLinkTag { get { return typeof(BarItemLink).Name; } }
		public override bool SupportsCommandBinding { get { return false; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(GalleryItemGroup));
		}
		public override string GenerateBarItemName(string commandName) {
			return "grp" + commandName;
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			item.Properties["IsCaptionVisible"].SetValue(isCaptionVisible ? DefaultBoolean.True : DefaultBoolean.False);
			object captionBinding = creator.CreateBindingToCaptionPropertyOfResourceEntryInstance(barManager.Context, creator.StringIdConverterKey, captionStringId);
			item.Properties["Caption"].SetValue(captionBinding);
		}
		public override void CreateChildItems(CommandBarCreator creator, ModelItem item, ModelItem masterControl) {
			ModelItemCollection target = item.Properties["Items"].Collection;
			creator.CreateBarItems(target, masterControl, this.items);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
		}
	}
	public class BarGalleryItemInfo : BarItemInfo {
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return typeof(GalleryItem).Name; } }
		public override string XamlItemLinkTag { get { return typeof(BarItemLink).Name; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(GalleryItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
		}
	}
}
