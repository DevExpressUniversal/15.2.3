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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraBars;
using System.Drawing.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Native;
using System.Drawing;
using DevExpress.XtraReports.Design;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
namespace DevExpress.XtraReports.UserDesigner.Native {
	public class ToolBoxBarsConfigurator : BarManagerConfigurator {
		public static string GetBarName(string category, string defaultCategory) {
			return string.IsNullOrEmpty(defaultCategory) || defaultCategory != category ?
				string.Format("{0} ({1})", XRDesignBarManagerBarNames.Toolbox, category) :
				XRDesignBarManagerBarNames.Toolbox;
		}
		public static bool TryGetCategory(string barName, out string category) {
			if(barName == XRDesignBarManagerBarNames.Toolbox) {
				category = ReportLocalizer.GetString(ReportStringId.UD_XtraReportsToolboxCategoryName);
				return true;
			}
			Match m = Regex.Match(barName, @"Toolbox \((?<category>.*)\)");
			category = m.Success ? m.Groups["category"].Value : string.Empty;
			return m.Success;
		}
		IServiceProvider ServProvider { get { return this.XRDesignBarManager.XRDesignPanel; } }
		XRDesignBarManager XRDesignBarManager {
			get { return manager as XRDesignBarManager; }
		}
		public ToolBoxBarsConfigurator(XRDesignBarManager manager)
			: base(manager) {
		}
		public override void ConfigInternal() {
			IToolboxService toolboxService = this.ServProvider.GetService(typeof(IToolboxService)) as IToolboxService;
			if(toolboxService == null)
				return;
			this.XRDesignBarManager.BeginUpdate();
			XRToolboxService xrToolboxService = toolboxService as XRToolboxService;
			int i = 0;
			foreach(string category in toolboxService.CategoryNames) {
				string barName = GetBarName(category, xrToolboxService.DefaultCategoryName);
				Bar bar = this.XRDesignBarManager.Bars[barName];
				if(bar == null) {
					bar = new Bar(this.XRDesignBarManager);
					bar.OptionsBar.AllowQuickCustomization = false;
					AddBar(bar, barName, i, i, BarDockStyle.Left, category);
				}
				XRDesignBarManager.SetToolboxType(bar, GetToolboxType(category));
				XRDesignBarManager.ApplyCategory(bar, category);
				bar.Text = category;
				i++;
			}
			this.XRDesignBarManager.EndUpdate();
		}
		static ToolboxType GetToolboxType(string category) {
			return category == ReportLocalizer.GetString(ReportStringId.UD_XtraReportsToolboxCategoryName) ? ToolboxType.Standard : ToolboxType.Custom;
		}
	}
	public class ToolBoxBarItemsConfigurator : BarManagerConfigurator {
		string category;
		Bar bar;
		XRDesignBarManager XRDesignBarManager {
			get { return manager as XRDesignBarManager; }
		}
		IServiceProvider ServProvider { get { return this.XRDesignBarManager.XRDesignPanel; } }
		public ToolBoxBarItemsConfigurator(XRDesignBarManager manager, Bar bar, string category)
			: base(manager) {
			this.category = category;
			this.bar = bar;
		}
		protected override bool ShouldAddBarItemToContainer {
			get { return false; }
		}
		public override void ConfigInternal() {
			this.XRDesignBarManager.BeginUpdate();
			XRToolboxService toolboxSvc = ServProvider.GetService(typeof(IToolboxService)) as XRToolboxService;
			IDesignerHost designerHost = ServProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			CreateBarItems(bar, toolboxSvc, designerHost, category);
			this.XRDesignBarManager.EndUpdate();
		}
		void CreateBarItems(Bar bar, XRToolboxService toolboxSvc, IDesignerHost designerHost, string category) {
			CreateCursorItem(bar);
			ToolboxItem[][] subcategorizedItems = XRToolboxService.GroupItemsBySubCategory(toolboxSvc.GetToolboxItems(category), designerHost);
			if(subcategorizedItems == null)
				return;
			foreach(ToolboxItem[] subcategory in subcategorizedItems)
				for(int i = 0; i < subcategory.Length; i++) {
					ToolboxItem item = subcategory[i];
					Image image = toolboxSvc != null ? toolboxSvc.GetImage(item) : null;
					string caption = item is LocalizableToolboxItem ? ((LocalizableToolboxItem)item).DisplayName : item.DisplayName;
					BarItem barItem = CreateBarItem(caption, image, false, item);
					AddBarItem(bar, barItem, caption, caption, caption, barItem.ImageIndex, i == 0);
				}
		}
		void CreateCursorItem(Bar bar) {
			string caption = ReportLocalizer.GetString(ReportStringId.UD_XtraReportsPointerItemCaption);
			XRToolboxService toolboxSvc = ServProvider.GetService(typeof(IToolboxService)) as XRToolboxService;
			Image image = null;
			if(toolboxSvc != null)
				image = toolboxSvc.GetImageByType(typeof(object));
			BarItem barItem = CreateBarItem(caption, image, true, null);
			AddBarItem(bar, barItem, caption, caption, caption, barItem.ImageIndex, false);
		}
		static BarItem CreateBarItem(string caption, Image glyph, bool down, ToolboxItem tag) {
			BarCheckItem btn = new BarCheckItem();
			btn.Caption = caption;
			btn.Glyph = glyph;
			btn.AllowAllUp = true;
			btn.GroupIndex = btn.GetHashCode();
			btn.Checked = down;
			btn.Tag = tag;
			return btn;
		}
	}
}
