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

using System.Windows.Forms;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public abstract class TabbedViewControllerCommand : BaseViewControllerCommand {
		protected override bool CanExecuteCore(object parameter) {
			Document document = parameter as Document;
			return document != null && !document.IsDisposing;
		}
		#region static
		public static readonly TabbedViewControllerCommand Horizontal = new HorizontalCommand();
		public static readonly TabbedViewControllerCommand Vertical = new VerticalCommand();
		public static readonly TabbedViewControllerCommand NewDocumentGroup = new NewDocumentGroupCommand();
		public static readonly TabbedViewControllerCommand NewHorizontalDocumentGroup = new NewHorizontalDocumentGroupCommand();
		public static readonly TabbedViewControllerCommand NewVerticalDocumentGroup = new NewVerticalDocumentGroupCommand();
		public static readonly TabbedViewControllerCommand MoveToNextDocumentGroup = new MoveToNextDocumentGroupCommand();
		public static readonly TabbedViewControllerCommand MoveToPrevDocumentGroup = new MoveToPrevDocumentGroupCommand();
		public static readonly TabbedViewControllerCommand MoveToMainDocumentGroup = new MoveToMainDocumentGroupCommand(); 
		public static readonly TabbedViewControllerCommand PinTab = new PinTabCommand();
		public static readonly TabbedViewControllerCommand UnpinTab = new UnpinTabCommand();
		public static readonly TabbedViewControllerCommand CloseAllButPinned = new CloseAllButPinnedCommand();
		public static readonly TabbedViewControllerCommand DockAll = new DockAllCommand();
		public static readonly TabbedViewControllerCommand FloatAllDocumentGroup = new FloatAllDocumentGroupCommand();
		#endregion static
		#region Commands
		[CommandGroup("DocumentGroup", Order = 2)]
		class NewDocumentGroupCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandNewDocumentGroup; }
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((ITabbedViewController)controller).CreateNewDocumentGroup((Document)parameter);
			}
		}
		[CommandGroup("DocumentGroup", Order = 2, Visibility = CommandGroupVisibility.Always)]
		class NewHorizontalDocumentGroupCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandNewHorizontalDocumentGroup; }
			}
			protected override bool CanExecuteCore(object parameter) {
				if(base.CanExecuteCore(parameter))
					return (parameter as Document).Properties.CanReorderTab;
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((ITabbedViewController)controller).CreateNewDocumentGroup((Document)parameter, Orientation.Horizontal);
			}
		}
		[CommandGroup("DocumentGroup", Order = 2, Visibility = CommandGroupVisibility.Always)]
		class NewVerticalDocumentGroupCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandNewVerticalDocumentGroup; }
			}
			protected override bool CanExecuteCore(object parameter) {
				if(base.CanExecuteCore(parameter))
					return (parameter as Document).Properties.CanReorderTab;
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((ITabbedViewController)controller).CreateNewDocumentGroup((Document)parameter, Orientation.Vertical);
			}
		}
		[CommandGroup("DocumentGroup", Order = 3, Index = 1, Visibility = CommandGroupVisibility.Always)]
		class MoveToNextDocumentGroupCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandMoveToNextDocumentGroup; }
			}
			protected override bool CanExecuteCore(object parameter) {
				if(base.CanExecuteCore(parameter))
					return (parameter as Document).Properties.CanReorderTab;
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((ITabbedViewController)controller).MoveToDocumentGroup((Document)parameter, true);
			}
		}
		[CommandGroup("DocumentGroup", Order = 3, Index = 1, Visibility = CommandGroupVisibility.Always)]
		class MoveToPrevDocumentGroupCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandMoveToPrevDocumentGroup; }
			}
			protected override bool CanExecuteCore(object parameter) {
				if(base.CanExecuteCore(parameter))
					return (parameter as Document).Properties.CanReorderTab;
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((ITabbedViewController)controller).MoveToDocumentGroup((Document)parameter, false);
			}
		}
		[CommandGroup("DocumentGroup", Order = 3, Index = 1, Visibility = CommandGroupVisibility.Always)]
		class MoveToMainDocumentGroupCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandMoveToMainDocumentGroup; }
			}
			protected override bool CanExecuteCore(object parameter) {
				Document document = parameter as Document;
				return document != null && !document.IsDisposing && DocumentsHostContext.IsChild(document.Manager);
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((ITabbedViewController)controller).MoveToMainDocumentGroup((Document)parameter);
			}
		}
		[CommandGroup("Document", Order = 1, Index = 0, Visibility = CommandGroupVisibility.Always)]
		public class PinTabCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandPinTab; }
			}
			protected override bool CanExecuteCore(object parameter) {
				Document document;
				if(Check(parameter, out document)) {
					return document.Properties.CanPin && !document.Pinned;
				}
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				Document document = parameter as Document;
				document.Pinned = true;
			}
		}
		[CommandGroup("Document", Order = 1, Index = 0, Visibility = CommandGroupVisibility.Always)]
		public class UnpinTabCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandUnpinTab; }
			}
			protected override bool CanExecuteCore(object parameter) {
				Document document;
				if(Check(parameter, out document)) {
					return document.Properties.CanPin && document.Pinned;
				}
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				Document document = parameter as Document;
				document.Pinned = false;
			}
		}
		[CommandGroup("Common", Order = 0, Index = 2)]
		public class CloseAllButPinnedCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandCloseAllButPinned; }
			}
			protected override bool CanExecuteCore(object parameter) {
				Document document;
				if(Check(parameter, out document)) {
					TabbedView view = document.Manager.View as TabbedView;
					return (view != null) && (view.Documents.Count > 1) && view.GetPinnedDocumentCount() > 0;
				}
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((ITabbedViewController)controller).CloseAllButPinned();
			}
		}
		[CommandGroup("TabbedView")]
		class HorizontalCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandHorizontalOrientation; }
			}
			protected override bool CanExecuteCore(object parameter) {
				TabbedView view;
				return Check(parameter, out view);
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((TabbedView)parameter).Orientation = Orientation.Horizontal;
			}
		}
		[CommandGroup("TabbedView")]
		class VerticalCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandVerticalOrientation; }
			}
			protected override bool CanExecuteCore(object parameter) {
				TabbedView view;
				return Check(parameter, out view);
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((TabbedView)parameter).Orientation = Orientation.Vertical;
			}
		}
		[CommandGroup("Additional")]
		class DockAllCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandDockAll; }
			}
			protected override bool CanExecuteCore(object parameter) {
				BaseView view;
				if(!Check(parameter, out view)) {
					BaseDocument document;
					if(Check(parameter, out document))
						view = document.Manager.View;
				}
				return (view != null) && view.FloatDocuments.Count > 0;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((TabbedView)parameter).Controller.DockAll();
			}
		}
		[CommandGroup("Additional", Order = 1, Index = 1, Visibility = CommandGroupVisibility.Always)]
		class FloatAllDocumentGroupCommand : TabbedViewControllerCommand {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandFloatAll; }
			}
			protected override bool CanExecuteCore(object parameter) {
				Document document;
				if(Check(parameter, out document)) {
					BaseView view = document.Manager.View ?? GetView();
					Document[] documents = (document.Parent != null) ? document.Parent.Items.ToArray() : new Document[0];
					return view != null && view.CanFloatAll(documents);
				}
				return false;
			}
			protected override void ExecuteCore(IBaseViewController controller, object parameter) {
				((ITabbedViewController)controller).FloatAll((Document)parameter);
			}
		}
		#endregion Commands
	}
}
