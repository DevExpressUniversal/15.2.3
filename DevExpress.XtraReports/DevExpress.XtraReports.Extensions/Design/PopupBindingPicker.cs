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

using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors;
using DevExpress.Data.Browsing.Design;
using System;
using DevExpress.XtraReports.Native;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.Native.Data;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Browsing;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using DevExpress.Data;
namespace DevExpress.XtraReports.Design {
	public interface IPopupFieldNamePicker : IDisposable {
		bool IsDisposed { get; }
		int Width { get; set; }
		void Start(IServiceProvider provider, object dataSource, string dataMember, object value, Control containerControl);
		string EndFieldNamePicker();
	}
	public class PopupFieldNamePicker : PopupBindingPickerBase, IPopupFieldNamePicker {
		#region inner classes
#if DEBUGTEST
		public
#endif
		class DisplayNameProviderPickManager : TreeListPickManager {
			IDisplayNameProvider displayNameProvider;
			string rootDataMember = String.Empty;
			string rootDisplayDataMember = String.Empty;
			public DisplayNameProviderPickManager()
				: this(new DataContextOptions(true, true)) {
			}
			public DisplayNameProviderPickManager(DataContextOptions options)
				: base(options) {
			}
			public void SetDisplayNameProvider(IDisplayNameProvider displayNameProvider, string rootDataMember) {
				this.displayNameProvider = displayNameProvider;
				this.rootDataMember = rootDataMember;
				if(displayNameProvider == null || string.IsNullOrEmpty(rootDataMember))
					return;
				rootDisplayDataMember = GetFieldDisplayName(rootDataMember, displayNameProvider);
			}
			static string GetFieldDisplayName(string path, IDisplayNameProvider displayNameProvider) {
				Guard.ArgumentIsNotNullOrEmpty(path, "path");
				Guard.ArgumentNotNull(displayNameProvider, "displayNameProvider");
				string[] accessors = path.Split('.');
				return ((IDisplayNameProvider)displayNameProvider).GetFieldDisplayName(accessors);
			}
			public override void FindDataMemberNode(IList nodes, string dataMember, Action<INode> callback) {
				FindDataMemberNodeCore(nodes, dataMember, 0, callback, true);
			}
			public override void GetDataSourceName(object dataSource, string dataMember, IPropertiesProvider provider, EventHandler<GetDataSourceDisplayNameEventArgs> callback) {
				string result = null;
				if(!string.IsNullOrEmpty(rootDisplayDataMember))
					result = rootDisplayDataMember;
				else if(displayNameProvider != null)
					result = ((PropertiesProvider)provider).DataContext.GetCustomDataSourceDisplayName(displayNameProvider);
				else if(!string.IsNullOrEmpty(dataMember))
					result = ((PropertiesProvider)provider).DataContext.GetDataMemberDisplayName(dataSource, dataMember);
				else {
					base.GetDataSourceName(dataSource, dataMember, provider, callback);
					return;
				}
				callback(this, new GetDataSourceDisplayNameEventArgs(result));
			}
#if DEBUGTEST
			public string Test_RootDisplayDataMember { get { return rootDisplayDataMember; } }
#endif
		} 
		#endregion
		string dataMember;
		public PopupFieldNamePicker()
			: base(new DisplayNameProviderPickManager()) {
		}
		public PopupFieldNamePicker(DataContextOptions options)
			: base(new DisplayNameProviderPickManager(options)) {
		}
		public void Start(IServiceProvider provider, object dataSource, string dataMember, object value, Control containerControl) {
			((DisplayNameProviderPickManager)PickManager).SetDisplayNameProvider(dataSource as IDisplayNameProvider, dataMember);
			if (value == null)
				value = String.Empty;
			Collection<Pair<object, string>> dataSourceDataMemberPairs = new Collection<Pair<object, string>>();
			dataSourceDataMemberPairs.Add(new Pair<object, string>(dataSource, dataMember));
			this.dataMember = dataMember;
			Start(dataSourceDataMemberPairs, provider, new DesignBinding(dataSource, (string)value), containerControl);
		}
		public string EndFieldNamePicker() {
			string resultDataMember = GetSelectedDataMember();
			End();
			return resultDataMember;
		}
		internal string GetSelectedDataMember(){
			string resultDataMember = SelectedBinding.DataMember;
			if(string.IsNullOrEmpty(dataMember) || string.IsNullOrEmpty(resultDataMember) || !resultDataMember.StartsWith(dataMember))
				return resultDataMember;
			return resultDataMember.Substring(dataMember.Length + 1);
		}
	}
}
