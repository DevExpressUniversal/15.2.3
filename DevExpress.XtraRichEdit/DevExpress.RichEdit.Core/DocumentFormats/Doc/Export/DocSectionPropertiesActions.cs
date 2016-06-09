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
using System.Text;
using System.IO;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
namespace DevExpress.XtraRichEdit.Export.Doc {
	public class DocSectionPropertiesActions : IDisposable {
		#region Fields
		BinaryWriter writer;
		Section section;
		#endregion
		public DocSectionPropertiesActions(MemoryStream output, Section section) {
			this.writer = new BinaryWriter(output);
			this.section = section;
		}
		public void CreateSectionPropertiesModifiers() {
			writer.Write(GetColumnsProperties());
			writer.Write(GetMarginsProperties());
			writer.Write(GetPageProperties());
			writer.Write(GetGeneralSettings());
			writer.Write(GetPageNumbering());
			writer.Write(GetLineNumbering());
			writer.Write(GetFootNote());
			writer.Write(GetEndNote());
		}
		byte[] GetColumnsProperties() {
			using (MemoryStream columnsStream = new MemoryStream()){
				DocColumnsActions actions = new DocColumnsActions(columnsStream, section.Columns);
				ColumnsHelper.ForEach(actions);
				return columnsStream.ToArray();
			}
		}
		byte[] GetMarginsProperties() {
			using (MemoryStream marginsStream = new MemoryStream()) {
				DocMarginsActions actions = new DocMarginsActions(marginsStream, section.Margins);
				MarginsHelper.ForEach(actions);
				return marginsStream.ToArray();
			}
		}
		byte[] GetPageProperties() {
			using (MemoryStream pageStream = new MemoryStream()) {
				DocPageActions actions = new DocPageActions(pageStream, section.Page);
				PageHelper.ForEach(actions);
				return pageStream.ToArray();
			}
		}
		byte[] GetGeneralSettings() {
			using (MemoryStream generalSettingsStream = new MemoryStream()) {
				DocGeneralSettingsActions actions = new DocGeneralSettingsActions(generalSettingsStream, section.GeneralSettings);
				GeneralSettingsHelper.ForEach(actions);
				return generalSettingsStream.ToArray();
			}
		}
		byte[] GetPageNumbering() {
			using (MemoryStream pageNumberingStream = new MemoryStream()) {
				DocPageNumberingActions actions = new DocPageNumberingActions(pageNumberingStream, section.PageNumbering);
				PageNumberingHelper.ForEach(actions);
				return pageNumberingStream.ToArray();
			}
		}
		byte[] GetLineNumbering() {
			using (MemoryStream lineNumberingStream = new MemoryStream()) {
				DocLineNumberingActions actions = new DocLineNumberingActions(lineNumberingStream, section.LineNumbering);
				LineNumberingHelper.ForEach(actions);
				return lineNumberingStream.ToArray();
			}
		}
		byte[] GetFootNote() {
			using (MemoryStream footNoteStream = new MemoryStream()) {
				DocFootNoteActions actions = new DocFootNoteActions(footNoteStream, section.FootNote, true);
				FootNoteHelper.ForEach(actions);
				return footNoteStream.ToArray();
			}
		}
		byte[] GetEndNote() {
			using (MemoryStream endNoteStream = new MemoryStream()) {
				DocFootNoteActions actions = new DocFootNoteActions(endNoteStream, section.EndNote, false);
				FootNoteHelper.ForEach(actions);
				return endNoteStream.ToArray();
			}
		}
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
}
