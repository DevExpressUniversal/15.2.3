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

using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
namespace DevExpress.Pdf.Native {
	public class PdfSignatureContents : PdfPlaceholder {
		const int bufferLength = 32768;
		static byte[] SignData(X509Certificate2 certificate, byte[] hash) {
			SignedCms signedCms = new SignedCms(new ContentInfo(hash));
			signedCms.ComputeSignature(new CmsSigner(certificate), true);
			return signedCms.Encode();
		}
		static int CalculateLength(X509Certificate2 certificate) {
			using (SHA1 sha1 = SHA1.Create()) {
				StringBuilder sb = new StringBuilder();
				sb.Append('<');
				foreach (byte b in SignData(certificate, sha1.ComputeHash(new byte[] { 1 })))
					sb.AppendFormat("{0:x2}", b);
				sb.Append('>');
				return Encoding.UTF8.GetBytes(sb.ToString()).Length;
			}
		}
		readonly X509Certificate2 certificate;
		public PdfSignatureContents(X509Certificate2 certificate) : base(CalculateLength(certificate)) {
			this.certificate = certificate;
		}
		public override void PatchStream(PdfDocumentStream stream) {
			base.PatchStream(stream);
			byte[] hash;
			using (PdfUnsignedDocumentStream chunk = new PdfUnsignedDocumentStream(stream, Offset, Length))
				using (SHA1 sha1 = SHA1.Create())
					hash = sha1.ComputeHash(chunk);
			stream.WriteObject(SignData(certificate, hash), Number);
		}
		public override void Write(PdfDocumentWritableStream stream, int number) {
			PdfDocumentStream documentStream = (PdfDocumentStream)stream;
			Offset = (int)documentStream.Position;
			documentStream.SetSignatureContents(this);
			base.Write(stream, number);
		}
	}
}
