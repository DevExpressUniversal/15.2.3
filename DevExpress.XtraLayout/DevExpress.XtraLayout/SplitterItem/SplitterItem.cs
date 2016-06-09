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
using System.Text;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.HitInfo;
namespace DevExpress.XtraLayout {
	public enum SplitterItemResizeMode { AllSiblings, OnlyAdjacentControls}
	public enum SplitterItemFixedStyles { None, LeftTop, RightBottom}
	public class SplitterItem : EmptySpaceItem {
		LayoutType layoutType = LayoutType.Vertical;
		Rectangle boundsCore = Rectangle.Empty;
		int splitterWidthCore = 6;
		bool isInitializing = false;
		SplitterItemResizeMode resizeMode = SplitterItemResizeMode.OnlyAdjacentControls;
		SplitterItemFixedStyles fixedStyleCore = SplitterItemFixedStyles.None;
		public SplitterItem(LayoutControlGroup parent): base(parent) {}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public Rectangle GetSizingRect() {
			Rectangle rect = ViewInfo.BoundsRelativeToControl;
			if (IsHorizontal)
				return new Rectangle(rect.X + rect.Width / 3, rect.Y, rect.Width / 3, rect.Height);
			else
				return new Rectangle(rect.X, rect.Y + rect.Height / 3, rect.Width, rect.Height / 3);
		}
		public SplitterItem(): base(null) {}
		public override string GetDefaultText() {
			if(Name != "")
				return Name;
			String itemText = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.SplitterItemDefaultText);
			return itemText;
		}
		protected internal override BaseLayoutItemHitInfo CalcHitInfo(Point hitPoint, bool calcForHandler) {
			BaseLayoutItemHitInfo baseHI = base.CalcHitInfo(hitPoint, calcForHandler);
			if (GetSizingRect().Contains(hitPoint)) {
				if (IsHorizontal)
					baseHI.SetHitTestType(LayoutItemHitTest.VSizing);
				else baseHI.SetHitTestType(LayoutItemHitTest.HSizing);
			}
			return baseHI;
		}
		protected override ViewInfo.BaseLayoutItemViewInfo CreateViewInfo() {
			return new ViewInfo.SplitterItemViewInfo(this);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override int BestFitWeight { get { return 1; } }
		protected internal override int BestFitCore() { return 1; }
		public override void BeginInit() {
			isInitializing = true;
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			isInitializing = false;
		}
		public override bool AllowHotTrack {
			get { return true; }
			set { }
		}
		[XtraSerializableProperty(), DefaultValue(SplitterItemFixedStyles.None)]
		public SplitterItemFixedStyles FixedStyle {
			get { return fixedStyleCore; }
			set {
				StartChange();
				fixedStyleCore = value;
				ShouldUpdateConstraintsDoUpdate = true;
				ShouldResize = true;
				ComplexUpdate();
				EndChange();
			}
		}
		[XtraSerializableProperty(), DefaultValue(SplitterItemResizeMode.OnlyAdjacentControls)]
		public SplitterItemResizeMode ResizeMode {
			get { return resizeMode; }
			set {
				StartChange();
				resizeMode = value;
				ShouldUpdateConstraintsDoUpdate = true;
				ShouldResize = true;
				ComplexUpdate();
				EndChange();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false)]
		public override bool TextVisible { get { return false; } set { } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Padding Padding { get { return base.Padding; } set { base.Padding = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Padding Spacing {get {return base.Spacing;}set {base.Spacing = value;}}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size TextSize { get { return Size.Empty; } set { } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override String Text { get { return base.Text; } set { } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override TextAlignModeItem TextAlignMode {get {return TextAlignModeItem.AutoSize;} set {}}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceObject AppearanceItemCaption { get { return base.AppearanceItemCaption; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override SizeConstraintsType SizeConstraintsType { get { return SizeConstraintsType.Default; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsHorizontal { get { return layoutType == LayoutType.Horizontal; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsVertical { get { return layoutType == LayoutType.Vertical; } }
		public override Size Size {
			get { return base.Size; }
			set {
				base.Size = value;
				if(isInitializing) { 
					UpdateLayoutType(Bounds); 
				}
			}
		}
		[XtraSerializableProperty(), DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TypeName {
			get { return "SplitterItem"; }
		}
		protected internal void UpdateLayoutType() {
			UpdateLayoutType(Bounds);   
		}
		int lockLayoutTypeChangeCore = 0;
		protected internal int LockLayoutTypeChange {
			get { return lockLayoutTypeChangeCore; }
			set { lockLayoutTypeChangeCore = value; }
		}
		protected void UpdateLayoutType(Rectangle bounds) {
			if(LockLayoutTypeChange > 0) return;
			if(bounds.Width > bounds.Height)
				layoutType = LayoutType.Horizontal;
			else
				layoutType = LayoutType.Vertical;
			boundsCore = bounds;
		}
		protected internal override void SetBounds(Rectangle bounds) {
			base.SetBounds(bounds);
			UpdateLayoutType(bounds);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SplitterWidth {
			get {
				if(Owner == null)
					return splitterWidthCore;
				if(GetPaintingType() == PaintingType.Skinned) {
					SkinElementInfo info = null;
					if(this.layoutType == LayoutType.Horizontal) {
						info = new SkinElementInfo(CommonSkins.GetSkin(Owner.LookAndFeel)[CommonSkins.SkinSplitterHorz], Rectangle.Empty);
						return CalcSkinElementMinSize(info).Height;
					} else {
						info = new SkinElementInfo(CommonSkins.GetSkin(Owner.LookAndFeel)[CommonSkins.SkinSplitter], Rectangle.Empty);
						return CalcSkinElementMinSize(info).Width;
					}
				} else
					return splitterWidthCore;
			}
			set {
				StartChange();
				splitterWidthCore = value;
				ShouldUpdateConstraintsDoUpdate = true;
				ShouldResize = true;
				ComplexUpdate();
				EndChange();
			}
		}
		Size CalcSkinElementMinSize(SkinElementInfo ee) {
			if(ee.Element == null) return Size.Empty;
			Size minSize = ee.Element.Size.MinSize;
			Size imageSize = GetImageMinSize(ee.Element.Image);
			minSize.Width = Math.Max(minSize.Width, imageSize.Width);
			minSize.Height = Math.Max(minSize.Height, imageSize.Height);
			imageSize = GetImageMinSize(ee.Element.Glyph);
			minSize.Width = Math.Max(minSize.Width, imageSize.Width);
			minSize.Height = Math.Max(minSize.Height, imageSize.Height);
			return minSize;
		}
		Size GetImageMinSize(SkinImage image) {
			if(image == null) return Size.Empty;
			Size res = image.GetImageBounds(0).Size;
			if(res.IsEmpty || image.Stretch != SkinImageStretch.NoResize) return Size.Empty;
			return res;
		}
		protected Size CalcSplitterSizeCore(bool calcMax) {
			LayoutRectangle lREct = new LayoutRectangle(boundsCore, layoutType);
			int width = SplitterWidth;
			lREct.Height = width;
			if(calcMax) lREct.Width = 0;
			else
				lREct.Width = width;
			return lREct.Rectangle.Size;
		}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("SplitterItemMinSize")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size MinSize {get { return CalcSplitterSizeCore(false); } set { }}
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("SplitterItemMaxSize")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Size MaxSize { get { return CalcSplitterSizeCore(true); } set { } }
		protected override string GetDisplayName() {
			return LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.SplitterItemDefaultText);
		}
		protected override Image GetDisplayImage() {
			return LayoutControlImageStorage.Default.DragHeaderPainter.Images[3];
		}
		protected override Type GetDefaultWrapperType() {
			return typeof(SplitterItemWrapper);
		}
	}
	public class SplitterItemWrapper : EmptySpaceItemWrapper {
		protected new SplitterItem Item { get { return WrappedObject as SplitterItem; } }
	   [DefaultValue(SplitterItemResizeMode.OnlyAdjacentControls)]
		public virtual SplitterItemResizeMode ResizeMode { get { return Item.ResizeMode; } set { Item.ResizeMode = value; OnSetValue(Item, value); } }
	}
}
