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
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Editors.Internal;
#if !SL
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#endif
namespace DevExpress.Xpf.Editors {
	public class WaitIndicatorItem : CustomItem {
		public static readonly DependencyProperty IsHitTestVisibleProperty;
		static WaitIndicatorItem() {
			IsHitTestVisibleProperty = DependencyPropertyManager.Register("IsHitTestVisible", typeof(bool), typeof(WaitIndicatorItem), new PropertyMetadata(true));
		}
		protected override ICustomItem GetCustomItem() {
			return new EditorCustomItem { DisplayValue = String.Empty, EditValue = null };
		}
		protected override Style GetItemStyleInternal() {
			FrameworkElement editor = (FrameworkElement)OwnerEdit;
			if(editor == null)
				return null;
			return (Style)editor.FindResource(new CustomItemThemeKeyExtension {
				ResourceKey = CustomItemThemeKeys.WaitIndicatorItemContainerStyle
			});
		}
		public bool IsHitTestVisible {
			get { return (bool)GetValue(IsHitTestVisibleProperty); }
			set { SetValue(IsHitTestVisibleProperty, value); }
		}
	}
}
