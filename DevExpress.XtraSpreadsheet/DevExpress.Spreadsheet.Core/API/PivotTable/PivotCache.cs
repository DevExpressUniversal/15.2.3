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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet {
	#region PivotCacheType
	public enum PivotCacheType {
		Consolidation = DevExpress.XtraSpreadsheet.Model.PivotCacheType.Consolidation,
		External = DevExpress.XtraSpreadsheet.Model.PivotCacheType.External,
		Scenario = DevExpress.XtraSpreadsheet.Model.PivotCacheType.Scenario,
		Worksheet = DevExpress.XtraSpreadsheet.Model.PivotCacheType.Worksheet
	}
	#endregion
	public interface PivotCache {
		PivotCacheType CacheType { get; }
		Range SourceRange { get; }
		void Refresh();
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Utils;
	using DevExpress.Spreadsheet;
	#region NativePivotCache
	partial class NativePivotCache : NativeObjectBase, PivotCache {
		#region Fields
		readonly Model.PivotCache modelPivotCache;
		readonly NativeWorkbook nativeWorkbook;
		#endregion
		public NativePivotCache(Model.PivotCache modelPivotCache, NativeWorkbook nativeWorkbook) {
			Guard.ArgumentNotNull(modelPivotCache, "modelPivotCache");
			Guard.ArgumentNotNull(nativeWorkbook, "nativeWorkbook");
			this.modelPivotCache = modelPivotCache;
			this.nativeWorkbook = nativeWorkbook;
		}
		protected internal Model.PivotCache ModelCache { get { return modelPivotCache; } }
		#region PivotCache Members
		public PivotCacheType CacheType {
			get { 
				CheckValid();
				return (PivotCacheType)modelPivotCache.Source.Type;
			}
		}
		public Range SourceRange {
			get {
				CheckValid();
				if (CacheType != PivotCacheType.Worksheet)
					return null; 
				Model.PivotCacheSourceWorksheet source = modelPivotCache.Source as Model.PivotCacheSourceWorksheet;
				Model.CellRange modelRange = source.GetRange(modelPivotCache.DocumentModel.DataContext);
				if (modelRange == null)
					return null;
				NativeWorksheet nativeWorksheet = nativeWorkbook.Worksheets[modelRange.Worksheet.Name] as NativeWorksheet;
				return new NativeRange(modelRange, nativeWorksheet);
			}
		}
		public void Refresh() {
			CheckValid();
			modelPivotCache.Refresh(ApiErrorHandler.Instance);
		}
		#endregion
	}
	#endregion
}
