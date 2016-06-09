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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraPrinting;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using System.ComponentModel;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Extensions.Localization;
namespace DevExpress.Snap.Extensions.Native.ActionLists {
	public class SNHyperlinkActionList : FieldActionList<SNHyperlinkField> {
		public SNHyperlinkActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[RefreshProperties(RefreshProperties.All)]
		[ResDisplayName(typeof(ResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_Text")]
		public DesignBinding Text {
			get {
				return GetTextBinding();
			}
			set {
				ApplyNewValue(UpdateText, value);
			}
		}
		[ResDisplayName(typeof(ResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_ScreenTip")]
		public string ScreenTip {
			get {
				return ParsedInfo.ScreenTip;
			}
			set {
				if (ScreenTip == value)
					return;
				ApplyNewValue(UpdateScreenTip, value);
			}
		}
		[ResDisplayName(typeof(ResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_TargetFrame")]
		public string TargetFrame {
			get {
				return ParsedInfo.Target;
			}
			set {
				if (TargetFrame == value)
					return;
				ApplyNewValue(UpdateTarget, value);
			}
		}
		DesignBinding GetTextBinding() {
			IDataSourceInfoContainer dataSourceInfoContainer = FieldInfo.PieceTable.DocumentModel.DataSources;
			if (String.IsNullOrEmpty(ParsedInfo.DisplayFieldName))
				return FieldsHelper.GetFieldDesignBinding(dataSourceInfoContainer, FieldInfo);
			Field parent = FieldsHelper.GetParent(FieldInfo.Field);
			if (parent != null)
				return FieldsHelper.GetFieldDesignBinding(dataSourceInfoContainer, new SnapFieldInfo(FieldInfo.PieceTable, parent), ParsedInfo.DisplayFieldName);
			IFieldDataAccessService dataAccessService = FieldInfo.PieceTable.DocumentModel.GetService<IFieldDataAccessService>();
			if (dataAccessService == null)
				return null;
			FieldPathInfo fieldPathInfo = dataAccessService.FieldPathService.FromString(ParsedInfo.DisplayFieldName);
			string fullPath = fieldPathInfo.GetFullPath();
			string dataMemberName = fullPath + ParsedInfo.DisplayFieldName;
			if (fieldPathInfo.DataSourceInfo.FieldDataSourceType == FieldDataSourceType.Root)
				return new DesignBinding(dataSourceInfoContainer.GetDataSourceByName(((RootFieldDataSourceInfo)fieldPathInfo.DataSourceInfo).Name), dataMemberName);
			if (dataSourceInfoContainer.DefaultDataSourceInfo == null)
				return new DesignBinding(null, string.Empty);
			return new DesignBinding(dataSourceInfoContainer.DefaultDataSourceInfo.DataSource, dataMemberName);
		}
		void UpdateText(InstructionController controller, DesignBinding binding) {
			DesignBinding fieldBinding = FieldsHelper.GetFieldDesignBinding(FieldInfo.PieceTable.DocumentModel.DataSources, FieldInfo);
			if (!fieldBinding.Equals(binding))
				controller.SetSwitch(SNHyperlinkField.DisplayFieldSwitch, binding.DataMember);
			else
				controller.RemoveSwitch(SNHyperlinkField.DisplayFieldSwitch);
		}
		void UpdateScreenTip(InstructionController controller, string value) {
			UpdateSwitchValue(controller, SNHyperlinkField.ScreenTipSwitch, value);
		}
		void UpdateTarget(InstructionController controller, string value) {
			UpdateSwitchValue(controller, SNHyperlinkField.TargetSwitch, value);
		}
		void UpdateSwitchValue(InstructionController controller, string name, string value) {
			if (!String.IsNullOrEmpty(value))
				controller.SetSwitch(name, value);
			else
				controller.RemoveSwitch(name);
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "Text", "Text");
			AddPropertyItem(actionItems, "ScreenTip", "ScreenTip");
			AddPropertyItem(actionItems, "TargetFrame", "TargetFrame");
		}
	}
}
