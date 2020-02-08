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


// List<T>関係のUtility

using System;
using System.Collections.Generic;
using System.Reflection;

using System.Diagnostics;

public class ListUtility
{
	private static T StaticField<T>( string name )
	{
		FieldInfo method = typeof( T ).GetField( name, BindingFlags.Public | BindingFlags.Static );
		if( method == null ) { throw new NotSupportedException( typeof( T ).Name ); }
		return ( T ) method.GetValue( null );
	}

	// 最大値のindexを返す（先頭のもの）
	public static int MaxPos<T>( List<T> list, int begin, int end )
		where T : struct, IComparable
	{
		int idx = 0;
		for( int i = begin; i < end; ++i )
		{
			if( list[ idx ].CompareTo( list[ i ] ) < 0 )
			{
				idx = i;
			}
		}

		return idx;
	}

	// 最小値の（同様）
	public static int MinPos<T>( List<T> list, int begin, int end )
		where T : struct, IComparable
	{
		int idx = 0;
		for( int i = begin; i < end; ++i )
		{
			if( list[ idx ].CompareTo( list[ i ] ) > 0 )
			{
				idx = i;
			}
		}

		return idx;
	}

	// 大津の二値化でthreshold（のindex）を求める
	// T = intに限定。。。
	public static int OtsuThresh( List<int> list, int begin, int end )
	{
		// threshより左の画素数（listの和）：ω1
		//           右の                 ：ω2
		// threshより左の平均値           ：m1
		//           右の                 ：m2
		// ω1ω2(m1-m2)^2 が最大になるthreshを求める

		int count = end - begin;
		Debug.Assert( count >= 1 );

		// 1個ならそこ！
		if( count == 1 )
		{
			return begin;
		}

		// 右側の合計値を出しておく
		double accum_right = 0;
		double sum_right = 0;
		for( int i = begin; i < end; ++i )
		{
			accum_right += i * ( double ) list[ i ];
			sum_right += list[ i ];
		}

		double accum_left = 0;
		double sum_left = 0;
		int thresh = 0;
		double sep_max = double.MinValue;

		// 右側が最低1個残るようにする
		for( int i = begin; i < end - 1; ++i )
		{
			// 左側の合計値を更新
			double accum = i * ( double ) list[ i ];
			accum_left += accum;
			sum_left += list[ i ];
			// 右側の
			accum_right -= accum;
			sum_right -= list[ i ];

			// 平均値を計算
			double avg_left = accum_left / sum_left;
			double avg_right = accum_right / sum_right;

			double separation = accum_left * accum_right * Math.Pow( avg_left - avg_right, 2 );

			if( separation > sep_max )
			{
				sep_max = separation;
				thresh = i;
			}
		}

		return thresh;
	}
}

