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
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region DocumentVariableDestination
	public class DocumentVariableDestination : StringValueDestination {
		#region Fields
		string name;
		string value;
		bool isNameRead;
		#endregion
		public DocumentVariableDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
			this.isNameRead = true;
		}
		protected internal override StringValueDestination CreateEmptyClone() {
			return new DocumentVariableDestination(Importer);
		}
		public override void AfterPopRtfState() {
			base.AfterPopRtfState();
			if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(value)) {
				if (name != DevExpress.XtraRichEdit.Model.DocumentProperties.UpdateDocVariableFieldsBeforePrintDocVarName)
					Importer.DocumentModel.Variables.Add(name, value);
				else
					Importer.DocumentModel.DocumentProperties.SetUpdateFieldsBeforePrintFromDocVar(value);
			}
		}
		public override void NestedGroupFinished(DestinationBase nestedDestination) {
			base.NestedGroupFinished(nestedDestination);
			DocumentVariableDestination nested = nestedDestination as DocumentVariableDestination;
			if (nested == null)
				return;
			string nestedValue = nested.Value.Trim();
			if (isNameRead) {
				name = nestedValue;
				isNameRead = false;
			}
			else
				value = nestedValue;
		}
	}
	#endregion
}
