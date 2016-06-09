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
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public class BarClientPanel : Panel {
		BarItemsLayoutCalculator itemsCalculator;
		public BarClientPanel() {
			Loaded += new RoutedEventHandler(OnLoaded);
		}		
		protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if(Owner != null && Owner.Bar != null)
				Owner.CalcBarCustomizationButonsVisibility();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			DragManager.SetDropTargetFactory(this, new BarControlDropTargetFactoryExtension());
		}		
		protected internal BarItemsLayoutCalculator ItemsCalculator {
			get {
				if(itemsCalculator == null)
					itemsCalculator = CreateItemsCalculator();
				return itemsCalculator;
			}
		}
		public BarControl Owner { get { return LayoutHelper.FindParentObject<BarControl>(this); } }
		public BarContainerControl Container { get { return LayoutHelper.FindParentObject<BarContainerControl>(this); } }
		protected virtual BarItemsLayoutCalculator CreateItemsCalculator() {
			return BarItemsLayoutCalculator.CreatePanel(this, this.Owner);
		}
		protected override Size MeasureOverride(Size constraint) {
			if(Owner == null || Owner.Bar == null)
				return base.MeasureOverride(constraint);
			return ItemsCalculator.MeassureBar(constraint);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			return Owner == null ? base.ArrangeOverride(arrangeBounds) : ItemsCalculator.ArrangeBar(arrangeBounds);
		}
	}
}
