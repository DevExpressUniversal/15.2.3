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
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Data;
using DevExpress.XtraPrinting.Native;
#if SILVERLIGHT
using DevExpress.Xpf.ComponentModel;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
#endif
namespace DevExpress.XtraPrinting {
	public class PSCommandsExportTypeConverter : PSCommandsTypeConverter {
		static PSCommandsExportTypeConverter() {
			List<PrintingSystemCommand> supportedCommands = new List<PrintingSystemCommand>();
			foreach(PrintingSystemCommand cmd in PSCommandHelper.ExportCommands)
				if(cmd != PrintingSystemCommand.ExportXps)
					supportedCommands.Add(cmd);
			commands = supportedCommands.ToArray();
			supportedCommands.Clear();
		}
		static readonly PrintingSystemCommand[] commands;
		protected override PrintingSystemCommand[] Commands {
			get { return commands; }
		}
	}
	public class PSCommandsSendTypeConverter : PSCommandsTypeConverter {
		static PSCommandsSendTypeConverter() {
			List<PrintingSystemCommand> supportedCommands = new List<PrintingSystemCommand>();
			foreach(PrintingSystemCommand cmd in PSCommandHelper.SendCommands)
				if(cmd != PrintingSystemCommand.SendXps)
					supportedCommands.Add(cmd);
			commands = supportedCommands.ToArray();
			supportedCommands.Clear();
		}
		static readonly PrintingSystemCommand[] commands;
		protected override PrintingSystemCommand[] Commands {
			get { return commands; }
		}
	}
	public abstract class PSCommandsTypeConverter : DevExpress.Utils.Design.EnumTypeConverter {
		protected abstract PrintingSystemCommand[] Commands { get; }
		public PSCommandsTypeConverter()
			: base(typeof(PrintingSystemCommand)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(Commands);
		}
	}
}
