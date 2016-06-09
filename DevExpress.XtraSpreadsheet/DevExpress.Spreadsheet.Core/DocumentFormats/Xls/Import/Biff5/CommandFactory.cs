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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public class XlsBIFF5CommandFactory : IXlsCommandFactory {
		const int minRecordSize = 4; 
		readonly List<XlsCommandInfo> infos;
		readonly Dictionary<short, IXlsCommand> commandInstances;
		public XlsBIFF5CommandFactory() {
			this.infos = new List<XlsCommandInfo>();
			this.commandInstances = new Dictionary<short, IXlsCommand>();
			AddCommandInfo(new XlsCommandInfo(0x0000, typeof(XlsCommandEmpty)));
			AddCommandInfo(new XlsCommandInfo(0x0809, typeof(XlsCommandBeginOfSubstream)));
			AddCommandInfo(new XlsCommandInfo(0x000a, typeof(XlsCommandEndOfSubstream)));
			AddCommandInfo(new XlsCommandInfo(0x0042, typeof(XlsCommandEncoding)));
			AddCommandInfo(new XlsCommandInfo(0x009c, typeof(XlsCommandBuiltInFunctionGroupCount)));
			AddCommandInfo(new XlsCommandInfo(0x0019, typeof(XlsCommandWindowsProtected)));
			AddCommandInfo(new XlsCommandInfo(0x0012, typeof(XlsCommandProtected)));
			AddCommandInfo(new XlsCommandInfo(0x0013, typeof(XlsCommandPasswordVerifier)));
			AddCommandInfo(new XlsCommandInfo(0x003d, typeof(XlsCommandWorkbookWindowInformation)));
			AddCommandInfo(new XlsCommandInfo(0x0040, typeof(XlsCommandShouldSaveBackup)));
			AddCommandInfo(new XlsCommandInfo(0x008d, typeof(XlsCommandDisplayObjectsOptions)));
			AddCommandInfo(new XlsCommandInfo(0x0022, typeof(XlsCommandIs1904DateSystemUsed)));
			AddCommandInfo(new XlsCommandInfo(0x000e, typeof(XlsCommandPrecisionAsDisplayed)));
			AddCommandInfo(new XlsCommandInfo(0x01b7, typeof(XlsCommandRefreshAllOnLoading)));
			AddCommandInfo(new XlsCommandInfo(0x00da, typeof(XlsCommandWorkbookBoolProperties)));
			AddCommandInfo(new XlsCommandInfo(0x0031, typeof(XlsBIFF5CommandFont)));
			AddCommandInfo(new XlsCommandInfo(0x041e, typeof(XlsBIFF5CommandNumberFormat)));
			AddCommandInfo(new XlsCommandInfo(0x00e0, typeof(XlsBIFF5CommandXF)));
			AddCommandInfo(new XlsCommandInfo(0x0293, typeof(XlsBIFF5CommandStyle)));
			AddCommandInfo(new XlsCommandInfo(0x0892, typeof(XlsCommandCellStyleExt)));
			AddCommandInfo(new XlsCommandInfo(0x0092, typeof(XlsCommandPalette)));
			AddCommandInfo(new XlsCommandInfo(0x0085, typeof(XlsBIFF5CommandBoundSheet)));
			AddCommandInfo(new XlsCommandInfo(0x000d, typeof(XlsCommandCalculationMode)));
			AddCommandInfo(new XlsCommandInfo(0x000c, typeof(XlsCommandIterationCount)));
			AddCommandInfo(new XlsCommandInfo(0x000f, typeof(XlsCommandReferenceMode)));
			AddCommandInfo(new XlsCommandInfo(0x0011, typeof(XlsCommandIterationsEnabled)));
			AddCommandInfo(new XlsCommandInfo(0x0010, typeof(XlsCommandCalculationDelta)));
			AddCommandInfo(new XlsCommandInfo(0x005f, typeof(XlsCommandRecalculateBeforeSaved)));
			AddCommandInfo(new XlsCommandInfo(0x002a, typeof(XlsCommandPrintRowColHeadings)));
			AddCommandInfo(new XlsCommandInfo(0x002b, typeof(XlsCommandPrintGridLines)));
			AddCommandInfo(new XlsCommandInfo(0x0082, typeof(XlsCommandPrintGridLinesSet)));
			AddCommandInfo(new XlsCommandInfo(0x0225, typeof(XlsCommandDefaultRowHeight)));
			AddCommandInfo(new XlsCommandInfo(0x0081, typeof(XlsCommandAdditionalWorksheetInformation)));
			AddCommandInfo(new XlsCommandInfo(0x001b, typeof(XlsCommandHorizontalPageBreaks)));
			AddCommandInfo(new XlsCommandInfo(0x001a, typeof(XlsCommandVerticalPageBreaks)));
			AddCommandInfo(new XlsCommandInfo(0x0014, typeof(XlsBIFF5CommandPageHeader)));
			AddCommandInfo(new XlsCommandInfo(0x0015, typeof(XlsBIFF5CommandPageFooter)));
			AddCommandInfo(new XlsCommandInfo(0x0083, typeof(XlsCommandPageHCenter)));
			AddCommandInfo(new XlsCommandInfo(0x0084, typeof(XlsCommandPageVCenter)));
			AddCommandInfo(new XlsCommandInfo(0x0026, typeof(XlsCommandPageLeftMargin)));
			AddCommandInfo(new XlsCommandInfo(0x0027, typeof(XlsCommandPageRightMargin)));
			AddCommandInfo(new XlsCommandInfo(0x0028, typeof(XlsCommandPageTopMargin)));
			AddCommandInfo(new XlsCommandInfo(0x0029, typeof(XlsCommandPageBottomMargin)));
			AddCommandInfo(new XlsCommandInfo(0x00a1, typeof(XlsCommandPageSetup)));
			AddCommandInfo(new XlsCommandInfo(0x00dd, typeof(XlsCommandScenarioProtected)));
			AddCommandInfo(new XlsCommandInfo(0x0063, typeof(XlsCommandObjectsProtected)));
			AddCommandInfo(new XlsCommandInfo(0x0055, typeof(XlsCommandDefaultColumnWidth)));
			AddCommandInfo(new XlsCommandInfo(0x007d, typeof(XlsCommandColumnInfo)));
			AddCommandInfo(new XlsCommandInfo(0x0208, typeof(XlsCommandRow)));
			AddCommandInfo(new XlsCommandInfo(0x0201, typeof(XlsCommandBlank)));
			AddCommandInfo(new XlsCommandInfo(0x00be, typeof(XlsCommandMulBlank)));
			AddCommandInfo(new XlsCommandInfo(0x0205, typeof(XlsCommandBoolErr)));
			AddCommandInfo(new XlsCommandInfo(0x0203, typeof(XlsCommandNumber)));
			AddCommandInfo(new XlsCommandInfo(0x0204, typeof(XlsBIFF5CommandLabel)));
			AddCommandInfo(new XlsCommandInfo(0x00bd, typeof(XlsCommandMulRk)));
			AddCommandInfo(new XlsCommandInfo(0x027e, typeof(XlsCommandRk)));
			AddCommandInfo(new XlsCommandInfo(0x00d6, typeof(XlsBIFF5CommandRichString)));
			AddCommandInfo(new XlsCommandInfo(0x0207, typeof(XlsBIFF5CommandString)));
			PopulateCommandInstances();
		}
		void AddCommandInfo(XlsCommandInfo info) {
			infos.Add(info);
		}
		void PopulateCommandInstances() {
			for (int i = 0; i < infos.Count; i++) {
				ConstructorInfo commandConstructor = infos[i].CommandType.GetConstructor(new Type[] { });
				IXlsCommand commandInstance = commandConstructor.Invoke(new object[] { }) as IXlsCommand;
				commandInstances.Add(infos[i].TypeCode, commandInstance);
			}
		}
		public IXlsCommand CreateCommand(XlsReader reader) {
			long bytesToRead = reader.StreamLength - reader.Position;
			if (bytesToRead < minRecordSize) {
				reader.Seek(bytesToRead, SeekOrigin.Current);
				return null;
			}
			short typeCode = reader.ReadNotCryptedInt16();
			if (!commandInstances.ContainsKey(typeCode))
				typeCode = 0x0000;
			IXlsCommand command = commandInstances[typeCode];
			return command.GetInstance();
		}
	}
}
