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
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Reports.UserDesigner.FieldList;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Reports.UserDesigner.GroupSort {
	public class ClosePopupOnFieldSelected : Behavior<PopupControlContainer> {
		public static readonly DependencyProperty GridControlProperty;
		public GridControl GridControl {
			get { return (GridControl)GetValue(GridControlProperty); }
			set { SetValue(GridControlProperty, value); }
		}
		static ClosePopupOnFieldSelected() {
			DependencyPropertyRegistrator<ClosePopupOnFieldSelected>.New()
				.Register(owner => owner.GridControl, out GridControlProperty, null);
		}
		protected override void OnAttached() {
			base.OnAttached();
			if (GridControl == null)
				return;
			GridControl.CurrentItemChanged += OnCurrentItemChanged;
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			if (GridControl == null)
				return;
			GridControl.CurrentItemChanged -= OnCurrentItemChanged;
		}
		void OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e) {
			var item = e.NewItem as FieldListNodeBase<XRDiagramControl>;
			if (item == null)
				return;
			if (item.Children.Count() == 0)
				AssociatedObject.ClosePopup();
		}
	}
}
