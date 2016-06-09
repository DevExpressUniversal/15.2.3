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

using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.WebUtils;
using System;
using System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum PivotGroupFilterMode { List, Tree }
	public delegate void PivotOptionsFilterEventHandler(object sender, PivotOptionsFilterEventArgs e);
	public class PivotOptionsFilterEventArgs : EventArgs {
		bool isUpdateRequired;
		internal PivotOptionsFilterEventArgs(bool isUpdateRequired) {
			this.isUpdateRequired = isUpdateRequired;
		}
		public bool IsUpdateRequired { get { return isUpdateRequired; } }
	}
	public class PivotGridOptionsFilterBase : PivotGridOptionsBase {
		PivotGroupFilterMode groupFilterMode;
		bool showOnlyAvailableItems;
		public PivotGridOptionsFilterBase(PivotOptionsFilterEventHandler optionsChanged)
			: this(optionsChanged, null, string.Empty) {
		}
		public PivotGridOptionsFilterBase(PivotOptionsFilterEventHandler optionsChanged, IViewBagOwner viewBagOwner, string projectPath)
			: base(null, viewBagOwner, projectPath) {
			this.OptionsChanged = optionsChanged;
			this.groupFilterMode = PivotGroupFilterMode.Tree;
			this.showOnlyAvailableItems = false;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsFilterBaseGroupFilterMode"),
#endif
		DefaultValue(PivotGroupFilterMode.Tree), XtraSerializableProperty()
		]
		public PivotGroupFilterMode GroupFilterMode {
			get { return groupFilterMode; }
			set {
				if(groupFilterMode == value) return;
				groupFilterMode = value;
				OnOptionsChanged(true);
			}
		}
		[DefaultValue(false), XtraSerializableProperty()]
		public bool ShowOnlyAvailableItems {
			get { return showOnlyAvailableItems; }
			set {
				if(showOnlyAvailableItems == value) return;
				showOnlyAvailableItems = value;
				OnOptionsChanged(false);
			}
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new event PivotOptionsFilterEventHandler OptionsChanged;
		protected void OnOptionsChanged(bool isLayoutChanged) {
			if(OptionsChanged != null) OptionsChanged(this, new PivotOptionsFilterEventArgs(isLayoutChanged));
		}
		protected override void OnOptionsChanged() {
			this.OnOptionsChanged(false);
		}
	}
}
