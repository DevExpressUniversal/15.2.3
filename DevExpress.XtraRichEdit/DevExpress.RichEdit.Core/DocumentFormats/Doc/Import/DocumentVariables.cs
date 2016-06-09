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

using System.Collections.Generic;
using System.IO;
using DevExpress.Utils;
using System.Text;
using System.Diagnostics;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class DocumentVariables {
		#region static
		public static DocumentVariables FromStream(BinaryReader reader, int offset, int size) {
			DocumentVariables result = new DocumentVariables();
			result.Read(reader, offset, size);
			return result;
		}
		public static DocumentVariables FromVariablesCollection(DocumentVariableCollection variables, DevExpress.XtraRichEdit.Model.DocumentProperties documentProperties) {
			DocumentVariables result = new DocumentVariables();
			result.GetVariables(variables, documentProperties);
			return result;
		}
		#endregion
		#region Fields
		const int extraDataSize = 4;
		List<string> names;
		List<string> values;
		#endregion
		protected DocumentVariables() {
			this.values = new List<string>();
		}
		#region Properties
		public List<string> Names { get { return names; } }
		public List<string> Values { get { return values; } }
		#endregion
		protected void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			DocStringTable variablesNames = DocStringTable.FromStream(reader, offset, size);
			this.names = variablesNames.Data;
			int count = this.names.Count;
			for (int i = 0; i < count; i++) {
				int length = reader.ReadUInt16() * 2;
				byte[] buffer = reader.ReadBytes(length);
				string value = Encoding.Unicode.GetString(buffer, 0, buffer.Length);
				Values.Add(value);
			}
		}
		public void Write(BinaryWriter writer) {
#if DEBUGTEST || DEBUG
			Debug.Assert(Names.Count == Values.Count);
#endif
			DocStringTable variableNames = new DocStringTable(Names);
			variableNames.ExtraDataSize = extraDataSize;
			variableNames.Write(writer);
			int count = this.values.Count;
			for (int i = 0; i < count; i++) {
				writer.Write((ushort)Values[i].Length);
				writer.Write(Encoding.Unicode.GetBytes(Values[i]));
			}
		}
		protected void GetVariables(DocumentVariableCollection variables, DevExpress.XtraRichEdit.Model.DocumentProperties documentProperties) {
			this.names = new List<string>();
			foreach (string name in variables.GetVariableNames()) {
				this.names.Add(name);
				object value = variables[name];
				if (value == null || value == DocVariableValue.Current)
					value = System.String.Empty;
				this.values.Add(value.ToString());
			}
			DevExpress.XtraRichEdit.UpdateDocVariablesBeforePrint updateFieldsBeforePrint = documentProperties.UpdateDocVariablesBeforePrint;
			if (updateFieldsBeforePrint != UpdateDocVariablesBeforePrint.Auto) {
				this.names.Add(DevExpress.XtraRichEdit.Model.DocumentProperties.UpdateDocVariableFieldsBeforePrintDocVarName);
				this.values.Add(documentProperties.GetUpdateFieldsBeforePrintDocVarValue());
			}
		}
		public void SetVariables(DocumentVariableCollection variables, DevExpress.XtraRichEdit.Model.DocumentProperties documentProperties) {
			int count = this.names.Count;
			for (int i = 0; i < count; i++) {
				string name = this.names[i];
				if (name != DevExpress.XtraRichEdit.Model.DocumentProperties.UpdateDocVariableFieldsBeforePrintDocVarName)
					variables.Add(this.names[i], this.values[i]);
				else
					documentProperties.SetUpdateFieldsBeforePrintFromDocVar(values[i]);
			}
		}
	}
}
