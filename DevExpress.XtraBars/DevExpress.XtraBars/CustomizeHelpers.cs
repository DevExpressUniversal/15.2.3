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
using System.Reflection;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.XtraBars.Customization.Helpers {
	internal class Menus {
		internal DevExpress.XtraBars.PopupMenu categoriesMenu;
		internal DevExpress.XtraBars.BarManager barManager1;
		internal DevExpress.XtraBars.BarButtonItem subMenuEditorCommand;
		internal DevExpress.XtraBars.BarButtonItem deleteCategory;
		internal DevExpress.XtraBars.BarButtonItem addCategory;
		internal DevExpress.XtraBars.BarButtonItem renameCategory;
		internal DevExpress.XtraBars.BarButtonItem insertCategory;
		internal DevExpress.XtraBars.BarButtonItem visibleCategory;
		internal DevExpress.XtraBars.BarButtonItem addCommand;
		internal DevExpress.XtraBars.BarButtonItem resetGlyphCommand;
		internal DevExpress.XtraBars.BarButtonItem moveUpCommand;
		internal DevExpress.XtraBars.BarButtonItem moveDownCommand;
		internal DevExpress.XtraBars.BarButtonItem deleteCommand;
		internal DevExpress.XtraBars.BarButtonItem clearCommand;
		internal DevExpress.XtraBars.PopupMenu commandsMenu;
		internal CheckButton categoriesBtn, commandsBtn;
		DevExpress.XtraBars.Customization.CustomizationForm form;
		public Menus(DevExpress.XtraBars.Customization.CustomizationForm form, Label label1, Label label2) {
			this.form = form;
			InitializeComponent();
			categoriesBtn = new CheckButton();
			categoriesBtn.DropDownMenu = this.categoriesMenu;
			categoriesBtn.Text = "... modify";
			categoriesBtn.Bounds = label1.Bounds;
			categoriesBtn.Parent = label1.Parent;
			label1.Visible = false;
			commandsBtn = new CheckButton();
			commandsBtn.DropDownMenu = this.commandsMenu;
			commandsBtn.Text = "... modify";
			commandsBtn.Bounds = label2.Bounds;
			commandsBtn.Parent = label2.Parent;
			label2.Visible = false;
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.visibleCategory = new DevExpress.XtraBars.BarButtonItem();
			this.addCommand = new DevExpress.XtraBars.BarButtonItem();
			this.resetGlyphCommand = new DevExpress.XtraBars.BarButtonItem();
			this.moveUpCommand = new DevExpress.XtraBars.BarButtonItem();
			this.subMenuEditorCommand = new DevExpress.XtraBars.BarButtonItem();
			this.categoriesMenu = new DevExpress.XtraBars.PopupMenu();
			this.insertCategory = new DevExpress.XtraBars.BarButtonItem();
			this.barManager1 = new DevExpress.XtraBars.BarManager();
			this.deleteCategory = new DevExpress.XtraBars.BarButtonItem();
			this.moveDownCommand = new DevExpress.XtraBars.BarButtonItem();
			this.renameCategory = new DevExpress.XtraBars.BarButtonItem();
			this.addCategory = new DevExpress.XtraBars.BarButtonItem();
			this.deleteCommand = new DevExpress.XtraBars.BarButtonItem();
			this.commandsMenu = new DevExpress.XtraBars.PopupMenu();
			this.clearCommand = new DevExpress.XtraBars.BarButtonItem();
			((System.ComponentModel.ISupportInitialize)(this.categoriesMenu)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.commandsMenu)).BeginInit();
			this.visibleCategory.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
			this.visibleCategory.Caption = "Visible";
			this.visibleCategory.Manager = this.barManager1;
			this.visibleCategory.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.visibleCategory_ItemClick);
			this.addCommand.Caption = "Add...";
			this.addCommand.ImageIndex = 0;
			this.addCommand.Manager = this.barManager1;
			this.addCommand.ItemShortcut = new BarShortcut(Keys.Insert);
			this.addCommand.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.addCommand_ItemClick);
			this.resetGlyphCommand.Caption = "Reset Glyph";
			this.resetGlyphCommand.ImageIndex = -1;
			this.resetGlyphCommand.Manager = this.barManager1;
			this.resetGlyphCommand.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.resetGlyphCommand_ItemClick);
			this.moveUpCommand.Caption = "Move Up";
			this.moveUpCommand.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Up));
			this.moveUpCommand.CloseSubMenuOnClick = false;
			this.moveUpCommand.ImageIndex = 1;
			this.moveUpCommand.Manager = this.barManager1;
			this.moveUpCommand.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.moveUpCommand_ItemClick);
			this.subMenuEditorCommand.Caption = "SubMenu Editor";
			this.subMenuEditorCommand.ImageIndex = -1;
			this.subMenuEditorCommand.Manager = this.barManager1;
			this.subMenuEditorCommand.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.subMenuEditorCommand_ItemClick);
			this.categoriesMenu.LinksPersistInfo = new DevExpress.XtraBars.LinksInfo(new object[] {new DevExpress.XtraBars.LinkPersistInfo(this.addCategory),
																									  new DevExpress.XtraBars.LinkPersistInfo(this.insertCategory),
																									  new DevExpress.XtraBars.LinkPersistInfo(this.renameCategory),
																									  new DevExpress.XtraBars.LinkPersistInfo(this.visibleCategory, true),
																									  new DevExpress.XtraBars.LinkPersistInfo(this.deleteCategory, true)});
			this.categoriesMenu.Manager = this.barManager1;
			this.insertCategory.Caption = "Insert...";
			this.insertCategory.Manager = this.barManager1;
			this.insertCategory.ItemShortcut = new BarShortcut(Keys.Insert);
			this.insertCategory.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.insertCategory_ItemClick);
			this.barManager1.Categories.Add("Commands");
			this.barManager1.Categories.Add("Categories");
			this.barManager1.Items.Add(this.addCategory);
			this.barManager1.Items.Add(this.insertCategory);
			this.barManager1.Items.Add(this.renameCategory);
			this.barManager1.Items.Add(this.visibleCategory);
			this.barManager1.Items.Add(this.deleteCategory);
			this.barManager1.Items.Add(this.addCommand);
			this.barManager1.Items.Add(this.deleteCommand);
			this.barManager1.Items.Add(this.clearCommand);
			this.barManager1.Items.Add(this.resetGlyphCommand);
			this.barManager1.Items.Add(this.moveUpCommand);
			this.barManager1.Items.Add(this.moveDownCommand);
			this.barManager1.Items.Add(this.subMenuEditorCommand);
			this.deleteCategory.Caption = "Delete...";
			this.deleteCategory.ImageIndex = 3;
			this.deleteCategory.Manager = this.barManager1;
			this.deleteCategory.ItemShortcut = new BarShortcut(Keys.Delete);
			this.deleteCategory.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.deleteCategory_ItemClick);
			this.moveDownCommand.CloseSubMenuOnClick = false;
			this.moveDownCommand.Caption = "Move Down";
			this.moveDownCommand.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Down));
			this.moveDownCommand.ImageIndex = 2;
			this.moveDownCommand.Manager = this.barManager1;
			this.moveDownCommand.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.moveDownCommand_ItemClick);
			this.renameCategory.Caption = "Rename...";
			this.renameCategory.Manager = this.barManager1;
			this.renameCategory.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.renameCategory_ItemClick);
			this.addCategory.Caption = "Add...";
			this.addCategory.ImageIndex = 0;
			this.addCategory.Manager = this.barManager1;
			this.addCategory.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.addCategory_ItemClick);
			this.deleteCommand.Caption = "Delete";
			this.deleteCommand.ImageIndex = 3;
			this.deleteCommand.Manager = this.barManager1;
			this.deleteCommand.ItemShortcut = new BarShortcut(Keys.Delete);
			this.deleteCommand.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.deleteCommand_ItemClick);
			this.commandsMenu.LinksPersistInfo = new DevExpress.XtraBars.LinksInfo(new object[] {new DevExpress.XtraBars.LinkPersistInfo(this.addCommand),
																									new DevExpress.XtraBars.LinkPersistInfo(this.deleteCommand),
																									new DevExpress.XtraBars.LinkPersistInfo(this.clearCommand),
																									new DevExpress.XtraBars.LinkPersistInfo(this.resetGlyphCommand, true),
																									new DevExpress.XtraBars.LinkPersistInfo(this.moveUpCommand, true),
																									new DevExpress.XtraBars.LinkPersistInfo(this.moveDownCommand),
																									new DevExpress.XtraBars.LinkPersistInfo(this.subMenuEditorCommand, true)});
			this.commandsMenu.Manager = this.barManager1;
			this.clearCommand.Caption = "Clear";
			this.clearCommand.Manager = this.barManager1;
			this.clearCommand.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(form.clearCommand_ItemClick);
			((System.ComponentModel.ISupportInitialize)(this.categoriesMenu)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.commandsMenu)).EndInit();
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class CheckButton : SimpleButton, DevExpress.XtraBars.Controls.IBarObject {
		PopupMenu dropDownMenu = null;
		bool IBarObject.ShouldCloseOnLostFocus(Control newFocus) { return true; }
		bool DevExpress.XtraBars.Controls.IBarObject.IsBarObject { get { return true; } }
		BarMenuCloseType DevExpress.XtraBars.Controls.IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			return BarMenuCloseType.None;
		}
		bool IBarObject.ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) { return true; }
		BarManager DevExpress.XtraBars.Controls.IBarObject.Manager { 
			get { 
				if(DropDownMenu == null) return null;
				return DropDownMenu.Manager;
			} 
		}
		public PopupMenu DropDownMenu {
			get { return dropDownMenu; }
			set {
				if(value == DropDownMenu) return;
				if(DropDownMenu != null) 
					DropDownMenu.CloseUp -= new EventHandler(OnDropDownCloseUp);
				dropDownMenu = value;
				if(DropDownMenu != null) 
					DropDownMenu.CloseUp += new EventHandler(OnDropDownCloseUp);
			}
		}
		protected override bool DownCore { get { return Opened; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(ViewInfo.IsPressed) {
				if(Opened) 
					HideDropDown();
				else
					ShowDropDown();
			}
		}
		protected bool Opened { get { return DropDownMenu != null && DropDownMenu.Opened; } }
		void OnDropDownCloseUp(object sender, EventArgs e) {
			ViewInfo.IsPressed = false;
			UpdateViewInfoState();
		}
		protected void ShowDropDown() {
			if(DropDownMenu == null) return;
			Point p = Parent.PointToScreen(new Point(Bounds.X, Bounds.Bottom));
			DropDownMenu.ShowPopup(p);
		}
		protected void HideDropDown() {
			if(DropDownMenu == null) return;
			DropDownMenu.HidePopup();
		}
	}
	[ToolboxItem(false)]
	public class ToolBarsPopup : PopupMenu {
		BarToolbarsListItem toolbarList;
		public ToolBarsPopup(BarManager manager) {
			toolbarList = new BarToolbarsListItem(true, manager);
			toolbarList.Manager = manager;
			Manager = manager;
			AddItem(toolbarList);
		}
		protected override void OnBeforePopup(CancelEventArgs e) {
			base.OnBeforePopup(e);
			if(e.Cancel) return;
			if(Manager != null) Manager.RaiseShowToolbarsContextMenu(new ShowToolbarsContextMenuEventArgs(ItemLinks));
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				HidePopup();
				toolbarList.Dispose();
			}
			base.Dispose(disposing);
		}
	}
	public class DesignTimeManager : IDisposable {
		BarManager designManager, manager;
		PopupMenu designMenu;
		internal DesignTimeManager(BarManager manager) {
			this.manager = manager;
			Init();
		}
		public virtual void Dispose(){ 
			DesignMenu = null;
			this.manager = null;
			this.designManager.Dispose();
		}
		protected PopupMenu DesignMenu { 
			get { return designMenu; }
			set {
				if(DesignMenu == value) return;
				if(designMenu != null)
					designMenu.Dispose();
				designMenu = value;
			}
		}
		public BarManager Manager { get { return manager; } } 
		public BarManager DesignManager { get { return designManager; } } 
		protected virtual void Init() {
			designManager = new BarManager(true);
			designManager.DockingEnabled = false;
			designManager.Controller = Manager.Controller;
			designManager.AllowShowToolbarsPopup = false;
			designManager.Form = Manager.Form;
			UpdateBarItemImages(designManager);
		}
		private void UpdateBarItemImages(BarManager designManager) {
			designManager.BarItemsImages.Clear();
			foreach(Image img in Manager.BarItemsImages.Images) {
				designManager.BarItemsImages.AddImage(img);
			}
		}
		public void CloseModalTextBox() {
			if(Manager == null) return;
			BarSelectionInfo selInfo = Manager.SelectionInfo;
			if(selInfo == null || !selInfo.ModalTextBoxActive) return;
			selInfo.ModalTextBox.DialogResult = DialogResult.Cancel;
		}
		class ModalTextBoxEx : ModalTextBox {
			BarManager manager;
			public ModalTextBoxEx(BarManager manager) {
				this.manager = manager;
			}
			Timer captureTimer;
			protected override void Dispose(bool disposing) {
				if(captureTimer != null) captureTimer.Dispose();
				this.captureTimer = null;
				base.Dispose(disposing);
			}
			protected override void OnShown(EventArgs e) {
				base.OnShown(e);
				GetEditorControl().MouseDown += new MouseEventHandler(Editor_MouseDown);
				captureTimer = new Timer() { Interval = 50, Enabled = true };
				captureTimer.Tick += (s, ee) => {
					if(!IsHandleCreated) return;
					CaptureMouse();
					captureTimer.Start();
				};
				CaptureMouse();
			}
			Control GetEditorControl() {
				return (Editor.Controls.Count == 0) ? Editor : Editor.Controls[0];
			}
			void CaptureMouse() {
				GetEditorControl().Capture = true;
			}
			void Editor_MouseDown(object sender, MouseEventArgs e) {
				Point point = PointToClient(Editor.PointToScreen(new Point(e.X, e.Y)));
				if(ClientRectangle.Contains(point)) return;
				Point screen = PointToScreen(point);
				foreach(BarItem i in manager.Items) {
					if(i.Links.Count == 0) continue;
					foreach(BarItemLink link in i.Links) {
						if(!link.Visible || link.BarControl == null) continue;
						if(link.ScreenBounds.Contains(screen)) {
							LinkSelectedByClosing = link;
							break;
						}
					}
				}
				DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			internal BarItemLink LinkSelectedByClosing { get; set; }
		}
		public virtual void EditLinkCaption(BarItemLink link, bool userCaption) {
			BarLinkViewInfo linkInfo = link.LinkViewInfo;
			if(linkInfo == null || (link.BarControl == null && link.RadialMenu == null) || Manager.SelectionInfo.ModalTextBoxActive) return;
			BarSelectionInfo selInfo = Manager.SelectionInfo;
			ModalTextBoxEx mt = new ModalTextBoxEx(Manager);
			mt.EditText = userCaption ? link.Caption : link.Item.Caption;
			mt.EditFont = link.Font;
			if(link.Item.Appearance.Options.UseForeColor)
				mt.ForeColor = link.Item.Appearance.ForeColor;
			Rectangle bounds = Rectangle.Empty;
			if(bounds.IsEmpty) bounds = linkInfo.Bounds;
			bounds.Location = link.LinkPointToScreen(bounds.Location);
			bounds.Width = Math.Max(60, bounds.Width);
			bounds.Height = Math.Max(bounds.Height, mt.CalcBestFit().Height);
			mt.Bounds = bounds;
			DialogResult rt = DialogResult.Cancel;
			try {
				selInfo.ModalTextBoxActive = true;
				selInfo.ModalTextBox = mt;
				if(link.RadialMenu != null)
					rt = mt.ShowDialog();
				else 
					rt = mt.ShowDialog(link.BarControl.FindForm());
				mt.Dispose();
			}
			finally {
				selInfo.ModalTextBox = null;
				selInfo.ModalTextBoxActive = false;
			}
			if(rt == DialogResult.OK) {
				if(userCaption)
					link.UserCaption = mt.EditText;
				else
					link.Item.Caption = mt.EditText;
				if(Manager != null) {
					Control c = Manager.GetTopMostControl();
					if(c != null && c.IsHandleCreated) {
						if(mt.LinkSelectedByClosing != null) link = mt.LinkSelectedByClosing;
						c.BeginInvoke((MethodInvoker)delegate {
							Manager.SelectionInfo.CustomizeSelectLink(link);
						});
					}
			}
		}
		}
		public void CloseMenu() {
			DesignMenu = null;
			designManager.SelectionInfo.CloseAllPopups();
		}
		public void ShowLinkMenu(BarItemLink link) {
			if(Manager.SelectionInfo.ModalTextBoxActive) return;
			DesignMenu = new ItemDesignMenu(Manager, this.designManager, link);
			DesignMenu.ShowPopup(Control.MousePosition);
		}
		protected virtual DesignTimeCreateItemMenu CreateItemsMenu(BarDesignTimeItemLink link) { 
			return new DesignTimeCreateItemMenu(this.designManager, this.Manager, link);
		}
		public void ShowCreateItemMenu(BarDesignTimeItemLink link) {
			if(Manager.SelectionInfo.ModalTextBoxActive) return;
			if((Control.MouseButtons & MouseButtons.Left) != 0) this.designManager.IgnoreLeftMouseUp ++;
			DesignMenu = CreateItemsMenu(link);
			DesignMenu.ShowPopup(Control.MousePosition);
		}
		[ToolboxItem(false)]
		public class ItemDesignMenu : PopupMenu {
			ArrayList items;
			static BarItemPaintStyle[] checkState = new BarItemPaintStyle[] { 
						BarItemPaintStyle.Standard, BarItemPaintStyle.Caption, BarItemPaintStyle.CaptionInMenu, 
						BarItemPaintStyle.CaptionGlyph };
			BarItemLink designLink;
			bool isDesignMode;
			public ItemDesignMenu(BarManager manager, BarManager designManager, BarItemLink link) {
				items = new ArrayList();
				this.isDesignMode = manager.IsDesignMode;
				MenuDrawMode = XtraBars.MenuDrawMode.SmallImagesText;
				Manager = designManager;
				designLink = link;
				InitMenu();
			}
			protected internal override bool IsDesignMode { get { return isDesignMode || base.IsDesignMode; } }
			const int ItemRecentlyUsed = 9, ItemBeginGroup = 7, ItemVisible = 8, ItemReset = 0, ItemDelete = 1, ItemName = 2;
			public virtual void InitMenu() {
				BarButtonItem bItem;
				BarItem item;
				string[] menuStrings = Manager.GetString(BarString.ToolBarMenu).Split('$');
				for(int n = 0; n < menuStrings.Length; n++) {
					string caption = menuStrings[n];
					string realCaption = caption[0] == '!' ? caption.Substring(1) : caption;
					bool beginGroup = (realCaption != caption);
					if(n == ItemRecentlyUsed && !IsDesignMode) continue;
					if(n == ItemName) {
						if(IsDesignMode) {
							bItem = new BarButtonItem(Manager, true);
							bItem.Caption = "Chang&e Caption";
							bItem.ItemClick += new ItemClickEventHandler(OnEditItemClick);
							AddItem(bItem).BeginGroup = true;
							beginGroup = false;
							realCaption = "Change &User Caption";
							item = bItem = new BarButtonItem(null, true);
							bItem.ItemClick += new ItemClickEventHandler(OnUserEditItemClick);
						}
						else {
							BarEditItem editItem = new BarEditItem(true);
							editItem.Width = 100;
							editItem.EditValue = designLink.Caption;
							editItem.Manager = Manager;
							editItem.Edit = Manager.RepositoryItems.Add("TextEdit");
							editItem.HiddenEditor += new ItemClickEventHandler(OnCaptionChanged);
							item = editItem;
						}
					} else {
						bItem = new BarButtonItem(null, true);
						if(n > 2)
							bItem.ButtonStyle = BarButtonStyle.Check;
						if(n > 2 && n < ItemBeginGroup) {
							bItem.GroupIndex = 1;
							bItem.Down = designLink.PaintStyle == checkState[n - 3];
						}
						if(n == ItemBeginGroup) bItem.Down = designLink.BeginGroup;
						if(n == ItemVisible) bItem.Down = designLink.Visible;
						if(n == ItemRecentlyUsed) bItem.Down = designLink.MostRecentlyUsed;
						item = bItem;
						bItem.ItemClick += new ItemClickEventHandler(OnItemClick);
					}
					item.Caption = realCaption;
					item.Manager = Manager;
					item.Tag = n;
					this.AddItem(item).BeginGroup = beginGroup;
					items.Add(item);
				}
			}
			protected void OnEditItemClick(object sender, ItemClickEventArgs e) {
				this.designLink.Manager.Helper.CustomizationManager.DesignTimeManager.EditLinkCaption(this.designLink, !IsDesignMode);
			}
			protected void OnUserEditItemClick(object sender, ItemClickEventArgs e) {
				this.designLink.Manager.Helper.CustomizationManager.DesignTimeManager.EditLinkCaption(this.designLink, true);
			}
			private void OnCaptionChanged(object sender, ItemClickEventArgs e) {
				BarEditItem editItem = e.Item as BarEditItem;
				string name = editItem.EditValue == null ? "" : editItem.EditValue.ToString();
				designLink.UserCaption = name;
			}
			void OnItemClick(object sender, ItemClickEventArgs e) {
				int index = (int)e.Item.Tag;
				if(index == -1) return;
				if(index > 2 && index < ItemBeginGroup) {
					if((e.Item as BarButtonItem).Down)
						designLink.UserPaintStyle = checkState[index - 3];
				}
				if(index == ItemRecentlyUsed)
					designLink.MostRecentlyUsed = (e.Item as BarButtonItem).Down;
				if(index == ItemBeginGroup)
					designLink.BeginGroup = (e.Item as BarButtonItem).Down;
				if(index == ItemVisible)
					designLink.Visible = (e.Item as BarButtonItem).Down;
				if(index == ItemReset) {
					designLink.Reset();
					designLink.LayoutChanged();
				}
				if(index == ItemDelete) {
					designLink.Dispose();
				}
			}
		}
		[ToolboxItem(false)]
		public class DesignTimeCreateItemMenu : PopupMenu {
			BarItemLink designLink;
			BarManager sourceManager;
			public DesignTimeCreateItemMenu(BarManager designManager, BarManager sourceManager, BarItemLink link) {
				this.Manager = designManager;
				this.Manager.Images = Manager.BarItemsImages;
				this.MenuDrawMode = XtraBars.MenuDrawMode.SmallImagesText;
				this.designLink = link;
				this.sourceManager = sourceManager;
				InitMenu(link.Holder);
			}
			public class EditorInfo {
				public BarItemInfo ItemInfo;
				public EditorClassInfo Editor;
				public EditorInfo(BarItemInfo itemInfo, EditorClassInfo editor) {
					this.ItemInfo = itemInfo;
					this.Editor = editor;
				}
			}
			public static void CreateEditors(BarLinksHolder holder, BarItemInfo info, BarManager manager, ItemClickEventHandler handler) {
				EditorClassInfoCollection editors = EditorRegistrationInfo.Default.Editors;
				foreach(EditorClassInfo editor in editors) {
					if(!editor.DesignTimeVisible || editor.AllowInplaceEditing == ShowInContainerDesigner.Never) continue;
					BarButtonItem item = new BarButtonItem(null, true);
					item.Caption = editor.Name;
					item.Glyph = editor.Image as Bitmap;
					item.ItemClick += handler; 
					item.Tag = new EditorInfo(info, editor);
					item.Manager = manager;
					holder.ItemLinks.Add(item);
				}
			}
			protected internal BarManager SourceManager { get { return sourceManager; } }
			public virtual void InitMenu(BarLinksHolder holder) {
				BarItemInfoCollection resCollection = new BarItemInfoCollection(Manager.PaintStyle);
				BarItemInfoCollection addCollection = new BarItemInfoCollection(Manager.PaintStyle);
				SourceManager.FillAdditionalBarItemInfoCollection(addCollection);
				resCollection.AddCollection(addCollection);
				resCollection.AddCollection(Manager.PaintStyle.ItemInfoCollection);
				for(int n = 0; n < resCollection.Count; n++) {
					BarItemInfo info = resCollection[n];
					ICustomizationMenuFilterSupports filter = holder as ICustomizationMenuFilterSupports;
					if(filter != null && !filter.ShouldShowItem(info.LinkType)) continue;
					if(!info.DesignTimeVisible) continue;
					BarButtonItem bItem;
					if(info.ItemType.Equals(typeof(BarEditItem)) || info.ItemType.IsSubclassOf(typeof(BarEditItem))) {
						BarButtonItem subItem = new BarButtonItem(null, true);
						subItem.Caption = info.GetCaption();
						subItem.ImageIndex = info.ImageIndex;
						subItem.Tag = info;
						subItem.Manager = Manager;
						PopupMenu menu = new PopupMenu();
						menu.MenuDrawMode = XtraBars.MenuDrawMode.SmallImagesText;
						menu.Manager = Manager;
						CreateEditors(menu, info, Manager, new ItemClickEventHandler(OnCreateEditorClick));
						subItem.DropDownControl = menu;
						subItem.ButtonStyle = BarButtonStyle.DropDown;
						bItem = subItem;
					} else
						bItem = new BarButtonItem(null, true);
					bItem.ItemClick += new ItemClickEventHandler(OnItemClick);
					bItem.Caption = info.GetCaption();
					bItem.ImageIndex = info.ImageIndex;
					bItem.Tag = info;
					bItem.Manager = Manager;
					this.AddItem(bItem);
				}
			}
			protected void OnCreateEditorClick(object sender, ItemClickEventArgs e) {
				EditorInfo info = e.Item.Tag as EditorInfo;
				if(info == null || info.ItemInfo == null) return;
				CreateItem(info.ItemInfo, info.Editor == null ? null : info.Editor.Name);
			}
			protected void OnItemClick(object sender, ItemClickEventArgs e) {
				BarItemInfo info = e.Item.Tag as BarItemInfo;
				if(info == null) return;
				CreateItem(info, null);
			}
			protected void CreateItem(BarItemInfo info, object arguments) {
				BarItem item = CreateItem(this.designLink.Manager, info, arguments);
				if(item == null) return;
				Bar bar = this.designLink.Bar;
				BarItemLink link = null;
				BarItemLinksControl linksControl = this.designLink.BarControl as BarItemLinksControl;
				PopupMenuBarControl menuControl = this.designLink.BarControl as PopupMenuBarControl;
				GalleryDropDownBarControl galleryControl = this.designLink.BarControl as GalleryDropDownBarControl;
				RadialMenu radialMenu = this.designLink.LinkedObject as RadialMenu;
				TabFormLinkCollection tabFormControlLinks = this.designLink.LinkedObject as TabFormLinkCollection;
				BarLinkContainerItem containerItem = this.designLink.OwnerItem as BarLinkContainerItem;
				if((containerItem is BarLinkContainerExItem) || (containerItem is BarToolbarsListItem))
					containerItem = null;
				PopupMenu menu = this.designLink.LinkedObject as PopupMenu;
				if(bar != null) {
					link = bar.AddItem(item);
					if(bar.BarControl != null) bar.BarControl.CheckDirty();
					if(bar.DockControl != null) bar.DockControl.CheckDirty();
				}
				if(radialMenu != null) {
					link = radialMenu.AddItem(item);
					radialMenu.Window.ForceUpdateViewInfo();
					radialMenu.Window.Invalidate();
				}
				if(tabFormControlLinks != null) {
					link = tabFormControlLinks.Add(item);
				}
				if(containerItem != null) link = containerItem.AddItem(item);
				if(menu != null) link = menu.AddItem(item);
				if(linksControl != null) linksControl.ForceLayout();
				else if(galleryControl != null) {
				}
				else if(menuControl != null) {
					menuControl.LayoutChanged();
				}
				if(link != null && link.LinkViewInfo != null) {
					this.designLink.Manager.Helper.CustomizationManager.CreateDesignTimeManager();
					this.designLink.Manager.Helper.CustomizationManager.DesignTimeManager.EditLinkCaption(link, false);					
				}
			}
			protected internal static BarItem CreateItem(BarManager manager, BarItemInfo itemInfo, object arguments) {
				BarItem barItem = System.Activator.CreateInstance(itemInfo.ItemType) as BarItem;
				try {
					manager.AddToContainer(barItem);
				} catch(Exception e) {
					MessageBox.Show(e.Message);
					return null;
				}
				barItem.Caption = barItem.Name;
				barItem.Manager = manager;
				barItem.OnItemCreated(arguments);
				barItem.UpdateId();
				manager.FireManagerChanged();
				return barItem;
			}
		}
	}
}
