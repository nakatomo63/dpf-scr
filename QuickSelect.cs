// Copyright 2010-2020 Nakagawa Tomoya
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


// quick selectとmedian of mediansアルゴリズムのList<T>に対する実装

using System;
using System.Collections.Generic;

using System.Diagnostics;

public class QuickSelect
{
	// list[a]とlist[b]を入れ替える
	private static void Swap<T>( List<T> list, int a, int b )
	{
		T temp = list[ a ];
		list[ a ] = list[ b ];
		list[ b ] = temp;
	}

	// list[begin,end)の範囲で、
	// pivot_value以下の値をbeginから順に集め、
	// 集めた部分の末尾の次の位置を返す（破壊的）。この位置の値はpivot_valueより大きい値となる。
	// pivot_valueがlist[begin,end)に含まれていなくても構わない
	// （実際にはPivot()で選んでくるので、必ず含まれていることになる）
	public static int Partition<T>( List<T> list, int begin, int end, T pivot_value )
		where T : IComparable
	{
		// 範囲ゼロなら何もしない
		if( begin == end )
		{
			return 0;
		}

		int stored = begin;
		int pivot_idx = -1;
		for( int i = begin; i < end; ++i )
		{
			// pivot_value以下の値を先頭の方に集める
			// （それらの値同士の間の大小関係は見ていないことに注意）
			var comp = list[ i ].CompareTo( pivot_value );
			if( comp <= 0 )
			{
				Swap( list, stored, i );
				// pivot_valueそのものなら、入れ替え後（リスト前半部分）の位置を覚えておく
				if( comp == 0 )
				{
					// 複数回該当してもOK 最後のものだけ覚えておけばいい
					pivot_idx = stored;
				}
				++stored;
			}
		}

		// pivot_valueが含まれていれば、（複数含まれていればどれか）一つをstoredの最後の位置に持ってくる
		if( pivot_idx >= 0 )
		{
			Debug.Assert( stored >= 1 && stored >= pivot_idx );
			Swap( list, pivot_idx, stored - 1 );
		}

		// list[stored,end) （＝後半部分）はpivot_valueより大きい値のみが含まれる
		return stored;
	}

	// Median of Mediansを使ってlist[begin,end)からpivotを求める
	// pivotはざっくり全体のmedianになる（確実にそうなるわけではないが近い値）
	private static T Pivot<T>( List<T> list, int begin, int end )
		where T : IComparable
	{
		int count = end - begin;

		// 5個以下なら要素数が小さいバージョンに飛ばす
		if( count <= 5 )
		{
			return SelectSmall( list, begin, end, count / 2 );
		}

		// 先頭から5要素ずつ区切り、その中での中央値を求める
		var representing = new List<T>();
		for( int i = begin; i < end; i += 5 )
		{
			int range_end = Math.Min( i + 5, end );
			representing.Add( SelectSmall( list, i, range_end, ( range_end - i ) / 2 ) );
		}

		// 5要素ずつの代表値（ざっくりmedian）の個数が非常に多ければ、それらに対してさらに同じ操作を実行する
		// 再帰しても10stepで10M elementsくらい処理できるため、loop展開はしない…。
		return Pivot( representing, 0, representing.Count );
	}

	// Selectの要素数が小さい配列に対する実装（n<=5）
	// kはlist全体のk番目ではなく、あくまで[begin,end)の中でのk番目
	private static T SelectSmall<T>( List<T> list, int begin, int end, int k )
		where T : IComparable
	{
		// 要素数が少ないので[begin,end)をソートしてk番目を求める
		list.Sort( begin, end - begin, null );
		return list[ begin + k ];
	}

	// list[begin,end)の範囲からk番目に小さい値を取得する
	public static T Select<T>( List<T> list, int begin, int end, int k )
		where T : IComparable
	{
		// 要素0個
		if( begin == end || list.Count == 0 )
		{
			throw new ArgumentException( "empty list" );
		}

		// 範囲がおかしい
		if( begin < 0 || end > list.Count )
		{
			throw new ArgumentException( "list index out of range" );
		}

		// 要素数が少ない場合はsortを使うものに飛ばす
		if( end - begin <= 5 )
		{
			return SelectSmall( list, begin, end, k - begin );
		}

		while( true )
		{
			//Debug.WriteLine( "list={" + String.Join( ",", list ) + "}\n[" + begin + "," + end + "),k=" + k );

			// pivotを選択する
			T pivot_value = Pivot( list, begin, end );
			// この値を注目点として値を並べ替える
			// pivot_valueは必ず前半部分の末尾に移動されている
			int pivot = Partition( list, begin, end, pivot_value ) - 1;

			// pivot_valueとして必ずlistに含まれている値から選んでくるため、あり得ないが一応確認する
			Debug.Assert( pivot >= 0 );

			// この時点でlist[0,pivot]の範囲にはpivot_value以下の値しか存在しない
			// （ソートされているわけではない）

			//Debug.WriteLine( "pivot value=" + pivot_value + ",index=" + pivot );

			// 発見？
			// （あるlistの内容をpivot_valueより大きいか、pivot_value以下かで分別したとき、pivot_value以下の値の個数がk個になった場合）
			if( pivot == k )
			{
				// list[k]はpivot_valueであるとは限らない？
				// （pivot_value以下の何かではある）
				// =>pivot_value未満の値をpivotより左側に全部集めたため、list[k]が正しい。
				//   （Partition()ではpivot_value自体を左側に集めてはいけない）
				return list[ k ];
			}
			// リストの左側の範囲を調べる
			else if( pivot > k )
			{
				// 探索範囲の末尾を狭める
				end = pivot;
			}
			// リストの左側は構わないので、右側を調べる
			else
			{
				// 探索範囲の先頭を狭める
				begin = pivot + 1;
			}
		}
	}

	// 全体から選ぶもの
	public static T Select<T>( List<T> list, int k )
		where T : IComparable
	{
		return Select( list, 0, list.Count, k );
	}

	// 中央値を選ぶ
	public static T Median<T>( List<T> list )
		where T : IComparable
	{
		return Select( list, list.Count / 2 );
	}
}
