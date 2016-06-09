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
using System.Collections;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.UI {
	public class XRCrossBandControlCollection : XRControlCollectionBase {
		public XRCrossBandControl this[int index] {
			get { return (XRCrossBandControl)List[index]; }
		}
		public XRCrossBandControl this[string name] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i] != null && this[i].Name == name)
						return this[i];
				}
				return null;
			}
		}
		XtraReport Report { get { return (XtraReport)base.owner; } }
		public XRCrossBandControlCollection(XtraReport owner) : base(owner) {
		}
		protected override void OnInsert(int index, object value) {
			base.OnInsert(index, value);
			if(!(value is XRCrossBandControl))
				throw new ArgumentException("value");
		}
		public bool Contains(XRCrossBandControl item) {
			return List.Contains(item);
		}
		public int IndexOf(XRCrossBandControl item) {
			return List.IndexOf(item);
		}
		public void Remove(XRCrossBandControl item) {
			List.Remove(item);
		}
		public void AddRange(XRCrossBandControl[] controls) {
			foreach(XRCrossBandControl item in controls)
				Add(item);
		}
		public int Add(XRCrossBandControl child) {
			return List.Contains(child) ? IndexOf(child) : List.Add(child);
		}
		internal void SyncDpi(float dpi) {
			foreach(XRCrossBandControl control in this)
				control.SyncDpi(dpi);
		}
		internal List<XRControl> GetPrintableControls(Band band) {
			List<XRControl> printableControls = new List<XRControl>();
			int bandIndex = this.Report.AllBands.IndexOf(band);
			if(bandIndex < 0)
				return printableControls;
			foreach(XRCrossBandControl cbControl in this) {
				XRControl[] controls = cbControl.GetPrintableControls(band);
				if(controls != null)
					printableControls.AddRange(controls);
			}
			return printableControls;
		}
	}
}
