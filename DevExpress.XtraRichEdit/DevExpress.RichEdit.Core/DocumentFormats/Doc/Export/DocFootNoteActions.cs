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
using System.Collections.Generic;
using System.IO;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.Doc;
namespace DevExpress.XtraRichEdit.Export.Doc {
	#region DocFootNoteActions
	public class DocFootNoteActions : IFootNoteActions, IDisposable {
		#region Fields
		BinaryWriter writer;
		SectionFootNote footNote;
		bool isFootNote;
		#endregion
		public DocFootNoteActions(MemoryStream output, SectionFootNote footNote, bool isFootNote) {
			this.writer = new BinaryWriter(output);
			this.footNote = footNote;
			this.isFootNote = isFootNote;
		}
		#region Properties
		protected internal bool IsFootNote {
			get { return isFootNote; }
			set { isFootNote = value; }
		}
		protected internal SectionFootNote FootNote {
			get { return footNote; }
			set { footNote = value; }
		}
		#endregion
		#region IFootNoteActions Members
		public void NumberingFormatAction() {
			DocCommandFootNoteNumberingFormatBase command;
			if (isFootNote)
				command = new DocCommandFootNoteNumberingFormat();
			else
				command = new DocCommandEndNoteNumberingFormat();
			command.NumberingFormat = footNote.NumberingFormat;
			command.Write(writer);
		}
		public void NumberingRestartTypeAction() {
			DocCommandFootNoteNumberingRestartTypeBase command;
			if (isFootNote)
				command = new DocCommandFootNoteNumberingRestartType();
			else
				command = new DocCommandEndNoteNumberingRestartType();
			command.NumberingRestartType = footNote.NumberingRestartType;
			command.Write(writer);
		}
		public void PositionAction() {
		}
		public void StartingNumberAction() {
			DocCommandShortPropertyValueBase command;
			if (isFootNote)
				command = new DocCommandFootNoteStartingNumber();
			else
				command = new DocCommandEndNoteStartingNumber();
			command.Value = footNote.StartingNumber;
			command.Write(writer);
		}
		#endregion
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				IDisposable resource = this.writer as IDisposable;
				if (resource != null) {
					resource.Dispose();
					resource = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region FootNoteHelper
	public static class FootNoteHelper {
		public static void ForEach(IFootNoteActions actions) {
			actions.NumberingFormatAction();
			actions.NumberingRestartTypeAction();
			actions.PositionAction();
			actions.StartingNumberAction();
		}
	}
	#endregion
	#region IFootNoteActions
	public interface IFootNoteActions {
		void NumberingFormatAction();
		void NumberingRestartTypeAction();
		void PositionAction();
		void StartingNumberAction();
	}
	#endregion
}
