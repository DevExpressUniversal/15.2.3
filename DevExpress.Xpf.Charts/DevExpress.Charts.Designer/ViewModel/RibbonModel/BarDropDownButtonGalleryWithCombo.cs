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

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
namespace DevExpress.Charts.Designer.Native {
	public class BarDropDownButtonGalleryWithComboViewModel : DropDownGalleryViewModelBase {
		public static readonly DependencyProperty selectedComboBoxItemProperty =
			DependencyProperty.Register("SelectedComboBoxItem", typeof(object), typeof(BarDropDownButtonGalleryWithComboViewModel), new PropertyMetadata(OnSelectedComboBoxItemChanged));
		public static readonly DependencyProperty comboBoxItemsProperty =
			DependencyProperty.Register("ComboBoxItems", typeof(object[]), typeof(BarDropDownButtonGalleryWithComboViewModel), new PropertyMetadata(OnComboBoxItemsChanged));
		static void OnComboBoxItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var gallery = (BarDropDownButtonGalleryWithComboViewModel)d;
			if (gallery.ComboBoxItems != null && gallery.ComboBoxItems.Length > 0)
				gallery.SelectedComboBoxItem = gallery.ComboBoxItems[0];
		}
		static void OnSelectedComboBoxItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var gallery = (BarDropDownButtonGalleryWithComboViewModel)d;
			if (e.NewValue == null  && gallery.ComboBoxItems != null && gallery.ComboBoxItems.Length > 0)
				gallery.SelectedComboBoxItem = gallery.ComboBoxItems[0];
		}
		ImageSource glyph;
		ImageSource largeGlyph;
		bool allowToolTips = true;
		RibbonItemStyles ribbonStyle = RibbonItemStyles.Large | RibbonItemStyles.SmallWithText;
		IValueConverter selectedComboBoxItemToGalleryItemCommandParameterConverter = null;
		public object[] ComboBoxItems {
			get { return (object[])GetValue(comboBoxItemsProperty); }
			set { SetValue(comboBoxItemsProperty, value); }
		}
		public object SelectedComboBoxItem {
			get { return GetValue(selectedComboBoxItemProperty); }
			set { SetValue(selectedComboBoxItemProperty, value); }
		}
		public ImageSource Glyph {
			get { return glyph; }
			private set {
				if (glyph != value) {
					glyph = value;
					OnPropertyChanged("Glyph");
				}
			}
		}
		public ImageSource LargeGlyph {
			get { return largeGlyph; }
			private set {
				if (largeGlyph != value) {
					largeGlyph = value;
					OnPropertyChanged("LargeGlyph");
				}
			}
		}
		public string ImageName {
			set {
				if (!string.IsNullOrEmpty(value)) {
					Glyph = GlyphUtils.GetGlyphByPath(value);
					LargeGlyph = GlyphUtils.GetLargeGlyphByPath(value);
				}
			}
		}
		public bool AllowToolTips {
			get { return allowToolTips; }
			set {
				if (allowToolTips != value) {
					allowToolTips = value;
					OnPropertyChanged("AllowToolTips");
				}
			}
		}
		public RibbonItemStyles RibbonStyle {
			get { return ribbonStyle; }
			set {
				if (ribbonStyle != value) {
					ribbonStyle = value;
					OnPropertyChanged("RibbonStyle");
				}
			}
		}
		public IValueConverter SelectedComboBoxItemToGalleryItemCommandParameterConverter {
			get { return selectedComboBoxItemToGalleryItemCommandParameterConverter; }
			set {
				if (selectedComboBoxItemToGalleryItemCommandParameterConverter != value) {
					selectedComboBoxItemToGalleryItemCommandParameterConverter = value;
					OnPropertyChanged("SelectedComboBoxItemToGalleryItemCommandParameterConverter");
				}
			}
		}
		public BarDropDownButtonGalleryWithComboViewModel(object coimboBoxItemsBindingSource, string coimboBoxItemsBindingPath, IValueConverter coimboBoxItemsBindingConverter = null) {
			Binding binding = new Binding(coimboBoxItemsBindingPath);
			binding.Source = coimboBoxItemsBindingSource;
			binding.Converter = coimboBoxItemsBindingConverter;
			BindingOperations.SetBinding(this, BarDropDownButtonGalleryWithComboViewModel.comboBoxItemsProperty, binding);
			Groups.CollectionChanged += Groups_CollectionChanged;
		}
		void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.NewItems != null)
				foreach (GalleryGroupViewModel groupModel in e.NewItems) {
					groupModel.Items.CollectionChanged += Items_CollectionChanged;
					foreach (BarButtonViewModel galleryItem in groupModel.Items)
						SetBindingGalleryItemCommandParametrToSelectedComboBoxItem(galleryItem);
				}
			if (e.OldItems != null)
				foreach (GalleryGroupViewModel groupModel in e.OldItems) {
					groupModel.Items.CollectionChanged -= Items_CollectionChanged;
					foreach (BarButtonViewModel galleryItem in groupModel.Items)
						BindingOperations.ClearBinding(galleryItem, BarButtonViewModel.commandParameterProperty);
				}
		}
		void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.NewItems != null)
				foreach (BarButtonViewModel galleryItem in e.NewItems)
					SetBindingGalleryItemCommandParametrToSelectedComboBoxItem(galleryItem);
			if (e.OldItems != null)
				foreach (BarButtonViewModel galleryItem in e.OldItems)
					BindingOperations.ClearBinding(galleryItem, BarButtonViewModel.commandParameterProperty);
		}
		void SetBindingGalleryItemCommandParametrToSelectedComboBoxItem(BarButtonViewModel galeryItem) {
			Binding binding = new Binding("SelectedComboBoxItem");
			binding.Source = this;
			binding.Converter = SelectedComboBoxItemToGalleryItemCommandParameterConverter;
			BindingOperations.SetBinding(galeryItem, BarButtonViewModel.commandParameterProperty, binding);
		}
	}
}
