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
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PropertyGrid.Themes;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class PopupBrushEditVisualClientOwner : VisualClientOwner {
		IPopupBrushEdit OwnerEdit { get { return base.Editor as IPopupBrushEdit; } }
		public PropertyGridControl PropertyGrid { get { return InnerEditor as PropertyGridControl; } }
		protected internal PopupBrushValue PopupPostValue { get; private set; }
		public PopupBrushEditVisualClientOwner(PopupBrushEdit editor)
			: base(editor) {
		}
		protected override FrameworkElement FindEditor() {
			if (LookUpEditHelper.GetPopupContentOwner(Editor).Child == null)
				return null;
			return LayoutHelper.FindElementByName(LookUpEditHelper.GetPopupContentOwner(Editor).Child, "PART_GridControl");
		}
		protected override void SetupEditor() {
			if (!IsLoaded)
				return;
			PropertyGrid.SortMode = PropertyGridSortMode.Definitions;
			PropertyGrid.UseCollectionEditor = true;
			PropertyGrid.ShowCategories = false;
			PropertyGrid.BorderThickness = new Thickness(0);
			PropertyGrid.ShowSearchBox = false;
			PropertyGrid.ShowToolPanel = false;
			PropertyGrid.ShowMenuButtonInRows = false;
			PropertyGrid.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
			AssignPropertyDefinitions();
			SetupEditorInternal(true);
		}
		protected virtual void AssignPropertyDefinitions() {
			PropertyGrid.PropertyDefinitions.Clear();
			PropertyGrid.PropertyDefinitions.Add(CreateBrushSelectorDefinition());
			PropertyGrid.PropertyDefinitions.Add(CreateSolidColorBrushDefinition());
			PropertyGrid.PropertyDefinitions.Add(CreateGradientStopsDefinition());
		}
		PropertyDefinitionBase CreateSolidColorBrushDefinition() {
			var pd = new PropertyDefinition();
			pd.PostOnEditValueChanged = true;
			pd.Path = "Color";
			pd.HeaderShowMode = HeaderShowMode.Hidden;
			pd.HighlightingMode = HeaderHighlightingMode.None;
			pd.EditSettings = new BrushEditSettings() { StyleSettings = new SolidColorBrushEditStyleSettings() };
			return pd;
		}
		PropertyDefinitionBase CreateBrushSelectorDefinition() {
			var pd = new PropertyDefinition();
			pd.Path = "BrushType";
			pd.ShowMenuButton = false;
			pd.HeaderShowMode = HeaderShowMode.Hidden;
			pd.HighlightingMode = HeaderHighlightingMode.None;
			pd.Visibility = OwnerEdit.AllowEditBrushType ? Visibility.Visible : Visibility.Collapsed;
			pd.Command = new DelegateCommand<BrushType>(ChangeBrushType);
			pd.ContentTemplate = (DataTemplate)PropertyGrid.FindResource(new PopupBrushEditThemeKeyExtension() {
				ResourceKey = PopupBrushEditThemeKeys.BrushSelectorTemplate,
				ThemeName = ThemeHelper.GetEditorThemeName(PropertyGrid),
			});
			return pd;
		}
		PropertyDefinitionBase CreateGradientStopsDefinition() {
			var pd = new PropertyDefinition();
			pd.Path = "GradientBrush";
			pd.HeaderShowMode = HeaderShowMode.Hidden;
			pd.HighlightingMode = HeaderHighlightingMode.None;
			pd.ContentTemplate = (DataTemplate)PropertyGrid.FindResource(new PopupBrushEditThemeKeyExtension() {
				ResourceKey = PopupBrushEditThemeKeys.GradientsTemplate,
				ThemeName = ThemeHelper.GetEditorThemeName(PropertyGrid),
			});
			return pd;
		}
		void ChangeBrushType(BrushType brushType) {
			PopupPostValue.BrushType = brushType;
		}
		protected virtual void SetupEditorInternal(bool assignSelectedObject) {
			if (assignSelectedObject)
				UpdateSelectedObject(PopupPostValue, OwnerEdit.GetPopupBrushValue(OwnerEdit.BrushType));
		}
		protected virtual void UpdateSelectedObject(PopupBrushValue oldValue, PopupBrushValue newValue) {
			oldValue.Do(x => x.BrushTypeChanged -= PopupValueBrushTypeChanged);
			newValue.Do(x => x.BrushTypeChanged += PopupValueBrushTypeChanged);
			PopupPostValue = newValue;
			PropertyGrid.SelectedObject = PopupPostValue;
		}
		void PopupValueBrushTypeChanged(object sender, EventArgs eventArgs) {
			BrushType brushType = PopupPostValue.BrushType;
			UpdateSelectedObject(PopupPostValue, OwnerEdit.GetPopupBrushValue(brushType));
		}
		public override void SyncProperties(bool syncDataSource) {
		}
		public override bool ProcessKeyDownInternal(KeyEventArgs e) {
			if (e.Handled)
				return true;
			if (!Editor.IsPopupOpen)
				return true;
			PropertyGridView view = PropertyGrid.View;
			if (view.ShouldPassKeyDownInLookUpMode(e))
				return false;
			view.ProcessKeyDown(e);
			return true;
		}
		public override object GetSelectedItem() {
			return PopupPostValue;
		}
		public override IEnumerable GetSelectedItems() {
			return null;
		}
	}
}
