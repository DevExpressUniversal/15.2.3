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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonQuickAccessToolbar : BarItemLinkHolderBase, ILinksHolder {
		#region static
		static RibbonQuickAccessToolbar() {
			EventManager.RegisterClassHandler(typeof(RibbonQuickAccessToolbar), DevExpress.Xpf.Core.Serialization.DXSerializer.StartDeserializingEvent, new RoutedEventHandler(OnStartDeserializingEvent));
		}
		private static void OnStartDeserializingEvent(object sender, RoutedEventArgs e) {
			((RibbonQuickAccessToolbar)sender).ItemLinks.Clear();
		}
		#endregion
		public RibbonQuickAccessToolbar() { }
		RibbonControl ribbon;
		public RibbonControl Ribbon {
			get { return ribbon; }
			protected internal set {
				if(Ribbon == value) return;
				RibbonControl oldValue = ribbon;
				ribbon = value;
				OnRibbonChanged(oldValue);
			}
		}
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {  }		
		RibbonQuickAccessToolbarControl control;
		protected internal RibbonQuickAccessToolbarControl Control {
			get {
				if(control == null)
					control = CreateControl();
				return control;
			}
		}
		protected internal virtual RibbonQuickAccessToolbarControl CreateControl() {
			return new RibbonQuickAccessToolbarControl(this);
		}
		protected override void OnMergedLinksHoldersChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			BarItemLinkMergeHelper helper = new BarItemLinkMergeHelper();
			helper.Merge(((ILinksHolder)this).Links, MergedLinksHolders, ((ILinksHolder)this).MergedLinks);
			if(Control != null)
				Control.RecreateItemsSource(((ILinksHolder)this).ActualLinks);
		}
		protected internal virtual void Remerge() {
			OnMergedLinksHoldersChanged(null, null);
		}
		internal void SetRibbonStyle(RibbonStyle ribbonStyle) {
			if(Control != null)
				Control.RibbonStyle = ribbonStyle;
		}
		internal void SetPlacement(RibbonQuckAccessToolbarPlacement placement) {
			if(Control != null)
				Control.Placement = placement;
		}
		LinksHolderType ILinksHolder.HolderType { get { return LinksHolderType.RibbonQuickAccessToolbar; } }
		internal double GetMinDesiredWidth() {
			if(Control == null)
				return 0d;
			return Control.GetMinDesiredWidth();
		}
	}
}
