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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Resizing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Adapters;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTab;
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.XtraLayout.Registrator;
using System.Collections.Generic;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraDashboardLayout;
namespace DevExpress.XtraLayout.Helpers {
	public enum GroupAppearanceType { AppearanceItemCaption, AppearanceGroup, AppearanceTabPageHeader, AppearanceTabPageHeaderActive, AppearanceTabPageHeaderDisabled, AppearanceTabPageHeaderHotTracked, AppearanceTabPagePageClient }
	public class LayoutGroupDefaultAppearance {
		AppearanceDefault groupAppearanceCore;
		AppearanceDefault appearanceItemCaptionCore;
		AppearanceDefault headerCore;
		AppearanceDefault headerActiveCore;
		AppearanceDefault headerDisabledCore;
		AppearanceDefault headerHotTrackedCore;
		AppearanceDefault pageClientCore;
		public LayoutGroupDefaultAppearance() {
			this.groupAppearanceCore = new AppearanceDefault();
			this.appearanceItemCaptionCore = new AppearanceDefault();
			this.headerCore = new AppearanceDefault();
			this.headerActiveCore = new AppearanceDefault();
			this.headerDisabledCore = new AppearanceDefault();
			this.headerHotTrackedCore = new AppearanceDefault();
			this.pageClientCore = new AppearanceDefault();
		}
		public AppearanceDefault AppearanceGroup {
			get { return groupAppearanceCore; }
			set { groupAppearanceCore = value; }
		}
		public AppearanceDefault AppearanceItemCaption {
			get { return appearanceItemCaptionCore; }
			set { appearanceItemCaptionCore = value; }
		}
		public AppearanceDefault Header {
			get { return headerCore; }
			set { headerCore = value; }
		}
		public AppearanceDefault HeaderActive {
			get { return headerActiveCore; }
			set { headerActiveCore = value; }
		}
		public AppearanceDefault HeaderDisabled {
			get { return headerDisabledCore; }
			set { headerDisabledCore = value; }
		}
		public AppearanceDefault HeaderHotTracked {
			get { return headerHotTrackedCore; }
			set { headerHotTrackedCore = value; }
		}
		public AppearanceDefault PageClient {
			get { return pageClientCore; }
			set { pageClientCore = value; }
		}
	}
	public class LayoutGroupAppearance : LayoutPageAppearance {
		AppearanceObject groupAppearanceCore;
		AppearanceObject appearanceItemCaptionCore;
		public LayoutGroupAppearance() {
		}
		public AppearanceObject AppearanceGroup {
			get {
				if(groupAppearanceCore == null)
					this.groupAppearanceCore = new AppearanceObject();
				return groupAppearanceCore;
			}
			set {
				if(appearanceItemCaptionCore == value) return;
				SetAppearanceCore(ref groupAppearanceCore, value);
			}
		}
		public AppearanceObject AppearanceItemCaption {
			get {
				if(appearanceItemCaptionCore == null)
					this.appearanceItemCaptionCore = new AppearanceObject();
				return appearanceItemCaptionCore;
			}
			set {
				if(appearanceItemCaptionCore == value) return;
				SetAppearanceCore(ref appearanceItemCaptionCore, value);
			}
		}
		public override void Dispose() {
			if(appearanceItemCaptionCore != null) {
				AppearanceGroup.Dispose();
				groupAppearanceCore = null;
			}
			if(appearanceItemCaptionCore != null) {
				AppearanceItemCaption.Dispose();
				appearanceItemCaptionCore = null;
			}
			base.Dispose();
		}
		void SetAppearanceCore(ref AppearanceObject appearance, AppearanceObject value) {
			if(appearance != null) appearance.Dispose();
			appearance = value;
		}
	}
	public class LayoutPageAppearance : PageAppearance {
		List<AppearanceObject> appList = new List<AppearanceObject>();
		protected override AppearanceObject CreateAppearance(bool suppressNotifications, AppearanceObject parent) {
			if(suppressNotifications) return new AppearanceObject(parent);
			AppearanceObject newParent = new AppearanceObject();
			AppearanceObject result = new AppearanceObject(this, newParent);
			appList.Add(newParent);
			result.Changed += new EventHandler(this.OnApperanceChanged);
			return result;
		}
		public override void Dispose() {
			if(appList != null) {
				foreach(AppearanceObject a in appList) a.Dispose();
				appList.Clear();
				appList = null;
			}
			base.Dispose();
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public override AppearanceObject Header { get { return base.Header; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public override AppearanceObject HeaderActive { get { return base.HeaderActive; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public override AppearanceObject HeaderDisabled { get { return base.HeaderDisabled; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public override AppearanceObject HeaderHotTracked { get { return base.HeaderHotTracked; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public override AppearanceObject PageClient { get { return base.PageClient; } }
		bool ShouldSerializeHeader() { return Header != null && Header.ShouldSerialize(); }
		bool ShouldSerializeHeaderActive() { return HeaderActive != null && HeaderActive.ShouldSerialize(); }
		bool ShouldSerializeHeaderDisabled() { return HeaderDisabled != null && HeaderDisabled.ShouldSerialize(); }
		bool ShouldSerializeHeaderHotTracked() { return HeaderHotTracked != null && HeaderHotTracked.ShouldSerialize(); }
		bool ShouldSerializePageClient() { return PageClient != null && PageClient.ShouldSerialize(); }
		public override string ToString() { return "Appearance"; }
	}
	public class EnabledStateController {
		Hashtable enabledState;
		ILayoutControl owner;
		FlatItemsList flatter;
		public EnabledStateController(ILayoutControl owner) {
			this.owner = owner;
			this.enabledState = new Hashtable();
			this.flatter = new FlatItemsList();
		}
		public bool GetItemEnabledState(BaseLayoutItem item) {
			if (enabledState.Contains(item)) return (bool)enabledState[item];
			bool result = CalcEnabledState(item, true);
			enabledState.Add(item, result);
			return result;
		}
		public void SetItemEnabledStateDirty(BaseLayoutItem item) {
			List<BaseLayoutItem> list = flatter.GetItemsList(item);
			foreach (BaseLayoutItem tItem in list) { enabledState.Remove(tItem); }
			owner.AppearanceController.SetAppearanceDirty(item);
			flatter.Clear();
		}
		protected internal void ClearReferences(BaseLayoutItem item) {
			if(enabledState.ContainsKey(item)) enabledState.Remove(item);
		}
		protected virtual bool CalcEnabledState(BaseLayoutItem item, bool state) {
			bool localEnabledState = true;
			LayoutGroup group = item as LayoutGroup;
			if (group != null) state &= group.Enabled;
			if (item.Parent == null)
				return !(owner.EnableCustomizationMode) & state && localEnabledState && owner.Control.Enabled;
			else
				return state & CalcEnabledState(item.Parent, true) & localEnabledState;
		}
	}
	public class DefaultAppearancesController : IDisposable {
		AppearanceController controller;
		Hashtable defaultAppearances;
		FlatItemsList flatter;
		TabbedGroup tabbedGroupCore = null;
		LayoutGroup layoutGroupCore = null;
		public DefaultAppearancesController(AppearanceController controller) {
			this.controller = controller;
			this.flatter = new FlatItemsList();
			this.defaultAppearances = new Hashtable();
		}
		public virtual void Dispose() {
			flatter = null;
			tabbedGroupCore = null;
			layoutGroupCore = null;
			controller = null;
			if(defaultAppearances != null) {
				foreach(object a in defaultAppearances.Values) {
					if(a is IDisposable) ((IDisposable)a).Dispose();
				}
				defaultAppearances.Clear();
				defaultAppearances = null;
			}
		}
		protected object GetDefaultAppearanceCore(BaseLayoutItem item) {
			if(defaultAppearances.Contains(item))
				return defaultAppearances[item];
			else return CalculateDefaultAppearance(item);
		}
		public AppearanceDefault GetDefaultAppearanceItem(BaseLayoutItem item) {
			return GetDefaultAppearanceCore(item) as AppearanceDefault;
		}
		public LayoutGroupDefaultAppearance GetDefaultAppearanceGroup(BaseLayoutItem item) {
			return GetDefaultAppearanceCore(item) as LayoutGroupDefaultAppearance;
		}
		protected virtual object CalculateDefaultAppearance(BaseLayoutItem item) {
			LayoutItemContainer liContainer = item as LayoutItemContainer;
			LayoutControlItem lci = item as LayoutControlItem;
			DashboardLayoutControlItemBase dli = item as DashboardLayoutControlItemBase;
			DashboardLayoutControlGroupBase dlg = item as DashboardLayoutControlGroupBase;
			object result = null;
			if(liContainer != null) result = CalculateContainerDefaultAppearance(liContainer);
			if(dli != null) {
				result = CalculateDashboardItemDefaultAppearance(dli);
			} else if(dlg != null) {
				result = CalculateDashboardGroupDefaultAppearance(dlg);
			} else {
				if(lci != null) result = CalculateItemDefaultAppearance(lci);
			}
			if(result != null) {
				if(defaultAppearances.ContainsKey(item)) defaultAppearances.Remove(item);
				defaultAppearances.Add(item, result);
			}
			return result;
		}
		protected virtual object CalculateDashboardGroupDefaultAppearance(DashboardLayoutControlGroupBase group) {
			SkinElementInfo seli;
			Skin skin = DashboardSkins.GetSkin(controller.Owner.LookAndFeel);
			SkinElement element = null;
			if(skin != null)
				element = skin[DashboardSkins.SkinDashboardItemCaptionTop];
			if(element == null)
				element = CommonSkins.GetSkin(controller.Owner.LookAndFeel)[CommonSkins.SkinLabel];
			seli = new SkinElementInfo(element, Rectangle.Empty);
			return CalculateItemDefaultAppearanceCore(group, seli);
		}
		protected virtual object CalculateDashboardItemDefaultAppearance(DashboardLayoutControlItemBase item) {
			SkinElementInfo seli; 
			Skin skin = DashboardSkins.GetSkin(controller.Owner.LookAndFeel);
			SkinElement element  = null;
			if(skin != null)
				element = skin[DashboardSkins.SkinDashboardItemCaptionTop];
			if(element == null)
				element = CommonSkins.GetSkin(controller.Owner.LookAndFeel)[CommonSkins.SkinLabel];		   
			seli = new SkinElementInfo(element, Rectangle.Empty);
			return CalculateItemDefaultAppearanceCore(item, seli);
		}
		protected virtual object CalculateItemDefaultAppearance(BaseLayoutItem item) {
			SkinElementInfo seli = new SkinElementInfo(CommonSkins.GetSkin(controller.Owner.LookAndFeel)[CommonSkins.SkinLabel], Rectangle.Empty);
			return CalculateItemDefaultAppearanceCore(item, seli);
		}
		protected virtual object CalculateItemDefaultAppearanceCore(BaseLayoutItem item, SkinElementInfo seli) {
			PaintingType pt = (controller.Owner as ISupportImplementor).Implementor.PaintingType;
			Color foreColor = SystemColors.WindowText;
			AppearanceDefault skinElementDefaultAppearance = null;
			if(pt == PaintingType.Skinned) {
				if(seli != null) {
					skinElementDefaultAppearance = seli.Element.GetAppearanceDefault();
					foreColor = skinElementDefaultAppearance.ForeColor;
				}
					ITransparentBackgroundManager itbm = controller.Owner as ITransparentBackgroundManager;
					if(itbm == null)
						foreColor = LookAndFeelHelper.GetTransparentForeColor(controller.Owner.LookAndFeel, controller.Owner.Control);
					else foreColor = itbm.GetForeColor(item);
			}
			AppearanceDefault result = new AppearanceDefault(foreColor, Color.Empty);
			if(skinElementDefaultAppearance != null) result.Font = skinElementDefaultAppearance.Font;
			return result;
		}
		protected virtual object CalculateGroupCaptionDefaultAppearance(BaseLayoutItem item) {
			AppearanceDefault temp = new AppearanceDefault();
			BaseViewInfo e = new BaseLayoutItemViewInfo(item);
			LayoutGroup group = item is LayoutGroup ? item as LayoutGroup : GetLayoutGroup(item.Owner);
			temp.Assign(((GroupObjectPainter)controller.Owner.PaintStyle.GetPainter(group).GetBorderPainter(e)).DefaultAppearanceCaption);
			return temp;
		}
		protected TabbedGroup GetTabbedGroup(ILayoutControl owner) {
			if(owner != null && (tabbedGroupCore == null || tabbedGroupCore.Owner != owner)) { tabbedGroupCore = owner.CreateTabbedGroup(null); tabbedGroupCore.Owner = owner; }
			return tabbedGroupCore;
		}
		protected LayoutGroup GetLayoutGroup(ILayoutControl owner) {
			if(owner != null && (layoutGroupCore == null || layoutGroupCore.Owner != owner)) { layoutGroupCore = owner.CreateLayoutGroup(null); layoutGroupCore.Owner = owner; }
			return layoutGroupCore;
		}
		protected LayoutGroupDefaultAppearance CalculateTabbedGroupAppearanceDefault(BaseLayoutItem item) {
			LayoutGroupDefaultAppearance resultAppearance = new LayoutGroupDefaultAppearance();
			if(item == tabbedGroupCore) return resultAppearance;
			resultAppearance.Header = GetTabbedGroup(item.Owner).ViewInfo.BorderInfo.Tab.ViewInfo.GetPageHeaderAppearanceByState(ObjectState.Normal);
			resultAppearance.HeaderActive = GetTabbedGroup(item.Owner).ViewInfo.BorderInfo.Tab.ViewInfo.GetPageHeaderAppearanceByState(ObjectState.Selected);
			resultAppearance.HeaderDisabled = GetTabbedGroup(item.Owner).ViewInfo.BorderInfo.Tab.ViewInfo.GetPageHeaderAppearanceByState(ObjectState.Disabled);
			resultAppearance.HeaderHotTracked = GetTabbedGroup(item.Owner).ViewInfo.BorderInfo.Tab.ViewInfo.GetPageHeaderAppearanceByState(ObjectState.Hot);
			return resultAppearance;
		}
		protected AppearanceDefault CalculateGroupAppearanceDefault(BaseLayoutItem item) {
			AppearanceDefault temp = new AppearanceDefault();
			if(item == layoutGroupCore) return temp;
			temp.Assign(item.Owner.PaintStyle.GetPainter(layoutGroupCore).DefaultAppearance);
			CheckAndRemoveTransparentBackColor(temp, item);
			return temp;
		}
		protected void CheckAndRemoveTransparentBackColor(AppearanceDefault ad, BaseLayoutItem item) {
			ad.BackColor = SystemColors.Control;
			PaintingType pt = (controller.Owner as ISupportImplementor).Implementor.PaintingType;
			if(pt == PaintingType.Skinned || (pt == PaintingType.XP ))
				ad.BackColor = Color.Transparent;
		}
		protected virtual object CalculateContainerDefaultAppearance(LayoutItemContainer container) {
			LayoutGroupDefaultAppearance resultAppearance = new LayoutGroupDefaultAppearance();
			AppearanceDefault temp = new AppearanceDefault();
			AppearanceDefault temp1 = CalculateGroupAppearanceDefault(container);
			LayoutGroupDefaultAppearance temp11 = CalculateTabbedGroupAppearanceDefault(container);
			temp.BackColor = Color.Transparent;
			temp.ForeColor = temp1.ForeColor;
			if((controller.Owner as ISupportImplementor).Implementor.PaintingType == PaintingType.Skinned)
				resultAppearance.Header = temp11.Header;
			else
				resultAppearance.Header = temp;
			resultAppearance.HeaderActive = temp11.HeaderActive;
			resultAppearance.HeaderDisabled = temp11.HeaderDisabled;
			resultAppearance.HeaderHotTracked = temp11.HeaderHotTracked;
			resultAppearance.AppearanceGroup = temp1;
			resultAppearance.AppearanceItemCaption = CalculateGroupCaptionDefaultAppearance(container) as AppearanceDefault;
			resultAppearance.PageClient = temp;
			return resultAppearance;
		}
		protected void ResetHelperElements() {
			if(layoutGroupCore!=null){
				if(defaultAppearances.Contains(layoutGroupCore)) {
					object groupAp = defaultAppearances[layoutGroupCore];
					defaultAppearances.Remove(layoutGroupCore);
					if(groupAp is IDisposable) ((IDisposable)groupAp).Dispose();
				}
				layoutGroupCore.Owner.EnabledStateController.SetItemEnabledStateDirty(layoutGroupCore);
				layoutGroupCore.Owner = null;
				layoutGroupCore.Dispose();
				layoutGroupCore = null;
			}
			if(tabbedGroupCore != null) {
				if(defaultAppearances.Contains(tabbedGroupCore)) {
					object tabAp = defaultAppearances[tabbedGroupCore];
					defaultAppearances.Remove(tabbedGroupCore);
					if(tabAp is IDisposable) ((IDisposable)tabAp).Dispose();
				}
				tabbedGroupCore.Owner.EnabledStateController.SetItemEnabledStateDirty(tabbedGroupCore);
				tabbedGroupCore.Dispose();
				tabbedGroupCore = null;
			}
		}
		public void SetDefaultAppearanceDirty(BaseLayoutItem item) {
			ResetHelperElements();
			List<BaseLayoutItem> list = flatter.GetItemsList(item);
			foreach(BaseLayoutItem tItem in list) 
				RemoveCore(tItem);
			flatter.Clear();
		}
		protected internal void ClearReferences(BaseLayoutItem item) {
			RemoveCore(item);
		}
		void RemoveCore(BaseLayoutItem item) {
			object app = defaultAppearances[item];
			if(app != null) {
				defaultAppearances.Remove(item);
				IDisposable dApp = app as IDisposable;
				if(dApp != null) dApp.Dispose();
			}
		}
	}
	public class AppearanceController : IDisposable{
		ILayoutControl ownerCore;
		Hashtable appearances;
		AppearanceHarvester appearanceHarvester;
		DefaultAppearancesController defaultAppearanceController;
		FlatItemsList flatter;
		public AppearanceController(ILayoutControl owner) {
			this.ownerCore = owner;
			this.appearances = new Hashtable();
			this.defaultAppearanceController = new DefaultAppearancesController(this);
			this.appearanceHarvester = new AppearanceHarvester();
			this.flatter = new FlatItemsList();
		}
		public virtual void Dispose() {
			ownerCore = null;
			flatter = null;
			appearanceHarvester = null;
			if(appearances != null) {
				foreach(object a in appearances) {
					if(a is IDisposable) ((IDisposable)a).Dispose();
				}
				appearances.Clear();
				appearances = null;
			}
			if(defaultAppearanceController != null) {
				defaultAppearanceController.Dispose();
				defaultAppearanceController = null;
			}
		}
		public ILayoutControl Owner {
			get { return ownerCore; }
		}
		public object GetAppearanceCore(BaseLayoutItem item) {
			return (appearances.Contains(item)) ? appearances[item] : CalculateAppearance(item);
		}
		public AppearanceObject GetAppearanceItem(BaseLayoutItem item) {
			return GetAppearanceCore(item) as AppearanceObject;
		}
		public LayoutGroupAppearance GetAppearanceItemContainer(BaseLayoutItem item) {
			return GetAppearanceCore(item) as LayoutGroupAppearance;
		}
		protected virtual object CalculateAppearance(BaseLayoutItem item) {
			object result = null;
			try {
				LayoutItemContainer liContainer = item as LayoutItemContainer;
				LayoutControlItem lci = item as LayoutControlItem;
				if(lci != null) result = CalculateAppearanceItemCaption(item);
				if(liContainer != null) result = CalculateGroupFullAppearance(liContainer);
			}
			finally {
				if(appearances.ContainsKey(item)) appearances.Remove(item);
				appearances.Add(item, result);
			}
			return result;
		}
		protected virtual LayoutGroupAppearance CalculateGroupFullAppearance(LayoutItemContainer container) {
			LayoutGroupAppearance groupAppearance = new LayoutGroupAppearance();
			groupAppearance.AppearanceGroup = CalculateGroupAppearance(container);
			groupAppearance.AppearanceItemCaption = CalculateAppearanceGroupCaption(container);
			using(FrozenAppearance tempAppearance = new FrozenAppearance()) {
				LayoutGroupDefaultAppearance groupDefaultAppearance = defaultAppearanceController.GetDefaultAppearanceGroup(container);
				AppearanceHelper.Combine(tempAppearance, AppearanceArray(container, GroupAppearanceType.AppearanceTabPageHeader), groupDefaultAppearance.Header);
				if(!tempAppearance.IsEqual(AppearanceObject.EmptyAppearance))
					groupAppearance.Header.Assign(tempAppearance);
				AppearanceHelper.Combine(tempAppearance, AppearanceArray(container, GroupAppearanceType.AppearanceTabPageHeaderActive), groupDefaultAppearance.HeaderActive);
				if(!tempAppearance.IsEqual(AppearanceObject.EmptyAppearance))
					groupAppearance.HeaderActive.Assign(tempAppearance);
				AppearanceHelper.Combine(tempAppearance, AppearanceArray(container, GroupAppearanceType.AppearanceTabPageHeaderDisabled), groupDefaultAppearance.HeaderDisabled);
				if(!tempAppearance.IsEqual(AppearanceObject.EmptyAppearance))
					groupAppearance.HeaderDisabled.Assign(tempAppearance);
				AppearanceHelper.Combine(tempAppearance, AppearanceArray(container, GroupAppearanceType.AppearanceTabPageHeaderHotTracked), groupDefaultAppearance.HeaderHotTracked);
				if(!tempAppearance.IsEqual(AppearanceObject.EmptyAppearance))
					groupAppearance.HeaderHotTracked.Assign(tempAppearance);
				AppearanceHelper.Combine(tempAppearance, AppearanceArray(container, GroupAppearanceType.AppearanceTabPagePageClient), groupDefaultAppearance.PageClient);
				if(!tempAppearance.IsEqual(AppearanceObject.EmptyAppearance))
					groupAppearance.PageClient.Assign(tempAppearance);
				if(!container.EnabledState) {
					Color disabledForeColorGroup = GetDisabledColor(Owner.Appearance.DisabledLayoutGroupCaption.ForeColor);
					groupAppearance.AppearanceGroup.ForeColor = disabledForeColorGroup;
					groupAppearance.AppearanceItemCaption.ForeColor = disabledForeColorGroup;
					AppearanceHelper.Combine(tempAppearance, groupAppearance.HeaderDisabled, Owner.Appearance.DisabledLayoutGroupCaption);
					groupAppearance.Header.Assign(tempAppearance);
					groupAppearance.HeaderActive.Assign(tempAppearance);
					groupAppearance.HeaderHotTracked.Assign(tempAppearance);
				}
			}
			return groupAppearance;
		}
		protected Color GetDisabledColor(Color proposedColor) {
			if(proposedColor == Color.FromArgb(0, 0, 0, 0)) proposedColor = SystemColors.GrayText;
			proposedColor = LookAndFeelHelper.GetSystemColor(Owner.LookAndFeel, proposedColor);
			return proposedColor;
		}
		protected virtual AppearanceObject CalculateGroupAppearance(LayoutItemContainer container) {
			AppearanceObject groupAppearanceObj = new AppearanceObject();
			AppearanceHelper.Combine(groupAppearanceObj, AppearanceArray(container, GroupAppearanceType.AppearanceGroup), defaultAppearanceController.GetDefaultAppearanceGroup(container).AppearanceGroup);
			return groupAppearanceObj;
		}
		protected virtual AppearanceObject CalculateAppearanceGroupCaption(LayoutItemContainer container) {
			AppearanceObject appCaption = new AppearanceObject();
			AppearanceHelper.Combine(appCaption, AppearanceArray(container, GroupAppearanceType.AppearanceGroup), defaultAppearanceController.GetDefaultAppearanceGroup(container).AppearanceItemCaption);
			return appCaption;
		}
		private AppearanceDefault GetAppearanceItemCaptionForLayoutGroup(BaseLayoutItem container) {
			var appearanceItemCaption = defaultAppearanceController.GetDefaultAppearanceItem(container);
			var group = container.Parent as LayoutGroup;
			if(group != null && group.LayoutMode == LayoutMode.Flow) {
				appearanceItemCaption.HAlignment = HorzAlignment.Far;
			}
			return appearanceItemCaption;
		}
		protected virtual AppearanceObject CalculateAppearanceItemCaption(BaseLayoutItem bItem) {
			AppearanceObject paintAppearanceItemCaption = new AppearanceObject();
			LayoutItemContainer container = bItem as LayoutItemContainer;
			AppearanceDefault appDefault = container == null ? GetAppearanceItemCaptionForLayoutGroup(bItem) : defaultAppearanceController.GetDefaultAppearanceGroup(container).AppearanceItemCaption;
			AppearanceHelper.Combine(paintAppearanceItemCaption, AppearanceArray(bItem, GroupAppearanceType.AppearanceItemCaption), appDefault);
			if(paintAppearanceItemCaption.TextOptions.HotkeyPrefix == HKeyPrefix.Default) paintAppearanceItemCaption.TextOptions.HotkeyPrefix = HKeyPrefix.Show;
			if (!bItem.EnabledState) paintAppearanceItemCaption.ForeColor = GetDisabledColor(GetDisabledColor(Owner.Appearance.DisabledLayoutItem.ForeColor));
			return paintAppearanceItemCaption;
		}
		public void SetAppearanceDirty(BaseLayoutItem item) {
			List<BaseLayoutItem> affectedItems = flatter.GetItemsList(item);
			foreach(BaseLayoutItem bItem in affectedItems) 
				RemoveCore(bItem);
			flatter.Clear();
		}
		void RemoveCore(BaseLayoutItem item) {
			object app = appearances[item];
			if(app != null) {
				appearances.Remove(item);
				IDisposable dApp = app as IDisposable;
				if(dApp != null) dApp.Dispose();
			}
		}
		public void SetDefaultAppearanceDirty(BaseLayoutItem item) {
			defaultAppearanceController.SetDefaultAppearanceDirty(item);
			SetAppearanceDirty(item);
		}
		protected AppearanceObject[] AppearanceArray(BaseLayoutItem item, GroupAppearanceType appearanceType) {
			return appearanceHarvester.GetAppearanceItemCaptionArray(item, appearanceType);
		}
		protected internal void ClearReferences(BaseLayoutItem item) {
			defaultAppearanceController.ClearReferences(item);
			RemoveCore(item);
		}
	}
	public class AppearanceHarvester {
		protected BaseLayoutItem GetNextParent(BaseLayoutItem item) {
			LayoutGroup group = item as LayoutGroup;
			if (group != null && group.ParentTabbedGroup != null) {
				return group.ParentTabbedGroup;
			}
			return item.Parent;
		}
		public virtual AppearanceObject GetAppearance(BaseLayoutItem item, GroupAppearanceType appearanceType) {
			LayoutItemContainer container = item as LayoutItemContainer;
			if (container == null)
				return item.AppearanceItemCaption;
			else {
				switch (appearanceType) {
					case GroupAppearanceType.AppearanceGroup:
						return container.AppearanceGroup;
					case GroupAppearanceType.AppearanceItemCaption:
						return container.AppearanceItemCaption;
					case GroupAppearanceType.AppearanceTabPageHeader:
						return container.AppearanceTabPage.Header;
					case GroupAppearanceType.AppearanceTabPageHeaderActive:
						return container.AppearanceTabPage.HeaderActive;
					case GroupAppearanceType.AppearanceTabPageHeaderDisabled:
						return container.AppearanceTabPage.HeaderDisabled;
					case GroupAppearanceType.AppearanceTabPageHeaderHotTracked:
						return container.AppearanceTabPage.HeaderHotTracked;
					case GroupAppearanceType.AppearanceTabPagePageClient:
						return container.AppearanceTabPage.PageClient;
				}
				return null;
			}
		}
		public virtual AppearanceObject[] GetAppearanceItemCaptionArray(BaseLayoutItem item, GroupAppearanceType appearanceType) {
			ArrayList list = new ArrayList();
			BaseLayoutItem currentItem = item;
			while (currentItem != null) {
				list.Add(GetAppearance(currentItem, appearanceType));
				currentItem = GetNextParent(currentItem);
			}
			return (AppearanceObject[])list.ToArray(typeof(AppearanceObject));
		}
	}
	public class FlatItemsList : BaseVisitor {
		protected List<BaseLayoutItem> list = new List<BaseLayoutItem>();
		public virtual List<BaseLayoutItem> GetItemsList(BaseLayoutItem item) {
			list.Clear();
			item.Accept(this);
			return list;
		}
		public void Clear() { list.Clear(); }
		public override void Visit(BaseLayoutItem item) {
			list.Add(item);
		}
	}
	public class SelectionHelper : FlatItemsList {
		public override void Visit(BaseLayoutItem item) {
			if (item.Selected) base.Visit(item);
		}
	}
}
