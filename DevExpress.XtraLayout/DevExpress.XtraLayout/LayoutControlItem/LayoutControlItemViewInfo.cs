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
using System.IO;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Menu;
using DevExpress.Utils;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraLayout.Helpers;
namespace DevExpress.XtraLayout.ViewInfo {
	public class LayoutControlItemViewInfo : BaseLayoutItemViewInfo {																			 
		LabelInfoArgs labelInfo;
		public LayoutControlItemViewInfo(LayoutControlItem owner) : base(owner) {
			labelInfo = new LabelInfoArgs(null, null, Rectangle.Empty);
		}
		public LabelInfoArgs LabelInfo { get { return labelInfo; } }
		public override object Clone() {
			LayoutControlItemViewInfo cloneInfo= (LayoutControlItemViewInfo)base.Clone();
			cloneInfo.labelInfo = new LabelInfoArgs(null, null, Rectangle.Empty);
			return cloneInfo;
		}
		public override Rectangle ClientArea {
			get { return base.ClientArea; }
			set { base.ClientArea = value; }
		}
		public override Point Offset {
			get { return base.Offset; }
			set { base.Offset = value; }
		}
		public override void UpdateAppearance() {
			UpdateAppearance(Owner.Image, Owner.Images, Owner.ImageIndex, Owner.ImageAlignment);
		}
		protected virtual void UpdateAppearance(Image image, object imageList, int imageIndex, ContentAlignment alignment) {
			if(labelInfo.Appearance==null) labelInfo.Appearance = CreateLabelAppearance();
			UpdateLabelAppearance(labelInfo.Appearance, image, imageList, imageIndex, alignment);
			labelInfo.PaintAppearance = Owner.PaintAppearanceItemCaption;
		}
		protected virtual void UpdateLabelAppearance(LabelControlAppearanceObject appearance, Image image, object imageList, int imageIndex, ContentAlignment alignment) {
			appearance.BeginUpdate();
			appearance.Assign(Owner.PaintAppearanceItemCaption);
			appearance.Image = image;
			appearance.ImageList = imageList;
			appearance.ImageIndex = imageIndex;
			appearance.ImageAlign = alignment;
			appearance.TextOptions.RightToLeft = Owner.IsRTL;
			labelInfo.AllowGlyphSkinning = Owner.GetAllowGlyphSkinning();
			labelInfo.Enabled = Owner.EnabledState;
			appearance.CancelUpdate();
		}
		protected virtual LabelControlAppearanceObject CreateLabelAppearance() {
			return new LabelControlAppearanceObject();
		}
		protected System.Drawing.Text.HotkeyPrefix ShowUICues {
			get {
				if(Owner == null || Owner.Owner == null || Owner.Owner.Control == null) return System.Drawing.Text.HotkeyPrefix.Hide;
				if(Owner.Owner.ShowKeyboardCues)
					return System.Drawing.Text.HotkeyPrefix.Show;
				else
					return System.Drawing.Text.HotkeyPrefix.Hide;
			}
		}
		string textCache;
		int widthCache;
		AppearanceObject apObjCache;
		protected override void UpdateTextArea() {
			UpdateAppearance();
			labelInfo.AllowHtmlString = Owner.AllowHtmlStringInCaption;
			if (labelInfo.AllowHtmlString) {
					var g = GraphicsInfo.Default.AddGraphics(null);
					try {
					labelInfo.StringInfo = GetStringInfo(g,Owner.TextSize.Width);
				} finally {
						GraphicsInfo.Default.ReleaseGraphics();
					}
			}
			labelInfo.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
			labelInfo.DefaultTextOptions = Owner.PaintAppearanceItemCaption.TextOptions;
			labelInfo.DefaultTextOptions.RightToLeft = Owner.IsRTL;
			labelInfo.DisplayText = Owner.Text;
			labelInfo.HotkeyPrefixState = ShowUICues;
			if(labelInfo.AllowHtmlString && labelInfo.DefaultTextOptions.HotkeyPrefix == HKeyPrefix.Default) labelInfo.DefaultTextOptions.HotkeyPrefix = HKeyPrefix.Show;
			labelInfo.LineVisible = false;
			labelInfo.Enabled = Owner.EnabledState;
			CalculateLabelRects(TextAreaRelativeToControl);
		}
	   internal DevExpress.Utils.Text.StringInfo GetStringInfo(System.Drawing.Graphics g,int textWidht) {
		   if(textCache != Owner.Text || widthCache != textWidht || !apObjCache.Equals(Owner.PaintAppearanceItemCaption)) {
			   using(AppearanceObject obj = Owner.PaintAppearanceItemCaption.Clone() as AppearanceObject) {
				   textCache = Owner.Text;
				   apObjCache = Owner.PaintAppearanceItemCaption;
				   widthCache = textWidht;
				   return StringPainter.Default.Calculate(g, obj, Owner.Text, textWidht);
			   }
		   } else { return labelInfo.StringInfo; }
		}
		protected internal virtual void CalculateLabelRects(Rectangle labelBounds) {
			labelInfo.Bounds = labelBounds;
			if(labelInfo.StringInfo != null) {
				labelInfo.StringInfo.SetLocation(labelBounds.Location);
				StringPainter.Default.UpdateLocation(labelInfo.StringInfo, labelBounds);
			}
			Size imageSize = CalcLabelImageSize(LabelInfo.Appearance);
			labelInfo.ImageBounds = CalcAlignedImageRect(labelBounds, imageSize, LabelInfo.Appearance.ImageAlign);
			labelInfo.TextRect = CalcAlignedTextRect(labelBounds, imageSize, LabelInfo.Appearance.ImageAlign);
		}
		protected Rectangle CalcAlignedTextRect(Rectangle bounds, Size imageSize, ContentAlignment alignment) {
			Rectangle rect = bounds;
			if(!imageSize.IsEmpty) {
				Point origin = bounds.Location;
				Size size = bounds.Size;
				switch(alignment) {
					case ContentAlignment.TopLeft:
					case ContentAlignment.BottomLeft:
						origin = bounds.Location + new Size(imageSize.Width + Owner.ImageToTextDistance, 0);
						size.Width -= (imageSize.Width + Owner.ImageToTextDistance);
						break;
					case ContentAlignment.TopRight:
					case ContentAlignment.BottomRight:
						origin = bounds.Location;
						size.Width -= (imageSize.Width + Owner.ImageToTextDistance);
						break;
					case ContentAlignment.MiddleLeft:
						origin = bounds.Location + new Size(imageSize.Width + Owner.ImageToTextDistance, 0);
						size.Width -= (imageSize.Width + Owner.ImageToTextDistance);
						break;
					case ContentAlignment.MiddleRight:
						origin = bounds.Location;
						size.Width -= (imageSize.Width + Owner.ImageToTextDistance);
						break;
					case ContentAlignment.TopCenter:
						origin = bounds.Location + new Size(0, imageSize.Height);
						size.Height -= (imageSize.Height + Owner.ImageToTextDistance);
						break;
					case ContentAlignment.BottomCenter:
						origin = bounds.Location;
						size.Height -= (imageSize.Height + Owner.ImageToTextDistance);
						break;
				}
				rect = new Rectangle(origin, size);
			} 
			return rect;
		}
		protected Rectangle CalcAlignedImageRect(Rectangle bounds, Size size, ContentAlignment alignment) {
			Rectangle rect = Rectangle.Empty;
			if(!size.IsEmpty) {
				Point origin = bounds.Location;
				switch(alignment) {
					case ContentAlignment.TopLeft: origin = new Point(bounds.Left, bounds.Top);break;
					case ContentAlignment.TopRight: origin = new Point(bounds.Right - size.Width, bounds.Top); break;
					case ContentAlignment.TopCenter: origin = new Point(bounds.Left + (bounds.Width-size.Width)/2, bounds.Top); break;
					case ContentAlignment.MiddleLeft: origin = new Point(bounds.Left, bounds.Top+ (bounds.Height-size.Height)/2); break;
					case ContentAlignment.MiddleRight: origin = new Point(bounds.Right - size.Width, bounds.Top+ (bounds.Height-size.Height)/2); break;
					case ContentAlignment.MiddleCenter: origin = new Point(bounds.Left + (bounds.Width-size.Width)/2, bounds.Top+ (bounds.Height-size.Height)/2); break;
					case ContentAlignment.BottomLeft: origin = new Point(bounds.Left, bounds.Bottom - size.Height); break;
					case ContentAlignment.BottomRight: origin = new Point(bounds.Right - size.Width, bounds.Bottom - size.Height); break;
					case ContentAlignment.BottomCenter: origin = new Point(bounds.Left + (bounds.Width-size.Width)/2, bounds.Bottom - size.Height); break;
				}
				rect = new Rectangle(origin, size);
			}
			return rect;
		}
		protected Size CalcLabelImageSize(LabelControlAppearanceObject appearance) {
			if(appearance==null) return Size.Empty;
			else return Owner.CalcItemImageSize(appearance.Image, appearance.ImageList, appearance.ImageIndex);
		}
		public new LayoutControlItem Owner {
			get {return (LayoutControlItem)base.Owner;}
		}
	}
}
