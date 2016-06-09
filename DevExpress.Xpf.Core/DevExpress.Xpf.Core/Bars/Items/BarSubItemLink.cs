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

using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using System.Windows;
namespace DevExpress.Xpf.Bars {
	public class BarSubItemLink : BarButtonItemLink {
		#region static
		public static readonly DependencyProperty IsOpenedProperty;
		private static readonly DependencyPropertyKey IsOpenedPropertyKey;
		public static readonly DependencyProperty ItemClickBehaviourProperty;		
		static BarSubItemLink() {
			IsOpenedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsOpened", typeof(bool), typeof(BarSubItemLink), new FrameworkPropertyMetadata(false));
			IsOpenedProperty = IsOpenedPropertyKey.DependencyProperty;
			ItemClickBehaviourProperty = DependencyPropertyManager.Register("ItemClickBehaviour", typeof(PopupItemClickBehaviour), typeof(BarSubItemLink), new FrameworkPropertyMetadata(PopupItemClickBehaviour.Undefined, (d, e) => ((BarSubItemLink)d).OnItemClickBehaviourChanged((PopupItemClickBehaviour)e.OldValue)));
		}
		#endregion
		public BarSubItemLink() { }
		protected internal BarSubItem SubItem {
			get { return base.Item as BarSubItem; }
			set { base.Item = value; }
		}
		protected internal BarSubItemLinkControl SubItemLinkControl {
			get { return base.LinkControl as BarSubItemLinkControl; }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSubItemLinkItemLinks")]
#endif
		public BarItemLinkCollection ItemLinks {
			get {
				if(SubItem != null)
					return ((ILinksHolder)SubItem).ActualLinks;
				return null;
			}
		}
		protected internal virtual bool CanOpenMenu { 
			get {
				RaiseGetItemData();
				return CanOpenMenuCore;
			} 
		}
		protected internal virtual bool CanOpenMenuCore {
			get {
				if(!IsEnabled) return false;
				foreach(BarItemLinkBase linkBase in ItemLinks) {
					if(linkBase.ActualIsVisible)
						return true;
				}
				return false;
			} 
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSubItemLinkIsOpened")]
#endif
		public bool IsOpened {
			get { return (bool)GetValue(IsOpenedProperty); }
			protected internal set { this.SetValue(IsOpenedPropertyKey, value); }
		}
		public PopupItemClickBehaviour ItemClickBehaviour {
			get { return (PopupItemClickBehaviour)GetValue(ItemClickBehaviourProperty); }
			set { SetValue(ItemClickBehaviourProperty, value); }
		}		
		internal void RaiseGetItemData() {
			if(SubItem != null) {
				SubItem.ItemLinks.BeginUpdate();
				SubItem.RaiseGetItemData();
				SubItem.ItemLinks.EndUpdate();
			}
		}
		internal void RaisePopup() {
			if(SubItem != null)
				SubItem.RaisePopup();
		}
		internal void RaiseCloseUp() {
			if(SubItem != null)
				SubItem.RaiseCloseUp();
		}
		protected virtual void OnItemClickBehaviourChanged(PopupItemClickBehaviour oldValue) {
			ExecuteActionOnLinkControls<BarSubItemLinkControl>(lc => lc.UpdateActualItemClickBehaviour());
		}
	}
}
