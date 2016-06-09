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

using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Xpf.Bars {
	[Browsable(false)]
	public abstract class ThemeSelectorBehavior<T> : Behavior<T> where T : DependencyObject {
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ThemesCollectionProperty;
		protected override bool AllowAttachInDesignMode { get { return true; } }
		static ThemeSelectorBehavior() {
			ThemesCollectionProperty = DependencyProperty.Register("ThemesCollection", typeof(ICollectionView), typeof(ThemeSelectorBehavior<T>), new PropertyMetadata(null, OnThemesCollectionPropertyChanged));
		}
		static void OnThemesCollectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ThemeSelectorBehavior<T>)d).OnThemesCollectionChanged((ICollectionView)e.OldValue);
		}
		public ICollectionView ThemesCollection {
			get { return (ICollectionView)GetValue(ThemesCollectionProperty); }
			set { SetValue(ThemesCollectionProperty, value); }
		}
		public object StyleKey { get; set; }
		protected virtual DependencyProperty StyleProperty { get { return FrameworkElement.StyleProperty; } }
		public ThemeSelectorBehavior() {
			ThemesCollection = CreateCollectionView();
			StyleKey = CreateStyleKey();
		}
		protected abstract object CreateStyleKey();
		protected override void OnAttached() {
			base.OnAttached();
			if(AssociatedObject != null)
				Initialize(AssociatedObject);
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			if(AssociatedObject != null)
				Clear(AssociatedObject);
		}
		protected virtual void Clear(T item) {
			item.ClearValue(StyleProperty);
		}
		protected abstract void Initialize(T item);
		protected abstract void OnThemesCollectionChanged(ICollectionView oldValue);
		protected virtual ICollectionView CreateCollectionView() {
			ICollectionView view = CollectionViewSource.GetDefaultView(Theme.Themes.Where(t => t.ShowInThemeSelector).Select(t => new ThemeViewModel(t)).ToArray());
			view.GroupDescriptions.Add(new PropertyGroupDescription("Theme.Category"));
			return view;
		}
	}
	[Browsable(false)]
	public abstract class BarItemThemeSelectorBehavior<T> : ThemeSelectorBehavior<T> where T : BarItem {
		protected override DependencyProperty StyleProperty { get { return BarItem.StyleProperty; } }
		protected override void Initialize(T item) {
			item.DataContext = ThemesCollection;
			UpdateAssociatedObjectResourceReference();
		}
		protected override void Clear(T item) {
			base.Clear(item);
			item.DataContext = null;
		}
		protected override void OnThemesCollectionChanged(ICollectionView oldValue) {
			if(AssociatedObject != null)
				AssociatedObject.DataContext = ThemesCollection;
		}
		protected void UpdateAssociatedObjectResourceReference() {
			if(AssociatedObject != null) {
				AssociatedObject.SetResourceReference(StyleProperty, StyleKey);
			}
		}
	}
	[Browsable(false)]
	public abstract class GalleryBarItemThemeSelectorBehavior<T> : BarItemThemeSelectorBehavior<T> where T : BarItem {
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty GalleryProperty;
		static GalleryBarItemThemeSelectorBehavior() {
			GalleryProperty = DependencyProperty.Register("Gallery", typeof(Gallery), typeof(GalleryBarItemThemeSelectorBehavior<T>), new PropertyMetadata(null, OnGalleryPropertyChanged));
		}
		static void OnGalleryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((GalleryBarItemThemeSelectorBehavior<T>)d).OnGalleryChanged(e.OldValue as Gallery);
		}
		public Gallery Gallery {
			get { return (Gallery)GetValue(GalleryProperty); }
			set { SetValue(GalleryProperty, value); }
		}
		protected GalleryThemeSelectorBehavior GalleryBehavior {
			get {
				if(galleryBehavior == null)
					galleryBehavior = new GalleryThemeSelectorBehavior();
				return galleryBehavior;
			}
		}
		public object GalleryStyleKey { get { return galleryStyleKey; }
			set {
				if(galleryStyleKey != value) {
					galleryStyleKey = value;
					OnGalleryStyleKeyChanged();
				}
			}
		}
		public GalleryBarItemThemeSelectorBehavior() {
			GalleryStyleKey = CreateGalleryStyleKey();
		}
		protected override void Clear(T item) {
			base.Clear(item);
			Gallery = null;
		}
		protected override void OnThemesCollectionChanged(ICollectionView oldValue) {
			base.OnThemesCollectionChanged(oldValue);
			GalleryBehavior.ThemesCollection = ThemesCollection;
		}
		protected virtual object CreateGalleryStyleKey() {
			return new GalleryThemeSelectorThemeKeyExtension() { ResourceKey = GalleryThemeSelectorThemeKeys.Style, IsThemeIndependent = true };
		}
		protected virtual void InitializeGallery(Gallery gallery, GalleryThemeSelectorBehavior behavior) {
			Interaction.GetBehaviors(gallery).Add(behavior);
		}
		protected virtual void UninitializeGallery(Gallery gallery, GalleryThemeSelectorBehavior behavior) {
			Interaction.GetBehaviors(gallery).Remove(behavior);
		}
		protected virtual void OnGalleryChanged(Gallery oldValue) {
			if(oldValue != null)
				UninitializeGallery(oldValue, GalleryBehavior);
			if(Gallery != null)
				InitializeGallery(Gallery, GalleryBehavior);
		}
		protected virtual void OnGalleryStyleKeyChanged() {
			GalleryBehavior.StyleKey = GalleryStyleKey;
		}
		GalleryThemeSelectorBehavior galleryBehavior;
		object galleryStyleKey;
	}
	public class ThemeViewModel : BindableBase {
		public ThemeViewModel(Theme theme) {
			if(theme == null)
				throw new ArgumentNullException();
			Theme = theme;
			IsSelected = string.Equals(Theme.Name, ThemeManager.ActualApplicationThemeName);
			ThemeManager.ApplicationThemeChanged += OnThemeManagerApplicationThemeChanged;
		}
		public Theme Theme { get; private set; }
		public bool IsSelected {
			get { return GetProperty(() => IsSelected); }
			set { SetProperty(() => IsSelected, value, OnIsSelectedChanged); }
		}
		void OnIsSelectedChanged() {
			if(IsSelected)
				ThemeManager.ApplicationThemeName = Theme.Name;
		}
		void OnThemeManagerApplicationThemeChanged(DependencyObject sender, ThemeChangedRoutedEventArgs e) {
			IsSelected = string.Equals(e.ThemeName, Theme.Name, StringComparison.Ordinal);
		}
	}
}
