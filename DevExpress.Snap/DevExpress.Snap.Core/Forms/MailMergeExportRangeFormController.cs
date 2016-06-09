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
using DevExpress.Snap.Core.Options;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Forms;
namespace DevExpress.Snap.Core.Forms {
	public class MailMergeExportFormController : FormController {
		internal enum ModeType {
			AllRecords = 0,
			CurrentRecord = 1,
			Range = 2
		}
		readonly ISnapControl control;
		int from;
		int count;
		SnapMailMergeExportOptions properties;
		MailMergeExportFormController.ModeType mode;
		RecordSeparator separator;
		internal MailMergeExportFormController(MailMergeExportFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = (ISnapControl)controllerParameters.Control;
			this.properties = controllerParameters.Properties;
			this.separator = this.properties.RecordSeparator;
			if (this.properties.ExportRecordsCount <= 0) {
				this.mode = ModeType.AllRecords;
				this.from = this.properties.ExportFrom + 1;
				this.count = 5;
			}
			else {
				this.mode = ModeType.Range;
				this.from = this.properties.ExportFrom + 1;
				this.count = this.properties.ExportRecordsCount;
			}
		}
		internal int From { get { return from; } set { from = value; } }
		internal int Count { get { return count; } set { count = value; } }
		internal RecordSeparator Separator { get { return separator; } set { separator = value; } }
		internal MailMergeExportFormController.ModeType Mode { get { return mode; } set { mode = value; } }
		internal SnapMailMergeExportOptions Properties { get { return properties; } }
		public override void ApplyChanges() {
			switch (mode) {
				case MailMergeExportFormController.ModeType.AllRecords:
					this.properties.ExportFrom = 0;
					this.properties.ExportRecordsCount = -1;
					break;
				case MailMergeExportFormController.ModeType.CurrentRecord:
					this.properties.ExportFrom = ((ISnapControlOptions)this.control.InnerControl.Options).SnapMailMergeVisualOptions.CurrentRecordIndex;
					this.properties.ExportRecordsCount = 1;
					break;
				case MailMergeExportFormController.ModeType.Range:
					this.properties.ExportFrom = this.from - 1;
					this.properties.ExportRecordsCount = this.count;
					break;
			}
			this.properties.RecordSeparator = this.separator;
		}		
	}
	public class MailMergeExportFormControllerParameters : FormControllerParameters {
		readonly SnapMailMergeExportOptions properties;
		public MailMergeExportFormControllerParameters(ISnapControl control, SnapMailMergeExportOptions properties)
			: base(control) {
				this.properties = properties;
		}
		internal SnapMailMergeExportOptions Properties { get { return properties; } }
	}
}
