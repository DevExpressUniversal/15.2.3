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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGrid.Views.Layout.Drawing;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
namespace DevExpress.XtraGrid.Views.Layout {
	internal class LayoutViewRenameItemManager : RenameItemManager {
		protected override void EditorBeforeOpen(BaseLayoutItem item, DevExpress.XtraEditors.Controls.ModalTextBox textBox) {
			LayoutViewField field = item as LayoutViewField;
			if(field != null) textBox.EditText = field.Column.Caption;
			else base.EditorBeforeOpen(item, textBox);
		}
	}
	internal class LayoutViewComponentsUpdateHelper : ComponentsUpdateHelper {
		protected IContainer container;
		public LayoutViewComponentsUpdateHelper(ComponentsUpdateHelperRoles role, ILayoutControl owner)
			: base(role, owner) {
			container = owner as IContainer;
		}
		protected override void AddComponentToOwnerCompnents(IComponent component) {
			if((controlOwner as ISupportImplementor).Implementor.CloneInProgress) return;
			ArrayList components = new ArrayList(controlOwner.Components);
			bool fNotExist = !AlreadyInExistingComponents(controlOwner as LayoutView, component, components);
			if(fNotExist) base.AddComponentToOwnerCompnents(component);
		}
		protected override void AddComponentToDesignSurface(IComponent component) {
			if((controlOwner as ISupportImplementor).Implementor.CloneInProgress) return;
			if(controlOwner.Site != null) {
				ArrayList components = new ArrayList(controlOwner.Site.Container.Components);
				bool fNotExist = !AlreadyInExistingComponents(controlOwner as LayoutView, component, components);
				if(fNotExist && ((ILayoutControl)controlOwner).AllowManageDesignSurfaceComponents) {
					controlOwner.Site.Container.Add(component);
					CheckComponentDesignTimeName(component);
				}
			}
		}
		public static void CheckComponentDesignTimeName(IComponent component) {
			BaseLayoutItem item = component as BaseLayoutItem;
			if(item != null && !(item is LayoutViewCard)) {
				string oldName = item.Name;
				if(oldName != String.Empty) {
					try {
						if(item.Name != oldName) item.Name = oldName;
						component.Site.Name = item.Name;
					}
					catch { item.Name = component.Site.Name; }
				}
			}
		}
		protected override void RemoveComponentFromDesignSurface(IComponent component) {
			if(controlOwner.Site != null) {
				ArrayList components = new ArrayList(controlOwner.Site.Container.Components);
				bool fExist = AlreadyInExistingComponents(controlOwner as LayoutView, component, components);
				if(fExist && ((ILayoutControl)controlOwner).AllowManageDesignSurfaceComponents) {
					controlOwner.Site.Container.Remove(component);
				}
			}
		}
		public static bool AlreadyInExistingComponents(LayoutView view, IComponent component, ArrayList components) {
			BaseLayoutItem item = component as BaseLayoutItem;
			foreach(IComponent comp in components) {
				if(comp.GetType() == item.GetType()) {
					BaseLayoutItem existingItem = comp as BaseLayoutItem;
					if(item.Name == existingItem.Name && item.Owner == view) {
						LayoutView ownerView = existingItem.Owner as LayoutView;
						if(ownerView != null && ownerView.Name != view.Name)
							continue;
						return true;
					}
				}
			}
			return false;
		}
		public static IComponent FindInExistingComponents(LayoutView view, BaseLayoutItem item, ComponentCollection components) {
			foreach(IComponent comp in components) {
				if(comp.GetType() == item.GetType()) {
					BaseLayoutItem existingItem = comp as BaseLayoutItem;
					if(item.Name == existingItem.Name && item.Owner == view) {
						LayoutView ownerView = existingItem.Owner as LayoutView;
						if(ownerView != null && ownerView.Name != view.Name)
							continue;
						return existingItem;
					}
				}
			}
			return null;
		}
	}
	public class LayoutViewLayoutControlImplementor : LayoutControlImplementor {
		public LayoutViewLayoutControlImplementor(ILayoutControlOwner owner) : base(owner) { }
		protected override LayoutControlGroup CreateRoot() { return (LayoutControlGroup)new LayoutViewCard(); }
		public override LayoutGroupHandlerWithTabHelper CreateRootHandler(LayoutGroup group) {
			LayoutGroupHandlerWithTabHelper handler = base.CreateRootHandler(group);
			handler.AllowProcessHotTracking = false;
			return handler;
		}
		protected override bool AllowTimer { get { return false; } }
		protected override void InitializePaintStyles() {
			ISupportLookAndFeel lookAndFeelOwner = owner.GetISupportLookAndFeel();
			if(lookAndFeelOwner != null) {
				PaintStyles.Add(new LayoutViewSkinPaintStyle(lookAndFeelOwner, View));
				PaintStyles.Add(new LayoutViewWindowsXPPaintStyle(lookAndFeelOwner, View));
				PaintStyles.Add(new LayoutViewMixedXPPaintStyle(lookAndFeelOwner, View));
				PaintStyles.Add(new LayoutViewOffice2003PaintStyle(lookAndFeelOwner, View));
				PaintStyles.Add(new LayoutViewStyle3DPaintStyle(lookAndFeelOwner, View));
				PaintStyles.Add(new LayoutViewUltraFlatPaintStyle(lookAndFeelOwner, View));
				PaintStyles.Add(new LayoutViewFlatPaintStyle(lookAndFeelOwner, View));
			}
			lookAndFeelOwner = null;
		}
		public override LayoutSerializationOptions OptionsSerialization {
			get {
				if(optionsSerializationCore == null) {
					optionsSerializationCore = new LayoutSerializationOptions();
					optionsSerializationCore.RestoreLayoutGroupAppearanceGroup = true;
					optionsSerializationCore.RestoreAppearanceItemCaption = true;
					optionsSerializationCore.RestoreGroupPadding = true;
					optionsSerializationCore.RestoreGroupSpacing = true;
					optionsSerializationCore.RestoreLayoutItemPadding = true;
					optionsSerializationCore.RestoreLayoutItemSpacing = true;
					optionsSerializationCore.RestoreRootGroupPadding = true;
					optionsSerializationCore.RestoreRootGroupSpacing = true;
					optionsSerializationCore.RestoreTabbedGroupPadding = true;
					optionsSerializationCore.RestoreTabbedGroupSpacing = true;
					optionsSerializationCore.RestoreTextToControlDistance = true;
				}
				return optionsSerializationCore;
			}
		}
		DevExpress.Skins.SkinElement cardElement;
		DevExpress.Skins.SkinElement cardElementNoBorder;
		protected override void EnsureSkinElements() {
			if(cardElement == null)
				cardElement = DevExpress.Skins.GridSkins.GetSkin(LookAndFeel)[DevExpress.Skins.GridSkins.SkinLayoutViewCard];
			if(cardElementNoBorder == null)
				cardElementNoBorder = DevExpress.Skins.GridSkins.GetSkin(LookAndFeel)[DevExpress.Skins.GridSkins.SkinLayoutViewCardNoBorder];
			base.EnsureSkinElements();
		}
		protected override void ResetSkinElements() {
			cardElement = null;
			cardElementNoBorder = null;
			base.ResetSkinElements();
		}
		protected override System.Drawing.Color GetControlColor(BaseLayoutItem item) {
			LayoutViewCard card = item as LayoutViewCard;
			if(card != null && cardElementNoBorder != null && card.AllowDrawBackground)
				return cardElementNoBorder.Color.GetForeColor();
			return View.GetForeColor();
		}
		protected override bool IsGroupElementVisible(LayoutGroup group) {
			if(group is LayoutViewCard && !group.GroupBordersVisible) {
				return group.AllowDrawBackground && View.OptionsView.ShowCardBorderIfCaptionHidden;
			}
			return base.IsGroupElementVisible(group);
		}
		protected override Color GetGroupColor(LayoutGroup group) {
			if(group is LayoutViewCard && cardElement != null)
				return cardElement.Color.GetForeColor();
			return base.GetGroupColor(group);
		}
		internal object XtraCreateItemsItem(XtraItemEventArgs e) {
			XtraPropertyInfo infoType = e.Item.ChildProperties["TypeName"];
			if(infoType != null && (string)infoType.Value == "LayoutViewField") {
				OptionsLayoutGrid optGrid = e.Options as OptionsLayoutGrid;
				if(optGrid != null) {
					if(optGrid.Columns.RemoveOldColumns) return null;
					if(!optGrid.Columns.StoreAllOptions) return null;
				}
			}
			return base.XtraCreateNewItem(e);
		}
		public override BaseLayoutItem XtraCreateNewItem(XtraItemEventArgs e) {
			XtraPropertyInfo infoType = e.Item.ChildProperties["TypeName"];
			if(infoType != null && (string)infoType.Value == "LayoutViewField") {
				return null;
			}
			return base.XtraCreateNewItem(e);
		}
		public void XtraClearItems(XtraItemEventArgs e) {
			OptionsLayoutGrid optGrid = e.Options as OptionsLayoutGrid;
			bool addNewItems = (optGrid != null && optGrid.Columns.AddNewColumns);
			if(!addNewItems) {
				if(e.Item.ChildProperties == null || e.Item.ChildProperties.Count == 0) {
					ItemsAndNames.Clear();
				}
				else RemoveNewItemsFromCache(e, ItemsAndNames);
			}
		}
		protected void RemoveNewItemsFromCache(XtraItemEventArgs e, Dictionary<string, BaseLayoutItem> cache) {
			List<string> oldItems = GetItemsToRestore(e);
			List<string> itemsToRemove = new List<string>();
			foreach(KeyValuePair<string, BaseLayoutItem> pair in cache) {
				if(!oldItems.Contains(pair.Key)) itemsToRemove.Add(pair.Key);
			}
			foreach(string removeName in itemsToRemove) cache.Remove(removeName);
		}
		protected List<string> GetItemsToRestore(XtraItemEventArgs e) {
			List<string> oldItems = new List<string>();
			foreach(XtraPropertyInfo xp in e.Item.ChildProperties) {
				oldItems.Add((string)xp.ChildProperties["Name"].Value);
			}
			return oldItems;
		}
		protected override void OnItemFinded(Dictionary<string, BaseLayoutItem> cache, BaseLayoutItem item, string cacheKey) {
			if(item is LayoutViewCard)
				cache.Remove(cacheKey);
			if(item is LayoutViewField) {
				OptionsLayoutGrid optGrid = View.OptionsLayout;
				bool addNewItems = (optGrid != null && optGrid.Columns.AddNewColumns);
				if(addNewItems) {
					cache.Remove(cacheKey);
				}
			}
		}
		protected override void CheckNotRestoredItems(Dictionary<string, BaseLayoutItem> cache) {
			OptionsLayoutGrid optGrid = View.OptionsLayout;
			bool addNewItems = (optGrid != null && optGrid.Columns.AddNewColumns);
			if(addNewItems) {
				foreach(KeyValuePair<string, BaseLayoutItem> pair in cache) {
					if(pair.Value is LayoutViewField)
						AddItemInItems(pair.Value);
				}
				cache.Clear();
			}
		}
		public override void CheckControlsWithoutItems() {
		}
		public override bool CanInvalidate() {
			return !(castedOwner.Control == null || !IsInitialized || RootGroup == null || RootGroup.IsUpdateLocked || ResizerBroken);
		}
		protected override ComponentsUpdateHelper CreateAddComponentHelper() {
			return new LayoutViewComponentsUpdateHelper(ComponentsUpdateHelperRoles.Add, castedOwner);
		}
		protected override ComponentsUpdateHelper CreateRemoveComponentHelper() {
			return new LayoutViewComponentsUpdateHelper(ComponentsUpdateHelperRoles.Remove, castedOwner);
		}
		public override string CreateRootName() {
			return View.CreateTemplateCardName();
		}
		public override void SaveLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			if(DisposingFlag) return;
			StartStoreRestore(true);
			Stream stream = path as Stream;
			if(stream != null)
				serializer.SerializeObject(owner, stream, "View", options); 
			else
				serializer.SerializeObject(owner, path.ToString(), "View", options);
			EndStoreRestore(true);
		}
		public override void RestoreLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			if(DisposingFlag) return;
			StartStoreRestore(false);
			Stream stream = path as Stream;
			if(stream != null)
				serializer.DeserializeObject(owner, stream, "View", options);
			else
				serializer.DeserializeObject(owner, path.ToString(), "View", options);
			EndStoreRestore(false);
		}
		public override void OnStartDeserializing(LayoutAllowEventArgs e) {
			bool lockLayoutDeserializing = Base.BaseView.ChangeContext.IsChangingTo(View);
			if(!lockLayoutDeserializing)
				base.OnStartDeserializing(e);
		}
		public override void OnEndDeserializing(string restoredVersion) {
			bool lockLayoutDeserializing = Base.BaseView.ChangeContext.IsChangingTo(View);
			if(!lockLayoutDeserializing)
				base.OnEndDeserializing(restoredVersion);
			else Invalidate();
		}
		protected override DevExpress.XtraLayout.Customization.RenameItemManager CreateRenameItemManager() {
			return new LayoutViewRenameItemManager();
		}
		protected override BaseLayoutItem CreateItemByType(XtraPropertyInfo info, string typeStr) {
			BaseLayoutItem newItem = null;
			switch(typeStr) {
				case "LayoutViewField":
					newItem = new LayoutViewField();
					break;
				case "LayoutViewCard":
					newItem = new LayoutViewCard();
					string rootName = CreateRootName();
					if((string)info.Value == rootName)
						owner.Root = (LayoutControlGroup)newItem;
					break;
				default:
					newItem = base.CreateItemByType(info, typeStr);
					break;
			}
			return newItem;
		}
		protected override void EndInitCore() {
			if(RootGroup == null || isEndInitCore || ((ILayoutControl)this).InitializationFinished) return;
			isEndInitCore = true;
			SetParentsAndOwners();
			((ILayoutControl)this).Invalidate();
			isEndInitCore = false;
			((ILayoutControl)this).InitializationFinished = true;
			OnLookAndFeelStyleChanged(owner, EventArgs.Empty);
			((ILayoutControl)this).SetIsModified(false);
		}
		protected LayoutView View { get { return owner as LayoutView; } }
		protected BaseLayoutItem GetTopParent(BaseLayoutItem item) {
			BaseLayoutItem top = item;
			while(top != null && top.Parent != null) top = top.Parent;
			return top;
		}
		public override void InitializeScrollerCore(ILayoutControl owner) {
		}
		protected virtual bool IsTemplateCardChange(IComponent component) {
			bool fTemplateCardChange = false;
			BaseLayoutItem item = component as BaseLayoutItem;
			if(View != null && View.TemplateCard != null && item != null) {
				if(View.isCardsCacheResetting > 0) return false;
				if(item is LayoutViewCard || item.Parent != null) {
					fTemplateCardChange = (View.TemplateCard == GetTopParent(item));
				}
				else {
					fTemplateCardChange = (item.ParentName == "Customization" && item.Owner == View);
				}
			}
			return fTemplateCardChange;
		}
		public override bool FireChanging(IComponent component) {
			bool result = base.FireChanging(component);
			if(IsTemplateCardChange(component)) View.FireTemplateCardChanging();
			return result;
		}
		public override void FireChanged(IComponent component) {
			if(IsTemplateCardChange(component)) View.FireTemplateCardChanged();
			base.FireChanged(component);
		}
		SerializeableUserLookAndFeel lfCore = null;
		public override SerializeableUserLookAndFeel LookAndFeel {
			get {
				if(lfCore == null && View != null && View.GridControl != null) {
					lfCore = new SerializeableUserLookAndFeel(View.GridControl);
					lfCore.Assign(View.GridControl.LookAndFeel);
				}
				return lfCore;
			}
		}
		public void ResetLookAndFeel() {
			if(lfCore != null) {
				lfCore.Dispose();
				lfCore = null;
			}
		}
		public override void DisposeLookAndFeelCore() {
			base.DisposeLookAndFeelCore();
			ResetLookAndFeel();
		}
		public override void OnItemAppearanceChanged(BaseLayoutItem item) {
			if(View.IsDesignMode) {
				FireChanging(item);
				FireChanged(item);
			}
		}
	}
	public static class LayoutViewRTLHelper {
		internal static void UpdateRTLCore(ref int pos, Rectangle bounds) {
			pos = bounds.X + (bounds.Right - pos);
		}
		internal static void UpdateRTLCore(ref Point pt, Rectangle bounds) {
			if(!pt.IsEmpty)
				pt = new Point(bounds.X + (bounds.Right - pt.X), pt.Y);
		}
		internal static void UpdateRTLCore(ref Rectangle r, Rectangle bounds) {
			if(!r.IsEmpty)
				r = new Rectangle(bounds.X + (bounds.Right - r.Right), r.Top, r.Width, r.Height);
		}
	}
}
