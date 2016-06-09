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
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Base;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Buttons;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public interface IDocumentGroupInfo : IBaseElementInfo, IDockingAdornerInfo, IUIElement {
		IXtraTab Tab { get; }
		Rectangle Client { get; }
		DocumentGroup Group { get; }
		bool IsActive { get; }
		void ProcessEvent(EventType eventType, object args);
		int TabReorderingIndex { get; }
		void UpdateReordering(Adorner adorner, Point point, BaseDocument dragItem);
		void ResetReordering(Adorner adorner);
		bool CanReorder(Point point);
		void Reorder(Point point, BaseDocument dragItem);
		ToolTipControlInfo GetToolTipControlInfo(BaseViewHitInfo hitInfo);
		void ShowDocumentSelectorMenu(ITabbedViewController controller);
		void CheckDropDownButton();
		void UpdateCustomHeaderButtons(DocumentGroup group);
	}
	class DocumentSelectorButton : CustomHeaderButton {
		public DocumentSelectorButton()
			: base(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown) {
		}
		string toolTipCore;
		public override string ToolTip {
			get {
				if(string.IsNullOrEmpty(toolTipCore))
					toolTipCore = XtraEditors.Controls.Localizer.Active.GetLocalizedString(XtraEditors.Controls.StringId.TabHeaderSelectorButton);
				return toolTipCore;
			}
			set {
				if(toolTipCore == value) return;
				toolTipCore = value;
				OnChanged();
			}
		}
	}
	class DocumentGroupInfo : BaseElementInfo, IXtraTab, IXtraTabProperties, IXtraTabPropertiesEx, IDocumentGroupInfo {
		DocumentGroup groupCore;
		CustomHeaderButtonCollection tabButtons;
		DocumentSelectorButton documentSelectorButton;
		protected BaseViewInfoRegistrator Registrator;
		protected internal BaseTabControlViewInfo ViewInfo;
		protected BaseTabPainter Painter;
		protected BaseTabHandler Handler;
		Rectangle clientCore;
		public DocumentGroupInfo(TabbedView view, DocumentGroup group)
			: base(view) {
			tabButtons = new CustomHeaderButtonCollection();
			documentSelectorButton = new DocumentSelectorButton();
			groupCore = group;
			UpdateCustomHeaderButtonsCore(group);
			Init(view);
		}
		protected override void OnDispose() {
			LayoutHelper.Unregister(this);
			base.OnDispose();
			groupCore = null;
		}
		public override System.Type GetUIElementKey() {
			return typeof(IDocumentGroupInfo);
		}
		public DocumentGroup Group {
			get { return groupCore; }
		}
		public bool IsActive {
			get {
				if(IsDisposing || !Owner.IsFocused) return false;
				return Group.Items.Contains(Owner.ActiveDocument as Document);
			}
		}
		protected override void UpdateStyleCore() {
			Init(Owner as TabbedView);
		}
		protected override void ResetStyleCore() {
			Reset();
		}
		void Init(TabbedView view) {
			Registrator = GetRegistrator(Owner as TabbedView);
			ViewInfo = Registrator.CreateViewInfo(this);
			Handler = Registrator.CreateHandler(this);
			Painter = Registrator.CreatePainter(this);
			ViewInfo.SetDefaultTabMiddleClickFiringMode(TabMiddleClickFiringMode.MouseDown);
			ViewInfo.SelectedPageChanged += OnSelectedPageChanged;
			ViewInfo.CloseButtonClick += OnCloseButtonClick;
			ViewInfo.TabMiddleClick += OnTabMiddleClick;
			ViewInfo.CustomHeaderButtonClick += OnCustomHeaderButtonClick;
			ViewInfo.PageClientBoundsChanged += OnPageClientBoundsChanged;
		}
		void Reset() {
			ViewInfo.PageClientBoundsChanged -= OnPageClientBoundsChanged;
			ViewInfo.CloseButtonClick -= OnCloseButtonClick;
			ViewInfo.TabMiddleClick -= OnTabMiddleClick;
			ViewInfo.CustomHeaderButtonClick -= OnCustomHeaderButtonClick;
			ViewInfo.SelectedPageChanged -= OnSelectedPageChanged;
			Ref.Dispose(ref Registrator);
			Ref.Dispose(ref Handler);
			Ref.Dispose(ref ViewInfo);
			Painter = null;
		}
		protected virtual BaseViewInfoRegistrator GetRegistrator(TabbedView view) {
			if(view.IsSkinPaintStyle)
				return new SkinDocumentGroupViewInfoRegistrator();
			string paintStyleName = view.PaintStyleName;
			switch(paintStyleName) {
				case "Office2003":
					return new Office2003DocumentGroupViewInfoRegistrator();
				case "Standard":
				case "Office2000":
					return new StandardDocumentGroupViewInfoRegistrator();
				case "Flat":
				case "OfficeXP":
					return new FlatDocumentGroupViewInfoRegistrator();
				case "WindowsXP":
					return new WindowsXPDocumentGroupViewInfoRegistrator();
			}
			return PaintStyleCollection.DefaultPaintStyles.GetView(
				view.ElementsLookAndFeel, GetTabPaintStyleName(paintStyleName));
		}
		protected virtual string GetTabPaintStyleName(string paintStyleName) {
			switch(paintStyleName) {
				case "Default":
				case "Skin": return "Skin";
				case "Office2003": return "Office2003";
				case "Office2000": return "Standard";
				case "OfficeXP": return "Flat";
				case "WindowsXP": return "WindowsXP";
			}
			return BaseViewInfoRegistrator.DefaultViewName;
		}
		#region IXtraTab Members
		bool IXtraTab.RightToLeftLayout {
			get { return Owner.Manager.IsRightToLeftLayout(); }
		}
		BaseTabHitInfo IXtraTab.CreateHitInfo() {
			return new BaseTabHitInfo();
		}
		IXtraTabPage IXtraTab.GetTabPage(int index) {
			return Group.Items[index].GetTabPage();
		}
		TabHeaderLocation IXtraTab.HeaderLocation {
			get { return Group.Properties.ActualHeaderLocation; }
		}
		TabOrientation IXtraTab.HeaderOrientation {
			get { return Group.Properties.ActualHeaderOrientation; }
		}
		object IXtraTab.Images {
			get { return Owner.Manager.Images; }
		}
		void IXtraTab.Invalidate(Rectangle rect) {
			Owner.Manager.Invalidate();
		}
		void IXtraTab.LayoutChanged() {
			ViewInfo.LayoutChanged();
		}
		void IDocumentGroupInfo.CheckDropDownButton() {
			if(!Group.Properties.HasDocumentSelectorButton && tabButtons.Contains(documentSelectorButton))
				tabButtons.RemoveAt(tabButtons.IndexOf(documentSelectorButton));
			if(Group.Properties.HasDocumentSelectorButton && !tabButtons.Contains(documentSelectorButton))
				tabButtons.Add(documentSelectorButton);
		}
		DevExpress.LookAndFeel.UserLookAndFeel IXtraTab.LookAndFeel {
			get { return Owner.ElementsLookAndFeel; }
		}
		void IXtraTab.OnPageChanged(IXtraTabPage page) {
			ViewInfo.OnPageChanged(page);
			Owner.LayoutChanged();
		}
		System.Windows.Forms.Control IXtraTab.OwnerControl {
			get { return Owner.Manager.GetOwnerControl(); }
		}
		int IXtraTab.PageCount {
			get { return Group.Items.Count; }
		}
		Point IXtraTab.ScreenPointToControl(Point point) {
			Control c = Owner.Manager.GetOwnerControl();
			if(c != null)
				return c.PointToClient(point);
			throw new Exception("The method or operation is not implemented.");
		}
		BaseViewInfoRegistrator IXtraTab.View {
			get { return Registrator; }
		}
		BaseTabControlViewInfo IXtraTab.ViewInfo {
			get { return ViewInfo; }
		}
		#endregion
		#region IDocumentGroupInfo Members
		IXtraTab IDocumentGroupInfo.Tab { get { return this; } }
		Rectangle IDocumentGroupInfo.Client { get { return clientCore; } }
		void IDocumentGroupInfo.ProcessEvent(EventType eventType, object args) {
			if(IsDisposing) return;
			Handler.ProcessEvent(eventType, args);
		}
		void IDocumentGroupInfo.UpdateCustomHeaderButtons(DocumentGroup group) {
			UpdateCustomHeaderButtonsCore(group);
		}
		#endregion IDocumentGroupInfo Members
		void UpdateCustomHeaderButtonsCore(DocumentGroup group) {
			tabButtons.Clear();
			foreach(CustomHeaderButton btn in group.Properties.GetActualCustomHeaderButtons()) {
				tabButtons.Add(btn);
			}
			if(group != null && group.Properties.HasDocumentSelectorButton)
				tabButtons.Add(documentSelectorButton);
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return Owner; } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get { return uiChildren; }
		}
		#endregion IUIElement
		int lockCalculation = 0;
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			lockCalculation++;
			ViewInfo.SelectedTabPage = Group.GetSelectedTabPage();
			ViewInfo.CalcViewInfo(bounds, g);
			ViewInfo.CheckFirstPageIndex();
			clientCore = ViewInfo.PageClientBounds;
			lockCalculation--;
		}
		protected override bool CalcIsVisible() {
			return (ViewInfo.PageBounds.Width > 0) && (ViewInfo.PageBounds.Height > 0);
		}
		protected override void DrawCore(GraphicsCache cache) {
			Rectangle groupRect = Bounds;
			Rectangle clientRect = ViewInfo.PageClientBounds;
			Rectangle rect = ViewInfo.Bounds;
			if(Owner.Manager.IsFormRightToLeftLayout()) {
				groupRect.X--;
				clientRect.X--;
				rect.X--;
			}
			using(GraphicsClipState state = cache.ClipInfo.SaveAndSetClip(rect)) {
				cache.ClipInfo.ExcludeClip(clientRect);
				Painter.Draw(new TabDrawArgs(cache, ViewInfo, groupRect));
				cache.ClipInfo.RestoreClip(state);
			}
		}
		void OnSelectedPageChanged(object sender, ViewInfoTabPageChangedEventArgs e) {
			if(lockCalculation > 0) return;
			DocumentInfo info = (DocumentInfo)e.Page;
			if(info != null) {
				using(Owner.LockPainting()) {
					bool activated = Owner.Controller.Activate(info.Document);
					if(activated && !info.Document.IsSelected) {
						Group.SetSelected(info.Document);
						Owner.RequestInvokePatchActiveChild();
					}
				}
			}
		}
		void OnCloseButtonClick(object sender, EventArgs e) {
			ClosePageButtonEventArgs ea = e as ClosePageButtonEventArgs;
			DocumentInfo info = (DocumentInfo)ea.Page;
			if(info != null)
				Owner.Controller.Close(info.Document);
		}
		void OnTabMiddleClick(object sender, PageEventArgs e) {
			DocumentInfo info = (DocumentInfo)e.Page;
			if(info != null)
				Owner.Controller.Close(info.Document);
		}
		void OnCustomHeaderButtonClick(object sender, XtraTab.ViewInfo.CustomHeaderButtonEventArgs e) {
			DocumentInfo info = (DocumentInfo)e.ActivePage;
			if(info != null && info.Document != null) {
				if(e.Button == documentSelectorButton)
					ShowDocumentSelectorMenu(Owner.Controller as ITabbedViewController);
				else ((TabbedView)Owner).RaiseCustomHeaderButtonClick(e.Button, info.Document);
			}
		}
		public void ShowDocumentSelectorMenu(ITabbedViewController controller) {
			if(controller != null) {
				Rectangle btn = ViewInfo.HeaderInfo.Buttons.GetButtonBounds(documentSelectorButton);
				if(!btn.IsEmpty)
					controller.ShowContextMenu(Group, new Point(btn.Left, btn.Bottom));
			}
		}
		void OnPageClientBoundsChanged(object sender, EventArgs e) {
			if(!IsDisposing && Owner != null && Owner.Manager != null) {
				if(Owner.ViewInfo != null)
					Owner.ViewInfo.SetDirty();
				Owner.Manager.InvokePatchActiveChildren();
			}
		}
		AdornerElementInfo ReorderingAdornerInfo;
		public void UpdateReordering(Adorner adorner, Point point, BaseDocument document) {
			ReorderingAdornerInfoArgs args;
			if(ReorderingAdornerInfo == null) {
				args = new ReorderingAdornerInfoArgs(Owner);
				tabReorderingIndexCore = -1;
				args.HeaderLocation = ((IXtraTab)this).HeaderLocation;
				args.Header = Rectangle.Empty;
				args.Content = ViewInfo.PageBounds;
				ReorderingAdornerInfo = new AdornerElementInfo(new ReorderingAdornerPainter(), args);
				adorner.Show(ReorderingAdornerInfo);
			}
			else args = ReorderingAdornerInfo.InfoArgs as ReorderingAdornerInfoArgs;
			bool horz = GetIsHorizontalHeader();
			Rectangle header = CalcPageHeaderBounds(horz ? point.X : point.Y, ViewInfo.PageBounds, horz, out tabReorderingIndexCore);
			if(args.Header != header) {
				args.Header = header;
				args.TabHintVisible = (document == null) || !(Group.IsFilledUp && !Group.Items.Contains(document as Document));
				if(args.TabHintVisible) {
					DockGuidesConfiguration configuration = new DockGuidesConfiguration(new DockGuide[0], new DockHint[0]);
					configuration.tabReorderingIndex = new int?(TabReorderingIndex);
					Owner.OnShowingDockGuides(configuration, document, point);
					args.TabHintVisible = configuration.IsTabHintEnabled;
				}
				adorner.Invalidate();
			}
		}
		public void ResetReordering(Adorner adorner) {
			adorner.Reset(ReorderingAdornerInfo);
			ReorderingAdornerInfo = null;
		}
		public bool CanReorder(Point point) {
			if(ReorderingAdornerInfo != null) {
				ReorderingAdornerInfoArgs args = ReorderingAdornerInfo.InfoArgs as ReorderingAdornerInfoArgs;
				return args.TabHintVisible;
			}
			return false;
		}
		public void Reorder(Point point, BaseDocument document) {
			if(ReorderingAdornerInfo != null) {
				ReorderingAdornerInfoArgs args = ReorderingAdornerInfo.InfoArgs as ReorderingAdornerInfoArgs;
				if(args.TabHintVisible) {
					ITabbedViewController controller = Owner.Controller as ITabbedViewController;
					controller.Dock(document as Document, Group, TabReorderingIndex);
				}
			}
		}
		int tabReorderingIndexCore = -1;
		public int TabReorderingIndex {
			get { return tabReorderingIndexCore; }
		}
		AdornerElementInfo DockingAdornerInfo;
		public void UpdateDocking(Adorner adorner, Point point, BaseDocument dragItem) {
			DockingAdornerInfoArgs args = DockingAdornerInfoArgs.EnsureInfoArgs(ref DockingAdornerInfo, adorner, Owner, dragItem, ViewInfo.Bounds);
			args.Adorner = Owner.Manager.GetDockingRect();
			args.Container = Owner.Manager.Bounds;
			args.Bounds = ViewInfo.Bounds;
			args.Content = ViewInfo.PageBounds;
			bool horz = GetIsHorizontalHeader();
			args.Header = CalcPageHeaderBounds(horz ? point.X : point.Y, ViewInfo.PageBounds, horz, out tabReorderingIndexCore);
			args.HeaderLocation = ((IXtraTab)this).HeaderLocation;
			args.MousePosition = point;
			args.DragItem = dragItem;
			if(args.Calc())
				adorner.Invalidate();
		}
		public void ResetDocking(Adorner adorner) {
			adorner.Reset(DockingAdornerInfo);
			DockingAdornerInfo = null;
		}
		public bool CanDock(Point point) {
			DockHint hint = DockHint.None;
			if(DockingAdornerInfo != null) {
				DockingAdornerInfoArgs args = DockingAdornerInfo.InfoArgs as DockingAdornerInfoArgs;
				return args.IsOverDockHint(point, out hint);
			}
			return false;
		}
		public void Dock(Point point, BaseDocument document) {
			DockHint hint = DockHint.None;
			if(DockingAdornerInfo != null) {
				DockingAdornerInfoArgs args = DockingAdornerInfo.InfoArgs as DockingAdornerInfoArgs;
				if(args.IsOverDockHint(point, out hint)) {
					ITabbedViewController controller = Owner.Controller as ITabbedViewController;
					int index = ((TabbedView)Owner).DocumentGroups.IndexOf(Group);
					int targetIndex;
					Docking.FloatForm fForm = document.Form as Docking.FloatForm;
					(controller as TabbedViewController).TargetGroup = Group;
					(controller as TabbedViewController).After = !(hint == DockHint.CenterTop || hint == DockHint.CenterLeft);
					switch(hint) {
						case DockHint.Center:
							controller.Dock(document as Document, Group);
							break;
						case DockHint.CenterTop:
							if(!IsAlreadyDockedAsTargetGroup(document, index, out targetIndex)) {
								controller.CreateNewDocumentGroup(document as Document, Orientation.Horizontal, targetIndex);
							}
							break;
						case DockHint.CenterBottom:
							if(!IsAlreadyDockedAsTargetGroup(document, index + 1, out targetIndex)) {
								controller.CreateNewDocumentGroup(document as Document, Orientation.Horizontal, targetIndex);
							}
							break;
						case DockHint.CenterLeft:
							if(!IsAlreadyDockedAsTargetGroup(document, index, out targetIndex)) {
								controller.CreateNewDocumentGroup(document as Document, Orientation.Vertical, targetIndex);
							}
							break;
						case DockHint.CenterRight:
							if(!IsAlreadyDockedAsTargetGroup(document, index + 1, out targetIndex)) {
								controller.CreateNewDocumentGroup(document as Document, Orientation.Vertical, targetIndex);
							}
							break;
						case DockHint.SideLeft:
						case DockHint.SideTop:
						case DockHint.SideRight:
						case DockHint.SideBottom:
							new DockHelper(Owner).DockSide(document, fForm, hint);
							break;
						case DockHint.CenterSideLeft:
						case DockHint.CenterSideTop:
						case DockHint.CenterSideRight:
						case DockHint.CenterSideBottom:
							new DockHelper(Owner).DockCenterSide(document, fForm, hint);
							break;
					}
					(controller as TabbedViewController).TargetGroup = null;
				}
			}
		}
		bool IsAlreadyDockedAsTargetGroup(BaseDocument document, int index, out int targetGroupIndex) {
			DocumentGroup originalGroup = ((Document)document).Parent;
			bool originalGroupWillBeRemoved = (originalGroup != null) && !document.CanFloat() && (originalGroup.Items.Count == 1);
			int originalGroupIndex = (originalGroup != null) ? ((TabbedView)Owner).DocumentGroups.IndexOf(originalGroup) : -1;
			targetGroupIndex = ((originalGroupIndex != -1) && (originalGroupIndex < index) && originalGroupWillBeRemoved) ? index - 1 : index;
			if(originalGroupIndex != -1 && originalGroupIndex == targetGroupIndex && !document.CanFloat()) return false;
			return (originalGroupIndex != -1) && originalGroupIndex == targetGroupIndex;
		}
		protected bool GetIsHorizontalHeader() {
			XtraTab.TabHeaderLocation hLocation = ((IXtraTab)this).HeaderLocation;
			return (hLocation == XtraTab.TabHeaderLocation.Top) || (hLocation == XtraTab.TabHeaderLocation.Bottom);
		}
		protected bool IsRightToLeftLayout() {
			return !IsDisposing && Owner.Manager.IsRightToLeftLayout();
		}
		Rectangle CalcPageHeaderBounds(int pos, Rectangle pageBounds, bool horz, out int pageIndex) {
			pageIndex = -1;
			if(ViewInfo.HeaderInfo.VisiblePages.Count == 0)
				return Rectangle.Empty;
			Rectangle header = Rectangle.Empty;
			BaseTabPageViewInfo selectedInfo = ViewInfo.SelectedTabPageViewInfo;
			if(selectedInfo != null) {
				header = selectedInfo.Bounds;
				if(BaseTabHeaderViewInfo.HitTestHeader(header, pos, horz)) {
					pageIndex = ViewInfo.HeaderInfo.AllPages.IndexOf(selectedInfo);
					return header;
				}
			}
			foreach(BaseTabPageViewInfo pInfo in ViewInfo.HeaderInfo.VisiblePages) {
				header = pInfo.Bounds;
				if(BaseTabHeaderViewInfo.HitTestHeader(header, pos, horz)) {
					pageIndex = ViewInfo.HeaderInfo.AllPages.IndexOf(pInfo);
					return header;
				}
			}
			pageIndex = ViewInfo.HeaderInfo.AllPages.Count;
			pos = BaseTabHeaderViewInfo.CorrectPos(IsRightToLeftLayout(), pageBounds, header, horz);
			return new Rectangle(horz ? pos : header.Left, horz ? header.Top : pos, header.Width, header.Height);
		}
		public ToolTipControlInfo GetToolTipControlInfo(BaseViewHitInfo hitInfo) {
			return ViewInfo.GetToolTipInfo(hitInfo.HitPoint);
		}
		#region IXtraTabProperties Members
		DefaultBoolean IXtraTabProperties.AllowHotTrack {
			get { return DefaultBoolean.Default; }
		}
		AppearanceObject IXtraTabProperties.Appearance {
			get { return Owner.Appearance; }
		}
		PageAppearance IXtraTabProperties.AppearancePage {
			get { return ((TabbedView)Owner).AppearancePage; }
		}
		DevExpress.XtraEditors.Controls.BorderStyles IXtraTabProperties.BorderStyle {
			get { return DevExpress.XtraEditors.Controls.BorderStyles.Default; }
		}
		DevExpress.XtraEditors.Controls.BorderStyles IXtraTabProperties.BorderStylePage {
			get { return DevExpress.XtraEditors.Controls.BorderStyles.Default; }
		}
		DefaultBoolean IXtraTabProperties.AllowGlyphSkinning {
			get { return DefaultBoolean.Default; }
		}
		ClosePageButtonShowMode IXtraTabProperties.ClosePageButtonShowMode {
			get {
				ClosePageButtonShowMode mode = Group.Properties.ActualClosePageButtonShowMode;
				return (mode == ClosePageButtonShowMode.Default) ? ClosePageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover : mode;
			}
		}
		PinPageButtonShowMode IXtraTabProperties.PinPageButtonShowMode {
			get {
				PinPageButtonShowMode mode = Group.Properties.ActualPinPageButtonShowMode;
				return (mode == PinPageButtonShowMode.Default) ? PinPageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover : mode;
			}
		}
		DefaultBoolean IXtraTabProperties.HeaderAutoFill {
			get { return DefaultBoolean.False; }
		}
		TabButtons IXtraTabProperties.HeaderButtons {
			get { return CalcTabButtons(((IXtraTabProperties)this).ClosePageButtonShowMode); }
		}
		protected virtual TabButtons CalcTabButtons(ClosePageButtonShowMode mode) {
			TabButtons buttons = Group.Properties.GetActualHeaderButtons;
			if((buttons & TabButtons.Default) == TabButtons.Default)
				buttons = TabButtons.Close | TabButtons.Prev | TabButtons.Next;
			return buttons;
		}
		TabButtonShowMode IXtraTabProperties.HeaderButtonsShowMode {
			get { return Group.Properties.ActualHeaderButtonsShowMode; }
		}
		DefaultBoolean IXtraTabProperties.MultiLine {
			get { return DefaultBoolean.False; }
		}
		TabPageImagePosition IXtraTabProperties.PageImagePosition {
			get { return Group.Properties.ActualPageImagePosition; }
		}
		DefaultBoolean IXtraTabProperties.ShowHeaderFocus {
			get { return DefaultBoolean.False; }
		}
		DefaultBoolean IXtraTabProperties.ShowTabHeader {
			get { return Group.Properties.HasTabHeader ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		DefaultBoolean IXtraTabProperties.ShowToolTips {
			get { return DefaultBoolean.Default; }
		}
		int IXtraTabProperties.TabPageWidth {
			get { return Group.Properties.TabWidth; }
		}
		#endregion
		#region IXtraTabPropertiesEx Members
		CustomHeaderButtonCollection IXtraTabPropertiesEx.CustomHeaderButtons {
			get { return tabButtons; }
		}
		TabMiddleClickFiringMode IXtraTabPropertiesEx.TabMiddleClickFiringMode {
			get { return (TabMiddleClickFiringMode)((int)Group.Properties.ActualCloseTabOnMiddleClick); }
		}
		#endregion
	}
	class SkinDocumentGroupViewInfoRegistrator : SkinViewInfoRegistrator {
		public override BaseTabControlViewInfo CreateViewInfo(IXtraTab tabControl) {
			return new SkinDocumentGroupViewInfo(tabControl);
		}
		public override BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new SkinDocumentGroupHeaderViewInfo(viewInfo);
		}
		public override ObjectPainter CreateClosePageButtonPainter(IXtraTab tabControl) {
			return new SkinDocumentButtonPainter(tabControl.LookAndFeel);
		}
		public override DevExpress.XtraEditors.ButtonPanel.BaseButtonsPanelPainter CreateControlBoxPainter(IXtraTab tabControl) {
			return new TabDocumentButtonsPanelSkinPainter(tabControl.LookAndFeel);
		}
		SkinElement GetSkinElement(IXtraTab tabControl, string element) {
			return GetDockingSkin(tabControl)[element];
		}
		Skin GetDockingSkin(IXtraTab tabControl) {
			return DockingSkins.GetSkin(tabControl.LookAndFeel);
		}
		protected override void RegisterDefaultAppearances(IXtraTab tabControl, Hashtable appearances) {
			base.RegisterDefaultAppearances(tabControl, appearances);
			SkinElement tabPaneElement = GetSkinElement(tabControl, DockingSkins.SkinDocumentGroupTabPane);
			if(tabPaneElement != null) {
				appearances[TabPageAppearance.PageClient] = tabPaneElement.Apply(
					new AppearanceDefault(SystemColors.ControlText, Color.Transparent, HorzAlignment.Center, VertAlignment.Center));
			}
			Skin skin = GetDockingSkin(tabControl);
			UseSkinColor(TabPageAppearance.PageHeader, DockingSkins.DocumentGroupHeaderTextColor, appearances, skin);
			UseSkinColor(TabPageAppearance.PageHeaderActive, DockingSkins.DocumentGroupHeaderTextColorActive, appearances, skin);
			UseSkinColor(TabPageAppearance.PageHeaderTabInactive, DockingSkins.DocumentGroupHeaderTextColorGroupInactive, appearances, skin);
			UseSkinColor(TabPageAppearance.PageHeaderHotTracked, DockingSkins.DocumentGroupHeaderTextColorHot, appearances, skin);
			UseSkinColor(TabPageAppearance.PageHeaderPressed, DockingSkins.DocumentGroupHeaderTextColorHot, appearances, skin);
			UseSkinColor(TabPageAppearance.PageHeaderDisabled, DockingSkins.DocumentGroupHeaderTextColorDisabled, appearances, skin);
			UseSkinColor(TabPageAppearance.TabHeaderButton, DockingSkins.DocumentGroupHeaderButtonTextColor, appearances, skin);
			UseSkinColor(TabPageAppearance.TabHeaderButtonHot, DockingSkins.DocumentGroupHeaderButtonTextColorHot, appearances, skin);
		}
		void UseSkinColor(TabPageAppearance appearance, string color, Hashtable appearances, Skin skin) {
			if(skin.Colors.Contains(color)) {
				AppearanceDefault ap = appearances[appearance] as AppearanceDefault;
				if(ap != null && ap.Font != null)
					appearances[appearance] = new AppearanceDefault(skin.Colors[color], Color.Transparent, ap.Font);
				else appearances[appearance] = new AppearanceDefault(skin.Colors[color], Color.Transparent);
			}
		}
	}
	class SkinDocumentGroupHeaderViewInfo : SkinTabHeaderViewInfo {
		public SkinDocumentGroupHeaderViewInfo(BaseTabControlViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected override EditorButtonPainter OnHeaderButtonGetPainter(TabButtonInfo button) {
			if(button.ButtonType == TabButtonType.User)
				return new SkinDocumentGroupCustomHeaderButtonPainter(ViewInfo as SkinDocumentGroupViewInfo, button.Button.Kind);
			return new SkinDocumentGroupHeaderButtonPainter(ViewInfo as SkinDocumentGroupViewInfo);
		}
		protected override BaseTabPageViewInfo CreatePage(IXtraTabPage page) {
			return new SkinDocumentGroupTabPageViewInfo(page);
		}
	}
	class SkinDocumentGroupTabPageViewInfo : BaseTabPageViewInfo {
		public SkinDocumentGroupTabPageViewInfo(IXtraTabPage page)
			: base(page) {
		}
		protected override AppearanceObject GetPageAppearance(PageAppearance app, ObjectState state, DefaultBoolean isActive) {
			bool actualIsActive = IsActive;
			if(isActive != DefaultBoolean.Default)
				actualIsActive = (isActive != DefaultBoolean.False);
			return PageAppearanceHelper.GetPageAppearance(app, state, actualIsActive);
		}
		public bool IsActive {
			get { return ((IDocumentGroupInfo)TabControl).IsActive; }
		}
	}
	class SkinDocumentGroupViewInfo : SkinTabControlViewInfo {
		Skin skin;
		public SkinDocumentGroupViewInfo(IXtraTab tabControl)
			: base(tabControl) {
			skin = DockingSkins.GetSkin(TabControl.LookAndFeel);
		}
		public override SkinElement SkinPane {
			get { return skin[DockingSkins.SkinDocumentGroupTabPane] ?? base.SkinPane; }
		}
		public override SkinElement SkinHeader {
			get { return skin[DockingSkins.SkinDocumentGroupTabHeader] ?? base.SkinHeader; }
		}
		public override bool AllowInactiveState {
			get { return !IsDisposing; }
		}
		public override bool IsActive {
			get { return ((IDocumentGroupInfo)TabControl).IsActive; }
		}
		public override Color GetSkinColoredTabAdjustForeColor() {
			return (skin[DockingSkins.SkinDocumentGroupTabHeader] != null) ?
				skin.Properties.GetColor(TabSkinProperties.ColoredTabAdjustForeColor) :
				base.GetSkinColoredTabAdjustForeColor();
		}
		public override Color GetSkinColoredTabBaseForeColor() {
			return (skin[DockingSkins.SkinDocumentGroupTabHeader] != null) ?
				skin.Properties.GetColor(TabSkinProperties.ColoredTabBaseForeColor) :
				base.GetSkinColoredTabBaseForeColor();
		}
	}
	class SkinDocumentButtonPainter : SkinTabPageButtonPainter {
		public SkinDocumentButtonPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetSkinElement(EditorButtonObjectInfoArgs e, DevExpress.XtraEditors.Controls.ButtonPredefines kind) {
			return DockingSkins.GetSkin(Provider)[DockingSkins.SkinDocumentGroupTabPageButton] ?? base.GetSkinElement(e, kind);
		}
	}
	class TabDocumentButtonSkinPainter : XtraEditors.ButtonPanel.TabButtonSkinPainter {
		public TabDocumentButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetBackground(XtraEditors.ButtonPanel.TabButtonsPanelState state) {
			switch(state) {
				case XtraEditors.ButtonPanel.TabButtonsPanelState.Active:
					return DockingSkins.GetSkin(SkinProvider)[DockingSkins.SkinDocumentGroupInactiveTabPageButton] ?? base.GetBackground();
				case XtraEditors.ButtonPanel.TabButtonsPanelState.Selected:
					return DockingSkins.GetSkin(SkinProvider)[DockingSkins.SkinDocumentGroupActiveTabPageButton] ?? base.GetBackground();
			}
			return DockingSkins.GetSkin(SkinProvider)[DockingSkins.SkinDocumentGroupTabPageButton] ?? base.GetBackground();
		}
		protected override Skin GetSkin() {
			return DockingSkins.GetSkin(SkinProvider) ?? base.GetSkin();
		}
	}
	class TabDocumentButtonsPanelSkinPainter : XtraEditors.ButtonPanel.TabButtonsPanelSkinPainter {
		public TabDocumentButtonsPanelSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override XtraEditors.ButtonPanel.BaseButtonPainter GetButtonPainter() {
			return new TabDocumentButtonSkinPainter(Provider);
		}
	}
	class SkinDocumentGroupHeaderButtonPainter : SkinTabHeaderButtonPainter {
		public SkinDocumentGroupHeaderButtonPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetSkinElement(EditorButtonObjectInfoArgs e, DevExpress.XtraEditors.Controls.ButtonPredefines kind) {
			return DockingSkins.GetSkin(Provider)[DockingSkins.SkinDocumentGroupTabHeaderButton] ?? base.GetSkinElement(e, kind);
		}
	}
	class SkinDocumentGroupCustomHeaderButtonPainter : SkinTabCustomHeaderButtonPainter {
		public SkinDocumentGroupCustomHeaderButtonPainter(ISkinProvider provider, DevExpress.XtraEditors.Controls.ButtonPredefines kind)
			: base(provider, kind) {
		}
		protected override SkinElement GetSkinElement(EditorButtonObjectInfoArgs e, DevExpress.XtraEditors.Controls.ButtonPredefines kind) {
			return DockingSkins.GetSkin(Provider)[DockingSkins.SkinDocumentGroupTabHeaderButton] ?? base.GetSkinElement(e, kind);
		}
	}
	class Office2003DocumentGroupViewInfoRegistrator : Office2003ViewInfoRegistrator {
		public override BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new Office2003DocumentGroupHeaderViewInfo(viewInfo);
		}
	}
	class Office2003DocumentGroupHeaderViewInfo : Office2003TabHeaderViewInfo {
		public Office2003DocumentGroupHeaderViewInfo(BaseTabControlViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected override BaseTabPageViewInfo CreatePage(IXtraTabPage page) {
			return new Office2003DocumentGroupTabPageViewInfo(page);
		}
	}
	class Office2003DocumentGroupTabPageViewInfo : Office2003TabPageViewInfo {
		public Office2003DocumentGroupTabPageViewInfo(IXtraTabPage page)
			: base(page) {
		}
		protected override AppearanceObject GetPageAppearance(PageAppearance app, ObjectState state, DefaultBoolean isActive) {
			bool actualIsActive = IsActive;
			if(isActive != DefaultBoolean.Default)
				actualIsActive = (isActive != DefaultBoolean.False);
			return PageAppearanceHelper.GetPageAppearance(app, state, actualIsActive);
		}
		public bool IsActive {
			get { return ((IDocumentGroupInfo)TabControl).IsActive; }
		}
	}
	class FlatDocumentGroupViewInfoRegistrator : FlatViewInfoRegistrator {
		public override BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new FlatDocumentGroupHeaderViewInfo(viewInfo);
		}
	}
	class FlatDocumentGroupHeaderViewInfo : FlatTabHeaderViewInfo {
		public FlatDocumentGroupHeaderViewInfo(BaseTabControlViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected override BaseTabPageViewInfo CreatePage(IXtraTabPage page) {
			return new FlatDocumentGroupTabPageViewInfo(page);
		}
	}
	class FlatDocumentGroupTabPageViewInfo : BaseTabPageViewInfo {
		public FlatDocumentGroupTabPageViewInfo(IXtraTabPage page)
			: base(page) {
		}
		protected override AppearanceObject GetPageAppearance(PageAppearance app, ObjectState state, DefaultBoolean isActive) {
			bool actualIsActive = IsActive;
			if(isActive != DefaultBoolean.Default)
				actualIsActive = (isActive != DefaultBoolean.False);
			return PageAppearanceHelper.GetPageAppearance(app, state, actualIsActive);
		}
		public bool IsActive {
			get { return ((IDocumentGroupInfo)TabControl).IsActive; }
		}
	}
	class StandardDocumentGroupViewInfoRegistrator : BaseViewInfoRegistrator {
		public override BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new StandardDocumentGroupHeaderViewInfo(viewInfo);
		}
	}
	class StandardDocumentGroupHeaderViewInfo : BaseTabHeaderViewInfo {
		public StandardDocumentGroupHeaderViewInfo(BaseTabControlViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected override BaseTabPageViewInfo CreatePage(IXtraTabPage page) {
			return new StandardDocumentGroupTabPageViewInfo(page);
		}
	}
	class StandardDocumentGroupTabPageViewInfo : BaseTabPageViewInfo {
		public StandardDocumentGroupTabPageViewInfo(IXtraTabPage page)
			: base(page) {
		}
		protected override AppearanceObject GetPageAppearance(PageAppearance app, ObjectState state, DefaultBoolean isActive) {
			bool actualIsActive = IsActive;
			if(isActive != DefaultBoolean.Default)
				actualIsActive = (isActive != DefaultBoolean.False);
			return PageAppearanceHelper.GetPageAppearance(app, state, actualIsActive);
		}
		public bool IsActive {
			get { return ((IDocumentGroupInfo)TabControl).IsActive; }
		}
	}
	class WindowsXPDocumentGroupViewInfoRegistrator : WindowsXPViewInfoRegistrator {
		public override BaseTabHeaderViewInfo CreateHeaderViewInfo(BaseTabControlViewInfo viewInfo) {
			return new WindowsXPDocumentGroupHeaderViewInfo(viewInfo);
		}
	}
	class WindowsXPDocumentGroupHeaderViewInfo : WindowsXPTabHeaderViewInfo {
		public WindowsXPDocumentGroupHeaderViewInfo(BaseTabControlViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected override BaseTabPageViewInfo CreatePage(IXtraTabPage page) {
			return new WindowsXPDocumentGroupTabPageViewInfo(page);
		}
	}
	class WindowsXPDocumentGroupTabPageViewInfo : WindowsXPTabPageViewInfo {
		public WindowsXPDocumentGroupTabPageViewInfo(IXtraTabPage page)
			: base(page) {
		}
		protected override AppearanceObject GetPageAppearance(PageAppearance app, ObjectState state, DefaultBoolean isActive) {
			bool actualIsActive = IsActive;
			if(isActive != DefaultBoolean.Default)
				actualIsActive = (isActive != DefaultBoolean.False);
			return PageAppearanceHelper.GetPageAppearance(app, state, actualIsActive);
		}
		public bool IsActive {
			get { return ((IDocumentGroupInfo)TabControl).IsActive; }
		}
	}
	static class PageAppearanceHelper {
		public static AppearanceObject GetPageAppearance(PageAppearance app, ObjectState state, bool isActive) {
			TabbedViewPageAppearance appearance = app as TabbedViewPageAppearance;
			bool selected = (state & ObjectState.Selected) != 0;
			ObjectState temp = state & (~ObjectState.Selected);
			AppearanceObject res = null;
			switch(temp) {
				case ObjectState.Disabled: res = app.HeaderDisabled; break;
				case ObjectState.Hot: res = app.HeaderHotTracked; break;
			}
			if(!selected) return res == null ? app.Header : res;
			AppearanceObject comb = new AppearanceObject();
			if(isActive)
				AppearanceHelper.Combine(comb, new AppearanceObject[] { app.HeaderActive, appearance.HeaderSelected, app.Header, res });
			else
				AppearanceHelper.Combine(comb, new AppearanceObject[] { appearance.HeaderSelected, app.Header, res });
			return comb;
		}
	}
}
