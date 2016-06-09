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
using RoutingStrategy = DevExpress.Diagram.Core.Routing.RoutingStrategy;
using System.IO;
using DevExpress.Diagram.Core.Serialization;
using DevExpress.Diagram.Core.Base;
namespace DevExpress.Diagram.Core {
	public interface IDiagramControl {
		void ApplyLastBackgroundColor();
		void ApplyLastForegroundColor();
		void ApplyLastStrokeColor();
		IAdornerFactory CreateAdornerFactory();
		DiagramCommandsBase CreateCommands();
		IDiagramConnector CreateConnector();
		IDiagramContainer CreateContainer();
		IDiagramRoot CreateRootItem();
		IDiagramShape CreateShape();
		void DestroyPopupMenu();
		void DisplayItemProperties();
		RoutingStrategy GetRoutingStrategy(ConnectorType connectorType);
		void MoveFocus(LogicalDirection direction);
		void RaiseSelectionChanged();
		void SetPageSize();
		void ShowMessage(string message, bool errorMessage = false);
		string ShowOpenFileDialog();
		void ShowPopupMenu(DiagramMenuPlacement placement = DiagramMenuPlacement.Mouse);
		string ShowSaveFileDialog();
		void UpdateLayout();
		DiagramTool ActiveTool { get; }
		bool AllowEmptySelection { get; }
		double BringIntoViewMargin { get; }
		bool CanRedo { get; set; }
		bool CanUndo { get; set; }
		CanvasSizeMode CanvasSizeMode { get; set; }
		DiagramController Controller { get; }
		double GlueToConnectionPointDistance { get; }
		double GlueToItemDistance { get; }
		Size? GridSize { get; }
		bool HasChanges { get; set; }
		DiagramItemTypeRegistrator ItemTypeRegistrator { get; }
		MeasureUnit MeasureUnit { get; }
		double MinDragDistance { get; }
		Size PageSize { get; set; }
		ResizingMode ResizingMode { get; }
		Thickness ScrollMargin { get; }
		SelectionModel<IDiagramItem> SelectionModel { get; set; }
		SelectionToolsModel<IDiagramItem> SelectionToolsModel { get; set; }
		SelectionToolsModel<IDiagramItem> RootToolsModel { get; set; }
		bool SnapToGrid { get; }
		bool SnapToItems { get; }
		double SnapToItemsDistance { get; }
		DiagramTheme Theme { get; set; }
		double ZoomFactor { get; set; }
	}
	public interface IAdornerFactory {
		IAdorner CreateVRulerShadow();
		IAdorner CreateHRulerShadow();
		IAdorner CreateHBoundsSnapLine(BoundsSnapLine snapLine);
		IAdorner CreateVBoundsSnapLine(BoundsSnapLine snapLine);
		IAdorner CreateHSizeSnapLine(SizeSnapLine snapLine);
		IAdorner CreateVSizeSnapLine(SizeSnapLine snapLine);
		IAdorner CreateInplaceEditor(IDiagramItem item, Action onDestroy);
		IAdorner CreateItemDragPreview(IDiagramItem item);
		IAdorner CreateConnectorDragPreview(ShapeGeometry shape);
		IAdorner<IConnectorMovePointAdorner> CreateConnectorMovePointPreview();
		IAdorner CreateSelectionPreview();
		IAdorner CreateVRuler();
		IAdorner CreateHRuler();
		IAdorner CreateBackground();
		IAdorner<ISelectionAdorner> CreateSelectionAdorner(DefaultSelectionLayerHandler handler, double? multipleSelectionMargin);
		IAdorner<IShapeParametersAdorner> CreateShapeParametersAdorner(IDiagramShape shape);
		IAdorner<ISelectionPartAdorner> CreateSelectionPartAdorner(IDiagramItem item, bool isPrimarySelection);
		IAdorner<IConnectorAdorner> CreateConnectorSelectionAdorner(IDiagramConnector connector);
		IAdorner<IConnectorSelectionPartAdorner> CreateConnectorSelectionPartAdorner(IDiagramConnector item, bool isPrimarySelection);
		IAdorner CreateGlueToItemAdorner();
		IAdorner CreateGlueToPointAdorner();
		IAdorner<IConnectionPointsAdorner> CreateConnectionPointsAdorner();
	}
	public interface IConnectorMovePointAdorner {
		ShapeGeometry Shape { get; set; }
	}
	public enum StoreRelationsMode {
		RelativeToDiagram,
		RelativeToSerializingItems,
	}
	public static class DiagramExtensions {
		#region serialization
		public static string SerializeItems(this IDiagramControl diagram, IList<IDiagramItem> items, StoreRelationsMode storeRelationsMode) {
			using(MemoryStream memoryStream = new MemoryStream()) {
				diagram.Controller.SerializeItems(items, memoryStream, storeRelationsMode);
				return memoryStream.GetString();
			}
		}
		public static IList<IDiagramItem> DeserializeItems(this IDiagramControl diagram, string stringValue, StoreRelationsMode? storeRelationsMode) {
			using(MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(stringValue))) {
				return diagram.Controller.DeserializeItems(memoryStream, storeRelationsMode);
			}
		}
		public static void SaveDocument(this IDiagramControl diagram, string fileName) {
			using(FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite)) {
				diagram.SaveDocument(stream);
			}
		}
		public static void LoadDocument(this IDiagramControl diagram, string fileName) {
			using(var stream = File.OpenRead(fileName)) {
				diagram.LoadDocument(stream);
			}
		}
		public static void NewDocument(this IDiagramControl diagram) {
			diagram.Items().Clear();
			diagram.Controller.FileName = string.Empty;
			diagram.UndoManager().Clear();
		}
		public static void SaveDocument(this IDiagramControl diagram, Stream stream) {
			diagram.Controller.SerializeItems(diagram.RootItem().Yield<IDiagramItem>().ToList(), stream, StoreRelationsMode.RelativeToDiagram);
		}
		public static void LoadDocument(this IDiagramControl diagram, Stream stream) {
			diagram.Controller.OnBeginInit();
			diagram.Items().Clear();
			try {
				diagram.Controller.DeserializeItems(stream, StoreRelationsMode.RelativeToDiagram);
			}
			finally {
				diagram.UndoManager().Clear();
				diagram.Controller.OnEndInit();
			}
		}
		#endregion
		public static IList<IDiagramItem> SelectedItems(this IDiagramControl diagram) {
			return diagram.Selection().SelectedItems;
		}
		public static IDiagramItem PrimarySelection(this IDiagramControl diagram) {
			return diagram.Selection().PrimarySelection;
		}
		public static void SelectItem(this IDiagramControl diagram, IDiagramItem item, bool addToSelection = false) {
			diagram.Selection().SelectItem(item, addToSelection);
		}
		public static void UnselectItem(this IDiagramControl diagram, IDiagramItem item) {
			diagram.Selection().UnselectItem(item, deletingItem: false);
		}
		public static void SelectItems(this IDiagramControl diagram, IEnumerable<IDiagramItem> items, bool addToSelection = false) {
			diagram.Selection().SelectItems(items, addToSelection);
		}
		public static void ClearSelection(this IDiagramControl diagram) {
			diagram.Selection().ClearSelection();
		}
		public static bool CanChangeZOrder(this IDiagramControl diagram) {
			return diagram.Selection().CanChangeZOrder;
		}
		public static SelectionController Selection(this IDiagramControl diagram) {
			return diagram.Controller.Selection;
		}
		public static UndoManager UndoManager(this IDiagramControl diagram) {
			return diagram.Controller.UndoManager;
		}
		public static IAdornerFactory AdornerFactory(this IDiagramControl diagram) {
			return diagram.Controller.AdornerFactory;
		}
		public static DiagramCommandsBase Commands(this IDiagramControl diagram) {
			return diagram.Controller.Commands;
		}
		public static void UpdateCommands(this IDiagramControl diagram, IEnumerable<DiagramCommandBase> commands) {
			diagram.Commands().UpdateCommands(commands);
		}
		public static IAdorner CreateShadow(this IDiagramControl diagram) {
			return new CompositeAdorner(
				diagram.AdornerFactory().CreateVRulerShadow(),
				diagram.AdornerFactory().CreateHRulerShadow()
			);
		}
		public static IAdorner AddShadow(this IAdorner adorner, IDiagramControl diagram) {
			return new CompositeAdorner(diagram.CreateShadow(), adorner);
		}
		public static IDiagramRoot RootItem(this IDiagramControl diagram) {
			return diagram.Controller.RootItem;
		}
		public static IList<IDiagramItem> Items(this IDiagramControl diagram) {
			return diagram.RootItem().NestedItems;
		}
		internal static void ClearSelectionOrMakePrimarySelection(this IDiagramControl diagram, IDiagramItem item) {
			diagram.Selection().ClearSelectionOrMakePrimarySelection(item);
		}
	}
}
