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
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Preview.Native {
	public abstract class PrintBarManagerUpdaterBase : PrintBarManagerConfigurator {
		public static bool GetUpdateNeeded(BarManager barManager, PrintingSystemCommand[] commands) {
			foreach(PrintingSystemCommand command in commands) {
				if(BarItemPresent(command, barManager))
					return false;
			}
			return true;
		}
		static bool BarItemPresent(PrintingSystemCommand command, BarManager barManager) {
			foreach(BarItem barItem in barManager.Items) {
				ISupportPrintingSystemCommand item = barItem as ISupportPrintingSystemCommand;
				if(item != null && item.Command == command)
					return true;
			}
			return false;
		}
		protected int baseImageIndex;
		protected PrintBarManagerConfigurator parentConfigurator;
		protected virtual PrintingSystemCommand[] UpdateCommands { get { return new PrintingSystemCommand[] { }; } }
		protected virtual string ImagesFileName { get { return string.Empty; } }
		public override bool UpdateNeeded { get { return GetUpdateNeeded(PrintBarManager, UpdateCommands); } }
		protected PrintBarManagerUpdaterBase(PrintBarManager manager, PrintBarManagerConfigurator parentConfigurator)
			: base(manager) {
			this.parentConfigurator = parentConfigurator;
		}
		protected PrintBarManagerUpdaterBase(PrintBarManager manager)
			: base(manager) {
			parentConfigurator = this;
		}
		public override void ConfigInternal() {
			baseImageIndex = PrintBarManager.Images.Count;
			if(!string.IsNullOrEmpty(ImagesFileName))
				ImageCollectionHelper.AddImagesToCollectionFromResources(PrintBarManager.ImageCollection, string.Format("DevExpress.XtraPrinting.Images.{0}.png", ImagesFileName), System.Reflection.Assembly.GetExecutingAssembly());
			CreateUpdateItems();
		}
		protected abstract void CreateUpdateItems();
	}
}
