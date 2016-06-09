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

using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.ASPxHtmlEditor.Rendering;
using System.Web.UI;
using DevExpress.Web.ASPxHtmlEditor.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	[ToolboxItem(false)]
	public class HtmlEditorPasteOptionsBarControl : BaseControl {
		public HtmlEditorPasteOptionsBarControl(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected ASPxMenu Menu { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Menu = new ASPxMenu();
			Menu.ID = HtmlEditor.RenderHelper.PasteOptionsBarControlID;
			Menu.ParentSkinOwner = HtmlEditor;
			Menu.EnableViewState = false;
			Menu.EnableClientSideAPI = true;
			Menu.ShowAsToolbar = true;
			Menu.EncodeHtml = HtmlEditor.EncodeHtml;
			Menu.RightToLeft = HtmlEditor.RightToLeft;
			Controls.Add(Menu);
			CreateItems();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Menu.ControlStyle.CopyFrom(HtmlEditor.GetPasteOptionsBarStyle());
			Menu.ItemStyle.Assign(HtmlEditor.GetPasteOptionsBarItemStyle());
			Menu.AutoSeparators = AutoSeparatorMode.None;
			Menu.ClientSideEvents.ItemClick += Scripts.PasteOptionsItemClick;
		}
		protected void CreateItems() {
			Menu.Items.Add(CreateMenuItem(new PasteOptionsPasteHtmlSourceFormattingButton()));
			Menu.Items.Add(CreateMenuItem(new PasteOptionsPasteHtmlMergeFormattingButton()));
			Menu.Items.Add(CreateMenuItem(new PasteOptionsPasteHtmlPlainTextButton()));
		}
		protected MenuItem CreateMenuItem(HtmlEditorPasteOptionsItem item) {
			MenuItem menuItem = new MenuItem();
			menuItem.Name = item.CommandName;
			menuItem.ToolTip = item.ToolTip;
			string resourceName = item.GetResourceImageName();
			menuItem.Image.Assign(string.IsNullOrEmpty(resourceName) ? item.Image : this.GetImageProperties(resourceName));
			if(!menuItem.Image.IsEmpty && string.IsNullOrEmpty(menuItem.Image.AlternateText))
				menuItem.Image.AlternateText = menuItem.ToolTip;
			menuItem.GroupName = item.CommandName;
			return menuItem;
		}
		protected internal PasteOptionsBarItemImageProperties GetImageProperties(string resourceImageName) {
			PasteOptionsBarItemImageProperties imageProps = new PasteOptionsBarItemImageProperties();
			imageProps.CopyFrom(HtmlEditor.Images.IconImages.GetImageProperties(Page, resourceImageName));
			return imageProps;
		}
	}
	public class HtmlEditorPasteOptionsItem : CollectionItem {
		PasteOptionsBarItemImageProperties image = null;
		public HtmlEditorPasteOptionsItem()
			: this("") {
		}
		public HtmlEditorPasteOptionsItem(string commandName)
			: base() {
			CommandName = commandName;
		}
		public HtmlEditorPasteOptionsItem(string commandName, string toolTip)
			: base() {
			ToolTip = toolTip;
			CommandName = commandName;
		}
		[Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual PasteOptionsBarItemImageProperties Image {
			get {
				if(image == null)
					image = new PasteOptionsBarItemImageProperties(this);
				return image;
			}
		}
		protected internal virtual string CommandName {
			get { return GetStringProperty("CommandName", ""); }
			set { SetStringProperty("CommandName", "", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public virtual string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set {
				SetStringProperty("ToolTip", "", value);
				LayoutChanged();
			}
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			HtmlEditorPasteOptionsItem button = source as HtmlEditorPasteOptionsItem;
			if(button != null) {
				Image.Assign(button.Image);
				ToolTip = button.ToolTip;
				CommandName = button.CommandName;
			}
		}
		protected internal virtual string GetResourceImageName() {
			return string.Empty;
		}
	}
	public class PasteOptionsPasteHtmlSourceFormattingButton : HtmlEditorPasteOptionsItem {
		protected const string DefaultCommandName = "pastehtmlsourceformatting";
		public PasteOptionsPasteHtmlSourceFormattingButton()
			: base(DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPasteHtmlSourceFormatting)) {
		}
		[DefaultValue(StringResources.HtmlEditorText_PasteHtmlSourceFormatting)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected internal override string GetResourceImageName() {
			return HtmlEditorIconImages.PasteHtmlSourceFormatting;
		}
	}
	public class PasteOptionsPasteHtmlPlainTextButton : HtmlEditorPasteOptionsItem {
		protected const string DefaultCommandName = "pastehtmlplaintext";
		public PasteOptionsPasteHtmlPlainTextButton()
			: base(DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPasteHtmlPlainText)) {
		}
		[DefaultValue(StringResources.HtmlEditorText_PasteHtmlPlainText)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected internal override string GetResourceImageName() {
			return HtmlEditorIconImages.PasteHtmlPlainText;
		}
	}
	public class PasteOptionsPasteHtmlMergeFormattingButton : HtmlEditorPasteOptionsItem {
		protected const string DefaultCommandName = "pastehtmlmergeformatting";
		public PasteOptionsPasteHtmlMergeFormattingButton()
			: base(DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPasteHtmlMergeFormatting)) {
		}
		[DefaultValue(StringResources.HtmlEditorText_PasteHtmlMergeFormatting)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected internal override string GetResourceImageName() {
			return HtmlEditorIconImages.PasteHtmlMergeFormatting;
		}
	}
}
