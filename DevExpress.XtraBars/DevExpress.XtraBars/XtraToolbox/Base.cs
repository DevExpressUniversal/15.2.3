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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Utils;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraToolbox {
	public class ToolboxElementInfoBase {
		public ToolboxElementInfoBase(ToolboxElementBase element, ToolboxViewInfo info) {
			this.element = element;
			this.info = info;
			State = ObjectState.Normal;
			PaintAppearance = new AppearanceObject();
		}
		ToolboxElementBase element;
		protected internal ToolboxElementBase Element {
			get { return element; }
		}
		ToolboxViewInfo info;
		protected internal ToolboxViewInfo ViewInfo { 
			get { return info; }
		}
		public virtual string[] WrappedText { get; set; }
		public virtual Rectangle Bounds { get; set; }
		public virtual Rectangle CaptionBounds { get; set; }
		public virtual Rectangle ImageBounds { get; set; }
		public virtual ObjectState State { get; set; }
		public virtual AppearanceObject PaintAppearance { get; set; }
		protected internal virtual bool HasImage {
			get { return !IsNameOnly && Element.Image != null; }
		}
		protected internal virtual bool HasCaption {
			get { return !IsIconOnly && !string.IsNullOrWhiteSpace(Element.Caption); }
		}
		protected int ImageToTextDistance {
			get { return ViewInfo.Toolbox.OptionsView.ImageToTextDistance; }
		}
		protected virtual Rectangle ContentArea {
			get { return ToolboxHelper.ApplyPaddings(Bounds, GetPadding()); }
		}
		protected internal virtual void CalcViewInfo(Rectangle content) {
			Bounds = CalcBounds(content);
			CalcImageBounds();
			CalcCaptionBounds();
		}
		protected internal virtual Rectangle CalcBounds(Rectangle content) {
			return Bounds;
		}
		protected internal virtual void CalcCaptionBounds() {
			CaptionBounds = Rectangle.Empty;
			if(!HasCaption) return;
			CaptionBounds = GetCaptionBounds();
		}
		protected internal virtual void CalcImageBounds() {
			ImageBounds = Rectangle.Empty;
			if(!HasImage) return;
			ImageBounds = GetImageBounds();
		}
		protected virtual SkinElement SkinElement {
			get { return AccordionControlSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinItem]; }
		}
		protected internal virtual SkinElementInfo GetSkinElementInfo() {
			SkinElementInfo info = new SkinElementInfo(SkinElement, Bounds);
			info.State = State;
			info.ImageIndex = SkinElementPainter.Default.CalcDefaultImageIndex(info.Element.Image, State);
			info.RightToLeft = ViewInfo.IsRightToLeft;
			return info;
		}
		protected internal virtual Padding GetPadding() {
			return GetSkinElementInfo().Element.ContentMargins.ToPadding();
		}
		protected internal virtual Size GetBestImageSize() {
			return HasImage ? Element.Image.Size : Size.Empty;
		}
		protected virtual Rectangle GetImageArea() {
			Size size = GetBestImageSize();
			Point location = new Point(ViewInfo.IsRightToLeft ? ContentArea.Right - size.Width : ContentArea.Left, ContentArea.Top);
			return new Rectangle(location, size);
		}
		protected virtual Size GetCaptionSize() {
			int emptyWidth = ContentArea.Width - (HasImage ? ImageToTextDistance + GetImageArea().Width : 0);
			Size size = GetTextSize(0);
			return new Size(Math.Min(emptyWidth, size.Width), size.Width > emptyWidth ? size.Height * 2 : size.Height);
		}
		protected virtual Point GetCaptionLocation() {
			int x = 0;
			Size caption = GetCaptionSize();
			if(ViewInfo.IsRightToLeft) {
				x = (HasImage ? GetImageArea().Left - ImageToTextDistance : GetImageArea().Right) - caption.Width;
			}
			else {
				x = HasImage ? GetImageArea().Right + ImageToTextDistance : GetImageArea().Left;
			}
			return new Point(x, Bounds.Y + (Bounds.Height / 2 - caption.Height / 2));
		}
		protected virtual Rectangle GetCaptionBounds() {
			return new Rectangle(GetCaptionLocation(), GetCaptionSize());
		}
		protected virtual Size GetImageSize() {
			if(!HasImage) return Size.Empty;
			Size realImageSize = Element.Image == null ? Size.Empty : Element.Image.Size;
			Size imageArea = GetImageArea().Size;
			double k = Math.Max((double)realImageSize.Width / imageArea.Width, (double)realImageSize.Height / imageArea.Height);
			if(k > 1) return new Size((int)(realImageSize.Width / k), (int)(realImageSize.Height / k));
			return realImageSize;
		}
		protected virtual Rectangle GetImageBounds() {
			return RectangleHelper.GetCenterBounds(IsIconOnly ? ContentArea : GetImageArea(), GetImageSize());
		}
		protected virtual Size GetTextSize(int width) {
			if(!HasCaption) return Size.Empty;
			return PaintAppearance.CalcTextSizeInt(ViewInfo.GInfo.Graphics, Element.Caption, width);
		}
		protected virtual bool IsIconOnly {
			get { return ViewInfo.GetItemViewMode() == ToolboxItemViewMode.IconOnly; }
		}
		protected virtual bool IsNameOnly {
			get { return ViewInfo.GetItemViewMode() == ToolboxItemViewMode.NameOnly; }
		}
	}
	#region Enums
	public enum ToolboxState {
		Minimized,
		Normal
	}
	public enum ToolboxItemViewMode {
		IconAndName,
		IconOnly,
		NameOnly
	}
	public enum ToolboxMinimizingScrollMode {
		Default,
		DropDown
	}
	[Flags]
	public enum ToolboxRectangle {
		HeaderRect,
		GroupsContainerRect,
		ItemsClientRect,
		GroupsClientRect,
		ItemsContentRect,
		GroupsContentRect,
		HeaderCaptionRect,
		SearchContentRect
	}
	public enum ToolboxItemSelectMode {
		None,
		Single,
		Multiple
	}
	#endregion
}
