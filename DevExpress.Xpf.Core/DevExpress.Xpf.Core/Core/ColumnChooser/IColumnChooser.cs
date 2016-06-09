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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public interface IColumnChooser {
		void Show();
		void Hide();
		void ApplyState(IColumnChooserState state);
		void SaveState(IColumnChooserState state);
		UIElement TopContainer { get; }
		void Destroy();
	}
	public interface IColumnChooserFactory {
		IColumnChooser Create(Control owner);
	}
	public interface IColumnChooserState {
	}
	public interface IVisualOwner : IInputElement {
		void AddChild(Visual child);
		void RemoveChild(Visual child);
	}
	public interface ILogicalOwner : IInputElement {
		bool IsLoaded { get; }
		event RoutedEventHandler Loaded;
		double ActualWidth { get; }
		double ActualHeight { get; }
		void AddChild(object child);
		void RemoveChild(object child);
	}
}
