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
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IMailMergeParametersGrid {
		void RefreshGrid();
		ISpreadsheetControl SpreadsheetControl { get; set; }
	}
	public class MailMergeParametersGridController {
		#region Fields
		readonly IMailMergeParametersGrid mailMergeParametersGrid;
		ISpreadsheetControl spreadsheetControl;
		#endregion
		#region Properties
		public ISpreadsheetControl SpreadsheetControl {
			get { return spreadsheetControl; }
			set {
				if(SpreadsheetControl != null)
					UnsubscribeDocumentModelEvents();
				spreadsheetControl = value;
				if(SpreadsheetControl != null)
					SubscribeDocumentModelEvents();
			}
		}
		IMailMergeParametersGrid MailMergeParametersGrid { get { return mailMergeParametersGrid; } }
		#endregion
		public MailMergeParametersGridController(IMailMergeParametersGrid mailMergeParametersGrid) {
			this.mailMergeParametersGrid = mailMergeParametersGrid;
			SpreadsheetControl = mailMergeParametersGrid.SpreadsheetControl;
		}
		public object EditSpreadsheetValue(int index, object newValue) {
			MailMergeParametersCollection mailMergeParameters = spreadsheetControl.InnerControl.DocumentModel.MailMergeParameters;
			SpreadsheetParameter parameter = mailMergeParameters[index];
			parameter.Value = newValue;
			return parameter.Value;
		}
		void SubscribeDocumentModelEvents() {
			MailMergeParametersCollection mailMergeParameters = spreadsheetControl.InnerControl.DocumentModel.MailMergeParameters;
			mailMergeParameters.CollectionCleared += OnMailMergeParametersCollectionChanged;
			mailMergeParameters.ParameterAdded += OnMailMergeParametersCollectionChanged;
			mailMergeParameters.ParameterInserted += OnMailMergeParametersCollectionChanged;
			mailMergeParameters.ParameterRemoved += OnMailMergeParametersCollectionChanged;
		}
		void UnsubscribeDocumentModelEvents() {
			MailMergeParametersCollection mailMergeParameters = spreadsheetControl.InnerControl.DocumentModel.MailMergeParameters;
			mailMergeParameters.CollectionCleared -= OnMailMergeParametersCollectionChanged;
			mailMergeParameters.ParameterAdded -= OnMailMergeParametersCollectionChanged;
			mailMergeParameters.ParameterInserted -= OnMailMergeParametersCollectionChanged;
			mailMergeParameters.ParameterRemoved -= OnMailMergeParametersCollectionChanged;
		}
		void OnMailMergeParametersCollectionChanged(object sender, EventArgs e) {
			RefreshMailMergeParametersGrid();
		}
		void RefreshMailMergeParametersGrid() {
			MailMergeParametersGrid.RefreshGrid();
		}
	}
}
