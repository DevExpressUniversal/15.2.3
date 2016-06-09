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

using System.ComponentModel;
using DevExpress.Xpf.Editors;
#if SILVERLIGHT
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.Grid {
	public class EditorEventArgs : EditorEventArgsBase {
		GridControl Grid { get { return (GridControl)view.DataControl; } }
		public object Row { get { return Grid.GetRow(RowHandle); } }
		public new GridColumn Column { get { return (GridColumn)base.Column; } }
		public EditorEventArgs(GridViewBase view, int rowHandle, GridColumn column, IBaseEdit editor)
			: base(TableView.ShownEditorEvent, view, rowHandle, column) {
			Editor = editor;
		}
		public IBaseEdit Editor { get; private set; }
	}
	public class ShowingEditorEventArgs : ShowingEditorEventArgsBase {
		GridControl Grid { get { return (GridControl)view.DataControl; } }
		public object Row { get { return Grid.GetRow(RowHandle); } }
		public new GridColumn Column { get { return (GridColumn)base.Column; } }
		public ShowingEditorEventArgs(GridViewBase view, int rowHandle, GridColumn column)
			: base(TableView.ShowingEditorEvent, view, rowHandle, column) {
		}
	}
	public delegate void ShowingEditorEventHandler(object sender, ShowingEditorEventArgs e);
	public delegate void EditorEventHandler(object sender, EditorEventArgs e);
}
