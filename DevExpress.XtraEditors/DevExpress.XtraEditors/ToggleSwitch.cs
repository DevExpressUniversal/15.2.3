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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.Skins;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemToggleSwitch : BaseRepositoryItemCheckEdit {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemToggleSwitch Properties { get { return this; } }
		public RepositoryItemToggleSwitch() {
			imageListCore = new ArrayList();
		}		
		public override string EditorTypeName { get { return "ToggleSwitch"; } }
		string onContentCore = "On";
		string offContentCore = "Off";
		[ Localizable(true), DXCategory(CategoryName.Behavior), SmartTagProperty("Off Text", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public string OffText {
			get { return offContentCore; }
			set {
				if(OffText == value) return;
				offContentCore = value;
				OnCaptionChanged();
			}
		}
		bool ShouldSerializeOffText() {
			return offContentCore != null;
		}
		void ResetOffText() {
			OffText = "Off";
		}
		[ Localizable(true), DXCategory(CategoryName.Behavior), SmartTagProperty("On Text", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public string OnText {
			get { return onContentCore; }
			set {
				if(OnText == value) return;
				onContentCore = value;
				OnCaptionChanged();
			}
		}
		bool ShouldSerializeOnText() {
			return onContentCore != null;
		}
		void ResetOnText() {
			OnText = "On";
		}
		bool allowThumbAnimation = true;
		[ DXCategory(CategoryName.Behavior), DefaultValue(true), SmartTagProperty("Allow Thumb Animation", "", 0, SmartTagActionType.RefreshBoundsAfterExecute)]
		public bool AllowThumbAnimation {
			get { return allowThumbAnimation; }
			set {
				if(AllowThumbAnimation != value) {
					allowThumbAnimation = value;
					LayoutChanged();
				}
			}
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override string Caption {
			get { return EditorTypeName; }
			set { }
		}
		ArrayList imageListCore;
		Image CatImage(Image image, Rectangle selection) {
			Bitmap bmp = image as Bitmap;
			Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);
			return cropBmp;
		}
		ArrayList GetImageList(ArrayList images) {
			if(images != null) {
				imageListCore.Add(images[0]);
				imageListCore.Add(CatImage((Image)images[1], new Rectangle(3, 3, 46, 15)));
				imageListCore.Add(CatImage((Image)images[2], new Rectangle(0, 0, 14, 21)));
			}
			return imageListCore;
		}
		protected internal ArrayList ImageList {
			get {
				if(imageListCore.Count == 0) {
					return GetImageList(CheckImages);
				}
				return imageListCore;
			}
			set {
				if(ImageList == value) return;
				imageListCore = value;
				OnPropertiesChanged();
			}			
		}
		bool showTextCore = true;
		[ DefaultValue(true), DXCategory(CategoryName.Behavior), SmartTagProperty("Show Text", "", 1, SmartTagActionType.RefreshBoundsAfterExecute)]	   
		public bool ShowText {
			get { return showTextCore; }
			set {
				if(ShowText == value) return;
				showTextCore = value;
				OnPropertiesChanged();
			}
		}
		static readonly object toggledCore = new object();
		public event EventHandler Toggled {
			add { Events.AddHandler(toggledCore, value); }
			remove { Events.RemoveHandler(toggledCore, value); }
		}
		protected override void RaiseEditValueChangedCore(EventArgs e) {
			base.RaiseEditValueChangedCore(e);
			RaiseToggled(EventArgs.Empty);
		}
		protected virtual void RaiseToggled(EventArgs e) {
			EventHandler handler = Events[toggledCore] as EventHandler;
			if(handler != null)
				handler(GetEventSender(), e);
		}
		public override void Assign(RepositoryItem item) {
			BeginUpdate();
			try {
				base.Assign(item);
				RepositoryItemToggleSwitch source = item as RepositoryItemToggleSwitch;
				if(source == null) return;
				OnText = source.OnText;
				OffText = source.OffText;
				ImageList = source.ImageList;
				AllowThumbAnimation = source.AllowThumbAnimation;
				Events.AddHandler(toggledCore, source.Events[toggledCore]);
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual ImageCollection GetImageCollection() {
			return DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Images.ToggleSwitch.gif", typeof(RepositoryItemToggleSwitch).Assembly, new Size(52, 21), Color.Magenta);
		}
		protected override internal ArrayList CheckImages {
			get {
				if(fToggleImages == null) LoadImages(GetImageCollection(), ref fToggleImages);
				return fToggleImages;
			}
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			if(GetExportMode() == ExportMode.DisplayText) {
				return CreateTextBrick(info);
			}
			if(GlyphAlignment == HorzAlignment.Center || GlyphAlignment == HorzAlignment.Default) {
				ToggleSwitchBrick checkBrick = new XEToggleSwitchBrick(CreateBrickStyle(info, "toggleSwitch"));
				checkBrick.CheckText = info.DisplayText == "True" ? "On" : "Off";
				checkBrick.ImageList = ImageList;				
				checkBrick.IsOn = IsEquals(info.EditValue, true);				
				return checkBrick;
			}
			ToggleSwitchTextBrick checkTextBrick = new XEToggleSwitchTextBrick(CreateBrickStyle(info, "toggleSwitch"));
			checkTextBrick.Text = info.DisplayText == "True" ? "On" : "Off";
			checkTextBrick.CheckText = Caption;
			checkTextBrick.CheckBoxAlignment = GlyphAlignment;
			checkTextBrick.ImageList = ImageList;
			checkTextBrick.IsOn = IsEquals(info.EditValue, true);
			return checkTextBrick;
		}
	}
}
namespace DevExpress.XtraEditors {
	[DefaultEvent("Toggled"), DXToolboxItem(DXToolboxItemKind.Free), DefaultProperty("IsOn"),
	DefaultBindingPropertyEx("IsOn"),
	Designer("DevExpress.XtraEditors.Design.ToggleSwitchDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	Description("A switch that can be toggled between two states."),
	ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagAction(typeof(ToggleSwitchActions), "On", "Switch On", SmartTagActionType.RefreshContentAfterExecute), SmartTagAction(typeof(ToggleSwitchActions), "Off", "Switch Off", SmartTagActionType.RefreshContentAfterExecute), SmartTagFilter(typeof(ToggleSwitchFilter)),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "ToggleSwitch")
	]
	public class ToggleSwitch : BaseCheckEdit {				
		public ToggleSwitch() : base() { }		
		internal object AnimationId = new object();
		#region NewPropertiesEvents
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ToggleSwitchToggled"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler Toggled {
			add { Properties.Toggled += value; }
			remove { Properties.Toggled -= value; }
		}
		#endregion
		public override void Toggle() {
			if(Properties.ReadOnly) return;
			if(GetValidationCanceled()) return;
			if(ViewInfo.MouseButtons == System.Windows.Forms.MouseButtons.None) IsOn = !IsOn;
			else {
				EditValue = ViewInfo.GetValue();
				ViewInfo.EndDrag(new DragInfoEventArgs(ViewInfo, Point.Empty));
			}
		}
		protected new ToggleSwitchViewInfo ViewInfo { get { return base.ViewInfo as ToggleSwitchViewInfo; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ToggleSwitchIsOn"),
#endif
 DXCategory(CategoryName.Appearance), RefreshProperties(RefreshProperties.All), DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsOn {
			get {
				return Properties.IsEquals(EditValue, true);
			}
			set {
				if(IsOn != value) {
					EditValue = value;					
					OnPropertiesChanged();
				}
			}
		}
		protected override bool CanAnimateCore { get { return true; } }
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override string Text {
			get { return Properties.Caption; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(false)]
		public override object EditValue {
			get { return base.EditValue; }
			set { 
				base.EditValue = value;
				OnPropertiesChanged();
			}
		}
		public override string EditorTypeName {
			get { return "ToggleSwitch"; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ToggleSwitchProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemToggleSwitch Properties {
			get { return base.Properties as RepositoryItemToggleSwitch; }
		}
		protected override Size DefaultSize { get { return new Size(95, 20); } }
		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseMove(e);
			ViewInfo.Drag(new DragInfoEventArgs(ViewInfo, e.Location));
		}
		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) {
			base.OnMouseDown(e);
			if(Properties.ReadOnly) return;
			ViewInfo.BeginDrag(new DragInfoEventArgs(ViewInfo, e.Location));
		}
		protected internal override void RemoveXtraAnimator() {
			if(XtraAnimator.Current.Animations.Count == 0) return;
			AnimationInfoCollection collection = XtraAnimator.Current.Animations;
			foreach(BaseAnimationInfo animationInfo in collection) {
				if(animationInfo is ToggleAnimationInfo) continue;
				XtraAnimator.RemoveObject(this, animationInfo.AnimatedObject);
			}
		}
		protected override void DestroyHandle() {					  
			base.DestroyHandle();
			ViewInfo.StopAnimation();  
		}
		protected override  bool CanUseMnemonic(char charCode) {
			return System.Windows.Forms.Control.IsMnemonic(charCode, IsOn ? Properties.OnText : Properties.OffText ) && this.CanSelect;
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class ToggleSwitchViewInfo : BaseCheckEditViewInfo, IToggleAnimationInfo, IDragInfo {
		bool isOnCore;
		int step;
		int vectMove;
		bool isAnimationCore;
		Point savePositionSwitch;
		public ToggleSwitchViewInfo(RepositoryItem item)
			: base(item) {
			vectMove = step = 0;
		}
		public virtual bool IsOn {
			get { return isOnCore; }
			set {
				if(IsOn != value) isOnCore = value;					   
			}
		}
		protected internal int GetIntProperty(string propertyName) {
			return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinToggleSwitch].Properties.GetInteger(propertyName);
		}
		int GetSwitchWidth() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return GetIntProperty("SwitchWidth");
			return 30;
		}
		int GetTextMargin() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return GetIntProperty("TextMargin");
			return 2;
		}
		public new ToggleObjectInfoArgs CheckInfo { get { return base.CheckInfo as ToggleObjectInfoArgs; } }
		public override EditHitInfo CalcHitInfo(Point p) {
			EditHitInfo hi = CreateHitInfo(p);
			if(InfoBounds.Contains(p) || IsDrag)
				hi.SetHitTest(EditHitTest.Bounds);
			return hi;
		}
		protected virtual Rectangle InfoBounds {
			get {
				return CheckInfo.Bounds;
			}
		}
		public object GetValue() {
			SetVectorMove();
			if(Item.AllowThumbAnimation) return EditValue;
			return vectMove == -1 ? false : true;
		}
		protected internal Size GetSizeProperty() {
			if(LookAndFeel.UseDefaultLookAndFeel == false && LookAndFeel.Style == LookAndFeelStyle.Skin)
				return EditorsSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinToggleSwitch].Size.MinSize;
			return new Size(10, 10);
		}
		protected override void UpdateCheckProperties(BaseCheckObjectInfoArgs e) {
			base.UpdateCheckProperties(e);
			ToggleObjectInfoArgs args = e as ToggleObjectInfoArgs;
			args.IsOn = IsOn;
			args.ImageList = Item.ImageList;
			args.DefaultImages = Item.CheckImages;
			args.Caption = args.IsOn ? Item.OnText : Item.OffText;
			args.TextOff = Item.OffText;
			args.TextOn = Item.OnText;
			args.ShowText = Item.ShowText;
			args.MinSize = GetSizeProperty();
			args.SwitchWidth = GetSwitchWidth();
			args.TextMargin = GetTextMargin();
			args.Step = Step;
			args.IsDrag = IsDrag;
			args.IsAnimation = IsAnimation;
		}
		public override bool DrawFocusRect {
			get {
				if(OwnerEdit != null) {
					bool ret = base.DrawFocusRect;
					if(!Item.ShowText) ret = false;
					return ret;
				}
				return false;
			}
		}
		new RepositoryItemToggleSwitch Item {
			get { return base.Item as RepositoryItemToggleSwitch; }
		}
		public override bool IsSupportFastViewInfo {
			get { return false; }
		}
		protected override BaseCheckObjectInfoArgs CreateCheckArgs() {
			return new ToggleObjectInfoArgs(PaintAppearance);
		}
		protected override BaseCheckObjectPainter CreateCheckPainter() {
			if(IsPrinting) return TogglePainterHelper.GetPainter(ActiveLookAndFeelStyle.Flat);
			return TogglePainterHelper.GetPainter(LookAndFeel);
		}
		protected override void OnBeforeCheckStateChanged(BaseCheckObjectInfoArgs e, ObjectState newState) {
			ToggleObjectInfoArgs args = e as ToggleObjectInfoArgs;
			args.ImageList = Item.ImageList;
			base.OnBeforeCheckStateChanged(args, newState);
		}
		protected override void OnEditValueChanged() {
			IsOn = Item.IsEquals(EditValue, true);
			CheckInfo.IsOn = IsOn;
			CheckInfo.Caption = IsOn ? Item.OnText : Item.OffText;
			base.OnEditValueChanged();
		}
		void SetVectorMove() {
			vectMove = CalcVectorMove();
		}
		protected virtual int CalcVectorMove() {
			int vect = CheckInfo.IsOn ? -1 : 1;
			if(savePositionSwitch.X != CheckInfo.SwitchRect.X && IsDrag) {
				if(vect > 0 && CheckInfo.SwitchRect.X < (CheckInfo.GlyphRect.X + CheckInfo.GlyphRect.Width / 4))
					return -1;
				if(vect < 0 && (CheckInfo.SwitchRect.X + CheckInfo.SwitchRect.Width) > (CheckInfo.GlyphRect.X + 3 * CheckInfo.GlyphRect.Width / 4))
					return 1;
			}
			return vect;
		}
		int Step {
			get {
				if(!IsDrag && !IsAnimation) 
					step = 0;
				return step;
			}
		}
		protected virtual void StartAnimation() {
			XtraAnimator.Current.AddAnimation(new ToggleAnimationInfo(this));
		}
		public virtual void StopAnimation() {
			XtraAnimator.RemoveObject(OwnerEdit, (OwnerEdit as ToggleSwitch).AnimationId);
		}
		public override bool AllowScaleWidth { get { return false; } }
		#region IAnimationInfo Members
		double IToggleAnimationInfo.Speed { get { return 5; } }
		int IToggleAnimationInfo.TimeAnimation { get { return 200000; } }
		int IToggleAnimationInfo.Length {
			get {
				return vectMove < 0 ? CheckInfo.SwitchRect.Left - CheckInfo.GlyphRect.Left : CheckInfo.GlyphRect.Right - CheckInfo.SwitchRect.Right;
			}
		}		
		int IToggleAnimationInfo.VectorMove { get { return vectMove; } }		
		ToggleSwitch IToggleAnimationInfo.Owner { get { return this.OwnerEdit as ToggleSwitch; } }
		object IToggleAnimationInfo.AnimationId { get { return (this.OwnerEdit as ToggleSwitch).AnimationId; } }		
		public bool IsAnimation { get { return isAnimationCore; } }
		void IToggleAnimationInfo.OnStartAnimation() {
			isAnimationCore = true;
		}
		void IToggleAnimationInfo.OnEndAnimation(bool isFinalFrame) {
			isAnimationCore = false;
			if(isFinalFrame) OwnerEdit.Refresh();
		}
		void IToggleAnimationInfo.SetStep(int value) { step = value; }
		#endregion
		#region IDragInfo Members
		public void BeginDrag(DragInfoEventArgs e) {
			StopAnimation();
			if(e.Info.CanDrag(e.Point)) {
				e.Info.PreviousPosition = e.Point;
				savePositionSwitch = CheckInfo.SwitchRect.Location;
				IsDrag = true;
			}
		}
		public void EndDrag(DragInfoEventArgs e) {
			if(Item.AllowThumbAnimation)
				StartAnimation();
			if(e.Info.IsDrag)
				IsDrag = false;
			OwnerEdit.Refresh();
		}
		public void Drag(DragInfoEventArgs e) {
			if(e.Info.IsDrag) {				
				step = e.Point.X - e.Info.PreviousPosition.X;
				e.Info.PreviousPosition = e.Point;
				Item.LayoutChanged();
			}
		}
		Point IDragInfo.PreviousPosition { get; set; }
		bool IDragInfo.CanDrag(Point p) {
			return CheckInfo.SwitchRect.Contains(p);
		}
		public bool IsDrag { get; set; }
		#endregion
	}
	public interface IDragInfo {
		void BeginDrag(DragInfoEventArgs e);
		void EndDrag(DragInfoEventArgs e);
		void Drag(DragInfoEventArgs e);
		Point PreviousPosition { get; set; }
		bool CanDrag(Point p);
		bool IsDrag { get; set; }
	}
	public interface IToggleAnimationInfo {		
		int TimeAnimation { get; }
		int Length { get; }		
		int VectorMove { get; }
		ToggleSwitch Owner { get; }
		object AnimationId { get; }
		double Speed { get; }
		void OnStartAnimation();
		void OnEndAnimation(bool isFinalFrame);
		void SetStep(int value);
		bool IsAnimation { get; }
	}
	public class DragInfoEventArgs {
		IDragInfo infoCore;
		Point point;
		public DragInfoEventArgs(IDragInfo info, Point p) {
			infoCore = info;
			point = p;
		}
		public Point Point { get { return point; } }
		public IDragInfo Info { get { return infoCore; } }
	}	 
	public class ToggleAnimationInfo : BaseAnimationInfo {
		IToggleAnimationInfo infoCore;		
		public ToggleAnimationInfo(IToggleAnimationInfo toggleInfo)
			: base(toggleInfo.Owner, toggleInfo.AnimationId, toggleInfo.TimeAnimation, (int)Math.Ceiling(toggleInfo.Length / toggleInfo.Speed)) {	  
			infoCore = toggleInfo;			
			OnStartAnimation();			
		}
		public virtual void OnStartAnimation(){			
			infoCore.OnStartAnimation();
			Owner.IsOn = infoCore.VectorMove == 1;			
		}
		public virtual void OnEndAnimation() {						
			infoCore.OnEndAnimation(IsFinalFrame);			
		}
		public ToggleSwitch Owner { get { return infoCore.Owner; } }
		public override void Dispose() {
			OnEndAnimation();
			base.Dispose();
		}		
		public override void FrameStep() {			
			infoCore.SetStep(Step);
			Owner.Refresh();
		}
		public int Step { get { return (int)Math.Ceiling(infoCore.VectorMove * infoCore.Speed); } }	
	}
}
namespace DevExpress.XtraEditors.Drawing {
   public class ToggleSwitchPainter : BaseEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawCheck(info);
		}
		public override void Draw(ControlGraphicsInfoArgs info) {
			base.Draw(info);
		}
		protected virtual void DrawCheck(ControlGraphicsInfoArgs info) {
			BaseCheckEditViewInfo vi = info.ViewInfo as BaseCheckEditViewInfo;
			vi.CheckInfo.Cache = info.Cache;
			vi.CheckInfo.IsDrawOnGlass = info.IsDrawOnGlass;
			Rectangle r = vi.CheckInfo.CaptionRect;
			try {	 
					vi.CheckPainter.DrawObject(vi.CheckInfo);
			}
			finally {
				vi.CheckInfo.Cache = null;
				vi.CheckInfo.CaptionRect = r;
			}
		}
	}
}
