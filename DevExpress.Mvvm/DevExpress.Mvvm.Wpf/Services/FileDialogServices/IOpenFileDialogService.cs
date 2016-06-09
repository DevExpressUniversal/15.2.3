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
using System.ComponentModel;
using System.IO;
using DevExpress.Mvvm.Native;
namespace DevExpress.Mvvm {
	public interface IOpenFileDialogService {
		string Filter { get; set; }
		int FilterIndex { get; set; }
		bool ShowDialog(Action<CancelEventArgs> fileOK, string directoryName);
		IFileInfo File { get; }
		IEnumerable<IFileInfo> Files { get; }
#if !SILVERLIGHT
		string Title { get; set; }
#endif
	}
	public static class OpenFileDialogServiceExtensions {
		public static bool ShowDialog(this IOpenFileDialogService service) {
			VerifyService(service);
			return service.ShowDialog(null, null);
		}
		public static bool ShowDialog(this IOpenFileDialogService service, Action<CancelEventArgs> fileOK) {
			VerifyService(service);
			return service.ShowDialog(fileOK, null);
		}
		public static bool ShowDialog(this IOpenFileDialogService service, string directoryName) {
			VerifyService(service);
			return service.ShowDialog(null, directoryName);
		}
#if !SILVERLIGHT
		public static string GetFullFileName(this IOpenFileDialogService service) {
			VerifyService(service);
			return service.File.Return(x => x.GetFullName(), () => string.Empty);
		}
#endif
		static void VerifyService(IOpenFileDialogService service) {
			if(service == null)
				throw new ArgumentNullException("service");
		}
	}
}
