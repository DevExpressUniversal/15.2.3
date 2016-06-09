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
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using DevExpress.Xpo.DB;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo;
using System.Collections.Generic;
using DevExpress.Xpo.Helpers;
using DevExpress.Data.Db;
using DevExpress.Xpo.Logger;
using DevExpress.Utils;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DevExpress.Compatibility.System.Collections.Specialized;
#if !CF && !SL
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
#if !DXPORTABLE
using System.Security.Principal;
#endif
using System.Security;
using DevExpress.Xpo.Exceptions;
#endif
#if DXRESTRICTED
using IDbTransaction = System.Data.Common.DbTransaction;
using IDataReader = System.Data.Common.DbDataReader;
using IDbConnection = System.Data.Common.DbConnection;
using IDbCommand = System.Data.Common.DbCommand;
using IDataParameter = System.Data.Common.DbParameter;
using IDbDataParameter = System.Data.Common.DbParameter;
#endif
namespace DevExpress {
	class NonCoverAttribute : Attribute {
	}
}
namespace DevExpress.Xpo.DB.Helpers {
	public delegate int GetValidBatchLengthCallback<T>(IList<T> items, int batchStart, int desiredLength, int batchesCount);
	public static class BatchBreaker {
		public static Dictionary<int, int> Do<T>(IList<T> statements, IEqualityComparer<T> comparer) {
			return Do<T>(statements, comparer
				, 3
				, x => true
				, (stbs, batchStart, desiredBatchLength, batchesCount) => batchesCount <= 1 ? Math.Min(desiredBatchLength, 2) : desiredBatchLength)
				;
		}
		public static Dictionary<int, int> Do<T>(IList<T> statements, IEqualityComparer<T> comparer, int MaxRepeatableBatchLength, Predicate<T> isBatcheableStatement, GetValidBatchLengthCallback<T> isValidBatch) {
			Dictionary<int, int> rv = new Dictionary<int, int>(statements.Count);
			bool[] used = new bool[statements.Count];
			if(isBatcheableStatement != null) {
				for(int i = 0; i < statements.Count; ++i) {
					if(!isBatcheableStatement(statements[i])) {
						rv.Add(i, 1);
						used[i] = true;
					}
				}
			}
			if(rv.Count == used.Length)
				return rv;
			if(rv.Count == 0 && GetValidBatchLength(statements, isValidBatch, 0, statements.Count, -1) == statements.Count) {
				rv.Add(0, statements.Count);
				return rv;
			}
			for(int batchLength = MaxRepeatableBatchLength; batchLength > 0 ; --batchLength) {
				if(used.Length < batchLength * 2)
					continue;
				SortedList<int, byte> batchesStarts = GenerateBatchesStarts(used, batchLength);
				IEqualityComparer<Batch> batchComparer = null;
				while(batchesStarts.Count > 1) {
					if(batchComparer == null)
						batchComparer = new BatchComparer<T>(batchLength, statements, comparer);
					List<int> best = null;
					foreach(BatchStartsCounter counter in GenerateStartsByBatches(batchLength, batchesStarts.Keys, batchComparer)) {
						if(counter.ManyStarts == null) {
							batchesStarts.Remove(counter.SingleStart);
						} else if(best == null || best.Count < counter.ManyStarts.Count) {
							if(GetValidBatchLength(statements, isValidBatch, counter.ManyStarts[0], batchLength, counter.ManyStarts.Count) == batchLength) {
								best = counter.ManyStarts;
							} else {
								foreach(int start in counter.ManyStarts) {
									batchesStarts.Remove(start);
								}
							}
						}
					}
					if(best != null) {
						foreach(int start in best) {
							rv.Add(start, batchLength);
							for(int i = 0; i < batchLength; ++i) {
								int pos = start + i;
								if(used[pos])
									throw new InvalidOperationException("internal error (statement already used)");
								used[pos] = true;
							}
							for(int i = start - batchLength + 1; i < start + batchLength; ++i) {
								batchesStarts.Remove(i);
							}
						}
					}
				}
			}
			{
				int batchStart = -1;
				for(int i = 0; i <= used.Length; ++i) {
					if(i == used.Length || used[i]) {
						if(batchStart >= 0) {
							int leftover = i - batchStart;
							int currentBatchStart = batchStart;
							while(leftover > 0) {
								int batchLength = GetValidBatchLength(statements, isValidBatch, currentBatchStart, leftover, 1);
								rv.Add(currentBatchStart, batchLength);
								currentBatchStart += batchLength;
								leftover -= batchLength;
							}
							batchStart = -1;
						}
					} else {
						if(batchStart < 0)
							batchStart = i;
					}
				}
				if(batchStart >= 0)
					throw new InvalidOperationException("batchStart >= 0");
			}
			return rv;
		}
		static int GetValidBatchLength<T>(IList<T> statements, GetValidBatchLengthCallback<T> isValidBatch, int currentBatchStart, int batchLength, int batchesCount) {
			if(batchLength <= 1)
				return 1;
			if(isValidBatch != null) {
				batchLength = isValidBatch(statements, currentBatchStart, batchLength, batchesCount);
				if(batchLength < 1)
					return 1;
			}
			return batchLength;
		}
		struct Batch {
			public int Start;
			public Batch(int start) {
				this.Start = start;
			}
		}
		struct BatchStartsCounter {
			public int SingleStart; 
			public List<int> ManyStarts;
		}
		class BatchComparer<T>: IEqualityComparer<Batch> {
			readonly int BatchLength;
			readonly IList<T> Bricks;
			readonly IEqualityComparer<T> BrickComparer;
			public BatchComparer(int batchLength, IList<T> bricks, IEqualityComparer<T> brickComparer) {
				this.BatchLength = batchLength;
				this.Bricks = bricks;
				this.BrickComparer = brickComparer;
			}
			public bool Equals(Batch x, Batch y) {
				if(x.Start == y.Start)
					return true;
				for(int i = 0; i < BatchLength; ++i) {
					if(!BrickComparer.Equals(Bricks[x.Start + i], Bricks[y.Start + i]))
						return false;
				}
				return true;
			}
			public int GetHashCode(Batch obj) {
				int rv = 0;
				for(int i = 0; i < BatchLength; ++i) {
					int hash = BrickComparer.GetHashCode(Bricks[obj.Start + i]);
					rv = unchecked(rv * 13 + hash);
				}
				return rv;
			}
		}
		static IEnumerable<BatchStartsCounter> GenerateStartsByBatches(int batchLength, IList<int> batchesStarts, IEqualityComparer<Batch> batchesMatcher) {
			Dictionary<Batch, BatchStartsCounter> startsByBatches = new Dictionary<Batch, BatchStartsCounter>(batchesStarts.Count, batchesMatcher);
			foreach(int batchStart in batchesStarts) {
				Batch batch = new Batch(batchStart);
				BatchStartsCounter starts;
				if(startsByBatches.TryGetValue(batch, out starts)) {
					if(starts.ManyStarts == null) {
						if(batchStart - starts.SingleStart < batchLength) {
						} else {
							starts.ManyStarts = new List<int>();
							starts.ManyStarts.Add(starts.SingleStart);
							starts.ManyStarts.Add(batchStart);
							starts.SingleStart = -1;
							startsByBatches[batch] = starts;	
						}
					} else {
						int lastPrev = starts.ManyStarts[starts.ManyStarts.Count - 1];
						if(batchStart - lastPrev < batchLength) {
						} else {
							starts.ManyStarts.Add(batchStart);
						}
					}
				} else {
					starts = new BatchStartsCounter();
					starts.SingleStart = batchStart;
					startsByBatches.Add(batch, starts);
				}
			}
			return startsByBatches.Values;
		}
		static SortedList<int, byte> GenerateBatchesStarts(bool[] used, int batchLength) {
			SortedList<int, byte> batchesStarts = new SortedList<int, byte>();
			int blockStart = -1;
			for(int i = 0; i < used.Length; ++i) {
				if(used[i]) {
					blockStart = -1;
				} else {
					if(blockStart < 0)
						blockStart = i;
					int batchStartThatEndsOnI = i - batchLength + 1;
					if(batchStartThatEndsOnI >= blockStart)
						batchesStarts.Add(batchStartThatEndsOnI, 0);
				}
			}
			return batchesStarts;
		}
	}
	public class BatchBreakerModificationStatementStub {
		public static int DefaultMaxRepeatableBatchLength = 3;
		public static Predicate<BatchBreakerModificationStatementStub> DefaultIsBacheableStatement = x => x.ApproxParametersCount <= 96;
		public static GetValidBatchLengthCallback<BatchBreakerModificationStatementStub> DefaultGetValidBatchLength =
 (stbs, batchStart, desiredBatchLength, batchesCount) => {
	 int sum = stbs[batchStart].ApproxParametersCount;
	 for(int i = 1; i < desiredBatchLength; ++i) {
		 sum += stbs[batchStart + i].ApproxParametersCount;
		 if(sum > 64) {
			 if(desiredBatchLength > 2)
				 return i;
			 if(sum > 128)
				 return i;
		 }
		 if(batchesCount <= 1 && i >= 5)
			 return i;
	 }
	 return desiredBatchLength;
 };
		public object Token;
		public readonly ModificationStatement Statement;
		public readonly int ApproxParametersCount;
		int? HashCode;
		public BatchBreakerModificationStatementStub(ModificationStatement original) {
			this.Statement = original;
			this.ApproxParametersCount = CalculateParameters(Statement);
		}
		static int CalculateParameters(ModificationStatement stmt) {
			int rv = stmt.Parameters.OfType<ParameterValue>().Count();
			rv += ParametersCounter.ApproximateParametersCount(stmt.Condition);
			InsertStatement ins = stmt as InsertStatement;
			if(ins != null && !ReferenceEquals(ins.IdentityParameter, null))
				++rv;
			return rv;
		}
		public int FastGetHashCode() {
			if(!HashCode.HasValue) {
				HashCode = SlowGetHashCode();
			}
			return HashCode.Value;
		}
		int CombineHashes(int a, int b) {
			return unchecked((a * 17) ^ b);
		}
		int CombineHashes(params int[] hashes) {
			int rv = 0;
			foreach(int h in hashes)
				rv = CombineHashes(rv, h);
			return rv;
		}
		int SlowGetHashCode() {
			ModificationStatement a = Statement;
			return CombineHashes(ApproxParametersCount, a.Table.GetHashCode(), a.RecordsAffected.GetHashCode(), a.SubNodes.Count, a.Operands.Count, a.GetType().GetHashCode(), GetParamsHashes());
		}
		int GetParamsHashes() {
			int rv = 1;
			int? firstParameterIndex = null;
			foreach(OperandValue v in Statement.Parameters) {
				int hash;
				ParameterValue pv = v as ParameterValue;
				if(ReferenceEquals(pv, null))
					hash = v.GetHashCode();
				else {
					if(!firstParameterIndex.HasValue)
						firstParameterIndex = pv.Tag;
					hash = pv.Tag - firstParameterIndex.Value;
				}
				rv = CombineHashes(rv, hash);
			}
			return rv;
		}
		bool SlowEquals(BatchBreakerModificationStatementStub another) {
			if(ApproxParametersCount != another.ApproxParametersCount)
				return false;
			ModificationStatement a = this.Statement;
			ModificationStatement b = another.Statement;
			if(!object.Equals(a.Table, b.Table))
				return false;
			if(a.Alias != b.Alias)
				return false;
			if(a.RecordsAffected != b.RecordsAffected)
				return false;
			if(a.SubNodes.Count != b.SubNodes.Count)
				return false;
			if(a.Operands.Count != b.Operands.Count)
				return false;
			if(a.Parameters.Count != b.Parameters.Count)
				return false;
			if(a.GetType() != b.GetType())
				return false;
			int? pTagShift = null;
			for(int i = 0; i < a.Parameters.Count; ++i) {
				OperandValue ova = a.Parameters[i];
				OperandValue ovb = b.Parameters[i];
				ParameterValue pva = ova as ParameterValue;
				ParameterValue pvb = ovb as ParameterValue;
				if(!ReferenceEquals(pva, null)) {
					if(ReferenceEquals(pvb, null))
						return false;
					int shift = pva.Tag - pvb.Tag;
					if(pTagShift.HasValue) {
						if(pTagShift.Value != shift)
							return false;
					} else {
						pTagShift = shift;
					}
					if((pva.Value == null) != (pvb.Value == null))
						return false;
				} else {
					if(!ReferenceEquals(pvb, null))
						return false;
					if(!Equals(ova, ovb))
						return false;
				}
			}
			for(int i = 0; i < a.Operands.Count; ++i) {
				if(!Equals(a.Operands[i], b.Operands[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			throw new InvalidOperationException();
		}
		public override bool Equals(object obj) {
			throw new InvalidOperationException();
		}
		class ParametersCounter: IQueryCriteriaVisitor {
			ParametersCounter() { }
			int counter;
			public void Visit(QuerySubQueryContainer theOperand) {
				Process(theOperand.AggregateProperty);
				if(theOperand.Node != null) {
					Process(theOperand.Node.Condition);
				}
			}
			public void Visit(QueryOperand theOperand) {
			}
			public void Visit(FunctionOperator theOperator) {
				Process(theOperator.Operands);
			}
			public void Visit(OperandValue theOperand) {
				if(theOperand is ParameterValue)
					++counter;
			}
			public void Visit(GroupOperator theOperator) {
				Process(theOperator.Operands);
			}
			public void Visit(InOperator theOperator) {
				Process(theOperator.LeftOperand);
				Process(theOperator.Operands);
			}
			public void Visit(UnaryOperator theOperator) {
				Process(theOperator.Operand);
			}
			public void Visit(BinaryOperator theOperator) {
				Process(theOperator.LeftOperand);
				Process(theOperator.RightOperand);
			}
			public void Visit(BetweenOperator theOperator) {
				Process(theOperator.TestExpression);
				Process(theOperator.BeginExpression);
				Process(theOperator.EndExpression);
			}
			void Process(CriteriaOperator op) {
				if(ReferenceEquals(op, null))
					return;
				op.Accept(this);
			}
			void Process(IEnumerable<CriteriaOperator> ops) {
				foreach(CriteriaOperator op in ops)
					Process(op);
			}
			public static int ApproximateParametersCount(CriteriaOperator op) {
				if(ReferenceEquals(op, null))
					return 0;
				ParametersCounter cnt = new ParametersCounter();
				cnt.Process(op);
				return cnt.counter;
			}
			public static int ApproximateParametersCount(IEnumerable<CriteriaOperator> ops) {
				ParametersCounter cnt = null;
				foreach(CriteriaOperator op in ops) {
					if(ReferenceEquals(op, null))
						continue;
					if(cnt == null)
						cnt = new ParametersCounter();
					cnt.Process(op);
				}
				if(cnt == null)
					return 0;
				return cnt.counter;
			}
		}
		public class FastComparer: IEqualityComparer<BatchBreakerModificationStatementStub> {
			class SlowComparer: IEqualityComparer<BatchBreakerModificationStatementStub> {
				SlowComparer() { }
				public static readonly SlowComparer Instance = new SlowComparer();
				public bool Equals(BatchBreakerModificationStatementStub x, BatchBreakerModificationStatementStub y) {
					return x.SlowEquals(y);
				}
				public int GetHashCode(BatchBreakerModificationStatementStub obj) {
					return obj.FastGetHashCode();
				}
			}
			readonly Dictionary<BatchBreakerModificationStatementStub, object> Tokens = new Dictionary<BatchBreakerModificationStatementStub, object>(SlowComparer.Instance);
			void EnsureToken(BatchBreakerModificationStatementStub x) {
				if(x.Token != null)
					return;
				object token;
				if(Tokens.TryGetValue(x, out token)) {
					x.Token = token;
				} else {
					x.Token = x;
					Tokens.Add(x, x.Token);
				}
			}
			public bool Equals(BatchBreakerModificationStatementStub x, BatchBreakerModificationStatementStub y) {
				EnsureToken(x);
				EnsureToken(y);
				return ReferenceEquals(x.Token, y.Token);
			}
			public int GetHashCode(BatchBreakerModificationStatementStub obj) {
				return obj.FastGetHashCode();
			}
		}
	}
}
namespace DevExpress.Xpo.DB {
	public class MSSqlProviderFactory : ProviderFactory {
		public override IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return MSSqlConnectionProvider.CreateProviderFromConnection(connection, autoCreateOption);
		}
		public override IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return MSSqlConnectionProvider.CreateProviderFromString(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override string GetConnectionString(Dictionary<string, string> parameters) {
			string connectionString;
			bool useIntegratedSecurity = false;
			if(parameters.ContainsKey(UseIntegratedSecurityParamID)) {
				useIntegratedSecurity = Convert.ToBoolean(parameters[UseIntegratedSecurityParamID]);
			}
			if(useIntegratedSecurity) {
				if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID)) { return null; }
				connectionString = MSSqlConnectionProvider.GetConnectionString(parameters[ServerParamID], parameters[DatabaseParamID]);
			} else {
				if(!parameters.ContainsKey(ServerParamID) || !parameters.ContainsKey(DatabaseParamID) ||
					!parameters.ContainsKey(UserIDParamID) || !parameters.ContainsKey(PasswordParamID)) {
					return null;
				}
				connectionString = MSSqlConnectionProvider.GetConnectionString(parameters[ServerParamID], parameters[UserIDParamID], parameters[PasswordParamID], parameters[DatabaseParamID]);
			}
			return connectionString;
		}
		public override IDataStore CreateProvider(Dictionary<string, string> parameters, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			string connectionString = GetConnectionString(parameters);
			if(connectionString == null) {
				objectsToDisposeOnDisconnect = new IDisposable[0];
				return null;
			}
			ConnectionStringParser helper = new ConnectionStringParser(connectionString);
			helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
			return CreateProviderFromString(helper.GetConnectionString(), autoCreateOption, out objectsToDisposeOnDisconnect);
		}
		public override bool HasUserName { get { return true; } }
		public override bool HasPassword { get { return true; } }
		public override bool HasIntegratedSecurity { get { return true; } }
		public override bool HasMultipleDatabases { get { return true; } }
		public override bool IsServerbased { get { return true; } }
		public override bool IsFilebased { get { return false; } }
		public override string ProviderKey { get { return MSSqlConnectionProvider.XpoProviderTypeString; } }
		public override bool SupportStoredProcedures { get { return true; } }
		string GetConnectionString(string server, string userid, string password) {
			if(String.IsNullOrEmpty(userid))
				return String.Format("data source={0};integrated security=SSPI", server);
			else
				return String.Format("data source={0};user id={1};password={2}", server, userid, password);
		}
		public override string[] GetDatabases(string server, string userid, string password) {
			if(String.IsNullOrEmpty(server))
				return new string[0];
			using(SqlConnection connection = new SqlConnection(GetConnectionString(server, userid, password))) {
				try {
					connection.Open();
				} catch {
					return new string[0];
				}
				using(SqlCommand command = new SqlCommand("select name from master..sysdatabases", connection)) {
					using(SqlDataReader reader = command.ExecuteReader()) {
						List<string> result = new List<string>();
						while(reader.Read()) {
							string name = reader.GetString(0);
							if(name != "master" && name != "model" && name != "tempdb" && name != "msdb" && name != "pubs")
								result.Add(name);
						}
						connection.Close();
						return result.ToArray();
					}
				}
			}
		}
		public override string FileFilter { get { return null; } }
		public override bool MeanSchemaGeneration { get { return true; } }
	}
	public class MSSqlConnectionProvider : ConnectionProviderSql {
		public const string XpoProviderTypeString = "MSSqlServer";
		const string ConnectionStringParameterInitialCatalog = "initial catalog";
		const string ConnectionStringParameterAttachDbFilename = "AttachDbFilename";
		const string ConnectionStringParameterUserInstance = "User Instance";
		const string ConnectionStringParameterDatabase = "Database";
		public static bool IsNotForReplication = true;
		protected static string GetConnectionStringForType(string providerTypeString, string server, string userid, string password, string database) {
			return String.Format("{4}={5};data source={0};user id={1};password={2};initial catalog={3};Persist Security Info=true", server, userid, password, database, DataStoreBase.XpoProviderTypeParameterName, providerTypeString);
		}
		protected static string GetConnectionStringForType(string providerTypeString, string server, string database) {
			return String.Format("{2}={3};data source={0};integrated security=SSPI;initial catalog={1}", server, database, DataStoreBase.XpoProviderTypeParameterName, providerTypeString);
		}
		protected static string GetConnectionStringForTypeWithAttach(string providerTypeString, string server, string userid, string password, string attachDbFilename, bool userInstance) {
			return String.Format("{4}={5};data source={0};user id={1};password={2};AttachDbFilename={3};Persist Security Info=true;{6}", server, userid, password, attachDbFilename, DataStoreBase.XpoProviderTypeParameterName, providerTypeString, userInstance ? "User Instance=True;" : string.Empty);
		}
		protected static string GetConnectionStringForTypeWithAttach(string providerTypeString, string server, string attachDbFilename, bool userInstance) {
			return String.Format("{2}={3};data source={0};integrated security=SSPI;AttachDbFilename={1};{4}", server, attachDbFilename, DataStoreBase.XpoProviderTypeParameterName, providerTypeString, userInstance ? "User Instance=True;" : string.Empty);
		}
		protected static string GetConnectionStringForTypeWithAttachForLocalDB(string providerTypeString, string server, string database, string attachDbFilename) {
			return String.Format("{2}={3};Server={0};integrated security=SSPI;Database={4};AttachDbFilename={1};", server, attachDbFilename, DataStoreBase.XpoProviderTypeParameterName, providerTypeString, database);
		}
		public static string GetConnectionString(string server, string userid, string password, string database) {
			return GetConnectionStringForType(XpoProviderTypeString, server, userid, password, database);
		}
		public static string GetConnectionString(string server, string database) {
			return GetConnectionStringForType(XpoProviderTypeString, server, database);
		}
		public static string GetConnectionStringWithAttach(string server, string userid, string password, string attachDbFilename, bool userInstance) {
			return GetConnectionStringForTypeWithAttach(XpoProviderTypeString, server, userid, password, attachDbFilename, userInstance);
		}
		public static string GetConnectionStringWithAttach(string server, string attachDbFilename, bool userInstance) {
			return GetConnectionStringForTypeWithAttach(XpoProviderTypeString, server, attachDbFilename, userInstance);
		}
		public static string GetConnectionStringWithAttachForLocalDB(string server, string database, string attachDbFilename) {
			return GetConnectionStringForTypeWithAttachForLocalDB(XpoProviderTypeString, server, database, attachDbFilename);
		}
#if !CF && !DXPORTABLE
		public static string GetConnectionString2005WithCache(string server, string userid, string password, string database) {
			return GetConnectionStringForType(MSSql2005SqlDependencyCacheRoot.XpoProviderTypeString_WithCache, server, userid, password, database);
		}
		public static string GetConnectionString2005WithCache(string server, string database) {
			return GetConnectionStringForType(MSSql2005SqlDependencyCacheRoot.XpoProviderTypeString_WithCache, server, database);
		}
		public static string GetConnectionString2005CacheRoot(string server, string userid, string password, string database) {
			return GetConnectionStringForType(MSSql2005SqlDependencyCacheRoot.XpoProviderTypeString_CacheRoot, server, userid, password, database);
		}
		public static string GetConnectionString2005CacheRoot(string server, string database) {
			return GetConnectionStringForType(MSSql2005SqlDependencyCacheRoot.XpoProviderTypeString_CacheRoot, server, database);
		}
#endif
		public static IDataStore CreateProviderFromString(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDbConnection connection = new SqlConnection(connectionString);
			objectsToDisposeOnDisconnect = new IDisposable[] { connection };
			return CreateProviderFromConnection(connection, autoCreateOption);
		}
		public static IDataStore CreateProviderFromConnection(IDbConnection connection, AutoCreateOption autoCreateOption) {
			return new MSSqlConnectionProvider(connection, autoCreateOption);
		}
		static MSSqlConnectionProvider() {
			RegisterDataStoreProvider(XpoProviderTypeString, new DataStoreCreationFromStringDelegate(CreateProviderFromString));
			RegisterDataStoreProvider("System.Data.SqlClient.SqlConnection", new DataStoreCreationFromConnectionDelegate(CreateProviderFromConnection));
			RegisterFactory(new MSSqlProviderFactory());
		}
		public static void Register() { }
		public MSSqlConnectionProvider(IDbConnection connection, AutoCreateOption autoCreateOption)
			: base(connection, autoCreateOption) {
			ReadDbVersion(Connection);
		}
		public override string ComposeSafeSchemaName(string tableName) {
			string res = base.ComposeSafeSchemaName(tableName);
			return res == String.Empty && ObjectsOwner != null ? ObjectsOwner : res;
		}
		protected override string GetSqlCreateColumnTypeForBoolean(DBTable table, DBColumn column) {
			return "bit";
		}
		protected override string GetSqlCreateColumnTypeForByte(DBTable table, DBColumn column) {
			return "tinyint";
		}
		protected override string GetSqlCreateColumnTypeForSByte(DBTable table, DBColumn column) {
			return "numeric(3,0)";
		}
		protected override string GetSqlCreateColumnTypeForChar(DBTable table, DBColumn column) {
			return "nchar(1)";
		}
		protected override string GetSqlCreateColumnTypeForDecimal(DBTable table, DBColumn column) {
			return "money";
		}
		protected override string GetSqlCreateColumnTypeForDouble(DBTable table, DBColumn column) {
			return "double precision";
		}
		protected override string GetSqlCreateColumnTypeForSingle(DBTable table, DBColumn column) {
			return "float";
		}
		protected override string GetSqlCreateColumnTypeForInt32(DBTable table, DBColumn column) {
			return "int";
		}
		protected override string GetSqlCreateColumnTypeForUInt32(DBTable table, DBColumn column) {
			return "numeric(10,0)";
		}
		protected override string GetSqlCreateColumnTypeForInt16(DBTable table, DBColumn column) {
			return "smallint";
		}
		protected override string GetSqlCreateColumnTypeForUInt16(DBTable table, DBColumn column) {
			return "numeric(5,0)";
		}
		protected override string GetSqlCreateColumnTypeForInt64(DBTable table, DBColumn column) {
			return "bigint";
		}
		protected override string GetSqlCreateColumnTypeForUInt64(DBTable table, DBColumn column) {
			return "numeric(20,0)";
		}
		public const int MaximumStringSize = 4000;
		protected override string GetSqlCreateColumnTypeForString(DBTable table, DBColumn column) {
			if(column.Size > 0 && column.Size <= MaximumStringSize)
				return "nvarchar(" + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
			else if(is2005)
				return "nvarchar(max)";
			else
				return "ntext";
		}
		protected override string GetSqlCreateColumnTypeForDateTime(DBTable table, DBColumn column) {
			return "datetime";
		}
		protected override string GetSqlCreateColumnTypeForGuid(DBTable table, DBColumn column) {
			return "uniqueidentifier";
		}
		public const int MaximumBinarySize = 8000;
		protected override string GetSqlCreateColumnTypeForByteArray(DBTable table, DBColumn column) {
			if(column.Size > 0 && column.Size <= MaximumBinarySize)
				return "varbinary(" + column.Size.ToString(CultureInfo.InvariantCulture) + ')';
			else if(is2005)
				return "varbinary(max)";
			else
				return "image";
		}
		public override string GetSqlCreateColumnFullAttributes(DBTable table, DBColumn column) {
			string result = GetSqlCreateColumnType(table, column);
			if(column.IsKey)
				result += " NOT NULL";
			else
				result += " NULL";
			if(column.IsKey) {
				if(column.IsIdentity && (column.ColumnType == DBColumnType.Int32 || column.ColumnType == DBColumnType.Int64) && IsSingleColumnPKColumn(table, column))
					result += GetIsAzure() ? " IDENTITY" : " IDENTITY NOT FOR REPLICATION";
				else if(column.ColumnType == DBColumnType.Guid && IsSingleColumnPKColumn(table, column) && !GetIsAzure())
					result += " ROWGUIDCOL";
			}
			return result;
		}
		protected override object ConvertToDbParameter(object clientValue, TypeCode clientValueTypeCode) {
			switch(clientValueTypeCode) {
				case TypeCode.SByte:
					return (Int16)(SByte)clientValue;
				case TypeCode.UInt16:
					return (Int32)(UInt16)clientValue;
				case TypeCode.UInt32:
					return (Int64)(UInt32)clientValue;
				case TypeCode.UInt64:
					return (Decimal)(UInt64)clientValue;
			}
			return base.ConvertToDbParameter(clientValue, clientValueTypeCode);
		}
		protected override Int64 GetIdentity(Query sql) {
			object value = GetScalar(new Query(sql.Sql + "\nselect " + (is2000 ? "SCOPE_IDENTITY()" : "@@Identity") + ' ', sql.Parameters, sql.ParametersNames));
			return (value as IConvertible).ToInt64(CultureInfo.InvariantCulture);
		}
		protected override void CommandBuilderDeriveParameters(IDbCommand command) {
#if !DXRESTRICTED
			SqlCommandBuilder.DeriveParameters((SqlCommand)command);
#endif
		}
		protected override Exception WrapException(Exception e, IDbCommand query) {
			SqlException sqlException = e as SqlException;
			if(sqlException != null && sqlException.Errors.Count > 0) {
				if(sqlException.Errors[0].Number == 208 || sqlException.Errors[0].Number == 207)
					return new SchemaCorrectionNeededException(sqlException.Errors[0].Message, sqlException);
				if(sqlException.Errors[0].Number == 547 || sqlException.Errors[0].Number == 2627 || sqlException.Errors[0].Number == 2601)
					return new ConstraintViolationException(query.CommandText, GetParametersString(query), e);
			}
			return base.WrapException(e, query);
		}
		protected override bool IsDeadLock(Exception e) {
			SqlException sqlException = e as SqlException;
			if(sqlException != null && sqlException.Errors.Count > 0 && sqlException.Errors[0].Number == 1205) {
				return true;
			}
			return base.IsDeadLock(e);
		}
		protected override IDbConnection CreateConnection() {
			return new SqlConnection(ConnectionString);
		}
		protected override void CreateDataBase() {
#if !DXPORTABLE
#if !CF
			ConnectionStringParser helper = new ConnectionStringParser(ConnectionString);
			if(!CanCreateDatabase || helper.GetPartByName("Pooling").ToLower() == "false") {
				CreateDataBase((SqlConnection)Connection);
			} else {
				using(SqlConnection conn = new SqlConnection("Pooling=false;" + ConnectionString)) {
					CreateDataBase(conn);
				}
			}
#else
			if(CanCreateDatabase) {
				CreateDataBase((SqlConnection)Connection);
			}
#endif
#endif
		}
#if !DXPORTABLE
		void CreateDataBase(SqlConnection conn) {
			const int CannotOpenDatabaseError = 4060;
			const int CannotOpenFileError = 15350;
			const int CannotAttachFile = 1832;
			try {
				conn.Open();
			} catch(Exception e) {
				if(e is System.Data.SqlClient.SqlException) {
					int errorNumber = ((System.Data.SqlClient.SqlException)e).Number;
					if((errorNumber == CannotAttachFile || errorNumber == CannotOpenDatabaseError || errorNumber == CannotOpenFileError) && CanCreateDatabase && CreateDb()) {
						conn.Open();
						return;
					}
				}
				throw new UnableToOpenDatabaseException(XpoDefault.ConnectionStringRemovePassword(ConnectionString), e);
			}
		}
		const string AspDataDirectory = "|DataDirectory|\\"; 
		bool CreateDb() {
			ConnectionStringParser helper = new ConnectionStringParser(ConnectionString);			
			string dbName = helper.GetPartByName(ConnectionStringParameterInitialCatalog);
			if(!string.IsNullOrEmpty(dbName))
				dbName = dbName.Trim('\"');
			string filename = helper.GetPartByName(ConnectionStringParameterAttachDbFilename);
			if(!string.IsNullOrEmpty(filename))
				filename = filename.Trim('\"');
			if(!string.IsNullOrEmpty(dbName) && string.IsNullOrEmpty(filename)) {
				helper.RemovePartByName(ConnectionStringParameterInitialCatalog);
				string connectToServer = string.Concat(ConnectionStringParameterInitialCatalog, "=master;", helper.GetConnectionString());
				using(SqlConnection conn = new SqlConnection(connectToServer)) {
					conn.Open();
					using(IDbCommand c = conn.CreateCommand()) {
						c.CommandText = string.Concat("Create Database [", dbName, "]");
						c.ExecuteNonQuery();
					}
				}
				return true;
			} else {
				if(filename.StartsWith(AspDataDirectory)) {
					object data = AppDomain.CurrentDomain.GetData("DataDirectory");
					string dataDirectory = data as string;
					if(dataDirectory == null && data != null) return false;
					if(string.IsNullOrEmpty(dataDirectory)) {
						dataDirectory = AppDomain.CurrentDomain.BaseDirectory;
					}
					if(dataDirectory == null) {
						dataDirectory = "";
					}
					filename = Path.Combine(dataDirectory, filename.Substring(AspDataDirectory.Length));
				}
				if(string.IsNullOrEmpty(dbName)) {
					dbName = helper.GetPartByName(ConnectionStringParameterDatabase);
					if(!string.IsNullOrEmpty(dbName))
						dbName = dbName.Trim('\"');
				}
				if(!string.IsNullOrEmpty(filename) && (helper.PartExists(ConnectionStringParameterUserInstance) || !string.IsNullOrEmpty(dbName))) {
					helper.RemovePartByName(ConnectionStringParameterAttachDbFilename);
					helper.RemovePartByName(ConnectionStringParameterInitialCatalog);
					helper.RemovePartByName(ConnectionStringParameterDatabase);
					string connectToServer = string.Concat(ConnectionStringParameterInitialCatalog, "=tempdb;", helper.GetConnectionString());
					string databaseName = string.IsNullOrEmpty(dbName) ? System.IO.Path.GetFileNameWithoutExtension(filename) : dbName;
					using(var connection = new SqlConnection(connectToServer)) {
						connection.Open();
						using(var command = connection.CreateCommand()) {
							try {
								command.CommandText = string.Format("CREATE DATABASE [{0}] ON PRIMARY (NAME='{0}', FILENAME='{1}')", databaseName, filename);
								command.ExecuteNonQuery();
								command.CommandText = string.Format("EXEC sp_detach_db '{0}', 'true'", databaseName);
								command.ExecuteNonQuery();
							}catch(Exception){
								if(!File.Exists(filename)) throw;
							}
						}
					}
					return true;
				}
			}
			return false;
		}
#endif
		void ReadDbVersion(IDbConnection conn) {
			SqlConnection sqlConnection = conn as SqlConnection;
			if(sqlConnection == null) {
				using(IDbCommand c = CreateCommand(new Query("select @@MICROSOFTVERSION / 0x1000000"))){
					int version = Convert.ToInt32(c.ExecuteScalar());
					is2000 = version > 7;
					is2005 = is2000 && version > 8;
					is2008 = is2005 && version > 9;
					is2012 = is2008 && version > 10;
				}
			}else{
				is2000 = !sqlConnection.ServerVersion.StartsWith("07.");
				is2005 = is2000 && !sqlConnection.ServerVersion.StartsWith("08.");
				is2008 = is2005 && !sqlConnection.ServerVersion.StartsWith("09.");
				is2012 = is2008 && !sqlConnection.ServerVersion.StartsWith("10.");
			}
		}
		bool GetIsAzure() {
			if(!is2000)
				return false;
			if(!isAzure.HasValue) {
				using(IDbCommand c = CreateCommand(new Query("select SERVERPROPERTY('edition')"))) {
					isAzure = (string)c.ExecuteScalar() == "SQL Azure";
				}
			}
			return isAzure.Value;
		}
		bool is2000;
		bool is2005;
		bool is2008;
		bool is2012;
		bool? isAzure;
		bool Exec(IDbCommand command, IDictionary parameters) {
			return LogManager.Log<bool>(LogManager.LogCategorySQL, () => {
				try {
					command.CommandText = command.CommandText + " set @r=1";
#if !CF
					Trace.WriteLineIf(xpoSwitch.TraceInfo, new DbCommandTracer(command));
#endif
					SqlParameter ret = new SqlParameter("@r", SqlDbType.Int);
					ret.Direction = ParameterDirection.Output;
					((SqlCommand)command).Parameters.Add(ret);
					command.ExecuteNonQuery();
					foreach(DictionaryEntry entry in parameters) {
						IDbDataParameter p = (IDbDataParameter)command.Parameters[(string)entry.Value];
						if(p.Direction != ParameterDirection.Output)
							continue;
						((ParameterValue)entry.Key).Value = p.Value;
					}
					return ((int)ret.Value) > 0;
				} catch(Exception e) {
					throw WrapException(e, command);
				}
			}, (d) => {
				return LogMessage.CreateMessage(this, command, d);
			});
		}
		class MSSqlSpInsertSqlGenerator : InsertSqlGenerator {
			public MSSqlSpInsertSqlGenerator(ISqlGeneratorFormatter formatter, TaggedParametersHolder identities, Dictionary<OperandValue, string> parameters) : base(formatter, identities, parameters) { }
			string identParamName;
			public Query GenerateSql(InsertStatement ins, string identParamName) {
				this.identParamName = identParamName;
				return GenerateSql(ins);
			}
			protected override string InternalGenerateSql() {
				InsertStatement ins = Root as InsertStatement;
				if(ReferenceEquals(ins, null)) { throw new InvalidOperationException(Res.GetString(Res.MsSql_RootIsNotInsertStatement)); }
				StringBuilder result = new StringBuilder();
				result.AppendFormat(CultureInfo.InvariantCulture, "EXEC [sp_{0}_insert] @{1}={2} OUT", ins.Table.Name, ins.IdentityColumn, identParamName);
				for(int i = 0; i < Root.Operands.Count; i++) {
					string name = Process(Root.Operands[i]);
					string val = GetNextParameterName(((InsertStatement)Root).Parameters[i]);
					result.AppendFormat(CultureInfo.InvariantCulture, ", @{0} = {1}", ((QueryOperand)Root.Operands[i]).ColumnName, val);
				}
				result.Append(';');
				return result.ToString();
			}
		}
		protected override ModificationResult ProcessModifyData(ModificationStatement[] dmlStatements) {
			BeginTransaction();
			try {
				IDbCommand command = (SqlCommand)CreateCommand();
				TaggedParametersHolder identities = new TaggedParametersHolder();
				List<ParameterValue> result = new List<ParameterValue>();
				Dictionary<OperandValue, string> parameters = new Dictionary<OperandValue, string>();
				StringBuilder sql = new StringBuilder();
				IList<BatchBreakerModificationStatementStub> stubs = dmlStatements.Select(x => new BatchBreakerModificationStatementStub(x)).ToArray();
				Dictionary<int, int> batches = BatchBreaker.Do(stubs, new BatchBreakerModificationStatementStub.FastComparer()
					, BatchBreakerModificationStatementStub.DefaultMaxRepeatableBatchLength
					, BatchBreakerModificationStatementStub.DefaultIsBacheableStatement
					, BatchBreakerModificationStatementStub.DefaultGetValidBatchLength);
				int pos = 0;
				int left = 0;
				foreach(ModificationStatement dml in dmlStatements) {
					if(left == 0) {
						left = batches[pos];
						pos += left;
					}
					if(sql.Length > 0) {
						sql.Append(' ');
					}
					if(dml is InsertStatement) {
						InsertStatement ins = (InsertStatement)dml;
						if(!ReferenceEquals(ins.IdentityParameter, null)) {
							ins.IdentityParameter.Value = DBNull.Value;
							identities.ConsolidateIdentity(ins.IdentityParameter);
							result.Add(ins.IdentityParameter);
							bool createParameter = true;
							IDataParameter param = CreateParameter(command, null, GetParameterName(ins.IdentityParameter, parameters.Count, ref createParameter));
							param.DbType = ins.IdentityColumnType == DBColumnType.Int32 ? DbType.Int32 : DbType.Int64;
							param.Direction = ParameterDirection.Output;
							parameters.Add(ins.IdentityParameter, param.ParameterName);
							command.Parameters.Add(param);
							string entityVal = is2000 ? "SCOPE_IDENTITY()" : "@@Identity";
							if(ins.IdentityColumn == IdentityColumnMagicName) {
								Query query = new MSSqlSpInsertSqlGenerator(this, identities, parameters).GenerateSql(ins, param.ParameterName);
								sql.Append(query.Sql);
								PrepareParameters(command, query);
							} else {
								Query query = new InsertSqlGenerator(this, identities, parameters).GenerateSql(ins);
								sql.Append(query.Sql);
								PrepareParameters(command, query);
								sql.AppendFormat(CultureInfo.InvariantCulture, " set {0}=" + entityVal, param.ParameterName);
							}
						} else {
							Query query = new InsertSqlGenerator(this, identities, parameters).GenerateSql(ins);
							PrepareParameters(command, query);
							sql.Append(query.Sql);
						}
					} else if(dml is UpdateStatement) {
						Query query = new UpdateSqlGenerator(this, identities, parameters).GenerateSql(dml);
						if(query.Sql != null) {
							sql.Append(query.Sql);
							PrepareParameters(command, query);
							if(dml.RecordsAffected != 0)
								sql.Append(" IF @@ROWCOUNT <> " + dml.RecordsAffected + " begin set @r=0 RETURN end ");
						}
					} else if(dml is DeleteStatement) {
						Query query = new DeleteSqlGenerator(this, identities, parameters).GenerateSql(dml);
						sql.Append(query.Sql);
						PrepareParameters(command, query);
						if(dml.RecordsAffected != 0)
							sql.Append(" IF @@ROWCOUNT <> " + dml.RecordsAffected + " begin set @r=0 RETURN end ");
					} else {
						throw new InvalidOperationException();
					}
					left--;
					if(left == 0) {
						command.CommandText = sql.ToString();
						if(!Exec(command, parameters)) {
							command = null;
							parameters = null;
							RollbackTransaction();
							throw new LockingException();
						}
						parameters = new Dictionary<OperandValue, string>();
						command.Dispose();
						command = CreateCommand();
						sql.Length = 0;
					}
				}
				if(sql.Length > 0) {
					Debug.Fail("s");
					command.CommandText = sql.ToString();
					if(!Exec(command, parameters)) {
						RollbackTransaction();
						throw new LockingException();
					}
				}
				command.Dispose();
				CommitTransaction();
				return new ModificationResult(result);
			} catch(Exception e) {
				try {
					RollbackTransaction();
				} catch(Exception e2) {
					throw new DevExpress.Xpo.Exceptions.ExceptionBundleException(e, e2);
				}
				throw;
			}
		}
		delegate bool TablesFilter(DBTable table);
		SelectStatementResult GetDataForTables(ICollection tables, TablesFilter filter, string queryText) {
			QueryParameterCollection parameters = new QueryParameterCollection();
			StringCollection inList = new StringCollection();
			StringBuilder names = new StringBuilder();
			Dictionary<string, string> schemas = new Dictionary<string, string>();
			foreach(DBTable table in tables) {
				if(filter == null || filter(table)) {
					parameters.Add(new OperandValue(ComposeSafeTableName(table.Name)));
					string name = "@p" + inList.Count.ToString(CultureInfo.InvariantCulture);
					inList.Add(name);
					if(names.Length > 0)
						names.Append(" OR ");
					names.Append("TABLE_NAME = ").Append(name);
					string schema = ComposeSafeSchemaName(table.Name);
					if(schema != string.Empty) {
						if(!schemas.TryGetValue(schema, out name)) {
							parameters.Add(new OperandValue(schema));
							name = "@p" + inList.Count.ToString(CultureInfo.InvariantCulture);
							inList.Add(name);
							schemas.Add(schema, name);
						}
						names.Append(" AND TABLE_SCHEMA = ").Append(name);
					}
				}
			}
			if(inList.Count == 0)
				return new SelectStatementResult();
			return SelectData(new Query(string.Format(CultureInfo.InvariantCulture, queryText, names.ToString()), parameters, inList));
		}
		DBColumnType GetTypeFromString(string typeName, int length) {
			switch(typeName) {
				case "int":
					return DBColumnType.Int32;
				case "image":
				case "varbinary":
					return DBColumnType.ByteArray;
				case "nchar":
				case "char":
					if(length == 1)
						return DBColumnType.Char;
					return DBColumnType.String;
				case "varchar":
				case "nvarchar":
				case "xml":
				case "ntext":
				case "text":
					return DBColumnType.String;
				case "bit":
					return DBColumnType.Boolean;
				case "tinyint":
					return DBColumnType.Byte;
				case "smallint":
					return DBColumnType.Int16;
				case "bigint":
					return DBColumnType.Int64;
				case "numeric":
				case "decimal":
					return DBColumnType.Decimal;
				case "money":
				case "smallmoney":
					return DBColumnType.Decimal;
				case "float":
					return DBColumnType.Double;
				case "real":
					return DBColumnType.Single;
				case "uniqueidentifier":
					return DBColumnType.Guid;
				case "datetime":
				case "datetime2":
				case "smalldatetime":
				case "date":
					return DBColumnType.DateTime;
			}
			return DBColumnType.Unknown;
		}
		void GetColumns(DBTable table) {
			string schema = ComposeSafeSchemaName(table.Name);
			Query query;
			if(string.IsNullOrEmpty(schema))
				query = new Query("select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @p1", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" });
			else
				query = new Query("select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = @p1 and TABLE_SCHEMA = @p2", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name)), new OperandValue(schema)), new string[] { "@p1", "@p2" });
			foreach(SelectStatementResultRow row in SelectData(query).Rows) {
				int size = row.Values[2] != DBNull.Value ? ((IConvertible)row.Values[2]).ToInt32(CultureInfo.InvariantCulture) : 0;
				DBColumnType type = GetTypeFromString((string)row.Values[1], size);
				table.AddColumn(new DBColumn((string)row.Values[0], false, String.Empty, type == DBColumnType.String ? size : 0, type));
			}
		}
		void GetPrimaryKey(DBTable table) {
			string schema = ComposeSafeSchemaName(table.Name);
			Query query;
			if(string.IsNullOrEmpty(schema))
				query = new Query(
					!is2005 ? @"select c.COLUMN_NAME, COLUMNPROPERTY(OBJECT_ID(c.TABLE_SCHEMA + '.' + c.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity')
from INFORMATION_SCHEMA.KEY_COLUMN_USAGE c 
join INFORMATION_SCHEMA.TABLE_CONSTRAINTS p on p.CONSTRAINT_NAME = c.CONSTRAINT_NAME 
where c.TABLE_NAME = @p1 and p.CONSTRAINT_TYPE = 'PRIMARY KEY'"
					:
@"select c.name, COLUMNPROPERTY(t.object_id, c.name, 'IsIdentity') from sys.key_constraints p
 join sys.index_columns i on p.parent_object_id = i.object_id and p.unique_index_id = i.index_id
 join sys.columns c on i.column_id = c.column_id and p.parent_object_id = c.object_id
 join sys.tables t on p.parent_object_id = t.object_id
where t.name = @p1 and p.type = 'PK'
order by i.key_ordinal"
					, new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" });
			else
				query = new Query(
					!is2005 ? @"SELECT
clmns.name,
COLUMNPROPERTY(tbl.id, clmns.name, 'IsIdentity')
FROM
dbo.sysobjects AS tbl
INNER JOIN sysusers AS stbl ON stbl.uid = tbl.uid
INNER JOIN dbo.syscolumns AS clmns ON clmns.id=tbl.id
LEFT OUTER JOIN dbo.sysindexes AS ik ON ik.id = clmns.id and 0 != ik.status & 0x0800
LEFT OUTER JOIN dbo.sysindexkeys AS cik ON cik.indid = ik.indid and cik.colid = clmns.colid and cik.id = clmns.id
WHERE
(tbl.type='U')and(tbl.name=@p1 and stbl.name=@p2) and cik.colid is not null" :
					@"select c.name, COLUMNPROPERTY(t.object_id, c.name, 'IsIdentity') from sys.key_constraints p
 join sys.index_columns i on p.parent_object_id = i.object_id and p.unique_index_id = i.index_id
 join sys.columns c on i.column_id = c.column_id and p.parent_object_id = c.object_id
 join sys.tables t on p.parent_object_id = t.object_id
 join sys.schemas s on s.schema_id = p.schema_id
where t.name = @p1 and p.type = 'PK' and s.name = @p2
order by i.key_ordinal"
					, new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name)), new OperandValue(schema)), new string[] { "@p1", "@p2" });
			SelectStatementResult data = SelectData(query);
			if(data.Rows.Length > 0) {
				StringCollection cols = new StringCollection();
				for(int i = 0; i < data.Rows.Length; i++)
					cols.Add((string)(data.Rows[i]).Values[0]);
				table.PrimaryKey = new DBPrimaryKey(cols);
				foreach(string columnName in cols) {
					DBColumn column = table.GetColumn(columnName);
					if(column != null)
						column.IsKey = true;
				}
				if(cols.Count == 1 && ((int)(data.Rows[0]).Values[1]) == 1)
					table.GetColumn(cols[0]).IsIdentity = true;
			}
		}
		void GetIndexes(DBTable table) {
			string schema = ComposeSafeSchemaName(table.Name);
			Query query;
			if(string.IsNullOrEmpty(schema)) {
				if(!is2005) {
					query = new Query(@"select i.name, c.name, INDEXPROPERTY(i.id, i.name, 'IsUnique') from sysobjects o
join sysindexes i on i.id=o.id
join sysindexkeys k on k.id=i.id and k.indid=i.indid
join syscolumns c on c.id=k.id and c.colid=k.colid
where o.name = @p1 and o.type='U' and i.name is not null and i.status&96=0
order by i.name, k.keyno", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" });
				} else {
					query = new Query(@"select i.name, c.name, i.is_unique from sys.objects t
join sys.indexes i on t.object_id = i.object_id
join sys.index_columns ic on ic.index_id = i.index_id and ic.object_id = t.object_id
join sys.columns c on c.column_id = ic.column_id and c.object_id = t.object_id
where t.name=@p1 and i.name is not null and ic.key_ordinal > 0
order by i.name, ic.key_ordinal", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" });
				}
			} else {
				if(!is2005) {
					query = new Query(@"select i.name, c.name, INDEXPROPERTY(i.id, i.name, 'IsUnique') from sysobjects o
join sysindexes i on i.id=o.id
join sysindexkeys k on k.id=i.id and k.indid=i.indid
join syscolumns c on c.id=k.id and c.colid=k.colid
join sysusers u on u.uid = o.uid
where o.name = @p1 and u.name = @p2 and o.type='U' and i.name is not null and i.status&96=0
order by i.name, k.keyno", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name)), new OperandValue(schema)), new string[] { "@p1", "@p2" });
				} else {
					query = new Query(@"select i.name, c.name, i.is_unique from sys.objects t
join sys.indexes i on t.object_id = i.object_id
join sys.index_columns ic on ic.index_id = i.index_id and ic.object_id = t.object_id
join sys.columns c on c.column_id = ic.column_id and c.object_id = t.object_id
join sys.schemas s on s.schema_id = t.schema_id
where t.name=@p1 and s.name = @p2 and i.name is not null and ic.key_ordinal > 0
order by i.name, ic.key_ordinal", new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name)), new OperandValue(schema)), new string[] { "@p1", "@p2" });
				}
			}
			SelectStatementResult data = SelectData(query);
			DBIndex index = null;
			foreach(SelectStatementResultRow row in data.Rows) {
				if(index == null || index.Name != (string)row.Values[0]) {
					StringCollection list = new StringCollection();
					list.Add((string)row.Values[1]);
					object isUnique = row.Values[2];
					index = new DBIndex((string)row.Values[0], list, isUnique is bool ? (bool)isUnique : (isUnique is int ? (int)isUnique == 1 : false));
					table.Indexes.Add(index);
				} else
					index.Columns.Add((string)row.Values[1]);
			}
		}
		void GetForeignKeys(DBTable table) {
			string schema = ComposeSafeSchemaName(table.Name);
			Query query;
			if(string.IsNullOrEmpty(schema))
				query = new Query(
					!is2005 ? @"select fk.name, c.name, rc.name, '', rtbl.name from sysforeignkeys fkdata
join sysobjects fk on fkdata.constid=fk.id
join sysobjects rtbl on rtbl.id=fkdata.rkeyid
join sysobjects tbl on tbl.id=fkdata.fkeyid
join syscolumns c on c.id=fkdata.fkeyid and c.colid=fkdata.fkey
join syscolumns rc on rc.id=fkdata.rkeyid and rc.colid=fkdata.rkey
where tbl.name = @p1
order by fk.name, fkdata.keyno"
					:
					@"select c.name, fk.name, pk.name, '', pkt.name from sys.foreign_key_columns r
 inner join sys.foreign_keys c on r.constraint_object_id = c.object_id
 inner join sys.columns fk on r.parent_object_id = fk.object_id and r.parent_column_id = fk.column_id
 inner join sys.tables fkt on r.parent_object_id = fkt.object_id
 inner join sys.columns pk on r.referenced_object_id = pk.object_id and r.referenced_column_id = pk.column_id
 inner join sys.tables pkt on r.referenced_object_id = pkt.object_id
where fkt.name = @p1
order by c.name, r.constraint_column_id"

					, new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name))), new string[] { "@p1" });
			else
				query = new Query(
					!is2005 ? @"select fk.name, c.name, rc.name, ru.name, rtbl.name from sysforeignkeys fkdata
join sysobjects fk on fkdata.constid=fk.id
join sysobjects rtbl on rtbl.id=fkdata.rkeyid
join sysobjects tbl on tbl.id=fkdata.fkeyid
join syscolumns c on c.id=fkdata.fkeyid and c.colid=fkdata.fkey
join syscolumns rc on rc.id=fkdata.rkeyid and rc.colid=fkdata.rkey
join sysusers u on u.uid = tbl.uid
join sysusers ru on ru.uid = rtbl.uid
where tbl.name = @p1 and u.name = @p2
order by fk.name, fkdata.keyno"
					: @"select c.name, fk.name, pk.name, pks.name, + pkt.name from sys.foreign_key_columns r
 inner join sys.foreign_keys c on r.constraint_object_id = c.object_id
 inner join sys.columns fk on r.parent_object_id = fk.object_id and r.parent_column_id = fk.column_id
 inner join sys.objects fkt on r.parent_object_id = fkt.object_id
 inner join sys.schemas fks on fks.schema_id = fkt.schema_id
 inner join sys.columns pk on r.referenced_object_id = pk.object_id and r.referenced_column_id = pk.column_id
 inner join sys.objects pkt on r.referenced_object_id = pkt.object_id
 inner join sys.schemas pks on pks.schema_id = pkt.schema_id
where fkt.name = @p1 and fks.name = @p2
order by c.name, r.constraint_column_id"
					, new QueryParameterCollection(new OperandValue(ComposeSafeTableName(table.Name)), new OperandValue(schema)), new string[] { "@p1", "@p2" });
			SelectStatementResult data = SelectData(query);
			Hashtable fks = new Hashtable();
			foreach(SelectStatementResultRow row in data.Rows) {
				DBForeignKey fk = (DBForeignKey)fks[row.Values[0]];
				if(fk == null) {
					StringCollection pkc = new StringCollection();
					StringCollection fkc = new StringCollection();
					pkc.Add((string)row.Values[1]);
					fkc.Add((string)row.Values[2]);
					string rtable = (string)row.Values[4];
					string rschema = (string)row.Values[3];
					if(ObjectsOwner != rschema && !String.IsNullOrEmpty(rschema))
						rtable = rschema + "." + rtable;
					fk = new DBForeignKey(pkc, rtable, fkc);
					table.ForeignKeys.Add(fk);
					fks[row.Values[0]] = fk;
				} else {
					fk.Columns.Add((string)row.Values[1]);
					fk.PrimaryKeyTableKeyColumns.Add((string)row.Values[2]);
				}
			}
		}
		public override void GetTableSchema(DBTable table, bool checkIndexes, bool checkForeignKeys) {
			GetColumns(table);
			GetPrimaryKey(table);
			if(checkIndexes)
				GetIndexes(table);
			if(checkForeignKeys)
				GetForeignKeys(table);
		}
		public override ICollection CollectTablesToCreate(ICollection tables) {
			Dictionary<string, bool> dbTables = new Dictionary<string, bool>();
			Dictionary<string, bool> dbSchemaTables = new Dictionary<string, bool>();
			foreach(SelectStatementResultRow row in GetDataForTables(tables, null, "select TABLE_NAME, TABLE_TYPE, TABLE_SCHEMA from INFORMATION_SCHEMA.TABLES where ({0}) and TABLE_TYPE in ('BASE TABLE', 'VIEW')").Rows) {
				if(row.Values[0] is DBNull) continue;
				string tableName = (string)row.Values[0];
				bool isView = (string)row.Values[1] == "VIEW";
				string tableSchemaName = (string)row.Values[2];
				if(tableSchemaName == ObjectsOwner) {
					dbTables[tableName] = isView;
				}
				dbSchemaTables.Add(string.Concat(tableSchemaName, ".", tableName), isView);
			}
			ArrayList list = new ArrayList();
			foreach(DBTable table in tables) {
				string tableName = ComposeSafeTableName(table.Name);
				string tableSchemaName = ComposeSafeSchemaName(table.Name);
				bool isView = false;
				if(!dbSchemaTables.TryGetValue(string.Concat(tableSchemaName, ".", tableName), out isView) && !dbTables.TryGetValue(tableName, out isView))
					list.Add(table);
				else
					table.IsView = isView;
			}
			return list;
		}
		string FormatOwnedDBObject(string schema, string objectName) {
			if(schema != String.Empty)
				return "\"" + schema + "\".\"" + objectName + "\"";
			if(string.IsNullOrEmpty(ObjectsOwner))
				return "\"" + objectName + "\"";
			else
				return "\"" + ObjectsOwner + "\".\"" + objectName + "\"";
		}
		public override string FormatTable(string schema, string tableName) {
			return FormatOwnedDBObject(schema, tableName);
		}
		public override string FormatTable(string schema, string tableName, string tableAlias) {
			return FormatOwnedDBObject(schema, tableName) + ' ' + tableAlias;
		}
		public override string FormatColumn(string columnName) {
			return MsSqlFormatterHelper.FormatColumn(columnName);
		}
		public override string FormatColumn(string columnName, string tableAlias) {
			return MsSqlFormatterHelper.FormatColumn(columnName, tableAlias);
		}
		public override string FormatSelect(string selectedPropertiesSql, string fromSql, string whereSql, string orderBySql, string groupBySql, string havingSql, int skipSelectedRecords, int topSelectedRecords) {
			if(skipSelectedRecords != 0)
				base.FormatSelect(selectedPropertiesSql, fromSql, whereSql, orderBySql, groupBySql, havingSql, skipSelectedRecords, topSelectedRecords);
			string modificatorsSql = string.Empty;
			if(topSelectedRecords != 0 && skipSelectedRecords == 0) {
				modificatorsSql = string.Format(CultureInfo.InvariantCulture, "top {0} ", topSelectedRecords);
			}
			string expandedWhereSql = whereSql == null ? null : ("\nwhere " + whereSql);
			string expandedOrderBySql = orderBySql != null ? "\norder by " + orderBySql : string.Empty;
			string expandedHavingSql = havingSql != null ? "\nhaving " + havingSql : string.Empty;
			string expandedGroupBySql = groupBySql != null ? "\ngroup by " + groupBySql : string.Empty;
			if(skipSelectedRecords == 0) {
				return string.Format(CultureInfo.InvariantCulture, "select {0}{1} from {2}{3}{4}{5}{6}", modificatorsSql, selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql);
			}
			if(is2012 && !string.IsNullOrEmpty(expandedOrderBySql)) {
				string fetchRowsSql = topSelectedRecords != 0 ? string.Format(CultureInfo.InvariantCulture, "\nfetch next {0} rows only", topSelectedRecords) : string.Empty;
				return string.Format(CultureInfo.InvariantCulture, "select {0} from {1}{2}{3}{4}{5}\noffset {6} rows{7}", selectedPropertiesSql, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedOrderBySql, skipSelectedRecords, fetchRowsSql);
			} else {
				string[] fields = SimpleSqlParser.GetColumns(selectedPropertiesSql);
				StringBuilder expandedSelectedProperties = SimpleSqlParser.GetExpandedProperties(fields, "resultSet");
				selectedPropertiesSql = string.Join(", ", fields);
				string baseFormat = "select {8} from(select {0}, row_number() over({1}) as 'rowNumber' from {4}{5}{6}{7})resultSet where resultSet.rowNumber > {2}";
				if(topSelectedRecords != 0) {
					baseFormat += " and resultSet.rowNumber <= {2} + {3}";
				}
				return string.Format(CultureInfo.InvariantCulture, baseFormat,
					selectedPropertiesSql, expandedOrderBySql, skipSelectedRecords, topSelectedRecords, fromSql, expandedWhereSql, expandedGroupBySql, expandedHavingSql, expandedSelectedProperties);
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("MSSqlConnectionProviderNativeSkipTakeSupported")]
#endif
		public override bool NativeSkipTakeSupported { get { return is2005; } }
#if !SL
	[DevExpressXpoLocalizedDescription("MSSqlConnectionProviderNativeOuterApplySupported")]
#endif
		public override bool NativeOuterApplySupported { get { return is2005; } }
		public override string FormatInsertDefaultValues(string tableName) {
			return MsSqlFormatterHelper.FormatInsertDefaultValues(tableName);
		}
		public override string FormatInsert(string tableName, string fields, string values) {
			return MsSqlFormatterHelper.FormatInsert(tableName, fields, values);
		}
		public override string FormatUpdate(string tableName, string sets, string whereClause) {
			return MsSqlFormatterHelper.FormatUpdate(tableName, sets, whereClause);
		}
		public override string FormatDelete(string tableName, string whereClause) {
			return MsSqlFormatterHelper.FormatDelete(tableName, whereClause);
		}
		public override string FormatFunction(FunctionOperatorType operatorType, params string[] operands) {
			string format = MsSqlFormatterHelper.FormatFunction(operatorType, new MsSqlFormatterHelper.MSSqlServerVersion(is2000, is2005, is2008, isAzure), operands);
			return format == null ? base.FormatFunction(operatorType, operands) : format;
		}
		public override string FormatFunction(ProcessParameter processParameter, FunctionOperatorType operatorType, params object[] operands) {
			string format = MsSqlFormatterHelper.FormatFunction(processParameter, operatorType, new MsSqlFormatterHelper.MSSqlServerVersion(is2000, is2005, is2008, isAzure), operands);
			return format == null ? base.FormatFunction(processParameter, operatorType, operands) : format;
		}
		public override string FormatBinary(BinaryOperatorType operatorType, string leftOperand, string rightOperand) {
			return MsSqlFormatterHelper.FormatBinary(operatorType, leftOperand, rightOperand);
		}
		protected override IDataParameter CreateParameter(IDbCommand command, object value, string name) {
			SqlParameter param = (SqlParameter)CreateParameter(command);
			param.Value = value;
			param.ParameterName = name;
			if(param.DbType == DbType.String && value is string)
				param.Size = GetStringSize(((string)value).Length);
			if(param.DbType == DbType.Binary && value is byte[])
				param.Size = GetBinarySize(((byte[])value).Length);
			return param;
		}
		int GetStringSize(int p) {
			if(p > MaximumStringSize)
				return p;
			return MaximumStringSize;
		}
		int GetBinarySize(int p) {
			if(p > MaximumBinarySize)
				return p;
			return MaximumBinarySize;
		}
		[ThreadStatic]		
		static string[] parameterNameCache;
		public override string GetParameterName(OperandValue parameter, int index, ref bool createParameter) {
			object value = parameter.Value;
			createParameter = false;
			if(parameter is ConstantValue && value != null) {
				switch(DXTypeExtensions.GetTypeCode(value.GetType())) {
					case TypeCode.Int32:
						return ((int)value).ToString(CultureInfo.InvariantCulture);
					case TypeCode.Boolean:
						return (bool)value ? "1" : "0";
					case TypeCode.String:
						return string.Concat("N'", ((string)value).Replace("'", "''"), "'");
				}
			}
			createParameter = true;
			int len = parameterNameCache == null ? 0 : parameterNameCache.Length;
			if(len <= index) {
				string[] newCache = new string[index + 10];
				if(parameterNameCache != null)
					Array.Copy(parameterNameCache, newCache, len);
				for(int i = len; i < newCache.Length; i++)
					newCache[i] = "@p" + i.ToString(CultureInfo.InvariantCulture);
				parameterNameCache = newCache;
			}
			return parameterNameCache[index];
		}
		public override string FormatConstraint(string constraintName) {
			return MsSqlFormatterHelper.FormatConstraint(constraintName);
		}
		protected override int GetSafeNameTableMaxLength() {
			return 128;
		}
		protected override string CreateForeignKeyTemplate {
			get {
				return GetIsAzure() ? base.CreateForeignKeyTemplate : base.CreateForeignKeyTemplate + (IsNotForReplication ? " NOT FOR REPLICATION" : string.Empty);
			}
		}
		public static void ClearDatabase(IDbCommand command) {
			command.CommandText = @"
declare @fk as sysname, @atbl as sysname
DECLARE fkcur CURSOR FORWARD_ONLY static for select CONSTRAINT_NAME, TABLE_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'FOREIGN KEY'
OPEN fkcur
FETCH NEXT FROM fkcur INTO @fk, @atbl
WHILE @@FETCH_STATUS = 0
BEGIN
  declare @alter as nvarchar(2048)
  set @alter = 'alter table ""' + @atbl + '"" drop constraint ""' + @fk + '""'
  exec sp_executesql @alter
  FETCH NEXT FROM fkcur INTO @fk, @atbl
END
close fkcur
DEALLOCATE fkcur

declare @dtbl as sysname
DECLARE tblcur CURSOR FORWARD_ONLY static for select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'BASE TABLE'
OPEN tblcur
FETCH NEXT FROM tblcur INTO @dtbl
WHILE @@FETCH_STATUS = 0
BEGIN
  declare @drop as nvarchar(2048)
  set @drop = 'drop table ""' + @dtbl + '""'
  exec sp_executesql @drop
  FETCH NEXT FROM tblcur INTO @dtbl
END
close tblcur
DEALLOCATE tblcur
";
			command.ExecuteNonQuery();
		}
		public static void EmptyAllTablesInDatabase(IDbCommand command) {
			command.CommandText = @"
declare @fk as sysname, @atbl as sysname, @prevFk as sysname
set @prevFk = ''
while 1=1
begin
  set @fk = null
  select top 1 @fk = CONSTRAINT_NAME, @atbl = TABLE_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'FOREIGN KEY' and CONSTRAINT_NAME > @prevFk order by CONSTRAINT_NAME
  if @fk is null
    break
  set @prevFk = @fk
  declare @alter as nvarchar(2048)
  set @alter = 'alter table ""' + @atbl + '"" nocheck constraint ""' + @fk + '""'
  exec sp_executesql @alter
end";
			command.ExecuteNonQuery();
			command.CommandText = @"
declare @dtbl as sysname, @prevTbl as sysname
set @prevTbl = ''
while 1=1
begin
  set @dtbl = null
  select top 1 @dtbl = TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'BASE TABLE' and TABLE_NAME > @prevTbl order by TABLE_NAME
  if @dtbl is null
    break
  set @prevTbl = @dtbl
  declare @drop as nvarchar(2048)
  set @drop = 'delete from ""' + @dtbl + '""'
  exec sp_executesql @drop
end
";
			command.ExecuteNonQuery();
			command.CommandText = @"
declare @fk as sysname, @atbl as sysname, @prevFk as sysname
set @prevFk = ''
while 1=1
begin
  set @fk = null
  select top 1 @fk = CONSTRAINT_NAME, @atbl = TABLE_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE = 'FOREIGN KEY' and CONSTRAINT_NAME > @prevFk order by CONSTRAINT_NAME
  if @fk is null
    break
  set @prevFk = @fk
  declare @alter as nvarchar(2048)
  set @alter = 'alter table ""' + @atbl + '"" check constraint ""' + @fk + '""'
  exec sp_executesql @alter
end";
			command.ExecuteNonQuery();
		}
		protected override void ProcessClearDatabase() {
			IDbCommand command = CreateCommand();
			ClearDatabase(command);
		}
		public override string[] GetStorageTablesList(bool includeViews) {
			if(is2005) {
				SelectStatementResult tables = SelectData(new Query(string.Format("select t.name, p.name from sys.objects t join sys.schemas p on p.schema_id = t.schema_id where (t.type ='u' {0}) and objectProperty(t.object_id, 'IsMSShipped') = 0", includeViews ? " or t.type ='v'" : "")));
				string[] result = new string[tables.Rows.Length];
				for(int i = 0; i < tables.Rows.Length; ++i) {
					result[i] = (string)tables.Rows[i].Values[0];
					string owner = (string)tables.Rows[i].Values[1];
					if(ObjectsOwner != owner && owner != null)
						result[i] = string.Concat(owner, ".", result[i]);
				}
				return result;
			} else {
				SelectStatementResult tables = SelectData(new Query(string.Format("select name from sysobjects where (type='U' {0}) and objectProperty(id, 'IsMSShipped') = 0", includeViews ? " or type='V'" : "")));
				string[] result = new string[tables.Rows.Length];
				for(int i = 0; i < tables.Rows.Length; ++i) {
					result[i] = (string)tables.Rows[i].Values[0];
				}
				return result;
			}
		}
		public string ObjectsOwner = "dbo";
		Dictionary<string, string> safeNames = new Dictionary<string, string>();
		protected override string GetSafeNameRoot(string originalName) {
			lock(safeNames) {
				string safeName;
				if(!safeNames.TryGetValue(originalName, out safeName)) {
					safeName = GetSafeNameMsSql(originalName);
					safeNames.Add(originalName, safeName);
				}
				return safeName;
			}
		}
		bool hasIdentityes;
		public override string GenerateStoredProcedures(DBTable table, out string dropLines) {
			List<string> dropList = new List<string>();
			StringBuilder result = new StringBuilder();
			hasIdentityes = false;
			GenerateView(table, result, dropList);
			GenerateInsertSP(table, result, dropList);
			GenerateUpdateSP(table, result, dropList);
			GenerateDeleteSP(table, result, dropList);
			GenerateInsteadOfInsertTrigger(table, result, dropList);
			GenerateInsteadOfUpdateTrigger(table, result, dropList);
			GenerateInsteadOfDeleteTrigger(table, result, dropList);
			if(dropList.Count > 0) {
				StringBuilder dropResult = new StringBuilder();
				for(int i = dropList.Count - 1; i >= 0; i--) {
					StringBuilderAppendLine(dropResult, dropList[i]);
					StringBuilderAppendLine(dropResult, "GO");
				}
				dropLines = dropResult.ToString();
			} else {
				dropLines = string.Empty;
			}
			return result.ToString();
		}
		void GenerateView(DBTable table, StringBuilder result, List<string> dropList) {
			StringBuilderAppendLine(result, string.Format("CREATE VIEW [{0}_xpoView] AS", table.Name));
			dropList.Add(string.Format("DROP VIEW [{0}_xpoView]", table.Name));
			StringBuilderAppendLine(result, "\tSELECT");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(!hasIdentityes) {
					hasIdentityes = table.Columns[i].IsIdentity;
				}
				string identityMagicAlias = table.Columns[i].IsIdentity ? " AS " + IdentityColumnMagicName : string.Empty;
				StringBuilderAppendLine(result, string.Format("\t\t[{0}]{2}{1}", table.Columns[i].Name, i < table.Columns.Count - 1 ? "," : string.Empty, identityMagicAlias));
			}
			StringBuilderAppendLine(result, string.Format("\tFROM [{0}]", table.Name));
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateInsertSP(DBTable table, StringBuilder result, List<string> dropList) {
			bool hasIdentityColumn = false;
			StringBuilderAppendLine(result, string.Format("CREATE PROCEDURE [sp_{0}_xpoView_insert]", table.Name));
			dropList.Add(string.Format("DROP PROCEDURE [sp_{0}_xpoView_insert]", table.Name));
			for(int i = 0; i < table.Columns.Count; i++) {
				string dbType = GetSqlCreateColumnType(table, table.Columns[i]);
				string name;
				string formatStr;
				bool isFK = false;
				for(int j = 0; j < table.ForeignKeys.Count; j++) {
					if(table.ForeignKeys[j].Columns.Contains(table.Columns[i].Name)) {
						isFK = true;
						break;
					}
				}
				if(table.Columns[i].IsIdentity) {
					hasIdentityColumn = true;
					name = IdentityColumnMagicName;
					formatStr = "\t@{0} {1}{3} OUT{2}";
				} else {
					name = table.Columns[i].Name;
					formatStr = "\t@{0} {1}{3}{2}";
				}
				StringBuilderAppendLine(result, string.Format(formatStr, name, dbType, i < table.Columns.Count - 1 ? "," : string.Empty, isFK ? " = null" : string.Empty));
			}
			StringBuilderAppendLine(result, "AS");
			StringBuilderAppendLine(result, "BEGIN");
			StringBuilderAppendLine(result, "\tBEGIN TRY");
			StringBuilderAppendLine(result, string.Format("\t\tINSERT INTO [{0}](", table.Name));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].IsIdentity) { continue; }
				StringBuilderAppendLine(result, string.Format("\t\t\t[{0}]{1}", table.Columns[i].Name, i < table.Columns.Count - 1 ? "," : string.Empty));
			}
			StringBuilderAppendLine(result, "\t\t)");
			StringBuilderAppendLine(result, "\t\tVALUES(");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(table.Columns[i].IsIdentity) { continue; }
				StringBuilderAppendLine(result, string.Format("\t\t\t@{0}{1}", table.Columns[i].Name, i < table.Columns.Count - 1 ? "," : string.Empty));
			}
			StringBuilderAppendLine(result, "\t\t);");
			if(hasIdentityColumn) {
				string entityVal = is2000 ? "SCOPE_IDENTITY()" : "@@Identity";
				StringBuilderAppendLine(result, string.Format("\t\tSET @{0} = {1};", IdentityColumnMagicName, entityVal));
			}
			StringBuilderAppendLine(result, "\tEND TRY");
			StringBuilderAppendLine(result, "\tBEGIN CATCH");
			StringBuilderAppendLine(result, "\t\tDECLARE @ErrorMessage NVARCHAR(4000);");
			StringBuilderAppendLine(result, "\t\tDECLARE @ErrorSeverity INT;");
			StringBuilderAppendLine(result, "\t\tDECLARE @ErrorState INT;");
			StringBuilderAppendLine(result, "\t\tSELECT @ErrorMessage = ERROR_MESSAGE(),");
			StringBuilderAppendLine(result, "\t\t\t@ErrorSeverity = ERROR_SEVERITY(),");
			StringBuilderAppendLine(result, "\t\t\t@ErrorState = ERROR_STATE();");
			StringBuilderAppendLine(result, "\t\tRAISERROR(");
			StringBuilderAppendLine(result, "\t\t\t@ErrorMessage,");
			StringBuilderAppendLine(result, "\t\t\t@ErrorSeverity,");
			StringBuilderAppendLine(result, "\t\t\t@ErrorState");
			StringBuilderAppendLine(result, "\t\t);");
			StringBuilderAppendLine(result, "\tEND CATCH");
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateUpdateSP(DBTable table, StringBuilder result, List<string> dropList) {
			StringBuilderAppendLine(result, string.Format("CREATE PROCEDURE [sp_{0}_xpoView_update]", table.Name));
			dropList.Add(string.Format("DROP PROCEDURE [sp_{0}_xpoView_update]", table.Name));
			AppendKeys(table, result);
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(i != 0 || table.PrimaryKey.Columns.Count > 0) { StringBuilderAppendLine(result, ","); }
				string dbType = GetSqlCreateColumnType(table, table.Columns[i]);
				StringBuilderAppendLine(result, string.Format("\t@old_{0} {1},", table.Columns[i].Name, dbType));
				result.Append(string.Format("\t@{0} {1}", table.Columns[i].Name, dbType));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "AS");
			bool hasColumns = false;
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				hasColumns = true;
			}
			if(!hasColumns) {
				StringBuilderAppendLine(result, "BEGIN");
				StringBuilderAppendLine(result, string.Format("\tRAISERROR('There are no columns to update in {0}_xpoView', 16, 1, null);", table.Name));
				StringBuilderAppendLine(result, "END");
			} else {
				StringBuilderAppendLine(result, string.Format("\tUPDATE [{0}] SET", table.Name));
				bool first = true;
				for(int i = 0; i < table.Columns.Count; i++) {
					if(IsKey(table, table.Columns[i].Name)) { continue; }
					if(first) { first = false; } else { StringBuilderAppendLine(result, ","); }
					result.Append(string.Format("\t\t[{0}]=@{0}", table.Columns[i].Name));
				}
				StringBuilderAppendLine(result);
				StringBuilderAppendLine(result, "\tWHERE");
				AppendWhere(table, result);
			}
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateDeleteSP(DBTable table, StringBuilder result, List<string> dropList) {
			StringBuilderAppendLine(result, string.Format("CREATE PROCEDURE [sp_{0}_xpoView_delete]", table.Name));
			dropList.Add(string.Format("DROP PROCEDURE [sp_{0}_xpoView_delete]", table.Name));
			AppendKeys(table, result);
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				if(i != 0 || table.PrimaryKey.Columns.Count > 0) { StringBuilderAppendLine(result, ","); }
				string dbType = GetSqlCreateColumnType(table, table.Columns[i]);
				result.Append(string.Format("\t@old_{0} {1}", table.Columns[i].Name, dbType));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "AS");
			StringBuilderAppendLine(result, string.Format("\tDELETE FROM [{0}] WHERE", table.Name));
			AppendWhere(table, result);
			StringBuilderAppendLine(result, "GO");
		}
		void GenerateInsteadOfInsertTrigger(DBTable table, StringBuilder result, List<string> dropList) {
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER [t_{0}_xpoView_insert]", table.Name));
			dropList.Add(string.Format("DROP TRIGGER [t_{0}_xpoView_insert]", table.Name));
			StringBuilderAppendLine(result, string.Format("ON [{0}_xpoView]", table.Name));
			StringBuilderAppendLine(result, "INSTEAD OF INSERT AS");
			if(hasIdentityes) {
				StringBuilderAppendLine(result, "BEGIN");
				StringBuilderAppendLine(result, string.Format("\tRAISERROR('Use sp_{0}_xpoView_insert instead', 16, 1, null);", table.Name));
				StringBuilderAppendLine(result, "END");
				StringBuilderAppendLine(result, "GO");
				return;
			}
			InitTrigger(table, result);
			StringBuilderAppendLine(result, "\t\tFROM inserted");
			StringBuilderAppendLine(result, "\tOPEN @cur");
			for(int i = 0; i < table.Columns.Count; i++) {
				StringBuilderAppendLine(result, string.Format("\tDECLARE @{0} {1}", table.Columns[i].Name, GetSqlCreateColumnType(table, table.Columns[i])));
			}
			StringBuilderAppendLine(result, "\tFETCH NEXT FROM @cur INTO");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				result.Append(string.Format("\t\t@{0}", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tWHILE(@@fetch_status <> -1)");
			StringBuilderAppendLine(result, "\tBEGIN");
			StringBuilderAppendLine(result, string.Format("\t\tEXEC [sp_{0}_xpoView_insert]", table.Name));
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				result.Append(string.Format("\t\t\t@{0}", table.Columns[i].Name));
			}
			FinalizeTrigger(table, result);
		}
		void GenerateInsteadOfUpdateTrigger(DBTable table, StringBuilder result, List<string> dropList) {
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER [t_{0}_xpoView_update]", table.Name));
			dropList.Add(string.Format("DROP TRIGGER [t_{0}_xpoView_update]", table.Name));
			StringBuilderAppendLine(result, string.Format("ON [{0}_xpoView]", table.Name));
			StringBuilderAppendLine(result, "INSTEAD OF UPDATE AS");
			InitTriggerCore(table, result);
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t\ti.[{0}]", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				StringBuilderAppendLine(result, string.Format("\t\t\td.[{0}] as [old_{0}],", table.Columns[i].Name));
				result.Append(string.Format("\t\t\ti.[{0}]", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\t\tFROM");
			StringBuilderAppendLine(result, "\t\t\tinserted i");
			StringBuilderAppendLine(result, "\t\t\tINNER JOIN");
			StringBuilderAppendLine(result, "\t\t\tdeleted d");
			StringBuilderAppendLine(result, "\t\t\tON");
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, " AND"); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t\t\ti.[{0}] = d.[{0}]", columnName));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tOPEN @cur");
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				string type = GetSqlCreateColumnType(table, GetDbColumnByName(table, table.PrimaryKey.Columns[i]));
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				StringBuilderAppendLine(result, string.Format("\tDECLARE @{0} {1}", columnName, type));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				string type = GetSqlCreateColumnType(table, table.Columns[i]);
				StringBuilderAppendLine(result, string.Format("\tDECLARE @old_{0} {1}", table.Columns[i].Name, type));
				StringBuilderAppendLine(result, string.Format("\tDECLARE @{0} {1}", table.Columns[i].Name, type));
			}
			StringBuilderAppendLine(result, "\tFETCH NEXT FROM @cur INTO");
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t@{0}", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				StringBuilderAppendLine(result, string.Format("\t\t@old_{0},", table.Columns[i].Name));
				result.Append(string.Format("\t\t@{0}", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tWHILE(@@fetch_status <> -1)");
			StringBuilderAppendLine(result, "\tBEGIN");
			StringBuilderAppendLine(result, string.Format("\t\tEXEC [sp_{0}_xpoView_update]", table.Name));
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t\t@{0}", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				StringBuilderAppendLine(result, string.Format("\t\t\t@old_{0},", table.Columns[i].Name));
				result.Append(string.Format("\t\t\t@{0}", table.Columns[i].Name));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\t\tFETCH NEXT FROM @cur INTO");
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t\t@{0}", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				StringBuilderAppendLine(result, string.Format("\t\t\t@old_{0},", table.Columns[i].Name));
				result.Append(string.Format("\t\t\t@{0}", table.Columns[i].Name));
			}
			FinalizeTriggerCore(result);
		}
		void GenerateInsteadOfDeleteTrigger(DBTable table, StringBuilder result, List<string> dropList) {
			StringBuilderAppendLine(result, string.Format("CREATE TRIGGER [t_{0}_xpoView_delete]", table.Name));
			dropList.Add(string.Format("DROP TRIGGER [t_{0}_xpoView_delete]", table.Name));
			StringBuilderAppendLine(result, string.Format("ON [{0}_xpoView]", table.Name));
			StringBuilderAppendLine(result, "INSTEAD OF DELETE AS");
			InitTrigger(table, result);
			StringBuilderAppendLine(result, "\t\tFROM deleted");
			StringBuilderAppendLine(result, "\tOPEN @cur");
			for(int i = 0; i < table.Columns.Count; i++) {
				string columnName = table.Columns[i].IsIdentity ? IdentityColumnMagicName : table.Columns[i].Name;
				StringBuilderAppendLine(result, string.Format("\tDECLARE @{0} {1}", columnName, GetSqlCreateColumnType(table, table.Columns[i])));
			}
			StringBuilderAppendLine(result, "\tFETCH NEXT FROM @cur INTO");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = table.Columns[i].IsIdentity ? IdentityColumnMagicName : table.Columns[i].Name;
				result.Append(string.Format("\t\t@{0}", columnName));
			}
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tWHILE(@@fetch_status <> -1)");
			StringBuilderAppendLine(result, "\tBEGIN");
			StringBuilderAppendLine(result, string.Format("\t\tEXEC [sp_{0}_xpoView_delete]", table.Name));
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = ColumnIsIdentity(table, table.PrimaryKey.Columns[i]) ? IdentityColumnMagicName : table.PrimaryKey.Columns[i];
				result.Append(string.Format("\t\t\t@{0}", columnName));
			}
			for(int i = 0; i < table.Columns.Count; i++) {
				if(IsKey(table, table.Columns[i].Name)) { continue; }
				StringBuilderAppendLine(result, ",");
				result.Append(string.Format("\t\t\t@{0}", table.Columns[i].Name));
			}
			FinalizeTrigger(table, result);
		}
		void InitTrigger(DBTable table, StringBuilder result) {
			InitTriggerCore(table, result);
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = table.Columns[i].IsIdentity ? IdentityColumnMagicName : table.Columns[i].Name;
				result.Append(string.Format("\t\t\t[{0}]", columnName));
			}
			StringBuilderAppendLine(result);
		}
		void InitTriggerCore(DBTable table, StringBuilder result) {
			StringBuilderAppendLine(result, "BEGIN");
			StringBuilderAppendLine(result, "\tDECLARE @cur CURSOR");
			StringBuilderAppendLine(result, "\tSET @cur = CURSOR FOR");
			StringBuilderAppendLine(result, "\t\tSELECT");
		}
		void FinalizeTrigger(DBTable table, StringBuilder result) {
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\t\tFETCH NEXT FROM @cur INTO");
			for(int i = 0; i < table.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				string columnName = table.Columns[i].IsIdentity ? IdentityColumnMagicName : table.Columns[i].Name;
				result.Append(string.Format("\t\t\t@{0}", columnName));
			}
			FinalizeTriggerCore(result);
		}
		void FinalizeTriggerCore(StringBuilder result) {
			StringBuilderAppendLine(result);
			StringBuilderAppendLine(result, "\tEND");
			StringBuilderAppendLine(result, "\tCLOSE @cur");
			StringBuilderAppendLine(result, "\tDEALLOCATE @cur");
			StringBuilderAppendLine(result, "END");
			StringBuilderAppendLine(result, "GO");
		}
		void AppendWhere(DBTable table, StringBuilder result) {
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, " AND"); }
				result.Append(string.Format("\t\t[{0}] = @{0}", table.PrimaryKey.Columns[i]));
			}
			StringBuilderAppendLine(result);
		}
		void AppendKeys(DBTable table, StringBuilder result) {
			for(int i = 0; i < table.PrimaryKey.Columns.Count; i++) {
				if(i != 0) { StringBuilderAppendLine(result, ","); }
				DBColumn keyColumn = GetDbColumnByName(table, table.PrimaryKey.Columns[i]);
				string dbType = GetSqlCreateColumnType(table, keyColumn);
				result.Append(string.Format("\t@{0} {1}", keyColumn.Name, dbType));
			}
		}
		public override DBStoredProcedure[] GetStoredProcedures() {
			List<DBStoredProcedure> result = new List<DBStoredProcedure>();
			List<int> ids = new List<int>();
			string query;
			IDbCommand cmd;
			if(is2005) {
				query = "SELECT obj.object_id, obj.name, p.name FROM sys.all_objects obj join sys.schemas p on p.schema_id = obj.schema_id WHERE obj.type = 'P' AND obj.is_ms_shipped = 0";					
				cmd = CreateCommand(new Query(query));
				using(IDataReader rdr = cmd.ExecuteReader()) {
					while(rdr.Read()) {
						if(rdr[1] == DBNull.Value || rdr[2] == DBNull.Value) continue;
						DBStoredProcedure proc = new DBStoredProcedure();
						string owner = (string)rdr[2];
						if(ObjectsOwner != owner && owner != null)
							proc.Name = string.Concat(owner, ".", (string)rdr[1]);
						else
							proc.Name = (string)rdr[1];
						result.Add(proc);
						ids.Add(Convert.ToInt32(rdr[0]));
					}
				}
			} else {
				query = "SELECT sp.id AS [ID], sp.name AS [Name] FROM dbo.sysobjects AS sp WHERE (sp.xtype = N'P') And OBJECTPROPERTY(sp.id, N'IsMSShipped') = 0";
				cmd = CreateCommand(new Query(query));
				using(IDataReader rdr = cmd.ExecuteReader()) {
					while(rdr.Read()) {
						if(rdr[1] == DBNull.Value) continue;
						DBStoredProcedure proc = new DBStoredProcedure();
						proc.Name = (string)rdr[1];
						result.Add(proc);
						ids.Add(Convert.ToInt32(rdr[0]));
					}
				}
			}
			StringBuilder sbArguments = new StringBuilder();
			for(int i = 0; i < ids.Count; i++) {
				query = string.Format(@"SELECT param.name AS [Name], ISNULL(baset.name, N'') AS [SystemType],
CAST(CASE WHEN baset.name IN (N'char', N'varchar', N'binary', N'varbinary', N'nchar', N'nvarchar') THEN param.prec ELSE param.length END AS int) AS [Length],
CAST(CASE param.isoutparam WHEN 1 THEN param.isoutparam WHEN 0 THEN CASE param.name WHEN '' THEN 1 ELSE 0 END END AS bit) AS [IsOutputParameter]
FROM syscolumns AS param LEFT OUTER JOIN systypes AS baset ON baset.xusertype = param.xtype and baset.xusertype = baset.xtype
WHERE param.id = {0} and param.number = 1", ids[i]);
				cmd = CreateCommand(new Query(query));
				using(IDataReader rdr = cmd.ExecuteReader()) {
					while(rdr.Read()) {
						DBStoredProcedureArgument arg = new DBStoredProcedureArgument();
						arg.Name = rdr[0].ToString();
						arg.Type = GetTypeFromString(rdr[1].ToString(), Convert.ToInt32(rdr[2]) / 2);
						arg.Direction = DBStoredProcedureArgumentDirection.In;
						if(Convert.ToInt32(rdr[3]) == 1) {
							arg.Direction = DBStoredProcedureArgumentDirection.Out;
						}
						result[i].Arguments.Add(arg);
					}
				}
				if(sbArguments.Length > 0) sbArguments.Remove(0, sbArguments.Length);				
				for(int j = 0; j < result[i].Arguments.Count; j++) {
					if(j != 0) { sbArguments.Append(", "); }
					sbArguments.Append("null");
				}
				query = string.Format("set fmtonly on; exec {0} {1}; set fmtonly off", FormatTable(ComposeSafeSchemaName(result[i].Name), ComposeSafeTableName(result[i].Name)), sbArguments);
				cmd = CreateCommand(new Query(query));
				try {
					using(IDataReader rdr = cmd.ExecuteReader()) {
						do {
							rdr.Read();
							if(rdr.FieldCount == 0) { continue; }
							DBStoredProcedureResultSet resultSet = new DBStoredProcedureResultSet();
							for(int j = 0; j < rdr.FieldCount; j++) {
								DBNameTypePair pair = new DBNameTypePair();
								pair.Name = rdr.GetName(j);
								if(string.IsNullOrEmpty(pair.Name)) pair.Name = "Column" + j.ToString(CultureInfo.InvariantCulture);
								pair.Type = DBColumn.GetColumnType(rdr.GetFieldType(j));
								resultSet.Columns.Add(pair);
							}
							result[i].ResultSets.Add(resultSet);
						} while(rdr.NextResult());
					}
				} catch(SqlException) { }
			}
			return result.ToArray();
		}
	}
}
#if !CF && !DXPORTABLE
namespace DevExpress.Xpo.DB.Helpers {
	public class MSSqlServer2005CacheRootProviderFactory: MSSqlProviderFactory {
		public override string ProviderKey {
			get {
				return MSSql2005SqlDependencyCacheRoot.XpoProviderTypeString_CacheRoot;
			}
		}
		public override bool MeanSchemaGeneration { get { return false; } }
	}
	public class MSSqlServer2005WithCacheProviderFactory: MSSqlProviderFactory {
		public override string ProviderKey {
			get {
				return MSSql2005SqlDependencyCacheRoot.XpoProviderTypeString_WithCache;
			}
		}
		public override bool MeanSchemaGeneration { get { return false; } }
	}
	public class MSSql2005SqlDependencyCacheRoot: ICachedDataStore, IDataStoreSchemaExplorer, ICommandChannel, IDisposable {
		public static string XpoProviderTypeString_CacheRoot = "MSSqlServer2005CacheRoot";
		public static string XpoProviderTypeString_WithCache = "MSSqlServer2005WithCache";
		static MSSql2005SqlDependencyCacheRoot() {
			DataStoreBase.RegisterDataStoreProvider(XpoProviderTypeString_CacheRoot, new DataStoreCreationFromStringDelegate(CreateProviderFromString_CacheRoot));
			DataStoreBase.RegisterDataStoreProvider(XpoProviderTypeString_WithCache, new DataStoreCreationFromStringDelegate(CreateProviderFromString_WithCache));
			DataStoreBase.RegisterFactory(new MSSqlServer2005CacheRootProviderFactory());
			DataStoreBase.RegisterFactory(new MSSqlServer2005WithCacheProviderFactory());
		}
		public static void Register() { }
		public static IDataStore CreateProviderFromString_WithCache(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			ICacheToCacheCommunicationCore c = CreateSqlDependencyCacheRoot(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
			DataCacheNode r = new DataCacheNode(c);
			r.MaxCacheLatency = TimeSpan.Zero;
			return r;
		}
		public static IDataStore CreateProviderFromString_CacheRoot(string connectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			ICacheToCacheCommunicationCore c = CreateSqlDependencyCacheRoot(connectionString, autoCreateOption, out objectsToDisposeOnDisconnect);
			DataCacheNode r = new DataCacheNodeLocal(c);
			r.MaxCacheLatency = TimeSpan.Zero;
			return r;
		}
		protected readonly ICacheToCacheCommunicationCore Root;
		protected readonly SqlConnection Connection;
		protected bool isDisposed;
		protected readonly string ConnectionString;
		protected readonly ISqlGeneratorFormatter SqlFormatter;
		protected MSSql2005SqlDependencyCacheRoot(ICacheToCacheCommunicationCore root, SqlConnection connection, string connectionString, ISqlGeneratorFormatter formatter) {
			this.Root = root;
			this.Connection = (SqlConnection)((ICloneable)connection).Clone();
			this.Connection.Open();
			this.ConnectionString = connectionString;
			this.SqlFormatter = formatter;
			SqlDependency.Start(this.ConnectionString);
		}
		public static ICacheToCacheCommunicationCore CreateSqlDependencyCacheRoot(IDataStore nonCachedProvider, SqlConnection connection, ISqlGeneratorFormatter formatter, out IDisposable[] objectsToDisposeOnDisconnect) {
			return CreateSqlDependencyCacheRoot(nonCachedProvider, connection, formatter, null, out objectsToDisposeOnDisconnect);
		}
		public static ICacheToCacheCommunicationCore CreateSqlDependencyCacheRoot(IDataStore nonCachedProvider, SqlConnection connection, string originalConnectionString, ISqlGeneratorFormatter formatter, out IDisposable[] objectsToDisposeOnDisconnect) {
			return CreateSqlDependencyCacheRoot(nonCachedProvider, connection, originalConnectionString, formatter, null, out objectsToDisposeOnDisconnect);
		}
		public static ICacheToCacheCommunicationCore CreateSqlDependencyCacheRoot(IDataStore nonCachedProvider, SqlConnection connection, ISqlGeneratorFormatter formatter, DataCacheConfiguration cacheConfiguration, out IDisposable[] objectsToDisposeOnDisconnect) {
			return CreateSqlDependencyCacheRoot(nonCachedProvider, connection, connection.ConnectionString, formatter, cacheConfiguration, out objectsToDisposeOnDisconnect);
		}
		public static ICacheToCacheCommunicationCore CreateSqlDependencyCacheRoot(IDataStore nonCachedProvider, SqlConnection connection, string originalConnectionString, ISqlGeneratorFormatter formatter, DataCacheConfiguration cacheConfiguration, out IDisposable[] objectsToDisposeOnDisconnect) {
			DataCacheRoot root = new DataCacheRoot(nonCachedProvider);
			root.Configure(cacheConfiguration);
			MSSql2005SqlDependencyCacheRoot result = new MSSql2005SqlDependencyCacheRoot(root, connection, originalConnectionString, formatter);
			objectsToDisposeOnDisconnect = new IDisposable[] { result };
			return result;
		}
		public static ICacheToCacheCommunicationCore CreateSqlDependencyCacheRoot(MSSqlConnectionProvider nonCachedProvider, out IDisposable[] objectsToDisposeOnDisconnect) {
			return CreateSqlDependencyCacheRoot(nonCachedProvider, (DataCacheConfiguration)null, out objectsToDisposeOnDisconnect);
		}
		public static ICacheToCacheCommunicationCore CreateSqlDependencyCacheRoot(MSSqlConnectionProvider nonCachedProvider, string originalConnectionString, out IDisposable[] objectsToDisposeOnDisconnect) {
			return CreateSqlDependencyCacheRoot(nonCachedProvider, originalConnectionString, null, out objectsToDisposeOnDisconnect);
		}
		public static ICacheToCacheCommunicationCore CreateSqlDependencyCacheRoot(MSSqlConnectionProvider nonCachedProvider, DataCacheConfiguration cacheConfiguration, out IDisposable[] objectsToDisposeOnDisconnect) {
			return CreateSqlDependencyCacheRoot(nonCachedProvider, (SqlConnection)nonCachedProvider.Connection, nonCachedProvider, cacheConfiguration, out objectsToDisposeOnDisconnect);
		}
		public static ICacheToCacheCommunicationCore CreateSqlDependencyCacheRoot(MSSqlConnectionProvider nonCachedProvider, string originalConnectionString, DataCacheConfiguration cacheConfiguration, out IDisposable[] objectsToDisposeOnDisconnect) {
			return CreateSqlDependencyCacheRoot(nonCachedProvider, (SqlConnection)nonCachedProvider.Connection, originalConnectionString, nonCachedProvider, cacheConfiguration, out objectsToDisposeOnDisconnect);
		}
		public static ICacheToCacheCommunicationCore CreateSqlDependencyCacheRoot(string sqlConnectionConnectionString, AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {
			return CreateSqlDependencyCacheRoot(sqlConnectionConnectionString, autoCreateOption, null, out objectsToDisposeOnDisconnect);
		}
		public static ICacheToCacheCommunicationCore CreateSqlDependencyCacheRoot(string sqlConnectionConnectionString, AutoCreateOption autoCreateOption, DataCacheConfiguration cacheConfiguration, out IDisposable[] objectsToDisposeOnDisconnect) {
			IDisposable[] providersDisposables;
			MSSqlConnectionProvider provider = (MSSqlConnectionProvider)MSSqlConnectionProvider.CreateProviderFromString(sqlConnectionConnectionString, autoCreateOption, out providersDisposables);
			IDisposable[] cacheRootDisposables;
			ICacheToCacheCommunicationCore result = CreateSqlDependencyCacheRoot(provider, sqlConnectionConnectionString, cacheConfiguration, out cacheRootDisposables);
			List<IDisposable> totalDisposables = new List<IDisposable>();
			totalDisposables.AddRange(providersDisposables);
			totalDisposables.AddRange(cacheRootDisposables);
			objectsToDisposeOnDisconnect = totalDisposables.ToArray();
			return result;
		}
		public DataCacheModificationResult ModifyData(DataCacheCookie cookie, ModificationStatement[] dmlStatements) {
			MarkModified(BaseStatement.GetTablesNames(dmlStatements));
			ProcessPossibleAging();
			return UpdateConfigIfNeeded(Root.ModifyData(cookie, dmlStatements));
		}
		public DataCacheResult NotifyDirtyTables(DataCacheCookie cookie, params string[] dirtyTablesNames) {
			MarkModified(dirtyTablesNames);
			ProcessPossibleAging();
			return UpdateConfigIfNeeded(Root.NotifyDirtyTables(cookie, dirtyTablesNames));
		}
		public DataCacheResult ProcessCookie(DataCacheCookie cookie) {
			ProcessPossibleAging();
			return UpdateConfigIfNeeded(Root.ProcessCookie(cookie));
		}
		public DataCacheSelectDataResult SelectData(DataCacheCookie cookie, SelectStatement[] selects) {
			ExtendAgeTrackers(selects);
			return UpdateConfigIfNeeded(Root.SelectData(cookie, selects));
		}
		public DataCacheUpdateSchemaResult UpdateSchema(DataCacheCookie cookie, DBTable[] tables, bool dontCreateIfFirstTableNotExist) {
			ProcessPossibleAging();
			return UpdateConfigIfNeeded(Root.UpdateSchema(cookie, tables, dontCreateIfFirstTableNotExist));
		}
		DBTable[] IDataStoreSchemaExplorer.GetStorageTables(params string[] tables) {
			return ((IDataStoreSchemaExplorer)Root).GetStorageTables(tables);
		}
		string[] IDataStoreSchemaExplorer.GetStorageTablesList(bool includeViews) {
			return ((IDataStoreSchemaExplorer)Root).GetStorageTablesList(includeViews);
		}
		class TableInfo {
			public readonly string TableName;
			public readonly Dictionary<string, object> TrackedColumns = new Dictionary<string, object>();
			public SqlDependency Dependency;
			public bool CreateDependencyInProgress;
			public TableInfo(string tableName) {
				this.TableName = tableName;
			}
		}
		DataCacheConfiguration _CacheConfiguration;
		Guid _CacheGuid;
		object CacheConfigLock = new object();
		DataCacheConfiguration CacheConfiguration {
			get {
				lock(CacheConfigLock) {
					if(_CacheConfiguration != null)
						return _CacheConfiguration;
					UpdateConfigIfNeeded(Root.ProcessCookie(DataCacheCookie.Empty));
					if(_CacheConfiguration == null)
						return DataCacheConfiguration.Empty;
					else
						return _CacheConfiguration;
				}
			}
		}
		T UpdateConfigIfNeeded<T>(T rootCallResult) where T: DataCacheResult {
			if(rootCallResult.CacheConfig != null) {
				lock(CacheConfigLock) {
					if(rootCallResult.Cookie.Guid != _CacheGuid) {
						_CacheGuid = rootCallResult.Cookie.Guid;
						_CacheConfiguration = rootCallResult.CacheConfig;
						InfosLock.AcquireWriterLock(Timeout.Infinite);
						try {
							Infos.Clear();
						} finally {
							InfosLock.ReleaseWriterLock();
						}
					}
				}
			}
			return rootCallResult;
		}
		Dictionary<string, TableInfo> Infos = new Dictionary<string, TableInfo>();
		ReaderWriterLock InfosLock = new ReaderWriterLock();
		protected void ProcessPossibleAging() {
			List<string> dirtyTables = new List<string>();
			InfosLock.AcquireReaderLock(Timeout.Infinite);
			try {
				foreach(TableInfo info in Infos.Values) {
					lock(info) {
						if(info.Dependency == null)
							continue;
						if(!info.Dependency.HasChanges)
							continue;
						dirtyTables.Add(info.TableName);
						info.Dependency = null;
					}
				}
			} finally {
				InfosLock.ReleaseReaderLock();
			}
			if(dirtyTables.Count > 0) {
				Root.NotifyDirtyTables(DataCacheCookie.Empty, dirtyTables.ToArray());
			}
		}
		void MarkModified(params string[] tableNames) {
			if(tableNames == null)
				return;
			for(int i = 0; i < tableNames.Length; ++i) {
				TableInfo info = TryGetInfo(tableNames[i]);
				if(info == null)
					continue;
				lock(info) {
					info.Dependency = null;
				}
			}
		}
		TableInfo TryGetInfo(string selectedTable) {
			InfosLock.AcquireReaderLock(Timeout.Infinite);
			try {
				TableInfo info;
				if(Infos.TryGetValue(selectedTable, out info))
					return info;
				else
					return null;
			} finally {
				InfosLock.ReleaseReaderLock();
			}
		}
		TableInfo GetInfo(string selectedTable) {
			TableInfo info = TryGetInfo(selectedTable);
			if(info != null)
				return info;
			InfosLock.AcquireWriterLock(Timeout.Infinite);
			try {
				if(!Infos.TryGetValue(selectedTable, out info)) {
					info = new TableInfo(selectedTable);
					Infos.Add(selectedTable, info);
				}
				return info;
			} finally {
				InfosLock.ReleaseWriterLock();
			}
		}
		void ReallyExtendAgeTracker(object data) {
			bool flagReset = false;
			string table = (string)data;
			TableInfo info = GetInfo(table);
			try {
				string alias = "Zzz";
				SelectStatement stmt = new SelectStatement();
				stmt.Alias = alias;
				stmt.Table = new DBTable() { Name = table };
				int trackedColumns;
				lock(info) {
					trackedColumns = info.TrackedColumns.Count;
					if(trackedColumns == 0)
						return;
					foreach(string field in info.TrackedColumns.Keys) {
						stmt.Operands.Add(new QueryOperand(field, alias));
					}
				}
				SelectSqlGenerator gena = new SelectSqlGenerator(SqlFormatter);
				Query query = gena.GenerateSql(stmt);
				SqlConnection conn;
				lock(Connection) {
					if(isDisposed)
						return;
					conn = (SqlConnection)((ICloneable)Connection).Clone();
				}
				try {
					conn.Open();
					using(SqlCommand command = conn.CreateCommand()) {
						command.CommandText = query.Sql;
						SqlDependency dependency = new SqlDependency(command);
						command.ExecuteNonQuery();
						PerformanceCounters.MSSql2005CacheRootDependencyEstablished.Increment();
						lock(info) {
							if(trackedColumns == info.TrackedColumns.Count) {
								info.Dependency = dependency;
							}
							flagReset = true;
							info.CreateDependencyInProgress = false;
						}
					}
				} catch {
				} finally {
					conn.Dispose();
				}
			} finally {
				if(!flagReset) {
					lock(info) {
						info.CreateDependencyInProgress = false;
					}
				}
				Root.NotifyDirtyTables(DataCacheCookie.Empty, table);
			}
		}
		void ExtendAgeTrackers(SelectStatement[] selects) {
			ProcessPossibleAging();
			Dictionary<string, List<IEnumerable<string>>> columns2cache = new Dictionary<string, List<IEnumerable<string>>>();
			foreach(SelectStatement ss in selects) {
				IEnumerable<KeyValuePair<string, IEnumerable<string>>> tables2columns = SelectStatement.GetTablesColumns(ss);
				bool badTableFound = false;
				foreach(KeyValuePair<string, IEnumerable<string>> ttt in tables2columns) {
					if(DataCacheBase.IsBadForCache(CacheConfiguration, ttt.Key)) {
						badTableFound = true;
						break;
					}
				}
				if(badTableFound)
					continue;
				foreach(KeyValuePair<string, IEnumerable<string>> ttt in tables2columns) {
					List<IEnumerable<string>> slot;
					if(!columns2cache.TryGetValue(ttt.Key, out slot)) {
						slot = new List<IEnumerable<string>>();
						columns2cache.Add(ttt.Key, slot);
						slot.Add(ttt.Value);
					}
				}
			}
			foreach(KeyValuePair<string, List<IEnumerable<string>>> tableWithColumns in columns2cache) {
				string selectedTable = tableWithColumns.Key;
				TableInfo info = GetInfo(selectedTable);
				bool needDirty = false;
				lock(info) {
					foreach(IEnumerable<string> columns in tableWithColumns.Value) {
						foreach(string column in columns) {
							if(info.TrackedColumns.ContainsKey(column))
								continue;
							if(info.Dependency != null) {
								needDirty = true;
								info.Dependency = null;
							}
							info.TrackedColumns.Add(column, column);
						}
					}
					if(info.Dependency != null)
						continue;
				}
				if(needDirty)
					Root.NotifyDirtyTables(DataCacheCookie.Empty, selectedTable);
				lock(info) {
					if(!info.CreateDependencyInProgress) {
						info.CreateDependencyInProgress = true;
						ThreadPool.QueueUserWorkItem(ReallyExtendAgeTracker, selectedTable);
					}
				}
			}
		}
		void IDisposable.Dispose() {
			lock(this.Connection) {
				if(!isDisposed) {
					isDisposed = true;
					SqlDependency.Stop(this.ConnectionString);
					this.Connection.Dispose();
				}
			}
		}
		bool isAutoCreateOptionCached = false;
		AutoCreateOption _AutoCreateOption = AutoCreateOption.None;
#if !SL
	[DevExpressXpoLocalizedDescription("MSSql2005SqlDependencyCacheRootAutoCreateOption")]
#endif
		public AutoCreateOption AutoCreateOption {
			get {
				if(!isAutoCreateOptionCached) {
					isAutoCreateOptionCached = true;
					try {
						_AutoCreateOption = ((IDataStore)Root).AutoCreateOption;
					} catch { }
				}
				return _AutoCreateOption;
			}
		}
		public ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			throw new NotSupportedException();
		}
		public SelectedData SelectData(params SelectStatement[] selects) {
			throw new NotSupportedException();
		}
		public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			throw new NotSupportedException();
		}
		public virtual object Do(string command, object args) {
			ICommandChannel commandChannel = Root as ICommandChannel;
			if(commandChannel == null) {
				if(Root == null) {
					return new OperationResult<object>(ServiceException.NotSupported, string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
				} else {
					return new OperationResult<object>(ServiceException.NotSupported, string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, Root.GetType().FullName));
				}
			}
			return commandChannel.Do(command, args);
		}
	}
#if !SL && !CF && !DXPORTABLE
	public class MSSqlLocalDBApi : IDisposable {
		const string installedVersionsKey = @"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions";
		static string apiDllPath = string.Empty;
		IntPtr handle;
		public MSSqlLocalDBApi() {
			if(string.IsNullOrEmpty(apiDllPath)) {
				using(RegistryKey ivKey = Registry.LocalMachine.OpenSubKey(installedVersionsKey, false)) {
					if(ivKey != null) {
						string[] versions = ivKey.GetSubKeyNames();
						if(versions.Length != 0) {
							if(versions.Length > 1) Array.Sort(versions);
							using(RegistryKey versionKey = ivKey.OpenSubKey(versions[versions.Length - 1], false)) {
								if(versionKey != null) {
									string fileName = versionKey.GetValue("InstanceAPIPath") as string;
									if(TryLoadLibrary(fileName)) {
										apiDllPath = fileName;
										return;
									}
								}
							}
						}
					}
				}
			} else
				if(TryLoadLibrary(apiDllPath))
					return;
			throw new MSSqlLocalDBApiException("Can't find LocalDBInstance.dll location.");
		}
		public MSSqlLocalDBApi(string fileName) {
			if(!TryLoadLibrary(fileName))
				throw new MSSqlLocalDBApiException("Can't find LocalDBInstance.dll location.");			
		}
		[SecuritySafeCritical]
		bool TryLoadLibrary(string fileName) {
			if(string.IsNullOrEmpty(fileName) || !File.Exists(fileName)) return false;
			handle = LoadLibrary(fileName);
			if(handle == IntPtr.Zero) {
				int errorCode = Marshal.GetLastWin32Error();
				throw new MSSqlLocalDBApiException(string.Format("Failed to load LocalDBInstance.dll (ErrorCode: {0})", errorCode), errorCode);
			}
			PrepareDelegates();
			return true;
		}
		LocalDBGetInstanceInfoDelegate getInstanceInfo;
		LocalDBGetInstancesDelegate getInstances;
		LocalDBGetVersionInfoDelegate getVersionInfo;
		LocalDBGetVersionsDelegate getVersions;
		LocalDBCreateInstanceDelegate createInstance;
		LocalDBDeleteInstanceDelegate deleteInstance;
		LocalDBStartInstanceDelegate startInstance;
		LocalDBStopInstanceDelegate stopInstance;
		LocalDBTracingDelegate startTracing;
		LocalDBTracingDelegate stopTracing;
		LocalDBShareInstanceDelegate shareInstance;
		LocalDBUnshareInstanceDelegate unshareInstance;
		LocalDBFormatMessageDelegate formatMessage;
		void PrepareDelegates() {
			if(handle == IntPtr.Zero) throw new InvalidOperationException();
			getInstanceInfo = GetDelegate<LocalDBGetInstanceInfoDelegate>("LocalDBGetInstanceInfo");
			getInstances = GetDelegate<LocalDBGetInstancesDelegate>("LocalDBGetInstances");
			getVersionInfo = GetDelegate<LocalDBGetVersionInfoDelegate>("LocalDBGetVersionInfo");
			getVersions = GetDelegate<LocalDBGetVersionsDelegate>("LocalDBGetVersions");
			createInstance = GetDelegate<LocalDBCreateInstanceDelegate>("LocalDBCreateInstance");
			deleteInstance = GetDelegate<LocalDBDeleteInstanceDelegate>("LocalDBDeleteInstance");
			startInstance = GetDelegate<LocalDBStartInstanceDelegate>("LocalDBStartInstance");
			stopInstance = GetDelegate<LocalDBStopInstanceDelegate>("LocalDBStopInstance");
			startTracing = GetDelegate<LocalDBTracingDelegate>("LocalDBStartTracing");
			stopTracing = GetDelegate<LocalDBTracingDelegate>("LocalDBStopTracing");
			formatMessage = GetDelegate<LocalDBFormatMessageDelegate>("LocalDBFormatMessage");
			shareInstance = GetDelegate<LocalDBShareInstanceDelegate>("LocalDBShareInstance");
			unshareInstance = GetDelegate<LocalDBUnshareInstanceDelegate>("LocalDBUnshareInstance");
		}
		[SecuritySafeCritical]
		T GetDelegate<T>(string procName) where T : class {
			CheckHandler();
			IntPtr pAddress = GetProcAddress(handle, procName);
			if(pAddress == IntPtr.Zero) throw new InvalidOperationException();
			return Marshal.GetDelegateForFunctionPointer(pAddress, typeof(T)) as T;			
		}
		void RaiseLocalDBException(int hResult) {
			if(hResult != 0) {
				int messageSize = 4096;
				StringBuilder message = new StringBuilder(messageSize);
				int hr = formatMessage(hResult, 1, CultureInfo.CurrentCulture.LCID & 0xFFFF, message, ref messageSize);
				if(hr != 0) throw new MSSqlLocalDBApiException(string.Format("LocalDBFormatMessage Error - ErrorCode: 0x{0:X}", hr), hr);
				throw new MSSqlLocalDBApiException(string.Format("LocalDB Error: {0}", message));
			}
		}
		[SecuritySafeCritical]
		public MSSqlLocalDBVersionInfo GetVersionInfo(string versionName) {
			CheckHandler();
			MSSqlLocalDBVersionInfo result = new MSSqlLocalDBVersionInfo();
			int res = getVersionInfo(versionName, ref result, Marshal.SizeOf(typeof(MSSqlLocalDBVersionInfo)));
			RaiseLocalDBException(res);
			return result;
		}
		[SecuritySafeCritical]
		public string[] GetVersions() {
			CheckHandler();
			MSSqlLocalDBVersionName[] versions = new MSSqlLocalDBVersionName[256];
			int versionCount = versions.Length;
			int res = getVersions(versions, ref versionCount);
			RaiseLocalDBException(res);
			return versions.Take(versionCount).Select(i => i.Value).ToArray();
		}
		[SecuritySafeCritical]
		public MSSqlLocalDBInstanceInfo GetInstanceInfo(string instanceName) {
			CheckHandler();
			MSSqlLocalDBInstanceInfo result = new MSSqlLocalDBInstanceInfo();
			int res = getInstanceInfo(instanceName, ref result, Marshal.SizeOf(typeof(MSSqlLocalDBInstanceInfo)));
			RaiseLocalDBException(res);
			return result;
		}
		[SecuritySafeCritical]
		public string[] GetInstances() {
			CheckHandler();
			MSSqlLocalDBInstanceName[] instances = new MSSqlLocalDBInstanceName[256];
			int instanceCount = instances.Length;
			int res = getInstances(instances, ref instanceCount);
			RaiseLocalDBException(res);
			return instances.Take(instanceCount).Select(i => i.Value).ToArray();
		}
		[SecuritySafeCritical]
		public void CreateInstance(string versionName, string instanceName) {
			CheckHandler();
			int res = createInstance(versionName, instanceName, 0);
			RaiseLocalDBException(res);
		}
		[SecuritySafeCritical]
		public void DeleteInstance(string instanceName) {
			CheckHandler();
			int res = deleteInstance(instanceName, 0);
			RaiseLocalDBException(res);
		}
		[SecuritySafeCritical]
		public void StartInstance(string instanceName, out string connectionString) {
			connectionString = null;
			CheckHandler();
			int connectionStringLength = 0;
			int res = startInstance(instanceName, 0, null, ref connectionStringLength);
			RaiseLocalDBException(res);
			StringBuilder connectionSB = new StringBuilder(connectionStringLength);
			res = startInstance(instanceName, 0, connectionSB, ref connectionStringLength);
			RaiseLocalDBException(res);
			connectionString = connectionSB.ToString();
		}
		[SecuritySafeCritical]
		public void StopInstance(string instanceName, MSSqlLocalDBShutdownFlags shutDownflags, long timeout) {
			CheckHandler();
			int res = stopInstance(instanceName, shutDownflags, timeout);
			RaiseLocalDBException(res);
		}
		[SecuritySafeCritical]
		public void StartTracing() {
			CheckHandler();
			int res = startTracing();
			RaiseLocalDBException(res);
		}
		[SecuritySafeCritical]
		public void StopTracing() {
			CheckHandler();
			int res = stopTracing();
			RaiseLocalDBException(res);
		}
		[SecuritySafeCritical]
		public void ShareInstance(SecurityIdentifier sid, string instancePrivateName, string instanceSharedName) {
			CheckHandler();
			IntPtr psid = Marshal.AllocHGlobal(sid.BinaryLength);
			try {
				byte[] psidData = new byte[sid.BinaryLength];
				sid.GetBinaryForm(psidData, 0);
				Marshal.Copy(psidData, 0, psid, psidData.Length);
				int res = shareInstance(psid, instancePrivateName, instanceSharedName, 0);
				RaiseLocalDBException(res);
			} finally {
				Marshal.FreeHGlobal(psid);
			}
		}
		[SecuritySafeCritical]
		public void UnshareInstance(string instanceSharedName) {
			CheckHandler();
			int res = unshareInstance(instanceSharedName, 0);
			RaiseLocalDBException(res);
		}
		[SecuritySafeCritical]
		public void CheckHandler() {
			if(handle == IntPtr.Zero) throw new ObjectDisposedException(this.ToString());
		}
		public void Dispose() {
			Dispose(true);
		}
		[SecuritySafeCritical]
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				GC.SuppressFinalize(this);
			if(handle != IntPtr.Zero) {
				FreeLibrary(handle);
				handle = IntPtr.Zero;
			}
		}
		~MSSqlLocalDBApi() {
			Dispose(false);
		}				
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBGetInstanceInfoDelegate([MarshalAs(UnmanagedType.LPWStr)]string instanceName,[MarshalAs(UnmanagedType.Struct)] ref MSSqlLocalDBInstanceInfo instanceInfo, int instanceInfoSize);
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBGetInstancesDelegate([MarshalAs(UnmanagedType.LPArray)] [Out] MSSqlLocalDBInstanceName[] instances, [In, Out] ref int instanceCount);
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBGetVersionInfoDelegate([MarshalAs(UnmanagedType.LPWStr)]string versionName, [MarshalAs(UnmanagedType.Struct)] ref MSSqlLocalDBVersionInfo versionInfo, int versionInfoSize);
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBGetVersionsDelegate([MarshalAs(UnmanagedType.LPArray)] [Out] MSSqlLocalDBVersionName[] versions, [In, Out] ref int versionCount);
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBCreateInstanceDelegate([MarshalAs(UnmanagedType.LPWStr)]string versionName, [MarshalAs(UnmanagedType.LPWStr)]string instanceName, [MarshalAs(UnmanagedType.U4)]int flags);
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBDeleteInstanceDelegate([MarshalAs(UnmanagedType.LPWStr)]string instanceName, [MarshalAs(UnmanagedType.U4)]int flags);
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBStartInstanceDelegate([MarshalAs(UnmanagedType.LPWStr)]string instanceName, [MarshalAs(UnmanagedType.U4)]int flags, [MarshalAs(UnmanagedType.LPWStr), Out]StringBuilder connectionString, [In, Out] ref int connectionStringLength);
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBStopInstanceDelegate([MarshalAs(UnmanagedType.LPWStr)]string instanceName, [MarshalAs(UnmanagedType.U4)]MSSqlLocalDBShutdownFlags flags, [MarshalAs(UnmanagedType.U8)] long timeout);
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBTracingDelegate();
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBFormatMessageDelegate(int hResult, [MarshalAs(UnmanagedType.U4)] int flags, [MarshalAs(UnmanagedType.U4)] int languageId, [MarshalAs(UnmanagedType.LPWStr), Out]StringBuilder message, [In, Out] ref int messageLength);
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBShareInstanceDelegate(IntPtr psid, [MarshalAs(UnmanagedType.LPWStr)]string instancePrivateName, [MarshalAs(UnmanagedType.LPWStr)]string instanceSharedNamem, [MarshalAs(UnmanagedType.U4)]int flags);
		[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
		delegate int LocalDBUnshareInstanceDelegate([MarshalAs(UnmanagedType.LPWStr)]string instanceSharedName, [MarshalAs(UnmanagedType.U4)]int flags);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr LoadLibrary(string libname);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern bool FreeLibrary(IntPtr hModule);
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
		public const int MAX_LOCALDB_INSTANCE_NAME_LENGTH = 128;
		public const int LOCALDB_MAX_SQLCONNECTION_BUFFER_SIZE = 260;
		public const int MAX_STRING_SID_LENGTH = 186;
		public const int MAX_LOCALDB_VERSION_LENGTH = 43;
	}
	[Flags]
	public enum MSSqlLocalDBShutdownFlags {
		None = 0x0000,
		KillProcess = 0x0001,
		WithNoWait = 0x0002
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	struct MSSqlLocalDBInstanceName {
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MSSqlLocalDBApi.MAX_LOCALDB_INSTANCE_NAME_LENGTH + 1)]
		public string Value;
	}
	[StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
	public struct MSSqlLocalDBInstanceInfo {
		[MarshalAs(UnmanagedType.U4)]
		public int LocalDBInstanceInfoSize;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=MSSqlLocalDBApi.MAX_LOCALDB_INSTANCE_NAME_LENGTH + 1)]
		public string InstanceName;
		[MarshalAs(UnmanagedType.Bool)]
		public bool Exists;
		[MarshalAs(UnmanagedType.Bool)]
		public bool ConfigurationCorrupted;
		[MarshalAs(UnmanagedType.Bool)]
		public bool IsRunning;
		[MarshalAs(UnmanagedType.U4)]
		public int Major;
		[MarshalAs(UnmanagedType.U4)]
		public int Minor;
		[MarshalAs(UnmanagedType.U4)]
		public int Build;
		[MarshalAs(UnmanagedType.U4)]
		public int Revision;
		public System.Runtime.InteropServices.ComTypes.FILETIME LastStartUTC;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MSSqlLocalDBApi.LOCALDB_MAX_SQLCONNECTION_BUFFER_SIZE)]
		public string Connection;
		[MarshalAs(UnmanagedType.Bool)]
		public bool IsShared;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MSSqlLocalDBApi.MAX_LOCALDB_INSTANCE_NAME_LENGTH + 1)]
		public string SharedInstanceName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MSSqlLocalDBApi.MAX_STRING_SID_LENGTH + 1)]
		public string OwnerSID;
		[MarshalAs(UnmanagedType.Bool)]
		public bool IsAutomatic;
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	struct MSSqlLocalDBVersionName {
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MSSqlLocalDBApi.MAX_LOCALDB_VERSION_LENGTH + 1)]
		public string Value;
	}
	[StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
	public struct MSSqlLocalDBVersionInfo {
		[MarshalAs(UnmanagedType.U4)]
		public int LocalDBVersionInfoSize;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MSSqlLocalDBApi.MAX_LOCALDB_VERSION_LENGTH + 1)]
		public string Version;
		[MarshalAs(UnmanagedType.Bool)]
		public bool Exists;
		[MarshalAs(UnmanagedType.U4)]
		public int Major;
		[MarshalAs(UnmanagedType.U4)]
		public int Minor;
		[MarshalAs(UnmanagedType.U4)]
		public int Build;
		[MarshalAs(UnmanagedType.U4)]
		public int Revision;
	};
#endif
}
#endif
