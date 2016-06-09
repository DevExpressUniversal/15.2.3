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
using System.Windows.Controls;
using DevExpress.Xpf.Docking.Images;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class HiddenItem : Control, IUIElement {
		#region static
		static HiddenItem() {
			var dProp = new DependencyPropertyRegistrator<HiddenItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		public HiddenItem() { 
		}
		protected internal BaseLayoutItem Item { get; set; }
		public Image PartImage { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Item = DataContext as BaseLayoutItem;
			PartImage = GetTemplateChild("PART_Image") as Image;
			if(PartImage != null)
				PartImage.Source = ImageHelper.GetImageForItem(Item);
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return DockLayoutManager.GetUIScope(this); } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get {
				if(uiChildren == null) uiChildren = new UIChildren();
				return uiChildren;
			}
		}
		#endregion IUIElement
	}
}
