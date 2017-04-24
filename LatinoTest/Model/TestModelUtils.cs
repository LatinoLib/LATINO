using System;
using System.Collections.Generic;
using Latino.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Latino;

namespace LatinoTest.Model
{
    [TestClass]
    public class TestModelUtils
    {
        [TestMethod]
        public void TestCaluculateEuclideanDistance()
        {
            // matching indexes
            var v1 = new SparseVector<double> { [0] = 1, [1] = 1, [2] = 1, [3] = 1 };
            var v2 = new SparseVector<double> { [0] = 1, [1] = 1, [2] = 1, [3] = 1 };
            double d = EuclideanDistance.Instance.GetDistance(v1, v2);
            Assert.AreEqual(0, d);

            // leading indexes
            v1 = new SparseVector<double> { [0] = 1, [1] = 1, [2] = 1, [3] = 1 };
            v2 = new SparseVector<double> {                   [2] = 1, [3] = 1 };
            d = EuclideanDistance.Instance.GetDistance(v1, v2);
            Assert.AreEqual(Math.Sqrt(2), d);

            // trailing indexes
            v1 = new SparseVector<double> { [0] = 1, [1] = 1, [2] = 1, [3] = 1 };
            v2 = new SparseVector<double> { [0] = 1, [1] = 1                   };
            d = EuclideanDistance.Instance.GetDistance(v1, v2);
            Assert.AreEqual(Math.Sqrt(2), d);

            // empty
            v1 = new SparseVector<double> { [0] = 1, [1] = 1, [2] = 1, [3] = 1 };
            v2 = new SparseVector<double> {                                    };
            d = EuclideanDistance.Instance.GetDistance(v1, v2);
            Assert.AreEqual(2, d);

            // leading & trailing
            v1 = new SparseVector<double> { [0] = 2, [1] = 1, [2] = 1, [3] = 2 };
            v2 = new SparseVector<double> {          [1] = 1, [2] = 1          };
            d = EuclideanDistance.Instance.GetDistance(v1, v2);
            Assert.AreEqual(Math.Sqrt(8), d);

            // mixed
            v1 = new SparseVector<double> { [0] = 1, [1] = 1, [2] = 1, [3] = 1, [5] = 1, [6] = 1, [7] = 1, [8] = 1 };
            v2 = new SparseVector<double> {          [1] = 1,          [3] = 1, [5] = 1, [6] = 1, [7] = 1          };
            d = EuclideanDistance.Instance.GetDistance(v1, v2);
            Assert.AreEqual(Math.Sqrt(3), d);

            // mixed cont.
            v1 = new SparseVector<double> { [0] = 2, [1] = 2, [2] = 2, [3] = 2, [5] = 2, [6] = 2, [7] = 2, [8] = 2 };
            v2 = new SparseVector<double> {          [1] = 1,          [3] = 1, [5] = 1, [6] = 1, [7] = 1          };
            d = EuclideanDistance.Instance.GetDistance(v1, v2);
            Assert.AreEqual(Math.Sqrt(17), d);
        }
    }
}
