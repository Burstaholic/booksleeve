﻿using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class Transactions // http://redis.io/commands#transactions
    {
        [Test]
        public void TestBasicMultiExec()
        {
            using (var conn = Config.GetUnsecuredConnection())
            {
                conn.Remove(1, "tran");
                conn.Remove(2, "tran");

                using (var tran = conn.Multi())
                {
                    var s1 = tran.Set(1, "tran", "abc");
                    var s2 = tran.Set(2, "tran", "def");
                    var g1 = tran.Get(1, "tran");
                    var g2 = tran.Get(2, "tran");

                    var outsideTran = conn.GetString(1, "tran");

                    var exec = tran.Execute();

                    Assert.IsNull(conn.Wait(outsideTran));
                    Assert.AreEqual("abc", conn.Wait(g1));
                    Assert.AreEqual("def", conn.Wait(g2));
                    conn.Wait(s1);
                    conn.Wait(s2);
                    conn.Wait(exec);
                }

            }
        }
    }
}

