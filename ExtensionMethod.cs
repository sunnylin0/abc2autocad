using System;
using System.Collections.Generic;
using System.Text;
using AngryArrays;
class tttt
{
    void sadf()
    {
        List<int> ll = new List<int> { 3, 4, 5, 1, 23, 4, 32 };
        int aa;
        aa = ll.Pop();


        int[] nn = { 3, 4, 5, 23, 3, aa };
        int[] bb = nn.Copy1();
        int[] cc = bb;
        
    }
}


public static class ExtensionMethod
{
    /// <summary>
    /// Removes the first element from the list and returns it,
    /// or null if the list is empty.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Shift<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
            return default(T);

        T result = list[0];
        list.RemoveAt(0);

        return result;
    }

    /// <summary>
    /// Removes the last element from the list and returns it,
    /// or null if the list is empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T Pop<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
            return default(T);

        T result = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);

        return result;
    }
    public static T[] Copy1<T>(this T[] array)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        return (T[])array.Clone();
    }

    //public static List<T> ShiftLeft<T>(this List<T> list, int shiftBy)
    //{
    //    if (list.Count <= shiftBy)
    //    {
    //        return list;
    //    }

    //    var result = list.GetRange(shiftBy, list.Count - shiftBy);
    //    result.AddRange(list.GetRange(0, shiftBy));
    //    return result;
    //}

    //public static List<T> ShiftRight<T>(this List<T> list, int shiftBy)
    //{
    //    if (list.Count <= shiftBy)
    //    {
    //        return list;
    //    }

    //    var result = list.GetRange(list.Count - shiftBy, shiftBy);
    //    result.AddRange(list.GetRange(0, list.Count - shiftBy));
    //    return result;
    //}
}
#region Copyright (c) 2015 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

//SPDX-License-Identifier: Unlicense

static partial class AngryArray
{
    public static T[] Push<T>(this T[] array, T item)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        var combined = new T[array.Length + 1];
        array.CopyTo(combined, 0);
        combined[combined.Length - 1] = item;
        return combined;
    }

    public static T[] Push<T>(this T[] array, params T[] items)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        var length = array.Length + (items?.Length ?? 0);
        if (length == 0)
            return array;
        var combined = new T[length];
        array.CopyTo(combined, 0);
        items?.CopyTo(combined, array.Length);
        return combined;
    }

    public static T[] Unshift<T>(this T[] array, T item)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        var combined = new T[array.Length + 1];
        array.CopyTo(combined, 1);
        combined[0] = item;
        return combined;
    }

    public static T[] Unshift<T>(this T[] array, params T[] items)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        var length2 = (items?.Length ?? 0);
        var length = array.Length + length2;
        if (length == 0)
            return array;
        var combined = new T[length];
        array.CopyTo(combined, length2);
        items?.CopyTo(combined, 0);
        return combined;
    }

    public static T[] Splice<T>(this T[] array, int index) =>
        Splice(array, index, array.Length);

    public static T[] Splice<T>(this T[] array, int index, int count) =>
        Splice(array, index, count, true, (s, _) => s);

    public static TResult Splice<T, TResult>(this T[] array, int index, Func<T[], T[], TResult> selector) =>
        Splice(array, index, array.Length, selector);

    public static TResult Splice<T, TResult>(this T[] array, int index, int count, Func<T[], T[], TResult> selector) =>
        Splice(array, index, count, false, selector);

    static TResult Splice<T, TResult>(T[] array, int index, int count, bool withoutDeletions, Func<T[], T[], TResult> selector)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

        if (array.Length == 0)
            return selector(array, EmptyArray<T>.Value);

        if (index < 0)
            index = Math.Max(array.Length + index, 0);

        if (index >= array.Length || count == 0)
            return selector(array.Copy(), EmptyArray<T>.Value);

        var index2 = index + count;
        var length2 = Math.Max(array.Length - index2, 0);
        var splicedCount = Math.Max(index + length2, 0);

        if (splicedCount == 0)
            return selector(EmptyArray<T>.Value, array.Copy());

        var spliced = new T[splicedCount];
        Array.Copy(array, 0, spliced, 0, index);
        if (index2 < array.Length && length2 > 0)
            Array.Copy(array, index2, spliced, index, length2);

        if (withoutDeletions)
            return selector(spliced, null);

        var deletedCount = array.Length - spliced.Length;
        if (deletedCount == 0)
            return selector(spliced, EmptyArray<T>.Value);

        var deleted = new T[deletedCount];
        Array.Copy(array, index, deleted, 0, deleted.Length);
        return selector(spliced, deleted);
    }

    public static TResult Pop<T, TResult>(this T[] array, Func<T, T[], TResult> selector)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        if (array.Length == 0) throw new InvalidOperationException();
        return array.Pop(1, (tail, head) => selector(tail[0], head));
    }

    public static TResult Pop<T, TResult>(this T[] array, int count, Func<T[], T[], TResult> selector)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        return array.Splice(-count, count, (rest, popped) => selector(popped, rest));
    }


    public static TResult Shift<T, TResult>(this T[] array, Func<T, T[], TResult> selector)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        if (array.Length == 0) throw new InvalidOperationException();
        return array.Shift(1, (shifted, rest) => selector(shifted[0], rest));
    }

    public static TResult Shift<T, TResult>(this T[] array, int count, Func<T[], T[], TResult> selector)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        return array.Splice(0, count, (rest, shifted) => selector(shifted, rest));
    }

    public static T[] Copy<T>(this T[] array)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        return (T[])array.Clone();
    }

    static class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }
}














