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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
using System.Windows.Media;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Threading;
using DevExpress.Xpf.Bars;
using DevExpress.Mvvm.Native;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Bars;
#endif
namespace DevExpress.Xpf.Editors.Internal {
	public class GalleryBarItem : BarItem {
		public static readonly DependencyProperty GalleryProperty;
		static GalleryBarItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(GalleryBarItem), typeof(GalleryBarItemLink), delegate(object arg) { return new GalleryBarItemLink(); });
			GalleryProperty = DependencyPropertyManager.Register("Gallery", typeof(Gallery), typeof(GalleryBarItem), new PropertyMetadata(null));
		}
		public Gallery Gallery {
			get { return (Gallery)GetValue(GalleryProperty); }
			set { SetValue(GalleryProperty, value); }
		}
	}
	public class GalleryBarItemLink : BarItemLink {
		#region static
		static GalleryBarItemLink() {
			BarItemLinkControlCreator.Default.RegisterObject(typeof(GalleryBarItemLink), typeof(GalleryBarItemLinkControl), delegate(object arg) { return new GalleryBarItemLinkControl((GalleryBarItemLink)arg); });
		}
		#endregion
		public GalleryBarItemLink() { }
		protected internal GalleryBarItem GalleryItem { get { return base.Item as GalleryBarItem; } }
	}
	public class GalleryBarItemLinkControl : BarItemLinkControl {
		#region static
		public static readonly DependencyProperty HasTopBorderProperty;
		static readonly DependencyPropertyKey HasTopBorderPropertyKey; 
		public static readonly DependencyProperty HasBottomBorderProperty;
		static readonly DependencyPropertyKey HasBottomBorderPropertyKey;
		static GalleryBarItemLinkControl() {
			HasTopBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasTopBorder", typeof(bool), typeof(GalleryBarItemLinkControl), new PropertyMetadata(true));
			HasTopBorderProperty = HasTopBorderPropertyKey.DependencyProperty;
			HasBottomBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasBottomBorder", typeof(bool), typeof(GalleryBarItemLinkControl), new PropertyMetadata(true));
			HasBottomBorderProperty = HasBottomBorderPropertyKey.DependencyProperty;
		}
		#endregion
		public GalleryBarItemLinkControl(GalleryBarItemLink link)
			: base(link) {
			DefaultStyleKey = typeof(GalleryBarItemLinkControl);
#if !SL
			LayoutUpdated += OnLayoutUpdated;
#endif
		}
#if !SL
		void OnLayoutUpdated(object sender, EventArgs e) {
			UpdateBorderVisibility();   
		}
#else 
		protected override void OnLayoutUpdated(object sender, EventArgs e) {
			base.OnLayoutUpdated(sender, e);
			UpdateBorderVisibility();   
		}
#endif
		protected object GetTemplateFromProvider(DependencyProperty prop) {
			return GetValue(prop);
		}
		protected internal override void UpdateTemplateByContainerType(LinkContainerType type) {
			base.UpdateTemplateByContainerType(type);
			Template = (ControlTemplate)GetTemplateFromProvider(BarItemLinkControlTemplateProvider.TemplateInMenuProperty);
		}
		public bool HasTopBorder {
			get { return (bool)GetValue(HasTopBorderProperty); }
			private set { this.SetValue(HasTopBorderPropertyKey, value); }
		}
		public bool HasBottomBorder {
			get { return (bool)GetValue(HasBottomBorderProperty); }
			private set { this.SetValue(HasBottomBorderPropertyKey, value); }
		}
		public GalleryBarItemLink GalleryLink { get { return base.Link as GalleryBarItemLink; } }
		protected GalleryControl GalleryControl { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			GalleryControl = GetTemplateChild("PART_GalleryControl") as GalleryControl;
			GalleryControl.Do(x => x.AllowCyclicNavigation = false);
		}
		protected override void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateBorderVisibility();
		}
		protected void UpdateBorderVisibility() {
			if(LinksControl == null) return;
			bool topBorderVisible = false, bottomBorderVisible = false;
			int indexOfGalleryLink = LinksControl.ItemLinks.IndexOf(GalleryLink);
			for(int i = 0; i < LinksControl.ItemLinks.Count; i++) {
				BarItemLinkBase link = LinksControl.ItemLinks[i] as BarItemLinkBase;
				if(i < indexOfGalleryLink && link.ActualIsVisible)
					topBorderVisible = true;
				if(i > indexOfGalleryLink && link.ActualIsVisible)
					bottomBorderVisible = true;
			}
			HasTopBorder = topBorderVisible;
			HasBottomBorder = bottomBorderVisible;
		}
		protected internal override bool GetIsSelectable() { return true; }
		protected internal override INavigationOwner GetBoundOwner() { return GalleryControl; }
		protected override bool SetFocus() {
			if (NavigationTree.CurrentElement is GalleryItemControl)
				return false;
			return NavigationTree.SelectElement(GalleryControl);
		}
	}
	public class ColorGalleryItem : GalleryItem {
		public static readonly DependencyProperty ColorProperty;
		public static readonly DependencyProperty HideBorderSideProperty;
		static ColorGalleryItem() {
			Type ownerType = typeof(ColorGalleryItem); 
			ColorProperty = DependencyPropertyManager.Register("Color", typeof(Color), ownerType);
			HideBorderSideProperty = DependencyPropertyManager.Register("HideBorderSide", typeof(HideBorderSide), ownerType, new PropertyMetadata(HideBorderSide.All));
		}
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public HideBorderSide HideBorderSide {
			get { return (HideBorderSide)GetValue(HideBorderSideProperty); }
			set { SetValue(HideBorderSideProperty, value); }
		}
	}
}
