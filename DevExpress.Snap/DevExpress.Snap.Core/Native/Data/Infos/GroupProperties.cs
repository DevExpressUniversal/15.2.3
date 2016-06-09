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
namespace DevExpress.Snap.Core.Native.Data {
	public class GroupProperties {
		List<GroupFieldInfo> groupFieldInfos;
		public GroupProperties() {
			groupFieldInfos = new List<GroupFieldInfo>();
		}
		public string TemplateHeaderSwitch { get; set; }
		public string TemplateFooterSwitch { get; set; }
		public string TemplateSeparatorSwitch { get; set; }
		public bool HasTemplateHeader { get { return !String.IsNullOrEmpty(TemplateHeaderSwitch); } }
		public bool HasTemplateFooter { get { return !String.IsNullOrEmpty(TemplateFooterSwitch); } }
		public bool HasTemplateSeparator { get { return !String.IsNullOrEmpty(TemplateSeparatorSwitch); } }
		public bool HasGroupTemplates { get { return HasTemplateHeader || HasTemplateFooter; } }
		public List<GroupFieldInfo> GroupFieldInfos { get { return groupFieldInfos; } }
		public GroupFieldInfo AddField(string fieldName) {
			GroupFieldInfo result = new GroupFieldInfo(fieldName);
			groupFieldInfos.Add(result);
			return result;
		}
		public override string ToString() {
			int count = groupFieldInfos.Count;
			string[] groupFieldInfoStrings = new string[count];
			for (int i = 0; i < count; i++)
				groupFieldInfoStrings[i] = groupFieldInfos[i].ToString();
			return String.Format("{0},{1},{2},{3}", TemplateHeaderSwitch, TemplateFooterSwitch, TemplateSeparatorSwitch, String.Join(",", groupFieldInfoStrings));
		}
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(this, obj))
				return true;
			GroupProperties other = obj as GroupProperties;
			if (Object.ReferenceEquals(other, null))
				return false;
			return ListUtils.AreEquals(groupFieldInfos, other.GroupFieldInfos);
		}
		public override int GetHashCode() {
			return ListUtils.CalcHashCode(groupFieldInfos);
		}
	}
}
