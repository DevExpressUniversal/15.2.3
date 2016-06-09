#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Linq;
#if NETFX_CORE
using DevExpress.Mvvm.Native;
#endif
namespace DevExpress.Mvvm {
	public static class DocumentManagerServiceExtensions {
		[Obsolete("Use other extension methods.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static IDocument CreateDocument(this IDocumentManagerService service, string documentType, object parameter, object parentViewModel, bool useParameterAsViewModel) {
			VerifyService(service);
			if(useParameterAsViewModel)
				return service.CreateDocument(documentType, parameter, null, parentViewModel);
			else
				return service.CreateDocument(documentType, null, parameter, parameter);
		}
		public static IDocument CreateDocument(this IDocumentManagerService service, object viewModel) {
			VerifyService(service);
			return service.CreateDocument(null, viewModel, null, null);
		}
		public static IDocument CreateDocument(this IDocumentManagerService service, string documentType, object viewModel) {
			VerifyService(service);
			return service.CreateDocument(documentType, viewModel, null, null);
		}
		public static IDocument CreateDocument(this IDocumentManagerService service, string documentType, object parameter, object parentViewModel) {
			VerifyService(service);
			return service.CreateDocument(documentType, null, parameter, parentViewModel);
		}
		public static IEnumerable<IDocument> GetDocumentsByParentViewModel(this IDocumentManagerService service, object parentViewModel) {
			VerifyService(service);
			return service.Documents.Where(d => {
				var supportParentViewModel = d.Content as ISupportParentViewModel;
				return supportParentViewModel != null && object.Equals(supportParentViewModel.ParentViewModel, parentViewModel);
			});
		}
		public static IDocument FindDocument(this IDocumentManagerService service, object parameter, object parentViewModel) {
			VerifyService(service);
			return service.GetDocumentsByParentViewModel(parentViewModel).FirstOrDefault(d => {
				var supportParameter = d.Content as ISupportParameter;
				return supportParameter != null && object.Equals(supportParameter.Parameter, parameter);
			});
		}
		public static IDocument FindDocument(this IDocumentManagerService service, object viewModel) {
			VerifyService(service);
			return service.Documents.FirstOrDefault(d => {
				return d.Content == viewModel;
			});
		}
		public static IDocument FindDocumentById(this IDocumentManagerService service, object id) {
			VerifyService(service);
			return service.Documents.FirstOrDefault(x => object.Equals(x.Id, id));
		}
		public static IDocument FindDocumentByIdOrCreate(this IDocumentManagerService service, object id, Func<IDocumentManagerService, IDocument> createDocumentCallback) {
			VerifyService(service);
			IDocument document = service.FindDocumentById(id);
			if(document == null) {
				document = createDocumentCallback(service);
				document.Id = id;
			}
			return document;
		}
		public static void CreateDocumentIfNotExistsAndShow(this IDocumentManagerService service, ref IDocument documentStorage, string documentType, object parameter, object parentViewModel, object title) {
			VerifyService(service);
			if(documentStorage == null) {
				documentStorage = service.CreateDocument(documentType, parameter, parentViewModel);
				documentStorage.Title = title;
			}
			documentStorage.Show();
		}
		static void VerifyService(IDocumentManagerService service) {
			if(service == null)
				throw new ArgumentNullException("service");
		}
	}
}
