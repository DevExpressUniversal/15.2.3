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
using DevExpress.XtraGrid.Design;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraLayout;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views;
using System.Data;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using System.Drawing;
using System.ComponentModel.Design;
using System.IO;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views.Layout.Frames;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Customization.Controls;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.Registrator;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraGrid.Views.Layout.Designer {
	internal class ItemNameVisitor : BaseVisitor {
		Dictionary<string, BaseLayoutItem> items;
		public ItemNameVisitor(Dictionary<string, BaseLayoutItem> items) {
			this.items = items;
		}
		public override void Visit(BaseLayoutItem item) {
			items.Add(item.Name, item);
		}
	}
	internal class UniqueItemNameCreator {
		public static string CreateUniqueName(string prefix, BaseLayoutItem item, List<string> names, int startCount) {
			string result = item.Name;
			int i = startCount;
			while(true) {
				string name = prefix + i.ToString(); i++;
				if(!names.Contains(name)) { result = name; break; }
			}
			return result;
		}
		public static string CheckAndCreateUniqueFieldName(string name,string prefix, BaseLayoutItem item, List<string> names, int startCount) {
			if(!String.IsNullOrEmpty(name) && !names.Contains(name)) return name;
			return CreateUniqueName(prefix, item, names, startCount);
		}
	}
	internal class UniqueItemNameFinder : BaseVisitor {
		List<string> names;
		List<BaseLayoutItem> items;
		Dictionary<string, BaseLayoutItem> uniqueItems;
		public UniqueItemNameFinder(List<BaseLayoutItem> items, List<string> names, Dictionary<string, BaseLayoutItem> uniqueItems) {
			this.names = names;
			this.items = items;
			this.uniqueItems = uniqueItems;
		}
		public override void Visit(BaseLayoutItem item) {
			items.Add(item);
			string itemName = item.Name;
			if(!string.IsNullOrEmpty(itemName) && !names.Contains(itemName)) {
				uniqueItems.Add(itemName, item);
				names.Add(itemName);
			}
		}
	}
	internal class FieldPreferredWidthSetter : BaseVisitor {
		SizeF scaleFactor;
		public FieldPreferredWidthSetter(SizeF scaleFactor) {
			this.scaleFactor = scaleFactor;
		}
		public override void Visit(BaseLayoutItem item) {
			LayoutViewField field = item as LayoutViewField;
			if(field != null) {
				LayoutViewFieldInfo fieldInfo = field.ViewInfo as LayoutViewFieldInfo;
				field.EditorPreferredWidth = DevExpress.Skins.RectangleHelper.DeScaleHorizontal(fieldInfo.ClientArea.Width, scaleFactor.Width);
				if(field.SizeConstraintsType == SizeConstraintsType.Custom && field.MaxSize.IsEmpty) 
					field.SizeConstraintsType = SizeConstraintsType.Default;
			}
		}
	}
	internal class FieldPreferredWidthResetter : BaseVisitor {
		public override void Visit(BaseLayoutItem item) {
			LayoutViewField field = item as LayoutViewField;
			if(field != null)
				field.ResetEditorPreferredWidth();
		}
	}
	public class LayoutViewComponentChangeHelper {
		LayoutView ownerView = null;
		IComponentChangeService componentChangeService= null;
		DesignerTransaction temporaryTransaction=null;
		public LayoutViewComponentChangeHelper(LayoutView view) {
			this.ownerView = view;
		}
		protected IDesignerHost DesignerHost {
			get {
				if(ownerView.Site == null) return null;
				else return ownerView.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			}
		}
		protected IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null) {
					if(DesignerHost!=null) componentChangeService = (IComponentChangeService)DesignerHost.GetService(typeof(IComponentChangeService));
				}
				return componentChangeService;
			}
		}
		public virtual void OnChanging() {
			if(DesignerHost!=null) temporaryTransaction = DesignerHost.CreateTransaction("layout control internal change");
			else temporaryTransaction = null;
			try {
				if(ComponentChangeService!=null) {
					ComponentChangeService.OnComponentChanging(ownerView, null);
					PassChangeEventToChildren(true);
				}
			}
			finally { }
		}
		public virtual void OnChanged() {
			try {
				if(ComponentChangeService!=null) {
					PassChangeEventToChildren(false);
					ComponentChangeService.OnComponentChanged(ownerView, null, null, null);
				}
			}
			finally {
				if(temporaryTransaction != null) {
					temporaryTransaction.Commit();
					temporaryTransaction = null;
				}
			}
		}
		protected void PassChangeEventToChildren(bool changing) {
			if(ownerView.GridControl.Container == null) return;
			foreach(IComponent component in ownerView.GridControl.Container.Components) {
				BaseLayoutItem item = component as BaseLayoutItem;
				if(item!=null && item.Owner==(ownerView as ILayoutControl) && ComponentChangeService!=null) {
					if(changing)
						ComponentChangeService.OnComponentChanging(component, null);
					else
						ComponentChangeService.OnComponentChanged(component, null, null, null);
				}
			}
		}
	}
	[DesignTimeVisible(false), ToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class DesignerLayoutControlImplementor : LayoutControlImplementor {
		public DesignerLayoutControlImplementor(ILayoutControlOwner owner)
			: base(owner) {
		}
		protected LayoutViewCustomizer Customizer { 
			get { return ((DesignerLayoutControl)owner).Customizer; } 
		}
		public override bool DesignMode { get { return true; } }
		public override void ShowCustomizationForm() {
			EnableCustomizationMode = true;
		}
		public override void HideCustomizationForm() {
			EnableCustomizationMode = false;
		}
		public override LayoutStyleManager LayoutStyleManager {
			get { return null; }
		}
		public override bool EnableCustomizationMode {
			get { return base.EnableCustomizationMode; }
			set {
				if(EnableCustomizationMode == value) return;
				if(Customizer != null && Customizer.DesignerHelper != null)
					Customizer.DesignerHelper.LockModification();
				try {
					if(UndoManager.Enabled) UndoManager.Enabled = false;
					isCustomizationMode = value;
					if(value) owner.RaiseOwnerEvent_ShowCustomization(owner, EventArgs.Empty);
					else owner.RaiseOwnerEvent_HideCustomization(owner, EventArgs.Empty);
					SelectedChangedCount = 0;
					if(RootGroup != null) {
						EnabledStateController.SetItemEnabledStateDirty(RootGroup);
						RootGroup.ClearSelection();
						ShouldUpdateControlsLookAndFeel = true;
					}
					Invalidate();
				}
				finally {
					if(Customizer != null && Customizer.DesignerHelper != null)
						Customizer.DesignerHelper.UnLockModification();
				}
			}
		}
		protected override DevExpress.XtraLayout.Customization.RenameItemManager CreateRenameItemManager() {
			return new LayoutViewRenameItemManager();
		}
		public override void OnChangeUICues() {
		}
		public override void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			bool savedModifications = false;
			if(Customizer != null && Customizer.HasDesignerHelper) {
				Customizer.DesignerHelper.LockModification();
				savedModifications = Customizer.ModificationExist;
			}
			try { base.OnLookAndFeelStyleChanged(sender, e); }
			finally {
				if(Customizer != null && Customizer.HasDesignerHelper) {
					Customizer.ModificationExist = savedModifications;
					Customizer.DesignerHelper.UnLockModification();
				}
			}
		}
	}
	public class DesignerLayoutRightButtonMenuBase : RightButtonMenuBase {
		public DesignerLayoutRightButtonMenuBase(ILayoutControl owner, ArrayList menuItemInfoList) : base(owner, menuItemInfoList) { }
		public override void ShowMenu(Point point) {
			MenuManagerHelper.GetMenuManager(Owner.LookAndFeel.ActiveLookAndFeel).ShowPopupMenu(Menu, Owner.Control, point);
		}
	}
	public class DesignerLayoutRightButtonMenuManager : RightButtonMenuManager {
		public DesignerLayoutRightButtonMenuManager(ILayoutControl control) : base(control) { }
		protected override RightButtonMenuBase CreatePopMenuMenuCore(ILayoutControl owner, ArrayList menuItemInfoList) {
			return new DesignerLayoutRightButtonMenuBase(owner, menuItemInfoList);
		}
	}
	[DesignTimeVisible(false), ToolboxItem(false), System.Runtime.InteropServices.ComVisible(false)]
	public class DesignerLayoutControl : LayoutControl, ILayoutControl {
		LayoutView viewCore = null;
		LayoutViewCustomizer customizerCore = null;
		public DesignerLayoutControl(LayoutViewCustomizer customizer, LayoutView view)
			: base(false, true) {
			this.viewCore = view;
			this.customizerCore = customizer;
			OptionsView.EnableIndentsInGroupsWithoutBorders = true;
		}
		protected override RightButtonMenuManager CreateRightButtonMenuManager() {
			return new DesignerLayoutRightButtonMenuManager(this);
		}
		SizeF ILayoutControl.AutoScaleFactor {
			get { return (View == null) ? new SizeF(1f, 1f) : ((ILayoutControl)View).AutoScaleFactor; }
		}
		protected LayoutView View { get { return viewCore; } }
		protected internal LayoutViewCustomizer Customizer { get { return customizerCore; } }
		protected override LayoutControlImplementor CreateILayoutControlImplementorCore() {
			return new DesignerLayoutControlImplementor(this);
		}
		public override DevExpress.XtraLayout.Customization.UserCustomizationForm CreateCustomizationForm() { return null; }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.viewCore = null;
				this.customizerCore = null;
			}
			base.Dispose(disposing);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			InitializeAsDragDropDispatcherClient();
		}
		protected override void OnForeColorUpdated() {
			if(Customizer != null && !Customizer.IsDisposed)
				Customizer.DesignerHelper.fInternalLockModificationCounter++;
			base.OnForeColorUpdated();
			if(Customizer != null && !Customizer.IsDisposed)
				Customizer.DesignerHelper.fInternalLockModificationCounter--;
		}
		protected override bool CalcIsActiveAndCanProcessEvent() { return Enabled && Visible && lockCustomizationCounter == 0; }
		protected override bool CalcAllowProcessDragging() { return true; }
		protected override DevExpress.XtraLayout.Handlers.LayoutControlDragDropHelper CreateDragDropHelper() {
			return new DesignerLayoutControlDragDropHelper(this);
		}
		int lockCustomizationCounter = 0;
		public void LockCustomizationDragging() {
			lockCustomizationCounter++;
		}
		public void UnlockCustomizationDragging() {
			lockCustomizationCounter--;
		}
		internal void ProcessMouseEvent(EventType eventType, MouseEventArgs ea) { 
			switch(eventType){
				case EventType.MouseDown: OnMouseDown(ea); break;
				case EventType.MouseUp: OnMouseUp(ea); break;
				case EventType.MouseMove: OnMouseMove(ea); break;
			}
		}
		internal void ResetResizer() {
			ResetRootGroupResizer();
		}
	}
	public class DesignerLayoutControlDragDropHelper : LayoutControlDragDropHelper {
		public DesignerLayoutControlDragDropHelper(ILayoutControl owner) : base(owner) { }
		protected override DevExpress.XtraLayout.Dragging.DraggingVisualizer CreateDraggingVisualizer() {
			return new DesignerLayoutControlDraggingVisualizer(Owner);
		}
	}
	public class DesignerLayoutControlDraggingVisualizer : DevExpress.XtraLayout.Dragging.DraggingVisualizer {
		public DesignerLayoutControlDraggingVisualizer(ILayoutControl owner) : base(owner) { }
		protected override DevExpress.XtraLayout.Dragging.DragFrameWindow CreateDragFrameWindow() {
			return new DesignerLayoutControlDragFrameWindow(Owner);
		}
	}
	public class DesignerLayoutControlDragFrameWindow : DevExpress.XtraLayout.Dragging.DragFrameWindow {
		public DesignerLayoutControlDragFrameWindow(ILayoutControl owner) : base(owner) { }
		protected override bool CalcAllowShowWindow() { return true; }
	}
	public delegate void DesignerControlModifiedHandler();
	public class LayoutViewDesignerHelper {
		protected internal DesignerSizeGrip sizeGrip = null;
		DesignerLayoutControl designerControlCore = null;
		Control parentControlCore = null;
		LayoutView viewCore = null;
		LayoutViewCustomizer customizerCore = null;
		LayoutViewComponentChangeHelper changeHelper = null;
		public LayoutViewDesignerHelper(LayoutViewCustomizer customizer, LayoutView view) {
			this.viewCore = view;
			this.customizerCore = customizer;
			designerControlCore = CreateDesignerLayoutControl();
			InitDesignerLayoutControl(DesignerControl);
			sizeGrip = new DesignerSizeGrip(DesignerControl, customizer);
		}
		protected LayoutView View { get { return viewCore; } }
		protected LayoutViewCustomizer Customizer { get { return customizerCore; } }
		protected void InitDesignerLayoutControl(LayoutControl lc) {
			lc.OptionsCustomizationForm.ShowPropertyGrid=false;
			lc.OptionsItemText.TextAlignMode = TextAlignMode.AlignInGroups;
			(lc as ILayoutControl).AllowPaintEmptyRootGroupText = false;
		}
		protected DesignerLayoutControl CreateDesignerLayoutControl() {
			return new DesignerLayoutControl(Customizer, View);
		}
		public Size CustomizedControlSize {
			get { return sizeGrip.CustomizedControlSize; }
		}
		public DesignerLayoutControl DesignerControl {
			get { return designerControlCore; }
		}
		public void Initialize(Control parentControl) {
			changeHelper = new LayoutViewComponentChangeHelper(View);
			this.parentControlCore = parentControl;
			DesignerControl.Parent = parentControl;
			sizeGrip.SetParentContainer(parentControl);
			SubscribeDesignerControlEvents();
			SubscribeSizeGripEvents();
		}
		protected virtual void SubscribeDesignerControlEvents() {
			DesignerControl.Changed+= OnDesignerControlItemsChanged;
			DesignerControl.RequestUniqueName += OnDesignerControlRequestUniqueName;
		}
		protected virtual void UnSubscribeDesignerControlEvents() {
			DesignerControl.Changed -= OnDesignerControlItemsChanged;
			DesignerControl.RequestUniqueName -= OnDesignerControlRequestUniqueName;
		}
		protected virtual void SubscribeSizeGripEvents() {
			sizeGrip.Move += OnSizeGripMoved;
		}
		protected virtual void UnSubscribeSizeGripEvents() {
			sizeGrip.Move -= OnSizeGripMoved;
		}
		protected void OnDesignerControlRequestUniqueName(object sender, UniqueNameRequestArgs e) {
			if(View.Site==null) return;
			List<string> names = new List<string>(GetAllSiteNames(View.Site));
			foreach(BaseLayoutItem item in DesignerControl.Items) {
				if(!names.Contains(item.Name)) names.Add(item.Name);
			}
			e.TargetItem.Name = CreateUniqueName("item", e.TargetItem, names);
		}
		public static string CreateUniqueName(string prefix, BaseLayoutItem item, List<string> names) {
			return UniqueItemNameCreator.CreateUniqueName(prefix, item, names, 1);
		}
		protected internal int lockMoveCounter = 0;
		protected internal int fCustomizationSizeGripMoving = 0;
		public bool IsCustomizationSizeGripMoving {
			get { return fCustomizationSizeGripMoving > 0; }
		}
		public bool IsMoveLocked { 
			get { return lockMoveCounter > 0; } 
		}
		protected void OnSizeGripMoved(object sender, EventArgs e) {
			if(IsMoveLocked || IsModificationLocked) return;
			if(IsCustomizationSizeGripMoving && DesignerControlModified!=null) {
				DesignerControlModified();
			}
		}
		public void Dispose() {
			if(DesignerControl!=null) {
				UnSubscribeDesignerControlEvents();
				DesignerControl.Dispose();
				designerControlCore = null;
			}
			if(sizeGrip!=null) {
				UnSubscribeSizeGripEvents();
				sizeGrip.Dispose();
				sizeGrip = null;
			}
			viewCore=null;
		}
		public void TopLeftByParentControl(Size parentSize) { 
		}
		public void PlaceInParentControl(Size parentSize) {
			lockMoveCounter++;
			try {
				Size cardSize = DesignerControl.Size;
				Point location = sizeGrip.GetSizedControlLocation(cardSize, parentSize, GetParentAutoScrollPosition());
				if(DesignerControl.Location != location) DesignerControl.Location = location;
				sizeGrip.Location = sizeGrip.GetSizeGripLocation(location, cardSize);
				DesignerControl.Invalidate();
			}
			finally { lockMoveCounter--; }
		}
		protected Point GetParentAutoScrollPosition() {
			if(ParentControl is XtraScrollableControl) {
				return ((XtraScrollableControl)ParentControl).AutoScrollPosition;
			}
			return Point.Empty;
		}
		public virtual void ResetView(LayoutView view) {
			LockModification();
			view.BeginInit();
			ClearView(view, true, true, true);
			view.InitializeTemplateCardDefault();
			view.EndInit();
			UnLockModification();
		}
		public virtual void CheckViewDesignTimeComponents(LayoutView view) {
			if(view.Site == null) return;
			ArrayList itemsToCheck = new ArrayList(view.Items);
			foreach(BaseLayoutItem hitem in view.HiddenItems) {
				if(!(hitem is EmptySpaceItem)) itemsToCheck.Add(hitem);
			}
			foreach(BaseLayoutItem item in itemsToCheck) {
				ArrayList components = new ArrayList(view.Site.Container.Components);
				bool fNotInThisView = !LayoutViewComponentsUpdateHelper.AlreadyInExistingComponents(view, item, components);
				if(fNotInThisView) {
					view.Site.Container.Add(item);
					LayoutViewComponentsUpdateHelper.CheckComponentDesignTimeName(item);
				}
			}
		}
		public virtual void SynchronizeViewByView(LayoutView sourceView, LayoutView targetView) {
			LockModification();
			AttachedProperty.SetValue(targetView.GridControl, LayoutView.IsCustomizationRestoringInProgressProperty, true);
			using(MemoryStream ms = new MemoryStream()) {
				sourceView.SaveLayoutToStream(ms, OptionsLayoutBase.FullLayout);
				ms.Seek(0, SeekOrigin.Begin);
				targetView.RestoreLayoutFromStream(ms, OptionsLayoutBase.FullLayout);
				ms.Close();
			}
			AttachedProperty.ClearValue(targetView.GridControl, LayoutView.IsCustomizationRestoringInProgressProperty);
			if(targetView.TemplateCard.GroupBordersVisible!=sourceView.TemplateCard.GroupBordersVisible) {
				targetView.TemplateCard.GroupBordersVisible=sourceView.TemplateCard.GroupBordersVisible;
			}
			UnLockModification();
		}
		string[] GetAllSiteNames(ISite site) {
			List<string> names = new List<string>();
			if(site != null) 
				foreach(IComponent c in site.Container.Components) 
					names.Add(c.Site.Name);
			return names.ToArray();
		}
		public void CorrectItemNames(LayoutViewCard card, LayoutView editingView) {
			List<string> names = new List<string>(GetAllSiteNames(editingView.Site));
			Dictionary<string, BaseLayoutItem> cardItems = new Dictionary<string, BaseLayoutItem>();
			editingView.TemplateCard.Accept(new ItemNameVisitor(cardItems));
			names.AddRange(new string[] { "Group ", "TabbedGroup", "Item" });
			List<BaseLayoutItem> items = new List<BaseLayoutItem>();
			Dictionary<string, BaseLayoutItem> uniqueItems = new Dictionary<string, BaseLayoutItem>();
			card.Accept(new UniqueItemNameFinder(items, names, uniqueItems));
			foreach(BaseLayoutItem item in items) {
				if(uniqueItems.ContainsValue(item)) continue;
				BaseLayoutItem cardItem;
				if(cardItems.TryGetValue(item.Name, out cardItem) && cardItem.TypeName == item.TypeName) continue;
				if(item is LayoutGroup) {
					item.Name = LayoutViewDesignerHelper.CreateUniqueName("Group", item, names);
				} else if(item is TabbedGroup) {
					item.Name = LayoutViewDesignerHelper.CreateUniqueName("TabbedGroup", item, names);
				} else {
					item.Name = LayoutViewDesignerHelper.CreateUniqueName("Item", item, names);
				}
				names.Add(item.Name);
			}
		}
		public virtual void SynchronizeViewFromDesigner(LayoutView view) {
			LockModification();
			DesignerControl.Root.Update();
			(view as ILayoutControl).Size = DesignerControl.Root.Size;
			if(CustomizedControlSize != Size.Empty) {
				view.CardMinSize = CustomizedControlSize;
			}
			var scaleFactor = view.GetScaleFactor();
			view.BeginUpdate();
			view.BeginInit();
			ClearView(view, true, true, true);
			ArrayList designerHiddenItems = new ArrayList(DesignerControl.HiddenItems);
			bool fdesignerHiddenItemsNotEmpty = DesignerControl.HiddenItems.Count > 0;
			if(fdesignerHiddenItemsNotEmpty) {
				AddDesignerHiddenItemsToView(view, designerHiddenItems);
			}
			view.TemplateCard = (LayoutViewCard)DesignerControl.Root.Clone(null, view as ILayoutControl);
			view.TemplateCard.Name = view.CreateTemplateCardName();
			view.TemplateCard.BeginUpdate();
			view.TemplateCard.Accept(new FieldPreferredWidthSetter(scaleFactor));
			view.TemplateCard.EndUpdate();
			view.OptionsItemText.AlignMode = view.OptionsItemText.ConvertFrom(DesignerControl.OptionsItemText.TextAlignMode);
			view.OptionsItemText.TextToControlDistance = DesignerControl.OptionsItemText.TextToControlDistance;
			view.EndInit();
			view.EndUpdate();
			view.Refresh();
			UnLockModification();
		}
		protected Control ParentControl {
			get { return parentControlCore; }
		}
		internal int fInternalLockModificationCounter = 0;
		public bool IsModificationLocked { get { return fInternalLockModificationCounter > 0; } }
		public void LockModification() { 
			fInternalLockModificationCounter++;
			DesignerControl.LockCustomizationDragging();
		}
		public void UnLockModification() {
			DesignerControl.UnlockCustomizationDragging();
			fInternalLockModificationCounter--; 
		}
		public virtual void SynchronizeDesignerFromView(LayoutView view) {
			LockModification();
			view.TemplateCard.Update();
			PlaceInParentControl(ParentControl.ClientSize);
			DesignerControl.HiddenItems.Clear();
			ArrayList viewHiddenItems = new ArrayList((view as ILayoutControl).HiddenItems);
			bool fViewHiddenItemsIsNotEmpty = (view as ILayoutControl).HiddenItems.Count > 0;
			if(fViewHiddenItemsIsNotEmpty) {
				AddItemsToDesignerHiddenItems(viewHiddenItems);
			}
			DesignerControl.Root = (LayoutControlGroup)view.TemplateCard.Clone(null, DesignerControl);
			DesignerControl.Root.Name = view.CreateTemplateCardName();
			DesignerControl.Root.Text = Properties.Resources.TemplateCardCaption;
			DesignerControl.Root.GroupBordersVisible = view.CalcCardGroupBordersVisibility();
			if(!DesignerControl.Root.Expanded) DesignerControl.Root.Expanded = true;
			if(DesignerControl.Root.ExpandButtonVisible) DesignerControl.Root.ExpandButtonVisible = false;
			DesignerControl.OptionsItemText.TextAlignMode = view.OptionsItemText.ConvertTo(view.OptionsItemText.AlignMode);
			DesignerControl.OptionsItemText.TextToControlDistance = view.OptionsItemText.TextToControlDistance;
			DesignerControl.Root.BeginUpdate();
			DesignerControl.Root.Accept(new FieldPreferredWidthResetter());
			DesignerControl.Root.EndUpdate();
			Size cardSize = new Size(Math.Max(view.CardMinSize.Width, view.TemplateCard.Width), Math.Max(view.CardMinSize.Height, view.TemplateCard.Height));
			if(view.TemplateCard.Items.Count == 0)
				cardSize = view.ScaleSize(new Size(200, 200));
			DesignerControl.Size = cardSize;
			DesignerControl.ResetResizer();
			sizeGrip.MinControlSize = DesignerControl.Root.MinSize;
			UnLockModification();
		}
		protected virtual void AddDesignerHiddenItemsToView(LayoutView view, ArrayList designerHiddenItems) {
			foreach(BaseLayoutItem designerItem in designerHiddenItems) {
				BaseLayoutItem cloneItem = designerItem.Clone(view.TemplateCard, view as ILayoutControl);
				if(cloneItem is TabbedControlGroup) {
					TabbedControlGroup addedTabGroup = view.TemplateCard.AddTabbedGroup(cloneItem as TabbedControlGroup);
					if(addedTabGroup!=null) addedTabGroup.HideToCustomization();
				}
				if(cloneItem.IsGroup) {
					LayoutControlGroup addedGroup = view.TemplateCard.AddGroup(cloneItem as LayoutControlGroup);
					if(addedGroup!=null) addedGroup.HideToCustomization();
				} else {
					BaseLayoutItem addedItem = view.TemplateCard.AddItem(cloneItem);
					if(addedItem!=null) addedItem.HideToCustomization();
				}
			}
		}
		protected virtual void AddItemsToDesignerHiddenItems(ArrayList viewHiddenItems) {
			foreach(BaseLayoutItem hiddenItem in viewHiddenItems) {
				if(hiddenItem == null) continue;
				BaseLayoutItem cloneItem = hiddenItem.Clone(DesignerControl.Root, DesignerControl);
				if(cloneItem.IsGroup) {
					LayoutControlGroup addedGroup = DesignerControl.AddGroup(cloneItem as LayoutControlGroup);
					addedGroup.HideToCustomization();
				}
				else {
					if(cloneItem is TabbedGroup) {
						BaseLayoutItem addedTabbedGroup = DesignerControl.AddTabbedGroup(cloneItem as TabbedGroup);
						addedTabbedGroup.HideToCustomization();
					} else {
						BaseLayoutItem addedItem = DesignerControl.AddItem(cloneItem);
						addedItem.HideToCustomization();
						LayoutViewField field = addedItem as LayoutViewField;
						if(field != null)
							field.ResetEditorPreferredWidth();
					}
				}
			}
		}
		public static void ClearView(LayoutView view, bool fClearViewTemplateCard, bool fClearViewComponents, bool fClearViewHiddenItems) {
			ISite site = view.Site;
			view.BeginUpdate();
			if(fClearViewTemplateCard) {
				ArrayList itemsToRemove = new ArrayList(view.Items);
				RemoveItemsFromSite(view, site, itemsToRemove);
				view.TemplateCard.Clear();
			}
			if(fClearViewComponents) {
				ArrayList componentsToRemove = new ArrayList((view as ILayoutControl).Components);
				RemoveComponentsFromSite(view, site, componentsToRemove);
				(view as ILayoutControl).Components.Clear();
			}
			if(fClearViewHiddenItems) {
				ArrayList itemsToRemove = new ArrayList(view.HiddenItems);
				RemoveItemsFromSite(view, site, itemsToRemove);
				view.HiddenItems.Clear();
			}
			view.EndUpdate();
		}
		public static void RemoveComponentsFromSite(LayoutView view, ISite site, ArrayList componentsToRemove) {
			if(site == null) return;
			ComponentCollection viewSiteComponents = site.Container.Components;
			foreach(IComponent component in componentsToRemove) {
				BaseLayoutItem viewItem = component as BaseLayoutItem;
				if(viewItem == null) break;
				IComponent siteComponent = LayoutViewComponentsUpdateHelper.FindInExistingComponents(view, viewItem, viewSiteComponents);
				if(siteComponent != null) site.Container.Remove(siteComponent);
			}
		}
		public static void RemoveItemsFromSite(LayoutView view, ISite site, ArrayList itemsToRemove) {
			if(site == null) return;
			ComponentCollection viewSiteComponents = site.Container.Components;
			foreach(BaseLayoutItem item in itemsToRemove) {
				IComponent siteComponent = LayoutViewComponentsUpdateHelper.FindInExistingComponents(view, item, viewSiteComponents);
				if(siteComponent != null) site.Container.Remove(siteComponent);
			}
		}
		protected virtual void OnDesignerControlItemsChanged(object sender, EventArgs e) {
			if(IsModificationLocked) return;
			DesignerControl.Size = DesignerControl.Root.Size;
			sizeGrip.MinControlSize = DesignerControl.Root.MinSize;
			PlaceInParentControl(ParentControl.ClientSize);
			if(DesignerControlModified != null) DesignerControlModified();
		}
		public event DesignerControlModifiedHandler DesignerControlModified;
		public virtual void OnChanging() { changeHelper.OnChanging(); }
		public virtual void OnChanged() { changeHelper.OnChanged(); }
	}
	internal enum CustomizationStringID {
		CustomizationFormCaption, CustomizationFormDescription,ModifiedMessage,
		ButtonOk, ButtonCancel, 
		ButtonApply, ButtonPreview, ButtonSaveLayout, ButtonLoadLayout,
		ButtonCustomizeShow, ButtonCustomizeHide, ButtonReset, ButtonShrinkToMinimum,
		PageTemplateCard, PageViewLayout,
		GroupCustomization, GroupCaptions, GroupIndents,
		GroupHiddenItems, GroupTreeView, GroupPropertyGrid,
		LabelTextIndent, LabelPadding, LabelSpacing,
		LabelCaptionLocation, LabelGroupCaptionLocation, LabelTextAlignment,
		GroupView, GroupLayout, GroupCards, GroupFields,
		LabelShowLines, LabelShowHeaderPanel, LabelShowFilterPanel, LabelScrollVisibility,
		LabelViewMode, LabelCardArrangeRule, LabelCardEdgeAlignment,
		IntervalsGroup, LabelHorizontal, LabelVertical,
		LabelShowCardCaption, LabelShowCardExpandButton, LabelShowCardBorder,
		LabelAllowFieldHotTracking, LabelShowFieldBorder, LabelShowFieldHint,
	}
	internal class RuntimeCustomizationLocalizeHelper : IDisposable {
		protected string strCustomizationFormCaption, strCustomizationFormDescription, strModifiedMessage;
		protected string strPageTemplateCard, strButtonApply, strButtonPreview;
		protected string strButtonOk, strButtonCancel;
		protected string strButtonCustomizeShow, strButtonCustomizeHide, strButtonReset, strButtonShrinkToMinimum;
		protected string strGroupCustomization, strGroupHiddenItems, strGroupTreeView, strGroupPropertyGrid;
		protected string strGroupIndents, strLabelTextIndent, strLabelPadding, strLabelSpacing;
		protected string strGroupCaptions, strLabelCaptionLocation, strLabelGroupCaptionLocation, strLabelTextAlignment;
		protected string strPageViewLayout, strButtonSaveLayout, strButtonLoadLayout;
		protected string strGroupLayout, strLabelViewMode, strLabelCardArrangeRule, strLabelCardEdgeAlignment;
		protected string strGroupIntervals, strLabelHorizontal, strLabelVertical;
		protected string strGroupCards, strLabelShowCardCaption, strLabelShowCardExpandButton, strLabelShowCardBorder;
		protected string strGroupFields;
		protected string strGroupView, strLabelShowLines, strLabelShowHeaderPanel, strLabelShowFilterPanel, strLabelScrollVisibility;
		protected string strLabelAllowFieldHotTracking, strLabelShowFieldBorder, strLabelShowFieldHint;
		public RuntimeCustomizationLocalizeHelper() {
			LoadStrings();
		}
		protected virtual void LoadStrings() {
			strCustomizationFormCaption = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewCustomizationFormCaption);
			strCustomizationFormDescription = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewCustomizationFormDescription);
			strModifiedMessage = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutModifiedWarning);
			strButtonApply = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewButtonApply);
			strButtonPreview = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewButtonPreview);
			strButtonOk = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewButtonOk);
			strButtonCancel = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewButtonCancel);
			strButtonSaveLayout = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewButtonSaveLayout);
			strButtonLoadLayout = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewButtonLoadLayout);
			strPageTemplateCard = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewPageTemplateCard);
			strPageViewLayout = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewPageViewLayout);
			strButtonCustomizeShow = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewButtonCustomizeShow);
			strButtonCustomizeHide = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewButtonCustomizeHide);
			strButtonReset = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewButtonReset);
			strButtonShrinkToMinimum = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewButtonShrinkToMinimum);
			strGroupCustomization = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupCustomization);
			strGroupCaptions = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupCaptions);
			strGroupIndents = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupIndents);
			strGroupHiddenItems = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupHiddenItems);
			strGroupTreeView = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupTreeStructure);
			strGroupPropertyGrid = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupPropertyGrid);
			strLabelTextIndent = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelTextIndent);
			strLabelPadding = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelPadding);
			strLabelSpacing = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelSpacing);
			strLabelCaptionLocation = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelCaptionLocation);
			strLabelGroupCaptionLocation = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelGroupCaptionLocation);
			strLabelTextAlignment = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelTextAlignment);
			strGroupView = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupView);
			strGroupLayout = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupLayout);
			strGroupCards = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupCards);
			strGroupFields = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupFields);
			strLabelShowLines = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelShowLines);
			strLabelShowHeaderPanel = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelShowHeaderPanel);
			strLabelShowFilterPanel = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelShowFilterPanel);
			strLabelScrollVisibility = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelScrollVisibility);
			strLabelViewMode = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelViewMode);
			strLabelCardArrangeRule = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelCardArrangeRule);
			strLabelCardEdgeAlignment = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelCardEdgeAlignment);
			strGroupIntervals = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewGroupIntervals);
			strLabelHorizontal = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelHorizontal);
			strLabelVertical = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelVertical);
			strLabelShowCardCaption = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelShowCardCaption);
			strLabelShowCardExpandButton = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelShowCardExpandButton);
			strLabelShowCardBorder = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelShowCardBorder);
			strLabelAllowFieldHotTracking = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelAllowFieldHotTracking);
			strLabelShowFieldBorder = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelShowFieldBorder);
			strLabelShowFieldHint = GridLocalizer.Active.GetLocalizedString(GridStringId.LayoutViewLabelShowFieldHint);
		}
		public string StringByID(CustomizationStringID id) {
			string str = string.Empty;
			switch(id) {
				case CustomizationStringID.CustomizationFormCaption: str = strCustomizationFormCaption; break;
				case CustomizationStringID.CustomizationFormDescription: str = strCustomizationFormDescription; break;
				case CustomizationStringID.ModifiedMessage: str = strModifiedMessage; break;
				case CustomizationStringID.PageTemplateCard: str = strPageTemplateCard; break;
				case CustomizationStringID.PageViewLayout: str = strPageViewLayout; break;
				case CustomizationStringID.ButtonCustomizeShow: str = strButtonCustomizeShow; break;
				case CustomizationStringID.ButtonCustomizeHide: str = strButtonCustomizeHide; break;
				case CustomizationStringID.ButtonReset: str = strButtonReset; break;
				case CustomizationStringID.ButtonShrinkToMinimum: str = strButtonShrinkToMinimum; break;
				case CustomizationStringID.GroupCustomization: str = strGroupCustomization; break;
				case CustomizationStringID.GroupCaptions: str = strGroupCaptions; break;
				case CustomizationStringID.GroupIndents: str = strGroupIndents; break;
				case CustomizationStringID.GroupHiddenItems: str = strGroupHiddenItems; break;
				case CustomizationStringID.GroupTreeView: str = strGroupTreeView; break;
				case CustomizationStringID.GroupPropertyGrid: str = strGroupPropertyGrid; break;
				case CustomizationStringID.LabelTextIndent: str = strLabelTextIndent; break;
				case CustomizationStringID.LabelPadding: str = strLabelPadding; break;
				case CustomizationStringID.LabelSpacing: str = strLabelSpacing; break;
				case CustomizationStringID.LabelCaptionLocation: str = strLabelCaptionLocation; break;
				case CustomizationStringID.LabelGroupCaptionLocation: str = strLabelGroupCaptionLocation; break;
				case CustomizationStringID.LabelTextAlignment: str = strLabelTextAlignment; break;
				case CustomizationStringID.ButtonApply: str = strButtonApply; break;
				case CustomizationStringID.ButtonPreview: str = strButtonPreview; break;
				case CustomizationStringID.ButtonOk: str = strButtonOk; break;
				case CustomizationStringID.ButtonCancel: str = strButtonCancel; break;
				case CustomizationStringID.ButtonSaveLayout: str = strButtonSaveLayout; break;
				case CustomizationStringID.ButtonLoadLayout: str = strButtonLoadLayout; break;
				case CustomizationStringID.GroupView: str = strGroupView; break;
				case CustomizationStringID.GroupLayout: str = strGroupLayout; break;
				case CustomizationStringID.GroupCards: str = strGroupCards; break;
				case CustomizationStringID.GroupFields: str = strGroupFields; break;
				case CustomizationStringID.LabelShowLines: str = strLabelShowLines; break;
				case CustomizationStringID.LabelShowHeaderPanel: str = strLabelShowHeaderPanel; break;
				case CustomizationStringID.LabelShowFilterPanel: str = strLabelShowFilterPanel; break;
				case CustomizationStringID.LabelScrollVisibility: str = strLabelScrollVisibility; break;
				case CustomizationStringID.LabelViewMode: str = strLabelViewMode; break;
				case CustomizationStringID.LabelCardArrangeRule: str = strLabelCardArrangeRule; break;
				case CustomizationStringID.LabelCardEdgeAlignment: str = strLabelCardEdgeAlignment; break;
				case CustomizationStringID.IntervalsGroup: str = strGroupIntervals; break;
				case CustomizationStringID.LabelHorizontal: str = strLabelHorizontal; break;
				case CustomizationStringID.LabelVertical: str = strLabelVertical; break;
				case CustomizationStringID.LabelShowCardCaption: str = strLabelShowCardCaption; break;
				case CustomizationStringID.LabelShowCardExpandButton: str = strLabelShowCardExpandButton; break;
				case CustomizationStringID.LabelShowCardBorder: str = strLabelShowCardBorder; break;
				case CustomizationStringID.LabelAllowFieldHotTracking: str = strLabelAllowFieldHotTracking; break;
				case CustomizationStringID.LabelShowFieldBorder: str = strLabelShowFieldBorder; break;
				case CustomizationStringID.LabelShowFieldHint: str = strLabelShowFieldHint; break;
			}
			return str;
		}
		IDictionary<object, string> cache;
		public string Converter<TEnum>(TEnum value) where TEnum : struct {
			if(cache == null)
				cache = new Dictionary<object, string>();
			string result;
			if(!cache.TryGetValue(value, out result)) {
				EnumStringID enumID = (EnumStringID)Enum.Parse(typeof(EnumStringID), 
					string.Concat(typeof(TEnum).Name, "_", value.ToString()));
				result = LayoutViewEnumLocalizer.Active.GetLocalizedString(enumID);
				cache.Add(value, result);
			}
			return result;
		}
		public void Dispose() { }
	}
	[ToolboxItem(false)]
	public class LayoutViewCustomizer : LayoutsBase, DevExpress.Utils.Menu.IDXMenuManagerProvider {
		LayoutViewDesignerHelper designerHelper = null;
		bool fModificationExist = false;
		bool fPreviewDataLoaded = false;
		bool fInitialized = false;
		RuntimeCustomizationLocalizeHelper localizerCore = null;
		protected internal GridControl designerGrid = null;
		protected internal BaseView designerViewCore = null;
		protected internal SplitGroupPanel designerPanel = null;
		protected internal SplitGroupPanel designerPropertiesPanel = null;
		protected internal SplitGroupPanel designerCustomizationPanel = null;
		protected internal DesignerControlSettingsManager designerSettingsControl = null;
		protected internal TemplateCardCustomizationControl templateCardCustomizationControl = null;
		protected internal SplitContainerControl designerSplitContainer1 = null;
		protected internal SplitContainerControl designerSplitContainer2 = null;
		protected internal SplitGroupPanel previewPanel = null;
		protected internal SplitGroupPanel previewPropertiesPanel = null;
		protected internal ViewSettingsManager previewSettingsControl = null;
		protected internal SplitContainerControl previewSplitContainer = null;
		protected internal XtraTabControl designerTabControl = null;
		protected internal XtraTabPage designerTabPage = null;
		protected internal XtraScrollableControl designerScroller = null;
		protected internal XtraScrollableControl designerCardSettingsScroller = null;
		protected internal XtraScrollableControl designerCardCustomizationScroller = null;
		protected internal XtraScrollableControl designerViewSettingsScroller = null;
		protected internal LabelControl footerSeparator = null;
		internal RuntimeCustomizationLocalizeHelper Localizer {
			get {
				if(localizerCore==null) localizerCore = new RuntimeCustomizationLocalizeHelper();
				return localizerCore;
			}
		}
		public override void InitComponent() {
			CreateMainPage();
			previewControl = CreatePreviewControl();
			previewControl.Parent = pnlGrid;
		}
		public override void EndInitialize() {
			base.EndInitialize();
			DesignerHelper.DesignerControl.ResetResizer();
			var minSize = DesignerHelper.DesignerControl.Root.MinSize;
			var size = DesignerHelper.DesignerControl.Size;
			DesignerHelper.DesignerControl.Size = new System.Drawing.Size(Math.Max(size.Width, minSize.Width), Math.Max(size.Height, minSize.Height));
			DesignerHelper.sizeGrip.MinControlSize = minSize;
			DesignerHelper.PlaceInParentControl(designerScroller.ClientSize);
		}
		protected override string PreviewPanelText {
			get { return Localizer.StringByID(CustomizationStringID.PageViewLayout); }
		}
		protected string ModifiedMessage {
			get { return Localizer.StringByID(CustomizationStringID.ModifiedMessage); }
		}
		protected string CustomizationFormCaption {
			get { return Localizer.StringByID(CustomizationStringID.CustomizationFormCaption); }
		}
		public bool ModificationExist {
			get { return fModificationExist; }
			set {
				fModificationExist = value;
				OnModificationChanged();
			}
		}
		protected virtual void OnModificationChanged() {
			(DesignerHelper.DesignerControl as ILayoutControl).SetIsModified(fModificationExist);
			SetLayoutChanged(fModificationExist);
		}
		public bool HasDesignerHelper {
			get { return designerHelper != null; }
		}
		public LayoutViewDesignerHelper DesignerHelper {
			get {
				if(designerHelper==null) designerHelper = CreateDesignerHelper();
				return designerHelper;
			}
		}
		protected void Close() {
			Form customizationForm = FindForm();
			customizationForm.Close();
		}
		protected internal void btnOk_Click(object sender, System.EventArgs e) {
			if(ModificationExist) Apply();
			Close();
		}
		protected internal void btnCancel_Click(object sender, System.EventArgs e) {
			ModificationExist=false;
			Close();
		}
		protected virtual LayoutViewDesignerHelper CreateDesignerHelper() {
			return new LayoutViewDesignerHelper(this, EditingLayoutView);
		}
		public LayoutViewCustomizer()
			: base(6) {
			SetComponentsText();
		}
		protected void DoDispose() {
			UnSubscribeEvents();
			btnCancel.Click -= new EventHandler(btnCancel_Click);
			btnOk.Click -= new EventHandler(btnOk_Click);
			ProcessClosing();
			if(templateCardCustomizationControl!=null) {
				templateCardCustomizationControl.Dispose();
				templateCardCustomizationControl = null;
			}
			if(designerSettingsControl!=null){
				designerSettingsControl.Dispose();
				designerSettingsControl = null;
			}
			if(previewSettingsControl!=null) {
				previewSettingsControl.Dispose();
				previewSettingsControl = null;
			}
			if(designerHelper!=null) {
				designerHelper.Dispose();
				designerHelper=null;
			}
			if(Localizer!=null) {
				Localizer.Dispose();
				localizerCore = null;
			}
		}
		public LayoutView DesignerView { get { return designerViewCore as LayoutView; } }
		protected override void Dispose(bool disposing) {
			if(disposing) DoDispose();
			base.Dispose(disposing);
		}
		protected void SubscribeEvents() {
			DesignerHelper.DesignerControlModified += OnDesignerControlModified;
		}
		protected void UnSubscribeEvents() {
			DesignerHelper.DesignerControlModified -= OnDesignerControlModified;
		}
		protected override string DescriptionText {
			get { return Localizer.StringByID(CustomizationStringID.CustomizationFormDescription); }
		}
		protected virtual void SetComponentsText() {
			btnOk.Text = Localizer.StringByID(CustomizationStringID.ButtonOk);
			btnCancel.Text = Localizer.StringByID(CustomizationStringID.ButtonCancel);
			btnApply.Text = Localizer.StringByID(CustomizationStringID.ButtonApply);
			sbPreview.Text = Localizer.StringByID(CustomizationStringID.ButtonPreview);
			btnLoad.Text = Localizer.StringByID(CustomizationStringID.ButtonLoadLayout);
			btnSave.Text = Localizer.StringByID(CustomizationStringID.ButtonSaveLayout);
			int dx = CheckButtonsTextWidth();
			if(dx > 0) {
				btnOk.Location -= new Size(dx*3, 0);
				btnOk.Size += new Size(dx, 0);
				btnCancel.Location -= new Size(dx*2, 0);
				btnCancel.Size += new Size(dx, 0);
				btnApply.Location -= new Size(dx, 0);
				btnApply.Size += new Size(dx, 0);
			}
		}
		protected int CheckButtonsTextWidth() {
			int offset = 0;
			int measureWidth = 200;
			float minTextWidth = 80 - 16 - 4 - 4 - 5;
			using(Bitmap measureBitmap = new Bitmap(measureWidth, 24)) {
				using(Graphics g = Graphics.FromImage(measureBitmap)) {
					float textWidth = 0;
					textWidth = Math.Max(btnOk.Appearance.CalcTextSize(g, btnOk.Text, measureWidth).Width, textWidth);
					textWidth = Math.Max(btnCancel.Appearance.CalcTextSize(g, btnCancel.Text, measureWidth).Width, textWidth);
					textWidth = Math.Max(btnApply.Appearance.CalcTextSize(g, btnApply.Text, measureWidth).Width, textWidth);
					if(textWidth > minTextWidth) offset = (int)(textWidth - minTextWidth);
				}
			}
			return offset;
		}
		protected override void OnLoad(EventArgs e) {
			DesignerHelper.LockModification();
			base.OnLoad(e);
			if(!fInitialized) {
				InitializeAllExistingControls();
				CreateTemplateCardTabPage();
				InitializeTemplateCardDesigner();
			}
			DesignerHelper.SynchronizeDesignerFromView(DesignerView);
			this.designerSettingsControl.SetDefaultValues();
			TemplateCardPageVisible = true;
			DesignerHelper.PlaceInParentControl(designerPanel.ClientSize);
			ModificationExist = false;
			if(!fInitialized) {
				SubscribeEvents();
			}
			fInitialized = true;
			DesignerHelper.UnLockModification();
		}
		protected void InitView() {
			DesignerView.fInternalLockActions = true;
			SyncTags(EditingLayoutView, DesignerView);
		}
		void SyncTags(LayoutView source, LayoutView target) {
			Dictionary<string, object> tags = new Dictionary<string, object>();
			foreach(BaseLayoutItem item in source.Items) {
				tags[item.Name] = item.Tag;
			}
			foreach(BaseLayoutItem item in target.Items) {
				object tag;
				if(tags.TryGetValue(item.Name, out tag)) {
					item.Tag = tag;
				}
			}
		}
		protected virtual void InitializeTemplateCardDesigner() {
			designerScroller = new XtraScrollableControl();
			designerScroller.Name = "designerScroller";
			designerScroller.Parent = designerPanel;
			designerScroller.Dock = DockStyle.Fill;
			designerScroller.AlwaysScrollActiveControlIntoView = false;
			SetControlLookAndFeel(designerScroller, EditingLayoutView.LookAndFeel);
			InitHelper(DesignerHelper, designerScroller, EditingLayoutView);
		}
		protected internal static void InitHelper(LayoutViewDesignerHelper helper, Control helperParent, LayoutView view) {
			helper.Initialize(helperParent);
			SetControlLookAndFeel(helper.sizeGrip, view.LookAndFeel);
			SetControlLookAndFeel(helper.DesignerControl, view.LookAndFeel);
			(helper.DesignerControl as ILayoutControl).SetIsModified(false);
		}
		protected void OnDesignerControlModified() {
			ModificationExist = true;
		}
		protected virtual void InitializeAllExistingControls() {
			try {
				designerTabControl = (XtraTabControl)pnlTab.Controls[0];
			}
			catch { }
			if(EditingView.IsDesignMode) {
				btnCancel.Visible = false;
				btnOk.Visible = false;
			}
			else {
				btnCancel.Click += btnCancel_Click;
				btnOk.Click += btnOk_Click;
				footerSeparator = new LabelControl();
				footerSeparator.LineLocation = LineLocation.Center;
				footerSeparator.LineOrientation = LabelLineOrientation.Horizontal;
				footerSeparator.LineVisible = true;
				footerSeparator.AutoSizeMode = LabelAutoSizeMode.None;
				pnlBottom.Controls.Add(footerSeparator);
				footerSeparator.Location = new Point(0, pnlBottom.Height - 6);
				footerSeparator.Size = new Size(pnlBottom.Width, 6);
				footerSeparator.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				if(EditingView.IsRightToLeft) {
					CheckRTL(btnOk);
					CheckRTL(btnApply);
					CheckRTL(btnCancel);
					CheckRTL(sbPreview);
				}
			}
		}
		void CheckRTL(Control control) {
			Rectangle controlBounds = control.Bounds;
			LayoutViewRTLHelper.UpdateRTLCore(ref controlBounds, control.Parent.ClientRectangle);
			var anchor = control.Anchor;
			if(anchor == (AnchorStyles.Right | AnchorStyles.Top))
				control.Anchor = (AnchorStyles.Left | AnchorStyles.Top);
			if(anchor == (AnchorStyles.Left | AnchorStyles.Top))
				control.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
			control.Bounds = controlBounds;
		}
		protected virtual void CreateTemplateCardTabPage() {
			designerTabControl.TabPages.Insert(0);
			designerTabPage = designerTabControl.TabPages[0];
			designerTabPage.Name = "designerTabPage";
			designerTabPage.Text = Localizer.StringByID(CustomizationStringID.PageTemplateCard);
			designerSplitContainer1 = new SplitContainerControl();
			designerSplitContainer1.Name = "designerSplitContainer1";
			designerSplitContainer1.Panel1.Name = "designerSplitContainer1_Panel1";
			designerSplitContainer1.Panel2.Name = "designerSplitContainer1_Panel2";
			designerSplitContainer1.Dock = DockStyle.Fill;
			designerSplitContainer1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			designerSplitContainer2 = new SplitContainerControl();
			designerSplitContainer2.Parent = designerSplitContainer1.Panel2;
			designerSplitContainer2.Name = "designerSplitContainer2";
			designerSplitContainer2.Panel1.Name = "designerSplitContainer2_Panel1";
			designerSplitContainer2.Panel2.Name = "designerSplitContainer2_Panel2";
			designerSplitContainer2.Dock = DockStyle.Fill;
			designerSplitContainer2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			designerSplitContainer1.FixedPanel = SplitFixedPanel.Panel1;
			designerPropertiesPanel = designerSplitContainer1.Panel1;
			designerPropertiesPanel.MinSize = EditingLayoutView.ScaleWidth(215);
			designerSplitContainer2.FixedPanel = SplitFixedPanel.Panel2;
			designerCustomizationPanel = designerSplitContainer2.Panel2;
			designerCustomizationPanel.MinSize = EditingLayoutView.ScaleWidth(180);
			designerPanel = designerSplitContainer2.Panel1;
			designerPanel.Resize += new EventHandler(OnDesignerPanelResize);
			designerCardCustomizationScroller = new XtraScrollableControl();
			designerCardCustomizationScroller.Name = "designerCardCustomizationScroller";
			designerCardCustomizationScroller.Dock = DockStyle.Fill;
			designerCardCustomizationScroller.Parent = designerCustomizationPanel;
			designerCardSettingsScroller = new XtraScrollableControl();
			designerCardSettingsScroller.Name = "designerCardSettingsScroller";
			designerCardSettingsScroller.Dock = DockStyle.Fill;
			designerCardSettingsScroller.Parent = designerPropertiesPanel;
			designerTabPage.Controls.Add(designerSplitContainer1);
			templateCardCustomizationControl = new TemplateCardCustomizationControl(this, DesignerHelper.DesignerControl);
			templateCardCustomizationControl.Parent = designerCardCustomizationScroller;
			templateCardCustomizationControl.Dock = DockStyle.Fill;
			designerSettingsControl = new DesignerControlSettingsManager(this, DesignerHelper.DesignerControl);
			designerSettingsControl.Parent = designerCardSettingsScroller;
			designerSettingsControl.Dock = DockStyle.Fill;
			designerPropertiesPanel.Width = designerSettingsControl.Width;
			designerSettingsControl.BorderStyle = BorderStyle.None;
			templateCardCustomizationControl.BorderStyle = BorderStyle.None;
			designerSplitContainer1.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			designerSplitContainer1.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			designerSplitContainer2.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			designerSplitContainer2.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			if(EditingView != null) {
				SetControlLookAndFeel(designerTabControl, EditingView.LookAndFeel);
				SetControlLookAndFeel(designerSplitContainer1, EditingView.LookAndFeel);
				SetControlLookAndFeel(designerSplitContainer2, EditingView.LookAndFeel);
				SetControlLookAndFeel(designerCardSettingsScroller, EditingView.LookAndFeel);
				SetControlLookAndFeel(designerSettingsControl.layoutControl1, EditingView.LookAndFeel);
				SetControlLookAndFeel(templateCardCustomizationControl.layoutControl, EditingView.LookAndFeel);
			}
			designerTabControl.SelectedPageChanged += new TabPageChangedEventHandler(OnDesignerTabControlPageChanged);
			designerSettingsControl.ActivateCustomization();
		}
		protected static void SetControlLookAndFeel(ISupportLookAndFeel control, UserLookAndFeel lf) {
			control.LookAndFeel.UseDefaultLookAndFeel = true;
			control.LookAndFeel.Assign(lf);
		}
		protected void OnDesignerPanelResize(object sender, EventArgs e) {
			if(fInitialized) {
				if(!DesignerHelper.sizeGrip.IsDraggingState) DesignerHelper.PlaceInParentControl(designerPanel.ClientSize);
			}
		}
		bool isPreviewDataCreated = false;
		protected void OnDesignerTabControlPageChanged(object sender, TabPageChangedEventArgs e) {
			bool fTemplatePageVisible = TemplateCardPageVisible;
			SetExistingControlsEnabledState(!fTemplatePageVisible);
			UpdatePreviewIfNeed();
			if(e.PrevPage == designerTabControl.TabPages[1]) {
				DesignerHelper.DesignerControl.Root.GroupBordersVisible = DesignerView.CalcCardGroupBordersVisibility();
			}
			if(PreviewPageVisible && !isPreviewDataCreated) {
				CreatePreviewData();
				BindPreview();
				isPreviewDataCreated = true;
			}
		}
		public virtual bool PreviewPageVisible {
			get { return designerTabControl.SelectedTabPageIndex == 1; }
			set {
				if(PreviewPageVisible == value) return;
				if(value) designerTabControl.SelectedTabPageIndex = 1;
				else TemplateCardPageVisible = true;
			}
		}
		public virtual bool TemplateCardPageVisible {
			get { return designerTabControl.SelectedTabPageIndex == 0; }
			set {
				if(TemplateCardPageVisible == value) return;
				if(value) designerTabControl.SelectedTabPageIndex = 0;
				else PreviewPageVisible = true;
			}
		}
		protected void UpdatePreviewIfNeed() {
			if(!fInitialized || !PreviewPageVisible) return;
			using(new WaitCursor()) {
				DesignerView.BeginUpdate();
				DesignerHelper.CorrectItemNames(DesignerHelper.DesignerControl.Root as LayoutViewCard, EditingView as LayoutView);
				DesignerHelper.SynchronizeViewFromDesigner(DesignerView);
				DesignerView.EndUpdate();
			}
		}
		protected virtual void SetExistingControlsEnabledState(bool state) {
			sbPreview.Enabled = fFakeDataSourceUsing && !fPreviewDataLoaded;
		}
		public BaseView EditingView { get { return EditingObject as BaseView; } }
		public LayoutView EditingLayoutView { get { return EditingObject as LayoutView; } }
		protected internal void ProcessClosing() {
			if(ModificationExist) {
#if DEBUGTEST
				bool fNeedApplyFromMsgBox = EditingLayoutView.fInternalAutoAnswerYesInDesigner;
#else
				bool fNeedApplyFromMsgBox = XtraMessageBox.Show(LookAndFeel.ParentLookAndFeel, this, ModifiedMessage, CustomizationFormCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
#endif
				if(fNeedApplyFromMsgBox) ApplyLayouts();
				ModificationExist = false;
			}
		}
		protected override DBAdapter CreateDBAdapter() {
			ArrayList adapters = new ArrayList();
			ComponentCollection components = GetCurrentComponents();
			if(components == null) return null;
			foreach(object comp in components)
				adapters.Add(comp);
			return new DBAdapter(adapters, EditingView.GridControl.DataSource, EditingView.GridControl.DataMember);
		}
		protected ComponentCollection GetCurrentComponents() {
			if(EditingView.GridControl.Container != null) return EditingView.GridControl.Container.Components;
			if(EditingView.Container != null) return EditingView.Container.Components;
			return null;
		}
		protected override DataTable CreateDataTableAdapter() {
			if(EditingView.GridControl.DataSource == null) return null;
			try {
				CurrencyManager manager = EditingView.GridControl.BindingContext[EditingView.GridControl.DataSource, EditingView.GridControl.DataMember] as CurrencyManager;
				if(manager == null) return null;
				DataView dv = manager.List as DataView;
				if(dv != null) return dv.Table;
			}
			catch { }
			return null;
		}
		protected override Control CreatePreviewControl() {
			previewSplitContainer = new SplitContainerControl();
			previewSplitContainer.Dock = DockStyle.Fill;
			previewSplitContainer.Name = "previewSplitContainer";
			previewPropertiesPanel = previewSplitContainer.Panel1;
			previewPropertiesPanel.MinSize = EditingLayoutView.ScaleWidth(230);
			previewPanel = previewSplitContainer.Panel2;
			designerGrid = new GridControl();
			designerGrid.Dock = DockStyle.Fill;
			PerformGridAssign(EditingLayoutView, designerGrid);
			designerGrid.Tag = "Design";
			designerViewCore = designerGrid.MainView;
			BindPreview();
			designerGrid.Parent = previewPanel;
			designerGrid.Dock = DockStyle.Fill;
			InitView();
			designerViewSettingsScroller = new XtraScrollableControl();
			designerViewSettingsScroller.Name = "designerViewSettingsScroller";
			designerViewSettingsScroller.Dock = DockStyle.Fill;
			designerViewSettingsScroller.Parent = previewPropertiesPanel;
			previewSettingsControl = new ViewSettingsManager(this, DesignerView);
			previewSettingsControl.Parent = designerViewSettingsScroller;
			previewSettingsControl.Dock = DockStyle.Fill;
			SetControlLookAndFeel(previewSplitContainer, EditingLayoutView.LookAndFeel);
			SetControlLookAndFeel(previewPropertiesPanel, EditingLayoutView.LookAndFeel);
			SetControlLookAndFeel(previewPanel, EditingLayoutView.LookAndFeel);
			SetControlLookAndFeel(previewSettingsControl.layoutControl1, EditingLayoutView.LookAndFeel);
			SetControlLookAndFeel(designerViewSettingsScroller, EditingLayoutView.LookAndFeel);
			previewSettingsControl.BorderStyle = BorderStyle.None;
			previewSplitContainer.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			previewSplitContainer.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			return previewSplitContainer;
		}
		void BindPreview() {
			if(dataSet != null && !fTryUseEdittingObjectDataSource) {
				designerGrid.DataSource = dataSet.Tables[tableName];
			}
			else {
				designerGrid.DataSource = EditingView.DataSource;
				fPreviewDataLoaded = fTryUseEdittingObjectDataSource;
			}
			sbPreview.Enabled = fFakeDataSourceUsing;
		}
		internal static void PerformGridAssign(LayoutView sourceView, GridControl grid) {
			AttachedProperty.SetValue(grid, LayoutView.IsCustomizationRestoringInProgressProperty, true);
			AttachedProperty.SetValue(grid, LayoutView.DesignerAutoScaleFactor, sourceView.GridControl.ScaleFactor);
			GridAssign.GridControlAssign(sourceView.GridControl, sourceView, grid, true, true);
			AttachedProperty.ClearValue(grid, LayoutView.IsCustomizationRestoringInProgressProperty);
		}
		protected override void OnFillGrid() {
			base.OnFillGrid();
			fPreviewDataLoaded = true;
			sbPreview.Enabled=false;
			if(TemplateCardPageVisible) {
				PreviewPageVisible = true;
			} else
				UpdatePreviewIfNeed();
		}
		protected override void ApplyLayouts() {
			try {
				DesignerHelper.OnChanging();
				DesignerHelper.LockModification();
				EditingLayoutView.BeginUpdate();
				LayoutViewDesignerHelper.ClearView(EditingLayoutView, true, true, true);
				DesignerHelper.CorrectItemNames(DesignerHelper.DesignerControl.Root as LayoutViewCard, EditingLayoutView);
				if(designerPanel.Visible) {
					DesignerHelper.SynchronizeViewFromDesigner(DesignerView);
				}
				DesignerView.TemplateCard.Update();
				DesignerHelper.SynchronizeViewByView(DesignerView, EditingLayoutView);
				SyncTags(DesignerView, EditingLayoutView);
				DesignerHelper.CheckViewDesignTimeComponents(EditingLayoutView);
				DesignerHelper.UnLockModification();
				EditingLayoutView.EndUpdate();
			}
			finally {
				DesignerHelper.OnChanged();
				ModificationExist = false;
			}
		}
		protected internal void OnRestoreLayoutFromXml(string fileName) {
			RestoreLayoutFromXml(fileName);
		}
		protected internal void OnSaveLayoutFromXml(string fileName) {
			SaveLayoutToXml(fileName);
		}
		protected override void RestoreLayoutFromXml(string fileName) {
			DesignerView.RestoreLayoutFromXml(fileName, OptionsLayoutBase.FullLayout);
			CorrectViewItemsSiteNameAfterRestore(DesignerView, EditingLayoutView.Site);
			DesignerView.Refresh();
			DesignerHelper.SynchronizeDesignerFromView(DesignerView);
			ModificationExist = true;
		}
		protected virtual void CorrectViewItemsSiteNameAfterRestore(LayoutView view, ISite site) {
			if(site==null) return;
			Dictionary<string, IComponent> siteComponents = new Dictionary<string, IComponent>();
			foreach(IComponent c in site.Container.Components) {
				if(!siteComponents.ContainsKey(c.Site.Name)) siteComponents.Add(c.Site.Name, c);
			}
			ArrayList viewItems = new ArrayList(view.Items);
			foreach(BaseLayoutItem item in viewItems) {
				if(!siteComponents.ContainsKey(item.Name)) continue;
				List<string> names = new List<string>();
				foreach(string s in siteComponents.Keys) names.Add(s);
				foreach(BaseLayoutItem i in viewItems) { if(!names.Contains(i.Name))names.Add(i.Name); }
				LayoutViewCard card = item as LayoutViewCard;
				LayoutViewField field = item as LayoutViewField;
				string prefix = "item";
				if(field!=null) {
					string columnSuffix = string.Empty;
					if(field.Column.FieldName!=null && field.Column.FieldName!=string.Empty) {
						columnSuffix = "_col"+field.Column.FieldName;
					}
					prefix = "layoutViewField"+columnSuffix;
				}
				if(card!=null) prefix = "layoutViewCard";
				item.Name = LayoutViewDesignerHelper.CreateUniqueName(prefix, item, names);
			}
		}
		protected override void SaveLayoutToXml(string fileName) {
			try {
				DesignerView.SaveLayoutToXml(fileName, OptionsLayoutBase.FullLayout);
			}
			catch(Exception ex) { DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source); }
		}
		#region IDXMenuManagerProvider Members
		Utils.Menu.IDXMenuManager Utils.Menu.IDXMenuManagerProvider.MenuManager {
			get {
				if(EditingLayoutView == null || EditingLayoutView.GridControl == null) 
					return DevExpress.Utils.Menu.MenuManagerHelper.GetMenuManager(LookAndFeel);
				return EditingLayoutView.GridControl.MenuManager ??
					DevExpress.Utils.Menu.MenuManagerHelper.GetMenuManager(EditingLayoutView.LookAndFeel, EditingLayoutView.GridControl);
			}
		}
		#endregion
	}
	public class LayoutViewDesigner : CardViewDesigner {
		protected override DesignerGroup CreateDefaultMainGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Main, Properties.Resources.GroupMainCaption, Properties.Resources.GroupMainDescription, GetDefaultGroupImage(0), true);
			group.Add(Properties.Resources.ItemViewsCaption, Properties.Resources.ItemViewsDescription, "DevExpress.XtraGrid.Frames.LevelStyle", GetDefaultLargeImage(0), GetDefaultSmallImage(0), true);
			group.Add(Properties.Resources.ItemColumnsCaption, Properties.Resources.ItemColumnsCardDescription, "DevExpress.XtraGrid.Frames.ColumnDesigner", GetDefaultLargeImage(1), GetDefaultSmallImage(1), null);
			group.Add(Properties.Resources.ItemFeatureBrowserCaption, Properties.Resources.ItemFeatureBrowserDescription, "DevExpress.XtraGrid.FeatureBrowser.FeatureBrowserGridMainFrame", GetDefaultLargeImage(3), GetDefaultSmallImage(3));
			group.Add(Properties.Resources.ItemLayoutCaption, Properties.Resources.ItemLayoutDescription, "DevExpress.XtraGrid.Views.Layout.Designer.LayoutViewCustomizer", GetDefaultLargeImage(4), GetDefaultSmallImage(4), null, false);
			return group;
		}
		protected override void CreateGroups() {
			base.CreateGroups();
			CreateDefaultMainGroup();
			CreateDefaultStyleGroup();
			CreateDefaultRepositoryGroup();
			CreateDefaultPrintingGroup();
		}
		protected override DesignerGroup CreateDefaultPrintingGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Printing, Properties.Resources.GroupPrintingCaption, Properties.Resources.GroupPrintingDescription, GetDefaultGroupImage(3));
			group.Add(Properties.Resources.ItemPrintAppearancesCaption, Properties.Resources.ItemPrintAppearancesDescription, "DevExpress.XtraGrid.Frames.PrintAppearancesDesigner", GetDefaultLargeImage(12), GetDefaultSmallImage(12), null, false);
			group.Add(Properties.Resources.ItemPrintingSettingsCaption, Properties.Resources.ItemPrintingSettingsDescription, "DevExpress.XtraGrid.Frames.LayoutViewPrintDesigner", GetDefaultLargeImage(11), GetDefaultSmallImage(11));
			return group;
		}
		protected override DesignerGroup CreateDefaultStyleGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Appearance, Properties.Resources.GroupAppearanceCaption, Properties.Resources.GroupAppearanceDescription, GetDefaultGroupImage(1), true);
			group.Add(Properties.Resources.ItemAppearancesCaption, Properties.Resources.ItemAppearancesDescription, "DevExpress.XtraGrid.Frames.AppearancesDesigner", GetDefaultLargeImage(8), GetDefaultSmallImage(8), null, false);
			group.Add(Properties.Resources.ItemFormatConditionsCaption, Properties.Resources.ItemFormatConditionsDescription, "DevExpress.XtraGrid.Frames.StyleFormatConditionFrame", GetDefaultLargeImage(9), GetDefaultSmallImage(9), null, false);
			group.Add(Properties.Resources.ItemStyleSchemesCaption, Properties.Resources.ItemStyleSchemesDescription, "DevExpress.XtraGrid.Frames.SchemeDesigner", GetDefaultLargeImage(10), GetDefaultSmallImage(10), null, false);
			return group;
		}
	}
	[ToolboxItem(false)]
	public class DesignerSizeGrip : SimpleButton {
		enum GripState { Nothing, TryStartDragging, Dragging };
		LayoutControl controlToSizing =null;
		LayoutViewCustomizer customizerCore = null;
		Size minControlSize=Size.Empty;
		Size customizedControlSize=Size.Empty;
		SkinElementInfo skinInfoGrip = null;
		GripState state = GripState.Nothing;
		int startDragDistance = 3;
		public Size MinControlSize {
			get { return minControlSize; }
			set { minControlSize = value; }
		}
		public Size CustomizedControlSize {
			get { return customizedControlSize; }
			set { customizedControlSize = value; }
		}
		public void SetParentContainer(Control parentContainer) {
			Parent = parentContainer;
		}
		public DesignerSizeGrip(LayoutControl controlToSizing, LayoutViewCustomizer customizer) {
			this.controlToSizing = controlToSizing;
			this.customizerCore = customizer;
			Cursor = Cursors.SizeNWSE;
			Size = new Size(24, 24);
			skinInfoGrip = CreateGripScinElementInfo(new Rectangle(Point.Empty, Size));
		}
		protected virtual SkinElementInfo CreateGripScinElementInfo(Rectangle r) {
			return new SkinElementInfo(CommonSkins.GetSkin(this.LookAndFeel)[CommonSkins.SkinSizeGrip], r);
		}
		protected LayoutViewCustomizer Customizer { get { return customizerCore; } }
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, skinInfoGrip);
				} else {
					e.Graphics.Clear(SystemColors.Control);
					e.Graphics.DrawImage(SizeGripFlatImage, new Rectangle(new Point(3, 3), new Size(16, 16)));
				}
			}
		}
		Image sizeGripFlatImageCore = null;
		Image SizeGripFlatImage {
			get {
				if(sizeGripFlatImageCore==null) sizeGripFlatImageCore = CreateSizeGripFlatImage();
				return sizeGripFlatImageCore;
			}
		}
		protected virtual Image CreateSizeGripFlatImage() {
			Bitmap bmp = new Bitmap(16,16);
			using(Graphics g =  Graphics.FromImage(bmp)) {
				ControlPaint.DrawSizeGrip(g, SystemColors.ControlText, new Rectangle(Point.Empty, new Size(16, 16)));
			}
			return bmp;
		}
		protected internal void ProcessMouseEvent(Point pt, EventType eventType) {
			MouseEventArgs ea = new MouseEventArgs(MouseButtons.Left, 0, pt.X, pt.Y, 0);
			switch(eventType) {
				case EventType.MouseDown: OnMouseDown(ea); break;
				case EventType.MouseUp: OnMouseUp(ea); break;
				case EventType.MouseMove: OnMouseMove(ea); break;
			}
		}
		Point startPoint = Point.Empty;
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			state =  GripState.TryStartDragging;
			startPoint = e.Location;
		}
		public bool IsDraggingState { get { return state ==  GripState.Dragging; } }
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if((e.Button & MouseButtons.Left)==0) return;
			Point currentPoint = e.Location;
			switch(state) {
				case GripState.Dragging: DoDragging(currentPoint); break;
				case GripState.TryStartDragging: if(CanStartDrag(currentPoint)) StartDragging(currentPoint); break;
			}
		}
		protected bool CanStartDrag(Point currentPoint) {
			return Math.Max(Math.Abs(startPoint.X - currentPoint.X), Math.Abs(startPoint.Y - currentPoint.Y)) >= startDragDistance;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(state ==  GripState.Dragging) EndDragging(e.Location);
		}
		protected virtual void StartDragging(Point p) {
			state =  GripState.Dragging;
			this.SendToBack();
		}
		protected virtual void DoDragging(Point p) {
			Customizer.DesignerHelper.lockMoveCounter++;
			Customizer.DesignerHelper.fCustomizationSizeGripMoving++;
			try {
				Size offset = new Size(p.X - startPoint.X, p.Y - startPoint.Y);
				if(Math.Max(Math.Abs(offset.Width), Math.Abs(offset.Height))<4) return;
				Size newSize = CalcDraggingSize(offset);
				if(newSize.Width < MinControlSize.Width) newSize.Width = MinControlSize.Width;
				if(newSize.Height < MinControlSize.Height) newSize.Height = MinControlSize.Height;
				int dx = (newSize.Width > MinControlSize.Width) ? offset.Width : 0;
				int dy = (newSize.Height>MinControlSize.Height) ? offset.Height : 0;
				if(dx != 0 || dy != 0) {
					controlToSizing.Size = newSize;
					Point newLocation = GetSizedControlLocation(newSize, Parent.ClientSize, GetParentAutoScrollPosition());
					if(controlToSizing.Location!=newLocation) controlToSizing.Location = newLocation;
					Location = GetSizeGripLocation(controlToSizing.Location, newSize);
				}
			}
			finally {
				Customizer.DesignerHelper.fCustomizationSizeGripMoving--;
				Customizer.DesignerHelper.lockMoveCounter--; 
			}
		}
		protected Point GetParentAutoScrollPosition(){
			if(Parent is XtraScrollableControl){
				return ((XtraScrollableControl)Parent).AutoScrollPosition;
			}
			return Point.Empty;
		}
		protected internal Point GetSizeGripLocation(Point controlToSizingLocation, Size controlToSizingSize) {
			return controlToSizingLocation + (controlToSizingSize - new Size(12, 12));
		}
		protected internal Point GetSizedControlLocation(Size cardSize, Size parentSize, Point scrollOffset) {
			return GetTopLeftAlignedLocation(cardSize, parentSize, scrollOffset);
		}
		protected virtual Size CalcDraggingSize(Size offset) {
			return controlToSizing.Size + offset;
		}
		protected Point GetCenteredLocation(Size cardSize, Size parentSize) {
			Point location  = new Point((parentSize.Width-cardSize.Width)/2, (parentSize.Height-cardSize.Height)/2);
			if(!IsDraggingState) {
				if(cardSize.Height > parentSize.Height) location.Y = 10;
				if(cardSize.Width > parentSize.Width) location.X = 10;
			}
			return location;
		}
		protected Point GetTopLeftAlignedLocation(Size cardSize, Size parentSize, Point scrollOffset) {
			return new Point(10 + scrollOffset.X, 10 + scrollOffset.Y);
		}
		protected virtual void EndDragging(Point p) {
			state =  GripState.Nothing;
			CustomizedControlSize = controlToSizing.Size;
			controlToSizing.Location = GetSizedControlLocation(CustomizedControlSize, Parent.ClientSize, GetParentAutoScrollPosition());
			Location = GetSizeGripLocation(controlToSizing.Location, CustomizedControlSize);
			Customizer.ModificationExist=true;
		}
	}
}
