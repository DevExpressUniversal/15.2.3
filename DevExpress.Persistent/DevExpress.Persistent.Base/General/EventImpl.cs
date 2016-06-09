#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
namespace DevExpress.Persistent.Base.General {
	public class EventImpl {
		private string subject;
		private bool allDay;
		private DateTime startOn;
		private DateTime endOn;
		private string description;
		private string location;
		private int label;
		private int status;
		private int type;
		private IResource resource;
		public void AfterConstruction() {
			startOn = DateTime.Now;
			endOn = startOn.AddHours(1);
		}
		public string Subject {
			get { return subject; 	}
			set { subject = value; }
		}
		public string Description {
			get { return description; 	}
			set { description = value; }
		}
		public DateTime StartOn {
			get { return startOn; 	}
			set { startOn = value; }
		}
		public DateTime EndOn {
			get { return endOn; 	}
			set { endOn = value; }
		}
		public bool AllDay {
			get { return allDay; 	}
			set { allDay = value; }
		}
		public string Location {
			get { return location; }
			set { location = value; }
		}
		public int Label {
			get { return label; }
			set { label = value; }
		}
		public int Status {
			get { return status; }
			set { status = value; }
		}
		public int Type {
			get { return type; }
			set { type = value; }
		}
		public IResource Resource {
			get { return resource; }
			set { resource = value; }
		}
	}
}
