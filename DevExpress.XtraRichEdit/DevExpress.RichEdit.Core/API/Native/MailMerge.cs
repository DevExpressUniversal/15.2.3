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

using System.ComponentModel;
using System.Runtime.InteropServices;
namespace DevExpress.XtraRichEdit.API.Native {
	[ComVisible(true)]
	public enum MergeMode {
		NewParagraph = DevExpress.XtraRichEdit.Model.MergeMode.NewParagraph,
		NewSection = DevExpress.XtraRichEdit.Model.MergeMode.NewSection,
		JoinTables = DevExpress.XtraRichEdit.Model.MergeMode.JoinTables
	}
	[ComVisible(true)]
	public interface MailMergeOptions {
		object DataSource {get; set;}
		string DataMember { get; set;}
		MergeMode MergeMode { get; set; }
		int FirstRecordIndex { get; set; }
		int LastRecordIndex { get; set; }
		bool CopyTemplateStyles { get; set; }
		bool HeaderFooterLinkToPrevious { get; set; }
	}	 
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	internal class NativeMailMergeOptions : MailMergeOptions {
		object dataSource;
		string dataMember;
		MergeMode mergeMode;
		int firstRecordIndex = 0;
		int lastRecordIndex = -1;
		bool copyTemplateStyles;
		bool headerFooterLinkToPrevious = true;
		#region MailMergeOptions Members
		public object DataSource {
			get { return dataSource; }
			set { dataSource = value; }
		}
		public string DataMember {
			get { return dataMember; }
			set { dataMember = value; }
		}
		public MergeMode MergeMode {
			get { return mergeMode; }
			set { mergeMode = value; }
		}
		public int FirstRecordIndex {
			get { return firstRecordIndex; }
			set { firstRecordIndex = value; }
		}
		public int LastRecordIndex {
			get { return lastRecordIndex; }
			set { lastRecordIndex = value; }
		}
		public bool CopyTemplateStyles {
			get { return copyTemplateStyles; }
			set { copyTemplateStyles = value; }
		}
		[DefaultValue(true)]
		public bool HeaderFooterLinkToPrevious {
			get { return headerFooterLinkToPrevious; }
			set { headerFooterLinkToPrevious = value; }
		}
		#endregion
		internal DevExpress.XtraRichEdit.Model.MailMergeOptions GetInternalMailMergeOptions() {
			DevExpress.XtraRichEdit.Model.MailMergeOptions result = new DevExpress.XtraRichEdit.Model.MailMergeOptions();
			result.MergeMode = (DevExpress.XtraRichEdit.Model.MergeMode)MergeMode;
			result.FirstRecordIndex = FirstRecordIndex;
			result.LastRecordIndex = LastRecordIndex;
			result.DataSource = DataSource;
			result.DataMember = DataMember;
			result.CopyTemplateStyles = CopyTemplateStyles;
			result.HeaderFooterLinkToPrevious = HeaderFooterLinkToPrevious;
			return result;
		}
	}
}
