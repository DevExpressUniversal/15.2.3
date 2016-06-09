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

using System.Collections;
using DevExpress.XtraRichEdit;
using DevExpress.Web.Office;
using DevExpress.Web.Office.Internal;
using System;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class SaveMergedDocumentCommand : SaveAsDocumentCommand {
		public SaveMergedDocumentCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.SaveMergedDocument; } }
		public override void SwitchSession() {
			string workDirectory = GetWorkDirectory();
			string fileName = Parameters["fileName"].ToString();
			string folderPath = Parameters["folderPath"].ToString();
			string fileExtension = Parameters["fileExtension"].ToString();
			if (IsNotAllowedFileExtension(fileExtension))
				throw new DocumentCannotBeSavedException();
			string filePath = UrlUtils.ResolvePhysicalPath(workDirectory + "\\" + folderPath + "\\" + fileName + fileExtension);
			DocumentFormat documentFormat = Manager.WorkSession.RichEdit.DocumentModel.AutodetectDocumentFormat(fileName + fileExtension, false);
			XtraRichEdit.API.Native.MailMergeOptions options = Manager.WorkSession.RichEdit.CreateMailMergeOptions();
			options.FirstRecordIndex = (int)Parameters["firstRecordIndex"];
			options.LastRecordIndex = (int)Parameters["lastRecordIndex"];
			options.MergeMode = (XtraRichEdit.API.Native.MergeMode)Parameters["mergeMode"];
			Manager.WorkSession.RichEdit.MailMerge(options, filePath, documentFormat);
		}
	}
}
