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
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public class BarEditItemLink : BarItemLink {
		#region static
		public static readonly DependencyProperty EditWidthProperty;
		public static readonly DependencyProperty EditHeightProperty;
		public static readonly DependencyProperty EditStyleProperty;
		static readonly DependencyPropertyKey ActualEditWidthPropertyKey;
		static readonly DependencyPropertyKey ActualEditHeightPropertyKey;
		public static readonly DependencyProperty ActualEditWidthProperty;
		public static readonly DependencyProperty ActualEditHeightProperty;
		public static readonly DependencyProperty UserContent2Property;
		public static readonly DependencyProperty ActualContent2Property;
		public static readonly DependencyProperty ActualContent2TemplateProperty;
		static readonly DependencyPropertyKey ActualContent2PropertyKey;
		static readonly DependencyPropertyKey ActualContent2TemplatePropertyKey;
		public static readonly DependencyProperty ActualContent2VisibilityProperty;
		static readonly DependencyPropertyKey ActualContent2VisibilityPropertyKey;
		public static readonly DependencyProperty EditHorizontalAlignmentProperty;						
		public static readonly DependencyProperty ShowInVerticalBarProperty;										
		static BarEditItemLink() {
			ShowInVerticalBarProperty = DependencyPropertyManager.Register("ShowInVerticalBar", typeof(DevExpress.Utils.DefaultBoolean), typeof(BarEditItemLink), new FrameworkPropertyMetadata(DevExpress.Utils.DefaultBoolean.Default, (d, e) => ((BarEditItemLink)d).OnShowInVerticalBarChanged((DevExpress.Utils.DefaultBoolean)e.OldValue)));
			EditHorizontalAlignmentProperty = DependencyPropertyManager.Register("EditHorizontalAlignment", typeof(HorizontalAlignment?), typeof(BarEditItemLink), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnEditHorizontalAlignmentPropertyChanged)));
			EditWidthProperty = DependencyPropertyManager.Register("EditWidth", typeof(double?), typeof(BarEditItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnWidthPropertyChanged)));
			EditHeightProperty = DependencyPropertyManager.Register("EditHeight", typeof(double?), typeof(BarEditItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnHeightPropertyChanged)));
			EditStyleProperty = DependencyPropertyManager.Register("EditStyle", typeof(Style), typeof(BarEditItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnEditStylePropertyChanged)));
			ActualEditWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualEditWidth", typeof(double?), typeof(BarEditItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ActualEditHeightPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualEditHeight", typeof(double?), typeof(BarEditItemLink), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ActualEditWidthProperty = ActualEditWidthPropertyKey.DependencyProperty;
			ActualEditHeightProperty = ActualEditHeightPropertyKey.DependencyProperty;			
			UserContent2Property = DependencyPropertyManager.Register("UserContent2", typeof(object), typeof(BarEditItemLink), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnUserContent2PropertyChanged)));
			ActualContent2PropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContent2", typeof(object), typeof(BarEditItemLink), new FrameworkPropertyMetadata(null));
			ActualContent2TemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContent2Template", typeof(DataTemplate), typeof(BarEditItemLink), new FrameworkPropertyMetadata(null));
			ActualContent2VisibilityPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContent2Visibility", typeof(Visibility), typeof(BarEditItemLink), new FrameworkPropertyMetadata(Visibility.Collapsed));
			ActualContent2Property = ActualContent2PropertyKey.DependencyProperty;
			ActualContent2TemplateProperty = ActualContent2TemplatePropertyKey.DependencyProperty;
			ActualContent2VisibilityProperty = ActualContent2VisibilityPropertyKey.DependencyProperty;
		}
		protected override void UpdateEditContentMargin() {
			if(LinkControl != null)
				((BarEditItemLinkControl)LinkControl).UpdateEditContentMargin();
		}
		protected static void OnWidthPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItemLink)obj).OnWidthChanged(e);
		}
		protected static void OnHeightPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItemLink)obj).OnHeightChanged(e);
		}
		protected static void OnUserContent2PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItemLink)obj).OnUserContent2Changed(e.OldValue);
		}
		protected static void OnEditStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItemLink)obj).OnEditStyleChanged(e.OldValue as Style);
		}
		protected static void OnEditHorizontalAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarEditItemLink)d).OnEditHorizontalAlignmentChanged((HorizontalAlignment?)e.OldValue);
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkEditItem")]
#endif
		public BarEditItem EditItem { get { return Item as BarEditItem; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkEditWidth")]
#endif
		public double? EditWidth {
			get { return (double?)GetValue(EditWidthProperty); }
			set { SetValue(EditWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkEditHorizontalAlignment")]
#endif
		public HorizontalAlignment? EditHorizontalAlignment {
			get { return (HorizontalAlignment?)GetValue(EditHorizontalAlignmentProperty); }
			set { SetValue(EditHorizontalAlignmentProperty, value); }
		}
		public DevExpress.Utils.DefaultBoolean ShowInVerticalBar {
			get { return (DevExpress.Utils.DefaultBoolean)GetValue(ShowInVerticalBarProperty); }
			set { SetValue(ShowInVerticalBarProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkEditHeight")]
#endif
		public double? EditHeight {
			get { return (double?)GetValue(EditHeightProperty); }
			set { SetValue(EditHeightProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkEditStyle")]
#endif
		public Style EditStyle {
			get { return (Style)GetValue(EditStyleProperty); }
			set { SetValue(EditStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkActualEditWidth")]
#endif
		public double? ActualEditWidth {
			get { return (double?)GetValue(ActualEditWidthProperty); }
			private set { this.SetValue(ActualEditWidthPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkActualEditHeight")]
#endif
		public double? ActualEditHeight {
			get { return (double?)GetValue(ActualEditHeightProperty); }
			private set { this.SetValue(ActualEditHeightPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkActualEditStyle")]
#endif
		public Style ActualEditStyle {
			get {
				if(EditStyle != null) return EditStyle;
				if(EditItem != null) return EditItem.EditStyle;
				return null;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkUserContent2")]
#endif
		public object UserContent2 {
			get { return (object)GetValue(UserContent2Property); }
			set { SetValue(UserContent2Property, value); }
		}
		public object ActualContent2 {
			get { return (object)GetValue(ActualContent2Property); }
			private set { this.SetValue(ActualContent2PropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkActualContent2Template")]
#endif
		public DataTemplate ActualContent2Template {
			get { return (DataTemplate)GetValue(ActualContent2TemplateProperty); }
			private set { this.SetValue(ActualContent2TemplatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkActualContent2Visibility")]
#endif
		public Visibility ActualContent2Visibility {
			get { return (Visibility)GetValue(ActualContent2VisibilityProperty); }
			private set { this.SetValue(ActualContent2VisibilityPropertyKey, value); }
		}		
		internal void UpdateActualContent2() {
			if(UserContent2 != null)
				ActualContent2 = UserContent2;
			else ActualContent2 = EditItem != null ? EditItem.Content2 : null;
			ActualContent2Template = EditItem != null ? EditItem.Content2Template : null;
			ActualContent2Visibility = ActualContent2 == null && ActualContent2Template == null ? Visibility.Collapsed : Visibility.Visible;
		}		
		protected internal override void Initialize() {
			base.Initialize();
			UpdateActualContent2();
		}
		protected internal void UpdateActualEditSize() {
			double? editWidth = EditItem != null ? EditItem.EditWidth : EditWidth;
			double? editHeight = EditItem != null ? EditItem.EditHeight : EditHeight;
			ActualEditWidth = EditWidth.HasValue? EditWidth : editWidth;
			ActualEditHeight = EditHeight.HasValue ? EditHeight : editHeight;
		}
		protected override void OnItemChanged(BarItem oldValue) {
			base.OnItemChanged(oldValue);
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnItemChanged((BarEditItem)oldValue, EditItem));
		}
		protected virtual void OnWidthChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnSourceEditWidthChanged());
			UpdateActualEditSize();
		}
		protected virtual void OnHeightChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnSourceEditHeightChanged());
			UpdateActualEditSize();
		}
		protected virtual void OnUserContent2Changed(object oldValue) {
			UpdateActualContent2();
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnSourceContent2Changed());
		}
		protected virtual void OnEditStyleChanged(Style oldValue) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnSourceEditStyleChanged());
		}
		protected virtual void OnEditHorizontalAlignmentChanged(HorizontalAlignment? oldValue) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.OnSourceHorizontalEditAlignmentChanged());
		}
		protected virtual void OnShowInVerticalBarChanged(DevExpress.Utils.DefaultBoolean oldValue) {
			ExecuteActionOnLinkControls<BarEditItemLinkControl>(lc => lc.UpdateShowInVerticalBar());
		}
		protected internal override void UpdateProperties() {
			base.UpdateProperties();
			UpdateActualEditSize();
		}
		public override void Assign(BarItemLinkBase link) {
			base.Assign(link);
			BarEditItemLink eLink = link as BarEditItemLink;
			if(eLink == null) return;
			EditWidth = eLink.EditWidth;
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarEditItemLinkEditor")]
#endif
		public BaseEdit Editor { 
			get { 
				if(LinkControl == null)
					return null;
				return ((BarEditItemLinkControl)LinkControl).Edit;
			} 
		}		
	}
}
