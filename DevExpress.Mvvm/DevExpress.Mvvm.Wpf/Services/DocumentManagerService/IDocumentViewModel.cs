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
using System.Reflection;
namespace DevExpress.Mvvm {
	[Obsolete("Use the IDocumentContent interface instead.")]
	public interface IDocumentViewModel {
		bool Close();
		object Title { get; }
	}
	public interface IDocumentContent {
		IDocumentOwner DocumentOwner { get; set; }
		object Title { get; }
		void OnClose(CancelEventArgs e);
		void OnDestroy();
	}
	public interface IDocumentOwner {
		void Close(IDocumentContent documentContent, bool force = true);
	}
}
namespace DevExpress.Mvvm.Native {
	public static class DocumentViewModelHelper {
		public static void OnClose(object documentContentOrDocumentViewModel, CancelEventArgs e) {
			IDocumentContent documentContent = documentContentOrDocumentViewModel as IDocumentContent;
			if(documentContent != null) {
				documentContent.OnClose(e);
				return;
			}
#pragma warning disable 612, 618
			IDocumentViewModel documentViewModel = documentContentOrDocumentViewModel as IDocumentViewModel;
#pragma warning restore 612, 618
			if(documentViewModel != null) {
				e.Cancel = !documentViewModel.Close();
				return;
			}
		}
		public static void OnDestroy(object documentContentOrDocumentViewModel) {
			IDocumentContent documentContent = documentContentOrDocumentViewModel as IDocumentContent;
			if(documentContent != null)
				documentContent.OnDestroy();
		}
		public static bool IsDocumentContentOrDocumentViewModel(object viewModel) {
#pragma warning disable 612, 618
			return viewModel is IDocumentContent || viewModel is IDocumentViewModel;
#pragma warning restore 612, 618
		}
		public static bool TitlePropertyHasImplicitImplementation(object documentContentOrDocumentViewModel) {
			IDocumentContent documentContent = documentContentOrDocumentViewModel as IDocumentContent;
			if(documentContent != null)
				return ExpressionHelper.PropertyHasImplicitImplementation(documentContent, i => i.Title);
#pragma warning disable 612, 618
			IDocumentViewModel documentViewModel = documentContentOrDocumentViewModel as IDocumentViewModel;
			if(documentViewModel != null)
				return ExpressionHelper.PropertyHasImplicitImplementation(documentViewModel, i => i.Title);
#pragma warning restore 612, 618
			throw new ArgumentException("", "documentContentOrDocumentViewModel");
		}
		public static object GetTitle(object documentContentOrDocumentViewModel) {
			IDocumentContent documentContent = documentContentOrDocumentViewModel as IDocumentContent;
			if(documentContent != null)
				return documentContent.Title;
#pragma warning disable 612, 618
			IDocumentViewModel documentViewModel = documentContentOrDocumentViewModel as IDocumentViewModel;
			if(documentViewModel != null)
				return documentViewModel.Title;
#pragma warning restore 612, 618
			throw new ArgumentException("", "documentContentOrDocumentViewModel");
		}
	}
}
