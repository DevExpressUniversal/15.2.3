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
using System.ComponentModel;
using System.Linq;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DataAccess.Native.Data;
namespace DevExpress.DashboardExport {
	public class GridPrinterSourceListWrapper {
		readonly ReadOnlyTypedList sourceList;
		readonly int scrollIndex;
		public int Count { get { return sourceList.Count - scrollIndex; } }
		public object this[int index] { get { return sourceList[CorrectIndex(index)]; } }
		public GridPrinterSourceListWrapper(ReadOnlyTypedList sourceList, int scrollIndex) {
			this.sourceList = sourceList;
			this.scrollIndex = scrollIndex > 0 ? scrollIndex : 0;
		}
		public PropertyDescriptor GetProperty(string property) {
			return string.IsNullOrEmpty(property) || !sourceList.Properties.Contains(property) ? null : sourceList.Properties[property];
		}
		public object GetPropertyValue(string property, int index) {
			PropertyDescriptor descriptor = GetProperty(property);
			return descriptor == null ? null : descriptor.GetValue(this[index]);
		}
		public IList<string> GetAllDisplayTexts(string dataId) {
			PropertyDescriptor descriptor = GetProperty(dataId + GridMultiDimensionalDataSource.DisplayTextPostfix);
			List<string> res = new List<string>();
			foreach(object row in sourceList)
				res.Add((string)descriptor.GetValue(row));
			return res;
		}
		public int CorrectIndex(int index) {
			return index + scrollIndex;
		}
	}
}
