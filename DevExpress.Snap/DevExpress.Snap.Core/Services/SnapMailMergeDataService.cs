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

using System.Collections.Generic;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Services {
	public class SnapMailMergeDataService : MailMergeDataService {
		SnapDocumentModel model;
		readonly SnapMailMergeExportOptions options;
		int recordCount = -1;
		public SnapMailMergeDataService(RichEditDataControllerAdapterBase dataController, SnapDocumentModel model)
			: this(dataController, 0, -1, model) {
		}
		public SnapMailMergeDataService(RichEditDataControllerAdapterBase dataController, int firstRecordIndex, int lastRecordIndex, SnapDocumentModel model)
			: base(dataController, firstRecordIndex, lastRecordIndex) {
			Guard.ArgumentNotNull(model, "model");
			this.model = model;
		}
		internal SnapMailMergeDataService(RichEditDataControllerAdapterBase dataController, SnapMailMergeExportOptions options, SnapDocumentModel model)
			: this(dataController, options.FirstRecordIndex, options.LastRecordIndex, model) {
				Guard.ArgumentNotNull(options, "options");
				this.options = options;
		}
		protected override int CalcActualRecordCount() {
			if (recordCount < 0)
				recordCount = CalcActualRecordCountCore();
			return recordCount;
		}
		int CalcActualRecordCountCore() {
			IFieldDataAccessService dataAccessService = model.GetService<IFieldDataAccessService>();
			if (dataAccessService == null || this.options == null)
				return base.CalcActualRecordCount();
			IFieldContextService fieldContextService = dataAccessService.FieldContextService;
			ICalculationContext calculationContext = fieldContextService.BeginCalculation(model.DataSourceDispatcher);
			try {
				IFieldContext rootContext = new RootFieldContext(model.DataSourceDispatcher, this.options.DataSourceName);
				FieldPathDataMemberInfo path = dataAccessService.FieldPathService.FromString(this.options.DataMember ?? string.Empty).DataMemberInfo;
				if (!string.IsNullOrEmpty(this.options.FilterString))
					path.AddFilter(this.options.FilterString);
				List<GroupProperties> groups = MailMergeParamsConverter.ConvertToGroupPropertiesList(this.options.Sorting);
				for (int i = 0; i < groups.Count; i++)
					path.AddGroup(groups[i]);
				IDataEnumerator enumerator = calculationContext.GetChildDataEnumerator(rootContext, path);
				if (enumerator != null) {
					int result = 0;
					while (enumerator.MoveNext())
						result++;
					return result;
				}
			}
			finally {
				fieldContextService.EndCalculation(calculationContext);
			}
			return base.CalcActualRecordCount();
		}
		protected override MailMergeDataService CreateNew(RichEditDataControllerAdapterBase dataController, IMailMergeOptions options) {
			return new SnapMailMergeDataService(dataController, (SnapMailMergeExportOptions)options, model);
		}
	}
}
