#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ColorPick.Picker;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class FormatRuleStyleSettingsControl : InnerColorPickControl, IStyleContainerManager {
		internal const int DefaultGroupPadding = 10;
		const int DefaultAppearanceItemSize = 24;
		const int DefaultAppearanceItemGap = 6;
		const int DefaultAppearanceItemsPerLine = 8;
		const int DefaultIconItemSize = 18;
		const int DefaultIconItemGap = 6;
		const int DefaultIconItemsPerLine = 10;
		const StyleMode DefaultStyleMode = StyleMode.Appearance;
		static string CustomAppearanceCaption { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.FormatConditionCustomAppearanceCaption); } }
		static string GetColorName(Color? color, Color defaultColor) {
			Color clr = color.HasValue && !color.Value.IsEmpty ? color.Value : defaultColor;
			return clr.IsNamedColor ? clr.Name : '#' + clr.Name;
		}
		static string GetFontStyle(FontStyle? fontStyle) {
			if(fontStyle.HasValue) {
				switch(fontStyle) {
					case FontStyle.Bold:
						return "<b>";
					case FontStyle.Italic:
						return "<i>";
					case FontStyle.Underline:
						return "<u>";
				}
			}
			return string.Empty;
		}
		static object selectedStyleChanged = new object();
		StyleMode styleMode;
		public StyleMode StyleMode { get { return styleMode; } }
		PickerItem SelectedPickerItem {
			get { return (PickerItem)SelectedColorItem; }
			set { SelectedColorItem = value; }
		}
		IEnumerable<PickerItem> PickerItems {
			get {
				foreach(PickerItem pickerItem in ThemeColors) {
					yield return pickerItem;
				}
				foreach(PickerItem pickerItem in StandardColors) {
					yield return pickerItem;
				}
			}
		}
		bool IsIconStyles { get { return StyleMode == StyleMode.Icon; } }
		bool IsGradientStyles { get { return StyleMode.IsGradient(); } }
		int CurrentItemSize { get { return IsIconStyles ? DefaultIconItemSize : DefaultAppearanceItemSize; } }
		int CurrentItemGap { get { return IsIconStyles ? DefaultIconItemGap : DefaultAppearanceItemGap; } }
		int CurrentItemsPerLine { get { return IsIconStyles ? DefaultIconItemsPerLine : DefaultAppearanceItemsPerLine; } }
		int CurrentGroupPadding { get { return DefaultGroupPadding; } }
		public FormatRuleStyleSettingsControl()
			: this(null) {
		}
		public FormatRuleStyleSettingsControl(UserLookAndFeel lookAndFeel)
			: this(DefaultStyleMode, false, lookAndFeel) {
		}
		public FormatRuleStyleSettingsControl(StyleMode styleMode, bool showEmptyStyleItem, UserLookAndFeel lookAndFeel) {
			this.styleMode = styleMode;
			AutomaticColorItem = CreateAutomaticColorItemInstance(Color.Empty);
			AutomaticButtonCaption = DashboardWinLocalizer.GetString(IsGradientStyles ? DashboardWinStringId.EditorAutomaticValue : DashboardWinStringId.FormatConditionRangeSetNoStyleCaption);
			ShowAutomaticButton = showEmptyStyleItem;
			StandardGroupCaption = CustomAppearanceCaption;
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			Initialize();
		}
		void Initialize() {
			ThemeColors.BeginUpdate();
			ICollection<StyleSettingsContainer> styles = StyleCache.GetPredefinedStyleTypes(DashboardWinHelper.IsDarkScheme(LookAndFeel) ? FormatConditionColorScheme.Dark : FormatConditionColorScheme.Light, StyleMode);
			foreach(StyleSettingsContainer style in styles)
				ThemeColors.Add(new PickerItem(style));
			ThemeColors.EndUpdate();
			if(StyleMode != StyleMode.Icon) {
				StandardColors.BeginUpdate();
				for(int i = 0; i < CurrentItemsPerLine; i++)
					StandardColors.Add(new CustomPickerItem(StyleMode));
				StandardColors.EndUpdate();
			}
			ItemSize = new Size(CurrentItemSize, CurrentItemSize);
			ItemHGap = ItemVGap = CurrentItemGap;
			FirstRowGap = 0;
			GroupPadding = new Padding(CurrentGroupPadding);
			Padding = Padding.Empty;
			Selectable = true;
			UserMouse = true;
			MaximumSize = MinimumSize = new Size(
				(ItemSize.Width + ItemHGap) * CurrentItemsPerLine - ItemHGap + 2 * (CurrentGroupPadding + 1), 
				ViewInfo.CalcBestHeight(CurrentItemsPerLine) + ItemVGap
			);
		}
		void SetDefaultSelectedItem(StyleSettingsContainer style) {
			PickerItem pickerItem;
			if(ShowAutomaticButton) {
				pickerItem = (PickerItem)AutomaticColorItem;
				if(IsGradientStyles)
					pickerItem.Style.Assign(style);
			} else {
				pickerItem = (PickerItem)ThemeColors[0];
			}
			SelectedPickerItem = pickerItem;
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			InnerColorPickControlHitInfo hitInfo = ViewInfo.CalcHitInfo(e.Location);
			if(hitInfo != null && hitInfo.ColorItem != null) {
				PickerItem pickerItem = (PickerItem)hitInfo.ColorItem.ColorItem;
				if(pickerItem.IsCustom && !pickerItem.IsEmpty) {
					if(styleMode == StyleMode.Bar) {
						ShowSelectColorForm(pickerItem);
					}
					else {
						this.BeginInvoke(new Action(() => {
							using(FormatRuleCustomStyleForm styleForm = new FormatRuleCustomStyleForm(LookAndFeel, pickerItem.Style)) {
								if(styleForm.ShowDialog(FindNonPopupForm()) == DialogResult.OK)
									SetPickerItemCustomStyle(pickerItem, styleForm.Style);
							}
						}));
					}
				}
			}
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			ToolTipControlInfo info = base.GetToolTipInfo(point);
			if(info != null && !IsIconStyles) {
				ColorItemInfo colorItemInfo = (ColorItemInfo)info.Object;
				PickerItem pickerItem = (PickerItem)colorItemInfo.ColorItem;
				FormatRuleStyleSettingsControlViewInfo viewInfo = (FormatRuleStyleSettingsControlViewInfo)ViewInfo;
				Skin skin = viewInfo.GetSkin();
				info.SuperTip.AllowHtmlText = DefaultBoolean.True;
				info.SuperTip.Items.Clear();
				StyleSettingsContainer style = pickerItem.Style;
				info.SuperTip.Items.Add(string.Format("{0}<backcolor={1}><color={2}>{3}</color></backcolor>",
					GetFontStyle(style.FontStyle),
					GetColorName(style.Mode == StyleMode.Bar? style.BarColor: style.BackColor, skin.Colors["Window"]),
					GetColorName(style.ForeColor, skin.Colors["WindowText"]),
					GetTooltipText(style.Mode == StyleMode.Bar? style.PredefinedBarColor: style.AppearanceType)));
				return info;
			} else
				return null;
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new FormatRuleStyleSettingsControlViewInfo(this);
		}
		protected override BaseControlPainter CreatePainter() {
			return new FormatRuleStyleSettingsControlPainter();
		}
		protected override ColorItem CreateAutomaticColorItemInstance(Color color) {
			return new AutomaticPickerItem(IsGradientStyles ? StyleMode.GradientGenerated : StyleMode);
		}
		protected override bool OnSelectedColorChanging(InnerColorPickControlSelectedColorChangingEventArgs e) {
			PickerItem nextPickerItem = (PickerItem)e.NewColorItem;
			if(nextPickerItem.IsCustom && nextPickerItem.IsEmpty) {
				if(styleMode == StyleMode.Bar) {
					if(ShowSelectColorForm(nextPickerItem) != DialogResult.OK) {
						e.Cancel = true;
						Refresh();
					}
				}
				else {
					using(FormatRuleCustomStyleForm styleForm = new FormatRuleCustomStyleForm(LookAndFeel, nextPickerItem.Style)) {
						if(styleForm.ShowDialog(FindNonPopupForm()) == DialogResult.OK && !styleForm.Style.IsEmpty) {
							SetPickerItemCustomStyle(nextPickerItem, styleForm.Style);
						}
						else {
							e.Cancel = true;
							Refresh();
						}
					}
				}
			}
			return base.OnSelectedColorChanging(e);
		}
		DialogResult ShowSelectColorForm(PickerItem pickerItem) {
			DialogResult dialogResult;
			using(FrmColorPicker colorForm = new FrmColorPicker()) {
				colorForm.StartPosition = FormStartPosition.CenterParent;
				Color? barColor = pickerItem.Style.BarColor;
				FormatConditionColorScheme scheme = DashboardWinHelper.IsDarkScheme(LookAndFeel) ? FormatConditionColorScheme.Dark : FormatConditionColorScheme.Light;
				colorForm.SelectedColor = barColor.HasValue ? barColor.Value : FormatConditionAppearanceType.PaleRed.ToBackColor(scheme);
				dialogResult = colorForm.ShowDialog(FindNonPopupForm());
				if(dialogResult == DialogResult.OK) {
					StyleSettingsContainer style = new StyleSettingsContainer(styleMode, colorForm.SelectedColor);
					SetPickerItemCustomStyle(pickerItem, style);
				}
			}
			return dialogResult;
		}
		protected override void OnSelectedColorItemChanged(ColorItem prevItem, ColorItem nextItem) {
			base.OnSelectedColorItemChanged(prevItem, nextItem);
			RaiseSelectedStyleChanged((PickerItem)prevItem, (PickerItem)nextItem);
		}
		void RaiseSelectedStyleChanged(PickerItem oldItem, PickerItem newItem) {
			StyleSettingsContainerChangedEventHandler handler = (StyleSettingsContainerChangedEventHandler)Events[selectedStyleChanged];
			if(handler != null) {
				handler(this, new StyleSettingsContainerItemChangedEventArgs(oldItem != null ? oldItem.Style : null, newItem != null ? newItem.Style : null));
			}
		}
		object GetTooltipText(FormatConditionAppearanceType appearanceType) {
			return appearanceType.Localize();
		}
		PickerItem AddCustomStyle(StyleSettingsContainer style) {
			if(style == null || style.IsEmpty) return null;
			foreach(PickerItem pickerItem in StandardColors) {
				if(pickerItem.IsEmpty) {
					SetPickerItemCustomStyle(pickerItem, style);
					return pickerItem;
				}
			}
			return null;
		}
		void SetPickerItemCustomStyle(PickerItem pickerItem, StyleSettingsContainer style) {
			pickerItem.Style = style;
			if(pickerItem == SelectedPickerItem)
				RaiseSelectedStyleChanged(pickerItem, pickerItem);
		}
		Form FindNonPopupForm() {
			Control parent = this;
			while((parent != null) && (!(parent is Form) || (parent is IPopupForm))) {
				parent = (parent is IPopupForm) ? ((Form)parent).Owner : parent.Parent;
			}
			return (Form) parent;
		}
		#region IStyleContainerProvider Members
		StyleSettingsContainer IStyleContainerProvider.Style {
			get { return SelectedPickerItem == null ? null : SelectedPickerItem.Style; }
			set {
				if(value != null && value.Mode == StyleMode.GradientGenerated) {
					PickerItem pickerItem = (PickerItem)CreateAutomaticColorItemInstance(Color.Empty);
					pickerItem.Style.Assign(value);
					AutomaticColorItem = pickerItem;
					SelectedPickerItem = pickerItem;
				} else {
					if(value == null)
						SelectedPickerItem = null;
					else if(value.IsEmpty)
						SetDefaultSelectedItem(value);
					else {
						bool isCustom = true;
						foreach(PickerItem pickerItem in PickerItems) {
							StyleSettingsContainer style = pickerItem.Style;
							if(object.Equals(value, pickerItem.Style)) {
								SelectedPickerItem = pickerItem;
								isCustom = false;
								break;
							}
						}
						if(isCustom) {
							SelectedPickerItem = AddCustomStyle(value);
						}
					}
				}
			}
		}
		#endregion
		#region IStyleContainerManager Members
		event StyleSettingsContainerChangedEventHandler IStyleContainerManager.StyleChanged {
			add { Events.AddHandler(selectedStyleChanged, value); }
			remove { Events.RemoveHandler(selectedStyleChanged, value); }
		}
		#endregion
	}
	public class FormatRuleStyleSettingsControlViewInfo : InnerColorPickControlViewInfo {
		const int AutomaticButtonHeight = 24;
		const int StandardGroupVOffset = 8;
		new FormatRuleStyleSettingsControl Owner { get { return (FormatRuleStyleSettingsControl)base.Owner; } }
		internal bool HighlightItemBackground { get { return Owner.StyleMode == StyleMode.Icon; } }
		bool IsStandardGroupShown { get { return Owner.StyleMode != StyleMode.Icon; } }
		Point AutomaticButtonOffset { get { return new Point(Owner.GroupPadding.Left, Owner.GroupPadding.Top); } }
		public FormatRuleStyleSettingsControlViewInfo(InnerColorPickControl owner) : base(owner) {
			LookAndFeel.ParentLookAndFeel = owner.LookAndFeel;
		}
		public Skin GetSkin() {
			return CommonSkins.GetSkin(LookAndFeel).CommonSkin;
		}
		protected override Rectangle CalcAutomaticButtonBounds() {
			Rectangle rect = base.CalcAutomaticButtonBounds();
			rect.Offset(AutomaticButtonOffset);
			rect.Height = AutomaticButtonHeight;
			rect.Width -= 2 * (AutomaticButtonOffset.X + 1);
			return rect;
		}
		protected override int CalcAutomaticButtonBestHeight() {
			return OwnerControl.ShowAutomaticButton ? AutomaticButtonHeight + Owner.GroupPadding.Top : 0;
		}
		protected override Rectangle CalcThemeGroupCaptionBounds() {
			if(!Owner.ShowAutomaticButton)
				return Rectangle.Empty;
			else {
				Rectangle rect = base.CalcThemeGroupCaptionBounds();
				rect.Height -= CalcGroupCaptionHeight() + AutomaticButtonOffset.Y;
				return rect;
			}
		}
		protected override Rectangle CalcThemeGroupCaptionTextBounds() {
			return Rectangle.Empty;
		}
		protected override int CalcThemeGroupBestHeight(int itemsInRow) {
			return base.CalcThemeGroupBestHeight(itemsInRow) - CalcGroupCaptionHeight();
		}
		protected override int CalcStandardGroupTopPoint() {
			return IsStandardGroupShown ? base.CalcStandardGroupTopPoint() - StandardGroupVOffset : 0;
		}
		protected override Rectangle CalcStandardGroupCaptionTextBounds() {
			if(IsStandardGroupShown) {
				Rectangle bounds = base.CalcStandardGroupCaptionTextBounds();
				bounds.Y += StandardGroupVOffset;
				return bounds;
			} else
				return Rectangle.Empty;
		}
		protected override int CalcStandardGroupBestHeight(int itemsInRow) {
			return IsStandardGroupShown ? base.CalcStandardGroupBestHeight(itemsInRow) - StandardGroupVOffset : 0;
		}
		protected override Rectangle CalcMoreButtonBounds() {
			return Rectangle.Empty;
		}
		protected override int CalcMoreColorButtonBestHeight() {
			return 0;
		}
		protected override Rectangle CalcMoreButtonGlyphBounds() {
			return Rectangle.Empty;
		}
	}
	public class FormatRuleStyleSettingsControlPainter : InnerColorPickControlPainter {
		static Color GetHighlightColor(Skin skin) {
			return skin != null ? skin.Colors["Highlight"] : Color.Empty;
		}
		static Color GetHighlightAlternateColor(Skin skin) {
			return skin != null ? skin.Colors["HighlightAlternate"] : Color.Empty;
		}
		protected override void DoDrawAutomaticButton(ControlGraphicsInfoArgs info) {
			FormatRuleStyleSettingsControlViewInfo viewInfo = (FormatRuleStyleSettingsControlViewInfo)info.ViewInfo;
			Rectangle bounds = viewInfo.Rects[InnerColorPickControlRect.AutomaticButtonBounds];
			if(viewInfo.AutomaticButtonState != ObjectState.Normal) {
				DrawBorder(viewInfo.GetSkin(), info.Cache, viewInfo.AutomaticButtonState, bounds, false);
			}
			StyleSettingsContainerPainter.Draw(info, bounds, viewInfo.GetAutomaticColorButtonCaption());
		}
		protected override void DoDrawColorItem(ControlGraphicsInfoArgs info, ColorItemInfo itemInfo) {
			if(itemInfo.State != ObjectState.Normal) {
				FormatRuleStyleSettingsControlViewInfo viewInfo = (FormatRuleStyleSettingsControlViewInfo)info.ViewInfo;
				DrawBorder(viewInfo.GetSkin(), info.Cache, itemInfo.State, itemInfo.Bounds, viewInfo.HighlightItemBackground);
			}
			StyleSettingsContainer style = ((PickerItem)itemInfo.ColorItem).Style;
			StyleSettingsContainerPainter.Draw(style, info, itemInfo.Bounds, true);
		}
		protected override void DoDrawStandardGroupCaption(ControlGraphicsInfoArgs info) {
			FormatRuleStyleSettingsControlViewInfo viewInfo = (FormatRuleStyleSettingsControlViewInfo)info.ViewInfo;
			viewInfo.PaintAppearanceGroupCaption.DrawString(info.Cache, viewInfo.GetStandardPaletteCaption(), viewInfo.Rects[InnerColorPickControlRect.StandardGroupCaptionTextBounds]);
		}
		void DrawBorder(Skin skin, GraphicsCache cache, ObjectState state, Rectangle bounds, bool highlightItemBackground) {
			if(highlightItemBackground) {
				Color highlightColor = GetHighlightColor(skin);
				cache.FillRectangle(cache.GetSolidBrush(highlightColor), Rectangle.Inflate(bounds, 2, 2));
			} else {
				Color alternateColor = GetHighlightAlternateColor(skin);
				if(alternateColor.IsEmpty)
					alternateColor = GetHighlightColor(skin);
				Color transparentColor = Color.FromArgb(0, 0xFF, 0xFF, 0xFF);
				Color borderColor = alternateColor;
				Color innerBorderColor = transparentColor;
				cache.DrawRectangle(cache.GetPen(innerBorderColor), Rectangle.Inflate(bounds, 1, 1));
				cache.DrawRectangle(cache.GetPen(borderColor), Rectangle.Inflate(bounds, 2, 2));
				cache.DrawRectangle(cache.GetPen(borderColor), Rectangle.Inflate(bounds, 3, 3));
			}
		}
	}
	public class PickerItem : ColorItem {
		StyleSettingsContainer style;
		public bool IsEmpty { get { return style.IsEmpty; } }
		public virtual bool IsCustom { get { return false; } }
		public virtual bool IsAutomatic { get { return false; } }
		protected override bool IsAutoColor { get { return IsAutomatic; } }
		public StyleSettingsContainer Style {
			get { return style; }
			set { style = value; }
		}
		public PickerItem(StyleSettingsContainer style) : base() {
			this.style = style;
		}
	}
	public class AutomaticPickerItem : PickerItem {
		public override bool IsAutomatic { get { return true; } }
		public AutomaticPickerItem(StyleMode styleMode)
			: base(StyleSettingsContainer.CreateDefaultEmpty(styleMode)) {
		}
	}
	public class CustomPickerItem : PickerItem {
		public override bool IsCustom { get { return true; } }
		public CustomPickerItem(StyleMode styleMode)
			: base(StyleSettingsContainer.CreateDefaultCustom(styleMode)) {
		}
	}
}
