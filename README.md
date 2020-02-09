# dpf-scr
is named after *Digital Photo Frame Screen Saver*

## Primary Features
- Picture file selection
	- Recursive directory scan finds every files in sub directories.
	- サブディレクトリ内のファイルをすべて抽出できる。
	- File name filter performs regular expression test to choose your favorite photograph.
	- ファイル名を正規表現でmatchし，該当するファイルのみを表示する。
- Rotation mode
	- 4 mode to order pictures. By Name, by date, random, shuffle.
	- 画像の表示順は名前順，日付順，ランダム，シャッフルから選択することができる。
- 5 View mode for multiple display environment
- 表示モードは五種類から選べる。
	- *Center*
		- Place to the center of the target screen
		- *Center*は拡大縮小をせず，各画面の中央に画像を配置する。
	- *Tile*
		- Place to the top-left of the target screen. If the image is smaller than the screen area, the image is tiled.
		- *Tile*は画面の左上端から画像を配置する。画像が画面サイズより小さければ並べて配置する。
	- *Fit to screen*
		- Larger or smaller images are resized to fit to the screen.
		- *Fit to screen*は大きい画像や小さい画像を縮小して画面に収まるように配置する。
	- *Fit to screen (keep aspect ratio)*
		- It is equivalent to the previous option, except the mode keeps the image's aspect ratio.
		- *Fit to screen (keep aspect ratio)*は基本的にはひとつ前のオプションと同じだが，画像のアスペクト比を保持して縮小する。このとき，画像がcropされないように短い辺を基準とする。
	- *Intelligent Crop*
		- This mode is basically the same as the previous option. While *Fit to screen (keep aspect ratio)* option avoid being cropped, this option do not.
		- In order to this feature, screen black frame (border) are minimized.
		- *Intelligent Crop*も前項と同様に画像をリサイズするが，一部cropされることを許容して画面に余白が出ないように配置する。
- Background color can be chose.
	- 背景色は変更できる。
- Remove image border
	- 画像の左右に帯がついている場合，それを除去して描画できる。
- Target display selection
	- 画像を表示する画面を選べる。
	- *Single*
		- Display to onle one screen.
		- どれか一つの画面にのみ表示する。
	- *Clone*
		- Display to all screens.
		- すべての画面に表示する。各画面に合わせて表示モードの通りに描画される。
	- *Span*
		- Display to the whole screen as they are spanned to big one.
		- すべての画面を連結して一つの画面だとして描画する。画面がずれて配置されている場合には画像の一部が切れることがある。
- Exif information display
- 画像ファイルのExif情報を読み取り，主要な撮影データを表示することができる。
- Digital clock
- デジタル時計を表示することができる。
	- Its size and position can be adjusted.
	- 時計の大きさと表示位置が調整できる。
	- Its color can chose both manually or automatically. Automatic color choose method can detect representing color from the image currently viewed.
	- 時計の表示色は手動で決めることもできるし，自動で選択することもできる。自動色選択は，現在表示している画像の代表色を抽出して決定する。


## Target Platform
- .NET Framework 4.7.2

## Developing Environment
- Windows 10 1809 Build 17763.973
- Microsoft Visual Studio 2019 16.4.3
- C# 7

## Update History

### ver.0.15.13 2020/02/09
* [Chg] GitHubに登録した。Apache License 2.0とした。
* [Chg] 時計を表示しないときに代表色を求める処理をやめた
* [Fix] ClockMode==0（表示しない）のときにエラーが出るのを修正

### ver.0.14.12 2020/01/23
* [Chg] 代表色を2色抽出して時計とカレンダーにそれぞれ採用するようにした。彩度を調整した。
* [Chg] configのSizeにショートカットキーを追加。タブオーダーを修正。
* [Chg] 時計の垂直位置の設定を追加。
* [Fix] スライダの表示値がロード時に更新されていなかったのを修正。
* [Fix] サブディスプレイがメインディスプレイよりも上にある（サブディスプレイの左上隅のY座標が負）のとき、時計が真ん中に来ない現象を修正した。
### ver.0.13.11 2020/01/16
* [Chg] 代表色抽出の精度を上げた。
* [Fix] 前のバージョンから設定が引き継がれない問題を対策した
### ver.0.12.10 2020/01/13
* [Chg] 曜日を大文字にした。曜日の大きさを変えた。行間に区切り線を追加した。
### ver.0.11.9  2020/01/13
* [Add] デジタル時計に曜日(Sun,Mon,...)を追加した。
### ver.0.10.8  2020/01/11
* [Add] デジタル時計表示を追加した。時計は7segment表示の各segmentをBezier曲線で描画する。
	* 毎秒になった瞬間に表示が変わるよう、更新タイミングを工夫している。
	* 画像の代表色を検出で時計を描画するよう（Automatic Color）にした。代表色はHが画像全体の中央値、Sは画像全体の平均値、Vは固定値。
* [Chg] フレームロードを別スレッドに移して画面更新をスムーズにした。
* [Fix] マウスを動かして終了するとき、最初にほんの少し動かしてしまうと、再度大きく動かしても終了しないことがあった。
### ver.0.07 2019/11/24
* [Add] Crop表示モードを追加。画面の横幅・縦幅に合わせて拡大縮小するが、はみ出した部分はcropする。画像の縦長・横長に応じて適切に切り替える。
* [Add] 額縁除去モードを追加。横長の画像限定。画像の左端・右端の列の色が全部同じ一色である場合、そのような列を左右から除去して表示する。
* [Chg] Shuffuleモードでもう少しうまく混ざるようにした。
* [Chg] レンズの実焦点距離と35mm換算焦点距離が一致する場合、後者を表示しないようにした。
* [Chg] 露出補正値が±1EVぴったりの時に1/1ではなく1と表示するようにした。
### ver.0.06    2019/11/21
* [Fix] SystemParametersInfoAのDLLimportの仕方がおかしかったのを修正（Windows10で動かない）
### ver.0.05    2011/12/04
* [Add] Resume slideshow when Enter key is pressed or timed out. Timeout is equal to screen saver's one.
* [Add] Change to next file when Left/PageUp and Right/PageDown key is pressed, and the slideshow is paused.
* [Chg] Shutter speed calculation.
### ver.0.04    2010/04/13
* [Fix] TopMost form property set to false
* [Add] Load previous version settings when upgraded.
* [Add] Image resolutional 'Size' in information area
* [Chg] Image 'Size' to 'Original Size'
### ver.0.03    2010/04/11
* [Fix] Too much RAM is consumed until GC starts its work.
### ver.0.02    2010/04/02
* [Fix] ExposureBiasValue GCD Calculation
* [Fix] Default File Name Filter set to '.*'
### ver.0.01    2010/04/02
* First Version

[EOF]
