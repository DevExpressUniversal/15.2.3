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
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
using DevExpress.SpreadsheetSource.Implementation;
namespace DevExpress.SpreadsheetSource.Xls {
	using DevExpress.Office.Utils;
	using DevExpress.XtraExport.Xls;
	#region IXlsSourceCommandFactory
	public interface IXlsSourceCommandFactory {
		IXlsSourceCommand CreateCommand(XlsReader reader);
		IXlsSourceCommand CreateCommand(int typeCode);
	}
	#endregion
	#region XlsSourceCommandFactory
	public class XlsSourceCommandFactory : IXlsSourceCommandFactory {
		const int minRecordSize = 4; 
		readonly Dictionary<int, IXlsSourceCommand> commands;
		public XlsSourceCommandFactory() {
			this.commands = new Dictionary<int, IXlsSourceCommand>();
			InitializeFactory();
		}
		void InitializeFactory() {
			this.commands.Add(0x0000, new XlsSourceCommandEmpty());
			this.commands.Add(0x0809, new XlsSourceCommandBOF());
			this.commands.Add(0x000a, new XlsSourceCommandEOF());
			this.commands.Add(0x002f, new XlsSourceCommandFilePassword());
			this.commands.Add(0x0022, new XlsSourceCommandDate1904());
			this.commands.Add(0x0085, new XlsSourceCommandBoundSheet8());
			this.commands.Add(0x041e, new XlsSourceCommandFormat());
			this.commands.Add(0x00e0, new XlsSourceCommandXF());
			this.commands.Add(0x00fc, new XlsSourceCommandSharedStrings());
			this.commands.Add(0x003c, new XlsSourceCommandContinue());
			this.commands.Add(0x01ae, new XlsSourceCommandSupBook());
			this.commands.Add(0x0017, new XlsSourceCommandExternSheet());
			this.commands.Add(0x0018, new XlsSourceCommandDefinedName());
			this.commands.Add(0x0894, new XlsSourceCommandNameComment());
			this.commands.Add(0x020b, new XlsSourceCommandIndex());
			this.commands.Add(0x0055, new XlsSourceCommandDefaultColumnWidth());
			this.commands.Add(0x007d, new XlsSourceCommandColumnInfo());
			this.commands.Add(0x00d7, new XlsSourceCommandDbCell());
			this.commands.Add(0x0208, new XlsSourceCommandRow());
			this.commands.Add(0x0201, new XlsSourceCommandBlank());
			this.commands.Add(0x00be, new XlsSourceCommandMulBlank());
			this.commands.Add(0x0205, new XlsSourceCommandBoolErr());
			this.commands.Add(0x0203, new XlsSourceCommandNumber());
			this.commands.Add(0x0204, new XlsSourceCommandLabel());
			this.commands.Add(0x00fd, new XlsSourceCommandLabelSst());
			this.commands.Add(0x00bd, new XlsSourceCommandMulRk());
			this.commands.Add(0x027e, new XlsSourceCommandRk());
			this.commands.Add(0x0006, new XlsSourceCommandFormula());
			this.commands.Add(0x0207, new XlsSourceCommandString());
			this.commands.Add(0x0872, new XlsSourceCommandFeature11());
			this.commands.Add(0x0878, new XlsSourceCommandFeature11());
		}
		#region IXlsSourceCommandFactory Members
		public IXlsSourceCommand CreateCommand(XlsReader reader) {
			long bytesToRead = reader.StreamLength - reader.Position;
			if(bytesToRead < minRecordSize) {
				reader.Seek(bytesToRead, SeekOrigin.Current);
				return null;
			}
			int typeCode = reader.ReadNotCryptedInt16();
			return CreateCommand(typeCode);
		}
		public IXlsSourceCommand CreateCommand(int typeCode) {
			if(!commands.ContainsKey(typeCode))
				typeCode = 0x0000;
			return commands[typeCode];
		}
		#endregion
	}
	#endregion
}
