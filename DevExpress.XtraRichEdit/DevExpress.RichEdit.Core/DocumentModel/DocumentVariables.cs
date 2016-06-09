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
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	public class DocVariableValue {
		public static readonly DocVariableValue Current = new DocVariableValue();
		DocVariableValue() {
		}
	}
	#region DocumentVariableCollection
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class DocumentVariableCollection {
		readonly Dictionary<string, object> variables = new Dictionary<string, object>();
		readonly DocumentModel documentModel;
		internal DocumentVariableCollection(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		public object this[string name] {
			get { return GetVariableValue(name, new ArgumentCollection()); }
			set { SetVariableValue(name, value); }
		}
		internal Dictionary<string, object> Items { get { return variables; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DocumentVariableCollectionCount")]
#endif
		public int Count { get { return variables.Count; } }
		public void Add(string name, object value) {
			variables[name] = value;
		}
		public void Remove(string name) {
			if (variables.ContainsKey(name))
				variables.Remove(name);
		}
		public void Clear() {
			variables.Clear();
		}
		public virtual object GetVariableValue(string variableName, ArgumentCollection arguments) { 
			return GetVariableValue(variableName, arguments, null, null);
		}
		protected internal virtual object GetVariableValue(string variableName, ArgumentCollection arguments, Field field, object unhandledValue) { 
			CalculateDocumentVariableEventArgs args = new CalculateDocumentVariableEventArgs(variableName, arguments, field, unhandledValue);
			if (documentModel.RaiseCalculateDocumentVariable(args)) {
				if(args.Field != null)
					args.Field.Locked = args.FieldLocked;
				DevExpress.XtraRichEdit.Internal.InternalRichEditDocumentServer server = DevExpress.XtraRichEdit.Internal.InternalRichEditDocumentServer.TryConvertInternalRichEditDocumentServer(args.Value);
				if (server != null) {
					RichEditMailMergeOptions options = server.DocumentModel.MailMergeOptions;
					if (!options.KeepLastParagraph)
						options.KeepLastParagraph = args.KeepLastParagraph;
				}
				return args.Value;
			}
			object value;
			if (variables.TryGetValue(variableName, out value))
				return value;
			else
				return unhandledValue;
		}
		protected internal virtual void SetVariableValue(string variableName, object value) {
			if (!variables.ContainsKey(variableName))
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_VariableDeletedOrMissed);
			variables[variableName] = value;
		}
		protected internal Dictionary<string, object>.KeyCollection GetVariableNames() {
			return variables.Keys;
		}
	}
	#endregion
	#region Argument
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class Argument {
		readonly Token token;
		internal Argument(Token token) {
			Guard.ArgumentNotNull(token, "token");
			this.token = token;
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ArgumentValue")]
#endif
		public string Value { get { return token.Value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ArgumentStartPosition")]
#endif
		public int StartPosition {
			get {
				IConvertToInt<DocumentLogPosition> value = token.Position;
				return value.ToInt();
			}
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ArgumentLength")]
#endif
		public int Length { get { return token.Length; } }
	}
	#endregion
	#region ArgumentCollection
	[ComVisible(true)]
	public class ArgumentCollection : List<Argument> {
	}
	#endregion
}
