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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Editors {
	public class PopupSettings {
		EditorPopupBase popup;
		protected internal UITypeEditorValue PopupValue { get; private set; }
		protected bool IsCustomPopup { get; private set; }
		public PopupCloseMode PopupCloseMode { get; private set; }
		protected PopupBaseEdit OwnerEdit { get; private set; }
		protected PopupBaseEditPropertyProvider PropertyProvider { get { return (PopupBaseEditPropertyProvider)OwnerEdit.PropertyProvider; } }
		protected PopupBaseEditSettings Settings { get { return OwnerEdit.Settings; } }
		protected IPopupSource PopupSource { get; private set; }
		protected internal EditorPopupBase Popup {
			get { return popup; }
			private set {
				if (Popup != null)
					UnsetupPopup(Popup);
				popup = value;
			}
		}
		PopupResizingStrategyBase popupResizingStrategy;
		protected internal PopupResizingStrategyBase PopupResizingStrategy {
			get {
				if (popupResizingStrategy == null)
					popupResizingStrategy = CreatePopupResizingStrategy();
				return popupResizingStrategy;
			}
		}
		protected virtual PopupResizingStrategyBase CreatePopupResizingStrategy() {
			if (BrowserInteropHelper.IsBrowserHosted)
				return new BrowserPopupResizingStrategy(OwnerEdit);
			return new DesktopPopupResizingStrategy(OwnerEdit);
		}
		public PopupSettings(PopupBaseEdit editor) {
			OwnerEdit = editor;
		}
		public void SetPopupSource(IPopupSource popupSource) {
			PopupSource = popupSource;
		}
		public void OpenPopup() {
			PopupCloseMode = PopupCloseMode.Normal;
			EnsurePopup();
			OwnerEdit.BeforePopupOpened();
			AssignPopupSizeFromSettings();
			SetSizeToPopup();
			OpenPopupCore();
			UpdatePopupProperties();
		}
		protected void EnsurePopup() {
			if (Popup == null) {
				Popup = CreatePopup();
				SetupPopup(Popup);
			}
		}
		protected virtual EditorPopupBase CreatePopup() {
			return new EditorPopupBase();
		}
		protected virtual void SetupPopup(EditorPopupBase popup) {
			BaseEdit.SetOwnerEdit(popup, OwnerEdit);
			SetPopupContent(popup);
			SetPopupPlacementTarget(popup);
			SetPopupBindings(popup);
			SubscribePopup(popup);
		}
		void SubscribePopup(EditorPopupBase popup) {
			SubscribePopupBorderControlSizeChanged();
			popup.PreviewKeyDown += PopupPreviewKeyDown;
		}
		void PopupPreviewKeyDown(object sender, KeyEventArgs e) {
			if (OwnerEdit.ProcessVisualClientKeyDown(e))
				OwnerEdit.ProcessPopupKeyDown(e);
		}
		void SubscribePopupBorderControlSizeChanged() {
			if (Popup != null && Popup.PopupBorderControl != null)
				Popup.PopupBorderControl.SizeChanged += PopupBorderControlSizeChanged;
		}
		void UnsubscribePopupBorderControlSizeChanged() {
			if (Popup != null && Popup.PopupBorderControl != null)
				Popup.PopupBorderControl.SizeChanged -= PopupBorderControlSizeChanged;
		}
		void PopupBorderControlSizeChanged(object sender, SizeChangedEventArgs e) {
			if (!updatePopupPropertiesLocker.IsLocked && IsPopupInVisualTree())
				UpdatePopupProperties();
		}
		bool IsPopupInVisualTree() {
			return Popup != null && Popup.IsInVisualTree();
		}
		readonly Locker updatePopupPropertiesLocker = new Locker();
		void AssignPopupSizeFromSettings() {
			if (OwnerEdit.EditMode == EditMode.Standalone || !Settings.IsSharedPopupSize)
				return;
			OwnerEdit.PopupMinHeight = Settings.PopupMinHeight;
			OwnerEdit.PopupHeight = Settings.PopupHeight;
			OwnerEdit.PopupMaxHeight = Settings.PopupMaxHeight;
			OwnerEdit.PopupMinWidth = Settings.PopupMinWidth;
			OwnerEdit.PopupWidth = Settings.PopupWidth;
			OwnerEdit.PopupMaxWidth = Settings.PopupMaxWidth;
		}
		void SetSizeToPopup() {
			SetSizeRestrictionsToPopup();
			SetActualSizeToPopup();
		}
		void SetActualSizeToPopup() {
			SetActualWidthToPopup();
			SetActualHeightToPopup();
		}
		void SetSizeRestrictionsToPopup() {
			SetMinWidthToPopup();
			SetMaxWidthToPopup();
			SetMinHeightToPopup();
			SetMaxHeightToPopup();
		}
		public void SetMinWidthToPopup() {
			if (Popup != null)
				Popup.PopupBorderControl.ContentMinWidth = OwnerEdit.ActualPopupMinWidth;
		}
		public void SetMaxWidthToPopup() {
			if (Popup != null)
				Popup.PopupBorderControl.ContentMaxWidth = OwnerEdit.PopupMaxWidth;
		}
		public void SetMinHeightToPopup() {
			if (Popup != null && OwnerEdit.ShouldApplyPopupSize)
				Popup.PopupBorderControl.ContentMinHeight = OwnerEdit.PopupMinHeight;
		}
		public void SetMaxHeightToPopup() {
			if (Popup != null)
				Popup.PopupBorderControl.ContentMaxHeight = OwnerEdit.PopupMaxHeight;
		}
		void OpenPopupCore() {
			Popup.FlowDirection = OwnerEdit.FlowDirection;
			Popup.IsOpen = true;
		}
		public void UpdatePopupProperties() {
			if (!OwnerEdit.IsLoaded)
				return;
			UpdateDropOpposite();
			UpdatePopupPlacement();
			UpdateActualPopupSize();
		}
		void UpdatePopupPlacement() {
			Popup.Placement = PopupResizingStrategy.GetPlacement();
		}
		void UpdateDropOpposite() {
			PopupResizingStrategy.UpdateDropOpposite();
			PropertyProvider.ResizeGripViewModel.Update();
			PropertyProvider.PopupViewModel.UpdateDropOpposite();
		}
		double GetPopupHeight(double offset) {
			return PopupResizingStrategy.GetPopupHeight(offset);
		}
		double GetPopupWidth(double offset) {
			return PopupResizingStrategy.GetPopupWidth(offset);
		}
		protected internal void SetHorizontalPopupSizeChange(double change) {
			if (PropertyProvider.IgnorePopupSizeConstraints)
				OwnerEdit.PopupMaxWidth = double.PositiveInfinity;
			OwnerEdit.PopupWidth = GetPopupWidth(change);
		}
		protected internal void SetVerticalPopupSizeChange(double change) {
			if (PropertyProvider.IgnorePopupSizeConstraints)
				OwnerEdit.PopupMaxHeight = double.PositiveInfinity;
			OwnerEdit.PopupHeight = GetPopupHeight(change);
		}
		public void UpdateActualPopupWidth(double width) {
			if (OwnerEdit.IsLoaded && OwnerEdit.IsInVisualTree())
				OwnerEdit.ActualPopupWidth = Math.Min(width, PopupResizingStrategy.ActualAvailableSize.Width);
			else
				OwnerEdit.ActualPopupWidth = width;
		}
		public void UpdateActualPopupHeight(double height) {
			if (OwnerEdit.IsLoaded && OwnerEdit.IsInVisualTree())
				OwnerEdit.ActualPopupHeight = Math.Min(height, PopupResizingStrategy.ActualAvailableSize.Height);
			else
				OwnerEdit.ActualPopupHeight = height;
		}
		internal void UpdateActualPopupSize() {
			UpdateActualPopupWidth(OwnerEdit.PopupWidth);
			UpdateActualPopupHeight(OwnerEdit.PopupHeight);
		}
		void SetPopupBindings(EditorPopupBase popup) {
			FrameworkElement root = LayoutHelper.FindRoot(OwnerEdit) as FrameworkElement;
			TransformGroup transforms = new TransformGroup();
			GeneralTransform transform = OwnerEdit.TransformToVisual(root);
			transforms.Children.Add(transform as Transform);
			if (OwnerEdit.FlowDirection == FlowDirection.RightToLeft && LayoutHelper.GetTopLevelVisual(OwnerEdit).GetType().FullName != "System.Windows.Controls.Primitives.PopupRoot")
				transforms.Children.Add(new ScaleTransform(-1, 1));
			Popup.RenderTransform = transforms;
		}
		public void UpdateActualPopupMinWidth() {
			if (!OwnerEdit.ShouldApplyPopupSize) {
				OwnerEdit.ActualPopupMinWidth = OwnerEdit.PopupMinWidth;
				return;
			}
			if (OwnerEdit.PopupMinWidth > OwnerEdit.ActualWidth)
				OwnerEdit.ActualPopupMinWidth = OwnerEdit.PopupMinWidth;
			else
				OwnerEdit.ActualPopupMinWidth = OwnerEdit.ActualWidth;
		}
		public void SetActualWidthToPopup() {
			if (Popup != null)
				Popup.PopupBorderControl.ContentWidth = OwnerEdit.ActualPopupWidth;
		}
		public void SetActualHeightToPopup() {
			if (Popup != null)
				Popup.PopupBorderControl.ContentHeight = OwnerEdit.ActualPopupHeight;
		}
		public void ClosePopup() {
			if (Popup != null)
				Popup.IsOpen = false;
			Popup = null;
		}
		protected virtual void UnsetupPopup(EditorPopupBase popup) {
			UnsubscribePopupBorderControlSizeChanged();
			DestroyPopupContent(popup);
			if (OwnerEdit.AllowRecreatePopupContent)
				OwnerEdit.PopupContentOwner.Child = null;
			UnsubcribePopup(popup);
			OwnerEdit.DestroyPopupContent(popup);
		}
		protected virtual void DestroyPopupContent(EditorPopupBase popup) {
			ContentControl contentControl = popup.Child as ContentControl;
			if (contentControl == null)
				return;
			PopupContentContainer container = LayoutHelper.FindElement(contentControl, element => element is PopupContentContainer) as PopupContentContainer;
			if (container != null) {
				NonLogicalDecorator decorator = container.Content as NonLogicalDecorator;
				if (decorator != null)
					decorator.Child = null;
				container.Content = null;
			}
			contentControl.Content = null;
			popup.PopupContent = null;
		}
		void UnsubcribePopup(EditorPopupBase popup) {
			popup.PreviewKeyDown -= PopupPreviewKeyDown;
		}
		void SetPopupContent(EditorPopupBase popup) {
			PopupContentContainer container = new PopupContentContainer(popup) {
				Focusable = false,
				Template = OwnerEdit.PopupContentContainerTemplate,
				Content = GetPopupContent()
			};
			popup.PopupContent = container;
		}
		protected virtual FrameworkElement GetPopupContent() {
			bool allowRecreatePopupContent = OwnerEdit.AllowRecreatePopupContent;
			NonLogicalDecorator popupChild = new NonLogicalDecorator();
			PopupContentControl popupContent = null;
			if (!allowRecreatePopupContent)
				popupContent = (PopupContentControl)OwnerEdit.PopupContentOwner.Child;
			if (popupContent == null) {
				popupContent = new PopupContentControl();
				FocusHelper.SetFocusable(popupContent, false);
			}
			popupContent.Editor = OwnerEdit;
			popupChild.Child = popupContent;
			OwnerEdit.PopupContentOwner.Child = popupContent;
			BaseEdit.SetOwnerEdit(popupContent, OwnerEdit); 
			popupContent.Tag = OwnerEdit;
			popupContent.Template = OwnerEdit.PopupContentTemplate;
			var popupSource = SelectPopupSource();
			if (popupSource != null) {
				IsCustomPopup = true;
				PopupValue = popupSource.GetEditableValue(OwnerEdit, OwnerEdit.EditValue);
				popupContent.ContentTemplate = popupSource.ContentTemplate;
				popupContent.ContentTemplateSelector = popupSource.ContentTemplateSelector;
				popupContent.Content = PopupValue;
			}
			else {
				IsCustomPopup = false;
				PopupValue = null;
			}
			return popupChild;
		}
		void SetPopupPlacementTarget(EditorPopupBase popup) {
			popup.PlacementTarget = OwnerEdit;
		}
		void SetPopupSizeRestrictions(EditorPopupBase popup) {
			if (double.IsNaN(OwnerEdit.ActualPopupMinWidth))
				Popup.PopupBorderControl.ClearValue(PopupBorderControl.ContentMinWidthProperty);
			else
				Popup.PopupBorderControl.ContentMinWidth = OwnerEdit.ActualPopupMinWidth;
			if (double.IsNaN(OwnerEdit.PopupMinHeight))
				Popup.ClearValue(FrameworkElement.MinHeightProperty);
			else
				popup.MinHeight = OwnerEdit.PopupMinHeight;
			if (double.IsNaN(OwnerEdit.PopupMaxWidth))
				Popup.PopupBorderControl.ClearValue(PopupBorderControl.ContentMaxWidthProperty);
			else
				popup.PopupBorderControl.ContentMaxWidth = OwnerEdit.PopupMaxWidth;
			if (double.IsNaN(OwnerEdit.PopupMaxHeight))
				Popup.ClearValue(FrameworkElement.MaxHeightProperty);
			else
				popup.MaxHeight = OwnerEdit.PopupMaxHeight;
		}
		public void SetPopupCloseMode(PopupCloseMode closeMode) {
			PopupCloseMode = closeMode;
		}
		public PopupCloseMode GetClosePopupOnClickMode() {
			return IsCustomPopup ? PopupCloseMode.Normal : PopupCloseMode.Cancel;
		}
		protected virtual IPopupSource SelectPopupSource() {
			if (PopupSource != null)
				return PopupSource;
			return GetPopupSourceSource().Select(GetBehavior).FirstOrDefault(x => x != null);
		}
		IEnumerable<DependencyObject> GetPopupSourceSource() {
			yield return OwnerEdit;
			yield return OwnerEdit.Settings;
			foreach (var button in OwnerEdit.ActualButtons.Where(x => x.IsDefaultButton)) {
				yield return button;
			}
			foreach (var button in OwnerEdit.Settings.Buttons.Where(x => x.IsDefaultButton))
				yield return button;
		}
		IPopupSource GetBehavior(DependencyObject element) {
			BehaviorCollection behaviors = (BehaviorCollection)element.GetValue(Interaction.BehaviorsProperty);
			if (behaviors == null)
				return null;
			return behaviors.FirstOrDefault(x => x is IPopupSource) as IPopupSource;
		}
		public virtual void AcceptPopupValue() {
			if (!IsCustomPopup)
				return;
			IPopupSource popupSource = (IPopupSource)PopupValue.Source;
			popupSource.AcceptEditableValue(PopupValue);
		}
	}
}
