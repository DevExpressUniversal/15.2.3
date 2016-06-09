#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.IO;
namespace DevExpress.Pdf.Native {
	internal class PdfObjectWriter {
		public const string EndOfLine = "\r\n";
		readonly PdfDocumentStream stream;
		public PdfDocumentStream Stream { get { return stream; } }
		public PdfObjectWriter(PdfDocumentStream stream) {
			this.stream = stream;
		}
		public PdfObjectWriter(Stream stream) : this(new PdfDocumentStream(stream)) {
		}
		public virtual PdfObjectSlot WriteIndirectObject(PdfObjectContainer container) {
			PdfObjectSlot result = null;
			if (container != null) {
				stream.SetPositionFromEnd(0);
				int number = container.ObjectNumber;
				int generation = container.ObjectGeneration;
				result = new PdfObjectSlot(number, 0, stream.Position);
				stream.WriteStringFormat("{0} 0 obj\r\n", number);
				stream.WriteObject(container.Value, number);
				stream.WriteString("\r\nendobj\r\n");
			}
			return result;
		}
	}
}
