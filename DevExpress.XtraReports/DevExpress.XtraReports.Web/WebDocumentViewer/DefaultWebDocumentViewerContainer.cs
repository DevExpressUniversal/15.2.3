#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.IO;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
namespace DevExpress.XtraReports.Web.WebDocumentViewer {
	public static class DefaultWebDocumentViewerContainer {
		public static IServiceProvider Current { get; set; }
		public static void UseFileDocumentStorage(string workingDirectory) {
			if(!Directory.Exists(workingDirectory)) {
				Directory.CreateDirectory(workingDirectory);
			}
			var settings = new FileDocumentStorageSettings(workingDirectory);
			DevExpress.XtraReports.Web.Native.ClientControls.DefaultContainerHelper.Register(settings, Current, WebDocumentViewerBootstrapper.AppendRegistration);
			Register<IDocumentStorage, FileDocumentStorage>();
		}
		public static void UseEmptyReportHashCodeGenerator() {
			UseReportHashCodeGenerator<EmptyReportHashCodeGenerator>();
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public static void UseReportHashCodeGenerator<T>()
			where T : IReportHashCodeGenerator {
			Register<IReportHashCodeGenerator, T>();
		}
		public static void UseEmptyStoragesCleaner() {
			Register<IStoragesCleaner, EmptyStoragesCleaner>();
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public static void Register<T, TImpl>()
			where TImpl : T {
			DevExpress.XtraReports.Web.Native.ClientControls.DefaultContainerHelper.Register<T, TImpl>(Current, WebDocumentViewerBootstrapper.AppendRegistration);
		}
	}
}
