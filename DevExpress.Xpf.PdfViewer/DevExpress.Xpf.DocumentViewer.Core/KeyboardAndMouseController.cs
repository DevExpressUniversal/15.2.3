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

using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.DocumentViewer {
	public class KeyboardAndMouseController {
		protected readonly DocumentPresenterControl presenter;
		protected CommandProvider CommandProvider { get { return presenter.ActualDocumentViewer.ActualCommandProvider; } }
		protected DocumentViewerPanel ItemsPanel { get { return presenter.ItemsPanel; } }
		protected NavigationStrategy NavigationStrategy { get { return presenter.NavigationStrategy; } }
		public KeyboardAndMouseController(DocumentPresenterControl documentPresenter) {
			presenter = documentPresenter;
		}
		public virtual void ProcessKeyDown(KeyEventArgs e) {
			switch(e.Key) {
				case Key.Up:
					CommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.LineUp));
					break;
				case Key.Down:
					CommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.LineDown));
					break;
				case Key.Left:
					CommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.LineLeft));
					break;
				case Key.Right:
					CommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.LineRight));
					break;
				case Key.PageUp:
					CommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.PageUp));
					break;
				case Key.PageDown:
					CommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.PageDown));
					break;
				case Key.Home:
					CommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.Home));
					break;
				case Key.End:
					CommandProvider.With(x => x.ScrollCommand).Do(x => x.Execute(ScrollCommand.End));
					break;
			}
		}
		public virtual void ProcessMouseLeftButtonUp(MouseButtonEventArgs e) { }
		public virtual void ProcessMouseLeftButtonDown(MouseButtonEventArgs e) { }
		public virtual void ProcessMouseRightButtonDown(MouseButtonEventArgs e) { }
		public virtual void ProcessMouseMove(MouseEventArgs e) { }
		public virtual void ProcessMouseWheel(MouseWheelEventArgs e) {
			if (!KeyboardHelper.IsControlPressed)
				return;
			var anchorPoint = e.GetPosition(ItemsPanel);
			NavigationStrategy.ZoomToAnchorPoint(e.Delta > 0, anchorPoint);
			e.Handled = true;
		}
	}
}
