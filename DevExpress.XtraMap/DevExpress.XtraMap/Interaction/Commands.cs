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

using DevExpress.Map;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraMap.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraMap.Commands {
	public abstract class MapCommandBase : Command {
		readonly MapControl map;
		protected MapControl Map { get { return map; } }
		public override string Description { get { return string.Empty; } }
		public override string MenuCaption { get { return string.Empty; } }
		protected MapCommandBase(MapControl map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	public class MoveCenterPointCommand : MapCommandBase {
		Point offset;
		protected Point Offset { get { return offset; } }
		public MoveCenterPointCommand(MapControl map, Point offset)
			: base(map) {
				this.offset = offset;
		}
		public override void ForceExecute(ICommandUIState state) {
			Map.CenterPoint = Map.Map.Move(Map.CenterPoint, Offset);
		}
	}
}
namespace DevExpress.XtraMap.Native {
	public abstract class InnerMapCommand : Command {
		readonly InnerMap map;
		protected InnerMap Map { get { return map; } }
		public override string Description { get { return string.Empty; } }
		public override string MenuCaption { get { return string.Empty; } }
		protected InnerMapCommand(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	public abstract class MapKeyboardCommand : InnerMapCommand {
		protected MapKeyboardCommand(InnerMap map)
			: base(map) {
		}
		public override string Description { get { return string.Empty; } }
		public override string MenuCaption { get { return string.Empty; } }
		public override void ForceExecute(ICommandUIState state) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
		}
	}
	public class ZoomInCommand : MapKeyboardCommand {
		public ZoomInCommand(InnerMap map)
			: base(map) {
		}
		public override void ForceExecute(ICommandUIState state) {
			Map.ZoomIn();
		}
	}
	public class ZoomOutCommand : MapKeyboardCommand {
		public ZoomOutCommand(InnerMap map)
			: base(map) {
		}
		public override void ForceExecute(ICommandUIState state) {
			Map.ZoomOut();
		}
	}
	public abstract class MoveCommand : MapKeyboardCommand {
		protected MoveCommand(InnerMap map)
			: base(map) {
		}
		protected void MoveCore(Point offset) {
			Map.KeyboardShift(offset);
		}
	}
	public class MoveLeftCommand : MoveCommand {
		public MoveLeftCommand(InnerMap map)
			: base(map) {
		}
		public override void ForceExecute(ICommandUIState state) {
			MoveCore(new Point(10, 0));
		}
	}
	public class MoveRightCommand : MoveCommand {
		public MoveRightCommand(InnerMap map)
			: base(map) {
		}
		public override void ForceExecute(ICommandUIState state) {
			MoveCore(new Point(-10, 0));
		}
	}
	public class MoveTopCommand : MoveCommand {
		public MoveTopCommand(InnerMap map)
			: base(map) {
		}
		public override void ForceExecute(ICommandUIState state) {
			MoveCore(new Point(0, 10));
		}
	}
	public class MoveBottomCommand : MoveCommand {
		public MoveBottomCommand(InnerMap map)
			: base(map) {
		}
		public override void ForceExecute(ICommandUIState state) {
			MoveCore(new Point(0, -10));
		}
	}
#if DEBUG
	public class DebugInfoCommand : MapKeyboardCommand {
		public DebugInfoCommand(InnerMap map)
			: base(map) {
		}
		public override void ForceExecute(ICommandUIState state) {
			Map.ShowDebugInfo = !Map.ShowDebugInfo;
		}
	}
	public class ViewCenterPointCommand : MapKeyboardCommand {
		public ViewCenterPointCommand(InnerMap map)
			: base(map) {
		}
		public override void ForceExecute(ICommandUIState state) {
			Map.CenterPoint = Map.AnchorPoint;
		}
	}
	public class ResetZoomCommand : MapKeyboardCommand {
		public ResetZoomCommand(InnerMap map)
			: base(map) {
		}
		public override void ForceExecute(ICommandUIState state) {
			Map.ZoomLevel = InnerMap.DefaultZoomLevel;
		}
	}
#endif
	public class DeferredActionCommand : InnerMapCommand, IDisposable {
		public const int DefaultTimeout = 10000;
		public static void Execute(InnerMap map, Action action) {
			using (DeferredActionCommand cmd = new DeferredActionCommand(map, action)) {
				cmd.Execute();
				int start = System.Environment.TickCount;
				while (!cmd.IsReady) {
					Application.DoEvents();
					if (System.Environment.TickCount - start > DefaultTimeout) {
						break;
					}
				}
			}
		}
		readonly List<LayerBase> layerQueue;
		Action action;
		protected List<LayerBase> LayerQueue { get { return layerQueue; } }
		protected SortedLayerCollection Layers { get { return Map.ActualLayers; } }
		protected bool IsReadyForExport {
			get {
				lock (layerQueue) {
					return LayerQueue.Count == 0;
				}
			}
		}
		public bool IsReady { get; set; }
		protected DeferredActionCommand(InnerMap map, Action action)
			: base(map) {
			Guard.ArgumentNotNull(action, "action");
			this.action = action;
			this.layerQueue = new List<LayerBase>();
		}
		protected virtual void PerformAction() {
			action();
		}
		#region IDisposable implementation
		bool isDisposed;
		void Dispose(bool disposing) {
			if (disposing && !isDisposed) {
				isDisposed = true;
				lock (layerQueue) {
					layerQueue.ForEach((d) => d.DataLoaded -= OnLayerDataLoaded);
					layerQueue.Clear();
				}
			}
		}
		~DeferredActionCommand() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
		protected virtual bool ShouldWaitForLayer(LayerBase layer) {
			if (layer == null || !layer.CheckVisibility())
				return false;
			IRenderItemProvider provider = layer as IRenderItemProvider;
			if (provider == null || provider.IsReady)
				return false;
			return true;
		}
		protected virtual void RegisterLayer(LayerBase layer) {
			if (ShouldWaitForLayer(layer)) {
				layer.DataLoaded += OnLayerDataLoaded;
				lock (layerQueue) {
					LayerQueue.Add(layer);
				}
			}
		}
		protected virtual void UnregisterLayer(LayerBase layer) {
			layer.DataLoaded -= OnLayerDataLoaded;
			lock (layerQueue) {
				LayerQueue.Remove(layer);
			}
		}
		void OnLayerDataLoaded(object sender, DataLoadedEventArgs e) {
			if (isDisposed)
				return;
			LayerBase layer = (LayerBase)sender;
			IRenderItemProvider provider = layer as IRenderItemProvider;
			if (provider != null && provider.IsReady) {
				UnregisterLayer(layer);
				CheckReadyForExport();
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			IsReady = false;
			Layers.ForEach((d) => RegisterLayer(d));
			CheckReadyForExport();
		}
		protected void CheckReadyForExport() {
			if (IsReadyForExport) {
				IsReady = true;
				PerformAction();
			}
		}
	}
}
