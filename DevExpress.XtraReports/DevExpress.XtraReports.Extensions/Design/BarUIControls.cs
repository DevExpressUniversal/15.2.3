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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.Design
{
	[ToolboxItem(false)]
	public class XtraContextMenu : XtraContextMenuBase {
		#region inner classes
		public class CommandMenuItem : CommandMenuItemBase, ISupportReportCommand {
			ReportCommand ISupportReportCommand.Command { get { return CommandIDReportCommandConverter.GetReportCommand(invoker.CommandID); }
			}
			public CommandMenuItem(string text, XtraContextMenu owner, int imageIndex, CommandID cmdID, object param) : base (text, owner, imageIndex, cmdID, param) {
			}
			public CommandMenuItem(string text, XtraContextMenu owner, int imageIndex, CommandID cmdID) : base(text, owner, imageIndex, cmdID) {
			}
			public CommandMenuItem(string text, XtraContextMenu owner, CommandID cmdID) : base (text, owner, cmdID) {
			}
		}
		public class ReportToolMenuItem : BarButtonItem {
			Guid toolItemKind;
			DevExpress.Data.Utils.IToolItem reportToolItem;
			public DevExpress.Data.Utils.IToolItem ReportToolItem {
				get { return reportToolItem; }
				set { reportToolItem = value; }
			}
			public Guid ToolItemKind {
				get { return toolItemKind; }
				set { toolItemKind = value; }
			}
			public ReportToolMenuItem(Guid toolItemKind, Bitmap bitmap, string caption)
				: base() {
				this.toolItemKind = toolItemKind;
				this.Caption = caption;
				this.ImageIndex = imageCollection.Images.Add(bitmap);
			}
			protected override void OnClick(BarItemLink link) {
				base.OnClick(link);
				reportToolItem.ShowActivate();
			}
		}
		public class CommandMenuSubItem : BarSubItem, ISupportCommandParameters, ISupportReportCommand {
			CommandMenuItemInvoker invoker = new CommandMenuItemInvoker();
			public virtual CommandID CommandID { get { return invoker.CommandID; }
			}
			object[] ISupportCommandParameters.Parameters {
				get { return invoker.Params; }
				set { invoker.Params = value; }
			}
			ReportCommand ISupportReportCommand.Command { get { return CommandIDReportCommandConverter.GetReportCommand(invoker.CommandID); }
			}
			public CommandMenuSubItem(string text, XtraContextMenu owner, int imageIndex, CommandID cmdID, object param) : this (text, owner, imageIndex, cmdID) {
				invoker.Params = new object[] { param };
			}
			public CommandMenuSubItem(string text, XtraContextMenu owner, int imageIndex, CommandID cmdID) : base() {
				Caption = text;
				ImageIndex = imageIndex;
				invoker.Owner = owner;
				invoker.CommandID= cmdID;
			}
			public CommandMenuSubItem(string text, XtraContextMenu owner, CommandID cmdID) : this (text, owner, -1, cmdID) {
			}
		}
		#endregion //inner classes
		#region static
		static int AddImageToImageList(string resource) {
			return AddImageToImageList(resource, typeof(LocalResFinder));
		}
		#endregion //static
		private IDesignerHost designerHost;
		internal IDesignerHost DesignerHost { get { return designerHost; } 
		}
		public override void Show(Control ctl, Point pos, IServiceProvider provider) {
			designerHost = provider as IDesignerHost;
			base.Show(ctl, pos, provider);
		}
		protected override Control GetBarManagerForm(IServiceProvider provider) {
			return provider.GetService(typeof(ReportTabControl)) as ReportTabControl;
		}
		protected override UserLookAndFeel GetLookAndFeel(IServiceProvider provider) {
			ReportTabControl view = provider.GetService(typeof(ReportTabControl)) as ReportTabControl;
			return view != null ? view.LookAndFeel : UserLookAndFeel.Default;
		}
		protected override MenuCommandHandlerBase GetMenuCommandHandler(IServiceProvider serviceProvider) {
			return (MenuCommandHandlerBase)serviceProvider.GetService(typeof(MenuCommandHandler));
		}
		protected static Image GetImage(string resource) {
			return ResLoaderBase.LoadBitmap("TlbrImages." + resource, typeof(LocalResFinder), Color.Magenta);
		}
		#region AddItem methods
		public void AddMenuItem(string text, string resource, CommandID cmdID) {
			int index = AddImageToImageList(resource);
			AddItem(text, index,  cmdID);
		}
		public void AddMenuItem(string text, Image image, CommandID cmdID) {
			AddMenuItem(text, image, this, cmdID);
		}
		public void AddMenuItem(string text, CommandID cmdID) {
			AddItem(text, -1,  cmdID);
		}
		public void AddItem(string text, int imageIndex, CommandID cmdID) {
			AddMenuItem(text, imageIndex, this, cmdID, null);
		}
		public void AddMenuItem(string text, BarLinksHolder parentItem, string resource, CommandID cmdID) {
			AddMenuItem(text, resource, parentItem, cmdID, null);
		}
		public void AddMenuItem(string text, Image image, BarLinksHolder parentItem, CommandID cmdID) {
			AddMenuItem(text, image, parentItem, cmdID, null);
		}
		protected override BarSubItem CreateMenuSubItem(string text, int imageIndex, CommandID cmdID) {
			return new CommandMenuSubItem(text, this, imageIndex, cmdID);
		}
		protected override CommandMenuItemBase CreateCommandMenuItem(string text, int imageIndex, CommandID cmdID, object param) {
			return new CommandMenuItem(text, this, imageIndex, cmdID, param);
		}
		#endregion //AddItem methods
		protected override void UpdateItemState(BarLinksHolder barLinksHolder, IServiceProvider provider) {
			if(barLinksHolder == null) return;
			ReportCommandService reportCommandService = provider.GetService(typeof(ReportCommandService)) as ReportCommandService;
			bool visibleItemExists = false;
			bool enabledItemExists = false;
			foreach(BarItemLink link in barLinksHolder.ItemLinks) {
				if (link.Item is ReportToolMenuItem) {
					DevExpress.Data.Utils.IToolShell reportToolShell = (DevExpress.Data.Utils.IToolShell)provider.GetService(typeof(DevExpress.Data.Utils.IToolShell));
					if (reportToolShell != null) {
						DevExpress.Data.Utils.IToolItem reportToolItem = reportToolShell[((ReportToolMenuItem)link.Item).ToolItemKind];
						if (reportToolItem != null) {
							((ReportToolMenuItem)link.Item).ReportToolItem = reportToolItem;
							visibleItemExists = true;
							enabledItemExists = true;
						}
						else
							link.Item.Visibility = BarItemVisibility.Never;
					}
				} 
				if(!(link.Item is ISupportCommand)) { continue; }
				UpdateItemVisibility(link.Item);
				if(link.Item is ISupportReportCommand && reportCommandService != null) {
					CommandVisibility visibility = reportCommandService.GetCommandVisibility(((ISupportReportCommand)link.Item).Command);
					if((visibility & CommandVisibility.ContextMenu) == 0)
						link.Item.Visibility = BarItemVisibility.Never;
				}
				UpdateItemState(link.Item as BarLinksHolder, provider);
				visibleItemExists |= link.Item.Visibility == BarItemVisibility.Always;
				enabledItemExists |= link.Item.Enabled;
			}
			if(barLinksHolder is BarItem) {
				SetBarItemVisibility((BarItem)barLinksHolder, visibleItemExists);
				((BarItem)barLinksHolder).Enabled = enabledItemExists;
			}
		}
	}
	public class FieldDropMenu : XtraContextMenu {
		public FieldDropMenu() : this(null) {
		}
		public FieldDropMenu(IServiceProvider servProvider) {
			MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
			AddBindFieldToControlMenuItem(items, typeof(XRLabel), BandCommands.BindFieldToXRLabel);
			AddBindFieldToControlMenuItem(items, typeof(XRPictureBox), BandCommands.BindFieldToXRPictureBox);
			AddBindFieldToControlMenuItem(items, typeof(XRRichText), BandCommands.BindFieldToXRRichText);
			AddBindFieldToControlMenuItem(items, typeof(XRCheckBox), BandCommands.BindFieldToXRCheckBox);
			AddBindFieldToControlMenuItem(items, typeof(XRBarCode), BandCommands.BindFieldToXRBarCode);
			AddBindFieldToControlMenuItem(items, typeof(XRZipCode), BandCommands.BindFieldToXRZipCode);
			if(servProvider != null) {
				IMenuCreationService serv = servProvider.GetService<IMenuCreationService>();
				if(serv != null)
					serv.ProcessMenuItems(MenuKind.FieldDrop, items);
			}
			AddMenuItems(items, null, null);
		}
		void AddBindFieldToControlMenuItem(MenuItemDescriptionCollection items, Type type, CommandID cmdID) {
			DevExpress.XtraReports.UserDesigner.Native.LocalizableToolboxItem tbItem = new DevExpress.XtraReports.UserDesigner.Native.LocalizableToolboxItem(type);
			items.Add(new MenuItemDescription(tbItem.DisplayName, tbItem != null ? tbItem.Bitmap : null, cmdID));
		}
		public override void Show(Control ctl, Point pos, IServiceProvider provider) { 
			base.Show(ctl, pos, provider);
			UpdateParams(pos);
		}
		void UpdateParams(Point pos) {
			IDictionaryService dictionaryService = DesignerHost.GetService(typeof(IDictionaryService)) as IDictionaryService;
			DevExpress.Data.Browsing.DataInfo[] dataInfos = dictionaryService.GetValue("DataInfos") as DevExpress.Data.Browsing.DataInfo[];
			foreach(BarItemLink itemLink in ItemLinks) {
				if(!(itemLink.Item is ISupportCommandParameters))
					continue;
				((ISupportCommandParameters)itemLink.Item).Parameters = new object[] { new PointF(pos.X, pos.Y), dataInfos };
			}
		}
	}
	public class SelectionMenu : XtraContextMenu {
		MenuKind menuKind = MenuKind.Selection;
		public SelectionMenu() : this(null, MenuKind.Selection) {
		}
		public SelectionMenu(IServiceProvider servProvider, MenuKind menuKind) {
			this.menuKind = menuKind;
			AddMenuItems(servProvider);
		}
		protected override void UpdateItemState(BarLinksHolder barLinksHolder, IServiceProvider provider) {
			if(ReferenceEquals(barLinksHolder, this)) {
				ClearVerbs(barLinksHolder);
				IList<MenuItemDescription> items = GetVerbItemDescriptions();
				if(barLinksHolder.ItemLinks.Count > 0 && items.Count > 0) {
					AddSeparator();
					ApplySeparator(barLinksHolder.ItemLinks[0]);
				}
				for(int i = items.Count - 1; i >= 0; i--) {
					BarItem barItem = CreateCommandMenuItem(items[i]);
					barLinksHolder.ItemLinks.Insert(0, barItem);
				}
			}
			base.UpdateItemState(barLinksHolder, provider);
		}
		static void ClearVerbs(BarLinksHolder barLinksHolder) {
			for(int i = barLinksHolder.ItemLinks.Count - 1; i >= 0; i--) {
				BarItemLink link = barLinksHolder.ItemLinks[i];
				if(link.Item is ISupportCommand && ((ISupportCommand)link.Item).CommandID == VerbCommands.ExecuteVerb) {
					barLinksHolder.ItemLinks.RemoveAt(i);
					barLinksHolder.Manager.Items.Remove(link.Item);
				}
			}
		}
		IList<MenuItemDescription> GetVerbItemDescriptions() {
			MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
			if(DesignerHost == null)
				return items;
			ISelectionService selectionServ = DesignerHost.GetService<ISelectionService>();
			if(selectionServ == null) return items;
			IComponent component = selectionServ.PrimarySelection as IComponent;
			if(component == null) return items;
			ComponentDesigner designer = (ComponentDesigner)DesignerHost.GetDesigner(component);
			if(designer == null || designer.Verbs == null || designer.Verbs.Count == 0) return items;
			foreach(DesignerVerb verb in designer.Verbs) {
				if(verb.Visible && verb.Supported && verb.Enabled)
					items.Add(new MenuItemDescription(verb, verb.Text, null, VerbCommands.ExecuteVerb));
			}
			IMenuCreationService serv = DesignerHost.GetService<IMenuCreationService>();
			if(serv != null) serv.ProcessMenuItems(menuKind, items);
			return items;
		}
		protected virtual void AddMenuItems(IServiceProvider servProvider) {
			MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
			items.Add(new MenuItemDescription());
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_ViewCode), GetImage("ViewCode.bmp"), WrappedCommands.ViewCode));
			items.Add(new MenuItemDescription());
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_BringToFront), XRBitmaps.BringToFront, StandardCommands.BringToFront));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_SendToBack), XRBitmaps.SendToBack, StandardCommands.SendToBack));
			items.Add(new MenuItemDescription());
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_AlignToGrid), XRBitmaps.AlignToGrid, StandardCommands.AlignToGrid));
			items.Add(new MenuItemDescription());
			MenuItemDescriptionCollection subItems = new MenuItemDescriptionCollection();
			subItems.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableInsertRowAbove), GetImage("InsRowAbove.bmp"), TableCommands.InsertRowAbove));
			subItems.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableInsertRowBelow), GetImage("InsRowBelow.bmp"), TableCommands.InsertRowBelow));
			subItems.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableInsertColumnToLeft), GetImage("InsColumnToLeft.bmp"), TableCommands.InsertColumnToLeft));
			subItems.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableInsertColumnToRight), GetImage("InsColumnToRight.bmp"), TableCommands.InsertColumnToRight));
			subItems.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableInsertCell), GetImage("InsCell.bmp"), TableCommands.InsertCell));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableInsert), null, TableCommands.InsertRowAbove, subItems.ToArray()));
			subItems.Clear();
			subItems.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableDeleteRow), GetImage("DelRows.bmp"), TableCommands.DeleteRow));
			subItems.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableDeleteColumn), GetImage("DelColumns.bmp"), TableCommands.DeleteColumn));
			subItems.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableDeleteCell), GetImage("DelCells.bmp"), TableCommands.DeleteCell));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableDelete), null, TableCommands.DeleteRow, subItems.ToArray()));
			subItems.Clear();
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TableConvertToLabels), null, TableCommands.ConvertToLabels));
			items.Add(new MenuItemDescription());
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_Cut), XRBitmaps.Cut, StandardCommands.Cut));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_Copy), XRBitmaps.Copy, StandardCommands.Copy));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_Paste), XRBitmaps.Paste, StandardCommands.Paste));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_Delete), GetImage("Delete.png"), StandardCommands.Delete));
			items.Add(new MenuItemDescription());
			items.Add(new MenuItemDescription(ReportStringId.Cmd_AddSubBand.GetString(), XRBitmaps.SubBand, BandCommands.AddSubBand));
			AddInsertBandsMenuItems(subItems);
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_InsertBand), null, null, subItems.ToArray()));
			subItems.Clear();
			AddDetailReportMenuItems(items);
			subItems.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.UD_Capt_ZoomOut), XRBitmaps.ZoomOut, ZoomCommands.ZoomOut));
			subItems.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.UD_Capt_ZoomIn), XRBitmaps.ZoomIn, ZoomCommands.ZoomIn));
			subItems.Add(new MenuItemDescription());
			foreach(int zoomFactorInPercents in ZoomService.PredefinedZoomFactorsInPercents) {
				subItems.Add(new MenuItemDescription(zoomFactorInPercents / 100f, zoomFactorInPercents + "%", null, ZoomCommands.Zoom));
			}
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.UD_Capt_Zoom), null, null, subItems.ToArray()));
			subItems.Clear();
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_BandMoveUp), null, ReorderBandsCommands.MoveUp));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_BandMoveDown), null, ReorderBandsCommands.MoveDown));
			items.Add(new MenuItemDescription());
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_Properties), GetImage("Properties.bmp"), WrappedCommands.PropertiesWindow));
			if(servProvider != null) {
				IMenuCreationService serv = servProvider.GetService<IMenuCreationService>();
				if(serv != null)
					serv.ProcessMenuItems(menuKind, items);
			}
			AddMenuItems(items, null, holder => {
				CommandMenuSubItem subItem = holder as CommandMenuSubItem;
				if(subItem != null) {
					if(subItem.CommandID == ReportCommands.InsertDetailReport)
						subItem.Popup += new EventHandler(InsertDetailReport_Popup);
				}
			});
		}
		protected virtual void AddInsertBandsMenuItems(MenuItemDescriptionCollection items) {
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_TopMargin), XRBitmaps.TopMarginBand, BandCommands.InsertTopMarginBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_ReportHeader), XRBitmaps.ReportHeaderBand, BandCommands.InsertReportHeaderBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_PageHeader), XRBitmaps.PageHeaderBand, BandCommands.InsertPageHeaderBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_GroupHeader), XRBitmaps.GroupHeaderBand, BandCommands.InsertGroupHeaderBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_Detail), XRBitmaps.DetailBand, BandCommands.InsertDetailBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_GroupFooter), XRBitmaps.GroupFooterBand, BandCommands.InsertGroupFooterBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_ReportFooter), XRBitmaps.ReportFooterBand, BandCommands.InsertReportFooterBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_PageFooter), XRBitmaps.PageFooterBand, BandCommands.InsertPageFooterBand));
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_BottomMargin), XRBitmaps.BottomMarginBand, BandCommands.InsertBottomMarginBand));
		}
		protected virtual void AddDetailReportMenuItems(MenuItemDescriptionCollection items) {
			MenuItemDescription subItem = new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_InsertDetailReport), XRBitmaps.DetailReport, ReportCommands.InsertDetailReport);
			items.Add(new MenuItemDescription(ReportLocalizer.GetString(ReportStringId.Cmd_InsertDetailReport), null, ReportCommands.InsertDetailReport, new MenuItemDescription[] {subItem}));
		}
		void InsertDetailReport_Popup(object sender, EventArgs e) {
			string unboundItemName = ReportLocalizer.GetString(ReportStringId.Cmd_InsertUnboundDetailReport);
			MenuItemDescriptionCollection items = new MenuItemDescriptionCollection();
			items.Add(new MenuItemDescription(unboundItemName, XRBitmaps.DetailReport, ReportCommands.InsertDetailReport));
			items.Add(new MenuItemDescription());
			ObjectNameCollection objectNames = GetRelationNames();
			foreach(ObjectName objectName in objectNames)
				items.Add(new MenuItemDescription(objectName.FullName, String.Format("\"{0}\"", objectName.DisplayName), XRBitmaps.DetailReport, ReportCommands.InsertDetailReport));
			ClearSubitems((BarSubItem)sender);
			IMenuCreationService serv = DesignerHost.GetService<IMenuCreationService>();
			if(serv != null) serv.ProcessMenuItems(menuKind, items);
			AddMenuItems(items, (BarSubItem)sender, null);
		}
		ObjectNameCollection GetRelationNames() {
			ReportDesigner reportDesigner = DesignerHost.GetDesigner(DesignerHost.RootComponent) as ReportDesigner;
			return reportDesigner.GetRelationNames();
		}
		void ClearSubitems(BarSubItem parentItem) {
			while(parentItem.ItemLinks.Count > 0)
				parentItem.Manager.Items.Remove(parentItem.ItemLinks[0].Item);
			parentItem.ItemLinks.Clear();
		}
	}
}
