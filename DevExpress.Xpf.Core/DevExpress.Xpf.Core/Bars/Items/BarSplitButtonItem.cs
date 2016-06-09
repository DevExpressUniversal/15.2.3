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
using DevExpress.Xpf.Bars.Converters;
using System.Windows.Markup;
namespace DevExpress.Xpf.Bars {
	[DevExpress.Xpf.Core.DXToolboxBrowsable(false)]
	[ContentProperty("PopupControl")]
	public class BarSplitButtonItem : BarButtonItem {
		#region static
		public static readonly DependencyProperty ActAsDropDownProperty;
		public static readonly DependencyProperty ArrowAlignmentProperty;
		public static readonly DependencyProperty ItemClickBehaviourProperty;
		public static readonly DependencyProperty FirstSectorIndexProperty;
		static BarSplitButtonItem() {
			ActAsDropDownProperty = DependencyPropertyManager.Register("ActAsDropDown", typeof(bool), typeof(BarSplitButtonItem), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnActAsDropDownPropertyChanged)));
			ArrowAlignmentProperty = DependencyPropertyManager.Register("ArrowAlignment", typeof(Dock), typeof(BarSplitButtonItem), new FrameworkPropertyMetadata(Dock.Right, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnArrowAlignmentPropertyChanged), new CoerceValueCallback(OnArrowAlignmentPropertyCoerce)));
			ItemClickBehaviourProperty = DependencyPropertyManager.Register("ItemClickBehaviour", typeof(PopupItemClickBehaviour), typeof(BarSplitButtonItem), new FrameworkPropertyMetadata(PopupItemClickBehaviour.Undefined, (d, e) => ((BarSplitButtonItem)d).OnItemClickBehaviourChanged((PopupItemClickBehaviour)e.OldValue)));
			FirstSectorIndexProperty = DependencyProperty.Register("FirstSectorIndex", typeof(int), typeof(BarSplitButtonItem), new PropertyMetadata(0));
		}
		protected static void OnActAsDropDownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarSplitButtonItem)d).OnActAstDropDownChanged(e);
		}
		protected static void OnArrowAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarSplitButtonItem)d).OnArrowAlignmentChanged(e);
		}
		protected static object OnArrowAlignmentPropertyCoerce(DependencyObject d, object baseValue) {
			return ((BarSplitButtonItem)d).OnArrowAlignmentCoerce((Dock)baseValue);
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSplitButtonItemActAsDropDown")]
#endif
		public bool ActAsDropDown {
			get { return (bool)GetValue(ActAsDropDownProperty); }
			set { SetValue(ActAsDropDownProperty, value); }
		}
		public PopupItemClickBehaviour ItemClickBehaviour {
			get { return (PopupItemClickBehaviour)GetValue(ItemClickBehaviourProperty); }
			set { SetValue(ItemClickBehaviourProperty, value); }
		}		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSplitButtonItemArrowAlignment")]
#endif
		public Dock ArrowAlignment {
			get { return (Dock)GetValue(ArrowAlignmentProperty); }
			set { SetValue(ArrowAlignmentProperty, value); }
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get {
				return new SingleLogicalChildEnumerator(PopupControl.With(x=>x.Popup));
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarSplitButtonItemPopupControl")]
#endif
		public IPopupControl PopupControl {
			get { return popupControl; }
			set {
				if(value == popupControl)
					return;
				OnPopupControlChanging();
				popupControl = value;
				OnPopupControlChanged();
				RaisePropertyChange("PopupControl");
			}
		}
		public int FirstSectorIndex {
			get { return (int)GetValue(FirstSectorIndexProperty); }
			set { SetValue(FirstSectorIndexProperty, value); }
		}
		IPopupControl popupControl = null;
		protected virtual void OnPopupControlChanging() {
			if(PopupControl == null) return;
			ClearDataContext(PopupControl.Popup);
			RemovePopupFromLogicalTree(PopupControl.Popup);
		}
		protected virtual void OnItemClickBehaviourChanged(PopupItemClickBehaviour oldValue) {
			ExecuteActionOnLinkControls<BarSplitButtonItemLinkControl>(lc => lc.UpdateActualItemClickBehaviour());
		}
		protected virtual void OnPopupControlChanged() {
			if(PopupControl != null) {
				SetDataContext(PopupControl.Popup);
				AddPopupToLogicalTree(PopupControl.Popup);
			}
			ExecuteActionOnLinkControls<BarSplitButtonItemLinkControl>(lc => lc.UpdateActualIsArrowEnabled());
		}		
		protected override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			base.OnDataContextChanged(sender, e);
			if(PopupControl != null) {
				if(PopupControl is PopupMenu) {
					SetDataContext(PopupControl as PopupMenu);
				}
				SetDataContext(PopupControl.Popup);
			}
		}
		protected virtual void AddPopupToLogicalTree(BarPopupBase popup) {
			if (popup.Parent == null)
				AddLogicalChild(popup);
		}
		protected virtual void RemovePopupFromLogicalTree(BarPopupBase popup) {
			RemoveLogicalChild(popup);
		}
		protected virtual void OnArrowAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinks<BarSplitButtonItemLink>(l => l.UpdateActualArrowAlignment());			
		}
		protected virtual Dock OnArrowAlignmentCoerce(Dock dock) {
			if(dock == Dock.Top || dock == Dock.Left) return Dock.Right;
			return dock;
		}
		protected virtual void OnActAstDropDownChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinks<BarSplitButtonItemLink>(l => l.ActAsDropDown = ActAsDropDown);
			ExecuteActionOnLinkControls<BarSplitButtonItemLinkControl>(lc => lc.OnSourceActAsDropDownChanged());
		}
	}
}
