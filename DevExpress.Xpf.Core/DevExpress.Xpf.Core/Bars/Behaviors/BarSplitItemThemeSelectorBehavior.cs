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

using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
using System;
using System.Windows.Data;
namespace DevExpress.Xpf.Bars {
	[TargetTypeAttribute(typeof(BarSplitButtonItem))]
	public class BarSplitItemThemeSelectorBehavior : GalleryBarItemThemeSelectorBehavior<BarSplitButtonItem> {
		protected override void Clear(BarSplitButtonItem item) {
			var popupContainer = item.PopupControl as PopupControlContainer;
			item.PopupControl = null;
			Gallery = null;
		}
		protected override void Initialize(BarSplitButtonItem item) {
			base.Initialize(item);
			PopupControlContainer containerControl = item.PopupControl as PopupControlContainer ?? new PopupControlContainer();
			item.PopupControl = containerControl;
			containerControl.CloseOnClick = true;
			GalleryControl galleryControl = containerControl.Content as GalleryControl ?? new GalleryControl();
			containerControl.Content = galleryControl;
			if (galleryControl.Gallery == null)
				 galleryControl.Gallery = new Gallery();
			Binding binding = new Binding("Gallery");
			binding.Source = galleryControl;
			BindingOperations.SetBinding(this, BarSplitItemThemeSelectorBehavior.GalleryProperty, binding);
		}
		protected override object CreateStyleKey() {
			return new BarSplitButtonItemThemeSelectorThemeKeyExtension() { ResourceKey = BarSplitButtonItemThemeSelectorThemeKeys.Style, IsThemeIndependent = true };
		}
		protected override object CreateGalleryStyleKey() {
			return new GalleryThemeSelectorThemeKeyExtension() { ResourceKey = GalleryThemeSelectorThemeKeys.InSplitButtonGalleryStyle, IsThemeIndependent = true };
		}
	}
}
