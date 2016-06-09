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
using System.Collections;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_TreeView", Type = typeof(psvTreeView))]
	public class LayoutTreeView : psvControl {
		#region static
		static LayoutTreeView() {
			var dProp = new DependencyPropertyRegistrator<LayoutTreeView>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		public LayoutTreeView() {
			DefaultStyleKey = typeof(LayoutTreeView);
		}
		protected override void OnDispose() {
			if(PartItemsControl != null) {
				PartItemsControl.ClearValue(ItemsControl.ItemsSourceProperty);
				PartItemsControl = null;
			}
			base.OnDispose();
		}
		protected ItemsControl PartItemsControl { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartItemsControl = GetTemplateChild("PART_TreeView") as ItemsControl;
			if(PartItemsControl != null && Container != null)
				PartItemsControl.ItemsSource = Container.CustomizationController.CustomizationItems;
		}
		protected IEnumerable GetCustomizationItems() {
			DockLayoutManager container = DockLayoutManager.Ensure(this, false);
			return container != null ? container.CustomizationController.CustomizationItems : null;
		}
	}
}
