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
using System.Drawing.Printing;
using DevExpress.Utils.Serializing;
using System.IO;
namespace DevExpress.XtraPrinting.Native {
	public class XtraPrinterSettings {
		bool collate;
		short copies;
		Duplex duplex;
		int fromPage;
		string printerName;
		PrintRange printRange;
		bool printToFile;
		int toPage;
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public bool Collate { get { return collate; } set { collate = value; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public short Copies { get { return copies; } set { copies = value; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public Duplex Duplex { get { return duplex; } set { duplex = value; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public int FromPage { get { return fromPage; } set { fromPage = value; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public string PrinterName { get { return printerName; } set { printerName = value; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public PrintRange PrintRange { get { return printRange; } set { printRange = value; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public bool PrintToFile { get { return printToFile; } set { printToFile = value; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public int ToPage { get { return toPage; } set { toPage = value; } }
		public XtraPrinterSettings() {
		}
		public void AssignFrom(PrinterSettings printerSettings) {
			collate = printerSettings.Collate;
			copies = printerSettings.Copies;
			duplex = printerSettings.Duplex;
			fromPage = printerSettings.FromPage;
			printerName = printerSettings.PrinterName;
			printRange = printerSettings.PrintRange;
			printToFile = printerSettings.PrintToFile;
			toPage = printerSettings.ToPage;
		}
		public void AssignTo (PrinterSettings printerSettings) {
			printerSettings.Collate = collate;
			printerSettings.Copies = copies;
			printerSettings.Duplex = duplex;
			printerSettings.PrinterName = PrinterName;
			printerSettings.PrintRange = printRange;
			printerSettings.PrintToFile = printToFile;
		}
		#region Save Restore
		public void SaveToXml(string xmlFile) {
			SaveCore(new XmlXtraSerializer(), xmlFile);
		}
		public void RestoreFromXml(string xmlFile) {
			RestoreCore(new XmlXtraSerializer(), xmlFile);
		}
		public void SaveToRegistry(string path) {
			SaveCore(new RegistryXtraSerializer(), path);
		}
		public void RestoreFromRegistry(string path) {
			RestoreCore(new RegistryXtraSerializer(), path);
		}
		public void SaveToStream(Stream stream) {
			long position = 0L;
			if(stream.CanSeek)
				position = stream.Position;
			SaveCore(new XmlXtraSerializer(), stream);
			if(stream.CanSeek)
				stream.Position = position;
		}
		public void RestoreFromStream(Stream stream) {
			long position = 0L;
			if(stream.CanSeek)
				position = stream.Position;
			RestoreCore(new XmlXtraSerializer(), stream);
			if(stream.CanSeek)
				stream.Position = position;
		}
		void SaveCore(XtraSerializer serializer, object path) {
			serializer.SerializeObject(this, path, "PrinterSettings");
		}
		void RestoreCore(XtraSerializer serializer, object path) {
			serializer.DeserializeObject(this, path, "PrinterSettings");
		}
		#endregion
	}
}
