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

using System.Web.Mvc;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	public class RibbonExtension: ExtensionBase {
		public RibbonExtension(RibbonSettings settings)
			: base(settings) {
		}
		public RibbonExtension(RibbonSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new ASPxRibbon Control { get { return (ASPxRibbon)base.Control; } }
		protected internal new RibbonSettings Settings { get { return (RibbonSettings)base.Settings; } }
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AllowMinimize = Settings.AllowMinimize;
			Control.Minimized = Settings.Minimized;
			Control.ActiveTabIndex = Settings.ActiveTabIndex;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ShowFileTab = Settings.ShowFileTab;
			Control.ShowTabs = Settings.ShowTabs;
			Control.ShowGroupLabels = Settings.ShowGroupLabels;
			Control.OneLineMode = Settings.OneLineMode;
			Control.Tabs.Assign(Settings.Tabs);
			Control.ContextTabCategories.Assign(Settings.ContextTabCategories);
			Control.TabDataFields.Assign(Settings.TabDataFields);
			Control.GroupDataFields.Assign(Settings.GroupDataFields);
			Control.ItemDataFields.Assign(Settings.ItemDataFields);
			Control.Images.CopyFrom(Settings.Images);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.StylesTabControl.CopyFrom(Settings.StylesTabControl);
			Control.StylesPopupMenu.CopyFrom(Settings.StylesPopupMenu);
			Control.TabDataBound += Settings.TabDataBound;
			Control.GroupDataBound += Settings.GroupDataBound;
			Control.ItemDataBound += Settings.ItemDataBound;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.FileTabTemplate = ContentControlTemplate<TabControlTemplateContainer>.Create(
				Settings.FileTabTemplateContent, Settings.FileTabTemplateContentMethod,
				typeof(TabControlTemplateContainer));
			AssignTabs(Settings.Tabs, Control.Tabs);
			for(int i = 0; i < Settings.ContextTabCategories.Count; i++)
				AssignTabs(((MVCxRibbonContextTabCategory)Settings.ContextTabCategories[i]).Tabs, Control.ContextTabCategories[i].Tabs);
		}
		public RibbonExtension Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public RibbonExtension BindToSiteMap(string fileName) {
			return BindToSiteMap(fileName, true);
		}
		public RibbonExtension BindToSiteMap(string fileName, bool showStartingNode) {
			BindToSiteMapInternal(fileName, showStartingNode);
			return this;
		}
		public RibbonExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public RibbonExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public RibbonExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxRibbon();
		}
		private void AssignTabs(MVCxRibbonTabCollection mvcTabs, RibbonTabCollection tabs) {
			for(var tabIndex = 0; tabIndex < mvcTabs.Count; tabIndex++) {
				var sourceTab = tabs[tabIndex];
				var destinationTab = tabs[tabIndex];
				for(var groupIndex = 0; groupIndex < sourceTab.Groups.Count; groupIndex++) {
					var sourceGroup = sourceTab.Groups[groupIndex];
					var destinationGroup = destinationTab.Groups[groupIndex];
					for(var itemIndex = 0; itemIndex < sourceGroup.Items.Count; itemIndex++) {
						var sourceItem = sourceGroup.Items[itemIndex] as MVCxRibbonTemplateItem;
						var destinationItem = destinationGroup.Items[itemIndex] as RibbonTemplateItem;
						if(sourceItem != null && destinationTab != null)
							destinationItem.Template = ContentControlTemplate<RibbonTemplateItemControl>.Create(
								sourceItem.Content, sourceItem.ContentMethod,
								typeof(RibbonTemplateItemControl));
					}
				}
			}
		}
	}
}
