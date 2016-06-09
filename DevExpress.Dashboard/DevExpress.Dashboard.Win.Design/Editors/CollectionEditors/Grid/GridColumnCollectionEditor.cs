#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native;
using DevExpress.Utils.UI;
namespace DevExpress.DashboardWin.Design {
	public class GridColumnCollectionEditor : Utils.UI.CollectionEditor {
		public GridColumnCollectionEditor(Type type)
			: base(type) {
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			return new GridColumnCollectionEditorForm(serviceProvider, this);
		}
		protected override string GetItemName(object item, int index) {
			string name = base.GetItemName(item, index);
			if(string.IsNullOrEmpty(name)) {
				name = item.GetType().Name;
			}
			return name;
		}
	}
	class GridColumnCollectionEditorForm : CollectionEditorForm {
		const string GridColumnsTitle = "Grid Columns";
		readonly static GridColumnMenuController menuController = new GridColumnMenuController();
		public override string Text { get { return GridColumnsTitle; } }
		protected override MenuItemController MenuItemController { get { return menuController; } }
		public GridColumnCollectionEditorForm(IServiceProvider provider, Utils.UI.CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
		}
	}
	class GridColumnMenuController : MenuItemController {
		const int GridColumnCount = 4;
		static Type[] GridColumnTypesList = new Type[GridColumnCount] {
			typeof(GridDimensionColumn),
			typeof(GridMeasureColumn),
			typeof(GridDeltaColumn),
			typeof(GridSparklineColumn)
		};
		static string[] GridColumnNamesList = new string[GridColumnCount] {
			ActionNames.Dimension,
			ActionNames.Measure,
			ActionNames.Delta,
			ActionNames.Sparkline
		};
		protected override int Count { get { return GridColumnCount; } }
		protected override Type[] TypesList { get { return GridColumnTypesList; } }
		protected override string[] NamesList { get { return GridColumnNamesList; } }
		public GridColumnMenuController()
			: base() {
		}
	}
}
