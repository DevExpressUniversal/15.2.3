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
namespace DevExpress.Pdf.Common {
	class TTFInitializeParam {
		public PdfCharCache Chars;
		public string NewFontName;
	}
	abstract class TTFTable {
		protected const string tableMissingError = "The required table doesn't exist in the font file";
		TTFFile owner;
		TTFTableDirectoryEntry entry;
		protected internal TTFTableDirectoryEntry Entry {
			get {
				if(entry == null)
					entry = Owner.TableDirectory[Tag];
				return entry;
			}
		}
		protected internal abstract string Tag { get; }
		public TTFFile Owner { get { return owner; } }
		public virtual int Length { get { return 0; } }
		public TTFTable(TTFFile owner) {
			this.owner = owner;
		}
		protected virtual void ReadTable(TTFStream ttfStream) {
		}
		protected virtual void WriteTable(TTFStream ttfStream) {
		}
		protected virtual void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
		}
		public void Read(TTFStream ttfStream) {
			if(Entry == null)
				throw new TTFFileException(tableMissingError);
			ttfStream.Seek(Entry.Offset);
			ReadTable(ttfStream);
		}
		public void Write(TTFStream ttfStream) {
			ttfStream.Pad4();
			if(Entry != null)
				Entry.InitializeOffset(ttfStream.Position);
			WriteTable(ttfStream);
		}
		public void Initialize(TTFTable pattern) {
			Initialize(pattern, null);
		}
		public void Initialize(TTFTable pattern, TTFInitializeParam param) {
			if(pattern == null) return;
			if(!this.GetType().Equals(pattern.GetType())) return;
			InitializeTable(pattern, param);
			if(Entry == null)
				Owner.TableDirectory.Register(this);
		}
	}
	class TTFBinaryTable : TTFTable {
		string name;
		byte[] data;
		protected internal override string Tag { get { return name; } }
		public string Name { get { return name; } }
		public byte[] Data { get { return data; } }
		public override int Length { get { return data.Length; } }
		public TTFBinaryTable(TTFFile ttfFile, string name)
			: base(ttfFile) {
			this.name = name;
		}
		protected override void ReadTable(TTFStream ttfStream) {
			data = ttfStream.ReadBytes(Entry.Length);
		}
		protected override void WriteTable(TTFStream ttfStream) {
			ttfStream.WriteBytes(data);
		}
		protected override void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
			TTFBinaryTable p = pattern as TTFBinaryTable;
			name = p.Name;
			data = new byte[p.Data.Length];
			p.Data.CopyTo(data, 0);
		}
	}
}
