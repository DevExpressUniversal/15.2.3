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
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.API.Native;
namespace DevExpress.Snap.Core.API {
	public interface SnapListGroupInfo : IList<SnapListGroupParam> {
		SnapDocument Header { get; }
		SnapDocument Footer { get; }
		SnapDocument Separator { get; }
		SnapDocument CreateHeader();
		SnapDocument CreateFooter();
		SnapDocument CreateSeparator();
		void RemoveHeader();
		void RemoveFooter();
		void RemoveSeparator();
	}
}
namespace DevExpress.Snap.API.Native {
	using DevExpress.Snap.Core.API;
	using DevExpress.Snap.Core.Native.Data;
	using DevExpress.Snap.Core.Native;
	using DevExpress.Snap.Core;
	public class NativeSnapListGroupInfo : SnapListGroupInfo {
		GroupProperties group;
		SnapDocument header;
		SnapDocument footer;
		SnapDocument separator;
		bool headerReady;
		bool footerReady;
		bool separatorReady;
		NativeSnapList list;
		Dictionary<string, InternalSnapDocumentServer> servers;
		bool obsolete;
		public NativeSnapListGroupInfo(NativeSnapList list, GroupProperties group) {
			this.list = list;
			this.group = group;
			this.servers = new Dictionary<string, InternalSnapDocumentServer>();
			headerReady = !group.HasTemplateHeader;
			footerReady = !group.HasTemplateFooter;
			separatorReady = !group.HasTemplateSeparator;
		}
		public void EndUpdate() {
			foreach (KeyValuePair<string, InternalSnapDocumentServer> pair in servers) 
				list.Controller.SetSwitch(pair.Key, pair.Value.DocumentModel, true);
			servers.Clear();
			if(header != null) {
				header = null;
				headerReady = false;
			}
			if(footer != null) {
				footer = null;
				footerReady = false;
			}
			if(separator != null) {
				separator = null;
				separatorReady = false;
			}
			SetObsolete();
		}
		internal GroupProperties Group { get { EnsureValid(); return group; } }
		void PrepareHeader() {
			header = group.HasTemplateHeader ? GetTemplate(group.TemplateHeaderSwitch) : null;
			headerReady = true;
		}
		void PrepareFooter() {
			footer = group.HasTemplateFooter ? GetTemplate(group.TemplateFooterSwitch) : null;
			footerReady = true;
		}
		void PrepareSeparator() {
			separator = group.HasTemplateSeparator ? GetTemplate(group.TemplateSeparatorSwitch) : null;
			separatorReady = true;
		}
		SnapNativeDocument GetTemplate(string switchName) {
			if(servers.ContainsKey(switchName))
				return (SnapNativeDocument) servers[switchName].Document;
			DocumentRange range = list.GetSwitchValue(switchName);
			SnapNativeDocument doc = list.Document as SnapNativeDocument;
			InternalSnapDocumentServer server = new InternalSnapDocumentServer(doc.DocumentModel.DocumentFormatsDependencies);
			DevExpress.XtraRichEdit.Internal.CopyHelper.CopyCore(doc.PieceTable, server.DocumentModel.MainPieceTable, new XtraRichEdit.Fields.DocumentLogInterval(range.Start.LogPosition, range.Length), DocumentLogPosition.Zero, true, true, UpdateFieldOperationType.Normal);
			server.DocumentModel.Modified = false;
			server.DocumentModel.ModifiedChanged += DocumentModel_ModifiedChanged;
			servers.Add(switchName, server);
			return (SnapNativeDocument)server.Document;
		}
		static GroupFieldInfo SnapListGroupParamToGroupFieldInfo(SnapListGroupParam value) { return new GroupFieldInfo(value.FieldName) { SortOrder = value.SortOrder, GroupInterval = value.Interval }; }
		static SnapListGroupParam GroupFieldInfoToSnapListGroupParam(GroupFieldInfo value) { return new SnapListGroupParam(value.FieldName, value.SortOrder, value.GroupInterval); }
		void DocumentModel_ModifiedChanged(object sender, EventArgs e) {
			SnapDocumentModel model = sender as SnapDocumentModel;
			if(model != null && model.Modified) {
				list.EnsureUpdateBegan();
				model.ModifiedChanged -= DocumentModel_ModifiedChanged;
			}
		}
		void EnsureValid() { if(obsolete) throw new InvalidOperationException(DevExpress.Snap.Localization.SnapLocalizer.GetString(Localization.SnapStringId.SnapListPropertyOutOfDataException)); }
		internal void SetObsolete() { obsolete = true; }
		#region SnapListGroupInfo Members
		public SnapDocument Header {
			get {
				EnsureValid();
				if(!headerReady)
					PrepareHeader();
				return header; 
			}
		}
		public SnapDocument Footer {
			get {
				EnsureValid();
				if(!footerReady)
					PrepareFooter();
				return footer; 
			}
		}
		public SnapDocument Separator {
			get {
				EnsureValid();
				if(!separatorReady)
					PrepareSeparator();
				return separator; 
			}
		}
		public SnapDocument CreateHeader() {
			EnsureValid();
			RemoveHeader();
			string key;
			DocumentModel model = list.CreateSwitch(out key);
			InternalSnapDocumentServer server = new InternalSnapDocumentServer((SnapDocumentModel)model);
			servers.Add(key, server);
			group.TemplateHeaderSwitch = key;
			this.header = (SnapDocument)server.Document;
			return this.header;
		}
		public SnapDocument CreateFooter() {
			EnsureValid();
			RemoveFooter();
			string key;
			DocumentModel model = list.CreateSwitch(out key);
			InternalSnapDocumentServer server = new InternalSnapDocumentServer((SnapDocumentModel)model);
			servers.Add(key, server);
			group.TemplateFooterSwitch = key;
			this.footer = (SnapDocument)server.Document;
			return this.footer;
		}
		public SnapDocument CreateSeparator() {
			EnsureValid();
			RemoveSeparator();
			string key;
			DocumentModel model = list.CreateSwitch(out key);
			InternalSnapDocumentServer server = new InternalSnapDocumentServer((SnapDocumentModel)model);
			servers.Add(key, server);
			group.TemplateSeparatorSwitch = key;
			this.separator = (SnapDocument)server.Document;
			return this.separator;
		}
		public void RemoveHeader() {
			EnsureValid();
			if(!group.HasTemplateHeader)
				return;
			list.EnsureUpdateBegan();
			string key = group.TemplateHeaderSwitch;
			if(servers.ContainsKey(key))
				servers.Remove(key);
			header = null;
			headerReady = true;
			list.Controller.RemoveSwitch(key);
			group.TemplateHeaderSwitch = null;
		}
		public void RemoveFooter() {
			EnsureValid();
			if(!group.HasTemplateFooter)
				return;
			list.EnsureUpdateBegan();
			string key = group.TemplateFooterSwitch;
			if(servers.ContainsKey(key))
				servers.Remove(key);
			footer = null;
			footerReady = true;
			list.Controller.RemoveSwitch(key);
			group.TemplateFooterSwitch = null;
		}
		public void RemoveSeparator() {
			EnsureValid();
			if(!group.HasTemplateSeparator)
				return;
			list.EnsureUpdateBegan();
			string key = group.TemplateSeparatorSwitch;
			if(servers.ContainsKey(key))
				servers.Remove(key);
			separator = null;
			separatorReady = true;
			list.Controller.RemoveSwitch(key);
			group.TemplateSeparatorSwitch = null;
		}
		#endregion
		#region IList<SnapListGroupParam> Members
		public int IndexOf(SnapListGroupParam item) {
			EnsureValid();
			return group.GroupFieldInfos.IndexOf(SnapListGroupParamToGroupFieldInfo(item));
		}
		public void Insert(int index, SnapListGroupParam item) {
			EnsureValid();
			list.EnsureUpdateBegan();
			group.GroupFieldInfos.Insert(index, SnapListGroupParamToGroupFieldInfo(item));
		}
		public void RemoveAt(int index) {
			EnsureValid();
			list.EnsureUpdateBegan();
			group.GroupFieldInfos.RemoveAt(index);
		}
		public SnapListGroupParam this[int index] {
			get {
				EnsureValid();
				return GroupFieldInfoToSnapListGroupParam(group.GroupFieldInfos[index]);
			}
			set {
				EnsureValid();
				list.EnsureUpdateBegan();
				group.GroupFieldInfos[index] = SnapListGroupParamToGroupFieldInfo(value);
			}
		}
		#endregion
		#region ICollection<SnapListGroupParam> Members
		public void Add(SnapListGroupParam item) {
			EnsureValid();
			list.EnsureUpdateBegan();
			group.GroupFieldInfos.Add(SnapListGroupParamToGroupFieldInfo(item));
		}
		public void Clear() {
			EnsureValid();
			list.EnsureUpdateBegan();
			group.GroupFieldInfos.Clear();
		}
		public bool Contains(SnapListGroupParam item) {
			EnsureValid();
			return group.GroupFieldInfos.Contains(SnapListGroupParamToGroupFieldInfo(item));
		}
		public void CopyTo(SnapListGroupParam[] array, int arrayIndex) {
			EnsureValid();
			if(Object.ReferenceEquals(array, null)) throw new ArgumentNullException();
			if(arrayIndex < 0) throw new ArgumentOutOfRangeException();
			if(array.Length < group.GroupFieldInfos.Count + arrayIndex) throw new ArgumentException();
			foreach(GroupFieldInfo item in group.GroupFieldInfos)
				array[arrayIndex++] = GroupFieldInfoToSnapListGroupParam(item);
		}
		public int Count {
			get { EnsureValid(); return group.GroupFieldInfos.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(SnapListGroupParam item) {
			EnsureValid();
			list.EnsureUpdateBegan();
			return group.GroupFieldInfos.Remove(SnapListGroupParamToGroupFieldInfo(item));
		}
		#endregion
		#region IEnumerable<SnapListGroupParam> Members
		public IEnumerator<SnapListGroupParam> GetEnumerator() {
			EnsureValid();
			foreach(GroupFieldInfo item in group.GroupFieldInfos)
				yield return GroupFieldInfoToSnapListGroupParam(item);
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			EnsureValid();
			foreach(var item in group.GroupFieldInfos)
				yield return new SnapListGroupParam(item.FieldName, item.SortOrder, item.GroupInterval);
		}
		#endregion
	}
}
