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

using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Bars {
	public class BarSplitButtonItemLink : BarButtonItemLink {
		#region
		public static readonly DependencyProperty ActAsDropDownProperty;
		static readonly DependencyPropertyKey ActAsDropDownPropertyKey;
		public static readonly DependencyProperty UserArrowAlignmentProperty;
		public static readonly DependencyProperty ItemClickBehaviourProperty;		
		static BarSplitButtonItemLink() {
			ActAsDropDownPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActAsDropDown", typeof(bool), typeof(BarSplitButtonItemLink), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnActAsDropDownPropertyChanged)));
			ActAsDropDownProperty = ActAsDropDownPropertyKey.DependencyProperty;
			UserArrowAlignmentProperty = DependencyPropertyManager.Register("UserArrowAlignment", typeof(Dock?), typeof(BarSplitButtonItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnArrowAlignmentPropertyChanged), new CoerceValueCallback(OnArrowAlignmentPropertyCoerce)));
			ItemClickBehaviourProperty = DependencyPropertyManager.Register("ItemClickBehaviour", typeof(PopupItemClickBehaviour), typeof(BarSplitButtonItemLink), new FrameworkPropertyMetadata(PopupItemClickBehaviour.Undefined, (d, e) => ((BarSplitButtonItemLink)d).OnItemClickBehaviourChanged((PopupItemClickBehaviour)e.OldValue)));
		}
		protected static void OnActAsDropDownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarSplitButtonItemLink)d).OnActAsDropDownChanged(e);
		}
		protected static void OnArrowAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarSplitButtonItemLink)d).OnArrowAlignmentChanged(e);
		}
		protected static object OnArrowAlignmentPropertyCoerce(DependencyObject d, object baseValue) {
			return ((BarSplitButtonItemLink)d).OnArrowAlignmentCoerce((Dock?)baseValue);
		}
		#endregion
		public BarSplitButtonItemLink() { }
		protected internal BarSplitButtonItem SplitButtonItem { get { return ButtonItem as BarSplitButtonItem; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSplitButtonItemLinkActAsDropDown")]
#endif
		public bool ActAsDropDown {
			get { return (bool)GetValue(ActAsDropDownProperty); }
			internal set { this.SetValue(ActAsDropDownPropertyKey, value); }
		}
		protected internal override void UpdateProperties() {
			base.UpdateProperties();
			if(SplitButtonItem == null) return;
			ActAsDropDown = SplitButtonItem.ActAsDropDown;
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSplitButtonItemLinkPopupControl")]
#endif
		public IPopupControl PopupControl {
			get {
				if(SplitButtonItem != null) return
					SplitButtonItem.PopupControl;
				return null;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSplitButtonItemLinkIsPopupVisible")]
#endif
		public bool IsPopupVisible {
			get { return PopupControl != null && PopupControl.IsPopupOpen; }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSplitButtonItemLinkUserArrowAlignment")]
#endif
		public Dock? UserArrowAlignment {
			get { return (Dock?)GetValue(UserArrowAlignmentProperty); }
			set { SetValue(UserArrowAlignmentProperty, value); }
		}
		public PopupItemClickBehaviour ItemClickBehaviour {
			get { return (PopupItemClickBehaviour)GetValue(ItemClickBehaviourProperty); }
			set { SetValue(ItemClickBehaviourProperty, value); }
		}		
		protected virtual void OnActAsDropDownChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarSplitButtonItemLinkControl>(lc => lc.OnSourceActAsDropDownChanged());
		}
		protected virtual void OnArrowAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			UpdateActualArrowAlignment();
		}
		protected virtual void OnItemClickBehaviourChanged(PopupItemClickBehaviour oldValue) {
			ExecuteActionOnLinkControls<BarSplitButtonItemLinkControl>(lc => lc.UpdateActualItemClickBehaviour());
		}
		protected virtual Dock? OnArrowAlignmentCoerce(Dock? dock) {
			if(dock == null) return dock;
			if(dock == Dock.Top || dock == Dock.Left) return Dock.Right;
			return dock;
		}
		public override void Assign(BarItemLinkBase link) {
			base.Assign(link);
			BarSplitButtonItemLink sLink = link as BarSplitButtonItemLink;
			if(sLink == null) return;
			UserArrowAlignment = sLink.UserArrowAlignment;
			ActAsDropDown = sLink.ActAsDropDown;
		}
		protected internal virtual void UpdateActualArrowAlignment() {
			foreach(BarItemLinkInfo linkInfo in LinkInfos) {
				BarSplitButtonItemLinkControl lc = linkInfo.LinkControl as BarSplitButtonItemLinkControl;
				if(lc != null) {
					lc.UpdateActualArrowAlignment();
				}
			}
		}
	}
}
