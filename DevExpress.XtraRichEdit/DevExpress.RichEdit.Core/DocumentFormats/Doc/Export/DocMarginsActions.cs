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
	public class DocMarginsActions : IMarginsActions, IDisposable {
		#region Fields
		BinaryWriter writer;
		SectionMargins margins;
		#endregion
		public DocMarginsActions(MemoryStream output, SectionMargins margins) {
			this.writer = new BinaryWriter(output);
			this.margins = margins;
		}
		#region IMarginsActions Members
		public void LeftAction() {
			DocCommandLeftMargin command = new DocCommandLeftMargin();
			command.Value = margins.Left;
			command.Write(writer);
		}
		public void RightAction() {
			DocCommandRightMargin command = new DocCommandRightMargin();
			command.Value = margins.Right;
			command.Write(writer);
		}
		public void TopAction() {
			DocCommandTopMargin command = new DocCommandTopMargin();
			command.Value = margins.Top;
			command.Write(writer);
		}
		public void BottomAction() {
			DocCommandBottomMargin command = new DocCommandBottomMargin();
			command.Value = margins.Bottom;
			command.Write(writer);
		}
		public void GutterAction() {
			DocCommandGutter command = new DocCommandGutter();
			command.Value = margins.DocumentModel.UnitConverter.ModelUnitsToTwips(margins.Gutter);
			command.Write(writer);
		}
		public void GutterAlignmentAction() {
			if (margins.GutterAlignment == SectionGutterAlignment.Top || margins.GutterAlignment == SectionGutterAlignment.Bottom)
				return;
			DocCommandRTLGutter command = new DocCommandRTLGutter();
			command.GutterAlignment = margins.GutterAlignment;
			command.Write(writer);
		}
		public void HeaderOffsetAction() {
			DocCommandHeaderOffset command = new DocCommandHeaderOffset();
			command.Value = margins.HeaderOffset;
			command.Write(writer);
		}
		public void FooterOffsetAction() {
			DocCommandFooterOffset command = new DocCommandFooterOffset();
			command.Value = margins.FooterOffset;
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
