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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Resizing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.ViewInfo;
namespace DevExpress.XtraLayout {
	public class LayoutControlInternalException : Exception {
		public LayoutControlInternalException(string reason) : base(reason) { }
		public LayoutControlInternalException(string reason, Exception innerException)
			: base(reason, innerException) {
		}
	}
	public class LayoutControlException : Exception {
		public LayoutControlException(string reason) : base(reason) { }
		public LayoutControlException(string reason, Exception innerException)
			: base(reason, innerException) {
		}
	}
#if DXWhidbey
	public abstract class SupportVisitor : Component {
#else
	public abstract class SupportVisitor : Component {
#endif
		abstract public void Accept(BaseVisitor visitor);
	}
	[ToolboxItem(false)]
	public abstract class BaseLayoutItem : SupportVisitor, ISupportInitialize, ISupportPropertyGridWrapper, IXtraSerializable, ICoverageFixerElement, LayoutStyleManagerClient, IBindableComponent {
		static FieldInfo fieldInfo_events;
		static BaseLayoutItem() {
			Type type = typeof(Component);
			fieldInfo_events = type.GetField("events", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		Point locationCore;
		Size size;
		Size textSize;
		LayoutGroup parent;
		BaseLayoutItem originalItem;
		ILayoutControl ownerControl;
		AppearanceObject appearanceItemCaption;
		AppearanceObject paintAppearanceItemCaptionCore = null;
		string text, customizationFormText;
		LayoutVisibility layoutVisibilityCore;
		object tag = null;
		protected ViewInfo.BaseLayoutItemViewInfo viewInfoCore;
		protected BaseLayoutItemHandler handlerInternal;
		protected int updateCount;
		protected bool isTextVisible;
		bool visible = true;
		bool allowHideItem = true;
		bool startNewLine = false;
		protected internal virtual bool NeedSupressDrawBorder { get { return false; } }
		protected internal bool shouldUpdateViewInfo = true, shouldResetBorderInfoCore = false;
		Size minSize, maxSize;
		Utils.Padding userSpaces = DevExpress.XtraLayout.Utils.Padding.Invalid, userPaddings = DevExpress.XtraLayout.Utils.Padding.Invalid;
		protected Locations textLocationCore;
		protected Size DefaultMaxSize = Size.Empty;
		protected bool showInCustomizationFormCore = true;
		BaseLayoutItemOptionsToolTip optionsToolTipCore = null;
		BaseLayoutItemToolTipOptions optionsItemToolTipCore = null;
		BaseLayoutItemToolTipOptions optionsIconToolTipCore = null;
		BaseLayoutItemCustomizationOptions optionsCustomization = null;
		OptionsTableLayoutItem optionsTableLayoutItem = null;
		private static readonly object mouseDown = new object();
		private static readonly object mouseUp = new object();
		private static readonly object click = new object();
		private static readonly object doubleClick = new object();
		private static readonly object hiddenEvent = new object();
		private static readonly object shownEvent = new object();
		private static readonly object hidingEvent = new object();
		private static readonly object showingEvent = new object();
		private static readonly object textChanged = new object();
		protected static readonly object selectedPageChangedCore = new object();
		protected static readonly object selectedPageChangingCore = new object();
		internal BaseLayoutItem(LayoutGroup parent) {
			SetPropertiesDefault();
			InitializeOptionsCustomization();
			InitializeOptionsToolTip();
			this.appearanceItemCaption = new AppearanceObject();
			this.parent = parent;
			this.size = Size.Empty;
			this.text = String.Empty;
			this.customizationFormText = String.Empty;
			layoutVisibilityCore = LayoutVisibility.Always;
			this.appearanceItemCaption.Changed += OnAppearanceChanged;
			if(!this.IsGroup)
				textSize = DefaultTextSize;
			minSize = Size.Empty;
			textLocationCore = Locations.Default;
		}
		protected internal virtual void SetPropertiesDefault() {
			isTextVisible = true;
		}
		bool ShouldSerializeOptionsCustomization() { return OptionsCustomization.ShouldSerializeCore(this); }
		[ Category("Customization")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public virtual BaseLayoutItemCustomizationOptions OptionsCustomization {
			get { return optionsCustomization; }
			set { optionsCustomization = value; }
		}
		[ Category("TableItem")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public virtual OptionsTableLayoutItem OptionsTableLayoutItem {
			get {
				if(optionsTableLayoutItem == null) optionsTableLayoutItem = new OptionsTableLayoutItem(this);
				return optionsTableLayoutItem;
			}
		}
		bool ShouldSerializeOptionsToolTip() { return OptionsToolTip.ShouldSerializeCore(this); }
		[ Category("ToolTip")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public virtual BaseLayoutItemOptionsToolTip OptionsToolTip {
			get { return optionsToolTipCore; }
			set { optionsToolTipCore = value; }
		}
		bool ShouldSerializeOptionsItemToolTip() { return OptionsToolTip.ShouldSerializeCore(this); }
		[Obsolete("Use OptionsToolTip instead")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BaseLayoutItemToolTipOptions OptionsItemToolTip {
			get { return optionsItemToolTipCore; }
			set { optionsItemToolTipCore = value; }
		}
		bool ShouldSerializeOptionsIconToolTip() { return OptionsToolTip.ShouldSerializeCore(this); }
		[Obsolete("Use OptionsToolTip instead")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual BaseLayoutItemToolTipOptions OptionsIconToolTip {
			get { return optionsIconToolTipCore; }
			set { optionsIconToolTipCore = value; }
		}
		protected virtual void InitializeOptionsCustomization() {
			optionsCustomization = CreateOptionsCustomization();
		}
		protected virtual void DisposeOptionsCustomization() {
			optionsCustomization = null;
		}
		protected virtual void InitializeOptionsToolTip() {
			optionsToolTipCore = CreateOptionsToolTip();
			optionsItemToolTipCore = CreateOptionsToolTipObsolete();
			optionsIconToolTipCore = CreateOptionsToolTipObsolete();
		}
		protected virtual void DisposeOptionsToolTip() {
			optionsToolTipCore = null;
			optionsItemToolTipCore = null;
			optionsIconToolTipCore = null;
		}
		protected virtual BaseLayoutItemCustomizationOptions CreateOptionsCustomization() {
			return new BaseLayoutItemCustomizationOptions(this);
		}
		protected virtual BaseLayoutItemOptionsToolTip CreateOptionsToolTip() {
			return new BaseLayoutItemOptionsToolTip();
		}
		protected virtual BaseLayoutItemToolTipOptions CreateOptionsToolTipObsolete() {
			return new BaseLayoutItemToolTipOptions();
		}
		protected void InvalidateEnabledState() {
			if(Owner != null) {
				Owner.EnabledStateController.SetItemEnabledStateDirty(this);
			}
		}
		protected void InvalidateAppearances() {
			if(Owner != null) { Owner.AppearanceController.SetAppearanceDirty(this); }
		}
		protected internal virtual bool EnabledState {
			get {
				if(Owner == null) return true;
				return Owner.EnabledStateController.GetItemEnabledState(this);
			}
		}
		void IXtraSerializable.OnEndDeserializing(string layoutVersion) { EndInit(); }
		void IXtraSerializable.OnEndSerializing() { }
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) { BeginInit(); }
		void IXtraSerializable.OnStartSerializing() { }
#if DXWhidbey
		protected void CheckBounds() {
#if DEBUGTEST
			if(!(X >= 0 && Y >= 0 && Height > 0 && Width > 0)) {
			}
#endif
		}
#endif
		protected virtual internal bool CanCustomize {
			get {
				return Parent == null ? true : Parent.CanCustomize && Parent.AllowCustomizeChildren;
			}
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			UpdateAppearance();
			if(Owner != null && Owner is ISupportImplementor) {
				((ISupportImplementor)Owner).Implementor.OnItemAppearanceChanged(this);
			}
			if(Owner != null) Owner.SetIsModified(true);
		}
		protected virtual void UpdateAppearance() {
			InvalidateAppearances();
			ShouldResetBorderInfo = true;
			ShouldArrangeTextSize = true;
			if(IsUpdateLocked) return;
			ComplexUpdate();
		}
		protected internal virtual void RaiseMouseDown(MouseEventArgs e) {
			MouseEventHandler handler = (MouseEventHandler)this.Events[mouseDown];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseMouseUp(MouseEventArgs e) {
			MouseEventHandler handler = (MouseEventHandler)this.Events[mouseUp];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseClick(MouseEventArgs e) {
			EventHandler handler = (EventHandler)this.Events[click];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseDoubleClick(MouseEventArgs e) {
			EventHandler handler = (EventHandler)this.Events[doubleClick];
			if(handler != null) handler(this, e);
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemTextChanged")]
#endif
		public event EventHandler TextChanged {
			add { base.Events.AddHandler(textChanged, value); }
			remove { base.Events.RemoveHandler(textChanged, value); }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemShowing")]
#endif
		public event EventHandler Showing {
			add { base.Events.AddHandler(showingEvent, value); }
			remove { base.Events.RemoveHandler(showingEvent, value); }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemHiding")]
#endif
		public event EventHandler Hiding {
			add { base.Events.AddHandler(hidingEvent, value); }
			remove { base.Events.RemoveHandler(hidingEvent, value); }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemShown")]
#endif
		public event EventHandler Shown {
			add { base.Events.AddHandler(shownEvent, value); }
			remove { base.Events.RemoveHandler(shownEvent, value); }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemHidden")]
#endif
		public event EventHandler Hidden {
			add { base.Events.AddHandler(hiddenEvent, value); }
			remove { base.Events.RemoveHandler(hiddenEvent, value); }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemClick")]
#endif
		public event EventHandler Click {
			add { base.Events.AddHandler(click, value); }
			remove { base.Events.RemoveHandler(click, value); }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemDoubleClick")]
#endif
		public event EventHandler DoubleClick {
			add { base.Events.AddHandler(doubleClick, value); }
			remove { base.Events.RemoveHandler(doubleClick, value); }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemMouseDown")]
#endif
		public event MouseEventHandler MouseDown {
			add { base.Events.AddHandler(mouseDown, value); }
			remove { base.Events.RemoveHandler(mouseDown, value); }
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemMouseUp")]
#endif
		public event MouseEventHandler MouseUp {
			add { base.Events.AddHandler(mouseUp, value); }
			remove { base.Events.RemoveHandler(mouseUp, value); }
		}
		public override void Accept(BaseVisitor visitor) {
			visitor.Visit(this);
		}
		protected internal virtual bool TruncateClientAreaToMaxSize { get { return true; } }
		protected void CheckParentOwner() {
			CheckParentOwner(true, true);
		}
		protected void CheckParentOwner(bool checkParent, bool checkOwner) {
			if(Owner == null && checkOwner) throw new LayoutControlInternalException("Owner cannot be null");
			if(Parent == null && checkParent) throw new LayoutControlInternalException("Parent cannot be null");
			if(DisposingFlag) throw new LayoutControlInternalException("Using disposed item");
		}
		int restoringFromCustomization = 0;
		protected bool IsRestoringFromCustomization {
			get { return restoringFromCustomization > 0; }
		}
		bool isRTLCore = false;
		protected internal virtual bool IsRTL { get { return isRTLCore; } }
		protected internal virtual void SetRTL(bool isRTL, bool updatePosition) {
			if(isRTLCore == isRTL) return;
			shouldResetBorderInfoCore = true;
			shouldUpdateViewInfo = true;
			isRTLCore = isRTL;
			if(updatePosition) OnRTLChanged();
		}
		protected virtual void OnRTLChanged() {
			Rectangle transformRectange = RTLHelper.Transform(Bounds, Parent == null ? this.Width : Parent.ViewInfo.ClientArea.Width);
			locationCore = transformRectange.Location;
			size = transformRectange.Size;
			if(optionsTableLayoutItem != null && parent != null && parent.LayoutMode == LayoutMode.Table) {
				optionsTableLayoutItem.ColumnIndex = parent.OptionsTableLayoutGroup.ColumnCount - optionsTableLayoutItem.ColumnIndex - optionsTableLayoutItem.ColumnSpan;
			}
		}
		public virtual void RestoreFromCustomization() {
			if(IsRestoringFromCustomization) return;
			OnItemRestoring();
			using(new SafeBaseLayoutItemChanger(this)) {
				restoringFromCustomization++;
				CheckParentOwner(false, true);
				shouldRiseShowOnRestore = visible;
				Owner.RootGroup.AddCore(this);
				if(Parent != null) OnItemRestoredCore();
				ShouldArrangeTextSize = true;
				restoringFromCustomization--;
			}
			ComplexUpdate();
		}
		public virtual void RestoreFromCustomization(LayoutGroup parent) {
			if(IsRestoringFromCustomization) return;
			OnItemRestoring();
			using(new SafeBaseLayoutItemChanger(this)) {
				restoringFromCustomization++;
				CheckParentOwner(false, true);
				if(parent == null)
					throw new LayoutControlInternalException("parameter parent cannot be null");
				shouldRiseShowOnRestore = visible;
				parent.Add(this);
				if(Parent != null) OnItemRestoredCore();
				restoringFromCustomization--;
			}
			ComplexUpdate();
		}
		public virtual void RestoreFromCustomization(LayoutItemDragController controller) {
			if(IsRestoringFromCustomization) return;
			OnItemRestoring();
			using(new SafeBaseLayoutItemChanger(this)) {
				restoringFromCustomization++;
				shouldRiseShowOnRestore = visible;
				CheckParentOwner(false, true);
				if(controller.Item.Parent == null)
					throw new LayoutControlInternalException("parameter parent cannot be null");
				Move(controller);
				if(Parent != null) OnItemRestoredCore();
				restoringFromCustomization--;
			}
			ComplexUpdate();
		}
		public virtual void RestoreFromCustomization(BaseLayoutItem baseItem, InsertType insertType) {
			if(IsRestoringFromCustomization) return;
			OnItemRestoring();
			using(new SafeBaseLayoutItemChanger(this)) {
				restoringFromCustomization++;
				CheckParentOwner(false, true);
				shouldRiseShowOnRestore = visible;
				if(baseItem.Parent == null) throw new LayoutControlInternalException("parameter parent cannot be null");
				Move(baseItem, insertType);
				if(Parent != null) OnItemRestoredCore();
				restoringFromCustomization--;
			}
			ComplexUpdate();
		}
		const string CustomizationParentName = "Customization";
		internal void SetCustomizationParentName() {
			ParentName = CustomizationParentName;
		}
		internal bool HasCustomizationParentName {
			get {
				if(ParentName == CustomizationParentName) return true;
#pragma warning disable 618
				return ParentName == LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.CustomizationParentName);
#pragma warning restore 618
			}
		}
		protected virtual void HideToCustomizationSmart_toBeRemoved() {
			if(!AllowHide || IsHidden) return;
			if(Parent == null) return;
			LayoutGroup parent = Parent;
			try {
				Parent.ResizeManager.AllowResetResizer = false;
				ILayoutControl iOwner = Owner;
				LayoutGroup iParent = Parent;
				CheckParentOwner();
				using(new SafeBaseLayoutItemChanger(this)) {
					Owner.HiddenItems.Add(this);
					parent.allowChangeComponents = false;
					parent.Items.Remove(this);
					if(Owner != null) Owner.FireItemRemoved(this, EventArgs.Empty);
					parent.allowChangeComponents = true;
					if(!IsDisposing)
						ViewInfo.Offset = new Point(int.MinValue, int.MinValue);
					UpdateChildren(false);
					Parent = null;
					SetCustomizationParentName();
					Owner = iOwner;
					ShouldArrangeTextSize = true;
				}
				ComplexUpdate();
				if(this is EmptySpaceItem && Owner != null && Owner.DesignMode) {
					Dispose();
				}
			} finally {
				parent.ResizeManager.AllowResetResizer = true;
			}
		}
		public virtual void HideToCustomization() {
			if(!AllowHide || IsHidden) return;
			if(Parent == null) return;
			LayoutGroup parent = Parent;
			ILayoutControl iOwner = Owner;
			CheckParentOwner();
			OnItemHiding();
			using(new SafeBaseLayoutItemChanger(this)) {
				Owner.HiddenItems.Add(this);
				parent.allowChangeComponents = false;
				parent.Remove(this);
				if(Owner != null) Owner.FireItemRemoved(this, EventArgs.Empty);
				parent.allowChangeComponents = true;
				if(!IsDisposing)
					ViewInfo.Offset = new Point(int.MinValue, int.MinValue);
				UpdateChildren(false);
				Parent = null;
				SetCustomizationParentName();
				Owner = iOwner;
				ShouldArrangeTextSize = true;
				ComplexUpdate();
				RootSelect();
				if(this is EmptySpaceItem && Owner != null && Owner.DesignMode) {
					Dispose();
				}
			}
		}
		private void RootSelect() {
			if(Owner != null && Owner.RootGroup != null && Owner.CustomizationForm != null && Owner.CustomizationMode == CustomizationModes.Quick) Owner.RootGroup.Selected = true;
		}
		internal void OnItemRestoredCore() {
			if(this is IFixedLayoutControlItem) {
				LayoutControlItem.InitializeItemAsFixed(this as LayoutControlItem, Owner);
			}
			OnItemRestored();
		}
		bool shouldRiseShowOnRestore = false;
		protected virtual void OnItemRestored() {
			if(Owner != null && (Owner.HiddenItems.Contains(this) || Owner.HiddenItems.FixedItems.Contains(this as IFixedLayoutControlItem))) {
				Owner.HiddenItems.Remove(this);
			}
			if(Owner != null) Owner.ShouldUpdateControls = true;
			if(shouldRiseShowOnRestore) RaiseShowHide(true);
		}
		protected virtual void OnItemRestoring() {
			RaiseShowHiding(true);
		}
		protected virtual void OnItemHiding() {
			RaiseShowHiding(false);
		}
		protected void ComplexUpdate() {
			ComplexUpdate(true);
		}
		protected void ComplexUpdate(bool shouldUpdateViewInfo) {
			ComplexUpdate(shouldUpdateViewInfo, false);
		}
		protected void ComplexUpdate(bool shouldUpdateViewInfo, bool shouldResize) {
			ComplexUpdate(shouldUpdateViewInfo, shouldResize, false);
		}
		[Localizable(false), Bindable(true), DefaultValue((string)null), TypeConverter(typeof(StringConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		protected internal BaseLayoutItem OriginalItem {
			get { return originalItem; }
			set { originalItem = value; }
		}
		protected virtual void BeginClone(ILayoutControl cloneOwner) {
			if(Owner != null) (Owner as ISupportImplementor).Implementor.iCloneCounter++;
			if(cloneOwner != null) (cloneOwner as ISupportImplementor).Implementor.iCloneCounter++;
		}
		protected virtual void EndClone(ILayoutControl cloneOwner) {
			if(cloneOwner != null) (cloneOwner as ISupportImplementor).Implementor.iCloneCounter--;
			if(Owner != null) (Owner as ISupportImplementor).Implementor.iCloneCounter--;
		}
		protected internal virtual void AssignInternal(BaseLayoutItem item) {
			this.locationCore = item.locationCore;
			this.size = item.size;
			this.maxSize = item.maxSize;
			this.minSize = item.minSize;
			this.textSize = item.textSize;
			this.visible = item.visible;
			this.isTextVisible = item.isTextVisible;
			this.textToControlDistanceCore = item.textToControlDistanceCore;
			this.layoutVisibilityCore = item.layoutVisibilityCore;
			this.actualVisibilityStatus = item.actualVisibilityStatus;
			this.shouldUpdateViewInfo = true;
		}
		public virtual BaseLayoutItem Clone(LayoutGroup cloneParent, ILayoutControl cloneOwner) {
			BaseLayoutItem clone = (BaseLayoutItem)this.MemberwiseClone();
			BeginClone(cloneOwner);
			clone.Site = null;
			clone.parent = cloneParent;
			clone.ownerControl = cloneOwner;
			clone.updateCount = 0;
			clone.BeginInit();
			CloneCommonProperties(cloneParent, cloneOwner, ref clone);
			CloneViewInfo(ref clone);
			clone.EndInit();
			EndClone(cloneOwner);
			return clone;
		}
		protected virtual void CloneViewInfo(ref BaseLayoutItem clone) {
			BaseLayoutItemViewInfo vi = this.ViewInfo;
			clone.viewInfoCore = (BaseLayoutItemViewInfo)vi.Clone();
			clone.viewInfoCore.ResetAfterClone();
			clone.viewInfoCore.Owner = clone;
		}
		protected virtual void CloneCommonProperties(LayoutGroup cloneParent, ILayoutControl cloneOwner, ref  BaseLayoutItem clone) {
			clone.paintAppearanceItemCaptionCore = null;
			clone.originalItem = this;
			clone.appearanceItemCaption = (AppearanceObject)this.appearanceItemCaption.Clone();
			clone.handlerInternal = null;
			clone.customizationFormText = (String)this.CustomizationFormText.Clone();
			clone.name = this.Name;
			clone.text = this.Text;
			clone.userSpaces = new Utils.Padding(this.userSpaces.Left, this.userSpaces.Right, this.userSpaces.Top, this.userSpaces.Bottom);
			clone.userPaddings = new Utils.Padding(this.userPaddings.Left, this.userPaddings.Right, this.userPaddings.Top, this.userPaddings.Bottom);
			ResetCloneEvents(clone);
			clone.InitializeOptionsToolTip();
			clone.OptionsToolTip.Assign(this.OptionsToolTip);
			clone.InitializeOptionsCustomization();
			clone.optionsCustomization.Assign(this.optionsCustomization);
		}
		protected void ResetCloneEvents(BaseLayoutItem clone) {
			if(fieldInfo_events != null) fieldInfo_events.SetValue(clone, null);
		}
		protected internal void ComplexUpdate(bool shouldUpdateViewInfo, bool shouldResize, bool shouldArrangeTextSize) {
			this.shouldUpdateViewInfo |= shouldUpdateViewInfo;
			this.ShouldResize |= shouldResize;
			this.ShouldArrangeTextSize |= shouldArrangeTextSize;
			this.ShouldUpdateLookAndFeel |= shouldResetBorderInfoCore;
			Invalidate();
		}
		protected void SafeRenameItem(BaseLayoutItem bli, string oldName, string name) {
			if(Owner != null) {
				foreach(BaseLayoutItem temp in Owner.Items) {
					LayoutClassificationArgs classified = LayoutClassifier.Default.Classify(temp);
					if(temp.ParentName == oldName) temp.ParentName = name;
					if(classified.IsGroup && classified.Group.TabbedGroupParentName == oldName) classified.Group.TabbedGroupParentName = name;
					if(classified.IsTabbedGroup && classified.TabbedGroup.SelectedTabPageName == oldName) classified.TabbedGroup.SelectedTabPageName = name;
				}
			}
		}
		protected internal virtual void RestoreChildren(BaseItemCollection items) {
		}
		protected internal virtual DevExpress.XtraLayout.Registrator.LayoutPaintStyle PaintStyle {
			get { return Owner == null ? DevExpress.XtraLayout.Registrator.LayoutPaintStyle.NullStyle : Owner.PaintStyle; }
		}
		protected internal bool StartChange() { if(Owner != null) return Owner.FireChanging(this); else return true; }
		protected internal void EndChange() { if(Owner != null) Owner.FireChanged(this); }
		protected internal bool CanCreateViewInfo {
			get {
				if(Owner == null) return true;
				if(Owner is LayoutControl) return true;
				if(Owner.UpdatedCount > 0) return false;
				if(!Owner.InitializationFinished) return false;
				return true;
			}
		}
		protected internal int UpdatedCount {
			get {
				if(Owner != null)
					return Owner.UpdatedCount;
				else {
					if(Parent != null)
						return Parent.UpdatedCount;
					else
						return updateCount;
				}
			}
			set {
				if(Owner != null)
					Owner.UpdatedCount = value;
				else {
					if(Parent != null)
						Parent.UpdatedCount = value;
					else
						updateCount = value;
				}
			}
		}
		protected bool ShouldResize {
			get {
				if(Owner != null) return Owner.ShouldResize;
				else return false;
			}
			set { if(Owner != null)Owner.ShouldResize = value; }
		}
		protected bool ShouldUpdateConstraints {
			get {
				if(Owner != null) return Owner.ShouldUpdateConstraints;
				else return false;
			}
			set { if(Owner != null)Owner.ShouldUpdateConstraints = value; }
		}
		protected bool ShouldUpdateConstraintsDoUpdate {
			get {
				return ShouldUpdateConstraints;
			}
			set {
				ShouldUpdateConstraints = value;
				if(Owner != null && Owner is ISupportImplementor) {
					((ISupportImplementor)Owner).Implementor.delayPaintingCore++;
					if(value == true) Invalidate();
					((ISupportImplementor)Owner).Implementor.delayPaintingCore--;
				}
			}
		}
		protected bool ShouldResetBorderInfo {
			get { return shouldResetBorderInfoCore; }
			set { shouldResetBorderInfoCore = value; }
		}
		protected bool ShouldArrangeTextSize {
			get {
				if(Owner != null) return Owner.ShouldArrangeTextSize;
				else return false;
			}
			set { if(Owner != null) Owner.ShouldArrangeTextSize = value; }
		}
		protected bool ShouldUpdateLookAndFeel {
			get {
				if(Owner != null) return Owner.ShouldUpdateLookAndFeel;
				else return false;
			}
			set { if(Owner != null) Owner.ShouldUpdateLookAndFeel = value; }
		}
		protected virtual ViewInfo.BaseLayoutItemViewInfo CreateViewInfo() {
			return new ViewInfo.BaseLayoutItemViewInfo(this);
		}
		protected internal virtual void SetShouldUpdateViewInfo() {
			shouldUpdateViewInfo = true;
			if(Parent != null) Parent.SetShouldUpdateViewInfo();
			if(Owner != null) {
				Owner.ShouldUpdateControls = true;
			}
		}
		protected void CreateViewInfoIfNeeded() {
			if(viewInfoCore == null) {
				if(IsDisposing) return;
				viewInfoCore = CreateViewInfo();
				SetShouldUpdateViewInfo();
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ViewInfo.BaseLayoutItemViewInfo ViewInfo {
			get {
				CreateViewInfoIfNeeded();
				return viewInfoCore;
			}
		}
		protected internal virtual void SetViewInfoAndPainter(ViewInfo.BaseLayoutItemViewInfo vi, Painting.BaseLayoutItemPainter painter) {
			if(viewInfoCore != null && (viewInfoCore is LayoutGroupViewInfo || viewInfoCore is XtraDashboardLayout.DashboardLayoutControlItemViewInfo)) viewInfoCore.Destroy();
			this.viewInfoCore = vi;
			ShouldResetBorderInfo = false;
		}
		int initializing = 0;
		int initialized = 0;
		public virtual void BeginInit() {
			initializing++;
			UpdatedCount++;
		}
		public virtual void EndInit() {
			UpdatedCount--;
			if(UpdatedCount < 0)
				UpdatedCount = 0;
			if(--initializing == 0)
				initialized++;
		}
		protected bool IsInitializing {
			get { return initializing > 0; }
		}
		protected bool IsInitialized {
			get { return initializing == 0 && initialized > 0; }
		}
		bool isOwnerUpdating = false;
		protected internal bool IsOwnerInvalidating {
			get { return isOwnerUpdating; }
			set { isOwnerUpdating = value; }
		}
		[Browsable(false)]
		public bool IsUpdateLocked {
			get { return (UpdatedCount != 0 || IsOwnerInvalidating); }
		}
		string name;
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string TypeName {
			get { return this.GetType().ToString(); }
		}
		protected virtual void NameChanged() { }
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), RefreshProperties(RefreshProperties.All)]
		public string Name {
			get {
				string val = name;
				if(val == null) {
					if(Site != null)
						val = Site.Name;
					if(val == null)
						val = "";
				}
				return val;
			}
			set {
				if(name == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					name = value == null || value.Length == 0 ? null : value;
					if(text == String.Empty) {
						ShouldUpdateConstraintsDoUpdate = true;
						ShouldArrangeTextSize = true;
						UpdateText();
					}
					NameChanged();
				}
			}
		}
		protected bool disposingFlagCore = false;
		protected internal virtual bool DisposingFlag {
			get { return disposingFlagCore || Parent != null && Parent.DisposingFlag || Owner != null && Owner.DisposingFlag; }
		}
		bool ShouldSerializeAppearanceItemCaption() { return AppearanceItemCaption.ShouldSerialize(); }
		void ResetAppearanceItemCaption() { AppearanceItemCaption.Reset(); }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemAppearanceItemCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceItemCaption { get { return appearanceItemCaption; } }
		void XtraDeserializeAppearanceItemCaption(XtraEventArgs ea) {
			if(Owner != null && Owner.OptionsSerialization.RestoreAppearanceItemCaption) {
				DeserializeHelper dh = new DeserializeHelper();
				AppearanceItemCaption.Reset();
				if(ea.Info != null) dh.DeserializeObject(AppearanceItemCaption, ea.Info.ChildProperties, OptionsLayoutBase.FullLayout);
			}
		}
		[Browsable(false)]
		public virtual AppearanceObject PaintAppearanceItemCaption {
			get {
				if(Owner != null)
					return Owner.AppearanceController.GetAppearanceItem(this);
				else {
					if(paintAppearanceItemCaptionCore == null) paintAppearanceItemCaptionCore = new AppearanceObject();
					return paintAppearanceItemCaptionCore;
				}
			}
		}
		[Browsable(false)]
		public bool IsDisposing {
			get { return disposingFlagCore; }
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				disposingFlagCore = true;
				if((Site != null) && (Site.Container != null)) {
					Site.Container.Remove(this);
					Site = null;
				}
				EnabledStateController ec = (Owner != null) ? Owner.EnabledStateController : null;
				AppearanceController ac = (Owner != null) ? Owner.AppearanceController : null;
				if(Owner != null && Owner.ItemsAndNames != null && Owner.ItemsAndNames.ContainsKey(Name) && Owner.ItemsAndNames[Name] == this) Owner.ItemsAndNames.Remove(Name);
				if(Owner != null && Owner.FocusHelper != null && Owner.FocusHelper.SelectedComponent != null && Owner.FocusHelper.SelectedComponent == this) Owner.FocusHelper.SelectedComponent = null;
				if(appearanceItemCaption != null) {
					this.appearanceItemCaption.Changed -= OnAppearanceChanged;
					this.appearanceItemCaption.Dispose();
					this.appearanceItemCaption = null;
				}
				if(paintAppearanceItemCaptionCore != null) {
					this.paintAppearanceItemCaptionCore.Dispose();
					this.paintAppearanceItemCaptionCore = null;
				}
				if(bindableProviderCore != null) {
					bindableProviderCore.Dispose();
					bindableProviderCore = null;
				}
				try {
					if(Parent != null) Parent.Remove(this);
				} finally {
					this.ownerControl = null;
					this.parent = null;
					DisposeOptionsToolTip();
					DisposeOptionsCustomization();
					this.originalItem = null;
					this.text = null;
					this.customizationFormText = null;
					this.name = null;
					if(ec != null) ec.ClearReferences(this);
					if(ac != null) ac.ClearReferences(this);
					ec = null;
					ac = null;
				}
			}
			base.Dispose(disposing);
		}
		string parentName;
		[XtraSerializableProperty(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public string ParentName {
			get { return parentName; }
			set { parentName = value; }
		}
		private int bestFitWeightCore = 100;
		[DefaultValue(100)]
		public virtual int BestFitWeight {
			get { return bestFitWeightCore; }
			set { bestFitWeightCore = value; }
		}
		protected internal virtual int BestFitCore() { return BestFitWeight; }
		protected void CheckContainsItems(LayoutGroup parent) {
			if(parent != null && !parent.Items.Contains(this)) {
				if(IsUpdateLocked) {
					parent.Items.Add(this);
				} else {
					parent.Items.Add(this);
					this.SetBounds(Rectangle.Empty);
					parent.AddCore(this, true);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual LayoutGroup Parent {
			get {
				return parent;
			}
			set {
				bool forse = false;
				LayoutGroup oldParent = parent;
				if(parent == null) forse = true;
				if(oldParent != value) {
					if(oldParent != null && oldParent.Items.Contains(this)) {
						if(IsUpdateLocked)
							oldParent.Items.Remove(this);
						else
							oldParent.Remove(this);
					}
					bool fParentForHiddenItem = (oldParent == null) && !IsInitializing && IsHidden;
					CheckContainsItems(value);
					parent = value;
					if(parent != null) {
						InvalidateEnabledState();
						SetRTL(parent.IsRTL,false);
					}
					if(forse && value != null && updateCount != 0) UpdatedCount += updateCount;
					updateCount = 0;
					if(parent != null)
						Owner = Parent.Owner;
					if(fParentForHiddenItem && !IsRestoringFromCustomization && !Owner.IsDeserializing)
						OnItemRestoredCore();
				} else {
					CheckContainsItems(value);
				}
			}
		}
		protected virtual Size DefaultTextSize {
			get { return new Size(50, 20); }
		}
		protected virtual Size GetLabelSize() {
			return TextSize;
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemTextVisible"),
#endif
 RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		[DefaultValue(true)]
		public virtual bool TextVisible {
			get { return isTextVisible; }
			set {
				if(isTextVisible == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					isTextVisible = value;
					ShouldResetBorderInfo = true;
					ShouldUpdateConstraintsDoUpdate = true;
					ShouldArrangeTextSize = true;
					if(Owner != null) Owner.SetIsModified(true);
					ComplexUpdate();
				}
			}
		}
		protected virtual bool GetTextVisible() {
			return TextVisible || (Owner != null ? Owner.TextAlignManager.GetAlignHiddenText(this) : false);
		}
		protected virtual Size GetCustomTextSize() {
			return GetTextVisible() ? textSize : Size.Empty;
		}
		protected virtual Size GetTextSize() {
			if(Owner == null) return GetCustomTextSize();
			else {
				int result = Owner.TextAlignManager.GetTextWidth(this);
				if(result < 0)
					return CalcScaledSize(GetCustomTextSize());
				else {
					int height = Owner.TextAlignManager.GetTextHeight(this);
					return new Size(result, TextVisible ? height : 0);
				}
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemTextSize"),
#endif
 RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		public virtual Size TextSize {
			get {
				return GetTextSize();
			}
			set {
				using(new SafeBaseLayoutItemChanger(this)) {
					value = CalcDeScaledSize(value);
					if(GetTextVisible()) {
						textSize = value;
					} else {
						textSize = new Size(value.Width, textSize.Height);
					}
					ShouldUpdateConstraintsDoUpdate = true;
					ShouldArrangeTextSize = true;
					ComplexUpdate();
				}
			}
		}
		protected virtual bool ShouldSerializeTextToControlDistance() {
			return GetTextToControlDistance() == GetCustomTextToControlDistance();
		}
		protected virtual int GetCustomTextToControlDistance() {
			return GetTextVisible() ? textToControlDistanceCore : 0;
		}
		internal bool IsDefaultTextToControlDistance {
			get {
				if(Owner == null || Owner.TextAlignManager.GetTextToControlDistance(this) < 0) return false;
				else return true;
			}
		}
		protected virtual int GetTextToControlDistance() {
			if(Owner == null) return GetCustomTextToControlDistance();
			else {
				int result = Owner.TextAlignManager.GetTextToControlDistance(this);
				if(result < 0)
					result = GetCustomTextToControlDistance();
				return result;
			}
		}
		protected virtual void XtraDeserializeTextToControlDistance(XtraEventArgs e) {
			if(Owner == null) TextToControlDistance = int.Parse((string)e.Info.Value);
			else {
				if(Owner.OptionsSerialization.RestoreTextToControlDistance) TextToControlDistance = int.Parse((string)e.Info.Value);
			}
		}
		protected int textToControlDistanceCore = 5;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemTextToControlDistance"),
#endif
 RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		public virtual int TextToControlDistance {
			get { return GetTextToControlDistance(); }
			set {
				using(new SafeBaseLayoutItemChanger(this)) {
					if(value < 0) value = 0;
					textToControlDistanceCore = value;
					UpdateTextToControlDistance();
				}
			}
		}
		protected virtual void UpdateTextToControlDistance() {
			ShouldUpdateConstraintsDoUpdate = true;
			ShouldArrangeTextSize = true;
			ShouldResetBorderInfo = true;
			ComplexUpdate();
		}
		protected internal virtual void UpdateAfterRestore() { }
		protected LayoutStyleManager GetLayoutStyleManager() {
			if(Owner == null) return null;
			return Owner.LayoutStyleManager;
		}
		protected virtual bool ShouldSerializeSpaces() {
			if(Spacing == CalcDefaultSpacing()) return false;
			return true;
		}
		Utils.Padding LayoutStyleManagerClient.GetDefaultSpaces() { return DefaultSpaces; }
		Utils.Padding LayoutStyleManagerClient.GetDefaultPaddings() { return DefaultPaddings; }
		void LayoutStyleManagerClient.Update() {
			OnPaddingsSpacesChanging(this, null);
			OnPaddingsSpacesChanged(this, null);
		}
		protected virtual Utils.Padding DefaultSpaces {
			get { return Owner != null ? Owner.DefaultValues.LayoutItemSpacing : new Utils.Padding(0); }
		}
		protected virtual Utils.Padding DefaultPaddings {
			get {
				if(Owner != null) {
					return Owner.DefaultValues.LayoutItemPadding;
				}
				return new Utils.Padding(5);
			}
		}
		protected virtual Utils.Padding CalcDefaultPadding() {
			LayoutStyleManager psh = GetLayoutStyleManager();
			return CalcScaledPadding(psh != null ? psh.GetItemPadding(this) : DefaultPaddings);
		}
		protected virtual Utils.Padding CalcDefaultSpacing() {
			LayoutStyleManager psh = GetLayoutStyleManager();
			return CalcScaledPadding(psh != null ? psh.GetItemSpacing(this) : DefaultSpaces);
		}
		protected int CalcScaledWidth(int width) {
			if(Owner == null) return width;
			SizeF factor = Owner.AutoScaleFactor;
			if(factor.Width == 1.0f) return width;
			return Skins.RectangleHelper.ScaleHorizontal(width, factor.Width);
		}
		protected int CalcDeScaledWidth(int width) {
			if(Owner == null) return width;
			SizeF factor = Owner.AutoScaleFactor;
			if(factor.Width == 1.0f) return width;
			return Skins.RectangleHelper.DeScaleHorizontal(width, factor.Width);
		}
		protected Size CalcScaledSize(Size suggestedSize) {
			if(Owner == null) return suggestedSize;
			SizeF factor = Owner.AutoScaleFactor;
			if(factor.Width == 1.0f && factor.Height == 1.0f) return suggestedSize;
			return Skins.RectangleHelper.ScaleSize(suggestedSize, factor);
		}
		protected Size CalcDeScaledSize(Size size) {
			if(Owner == null) return size;
			SizeF factor = Owner.AutoScaleFactor;
			if(factor.Width == 1.0f && factor.Height == 1.0f) return size;
			return Skins.RectangleHelper.DeScaleSize(size, factor);
		}
		protected Utils.Padding CalcScaledPadding(Utils.Padding suggestedPadding) {
			if(Owner == null) return suggestedPadding;
			else {
				SizeF factor = Owner.AutoScaleFactor;
				if(factor.Width == 1.0f && factor.Height == 1.0f) return suggestedPadding;
				return CalcScaledPaddingCore(suggestedPadding, factor);
			}
		}
		internal static Utils.Padding CalcScaledPaddingCore(Utils.Padding suggestedPadding, SizeF factor) {
			return new Utils.Padding(
				 Skins.RectangleHelper.ScaleHorizontal(suggestedPadding.Left, factor.Width),
					Skins.RectangleHelper.ScaleHorizontal(suggestedPadding.Right, factor.Width),
					Skins.RectangleHelper.ScaleVertical(suggestedPadding.Top, factor.Height),
					Skins.RectangleHelper.ScaleVertical(suggestedPadding.Bottom, factor.Height));
		}
		protected Utils.Padding CalcDeScaledPadding(Utils.Padding suggestedPadding) {
			if(Owner == null) return suggestedPadding;
			else {
				SizeF factor = Owner.AutoScaleFactor;
				if(factor.Width == 1.0f && factor.Height == 1.0f) return suggestedPadding;
				return CalcDeScaledPaddingCore(suggestedPadding, factor);
			}
		}
		internal static Utils.Padding CalcDeScaledPaddingCore(Utils.Padding suggestedPadding, SizeF factor) {
			return new Utils.Padding(
								 Skins.RectangleHelper.DeScaleHorizontal(suggestedPadding.Left, factor.Width),
									Skins.RectangleHelper.DeScaleHorizontal(suggestedPadding.Right, factor.Width),
									Skins.RectangleHelper.DeScaleVertical(suggestedPadding.Top, factor.Height),
									Skins.RectangleHelper.DeScaleVertical(suggestedPadding.Bottom, factor.Height));
		}
		internal bool ShouldSerializeSpacing() { return ShouldSerializeSpaces(); }
		internal bool IsDefaultSpacing {
			get {
				return userSpaces == DevExpress.XtraLayout.Utils.Padding.Invalid;
			}
		}
		protected virtual void XtraDeserializeSpacing(XtraEventArgs e) {
			this.Spacing = (Utils.Padding)(new Utils.PaddingConverter().ConvertFrom(e.Info.Value));
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemSpacing"),
#endif
 XtraSerializableProperty()]
		public virtual Utils.Padding Spacing {
			get {
				if(IsDefaultSpacing) return CalcDefaultSpacing();
				return CalcScaledPadding(userSpaces);
			}
			set {
				using(new SafeBaseLayoutItemChanger(this)) {
					value = CalcDeScaledPadding(value);
					if(value == Spacing) return;
					if(userSpaces != value) {
						OnPaddingsSpacesChanging(this, EventArgs.Empty);
						userSpaces = value;
						OnPaddingsSpacesChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		internal bool ShouldSerializePadding() { return ShouldSerializePaddings(); }
		protected virtual bool ShouldSerializePaddings() {
			if(Padding == CalcDefaultPadding()) return false;
			return true;
		}
		internal bool IsDefaultPadding {
			get {
				return userPaddings == DevExpress.XtraLayout.Utils.Padding.Invalid;
			}
		}
		protected virtual void XtraDeserializePadding(XtraEventArgs e) {
			this.Padding = (Utils.Padding)(new Utils.PaddingConverter().ConvertFrom(e.Info.Value));
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemPadding"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		public virtual Utils.Padding Padding {
			get {
				if(IsDefaultPadding) return CalcDefaultPadding();
				return CalcScaledPadding(userPaddings);
			}
			set {
				using(new SafeBaseLayoutItemChanger(this)) {
					value = CalcDeScaledPadding(value);
					if(value == userPaddings) return;
					OnPaddingsSpacesChanging(this, EventArgs.Empty);
					userPaddings = value;
					OnPaddingsSpacesChanged(this, EventArgs.Empty);
				}
			}
		}
		protected void ProcessChangesInResizer() {
			ShouldUpdateConstraintsDoUpdate = true;
		}
		protected void OnPaddingsSpacesChanging(object sender, EventArgs e) {
			StartChange();
		}
		protected virtual void OnPaddingsSpacesChanged(object sender, EventArgs e) {
			ShouldResetBorderInfo = true;
			ProcessChangesInResizer();
			ComplexUpdate(true, true);
			EndChange();
		}
		protected virtual bool ShouldSerializeMinSize() {
			if(MinSize == Size.Empty) return false;
			return true;
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemMinSize"),
#endif
 RefreshProperties(RefreshProperties.All)]
		public virtual Size MinSize {
			get {
				if(minSize.Height == 0 || minSize.Width == 0) {
					if(Parent != null) return Parent.DefaultMinItemSize;
					else return new Size(1, 1);
				} else
					return minSize;
			}
			set {
				using(new SafeBaseLayoutItemChanger(this)) {
					if(minSize != value) {
						minSize = value;
						if(minSize.Width <= 0) {
							minSize.Width = 1;
						}
						if(minSize.Height <= 0) {
							minSize.Height = 1;
						}
						CheckIfConstraintsValid();
						if(Parent != null) {
							ProcessChangesInResizer();
						}
						Invalidate();
					}
				}
			}
		}
		[Browsable(false)]
		protected internal void SetConstraintsCore(Size minSize, Size maxSize) {
			this.maxSize = maxSize;
			this.minSize = minSize;
		}
		protected virtual bool ShouldSerializeMaxSize() {
			if(MaxSize == Size.Empty) return false;
			return true;
		}
		void CheckIfConstraintsValid() {
			if(!IsUpdateLocked && !IsDisposing) {
				if(maxSize.Width < MinSize.Width && maxSize.Width != 0) {
					maxSize.Width = MinSize.Width;
				}
				if(maxSize.Height < MinSize.Height && maxSize.Height != 0) {
					maxSize.Height = MinSize.Height;
				}
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemMaxSize"),
#endif
 RefreshProperties(RefreshProperties.All)]
		public virtual Size MaxSize {
			get {
				return maxSize;
			}
			set {
				if(maxSize == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					if(maxSize != value) {
						maxSize = value;
						if(maxSize.Width < 0) {
							maxSize.Width = 0;
						}
						if(maxSize.Height < 0) {
							maxSize.Height = 0;
						}
						CheckIfConstraintsValid();
						ProcessChangesInResizer();
						Invalidate();
					}
				}
			}
		}
		[Browsable(false)]
		public Rectangle Bounds { get { return new Rectangle(Location, Size); } }
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int X {
			get { return Location.X; }
			set { Location = new Point(value, Y); }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Y {
			get { return Location.Y; }
			set { Location = new Point(X, value); }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemLocation"),
#endif
 XtraSerializableProperty()]
		public virtual Point Location {
			get { return locationCore; }
			set {
				if(Location.Equals(value)) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					CheckLocation(value);
					if(Parent != null && !Parent.IsUpdateLocked)
						Parent.ChangeItemPosition(this, value);
					else
						locationCore = value;
				}
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Height {
			get { return Size.Height; }
			set { Size = new Size(Width, value); }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Width {
			get { return Size.Width; }
			set { Size = new Size(value, Height); }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemSize"),
#endif
 XtraSerializableProperty()]
		public virtual Size Size {
			get { return size; }
			set {
				if(size == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					SetSize(value); ComplexUpdate();
				}
			}
		}
		protected virtual void SetSize(Size value) {
			if(size.Equals(value)) return;
			if(!IsUpdateLocked && Parent != null) {
				Parent.ChangeItemSize(this, value);
				if(Parent != null) Parent.ResetResizerProportions();
			} else {
				size = value;
				if(Parent != null) Parent.ResetResizer();
				if(size.Width <= 0 || size.Height <= 0) throw new LayoutControlInternalException("The size cannot be less than or equal to zero.");
			}
		}
		bool selectionChanged = false;
		protected internal void StartChangeSelection() {
			selectionChanged = false;
			if(Owner != null)
				Owner.SelectedChangedCount++;
		}
		protected internal void CancelChangeSelection() {
			if(Owner != null) {
				Owner.SelectedChangedFlag = false;
				Owner.SelectedChangedCount--;
			}
		}
		protected internal void EndChangeSelection() {
			if(Owner != null) {
				Owner.SelectedChangedFlag |= selectionChanged;
				if(Owner.SelectedChangedCount == 1 && Owner.SelectedChangedFlag)
					Owner.SelectionChanged(this);
				else
					Owner.SelectedChangedCount--;
			}
		}
		bool isSelectedCore = false;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Selected {
			get { if(Parent != null) return Parent.IsItemSelected(this); else { selectionChanged = true; return isSelectedCore; } }
			set {
				StartChangeSelection();
				if(Parent != null) {
					if(value) {
						selectionChanged = !Parent.SelectedItems.Contains(this);
						if(selectionChanged) Parent.AddItemToSelectedList(this);
					} else {
						selectionChanged = Parent.SelectedItems.Contains(this);
						if(selectionChanged) Parent.RemoveItemFromSelectedList(this);
					}
				} else {
					isSelectedCore = value;
					if(Owner != null && Owner.RootGroup == this) selectionChanged = true;
				}
				EndChangeSelection();
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string GetDefaultText() {
			if(itemAnnotationAttributes != null) {
				string caption = DevExpress.Data.Utils.AnnotationAttributes.GetColumnCaption(itemAnnotationAttributes);
				if(!string.IsNullOrEmpty(caption)) return caption;
			}
			if(Name != "")
				return Name;
			String itemText = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.DefaultItemText);
			return itemText;
		}
		protected internal virtual void RaiseTextChanged(EventArgs e) {
			if(Owner != null) Owner.SetIsModified(true);
			EventHandler handler = (EventHandler)this.Events[textChanged];
			if(handler != null) handler(this, e);
		}
		protected virtual void UpdateText() {
			ShouldArrangeTextSize = true;
			if(IsGroup) ShouldResetBorderInfo = true;
			ComplexUpdate();
			if(!IsUpdateLocked) {
				RaiseTextChanged(EventArgs.Empty);
				TextChangedNotifyParent();
			}
		}
		protected virtual void TextChangedNotifyParent() {
			if(Parent != null) Parent.ProcessChildTextChanged();
		}
		[XtraSerializableProperty(), DefaultValue(true)]
		public virtual bool ShowInCustomizationForm {
			get {
				return showInCustomizationFormCore;
			}
			set {
				if(showInCustomizationFormCore == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					showInCustomizationFormCore = value;
					Invalidate();
				}
			}
		}
		protected virtual void XtraDeserializeText(XtraEventArgs e) {
			if(Owner == null) Text = e.Info.Value as String;
			else {
				if(Owner.OptionsSerialization.RestoreLayoutItemText) Text = e.Info.Value as String;
			}
		}
		bool ShouldSerializeText() { return !(text == null || text == string.Empty || text == ""); }
		void ResetText() { Text = ""; }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemText"),
#endif
 XtraSerializableProperty(), Localizable(true), Bindable(true)]
		[Category("Text")]
		public virtual string Text {
			get {
				if(Owner != null && Owner.Control != null && Owner.Control.IsHandleCreated && DataBindings["Text"] != null) {
					return text;
				}
				if(text == String.Empty)
					return GetDefaultText();
				else
					return text;
			}
			set {
				using(new SafeBaseLayoutItemChanger(this)) {
					if(value == null) value = "";
					if(value != Text) {
						text = value;
						if(Owner != null) Owner.SetIsModified(true);
						UpdateText();
					}
				}
			}
		}
		internal DevExpress.Data.Utils.AnnotationAttributes itemAnnotationAttributes;
		protected virtual void XtraDeserializeCustomizationFormText(XtraEventArgs e) {
			if(Owner == null) CustomizationFormText = e.Info.Value as String;
			else {
				if(Owner.OptionsSerialization.RestoreLayoutItemCustomizationFormText) CustomizationFormText = e.Info.Value as String;
			}
		}
		bool ShouldSerializeCustomizationFormText() { return !(customizationFormText == null || customizationFormText == string.Empty || customizationFormText == ""); }
		void ResetCustomizationFormText() { CustomizationFormText = ""; }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemCustomizationFormText"),
#endif
 XtraSerializableProperty(), Localizable(true), Bindable(true)]
		[Category("Text"), SearchColumn()]
		public virtual string CustomizationFormText {
			get {
				if(customizationFormText == null || customizationFormText == string.Empty || customizationFormText == "")
					return Text;
				return customizationFormText;
			}
			set {
				if(customizationFormText == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					if(value == null) value = "";
					customizationFormText = value;
					ComplexUpdate();
				}
			}
		}
		public virtual bool CanMove(LayoutItemDragController controller) {
			if(controller.Item != null && !controller.Item.CanCustomize && controller.MoveType == MoveType.Inside) return false;
			if(controller.DragItem != null && !controller.DragItem.CanCustomize && !controller.DragItem.IsHidden) return false;
			LayoutGroup group = controller.Item as LayoutGroup;
			LayoutGroup dragGroup = controller.DragItem as LayoutGroup;
			if(controller.Item == controller.DragItem) return false;
			if(dragGroup != null && dragGroup.Owner != null && (dragGroup == dragGroup.Owner.RootGroup)) return false;
			if(controller.Item.Parent == null && controller.MoveType == MoveType.Outside && group != null && group.Items.Count != 0) return false;
			if(controller.Item == null || (controller.Item.Parent == null && controller.Item != controller.Item.Owner.RootGroup)) return false;
			if(controller.DragItem != null && !controller.DragItem.ActualItemVisibility) return false;
			if(!IsConsistsDragging(controller.Item)) return false;
			BaseLayoutItem item = controller.DragItem;
			BaseLayoutItem insertTo = controller.Item;
			LayoutGroup insertToGroup = insertTo as LayoutGroup;
			LayoutGroup insertGroup = item as LayoutGroup;
			TabbedGroup insertTabbedGroup = item as TabbedGroup;
			if(insertToGroup != null && insertToGroup.ParentTabbedGroup != null && insertGroup == null && insertTabbedGroup == null && insertToGroup.Count != 0) return false;
			if(Parent != null)
				return Parent.CanMoveItem(controller);
			else {
				if(IsHidden && Owner != null && Owner.RootGroup != null && Owner.RootGroup.CanMoveItem(controller))
					return true;
				else
					return false;
			}
		}
		public virtual bool Move(BaseLayoutItem baseItem, InsertType insertType) {
			LayoutType layoutType;
			InsertLocation insertLocation;
			InsertTypeToInsertLocationLayoutTypesConverter.Convert(insertType, out insertLocation, out layoutType);
			LayoutItemDragController controller = new LayoutItemDragController(this, baseItem, MoveType.Inside, insertLocation, layoutType);
			return Move(controller);
		}
		public virtual bool Move(LayoutItemDragController controller) {
			if(!CanMove(controller)) return false;
			if(Parent != null)
				return Parent.MoveItem(controller);
			else {
				return Owner.RootGroup.MoveItem(controller);
			}
		}
		internal BaseLayoutItem Split() {
			return Split(Parent.DefaultLayoutType);
		}
		internal BaseLayoutItem Split(LayoutType layoutType) {
			return Split(InsertLocation.After, layoutType);
		}
		internal BaseLayoutItem Split(InsertLocation insertLocation, LayoutType layoutType) {
			return Parent.Split(this, insertLocation, layoutType);
		}
		internal void ChangeLocation(int dif, LayoutType layoutType) {
			Point newLocation = locationCore;
			if(layoutType == LayoutType.Horizontal)
				newLocation.X += dif;
			else newLocation.Y += dif;
			SetLocation(newLocation);
		}
		protected void CheckLocation(Point newLocation) {
			if(Parent == null) return;
		}
		protected virtual void SetLocation(Point newLocation) {
			if(Location != newLocation)
				SetShouldUpdateViewInfo();
			CheckLocation(newLocation);
			SetLocationWithoutCorrection(newLocation);
		}
		protected internal virtual void SetLocationWithoutCorrection(Point newLocation) {
			this.locationCore = newLocation;
		}
		int sizingInterval { get { return Owner is LayoutControl ? ((LayoutControl)Owner).GetSizingInerval() : 2; } }
		protected internal virtual BaseLayoutItem GetSizingItem(LayoutType layoutType, BaseLayoutItemHitInfo hitInfo) {
			Rectangle itemBounds = ViewInfo.BoundsRelativeToControl;
			Rectangle sizingBounds = new Rectangle(itemBounds.Right - sizingInterval, itemBounds.Top, 2 * sizingInterval, itemBounds.Height);
			if(layoutType == LayoutType.Vertical) {
				sizingBounds = new Rectangle(itemBounds.Left, itemBounds.Bottom - sizingInterval, itemBounds.Width, 2 * sizingInterval);
			}
			BaseLayoutItem nextItem = GetItemAtSizingBounds(sizingBounds);
			if(nextItem != this) {
				if(nextItem is SplitterItem && nextItem.ViewInfo.BoundsRelativeToControl.Contains(hitInfo.HitPoint))
					return null;
			}
			return sizingBounds.Contains(hitInfo.HitPoint) ? this : null;
		}
		BaseLayoutItem GetItemAtSizingBounds(Rectangle sizingBounds) {
			if(Parent != null) {
				foreach(BaseLayoutItem item in Parent.Items) {
					if(item == this) continue;
					if(item.ViewInfo != null && item.ViewInfo.BoundsRelativeToControl.IntersectsWith(sizingBounds)) return item;
				}
			}
			return this;
		}
		protected internal virtual BaseLayoutItemHitInfo CalcHitInfo(Point hitPoint, bool calcForHandler) {
			BaseLayoutItemHitInfo hitInfo = new BaseLayoutItemHitInfo();
			hitInfo.SetHitPoint(hitPoint);
			if(ViewInfo.BoundsRelativeToControl.Contains(hitPoint)) {
				hitInfo.SetItem(this);
				if(ViewInfo.ClientAreaRelativeToControl.Contains(hitPoint)) {
					hitInfo.SetHitTestType(LayoutItemHitTest.ControlsArea);
					return hitInfo;
				}
				if(ViewInfo.TextAreaRelativeToControl.Contains(hitPoint) && TextVisible) {
					hitInfo.SetHitTestType(LayoutItemHitTest.TextArea);
					return hitInfo;
				}
				hitInfo.SetHitTestType(LayoutItemHitTest.Item);
				return hitInfo;
			} else {
				hitInfo.SetItem(null);
				hitInfo.SetHitTestType(LayoutItemHitTest.None);
			}
			return hitInfo;
		}
		protected internal virtual void ChangeSize(int dif, LayoutType layoutType) {
			Size newSize = new Size(Size.Width, Size.Height);
			if(layoutType == LayoutType.Horizontal)
				newSize.Width += dif;
			else newSize.Height += dif;
			if(newSize.Width < 0) newSize.Width = 1;
			if(newSize.Height < 0) newSize.Height = 1;
			bool result = false;
			int watchdog = 100;
			while(!result && watchdog > 0) {
				result = SetInternalSize(newSize);
				watchdog--;
			}
		}
		protected internal virtual void ChangeSize(LayoutSize dif) {
			ChangeSize(dif.Width, dif.LayoutType);
		}
		internal LayoutSizingType sizingTypeCore = LayoutSizingType.None;
		internal LayoutSizingType SizingType {
			get {
				if(sizingTypeCore != LayoutSizingType.None && Parent != null && Parent.Handler != null && Parent.Handler.State == LayoutHandlerState.Normal) {
					sizingTypeCore = LayoutSizingType.None;
					return LayoutSizingType.None;
				}
				return sizingTypeCore;
			}
		}
		void SetSizingType(Size size) {
			if(this.size.Width != size.Width) sizingTypeCore = LayoutSizingType.Horizontal;
			else
				if(this.size.Height != size.Height) sizingTypeCore = LayoutSizingType.Vertical;
		}
		protected virtual bool AllowSetSizingType { get { return false; } }
		internal protected virtual void SetSizeWithoutCorrection(Size size) {
			if(Size != size) {
				SetShouldUpdateViewInfo();
				if(AllowSetSizingType && Parent != null && Parent.Handler != null && Parent.Handler.State == LayoutHandlerState.Sizing && !Owner.IsSerializing) {
					SetSizingType(size);
			}
		}
			this.size = size;
		}
		protected virtual bool SetInternalSize(Size size) {
			SetSizeWithoutCorrection(size);
			return true;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public bool IsHidden {
			get {
				if(Owner != null) {
					return Owner.IsHidden(this);
				} else
					return false;
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemAllowHide"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), DefaultValue(true)]
		public virtual bool AllowHide {
			get { return allowHideItem; }
			set { allowHideItem = value; }
		}
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), DefaultValue(false)]
		[XtraSerializableProperty()]
		public virtual bool StartNewLine {
			get { return startNewLine; }
			set {
				startNewLine = value;
				if(Parent != null && Parent.LayoutMode == LayoutMode.Flow) {
					Parent.Invalidate();
				}
			}
		}
		protected internal virtual void SetBounds(Rectangle bounds) {
			SetLocation(bounds.Location);
			SetInternalSize(bounds.Size);
		}
		internal void SetBounds(LayoutRectangle lBounds) { SetBounds(lBounds.Rectangle); }
		internal LayoutRectangle GetLayoutBounds(LayoutType layoutType) { return new LayoutRectangle(Bounds, layoutType); }
		internal LayoutPoint GetLayoutLocation(LayoutType layoutType) { return new LayoutPoint(Location, layoutType); }
		[Browsable(false)]
		public bool IsGroup { get { return this is LayoutGroup; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ILayoutControl Owner {
			get { return ownerControl; }
			set {
				bool force = false;
				if(ownerControl == null) force = true;
				ownerControl = value;
				if(force && value != null && updateCount != 0) {
					UpdatedCount += updateCount;
					updateCount = 0;
				}
			}
		}
		[DefaultValue(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool Expanded {
			get { return true; }
			set { }
		}
		[XtraSerializableProperty()]
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemVisibility"),
#endif
 DefaultValue(LayoutVisibility.Always)]
		public LayoutVisibility Visibility {
			get { return layoutVisibilityCore; }
			set {
				if(layoutVisibilityCore == value) return;
				layoutVisibilityCore = value;
				if(!IsParentVisible()) return;
				UpdateVisibility();
				if(Parent != null) {
					Parent.BeginUpdate();
					Parent.ResizeManager.UpdateVisibility(this);
					Parent.ResizeManager.UpdateVisibility();
					Parent.EndUpdate();
				} else {
					if(Owner != null && Owner.RootGroup != null && Owner.RootGroup == this) {
						Owner.RootGroup.BeginUpdate();
						Owner.RootGroup.ResizeManager.UpdateVisibility(this);
						Owner.RootGroup.ResizeManager.UpdateVisibility();
						Owner.RootGroup.EndUpdate();
					}
				}
			}
		}
		protected virtual void UpdateVisibility() { }
		protected bool IsParentVisible() {
			BaseLayoutItem currentItem = this;
			while(currentItem.Parent != null) {
				LayoutGroup lg = currentItem as LayoutGroup;
				if(lg != null && lg.ParentTabbedGroup != null && lg.ParentTabbedGroup.Visibility == LayoutVisibility.Never) return false;
				if(currentItem.Parent.Visibility == LayoutVisibility.Never) return false;
				currentItem = currentItem.Parent;
			}
			return true;
		}
		protected internal bool RequiredItemVisibility {
			get {
				if(Owner == null) return true;
				if(Owner.DesignMode) return true;
				if(Parent == null) return true;
				if(!IsParentVisible()) return false;
				if(Visibility == LayoutVisibility.Always) return true;
				if(Visibility == LayoutVisibility.Never) return false;
				if(Owner.EnableCustomizationMode) {
					if(Visibility == LayoutVisibility.OnlyInCustomization) return true;
					if(Visibility == LayoutVisibility.OnlyInRuntime) return false;
				} else {
					if(Visibility == LayoutVisibility.OnlyInCustomization) return false;
					if(Visibility == LayoutVisibility.OnlyInRuntime) return true;
				}
				return false;
			}
		}
		bool actualVisibilityStatus = true;
		protected internal bool ActualItemVisibility {
			get { return actualVisibilityStatus; }
			set {
				bool prevVal = actualVisibilityStatus;
				actualVisibilityStatus = value;
				if(prevVal != actualVisibilityStatus) RaiseShowHide(actualVisibilityStatus);
			}
		}
		[Browsable(false), DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Visible {
			get { return visible & !IsHidden & ActualItemVisibility; }
		}
		protected internal void SetVisible(bool lVisible) {
			if(this.visible != lVisible) {
				using(new SafeBaseLayoutItemChanger(this)) {
					bool prevVisible = this.visible;
					this.visible = lVisible;
					if(prevVisible != lVisible) {
						RaiseShowHide(this.visible);
					}
				}
			}
		}
		protected internal virtual void RaiseShowHide(bool visible) {
			EventHandler handler = (EventHandler)this.Events[visible ? shownEvent : hiddenEvent];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseShowHiding(bool visible) {
			EventHandler handler = (EventHandler)this.Events[visible ? showingEvent : hidingEvent];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void FakeOwnerUpdate() { }
		public virtual void Update() {
			Invalidate();
			if(Owner != null) {
				Owner.Control.Invalidate(ViewInfo.BoundsRelativeToControl);
				Owner.Control.Update();
			}
		}
		public virtual void Invalidate() {
			if(IsUpdateLocked) return;
			if(Owner != null)
				Owner.Invalidate();
			else {
				if(Parent != null)
					Parent.Invalidate();
				else {
					FakeOwnerUpdate();
				}
			}
		}
		protected internal virtual void UpdateChildren(bool visible) {
			if(IsDisposing) return;
			ViewInfo.CalculateViewInfo();
			SetVisible(visible);
		}
		[Browsable(false)]
		protected internal virtual void SelectionChanged(IComponent component) {
			if(Owner != null) Owner.SelectionChanged(component);
		}
		internal bool ShouldSerializeTextLocation() { return TextLocation != Locations.Default; }
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("BaseLayoutItemTextLocation"),
#endif
 DefaultValue(Locations.Default)]
		[RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		public virtual Locations TextLocation {
			get { return textLocationCore; }
			set {
				using(new SafeBaseLayoutItemChanger(this)) {
					textLocationCore = value;
					UpdateLabelPlace();
					ProcessChangesInResizer();
					if(Owner != null) Owner.SetIsModified(true);
					if(IsUpdateLocked) return;
					ComplexUpdate(true, true);
				}
			}
		}
		protected virtual void UpdateLabelPlace() { }
		protected virtual bool IsConsistsDragging(BaseLayoutItem baseItem) {
			BaseLayoutItem item = baseItem;
			while(item.Parent != null) {
				item = item.Parent;
				LayoutGroup group = item as LayoutGroup;
				if(group != null && group.ParentTabbedGroup == this) return false;
				if(item == this) return false;
			}
			return true;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal virtual int ChangeItemSize(BaseLayoutItem item, int dif, LayoutType layoutType, Constraints constraint) {
			LayoutSize ls = new LayoutSize(this.Size, layoutType);
			Size maxSize = this.MaxSize;
			if(constraint.max == 0)
				constraint.max = new LayoutSize(maxSize, layoutType).Width;
			else {
				if(new LayoutSize(maxSize, layoutType).Width != 0 && constraint.max > new LayoutSize(maxSize, layoutType).Width)
					constraint.max = new LayoutSize(maxSize, layoutType).Width;
			}
			Size minSize = this.MinSize;
			if(constraint.min == 0)
				constraint.min = new LayoutSize(minSize, layoutType).Width;
			else {
				if(constraint.min < new LayoutSize(minSize, layoutType).Width)
					constraint.min = new LayoutSize(minSize, layoutType).Width;
			}
			if(constraint.max != 0 && constraint.min > constraint.max)
				constraint.max = constraint.min;
			if(constraint.max != 0 && ls.Width + dif > constraint.max)
				dif = constraint.max - ls.Width;
			if(constraint.min != 0 && ls.Width + dif < constraint.min)
				dif = constraint.min - ls.Width;
			ChangeSize(dif, layoutType);
			return dif;
		}
		#region ICoverageFixerElement
		int ICoverageFixerElement.GetPos(bool vertical, bool max) {
			if(vertical) {
				if(max) return Bounds.Bottom;
				else return Bounds.Top;
			} else {
				if(max) return Bounds.Right;
				else return Bounds.Left;
			}
		}
		void ICoverageFixerElement.SetPos(bool vertical, bool max, int value) {
			if(vertical) {
				if(max) Height += value - Bounds.Bottom;
				else {
					Height += Bounds.Top - value;
					Y = value;
				}
			} else {
				if(max) Width += value - Bounds.Right;
				else {
					Width += Bounds.Left - value;
					X = value;
				}
			}
		}
		void ICoverageFixerElement.Shift(bool vertical, int range) {
			if(vertical)
				Y += range;
			else
				X += range;
		}
		#endregion
		Type ISupportPropertyGridWrapper.WrapperType {
			get {
				return GetWrapperType();
			}
		}
		protected virtual Type GetWrapperType() {
			if(Owner != null && (Owner as ISupportImplementor).Implementor.IsCustomPropertyGridWrapperExist(this)) {
				return (Owner as ISupportImplementor).Implementor.CustomWrappers[this.GetType()];
			} else
				return GetDefaultWrapperType();
		}
		protected virtual Type GetDefaultWrapperType() {
			if(Parent != null) {
				switch(Parent.LayoutMode) {
					case LayoutMode.Flow:
						return typeof(BaseFlowLayoutItemWrapper);
					case LayoutMode.Table:
						return typeof(BaseTableLayoutItemWrapper);
				}
			}
			return typeof(BaseLayoutItemWrapper);
		}
		#region IBindableComponent Members
		BaseBindableProvider bindableProviderCore = null;
		protected BaseBindableProvider BindableProvider {
			get {
				if(bindableProviderCore == null)
					bindableProviderCore = CreateBindableProvider();
				return bindableProviderCore;
			}
		}
		protected virtual BaseBindableProvider CreateBindableProvider() {
			return new BaseBindableProvider(this);
		}
		[ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get { return BindableProvider.DataBindings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get { return BindableProvider.BindingContext; }
			set { BindableProvider.BindingContext = value; }
		}
		#endregion
	}
	public class SafeBaseLayoutItemChanger : IDisposable {
		BaseLayoutItem safeItem;
		bool wasOwner = false;
		public SafeBaseLayoutItemChanger(BaseLayoutItem item) {
			safeItem = item;
			if(safeItem != null) safeItem.StartChange();
			wasOwner = HasOwner;
		}
		protected bool HasOwner {
			get { return safeItem != null && safeItem.Owner != null; }
		}
		public void Dispose() {
#if DEBUG
			if(HasOwner != wasOwner) throw new Exception("internal error");
#endif
			if(safeItem != null) safeItem.EndChange();
		}
	}
	public class BaseLayoutItemToolTipOptions : BaseToolTipOptions {
		protected internal bool ShouldSerializeCore(IComponent owner) {
			return ShouldSerialize(owner);
		}
	}
	public enum ItemDragDropMode { Default, UseParentOptions, Allow, Disable }
	public class BaseLayoutItemCustomizationOptions : BaseOptions {
		BaseLayoutItem ownerCore;
		ItemDragDropMode allowDragCore;
		ItemDragDropMode allowDropCore;
		public BaseLayoutItemCustomizationOptions(BaseLayoutItem owner) {
			this.ownerCore = owner;
			this.allowDragCore = ItemDragDropMode.Default;
			this.allowDropCore = ItemDragDropMode.Default;
		}
		protected BaseLayoutItem Owner {
			get { return ownerCore; }
		}
		protected internal bool ShouldSerializeCore(IComponent owner) {
			return ShouldSerialize(owner);
		}
		[ Category("Customization")]
		[XtraSerializableProperty(), DefaultValue(ItemDragDropMode.Default)]
		public ItemDragDropMode AllowDrag {
			get { return allowDragCore; }
			set { allowDragCore = value; }
		}
		[ Category("Customization")]
		[XtraSerializableProperty(), DefaultValue(ItemDragDropMode.Default)]
		public ItemDragDropMode AllowDrop {
			get { return allowDropCore; }
			set { allowDropCore = value; }
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			BaseLayoutItemCustomizationOptions source = options as BaseLayoutItemCustomizationOptions;
			if(source != null) {
				this.allowDragCore = source.allowDragCore;
				this.allowDropCore = source.allowDropCore;
			}
		}
		public bool CanDrag() {
			if(CheckUseParent(AllowDrag) && (Owner.Parent != null)) {
				return Owner.Parent.OptionsCustomization.CanDrag();
			}
			return AllowDrag != ItemDragDropMode.Disable;
		}
		public bool CanDrop() {
			if(CheckUseParent(AllowDrop) && (Owner.Parent != null)) {
				return Owner.Parent.OptionsCustomization.CanDrop();
			}
			return AllowDrop != ItemDragDropMode.Disable;
		}
		bool CheckUseParent(ItemDragDropMode option) {
			return option == (ItemDragDropMode.UseParentOptions) || (option == ItemDragDropMode.Default);
		}
	}
	public class BaseToolTipOptions : BaseOptions {
		bool showToolTipCore = false;
		string toolTipTextCore;
		string toolTipTitleCore;
		ToolTipIconType toolTipIconTypeCore = ToolTipIconType.None;
		public BaseToolTipOptions() {
			this.showToolTipCore = false;
			this.toolTipTextCore = string.Empty;
			this.toolTipTitleCore = string.Empty;
			this.toolTipIconTypeCore = ToolTipIconType.None;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			BaseToolTipOptions source = options as BaseToolTipOptions;
			if(source != null) {
				this.showToolTipCore = source.showToolTipCore;
				this.toolTipTextCore = source.toolTipTextCore;
				this.toolTipTitleCore = source.toolTipTitleCore;
				this.toolTipIconTypeCore = source.toolTipIconTypeCore;
			}
		}
		[ Category("ToolTip")]
		[XtraSerializableProperty(), DefaultValue(ToolTipIconType.None)]
		public ToolTipIconType ToolTipIconType {
			get { return toolTipIconTypeCore; }
			set { toolTipIconTypeCore = value; }
		}
		[ Category("ToolTip")]
		[XtraSerializableProperty(), DefaultValue(false)]
		public bool ShowToolTip {
			get { return showToolTipCore; }
			set { showToolTipCore = value; }
		}
		[ Category("ToolTip")]
		[XtraSerializableProperty(), DefaultValue("")]
		public string ToolTipText {
			get { return toolTipTextCore; }
			set { toolTipTextCore = value; }
		}
		[ Category("ToolTip")]
		[XtraSerializableProperty(), DefaultValue("")]
		public string ToolTipTitle {
			get { return toolTipTitleCore; }
			set { toolTipTitleCore = value; }
		}
	}
	public class BaseLayoutItemOptionsToolTip :BaseOptions {
		string toolTipTextCore;
		string toolTipTitleCore;
		ToolTipIconType toolTipIconTypeCore;
		bool immediateToolTip;
		DefaultBoolean allowHtmlString;
		string iconToolTipTextCore;
		string iconToolTipTitleCore;
		ToolTipIconType iconToolTipIconTypeCore;
		bool enableIconToolTipCore;
		bool iconimmediateToolTip;
		DefaultBoolean iconallowHtmlString;
		public bool ShouldSerializeCore(IComponent owner) {
			return ShouldSerialize(owner);
		}
		public BaseLayoutItemOptionsToolTip() {
			this.toolTipTextCore = string.Empty;
			this.toolTipTitleCore = string.Empty;
			this.toolTipIconTypeCore = ToolTipIconType.None;
			this.immediateToolTip = false;
			this.allowHtmlString = DefaultBoolean.Default;
			this.iconToolTipTextCore = string.Empty;
			this.iconToolTipTitleCore = string.Empty;
			this.iconToolTipIconTypeCore = ToolTipIconType.None;
			this.enableIconToolTipCore = true;
			this.iconimmediateToolTip = false;
			this.iconallowHtmlString = DefaultBoolean.Default;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			BaseLayoutItemOptionsToolTip source = options as BaseLayoutItemOptionsToolTip;
			if(source != null) {
				this.toolTipTextCore = source.toolTipTextCore;
				this.toolTipTitleCore = source.toolTipTitleCore;
				this.toolTipIconTypeCore = source.toolTipIconTypeCore;
				this.immediateToolTip = source.immediateToolTip;
				this.allowHtmlString = source.allowHtmlString;
				this.iconToolTipTextCore = source.iconToolTipTextCore;
				this.iconToolTipTitleCore = source.iconToolTipTitleCore;
				this.iconToolTipIconTypeCore = source.iconToolTipIconTypeCore;
				this.enableIconToolTipCore = source.enableIconToolTipCore;
				this.iconimmediateToolTip = source.iconimmediateToolTip;
				this.iconallowHtmlString = source.iconallowHtmlString;
			}
		}
		#region ToolTip
		[XtraSerializableProperty(), DefaultValue("")]
		[Localizable(true)]
		public string ToolTip {
			get { return toolTipTextCore; }
			set { toolTipTextCore = value; }
		}
		[XtraSerializableProperty(), DefaultValue("")]
		[Localizable(true)]
		public string ToolTipTitle {
			get { return toolTipTitleCore; }
			set { toolTipTitleCore = value; }
		}
		[XtraSerializableProperty(), DefaultValue(ToolTipIconType.None)]
		[Localizable(true)]
		public ToolTipIconType ToolTipIconType {
			get { return toolTipIconTypeCore; }
			set { toolTipIconTypeCore = value; }
		}
		[XtraSerializableProperty(), DefaultValue(false)]
		public bool ImmediateToolTip {
			get { return immediateToolTip; }
			set { immediateToolTip = value; }
		}
		[XtraSerializableProperty(), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowHtmlString {
			get { return allowHtmlString; }
			set { allowHtmlString = value; }
		}
		#endregion
		#region IconToolTip
		[XtraSerializableProperty(), DefaultValue("")]
		public string IconToolTip {
			get { return iconToolTipTextCore; }
			set { iconToolTipTextCore = value; }
		}
		[XtraSerializableProperty(), DefaultValue("")]
		public string IconToolTipTitle {
			get { return iconToolTipTitleCore; }
			set { iconToolTipTitleCore = value; }
		}
		[XtraSerializableProperty(), DefaultValue(ToolTipIconType.None)]
		public ToolTipIconType IconToolTipIconType {
			get { return iconToolTipIconTypeCore; }
			set { iconToolTipIconTypeCore = value; }
		}
		[XtraSerializableProperty(), DefaultValue(true)]
		public bool EnableIconToolTip {
			get { return enableIconToolTipCore; }
			set { enableIconToolTipCore = value; }
		}
		[XtraSerializableProperty(), DefaultValue(false)]
		public bool IconImmediateToolTip {
			get { return iconimmediateToolTip; }
			set { iconimmediateToolTip = value; }
		}
		[XtraSerializableProperty(), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean IconAllowHtmlString {
			get { return iconallowHtmlString; }
			set { iconallowHtmlString = value; }
		}
		#endregion
		public bool CanShowToolTip() {
			return !String.IsNullOrEmpty(ToolTip) || !String.IsNullOrEmpty(ToolTipTitle);
		}
		public bool CanShowIconToolTip() {
			return EnableIconToolTip && (!String.IsNullOrEmpty(IconToolTip) || !String.IsNullOrEmpty(IconToolTipTitle));
		}
	}
	public abstract class LayoutWrapperBase : BasePropertyGridObjectWrapper {
		protected virtual void OnSetValue(object target, object value) {
			BaseLayoutItem bli = target as BaseLayoutItem;
			if(bli != null && bli.Owner != null) bli.Owner.SetIsModified(true);
		}
	}
	public class BaseLayoutItemWrapper : LayoutWrapperBase {
		protected BaseLayoutItem Item {
			get { return WrappedObject as BaseLayoutItem; }
		}
		[Category("Name"), DefaultValue(true)]
		public virtual string Name { get { return Item.Name; } }
		[Category("Text"), DefaultValue(true)]
		public virtual bool TextVisible { get { return Item.TextVisible; } set { Item.TextVisible = value; OnSetValue(Item, value); } }
		[Category("Text"), DefaultValue("")]
		public virtual string Text { get { return Item.Text; } set { Item.Text = value; OnSetValue(Item, value); } }
		[Category("Text"), DefaultValue(Locations.Default)]
		public virtual Locations TextLocation { get { return Item.TextLocation; } set { Item.TextLocation = value; OnSetValue(Item, value); } }
		[Category("Text")]
		public virtual Size TextSize { get { return Item.TextSize; } set { Item.TextSize = value; OnSetValue(Item, value); } }
		[Category("Text"), DefaultValue(5)]
		public virtual int TextToControlDistance { get { return Item.TextToControlDistance; } set { Item.TextToControlDistance = value; OnSetValue(Item, value); } }
		[Category("Text"), DefaultValue("")]
		public virtual string CustomizationFormText { get { return Item.CustomizationFormText; } set { Item.CustomizationFormText = value; OnSetValue(Item, value); } }
		[Category("Appearance")]
		public virtual AppearanceObject AppearanceItemCaption { get { return Item.AppearanceItemCaption; } }
		public virtual Point Location { get { return Item.Location; } set { Item.Location = value; OnSetValue(Item, value); } }
		public virtual Size Size { get { return Item.Size; } set { Item.Size = value; OnSetValue(Item, value); } }
		public virtual Size MaxSize { get { return Item.MaxSize; } set { Item.MaxSize = value; OnSetValue(Item, value); } }
		public virtual Size MinSize { get { return Item.MinSize; } set { Item.MinSize = value; OnSetValue(Item, value); } }
		public virtual Utils.Padding Padding { get { return Item.Padding; } set { Item.Padding = value; OnSetValue(Item, value); } }
		public virtual Utils.Padding Spacing { get { return Item.Spacing; } set { Item.Spacing = value; OnSetValue(Item, value); } }
		[DefaultValue(true)]
		public virtual bool AllowHide { get { return Item.AllowHide; } set { Item.AllowHide = value; OnSetValue(Item, value); } }
		[DefaultValue(LayoutVisibility.Always)]
		public virtual LayoutVisibility Visibility { get { return Item.Visibility; } set { Item.Visibility = value; OnSetValue(Item, value); } }
		[DefaultValue(true)]
		public virtual bool ShowInCustomizationForm { get { return Item.ShowInCustomizationForm; } set { Item.ShowInCustomizationForm = value; OnSetValue(Item, value); } }
		[Category("ToolTip")]
		public virtual BaseLayoutItemOptionsToolTip OptionsToolTip { get { return Item.OptionsToolTip; } }
		[Category("Customization")]
		public virtual BaseLayoutItemCustomizationOptions OptionsCustomization { get { return Item.OptionsCustomization; } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new BaseLayoutItemWrapper();
		}
	}
	public class BaseFlowLayoutItemWrapper : BaseLayoutItemWrapper {
		[Category("OptionsFlowLayoutItem"), DefaultValue(false)]
		public virtual bool StartNewLine { get { return Item.StartNewLine; } set { Item.StartNewLine = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new BaseFlowLayoutItemWrapper();
		}
	}
	public class BaseTableLayoutItemWrapper :BaseLayoutItemWrapper {
		[Category("OptionsTableLayoutItem"),DefaultValue(1)]
		public virtual int RowSpan { get { return Item.OptionsTableLayoutItem.RowSpan; } set { Item.OptionsTableLayoutItem.RowSpan = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int ColumnSpan { get { return Item.OptionsTableLayoutItem.ColumnSpan; } set { Item.OptionsTableLayoutItem.ColumnSpan = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int RowIndex { get { return Item.OptionsTableLayoutItem.RowIndex; } set { Item.OptionsTableLayoutItem.RowIndex = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int ColumnIndex { get { return Item.OptionsTableLayoutItem.ColumnIndex; } set { Item.OptionsTableLayoutItem.ColumnIndex = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new BaseTableLayoutItemWrapper();
		}
	}
	public class BaseBindableProvider : IDisposable {
		BaseLayoutItem componentCore;
		BindingContext bindingContextCore = null;
		ControlBindingsCollection dataBindingsCore = null;
		public BaseBindableProvider(BaseLayoutItem component)
			: base() {
			this.componentCore = component;
			OnCreate();
		}
		public void Dispose() {
			OnDispose();
		}
		protected void OnCreate() { }
		protected void OnDispose() {
			if(dataBindingsCore != null) {
				dataBindingsCore.Clear();
				dataBindingsCore = null;
			}
			bindingContextCore = null;
			componentCore = null;
		}
		protected IBindableComponent Component {
			get { return componentCore; }
		}
		public ControlBindingsCollection DataBindings {
			get {
				if(dataBindingsCore == null) dataBindingsCore = new ControlBindingsCollection(Component);
				return dataBindingsCore;
			}
		}
		public BindingContext BindingContext {
			get {
				if(bindingContextCore != null) {
					return bindingContextCore;
				}
				IBindableComponent parentComponent = GetParentBindableComponent();
				if(parentComponent != null) {
					return parentComponent.BindingContext;
				}
				return null;
			}
			set {
				if(bindingContextCore == value) return;
				this.bindingContextCore = value;
				OnBindingContextChanged();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnBindingContextChanged() {
			if(bindingContextCore != null) {
				this.UpdateBindings(this.BindingContext);
			}
		}
		public void UpdateBindings(BindingContext context) {
			if(dataBindingsCore == null) return;
			for(int i = 0; i < DataBindings.Count; i++) {
				BindingContext.UpdateBinding(context, this.DataBindings[i]);
			}
		}
		IBindableComponent GetParentBindableComponent() {
			if(componentCore != null && componentCore.Owner != null) return componentCore.Owner.Control;
			return null;
		}
	}
}
