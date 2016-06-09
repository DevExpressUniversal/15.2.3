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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.Core;
using Platform::DevExpress.Xpf.LayoutControl;
namespace DevExpress.Xpf.LayoutControl.Design {
	using LayoutControl = Platform::DevExpress.Xpf.LayoutControl.LayoutControl;
	using DataLayoutControl = Platform::DevExpress.Xpf.LayoutControl.DataLayoutControl;
	[FeatureConnector(typeof(LayoutControlDesignFeatureConnector))]
	[UsesItemPolicy(typeof(LayoutControlSelectionPolicy))]
	class LayoutControlAdornerProvider : AdornerProvider {
		private LayoutControl _Control;
		private ModelItem _ModelItem;
		public LayoutControlAdornerProvider() {
			Adorner = new LayoutControlAdorner();
			Adorners.Add(Adorner);
		}
		protected override void Activate(ModelItem item) {
			base.Activate(item);
			ModelItem = item;
			Context.Items.Subscribe<Selection>(OnSelectionChanged);
			Context.Services.GetService<ViewService>().LayoutUpdated += OnLayoutUpdated;
			Context.Services.GetService<ModelService>().ModelChanged += OnModelChanged;
			OnModelChanged(null, null);
		}
		protected override void Deactivate() {
			Context.Services.GetService<ModelService>().ModelChanged -= OnModelChanged;
			Context.Services.GetService<ViewService>().LayoutUpdated -= OnLayoutUpdated;
			Context.Items.Unsubscribe<Selection>(OnSelectionChanged);
			ModelItem = null;
			base.Deactivate();
		}
		bool IsValidForControlSelection(Platform::System.Windows.FrameworkElement element) {
			ILayoutControl layoutControl = element.GetLayoutControl();
			if (layoutControl == null) {
				LayoutItem layoutItem = element.GetLayoutItem();
				if (layoutItem != null)
					return IsValidForControlSelection(layoutItem);
			}
			if (layoutControl == element && element != Control)
				layoutControl = element.GetParentLayoutControl();
			return layoutControl == Control;
		}
		void OnControlChanged(LayoutControl oldControl) {
			if (oldControl != null) {
				Adorner.Control = null;
				if (oldControl.Controller.CustomizationController != null)
					oldControl.Controller.CustomizationController.SelectionChanged -= OnControlSelectionChanged;
				oldControl.Controller.ModelChanged -= OnControlModelChanged;
				oldControl.Controller.IsCustomizationChanged -= OnControlIsCustomizationChanged;
				oldControl.Controller.IsCustomization = false;
				oldControl.Loaded -= OnControlLoaded;
			}
			if (Control != null) {
				Control.Loaded += OnControlLoaded;
				Control.Controller.IsCustomization = true;
				Control.Controller.IsCustomizationChanged += OnControlIsCustomizationChanged;
				Control.Controller.ModelChanged += OnControlModelChanged;
				Control.Controller.CustomizationController.SelectionChanged += OnControlSelectionChanged;
				Adorner.Control = Control;
#if !SILVERLIGHT
				Adorner.Visibility = Visibility.Visible;
#endif
			}
		}
		void OnControlIsCustomizationChanged(object sender, EventArgs e) {
			try {
				var newControl = (LayoutControl)ModelItem.GetCurrentValue();
				if (Control != newControl)
					Control = newControl;
				else
					if (Control.Controller.IsCustomization)
						Control.Controller.CustomizationController.SelectionChanged += OnControlSelectionChanged;
					else
						Control.Controller.CustomizationController.SelectionChanged -= OnControlSelectionChanged;
			}
			catch {
				Deactivate();
				return;
			}
			if (Control != null) {
#if !SILVERLIGHT
				Adorner.Visibility = Control.Controller.IsCustomization ? Visibility.Visible : Visibility.Collapsed;
#endif
				if (Control.Controller.IsCustomization)
					OnSelectionChanged(Context.Items.GetValue<Selection>());
			}
		}
		void OnControlLoaded(object sender, EventArgs e) {
			Control.Controller.IsCustomization = true;
		}
		void OnControlModelChanged(object sender, LayoutControlModelChangedEventArgs e) {			
			if(e is LayoutControlModelPropertyChangedEventArgs) {
				if(((LayoutControlModelPropertyChangedEventArgs)e).Property == DataLayoutControl.AutoGenerateItemsProperty) {
					DataLayoutControl dlc = sender as DataLayoutControl;
					if(dlc != null && !dlc.AutoGenerateItems) {
						ModelItemCollection children = ModelItem.Properties["Children"].Collection;						
						foreach(var child in children) {
							var currentValue = child.GetCurrentValue() as UIElement;
							if(currentValue != null)
								dlc.Children.Add(currentValue);
						}
					}
				}
			}
			using (ModelEditingScope editingScope = ModelItem.BeginEdit(e.ChangeDescription)) {
				if (e is LayoutControlModelStructureChangedEventArgs)
					UpdateModelStructure();
				if (e is LayoutControlModelPropertyChangedEventArgs)
					UpdateModelProperty((LayoutControlModelPropertyChangedEventArgs)e);
				editingScope.Complete();
			}
			if (StoredLayout != null)
				using (ModelEditingScope editingScope = ModelItem.BeginEdit(e.ChangeDescription + " - BUG WORKAROUND")) {
					var modelUpdater = new LayoutControlModelUpdater(ModelItem, Control);
					modelUpdater.RestoreLayout(StoredLayout);
					StoredLayout = null;
					editingScope.Complete();
				}
		}
		void OnControlSelectionChanged(object sender, LayoutControlSelectionChangedEventArgs e) {
			if (!Control.Controller.IsCustomization || IsChangingSelection)
				return;
			List<ModelItem> selectedItems = Context.Services.GetService<ModelService>().Find(ModelItem, e.SelectedElements);
			if (selectedItems.Count == 0)
				return;
			selectedItems.Reverse();
			IsChangingSelection = true;
			try {
				Context.Items.SetValue(new Selection(selectedItems));
			}
			finally {
				IsChangingSelection = false;
			}
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			if (Control == null)
				Control = (LayoutControl)ModelItem.GetCurrentValue();
			if (Control != null && Control.Controller.IsCustomization && Control.Controller.CustomizationController.SelectedElements.Count == 0)
				OnSelectionChanged(Context.Items.GetValue<Selection>());
		}
		void OnModelChanged(object sender, ModelChangedEventArgs e) {
			Adorner.OnModelChanged();
		}
		void OnSelectionChanged(Selection selection) {
			if (ModelItem == null)
				return;
			if (IsChangingSelection)
				return;
				Control = (LayoutControl)ModelItem.GetCurrentValue();
			if (Control == null || !Control.Controller.IsCustomization)
				return;
			var selectedElements = new List<Platform::System.Windows.FrameworkElement>(selection.SelectedObjects.GetPlatformObjects());
			selectedElements.RemoveAll(element => !IsValidForControlSelection(element));
			selectedElements.Reverse();
			IsChangingSelection = true;
			try {
				Control.Controller.CustomizationController.SelectedElements.Assign(selectedElements);
			}
			finally {
				IsChangingSelection = false;
			}
		}
		void OnModelItemChanged() {
			if (ModelItem != null)
				Control = (LayoutControl)ModelItem.GetCurrentValue();
			else
				Control = null;
			Adorner.ModelItem = ModelItem;
		}
		void UpdateModelProperty(LayoutControlModelPropertyChangedEventArgs args) {
			ModelItem modelItem = Context.Services.GetService<ModelService>().Find(ModelItem, args.Object);
			modelItem.UpdateProperty(args.PropertyName, args.Property, args.Object);
		}
		void UpdateModelStructure() {
			var modelUpdater = new LayoutControlModelUpdater(ModelItem, Control);
			modelUpdater.UpdateModel();
			StoredLayout = modelUpdater.StoreLayout();
			Context.Items.SetValue(Context.Items.GetValue<Selection>());
		}
		LayoutControlAdorner Adorner { get; set; }
		LayoutControl Control {
			get { return _Control; }
			set {
				if (Control == value)
					return;
				LayoutControl oldControl = Control;
				_Control = value;
				OnControlChanged(oldControl);
			}
		}
		bool IsChangingSelection { get; set; }
		ModelItem ModelItem {
			get { return _ModelItem; }
			set {
				if (ModelItem == value)
					return;
				_ModelItem = value;
				OnModelItemChanged();
			}
		}
		Dictionary<ModelItem, ModelItem[]> StoredLayout { get; set; }
	}
	class LayoutControlAdorner : FrameworkElement {
		private LayoutControl _Control;
		public LayoutControlAdorner() {
			Focusable = true;
			FocusVisualStyle = null;
			SnapsToDevicePixels = true;
		}
		public LayoutControl Control {
			get { return _Control; }
			set {
				if (Control == value)
					return;
				OnControlChanging();
				_Control = value;
				OnControlChanged();
			}
		}
		public ModelItem ModelItem { get; set; }
		protected bool CanProcessMouseInput(MouseEventArgs e) {
			return true;
		}
		protected void FormatHintText(FormattedText text) {
			if (Control is DataLayoutControl)
				text.SetFontWeight(FontWeights.Bold, text.Text.IndexOf("CurrentItem"), "CurrentItem".Length);
			else {
				text.SetFontWeight(FontWeights.Bold, text.Text.IndexOf("LayoutGroup"), "LayoutGroup".Length);
				text.SetFontWeight(FontWeights.Bold, text.Text.IndexOf("LayoutItem"), "LayoutItem".Length);
				text.SetFontWeight(FontWeights.Bold, text.Text.IndexOf("DataLayoutItem"), "DataLayoutItem".Length);
			}
		}
		protected string GetHintText() {
			if (Control is DataLayoutControl)
				return "Set the CurrentItem property to automatically generate content and build layout";
			else
				return "Drag and drop LayoutGroups, LayoutItems, DataLayoutItems and regular controls here to build your layout";
		}
		protected UIElement GetRootElement() {
			UIElement result = this;
			for (UIElement element = this; element != null; element = VisualTreeHelper.GetParent(element) as UIElement)
				result = element;
			return result;
		}
		protected bool IsHintNeeded() {
			if (Control is DataLayoutControl)
				return ((DataLayoutControl)Control).CurrentItem == null && ((DataLayoutControl)Control).AutoGenerateItems;
			else
				return Control != null && Control.GetLogicalChildren(false).Count == 0 ||
					ModelItem != null && ModelItem.Properties["Children"].Collection.Count == 0;
		}
		protected void OnControlChanging() {
#if SILVERLIGHT
			if (Control != null)
				ReleaseMouseCapture();
#endif
			if (Control is DataLayoutControl)
				((DataLayoutControl)Control).CurrentItemChanged -= OnControlCurrentItemChanged;
		}
		protected void OnControlChanged() {
			if (Control is DataLayoutControl)
				((DataLayoutControl)Control).CurrentItemChanged += OnControlCurrentItemChanged;
		}
		protected void OnControlCurrentItemChanged(object sender, ValueChangedEventArgs<object> e) {
			InvalidateVisual();
		}
		protected internal void OnModelChanged() {
			InvalidateVisual();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			Control.Controller.DesignTimeKeyDown(new DXKeyEventArgsFromWPF(e, args => e.Handled = args.Handled));
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			Control.Controller.DesignTimeKeyUp(new DXKeyEventArgsFromWPF(e, args => e.Handled = args.Handled));
		}
#if SILVERLIGHT
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			base.OnLostMouseCapture(e);
			Control.Controller.DesignTimeMouseCaptureCancelled();
		}
#endif
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			Control.Controller.DesignTimeMouseEnter(new DXMouseEventArgsFromWPF(e, this, this.Control));
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			if (Control != null)
#if !SILVERLIGHT
				if (!Control.Controller.IsMouseCaptured)
#endif
					Control.Controller.DesignTimeMouseLeave(new DXMouseEventArgsFromWPF(e, this, this.Control));
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if (!CanProcessMouseInput(e))
				return;
			if (e.ClickCount == 2) {
				var data = new GestureData(ModelItem.Context, ModelItem, ModelItem);
				if (SelectionCommands.ShowEvent.CanExecute(data))
					SelectionCommands.ShowEvent.Execute(data);
				e.Handled = true;
				return;
			}
			Focus();
			Control.Controller.DesignTimeMouseLeftButtonDown(new DXMouseButtonEventArgsFromWPF(e, this, this.Control));
#if SILVERLIGHT
			if (Control.Controller.IsMouseCaptured)
				CaptureMouse();
#endif
			if (Control.GetParentLayoutControl() != null ||
				!(Control.Controller.CustomizationController.SelectedElements.Count == 1 &&
				  Control.Controller.CustomizationController.SelectedElements[0] == Control))
				e.Handled = true;
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			Control.Controller.DesignTimeMouseLeftButtonUp(new DXMouseButtonEventArgsFromWPF(e, this, this.Control));
#if SILVERLIGHT
			if (!Control.Controller.IsMouseCaptured)
				ReleaseMouseCapture();
#endif
		}
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
			base.OnMouseRightButtonDown(e);
			Control.Controller.DesignTimeMouseRightButtonDown(new DXMouseButtonEventArgsFromWPF(e, this, this.Control));
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			Control.Controller.DesignTimeMouseMove(new DXMouseEventArgsFromWPF(e, this, this.Control));
#if SILVERLIGHT
			if (Control.Controller.IsMouseCaptured)
				CaptureMouse();
			e.Handled = IsMouseCaptured;
#endif
		}
		protected override void OnRender(DrawingContext drawingContext) {
			drawingContext.DrawRectangle(new SolidColorBrush(Colors.Transparent), null, new Rect(RenderSize));
			if (IsHintNeeded())
				DrawHint(drawingContext);
		}
		void DrawHint(DrawingContext drawingContext) {
			if (RenderSize.Width == 0 || RenderSize.Height == 0)
				return;
			const double RectangleOffset = 12;
			const double RectangleCornerRadius = 10;
			const double RectangleThickness = 2;
			const double TextOffsetX = 12;
			const double TextOffsetY = 10;
			var transparentBrush = new SolidColorBrush(Colors.Transparent);
			var lightGrayBrush = new SolidColorBrush(Colors.LightGray);
			var grayBrush = new SolidColorBrush(Colors.DimGray);
			var text = new FormattedText(GetHintText(),
				CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Portable User Interface"), 12, grayBrush);
			text.MaxTextWidth = Math.Max(0, RenderSize.Width - 2 * (RectangleOffset + TextOffsetX));
			FormatHintText(text);
			var rectangle = new Rect(RectangleOffset, RectangleOffset,
				Math.Max(0, RenderSize.Width - 2 * RectangleOffset), Math.Ceiling(2 * TextOffsetY + text.Height));
			drawingContext.DrawRoundedRectangle(transparentBrush, new Pen(lightGrayBrush, RectangleThickness), rectangle,
				RectangleCornerRadius, RectangleCornerRadius);
			drawingContext.DrawText(text, new Point(RectangleOffset + TextOffsetX, RectangleOffset + TextOffsetY));
		}
	}
	class LayoutControlDesignFeatureConnector : FeatureConnector<LayoutControlAdornerProvider> {
		public LayoutControlDesignFeatureConnector(FeatureManager manager) : base(manager) {
			Context.Services.Publish<LayoutControlDesignService>(new LayoutControlDesignService(Context));
		}
	}
	class LayoutControlDesignService {
		public LayoutControlDesignService(EditingContext context) {
			Context = context;
		}
		public bool GetIsInCustomizationMode(ModelItem layoutControlModelItem) {
			return ControlsNotInCustomizationMode == null || !ControlsNotInCustomizationMode.Contains(layoutControlModelItem);
		}
		public void SetIsInCustomizationMode(ModelItem layoutControlModelItem, bool value) {
			if (GetIsInCustomizationMode(layoutControlModelItem) == value)
				return;
			if (value) {
				ControlsNotInCustomizationMode.Remove(layoutControlModelItem);
				if (ControlsNotInCustomizationMode.Count == 0) {
					Context.Services.GetService<ModelService>().ModelChanged -= OnModelChangedForCustomizationMode;
					ControlsNotInCustomizationMode = null;
				}
			}
			else {
				if (ControlsNotInCustomizationMode == null) {
					ControlsNotInCustomizationMode = new List<ModelItem>();
					Context.Services.GetService<ModelService>().ModelChanged += OnModelChangedForCustomizationMode;
				}
				ControlsNotInCustomizationMode.Add(layoutControlModelItem);
			}
			OnIsInCustomizationModeChanged(layoutControlModelItem);
		}
		public bool GetIsInplaceLayoutBuilderVisible(ModelItem layoutControlModelItem) {
			return layoutControlModelItem != null && IsInplaceLayoutBuilderSupported(layoutControlModelItem) &&
				(ControlsWithHiddenInplaceLayoutBuilder == null || !ControlsWithHiddenInplaceLayoutBuilder.Contains(layoutControlModelItem));
		}
		public void SetIsInplaceLayoutBuilderVisible(ModelItem layoutControlModelItem, bool value) {
			if (GetIsInplaceLayoutBuilderVisible(layoutControlModelItem) == value)
				return;
			if (value) {
				ControlsWithHiddenInplaceLayoutBuilder.Remove(layoutControlModelItem);
				if (ControlsWithHiddenInplaceLayoutBuilder.Count == 0) {
					Context.Services.GetService<ModelService>().ModelChanged -= OnModelChangedForInplaceLayoutBuilder;
					ControlsWithHiddenInplaceLayoutBuilder = null;
				}
			}
			else {
				if (ControlsWithHiddenInplaceLayoutBuilder == null) {
					ControlsWithHiddenInplaceLayoutBuilder = new List<ModelItem>();
					Context.Services.GetService<ModelService>().ModelChanged += OnModelChangedForInplaceLayoutBuilder;
				}
				ControlsWithHiddenInplaceLayoutBuilder.Add(layoutControlModelItem);
			}
			OnIsInplaceLayoutBuilderVisibleChanged(layoutControlModelItem);
		}
		public EditingContext Context { get; private set; }
		public event Action<LayoutControlDesignService, ModelItem> IsInCustomizationModeChanged;
		public event Action<LayoutControlDesignService, ModelItem> IsInplaceLayoutBuilderVisibleChanged;
		bool IsInplaceLayoutBuilderSupported(ModelItem layoutControlModelItem) {
			return !layoutControlModelItem.IsItemOfType(typeof(DataLayoutControl));
		}
		void OnIsInCustomizationModeChanged(ModelItem layoutControlModelItem) {
			if (IsInCustomizationModeChanged != null)
				IsInCustomizationModeChanged(this, layoutControlModelItem);
		}
		void OnIsInplaceLayoutBuilderVisibleChanged(ModelItem layoutControlModelItem) {
			if (IsInplaceLayoutBuilderVisibleChanged != null)
				IsInplaceLayoutBuilderVisibleChanged(this, layoutControlModelItem);
		}
		void OnModelChangedForCustomizationMode(object sender, ModelChangedEventArgs e) {
			foreach (ModelItem item in e.ItemsRemoved)
				SetIsInCustomizationMode(item, true);
		}
		void OnModelChangedForInplaceLayoutBuilder(object sender, ModelChangedEventArgs e) {
			foreach (ModelItem item in e.ItemsRemoved)
				SetIsInplaceLayoutBuilderVisible(item, true);
		}
		List<ModelItem> ControlsNotInCustomizationMode { get; set; }
		List<ModelItem> ControlsWithHiddenInplaceLayoutBuilder { get; set; }
	}
	class LayoutControlSelectionPolicy : SelectionPolicy {
		protected override IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			ModelService modelService = Context.Services.GetService<ModelService>();
			LayoutControlDesignService designService = Context.Services.GetService<LayoutControlDesignService>();
			foreach (ModelItem item in modelService.Find(modelService.Root, t => typeof(LayoutControl).IsAssignableFrom(t)))
				if (designService.GetIsInCustomizationMode(item) && item.View != null) {
#if !SILVERLIGHT
					if (!(bool)item.Properties["IsVisible"].ComputedValue)
						continue;
#endif
					var layoutControl = (LayoutControl)item.GetCurrentValue();
					ILayoutControl parentLayoutControl = layoutControl.GetParentLayoutControl();
					if (parentLayoutControl == null || !IsInSelection(parentLayoutControl, selection))
						yield return item;
				}
			AttachToIsInCustomizationModeChanged(designService);
		}
		bool IsInSelection(ILayoutControl layoutControl, Selection selection) {
			return selection.SelectedObjects.GetPlatformObjects().Any(element => element.GetLayoutControl() == layoutControl);
		}
		static void AttachToIsInCustomizationModeChanged(LayoutControlDesignService designService) {
			designService.IsInCustomizationModeChanged -= OnIsInCustomizationModeChanged;
			designService.IsInCustomizationModeChanged += OnIsInCustomizationModeChanged;
		}
		static void OnIsInCustomizationModeChanged(LayoutControlDesignService service, ModelItem layoutControlModelItem) {
			service.Context.Items.SetValue(service.Context.Items.GetValue<Selection>());
		}
	}
	class DXKeyEventArgsFromWPF : DXKeyEventArgs {
		public DXKeyEventArgsFromWPF(KeyEventArgs args, Action<DXKeyEventArgs> handledChanged) {
			Handled = args.Handled;
#if SILVERLIGHT
			if (Enum.IsDefined(typeof(Platform::System.Windows.Input.Key), args.Key.ToString()))
				Key = (Platform::System.Windows.Input.Key)Enum.Parse(typeof(Platform::System.Windows.Input.Key), args.Key.ToString());
			else
				Key = Platform::System.Windows.Input.Key.Unknown;
#else
			Key = args.Key;
#endif
			HandledChanged = handledChanged;
		}
	}
	static class LayoutControlExtensions {
		public static ILayoutControl GetParentLayoutControl(this Platform::System.Windows.FrameworkElement layoutControl) {
			return (layoutControl.Parent as Platform::System.Windows.FrameworkElement).GetLayoutControl();
		}
	}
}
