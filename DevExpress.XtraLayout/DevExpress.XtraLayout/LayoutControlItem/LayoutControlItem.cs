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
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.Customization.Controls;
namespace DevExpress.XtraLayout {
	public class LayoutItem : BaseLayoutItem {
		internal LayoutItem(LayoutGroup parent) : base(parent) { }
		protected bool allowHotTrackCore = true;
		protected override void CloneCommonProperties(LayoutGroup parent, ILayoutControl owner, ref BaseLayoutItem clone) {
			base.CloneCommonProperties(parent, owner, ref clone);
			LayoutItem item = (LayoutItem)clone;
			item.allowHotTrackCore = true;
		}
	}
	[DesignTimeVisible(false)]
	[DesignerCategory("Component")]
	public class LayoutControlItem : LayoutItem {
		bool settingControlBounds = false;
		bool settingEnableState = false;
		ContentAlignment controlAlignmentCore = ContentAlignment.TopLeft;
		bool contentVisibleCore = true;
		SizeConstraintsType sizeConstraintsType = SizeConstraintsType.Default;
		Size savedMin;
		Size savedMax;
		public LayoutControlItem()
			: this(null) {
			textLocationCore = Locations.Default;
			UpdateCachedControlConstraints();
		}
		public LayoutControlItem(LayoutControl layoutControl, Control control)
			: this(null) {
			layoutControl.Root.AddCore(this);
			BeginInit();
			if(control != null)
				if(!Owner.Control.Controls.Contains(control)) Owner.Control.Controls.Add(control);
			Control = control;
			EndInit();
			ComplexUpdate();
		}
		public LayoutControlItem(LayoutControlGroup parent) : base(parent) { }
		[DefaultValue(true), Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Expanded {
			get { return base.Expanded; }
			set { }
		}
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TypeName {
			get { return "LayoutControlItem"; }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ViewInfo.LayoutControlItemViewInfo ViewInfo {
			get { return base.ViewInfo as ViewInfo.LayoutControlItemViewInfo; }
		}
		string controlName;
		[XtraSerializableProperty(), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual string ControlName {
			get { return controlName; }
			set { controlName = value; }
		}
		bool allowHtmlStringInCaptionCore = false;
		[XtraSerializableProperty(), DefaultValue(false)]
		public virtual bool AllowHtmlStringInCaption {
			get { return allowHtmlStringInCaptionCore; }
			set {
				if(value != AllowHtmlStringInCaption) {
					allowHtmlStringInCaptionCore = value;
					UpdateText();
				}
			}
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlItemOwner")]
#endif
		public override ILayoutControl Owner {
			get { return base.Owner; }
			set {
				base.Owner = value;
				if(Owner != null) {
					if(Control != null) {
						if(!Owner.Control.Controls.Contains(Control)) {
							Control.Visible = Visible;
							if(Control == null) return;
							Control.Parent = Owner.Control;
						}
					}
				}
			}
		}
		internal void XtraResetTextLocation() {
			textLocationCore = Locations.Default;
		}
		internal static void InitializeItemAsFixed(LayoutControlItem item, ILayoutControl owner) {
			if(item == null) return;
			string itemName = (owner != null) ? owner.GetUniqueName(item) : ("LayoutItem" + new Guid().ToString());
			string controlName = itemName + "Control";
			item.Name = itemName;
			Control control = (item as IFixedLayoutControlItem).OnCreateControl();
			if(control != null) {
				control.Name = controlName;
				item.Control = control;
				item.originalEnabledInitialized = true;
				item.originalEnabled = true;
			}
			(item as IFixedLayoutControlItem).OnInitialize();
		}
		internal static void DestroyItemAsFixed(LayoutControlItem item) {
			if(item == null) return;
			(item as IFixedLayoutControlItem).OnDestroy();
			item.Dispose();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(disposingFlagCore) return;
				try {
					using(new SafeBaseLayoutItemChanger(this)) {
						Control controlRef = control;
						RemoveControl();
						if(Parent != null) Parent.Remove(this);
						disposingFlagCore = true;
						if(controlRef != null) {
							RemoveControl();
						}
						if(viewInfoCore != null) viewInfoCore.Destroy();
						viewInfoCore = null;
						controlRef = null;
					}
				}
				catch { }
			}
			base.Dispose(disposing);
		}
		protected override ViewInfo.BaseLayoutItemViewInfo CreateViewInfo() {
			return new ViewInfo.LayoutControlItemViewInfo(this);
		}
		internal PaintingType GetPaintingType() {
			if(Owner == null) return PaintingType.Normal;
			return (Owner as ISupportImplementor).Implementor.PaintingType;
		}
		protected override bool ShouldSerializeMaxSize() {
			if(SizeConstraintsType == SizeConstraintsType.Default || SizeConstraintsType == SizeConstraintsType.SupportHorzAlignment) return false;
			return base.ShouldSerializeMaxSize();
		}
		protected bool ShouldSerializeControlMinSize() {
			return false;
		}
		protected bool ShouldSerializeControlMaxSize() {
			return false;
		}
		protected override bool ShouldSerializeMinSize() {
			if(SizeConstraintsType == SizeConstraintsType.Default || SizeConstraintsType == SizeConstraintsType.SupportHorzAlignment) return false;
			return base.ShouldSerializeMinSize();
		}
		protected override bool GetTextVisible() {
			return TextVisible || (Owner != null ? (
				Owner.OptionsItemText.TextAlignMode != DevExpress.XtraLayout.TextAlignMode.CustomSize &&
				TextAlignMode != TextAlignModeItem.CustomSize &&
				((ILayoutControl)Owner).TextAlignManager.GetAlignHiddenText(this)) : false);
		}
		protected override bool ShouldSerializeTextToControlDistance() {
			if(TextAlignMode == TextAlignModeItem.CustomSize) return true;
			if(TextAlignMode == TextAlignModeItem.UseParentOptions) return false;
			return base.ShouldSerializeTextToControlDistance();
		}
		protected virtual bool CanOptimizeTextCalculation() {
			return TextAlignMode == TextAlignModeItem.AutoSize || TextAlignMode == TextAlignModeItem.CustomSize;
		}
		TextAlignModeItem textAlignMode = TextAlignModeItem.UseParentOptions;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemTextAlignMode"),
#endif
 DefaultValue(TextAlignModeItem.UseParentOptions)]
		[RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		public virtual TextAlignModeItem TextAlignMode {
			get { return textAlignMode; }
			set {
				using(new SafeBaseLayoutItemChanger(this)) {
					textAlignMode = value;
					ShouldArrangeTextSize = true;
					shouldResetBorderInfoCore = true;
					ComplexUpdate(true, true);
				}
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use TextAlignMode instead")]
		public virtual bool AllowAutoAlignment {
			get { return TextAlignMode != TextAlignModeItem.CustomSize; }
			set {
				if(value) TextAlignMode = TextAlignModeItem.UseParentOptions;
				else
					TextAlignMode = TextAlignModeItem.CustomSize;
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemSizeConstraintsType"),
#endif
 DefaultValue(SizeConstraintsType.Default)]
		[RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		public virtual SizeConstraintsType SizeConstraintsType {
			get { return sizeConstraintsType; }
			set {
				if(SizeConstraintsType == value && IsInitializing) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					BeginInit();
					if(value == SizeConstraintsType.Custom && Owner != null && Owner.DesignMode && !Owner.IsDeserializing) {
						base.MaxSize = MaxSize;
						base.MinSize = MinSize;
					}
					sizeConstraintsType = value;
					EndInit();
					if(Owner != null) Owner.ShouldUpdateConstraints = true;
					SetShouldUpdateViewInfo();
					ComplexUpdate();
					if(Owner != null) Owner.SetIsModified(true);
				}
			}
		}
		protected bool GetControlRealEnabledState() { 
			return GetControlState(4); 
		}
		protected bool GetControlRealVisibleState() { 
			return GetControlState(2); 
		}
		protected bool GetControlState(int state) {
			return GetControlState(Control, state);
		}
		protected static bool GetControlRealEnabledState(Control control) {
			return GetControlState(control, 4);
		}
		protected static bool GetControlRealVisibleState(Control control) {
			return GetControlState(control, 2);
		}
		protected static bool GetControlState(Control control, int state) {
			bool result = true;
			if(control != null) {
				MethodInfo mi = typeof(Control).GetMethod("GetState", BindingFlags.Instance | BindingFlags.NonPublic);
				if(mi != null) 
					result = (bool)mi.Invoke(control, new object[] { state });
			}
			return result;
		}
		protected Size GetControlConstraints(Size size) {
			if(IsDisposing) return size;
			Size paddingAndTextSize = ViewInfo.AddLabelIndentions(Size.Empty);
			if(size.Width != 0) {
				size.Width -= paddingAndTextSize.Width;
			}
			if(size.Height != 0) {
				size.Height -= paddingAndTextSize.Height;
			}
			return size;
		}
		protected Size SetControlConstraints(Size size) {
			Size paddingAndTextSize = ViewInfo.AddLabelIndentions(Size.Empty);
			if(size.Width != 0) {
				size.Width += paddingAndTextSize.Width;
			}
			if(size.Height != 0) {
				size.Height += paddingAndTextSize.Height;
			}
			return size;
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemControlMaxSize"),
#endif
 RefreshProperties(RefreshProperties.All)]
		public virtual Size ControlMaxSize {
			get { return GetControlConstraints(MaxSize); }
			set { MaxSize = SetControlConstraints(value); }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemControlMinSize"),
#endif
 RefreshProperties(RefreshProperties.All)]
		public virtual Size ControlMinSize {
			get { return GetControlConstraints(MinSize); }
			set { MinSize = SetControlConstraints(value); }
		}
		protected internal Size CalcItemImageSize(Image image, object imageList, int imageIndex) {
			return ImageCollection.GetImageListSize(image, imageList, imageIndex);
		}
		protected internal Size CalcItemTextAndImageSize(Size textSize, Size imageSize, ContentAlignment alignment) {
			Size size = textSize;
			if(!imageSize.IsEmpty) {
				switch(alignment) {
					case ContentAlignment.BottomRight:
					case ContentAlignment.TopRight:
					case ContentAlignment.TopLeft:
					case ContentAlignment.BottomLeft:
						size = new Size(size.Width + imageSize.Width + ImageToTextDistance, size.Height + imageSize.Height);
						break;
					case ContentAlignment.MiddleRight:
					case ContentAlignment.MiddleLeft:
						size = new Size(textSize.Width + imageSize.Width + ImageToTextDistance, Math.Max(textSize.Height, imageSize.Height));
						break;
					case ContentAlignment.TopCenter:
					case ContentAlignment.BottomCenter:
						size = new Size(Math.Max(textSize.Width, imageSize.Width), textSize.Height + imageSize.Height + ImageToTextDistance);
						break;
				}
			}
			return size;
		}
		protected Size defaultMinSize = new Size(1, 1);
		protected Size GetDefaultMinMaxSize(bool getMin) {
			if(getMin) return defaultMinSize;
			else return Size.Empty;
		}
		protected virtual Size GetContentMinMaxSize(bool getMin) {
			return ConstraintsManager.Default.GetMinMaxSize(this.Control, getMin);
		}
		protected internal virtual Size CalcDefaultMinMaxSize(bool getMin) {
			return CalcDefaultMinMaxSize(getMin, true);
		}
		protected bool IsDefaultMinConstraint(Size minSize) {
			Size defMinSize = ConstraintsManager.Default.GetDefaultMinSize();
			return minSize == defMinSize;
		}
		protected virtual Size CalcSizeWithLabel(Size size, Size labelSize) {
			Size result = size;
			switch(TextLocation) {
				case Locations.Default:
				case Locations.Left:
				case Locations.Right:
					if(result.Height != 0) result.Height = Math.Max(size.Height, labelSize.Height);
					break;
				case Locations.Top:
				case Locations.Bottom:
					if(result.Width != 0) result.Width = Math.Max(size.Width, labelSize.Width);
					break;
			}
			return result;
		}
		protected internal Size CalcDefaultMinMaxSize(bool getMin, bool addLabelIndentions) {
			Size size = GetContentMinMaxSize(getMin);
			if(!addLabelIndentions || IsDisposing) return size;
			Size labelSize = GetLabelSize();
			size = CalcSizeWithLabel(size, labelSize);
			if(getMin) {
				return ViewInfo.AddLabelIndentions(size);
			}
			else {
				Size labelIndentionsSize = ViewInfo.AddLabelIndentions(size);
				if(size.Width != 0)
					size.Width = labelIndentionsSize.Width;
				if(size.Height != 0)
					size.Height = labelIndentionsSize.Height;
				return size;
			}
		}
		Image layoutItemLabelImage = null;
		int layoutItemLabelImageIndex = -1;
		ContentAlignment layoutItemLabelImageAlignment = ContentAlignment.MiddleLeft;
		int layoutItemImageToTextDistance = 5;
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlItemImage")]
#endif
		[XtraSerializableProperty(), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Image {
			get { return layoutItemLabelImage; }
			set {
				if(layoutItemLabelImage == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					layoutItemLabelImage = value;
					ShouldUpdateConstraints = true;
					ComplexUpdate(true, true, true);
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Images {
			get { return (Owner != null) ? Owner.Images : null; }
			set { if(Owner != null) Owner.Images = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ImageCollection ItemImages { get { return Owner == null ? null : Owner.Images as ImageCollection; } }
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlItemImageIndex")]
#endif
		[XtraSerializableProperty(), DefaultValue(-1)]
		[Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)), ImageListAttribute("Images")]
		public virtual int ImageIndex {
			get { return layoutItemLabelImageIndex; }
			set {
				if(layoutItemLabelImageIndex == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					layoutItemLabelImageIndex = value;
					ShouldUpdateConstraints = true;
					ComplexUpdate(true, true, true);
				}
			}
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlItemImageAlignment")]
#endif
		[XtraSerializableProperty(), DefaultValue(ContentAlignment.MiddleLeft)]
		public virtual ContentAlignment ImageAlignment {
			get { return layoutItemLabelImageAlignment; }
			set {
				if(layoutItemLabelImageAlignment == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					layoutItemLabelImageAlignment = value;
					ShouldUpdateConstraints = true;
					ComplexUpdate(true, true, true);
				}
			}
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlItemImageToTextDistance")]
#endif
		[XtraSerializableProperty(), DefaultValue(5)]
		public virtual int ImageToTextDistance {
			get { return layoutItemImageToTextDistance; }
			set {
				if(layoutItemImageToTextDistance == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					layoutItemImageToTextDistance = value;
					ShouldArrangeTextSize = true;
					ShouldUpdateConstraints = true;
					ComplexUpdate(true, true);
				}
			}
		}
		OptionsPrintItem optionsPrintCore;
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlItemOptionsPrint")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public virtual OptionsPrintItem OptionsPrint {
			get {
				if(optionsPrintCore == null) optionsPrintCore = new OptionsPrintItem(this);
				return optionsPrintCore;
			}
			set {
				optionsPrintCore = value;
			}
		}
		protected internal override void SetShouldUpdateViewInfo() {
			base.SetShouldUpdateViewInfo();
			needRecalcMinSize = true;
			needRecalcMaxSize = true;
		}
		protected Size calculatedMinSize, calculatedMaxSize;
		protected bool needRecalcMinSize = true;
		protected bool needRecalcMaxSize = true;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemMaxSize"),
#endif
 RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		public override Size MaxSize {
			get {
				if(SizeConstraintsType == SizeConstraintsType.Custom) 
					return CalcScaledSize(base.MaxSize);
				if(SizeConstraintsType == SizeConstraintsType.Default) {
					if(needRecalcMaxSize) { calculatedMaxSize = CalcDefaultMinMaxSize(false);}
					return calculatedMaxSize;
				}
				if(SizeConstraintsType == SizeConstraintsType.SupportHorzAlignment) {
					if(needRecalcMaxSize) { calculatedMaxSize = new Size(0, CalcDefaultMinMaxSize(false).Height);}
					return calculatedMaxSize;
				}
				return Size.Empty;
			}
			set {
				SetShouldUpdateViewInfo();
				if(Owner != null) Owner.ShouldUpdateConstraints = true;
				if(SizeConstraintsType == SizeConstraintsType.Custom)
					value = CalcDeScaledSize(value);
				base.MaxSize = value;
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemMinSize"),
#endif
 RefreshProperties(RefreshProperties.All)]
		[XtraSerializableProperty()]
		public override Size MinSize {
			get {
				if(SizeConstraintsType == SizeConstraintsType.Custom) 
					return CalcScaledSize(base.MinSize);
				if(SizeConstraintsType == SizeConstraintsType.Default || SizeConstraintsType == SizeConstraintsType.SupportHorzAlignment) {
					if(needRecalcMinSize) { calculatedMinSize = CalcDefaultMinMaxSize(true);}
					return calculatedMinSize;
				}
				return Size.Empty;
			}
			set {
				SetShouldUpdateViewInfo();
				if(Owner != null) Owner.ShouldUpdateConstraints = true;
				if(SizeConstraintsType == SizeConstraintsType.Custom) 
					value = CalcDeScaledSize(value);
				base.MinSize = value;
			}
		}
		Control control;
		protected internal void RemoveControl() {
			if(control != null) {
				UnSubscribeControlEvents();
				ISupportLookAndFeel lf = control as ISupportLookAndFeel;
				if(lf != null && lf.LookAndFeel != null) lf.LookAndFeel.ParentLookAndFeel = null;
				SetConstraintsCore(Size.Empty, Size.Empty);
				ConstraintsManager.Default.UpdateIResizableConstraints(control);
				control = null;
			}
		}
		protected bool CheckIfControlIsInLayout(Control control) {
			if(Owner != null) return Owner.CheckIfControlIsInLayout(control);
			else
				return false;
		}
		protected virtual void SubscribeControlEvents() {
			control.SizeChanged += new EventHandler(OnControlLayout);
			control.VisibleChanged += new EventHandler(OnControlVisibleChanged);
			control.LocationChanged += new EventHandler(OnControlLayout);
			control.Disposed += new EventHandler(OnControlDisposed);
			control.EnabledChanged += new EventHandler(OnControlEnabledChanged);
			control.GotFocus += new EventHandler(OnControlGotFocus);
			control.LostFocus += new EventHandler(OnControlLostFocus);
			control.MouseEnter += new EventHandler(OnControlMouseEnter);
			control.MouseLeave += new EventHandler(OnControlMouseLeave);
			if(control is XtraUserControl) {
				control.ControlAdded += OnChildControlsRemovedOrAdded;
				control.ControlRemoved += OnChildControlsRemovedOrAdded;
			}
			if(control is TextBox) {
				((TextBox)control).MultilineChanged += new EventHandler(OnConstraintsChanged);
			}
			if(control is DevExpress.Utils.Controls.IXtraResizableControl)
				((DevExpress.Utils.Controls.IXtraResizableControl)control).Changed += new EventHandler(OnConstraintsChanged);
			control.Enter += new EventHandler(OnControlEnter);
			control.Leave += new EventHandler(OnControlLeave);
			if(control is TextBoxBase) {
				((TextBoxBase)control).ReadOnlyChanged += new EventHandler(LayoutControlItem_ReadOnlyChanged);
		}
			if(control is BaseEdit) {
				((BaseEdit)control).PropertiesChanged += new EventHandler(LayoutControlItem_PropertiesChanged);
			}
		}
		void OnChildControlsRemovedOrAdded(object sender, ControlEventArgs e) {
			if(Control != null) ConstraintsManager.Default.UpdateIResizableConstraints(Control);
		}
		void LayoutControlItem_PropertiesChanged(object sender, EventArgs e) {
			PropertyChangedEventArgs pchea = e as PropertyChangedEventArgs;
			if(pchea != null && pchea.PropertyName == "ReadOnly") {
				UpdateControl();
			}
		}
		void LayoutControlItem_ReadOnlyChanged(object sender, EventArgs e) {
			UpdateControl();
		}
		protected virtual void UnSubscribeControlEvents() {
			control.SizeChanged -= new EventHandler(OnControlLayout);
			control.VisibleChanged -= new EventHandler(OnControlVisibleChanged);
			control.LocationChanged -= new EventHandler(OnControlLayout);
			control.Disposed -= new EventHandler(OnControlDisposed);
			control.EnabledChanged -= new EventHandler(OnControlEnabledChanged);
			control.GotFocus -= new EventHandler(OnControlGotFocus);
			control.LostFocus -= new EventHandler(OnControlLostFocus);
			control.MouseEnter -= new EventHandler(OnControlMouseEnter);
			control.MouseLeave -= new EventHandler(OnControlMouseLeave);
			if(control is XtraUserControl) {
				control.ControlAdded -= OnChildControlsRemovedOrAdded;
				control.ControlRemoved -= OnChildControlsRemovedOrAdded;
			}
			if(control is TextBox) {
				((TextBox)control).MultilineChanged -= new EventHandler(OnConstraintsChanged);
			}
			if(control is DevExpress.Utils.Controls.IXtraResizableControl)
				((DevExpress.Utils.Controls.IXtraResizableControl)control).Changed -= new EventHandler(OnConstraintsChanged);
			control.Enter -= new EventHandler(OnControlEnter);
			control.Leave -= new EventHandler(OnControlLeave);
			if(control is TextBoxBase) {
				((TextBoxBase)control).ReadOnlyChanged -= new EventHandler(LayoutControlItem_ReadOnlyChanged);
			}
			if(control is BaseEdit) {
				((BaseEdit)control).PropertiesChanged -= new EventHandler(LayoutControlItem_PropertiesChanged);
			}
		}
		protected bool AllowChangeFocusedElement() {
			if(Owner == null) return false;
			if(Owner.FocusHelper == null) return false;
			if(Owner.RootGroup == null) return false;
			if(Owner.RootGroup.IsUpdateLocked) return false;
			return true;
		}
		protected virtual void OnControlEnter(object sender, EventArgs e) {
			Control controlToFocus = sender as Control;
			if(AllowChangeFocusedElement()) {
				Owner.UpdateFocusedElement(this);
			}
			if(controlToFocus != null) {
				if(AllowChangeFocusedElement()) {
					Owner.FocusHelper.PlaceItemIntoViewRestricted(this);
				}
				ComplexUpdate(false, false, false);
			}
		}
		protected internal override void UpdateChildren(bool visible) {
			base.UpdateChildren(visible);
		}
		protected virtual void OnControlLeave(object sender, EventArgs e) {
		}
		protected void CheckOneControlForManyItemsIssue(Control control) {
			if(Owner != null && control != null) {
				BaseLayoutItem item = Owner.GetItemByControl(control);
				if(item != null && item != this) throw new LayoutControlInternalException("Control named: " + control.Name + " already in layoutItem named:" +
					item.Name + ". Do not assign one control to many layout items");
			}
		}
		bool ShouldSerializeAllowHotTrack() { return allowHotTrackCore != true; }
		void ResetAllowHotTrack() { allowHotTrackCore = true; }
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlItemAllowHotTrack")]
#endif
		public virtual bool AllowHotTrack {
			get { return allowHotTrackCore && (Owner != null && Owner.OptionsView.AllowHotTrack); }
			set { allowHotTrackCore = value; }
		}
		bool fillControlToClientAreaCore = true;
		[DefaultValue(true)]
		public bool FillControlToClientArea {
			get { return fillControlToClientAreaCore; }
			set { fillControlToClientAreaCore = value; UpdateControl(); Invalidate(); }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemControlAlignment"),
#endif
 DefaultValue(ContentAlignment.TopLeft), XtraSerializableProperty()]
		public virtual ContentAlignment ControlAlignment {
			get { return controlAlignmentCore; }
			set { controlAlignmentCore = value; UpdateControl(); Invalidate(); }
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemContentVisible"),
#endif
 DefaultValue(true), XtraSerializableProperty(), Localizable(true)]
		public virtual bool ContentVisible {
			get { return contentVisibleCore; }
			set {
				contentVisibleCore = value;
				if(!IsUpdateLocked) UpdateControl();
				else if(Owner != null) Owner.ShouldUpdateControls = true;
				Invalidate();
			}
		}
		protected virtual void ValidateControl(Control control){
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemControl"),
#endif
 DefaultValue(null)]
		public virtual Control Control {
			get {
				return control;
			}
			set {
				using(new SafeBaseLayoutItemChanger(this)) {
					try {
						ValidateControl(value);
						if(!IsUpdateLocked && control != null) return;
						if(control != value) RemoveControl();
						else return;
						CheckOneControlForManyItemsIssue(value);
						control = value;
						UpdateCachedControlConstraints();
						if(Owner != null && CanCreateViewInfo) SetShouldUpdateViewInfo();
						if(control != null) {
#if(DXWhidbey)
							try {
								control.AutoSize = false;
							} catch { }
#endif
							ControlName = LayoutControl.GetControlName(control);
							control.Dock = DockStyle.None;
							SubscribeControlEvents();
							if(Owner != null && !Owner.IsDeserializing && !IsInitializing) TextVisible = ConstraintsManager.Default.TextVisible(control);
							ShouldUpdateConstraintsDoUpdate = true;
						}
					} finally {
						if(Owner != null) {
							if(this.IsHidden) (Owner as ISupportImplementor).Implementor.UpdateHiddenItems();
							Owner.Invalidate();
							if(control != null && control.Parent != Owner.Control) {
								UpdateControlParent();
							}
						}
					}
				}
			}
		}
		void UpdateControlParent() {
			LayoutTreeView dxTreeView = control as LayoutTreeView;
			if(dxTreeView != null && dxTreeView.AllowSkinning && dxTreeView.ContainerControl != null) {
				BeginInit();
				dxTreeView.ContainerControl.Name = dxTreeView.Name + "ScrollableContainer";
				Control = dxTreeView.ContainerControl;
				EndInit();
			}
			if(dxTreeView != null) dxTreeView.AllowSkinning = false;
			control.Parent = Owner.Control; ShouldUpdateConstraints = true; Owner.Invalidate();
			if(dxTreeView != null) dxTreeView.AllowSkinning = true;
		}
		void OnControlGotFocus(object sender, EventArgs e) {
			if(Owner != null && Owner.Control != null) {
				Owner.Control.Invalidate(ViewInfo.BoundsRelativeToControl);
				if(Owner.FocusHelper != null) Owner.FocusHelper.SelectedComponent = this;
			}
		}
		void OnControlLostFocus(object sender, EventArgs e) {
			Invalidate();
		}
		protected virtual void OnControlMouseEnter(object sender, EventArgs e) {
			if(!AllowHotTrack) return;
			Invalidate();
		}
		protected virtual void OnControlMouseLeave(object sender, EventArgs e) {
			if(!AllowHotTrack) return;
			Invalidate();
		}
		protected bool IsThereAnyDisposing() {
			return DisposingFlag || (Control != null && Control.Disposing);
		}
		int whatchDog = 0;
		protected virtual void OnConstraintsChanged(object sender, EventArgs ea) {
			if(Control != null) ConstraintsManager.Default.UpdateIResizableConstraints(Control);
			if(viewInfoCore != null)
				viewInfoCore.Destroy();
			viewInfoCore = null;
			Size tSize = Size;
			ShouldUpdateConstraintsDoUpdate = true;
			if(!IsUpdateLocked && Parent != null && !IsThereAnyDisposing()) {
				Parent.ChangeItemSize(this, tSize);
				ViewInfo.CalculateViewInfo();
				whatchDog++;
				if(whatchDog < 3) settingControlBounds = false;
				ComplexUpdate();
				whatchDog = 0;
			}
		}
		void OnControlEnabledChanged(object sender, EventArgs e) {
			if(settingControlBounds || settingEnableState) return;
			originalEnabledInitialized = false;
			InvalidateEnabledState();
			originalEnabled = GetControlRealEnabledState();
			originalEnabledInitialized = true;
			if(Owner != null) {
				Owner.ShouldUpdateControls = true;
				if(AllowHtmlStringInCaption) SetShouldUpdateViewInfo();
			}
			Invalidate();
		}
		void OnControlDisposed(object sender, EventArgs e) {
			using(new SafeBaseLayoutItemChanger(this)) {
				BeginInit();
				Control = null;
				EndInit();
				ILayoutControl owner = Owner;
				Dispose();
				Owner = owner;
			}
		}
		protected virtual void UpdateCachedControlConstraints() {
			if(Control != null) {
				savedMin = ConstraintsManager.Default.CorrectValueByDefaultNet20ControlMinSize(Control.MinimumSize);
				savedMax = Control.MaximumSize;
			}
			else {
				savedMin = new Size(20, 20);
				savedMax = Size.Empty;
			}
		}
		bool isCheckingNet20SizeConstraintsChanged = false;
		protected virtual bool CheckNet20SizeConstraintsChanged() {
			if(isCheckingNet20SizeConstraintsChanged) return false;
			if(Control == null) return false;
			isCheckingNet20SizeConstraintsChanged = true;
			Size actualMin = ConstraintsManager.Default.CorrectValueByDefaultNet20ControlMinSize(Control.MinimumSize);
			Size actualMax = Control.MaximumSize;
			if(actualMin == Size.Empty && actualMax == Size.Empty) return false;
			if(savedMin != actualMin || savedMax != actualMax) {
				savedMin = actualMin;
				savedMax = actualMax;
				OnConstraintsChanged(this, EventArgs.Empty);
				return true;
			}
			isCheckingNet20SizeConstraintsChanged = false;
			return false;
		}
		void OnControlLayout(object sender, EventArgs e) {
			if(settingControlBounds || !Visible || IsHidden || Owner == null) return;
			CheckNet20SizeConstraintsChanged();
			UpdateControl();
		}
		bool controlVisibleChanging = false;
		void OnControlVisibleChanged(object sender, EventArgs e) {
			if(Control != null && GetControlRealVisibleState() != (Visible && ContentVisible) && !controlVisibleChanging) {
				controlVisibleChanging = true;
				Control.Visible = Visible && ContentVisible;
				controlVisibleChanging = false;
			}
		}
		void BeginSetControlBounds() {
			settingControlBounds = true;
		}
		void EndSetControlBounds() {
			settingControlBounds = false;
		}
		Size CheckControlConstraints(Size size, Size maxSize, Size minSize, bool ignoreDefaultMinSize, bool allowIncreaseControlSize) {
			if(Control == null) return Size.Empty;
			Size controlSize = size;
			Size controlMaxSize = maxSize;
			Size controlMinSize = minSize;
			Size temporaryResult = Size.Empty;
			if(controlMaxSize.Width != 0 && controlSize.Width > controlMaxSize.Width)
				controlSize.Width = controlMaxSize.Width;
			if(controlMaxSize.Height != 0 && controlSize.Height > controlMaxSize.Height)
				controlSize.Height = controlMaxSize.Height;
			temporaryResult = controlSize;
			if(allowIncreaseControlSize) {
				if(controlMinSize.Width != 0 && controlSize.Width < controlMinSize.Width)
					controlSize.Width = controlMinSize.Width;
				if(controlMinSize.Height != 0 && controlSize.Height < controlMinSize.Height)
					controlSize.Height = controlMinSize.Height;
			}
			if(ignoreDefaultMinSize && IsDefaultMinConstraint(minSize))
				controlSize = temporaryResult;
			return controlSize;
		}
		protected virtual Size CalcControlSize() {
			Size controlSize = ViewInfo.ClientAreaRelativeToControl.Size;
			Size result = Size.Empty;
			if(controlSize.Width > ControlMaxSize.Width && MaxSize.Width != 0) {
				controlSize.Width = ControlMaxSize.Width;
			}
			if(controlSize.Height > ControlMaxSize.Height && MaxSize.Height != 0) {
				controlSize.Height = ControlMaxSize.Height;
			}
			result = controlSize;
			if(!FillControlToClientArea || SizeConstraintsType == SizeConstraintsType.SupportHorzAlignment) {
				if(Control != null)
					result = CheckControlConstraints(controlSize, Control.MaximumSize, Control.MinimumSize, false, false);
					result = CheckControlConstraints(controlSize, CalcDefaultMinMaxSize(false, false), CalcDefaultMinMaxSize(true, false), true, false);
			}
			return result;
		}
		protected int CalcCenterValue(int boundBegin, int boundSize, int itemSize) {
			return (boundBegin + ((boundSize - itemSize) / 2));
		}
		protected internal override bool TruncateClientAreaToMaxSize {
			get {
				return truncateClientAreaToMaxSizeCore;
			}
		}
		bool truncateClientAreaToMaxSizeCore = true;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemTrimClientAreaToControl"),
#endif
 RefreshProperties(RefreshProperties.All), DefaultValue(true)]
		[XtraSerializableProperty()]
		public bool TrimClientAreaToControl {
			get { return truncateClientAreaToMaxSizeCore; }
			set { truncateClientAreaToMaxSizeCore = value; ComplexUpdate(true, true, true); }
		}
		protected virtual Rectangle CalcBoundsToSet() {
			return CalcBoundsToSetCore();
		}
		protected virtual Rectangle CalcBoundsToSetCore() {
			Rectangle boundsToSet = ViewInfo.ClientAreaRelativeToControl;
			if(!Visible) return boundsToSet;
			boundsToSet.Size = CalcControlSize();
			Rectangle alignRect = ViewInfo.ClientAreaRelativeToControl;
			switch(ControlAlignment) {
				case ContentAlignment.TopCenter:
					boundsToSet.X = this.CalcCenterValue(alignRect.X, alignRect.Width, boundsToSet.Width);
					return boundsToSet;
				case (ContentAlignment.TopCenter | ContentAlignment.TopLeft):
					return boundsToSet;
				case ContentAlignment.TopRight:
					boundsToSet.X = alignRect.Right - boundsToSet.Width;
					return boundsToSet;
				case ContentAlignment.MiddleLeft:
					boundsToSet.Y = this.CalcCenterValue(alignRect.Y, alignRect.Height, boundsToSet.Height);
					return boundsToSet;
				case ContentAlignment.MiddleCenter:
					boundsToSet.Y = this.CalcCenterValue(alignRect.Y, alignRect.Height, boundsToSet.Height);
					boundsToSet.X = this.CalcCenterValue(alignRect.X, alignRect.Width, boundsToSet.Width);
					return boundsToSet;
				case ContentAlignment.BottomCenter:
					boundsToSet.X = this.CalcCenterValue(alignRect.X, alignRect.Width, boundsToSet.Width);
					boundsToSet.Y = alignRect.Bottom - boundsToSet.Height;
					return boundsToSet;
				case ContentAlignment.BottomRight:
					boundsToSet.Y = alignRect.Bottom - boundsToSet.Height;
					boundsToSet.X = alignRect.Right - boundsToSet.Width;
					return boundsToSet;
				case ContentAlignment.MiddleRight:
					boundsToSet.X = alignRect.Right - boundsToSet.Width;
					boundsToSet.Y = this.CalcCenterValue(alignRect.Y, alignRect.Height, boundsToSet.Height);
					return boundsToSet;
				case ContentAlignment.BottomLeft:
					boundsToSet.Y = alignRect.Bottom - boundsToSet.Height;
					return boundsToSet;
			}
			return boundsToSet;
		}
		protected internal override bool EnabledState {
			get {
				return originalEnabledInitialized ? base.EnabledState && originalEnabled : base.EnabledState;
			}
		}
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutControlItemEnabled"),
#endif
 DefaultValue(true)]
		public bool Enabled {
			get { return originalEnabled; }
			set {
				if(!originalEnabledInitialized) originalEnabledInitialized = true;
				if(originalEnabled == value) return;
				originalEnabled = value;
				InvalidateEnabledState();
				SetShouldUpdateViewInfo();
				shouldResetBorderInfoCore = true;
				ComplexUpdate(true, true);
			}
		}
		public override void EndInit() {
			if(Control != null && !originalEnabledInitialized) {
				originalEnabled = GetControlRealEnabledState();
				originalEnabledInitialized = true;
			}
			base.EndInit();
		}
		protected bool originalEnabled = true;
		protected bool originalEnabledInitialized = false;
		protected override void OnRTLChanged() {
			base.OnRTLChanged();
			shouldUpdateViewInfo = true;
			switch(TextLocation) {
				case Locations.Left:
					TextLocation = Locations.Right;
					break;
				case Locations.Right:
					TextLocation = Locations.Left;
					break;
				default:
					break;
			}
		}
		protected internal void UpdateControl() {
			Control controlRef = Control;  
			if(controlRef != null && !IsUpdateLocked) {
				bool controlVisibleStatus = false;
				bool shouldSetControlVisibleStatus = false;
				try {
					if(GetControlRealVisibleState(controlRef) != (Visible && ContentVisible) || IsHidden) {
						shouldSetControlVisibleStatus = true;
						controlVisibleStatus = Visible && ContentVisible;
					}
					Rectangle boundsToSet = CalcBoundsToSet();
					if(!settingControlBounds && (controlRef.Enabled != EnabledState || GetControlRealEnabledState() != EnabledState)) {
						BeginSetControlBounds();
						SetEnabledState(controlRef);
						EndSetControlBounds();
						boundsToSet = CalcBoundsToSet();
					}
					if(!settingControlBounds && Owner != null && Owner.OptionsView.IsReadOnly != DefaultBoolean.Default)
						UpdateControlReadonlyStatus(Owner.OptionsView.IsReadOnly);
					if(!settingControlBounds && (controlRef.Bounds != boundsToSet)) {
						BeginSetControlBounds();
						if(controlRef.AutoSize)
							controlRef.AutoSize = false;
						ContainerControl cc = controlRef as ContainerControl;
						if(cc != null && cc.AutoScaleMode != AutoScaleMode.None)
							cc.AutoScaleMode = AutoScaleMode.None;
						if(controlRef.Dock != DockStyle.None)
							controlRef.Dock = DockStyle.None;
						controlRef.Bounds = boundsToSet;
						EndSetControlBounds();
					}
				}
				finally {
					if(shouldSetControlVisibleStatus && controlRef != null) { 
						controlRef.Visible = controlVisibleStatus;
						controlRef = null;
					}
				}
			}
		}
		void SetEnabledState(Control controlRef) {
			if(settingEnableState) return;
			settingEnableState = true;
			if(!originalEnabledInitialized) {
				originalEnabled = GetControlRealEnabledState(controlRef);
				originalEnabledInitialized = true;
			}
			controlRef.Enabled = EnabledState;
			settingEnableState = false;
		}
		bool ExtractBoolFromDefaultBoolean(DefaultBoolean status) {
			return status == DefaultBoolean.True;
		}
		protected internal virtual bool GetControlReadonlyStatus() {
			BaseEdit be = control as BaseEdit;
			TextBoxBase tbb = control as TextBoxBase;
			if(be != null) return be.Properties.ReadOnly;
			if(tbb != null) return tbb.ReadOnly;
			return false;
		}
		protected virtual void UpdateControlReadonlyStatus(DefaultBoolean status) {
			BeginSetControlBounds();
			BaseEdit be = control as BaseEdit;
			bool boolStatus = ExtractBoolFromDefaultBoolean(status);
			if(be != null && be.Properties.ReadOnly != boolStatus) be.Properties.ReadOnly = boolStatus;
			TextBoxBase tbb = control as TextBoxBase;
			if(tbb != null && tbb.ReadOnly != boolStatus) tbb.ReadOnly = boolStatus;
			EndSetControlBounds();
		}
		protected override Type GetDefaultWrapperType() {
			if(Parent != null)
				switch(Parent.LayoutMode) {
					case DevExpress.XtraLayout.Utils.LayoutMode.Flow:
						return typeof(FlowLayoutControlItemWrapper);
					case DevExpress.XtraLayout.Utils.LayoutMode.Table:
						return typeof(TableLayoutControlItemWrapper);
				}
			return typeof(LayoutControlItemWrapper);
		}
		protected override void XtraDeserializePadding(XtraEventArgs e) {
			if(Owner == null) base.XtraDeserializePadding(e);
			else {
				if(Owner.OptionsSerialization.RestoreLayoutItemPadding) base.XtraDeserializePadding(e);
			}
		}
		protected override void XtraDeserializeSpacing(XtraEventArgs e) {
			if(Owner == null) base.XtraDeserializeSpacing(e);
			else {
				if(Owner.OptionsSerialization.RestoreLayoutItemSpacing) base.XtraDeserializeSpacing(e);
			}
		}
		DefaultBoolean allowGlyphSkinningCore = DefaultBoolean.Default;
		[
#if !SL
	DevExpressXtraLayoutLocalizedDescription("LayoutGroupAllowGlyphSkinning"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		[XtraSerializableProperty()]
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinningCore; }
			set {
				if(allowGlyphSkinningCore == value) return;
				allowGlyphSkinningCore = value;
				shouldResetBorderInfoCore = true;
				ComplexUpdate(true, false);
			}
		}
		protected internal virtual bool GetAllowGlyphSkinning() {
			if(AllowGlyphSkinning != DefaultBoolean.Default)
				return AllowGlyphSkinning == DefaultBoolean.True;
			return (Owner != null) && Owner.OptionsView.AllowGlyphSkinning;
		}
		protected override void UpdateText() {
			if(!CanOptimizeTextCalculation()) ShouldArrangeTextSize = true;
			else {
				if(Owner != null)
					Owner.TextAlignManager.Reset(GetListItemsReCalc());
			}
			if(IsGroup) ShouldResetBorderInfo = true;
			ComplexUpdate();
			if(!IsUpdateLocked) {
				RaiseTextChanged(EventArgs.Empty);
				TextChangedNotifyParent();
			}
		}
		protected virtual List<BaseLayoutItem> GetListItemsReCalc() {
			List<BaseLayoutItem> itemsReCalc = new List<BaseLayoutItem>();
			if(!(Owner is LayoutControl)) return null;
			foreach(var item in (Owner as LayoutControl).Items) {
				if(item is LayoutControlItem && (item as LayoutControlItem).CanOptimizeTextCalculation())
					itemsReCalc.Add(item as BaseLayoutItem);
			}
			return itemsReCalc;
		}
		protected override bool AllowSetSizingType {
			get {
				return true;
			}
		}
	}
	public class LayoutControlItemWrapper : BaseLayoutItemWrapper {
		protected new LayoutControlItem Item { get { return WrappedObject as LayoutControlItem; } }
		[DefaultValue(true)]
		public virtual bool ContentVisible { get { return Item.ContentVisible; } set { Item.ContentVisible = value; OnSetValue(Item, value); } }
		[DefaultValue(ContentAlignment.TopLeft), Category("Control")]
		public virtual ContentAlignment ControlAlignment { get { return Item.ControlAlignment; } set { Item.ControlAlignment = value; OnSetValue(Item, value); } }
		[Category("Control")]
		public virtual Size ControlMaxSize { get { return Item.ControlMaxSize; } set { Item.ControlMaxSize = value; OnSetValue(Item, value); } }
		[Category("Control")]
		public virtual Size ControlMinSize { get { return Item.ControlMinSize; } set { Item.ControlMinSize = value; OnSetValue(Item, value); } }
		[DefaultValue(ContentAlignment.MiddleLeft), Category("Image")]
		public virtual ContentAlignment ImageAlignment { get { return Item.ImageAlignment; } set { Item.ImageAlignment = value; OnSetValue(Item, value); } }
		[DefaultValue(-1), Category("Image")]
		public virtual int ImageIndex { get { return Item.ImageIndex; } set { Item.ImageIndex = value; OnSetValue(Item, value); } }
		[DefaultValue(5), Category("Image")]
		public virtual int ImageToTextDistance { get { return Item.ImageToTextDistance; } set { Item.ImageToTextDistance = value; OnSetValue(Item, value); } }
		[DefaultValue(true)]
		public virtual bool Enabled { get { return Item.Enabled; } set { Item.Enabled = value; OnSetValue(Item, value); } }
		[DefaultValue(SizeConstraintsType.Default)]
		public virtual SizeConstraintsType SizeConstraintsType { get { return Item.SizeConstraintsType; } set { Item.SizeConstraintsType = value; OnSetValue(Item, value); } }
		[DefaultValue(TextAlignModeItem.UseParentOptions), Category("Text")]
		public virtual TextAlignModeItem TextAlignMode { get { return Item.TextAlignMode; } set { Item.TextAlignMode = value; OnSetValue(Item, value); } }
		public virtual OptionsPrintItem OptionsItemPrint { get { return Item.OptionsPrint; } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new LayoutControlItemWrapper();
		}
	}
	public class FlowLayoutControlItemWrapper : LayoutControlItemWrapper {
		[Category("OptionsFlowLayoutItem"), DefaultValue(false)]
		public virtual bool NewLineInFlowLayout { get { return Item.StartNewLine; } set { Item.StartNewLine = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new FlowLayoutControlItemWrapper();
		}
	}
	public class TableLayoutControlItemWrapper :LayoutControlItemWrapper {
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int RowSpan { get { return Item.OptionsTableLayoutItem.RowSpan; } set { Item.OptionsTableLayoutItem.RowSpan = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem"), DefaultValue(1)]
		public virtual int ColumnSpan { get { return Item.OptionsTableLayoutItem.ColumnSpan; } set { Item.OptionsTableLayoutItem.ColumnSpan = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int RowIndex { get { return Item.OptionsTableLayoutItem.RowIndex; } set { Item.OptionsTableLayoutItem.RowIndex = value; OnSetValue(Item, value); } }
		[Category("OptionsTableLayoutItem")]
		public virtual int ColumnIndex { get { return Item.OptionsTableLayoutItem.ColumnIndex; } set { Item.OptionsTableLayoutItem.ColumnIndex = value; OnSetValue(Item, value); } }
		public override BasePropertyGridObjectWrapper Clone() {
			return new TableLayoutControlItemWrapper();
		}
	}
}
