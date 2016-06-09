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
using System.Drawing;
using System.Security;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using DevExpress.Pdf.Interop;
using DevExpress.Pdf.Native;
using DevExpress.Utils;
namespace DevExpress.Pdf.Drawing {
	public class EmfMetafile : PdfDisposableObject {
		public static void ThrowIncorrectDataException() {
			throw new ArgumentException();
		}
		List<EmfRecord> records = new List<EmfRecord>();
		bool processEmf = true;
		float dpiX;
		float dpiY;
		Metafile metafile;
		bool isNotSupported;
		public float DpiX { get { return dpiX; } }
		public float DpiY { get { return dpiY; } }
		public IEnumerable<EmfRecord> Records { get { return records; } }
		public EmfMetafile(Metafile metafile) {
			byte[] data = CreateMetafileData(metafile);
			if (data != null)
				ParseMetafile(data, metafile);
			else
				SaveNotSupportedMetafile(metafile);
		}
		internal void Draw(PdfGraphicsFormContentCommandConstructor constructor) {
			if (isNotSupported) {
				constructor.DrawUnsupportedMetafile(metafile);
				return;
			}
			using (EmfMetafileGraphics context = new EmfMetafileGraphics(constructor)) {
				context.ProcessEmf = processEmf;
				foreach (EmfRecord record in records) {
					if (record is EmfPlusRecord || context.ProcessEmf || record is EmfMetafileHeaderRecord)
						record.Execute(context);
				}
			}
		}
		[SecuritySafeCritical]
		byte[] CreateMetafileData(Metafile metafile) {
			using (Metafile meta = (Metafile)metafile.Clone()) {
				IntPtr gdiHandle = meta.GetHenhmetafile();
				uint size = Gdi32Interop.GetEnhMetaFileBits(gdiHandle, 0, null);
				int error = Marshal.GetLastWin32Error();
				if (size == 0 && error != 0) {
					isNotSupported = true;
					Gdi32Interop.DeleteMetaFile(gdiHandle);
					return null;
				}
				byte[] data = new byte[size];
				Gdi32Interop.GetEnhMetaFileBits(gdiHandle, size, data);
				Gdi32Interop.DeleteEnhMetaFile(gdiHandle);
				return data;
			}
		}
		void SaveNotSupportedMetafile(Metafile metafile) {
			dpiX = metafile.HorizontalResolution;
			dpiY = metafile.VerticalResolution;
			this.metafile = (Metafile)metafile.Clone();
			isNotSupported = true;
		}
		void ParseMetafile(byte[] data, Metafile metafile) {
			using (IEnumerator<EmfRecord> recordEnumerator = (new EmfMetafileParser(data)).GetEnumerator()) {
				if (recordEnumerator.MoveNext()) {
					EmfMetafileHeaderRecord header = (EmfMetafileHeaderRecord)recordEnumerator.Current;
					if (recordEnumerator.MoveNext()) {
						if (recordEnumerator.Current is EmfPlusHeaderRecord) {
							EmfPlusHeaderRecord emfPlusHeader = (EmfPlusHeaderRecord)recordEnumerator.Current;
							dpiX = emfPlusHeader.LogicalDpiX;
							dpiY = emfPlusHeader.LogicalDpiY;
							processEmf = false;
						}
						else {
							SaveNotSupportedMetafile(metafile);
							return;
						}
						records = new List<EmfRecord>(header.RecordsCount);
						records.Add(header);
						records.Add(recordEnumerator.Current);
						while (recordEnumerator.MoveNext())
							if (recordEnumerator.Current != null)
								records.Add(recordEnumerator.Current);
					}
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				foreach (IDisposable record in records) {
					if (record != null)
						record.Dispose();
				}
				if (metafile != null)
					metafile.Dispose();
			}
		}
	}
}
