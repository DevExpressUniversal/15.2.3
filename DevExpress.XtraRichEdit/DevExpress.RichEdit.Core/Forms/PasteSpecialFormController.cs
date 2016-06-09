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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Forms {
	#region PasteSpecialFormControllerParameters
	public class PasteSpecialFormControllerParameters : FormControllerParameters {
		readonly PasteSpecialInfo pasteSpecialInfo;
		internal PasteSpecialFormControllerParameters(IRichEditControl control, PasteSpecialInfo pasteSpecialInfo)
			: base(control) {
			Guard.ArgumentNotNull(pasteSpecialInfo, "pasteSpecialInfo");
			this.pasteSpecialInfo = pasteSpecialInfo;
		}
		internal PasteSpecialInfo PasteSpecialInfo { get { return pasteSpecialInfo; } }
	}
	#endregion
	#region PasteSpecialFormController
	public class PasteSpecialFormController : FormController {
		readonly IRichEditControl control;
		readonly PasteSpecialInfo sourcePasteSpecialInfo;
		readonly PasteSpecialInfo pasteSpecialInfo;
		IList<Type> availableCommandTypes;
		public PasteSpecialFormController(PasteSpecialFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.sourcePasteSpecialInfo = controllerParameters.PasteSpecialInfo;
			this.pasteSpecialInfo = sourcePasteSpecialInfo.Clone();
#if SL
			availableCommandTypes = GetAvailableCommandTypes();
#endif
		}
		public IRichEditControl Control { get { return control; } }
		protected internal PasteSpecialInfo SourcePasteSpecialInfo { get { return sourcePasteSpecialInfo; } }
		public Type PasteCommandType { get { return pasteSpecialInfo.PasteCommandType; } set { pasteSpecialInfo.PasteCommandType = value; } }
		public override void ApplyChanges() {
			sourcePasteSpecialInfo.CopyFrom(pasteSpecialInfo);
		}
		public IList<Type> AvailableCommandTypes {
			get {
				if (availableCommandTypes == null)
					availableCommandTypes = GetAvailableCommandTypes();
				return availableCommandTypes;
			}
		}
		IList<Type> GetAvailableCommandTypes() {
			List<Type> result = new List<Type>();
			PasteSelectionCommand command = new PasteSelectionCommand(Control);
			PasteSelectionCoreCommand coreCommand = (PasteSelectionCoreCommand)command.InsertObjectCommand;
			CommandCollection commands = coreCommand.Commands;
			int count = commands.Count;
			for (int i = 0; i < count; i++) {
				PasteContentCommandBase pasteCommand = commands[i] as PasteContentCommandBase;
				if (pasteCommand != null && pasteCommand.CanExecute())
					result.Add(pasteCommand.GetType());
			}
			return result;
		}
	}
	#endregion
	#region PasteSpecialInfo
	public class PasteSpecialInfo : ICloneable<PasteSpecialInfo>, ISupportsCopyFrom<PasteSpecialInfo> {
		Type pasteCommandType = typeof(PasteSelectionCommand);
		public Type PasteCommandType { get { return pasteCommandType; } set { pasteCommandType = value; } }
		#region ICloneable<PasteSpecialInfo> Members
		public PasteSpecialInfo Clone() {
			PasteSpecialInfo clone = new PasteSpecialInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<PasteSpecialInfo> Members
		public void CopyFrom(PasteSpecialInfo value) {
			this.PasteCommandType = value.PasteCommandType;
		}
		#endregion
		internal PasteContentCommandBase CreateCommand(IRichEditControl control) {
			return CreateCommand(control, PasteCommandType);
		}
		internal static PasteContentCommandBase CreateCommand(IRichEditControl control, Type commandType) {
			try {
				ConstructorInfo ci = commandType.GetConstructor(new Type[] { typeof(IRichEditControl) });
				if (ci == null)
					return null;
				PasteContentCommandBase result = ci.Invoke(new object[] { control }) as PasteContentCommandBase;
				if (result != null)
					result.PasteSource = new ClipboardPasteSource();
				return result;
			}
			catch {
				return null;
			}
		}
	}
	#endregion
	#region PasteSpecialListBoxItem
	public class PasteSpecialListBoxItem {
		readonly IRichEditControl control;
		readonly Type commandType;
		public PasteSpecialListBoxItem(IRichEditControl control, Type commandType) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.commandType = commandType;
		}
		public IRichEditControl Control { get { return control; } }
		public Type CommandType { get { return commandType; } }
		public override string ToString() {
			PasteContentCommandBase command = PasteSpecialInfo.CreateCommand(control, CommandType);
			if (command == null)
				return String.Empty;
			else
				return command.MenuCaption;
		}
	}
	#endregion
}
