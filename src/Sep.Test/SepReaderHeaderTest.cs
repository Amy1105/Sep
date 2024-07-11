﻿using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace nietras.SeparatedValues.Test;

[TestClass]
public class SepReaderHeaderTest
{
    [TestMethod]
    public void SepReaderHeaderTest_Empty()
    {
        // If no line read i.e. line == null
        var header = SepReaderHeader.Empty;

        Assert.AreEqual(true, header.IsEmpty);
        Assert.AreEqual(0, header.ColNames.Count);
        Assert.AreEqual(string.Empty, header.ToString());
    }

    [TestMethod]
    public void SepReaderHeaderTest_EmptyString()
    {
        var header = SepReaderHeader.Parse(Sep.Default, string.Empty);

        Assert.AreEqual(false, header.IsEmpty);
        Assert.AreEqual(1, header.ColNames.Count);
        Assert.AreEqual(0, header.IndexOf(""));
        Assert.AreEqual(string.Empty, header.ToString());
    }

    [TestMethod]
    public void SepReaderHeaderTest_NotEmpty()
    {
        var header = SepReaderHeader.Parse(Sep.New(';'), "A;B;C");

        Assert.AreEqual(false, header.IsEmpty);
        Assert.AreEqual(3, header.ColNames.Count);
        AreEqual(["A", "B", "C"], header.ColNames);

        Assert.AreEqual(1, header.IndexOf("B"));
        AreEqual([2, 0, 1], header.IndicesOf("C", "A", "B"));
        AreEqual([1, 2, 0], header.IndicesOf(new[] { "B", "C", "A" }.AsSpan()));
        AreEqual([0, 2], header.IndicesOf((ReadOnlySpan<string>)["A", "C"]));
        AreEqual([2, 0], header.IndicesOf((IReadOnlyList<string>)["C", "A"]));

        var tryTrue = header.TryIndexOf("B", out var tryTrueIndex);
        Assert.IsTrue(tryTrue);
        Assert.AreEqual(1, tryTrueIndex);
        var tryFalse = header.TryIndexOf("XX", out var tryFalseIndex);
        Assert.IsFalse(tryFalse);
        Assert.AreEqual(0, tryFalseIndex);

        var actualIndices = new int[2];
        header.IndicesOf((ReadOnlySpan<string>)["A", "C"], actualIndices);
        AreEqual([0, 2], actualIndices);

        Assert.AreEqual("A;B;C", header.ToString());
    }

    [TestMethod]
    public void SepReaderHeaderTest_NamesStartingWith()
    {
        var header = SepReaderHeader.Parse(Sep.New(';'), "A;B;C;GT_0;RE_0;GT_1;RE_1");
        AreEqual(new[] { "GT_0", "GT_1" }, header.NamesStartingWith("GT_"));
    }

    [TestMethod]
    public void SepReaderHeaderTest_IndicesOf_LengthsNotSame_Throws()
    {
        var header = SepReaderHeader.Parse(Sep.New(';'), "A;B;C");

        var e = Assert.ThrowsException<ArgumentException>(() =>
        {
            var colNames = new[] { "A", "B" };
            Span<int> colIndices = stackalloc int[1];
            header.IndicesOf(colNames, colIndices);
        });
        Assert.AreEqual("'colIndices':1 must have same length as 'colNames':2", e.Message);
    }

    static void AreEqual<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual) =>
        CollectionAssert.AreEqual((ICollection)expected, (ICollection)actual);
}
