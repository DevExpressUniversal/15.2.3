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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors;
namespace DevExpress.XtraTab.Buttons {
	public enum TabButtonType { 
		Next, Prev, Close, User 
	};
	[TypeConverter(typeof(EditorButtonTypeConverter))]
	public class CustomHeaderButton : EditorButton {
		[EditorButtonPreferredConstructor]
		public CustomHeaderButton() { }
		[EditorButtonPreferredConstructor]
		public CustomHeaderButton(ButtonPredefines kind)
			: base(kind) {
		}
		[EditorButtonPreferredConstructor]
		public CustomHeaderButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, ImageLocation imageLocation, Image image, AppearanceObject appearance, string toolTip, object tag, SuperToolTip superTip, bool enableImageTransparency)
			: base(kind, caption, width, enabled, visible, true, imageLocation, image, KeyShortcut.Empty, appearance, toolTip, tag, superTip, enableImageTransparency) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override KeyShortcut Shortcut { get { return base.Shortcut; } set { base.Shortcut = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool IsLeft { get { return base.IsLeft; } set { base.IsLeft = value; } }
		CustomHeaderButtonCollection collectionCore;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new CustomHeaderButtonCollection Collection { get { return collectionCore; } }
		internal void SetCollection(CustomHeaderButtonCollection collection) {
			this.collectionCore = collection;
			this.indexCore = -1;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int Index {
			get {
				if(indexCore == -1) {
					if(Collection != null)
						indexCore = Collection.IndexOf(this);
				}
				return indexCore;
			}
		}
		protected override void InitAppearance(AppearanceObject appearance) {
			if(appearance != null)
				Appearance.AssignInternal(appearance);
		}
	}
	[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
	[Editor("DevExpress.XtraEditors.Design.CustomButtonsCollectionEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
	public class CustomHeaderButtonCollection : CollectionBase {
		int _lockUpdate;
		public event CollectionChangeEventHandler CollectionChanged;
		public override string ToString() {
			if(Count == 0) return "None";
			if(Count == 1) {
				return string.Concat("{", this[0].Kind.ToString(), "}");
			}
			return string.Format("Count {0}", Count);
		}
		public CustomHeaderButtonCollection() {
			this._lockUpdate = 0;
		}
		protected virtual void BeginUpdate() {
			_lockUpdate++;
		}
		protected virtual void EndUpdate() {
			if(--_lockUpdate == 0)
				OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected void UpdateIndexes() {
			for(int n = Count - 1; n >= 0; n--) this[n].SetIndex(n);
		}
		public CustomHeaderButton this[int index] { get { return List[index] as CustomHeaderButton; } }
		public virtual void AddRange(CustomHeaderButton[] buttons) {
			BeginUpdate();
			try {
				foreach(CustomHeaderButton button in buttons) {
					List.Add(button);
				}
			}
			finally {
				EndUpdate();
			}
		}
		[Browsable(false)]
		public virtual int VisibleCount {
			get {
				int count = Count, res = 0;
				if(count == 0) return 0;
				for(int n = 0; n < count; n++) {
					if(this[n].Visible) res++;
				}
				return res;
			}
		}
		public virtual void Assign(CustomHeaderButtonCollection collection) {
			BeginUpdate();
			try {
				Clear();
				for(int n = 0; n < collection.Count; n++) {
					CustomHeaderButton button = collection[n];
					CustomHeaderButton newButton = new CustomHeaderButton();
					newButton.Assign(button);
					Add(newButton);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual int Add(CustomHeaderButton button) {
			int res = IndexOf(button);
			if(res == -1) res = List.Add(button);
			return res;
		}
		public virtual int IndexOf(CustomHeaderButton button) { return List.IndexOf(button); }
		public virtual bool Contains(CustomHeaderButton button) { return List.Contains(button); }
		public virtual void Insert(int index, CustomHeaderButton button) {
			if(Contains(button)) return;
			List.Insert(index, button);
		}
		protected override void OnInsertComplete(int index, object item) {
			CustomHeaderButton button = item as CustomHeaderButton;
			button.Changed += new EventHandler(OnButton_Changed);
			button.SetCollection(this);
			UpdateIndexes();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int index, object item) {
			CustomHeaderButton button = item as CustomHeaderButton;
			button.Changed -= new EventHandler(OnButton_Changed);
			button.SetCollection(null);
			UpdateIndexes();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected override void OnClear() {
			if(Count == 0) return;
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--)
					RemoveAt(n);
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void OnButton_Changed(object sender, EventArgs e) {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
		}
		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		static CustomHeaderButtonCollection emptyCollection = new EmptyCustomHeaderButtonCollection();
		internal static CustomHeaderButtonCollection Empty {
			get { return emptyCollection; }
		}
		class EmptyCustomHeaderButtonCollection : CustomHeaderButtonCollection {
			public override int Add(CustomHeaderButton button) { return -1; }
			public override void AddRange(CustomHeaderButton[] buttons) { }
			public override void Assign(CustomHeaderButtonCollection collection) { }
			public override bool Contains(CustomHeaderButton button) { return false; }
			public override int IndexOf(CustomHeaderButton button) { return -1; }
			public override void Insert(int index, CustomHeaderButton button) { }
		}
	}
	public class TabButtonsPanel : IDisposable {
		GraphicsInfo gInfo;
		TabButtonInfoCollection buttons;
		TabButtonInfo pressedButton, hotTrackedButton;
		Rectangle bounds, client;
		BaseTabControlViewInfo tabViewInfo;
		bool dirty = true;
		Timer clickTimer;
		Timer ClickTimer {
			get {
				if(clickTimer == null) {
					this.clickTimer = new Timer();
					this.clickTimer.Interval = 200;
					this.clickTimer.Tick += new EventHandler(OnClickTimer);
				}
				return clickTimer;
			}
		}
		public TabButtonsPanel(BaseTabControlViewInfo tabViewInfo) {
			this.tabViewInfo = tabViewInfo;
			this.client = this.bounds = Rectangle.Empty;
			this.gInfo = new GraphicsInfo();
			this.buttons = new TabButtonInfoCollection(this);
			this.pressedButton = this.hotTrackedButton = null;
		}
		public virtual void Dispose() {
			this.buttons.Clear();
			if(clickTimer != null) {
				this.clickTimer.Tick -= new EventHandler(OnClickTimer);
				this.clickTimer.Dispose();
				clickTimer = null;
			}
			this.pressedButton = null;
			this.hotTrackedButton = null;
		}
		public bool IsDirty { get { return dirty; } set { dirty = value; } }
		public virtual void CreateButtons(TabButtons buttons, CustomHeaderButtonCollection userButtons) {
			if(this.keepButtons > 0) return;
			Buttons.Clear();
			CreateButtonsCore(buttons, userButtons);
		}
		public virtual void CreateButtons(TabButtons buttons) {
			CreateButtons(buttons, CustomHeaderButtonCollection.Empty);
		}
		protected virtual void CreateButtonsCore(TabButtons buttons, CustomHeaderButtonCollection userButtons) {
			bool rtl = TabViewInfo.IsRightToLeftLocation;
			foreach(CustomHeaderButton button in userButtons)
				this.buttons.AddRTL(CreateUserButton(button), rtl);
			if(rtl) {
				if((buttons & TabButtons.Next) != 0)
					this.buttons.AddRTL(CreateButton(TabButtonType.Next), rtl);
				if((buttons & TabButtons.Prev) != 0)
					this.buttons.AddRTL(CreateButton(TabButtonType.Prev), rtl);
			}
			else {
				if((buttons & TabButtons.Prev) != 0)
					this.buttons.AddRTL(CreateButton(TabButtonType.Prev), rtl);
				if((buttons & TabButtons.Next) != 0)
					this.buttons.AddRTL(CreateButton(TabButtonType.Next), rtl);
			}
			if((buttons & TabButtons.Close) != 0)
				this.buttons.AddRTL(CreateButton(TabButtonType.Close), rtl);
		}
		protected internal virtual ToolTipControlInfo GetToolTipInfo(Point point) {
			if(Bounds.IsEmpty || !Bounds.Contains(point)) return null;
			TabButtonInfo button = Buttons[point];
			if(button != null && this.Orientation != TabOrientation.Vertical) {
				string tool = GetButtonTooltip(button);
				if((tool != string.Empty) || (button.Button.SuperTip != null && !button.Button.SuperTip.IsEmpty)) {
					ToolTipControlInfo toolTip = new ToolTipControlInfo(button, tool);
					toolTip.SuperTip = button.Button.SuperTip != null ? button.Button.SuperTip : null;
					return toolTip;
				}
			}
			return null;
		}
		public virtual string GetButtonTooltip(TabButtonInfo info) {
			StringId id = StringId.None;
			switch(info.ButtonType) {
				case TabButtonType.Prev : id = StringId.TabHeaderButtonPrev; break;
				case TabButtonType.Next : id = StringId.TabHeaderButtonNext; break;
				case TabButtonType.Close : id = StringId.TabHeaderButtonClose; break;
				case TabButtonType.User: 
					return info.Button.ToolTip;
			}
			if(id == StringId.None) return string.Empty;
			return Localizer.Active.GetLocalizedString(id);
		}
		static TabButtons[] buttonTypes = new TabButtons[] { TabButtons.Next, TabButtons.Prev, TabButtons.Close, TabButtons.None};
		protected virtual bool CanShowButton(TabButtonType button) {
			TabButtons allowedButtons = TabViewInfo.HeaderInfo.GetHeaderButtons();
			if(button == TabButtonType.User) 
				return true;
			TabButtons b = buttonTypes[(int)button];
			if(b == TabButtons.None || (allowedButtons & b) == 0) return false;
			return true;
		}
		protected virtual TabButtonInfo CreateUserButton(CustomHeaderButton button) {
			return new UserTabButtonInfo(button);
		}
		protected virtual TabButtonInfo CreateButton(TabButtonType button) {
			if(!CanShowButton(button)) return null;
			ButtonPredefines predefines = ButtonPredefines.Glyph;
			switch(button) {
				case TabButtonType.Prev:
					predefines = Orientation == TabOrientation.Horizontal ? ButtonPredefines.Left : ButtonPredefines.Up;
					break;
				case TabButtonType.Next:
					predefines = Orientation == TabOrientation.Horizontal ? ButtonPredefines.Right : ButtonPredefines.Down;
					break;
				case TabButtonType.Close:
					predefines = ButtonPredefines.Close;
					break;
			}
			EditorButton editorButton = new EditorButton(predefines);
			return new TabButtonInfo(editorButton, button);
		}
		public BaseTabControlViewInfo TabViewInfo { get { return tabViewInfo; } }
		public virtual void ProcessEvent(ProcessEventEventArgs e) {
			if(Bounds.IsEmpty) return;
			MouseEventArgs mouseArgs = e.EventArgs as MouseEventArgs;
			switch(e.EventType) {
				case EventType.LostCapture: 
					PressedButton = null;
					break;
				case EventType.MouseMove :
				case EventType.MouseEnter:
					HotTrackedButton = Buttons[mouseArgs.Location];
					break;
				case EventType.MouseLeave:
					HotTrackedButton = null;
					break;
				case EventType.MouseDown :
					OnMouseDown(mouseArgs);
					break;
				case EventType.MouseUp :
					OnMouseUp(mouseArgs);
					break;
			}
		}
		protected virtual void OnMouseDown(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) return;
			TabButtonInfo button = Buttons[e.Location];
			OnPressButton(button);
		}
		protected virtual void OnMouseUp(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) return;
			TabButtonInfo button = Buttons[e.Location], prevPressed = PressedButton;
			PressedButton = null;
			if(button != null && AreEqual(button, prevPressed) && !IsTimerButton(button)) OnClickButton(button);
		}
		static bool AreEqual(TabButtonInfo btnInfo1, TabButtonInfo btnInfo2) {
			return (btnInfo1 == btnInfo2) || AreButtonsEqual(btnInfo1, btnInfo2) || AreButtonTypesEqual(btnInfo1, btnInfo2);
		}
		static bool AreButtonsEqual(TabButtonInfo btnInfo1, TabButtonInfo btnInfo2) {
			return (btnInfo1 != null) && (btnInfo2 != null) && (btnInfo1.Button == btnInfo2.Button);
		}
		static bool AreButtonTypesEqual(TabButtonInfo btnInfo1, TabButtonInfo btnInfo2) {
			return (btnInfo1 != null) && (btnInfo2 != null) && ((btnInfo1.ButtonType == btnInfo2.ButtonType) && btnInfo1.ButtonType != TabButtonType.User);
		}
		protected virtual bool IsTimerButton(TabButtonInfo button) {
			return button != null && (button.ButtonType == TabButtonType.Next || button.ButtonType == TabButtonType.Prev);
		}
		protected virtual void OnClickTimer(object sender, EventArgs e) {
			Point pt = TabViewInfo.TabControl.ScreenPointToControl(Control.MousePosition);
			TabButtonInfo button = Buttons[pt];
			if(PressedButton != null && IsTimerButton(PressedButton) && AreEqual(button, PressedButton)) {
				OnClickButton(PressedButton);
			}
		}
		protected int keepButtons = 0;
		protected internal virtual void OnClickButton(TabButtonInfo button) {
			this.keepButtons ++;
			try {
				if(CanRaiseHeaderButtonClick(button))
					TabViewInfo.OnHeaderButtonClick(button);
			} finally {
				this.keepButtons --;
			}
			if(!TabViewInfo.IsDisposing)
				TabViewInfo.LayoutChanged();	
		}
		protected virtual bool CanRaiseHeaderButtonClick(TabButtonInfo button) {
			return button.Button.Enabled && (button.State & ObjectState.Disabled) == 0;
		}
		protected virtual void OnUnPressButton(TabButtonInfo button) {
			StopClickTimer();
		}
		protected void StopClickTimer() {
			if(clickTimer != null)
				clickTimer.Stop();
		}
		protected void StartClickTimer() {
			ClickTimer.Start();
		}
		protected virtual void OnPressButton(TabButtonInfo button) {
			TabViewInfo.TabControl.OwnerControl.Capture = button != null;
			PressedButton = button;
			if(IsTimerButton(button)) {
				StartClickTimer();
				OnClickButton(button);
			}
		}
		public virtual void Draw(GraphicsCache cache) {
			if(IsDirty) CalcViewInfo(cache.Graphics);
			Buttons.Draw(cache);
		}
		public virtual Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public virtual Rectangle Client { get { return client; } }
		protected internal virtual TabButtonInfoCollection Buttons { get { return buttons; } }
		protected GraphicsInfo GInfo { get { return gInfo; } }
		public TabOrientation Orientation { 
			get { 
				if(TabViewInfo.HeaderInfo.IsSideLocation) return TabOrientation.Vertical;
				return TabOrientation.Horizontal;
			}
		}
		public Rectangle GetButtonBounds(EditorButton button) {
			foreach(TabButtonInfo info in Buttons) {
				if(info.Button == button)
					return info.Bounds;
			}
			return Rectangle.Empty;
		}
		public virtual void Invalidate(Rectangle bounds) {
			TabViewInfo.Invalidate(bounds);
		}
		public virtual TabButtonInfo HotTrackedButton {
			get { return hotTrackedButton; }
			set {
				if(HotTrackedButton == value) return;
				hotTrackedButton = value;
				UpdateButtonStates();
				Invalidate(Bounds);
			}
		}
		public virtual TabButtonInfo PressedButton {
			get { return pressedButton; }
			set {
				if(PressedButton == value) return;
				TabButtonInfo prev = pressedButton;
				pressedButton = value;
				UpdateButtonStates();
				Invalidate(Bounds);
				if(prev != null) OnUnPressButton(prev);
			}
		}
		public virtual void UpdateButtonStates() {
			for(int n = 0; n < Buttons.Count; n++) {
				TabButtonInfo info = Buttons[n];
				ObjectState old = info.State;
				info.State = CalcButtonState(info);
				if(info.State != old) this.dirty = true;
			}
		}
		protected virtual ObjectState CalcButtonState(TabButtonInfo button) {
			ObjectState state = ObjectState.Normal;
			state = TabViewInfo.OnHeaderButtonCalcState(button);
			if(state == ObjectState.Disabled) return state;
			if(button == HotTrackedButton) state |= ObjectState.Hot;
			if(button == PressedButton) state |= ObjectState.Pressed;
			return state;
		}
		public virtual void CalcViewInfo(Graphics g) {
			UpdateButtonStates();
			this.dirty = false;
			Size res = Size.Empty;
			g = GInfo.AddGraphics(g);
			try {
				this.client = Buttons.CalcViewInfo(g, Bounds);
			} finally {
				GInfo.ReleaseGraphics();
			}
		}
		public virtual Size CalcSize(Graphics g) {
			Size res = Size.Empty;
			g = GInfo.AddGraphics(g);
			try {
				res = Buttons.CalcSize(g);
			} finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
	}
	public class UserTabButtonInfo : TabButtonInfo {
		public UserTabButtonInfo(EditorButton button)
			: base(button, TabButtonType.User) {
		}
		protected override void UpdateViewInfo(Graphics g) {
			base.UpdateViewInfo(g);
			if(Button.Kind == ButtonPredefines.Glyph)
				ViewInfo.Indent = new Size(2, 1);
		}
	}
	public class TabButtonInfo {
		EditorButton button;
		EditorButtonObjectInfoArgs viewInfo;
		TabButtonInfoCollection collection = null;
		EditorButtonPainter painter;
		TabButtonType buttonType;
		public TabButtonInfo(EditorButton button) : this(button, TabButtonType.User) { }
		public TabButtonInfo(EditorButton button, TabButtonType buttonType) {
			this.buttonType = buttonType;
			this.button = button;
			this.paintAppearanceCore = new FrozenAppearance();
			CreateViewInfo();
		}
		AppearanceObject paintAppearanceCore;
		public AppearanceObject PaintAppearance {
			get { return paintAppearanceCore; }
		}
		public void UpdatePaintAppearance(BaseTabControlViewInfo viewInfo) {
			AppearanceObject appearance = (ButtonType == TabButtonType.User) ?
				GetCustomHeaderButtonAppearance(viewInfo) :
				viewInfo.HeaderInfo.HeaderButtonPaintAppearance;
			AppearanceHelper.Combine(PaintAppearance, Button.Appearance, appearance);
		}
		protected virtual AppearanceObject GetCustomHeaderButtonAppearance(BaseTabControlViewInfo viewInfo) {
			return ((State & ObjectState.Hot) == 0) ?
				viewInfo.HeaderInfo.CustomHeaderButtonPaintAppearance :
				viewInfo.HeaderInfo.CustomHeaderButtonPaintAppearanceHot;
		}
		public TabButtonType ButtonType {
			get { return buttonType; }
			set {
				if(ButtonType == value) return;
				buttonType = value;
				CreatePainter();
			}
		}
		protected internal virtual void CreatePainter() {
			if(Collection != null) 
				Painter = Collection.Panel.TabViewInfo.HeaderInfo.OnHeaderButtonGetPainter(this);
			else {
				Painter = new EditorButtonPainter(LookAndFeel.UserLookAndFeel.Default.Painter.Button);
			}
		}
		public EditorButtonPainter Painter { 
			get { 
				if(painter == null) CreatePainter();
				return painter; 
			}
			set { painter = value; }
		}
		public EditorButton Button { get { return button; } }
		public Rectangle Bounds { get { return ViewInfo.Bounds; } set { ViewInfo.Bounds = value; } }
		public EditorButtonObjectInfoArgs ViewInfo { get { return viewInfo; } }
		public void CreateViewInfo() {
			this.viewInfo = new EditorButtonObjectInfoArgs(Button, null);
		}
		public ObjectState State { 
			get { return ViewInfo.State; }
			set { ViewInfo.State = value; }
		}
		public void UpdateState(Graphics g, ObjectState state) {
			if(State == state) return;
			try {
				ViewInfo.Graphics = g;
				Painter.CalcObjectBounds(ViewInfo);
			} finally {
				ViewInfo.Graphics = null;
			}
		}
		public void Draw(GraphicsCache cache) {
			if(Bounds.IsEmpty || !Button.Visible) return;
			ViewInfo.Cache = cache;
			ViewInfo.SetAppearance(PaintAppearance);
			try {
				Painter.DrawObject(ViewInfo);
			} finally {
				ViewInfo.Cache = null;
			}
		}
		protected internal void CalcInfo(Graphics g) {
			if(Bounds.IsEmpty) return;
			try {
				ViewInfo.Graphics = g;
				Painter.CalcObjectBounds(ViewInfo);
			} finally {
				ViewInfo.Graphics = null;
			}
		}
		public Size CalcSize(Graphics g) {
			if(!Button.Visible) return Size.Empty;
			Size size = Size.Empty;
			try {
				UpdateViewInfo(g);
				size = Painter.CalcObjectMinBounds(ViewInfo).Size;
				size.Width = Math.Max(14, size.Width);
				size.Height = Math.Max(14, size.Height);
			}
			finally {
				ViewInfo.Graphics = null;
			}
			return size;
		}
		protected virtual void UpdateViewInfo(Graphics g) {
			ViewInfo.Graphics = g;
		}
		public void Invalidate() {
			if(Bounds.IsEmpty || Panel == null) return;
			Panel.Invalidate(Bounds);
		}
		protected TabButtonInfoCollection Collection { get { return collection; } }
		protected TabButtonsPanel Panel { get { return Collection == null ? null : Collection.Panel; } }
		internal void SetCollection(TabButtonInfoCollection collection) { 
			this.collection = collection;
		}
	}
	public class TabButtonInfoCollection : CollectionBase {
		TabButtonsPanel panel;
		public TabButtonInfoCollection(TabButtonsPanel panel) {
			this.panel = panel;
		}
		public TabButtonsPanel Panel { get { return panel; } }
		public TabButtonInfo this[int index] { get { return List[index] as TabButtonInfo; } }
		public TabButtonInfo this[Point pt] { 
			get { 
				foreach(TabButtonInfo info in this) {
					if(!info.Bounds.IsEmpty && info.Bounds.Contains(pt)) return info;
				}
				return null;
			} 
		}
		public int Add(EditorButton button) { 
			if(button == null) return -1;
			return Add(new TabButtonInfo(button)); 
		}
		public int Add(TabButtonInfo info) { 
			if(info == null) return -1;
			return List.Add(info); 
		}
		internal int AddRTL(TabButtonInfo info, bool rightToLeft) {
			if(info == null) return -1;
			if(rightToLeft) {
				List.Insert(0, info);
				return 0;
			}
			return Add(info);
		}
		protected override void OnInsertComplete(int position, object item) {
			base.OnInsertComplete(position, item);
			TabButtonInfo info = (TabButtonInfo)item;
			info.SetCollection(this);
		}
		protected override void OnRemoveComplete(int position, object item) {
			base.OnRemoveComplete(position, item);
			TabButtonInfo info = (TabButtonInfo)item;
			info.SetCollection(null);
		}
		public virtual void Draw(GraphicsCache cache) {
			foreach(TabButtonInfo info in this) info.Draw(cache);
		}
		protected virtual void ResetViewInfo() {
			foreach(TabButtonInfo info in this) info.Bounds = Rectangle.Empty;
		}
		protected internal Rectangle CalcViewInfo(Graphics g, Rectangle bounds) {
			if(bounds.IsEmpty) {
				ResetViewInfo();
				return Rectangle.Empty;
			}
			Rectangle res = bounds;
			Size panelSize = CalcSize(g);
			res.Size = panelSize;
			if(Panel.Orientation == TabOrientation.Horizontal) {
				res.Y = bounds.Y + (bounds.Height - panelSize.Height) / 2;
			} else {
				res.X = bounds.X + (bounds.Width - panelSize.Width) / 2;
			}
			Rectangle button = bounds;
			foreach(TabButtonInfo info in this) {
				info.UpdatePaintAppearance(Panel.TabViewInfo);
				info.Bounds = Rectangle.Empty;
				Size bsize = info.CalcSize(g);
				button.Size = bsize;
				if(Panel.Orientation == TabOrientation.Horizontal) {
					if(bsize.Height > bounds.Height || button.X + bsize.Width > bounds.Right) continue;
					button.Height = panelSize.Height;
					button.Y = res.Y;
					info.Bounds = button;
					button.X += bsize.Width;
				} else {
					if(bsize.Width > bounds.Width || button.Y + bsize.Height > bounds.Bottom) continue;
					button.Width = panelSize.Width;
					button.X = res.X;
					info.Bounds = button;
					button.Y += bsize.Height;
				}
				info.CalcInfo(g);
			}
			return res;
		}
		protected internal Size CalcSize(Graphics g) {
			Size size = Size.Empty;
			foreach(TabButtonInfo info in this) {
				Size bsize = info.CalcSize(g);
				if(Panel.Orientation == TabOrientation.Horizontal) {
					size.Width += bsize.Width;
					size.Height = Math.Max(size.Height, bsize.Height);
				} else {
					size.Height += bsize.Height;
					size.Width = Math.Max(size.Width, bsize.Width);
				}
			}
			return size;
		}
	}
}
