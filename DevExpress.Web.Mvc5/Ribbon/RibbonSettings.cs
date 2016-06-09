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

using DevExpress.Web;
using System;
namespace DevExpress.Web.Mvc {
	public class RibbonSettings : SettingsBase {
		public RibbonSettings() {
			AllowMinimize = true;
			Minimized = false;
			Tabs = new MVCxRibbonTabCollection();
			ContextTabCategories = new MVCxRibbonContextTabCategoryCollection();
			TabDataFields = new RibbonTabDataFields(null);
			GroupDataFields = new RibbonGroupDataFields(null);
			ItemDataFields = new RibbonItemDataFields(null);
			ShowFileTab = true;
			ShowTabs = true;
			ShowGroupLabels = true;
			OneLineMode = false;
			StylesTabControl = new RibbonTabControlStyles(null);
			StylesPopupMenu = new RibbonMenuStyles(null);
		}
		public bool AllowMinimize { get; set; }
		public bool Minimized { get; set; }
		public int ActiveTabIndex { get; set; }
		public RibbonClientSideEvents ClientSideEvents { get { return (RibbonClientSideEvents)ClientSideEventsInternal; } }
		public bool ShowFileTab { get; set; }
		public bool ShowTabs { get; set;}
		public bool ShowGroupLabels { get; set; }
		public bool OneLineMode { get; set; }
		public MVCxRibbonTabCollection Tabs { get; private set; }
		public MVCxRibbonContextTabCategoryCollection ContextTabCategories { get; private set; }
		public RibbonTabDataFields TabDataFields { get; private set; }
		public RibbonGroupDataFields GroupDataFields { get; private set; }
		public RibbonItemDataFields ItemDataFields { get; private set; }
		public RibbonImages Images { get { return (RibbonImages)ImagesInternal; } }
		public RibbonStyles Styles { get { return (RibbonStyles)StylesInternal; } }
		public RibbonTabControlStyles StylesTabControl { get; private set; }
		public RibbonMenuStyles StylesPopupMenu { get; private set; }
		public RibbonTabEventHandler TabDataBound { get; set; }
		public RibbonGroupEventHandler GroupDataBound { get; set; }
		public RibbonItemEventHandler ItemDataBound { get; set; }
		protected internal string FileTabTemplateContent { get; set; }
		protected internal Action<TabControlTemplateContainer> FileTabTemplateContentMethod { get; set; }
		public void SetFileTabTemplateContent(Action<TemplateContainerBase> contentMethod) {
			FileTabTemplateContentMethod = contentMethod;
		}
		public void SetFileTabTemplateContent(string content) {
			FileTabTemplateContent = content;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new RibbonClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new RibbonImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new RibbonStyles(null);
		}
	}
}
