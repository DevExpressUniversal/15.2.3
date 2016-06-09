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
	public class BarStaticItemLink : BarItemLink {
		#region static
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ContentAlignmentProperty;
		public static readonly DependencyProperty UserWidthProperty;
		public static readonly DependencyProperty UserMinWidthProperty;
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualLinkWidthProperty;
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualMinWidthProperty;
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HasWidthProperty;
		static BarStaticItemLink() {
			ActualLinkWidthProperty = DependencyPropertyManager.Register("ActualLinkWidth", typeof(double), typeof(BarStaticItemLink), new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnActualWidthPropertyChanged)));
			UserWidthProperty = DependencyPropertyManager.Register("UserWidth", typeof(double), typeof(BarStaticItemLink), new FrameworkPropertyMetadata(double.NaN, new PropertyChangedCallback(OnWidthPropertyChanged), new CoerceValueCallback(OnCoerceWidthValue)));
			UserMinWidthProperty = DependencyPropertyManager.Register("UserMinWidth", typeof(double), typeof(BarStaticItemLink), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnUserMinWidthChanged)));
			ActualMinWidthProperty = DependencyPropertyManager.Register("ActualMinWidth", typeof(double), typeof(BarStaticItemLink), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnActualMinWidthPropertyChanged)));
			ContentAlignmentProperty = DependencyPropertyManager.Register("ContentAlignment", typeof(HorizontalAlignment), typeof(BarStaticItemLink), new FrameworkPropertyMetadata(HorizontalAlignment.Left, new PropertyChangedCallback(OnContentAlignmentPropertyChanged)));
			HasWidthProperty = DependencyPropertyManager.Register("HasWidth", typeof(bool), typeof(BarStaticItemLink), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHasWidthPropertyChanged)));
		}
		protected static void OnHasWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItemLink)d).OnHasWidthChanged(e);
		}
		protected static void OnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItemLink)d).WidthChanged(e);
		}
		protected static void OnActualMinWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItemLink)d).OnActualMinWidthChanged(e);
		}
		protected static void OnActualWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItemLink)d).OnActualWidthChanged(e);
		}
		protected static void OnContentAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItemLink)d).OnContentAlignmentChanged((HorizontalAlignment)e.OldValue);
		}		
		protected static void OnUserMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarStaticItemLink)d).OnUserMinWidthChanged(e);
		}
		static object OnCoerceWidthValue(DependencyObject d, object baseValue) {
			double value = (double)baseValue;
			if(value < 0) value = 0;
			return value;
		}
		#endregion
		public BarStaticItemLink() {
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemLinkHasWidth")]
#endif
		public bool HasWidth {
			get { return (bool)GetValue(HasWidthProperty); }
			private set { SetValue(HasWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemLinkContentAlignment")]
#endif
		public HorizontalAlignment ContentAlignment {
			get { return (HorizontalAlignment)GetValue(ContentAlignmentProperty); }
			private set { SetValue(ContentAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemLinkUserWidth")]
#endif
		public double UserWidth {
			get { return (double)GetValue(UserWidthProperty); }
			set { SetValue(UserWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemLinkUserMinWidth")]
#endif
		public double UserMinWidth {
			get { return (double)GetValue(UserMinWidthProperty); }
			set { SetValue(UserMinWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemLinkActualLinkWidth")]
#endif
		public double ActualLinkWidth {
			get { return (double)GetValue(ActualLinkWidthProperty); }
			private set { SetValue(ActualLinkWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarStaticItemLinkActualMinWidth")]
#endif
		public double ActualMinWidth {
			get { return (double)GetValue(ActualMinWidthProperty); }
			private set { SetValue(ActualMinWidthProperty, value); }
		}
		protected internal BarStaticItem StaticItem { get { return base.Item as BarStaticItem; } }
		protected internal virtual void UpdateContentAlignment() {
			ContentAlignment = StaticItem.ContentAlignment;
		}
		protected internal virtual void UpdateActualMinWidth() {
			if(UserMinWidth != (double)UserMinWidthProperty.GetMetadata(typeof(BarStaticItemLink)).DefaultValue)
				ActualMinWidth = UserMinWidth;
			else if(StaticItem != null)
				ActualMinWidth = StaticItem.ItemMinWidth;
		}
		protected internal override void UpdateProperties() {
			base.UpdateProperties();
			if(StaticItem == null) return;
			UpdateContentAlignment();
			if(StaticItem.AutoSizeMode == BarItemAutoSizeMode.None) {
				if(!Double.IsNaN(UserWidth))
					ActualLinkWidth = UserWidth;
				else if(StaticItem != null)
					ActualLinkWidth = StaticItem.ItemWidth;
			} else {
				ActualLinkWidth = double.NaN;
			}
			HasWidth = !Double.IsNaN(ActualLinkWidth);
			UpdateActualMinWidth();
			if(StaticItem.ForceMeasurePanel)
				TryMeasurePanel();
		}
		private void OnUserMinWidthChanged(DependencyPropertyChangedEventArgs e) {
			UpdateActualMinWidth();
			ExecuteActionOnLinkControls<BarStaticItemLinkControl>(lc => lc.OnSourceMinWidthChanged());
		}
		protected virtual void WidthChanged(DependencyPropertyChangedEventArgs e) {
			UpdateProperties();
		}
		protected virtual void OnActualWidthChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarStaticItemLinkControl>(lc => lc.OnSourceLinkWidthChanged());
		}
		protected virtual void OnContentAlignmentChanged(HorizontalAlignment oldValue) {
			ExecuteActionOnLinkControls<BarStaticItemLinkControl>(lc => lc.OnSourceContentAlignmentChanged());
		}		
		protected virtual void OnActualMinWidthChanged(DependencyPropertyChangedEventArgs e) {
		}
		public override void Assign(BarItemLinkBase link) {
			base.Assign(link);
			BarStaticItemLink sLink = link as BarStaticItemLink;
			if(sLink == null) return;
			ContentAlignment = sLink.ContentAlignment;
			UserMinWidth = sLink.UserMinWidth;
			UserWidth = sLink.UserWidth;
		}
		protected virtual void OnHasWidthChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarStaticItemLinkControl>(lc => lc.UpdateVisualStateByWidth());
		}
	}
}
