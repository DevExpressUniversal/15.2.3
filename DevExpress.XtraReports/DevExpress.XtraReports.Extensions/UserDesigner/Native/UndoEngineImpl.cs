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
using System.Text;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using System.IO;
using System.ComponentModel;
using DevExpress.XtraReports.Design.Commands;
namespace System.ComponentModel.Design {
	public static class UndoEngineExtention {
		public static void ExecuteWithoutUndo(this UndoEngine eng, Action action) {
			if(eng == null) {
				action();
				return;
			}
			bool enabled = eng.Enabled;
			eng.Enabled = false;
			try {
				action();
			} finally {
				eng.Enabled = enabled;
			}
		}
	}
}
namespace DevExpress.XtraReports.UserDesigner.Native {
	public interface IUndoService {
		void ClearUndoStack();
	}
	class UndoService : IUndoService {
		IServiceProvider servProvider;
		public UndoService(IServiceProvider servProvider) {
			this.servProvider = servProvider;
		}
		public void ClearUndoStack() {
			UndoEngineImpl undoEngine = servProvider.GetService(typeof(UndoEngine)) as UndoEngineImpl;
			if(undoEngine != null) undoEngine.Clear();
		}
	}
	public class UndoEngineImpl : UndoEngine {
		#region inner classes
		class RestoreSubreportAction {
			MemoryStream stream = new MemoryStream();
			string name;
			public RestoreSubreportAction(XtraReport report) {
				report.SaveLayoutInternal(stream);
				name = report.Site.Name;
			}
			public string Name { get { return name; } }
			public void Undo(XtraReport report) {
				report.LoadLayoutInternal(stream);
			}
		}
		class XRUndoUnit : UndoUnit {
			Dictionary<string, RestoreSubreportAction> restoreSubreportsActions = new Dictionary<string,RestoreSubreportAction>();
			public XRUndoUnit(UndoEngine engine, string name)
				: base(engine, name) {
			}
			bool IsSubreportSource(IComponent component) {
				IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
				XtraReport report = component as XtraReport;
				return report != null && designerHost != null && report != designerHost.RootComponent;
			}
			RestoreSubreportAction CreateRestoreSubreportAction(IComponent component) {
				try {
					if(IsSubreportSource(component) && component.Site != null && !string.IsNullOrEmpty(component.Site.Name))
						return new RestoreSubreportAction((XtraReport)component);
				}
				catch { }
				return null;
			}
			public override void ComponentRemoving(ComponentEventArgs e) {
				if(e.Component is XtraReport) {
					IDesignerHost designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
					if(designerHost != null && designerHost.RootComponent == e.Component)
						return;
				}
				RestoreSubreportAction action = CreateRestoreSubreportAction(e.Component);
				if(action != null && !restoreSubreportsActions.ContainsKey(action.Name))
					restoreSubreportsActions.Add(action.Name, action);
				base.ComponentRemoving(e);
			}
			public void ComponentAdded(IComponent component) {
				if(!UndoEngine.UndoInProgress || component.Site == null)
					return;
				XtraReport report = component as XtraReport;
				if(report == null)
					return;
				if(!restoreSubreportsActions.ContainsKey(report.Site.Name))
					return;
				RestoreSubreportAction action = restoreSubreportsActions[report.Site.Name];
				if(action == null)
					return;
				action.Undo(report);
			}
		}
		#endregion
		List<UndoEngine.UndoUnit> undoUnitList = new List<UndoUnit>();
		int currentPos;
#if DEBUGTEST
		internal int Test_UndoUnitCount {
			get { return undoUnitList.Count; }
		}
		internal int Test_UndoUnitPosition {
			get { return currentPos; }
		}
#endif
		public UndoEngineImpl(IServiceProvider provider) : base(provider) {
			UpdateUndoRedoMenuCommandsStatus();
			IComponentChangeService componentChangeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(componentChangeService != null)
				componentChangeService.ComponentAdded += new ComponentEventHandler(componentChangeService_ComponentAdded);
		}
		void componentChangeService_ComponentAdded(object sender, ComponentEventArgs e) {
			foreach(UndoEngine.UndoUnit unit in undoUnitList) {
				XRUndoUnit xrUndoUnit = unit as XRUndoUnit;
				if(xrUndoUnit != null)
					xrUndoUnit.ComponentAdded(e.Component);
			}
		}
		public void Clear() {
			undoUnitList.Clear();
			currentPos = 0;
			UpdateUndoRedoMenuCommandsStatus();
		}
		public void DoUndo() {
			if(CanUndo) {
				UndoEngine.UndoUnit undoUnit = undoUnitList[currentPos - 1];
				undoUnit.Undo();
				currentPos--;
			}
			UpdateUndoRedoMenuCommandsStatus();
		}
		public void DoRedo() {
			if(CanRedo) {
				UndoEngine.UndoUnit undoUnit = undoUnitList[currentPos];
				undoUnit.Undo();
				currentPos++;
			}
			UpdateUndoRedoMenuCommandsStatus();
		}
		public bool CanUndo {
			get {
				return currentPos > 0;
			}
		}
		public bool CanRedo {
			get {
				return currentPos < this.undoUnitList.Count;
			}
		}
		void UpdateUndoRedoMenuCommandsStatus() {
			IMenuCommandService menuCommandService = GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if(menuCommandService == null)
				return;
			CommandSetItem undoMenuCommand = menuCommandService.FindCommand(StandardCommands.Undo) as CommandSetItem;
			if(undoMenuCommand != null)
				undoMenuCommand.UpdateStatus();
			CommandSetItem redoMenuCommand = menuCommandService.FindCommand(StandardCommands.Redo) as CommandSetItem;
			if(redoMenuCommand != null)
				redoMenuCommand.UpdateStatus();
		}
		protected override void AddUndoUnit(UndoEngine.UndoUnit unit) {
			undoUnitList.RemoveRange(currentPos, undoUnitList.Count - currentPos);
			undoUnitList.Add(unit);
			currentPos = undoUnitList.Count;
			UpdateUndoRedoMenuCommandsStatus();
		}
		protected override UndoEngine.UndoUnit CreateUndoUnit(string name, bool primary) {
			return new XRUndoUnit(this, name);
		}
		protected override void DiscardUndoUnit(UndoEngine.UndoUnit unit) {
			undoUnitList.Remove(unit);
			base.DiscardUndoUnit(unit);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				IComponentChangeService componentChangeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				if(componentChangeService != null)
					componentChangeService.ComponentAdded -= new ComponentEventHandler(componentChangeService_ComponentAdded);
			}			
			base.Dispose(disposing);
		}
	}
}
