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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class PropertyNameFilterClient : IFilterClient {
		readonly PropertyNameFilterObject filterObject;
		public PropertyNameFilterClient(DataViewBase view) {
			filterObject = new PropertyNameFilterObject(view);
		}
		public object GetObject(RowHandle handle) {
			filterObject.Initialize(handle);
			return filterObject;
		}
		public object GetDefaultObject() {
			return filterObject;
		}
	}
	public class PropertyNameFilterObject {
		DataViewBase View;
		RowHandle handle;
		public PropertyNameFilterObject(DataViewBase view) {
			this.View = view;
		}
		public string Name { get { return View.GetDisplayName(handle); } }
		public void Initialize(RowHandle handle) {
			this.handle = handle;
		}
	}
	public class SortClient : ISortClient {
		public SortClient() {
		}
		IEnumerable<RowHandle> ISortClient.GetMappings(RowHandle parent, Func<string, string, RowHandle> addMappingSourceToTarget) {
			return EmptyEnumerable<RowHandle>.Instance;
		}
		IEnumerable<RowHandle> ISortClient.GetSortedChildren(RowHandle parent, IEnumerable<RowHandle> children) {
			return children;
		}
	}
}
