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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace DevExpress.Xpf.NavBar {
	public class NavBarGroupCollection : ObservableCollection<NavBarGroup> {		
		NavBarControl navBar;
		public NavBarGroupCollection(NavBarControl navBar) {
			this.navBar = navBar;
		}
		public NavBarGroup this[string name] {
			get {
				foreach(NavBarGroup group in this)
					if(group.Name == name) return group;
				return null;
			}
		}
		internal NavBarControl NavBar { get { return navBar; } }
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {			
			if(e.NewItems != null) {
				foreach(NavBarGroup group in e.NewItems) {
					if(group.NavBar != null && group.NavBar!=navBar) {
						throw new ArgumentException(NavBarLocalizer.GetString(NavBarStringId.GroupIsAlreadyAddedToAnotherNavBarException));
					}
					group.NavBar = navBar;
					group.IsActive = navBar == null ? false : navBar.ActiveGroup == group;
					navBar.AddChild(group);
					group.OwnerCollection = this;
					group.CoerceValue(NavBarGroup.IsEnabledProperty);
				}
			}
			base.OnCollectionChanged(e);
			if (e.OldItems != null) {
				foreach (NavBarGroup group in e.OldItems) {
					ClearGroupProperties(group);
				}
			}
			navBar.SelectionStrategy.SelectFirstVisibleGroup();
			if(navBar.View != null) navBar.View.UpdateViewForce();
			if (navBar.View != null && navBar.View is NavigationPaneView)
				navBar.RaisePropertyChanged("Items");
		}
		protected override void ClearItems() {
			while (navBar.Groups.Count != 0) {
				RemoveAt(0);
			}			
		}
		void ClearGroupProperties(NavBarGroup group) {
			navBar.RemoveChild(group);
			group.NavBar = null;
			group.IsActive = false;
			NavBar.SelectionStrategy.OnGroupRemoved(group);
			group.CoerceValue(NavBarGroup.IsEnabledProperty);
		}				
	}
}
