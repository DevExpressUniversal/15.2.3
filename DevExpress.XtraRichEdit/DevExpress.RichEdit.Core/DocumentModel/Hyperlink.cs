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
using System.Collections;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Model {
	#region TextToDisplaySource
	public enum TextToDisplaySource { ExistingText, NewText }
	#endregion
	#region HyperlinkInfo
	public class HyperlinkInfo : ISupportsCopyFrom<HyperlinkInfo>, ICloneable<HyperlinkInfo> {
		static readonly HyperlinkInfo empty = new HyperlinkInfo();
		public static HyperlinkInfo Empty { get { return empty; } }
		#region Fields
		bool visited;
		string navigateUri = String.Empty;
		string anchor = String.Empty;
		string toolTip = String.Empty;
		string target = String.Empty;
		#endregion
		#region Properties
		public string NavigateUri { get { return navigateUri; } set { navigateUri = value; } }
		public string Anchor { get { return anchor; } set { anchor = value; } }
		public string ToolTip { get { return toolTip; } set { toolTip = value; } }
		public string Target { get { return target; } set { target = value; } }
		public bool Visited { get { return visited; } set { visited = value; } }
		#endregion
		protected internal string GetActualToolTip() {
			if (!String.IsNullOrEmpty(ToolTip))
				return ToolTip;
			if (String.IsNullOrEmpty(NavigateUri) && !String.IsNullOrEmpty(Anchor) && Anchor.StartsWith("_"))
				return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_CurrentDocumentHyperlinkTooltip);
			string result = String.Format("{0}#{1}", NavigateUri, Anchor);
			return result.Trim('#');
		}
		protected internal string CreateUrl() {
			if(String.IsNullOrEmpty(NavigateUri))
				return string.Empty;
			if (!String.IsNullOrEmpty(Anchor))
				return String.Format("{0}#{1}", NavigateUri, Anchor);
			return NavigateUri;
		}
		#region ISupportsCopyFrom<HyperlinkInfo> Members
		public void CopyFrom(HyperlinkInfo value) {
			NavigateUri = value.NavigateUri;
			Anchor = value.Anchor;
			ToolTip = value.ToolTip;
			Target = value.Target;
			Visited = value.Visited;
		}
		#endregion
		#region ICloneable<HyperlinkInfo> Members
		public HyperlinkInfo Clone() {
			HyperlinkInfo clone = new HyperlinkInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
	}
	#endregion
	#region HyperlinkInfoCollection
	public class HyperlinkInfoCollection : IEnumerable {
		Dictionary<int, HyperlinkInfo> hyperlinkInfos;
		public HyperlinkInfoCollection() {
			this.hyperlinkInfos = new Dictionary<int, HyperlinkInfo>();
		}
		public HyperlinkInfo this[int fieldIndex] { get { return hyperlinkInfos[fieldIndex]; } set { hyperlinkInfos[fieldIndex] = value; } }
		public int Count { get { return hyperlinkInfos.Count; } }
		public void Clear() {
			hyperlinkInfos.Clear();
		}
		public void Add(int fieldIndex, HyperlinkInfo info) {
			hyperlinkInfos.Add(fieldIndex, info);
		}
		public bool Remove(int fieldIndex) {
			return hyperlinkInfos.Remove(fieldIndex);
		}
		public bool IsHyperlink(int fieldIndex) {
			return hyperlinkInfos.ContainsKey(fieldIndex);
		}
		public bool TryGetHyperlinkInfo(int fieldIndex, out HyperlinkInfo info) {
			return hyperlinkInfos.TryGetValue(fieldIndex, out info);
		}
		protected internal virtual void RecalcKeys(int from, int deltaIndex) {
			Dictionary<int, HyperlinkInfo> result = new Dictionary<int, HyperlinkInfo>();
			foreach (KeyValuePair<int, HyperlinkInfo> row in hyperlinkInfos) {
				if (row.Key >= from)
					result.Add(row.Key + deltaIndex, row.Value);
				else
					result.Add(row.Key, row.Value);
			}
			this.hyperlinkInfos = result;
		}
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return hyperlinkInfos.Keys.GetEnumerator();
		}
		#endregion
	}
	#endregion
	public delegate object GetSwitchValue(HyperlinkInstructionBuilder builder);
	#region HyperlinkInstructionBuilder
	public class HyperlinkInstructionBuilder {
		Dictionary<string, GetSwitchValue> attributes = CreateAttributes();
		static Dictionary<string, GetSwitchValue> CreateAttributes() {
			Dictionary<string, GetSwitchValue> result = new Dictionary<string, GetSwitchValue>();
			result.Add("l", GetAnchor);
			result.Add("o", GetTooltip);
			result.Add("t", GetTarget);
			return result;
		}
		static object GetAnchor(HyperlinkInstructionBuilder builder) {
			string anchor = builder.HyperlinkInfo.Anchor;
			return !String.IsNullOrEmpty(anchor) ? anchor : null;
		}
		static object GetTarget(HyperlinkInstructionBuilder builder) {
			string target = builder.HyperlinkInfo.Target;
			return !String.IsNullOrEmpty(target) ? target : null;
		}
		static object GetTooltip(HyperlinkInstructionBuilder builder) {
			string tooltip = builder.HyperlinkInfo.ToolTip;
			if (String.IsNullOrEmpty(tooltip))
				return null;
			tooltip = HyperlinkUriHelper.EscapeHyperlinkFieldParameterString(tooltip);
			return HyperlinkUriHelper.PrepareHyperlinkTooltipQuotes(tooltip);
		}
		readonly HyperlinkInfo hyperlinkInfo;
		public HyperlinkInstructionBuilder(HyperlinkInfo info) {
			Guard.ArgumentNotNull(info, "info");
			this.hyperlinkInfo = info;
		}
		public HyperlinkInfo HyperlinkInfo { get { return hyperlinkInfo; } }
		public virtual Dictionary<string, GetSwitchValue> Attributes { get { return attributes; } } 
		public virtual string FieldType { get { return HyperlinkField.FieldType; } } 
		public virtual string GetFieldInstruction() {
			StringBuilder result = new StringBuilder(FieldType);
			string argument = GetArgument();
			if (!String.IsNullOrEmpty(argument))
				result.AppendFormat(" \"{0}\"", argument);
			string individualSwitches = GetIndividualSwitches();
			if (!String.IsNullOrEmpty(individualSwitches)) {
				result.Append(" ");
				result.Append(individualSwitches);
			}
			return result.ToString();
		}
		protected internal virtual string GetIndividualSwitches() {
			List<string> result = new List<string>();
			foreach (KeyValuePair<string, GetSwitchValue> attribute in Attributes) {
				GetSwitchValue callback = attribute.Value;
				object value = callback(this);
				if (value != null)
					result.Add(String.Format(@"\{0} ""{1}""", attribute.Key, value.ToString()));
			}
			return String.Join(" ", result.ToArray());
		}
		protected virtual string GetArgument() { 
			if (String.IsNullOrEmpty(HyperlinkInfo.NavigateUri))
				return String.Empty;
			return HyperlinkUriHelper.ConvertToHyperlinkUri(HyperlinkInfo.NavigateUri);
		}
	}
	#endregion
}
