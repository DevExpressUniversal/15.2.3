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

using System.Windows;
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public class BarSplitCheckItem : BarSplitButtonItem, IBarCheckItem {
		BarCheckItemHelper helper;
		#region static        
		public static readonly RoutedEvent CheckedChangedEvent;
		public static readonly DependencyProperty AllowUncheckInGroupProperty;
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty IsThreeStateProperty;
		public static readonly DependencyProperty GroupIndexProperty;
		static BarSplitCheckItem() {
			CheckedChangedEvent = BarCheckItem.CheckedChangedEvent.AddOwner(typeof(BarSplitCheckItem));
			GroupIndexProperty = BarCheckItem.GroupIndexProperty.AddOwner(typeof(BarSplitCheckItem));
			AllowUncheckInGroupProperty = BarCheckItem.AllowUncheckInGroupProperty.AddOwner(typeof(BarSplitCheckItem));
			IsThreeStateProperty = BarCheckItem.IsThreeStateProperty.AddOwner(typeof(BarSplitCheckItem), new FrameworkPropertyMetadata(false, (d, e) => ((BarItem)d).UpdateProperties()));
			IsCheckedProperty = BarCheckItem.IsCheckedProperty.AddOwner(typeof(BarSplitCheckItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ((d, e) => ((BarSplitCheckItem)d).OnIsCheckedChanged(e)), (d, e) => ((BarSplitCheckItem)d).OnIsCheckedCoerce(e)));			
		}
		#endregion
		#region dep props
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSplitCheckItemAllowUncheckInGroup")]
#endif
		public bool AllowUncheckInGroup {
			get { return (bool)GetValue(AllowUncheckInGroupProperty); }
			set { SetValue(AllowUncheckInGroupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarSplitCheckItemIsChecked"),
#endif
 TypeConverter(typeof(NullableBoolConverter))]
		public bool? IsChecked {
			get { return (bool?)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, (bool?)value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSplitCheckItemIsThreeState")]
#endif
		public bool IsThreeState {
			get { return (bool)GetValue(IsThreeStateProperty); }
			set { base.SetValue(IsThreeStateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarSplitCheckItemGroupIndex"),
#endif
 DefaultValue(-1)]
		public int GroupIndex {
			get { return (int)GetValue(GroupIndexProperty); }
			set { SetValue(GroupIndexProperty, value); }
		}
		public event ItemClickEventHandler CheckedChanged {
			add { AddHandler(CheckedChangedEvent, value); }
			remove { RemoveHandler(CheckedChangedEvent, value); }
		}
		#endregion        
		public BarSplitCheckItem() {
			helper = new BarCheckItemHelper(this);
		}
		public virtual void Toggle() { helper.Toggle(); }
		protected virtual object OnIsCheckedCoerce(object value) { return helper.OnIsCheckedCoerce(value); }
		protected virtual void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e) { helper.OnIsCheckedChanged(e); }
		protected internal virtual void RaiseItemCheckedChanged() { helper.RaiseItemCheckedChanged(); }
		protected internal override void OnItemClick(BarItemLink link) {
			helper.OnItemClick(link);
			base.OnItemClick(link);
		}
	}
}
