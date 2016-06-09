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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DevExpress.Xpf.Printing {
	public abstract class CustomCursor : ContentControl {
		public abstract ImageSource ImageSource { get; }
		public abstract Point HotSpot { get; }
		public CustomCursor() {
			Content = new Image { Source = ImageSource, Stretch = Stretch.None };
		}
		protected static BitmapImage LoadCursorFile(string cursorFilePath, string rootNamespace) {
			cursorFilePath = GetUriString(cursorFilePath, rootNamespace);
#if DEBUGTEST
			UriKind uriKind = UriKind.Absolute;
#else
			UriKind uriKind = UriKind.Relative;
#endif
			Uri cursorFileUri = new Uri(cursorFilePath, uriKind);
			return new BitmapImage(cursorFileUri);
		}
		static string GetUriString(string cursorFilePath, string rootNamespace) {
#if DEBUGTEST
			const string formatString = "pack://application:,,,/{0}{1};component/{2}";
#else
			const string formatString = "/{0}{1};component/{2}";
#endif
			return string.Format(formatString, rootNamespace, AssemblyInfo.VSuffix, cursorFilePath);
		}
	}
}
