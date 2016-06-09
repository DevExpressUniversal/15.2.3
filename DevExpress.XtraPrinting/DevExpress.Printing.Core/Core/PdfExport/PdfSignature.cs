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
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Text;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfSignature : PdfDocumentDictionaryObject {
		PdfSignatureContentString content;
		PdfSignatureByteRange byteRange;
		public string Reason { get; set; }
		public string Location { get; set; }
		public string ContactInfo { get; set; }
		public X509Certificate2 Certificate { 
			set {
				if(value != null) {
					this.content = new PdfSignatureContentString(value);
					this.byteRange = new PdfSignatureByteRange();
				}
			} 
		}
		public bool Active { get { return content != null; } }
		public PdfSignature()
			: base(false) {
		}
		public override void FillUp() {
			if(content == null || byteRange == null)
				return;
			Dictionary.Add("Type", "Sig");
			Dictionary.Add("Filter", "Adobe.PPKMS");
			Dictionary.Add("SubFilter", "adbe.pkcs7.sha1");
			if(!string.IsNullOrEmpty(Reason))
				Dictionary.Add("Reason", CreatePdfString(Reason));
			if(!string.IsNullOrEmpty(Location))
				Dictionary.Add("Location", CreatePdfString(Location));
			if(!string.IsNullOrEmpty(ContactInfo))
				Dictionary.Add("ContactInfo", CreatePdfString(ContactInfo));
			Dictionary.Add("M", new PdfDate(DateTime.Now));
			Dictionary.Add("Contents", content);
			Dictionary.Add("ByteRange", byteRange);
		}
		PdfObject CreatePdfString(string s) {
			return Encoding.UTF8.GetByteCount(s) == s.Length ?
				(PdfObject) new PdfLiteralString(s) :
				new PdfTextUnicode(s);
		}
		public void Finish(StreamWriter writer) {
			if(content == null || byteRange == null)
				return;
			byteRange.Patch(writer, content.Offset, content.Length);
			content.Patch(writer);
		}
	}
	public class PdfAcroForm : PdfDocumentDictionaryObject {
		PdfArray fields = new PdfArray();
		public PdfArray Fields { get { return fields; } }
		public bool Active { get { return Fields.Count > 0; } }
		public PdfAcroForm(bool compressed)
			: base(compressed) {
		}
		public override void FillUp() {
			Dictionary.Add("Fields", fields);
			Dictionary.Add("SigFlags", 3);
		}
	}
	public class PdfSignatureContentString : PdfHexadecimalString {
		long offset;
		long length;
		int placeholderLength;
		X509Certificate2 certificate;
#if !DEBUGTEST
		const int bufferLength = 32768;
#else
		int bufferLength = 32768;
		public int Test_BufferLength { get { return bufferLength; } set { bufferLength = value; } }
#endif
		public long Offset { get { return offset; } }
		public long Length { get { return length; } }
		public PdfSignatureContentString(X509Certificate2 certificate)
			: base(null) {
			this.certificate = certificate;
			if(certificate == null) return;
			using(HashAlgorithm sha1 = SHA1Managed.Create()) {
				byte[] placeholder = SignData(sha1.ComputeHash(new byte[] { 1 }));
				placeholderLength = placeholder.Length;
				SetValue(placeholder);
			}
		}
		protected override void WriteContent(StreamWriter writer) {
			writer.Flush();
			offset = writer.BaseStream.Position;
			base.WriteContent(writer);
			writer.Flush();
			length = writer.BaseStream.Position - offset;
		}
		public void Patch(StreamWriter writer) {
			writer.Flush();
			writer.BaseStream.Position = 0;
			using(HashAlgorithm sha1 = SHA1Managed.Create()) {
				byte[] data = SignData(CalculateHash(writer.BaseStream, sha1, offset, length));
				if(data.Length != placeholderLength)
					throw new Exception("PDF signature length");
				SetValue(data);
			}
			writer.BaseStream.Position = offset;
			base.WriteContent(writer);
			writer.Flush();
		}
#if DEBUGTEST
		public byte[] Test_CalculateHash(Stream stream, System.Security.Cryptography.HashAlgorithm hashAlgorithm, long offset, long length) {
			return CalculateHash(stream, hashAlgorithm, offset, length);
		}
#endif
		byte[] CalculateHash(Stream stream, System.Security.Cryptography.HashAlgorithm hashAlgorithm, long offset, long length) {
			byte[] buffer = new byte[bufferLength];
			long skipStart = offset;
			long skipEnd = offset + length;
			while(true) {
				int count = stream.Read(buffer, 0, buffer.Length);
				long start = stream.Position - count;
				long end = stream.Position;
				if(start >= skipEnd || skipStart >= end)
					hashAlgorithm.TransformBlock(buffer, 0, count, null, 0);
				else {
					if(start < skipStart)
						hashAlgorithm.TransformBlock(buffer, 0, (int)(skipStart - start), null, 0);
					if(skipEnd < end) {
						hashAlgorithm.TransformBlock(buffer, (int)(skipEnd - start), count - (int)(skipEnd - start), null, 0);
					}
				}
				if(end == stream.Length)
					break;
			}
			hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
			return hashAlgorithm.Hash;
		}
		byte[] SignData(byte[] hash) {
			ContentInfo contentInfo = new ContentInfo(hash);
			SignedCms signedCms = new SignedCms(contentInfo);
			CmsSigner cmsSigner = new CmsSigner(certificate);
			bool silent = !Environment.UserInteractive || DevExpress.XtraPrinting.Native.PSNativeMethods.AspIsRunning;
			signedCms.ComputeSignature(cmsSigner, silent);
			return signedCms.Encode();
		}
	}
	public class PdfSignatureByteRange : PdfArray {
		long start;
		long end;
		protected override void WriteContent(StreamWriter writer) {
			writer.Flush();
			start = writer.BaseStream.Position;
			int firstPartLength = (int)start;
			int secondPartStart = (int)start;
			this.AddRange(new int[] { 0, firstPartLength, secondPartStart, int.MaxValue });
			base.WriteContent(writer);
			writer.Flush();
			end = writer.BaseStream.Position;
		}
		public void Patch(StreamWriter writer, long excludedRegionStart, long excludedRegionLength) {
			writer.Flush();
			int secondPartStart = (int)(excludedRegionStart + excludedRegionLength);
			int secondPartLength = (int)(writer.BaseStream.Length - secondPartStart);
			writer.BaseStream.Position = start;
			this.Clear();
			this.AddRange(new int[] { 0, (int)excludedRegionStart, secondPartStart, secondPartLength });
			base.WriteContent(writer);
			writer.Flush();
			for(long i = writer.BaseStream.Position; i < end; i++) {
				writer.Write(' ');
			}
		}
	}
}
