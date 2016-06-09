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
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Linq.Expressions;
using System.Windows.Documents;
using DevExpress.Diagram.Core;
using ResizeMode = DevExpress.Diagram.Core.ResizeMode;
using DevExpress.Internal;
using DevExpress.Utils;
using System.Collections.Specialized;
namespace DevExpress.Diagram.Core {
	public static class EditCollectionAction {
		public static void EditCollection<TList, TItem>(
			this IDiagramControl diagram,
			IDiagramItem item,
			Func<IDiagramItem, TList> getList,
			Func<EditCollectionHelper.StandaloneCollectionModel<IDiagramItem, TList, TItem>, bool> editAction
		) where TList : IList<TItem> {
			diagram.ExecuteWithSelectionRestore(transaction => {
				var context = diagram.Controller.CreateDiagramMultimodelContext(typeof(SelectionModel<>), typeof(SelectionModel<>), getList, action => action(transaction));
				EditCollectionHelper.EditCollection(item, getList, transaction, context, x => x.GetFinder(), editAction);
			}, allowMerge: false);
		}
		public static void Edit<TList, TItem>(this CollectionModel<IDiagramItem, TList, TItem> collectionModel, Func<EditCollectionHelper.StandaloneCollectionModel<IDiagramItem, TList, TItem>, bool> editAction) where TList : IList<TItem> {
			IMultiModel multiModel = collectionModel;
			var propertiesProvider = (PropertiesProvider<IDiagramItem, TList>)multiModel.PropertiesProvider;
			var diagramItem = propertiesProvider.Components.Single();
			var diagram = diagramItem.GetRootDiagram();
			var getList = propertiesProvider.Context.GetComponent;
			diagram.EditCollection(diagramItem, getList, editAction);
		}
	}
}
