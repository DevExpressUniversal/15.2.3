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
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.ButtonsPanelControl;
using DevExpress.XtraTab.Registrator;
using System.Diagnostics;
using DevExpress.LookAndFeel.Helpers;
namespace DevExpress.XtraEditors.ButtonPanel {
	#region ITabButtonInfo
	public interface ITabButtonInfo {
		string Caption { get; }
		Color Color { get; }
		bool Checked { get; }
	}
	#endregion
	#region TabNavigator
	[ToolboxItem(false)]
	public class TabNavigator : ButtonPanelControl {
		#region Fields
		TabNavigatorButtonCollection tabButtonCollectionCore;
		IDXMenuManager menuManagerCore;
		#endregion
		public TabNavigator() {
			ContentAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			EnsureButtonCollection();
			SubscribeEvents();
		}
		#region Properties
		[DefaultValue(ContentAlignment.MiddleLeft), Category("Layout")]
		public new ContentAlignment ContentAlignment {
			get { return ButtonsPanel.ContentAlignment; }
			set { ButtonsPanel.ContentAlignment = value; }
		}
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraEditors.Design.ButtonsPanelControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign,
			typeof(System.Drawing.Design.UITypeEditor)), Category("Buttons"), Localizable(true)]
		public new TabNavigatorButtonCollection Buttons {
			get { return tabButtonCollectionCore; }
		}
		public int FirstVisibleButtonIndex {
			get { return ViewInfo != null ? ViewInfo.FirstVisibleButtonIndex : 0; }
			set {
				if (ViewInfo != null)
					ViewInfo.FirstVisibleButtonIndex = value;
			}
		}
		public int LastVisibleButtonIndex {
			get { return ViewInfo != null ? ViewInfo.LastVisibleButtonIndex : 0; }
		}
		public SelectedButtonCollection SelectedButtons { get { return ButtonsPanel != null && ButtonsPanel is TabNavigationButtonsPanel ? (ButtonsPanel as TabNavigationButtonsPanel).SelectedButtons : null; } }
		public TabNavigationButtonsPanel TabNavigationButtonsPanel { get { return ButtonsPanel as TabNavigationButtonsPanel; } }
		public virtual bool ForbidOperation { get { return false; } }
		[DefaultValue(null)]
		public IDXMenuManager MenuManager {
			get { return menuManagerCore; }
			set { menuManagerCore = value; }
		}
		#endregion
		#region Events
		#region RenameWorksheet
		RenameWorksheetEventHandler onRenameWorksheet;
		public event RenameWorksheetEventHandler RenameWorksheet { add { onRenameWorksheet += value; } remove { onRenameWorksheet -= value; } }
		protected virtual void RaiseWorksheetRename(WorksheetEventArgs e) {
			if (onRenameWorksheet != null)
				onRenameWorksheet(this, e);
		}
		#endregion
		#region AddWorksheet
		AddWorksheetEventHandler onAddWorksheet;
		public event AddWorksheetEventHandler AddWorksheet { add { onAddWorksheet += value; } remove { onAddWorksheet -= value; } }
		protected virtual void RaiseAddWorksheet(WorksheetEventArgs e) {
			if (onAddWorksheet != null)
				onAddWorksheet(this, e);
		}
		#endregion
		#region ActiveButtonChanged
		BaseButtonEventHandler onActiveButtonChanged;
		public event BaseButtonEventHandler ActiveButtonChanged { add { onActiveButtonChanged += value; } remove { onActiveButtonChanged -= value; } }
		protected internal virtual void RaiseActiveButtonChanged(BaseButtonEventArgs args) {
			if (onActiveButtonChanged != null)
				onActiveButtonChanged(this, args);
		}
		#endregion
		#endregion
		protected override UserLookAndFeel CreateLookAndFeel() {
			return new ControlUserLookAndFeel(this);
		}
		protected override void OnPaintBackground(PaintEventArgs pevent) {
			base.OnPaintBackground(pevent);
			pevent.Graphics.FillRectangle(new SolidBrush(LookAndFeelHelper.GetSystemColorEx(LookAndFeel, SystemColors.Control)), ClientRectangle);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeEvents();
				ResetImageCache();
			}
			base.Dispose(disposing);
		}
		protected internal void SubscribeEvents() {
			TabNavigationButtonsPanel.ActiveButtonChanged += OnActiveButtonChanged;
		}
		protected internal void UnsubscribeEvents() {
			TabNavigationButtonsPanel.ActiveButtonChanged -= OnActiveButtonChanged;
		}
		void OnActiveButtonChanged(object sender, BaseButtonEventArgs e) {
			RaiseActiveButtonChanged(e);
		}
		protected internal void ResetImageCache() {
			ColoredTabNavigationButtonCache.Reset();
		}
		protected virtual void EnsureButtonCollection() {
			tabButtonCollectionCore = new TabNavigatorButtonCollection(ButtonsPanel);
			tabButtonCollectionCore.CollectionChanged += ButtonsPanel.OnButtonsCollectionChanged;
			ButtonsPanel.Buttons.AddRange(new IBaseButton[] { 
				new FirstNavigationButton(), 
				new PrevNavigationButton(), 
				new NextNavigationButton(), 
				new LastNavigationButton() });
		}
		protected override void OnLookAndFeelStyleChanged() {
			base.OnLookAndFeelStyleChanged();
			defaultAppearancesCore = null;
			ButtonsPanel.Buttons.ForEach(button => {
				if (button is NewTabButton)
					button.Properties.Glyphs = null;
			});
			ResetImageCache();
			Invalidate();
			Update();
		}
		Hashtable defaultAppearancesCore;
		protected internal Hashtable DefaultAppearances {
			get {
				if (defaultAppearancesCore == null) {
					defaultAppearancesCore = new Hashtable();
					RegisterDefaultAppearances(defaultAppearancesCore);
				}
				return defaultAppearancesCore;
			}
		}
		protected internal virtual void RegisterDefaultAppearances(Hashtable appearances) {
			appearances.Clear();
			Font font = GetSkin()[TabSkins.SkinTabHeader].GetFont(AppearanceObject.DefaultFont);
			appearances[TabPageAppearance.PageHeader] = new FrozenAppearance(new AppearanceDefault(GetSkin().Colors[TabSkinProperties.TabHeaderTextColor], Color.Transparent, font));
			appearances[TabPageAppearance.PageHeaderHotTracked] = new FrozenAppearance(new AppearanceDefault(GetSkin().Colors[TabSkinProperties.TabHeaderTextColorHot], Color.Transparent, font));
			appearances[TabPageAppearance.PageHeaderPressed] = new FrozenAppearance(new AppearanceDefault(GetSkin().Colors[TabSkinProperties.TabHeaderTextColorActive], Color.Transparent, font));
		}
		public Skin GetSkin() { return TabSkins.GetSkin(LookAndFeel); }
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
		}
		public void EnsureButtons(IEnumerable<ITabButtonInfo> infos) {
			Buttons.BeginUpdate();
			try {
				Buttons.Clear();
				foreach (ITabButtonInfo info in infos) {
					CreateTabNavigationButton(info);
				}
				Buttons.Add(new NewTabButton());
				ButtonsPanel.Buttons.Merge(Buttons, false);
			}
			finally {
				Buttons.EndUpdate();
			}
		}
		protected virtual void CreateTabNavigationButton(ITabButtonInfo info) {
			Buttons.Add(CreateTabButtons(info));
		}
		protected virtual TabNavigationButton CreateTabButtons(ITabButtonInfo info) {
			TabNavigationButton result = new TabNavigationButton(info.Caption);
			result.Color = info.Color;
			result.Checked = info.Checked;
			return result;
		}
		protected override void DrawButtonsPanel(GraphicsCache cache) {
			Color color = GetSkin()[TabSkins.SkinTabHeaderLine].Color.GetBackColor();
			cache.Graphics.DrawLine(new Pen(color), ButtonsPanel.Bounds.Location, new Point(ClientRectangle.Right, ButtonsPanel.Bounds.Top));
			base.DrawButtonsPanel(cache);
			cache.Graphics.DrawLine(new Pen(color), new Point(ClientRectangle.Right - 1, ButtonsPanel.Bounds.Top), new Point(ClientRectangle.Right - 1, ButtonsPanel.Bounds.Bottom - 1));
		}
		public override ObjectPainter GetPainter() {
			return new TabNavigationButtonsPanelSkinPainter(LookAndFeel);
		}
		protected override DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControl CreateButtonsPanel() {
			return new TabNavigationButtonsPanel(this);
		}
		protected override void RaiseButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e) {
			if (e.Button is NewTabButton) {
				if (ForbidOperation)
					return;
				WorksheetEventArgs args = new WorksheetEventArgs(null);
				RaiseAddWorksheet(args);
			}
			if (e.Button is BaseNavigationButton) {
				if (e.Button is NextNavigationButton)
					MoveNext();
				if (e.Button is PrevNavigationButton)
					MovePrev();
				if (e.Button is FirstNavigationButton)
					MoveFirst();
				if (e.Button is LastNavigationButton)
					MoveLast();
				InvalidateViewInfo();
			}
			base.RaiseButtonClick(sender, e);
		}
		protected virtual void MoveLast() {
			int lastButtonIndex = Buttons.Count - 1;
			FirstVisibleButtonIndex = CalculateFirstVisibleButtonIndex(lastButtonIndex);
		}
		protected virtual void MoveFirst() {
			FirstVisibleButtonIndex = 0;
		}
		protected virtual void MovePrev() {
			FirstVisibleButtonIndex--;
		}
		protected virtual void MoveNext() {
			FirstVisibleButtonIndex++;
		}
		protected internal void MakeVisibleButton(string caption) {
			if (ClientRectangle.IsEmpty)
				return;
			TabNavigationButton button = Buttons[caption];
			if (button == null)
				return;
			int firstVisibleButtonIndex = FirstVisibleButtonIndex;
			int buttonIndex = Buttons.IndexOf(button);
			if (buttonIndex >= firstVisibleButtonIndex && buttonIndex <= LastVisibleButtonIndex)
				return;
			FirstVisibleButtonIndex = buttonIndex < firstVisibleButtonIndex ? buttonIndex : CalculateFirstVisibleButtonIndex(buttonIndex);
			InvalidateViewInfo();
		}
		void InvalidateViewInfo() {
			ButtonsPanel.ViewInfo.SetDirty();
		}
		int CalculateFirstVisibleButtonIndex(int necessaryLastVisibleButtonIndex) {
			Rectangle clientBounds = CalcClientRectangle(ClientRectangle);
			return ViewInfo.CalculateFirstVisibleButtonIndex(clientBounds, necessaryLastVisibleButtonIndex);
		}
		protected TabNavigationButtonsPanelViewInfo ViewInfo { get { return ButtonsPanel.ViewInfo as TabNavigationButtonsPanelViewInfo; } }
		protected override void CalcButtonsPanel(Graphics g) {
			if (ButtonsPanel != null)
				ViewInfo.Calc(g, CalcClientRectangle(ClientRectangle));
			CalcButtonsVisibility();
		}
		void CalcButtonsVisibility() {
			if (ButtonsPanel.Buttons.Count == 0)
				return;
			BeginUpdate();
			try {
				bool isFirstButtonHidden = FirstVisibleButtonIndex > 0 && Enabled;
				ButtonsPanel.Buttons[0].Properties.Enabled = isFirstButtonHidden;
				ButtonsPanel.Buttons[1].Properties.Enabled = isFirstButtonHidden;
				bool isLastButtonHidden = !ViewInfo.IsLastTabButtonVisible && FirstVisibleButtonIndex != ViewInfo.TabButtonsCount - 1 && Enabled;
				ButtonsPanel.Buttons[2].Properties.Enabled = isLastButtonHidden;
				ButtonsPanel.Buttons[3].Properties.Enabled = isLastButtonHidden;
			}
			finally {
				CancelUpdate();
			}
		}
		protected override void OnMouseDoubleClick(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			if (e.Button != MouseButtons.Left)
				return;
			BaseButtonInfo hitInfo = ButtonsPanel.ViewInfo.CalcHitInfo(e.Location);
			if (hitInfo == null || !(hitInfo.Button is TabNavigationButton) || hitInfo.Button is NewTabButton)
				return;
			WorksheetEventArgs args = new WorksheetEventArgs(hitInfo.Button.Properties.Caption);
			RaiseWorksheetRename(args);
		}
		const int WM_CONTEXTMENU = 0x007B;
		protected override void WndProc(ref Message m) {
			if (m.Msg == WM_CONTEXTMENU) {
				ShowContextMenuCore(ref m);
			}
			base.WndProc(ref m);
		}
		protected virtual void ShowContextMenuCore(ref Message m) {
			Point clientPoint = PointToClient(new Point(GetInt(m.LParam)));
			BaseButtonInfo hitInfo = ButtonsPanel.ViewInfo.CalcHitInfo(clientPoint);
			if (hitInfo == null)
				return;
			IBaseButton button = hitInfo.Button;
			if (button is ITabNavigationButton && !(button is NewTabButton)) {
				RaiseActiveButtonChanged(new BaseButtonEventArgs(button));
				OnPopupMenu(clientPoint);
				m.Result = new IntPtr(1);
			}
		}
		protected internal virtual void OnPopupMenu(Point point) {
		}
		public static int GetInt(IntPtr ptr) {
			return IntPtr.Size == 8 ? (int)ptr.ToInt64() : ptr.ToInt32();
		}
		protected internal void SelectActiveButton(string name) {
			IBaseButton button = Buttons[name];
			if (button == null)
				return;
			ButtonsPanel.CheckButtonGroupIndex(button);
			button.Properties.Checked = true;
			TabNavigationButton tabButton = button as TabNavigationButton;
			if (tabButton != null && SelectedButtons.Contains(tabButton))
				SelectedButtons.RemoveCore(tabButton);
		}
		protected internal void BeginUpdate() {
			ButtonsPanel.BeginUpdate();
		}
		protected internal void EndUpdate() {
			ButtonsPanel.EndUpdate();
		}
		protected internal void CancelUpdate() {
			ButtonsPanel.CancelUpdate();
		}
	}
	#endregion
	#region Buttons
	public class BaseNavigationButton : ButtonPanel.BaseButton, IDefaultButton {
		public BaseNavigationButton() {
			Visible = true;
		}
		public override bool UseCaption { get { return false; } }
	}
	public class NextNavigationButton : BaseNavigationButton {
		public override int ImageIndex { get { return 3; } }
	}
	public class PrevNavigationButton : BaseNavigationButton {
		public override int ImageIndex { get { return 2; } }
	}
	public class FirstNavigationButton : BaseNavigationButton {
		public override int ImageIndex { get { return 0; } }
	}
	public class LastNavigationButton : BaseNavigationButton {
		public override int ImageIndex { get { return 5; } }
	}
	public interface ITabNavigationButton {
		Padding Margins { get; set; }
	}
	public class TabNavigationButton : ButtonPanel.BaseButton, ITabNavigationButton {
		#region Fields
		public static int MainButtonGroupIndex = 1;
		Padding margins;
		Color color;
		#endregion
		public TabNavigationButton() { }
		public TabNavigationButton(string name) {
			Caption = name;
			margins = new Padding(0, 0, -1, 0);
		}
		#region Properties
		public override DevExpress.XtraBars.Docking2010.ButtonStyle Style { get { return DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton; } }
		public override int GroupIndex { get { return MainButtonGroupIndex; } }
		public Padding Margins {
			get { return margins; }
			set { SetValue<Padding>(ref margins, value, "Margins"); }
		}
		public Color Color { get { return color; } set { color = value; } }
		#endregion
	}
	public class NewTabButton : TabNavigationButton {
		public override bool UseCaption { get { return true; } }
		public override DevExpress.XtraBars.Docking2010.ButtonStyle Style { get { return DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton; } }
		public override int GroupIndex { get { return -1; } }
		public override string Caption { get { return string.Empty; } }
		object glyphsCore;
		public override object Glyphs {
			get {
				if (glyphsCore == null) {
					UserLookAndFeel lookAndFeel = ((TabNavigator)GetOwner().Owner).LookAndFeel;
					if (lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
						SkinElement element = SpreadsheetSkins.GetSkin(lookAndFeel)[SpreadsheetSkins.SkinAddNewWorksheetButton];
						if (element.Image != null)
							glyphsCore = element.Image.GetImages();
					}
				}
				return glyphsCore;
			}
			set { glyphsCore = value; }
		}
	}
	#endregion
	#region TabNavigationButtonsPanel
	public class TabNavigationButtonsPanel : DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControl, IButtonsPanel {
		#region Fields
		SelectedButtonCollection selectedButtonsCore;
		#endregion
		public TabNavigationButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
		}
		#region Properties
		object IButtonsPanel.Images { get { return GetDefaultImages(); } }
		protected internal SelectedButtonCollection SelectedButtons {
			get {
				if (selectedButtonsCore == null)
					selectedButtonsCore = new SelectedButtonCollection();
				return selectedButtonsCore;
			}
		}
		#endregion
		#region Events
		#region ActiveButtonChanged
		BaseButtonEventHandler onActiveButtonChanged;
		public event BaseButtonEventHandler ActiveButtonChanged { add { onActiveButtonChanged += value; } remove { onActiveButtonChanged -= value; } }
		protected internal virtual void RaiseActiveButtonChanged(BaseButtonEventArgs args) {
			if (onActiveButtonChanged != null)
				onActiveButtonChanged(this, args);
		}
		#endregion
		#endregion
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new TabNavigationButtonsPanelViewInfo(this); ;
		}
		protected override IButtonsPanelHandler CreateHandler() {
			return new TabNavigationButtonsPanelHandler(this);
		}
		protected internal virtual object GetDefaultImages() {
			if (((TabNavigator)Owner).LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = EditorsSkins.GetSkin(((TabNavigator)Owner).LookAndFeel)[EditorsSkins.SkinNavigator];
				if (element.Image != null) return element.Image.GetImages();
			}
			return null;
		}
		void IButtonsPanel.PerformClick(IBaseButton button) {
			IButtonProperties properties = button.Properties;
			TabNavigationButton navigationButton = button as TabNavigationButton;
			if (Control.ModifierKeys == Keys.Control && navigationButton != null && navigationButton.GroupIndex == TabNavigationButton.MainButtonGroupIndex) {
				if (properties.Checked || ((TabNavigator)Owner).ForbidOperation)
					return;
				if (SelectedButtons.Contains(navigationButton))
					SelectedButtons.Remove(navigationButton);
				else
					SelectedButtons.Add(navigationButton);
			}
			else {
				if (Buttons.Contains(button)) {
					if (properties.Style == ButtonStyle.CheckButton && properties.Checked != CalcNewCheckedState(button)) {
						RaiseActiveButtonChanged(new BaseButtonEventArgs(button));
						SelectedButtons.Clear();
					}
					else RaiseButtonClick(button);
				}
			}
		}
	}
	#endregion
	#region SelectedButtonCollection
	public class SelectedButtonCollection {
		#region Fields
		List<TabNavigationButton> innerList;
		#endregion
		public SelectedButtonCollection() {
			this.innerList = new List<TabNavigationButton>();
		}
		#region Properties
		public int Count { get { return innerList.Count; } }
		public TabNavigationButton this[int index] { get { return innerList[index]; } }
		public TabNavigationButton First { get { return Count > 0 ? innerList[0] : null; } }
		public TabNavigationButton Last { get { return Count > 0 ? innerList[Count - 1] : null; } }
		#endregion
		#region Events
		#region ButtonInserted
		SelectedButtonEventHandler onButtonInserted;
		public event SelectedButtonEventHandler ButtonInserted { add { onButtonInserted += value; } remove { onButtonInserted -= value; } }
		protected internal virtual void RaiseButtonInserted(string sheetName) {
			if (onButtonInserted != null) {
				WorksheetEventArgs args = new WorksheetEventArgs(sheetName);
				onButtonInserted(this, args);
			}
		}
		#endregion
		#region ButtonRemoved
		SelectedButtonEventHandler onButtonRemoved;
		public event SelectedButtonEventHandler ButtonRemoved { add { onButtonRemoved += value; } remove { onButtonRemoved -= value; } }
		protected internal virtual void RaiseButtonRemoved(string sheetName) {
			if (onButtonRemoved != null) {
				WorksheetEventArgs args = new WorksheetEventArgs(sheetName);
				onButtonRemoved(this, args);
			}
		}
		#endregion
		#endregion
		public virtual void Add(TabNavigationButton item) {
			Guard.ArgumentNotNull(item, "Item");
			this.innerList.Add(item);
			RaiseButtonInserted(item.Caption);
		}
		public virtual void Insert(int index, TabNavigationButton item) {
			innerList.Insert(index, item);
			RaiseButtonInserted(item.Caption);
		}
		public virtual void Remove(TabNavigationButton item) {
			RemoveCore(item);
			RaiseButtonRemoved(item.Caption);
		}
		protected internal void RemoveCore(TabNavigationButton item) {
			innerList.Remove(item);
		}
		public virtual void RemoveAt(int index) {
			string sheetName = innerList[index].Caption;
			RemoveAtCore(index);
			RaiseButtonRemoved(sheetName);
		}
		protected internal virtual void RemoveAtCore(int index) {
			innerList.RemoveAt(index);
		}
		public virtual void Clear() {
			this.innerList.Clear();
		}
		public bool Contains(TabNavigationButton item) {
			return this.innerList.Contains(item);
		}
		public int IndexOf(TabNavigationButton item) {
			return innerList.IndexOf(item);
		}
		public void ForEach(Action<TabNavigationButton> action) {
			innerList.ForEach(action);
		}
		public virtual IEnumerator<TabNavigationButton> GetEnumerator() {
			return innerList.GetEnumerator();
		}
	}
	#endregion
	#region TabNavigationButtonInfo
	public class TabNavigationButtonInfo : BaseButtonControlInfo {
		public TabNavigationButtonInfo(IBaseButton button)
			: base(button) {
		}
		protected override AppearanceObject GetStateAppearance(ObjectState state) {
			TabNavigator navigator = ButtonPanelOwner as TabNavigator;
			if (navigator == null)
				return null;
			if ((state & ObjectState.Pressed) != 0)
				return navigator.DefaultAppearances[TabPageAppearance.PageHeaderPressed] as AppearanceObject;
			if ((state & ObjectState.Hot) != 0)
				return navigator.DefaultAppearances[TabPageAppearance.PageHeaderHotTracked] as AppearanceObject;
			return navigator.DefaultAppearances[TabPageAppearance.PageHeader] as AppearanceObject;
		}
	}
	#endregion
	#region TabNavigationButtonsPanelViewInfo
	public class TabNavigationButtonsPanelViewInfo : ButtonsPanelControlViewInfo {
		#region Fields
		int firstVisibleButtonIndex;
		int lastVisibleButtonIndex;
		bool lastTabButtonVisible;
		#endregion
		public TabNavigationButtonsPanelViewInfo(IButtonsPanel panel)
			: base(panel) {
		}
		public int FirstVisibleButtonIndex {
			get { return firstVisibleButtonIndex; }
			set {
				if (value < 0) { firstVisibleButtonIndex = 0; return; }
				firstVisibleButtonIndex = value;
				if (firstVisibleButtonIndex >= TabButtonsCount && firstVisibleButtonIndex != 0)
					firstVisibleButtonIndex = TabButtonsCount - 1;
			}
		}
		public int LastVisibleButtonIndex { get { return lastVisibleButtonIndex; } }
		protected internal bool IsLastTabButtonVisible { get { return lastTabButtonVisible; } }
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			return new TabNavigationButtonInfo(button);
		}
		protected override Point CalcButtonInfos(Graphics g, BaseButtonPainter buttonPainter, int interval, bool horz, IEnumerable<BaseButtonInfo> buttons, Point offset) {
			Padding buttonMargins = Padding.Empty;
			foreach (BaseButtonInfo buttonInfo in buttons) {
				ITabNavigationButton tabNavigationButton = buttonInfo.Button as ITabNavigationButton;
				if (tabNavigationButton != null) buttonMargins = tabNavigationButton.Margins;
				if (horz) {
					offset.X += buttonMargins.Left;
					buttonInfo.Calc(g, buttonPainter, offset, new Rectangle(Content.Location, new Size(Content.Width, MinSize.Height)), horz);
					offset.X += (buttonInfo.Bounds.Width + interval) + buttonMargins.Right;
				}
				else {
					buttonInfo.Calc(g, buttonPainter, offset, new Rectangle(Content.Location, new Size(MinSize.Width, Content.Height)), horz);
					offset.Y += (buttonInfo.Bounds.Height + interval);
				}
			}
			return offset;
		}
		protected int FirstTabButtonIndex {
			get {
				IBaseButton button = Panel.Buttons.FindFirst((btn) => (btn is TabNavigationButton));
				if (button != null)
					return Panel.Buttons.IndexOf(button);
				return 0;
			}
		}
		public int TabButtonsCount {
			get {
				int i = 0;
				foreach (IBaseButton button in Panel.Buttons) {
					if (button is TabNavigationButton)
						i++;
				}
				return i;
			}
		}
		protected override IBaseButton[] SortButtonList(BaseButtonCollection buttons) {
			List<IBaseButton> positiveList = new List<IBaseButton>();
			List<IBaseButton> negativeList = new List<IBaseButton>();
			foreach (IBaseButton button in buttons) {
				if (button.Properties.VisibleIndex < 0)
					negativeList.Add(button);
				else
					positiveList.Add(button);
			}
			SortButtonListCore(positiveList);
			positiveList.AddRange(negativeList);
			IBaseButton newTabButton = positiveList.FirstOrDefault((button) => (button is NewTabButton));
			if (newTabButton != null) {
				positiveList.Remove(newTabButton);
				positiveList.Add(newTabButton);
			}
			return positiveList.ToArray();
		}
		public override Size CalcMinSize(Graphics g) {
			base.CalcMinSize(g);
			if (Panel != null && Panel.Owner != null && (Panel.Owner as TabNavigator).Bounds.Height != MinSize.Height)
				MinSize = new Size(MinSize.Width, (Panel.Owner as TabNavigator).Bounds.Height);
			return MinSize;
		}
		protected override void CalcButtonsCore(Graphics g, BaseButtonPainter buttonPainter, bool horz, Rectangle maxRect, ref int width, ref int height, IList<BaseButtonInfo> buttonInfos, IBaseButton[] buttons) {
			int maxSize = horz ? maxRect.Width : maxRect.Height;
			Size buttonPanelMinSize = CalcMinSize(g);
			width = 0;
			height = 0;
			buttonInfos.Clear();
			for (int i = 0; i < buttons.Length; i++) {
				IBaseButton button = buttons[i];
				if (i >= FirstTabButtonIndex && i < FirstTabButtonIndex + FirstVisibleButtonIndex) continue;
				BaseButtonInfo buttonInfo = CreateButtonInfo(button);
				Size buttonSize = buttonInfo.CalcMinSize(g, buttonPainter);
				if (buttonSize.Height == 0 || buttonSize.Width == 0)
					continue;
				int interval = (buttonInfos.Count == 0) ? 0 : Panel.ButtonInterval;
				if (horz) {
					width += (buttonSize.Width + interval);
					buttonInfos.Add(buttonInfo);
					height = Math.Max(buttonPanelMinSize.Height, height);
				}
				else {
					height += (buttonSize.Height + interval);
					buttonInfos.Add(buttonInfo);
					width = Math.Max(buttonPanelMinSize.Width, width);
				}
				if (width - interval <= maxSize)
					this.lastVisibleButtonIndex = i;
			}
			this.lastVisibleButtonIndex -= FirstTabButtonIndex;
			lastTabButtonVisible = width <= maxSize;
		}
		protected internal int CalculateFirstVisibleButtonIndex(Rectangle bounds, int necessaryLastVisibleButtonIndex) {
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero)) {
				BaseButtonsPanelPainter painter = Panel.Owner.GetPainter() as BaseButtonsPanelPainter;
				return CalculateFirstVisibleButtonIndexCore(graphics, painter.GetButtonPainter(), bounds, SortButtonList(Panel.Buttons), necessaryLastVisibleButtonIndex);
			}
		}
		protected internal int CalculateFirstVisibleButtonIndexCore(Graphics graphics, BaseButtonPainter buttonPainter, Rectangle maxRect, IBaseButton[] buttons, int necessaryLastVisibleButtonIndex) {
			int maxLength = Panel.IsHorizontal ? maxRect.Width : maxRect.Height;
			int navigationButtonsLength = 0;
			int firstTabButtonIndex = FirstTabButtonIndex;
			for (int i = 0; i < firstTabButtonIndex; i++) {
				int buttonLength = GetButtonLength(graphics, buttonPainter, buttons[i]);
				int interval = i > 0 ? Panel.ButtonInterval : 0;
				navigationButtonsLength += (buttonLength + interval);
			}
			maxLength -= navigationButtonsLength;
			int length = 0;
			int startIndex = necessaryLastVisibleButtonIndex + 4; 
			int lastVisibleButtonIndex = startIndex;
			for (int i = startIndex; i >= firstTabButtonIndex; i--) {
				int buttonLength = GetButtonLength(graphics, buttonPainter, buttons[i]);
				if (buttonLength == 0)
					continue;
				int interval = i == startIndex ? 0 : Panel.ButtonInterval;
				length += (buttonLength + interval);
				if (length <= maxLength)
					lastVisibleButtonIndex = i;
			}
			lastVisibleButtonIndex -= firstTabButtonIndex;
			return firstVisibleButtonIndex > lastVisibleButtonIndex ? firstVisibleButtonIndex : lastVisibleButtonIndex;
		}
		int GetButtonLength(Graphics g, BaseButtonPainter buttonPainter, IBaseButton button) {
			BaseButtonInfo buttonInfo = CreateButtonInfo(button);
			Size buttonSize = buttonInfo.CalcMinSize(g, buttonPainter);
			return Panel.IsHorizontal ? buttonSize.Width : buttonSize.Height;
		}
	}
	#endregion
	#region TabNavigationButtonSkinPainter
	public class TabNavigationButtonSkinPainter : ButtonControlSkinPainter {
		public TabNavigationButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override Image GetButtonGlyphs() {
			if (!(Info.Button is TabNavigationButton)) return null;
			return base.GetButtonGlyphs();
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			Rectangle baseBounds = base.CalcBoundsByClientRectangle(e, client);
			baseBounds.Height = client.Height;
			if ((e as BaseButtonInfo).Button is BaseNavigationButton) {
				baseBounds.Inflate(4, 0);
			}
			return baseBounds;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			Rectangle baseBounds = base.GetObjectClientRectangle(e);
			baseBounds.Height = e.Bounds.Height;
			baseBounds.Y = e.Bounds.Location.Y;
			if ((e as BaseButtonInfo).Button is BaseNavigationButton) {
				baseBounds.Inflate(-4, 0);
			}
			return baseBounds;
		}
		protected override void DrawBorder(GraphicsCache cache, BaseButtonInfo info) {
			if (info.Button.IsChecked.HasValue && info.Button.IsChecked.Value) return;
			Color color = TabSkins.GetSkin(SkinProvider)[TabSkins.SkinTabHeaderLine].Color.GetBackColor();
			cache.Graphics.DrawLine(new Pen(color), info.Bounds.Location, new Point(info.Bounds.Right, info.Bounds.Top));
		}
		protected override void DrawStandartBackground(GraphicsCache cache, BaseButtonInfo info) {
			if (info.Button is BaseNavigationButton) {
				DrawNavigationButtonBackground(cache, info);
				return;
			}
			DrawStandartBackgroundCore(cache, info);
			DrawBorder(cache, info);
		}
		void DrawStandartBackgroundCore(GraphicsCache cache, BaseButtonInfo info) {
			BitmapRotate.CreateBufferBitmap(info.Bounds.Size, true);
			Rectangle saveBounds = info.Bounds;
			try {
				info.Bounds = new Rectangle(0, 0, info.Bounds.Width, info.Bounds.Height);
				TabNavigationButtonSkinElementInfo elementInfo = new TabNavigationButtonSkinElementInfo(GetBackground(), info.Bounds);
				elementInfo.ImageIndex = -1;
				elementInfo.State = info.State;
				ObjectPainter.DrawObject(BitmapRotate.BufferCache, SkinElementPainter.Default, elementInfo);
				DrawColoredTabHeader(BitmapRotate.BufferCache, Info.Button, info);
			}
			finally {
				info.Bounds = saveBounds;
			}
			BitmapRotate.RotateBitmap(RotateFlipType.Rotate180FlipX);
			info.Paint.DrawImage(cache.Graphics, BitmapRotate.BufferBitmap, info.Bounds, new Rectangle(Point.Empty, info.Bounds.Size), true);
			BitmapRotate.RestoreBitmap(RotateFlipType.Rotate180FlipX);
		}
		void DrawColoredTabHeader(GraphicsCache cache, IBaseButton button, BaseButtonInfo info) {
			TabNavigationButton tabButton = button as TabNavigationButton;
			if (tabButton == null || DXColor.IsTransparentOrEmpty(tabButton.Color))
				return;
			TabNavigationButtonSkinElementInfo coloredTabInfo = new TabNavigationButtonSkinElementInfo(GetColoredTabHeader(), info.Bounds);
			coloredTabInfo.Color = tabButton.Color;
			coloredTabInfo.ImageIndex = -1;
			coloredTabInfo.State = info.State;
			Image image = ColoredTabNavigationButtonCache.GetTabNavigationButtonImage(coloredTabInfo, SkinProvider);
			DevExpress.XtraTab.ColoredTabSkinElementPainter.Draw(BitmapRotate.BufferCache, coloredTabInfo, image);
		}
		protected virtual void DrawNavigationButtonBackground(GraphicsCache cache, BaseButtonInfo info) {
			Color color = LookAndFeelHelper.GetSystemColorEx(SkinProvider, SystemColors.Window);
			cache.FillRectangle(new SolidBrush(color), info.Bounds);
			base.DrawStandartBackground(cache, info);
			DrawBorder(cache, info);
		}
		protected override SkinElement GetBackground() {
			return GetBackground(Info);
		}
		protected override SkinElement GetBackground(ObjectInfoArgs e) {
			BaseButtonInfo info = e as BaseButtonInfo;
			if (info == null)
				return null;
			IBaseButton button = info.Button;
			if (button is BaseNavigationButton)
				return GetSkin(e)[EditorsSkins.SkinNavigatorButton];
			return GetTabHeader(button);
		}
		protected internal SkinElement GetTabHeader(IBaseButton button) {
			SkinElement element = element = SpreadsheetSkins.GetSkin(SkinProvider)[SpreadsheetSkins.SkinTabHeader];
			if (element != null)
				return element;
			return TabSkins.GetSkin(SkinProvider)[TabSkins.SkinTabHeader];
		}
		SkinElement GetColoredTabHeader() {
			return SpreadsheetSkins.GetSkin(SkinProvider)[SpreadsheetSkins.SkinColoredTabHeader];
		}
		protected override Skin GetSkin() {
			return GetSkin(Info);
		}
		protected override Skin GetSkin(ObjectInfoArgs e) {
			return EditorsSkins.GetSkin(SkinProvider);
		}
	}
	#endregion
	#region TabNavigationButtonsPanelSkinPainter
	public class TabNavigationButtonsPanelSkinPainter : BaseButtonsPanelSkinPainter {
		public TabNavigationButtonsPanelSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new TabNavigationButtonSkinPainter(Provider);
		}
		protected override void DrawButtons(GraphicsCache cache, IButtonsPanelViewInfo info) {
			BaseButtonPainter painter = GetButtonPainter();
			if (info.Buttons == null) return;
			for (int i = info.Buttons.Count - 1; i > -1; i--) {
				BaseButtonInfo button = info.Buttons[i];
				button.State = CalcButtonState(button.Button, info.Panel);
				ObjectPainter.DrawObject(cache, painter, button);
			}
			IEnumerable<BaseButtonInfo> pressedInfos = info.Buttons.Where((buttonInfo) => ((buttonInfo.State & ObjectState.Pressed) != 0));
			foreach (var item in pressedInfos) {
				ObjectPainter.DrawObject(cache, painter, item);
			}
		}
		public override ObjectState CalcButtonState(IBaseButton button, IButtonsPanel panel) {
			ObjectState state = ObjectState.Normal;
			TabNavigationButtonsPanel navigator = panel as TabNavigationButtonsPanel;
			if (!button.Properties.Enabled)
				state |= ObjectState.Disabled;
			if (navigator != null && navigator.SelectedButtons.Contains(button as TabNavigationButton)) {
				state |= ObjectState.Pressed;
				return state;
			}
			if (button == panel.Handler.HotButton)
				state |= ObjectState.Hot;
			if (button == panel.Handler.PressedButton)
				state |= ObjectState.Pressed;
			if (button.IsChecked.HasValue && button.IsChecked.Value && !(button is DefaultButton))
				state |= ObjectState.Pressed;
			return state;
		}
	}
	#endregion
	#region TabNavigationButtonsPanelHandler
	public class TabNavigationButtonsPanelHandler : BaseButtonHandler, IButtonsPanelHandler {
		public TabNavigationButtonsPanelHandler(IButtonsPanel panel) {
			Panel = panel;
		}
		public IButtonsPanel Panel { get; private set; }
		protected override void Invalidate() {
			Panel.Owner.Invalidate();
		}
		protected override BaseButtonInfo CalcHitInfo(Point point) {
			return Panel.ViewInfo.CalcHitInfo(point);
		}
		public override void OnMouseDown(MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				BaseButtonInfo hitInfo = CalcHitInfo(e.Location);
				if (hitInfo != null)
					if (hitInfo.Button is TabNavigationButton) {
						PerformClick(hitInfo.Button);
						Reset();
					}
					else base.OnMouseDown(e);
			}
		}
		protected override void PerformClick(IBaseButton button) {
			Panel.PerformClick(button);
		}
	}
	#endregion
	public delegate void RenameWorksheetEventHandler(object sender, WorksheetEventArgs args);
	public delegate void AddWorksheetEventHandler(object sender, WorksheetEventArgs args);
	public delegate void SelectedButtonEventHandler(object sender, WorksheetEventArgs args);
	#region WorksheetEventArgs
	public class WorksheetEventArgs : EventArgs {
		public WorksheetEventArgs(string name) {
			WorksheetName = name;
		}
		public string WorksheetName { get; set; }
	}
	#endregion
	#region ColoredTabNavigationButtonCache
	static class ColoredTabNavigationButtonCache {
		static IDictionary<Color, Image> cache = new Dictionary<Color, Image>();
		public static Image GetTabNavigationButtonImage(TabNavigationButtonSkinElementInfo info, ISkinProvider skinProvider) {
			Color color = info.Color;
			if (DXColor.IsTransparentOrEmpty(color))
				return null;
			return GetCachedImage(info, color, skinProvider);
		}
		static Image GetCachedImage(SkinElementInfo info, Color color, ISkinProvider skinProvider) {
			Image image;
			if (!cache.TryGetValue(color, out image)) {
				image = GetColoredImage(info, color, skinProvider);
				cache.Add(color, image);
			}
			return image;
		}
		static Image GetColoredImage(SkinElementInfo info, Color color, ISkinProvider skinProvider) {
			bool useMultiplyMode = SpreadsheetSkins.GetSkin(skinProvider).Properties.GetBoolean(SpreadsheetSkins.OptColoredTabUseMultiplyMode);
			if (useMultiplyMode)
				return GetColoredImage_BlendModeMultiply(info, color);
			return GetColoredImage_BlendModeColor(info, color);
		}
		static Image GetColoredImage_BlendModeMultiply(SkinElementInfo info, Color color) {
			Bitmap image = CloneImage(info);
			for (int row = 0; row < image.Height; row++) {
				for (int col = 0; col < image.Width; col++) {
					Color c = image.GetPixel(col, row);
					int r = (int)(c.R * color.R / 255.0f);
					int g = (int)(c.G * color.G / 255.0f);
					int b = (int)(c.B * color.B / 255.0f);
					image.SetPixel(col, row, Color.FromArgb(c.A, r, g, b));
				}
			}
			return image;
		}
		static Bitmap CloneImage(SkinElementInfo info) {
			Image source = info.GetActualImage();
			int width = source.Width;
			int height = source.Height;
			Bitmap newImage = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
			using (Graphics graphics = Graphics.FromImage(newImage)) {
				graphics.Clear(Color.Transparent);
				graphics.DrawImage(source, new Rectangle(Point.Empty, newImage.Size), 0, 0, width, height, GraphicsUnit.Pixel);
			}
			return newImage;
		}
		static Image GetColoredImage_BlendModeColor(SkinElementInfo info, Color color) {
			return SkinImageColorizer.GetColoredImage(info.GetActualImage(), color, SkinColorizationMode.Color);
		}
		public static void Reset() {
			foreach (KeyValuePair<Color, Image> pair in cache) {
				if (pair.Value != null)
					pair.Value.Dispose();
			}
			cache.Clear();
		}
		public static void Reset(Color color) {
			ResetCachedImage(cache, color);
		}
		static void ResetCachedImage(IDictionary<Color, Image> cache, Color color) {
			Image image;
			if (cache.TryGetValue(color, out image)) {
				if (image != null) {
					image.Dispose();
					image = null;
				}
				cache.Remove(color);
			}
		}
	}
	#endregion
	#region TabNavigationButtonSkinElementInfo
	public class TabNavigationButtonSkinElementInfo : SkinElementInfo {
		#region Fields
		Color color;
		#endregion
		public TabNavigationButtonSkinElementInfo(SkinElement element)
			: base(element) {
		}
		public TabNavigationButtonSkinElementInfo(SkinElement element, Rectangle bounds)
			: base(element, bounds) {
		}
		#region Properties
		public Color Color { get { return color; } set { color = value; } }
		#endregion
	}
	#endregion
	#region TabNavigatorButtonCollection
	[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
	RefreshProperties(RefreshProperties.All)]
	public class TabNavigatorButtonCollection : ButtonCollectionBase<TabNavigationButton> {
		public TabNavigatorButtonCollection(IButtonsPanel panel)
			: base(panel) {
		}
		protected override string ToStringCore() {
			if (Count == 0) return "None";
			if (Count == 1)
				return string.Concat("{", ((TabNavigationButton)List[0]).Caption, "}");
			return string.Format("Count {0}", Count);
		}
		public TabNavigationButton this[string caption] {
			get {
				foreach (TabNavigationButton button in List) {
					if (button.Caption == caption)
						return button;
				}
				return null;
			}
		}
	}
	#endregion
}
