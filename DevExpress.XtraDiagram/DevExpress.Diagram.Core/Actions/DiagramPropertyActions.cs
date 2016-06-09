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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
namespace DevExpress.Diagram.Core {
	public static class DiagramPropertyActions {
		public static void SetMultipleItemsPropertyValuesWithSelectionRestore<T>(this IDiagramControl diagram, IEnumerable<IDiagramItem> items, PropertyDescriptor property, IEnumerable<object> values, bool allowMerge, Func<IDiagramItem, T> getComponent) {
			diagram.ExecuteWithSelectionRestore(transaction => {
				transaction.SetMultipleItemsPropertyValues(items, property, values, x => x.GetFinder(), getComponent);
			}, null, allowMerge);
		}
		public static void ResetMultipleItemsPropertyValueWithSelectionRestore<T>(this IDiagramControl diagram, IEnumerable<IDiagramItem> items, PropertyDescriptor property, Func<IDiagramItem, T> getComponent) {
			diagram.ExecuteWithSelectionRestore(transaction => {
				transaction.ResetMultipleItemsPropertyValues(items, property, x => x.GetFinder(), getComponent);
			});
		}
		public static void SetItemProperty(this Transaction transaction, IDiagramItem item, object value, PropertyDescriptor property) {
			transaction.SetProperty(value, item, property, IDiagramItemExtensions.GetFinder);
		}
		static readonly PropertyDescriptor ControllerBoundsProperty
			= ProxyPropertyDescriptor.Create(ExpressionHelper.GetProperty((DiagramItemController x) => x.Bounds), (IDiagramItem item) => item.Controller);
		public static void SetItemBounds(this Transaction transaction, IDiagramItem item, Rect value) {
			transaction.SetItemProperty(item, value, ControllerBoundsProperty);
		}
		}
}
