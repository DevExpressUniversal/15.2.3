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
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraLayout.ViewInfo {
	public class LayoutGroupViewInfo : BaseLayoutItemViewInfo {
		CaptionImageInfo imageInfo;
		public LayoutGroupViewInfo(BaseLayoutItem owner)
			: base(owner) {
			this.imageInfo = CreateCaptionImageInfo();
		}
		protected internal override void Destroy() {
			base.Destroy();
			if((borderInfoCore as GroupObjectInfoArgs) != null)
				(borderInfoCore as GroupObjectInfoArgs).Dispose();
			if((LastInfo as GroupObjectInfoArgs) != null)
				(LastInfo as GroupObjectInfoArgs).Dispose();
			LastInfo = null;
		}
		public override Rectangle BoundsRelativeToControl {
			get {
				if(Owner.IsInTabbedGroup) 
					return Owner.ParentTabbedGroup.ViewInfo.ClientAreaRelativeToControl;
				else return base.BoundsRelativeToControl;
			}
		}
		protected override ObjectInfoArgs CreateLastInfo() {
			var temp = new GroupObjectInfoArgs();
			temp.SetButtonsPanelOwner(Owner, false);
			return temp;
		}
		protected override ObjectInfoArgs CreateBorderInfo() {
			var temp = new GroupObjectInfoArgs();
			temp.SetButtonsPanelOwner(Owner);
			return temp;
		}
		public new GroupObjectInfoArgs BorderInfo {
			get {
				CalculateViewInfoIfNeeded();
				return base.BorderInfo as GroupObjectInfoArgs;
			}
		}
		public new LayoutGroup Owner {
			get { return (LayoutGroup)base.Owner; }
		}
		protected override Size AddLabel(Size size) {
			return size;
		}
		protected override Rectangle CalculateObjectBounds(Rectangle rect) {
			if(CanHideBorders) return rect;
			return base.CalculateObjectBounds(rect);
		}
		protected override internal void UpdateState() {
			GroupObjectInfoArgs tempBorderInfo = BorderInfo as GroupObjectInfoArgs;
			if(tempBorderInfo != null) {
				tempBorderInfo.ButtonState = GetState(true);
				tempBorderInfo.State = State;
			}
		}
		public void UpdateAppearanceGroup(GroupObjectInfoArgs tempBorderInfo) {
			tempBorderInfo.SetAppearance(Owner.PaintAppearanceGroup.AppearanceGroup);
			tempBorderInfo.AppearanceCaption = Owner.PaintAppearanceGroup.AppearanceItemCaption;
		}
		protected CaptionImageInfo ImageInfo {
			get { return imageInfo; }
		}
		protected virtual CaptionImageInfo CreateCaptionImageInfo() {
			return new CaptionImageInfo(Owner);
		}
		protected override void UpdateBorderInfo(ObjectInfoArgs borderInfo) {
			GroupObjectInfoArgs tempBorderInfo = borderInfo as GroupObjectInfoArgs;
			GroupBoxButtonsPanel gbbp =tempBorderInfo.ButtonsPanel as GroupBoxButtonsPanel;
			bool shouldHandleRaiseEvent =gbbp.RaiseEvents;
			if(shouldHandleRaiseEvent) gbbp.RaiseEvents = false;
			UpdateAppearanceGroup(tempBorderInfo);
			tempBorderInfo.BackgroundImage = Owner.BackgroundImage;
			tempBorderInfo.BackgroundImageLayout = Owner.BackgroundImageLayout;
			tempBorderInfo.ShowBackgroundImage = Owner.BackgroundImageVisible;
			tempBorderInfo.ShowCaption = Owner.GroupBordersVisible ? (Owner.TextVisible | Owner.ExpandButtonVisible) : false;
			tempBorderInfo.ShowButton = Owner.ExpandButtonVisible;
			tempBorderInfo.Expanded = Owner.ExpandButtonMode != ExpandButtonMode.Inverted ? Owner.Expanded : !Owner.Expanded;
			tempBorderInfo.Caption = Owner.TextVisible ? Owner.Text : String.Empty; 
			tempBorderInfo.CaptionLocation = Owner.TextLocation;
			tempBorderInfo.AllowHtmlText = Owner.AllowHtmlStringInCaption;
			tempBorderInfo.AllowGlyphSkinning = Owner.GetAllowGlyphSkinning();
			tempBorderInfo.ShowCaptionImage = ImageInfo.ImageVisible;
			tempBorderInfo.CaptionImage = ImageInfo.Image;
			tempBorderInfo.CaptionImageLocation = ImageInfo.ImageLocation;
			tempBorderInfo.CaptionImagePadding = ImageInfo.ImagePadding;
			tempBorderInfo.AllowBorderColorBlending = Owner.AllowBorderColorBlending;
			tempBorderInfo.ButtonLocation = Owner.HeaderButtonsLocation;
			tempBorderInfo.ContentImage = Owner.ContentImage;
			tempBorderInfo.ContentImageAlignment = Owner.ContentImageAlignment;
			tempBorderInfo.BorderStyle = Owner.GroupBordersVisible ?
				DevExpress.XtraEditors.Controls.BorderStyles.Default :
				DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			if(Owner.IsRoot && !Owner.GroupBordersVisible) {
				tempBorderInfo.ShowCaption = false;
				tempBorderInfo.ShowButton = false;
			}
			if(!Owner.EnabledState)
				tempBorderInfo.State = ObjectState.Disabled;
			tempBorderInfo.RightToLeft = Owner.IsRTL;
			if(shouldHandleRaiseEvent) gbbp.RaiseEvents = true;
			if(tempBorderInfo.AppearanceCaption.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				tempBorderInfo.AppearanceCaption.TextOptions.HotkeyPrefix = HKeyPrefix.Show;
		}
		protected override void CalculatePaddingCore() {
			if(Owner.GroupBordersVisible)
				base.CalculatePaddingCore();
		}
		protected bool CanHideBorders {
			get {
				if(Owner == null) return false;
				if(Owner.Owner == null) return !Owner.GroupBordersVisible;
				return !Owner.GroupBordersVisible && !GetEnableIndents();
			}
		}
		protected bool GetEnableIndents() {
			if(Owner.EnableIndentsWithoutBorders == DefaultBoolean.Default)
				return Owner.Owner.OptionsView.EnableIndentsInGroupsWithoutBorders;
			else return (Owner.EnableIndentsWithoutBorders == DefaultBoolean.True);
		}
		protected override void CalculatePaddings() {
			base.CalculatePaddings();
			if(!Owner.GroupBordersVisible) {
				DevExpress.XtraLayout.Utils.Padding p = Owner.Padding;
				if(!Owner.Expanded) p = DevExpress.XtraLayout.Utils.Padding.Empty;
				Padding = p + Spaces;
			}
			if(CanHideBorders) { Padding = new DevExpress.XtraLayout.Utils.Padding(0); }
		}
		protected override void CalculateRegions() {
			if(!Owner.Expanded)
				ClientArea = Rectangle.Empty;
			else {
				Rectangle controlsAreaRect = Rectangle.Empty;
				controlsAreaRect.Size = Owner.Items.ItemsBounds.Size;
				if(Owner.LayoutMode != Utils.LayoutMode.Regular) {
					controlsAreaRect.Size = Owner.ParentTabbedGroup == null ? SubLabelSizeIndentions(Owner.Size) : Owner.Size;
				}
				controlsAreaRect.Y += Padding.Top;
				controlsAreaRect.X += Padding.Left;
				ClientArea = controlsAreaRect;
			}
		}
		public BaseLayoutItem GetItemAtPoint(Point pt) {
			CalculateViewInfoIfNeeded();
			for(int i = 0; i < Owner.Count; i++)
				if(Owner[i].Bounds.Contains(pt))
					return Owner[i];
			return null;
		}
		protected class CaptionImageInfo {
			LayoutGroup group;
			public CaptionImageInfo(LayoutGroup group) {
				this.group = group;
			}
			public virtual Image Image {
				get { return group.CaptionImage; }
			}
			public virtual bool ImageVisible {
				get { return group.CaptionImageVisible; }
			}
			public virtual GroupElementLocation ImageLocation {
				get { return group.CaptionImageLocation; }
			}
			public virtual System.Windows.Forms.Padding ImagePadding {
				get {
					return new System.Windows.Forms.Padding(
								group.CaptionImagePadding.Left, group.CaptionImagePadding.Top,
								group.CaptionImagePadding.Right, group.CaptionImagePadding.Bottom);
				}
			}
		}
	}
}
