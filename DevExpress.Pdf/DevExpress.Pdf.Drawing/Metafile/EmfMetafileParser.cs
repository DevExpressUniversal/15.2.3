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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class EmfMetafileParser : IEnumerable<EmfRecord> {
		const int EmfCommentEmfPlusRecord = 0x2B464D45;
		readonly byte[] metafileBytes;
		public EmfMetafileParser(byte[] metafileBytes) {
			this.metafileBytes = metafileBytes;
		}
		public IEnumerator<EmfRecord> GetEnumerator() {
			using (BinaryReader reader = new BinaryReader(new MemoryStream(metafileBytes))) {
				while (reader.BaseStream.Position < reader.BaseStream.Length) {
					EmfRecordType type = (EmfRecordType)reader.ReadInt32();
					int size = reader.ReadInt32();
					if (type == EmfRecordType.EMR_COMMENT) {
						int dataSize = reader.ReadInt32();
						int emfCommentType = reader.ReadInt32();
						if (emfCommentType == EmfCommentEmfPlusRecord) {
							int readBytesCount = 8;
							while (readBytesCount < dataSize) {
								EmfPlusRecordType emfPlusType = (EmfPlusRecordType)(reader.ReadInt16());
								short flags = reader.ReadInt16();
								readBytesCount += reader.ReadInt32();
								yield return EmfPlusRecord.Create(emfPlusType, flags, reader.ReadBytes(reader.ReadInt32()));
							}
						}
						else 
							reader.ReadBytes(size - 16);
					}
					else {
						yield return EmfRecord.Create(type, reader.ReadBytes(size - 8));
						if (type == EmfRecordType.EMR_EOF)
							yield break;
					}
				}
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return (IEnumerator)GetEnumerator();
		}
	}
}
