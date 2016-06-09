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
using System.Windows.Data;
using DevExpress.Xpf.Utils;
using System.Windows.Controls;
namespace DevExpress.Xpf.NavBar {
	public class NavBarGroupExpandedChangedEventArgs : RoutedEventArgs {
		public NavBarGroupExpandedChangedEventArgs(NavBarGroup group, bool isExpanded) {
			Group = group;
			IsExpanded = isExpanded;
		}
		public NavBarGroup Group { get; internal set; }
		public bool IsExpanded { get; internal set; }
	}
	public class NavBarGroupExpandedChangingEventArgs : NavBarGroupExpandedChangedEventArgs {
		public NavBarGroupExpandedChangingEventArgs(NavBarGroup group, bool isExpanded)
			: base(group, isExpanded) {
		}
		public bool Cancel { get; set; }		
	}
	public delegate void NavBarGroupExpandedChangedEventHandler(object sender, NavBarGroupExpandedChangedEventArgs e);
	public delegate void NavBarGroupExpandedChangingEventHandler(object sender, NavBarGroupExpandedChangingEventArgs e);
	public partial class ExplorerBarView : NavBarViewBase, IScrollMode {
		public static RoutedEvent GroupExpandedChangingEvent;
		public static RoutedEvent GroupExpandedChangedEvent;
		static ExplorerBarView() {
			GroupExpandedChangingEvent = EventManager.RegisterRoutedEvent("GroupExpandedChangingEvent", RoutingStrategy.Direct, typeof(NavBarGroupExpandedChangingEventHandler), typeof(ExplorerBarView));
			GroupExpandedChangedEvent = EventManager.RegisterRoutedEvent("GroupExpandedChangedEvent", RoutingStrategy.Direct, typeof(NavBarGroupExpandedChangedEventHandler), typeof(ExplorerBarView));			
		}
		ScrollControl IScrollMode.ScrollControl {
			get { return (ScrollControl)GetTemplateChild("scrollControl"); }
		}
		public ExplorerBarView() {
			this.SetDefaultStyleKey(typeof(ExplorerBarView));
			NavBarViewKind = NavBarViewKind.ExplorerBar;
		}
		public event NavBarGroupExpandedChangingEventHandler GroupExpandedChanging {
			add { AddHandler(GroupExpandedChangingEvent, value); }
			remove { RemoveHandler(GroupExpandedChangingEvent, value); }
		}
		public event NavBarGroupExpandedChangedEventHandler GroupExpandedChanged {
			add { AddHandler(GroupExpandedChangedEvent, value); }
			remove { RemoveHandler(GroupExpandedChangedEvent, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateScrollModeStates();
		}
		void UpdateScrollModeStates() {
			ScrollingSettings.UpdateScrollModeStates(this);
		}
	}
   public class ExplorerBarExpandButton : ExpandButtonBase {
	   public ExplorerBarExpandButton() {
			this.SetDefaultStyleKey(typeof(ExplorerBarExpandButton));
			Click += ExplorerBarExpandButtonClick;
		}
		void ExplorerBarExpandButtonClick(object sender, RoutedEventArgs e) {
			NavBarGroup group = this.DataContext as NavBarGroup;
			if(group != null)
				group.ChangeGroupExpanded();
			e.Handled = true;
		}
		protected internal override void SetBindings() {
			SetBinding(IsExpandedProperty, new Binding("IsExpanded"));
		}
	}
}
