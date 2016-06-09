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
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	class BarCheckItemHelper {
		BarItem Owner { get;set; }
		IBarCheckItem COwner { get { return ((IBarCheckItem)Owner); } }
		bool IsEnabled { get { return (bool)Owner.GetValue(ContentElement.IsEnabledProperty); } }
		bool? IsChecked { get { return (bool?)Owner.GetValue(BarCheckItem.IsCheckedProperty); } set { Owner.SetValue(BarCheckItem.IsCheckedProperty,value); } }
		int GroupIndex { get { return (int)Owner.GetValue(BarCheckItem.GroupIndexProperty); } set { Owner.SetValue(BarCheckItem.GroupIndexProperty, value); } }
		bool AllowUncheckInGroup { get { return (bool)Owner.GetValue(BarCheckItem.AllowUncheckInGroupProperty); } set { Owner.SetValue(BarCheckItem.AllowUncheckInGroupProperty, value); } }
		bool IsThreeState { get { return (bool)Owner.GetValue(BarCheckItem.IsThreeStateProperty); } set { Owner.SetValue(BarCheckItem.IsThreeStateProperty, value); } }
		public BarCheckItemHelper(BarItem owner) {
			Owner = owner;
			if (!(Owner is IBarCheckItem))
				throw new ArgumentException("owner");
		}
		public virtual void Toggle() {
			if (!IsEnabled) return;
			if (IsChecked.HasValue && IsChecked.Value && (GroupIndex > -1) && !AllowUncheckInGroup)
				return;
			if (IsChecked == true)
				SetCurrentValue(BarCheckItem.IsCheckedProperty, IsThreeState ? null : ((bool?)false));
			else
				SetCurrentValue(BarCheckItem.IsCheckedProperty, new bool?(IsChecked.HasValue));
		}
		public object OnIsCheckedCoerce(object value) {
			if (GroupIndex > -1) {
				bool? newValue = (bool?)value;
				if (newValue == false || newValue == null)
					return BarNameScope.GetService<IRadioGroupService>(Owner).CanUncheck(COwner) ? value : IsChecked;
			}
			return value;
		}
		public void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e) {
			RaiseItemCheckedChanged();
			foreach (BarItemLinkBase link in Owner.Links) {
				if (link is IBarCheckItemLink) {
					((IBarCheckItemLink)link).UpdateCheckItemProperties();
				}
			}
			if (Equals(true, IsChecked))
				BarNameScope.GetService<IRadioGroupService>(Owner).OnChecked(COwner);
			Owner.ExecuteActionOnLinkControls<BarItemLinkControlBase>(lc => (lc as IBarCheckItemLinkControl).Do(x => x.OnSourceIsCheckedChanged()));
		}
		public void RaiseItemCheckedChanged() {
			Owner.RaiseEvent(new ItemClickEventArgs(Owner, isCheckedChangeLink) { RoutedEvent = BarCheckItem.CheckedChangedEvent });			
		}
		public void OnItemClick(BarItemLink link) {
			isCheckedChangeLink = link;
			Toggle();
			isCheckedChangeLink = null;
		}
		void SetCurrentValue(DependencyProperty property, object value) {
			Owner.SetCurrentValue(property, value);
		}
		BarItemLink isCheckedChangeLink;
	}
	public class BarCheckItem : BarButtonItem, IBarCheckItem {
		BarCheckItemHelper helper;
		#region static        
		public static readonly RoutedEvent CheckedChangedEvent;		
		public static readonly DependencyProperty AllowUncheckInGroupProperty;
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty IsThreeStateProperty;
		public static readonly DependencyProperty GroupIndexProperty;
		static BarCheckItem() {
			CheckedChangedEvent = EventManager.RegisterRoutedEvent("CheckedChanged", RoutingStrategy.Direct, typeof(ItemClickEventHandler), typeof(BarCheckItem));
			AllowUncheckInGroupProperty = DependencyPropertyManager.Register("AllowUncheckInGroup", typeof(bool), typeof(BarCheckItem), new FrameworkPropertyMetadata(false));
			IsCheckedProperty = DependencyPropertyManager.Register("IsChecked", typeof(bool?), typeof(BarCheckItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ((d, e) => ((BarCheckItem)d).OnIsCheckedChanged(e)), (d, e) => ((BarCheckItem)d).OnIsCheckedCoerce(e)));
			IsThreeStateProperty = DependencyPropertyManager.Register("IsThreeState", typeof(bool), typeof(BarCheckItem), new FrameworkPropertyMetadata(false, (d, e) => ((BarCheckItem)d).UpdateProperties()));
			GroupIndexProperty = DependencyPropertyManager.Register("GroupIndex", typeof(int), typeof(BarCheckItem), new FrameworkPropertyMetadata(-1));
		}
		#endregion
		#region dep props
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarCheckItemAllowUncheckInGroup")]
#endif
		public bool AllowUncheckInGroup {
			get { return (bool)GetValue(AllowUncheckInGroupProperty); }
			set { SetValue(AllowUncheckInGroupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarCheckItemIsChecked"),
#endif
 TypeConverter(typeof(NullableBoolConverter))]
		public bool? IsChecked {
			get { return (bool?)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, (bool?)value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarCheckItemIsThreeState")]
#endif
		public bool IsThreeState {
			get { return (bool)GetValue(IsThreeStateProperty); }
			set { base.SetValue(IsThreeStateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("BarCheckItemGroupIndex"),
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
		public BarCheckItem() {
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
