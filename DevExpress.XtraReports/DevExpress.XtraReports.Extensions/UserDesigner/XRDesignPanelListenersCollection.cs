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
namespace DevExpress.XtraReports.UserDesigner {
	[TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter))]
	public class XRDesignPanelListenersCollection : System.Collections.CollectionBase {
		IServiceProvider serviceProvider;
		public XRDesignPanelListener this[int index] { get { return (XRDesignPanelListener)InnerList[index]; } }
		public XRDesignPanelListenersCollection(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			XRDesignPanelListener item = (XRDesignPanelListener)value;
			if(item.DesignControl is IWeakServiceProvider)
				((IWeakServiceProvider)item.DesignControl).ServiceProvider = serviceProvider;
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			if(value is IWeakServiceProvider)
				((IWeakServiceProvider)value).ServiceProvider = null;
		}
		public int Add(XRDesignPanelListener item) {
			return List.Add(item);
		}
		public int Add(IDesignPanelListener item) {
			return List.Add(new XRDesignPanelListener(item));
		}
		public void AddRange(XRDesignPanelListener[] items) {
			foreach(XRDesignPanelListener item in items)
				Add(item);
		}
		public void Remove(XRDesignPanelListener item) {
			List.Remove(item);
		}
		public void Remove(IDesignPanelListener item) {
			XRDesignPanelListener designPanelListener = FindDesignPanelListener(item);
			if(designPanelListener != null)
				Remove(designPanelListener);
		}
		XRDesignPanelListener FindDesignPanelListener(IDesignPanelListener item) {
			foreach(XRDesignPanelListener designPanelListener in this) {
				if(designPanelListener.DesignControl == item)
					return designPanelListener;
			}
			return null;
		}
		public bool Contains(IDesignPanelListener item) {
			XRDesignPanelListener designPanelListener = FindDesignPanelListener(item);
			return designPanelListener != null ? Contains(designPanelListener) : false;
		}
		public bool Contains(XRDesignPanelListener item) {
			return List.Contains(item);
		}
	}
}
