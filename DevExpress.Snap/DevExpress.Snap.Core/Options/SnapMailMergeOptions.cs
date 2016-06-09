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

using DevExpress.Office;
using DevExpress.Snap.Core.API;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Options {
	public interface IDataDispatcherOptions : IDataSourceContainerOptions {
		string DataSourceName { get; set; }
		SnapListSorting Sorting { get; }
		string FilterString { get; set; }
	}
	public interface SnapMailMergeVisualOptions : IDataDispatcherOptions, ISupportsCopyFrom<SnapMailMergeVisualOptions> {
		int CurrentRecordIndex { get; set; }
		int RecordCount { get; }
		bool IsMailMergeEnabled { get; }
		int RefreshRecordCount();
		void ResetMailMerge();
	}
	public interface SnapMailMergeExportOptions : IMailMergeOptions, IDataDispatcherOptions, ISupportsCopyFrom<SnapMailMergeExportOptions> {
		int ExportFrom { get; set; }
		int ExportRecordsCount { get; set; }
		RecordSeparator RecordSeparator { get; set; }
		SnapDocument CustomSeparator { get; }
		bool StartEachRecordFromNewParagraph { get; set; }
		bool ProgressIndicationFormVisible { get; set; }
	}
	public enum RecordSeparator {
		Custom = -1,
		None = 0,
		PageBreak = 1,
		SectionNextPage = 2,
		SectionEvenPage = 3,
		SectionOddPage = 4,
		Paragraph = 5
	}
}
