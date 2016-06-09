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
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Charts.Designer.Native {
	public sealed class BarItemTemplateSelector : DataTemplateSelector {
		ResourceDictionary itemsTemplateDictionary;
		ResourceDictionary editItemsTemplateDictionary;
		ResourceDictionary galleriesTemplateDictionary;
		public BarItemTemplateSelector() {
			string asm = (typeof(ChartDesigner)).Assembly.GetName().Name;
			Uri uri = new Uri("pack://application:,,,/" + asm + ";component/Themes/RibbonElementTemplates/BarItems.xaml", UriKind.Absolute);
			this.itemsTemplateDictionary = new ResourceDictionary() { Source = uri };
			Uri editItemuri = new Uri("pack://application:,,,/" + asm + ";component/Themes/RibbonElementTemplates/BarEditItems.xaml", UriKind.Absolute);
			this.editItemsTemplateDictionary = new ResourceDictionary() { Source = editItemuri };
			Uri galleryItemuri = new Uri("pack://application:,,,/" + asm + ";component/Themes/RibbonElementTemplates/Galleries.xaml", UriKind.Absolute);
			this.galleriesTemplateDictionary = new ResourceDictionary() { Source = galleryItemuri };
		}
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			Type itemType = item.GetType();
			if (itemType == typeof(BarSplitButtonGalleryViewModel))
				return (DataTemplate)this.galleriesTemplateDictionary["BarSplitButtonGalleryViewModelTemplate"];
			else if (itemType == typeof(InRibbonGalleryViewModel))
				return (DataTemplate)this.galleriesTemplateDictionary["InRibbonGalleryViewModelTemplate"];
			else if (itemType == typeof(BarDropDownButtonGalleryViewModel))
				return (DataTemplate)this.galleriesTemplateDictionary["BarDropDownButtonGalleryViewModelTemplate"];
			else if (itemType == typeof(BarDropDownButtonGalleryWithComboViewModel))
				return (DataTemplate)this.galleriesTemplateDictionary["BarDropDownButtonGalleryWithComboViewModelTemplate"];
			else if (itemType == typeof(BarButtonViewModel))
				return (DataTemplate)this.itemsTemplateDictionary["BarButtonViewModelTemplate"];
			else if (itemType == typeof(BarCheckButtonViewModel))
				return (DataTemplate)this.itemsTemplateDictionary["BarCheckButtonViewModelViewModelTemplate"];
			else if (itemType == typeof(BarEditValueItemViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["TextEdit"];
			else if (itemType == typeof(BarCheckEditViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["BarCheckEditViewModelTemplate"];
			else if (itemType == typeof(BarFontFamilyEditViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["BarFontFamilyEditViewModelTemplate"];
			else if (itemType == typeof(BarComboBoxViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["BarComboBoxViewModelTemplate"];
			else if (itemType == typeof(BarDynamicComboBoxViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["BarDynamicComboBoxViewModelTemplate"];
			else if (itemType == typeof(BarSeparatorViewModel))
				return (DataTemplate)this.itemsTemplateDictionary["BarItemSeparatorViewModelTemplate"];
			else if (itemType == typeof(BarStaticTextViewModel))
				return (DataTemplate)this.itemsTemplateDictionary["BarStaticTextViewModelTemplate"];
			else if (itemType == typeof(BarColorEditViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["BarColorEditViewModelTemplate"];
			else if (itemType == typeof(BarThicknessEditViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["BarThicknessEditViewModelTemplate"];
			else if (itemType == typeof(BarSpinEditViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["BarSpinEditViewModelTemplate"];
			else if (itemType == typeof(BarDateTimeEditViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["BarDateTimeEditViewModelTemplate"];
			else if (itemType == typeof(BarPopupDataBrowserEditViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["BarPopupDataBrowserEditViewModelTemplate"];
			else if (itemType == typeof(BarDoubleEditViewModel))
				return (DataTemplate)this.editItemsTemplateDictionary["BarDoubleEditViewModelTemplate"];
			else
				throw new ChartDesignerException("BarItemTemplateSelector can't give a DataTemplate for item: " + item);
		}
	}
}
