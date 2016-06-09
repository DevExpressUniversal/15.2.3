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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Utils.Serializing.Helpers;
using System.Xml;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using DevExpress.Compatibility.System.Collections.Specialized;
namespace DevExpress.Utils.Serializing {
	#region TypedBinaryWriterExWithStringTable
	public class TypedBinaryWriterExWithStringTable : TypedBinaryWriterEx {
		bool closed;
		int startPosition = -1;
		StringCollection stringTable;
		public TypedBinaryWriterExWithStringTable(BinaryWriter output)
			: base(output) {
			this.stringTable = new StringCollection();
		}
		public override void WriteObject(object obj) {
			CorrectStartPosition();
			base.WriteObject(obj);
		}
		void CorrectStartPosition() {
			if(startPosition == -1) {
				Output.Write((Int32)0);
				startPosition = (Int32)Output.BaseStream.Position;
			}
		}
		public void WriteCustomObject(Type type, string serializedObject) {
			CorrectStartPosition();
			base.WriteObjectCore(type, serializedObject, true);
		}
		public override void Close() {
			if(!closed) {
				Flush();
				int currentPosition = (int)Output.BaseStream.Position;
				Output.Seek(startPosition - sizeof(Int32), SeekOrigin.Begin);
				Output.Write(currentPosition - startPosition);
				Flush();
				Output.Seek(currentPosition, SeekOrigin.Begin);
				WriteStringTable();
				closed = true;
			}
			base.Close();
		}
		protected internal virtual void WriteStringTable() {
			int count = stringTable.Count;
			WriteInt32(count);
			for(int i = 0; i < count; i++)
				Output.Write(stringTable[i]);
		}
		protected internal override void WriteString(string val) {
			Output.Write((byte)DXTypeCode.String);
			int stringIndex = stringTable.IndexOf(val);
			if (stringIndex < 0) {
				stringIndex = stringTable.Count;
				stringTable.Add(val);
			}
			WriteInt32(stringIndex);
		}
	}
	#endregion
}
