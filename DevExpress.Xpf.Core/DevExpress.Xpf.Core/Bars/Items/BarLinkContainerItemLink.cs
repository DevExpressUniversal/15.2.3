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
using System.ComponentModel;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Bars {
	public class BarLinkContainerItemLink : BarItemLink {
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarLinkContainerItemLinkContainerItem")]
#endif
		public BarLinkContainerItem ContainerItem {
			get { return base.Item as BarLinkContainerItem; }
			protected internal set { base.Item = value; }
		}
		protected override void OnActualIsVisibleChanged(DependencyPropertyChangedEventArgs e) {
			base.OnActualIsVisibleChanged(e);
			if(ContainerItem != null) {
				foreach(BarItemLinkBase link in ContainerItem.ItemLinks) {
					link.ExecuteActionOnBaseLinkControls((lc) => lc.UpdateVisibility());
				}				
			}
		}
		protected override void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			base.OnIsEnabledChanged(sender, e);
			if(ContainerItem != null) {
				foreach(BarItemLinkBase link in ContainerItem.ItemLinks) {
					link.ExecuteActionOnBaseLinkControls((lc) => lc.UpdateIsEnabled());
				}
			}
		}
		protected override void OnItemChanged(BarItem oldValue) {
			base.OnItemChanged(oldValue);
			if(oldValue != Item && Item != null) {
				List<BarItemLinkInfo> linkInfos = LinkInfos.ToList<BarItemLinkInfo>();
				foreach(BarItemLinkInfo linkInfo in linkInfos) {
					linkInfo.OwnerCollection.UpdateFromSource(new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Replace, this, this, linkInfo.OwnerCollection.Source.IndexOf(this))
					);
				}
			}
		}
	}
}
