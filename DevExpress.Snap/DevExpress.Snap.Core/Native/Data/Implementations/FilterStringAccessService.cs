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
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.Snap.Core.Native.Data.Implementations {
	public class FilterStringAccessService : IFilterStringAccessService {
		SnapDocumentModel documentModel;
		public FilterStringAccessService(SnapDocumentModel documentModel) {
			this.documentModel = documentModel;
		}
		public string GetFilterString() {
			return GetFilterString(GetParsedInfo());
		}
		SNListField GetParsedInfo() {
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			Field field = GetSelectedField();
			if (field == null)
				return null;
			return calculator.ParseField(documentModel.Selection.PieceTable, field) as SNListField;
		}
		Field GetSelectedField() {
			FieldController controller = new FieldController();
			return controller.FindFieldBySelection(documentModel.Selection).Parent;
		}
		public string GetFilterString(SNListField list) {
			IFieldDataAccessService dataAccessService = documentModel.GetService<IFieldDataAccessService>();
			if (dataAccessService == null)
				return String.Empty;
			FieldPathInfo fieldPathInfo = dataAccessService.FieldPathService.FromString(list.DataSourceName);
			FieldDataMemberInfoItem item = fieldPathInfo.DataMemberInfo.LastItem;
			if (item == null || !item.HasFilters)
				return String.Empty;
			return item.FilterProperties.Filters[item.FilterProperties.Filters.Count - 1];
		}
	}
}
