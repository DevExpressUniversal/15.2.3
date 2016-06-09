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
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.Helpers.Docking;
using DevExpress.XtraBars.InternalItems;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars.Styles;
using DevExpress.Utils.Serializing.Helpers;
using System.Collections.ObjectModel;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.XtraBars {
	public class BarDockInfo : ICloneable {
		bool modified = false;
		int dockRow, dockCol, offset;
		BarDockStyle dockStyle;
		BarDockControl dockControl = null;
		public BarDockInfo(Bar bar) {
			this.dockRow = bar.DockRow;
			this.dockCol = bar.DockCol;
			this.offset = bar.Offset;
			this.dockStyle = bar.DockStyle;
		}
		public BarDockInfo(BarDockStyle dockStyle, int dockRow, int dockCol, int offset) {
			this.dockStyle = dockStyle;
			this.dockRow = dockRow;
			this.dockCol = dockCol;
			this.offset = offset;
		}
		public BarDockInfo(BarDockControl control, int dockRow, int dockCol, int offset) : this(control, control.DockStyle, dockRow, dockCol, offset) { }
		public BarDockInfo(BarDockControl control, BarDockStyle dockStyle, int dockRow, int dockCol, int offset) : this(dockStyle, dockRow, dockCol, offset) {
			this.dockControl = control;
		}
		public BarDockInfo() : this(BarDockStyle.Top, -1, -1, 0) { }
		public int DockRow { 
			get { return dockRow; } 
			set { 
				if(DockRow == value) return;
				dockRow = value; 
				OnChanged();
			} 
		}
		public BarDockControl DockControl {
			get { return dockControl; }
			set {
				if(dockControl == value) return;
				dockControl = value;
				OnChanged();
			}
		}
		public int DockCol { 
			get { return dockCol; } 
			set { 
				if(DockCol == value) return;
				dockCol = value; 
				OnChanged();
			} 
		}
		public int Offset { 
			get { return offset; } 
			set { 
				if(value < 0) value = 0;
				if(Offset == value) return;
				offset = value; 
				OnChanged();
			} 
		}
		public BarDockStyle DockStyle { 
			get { return dockStyle; } 
			set { 
				if(DockStyle == value) return;
				dockStyle = value; 
				OnChanged();
			} 
		}
		public virtual void Assign(BarDockInfo dockInfo) {
			this.modified = false;
			this.dockCol = dockInfo.DockCol;
			this.dockRow = dockInfo.DockRow;
			this.dockStyle = dockInfo.DockStyle;
			this.offset = dockInfo.offset;
		}
		protected virtual void OnChanged() {
			this.modified = true;
		}
		protected internal bool IsModified { get { return modified; } }
		protected internal void SetModified() {
			this.modified = true;
		}
		protected internal void ResetModified() {
			this.modified = false;
		}
		public virtual bool IsEquals(BarDockInfo dockInfo) {
			return dockInfo.DockControl == DockControl && dockInfo.DockCol == DockCol && dockInfo.DockRow == DockRow && dockInfo.DockStyle == DockStyle &&
				dockInfo.Offset == Offset;
		}
		public bool CanDock(BarCanDockStyle canDockStyle) {
			return CanDock(DockStyle, canDockStyle);
		}
		public static bool CanDock(BarDockStyle dockStyle, BarCanDockStyle canDockStyle) {
			BarCanDockStyle res = BarCanDockStyle.Floating;
			switch(dockStyle) {
				case BarDockStyle.Left : res = BarCanDockStyle.Left; break;
				case BarDockStyle.Right : res = BarCanDockStyle.Right; break;
				case BarDockStyle.Top : res = BarCanDockStyle.Top; break;
				case BarDockStyle.Bottom : res = BarCanDockStyle.Bottom; break;
				case BarDockStyle.Standalone: res = BarCanDockStyle.Standalone; break;
			}
			return ((canDockStyle & res) != 0);
		}
		public virtual object Clone() {
			BarDockInfo dockInfo = new BarDockInfo(DockControl, DockStyle, DockRow, DockCol, Offset);
			return dockInfo;
		}
		public void UpdateDockPosition(int dockRow, int dockCol) {
			this.dockRow = dockRow;
			this.dockCol = dockCol;
		}
	}
	public enum BarState { Collapsed, Expanded }
	[DXToolboxItem(false), DesignTimeVisible(false), 
	Designer("DevExpress.XtraBars.Design.BarDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner))]
	public class Bar : BaseBarComponent, BarLinksHolder, IDockableObject, ISupportWindowActivate, IAppearanceOwner, ISupportXtraAnimation, IXtraObjectWithBounds, IXtraSerializationIdProvider, IXtraCollectionDeserializationOptionsProvider, IXtraSupportDeserializeCollection, IVisualEffectsHolder {
		private static readonly object animatedBounds = new object();
		[Flags]
		enum BarDirtyState { None = 0, Links = 1, Visual = 2, All = 3 }
		internal bool postMainMenu = false, postStatusBar = false; 
		BarDirtyState dirty = BarDirtyState.None;
		BarDockInfo dockInfo, prevDockInfo;
		BarCanDockStyle canDockStyle;
		BarOptions optionsBar;
		Image image;
		AppearanceObject appearance;
		StateAppearances barAppearance;
		CustomBarControl barControl;
		string barName, text;
		BarItemLinkCollection itemLinks;
		BarItemLinkReadOnlyCollection visibleLinks;
		protected int lockUpdate;
		private bool isDragging;
		protected internal bool canCloseMenu;
		bool visible;
		Size floatSize;
		internal Point floatLocation, floatMousePosition;
		int barItemHorzIndent, barItemVertIndent;
		int lockAllowFireEvents;
		object tag;
		internal const BarOptionFlags defaultOptions = BarOptionFlags.AllowDelete | BarOptionFlags.AllowQuickCustomization | BarOptionFlags.RotateWhenVertical | BarOptionFlags.DrawDragBorder;
		BarManager manager;
		LinksInfo defaultLinks;
		public Bar(BarManager manager, string name) : this(manager, name, BarDockStyle.None, Rectangle.Empty, null) {
		}
		public Bar() : this(null) {
		}
		public Bar(BarManager manager) : this(manager, null, BarDockStyle.None, Rectangle.Empty, null) {
		}
		[Obsolete(BarsObsoleteText.SRObsoleteCtorWithBarOptionFlagsParam)]
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, BarOptionFlags AOptions, Rectangle floatRect, LinksInfo linksInfo)
			: this(manager, name, ADockStyle, AOptions, floatRect, linksInfo, new Point(-1, -1), 0) {
		}
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, Rectangle floatRect, LinksInfo linksInfo)
			: this(manager, name, ADockStyle, floatRect, linksInfo, new Point(-1, -1), 0) {
		}
		[Obsolete(BarsObsoleteText.SRObsoleteCtorWithBarOptionFlagsParam)]
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, BarOptionFlags AOptions, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset)
			: this(manager, name, ADockStyle, AOptions, floatRect, linksInfo, dockPos, offset, BarCanDockStyle.All) {
		}
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset)
			: this(manager, name, ADockStyle, floatRect, linksInfo, dockPos, offset, BarCanDockStyle.All) {
		}
		[Obsolete(BarsObsoleteText.SRObsoleteCtorWithBarOptionFlagsParam)]
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, BarOptionFlags AOptions, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset, BarCanDockStyle canDockStyle)
			: this(manager, name, ADockStyle, AOptions, floatRect, linksInfo, dockPos, offset, canDockStyle, Color.Empty) {
		}
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset, BarCanDockStyle canDockStyle)
			: this(manager, name, ADockStyle, floatRect, linksInfo, dockPos, offset, canDockStyle, Color.Empty) {
		}
		[Obsolete(BarsObsoleteText.SRObsoleteCtorWithBarOptionFlagsParam)]
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, BarOptionFlags AOptions, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset, BarCanDockStyle canDockStyle, Color backColor)
			: this(manager, name, ADockStyle, AOptions, floatRect, linksInfo, dockPos, offset, canDockStyle, backColor, null) {
		}
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset, BarCanDockStyle canDockStyle, Color backColor)
			: this(manager, name, ADockStyle, floatRect, linksInfo, dockPos, offset, canDockStyle, backColor, null) {
		}
		[Obsolete(BarsObsoleteText.SRObsoleteCtorWithBarOptionFlagsParam)]
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, BarOptionFlags AOptions, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset, BarCanDockStyle canDockStyle, Color backColor, Image backImage)
			: this(manager, name, ADockStyle, AOptions, floatRect, linksInfo, dockPos, offset, canDockStyle, backColor, backImage, -1, -1) {
		}
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset, BarCanDockStyle canDockStyle, Color backColor, Image backImage)
			: this(manager, name, ADockStyle, floatRect, linksInfo, dockPos, offset, canDockStyle, backColor, backImage, -1, -1) {
		}
		[Obsolete(BarsObsoleteText.SRObsoleteCtorWithBarOptionFlagsParam)]
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, BarOptionFlags AOptions, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset, BarCanDockStyle canDockStyle, Color backColor, int barItemHorzIndent, int barItemVertIndent)
			: this(manager, name, ADockStyle, AOptions, floatRect, linksInfo, dockPos, offset, canDockStyle, backColor, null, barItemHorzIndent, barItemVertIndent) {
		}
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset, BarCanDockStyle canDockStyle, Color backColor, int barItemHorzIndent, int barItemVertIndent)
			: this(manager, name, ADockStyle, floatRect, linksInfo, dockPos, offset, canDockStyle, backColor, null, barItemHorzIndent, barItemVertIndent) {
		}
		[Obsolete(BarsObsoleteText.SRObsoleteCtorWithBarOptionFlagsParam)]
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, BarOptionFlags AOptions, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset, BarCanDockStyle canDockStyle, Color backColor, Image backImage, int barItemHorzIndent, int barItemVertIndent)
			: this(manager, name, ADockStyle, floatRect, linksInfo, dockPos, offset, canDockStyle, backColor, backImage, barItemHorzIndent, barItemVertIndent) {
		}
		public Bar(BarManager manager, string name, BarDockStyle ADockStyle, Rectangle floatRect, LinksInfo linksInfo, Point dockPos, int offset, BarCanDockStyle canDockStyle, Color backColor, Image backImage, int barItemHorzIndent, int barItemVertIndent) { 
			this.dockInfo = new BarDockInfo(ADockStyle, dockPos.X, dockPos.Y, offset);
			this.prevDockInfo = this.dockInfo.Clone() as BarDockInfo;
			this.optionsBar = new BarOptions();
			this.optionsBar.Changed += new BaseOptionChangedEventHandler(OnOptionsBarChanged);
			this.visible = true;
			this.barItemHorzIndent = barItemHorzIndent;
			this.barItemVertIndent = barItemVertIndent;
			this.image = backImage;
			this.canCloseMenu = true;
			this.isDragging = false;
			this.lockAllowFireEvents = 0;
			this.lockUpdate = 0;
			this.canDockStyle = canDockStyle;
			this.manager = manager;
			this.barControl = null;
			this.appearance = new AppearanceObject(this, true);
			this.appearance.Changed += new EventHandler(OnBarAppearanceChanged);
			this.barAppearance = new StateAppearances(this);
			this.barAppearance.Changed +=new EventHandler(OnBarAppearanceChanged);
			this.itemLinks = CreateLinkCollection();
			this.itemLinks.CollectionChanged += new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
			this.visibleLinks = new BarItemLinkReadOnlyCollection();
			this.floatMousePosition = this.floatLocation = floatRect.Location;
			this.floatSize = floatRect.Size;
			if(linksInfo != null) LinksPersistInfo = linksInfo;
			this.defaultLinks = null;
			if(Manager != null && (name == null || name == "")) {
				this.barName = Manager.GetNewBarName();
			}
			else 
				this.barName = name;
			if(barName == null) this.barName = "";
			this.text = this.barName;
			if(Manager != null) {
				Manager.AddBar(this);
				if(Manager.IsManagerLockUpdate) BeginUpdate();
			}
			HideWhenMerging = DefaultBoolean.Default;
			if(!IsLoading) ApplyDocking(DockInfo);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public BarItemLinkReadOnlyCollection GetActualLinks() {
			if(BarControl == null)
				return null;
			return BarControl.ViewInfo.ReallyVisibleLinks;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ForceUpdateLayout() {
			BarControl.UpdateViewInfo();
		}
		bool BarLinksHolder.Enabled { 
			get {
				if(BarControl != null && !BarControl.Enabled)
					return false;
				return true; 
			} 
		}
		protected virtual BarItemLinkCollection CreateLinkCollection() {
			return new BarItemLinkCollection(this);
		}
		[EditorBrowsable( EditorBrowsableState.Never)]
		public CustomBarControl GetBarControl() { return BarControl; }
		bool IAppearanceOwner.IsLoading { get { return IsLoading; } }
		void OnBarAppearanceChanged(object sender, EventArgs e) {
			OnBarChanged();
		}
		bool IDockableObject.IsVisible { 
			get { return GetBarVisible(); } 
			set { Visible = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Size Size { get { return BarControl == null ? Size.Empty : BarControl.Size; } }
		[Browsable(false)]
		public bool IsMerged { get { return ItemLinks.IsMergedState; } }
		[Browsable(false)]
		public BarItemLinkReadOnlyCollection MergeSource { get { return ItemLinks.MergeSource; } }
		bool IDockableObject.UseWholeRow { get { return OptionsBar.UseWholeRow; } }
		void IDockableObject.SetVisible(bool visible) { if(BarControl != null) BarControl.Visible = visible; }
		Rectangle IDockableObject.Bounds {
			get { return BarControl == null ? Rectangle.Empty : BarControl.Bounds; }
			set {
				if(BarControl != null) {
					if(AnimatedBounds)
						return;
					if(OptionsBar.BarState == BarState.Collapsed)
						BarControl.Bounds = new Rectangle(value.Location, BarControl.Size);
					else
						BarControl.Bounds = value;
				}
			}
		}
		Size IDockableObject.CalcMinSize() { return BarControl == null ? Size.Empty : BarControl.CalcMinSize(); }
		Size IDockableObject.CalcSize(int width) {
			if(BarControl == null) return Size.Empty;
			if(OptionsBar.BarState == BarState.Collapsed || AnimatedBounds) return BarControl.Size;
			return BarControl.CalcSize(width); 
		}
		void ResetOptionsBar() { OptionsBar.Reset(); }
		bool ShouldSerializeOptionsBar() { return OptionsBar.ShouldSerializeCore(); }
		[XtraSerializableProperty(XtraSerializationVisibility.Content), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOptionsBar"),
#endif
 Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarOptions OptionsBar { get { return optionsBar; } }
		[Obsolete(BarsObsoleteText.SRObsoleteOptions), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public BarOptionFlags Options { 
			get { return BarOptionFlags.None; }
			set { 
				if(IsLoading) {
					OptionsBar.ConvertFromFlags(value);
					if((value & BarOptionFlags.IsMainMenu) != 0) {
						if(Manager != null) Manager.MainMenu = this;
						this.postMainMenu = true;
					}
					if((value & BarOptionFlags.IsStatusBar) != 0) {
						if(Manager != null) Manager.StatusBar = this;
						this.postStatusBar = true;
					}
					this.Visible = (value & BarOptionFlags.Visible) != 0;
				}
			}
		}
		bool ShouldSerializeTag() { return Tag != null; }
		void ResetTag() { Tag = null; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonMiniToolbarTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		protected internal BarAutoPopupMode AutoPopupMode {
			get {
				if(OptionsBar.AutoPopupMode != BarAutoPopupMode.Default)
					return OptionsBar.AutoPopupMode;
				return IsMainMenu ? BarAutoPopupMode.All : BarAutoPopupMode.OnlyMenu;
			}
		}
		[Obsolete(BarsObsoleteText.SRObsoleteAppearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Color BackColor {
			get { return Appearance.BackColor; }
			set { Appearance.BackColor = value; }
		}
		[Obsolete(BarsObsoleteText.SRObsoleteAppearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Image BackgroundImage {
			get { return Appearance.Image; }
			set { Appearance.Image = value; }
		}
		protected virtual void OnOptionsBarChanged(object sender, BaseOptionChangedEventArgs e) {
			if(IsLoading) return;
			if(e.Name == "UseWholeRow") {
				if(OptionsBar.UseWholeRow && BarControl != null) {
					Offset = 0;
					((IDockableObject)this).VisibleChanged();
				}
			}
			if(e.Name == "BarState") {
				ProcessBarStateChanged();
			}
			LayoutChanged();
			UpdateCustomizationWindow();
		}
		protected virtual void UpdateCustomizationWindow() {
			if(IsLoading) return;
			Manager.Helper.CustomizationManager.UpdateBar(this);
		}
		protected virtual bool IsAllowFireEvents { get { return Manager != null && !Manager.IsLoading && lockAllowFireEvents == 0; } }
		bool IsLoadedAsInvisibleBar;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SetCurrentStateAsDefault() {
			lockUpdate ++;
			lockAllowFireEvents ++;
			try {
				defaultLinks = LinksPersistInfo;
				LinksPersistInfo = new LinksInfo();
				try {
					Manager.CreateLinks(this, defaultLinks);
				}
				catch {
				}
				Manager.SynchronizeLinksInfo(LinksPersistInfo, defaultLinks);
				LinksInfo temp = defaultLinks;
				defaultLinks = LinksPersistInfo;
				IsLoadedAsInvisibleBar = !Visible;
				LinksPersistInfo = temp;
				MakeVisibleList();
			}
			finally {
				lockUpdate --;
				lockAllowFireEvents --;
			}
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean HideWhenMerging { get; set; }
		public void Merge(Bar bar) {
			if(bar == null || bar.Manager == Manager) throw new ArgumentException("Bar should be not null and from other BarManager", "bar");
			MergedChild = bar;
			if(Manager.AllowMergeInvisibleLinks)
				ItemLinks.Merge(bar.ItemLinks);
			else
				ItemLinks.Merge(bar.VisibleLinks);
			UpdateBarVisibilityInMerge(true);
		}
		public void UnMerge() {
			UpdateBarVisibilityInMerge(false);
			ItemLinks.UnMerge();
			MergedChild = null;
			SetDirtyCore(BarDirtyState.All);
		}
		bool isMdiChildBar = false;
		protected bool IsMdiChildBar {
			get { return isMdiChildBar; }
			set {
				if(IsMdiChildBar == value)
					return;
				isMdiChildBar = value;
				OnIsMdiChildBarChanged();
			}
		}
		protected internal virtual void OnIsMdiChildBarChanged() {
			if(IsLoadedAsInvisibleBar || !GetHideWhenMerging())
				return;
			bool actualVisibility = (DockStyle == BarDockStyle.Standalone) || !IsMdiChildBar;
			Form form = Manager.GetForm();
			if(!actualVisibility && form != null && !(form is Docking2010.FloatDocumentForm)) {
				BarMdiMenuMergeStyle mergeStyle;
				if(!TryGetParentMdiMenuMergeStyle(form, out mergeStyle) || mergeStyle == BarMdiMenuMergeStyle.Never)
					return;
				if(mergeStyle == BarMdiMenuMergeStyle.OnlyWhenChildMaximized)
					actualVisibility = (form.WindowState != FormWindowState.Maximized);
				if(mergeStyle == BarMdiMenuMergeStyle.WhenChildActivated)
					actualVisibility = form.MdiParent != null && form.MdiParent.ActiveMdiChild != form;
			}
			Docking2010.Views.BaseDocument document = manager.GetDocument();
			if(!actualVisibility && document != null) {
				BarMdiMenuMergeStyle mergeStyle;
				if(!TryGetParentMdiMenuMergeStyle(document, out mergeStyle) || mergeStyle == BarMdiMenuMergeStyle.Never)
					return;
				Docking2010.DocumentContainer container = manager.GetDocumentContainer();
				if(container != null) {
					if(mergeStyle == BarMdiMenuMergeStyle.OnlyWhenChildMaximized)
						actualVisibility = !container.IsMaximized;
					if(mergeStyle == BarMdiMenuMergeStyle.WhenChildActivated)
						actualVisibility = container.Parent != null && ((Docking2010.DocumentsHost)container.Parent).ActiveContainer != container;
				}
			}
			Visible = actualVisibility;
		}
		bool TryGetParentMdiMenuMergeStyle(Docking2010.Views.BaseDocument document, out BarMdiMenuMergeStyle mergeStyle) {
			mergeStyle = BarMdiMenuMergeStyle.Always;
			var manager = (document != null) ? document.Manager : null;
			if(manager != null && manager.GetContainer() != null && manager.CanMergeOnDocumentActivate()) {
				BarManager parentManager = BarManager.FindManager(manager.GetContainer());
				if(parentManager != null) {
					mergeStyle = parentManager.MdiMenuMergeStyle;
					if(!IsMainMenu && !parentManager.HasMergeEventSubscription)
						mergeStyle = BarMdiMenuMergeStyle.Never;
					return true;
				}
			}
			return false;
		}
		bool TryGetParentMdiMenuMergeStyle(Form form, out BarMdiMenuMergeStyle mergeStyle) {
			mergeStyle = BarMdiMenuMergeStyle.Always;
			if(form != null && form.MdiParent != null) {
				BarManager parentManager = BarManager.FindManager(form.MdiParent);
				if(parentManager != null) {
					mergeStyle = parentManager.MdiMenuMergeStyle;
					if(!IsMainMenu && !parentManager.HasMergeEventSubscription)
						mergeStyle = BarMdiMenuMergeStyle.Never;
					return true;
				}
			}
			return false;
		}
		bool GetHideWhenMerging() {
			if(HideWhenMerging == DefaultBoolean.Default)
				return Manager == null || Manager.HideBarsWhenMerging;
			return HideWhenMerging != DefaultBoolean.False;
		}
		protected internal virtual void UpdateIsMdiChildBar() {
			if(Manager == null) return;
			Form form = Manager.GetForm();
			Manager.SuppressUpdateDockIndexes = true;
			try {
				IsMdiChildBar = ((form != null) && form.IsMdiChild) || manager.GetIsMdiChildManager();
			}
			finally {
				Manager.SuppressUpdateDockIndexes = false;
			}
		}
		protected internal virtual bool CheckIsMdiChildBar() {
			bool prevIsMdiChildBar = IsMdiChildBar;
			UpdateIsMdiChildBar();
			return prevIsMdiChildBar != isMdiChildBar;
		}
		protected Bar MergedChild { get; set; }
		void UpdateBarVisibilityInMerge(bool merged) {
			if(!GetHideWhenMerging())
				return;
			if(MergedChild != null && Manager.MdiMenuMergeStyle == BarMdiMenuMergeStyle.OnlyWhenChildMaximized)
				MergedChild.Visible = !merged;
		}
		public void AskReset() {
			if(Manager == null) return;
			DialogResult res = XtraMessageBox.Show(Manager.PaintStyle.CustomizationLookAndFeel, String.Format(Manager.GetString(BarString.ResetBar), Text),
				Manager.GetString(BarString.ResetBarCaption), MessageBoxButtons.OKCancel);
			if(res == DialogResult.Cancel) return;
			Reset();
		}
		public void Reset() {
			if(defaultLinks == null) return;
			BeginUpdate();
			try {
				ClearLinks();
				LinksPersistInfo = defaultLinks;
				SetCurrentStateAsDefault();
				for(int n = 0; n < ItemLinks.Count; n++) {
					BarItemLink link = ItemLinks[n];
					link.Item.Reset();
				}
				if(ItemLinks.MergeSource != null) ItemLinks.ReMerge();
			} finally {
				EndUpdate();
			}
		}
		[Browsable(false)]
		public bool IsMainMenu { get { return Manager != null && Manager.MainMenu == this; } }
		[Browsable(false)]
		public bool IsStatusBar { get { return Manager != null && Manager.StatusBar == this; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarBarItemVertIndent"),
#endif
 DefaultValue(-1), Category("Appearance")]
		public virtual int BarItemVertIndent {
			get { return barItemVertIndent; }
			set {
				if(value < 0) value = -1;
				if(BarItemVertIndent == value) return;
				barItemVertIndent = value;
				OnBarChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarBarItemHorzIndent"),
#endif
 DefaultValue(-1), Category("Appearance")]
		public virtual int BarItemHorzIndent {
			get { return barItemHorzIndent; }
			set {
				if(value < 0) value = -1;
				if(BarItemHorzIndent == value) return;
				barItemHorzIndent = value;
				OnBarChanged();
			}
		}
		bool ShouldSerializeFloatSize() { return !FloatSize.IsEmpty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarFloatSize"),
#endif
 XtraSerializableProperty(), Category("Layout")]
		public Size FloatSize {
			get { return floatSize; }
			set {
				if(value == FloatSize) return;
				floatSize = value;
				if(IsLoading) return;
				if(IsFloating)
					((FloatingBarControl)BarControl).Form.ClientSize = FloatSize;
				FireChanged();
			}
		}
		internal int GetFloatWidht() {
			if(!FloatSize.IsEmpty) return FloatSize.Width;
			return Screen.GetWorkingArea(Control.MousePosition).Width - 100;
		}
		internal bool ShouldSerializeFloatLocation() { return !FloatLocation.IsEmpty; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarFloatLocation"),
#endif
 XtraSerializableProperty(), Category("Layout")]
		public Point FloatLocation {
			get { return floatLocation; }
			set {
				if(FloatLocation == value) return;
				floatLocation = value;
				if(IsLoading) return;
				if(IsFloating)
					((FloatingBarControl)BarControl).Form.Location = FloatLocation;
				FireChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarBarName"),
#endif
  XtraSerializableProperty(), Category("Design")]
		public string BarName {
			get { return barName; }
			set {
				if(BarName == value) return;
				barName = value;
				OnBarChanged();
			}
		}
		protected internal virtual bool ShouldSerializeBarName() {
			return !String.IsNullOrEmpty(BarName);
		}
		protected internal virtual void ResetBarName() {
			BarName = String.Empty;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarText"),
#endif
  XtraSerializableProperty(), Category("Appearance"), Localizable(true)]
		public virtual string Text {
			get { return text; }
			set {
				if(Text == value) return;
				this.text = value;
				OnBarChanged();
				UpdateCustomizationWindow();
			}
		}
		protected internal virtual bool ShouldSerializeText() {
			return !String.IsNullOrEmpty(Text);
		}
		protected internal virtual void ResetText() {
			Text = String.Empty;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarVisible"),
#endif
 XtraSerializableProperty(), DefaultValue(true), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public virtual bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				IsLoadedAsInvisibleBar = false;
				bool prevValue = Visible;
				visible = value;
				if(IsLoading) return;
				if(Manager != null && !Manager.IsManagerLockUpdate) {
					((IDockableObject)this).VisibleChanged();
				} else {
					DockInfo.SetModified();
				}
				RaiseVisibleChanged();
				LayoutChanged();
				UpdateCustomizationWindow();
			}
		}
		public virtual int GetLinkHorzIndent() {
			if(Manager == null || BarItemHorzIndent != -1) return BarItemHorzIndent;
			if(Manager != null && this == Manager.MainMenu && !(Manager.PaintStyle is SkinBarManagerPaintStyle))
				return Manager.GetController().PropertiesBar.GetLinkHorzIndent() + 4;
			return Manager.GetController().PropertiesBar.GetLinkHorzIndent();
		}
		public virtual int GetLinkVertIndent() {
			if(Manager == null || BarItemVertIndent != -1) return BarItemVertIndent;
			return Manager.GetController().PropertiesBar.GetLinkVertIndent();
		}
		[Browsable(false)]
		public bool IsFloating { 
			get { return DockStyle == BarDockStyle.None && BarControl != null && BarControl is FloatingBarControl; } 
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarAppearance"),
#endif
 Category("Appearance"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual AppearanceObject Appearance {
			get { return BarAppearance.Normal; }
		}
		bool ShouldSerializeBarAppearance() { return BarAppearance.ShouldSerialize(); }
		void ResetBarAppearance() { BarAppearance.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarBarAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual StateAppearances BarAppearance {
			get { return barAppearance; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarCanDockStyle"),
#endif
 DefaultValue(BarCanDockStyle.All), Category("Behavior"),
		System.ComponentModel.Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public BarCanDockStyle CanDockStyle {
			get { return canDockStyle; }
			set {
				if(IsStatusBar) value = BarCanDockStyle.Bottom;
				if(value == 0) value = BarCanDockStyle.Top;
				if(value == CanDockStyle) return;
				canDockStyle = value;
				FireChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarDockStyle"),
#endif
 DefaultValue(BarDockStyle.None), XtraSerializableProperty(), Category("Behavior")]
		public BarDockStyle DockStyle {
			get { return DockInfo.DockStyle; }
			set {
				if(IsStatusBar) value = BarDockStyle.Bottom;
				if(value == DockStyle) return;
				if(IsLoading) {
					if(DockControl != null) DockControl.RemoveDockable(this);
					BarControl = null;
					DockInfo.DockStyle = value;
					return;
				}
				DockInfo = new BarDockInfo(value, DockInfo.DockRow, DockInfo.DockCol, DockInfo.Offset);
				FireChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarStandaloneBarDockControlName"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), DefaultValue(null), XtraSerializableProperty(), Category("Behavior")]
		public string StandaloneBarDockControlName {
			get {
				if(StandaloneBarDockControl != null) return StandaloneBarDockControl.Name;
				return string.Empty;
			}
			set {
				if(StandaloneBarDockControlName == value || Manager == null) return;
				foreach(BarDockControl control in Manager.DockControls) {
					StandaloneBarDockControl scontrol = control as StandaloneBarDockControl;
					if(scontrol == null || scontrol.Name != value) continue;
					StandaloneBarDockControl = scontrol;
					break;
				}
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarStandaloneBarDockControl"),
#endif
 DefaultValue(null), Category("Behavior")]
		public StandaloneBarDockControl StandaloneBarDockControl {
			get {
				if(DockInfo.DockControl is StandaloneBarDockControl) return DockInfo.DockControl as StandaloneBarDockControl;
				return null;
			}
			set {
				if(value == DockInfo.DockControl) return;
				if(IsLoading) {
					if(DockControl != null) DockControl.RemoveDockable(this);
					BarControl = null;
					DockInfo.DockControl = value;
					return;
				}
				DockInfo = new BarDockInfo(value, BarDockStyle.Standalone, DockInfo.DockRow, DockInfo.DockCol, DockInfo.Offset);
				DockInfo.SetModified();
			}
		}
		[Browsable(false)]
		public virtual bool CanMove { get { return OptionsBar.DrawDragBorder; } }
		BarItemLink BarLinksHolder.AddItem(BarItem item, LinkPersistInfo info) {
			BarItemLink link = ItemLinks.Add(item, info);
			return link;
		}
		MenuDrawMode BarLinksHolder.MenuDrawMode { get { return MenuDrawMode.Default; } }
		protected bool IsDestroying {
			get {
				if(Manager == null) return false;
				return Manager.IsDestroying;
			}
		}
		[Browsable(false)]
		public virtual bool IsLoading {
			get { return Manager == null || Manager.IsLoading; }
		}
		public virtual void AddItems(BarItem[] items) { ItemLinks.AddRange(items); 	}
		public virtual BarItemLink AddItem(BarItem item) { return ItemLinks.Add(item); }
		public virtual BarItemLink InsertItem(BarItemLink beforeLink, BarItem item) { return ItemLinks.Insert(beforeLink, item); }
		public virtual void RemoveLink(BarItemLink itemLink) { ItemLinks.Remove(itemLink); }
		public virtual void ClearLinks() { ItemLinks.Clear(); }
		protected void UpdateAccessibleLinks() {
			if(BarControl != null && BarControl.HasAccessible && !IsLockUpdate) {
				BarControl.ResetChildrenAccessible();
				SetDirty();
				BarControl.CheckDirty();
			}
		}
		protected virtual void OnItemLinksCollectionChanged(object sender, CollectionChangeEventArgs e) {
			BarItemLink link = e.Element as BarItemLink;
			if(e.Action == CollectionChangeAction.Add) RaiseLinkAdded(link);
			if(e.Action == CollectionChangeAction.Remove) RaiseLinkDeleted(link);
			if(ItemLinks.IsLockUpdate != 0) return;
			UpdateAccessibleLinks();
			OnBarChanged();
		}
		public virtual void BeginUpdate() {
			lockUpdate++;
		}
		public virtual void EndUpdate() {
			if(lockUpdate == 0 || --lockUpdate == 0) {
				if(IsLoading) return;
				OnBarChanged();
				if(DockInfo.IsModified) {
					((IDockableObject)this).VisibleChanged();
					DockInfo.ResetModified();
				}
			}
		}
		public virtual void CancelUpdate() {
			--lockUpdate;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false)
#if DXWhidbey
		,InheritableCollection
#endif
		]
		public LinksInfo LinksPersistInfo { 
			get { return ItemLinks.LinksPersistInfo; } 
			set { ItemLinks.LinksPersistInfo = value; }
		}
		protected virtual void OnLinkAdded(BarItemLink link, int index) {
			LinkPersistInfo pi = new LinkPersistInfo(link);
			if(index == -1 || index >= LinksPersistInfo.Count)
				LinksPersistInfo.Add(pi);
			else 
				LinksPersistInfo.Insert(index, pi);
			RaiseLinkAdded(link);
			OnBarChanged();
		}
		protected virtual void OnLinkDeleted(BarItemLink link) {
			LinksPersistInfo.RemoveLink(link);
			RaiseLinkDeleted(link);
			OnBarChanged();
		}
		private static object dockChanged = new object();
		private static object linkAdded = new object();
		private static object linkDeleted = new object();
		private static object visibleChanged = new object();
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarVisibleChanged"),
#endif
 Category("Events")]
		public event EventHandler VisibleChanged {
			add { Events.AddHandler(visibleChanged, value); }
			remove { Events.RemoveHandler(visibleChanged, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarDockChanged"),
#endif
 Category("Events")]
		public event EventHandler DockChanged {
			add { Events.AddHandler(dockChanged, value); }
			remove { Events.RemoveHandler(dockChanged, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarLinkDeleted"),
#endif
 Category("Events")]
		public event LinkEventHandler LinkDeleted {
			add { Events.AddHandler(linkDeleted, value); }
			remove { Events.RemoveHandler(linkDeleted, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarLinkAdded"),
#endif
 Category("Events")]
		public event LinkEventHandler LinkAdded {
			add { Events.AddHandler(linkAdded, value); }
			remove { Events.RemoveHandler(linkAdded, value); }
		}
		protected internal virtual void RaiseDockChanged() {
			if(!IsAllowFireEvents) return;
			EventHandler handler = (EventHandler)Events[dockChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseLinkAdded(BarItemLink link) {
			if(!IsAllowFireEvents) return;
			LinkEventHandler handler = (LinkEventHandler)Events[linkAdded];
			if(handler != null) handler(this, new LinkEventArgs(link));
		}
		protected virtual void RaiseVisibleChanged() {
			if(!IsAllowFireEvents) return;
			EventHandler handler = (EventHandler)Events[visibleChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseLinkDeleted(BarItemLink link) {
			if(!IsAllowFireEvents) return;
			LinkEventHandler handler = (LinkEventHandler)Events[linkDeleted];
			if(handler != null) handler(this, new LinkEventArgs(link));
		}
		public override string ToString() {
			return Text;
		}
		protected internal virtual void OnRemove() {
			Manager = null;
			if(DockControl != null)
				DockControl.RemoveDockable(this);
			BarControl = null;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UpdateVisualEffects(UpdateAction.Dispose);
				this.lockAllowFireEvents ++; 
				if(DockControl != null)
					DockControl.RemoveDockable(this);
				BarControl = null;
				BeginUpdate();
				ClearLinks();
				BarManager oldManager = Manager;
				this.manager = null;
				if(oldManager != null) {
					oldManager.Bars.Remove(this);
				}
				if(Appearance != null) {
					Appearance.Changed -= new EventHandler(OnBarAppearanceChanged);
				}
				if(OptionsBar != null) {
					this.optionsBar.Changed -= new BaseOptionChangedEventHandler(OnOptionsBarChanged);
				}
				if(ItemLinks != null) {
					ItemLinks.CollectionChanged -= new CollectionChangeEventHandler(OnItemLinksCollectionChanged);
				}
			}
			base.Dispose(disposing);
		}
		internal BarDockControl DockControl {
			get { 
				if(DockStyle == BarDockStyle.Standalone) return DockInfo.DockControl;
				if(DockStyle == BarDockStyle.None || Manager == null) return null;
				return Manager.DockControls[DockStyle];
			}
		}
		[DefaultValue(-1), Browsable(false), XtraSerializableProperty()]
		public virtual int DockRow { 
			get { 
				if(Manager != null && Manager.IsStoring && DockControl != null && Visible) 
					return DockControl.GetDockableDockRow(this);
				return DockInfo.DockRow; } 
			set {
				DockInfo.DockRow = value;
				FireChanged();
			}
		}
		[DefaultValue(-1), Browsable(false), XtraSerializableProperty()]
		public virtual int DockCol {
			get { 
				if(Manager != null && Manager.IsStoring && DockControl != null && Visible) 
					return DockControl.GetDockableDockCol(this);
				return DockInfo.DockCol == -1 ? 0 : DockInfo.DockCol; 
			}
			set {
				DockInfo.DockCol = value;
				FireChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarOffset"),
#endif
 DefaultValue(0), Category("Appearance"), XtraSerializableProperty()]
		public virtual int Offset {
			get { return DockInfo.Offset; }
			set {
				if(value < 0) value = 0;
				if(OptionsBar.UseWholeRow) value = 0;
				DockInfo.Offset = value;
				if(DesignMode && !IsLoading && Manager != null && !Manager.Helper.DockingHelper.SuppressApplyDockRowCol)
					ApplyDockRowCol();
			}
		}
		public virtual void ApplyDockRowCol() {
			if(DockControl == null || !GetBarVisible()) return;
			ApplyDocking(DockInfo);
		}
		protected internal void SwitchDockStyle() {
			if(!OptionsBar.DrawDragBorder) return;
			if(DockStyle == BarDockStyle.None) {
				if(PrevDockInfo.DockStyle != DockStyle) {
					ApplyDocking(PrevDockInfo);
				}
				else 
					DockStyle = BarDockStyle.Top;
			}
			else
				DockStyle = BarDockStyle.None;
			this.prevDockInfo = DockInfo.Clone() as BarDockInfo;
		}
		protected internal CustomBarControl BarControl { 
			get { return barControl; } 
			set {
				if(BarControl == value) return;
				if(BarControl != null) {
					XtraAnimator.RemoveObject(BarControl);
					BarControl.Dispose();
				}
				barControl = value;
				if(BarControl != null)
					BarControl.Init();
				OnBarControlChanged();
			}
		}
		protected virtual void OnBarControlChanged(){
			foreach(BarItemLink link in ItemLinks)
				link.UpdateVisualEffects(UpdateAction.OwnerChanged);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ForceUpdateBar() {
			if(BarControl != null && ViewInfo != null) {
				ViewInfo.ClearReady();
				BarControl.Invalidate();
			}
		}
		protected internal BarControlViewInfo ViewInfo {
			get { return BarControl == null ? null : BarControl.ViewInfo; }
		}
		internal bool XtraShouldSerializeText() { return OptionsBar.AllowRename; }
		internal void XtraClearItemLinks(XtraItemEventArgs e) {
			ClearLinks();
		}
		internal object XtraCreateItemLinksItem(XtraItemEventArgs e) {
			return BarLinksHolderSerializer.CreateItemLink(Manager, e, this);
		}
		[Browsable(false), XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, true, 0, XtraSerializationFlags.None)]
		public BarItemLinkCollection ItemLinks { get { return itemLinks; } 
		}
		internal BarItemLinkReadOnlyCollection VisibleLinksCore { get { return visibleLinks; } }
		[Browsable(false)]
		public BarItemLinkReadOnlyCollection VisibleLinks { 
			get {
				if(IsLinksDirty) MakeVisibleList();
				return visibleLinks; 
			} 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarManager Manager { 
			get { return manager; }
			set {
				if(Manager != null) return;
				if(Manager == value) return;
				manager = value;
				UpdateBarNameAndText();
				if(Manager.IsManagerLockUpdate) BeginUpdate();
				Manager.AddBar(this);
			}
		}
		bool IDockableObject.CanDock(BarDockControl dockControl) {
			BarCanDockStyle res = BarCanDockStyle.Floating;
			if(dockControl != null) {
				switch(dockControl.DockStyle) {
					case BarDockStyle.Left : res = BarCanDockStyle.Left; break;
					case BarDockStyle.Right : res = BarCanDockStyle.Right; break;
					case BarDockStyle.Top : res = BarCanDockStyle.Top; break;
					case BarDockStyle.Bottom : res = BarCanDockStyle.Bottom; break;
					case BarDockStyle.Standalone: res = BarCanDockStyle.Standalone; break;
				}
			}
			return ((CanDockStyle & res) != 0);
		}
		protected virtual CustomBarControl CreateFloatingBar() {
			FloatingBarControl fb = new FloatingBarControl(Manager, this);
			BarControl = fb;
			fb.Init();
			fb.UpdateVisibleLinks();
			DevExpress.XtraBars.Forms.FloatingBarControlForm form = new DevExpress.XtraBars.Forms.FloatingBarControlForm(Manager, fb);
			fb.Form = form;
			return fb;
		}
		protected internal virtual bool GetBarVisible() {
			if(DesignMode) return true;
			if(!Visible) return false;
			if(IsMainMenu && Manager != null) return Manager.CanShowMainMenu;
			if(GetAllowHideEmptyBar() && VisibleLinksCore.Count == 0 && Manager != null && !Manager.IsCustomizing) return false;
			return true;
		}
		BarDockInfo IDockableObject.DockInfo {
			get { return dockInfo; }
			set {
				if(value == null) return;
				if(DockInfo.IsEquals(value)) return;
				ApplyDocking(value);
			}
		}
		protected internal BarDockInfo DockInfo {
			get { return ((IDockableObject)this).DockInfo; }
			set { ((IDockableObject)this).DockInfo = value; }
		}
		protected internal BarDockInfo PrevDockInfo {
			get { return prevDockInfo; }
		}
		void IDockableObject.ApplyDocking(BarDockInfo dockInfo) {
			ApplyDocking(dockInfo);
		}
		protected internal virtual void UpdateBarNameAndText() {
			if(BarName == null || BarName == "") {
				this.barName = Manager.GetNewBarName();
				if(Text == "")
					this.text = BarName;
			}
		}
		BarDockControl prevLockedDockControl = null;
		protected internal virtual void ApplyDocking(BarDockInfo dockInfo) {
			if(Manager == null) return;
			if(this.lockUpdate != 0) {
				if(this.prevLockedDockControl == null)
					this.prevLockedDockControl = DockControl;
				this.dockInfo = dockInfo.Clone() as BarDockInfo;
				this.dockInfo.SetModified();
				return;
			}
			dockInfo = dockInfo.Clone() as BarDockInfo;
			if(!dockInfo.CanDock(CanDockStyle)) return;
			Manager.SelectionInfo.EditingLink = null;
			if(!GetBarVisible()) {
				this.dockInfo = dockInfo;
				return;
			}
			BarDockControl lockedDockControl = null, oldDockControl = DockControl, newDockControl = Manager.DockControls[dockInfo.DockStyle];
			if(dockInfo.DockStyle == BarDockStyle.Standalone) newDockControl = dockInfo.DockControl;
			if(prevLockedDockControl != null) {
				oldDockControl = prevLockedDockControl;
				prevLockedDockControl = null;
			}
			if(DockInfo.DockStyle == dockInfo.DockStyle) {
				lockedDockControl = oldDockControl;
			}
			if(lockedDockControl != null) lockedDockControl.BeginUpdate();
			try {
				if(oldDockControl != null) {
					if(oldDockControl == newDockControl)
						oldDockControl.SuspendLayout();
					oldDockControl.RemoveDockable(this, Manager == null || !Manager.IsManagerLockUpdate);
				}
				this.dockInfo = dockInfo.Clone() as BarDockInfo;
				if(newDockControl == null) {
					BarControl = CreateFloatingBar();
				} else {
					if(newDockControl != lockedDockControl) BarControl = null;
					if(BarControl == null) BarControl = new DockedBarControl(Manager, this);
					newDockControl.AddDockable(this, dockInfo.DockRow, dockInfo.DockCol, Manager == null || !Manager.IsManagerLockUpdate);
					if(oldDockControl == newDockControl)
						oldDockControl.ResumeLayout(true);
				}
				BarControl.DoSetVisible(GetBarVisible());
			} finally {
				if(lockedDockControl != null) lockedDockControl.EndUpdate();
			}
			OnBarChanged();
		}
		protected internal virtual void SetFloatSize(Size newSize) {
			if(Manager.IsDesignMode) {
				if(BarControl != null && BarControl.VisibleLinks.Count == 1) {
					if(BarControl.VisibleLinks[0].Item is BarEmptyItem) return;
				}
			}
			floatSize = newSize;
		}
		void ISupportWindowActivate.Activate() {
			IDockableObject dockObject = this as IDockableObject;
			if(dockObject.IsVisible && dockObject.DockStyle == BarDockStyle.None) {
				dockObject.VisibleChanged();
			}
		}
		void ISupportWindowActivate.Deactivate() {
			IDockableObject dockObject = this as IDockableObject;
			if(dockObject.IsVisible && dockObject.DockStyle == BarDockStyle.None) 
				dockObject.BarControl = null;
		}
		BarDockControl IDockableObject.DockControl { get { return DockControl; } }
		CustomControl IDockableObject.BarControl { get { return BarControl; } set { BarControl = value as CustomBarControl; } }
		Size IDockableObject.FloatSize {
			get { return this.FloatSize; }
		}
		Size IDockableObject.DockedSize {
			get { return this.FloatSize; }
		}
		Point IDockableObject.FloatMousePosition {
			get { return floatMousePosition; }
			set {
				floatMousePosition = value;
			}
		}
		void IDockableObject.VisibleChanged() {
			BarDockInfo prevInfo = DockInfo.Clone() as BarDockInfo;
			try {
				if(!GetBarVisible()) {
					if(DockControl != null) 
						DockControl.RemoveDockable(this, Manager == null || (!Manager.IsManagerLockUpdate && !Manager.SuppressUpdateDockIndexes));
					BarControl = null;
				} else {
					ApplyDocking(DockInfo);
				}
				if(BarControl != null) BarControl.DoSetVisible(GetBarVisible());
			} finally {
				if(Manager != null && Manager.IsManagerLockUpdate) this.dockInfo = prevInfo;
			}
		}
		protected internal virtual void UpdateScheme() {
			if(BarControl != null) {
				CheckDirty();
				BarControl.UpdateScheme();
			}
		}
		protected virtual void SetDirty() {
			if(DesignMode) {
				FireChanged();
			}
			this.dirty = BarDirtyState.All;
			if(ViewInfo != null) ViewInfo.ClearReady();
		}
		internal void RemoveVisualDirty() { this.dirty &= ~BarDirtyState.Visual; }
		protected internal bool IsDirty { get { return dirty != BarDirtyState.None; } }
		protected internal bool IsVisualDirty { get { return (dirty & BarDirtyState.Visual) != 0; } }
		protected internal bool IsLinksDirty { get { return (dirty & BarDirtyState.Links) != 0; } }
		protected virtual void CheckDirty() {
			if(IsLinksDirty) MakeVisibleList();
		}
		protected internal void SetVisualDirty() { SetDirtyCore(BarDirtyState.Visual); }
		protected internal void SetLinksDirty() { SetDirtyCore(BarDirtyState.Links); }
		void SetDirtyCore(BarDirtyState state) { dirty |= state; }
		protected internal virtual void OnLinkChanged(BarItemLink link) {
			if(IsLockUpdate) return;
			if(IsLoading || IsDestroying) return;
			SetDirty();
			Invalidate();
			if(BarControl == null && GetAllowHideEmptyBar() && VisibleLinksCore.Count == 0) MakeVisibleList();
		}
		public virtual void Invalidate() {
			if(BarControl == null) return;
			BarControl.Invalidate();
		}
		protected internal virtual void OnBarChanged() {
			if(IsLockUpdate || IsDestroying) return;
			SetDirty();
			if(IsLoading) return;
			Invalidate();
		}
		protected internal bool IsLockUpdate { get { return this.lockUpdate != 0; } }
		protected virtual void FireChanged() {
			if(IsLoading || Manager.lockFireChanged != 0) return;
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(this, null, null, null);
		}
		protected internal virtual void LayoutChanged() {
			if(IsLockUpdate) return;
			if(BarControl != null) {
				BarControl.LayoutChanged();
			}
		}
		protected virtual bool GetAllowHideEmptyBar() { return !IsMainMenu && Manager != null && Manager.GetAllowHideEmptyBar(); } 
		protected internal virtual void MakeVisibleList() {
			bool isZeroLinks = VisibleLinksCore.Count == 0;
			this.dirty &= ~BarDirtyState.Links;
			VisibleLinksCore.ClearItems();
			for(int n = 0; n < ItemLinks.Count; n ++) {
				BarItemLink link = ItemLinks[n];
				if(link.CanVisible)
					VisibleLinksCore.AddItem(link);
			}
			if(GetAllowHideEmptyBar() && Visible && (VisibleLinksCore.Count == 0) != isZeroLinks) {
				RefreshVisiblity();
			}
			if(BarControl != null) {
				BarControl.UpdateVisibleLinks();
			}
		}
		void RefreshVisiblity() {
			BeginInvoke(true, new MethodInvoker(((IDockableObject)this).VisibleChanged));
		}
		void BeginInvoke(bool invokeIfHandleExists, Delegate method) {
			if(BarControl != null && BarControl.IsHandleCreated) {
				BarControl.BeginInvoke(method);
				return;
			}
			if(Manager == null) return;
			if(Manager.Form != null && Manager.Form.IsHandleCreated) {
				Manager.Form.BeginInvoke(method);
				return;
			}
			if(invokeIfHandleExists) return;
			method.DynamicInvoke();
		}
		bool IDockableObject.IsDragging {
			get { return isDragging; }
			set {
				isDragging = value;
				Cursor.Current = (isDragging ? Cursors.SizeAll : Cursors.Default);
			}
		}
		protected internal void StartMoving(Control mouseDown) {
			if(DockInfo.DockStyle != BarDockStyle.None) {
				this.prevDockInfo = DockInfo.Clone() as BarDockInfo;
				if(DockControl != null && DockControl.GetDockableOnRowCount(this) == 1) this.prevDockInfo.DockCol = -1;
			}
			Manager.SelectionInfo.DockManager = new DockingManager(Manager);
			Manager.SelectionInfo.DockManager.StartMoving(this, mouseDown);
		}
		protected internal virtual void CreateQuickCustomizationLinks(BarSubItem item, bool addCustomizeButton) {
			bool allowCustomization = OptionsBar.AllowQuickCustomization;
			item.Enabled = allowCustomization;
			item.ItemLinks.Clear();
			if(!allowCustomization) return;
			CreateQuickCustomizationLinksCore(item, addCustomizeButton);
		}
		protected internal virtual void CreateQuickCustomizationLinksCore(BarSubItem item, bool addCustomizeButton) {
			if(item == null) return;
			BarSubItem subItem = new BarSubItem(Manager, true);
			subItem.Caption = Text;
			foreach(BarItemLink link in ItemLinks) {
				if(link.Item.GetRunTimeVisibility())
					subItem.AddItem(CreateQMenuItem(this, link, link.Caption));
			}
			subItem.AddItem(CreateQMenuItem(this, BarString.ResetButton, Manager.GetString(BarString.ResetButton))).BeginGroup = true;
			item.AddItem(subItem);
			if(addCustomizeButton) {
				BarItem menuItem = CreateQMenuItem(null, BarString.CustomizeButton, Manager.GetString(BarString.CustomizeButton));
				menuItem.Enabled = Manager.AllowCustomization;
				item.AddItem(menuItem).BeginGroup = true;
			}
		}
		protected virtual BarItem CreateQMenuItem(Bar owner, object tag, string caption) {
			BarButtonItem item = null;
			if(tag is BarItemLink) {
				item = new BarQMenuCustomizationItem(owner, Manager, true);
			} else {
				item = new BarQMenuCustomizationItem(owner, Manager, false);
				item.Caption = caption;
			}
			item.Tag = tag;
			return item;
		}
		Control ISupportXtraAnimation.OwnerControl { get { return this.BarControl; } }
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		Rectangle IXtraObjectWithBounds.AnimatedBounds { 
			get { return BarControl.Bounds; } 
			set { 
				BarControl.Bounds = value;
				LayoutChanged();
			} 
		}
		void IXtraObjectWithBounds.OnEndBoundAnimation(BoundsAnimationInfo info) {
		}
		internal bool AnimatedBounds { get { return XtraAnimator.Current.Get(this, animatedBounds) != null; } }
		protected virtual Size CalcBarEndSize() {
			if(BarControl == null) return Size.Empty;
			if(OptionsBar.BarState == BarState.Collapsed) {
				int itemLinksSize = 0, lli = BarControl.VisibleLinks.Count - 1;
				while(lli > 0 && (BarControl.VisibleLinks[lli] is BarQBarCustomizationItemLink || BarControl.VisibleLinks[lli].LinkViewInfo == null)) lli--;
				if(lli > 0) {
					if(ViewInfo.IsVertical)
						itemLinksSize = Math.Abs(BarControl.VisibleLinks[lli].LinkViewInfo.Bounds.Bottom - BarControl.VisibleLinks[0].LinkViewInfo.Bounds.Y);
					else
						itemLinksSize = Math.Abs(BarControl.VisibleLinks[lli].LinkViewInfo.Bounds.Right - BarControl.VisibleLinks[0].LinkViewInfo.Bounds.X);
				}
				if(ViewInfo.IsVertical)
					return new Size(BarControl.Bounds.Width, BarControl.Height - itemLinksSize);
				else 
					return new Size(BarControl.Bounds.Width - itemLinksSize, BarControl.Height);
			}
			else {
				return BarControl.CalcSize(10000);
			}
		}
		protected virtual void ProcessBarStateChanged() {
			Rectangle beginRect = BarControl.Bounds;
			Rectangle endRect = new Rectangle(BarControl.Location, CalcBarEndSize());
			XtraAnimator.RemoveObject(this, OptionsBar);
			XtraAnimator.Current.AddBoundsAnimation(this, this, animatedBounds, OptionsBar.BarState == BarState.Expanded, beginRect, endRect, 300);
		}
		internal void OnCustomizationEnd() {
			OnBarChanged();
			if(GetAllowHideEmptyBar() && VisibleLinks.Count == 0) RefreshVisiblity();
		}
		internal void OnCustomizationStart() {
			OnBarChanged();
			if(GetAllowHideEmptyBar() && VisibleLinks.Count == 0) RefreshVisiblity();
		}
		void XtraIsNewItemItemLinks(XtraNewItemEventArgs e) {
			if(Manager == null)
				return;
			foreach(BarItemLinkId linkId in Manager.ItemLinkIdCollection) {
				if(linkId.Id == ((BarItemLink)e.Item.Value).Item.Name) {
					return;
				}
			}
			e.NewItem = true;
		}
		void XtraIsOldItemItemLinks(XtraOldItemEventArgs e) {
			if(Manager == null)
				return;
			foreach(BarItem item in Manager.Items) {
				if(item.Name == ((BarItemLink)e.Item.Value).Item.Name) {
					return;
				}
			}
			e.OldItem = true;
		}
		object IXtraSerializationIdProvider.GetSerializationId(XtraSerializableProperty property, object item) {
			BarItemLink link = item as BarItemLink;
			if(link == null)
				return null;
			return link.Item.Name;
		}
		bool IXtraCollectionDeserializationOptionsProvider.AddNewItems {
			get { return Manager == null ? true : Manager.OptionsLayout.AllowAddNewItems; }
		}
		bool IXtraCollectionDeserializationOptionsProvider.RemoveOldItems {
			get { return Manager == null ? false : Manager.OptionsLayout.AllowRemoveOldItems; }
		}
		void XtraSetIndexItemLinksItem(XtraSetItemIndexEventArgs e) {
			BarItemLink link = e.Item.Value as BarItemLink;
			if(link == null || ItemLinks.Contains(link))
				return;
			if(e.NewIndex == -1)
				ItemLinks.Add(link);
			else
				ItemLinks.Insert(e.NewIndex, link);
		}
		void XtraRemoveItemLinksItem(XtraSetItemIndexEventArgs e) {
			bool prev = BarItem.SkipDisposeLinkOnRemove;
			BarItem.SkipDisposeLinkOnRemove = false;
			ItemLinks.Remove((BarItemLink)e.Item.Value);
			BarItem.SkipDisposeLinkOnRemove = prev;
		}
		void IXtraSupportDeserializeCollection.AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			BarItem.SkipDisposeLinkOnRemove = false;
		}
		void IXtraSupportDeserializeCollection.BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) {
			BarItem.SkipDisposeLinkOnRemove = true;
		}
		bool IXtraSupportDeserializeCollection.ClearCollection(string propertyName, XtraItemEventArgs e) {
			ItemLinks.Clear();
			return true;
		}
		#region ISupportAdornerElement Members
		protected override Rectangle GetVisualEffectBounds() {
			return BarControl != null ? BarControl.Bounds : Rectangle.Empty;
		}
		protected override bool GetVisualEffectsVisible() { return Visible && !IsFloating; }
		protected override ISupportAdornerUIManager GetVisualEffectsOwner() { return DockControl; }
		bool IVisualEffectsHolder.VisualEffectsVisible {
			get { return GetVisualEffectsVisible(); }
		}
		ISupportAdornerUIManager IVisualEffectsHolder.VisualEffectsOwner { get { return BarControl; } }
		#endregion
	}
	[ListBindable(false)]
	public class Bars : CollectionBase {
		BarManager manager;
		public Bars(BarManager manager) {
			this.manager = manager;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarsManager")]
#endif
		public virtual BarManager Manager { get { return manager; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarsItem")]
#endif
		public DevExpress.XtraBars.Bar this[string barName] {
			get {
				for(int n = 0; n < Count; n++) {
					if(this[n].BarName == barName) return this[n];
				}
				return null;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarsItem")]
#endif
		public DevExpress.XtraBars.Bar this[int index] {
			get {
				return (Bar)List[index];
			}
		}
		public virtual void AddRange(Bar[] bars) {
			foreach(Bar bar in bars) {
				Add(bar);
			}
		}
		public virtual int Add(Bar bar) {
			if(List.Contains(bar)) return List.IndexOf(bar);
			int index = List.Add(bar);
			bar.Manager = Manager;
			return index;
		}
		public void Insert(int index, Bar bar) {
			if (List.Contains(bar))
				return;
			List.Insert(index, bar);
			bar.Manager = Manager;
		}
		internal void DisposeBars() {
			ArrayList list = new ArrayList(InnerList);
			InnerList.Clear();
			foreach(Bar bar in list) {
				bar.Dispose();
				if(Manager != null) Manager.OnRemoveBar(bar);
			}
		}
		internal void Remove(Bar bar) {
			if(!List.Contains(bar)) return;
			List.Remove(bar);
		}
		public virtual bool Contains(Bar bar) {
			return List.Contains(bar);
		}
		public virtual int IndexOf(Bar bar) {
			return List.IndexOf(bar);
		}
		protected override void OnRemoveComplete(int position, object item) {
			Bar bar = item as Bar;
			Manager.OnRemoveBar(bar);
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) {
				RemoveAt(n);
			}
			InnerList.Clear();
		}
	}
	interface IVisualEffectsHolder {
		bool VisualEffectsVisible { get; }
		ISupportAdornerUIManager VisualEffectsOwner { get; }
	}
	public interface BarLinksHolder {
		BarManager Manager { get; }
		BarItemLink InsertItem(BarItemLink beforeLink, BarItem item);
		BarItemLink AddItem(BarItem item);
		BarItemLink AddItem(BarItem item, LinkPersistInfo info);
		void RemoveLink(BarItemLink itemLink);
		BarItemLinkCollection ItemLinks { get; }
		void BeginUpdate();
		void EndUpdate();
		void ClearLinks();
		MenuDrawMode MenuDrawMode { get; }
		bool Enabled { get; }
	}
	public interface ICustomizationMenuFilterSupports {
		bool ShouldShowItem(Type type);
	}
	public class BarItemLinkId {
		[XtraSerializableProperty]
		public string Id { get; set; }
	}
	public class BarItemLinkIdCollection : Collection<BarItemLinkId> { }
	[ToolboxItem(false), DesignTimeVisible(false)]
	public abstract class BaseBarComponent : Component, ISupportAdornerElement {
		readonly static object updateVisualEffects = new object();
		#region ISupportAdornerElement Members
		Rectangle ISupportAdornerElement.Bounds { get { return GetVisualEffectBounds(); } }
		event UpdateActionEventHandler ISupportAdornerElement.Changed {
			add { Events.AddHandler(updateVisualEffects, value); }
			remove { Events.RemoveHandler(updateVisualEffects, value); }
		}
		bool ISupportAdornerElement.IsVisible { get { return GetVisualEffectsVisible(); } }
		ISupportAdornerUIManager ISupportAdornerElement.Owner { get { return GetVisualEffectsOwner(); } }
		protected abstract bool GetVisualEffectsVisible();
		protected abstract ISupportAdornerUIManager GetVisualEffectsOwner();
		protected abstract Rectangle GetVisualEffectBounds();
		protected void UpdateVisualEffects(UpdateAction action) {
			UpdateActionEventHandler handler = Events[updateVisualEffects] as UpdateActionEventHandler;
			if(handler == null) return;
			UpdateActionEvendArgs e = new UpdateActionEvendArgs(action);
			handler(this, e);
		}
		#endregion
	}
}
