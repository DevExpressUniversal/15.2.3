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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Editors.Automation;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Collections;
using DevExpress.Xpf.Grid;
using DevExpress.Utils;
using DevExpress.Xpf.Grid.LookUp.Native;
using DevExpress.Xpf.Grid.Themes;
using DevExpress.Data.Filtering;
using DevExpress.Data;
#if !SL
using DevExpress.Utils.Design.DataAccess;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Printing;
using System.Windows.Interop;
using DevExpress.Xpf.Grid.LookUp;
#else
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Grid.LookUp {
	[DXToolboxBrowsable(true), ToolboxTabName(AssemblyInfo.DXTabWpfCommon)]
#if !SL
	[DataAccessMetadata("All", SupportedProcessingModes = "GridLookup")]
#endif
	public class LookUpEdit : LookUpEditBase {
		public const double DefaultPopupMinHeight = 300d, DefaultPopupMinWidth=200d;
		public static readonly DependencyProperty IsPopupAutoWidthProperty;
		public static readonly DependencyProperty AutoPopulateColumnsProperty;
		static LookUpEdit() {
			Type ownerType = typeof(LookUpEdit);
			IsPopupAutoWidthProperty = DependencyPropertyManager.Register("IsPopupAutoWidth", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			AutoPopulateColumnsProperty = DependencyPropertyManager.Register("AutoPopulateColumns", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, OnAutoPopulateColumnsChanged));
#if !SL
			EditorSettingsProvider.Default.RegisterUserEditor2(typeof(LookUpEdit), typeof(LookUpEditSettings),
				optimized => optimized ? new InplaceBaseEdit() : (IBaseEdit)new LookUpEdit(), () => new LookUpEditSettings());
			PopupMinHeightProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(DefaultPopupMinHeight));
			PopupMinWidthProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(DefaultPopupMinWidth));
#else
			EditorSettingsProvider.Default.RegisterUserEditor(ownerType, typeof(LookUpEditSettings), () => new LookUpEdit(), () => new LookUpEditSettings());
#endif
		}
		static void OnAutoPopulateColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((LookUpEdit)d).OnAutoPopulateColumnsChanged((bool)e.NewValue);
		}
		protected override Type StyleSettingsType { get { return typeof(LookUpEditStyleSettings); } }
		protected internal new LookUpEditStrategy EditStrategy { get { return base.EditStrategy as LookUpEditStrategy; } set { EditStrategy = value; } }
		public LookUpEdit() {
			this.SetDefaultStyleKey(typeof(LookUpEdit));
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("LookUpEditIsPopupAutoWidth")]
#endif
		public bool IsPopupAutoWidth {
			get { return (bool)GetValue(IsPopupAutoWidthProperty); }
			set { SetValue(IsPopupAutoWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("LookUpEditAutoPopulateColumns")]
#endif
		public bool AutoPopulateColumns {
			get { return (bool)GetValue(AutoPopulateColumnsProperty); }
			set { SetValue(AutoPopulateColumnsProperty, value); }
		}
		public GridControl GetGridControl() {
			return LookUpEditHelper.GetVisualClient(this).InnerEditor as GridControl;
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("LookUpEditFilteredItems")]
#endif
		public IEnumerable FilteredItems { get { return ItemsProvider.VisibleListSource; } }
		protected override bool ShouldApplyPopupSize {
			get { return IsPopupAutoWidth; }
		}
		[Category(EditSettingsCategories.Behavior)]
		[Browsable(true)]
		new public BaseEditStyleSettings StyleSettings { get { return base.StyleSettings; } set { base.StyleSettings = value; } }
		protected override void BeforePopupOpened() {
			base.BeforePopupOpened();
			EditStrategy.SetInitialPopupSize();
		}
		internal void SetInitialPopupSizeInternal() {
			if(double.IsNaN(PopupWidth) && IsPopupAutoWidth)
				PopupWidth = ActualWidth;
#if SL
			if (double.IsNaN(PopupMinHeight)) {
				PopupMinHeight = DefaultPopupMinHeight;
			}
			if (double.IsNaN(PopupMinWidth)) {
				PopupMinWidth = DefaultPopupMinWidth;
			}
#endif
			if (double.IsNaN(PopupHeight)) {
#if !SL
				PopupHeight = PopupMinHeight;
#else
				PopupHeight = LookUpEditBase.GetDefaultPopupMaxHeight();
#endif
			}
		}
		protected override bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return base.IsClosePopupWithAcceptGesture(key, modifiers) && !EditStrategy.AllowPopupProcessGestures(key, modifiers);
		}
		protected override bool IsClosePopupWithCancelGesture(Key key, ModifierKeys modifiers) {
			return base.IsClosePopupWithCancelGesture(key, modifiers) && !EditStrategy.AllowPopupProcessGestures(key, modifiers);
		}
		protected override bool IsTogglePopupOpenGesture(Key key, ModifierKeys modifiers) {
			return base.IsTogglePopupOpenGesture(key, modifiers) && !EditStrategy.AllowPopupProcessGestures(key, modifiers);
		}
		protected override EditStrategyBase CreateEditStrategy() {
			return new LookUpEditStrategy(this);
		}
		protected override bool CanShowPopupCore() { return true; }	  
		protected virtual void OnAutoPopulateColumnsChanged(bool newValue) {
			if(newValue && GridControl != null) 
				GridControl.PopulateColumns();
		}
		protected override void OnPopupIsKeyboardFocusWithinChanged(Editors.Popups.EditorPopupBase popupBase) {
			if(!CanClose())
				return;
			base.OnPopupIsKeyboardFocusWithinChanged(popupBase);
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			if(!CanClose())
				return;
			base.OnIsKeyboardFocusWithinChanged(e);
		}
		bool CanClose() {
			if((GridControl != null) && (GridControl.View != null)) {
				if(GridControl.View.IsFilterControlOpened)
					return false;
			}
			return true;
		}
		internal GridControl GridControl { get { return GetGridControl(); } }
		protected override VisualClientOwner CreateVisualClient() {
			return new GridControlVisualClientOwner(this);
		}
	}
}
