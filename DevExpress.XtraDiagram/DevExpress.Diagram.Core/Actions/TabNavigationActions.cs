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
using System.Linq;
using System.Text;
using System.Windows.Documents;
namespace DevExpress.Diagram.Core {
	public static class DXTabNavigationActions {
		public static void MoveSelection(this IDiagramControl diagram, LogicalDirection direction) {
			var nextItem = FindNextItem(diagram.PrimarySelection() ?? diagram.RootItem(), direction);
			if(nextItem != null)
				diagram.SelectItem(nextItem);
			else
				diagram.ClearSelection();
			diagram.BringSelectionIntoView();
		}
		static IDiagramItem FindNextItem(IDiagramItem item, LogicalDirection direction) {
			var flatten = (direction == LogicalDirection.Forward) ?
				(Func<IEnumerable<IDiagramItem>, Func<IDiagramItem, IList<IDiagramItem>>, Func<IList<IDiagramItem>, IDiagramItem, int>, IEnumerable<IDiagramItem>>)TreeExtensions.FlattenFromWithinForward :
				TreeExtensions.FlattenFromWithinBackward;
			return flatten(item.GetParentsIncludingSelf(), x => x.Controller.TabOrderProvider.GetChildren(), (list, x) => x.Owner().Controller.TabOrderProvider.IndexOf(x))
				.Skip(1)
				.Concat(item.Yield())
				.Where(x => x.IsTabStop && x.Controller.ActualCanSelect)
				.FirstOrDefault();
		}
	}
}
