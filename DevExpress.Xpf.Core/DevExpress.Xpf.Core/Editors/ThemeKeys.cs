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

using DevExpress.Xpf.Utils.Themes;
using DevExpress.XtraEditors.DXErrorProvider;
namespace DevExpress.Xpf.Editors.Themes {
	public enum CameraControlsThemeKeys {
		Template,
		BorderTemplate,
		SettingsButtonStyle,
		CameraSettingsTemplate,
		TakePictureControlTemplate,
		EmptyBorderTemplate
	}
	public enum TokenEditorThemeKeys {
		BorderStyle,
		Template,
		TokenContainerTemplate,
		EmptyTokenContainerTemplate,
		CellPresenterTemplate,
		ActiveEditorStyle,
		DeleteButtonTemplate,
		DeleteButtonStyle,
		TokenEditorMargin,
		TokenHeight
	}
	public enum FlyoutControlThemeKeys {
		Background,
		Foreground,
		Padding,
		BorderBrush,
		BorderThickness
	}
	public enum ProgressBarEditThemeKeys {
		Template,
		DisplayTemplate,
		EditTemplate,
		BorderTemplate,
		EmptyBorderTemplate,
		PanelStyle,
		PanelTemplate,
		PanelMarqueeCyrcleTemplate,
		PanelMarqueePingPongTemplate,
		PanelLeftSideHorizontalStyle,
		PanelLeftSideVerticalStyle,
		PanelRightSideHorizontalStyle,
		PanelRightSideVerticalStyle,
		PanelBackgroundHorizontalStyle,
		PanelBackgroundVerticalStyle,
		PanelForegroundStyle,
		PanelForegroundTemplate,
		PanelLeftSideHorizontalTemplate,
		PanelRightSideHorizontalTemplate,
		PanelBackgroundHorizontalTemplate,
		PanelLeftSideVerticalTemplate,
		PanelRightSideVerticalTemplate,
		PanelBackgroundVerticalTemplate,
		ContentStyle,
		ContentTemplate,
		ContentTemplate_Content,
		ContentTemplate_None,
		AnmationStyle,
		AdditionalForeground,
	}
	public enum TrackBarEditThumbThemeKeys {
		HorizontalThumbStyle,
		HorizontalThumbTemplate,
		HorizontalThumbTickPlacementTopLeftTemplate,
		HorizontalThumbTickPlacementBottomRightTemplate,
		HorizontalThumbTickPlacementBothTemplate,
		VerticalThumbStyle,
		VerticalThumbTemplate,
		VerticalThumbTickPlacementTopLeftTemplate,
		VerticalThumbTickPlacementBottomRightTemplate,
		VerticalThumbTickPlacementBothTemplate,
		LeftHorizontalThumbStyle,
		LeftHorizontalThumbTemplate,
		LeftHorizontalThumbTickPlacementTopLeftTemplate,
		LeftHorizontalThumbTickPlacementBottomRightTemplate,
		LeftHorizontalThumbTickPlacementBothTemplate,
		LeftVerticalThumbStyle,
		LeftVerticalThumbTemplate,
		LeftVerticalThumbTickPlacementTopLeftTemplate,
		LeftVerticalThumbTickPlacementBottomRightTemplate,
		LeftVerticalThumbTickPlacementBothTemplate,
		RightHorizontalThumbStyle,
		RightHorizontalThumbTemplate,
		RightHorizontalThumbTickPlacementTopLeftTemplate,
		RightHorizontalThumbTickPlacementBottomRightTemplate,
		RightHorizontalThumbTickPlacementBothTemplate,
		RightVerticalThumbStyle,
		RightVerticalThumbTemplate,
		RightVerticalThumbTickPlacementTopLeftTemplate,
		RightVerticalThumbTickPlacementBottomRightTemplate,
		RightVerticalThumbTickPlacementBothTemplate,
		ScrollableLeftHorizontalThumbStyle,
		ScrollableLeftHorizontalThumbTemplate,
		ScrollableLeftHorizontalThumbTickPlacementTopLeftTemplate,
		ScrollableLeftHorizontalThumbTickPlacementBottomRightTemplate,
		ScrollableLeftHorizontalThumbTickPlacementBothTemplate,
		ScrollableLeftVerticalThumbStyle,
		ScrollableLeftVerticalThumbTemplate,
		ScrollableLeftVerticalThumbTickPlacementTopLeftTemplate,
		ScrollableLeftVerticalThumbTickPlacementBottomRightTemplate,
		ScrollableLeftVerticalThumbTickPlacementBothTemplate,
		ScrollableRightHorizontalThumbStyle,
		ScrollableRightHorizontalThumbTemplate,
		ScrollableRightHorizontalThumbTickPlacementTopLeftTemplate,
		ScrollableRightHorizontalThumbTickPlacementBottomRightTemplate,
		ScrollableRightHorizontalThumbTickPlacementBothTemplate,
		ScrollableRightVerticalThumbStyle,
		ScrollableRightVerticalThumbTemplate,
		ScrollableRightVerticalThumbTickPlacementTopLeftTemplate,
		ScrollableRightVerticalThumbTickPlacementBottomRightTemplate,
		ScrollableRightVerticalThumbTickPlacementBothTemplate,
		ScrollableCenterHorizontalThumbStyle,
		ScrollableCenterHorizontalThumbTemplate,
		ScrollableCenterHorizontalThumbTickPlacementTopLeftTemplate,
		ScrollableCenterHorizontalThumbTickPlacementBottomRightTemplate,
		ScrollableCenterHorizontalThumbTickPlacementBothTemplate,
		ScrollableCenterVerticalThumbStyle,
		ScrollableCenterVerticalThumbTemplate,
		ScrollableCenterVerticalThumbTickPlacementTopLeftTemplate,
		ScrollableCenterVerticalThumbTickPlacementBottomRightTemplate,
		ScrollableCenterVerticalThumbTickPlacementBothTemplate,
	}
	public enum TrackBarEditThemeKeys {
		Background,
		Foreground,
		Template,
		DisplayTemplate,
		EditTemplate,
		EmptyBorderTemplate,
		PanelStyle,
		TickBarStyle,
		LeftStepButtonStyle,
		LeftStepButtonTemplate,
		LeftStepButtonSize,
		RightStepButtonStyle,
		RightStepButtonTemplate,
		RightStepButtonSize,
		PanelTemplate,
		PanelHorizontalTemplate,
		PanelVerticalTemplate,
		RangePanelHorizontalTemplate,
		RangePanelVerticalTemplate,
		ScrollableRangePanelHorizontalTemplate,
		ScrollableRangePanelVerticalTemplate,
		PanelLeftSideTemplate,
		PanelRightSideTemplate,
		PanelBackgroundTemplate,
		PanelLeftSideStyle,
		PanelRightSideStyle,
		PanelBackgroundStyle,
		ContentStyle,
		ContentTemplate,
		ThumbStyle,
		HorizontalThumbStyle,
		VerticalThumbStyle,
		LeftThumbStyle,
		RightThumbStyle,
		ThumbTemplate,
		HorizontalThumbTemplate,
		VerticalThumbTemplate,
		LeftThumbTemplate,
		RightThumbTemplate,
		SelectionRangeHorizontalStyle,
		SelectionRangeVerticalStyle,
		SelectionRangeTemplate,
		SelectionRangeLeftButtonStyle,
		SelectionRangeRightButtonStyle,
		SelectionRangeLeftButtonTemplate,
		SelectionRangeRightButtonTemplate,
		HorizontalBackgroundStyle,
		HorizontalBackgroundTemplate,
		VerticalBackgroundStyle,
		VerticalBackgroundTemplate,
		HorizontalSelectionRangeBackgroundStyle,
		HorizontalSelectionRangeBackgroundTemplate,
		VerticalSelectionRangeBackgroundStyle,
		VerticalSelectionRangeBackgroundTemplate,
		HorizontalSelectionRangeStyle,
		HorizontalSelectionRangeTemplate,
		VerticalSelectionRangeStyle,
		VerticalSelectionRangeTemplate,
		SelectionRangeBackgroundTemplate,
	}
	public enum RangeEditThemeKeys {
		BorderTemplate,
		EmptyBorderTemplate,
		Template,
		DisplayTemplate,
		DateTimePanelStyle,
		DateTimePanelTemplate,
		NumericPanelStyle,
		NumericPanelTemplate,
		RangeEditSelectorControlTemplate,
		RangeEditVisibleIntervalViewerTemplate,
		RangeEditVisibleIntervalViewerLeftPanelTemplate,
		RangeEditVisibleIntervalViewerCenterPanelTemplate,
		RangeEditVisibleIntervalViewerRightPanelTemplate,
		RangeEditScrollerPanelHorizontalStyle,
		RangeEditScrollerPanelHorizontalTemplate,
		RangeEditScrollerHorizontalBackgroundStyle,
		RangeEditScrollerHorizontalBackgroundTemplate,
		RangeEditScrollerPanelLeftSideStyle,
		RangeEditScrollerPanelLeftSideTemplate,
		RangeEditScrollerScrollableCenterHorizontalThumbStyle,
		RangeEditScrollerScrollableCenterHorizontalThumbTemplate,
		RangeEditScrollerScrollableLeftHorizontalThumbStyle,
		RangeEditScrollerScrollableLeftHorizontalThumbTemplate,
		RangeEditScrollerScrollableRightHorizontalThumbStyle,
		RangeEditScrollerScrollableRightHorizontalThumbTemplate,
		RangeEditScrollerPanelRightSideStyle,
		RangeEditScrollerPanelRightSideTemplate,
		DateTimePanelScrollerStyle,
		DateTimePanelScrollerTemplate,
		DateTimePanelScrollerTrackBarStyle,
		DateTimePanelScrollerTrackBarTemplate,
		DateTimePanelIntervalViewerLeftTemplate,
		DateTimePanelIntervalViewerCenterTemplate,
		DateTimePanelIntervalViewerRightTemplate,
		DateTimePanelSelectorStyle,
		DateTimePanelSelectorTemplate,
		PanelSelectorTrackBarStyle,
		PanelSelectorTrackBarTemplate,
		PanelSelectorLeftHorizontalThumbStyle,
		PanelSelectorLeftHorizontalThumbTemplate,
		PanelSelectorRightHorizontalThumbStyle,
		PanelSelectorRightHorizontalThumbTemplate,
		PanelSelectorCenterHorizontalThumbStyle,
		DateTimePanelSelectorCenterHorizontalThumbTemplate,
		DateTimePanelSelectorIntervalViewerLeftTemplate,
		DateTimePanelSelectorIntervalViewerRightTemplate,
		DateTimePanelSelectorIntervalViewerCenterTemplate,
		DateTimePanelGroupIntervalItemTemplate,
		DateTimePanelItemIntervalItemTemplate,
		NumericPanelItemIntervalItemStyle,
		NumericPanelItemIntervalItemTemplate,
		NumericPanelGroupIntervalItemStyle,
		NumericPanelGroupIntervalItemTemplate,
		NumericPanelSelectorStyle,
		NumericPanelSelectorTemplate,
		NumericPanelScrollerStyle,
		NumericPanelScrollerTemplate,
	}
	public enum InplaceBaseEditThemeKeys {
		CommonBaseEditInplaceInactiveTemplate,
		SelectedItemTemplate,
		SelectedItemImageTemplate,
		CommonBaseEditInplaceInactiveTemplateWithDisplayTemplate,
		RealContentPresenterTemplate,
		ContentPresenterTemplate,
		TextContentPresenterTemplate,
		CheckEditInplaceInactiveTemplate,
		ButtonInfoInplaceInactiveTemplate,
		TextEditInplaceInactiveTemplate,
		TextEditInplaceActiveTemplate,
		TextEditInplaceTemplateSelector,		
		ButtonInfoTemplateSelector,
		DefaultButtonInfoTemplateSelector,
		ValidationErrorTemplateSelector,
		DropDownGlyph,
		RegularGlyph,
		DownGlyph,
		UpGlyph,
		LeftGlyph,
		RightGlyph,
		CancelGlyph,
		ApplyGlyph,
		PlusGlyph,
		MinusGlyph,
		UndoGlyph,
		RedoGlyph,
		RefreshGlyph,
		SearchGlyph,
		NextPageGlyph,
		PrevPageGlyph,
		LastGlyph,
		FirstGlyph,
		EditGlyph,
		SpinUpGlyph,
		SpinDownGlyph,
		SpinLeftGlyph,
		SpinRightGlyph,
		NoneGlyph,
		CriticalErrorTemplate,
		ValidationErrorTemplate,
		WarningErrorTemplate,
		InformationErrorTemplate,
		ValidationErrorToolTipTemplate,
		TrimmedTextToolTipTemplate,
		RenderCheckBoxTemplate,
		RenderButtonTemplate,
		RenderSpinLeftButtonTemplate,
		RenderSpinUpButtonTemplate,
		RenderSpinRightButtonTemplate,
		RenderSpinDownButtonTemplate,
		RenderSpinButtonTemplate,
		BorderTemplate,
		TextEditPadding,
		CheckEditPadding,
		ValidationErrorPadding,
		AutoCompleteBoxTemplate,
	}
	public enum TextEditThemeKeys {
		TextStyle,
		TextInplaceStyle,
		TextBlockStyle,
		TextBlockInplaceStyle,
		DisplayTemplate,
		DisplayInplaceTemplate,
		EditTemplate,
		EditInplaceTemplate,
		EditNonEditableTemplate,
		EditNonEditableInplaceTemplate,
		Template,
		ErrorContentPresenterStyle,
		ErrorControlTemplate,
		BorderTemplate,
		EmptyBorderTemplate,
		NullTextForeground,
		CaretBrush
	}
	public enum PasswordBoxEditThemeKeys {
		PasswordBoxStyle,
		PasswordBoxTemplate,
		DisplayTemplate,
		EditTemplate,
		Template,
		NullTextEditorStyle,
		CapsLockWarningToolTipTemplate,
	}
	public enum CheckEditThemeKeys {
		CheckEditBoxTemplate,
		NormalCheckEditBoxTemplate,
		RenderTemplate,
		CheckEditBoxStyle,
		CheckEditBoxInplaceStyle,
		DisplayTemplate,
		EditTemplate,
		DisplayInplaceTemplate,
		EditInplaceTemplate,
		Template,
		Padding,
		FocusVisualStyle,
		EmptyFocusVisualStyle,
		RenderInfo,
		CheckSize
	}
	public enum ButtonEditThemeKeys {
		[BlendVisibility(false)]
		Template,
		[BlendVisibility(false)]
		TemplateWithoutEditBox,
		ItemTemplate,
		ContentEditorStyle,
		ButtonEditPadding,
		ButtonEditPaddingCorrection
	}
	public enum EditorListBoxThemeKeys {
		Style,
		Template,
		ItemTemplate,
		ItemContainerStyle,
		DefaultItemStyle,
		CheckBoxItemStyle,
		CustomItemTemplate,
		RadioButtonItemStyle,
		ScrollViewerTemplate,
		PopupStyle,
	}
	public enum DateEditThemeKeys {
		HeaderButtonTemplate,
		HeaderButtonStyle,
		CurrentDateButtonTemplate,
		CurrentDateButtonStyle,
		ClearButtonStyle,
		ClearButtonTemplate,
		WeekdayNameStyle,
		LeftArrowTemplate,
		RightArrowTemplate,
		LeftArrowStyle,
		RightArrowStyle,
		WeekdayAbbreviationStyle,
		MonthInfoDelimeter,
		WeekNumberDelimeter,
		CellButtonTemplate,
		CellButtonStyle,
		YearInfoTemplate,
		MonthInfoTemplate,
		PopupContentTemplate,
		CalendarPopupContentTemplate,
		PickerPopupContentTemplate,
		CalendarTemplate,
		WeekNumbersStyle,
		CalendarTranserStyle,
		TransferControlFadeStyle
	}
	public enum DateNavigatorThemeKeys {
		Template,
		HeaderButtonTemplate,
		HeaderButtonStyle,
		CurrentDateButtonTemplate,
		CurrentDateButtonStyle,
		ClearButtonStyle,
		ClearButtonTemplate,
		WeekdayNameStyle,
		LeftArrowTemplate,
		RightArrowTemplate,
		LeftArrowStyle,
		RightArrowStyle,
		WeekdayAbbreviationStyle,
		MonthInfoDelimeter,
		WeekNumberDelimeter,
		CellButtonTemplate,
		CellButtonStyle,
		YearInfoTemplate,
		MonthInfoTemplate,
		PopupContentTemplate,
		CalendarTemplate,
		WeekNumberStyle,
		TransferStyle,
		TransferControlFadeStyle,
		CalendarCurrentDateStyle,
		HeaderStyle,
		TodayButtonStyle,
		BackgroundBrush,
	}
	public enum DateNavigatorContentThemeKeys {
		Template
	}
	public enum PopupBaseEditThemeKeys {
		DialogServiceContentTemplate,
		PopupContentTemplate,
		PopupContentContainerTemplate,
		ResizeGripTemplate,
		ResizeGripStyle,
		CloseButtonTemplate,
		AddNewButtonStyle,
		NullValueButtonStyle,
		FooterTemplate,
		PopupTopAreaTemplate,
		PopupTopAreaStyle,
		PopupBottomAreaTemplate,
		PopupBottomAreaStyle
	}
	public enum MemoEditThemeKeys {
		PopupContentTemplate,
		DisplayIconTemplate,
		EditNonEditableInplaceTemplate,
		EditNonEditableIconTemplate,
		EditNonEditableIconInplaceTemplate,
		MemoStyle,
		MemoBackgroundStyle
	}
	public enum ComboBoxThemeKeys {
		BorderTemplate,
		PopupTopAreaTemplate,
		PopupContentTemplate,
		PopupContentContainerTemplate,
		SelectedItemTemplate,
		SelectedItemImageTemplate,
		SelectedItemImagePadding,
		SelectedItemContentPadding,
		AutoCompleteBoxTemplate,
		AutoCompleteBoxNonEditableTemplate,
		AutoCompleteBoxDisplayTemplate,
		LoadingButtonTemplate,
		LoadingButtonStyle
	}
	public enum ButtonsThemeKeys {
		DropDownGlyph,
		RegularGlyph,
		DownGlyph,
		UpGlyph,
		LeftGlyph,
		RightGlyph,
		CancelGlyph,
		ApplyGlyph,
		PlusGlyph,
		MinusGlyph,
		UndoGlyph,
		RedoGlyph,
		RefreshGlyph,
		SearchGlyph,
		NextPageGlyph,
		PrevPageGlyph,
		LastGlyph,
		FirstGlyph,
		EditGlyph,
		SpinUpGlyph,
		SpinDownGlyph,
		SpinLeftGlyph,
		SpinRightGlyph,
		LeftButtonMargin,
		RightButtonMargin,
		ButtonMargin,
		ButtonMarginCorrection,
		LeftButtonMarginCorrection,
		RightButtonMarginCorrection,
		ButtonTemplate,
		[BlendVisibility(false)]
		CustomButtonInfoTemplate,
		[BlendVisibility(false)]
		ButtonInfoTemplate,
		[BlendVisibility(false)]
		RepeatButtonInfoTemplate,
		[BlendVisibility(false)]
		ToggleButtonInfoTemplate,
		[BlendVisibility(false)]
		DefaultToggleButtonInfoTemplate,
		[BlendVisibility(false)]
		ButtonStyle,
		[BlendVisibility(false)]
		ButtonContainerStyle,
		ButtonInfoContentStyle,
		UserButtonInfoContentStyle,
		NoneButtonInfoContentStyle,
		SpinUpButtonTemplate,
		SpinDownButtonTemplate,
		SpinLeftButtonTemplate,
		SpinRightButtonTemplate,
		SpinButtonInfoVerticalStyle,
		SpinButtonInfoHorizontalStyle,
		SpinButtonInfoHorizontalTemplate,
		SpinButtonInfoVerticalTemplate,
		Foreground
	}
	public enum ColorChooserThemeKeys {
		ControlTemplate,
		BorderBrush,
		BackgroundBrush
	}
	public enum ExpressionEditorControlThemeKeys {
		Template,
		AndGlyph,
		DivideGlyph,
		EqualGlyph,
		LargerGlyph,
		LargerOrEqualGlyph,
		LessGlyph,
		LessOrEqualGlyph,
		MinusGlyph,
		ModuloGlyph,
		MultiplyGlyph,
		NotGlyph,
		NotEqualGlyph,
		OrGlyph,
		PlusGlyph,
		WrapSelectionGlyph
	}
	public enum FilterControlThemeKeys {
		FilterControlTemplate,
		ItemsControlItemTemplate,
		ClauseNodeTemplate,
		GroupNodeTemplate,
		SomeValuesTemplate,
		SeveralElementsItemPanelTemplate,
		FilterControlGroupTypeButtonTemplate,
		FilterControlFirstOperandButtonTemplate,
		FilterControlClauseOperationButtonTemplate,
		FilterControlGroupNodeTemplate,
		FilterControlClauseNodeTemplate,
		OneElementsInSecondOperandTemplate,
		TwoElementsInSecondOperandTemplate,
		SeveralElementsInSecondOperandTemplate,
		OneLocalDateTimeTemplate,
		FilterControlEditorTemplate,
		ValueTemplate,
		EmptyValueTemplate,
		EmptyStringTemplate,
		BooleanValueTemplate,
		FieldInOperationButtonTemplate,
		FilterControlBorderTemplate,
		AddButtonTemplate,
		GroupCommandsButtonTemplate,
		DeleteNodeButtonTemplate,
		ChangeOperandTypeButtonTemplate,
		FocusVisualStyle,
		InactiveTextBlockStyle,
		InplaceFilterEditorForeground
	}
	public enum FilterPanelControlBaseThemeKeys {
		ClearFilterButtonTemplate,
		FilterControlButtonTemplate,
		FilterPanelBorderStyle,
		FilterPanelEnableFilterStyle,
		FilterPanelTextStyle,
		MRUComboBoxTemplate,
		FilterPanelContentStyle
	}
	public enum ListBoxEditThemeKeys {
		ListBoxStyle,
		ListBoxInplaceStyle,
		DisplayTemplate,
		EditTemplate,
		Template,
		BorderTemplate,
		EmptyBorderTemplate,
		Background
	}
	public enum ColorEditThemeKeys {
		Template,
		EditTemplate,
		EditInplaceTemplate,
		BorderTemplate,
		EmptyBorderTemplate,
		ChipBorderBrush,
		GalleryItemControlTemplate,
		Background,
		GalleryBarItemLinkControlTemplate
	}
	public enum PopupColorEditThemeKeys {
		PopupContentTemplate,
		EditTemplate,
		EditNonEditableTemplate,
		DisplayTemplate,
		EditInplaceTemplate,
		EditNonEditableInplaceTemplate,
		DisplayInplaceTemplate,
		PART_EditorStyle,
	}
	public enum CalcEditThemeKeys {
		PopupContentTemplate
	}
	public enum CalculatorThemeKeys {
		BorderTemplate,
		ButtonCEStyle,
		ButtonDigitStyle,
		ButtonOperationStyle,
		ButtonPanelTemplate,
		DisplayContentTemplate,
		DisplayTemplate,
		DisplayStyle,
		GaugeSegmentTemplate,
		GaugeTemplate,
		MemoryIndicatorTemplate,
		PopupContentContainerTemplate,
		PopupTemplate,
		Template
	}
	public enum ImageEditThemeKeys {
		Template,
		EditTemplate,
		EditInplaceTemplate,
		BorderTemplate,
		EmptyBorderTemplate,
		ImageBackgroundBrush,
		EmptyContentTemplate,
		MenuTemplate,
		MenuPopupTemplate,
		ToolButtonTemplate
	}
	public enum PopupImageEditThemeKeys {
		PopupContentTemplate,
		PopupContentContainerTemplate,
		SizeGripStyle,
		EditTemplate,
		EditNonEditableTemplate,
		DisplayTemplate,
		EditInplaceTemplate,
		EditNonEditableInplaceTemplate,
		DisplayInplaceTemplate,
		MenuTemplate,
	}
	public enum CustomItemThemeKeys {
		DefaultTemplate,
		EmptyItemTemplate,
		DefaultButtonStyle,
		ItemContainerStyle,
		SelectAllItemContainerStyle,
		WaitIndicatorItemContainerStyle,
		SeparatorTemplate,
	}
	public enum SpinEditThemeKeys {
		ArrowBrushColor
	}
	public enum DateNavigatorCalendarThemeKeys {
		Template
	}
	public enum FontEditThemeKeys {
		ItemTemplate
	}
	public enum SearchControlThemeKeys {
		Template,
		FindButtonStyle,
		CloseButtonStyle,
		PanelStyle,
		PanelTemplate,
		ClearButtonTemplate,
		StandaloneStyle,
	}
	public enum DisplayFormatTextControlThemeKeys {
		Template,
		ComboBoxStyle,
	}
	public enum DataPagerThemeKeys {
		Template,
		ButtonAreaBorderTemplate,
		NumericButtonAreaBorderTemplate,
		FixedNumericButtonCountButtonContainerTemplate,
		AutoNumericButtonCountButtonContainerTemplate
	}
	public enum DataPagerNumericButtonContainerThemeKeys {
		Template
	}
	public enum DataPagerButtonThemeKeys {
		Template,
		FirstPageGlyph,
		LastPageGlyph,
		NextPageGlyph,
		PrevPageGlyph
	}
	public enum DataPagerNumericButtonThemeKeys {
		Template
	}
	public enum DateTimePickerThemeKeys {
		Template,
		SelectorTemplate,
		ItemTemplate,
		ItemsControlTemplate,
		YearItemTemplate,
		MonthItemTemplate,
		DayItemTemplate,
		Hour24ItemTemplate,
		Hour12ItemTemplate,
		MinuteItemTemplate,
		SecondItemTemplate,
		MillisecondItemTemplate,
		AmPmItemTemplate,
		NoneItemTemplate,
		DateTimePickerItemTemplate
	}
	public enum RangeControlThemeKeys {
		SimpleStyle,
		SelectionThumbBaseStyle,
		LeftSelectionThumbTemplate,
		RightSelectionThumbTemplate,
		OutOfRangeAreaBorderStyle,
		SelectionRectangleStyle,
		LeftSelectionThumbStyle,
		RightSelectionThumbStyle,
		RangeBarStyle,
		RangeBarTemplate,
		RangeBarSliderStyle,
		RangeBarSliderTemplate,
		RangeBarSelectionAreaStyle,
		RangeBarResizeThumbStyle,
		RangeBarOutOfRangeBorderStyle,
		RangeBarBottomBorderStyle,
		RangeBarRootBorderTemplate,
		RangeBarOutOfSelectionBorderStyle,
		RangeBarSelectionBorderStyle,
		RangeControlTemplate,
		RangeBarLeftResizeThumbTemplate,
		RangeBarRightResizeThumbTemplate,
		BorderTemplate,
		LabelStyle,
		RightNavigationButtonStyle,
		LeftNavigationButtonStyle,
		LeftNavigationButtonTemplate,
		RightNavigationButtonTemplate,
		LabelTemplate,
		LeftLabelMargin,
		RightLabelMargin
	}
	public enum RangeControlBrushesThemeKeys {
		RangeThumbsBrush,
		RangeThumbsHighlightBrush,
		RangeBarSelectionBrush
	}
	public enum CalendarClientThemeKeys {
		CalendarItemTemplate,
		CalendarGroupItemTemplate,
		CalendarClientTemplate,
		CalendarClientForegroundBrush,
		CalendarSelectionMarkerTemplate
	}
	public enum ColorPickerThemeKeys {
		Template,
		HSBDataTemplate,
		HLSDataTemplate,
		CMYKDataTemplate,
		RGBDataTemplate,
		PipetButtonTemplate,
	}
	public enum BrushEditThemeKeys {
		Template,
		EmptyBorderTemplate,
		BorderTemplate,
		Background,
		DisplayTemplate,
		EditTemplate,
		BrushTypeSelectorControlTemplate,
		BrushPickerNoneTemplate,
		BrushPickerSolidTemplate,
		BrushPickerGradientTemplate,
	}
	public enum PopupBrushEditThemeKeys {
		BorderTemplate,
		ChipBorderBrush,
		DisplayTemplate,
		DisplayInplaceTemplate,
		PopupContentTemplate,
		SolidColorBrushPopupContentTemplate,
		LinearGradientBrushPopupContentTemplate,
		TextBlockStyle,
		TextBlockInplaceStyle,
	}
	public enum BarCodeEditThemeKeys {
		Background,
		Template,
		DisplayTemplate,
		EditTemplate,
		EmptyBorderTemplate,
		BorderTemplate,
	}
	public enum SparklineEditThemeKeys {
		Background,
		Template,
		EmptyBorderTemplate,
		BorderTemplate,
		LineSparklineStyle,
		BarSparklineStyle,
		AreaSparklineStyle,
		WinLossSparklineStyle,
		LineDisplayTemplate,
		BarDisplayTemplate,
		AreaDisplayTemplate,
		WinLossDisplayTemplate,
		Brush,
		MaxPointBrush,
		MinPointBrush,
		StartPointBrush,
		EndPointBrush,
		NegativePointBrush,
		MarkerBrush,
		MinHeight
	}
	public enum GradientMultiSliderThemeKeys {
		Template,
		ThumbTemplate,
		ThumbSelectorTemplate,
		FlipThumbsButtonStyle,
		FlipThumbsButtonTemplate,
		NextThumbButtonStyle,
		NextThumbButtonTemplate,
		PreviousThumbButtonStyle,
		PreviousThumbButtonTemplate,
		SelectedThumbContentPresenterStyle,
		SelectedThumbContentPresenterContentTemplate
	}
	public enum InplaceButtonThemeKey {
		RenderInfo,
	}
	public class CameraControlThemeKeyExtension : ThemeKeyExtensionBase<CameraControlsThemeKeys> { }
	public class TokenEditorThemeKeyExtension : ThemeKeyExtensionBase<TokenEditorThemeKeys> { }
	public class RangeControlBrushesThemeKeyExtension : ThemeKeyExtensionBase<RangeControlBrushesThemeKeys> { }
	public class RangeControlThemeKeyExtension : ThemeKeyExtensionBase<RangeControlThemeKeys> { }
	public class CalendarClientThemeKeyExtension : ThemeKeyExtensionBase<CalendarClientThemeKeys> { }
	public class PasswordBoxEditThemeKeyExtension : ThemeKeyExtensionBase<PasswordBoxEditThemeKeys> {
	}
	public class ProgressBarEditStyleThemeKeyExtension : ThemeKeyExtensionBase<string> {
	}
	public class TrackBarEditStyleThemeKeyExtension : ThemeKeyExtensionBase<string> {
	}
	public class TrackBarEditThemeKeyExtension : ThemeKeyExtensionBase<TrackBarEditThemeKeys> {
	}
	public class TrackBarEditThumbThemeKeyExtension : ThemeKeyExtensionBase<TrackBarEditThumbThemeKeys> {
	}
	public class RangeEditThemeKeyExtension : ThemeKeyExtensionBase<RangeEditThemeKeys> {
	}
	public class FlyoutControlThemeKeyExtension : ThemeKeyExtensionBase<FlyoutControlThemeKeys> {
	}
	public class ProgressBarEditThemeKeyExtension : ThemeKeyExtensionBase<ProgressBarEditThemeKeys> {
	}
	public class InplaceBaseEditThemeKeyExtension : ThemeKeyExtensionBase<InplaceBaseEditThemeKeys> {
	}
	public class TextEditThemeKeyExtension : ThemeKeyExtensionBase<TextEditThemeKeys> {
	}
	public class CheckEditThemeKeyExtension : ThemeKeyExtensionBase<CheckEditThemeKeys> {
	}
	public class ButtonEditThemeKeyExtension : ThemeKeyExtensionBase<ButtonEditThemeKeys> {
	}
	public class PopupBaseEditThemeKeyExtension : ThemeKeyExtensionBase<PopupBaseEditThemeKeys> {
	}
	public class DateEditThemeKeyExtension : ThemeKeyExtensionBase<DateEditThemeKeys> {
	}
	public class DateNavigatorThemeKeyExtension : ThemeKeyExtensionBase<DateNavigatorThemeKeys> {
	}
	public class DateNavigatorContentThemeKeyExtension : ThemeKeyExtensionBase<DateNavigatorContentThemeKeys> {
	}
	public class ComboBoxEditThemeKeyExtension : ThemeKeyExtensionBase<ComboBoxThemeKeys> {
	}
	public class CustomItemThemeKeyExtension : ThemeKeyExtensionBase<CustomItemThemeKeys> {
	}
	public class MemoEditThemeKeyExtension : ThemeKeyExtensionBase<MemoEditThemeKeys> {
	}
	public class EditorListBoxThemeKeyExtension : ThemeKeyExtensionBase<EditorListBoxThemeKeys> {
	}
	public class ButtonsThemeKeyExtension : ThemeKeyExtensionBase<ButtonsThemeKeys> {
	}
	public class ErrorTypesThemeKeyExtension : ThemeKeyExtensionBase<ErrorType> {
	}
	public class ButtonEditButtonsThemeKeyExtension : ThemeKeyExtensionBase<GlyphKind> {
	}
	public class ColorChooserThemeKeyExtension : ThemeKeyExtensionBase<ColorChooserThemeKeys> {
	}
	public class ExpressionEditorControlThemeKeyExtension : ThemeKeyExtensionBase<ExpressionEditorControlThemeKeys> {
	}
	public class FilterControlThemeKeyExtension : ThemeKeyExtensionBase<FilterControlThemeKeys> {
	}
	public class FilterPanelControlBaseThemeKeyExtension : ThemeKeyExtensionBase<FilterPanelControlBaseThemeKeys> {
	}
	public class ListBoxEditThemeKeyExtension : ThemeKeyExtensionBase<ListBoxEditThemeKeys> {
	}
	public class ColorEditThemeKeyExtension : ThemeKeyExtensionBase<ColorEditThemeKeys> {
	}
	public class PopupColorEditThemeKeyExtension : ThemeKeyExtensionBase<PopupColorEditThemeKeys> {
	}
	public class CalcEditThemeKeyExtension : ThemeKeyExtensionBase<CalcEditThemeKeys> {
	}
	public class CalculatorThemeKeyExtension : ThemeKeyExtensionBase<CalculatorThemeKeys> {
	}
	public class ImageEditThemeKeyExtension : ThemeKeyExtensionBase<ImageEditThemeKeys> {
	}
	public class PopupImageEditThemeKeyExtension : ThemeKeyExtensionBase<PopupImageEditThemeKeys> {
	}
	public class SpinEditThemeKeyExtension : ThemeKeyExtensionBase<SpinEditThemeKeys> {
	}
	public class DateNavigatorCalendarThemeKeyExtension : ThemeKeyExtensionBase<DateNavigatorCalendarThemeKeys> {
	}
	public class FontEditThemeKeyExtension : ThemeKeyExtensionBase<FontEditThemeKeys> {
	}
	public class DataPagerThemeKeyExtension : ThemeKeyExtensionBase<DataPagerThemeKeys> {
	}
	public class DataPagerNumericButtonContainerThemeKeyExtension : ThemeKeyExtensionBase<DataPagerNumericButtonContainerThemeKeys> {
	}
	public class DataPagerButtonThemeKeyExtension : ThemeKeyExtensionBase<DataPagerButtonThemeKeys> {
	}
	public class DataPagerNumericButtonThemeKeyExtension : ThemeKeyExtensionBase<DataPagerNumericButtonThemeKeys> {
	}
	public class SearchControlThemeKeyExtension : ThemeKeyExtensionBase<SearchControlThemeKeys> {
	}
	public class DisplayFormatTextControlThemeKeyExtension : ThemeKeyExtensionBase<DisplayFormatTextControlThemeKeys> {
	}
	public class DateTimePickerThemeKeyExtension : ThemeKeyExtensionBase<DateTimePickerThemeKeys> {
	}
	public class ColorPickerThemeKeyExtension : ThemeKeyExtensionBase<ColorPickerThemeKeys> {
	}
	public class SparklineEditThemeKeyExtension : ThemeKeyExtensionBase<SparklineEditThemeKeys> {
	}
	public class BarCodeEditThemeKeyExtension : ThemeKeyExtensionBase<BarCodeEditThemeKeys> {
	}
	public class BrushEditThemeKeyExtension : ThemeKeyExtensionBase<BrushEditThemeKeys> {
	}
	public class PopupBrushEditThemeKeyExtension : ThemeKeyExtensionBase<PopupBrushEditThemeKeys> {
	}
	public class GradientMultiSliderThemeKeyExtension : ThemeKeyExtensionBase<GradientMultiSliderThemeKeys> {   
	}
	public class InplaceButtonThemeKeyExtension : ThemeKeyExtensionBase<InplaceButtonThemeKey> {
	}
}
