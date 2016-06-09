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
#if SILVERLIGHT
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
#endif
namespace DevExpress.Xpf.WindowsUI.Navigation {
	public abstract class NavigationBaseEventArgs : EventArgs {
		public NavigationBaseEventArgs(object source, object parameter) {
			Source = source;
			Parameter = parameter;
		}
		public object Source { get; private set; }
		public object Parameter { get; private set; }
	}
	public class NavigationEventArgs : NavigationBaseEventArgs {
		public NavigationEventArgs(object source, object content, object parameter)
			: base(source, parameter) {
			this.Content = content;
		}
		public object Content { get; private set; }
	}
	public class NavigatingEventArgs : NavigationBaseEventArgs {
		public NavigatingEventArgs(object source, NavigationMode mode, object parameter)
			: base(source, parameter) {
			NavigationMode = mode;
		}
		public NavigationMode NavigationMode { get; private set; }
		public bool Cancel { get; set; }
	}
	public class NavigationFailedEventArgs : NavigationBaseEventArgs {
		public NavigationFailedEventArgs(object source, Exception ex)
			: base(source, null) {
			Exception = ex;
		}
		public Exception Exception { get; private set; }
	}
	public delegate void NavigatedEventHandler(object sender, NavigationEventArgs e);
	public delegate void NavigatingCancelEventHandler(object sender, NavigatingEventArgs e);
	public delegate void NavigationFailedEventHandler(object sender, NavigationFailedEventArgs e);
}
