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
using DevExpress.DashboardCommon;
using DevExpress.Utils.UI;
namespace DevExpress.DashboardWin.Design {
	public class ChoroplethMapCollectionEditor : Utils.UI.CollectionEditor {
		public ChoroplethMapCollectionEditor(Type type)
			: base(type) {
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			return new ChoroplethMapCollectionEditorForm(serviceProvider, this);
		}
		protected override string GetItemName(object item, int index) {
			string name = base.GetItemName(item, index);
			if(string.IsNullOrEmpty(name)) {
				name = item.GetType().Name;
			}
			return name;
		}
	}
	class ChoroplethMapCollectionEditorForm : CollectionEditorForm {
		const string MapsTitle = "Maps";
		readonly static ChoroplethMapMenuController menuController = new ChoroplethMapMenuController();
		public override string Text { get { return MapsTitle; } }
		protected override MenuItemController MenuItemController { get { return menuController; } }
		public ChoroplethMapCollectionEditorForm(IServiceProvider provider, Utils.UI.CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
		}
	}
	class ChoroplethMapMenuController : MenuItemController {
		const int ChoroplethMapCount = 2;
		static Type[] ChoroplethMapTypesList = new Type[ChoroplethMapCount] {
			typeof(ValueMap),
			typeof(DeltaMap)
		};
		static string[] ChoroplethMapNamesList = new string[ChoroplethMapCount] {
			ActionNames.Value,
			ActionNames.Delta
		};
		protected override int Count { get { return ChoroplethMapCount; } }
		protected override Type[] TypesList { get { return ChoroplethMapTypesList; } }
		protected override string[] NamesList { get { return ChoroplethMapNamesList; } }
		public ChoroplethMapMenuController()
			: base() {
		}
	}
}
