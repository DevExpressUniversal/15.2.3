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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
namespace DevExpress.Xpf.Ribbon {
	[DevExpress.Mvvm.UI.Interactivity.TargetTypeAttribute(typeof(RibbonGalleryBarItem))]
	public class RibbonGalleryItemThemeSelectorBehavior : GalleryBarItemThemeSelectorBehavior<RibbonGalleryBarItem> {
		protected GalleryThemeSelectorBehavior DropDownGalleryBehavior {
			get {
				if(dropDownGalleryBehavior == null)
					dropDownGalleryBehavior = new GalleryThemeSelectorBehavior();
				return dropDownGalleryBehavior;
			}
		}
		protected Gallery DropDownGallery {
			get { return dropDownGallery; }
			set {
				if(dropDownGallery != value) {
					var oldValue = dropDownGallery;
					dropDownGallery = value;
					OnDropDownGalleryChanged(oldValue);
				}
			}
		}
		public object DropDownGalleryStyleKey {
			get { return dropDownGalleryStyleKey; }
			set {
				if(dropDownGalleryStyleKey != value) {
					dropDownGalleryStyleKey = value;
					OnDropDownGalleryStyleKeyChanged();
				}
			}
		}
		public RibbonGalleryItemThemeSelectorBehavior() {
			DropDownGalleryStyleKey = CreateDropDownGalleryStyleKey();
		}
		protected override void Initialize(RibbonGalleryBarItem item) {
			base.Initialize(item);
			Gallery = item.Gallery ?? (item.Gallery = new Gallery());
			Binding binding = new Binding("Gallery");
			binding.Source = item;
			BindingOperations.SetBinding(this, RibbonGalleryItemThemeSelectorBehavior.GalleryProperty, binding);
			item.DropDownGalleryInit += OnDropDownGalleryInit;
			item.DropDownGalleryClosed += OnDropDownGalleryClosed;
		}
		protected override void Clear(RibbonGalleryBarItem item) {
			base.Clear(item);
			item.DropDownGalleryInit -= OnDropDownGalleryInit;
			item.DropDownGalleryClosed -= OnDropDownGalleryClosed;
			DropDownGallery = null;
			Gallery = null;
		}
		protected override void OnThemesCollectionChanged(System.ComponentModel.ICollectionView oldValue) {
			base.OnThemesCollectionChanged(oldValue);
			DropDownGalleryBehavior.ThemesCollection = ThemesCollection;
		}
		void OnDropDownGalleryInit(object sender, DropDownGalleryEventArgs e) {
			DropDownGallery = e.DropDownGallery.Gallery;
		}
		void OnDropDownGalleryClosed(object sender, DropDownGalleryEventArgs e) {
			DropDownGallery = null;
		}
		protected virtual void OnDropDownGalleryChanged(Gallery oldValue) {
			if(oldValue != null)
				UninitializeGallery(oldValue, DropDownGalleryBehavior);
			if(DropDownGallery != null)
				InitializeGallery(DropDownGallery, DropDownGalleryBehavior);
		}
		protected virtual void OnDropDownGalleryStyleKeyChanged() {
			DropDownGalleryBehavior.StyleKey = DropDownGalleryStyleKey;
		}
		protected override object CreateStyleKey() {
			return new RibbonGalleryItemThemeSelectorThemeKeyExtension() { ResourceKey = RibbonGalleryItemThemeSelectorThemeKeys.Style, IsThemeIndependent = true };
		}
		protected override object CreateGalleryStyleKey() {
			return new GalleryThemeSelectorThemeKeyExtension() { ResourceKey = GalleryThemeSelectorThemeKeys.InRibbonGalleryStyle, IsThemeIndependent = true };
		}
		protected virtual object CreateDropDownGalleryStyleKey() {
			return new GalleryThemeSelectorThemeKeyExtension() { ResourceKey = GalleryThemeSelectorThemeKeys.InRibbonDropDownGalleryStyle, IsThemeIndependent = true };
		}
		Gallery dropDownGallery;
		GalleryThemeSelectorBehavior dropDownGalleryBehavior;
		object dropDownGalleryStyleKey;
	}
}
