#!/usr/bin/perl

# Copyright 2010-2020 Nakagawa Tomoya
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# 
#     http://www.apache.org/licenses/LICENSE-2.0
# 
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

use strict;
use warnings;

use XML::Simple;
use Data::Dumper;

my $usage = << "USAGE";
perl svg2curve.pl x0 y0 scale_x scale_y bezier.svg > bezier.txt
and prints min(x),min(y),max(x),max(y)
USAGE

my $code_prolog = "public double[] {{id}} { get { return new double[] {\n";
my $code_epilog = "}; } }\n";

my $eps = 1e-5;

sub roundzero
{
	my ( $a ) = @_;
	if( abs( $a ) < $eps ) { return 0.0; }
	else { return $a; }
}

# SVGを入力すると、svg/g（最初の）の中にあるpathからBezier曲線の制御点の座標を出力する。
# このpathはBezier曲線のみで描かれている（C or c命令のみ。lなどは×）
#   2020年1月12日 m,M（パラメータ数2個以上）、l,L,hHvVに対応することにした...

# 相対座標は絶対座標にする
# 初期位置は(x0,y0)とする。ここから描き始める。
# すべての座標はscale_x,scale_y倍される。
# 最後にy_offだけy座標が加えられる。（上下反転用）

# svg/g/pathのd属性の中身は以下の形式となっている
# m dx0,dy0 c dx1,dy1 dx2 dy2 dx3,dy3 dx4,dy4 dx5,dy5 ... z
# pathは上から描いていくので、すべての始点～終点座標を辿っていかないと
# 後ろの方のpathの絶対座標はわからないということになる。

# M,mは絶対でも相対でも変わらない。ただし先頭に1個のみ。
# C,cは絶対と相対を区別して扱い、よきに計らう。
# mやcと数値の間にはスペースを入れてほしい。 -> なくてもいいようにした

# 出力はCubic Bezier CurveのControl Pointの羅列

my ( $x_off, $y_off, $master_scale_x, $master_scale_y, $svgfile ) = @ARGV;

if( !defined( $svgfile ) )
{
	print STDERR $usage;
	exit( 0 );
}

# SVGファイル（中身はXML）を読む
my $xmlparser = XML::Simple->new( ForceArray => [ 'g', 'path' ], KeepRoot => 1 );
my $svg = $xmlparser->XMLin( $svgfile );

#print Dumper( $svg );

my %xml_g = %{$svg->{'svg'}->{'g'}};
# 一つ取得
my @layers = sort( keys( %xml_g ) );
my ( $id, $content ) = ( $layers[0], $xml_g{ $layers[0] } );

#print $id . "\n";
#print Dumper( $content );
print STDERR "Target Graphic: $id\n";

my %xml_path = %{$content->{'path'}};

# （最少）パラメータ個数リスト
# x,y別でカウントする
my %svg_param_count = (
	'm' => 2, 'M' => 2,
	'l' => 2, 'L' => 2,
	'h' => 1, 'H' => 1,
	'v' => 1, 'V' => 1,
	'c' => 6, 'C' => 6,
	'z' => 0, 'Z' => 0 );

# 現在の位置
# Inkscapeのpathは基本的にすべて先頭にmMがついているため、
# 初期座標を前のpathから引き継ぐということがなく、よって
# グローバルな現在の位置を覚えておく必要はなさそう。
my ( $x, $y, $dx, $dy ) = ( 0 ) x 4;

$x = $dx = $x_off * $master_scale_x;
$y = $dy = $y_off * $master_scale_y;

# 全体の外枠を求めたいので座標の最小値・最大値を出す
# （Bezierの場合は曲線が「はみ出る」ので、厳密には外枠にならない）
my ( $x_min, $x_max );
my ( $y_min, $y_max );

$x_min = $y_min = ~0;
$x_max = $y_max = -( ~0 >> 1 ) - 1;


# lineの(x0,y0)-(x1,y1)を与えると等価なBezier曲線の(x0,y0)-(xc0,yc0)-(xc1,yc1)-(x1,y1)を返す
sub line_to_curve
{
	my ( $x0, $y0, $x1, $y1 ) = @_;
	
	# 制御点は始点-終点の3等分点とする
	my ( $xc0, $yc0, $xc1, $yc1 );
	
	# 1:2に内分...
	$xc0 = &roundzero( ( 2.0 * $x0 + $x1 ) / ( 1.0 + 2.0 ) );
	$yc0 = &roundzero( ( 2.0 * $y0 + $y1 ) / ( 1.0 + 2.0 ) );
	# 2:1に...
	$xc1 = &roundzero( ( $x0 + 2.0 * $x1 ) / ( 2.0 + 1.0 ) );
	$yc1 = &roundzero( ( $y0 + 2.0 * $y1 ) / ( 2.0 + 1.0 ) );
	
	return ( $x0, $y0, $xc0, $yc0, $xc1, $yc1, $x1, $y1 );
}

foreach my $key ( sort( keys( %xml_path ) ) )
{
	my $d = $xml_path{ $key }->{'d'};
	
	# opcodeが数値とくっついていることがあるので、離す
	$d =~ s/([mMlLhHvVcCzZ])/ $1 /g;
	
	# 余分なスペースを削除
	$d =~ s/\s+/ /g;
	$d =~ s/^\s+//;
	
	#print "$key: $d\n";
	#print Dumper split( /[\s,]+/, $d . " Z" );
	
	# scaleが指定されていることがある...
	my $transform = $xml_path{ $key }->{'transform'};
	my $scale = 1.0;
	my ( $scale_x, $scale_y ) = ( $master_scale_x, $master_scale_y );
	if( defined( $transform ) )
	{
		if( $transform =~ /^scale\(([\d\.\-\+eE]+)\)$/ )
		{
			$scale *= $1;
			$scale_x *= $scale;
			$scale_y *= $scale;
		}
		
		#print "transform=$transform, scale=$scale\n";
	}
	
	# 逐次的に読むと面倒くさいので、（命令：パラメータ）の配列にpackすることにする
	my @operation = ();
	
	my $opname = "";
	my @param = ();
	
	# とりあえず偶然にもx,yの間にはスペースがないため、\sで区切ることにする（暫定）
	# =>linetoは古い処理なのか、スペースで出してくることが判明。
	#   仕方がないので(x,y)を組みにせず、ベタにパラメータに格納する
	# 最後のtokenを処理するため、余分なZを一つ追加する（これは処理されない）
	foreach my $p ( split( /[\s,]+/, $d . " Z" ) )
	{
		#print "$p\n";
		
		# 数値？
		if( $p =~ /^([\d\.\-\+eE]+)$/ )
		{
			my $a = $1 * 1.0;
			
			# しょっぱなから数字はない
			if( $opname eq "" )
			{
				print STDERR "No operator but with a parameter.\n";
				exit( 1 );
			}
			
			# 何らかのパラメータ取り込み中
			# もっとパラメータが必要
			if( $#param + 1 < $svg_param_count{ $opname } )
			{
				push( @param, $a );
			}
			# 十分？
			else
			{
				# hHvVは連続で入れられる（意味ないけど）
				# =>複数の命令に分割
				# [cClL]の場合は連続で入れられる
				if( "hHvVcClL" =~ /$opname/ )
				{
					# いったん保存する
					push( @operation, [ $opname, [ @param ] ] );
					# 空にして再開
					@param = ( $a );
				}
				# [mM]でオーバーパラメータの場合、二番目以降は[lL]と同じ意味
				elsif( "mM" =~ /$opname/ )
				{
					# いったん保存する
					push( @operation, [ $opname, [ @param ] ] );
					# 空にして再開
					@param = ( $a );
					
					# opcodeを変える
					if( $opname eq "M" )
					{
						$opname = "L";
					}
					else
					{
						$opname = "l";
					}
				}
				else
				{
					print STDERR "Too much parameters for $opname\n";
					exit( 1 );
				}
			}
		}
		# operator found
		elsif( defined( $svg_param_count{ $p } ) )
		{
			# 前のopを解析中
			if( $opname ne "" )
			{
				# パラメータ数が合っていれば保存
				if( $#param + 1 == $svg_param_count{ $opname } )
				{
					push( @operation, [ $opname, [ @param ] ] );
				}
				else
				{
					print STDERR "Not enough parameters for $opname\n";
					#print Dumper( @operation );
					#print Dumper( @param );
					exit( 1 );
				}
			}
			
			$opname = $p;
			@param = ();
		}
		else
		{
			print STDERR "Invalid token '$p'\n";
			exit( 1 );
		}
	}
	
	#print Dumper( @operation );
	
	# コードプロローグを出力
	my $myprolog = $code_prolog; $myprolog =~ s/\{\{id\}\}/$key/g;
	my $myepilog = $code_epilog; $myepilog =~ s/\{\{id\}\}/$key/g;
	
	print $myprolog;
	
	# 解析した内容を実行する（インタプリタ）
	
	# pathの始点
	my ( $x_p0, $y_p0 ) = ( 0 ) x 2;
	
	foreach my $op ( @operation )
	{
		my ( $opname, $p ) = @{$op};
		my @param = @{$p};
		
		#print "$opname => " . Dumper( @param ) . "\n";
		
		if( $opname eq "m" || $opname eq "M" )
		{
			# 現在座標移動（絶対のみ）
			$x_p0 = $x = &roundzero( $param[0] * $scale_x + $dx );
			$y_p0 = $y = &roundzero( $param[1] * $scale_y + $dy );
			
			# 初期座標出力
			print( "$x_p0,$y_p0,\n" );
		}
		# lineto operators
		elsif( "lLhHvV" =~ /$opname/ )
		{
			my ( $xl1, $yl1 );
			
			# lineto (relative)
			if( $opname eq "l" )
			{
				$xl1 = $param[0] * $scale_x + $x_p0;
				$yl1 = $param[1] * $scale_y + $y_p0;
			}
			# lineto (absolute)
			elsif( $opname eq "L" )
			{
				$xl1 = $param[0] * $scale_x + $dx;
				$yl1 = $param[1] * $scale_y + $dy;
			}
			# horizontal lineto (relative)
			elsif( $opname eq "h" )
			{
				$xl1 = $param[0] * $scale_x + $x_p0;
				$yl1 = $y_p0;
			}
			# horizontal lineto (absolute)
			elsif( $opname eq "H" )
			{
				$xl1 = $param[0] * $scale_x + $dx;
				$yl1 = $y_p0;
			}
			# vertical lineto (relative)
			elsif( $opname eq "v" )
			{
				$xl1 = $x_p0;
				$yl1 = $param[0] * $scale_y + $y_p0;
			}
			# vertical lineto (absolute)
			elsif( $opname eq "V" )
			{
				$xl1 = $x_p0;
				$yl1 = $param[0] * $scale_y + $dy;
			}
			
			# convert straight line to cubic Bezier curve
			my ( $x0, $y0, $xc0, $yc0, $xc1, $yc1, $x1, $y1 )
				= &line_to_curve( $x_p0, $y_p0, $xl1, $yl1 );
			
			print( "$xc0,$yc0, $xc1,$yc1, $x1,$y1,\n" );
			
			# 現在位置の更新
			$x_p0 = $x1;
			$y_p0 = $y1;
		}
		# cubic Bezier curve (relative)
		elsif( $opname eq "c" )
		{
			# 相対座標Bezier曲線
			# Bezier Curveの制御点
			my ( $x0, $y0, $xc0, $yc0, $xc1, $yc1, $x1, $y1 ) = ( 0 ) x 8;
			
			$x0  = $x_p0;
			$y0  = $y_p0;
			$xc0 = &roundzero( $param[0] * $scale_x + $x0 );
			$yc0 = &roundzero( $param[1] * $scale_y + $y0 );
			$xc1 = &roundzero( $param[2] * $scale_x + $x0 );
			$yc1 = &roundzero( $param[3] * $scale_y + $y0 );
			$x1  = &roundzero( $param[4] * $scale_x + $x0 );
			$y1  = &roundzero( $param[5] * $scale_y + $y0 );
			
			print( "$xc0,$yc0, $xc1,$yc1, $x1,$y1,\n" );
			
			# 現在位置の更新
			$x_p0 = $x1;
			$y_p0 = $y1;
		}
		elsif( $opname eq "C" )
		{
			# 絶対座標Bezier曲線
			# Bezier Curveの制御点
			my ( $x0, $y0, $xc0, $yc0, $xc1, $yc1, $x1, $y1 ) = ( 0 ) x 8;
			
			$x0  = $x_p0;
			$y0  = $y_p0;
			$xc0 = &roundzero( $param[0] * $scale_x + $dx );
			$yc0 = &roundzero( $param[1] * $scale_y + $dy );
			$xc1 = &roundzero( $param[2] * $scale_x + $dx );
			$yc1 = &roundzero( $param[3] * $scale_y + $dy );
			$x1  = &roundzero( $param[4] * $scale_x + $dx );
			$y1  = &roundzero( $param[5] * $scale_y + $dy );
			
			print( "$xc0,$yc0, $xc1,$yc1, $x1,$y1,\n" );
			
			# 現在位置の更新
			$x_p0 = $x1;
			$y_p0 = $y1;
		}
		elsif( $opname eq "z" || $opname eq "Z" )
		{
			# 現在位置の更新（もどる）
			$x_p0 = $x;
			$y_p0 = $y;
		}
		
		# bounding boxの更新
		if( $x_p0 < $x_min ) { $x_min = $x_p0; }
		if( $x_p0 > $x_max ) { $x_max = $x_p0; }
		if( $y_p0 < $y_min ) { $y_min = $y_p0; }
		if( $y_p0 > $y_max ) { $y_max = $y_p0; }
	}
	
	print $myepilog;
}

print STDERR join( "\t", ( $x_min, $y_min, $x_max, $y_max ) );

exit( 0 );

