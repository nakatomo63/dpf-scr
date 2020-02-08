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

# SVG����͂���ƁAsvg/g�i�ŏ��́j�̒��ɂ���path����Bezier�Ȑ��̐���_�̍��W���o�͂���B
# ����path��Bezier�Ȑ��݂̂ŕ`����Ă���iC or c���߂̂݁Bl�Ȃǂ́~�j
#   2020�N1��12�� m,M�i�p�����[�^��2�ȏ�j�Al,L,hHvV�ɑΉ����邱�Ƃɂ���...

# ���΍��W�͐�΍��W�ɂ���
# �����ʒu��(x0,y0)�Ƃ���B��������`���n�߂�B
# ���ׂĂ̍��W��scale_x,scale_y�{�����B
# �Ō��y_off����y���W����������B�i�㉺���]�p�j

# svg/g/path��d�����̒��g�͈ȉ��̌`���ƂȂ��Ă���
# m dx0,dy0 c dx1,dy1 dx2 dy2 dx3,dy3 dx4,dy4 dx5,dy5 ... z
# path�͏ォ��`���Ă����̂ŁA���ׂĂ̎n�_�`�I�_���W��H���Ă����Ȃ���
# ���̕���path�̐�΍��W�͂킩��Ȃ��Ƃ������ƂɂȂ�B

# M,m�͐�΂ł����΂ł��ς��Ȃ��B�������擪��1�̂݁B
# C,c�͐�΂Ƒ��΂���ʂ��Ĉ����A�悫�Ɍv�炤�B
# m��c�Ɛ��l�̊Ԃɂ̓X�y�[�X�����Ăق����B -> �Ȃ��Ă������悤�ɂ���

# �o�͂�Cubic Bezier Curve��Control Point�̗���

my ( $x_off, $y_off, $master_scale_x, $master_scale_y, $svgfile ) = @ARGV;

if( !defined( $svgfile ) )
{
	print STDERR $usage;
	exit( 0 );
}

# SVG�t�@�C���i���g��XML�j��ǂ�
my $xmlparser = XML::Simple->new( ForceArray => [ 'g', 'path' ], KeepRoot => 1 );
my $svg = $xmlparser->XMLin( $svgfile );

#print Dumper( $svg );

my %xml_g = %{$svg->{'svg'}->{'g'}};
# ��擾
my @layers = sort( keys( %xml_g ) );
my ( $id, $content ) = ( $layers[0], $xml_g{ $layers[0] } );

#print $id . "\n";
#print Dumper( $content );
print STDERR "Target Graphic: $id\n";

my %xml_path = %{$content->{'path'}};

# �i�ŏ��j�p�����[�^�����X�g
# x,y�ʂŃJ�E���g����
my %svg_param_count = (
	'm' => 2, 'M' => 2,
	'l' => 2, 'L' => 2,
	'h' => 1, 'H' => 1,
	'v' => 1, 'V' => 1,
	'c' => 6, 'C' => 6,
	'z' => 0, 'Z' => 0 );

# ���݂̈ʒu
# Inkscape��path�͊�{�I�ɂ��ׂĐ擪��mM�����Ă��邽�߁A
# �������W��O��path��������p���Ƃ������Ƃ��Ȃ��A�����
# �O���[�o���Ȍ��݂̈ʒu���o���Ă����K�v�͂Ȃ������B
my ( $x, $y, $dx, $dy ) = ( 0 ) x 4;

$x = $dx = $x_off * $master_scale_x;
$y = $dy = $y_off * $master_scale_y;

# �S�̂̊O�g�����߂����̂ō��W�̍ŏ��l�E�ő�l���o��
# �iBezier�̏ꍇ�͋Ȑ����u�͂ݏo��v�̂ŁA�����ɂ͊O�g�ɂȂ�Ȃ��j
my ( $x_min, $x_max );
my ( $y_min, $y_max );

$x_min = $y_min = ~0;
$x_max = $y_max = -( ~0 >> 1 ) - 1;


# line��(x0,y0)-(x1,y1)��^����Ɠ�����Bezier�Ȑ���(x0,y0)-(xc0,yc0)-(xc1,yc1)-(x1,y1)��Ԃ�
sub line_to_curve
{
	my ( $x0, $y0, $x1, $y1 ) = @_;
	
	# ����_�͎n�_-�I�_��3�����_�Ƃ���
	my ( $xc0, $yc0, $xc1, $yc1 );
	
	# 1:2�ɓ���...
	$xc0 = &roundzero( ( 2.0 * $x0 + $x1 ) / ( 1.0 + 2.0 ) );
	$yc0 = &roundzero( ( 2.0 * $y0 + $y1 ) / ( 1.0 + 2.0 ) );
	# 2:1��...
	$xc1 = &roundzero( ( $x0 + 2.0 * $x1 ) / ( 2.0 + 1.0 ) );
	$yc1 = &roundzero( ( $y0 + 2.0 * $y1 ) / ( 2.0 + 1.0 ) );
	
	return ( $x0, $y0, $xc0, $yc0, $xc1, $yc1, $x1, $y1 );
}

foreach my $key ( sort( keys( %xml_path ) ) )
{
	my $d = $xml_path{ $key }->{'d'};
	
	# opcode�����l�Ƃ������Ă��邱�Ƃ�����̂ŁA����
	$d =~ s/([mMlLhHvVcCzZ])/ $1 /g;
	
	# �]���ȃX�y�[�X���폜
	$d =~ s/\s+/ /g;
	$d =~ s/^\s+//;
	
	#print "$key: $d\n";
	#print Dumper split( /[\s,]+/, $d . " Z" );
	
	# scale���w�肳��Ă��邱�Ƃ�����...
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
	
	# �����I�ɓǂނƖʓ|�������̂ŁA�i���߁F�p�����[�^�j�̔z���pack���邱�Ƃɂ���
	my @operation = ();
	
	my $opname = "";
	my @param = ();
	
	# �Ƃ肠�������R�ɂ�x,y�̊Ԃɂ̓X�y�[�X���Ȃ����߁A\s�ŋ�؂邱�Ƃɂ���i�b��j
	# =>lineto�͌Â������Ȃ̂��A�X�y�[�X�ŏo���Ă��邱�Ƃ������B
	#   �d�����Ȃ��̂�(x,y)��g�݂ɂ����A�x�^�Ƀp�����[�^�Ɋi�[����
	# �Ō��token���������邽�߁A�]����Z����ǉ�����i����͏�������Ȃ��j
	foreach my $p ( split( /[\s,]+/, $d . " Z" ) )
	{
		#print "$p\n";
		
		# ���l�H
		if( $p =~ /^([\d\.\-\+eE]+)$/ )
		{
			my $a = $1 * 1.0;
			
			# ������ςȂ��琔���͂Ȃ�
			if( $opname eq "" )
			{
				print STDERR "No operator but with a parameter.\n";
				exit( 1 );
			}
			
			# ���炩�̃p�����[�^��荞�ݒ�
			# �����ƃp�����[�^���K�v
			if( $#param + 1 < $svg_param_count{ $opname } )
			{
				push( @param, $a );
			}
			# �\���H
			else
			{
				# hHvV�͘A���œ������i�Ӗ��Ȃ����ǁj
				# =>�����̖��߂ɕ���
				# [cClL]�̏ꍇ�͘A���œ������
				if( "hHvVcClL" =~ /$opname/ )
				{
					# ��������ۑ�����
					push( @operation, [ $opname, [ @param ] ] );
					# ��ɂ��čĊJ
					@param = ( $a );
				}
				# [mM]�ŃI�[�o�[�p�����[�^�̏ꍇ�A��Ԗڈȍ~��[lL]�Ɠ����Ӗ�
				elsif( "mM" =~ /$opname/ )
				{
					# ��������ۑ�����
					push( @operation, [ $opname, [ @param ] ] );
					# ��ɂ��čĊJ
					@param = ( $a );
					
					# opcode��ς���
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
			# �O��op����͒�
			if( $opname ne "" )
			{
				# �p�����[�^���������Ă���Εۑ�
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
	
	# �R�[�h�v�����[�O���o��
	my $myprolog = $code_prolog; $myprolog =~ s/\{\{id\}\}/$key/g;
	my $myepilog = $code_epilog; $myepilog =~ s/\{\{id\}\}/$key/g;
	
	print $myprolog;
	
	# ��͂������e�����s����i�C���^�v���^�j
	
	# path�̎n�_
	my ( $x_p0, $y_p0 ) = ( 0 ) x 2;
	
	foreach my $op ( @operation )
	{
		my ( $opname, $p ) = @{$op};
		my @param = @{$p};
		
		#print "$opname => " . Dumper( @param ) . "\n";
		
		if( $opname eq "m" || $opname eq "M" )
		{
			# ���ݍ��W�ړ��i��΂̂݁j
			$x_p0 = $x = &roundzero( $param[0] * $scale_x + $dx );
			$y_p0 = $y = &roundzero( $param[1] * $scale_y + $dy );
			
			# �������W�o��
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
			
			# ���݈ʒu�̍X�V
			$x_p0 = $x1;
			$y_p0 = $y1;
		}
		# cubic Bezier curve (relative)
		elsif( $opname eq "c" )
		{
			# ���΍��WBezier�Ȑ�
			# Bezier Curve�̐���_
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
			
			# ���݈ʒu�̍X�V
			$x_p0 = $x1;
			$y_p0 = $y1;
		}
		elsif( $opname eq "C" )
		{
			# ��΍��WBezier�Ȑ�
			# Bezier Curve�̐���_
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
			
			# ���݈ʒu�̍X�V
			$x_p0 = $x1;
			$y_p0 = $y1;
		}
		elsif( $opname eq "z" || $opname eq "Z" )
		{
			# ���݈ʒu�̍X�V�i���ǂ�j
			$x_p0 = $x;
			$y_p0 = $y;
		}
		
		# bounding box�̍X�V
		if( $x_p0 < $x_min ) { $x_min = $x_p0; }
		if( $x_p0 > $x_max ) { $x_max = $x_p0; }
		if( $y_p0 < $y_min ) { $y_min = $y_p0; }
		if( $y_p0 > $y_max ) { $y_max = $y_p0; }
	}
	
	print $myepilog;
}

print STDERR join( "\t", ( $x_min, $y_min, $x_max, $y_max ) );

exit( 0 );

