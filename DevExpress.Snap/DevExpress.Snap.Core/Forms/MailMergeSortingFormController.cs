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

using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Forms {
	public class MailMergeSortingFormController : SortingFormControllerBase {
		public MailMergeSortingFormController(SortingFormControllerBaseParameters controllerParameters)
			: base(controllerParameters) {
		}
		protected override bool AllowEmptySortList { get { return true; } }		
		protected override Native.SnapListFieldInfo GetListFieldInfo() {
			return new Native.SnapListFieldInfo(((DevExpress.Snap.API.Native.SnapNativeDocument)Control.Document).PieceTable, null);
		}
		protected override FieldPathInfo GetFieldPathInfo() {
			SnapMailMergeVisualOptions mailMergeOptions = ((ISnapControlOptions)Control.InnerControl.Options).SnapMailMergeVisualOptions;
			RootFieldDataSourceInfo dataSourceInfo = new RootFieldDataSourceInfo(mailMergeOptions.DataSourceName);
			FieldPathDataMemberInfo dataMemberInfo = new FieldPathDataMemberInfo();
			if(!string.IsNullOrEmpty(mailMergeOptions.DataMember))
				dataMemberInfo.AddFieldName(mailMergeOptions.DataMember);
			if (!string.IsNullOrEmpty(mailMergeOptions.FilterString))
				dataMemberInfo.AddFilter(mailMergeOptions.FilterString);
			foreach(SnapListGroupParam param in mailMergeOptions.Sorting) {
				GroupProperties groupProperties = new GroupProperties();
				GroupFieldInfo groupFieldInfo = new GroupFieldInfo(param.FieldName) { SortOrder = param.SortOrder, GroupInterval = param.Interval };
				groupProperties.GroupFieldInfos.Add(groupFieldInfo);
				dataMemberInfo.AddGroup(groupProperties);
			}				
			return new FieldPathInfo() { DataSourceInfo = dataSourceInfo, DataMemberInfo = dataMemberInfo };
		}
		SnapMailMergeVisualOptions GetMailMergeOptions() { return ((ISnapControlOptions)Control.InnerControl.Options).SnapMailMergeVisualOptions; }
		private string GetListDataMember() {
			SnapMailMergeVisualOptions mailMergeOptions = GetMailMergeOptions();
			return mailMergeOptions.DataMember;
		}
		public override void ApplyChanges() {
			SnapListSorting sorting = GetMailMergeOptions().Sorting;
			sorting.Clear();
			foreach(SortField level in SortList) { 
				sorting.Add(new SnapListGroupParam(level.FieldName, level.SortOrder));
			}
		}
	}
}
