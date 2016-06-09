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

using DevExpress.XtraBars.Navigation;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Docking2010.Customization {
	class SearchTileBar : TileBar, ISearchControlClient, ISearchPanel {
		TileBarSearchControl searchControlCore;
		ISearchProvider providerCore;
		int visibleRowCountCore;
		AppearanceObject appearanceCore;
		public SearchTileBar()
			: base() {
			this.searchControlCore = CreateSearchControl();
			appearanceCore = new AppearanceObject();
			InitializeComponents();
		}
		protected ISearchProvider Provider { get { return providerCore; } }
		protected virtual TileBarSearchControl CreateSearchControl() { return new TileBarSearchControl(); }
		protected virtual void InitializeComponents() {
			this.AllowDrag = false;
			this.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
			this.Orientation = Orientation.Vertical;
			this.Name = "searchTileBar";
			this.ScrollMode = TileControlScrollMode.ScrollBar;
			this.ShowGroupText = true;
			this.ShowText = true;
			this.AllowSelectedItem = true;
			this.IndentBetweenGroups = 20;
			this.ItemSize = 40;
			this.Padding = new System.Windows.Forms.Padding(30, 20, 30, 30);
			this.AppearanceItem.Hovered.TextOptions.WordWrap = WordWrap.NoWrap;
			this.AppearanceItem.Normal.TextOptions.WordWrap = WordWrap.NoWrap;
			this.AppearanceItem.Pressed.TextOptions.WordWrap = WordWrap.NoWrap;
			this.AppearanceItem.Selected.TextOptions.WordWrap = WordWrap.NoWrap;
			this.searchControlCore.Client = this;
			this.searchControlCore.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.searchControlCore.Properties.AutoHeight = false;
			this.searchControlCore.Size = new Size(100, 40);
			this.searchControlCore.Properties.AllowGlyphSkinning = DefaultBoolean.True;
			this.searchControlCore.Properties.Appearance.FontSizeDelta = 3;
			this.searchControlCore.Properties.ImmediatePopup = false;
			this.searchControlCore.Properties.Padding = new System.Windows.Forms.Padding(0, 0, 9, 0);
			this.searchControlCore.Properties.CustomDrawButton += OnCustomDrawButton;
			InitializeSearchControlButtons();
			this.Controls.Add(searchControlCore);
		}
		void InitializeSearchControlButtons() {
			if(SearchControl == null || SearchControl.Properties.Buttons == null) return;
			foreach(DevExpress.XtraEditors.Repository.DefaultEditorButton button in SearchControl.Properties.Buttons) {
				if(button == null) continue;
				button.AllowGlyph = true;
				button.Width = 24;
				button.ImageLocation = XtraEditors.ImageLocation.MiddleCenter;
				button.Image = GetDefaultEditorButtonImage(button.GetType().Name);
			}
		}
		Image GetDefaultEditorButtonImage(string name) {
			return ResourceImageHelper.CreateImageFromResources(string.Format("DevExpress.XtraBars.Docking2010.Resources.SearchPanel.{0}.png", name), this.GetType().Assembly);
		}
		protected override void CheckViewInfo() {
			base.CheckViewInfo();
			if(SearchControl != null)
				SearchControl.Bounds = ViewInfoCore.SearchControlBounds;
		}
		public bool AttachProvider(ISearchProvider provider) {
			if(provider == null || providerCore == provider || !provider.CanShowSearchPanel) return false;
			if(Provider != null)
				DetachProvider();
			providerCore = provider;
			SetSettings();
			OnProviderAttached();
			return true;
		}
		void SetSettings() {
			if(Provider == null) return;
			SetSettingsCore(providerCore.SearchPanelProperties);
		}
		protected virtual void SetSettingsCore(ISearchTileBarProperties properties) {
			BeginUpdate();
			if(SearchControl is TileBarSearchControl)
				((TileBarSearchControl)SearchControl).SetSettings(properties);
			this.Text = properties.Caption;
			this.VisibleRowCount = properties.VisibleRowCount;
			EndUpdate();
		}
		protected virtual SearchTileBarItem CreateTileItem(ISearchContainer searchContainer, ISearchObject searchObject) {
			using(ICustomSearchObjectProperties properties = new CustomSearchObjectProperties(searchContainer, searchObject)) {
				CustomizeSearchItems(properties);
				if(!properties.Visible) return null;
				SearchTileBarItem item = new SearchTileBarItem(properties);
				return item;
			}
		}
		void CustomizeSearchItems(ICustomSearchObjectProperties properties) {
			Views.WindowsUI.CustomizeSearchItemsEventArgs args = new Views.WindowsUI.CustomizeSearchItemsEventArgs(properties.SourceContainer, properties.Source);
			Provider.CustomizeSearchItems(args);
			properties.Image = args.Image;
			properties.Content = args.Content;
			properties.AllowGlyphSkinning = args.AllowGlyphSkinning;
			properties.Title = args.Title;
			properties.SubTitle = args.SubTitle;
			properties.SubTitleForeColor = AppearanceGroupText.ForeColor;
			properties.Visible = args.Visible;
		}
		SearchTileBarGroup GetGroup(string name) {
			SearchTileBarGroup group = Groups[name] as SearchTileBarGroup;
			if(group == null) {
				group = GroupHelper.CreateGroup(name, Provider.SearchPanelProperties);
				Groups.Add(group);
			}
			return group;
		}
		public int VisibleRowCount {
			get { return visibleRowCountCore; }
			set {
				if(VisibleRowCount == value) return;
				visibleRowCountCore = value;
				OnPropertiesChanged();
			}
		}
		public void DetachProvider() {
			providerCore = null;
			if(Groups == null || Groups.Count == 0) return;
			List<TileGroup> groupsCore = new List<TileGroup>();
			foreach(TileGroup group in Groups)
				groupsCore.Add(group);
			Groups.Clear();
			foreach(TileGroup group in groupsCore)
				group.Dispose();
		}
		void RegisterContainerOfSearch(ISearchContainer searchContainer, string groupName) {
			if(searchContainer == null || !searchContainer.EnabledInSearch) return;
			SearchTileBarGroup group = GetGroup(groupName);
			if(group == null) return;
			AddToGroup(group, CreateTileItem(searchContainer, searchContainer));
			RegisterObjectsOfSearch(searchContainer, groupName);
		}
		void AddToGroup(SearchTileBarGroup group, SearchTileBarItem item) {
			if(item == null) return;
			group.Items.Add(item);
		}
		void RegisterObjectsOfSearch(ISearchContainer searchContainer, string groupName) {
			SearchTileBarGroup group = GetGroup(groupName);
			if(searchContainer.SearchObjectList == null || group == null) return;
			foreach(ISearchObject searchObject in searchContainer.SearchObjectList) {
				if(searchObject == null || !searchObject.EnabledInSearch) continue;
				AddToGroup(group, CreateTileItem(searchContainer, searchObject));
			}
		}
		void RegisterActiveContainerOfSearch(ISearchContainer activeContainer) {
			if(activeContainer == null) return;
			if(activeContainer.EnabledInSearch)
				RegisterObjectsOfSearch(activeContainer, GroupHelper.ActivePage);
			if(activeContainer.SearchChildList == null) return;
			foreach(ISearchContainer searchObject in activeContainer.SearchChildList)
				RegisterContainerOfSearch(searchObject, GroupHelper.ChildPages);
		}
		void OnProviderAttached() {
			if(providerCore == null) return;
			RegisterActiveContainerOfSearch(providerCore.SearchActiveContainer);
			if(providerCore.SearchOtherList == null) return;
			foreach(ISearchContainer searchObject in providerCore.SearchOtherList)
				RegisterContainerOfSearch(searchObject, GroupHelper.Other);
			SearchControl.ActionSearch();
		}
		public void SetFocus() { FocusHelper.SetFocus(SearchControl); }
		public void ResetFocus() { FocusHelper.RestoreFocus(); }
		internal TileBarSearchControl SearchControl { get { return searchControlCore; } }
		protected new SearchTileBarViewInfo ViewInfoCore { get { return base.ViewInfoCore as SearchTileBarViewInfo; } }
		protected override XtraEditors.TileControlViewInfo CreateViewInfo() { return new SearchTileBarViewInfo(this); }
		protected override TileControlPainter CreatePainter() { return new SearchTileBarPainter(); }
		protected override TileControlHandler CreateHandler() { return new SearchTileBarHandler(this); }
		protected override void OnItemClickCore(TileItem item) {
			SearchTileBarItem searchItem = item as SearchTileBarItem;
			if(searchItem != null && Provider != null)
				Provider.Activate(searchItem);
			base.OnItemClickCore(item);
		}
		protected override void Dispose(bool disposing) {
			DetachProvider();
			if(disposing) {
				this.searchControlCore.Properties.CustomDrawButton -= OnCustomDrawButton;
				Ref.Dispose(ref appearanceCore);
				Ref.Dispose(ref searchControlCore);
			}
			base.Dispose(disposing);
		}
		#region CustomDrawButton
		void OnCustomDrawButton(object sender, XtraEditors.Controls.CustomDrawButtonEventArgs e) {
			if(e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph && e.Info.IsImageExists && e.Info.AllowGlyphSkinning) {
				Rectangle r = new Rectangle(Point.Empty, e.Info.ImageSize);
				var attributes = GetColoredAttributesButton(e);
				Rectangle rect = DevExpress.Utils.Drawing.ImageLayoutHelper.GetImageBounds(e.Bounds, e.Info.ImageSize, DevExpress.Utils.Drawing.ImageLayoutMode.MiddleCenter);
				if(e.Button.Image != null) {
					e.GraphicsClear();
					e.GraphicsCache.Paint.DrawImage(e.Graphics, e.Info.ActualImage, rect, r, attributes);
					e.Handled = true;
				}
			}
		}
		int GetOpacityButton(XtraEditors.Controls.CustomDrawButtonEventArgs e) {
			if((e.State & DevExpress.Utils.Drawing.ObjectState.Pressed) == DevExpress.Utils.Drawing.ObjectState.Pressed)
				return 90;
			if(e.Button is DevExpress.XtraEditors.Repository.ClearButton) {
				if(e.State == DevExpress.Utils.Drawing.ObjectState.Hot || e.State == DevExpress.Utils.Drawing.ObjectState.Normal)
					return 130;
			}
			return 255;
		}
		Color GetForeColorButton(XtraEditors.Controls.CustomDrawButtonEventArgs e) {
			if((e.State & DevExpress.Utils.Drawing.ObjectState.Pressed) == DevExpress.Utils.Drawing.ObjectState.Pressed || e.State == DevExpress.Utils.Drawing.ObjectState.Hot)
				return e.GetForeColor();
			return Appearance.BackColor;
		}
		System.Drawing.Imaging.ImageAttributes GetColoredAttributesButton(XtraEditors.Controls.CustomDrawButtonEventArgs e) {
			return DevExpress.Utils.Drawing.ImageColorizer.GetColoredAttributes(GetForeColorButton(e), GetOpacityButton(e));
		}
		#endregion CustomDrawButton
		#region ISearchControlClient Members
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase searchInfo) {
			SearchCriteriaInfo info = searchInfo as SearchCriteriaInfo;
			if(info == null || Groups == null) return;
			((ITileControl)this).Navigator.SetSelectedItemCore(null);
			foreach(XtraEditors.TileGroup group in Groups) {
				SearchTileBarGroup searchGroup = group as SearchTileBarGroup;
				if(searchGroup == null || searchGroup.Items == null) continue;
				searchGroup.Items.FilterCriteria = info.CriteriaOperator;
			}
			this.ViewInfoCore.SetDirty();
			this.Invalidate();
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() { return new SearchTileBarCriteriaProvider(); }
		bool ISearchControlClient.IsAttachedToSearchControl { get { return true; } }
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) { }
		#endregion
		public AppearanceObject Appearance { get { return appearanceCore; } }
		static class GroupHelper {
			public static string ActivePage { get { return "ActivePage"; } }
			public static string ChildPages { get { return "ChildPages"; } }
			public static string Other { get { return "Other"; } }
			public static SearchTileBarGroup CreateGroup(string name, ISearchPanelProperties properties) {
				SearchTileBarGroup group = new SearchTileBarGroup();
				group.Name = name;
				group.Text = GetGroupText(name, properties);
				return group;
			}
			static string GetGroupText(string name, ISearchPanelProperties properties) {
				if(ActivePage == name) return properties.HeaderCategoryActive;
				if(ChildPages == name) return properties.HeaderCategoryChildren;
				if(Other == name) return properties.HeaderCategoryOther;
				return null;
			}
		}
	}
	class SearchTileBarHandler : TileBarHandler {
		public SearchTileBarHandler(ITileControl control) : base(control) { }
		protected override void OnMouseDrag(Point pt) { }
		protected override void OnDragDrop(bool cancelDrop) { }
		public override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == System.Windows.Forms.MouseButtons.Right) return;
			base.OnMouseDown(e);
		}
		public override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == System.Windows.Forms.MouseButtons.Right) return;
			Control.ViewInfo.UpdateButtonsState(e.Location, e.Button);
			if(!CanProceedMouse) return;
			Control.Capture = false;
			if(State == TileControlHandlerState.DragMode || State == TileControlHandlerState.MouseScroll) {
				base.OnMouseUp(e);
				return;
			}
			else {
				TileControlHitInfo upInfo = Control.ViewInfo.CalcHitInfo(e.Location);
				if(Control.ViewInfo.PressedInfo.InItem && upInfo.InItem && Control.ViewInfo.PressedInfo.ItemInfo == upInfo.ItemInfo) {
					ISearchTileBarItemViewInfo itemInfo = upInfo.ItemInfo as ISearchTileBarItemViewInfo;
					if(itemInfo != null && !itemInfo.AllowSelected)
						ProceedPressInItem(e);
					else
						base.OnMouseUp(e);
				}
			}
			Control.ViewInfo.PressedInfo = TileControlHitInfo.Empty;
		}
		protected override bool OnKeyDownCore(Keys keyData) {
			if(DragItem != null)
				return false;
			switch(keyData) {
				case Keys.Up:
					Control.Navigator.MoveUp();
					return true;
				case Keys.Down:
					Control.Navigator.MoveDown();
					return true;
				case Keys.Enter:
					Control.Navigator.OnKeyClick();
					return true;
			}
			return false;
		}
	}
	class SearchTileBarItemBase : TileBarItem {
		public SearchTileBarItemBase() : base() { }
		protected override TileItemViewInfo CreateViewInfo() { return new SearchTileBarItemViewInfoBase(this); }
	}
	[SearchColumn("Text"), SearchColumn("Tag"), SearchColumn("Content")]
	class SearchTileBarItem : SearchTileBarItemBase, ISearchObjectContext {
		Views.WindowsUI.IContentContainer sourceContainerCore;
		Base.BaseComponent sourceCore;
		public SearchTileBarItem(ICustomSearchObjectProperties properties)
			: base() {
			sourceContainerCore = properties.SourceContainer;
			sourceCore = properties.Source;
			Tag = properties.Tag;
			Name = properties.Title;
			Content = properties.GetContent();
			AllowGlyphSkinning = properties.AllowGlyphSkinning == DefaultBoolean.Default ? DefaultBoolean.True : properties.AllowGlyphSkinning;
			AllowHtmlText = properties.AllowHtmlText;
			CreateTileItemElements(properties);
		}
		public string Content { get; private set; }
		void CreateTileItemElements(ICustomSearchObjectProperties properties) {
			TileItemElement textElement = new TileItemElement();
			TileItemElement subElement = new TileItemElement();
			TileItemElement imageElement = new TileItemElement();
			Elements.Add(textElement);
			Elements.Add(subElement);
			Elements.Add(imageElement);
			imageElement.Image = properties.Image ?? GetSearchContainerImageFromResources();
			imageElement.ImageSize = properties.ImageSize;
			imageElement.ImageScaleMode = properties.ImageScaleMode;
			textElement.Text = properties.Title;
			textElement.AnchorElement = imageElement;
			textElement.AnchorAlignment = ConvertAlignment(properties.ImageToTextAlignment);
			textElement.AnchorIndent = properties.ImageToTextIndent;
			subElement.AnchorElement = textElement;
			subElement.AnchorIndent = 0;
			subElement.Text = string.Format("<size={0}><color={1}>{2}</color></size>", new object[] { properties.SubTitleFontSizeDelta, properties.SubTitleForeColor.ToArgb(), properties.SubTitle });
		}
		protected AnchorAlignment ConvertAlignment(TileControlImageToTextAlignment alignment) {
			switch(alignment) {
				case TileControlImageToTextAlignment.Right: return AnchorAlignment.Left;
				case TileControlImageToTextAlignment.Top: return AnchorAlignment.Bottom;
				case TileControlImageToTextAlignment.Bottom: return AnchorAlignment.Top;
				default: return AnchorAlignment.Right;
			}
		}
		Bitmap GetSearchContainerImageFromResources() {
			return DevExpress.Utils.ResourceImageHelper.CreateImageFromResourcesEx(string.Format("DevExpress.XtraBars.Docking2010.Resources.SearchPanel.Search{0}.png", sourceCore.GetType().Name), this.GetType().Assembly) as Bitmap;
		}
		Views.WindowsUI.IContentContainer ISearchObjectContext.SourceContainer { get { return sourceContainerCore; } }
		Base.BaseComponent ISearchObjectContext.Source { get { return sourceCore; } }
		string ISearchObjectContext.Title { get { return Text; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Tag = null;
				sourceCore = null;
				sourceContainerCore = null;
			}
			base.Dispose(disposing);
		}
	}
	class SearchTileBarGroup : TileBarGroup {
		SearchTileBarExpand tileBarExpandCore;
		public SearchTileBarGroup()
			: base() {
			tileBarExpandCore = new SearchTileBarExpand(this);
		}
		public SearchTileBarExpand ExpandButton { get { return tileBarExpandCore; } }
		protected override TileGroupViewInfo CreateViewInfo() { return new SearchTileBarGroupViewInfo(this); }
		protected override TileItemCollection CreateItems() { return new SearchTileBarItemCollection(this); }
		public new SearchTileBarItemCollection Items { get { return base.Items as SearchTileBarItemCollection; } }
		SearchTileBar SearchTileBar { get { return Control as SearchTileBar; } }
		public int VisibleRowCount {
			get {
				if(!AllowShowTileBarExpand) return Items == null ? 0 : Items.VisibleCount;
				if(IsExpanded) return Items.VisibleCount;
				return SearchTileBar.VisibleRowCount;
			}
		}
		public bool AllowShowTileBarExpand {
			get {
				if(Items == null) return false;
				if(SearchTileBar == null || SearchTileBar.VisibleRowCount <= 0) return false;
				return SearchTileBar.VisibleRowCount < Items.VisibleCount;
			}
		}
		protected bool IsExpanded { get { return tileBarExpandCore.Expanded; } }
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing)
				Ref.Dispose(ref tileBarExpandCore);
		}
	}
	class SearchTileBarExpand : SearchTileBarItemBase {
		SearchTileBarGroup groupCore;
		bool expandedCore;
		ImageCollection images;
		public SearchTileBarExpand(SearchTileBarGroup group)
			: base() {
			InitializeImages();
			this.groupCore = group;
			this.ImageAlignment = TileItemContentAlignment.MiddleCenter;
			this.AllowGlyphSkinning = DefaultBoolean.True;
			ChangeImage(0);
		}
		void InitializeImages() {
			images = new ImageCollection();
			images.ImageSize = new Size(27, 27);
			images.AddImage(GetImageFromResources("SearchTileBarExpandButton"));
			images.AddImage(GetImageFromResources("SearchTileBarCollapseButton"));
		}
		Image GetImageFromResources(string name) {
			return ResourceImageHelper.CreateImageFromResources(string.Format("DevExpress.XtraBars.Docking2010.Resources.SearchPanel.{0}.png", name), this.GetType().Assembly);
		}
		public bool Expanded {
			get { return expandedCore; }
			set {
				if(Expanded == value) return;
				ChangeImage(value ? 1 : 0);
				expandedCore = value;
				OnPropertiesChanged();
			}
		}
		public override bool Enabled {
			get { return groupCore != null && groupCore.VisibleRowCount > 0; }
			set { }
		}
		public override void OnItemClick() {
			if(!Enabled) return;
			Expanded = !expandedCore;
		}
		void ChangeImage(int index) { Image = images.Images[index]; }
		protected override TileGroup GetGroup() { return groupCore; }
		protected override TileItemViewInfo CreateViewInfo() { return new SearchTileBarExpandViewInfo(this); }
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				groupCore = null;
				Ref.Dispose(ref images);
			}
		}
	}
	class TileBarSearchControl : SearchControl {
		int lockValidating = 0;
		public void SetSettings(ISearchControlProperties properties) {
			if(properties == null) return;
			Properties.BeginUpdate();
			this.Properties.ShowDefaultButtonsMode = ConvertToShowDefaultButtonsMode(properties.ClearButtonShowMode);
			this.Properties.ShowSearchButton = properties.ShowSearchButton;
			this.Properties.ShowClearButton = properties.ClearButtonShowMode != ClearButtonShowMode.Hidden;
			this.Properties.ShowNullValuePromptWhenFocused = properties.ShowNullValuePromptWhenFocused;
			this.Properties.FindDelay = properties.FindDelay;
			this.Properties.AllowAutoApply = properties.FindDelay > 0;
			this.Properties.NullValuePrompt = properties.NullValuePrompt;
			this.Properties.NullValuePromptShowForEmptyValue = properties.NullValuePromptShowForEmptyValue;
			Properties.EndUpdate();
		}
		public new void ActionSearch() { base.ActionSearch(); }
		DevExpress.XtraEditors.Repository.ShowDefaultButtonsMode ConvertToShowDefaultButtonsMode(ClearButtonShowMode mode) {
			switch(mode) {
				case ClearButtonShowMode.AlwaysVisible:
				case ClearButtonShowMode.Hidden:
					return XtraEditors.Repository.ShowDefaultButtonsMode.Always;
				case ClearButtonShowMode.Auto: return XtraEditors.Repository.ShowDefaultButtonsMode.AutoShowClear;
			}
			return XtraEditors.Repository.ShowDefaultButtonsMode.AutoChangeSearchToClear;
		}
		void LockValidating() { lockValidating++; }
		void UnlockValidating(System.ComponentModel.CancelEventArgs e) {
			if(--lockValidating == 0)
				base.OnValidatingCore(e);
		}
		protected override void OnValidating(System.ComponentModel.CancelEventArgs e) {
			LockValidating();
			base.OnValidating(e);
			UnlockValidating(e);
		}
		protected override void OnValidatingCore(System.ComponentModel.CancelEventArgs e) {
			if(lockValidating > 0) return;
			base.OnValidatingCore(e);
		}
	}
	class FocusHelper {
		static System.IntPtr saveFocusedObject;
		static System.IntPtr focusedObject;
		static Form ownerFormCore;
		static int isLock = 0;
		static void SaveFocus() { saveFocusedObject = BarNativeMethods.GetFocus(); }
		public static void SetFocus(Control control) {
			if(control == null || !control.IsHandleCreated) return;
			SaveFocus();
			focusedObject = control.Handle;
			BarNativeMethods.SetFocus(new System.Runtime.InteropServices.HandleRef(Control.FromHandle(focusedObject), focusedObject));
		}
		public static void LockFocus(Form ownerForm) {
			if(IsLock) return;
			Control control = Control.FromHandle(focusedObject);
			if(control == null) return;
			ownerFormCore = ownerForm ?? control.FindForm();
			if(ownerFormCore == null) return;
			control.Validating += new System.ComponentModel.CancelEventHandler(OnValidating);
			ownerFormCore.FormClosing += new FormClosingEventHandler(OnFormClosing);
			isLock++;
		}
		static void OnFormClosing(object sender, FormClosingEventArgs e) { e.Cancel = false; }
		static void OnValidating(object sender, System.ComponentModel.CancelEventArgs e) { e.Cancel = true; }
		public static void UnlockFocus() {
			if(!IsLock) return;
			Control control = Control.FromHandle(focusedObject);
			if(control == null) return;
			control.Validating -= new System.ComponentModel.CancelEventHandler(OnValidating);
			ownerFormCore.FormClosing -= new FormClosingEventHandler(OnFormClosing);
			isLock--;
		}
		public static void RestoreFocus() {
			if(saveFocusedObject == System.IntPtr.Zero || IsLock) return;
			BarNativeMethods.SetFocus(new System.Runtime.InteropServices.HandleRef(Control.FromHandle(saveFocusedObject), saveFocusedObject));
			focusedObject = saveFocusedObject = System.IntPtr.Zero;
			ownerFormCore = null;
		}
		public static bool IsLock { get { return isLock > 0; } }
	}
}
