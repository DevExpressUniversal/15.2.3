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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Design;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Commands {
	public interface IBarItemContainer {
		List<BarItem> GetBarItems();
	}
	#region ICommandBarItem interface
	public interface ICommandBarItem {
		void UpdateCaption();
		void UpdateImages();
		void UpdateVisibility();
		void UpdateChecked();
		void UpdateGroupIndex();
		void InvokeCommand();
		bool IsEqual(ICommandBarItem item);
		void SetupStatusBarLink(BarItemLink link);
	}
	#endregion
	#region IBeginGroupSupport interface
	public interface IBeginGroupSupport {
		bool BeginGroup { get; set; }
	}
	#endregion
	#region IControlCommandBarItem<TControl, TCommandId>
	public interface IControlCommandBarItem<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		TControl Control { get; set; }
	}
	#endregion
	#region IBarSubItem
	public interface IBarSubItem {
		void AddBarItem(BarItem barItem);
		List<BarItem> GetSubItems();
	}
	#endregion
	public interface IBarButtonGroupMember {
		object ButtonGroupTag { get; }
	}
	public interface IBarDefaultLinkSettings {
		bool ActAsButtonGroup { get; }
	}
	public abstract class BarCreationContextBase {
		public abstract bool IsRibbon { get; }
	}
	public class BarCreationContext : BarCreationContextBase {
		public override bool IsRibbon { get { return false; } }
	}
	public class RibbonBarCreationContext : BarCreationContextBase {
		public override bool IsRibbon { get { return true; } }
	}
	#region CommandBasedBarComponentBase (abstract class)
	[ToolboxItem(false), DesignerCategory("Component")]
	public abstract class CommandBasedBarComponentBase : Component, ISupportInitialize {
		#region Fields
		bool initialization = false;
		CommandBasedBarItemBuilder barItemBuilder;
		bool leaveBarItems;
		BarCreationContextBase creationContext;
		#endregion
		protected CommandBasedBarComponentBase() {
			this.creationContext = CreateBarCreationContext();
		}
		protected virtual BarCreationContextBase CreationContext { get { return creationContext; } }
		protected CommandBasedBarComponentBase(IContainer container) {
			if (container != null)
				container.Add(this);
		}
		#region Properties
		protected abstract Type SupportedBarItemType { get; }
		[Browsable(false)]
		protected internal bool Initialization { get { return initialization; } }
		public CommandBasedBarItemBuilder BarItemBuilder {
			get {
				if (barItemBuilder == null) {
					barItemBuilder = CreateBarItemBuilder();
				}
				return barItemBuilder;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public bool LeaveBarItems { get { return leaveBarItems; } set { leaveBarItems = value; } }
		#endregion
		public delegate void SetRelatedVisualControlDelegate();
		protected internal virtual void SetRelatedVisualControlCore(SetRelatedVisualControlDelegate setter) {
			if (Initialization) {
				setter();
				return;
			}
			UnsubscribeRelatedVisualControlEvents();
			DeleteVisualItems();
			setter();
			CreateNewVisualItems();
			SubscribeRelatedVisualControlEvents();
		}
		protected internal virtual void CreateNewVisualItems() {
			if (!CanCreateVisualItems())
				return;
			BeforeCreateNewVisualItems();
			try {
				Component itemContainer = GetBarItemContainer();
				UpdateBarItemContainer();
				List<BarItem> items = CreateNewVisualItemsCore();
				FillBarItemContainer(itemContainer, items);
				UpdateVisualItemsCore(items);
			}
			finally {
				AfterCreateNewVisualItems();
			}
		}
		protected internal virtual List<BarItem> CreateNewVisualItemsCore() {
			List<BarItem> items = new List<BarItem>();
			PopulateNewItems(items, CreationContext);
			AddNewItemsToContainer(items);
			return items;
		}
		protected abstract CommandBasedBarItemBuilder CreateBarItemBuilder();
		protected internal abstract BarItems GetSourceBarItems();
		protected abstract bool CanCreateVisualItems();
		protected internal abstract Component GetBarItemContainer();
		protected internal abstract void UpdateBarItemContainer();
		protected abstract void FillBarItemContainer(Component parent, List<BarItem> items);
		protected virtual void BeforeCreateNewVisualItems() {
		}
		protected virtual void AfterCreateNewVisualItems() {
		}
		protected internal virtual List<BarItem> GetSupportedBarItems() {
			List<BarItem> result = new List<BarItem>();
			BarItems sourceItems = GetSourceBarItems();
			if (sourceItems == null)
				return result;
			foreach (BarItem barItem in sourceItems)
				if (IsBarItemSupported(barItem))
					result.Add(barItem);
			return result;
		}
		protected virtual bool IsBarItemSupported(BarItem barItem) {
			return SupportedBarItemType.IsAssignableFrom(barItem.GetType());
		}
		protected internal virtual void DeleteVisualItems() {
			if (!LeaveBarItems)
				DeleteVisualItemsCore();
		}
		protected internal virtual void DeleteVisualItemsCore() {
			List<BarItem> items = GetSupportedBarItems();
			if (items.Count == 0)
				return;
			foreach (BarItem btn in items) {
				if (DesignMode && Container != null)
					Container.Remove(btn);
				btn.Dispose();
			}
		}
		protected internal virtual bool AreAnyKnownItemsExist() {
			bool btnExist = GetSupportedBarItems().Count != 0;
			return btnExist;
		}
		protected internal virtual void UpdateVisualItems() {
			if (LeaveBarItems)
				return;
			List<BarItem> items = GetSupportedBarItems();
			UpdateVisualItemsCore(items);
		}
		protected internal virtual void UnsubscribeItemProviderControlEvents() {
		}
		protected internal virtual void SubscribeItemProviderControlEvents() {
		}
		protected internal virtual void SubscribeRelatedVisualControlEvents() {
		}
		protected internal virtual void UnsubscribeRelatedVisualControlEvents() {
		}
		protected virtual void PopulateNewItems(List<BarItem> items, BarCreationContextBase creationContex) {
			InitializeBarItemBuilder();
			BarItemBuilder.PopulateItems(items, creationContex);
		}
		protected virtual void InitializeBarItemBuilder() {
		}
		protected void AddNewItemsToContainer(List<BarItem> items) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				if (DesignMode && Container != null) {
					Container.Add(items[i]);
				}
			}
		}
		protected internal virtual void UpdateVisualItemsCore(List<BarItem> items) {
			foreach (BarItem btn in items) {
				UpdateBarItem(btn);
				SubscribeBarItemEvents(btn);
			}
		}
		protected internal virtual void UpdateBarItem(BarItem item) {
			ICommandBarItem barItem = item as ICommandBarItem;
			if (barItem == null)
				return;
			item.BeginUpdate();
			try {
				barItem.UpdateCaption();
				barItem.UpdateImages();
				barItem.UpdateVisibility();
				barItem.UpdateGroupIndex();
				barItem.UpdateChecked();
			}
			finally {
				item.EndUpdate();
			}
		}
		protected virtual void SubscribeBarItemEvents(BarItem btn) {
			BarCheckItem checkItem = btn as BarCheckItem;
			if (checkItem != null)
				checkItem.CheckedChanged += new ItemClickEventHandler(HandleCheckBarItemCheckedChanged);
			BarButtonItem btnItem = btn as BarButtonItem;
			if (btnItem != null) {
				btnItem.ItemClick += new ItemClickEventHandler(HandleButtonBarItemClick);
			}
			BarEditItem editItem = btn as BarEditItem;
			if (editItem != null) {
				editItem.EditValueChanged += new EventHandler(HandleEditValueChanged);
			}
		}
		protected virtual void HandleEditValueChanged(object sender, EventArgs e) {
			InvokeItemCommand(sender as ICommandBarItem);
		}
		protected virtual void HandleButtonBarItemClick(object sender, ItemClickEventArgs e) {
			InvokeItemCommand(e.Item as ICommandBarItem);
		}
		protected virtual void InvokeItemCommand(ICommandBarItem item) {
			if (item != null)
				item.InvokeCommand();
		}
		protected virtual void HandleCheckBarItemCheckedChanged(object sender, ItemClickEventArgs e) {
			ICommandBarItem item = e.Item as ICommandBarItem;
			if (item != null)
				item.InvokeCommand();
		}
		#region ISupportInitialize Members
		public void BeginInit() {
			initialization = true;
			BeginInitCore();
		}
		protected internal virtual void BeginInitCore() {
		}
		public void EndInit() {
			EndInitCore();
			initialization = false;
		}
		protected internal virtual void EndInitCore() {
			if (AreAnyKnownItemsExist()) {
				UpdateBarItemContainer();
				UpdateVisualItems();
			}
			else
				CreateNewVisualItems();
			SubscribeItemProviderControlEvents();
			SubscribeRelatedVisualControlEvents();
		}
		#endregion
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (!LeaveBarItems)
						DeleteVisualItems();
					UnsubscribeItemProviderControlEvents();
					DetachItemProviderControl();
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected virtual void DetachItemProviderControl() {
		}
		#endregion
		protected abstract BarCreationContextBase CreateBarCreationContext();
	}
	#endregion
	#region CommandBarComponentBase (abstract class)
	public abstract class CommandBarComponentBase : CommandBasedBarComponentBase {
		#region Fields
		BarManager barManager;
		#endregion
		protected CommandBarComponentBase()
			: base() {
		}
		protected CommandBarComponentBase(IContainer container)
			: base(container) {
		}
		#region Properties
		protected abstract Type SupportedBarType { get; }
		[DefaultValue(null)]
		public BarManager BarManager {
			get { return barManager; }
			set {
				if (Object.ReferenceEquals(barManager, value))
					return;
				SetBarManager(value);
			}
		}
		#endregion
		protected internal abstract CommandBasedBar CreateBarInstance();
		protected override BarCreationContextBase CreateBarCreationContext() {
			return new BarCreationContext();
		}
		protected internal virtual void SetBarManager(BarManager value) {
			SetRelatedVisualControlCore(delegate() { this.barManager = value; });
		}
		protected override bool CanCreateVisualItems() {
			return BarManager != null;
		}
		protected override void BeforeCreateNewVisualItems() {
			barManager.BeginUpdate();
		}
		protected override void AfterCreateNewVisualItems() {
			barManager.EndUpdate();
		}
		protected internal override Component GetBarItemContainer() {
			CommandBasedBar bar = FindCommandBar();
			if (bar == null) {
				bar = CreateBarInstance();
				bar.UpdateDockStyle();
			}
			barManager.Bars.Add(bar);
			return bar;
		}
		protected internal override void UpdateBarItemContainer() {
			CommandBasedBar bar = FindCommandBar();
			UpdateBar(bar);
		}
		protected internal virtual void UpdateBar(CommandBasedBar bar) {
			if (bar == null)
				return;
			bar.UpdateName();
			bar.UpdateText();
		}
		protected override void FillBarItemContainer(Component parent, List<BarItem> items) {
			Bar bar = (Bar)parent;
			foreach (BarItem btn in items) {
				BarManager.Items.Add(btn);
				bar.AddItem(btn);
			}
		}
		protected internal override void DeleteVisualItems() {
			if (BarManager == null)
				return;
			DeleteBar();
			base.DeleteVisualItems();
		}
		protected internal override void SubscribeRelatedVisualControlEvents() {
			if (barManager != null)
				barManager.Disposed += new EventHandler(barManager_Disposed);
		}
		protected internal override void UnsubscribeRelatedVisualControlEvents() {
			if (barManager != null)
				barManager.Disposed -= new EventHandler(barManager_Disposed);
		}
		protected internal virtual void barManager_Disposed(object sender, EventArgs e) {
			BarManager = null;
		}
		protected internal override BarItems GetSourceBarItems() {
			return BarManager != null ? BarManager.Items : null;
		}
		protected internal virtual void DeleteBar() {
			Bar bar = FindCommandBar();
			if (bar != null) {
				bar.Dispose();
			}
		}
		protected internal virtual CommandBasedBar FindCommandBar() {
			if (BarManager != null) {
				foreach (Bar bar in BarManager.Bars)
					if (bar.GetType() == SupportedBarType)
						return (CommandBasedBar)bar;
			}
			return null;
		}
	}
	#endregion
	#region CommandBasedBar (abstract class)
	public abstract class CommandBasedBar : Bar {
		protected CommandBasedBar()
			: base() {
		}
		protected CommandBasedBar(BarManager manager)
			: base(manager) {
		}
		protected CommandBasedBar(BarManager manager, string name)
			: base(manager, name) {
		}
		[Browsable(false)]
		public abstract string DefaultText { get; }
		protected internal override bool ShouldSerializeBarName() {
			return BarName != DefaultText;
		}
		protected internal override void ResetBarName() {
			BarName = DefaultText;
		}
		protected internal override bool ShouldSerializeText() {
			return Text != DefaultText;
		}
		protected internal override void ResetText() {
			Text = DefaultText;
		}
		protected internal virtual void UpdateDockStyle() {
			DockStyle = BarDockStyle.Top;
		}
		protected internal virtual void UpdateName() {
			if (String.IsNullOrEmpty(BarName))
				BarName = DefaultText;
		}
		protected internal virtual void UpdateText() {
			if (String.IsNullOrEmpty(Text))
				Text = DefaultText;
		}
		protected internal override void UpdateBarNameAndText() {
		}
	}
	#endregion
	#region ControlCommandBasedBar (abstract class)
	public abstract class ControlCommandBasedBar<TControl, TCommandId> : CommandBasedBar
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		TControl control;
		public virtual TControl Control {
			get { return control; }
			set { control = value; }
		}
		protected ControlCommandBasedBar() {
		}
		protected ControlCommandBasedBar(BarManager manager)
			: base(manager) {
		}
		protected ControlCommandBasedBar(BarManager manager, string name)
			: base(manager, name) {
		}
		internal void Update() {
			UpdateName();
			UpdateText();
		}
	}
	#endregion
	#region CommandBasedBarItemBuilder (abstract class)
	public abstract class CommandBasedBarItemBuilder {
		public abstract void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex);
	}
	#endregion
	static class CommandBasedBarItemGlyphHelper {
		public static Image GetActualGlyph(Image glyph, object images, int imageIndex) {
			if (glyph != null)
				return glyph;
			if (images != null && ImageCollection.IsImageListImageExists(images, imageIndex)) {
				glyph = ImageCollection.GetImageListImage(images, imageIndex);
				if (glyph != null)
					glyph.Tag = BarCollectorBase.ImageFromListTag;
			}
			return glyph;
		}
	}
	#region CommandBasedBarCheckItem (abstract class)
	public abstract class CommandBasedBarCheckItem : BarCheckItem, ICommandBarItem {
		bool defaultSuperTip;
		protected CommandBasedBarCheckItem() {
			Initialize();
		}
		protected bool DefaultSuperTip { get { return defaultSuperTip; } }
		protected CommandBasedBarCheckItem(BarManager manager)
			: base(manager) {
		}
		protected CommandBasedBarCheckItem(string caption)
			: base() {
			Caption = caption;
			Initialize();
		}
		protected CommandBasedBarCheckItem(BarManager manager, string caption)
			: base(manager) {
			Caption = caption;
			Initialize();
		}
		#region Properties
		public override string Caption {
			get { return base.Caption; }
			set {
				base.Caption = value;
				if (defaultSuperTip && !SuperTipHasDefaultContent())
					UpdateSuperTipAndShortCut();
			}
		}
		public override SuperToolTip SuperTip {
			get {
				return base.SuperTip;
			}
			set {
				base.SuperTip = value;
				defaultSuperTip = false;
			}
		}
		protected internal virtual BarShortcut SuperTipShortcut {
			get {
				if (!ShouldAssignItemShortcut && CommandBasedBarButtonItem.BarShortcutAuto == ItemShortcut)
					return new BarShortcut(GetDefaultShortcutKeys());
				else
					return ItemShortcut;
			}
		}
		public override string ShortcutKeyDisplayString { get { return GetShortcutKeyDisplayString(SuperTipShortcut); } set { base.ShortcutKeyDisplayString = value; } }
		protected internal virtual bool ShouldAssignItemShortcut { get { return true; } }
		public override BarShortcut ItemShortcut {
			get { return base.ItemShortcut; }
			set {
				base.ItemShortcut = value;
				if (defaultSuperTip && !SuperTipHasDefaultContent())
					UpdateSuperTipAndShortCut();
			}
		}
		protected internal override void ResetItemShortcut() {
			ItemShortcut = CommandBasedBarButtonItem.BarShortcutAuto;
		}
		protected internal override bool ShouldSerializeItemShortcut() {
			return !CommandBasedBarButtonItem.BarShortcutAuto.Equals(ItemShortcut) && !new BarShortcut(GetDefaultShortcutKeys()).Equals(ItemShortcut);
		}
		protected internal override bool ShouldSerializePaintStyle() {
			return PaintStyle != BarItemPaintStyle.CaptionGlyph;
		}
		protected internal override void ResetPaintStyle() {
			PaintStyle = BarItemPaintStyle.CaptionGlyph;
		}
		protected internal override bool ShouldSerializeRibbonStyle() {
			return RibbonStyle != DefaultRibbonStyle;
		}
		protected internal override void ResetRibbonStyle() {
			RibbonStyle = DefaultRibbonStyle;
		}
		protected internal bool CanUpdateVisibility { get { return Manager != null && !Manager.IsDesignMode && !Manager.IsLoading; } }
		protected internal virtual RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.All; } }
		protected internal override bool ShouldSerializeGlyph() {
			if (GlyphComparer.IsDefaultGlyph(Glyph))
				return false;
			return base.ShouldSerializeGlyph();
		}
		protected internal override void ResetGlyph() {
			Glyph = LoadDefaultImage();
		}
		protected internal override bool ShouldSerializeLargeGlyph() {
			if (GlyphComparer.IsDefaultGlyph(LargeGlyph))
				return false;
			return base.ShouldSerializeLargeGlyph();
		}
		protected internal override void ResetLargeGlyph() {
			LargeGlyph = LoadDefaultLargeImage();
		}
		#endregion
		protected internal abstract Command CreateCommand();
		protected internal virtual void Initialize() {
			ItemShortcut = CommandBasedBarButtonItem.BarShortcutAuto;
			ResetRibbonStyle();
			ResetPaintStyle();
			UpdateItemGlyphs();
		}
		protected virtual string GetDefaultCaption() {
			Command command = CreateCommand();
			return command != null ? command.MenuCaption : String.Empty;
		}
		protected virtual string GetDefaultSuperTipTitle() {
			return Caption;
		}
		protected virtual string GetDefaultSuperTipDescription() {
			Command command = CreateCommand();
			return command != null ? command.Description : String.Empty;
		}
		protected virtual Keys GetDefaultShortcutKeys() {
			return Keys.None;
		}
		protected internal override bool ShouldSerializeCaption() {
			return GetDefaultCaption() != Caption;
		}
		protected internal override void ResetCaption() {
			Caption = GetDefaultCaption();
		}
		protected override bool ShouldSerializeSuperTip() {
			return !SuperTipHasDefaultContent();
		}
		public override void ResetSuperTip() {
			UpdateSuperTipAndShortCut();
		}
		protected internal virtual void UpdateItemCaption() {
			if (String.IsNullOrEmpty(Caption))
				Caption = GetDefaultCaption();
		}
		protected internal virtual void UpdateItemGroupIndex() {
			GroupIndex = 1;
		}
		protected internal virtual void UpdateItemChecked() {
			if (!CanUpdateVisibility)
				return;
			BeginUpdate();
			try {
				Command command = CreateCommand();
				if (command != null) {
					ICommandUIState state = CreateCommandUIState(command);
					command.UpdateUIState(state);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void UpdateItemVisibility() {
			if (!CanUpdateVisibility)
				return;
			BeginUpdate();
			try {
				Command command = CreateCommand();
				if (command != null) {
					ICommandUIState state = CreateCommandUIState(command);
					command.UpdateUIState(state);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual ICommandUIState CreateCommandUIState(Command command) {
			return new BarCheckItemUIState(this);
		}
		protected internal virtual void UpdateItemGlyphs() {
			Glyph = CommandBasedBarItemGlyphHelper.GetActualGlyph(Glyph, Images, ImageIndex);
			if (Glyph == null)
				Glyph = LoadDefaultImage();
			LargeGlyph = CommandBasedBarItemGlyphHelper.GetActualGlyph(LargeGlyph, LargeImages, LargeImageIndex);
			if (LargeGlyph == null)
				LargeGlyph = LoadDefaultLargeImage();
		}
		protected internal virtual void UpdateItemSuperTip() {
			if (SuperTip == null)
				UpdateSuperTipAndShortCut();
		}
		protected internal virtual void InvokeCommand() {
			if (IsLockUpdate)
				return;
			Command command = CreateCommand();
			if (command != null)
				command.Execute();
		}
		protected virtual Image LoadDefaultImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.Image : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected virtual Image LoadDefaultLargeImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.LargeImage : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected virtual bool SuperTipHasDefaultContent() {
			return SuperToolTipHelper.SuperTipHasDefaultContent(SuperTip, GetDefaultSuperTipTitle(), GetDefaultSuperTipDescription(), SuperTipShortcut);
		}
		protected internal virtual void UpdateSuperTipAndShortCut() {
			if (ItemShortcut == CommandBasedBarButtonItem.BarShortcutAuto && ShouldAssignItemShortcut)
				RecreateItemShortcut();
			RecreateItemSuperToolTip();
		}
		protected virtual void RecreateItemShortcut() {
			Keys shortCutKeys = GetDefaultShortcutKeys();
			if (shortCutKeys != Keys.None)
				ItemShortcut = new BarShortcut(shortCutKeys);
			else
				ItemShortcut = BarShortcut.Empty;
		}
		protected virtual void RecreateItemSuperToolTip() {
			SuperTip = SuperToolTipHelper.CreateSuperToolTip(GetDefaultSuperTipTitle(), GetDefaultSuperTipDescription(), SuperTipShortcut);
			defaultSuperTip = true;
		}
		protected virtual bool IsEqual(ICommandBarItem item) {
			if (item == null)
				return false;
			if (item == this)
				return true;
			return item.GetType() == this.GetType();
		}
		protected virtual void SetupStatusBarLink(BarItemLink link) {
		}
		#region ICommandBarItem Members
		void ICommandBarItem.UpdateCaption() {
			UpdateItemCaption();
			UpdateItemSuperTip();
		}
		void ICommandBarItem.UpdateVisibility() {
			UpdateItemVisibility();
		}
		void ICommandBarItem.UpdateImages() {
			UpdateItemGlyphs();
		}
		void ICommandBarItem.UpdateChecked() {
			UpdateItemChecked();
		}
		void ICommandBarItem.UpdateGroupIndex() {
			UpdateItemGroupIndex();
		}
		void ICommandBarItem.InvokeCommand() {
			InvokeCommand();
		}
		bool ICommandBarItem.IsEqual(ICommandBarItem item) {
			return this.IsEqual(item);
		}
		void ICommandBarItem.SetupStatusBarLink(BarItemLink link) {
			this.SetupStatusBarLink(link);
		}
		#endregion
	}
	#endregion
	#region ControlCommandBarCheckItem<TControl, TCommandId> (abstract class)
	public abstract class ControlCommandBarCheckItem<TControl, TCommandId> : CommandBasedBarCheckItem, IControlCommandBarItem<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		TControl control;
		#endregion
		protected ControlCommandBarCheckItem()
			: base() {
		}
		protected ControlCommandBarCheckItem(string caption)
			: base(caption) {
		}
		protected ControlCommandBarCheckItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		protected ControlCommandBarCheckItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		#region Control
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TControl Control {
			get { return control; }
			set {
				if (Object.ReferenceEquals(control, value))
					return;
				if (control != null)
					UnsubscribeControlEvents();
				if (DefaultSuperTip)
					SuperTip = null;
				control = value;
				if (control != null)
					SubscribeControlEvents();
			}
		}
		#endregion
		#region PaintStyle
		protected internal override bool ShouldSerializePaintStyle() {
			return PaintStyle != BarItemPaintStyle.Standard;
		}
		protected internal override void ResetPaintStyle() {
			PaintStyle = BarItemPaintStyle.Standard;
		}
		#endregion
		protected internal override bool ShouldAssignItemShortcut { get { return false; } }
		protected internal abstract TCommandId CommandId { get; }
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (control != null) {
						UnsubscribeControlEvents();
						control = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal virtual void SubscribeControlEvents() {
			Control.BeforeDispose += OnBeforeDisposeControl;
			Control.UpdateUI += OnControlUpdateUI;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			Control.BeforeDispose -= OnBeforeDisposeControl;
			Control.UpdateUI -= OnControlUpdateUI;
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.Control = null;
		}
		protected internal virtual void OnControlUpdateUI(object sender, EventArgs e) {
			this.UpdateItemVisibility();
		}
		protected internal override void UpdateItemGroupIndex() {
			this.GroupIndex = 0;
		}
		protected internal override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(CommandId);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected override Keys GetDefaultShortcutKeys() {
			if (Control == null)
				return Keys.None;
			CommandBasedKeyboardHandler<TCommandId> keyboardHandler = Control.KeyboardHandler;
			if (keyboardHandler == null)
				return Keys.None;
			return keyboardHandler.GetKeys(CommandId);
		}
		protected internal virtual bool HandleException(Exception e) {
			if (e != null && Control != null)
				return Control.HandleException(e);
			else
				return false;
		}
		protected internal override void InvokeCommand() {
			try {
				Control.CommitImeContent();
				base.InvokeCommand();
				UpdateItemVisibility();
				if (!IsLockUpdate)
					Control.Focus();
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected internal override void UpdateItemVisibility() {
			base.UpdateItemVisibility();
			if (DesignMode && !Enabled && !ShouldDisableBarItem())
				Enabled = true;
			if (Enabled && ShouldDisableBarItem())
				Enabled = false;
		}
		bool ShouldDisableBarItem() {
			return Control == null;
		}
		protected override bool IsEqual(ICommandBarItem item) {
			if (!base.IsEqual(item))
				return false;
			ControlCommandBarCheckItem<TControl, TCommandId> otherItem = (ControlCommandBarCheckItem<TControl, TCommandId>)item;
			return Object.Equals(this.CommandId, otherItem.CommandId);
		}
	}
	#endregion
	#region CommandBasedBarButtonItem (abstract class)
	public abstract class CommandBasedBarButtonItem : BarButtonItem, ICommandBarItem {
		internal static BarShortcut BarShortcutAuto = new BarShortcut((Keys)(-1));
		bool defaultSuperTip;
		protected bool DefaultSuperTip { get { return defaultSuperTip; } }
		protected CommandBasedBarButtonItem() {
			Initialize();
		}
		protected CommandBasedBarButtonItem(string caption)
			: base() {
			Caption = caption;
			Initialize();
		}
		protected CommandBasedBarButtonItem(BarManager manager)
			: base(manager, string.Empty) {
			Initialize();
		}
		protected CommandBasedBarButtonItem(BarManager manager, string caption)
			: base(manager, caption) {
			Initialize();
		}
		#region Properties
		public override string Caption {
			get { return base.Caption; }
			set {
				base.Caption = value;
				if (defaultSuperTip && !SuperTipHasDefaultContent())
					UpdateSuperTipAndShortCut();
			}
		}
		protected internal virtual BarShortcut SuperTipShortcut {
			get {
				if (!ShouldAssignItemShortcut && CommandBasedBarButtonItem.BarShortcutAuto == ItemShortcut)
					return new BarShortcut(GetDefaultShortcutKeys());
				else
					return ItemShortcut;
			}
		}
		public override string ShortcutKeyDisplayString { get { return GetShortcutKeyDisplayString(SuperTipShortcut); } set { base.ShortcutKeyDisplayString = value; } }
		protected internal virtual bool ShouldAssignItemShortcut { get { return true; } }
		public override BarShortcut ItemShortcut {
			get { return base.ItemShortcut; }
			set {
				base.ItemShortcut = value;
				if (defaultSuperTip && !SuperTipHasDefaultContent())
					UpdateSuperTipAndShortCut();
			}
		}
		protected internal override void ResetItemShortcut() {
			ItemShortcut = CommandBasedBarButtonItem.BarShortcutAuto;
		}
		protected internal override bool ShouldSerializeItemShortcut() {
			return !CommandBasedBarButtonItem.BarShortcutAuto.Equals(ItemShortcut) && !new BarShortcut(GetDefaultShortcutKeys()).Equals(ItemShortcut);
		}
		protected internal override bool ShouldSerializePaintStyle() {
			return PaintStyle != BarItemPaintStyle.CaptionGlyph;
		}
		protected internal override void ResetPaintStyle() {
			PaintStyle = BarItemPaintStyle.CaptionGlyph;
		}
		protected internal override bool ShouldSerializeRibbonStyle() {
			return RibbonStyle != DefaultRibbonStyle;
		}
		protected internal override void ResetRibbonStyle() {
			RibbonStyle = DefaultRibbonStyle;
		}
		protected internal bool CanUpdateVisibility { get { return Manager != null && !Manager.IsDesignMode && !Manager.IsLoading; } }
		protected internal virtual RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.All; } }
		protected internal override bool ShouldSerializeGlyph() {
			if (GlyphComparer.IsDefaultGlyph(Glyph))
				return false;
			return base.ShouldSerializeGlyph();
		}
		protected internal override void ResetGlyph() {
			Glyph = LoadDefaultImage();
		}
		protected internal override bool ShouldSerializeLargeGlyph() {
			if (GlyphComparer.IsDefaultGlyph(LargeGlyph))
				return false;
			return base.ShouldSerializeLargeGlyph();
		}
		protected internal override void ResetLargeGlyph() {
			LargeGlyph = LoadDefaultLargeImage();
		}
		#endregion
		protected internal abstract Command CreateCommand();
		protected internal virtual void Initialize() {
			ItemShortcut = CommandBasedBarButtonItem.BarShortcutAuto;
			ResetRibbonStyle();
			ResetPaintStyle();
			UpdateItemGlyphs();
		}
		protected virtual string GetDefaultCaption() {
			Command command = CreateCommand();
			return command != null ? command.MenuCaption : String.Empty;
		}
		protected virtual string GetDefaultSuperTipTitle() {
			return Caption;
		}
		protected virtual string GetDefaultSuperTipDescription() {
			Command command = CreateCommand();
			return command != null ? command.Description : String.Empty;
		}
		protected virtual Keys GetDefaultShortcutKeys() {
			return Keys.None;
		}
		protected internal override bool ShouldSerializeCaption() {
			return GetDefaultCaption() != Caption;
		}
		protected internal override void ResetCaption() {
			Caption = GetDefaultCaption();
		}
		protected override bool ShouldSerializeSuperTip() {
			return !SuperTipHasDefaultContent();
		}
		public override void ResetSuperTip() {
			UpdateSuperTipAndShortCut();
		}
		protected internal virtual void UpdateItemCaption() {
			if (String.IsNullOrEmpty(Caption))
				Caption = GetDefaultCaption();
		}
		protected internal virtual void UpdateItemGroupIndex() {
			GroupIndex = 1;
		}
		protected internal virtual void UpdateItemChecked() {
			UpdateUIStateCore();
		}
		protected internal virtual void UpdateItemVisibility() {
			UpdateUIStateCore();
		}
		protected virtual void UpdateCaption() {
			UpdateItemCaption();
			UpdateItemSuperTip();
		}
		protected internal virtual void UpdateUIStateCore() {
			if (!CanUpdateVisibility)
				return;
			BeginUpdate();
			try {
				Command command = CreateCommand();
				if (command != null) {
					ICommandUIState state = CreateButtonItemUIState();
					command.UpdateUIState(state);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual ICommandUIState CreateButtonItemUIState() {
			return new BarButtonItemUIState(this);
		}
		protected internal virtual void UpdateItemGlyphs() {
			Glyph = CommandBasedBarItemGlyphHelper.GetActualGlyph(Glyph, Images, ImageIndex);
			if (Glyph == null)
				Glyph = LoadDefaultImage();
			LargeGlyph = CommandBasedBarItemGlyphHelper.GetActualGlyph(LargeGlyph, LargeImages, LargeImageIndex);
			if (LargeGlyph == null)
				LargeGlyph = LoadDefaultLargeImage();
		}
		protected internal virtual void UpdateItemSuperTip() {
			if (SuperTip == null)
				UpdateSuperTipAndShortCut();
		}
		protected internal virtual void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null)
				command.Execute();
		}
		protected virtual Image LoadDefaultImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.Image : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected virtual Image LoadDefaultLargeImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.LargeImage : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected virtual bool SuperTipHasDefaultContent() {
			return SuperToolTipHelper.SuperTipHasDefaultContent(SuperTip, GetDefaultSuperTipTitle(), GetDefaultSuperTipDescription(), SuperTipShortcut);
		}
		protected internal virtual void UpdateSuperTipAndShortCut() {
			if (ItemShortcut == CommandBasedBarButtonItem.BarShortcutAuto && ShouldAssignItemShortcut)
				RecreateItemShortcut();
			RecreateItemSuperToolTip();
		}
		protected virtual void RecreateItemShortcut() {
			Keys shortCutKeys = GetDefaultShortcutKeys();
			if (shortCutKeys != Keys.None)
				ItemShortcut = new BarShortcut(shortCutKeys);
			else
				ItemShortcut = BarShortcut.Empty;
		}
		protected virtual void RecreateItemSuperToolTip() {
			SuperTip = SuperToolTipHelper.CreateSuperToolTip(GetDefaultSuperTipTitle(), GetDefaultSuperTipDescription(), SuperTipShortcut);
			defaultSuperTip = true;
		}
		protected virtual bool IsEqual(ICommandBarItem item) {
			if (item == null)
				return false;
			if (item == this)
				return true;
			return item.GetType() == this.GetType();
		}
		protected virtual void SetupStatusBarLink(BarItemLink link) {
		}
		#region ICommandBarItem Members
		void ICommandBarItem.UpdateCaption() {
			UpdateCaption();
		}
		void ICommandBarItem.UpdateVisibility() {
			UpdateItemVisibility();
		}
		void ICommandBarItem.UpdateImages() {
			UpdateItemGlyphs();
		}
		void ICommandBarItem.UpdateChecked() {
			UpdateItemChecked();
		}
		void ICommandBarItem.UpdateGroupIndex() {
			UpdateItemGroupIndex();
		}
		void ICommandBarItem.InvokeCommand() {
			InvokeCommand();
		}
		bool ICommandBarItem.IsEqual(ICommandBarItem item) {
			return this.IsEqual(item);
		}
		void ICommandBarItem.SetupStatusBarLink(BarItemLink link) {
			this.SetupStatusBarLink(link);
		}
		#endregion
	}
	#endregion
	#region ControlCommandBarButtonItem<TControl, TCommandId> (abstract class)
	public abstract class ControlCommandBarButtonItem<TControl, TCommandId> : CommandBasedBarButtonItem, IControlCommandBarItem<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		TControl control;
		#endregion
		protected ControlCommandBarButtonItem() {
		}
		protected ControlCommandBarButtonItem(string caption)
			: base(caption) {
		}
		protected ControlCommandBarButtonItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		protected ControlCommandBarButtonItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		#region Control
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TControl Control {
			get { return control; }
			set {
				if (Object.ReferenceEquals(control, value))
					return;
				if (control != null)
					UnsubscribeControlEvents();
				if (DefaultSuperTip)
					SuperTip = null;
				control = value;
				if (control != null)
					SubscribeControlEvents();
				OnControlChanged();
				ItemsSetControl();
				GroupsSetControl();
			}
		}
		#endregion
		#region PaintStyle
		protected internal override bool ShouldSerializePaintStyle() {
			return PaintStyle != BarItemPaintStyle.Standard;
		}
		protected internal override void ResetPaintStyle() {
			PaintStyle = BarItemPaintStyle.Standard;
		}
		#endregion
		protected internal override bool ShouldAssignItemShortcut { get { return false; } }
		protected virtual bool ShouldSetControlFocusAfterInvokeCommand { get { return true; } }
		protected internal abstract TCommandId CommandId { get; }
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (control != null) {
						UnsubscribeControlEvents();
						control = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		public TCommandId GetCommandId() {
			return CommandId;
		}
		protected internal virtual void ItemsSetControl() {
		}
		protected internal virtual void GroupsSetControl() {
		}
		protected internal virtual void SubscribeControlEvents() {
			Control.BeforeDispose += OnBeforeDisposeControl;
			Control.UpdateUI += OnControlUpdateUI;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			Control.BeforeDispose -= OnBeforeDisposeControl;
			Control.UpdateUI -= OnControlUpdateUI;
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.Control = null;
		}
		protected internal virtual void OnControlUpdateUI(object sender, EventArgs e) {
			this.UpdateItemVisibility();
		}
		protected internal override void UpdateItemGroupIndex() {
			this.GroupIndex = 0;
		}
		protected virtual void OnControlChanged() {
		}
		protected internal override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(CommandId);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected override Keys GetDefaultShortcutKeys() {
			if (Control == null)
				return Keys.None;
			CommandBasedKeyboardHandler<TCommandId> keyboardHandler = Control.KeyboardHandler;
			if (keyboardHandler == null)
				return Keys.None;
			return keyboardHandler.GetKeys(CommandId);
		}
		protected internal virtual bool HandleException(Exception e) {
			if (e != null && Control != null)
				return Control.HandleException(e);
			else
				return false;
		}
		protected internal override void InvokeCommand() {
			try {
				Control.CommitImeContent();
				base.InvokeCommand();
				UpdateItemVisibility();
				if (ShouldSetControlFocusAfterInvokeCommand)
					Control.Focus();
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected internal override void UpdateItemVisibility() {
			base.UpdateItemVisibility();
			if (DesignMode && !Enabled && !ShouldDisableBarItem())
				Enabled = true;
			if (Enabled && ShouldDisableBarItem())
				Enabled = false;
		}
		bool ShouldDisableBarItem() {
			return Control == null;
		}
		protected override bool IsEqual(ICommandBarItem item) {
			if (!base.IsEqual(item))
				return false;
			ControlCommandBarButtonItem<TControl, TCommandId> otherItem = (ControlCommandBarButtonItem<TControl, TCommandId>)item;
			return Object.Equals(this.CommandId, otherItem.CommandId);
		}
	}
	#endregion
	#region CommandBarGalleryDropDown
	[DXToolboxItem(false), DesignTimeVisible(false)]
	public class CommandBarGalleryDropDown : GalleryDropDown {
		public CommandBarGalleryDropDown() {
		}
		public CommandBarGalleryDropDown(IContainer container)
			: base(container) {
		}
	}
	#endregion
	public interface ISupportsSelectedGalleryItem {
		void InvokeSelectedGalleryItemCommand(GalleryItem item);
	}
	#region ControlCommandBarButtonGalleryDropDownItem (abstract class)
	public abstract class ControlCommandBarButtonGalleryDropDownItem<TControl, TCommandId> : ControlCommandBarButtonItem<TControl, TCommandId>, ISupportsSelectedGalleryItem
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		Size defaultGalleryImageSize = new Size(65, 46);
		ShowScrollBar defaultShowScrollBar = ShowScrollBar.Auto;
		const bool defaultShowGroupCaption = true;
		const int defaultGalleryColumnCount = 5;
		const int defaultGalleryRowCount = 3;
		BarButtonStyle buttonStyle = BarButtonStyle.DropDown;
		const bool actAsDropDown = true;
		const bool defaultShowItemText = false;
		GalleryDropDown galleryDropDown;
		ControlCommandBarGalleryWalker<TControl, TCommandId> walker = new ControlCommandBarGalleryWalker<TControl, TCommandId>();
		#endregion
		protected ControlCommandBarButtonGalleryDropDownItem()
			: base() {
		}
		protected ControlCommandBarButtonGalleryDropDownItem(string caption)
			: base(caption) {
		}
		protected ControlCommandBarButtonGalleryDropDownItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		protected ControlCommandBarButtonGalleryDropDownItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		public override BarButtonStyle ButtonStyle { get { return buttonStyle; } set { } }
		protected internal override bool ShouldSerializeButtonStyle() {
			return ButtonStyle != buttonStyle;
		}
		public override bool ActAsDropDown { get { return actAsDropDown; } set { } }
		protected internal override bool ShouldSerializeActAsDropDown() {
			return ActAsDropDown != actAsDropDown;
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if(this.galleryDropDown != null)
				this.galleryDropDown.Ribbon = Ribbon;
		}
		public override PopupControl DropDownControl {
			get { return galleryDropDown; }
			set {
				if (galleryDropDown != null)
					UnsubscribeGalleryEvents();
				galleryDropDown = (GalleryDropDown)value;
				if (galleryDropDown != null) {
					SubscribeGalleryEvents();
					galleryDropDown.Ribbon = Ribbon;
					InitializeDropDownGallery(galleryDropDown.Gallery);
				}
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GalleryDropDown GalleryDropDown { get { return galleryDropDown; } }
		protected internal virtual bool ShowGroupCaption { get { return defaultShowGroupCaption; } }
		protected internal virtual Size GalleryImageSize { get { return defaultGalleryImageSize; } }
		protected internal virtual ShowScrollBar ShowScrollBar { get { return defaultShowScrollBar; } }
		protected internal virtual int GalleryColumnCount { get { return defaultGalleryColumnCount; } }
		protected internal virtual int GalleryRowCount { get { return defaultGalleryRowCount; } }
		protected internal virtual bool ShowItemText { get { return defaultShowItemText; } }
		#endregion
		protected internal override void Initialize() {
			galleryDropDown = new CommandBarGalleryDropDown();
			base.Initialize();
		}
		protected internal virtual void SubscribeGalleryEvents() {
		}
		protected internal virtual void UnsubscribeGalleryEvents() {
		}
		protected internal virtual void InitializeDropDownGallery(InDropDownGallery gallery) {
			gallery.ImageSize = GalleryImageSize;
			gallery.ShowGroupCaption = ShowGroupCaption;
			gallery.AllowFilter = false;
			gallery.ShowScrollBar = ShowScrollBar;
			gallery.ColumnCount = GalleryColumnCount;
			gallery.RowCount = GalleryRowCount;
			gallery.ShowItemText = ShowItemText;
		}
		protected internal override void ItemsSetControl() {
			if (GalleryDropDown != null)
				walker.SetItemsControl(GalleryDropDown.Gallery, Control);
		}
		protected internal override void GroupsSetControl() {
			if (GalleryDropDown != null)
				walker.SetGroupsControl(GalleryDropDown.Gallery, Control);
		}
		protected internal override void UpdateItemCaption() {
			base.UpdateItemCaption();
			GroupsUpdateCaption();
			ItemsUpdateCaption();
		}
		protected internal virtual void GroupsUpdateCaption() {
			if (GalleryDropDown != null)
				walker.UpdateGroupsCaption(GalleryDropDown.Gallery);
		}
		protected internal virtual void ItemsUpdateCaption() {
			if (GalleryDropDown != null)
				walker.UpdateItemsCaption(GalleryDropDown.Gallery);
		}
		protected internal override void UpdateItemGlyphs() {
			base.UpdateItemGlyphs();
			ItemsUpdateImages();
		}
		protected internal virtual void ItemsUpdateImages() {
			if (GalleryDropDown != null)
				walker.UpdateItemsImages(GalleryDropDown.Gallery);
		}
		protected internal override void UpdateItemVisibility() {
			ItemsUpdateUIState();
			GroupsUpdateVisibility();
			base.UpdateItemVisibility();
		}
		protected internal virtual void GroupsUpdateVisibility() {
			GalleryItemGroupProcessorDelegate groupsUpdateVisibility = delegate(GalleryItemGroup group) {
				ICommandBarItem currentItem = group as ICommandBarItem;
				if (currentItem != null)
					currentItem.UpdateVisibility();
			};
			if (GalleryDropDown != null)
				walker.ForEachGalleryGroups(GalleryDropDown.Gallery, groupsUpdateVisibility);
		}
		protected internal override void UpdateItemChecked() {
			base.UpdateItemChecked();
			ItemsUpdateUIState();
		}
		protected internal virtual void ItemsUpdateUIState() {
			GalleryItemProcessorDelegate itemsUpdateUIStateCore = delegate(GalleryItem item) {
				CommandBasedGalleryItem currentItem = item as CommandBasedGalleryItem;
				if (currentItem != null)
					currentItem.UpdateUIStateCore();
			};
			if (GalleryDropDown != null)
				walker.ForEachGalleryItems(GalleryDropDown.Gallery, itemsUpdateUIStateCore);
		}
		public virtual void InvokeSelectedGalleryItemCommand(GalleryItem item) {
			ICommandBarItem currentItem = item as ICommandBarItem;
			if (currentItem != null)
				currentItem.InvokeCommand();
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing)
					DisposeItems(disposing);
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal virtual void DisposeItems(bool disposing) {
			GalleryItemProcessorDelegate disposeItems = delegate(GalleryItem item) {
				CommandBasedGalleryItem currentItem = item as CommandBasedGalleryItem;
				if (currentItem != null)
					currentItem.Dispose(disposing);
			};
			if (GalleryDropDown != null)
				walker.ForEachGalleryItems(GalleryDropDown.Gallery, disposeItems);
		}
	}
	#endregion
	public delegate void GalleryItemProcessorDelegate(GalleryItem item);
	public delegate void GalleryItemGroupProcessorDelegate(GalleryItemGroup group);
	public class ControlCommandBarGalleryWalker<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		public void SetItemsControl(StandaloneGallery gallery, TControl control) {
			GalleryItemProcessorDelegate itemsSetControl = delegate(GalleryItem item) {
				ControlCommandGalleryItem<TControl, TCommandId> currentItem = item as ControlCommandGalleryItem<TControl, TCommandId>;
				if (currentItem != null)
					currentItem.Control = control;
			};
			ForEachGalleryItems(gallery, itemsSetControl);
		}
		public void SetGroupsControl(StandaloneGallery gallery, TControl control) {
			GalleryItemGroupProcessorDelegate groupsSetControl = delegate(GalleryItemGroup group) {
				ControlCommandGalleryItemGroup<TControl, TCommandId> currentGroup = group as ControlCommandGalleryItemGroup<TControl, TCommandId>;
				if (currentGroup != null)
					currentGroup.Control = control;
			};
			ForEachGalleryGroups(gallery, groupsSetControl);
		}
		public void UpdateGroupsCaption(StandaloneGallery gallery) {
			GalleryItemGroupProcessorDelegate groupsUpdateCaption = delegate(GalleryItemGroup group) {
				ICommandBarItem currentGroup = group as ICommandBarItem;
				if (currentGroup != null)
					currentGroup.UpdateCaption();
			};
			ForEachGalleryGroups(gallery, groupsUpdateCaption);
		}
		public void UpdateItemsCaption(StandaloneGallery gallery) {
			GalleryItemProcessorDelegate itemsUpdateCaption = delegate(GalleryItem item) {
				ICommandBarItem currentItem = item as ICommandBarItem;
				if (currentItem != null)
					currentItem.UpdateCaption();
			};
			ForEachGalleryItems(gallery, itemsUpdateCaption);
		}
		public void UpdateItemsImages(StandaloneGallery gallery) {
			GalleryItemProcessorDelegate itemsUpdateImages = delegate(GalleryItem item) {
				ICommandBarItem currentItem = item as ICommandBarItem;
				if (currentItem != null)
					currentItem.UpdateImages();
			};
			ForEachGalleryItems(gallery, itemsUpdateImages);
		}
		public void ForEachGalleryGroups(StandaloneGallery gallery, GalleryItemGroupProcessorDelegate galleryGroupProcessor) {
			GalleryItemGroupCollection groups = gallery.Groups;
			int groupsCount = groups.Count;
			for (int i = 0; i < groupsCount; i++) {
				galleryGroupProcessor(groups[i]);
			}
		}
		public void ForEachGalleryItems(StandaloneGallery gallery, GalleryItemProcessorDelegate galleryItemProcessor) {
			if (gallery == null)
				return;
			gallery.BeginUpdate();
			try {
				GalleryItemGroupCollection groups = gallery.Groups;
				int groupsCount = groups.Count;
				for (int i = 0; i < groupsCount; i++) {
					GalleryItemCollection items = groups[i].Items;
					int itemsCount = items.Count;
					for (int j = 0; j < itemsCount; j++) {
						galleryItemProcessor(items[j]);
					}
				}
			}
			finally {
				gallery.EndUpdate();
			}
		}
	}
	#region CommandBasedGalleryBarItem (abstract class)
	public abstract class CommandBasedGalleryBarItem : RibbonGalleryBarItem, ICommandBarItem {
		#region Fields
		Size defaultGalleryImageSize = new Size(65, 46);
		const bool defaultDropDownGalleryShowGroupCaption = false;
		GalleryItem selectedItem;
		#endregion
		protected CommandBasedGalleryBarItem() {
			Initialize();
			SubscribeEvents();
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GalleryItem SelectedItem { get { return selectedItem; } set { selectedItem = value; } }
		protected internal virtual bool DropDownGalleryShowGroupCaption { get { return defaultDropDownGalleryShowGroupCaption; } }
		protected internal virtual Size GalleryImageSize { get { return defaultGalleryImageSize; } }
		protected internal bool CanUpdateVisibility { get { return Manager != null && !Manager.IsDesignMode && !Manager.IsLoading; } }
		public delegate void GalleryItemProcessorDelegate(GalleryItem item);
		public delegate void GalleryItemGroupProcessorDelegate(GalleryItemGroup group);
		#endregion
		protected internal abstract Command CreateCommand();
		protected internal virtual void Initialize() {
			Gallery.ImageSize = GalleryImageSize;
		}
		protected internal virtual void SubscribeEvents() {
			Gallery.InitDropDownGallery += InitDropDownGallery;
		}
		protected internal virtual void InitDropDownGallery(object sender, InplaceGalleryEventArgs e) {
			InDropDownGallery popupGallery = e.PopupGallery;
			popupGallery.AllowFilter = false;
			popupGallery.ShowGroupCaption = DropDownGalleryShowGroupCaption;
		}
		protected internal virtual void UpdateItemCaption() {
			if (String.IsNullOrEmpty(Caption))
				Caption = GetDefaultCaption();
			GroupsUpdateCaption();
			ItemsUpdateCaption();
		}
		protected internal override bool ShouldSerializeCaption() {
			return Caption != GetDefaultCaption();
		}
		protected internal override void ResetCaption() {
			Caption = GetDefaultCaption();
		}
		protected virtual string GetDefaultCaption() {
			Command command = CreateCommand();
			return command != null ? command.MenuCaption : String.Empty;
		}
		protected internal virtual void GroupsUpdateCaption() {
			GalleryItemGroupProcessorDelegate groupsUpdateCaption = delegate(GalleryItemGroup group) {
				ICommandBarItem currentGroup = group as ICommandBarItem;
				if (currentGroup != null)
					currentGroup.UpdateCaption();
			};
			ForEachGalleryGroups(groupsUpdateCaption);
		}
		protected internal virtual void ItemsUpdateCaption() {
			GalleryItemProcessorDelegate itemsUpdateCaption = delegate(GalleryItem item) {
				ICommandBarItem currentItem = item as ICommandBarItem;
				if (currentItem != null)
					currentItem.UpdateCaption();
			};
			ForEachGalleryItems(itemsUpdateCaption);
		}
		protected internal virtual void InvokeCommand() {
			ICommandBarItem currentItem = SelectedItem as ICommandBarItem;
			if (currentItem != null)
				currentItem.InvokeCommand();
		}
		protected internal virtual bool IsEqual(ICommandBarItem item) {
			if (item == null)
				return false;
			if (item == this)
				return true;
			return item.GetType() == this.GetType();
		}
		protected internal virtual void UpdateItemGlyphs() {
			Glyph = CommandBasedBarItemGlyphHelper.GetActualGlyph(Glyph, Images, ImageIndex);
			if (Glyph == null)
				Glyph = LoadDefaultImage();
			LargeGlyph = CommandBasedBarItemGlyphHelper.GetActualGlyph(LargeGlyph, LargeImages, LargeImageIndex);
			if (LargeGlyph == null)
				LargeGlyph = LoadDefaultLargeImage();
			ItemsUpdateImages();
		}
		protected virtual Image LoadDefaultImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.Image : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected internal override bool ShouldSerializeGlyph() {
			if (GlyphComparer.IsDefaultGlyph(Glyph))
				return false;
			return base.ShouldSerializeGlyph();
		}
		protected internal override void ResetGlyph() {
			Glyph = LoadDefaultImage();
		}
		protected virtual Image LoadDefaultLargeImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.LargeImage : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected internal override bool ShouldSerializeLargeGlyph() {
			if (GlyphComparer.IsDefaultGlyph(LargeGlyph))
				return false;
			return base.ShouldSerializeLargeGlyph();
		}
		protected internal override void ResetLargeGlyph() {
			LargeGlyph = LoadDefaultLargeImage();
		}
		protected internal virtual void ItemsUpdateImages() {
			GalleryItemProcessorDelegate itemsUpdateImages = delegate(GalleryItem item) {
				ICommandBarItem currentItem = item as ICommandBarItem;
				if (currentItem != null)
					currentItem.UpdateImages();
			};
			ForEachGalleryItems(itemsUpdateImages);
		}
		protected internal virtual void UpdateItemVisibility() {
			UpdateUIStateCore();
			GroupsUpdateVisibility();
		}
		protected internal virtual void GroupsUpdateVisibility() {
			GalleryItemGroupProcessorDelegate groupsUpdateVisibility = delegate(GalleryItemGroup group) {
				ICommandBarItem currentItem = group as ICommandBarItem;
				if (currentItem != null)
					currentItem.UpdateVisibility();
			};
			ForEachGalleryGroups(groupsUpdateVisibility);
		}
		protected internal virtual void UpdateItemChecked() {
			UpdateUIStateCore();
		}
		protected internal virtual void UpdateUIStateCore() {
			if (!CanUpdateVisibility)
				return;
			BeginUpdate();
			try {
				Command command = CreateCommand();
				if (command != null) {
					ICommandUIState state = CreateGalleryItemUIState();
					command.UpdateUIState(state);
				}
			}
			finally {
				EndUpdate();
			}
			ItemsUpdateUIState();
			MakeVisibleCheckedItem();
		}
		protected internal virtual ICommandUIState CreateGalleryItemUIState() {
			return new BarGalleryItemUIState(this);
		}
		protected internal virtual void ItemsUpdateUIState() {
			GalleryItemProcessorDelegate itemsUpdateUIStateCore = delegate(GalleryItem item) {
				CommandBasedGalleryItem currentItem = item as CommandBasedGalleryItem;
				if (currentItem != null)
					currentItem.UpdateUIStateCore();
			};
			ForEachGalleryItems(itemsUpdateUIStateCore);
		}
		protected internal virtual void MakeVisibleCheckedItem() {
			List<GalleryItem> items = Gallery.GetCheckedItems();
			if (items.Count == 1)
				MakeVisible(items[0]);
		}
		protected internal virtual void UpdateItemGroupIndex() {
		}
		protected virtual void SetupStatusBarLink(BarItemLink link) {
		}
		protected internal virtual void ForEachGalleryGroups(GalleryItemGroupProcessorDelegate galleryGroupProcessor) {
			GalleryItemGroupCollection groups = Gallery.Groups;
			int groupsCount = groups.Count;
			for (int i = 0; i < groupsCount; i++) {
				galleryGroupProcessor(groups[i]);
			}
		}
		protected internal virtual void ForEachGalleryItems(GalleryItemProcessorDelegate galleryItemProcessor) {
			Gallery.BeginUpdate();
			try {
				GalleryItemGroupCollection groups = Gallery.Groups;
				int groupsCount = groups.Count;
				for (int i = 0; i < groupsCount; i++) {
					GalleryItemCollection items = groups[i].Items;
					int itemsCount = items.Count;
					for (int j = 0; j < itemsCount; j++) {
						galleryItemProcessor(items[j]);
					}
				}
			}
			finally {
				Gallery.EndUpdate();
			}
		}
		#region ICommandBarItem Members
		void ICommandBarItem.UpdateCaption() {
			UpdateItemCaption();
		}
		void ICommandBarItem.UpdateImages() {
			UpdateItemGlyphs();
		}
		void ICommandBarItem.UpdateVisibility() {
			UpdateItemVisibility();
		}
		void ICommandBarItem.UpdateChecked() {
			UpdateItemChecked();
		}
		void ICommandBarItem.UpdateGroupIndex() {
			UpdateItemGroupIndex();
		}
		void ICommandBarItem.InvokeCommand() {
			InvokeCommand();
		}
		bool ICommandBarItem.IsEqual(ICommandBarItem item) {
			return IsEqual(item);
		}
		void ICommandBarItem.SetupStatusBarLink(BarItemLink link) {
			SetupStatusBarLink(link);
		}
		#endregion
	}
	#endregion
	#region CommandBasedSkinGalleryBarItem (abstract class)
	public abstract class CommandBasedSkinGalleryBarItem : SkinRibbonGalleryBarItem, ICommandBarItem {
		protected CommandBasedSkinGalleryBarItem() {
		}
		protected internal abstract Command CreateCommand();
		protected internal virtual void UpdateItemCaption() {
			if(string.IsNullOrEmpty(Caption)) Caption = GetDefaultCaption();
		}
		protected internal override bool ShouldSerializeCaption() {
			return Caption != GetDefaultCaption();
		}
		protected internal override void ResetCaption() {
			Caption = GetDefaultCaption();
		}
		protected virtual string GetDefaultCaption() {
			Command command = CreateCommand();
			return command != null ? command.MenuCaption : String.Empty;
		}
		protected internal virtual bool IsEqual(ICommandBarItem item) {
			if(item == null)
				return false;
			if(item == this)
				return true;
			return item.GetType() == this.GetType();
		}
		protected internal override bool ShouldSerializeGlyph() {
			return false;
		}
		protected internal override void ResetGlyph() {
			Glyph = null;
		}
		protected internal override bool ShouldSerializeLargeGlyph() {
			return false;
		}
		protected internal override void ResetLargeGlyph() {
			LargeGlyph = null;
		}
		#region ICommandBarItem Members
		void ICommandBarItem.UpdateCaption() {
			UpdateItemCaption();
		}
		bool ICommandBarItem.IsEqual(ICommandBarItem item) {
			return IsEqual(item);
		}
		void ICommandBarItem.UpdateImages() { }
		void ICommandBarItem.UpdateVisibility() { UpdateItemVisibility(); }
		void ICommandBarItem.UpdateChecked() { }
		void ICommandBarItem.UpdateGroupIndex() { }
		void ICommandBarItem.InvokeCommand() { }
		void ICommandBarItem.SetupStatusBarLink(BarItemLink link) { }
		#endregion
		protected internal virtual void UpdateItemVisibility() {
			UpdateUIStateCore();
		}
		protected internal bool CanUpdateVisibility { get { return Manager != null && !Manager.IsDesignMode && !Manager.IsLoading; } }
		protected internal virtual void UpdateUIStateCore() {
			if(!CanUpdateVisibility)
				return;
			BeginUpdate();
			try {
				Command command = CreateCommand();
				if(command != null) {
					ICommandUIState state = CreateButtonItemUIState();
					command.UpdateUIState(state);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual ICommandUIState CreateButtonItemUIState() {
			return new RibbonGalleryBarItemUIState(this);
		}
	}
	#endregion
	#region ControlCommandGalleryBarItem (abstract class)
	public abstract class ControlCommandGalleryBarItem<TControl, TCommandId> : CommandBasedGalleryBarItem, IControlCommandBarItem<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		TControl control;
		#endregion
		protected ControlCommandGalleryBarItem() {
		}
		#region Properties
		#region Control
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TControl Control {
			get { return control; }
			set {
				if (Object.ReferenceEquals(control, value))
					return;
				if (control != null)
					UnsubscribeControlEvents();
				control = value;
				if (control != null)
					SubscribeControlEvents();
				OnControlChanged();
				GroupsSetControl();
				ItemsSetControl();
			}
		}
		#endregion
		protected internal abstract TCommandId CommandId { get; }
		#endregion
		protected internal virtual void ItemsSetControl() {
			GalleryItemProcessorDelegate itemsSetControl = delegate(GalleryItem item) {
				ControlCommandGalleryItem<TControl, TCommandId> currentItem = item as ControlCommandGalleryItem<TControl, TCommandId>;
				if (currentItem != null)
					currentItem.Control = Control;
			};
			ForEachGalleryItems(itemsSetControl);
		}
		protected internal virtual void GroupsSetControl() {
			GalleryItemGroupProcessorDelegate groupsSetControl = delegate(GalleryItemGroup group) {
				ControlCommandGalleryItemGroup<TControl, TCommandId> currentGroup = group as ControlCommandGalleryItemGroup<TControl, TCommandId>;
				if (currentGroup != null)
					currentGroup.Control = Control;
			};
			ForEachGalleryGroups(groupsSetControl);
		}
		public TCommandId GetCommandId() {
			return CommandId;
		}
		protected internal virtual void SubscribeControlEvents() {
			Control.BeforeDispose += OnBeforeDisposeControl;
			Control.UpdateUI += OnControlUpdateUI;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			Control.BeforeDispose -= OnBeforeDisposeControl;
			Control.UpdateUI -= OnControlUpdateUI;
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.Control = null;
		}
		protected internal virtual void OnControlUpdateUI(object sender, EventArgs e) {
			this.UpdateItemVisibility();
		}
		protected internal override void UpdateItemVisibility() {
			base.UpdateItemVisibility();
			if (DesignMode && !Enabled && !ShouldDisableBarItem())
				Enabled = true;
			if (Enabled && ShouldDisableBarItem())
				Enabled = false;
		}
		bool ShouldDisableBarItem() {
			return Control == null;
		}
		protected virtual void OnControlChanged() {
		}
		protected internal override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(CommandId);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					DisposeItems(disposing);
					if (control != null) {
						UnsubscribeControlEvents();
						control = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal virtual void DisposeItems(bool disposing) {
			GalleryItemProcessorDelegate disposeItems = delegate(GalleryItem item) {
				CommandBasedGalleryItem currentItem = item as CommandBasedGalleryItem;
				if (currentItem != null)
					currentItem.Dispose(disposing);
			};
			ForEachGalleryItems(disposeItems);
		}
		#endregion
	}
	#endregion
	#region ControlCommandSkinGalleryBarItem (abstract class)
	public abstract class ControlCommandSkinGalleryBarItem<TControl, TCommandId> : CommandBasedSkinGalleryBarItem, IControlCommandBarItem<TControl, TCommandId> where TControl : class, ICommandAwareControl<TCommandId> where TCommandId : struct {
		#region Fields
		TControl control;
		#endregion
		protected ControlCommandSkinGalleryBarItem() {
		}
		#region Properties
		#region Control
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual TControl Control {
			get { return control; }
			set {
				if(object.ReferenceEquals(control, value))
					return;
				if(control != null)
					UnsubscribeControlEvents();
				control = value;
				if(control != null)
					SubscribeControlEvents();
				OnControlChanged();
			}
		}
		#endregion
		protected internal abstract TCommandId CommandId { get; }
		#endregion
		public TCommandId GetCommandId() {
			return CommandId;
		}
		protected internal virtual void SubscribeControlEvents() {
			Control.BeforeDispose += OnBeforeDisposeControl;
			Control.UpdateUI += OnControlUpdateUI;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			Control.BeforeDispose -= OnBeforeDisposeControl;
			Control.UpdateUI -= OnControlUpdateUI;
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.Control = null;
		}
		protected internal virtual void OnControlUpdateUI(object sender, EventArgs e) {
		}
		bool ShouldDisableBarItem() {
			return Control == null;
		}
		protected virtual void OnControlChanged() {
		}
		protected internal override Command CreateCommand() {
			if(Control == null)
				return null;
			Command command = Control.CreateCommand(CommandId);
			if(command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected internal override void UpdateItemVisibility() {
			base.UpdateItemVisibility();
			if(DesignMode && !Enabled && !ShouldDisableBarItem())
				Enabled = true;
			if(Enabled && ShouldDisableBarItem())
				Enabled = false;
		}
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(control != null) {
						UnsubscribeControlEvents();
						control = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
	}
	#endregion
	#region CommandBasedGalleryItem (abstract class)
	public abstract class CommandBasedGalleryItem : GalleryItem, ICommandBarItem, IDisposable {
		#region Fields
		bool captionAsValue;
		bool alwaysUpdateDescription;
		bool isEmptyHint;
		#endregion
		#region Properties
		[DefaultValue(false), Browsable(false), Localizable(false)]
		public bool CaptionAsValue { get { return captionAsValue; } set { captionAsValue = value; } }
		[DefaultValue(false), Browsable(false), Localizable(false)]
		public bool AlwaysUpdateDescription { get { return alwaysUpdateDescription; } set { alwaysUpdateDescription = value; } }
		[DefaultValue(false), Browsable(false), Localizable(false)]
		public bool IsEmptyHint { get { return isEmptyHint; } set { isEmptyHint = value; } }
		#endregion
		#region Caption
		protected internal virtual void UpdateItemCaption() {
			if (!String.IsNullOrEmpty(Caption))
				return;
			string caption = GetDefaultCaption();
			Caption = caption;
			if (CaptionAsValue)
				Value = caption;
		}
		protected internal override bool ShouldSerializeCaption() {
			return Caption != GetDefaultCaption();
		}
		protected internal override void ResetCaption() {
			Caption = GetDefaultCaption();
		}
		protected virtual string GetDefaultCaption() {
			Command command = CreateCommand();
			return command != null ? command.MenuCaption : String.Empty;
		}
		#endregion
		#region Value
		protected internal override bool ShouldSerializeValue() {
			if (CaptionAsValue)
				return !Object.Equals(Value, GetDefaultCaption());
			return base.ShouldSerializeValue();
		}
		protected internal override void ResetValue() {
			if (CaptionAsValue)
				Value = GetDefaultCaption();
			else
				base.ResetValue();
		}
		#endregion
		#region Description
		protected internal virtual void UpdateItemDescription() {
			if (String.IsNullOrEmpty(Description) || AlwaysUpdateDescription)
				Description = GetDefaultDescription();
		}
		protected internal override bool ShouldSerializeDescription() {
			return Description != GetDefaultDescription();
		}
		protected internal override void ResetDescription() {
			Description = GetDefaultDescription();
		}
		protected virtual string GetDefaultDescription() {
			Command command = CreateCommand();
			return command != null ? command.Description : String.Empty;
		}
		#endregion
		protected internal abstract Command CreateCommand();
		protected internal virtual void UpdateItemHint() {
			if (String.IsNullOrEmpty(Hint))
				Hint = GetDefaultHint();
		}
		protected internal override bool ShouldSerializeHint() {
			return Hint != GetDefaultHint();
		}
		protected internal override void ResetHint() {
			Hint = GetDefaultHint();
		}
		protected virtual string GetDefaultHint() {
			if (IsEmptyHint)
				return String.Empty;
			Command command = CreateCommand();
			return command != null ? command.Description : String.Empty;
		}
		protected internal virtual void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null)
				command.Execute();
		}
		protected internal virtual ICommandUIState CreateCommandUIState(Command command) {
			return command.CreateDefaultCommandUIState();
		}
		protected internal virtual bool IsEqual(ICommandBarItem item) {
			if (item == null)
				return false;
			if (item == this)
				return true;
			return item.GetType() == this.GetType();
		}
		protected internal virtual void UpdateItemGlyphs() {
			if (Image == null && ImageIndex == -1)
				Image = LoadDefaultImage();
		}
		protected internal override bool ShouldSerializeImage() {
			if (Image != null && Object.Equals(Image.Tag, BarCollectorBase.DefaultImageTag))
				return false;
			return base.ShouldSerializeImage();
		}
		protected internal override void ResetImage() {
			Image = LoadDefaultImage();
		}
		protected virtual Image LoadDefaultImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.LargeImage : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected internal virtual void UpdateItemVisibility() {
			UpdateUIStateCore();
		}
		protected internal virtual void UpdateItemChecked() {
			UpdateUIStateCore();
		}
		protected internal virtual void UpdateUIStateCore() {
			Command command = CreateCommand();
			if (command != null) {
				GalleryItemUIState state = new GalleryItemUIState(this);
				command.UpdateUIState(state);
			}
		}
		protected internal virtual void UpdateItemGroupIndex() {
		}
		protected virtual void SetupStatusBarLink(BarItemLink link) {
		}
		protected override SuperToolTip GetSuperTip() {
			SuperToolTip superTip = new SuperToolTip();
			ToolTipTitleItem titleItem = new ToolTipTitleItem();
			ToolTipItem item = new ToolTipItem();
			titleItem.Text = Caption;
			item.Text = Hint;
			superTip.Items.Add(titleItem);
			superTip.Items.Add(item);
			return superTip;
		}
		#region ICommandBarItem Members
		void ICommandBarItem.UpdateCaption() {
			UpdateItemCaption();
			UpdateItemDescription();
			UpdateItemHint();
		}
		void ICommandBarItem.UpdateImages() {
			UpdateItemGlyphs();
		}
		void ICommandBarItem.UpdateVisibility() {
			UpdateItemVisibility();
		}
		void ICommandBarItem.UpdateChecked() {
			UpdateItemChecked();
		}
		void ICommandBarItem.UpdateGroupIndex() {
			UpdateItemGroupIndex();
		}
		void ICommandBarItem.InvokeCommand() {
			InvokeCommand();
		}
		bool ICommandBarItem.IsEqual(ICommandBarItem item) {
			return IsEqual(item);
		}
		void ICommandBarItem.SetupStatusBarLink(BarItemLink link) {
			SetupStatusBarLink(link);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected internal virtual void Dispose(bool disposing) {
		}
		#endregion
	}
	#endregion
	#region ControlCommandGalleryItem (abstract class)
	public abstract class ControlCommandGalleryItem<TControl, TCommandId> : CommandBasedGalleryItem, IControlCommandBarItem<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		TControl control;
		#endregion
		protected ControlCommandGalleryItem() {
		}
		#region Properties
		#region Control
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TControl Control {
			get { return control; }
			set {
				if (Object.ReferenceEquals(control, value))
					return;
				if (control != null)
					UnsubscribeControlEvents();
				control = value;
				if (control != null)
					SubscribeControlEvents();
				OnControlChanged();
			}
		}
		#endregion
		protected internal abstract TCommandId CommandId { get; }
		#endregion
		public TCommandId GetCommandId() {
			return CommandId;
		}
		protected internal virtual void SubscribeControlEvents() {
			Control.BeforeDispose += OnBeforeDisposeControl;
			Control.UpdateUI += OnControlUpdateUI;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			Control.BeforeDispose -= OnBeforeDisposeControl;
			Control.UpdateUI -= OnControlUpdateUI;
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.Control = null;
		}
		protected internal virtual void OnControlUpdateUI(object sender, EventArgs e) {
			this.UpdateItemVisibility();
		}
		protected internal override void UpdateItemVisibility() {
			base.UpdateItemVisibility();
			if (Gallery != null && Gallery.DesignModeCore && !Enabled && !ShouldDisableBarItem())
				Enabled = true;
			if (Enabled && ShouldDisableBarItem())
				Enabled = false;
		}
		bool ShouldDisableBarItem() {
			return Control == null;
		}
		protected virtual void OnControlChanged() {
		}
		protected internal override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(CommandId);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		#region IDisposable Members
		protected internal override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (control != null) {
						UnsubscribeControlEvents();
						control = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
	}
	#endregion
	#region CommandBasedGalleryItemGroup (abstract class)
	public abstract class CommandBasedGalleryItemGroup : GalleryItemGroup, ICommandBarItem, IDisposable {
		protected internal abstract Command CreateCommand();
		protected internal virtual void UpdateItemCaption() {
			if (String.IsNullOrEmpty(Caption))
				Caption = GetDefaultCaption();
		}
		protected internal override bool ShouldSerializeCaption() {
			return Caption != GetDefaultCaption();
		}
		protected internal override void ResetCaption() {
			Caption = GetDefaultCaption();
		}
		protected virtual string GetDefaultCaption() {
			Command command = CreateCommand();
			return command != null ? command.MenuCaption : String.Empty;
		}
		protected internal virtual void UpdateGroupVisibility() {
			Command command = CreateCommand();
			if (command != null) {
				GalleryItemGroupUIState state = new GalleryItemGroupUIState(this);
				command.UpdateUIState(state);
			}
		}
		protected internal virtual bool IsEqual(ICommandBarItem item) {
			if (item == null)
				return false;
			if (item == this)
				return true;
			return item.GetType() == this.GetType();
		}
		#region ICommandBarItem Members
		void ICommandBarItem.UpdateCaption() {
			UpdateItemCaption();
		}
		void ICommandBarItem.UpdateImages() {
		}
		void ICommandBarItem.UpdateVisibility() {
			UpdateGroupVisibility();
		}
		void ICommandBarItem.UpdateChecked() {
		}
		void ICommandBarItem.UpdateGroupIndex() {
		}
		void ICommandBarItem.InvokeCommand() {
		}
		bool ICommandBarItem.IsEqual(ICommandBarItem item) {
			return IsEqual(item);
		}
		void ICommandBarItem.SetupStatusBarLink(BarItemLink link) {
		}
		#endregion
	}
	#endregion
	#region ControlCommandGalleryItemGroup (abstract class)
	public abstract class ControlCommandGalleryItemGroup<TControl, TCommandId> : CommandBasedGalleryItemGroup, IControlCommandBarItem<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		TControl control;
		#endregion
		protected ControlCommandGalleryItemGroup() {
		}
		#region Properties
		#region Control
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TControl Control {
			get { return control; }
			set {
				if (Object.ReferenceEquals(control, value))
					return;
				if (control != null)
					UnsubscribeControlEvents();
				control = value;
				if (control != null)
					SubscribeControlEvents();
				OnControlChanged();
			}
		}
		#endregion
		protected internal abstract TCommandId CommandId { get; }
		#endregion
		public TCommandId GetCommandId() {
			return CommandId;
		}
		protected internal virtual void SubscribeControlEvents() {
			Control.BeforeDispose += OnBeforeDisposeControl;
			Control.UpdateUI += OnControlUpdateUI;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			Control.BeforeDispose -= OnBeforeDisposeControl;
			Control.UpdateUI -= OnControlUpdateUI;
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.Control = null;
		}
		protected internal virtual void OnControlUpdateUI(object sender, EventArgs e) {
			this.UpdateGroupVisibility();
		}
		protected internal override void UpdateGroupVisibility() {
			base.UpdateGroupVisibility();
		}
		protected virtual void OnControlChanged() {
		}
		protected internal override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(CommandId);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		#region IDisposable Members
		protected internal override void Dispose(bool disposing) {
			base.Dispose(disposing);
			try {
				if (disposing) {
					if (control != null) {
						UnsubscribeControlEvents();
						control = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
	}
	#endregion
	#region CommandBasedBarEditItem<T> (abstract class)
	public abstract class CommandBasedBarEditItem<T> : BarEditItem, ICommandBarItem {
		bool defaultSuperTip;
		protected bool DefaultSuperTip { get { return defaultSuperTip; } }
		protected CommandBasedBarEditItem() {
			Initialize();
		}
		protected CommandBasedBarEditItem(string caption)
			: base() {
			Caption = caption;
			Initialize();
		}
		protected CommandBasedBarEditItem(BarManager manager)
			: base(manager, string.Empty) {
			Initialize();
		}
		protected CommandBasedBarEditItem(BarManager manager, string caption)
			: base(manager, String.Empty) {
			Initialize();
			Caption = caption;
		}
		#region Properties
		public override string Caption {
			get { return base.Caption; }
			set {
				base.Caption = value;
				if (defaultSuperTip && !SuperTipHasDefaultContent())
					UpdateSuperTipAndShortCut();
			}
		}
		protected internal virtual BarShortcut SuperTipShortcut {
			get {
				if (!ShouldAssignItemShortcut && CommandBasedBarButtonItem.BarShortcutAuto == ItemShortcut)
					return new BarShortcut(GetDefaultShortcutKeys());
				else
					return ItemShortcut;
			}
		}
		public override string ShortcutKeyDisplayString { get { return GetShortcutKeyDisplayString(SuperTipShortcut); } set { base.ShortcutKeyDisplayString = value; } }
		protected internal virtual bool ShouldAssignItemShortcut { get { return true; } }
		public override BarShortcut ItemShortcut {
			get { return base.ItemShortcut; }
			set {
				base.ItemShortcut = value;
				if (defaultSuperTip && !SuperTipHasDefaultContent())
					UpdateSuperTipAndShortCut();
			}
		}
		protected internal override void ResetItemShortcut() {
			ItemShortcut = CommandBasedBarButtonItem.BarShortcutAuto;
		}
		protected internal override bool ShouldSerializeItemShortcut() {
			return !CommandBasedBarButtonItem.BarShortcutAuto.Equals(ItemShortcut) && !new BarShortcut(GetDefaultShortcutKeys()).Equals(ItemShortcut);
		}
		protected internal override bool ShouldSerializePaintStyle() {
			return PaintStyle != BarItemPaintStyle.CaptionGlyph;
		}
		protected internal override void ResetPaintStyle() {
			PaintStyle = BarItemPaintStyle.CaptionGlyph;
		}
		protected internal override bool ShouldSerializeRibbonStyle() {
			return RibbonStyle != DefaultRibbonStyle;
		}
		protected internal override void ResetRibbonStyle() {
			RibbonStyle = DefaultRibbonStyle;
		}
		protected internal bool CanUpdateVisibility { get { return Manager != null && !Manager.IsDesignMode && !Manager.IsLoading; } }
		protected internal virtual RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.All; } }
		protected internal override bool ShouldSerializeGlyph() {
			if (GlyphComparer.IsDefaultGlyph(Glyph))
				return false;
			return base.ShouldSerializeGlyph();
		}
		protected internal override void ResetGlyph() {
			Glyph = LoadDefaultImage();
		}
		protected internal override bool ShouldSerializeLargeGlyph() {
			if (GlyphComparer.IsDefaultGlyph(LargeGlyph))
				return false;
			return base.ShouldSerializeLargeGlyph();
		}
		protected internal override void ResetLargeGlyph() {
			LargeGlyph = LoadDefaultLargeImage();
		}
		#endregion
		protected internal abstract Command CreateCommand();
		protected internal virtual void Initialize() {
			ItemShortcut = CommandBasedBarButtonItem.BarShortcutAuto;
			ResetRibbonStyle();
			ResetPaintStyle();
			UpdateItemGlyphs();
		}
		protected virtual string GetDefaultCaption() {
			Command command = CreateCommand();
			return command != null ? command.MenuCaption : String.Empty;
		}
		protected virtual string GetDefaultSuperTipTitle() {
			return Caption;
		}
		protected virtual string GetDefaultSuperTipDescription() {
			Command command = CreateCommand();
			return command != null ? command.Description : String.Empty;
		}
		protected virtual Keys GetDefaultShortcutKeys() {
			return Keys.None;
		}
		protected internal override bool ShouldSerializeCaption() {
			return GetDefaultCaption() != Caption;
		}
		protected internal override void ResetCaption() {
			Caption = GetDefaultCaption();
		}
		protected override bool ShouldSerializeSuperTip() {
			return !SuperTipHasDefaultContent();
		}
		public override void ResetSuperTip() {
			UpdateSuperTipAndShortCut();
		}
		protected internal virtual void UpdateItemCaption() {
			if (String.IsNullOrEmpty(Caption))
				Caption = GetDefaultCaption();
		}
		protected internal virtual void UpdateItemGroupIndex() {
		}
		protected internal virtual BarEditItemUIState<T> CreateBarEditItemUIState() {
			BarEditItemUIState<T> state = new BarEditItemUIState<T>(this);
			return state;
		}
		protected internal virtual void UpdateItemChecked() {
			if (!CanUpdateVisibility)
				return;
			BeginUpdate();
			try {
				Command command = CreateCommand();
				if (command != null) {
					BarEditItemUIState<T> state = CreateBarEditItemUIState();
					command.UpdateUIState(state);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void UpdateItemVisibility() {
			if (!CanUpdateVisibility)
				return;
			BeginUpdate();
			try {
				Command command = CreateCommand();
				if (command != null) {
					BarEditItemUIState<T> state = CreateBarEditItemUIState();
					command.UpdateUIState(state);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void UpdateItemGlyphs() {
			Glyph = CommandBasedBarItemGlyphHelper.GetActualGlyph(Glyph, Images, ImageIndex);
			if (Glyph == null)
				Glyph = LoadDefaultImage();
			LargeGlyph = CommandBasedBarItemGlyphHelper.GetActualGlyph(LargeGlyph, LargeImages, LargeImageIndex);
			if (LargeGlyph == null)
				LargeGlyph = LoadDefaultLargeImage();
		}
		protected internal virtual void UpdateItemSuperTip() {
			if (SuperTip == null)
				UpdateSuperTipAndShortCut();
		}
		protected internal virtual void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected internal virtual ICommandUIState CreateCommandUIState(Command command) {
			return command.CreateDefaultCommandUIState();
		}
		protected virtual Image LoadDefaultImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.Image : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected virtual Image LoadDefaultLargeImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.LargeImage : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected virtual bool SuperTipHasDefaultContent() {
			return SuperToolTipHelper.SuperTipHasDefaultContent(SuperTip, GetDefaultSuperTipTitle(), GetDefaultSuperTipDescription(), SuperTipShortcut);
		}
		protected internal virtual void UpdateSuperTipAndShortCut() {
			if (ItemShortcut == CommandBasedBarButtonItem.BarShortcutAuto && ShouldAssignItemShortcut)
				RecreateItemShortcut();
			RecreateItemSuperToolTip();
		}
		protected virtual void RecreateItemShortcut() {
			Keys shortCutKeys = GetDefaultShortcutKeys();
			if (shortCutKeys != Keys.None)
				ItemShortcut = new BarShortcut(shortCutKeys);
			else
				ItemShortcut = BarShortcut.Empty;
		}
		protected virtual void RecreateItemSuperToolTip() {
			SuperTip = SuperToolTipHelper.CreateSuperToolTip(GetDefaultSuperTipTitle(), GetDefaultSuperTipDescription(), SuperTipShortcut);
			defaultSuperTip = true;
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if (Manager != null) {
				if (!Manager.IsLoading && !IsDisposing) {
					if (Edit != null) {
						Manager.RepositoryItems.Remove(Edit);
					}
					Edit = CreateEdit();
					if (Edit != null) {
						if (Manager.Site != null && Manager.Site.Container != null)
							Manager.Site.Container.Add(Edit);
						Manager.RepositoryItems.Add(Edit);
					}
				}
			}
		}
		protected internal override void OnEditChanging() {
			if (Edit == null)
				return;
			UnsubscribeEditEvents();
		}
		protected internal override void OnEditChanged() {
			if (Edit == null)
				return;
			SubscribeEditEvents();
		}
		protected virtual void UnsubscribeEditEvents() {
			Edit.Validating -= OnEditValidating;
		}
		protected virtual void SubscribeEditEvents() {
			Edit.Validating += OnEditValidating;
		}
		protected virtual void OnEditValidating(object sender, CancelEventArgs e) {
		}
		protected virtual bool IsEqual(ICommandBarItem item) {
			if (item == null)
				return false;
			if (item == this)
				return true;
			return item.GetType() == this.GetType();
		}
		protected virtual void SetupStatusBarLink(BarItemLink link) {
		}
		protected abstract RepositoryItem CreateEdit();
		#region ICommandBarItem Members
		void ICommandBarItem.UpdateCaption() {
			UpdateItemCaption();
			UpdateItemSuperTip();
		}
		void ICommandBarItem.UpdateVisibility() {
			UpdateItemVisibility();
		}
		void ICommandBarItem.UpdateImages() {
			UpdateItemGlyphs();
		}
		void ICommandBarItem.UpdateChecked() {
			UpdateItemChecked();
		}
		void ICommandBarItem.UpdateGroupIndex() {
			UpdateItemGroupIndex();
		}
		void ICommandBarItem.InvokeCommand() {
			InvokeCommand();
		}
		bool ICommandBarItem.IsEqual(ICommandBarItem item) {
			return this.IsEqual(item);
		}
		void ICommandBarItem.SetupStatusBarLink(BarItemLink link) {
			this.SetupStatusBarLink(link);
		}
		#endregion
	}
	#endregion
	#region ControlCommandBarEditItem<TControl, TCommandId, T> (abstract class)
	public abstract class ControlCommandBarEditItem<TControl, TCommandId, T> : CommandBasedBarEditItem<T>, IControlCommandBarItem<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		TControl control;
		#endregion
		protected ControlCommandBarEditItem()
			: base() {
		}
		protected ControlCommandBarEditItem(string caption)
			: base(caption) {
		}
		protected ControlCommandBarEditItem(BarManager manager)
			: base(manager) {
		}
		protected ControlCommandBarEditItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TControl Control {
			get { return control; }
			set {
				if (Object.ReferenceEquals(control, value))
					return;
				if (control != null)
					UnsubscribeControlEvents();
				if (DefaultSuperTip)
					SuperTip = null;
				control = value;
				if (control != null)
					SubscribeControlEvents();
				OnControlChanged();
				ItemsSetControl();
				GroupsSetControl();
			}
		}
		protected internal override bool ShouldSerializePaintStyle() {
			return PaintStyle != BarItemPaintStyle.Standard;
		}
		protected internal override void ResetPaintStyle() {
			PaintStyle = BarItemPaintStyle.Standard;
		}
		protected internal override bool ShouldAssignItemShortcut { get { return false; } }
		protected internal abstract TCommandId CommandId { get; }
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (control != null) {
						UnsubscribeControlEvents();
						control = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal virtual void SubscribeControlEvents() {
			Control.BeforeDispose += OnBeforeDisposeControl;
			Control.UpdateUI += OnControlUpdateUI;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			Control.BeforeDispose -= OnBeforeDisposeControl;
			Control.UpdateUI -= OnControlUpdateUI;
		}
		protected internal virtual void ItemsSetControl() {
		}
		protected internal virtual void GroupsSetControl() {
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.Control = null;
		}
		protected internal virtual void OnControlUpdateUI(object sender, EventArgs e) {
			this.UpdateItemVisibility();
		}
		protected internal override void UpdateItemGroupIndex() {
		}
		protected internal override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(CommandId);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected internal override ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<T> value = new DefaultValueBasedCommandUIState<T>();
			value.Value = (T)EditValue;
			return value;
		}
		protected virtual void OnControlChanged() {
		}
		protected internal virtual bool HandleException(Exception e) {
			if (e != null && Control != null)
				return Control.HandleException(e);
			else
				return false;
		}
		protected internal override void InvokeCommand() {
			try {
				if (Control == null)
					return;
				Control.CommitImeContent();
				base.InvokeCommand();
				UpdateItemVisibility();
				CloseAllRibbonPopups();
				Control.Focus();
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected internal override void UpdateItemVisibility() {
			base.UpdateItemVisibility();
			if (DesignMode && !Enabled && !ShouldDisableBarItem())
				Enabled = true;
			if (Enabled && ShouldDisableBarItem())
				Enabled = false;
		}
		bool ShouldDisableBarItem() {
			return Control == null;
		}
		void CloseAllRibbonPopups() {
			if (Ribbon != null && Ribbon.Manager != null)
				Ribbon.Manager.ClosePopupForms(false);
		}
		protected override bool IsEqual(ICommandBarItem item) {
			if (!base.IsEqual(item))
				return false;
			ControlCommandBarEditItem<TControl, TCommandId, T> otherItem = (ControlCommandBarEditItem<TControl, TCommandId, T>)item;
			return Object.Equals(this.CommandId, otherItem.CommandId);
		}
		protected override Keys GetDefaultShortcutKeys() {
			return Keys.None;
		}
		protected override string GetDefaultCaption() {
			return "";
		}
		protected override string GetDefaultSuperTipTitle() {
			if (!String.IsNullOrEmpty(Caption))
				return Caption;
			Command command = CreateCommand();
			return command != null ? command.MenuCaption : String.Empty;
		}
	}
	#endregion
	#region ControlCommandBarPopupGalleryEditItem<TControl, TCommandId, T> (abstract class)
	public abstract class ControlCommandBarPopupGalleryEditItem<TControl, TCommandId, T> : ControlCommandBarEditItem<TControl, TCommandId, T>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		RepositoryItemPopupGalleryEdit popupGalleryEdit;
		#endregion
		protected ControlCommandBarPopupGalleryEditItem()
			: base() {
		}
		protected ControlCommandBarPopupGalleryEditItem(string caption)
			: base(caption) {
		}
		protected ControlCommandBarPopupGalleryEditItem(BarManager manager)
			: base(manager) {
		}
		protected ControlCommandBarPopupGalleryEditItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RepositoryItemPopupGalleryEdit PopupGalleryEdit { get { return popupGalleryEdit; } }
		#endregion
		protected internal override void Initialize() {
			InitializePopupGalleryEdit();
			InitializeGallery(popupGalleryEdit.Gallery);
			SubscribeEvents();
			base.Initialize();
		}
		protected internal virtual void InitializePopupGalleryEdit() {
			this.popupGalleryEdit = new RepositoryItemPopupGalleryEdit();
			this.popupGalleryEdit.ShowButtons = false;
			this.popupGalleryEdit.ShowSizeGrip = false;
			this.popupGalleryEdit.ShowPopupCloseButton = false;
		}
		protected internal virtual void InitializeGallery(PopupGalleryEditGallery gallery) {
			gallery.AllowFilter = false;
			gallery.AutoFitColumns = false;
			gallery.ShowGroupCaption = false;
			gallery.ShowScrollBar = XtraBars.Ribbon.Gallery.ShowScrollBar.Hide;
			gallery.ShowItemText = true;
			gallery.StretchItems = true;
			gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.MiddleLeft;
			gallery.ItemImageLocation = Locations.Left;
			gallery.FixedImageSize = false;
		}
		protected internal virtual void SubscribeEvents() {
			ShowingEditor += OnShowingEditor;
		}
		void OnShowingEditor(object sender, ItemCancelEventArgs e) {
			ItemsUpdateCaption();
		}
		protected override RepositoryItem CreateEdit() {
			return popupGalleryEdit;
		}
		protected internal override void ItemsSetControl() {
			GalleryItemProcessorDelegate itemsSetControl = delegate(GalleryItem item) {
				ControlCommandGalleryItem<TControl, TCommandId> currentItem = item as ControlCommandGalleryItem<TControl, TCommandId>;
				if (currentItem != null)
					currentItem.Control = Control;
			};
			ForEachGalleryItems(itemsSetControl);
		}
		protected internal override void GroupsSetControl() {
			GalleryItemGroupProcessorDelegate groupsSetControl = delegate(GalleryItemGroup group) {
				ControlCommandGalleryItemGroup<TControl, TCommandId> currentGroup = group as ControlCommandGalleryItemGroup<TControl, TCommandId>;
				if (currentGroup != null)
					currentGroup.Control = Control;
			};
			ForEachGalleryGroups(groupsSetControl);
		}
		protected internal override void UpdateItemCaption() {
			base.UpdateItemCaption();
			GroupsUpdateCaption();
			ItemsUpdateCaption();
		}
		protected internal virtual void GroupsUpdateCaption() {
			GalleryItemGroupProcessorDelegate groupsUpdateCaption = delegate(GalleryItemGroup group) {
				ICommandBarItem currentGroup = group as ICommandBarItem;
				if (currentGroup != null)
					currentGroup.UpdateCaption();
			};
			ForEachGalleryGroups(groupsUpdateCaption);
		}
		protected internal virtual void ItemsUpdateCaption() {
			GalleryItemProcessorDelegate itemsUpdateCaption = delegate(GalleryItem item) {
				ICommandBarItem currentItem = item as ICommandBarItem;
				if (currentItem != null)
					currentItem.UpdateCaption();
			};
			ForEachGalleryItems(itemsUpdateCaption);
		}
		protected internal override void UpdateItemGlyphs() {
			base.UpdateItemGlyphs();
			ItemsUpdateImages();
		}
		protected internal virtual void ItemsUpdateImages() {
			GalleryItemProcessorDelegate itemsUpdateImages = delegate(GalleryItem item) {
				ICommandBarItem currentItem = item as ICommandBarItem;
				if (currentItem != null)
					currentItem.UpdateImages();
			};
			ForEachGalleryItems(itemsUpdateImages);
		}
		protected internal override void UpdateItemVisibility() {
			ItemsUpdateUIState();
			GroupsUpdateVisibility();
			base.UpdateItemVisibility();
		}
		protected internal virtual void GroupsUpdateVisibility() {
			GalleryItemGroupProcessorDelegate groupsUpdateVisibility = delegate(GalleryItemGroup group) {
				ICommandBarItem currentItem = group as ICommandBarItem;
				if (currentItem != null)
					currentItem.UpdateVisibility();
			};
			ForEachGalleryGroups(groupsUpdateVisibility);
		}
		protected internal override void UpdateItemChecked() {
			base.UpdateItemChecked();
			ItemsUpdateUIState();
		}
		protected internal virtual void ItemsUpdateUIState() {
			GalleryItemProcessorDelegate itemsUpdateUIStateCore = delegate(GalleryItem item) {
				CommandBasedGalleryItem currentItem = item as CommandBasedGalleryItem;
				if (currentItem != null)
					currentItem.UpdateUIStateCore();
			};
			ForEachGalleryItems(itemsUpdateUIStateCore);
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					DisposeItems(disposing);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal virtual void DisposeItems(bool disposing) {
			GalleryItemProcessorDelegate disposeItems = delegate(GalleryItem item) {
				CommandBasedGalleryItem currentItem = item as CommandBasedGalleryItem;
				if (currentItem != null)
					currentItem.Dispose(disposing);
			};
			ForEachGalleryItems(disposeItems);
		}
		protected internal virtual void ForEachGalleryGroups(GalleryItemGroupProcessorDelegate galleryGroupProcessor) {
			RepositoryItemPopupGalleryEdit galleryEdit = Edit as RepositoryItemPopupGalleryEdit;
			if (galleryEdit == null)
				return;
			GalleryItemGroupCollection groups = galleryEdit.Gallery.Groups;
			int groupsCount = groups.Count;
			for (int i = 0; i < groupsCount; i++) {
				galleryGroupProcessor(groups[i]);
			}
		}
		protected internal virtual void ForEachGalleryItems(GalleryItemProcessorDelegate galleryItemProcessor) {
			RepositoryItemPopupGalleryEdit galleryEdit = Edit as RepositoryItemPopupGalleryEdit;
			if (galleryEdit == null)
				return;
			galleryEdit.Gallery.BeginUpdate();
			try {
				GalleryItemGroupCollection groups = galleryEdit.Gallery.Groups;
				int groupsCount = groups.Count;
				for (int i = 0; i < groupsCount; i++) {
					GalleryItemCollection items = groups[i].Items;
					int itemsCount = items.Count;
					for (int j = 0; j < itemsCount; j++) {
						galleryItemProcessor(items[j]);
					}
				}
			}
			finally {
				galleryEdit.Gallery.EndUpdate();
			}
		}
		public delegate void GalleryItemProcessorDelegate(GalleryItem item);
		public delegate void GalleryItemGroupProcessorDelegate(GalleryItemGroup group);
	}
	#endregion
	#region CommandBasedBarSubItem (abstract class)
	public abstract class CommandBasedBarSubItem : BarSubItem, ICommandBarItem {
		#region Fields
		bool isNeedOpenArrow = true;
		#endregion
		bool defaultSuperTip;
		protected bool DefaultSuperTip { get { return defaultSuperTip; } }
		protected CommandBasedBarSubItem() {
			Initialize();
		}
		protected CommandBasedBarSubItem(string caption)
			: base() {
			Caption = caption;
			Initialize();
		}
		protected CommandBasedBarSubItem(BarManager manager)
			: base(manager, string.Empty) {
			Initialize();
		}
		protected CommandBasedBarSubItem(BarManager manager, string caption)
			: base(manager, caption) {
			Initialize();
		}
		#region Properties
		public override string Caption {
			get { return base.Caption; }
			set {
				base.Caption = value;
				if (defaultSuperTip && !SuperTipHasDefaultContent())
					UpdateSuperTipAndShortCut();
			}
		}
		internal override bool IsNeedOpenArrow { get { return isNeedOpenArrow; } }
		protected internal virtual BarShortcut SuperTipShortcut {
			get {
				if (!ShouldAssignItemShortcut && CommandBasedBarButtonItem.BarShortcutAuto == ItemShortcut)
					return new BarShortcut(GetDefaultShortcutKeys());
				else
					return ItemShortcut;
			}
		}
		public override string ShortcutKeyDisplayString { get { return GetShortcutKeyDisplayString(SuperTipShortcut); } set { base.ShortcutKeyDisplayString = value; } }
		protected internal virtual bool ShouldAssignItemShortcut { get { return true; } }
		public override BarShortcut ItemShortcut {
			get { return base.ItemShortcut; }
			set {
				base.ItemShortcut = value;
				if (defaultSuperTip && !SuperTipHasDefaultContent())
					UpdateSuperTipAndShortCut();
			}
		}
		protected internal override void ResetItemShortcut() {
			ItemShortcut = CommandBasedBarButtonItem.BarShortcutAuto;
		}
		protected internal override bool ShouldSerializeItemShortcut() {
			return !CommandBasedBarButtonItem.BarShortcutAuto.Equals(ItemShortcut) && !new BarShortcut(GetDefaultShortcutKeys()).Equals(ItemShortcut);
		}
		protected internal override bool ShouldSerializePaintStyle() {
			return PaintStyle != BarItemPaintStyle.CaptionGlyph;
		}
		protected internal override void ResetPaintStyle() {
			PaintStyle = BarItemPaintStyle.CaptionGlyph;
		}
		protected internal override bool ShouldSerializeRibbonStyle() {
			return RibbonStyle != DefaultRibbonStyle;
		}
		protected internal override void ResetRibbonStyle() {
			RibbonStyle = DefaultRibbonStyle;
		}
		protected internal bool CanUpdateVisibility { get { return Manager != null && !Manager.IsDesignMode && !Manager.IsLoading; } }
		protected internal virtual RibbonItemStyles DefaultRibbonStyle { get { return RibbonItemStyles.All; } }
		protected internal override bool ShouldSerializeGlyph() {
			if (GlyphComparer.IsDefaultGlyph(Glyph))
				return false;
			return base.ShouldSerializeGlyph();
		}
		protected internal override void ResetGlyph() {
			Glyph = LoadDefaultImage();
		}
		protected internal override bool ShouldSerializeLargeGlyph() {
			if (GlyphComparer.IsDefaultGlyph(LargeGlyph))
				return false;
			return base.ShouldSerializeLargeGlyph();
		}
		protected internal override void ResetLargeGlyph() {
			LargeGlyph = LoadDefaultLargeImage();
		}
		#endregion
		protected internal abstract Command CreateCommand();
		protected internal virtual void Initialize() {
			ItemShortcut = CommandBasedBarButtonItem.BarShortcutAuto;
			ResetRibbonStyle();
			ResetPaintStyle();
			UpdateItemGlyphs();
		}
		protected virtual string GetDefaultCaption() {
			Command command = CreateCommand();
			return command != null ? command.MenuCaption : String.Empty;
		}
		protected virtual string GetDefaultSuperTipTitle() {
			return Caption;
		}
		protected virtual string GetDefaultSuperTipDescription() {
			Command command = CreateCommand();
			return command != null ? command.Description : String.Empty;
		}
		protected virtual Keys GetDefaultShortcutKeys() {
			return Keys.None;
		}
		protected internal override bool ShouldSerializeCaption() {
			return GetDefaultCaption() != Caption;
		}
		protected internal override void ResetCaption() {
			Caption = GetDefaultCaption();
		}
		protected override bool ShouldSerializeSuperTip() {
			return !SuperTipHasDefaultContent();
		}
		public override void ResetSuperTip() {
			UpdateSuperTipAndShortCut();
		}
		protected internal virtual void UpdateItemCaption() {
			if (String.IsNullOrEmpty(Caption))
				Caption = GetDefaultCaption();
		}
		protected internal virtual void UpdateItemGroupIndex() {
		}
		protected internal virtual void UpdateItemChecked() {
			if (!CanUpdateVisibility)
				return;
			BeginUpdate();
			try {
				Command command = CreateCommand();
				if (command != null) {
					ICommandUIState state = CreateCommandUIState(command);
					command.UpdateUIState(state);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void UpdateItemVisibility() {
			if (!CanUpdateVisibility)
				return;
			BeginUpdate();
			try {
				Command command = CreateCommand();
				if (command != null) {
					ICommandUIState state = CreateCommandUIState(command);
					command.UpdateUIState(state);
					ChangeIsNeedOpenArrow(command, state);
				}
			}
			finally {
				EndUpdate();
			}
		}
		void ChangeIsNeedOpenArrow(Command command, ICommandUIState state) {
			this.isNeedOpenArrow = GetIsNeedOpenArrow(command, state);
		}
		protected internal virtual bool GetIsNeedOpenArrow(Command command, ICommandUIState state) {
			return true;
		}
		protected virtual ICommandUIState CreateCommandUIState(Command command) {
			return new BarSubItemUIState(this);
		}
		protected internal virtual void UpdateItemGlyphs() {
			Glyph = CommandBasedBarItemGlyphHelper.GetActualGlyph(Glyph, Images, ImageIndex);
			if (Glyph == null)
				Glyph = LoadDefaultImage();
			LargeGlyph = CommandBasedBarItemGlyphHelper.GetActualGlyph(LargeGlyph, LargeImages, LargeImageIndex);
			if (LargeGlyph == null)
				LargeGlyph = LoadDefaultLargeImage();
		}
		protected internal virtual void UpdateItemSuperTip() {
			if (SuperTip == null)
				UpdateSuperTipAndShortCut();
		}
		protected internal virtual void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null && command.CanExecute())
				command.Execute();
		}
		protected internal override void OnPopup() {
			base.OnPopup();
		}
		protected internal override void OnCloseUp() {
			base.OnCloseUp();
		}
		protected virtual Image LoadDefaultImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.Image : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected virtual Image LoadDefaultLargeImage() {
			Command command = CreateCommand();
			Image image = command != null ? command.LargeImage : null;
			if (image != null)
				image.Tag = BarCollectorBase.DefaultImageTag;
			return image;
		}
		protected virtual bool SuperTipHasDefaultContent() {
			return SuperToolTipHelper.SuperTipHasDefaultContent(SuperTip, GetDefaultSuperTipTitle(), GetDefaultSuperTipDescription(), SuperTipShortcut);
		}
		protected internal virtual void UpdateSuperTipAndShortCut() {
			if (ItemShortcut == CommandBasedBarButtonItem.BarShortcutAuto && ShouldAssignItemShortcut)
				RecreateItemShortcut();
			RecreateItemSuperToolTip();
		}
		protected virtual void RecreateItemShortcut() {
			Keys shortCutKeys = GetDefaultShortcutKeys();
			if (shortCutKeys != Keys.None)
				ItemShortcut = new BarShortcut(shortCutKeys);
			else
				ItemShortcut = BarShortcut.Empty;
		}
		protected virtual void RecreateItemSuperToolTip() {
			SuperTip = SuperToolTipHelper.CreateSuperToolTip(GetDefaultSuperTipTitle(), GetDefaultSuperTipDescription(), SuperTipShortcut);
			defaultSuperTip = true;
		}
		protected virtual bool IsEqual(ICommandBarItem item) {
			if (item == null)
				return false;
			if (item == this)
				return true;
			return item.GetType() == this.GetType();
		}
		protected virtual void SetupStatusBarLink(BarItemLink link) {
		}
		#region ICommandBarItem Members
		void ICommandBarItem.UpdateCaption() {
			UpdateItemCaption();
			UpdateItemSuperTip();
		}
		void ICommandBarItem.UpdateVisibility() {
			UpdateItemVisibility();
		}
		void ICommandBarItem.UpdateImages() {
			UpdateItemGlyphs();
		}
		void ICommandBarItem.UpdateChecked() {
			UpdateItemChecked();
		}
		void ICommandBarItem.UpdateGroupIndex() {
			UpdateItemGroupIndex();
		}
		void ICommandBarItem.InvokeCommand() {
			InvokeCommand();
		}
		bool ICommandBarItem.IsEqual(ICommandBarItem item) {
			return this.IsEqual(item);
		}
		void ICommandBarItem.SetupStatusBarLink(BarItemLink link) {
			this.SetupStatusBarLink(link);
		}
		#endregion
	}
	#endregion
	#region ControlCommandBarSubItem<TControl, TCommandId> (abstract class)
	public abstract class ControlCommandBarSubItem<TControl, TCommandId> : CommandBasedBarSubItem, IControlCommandBarItem<TControl, TCommandId>, IBarSubItem
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		TControl control;
		List<BarItem> items;
		#endregion
		protected ControlCommandBarSubItem()
			: base() {
			Init();
		}
		protected ControlCommandBarSubItem(string caption)
			: base(caption) {
			Init();
		}
		protected ControlCommandBarSubItem(BarManager manager)
			: base(manager, string.Empty) {
			Init();
		}
		protected ControlCommandBarSubItem(BarManager manager, string caption)
			: base(manager, caption) {
			Init();
		}
		#region Properties
		#region Control
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TControl Control {
			get { return control; }
			set {
				if (Object.ReferenceEquals(control, value))
					return;
				if (control != null)
					UnsubscribeControlEvents();
				if (DefaultSuperTip)
					SuperTip = null;
				control = value;
				if (control != null)
					SubscribeControlEvents();
			}
		}
		#endregion
		#region PaintStyle
		protected internal override bool ShouldSerializePaintStyle() {
			return PaintStyle != BarItemPaintStyle.Standard;
		}
		protected internal override void ResetPaintStyle() {
			PaintStyle = BarItemPaintStyle.Standard;
		}
		#endregion
		protected internal override bool ShouldAssignItemShortcut { get { return false; } }
		protected internal abstract TCommandId CommandId { get; }
		protected virtual bool ShouldSetControlFocusAfterInvokeCommand { get { return false; } }
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (control != null) {
						UnsubscribeControlEvents();
						control = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal virtual void SubscribeControlEvents() {
			Control.BeforeDispose += OnBeforeDisposeControl;
			Control.UpdateUI += OnControlUpdateUI;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			Control.BeforeDispose -= OnBeforeDisposeControl;
			Control.UpdateUI -= OnControlUpdateUI;
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.Control = null;
		}
		protected internal virtual void OnControlUpdateUI(object sender, EventArgs e) {
			this.UpdateItemVisibility();
		}
		protected internal override void UpdateItemGroupIndex() {
		}
		protected internal override Command CreateCommand() {
			if (Control == null)
				return null;
			Command command = Control.CreateCommand(CommandId);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected override Keys GetDefaultShortcutKeys() {
			if (Control == null)
				return Keys.None;
			CommandBasedKeyboardHandler<TCommandId> keyboardHandler = Control.KeyboardHandler;
			if (keyboardHandler == null)
				return Keys.None;
			return keyboardHandler.GetKeys(CommandId);
		}
		protected internal virtual bool HandleException(Exception e) {
			if (e != null && Control != null)
				return Control.HandleException(e);
			else
				return false;
		}
		protected internal override void InvokeCommand() {
			try {
				Control.CommitImeContent();
				base.InvokeCommand();
				UpdateItemVisibility();
				if (!IsLockUpdate && ShouldSetControlFocusAfterInvokeCommand)
					Control.Focus();
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected internal override void UpdateItemVisibility() {
			base.UpdateItemVisibility();
			if (DesignMode && !Enabled && !ShouldDisableBarItem())
				Enabled = true;
			if (Enabled && ShouldDisableBarItem())
				Enabled = false;
		}
		bool ShouldDisableBarItem() {
			return Control == null;
		}
		protected internal void Init() {
			this.items = new List<BarItem>();
		}
		protected override bool IsEqual(ICommandBarItem item) {
			if (!base.IsEqual(item))
				return false;
			ControlCommandBarSubItem<TControl, TCommandId> otherItem = (ControlCommandBarSubItem<TControl, TCommandId>)item;
			return Object.Equals(this.CommandId, otherItem.CommandId);
		}
		#region IBarSubItem Members
		public void AddBarItem(BarItem barItem) {
			items.Add(barItem);
		}
		public List<BarItem> GetSubItems() {
			return items;
		}
		#endregion
	}
	#endregion
	#region CommandBarController (abstract class)
	public abstract class CommandBarController : Component, ISupportInitialize {
		readonly List<BarItem> barItemCollection;
		bool isInitializationStarted;
		protected CommandBarController() {
			barItemCollection = new List<BarItem>();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public List<BarItem> BarItems { get { return barItemCollection; } }
		protected bool IsInitializationStarted { get { return isInitializationStarted; } }
		protected virtual void UpdateController() {
			UpdateBarItems();
			UpdateBarItemContainers();
		}
		void UpdateBarItemContainers() {
			RibbonBarCollector ribbonCollector = new RibbonBarCollector(BarItems);
			BarCollector barCollector = new BarCollector(BarItems);
			List<BarManager> managerCollection = GetManagerCollection();
			ribbonCollector.CollectContainers(managerCollection);
			barCollector.CollectContainers(managerCollection);
			UpdateRibbonBarItemContainers(ribbonCollector.UsedRibbonPageGroups, ribbonCollector.UsedRibbonPages, ribbonCollector.UsedRibbonPageCategories);
			UpdateBarItemContainers(barCollector.UsedBars);
		}
		void UpdateBarItemContainers(List<Bar> barCollection) {
			int count = barCollection.Count;
			for (int i = 0; i < count; i++)
				UpdateBar(barCollection[i]);
		}
		protected virtual void UpdateRibbonBarItemContainers(List<RibbonPageGroup> pageGroupCollection, List<RibbonPage> pageCollection, List<RibbonPageCategory> categoryCollection) {
			int count = categoryCollection.Count;
			for (int i = 0; i < count; i++)
				UpdateRibbonPageCategory(categoryCollection[i]);
			count = pageCollection.Count;
			for (int i = 0; i < count; i++)
				UpdateRibbonPage(pageCollection[i]);
			count = pageGroupCollection.Count;
			for (int i = 0; i < count; i++)
				UpdateRibbonPageGroup(pageGroupCollection[i]);
		}
		protected virtual void UpdateRibbonPageCategory(RibbonPageCategory category) {
			CommandBasedRibbonPageCategory commandBasedPageCategory = category as CommandBasedRibbonPageCategory;
			if (commandBasedPageCategory != null)
				commandBasedPageCategory.UpdateText();
		}
		protected virtual void UpdateRibbonPage(RibbonPage page) {
			CommandBasedRibbonPage commandBasedPage = page as CommandBasedRibbonPage;
			if (commandBasedPage != null)
				commandBasedPage.UpdateText();
		}
		protected virtual void UpdateRibbonPageGroup(RibbonPageGroup pageGroup) {
			CommandBasedRibbonPageGroup commandBasedPageGroup = pageGroup as CommandBasedRibbonPageGroup;
			if (commandBasedPageGroup != null) {
				commandBasedPageGroup.UpdateText();
				commandBasedPageGroup.UpdateSuperTip();
				commandBasedPageGroup.UpdateCaptionButtonVisibility();
			}
		}
		List<BarManager> GetManagerCollection() {
			List<BarManager> managerCollection = new List<BarManager>();
			int count = BarItems.Count;
			for (int i = 0; i < count; i++) {
				BarManager manager = BarItems[i].Manager;
				if (manager != null && !managerCollection.Contains(manager))
					managerCollection.Add(manager);
			}
			return managerCollection;
		}
		protected virtual void UpdateBar(Bar bar) {
			CommandBasedBar commandBar = bar as CommandBasedBar;
			if (commandBar == null)
				return;
			commandBar.UpdateName();
			commandBar.UpdateText();
		}
		protected void UpdateBarItems() {
			int count = BarItems.Count;
			for (int i = 0; i < count; i++) {
				BarItem item = BarItems[i];
				UpdateBarItem(item);
			}
		}
		protected virtual void UpdateBarItem(BarItem item) {
			ICommandBarItem barItem = item as ICommandBarItem;
			if (barItem == null)
				return;
			item.BeginUpdate();
			try {
				barItem.UpdateCaption();
				barItem.UpdateImages();
				barItem.UpdateVisibility();
				barItem.UpdateGroupIndex();
				barItem.UpdateChecked();
			}
			finally {
				item.EndUpdate();
			}
		}
		protected virtual void SubscribeBarItemsEvents() {
			if (DesignMode)
				return;
			int count = BarItems.Count;
			for (int i = 0; i < count; i++) {
				BarItem item = BarItems[i];
				SubscribeBarItemEvents(item);
			}
		}
		protected virtual void SubscribeBarItemEvents(BarItem btn) {
			btn.Disposed += new EventHandler(OnBarItemDisposed);
			BarCheckItem checkItem = btn as BarCheckItem;
			if (checkItem != null)
				checkItem.CheckedChanged += new ItemClickEventHandler(HandleCheckBarItemCheckedChanged);
			BarButtonItem btnItem = btn as BarButtonItem;
			if (btnItem != null) {
				btnItem.ItemClick += new ItemClickEventHandler(HandleButtonBarItemClick);
				GalleryDropDown gdd = btnItem.DropDownControl as GalleryDropDown;
				if (gdd != null)
					gdd.GalleryItemClick += new GalleryItemClickEventHandler(HandleButtonGalleryItemClick);
			}
			BarEditItem editItem = btn as BarEditItem;
			if (editItem != null) {
				editItem.EditValueChanged += new EventHandler(HandleEditValueChanged);
				RepositoryItemPopupGalleryEdit edit = editItem.Edit as RepositoryItemPopupGalleryEdit;
				if (edit != null)
					edit.Gallery.ItemClick += HandleGalleryPopupEditItemClick;
			}
			BarSubItem subItem = btn as BarSubItem;
			if (subItem != null)
				subItem.ItemClick += new ItemClickEventHandler(HandleBarSubItemClick);
			RibbonGalleryBarItem rgbItem = btn as RibbonGalleryBarItem;
			if (rgbItem != null) {
				rgbItem.GalleryItemClick += new GalleryItemClickEventHandler(HandleGalleryBarItemClick);
				GalleryDropDown galleryDropDown = rgbItem.GalleryDropDown;
				if (galleryDropDown != null)
					galleryDropDown.GalleryItemClick += new GalleryItemClickEventHandler(HandleGalleryDropDownBarItemClick);
			}
		}
		protected virtual void HandleBarSubItemClick(object sender, ItemClickEventArgs e) {
			InvokeItemCommand(e.Item as ICommandBarItem);
		}
		protected virtual void HandleEditValueChanged(object sender, EventArgs e) {
			InvokeItemCommand(sender as ICommandBarItem);
		}
		protected virtual void HandleButtonBarItemClick(object sender, ItemClickEventArgs e) {
			BarItem item = e.Item;
			if (item is RibbonGalleryBarItem) 
				return;
			InvokeItemCommand(e.Item as ICommandBarItem);
		}
		protected virtual void HandleButtonGalleryItemClick(object sender, GalleryItemClickEventArgs e) {
			InDropDownGallery dropDown = sender as InDropDownGallery;
			if (dropDown == null) {
				InvokeItemCommand(e.Item as ICommandBarItem);
				return;
			}
			BarItems items = dropDown.GalleryDropDown.Manager.Items;
			ISupportsSelectedGalleryItem buttonGalleryDropDown = FindButtonGalleryDropDown(items, dropDown.GalleryDropDown);
			if (buttonGalleryDropDown != null)
				buttonGalleryDropDown.InvokeSelectedGalleryItemCommand(e.Item);
			else
				InvokeItemCommand(e.Item as ICommandBarItem);
		}
		ISupportsSelectedGalleryItem FindButtonGalleryDropDown(BarItems items, GalleryDropDown senderDropDown) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				BarButtonItem buttonItem = items[i] as BarButtonItem;
				if (buttonItem == null)
					continue;
				GalleryDropDown galleryDropDown = buttonItem.DropDownControl as GalleryDropDown;
				if (galleryDropDown == null)
					continue;
				if (galleryDropDown == senderDropDown)
					return buttonItem as ISupportsSelectedGalleryItem;
			}
			return null;
		}
		protected virtual void HandleGalleryBarItemClick(object sender, GalleryItemClickEventArgs e) {
			CommandBasedGalleryBarItem item = ((InRibbonGallery)sender).OwnerItem as CommandBasedGalleryBarItem;
			if (item != null) {
				GalleryItem galleryItem = e.Item;
				GalleryItem originGalleryItem = galleryItem.OriginItem;
				item.SelectedItem = originGalleryItem == null ? galleryItem : originGalleryItem;
			}
			InvokeItemCommand(item as ICommandBarItem);
		}
		protected virtual void HandleGalleryDropDownBarItemClick(object sender, GalleryItemClickEventArgs e) {
			CommandBasedGalleryBarItem item = ((InDropDownGallery)sender).GalleryDropDown.OwnerGalleryLink.Item as CommandBasedGalleryBarItem;
			if (item != null)
				item.SelectedItem = e.Item;
			InvokeItemCommand(item as ICommandBarItem);
		}
		protected virtual void HandleCheckBarItemCheckedChanged(object sender, ItemClickEventArgs e) {
			InvokeItemCommand(e.Item as ICommandBarItem);
		}
		protected virtual void HandleGalleryPopupEditItemClick(object sender, GalleryItemClickEventArgs e) {
			InvokeItemCommand(e.Item.OriginItem.OriginItem as ICommandBarItem);
		}
		void OnBarItemDisposed(object sender, EventArgs e) {
			BarItem item = sender as BarItem;
			if (item == null)
				return;
			BarItems.Remove(item);
		}
		protected virtual void InvokeItemCommand(ICommandBarItem item) {
			if (item != null)
				item.InvokeCommand();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing) {
				RibbonBarCollector ribbonCollector = new RibbonBarCollector(BarItems);
				BarCollector barCollector = new BarCollector(BarItems);
				List<BarManager> managerCollection = GetManagerCollection();
				ribbonCollector.CollectContainers(managerCollection);
				barCollector.CollectContainers(managerCollection);
				DeleteBarItems();
				DeleteBarItemGroups(ribbonCollector.UsedRibbonPageGroups);
				DeleteUnusedRibbonBarItemContainers(ribbonCollector.UsedRibbonPageGroups, ribbonCollector.UsedRibbonPages, ribbonCollector.UsedRibbonPageCategories);
				DeleteUnusedBarItemContainers(barCollector.UsedBars);
			}
		}
		void DeleteBarItemGroups(List<RibbonPageGroup> pageGroups) {
			List<BarButtonGroup> groups = new List<BarButtonGroup>();
			int count = pageGroups.Count;
			for (int i = 0; i < count; i++)
				CollectBarItemGroups(pageGroups[i], groups);
			count = groups.Count - 1;
			for (int i = count; i >= 0; i--) {
				BarButtonGroup item = groups[i];
				groups.Remove(item);
				item.Dispose();
			}
		}
		void CollectBarItemGroups(RibbonPageGroup pageGroup, List<BarButtonGroup> groups) {
			RibbonPageGroupItemLinkCollection links = pageGroup.ItemLinks;
			int count = links.Count;
			for (int i = 0; i < count; i++) {
				BarButtonGroup item = links[i].Item as BarButtonGroup;
				if (item != null)
					groups.Add(item);
			}
		}
		void DeleteUnusedBarItemContainers(List<Bar> barCollection) {
			int count = barCollection.Count;
			for (int i = 0; i < count; i++) {
				CommandBasedBar bar = barCollection[i] as CommandBasedBar;
				if (bar == null)
					continue;
				if (bar.ItemLinks.Count <= 0)
					bar.Dispose();
			}
		}
		void DeleteUnusedRibbonBarItemContainers(List<RibbonPageGroup> pageGroupCollection, List<RibbonPage> pageCollection, List<RibbonPageCategory> categoryCollection) {
			DeleteEmptyPageGroups(pageGroupCollection);
			DeleteEmptyPages(pageCollection);
			DeleteEmptyPageCategories(categoryCollection);
		}
		void DeleteEmptyPageGroups(List<RibbonPageGroup> pageGroupCollection) {
			int count = pageGroupCollection.Count;
			for (int i = 0; i < count; i++) {
				CommandBasedRibbonPageGroup pageGroup = pageGroupCollection[i] as CommandBasedRibbonPageGroup;
				if (pageGroup == null)
					continue;
				if (pageGroup.ItemLinks.Count <= 0)
					pageGroup.Dispose();
			}
		}
		void DeleteEmptyPages(List<RibbonPage> pageCollection) {
			int count = pageCollection.Count;
			for (int i = 0; i < count; i++) {
				CommandBasedRibbonPage page = pageCollection[i] as CommandBasedRibbonPage;
				if (page == null)
					continue;
				if (page.Groups.Count <= 0)
					page.Dispose();
			}
		}
		void DeleteEmptyPageCategories(List<RibbonPageCategory> categoryCollection) {
			int count = categoryCollection.Count;
			for (int i = 0; i < count; i++) {
				CommandBasedRibbonPageCategory category = categoryCollection[i] as CommandBasedRibbonPageCategory;
				if (category == null)
					continue;
				if (category.Pages.Count <= 0)
					category.Dispose();
			}
		}
		void DeleteBarItems() {
			int count = BarItems.Count;
			for (int i = count - 1; i >= 0; i--) {
				BarItem item = BarItems[i];
				BarItems.Remove(item);
				item.Dispose();
			}
		}
		protected virtual void Initialize() { 
		}
		#region ISupportInitialize Members
		void ISupportInitialize.BeginInit() {
			this.isInitializationStarted = true;
			Initialize();
		}
		void ISupportInitialize.EndInit() {
			UpdateBarItemContainers();
			UpdateBarItems();
			SubscribeBarItemsEvents();
			this.isInitializationStarted = false;
		}
		#endregion
	}
	#endregion
	#region ControlCommandBarControllerBase<TControl, TCommandId> (abstract class)
	public abstract class ControlCommandBarControllerBase<TControl, TCommandId> : CommandBarController
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		TControl control;
		[DefaultValue(null)]
		public TControl Control {
			get { return control; }
			set {
				if (control == value)
					return;
				SetControlCore(value);
			}
		}
		[DefaultValue(null)]
		protected virtual TControl ActualControl { get { return Control; } }
		protected virtual void SetControlCore(TControl value) {
			UnsubscribeControlEvents();
			control = value;
			SubscribeControlEvents();
			if (IsInitializationStarted)
				return;
			UpdateController();
		}
		protected override void UpdateBarItem(BarItem item) {
			IControlCommandBarItem<TControl, TCommandId> barItem = item as IControlCommandBarItem<TControl, TCommandId>;
			if (barItem != null)
				barItem.Control = ActualControl;
			base.UpdateBarItem(item);
		}
		protected override void UpdateBar(Bar bar) {
			ControlCommandBasedBar<TControl, TCommandId> barItem = bar as ControlCommandBasedBar<TControl, TCommandId>;
			if (barItem != null)
				barItem.Control = ActualControl;
			base.UpdateBar(bar);
		}
		protected override void UpdateRibbonPageGroup(RibbonPageGroup pageGroup) {
			ControlCommandBasedRibbonPageGroup<TControl, TCommandId> ribbonPageGroup = pageGroup as ControlCommandBasedRibbonPageGroup<TControl, TCommandId>;
			if (ribbonPageGroup != null)
				ribbonPageGroup.Control = ActualControl;
			base.UpdateRibbonPageGroup(pageGroup);
		}
		protected override void UpdateRibbonPageCategory(RibbonPageCategory category) {
			ControlCommandBasedRibbonPageCategory<TControl, TCommandId> pageCategory = category as ControlCommandBasedRibbonPageCategory<TControl, TCommandId>;
			if (pageCategory != null)
				pageCategory.Control = ActualControl;
			base.UpdateRibbonPageCategory(category);
		}
		protected internal virtual void SubscribeControlEvents() {
			if (Control != null)
				Control.BeforeDispose += OnControlBeforeDispose;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			if (Control != null)
				Control.BeforeDispose -= OnControlBeforeDispose;
		}
		protected internal void OnControlBeforeDispose(object sender, EventArgs e) {
			Control = null;
		}
	}
	#endregion
	#region GlyphComparer
	public class GlyphComparer {
		public static bool IsDefaultGlyph(Image glyph) {
			return glyph != null && (Object.Equals(glyph.Tag, BarCollectorBase.DefaultImageTag) || Object.Equals(glyph.Tag, BarCollectorBase.ImageFromListTag));
		}
	}
	#endregion
}
namespace DevExpress.XtraBars.Commands.Ribbon {
	#region ShowCaptionButtonMode
	public enum ShowCaptionButtonMode { Show, Hide, Auto }
	#endregion
	#region RibbonCommandBarComponentBase (abstract class)
	public abstract class RibbonCommandBarComponentBase : CommandBasedBarComponentBase {
		#region Fields
		RibbonControl ribbonControl;
		string targetRibbonPageName;
		#endregion
		protected RibbonCommandBarComponentBase()
			: base() {
		}
		protected RibbonCommandBarComponentBase(IContainer container)
			: base(container) {
			container.Add(this);
		}
		#region Properties
		[DefaultValue(null)]
		public RibbonControl RibbonControl {
			get { return ribbonControl; }
			set {
				if (Object.ReferenceEquals(ribbonControl, value))
					return;
				SetRibbonControl(value);
			}
		}
		[DefaultValue("")]
		public string TargetRibbonPageName {
			get { return targetRibbonPageName; }
			set {
				if (targetRibbonPageName == value)
					return;
				SetTargetRibbonPageName(value);
			}
		}
		protected virtual Type SupportedRibbonPageCategoryType { get { return typeof(int); } } 
		protected abstract Type SupportedRibbonPageType { get; }
		protected abstract Type SupportedRibbonPageGroupType { get; }
		#endregion
		protected override BarCreationContextBase CreateBarCreationContext() {
			return new RibbonBarCreationContext();
		}
		protected internal virtual void SetTargetRibbonPageName(string targetName) {
			SetRelatedVisualControlCore(delegate() { this.targetRibbonPageName = targetName; });
		}
		protected internal virtual void SetRibbonControl(RibbonControl value) {
			SetRelatedVisualControlCore(delegate() { this.ribbonControl = value; });
		}
		protected override bool CanCreateVisualItems() {
			return RibbonControl != null;
		}
		protected internal override Component GetBarItemContainer() {
			CommandBasedRibbonPageGroup pageGroup = GetRibbonPageGroup();
			if (pageGroup.Page == null) {
				RibbonPage page = null;
				if (!string.IsNullOrEmpty(TargetRibbonPageName))
					page = FindRibbonPageByName(TargetRibbonPageName);
				if (page == null)
					page = GetRibbonPage();
				CommandBasedRibbonPageCategory category = GetRibbonPageCategory();
				if (category != null) {
					ribbonControl.PageCategories.Add(category);
					category.Pages.Add(page);
				}
				else {
					ribbonControl.Pages.Add(page);
				}
				page.Groups.Add(pageGroup);
			}
			return pageGroup;
		}
		protected override void FillBarItemContainer(Component parent, List<BarItem> items) {
			CommandBasedRibbonPageGroup pageGroup = (CommandBasedRibbonPageGroup)parent;
			foreach (BarItem btn in items) {
				ribbonControl.Items.Add(btn);
				pageGroup.ItemLinks.Add(btn);
			}
		}
		protected internal override void DeleteVisualItems() {
			if (RibbonControl == null)
				return;
			base.DeleteVisualItems();
			DeletePageGroup();
			DeletePage();
			DeletePageCategory();
		}
		protected internal virtual CommandBasedRibbonPageGroup GetRibbonPageGroup() {
			CommandBasedRibbonPageGroup pageGroup = FindCommandRibbonPageGroup();
			if (pageGroup == null) {
				pageGroup = CreateRibbonPageGroupInstance();
				if (DesignMode && Container != null)
					Container.Add(pageGroup);
			}
			return pageGroup;
		}
		protected abstract CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance();
		protected internal virtual CommandBasedRibbonPage GetRibbonPage() {
			CommandBasedRibbonPage page = FindCommandRibbonPage();
			if (page == null) {
				page = CreateRibbonPageInstance();
				if (DesignMode && Container != null)
					Container.Add(page);
			}
			return page;
		}
		protected abstract CommandBasedRibbonPage CreateRibbonPageInstance();
		protected internal virtual CommandBasedRibbonPageCategory GetRibbonPageCategory() {
			CommandBasedRibbonPageCategory category = FindCommandRibbonPageCategory();
			if (category == null) {
				category = CreateRibbonPageCategoryInstance();
				if (category != null) {
					if (DesignMode && Container != null)
						Container.Add(category);
				}
			}
			return category;
		}
		protected virtual CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return null; 
		}
		protected internal virtual void DeletePageCategory() {
			CommandBasedRibbonPageCategory category = FindCommandRibbonPageCategory();
			if (category != null && category.Pages.Count <= 0) {
				if (DesignMode && Container != null)
					Container.Remove(category);
				category.Dispose();
			}
		}
		protected internal virtual void DeletePage() {
			CommandBasedRibbonPage page = FindCommandRibbonPage();
			if (page != null && page.Groups.Count <= 0) {
				if (DesignMode && Container != null)
					Container.Remove(page);
				page.Dispose();
			}
		}
		protected internal virtual void DeletePageGroup() {
			CommandBasedRibbonPageGroup pageGroup = FindCommandRibbonPageGroup();
			if (pageGroup != null && pageGroup.ItemLinks.Count <= 0) {
				pageGroup.DeleteToolbarContentButton();
				if (DesignMode && Container != null)
					Container.Remove(pageGroup);
				pageGroup.Dispose();
			}
		}
		protected internal virtual CommandBasedRibbonPageCategory FindCommandRibbonPageCategory() {
			if (RibbonControl != null) {
				foreach (RibbonPageCategory category in RibbonControl.PageCategories)
					if (SupportedRibbonPageCategoryType.IsAssignableFrom(category.GetType()))
						return (CommandBasedRibbonPageCategory)category;
			}
			return null;
		}
		protected internal virtual CommandBasedRibbonPage FindCommandRibbonPage() {
			if (RibbonControl != null) {
				foreach (RibbonPage page in RibbonControl.Pages)
					if (SupportedRibbonPageType.IsAssignableFrom(page.GetType()))
						return (CommandBasedRibbonPage)page;
			}
			return null;
		}
		protected internal virtual RibbonPage FindRibbonPageByName(string name) {
			if (RibbonControl != null) {
				foreach (RibbonPage page in RibbonControl.Pages)
					if (page.Name == name)
						return page;
			}
			return null;
		}
		protected internal virtual CommandBasedRibbonPageGroup FindCommandRibbonPageGroup() {
			if (RibbonControl == null)
				return null;
			CommandBasedRibbonPageGroup result = FindCommandRibbonPageGroup(RibbonControl.Pages);
			if (result != null)
				return result;
			int categoryCount = RibbonControl.PageCategories.Count;
			for (int i = 0; i < categoryCount; i++) {
				result = FindCommandRibbonPageGroup(RibbonControl.PageCategories[i].Pages);
				if (result != null)
					return result;
			}
			return null;
		}
		protected internal virtual CommandBasedRibbonPageGroup FindCommandRibbonPageGroup(RibbonPageCollection pages) {
			int pagesCount = pages.Count;
			for (int i = 0; i < pagesCount; i++) {
				CommandBasedRibbonPageGroup result = FindCommandRibbonPageGroupFromPage(pages[i]);
				if (result != null)
					return result;
			}
			return null;
		}
		protected internal virtual CommandBasedRibbonPageGroup FindCommandRibbonPageGroupFromPage(RibbonPage page) {
			int groupsCount = page.Groups.Count;
			for (int i = 0; i < groupsCount; i++) {
				RibbonPageGroup group = page.Groups[i];
				if (SupportedRibbonPageGroupType.IsAssignableFrom(group.GetType()))
					return (CommandBasedRibbonPageGroup)group;
			}
			return null;
		}
		protected internal override BarItems GetSourceBarItems() {
			return RibbonControl != null ? RibbonControl.Items : null;
		}
		protected internal override void UpdateBarItemContainer() {
			CommandBasedRibbonPageGroup pageGroup = FindCommandRibbonPageGroup();
			UpdatePageGroup(pageGroup);
			if (pageGroup != null)
				UpdatePage(pageGroup.Page as CommandBasedRibbonPage);
		}
		protected internal virtual void UpdatePage(CommandBasedRibbonPage page) {
			if (page != null)
				page.UpdateText();
		}
		protected internal virtual void UpdatePageGroup(CommandBasedRibbonPageGroup pageGroup) {
			if (pageGroup != null)
				pageGroup.UpdateText();
		}
		protected internal override void SubscribeRelatedVisualControlEvents() {
			if (ribbonControl != null)
				ribbonControl.Disposed += ribbonControl_Disposed;
		}
		protected internal override void UnsubscribeRelatedVisualControlEvents() {
			if (ribbonControl != null)
				ribbonControl.Disposed -= ribbonControl_Disposed;
		}
		protected internal virtual void ribbonControl_Disposed(object sender, EventArgs e) {
			RibbonControl = null;
		}
	}
	#endregion
	#region ControlRibbonCommandBarComponent<TControl> (abstract class)
	public abstract class ControlRibbonCommandBarComponent<TControl, TCommandId> : RibbonCommandBarComponentBase, IBarItemContainer
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		TControl control;
		#endregion
		protected ControlRibbonCommandBarComponent()
			: base() {
		}
		protected ControlRibbonCommandBarComponent(IContainer container)
			: base(container) {
		}
		#region Properties
		[DefaultValue(null)]
		public TControl Control {
			get { return control; }
			set {
				if (Object.ReferenceEquals(control, value))
					return;
				SetControl(value);
			}
		}
		#endregion
		protected override void DetachItemProviderControl() {
			this.control = null;
		}
		protected internal virtual void SetControl(TControl value) {
			if (Initialization) {
				SetControlCore(value);
				return;
			}
			if (Control != null) {
				UnsubscribeControlEvents();
				DeleteVisualItems();
			}
			SetControlCore(value);
			if (Control != null) {
				CreateNewVisualItems();
				SubscribeControlEvents();
			}
		}
		protected internal virtual void SetControlCore(TControl control) {
			this.control = control;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			if (control != null)
				control.BeforeDispose -= OnControlBeforeDispose;
		}
		protected internal override void SubscribeItemProviderControlEvents() {
			SubscribeControlEvents();
		}
		protected internal override void UnsubscribeItemProviderControlEvents() {
			UnsubscribeControlEvents();
		}
		protected internal virtual void SubscribeControlEvents() {
			if (control != null)
				control.BeforeDispose += OnControlBeforeDispose;
		}
		protected internal virtual void OnControlBeforeDispose(object sender, EventArgs e) {
			Control = null;
		}
		protected internal override void UpdateBarItem(BarItem item) {
			IControlCommandBarItem<TControl, TCommandId> btn = item as IControlCommandBarItem<TControl, TCommandId>;
			if (btn == null)
				return;
			btn.Control = control;
			base.UpdateBarItem(item);
		}
		protected internal override void UpdatePageGroup(CommandBasedRibbonPageGroup pageGroup) {
			ControlCommandBasedRibbonPageGroup<TControl, TCommandId> group = pageGroup as ControlCommandBasedRibbonPageGroup<TControl, TCommandId>;
			if (group != null)
				group.Control = Control;
			base.UpdatePageGroup(pageGroup);
		}
		protected override void PopulateNewItems(List<BarItem> items, BarCreationContextBase creationContex) {
			if (Control == null)
				return;
			base.PopulateNewItems(items, creationContex);
		}
		protected override bool CanCreateVisualItems() {
			return base.CanCreateVisualItems() && Control != null;
		}
		#region IBarItemContainer Members
		List<BarItem> IBarItemContainer.GetBarItems() {
			return base.GetSupportedBarItems();
		}
		#endregion
	}
	#endregion
	#region CommandBasedRibbonPageGroup (abstract class)
	public abstract class CommandBasedRibbonPageGroup : RibbonPageGroup {
		SuperToolTip defaultSuperToolTip;
		ShowCaptionButtonMode showCaptionButtonMode = ShowCaptionButtonMode.Auto;
		protected CommandBasedRibbonPageGroup()
			: this(String.Empty) {
		}
		protected CommandBasedRibbonPageGroup(string text)
			: base(text) {
			this.defaultSuperToolTip = new SuperToolTip();
			CaptionButtonClick += new RibbonPageGroupEventHandler(OnCaptionButtonClick);
		}
		[Browsable(false)]
		public abstract string DefaultText { get; }
		SuperToolTip DefaultSuperToolTip { get { return defaultSuperToolTip; } }
		[DefaultValue(ShowCaptionButtonMode.Auto), System.ComponentModel.Category("Behavior")]
		public ShowCaptionButtonMode ShowCaptionButtonMode { 
			get { return showCaptionButtonMode; }
			set {
				if (ShowCaptionButtonMode == value)
					return;
				showCaptionButtonMode = value;
				UpdateCaptionButtonVisibility();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShowCaptionButton { get { return base.ShowCaptionButton; } set { base.ShowCaptionButton = value; } } 
		internal override bool ShouldSerializeSuperTip() {
			return SuperTip != null && !SuperTip.IsEmpty && SuperTip != DefaultSuperToolTip;
		}
		protected internal override bool ShouldSerializeText() {
			return Text != DefaultText;
		}
		protected internal override void ResetText() {
			Text = DefaultText;
		}
		protected internal virtual void UpdateText() {
			if (String.IsNullOrEmpty(Text))
				Text = DefaultText;
		}
		protected internal virtual void UpdateSuperTip() {
			Command command = CreateCommand();
			UpdateSuperTip(command);
		}
		protected internal virtual void UpdateCaptionButtonVisibility() {
			if (ShowCaptionButtonMode == ShowCaptionButtonMode.Hide)
				ShowCaptionButton = false;
			else if (ShowCaptionButtonMode == ShowCaptionButtonMode.Show)
				ShowCaptionButton = true;
			else {
				Command command = CreateCommand();
				UpdateCaptionButtonVisibility(command);
			}
		}
		void UpdateSuperTip(Command command) {
			if (command == null)
				return;
			if (SuperTip == null)
				SuperTip = DefaultSuperToolTip;
			if (!SuperTip.Equals(DefaultSuperToolTip))
				return;
			SuperTip.Items.Clear();
			SuperTip.Items.AddTitle(command.MenuCaption);
			SuperTip.Items.Add(command.Description);
		}
		void UpdateCaptionButtonVisibility(Command command) {
			ShowCaptionButton = CanExecuteCommand(command);
		}
		void OnCaptionButtonClick(object sender, RibbonPageGroupEventArgs e) {
			if (ShowCaptionButtonMode != ShowCaptionButtonMode.Auto)
				return;
			Command command = CreateCommand();
			if (!CanExecuteCommand(command))
				return;
			command.Execute();
		}
		protected virtual Command CreateCommand() {
			return null;
		}
		bool CanExecuteCommand(Command command) {
			return command != null;
		}
		protected internal virtual void DeleteToolbarContentButton() {
			if (ToolbarContentButton != null)
				ToolbarContentButton.Dispose();
		}
	}
	#endregion
	#region CommandBasedRibbonMiniToolbar (abstract class)
	public abstract class CommandBasedRibbonMiniToolbar<TControl, TCommandId> : RibbonMiniToolbar
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		public virtual TControl Control {
			get;
			set;
		}
	}
	#endregion
	#region ControlCommandBasedRibbonPageGroup<TControl> (abstract class)
	public abstract class ControlCommandBasedRibbonPageGroup<TControl, TCommandId> : CommandBasedRibbonPageGroup
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		TControl control;
		protected ControlCommandBasedRibbonPageGroup()
			: this(String.Empty) {
		}
		protected ControlCommandBasedRibbonPageGroup(string text)
			: base(text) {
		}
		[Browsable(false)]
		public virtual TCommandId CommandId { get { return EmptyCommandId; } }
		protected abstract TCommandId EmptyCommandId { get; }
		protected internal virtual void SubscribeControlEvents() {
			Control.BeforeDispose += OnBeforeDisposeControl;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			Control.BeforeDispose -= OnBeforeDisposeControl;
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			Control = null;
		}
		protected override Command CreateCommand() {
			if (Control == null || Object.Equals(CommandId, EmptyCommandId))
				return null;
			Command command = Control.CreateCommand(CommandId);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TControl Control {
			get { return control; }
			set {
				if (Object.ReferenceEquals(control, value))
					return;
				if (control != null)
					UnsubscribeControlEvents();
				control = value;
				if (control != null)
					SubscribeControlEvents();
				UpdateCaptionButtonVisibility();
			}
		}
	}
	#endregion
	#region CommandBasedRibbonPage (abstract class)
	public abstract class CommandBasedRibbonPage : RibbonPage {
		protected CommandBasedRibbonPage()
			: base() {
		}
		protected CommandBasedRibbonPage(string text)
			: base(text) {
		}
		[Browsable(false)]
		public abstract string DefaultText { get; }
		protected internal override bool ShouldSerializeText() {
			return Text != DefaultText;
		}
		protected internal override void ResetText() {
			Text = DefaultText;
		}
		protected internal virtual void UpdateText() {
			if (String.IsNullOrEmpty(Text))
				Text = DefaultText;
		}
	}
	#endregion
	#region ControlCommandBasedRibbonPage (abstract class)
	public abstract class ControlCommandBasedRibbonPage : CommandBasedRibbonPage {
		protected ControlCommandBasedRibbonPage() {
			Text = DefaultText;
		}
		protected ControlCommandBasedRibbonPage(string text)
			: base(text) {
		}
		public void Update() {
			UpdateText();
		}
	}
	#endregion
	#region CommandBasedRibbonPageCategory (abstract class)
	public abstract class CommandBasedRibbonPageCategory : RibbonPageCategory {
		protected CommandBasedRibbonPageCategory() {
		}
		protected CommandBasedRibbonPageCategory(string text, Color color)
			: base(text, color) {
		}
		protected CommandBasedRibbonPageCategory(string text, Color color, bool visible)
			: base(text, color, visible) {
		}
		[Browsable(false)]
		public abstract string DefaultText { get; }
		protected internal override bool ShouldSerializeText() {
			return Text != DefaultText;
		}
		protected internal override void ResetText() {
			Text = DefaultText;
		}
		protected internal virtual void UpdateText() {
			if (String.IsNullOrEmpty(Text))
				Text = DefaultText;
		}
	}
	#endregion
	#region ControlCommandBasedRibbonPageCategory (abstract class)
	public abstract class ControlCommandBasedRibbonPageCategory<TControl, TCommandId> : CommandBasedRibbonPageCategory
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		#region Fields
		TControl control;
		#endregion
		protected ControlCommandBasedRibbonPageCategory() {
			Text = DefaultText;
		}
		public virtual TControl Control {
			get { return control; }
			set {
				if (Object.ReferenceEquals(control, value))
					return;
				if (control != null)
					UnsubscribeControlEvents();
				control = value;
				if (control != null)
					SubscribeControlEvents();
			}
		}
		public virtual TCommandId CommandId { get { return EmptyCommandId; } }
		protected abstract TCommandId EmptyCommandId { get; }
		BarManager Manager { get { return Ribbon != null ? Ribbon.Manager : null; } }
		protected internal bool CanUpdateVisibility { get { return Manager != null && !Manager.IsDesignMode && !Manager.IsLoading; } }
		public void Update() {
			UpdateText();
		}
		protected internal virtual void SubscribeControlEvents() {
			Control.BeforeDispose += OnBeforeDisposeControl;
			Control.UpdateUI += OnControlUpdateUI;
		}
		protected internal virtual void UnsubscribeControlEvents() {
			Control.BeforeDispose -= OnBeforeDisposeControl;
			Control.UpdateUI -= OnControlUpdateUI;
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.Control = null;
		}
		protected internal virtual void OnControlUpdateUI(object sender, EventArgs e) {
			this.UpdateItemVisibility();
		}
		protected internal virtual void UpdateItemVisibility() {
			UpdateUIStateCore();
		}
		protected internal virtual Command CreateCommand() {
			if (Control == null || Object.Equals(CommandId, EmptyCommandId))
				return null;
			Command command = Control.CreateCommand(CommandId);
			if (command == null)
				return null;
			command.CommandSourceType = CommandSourceType.Menu;
			return command;
		}
		protected internal virtual void UpdateUIStateCore() {
			if (!CanUpdateVisibility)
				return;
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateItemUIState();
				command.UpdateUIState(state);
			}
		}
		protected internal virtual ICommandUIState CreateItemUIState() {
			return new RibbonPageCategoryUIState(this);
		}
	}
	#endregion
}
namespace DevExpress.XtraBars.Commands.Internal {
	#region ControlCommandBarCreator (abstract class)
	public abstract class ControlCommandBarCreator {
		CommandBasedBarItemBuilder itemBuilder;
		protected ControlCommandBarCreator() {
		}
		#region Properties
		CommandBasedBarItemBuilder ItemBuilder {
			get {
				if (itemBuilder == null)
					this.itemBuilder = CreateBarItemBuilder();
				return itemBuilder;
			}
		}
		public virtual Type SupportedRibbonPageCategoryType { get { return typeof(int); } } 
		public virtual int DockRow { get { return 0; } }
		public virtual int DockColumn { get { return 0; } }
		public abstract Type SupportedRibbonPageType { get; }
		public abstract Type SupportedRibbonPageGroupType { get; }
		public abstract Type SupportedBarType { get; }
		public virtual Type[] SupportedRibbonMiniToolbarTypes { get { return null; } }
		#endregion
		public abstract Bar CreateBar();
		public abstract CommandBasedBarItemBuilder CreateBarItemBuilder();
		public void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex) {
			ItemBuilder.PopulateItems(items, creationContex);
		}
		public virtual CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return null; 
		}
		public virtual bool ShouldCreateRibbonMiniToolbar {
			get { return false; }
		}
		public abstract CommandBasedRibbonPage CreateRibbonPageInstance();
		public abstract CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance();
		public virtual bool ShouldAddItemToRibbonMiniToolbar(BarItem item, RibbonMiniToolbar miniToolbar) {
			return true;
		}
	}
	#endregion
	#region BarButtonItemUIState
	public class BarButtonItemUIState : ICommandUIState {
		readonly BarButtonItem item;
		public BarButtonItemUIState(BarButtonItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		public virtual bool Enabled { get { return item.Enabled; } set { item.Enabled = value; } }
		public virtual bool Visible { get { return item.Visibility == BarItemVisibility.Always; } set { ChangeVisible(value); } }
		public virtual bool Checked { get { return false; } set { } }
		public virtual object EditValue { get { return null; } set { } }
		protected internal virtual void ChangeVisible(bool visible) {
			if (visible)
				item.Visibility = BarItemVisibility.Always;
			else
				item.Visibility = BarItemVisibility.Never;
		}
	}
	#endregion
	#region 
	public class RibbonGalleryBarItemUIState : BarButtonItemUIState {
		public RibbonGalleryBarItemUIState(RibbonGalleryBarItem item)
			: base(item) {
		}
	}
	#endregion
	#region BarButtonGalleryDropDownItemValueUIState
	public class BarButtonGalleryDropDownItemValueUIState<T> : IValueBasedCommandUIState<T> {
		#region Fields
		readonly BarButtonItem item;
		#endregion
		public BarButtonGalleryDropDownItemValueUIState(BarButtonItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		#region Properties
		public virtual bool Enabled { get { return item.Enabled; } set { item.Enabled = value; } }
		public virtual bool Visible { get { return item.Visibility == BarItemVisibility.Always; } set { ChangeVisible(value); } }
		public virtual bool Checked { get { return false; } set { } }
		public virtual object EditValue { get { return null; } set { } }
		public virtual T Value { get { return (T)EditValue; } set { MakeVisibleCurrentItem(value); } }
		#endregion
		protected internal virtual void ChangeVisible(bool visible) {
			if (visible)
				item.Visibility = BarItemVisibility.Always;
			else
				item.Visibility = BarItemVisibility.Never;
		}
		protected internal virtual void MakeVisibleCurrentItem(T value) {
			GalleryDropDown galleryDropDown = item.DropDownControl as GalleryDropDown;
			if (galleryDropDown == null)
				return;
			BarItemValueUIStateHelper<T>.MakeVisibleCurrentItem(galleryDropDown.Gallery, value);
		}
	}
	#endregion
	#region BarCheckItemUIState
	public class BarCheckItemUIState : ICommandUIState {
		readonly BarCheckItem item;
		public BarCheckItemUIState(BarCheckItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		public virtual bool Enabled { get { return item.Enabled; } set { item.Enabled = value; } }
		public virtual bool Visible { get { return item.Visibility == BarItemVisibility.Always; } set { ChangeVisible(value); } }
		public virtual bool Checked { get { return item.Checked; } set { item.Checked = value; } }
		public virtual object EditValue { get { return null; } set { } }
		protected BarCheckItem Item { get { return item; } }
		protected internal virtual void ChangeVisible(bool visible) {
			if (visible)
				item.Visibility = BarItemVisibility.Always;
			else
				item.Visibility = BarItemVisibility.Never;
		}
	}
	#endregion
	#region BarEditItemUIState
	public class BarEditItemUIState<T> : IValueBasedCommandUIState<T> {
		readonly BarEditItem item;
		public BarEditItemUIState(BarEditItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		protected BarEditItem Item { get { return item; } }
		public virtual bool Enabled { get { return item.Enabled; } set { item.Enabled = value; } }
		public virtual bool Visible { get { return item.Visibility == BarItemVisibility.Always; } set { ChangeVisible(value); } }
		public virtual bool Checked { get { return false; } set { } }
		public virtual object EditValue {
			get { return Value; }
			set {
				try {
					Value = (T)value;
				}
				catch {
					try {
						Value = (T)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
					}
					catch {
					}
				}
			}
		}
		public virtual T Value { get { return (T)item.EditValue; } set { item.EditValue = value; } }
		protected internal virtual void ChangeVisible(bool visible) {
			if (visible)
				item.Visibility = BarItemVisibility.Always;
			else
				item.Visibility = BarItemVisibility.Never;
		}
	}
	#endregion
	#region BarSubItemUIState
	public class BarSubItemUIState : ICommandUIState {
		readonly BarSubItem item;
		public BarSubItemUIState(BarSubItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		public virtual bool Enabled { get { return item.Enabled; } set { item.Enabled = value; } }
		public virtual bool Visible { get { return item.Visibility == BarItemVisibility.Always; } set { ChangeVisible(value); } }
		public virtual bool Checked { get { return false; } set { } }
		public virtual object EditValue { get { return null; } set { } }
		protected internal virtual void ChangeVisible(bool visible) {
			if (visible)
				item.Visibility = BarItemVisibility.Always;
			else
				item.Visibility = BarItemVisibility.Never;
		}
	}
	#endregion
	#region BarGalleryItemUIState
	public class BarGalleryItemUIState : ICommandUIState {
		readonly RibbonGalleryBarItem item;
		public BarGalleryItemUIState(RibbonGalleryBarItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		public virtual bool Enabled { get { return item.Enabled; } set { item.Enabled = value; } }
		public virtual bool Visible { get { return item.Visibility == BarItemVisibility.Always; } set { ChangeVisible(value); } }
		public virtual bool Checked { get { return false; } set { } }
		public virtual object EditValue { get { return null; } set { } }
		protected internal virtual void ChangeVisible(bool visible) {
			if (visible)
				item.Visibility = BarItemVisibility.Always;
			else
				item.Visibility = BarItemVisibility.Never;
		}
	}
	#endregion
	#region BarGalleryItemValueUIState
	public class BarGalleryItemValueUIState<T> : IValueBasedCommandUIState<T> {
		readonly RibbonGalleryBarItem item;
		public BarGalleryItemValueUIState(RibbonGalleryBarItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		public virtual bool Enabled { get { return item.Enabled; } set { item.Enabled = value; } }
		public virtual bool Visible { get { return item.Visibility == BarItemVisibility.Always; } set { ChangeVisible(value); } }
		public virtual bool Checked { get { return false; } set { } }
		public virtual object EditValue { get { return null; } set { } }
		public virtual T Value { get { return (T)EditValue; } set { MakeVisibleCurrentItem(value); } }
		protected internal virtual void ChangeVisible(bool visible) {
			if (visible)
				item.Visibility = BarItemVisibility.Always;
			else
				item.Visibility = BarItemVisibility.Never;
		}
		protected internal virtual void MakeVisibleCurrentItem(T value) {
			BarItemValueUIStateHelper<T>.MakeVisibleCurrentItem(item.Gallery, value);
		}
	}
	#endregion
	#region GalleryItemUIState
	public class GalleryItemUIState : ICommandUIState {
		readonly GalleryItem item;
		public GalleryItemUIState(GalleryItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		public virtual bool Enabled { get { return false; } set { } }
		public virtual bool Visible { get { return item.Visible; } set { item.Visible = value; } }
		public virtual bool Checked { get { return item.Checked; } set { item.Checked = value; } }
		public virtual object EditValue { get { return null; } set { } }
	}
	#endregion
	#region GalleryItemGroupUIState
	public class GalleryItemGroupUIState : ICommandUIState {
		readonly GalleryItemGroup item;
		public GalleryItemGroupUIState(GalleryItemGroup item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		public virtual bool Enabled { get { return false; } set { } }
		public virtual bool Visible { get { return item.Visible; } set { item.Visible = value; } }
		public virtual bool Checked { get { return false; } set { } }
		public virtual object EditValue { get { return null; } set { } }
	}
	#endregion
	#region RibbonPageCategoryUIState
	public class RibbonPageCategoryUIState : ICommandUIState {
		readonly RibbonPageCategory category;
		public RibbonPageCategoryUIState(RibbonPageCategory category) {
			Guard.ArgumentNotNull(category, "category");
			this.category = category;
		}
		public virtual bool Enabled { get { return category.Visible; } set { SetVisible(value); } } 
		public virtual bool Visible { get { return category.Visible; } set { SetVisible(value); } }
		public virtual bool Checked { get { return false; } set { } }
		public virtual object EditValue { get { return null; } set { } }
		void SetVisible(bool visible) {
			category.Visible = visible;
			foreach (RibbonPage page in category.Pages)
				page.Visible = visible;
		}
	}
	#endregion
	#region BarItemValueUIStateHelper
	public static class BarItemValueUIStateHelper<T> {
		public static void MakeVisibleCurrentItem(BaseGallery gallery, T value) {
			GalleryItemGroupCollection groups = gallery.Groups;
			int groupsCount = groups.Count;
			for (int i = 0; i < groupsCount; i++) {
				MakeVisibleItem(groups[i].Items, value);
			}
		}
		public static void MakeVisibleItem(GalleryItemCollection galleryItems, T value) {
			int count = galleryItems.Count;
			for (int i = 0; i < count; i++) {
				GalleryItem currentItem = galleryItems[i];
				if (object.Equals((T)currentItem.Tag, value))
					currentItem.Checked = true;
				else
					currentItem.Checked = false;
			}
		}
	}
	#endregion
	#region SuperToolTipHelper
	public class SuperToolTipHelper {
		public static bool SuperTipHasDefaultContent(SuperToolTip superTip, string title, string descr, BarShortcut itemShortcut) {
			if (superTip == null)
				return true;
			if (superTip.Items.Count != 2)
				return false;
			ToolTipTitleItem titleItem = superTip.Items[0] as ToolTipTitleItem;
			if (!IsDefaultOrEmptyItemText(titleItem, CreateTitleText(title, GetShortcutText(itemShortcut))))
				return false;
			ToolTipItem descrItem = superTip.Items[1] as ToolTipItem;
			if (!IsDefaultOrEmptyItemText(descrItem, descr))
				return false;
			return true;
		}
		internal static bool IsDefaultOrEmptyItemText(ToolTipItem item, string defaultText) {
			if (item == null)
				return false;
			return string.IsNullOrEmpty(item.Text) || item.Text == defaultText;
		}
		public static SuperToolTip CreateSuperToolTip(string title, string descr, BarShortcut itemShortcut) {
			SuperToolTip superTip = new SuperToolTip();
			string titleText = CreateTitleText(title, GetShortcutText(itemShortcut));
			superTip.Items.AddTitle(titleText);
			superTip.Items.Add(descr);
			return superTip;
		}
		internal static string CreateTitleText(string title, string shortcutText) {
			return !string.IsNullOrEmpty(shortcutText) ? String.Format("{0} ({1})", title, shortcutText) : title;
		}
		static string GetShortcutText(BarShortcut itemShortcut) {
			if (CommandBasedBarButtonItem.BarShortcutAuto.Equals(itemShortcut))
				return String.Empty;
			return itemShortcut.IsExist ? itemShortcut.ToString() : string.Empty;
		}
	}
	#endregion
	#region BarCollectorBase
	public abstract class BarCollectorBase {
		internal static readonly string DefaultImageTag = "default";
		internal static readonly string ImageFromListTag = "dx_imagefromlist";
		readonly List<BarItem> items;
		protected BarCollectorBase(List<BarItem> items) {
			Guard.ArgumentNotNull(items, "items");
			this.items = items;
		}
		public List<BarItem> Items { get { return items; } }
		public virtual void CollectContainers(List<BarManager> managerCollection) {
			for (int i = 0; i < managerCollection.Count; i++) {
				BarManager manager = managerCollection[i];
				CollectContainers(manager);
			}
		}
		protected abstract void CollectContainers(BarManager manager);
	}
	#endregion
	#region RibbonBarCollector
	public class RibbonBarCollector : BarCollectorBase {
		readonly List<RibbonPageGroup> usedRibbonPageGroups;
		readonly List<RibbonPage> usedRibbonPages;
		readonly List<RibbonPageCategory> usedRibbonPageCategories;
		public RibbonBarCollector(List<BarItem> items)
			: base(items) {
			this.usedRibbonPageGroups = new List<RibbonPageGroup>();
			this.usedRibbonPages = new List<RibbonPage>();
			this.usedRibbonPageCategories = new List<RibbonPageCategory>();
		}
		public List<RibbonPageGroup> UsedRibbonPageGroups { get { return usedRibbonPageGroups; } }
		public List<RibbonPage> UsedRibbonPages { get { return usedRibbonPages; } }
		public List<RibbonPageCategory> UsedRibbonPageCategories { get { return usedRibbonPageCategories; } }
		public override void CollectContainers(List<BarManager> managerCollection) {
			base.CollectContainers(managerCollection);
			CollectUsedRibbonPages(UsedRibbonPageGroups);
			CollectUsedRibbonPageCategories(UsedRibbonPages);
		}
		protected override void CollectContainers(BarManager manager) {
			RibbonBarManager ribbonBarManager = manager as RibbonBarManager;
			if (ribbonBarManager == null)
				return;
			RibbonControl ribbon = ribbonBarManager.Ribbon;
			CollectUsedRibbonGroups(ribbon.Pages);
			int categoryCount = ribbon.PageCategories.Count;
			for (int i = 0; i < categoryCount; i++)
				CollectUsedRibbonGroups(ribbon.PageCategories[i].Pages);
		}
		void CollectUsedRibbonPageCategories(List<RibbonPage> usedRibbonPages) {
			int count = usedRibbonPages.Count;
			for (int i = 0; i < count; i++) {
				RibbonPage page = usedRibbonPages[i];
				RibbonPageCategory category = page.Category;
				if (category != null && !UsedRibbonPageCategories.Contains(category))
					UsedRibbonPageCategories.Add(category);
			}
		}
		void CollectUsedRibbonPages(List<RibbonPageGroup> usedRibbonPageGroups) {
			int count = usedRibbonPageGroups.Count;
			for (int i = 0; i < count; i++) {
				RibbonPageGroup pageGroup = usedRibbonPageGroups[i];
				RibbonPage page = pageGroup.Page;
				if (page != null && !UsedRibbonPages.Contains(page))
					UsedRibbonPages.Add(page);
			}
		}
		public void CollectUsedRibbonGroups(RibbonPageCollection pages) {
			int count = pages.Count;
			for (int i = 0; i < count; i++) {
				RibbonPage page = pages[i];
				List<RibbonPageGroup> pageGroups = FindPageGroupsWhichHoldSpecifiedItems(page, Items);
				UsedRibbonPageGroups.AddRange(pageGroups);
			}
		}
		List<RibbonPageGroup> FindPageGroupsWhichHoldSpecifiedItems(RibbonPage page, List<BarItem> items) {
			List<RibbonPageGroup> result = new List<RibbonPageGroup>();
			RibbonPageGroupCollection groups = page.Groups;
			int count = groups.Count;
			for (int i = 0; i < count; i++) {
				RibbonPageGroup group = groups[i];
				if (IsGroupContainsOneOfTheItems(group, items))
					result.Add(group);
			}
			return result;
		}
		bool IsGroupContainsOneOfTheItems(RibbonPageGroup group, List<BarItem> items) {
			RibbonPageGroupItemLinkCollection groupItemLinks = group.ItemLinks;
			int count = groupItemLinks.Count;
			for (int i = 0; i < count; i++) {
				BarItemLink groupItemLink = groupItemLinks[i];
				BarButtonGroup buttonGroup = groupItemLink.Item as BarButtonGroup;
				if (buttonGroup != null)
					return IsButtonGroupContainsOneOfTheItems(buttonGroup, items);
				if (items.Contains(groupItemLink.Item))
					return true;
			}
			return false;
		}
		bool IsButtonGroupContainsOneOfTheItems(BarButtonGroup buttonGroup, List<BarItem> items) {
			BarItemLinkCollection groupItemLinks = buttonGroup.ItemLinks;
			int count = groupItemLinks.Count;
			for (int i = 0; i < count; i++) {
				BarItemLink groupItemLink = groupItemLinks[i];
				if (items.Contains(groupItemLink.Item))
					return true;
			}
			return false;
		}
	}
	#endregion
	#region BarCollector
	public class BarCollector : BarCollectorBase {
		readonly List<Bar> usedBars;
		public BarCollector(List<BarItem> items)
			: base(items) {
			this.usedBars = new List<Bar>();
		}
		public List<Bar> UsedBars { get { return usedBars; } }
		protected override void CollectContainers(BarManager manager) {
			Bars bars = manager.Bars;
			int count = bars.Count;
			for (int i = 0; i < count; i++) {
				Bar bar = bars[i];
				if (IsBarContainsItems(bar, Items))
					UsedBars.Add(bar);
			}
		}
		bool IsBarContainsItems(Bar bar, List<BarItem> items) {
			LinksInfo linksInfo = bar.LinksPersistInfo;
			int count = linksInfo.Count;
			for (int i = 0; i < count; i++) {
				BarItem item = linksInfo[i].Item;
				if (items.Contains(item))
					return true;
			}
			return false;
		}
	}
	#endregion
	#region CommandBasedBarComponentBaseHelper
	public static class CommandBasedBarComponentBaseHelper {
		public static List<BarItem> GetSupportedBarItems(CommandBasedBarComponentBase component) {
			return component.GetSupportedBarItems();
		}
	}
	#endregion
}
