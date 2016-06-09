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
using System.Collections.Generic;
using DevExpress.DashboardWin.Bars;
namespace DevExpress.DashboardWin.Native {
	public abstract class DashboardDesignerPopupMenuCreator {
		protected abstract void FillBarItemTypes(List<Type> barItemTypes);
		public List<Type> GetBarItemTypes() {
			List<Type> barItemTypes = new List<Type>();
			FillBarItemTypes(barItemTypes);
			return barItemTypes;
		}
	}
	public class ShowItemCaptionBarItemPopupMenuCreator : DashboardDesignerPopupMenuCreator {
		protected override void FillBarItemTypes(List<Type> barItemTypes) {
			barItemTypes.Add(typeof(ShowItemCaptionBarItem));
		}
	}
	public class CommonDashboardItemBarItemsPopupMenuCreator : DashboardDesignerPopupMenuCreator {
		protected override void FillBarItemTypes(List<Type> barItemTypes) {
			barItemTypes.Add(typeof(DuplicateItemBarItem));
			barItemTypes.Add(typeof(DeleteItemBarItem));
			barItemTypes.Add(typeof(ConvertDashboardItemTypeBarItem));
			barItemTypes.Add(typeof(RemoveDataItemsBarItem));
			barItemTypes.Add(typeof(TransposeItemBarItem));
			barItemTypes.Add(typeof(EditRulesBarItem));
		}
	}
	public class EditNamesBarItemsPopupMenuCreator : DashboardDesignerPopupMenuCreator {
		protected override void FillBarItemTypes(List<Type> barItemTypes) {
			barItemTypes.Add(typeof(EditItemNamesBarItem));
		}
	}
	public class FilterOperationsBarItemsPopupMenuCreator : DashboardDesignerPopupMenuCreator {
		protected override void FillBarItemTypes(List<Type> barItemTypes) {
			barItemTypes.Add(typeof(EditFilterBarItem));
			barItemTypes.Add(typeof(ClearFilterBarItem));
		}
	}
	public class CommonMapBarItemsPopupMenuCreator : DashboardDesignerPopupMenuCreator {
		protected override void FillBarItemTypes(List<Type> barItemTypes) {
			barItemTypes.Add(typeof(MapLoadBarItem));
			barItemTypes.Add(typeof(MapImportBarItem));
			barItemTypes.Add(typeof(MapDefaultShapefileBarItem));
		}
	}
	public class MapFullExtentBarItemPopupMenuCreator : DashboardDesignerPopupMenuCreator {
		protected override void FillBarItemTypes(List<Type> barItemTypes) {
			barItemTypes.Add(typeof(MapFullExtentBarItem));
		}
	}
	public class ImageOperationsBarItemsPopupMenuCreator : DashboardDesignerPopupMenuCreator {
		protected override void FillBarItemTypes(List<Type> barItemTypes) {
			barItemTypes.Add(typeof(ImageLoadBarItem));
			barItemTypes.Add(typeof(ImageImportBarItem));
		}
	}
	public class TextBoxEditTextBarItemPopupMenuCreator : DashboardDesignerPopupMenuCreator {
		protected override void FillBarItemTypes(List<Type> barItemTypes) {
			barItemTypes.Add(typeof(TextBoxEditTextBarItem));
		}
	}
	public class DashboardItemGroupBarItemsPopupMenuCreator : DashboardDesignerPopupMenuCreator {
		protected override void FillBarItemTypes(List<Type> barItemTypes) {
			barItemTypes.Add(typeof(DeleteGroupBarItem));
		}
	}
}
