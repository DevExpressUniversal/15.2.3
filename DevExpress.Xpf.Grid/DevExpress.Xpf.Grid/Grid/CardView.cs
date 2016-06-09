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
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid.Native;
using System.Windows.Media;
using DevExpress.Xpf.Grid.Hierarchy;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.Xpf.Printing.BrickCollection;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Xpf.Grid.Printing;
using DevExpress.Mvvm.Native;
using System.IO;
namespace DevExpress.Xpf.Grid {
	public enum CollapsedCardOrientation { Vertical, Horizontal};
	public enum CardViewSelectMode { None, Row };
	public enum CardLayout { Rows, Columns }
	public enum ScrollMode { Item, Pixel }
	public class CardLayoutToOrientationConverter : IValueConverter {
		public static readonly CardLayoutToOrientationConverter Instance = new CardLayoutToOrientationConverter();
		CardLayoutToOrientationConverter() {
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return CardView.CardLayoutToOrientation((CardLayout)value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class CardView : GridViewBase {
		public static readonly DependencyProperty ScrollModeProperty;
		public static readonly DependencyProperty FixedSizeProperty;
		public static readonly DependencyProperty MaxCardCountInRowProperty;
		public static readonly DependencyProperty CardAlignmentProperty;
		public static readonly DependencyProperty CardMarginProperty;
		public static readonly DependencyProperty ShowCardExpandButtonProperty;
		public static readonly DependencyProperty CardStyleProperty;
		public static readonly DependencyProperty CardTemplateProperty;
		public static readonly DependencyProperty CardTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualCardTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualCardTemplateSelectorProperty;
		public static readonly DependencyProperty CardHeaderTemplateProperty;
		public static readonly DependencyProperty CardHeaderTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualCardHeaderTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualCardHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty CardRowTemplateProperty;
		public static readonly DependencyProperty CardRowTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualCardRowTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualCardRowTemplateSelectorProperty;
		public static readonly DependencyProperty MinFixedSizeProperty;
		public static readonly DependencyProperty AllowCardResizingProperty;
		static readonly DependencyPropertyKey IsResizingEnabledPropertyKey;
		public static readonly DependencyProperty IsResizingEnabledProperty;
		public static readonly DependencyProperty SeparatorTemplateProperty;
		public static readonly DependencyProperty SeparatorThicknessProperty;
		public static readonly DependencyProperty CardLayoutProperty;
		static readonly DependencyPropertyKey OrientationPropertyKey;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty FocusedCardBorderTemplateProperty;
		public static readonly DependencyProperty FocusedCellBorderCardViewTemplateProperty;
		public static readonly DependencyProperty VerticalFocusedGroupRowBorderTemplateProperty;
		public static readonly DependencyProperty MultiSelectModeProperty;
		static readonly DependencyPropertyKey CollapsedCardOrientationPropertyKey;
		public static readonly DependencyProperty CollapsedCardOrientationProperty;
		public static readonly DependencyProperty UseLightweightTemplatesProperty;
		public static readonly DependencyProperty LeftGroupAreaIndentProperty;
		#region Printing
		public static readonly DependencyProperty PrintCardRowTemplateProperty;
		public static readonly DependencyProperty PrintCardTemplateProperty;
		public static readonly DependencyProperty PrintCardContentTemplateProperty;
		public static readonly DependencyProperty PrintCardHeaderTemplateProperty;
		public static readonly DependencyProperty PrintCardRowIndentTemplateProperty;
		public static readonly DependencyProperty PrintCardMarginProperty;
		public static readonly DependencyProperty PrintAutoCardWidthProperty;
		public static readonly DependencyProperty PrintMaximumCardColumnsProperty;
		public static readonly DependencyProperty PrintTotalSummarySeparatorStyleProperty;
		#endregion
		static CardView() {
			Type ownerType = typeof(CardView);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			ScrollModeProperty = DependencyProperty.Register("ScrollMode", typeof(ScrollMode), ownerType, new PropertyMetadata(ScrollMode.Pixel, (d, e) => ((CardView)d).OnAllowPerPixelScrollingChanged()));
			FocusedCardBorderTemplateProperty = DependencyProperty.Register("FocusedCardBorderTemplate", typeof(ControlTemplate), ownerType);
			FocusedCellBorderCardViewTemplateProperty = DependencyProperty.Register("FocusedCellBorderCardViewTemplate", typeof(ControlTemplate), ownerType);
			VerticalFocusedGroupRowBorderTemplateProperty = DependencyProperty.Register("VerticalFocusedGroupRowBorderTemplate", typeof(ControlTemplate), ownerType);
			FixedSizeProperty = CardsPanel.FixedSizeProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(CardsPanel.DefaultFixedSize, FrameworkPropertyMetadataOptions.None, (d, e) => ((CardView)d).UpdateIsResizingEnabled(), (d, baseValue) => Math.Max((double)baseValue, ((CardView)d).MinFixedSize)));
			MaxCardCountInRowProperty = CardsPanel.MaxCardCountInRowProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(CardsPanel.DefaultMaxCardCountInRow, FrameworkPropertyMetadataOptions.None, (d, e) => d.CoerceValue(CollapsedCardOrientationProperty), (d, baseValue) => ((int)baseValue <= 0) ? ((CardView)d).MaxCardCountInRow : (int)baseValue));
			CardAlignmentProperty = CardsPanel.CardAlignmentProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(CardsPanel.DefaultCardAlignment, FrameworkPropertyMetadataOptions.None, null, null));
			CardLayoutProperty = DependencyProperty.Register("CardLayout", typeof(CardLayout), ownerType, new FrameworkPropertyMetadata(OrientationToCardLayout(CardsPanel.DefaultOrientation), (d, e) => ((CardView)d).OnCardLayoutChanged()));
			OrientationPropertyKey = DependencyProperty.RegisterReadOnly("Orientation", typeof(Orientation), ownerType, new FrameworkPropertyMetadata(CardsPanel.DefaultOrientation));
			OrientationProperty = OrientationPropertyKey.DependencyProperty;
			CardMarginProperty = CardsPanel.CardMarginProperty.AddOwner(ownerType, new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.None, null, null));
			ShowCardExpandButtonProperty = DependencyProperty.Register("ShowCardExpandButton", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			MinFixedSizeProperty = DependencyProperty.Register("MinFixedSize", typeof(double), ownerType, new FrameworkPropertyMetadata(10d, null, CoerceMinFixedSize));
			AllowCardResizingProperty = DependencyProperty.Register("AllowCardResizing", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((CardView)d).UpdateIsResizingEnabled()));
			IsResizingEnabledPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsResizingEnabled", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
			IsResizingEnabledProperty = IsResizingEnabledPropertyKey.DependencyProperty;
			CardTemplateProperty = DependencyProperty.Register("CardTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((CardView)d).UpdateActualCardTemplateSelector()));
			CardTemplateSelectorProperty = DependencyProperty.Register("CardTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((CardView)d).UpdateActualCardTemplateSelector()));
			ActualCardTemplateSelectorPropertyKey = DependencyProperty.RegisterReadOnly("ActualCardTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualCardTemplateSelectorProperty = ActualCardTemplateSelectorPropertyKey.DependencyProperty;
			CardHeaderTemplateProperty = DependencyProperty.Register("CardHeaderTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((CardView)d).UpdateActualCardHeaderTemplateSelector()));
			CardHeaderTemplateSelectorProperty = DependencyProperty.Register("CardHeaderTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((CardView)d).UpdateActualCardHeaderTemplateSelector()));
			ActualCardHeaderTemplateSelectorPropertyKey = DependencyProperty.RegisterReadOnly("ActualCardHeaderTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualCardHeaderTemplateSelectorProperty = ActualCardHeaderTemplateSelectorPropertyKey.DependencyProperty;
			CardRowTemplateProperty = DependencyProperty.Register("CardRowTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((CardView)d).UpdateActualCardRowTemplateSelector()));
			CardRowTemplateSelectorProperty = DependencyProperty.Register("CardRowTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((CardView)d).UpdateActualCardRowTemplateSelector()));
			ActualCardRowTemplateSelectorPropertyKey = DependencyProperty.RegisterReadOnly("ActualCardRowTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualCardRowTemplateSelectorProperty = ActualCardRowTemplateSelectorPropertyKey.DependencyProperty;
			SeparatorTemplateProperty = DependencyProperty.Register("SeparatorTemplate", typeof(DataTemplate), ownerType);
			SeparatorThicknessProperty = DependencyProperty.Register("SeparatorThickness", typeof(double), ownerType, new PropertyMetadata(7.0));
			CardStyleProperty = DependencyProperty.Register("CardStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			MultiSelectModeProperty = DependencyProperty.Register("MultiSelectMode", typeof(CardViewSelectMode), ownerType, new FrameworkPropertyMetadata(CardViewSelectMode.None, (d, e) => ((CardView)d).OnMultiSelectModeChanged()));
			CollapsedCardOrientationPropertyKey = DependencyProperty.RegisterReadOnly("CollapsedCardOrientation", typeof(CollapsedCardOrientation), ownerType, new PropertyMetadata(CollapsedCardOrientation.Horizontal,null,CoerceCollapsedCardOrientation));
			CollapsedCardOrientationProperty = CollapsedCardOrientationPropertyKey.DependencyProperty;
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(GridCommands.ChangeCardExpanded, (d, e) => ((CardView)d).OnChangeCardExpanded(e)));
			UseLightweightTemplatesProperty = CardViewBehavior.RegisterUseLightweightTemplatesProperty(ownerType);
			LeftGroupAreaIndentProperty = DependencyProperty.Register("LeftGroupAreaIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, (d, e) => ((CardView)d).RebuildVisibleColumns()));
			#region Printing
			PrintCardRowTemplateProperty = DependencyProperty.Register("PrintCardRowTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			PrintCardTemplateProperty = DependencyProperty.Register("PrintCardTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			PrintCardContentTemplateProperty = DependencyProperty.Register("PrintCardContentTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			PrintCardHeaderTemplateProperty = DependencyProperty.Register("PrintCardHeaderTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			PrintCardRowIndentTemplateProperty = DependencyProperty.Register("PrintCardRowIndentTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			PrintCardMarginProperty = DependencyProperty.Register("PrintCardMargin", typeof(Thickness), ownerType, new UIPropertyMetadata(new Thickness(20, 0, 0, 20)));
			PrintAutoCardWidthProperty = DependencyProperty.Register("PrintAutoCardWidth", typeof(bool), ownerType, new UIPropertyMetadata(false));
			PrintMaximumCardColumnsProperty = DependencyProperty.Register("PrintMaximumCardColumns", typeof(int), ownerType, new UIPropertyMetadata(-1));
			PrintTotalSummarySeparatorStyleProperty = DependencyProperty.Register("PrintTotalSummarySeparatorStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, OnUpdateColumnsAppearance));
			#endregion
		}
		static object CoerceCollapsedCardOrientation(DependencyObject d, object value) {
			CardView view = (CardView)d;
			CollapsedCardOrientation direction = CollapsedCardOrientation.Horizontal;
			if((view.CardLayout == CardLayout.Rows && view.MaxCardCountInRow != 1) || (view.CardLayout == CardLayout.Columns && view.MaxCardCountInRow == 1)) {
				direction = CollapsedCardOrientation.Vertical;
			}
			if(!IsDefaultFixedSize(view.FixedSize)) {
				if(view.CardLayout == CardLayout.Rows) {
					direction = CollapsedCardOrientation.Vertical;
				}
				if(view.CardLayout == CardLayout.Columns) {
					direction = CollapsedCardOrientation.Horizontal;
				}
			}
			return direction;
		}
		static bool IsDefaultFixedSize(double size) {
			if(CardsPanel.DefaultFixedSize.IsNotNumber()) {
				return size.IsNotNumber();
			}
			return size == CardsPanel.DefaultFixedSize; 
		}
		internal static Orientation CardLayoutToOrientation(CardLayout cardLayout) {
			return cardLayout == CardLayout.Columns ? Orientation.Horizontal : Orientation.Vertical;
		}
		internal static CardLayout OrientationToCardLayout(Orientation orientation) {
			return orientation == Orientation.Horizontal ? CardLayout.Columns : CardLayout.Rows;
		}
		static object CoerceMinFixedSize(DependencyObject d, object baseValue) {
			CardView cardView = (CardView)d;
			double result = Math.Max((double)baseValue, cardView.MinFixedSize);
			cardView.FixedSize = Math.Max(cardView.FixedSize, result);
			return result;
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("CardViewCollapsedCardOrientation")]
#endif
public CollapsedCardOrientation CollapsedCardOrientation {
			get { return (CollapsedCardOrientation)GetValue(CollapsedCardOrientationProperty); }
			internal set { SetValue(CollapsedCardOrientationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewFocusedCardBorderTemplate"),
#endif
Category(Categories.Appearance)]
		public ControlTemplate FocusedCardBorderTemplate {
			get { return (ControlTemplate)GetValue(FocusedCardBorderTemplateProperty); }
			set { SetValue(FocusedCardBorderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewFocusedCellBorderCardViewTemplate"),
#endif
Category(Categories.Appearance)]
		public ControlTemplate FocusedCellBorderCardViewTemplate {
			get { return (ControlTemplate)GetValue(FocusedCellBorderCardViewTemplateProperty); }
			set { SetValue(FocusedCellBorderCardViewTemplateProperty, value); }
		}
		public ControlTemplate VerticalFocusedGroupRowBorderTemplate {
			get { return (ControlTemplate)GetValue(VerticalFocusedGroupRowBorderTemplateProperty); }
			set { SetValue(VerticalFocusedGroupRowBorderTemplateProperty, value); }
		}
		[Obsolete("Use the DataControlBase.SelectionMode property instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), XtraSerializableProperty, Category(Categories.OptionsSelection)]
		public CardViewSelectMode MultiSelectMode {
			get { return (CardViewSelectMode)GetValue(MultiSelectModeProperty); }
			set { SetValue(MultiSelectModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardStyle"),
#endif
 Category(Categories.Appearance)]
		public Style CardStyle {
			get { return (Style)GetValue(CardStyleProperty); }
			set { SetValue(CardStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardLayout"),
#endif
 Category(GridCategories.OptionsCards), XtraSerializableProperty]
		public CardLayout CardLayout {
			get { return (CardLayout)GetValue(CardLayoutProperty); }
			set { SetValue(CardLayoutProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewShowCardExpandButton"),
#endif
 Category(GridCategories.OptionsCards), XtraSerializableProperty]
		public bool ShowCardExpandButton {
			get { return (bool)GetValue(ShowCardExpandButtonProperty); }
			set { SetValue(ShowCardExpandButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("CardViewOrientation")]
#endif
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			private set { SetValue(OrientationPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate CardTemplate {
			get { return (DataTemplate)GetValue(CardTemplateProperty); }
			set { SetValue(CardTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector CardTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CardTemplateSelectorProperty); }
			set { SetValue(CardTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("CardViewActualCardTemplateSelector")]
#endif
		public DataTemplateSelector ActualCardTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualCardTemplateSelectorProperty); }
			private set { SetValue(ActualCardTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardHeaderTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate CardHeaderTemplate {
			get { return (DataTemplate)GetValue(CardHeaderTemplateProperty); }
			set { SetValue(CardHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardHeaderTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector CardHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CardHeaderTemplateSelectorProperty); }
			set { SetValue(CardHeaderTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("CardViewActualCardHeaderTemplateSelector")]
#endif
		public DataTemplateSelector ActualCardHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualCardHeaderTemplateSelectorProperty); }
			private set { SetValue(ActualCardHeaderTemplateSelectorPropertyKey, value); }
		}
		[Obsolete("Use the CardHeaderBinding property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(null), Category(GridCategories.OptionsCards)]
		public BindingBase CardHeaderDisplayMemberBinding { get { return CardHeaderBinding; } set { CardHeaderBinding = value; } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardHeaderDisplayMemberBinding"),
#endif
 DefaultValue(null), Category(GridCategories.OptionsCards)]
		public BindingBase CardHeaderBinding { get; set; }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardRowTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate CardRowTemplate {
			get { return (DataTemplate)GetValue(CardRowTemplateProperty); }
			set { SetValue(CardRowTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardRowTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector CardRowTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CardRowTemplateSelectorProperty); }
			set { SetValue(CardRowTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("CardViewActualCardRowTemplateSelector")]
#endif
		public DataTemplateSelector ActualCardRowTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualCardRowTemplateSelectorProperty); }
			private set { SetValue(ActualCardRowTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewSeparatorTemplate"),
#endif
 Category(GridCategories.OptionsCards)]
		public DataTemplate SeparatorTemplate {
			get { return (DataTemplate)GetValue(SeparatorTemplateProperty); }
			set { SetValue(SeparatorTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewSeparatorThickness"),
#endif
 Category(GridCategories.OptionsCards), XtraSerializableProperty]
		public double SeparatorThickness {
			get { return (double)GetValue(SeparatorThicknessProperty); }
			set { SetValue(SeparatorThicknessProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewScrollMode"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, GridUIProperty]
		public ScrollMode ScrollMode {
			get { return (ScrollMode)GetValue(ScrollModeProperty); }
			set { SetValue(ScrollModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewFixedSize"),
#endif
 Category(GridCategories.OptionsCards), XtraSerializableProperty, GridUIProperty]
		public double FixedSize {
			get { return (double)GetValue(FixedSizeProperty); }
			set { SetValue(FixedSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewMinFixedSize"),
#endif
 Category(GridCategories.OptionsCards), XtraSerializableProperty]
		public double MinFixedSize {
			get { return (double)GetValue(MinFixedSizeProperty); }
			set { SetValue(MinFixedSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewAllowCardResizing"),
#endif
 Category(GridCategories.OptionsCards), XtraSerializableProperty]
		public bool AllowCardResizing {
			get { return (bool)GetValue(AllowCardResizingProperty); }
			set { SetValue(AllowCardResizingProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("CardViewIsResizingEnabled")]
#endif
		public bool IsResizingEnabled {
			get { return (bool)GetValue(IsResizingEnabledProperty); }
			private set { SetValue(IsResizingEnabledPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewMaxCardCountInRow"),
#endif
 Category(GridCategories.OptionsCards), XtraSerializableProperty]
		public int MaxCardCountInRow {
			get { return (int)GetValue(MaxCardCountInRowProperty); }
			set { SetValue(MaxCardCountInRowProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardAlignment"),
#endif
 Category(GridCategories.OptionsCards), XtraSerializableProperty]
		public Alignment CardAlignment {
			get { return (Alignment)GetValue(CardAlignmentProperty); }
			set { SetValue(CardAlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewCardMargin"),
#endif
 Category(GridCategories.OptionsCards), XtraSerializableProperty]
		public Thickness CardMargin {
			get { return (Thickness)GetValue(CardMarginProperty); }
			set { SetValue(CardMarginProperty, value); }
		}
		[ Category(GridCategories.OptionsCards), XtraSerializableProperty]
		public UseCardLightweightTemplates? UseLightweightTemplates {
			get { return (UseCardLightweightTemplates?)GetValue(UseLightweightTemplatesProperty); }
			set { SetValue(UseLightweightTemplatesProperty, value); }
		}
		public double LeftGroupAreaIndent {
			get { return (double)GetValue(LeftGroupAreaIndentProperty); }
			set { SetValue(LeftGroupAreaIndentProperty, value); }
		}
		#region Printing properties
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewPrintCardRowTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintCardRowTemplate {
			get { return (DataTemplate)GetValue(PrintCardRowTemplateProperty); }
			set { SetValue(PrintCardRowTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewPrintCardTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintCardTemplate {
			get { return (DataTemplate)GetValue(PrintCardTemplateProperty); }
			set { SetValue(PrintCardTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewPrintCardContentTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintCardContentTemplate {
			get { return (DataTemplate)GetValue(PrintCardContentTemplateProperty); }
			set { SetValue(PrintCardContentTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewPrintCardHeaderTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintCardHeaderTemplate {
			get { return (DataTemplate)GetValue(PrintCardHeaderTemplateProperty); }
			set { SetValue(PrintCardHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewPrintCardRowIndentTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintCardRowIndentTemplate {
			get { return (DataTemplate)GetValue(PrintCardRowIndentTemplateProperty); }
			set { SetValue(PrintCardRowIndentTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewPrintCardMargin"),
#endif
 Category(Categories.AppearancePrint)]
		public Thickness PrintCardMargin {
			get { return (Thickness)GetValue(PrintCardMarginProperty); }
			set { SetValue(PrintCardMarginProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewPrintAutoCardWidth"),
#endif
 Category(Categories.AppearancePrint)]
		public bool PrintAutoCardWidth {
			get { return (bool)GetValue(PrintAutoCardWidthProperty); }
			set { SetValue(PrintAutoCardWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewPrintMaximumCardColumns"),
#endif
 Category(Categories.AppearancePrint)]
		public int PrintMaximumCardColumns {
			get { return (int)GetValue(PrintMaximumCardColumnsProperty); }
			set { SetValue(PrintMaximumCardColumnsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("CardViewPrintTotalSummarySeparatorStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintTotalSummarySeparatorStyle {
			get { return (Style)GetValue(PrintTotalSummarySeparatorStyleProperty); }
			set { SetValue(PrintTotalSummarySeparatorStyleProperty, value); }
		}
		#endregion
		protected internal override Orientation OrientationCore { get { return Orientation; } }
		internal override bool IsDesignTimeAdornerPanelLeftAligned { get { return true; } }
		public CardView() : base(null, null, null) {
			UpdateIsResizingEnabled();
		}
		protected override DataViewBehavior CreateViewBehavior() {
			return new CardViewBehavior(this);
		}
		protected override DataViewCommandsBase CreateCommandsContainer() {
			return new CardViewCommands(this);
		}
		protected internal CardData FocusedCardData { get { return (CardData)FocusedRowData; } }
		void UpdateActualCardRowTemplateSelector() {
			ActualCardRowTemplateSelector = new ActualTemplateSelectorWrapper(CardRowTemplateSelector, CardRowTemplate);
		}
		void UpdateActualCardTemplateSelector() {
			ActualCardTemplateSelector = new ActualTemplateSelectorWrapper(CardTemplateSelector, CardTemplate);
		}
		void UpdateActualCardHeaderTemplateSelector() {
			ActualCardHeaderTemplateSelector = new ActualTemplateSelectorWrapper(CardHeaderTemplateSelector, CardHeaderTemplate);
		}
		void UpdateIsResizingEnabled() {
			IsResizingEnabled = !double.IsNaN(FixedSize) && AllowCardResizing;
			CoerceValue(CollapsedCardOrientationProperty);
		}
		protected internal override FrameworkElement CreateGroupControl(GroupRowData rowData) {
			return ((CardViewBehavior)ViewBehavior).CreateElement(() => new GroupCardRowControl(rowData),() =>  new GroupCardRow(), UseCardLightweightTemplates.GroupRow);
		}
		#region Navigation
		Point lastNavPoint;
		Point LastNavPoint { get { return lastNavPoint; } set { lastNavPoint = value; } }
		Point GetElemLocation(UIElement elem) {
			if(elem == null) return new Point(0,0);
			Point pt = elem.TranslatePoint(new Point(0, 0), ScrollInfoOwner as UIElement);
			if(!IsGroupRow(elem) || GridColumn.GetNavigationIndex(elem) != Constants.InvalidNavigationIndex)
				return pt;
			if(Orientation == Orientation.Vertical) pt.X = LastNavPoint.X;
			else pt.Y = LastNavPoint.Y;
			return pt;
		}
		protected virtual bool IsNearer(Point near, Point pt) {
			if(Orientation == Orientation.Vertical)
				return (pt.Y < near.Y) || (pt.Y == near.Y && pt.X <= near.X);
			return (pt.X < near.X) || (pt.X == near.X && pt.Y < near.Y);
		}
		public enum NavObjectDirection { Left, Top, Bottom, Right }
		protected virtual bool ShouldProcessObjectByLocation(Point currLoc, Point loc, NavObjectDirection dir) {
			switch(dir) { 
				case NavObjectDirection.Top:
					return currLoc.Y > loc.Y;
				case NavObjectDirection.Bottom:
					return currLoc.Y < loc.Y;
				case NavObjectDirection.Left:
					return currLoc.X > loc.X;
				case NavObjectDirection.Right:
					return currLoc.X < loc.X;
			}
			return false;
		}
		protected virtual Point GetDistance(Point pt1, Point pt2) {
			return new Point((pt2.X - pt1.X) * (pt2.X - pt1.X), (pt2.Y - pt1.Y) * (pt2.Y - pt1.Y));
		}
		protected virtual DependencyObject FindNearNavObject(DependencyObject currObj, NavObjectDirection dir, bool processCells, bool searchInCurrentCardRow) {
			if(currObj == null) return null;
			Point near = new Point(double.MaxValue, double.MaxValue), currDist = new Point();
			int currentRowIndex, elementIndex;
			if(!CardsHierarchyPanel.TryFindRowElement(DataControl.GetRowVisibleIndexByHandleCore(FocusedRowHandle), out currentRowIndex, out elementIndex)) {
				currentRowIndex = dir == NavObjectDirection.Left || dir == NavObjectDirection.Top ? CardsHierarchyPanel.RowsInfo.Count - 1 : 0;
			}
			DependencyObject nearObject = null;
			double rowOffset = SizeHelper.GetDefinePoint(LastNavPoint);
			for(int i = 0; i < currentRowIndex; i++) {
				rowOffset -= SizeHelper.GetDefineSize(CardsHierarchyPanel.RowsInfo[i].RenderSize);
			}
			for(int i = 0; i < CardsHierarchyPanel.RowsInfo.Count; i++) {
				CardRowInfo rowInfo = CardsHierarchyPanel.RowsInfo[i];
				for(int j = 0; j < rowInfo.Elements.Count; j++) {
					FrameworkElement uiElem = rowInfo.Elements[j].Element;
					Point pt = GetElemLocation(uiElem);
					CardData cardData = uiElem.DataContext as CardData;
					if(!processCells || uiElem.DataContext is GroupRowData || (cardData != null && !cardData.IsExpanded)) {
						currDist = GetDistance(LastNavPoint, SizeHelper.CreatePoint(rowOffset, SizeHelper.GetSecondaryPoint(pt)));
						if(uiElem.IsVisible && ShouldProcessObjectByLocation(LastNavPoint, pt, dir) && IsNearer(near, currDist)) {
							nearObject = uiElem;
							near = currDist;
						}
						continue;
					}
					if(searchInCurrentCardRow && currentRowIndex != i) continue;
					GridCellsEnumerator ce = new GridCellsEnumerator(uiElem);
					while(ce.MoveNext()) {
						if(ce.Current == currObj) continue;
						int index = ce.CurrentNavigationIndex;
						if(index >= VisibleColumns.Count) continue;
						if(!VisibleColumns[index].AllowFocus) continue;
						pt = GetElemLocation(ce.Current as UIElement);
						if(!ShouldProcessObjectByLocation(LastNavPoint, pt, dir)) continue;
						currDist = GetDistance(LastNavPoint, pt);
						if(IsNearer(near, currDist)) {
							nearObject = ce.Current;
							near = currDist;
						}
					}
				}
				rowOffset += SizeHelper.GetDefineSize(rowInfo.RenderSize);
			}
			return nearObject;
		}
		public CardViewCommands CardViewCommands { get { return (CardViewCommands)Commands; } }
		protected virtual DependencyObject FindNearNextRow(DependencyObject row) {
			return FindNearNavObject(row, Orientation == Orientation.Vertical? NavObjectDirection.Bottom: NavObjectDirection.Right, false, false);
		}
		protected virtual DependencyObject FindNearPrevRow(DependencyObject row) {
			return FindNearNavObject(row, Orientation == Orientation.Vertical ? NavObjectDirection.Top : NavObjectDirection.Left, false, false);
		}
		protected internal virtual void MoveNextRowCard() {
			DependencyObject obj = FindNearNextRow(FocusedRowElement);
			if(obj != null) {
				MoveFocusedRow(Grid.GetRowVisibleIndexByHandle(TableView.GetRowHandle(obj).Value));
				return;
			}
			MoveNextCard();
		}
		void MoveNextCard() {
			if(ConvertVisibleIndexToScrollIndex(Grid.GetRowVisibleIndexByHandle(FocusedRowHandle)) < ScrollInfo.Extent - 1)
				MoveFocusedRow(Math.Min((int)ScrollInfo.Extent - 1, ScrollInfoOwner.Offset + ScrollInfoOwner.ItemsOnPage));
		}
		protected internal virtual void MovePrevRowCard() {
			DependencyObject obj = FindNearPrevRow(FocusedRowElement);
			if(obj != null) {
				MoveFocusedRow(Grid.GetRowVisibleIndexByHandle(TableView.GetRowHandle(obj).Value));
				return;
			}
			MovePrevCard();
		}
		bool MovePrevCard() {
			if(ConvertVisibleIndexToScrollIndex(Grid.GetRowVisibleIndexByHandle(FocusedRowHandle)) <= 0) return false;
			int visibleIndex = Math.Max(0, CardsHierarchyPanel.CalcGenerateItemsOffset(DataPresenter.ActualScrollOffset - 1));
			int rowHandle = Grid.GetRowHandleByVisibleIndex(visibleIndex);
			if(!Grid.IsGroupRowHandle(rowHandle)) {
				int rowIndex, elementIndex;
				if(CardsHierarchyPanel.TryFindRowElement(Grid.GetRowVisibleIndexByHandle(FocusedRowHandle), out rowIndex, out elementIndex) && rowIndex == 0)
					if(ViewBehavior.AllowPerPixelScrolling)
						visibleIndex += elementIndex;
					rowHandle = Grid.GetRowHandleByVisibleIndex(visibleIndex);
			}
			MoveFocusedRow(Grid.GetRowVisibleIndexByHandle(rowHandle));
			return true;
		}
		protected override void MoveNextPageCore() {
			int lastVisibleRowIndex = (int)(ScrollInfo.Offset + Math.Max(1, ScrollInfo.Viewport) - 1);
			int focusedRowIndex = (int)CardsHierarchyPanel.CalcExtent(SizeHelper.GetDefineSize(DataPresenter.LastConstraint), DataControl.GetRowVisibleIndexByHandleCore(FocusedRowHandle));
			if(focusedRowIndex < ScrollInfo.Offset || focusedRowIndex > ScrollInfo.Offset + ScrollInfo.Viewport)
				focusedRowIndex = (int)ScrollInfo.Offset;
			if(focusedRowIndex == lastVisibleRowIndex && ScrollInfo.Offset + ScrollInfo.Viewport < ScrollInfo.Extent) {
				double delta = ScrollInfo.Viewport < 1 ? 1 : Math.Floor(ScrollInfo.Viewport) - ScrollInfo.Viewport;
				ScrollInfo.SetOffset(lastVisibleRowIndex + delta);
				EnqueueImmediateAction(new FocusFirstRowAfterPageDownCardView(this));
			} else {
				MoveFocusedRowToLastScrollRow();
			}
		}
		protected internal override void MoveFocusedRowToLastScrollRow() {
			int lastVisibleRow = (int)(ScrollInfo.Offset + Math.Max(1, ScrollInfo.Viewport) - 1);
			int visibleIndex = CardsHierarchyPanel.CalcGenerateItemsOffset(lastVisibleRow);
			int rowIndex, elementIndex;
			if(ViewBehavior.AllowPerPixelScrolling && CardsHierarchyPanel.TryFindRowElement(DataControl.GetRowVisibleIndexByHandleCore(FocusedRowHandle), out rowIndex, out elementIndex)) {
				visibleIndex += elementIndex;
				if(rowIndex + (int)ScrollInfo.Offset == lastVisibleRow)
					visibleIndex = DataControl.VisibleRowCount - 1;
			}
			if(visibleIndex == DataControl.GetRowVisibleIndexByHandleCore(FocusedRowHandle))
				return;
			MoveFocusedRow(visibleIndex);
			SelectionStrategy.OnNavigationComplete(false);
		}
		protected override void MovePrevPageCore() {
			int firstVisibleRowIndex = (int)Math.Min(Math.Ceiling(ScrollInfo.Offset), ScrollInfo.Extent - 1);
			int focusedRowIndex = (int)CardsHierarchyPanel.CalcExtent(SizeHelper.GetDefineSize(DataPresenter.LastConstraint), DataControl.GetRowVisibleIndexByHandleCore(FocusedRowHandle));
			if(focusedRowIndex <= firstVisibleRowIndex && ScrollInfo.Offset > 0 && focusedRowIndex >= (int)ScrollInfo.Offset) {
				double delta = ScrollInfo.Viewport < 1 ? 1 : Math.Floor(ScrollInfo.Viewport) - 1;
				ScrollInfo.SetOffset(firstVisibleRowIndex - delta);
				EnqueueImmediateAction(new ScrollAndFocusFirstAfterPageUpCardView(this, DataControl.GetRowVisibleIndexByHandleCore(FocusedRowHandle), 0));
			} else {
				MoveFocusedRowToFirstScrollRow();
			}
		}
		protected internal override void MoveFocusedRowToFirstScrollRow() {
			int firstVisibleRowIndex = (int)Math.Min(Math.Ceiling(ScrollInfo.Offset), ScrollInfo.Extent - 1);
			int visibleIndex = CardsHierarchyPanel.CalcGenerateItemsOffset(firstVisibleRowIndex);
			int rowIndex, elementIndex;
			if(ViewBehavior.AllowPerPixelScrolling && CardsHierarchyPanel.TryFindRowElement(DataControl.GetRowVisibleIndexByHandleCore(FocusedRowHandle), out rowIndex, out elementIndex)) {
				visibleIndex += elementIndex;
				if(rowIndex + (int)ScrollInfo.Offset == firstVisibleRowIndex)
					visibleIndex = 0;
			}
			if(visibleIndex == DataControl.GetRowVisibleIndexByHandleCore(FocusedRowHandle))
				return;
			MoveFocusedRow(visibleIndex);
			SelectionStrategy.OnNavigationComplete(false);
		}
		protected internal virtual void MovePrevColumnCard() {
			MovePrevRow();
		}
		protected internal virtual void MoveNextColumnCard() {
			MoveNextRow();
		}
		protected internal virtual void MoveFirstVisibleRow() {
			MoveFocusedRow(ScrollInfoOwner.Offset);
		}
		protected internal virtual void MoveLastVisibleRow() {
			MoveFocusedRow(ScrollInfoOwner.Offset + ScrollInfoOwner.ItemsOnPage - 1);
		}
		ScrollInfoBase ScrollInfo { get { return DataPresenter.ScrollInfoCore.DefineSizeScrollInfo; } }
		protected virtual void UpdateNavigationIndex(DependencyObject dobj, NavObjectDirection loc) {
			if(dobj == null) {
				if((Orientation == Orientation.Vertical && loc == NavObjectDirection.Top) || (Orientation == Orientation.Horizontal && loc == NavObjectDirection.Left)) {
					if(MovePrevCard())
						NavigationIndex = VisibleColumns.Count - 1;
				}
				else if((Orientation == Orientation.Vertical && loc == NavObjectDirection.Bottom) || (Orientation == Orientation.Horizontal && loc == NavObjectDirection.Right)) {
					MoveNextCard();
				}
				return;
			}
			RowHandle rHandle = TableView.GetRowHandle(FindParentRow(dobj));
			if(rHandle != null && rHandle.Value != TableView.GetRowHandle(FocusedRowElement).Value) {
				MoveFocusedRow(Grid.GetRowVisibleIndexByHandle(rHandle.Value));
			}
			if(GridColumn.GetNavigationIndex(dobj) != Constants.InvalidNavigationIndex)
				NavigationIndex = GridColumn.GetNavigationIndex(dobj);
			return;
		}
		protected internal override int ConvertVisibleIndexToScrollIndex(int visibleIndex) {
			if(CardsHierarchyPanel == null) return visibleIndex;
			return (int)CardsHierarchyPanel.CalcExtent(SizeHelper.GetDefineSize(DataPresenter.LastConstraint), visibleIndex);
		}
		protected internal override void OnDataReset() {
			base.OnDataReset();
			if(CardsHierarchyPanel != null)
				CardsHierarchyPanel.ClearRows();
		}
		bool SearchInHorizontalRow { get { return Orientation == Orientation.Horizontal ? true : false; } }
		bool SearchInVerticalRow { get { return Orientation == Orientation.Vertical ? true : false; } }
		DependencyObject CurrNavigationObject { get { return CurrentCell != null ? CurrentCell : FocusedRowElement; } }
		protected virtual DependencyObject GetNearUpCell() {
			return FindNearNavObject(CurrNavigationObject, NavObjectDirection.Top, true, SearchInHorizontalRow);
		}
		protected virtual DependencyObject GetNearDownCell() {
			return FindNearNavObject(CurrNavigationObject, NavObjectDirection.Bottom, true, SearchInHorizontalRow);
		}
		protected virtual DependencyObject GetNearLeftCell() {
			return FindNearNavObject(CurrNavigationObject, NavObjectDirection.Left, true, SearchInVerticalRow);
		}
		protected virtual DependencyObject GetNearRightCell() {
			return FindNearNavObject(CurrNavigationObject, NavObjectDirection.Right, true, SearchInVerticalRow);
		}
		protected internal virtual void MoveUpCell() {
			UpdateNavigationIndex(GetNearUpCell(), NavObjectDirection.Top);
		}
		protected internal virtual void MoveDownCell() {
			UpdateNavigationIndex(GetNearDownCell(), NavObjectDirection.Bottom);
		}
		protected internal virtual void MoveLeftCell() {
			UpdateNavigationIndex(GetNearLeftCell(), NavObjectDirection.Left);
		}
		protected internal virtual void MoveRightCell() {
			UpdateNavigationIndex(GetNearRightCell(), NavObjectDirection.Right);
		}
		protected internal override bool UpdateRowsState() {
			bool res = false;
			if(RowsStateDirty)
				CurrentCell = null;
			res = base.UpdateRowsState();
			if(NavigationStyle == GridViewNavigationStyle.Row || DataControl.IsGroupRowHandleCore(FocusedRowHandle)) {
				RowData rowData = GetRowData(FocusedRowHandle);
				if(rowData != null)
					UpdateLastNavPoint(rowData.WholeRowElement);
			} else {
				UpdateLastNavPoint(CurrentCell);
			}
			return res;
		}
		protected internal override void ForceLayout() {
			base.ForceLayout();
			if(CardsHierarchyPanel != null)
				CardsHierarchyPanel.InvalidateMeasure();
		}
		protected virtual void UpdateLastNavPoint(DependencyObject obj) {
			if(obj == null) return;
			if(!IsGroupRow(obj) || GridColumn.GetNavigationIndex(obj) != Constants.InvalidNavigationIndex) {
				LastNavPoint = GetElemLocation(obj as UIElement);
				return;
			}
			if(Orientation == Orientation.Vertical)
				LastNavPoint = new Point(LastNavPoint.X, GetElemLocation(obj as UIElement).Y);
			else
				LastNavPoint = new Point(GetElemLocation(obj as UIElement).X, LastNavPoint.Y);
		}
		protected override void OnFocusedRowHandleChangedCore(int oldRowHandle) {
			base.OnFocusedRowHandleChangedCore(oldRowHandle);
			UpdateLastNavPoint(FocusedRowElement);
		}
		internal override void OnCurrentCellChanged() {
			base.OnCurrentCellChanged();
			if(CurrentCell == null) return;
			UpdateLastNavPoint(CurrentCell);
		}
		void OnCardLayoutChanged() {
			Orientation = CardLayoutToOrientation(CardLayout);
			ScrollInfoOwner.Do(x => x.OnDefineScrollInfoChanged());
			CoerceValue(CollapsedCardOrientationProperty);
			UpdateRowData(UpdateRowDataChangeLayout);
		}
		#endregion
		#region expand/collapse cards API
		public bool IsCardExpanded(int rowHandle) {
			DependencyObject rowState = Grid.GetRowState(rowHandle, false);
			return rowState == null || CardData.GetIsExpanded(rowState);
		}
		public void ExpandAllCards() {
			SetAllCardsExpanded(true);
		}
		public void CollapseAllCards() {
			SetAllCardsExpanded(false);
		}
		protected internal virtual void SetAllCardsExpanded(bool expanded) {
			for(int i = 0; i < Grid.VisibleRowCount; i++) {
				SetCardExpandedCore(Grid.GetRowHandleByVisibleIndex(i), expanded);
			}
			UpdateRowData(UpdateRowDataIsExpanded);
		}
		public void ExpandCard(int rowHandle) {
			SetCardExpanded(rowHandle, true);
		}
		public void CollapseCard(int rowHandle) {
			SetCardExpanded(rowHandle, false);
		}
		protected internal virtual void SetCardExpanded(int rowHandle, bool expanded) {
			SetCardExpandedCore(rowHandle, expanded);
			UpdateRowDataByRowHandle(rowHandle, UpdateRowDataIsExpanded);
		}
		internal void ChangeCardExpanded(int rowHandle) {
			SetCardExpanded(rowHandle, !IsCardExpanded(rowHandle));
		}
		void SetCardExpandedCore(int rowHandle, bool expanded) {
			DependencyObject rowState = Grid.GetRowState(rowHandle, !expanded);
			if(rowState == null) return;
			CardData.SetIsExpanded(rowState, expanded);
		}
		void UpdateRowDataIsExpanded(RowData rowData) {
			CardData cardData = rowData as CardData;
			if(cardData != null)
				cardData.UpdateIsExpanded();
		}
		void UpdateRowDataChangeLayout(RowData rowData) {
			GroupRowData groupData = rowData as GroupRowData;
			if(groupData != null)
				groupData.UpdateCardLayout();
		}
		void OnChangeCardExpanded(ExecutedRoutedEventArgs e) {
			if(e.Parameter is int)
				ChangeCardExpanded((int)e.Parameter);
		}
		#endregion
		#region CopyRows
		internal void GetDataRowTextCore(StringBuilder sb, int rowHandle) {
			if(rowHandle != GridControl.InvalidRowHandle) {
				if(!Grid.IsGroupRowHandle(rowHandle)) {
					string header = GetCardHeaderText(rowHandle);
					if(header != null) {
						sb.Append(header);
						sb.Append("\r\n");
					}
				}
				for(int i = 0; i < VisibleColumns.Count; i++) {
					string headerText = string.Empty;
					string cellText = string.Empty;
					if(ActualClipboardCopyWithHeaders && !Grid.IsGroupRowHandle(rowHandle)) {
						sb.Append(GetTextForClipboard(GridControl.InvalidRowHandle, i));
						sb.Append("\t");
					}
					sb.Append(GetTextForClipboard(rowHandle, i));
					sb.Append("\r\n");
					if(Grid.IsGroupRowHandle(rowHandle)) {
						return;
					}
				}
			}
		}
		CardHeaderData cardHeaderData = null;
		protected virtual string GetCardHeaderText(int rowHandle) {
			if(cardHeaderData == null) {
				cardHeaderData = new CardHeaderData();
			}
			cardHeaderData.Data = DataProviderBase.GetWpfRow(new RowHandle(rowHandle));
			cardHeaderData.Binding = CardHeaderBinding;
			cardHeaderData.RowData = GetRowData(rowHandle);
			return cardHeaderData.Value as string;
		}
		#endregion
		protected override SelectionStrategyBase CreateSelectionStrategy() {
			if(NavigationStyle == GridViewNavigationStyle.None)
				return new SelectionStrategyNavigationNone(this);
			if(IsMultiRowSelection)
				return new SelectionStrategyRow(this);
			return new SelectionStrategyNone(this);
		}
		void SetSizeExpansion(int value) {
			if(FocusRectPresenter != null)
				FocusRectPresenter.SizeExpansion = value;
		}
		internal override void SetFocusedRectangleOnRow() {
			SetSizeExpansion(2);
			base.SetFocusedRectangleOnRow();
		}
		internal override void SetFocusedRectangleOnCell() {
			var cell = FocusedView.CurrentCellEditor != null ? FocusedView.CurrentCell as FrameworkElement : null;
			if(cell != null && DevExpress.Xpf.Utils.UIElementHelper.IsVisibleInTree(cell, true)) {
				SetSizeExpansion(0);
				base.SetFocusedRectangleOnCell();
			}
			else {
				SetFocusedRectangleOnRow();
			}
		}
		internal override void SetFocusedRectangleOnGroupRow() {
			SetSizeExpansion(0);
			base.SetFocusedRectangleOnGroupRow();
		}
		protected override ControlTemplate GetCellFocusedRectangleTemplate() {
			return FocusedCellBorderCardViewTemplate;
		}
		protected override ControlTemplate GetGroupRowFocusedRectangleTemplate() {
			if(CardLayout == CardLayout.Columns) {
				return VerticalFocusedGroupRowBorderTemplate;
			} else {
				return FocusedGroupRowBorderTemplate;
			}
		}
		protected override ControlTemplate GetRowFocusedRectangleTemplate() {
			return FocusedCardBorderTemplate;
		}
		#region HitTest
		public CardViewHitInfo CalcHitInfo(DependencyObject d) {
			return new CardViewHitInfo(GetStartHitTestObject(d, this), this);
		}
		public CardViewHitInfo CalcHitInfo(Point hitTestPoint) {
			return CalcHitInfo(VisualTreeHelper.HitTest(this, hitTestPoint).VisualHit);
		}
		internal override IDataViewHitInfo CalcHitInfoCore(DependencyObject source) {
			return CalcHitInfo(source);
		}
		#endregion
		internal double CardsPanelMaxSize {
			get {
				CardsHierarchyPanel panel = (CardsHierarchyPanel)LayoutHelper.FindElement(DataPresenter.ContentElement, e => e is CardsHierarchyPanel);
				return panel.MaxSecondarySize;
			}
		}
		internal double CardsPanelViewPort {
			get {
				Size availableSize = new Size(DataPresenter.ActualWidth, DataPresenter.ActualHeight);
				return SizeHelper.GetSecondarySize(availableSize);
			}
		}
		internal override FrameworkElement GetRowVisibleElement(RowDataBase rowData) {
			return ((RowData)rowData).RowElement;
		}
		protected override RowData CreateFocusedRowData() {
			return ViewBehavior.CreateRowDataCore(VisualDataTreeBuilder, true);
		}
		protected internal override bool ShouldChangeForwardIndex(int rowHandle) {
			int visibleIndex = DataControl.GetRowVisibleIndexByHandleCore(rowHandle);
			return GetRowElementByVisibleIndex(visibleIndex) != null;
		}
		FrameworkElement GetRowElementByVisibleIndex(int visibleIndex) {
			return GetRowElementByRowHandle(DataControl.GetRowHandleByVisibleIndexCore(visibleIndex));
		}
		CardsHierarchyPanel CardsHierarchyPanel { get { return DataPresenter != null ? DataPresenter.Panel as CardsHierarchyPanel : null; } }
		protected internal override double CalcOffsetForward(int rowHandle, bool perPixelScrolling) {
			RowData rowData = GetRowData(rowHandle);
			if(rowData == null || CardsHierarchyPanel == null) return 0;
			double invisibleSize = GetItemInvisibleSize(rowData.WholeRowElement) - CardsHierarchyPanel.GetRowOffset();
			double offset = 0;
			for(int i = 0; i < CardsHierarchyPanel.RowsInfo.Count; i++) {
				CardRowInfo rowInfo = CardsHierarchyPanel.RowsInfo[i];
				double rowSize = SizeHelper.GetDefineSize(rowInfo.RenderSize);
				if(rowSize >= invisibleSize)
					return offset + invisibleSize / rowSize;
				invisibleSize -= rowSize;
				offset++;
			}
			return 0;
		}
		double GetElementDefineOffset(FrameworkElement elem) {
			return SizeHelper.GetDefinePoint(LayoutHelper.GetRelativeElementRect(elem, DataPresenter).Location());
		}
		protected internal override MultiSelectMode GetSelectionMode() {
			return SelectionModeHelper.ConvertToMultiSelectMode((CardViewSelectMode)GetValue(MultiSelectModeProperty));
		}
		protected internal override bool ShouldUpdateCellData { get { return true; } }
		protected internal override bool ForceShowTotalSummaryColumnName { get { return true; } }
		#region Printing
		protected internal override DataTemplate GetPrintRowTemplate() {
			return PrintCardRowTemplate;
		}
		protected override IRootDataNode CreateRootNode(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			return CardViewPrintingHelper.CreatePrintingTreeNode(this, usablePageSize);
		}
		protected override void CreateRootNodeAsync(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			CardViewPrintingHelper.CreatePrintingTreeNodeAsync(this, usablePageSize);
		}
		protected override void PagePrintedCallback(IEnumerator pageBrickEnumerator, Dictionary<IVisualBrick, IOnPageUpdater> brickUpdaters) {
		}
		protected internal override PrintingDataTreeBuilderBase CreatePrintingDataTreeBuilder(double totalHeaderWidth, ItemsGenerationStrategyBase itemsGenerationStrategy, MasterDetailPrintInfo masterDetailPrintInfo, BandsLayoutBase bandsLayout) {
			return new CardViewPrintingDataTreeBuilder(this, totalHeaderWidth, itemsGenerationStrategy);
		}
		protected internal virtual CardViewPrintingDataTreeBuilder CreatePrintingDataTreeBuilder(double totalHeaderWidth, ItemsGenerationStrategyBase itemsGenerationStrategy) {
			return (CardViewPrintingDataTreeBuilder)CreatePrintingDataTreeBuilder(totalHeaderWidth, itemsGenerationStrategy, null, null);
		}
		#region CSV
		public void ExportToCsv(Stream stream) {
			PrintHelper.ExportToCsv(this, stream);
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			PrintHelper.ExportToCsv(this, stream, options);
		}
		public void ExportToCsv(string filePath) {
			PrintHelper.ExportToCsv(this, filePath);
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			PrintHelper.ExportToCsv(this, filePath, options);
		}
		#endregion
		#region XLS
		public void ExportToXls(Stream stream) {
			PrintHelper.ExportToXls(this, stream);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			PrintHelper.ExportToXls(this, stream, options);
		}
		public void ExportToXls(string filePath) {
			PrintHelper.ExportToXls(this, filePath);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			PrintHelper.ExportToXls(this, filePath, options);
		}
		#endregion
		#region XLSX
		public void ExportToXlsx(Stream stream) {
			PrintHelper.ExportToXlsx(this, stream);
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			PrintHelper.ExportToXlsx(this, stream, options);
		}
		public void ExportToXlsx(string filePath) {
			PrintHelper.ExportToXlsx(this, filePath);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			PrintHelper.ExportToXlsx(this, filePath, options);
		}
		#endregion
		#endregion
		internal List<CardRowInfo> CardRowInfoCollection { get { return CardsHierarchyPanel.RowsInfo; } }
		CardViewBehavior CardViewBehavior { get { return ViewBehavior as CardViewBehavior; } }
		protected override bool IsGroupRowOptimized { get { return CardViewBehavior != null && CardViewBehavior.UseLightweightTemplatesHasFlag(UseCardLightweightTemplates.GroupRow); } }
	}
}
