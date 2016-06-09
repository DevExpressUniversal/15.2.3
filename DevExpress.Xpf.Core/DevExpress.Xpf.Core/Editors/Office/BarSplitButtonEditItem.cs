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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Input;
using DevExpress.Office.Internal;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using ObjectConverter = DevExpress.Xpf.Core.WPFCompatibility.ObjectConverter;
#else
using ObjectConverter = DevExpress.Xpf.Core.ObjectConverter;
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Office.UI {
	#region BarSplitButtonEditItem
	[DXToolboxBrowsable(false)]
	public class BarSplitButtonEditItem : BarSplitButtonItem, IEditValueBarItem {
		public static readonly DependencyProperty EditValueProperty;
		static BarSplitButtonEditItem() {
			Type ownerType = typeof(BarSplitButtonEditItem);
#if !SL
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnEditValueChanged), OnCoerceEditValue, true, UpdateSourceTrigger.LostFocus));
#else
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnEditValueChanged), OnCoerceEditValue));
#endif
		}
		[TypeConverter(typeof(ObjectConverter))]
		public object EditValue {
			get { return GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		#region EditValueChanged
		EditValueChangedEventHandler onEditValueChanged;
		public event EditValueChangedEventHandler EditValueChanged { add { onEditValueChanged += value; } remove { onEditValueChanged -= value; } }
		protected internal virtual void RaiseEditValueChanged(object oldValue, object newValue) {
			if (onEditValueChanged != null)
				onEditValueChanged(this, new EditValueChangedEventArgs(oldValue, newValue));
		}
		#endregion
		protected static void OnEditValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarSplitButtonEditItem)obj).OnEditValueChanged(e.OldValue, e.NewValue);
		}
		protected static object OnCoerceEditValue(DependencyObject obj, object value) {
			return ((BarSplitButtonEditItem)obj).CoerceEditValue(obj, value);
		}
		protected virtual void OnEditValueChanged(object oldValue, object newValue) {
			RaiseEditValueChanged(oldValue, newValue);
			UpdateLinkControls();
		}
		protected internal virtual void UpdateLinkControls() {
			ReadOnlyLinkCollection links = this.Links;
			int count = links.Count;
			for (int i = 0; i < count; i++)
				UpdateLinkControl(links[i]);
		}
		protected internal virtual void UpdateLinkControl(BarItemLinkBase link) {
		}
		protected virtual object CoerceEditValue(DependencyObject d, object value) {
			return value;
		}
	}
	#endregion
	#region BarButtonColorEditItem
	[DXToolboxBrowsable(false)]
	public class BarButtonColorEditItem : BarSplitButtonEditItem {
		static BarButtonColorEditItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(BarButtonColorEditItem), typeof(BarButtonColorEditItemLink), delegate(object arg) { return new BarButtonColorEditItemLink(); });
		}
		public BarButtonColorEditItem() {
			ActAsDropDown = true;
		}
		public override BarItemLink CreateLink(bool isPrivate) {
			return new BarButtonColorEditItemLink();
		}
	}
	#endregion
	#region BarButtonColorEditItemLink
	[DXToolboxBrowsable(false)]
	public class BarButtonColorEditItemLink : BarSplitButtonItemLink {
		public BarButtonColorEditItemLink() {
		}
		static BarButtonColorEditItemLink() {
			BarItemLinkControlCreator.Default.RegisterObject(typeof(BarButtonColorEditItemLink), typeof(BarButtonColorEditItemLinkControl), delegate(object arg) { return new BarButtonColorEditItemLinkControl((BarButtonColorEditItemLink)arg); });
		}
		protected internal override BarItemLinkControlBase CreateBarItemLinkControl() {
			return new BarButtonColorEditItemLinkControl(this);
		}
		protected internal override void OnClick() {
		}
	}
	#endregion
	#region BarButtonColorEditItemLinkControl
	[DXToolboxBrowsable(false)]
	public class BarButtonColorEditItemLinkControl : BarSplitButtonItemLinkControl {
		public BarButtonColorEditItemLinkControl(BarSplitButtonItemLink link)
			: base(link) {
		}
	}
	#endregion
}
namespace DevExpress.Office.Internal {
	public interface IEditValueBarItem {
		object EditValue { get; set; }
		event EditValueChangedEventHandler EditValueChanged;
		ICommand Command { get; set; }
	}
}
