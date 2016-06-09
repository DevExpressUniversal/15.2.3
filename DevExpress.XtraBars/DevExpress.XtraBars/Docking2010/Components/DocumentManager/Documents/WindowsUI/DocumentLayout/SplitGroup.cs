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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ISplitGroupProperties : IDocumentGroupProperties { }
	public interface ISplitGroupDefaultProperties : IDocumentGroupDefaultProperties { }
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class SplitGroup : DocumentGroup {
		public SplitGroup()
			: base((IContainer)null) {
		}
		public SplitGroup(IContainer container)
			: base(container) {
		}
		public SplitGroup(ISplitGroupProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override IContentContainerDefaultProperties CreateDefaultProperties(IContentContainerProperties parentProperties) {
			return new SplitGroupDefaultProperties(parentProperties as ISplitGroupProperties);
		}
		protected override IContentContainerInfo CreateContentContainerInfo(WindowsUIView view) {
			return new SplitGroupInfo(view, this);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("SplitGroupProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ISplitGroupDefaultProperties Properties {
			get { return base.Properties as ISplitGroupDefaultProperties; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("SplitGroupDetailContainerProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IContentContainerDefaultProperties DetailContainerProperties {
			get { return base.DetailContainerPropertiesCore; }
		}
		protected override DevExpress.Utils.Base.IBaseProperties GetParentProperties(WindowsUIView view) {
			return view.SplitGroupProperties;
		}
		protected override void EnsureDeferredControlLoadDocuments() {
			foreach(Document document in Items)
				document.EnsureIsBoundToControl(Info.Owner);
		}
		protected override void GetActualActionsCore(IList<IContentContainerAction> actions) {
			base.GetActualActionsCore(actions);
			actions.Add(SplitGroupAction.Rotate);
			actions.Add(SplitGroupAction.Flip);
		}
	}
	public class SplitGroupProperties : DocumentGroupProperties, ISplitGroupProperties {
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new SplitGroupProperties();
		}
	}
	public class SplitGroupDefaultProperties : DocumentGroupDefaultProperties, ISplitGroupDefaultProperties {
		public SplitGroupDefaultProperties(ISplitGroupProperties parentProperties)
			: base(parentProperties) {
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new SplitGroupDefaultProperties(ParentProperties as ISplitGroupProperties);
		}
	}
	public abstract class SplitGroupAction : ContentContainerAction {
		#region static
		public static readonly IContentContainerAction Rotate = new RotateAction();
		public static readonly IContentContainerAction Flip = new FlipAction();
		#endregion static
		protected override bool CanExecuteCore(IContentContainer container) {
			SplitGroup group = container as SplitGroup;
			return (group != null) && group.Items.Count > 1;
		}
		[ActionGroup("SplitGroup", ActionType.Context, Order = 1, Index = 0)]
		class RotateAction : SplitGroupAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandRotate; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandRotateDescription; }
			}
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				view.Controller.Rotate(container as SplitGroup);
			}
		}
		[ActionGroup("SplitGroup", ActionType.Context, Order = 1, Index = 1)]
		class FlipAction : SplitGroupAction {
			protected override DocumentManagerStringId ID {
				get { return DocumentManagerStringId.CommandFlip; }
			}
			protected override DocumentManagerStringId DescriptionID {
				get { return DocumentManagerStringId.CommandFlipDescription; }
			}
			protected override void ExecuteCore(WindowsUIView view, IContentContainer container) {
				view.Controller.Flip(container as SplitGroup);
			}
		}
	}
}
