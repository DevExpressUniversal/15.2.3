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
using System.Collections;
namespace DevExpress.XtraReports.Native {
	using DevExpress.XtraReports.UI;
	using System.Collections.Generic;
	class DetailReportCollection : List<DetailReportBand> {
		#region inner classes
		class LevelComparer : IComparer<DetailReportBand> {
			IList<DetailReportBand> sourceList;
			public LevelComparer(IList<DetailReportBand> sourceList) {
				this.sourceList = sourceList;
			}
			public int Compare(DetailReportBand x, DetailReportBand y) {
				if(x.Level < 0 || y.Level < 0)
					return Comparer.Default.Compare(sourceList.IndexOf(x), sourceList.IndexOf(y));
				else if(x.Level == y.Level)
					return Comparer.Default.Compare(sourceList.IndexOf(y), sourceList.IndexOf(x));
				else
					return Comparer.Default.Compare(x.Level, y.Level);
			}
		}
		#endregion
		XtraReportBase owner;
		public DetailReportCollection(XtraReportBase owner) {
			this.owner = owner;
		}
		public void AddRange(IList bands) {
			foreach(Band band in bands) {
				if(band is DetailReportBand)
					Add((DetailReportBand)band);
			}
			if(!owner.ReportIsLoading)
				Update();
		}
		public void Update() {
			List<DetailReportBand> savedOrder = new List<DetailReportBand>(this);
			this.Sort(new LevelComparer(savedOrder));
			UpdateLevels();
		}
		public void AddBand(DetailReportBand band) {
			if(owner.ReportIsLoading)
				Add(band);
			else if(band.Level == -1) {
				Add(band);
				band.SetLevelCore(Count - 1);
			} else {
				Insert(band.Level, band);
				UpdateLevels();
			}
		}
		public void RemoveBand(DetailReportBand band) {
			Remove(band);
			UpdateLevels();
		}
		public void AssignLevel(DetailReportBand band, int level) {
			if(Contains(band)) {
				int index = Math.Max(0, Math.Min(level, Count - 1));
				Remove(band);
				Insert(index, band);
				UpdateLevels();
			}
		}
		public DetailReportBand GetByLevel(int level) {
			if(level >= 0 && level < Count)
				return this[level];
			return null;
		}
		void UpdateLevels() {
			for(int i = 0; i < Count; i++)
				this[i].SetLevelCore(i);
		}
	}
}
namespace DevExpress.XtraReports.UI 
{
	using DevExpress.XtraReports.Native;
	using DevExpress.XtraReports.Design;
	using DevExpress.XtraReports.Localization;
	using System.ComponentModel;
	using DevExpress.XtraReports.Serialization;
	using System.Collections.Generic;
	public class BandCollection : XRControlCollection, IEnumerable<Band> {
#if !SL
	[DevExpressXtraReportsLocalizedDescription("BandCollectionItem")]
#endif
		public new Band this[int index] { get { return InnerList[index] as Band; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("BandCollectionItem")]
#endif
		public new Band this[string name] { get { return base[name] as Band; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("BandCollectionItem")]
#endif
		public Band this[BandKind kind] { get { return GetBandBy(kind); }
		}
		internal XtraReportBase Report { get { return (XtraReportBase)owner; } 
		}
		public BandCollection(XtraReportBase owner) : base(owner) {
		}
		public void AddRange(Band[] bands) {
			List<Band> addingBands = new List<Band>();
			foreach(Band band in bands) {
				if(!Contains(band) && CanAdd(band))
					addingBands.Add(band);
			}
			InnerList.AddRange(addingBands);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
			SubscribeEvents(addingBands);
			foreach(Band band in InnerList) {
				band.AssignParent(owner, false);
			}
			Report.DetailReportBands.AddRange(addingBands);
		}
		void SubscribeEvents(ICollection bands) {
			foreach(Band band in bands) {
				if(band is GroupBand) {
					((GroupBand)band).BandLevelChanged -= OnBandLevelChanged;
					((GroupBand)band).BandLevelChanged += OnBandLevelChanged;
				}
			}
		}
		void UnsubscribeEvents(ICollection bands) {
			foreach(Band band in bands) {
				if(band is GroupBand)
					((GroupBand)band).BandLevelChanged -= OnBandLevelChanged;
			}
		}
		void OnBandLevelChanged(object sender, EventArgs e) {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public int Add(Band band) { 
			return base.Add(band);
		}
		public int IndexOf(Band band) { 
			return List.IndexOf(band);
		}
		public bool Contains(Band band) { 
			return IndexOf(band) >= 0; 
		}
		protected override void OnInsert(int index,	object value) {
			Band band = value as Band;
			if(!CanAdd(band))
				throw (new Exception(ReportLocalizer.GetString(ReportStringId.Msg_IncorrectBandType)));
		}
		bool CanAdd(Band band) { 
			bool isValidBand = ((XtraReportBase)owner).CanAddComponent(band);
			return isValidBand || CanAdd(band.GetType());
		}
		protected override void OnInsertCompleteCore(int index,	object value) {
			((Band)value).AssignParent(owner, true);
			if(value is DetailReportBand)
				Report.DetailReportBands.AddBand((DetailReportBand)value);
			SubscribeEvents(new Band[] { (Band)value });
		}
		protected override void OnRemoveCompleteCore(int index,	object value) {
			Band band = (Band)value;
			if(band.Parent != null && !object.ReferenceEquals(band.Parent, this.owner))
				return;
			if(value is DetailReportBand)
				Report.DetailReportBands.RemoveBand((DetailReportBand)value);
			UnsubscribeEvents(new Band[] { (Band)value });
		}
		public void Remove(Band band) {
			List.Remove(band);
		}
		protected internal override bool CanAdd(Type controlType) {
			BandKind bandKind = Band.GetBandKindByType(controlType);
			return CanAdd(bandKind);
		}
		protected internal virtual bool CanAdd(BandKind bandKind) {
			if(bandKind == BandKind.None || bandKind == BandKind.SubBand)
				return false;
			if((bandKind & (BandKind.GroupHeader | BandKind.GroupFooter | BandKind.DetailReport)) > 0)
				return true;
			return GetBandBy(bandKind) == null;
		}
		protected Band GetBandBy(BandKind kind) {
			for(int i = 0; i < Count; i++) {
				if(this[i].BandKind == kind)
					return this[i];
			}
			return null;
		}
		public Band GetBandByType(Type type) {
			if(type == null)
				throw new ArgumentException("type");
			foreach(Band band in List) {
				if(type.IsAssignableFrom(band.GetType()))
					return band;
			}
			return null;
		}
		internal void CopyFrom(Band[] bands) {
			Clear();
			AddRange(bands);
		}
		protected override void OnClearCore() {
			base.OnClearCore();
			Report.DetailReportBands.Clear();
			UnsubscribeEvents(List);
		}
		#region IEnumerable<Band> Members
		IEnumerator<Band> IEnumerable<Band>.GetEnumerator() {
			foreach(Band band in List)
				yield return band;
		}
		#endregion
	}
	public class DetailReportBandCollection : BandCollection
	{
		public DetailReportBandCollection(XtraReportBase owner) : base(owner) {
		}
		protected internal override bool CanAdd(BandKind bandKind) {
			return base.CanAdd(bandKind) && (bandKind & (BandKind.TopMargin | BandKind.BottomMargin | BandKind.PageHeader | BandKind.PageFooter)) == 0;  
		}
	}
}
namespace DevExpress.XtraReports.Native 
{
	using DevExpress.XtraReports.UI;
	using System.Collections.Generic;
	public class XRGroupCollection : List<XRGroup>, IDisposable
	{
		public XRGroupCollection(ICollection c) : base() {
			Initialize(c);
		}
		void Initialize(ICollection c) {
			foreach(object item in c) {
				if(item is GroupBand) {
					GroupBand band = (GroupBand)item;
					int level = Math.Max(0, band.Level);
					if(!LevelExist(band, level))
						OnAddBand(band, level);
				}
			}
			RemoveEmptyGroups();
			ValidateGroupLevels();
		}
		bool LevelExist(GroupBand band, int level) {
			if(band == null) return false;
			foreach(XRGroup group in this) {
				if(band is GroupHeaderBand) {
					if(group.Header != null && group.Header.Level == level)
						return true;
				}
				if(band is GroupFooterBand) {
					if(group.Footer != null && group.Footer.Level == level)
						return true;
				}
			}
			return false;
		}
		bool BandExist(GroupBand band) {
			return FindGroupByBand(band) != null;
		}
		public void OnAddBand(GroupBand band, int groupIndex) {
			if(BandExist(band)) return;
			while(Count < groupIndex + 1)
				Add( new XRGroup() );
			this[groupIndex].SetGroupBand(band);
			band.SetLevelCore(groupIndex);
			band.BandLevelChanged += new EventHandler(band_OnBandLevelChanged);
		}
		public void OnAddBand(GroupBand band) {
			if(BandExist(band)) return;
			XRGroup group = null;
			if(band is GroupHeaderBand) {
				GroupHeaderBand ghBand = (GroupHeaderBand)band;
				group = FindGroupByHeader(null);
				if(group != null) {
					group.Header = ghBand;
				} else {
					group = new XRGroup(ghBand, null);
					Add(group);
				}
			} else if(band is GroupFooterBand) {
				GroupFooterBand gfBand = band as GroupFooterBand;
				group = gfBand.Level == 0 ? FindGroupByFooter(null) : FindGroupByLevel(gfBand.Level);
				if(group != null) {
					group.Footer = gfBand;
				} else {
					group = new XRGroup(null, gfBand);
					Add(group);
				}
			}
			band.SetLevelCore( IndexOf(group) );
			band.BandLevelChanged += new EventHandler(band_OnBandLevelChanged);
		}
		XRGroup FindGroupByLevel(int level) {
			foreach(XRGroup group in this) {
				if(group.Header.Level == level)
					return group;
			}
			return null;
		}
		public void OnRemoveBand(GroupBand band) {
			XRGroup group = null;			
			if(band is GroupHeaderBand) {
				group = FindGroupByHeader((GroupHeaderBand)band);
				if(group != null) {
					Unsubscribe(group.Header);
					group.Header = null; 
				}
			} else if(band is GroupFooterBand) {
				group = FindGroupByFooter((GroupFooterBand)band);
				if(group != null) {
					Unsubscribe(group.Footer);
					group.Footer = null;
				}
			}
			if(group != null && group.IsEmpty) {
				Remove(group);
				ValidateGroupLevels();
			}
		}
		public void RemoveEmptyGroups() {
			for(int i = Count - 1; i >= 0; i--) {
				XRGroup group = this[i];
				if(group.IsEmpty) Remove(group);
			}
		}
		public void ValidateGroupLevels() {
			for(int i = 0; i < Count; i++) {
				this[i].SetLevel(i);
			}
		}
		void band_OnBandLevelChanged(object sender, EventArgs e) {
			GroupBand band = sender as GroupBand;
			XRGroup group = FindGroupByBand(band);
			if(group != null) {
				SetChildIndex(group, band.Level);
				ValidateGroupLevels();
			}
		}
		void SetChildIndex(XRGroup child, int index) {
			Remove(child);
			index = Math.Min( Math.Max(0,index), Count);
			Insert(index, child);
		}
		XRGroup FindGroupByHeader(GroupHeaderBand band) {
			foreach(XRGroup group in this) {
				if( Comparer.Equals(group.Header,band) )
					return group;
			}
			return null;
		}
		XRGroup FindGroupByFooter(GroupFooterBand band) {
			foreach(XRGroup group in this) {
				if( Comparer.Equals(group.Footer,band) )
					return group;
			}
			return null;
		}
		public XRGroup FindGroupByBand(GroupBand band) {
			foreach(XRGroup group in this) {
				if(Comparer.Equals(group.Header,band) || Comparer.Equals(group.Footer,band))
					return group;
			}
			return null;
		}
		internal bool CanBandBeMoved(GroupBand band, BandReorderDirection direction) {
			GroupBand result = FindBand(band, direction);
			if(result != null) {
				System.ComponentModel.InheritanceAttribute att = (System.ComponentModel.InheritanceAttribute)System.ComponentModel.TypeDescriptor.GetAttributes(result)[typeof(System.ComponentModel.InheritanceAttribute)];
				return att.InheritanceLevel != System.ComponentModel.InheritanceLevel.InheritedReadOnly;
			}
			return false;
		}
		internal void MoveBand(GroupBand band, BandReorderDirection direction) {
			GroupBand replacingBand = FindBand(band, direction);
			if(replacingBand != null)
				band.Level = replacingBand.Level;
		}
		GroupBand FindBand(GroupBand band, BandReorderDirection direction) {
			int currentGroupIndex = IndexOf(FindGroupByBand(band));
			if(currentGroupIndex >= 0) {
				if(band is GroupHeaderBand)
					return FindHeaderBand(currentGroupIndex, direction);
				else
					return FindFooterBand(currentGroupIndex, direction);
			}
			return null;
		}
		GroupBand FindHeaderBand(int currentGroupIndex, BandReorderDirection direction) {
			if(direction == BandReorderDirection.Up) {
				for(int index = currentGroupIndex + 1; index < Count; index++)
					if(this[index].Header != null) 
						return this[index].Header;
			} else if(direction == BandReorderDirection.Down) {
				for(int index = currentGroupIndex - 1; index >= 0; index--)
					if(this[index].Header != null) 
						return this[index].Header;
			}
			return null;
		}
		GroupBand FindFooterBand(int currentGroupIndex, BandReorderDirection direction) {
			if(direction == BandReorderDirection.Up) {
				for(int index = currentGroupIndex - 1; index >= 0; index--)
					if(this[index].Footer != null) 
						return this[index].Footer;
			} else if(direction == BandReorderDirection.Down) {
				for(int index = currentGroupIndex + 1; index < Count; index++)
					if(this[index].Footer != null) 
						return this[index].Footer;
			}
			return null;
		}
		public void Dispose() {
			foreach(XRGroup group in this) {
				Unsubscribe(group.Header);
				Unsubscribe(group.Footer);
			}
		}
		void Unsubscribe(GroupBand band) {
			if(band != null)
				band.BandLevelChanged -= new EventHandler(band_OnBandLevelChanged);
		}
	}
	public class XRGroup 
	{
		private GroupHeaderBand header;
		private GroupFooterBand footer;		
		public GroupHeaderBand Header { get { return header; } set { header = value; }
		}
		public GroupFooterBand Footer { get { return footer; } set { footer = value; }
		}
		public bool IsEmpty { get { return header == null && footer == null; }
		}
		internal GroupFieldCollection GroupFields {
			get { return header != null ? header.GroupFields : null; }
		}
		public XRGroup() {
		}
		public XRGroup(GroupHeaderBand header, GroupFooterBand footer) {			
			this.header = header;
			this.footer = footer;
		}
		public void SetGroupBand(GroupBand band) {
			if(band is GroupHeaderBand) header = (GroupHeaderBand)band;
			else footer = band as GroupFooterBand;
		}
		public void SetLevel(int level) {
			if(header != null) header.SetLevelCore(level);
			if(footer != null) footer.SetLevelCore(level);
		}
	}
}
