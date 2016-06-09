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
using System.IO;
using System.Linq;
using System.Text;
namespace DevExpress.Utils.StoredObjects {
	public static class StoreAgentRepository {
		#region // inner classes
		class FromTypeKey {
			Type type;
			int hashCode;
			public FromTypeKey(Type type) {
				this.type = type;
				this.hashCode = type.GetHashCode();
			}
			public override int GetHashCode() {
				return this.hashCode;
			}
			public override bool Equals(object o) {
				return o is FromTypeKey && EqualsCore((FromTypeKey)o);
			}
			bool EqualsCore(FromTypeKey key) {
				return type == key.type;
			}
		}
		#endregion
		#region // static
		static object synch = new object();
		static Dictionary<FromTypeKey, IStoreAgent> registeredAgents = new Dictionary<FromTypeKey, IStoreAgent>();
		static StoreAgentRepository() {
			Register<string>(new StringStoreAgent());
			Register<bool>(new BooleanStoreAgent());
			Register<float>(new SingleStoreAgent());
			Register<int>(new IntegerStoreAgent());
		}
		public static void Register<T>(StoreAgent<T> agent) {
			FromTypeKey key = new FromTypeKey(typeof(T));
			lock(synch) {
				registeredAgents[key] = agent;
			}
		}
		public static bool TryGetAgent(Type objType, out IStoreAgent value) {
			FromTypeKey key = new FromTypeKey(objType);
			lock(synch) {
				return registeredAgents.TryGetValue(key, out value);
			}
		}
		#endregion
	} 
	public interface IStoreAgent {
		Type ObjectType { get; }
		void StoreObject(IRepositoryProvider provider, Object obj, BinaryWriter writer);
		object RestoreObject(IRepositoryProvider provider, BinaryReader reader);
	}
	public abstract class StoreAgent<T> : IStoreAgent {
		public Type ObjectType { get { return typeof(T); } }
		public abstract void StoreObject(IRepositoryProvider provider, Object obj, BinaryWriter writer);
		public abstract object RestoreObject(IRepositoryProvider provider, BinaryReader reader);
	}
	public class StringStoreAgent : StoreAgent<string> {
		public override void StoreObject(IRepositoryProvider provider, Object obj, BinaryWriter writer) {
			writer.Write(provider.StoreObject<string>((string)obj));
		}
		public override object RestoreObject(IRepositoryProvider provider, BinaryReader reader) {
			return provider.RestoreObject<string>(reader.ReadInt64(), string.Empty);
		}
	}
	public class BooleanStoreAgent : StoreAgent<Boolean> {
		public override void StoreObject(IRepositoryProvider provider, Object obj, BinaryWriter writer) {
			writer.Write((bool)obj);
		}
		public override object RestoreObject(IRepositoryProvider provider, BinaryReader reader) {
			return reader.ReadBoolean();
		}
	}
	public class SingleStoreAgent : StoreAgent<float> {
		public override void StoreObject(IRepositoryProvider provider, Object obj, BinaryWriter writer) {
			writer.Write((float)obj);
		}
		public override object RestoreObject(IRepositoryProvider provider, BinaryReader reader) {
			return reader.ReadSingle();
		}
	}
	public class IntegerStoreAgent : StoreAgent<int> {
		public override void StoreObject(IRepositoryProvider provider, Object obj, BinaryWriter writer) {
			writer.Write((int)obj);
		}
		public override object RestoreObject(IRepositoryProvider provider, BinaryReader reader) {
			return reader.ReadInt32();
		}
	}
}
