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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Internal;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using System.Windows.Documents;
using System.ComponentModel;
using System.Windows.Input;
using DevExpress.Diagram.Core.Routing;
using DevExpress.Diagram.Core.Base;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Serialization;
using System.IO;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
namespace DevExpress.Diagram.Core {
	public class DiagramController {
		#region ItemAdornersController
		public class ItemAdornersController {
			readonly IDiagramControl diagram;
			Dictionary<IDiagramItem, IAdorner[]> adorners = new Dictionary<IDiagramItem, IAdorner[]>();
#if DEBUGTEST
			public IAdorner PageBorderAdornerForTest { get { return adorners[diagram.RootItem()].First(); } }
			public IAdorner GetItemAdornerForTests(IDiagramItem item) {
				return GetItemAdorners(item).SingleOrDefault();
			}
#endif
			public ItemAdornersController(IDiagramControl diagram) {
				this.diagram = diagram;
			}
			public void ItemChanged(IDiagramItem item, ItemChangedKind kind) {
				switch(kind) {
				case ItemChangedKind.Bounds:
					UpdateAdorner(item);
					break;
				case ItemChangedKind.Added:
					if(adorners.ContainsKey(item))
						throw new NotImplementedException();
					var itemAdorners = item.Controller.CreateAdorners().ToArray();
					if(itemAdorners.Any())
						adorners[item] = itemAdorners;
					UpdateAdorner(item);
					break;
				case ItemChangedKind.Removed:
					foreach(var adorner in GetItemAdorners(item)) {
						adorner.Destroy();
					}
					adorners.Remove(item);
					break;
				default:
					break;
				}
				UpdateRootItemAdorners();
			}
			void UpdateAdorner(IDiagramItem item) {
				foreach(var adorner in GetItemAdorners(item))
					adorner.SetBounds(item);
			}
			IEnumerable<IAdorner> GetItemAdorners(IDiagramItem item) {
				return adorners.GetValueOrDefault(item, EmptyArray<IAdorner>.Instance);
			}
			internal void UpdateRootItemAdorners() {
				SetRootItemAdornersBounds(diagram.Controller.ActualPageBounds);
			}
			void SetRootItemAdornersBounds(Rect newBounds) {
				GetItemAdorners(diagram.RootItem()).ForEach(adorner => adorner.SetBounds(newBounds.TopLeft, newBounds.BottomRight));
			}
		}
		#endregion
		public static readonly DiagramTool DefaultTool = new PointerTool();
		public DiagramTool CoerceTool(DiagramTool tool) {
			return tool ?? DefaultTool;
		}
		public const double MinZoomFactor = 0.01;
		public const double MaxZoomFactor = 30.0;
		public readonly IDiagramControl Diagram;
		internal readonly UndoManager UndoManager;
		internal readonly IAdornerFactory AdornerFactory;
		IInputElement inputElement = NullInputElement.Instance;
		InputState inputState;
		Locker eventProcessLock = new Locker();
		public ILayersHost LayersHost;
		internal readonly SelectionController Selection;
		internal readonly DiagramCommandsBase Commands;
		internal readonly ConnectorsController Connectors;
		public readonly IDiagramRoot RootItem;
		public readonly ItemAdornersController ItemAdorners;
		readonly Dictionary<ConnectorType, Routing.RoutingStrategy> RegisteredRoutingStrategy;
		Rect actualPageBounds;
		public Rect ActualPageBounds {
			get { return actualPageBounds; }
			private set {
				if (actualPageBounds.Equals(value))
					return;
				var oldValue = actualPageBounds;
				actualPageBounds = value;
				OnActualPageBoundsChanged(oldValue);
			}
		}
		public DiagramController(IDiagramControl diagram) {
			this.Diagram = diagram;
			this.UndoManager = UndoManager.New(OnUndoRedo);
			this.Selection = new SelectionController(Diagram, DefaultSelectionLayer.Instance);
			this.Commands = Diagram.CreateCommands();
			this.ItemAdorners = new ItemAdornersController(diagram);
			this.AdornerFactory = diagram.CreateAdornerFactory();
			this.Connectors = new ConnectorsController(diagram);
			this.RegisteredRoutingStrategy = new Dictionary<ConnectorType, Routing.RoutingStrategy>();
			inputState = DefaultInputState.Create(Diagram, DiagramCursor.Default, MouseMoveFeedbackHelper.Empty);
			RootItem = Diagram.CreateRootItem();
			Diagram.RootToolsModel = (SelectionToolsModel<IDiagramItem>)CreateMultiModel(typeof(SelectionToolsModel<>), typeof(SelectionToolsModel<>), false, new[] { RootItem });
			actualPageBounds = new Rect(diagram.PageSize);
		}
		public void Initialize() {
			RootItem.UpdateSize(Diagram);
			ItemChanged(RootItem, ItemChangedKind.Added);
			Selection.ValidateSelection();
		}
		#region Serialization
		static readonly string DiagramDocumentName = "DiagramDocument";
		protected internal virtual void SerializeItems(IList<IDiagramItem> items, Stream stream, StoreRelationsMode storeRelationsMode) {
			var container = new DiagramSerializingItemsContainer(Diagram, storeRelationsMode, items);
			container.AllItems.ForEach(x => x.Controller.PrepareRelations(container.GetPath));
			new DiagramSerializer().SerializeObject(container, stream, DiagramDocumentName, OptionsLayoutBase.FullLayout);
		}
		protected internal virtual IList<IDiagramItem> DeserializeItems(Stream stream, StoreRelationsMode? storeRelationsMode) {
			IList<IDiagramItem> items = null;
			var container = new DiagramDeserializingItemsContainer(Diagram, storeRelationsMode ?? default(StoreRelationsMode));
			try {
				new DiagramSerializer().DeserializeObject(container, stream, DiagramDocumentName, OptionsLayoutBase.FullLayout);
				if(storeRelationsMode != null)
					container.AllItems.ForEach(x => x.Controller.RestoreRelations(container.GetItem));
				items = container.Items;
			} catch {
#if DEBUGTEST
				throw;
#endif
			}
			return items;
		}
		#endregion
		public IList<IDiagramItem> SelectedItems { get { return Selection.SelectedItems; } }
		public IDiagramItem PrimarySelection { get { return Selection.PrimarySelection; } }
#if DEBUGTEST
		public Func<IDiagramItem, IEnumerable<IDiagramItem>, IDiagramItem> CoerceInsertTargetForTests;
		public Func<IDiagramItem, IDiagramItem, bool> CanAddItemForTests { get; set; }
#endif
		protected virtual IDiagramItem CoerceInsertTarget(IDiagramItem suggestedTarget, IEnumerable<IDiagramItem> items) {
#if DEBUGTEST
			if(CoerceInsertTargetForTests != null)
				return CoerceInsertTargetForTests(suggestedTarget, items);
			if(CanAddItemForTests != null && items.Any(x => !CanAddItemForTests(suggestedTarget, x)))
				return null;
#endif
			return suggestedTarget;
		}
		public bool DoCoercedInsertTargetAction(IDiagramItem suggestedTarget, IEnumerable<IDiagramItem> items, Action<IDiagramItem> targetAction) {
			return CoerceInsertTarget(suggestedTarget, items).Do(x => targetAction(x)).ReturnSuccess();
		}
		public bool DoDefaultInsertTargetAction(IEnumerable<IDiagramItem> items, Action<IDiagramItem> targetAction) {
			var suggestedTarget = PrimarySelection != null ?
								(PrimarySelection.NestedItems.IsReadOnly ? PrimarySelection.Owner() : PrimarySelection)
								: Diagram.RootItem();
			return DoCoercedInsertTargetAction(suggestedTarget, items, targetAction);
		}
		int lockInit = 0;
		public virtual void OnBeginInit() {
			lockInit++;
		}
		public virtual void OnEndInit() {
			if(lockInit > 0) {
				if(--lockInit == 0) OnInitialized();
			}
		}
		public bool IsInitializing { get { return lockInit != 0; } }
		void OnInitialized() {
			Connectors.RouteConnectorsWithoutMiddlePoints();
		}
		public InputState CreateDefaultInputState(IMouseArgs mouseArgs, MouseMoveFeedbackHelper feedbackHelper = null, IInputElement item = null) {
			var cursor = Diagram.ActiveTool.GetDefaultInputStateCursor(Diagram, item ?? inputElement, mouseArgs);
			if(feedbackHelper == null) {
				feedbackHelper = Diagram.ActiveTool.CreateFeedbackHelper(Diagram);
				feedbackHelper.ProvideMouseMoveFeedback(Diagram, mouseArgs.Position);
			}
			return DefaultInputState.Create(Diagram, cursor, feedbackHelper);
		}
		public InputState CreateActiveInputState(IInputElement item, IMouseButtonArgs mouseArgs) {
			return Diagram.ActiveTool.CreateActiveInputState(Diagram, item, mouseArgs);
		}
		public bool UpdateInputState(Func<InputState, IInputElement, InputResult> update, IMouseArgs args) {
			return UpdateInputState(x => update(x, inputElement = Diagram.Controller.FindInputElement(args)));
		}
		public bool UpdateInputState(Func<InputState, InputResult> update) {
			var handled = false;
			eventProcessLock.DoLockedActionIfNotLocked(() => {
				var inputResult = update(inputState);
				handled = inputResult.Handled;
				inputState = inputResult.State;
				UpdateCursor();
			});
			return handled;
		}
		public bool IsActiveInputState() {
			return inputState is MouseActiveInputState;
		}
		public bool EscapeState(IMouseArgs mouse) {
			return UpdateInputState(x => x.HandleEscape(mouse));
		}
#if DEBUGTEST
		public DiagramCursor CursorForTests { get; private set; }
		public ConnectorsController ConnectorsControllerForTests { get { return Connectors; } }
#endif
		void UpdateCursor() {
			var cursor = inputState.Cursor;
#if DEBUGTEST
			CursorForTests = cursor;
#endif
			SetCursor(cursor);
		}
		public void SelectItemsArea(Point startPosition, Point endPosition, bool addToSelection = false) {
			List<IDiagramItem> itemsForSelection = new List<IDiagramItem>();
			Rect inputSelection = MathHelper.RectFromPoints(startPosition, endPosition);
			RootItem.IterateChildren(item => {
				if(inputSelection.Contains(item.RotatedDiagramBounds().RotatedRect)) {
					itemsForSelection.Add(item);
					return true;
				}
				return false;
			});
			Diagram.SelectItems(itemsForSelection, addToSelection);
		}
		public virtual IEnumerable<PropertyDescriptor> GetProxyDescriptors<T>(T item, Func<T, object> getRealComponent, ITypeDescriptorContext realComponentContext = null, TypeConverter realComponentOwnerPropertyConverter = null, Attribute[] attributes = null) {
			return ProxyPropertyDescriptor.GetProxyDescriptors(item, getRealComponent, realComponentContext, realComponentOwnerPropertyConverter, attributes);
		}
		public void ItemChanged(IDiagramItem item, ItemChangedKind kind) {
			item.Owner().Do(x => x.Controller.ChildChanged(item, kind));
			switch(kind) {
				case ItemChangedKind.Added:
					OnItemAdded(item);
					break;
				case ItemChangedKind.Removed:
					OnItemRemoved(item);
					break;
				case ItemChangedKind.ZOrderChanged:
					OnItemZOrderChanged(item);
					break;
				case ItemChangedKind.Interaction:
					OnItemInteractionChanged(item);
					break;
				case ItemChangedKind.Bounds:
					OnItemBoundsChanged(item);
					break;
			}
			Selection.ItemChanged(item, kind);
			ItemAdorners.ItemChanged(item, kind);
			Connectors.ItemChanged(item, kind);
		}
		internal Rect CalculateActualPageBounds() {
			var originBounds = new Rect(Diagram.PageSize);
			var itemsBounds = Diagram.Items().Select(x => x.RotatedDiagramBounds());
			if(Diagram.CanvasSizeMode == CanvasSizeMode.None || !itemsBounds.Any())
				return originBounds;
			var newBounds = MathHelper.GetContainingRect(itemsBounds);
			var result = originBounds.ProportionalRect(newBounds);
			return result;
		}
		public void OnCanvasSizeModeChanged() {
			UpdateActualPageBounds();
		}
		void UpdateActualPageBounds() {
			ActualPageBounds = CalculateActualPageBounds();
		}
		void OnActualPageBoundsChanged(Rect oldBounds) {
			UpdateLayersHost(OffsetCorrections.Extent(KeepHorizontalOffset, KeepVerticalOffset));
			ItemAdorners.UpdateRootItemAdorners();
		}
#if DEBUGTEST
		public int OnItemAddedCountForTests;
		public int OnItemRemovedCountForTests;
		public int OnItemZOrderChangedForTests;
		WeakReference lastItemNotifiedForTestsRef;
		public IDiagramItem LastItemNotifiedForTests {
			get { return lastItemNotifiedForTestsRef.With(x => x.Target as IDiagramItem); }
			set { lastItemNotifiedForTestsRef = new WeakReference(value); }
		}
#endif
		protected virtual void OnItemAdded(IDiagramItem item) {
#if DEBUGTEST
			OnItemAddedCountForTests++;
			LastItemNotifiedForTests = item;
#endif
			UpdateItemCustomStyle(item);
			UpdateActualPageBounds();
		}
		protected virtual void OnItemRemoved(IDiagramItem item) {
#if DEBUGTEST
			OnItemRemovedCountForTests++;
			LastItemNotifiedForTests = item;
#endif
			ClearItemCustomStyle(item);
			UpdateActualPageBounds();
		}
		protected virtual void OnItemZOrderChanged(IDiagramItem item) {
#if DEBUGTEST
			OnItemZOrderChangedForTests++;
			LastItemNotifiedForTests = item;
#endif
		}
		protected virtual void OnItemInteractionChanged(IDiagramItem item) {
			Diagram.UpdateCommands(DiagramCommandsBase.SelectionCommands);
			Selection.UpdateSelectionAdorners();
		}
		protected virtual void OnItemBoundsChanged(IDiagramItem item) {
			UpdateActualPageBounds();
		}
		public bool KeepVerticalOffset { get; set; }
		public bool KeepHorizontalOffset { get; set; }
		public OffsetCorrection GetOffsetCorrection(Func<bool, bool, OffsetCorrection> correctionFunction) {
			return correctionFunction(KeepHorizontalOffset, KeepVerticalOffset);
		}
		#region input
		public bool ProcessMouseUp(IMouseButtonArgs args) {
			return UpdateInputState((x, inputElement) => x.HandleMouseUp(inputElement, args), args);
		}
		public bool ProcessMouseMove(IMouseArgs args) {
			return UpdateInputState((x, inputElement) => x.HandleMouseMove(inputElement, args), args);
		}
		public bool ProcessMouseLeave(IMouseArgs args) {
			return UpdateInputState((x, inputElement) => x.HandleMouseLeave(args), args);
		}
		public bool ProcessMouseDown(IMouseButtonArgs args) {
			return UpdateInputState((x, inputElement) => x.HandleMouseDown(inputElement, args), args);
		}
		public bool ProcessKeyDown(Key key, IMouseArgs mouse) {
			return ProcessKeyCore(key, mouse, DiagramCommandsBase.GetKeyDownCommand);
		}
		public bool ProcessKeyUp(Key key, IMouseArgs mouse) {
			return ProcessKeyCore(key, mouse, DiagramCommandsBase.GetKeyUpCommand);
		}
		public bool ProcessRootKey(Key key, IMouseArgs mouse) {
			return ProcessKeyCore(key, mouse, (k, m) => DiagramCommandsBase.EmptyCommand);
		}
		bool ProcessKeyCore(Key key, IMouseArgs mouse, Func<Key, ModifierKeys, DiagramCommand> getCommand) {
			if(IsModifierKey(key)) {
				return UpdateInputState(x => x.HandleModifiersChanged(mouse));
			} else {
				var command = getCommand(key, mouse.Modifiers);
				Commands.Execute(command, null, mouse);
				return command != DiagramCommandsBase.EmptyCommand;
			}
		}
		static bool IsModifierKey(Key key) {
			return key == Key.LeftCtrl || key == Key.RightCtrl || key == Key.LeftAlt || key == Key.RightAlt;
		}
		public bool ProcessLostMouseCapture(IMouseArgs mouse) {
			return EscapeState(mouse);
		}
		#endregion
		#region Save/Load Document Options
		public string FileDialogFilter {
			get {
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("{0} (*.xml)|*.xml|", DiagramControlLocalizer.GetString(DiagramControlStringId.DocumentFormat_XmlFile));
				stringBuilder.AppendFormat("{0} (*.*)|*.*", DiagramControlLocalizer.GetString(DiagramControlStringId.DocumentFormat_AllFiles));
				return stringBuilder.ToString();
			}
		}
		public string DefaultFileName { get { return DiagramControlLocalizer.GetString(DiagramControlStringId.DefaultDocumentName); } }
		public string FileName { get; set; }
		#endregion
		public void OnActiveToolChanged() {
			Selection.UpdateSelectionAdorners();
			if(LayersHost != null)
				EscapeState(Diagram.Controller.CreatePlatformMouseArgs());
		}
		Locker updateLayersHostLocker = new Locker();
		public void OnExtentChanged() {
			UpdateActualPageBounds();
			UpdateLayersHost(GetOffsetCorrection(OffsetCorrections.Extent));
		}
		public void OnZoomFactorChanged() {
			UpdateLayersHost(OffsetCorrections.Zoom());
		}
		void UpdateLayersHost(OffsetCorrection offsetCorrection) {
			updateLayersHostLocker.DoIfNotLocked(() => {
				UpdateLayersHostCore(offsetCorrection);
			});
			RootItem.UpdateSize(Diagram);
			Diagram.UpdateCommands(DiagramCommandsBase.ZoomCommands);
		}
		void UpdateLayersHostCore(OffsetCorrection offsetCorrection) {
			LayersHost.Do(x => x.Controller.Update(offsetCorrection));
		}
		public void ZoomTo(double newZoom, IMouseArgs e) {
			var displayPoint = GetDisplayPosition(e);
			updateLayersHostLocker.DoLockedAction(() => {
				Diagram.ZoomFactor = newZoom;
			});
			UpdateLayersHostCore(OffsetCorrections.GetZoomCorrection(displayPoint));
		}
		public void Zoom(double delta, IMouseArgs e) {
			ZoomTo(ModifyZoomFactor(delta), e);
		}
		public double CoerceZoom(double value) {
			return MathHelper.ValidateValue(value, MinZoomFactor, MaxZoomFactor);
		}
		static readonly ZoomHelper zoomHelper = new ZoomHelper();
		protected virtual double ModifyZoomFactor(double delta) {
			return zoomHelper.ModZoomFactor(Diagram.ZoomFactor, (int)delta);
		}
		public void SetCursor(DiagramCursor cursor) {
			LayersHost.Do(x => x.SetCursor(cursor));
		}
		public void FocusSurface() {
			LayersHost.FocusSurface();
		}
		public void BringRectIntoView(Rect rect) {
			LayersHost.Do(x => x.Controller.BringRectIntoView(rect));
		}
		public IInputElement FindInputElement(IMouseArgs args) {
			return LayersHost.FindInputElement(GetDisplayPosition(args));
		}
		protected virtual Point GetDisplayPosition(IMouseArgs args) {
			return args.Position.TransformPoint(LayersHost.Controller.CreateLogicToDisplayTransform());
		}
		public IMouseArgs CreatePlatformMouseArgs() {
			return LayersHost.CreatePlatformMouseArgs();
		}
		public void RegisterRoutingStrategy(ConnectorType connectorType, Routing.RoutingStrategy strategy) {
			RegisteredRoutingStrategy[connectorType] = strategy;
		}
		public void UnregisterRoutingStrategy(ConnectorType connectorType) {
			RegisteredRoutingStrategy.Remove(connectorType);
		}
		public Routing.RoutingStrategy GetRoutingStrategy(ConnectorType connectorType) {
			((RightAngleRoutingStrategy)Routing.RoutingStrategy.RightAngle).ItemMargin = (int)Diagram.GlueToConnectionPointDistance;
			return RegisteredRoutingStrategy.GetValueOrDefault(connectorType)
				?? GetDefaultRoutingStrategy(connectorType);
		}
		static Routing.RoutingStrategy GetDefaultRoutingStrategy(ConnectorType connectorType) {
			if(connectorType == ConnectorType.Curved || connectorType == ConnectorType.Straight)
				return Routing.RoutingStrategy.Straight;
			else if(connectorType == ConnectorType.RightAngle)
				return Routing.RoutingStrategy.RightAngle;
			throw new NotSupportedException();
		}
		void OnUndoRedo() {
			Diagram.SelectionModel.Do(x => RaiseSelectionModelPropertiesChanged());
			Diagram.SelectionToolsModel.Do(x => x.RaisePropertiesChanged());
			Diagram.RootToolsModel.RaisePropertiesChanged();
			Diagram.UpdateCommands(DiagramCommandsBase.UndoRedoCommands);
			Diagram.CanUndo = UndoManager.CanUndo();
			Diagram.CanRedo = UndoManager.CanRedo();
			Diagram.HasChanges = Diagram.CanUndo || Diagram.CanRedo;
		}
		protected virtual void RaiseSelectionModelPropertiesChanged() {
			Diagram.SelectionModel.RaisePropertyChanged();
		}
		public void OnSelectionChanged() {
#if DEBUGTEST
			if(!Diagram.AllowEmptySelection && PrimarySelection == null)
				throw new InvalidOperationException();
#endif
			Diagram.DestroyPopupMenu();
			Diagram.SelectionModel = CreateSelectionModel<SelectionModel<IDiagramItem>>(typeof(SelectionModel<>), typeof(SelectionModel<>), allowMerge: true);
			Diagram.SelectionToolsModel = CreateSelectionModel<SelectionToolsModel<IDiagramItem>>(typeof(SelectionToolsModel<>), typeof(SelectionModel<>), allowMerge: false);
			Commands.UpdateCommands(DiagramCommandsBase.SelectionCommands);
			Diagram.RaiseSelectionChanged();
		}
		T CreateSelectionModel<T>(Type modelType, Type nextModelType, bool allowMerge) where T : class, IMultiModel {
			return (T)PrimarySelection.With(_ => CreateMultiModel(modelType, nextModelType, allowMerge, SelectedItems));
		}
		public IMultiModel CreateMultiModel(Type modelType, Type nextModelType, bool allowMerge, IList<IDiagramItem> items) {
			Action<Action<Transaction>> executeTransactionAction = action => {
				Diagram.ExecuteWithSelectionRestore(transaction => action(transaction), allowMerge: allowMerge);
			};
			var context = CreateDiagramMultimodelContext<IDiagramItem>(modelType, nextModelType, x => x, executeTransactionAction);
			return MultiPropertyHelper.CreateMultimodel(items.First(), items, context);
		}
		internal MultiModelContext<IDiagramItem, T> CreateDiagramMultimodelContext<T>(Type modelType, Type nextModelType, Func<IDiagramItem, T> getItem, Action<Action<Transaction>> executeTransactionAction) {
			return new MultiModelContext<IDiagramItem, T>(x => x.GetFinder(), getItem, modelType, nextModelType, executeTransactionAction,
				(component, descriptorContext, converter, attributes) => {
					return (component as IDiagramItem).Return(x => x.Controller.EditableProperties,
						() => Diagram.Controller.GetProxyDescriptors(component, x => x, descriptorContext, converter, attributes));
				});
		}
		public void OnThemeChanged() {
			UpdateCustomStyles();
		}
		#region themes
		List<ICustomStyleProvider> customStyleProviders = new List<ICustomStyleProvider>();
		public void RegisterStyleProvider(ICustomStyleProvider provider) {
			customStyleProviders.Add(provider);
			UpdateCustomStyles();
		}
		public void UnregisterStyleProvider(ICustomStyleProvider provider) {
			customStyleProviders.Remove(provider);
			UpdateCustomStyles();
		}
		void UpdateCustomStyles() {
			RootItem.GetChildren().ForEach(item => UpdateItemCustomStyle(item));
		}
		IDiagramItemStyle FindCustomStyle(IDiagramItem item, object id) {
			return customStyleProviders
				.Reverse<ICustomStyleProvider>()
				.Select(x => x.GetStyle(item, id))
				.FirstOrDefault(x => x != null);
		}
		public void UpdateItemCustomStyle(IDiagramItem item) {
			if(item == null)
				return;
			var styleIds = item.GetActualStyleId().Yield()
				.Concat(item.CustomStyleId.Yield())
				.Concat(item.Controller.AdditionalStyles);
			var styles = styleIds.Select(styleId => styleId.With(x => FindCustomStyle(item, x)));
			item.SetCustomStyles(styles.ToArray());
		}
		public void ClearItemCustomStyle(IDiagramItem item) {
			item.SetCustomStyles(new IDiagramItemStyle[0]);
		}
		#endregion
	}
	public interface ICustomStyleProvider {
		IDiagramItemStyle GetStyle(IDiagramItem item, object id);
	}
	public interface IDiagramItemStyle {
	}
	public sealed class CustomStyleProvider : ICustomStyleProvider {
		Func<IDiagramItem, object, IDiagramItemStyle> getStyle;
		public CustomStyleProvider(Func<IDiagramItem, object, IDiagramItemStyle> getStyle) {
			this.getStyle = getStyle;
		}
		IDiagramItemStyle ICustomStyleProvider.GetStyle(IDiagramItem item, object id) {
			return getStyle(item, id);
		}
	}
}
