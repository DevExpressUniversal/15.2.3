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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Editors;
using Expression = System.Linq.Expressions.Expression;
using System.Windows.Input;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Diagram {
	public static class UseCollectionEditorAttributeFluentAPIExtension {
		public static PropertyMetadataBuilder<T, TProperty> UseCollectionEditor<T, TProperty>(this PropertyMetadataBuilder<T, TProperty> builder) {
			IAttributeBuilderInternal<PropertyMetadataBuilder<T, TProperty>> builderInternal = builder;
			builderInternal.AddOrReplaceAttribute(new UseCollectionEditorAttribute());
			return builder;
		}
		public static ClassMetadataBuilder<T> UseCollectionEditor<T>(this ClassMetadataBuilder<T> builder) {
			IAttributeBuilderInternal<ClassMetadataBuilder<T>> builderInternal = builder;
			builderInternal.AddOrReplaceAttribute(new UseCollectionEditorAttribute());
			return builder;
		}
	}
	public class CollectionDialogService : DialogService, IDialogService { 
		UICommand IDialogService.ShowDialog(IEnumerable<UICommand> dialogCommands, string title, string documentType, object viewModel, object parameter, object parentViewModel) {
			var editorValue = (UITypeEditorValue)viewModel;
			if(editorValue.OriginalValue == null) return null; 
			var collectionModelType = editorValue.OriginalValue.GetType();
			var genericArguments = collectionModelType.GetGenericArguments();
			var editMethodGeneric = typeof(CollectionDialogService).GetMethod("EditCollection", BindingFlags.NonPublic | BindingFlags.Instance);
			var editMethod = editMethodGeneric.MakeGenericMethod(genericArguments[1], genericArguments[2]);
			return (UICommand)editMethod.Invoke(this, new object[] { editorValue.OriginalValue, documentType, editorValue, dialogCommands, title });
		}
		internal UICommand EditCollection<TList, TItem>(CollectionModel<IDiagramItem, TList, TItem> collectionModel, string documentType, UITypeEditorValue editorValue, IEnumerable<UICommand> dialogCommands, string title) where TList : IList<TItem> {
			UICommand result = null;
			var okResult = dialogCommands.FirstOrDefault();
			EditCollectionAction.Edit(collectionModel, model => {
				object editValue = model;
				object view = CreateAndInitializeView(documentType, new UITypeEditorValue(null, null, model, editorValue.Content), null, null, this);
				result = ShowDialog(dialogCommands, title, view);
				return okResult == null || result == okResult;
			});
			return result;
		}
	}
}
