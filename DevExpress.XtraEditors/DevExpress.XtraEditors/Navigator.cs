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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.NavigatorButtons;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.ViewInfo {
	public abstract class NavigatorObjectViewInfo {
		NavigatorButtonsViewInfo viewInfo;
		public NavigatorObjectViewInfo(NavigatorButtonsViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		public abstract Rectangle Bounds { get; set; }
		public NavigatorButtonsViewInfo ViewInfo { get { return this.viewInfo; } }
		public NavigatorButtonsBase Buttons { get { return ViewInfo.Buttons; } }
		public abstract Size MinSize { get; } 
	}
	public class NavigatorEditorButtonInfoArgs : EditorButtonObjectInfoArgs {
		public NavigatorEditorButtonInfoArgs(EditorButton button, AppearanceObject backStyle) : base(button, backStyle) { }
		Orientation orientation = Orientation.Horizontal;
		public Orientation Orientation { get { return orientation; } set { orientation = value; } }
	}
	public class NavigatorButtonViewInfo : NavigatorObjectViewInfo {
		NavigatorButtonBase button;
		EditorButtonObjectInfoArgs infoArgs;
		EditorButton editorButtonInfo;
		Rectangle bounds;
		public NavigatorButtonViewInfo(NavigatorButtonsViewInfo viewInfo, NavigatorButtonBase button) : base(viewInfo) {
			this.bounds = Rectangle.Empty;
			this.button = button;
			this.editorButtonInfo = new EditorButton(ButtonPredefines.Glyph);
			NavigatorEditorButtonInfoArgs navArgs = new NavigatorEditorButtonInfoArgs(editorButtonInfo, Buttons.Appearance);
			navArgs.Orientation = viewInfo.Orientation;
			this.infoArgs = navArgs;
			this.editorButtonInfo.Enabled = button.ViewInfoEnabled;  
			this.editorButtonInfo.ImageList = (Buttons.ImageList != null && Button.ImageIndex != -1) ? Buttons.ImageList : Buttons.GetDefaultImages();
			this.editorButtonInfo.ImageIndex = Button.ViewInfoImageIndex;
		}
		public override Rectangle Bounds {
			get { return this.bounds; }
			set { this.bounds = value; }
		}
		public NavigatorButtonBase Button { get { return this.button; } }
		public bool Enabled {
			get { return this.editorButtonInfo.Enabled; } 
			set {
				if(Enabled == value) return;
				this.editorButtonInfo.Enabled = value;
				Buttons.InvalidateButton(this);
			}
		}
		public string Hint { get {return Button.Hint; } }
		public int ImageIndex { get { return this.editorButtonInfo.ImageIndex; } }
		public Point ImageLocation {
			get {
				Size images = ImageCollection.GetImageListSize(ImageList);
				Point pt = new Point(Bounds.X + (Bounds.Width - images.Width) / 2,
					Bounds.Y + (Bounds.Height - images.Height) / 2);
				if(State == ObjectState.Pressed)
					pt.Offset(1, 1);
				return pt;
			}
		}
		public object ImageList { get { return this.editorButtonInfo.ImageList; } }
		public override Size MinSize { 
			get { return Painter.CalcObjectMinBounds(InfoArgs).Size; } 
		}
		public ObjectState State { 
			get { 
				if(! Enabled)  
					return ObjectState.Disabled;
				if((this == Buttons.PressedButtonViewInfo) 
					&& ((this == Buttons.HottrackButtonViewInfo) || 
					(this.Button == ViewInfo.FocusedButton)))
					return ObjectState.Pressed;
				if(this == Buttons.HottrackButtonViewInfo) 
					return ObjectState.Hot;
				return ObjectState.Normal; 
			}
		}
		public EditorButtonPainter Painter { get { return Buttons.ViewInfo.ButtonPainter; } }
		public EditorButtonObjectInfoArgs InfoArgs { 
			get {
				UpdateInfoArgs(null);
				return this.infoArgs;
			}
		}
		public void UpdateInfoArgs(GraphicsCache cache) {
			if(this.infoArgs.State != State) OnBeforeButtonStateChanged(infoArgs, State);
			this.infoArgs.Bounds = Bounds;
			this.infoArgs.State = State;
			if(cache != null)
				this.infoArgs.Cache = cache;
			this.infoArgs.DrawFocusRectangle = (Button == ViewInfo.FocusedButton) && 
				(Buttons.Owner.OwnerControl.ContainsFocus || Buttons.Owner.OwnerControl.Capture);
		}
		protected virtual void OnBeforeButtonStateChanged(EditorButtonObjectInfoArgs info, ObjectState state) {
			XtraAnimator.RemoveObject(ViewInfo.OwnerControl as ISupportXtraAnimation, Button);
			if(((info.State | state) & ObjectState.Pressed) != 0) return;
			XtraAnimator.Current.AddBitmapAnimation(ViewInfo.OwnerControl as ISupportXtraAnimation, Button, XtraAnimator.Current.CalcAnimationLength(info.State, state),
				null, new ObjectPaintInfo(Painter, info), new BitmapAnimationImageCallback(OnGetButtonBitmap));
		}
		Bitmap OnGetButtonBitmap(BaseAnimationInfo info) {
			return XtraAnimator.Current.CreateBitmap(Painter, InfoArgs);
		}
	}
	public class NavigatorTextViewInfo : NavigatorObjectViewInfo {
		protected string fText;
		Rectangle bounds;
		public NavigatorTextViewInfo(NavigatorButtonsViewInfo viewInfo) : base(viewInfo) {
			this.bounds = Rectangle.Empty;
			UpdateText();
		}
		public override Rectangle Bounds { get { return this.bounds; } set { this.bounds = value; } }
		public override Size MinSize { 
			get { 
				Size minSize = TextSize; 
				minSize.Width += 4; minSize.Height += 4;
				return minSize;
			} 
		}
		public string Text { get { return this.fText; } }
		public Rectangle DrawTextBounds {
			get {
				Size size = TextSize;
				return new Rectangle(Bounds.Left + (Bounds.Width - size.Width) /2, Bounds.Top + (Bounds.Height - size.Height) /2, size.Width, size.Height);
			}
		}
		public void UpdateText() {
			this.fText = GetText(Buttons.Owner.CurrentRecordIndex, Buttons.Owner.RecordCount);
			if((Bounds != Rectangle.Empty) && (Bounds.Width < MinSize.Width)) 
				Buttons.LayoutChanged();
		}
		protected virtual string TextStringFormat {
			get {
				if(ViewInfo == null || ViewInfo.OwnerControl == null || !(ViewInfo.OwnerControl is NavigatorBase)) 
					return Localizer.Active.GetLocalizedString(StringId.NavigatorTextStringFormat);
				return ((NavigatorBase)ViewInfo.OwnerControl).TextStringFormat;
			}
		}
		protected virtual string GetText(int currentRecord, int count) {
			return string.Format(TextStringFormat, (currentRecord < 0 ? 0 : currentRecord), count);
		}
		string GetWidestText(int count) {
			int n = 1, c = count;
			while(c > 0) { c /= 10; n++; }
			try {
				return string.Format(TextStringFormat, new String('0', n), count);
			} catch {
				return GetText(0, count);
			}
		}
		Size TextSize {
			get {
				Size size = Size.Empty;
				ViewInfo.GInfo.AddGraphics(null);
				try {
					SizeF tsize = Buttons.Appearance.CalcTextSize(ViewInfo.GInfo.Graphics, GetWidestText(Buttons.Owner.RecordCount), 0);
					size.Width = (int)tsize.Width;
					size.Height = (int)tsize.Height;
				} 
				finally {
					ViewInfo.GInfo.ReleaseGraphics();
				}
				return size;
			}
		}
	}		
	[ListBindable(false)]
	public class NavigatorObjectViewInfoCollection : CollectionBase {
		public NavigatorObjectViewInfoCollection(){}
		public NavigatorObjectViewInfo this[int index] { get { return List[index] as  NavigatorObjectViewInfo; } }
		public void Add(NavigatorObjectViewInfo value) { List.Add(value); }
	}
	[ListBindable(false)]
	public class NavigatorButtonViewInfoCollection : CollectionBase {
		public NavigatorButtonViewInfoCollection(){}
		public NavigatorButtonViewInfo this[int index] { get { return List[index] as  NavigatorButtonViewInfo; } }
		public void Add(NavigatorButtonViewInfo value) { List.Add(value); }
		public int IndexOf(NavigatorButtonViewInfo value) {
			return InnerList.IndexOf(value);
		}
	}
	public class NavigatorButtonsViewInfo {
		NavigatorButtonViewInfoCollection buttonCollection;
		NavigatorButtonsBase buttons;
		bool isDirty;
		NavigatorButtonBase focusedButton;
		protected NavigatorTextViewInfo fTextViewInfo;
		GraphicsInfo gInfo;
		public NavigatorButtonsViewInfo(NavigatorButtonsBase buttons){
			this.buttons = buttons;
			this.focusedButton = null;
			buttonCollection = new NavigatorButtonViewInfoCollection();
			Clear();
			this.gInfo = new GraphicsInfo();
		}
		protected internal Orientation Orientation { get { return buttons.Owner.Orientation; } }
		public Rectangle Bounds { get {	return buttons.Owner.Bounds; } }
		public NavigatorButtonViewInfoCollection ButtonCollection { get {return buttonCollection; } }
		public EditorButtonPainter ButtonPainter { get { return Buttons.Owner.ButtonPainter; } }
		public NavigatorButtonsBase Buttons { get {return buttons; } }
		public UserLookAndFeel LookAndFeel { get { return Buttons.Owner.LookAndFeel; } } 
		public NavigatorTextViewInfo TextViewInfo { get { return this.fTextViewInfo; } }
		public GraphicsInfo GInfo { get { return this.gInfo; } }
		public NavigatorButtonViewInfo ButtonViewInfoAt(Point location){
			for(int i = 0; i < ButtonCollection.Count; i ++)
				if(ButtonCollection[i].Bounds.Contains((location)))
					return ButtonCollection[i];
			return null;
		}
		public Control OwnerControl { get { return Buttons != null && Buttons.Owner != null ? Buttons.Owner.OwnerControl : null; } }
		public void Calculate() {
			Size size = buttons.Owner.Bounds.Size;
			CheckSize(ref size);
			isDirty = false;
			if(OwnerControl != null && OwnerControl.IsHandleCreated) 
				Buttons.HottrackButtonViewInfo = ButtonViewInfoAt(OwnerControl.PointToClient(Control.MousePosition));
		}
		void AddViewInfoObject(NavigatorObjectViewInfo viewInfo, NavigatorObjectViewInfoCollection objectCollection, 
			ref int minWidth, ref int minHeight) {
			objectCollection.Add(viewInfo);
			ApplyViewInfoMinSize(viewInfo, ref minWidth, ref minHeight);
		}
		protected virtual void ApplyViewInfoMinSize(NavigatorObjectViewInfo viewInfo, ref int minWidth, ref int minHeight) {
			Size minSize = viewInfo.MinSize;
			minWidth += minSize.Width;
			if(minSize.Height > minHeight)
				minHeight = minSize.Height;
		}
		protected virtual NavigatorTextViewInfo CreateTextViewInfo() {
			return new NavigatorTextViewInfo(this);
		}
		protected void AddTextViewInfo(NavigatorButtonBase button, NavigatorObjectViewInfoCollection objectCollection,
			ref int minWidth, ref int minHeight) {
			if((Buttons.TextLocation == NavigatorButtonsTextLocation.None) 
				|| (TextViewInfo != null)) return;
			if((Buttons.TextLocation == NavigatorButtonsTextLocation.Begin) || (button == null))
				fTextViewInfo = CreateTextViewInfo();
			if((Buttons.TextLocation == NavigatorButtonsTextLocation.Center) 
				&& (button != null) && ((int)button.ButtonType > (int)NavigatorButtonType.Prev))
				fTextViewInfo = CreateTextViewInfo();
			if(TextViewInfo != null)
				AddViewInfoObject(TextViewInfo, objectCollection, ref minWidth, ref minHeight);
		}
		protected virtual void AddButtonViewInfo(NavigatorButtonBase button, NavigatorObjectViewInfoCollection objectCollection,
			ref int minWidth, ref int minHeight) {
			if(button.Visible) {
				AddTextViewInfo(button, objectCollection, ref minWidth, ref minHeight);
				NavigatorButtonViewInfo buttonViewInfo = CreateButtonViewInfo(button);
				ButtonCollection.Add(buttonViewInfo);
				AddViewInfoObject(buttonViewInfo, objectCollection, ref minWidth, ref minHeight);
			}
		}
		protected virtual NavigatorButtonViewInfo CreateButtonViewInfo(NavigatorButtonBase button) {
			return new NavigatorButtonViewInfo(this, button);
		}
		protected ArrayList CreateButtonList() {
			ArrayList list = new ArrayList();
			for(int i = 0; i < Buttons.ButtonCollection.Count; i ++) {
				list.Add(Buttons.ButtonCollection[i]);
			}
			ArrayList customList = new ArrayList();
			for(int i = 0; i < Buttons.CustomButtons.Count; i ++) {
				if(Buttons.CustomButtons[i].Index >= 0)
					customList.Add(Buttons.CustomButtons[i]);
			}
			customList.Sort();
			for(int i = 0; i < customList.Count; i ++) {
				NavigatorCustomButton button = customList[i] as NavigatorCustomButton;
				if(button.Index < list.Count)
					list.Insert(button.Index, button);
				else list.Add(button);
			}
			for(int i = 0; i < Buttons.CustomButtons.Count; i ++) {
				if(Buttons.CustomButtons[i].Index < 0)
					list.Add(Buttons.CustomButtons[i]);
			}
			return list;
		}
		protected bool IsUltraFlatButtonBorder {
			get { return this.Buttons.Owner.IsUltraFlatButtonBorder; } 
		}
		public virtual void CheckSize(ref Size size) {
			Clear();
			NavigatorObjectViewInfoCollection objectCollection = new NavigatorObjectViewInfoCollection();
			int minWidth = 0;
			int minHeight = 0; 
			int difX = 0;
			Rectangle bounds = Bounds;
			ArrayList buttonList = CreateButtonList();
			for (int i = 0; i < buttonList.Count; i ++)
				AddButtonViewInfo(buttonList[i] as NavigatorButtonBase, objectCollection, ref minWidth, ref minHeight);
			AddTextViewInfo(null, objectCollection, ref minWidth, ref minHeight);
			int widthIncCount = 0;
			if(Buttons.Owner.AutoSize) {
				size.Height = minHeight;
				size.Width = minWidth;
			}
			else {
				if(size.Height < minHeight)
					size.Height = minHeight;
				if(size.Width < minWidth)
					size.Width = minWidth;
				else {
					if(objectCollection.Count != 0)
						difX = (size.Width - minWidth) / objectCollection.Count;
					else difX = 0;
					widthIncCount = size.Width - (minWidth + difX * objectCollection.Count);
				}
			}
			int left = ! IsRightToLeft ? bounds.Left : bounds.Right;
			for (int i = 0; i < objectCollection.Count; i ++) {
				int x = ! IsRightToLeft ? left : left - objectCollection[i].MinSize.Width - difX;
				Rectangle r = new Rectangle(x, bounds.Y,
					objectCollection[i].MinSize.Width + difX, size.Height);
				if(widthIncCount-- > 0) {
					if( ! IsRightToLeft)
						r.Width ++;
					else r.X --;
				}
				Rectangle button = r;
				if(IsUltraFlatButtonBorder) {
					button.Inflate(0, 1);
					button.Width ++;
					if(i == 0) { button.X--;button.Width ++; }
				}
				objectCollection[i].Bounds = button;
				left = ! IsRightToLeft ? r.Right : r.Left;
			}
		}
		public void Clear(){
			Buttons.HottrackButtonViewInfo = null;
			ButtonCollection.Clear();
			fTextViewInfo = null;
			isDirty = true;
		}
		public bool IsDirty {
			get { return isDirty; }
			set {
				if(value != isDirty){
					if(! value)
						Calculate();
					else Clear();
					isDirty  = value;
				}
			}
		}
		public bool IsRightToLeft { get { return Buttons.Owner.IsRightToLeft; } }
		public NavigatorButtonBase FocusedButton {
			get {
				if(! Buttons.Owner.TabStop) 
					this.focusedButton = null;
				else {
					if(GetButtonViewInfo(this.focusedButton) == null)
						this.focusedButton = null;
					if(this.focusedButton == null) 
						SetFocusedButtonCore(true);
				}
				return this.focusedButton;
			}
			set {
				if((value == null) || !value.Visible || (value == this.focusedButton)) return;
				NavigatorButtonBase oldFocusedButton = this.focusedButton;
				this.focusedButton = value;
				InvalidateButton(oldFocusedButton);
				InvalidateButton(value);
			}
		}
		public void SetNextFocusedButton() {
			SetFocusedButtonCore(true);
		}
		public void SetPrevFocusedButton() {
			SetFocusedButtonCore(false);
		}
		public virtual void Draw(GraphicsInfoArgs e){
			DrawBackGround(e);
			DrawButtons(e, false);
			DrawButtons(e, true);
			if(TextViewInfo != null)
				DrawText(e);
		}
		protected void DrawButtons(GraphicsInfoArgs e, bool onlyHotPressed) {
			for(int i = 0; i < ButtonCollection.Count; i ++) {
				NavigatorButtonViewInfo button = ButtonCollection[i];
				if(((button.State & (ObjectState.Pressed | ObjectState.Hot)) != 0) != onlyHotPressed) continue;
				if(e.Graphics.IsVisible(ButtonCollection[i].Bounds))
					DrawButton(e, button);
			}
		}
		public virtual void DrawButton(GraphicsInfoArgs e, NavigatorButtonViewInfo bViewInfo){
			bViewInfo.InfoArgs.Cache = e.Cache;
			if(XtraAnimator.Current.DrawFrame(e.Cache, OwnerControl as ISupportXtraAnimation, bViewInfo.Button)) {
				bViewInfo.Painter.DrawTextOnly(bViewInfo.InfoArgs);
				return;
			}
			bViewInfo.Painter.DrawObject(bViewInfo.InfoArgs);
		}
		public virtual void DrawBackGround(GraphicsInfoArgs e){
			e.Graphics.FillRectangle(Buttons.Appearance.GetBackBrush(e.Cache), Bounds);
		}
		public virtual void DrawText(GraphicsInfoArgs e) {
			e.Cache.DrawString(TextViewInfo.Text, Buttons.Appearance.Font, Buttons.Appearance.GetForeBrush(e.Cache), TextViewInfo.DrawTextBounds, Buttons.Appearance.GetStringFormat());
		}
		protected void InvalidateButton(NavigatorButtonViewInfo bViewInfo) {
			if(bViewInfo != null)
				Buttons.InvalidateButton(bViewInfo);
		}
		protected void InvalidateButton(NavigatorButtonBase button) {
			InvalidateButton(GetButtonViewInfo(button));
		}
		public NavigatorButtonViewInfo GetButtonViewInfo(NavigatorButtonBase button) {
			if(button == null) return null;
			for (int i = 0; i < ButtonCollection.Count; i ++) 
				if(ButtonCollection[i].Button == button) 
					return ButtonCollection[i];
			return null;
		}
		private void SetFocusedButtonCore(bool LeftToRightDirection) {
			if(ButtonCollection.Count == 0) return;
			if(!Buttons.Owner.TabStop)
				FocusedButton = null;
			NavigatorButtonViewInfo bViewInfo = GetButtonViewInfo(this.focusedButton);
			int startIndex = bViewInfo != null ? ButtonCollection.IndexOf(bViewInfo) : 0;
			if(LeftToRightDirection) 
				startIndex ++;
			else startIndex --;
			if(startIndex < 0) 
				startIndex = ButtonCollection.Count - 1;
			if(startIndex >= ButtonCollection.Count)
				startIndex = 0;
			FocusedButton = ButtonCollection[startIndex].Button;
		}
	}
}
namespace DevExpress.XtraEditors {
	public interface INavigatorOwner {
		EditorButtonPainter ButtonPainter { get; }
		bool AutoSize { get; }
		bool Visible { get; }
		Rectangle Bounds { get; }
		bool Focused { get; }
		void Focus();
		bool TabStop { get; }
		Control OwnerControl { get; }
		void LayoutChanged();
		UserLookAndFeel LookAndFeel { get; }
		AppearanceObject Appearance { get; }
		BorderStyles ButtonStyle { get; }
		bool DesignMode { get; }
		Orientation Orientation { get; }
		bool IsUltraFlatButtonBorder { get; }
		bool IsRightToLeft { get; }
		int RecordCount { get; }
		int CurrentRecordIndex { get; }
		NavigatorButtonsBase Buttons { get; }
		void OnButtonClick(NavigatorButtonClickEventArgs e);
		void OnNavigatorException(Exception sourceException, NavigatorButton button);
		bool GetValidationCanceled();
	}
	public enum NavigatorButtonType { Custom = 0, First, PrevPage, Prev, Next, NextPage, Last, 
		Append, Remove, Edit, EndEdit, CancelEdit }
	public abstract class NavigatorButtonBase {
		bool enabled, visible;
		string hint;
		int imageIndex;
		object tag;
		public NavigatorButtonBase(int imageIndex, bool enabled, bool visible, string hint, object tag) {
			this.imageIndex = imageIndex;
			this.enabled = enabled;
			this.visible = visible;
			this.hint = hint;
			this.tag = tag;
		}
		[Browsable(false)]
		public virtual NavigatorButtonType ButtonType { get { return NavigatorButtonType.Custom; } }
		protected internal abstract void DoClick();
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorButtonBaseEnabled"),
#endif
 DefaultValue(true)]
		public bool Enabled { 
			get { return this.enabled; }
			set {
				if(this.enabled == value) return;
				this.enabled = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorButtonBaseHint"),
#endif
 DefaultValue(""), Localizable(true)]
		public string Hint {
			get { return this.hint; }
			set { this.hint = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorButtonBaseImageIndex"),
#endif
 DefaultValue(-1),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)), 
		ImageList("ImageList")
		]
		public int ImageIndex {
			get { return this.imageIndex; }
			set {
				if(this.imageIndex == value) return;
				this.imageIndex = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorButtonBaseVisible"),
#endif
 DefaultValue(true)]
		public bool Visible {
			get { return this.visible; }
			set {
				if(Visible == value) return;
				this.visible = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorButtonBaseTag"),
#endif
		DefaultValue(null), 
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public object Tag { get { return tag; } set { tag = value; } }
		[Browsable(false)]
		public object ImageList { 
			get {
				if(Buttons == null) return null;
				return Buttons.ImageList != null ? Buttons.ImageList : Buttons.GetDefaultImages(); 
			}
		}
		[Browsable(false)]
		public virtual bool IsAutoRepeatSupported { get { return false; } }
		protected abstract NavigatorButtonsBase Buttons { get; }
		protected internal virtual bool ViewInfoEnabled { get { return Enabled; } }
		protected internal virtual int ViewInfoImageIndex { get { return ImageIndex; } }
		protected internal virtual string ViewInfoHint { get { return Hint; } }
		protected abstract void LayoutChanged();
	}
	[TypeConverter("System.ComponentModel.ExpandableObjectConverter, System")]
	public class NavigatorButton : NavigatorButtonBase {
		NavigatorButtonHelper helper;
		public NavigatorButton(NavigatorButtonHelper helper) : base(-1, true, true, string.Empty, null) {
			this.helper = helper;
		}
		[Browsable(false)]
		public override NavigatorButtonType ButtonType { get { return Helper.ButtonType; } }
		protected internal override void DoClick() { 
			try {
				Helper.DoClick(); 
			}
			catch(Exception e) {
				if(e is HideException) return;
				this.Helper.Buttons.Owner.OnNavigatorException(e, this);
			}
		}
		protected override NavigatorButtonsBase Buttons { get { return Helper.Buttons; } }
		protected internal override bool ViewInfoEnabled { get { return Enabled && DefaultEnabled; } }
		protected internal override int ViewInfoImageIndex { get { return ImageIndex != -1 ? ImageIndex : DefaultIndex; } }
		protected internal override string ViewInfoHint { get { return Hint != string.Empty ? Hint : DefaultHint; } }
		protected override void LayoutChanged() {
			Buttons.LayoutChanged();
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("NavigatorButtonIsAutoRepeatSupported")]
#endif
		public override bool IsAutoRepeatSupported { get { return Helper.IsAutoRepeatSupported; } }
		protected bool DefaultEnabled { get { return Buttons.Owner.DesignMode || Helper.Enabled ; } }
		protected string DefaultHint { get { return Helper.Hint; } }
		protected int DefaultIndex { get { return Helper.DefaultIndex; } }
		protected NavigatorButtonHelper Helper { get { return this.helper; } }
		[Browsable(false)]
		public bool ShouldSerialize { 
			get {
				return (Visible != true) || (ImageIndex != -1) || (Hint != string.Empty) || (Enabled != true) || (Tag != null);
			}
		}
	}
	public abstract class NavigatorButtonHelper {
		NavigatorButtonsBase buttons;
		public NavigatorButtonHelper(NavigatorButtonsBase buttons) {
			this.buttons = buttons;
		}
		public NavigatorButtonsBase Buttons { get { return this.buttons; } }
		public abstract NavigatorButtonType ButtonType { get; }
		public virtual int DefaultIndex { get { return NavigatorButtonsBase.DefaultIndexByButtonType(ButtonType, Buttons.IsRightToLeft ); } }
		public abstract void DoClick();
		public abstract bool Enabled { get; }
		public virtual bool IsAutoRepeatSupported { get { return "NavigatorButtonType.NextPageNavigatorButtonType.PrevPage".IndexOf(ButtonType.ToString()) > -1; } }
		public virtual string Hint { get { return Localizer.Active.GetLocalizedString(HintStringId); } }
		public virtual StringId HintStringId { get { return GetHintStringIdByButtonType(ButtonType); } }
		protected static StringId GetHintStringIdByButtonType(NavigatorButtonType buttonType) {
			StringId[] stringIds = {StringId.NavigatorFirstButtonHint, StringId.NavigatorPreviousPageButtonHint,
				StringId.NavigatorPreviousButtonHint, StringId.NavigatorNextButtonHint, 
				StringId.NavigatorNextPageButtonHint, StringId.NavigatorLastButtonHint, 
				StringId.NavigatorAppendButtonHint, StringId.NavigatorRemoveButtonHint,
				StringId.NavigatorEditButtonHint, StringId.NavigatorEndEditButtonHint, 
				StringId.NavigatorCancelEditButtonHint};
			int n = (int)buttonType - 1;
			if(n >= stringIds.Length || n < 0) return StringId.None;
			return stringIds[n];  
		}
	}
	[ListBindable(false)]
	public abstract class NavigatorButtonCollectionBase : ReadOnlyCollectionBase {
		public NavigatorButtonCollectionBase(NavigatorButtonsBase buttons){
			CreateButtons(buttons);
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("NavigatorButtonCollectionBaseItem")]
#endif
		public NavigatorButton this[int index] { get { return InnerList[index] as NavigatorButton; } }
		protected abstract void CreateButtons(NavigatorButtonsBase buttons);
		protected virtual void AddButton(NavigatorButtonHelper buttonHelper) {
			InnerList.Add(new NavigatorButton(buttonHelper));
		}
	}
	public enum NavigatorButtonsTextLocation {None, Begin, End, Center};
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverter))]
	public class NavigatorCustomButton : NavigatorButtonBase, IComparable {
		int index;
		NavigatorCustomButtons collection;
		public NavigatorCustomButton() : this(-1, -1, true, true, string.Empty, null) {
		}
		public NavigatorCustomButton(int imageIndex) : this(-1, imageIndex) {}
		public NavigatorCustomButton(int index, int imageIndex) : this(index, imageIndex, string.Empty) {}
		public NavigatorCustomButton(int imageIndex, string hint) : this(-1, imageIndex, hint) {}
		public NavigatorCustomButton(int index, int imageIndex, string hint) : this(index, imageIndex, true, hint) {}
		public NavigatorCustomButton(int index, int imageIndex, bool enabled, string hint) : this(index, imageIndex, enabled, true, hint, null) {}
		public NavigatorCustomButton(int index, int imageIndex, bool enabled, bool visible, string hint, object tag) : 
			base(imageIndex, enabled, visible, hint, tag) {
			this.index = index;
		}
		protected internal override void DoClick() {
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorCustomButtonIndex"),
#endif
 DefaultValue(-1)]
		public int Index { 
			get { return index; } 
			set {
				if(value < -1) value = -1;
				if(value == Index) return;
				index = value; 
				LayoutChanged();
			}
		}
		protected internal NavigatorCustomButtons Collection { get { return collection; } set { collection = value; } }
		protected override NavigatorButtonsBase Buttons { 
			get { 
				if(Collection == null) return null;
				return Collection.Buttons; 
			} 
		}
		protected override void LayoutChanged() {
			if(Collection != null)
				Collection.LayoutChanged();
		}
		int IComparable.CompareTo(object obj) {
			NavigatorCustomButton button = obj as NavigatorCustomButton;
			if(button == null) return -1;
			return Comparer.Default.Compare(Index, button.Index);
		}
	}
	[ListBindable(false)]
	public class NavigatorCustomButtons : CollectionBase {
		NavigatorButtonsBase buttons;
		public NavigatorCustomButtons(NavigatorButtonsBase buttons) {
			this.buttons = buttons;
		}
		public NavigatorCustomButton this[int index] { get {return InnerList[index] as NavigatorCustomButton; } }
		public NavigatorCustomButton Add() {
			NavigatorCustomButton button = new NavigatorCustomButton();
			button.Collection = this;
			InnerList.Add(button);
			LayoutChanged();
			return button;
		}
		public void AddRange(NavigatorCustomButton[] buttons) {
			for(int i = 0; i < buttons.Length; i ++) {
				buttons[i].Collection = this;
				InnerList.Add(buttons[i]);
			}
			LayoutChanged();
		}
		protected internal NavigatorButtonsBase Buttons { get { return buttons; } }
		protected internal void LayoutChanged() {
			if(Buttons != null)
				Buttons.LayoutChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			LayoutChanged();
		}
		protected override void OnInsertComplete(int index, object value) {
			NavigatorCustomButton button = value as NavigatorCustomButton;
			button.Collection = this;
			base.OnInsertComplete(index, value);
			LayoutChanged();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			LayoutChanged();
		}
	}
	public abstract class NavigatorButtonsBase : IImageCollectionHelper {
		const string internalImageResourceName = "DevExpress.XtraEditors.Images.Navigator.png";
		internal const int DefaultPageRecordCount = 10;
		NavigatorButtonsTextLocation textLocation;
		INavigatorOwner owner;
		NavigatorButtonCollectionBase buttonCollection;
		NavigatorCustomButtons customButtons;
		NavigatorButtonsViewInfo viewInfo;
		object imageList;
		[ThreadStatic]
		static ImageCollection defaultImages = null;
		int updateCount;
		NavigatorButtonBase pressedButton;
		NavigatorButtonViewInfo hottrackButtonViewInfo;
		bool enabledAutoRepeat;
		int autoRepeatDelay;
		System.Windows.Forms.Timer clickTimer = null;
		[Browsable(false)]
		public Control OwnerControl {
			get { return Owner.OwnerControl; }
		}
		public NavigatorButtonsBase(INavigatorOwner owner) {
			this.owner = owner;
			this.buttonCollection = CreateNavigatorButtonCollection();
			this.customButtons = new NavigatorCustomButtons(this);
			this.viewInfo = CreateViewInfo();
			this.textLocation = NavigatorButtonsTextLocation.None;
			this.imageList = null;
			this.updateCount = 0;
			this.pressedButton = null;
			this.hottrackButtonViewInfo = null;
			this.enabledAutoRepeat = true;
			this.autoRepeatDelay = 300;
		}
		static object ImagesDefault {
			get { 
				if(defaultImages == null) {
					defaultImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources(
						internalImageResourceName, typeof(NavigatorButtonsBase).Assembly, 
						new Size(11, 11));
				}
				return defaultImages;
			}
		}
		public void BeginUpdate(){
			this.updateCount ++;
		}
		[Browsable(false)]
		public NavigatorButtonCollectionBase ButtonCollection { get { return this.buttonCollection; } }
		public NavigatorButton ButtonByButtonType(NavigatorButtonType type) {
			for(int i = 0; i < ButtonCollection.Count; i ++)
				if(ButtonCollection[i].ButtonType == type)
					return ButtonCollection[i];
			return null;
		}
		public void CheckSize(ref Size size) {
			ViewInfo.CheckSize(ref size);
		}
		[Browsable(false)]
		public object DefaultImageList { get {return GetDefaultImages(); } }
		public static int DefaultIndexByButtonType(NavigatorButtonType type, bool isRightToLeft) {
			if(isRightToLeft) type = RotateButtonType(type);
			return (int)type - (int)NavigatorButtonType.First;
		}
		static NavigatorButtonType RotateButtonType(NavigatorButtonType type) {
			switch(type) {
				case NavigatorButtonType.First: return NavigatorButtonType.Last;
				case NavigatorButtonType.PrevPage: return NavigatorButtonType.NextPage;
				case NavigatorButtonType.Prev: return NavigatorButtonType.Next;
				case NavigatorButtonType.Next: return NavigatorButtonType.Prev;
				case NavigatorButtonType.NextPage: return NavigatorButtonType.PrevPage;
				case NavigatorButtonType.Last: return NavigatorButtonType.First;
			}
			return type;
		}
		public void Draw(GraphicsInfoArgs e){
			ViewInfo.IsDirty = false;
			ViewInfo.Draw(e);
		}
		public void EndUpdate(){
			this.updateCount --;
			if(this.updateCount < 0)
				this.updateCount = 0;
			LayoutChanged();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorButtonsBaseEnabledAutoRepeat"),
#endif
 DefaultValue(true)]
		public bool EnabledAutoRepeat {
			get {
				return this.enabledAutoRepeat;
			}
			set {
				if(enabledAutoRepeat == value) return;
				this.enabledAutoRepeat = value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorButtonsBaseAutoRepeatDelay"),
#endif
 DefaultValue(300)]
		public int AutoRepeatDelay {
			get {
				return this.autoRepeatDelay;
			}
			set {
				if(autoRepeatDelay == value) return;
				this.autoRepeatDelay = value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorButtonsBaseImageList"),
#endif
 DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ImageList {
			get {
				return this.imageList;
			}
			set {
				if(ImageList == value) return;
				this.imageList = value;
				LayoutChanged();
			}
		}
		protected internal virtual object GetDefaultImages() { 
			if(Owner.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				SkinElement element = EditorsSkins.GetSkin(Owner.LookAndFeel)[EditorsSkins.SkinNavigator];
				if(element.Image != null) return element.Image.GetImages();
			}
			return ImagesDefault; 
		} 
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorButtonsBaseCustomButtons"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Editor("DevExpress.XtraEditors.Design.CustomButtonsCollectionEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public NavigatorCustomButtons CustomButtons { get { return customButtons; } }
		[Browsable(false)]
		public void LayoutChanged(){
			if(this.updateCount == 0)
				Owner.LayoutChanged();
		}
		[Browsable(false)]
		public INavigatorOwner Owner { get { return this.owner; } }
		protected internal NavigatorButtonsViewInfo ViewInfo { get { return this.viewInfo; } }
		protected virtual NavigatorButtonsViewInfo CreateViewInfo() {
			return new NavigatorButtonsViewInfo(this);
		}
		protected internal void OnKeyDown(KeyEventArgs e) {
			if(! Owner.TabStop) return;
			if(IsButtonPressedKey(e.KeyData)) {
				PressedButtonViewInfo = ViewInfo.GetButtonViewInfo(ViewInfo.FocusedButton);
				InvalidateButton(PressedButtonViewInfo);
				StartTimer();
			}
			if(e.KeyData == Keys.Right)
				ViewInfo.SetNextFocusedButton();
			if(e.KeyData == Keys.Left)
				ViewInfo.SetPrevFocusedButton();
		}
		protected internal void OnKeyUp(KeyEventArgs e) {
			StopTimer();
			if(! Owner.TabStop) return;
			if(IsButtonPressedKey(e.KeyData) && (PressedButtonViewInfo != null)) {
				NavigatorButtonViewInfo bViewInfo = PressedButtonViewInfo;
				PressedButtonViewInfo = null;
				InvalidateButton(bViewInfo);
				DoClick(bViewInfo.Button);
			}
		}
		protected internal void OnLostCapture() {
			PressedButtonViewInfo = null;
		}
		protected internal void OnMouseDown(MouseEventArgs e) {
			NavigatorButtonViewInfo bViewInfo = ViewInfo.ButtonViewInfoAt(new Point(e.X, e.Y));
			if(e.Button != MouseButtons.Left) return;
			if(bViewInfo != null && bViewInfo.State != ObjectState.Disabled) {
				if(Owner.TabStop) {
					ViewInfo.FocusedButton = bViewInfo.Button;
				}
				PressedButtonViewInfo = bViewInfo;
				InvalidateButton(bViewInfo);
				StartTimer();
			}
		}
		protected internal void OnMouseUp(MouseEventArgs e) {
			StopTimer();
			NavigatorButtonViewInfo bViewInfo = PressedButtonViewInfo;
			if(PressedButtonViewInfo != null) {
				bool needDoClick = PressedButtonViewInfo == ViewInfo.ButtonViewInfoAt(new Point(e.X, e.Y));
				PressedButtonViewInfo = null;
				InvalidateButton(bViewInfo);
				if(needDoClick)
					DoClick(bViewInfo.Button);
			}
		}
		protected internal void OnMouseMove(MouseEventArgs e) {
			HottrackButtonViewInfo = ViewInfo.ButtonViewInfoAt(new Point(e.X, e.Y));
			if(e.Button == MouseButtons.Left &&	PressedButtonViewInfo != null) {
				if(PressedButtonViewInfo != HottrackButtonViewInfo) 
					StopTimer();
				else StartTimer();
			}
		}
		protected internal void OnMouseLeave(EventArgs e) {
			HottrackButtonViewInfo = null;
		}
		public virtual void DoClick(NavigatorButtonBase button) {
			if(button == null) throw new ArgumentNullException("button");
			NavigatorButtonClickEventArgs e = new NavigatorButtonClickEventArgs(button);
			Owner.OnButtonClick(e);
			if(!e.Handled &&  Owner != null && !Owner.GetValidationCanceled())
				button.DoClick();
		}
		[Browsable(false), DefaultValue(DefaultPageRecordCount)]
		public virtual int PageRecordCount { get { return DefaultPageRecordCount; } set {} }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceObject Appearance { get { return Owner.Appearance; } }
		[Browsable(false), DefaultValue(NavigatorButtonsTextLocation.None), 
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal protected NavigatorButtonsTextLocation TextLocation{ 
			get { return this.textLocation; }
			set {
				if(value == TextLocation) return;
				this.textLocation = value;
				LayoutChanged();
			}
		}
		public void UpdateButtonsState() {
			for(int i = 0; i < ViewInfo.ButtonCollection.Count; i ++) 
				ViewInfo.ButtonCollection[i].Enabled = ViewInfo.ButtonCollection[i].Button.Enabled 
					&& ViewInfo.ButtonCollection[i].Button.ViewInfoEnabled;
			UpdateText();
		}
		public object GetToolTipObject(Point point) {
			NavigatorButtonViewInfo bViewInfo = ViewInfo.ButtonViewInfoAt(point);
			if(bViewInfo != null && (bViewInfo.Button.ViewInfoHint != string.Empty))
				return bViewInfo.Button;
			else return null;
		}
		public string GetToolTipText(object obj, Point point) {
			NavigatorButtonViewInfo bViewInfo = ViewInfo.ButtonViewInfoAt(point);
			if(bViewInfo != null)
				return bViewInfo.Button.ViewInfoHint;
			else return string.Empty;
		}
		protected abstract NavigatorButtonCollectionBase CreateNavigatorButtonCollection();
		protected virtual bool IsButtonPressedKey(Keys key) {
			return (key == Keys.Space) || (key == Keys.Return);
		}
		protected internal void InvalidateButton(NavigatorButtonViewInfo bViewInfo) {
			if(bViewInfo == null) return;
			Owner.OwnerControl.Invalidate(bViewInfo.Bounds, false);
		}
		protected internal NavigatorButtonViewInfo HottrackButtonViewInfo {
			get { return this.hottrackButtonViewInfo; }
			set { 
				if(HottrackButtonViewInfo == value) return;
				NavigatorButtonViewInfo old = HottrackButtonViewInfo;
				this.hottrackButtonViewInfo = null;
				InvalidateButton(old);
				if(value != null && value.State != ObjectState.Disabled) {
					this.hottrackButtonViewInfo = value;
					InvalidateButton(value);
				}
			}
		}
		protected internal NavigatorButtonViewInfo PressedButtonViewInfo{
			get { return ViewInfo.GetButtonViewInfo(pressedButton); }
			set { 
				if(value != null)
					this.pressedButton = value.Button;
				else this.pressedButton = null; 
			}
		}
		protected void UpdateText() {
			if(ViewInfo.TextViewInfo != null) {
				ViewInfo.TextViewInfo.UpdateText();
				Owner.OwnerControl.Invalidate(ViewInfo.TextViewInfo.Bounds, false);
			}
		}
		void StartTimer() {
			if(clickTimer != null) return;
			if(!EnabledAutoRepeat || pressedButton == null || !pressedButton.IsAutoRepeatSupported) return;
			clickTimer = new System.Windows.Forms.Timer();
			clickTimer.Interval = AutoRepeatDelay;
			clickTimer.Tick += new EventHandler(OnClickTick);
			clickTimer.Start();
		}
		void StopTimer() {
			if(clickTimer == null) return;
			clickTimer.Stop();
			clickTimer.Tick -= new EventHandler(OnClickTick);
			clickTimer.Dispose();
			clickTimer = null;
		}
		void OnClickTick(object sender, EventArgs e) {
			if(pressedButton == null)
				StopTimer();
			else {
				DoClick(pressedButton);
				if(clickTimer != null && clickTimer.Interval > 20) clickTimer.Interval /= 2;
			}
		}
		protected internal bool IsRightToLeft { get { return Owner != null && Owner.IsRightToLeft; } }
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class NavigatorControlViewInfo : BaseStyleControlViewInfo {
		public NavigatorControlViewInfo(BaseStyleControl owner) : base(owner) {
		}
		protected override AppearanceDefault CreateDefaultAppearance() { 
			return new AppearanceDefault(GetSystemColor(SystemColors.WindowText), GetSystemColor(SystemColors.Window), HorzAlignment.Center);
		}
		public NavigatorBase Navigator { get { return Owner as NavigatorBase; } }
		public NavigatorButtonsBase Buttons { get { return Navigator.ButtonsCore; }} 
		public override ObjectState CalcBorderState() {
			ObjectState state = State;
			if(state == ObjectState.Normal && (BorderPainter is Office2003BorderPainter || BorderPainter is UltraFlatBorderPainter)) {
				state = ObjectState.Hot;
			}
			return state;
		}
		public override void CalcViewInfo(Graphics g) {
			base.CalcViewInfo(g);
			Size size = ClientRect.Size;
			Buttons.CheckSize(ref size);
			if(!ClientRect.Size.Equals(size) && ClientRect.Width != 0 && ClientRect.Height != 0) { 
				size.Width += Bounds.Width - ClientRect.Width;
				size.Height += Bounds.Height - ClientRect.Height;
				Navigator.RaiseSizeableChangedInternal();
				Navigator.Size = size;
			} 
		}
		public override Size CalcBestFit(Graphics g) {
			Size size = Size.Empty;
			UpdatePaintAppearance();
			UpdatePainters();
			Rectangle bounds = new Rectangle(0, 0, 10000, 10000);
			Rectangle client = CalcClientRectCore(bounds);
			Buttons.CheckSize(ref size);
			size.Width += bounds.Width - client.Width;
			size.Height += bounds.Height - client.Height;
			return size;
		}
		public string GetText() {
			if(Buttons.ViewInfo == null || Buttons.ViewInfo.TextViewInfo == null) return null;
			Buttons.ViewInfo.TextViewInfo.UpdateText();
			return Buttons.ViewInfo.TextViewInfo.Text;
		}
	}
}
namespace DevExpress.XtraEditors {
	public class NavigatorControlPainter : BaseControlPainter {
		public NavigatorControlPainter() {}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			NavigatorControlViewInfo vi = info.ViewInfo as NavigatorControlViewInfo;
			vi.Buttons.Draw(info);
		}
	}
	public class NavigatorButtonClickEventArgs : EventArgs {
		NavigatorButtonBase button;
		bool handled = false;
		public NavigatorButtonClickEventArgs(NavigatorButtonBase button) {
			this.button = button;
		}
		public NavigatorButtonBase Button { get { return button; } }
		public bool Handled { get { return handled; } set { handled = value; } }
	}
	public delegate void NavigatorButtonClickEventHandler(object sender, NavigatorButtonClickEventArgs e);
	public delegate void NavigatorExceptionEventHandler(object sender, NavigatorExceptionEventArgs e);
	[Designer("DevExpress.XtraEditors.Design.NavigatorBaseDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public abstract class NavigatorBase : BaseStyleControl, INavigatorOwner {
		NavigatorButtonsBase buttons;
		BorderStyles buttonStyle;
		int prevCustomButonCount = 0;
		string textStringFormat = String.Empty;
		bool autoSize;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(true)]
		public NavigatorCustomButtons CustomButtons { get { return buttons.CustomButtons; } }
		protected NavigatorBase(){
			this.buttons = CreateButtons();
			base.TabStop = false;
			this.autoSize = false;
			this.fShowToolTips = false;
			this.textStringFormat = Localizer.Active.GetLocalizedString(StringId.NavigatorTextStringFormat);
			this.buttonStyle = BorderStyles.Default;
			this.SetStyle(ControlStyles.UserMouse, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(Visible && IsHandleCreated)
				LayoutChanged();
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.Accessibility.NavigatorAccessible(this); 
		}
		protected override bool CanAnimateCore {
			get {
				return base.CanAnimateCore && LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.WindowsXP;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new NavigatorControlViewInfo ViewInfo { get { return base.ViewInfo as NavigatorControlViewInfo; } }
		AppearanceObject INavigatorOwner.Appearance { get { return ViewInfo.PaintAppearance; } }
		bool INavigatorOwner.GetValidationCanceled() { return GetValidationCanceledCore(); }
		bool INavigatorOwner.IsRightToLeft { get { return IsRightToLeft; } }
		protected virtual bool GetValidationCanceledCore() { return GetValidationCanceled(); }
		private static readonly object buttonClick = new object();
		[DXCategory(CategoryName.Events), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorBaseButtonClick")
#else
	Description("")
#endif
]
		public event NavigatorButtonClickEventHandler ButtonClick {
			add { Events.AddHandler(buttonClick, value); }
			remove { Events.RemoveHandler(buttonClick, value); }
		}
		protected override Size CalcSizeableMinSize() { return MinSizeSizeable; }
		protected override Size CalcSizeableMaxSize() { return new Size((int)(MinSizeSizeable.Width * 1.5), MinSizeSizeable.Height); } 
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorBaseShowToolTips"),
#endif
 DefaultValue(false)]
		public override bool ShowToolTips { get { return base.ShowToolTips; } set { base.ShowToolTips = value; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorBaseTabStop"),
#endif
 DefaultValue(false)]
		public new bool TabStop { get { return base.TabStop; } set { base.TabStop = value; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorBaseButtonStyle"),
#endif
 DefaultValue(BorderStyles.Default), SmartTagProperty("Button Style", "")]
		public BorderStyles ButtonStyle { 
			get { return this.buttonStyle; }
			set {
				if(value == ButtonStyle) return;
				this.buttonStyle = value;
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorBaseTextLocation"),
#endif
 DefaultValue(NavigatorButtonsTextLocation.None), Localizable(true), SmartTagProperty("Text Location", "")]
		public virtual NavigatorButtonsTextLocation TextLocation{ 
			get { return ButtonsCore.TextLocation; }
			set { ButtonsCore.TextLocation = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorBaseAutoSize"),
#endif
 DefaultValue(false), Browsable(true), SmartTagProperty("Auto Size", "", 0)]
		public override bool AutoSize {
			get { return autoSize; }
			set {
				if(value == AutoSize) return;
				this.autoSize = value;
				LayoutChanged();
			}
		}
		protected virtual void ResetTextStringFormat() {
			TextStringFormat = Localizer.Active.GetLocalizedString(StringId.NavigatorTextStringFormat);
		}
		protected virtual bool ShouldSerializeTextStringFormat() {
			return TextStringFormat != Localizer.Active.GetLocalizedString(StringId.NavigatorTextStringFormat);
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorBaseTextStringFormat"),
#endif
 Localizable(true), SmartTagProperty("Text String Format", "")]
		public virtual string TextStringFormat{ 
			get { return textStringFormat; }
			set { 
				textStringFormat = value; 
				LayoutChanged();
				RaiseSizeableChanged();
			}
		}
		protected internal void RaiseSizeableChangedInternal(){
			RaiseSizeableChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size MinSize {
			get {
				Size minSize = Size.Empty;
				ButtonsCore.CheckSize(ref minSize);
				minSize.Width += 2;
				return minSize;
			}
		}
		internal Size MinSizeSizeable {
			get {
				var res = MinSize;
				res.Height += 2;
				return res;
			}
		}
		protected override BaseControlPainter CreatePainter() {
			return new NavigatorControlPainter();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new NavigatorControlViewInfo(this);
		}
		protected override Size DefaultSize { get { return new Size(160, 24); } }
		internal protected NavigatorButtonsBase ButtonsCore { get { return buttons; } }
		protected abstract NavigatorButtonsBase CreateButtons();
		protected abstract int RecordCount { get; }
		protected abstract int CurrentRecordIndex { get; }
		protected override bool IsInputKey(Keys keyData) {
			return (keyData == Keys.Right) || (keyData == Keys.Left);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			ButtonsCore.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(e.Handled) return;
			ButtonsCore.OnKeyUp(e);
		}
		protected override void OnLostCapture() {
			ButtonsCore.OnLostCapture();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			ButtonsCore.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			ButtonsCore.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			ButtonsCore.OnMouseMove(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ButtonsCore.OnMouseLeave(e);
		}
		protected override void OnTabStopChanged(EventArgs e) {
			base.OnTabStopChanged(e);
			if(Focused)	LayoutChanged();
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			if(TabStop) 
				Invalidate();
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			if(TabStop) 
				Invalidate();
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			LayoutChanged();
		}
		protected virtual EditorButtonPainter ButtonPainterCore {
			get {
				return EditorButtonHelper.GetPainter(ButtonStyle, LookAndFeel, true);
			}
		}
		EditorButtonPainter INavigatorOwner.ButtonPainter {
			get { return ButtonPainterCore; }
		}
		bool INavigatorOwner.IsUltraFlatButtonBorder {
			get {
				return ViewInfo != null && ViewInfo.BorderPainter is UltraFlatBorderPainter && ButtonsCore.ViewInfo.ButtonPainter.ButtonPainter is UltraFlatButtonObjectPainter;
			}
		}
		Orientation INavigatorOwner.Orientation { get { return Orientation.Horizontal; } }
		Rectangle INavigatorOwner.Bounds { get { return ViewInfo.ClientRect; } }
		bool INavigatorOwner.Focused { get { return Focused; } }
		void INavigatorOwner.Focus() { this.Focus(); }
		bool INavigatorOwner.Visible { get { return Visible; } }
		bool INavigatorOwner.TabStop { get { return TabStop; } } 
		Control INavigatorOwner.OwnerControl { get { return this; } }
		UserLookAndFeel INavigatorOwner.LookAndFeel { get { return LookAndFeel; } }
		BorderStyles INavigatorOwner.ButtonStyle { get  { return ButtonStyle; }  }
		bool INavigatorOwner.DesignMode { get { return DesignMode; } }
		void INavigatorOwner.LayoutChanged(){
			if(CustomButtons.Count != prevCustomButonCount) { prevCustomButonCount = CustomButtons.Count; RaiseSizeableChanged(); }
			LayoutChanged();
		}
		void INavigatorOwner.OnButtonClick(NavigatorButtonClickEventArgs e) {
			NavigatorButtonClickEventHandler handler = (NavigatorButtonClickEventHandler)this.Events[buttonClick];
			if(handler != null) handler(this, e);
		}
		int INavigatorOwner.RecordCount { get { return RecordCount; } }
		int INavigatorOwner.CurrentRecordIndex { get { return CurrentRecordIndex; } }
		NavigatorButtonsBase INavigatorOwner.Buttons { get { return this.ButtonsCore; } }
		protected override ToolTipControlInfo GetToolTipInfo(Point point) { 
			object obj = ButtonsCore.GetToolTipObject(point);
			if(obj == null) return null;
			return new ToolTipControlInfo(obj, ButtonsCore.GetToolTipText(obj, point));
		}
		#region Navigator Exception
		private static readonly object navigatorException = new object();
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("NavigatorBaseNavigatorException"),
#endif
 DXCategory(CategoryName.Action)]
		public event NavigatorExceptionEventHandler NavigatorException {
			add { this.Events.AddHandler(navigatorException, value); }
			remove { this.Events.RemoveHandler(navigatorException, value); }
		}
		protected internal virtual void RaiseNavigatorException(NavigatorExceptionEventArgs e) {
			NavigatorExceptionEventHandler handler = (NavigatorExceptionEventHandler)this.Events[navigatorException];
			if(handler != null) handler(this, e);
		}
		void INavigatorOwner.OnNavigatorException(Exception sourceException, NavigatorButton button) {
			NavigatorExceptionEventArgs e = new NavigatorExceptionEventArgs(sourceException.Message, sourceException, button);
			RaiseNavigatorException(e);
			if(e.ExceptionMode == ExceptionMode.ThrowException) throw e.Exception;
			if(e.ExceptionMode == ExceptionMode.DisplayError) {
				ShowError(e.WindowCaption, e.ErrorText);
			}
			if(e.ExceptionMode == ExceptionMode.Ignore || e.ExceptionMode == ExceptionMode.NoAction) return;
		}
		private void ShowError(string windowCaption, string errorText) {
			XtraMessageBox.Show(LookAndFeel, errorText, windowCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		#endregion
	}
	interface IDataNavigatorOwner {
		CurrencyManager CurrencyManager { get; }
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface INavigatableControl {
		void AddNavigator(INavigatorOwner owner);
		void RemoveNavigator(INavigatorOwner owner);
		int RecordCount { get; }
		int Position { get; }
		bool IsActionEnabled(NavigatorButtonType type);
		void DoAction(NavigatorButtonType type);
	}
	interface IGetNavigatableControl {
		INavigatableControl Control { get; }
	}
	public class NavigatableControlHelper {
		[ListBindable(false)]
		class NavigatorOwnerCollection : CollectionBase {
			public NavigatorOwnerCollection(){}
			public INavigatorOwner this[int index] { get { return List[index] as  INavigatorOwner; } }
			public void Add(INavigatorOwner value) { List.Add(value); }
			public void Remove(INavigatorOwner value) { List.Remove(value); }
		}
		NavigatorOwnerCollection collection;
		public NavigatableControlHelper() {
			this.collection = new NavigatorOwnerCollection();
		}
		public void AddNavigator(INavigatorOwner owner) {
			this.collection.Add(owner);
		}
		public void RemoveNavigator(INavigatorOwner owner) {
			this.collection.Remove(owner);
		}
		public void UpdateButtons() {
			for(int i = 0; i < this.collection.Count; i ++)
				UpdateButtons(this.collection[i]);
		}
		public void Clear() {
			this.collection.Clear();
		}
		void UpdateButtons(INavigatorOwner owner) {
			if(!owner.Visible) return;
			if(owner.Buttons.TextLocation == NavigatorButtonsTextLocation.None)
				owner.Buttons.UpdateButtonsState();
			else owner.Buttons.LayoutChanged();
		}
	}
}
namespace DevExpress.XtraEditors.NavigatorButtons {
	public abstract class DataNavigatorButtonHelperBase : NavigatorButtonHelper {
		public DataNavigatorButtonHelperBase(NavigatorButtonsBase buttons) : base(buttons) {}
		protected DataNavigatorButtons DataButtons { get { return Buttons as DataNavigatorButtons; } }
		public CurrencyManager CurrencyManager { get { return DataButtons.CurrencyManager; } }
		public IBindingList BindingList { get { return DataButtons.BindingList; } }
		public IList List { get { return DataButtons.List; } }
		public override void DoClick() {
			if(Enabled)
				DoDataClick();
		}
		protected abstract void DoDataClick();
		public override bool Enabled { get { return CurrencyManager != null; } }	
	}
	public class DataNavigatorFirstButtonHelper : DataNavigatorButtonHelperBase {
		public DataNavigatorFirstButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.First; } }
		protected override void DoDataClick() {
			CurrencyManager.Position = 0;
		}
		public override bool Enabled { get {return base.Enabled && CurrencyManager.Position > 0; } }
	}
	public class DataNavigatorPrevPageButtonHelper : DataNavigatorFirstButtonHelper {
		public DataNavigatorPrevPageButtonHelper(NavigatorButtonsBase buttons) : base(buttons){}
		public override NavigatorButtonType ButtonType {get { return NavigatorButtonType.PrevPage; } }
		protected override void DoDataClick() {
			CurrencyManager.Position -= Buttons.PageRecordCount;
		}
	}
	public class DataNavigatorPrevButtonHelper : DataNavigatorFirstButtonHelper {
		public DataNavigatorPrevButtonHelper(NavigatorButtonsBase buttons) : base(buttons){}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Prev; } }
		protected override void DoDataClick() {
			CurrencyManager.Position --;
		}
	}
	public class DataNavigatorNextButtonHelper : DataNavigatorLastButtonHelper {
		public DataNavigatorNextButtonHelper(NavigatorButtonsBase buttons) : base(buttons){}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Next; } }
		protected override void DoDataClick() {
			CurrencyManager.Position ++;
		}
	}
	public class DataNavigatorNextPageButtonHelper : DataNavigatorLastButtonHelper {
		public DataNavigatorNextPageButtonHelper(NavigatorButtonsBase buttons) : base(buttons){}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.NextPage; } }
		protected override void DoDataClick() {
			CurrencyManager.Position += Buttons.PageRecordCount; 
		}
	}
	public class DataNavigatorLastButtonHelper : DataNavigatorButtonHelperBase {
		public DataNavigatorLastButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Last; } }
		protected override void DoDataClick() {
			CurrencyManager.Position = CurrencyManager.Count - 1;
		}
		public override bool Enabled {get { return base.Enabled && CurrencyManager.Position < CurrencyManager.Count - 1; } }
	}
	public class DataNavigatorAddButtonHelper : DataNavigatorButtonHelperBase {
		public DataNavigatorAddButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Append; } }
		public override bool Enabled { 
			get {
				bool ret = BindingList != null && BindingList.AllowNew;
				if(ret && this.CurrencyManager.List is DataView) 
					ret = ((DataView)this.CurrencyManager.List).Table != null; 
				return ret;
			}
		}	
		protected override void DoDataClick() {
			CurrencyManager.EndCurrentEdit();
			CurrencyManager.AddNew();
		}
	}
	public class DataNavigatorRemoveButtonHelper : DataNavigatorButtonHelperBase {
		public DataNavigatorRemoveButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Remove; } }
		public override bool Enabled { 
			get { 
				if (!base.Enabled || CurrencyManager.Count == 0) 
					return false;
				if(BindingList != null)
					return BindingList.AllowRemove;
				if(List != null && ! List.IsReadOnly)
					return true;
				return false;
			} 
		}
		protected override void DoDataClick() {
			CurrencyManager.RemoveAt(CurrencyManager.Position);
			if(BindingList == null) 
				CurrencyManager.Refresh();
		}
	}
	public class DataNavigatorEndEditButtonHelper : DataNavigatorButtonHelperBase {
		public DataNavigatorEndEditButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.EndEdit; } }
		protected override void DoDataClick() {
			CurrencyManager.EndCurrentEdit();
		}
		public override bool Enabled { 
			get { 
				if (! base.Enabled || CurrencyManager.Count == 0)
					return false;
				if(BindingList != null && BindingList.AllowEdit)
					return true;
				if(List != null && ! List.IsReadOnly)
					return true;
				return false;
			} 
		}
	}
	public class DataNavigatorCancelEditButtonHelper : DataNavigatorEndEditButtonHelper {
		public DataNavigatorCancelEditButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.CancelEdit; } }
		protected override void DoDataClick() {
			CurrencyManager.CancelCurrentEdit();
		}
	}
	public abstract class ControlNavigatorButtonHelperBase : NavigatorButtonHelper {
		public ControlNavigatorButtonHelperBase(NavigatorButtonsBase buttons) : base(buttons) {}
		protected ControlNavigatorButtons ControlButtons { get { return Buttons as ControlNavigatorButtons; } }
		public INavigatableControl Control { get { return ControlButtons.Control; } }
		public override void DoClick() {
			if(Enabled)
				DoDataClick();
		}
		protected virtual void DoDataClick() {
			Control.DoAction(ButtonType);
		}
		public override bool Enabled { get { return (Control != null) && Control.IsActionEnabled(ButtonType); } }	
	}
	public class ControlNavigatorFirstButtonHelper : ControlNavigatorButtonHelperBase {
		public ControlNavigatorFirstButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.First; } }
	}
	public class ControlNavigatorPrevPageButtonHelper : ControlNavigatorFirstButtonHelper {
		public ControlNavigatorPrevPageButtonHelper(NavigatorButtonsBase buttons) : base(buttons){}
		public override NavigatorButtonType ButtonType {get { return NavigatorButtonType.PrevPage; } }
	}
	public class ControlNavigatorPrevButtonHelper : ControlNavigatorFirstButtonHelper {
		public ControlNavigatorPrevButtonHelper(NavigatorButtonsBase buttons) : base(buttons){}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Prev; } }
	}
	public class ControlNavigatorNextButtonHelper : ControlNavigatorLastButtonHelper {
		public ControlNavigatorNextButtonHelper(NavigatorButtonsBase buttons) : base(buttons){}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Next; } }
	}
	public class ControlNavigatorNextPageButtonHelper : ControlNavigatorLastButtonHelper {
		public ControlNavigatorNextPageButtonHelper(NavigatorButtonsBase buttons) : base(buttons){}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.NextPage; } }
	}
	public class ControlNavigatorLastButtonHelper : ControlNavigatorButtonHelperBase {
		public ControlNavigatorLastButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Last; } }
	}
	public class ControlNavigatorAddButtonHelper : ControlNavigatorButtonHelperBase {
		public ControlNavigatorAddButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Append; } }
	}
	public class ControlNavigatorRemoveButtonHelper : ControlNavigatorButtonHelperBase {
		public ControlNavigatorRemoveButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Remove; } }
	}
	public class ControlNavigatorEditButtonHelper : ControlNavigatorButtonHelperBase {
		public ControlNavigatorEditButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.Edit; } }
	}
	public class ControlNavigatorEndEditButtonHelper : ControlNavigatorButtonHelperBase {
		public ControlNavigatorEndEditButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.EndEdit; } }
	}
	public class ControlNavigatorCancelEditButtonHelper : ControlNavigatorButtonHelperBase {
		public ControlNavigatorCancelEditButtonHelper(NavigatorButtonsBase buttons) : base(buttons) {}
		public override NavigatorButtonType ButtonType { get { return NavigatorButtonType.CancelEdit; } }
	}
}
namespace DevExpress.XtraEditors {
	public class DataNavigatorButtonCollection : NavigatorButtonCollectionBase {
		public DataNavigatorButtonCollection(NavigatorButtonsBase buttons) : base(buttons){
		}
		protected override void CreateButtons(NavigatorButtonsBase buttons){
			AddButton(new DataNavigatorFirstButtonHelper(buttons));
			AddButton(new DataNavigatorPrevPageButtonHelper(buttons));
			AddButton(new DataNavigatorPrevButtonHelper(buttons));
			AddButton(new DataNavigatorNextButtonHelper(buttons));
			AddButton(new DataNavigatorNextPageButtonHelper(buttons));
			AddButton(new DataNavigatorLastButtonHelper(buttons));
			AddButton(new DataNavigatorAddButtonHelper(buttons));
			AddButton(new DataNavigatorRemoveButtonHelper(buttons));
			AddButton(new DataNavigatorEndEditButtonHelper(buttons));
			AddButton(new DataNavigatorCancelEditButtonHelper(buttons));
		}
	}
	[TypeConverter("System.ComponentModel.ExpandableObjectConverter, System")]
	public class DataNavigatorButtons : NavigatorButtonsBase {
		int pageRecordCount;
		public DataNavigatorButtons(INavigatorOwner owner) : base(owner){
			this.pageRecordCount = DefaultPageRecordCount;
		}
		protected override NavigatorButtonCollectionBase CreateNavigatorButtonCollection(){
			return new DataNavigatorButtonCollection(this);
		}
		[Browsable(false)]
		public CurrencyManager CurrencyManager {
			get {
				if(Owner is IDataNavigatorOwner)
					return ((IDataNavigatorOwner)Owner).CurrencyManager;
				else return null;
			}
		}
		protected internal IBindingList BindingList{ 
			get {
				return (CurrencyManager != null) && (CurrencyManager.List is IBindingList) ? CurrencyManager.List as IBindingList : null;
			}
		}
		protected internal IList List{ 
			get {
				return (CurrencyManager != null) ? CurrencyManager.List : null;
			}
		}
		public void CurrencyManagerChanged() {
			UpdateButtonsState();
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsPageRecordCount")]
#endif
		public override int PageRecordCount {
			get { return pageRecordCount; } 
			set { 
				if(value <= 1) return;
				pageRecordCount = value;
			}
		}
		bool ShouldSerializeFirst() { return First.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsFirst"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigatorButton First { get { return ButtonByButtonType(NavigatorButtonType.First); } }
		bool ShouldSerializePrevPage() { return PrevPage.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsPrevPage"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigatorButton PrevPage { get { return ButtonByButtonType(NavigatorButtonType.PrevPage); } }
		bool ShouldSerializePrev() { return Prev.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsPrev"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigatorButton Prev { get { return ButtonByButtonType(NavigatorButtonType.Prev); } }
		bool ShouldSerializeNext() { return Next.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsNext"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigatorButton Next { get { return ButtonByButtonType(NavigatorButtonType.Next); } }
		bool ShouldSerializeNextPage() { return NextPage.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsNextPage"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigatorButton NextPage { get { return ButtonByButtonType(NavigatorButtonType.NextPage); } }
		bool ShouldSerializeLast() { return Last.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsLast"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigatorButton Last { get { return ButtonByButtonType(NavigatorButtonType.Last); } }
		bool ShouldSerializeAppend() { return Append.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsAppend"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigatorButton Append { get { return ButtonByButtonType(NavigatorButtonType.Append); } }
		bool ShouldSerializeRemove() { return Remove.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsRemove"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigatorButton Remove { get { return ButtonByButtonType(NavigatorButtonType.Remove); } }
		bool ShouldSerializeEndEdit() { return EndEdit.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsEndEdit"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigatorButton EndEdit { get { return ButtonByButtonType(NavigatorButtonType.EndEdit); } }
		bool ShouldSerializeCancelEdit() { return CancelEdit.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtonsCancelEdit"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigatorButton CancelEdit { get { return ButtonByButtonType(NavigatorButtonType.CancelEdit); } }
	}
	[DXToolboxItem(DXToolboxItemKind.Free),
	 Description("Allows an end-user to navigate through the records in a data source and perform operations against the data."),
	 ToolboxTabName(AssemblyInfo.DXTabData), SmartTagAction(typeof(DataNavigatorButtonsActions), "CustomButtons", "Custom Buttons", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "DataNavigator")
	]
	public class DataNavigator : NavigatorBase, IDataNavigatorOwner {
		object dataSource = null;
		string dataMember = string.Empty;
		CurrencyManager currencyManager = null;
		public DataNavigator(){
		}
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			if(disposing) {
				CurrencyManager = null;
				if(GetBindingList(DataSource) != null)
					GetBindingList(DataSource).ListChanged -= new ListChangedEventHandler(OnBindingList_ListChanged);
			}
			base.Dispose(disposing);
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorButtons"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public DataNavigatorButtons Buttons { get {return ButtonsCore as DataNavigatorButtons;}}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorDataSource"),
#endif
#if DXWhidbey
		AttributeProvider(typeof(IListSource)), 
#else
		TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design"),
#endif
		DefaultValue(null)]
		public object DataSource {
			get { return dataSource; }
			set {
				if(value == DataSource) return;
				if(value != null && DataSource != null && DataSource.Equals(value)) return;
				if(IsValidDataSource(value)){
					InitDataSource(value);
					dataSource = value;
					UpdateCurrencyManager();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorPosition"),
#endif
 Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Position {
			get { 
				if(CurrencyManager != null)
					return CurrencyManager.Position; 
				return -1;
			}
			set { 
				if(CurrencyManager != null)
					CurrencyManager.Position = value; 
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorDataMember"),
#endif
 DefaultValue(""), Editor(ControlConstants.DataMemberEditor, typeof(System.Drawing.Design.UITypeEditor))]
		public string DataMember {
			get { return dataMember; }
			set {
				if(DataMember == value) return;
				dataMember = value;
				UpdateCurrencyManager();
			}
		}
		protected override NavigatorButtonsBase CreateButtons(){
			return new DataNavigatorButtons(this);
		}
		protected override int RecordCount { get { return CurrencyManager != null ? CurrencyManager.Count : 0; }	}
		protected override int CurrentRecordIndex { get { return CurrencyManager != null && RecordCount > 0 ? CurrencyManager.Position + 1 : 0; }	}
		protected CurrencyManager CurrencyManager {
			get {return currencyManager;}
			set {
				if(CurrencyManager != value) {
					if(CurrencyManager != null) {
						CurrencyManager.ItemChanged -=  new ItemChangedEventHandler(CurrencyManagerItemChanged);
						CurrencyManager.PositionChanged -= new EventHandler(CurrencyManagerPositionChanged);
					}
					currencyManager = value;
					if(value != null) {
						value.ItemChanged +=  new ItemChangedEventHandler(CurrencyManagerItemChanged);
						value.PositionChanged += new EventHandler(CurrencyManagerPositionChanged);
					}
					Buttons.CurrencyManagerChanged();
				}
			}
		}
		protected override void OnBindingContextChanged(EventArgs e) {
			UpdateCurrencyManager();
			base.OnBindingContextChanged(e);
		}
		CurrencyManager IDataNavigatorOwner.CurrencyManager { get { return this.CurrencyManager; } }
		private void CurrencyManagerItemChanged(object sender, System.Windows.Forms.ItemChangedEventArgs e){
			Buttons.UpdateButtonsState();
		}
		private static readonly object positionChanged = new object();
		[DXCategory(CategoryName.Events), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("DataNavigatorPositionChanged")
#else
	Description("")
#endif
]
		public event EventHandler PositionChanged {
			add { Events.AddHandler(positionChanged, value); }
			remove { Events.RemoveHandler(positionChanged, value); }
		}
		protected internal virtual void RaisePositionChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[positionChanged];
			if(handler != null) handler(this, e);
		}
		private void CurrencyManagerPositionChanged(object sender, System.EventArgs e){
			Buttons.UpdateButtonsState();
			RaisePositionChanged(e);
		}
		private bool IsValidDataSource(object dataSource) {
			if(dataSource == null) return true;
			if(dataSource is IList) return true;
			if(dataSource is IListSource) return true;
			return false;
		}
		private void UpdateCurrencyManager(){
			if((DataSource != null) && (BindingContext != null))
				CurrencyManager = this.BindingContext[DataSource, DataMember] as CurrencyManager;
			else CurrencyManager = null;
			LayoutChanged();
		}
		private void InitDataSource(object dataSource) {
			InitBindingList(dataSource);
		}
		private void InitBindingList(object dataSource) {
			if(GetBindingList(DataSource) != null)
				GetBindingList(DataSource).ListChanged -= new ListChangedEventHandler(OnBindingList_ListChanged);
			if(GetBindingList(dataSource) != null)
				GetBindingList(dataSource).ListChanged += new ListChangedEventHandler(OnBindingList_ListChanged);
		}
		private IBindingList GetBindingList(object dataSource) {
			return (dataSource != null) && (dataSource is IBindingList) ? dataSource as IBindingList : null;
		}
		private void OnBindingList_ListChanged(object sender, ListChangedEventArgs e) {
			Buttons.UpdateButtonsState();
		}
	}
	public class ControlNavigatorButtonCollection : NavigatorButtonCollectionBase {
		public ControlNavigatorButtonCollection(NavigatorButtonsBase buttons) : base(buttons){
		}
		protected override void CreateButtons(NavigatorButtonsBase buttons){
			AddButton(new ControlNavigatorFirstButtonHelper(buttons));
			AddButton(new ControlNavigatorPrevPageButtonHelper(buttons));
			AddButton(new ControlNavigatorPrevButtonHelper(buttons));
			AddButton(new ControlNavigatorNextButtonHelper(buttons));
			AddButton(new ControlNavigatorNextPageButtonHelper(buttons));
			AddButton(new ControlNavigatorLastButtonHelper(buttons));
			AddButton(new ControlNavigatorAddButtonHelper(buttons));
			AddButton(new ControlNavigatorRemoveButtonHelper(buttons));
			AddButton(new ControlNavigatorEditButtonHelper(buttons));
			AddButton(new ControlNavigatorEndEditButtonHelper(buttons));
			AddButton(new ControlNavigatorCancelEditButtonHelper(buttons));
		}
	}
	[TypeConverter("System.ComponentModel.ExpandableObjectConverter, System")]
	public class ControlNavigatorButtons : NavigatorButtonsBase {
		public ControlNavigatorButtons(INavigatorOwner owner) : base(owner){
		}
		[Browsable(false)]
		public INavigatableControl Control {
			get {
				if(Owner is IGetNavigatableControl)
					return ((IGetNavigatableControl)Owner).Control;
				else return null;
			}
		}
		protected override NavigatorButtonCollectionBase CreateNavigatorButtonCollection(){
			return new ControlNavigatorButtonCollection(this);
		}
		protected virtual bool ShouldSerializeFirst() { return First.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsFirst"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton First { get { return ButtonByButtonType(NavigatorButtonType.First); } }
		protected virtual bool ShouldSerializePrevPage() { return PrevPage.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsPrevPage"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton PrevPage { get { return ButtonByButtonType(NavigatorButtonType.PrevPage); } }
		protected virtual bool ShouldSerializePrev() { return Prev.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsPrev"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton Prev { get { return ButtonByButtonType(NavigatorButtonType.Prev); } }
		protected virtual bool ShouldSerializeNext() { return Next.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsNext"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton Next { get { return ButtonByButtonType(NavigatorButtonType.Next); } }
		protected virtual bool ShouldSerializeNextPage() { return NextPage.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsNextPage"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton NextPage { get { return ButtonByButtonType(NavigatorButtonType.NextPage); } }
		protected virtual bool ShouldSerializeLast() { return Last.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsLast"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton Last { get { return ButtonByButtonType(NavigatorButtonType.Last); } }
		protected virtual bool ShouldSerializeAppend() { return Append.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsAppend"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton Append { get { return ButtonByButtonType(NavigatorButtonType.Append); } }
		protected virtual bool ShouldSerializeRemove() { return Remove.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsRemove"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton Remove { get { return ButtonByButtonType(NavigatorButtonType.Remove); } }
		protected virtual bool ShouldSerializeEdit() { return Edit.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsEdit"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton Edit { get { return ButtonByButtonType(NavigatorButtonType.Edit); } }
		protected virtual bool ShouldSerializeEndEdit() { return EndEdit.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsEndEdit"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton EndEdit { get { return ButtonByButtonType(NavigatorButtonType.EndEdit); } }
		protected virtual bool ShouldSerializeCancelEdit() { return CancelEdit.ShouldSerialize; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtonsCancelEdit"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual NavigatorButton CancelEdit { get { return ButtonByButtonType(NavigatorButtonType.CancelEdit); } }
	}
	[DXToolboxItem(DXToolboxItemKind.Free), 
	 Description("Allows an end-user to navigate through the records in a data-aware control and perform operations against the data."),
	 ToolboxTabName(AssemblyInfo.DXTabData), SmartTagAction(typeof(ControlNavigatorButtonsActions), "CustomButtons", "Custom Buttons", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "ControlNavigator")
	]
	public class ControlNavigator : NavigatorBase, IGetNavigatableControl {
		INavigatableControl navigatableControl;
		public ControlNavigator(){
			this.navigatableControl = null;
			SetStyle(ControlStyles.Selectable, false);
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorButtons"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public ControlNavigatorButtons Buttons { get {return ButtonsCore as ControlNavigatorButtons;}}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ControlNavigatorNavigatableControl"),
#endif
 DefaultValue(null), SmartTagProperty("Navigatable Control", "")]
		public virtual INavigatableControl NavigatableControl {	
			get { return navigatableControl; }
			set {
				if(value == NavigatableControl) return;
				if(NavigatableControl != null)
					NavigatableControl.RemoveNavigator(this);
				this.navigatableControl = value;
				if(NavigatableControl != null)
					NavigatableControl.AddNavigator(this);
			}
		}
		public override string ToString() {
			return string.Empty;
		}
		protected override NavigatorButtonsBase CreateButtons(){
			return new ControlNavigatorButtons(this);
		}
		protected override int RecordCount { get { return NavigatableControl != null ? NavigatableControl.RecordCount : 0; }	}
		protected override int CurrentRecordIndex { get { return NavigatableControl != null && RecordCount > 0  ? NavigatableControl.Position + 1 : 0; } }
		INavigatableControl IGetNavigatableControl.Control { get { return NavigatableControl; } }
	}
	public class NavigatorExceptionEventArgs : ExceptionEventArgs {
		NavigatorButton button;
		public NavigatorExceptionEventArgs(string errorText, Exception exception, NavigatorButton button) : 
			this(errorText, Localizer.Active.GetLocalizedString(StringId.CaptionError), exception, ExceptionMode.DisplayError, button) { }
		public NavigatorExceptionEventArgs(string errorText, string windowCaption, Exception exception, ExceptionMode exceptionMode, NavigatorButton button) :
			base(errorText, windowCaption, exception, exceptionMode) {
			this.button = button;
		}
		public NavigatorButton Button { get { return button; } }
	}
}
