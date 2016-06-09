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
using System.IO;
using System.Drawing;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Adapters;
using DevExpress.XtraLayout.Scrolling;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Customization.UserCustomization;
namespace DevExpress.XtraLayout {
	#region XtraLayout Enums
	public enum SizeConstraintsType { Default, SupportHorzAlignment, Custom };
	public enum LayoutControlRoles { MainControl, ClonedControl, CustomizationFormControl }
	public enum PaintingType { Normal, Skinned, XP }
	#endregion
	#region XtraLayout Constants
	public class LayoutControlConstants {
		public const string LayoutControlDesignerName = "DevExpress.XtraLayout.DesignTime.LayoutControlDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
		public const string LayoutConverterDesignerName = "DevExpress.XtraLayout.DesignTime.LayoutConverterDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
		public const string WinRTLiveTileManagerDesignerName = "DevExpress.XtraLayout.DesignTime.WinRTLiveTileManagerDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
		public const string LayoutControlItemDesignerName = "DevExpress.XtraLayout.DesignTime.LayoutControlItemDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
		public const string LayoutControlGroupDesignerName = "DevExpress.XtraLayout.DesignTime.LayouytControlGroupDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
		public const string TabbedGroupDesignerName = "DevExpress.XtraLayout.DesignTime.TabbedGroupDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
		public const string DataLayoutControlDesignerName = "DevExpress.XtraDataLayout.DesignTime.DataLayoutDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
		public const string HiddenItemsListDesignerName = "DevExpress.XtraLayout.HiddenItemsListDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
		public const string LayoutTreeViewDesignerName = "DevExpress.XtraLayout.LayoutTreeViewDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
		public const string CustomizationPropertyGridDesignerName = "DevExpress.XtraLayout.CustomizationPropertyGridDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
		public const string ButtonsPanelDesignerName = "DevExpress.XtraLayout.ButtonsPanelDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign;
	}
		#endregion
	#region XtraLayout Interfaces
	public interface ILayoutDesignerMethods {
		void BeginChangeUpdate();
		void EndChangeUpdate();
		bool AllowHandleEvents { get;set;}
		bool AllowHandleControlRemovedEvent { get;set;}
		LayoutControlHandler RuntimeHandler { get;set;}
		bool IsInternalControl(Control control);
		bool AllowDisposeControlOnItemDispose{get; set;}
		void ResetResizer();
		bool CanInvokePainting { get;}
		void RecreateHandle();
		LayoutControlDragDropHelper DragDropDispatcherClientHelper { get; }
		event DeleteSelectedItemsEventHandler DeleteSelectedItems;
		void RaiseDeleteSelectedItems(DeleteSelectedItemsEventArgs e);
	}
	public delegate void DeleteSelectedItemsEventHandler(object sender, DeleteSelectedItemsEventArgs e);
	public class DeleteSelectedItemsEventArgs : EventArgs {
		public BaseItemCollection Collection { get; set; }
		public DeleteSelectedItemsEventArgs(BaseItemCollection collection) {
			this.Collection = collection;
		}
	}
	public interface ILayoutControl {
		event EventHandler Changed;
		event CancelEventHandler Changing;
		event EventHandler ItemSelectionChanged;
		Size Size { get;set;}
		int Width { get;set;}
		int Height { get;set;}
		int ClientWidth { get;}
		int ClientHeight { get;}
		Rectangle Bounds { get;}
		Control Parent { get; set; }
		ISite Site { get; set; }
		bool IsDeserializing { get;set;}
		bool IsSerializing { get;}
		bool AllowPaintEmptyRootGroupText { get;set;}
		bool IsUpdateLocked { get;}
		bool AllowManageDesignSurfaceComponents { get; set;}
		bool AllowCustomizationMenu { get; set;}
		CustomizationModes CustomizationMode { get; set; }
		bool DesignMode { get;}
		bool SelectedChangedFlag { get; set;}
		bool EnableCustomizationMode { get; set;}
		bool AllowSetIsModified { get; set;}
		bool EnableCustomizationForm { get; set;}
		bool DisposingFlag { get;}
		LongPressControl LongPressControl { get; }
		bool InitializationFinished { get;set;}
		bool ExceptionsThrown { get;set;}
		bool ShouldResize { get;set;}
		bool ShouldArrangeTextSize { get;set;}
		bool ShouldUpdateConstraints { get;set;}
		bool ShouldUpdateLookAndFeel { get;set;}
		bool ShouldUpdateControls { get;set;}
		bool ShouldUpdateControlsLookAndFeel { get;set;}
		bool LockUpdateOnChangeUICuesRequest { get; set; }
		void OnChangeUICues();
		int DelayPainting { get;set;}
		int SelectedChangedCount { get; set;}
		int UpdatedCount { get; set;}
		SizeF AutoScaleFactor { get;}
		LayoutControlDefaultsPropertyBag DefaultValues { get; set;}
		LayoutControlGroup RootGroup { get; set; }
		ReadOnlyItemCollection Items { get; set;}
		Dictionary<string, BaseLayoutItem> ItemsAndNames { get; set;}
		Dictionary<Control, LayoutControlItem> ControlsAndItems { get; set;}
		List<IComponent> Components { get;}
		LayoutPaintStyle PaintStyle { get; }
		HiddenItemsCollection HiddenItems { get; }
		RightButtonMenuManager CustomizationMenuManager { get; }
		UserCustomizationForm CustomizationForm { get;}
		Control Control { get;}
		ScrollInfo Scroller { get;}
		LayoutControlRoles ControlRole { get;set;}
		SerializeableUserLookAndFeel LookAndFeel { get;}
		LayoutSerializationOptions OptionsSerialization { get;}
		LayoutAppearanceCollection Appearance { get;}
		AppearanceController AppearanceController { get;set;}
		EnabledStateController EnabledStateController { get;set;}
		LayoutControlHandler ActiveHandler { get;}
		UserInteractionHelper UserInteractionHelper { get; }
		EditorContainer GetEditorContainer();
		FocusHelperBase FocusHelper { get;set;}
		ConstraintsManager ConstraintsManager { get;}
		FakeFocusContainer FakeFocusContainer { get;}
		TextAlignManager TextAlignManager { get;}
		LayoutGroupHandlerWithTabHelper CreateRootHandler(LayoutGroup group);
		OptionsView OptionsView { get;}
		OptionsItemText OptionsItemText { get;}
		OptionsFocus OptionsFocus { get;}
		bool ShowKeyboardCues { get;}
		OptionsCustomizationForm OptionsCustomizationForm { get;}
		IDXMenuManager MenuManager { get;set;}
		UndoManager UndoManager { get;}
		IComparer HiddenItemsSortComparer { get;set;}
		void BestFit();
		void RegisterLayoutAdapter(ILayoutAdapter adapter);
		void RaiseGroupExpandChanging(LayoutGroupCancelEventArgs e);
		void RaiseGroupExpandChanged(LayoutGroupEventArgs e);
		void RaiseShowCustomizationMenu(PopupMenuShowingEventArgs ma);
		void RaiseShowLayoutTreeViewContextMenu(PopupMenuShowingEventArgs ma);
		bool FireChanging(IComponent component);
		void FireChanged(IComponent component);
		void FireItemAdded(BaseLayoutItem component, EventArgs ea);
		void FireItemRemoved(BaseLayoutItem component, EventArgs ea);
		void FireCloseButtonClick(LayoutGroup component);
		void SetIsModified(bool newVal);		
		void SelectionChanged(IComponent component);
		void AddComponent(BaseLayoutItem component);
		void RemoveComponent(BaseLayoutItem component);
		void BeginUpdate();
		void EndUpdate();
		void BeginInit();
		void EndInit();
		void Invalidate();
		void SetControlDefaults();
		void SetControlDefaultsLast();
		void ShowCustomizationForm();
		void HideCustomizationForm();
		void HideSelectedItems();
		void SelectParentGroup();
		void RestoreDefaultLayout();
		void RebuildCustomization();
		void UpdateChildControlsLookAndFeel();
		void UpdateFocusedElement(BaseLayoutItem item);
		void SetCursor(Cursor cursor);
		void SaveLayoutToXml(string xmlFile);
		void RestoreLayoutFromXml(string xmlFile);
		void RestoreLayoutFromStream(Stream stream);
		void SaveLayoutToStream(Stream stream);
		void RestoreLayoutFromRegistry(string path);
		void SaveLayoutToRegistry(string path);
		LayoutGroup CreateLayoutGroup(LayoutGroup parent);
		BaseLayoutItem CreateLayoutItem(LayoutGroup parent);
		TabbedGroup CreateTabbedGroup(LayoutGroup parent);
		EmptySpaceItem CreateEmptySpaceItem(LayoutGroup parent);
		SplitterItem CreateSplitterItem(LayoutGroup parent);
		object Images { get;set;}
		bool CheckIfControlIsInLayout(Control control);
		LayoutGroup GetGroupAtPoint(Point point);
		LayoutControlItem GetItemByControl(Control control);
		bool IsHidden(BaseLayoutItem item);
		string GetUniqueName(BaseLayoutItem item);
		LayoutStyleManager LayoutStyleManager { get;}
		void RaiseSizeableChanged();
	}
	public interface ILayoutItemOwner {
		ILayoutControl LayoutControl { get; }
		void Invalidate();
		void AddComponent(BaseLayoutItem component);
		void RemoveComponent(BaseLayoutItem component);
		void SelectionChanged(IComponent component);
	}
	#endregion
	#region XtraLayout Collections
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LayoutAppearanceCollection : BaseAppearanceCollection {
		AppearanceObject control, controlDisabled, controlFocused, controlDropDown, controlDropDownHeader, layoutItemDisabled, layoutGroupDisabled, readonlyCore;
		protected override void CreateAppearances() {
			this.control = CreateAppearance("Control");
			this.controlDisabled = CreateAppearance("ControlDisabled");
			this.controlFocused = CreateAppearance("ControlFocused");
			this.controlDropDown = CreateAppearance("ControlDropDown");
			this.controlDropDownHeader = CreateAppearance("ControlDropDownHeader");
			this.layoutItemDisabled = CreateAppearance("DisabledLayoutItem");
			this.layoutGroupDisabled = CreateAppearance("DisabledLayoutGroupCaption");
			this.readonlyCore = CreateAppearance("ControlReadOnly");
		}
		protected override AppearanceObject CreateAppearance(string name, AppearanceObject parent) {
			AppearanceObject baseResult = base.CreateAppearance(name, parent);
			if(name == "DisabledLayoutItem" || name == "DisabledLayoutGroupCaption")
				baseResult.ForeColor = SystemColors.GrayText;
			return baseResult;
		}
		bool ShouldSerializeDisabledLayoutItem() { return DisabledLayoutItem.ForeColor != SystemColors.GrayText; }
		void ResetDisabledLayoutItem() { DisabledLayoutItem.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutAppearanceCollectionDisabledLayoutItem"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DisabledLayoutItem { get { return layoutItemDisabled; } }
		bool ShouldSerializeDisabledLayoutGroupCaption() { return DisabledLayoutGroupCaption.ForeColor != SystemColors.GrayText; }
		void ResetDisabledLayoutGroupCaption() { DisabledLayoutGroupCaption.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutAppearanceCollectionDisabledLayoutGroupCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject DisabledLayoutGroupCaption { get { return layoutGroupDisabled; } }
		bool ShouldSerializeControl() { return Control.ShouldSerialize(); }
		void ResetControl() { Control.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutAppearanceCollectionControl"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Control { get { return control; } }
		bool ShouldSerializeControlReadOnly() { return ControlReadOnly.ShouldSerialize(); }
		void ResetControlReadOnly() { ControlReadOnly.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutAppearanceCollectionControlReadOnly"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ControlReadOnly { get { return readonlyCore; } }
		bool ShouldSerializeControlDisabled() { return ControlDisabled.ShouldSerialize(); }
		void ResetControlDisabled() { ControlDisabled.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutAppearanceCollectionControlDisabled"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ControlDisabled { get { return controlDisabled; } }
		bool ShouldSerializeControlFocused() { return ControlFocused.ShouldSerialize(); }
		void ResetControlFocused() { ControlFocused.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutAppearanceCollectionControlFocused"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ControlFocused { get { return controlFocused; } }
		bool ShouldSerializeControlDropDown() { return ControlDropDown.ShouldSerialize(); }
		void ResetControlDropDown() { ControlDropDown.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutAppearanceCollectionControlDropDown"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ControlDropDown { get { return controlDropDown; } }
		bool ShouldSerializeControlDropDownHeader() { return ControlDropDownHeader.ShouldSerialize(); }
		void ResetControlDropDownHeader() { ControlDropDownHeader.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutAppearanceCollectionControlDropDownHeader"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ControlDropDownHeader { get { return controlDropDownHeader; } }
	}
	public class LayoutControlCollection : Control.ControlCollection {
		public LayoutControlCollection(Control owner) : base(owner) { }
		public override void Clear() {
			if(Count == 4) return;
			int count = Count;
			for(int i = 4; i < count; i++) {
				Remove(this[4]);
			}
		}
	}
	public interface IFixedLayoutControlItem {
		string TypeName { get;}
		Control OnCreateControl();
		void OnInitialize();
		void OnDestroy();
		ILayoutControl Owner { get; set;}
		string CustomizationName { get;}
		Image CustomizationImage { get;}
		bool AllowClipText { get;}
		bool AllowChangeTextVisibility { get;}
		bool AllowChangeTextLocation { get;}
	}
	[ListBindable(BindableSupport.No)]
	public class HiddenItemsCollection : BaseItemCollection {
		ILayoutControl ownerControlCore = null;
		List<Type> fixedTypesCore = null;
		List<IFixedLayoutControlItem> fixedItemsCore;
		public HiddenItemsCollection(ILayoutControl owner) {
			this.ownerControlCore = owner;
			InitializeList();
		}
		public HiddenItemsCollection(){
			InitializeList();
		}
		protected ILayoutControl OwnerControl { get { return ownerControlCore; } }
		protected internal List<Type> FixedTypes {
			get {
				if(fixedTypesCore==null) fixedTypesCore=new List<Type>();
				return fixedTypesCore; 
			}
		}
		public List<IFixedLayoutControlItem> FixedItems {
			get {
				if(fixedItemsCore==null) fixedItemsCore = CreateFixedItems();
				return fixedItemsCore;
			}
		}
		[Obsolete("Use HiddenItemsCollection.FixedItemsCount instead")]
		public int PhantomItemCount { get { return FixedItemsCount; } }
		public int FixedItemsCount { get { return FixedItems.Count; } }
		protected virtual void InitializeFixedItems() {
			foreach(IFixedLayoutControlItem existingItem in FixedItems) {
				LayoutControlItem.DestroyItemAsFixed(existingItem as LayoutControlItem);
			}
			FixedItems.Clear();
			foreach(Type t in FixedTypes) {
				IFixedLayoutControlItem item = CreateFixedItemByType(t);
				FixedItems.Add(item);
			}
			OnChanged(null);
		}
		protected virtual IFixedLayoutControlItem CreateFixedItemByType(Type t) {
			IFixedLayoutControlItem instance = Activator.CreateInstance(t, false) as IFixedLayoutControlItem;
			if(instance!=null) {
				instance.Owner = OwnerControl;
			}
			return instance;
		}
		protected virtual List<IFixedLayoutControlItem> CreateFixedItems() {
			return new List<IFixedLayoutControlItem>();
		}
		protected internal void DestroyCollection() {
			if(fixedItemsCore == null) return;
			foreach(IDisposable item in fixedItemsCore) {
				item.Dispose();
			}
			fixedItemsCore.Clear();
			fixedItemsCore = null;
		}
		int iRegCount=0;
		protected internal bool IsUpdateLocked { get { return iRegCount>0; } }
		public void BeginRegistration() { iRegCount++; }
		public void EndRegistration() {
			if(--iRegCount == 0) {
				ArrayList list = new ArrayList(InnerList);
				Clear();
				if(list.Count > 0) AddRange((BaseLayoutItem[])list.ToArray(typeof(BaseLayoutItem)));
			}
		}
		protected internal void RegisterFixedItemType(Type itemType) {
			if(itemType==null || itemType.GetInterface("IFixedLayoutControlItem")==null) return;
			BeginRegistration();
			if(!FixedTypes.Contains(itemType)) FixedTypes.Add(itemType);
			EndRegistration();
		}
		protected internal void UnRegisterFixedItemType(Type itemType) {
			BeginRegistration();
			if(FixedTypes.Contains(itemType)) FixedTypes.Remove(itemType);
			EndRegistration();
		}
		protected virtual void InitializeList() {
			if(IsUpdateLocked) return;
			InitializeFixedItems();
		}
		public new void Clear() {
			base.Clear();
			InitializeList();
		}
		protected internal override void Remove(BaseLayoutItem item) {
			if(item is IFixedLayoutControlItem) OnRemoveFixedItem(item);
			else OnRemoveItem(item);
		}
		protected override internal void Add(BaseLayoutItem item) {
			if(item is IFixedLayoutControlItem) OnAddFixedItem(item);
			else OnAddItem(item);
		}
		protected virtual void OnAddItem(BaseLayoutItem item) {
			base.Add(item);
			if(item.Owner == null) item.Owner = OwnerControl;
		}
		protected virtual void OnAddFixedItem(BaseLayoutItem item) {
			LayoutControlItem.DestroyItemAsFixed(item as LayoutControlItem);
		}
		protected virtual void OnRemoveItem(BaseLayoutItem item) {
			base.Remove(item);
		}
		protected virtual void OnRemoveFixedItem(BaseLayoutItem item) {
			Type itemType = item.GetType();
			IFixedLayoutControlItem oldInstance = item as IFixedLayoutControlItem;
			IFixedLayoutControlItem newInstance = CreateFixedItemByType(itemType);
			int iPosInFixedItems = FixedItems.IndexOf(oldInstance);
			if(iPosInFixedItems>=0) {
				FixedItems.Remove(oldInstance);
				FixedItems.Insert(iPosInFixedItems, newInstance);
				OnChanged(null);
			}
		}
		public void Assign(HiddenItemsCollection hiddenItems) {
			BaseLayoutItem[] existingItems = ToArray();
			InnerList.Clear();
			foreach(BaseLayoutItem existingItem in existingItems) {
				existingItem.Dispose();
			}
			InnerList.AddRange(Clone(hiddenItems));
		}
		BaseLayoutItem[] ToArray() {
			BaseLayoutItem[] result = new BaseLayoutItem[InnerList.Count];
			InnerList.CopyTo(result, 0);
			return result;
		}
		BaseLayoutItem[] Clone(HiddenItemsCollection hiddenItems) {
			BaseLayoutItem[] result = new BaseLayoutItem[hiddenItems.Count];
			for(int i = 0; i < result.Length; i++)
				result[i] = hiddenItems[i].Clone(null, OwnerControl);
			return result;
		}
	}
	#endregion
	#region XtraLayout Others
	public class SerializeableUserLookAndFeel : DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel {
		public SerializeableUserLookAndFeel(Control owner) : base(owner) { }
		[XtraSerializableProperty()]
		public override string SkinName { get { return base.SkinName; } set { base.SkinName = value; } }
		[XtraSerializableProperty()]
		public override LookAndFeelStyle Style { get { return base.Style; } set { base.Style = value; } }
		[XtraSerializableProperty()]
		public override bool UseDefaultLookAndFeel { get { return base.UseDefaultLookAndFeel; } set { base.UseDefaultLookAndFeel = value; } }
		[XtraSerializableProperty()]
		public override bool UseWindowsXPTheme { get { return base.UseWindowsXPTheme; } set { base.UseWindowsXPTheme = value; } }
	}
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	public class PopupMenuShowingEventArgs : EventArgs {
		DXPopupMenu menu;
		BaseLayoutItemHitInfo hitInfo;
		bool allow;
		public PopupMenuShowingEventArgs(DXPopupMenu menu, BaseLayoutItemHitInfo hitInfo) : this(menu, hitInfo, true) { }
		public PopupMenuShowingEventArgs(DXPopupMenu menu, BaseLayoutItemHitInfo hitInfo, bool allow) {
			this.allow = allow;
			this.menu = menu;
			this.hitInfo = hitInfo;
		}
		public BaseLayoutItemHitInfo HitInfo { get { return hitInfo; } }
		public bool Allow { get { return allow; } set { allow = value; } }
		public DXPopupMenu Menu { get { return menu; } set { menu = value; } }
		public Point Point { get { return hitInfo.HitPoint; } }
	}
	[Obsolete("You should use the 'PopupMenuShowingEventHandler' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void LayoutMenuEventHandler(object sender, LayoutMenuEventArgs e);
	[Obsolete("You should use the 'PopupMenuShowingEventArgs' instead", false), EditorBrowsable(EditorBrowsableState.Never)]
	public class LayoutMenuEventArgs : PopupMenuShowingEventArgs {
		public LayoutMenuEventArgs(DXPopupMenu menu, BaseLayoutItemHitInfo hitInfo) : base(menu, hitInfo) { }
		public LayoutMenuEventArgs(DXPopupMenu menu, BaseLayoutItemHitInfo hitInfo, bool allow) : base(menu, hitInfo, allow) {
		}
	}
	public class LayoutControlCaseInsensitiveComparer : IComparer {
		CompareInfo compareInfo;
		public LayoutControlCaseInsensitiveComparer() {
			this.compareInfo = CultureInfo.CurrentCulture.CompareInfo;
		}
		public int Compare(object a, object b) {
			BaseLayoutItem i1 = a as BaseLayoutItem;
			BaseLayoutItem i2 = b as BaseLayoutItem;
			if((i1 != null) && (i2 != null)) {
				return compareInfo.Compare(i1.Text, i2.Text, CompareOptions.IgnoreSymbols);
			}
			return Comparer.Default.Compare(a, b);
		}
	}
	#endregion
}
