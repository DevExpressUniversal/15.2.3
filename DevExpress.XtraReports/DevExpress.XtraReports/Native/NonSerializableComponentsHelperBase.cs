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
using DevExpress.XtraReports.UI;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
namespace DevExpress.XtraReports.Native {
	public class NonSerializableComponentsHelperBase {
		protected List<IComponent> nonSerializableComponents = new List<IComponent>();
		IList<IComponent> container;
		public NonSerializableComponentsHelperBase() {
		}
		public virtual List<IComponent> GetNonSerializableComponents(XtraReport report) {
			this.container = report.ComponentStorage;
			NestedComponentEnumerator enumerator = new NestedComponentEnumerator(new object[] { report });
			while(enumerator.MoveNext()) {
				IComponent[] components = enumerator.Current.GetNonSerializableComponents();
				foreach(IComponent item in components)
					AddComponentCore(item);
				IDataContainer dataContainer = enumerator.Current as IDataContainer;
				if(dataContainer == null)
					continue;
				ProcessDataContainer(dataContainer);
			}
			return nonSerializableComponents;
		}
		protected virtual void ProcessDataContainer(IDataContainer dataContainer) {
			object dataSource = dataContainer.GetSerializableDataSource();
			if(dataSource is XRPivotGrid || dataSource is DevExpress.Data.IListAdapter)
				return;
			AddComponent(dataSource as IComponent);
			AddComponent(dataContainer.DataAdapter as IComponent);
		}
		protected void AddComponent(IComponent item) {
			if(item == null || (container != null && container.Contains(item)))
				return;
			AddComponentCore(item);
			ICollection associatedComponents = GetAssociatedComponents(item);
			if(associatedComponents == null)
				return;
			foreach(object associatedComponent in associatedComponents)
				AddComponent(associatedComponent as IComponent);
		}
		void AddComponentCore(IComponent item) {
			if(item != null && !nonSerializableComponents.Contains(item))
				nonSerializableComponents.Add(item);
		}
		protected virtual ICollection GetAssociatedComponents(IComponent item) {
			if(item is BindingSource)
				return new object[] { ((BindingSource)item).DataSource };
			return null;
		}
	}
	class NonSerializableComponentsHelper : NonSerializableComponentsHelperBase {
		protected override void ProcessDataContainer(IDataContainer dataContainer) {
			IComponent dataSource = dataContainer.GetSerializableDataSource() as IComponent;
			IComponent dataAdapter = dataContainer.DataAdapter as IComponent;
			if(dataAdapter == null || dataSource == null) {
				base.ProcessDataContainer(dataContainer);
			}
		}
	}
}
