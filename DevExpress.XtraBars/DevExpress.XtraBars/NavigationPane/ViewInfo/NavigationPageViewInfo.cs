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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
namespace DevExpress.XtraBars.Navigation {
	public class NavigationPageViewInfo : ObjectInfoArgs {
		INavigationPageBase ownerCore;
		bool backgroundSkinningCore;
		public INavigationPageBase Owner {
			get { return ownerCore; }
		}
		public NavigationPageViewInfo(INavigationPageBase owner) {
			ownerCore = owner;
			PaintAppearance = new FrozenAppearance();
			backgroundSkinningCore = true;
		}
		public AppearanceObject PaintAppearance { get; set; }
		public Rectangle ClientBounds { get; set; }
		public Rectangle CaptionBounds { get; set; }
		public Rectangle TextBounds { get; set; }
		public Rectangle BackgroundBounds { get; set; }
		public StringInfo TextInfo { get; set; }
		public bool IsAllowedBackgroundSkinning { 
			get { return backgroundSkinningCore; } 
			protected set { backgroundSkinningCore = value; } 
		}
		const int DefaultButtonToTextInterval = 5;
		public virtual void CalcNC(Graphics g, Rectangle bounds, ObjectPainter painter) {
			if(Owner is NavigationPage) {
				INavigationPage owner = Owner as NavigationPage;
				UpdateDefaultButtonsVisibility();
				Bounds = bounds;
				var navigationPane = Owner.Parent as INavigationFrame;
				bool rightToLeft = navigationPane != null && navigationPane.IsRightToLeftLayout();
				owner.ButtonsPanel.BeginUpdate();
				if(rightToLeft) {
					owner.ButtonsPanel.RightToLeft = true;
					owner.ButtonsPanel.ContentAlignment = ContentAlignment.MiddleLeft;
				}
				else {
					owner.ButtonsPanel.RightToLeft = false;
					owner.ButtonsPanel.ContentAlignment = ContentAlignment.MiddleRight;
				}
				owner.ButtonsPanel.CancelUpdate();
				var pagePainter = painter as NavigationPagePainter;
				if(Owner is NavigationPage)
					AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { (Owner as NavigationPage).Properties.ActualAppearanceCaption }, pagePainter.DefaultAppearanceCaption);
				PaintAppearance.TextOptions.RightToLeft = navigationPane != null && navigationPane.IsRightToLeft();
				if(PaintAppearance.TextOptions.HAlignment == HorzAlignment.Default)
					PaintAppearance.TextOptions.HAlignment = HorzAlignment.Near;
				owner.ButtonsPanel.Buttons.Merge(owner.CustomHeaderButtons);
				int buttonsHeight = owner.ButtonsPanel.ViewInfo.CalcMinSize(g).Height;
				Rectangle captionClientBounds = painter.GetObjectClientRectangle(this);
				Size textSize = CalcTextSize(g, captionClientBounds);
				captionClientBounds = new Rectangle(captionClientBounds.Location, new Size(captionClientBounds.Width, Math.Max(buttonsHeight, textSize.Height)));
				CaptionBounds = painter.CalcBoundsByClientRectangle(this, captionClientBounds);
				owner.ButtonsPanel.ViewInfo.SetDirty();
				owner.ButtonsPanel.ViewInfo.Calc(g, captionClientBounds);
				captionClientBounds.Width -= owner.ButtonsPanel.ViewInfo.Bounds.Width;
				if(rightToLeft) {
					captionClientBounds.X += owner.ButtonsPanel.ViewInfo.Bounds.Width + DefaultButtonToTextInterval;
					captionClientBounds.Width -= DefaultButtonToTextInterval;
				}
				textSize.Width = captionClientBounds.Width;
				TextBounds = PlacementHelper.Arrange(textSize, captionClientBounds, ContentAlignment.MiddleCenter);
				BackgroundBounds = new Rectangle(bounds.X, CaptionBounds.Bottom, bounds.Width, bounds.Height - CaptionBounds.Height);
				ClientBounds = GetBackgroundObjectClientRectangle(pagePainter, BackgroundBounds);
			}
			else {
				CalcNCWithoutCaption(g, bounds, painter);
			}
		}
		public void CalcNCWithoutCaption(Graphics g, Rectangle bounds, ObjectPainter painter) {
			backgroundSkinningCore = false;
			Bounds = bounds;
			var pagePainter = painter as NavigationPagePainter;
			if(Owner is NavigationPage)
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { (Owner as NavigationPage).Properties.ActualAppearanceCaption }, pagePainter.DefaultAppearanceCaption);
			BackgroundBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			ClientBounds = GetBackgroundObjectClientRectangle(pagePainter, BackgroundBounds);
		}
		void UpdateDefaultButtonsVisibility() {
			INavigationPage owner = Owner as NavigationPage;
			owner.ButtonsPanel.BeginUpdate();
			ExpandButton expandButton = null;
			CollapseButton collapseButton = null;
			foreach(var button in owner.ButtonsPanel.Buttons) {
				if(button is ExpandButton) expandButton = button as ExpandButton;
				if(button is CollapseButton) collapseButton = button as CollapseButton;
			}
			if(expandButton != null)
				expandButton.Visible = owner.Properties.CanShowExpandButton;
			if(collapseButton != null)
				collapseButton.Visible = owner.Properties.CanShowCollapseButton;
			owner.ButtonsPanel.CancelUpdate();
		}
		protected Rectangle GetBackgroundObjectClientRectangle(NavigationPagePainter painer, Rectangle backgroundBounds) {
			var sizingMargin = painer.SizingMargins;
			var padding = Owner.BackgroundPadding;
			var resultPadding = Padding.Add(sizingMargin, padding);
			return new Rectangle(backgroundBounds.X + resultPadding.Left, backgroundBounds.Y + resultPadding.Top,
				backgroundBounds.Width - resultPadding.Horizontal, backgroundBounds.Height - resultPadding.Vertical);
		}
		protected virtual Size CalcTextSize(Graphics g, Rectangle clientBounds) {
			clientBounds.Width = clientBounds.Width - 5 - (Owner as INavigationPage).ButtonsPanel.ViewInfo.Bounds.Width;
			if(Owner is NavigationPage && (Owner as NavigationPage).Properties.CanHtmlDraw) {
				TextInfo = StringPainter.Default.Calculate(g, PaintAppearance, PaintAppearance.TextOptions, Owner.Caption, clientBounds);
				return TextInfo.Bounds.Size;
			}
			return Size.Round(PaintAppearance.CalcTextSize(g, Owner.Caption, clientBounds.Width));
		}
	}
}
