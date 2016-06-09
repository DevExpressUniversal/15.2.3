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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native.DrillDown;
namespace DevExpress.XtraReports.UI {
	[
	BandKind(BandKind.SubBand),
	XRDesigner("DevExpress.XtraReports.Design.SubBandDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design.SubBandDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.SubBand", "SubBand"),
	]
	public class SubBand : Band {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override SubBandCollection SubBands {
			get { return SubBandCollection.Empty; }
		}
		internal override XRControlCollectionBase ControlContainer { 
			get { return Parent is Band ? ((Band)Parent).SubBands : null; }
		}
		protected override DocumentBandKind DocumentBandKind {
			get {
				Band parent = new XRControlParentInfo(this).FindParent(item => !(item is SubBand)) as Band;
				return parent != null ? parent.BandKind.ToDocumentBandKind() : BandKind.ToDocumentBandKind();
			}
		}
		protected internal override void OnBeforePrint(System.Drawing.Printing.PrintEventArgs e) {
			base.OnBeforePrint(e);
			if(Parent is IDrillDownNode) {
				IDrillDownService serv = RootReport.GetService<IDrillDownService>();
				if(serv != null && !serv.BandExpanded((Band)Parent))
					e.Cancel = true;
			}
		}
	}
	public class SubBandCollection : XRControlCollectionBase, IEnumerable<SubBand> {
		static SubBandCollection empty = new SubBandCollection(null);
		public static SubBandCollection Empty { 
			get {
				return empty;
			}
		}
		public SubBand this[int index] {
			get { return (SubBand)List[index]; }
		}
		public SubBandCollection(Band owner)
			: base(owner) {
		}
		protected override void OnInsert(int index, object value) {
			base.OnInsert(index, value);
			if(!(value is SubBand))
				throw new ArgumentException("value");
		}
		public bool Contains(SubBand item) {
			return List.Contains(item);
		}
		public int IndexOf(SubBand item) {
			return List.IndexOf(item);
		}
		public void Remove(SubBand item) {
			List.Remove(item);
		}
		public void AddRange(SubBand[] items) {
			foreach(SubBand item in items)
				Add(item);
		}
		public int Add(SubBand item) {
			return List.Contains(item) ? IndexOf(item) : List.Add(item);
		}
		#region IEnumerable<SubBand> Members
		IEnumerator<SubBand> IEnumerable<SubBand>.GetEnumerator() {
			foreach(SubBand band in List)
				yield return band;
		}
		#endregion
	}
}
