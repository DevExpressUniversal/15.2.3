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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Themes;
namespace DevExpress.Xpf.Editors.Internal {
	public class InplaceResourceProvider {
		RenderTemplate textEditInplaceInactiveTemplate;		
		RenderTemplate textEditInplaceActiveTemplate;
		RenderTemplateSelector textEditTemplateSelector;
		Size? checkBoxRenderSize;
		RenderTemplate applyGlyphKindTemplate;
		RenderTemplate cancelGlyphKindTemplate;
		RenderTemplate dropDownGlyphTemplate;
		RenderTemplate regularGlyphTemplate;
		RenderTemplate criticalErrorTemplate;
		RenderTemplate validationErrorTemplate;
		RenderTemplate informationErrorTemplate;
		RenderTemplate warningErrorTemplate;
		RenderTemplate renderCheckBoxTemplate;
		RenderTemplate checkEditInplaceInactiveTemplate;
		RenderTemplate borderTemplate;
		RenderTemplate rightGlyphTemplate;
		RenderTemplate leftGlyphTemplate;
		RenderTemplate upGlyphTemplate;
		RenderTemplate downGlyphTemplate;
		RenderTemplate rightSpinGlyphTemplate;
		RenderTemplate leftSpinGlyphTemplate;
		RenderTemplate upSpinGlyphTemplate;
		RenderTemplate downSpinGlyphTemplate;
		RenderTemplate plusGlyphTemplate;
		RenderTemplate minusGlyphTemplate;
		RenderTemplate redoGlyphTemplate;
		RenderTemplate undoGlyphTemplate;
		RenderTemplate refreshGlyphTemplate;
		RenderTemplate noneGlyphTemplate;
		RenderTemplate searchGlyphTemplate;
		RenderTemplate nextPageGlyphTemplate;
		RenderTemplate prevPageGlyphTemplate;
		RenderTemplate lastGlyphTemplate;
		RenderTemplate firstGlyphTemplate;
		RenderTemplate editGlyphTemplate;
		RenderTemplate commonBaseEditInplaceInactiveTemplate;
		ControlTemplate selectedItemTemplate;
		ControlTemplate selectedItemImageTemplate;
		ControlTemplate tokenEditorDisplayTemplate;
		RenderTemplate commonBaseEditInplaceInactiveTemplateWithDisplayTemplate;
		RenderTemplate renderButtonTemplate;
		RenderTemplate renderSpinButtonTemplate;
		RenderTemplate renderSpinUpButtonTemplate;
		RenderTemplate renderSpinLeftButtonTemplate;
		RenderTemplate renderSpinRightButtonTemplate;
		RenderTemplate renderSpinDownButtonTemplate;		
		RenderTemplate realContentPresenterTemplate;
		RenderTemplate contentPresenterTemplate;
		RenderTemplate textContentPresenterTemplate;
		Thickness? rightButtonMargin;
		string themeName;
		public InplaceResourceProvider(string themeName) {
			this.themeName = themeName;
		}
		T GetResource<T>(FrameworkElement element, ref T field, InplaceBaseEditThemeKeys key) where T : class {
			return field ?? (field = (T)element.FindResource(new InplaceBaseEditThemeKeyExtension() {
				ResourceKey = key,
				ThemeName = themeName,
			}));
		}		
		public Size GetCheckBoxRenderSize(FrameworkElement element) {
			if (checkBoxRenderSize == null) {
				checkBoxRenderSize = (Size)element.FindResource(new CheckEditThemeKeyExtension() {
					ResourceKey = CheckEditThemeKeys.CheckSize,
					ThemeName = themeName
				});
			}
			return checkBoxRenderSize.Value;
		}
		public Thickness GetRightButtonMargin(FrameworkElement element) {
			if (rightButtonMargin == null) {
				rightButtonMargin = (Thickness)element.FindResource(new ButtonsThemeKeyExtension() {
					ResourceKey = ButtonsThemeKeys.RightButtonMargin,
					ThemeName = themeName
				});
			}
			return rightButtonMargin.Value;
		}
		public RenderTemplate GetContentPresenterTemplate(FrameworkElement element) {
			return GetResource(element, ref contentPresenterTemplate, InplaceBaseEditThemeKeys.ContentPresenterTemplate);
		}
		public RenderTemplate GetTextContentPresenterTemplate(FrameworkElement element) {
			return GetResource(element, ref textContentPresenterTemplate, InplaceBaseEditThemeKeys.TextContentPresenterTemplate);
		}
		public RenderTemplate GetRealContentPresenterTemplate(FrameworkElement element) {
			return GetResource(element, ref realContentPresenterTemplate, InplaceBaseEditThemeKeys.RealContentPresenterTemplate);
		}
		public RenderTemplate GetTextEditInplaceInactiveTemplate(FrameworkElement element) {
			return GetResource(element, ref textEditInplaceInactiveTemplate, InplaceBaseEditThemeKeys.TextEditInplaceInactiveTemplate);			
		}		
		public RenderTemplate GetCommonBaseEditInplaceInactiveTemplateWithDisplayTemplate(FrameworkElement element) {
			return GetResource(element, ref commonBaseEditInplaceInactiveTemplateWithDisplayTemplate, InplaceBaseEditThemeKeys.CommonBaseEditInplaceInactiveTemplateWithDisplayTemplate);			
		}		
		public RenderTemplate GetTextEditInplaceActiveTemplate(FrameworkElement element) {
			return GetResource(element, ref textEditInplaceActiveTemplate, InplaceBaseEditThemeKeys.TextEditInplaceActiveTemplate);
		}
		public RenderTemplateSelector GetTextEditTemplateSelector(FrameworkElement element) {
			return GetResource(element, ref textEditTemplateSelector, InplaceBaseEditThemeKeys.TextEditInplaceTemplateSelector);			
		}				
		public RenderTemplate GetCriticalErrorTemplate(FrameworkElement element) {
			return GetResource(element, ref criticalErrorTemplate, InplaceBaseEditThemeKeys.CriticalErrorTemplate);			
		}
		public RenderTemplate GetValidationErrorTemplate(FrameworkElement element) {
			return GetResource(element, ref validationErrorTemplate, InplaceBaseEditThemeKeys.ValidationErrorTemplate);			
		}
		public RenderTemplate GetInformationErrorTemplate(FrameworkElement element) {
			return GetResource(element, ref informationErrorTemplate, InplaceBaseEditThemeKeys.InformationErrorTemplate);			
		}
		public RenderTemplate GetWarningErrorTemplate(FrameworkElement element) {
			return GetResource(element, ref warningErrorTemplate, InplaceBaseEditThemeKeys.WarningErrorTemplate);			
		}
		public RenderTemplate GetRenderCheckBoxTemplate(FrameworkElement element) {
			return GetResource(element, ref renderCheckBoxTemplate, InplaceBaseEditThemeKeys.RenderCheckBoxTemplate);			
		}		
		public RenderTemplate GetRenderButtonTemplate(FrameworkElement element) {
			return GetResource(element, ref renderButtonTemplate, InplaceBaseEditThemeKeys.RenderButtonTemplate);	 
		}
		public RenderTemplate GetRenderSpinButtonTemplate(FrameworkElement element) {
			return GetResource(element, ref renderSpinButtonTemplate, InplaceBaseEditThemeKeys.RenderSpinButtonTemplate);
		}
		public RenderTemplate GetRenderSpinUpButtonTemplate(FrameworkElement element) {
			return GetResource(element, ref renderSpinUpButtonTemplate, InplaceBaseEditThemeKeys.RenderSpinUpButtonTemplate);
		}
		public RenderTemplate GetRenderSpinLeftButtonTemplate(FrameworkElement element) {
			return GetResource(element, ref renderSpinLeftButtonTemplate, InplaceBaseEditThemeKeys.RenderSpinLeftButtonTemplate);
		}
		public RenderTemplate GetRenderSpinRightButtonTemplate(FrameworkElement element) {
			return GetResource(element, ref renderSpinRightButtonTemplate, InplaceBaseEditThemeKeys.RenderSpinRightButtonTemplate);
		}
		public RenderTemplate GetRenderSpinDownButtonTemplate(FrameworkElement element) {
			return GetResource(element, ref renderSpinDownButtonTemplate, InplaceBaseEditThemeKeys.RenderSpinDownButtonTemplate);
		}
		public RenderTemplate GetCheckEditInplaceInactiveTemplate(FrameworkElement element) {
			return GetResource(element, ref checkEditInplaceInactiveTemplate, InplaceBaseEditThemeKeys.CheckEditInplaceInactiveTemplate);			
		}		
		public RenderTemplate GetBorderTemplate(FrameworkElement element) {
			return GetResource(element, ref borderTemplate, InplaceBaseEditThemeKeys.BorderTemplate);			
		}
		public RenderTemplate GetButtonInfoApplyGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref applyGlyphKindTemplate, InplaceBaseEditThemeKeys.ApplyGlyph);
		}
		public RenderTemplate GetButtonInfoCancelGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref cancelGlyphKindTemplate, InplaceBaseEditThemeKeys.CancelGlyph);
		}
		public RenderTemplate GetButtonInfoDropDownGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref dropDownGlyphTemplate, InplaceBaseEditThemeKeys.DropDownGlyph);
		}
		public RenderTemplate GetButtonInfoRegularGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref regularGlyphTemplate, InplaceBaseEditThemeKeys.RegularGlyph);
		}				
		internal RenderTemplate GetButtonInfoRightGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref rightGlyphTemplate, InplaceBaseEditThemeKeys.RightGlyph);
		}		
		internal RenderTemplate GetButtonInfoLeftGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref leftGlyphTemplate, InplaceBaseEditThemeKeys.LeftGlyph);
		}		
		internal RenderTemplate GetButtonInfoUpGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref upGlyphTemplate, InplaceBaseEditThemeKeys.UpGlyph);
		}		
		internal RenderTemplate GetButtonInfoDownGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref downGlyphTemplate, InplaceBaseEditThemeKeys.DownGlyph);
		}
		internal RenderTemplate GetButtonInfoSpinRightGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref rightSpinGlyphTemplate, InplaceBaseEditThemeKeys.SpinRightGlyph);
		}
		internal RenderTemplate GetButtonInfoSpinLeftGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref leftSpinGlyphTemplate, InplaceBaseEditThemeKeys.SpinLeftGlyph);
		}
		internal RenderTemplate GetButtonInfoSpinUpGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref upSpinGlyphTemplate, InplaceBaseEditThemeKeys.SpinUpGlyph);
		}
		internal RenderTemplate GetButtonInfoSpinDownGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref downSpinGlyphTemplate, InplaceBaseEditThemeKeys.SpinDownGlyph);
		}		
		internal RenderTemplate GetButtonInfoPlusGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref plusGlyphTemplate, InplaceBaseEditThemeKeys.PlusGlyph);
		}		
		internal RenderTemplate GetButtonInfoMinusGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref minusGlyphTemplate, InplaceBaseEditThemeKeys.MinusGlyph);
		}		
		internal RenderTemplate GetButtonInfoRedoGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref redoGlyphTemplate, InplaceBaseEditThemeKeys.RedoGlyph);
		}		
		internal RenderTemplate GetButtonInfoUndoGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref undoGlyphTemplate, InplaceBaseEditThemeKeys.UndoGlyph);
		}
		internal RenderTemplate GetButtonInfoRefreshGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref refreshGlyphTemplate, InplaceBaseEditThemeKeys.RefreshGlyph);
		}
		internal RenderTemplate GetButtonInfoNoneGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref noneGlyphTemplate, InplaceBaseEditThemeKeys.NoneGlyph);
		}		
		internal RenderTemplate GetButtonInfoSearchGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref searchGlyphTemplate, InplaceBaseEditThemeKeys.SearchGlyph);
		}		
		internal RenderTemplate GetButtonInfoNextPageGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref nextPageGlyphTemplate, InplaceBaseEditThemeKeys.NextPageGlyph);
		}
		internal RenderTemplate GetButtonInfoPrevPageGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref prevPageGlyphTemplate, InplaceBaseEditThemeKeys.PrevPageGlyph);
		}		
		internal RenderTemplate GetButtonInfoLastGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref lastGlyphTemplate, InplaceBaseEditThemeKeys.LastGlyph);
		}		
		internal RenderTemplate GetButtonInfoFirstGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref firstGlyphTemplate, InplaceBaseEditThemeKeys.FirstGlyph);
		}		
		internal RenderTemplate GetButtonInfoEditGlyphKindTemplate(FrameworkElement element) {
			return GetResource(element, ref editGlyphTemplate, InplaceBaseEditThemeKeys.EditGlyph);
		}
		public RenderTemplate GetCommonBaseEditInplaceInactiveTemplate(FrameworkElement element) {
			return GetResource(element, ref commonBaseEditInplaceInactiveTemplate, InplaceBaseEditThemeKeys.CommonBaseEditInplaceInactiveTemplate);
		}
		public ControlTemplate GetSelectedItemTemplate(FrameworkElement element) {
			return GetResource(element, ref selectedItemTemplate, InplaceBaseEditThemeKeys.SelectedItemTemplate);
		}
		public ControlTemplate GetSelectedItemImageTemplate(FrameworkElement element) {
			return GetResource(element, ref selectedItemImageTemplate, InplaceBaseEditThemeKeys.SelectedItemImageTemplate);
		}
		public ControlTemplate GetTokenEditorDisplayTemplate(FrameworkElement element) {
			return GetResource(element, ref tokenEditorDisplayTemplate, InplaceBaseEditThemeKeys.AutoCompleteBoxTemplate);
		}
	}
}
