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
using System.Linq;
using System.Text;
using DevExpress.Internal.DXWindow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
namespace DevExpress.DemoData.Helpers {
	public enum MessageBoxHelperResult { OK, Ignore }
	public interface IMessageBoxHelperSupport {
		MessageBoxHelperResult Result { get; }
		event EventHandler Close;
	}
	public static class MessageBoxHelper {
		public static MessageBoxHelperResult Show(UIElement content) {
			Window dialog = new Window() { ResizeMode = ResizeMode.NoResize, WindowStyle = WindowStyle.None, ShowInTaskbar = false };
			dialog.Owner = Application.Current == null ? null : Application.Current.MainWindow;
			dialog.WindowStartupLocation = dialog.Owner == null ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner;
			dialog.SizeToContent = SizeToContent.WidthAndHeight;
			dialog.Content = content;
			IMessageBoxHelperSupport mbhs = content as IMessageBoxHelperSupport;
			if(mbhs != null)
				mbhs.Close += (s, e) => dialog.Close();
			dialog.ShowDialog();
			return mbhs == null ? MessageBoxHelperResult.OK : mbhs.Result;
		}
	}
}
