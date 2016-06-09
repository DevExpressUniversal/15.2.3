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
using DevExpress.Data.IO;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.PivotGrid.OLAP {
	public abstract class OLAPMetadata : QueryMetadata<OLAPCubeColumn>, IOLAPMetadata {
		readonly List<PivotOLAPKPIMeasures> kpis = new List<PivotOLAPKPIMeasures>();
		readonly OLAPHierarchies hierarchies = new OLAPHierarchies();
		IOLAPConnection olapConnection;
		bool? isAS2000, isAS2005;
		bool isDesignMode;
		string cubeCaption;
		protected new IOLAPQueryExecutor QueryExecutor { get { return (IOLAPQueryExecutor)base.QueryExecutor; } }
		public new OLAPMetadataColumns Columns { get { return (OLAPMetadataColumns)base.Columns; } }
		public List<PivotOLAPKPIMeasures> KPIs { get { return kpis; } }
		public OLAPHierarchies Hierarchies { get { return hierarchies; } }
		protected IOLAPConnection Connection { get { return olapConnection; } }
		public override bool Connected { get { return olapConnection != null; } }
		protected internal abstract string ServerVersion { get; }
		bool IOLAPMetadata.IsLocked {
			get {
				foreach(IOLAPHelpersOwner item in CurrentOwners)
					if(item.IsLocked)
						return true;
				return false;
			}
		}
		string IOLAPMetadata.ServerVersion { get { return ServerVersion; } }
		protected internal bool IsAS2000 {
			get {
				if(!isAS2000.HasValue)
					isAS2000 = OLAPMetadataHelper.IsAS2000(ServerVersion);
				return isAS2000.Value;
			}
		}
		protected internal bool IsAS2005 {
			get {
				if(!isAS2005.HasValue)
					isAS2005 = OLAPMetadataHelper.IsAS2005(ServerVersion);
				return isAS2005.Value;
			}
		}
		public bool IsDesignMode {
			get {
				foreach(IOLAPHelpersOwner item in CurrentOwners)
					if(item.IsDesignMode)
						return true;
				return isDesignMode;
			}
			set {
				isDesignMode = value;
			}
		}
		protected OLAPMetadata()
			: base() {
			this.connectionSettings = CreateConnectionSettings();
		}
		string sessionID;
		public string SessionID {
			get { return sessionID; }
			set { sessionID = value; }
		}
		string fullConnectionString;
		protected internal string FullConnectionString {
			get { return fullConnectionString; }
			set {
				if(FullConnectionString == value)
					return;
				fullConnectionString = value;
				SettingsFullConnectionString = value;
				OnConnectionStringChanged();
			}
		}
		#region OLAP Connection Settings
		readonly IOLAPConnectionSettings connectionSettings;
		IOLAPConnectionSettings CreateConnectionSettings() {
			return new OLAPConnectionStringBuilder();
		}
		string SettingsFullConnectionString { set { connectionSettings.FullConnectionString = value; } }
		bool IsConnectionSettingsValid { get { return connectionSettings.IsValid; } }
		public int QueryTimeout { get { return connectionSettings.QueryTimeout; } }
		public string CubeName {
			get { return connectionSettings.CubeName; }
			set {
				ChangeCubeName(value, true);
			}
		}
		public string CubeCaption {
			get { return cubeCaption; }
			set { cubeCaption = value; }
		}
		protected void ChangeCubeName(string name, bool reconnect) {
			if(CubeName != name) {
				connectionSettings.CubeName = name;
				if(reconnect)
					Connect(false);
			}
		}
		public string CatalogName { 
			get { return connectionSettings.CatalogName; }  
			set { connectionSettings.CatalogName = value; }
		}
		#endregion
		protected internal abstract IOLAPConnection CreateConnection(IOLAPConnectionSettings connectionSettings, IOLAPMetadata data);
		protected override QueryMetadataColumns CreateMetadataColumns() {
			return new OLAPMetadataColumns(this);
		}
		public void FetchMemberProperties(OLAPMember member) {
			if(!Connected) {
				member.InitProperties(new List<OLAPMemberProperty>());
				return;
			}
			Dictionary<string, object> restrictions = new Dictionary<string, object>();
			restrictions.Add("CATALOG_NAME", CatalogName);
			restrictions.Add("CUBE_NAME", CubeName);
			restrictions.Add(OLAPDataSourceQueryBase.MemberUniqueName, member.UniqueName);
			restrictions.Add("HIERARCHY_UNIQUE_NAME", member.Column.Hierarchy.UniqueName);
			member.InitProperties(GetShemaRowSet(OlapSchema.Members, restrictions));
		}
		public virtual IOLAPCommand CreateCommand(string mdx) {
			return Connection.CreateCommand(mdx);
		}
		public virtual void QueryMembers(OLAPCubeColumn column, string[] uniqueNames) {
			if(QueryExecutor == null)
				return;
			List<string> names = new List<string>();
			if(uniqueNames != null) {
				Func<OLAPMember, bool> sp = GetCheckSortProperty(column);
				for(int i = 0; i < uniqueNames.Length; i++) {
					OLAPMember member = column[uniqueNames[i]];
					if(member != null && sp(member))
						continue;
					names.Add(uniqueNames[i]);
				}
				if(names.Count == 0)
					return;
			}
			QueryExecutor.QueryMembers(column, names.ToArray());
		}
		public virtual void QueryMembers(OLAPCubeColumn column, OLAPMember[] members) {
			if(QueryExecutor == null)
				return;
			List<string> names = new List<string>();
			if(members != null) {
				Func<OLAPMember, bool> sp = GetCheckSortProperty(column);
				for(int i = 0; i < members.Length; i++) {
					OLAPMember member = members[i];
					if(sp(member))
						continue;
					names.Add(members[i].UniqueName);
				}
				if(names.Count == 0)
					return;
			}
			QueryExecutor.QueryMembers(column, names.ToArray());
		}
		public Func<OLAPMember, bool> GetCheckSortProperty(OLAPCubeColumn column) {
			List<string> keys = new List<string>(column.AutoProperties.Select(s => s.Name));
			if(keys.Count != 0)
				return (member) => member.AutoPopulatedProperties != null && keys.TrueForAll(propertyKey => member.AutoPopulatedProperties.ContainsKey(propertyKey));
			return (m) => true;
		}
		bool IOLAPMetadata.IsGT2005 { get { return !IsAS2000 && !IsAS2005; } }
		bool IOLAPMetadata.CanCreateMember() {
			return ContainsMemberPropertiesInResponse();
		}
		protected virtual bool ContainsMemberPropertiesInResponse() {
			 return !OLAPMetadataHelper.IsAS2000(ServerVersion);
		}
		public virtual void EnsureMembersLoadedOnColumnExpand(OLAPCubeColumn column) {
			if(QueryExecutor == null || ContainsMemberPropertiesInResponse() || column.Metadata.Cardinality >= 1000) 
				return;
			column.EnsureColumnMembersLoaded(); 
		}
		public void QueryChildMembers(OLAPCubeColumn childColumn, QueryMember member) {
			if(QueryExecutor == null)
				return;
			QueryExecutor.QueryChildMembers(childColumn, (OLAPMember)member);
		}
		protected virtual void OpenConnection(IOLAPConnection connection) {
			connection.Open();
		}
		protected internal override void OnInitialized(IDataSourceHelpersOwner<OLAPCubeColumn> owner) {
			base.OnInitialized(owner);
			IOLAPHelpersOwner helpersOwner = (IOLAPHelpersOwner)owner;
			if(!string.IsNullOrEmpty(FullConnectionString))
				if(Columns.Count == 0 || !Connected)
					Connect(false);
				else
					if(CanConnect(false))
						ClearDataSourceState(false, helpersOwner);
					else
						ClearDataSourceState(true, helpersOwner);
		}
		void OnConnectionStringChanged() {
			if(CanConnect(false)) {
				ClearState(false);
				Connect(false);
			} else
				ClearState(true);
		}
		protected bool CanConnect(bool forced) {
			bool hasOwner = false;
			foreach(OLAPDataSourceBase dataSource in CurrentOwners)
				hasOwner = hasOwner || dataSource.Data != null;
			return hasOwner && (!IsDesignMode || forced);
		}
		protected virtual void PopulateColumnsCore() {
			new OLAPMetadataPopulator(this).PopulateColumns(Hierarchies, Columns, KPIs);
		}
		protected internal bool PopulateColumns(bool tryConnect, IOLAPHelpersOwner owner) {
			if(Columns.Count > 0) {
				ClearDataSourceState(false, owner);
				return true;
			}
			bool wasConnected = Connected;
			if(tryConnect && !wasConnected)
				Connect(true);
			if(!Connected)
				return false;
			ClearState(false);
			try {
				PopulateColumnsCore();
			} catch(Exception exception) {
				ClearState(true);
				RaiseConnectionException(exception);
				return false;
			}
			if(tryConnect && !wasConnected)
				Disconnect();
			return true;
		}
		void RaiseConnectionException(Exception innerException) {
			Disconnect();
			OLAPConnectionException e = new OLAPConnectionException(OLAPConnectionException.DefaultMessage, innerException);
			bool handled = CurrentOwners[0].HandleException(e);
			if(!handled)
				throw e;
		}
		protected internal virtual void Connect(bool forced) {
			if(!CanConnect(forced))
				return;
			try {
				ConnectCore();
			} catch(OLAPConnectionException exception) {
				if(!IsDesignMode)
					throw exception;
			}
		}
		protected virtual void ClearState(bool layoutChanged) {
			KPIs.Clear();
			Columns.Clear();
			Hierarchies.Clear();
			ClearDataSourceState(layoutChanged, null);
		}
		void ClearDataSourceState(bool layoutChanged, IOLAPHelpersOwner owner) {
			if(owner != null)
				owner.ClearState(layoutChanged);
			else
				foreach(IOLAPHelpersOwner item in CurrentOwners)
					item.ClearState(layoutChanged);
		}
		protected virtual void ConnectCore() {
			if(Connected)
				Disconnect();
			if(!IsConnectionSettingsValid)
				return;
			try {
				olapConnection = CreateConnection(connectionSettings, this);
				OpenConnection(olapConnection);
				if(string.IsNullOrEmpty(olapConnection.Database)) {
					RaiseConnectionException(null);
					return;
				}
				base.QueryExecutor = CreateQueryExecutor();
				OnConnectionEstablished();
			} catch(Exception exception) {
				RaiseConnectionException(exception);
				return;
			}
			foreach(IOLAPHelpersOwner item in CurrentOwners)
				item.OnSuccessConnected();
		}
		protected virtual void OnConnectionEstablished() {
			LocalizerChangeHelper.Add(this);
		}
		static class LocalizerChangeHelper {
			static List<WeakReference> refs;
			static bool subscribed;
			public static void Add(OLAPMetadata metadata) {
				if(!subscribed)
					PivotGridLocalizer.ActiveChanged += LocalizerChanged;
				subscribed = true;
				Remove(metadata);
				EnsureRefs();
				if(refs == null)
					refs = new List<WeakReference>();
				refs.Add(new WeakReference(metadata));
			}
			public static void Remove(OLAPMetadata metadata) {
				if(refs == null)
					return;
				for(int i = refs.Count - 1; i >= 0; i--) {
					object value = refs[i] == null ? null : refs[i].Target;
					if(object.ReferenceEquals(null, value) || object.Equals(value, metadata))
						refs.RemoveAt(i);
				}
				if(metadata != null)
					EnsureRefs();
			}
			static void LocalizerChanged(object sender, EventArgs e) {
				EnsureRefs();
				if(refs == null)
					return;
				foreach(WeakReference reference in refs) {
					OLAPMetadata metadata = reference.Target as OLAPMetadata;
					if(metadata != null)
						metadata.PivotGridLocalizer_ActiveChanged(sender, e);
				}
			}
			static void EnsureRefs() {
				if(refs == null)
					return;
				Remove(null);
				if(refs.Count == 0) {
					refs = null;
					PivotGridLocalizer.ActiveChanged -= LocalizerChanged;
					subscribed = false;
				}
			}
		}
		void PivotGridLocalizer_ActiveChanged(object sender, EventArgs e) {
			OLAPHierarchy hierarchy = null;
			if(hierarchies.TryGetValue(OLAPHierarchy.MeasuresHierarchyUniqueName, out hierarchy))
				hierarchy.Caption = PivotGridLocalizer.GetString(PivotGridStringId.OLAPMeasuresCaption);
			if(hierarchies.TryGetValue(OLAPHierarchy.KPIsHierarchyUniqueName, out hierarchy))
				hierarchy.Caption = PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPIsCaption);
		}
		protected internal override bool Disconnect() {
			if(!Connected)
				return false;
			base.QueryExecutor = null;
			olapConnection.Dispose();
			olapConnection = null;
			sessionID = null;
			isAS2000 = null;
			isAS2005 = null;
			LocalizerChangeHelper.Remove(this);
			return true;
		}
		internal string GetHierarchyName(string columnName) {
			if(!string.IsNullOrEmpty(columnName)) {
				MetadataColumnBase columnValue;
				if(Columns.TryGetValue(columnName, out columnValue))
					return columnValue.IsMeasure ? null : ((OLAPMetadataColumn)columnValue).Hierarchy.UniqueName;
			}
			return null;
		}
		internal override string GetColumnCaption(string columnName) {
			string caption = base.GetColumnCaption(columnName);
			if(string.IsNullOrEmpty(caption) && !string.IsNullOrEmpty(columnName)) {
				OLAPHierarchy hierachyValue;
				if(hierarchies.TryGetValue(columnName, out hierachyValue))
					return hierachyValue.Caption;
			}
			return caption;
		}
		internal override string GetColumnDisplayFolder(string columnName) {
			string displayFolder = base.GetColumnDisplayFolder(columnName);
			if(string.IsNullOrEmpty(displayFolder) && !string.IsNullOrEmpty(columnName)) {
				OLAPHierarchy hierachyValue;
				if(hierarchies.TryGetValue(columnName, out hierachyValue))
					return hierachyValue.DisplayFolder;
			}
			return displayFolder;
		}
		void EnsureColumnsPopulatedNotDT() {
			if((Connected || !IsDesignMode) && Columns.Count == 0)
				PopulateColumnsCore(null);
		}
		public PivotOLAPKPIMeasures GetKPIMeasures(string kpiName) {
			EnsureColumnsPopulatedNotDT();
			foreach(PivotOLAPKPIMeasures kpi in KPIs)
				if(kpi.KPIName == kpiName)
					return kpi;
			return null;
		}
		public PivotKPIGraphic GetKPIServerDefinedGraphic(string kpiName, PivotKPIType kpiType) {
			if(kpiType != PivotKPIType.Status && kpiType != PivotKPIType.Trend)
				return PivotKPIGraphic.None;
			PivotOLAPKPIMeasures measures = GetKPIMeasures(kpiName);
			if(measures == null)
				return PivotKPIGraphic.None;
			OLAPKPIMetadataColumn column = Columns[kpiType == PivotKPIType.Trend ? measures.TrendMeasure : measures.StatusMeasure] as OLAPKPIMetadataColumn;
			return column == null ? PivotKPIGraphic.None : column.Graphic;
		}
		public string GetMeasureServerDefinedFormatString(string fieldName) {
			OLAPMetadataColumn column = Columns[fieldName];
			return column == null ? null : column.FormatString;
		}
		public List<string> GetKPIList() {
			List<string> res = new List<string>();
			foreach(PivotOLAPKPIMeasures kpi in KPIs) {
				res.Add(kpi.KPIName);
			}
			return res;
		}
		public string GetKPIName(string fieldName, PivotKPIType kpiType) {
			if(kpiType == PivotKPIType.None)
				return null;
			EnsureColumnsPopulatedNotDT();
			OLAPMetadataColumn column = MetadataColumnBase.GetOriginalColumn(Columns, Columns[fieldName]);
			if(column == null)
				return null;
			string name = column.UniqueName;
			Predicate<PivotOLAPKPIMeasures> selector;
			switch(kpiType) {
				case PivotKPIType.Value:
					selector = kpi => kpi.ValueMeasure == name;
					break;
				case PivotKPIType.Goal:
					selector = kpi => kpi.GoalMeasure == name;
					break;
				case PivotKPIType.Status:
					selector = kpi => kpi.StatusMeasure == name;
					break;
				case PivotKPIType.Trend:
					selector = kpi => kpi.TrendMeasure == name;
					break;
				case PivotKPIType.Weight:
					selector = kpi => kpi.WeightMeasure == name;
					break;
				default:
					selector = kpi => false;
					break;
			}
			PivotOLAPKPIMeasures kpiMeasures = KPIs.Find(selector);
			return kpiMeasures != null ? kpiMeasures.KPIName : null;
		}
		protected override void SaveMemberCore(QueryMember member, TypedBinaryWriter writer) {
			OLAPMember olapMember = (OLAPMember)member;
#if DEBUGTEST
			writer.Write((int)4567809);
#endif
			writer.Write(olapMember.UniqueName);
			writer.WriteNullableString(olapMember.Caption);
			bool isCalculated = olapMember.Column.GetIsCalculatedMember(olapMember);
			writer.Write(isCalculated);
			SaveMemberValue(olapMember, writer, isCalculated);
#if DEBUGTEST
			writer.Write((int)23534534);
#endif
		}
		protected override QueryMember LoadMemberCore(IQueryMetadataColumn column, TypedBinaryReader reader) {
#if DEBUGTEST
			int sign = reader.ReadInt32();
			if(sign != 4567809)
				throw new Exception("corr");
#endif
			string memberUniqueName = reader.ReadString();
			string memberCaption = reader.ReadNullableString();
			bool isCalculated = reader.ReadBoolean();
			object value = LoadMemberValue(column, reader, isCalculated);
#if DEBUGTEST
			sign = reader.ReadInt32();
			if(sign != 23534534)
				throw new Exception("corr");
#endif            
			OLAPChildMember member = new OLAPChildMember((OLAPMetadataColumn)column, memberUniqueName, value, memberCaption);		   
			if(isCalculated)
				((OLAPMetadataColumn)column).AddCalculatedMember(member);
			return member;
		}
		protected override void SaveColumnsCore(TypedBinaryWriter writer) {
			Hierarchies.SaveToStream(writer);
			base.SaveColumnsCore(writer);
			writer.Write(KPIs.Count);
			for(int i = 0; i < KPIs.Count; i++) {
				KPIs[i].SaveToStream(writer);
			}
		}
		protected override void RestoreColumnsCore(TypedBinaryReader reader) {
			Hierarchies.RestoreFromStream(reader);
			base.RestoreColumnsCore(reader);
			int kpisCount = reader.ReadInt32();
			for(int i = 0; i < kpisCount; i++) {
				KPIs.Add(new PivotOLAPKPIMeasures(reader));
			}
		}
		public abstract bool DimensionPropertiesSupported { get; }
		OLAPMember[] IOLAPMetadata.QuerySortMembers(IOLAPHelpersOwner owner, OLAPCubeColumn column, IEnumerable<QueryMember> members) {
			if(!Connected)
				return null;
			return QueryExecutor.QuerySortMembers(owner, column, members);
		}
		internal PivotOLAPKPIValue QueryKPIValue(string kpiName) {
			return QueryExecutor.QueryKPIValue(kpiName);
		}
		internal PivotKPIType GetKPIType(string fieldName) {
			EnsureColumnsPopulatedNotDT();
			OLAPKPIMetadataColumn column = Columns[fieldName] as OLAPKPIMetadataColumn;
			if(column != null)
				return column.Type;
			return PivotKPIType.None;
		}
		internal PivotKPIGraphic GetKPIGraphic(string fieldName) {
			EnsureColumnsPopulatedNotDT();
			OLAPKPIMetadataColumn column = Columns[fieldName] as OLAPKPIMetadataColumn;
			if(column != null)
				return column.Graphic;
			return PivotKPIGraphic.None;
		}
		internal DevExpress.Utils.DefaultBoolean GetColumnIsAggregatable(string dimensionName) {
			if(string.IsNullOrEmpty(dimensionName) || !Columns.ContainsKey(dimensionName))
				return DefaultBoolean.Default;
			return Columns[dimensionName].IsAggregatable ? DefaultBoolean.True : DefaultBoolean.False;
		}
		internal OLAPMember GetMemberByValue(OLAPCubeColumn column, object value) {
			if(column == null || column.IsMeasure)
				return null;
			IList<OLAPMember> members = ((IOLAPEditableMemberCollection)column.Metadata).GetMembersByValue(false, -1, value);
			if(members != null && members.Count > 0)
				return members[0];
			column.EnsureColumnMembersLoaded();
			members = ((IOLAPEditableMemberCollection)column.Metadata).GetMembersByValue(false, -1, value);
			if(members != null && members.Count > 0)
				return members[0];
			return null;
		}
		internal IOLAPMember[] GetUniqueMembers(OLAPCubeColumn column) {
			if(column == null || column.IsMeasure)
				return null;
			column.Metadata.EnsureColumnMembersLoaded(column);
			return column.Metadata.GetMembers(true).Select((d) => ((IOLAPMember)d)).ToArray();
		}
		internal string GetDrillDownColumnName(string fieldName) {
			OLAPMetadataColumn column = Columns[fieldName];
			return column == null ? null : column.DrillDownColumn;
		}
		internal int GetFieldHierarchyLebel(string fieldName) {
			EnsureColumnsPopulatedNotDT();
			OLAPMetadataColumn column = Columns[fieldName];
			if(column != null)
				return column.Level;
			return PivotGridFieldBase.DefaultLevel;
		}
		internal bool GetOlapIsUserHierarchy(string hierarchyName) {
			if(hierarchyName != null) {
				OLAPHierarchy hierarchy = null;
				if(Hierarchies.TryGetValue(hierarchyName, out hierarchy))
					return hierarchy.IsUserDefined;
			}
			return false;
		}
		public string GetColumnCaptionByDrilldownName(string name) {
			foreach(KeyValuePair<string, MetadataColumnBase> pair in Columns) {
				OLAPMetadataColumn column = (OLAPMetadataColumn)pair.Value;
				if(column.DrillDownColumn == name && !string.IsNullOrEmpty(column.Caption))
					return column.Caption;
			}
			return name;
		}
		protected override bool HasNullValuesCore(IDataSourceHelpersOwner<OLAPCubeColumn> owner, IQueryMetadataColumn column) {
			if(((IOLAPEditableMemberCollection)column).GetMembersByValue(false, -1, null).Count > 0)
				return true;
			return base.HasNullValuesCore(owner, column);
		}
		public abstract IOLAPRowSet GetShemaRowSet(string name, Dictionary<string, object> restrictions);
	}
}
