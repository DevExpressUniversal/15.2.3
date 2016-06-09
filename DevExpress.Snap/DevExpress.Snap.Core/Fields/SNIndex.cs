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
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Utils.Design;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Fields {
	[TypeConverter(typeof(EnumTypeConverter)), ResourceFinder(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum GroupIndexMode {
		Restarted,
		Continued
	}
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.FormatStringActionList, " + AssemblyInfo.SRAssemblySnapExtensions, 0)]
	[ActionList("DevExpress.Snap.Extensions.Native.ActionLists.SNIndexActionList," + AssemblyInfo.SRAssemblySnapExtensions, 1)]
	public class SNIndexField : SNMergeFieldBase {
		#region static
		public static new readonly string FieldType = "SNINDEX";
		static readonly string NullTextFormat = "<<{0}>>";
		internal static readonly Dictionary<GroupIndexMode, string> groupIndexModeDictionary = new Dictionary<GroupIndexMode, string>();
		static readonly Dictionary<string, GroupIndexMode> updateGroupIndexMode = CreateUpdateGroupIndexModes();
		static readonly Dictionary<string, bool> indexSwithcesWithArguments;
		public static readonly string IndexModeSwitch = "im";
		static SNIndexField() {
			indexSwithcesWithArguments = CreateSwitchesWithArgument(IndexModeSwitch);
			foreach (KeyValuePair<string, bool> sw in MergefieldField.SwitchesWithArgument)
				indexSwithcesWithArguments.Add(sw.Key, sw.Value);
			groupIndexModeDictionary.Add(GroupIndexMode.Continued, "continued");
			groupIndexModeDictionary.Add(GroupIndexMode.Restarted, "restarted");
		}
		public static new CalculatedFieldBase Create() {
			return new SNIndexField();
		}
		static Dictionary<string, GroupIndexMode> CreateUpdateGroupIndexModes() {
			Dictionary<string, GroupIndexMode> result = new Dictionary<string, GroupIndexMode>();
			result.Add("restarted", GroupIndexMode.Restarted);
			result.Add("continued", GroupIndexMode.Continued);
			return result;
		}
		public static string GetGroupIndexModeString(GroupIndexMode groupMode) {
			return groupIndexModeDictionary[groupMode];
		}
		#endregion
		GroupIndexMode groupIndexMode = GroupIndexMode.Restarted;
		public GroupIndexMode GroupMode { get { return groupIndexMode; } }
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return indexSwithcesWithArguments; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection switches) {
			base.Initialize(pieceTable, switches);
			SetMode(switches);
		}
		void SetMode(InstructionCollection switches) {
			string updateString = switches.GetString(IndexModeSwitch);
			if (String.IsNullOrEmpty(updateString))
				return;
			GroupIndexMode updateIndexMode;
			if (updateGroupIndexMode.TryGetValue(updateString, out updateIndexMode))
				this.groupIndexMode = updateIndexMode;
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			IFieldDataAccessService fieldDataAccessSrv = sourcePieceTable.DocumentModel.GetService<IFieldDataAccessService>();
			if (fieldDataAccessSrv == null)
				return null;
			FieldDataValueDescriptor descriptor = fieldDataAccessSrv.GetFieldValueDescriptor((SnapPieceTable)sourcePieceTable, documentField, DataFieldName);
			ISingleObjectFieldContext context = descriptor.ParentDataContext as ISingleObjectFieldContext;
			if (context != null)
				if (GroupMode == GroupIndexMode.Restarted)
					return new CalculatedFieldValue(context.CurrentRecordIndexInGroup);
				else
					return new CalculatedFieldValue(context.CurrentRecordIndex);
			return new CalculatedFieldValue(String.Format(NullTextFormat, DataFieldName)); 
		}
		protected internal override string[] GetNativeSwithes() {
			return new string[] { IndexModeSwitch};
		}
	}
}
