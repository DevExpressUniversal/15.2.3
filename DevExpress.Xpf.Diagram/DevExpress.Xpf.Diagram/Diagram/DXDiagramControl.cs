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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Base;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Native;
using DevExpress.Images;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Editors;
using RoutingStrategy = DevExpress.Diagram.Core.Routing.RoutingStrategy;
namespace DevExpress.Xpf.Diagram {
	[ContentProperty("Items")]
	[TemplatePart(Name = PART_ScrollViewer, Type = typeof(ScrollViewer))]
	[TemplatePart(Name = PART_LayersHost, Type = typeof(LayersHost))]
	public partial class DiagramControl : Control, IDiagramControl {
		public static readonly DiagramItemTypeRegistrator ItemTypeRegistrator = new DiagramItemTypeRegistrator();
		const string PART_LayersHost = "LayersHost";
		const string PART_ScrollViewer = "ScrollViewer";
		static DiagramControl() {
			ImagesAssemblyLoader.Load();
			PropertyDescriptorWrapper.ExternalAndFluentAPIAttributesProvider = MetadataHelper.GetExternalAndFluentAPIAttrbutes;
			DependencyPropertyRegistrator<DiagramControl>.New()
				.OverrideDefaultStyleKey();
			ItemTypeRegistrator.Register(
				typeof(DiagramShape),
				typeof(DiagramConnector),
				typeof(DiagramContainer),
				typeof(DiagramContentItem),
				typeof(DiagramList)
			);
		}
		public DiagramItemCollection Items { get { return RootItem.Items; } }
		public IAdorner CreateHRulerAdorner(FrameworkElement element) {
			return HRuler.CreateAdorner(element);
		}
		public IAdorner CreateVRulerAdorner(FrameworkElement element) {
			return VRuler.CreateAdorner(element);
		}
		public IAdorner CreateBackgroundAdorner(FrameworkElement element) {
			return backgroundLayer.CreateAdorner(element);
		}
		public IAdorner CreateAdorner(FrameworkElement element) {
			return adornerLayer.CreateAdorner(element);
		}
		public IAdorner<TInterface> CreateAdornerEx<TObject, TInterface>(TObject control)
			where TObject : FrameworkElement, TInterface
			where TInterface : class {
			return adornerLayer.CreateAdornerEx<TObject, TInterface>(control);
		}
		protected virtual DiagramController CreateController() {
			return new DiagramController(this);
		}
		protected virtual void OnGridSizeChanged() {
		}
		protected virtual HorizontalRuler CreateHorizontalRuler() {
			return new HorizontalRuler(this);
		}
		protected virtual VerticalRuler CreateVerticalRuler() {
			return new VerticalRuler(this);
		}
		public DiagramControl() {
			backgroundLayer = CreateBackgroundAdornerLayer();
			adornerLayer = CreateAdornerLayer();
			hRuler = new Lazy<HorizontalRuler>(CreateHorizontalRuler);
			vRuler = new Lazy<VerticalRuler>(CreateVerticalRuler);
			Controller = CreateController();
			Controller.Initialize();
			menuController = CreateMenuController();
			Controller.RegisterStyleProvider(new DXCustomStyleProvider((item, id) => TryFindResource(id) as Style));
			Controller.RegisterStyleProvider(new DXCustomStyleProvider((item, id) => DiagramThemeHelper.CreateShapeStyle(item.GetStyle(this, id as DiagramItemStyleId))));
		}
#if DEBUGTEST
		internal void ResetHasChangesForTests() {
			HasChanges = false;
		}
#endif
		protected virtual AdornerLayer CreateBackgroundAdornerLayer() {
			return new AdornerLayer(this);
		}
		protected virtual AdornerLayer CreateAdornerLayer() {
			return new AdornerLayer(this);
		}
		protected virtual MenuController CreateMenuController() {
			return new MenuController(this);
		}
		protected virtual DiagramRoot CreateRootItem() {
			return new DiagramRoot(this);
		}
		public DiagramCommands Commands {
			get { return (DiagramCommands)this.Commands(); }
		}
		protected virtual DiagramCommands CreateCommands() {
			return new DiagramCommands(this);
		}
		readonly Lazy<HorizontalRuler> hRuler;
		HorizontalRuler HRuler { get { return hRuler.Value; } }
		readonly Lazy<VerticalRuler> vRuler;
		VerticalRuler VRuler { get { return vRuler.Value; } }
		ScrollViewer scrollViewer;
		protected internal DiagramRoot RootItem { get { return (DiagramRoot)Controller.RootItem; } }
		readonly AdornerLayer backgroundLayer;
		readonly AdornerLayer adornerLayer;
		public readonly DiagramController Controller;
		readonly MenuController menuController;
		public IList<DiagramItem> SelectedItems { get { return new SimpleBridgeList<DiagramItem, IDiagramItem>(this.Selection().SelectedItems); } }
		public DiagramItem PrimarySelection { get { return (DiagramItem)this.Selection().PrimarySelection; } }
		public event EventHandler SelectionChanged;
		protected virtual void RaiseSelectionChanged() {
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}
		internal LayersHost layersHost { get { return (LayersHost)Controller.LayersHost; } }
#if DEBUGTEST
		internal void ClearMultipleSelectionMarginForTests() {
			SelectionHandlerForTests.ClearMultipleSelectionMarginForTests();
		}
		internal DefaultSelectionLayerHandler SelectionHandlerForTests { get { return (DefaultSelectionLayerHandler)this.Selection().HandlerForTests; } }
		internal Rect SelectionRectForTests { get { return SelectionHandlerForTests.SelectionRectForTests; } }
		internal IAdorner SelectionAdornerForTest { get { return SelectionHandlerForTests.SelectionAdorderForTest; } }
		internal IAdorner ShapeParametersAdornerForTest { get { return SelectionHandlerForTests.ShapeParametersAdornerForTest; } }
		internal IAdorner[] SelectionPartAdornersForTest { get { return SelectionHandlerForTests.SelectionPartAdornersForTest.ToArray(); } }
		internal AdornerLayer AdornerLayerForTest { get { return adornerLayer; } }
		internal AdornerLayer BackgroundLayerForTest { get { return backgroundLayer; } }
		internal HorizontalRuler HRulerForTest { get { return HRuler; } }
		internal VerticalRuler VRulerForTest { get { return VRuler; } }
		internal DiagramController.ItemAdornersController ItemAdornersForTests { get { return Controller.ItemAdorners; } }
		public DiagramMenu MenuForTests { get { return menuController.MenuForTests; } }
		internal DiagramPopupToolBar ToolBarForTests { get { return menuController.ToolbarForTests; } }
		public string FileNameForTests { get; set; }
#endif
		void ShowPopupMenu(DiagramMenuPlacement placement = DiagramMenuPlacement.Mouse) {
			menuController.ShowPopupMenu(placement);
		}
		public override void OnApplyTemplate() {
			DettachFromLayersHost();
			DettachFromScrollViewer();
			base.OnApplyTemplate();
			Controller.LayersHost = (LayersHost)GetTemplateChild(PART_LayersHost);
			AttachToLayersHost();
			var layers = GetBackgroundLayers()
				.Concat(new LayerInfo(RootItem).Yield())
				.Concat(GetAdornerLayers()).ToArray();
			SetLayers(layers, HRuler, VRuler);
			scrollViewer = (ScrollViewer)GetTemplateChild(PART_ScrollViewer);
			AttachToScrollViewer();
		}
		public override void BeginInit() {
			base.BeginInit();
			Controller.OnBeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			Controller.OnEndInit();
		}
		protected virtual void SetLayers(LayerInfo[] layers, HorizontalRuler hRuler, VerticalRuler vRuler) {
			layersHost.SetLayers(layers, hRuler, vRuler);
		}
		protected virtual IEnumerable<LayerInfo> GetBackgroundLayers() {
			yield return new LayerInfo(backgroundLayer);
		}
		protected virtual IEnumerable<LayerInfo> GetAdornerLayers() {
			yield return new LayerInfo(adornerLayer);
		}
		protected virtual void ApplyBackgroundColor() { }
		protected virtual void ApplyForegroundColor() { }
		protected virtual void ApplyStrokeColor() { }
		protected virtual void DisplayItemProperties() { }
		protected virtual void SetPageSize() { }
		#region input
		void AttachToScrollViewer() {
			scrollViewer.Do(x => {
				x.KeyDown += OnKeyDown;
				x.KeyUp += OnKeyUp;
			});
		}
		void DettachFromScrollViewer() {
			scrollViewer.Do(x => {
				x.KeyDown -= OnKeyDown;
				x.KeyUp -= OnKeyUp;
			});
		}
		void AttachToLayersHost() {
			layersHost.Do(x => {
				x.Controller.SetOwner(this);
			});
		}
		void DettachFromLayersHost() {
			layersHost.Do(x => {
				x.Controller.SetOwner(null);
				layersHost.SetLayers(new LayerInfo[0], null, null);
			});
		}
		void OnKeyDown(object sender, KeyEventArgs e) {
			HandleKeyEvent(e, Controller.ProcessKeyDown);
		}
		void OnKeyUp(object sender, KeyEventArgs e) {
			HandleKeyEvent(e, Controller.ProcessKeyUp);
		}
		void OnRootKey(object sender, KeyEventArgs e) {
			HandleKeyEvent(e, Controller.ProcessRootKey);
		}
		void HandleKeyEvent(KeyEventArgs e, Func<Key, IMouseArgs, bool> handler) {
			var realKey = e.Key == Key.System ? e.SystemKey : e.Key;
			e.HandleEvent(() => handler(realKey, layersHost.CreatePlatformMouseArgs()));
		}
		protected override bool HandlesScrolling { get { return true; } }
		#endregion
		static readonly DependencyProperty IWindowServiceProperty = (DependencyProperty)typeof(Window).GetField("IWindowServiceProperty", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if (e.Property == IWindowServiceProperty) {
				(e.OldValue as FrameworkElement).Do(x => {
					x.KeyDown -= OnRootKey;
					x.KeyUp -= OnRootKey;
				});
				(e.NewValue as FrameworkElement).Do(x => {
					x.KeyDown += OnRootKey;
					x.KeyUp += OnRootKey;
				});
			}
			base.OnPropertyChanged(e);
		}
		protected internal virtual IEnumerable<IBarManagerControllerAction> CreateContextMenu() {
			yield return new BarButtonItem() {
				Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.Cut_16x16.png"),
				Content = DiagramControlLocalizer.GetString(DiagramControlStringId.ContextMenu_Cut), Command = Commands.Cut
			};
			yield return new BarButtonItem() {
				Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.Copy_16x16.png"),
				Content = DiagramControlLocalizer.GetString(DiagramControlStringId.ContextMenu_Copy), Command = Commands.Copy
			};
			yield return new BarButtonItem() {
				Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.Paste_16x16.png"),
				Content = DiagramControlLocalizer.GetString(DiagramControlStringId.ContextMenu_Paste), Command = Commands.Paste
			};
			yield return new BarItemSeparator();
			yield return new BarButtonItem() { Content = DiagramControlLocalizer.GetString(DiagramControlStringId.ContextMenu_EditText), Command = Commands.Edit };
			yield return new BarItemSeparator();
			yield return new BarButtonItem() { Content = DiagramControlLocalizer.GetString(DiagramControlStringId.Panel_Properties_Name), Command = Commands.DisplayItemProperties };
		}
		protected internal virtual void DestroyContextMenu() { }
		protected internal virtual IEnumerable<IBarManagerControllerAction> CreateContextToolBar() {
			var boldCheckItem = new BarCheckItem() { Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.Bold_16x16.png") };
			boldCheckItem.SetBinding(BarCheckItem.IsCheckedProperty, new Binding("[IsFontBold]") { Source = SelectionToolsModel });
			var italicChekItem = new BarCheckItem() { Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.Italic_16x16.png") };
			italicChekItem.SetBinding(BarCheckItem.IsCheckedProperty, new Binding("[IsFontItalic]") { Source = SelectionToolsModel });
			var sendToBackSplitItem = new BarSplitButtonItem() {
				Content = DiagramControlLocalizer.GetString(DiagramControlStringId.Layout_BringToFront),
				Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.BringToFront_16x16.png"), Command = Commands.BringToFront,
				PopupControl = new PopupMenu() {
					Items = {
						new BarButtonItem() { Content = DiagramControlLocalizer.GetString(DiagramControlStringId.Layout_BringForward),
							Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.BringForward_16x16.png"), Command = Commands.BringForward },
						new BarButtonItem() { Content = DiagramControlLocalizer.GetString(DiagramControlStringId.Layout_BringToFront),
							Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.BringToFront_16x16.png"), Command = Commands.BringToFront },
					}
				}
			};
			var bringToFrontSplitItem = new BarSplitButtonItem() {
				Content = DiagramControlLocalizer.GetString(DiagramControlStringId.Layout_SendToBack),
				Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.SendToBack_16x16.png"), Command = Commands.SendToBack,
				PopupControl = new PopupMenu {
					Items = {
						 new BarButtonItem() { Content =DiagramControlLocalizer.GetString(DiagramControlStringId.Layout_SendBackward),
							Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.SendBackward_16x16.png"), Command = Commands.SendBackward },
						new BarButtonItem() { Content = DiagramControlLocalizer.GetString(DiagramControlStringId.Layout_SendToBack),
							Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.SendToBack_16x16.png"), Command = Commands.SendToBack },
					}
				}
			};
			return new IBarManagerControllerAction[] {
				new BarButtonItem() { Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.FontSizeIncrease_16x16.png"), Command = Commands.IncreaseFontSize },
				new BarButtonItem() { Glyph = DiagramImageExtension.GetDiagramImage("DevExpress.Diagram.Core.Images.Menu.FontSizeDecrease_16x16.png"), Command = Commands.DecreaseFontSize },
				new BarItemSeparator(), boldCheckItem, italicChekItem, new BarItemSeparator(), sendToBackSplitItem, bringToFrontSplitItem
			};
		}
		protected internal void DestroyContextToolBar() { }
		#region IDiagramControl
		void IDiagramControl.DestroyPopupMenu() {
			menuController.Do(x => x.DestroyPopupMenu());
		}
		void IDiagramControl.RaiseSelectionChanged() {
			RaiseSelectionChanged();
		}
		DiagramController IDiagramControl.Controller {
			get { return Controller; }
		}
		void IDiagramControl.ShowPopupMenu(DiagramMenuPlacement placement) {
			ShowPopupMenu(placement);
		}
		void IDiagramControl.MoveFocus(LogicalDirection direction) {
			MoveFocusHelper.MoveFocus(this, direction == LogicalDirection.Backward);
		}
		IAdornerFactory IDiagramControl.CreateAdornerFactory() {
			return new AdornerFactory(this);
		}
		DiagramCommandsBase IDiagramControl.CreateCommands() {
			return CreateCommands();
		}
		IDiagramRoot IDiagramControl.CreateRootItem() {
			return CreateRootItem();
		}
		IDiagramConnector IDiagramControl.CreateConnector() {
			return new DiagramConnector();
		}
		IDiagramShape IDiagramControl.CreateShape() {
			return new DiagramShape();
		}
		IDiagramContainer IDiagramControl.CreateContainer() {
			return new DiagramContainer();
		}
		SelectionModel<IDiagramItem> IDiagramControl.SelectionModel { get { return SelectionModel; } set { SelectionModel = value; } }
		SelectionToolsModel<IDiagramItem> IDiagramControl.SelectionToolsModel { get { return SelectionToolsModel; } set { SelectionToolsModel = value; } }
		SelectionToolsModel<IDiagramItem> IDiagramControl.RootToolsModel { get { return RootToolsModel; } set { RootToolsModel = value; } }
		bool IDiagramControl.CanUndo { get { return CanUndo; } set { CanUndo = value; } }
		bool IDiagramControl.CanRedo { get { return CanRedo; } set { CanRedo = value; } }
		bool IDiagramControl.HasChanges { get { return HasChanges; } set { HasChanges = value; } }
		RoutingStrategy IDiagramControl.GetRoutingStrategy(ConnectorType connectorType) {
			return Controller.GetRoutingStrategy(connectorType);
		}
		DiagramItemTypeRegistrator IDiagramControl.ItemTypeRegistrator { get { return ItemTypeRegistrator; } }
		string IDiagramControl.ShowOpenFileDialog() {
#if DEBUGTEST
			if (FileNameForTests != null) return FileNameForTests;
#endif
			IOpenFileDialogService service = new OpenFileDialogService() {
				Filter = Controller.FileDialogFilter,
				Title = DiagramControlLocalizer.GetString(DiagramControlStringId.DiagramCommand_Open), Multiselect = false
			};
			bool dialogResult = service.ShowDialog();
			return dialogResult ? service.Files.First().GetFullName() : null;
		}
		string IDiagramControl.ShowSaveFileDialog() {
#if DEBUGTEST
			if (FileNameForTests != null) return FileNameForTests;
#endif
			ISaveFileDialogService service = new SaveFileDialogService() {
				ValidateNames = true,
				DefaultFileName = Controller.DefaultFileName,
				Filter = Controller.FileDialogFilter,
				Title = DiagramControlLocalizer.GetString(DiagramControlStringId.DiagramCommand_SaveAs)
			};
			bool dialogResult = service.ShowDialog();
			return dialogResult ? service.GetFullFileName() : null;
		}
		void IDiagramControl.ShowMessage(string message, bool errorFlag) {
			Window window = (Win32.GetOwnerWindow() as FrameworkElement) as Window;
			string title = (window != null) ? window.Title : string.Empty;
			if (errorFlag)
				DXMessageBox.Show(message, string.IsNullOrEmpty(title) ? DiagramControlLocalizer.GetString(DiagramControlStringId.DiagramNotification_DefaultCaption) : title,
					MessageBoxButton.OK, MessageBoxImage.Error);
			else
				DXMessageBox.Show(message, string.IsNullOrEmpty(title) ? DiagramControlLocalizer.GetString(DiagramControlStringId.DiagramNotification_DefaultCaption) : title,
					MessageBoxButton.OK);
		}
		void IDiagramControl.ApplyLastBackgroundColor() {
			ApplyBackgroundColor();
		}
		void IDiagramControl.ApplyLastForegroundColor() {
			ApplyForegroundColor();
		}
		void IDiagramControl.ApplyLastStrokeColor() {
			ApplyStrokeColor();
		}
		void IDiagramControl.DisplayItemProperties() {
			DisplayItemProperties();
		}
		void IDiagramControl.SetPageSize() {
			SetPageSize();
		}
		#endregion
		#region AdornerFactory
		class AdornerFactory : IAdornerFactory {
			readonly DiagramControl diagram;
			public AdornerFactory(DiagramControl diagram) {
				this.diagram = diagram;
			}
			IAdorner IAdornerFactory.CreateHRulerShadow() {
				return diagram.CreateHRulerAdorner(new RulerShadow());
			}
			IAdorner IAdornerFactory.CreateVRulerShadow() {
				return diagram.CreateVRulerAdorner(new RulerShadow());
			}
			IAdorner IAdornerFactory.CreateHBoundsSnapLine(BoundsSnapLine snapLine) {
				return diagram.CreateAdorner(new HorizontalBoundsSnapLineAdorner(snapLine));
			}
			IAdorner IAdornerFactory.CreateVBoundsSnapLine(BoundsSnapLine snapLine) {
				return diagram.CreateAdorner(new VerticalBoundsSnapLineAdorner(snapLine));
			}
			IAdorner IAdornerFactory.CreateHSizeSnapLine(SizeSnapLine snapLine) {
				return diagram.CreateAdorner(new HorizontalSizeSnapLineAdorner());
			}
			IAdorner IAdornerFactory.CreateVSizeSnapLine(SizeSnapLine snapLine) {
				return diagram.CreateAdorner(new VerticalSizeSnapLineAdorner());
			}
			IAdorner IAdornerFactory.CreateInplaceEditor(IDiagramItem item, Action onDestroy) {
				return diagram.CreateAdorner(new ItemEditorControl(item, onDestroy));
			}
			IAdorner IAdornerFactory.CreateSelectionPreview() {
				return diagram.CreateAdorner(new SelectionPreview());
			}
			IAdorner IAdornerFactory.CreateItemDragPreview(IDiagramItem item) {
				return diagram.CreateAdorner(new DragPreview((DiagramItem)item));
			}
			IAdorner IAdornerFactory.CreateConnectorDragPreview(ShapeGeometry shape) {
				return diagram.CreateAdorner(new ConnectorDragPreviewAdorner() { Shape = shape });
			}
			IAdorner<IConnectorMovePointAdorner> IAdornerFactory.CreateConnectorMovePointPreview() {
				return diagram.CreateAdornerEx<ConnectorMovePointPreviewAdorner, IConnectorMovePointAdorner>(new ConnectorMovePointPreviewAdorner());
			}
			IAdorner IAdornerFactory.CreateVRuler() {
				return diagram.CreateVRulerAdorner(new VerticalRulerScale(diagram));
			}
			IAdorner IAdornerFactory.CreateHRuler() {
				return diagram.CreateHRulerAdorner(new HorizontalRulerScale(diagram));
			}
			IAdorner IAdornerFactory.CreateBackground() {
				return diagram.CreateBackgroundAdorner(new PageBackgroundControl(diagram));
			}
			IAdorner<ISelectionAdorner> IAdornerFactory.CreateSelectionAdorner(DefaultSelectionLayerHandler handler, double? multipleSelectionMargin) {
				return diagram.CreateAdornerEx<SelectionAdorner, ISelectionAdorner>(new SelectionAdorner(handler, multipleSelectionMargin));
			}
			IAdorner<IConnectorAdorner> IAdornerFactory.CreateConnectorSelectionAdorner(IDiagramConnector connector) {
				return diagram.CreateAdornerEx<ConnectorSelectionAdorner, IConnectorAdorner>(new ConnectorSelectionAdorner(connector));
			}
			IAdorner<ISelectionPartAdorner> IAdornerFactory.CreateSelectionPartAdorner(IDiagramItem item, bool isPrimarySelection) {
				return diagram.CreateAdornerEx<SelectionPartAdorner, ISelectionPartAdorner>(new SelectionPartAdorner(item, isPrimarySelection));
			}
			IAdorner<IConnectorSelectionPartAdorner> IAdornerFactory.CreateConnectorSelectionPartAdorner(IDiagramConnector item, bool isPrimarySelection) {
				return diagram.CreateAdornerEx<ConnectorSelectionPartAdorner, IConnectorSelectionPartAdorner>(new ConnectorSelectionPartAdorner(item, isPrimarySelection));
			}
			IAdorner<IShapeParametersAdorner> IAdornerFactory.CreateShapeParametersAdorner(IDiagramShape shape) {
				return diagram.CreateAdornerEx<ShapeParametersAdorner, IShapeParametersAdorner>(new ShapeParametersAdorner(shape));
			}
			IAdorner IAdornerFactory.CreateGlueToItemAdorner() {
				return diagram.CreateAdorner(new GlueToItemAdorner());
			}
			IAdorner IAdornerFactory.CreateGlueToPointAdorner() {
				return diagram.CreateAdorner(new GlueToPointAdorner());
			}
			IAdorner<IConnectionPointsAdorner> IAdornerFactory.CreateConnectionPointsAdorner() {
				return diagram.CreateAdornerEx<ConnectionPointsAdorner, IConnectionPointsAdorner>(new ConnectionPointsAdorner());
			}
		}
		#endregion
	}
	class DXDiagramItemStyle : IDiagramItemStyle {
		public readonly Style Style;
		public DXDiagramItemStyle(Style style) {
			this.Style = style;
		}
	}
	public sealed class DXCustomStyleProvider : ICustomStyleProvider {
		Func<IDiagramItem, object, Style> getStyle;
		public DXCustomStyleProvider(Func<IDiagramItem, object, Style> getStyle) {
			this.getStyle = getStyle;
		}
		IDiagramItemStyle ICustomStyleProvider.GetStyle(IDiagramItem item, object id) {
			return getStyle(item, id).With(x => new DXDiagramItemStyle(x));
		}
	}
}
