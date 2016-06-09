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
using System.Collections;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Service {
	public class DashboardPaneContent {
		public string Name { get; set; }
		public string Type { get; set; }
		public string Group { get; set; }
		public ContentType ContentType { get; set; }
		public IEnumerable DataSource { get { return null; } set { } }
		public HierarchicalItemData ItemData { get; set; }
		public IEnumerable DataSourceMembers { get { return null; } set { } }
		public IList<IList> SelectedValues { get; set; }
		public string[] AxisNames { get; set; }
		public string[] DimensionIds { get; set; }
		public DashboardItemViewModel ViewModel { get; set; }
		public ConditionalFormattingModel ConditionalFormattingModel { get; set; }
		public DashboardItemCaptionViewModel CaptionViewModel { get; set; }
		public ActionModel ActionModel { get; set; }
		public object[] Parameters { get; set; }
		public IList<FormattableValue> DrillDownValues { get; set; }
		public IList<object> DrillDownUniqueValues { get; set; }
	}
	[Flags]
	public enum ContentType {
		Empty = 0,
		ViewModel = 1,
		ActionModel = 2,
		CompleteDataSource = 4,
		PartialDataSource = 8,
		CaptionViewModel = 16,
		ConditionalFormattingModel = 32,
		FullContentNoActionModel = ViewModel | CaptionViewModel | CompleteDataSource | ConditionalFormattingModel,
		FullContent = FullContentNoActionModel | ActionModel,		
	}
}
