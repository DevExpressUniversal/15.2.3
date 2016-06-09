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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonShowModeSelectorItem : Button {
		static RibbonShowModeSelectorItem() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonShowModeSelectorItem), new FrameworkPropertyMetadata(typeof(RibbonShowModeSelectorItem)));
		}
		public BarCheckItem AutohideRibbonItem {
			get {
				if(autohideRibbonItem == null) {
					autohideRibbonItem = new BarCheckItem();
					autohideRibbonItem.Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonShowModeSelector_AutoHideMode);
					autohideRibbonItem.IsPrivate = true;
					autohideRibbonItem.Command = ActivateAutoHideModeCommand;
					autohideRibbonItem.Glyph = ActualAutohideGlyph;
					autohideRibbonItem.GlyphSize = GlyphSize.Large;
				}
				return autohideRibbonItem;
			}
		}
		public ImageSource ActualNormalGlyph {
			get {
				if(Ribbon == null)
					return ImageHelper.GetImage("mono_2.png");
				switch(Ribbon.MenuIconStyle) {
					case RibbonMenuIconStyle.Mono:
						return ImageHelper.GetImage("mono_2.png");
					case RibbonMenuIconStyle.Color:
						return ImageHelper.GetImage("color_2.png");
					case RibbonMenuIconStyle.Office2013:
						return ImageHelper.GetImage("off2013_2.png");
					default:
						return null;
				}
			}
		}
		public ImageSource ActualMinimizedGlyph {
			get {
				if(Ribbon == null)
					return ImageHelper.GetImage("mono_1.png");
				switch(Ribbon.MenuIconStyle) {
					case RibbonMenuIconStyle.Mono:
						return ImageHelper.GetImage("mono_1.png");
					case RibbonMenuIconStyle.Color:
						return ImageHelper.GetImage("color_1.png");
					case RibbonMenuIconStyle.Office2013:
						return ImageHelper.GetImage("off2013_1.png");
					default:
						return null;
				}
			}
		}
		public ImageSource ActualAutohideGlyph {
			get {
				if(Ribbon == null)
					return ImageHelper.GetImage("mono_3.png");
				switch(Ribbon.MenuIconStyle) {
					case RibbonMenuIconStyle.Mono:
						return ImageHelper.GetImage("mono_3.png");
					case RibbonMenuIconStyle.Color:
						return ImageHelper.GetImage("color_3.png");
					case RibbonMenuIconStyle.Office2013:
						return ImageHelper.GetImage("off2013_3.png");
					default:
						return null;
				}
			}
		}
		public BarCheckItem NormalRibbonItem {
			get {
				if(normalRibbonItem == null) {
					normalRibbonItem = new BarCheckItem();
					normalRibbonItem.Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonShowModeSelector_NormalMode);
					normalRibbonItem.IsPrivate = true;
					normalRibbonItem.Command = ActivateNormalModeCommand;
					normalRibbonItem.Glyph = ActualNormalGlyph;
					normalRibbonItem.GlyphSize = GlyphSize.Large;
				}
				return normalRibbonItem;
			}
		}
		public BarCheckItem MinimizedRibbonItem {
			get {
				if(minimizedRibbonItem == null) {
					minimizedRibbonItem = new BarCheckItem();
					minimizedRibbonItem.Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonShowModeSelector_MinimizedMode);
					minimizedRibbonItem.IsPrivate = true;
					minimizedRibbonItem.Command = ActivateMinimizedModeCommand;
					minimizedRibbonItem.Glyph = ActualMinimizedGlyph;
					minimizedRibbonItem.GlyphSize = GlyphSize.Large;
				}
				return minimizedRibbonItem;
			}
		}
		public RibbonControl Ribbon { get { return (Window.GetWindow(this) as DXRibbonWindow).With(x => x.Ribbon); } }
		public ICommand ActivateNormalModeCommand { get; set; }
		public ICommand ActivateMinimizedModeCommand { get; set; }
		public ICommand ActivateAutoHideModeCommand { get; set; }
		protected override void OnClick() {
			base.OnClick();
			if(Ribbon != null) {
				AutohideRibbonItem.IsChecked = Ribbon.AutoHideMode;
				NormalRibbonItem.IsChecked = !Ribbon.IsMinimized && !Ribbon.AutoHideMode;
				MinimizedRibbonItem.IsChecked = Ribbon.IsMinimized;
			}
			var popup = new PopupMenu();
			popup.Items.Add(NormalRibbonItem);
			popup.Items.Add(MinimizedRibbonItem);
			popup.Items.Add(AutohideRibbonItem);
			popup.ShowPopup(this);
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			ActivateAutoHideModeCommand = new DelegateCommand(ActivateAutoHide);
			ActivateMinimizedModeCommand = new DelegateCommand(ActivateMinimized);
			ActivateNormalModeCommand = new DelegateCommand(ActivateNormal);
		}
		void ActivateNormal() {
			Ribbon.SetCurrentValue(RibbonControl.AutoHideModeProperty, false);
			Ribbon.SetValue(RibbonControl.IsMinimizedProperty, false);
		}
		void ActivateMinimized() {
			Ribbon.SetValue(RibbonControl.IsMinimizedProperty, true);
			Ribbon.SetCurrentValue(RibbonControl.AutoHideModeProperty, false);
		}
		void ActivateAutoHide() {
			Ribbon.SetCurrentValue(RibbonControl.AutoHideModeProperty, true);
		}
		BarCheckItem autohideRibbonItem, normalRibbonItem, minimizedRibbonItem;
	}
}
