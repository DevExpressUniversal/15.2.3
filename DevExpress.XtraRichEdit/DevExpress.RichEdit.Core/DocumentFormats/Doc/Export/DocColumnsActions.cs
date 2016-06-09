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
using DevExpress.XtraRichEdit.Import.Doc;
namespace DevExpress.XtraRichEdit.Export.Doc {
	public class DocColumnsActions : IColumnsActions, IDisposable {
		#region Fields
		BinaryWriter writer;
		SectionColumns columns;
		#endregion
		public DocColumnsActions(MemoryStream output, SectionColumns columns) {
			this.writer = new BinaryWriter(output);
			this.columns = columns;
		}
		#region IColumnsActions Members
		public void ColumnCountAction() {
			DocCommandColumnCount command = new DocCommandColumnCount();
			command.Value = columns.ColumnCount;
			command.Write(writer);
		}
		public void ColumnsAction() {
			if (columns.EqualWidthColumns)
				return;
			int count = columns.Info.Columns.Count;
			for (int i = 0; i < count; i++) {
				ColumnInfo info = columns.Info.Columns[i];
				DocCommandColumnWidth commandWidth = new DocCommandColumnWidth();
				commandWidth.ColumnIndex = (byte)i;
				commandWidth.ColumnWidth = (short)info.Width;
				commandWidth.Write(writer);
				DocCommandNotEvenlyColumnsSpace commandSpace = new DocCommandNotEvenlyColumnsSpace();
				commandSpace.ColumnIndex = (byte)i;
				commandSpace.ColumnSpace = (short)info.Space;
				commandSpace.Write(writer);
			}
		}
		public void DrawVerticalSeparatorAction() {
			DocCommandDrawVerticalSeparator command = new DocCommandDrawVerticalSeparator();
			command.Value = columns.DrawVerticalSeparator;
			command.Write(writer);
		}
		public void EqualWidthColumnsAction() {
			DocCommandEqualWidthColumns command = new DocCommandEqualWidthColumns();
			command.Value = columns.EqualWidthColumns;
			command.Write(writer);
		}
		public void SpaceAction() {
			DocCommandColumnSpace command = new DocCommandColumnSpace();
			command.Value = columns.Space;
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
}
