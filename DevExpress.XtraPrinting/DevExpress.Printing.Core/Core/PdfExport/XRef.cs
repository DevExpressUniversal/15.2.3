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
using System.IO;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfXRef {
		ArrayList entiers = new ArrayList();
		long byteOffset = -1;
		public int Count { get { return entiers.Count; } }
		public PdfXRefEntryBase this[int index] { get { return (PdfXRefEntryBase)entiers[index]; } }
		public long ByteOffset { get { return byteOffset; } }
		public PdfXRef() {
			this.entiers.Add(new PdfXRefEntryFree());
		}
		void CalcByteOffset(StreamWriter writer) {
			writer.Flush();
			byteOffset = writer.BaseStream.Position;
		}
		int AddEntry(PdfXRefEntry entry) {
			return this.entiers.Add(entry);
		}
		void RegisterIndirectReference(PdfIndirectReference indirectReference) {
			if(indirectReference == null)
				throw new PdfException("Can't register direct object");
			if(indirectReference.Number == 0) {
				indirectReference.Number = this.entiers.Add(new PdfXRefEntry(indirectReference));
			}
		}
		public void RegisterObject(PdfObject obj) {
			if(obj == null)
				throw new ArgumentException();
			RegisterIndirectReference(obj.IndirectReference);
		}
		public void Clear() {
			entiers.Clear();
		}
		public void Write(StreamWriter writer) {
			CalcByteOffset(writer);
			writer.WriteLine("xref");
			writer.Write("0 " + Convert.ToString(Count));
			for(int i = 0; i < Count; i++) {
				writer.WriteLine();
				this[i].Write(writer);				
			}
			writer.WriteLine();
		}
	}
	public abstract class PdfXRefEntryBase {
		protected abstract string TypeString { get; }
		protected abstract long ByteOffset { get; }
		protected abstract int Generation { get; }
		public void Write(StreamWriter writer) {
			writer.Write(ByteOffset.ToString("D10") + " " + Generation.ToString("D5") + " " + TypeString);
		}
	}
	public class PdfXRefEntryFree : PdfXRefEntryBase {
		protected override string TypeString { get { return "f"; } }
		protected override long ByteOffset { get { return 0; } }
		protected override int Generation { get { return 65535; } }
	}
	public class PdfXRefEntry : PdfXRefEntryBase {
		PdfIndirectReference indirectReference;
		protected override string TypeString { get { return "n"; } }
		protected override long ByteOffset { get { return indirectReference.ByteOffset; } }
		protected override int Generation { get { return PdfIndirectReference.Generation; } }
		public PdfXRefEntry(PdfIndirectReference indirectReference) : base() {
			this.indirectReference = indirectReference;
		}
	}	
}
