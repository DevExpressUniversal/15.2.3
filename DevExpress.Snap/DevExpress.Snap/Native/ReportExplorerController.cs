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

using System.Collections;
using System.Collections.Generic;
using DevExpress.Snap.Core;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.Snap.Native {
	public class ReportExplorerController {
		public ISnapControl Control { get; set; }
		protected SnapPieceTable PieceTable { get { return (SnapPieceTable)Control.InnerControl.DocumentModel.MainPieceTable; } }
		protected DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		public void FillNodes(Field parent, XtraListNodes nodes) {
			Pair<Field, string>[] rootFieldsPairs = GetFieldsWithParent(parent);
			FillNodesInternal(rootFieldsPairs, nodes);
		}
		public void FillNodesByParentList(Field field, XtraListNodes nodes) {
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			SNListField snList = calculator.ParseField(PieceTable, field) as SNListField;
			if (snList != null)
				FillNodesInternal(new Pair<Field, string>[] { new Pair<Field, string>(field, snList.Name) }, nodes);
		}
		void FillNodesInternal(Pair<Field, string>[] rootFieldsPairs, XtraListNodes nodes) {
			foreach (var rootField in rootFieldsPairs) {
				if (!Validate(rootField))
					continue;
				XtraListNode node = new XtraListNode(string.Format("{0} - {1}", SnapLocalizer.GetString(SnapStringId.ReportExplorer_ListNode), rootField.Second), nodes);
				((IList)nodes).Add(node);
				node.Tag = new Pair<string, int>(rootField.Second, -1);
				node.StateImageIndex = 0;
				FillGroups(rootField.First, (XtraListNodes)node.Nodes);
				FillNodes(rootField.First, (XtraListNodes)node.Nodes);
			}
		}
		protected virtual bool Validate(Pair<Field, string> rootField) {
			return true;
		}
		void FillGroups(Field field, XtraListNodes nodes) {
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			SNListField snList = calculator.ParseField(PieceTable, field) as SNListField;
			if (snList != null) {
				IFieldPathService fieldPathService = DocumentModel.GetService<IFieldDataAccessService>().FieldPathService;
				FieldPathInfo info = fieldPathService.FromString(snList.DataSourceName);
				if (info.DataMemberInfo.Items.Count > 0 && info.DataMemberInfo.Items[0].Groups != null) {
					for (int i = 0; i < info.DataMemberInfo.Items[0].Groups.Count; i++) {
						GroupProperties groupProperties = info.DataMemberInfo.Items[0].Groups[i];
						if (groupProperties.HasGroupTemplates) {
							string gInfo = string.Empty;
							for (int j = 0; j < groupProperties.GroupFieldInfos.Count - 1; j++) {
								string groupFieldName = groupProperties.GroupFieldInfos[j].FieldName;
								string groupFieldDisplayName = FieldsHelper.GetFieldDisplayName(PieceTable.DocumentModel.DataSources, new SnapListFieldInfo(PieceTable, field, snList), groupFieldName, groupFieldName);
								gInfo += groupFieldDisplayName + ", ";
							}
							string lastGroupFieldName = groupProperties.GroupFieldInfos[groupProperties.GroupFieldInfos.Count - 1].FieldName;
							string lastGroupFieldDisplayName = FieldsHelper.GetFieldDisplayName(PieceTable.DocumentModel.DataSources, new SnapListFieldInfo(PieceTable, field, snList), lastGroupFieldName, lastGroupFieldName);
							gInfo += lastGroupFieldDisplayName;
							XtraListNode node = new XtraListNode(string.Format("{0} - ({1})", SnapLocalizer.GetString(SnapStringId.ReportExplorer_GroupNode), gInfo), nodes);
							((IList)nodes).Add(node);
							node.StateImageIndex = 1;
							node.Tag = new Pair<string, int>(snList.Name, i);
						}
					}
				}
			}
		}
		Pair<Field, string>[] GetFieldsWithParent(Field parent) {
			List<Pair<Field, string>> result = new List<Pair<Field, string>>();
			PieceTable.Fields.ForEach(field => {
				if (field.Parent != parent)
					if (field.Parent == null || !field.Parent.DisableUpdate || field.Parent.Parent != parent)
						return;
				if (parent != null && field.FirstRunIndex >= parent.Code.End)
					return;
				SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
				SNListField snList = calculator.ParseField(PieceTable, field) as SNListField;
				if (snList != null) {
					result.Add(new Pair<Field, string>(field, snList.Name));
				}
			}
			);
			return result.ToArray();
		}
		public void SwapNodeTags(TreeListNode first, TreeListNode second) {
			object tmp = first.Tag;
			first.Tag = second.Tag;
			second.Tag = tmp;
		}
	}
	public class ReorderingReportExplorerController : ReportExplorerController {
		List<string> listNames = new List<string>();
		protected override bool Validate(Pair<Field, string> rootField) {
			if (listNames.Contains(rootField.Second))
				return false;
			listNames.Add(rootField.Second);
			return true;
		}
	}
}
