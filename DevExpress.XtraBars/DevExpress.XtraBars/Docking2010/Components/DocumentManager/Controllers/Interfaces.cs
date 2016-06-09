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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views {
	public interface IDockOperation : IDisposable {
		bool Canceled { get; }
	}
	internal interface IBaseViewControllerInternal {
		BaseDocument CreateAndInitializeDocument(Control control);
		BaseDocument AddDocument(Control control);
		BaseDocument AddFloatDocument(Control control);
		void RemoveDocument(Control control);
		BaseDocument RegisterDockPanel(Control control);
		void UnregisterDockPanel(Control control);
		void CreateBarDockingMenuItemCommands(BarDockingMenuItem documentListItem);
	}
	public interface IBaseViewController : DevExpress.Utils.Base.IBaseObject {
		DocumentManager Manager { get; }
		BaseView View { get; }
		bool AddDocument(BaseDocument document);
		bool RemoveDocument(BaseDocument document);
		bool Activate(BaseDocument document);
		bool Close(BaseDocument document);
		bool CloseAll();
		bool CloseAllDocumentsAndHosts();
		bool CloseAllButThis(BaseDocument document);
		bool FloatAll();
		bool Float(BaseDocument document);
		bool Float(BaseDocument document, Point floatLocation);
		bool Float(BaseDocument document, Point floatLocation, Size floatSize);
		bool Dock(BaseDocument document);
		IEnumerable<BaseViewControllerCommand> GetCommands(BaseDocument document);
		IEnumerable<BaseViewControllerCommand> GetCommands(BaseView view);
		bool ShowContextMenu(Point point);
		bool ShowContextMenu(BaseDocument document, Point point);
		bool Dock(Docking.DockPanel dockPanel);
		bool Float(Docking.DockPanel dockPanel);
		bool Execute(BaseViewControllerCommand command, object parameter);
		void ShowWindowsDialog();
		void ResetLayout();
		BaseView GetView(BaseDocument document);
	}
}
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public interface ITabbedViewController : IBaseViewController {
		bool Select(Document document);
		bool Dock(Document document, DocumentGroup group);
		bool Dock(Document document, DocumentGroup group, int insertIndex);
		bool Dock(Docking.DockPanel dockPanel, DocumentGroup group);
		bool Dock(Docking.DockPanel dockPanel, DocumentGroup group, int insertIndex);
		bool CreateNewDocumentGroup(Document document);
		bool CreateNewDocumentGroup(Document document, Orientation orientation);
		bool CreateNewDocumentGroup(Document document, Orientation orientation, int insertIndex);
		bool Move(Document document, int position);
		bool MoveToMainDocumentGroup(Document document);
		bool MoveToDocumentGroup(Document document, bool next);
		bool FloatAll(Document document);
		bool FloatAll(DocumentGroup group);
		void DockAll();
		bool ShowDocumentSelectorMenu(DocumentGroup group);
		bool ShowContextMenu(DocumentGroup group, Point point);
		bool SelectNextTab(bool forward);
		bool CloseAllButPinned();
	}
}
namespace DevExpress.XtraBars.Docking2010.Views.NativeMdi {
	public interface INativeMdiViewController : IBaseViewController {
		bool Cascade();
		bool TileVertical();
		bool TileHorizontal();
		bool MinimizeAll();
		bool ArrangeIcons();
		bool RestoreAll();
	}
}
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	internal interface IWindowsUIViewControllerInternal {
		bool ProcessCheckedTiles(TileContainer container, Func<BaseTile, bool> action);
		bool PrepareAssociatedContentContainer(BaseTile tile, IContentContainer associatedContainer);
	}
	public interface IWindowsUIViewController : IBaseViewController {
		bool Back();
		bool Home();
		bool Exit();
		bool Activate(BaseTile tile);
		bool Activate(IContentContainer container);
		bool Overview(IContentContainer container);
		bool Rotate(SplitGroup group);
		bool Flip(SplitGroup group);
		bool AddTile(Document document);
		bool RemoveTile(Document document);
		bool EnableFullScreenMode(bool enable);		
	}
	public interface INavigationContext : IDisposable {
		WindowsUIView View { get; }
		IContentContainer Source { get; }
		IContentContainer Target { get; }
		object Tag { get; set; }
		object Parameter { get; set; }
	}
	public interface INavigationArgs {
		WindowsUIView View { get; }
		Document Document { get; }
		IContentContainer Source { get; }
		IContentContainer Target { get; }
		ContextualZoomLevel SourceContextualZoomLevel { get; }
		ContextualZoomLevel TargetContextualZoomLevel { get; }
		NavigationMode NavigationMode { get; }
		object Tag { get; }
		object Parameter { get; set; }
	}
	public interface ISupportNavigation {
		void OnNavigatedTo(INavigationArgs args);
		void OnNavigatedFrom(INavigationArgs args);
	}
	public enum NavigationMode {
		New,
		Back,
		Forward,
		Refresh
	}
}
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	internal interface IWidgetViewControllerInternal {
		void OnOrientationChanged();
	}
	public interface IWidgetViewController : IBaseViewController {
		bool Dock(Document document, StackGroup group);
		void Maximize(Document document);
		void Restore(Document document);
	}
}
