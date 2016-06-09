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

using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using System;
using System.ComponentModel;
using System.Reflection;
#if SL
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Scheduler.Commands {
	public abstract class XpfSchedulerCommand : SchedulerCommand {
		static InnerSchedulerControl GetInnerSchedulerControl(ISchedulerCommandTarget target) {
			Guard.ArgumentNotNull(target, "target");
			IInnerSchedulerCommandTarget actualTarget = target as IInnerSchedulerCommandTarget;
			if (actualTarget == null)
				Exceptions.ThrowArgumentException("actualTarget", actualTarget);
			InnerSchedulerControl innerControl = actualTarget.InnerSchedulerControl;
			if (innerControl == null)
				Exceptions.ThrowArgumentException("control", innerControl);
			return innerControl;
		}
		protected XpfSchedulerCommand(ISchedulerCommandTarget target)
			: base(GetInnerSchedulerControl(target)) {
		}
		#region Properties
		protected override Assembly ImageResourceAssembly { get { return Assembly.GetExecutingAssembly(); } }
		protected override string ImageResourcePrefix { get { return "DevExpress.Xpf.Scheduler.Images"; } }
		public override SchedulerStringId DescriptionStringId {
			get { throw new NotSupportedException(); }
		}
		public override SchedulerStringId MenuCaptionStringId {
			get { throw new NotSupportedException(); }
		}
		public override string MenuCaption { get { return SchedulerCommandLocalizationHelper.GetMenuCaption(this); } }
		public override string Description { get { return SchedulerCommandLocalizationHelper.GetDescription(this); } }
		#endregion
	}
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CommandLocalizationAttribute : Attribute {
		#region properties
		public SchedulerControlStringId MenuCaptionId { get; private set; }
		public SchedulerControlStringId DescriptionId { get; private set; }
		#endregion
		public CommandLocalizationAttribute(SchedulerControlStringId menuCaptionId, SchedulerControlStringId descriptionId) {
			this.MenuCaptionId = menuCaptionId;
			this.DescriptionId = descriptionId;
		}
	}
	public static class SchedulerCommandLocalizationHelper {
		public static string GetMenuCaption(XpfSchedulerCommand schedulerCommand) {
			CommandLocalizationAttribute xpfSchedulerCommandAttribute = TypeDescriptor.GetAttributes(schedulerCommand)[typeof(CommandLocalizationAttribute)] as CommandLocalizationAttribute;
			return xpfSchedulerCommandAttribute != null ? SchedulerControlLocalizer.Active.GetLocalizedString(xpfSchedulerCommandAttribute.MenuCaptionId) : string.Empty;
		}
		public static string GetDescription(XpfSchedulerCommand schedulerCommand) {
			CommandLocalizationAttribute xpfSchedulerCommandAttribute = TypeDescriptor.GetAttributes(schedulerCommand)[typeof(CommandLocalizationAttribute)] as CommandLocalizationAttribute;
			return xpfSchedulerCommandAttribute != null ? SchedulerControlLocalizer.Active.GetLocalizedString(xpfSchedulerCommandAttribute.DescriptionId) : string.Empty;
		}
	}
}
