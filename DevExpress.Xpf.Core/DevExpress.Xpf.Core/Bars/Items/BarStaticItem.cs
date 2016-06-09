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
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public enum BarItemAutoSizeMode { None, Content, Fill }
	public class BarStaticItem : BarItem {
		#region static
		public static readonly DependencyProperty ItemWidthProperty;
		public static readonly DependencyProperty ItemMinWidthProperty;
		public static readonly DependencyProperty ContentAlignmentProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		public static readonly DependencyProperty AutoSizeModeProperty;
		static BarStaticItem() {
			ItemWidthProperty = DependencyPropertyManager.Register("ItemWidth", typeof(double), typeof(BarStaticItem), new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnWidthPropertyChanged), new CoerceValueCallback(OnCoerceWidthValue)));
			ItemMinWidthProperty = DependencyPropertyManager.Register("ItemMinWidth", typeof(double), typeof(BarStaticItem), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnMinWidthChanged)));
			ContentAlignmentProperty = DependencyPropertyManager.Register("ContentAlignment", typeof(HorizontalAlignment), typeof(BarStaticItem), new FrameworkPropertyMetadata(HorizontalAlignment.Left, new PropertyChangedCallback(OnContentAlignmentPropertyChanged)));
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(BarStaticItem), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowBorderPropertyChanged)));
			AutoSizeModeProperty = DependencyPropertyManager.Register("AutoSizeMode", typeof(BarItemAutoSizeMode), typeof(BarStaticItem), new FrameworkPropertyMetadata(BarItemAutoSizeMode.None, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnAutoSizeModePropertyChanged)));
		}
		protected static void OnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItem)d).WidthChanged(e);
		}
		protected static void OnAutoSizeModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItem)d).OnAutoSizeModeChanged(e);
		}
		protected static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItem)d).OnMinWidthChanged(e);
		}
		protected static void OnContentAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItem)d).OnContentAlignmentChanged((HorizontalAlignment)e.OldValue);
		}		
		protected static void OnShowBorderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItem)d).OnShowBorderChanged((bool)e.OldValue);
		}		
		static object OnCoerceWidthValue(DependencyObject d, object baseValue) {
			double value = (double)baseValue; 
			if(value < 0) value = 0;
			return value;
		}
		#endregion 
		public BarStaticItem() { }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemItemWidth")]
#endif
		public double ItemWidth {
			get { return (double)GetValue(ItemWidthProperty); }
			set { SetValue(ItemWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemItemMinWidth")]
#endif
		public double ItemMinWidth {
			get { return (double)GetValue(ItemMinWidthProperty); }
			set { SetValue(ItemMinWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemContentAlignment")]
#endif
		public HorizontalAlignment ContentAlignment {
			get { return (HorizontalAlignment)GetValue(ContentAlignmentProperty); }
			set { SetValue(ContentAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemShowBorder")]
#endif
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemAutoSizeMode")]
#endif
		public BarItemAutoSizeMode AutoSizeMode {
			get { return (BarItemAutoSizeMode)GetValue(AutoSizeModeProperty); }
			set { SetValue(AutoSizeModeProperty, value); }
		}
		protected virtual void OnMinWidthChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinks<BarStaticItemLink>(l => l.UpdateActualMinWidth());
			ExecuteActionOnLinkControls<BarStaticItemLinkControl>(lc => lc.OnSourceMinWidthChanged());
		}
		protected virtual void OnContentAlignmentChanged(HorizontalAlignment oldValue) {
			ExecuteActionOnLinks<BarStaticItemLink>(l => l.UpdateContentAlignment());
			ExecuteActionOnLinkControls<BarStaticItemLinkControl>(lc => lc.OnSourceContentAlignmentChanged());
		}
		protected virtual void OnShowBorderChanged(bool oldValue) {
			ExecuteActionOnLinkControls<BarStaticItemLinkControl>(lc => lc.OnSourceShowBorderChanged());
		}
		internal bool ForceMeasurePanel { get; set; }
		void UpdatePropertiesAndMeasurePanel() {
			ForceMeasurePanel = true;
			try {
				UpdateProperties();
			}
			finally { ForceMeasurePanel = false; }
		}
		protected virtual void OnAutoSizeModeChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarStaticItemLinkControl>(lc => lc.UpdateLayoutPanelWidth());
			UpdatePropertiesAndMeasurePanel();
		}
		protected virtual void WidthChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarStaticItemLinkControl>(lc => lc.UpdateLayoutPanelWidth());
			UpdatePropertiesAndMeasurePanel();	
		}
		protected internal override bool CanKeyboardSelect { get { return false; } }
	}
}
