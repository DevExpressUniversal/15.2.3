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

using System.IO;
using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraPrinting.Export.Rtf {
	public abstract class RtfExportProviderBase {
		Stream stream;
		protected StreamWriter writer;
		protected RtfExportHelper rtfExportHelper;
		protected RtfExportProviderBase(Stream stream, RtfExportHelper rtfExportHelper) {
			this.stream = stream;
			this.rtfExportHelper = rtfExportHelper;
		}
		public void Commit() {
			writer = new StreamWriter(stream, DXEncoding.Default);
			writer.WriteLine(String.Format(RtfTags.StartWithCodePage, DXEncoding.Default.CodePage));
			WriteHeader();
			WriteContent();
			writer.WriteLine(@"}");
			writer.Flush();
		}
		protected virtual void WriteDocumentHeaderFooter() {
		}
		protected virtual void WritePageNumberingInfo() { 
		}
		protected virtual void WriteHeader() {
			WriteFontTable();
			WriteColorTable();
			WriteListTable();
			WriteListOverrideTable();
		}
		protected abstract void WriteContent();
		protected void WriteFontTable() {
			writer.WriteLine("{");
			writer.WriteLine(RtfTags.FontTable);
			StringCollection fontNames = rtfExportHelper.FontNamesCollection;
			int count = fontNames.Count;
			for(int i = 0; i < count; i++)
				writer.WriteLine(String.Format(RtfTags.DefineFont, i, fontNames[i]));
			writer.WriteLine("}");
		}
		protected void WriteColorTable() {
			writer.WriteLine("{");
			writer.WriteLine(RtfTags.ColorTable);
			ColorCollection colors = rtfExportHelper.ColorCollection;
			int count = colors.Count;
			for(int i = 0; i < count; i++) {
				if(colors[i] != Color.Empty)
					writer.WriteLine(String.Format(RtfTags.RGB, colors[i].R, colors[i].G, colors[i].B));
				else
					writer.WriteLine(";");
			}
			writer.WriteLine("}");
		}
		protected internal virtual void WriteListTable() {
			Dictionary<int, string> listCollection = rtfExportHelper.ListCollection;
			if (listCollection.Count <= 0)
				return;
			writer.WriteLine("{");
			writer.WriteLine(RtfTags.NumberingListTable);
			foreach (string value in listCollection.Values)
				writer.WriteLine(value);
			writer.WriteLine("}");
		}
		protected internal virtual void WriteListOverrideTable() {
			List<string> overrides = rtfExportHelper.ListOverrideCollection;
			if (overrides.Count == 0)
				return;
			writer.WriteLine("{");
			writer.WriteLine(RtfTags.ListOverrideTable);
			int count = overrides.Count;
			for (int i = 0; i < count; i++)
				writer.WriteLine(overrides[i]);
			writer.WriteLine("}");
		}
	}
}
