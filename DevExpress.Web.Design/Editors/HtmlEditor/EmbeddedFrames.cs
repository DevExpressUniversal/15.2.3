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
using DevExpress.Utils.Design;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.ASPxHtmlEditor.Design;
using DevExpress.Web.ASPxSpellChecker.Design;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.XtraEditors.FeatureBrowser;
using System.Windows.Forms;
namespace DevExpress.Web.Design {
	[CLSCompliant(false)]
	public class HtmlEditorCssFilesEmbeddedFrame : MainEmbeddedFrame {
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.CssFiles.xml" }; } }
		public override Type FeatureBrowserFormBase { get { return typeof(HtmlEditorCssFilesEmbeddedForm); } }
	}
	[CLSCompliant(false)]
	public class HtmlEditorEditingSettingsEmbeddedFrame : MainEmbeddedFrame {
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.EditingSettings.xml" }; } }
		public override Type FeatureBrowserFormBase { get { return typeof(HtmlEditorEditingEmbeddedForm); } }
	}
	[CLSCompliant(false)]
	public class HtmlEditorPlaceholdersEmbeddedFrame : MainEmbeddedFrame {
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.Placeholders.xml" }; } }
		public override Type FeatureBrowserFormBase { get { return typeof(HtmlEditorPlaceholdersEmbeddedForm); } }
	}
	[CLSCompliant(false)]
	public class HtmlEditorRibbonToolbarEmbeddedFrame : MainEmbeddedFrame {
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.RibbonToolbar.xml" }; } }
		public override Type FeatureBrowserFormBase { get { return typeof(HtmlEditorRibbonToolbarEmbeddedForm); } }
	}
	[CLSCompliant(false)]
	public class HtmlEditorSpellCheckingEmbeddedFrame : MainEmbeddedFrame {
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.SpellChecker.xml" }; } }
		public override Type FeatureBrowserFormBase { get { return typeof(HtmlEditorSpellCheckingEmbeddedForm); } }
	}
	[CLSCompliant(false)]
	public class HtmlEditorEditingEmbeddedForm : FeatureTabbedViewForm {
		protected override FeatureBrowserDefaultPageBase CreateDefaultPageDesigner() {
			return new FeatureBrowserDefaultPageDescriptions();
		}
	}
	[CLSCompliant(false)]
	public class HtmlEditorCssFilesEmbeddedForm : FeatureTabbedViewForm {
		protected override FeatureBrowserDefaultPageBase CreateDefaultPageDesigner() {
			return new HtmlEditorCssFilesDescriptorFrame((ASPxHtmlEditor.ASPxHtmlEditor)SourceObject);
		}
	}
	public class HtmlEditorCssFilesDescriptorFrame : FeatureBrowserDefaultPageDescriptions {
		ItemsEditorFrame cssFilesFrame;
		public HtmlEditorCssFilesDescriptorFrame(ASPxHtmlEditor.ASPxHtmlEditor htmlEditor) {
			HtmlEditor = htmlEditor;
		}
		ASPxHtmlEditor.ASPxHtmlEditor HtmlEditor { get; set; }
		ItemsEditorFrame CssFilesFrame {
			get {
				if(cssFilesFrame == null) {
					cssFilesFrame = new ItemsEditorFrame();
					cssFilesFrame.DesignerItem = new DesignerItem() { Tag = new FlatCollectionItemsOwner<HtmlEditorCssFile>(HtmlEditor, HtmlEditor.Site, HtmlEditor.CssFiles, "CSS Files") };
				}
				return cssFilesFrame;
			}
		}
		protected override bool NeedSwapSplitterPanels { get { return LabelInfo != null; } }
		protected override void FillLeftSplitPanel() {
			pgMain.Parent = null;
			CssFilesFrame.DoInitFrame();
			CssFilesFrame.Parent = SplitContainer.Panel1;
			CssFilesFrame.Dock = DockStyle.Fill;
		}
	}
	[CLSCompliant(false)]
	public class HtmlEditorPlaceholdersEmbeddedForm : HtmlEditorToolbarDialogFormBase {
		ItemsEditorFrame placeholdersFrame;
		ItemsEditorFrame PlaceholdersFrame {
			get {
				if(placeholdersFrame == null) {
					placeholdersFrame = new ItemsEditorFrame();
					placeholdersFrame.DesignerItem = new DesignerItem() {
						Tag = new FlatCollectionItemsOwner<HtmlEditorPlaceholderItem>(HtmlEditor, HtmlEditor.Site, HtmlEditor.Placeholders, "Placeholders")
					};
				}
				return placeholdersFrame;
			}
		}
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarInsertPlaceholderDialogButton>();
		}
		protected override void FillPageToFrameAssociator() {
			AddPageToFrameAssociation("Placeholders_Items", (IEmbeddedFrame)GeneralFrame);
			AddPageToFrameAssociation("Placeholders", PlaceholdersFrame);
		}
		protected override FeatureBrowserDefaultPageBase CreateDefaultPageDesigner() {
			return new FeatureBrowserDefaultPageDescriptions();
		}
	}
	[CLSCompliant(false)]
	public class HtmlEditorDefaultEmbeddedFrame : FeatureTabbedViewForm {
		protected override FeatureBrowserDefaultPageBase CreateDefaultPageDesigner() {
			return new FeatureBrowserDefaultPageDescriptions();
		}
		protected override void FillPageToFrameAssociator() {
		}
	}
	[CLSCompliant(false)]
	public class HtmlEditorRibbonToolbarEmbeddedForm : FeatureTabbedViewForm {
		ItemsEditorFrame ribbonToolbarTabsFrame;
		ItemsEditorFrame ribbonToolbarContextTabsFrame;
		IServiceProvider Provider { get { return HtmlEditor.Site; } }
		ASPxHtmlEditor.ASPxHtmlEditor HtmlEditor { get { return (ASPxHtmlEditor.ASPxHtmlEditor)SourceObject; } }
		ItemsEditorFrame RibbonToolbarTabsFrame {
			get {
				if(ribbonToolbarTabsFrame == null) {
					ribbonToolbarTabsFrame = new HtmlEditorRibbonItemsEditorFrame();
					ribbonToolbarTabsFrame.DesignerItem = new DesignerItem() { Tag = new HtmlEditorRibbonItemsOwner(HtmlEditor, Provider, HtmlEditor.RibbonTabs) };
				}
				return ribbonToolbarTabsFrame;
			}
		}
		ItemsEditorFrame RibbonToolbarContextTabsFrame {
			get {
				if(ribbonToolbarContextTabsFrame == null) {
					ribbonToolbarContextTabsFrame = new HtmlEditorRibbonContextTabsEditorFrame();
					ribbonToolbarContextTabsFrame.DesignerItem = new DesignerItem() { Tag = new HtmlEditorRibbonContextTabsOwner(HtmlEditor, Provider, HtmlEditor.RibbonContextTabCategories) };
				}
				return ribbonToolbarContextTabsFrame;
			}
		}
		protected override bool RemoveEmptyPages { get { return true; } }
		protected override void FillPageToFrameAssociator() {
			AddPageToFrameAssociation("Tabs", RibbonToolbarTabsFrame);
			AddPageToFrameAssociation("ContextTabs", RibbonToolbarContextTabsFrame);
		}
	}
	[CLSCompliant(false)]
	public class HtmlEditorSpellCheckingEmbeddedForm : FeatureTabbedViewForm {
		ItemsEditorFrame spellCheckerFrame;
		IServiceProvider Provider { get { return HtmlEditor.Site; } }
		ASPxHtmlEditor.ASPxHtmlEditor HtmlEditor { get { return (ASPxHtmlEditor.ASPxHtmlEditor)SourceObject; } }
		ItemsEditorFrame SpellCheckerFrame {
			get {
				if(spellCheckerFrame == null) {
					spellCheckerFrame = new ItemsEditorFrame();
					spellCheckerFrame.DesignerItem = new DesignerItem() { Tag = new SpellCheckerDictionaryOwner(SourceObject, Provider, HtmlEditor.SettingsSpellChecker.Dictionaries) };
				}
				return spellCheckerFrame;
			}
		}
		protected override void FillPageToFrameAssociator() {
			AddPageToFrameAssociation("Dictionaries", SpellCheckerFrame);
		}
		protected override FeatureBrowserDefaultPageBase CreateDefaultPageDesigner() {
			return new SpellCheckerPageDescriptions(HtmlEditor);
		}
	}
	[CLSCompliant(false)]
	public class SpellCheckerPageDescriptions : FeatureBrowserDefaultPageDescriptions {
		HtmlEditorEditingDescriptionActionsOwner descriptionActions;
		public SpellCheckerPageDescriptions(ASPxHtmlEditor.ASPxHtmlEditor htmlEditor) {
			HtmlEditor = htmlEditor;
		}
		ASPxHtmlEditor.ASPxHtmlEditor HtmlEditor { get; set; }
		protected override DescriptionActions DescriptionActions {
			get {
				if(descriptionActions == null)
					descriptionActions = new HtmlEditorEditingDescriptionActionsOwner(HtmlEditor);
				return descriptionActions;
			}
		}
	}
	public class HtmlEditorEditingDescriptionActionsOwner : DescriptionActions {
		public HtmlEditorEditingDescriptionActionsOwner(ASPxHtmlEditor.ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected internal override string FeatureItemName { get { return "Spell Checker"; } }
		protected override void FillDescriptionItems() {
			AddDescriptionAction("Dialogs_SpellChecking", () => { NavigateToEditForm("Spell Checking"); }, false, null);
		}
	}
}
