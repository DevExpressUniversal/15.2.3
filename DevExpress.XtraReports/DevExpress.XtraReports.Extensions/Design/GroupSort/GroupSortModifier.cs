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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design.GroupSort {
	public abstract class ModifierStrategyBase {
		IServiceProvider serviceProvider;
		IDesignerHost designerHost;
		protected ModifierStrategyBase(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			this.designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
		}
		protected IServiceProvider ServiceProvider { get { return serviceProvider; } }
		protected IDesignerHost DesignerHost { get { return designerHost; } }
		public abstract void DeleteComponents(params Component[] components);
		public abstract void InsertGroupFields(GroupField[] sourceArray, GroupFieldCollection dest, int groupFieldCount);
		public abstract void AddGroupHeader(ReportAddGroupHeaderModifier modifier, XtraReportBase report, GroupFieldCollection sourceFields, int level);
		public abstract void Modify(BandModifier modifier, params Band[] bands);
		public abstract void AddGroupFooter(XtraReportBase report, int level);
	}
	public class RealModifierStrategy : ModifierStrategyBase {
		public RealModifierStrategy(IServiceProvider serviceProvider)
			: base(serviceProvider) {
		}
		public override void DeleteComponents(params Component[] components) {
			DesignerTransaction transaction = DesignerHost.CreateTransaction(DesignSR.Trans_Delete);
			try {
				foreach(Component component in components)
					if(component != null)
						component.Dispose();
				transaction.Commit();
			}
			catch {
				transaction.Cancel();
			}
		}
		public override void InsertGroupFields(GroupField[] sourceArray, GroupFieldCollection dest, int groupFieldCount) {
			for(int i = 0; i < sourceArray.Length && i < groupFieldCount; i++)
				dest.Insert(i, sourceArray[i]);
		}
		public override void AddGroupFooter(XtraReportBase report, int level) {
			GroupFooterBand footerBand = new GroupFooterBand();
			string description = String.Format(DesignSR.Trans_Add, footerBand.GetType().Name);
			DesignerTransaction transaction = DesignerHost.CreateTransaction(description);
			try {
				string name = BandDesigner.CreateBandName(DesignerHost, footerBand);
				footerBand.Level = level;
				report.Bands.Add(footerBand);
				DesignToolHelper.AddToContainer(DesignerHost, footerBand, name);
				transaction.Commit();
			}
			catch {
				transaction.Cancel();
			}
		}
		public override void AddGroupHeader(ReportAddGroupHeaderModifier modifier, XtraReportBase report, GroupFieldCollection sourceFields, int level) {
			GroupHeaderBand headerBand = modifier.CreateHeader();
			string description = String.Format(DesignSR.Trans_Add, headerBand.GetType().Name);
			DesignerTransaction transaction = DesignerHost.CreateTransaction(description);
			try {
				string name = BandDesigner.CreateBandName(DesignerHost, headerBand);
				DesignToolHelper.AddToContainer(DesignerHost, headerBand, name);
				modifier.AddGroupFields(sourceFields.ToArray(), headerBand.GroupFields);
				headerBand.Level = level;
				transaction.Commit();
			}
			catch {
				transaction.Cancel();
			}
		}
		public override void Modify(BandModifier modifier, params Band[] bands) {
			string propertyName = bands[0].SortFieldsPropertyName;
			string desc = String.Format(DesignSR.TransFmt_SetProperty, propertyName);
			DesignerTransaction trans = DesignerHost.CreateTransaction(desc);
			try {
				IComponentChangeService changeService = ServiceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				foreach(Band band in bands)
					XRControlDesignerBase.RaiseComponentChanging(changeService, band, band.SortFieldsPropertyName);
				modifier.ModifyCore(bands);
				foreach(Band band in bands)
					XRControlDesignerBase.RaiseComponentChanged(changeService, band);
				trans.Commit();
			}
			catch {
				trans.Cancel();
			}
		}
	}
	public class CheckModifierStrategy : ModifierStrategyBase {
		bool valid = true;
		LockService lockService;
		public CheckModifierStrategy(IServiceProvider serviceProvider)
			: base(serviceProvider) {
			lockService = LockService.GetInstance(ServiceProvider);
		}
		public bool Valid {
			get { return valid; }
			set { valid = value; }
		}
		public override void DeleteComponents(params Component[] components) {
			if (components == null || components.Length == 0)
				return;
			foreach(Component component in components) {
				if(component == null)
					continue;
				if(!lockService.CanChangeComponent(component)) {
					valid = false;
					return;
				}
			}
		}
		public override void InsertGroupFields(GroupField[] sourceArray, GroupFieldCollection dest, int groupFieldCount) {
			if(dest == null || dest.Band == null || groupFieldCount == 0)
				return;
			if(!lockService.CanChangeComponent(dest.Band))
				valid = false;
		}
		public override void AddGroupFooter(XtraReportBase report, int level) {
		}
		public override void AddGroupHeader(ReportAddGroupHeaderModifier modifier, XtraReportBase report, GroupFieldCollection sourceFields, int level) {
		}
		public override void Modify(BandModifier modifier, params Band[] bands) {
			if(bands == null && bands.Length == 0)
				return;
			foreach(Component component in bands) {
				if(component == null)
					continue;
				if(!lockService.CanChangeComponent(component)) {
					valid = false;
					return;
				}
			}
		}
	}
	public class ModifierBase {
		ModifierStrategyBase modifierStrategy;
		public ModifierBase(ModifierStrategyBase modifierStrategy) {
			this.modifierStrategy = modifierStrategy;
		}
		protected ModifierStrategyBase ModifierStrategy { get { return modifierStrategy; } }
	}
	public class ReportDeleteModifier : ModifierBase {
		public ReportDeleteModifier(ModifierStrategyBase modifierStrategy)
			: base(modifierStrategy) {
		}
		public void DeleteComponents(params Component[] components) {
			ModifierStrategy.DeleteComponents(components);
		}
	}
	public class ReportDeleteHeaderModifier : ReportDeleteModifier {
		public ReportDeleteHeaderModifier(ModifierStrategyBase modifierStrategy)
			: base(modifierStrategy) {
		}
		public void DeleteHeader(XRGroup group, GroupFieldCollection destFields, int groupFieldCount) {
			if(destFields != null) {
				ModifierStrategy.InsertGroupFields(group.GroupFields.ToArray(), destFields, groupFieldCount);
			}
			ModifierStrategy.DeleteComponents(group.Header, group.Footer);
		}
	}
	public class ReportAddGroupFooterModifier : ModifierBase {
		public ReportAddGroupFooterModifier(ModifierStrategyBase modifierStrategy)
			: base(modifierStrategy) {
		}
		public void AddGroupFooter(XtraReportBase report, int level) {
			ModifierStrategy.AddGroupFooter(report, level);
		}
	}
	public class ReportAddGroupHeaderModifier : ModifierBase {
		public ReportAddGroupHeaderModifier(ModifierStrategyBase modifierStrategy)
			: base(modifierStrategy) {
		}
		public void AddGroupHeader(XtraReportBase report, GroupFieldCollection sourceFields, int level) {
			ModifierStrategy.AddGroupHeader(this, report, sourceFields, level);
		}
		protected internal virtual GroupHeaderBand CreateHeader() {
			return XtraReport.CreateBand(BandKind.GroupHeader) as GroupHeaderBand;
		}
		protected internal virtual void AddGroupFields(GroupField[] sourceArray, GroupFieldCollection dest) {
			for(int i = 0; i < sourceArray.Length; i++)
				dest.Add(sourceArray[i]);
		}
	}
	public class ReportGroupHeaderModifier : ReportAddGroupHeaderModifier {
		string fieldName;
		public ReportGroupHeaderModifier(ModifierStrategyBase modifierStrategy, string fieldName)
			: base(modifierStrategy) {
			this.fieldName = fieldName;
		}
		protected internal override GroupHeaderBand CreateHeader() {
			GroupHeaderBand headerBand = base.CreateHeader();
			headerBand.GroupFields.Add(new GroupField(fieldName));
			return headerBand;
		}
		protected internal override void AddGroupFields(GroupField[] sourceArray, GroupFieldCollection dest) {
			for(int i = 0; i < sourceArray.Length; i++)
				dest.Insert(i, sourceArray[i]);
		}
	}
	public class ReportGroupHeaderModifier2 : ReportAddGroupHeaderModifier {
		int groupFieldCount;
		public ReportGroupHeaderModifier2(ModifierStrategyBase modifierStrategy, int groupFieldCount)
			: base(modifierStrategy) {
			this.groupFieldCount = groupFieldCount;
		}
		protected internal override void AddGroupFields(GroupField[] sourceArray, GroupFieldCollection dest) {
			for(int i = 0; i < sourceArray.Length && i < groupFieldCount; i++)
				dest.Add(sourceArray[i]);
		}
	}
	public abstract class BandModifier : ModifierBase {
		protected BandModifier(ModifierStrategyBase modifierStrategy)
			: base(modifierStrategy) {
		}
		public void Modify(params Band[] bands) {
			ModifierStrategy.Modify(this, bands);
		}
		protected internal abstract void ModifyCore(params Band[] bands);
	}
	public class BandPropertyModifier : BandModifier {
		object component;
		PropertyDescriptor propertyDescriptor;
		object value;
		public BandPropertyModifier(ModifierStrategyBase modifierStrategy, object component, PropertyDescriptor propertyDescriptor, object value)
			: base(modifierStrategy) {
			this.component = component;
			this.propertyDescriptor = propertyDescriptor;
			this.value = value;
		}
		protected internal override void ModifyCore(params Band[] bands) {
			propertyDescriptor.SetValue(component, value);
		}
	}
	public class BandAddSortModifier : BandModifier {
		string fieldName;
		public BandAddSortModifier(ModifierStrategyBase modifierStrategy, string fieldName)
			: base(modifierStrategy) {
			this.fieldName = fieldName;
		}
		protected internal override void ModifyCore(params Band[] bands) {
			bands[0].SortFieldsInternal.Add(new GroupField(fieldName));
		}
	}
	public class BandRemoveSortModifier : BandModifier {
		protected GroupField groupField;
		public BandRemoveSortModifier(ModifierStrategyBase modifierStrategy, GroupField groupField)
			: base(modifierStrategy) {
			this.groupField = groupField;
		}
		protected internal override void ModifyCore(params Band[] bands) {
			GroupFieldCollection groupFields = bands[0].SortFieldsInternal;
			groupFields.Remove(groupField);
		}
	}
	public class BandMoveSortDownModifier : BandModifier {
		GroupField groupField;
		public BandMoveSortDownModifier(ModifierStrategyBase modifierStrategy, GroupField groupField)
			: base(modifierStrategy) {
			this.groupField = groupField;
		}
		protected internal override void ModifyCore(params Band[] bands) {
			bands[0].SortFieldsInternal.Insert(bands[0].SortFieldsInternal.Count - 1, groupField);
		}
	}
	public class BandMoveSortDownModifier2 : BandModifier {
		GroupField groupField;
		public BandMoveSortDownModifier2(ModifierStrategyBase modifierStrategy, GroupField groupField)
			: base(modifierStrategy) {
			this.groupField = groupField;
		}
		protected internal override void ModifyCore(params Band[] bands) {
			MoveGroupFieldDown(bands[0].SortFieldsInternal);
		}
		void MoveGroupFieldDown(GroupFieldCollection owner) {
			int index = owner.IndexOf(groupField);
			if(index + 1 < owner.Count) {
				owner.RemoveAt(index);
				owner.Insert(index + 1, groupField);
			}
		}
	}
	public class BandMoveSortDownModifier3 : BandModifier {
		GroupField groupField;
		public BandMoveSortDownModifier3(ModifierStrategyBase modifierStrategy, GroupField groupField)
			: base(modifierStrategy) {
			this.groupField = groupField;
		}
		protected internal override void ModifyCore(params Band[] bands) {
			Band nextBand = bands[1];
			nextBand.SortFieldsInternal.Insert(0, groupField);
		}
	}
	public class BandMoveGroupDownModifier : BandModifier {
		public BandMoveGroupDownModifier(ModifierStrategyBase modifierStrategy)
			: base(modifierStrategy) {
		}
		protected internal override void ModifyCore(params Band[] bands) {
			Band band = bands[0];
			Band nextBand = bands[1];
			GroupField[] groupFields = band.SortFieldsInternal.ToArray();
			for(int i = 0; i < groupFields.Length - 1; i++)
				nextBand.SortFieldsInternal.Insert(i, groupFields[i]);
			band.LevelInternal = nextBand.LevelInternal;
		}
	}
}
