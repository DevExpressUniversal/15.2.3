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
using DevExpress.XtraRichEdit.Native;
using System.IO;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.Doc;
namespace DevExpress.XtraRichEdit.Export.Doc {
	public class DocLineNumberingActions : ILineNumberingActions, IDisposable {
		#region Fields
		BinaryWriter writer;
		SectionLineNumbering lineNumbering;
		#endregion
		public DocLineNumberingActions(MemoryStream output, SectionLineNumbering lineNumbering) {
			this.writer = new BinaryWriter(output);
			this.lineNumbering = lineNumbering;
		}
		#region ILineNumberingActions Members
		public void DistanceAction() {
			DocCommandLineNumberingDistance command = new DocCommandLineNumberingDistance();
			command.Value = lineNumbering.Distance;
			command.Write(writer);
		}
		public void NumberingRestartTypeAction() {
			DocCommandNumberingRestartType command = new DocCommandNumberingRestartType();
			command.NumberingRestartType = lineNumbering.NumberingRestartType;
			command.Write(writer);
		}
		public void StartingLineNumberAction() {
			DocCommandStartLineNumber command = new DocCommandStartLineNumber();
			command.Value = lineNumbering.StartingLineNumber;
			command.Write(writer);
		}
		public void StepAction() {
			DocCommandStep command = new DocCommandStep();
			command.Value = lineNumbering.Step;
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
