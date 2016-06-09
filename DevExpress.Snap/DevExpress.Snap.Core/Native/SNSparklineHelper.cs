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
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Native {
	public static class SNSparklineHelper {
		public static void SaveSparklineField(SNSparklineField sparklineField, PieceTable pieceTable, Field field, string sourceName, string fieldName) {
			SnapDocumentModel model = (SnapDocumentModel)pieceTable.DocumentModel;
			model.BeginUpdate();
			try {
				using(InstructionController controller = new InstructionController(pieceTable, sparklineField, field))
					SaveBindingValues(controller, SNSparklineField.SnSparklineDataSourceNameSwitch, sourceName, fieldName);
			}
			finally {
				model.EndUpdate();
			}
		}
		public static void SaveBindingValues(InstructionController controller, string switchName, string sourceName, string fieldName) {
			SaveArgument(controller, fieldName);
			SaveSwitchValue(controller, switchName, sourceName);
		}
		public static void SaveArgument(InstructionController controller, string argument) {
			if(!String.IsNullOrEmpty(argument))
				controller.SetArgument(0, argument);
		}
		public static void SaveSwitchValue(InstructionController controller, string switchName, string switchValue) {
			if(!String.IsNullOrEmpty(switchValue))
				controller.SetSwitch(switchName, switchValue);
			else
				controller.RemoveSwitch(switchName);
		}
	}
	public static class SparklineBindingHelper {
		public static string GetDataSourceNameFromDataInfo(SNDataInfo dataInfo, SnapFieldInfo fieldInfo, string sourceName) {
			string bindingContext = GetContextBinding(fieldInfo);
			string res = (string.IsNullOrEmpty(sourceName) || !string.IsNullOrEmpty(bindingContext)) ? string.Empty : string.Format("/{0}", sourceName);
			if(dataInfo == null || dataInfo.DataPaths.Length <= 1) return res;
			for(int i = 0; i < dataInfo.DataPaths.Length - 1; i++) {
				var part = dataInfo.DataPaths[i];
				res += string.IsNullOrEmpty(res) ? part : string.Format(".{0}", part);
			}
			if(string.Equals(res, bindingContext))
				res = string.Empty;
			else if(!string.IsNullOrEmpty(bindingContext))
				res = res.Substring(bindingContext.Length + 1);
			return res;
		}
		public static string GetContextBinding(SnapFieldInfo fieldInfo) {
			if(fieldInfo == null || fieldInfo.Field == null || fieldInfo.Field.Parent == null) return string.Empty;
			SnapListFieldInfo listFieldInfo = new SnapListFieldInfo(fieldInfo.PieceTable, fieldInfo.Field.Parent);
			DesignBinding listDesignBinding = FieldsHelper.GetFieldDesignBinding(listFieldInfo.PieceTable.DocumentModel.DataSourceDispatcher, new SnapFieldInfo(listFieldInfo.PieceTable, listFieldInfo.Field));
			if(listDesignBinding == null) return string.Empty;
			return listDesignBinding.DataMember;
		}
		public static string GetDataFieldNameFromDataInfo(SNDataInfo dataInfo) {
			if(dataInfo == null || dataInfo.DataPaths.Length == 0) return string.Empty;
			return dataInfo.DataPaths[dataInfo.DataPaths.Length - 1];
		}
	}
	[Editor("DevExpress.Snap.Extensions.Native.ActionLists.SnapDesignBindingEditor," + AssemblyInfo.SRAssemblySnapExtensions, typeof(UITypeEditor))]
	public class SnapDesignBinding : DesignBinding {
		public SnapDesignBinding() : base() { }
		public SnapDesignBinding(object dataSource, string dataMember) : base(dataSource, dataMember) { }
		public SNDataInfo SelectedFieldDataInfo { get; set; }
	}
}
