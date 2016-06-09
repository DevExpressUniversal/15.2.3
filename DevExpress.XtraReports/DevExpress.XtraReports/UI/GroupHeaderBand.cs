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
using System.Text;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Localization;
using DevExpress.Utils.Serializing;
using System.Collections;
using DevExpress.Utils.Design;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.XtraReports.Native.DrillDown;
using DevExpress.XtraReports.Design;
using DevExpress.Utils;
namespace DevExpress.XtraReports.UI {
	[
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.GroupHeaderBand", "GroupHeader"),
	BandKind(BandKind.GroupHeader)
	]
	public class GroupHeaderBand : GroupBand, IDrillDownNode {
		#region inner classes
		new public static class EventNames {
			public const string SortingSummaryGetResult = "SortingSummaryGetResult";
			public const string SortingSummaryReset = "SortingSummaryReset";
			public const string SortingSummaryRowChanged = "SortingSummaryRowChanged";
		}
		#endregion
		GroupFieldCollection groupFields;
		GroupUnion groupUnion = GroupUnion.None;
		XRGroupSortingSummary groupSummary;
		DrillDownHelper drillDownHelper;
		public GroupHeaderBand()
			: base() {
			weightingFactor = GroupHeaderWeight;
			groupFields = new GroupFieldCollection(this);
			groupSummary = new XRGroupSortingSummary();
			groupSummary.Band = this;
			DrillDownControl = null;
		}
		#region Properties
		internal override bool DrillDownExpandedInternal {
			get {
				return DrillDownExpanded;
			}
			set {
				DrillDownExpanded = value;
			}
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.Band.DrillDownExpanded"),
		XtraSerializableProperty,
		]
		public bool DrillDownExpanded {
			get;
			set;
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(null),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.Band.DrillDownControl"),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlReferenceConverter)),
		Editor("DevExpress.XtraReports.Design.DrillDownBandEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public XRControl DrillDownControl {
			get;
			set;
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("GroupHeaderBandGroupUnion"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupHeaderBand.GroupUnion"),
		DefaultValue(GroupUnion.None),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public GroupUnion GroupUnion {
			get { return groupUnion; }
			set { groupUnion = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("GroupHeaderBandGroupFields"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupHeaderBand.GroupFields"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraReports.Design.GroupFieldCollectionEditor, " + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
		]
		public GroupFieldCollection GroupFields {
			get { return groupFields; }
		}
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupHeaderBand.SortingSummary"),
		Browsable(true),
		SRCategory(ReportStringId.CatBehavior),
		Editor("DevExpress.XtraReports.Design.XRGroupSortingSummaryUIEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public XRGroupSortingSummary SortingSummary {
			get { return groupSummary; }
			set {
				groupSummary = value;
				if (groupSummary != null)
					groupSummary.Band = this;
			}
		}
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupHeaderBand.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new GroupHeaderBandScripts Scripts { get { return (GroupHeaderBandScripts)fEventScripts; } }
		protected internal override string SortFieldsPropertyName {
			get { return "GroupFields"; }
		}
		protected internal override GroupFieldCollection SortFieldsInternal {
			get {
				return groupFields;
			}
		}		
		#endregion
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeInteger("GroupFieldCount", groupFields.Count);
			for (int i = 0; i < groupFields.Count; i++)
				serializer.Serialize("GroupField" + i, groupFields[i]);
			serializer.SerializeEnum("GroupUnion", groupUnion);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			groupFields.Clear();
			int count = serializer.DeserializeInteger("GroupFieldCount", 0);
			for (int i = 0; i < count; i++) {
				GroupField groupField = new GroupField();
				serializer.Deserialize("GroupField" + i, groupField);
				groupFields.Add(groupField);
			}
			groupUnion = (GroupUnion)serializer.DeserializeEnum("GroupUnion", typeof(GroupUnion), GroupUnion.None);
		}
		#endregion
		#region Events
		private static readonly object SortingSummaryGetResultEvent = new object();
		private static readonly object SortingSummaryResetEvent = new object();
		private static readonly object SortingSummaryRowChangedEvent = new object();
		public event GroupSortingSummaryGetResultEventHandler SortingSummaryGetResult {
			add { Events.AddHandler(SortingSummaryGetResultEvent, value); }
			remove { Events.RemoveHandler(SortingSummaryGetResultEvent, value); }
		}
		public event EventHandler SortingSummaryReset {
			add { Events.AddHandler(SortingSummaryResetEvent, value); }
			remove { Events.RemoveHandler(SortingSummaryResetEvent, value); }
		}
		public event GroupSortingSummaryRowChangedEventHandler SortingSummaryRowChanged {
			add { Events.AddHandler(SortingSummaryRowChangedEvent, value); }
			remove { Events.RemoveHandler(SortingSummaryRowChangedEvent, value); }
		}
		protected internal virtual void OnSortingSummaryGetResult(GroupSortingSummaryGetResultEventArgs e) {
			RunEventScript(SortingSummaryGetResultEvent, EventNames.SortingSummaryGetResult, e);
			GroupSortingSummaryGetResultEventHandler handler = (GroupSortingSummaryGetResultEventHandler)Events[SortingSummaryGetResultEvent];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void OnSortingSummaryReset(EventArgs e) {
			RunEventScript(SortingSummaryResetEvent, EventNames.SortingSummaryReset, e);
			EventHandler handler = (EventHandler)Events[SortingSummaryResetEvent];
			if (handler != null) handler(this, e);
		}
		protected internal virtual void OnSortingSummaryRowChanged(GroupSortingSummaryRowChangedEventArgs e) {
			RunEventScript(SortingSummaryRowChangedEvent, EventNames.SortingSummaryRowChanged, e);
			GroupSortingSummaryRowChangedEventHandler handler = (GroupSortingSummaryRowChangedEventHandler)Events[SortingSummaryRowChangedEvent];
			if (handler != null) handler(this, e);
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if(disposing)
				NullDrillDownHelper();
			base.Dispose(disposing);
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			NullDrillDownHelper();
			drillDownHelper = DrillDownHelper.CreateInstance(this, RootReport);
		}
		void NullDrillDownHelper() {
			if(drillDownHelper != null) {
				drillDownHelper.Dispose();
				drillDownHelper = null;
			}
		}
		internal bool ContainsGrouping() {
			foreach (GroupField field in groupFields) {
				if (field.FieldName.Length > 0)
					return true;
			}
			return false;
		}
		protected internal override int GetWeightingFactor() {
			return base.GetWeightingFactor() - Level;
		}
		protected override XRControlScripts CreateScripts() {
			return new GroupHeaderBandScripts(this);
		}
		bool ShouldSerializeSortingSummary() {
			return groupSummary != null && groupSummary.ShouldSerialize();
		}
		bool ShouldSerializeScripts() {
			return !fEventScripts.IsDefault();
		}
		bool ShouldSerializeGroupFields() {
			return groupFields.Count != 0;
		}
		protected override object CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.GroupFields) {
				return new GroupField();
			}
			return base.CreateCollectionItem(propertyName, e);
		}
		protected override void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.GroupFields)
				GroupFields.Add((GroupField)e.Item.Value);
			else
				base.SetIndexCollectionItem(propertyName, e);
		}
	}
	class DrillDownHelper : IDisposable {
		public static DrillDownHelper CreateInstance(Band band, IServiceProvider provider) {
			IDrillDownNode node = band as IDrillDownNode;
			Guard.ArgumentNotNull(node, "owner");
			Guard.ArgumentNotNull(provider, "provider");
			if(node.DrillDownControl != null) { 
				if(!ReferenceEquals(node.DrillDownControl.Band, band.GetPreviousBand()))
					throw new Exception(ReportStringId.Msg_InvalidDrillDownControl.GetString(band.Name));
				IDrillDownService serv = provider.GetService<IDrillDownService>();
				if(serv != null)
					serv.SetDrillDownControlPresent();
				return new DrillDownHelper(node, provider);
			}
			return null;
		}
		IDrillDownNode node;
		DrillDownKey groupUniqueKey = DrillDownKey.Empty;
		IServiceProvider provider;
		Band ddContolBand;
		XRControl DrillDownControl {
			get { return node.DrillDownControl; }
		}
		DrillDownHelper(IDrillDownNode node, IServiceProvider provider) {
			this.node = node;
			this.provider = provider;
			this.ddContolBand = DrillDownControl.Band;
			ddContolBand.BeforePrintInternal += OnBeforeDrillDownPrint;
		}
		void OnBeforeDrillDownPrint(object sender, PrintEventArgs e) {
			SyncDrillDownExpanded((Band)sender);
		}
		void SyncDrillDownExpanded(Band band) {
			IDrillDownService serv = provider.GetService<IDrillDownService>();
			if(serv == null) return;
			if(groupUniqueKey == DrillDownKey.Empty || !band.WriteInfo.IsSecondary)
				groupUniqueKey = serv.CreateKey(band.Name, band.LevelInternal + 1, band.Report.GetRowIndexes());
			if(groupUniqueKey == DrillDownKey.Empty) return;
			if(!DrillDownControl.StateBag.Contains(XRComponentPropertyNames.Tag))
				DrillDownControl.StateBag.StoreValue(XRComponentPropertyNames.Tag, DrillDownControl.Tag, value => DrillDownControl.Tag = value);
			DrillDownControl.Tag = groupUniqueKey;
			if(!DrillDownControl.StateBag.Contains(XRComponentPropertyNames.NavigateUrl))
				DrillDownControl.StateBag.StoreValue(XRComponentPropertyNames.NavigateUrl, DrillDownControl.NavigateUrl, value => DrillDownControl.NavigateUrl = (string)value);
			DrillDownControl.NavigateUrl = SR.BrickEmptyUrl;
			bool expanded;
			if(!serv.TryGetGroupExpanded(groupUniqueKey, out expanded))
				serv.SetGroupExpanded(groupUniqueKey, node.DrillDownExpanded);
			else
				node.DrillDownExpanded = expanded;
		}
		public void Dispose() {
			ddContolBand.BeforePrintInternal -= OnBeforeDrillDownPrint;
		}
	}
}
